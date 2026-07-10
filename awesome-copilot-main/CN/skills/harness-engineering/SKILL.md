---
name: harness-engineering
description: 'Adopt repository-level harness engineering for coding agents. Use when a user wants to prevent repeated AI coding-agent mistakes by turning failures into durable instructions, drift checks, regression tests, failure memory, and adoption reports tailored to the target repository.'
---
#线束工程

线束工程将重复的编码代理错误变成持久的
库中构件:```text
Harness = Instructions + Constraints + Feedback + Memory + Evaluation + Governance
```
当用户请求时使用此技能：

-使存储库对GitHub Copilot或其他编码代理更可靠
-添加持久代理指令、存储库规则或护栏
-防止重复AI编码代理错误
-记录已知的故障路径和防止再次发生的检查
-为项目规则添加轻量级漂移检查
-检查、刷新或更新现有的代理线束

除非用户要求，否则不要将此技能用于普通功能的实现
改进存储库的代理操作环境。

##核心原则—将目标存储库视为真相的来源。
-编辑前检查。保留现有的堆栈、包管理器、CI、
文档、命名和体系结构。
-添加最小的有用线束。更新现有文件优于添加文件
重复的指导。
-通过测试、测试、
类型检查、CI、预提交钩子或漂移脚本。
-只有当自动化会变得脆弱或容易误导时才使用手动审查点。
-记录不应再发生的高风险故障，并命名检查或评审
抓住递归的点。
—不要盲目复制通用模板。把每件藏物都变成真实的证据
在目标存储库中。

# #发现

在提出建议或进行线束更改之前，检查现有的存储库
规则和证据。

当这些文件和文件夹存在时，读取它们：- `README.md`
- `AGENTS.md`
- `.github/copilot-instructions.md`
——`.github/instructions/`——`.github/workflows/`- `CONTRIBUTING.md`
-包清单，如`package.json`，`pyproject.toml`,`go.mod`，`Cargo.toml`,`pom.xml`，或`build.gradle`-现有文档在`docs/`下
-`scripts/`下的现有脚本
-现有测试和CI检查

然后总结:

-堆栈、包管理器和入口点
—已有的开发和验证命令
-当前代理指令或存储库约定
-已知的失败、事件、不可靠的路径或重复的评审意见
-未执行项目规则的空白

##采用流程

遵循以下顺序：

1. 选择适合目标存储库的线束表面。
2. 编写特定于目标的代理指令。
3. 为高价值规则添加可强制执行的检查。
4. 对高风险或反复发生的故障进行故障记忆记录。
5. 增加漂移检查，引导可能会无声地变得陈旧。
6. 报告采用情况，包括证据、假设和后续情况。

# # # 1。选择线束表面只选择适合目标存储库的表面：

|需要|首选工件|| --- | --- |
|始终在线代理行为|`AGENTS.md`或`.github/copilot-instructions.md`|
|文件作用域指导|`.github/instructions/*.instructions.md`|
|循环项目检查|`scripts/check_*.py`、shell脚本或包脚本|
| CI执行|现有工作流文件或小型新工作流|
|已知故障|`docs/failures/*.md`|
|架构或流程决策|`docs/decisions/*.md`|
|采用证据|`docs/harness/adoption-report.md`或类似|

如果存储库已经具有等效的位置，则更新它而不是
创建一个并行系统。

# # # 2。编写代理指令

代理指示应该是具体的和可操作的。包括:

-工程项目用途及主要业权界限
- setup, test, lint， build和verification命令
-包管理器和依赖规则
—安全编辑规则、生成文件规则和禁止路径
-测试对更改代码的期望
- PR和提交约定(如果回购有的话
-如何记录新的失败或决定避免宽泛的个性指导、通用的最佳实践和不能做到这一点的规则
被检查或审查。

# # # 3。添加可执行的检查

将高价值规则转换为检查。良好的线束检查是：

-足够窄以避免误报
-足够快，可以在本地和CI中运行
-明确命名，以便代理可以在完成之前运行它们
-记录他们所保护的规则

例子:```text
Rule: Do not edit generated API clients.
Check: script scans diffs for generated paths and fails with a clear message.

Rule: Every failure memory note names a regression check.
Check: script validates docs/failures/*.md for a "Detection" section.

Rule: Profile docs and templates must stay aligned.
Check: test compares profile README files to expected template files.
```
# # # 4。记录故障内存

当故障是用户可见的、高风险的或可能再次发生时，记录故障。
使用`docs/failures/`下的新文件，除非已有注释已经覆盖
同样的根本原因。

推荐的结构:```markdown
# Short Failure Title

## Summary

What failed, who saw it, and why it matters.

## Root Cause

The technical or process cause. Avoid blame.

## Prevention

Instruction, test, drift check, CI gate, fixture, or manual review point that
prevents or detects recurrence.

## Evidence

Links to issue, PR, test, log, command output, or file paths.
```
如果没有实际的自动检查，记录人工检查点和原因
自动化将是不安全或误导的。

# # # 5。添加漂移检查

使用漂移检查来进行可能会变得过时的引导。常见的例子:

- docs提到不再存在的命令
-配置文件片段和生成的示例不同
-失败说明省略回归检查
-缺少结构变更的决策记录
—CI引用过时的脚本或包命令

更喜欢使用存储库现有语言的小脚本。如果回购有
没有脚本约定，只有标准库的Python是可移植的
违约。

# # # 6。报告领养情况

完成大量的线束工作，并提交一份采用报告，其中包括：-文件已更改
-添加或更新的规则
-添加或重用的检查
-运行的命令和结果
-假设和手动跟踪
-已创建或有意跳过的故障内存
-如何衡量效率

##审查工作流程

当被要求审查一个控制变化时，采取相反的观点。寻找:

-在没有证据的情况下从目标存储库复制通用规则
-重复或冲突的指令文件
-对有效更改可能失败的广泛检查
-未执行的高风险规则
-缺少重复错误或运行失败的失败记忆
-生成的文档在源代码更改后不刷新
-不运行相关检查的CI门
-目标存储库约定被harness默认值覆盖首先报告发现，按严重程度排序，当有文件和行引用时
可用。除非用户明确要求，否则不要在评审期间修改文件
修复。

##输出合同

在完成线束采用工作之前，验证：

-在编辑之前检查了目标存储库
-新的指导是针对目标存储库的
-变更后的检查可以在本地运行或有书面的人工替代
-在需要时记录故障记忆，或最终响应解释原因
它被跳过了
-刷新生成的文档或索引
-最终报告命名每个命令运行及其结果

##可选参考

提示优先的工作流程`https://github.com/baskduf/harness-starter-kit`是一个参考实现
这些想法。只有当用户要求时，才将其用作参考资料
当存储库已经包含它时。目标存储库仍然是
真相的来源。