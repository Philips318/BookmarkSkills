---
name: Product Defect Analysis
description: 'Fill in the Investigated section of a Product Defect record (e.g. CTD-style) for CT medical device software, from a Code Review Report and SSRS/SRS requirements. Produces a self-contained HTML report covering all 11 Investigated fields.'
argument-hint: 'Provide the Code Review Report (path or text) and the SSRS/SRS requirements (path or text); optionally the Product Defect ID, title, and related defect IDs'
agent: 'agent'
---

Complete the **Investigated** section of a Product Defect record and generate a **self-contained HTML report**.

Use these repository assets as the source of method, field guidance, and template:

- [Skill definition](../skills/productdefect-analysis/SKILL.md)
- [Field Guide](../skills/productdefect-analysis/references/field-guide.md)
- [Report Template](../skills/productdefect-analysis/references/report-template.html)

## Context

This is a **Philips CT medical device** Product Defect investigation under IEC 62304 / ISO 13485. The
analysis must trace the defect to a real failing SSRS/SRS requirement and keep the technical answers
consistent with the Code Review Report. **Never invent** a requirement ID, release number, or root
cause — mark non-derivable values as "to confirm".

## Inputs

- **Code Review Report**: path or pasted content (root cause, fix, changed files, regression risk,
  test impact, residual risks, commit log). Read an HTML report via `fetch_webpage` with the
  `file://` URL, or `read_file`.
- **SSRS / SRS requirements**: a document path (`.doc`/`.docx`) or pasted text. Extract the failing
  `Requirement:` text, `Requirement ID:`, `Product:`, and `Target Release:` using a PowerShell Word
  COM one-liner (see the skill).
- **Product Defect ID + title** (optional) and **related defect IDs** that share the root cause.

If any input is missing, ask for it before generating.

## Workflow

1. **Parse the Code Review Report**: extract symptom, root cause, fix, changed files, regression risk,
   test impact, residual risks.
2. **Parse the SSRS/SRS**: locate the failing requirement (ID + text), affected products, target release.
3. **Map shared root causes**: if related defect IDs are given, confirm they share the root cause and
   note that one fix resolves all; recommend linking duplicates.
4. **Fill the 11 fields** per the [Field Guide](../skills/productdefect-analysis/references/field-guide.md):
   1. Customer impact + work-around
   2. How to recover from failure mode
   3. Root cause-analysis
   4. Installed-base yes/no + oldest release + affected configurations
   5. Frequency of occurrence (Occurs every time / May occur / Not expected to occur)
   6. Proposed solution + technical risk + reliability impact (Change Point Analysis)
   7. DHF/DMR documents to create or modify
   8. Is testing required (reference the regression test + manual re-execution)
   9. IFU / SMI update required
   10. Investigator's advice
   11. Identify/Confirm failing product requirement (SSRS/SRS ID + text)
5. **Build the HTML report** from the
   [report template](../skills/productdefect-analysis/references/report-template.html): replace every
   `{{PLACEHOLDER}}`; render the frequency as a highlighted selection; use yes/no pills for installed
   base and IFU; add a "to confirm" note for any non-derivable value.
6. **Save the HTML file**: write to `PD Analysis/productdefect-analysis-{DEFECT_ID}.html` under the
   workspace root (create the `PD Analysis` directory if needed), unless the user explicitly specifies
   another output path.
7. **Summarize**: report the file path and list any "to confirm" items the investigator must verify.

## Output

A single self-contained HTML file (light theme, inline CSS, no external assets) with one titled
section per Investigated field, plus a footnote citing the Code Review Report and SSRS references.
Default save location: workspace root `PD Analysis/` directory, with filename
`productdefect-analysis-{DEFECT_ID}.html` unless the user explicitly requests another path.
