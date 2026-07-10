---
description: 进行五轴代码审查 — correctness、readability、architecture、security、performance
---

调用 agent-skills:code-review-and-quality 技能。

从全部五个轴向审查当前更改（staged 或最近提交）：

1. **Correctness** — 是否符合 spec？边界情况处理了吗？测试足够吗？
2. **Readability** — 命名清晰吗？逻辑直接吗？组织良好吗？
3. **Architecture** — 是否遵循现有模式？边界清晰吗？抽象层级合适吗？
4. **Security** — 输入已验证吗？secrets 安全吗？auth 已检查吗？（使用 security-and-hardening 技能）
5. **Performance** — 没有 N+1 queries 吗？没有无界操作吗？（使用 performance-optimization 技能）

将发现归类为 Critical、Important 或 Suggestion。
输出带有具体 file:line 引用和修复建议的结构化审查。