---
name: Generate 3PP DMR
description: 'Draft a Device Master Record (DMR) Word document for a Third Party Product (3PP) accessory sold with Philips CT/AMI systems. Takes a CIP, TRR, and optional supplier/IFU/spec/website inputs and produces a standard-template .docx DMR.'
argument-hint: 'Provide paths to the CIP and TRR document(s), plus any supplier docs / IFU / spec sheets / website snapshots'
agent: 'agent'
tools: ['read_file', 'create_file', 'replace_string_in_file', 'list_dir', 'grep_search', 'run_in_terminal']
---

Draft a **Device Master Record (DMR)** Word document for a **Third Party Product (3PP)** accessory (injector, gating system, adapter, etc.) released with a Philips 12NC and sold with Philips CT/AMI systems.

Use these repository assets as the source of structure, mapping rules, boilerplate, and the Word builder:

- [DMR Structure](../skills/generate-3pp-dmr/references/dmr-structure.md) — per-section specification + checklist
- [Input Mapping](../skills/generate-3pp-dmr/references/input-mapping.md) — CIP / TRR / supplier → DMR field map
- [Boilerplate Text](../skills/generate-3pp-dmr/references/boilerplate-text.md) — exact standard wording (verbatim)
- [Extracting Source Docs](../skills/generate-3pp-dmr/references/extracting-source-docs.md) — how to read CIP/TRR .docx
- [Extract-SourceDoc.ps1](../skills/generate-3pp-dmr/references/Extract-SourceDoc.ps1) — Word→text extractor (merged-cell safe)
- [Build-DMR-Word.ps1](../skills/generate-3pp-dmr/references/Build-DMR-Word.ps1) — JSON→.docx DMR builder
- Reference folder of finished DMRs (style/structure): `E:\3PP\Taichi DMR Documents`

## Context

This is a **Philips CT/AMI** third-party-product release. A 3PP DMR is a logistical/regulatory record, **not** an in-house design document: design, quality, and robustness are the third-party manufacturer's responsibility, so most sections use standard N/A wording. The real content is product/model identification (12NC), manufacturer details, functionality, compatibility references, affected Philips products, and references to the Technical Review Report(s).

## Required Inputs (ask the user)

1. **CIP** — Change Request / Change Plan (required). Example folder: `E:\3PP\Taichi CIP`.
2. **TRR** — Technical Review Report(s) (required). Example folder: `E:\3PP\Taichi TRR`.
3. **Supplier documentation / operator & service manuals** (optional) — functionality + installation + appendices.
4. **IFU** (optional) — intended use / functionality wording.
5. **Specification sheets** (optional) — model numbers + manufacturer P/N + 12NC mapping.
6. **Product website snapshots** (optional) — supplementary product identification.

If the CIP or TRR is not provided, ask for it before proceeding.

## Workflow

1. **Confirm inputs**: Get the CIP and TRR paths (and any optional material). If given a folder, identify the relevant files.
2. **Extract**: Run `Extract-SourceDoc.ps1` on each `.docx` to dump paragraphs (with styles) and tables to a `_dmr_work/` text file, then read them. Watch for merged cells in CIP tables.
3. **Map**: Using [Input Mapping](../skills/generate-3pp-dmr/references/input-mapping.md), pull product/model identification, 12NCs, manufacturer + P/N, affected Philips products + numbers, interface type, functionality, and TRR Document IDs + revisions. Mark anything missing `[TBD]`.
4. **Confirm identity**: Determine DMR Document ID, product title, 12NC(s), author, CR/CN, and revision (ask the user if not in the inputs). Initial release → Revision A.
5. **Assemble JSON**: Produce the data JSON matching the schema in `Build-DMR-Word.ps1` (use exact boilerplate from [Boilerplate Text](../skills/generate-3pp-dmr/references/boilerplate-text.md) for the dynamic sentences; the script supplies the fixed N/A text).
6. **Build**: Run `Build-DMR-Word.ps1 -DataPath <json> -OutPath <docx>` to generate the DMR.
7. **Self-check**: Validate against the [DMR checklist](../skills/generate-3pp-dmr/references/dmr-structure.md#checklist).
8. **Deliver**: Save the `.docx` and state it is an AI-generated draft requiring Systems Engineering review and the standard PLM release/approval workflow.

## Output

A Microsoft Word `.docx` DMR following the standard template (title, TOC, 9 sections, all data tables), named `D00XXXXXXX <Manufacturer> <Product> <12NC>.docx`. Unknown values appear as visible `[TBD]` placeholders.

## Rules

- **This is a draft** — always communicate the review/approval disclaimer.
- **Never invent** 12NCs, model numbers, product numbers, manufacturer P/N, or document IDs — copy verbatim from the inputs; unknown → `[TBD]`.
- **Use boilerplate verbatim** — do not paraphrase the Purpose/Scope/N/A/confidentiality wording.
- **Third-party responsibility** — keep N/A phrasing for architecture, quality aspects, detailed design, design constraints, and robustness.
- **Compatibility & installation are references** to appendices / supplier service manual, not restated content.
- **References table** must list the TRR(s) with the correct Document ID + revision.
- **One product family per DMR.**
