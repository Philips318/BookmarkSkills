---
name: generate-3pp-dmr
description: 'Draft Device Master Record (DMR) Word documents for Third Party Products (3PP) sold with Philips CT/AMI systems — injectors, gating systems, and other accessories. Takes a CIP (Change Request/Change Plan), TRR (Technical Review Report), supplier documentation, IFU, specification sheets, and product website snapshots as input, and produces a Word (.docx) DMR that follows the standard Philips 3PP DMR template. Use when releasing a 12NC and DMR for a new third-party accessory.'
argument-hint: 'Provide paths to: 1) CIP document, 2) TRR document(s), and optionally supplier docs / IFU / spec sheets / website snapshots'
user-invocable: true
---

# Generate 3PP DMR

Use this skill to draft a **Device Master Record (DMR)** Word document for a **Third Party Product (3PP)** — an accessory (CT injector, respiratory gating system, etc.) that is released with a Philips 12NC and sold alongside Philips CT/AMI systems.

A 3PP DMR is **not** a design document for an in-house part. It is a logistical/regulatory record that:
- Identifies the third-party product and its Philips 12NCs.
- States that design, quality, and robustness are the **responsibility of the third-party manufacturer**.
- Captures functionality, manufacturer identification, compatibility statements, installation reference, and the list of affected Philips products.
- References the Technical Review Report(s) that established compatibility.

Because the template is highly standardized, most of the document is fixed boilerplate plus structured data tables that are extracted and mapped from the input documents.

## When to Use

- A CIP (Change Request / Change Plan) authorizes releasing a 12NC + DMR for a new third-party accessory.
- A TRR (Technical Review Report) has established compatibility of the accessory with Philips CT systems.
- You need to draft one or more DMR `.docx` files for a third-party injector, gating system, adapter, or similar accessory.
- You are preparing the DMR deliverable referenced in a 3PP release (QMS-0014 / portfolio of third-party items).

## Required Inputs

Ask the user to provide these when the skill is invoked. The CIP and TRR are the primary sources; the rest enrich specific sections.

1. **CIP — Change Request / Change Plan (required)**: Source for CR number, product identification (third-party model ↔ Philips products), supplier/manufacturer names, the list of affected Philips CT products (with product numbers), purpose of change, and engineering project.
   - Example folder: `E:\3PP\Taichi CIP`
2. **TRR — Technical Review Report(s) (required)**: Source for scope (Philips CT systems + 6NC, injector manufacturer/description/models), interface confirmation (e.g., CANOpen CiA 425, SAS), the compatibility conclusion, and the document ID + revision that goes into the DMR **References** table.
   - Example folder: `E:\3PP\Taichi TRR`
3. **Supplier documentation / operator & service manuals (optional)**: Source for the **Functionality** text and for **Installation**; operator manuals are listed as **Appendices** (attached in the PLM tool).
4. **IFU / Instructions for Use (optional)**: Confirms intended use and functionality wording.
5. **Specification sheets (optional)**: Source for model numbers, manufacturer part numbers, and the 12NC ↔ model mapping.
6. **Product website snapshots (optional)**: Supplementary product/model identification when supplier docs are incomplete.

If a required field cannot be derived from the inputs, insert `[TBD]` rather than inventing a value.

Reference folder of finished DMRs (for style and structure): `E:\3PP\Taichi DMR Documents`.

## DMR Template Structure

The DMR follows this fixed section structure (see [dmr-structure.md](./references/dmr-structure.md) for the full per-section specification). Sections marked *(boilerplate)* use standard text; sections marked *(data)* are filled from the inputs.

1. **Purpose** *(boilerplate)* — "This document describes the detailed technical design of an element of the product."
2. **Scope** *(boilerplate)* — "This document applies to Philips CT/AMI."
3. **Terminology & Abbreviations** *(data table)* — CT, DMR, P/N, plus any product-specific terms.
4. **Overview** *(data)* — Product name + affected Philips products list + **12NC table** (12NC | 12NC Name | Model Numbers and Description).
5. **Architecture Views** *(N/A)* — "N/A — This is a 3rd party item and the architecture views are the responsibility of the manufacturer."
6. **Design Details**
   - 6.1 **Allocation of Quality Aspects** *(N/A)*
   - 6.2 **Element detailed design** *(N/A)*
   - 6.3 **Interfaces** *(data)* — Describe the interface to the CT gantry (cable 12NC, SAS/CANOpen).
   - 6.4 **Parts → 6.4.1 [Product]**
     - **Functionality** *(data)* — What the accessory does in the clinical workflow.
     - **Design Constraints** *(N/A)*
     - **Compatibility Statement** *(reference)* — "Compatibility Statement can be found in Appendix A, B and C."
     - **Manufacturer** *(data table)* — Manufacturer | Manufacturer Model | Manufacturer P/N.
     - **Installation** *(reference)* — Refer to the supplier's service manual.
     - **Affected Products** *(data table)* — Product | Product Number (Philips CT systems).
   - 6.5 **Design robustness** *(N/A)*
