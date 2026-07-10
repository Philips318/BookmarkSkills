# agent-skills

这是 agent-skills 项目，一组面向 AI 编码代理的生产级工程技能集合。

## 项目结构

```
skills/       → 核心技能（每个目录一个 SKILL.md）
agents/       → 可复用的代理角色（code-reviewer、test-engineer、security-auditor、web-performance-auditor）
hooks/        → 会话生命周期钩子
.claude/commands/ → 斜杠命令（/spec、/plan、/build、/test、/review、/code-simplify、/ship，以及 /webperf 专项审计）
references/   → 补充检查清单（测试、性能、安全、可访问性、可观测性）
evals/        → 技能评测用例与框架（见 evals/README.md）
docs/         → 不同工具的设置指南
```

## 按阶段划分的技能

**定义：** interview-me、idea-refine、spec-driven-development
**计划：** planning-and-task-breakdown
**构建：** incremental-implementation、test-driven-development、context-engineering、source-driven-development、doubt-driven-development、frontend-ui-engineering、api-and-interface-design
**验证：** browser-testing-with-devtools、debugging-and-error-recovery
**审查：** code-review-and-quality、code-simplification、security-and-hardening、performance-optimization
**发布：** git-workflow-and-versioning、ci-cd-and-automation、deprecation-and-migration、documentation-and-adrs、observability-and-instrumentation、shipping-and-launch

## 约定

- 每个技能都位于 `skills/<name>/SKILL.md`
- YAML frontmatter 包含 `name` 和 `description` 字段
- `description` 以技能做什么开头（第三人称），随后说明触发条件（“Use when...”）
- 每个技能都包含：Overview、When to Use、Process、Common Rationalizations、Red Flags、Verification
- 参考资料放在 `references/` 中，而不是技能目录内部
- 只有当支撑文件内容超过 100 行时才创建支撑文件

## 贡献

在添加新技能或显著重做现有技能之前，请先运行 [CONTRIBUTING.md](CONTRIBUTING.md#before-proposing-a-new-skill) 中的预检：搜索目录、检查开放 PR、确认想法符合 [docs/skill-anatomy.md](docs/skill-anatomy.md)，并说明差距。优先扩展现有技能，而不是添加近似重复的技能。CONTRIBUTING.md 是该工作流的唯一事实来源；不要在这里或其他地方重复它的检查清单，而应链接过去。

## 命令

- `npm test` — 不适用（这是一个文档项目）
- 验证：检查所有 SKILL.md 文件是否具有有效的 YAML frontmatter，并包含 `name` 与 `description`
- 评测：`node scripts/run-evals.js` — 对每个技能运行触发/路由评测（CI）；使用 `--behavioral <skill>` 运行分级评测

## Pull Requests

PR 应指向上游仓库的默认分支。在典型 fork 设置中，上游远程通常是 `upstream`，你的 fork 是 `origin`，但具体远程名称并不是重点。

- 打开 PR 前，搜索上游仓库中开放的 PR 和 issue，查看是否有触及相同文件或规则的工作。如果有重叠，请先协调（基于它继续、让规则保持一致，或等它合并后 rebase），不要打开冲突 PR。
- 优先提交小而聚焦的 PR，避免对广泛共享的文件（例如 `scripts/` 下的文件）做大型重构，因为这类改动更容易与进行中的工作冲突。

## 边界

- 始终：创建新技能目录前，运行 CONTRIBUTING.md 中的预检
- 始终：新技能遵循 skill-anatomy.md 格式
- 始终：打开新 PR 前，检查上游仓库开放的 PR 和 issue 是否有重叠
- 绝不：添加只提供模糊建议、而不是可执行流程的技能
- 绝不：在技能之间复制内容，应引用其他技能