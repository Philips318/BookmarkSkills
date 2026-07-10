---
name: migrating-oracle-to-postgres-stored-procedures
description: 'Migrates Oracle PL/SQL stored procedures to PostgreSQL PL/pgSQL. Translates Oracle-specific syntax, preserves method signatures and type-anchored parameters, leverages orafce where appropriate, and applies COLLATE "C" for Oracle-compatible text sorting. Use when converting Oracle stored procedures or functions to PostgreSQL equivalents during a database migration.'
---
#将Oracle存储过程迁移到PostgreSQL

将OraclePL/SQL存储过程和函数转换为PostgreSQLPL/pgSQL。

# #工作流程```
Progress:
- [ ] Step 1: Read the Oracle source procedure
- [ ] Step 2: Translate to PostgreSQL PL/pgSQL
- [ ] Step 3: Write the migrated procedure to Postgres output directory
```
**步骤1：读取Oracle源程序**

从`.github/oracle-to-postgres-migration/DDL/Oracle/Procedures and Functions/`读取Oracle存储过程。有关类型解析，请参阅Oracletable/view的`.github/oracle-to-postgres-migration/DDL/Oracle/Tables and Views/`定义。

**第二步：转换为PostgreSQLPL/pgSQL**

应用以下翻译规则：-翻译所有oracle特定的语法到PostgreSQL等效。
-保留原有功能和控制流逻辑。
-保持类型锚定的输入参数（例如，`PARAM_NAME IN table_name.column_name%TYPE`）。
-对传递给其他过程的输出参数使用显式类型（`NUMERIC`、`VARCHAR`、`INTEGER`） -不要使用类型锚定。
—不修改方法签名。
-除非Oracle源代码中已经存在，否则不要在对象名称前加上模式名称。
—保持异常处理和回滚逻辑不变。
—请勿生成`COMMENT`或`GRANT`语句。
-通过文本字段排序时使用`COLLATE "C"`进行oracle兼容排序。
-利用`orafce`扩展，当它提高清晰度或保真度。

在`.github/oracle-to-postgres-migration/DDL/Postgres/Tables and Views/`上查阅PostgreSQLtable/view定义，了解目标模式的详细信息。

步骤3：将迁移后的过程写入Postgres输出目录**将每个迁移的过程放在`.github/oracle-to-postgres-migration/DDL/Postgres/Procedures and Functions/{PACKAGE_NAME_IF_APPLICABLE}/`下各自的文件中。每个文件一个过程。