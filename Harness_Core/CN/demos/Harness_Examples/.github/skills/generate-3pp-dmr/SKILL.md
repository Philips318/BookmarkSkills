---
name: generate-3pp-dmr
description: '为随 Philips CT/AMI systems 销售的 Third Party Products (3PP) 起草 Device Master Record (DMR) Word documents — injectors、gating systems 和其他 accessories。输入 CIP（Change Request/Change Plan）、TRR（Technical Review Report）、supplier documentation、IFU、specification sheets 和 product website snapshots，产出遵循 standard Philips 3PP DMR template 的 Word (.docx) DMR。用于发布 new third-party accessory 的 12NC 和 DMR。'
argument-hint: 'Provide paths to: 1) CIP document, 2) TRR document(s), and optionally supplier docs / IFU / spec sheets / website snapshots'
user-invocable: true
---

# Generate 3PP DMR

使用此 skill 为 **Third Party Product (3PP)** 起草 **Device Master Record (DMR)** Word document — 这类 accessory（CT injector、respiratory gating system 等）会随 Philips 12NC 发布，并与 Philips CT/AMI systems 一起销售。

3PP DMR **不是** in-house part 的 design document。它是 logistical/regulatory record，用于：
- 识别 third-party product 及其 Philips 12NCs。
- 声明 design、quality 和 robustness 是 **third-party manufacturer 的责任**。
- 捕获 functionality、manufacturer identification、compatibility statements、installation reference，以及 affected Philips products list。
- 引用建立 compatibility 的 Technical Review Report(s)。

由于 template 高度 standardized，document 的大部分内容是 fixed boilerplate 加上从 input documents 中 extracted and mapped 的 structured data tables。

## When to Use

- CIP（Change Request / Change Plan）授权为 new third-party accessory 发布 12NC + DMR。
- TRR（Technical Review Report）已建立 accessory 与 Philips CT systems 的 compatibility。
- 需要为 third-party injector、gating system、adapter 或类似 accessory 起草一个或多个 DMR `.docx` files。
- 准备 3PP release 中引用的 DMR deliverable（QMS-0014 / portfolio of third-party items）。

## Required Inputs

skill 被 invoked 时，请 user 提供这些 inputs。CIP 和 TRR 是 primary sources；其余 material 用于丰富 specific sections。

1. **CIP — Change Request / Change Plan (required)**: CR number、product identification（third-party model ↔ Philips products）、supplier/manufacturer names、affected Philips CT products list（with product numbers）、purpose of change 和 engineering project 的来源。
   - Example folder: `E:\3PP\Taichi CIP`
2. **TRR — Technical Review Report(s) (required)**: scope（Philips CT systems + 6NC、injector manufacturer/description/models）、interface confirmation（例如 CANOpen CiA 425、SAS）、compatibility conclusion，以及进入 DMR **References** table 的 document ID + revision 的来源。
   - Example folder: `E:\3PP\Taichi TRR`
3. **Supplier documentation / operator & service manuals (optional)**: **Functionality** text 和 **Installation** 的来源；operator manuals 作为 **Appendices** 列出（attached in the PLM tool）。
4. **IFU / Instructions for Use (optional)**: 确认 intended use 和 functionality wording。
5. **Specification sheets (optional)**: model numbers、manufacturer part numbers 和 12NC ↔ model mapping 的来源。
6. **Product website snapshots (optional)**: 当 supplier docs 不完整时补充 product/model identification。

如果 required field 无法从 inputs 推导，插入 `[TBD]`，不要 invent value。

Finished DMRs 的 reference folder（style and structure）: `E:\3PP\Taichi DMR Documents`。

## DMR Template Structure

DMR 遵循此 fixed section structure（完整 per-section specification 见 [dmr-structure.md](./references/dmr-structure.md)）。标为 *(boilerplate)* 的 sections 使用 standard text；标为 *(data)* 的 sections 从 inputs 填充。

