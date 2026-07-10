#数据库和数据管理插件

数据库管理、SQL优化和数据管理工具，适用于PostgreSQL、SQL Server和一般数据库开发的最佳实践。

# #安装```bash
# Using Copilot CLI
copilot plugin install database-data-management@awesome-copilot
```
包含的内容

###命令（斜杠命令）

|命令|描述||---------|-------------|
|`/database-data-management:sql-optimization`|通用SQL性能优化助手，用于跨所有SQL数据库（MySQL, PostgreSQL, SQL Server, Oracle）的全面查询调优，索引策略和数据库性能分析。提供执行计划分析、分页优化、批处理操作和性能监控指导。|
|`/database-data-management:sql-code-review`|通用SQL代码审查助手，执行所有SQL数据库（MySQL, PostgreSQL, SQL Server, Oracle）的全面安全性，可维护性和代码质量分析。重点介绍SQL注入预防、访问控制、代码标准和反模式检测。为完整的开发覆盖补充SQL优化提示。|
|`/database-data-management:postgresql-optimization`| PostgreSQL特定的开发助手，专注于独特的PostgreSQL功能，高级数据类型和PostgreSQL独有的功能。涵盖了JSONB操作，数组类型，自定义类型，range/geometric类型、全文搜索、窗口函数和PostgreSQL扩展生态系统。|
PostgreSQL特定代码审查助手，专注于PostgreSQL最佳实践，反模式和独特的质量标准。涵盖JSONB操作、数组使用、自定义类型、模式设计、函数优化和postgresql独有的安全特性，如行级安全（RLS）。|# # #代理

|代理|描述||-------|-------------|
|`postgresql-dba`|使用PostgreSQL扩展与PostgreSQL数据库一起工作。|
|`ms-sql-dba`|使用MS SQL扩展与Microsoft SQL Server数据库一起工作。|

# #源

这个插件是[Awesome Copilot]（https://github.com/github/awesome-copilot）的一部分，这是一个社区驱动的GitHub Copilot扩展集合。

# #许可证

麻省理工学院