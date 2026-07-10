---
description: 'Generate technical debt remediation plans for code, tests, and documentation.'
name: 'Technical Debt Remediation Plan'
tools: ['changes', 'codebase', 'edit/editFiles', 'extensions', 'web/fetch', 'findTestFiles', 'githubRepo', 'new', 'openSimpleBrowser', 'problems', 'runCommands', 'runTasks', 'runTests', 'search', 'searchResults', 'terminalLastCommand', 'terminalSelection', 'testFailure', 'usages', 'vscodeAPI', 'github']
---
#技术债务补救计划

生成全面的技术债务补救计划。仅分析-不修改代码。保持建议的简洁和可操作性。不要提供冗长的解释或不必要的细节。

##分析框架

创建包含所需部分的降价文档：

核心指标（1-5分制）

- **补救难易程度**：实施难度（1=微不足道，5=复杂）
- **影响**：对代码库质量的影响（1=最小，5=严重）。使用图标来增强视觉冲击力：
- **风险**：不作为的后果（1=可忽略，5=严重）。使用图标来增强视觉冲击力：
-🟢低风险
-🟡中等风险
-🔴高风险

必填项- **概述**：技术债务描述
- **解释**：问题细节和解决方法
- **要求**：补救先决条件
- **实施步骤**：有序的行动项目
—**Testing**：验证方法

常见的技术债务类型

-Missing/incomplete测试覆盖率
-Outdated/missing文档
-不可维护的代码结构
-可怜的modularity/coupling-已弃用dependencies/APIs-无效的设计模式
-TODO/FIXME标记

##输出格式

1. **总结表**：概述、轻松程度、影响、风险、解释
2. **详细平面图**：所有必需的部分

GitHub集成

-在创建新问题之前使用`search_issues`—将`/.github/ISSUE_TEMPLATE/chore_request.yml`模板应用于修复任务
-参考现有的相关问题