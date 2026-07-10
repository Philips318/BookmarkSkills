---
name: productdefect-analysis
description: 'Generate a Product Defect Analysis (the "Investigated" section of a Product Defect record, e.g. CTD-style defects) for CT medical device software. Takes a Code Review Report and SSRS/SRS requirements as input and produces a structured, self-contained HTML report covering customer impact, recovery, root-cause analysis, installed-base exposure, frequency, proposed solution, DHF/DMR impact, testing, IFU/SMI impact, investigator advice, and the failing product requirement.'
argument-hint: 'Provide the Code Review Report (path or text) and the SSRS/SRS requirements (path or text); optionally the Product Defect ID and title'
user-invocable: true
---

# Product Defect Analysis

Use this skill to fill in the **Investigated** section of a Product Defect record for CT medical
device software. Given a **Code Review Report** (the technical root cause + fix) and the
**SSRS/SRS requirements** (the requirement that is being violated), it produces a complete,
audit-ready Product Defect Analysis and delivers it as a **self-contained HTML report**.

## When to Use

- A code review has identified the root cause and fix for a defect, and you must complete the
  Product Defect "Investigated" form for quality/regulatory sign-off.
- Multiple defect tickets share one root cause and each needs its own filled-in analysis.
- You need to trace a defect to a specific failing SSRS/SRS requirement ID.
- You want a consistent, reviewer-friendly HTML deliverable instead of free-text answers.

## Preferred Inputs

Provide one or more of the following:

- **Code Review Report**: an HTML/Markdown/text report (path or pasted content). Source of the
  root cause, the fix, changed files, regression risk, test impact, and commit log.
- **SSRS / SRS requirements**: a requirements document path (e.g. a `.doc`/`.docx` SSRS) or pasted
  requirement text. Source of the **failing product requirement** (ID + text), affected products,
  and target release.
- **Product Defect ID + title** (optional): e.g. `CTD00025333 — 2 polyps were marked each time`.
- **Related defect IDs** (optional): other tickets that share the same root cause.

If an input is missing, ask for it before generating. Do not invent a root cause or a requirement ID.

## Reading the Inputs

- **Code Review Report (HTML)**: use `fetch_webpage` with the `file://` URL, or `read_file`. Extract:
  problem background / symptom, root cause, changed files, the fix description/code, regression risk,
  test impact (existing/new tests), residual risks, and the commit log.
