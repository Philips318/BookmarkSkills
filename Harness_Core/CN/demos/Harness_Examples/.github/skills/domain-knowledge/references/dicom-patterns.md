# DICOM Patterns & Pitfalls

## Key Tags Every Analyst Must Know

| 标签 | Keyword | VR/VM | 用途 |
|-----|---------|-------|---------|
| (0008,0016) | SOPClassUID | UI/1 | Object type — CT = `1.2.840.10008.5.1.4.1.1.2` |
| (0008,0018) | SOPInstanceUID | UI/1 | Globally unique instance identity |
| (0008,0060) | Modality | CS/1 | `CT` / `MR` / `US` / `DX` |
| (0008,0008) | ImageType | CS/2-n | `ORIGINAL\PRIMARY\AXIAL` — multi-value attribute |
| (0010,0010) | PatientName | PN/1 | Format: `Family^Given` |
| (0010,0020) | PatientID | LO/1 | Patient identifier |
| (0020,000D) | StudyInstanceUID | UI/1 | Unique per study |
| (0020,000E) | SeriesInstanceUID | UI/1 | Unique per series |
| (0020,0032) | ImagePositionPatient | DS/3 | patient coordinates 中的 top-left pixel（x\y\z） |
| (0020,0037) | ImageOrientationPatient | DS/6 | Row + Column direction cosines |
| (0028,0010) | Rows | US/1 | Pixel matrix height（通常 512） |
| (0028,0011) | Columns | US/1 | Pixel matrix width（通常 512） |
| (0028,0030) | PixelSpacing | DS/2 | Physical mm per pixel（`row\col`） |
| (0028,1050) | WindowCenter | DS/1-n | Display brightness center |
| (0028,1051) | WindowWidth | DS/1-n | Display contrast range |
| (0028,1052) | RescaleIntercept | DS/1 | Pixel→HU conversion offset |
| (0028,1053) | RescaleSlope | DS/1 | Pixel→HU conversion multiplier |
| (0018,0050) | SliceThickness | DS/1 | Acquisition collimation width（mm） |
| (0018,0088) | SpacingBetweenSlices | DS/1 | slice centers 之间的 distance（mm） |
| (0018,5100) | PatientPosition | CS/1 | HFS/HFP/FFS/FFP/HFDR/HFDL/FFDR/FFDL |

## Value Representation (VR) Types

| VR | 全称 | Max Len | 用途 |
|----|-----------|---------|-----|
| AE | Application Entity | 16 | AE Titles |
| CS | Code String | 16 | Enumerated values |
| DA | Date | 8 | YYYYMMDD |
| DS | Decimal String | 16 | Numeric as string |
| IS | Integer String | 12 | Integer as string |
| LO | Long String | 64 | Free text labels |
| OB/OW | Other Byte/Word | variable | Pixel data |
| PN | Person Name | 64×5 | `Family^Given`（5 component groups） |
| SQ | Sequence | variable | Nested datasets |
| UI | Unique Identifier | 64 | OID-format UIDs |
| US/SS | Unsigned/Signed Short | 2 bytes | 16-bit integers |

## Pixel Display Pipeline

```
Raw Pixels (16-bit) → Rescale (HU = pixel × Slope + Intercept) → Window → Quantize (0-255)
```

**Window formula:** `gray = clamp((HU - (WC - WW/2)) / WW, 0, 1)`

## Transfer Syntax

| 名称 | UID | Compression | 备注 |
|------|-----|-------------|-------|
| Implicit VR LE | 1.2.840.10008.1.2 | 无 | Default, all devices must support |
| Explicit VR LE | 1.2.840.10008.1.2.1 | 无 | Recommended, debug-friendly |
| JPEG Lossless SV1 | ...4.70 | Lossless | Required for CT post-processing |
| JPEG 2000 Lossless | ...4.90 | Lossless | Modern preferred |

**Rule:** CT post-processing 必须使用 **lossless** transfer syntax 以 preserve HU precision。

## DIMSE Services

| Service | 用途 | Typical Use |
|---------|---------|-------------|
| C-ECHO | Verify connectivity | Heartbeat / ping |
| C-STORE | Send one SOP instance | Image transfer to PACS |
| C-FIND | Query | Search by patient/study/series |
| C-MOVE | Cross-AE retrieval | Request images to 3rd party |
| C-GET | Same-session retrieval | Pull images on current association |
| N-CREATE | Create management object | MPPS: create procedure step |
| N-SET | Update management object | MPPS: IN PROGRESS → COMPLETED |
| N-ACTION | Trigger action | Storage Commitment request |
| N-EVENT-REPORT | Event notification | Storage Commitment confirmation |

## UID Structure

- Format: dot-separated digits，**max 64 chars**
- Philips root: `1.3.46.670589`
- CT Image Storage: `1.2.840.10008.5.1.4.1.1.2`
- Separator: backslash `\` for multi-value elements

## Common Pitfalls

| Pitfall | 影响 | Fix |
|---------|--------|-----|
| Implicit VR + Private Tags | Cannot determine VR → parse failure | Always use Explicit VR |
| MONOCHROME1 vs MONOCHROME2 | Inverted display（bright↔dark） | Always check PhotometricInterpretation |
| SliceThickness ≠ SpacingBetweenSlices | Gap/overlap miscalculation | Use IPP Z-difference for actual spacing |
| Missing RescaleIntercept | Wrong HU values → wrong diagnosis | Always validate before spectral calc |
| WW = 0 | Division by zero in display | Validate WW > 0 |
| RGB vs MONOCHROME2 for non-HU | Third-party viewers show wrong "HU" | Save non-HU as RGB with unit warning |
| US vs SS (PixelRepresentation) | Unsigned vs signed pixel data | CT uses signed（HU has negatives） |
| Multi-value separator `\` | Parsing error if treated as escape | DICOM uses `\`, not `/` |

## ISP Interoperability Tolerances (from SSRS)

| Parameter | Tolerance |
|-----------|-----------|
| PixelSpacing delta | ≤ 0.01 mm |
| ImageOrientation delta | ≤ 0.01 |
| Z-spacing delta | ≤ 0.2 mm |

## Private Tag Rules

- Odd group numbers are private（例如 `01F1`、`7005`）
- Elements `(gg,0010)`–`(gg,00FF)` 是 creator reservation slots
- 每个 creator owns 256 tags
- Anonymization 必须 delete all private tags（may leak info）
- Private tags under Implicit VR are unparseable

## Troubleshooting — Four-Quadrant Method

| Quadrant | 检查项 |
|----------|-------|
| 1. Configuration | AE Title, IP, Port, Called AE |
| 2. Data | UID correctness, spatial tags, compression format |
| 3. Protocol | Association Reject, Presentation Context, DIMSE status |
| 4. Remote Capability | Does remote support the Transfer Syntax / SOP Class? |
