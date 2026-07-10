---
description: 'ColdFusion cfm files and application patterns'
applyTo: "**/*.cfm"
---
# ColdFusion编码标准

-使用CFScript在可能的地方更干净的语法。
—避免使用不推荐的标签和函数。
—遵循一致的变量和组件命名约定。
—使用“`cfqueryparam`”防止SQL注入。
使用##转义<cfoutput>块内的CSS散列符号
-当在<cfoutput>块内使用html时，通过使用双哈希（##）转义哈希符号（#），以防止意外的变量插值。
-如果你在一个html目标文件中，那么确保上面一行是：<cfsetting showDebugOutput = "false">

#其他最佳实践-使用`Application.cfc`进行应用程序设置和请求处理。
-将代码组织成可重用的cfc（组件），以提高可维护性。
-验证和清理所有用户输入。
-使用`cftry`/`cfcatch`进行错误处理和日志记录。
—避免在源文件中硬编码凭证或敏感数据。
-使用一致的缩进（2个空格，根据全球标准）。
—注释复杂的逻辑和文档函数，说明目的和参数。
-共享模板首选`cfinclude`，但避免循环包含。

-尽可能使用三元操作符
-确保标签对齐一致。