# 在 Windsurf 中使用 agent-skills

## 设置

### Project Rules

Windsurf 使用 `.windsurfrules` 作为项目特定的 agent instructions：

```bash
# Create a combined rules file from your most important skills
cat /path/to/agent-skills/skills/test-driven-development/SKILL.md > .windsurfrules
echo "\n---\n" >> .windsurfrules
cat /path/to/agent-skills/skills/incremental-implementation/SKILL.md >> .windsurfrules
echo "\n---\n" >> .windsurfrules
cat /path/to/agent-skills/skills/code-review-and-quality/SKILL.md >> .windsurfrules
```

### Global Rules

对于你希望在所有项目中使用的 skills，请将它们添加到 Windsurf 的 global rules：

1. 打开 Windsurf → Settings → AI → Global Rules
2. 粘贴你最常用 skills 的内容

## 推荐配置

让 `.windsurfrules` 聚焦在 2-3 个核心 skills 上，以保持在上下文限制内：

```
# .windsurfrules
# Essential agent-skills for this project

[Paste test-driven-development SKILL.md]

---

[Paste incremental-implementation SKILL.md]

---

[Paste code-review-and-quality SKILL.md]
```

## 使用提示

1. **有选择地加载** — Windsurf 的上下文有限。选择能解决你最大质量缺口的 skills。
2. **在对话中引用** — 处理特定阶段时，将额外 skill 内容粘贴到 chat 中（例如构建 auth 时粘贴 `security-and-hardening`）。
3. **将 references 当作 checklists 使用** — 粘贴 `references/security-checklist.md`，并要求 Windsurf 验证每一项。
