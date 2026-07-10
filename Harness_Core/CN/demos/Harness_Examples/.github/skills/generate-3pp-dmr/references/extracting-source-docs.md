# Extracting Source Documents (CIP / TRR / Supplier)

`generate-3pp-dmr` skill 的 companion reference。Inputs 是 Word `.docx`（CIP、TRR），且经常包括 PDF（operator manuals、510(k) summaries）。用它从文档中提取 structured text 和 tables。

## Word `.docx` — paragraphs + tables (recommended)

使用 helper script [Extract-SourceDoc.ps1](./Extract-SourceDoc.ps1)。它会以 read-only 方式通过 Word COM 打开 document，dump 每个 paragraph 及其 style name，并逐行 dump 每张 table（对 vertically merged cells 提供 fallback；CIP tables 中常见）。

```powershell
powershell -ExecutionPolicy Bypass -File `
  ".github/skills/generate-3pp-dmr/references/Extract-SourceDoc.ps1" `
  -DocPath "E:\3PP\Taichi CIP\CR244787 CIP.docx" `
  -OutPath ".\_dmr_work\CIP.txt"
```

然后读取生成的 `.txt`。Paragraph styles（`[Heading 1]`、`[Heading 2]`、`[Normal]`）揭示 section structure；tables 会打印为 `R{n}: cell | cell | cell`。

### What to look for

**In the CIP:**
- Section "Product Identification" — third-party models ↔ Philips products。
- "Affected Sites, Modalities, Products" — Philips product numbers。
- "Purpose of Change" — CR number and release intent。

**In the TRR:**
- "Scope" — 两张 tables：Philips CT systems（+6NC）和 Injector/Device products（Manufacture | Device Description | Device Models）。
- "Interface Confirmation" — interface type（CANOpen CiA 425 / SAS）。
- "References" / title block — TRR 自身的 Document ID + revision。

## Merged cells

CIP tables 经常使用 vertically merged cells。逐行访问会抛出 "Cannot access individual rows … vertically merged cells"。helper script 会捕获并 fallback 到 linear cell dump（`C[row,col]: text`）。从 row/column indices 重建 logical rows。

## PDF inputs (operator manuals, 510(k) summaries)

这些通常是 **appendices**，而不是 data sources — 一般只需要它们的 **titles** 用于 Appendices 和 References sections，不需要 full text。如果需要 specific functionality wording，用 PDF text tool 读取相关前几页；不要尝试 ingest entire multi-MB manuals。

## Privacy / confidentiality

这些 documents 是 Philips confidential，且可能包含 named individuals（review participants、authors）。只把 DMR template 需要的信息带入 DMR（revision history 中的 author name、manufacturer identification）。不要复制 unrelated personal data。
