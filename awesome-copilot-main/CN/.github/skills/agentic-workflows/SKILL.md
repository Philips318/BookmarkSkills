---
name: agentic-workflows
description: Route gh-aw workflow design/create/debug/upgrade requests to the right prompts.
---
#代理工作流路由器

当用户要求在此存储库中设计、创建、更新、调试或升级GitHub代理工作流时，使用此技能。

该技能是一个调度程序：识别任务类型，加载匹配的工作流prompt/skill文件，并直接执行它。保持回答的简洁，如果不清楚正确的提示，问一个澄清性的问题。

只读取你需要的文件：
从`github/gh-aw`加载这些文件（它们在本地不可用）。- `.github/aw/agentic-chat.md`
- `.github/aw/agentic-workflows-mcp.md`
- `.github/aw/asciicharts.md`
- `.github/aw/campaign.md`
- `.github/aw/charts-trending.md`
- `.github/aw/charts.md`
- `.github/aw/cli-commands.md`
- `.github/aw/context.md`
- `.github/aw/create-agentic-workflow.md`
- `.github/aw/create-shared-agentic-workflow.md`
- `.github/aw/debug-agentic-workflow.md`
- `.github/aw/dependabot.md`
- `.github/aw/deployment-status.md`
- `.github/aw/experiments.md`
- `.github/aw/github-agentic-workflows.md`
- `.github/aw/github-mcp-server.md`
- `.github/aw/instructions.md`
- `.github/aw/llms.md`
- `.github/aw/loop.md`
- `.github/aw/lsp.md`
- `.github/aw/mcp-clis.md`
- `.github/aw/memory-stateful-patterns.md`
- `.github/aw/memory.md`
- `.github/aw/messages.md`
- `.github/aw/network.md`
- `.github/aw/optimize-agentic-workflow.md`
- `.github/aw/patterns.md`
- `.github/aw/pr-reviewer.md`
- `.github/aw/report.md`
- `.github/aw/reuse.md`
- `.github/aw/safe-outputs-automation.md`
- `.github/aw/safe-outputs-content.md`
- `.github/aw/safe-outputs-management.md`
- `.github/aw/safe-outputs-runtime.md`
- `.github/aw/safe-outputs.md`
- `.github/aw/serena-tool.md`
- `.github/aw/shared-safe-jobs.md`
- `.github/aw/skills.md`
- `.github/aw/subagents.md`
- `.github/aw/syntax-agentic.md`
- `.github/aw/syntax-core.md`
- `.github/aw/syntax-tools-imports.md`
- `.github/aw/syntax.md`
- `.github/aw/test-coverage.md`
- `.github/aw/test-expression.md`
- `.github/aw/token-optimization.md`
- `.github/aw/triggers.md`
- `.github/aw/update-agentic-workflow.md`
- `.github/aw/upgrade-agentic-workflows.md`
- `.github/aw/visual-regression.md`
- `.github/aw/workflow-constraints.md`
- `.github/aw/workflow-editing.md`
- `.github/aw/workflow-patterns.md`

- `.github/skills/agentic-workflow-designer/SKILL.md`
加载匹配的工作流提示或技能后，直接按照它操作：
-通过面试从头开始设计工作流程：`skills/agentic-workflow-designer/SKILL.md`—创建新的工作流：`.github/aw/create-agentic-workflow.md`—更新现有工作流：`.github/aw/update-agentic-workflow.md`-调试、审计或调查工作流：`.github/aw/debug-agentic-workflow.md`-升级工作流和修复弃用：`.github/aw/upgrade-agentic-workflows.md`—创建共享组件或MCP封装：`.github/aw/create-shared-agentic-workflow.md`—创建报表生成工作流：`.github/aw/report.md`修复了依赖于manifest pr:`.github/aw/dependabot.md`—分析覆盖工作流：`.github/aw/test-coverage.md`-渲染紧凑的降价图表：`.github/aw/asciicharts.md`—将CLI命令映射到MCP使用情况：`.github/aw/cli-commands.md`—选择工作流架构和模式：`.github/aw/patterns.md`-优化令牌使用和成本：`.github/aw/token-optimization.md`当任务涉及OTEL、OTLP、跟踪、可观察性后端或遥测驱动的分析时，在加载匹配的工作流提示或技能后，也要读取并遵循`skills/otel-queries/SKILL.md`。