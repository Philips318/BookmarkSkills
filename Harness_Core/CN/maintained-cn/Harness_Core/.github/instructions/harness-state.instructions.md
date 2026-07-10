---
applyTo: ".harness/**"
---

# Harness State Files Instructions

处理 harness state files（backlogs、progress、feedback、requirements、architecture）时：

## Backlog Files (`.harness/backlogs/*.json`)

- 绝不删除或重排任务
- 只修改 `status` 字段：`pending` → `in_progress` → `complete`
- 当状态改为 `in_progress` 时，将 `started_at` 设置为当前 ISO 8601 时间戳
- 当 orchestrator 标记为 `complete` 时，将 `completed_at` 设置为当前 ISO 8601 时间戳
- 每次修改时始终更新 backlog 级别的 `updated` 时间戳
- 执行开始后绝不修改 `acceptance_criteria`（先与用户讨论）
- 使用 `.harness/backlogs/backlog-schema.json` 中的 schema 进行验证
- 开始新 backlog 时复制 `.harness/backlogs/_template.json`
- `priority` 决定多个 backlogs 之间的处理顺序（1 = 最高）

## Requirements Documents (`.harness/requirements/`)

- 文件命名为 `{slug}-requirements.md`
- 只来自 `@analyst` 输出 — 分析完成后不要手动编辑
- `FR-XX` 和 `NFR-XX` ID 必须保持稳定 — `@dev-evaluator` 会将实现追踪回这些 ID
- 包含带有 `created` 和 `updated` ISO 8601 时间戳的 YAML frontmatter block

## Architecture (`.harness/architecture/`)

- `adr/` 中的 ADRs 命名为 `ADR-{NNN}-{short-name}.md`
- 一旦 `Status: Accepted`，ADRs 即为强制约束 — 修改它们需要新的 ADR 取代旧 ADR
- `diagrams/` 中的图是在 `.md` 文件里的 Mermaid
- 每个 ADR 的 header 中必须包含 `Date:` 字段（ISO 8601 日期）

## Progress Notes (`.harness/progress.md`)

- 始终追加，绝不覆盖现有条目
- 使用文件底部符合角色的模板格式
- **所有 agents 都必须写入 progress.md** — 这是 pipeline 状态的单一事实来源
- Orchestrator 记录阶段转换和门禁决策（包括 per-task design & test-design review gate）
- Planning agents（analyst、architect、product-owner）记录产出的工件和 open questions
- Task agents（developer、evaluator、demonstrator）记录任务范围的活动和结果
- Developer 和 test-designer 每个任务运行**两次** — 先是 design mode（DESIGN / TEST-DESIGN），然后是 implementation mode；在每个条目中记录 mode，这样重新派生时知道正在恢复哪个阶段
- 包含：日期、agent 名称、状态、产出/验证内容、后续步骤

## 并行下的共享资源

当任务并发运行（`@orchestrator-parallel`）时，少数文件会被**每个**任务触碰，无法按 `files_likely_affected` 分区。对这些文件的写入会**一次只串行化一个任务**（按 task-ID 顺序）— 一个任务在当前写入者完成后才应用自己的追加/编辑：

- `.harness/progress.md` — 仅追加；串行化追加，避免两个 agents 的写入交错
- `Src/VerificationTests/VerificationTests.sln` — 只有 `<Project>` 注册行是共享的；每个任务自己的 `Src/VerificationTests/{slug}/Task{id}/` project files 是私有的，永不串行化
- 共享 QMS documents（`docs/qms/SDD.md`、`MVP.md`、`MVProcedure.md`、`VerificationPlan.md`、`VerificationProcedure.md`）— 见 qms-documentation skill

只有**写操作**串行化；每个任务贡献的内容对该任务是私有的，底层设计/实现工作继续并行。

## Eval Feedback (`.harness/eval_feedback/`)

- 文件命名为 `{backlog-slug}_{task-id}.json`
- 只来自 `@dev-evaluator` 输出 — 不要手动编辑
- 修复 evaluator 发现的问题时读取它们
- 每个文件包含一个 `evaluated_at` 字段（ISO 8601），在执行评估时设置

## Demo Evidence (`.harness/demo_evidence/`)

- 视频录制存储在 `{backlog-slug}/task-{id}-{timestamp}/demo.mp4`
- Demo log JSON 位于 `{backlog-slug}_{task-id}_demo.json`
- 只来自 `@feature-demonstrator` 输出 — 不要手动编辑
- Demo logs 不包含 pass/fail verdicts — 用户是门禁
- 每个 log 包含一个 `demonstrated_at` 字段（ISO 8601），在执行 demo 时设置
- 绝不捕获完整桌面 — 视频只录制应用程序窗口

## Tool Outputs (`.harness/tool_outputs/`)

- 文件存储在 `.harness/tool_outputs/{backlog-slug}_{task-id}/`（developer）和 `.harness/tool_outputs/{backlog-slug}_{task-id}_verification/`（test-designer）
- 由 `@developer` 和 `@test-designer` 在各自 Verify 步骤期间写入，使 `@dev-evaluator` 可以 trust-but-verify，而无需重新运行所有内容。两者写入不同文件夹，因为它们并行运行。
- Developer 文件夹包含：`build.log`（dotnet build）、`tests.trx`（dotnet test results）、`resharper.xml`（ReSharper CLI report）、`coverage.xml`（dotCover report）
- Test-designer `_verification` 文件夹包含：`VerificationTests.sln` 的 `build.log` 和 `resharper.xml`（测试代码与生产代码遵循相同的 build-clean / zero-ReSharper-error 标准）
- `@dev-evaluator` 将 combined coverage report 写入 `.harness/tool_outputs/{backlog-slug}_{task-id}_eval/coverage-combined.xml`（任务范围，因此并行评估不会冲突）
- `@dev-evaluator` 读取这些输出，并仅在输出缺失、过期、边界可疑或不可信时重新运行工具
- 这些是 runtime artefacts — 不要手动编辑

## Verification Tests (`Src/VerificationTests/`)

- 由 `@test-designer` 编写的独立 C# FlaUI test projects
- 存储在 `Src/VerificationTests/{slug}/Task{id}/` — 每个任务一个 NUnit test project
- Solution：`Src/VerificationTests/VerificationTests.sln`
- `@test-designer` 仅根据 feature files + requirements 编写这些内容，与 developer 并行，且绝不查看生产代码
- `@dev-evaluator` 运行这些测试（它不编写测试）并判断其质量
- 始终使用 C# + FlaUI + NUnit — 不使用 PowerShell scripts
- 所有测试都有 `[Category("Verification")]` attribute
- 针对 simulator mode 中的 live application 运行（不是 mocks）
- 可在 CI pipelines 中复用 — combined coverage = developer tests + verification tests
- 不要将 verification tests 存储在 `.harness/` — 它们是正式 repo assets
