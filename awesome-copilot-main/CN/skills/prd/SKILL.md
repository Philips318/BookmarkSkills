---
name: prd
description: 'Generate high-quality Product Requirements Documents (PRDs) for software systems and AI-powered features. Includes executive summaries, user stories, technical specifications, and risk analysis.'
license: MIT
---
#产品需求文件（PRD）

# #概述

设计全面的、生产级的产品需求文档（prd），在业务愿景和技术执行之间架起桥梁。这项技能适用于现代软件系统，确保需求被清楚地定义。

##何时使用

在以下情况下使用此技能：

-开始新产品或功能开发周期
-将模糊的想法转化为具体的技术规范
-定义ai功能的需求
-利益相关者需要一个统一的项目范围“真相来源”
-用户要求“编写产品开发计划”、“文档需求”或“计划功能”

---

##操作流程

阶段1：发现（采访）

在编写PRD的单行之前，你**必须**询问用户以填补知识空白。不要假设语境。

* *问:* ***核心问题**：为什么我们现在要做这个？
- **成功指标**：我们如何知道它是有效的？
- **限制**：预算，技术堆栈，还是截止日期？

阶段2：分析和范围界定

综合用户的输入。识别依赖关系和隐藏的复杂性。

—绘制**用户流程**。
-定义**非目标**以保护时间线。

阶段3：技术起草

使用下面的**Strict PRD Schema**生成文档。

---

珠三角质量标准

质量要求

使用具体的、可测量的标准。避免“快速”、“简单”或“直观”。```diff
# Vague (BAD)
- The search should be fast and return relevant results.
- The UI must look modern and be easy to use.

# Concrete (GOOD)
+ The search must return results within 200ms for a 10k record dataset.
+ The search algorithm must achieve >= 85% Precision@10 in benchmark evals.
+ The UI must follow the 'Vercel/Next.js' design system and achieve 100% Lighthouse Accessibility score.
```
---

严格的PRD模式

你**必须**遵循这个精确的输出结构：

# # # 1。执行概要

- **问题陈述**:1-2句话阐述痛点。
- **建议解决方案**:1-2句关于修复。
- **成功标准**:3-5个可测量的kpi。

# # # 2。用户体验与功能

- **用户角色**：这是为谁？
—**用户故事**:`As a [user], I want to [action] so that [benefit].`- **验收标准**：每个故事的“完成”定义的项目符号列表。
- **非目标**：我们没有构建什么？

# # # 3。AI系统要求（如适用）

- **工具要求**：需要哪些工具和api ？
- **评价策略**：如何衡量输出的质量和准确性。

# # # 4。技术规格

- **架构概述**：数据流和组件交互。
- **集成点**:api、db和Auth。
- **安全与隐私**：数据处理和合规。

# # # 5。风险与路线图- **分阶段推出**:MVP -> v1.1 -> v2.0。
—**技术风险**：延迟、成本或依赖失败。

---

##实施指南

###做（总是）

- **定义测试**：对于AI系统，指定如何测试和验证输出质量。
- **迭代**：提出一个草案，并征求对特定部分的反馈。

不要（避免）

- **跳过发现**：在写PRD之前，一定要先问至少两个澄清性问题。
- **Hallucinate Constraints**：如果用户没有指定一个技术栈，询问或标记为`TBD`。

---

示例：智能搜索系统

# # # 1。执行概要

**问题**：用户很难在海量存储库中找到特定的文档片段。
**解决方案**：一个智能搜索系统，提供直接的答案与来源引用。
* *成功* *:

-减少50%的搜索时间。
-引文准确性>= 95%。

# # # 2。用户故事- **故事**：作为一名开发者，我想问自然语言问题，这样我就不用猜测关键词了。
- * *交流* *:
-支持多回合澄清。
-返回带有“复制”按钮的代码块。

# # # 3。AI系统架构

- **所需工具**:`codesearch`、`grep`、`webfetch`。

# # # 4。评价

- **基准**：测试50个常见的开发人员问题。
- **通过率**:90%必须符合预期的引用。