# 在 Cursor 中使用 agent-skills

## 设置

### 选项 1：Rules 目录（推荐）

Cursor 支持 `.cursor/rules/` 目录，用于项目特定 rules：

```bash
# Create the rules directory
mkdir -p .cursor/rules

# Copy skills you want as rules
cp /path/to/agent-skills/skills/test-driven-development/SKILL.md .cursor/rules/test-driven-development.md
cp /path/to/agent-skills/skills/code-review-and-quality/SKILL.md .cursor/rules/code-review-and-quality.md
cp /path/to/agent-skills/skills/incremental-implementation/SKILL.md .cursor/rules/incremental-implementation.md
```

此目录中的 rules 会自动加载到 Cursor 的上下文中。

### 选项 2：.cursorrules 文件

在项目根目录创建一个 `.cursorrules` 文件，并内联必要 skills：

```bash
# Generate a combined rules file
cat /path/to/agent-skills/skills/test-driven-development/SKILL.md > .cursorrules
echo "\n---\n" >> .cursorrules
cat /path/to/agent-skills/skills/code-review-and-quality/SKILL.md >> .cursorrules
```

## 推荐配置

### 核心 Skills（始终加载）

将这些加入 `.cursor/rules/`：

1. `test-driven-development.md` — TDD 工作流和 Prove-It 模式
2. `code-review-and-quality.md` — 五轴评审
3. `incremental-implementation.md` — 以小而可验证的切片构建

### 阶段特定 Skills（按需加载）

对于阶段特定工作，根据需要创建额外 rule 文件：

- `spec-development.md` -> `spec-driven-development/SKILL.md`
- `frontend-ui.md` -> `frontend-ui-engineering/SKILL.md`
- `security.md` -> `security-and-hardening/SKILL.md`
- `performance.md` -> `performance-optimization/SKILL.md`

处理相关任务时，将这些加入 `.cursor/rules/`，完成后移除，以管理上下文限制。

## 使用提示

1. **不要一次加载所有 skills** - Cursor 有上下文限制。将 2-3 个核心 skills 作为 rules 加载，并按需添加阶段特定 skills。
2. **显式引用 skills** - 告诉 Cursor “Follow the test-driven-development rules for this change”，确保它读取已加载 rules。
3. **使用 agents 做评审** - 复制 `agents/code-reviewer.md` 内容，并告诉 Cursor “review this diff using this code review framework.”
4. **按需加载 references** - 处理性能工作时，将 `performance.md` 添加到 `.cursor/rules/`，或直接粘贴 checklist 内容。
