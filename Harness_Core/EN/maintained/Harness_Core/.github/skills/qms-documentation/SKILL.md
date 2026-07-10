---
name: qms-documentation
description: "Maintaining living QMS markdown documents (SwRS, SSDS, Software Design Doc, Module Verification Plan/Procedure/Report, and the system-level Verification Plan/Procedure) that map 1:1 to PDLM Word templates. Load this skill when updating QMS documentation as part of implementing a feature."
---

# QMS Documentation Skill

This skill defines how agents maintain **living markdown files** in `docs/qms/` that
map **1:1** to the PDLM Word templates in `docs/qms-templates/`. Documents are updated
incrementally per feature — never regenerated from scratch during the harness loop.

## The skeleton is the contract

The `.docx` templates are the authoritative source of structure, but they are binary and
are **not** readable by agents at runtime. Each template is distilled — by
`.github/scripts/Extract-QmsSkeleton.py` — into a canonical markdown **skeleton** under
`docs/qms-templates/skeletons/{Doc}.skeleton.md` that captures, verbatim:

- every **heading** (exact text and level, in template order),
- every **table** with its exact **column headers**,
- the template's embedded **author guidance**, preserved as `<!-- GUIDANCE: ... -->`.

The living `docs/qms/{Doc}.md` files are seeded from these skeletons and must stay
conformant to them. `.github/scripts/Verify-QmsStructure.py` enforces conformance.

## Document Inventory

| Living Doc | Skeleton | Owner Agent | IEC 62304 |
|------------|----------|-------------|-----------|
| `docs/qms/SwRS.md` | `skeletons/SwRS.skeleton.md` | `@analyst` | §5.2 |
| `docs/qms/SSDS.md` | `skeletons/SSDS.skeleton.md` | `@sw-architect` | §5.3 |
| `docs/qms/SDD.md` | `skeletons/SDD.skeleton.md` | `@developer` | §5.4 |
| `docs/qms/MVP.md` | `skeletons/MVP.skeleton.md` | `@developer` | §5.5 |
| `docs/qms/MVProcedure.md` | `skeletons/MVProcedure.skeleton.md` | `@developer` | §5.6 |
| `docs/qms/MVReport.md` | `skeletons/MVReport.skeleton.md` | `@dev-evaluator` | §5.7 |
| `docs/qms/VerificationPlan.md` | `skeletons/VerificationPlan.skeleton.md` ¹ | `@test-designer` | §5.6 |
| `docs/qms/VerificationProcedure.md` | `skeletons/VerificationProcedure.skeleton.md` ¹ | `@test-designer` | §5.7 |

¹ **Skeleton pending.** The PDLM `.docx` templates for the two system-level
verification documents have not been added yet. The scripts and agents reference
these stems, but until their skeletons are extracted (see Tooling) the conformance
checker **skips** them and `@test-designer` captures its test design in its handover
and progress notes instead of authoring the formal doc. Once the `.docx` are dropped
into `docs/qms-templates/` and `Extract-QmsSkeleton.py` is run, these activate
automatically.

The `@developer`'s MODULE verification docs (`MVP`/`MVProcedure`/`MVReport`,
white-box unit/module tests) are distinct from the `@test-designer`'s SYSTEM-level
black-box `VerificationPlan`/`VerificationProcedure`.

The skeleton-to-template mapping lives in `Extract-QmsSkeleton.py` (`DOC_MAP`).

## Authoring Rules (NON-NEGOTIABLE)

1. **Exact structural fidelity.** Never rename, remove, reorder, or merge a heading that
   came from the skeleton. Every skeleton heading must remain present with its **exact
   text and level**, including case and punctuation (e.g. it is
   `Design Elements – Requirements Specification Traceability`, not
   `DESIGN ELEMENTS – REQUIREMENTS TRACEABILITY`).

2. **Mark Not Applicable — never delete.** If a section does not apply to this product or
   feature, keep the heading and write `*Not Applicable — <one-line reason>*` underneath.
   Empty sections are not allowed: either real content or an explicit N/A marker.

3. **Exact table fidelity.** Keep every skeleton table's **column headers exactly** as
   given. Fill in rows; if there is genuinely nothing to record, add a single row of
   `Not Applicable` cells. Do not add, drop, or rename columns.

4. **Self-contained — no `.harness/` references.** The QMS docs are the deliverable; the
   `.harness/` requirements, architecture, backlogs, and eval feedback are **planning
   artifacts that are NOT shipped**. Never link or refer to anything under `.harness/`.
   **Copy** all needed content — narrative, tables, and **diagrams** — *into* the QMS
   document. A reader with only the QMS docs must get the complete picture.

