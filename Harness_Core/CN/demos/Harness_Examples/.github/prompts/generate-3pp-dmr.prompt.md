---
name: Generate 3PP DMR
description: '为随 Philips CT/AMI systems 销售的 Third Party Product (3PP) accessory 起草 Device Master Record (DMR) Word document。接收 CIP、TRR，以及可选 supplier/IFU/spec/website inputs，并产出 standard-template .docx DMR。'
argument-hint: 'Provide paths to the CIP and TRR document(s), plus any supplier docs / IFU / spec sheets / website snapshots'
agent: 'agent'
tools: ['read_file', 'create_file', 'replace_string_in_file', 'list_dir', 'grep_search', 'run_in_terminal']
---

为随 Philips CT/AMI systems 发布并以 Philips 12NC 销售的 **Third Party Product (3PP)** accessory（injector、gating system、adapter 等）起草 **Device Master Record (DMR)** Word document。

使用以下 repository assets 作为 structure、mapping rules、boilerplate 和 Word builder 的来源：

- [DMR Structure](../skills/generate-3pp-dmr/references/dmr-structure.md) — 每个 section 的 specification + checklist
- [Input Mapping](../skills/generate-3pp-dmr/references/input-mapping.md) — CIP / TRR / supplier → DMR field map
- [Boilerplate Text](../skills/generate-3pp-dmr/references/boilerplate-text.md) — 精确 standard wording（verbatim）
- [Extracting Source Docs](../skills/generate-3pp-dmr/references/extracting-source-docs.md) — 如何读取 CIP/TRR .docx
- [Extract-SourceDoc.ps1](../skills/generate-3pp-dmr/references/Extract-SourceDoc.ps1) — Word→text extractor（merged-cell safe）
- [Build-DMR-Word.ps1](../skills/generate-3pp-dmr/references/Build-DMR-Word.ps1) — JSON→.docx DMR builder
- 已完成 DMR 的 reference folder（style/structure）：`E:\3PP\Taichi DMR Documents`

## Context

这是 **Philips CT/AMI** third-party-product release。3PP DMR 是 logistical/regulatory record，**不是** in-house design document：design、quality 和 robustness 是 third-party manufacturer 的责任，因此大多数 sections 使用标准 N/A wording。真正内容是 product/model identification（12NC）、manufacturer details、functionality、compatibility references、affected Philips products，以及对 Technical Review Report(s) 的引用。

## Required Inputs（ask the user）

1. **CIP** — Change Request / Change Plan（required）。Example folder: `E:\3PP\Taichi CIP`。
2. **TRR** — Technical Review Report(s)（required）。Example folder: `E:\3PP\Taichi TRR`。
3. **Supplier documentation / operator & service manuals**（optional）— functionality + installation + appendices。
4. **IFU**（optional）— intended use / functionality wording。
5. **Specification sheets**（optional）— model numbers + manufacturer P/N + 12NC mapping。
6. **Product website snapshots**（optional）— supplementary product identification。

如果未提供 CIP 或 TRR，请先询问用户再继续。

## Workflow

1. **Confirm inputs**：获取 CIP 和 TRR paths（以及任何 optional material）。如果给的是 folder，识别相关 files。
2. **Extract**：对每个 `.docx` 运行 `Extract-SourceDoc.ps1`，将 paragraphs（with styles）和 tables 转储到 `_dmr_work/` text file，然后读取它们。注意 CIP tables 中的 merged cells。
3. **Map**：使用 [Input Mapping](../skills/generate-3pp-dmr/references/input-mapping.md)，提取 product/model identification、12NCs、manufacturer + P/N、affected Philips products + numbers、interface type、functionality，以及 TRR Document IDs + revisions。缺失项标为 `[TBD]`。
4. **Confirm identity**：确定 DMR Document ID、product title、12NC(s)、author、CR/CN 和 revision（如果 inputs 中没有，请询问用户）。Initial release → Revision A。
5. **Assemble JSON**：生成与 `Build-DMR-Word.ps1` 中 schema 匹配的数据 JSON（dynamic sentences 使用 [Boilerplate Text](../skills/generate-3pp-dmr/references/boilerplate-text.md) 中的 exact boilerplate；script 会提供 fixed N/A text）。
6. **Build**：运行 `Build-DMR-Word.ps1 -DataPath <json> -OutPath <docx>` 生成 DMR。
7. **Self-check**：根据 [DMR checklist](../skills/generate-3pp-dmr/references/dmr-structure.md#checklist) 验证。
8. **Deliver**：保存 `.docx`，并声明这是 AI-generated draft，需要 Systems Engineering review 和 standard PLM release/approval workflow。

## Output

一个遵循 standard template 的 Microsoft Word `.docx` DMR（title、TOC、9 sections、所有 data tables），命名为 `D00XXXXXXX <Manufacturer> <Product> <12NC>.docx`。Unknown values 显示为可见 `[TBD]` placeholders。

## Rules

- **This is a draft** — 始终沟通 review/approval disclaimer。
- **Never invent** 12NCs、model numbers、product numbers、manufacturer P/N 或 document IDs — 从 inputs 原样复制；未知 → `[TBD]`。
- **Use boilerplate verbatim** — 不要改写 Purpose/Scope/N/A/confidentiality wording。
- **Third-party responsibility** — architecture、quality aspects、detailed design、design constraints 和 robustness 保持 N/A phrasing。
- **Compatibility & installation are references** to appendices / supplier service manual，不要复述内容。
- **References table** 必须列出 TRR(s)，并包含正确 Document ID + revision。
- **One product family per DMR.**
