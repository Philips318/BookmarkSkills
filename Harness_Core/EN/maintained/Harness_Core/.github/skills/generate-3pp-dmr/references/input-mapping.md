# Input Mapping — CIP / TRR / Supplier → DMR Fields

Companion reference for the `generate-3pp-dmr` skill. Tells you which input document supplies each DMR field. Primary sources are the **CIP** (Change Request / Change Plan) and the **TRR** (Technical Review Report).

## Quick map

| DMR field / table | Primary source | Where in the source |
|---|---|---|
| Product title (Manufacturer + Product) | CIP / TRR | CIP "Product Identification"; TRR scope "Injector products" table |
| Affected Philips systems + product numbers | CIP | "Product Identification" / "Affected Sites, Modalities, Products" |
| Philips systems 6NC list | TRR | Scope "Philips CT systems" table |
| 12NC table (12NC, name, model+desc) | CIP + spec sheets | CIP product identification; supplier/spec sheets for model numbers |
| Interface type (CANOpen / SAS) | TRR | "Interface Confirmation" / "Compatibility" sections |
| Cable 12NC | CIP / supplier | Interface/BOM references |
| Functionality text | Supplier docs / IFU / TRR | Operator manual intro; TRR purpose; IFU intended use |
| Manufacturer table (Mfr, Model, P/N) | TRR + spec sheets | TRR injector table (manufacturer, device models); spec sheets for P/N |
| Compatibility Statement (appendix refs) | TRR | Compatibility conclusion + attached statements |
| Installation reference | Supplier service manual | Manufacturer name only (DMR just points to the manual) |
| References table (TRR IDs + revisions) | TRR | Document number + revision (e.g., `D002112428 Rev B`) |
| CR / CN number (revision history) | CIP | CR# / Purpose of Change section |
| Author | User / CIP | Change lead or document author |
| Document ID + revision of the DMR | User / CIP | Assigned in PLM tool; ask the user if not provided |
| Appendices list | TRR + supplier | Compatibility statements + operator manuals |

## CIP — Change Request / Change Plan

What to extract:
- **CR number** (e.g., `CR244787`) → Document Revision History "CR / Reason".
- **Product Identification** table: third-party product description, model number(s), and the Philips products they ship with → Overview + Affected Products + 12NC name.
- **Affected Sites, Modalities, Products** and the system list (with product numbers like `728381`) → **Affected Products** table.
- **Purpose of Change** → confirms this is a logistical 3PP release (12NC + DMR); informs the Overview framing.
- **Engineering Project** (e.g., `PJ-012080`) → context only.

Notes:
- The CIP lists multiple accessories at once (e.g., RPM, RGSC, Sentinel 4DCT, SimRT). Select only the rows relevant to the DMR being drafted.
- CIP tables may use merged cells; read carefully so model ↔ Philips-product associations stay correct.

## TRR — Technical Review Report

What to extract:
- **Scope → Philips CT systems** table (CT system + 6NC) → cross-check Affected Products.
- **Scope → Injector/Device products** table (Manufacture | Device Description | Device Models) → **Manufacturer** table and model identification.
- **Interface Confirmation** (e.g., "CANOpen function … based on the CiA 425 CANOpen protocol", or SAS "Start Automatic Scan") → **Interfaces** section and Functionality.
- **Conclusion** (compatibility confirmed) → supports the Compatibility Statement reference.
- **Document number + revision** of the TRR itself → **References** table. Use the exact ID and `Rev <X>`.
- There may be **multiple TRRs** (e.g., one for CANOpen, one for SAS compatibility, one per system). List all that apply.

## Supplier documentation / IFU / spec sheets / website snapshots

- **Operator / service manuals** → become **Appendices**; the manufacturer name feeds the **Installation** reference.
- **IFU intended use** → confirms **Functionality** wording.
- **Spec sheets** → authoritative **model numbers** and **manufacturer P/N**; help complete the 12NC table.
- **Website snapshots** → only to corroborate product/model identification when other sources are incomplete.

## Mapping rules

- Copy all identifiers (12NC, 6NC, product numbers, model numbers, document IDs) **verbatim**.
- When the CIP and TRR disagree, prefer the **TRR** for technical/interface facts and the **CIP** for release/identification facts; flag the discrepancy to the user.
- If a value is in none of the inputs, write `[TBD]` — never guess.
- Keep one product family per DMR; do not merge unrelated accessories.
