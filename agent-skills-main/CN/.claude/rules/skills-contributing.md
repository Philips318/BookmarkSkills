---
description: 添加或更改技能时的反重复护栏
paths:
  - "skills/**"
---

# 添加或更改技能

本仓库已经覆盖了开发生命周期的大部分内容，因此多数新技能想法会与现有技能或开放 PR 重叠。在创建新的 `skills/<name>/` 目录或大幅重做现有技能之前：

- 运行 [CONTRIBUTING.md](../../CONTRIBUTING.md#before-proposing-a-new-skill) 中的预检：搜索目录，检查开放 PR（`gh pr list --state open`），并说明缺口。
- 优先扩展现有技能，而不是添加近似重复项。如果想法与现有技能重叠，请编辑该技能，而不是添加新目录。
- 让 `SKILL.md` 符合 [docs/skill-anatomy.md](../../docs/skill-anatomy.md)，并且绝不要在技能之间重复内容；请改为引用其他技能。

CONTRIBUTING.md 是完整工作流的唯一事实来源；此规则只指向它，而不是复述它的检查清单。