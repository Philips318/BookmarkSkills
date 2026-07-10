---
name: github-codespaces-efficiency
description: 'Audit and improve GitHub Codespaces efficiency. Use this skill when a user wants faster Codespaces startup, lower Codespaces spend, slim devcontainers, right-size machines, tune idle timeout, or scope prebuilds to branches with sustained usage.'
---
# GitHub代码空间效率

使用此技能作为GitHub代码空间效率工作的精益入口点。检查repo，识别浪费，只加载需要的引用。

如果还不存在`.devcontainer/`，在继续执行下面的步骤之前，加载[`references/codespaces.md`]（./references/codespaces.md）并定义一个基线。

使用此技能时

-用户想要更快的代码空间启动或更低的代码空间花费。
- repo有一个`.devcontainer/`或明确的代码空间配置问题。
—用户询问devcontainer优化、机器大小、预构建策略或空闲超时指导。
-用户是第一次设置代码空间或需要帮助从头开始创建新的`.devcontainer/`。

##只加载你需要的

- [`references/codespaces.md`](./references/codespaces.md) - devcontainer，机器大小，预构建，空闲超时指导和报告。
- [`references/review-rubric.md`](./references/review-rubric.md) -只加载审查通过。

核心工作流程

# # # 1。第一次测量```bash
find .devcontainer -maxdepth 2 -type f
gh codespace list
repo=$(gh repo view --json nameWithOwner --jq .nameWithOwner)
gh api "/repos/$repo/codespaces/machines"
```
如果`gh`验证失败或用户缺乏repo管理范围，则继续对`.devcontainer/`文件进行静态分析；将机器类型和预构建建议标记为未经验证。

寻找：devcontainer映像>2 GB或超过10个特性，机器类型大于使用数据支持，缺少`devcontainer-lock.json`（建议添加-许多repos预编译锁定文件支持），预构建范围太广，空闲超时与使用模式不匹配。

# # # 2。应用护栏

在推荐之前，请根据这些规则检查每个提议的修复：1. 不删除团队每天使用的工具-删除任何删除所需开发工具或扩展的修复。
2. 不认为越小越好-平衡机器成本与开发人员的体验和吞吐量。
3. 不会把devcontainer变成生产镜像——删除任何添加了仅用于生产的依赖的修复，除非团队明确要求。
4. 首选增量更改—只有当不存在`.devcontainer/`时，绿地基线才合适；标记（不删除）重构现有配置的更改。
5. Repo更改与组织设置分开——将任何将可编辑文件与组织级或用户级代码空间设置混合在一起的修复拆分为两个不同的建议。

# # # 3。选择前3个修复从下面的六个候选中，只保留那些有步骤1 *和*的审计证据支持的，并通过步骤2的所有护栏。按估计每月节省的成本（美元）对幸存者进行排名。选择所有符合这两个标准的候选人，最多3人。1. 修剪devcontainer——删除日常开发工作不需要的特性、包或扩展；目标映像< 2gb且少于10个特征
2. 大小合适的机器类型-与观察到的使用模式相匹配；如果没有数据，明确地说明假设
3. 范围预构建-启用默认分支，`release/*`分支在过去14天内活动，以及每周有超过5个代码空间的分支；其他所有禁用
4. 调整空闲超时-默认30分钟；如果大多数课程在30分钟前结束，则为15分钟；如果大部分时间较长，则为60分钟
5. 删除未使用的扩展或端口转发规则
6. 减少devcontainer映像大小并改进层缓存

# # # 4。验证-启动测试代码空间，以确认devcontainer更改构建并按预期启动。
-当遥测可用时，根据观察到的使用情况验证机器尺寸；否则标记为未经验证。
-将意外构建或启动失败视为真正的bug，即使配置看起来是正确的。

##所需输出

**浪费来源：**[最高成本或启动时间驱动因素]

**建议修复：**[由审计证据和通过护栏支持的前3项更改]

**验证：**[已证实的活动/仅静态/剩余风险]

* *影响:* *
-启动时间：[预期]/[可用时测量]
-每月支出：[预期]/[可测量的]
-资源利用率：[预期]/[可用时测量]

# #引用

——[`references/codespaces.md`] (./references/codespaces.md)
- [`references/review-rubric.md`](./references/review-rubric.md) -审查已完成效率工作时的负载