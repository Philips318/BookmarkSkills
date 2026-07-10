# 在 GitHub Copilot 中使用 agent-skills

## 设置

### Copilot Instructions

Copilot 支持使用仓库中的 `.github/skills`、`.claude/skills` 或 `.agents/skills` 目录创建 agent skills。

```bash
mkdir -p .github

# Create files for essential skills
cat /path/to/agent-skills/skills/test-driven-development/SKILL.md > .github/skills/test-driven-development/SKILL.md
cat /path/to/agent-skills/skills/code-review-and-quality/SKILL.md > .github/skills/code-review-and-quality/SKILL.md
```

更多详情请参考 [Creating agent skills for GitHub Copilot](https://docs.github.com/en/copilot/how-tos/use-copilot-agents/coding-agent/create-skills)。

### Agent Personas (*.agent.md)

Copilot 支持专门的 agent personas。使用 agent-skills agents：

> **Important:** GitHub Copilot 要求自定义 agent 文件命名为 `*.agent.md`。
> 命名为 `*.md` 的文件会被 Copilot 静默忽略。
> 详情见 [VS Code custom agents docs](https://code.visualstudio.com/docs/copilot/customization/custom-agents#_custom-agent-file-structure)。

```bash
# Create the agents directory and copy agent definitions
mkdir -p .github/agents
cp /path/to/agent-skills/agents/code-reviewer.md .github/agents/code-reviewer.agent.md
cp /path/to/agent-skills/agents/test-engineer.md .github/agents/test-engineer.agent.md
cp /path/to/agent-skills/agents/security-auditor.md .github/agents/security-auditor.agent.md
```

在 Copilot Chat 中调用 agents：
- `@code-reviewer Review this PR`
- `@test-engineer Analyze test coverage for this module`
- `@security-auditor Check this endpoint for vulnerabilities`

### Custom Instructions（用户级）

对于你希望在所有仓库中使用的 skills：

1. 打开 VS Code → Settings → GitHub Copilot → Custom Instructions
2. 添加你最常用的 skill 摘要

## 推荐配置

### .github/copilot-instructions.md

GitHub Copilot 支持通过 `.github/copilot-instructions.md` 设置项目级 instructions。

```markdown
# Project Coding Standards

## Testing
- Write tests before code (TDD)
- For bugs: write a failing test first, then fix (Prove-It pattern)
- Test hierarchy: unit > integration > e2e (use the lowest level that captures the behavior)
- Run `npm test` after every change

## Code Quality
- Review across five axes: correctness, readability, architecture, security, performance
- Every PR must pass: lint, type check, tests, build
- No secrets in code or version control

## Implementation
- Build in small, verifiable increments
- Each increment: implement → test → verify → commit
- Never mix formatting changes with behavior changes

## Boundaries
- Always: Run tests before commits, validate user input
- Ask first: Database schema changes, new dependencies
- Never: Commit secrets, remove failing tests, skip verification
```

### Specialized Agents

在 Copilot Chat 中使用这些 agents 来执行有针对性的评审工作流。

## 使用提示

1. **保持 instructions 简洁** — Copilot instructions 在聚焦时效果最好。总结关键规则，而不是包含完整 skill 文件。
2. **使用 agents 做评审** — code-reviewer、test-engineer 和 security-auditor agents 是为 Copilot 的 agent 模型设计的。
3. **在 chat 中引用** — 处理某个具体阶段时，将相关 skill 内容粘贴到 Copilot Chat 中作为上下文。
4. **结合 PR reviews** — 设置 Copilot 使用 code-reviewer agent persona 来评审 PR。
