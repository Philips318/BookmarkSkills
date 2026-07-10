# Skill Anatomy

本文档描述 agent-skills skill 文件的结构和格式。贡献新 skills 或理解现有 skills 时，请将其作为指南。

## File Location

每个 skill 都位于 `skills/` 下自己的目录中：

```
skills/
  skill-name/
    SKILL.md           # Required: The skill definition
    scripts/           # Optional: Runnable helpers used by the skill workflow
    supporting-file.md # Optional: Reference material loaded on demand
```

`SKILL.md` 是唯一必需文件。只有当 skill 真正附带可运行 helpers 时才添加 `scripts/`，对于纯 markdown skills 请完全省略该目录。

## SKILL.md Format

### Frontmatter（必需）

```yaml
---
name: skill-name-with-hyphens
description: Guides agents through [task/workflow]. Use when [specific trigger conditions].
---
```

**Rules:**
- `name`: 小写、用连字符分隔。必须与目录名匹配。
- `description`: 先用第三人称说明该 skill 做什么，然后包含一个或多个清晰的 “Use when” 触发条件。同时包含 *what* 和 *when*。最多 1024 个字符。

**为什么重要：** Agents 通过读取 descriptions 来发现 skills。description 会注入 system prompt，因此它必须告诉 agent 该 skill 提供什么，以及何时激活。不要总结工作流 — 如果 description 包含流程步骤，agent 可能会遵循摘要，而不是读取完整 skill。

### Standard Sections（推荐模式）

上面的 frontmatter contract 是必需的。下面的 section layout 是推荐模式，不是僵硬模板：只要等价标题能清晰服务相同目的，也可以接受。

```markdown
# Skill Title

## Overview
One-two sentences explaining what this skill does and why it matters.

## When to Use
- Bullet list of triggering conditions (symptoms, task types)
- When NOT to use (exclusions)

## [Core Process / The Workflow / Steps]
The main workflow, broken into numbered steps or phases.
Include code examples where they help.
Use flowcharts (ASCII) where decision points exist.

## [Specific Techniques / Patterns]
Detailed guidance for specific scenarios.
Code examples, templates, configuration.

## Common Rationalizations
| Rationalization | Reality |
|---|---|
| Excuse agents use to skip steps | Why the excuse is wrong |

## Red Flags
- Behavioral patterns indicating the skill is being violated
- Things to watch for during review

## Verification
After completing the skill's process, confirm:
- [ ] Checklist of exit criteria
- [ ] Evidence requirements
```

## Section Purposes

### Overview
该 skill 的“电梯陈述”。应回答：这个 skill 做什么，为什么 agent 应该遵循它？

### When to Use
帮助 agents 和人类判断此 skill 是否适用于当前任务。包含正向触发条件（“Use when X”）和负向排除条件（“NOT for Y”）。

### Core Process
skill 的核心。这是 agent 遵循的逐步工作流。必须具体且可执行 — 不是模糊建议。

**Good:** “Run `npm test` and verify all tests pass”
**Bad:** “Make sure the tests work”

### Common Rationalizations
精心编写的 skills 最有辨识度的特性。这些是 agents 用来跳过重要步骤的借口，并配有反驳。它们防止 agent 通过合理化逃避流程。

想想 agent 每次说 “I'll add tests later” 或 “This is simple enough to skip the spec” 的时候 — 这些就应该放在这里，并加上事实性反驳。

### Red Flags
可观察到的 skill 被违反的信号。对代码评审和自我监控很有用。

### Verification
退出标准。agent 用此 checklist 确认 skill 流程已经完成。每个 checkbox 都应能通过证据验证（test output、build result、screenshot 等）。

## Supporting Files

只有在以下情况下才创建 supporting files：
- 参考材料超过 100 行（保持 main SKILL.md 聚焦）
- 需要 code tools 或 scripts
- Checklists 足够长，值得单独成文件

当 patterns 和 principles 少于 50 行时，保持内联。

如果某个 skill 不需要可运行 helpers，不要为了模仿其他 skills 而创建空的 `scripts/` 目录。空目录只会增加噪音，不会改变 skill 的工作方式。

## Context Efficiency

Skills 按需加载：启动时只有 skill name 和 description 位于上下文中。完整 `SKILL.md` 只有在 agent 判定 skill 相关时才加载。为了让加载成本低：

- **保持 `SKILL.md` 少于 500 行。** 将详细参考材料移入 supporting files。
- **编写具体 descriptions。** 精确的 description 帮助 agent 在合适时机激活 skill，并在其他时候跳过。
- **使用渐进披露。** 引用 supporting files，只在工作流到达相应步骤时读取它们。
- **优先使用 scripts，而不是内联代码。** 执行 script 不消耗上下文；只有输出会消耗。内联代码块每次加载都要付出成本。
- **保持文件引用一层深。** 从 `SKILL.md` 直接链接到 supporting files，而不是通过中间文档串联。

## Script Requirements

当 skill 在 `scripts/` 下附带可运行 helpers 时，每个 script 遵循这些约定：

- 使用 `#!/bin/bash` shebang。
- 使用 `set -e` 实现 fail-fast 行为。
- 将状态消息写入 stderr：`echo "Message" >&2`。
- 将机器可读输出（JSON）写入 stdout。
- 为临时文件包含 cleanup trap。
- 将 script path 引用为 `skills/<skill-name>/scripts/<script>.sh`（repo-relative）。

## Writing Principles

1. **Process over knowledge.** Skills 是工作流，不是参考文档。要写步骤，而不是事实。
2. **Specific over general.** “Run `npm test`” 胜过 “verify the tests”。
3. **Evidence over assumption.** 每个 verification checkbox 都需要证明。
4. **Anti-rationalization.** 每个容易被跳过的步骤都需要在 rationalizations table 中有反驳。
5. **Progressive disclosure.** Main SKILL.md 是入口点。Supporting files 只在需要时加载。
6. **Token-conscious.** 每个 section 都必须证明自己有必要。如果删除它不会改变 agent 行为，就删除它。

## Naming Conventions

- Skill directories: `lowercase-hyphen-separated`
- Skill files: `SKILL.md`（始终大写）
- Supporting files: `lowercase-hyphen-separated.md`
- References: 存放在项目根目录的 `references/` 中，而不是 skill 目录内

## Cross-Skill References

按名称引用其他 skills：

```markdown
Follow the `test-driven-development` skill for writing tests.
If the build breaks, use the `debugging-and-error-recovery` skill.
```

不要在 skills 之间重复内容 — 改为引用和链接。

## Required vs Recommended

Required:

- 一个 `skills/<skill-name>/SKILL.md` 文件
- 有效的 YAML frontmatter，包含 `name` 和 `description`
- description 同时包含该 skill 做什么以及何时使用

Recommended:

- 上方展示的标准 section flow
- 当读起来对 skill 更自然时，使用等价标题，例如 `How It Works`、`Core Process` 或 `Workflow`
- 仅当 supporting files 能保持 main `SKILL.md` 聚焦时才使用
