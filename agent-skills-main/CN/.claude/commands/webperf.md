---
description: 通过 web-performance-auditor persona 运行 Web 性能审计
---

`/webperf` 专门面向 Web 应用程序。不要将它用于 utility libraries、CLIs，或没有浏览器端输出的 server-only code。

## 确定模式

**Deep mode** — 当以下任一项可用时启用：
- Lighthouse JSON 报告文件（例如 `npx lighthouse <url> --output json --output-path ./report.json`，或通过 Chrome DevTools MCP CLI 运行 `npx -p chrome-devtools-mcp chrome-devtools lighthouse_audit --output-format=json`）
- PageSpeed Insights JSON response（包含 Lighthouse + CrUX）
- CrUX API response（需要 `CRUX_API_KEY` 或 `GOOGLE_API_KEY`）
- DevTools performance trace
- live URL 加上 harness 中配置的 `chrome-devtools` MCP server（agent 可通过 `lighthouse_audit` 和 `performance_*` tools 直接捕获指标）
- 本地调用的 Chrome DevTools MCP CLI（通过 `npx -p chrome-devtools-mcp chrome-devtools <tool>`，或在 `npm i -g chrome-devtools-mcp` 之后）— 用户运行类似 `chrome-devtools lighthouse_audit --output-format=json` 的命令，并将 JSON 输出传给 agent

**Quick mode** — 当以上内容都不可用时的默认模式。agent 扫描源代码中的结构性 anti-patterns，并将每个发现标记为 `potential impact`。

## 运行审计

生成 `web-performance-auditor` subagent。明确传递给它：

- 正在审查的文件、组件或 diff
- 任何 artifact paths（Lighthouse JSON、PSI JSON、CrUX response、trace）或粘贴的 JSON 内容
- 已知时的 target URL 或 page name
- 你预期的模式说明（Quick 或 Deep），以便 agent 在本应使用 Deep 但缺少输入时指出

subagent 会返回 scorecard（只填入有来源的值）、按优先级排序的 findings、positive observations 和 proactive recommendations。

## 输出

向用户返回完整审计报告。不需要综合或合并步骤 — 这是单 persona 命令。