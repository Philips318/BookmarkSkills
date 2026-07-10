---
name: qms-documentation
description: "维护现行 QMS markdown documents（SwRS、SSDS、Software Design Doc、Module Verification Plan/Procedure/Report，以及 system-level Verification Plan/Procedure），它们与 PDLM Word templates 1:1 映射。作为实现 feature 的一部分更新 QMS documentation 时加载此 skill。"
---

# QMS Documentation Skill

此 skill 定义 agents 如何维护 `docs/qms/` 中的**现行 markdown 文件**，这些文件与 `docs/qms-templates/` 中的 PDLM Word templates **1:1** 映射。Documents 按 feature 增量更新 — 在 harness loop 中绝不从头重新生成。

## skeleton 是契约

`.docx` templates 是结构的权威来源，但它们是二进制文件，agents 在运行时**不可读**。每个 template 都由 `.github/scripts/Extract-QmsSkeleton.py` 提炼为 `docs/qms-templates/skeletons/{Doc}.skeleton.md` 下的规范 markdown **skeleton**，逐字捕获：

- 每个 **heading**（template 顺序中的精确文本和层级），
- 每个带有精确 **column headers** 的 **table**，
- template 中嵌入的 **author guidance**，保留为 `<!-- GUIDANCE: ... -->`。

现行 `docs/qms/{Doc}.md` 文件从这些 skeletons 播种，并且必须保持与它们一致。`.github/scripts/Verify-QmsStructure.py` 强制执行一致性。

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

¹ **Skeleton pending.** 两个 system-level verification documents 的 PDLM `.docx` templates 尚未添加。Scripts 和 agents 会引用这些 stems，但在提取其 skeletons 前（见 Tooling），conformance checker 会**跳过**它们，`@test-designer` 会在其 handover 和 progress notes 中记录 test design，而不是编写正式 doc。将 `.docx` 放入 `docs/qms-templates/` 并运行 `Extract-QmsSkeleton.py` 后，它们会自动激活。

`@developer` 的 MODULE verification docs（`MVP`/`MVProcedure`/`MVReport`，white-box unit/module tests）不同于 `@test-designer` 的 SYSTEM-level black-box `VerificationPlan`/`VerificationProcedure`。

skeleton-to-template 映射位于 `Extract-QmsSkeleton.py`（`DOC_MAP`）。

## Authoring Rules（NON-NEGOTIABLE）

1. **精确结构保真。** 绝不重命名、删除、重排或合并来自 skeleton 的 heading。每个 skeleton heading 都必须以其**精确文本和层级**保留，包括大小写和标点（例如是 `Design Elements – Requirements Specification Traceability`，不是 `DESIGN ELEMENTS – REQUIREMENTS TRACEABILITY`）。

2. **标记 Not Applicable — 绝不删除。** 如果某节不适用于此产品或 feature，保留 heading，并在其下写 `*Not Applicable — <one-line reason>*`。不允许空节：要么是真实内容，要么是显式 N/A 标记。

3. **精确表格保真。** 保留每个 skeleton table 的 **column headers exact**。填入 rows；如果确实没有内容可记录，添加一行全部为 `Not Applicable` 的 cells。不要添加、删除或重命名 columns。

4. **自包含 — 不引用 `.harness/`。** QMS docs 是交付物；`.harness/` requirements、architecture、backlogs 和 eval feedback 是**不会交付的 planning artifacts**。绝不链接或引用 `.harness/` 下任何内容。将所有需要的内容 — narrative、tables 和 **diagrams** — **复制**到 QMS document 中。只有 QMS docs 的读者也必须获得完整图景。

