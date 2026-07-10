# Input Mapping — CIP / TRR / Supplier → DMR Fields

`generate-3pp-dmr` skill 的 companion reference。说明哪个 input document 供应哪个 DMR field。Primary sources 是 **CIP**（Change Request / Change Plan）和 **TRR**（Technical Review Report）。

## Quick map

| DMR field / table | 主要来源 | Where in the source |
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

需要 extract：
- **CR number**（例如 `CR244787`）→ Document Revision History "CR / Reason"。
- **Product Identification** table：third-party product description、model number(s)，以及它们 ship with 的 Philips products → Overview + Affected Products + 12NC name。
- **Affected Sites, Modalities, Products** 和 system list（with product numbers like `728381`）→ **Affected Products** table。
- **Purpose of Change** → confirms this is a logistical 3PP release（12NC + DMR）；informs Overview framing。
- **Engineering Project**（例如 `PJ-012080`）→ context only。

Notes:
- CIP 会一次列出多个 accessories（例如 RPM、RGSC、Sentinel 4DCT、SimRT）。只选择与正在起草的 DMR 相关的 rows。
- CIP tables 可能使用 merged cells；仔细阅读，确保 model ↔ Philips-product associations 正确。

## TRR — Technical Review Report

需要 extract：
- **Scope → Philips CT systems** table（CT system + 6NC）→ cross-check Affected Products。
- **Scope → Injector/Device products** table（Manufacture | Device Description | Device Models）→ **Manufacturer** table 和 model identification。
- **Interface Confirmation**（例如 "CANOpen function … based on the CiA 425 CANOpen protocol"，或 SAS "Start Automatic Scan"）→ **Interfaces** section 和 Functionality。
- **Conclusion**（compatibility confirmed）→ supports Compatibility Statement reference。
- TRR 本身的 **Document number + revision** → **References** table。使用 exact ID 和 `Rev <X>`。
- 可能存在 **multiple TRRs**（例如一个用于 CANOpen，一个用于 SAS compatibility，一个 per system）。列出所有适用项。

## Supplier documentation / IFU / spec sheets / website snapshots

- **Operator / service manuals** → 成为 **Appendices**；manufacturer name feed **Installation** reference。
- **IFU intended use** → confirms **Functionality** wording。
- **Spec sheets** → authoritative **model numbers** 和 **manufacturer P/N**；帮助完成 12NC table。
- **Website snapshots** → 仅在其他 sources 不完整时用于 corroborate product/model identification。

## Mapping rules

- 所有 identifiers（12NC、6NC、product numbers、model numbers、document IDs）**verbatim** copy。
- 当 CIP 和 TRR 不一致时，technical/interface facts 优先使用 **TRR**，release/identification facts 优先使用 **CIP**；向 user flag discrepancy。
- 如果任何 input 都没有某值，写 `[TBD]` — never guess。
- 每个 DMR 保持 one product family；不要 merge unrelated accessories。
