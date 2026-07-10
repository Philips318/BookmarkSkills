---
name: 'audit-integrity'
description: 'Shared audit integrity framework for all AppSec agents — enforces output quality, intellectual honesty, and continuous improvement through anti-rationalization guards, self-critique loops, retry protocols, non-negotiable behaviors, self-reflection quality gates (1-10 scoring, ≥8 threshold), and a self-learning system with lesson/memory governance for security analysis agents.'
compatibility: 'Cross-platform. Works with any language or framework analyzed by AppSec agents.'
metadata:
  version: '1.0'
---
#审计完整性技能

在所有AppSec代理中强制执行输出质量、知识诚实和持续改进。

##何时使用

-运行每个安全分析、代码审查、威胁模型或质量扫描代理
-自动应用于分析后质量门
—适用于任何执行SAST、SCA、威胁建模或代码质量分析的座席

# #组件

这个技能提供了7个可重用的能力。代理应用所有7个组件，除非它们的作用域不包括特定组件。

|组件|参考文件|用途| . | . ||-----------|---------------|---------|
|澄清协议| [clarification-protocol.md](references/clarification-protocol.md) |当范围不明确时，在分析前提出≤2个有针对性的问题|
| Anti-Rationalization Guard | [anti-rationalization-guard.md](references/anti-rationalization-guard.md) |带有强制响应的禁止合理化表|
|自我批评循环| [self-critique-loop.md](references/self-critique-loop.md) |初始分析后的强制性第二轮审查|
|重试协议| [retry-protocol.md](references/retry-protocol.md) |工具失败处理-重试一次，然后记录|
硬原则：绝不捏造，总是引用证据，报告差距|
|自我反思质量门| [self-reflection-quality-gate.md](references/self-reflection-quality-gate.md) | 1-10个评分标准，每个类别|有≥8个阈值
|自学系统| [self-learning-system.md](references/self-learning-system.md) |Lesson/Memory模板及治理规则|

##执行流程1. **分析前**：如果范围不明确，请使用澄清协议
2. **在分析期间**：在每个决策点应用反合理化保护
3. **初始通过后**：执行自我批评循环（强制性第二次通过）
4. **工具失败**：应用重试协议
5. **出厂前**：运行自我反思质量门（所有类别必须得分≥8）
6. **交付后**：创建Lessons/Memories的新发现，误报，或方法差距（见自学系统）

特定于代理的适应

每个代理自定义**自我批评循环**清单和**自我反思质量门**类别以匹配其领域。参考文件提供基本模板；代理用特定于领域的项扩展它们。每个代理类型的扩展示例
- **SAST/SCA代理**：添加污染跟踪完整性和清单覆盖检查
- ** sonarqube风格代理**：添加评级完整性检查（A-E与发现的一致性）
- **威胁建模代理**：每个信任边界添加跨步类别完整性
- **代码审查代理**：添加信任边界审计与数据流跟踪