---
description: 运行 TDD 工作流 — 编写失败测试、实现、验证。对于 bugs，使用 Prove-It 模式。
---

调用 agent-skills:test-driven-development 技能。

对于新功能：
1. 编写描述预期行为的测试（它们应该 FAIL）
2. 实现代码使测试通过
3. 在保持测试为 green 的同时重构

对于 bug fixes（Prove-It 模式）：
1. 编写一个复现 bug 的测试（必须 FAIL）
2. 确认该测试失败
3. 实现修复
4. 确认测试通过
5. 运行完整测试套件检查回归

对于浏览器相关问题，还要调用 agent-skills:browser-testing-with-devtools，以通过 Chrome DevTools MCP 验证。