5. **Embed diagrams inline.** Architecture and design diagrams (e.g. Mermaid authored in
   `.harness/architecture`) must be reproduced **inside** the relevant QMS section as a
   ```` ```mermaid ```` block, not referenced. The export script renders them to images.

6. **Read and answer the guidance.** Each `<!-- GUIDANCE: ... -->` block is the template
   author's instruction for that section. Read it and write content that answers what it
   asks. Leave the guidance comments in place (they are stripped at export); add your
   content as normal markdown beneath them.

7. **Depth over stubs.** Write detailed, well-structured prose — not one-line
   placeholders. See "Depth Expectations" below.

8. **Repeatable sections.** Where a skeleton guidance block marks a
   `repeatable example section` (e.g. one subsection per module, sub-system, or issue),
   add as many concrete subsections as the design needs, nested at the indicated level.
   These additions are expected and allowed — only the *skeleton* headings are fixed.

9. **Use requirement IDs.** Every requirement, design element, test, and result references
   its `FR-XX` / `NFR-XX` ID so the traceability chain stays intact.

10. **Record change history.** After every update, append a row to the
    `RECORD CHANGE SUMMARY` table at the bottom of the file.

## Depth Expectations

The most common defect is shallow content, especially in SSDS and SDD. Minimum bar:

- **SwRS** — each requirement stated atomically with rationale and acceptance criteria;
  grouped under the correct module section.
- **SSDS** — for each sub-system: a design-overview narrative, an architecture **diagram**,
  interfaces (inputs/outputs/protocols), data/persistence, error handling, and the
  software risk-classification table filled with real module rows.
- **SDD** — for each module: functionality, use cases, detailed design, a **class
  diagram**, a **sequence diagram** for key flows, interface specs, and SW safety
  classification. SOUP and tool tables filled or marked N/A.
- **MVP / MVProcedure** — concrete test modules and step-by-step procedures with expected
  results, each traced to `FR-XX`.
- **VerificationPlan / VerificationProcedure** — system-level black-box test strategy,
  scope, environment, and step-by-step procedures with expected results, each traced to
  `FR-XX`. (Skeleton pending PDLM template — author once available.)
- **MVReport** — actual execution results, pass/fail, coverage %, and build configuration.

## How Documents Are Updated

```
@analyst        → SwRS.md        (requirement content under the correct module section)
@sw-architect   → SSDS.md        (architecture + diagrams + risk-classification table)
@developer      → SDD.md         (per-module design, class/sequence diagrams, safety class)
                → MVP.md         (planned MODULE test modules)
                → MVProcedure.md (MODULE test procedures traced to FR-XX)
@test-designer  → VerificationPlan.md       (SYSTEM black-box test strategy + scope) ¹
                → VerificationProcedure.md  (SYSTEM black-box procedures traced to FR-XX) ¹
@dev-evaluator  → MVReport.md    (execution results, pass/fail, coverage)
```

¹ Skeleton pending PDLM `.docx` — see the Document Inventory note. Until the
template lands, `@test-designer` records its test design in its handover and
progress notes and the conformance checker skips these two stems.

The `@developer` (SDD/MVP/MVProcedure) and `@test-designer`
(VerificationPlan/VerificationProcedure) docs are authored during the **per-task
design gate**, BEFORE implementation, then re-touched during implementation only
if the build deviates from the approved design.

**Under parallel execution** (`@orchestrator-parallel`), these QMS documents are
**shared across all tasks** — every task appends its own module/section content
to the same files. Writes to the shared QMS docs are therefore **serialized one
task at a time** (task-ID order): when several tasks run concurrently, each waits
for the current writer to finish its `docs/qms/{Doc}.md` update before applying
its own, so no two tasks edit the same document simultaneously. The content a
task adds is private to that task's module section; only the write operation is
serialized.

Workflow each session:
1. Read your target `docs/qms/{Doc}.md` **and** its `skeletons/{Doc}.skeleton.md` to see
   the fixed structure and the guidance.
2. Fill or update the relevant sections per the rules above — embed content and diagrams
   inline, mark anything N/A, keep all skeleton headings and table columns.
3. Run the conformance checker (see Tooling) and fix any violation.
4. Append a `RECORD CHANGE SUMMARY` row.

## Tooling

- **Regenerate skeletons** (only when a `.docx` template itself changes):
  `python .github/scripts/Extract-QmsSkeleton.py` — writes `docs/qms-templates/skeletons/`.
  Add `--reseed` to also re-create empty living docs (destructive; planning use only).
- **Verify conformance** (run before claiming completion, and at export):
  `python .github/scripts/Verify-QmsStructure.py --check-content`
  Fails on a missing/renamed/reordered heading, a mismatched table, any `.harness/`
  reference, an empty section without an N/A marker, or a stray placeholder.

## IEC 62304 Traceability Chain

```
FR-XX (SwRS) → Architecture Component (SSDS) → Design Detail (SDD) → Test Plan (MVP)
            → Test Procedure (MVProcedure) → Test Result (MVReport)
```

Maintain this chain by always including requirement IDs when updating any document.

## Conversion: Markdown → Word (.docx)

The markdown files are the source of truth. Conversion to Word is a separate, manual step
run when a document is submitted for formal review — never during the runtime harness loop.

**The export is a human-invoked step — run the `qms-export` prompt**
([.github/prompts/qms-export.prompt.md](../../prompts/qms-export.prompt.md)). It drives the
pre-tested script `.github/scripts/Export-Qms.py`, which is the single source of truth for
the conversion mechanics (template map, structure verification, Mermaid pre-rendering,
pandoc invocation, cover-page merge, flags, and defaults). Do not re-document those
mechanics here or hand-write pandoc commands. Agents that only maintain the markdown never
run the export themselves.

## Review Checklist (Before md→docx Conversion)

- [ ] `Verify-QmsStructure.py --check-content` passes for the document
- [ ] Every skeleton heading present, exact text and level; nothing renamed or removed
- [ ] Every table keeps its exact skeleton columns
- [ ] Irrelevant sections marked `*Not Applicable*`, not deleted
- [ ] No reference to anything under `.harness/`; all content and diagrams embedded inline
- [ ] All requirements have unique IDs (`FR-XX`, `NFR-XX`); traceability complete
- [ ] RECORD CHANGE SUMMARY has an entry for this revision
- [ ] Mermaid diagrams render correctly in markdown preview
- [ ] No patient data, credentials, or internal hostnames in the document
