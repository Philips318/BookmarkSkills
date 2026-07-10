# Patient Safety Rules

## Classification (IEC 62304)

| Class | 描述 | Examples | Implication |
|-------|-------------|----------|-------------|
| **A** | No risk of injury | UI layout, cosmetic, report formatting | Basic verification |
| **B** | Non-serious injury possible | Performance degradation, workflow delay | Risk controls required |
| **C** | Death or serious injury possible | Dose, image quality, wrong patient, wrong side | Full risk management + V&V |

### Safety Escalation Triggers

如果 requirement touches 以下任一项，**must be classified Class C**：

- **Dose control** — CTDIvol/DLP thresholds、Dose Check、ACS、DRI
- **Image diagnostic quality** — HU accuracy、spatial accuracy、artifact suppression
- **Patient identification** — Patient ID、name、matching between systems
- **Wrong-side / wrong-patient** — Laterality、orientation、position markers
- **Radiation interlock** — X-ray on/off、exposure timing
- **Emergency stop** — Motion control、gantry stop、table stop
- **Data integrity** — DICOM UID consistency、lossless pixel data
- **Contrast injection** — timing、volume、rate
- **Spectral quantification** — MonoE keV accuracy、iodine mg/ml、Z Effective

## Radiation Safety Red Lines

| 规则 | 理由 |
|------|-----------|
| Dose Check **must** be active and cannot be silently disabled | Prevent accidental overexposure |
| Dose Alert requires operator **acknowledgement** before scan proceeds | Hard stop for protocol errors |
| Pediatric patients (< 70 kg) must use pediatric exam cards | Children are 10× more radiation-sensitive |
| Surview images are NOT diagnostic quality | Lower dose, different geometry |
| DoseRight/ACS calculates tube current from patient size | Auto dose optimization |
| kVp selection affects dose non-linearly（120→80 kVp ≈ 50% dose reduction） | Protocol optimization |

## Data Integrity Red Lines

| 规则 | 理由 |
|------|-----------|
| Patient Name+ID must be consistent across all series in a study | Wrong patient data → wrong diagnosis |
| UID uniqueness: each SOPInstanceUID must be globally unique | Duplicate → overwrite/confusion in PACS |
| Annotations must be preserved during all operations | Loss → diagnostic information lost |
| Pixel data must use **lossless** compression for post-processing | Lossy → HU inaccuracy → wrong measurement |
| RescaleSlope/Intercept must match actual pixel values | Wrong HU mapping → wrong diagnosis |
| Storage Commitment must be confirmed before local deletion | Data loss prevention |

## Spectral-Specific Safety Rules

| 规则 | 理由 |
|------|-----------|
| VNC recommended for **body scans only** | Head VNC has higher inaccuracy |
| VNC iodine removal optimal at < 10 mg/ml（100 kVp）/ < 20 mg/ml（120 kVp） | Beyond limits: incomplete iodine removal |
| MonoE HU values **vary with keV** — users must be trained | Low keV: iodine bright；High keV: beam hardening reduced |
| MonoE < 60 keV: water HU variation may exceed **±8 HU** | Exceeds conventional CT tolerance |
| MonoE at edges（40, 200 keV）: darkening/brightening artifacts | Known limitation |
| Iodine quantification accuracy reduced below **5 mg/ml** | Below detection threshold |
| Uric Acid: stones < 3 mm may be **missed** at typical abdomen dose | Resolution limitation |
| Non-HU images saved as **RGB by default** | Prevent wrong "HU" reading on third-party viewers |
| EFOV spectral results limited to **500 mm** | HU/geometry unreliable beyond 500 mm |

## Console Security Rules (from SSRS)

| 规则 | 理由 |
|------|-----------|
| Service login requires **IST key + password** | Physical + knowledge factor |
| Emergency login: no password, but limited to **5 exams**, then forced re-login | Safety net for critical situations |
| Clinical users blocked from Service UI and OS applications | Prevent unqualified configuration changes |
| Login/logout events must be **logged** | Audit trail for IEC 62304 / ISO 13485 |
| Auto screen blank after timeout | Prevent unauthorized access to patient data |
| DICOM de-identification must delete all private tags | Prevent PHI leakage |
| IVC (Image Viewing Computer) accepts only authorized network communication | Network security |

## Performance Safety Thresholds (from SSRS)

| 指标 | 阈值 | Safety Implication |
|--------|-----------|-------------------|
| Console init | ≤ 226 s | Emergency availability |
| Image handling | ≤ 75 ms per image | Real-time review during intervention |
| Preview image | ≤ 150 ms | Workflow efficiency |
| Couch precision | 0.5 mm increments | Spatial accuracy for treatment |
| X-ray indicator | During exposure | Operator/bystander awareness |

## Requirement Writing Checklist for Safety

编写涉及 patient safety 的 requirement 时：

- [ ] 是否明确说明 **safety class**（A/B/C）？
- [ ] 是否识别 **failure modes**？（如果 feature fails 会发生什么？）
- [ ] 是否用 explicit units 定义 **boundary values**？
- [ ] 是否 specified **error handling**？（invalid input 时 system 做什么？）
- [ ] 是否定义 **user notification**？（operator 是否会收到 alert？）
- [ ] 是否 specified **undo/recovery** paths？
- [ ] 是否在 system boundaries 要求 **data validation**？
- [ ] 是否有能 demonstrate safety 的 **verification method**？
- [ ] Requirement 是否 **reference applicable standards**（IEC 62304、IEC 60601）？
- [ ] Requirement 是否 **testable** — tester 能否 unambiguously determine pass/fail？

## ISP (Post-Processing) Specific Safety Rules

| 规则 | 理由 |
|------|-----------|
| **NOT for mammography** | Indications explicitly exclude mammography use |
| **NOT for diagnosis of lossy-compressed images** | Lossy compression alters pixel values → diagnostic risk |
| User responsible for assessing image quality before review | Lossless and lossy images both displayable — user must judge fitness |
| Citrix/virtualization may **degrade image quality / skip frames** | Network/VM bandwidth dependency |
| Multi-phase auto-alignment may **reduce image quality** | When phases have different FOV/matrix/zoom |
| Clipboard copy may leak PHI | User must clear Windows clipboard explicitly |
| Data deletion restricted to **site administrator only** | Prevent accidental clinical data loss |
| De-identification creates new study, **original retained** | Prevents data loss；admin must delete original separately |
| Change Patient Details creates new study, **original retained** | Audit trail preservation |
| Session **time-out** logs user off after configured idle period | Prevent unauthorized PHI access |
| Client and Server software versions must match | Prevent data corruption from protocol mismatch |
| Patient Disk includes embedded Philips DICOM Viewer | Recipient does not need own viewer；reduces interop risk |
| Use of **incompatible workstation** for image transfer is user/manufacturer responsibility | Philips not liable for third-party display errors |
| Improperly operating components → potential **fatal injury** (explicit IFU warning) | Must verify system health before clinical use |