7. **References** *(data table)* — Reference Number | Document Title | Document ID (mostly TRRs).
8. **Document Revision History** *(data table)* — Revision | Release Date | Author | Description of changes | CR / Reason.
9. **Appendices** *(data)* — Compatibility statements and operator manuals, each "attached to this record in the PLM tool."

## Workflow

1. **Collect inputs**: Confirm the CIP and TRR paths with the user. Ask for any optional supplier/IFU/spec/website material. If the user gives a folder, identify the relevant `.docx`/`.pdf` files.
2. **Extract source content**: Read the CIP and TRR (and any supplier docs). For `.docx` files, extract paragraphs and tables (use the helper described in [extracting-source-docs.md](./references/extracting-source-docs.md)). Capture: product/model identification, 12NCs, manufacturer name + part numbers, affected Philips products + product numbers, interface type, functionality description, and TRR document IDs + revisions.
3. **Map inputs to DMR fields**: Use [input-mapping.md](./references/input-mapping.md) to place each extracted item into the correct DMR section/table. Mark anything missing as `[TBD]`.
4. **Apply boilerplate**: Fill all *(boilerplate)* and *(N/A)* sections using the exact standard wording from [boilerplate-text.md](./references/boilerplate-text.md).
5. **Confirm document identity**: Determine the DMR document ID, product title, 12NC(s), and revision (from the CIP / user). If this is an initial release, set Revision = A; otherwise increment and add a revision-history row describing the change and CR.
6. **Build the Word document**: Generate the `.docx` using the [Build-DMR-Word.ps1](./references/Build-DMR-Word.ps1) script (PowerShell + Word COM). Pass the mapped data; the script produces headings, tables, and the table of contents in the standard style.
7. **Self-check**: Verify against the [DMR checklist](./references/dmr-structure.md#checklist) — all sections present, every data table populated or `[TBD]`, References lists the TRR(s), revision history correct.
8. **Deliver + disclaimer**: Save the `.docx` and tell the user it is an **AI-generated draft** that requires Systems Engineering review and the standard release/approval workflow before use.

## Output

- A Microsoft Word `.docx` DMR named to match the convention of the example folder, e.g. `D00XXXXXXX <Manufacturer> <Product> <12NC>.docx`.
- The document mirrors the standard template: title page, table of contents, the nine numbered sections above, and all data tables.
- Any value not derivable from the inputs is left as a visible `[TBD]` placeholder for the engineer to complete.

## Rules

- **This is a draft.** Always state that the DMR requires Systems Engineering review and the formal release/approval workflow (PLM tool) before use.
- **Do not invent data.** Model numbers, 12NCs, manufacturer part numbers, product numbers, and document IDs must come from the inputs. Unknown → `[TBD]`.
- **Preserve exact identifiers.** Copy 12NCs, 6NC/product numbers, model numbers, and document IDs (with revision) verbatim — never reformat or "correct" them.
- **Use the standard boilerplate verbatim.** The N/A and Purpose/Scope sentences are fixed wording (see [boilerplate-text.md](./references/boilerplate-text.md)); do not paraphrase.
- **Third-party responsibility.** Architecture, quality aspects, detailed design, design constraints, and design robustness are always the manufacturer's responsibility — keep the standard N/A phrasing.
- **Compatibility & installation are references**, not restated content — point to the appendices and the supplier service manual.
- **References table** must list the TRR(s) that established compatibility, with the correct Document ID and revision.
- **Confidentiality notice.** Keep the standard Philips confidential/proprietary statement in the Design Details section.
- **One product family per DMR**, consistent with the example documents.