- **SSRS/SRS (.doc/.docx)**: extract via a PowerShell Word COM one-liner, then search for the
  feature keyword to locate the `Requirement:` text, `Requirement ID:`, `Product:`, and
  `Target Release:` lines. Example:

  ```powershell
  $word = New-Object -ComObject Word.Application; $word.Visible=$false
  $doc = $word.Documents.Open("<SSRS path>",$false,$true)
  $text = $doc.Content.Text; $doc.Close($false); $word.Quit()
  $lines = $text -split "`r"
  for($i=0;$i -lt $lines.Count;$i++){ if($lines[$i] -match "<keyword>"){
    $s=[Math]::Max(0,$i-4); $e=[Math]::Min($lines.Count-1,$i+4)
    for($j=$s;$j -le $e;$j++){ Write-Output $lines[$j].Trim() } } }
  ```

## Output Template (the 11 fields)

The report must answer every field below, in this order. See
[field-guide.md](./references/field-guide.md) for how to source and word each one.

1. **Describe the impact including possible work-around for the Customer**
2. **How to recover the system back from failure mode?**
3. **Root cause-analysis of this problem**
4. **Is the issue present in the installed base (yes/no)** — if yes:
   - What is the oldest release that has this issue
   - List affected product configurations
5. **What is the frequency of occurrence?** — `Occurs every time` / `May occur` / `Not expected to occur`
6. **Proposed solution** (describe technical risk + reliability impact; consider Change Point Analysis)
7. **DHF/DMR documents to create or modify** — Requirements, IFU, Design-, Test- and/or Purchase Specs
8. **Is testing required?** — if not, give rationale; reference a test to re-execute
9. **Is an update of the IFU / SMI required** (add or remove content)
10. **My (Investigator's) advice**
11. **Identify/Confirm failing product requirement** (SSRS/SRS ID + text)

## Field Sourcing Rules

| Field | Primary source | Notes |
|-------|----------------|-------|
| Customer impact + work-around | Code Review (symptom) | Describe user-visible effect; give manual work-around or state none exists |
| Recovery from failure mode | Code Review (severity) | If app stays stable, say no recovery needed; else give steps |
| Root cause | Code Review (root cause section) | Reuse the exact mechanism; cite the changed method/class |
| Installed base yes/no + oldest release | SSRS (Target Release / Rationale) + code history | If the bug is in shared/pre-existing code, answer **Yes**; flag exact oldest release as "to confirm" if not derivable |
| Affected configurations | SSRS `Product:` line | List the products/configurations that ship the affected component |
| Frequency | Code Review (reproducibility) | "Occurs every time" if deterministic; justify the choice |
| Proposed solution + risk | Code Review (fix + regression risk) | State the fix; risk usually Low; reference Change Point Analysis blast radius |
| DHF/DMR docs | Code Review (test impact) | Usually Test Spec update only; requirement/IFU unchanged if requirement still valid |
| Testing required | Code Review (Test Plan / new tests) | Reference the specific regression test + manual re-execution |
| IFU / SMI update | Defect nature | "No" for internal behavior fixes with no user-instruction change |
| Investigator advice | Synthesis | Accept/reject fix; pre-commit actions; link duplicate tickets |
| Failing requirement | SSRS (Requirement ID + text) | Quote the ID and text; add related IDs if applicable |

## Workflow

1. **Collect inputs**: confirm the Code Review Report and SSRS are available; ask for any missing item.
2. **Parse the Code Review Report**: extract symptom, root cause, fix, changed files, regression risk,
   test impact, residual risks.
3. **Parse the SSRS**: locate the failing requirement (ID + text), affected products, target release.
4. **Map shared root causes**: if related defect IDs are given, confirm they share the root cause and
   note that one fix resolves all; recommend linking duplicates.
5. **Fill the 11 fields**: follow the Field Sourcing Rules; mark anything not derivable from the inputs
   (e.g. exact oldest release) as "to confirm" rather than guessing.
6. **Build the HTML report**: use [report-template.html](./references/report-template.html), replacing
   every `{{PLACEHOLDER}}`. Each field becomes a titled section; the frequency choice is rendered as a
   highlighted selection among the three options.
7. **Save the HTML file**: write to `PD Analysis/productdefect-analysis-{DEFECT_ID}.html` under the
  workspace root (create the `PD Analysis` directory if needed), unless the user explicitly specifies
  another output path (e.g. `PD Analysis/productdefect-analysis-CTD00025333.html`).
8. **Summarize**: report the file path and call out any "to confirm" items the investigator must verify.

## Quality Rules

- **No fabrication**: never invent a requirement ID, release number, or root cause. If the inputs do
  not support a value, write "to confirm" and explain what is needed.
- **Traceability**: the failing requirement field must quote a real SSRS/SRS ID + text from the input.
- **Consistency with the fix**: the proposed solution, frequency, and testing answers must match what
  the Code Review Report actually states (e.g. deterministic symptom → "Occurs every time").
- **Self-contained HTML**: inline all CSS; no external assets or network calls.
- **Language**: write field answers in clear English (template labels are English). Keep the
  investigator's wording precise and review-ready.

## Related Skills

- `code-review` — produces the Code Review Report that feeds this skill.
- `domain-knowledge` — verify CT/DICOM/Spectral terminology and safety wording used in impact/root cause.
- `impact-analysis` — deeper upstream/downstream impact if the defect crosses modules.
- `requirements-traceability` — confirm the requirement-to-test trace cited in the testing field.
