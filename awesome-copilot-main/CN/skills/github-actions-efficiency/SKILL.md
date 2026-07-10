---
name: github-actions-efficiency
description: 'Audit GitHub Actions workflow efficiency and recommend fixes to reduce CI minutes and costs.'
---
#GitHub Actions将此技能作为高效工作的精益切入点。检查repo，确定浪费源，只加载当前任务所需的参考材料。

如果还没有工作流存在，在继续执行下面的步骤之前，加载[`references/actions.md`]（./references/actions.md）并定义一个基线。

**如果无法访问shell或`gh`命令行：**要求用户粘贴`.github/workflows/`内容和`gh run list --limit 10`输出。如果只提供了部分文件，请注明：“仅根据提供的文件进行审核；有些见解可能是不完整的。”仅从文件开始响应：“**仅静态分析**（未通过实时运行确认）。”

使用此技能时—用户希望减少GitHub Actions运行时、CI成本或浪费的工作流运行。
-回购有现有的工作流在`.github/workflows/`或明确的GitHub Actions配置问题。
-用户要求缓存，并发，路径过滤器，矩阵缩减，作业优化，或工作流特定的修复。
-用户需要帮助从头开始创建新的GitHub Actions工作流或CI基线。

##只加载你需要的

- [`references/actions.md`](./references/actions.md) -审计、作业门控、矩阵缩减、实时验证和特定工作流修复。
- [`references/reporting.md`](./references/reporting.md) -当用户要求before/after效率报告时。
—[`references/patterns.md`]（./references/patterns.md）—当内联审计命令不够时，完整的YAML示例。

核心工作流程

# # # 1。第一次测量```bash
rg -n "on:|concurrency:|paths:|paths-ignore:|strategy:|matrix:|cache:" .github/workflows
gh run list --limit 10
run_id=$(gh run list --limit 1 --json databaseId --jq '.[0].databaseId')
gh run view "$run_id" --log-failed
```
注意：缺少依赖项缓存、缺少`concurrency`取消、过于宽泛的触发器、重复的工作流覆盖，以及无论范围如何，每次更改都运行昂贵的作业。

# # # 2。应用护栏

在推荐之前，请根据这些规则检查每个提议的修复：1. 不隐藏所需的验证-删除删除发布、架构、迁移或共享库检查的任何修复。
2. 在没有理由的情况下不会降低并行度——除非用户优先考虑成本而不是延迟，并且新的关键路径保持在原始路径的1.25倍以内。
3. 只保留已记录的矩阵分支-删除没有明确版本或平台承诺的矩阵分支。
4. 回写作业使用可选触发器-标记（不删除）自动运行的格式化程序或bot作业；建议使用可选择的触发器。
5. Repo更改与org设置分开-将任何将可编辑的YAML与org级别或github帐户设置混合的修复拆分为两个不同的建议。

# # # 3。选择前3个修复从下面的六个候选中，只保留那些有步骤1 *和*的审计证据支持的，并通过步骤2的所有护栏。根据估计每天节省的CI分钟（每次运行节省×每天运行）对幸存者进行排名。选择所有符合这两个标准的候选人，最多3人。

1. 使用基于锁文件的键添加依赖缓存
2. 添加或更正`concurrency`消去
3. 在合并作业之前删除重复的工作流覆盖
4. 安全地缩小工作流或作业触发器
5. 减少矩阵宽度以匹配风险和事件类型
6. 在关键路径上并行化独立的作业

# # # 4。验证

—如果`gh`CLI访问可用，则在非受保护的分支上通过实时测试推送验证路径控制和并发取消。
-如果现场验证是不可能的，在输出中明确说明。
-将意外的实时行为视为真正的bug，即使YAML看起来是正确的。##所需输出

1. **浪费来源** -在步骤1中发现的最高成本或延迟驱动程序
2. **建议修复** -前3名（或所有剩余）与支持审计证据
3. **验证** -哪些已被证明是有效的，哪些仅在本地检查过，以及任何剩余的风险
4. **影响** -预期节约与实际节约；将PR挂钟时间与总运行时间分开

# #引用

——[`references/actions.md`] (./references/actions.md)
——[`references/reporting.md`] (./references/reporting.md)
——[`references/patterns.md`] (./references/patterns.md)
- [`references/review-rubric.md`](./references/review-rubric.md) -审查已完成效率工作时的负载