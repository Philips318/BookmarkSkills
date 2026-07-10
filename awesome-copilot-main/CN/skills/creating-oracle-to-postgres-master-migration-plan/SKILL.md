---
name: creating-oracle-to-postgres-master-migration-plan
description: 'Discovers all projects in a .NET solution, classifies each for Oracle-to-PostgreSQL migration eligibility, and produces a persistent master migration plan. Use when starting a multi-project Oracle-to-PostgreSQL migration, creating a migration inventory, or assessing which .NET projects contain Oracle dependencies.'
---
创建Oracle-to-PostgreSQL主迁移计划

解析a。. NET解决方案，对每个项目进行Oracle→PostgreSQL迁移资格分类，并编写一个结构化的计划，下游代理和技能可以解析。

# #工作流程```
Progress:
- [ ] Step 1: Discover projects in the solution
- [ ] Step 2: Classify each project
- [ ] Step 3: Confirm with user
- [ ] Step 4: Write the plan file
```
第一步：发现项目

在工作空间根目录中找到解决方案文件（它具有`.sln`或`.slnx`扩展名）（询问用户是否存在多个）。解析它以提取所有`.csproj`项目引用。对于每个项目，请注意名称、路径和类型（类库、web API、控制台、测试等）。

**第二步：对每个项目进行分类**

扫描每个非测试项目的Oracle指标：

- NuGet参考：`Oracle.ManagedDataAccess`，`Oracle.EntityFrameworkCore`（检查`.csproj`和`packages.config`）
—配置项：“`appsettings.json`”、“`web.config`”、“`app.config`”中的Oracle连接字符串
—代码用法：`OracleConnection`、`OracleCommand`、`OracleDataReader`- DDL在`.github/oracle-to-postgres-migration/DDL/Oracle/`下的交叉引用（如果存在）

每个项目分配一个分类：

|分类|含义||---|---|
| **MIGRATE** |有需要转换的Oracle交互|
| **SKIP** |无Oracle指标（仅ui、共享实用程序等）|
| ** already_migrate ** |存在`-postgres`或`.Postgres`副本，并显示已处理|
| **TEST_PROJECT** |测试项目由测试工作流|处理

**步骤3：与用户**确认

呈现分类列表。让用户在完成迁移之前调整分类或迁移顺序。

**步骤4：编写计划文件**

保存到：`.github/oracle-to-postgres-migration/Reports/Master Migration Plan.md`使用这个模板——下游消费者依赖于这个结构：````markdown
# Master Migration Plan

**Solution:** {solution file name}
**Solution Root:** {REPOSITORY_ROOT}
**Created:** {timestamp}
**Last Updated:** {timestamp}

## Solution Summary

| Metric | Count |
|--------|-------|
| Total projects in solution | {n} |
| Projects requiring migration | {n} |
| Projects already migrated | {n} |
| Projects skipped (no Oracle usage) | {n} |
| Test projects (handled separately) | {n} |

## Project Inventory

| # | Project Name | Path | Classification | Notes |
|---|---|---|---|---|
| 1 | {name} | {relative path} | MIGRATE | {notes} |
| 2 | {name} | {relative path} | SKIP | No Oracle dependencies |

## Migration Order

1. **{ProjectName}** — {rationale, e.g., "Core data access library; other projects depend on it."}
2. **{ProjectName}** — {rationale}
````
对项目进行排序，以便在迁移shared/foundational库之前迁移它们的依赖项。