5. **内联嵌入 diagrams。** Architecture 和 design diagrams（例如 `.harness/architecture` 中编写的 Mermaid）必须在相关 QMS section **内部**重现为 ```` ```mermaid ```` block，而不是引用。Export script 会将它们渲染为 images。

6. **阅读并回答 guidance。** 每个 `<!-- GUIDANCE: ... -->` block 都是 template 作者对该 section 的说明。阅读它，并编写回答其要求的内容。保留 guidance comments（导出时会剥离）；在其下方以普通 markdown 添加你的内容。

7. **深度优先于 stubs。** 编写详细、结构良好的 prose — 不要写一行 placeholders。见下方 “Depth Expectations”。

8. **Repeatable sections。** 当 skeleton guidance block 标记 `repeatable example section`（例如每个 module、sub-system 或 issue 一个 subsection）时，根据设计需要添加任意数量的具体 subsections，嵌套在指示的层级。这些新增内容是预期且允许的 — 只有 *skeleton* headings 是固定的。

9. **使用 requirement IDs。** 每个 requirement、design element、test 和 result 都引用其 `FR-XX` / `NFR-XX` ID，使 traceability chain 保持完整。

10. **记录变更历史。** 每次更新后，向文件底部的 `RECORD CHANGE SUMMARY` table 追加一行。

## Depth Expectations

最常见缺陷是内容浅，尤其是 SSDS 和 SDD。最低标准：

- **SwRS** — 每条 requirement 以原子方式陈述，并包含 rationale 和 acceptance criteria；归入正确的 module section。
- **SSDS** — 对每个 sub-system：design-overview narrative、architecture **diagram**、interfaces（inputs/outputs/protocols）、data/persistence、error handling，以及填入真实 module rows 的 software risk-classification table。
- **SDD** — 对每个 module：functionality、use cases、detailed design、**class diagram**、关键流程的 **sequence diagram**、interface specs 和 SW safety classification。SOUP 和 tool tables 已填写或标记 N/A。
- **MVP / MVProcedure** — 具体 test modules 和逐步 procedures，每一步都有 expected results，并追踪到 `FR-XX`。
- **VerificationPlan / VerificationProcedure** — system-level black-box test strategy、scope、environment，以及带 expected results 的逐步 procedures，每项追踪到 `FR-XX`。（Skeleton pending PDLM template — 可用后编写。）
- **MVReport** — 实际 execution results、pass/fail、coverage % 和 build configuration。

## Documents 如何更新

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

¹ Skeleton pending PDLM `.docx` — 见 Document Inventory 注释。在 template 落地前，`@test-designer` 会在其 handover 和 progress notes 中记录 test design，conformance checker 会跳过这两个 stems。

`@developer`（SDD/MVP/MVProcedure）和 `@test-designer`（VerificationPlan/VerificationProcedure）docs 在 **per-task design gate** 期间编写，早于 implementation；之后只有当 build 偏离已批准设计时，才在 implementation 期间重新触碰。

**在并行执行下**（`@orchestrator-parallel`），这些 QMS documents 在**所有任务之间共享** — 每个任务都向相同文件追加自己的 module/section content。因此，对共享 QMS docs 的写入会**一次只串行化一个任务**（按 task-ID 顺序）：当多个任务并发运行时，每个任务都会等待当前写入者完成其 `docs/qms/{Doc}.md` 更新后，再应用自己的更新，避免两个任务同时编辑同一文档。任务添加的内容对该任务的 module section 是私有的；只有写操作被串行化。

每个会话的工作流：
1. 读取目标 `docs/qms/{Doc}.md` **以及** 它的 `skeletons/{Doc}.skeleton.md`，了解固定结构和 guidance。
2. 根据上述规则填充或更新相关 sections — 内联嵌入内容和 diagrams，标记任何 N/A，保留所有 skeleton headings 和 table columns。
3. 运行 conformance checker（见 Tooling）并修复任何 violation。
4. 追加一行 `RECORD CHANGE SUMMARY`。

## Tooling

- **重新生成 skeletons**（仅当 `.docx` template 本身变化时）：
  `python .github/scripts/Extract-QmsSkeleton.py` — 写入 `docs/qms-templates/skeletons/`。
  添加 `--reseed` 也会重新创建空的 living docs（破坏性；仅规划用途）。
- **验证 conformance**（声明完成前和导出时运行）：
  `python .github/scripts/Verify-QmsStructure.py --check-content`
  如果 heading 缺失/重命名/重排、table 不匹配、有任何 `.harness/` 引用、section 没有 N/A marker 却为空，或存在 stray placeholder，则失败。

## IEC 62304 Traceability Chain

```
FR-XX (SwRS) → Architecture Component (SSDS) → Design Detail (SDD) → Test Plan (MVP)
            → Test Procedure (MVProcedure) → Test Result (MVReport)
```

更新任何 document 时始终包含 requirement IDs，以维护此链路。

## Conversion: Markdown → Word (.docx)

markdown 文件是事实来源。转换为 Word 是单独的手动步骤，在文档提交正式评审时运行 — 绝不在 runtime harness loop 中运行。

**导出是人工触发步骤 — 运行 `qms-export` prompt**
([.github/prompts/qms-export.prompt.md](../../prompts/qms-export.prompt.md))。它驱动预先测试过的脚本 `.github/scripts/Export-Qms.py`，该脚本是转换机制的单一事实来源（template map、structure verification、Mermaid pre-rendering、pandoc invocation、cover-page merge、flags 和 defaults）。不要在这里重新记录这些机制，也不要手写 pandoc commands。只维护 markdown 的 agents 不自行运行导出。

## Review Checklist（md→docx Conversion 前）

- [ ] `Verify-QmsStructure.py --check-content` 对该 document 通过
- [ ] 每个 skeleton heading 都存在，文本和层级精确；没有重命名或删除
- [ ] 每个 table 保持其精确 skeleton columns
- [ ] 无关 sections 标记为 `*Not Applicable*`，而不是删除
- [ ] 不引用 `.harness/` 下任何内容；所有内容和 diagrams 都内联嵌入
- [ ] 所有 requirements 都有唯一 IDs（`FR-XX`、`NFR-XX`）；traceability 完整
- [ ] RECORD CHANGE SUMMARY 有此 revision 的条目
- [ ] Mermaid diagrams 在 markdown preview 中正确渲染
- [ ] 文档中没有 patient data、credentials 或 internal hostnames
