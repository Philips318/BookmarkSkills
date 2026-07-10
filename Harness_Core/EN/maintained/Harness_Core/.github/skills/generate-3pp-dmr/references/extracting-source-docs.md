# Extracting Source Documents (CIP / TRR / Supplier)

Companion reference for the `generate-3pp-dmr` skill. The inputs are Word `.docx` (CIP, TRR) and often PDF (operator manuals, 510(k) summaries). Use this to pull structured text and tables out of them.

## Word `.docx` — paragraphs + tables (recommended)

Use the helper script [Extract-SourceDoc.ps1](./Extract-SourceDoc.ps1). It opens the document read-only with Word COM, dumps every paragraph with its style name, and dumps each table row-by-row (with a fallback for vertically merged cells, which appear in CIP tables).

```powershell
powershell -ExecutionPolicy Bypass -File `
  ".github/skills/generate-3pp-dmr/references/Extract-SourceDoc.ps1" `
  -DocPath "E:\3PP\Taichi CIP\CR244787 CIP.docx" `
  -OutPath ".\_dmr_work\CIP.txt"
```

Then read the resulting `.txt`. Paragraph styles (`[Heading 1]`, `[Heading 2]`, `[Normal]`) reveal the section structure; tables are printed as `R{n}: cell | cell | cell`.

### What to look for

**In the CIP:**
- Section "Product Identification" — third-party models ↔ Philips products.
- "Affected Sites, Modalities, Products" — Philips product numbers.
- "Purpose of Change" — CR number and release intent.

**In the TRR:**
- "Scope" — two tables: Philips CT systems (+6NC) and Injector/Device products (Manufacture | Device Description | Device Models).
- "Interface Confirmation" — interface type (CANOpen CiA 425 / SAS).
- "References" / title block — the TRR's own Document ID + revision.

## Merged cells

CIP tables frequently use vertically merged cells. Row-by-row access throws "Cannot access individual rows … vertically merged cells". The helper script catches this and falls back to a linear cell dump (`C[row,col]: text`). Reconstruct the logical rows from the row/column indices.

## PDF inputs (operator manuals, 510(k) summaries)

These are usually **appendices** rather than data sources — you normally only need their **titles** for the Appendices and References sections, not their full text. If specific functionality wording is needed, read the first relevant pages with a PDF text tool; do not attempt to ingest entire multi-MB manuals.

## Privacy / confidentiality

These documents are Philips confidential and may contain named individuals (review participants, authors). Only carry into the DMR what the template requires (author name in revision history, manufacturer identification). Do not copy unrelated personal data.