1. **Purpose** *(boilerplate)* — "This document describes the detailed technical design of an element of the product."
2. **Scope** *(boilerplate)* — "This document applies to Philips CT/AMI."
3. **Terminology & Abbreviations** *(data table)* — CT、DMR、P/N，加任何 product-specific terms。
4. **Overview** *(data)* — Product name + affected Philips products list + **12NC table**（12NC | 12NC Name | Model Numbers and Description）。
5. **Architecture Views** *(N/A)* — "N/A — This is a 3rd party item and the architecture views are the responsibility of the manufacturer."
6. **Design Details**
   - 6.1 **Allocation of Quality Aspects** *(N/A)*
   - 6.2 **Element detailed design** *(N/A)*
   - 6.3 **Interfaces** *(data)* — Describe the interface to the CT gantry（cable 12NC、SAS/CANOpen）。
   - 6.4 **Parts → 6.4.1 [Product]**
     - **Functionality** *(data)* — accessory 在 clinical workflow 中做什么。
     - **Design Constraints** *(N/A)*
     - **Compatibility Statement** *(reference)* — "Compatibility Statement can be found in Appendix A, B and C."
     - **Manufacturer** *(data table)* — Manufacturer | Manufacturer Model | Manufacturer P/N。
     - **Installation** *(reference)* — Refer to the supplier's service manual。
     - **Affected Products** *(data table)* — Product | Product Number（Philips CT systems）。
   - 6.5 **Design robustness** *(N/A)*
7. **References** *(data table)* — Reference Number | Document Title | Document ID（mostly TRRs）。
8. **Document Revision History** *(data table)* — Revision | Release Date | Author | Description of changes | CR / Reason。
9. **Appendices** *(data)* — Compatibility statements 和 operator manuals，每个都写 "attached to this record in the PLM tool."

## Workflow

1. **Collect inputs**: Confirm CIP 和 TRR paths with the user。询问任何 optional supplier/IFU/spec/website material。如果 user 给 folder，识别相关 `.docx`/`.pdf` files。
2. **Extract source content**: 读取 CIP 和 TRR（以及 any supplier docs）。对 `.docx` files，extract paragraphs and tables（使用 [extracting-source-docs.md](./references/extracting-source-docs.md) 中描述的 helper）。捕获：product/model identification、12NCs、manufacturer name + part numbers、affected Philips products + product numbers、interface type、functionality description、TRR document IDs + revisions。
3. **Map inputs to DMR fields**: 使用 [input-mapping.md](./references/input-mapping.md) 将 extracted items 放入正确 DMR section/table。缺失项标记为 `[TBD]`。
4. **Apply boilerplate**: 使用 [boilerplate-text.md](./references/boilerplate-text.md) 中 exact standard wording 填充所有 *(boilerplate)* 和 *(N/A)* sections。
5. **Confirm document identity**: 从 CIP / user 确定 DMR document ID、product title、12NC(s) 和 revision。如果是 initial release，set Revision = A；否则 increment，并添加 revision-history row 描述 change 和 CR。
6. **Build the Word document**: 使用 [Build-DMR-Word.ps1](./references/Build-DMR-Word.ps1) script（PowerShell + Word COM）生成 `.docx`。传入 mapped data；script 产出 standard style 的 headings、tables 和 table of contents。
7. **Self-check**: 根据 [DMR checklist](./references/dmr-structure.md#checklist) 验证 — all sections present、every data table populated or `[TBD]`、References lists TRR(s)、revision history correct。
8. **Deliver + disclaimer**: 保存 `.docx`，并告知 user 这是 **AI-generated draft**，需要 Systems Engineering review 以及 standard release/approval workflow 后才能使用。

## Output

- 一个 Microsoft Word `.docx` DMR，命名匹配 example folder convention，例如 `D00XXXXXXX <Manufacturer> <Product> <12NC>.docx`。
- Document mirrors standard template：title page、table of contents、above nine numbered sections，以及 all data tables。
- 任何无法从 inputs 推导的 value 都保留为 visible `[TBD]` placeholder，供 engineer 完成。

## Rules

- **This is a draft.** 始终说明 DMR 需要 Systems Engineering review 和 formal release/approval workflow（PLM tool）后才能使用。
- **Do not invent data.** Model numbers、12NCs、manufacturer part numbers、product numbers 和 document IDs 必须来自 inputs。Unknown → `[TBD]`。
- **Preserve exact identifiers.** Copy 12NCs、6NC/product numbers、model numbers 和 document IDs（with revision）verbatim — never reformat or "correct" them。
- **Use the standard boilerplate verbatim.** N/A 和 Purpose/Scope sentences 是 fixed wording（见 [boilerplate-text.md](./references/boilerplate-text.md)）；不要 paraphrase。
- **Third-party responsibility.** Architecture、quality aspects、detailed design、design constraints 和 design robustness 始终是 manufacturer 的 responsibility — 保持 standard N/A phrasing。
- **Compatibility & installation are references**，不是 restated content — 指向 appendices 和 supplier service manual。
- **References table** 必须列出建立 compatibility 的 TRR(s)，并带 correct Document ID 和 revision。
- **Confidentiality notice.** 在 Design Details section 中保留 standard Philips confidential/proprietary statement。
- **One product family per DMR**，与 example documents 一致。
