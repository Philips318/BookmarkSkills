---
name: creating-oracle-to-postgres-migration-bug-report
description: 'Creates structured bug reports for defects found during Oracle-to-PostgreSQL migration. Use when documenting behavioral differences between Oracle and PostgreSQL as actionable bug reports with severity, root cause, and remediation steps.'
---
为oracle到postgresql的迁移创建Bug报告

##何时使用

-记录由Oracle和PostgreSQL之间的行为差异引起的缺陷
为oracle到postgresql的迁移项目编写或审核bug报告

Bug报告格式

使用[references/BUG-REPORT-TEMPLATE.md]（references/BUG-REPORT-TEMPLATE.md）中的模板。每份报告必须包括：—**状态**：✅RESOLVED、⛔UNRESOLVED、⏳IN PROGRESS
—**组件**：受影响的端点、存储库或存储过程
—**Test**：相关的自动化测试名称
—**严重程度**：低/中/高/紧急-根据影响范围
- **问题**：预期的Oracle行为与观察到的PostgreSQL行为
—**场景**：种子数据、操作、预期结果、实际结果的有序复制步骤
- **根本原因**：导致缺陷的具体Oracle/PostgreSQL行为差异
**解决方案**：所做的或需要的更改，带有显式的文件路径
- **Validation**：在两个数据库上确认修复的步骤

Oracle-to-PostgreSQL指南- **Oracle是真相的来源** -从Oracle基线框架期望的行为
-显式地调用数据层的细微差别：空字符串与NULL、类型强制严格性、排序、序列值、时区、填充、约束
-应避免客户代码更改，除非需要正确的行为；当提出建议时，要清楚地记录和证明它们

##写作风格

简洁的语言，简短的句子，明确的下一步动作
-现在时或过去时一致
-步骤和验证的项目符号和编号列表
-最少的SQL摘录和日志作为证据；省略敏感数据并保持片段的可重复性
-坚持现有的runtime/language版本避免投机性修复

##文件名约定

将bug报告保存为`BUG_REPORT_<DescriptiveSlug>.md`，其中`<DescriptiveSlug>`是一个简短的PascalCase标识符（例如，`EmptyStringNullHandling`,`RefCursorUnwrapFailure`）。