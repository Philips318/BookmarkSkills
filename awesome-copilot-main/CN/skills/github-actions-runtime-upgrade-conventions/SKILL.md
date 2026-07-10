---
name: github-actions-runtime-upgrade-conventions
description: 'Upgrade GitHub Actions to supported runtimes by selecting safe action versions, preserving workflow behavior, and validating post-upgrade execution.'
---
#GitHub Actions运行时升级约定

在编辑GitHub Actions工作流时使用此技能来处理有关操作运行时的弃用警告（例如Node.js运行时迁移）。

使用此技能时

—工作流日志报告一个操作在一个弃用的运行时运行。
-你正在升级`.github/workflows/*.yml`或`.github/workflows/*.yaml`的动作版本。
-您需要在现代化操作依赖关系的同时保持现有的工作流行为。

##升级规则-更喜欢升级到最新的稳定** *主要**版本的每个行动，是兼容的工作流程。
-首选不可变引脚：将目标版本解析为完整的提交SHA，并在`uses:`中使用该SHA。
-在最终建议中不要固定到可变标签或分支（例如`@v4`或`@main`）。
每次提交一次升级一个操作（或一个紧密相关的组），这样失败就很容易隔离。
-在升级runtime/dependency动作时保持现有工作流行为不变。

##行动我们跟踪在这个回购

当出现警告时，优先考虑这些组的运行时审查：

-`actions/*`下的任何第一方行动
-特别是在`actions/setup-*`下的设置动作（例如`setup-node`，`setup-python`,`setup-dotnet`）
-在工作流日志中由运行时弃用警告显式命名的任何其他操作

##钉钉图案```yaml
steps:
  - uses: actions/checkout@8ade135a41bc03ea155e62e844d188df1ea18608 # v4.3.1
  - uses: actions/setup-node@60edb5dd545a775178f52524783378180af0d1f8 # v4.0.4
```
在推荐升级时，首先确定最新的兼容版本，然后使用相应的提交SHA和可选的版本注释。

##验证清单

更改动作版本后：

1. 确保所有编辑的工作流仍然解析并保持相同的triggers/permissions，除非有意更改。
2. 运行受影响的工作流（或等效的本地build/test命令）并确认升级步骤成功完成。
3. 在适用的情况下，确认release/signing/artifact步骤仍然产生预期的输出。
4. 检查工作流运行日志，查看任何新的弃用警告或运行时迁移注释。

## PR说明

在PR总结中包括：

-哪些动作升级了（从->升级到）。
是否有动作不能转到新专业，为什么？
-重新运行哪些工作流以验证更改。

这是如何补充Dependabot的

Dependabot可以自动进行许多更新，但这项技能在以下情况下仍然有用：—仓库中的工作流没有启用Dependabot。
-在自动更新可用之前出现运行时警告。
-工作流需要在动作碰撞后保持行为的验证。