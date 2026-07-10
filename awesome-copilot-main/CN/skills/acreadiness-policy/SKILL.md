---
name: acreadiness-policy
description: 'Help the user pick, write, or apply an AgentRC policy. Policies customise readiness scoring by disabling irrelevant checks, overriding impact/level, setting pass-rate thresholds, or chaining org baselines with team overrides. Use when the user asks about strict mode, AI-only scoring, custom weights, CI gating, or wants org-wide standardisation.'
argument-hint: "[show | new <name> | apply <path-or-pkg>] — e.g. /acreadiness-policy show, /acreadiness-policy new strict-frontend"
---
# / acreadness -policy - AgentRC策略

当用户询问**策略**、**严格模式**、**自定义评分**、**禁用检查**、**org标准**或**CI控制**时，使用此技能。

策略是一个小的JSON文件，包含三个可选部分——`criteria`、`extras`、`thresholds`——用于定制AgentRC如何对准备情况进行评分。

##内置示例

AgentRC在`examples/policies/`中附带了三个示例策略：

|政策|它做什么||---|---|
|`strict.json`| 100%通过率，提高对关键标准|的影响
|`ai-only.json`|禁用所有回购健康检查，专注于AI工具|
|`repo-health-only.json`|禁用AI检查，专注于传统质量|

在编写自定义策略之前，建议将这些作为起点。

##策略模式```jsonc
{
  "name": "my-policy",
  "criteria": {
    "disable":  ["env-example", "observability", "dependabot"],
    "override": {
      "readme":      { "impact": "high", "level": 2 },
      "lint-config": { "title": "Linter required" }
    }
  },
  "extras": {
    "disable": ["pre-commit"]
  },
  "thresholds": {
    "passRate": 0.9
  }
}
```
影响权重

|影响|权重||---|---|
|临界| 5 |
|高| 4 |
|中| 3 |
|低| 2 |
| info | 0 |`Score = 1 − (deductions / max possible weight)`。成绩:* * * *≥0.9,* * B * *≥0.8,* * C * *≥0.7,* * D * *≥0.6,F * * * * < 0.6。

# # Sub-commands

# # #`show`列出当前有效的策略（来自`agentrc.config.json``policies`数组，或者没有）。

# # #`new <name>`用合理的默认值支撑`policies/<name>.json`。引导用户完成：
1. **什么禁用** -无关的支柱或额外的堆栈（例如禁用`observability`为静态站点）。
2. ** * -覆盖`impact`到`high`或`critical`的必须（例如`readme`，`codeowners`）。
3. **通过率阈值** -典型的组织基准：`0.7`（宽松），`0.85`（标准），`1.0`（严格）。
4. 参考`agentrc.config.json`的策略：   ```json
   { "policies": ["./policies/<name>.json"] }
   ```
# # #`apply <path-or-pkg>`运行`agentrc readiness --json --policy <source>`，并通过将报告移交给`assess`技能/`ai-readiness-reporter`代理来重新呈现报告。支持链接:```bash
npx -y github:microsoft/agentrc readiness --json --policy ./org-baseline.json,./team-frontend.json
```
## CI门控

将策略与`--fail-level`结合起来，在CI中强制执行最低成熟度级别：```yaml
- run: npx -y github:microsoft/agentrc readiness --policy ./policies/strict.json --fail-level 3
```
# #先进

JSON策略可以禁用、覆盖和设置阈值，但是不能添加新的标准。对于新的检测逻辑，指向AgentRC的TypeScript插件系统（`docs/dev/plugins.md`）。

##操作规则

- **永远不要静默地禁用一个支柱。**如果用户想要禁用`observability`，确认并解释权衡。
- **优先覆盖`impact`而不是禁用。**禁用完全隐藏差距；重写使其仍然显示在报表中。
- **建议保持启用。**他们不花钱-他们不影响得分。
- **建议分层** -大多数组织想要一个基线策略+每个团队覆盖链接`--policy a.json,b.json`。