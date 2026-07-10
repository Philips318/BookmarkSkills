---
name: efcore-d2-db-diagram
description: "Generate D2 database diagrams from Entity Framework Core models. USE FOR: EF Core database diagram, Entity Framework Core ERD, DbContext diagram, C# entity relationship diagram, PostgreSQL schema visualization, generate .d2 file from EF Core entities, Fluent API mapping diagram, migrations-based database diagram, table relationships, owned types, many-to-many join tables, indexes and constraints. DO NOT USE FOR: runtime debugging, database migration execution, schema deployment, SQL performance tuning, or draw.io diagrams."
---
# Core D2数据库图生成器

##何时使用

当用户希望从实体框架核心代码库生成数据库/ ERD图时，使用此技能。

典型的请求:

-从EF Core实体生成D2数据库图。
-可视化表，列，主键，外键和关系。
-分析`DbContext`，`DbSet<T>`,`IEntityTypeConfiguration<T>`， Fluent API和迁移。
-使用`d2`命令生成可渲染为SVG/PNG的`.d2`文件。
-记录ASP的数据库模型。. NET Core /。网络项目。

# #目标

创建一个可读的D2实体关系图，以反映实际的EF Core持久性模型，而不仅仅是原始的c#类形状。

图表必须优先考虑：1. 数据库表和关系。
2. 主键、外键、required/optional列。
3. 拥有的类型和值对象。
4. 多对多关系和连接表。
5. 索引、唯一约束和表名。
6. EF Core仅在显式映射不存在的情况下使用约定。

输出为`.d2`源代码。它可以通过`d2`CLI呈现为SVG或PNG。

# #工具

- **d2命令行**：将`.d2`文件渲染为SVG/PNG.——`d2 input.d2 output.svg`——`d2 --layout=elk input.d2 output.svg`—**d2 fmt**：格式化d2文件。
——`d2 fmt input.d2`—不需要MCP服务器。该技能将D2源代码生成为文本。

推荐工作流程1. 阅读EF Core项目结构。
2. 找到所有`DbContext`类。
3. 找到所有`DbSet<T>`声明。
4. 定位实体类、拥有的类型、枚举类型和值对象。
5. 阅读`OnModelCreating`和所有`IEntityTypeConfiguration<T>`类。
6. 在可用时读取迁移，以确认表名、连接表、索引和删除行为。
7. 在编写D2之前，构建一个规范化的数据库模型。
8. 在生成之前进行强制性的图表问卷调查。
9. 使用数据库模型生成`.d2`文件，而不是使用原始类嵌套。
10. 在交付之前用`d2 fmt`验证D2语法。
11. 尽可能用`d2 --layout=elk schema.d2 schema.svg`渲染。
12. 如果重新生成，请首先重新读取EF Core映射和迁移。

生成图表前必须回答的问题

对于每个新图表和每次再生，都要问这些问题，除非用户已经在同一个请求中回答了这些问题。1.`Which DbContext should be diagrammed? (auto-detect/all/specific name)`2.`Display columns? (all/key-only/none)`3.`Display column types? (Yes/No)`4.`Display nullable/required markers? (Yes/No)`5.`Display indexes and unique constraints? (Yes/No)`6.`Display enum values? (Yes/No)`7.`Display owned types? (inline/separate/hide)`8.`Display many-to-many join tables? (explicit/compact/hide)`9.`Display audit/technical tables? (Yes/No)`10.`Display migration-only tables not present as entities? (Yes/No)`11.`Which grouping mode? (bounded-context/schema/namespace/flat)`12.`Which layout engine? (elk/dagre/tala)`13.`Which output format? (d2/svg/png)`当用户要求快速生成时的默认值：

—DbContext:`auto-detect`—列号：`key-only`—列类型：`Yes`—可空标记：`Yes`—索引：`Yes`—枚举：`No`—所属类型：`inline`—连接表：`explicit`—Audit/technical表：`No`—迁移专用表：`Yes`—分组：`bounded-context`—布局：`elk`—输出：`d2`##参考文档

根据需要加载这些内容：

|参考|何时加载||---|---|
|`references/efcore-model-extraction.md`|读取DbContext， DbSet, Fluent API，配置和迁移的规则|
| D2语法和ERD图的可视化约定|
|`references/relationship-rules.md`|如何推断一对一、一对多、多对多和拥有的关系|
|`references/grouping-modes.md`|绑定上下文、模式、命名空间和平面分组的规则|
交付生成的图|之前的最终检查表

EF核心提取规则

###源优先级

当来源不一致时，使用以下优先顺序：

1. 最近应用的迁移/迁移快照。
2.`OnModelCreating`或`IEntityTypeConfiguration<T>`中的流畅API配置。
3. 数据注释。
4. EF核心约定。
5. 原始的c#类形状。

需要EF核心概念来检测

检测和表示：—`DbContext`和`DbSet<T>`。
-实体类名和实际表名来自`ToTable`。
-模式名称来自`ToTable("Table", "schema")`。
-`HasKey`，`[Key]`的主键，约定和迁移。
—组合键。
-`HasForeignKey`的外键，导航属性和迁移操作。
-删除明确的行为：`Cascade`，`Restrict`,`NoAction`,`SetNull`,`ClientSetNull`。
-Required/optional关系标记。
-拥有`OwnsOne`，`OwnsMany`和`[Owned]`的类型。
-来自`UsingEntity`的多对多关系和隐式EF Core连接表。
—来自`HasIndex`、`IsUnique`和迁移的索引。
-`HasAlternateKey`的备用密钥。
-在Fluent API中配置的阴影属性。
-影响持久化类型或可读性的值转换。
—Enum属性。
—忽略属性和实体。

图表渲染规则

# # #表

如果可能的话，使用`shape: sql_table`将每个持久化表表示为D2节点。使用以下内容约定：```d2
Clients: {
  shape: sql_table
  constraint: primary_key
  Id: uuid {constraint: primary_key}
  Name: text
  Status: enum
}
```
如果`sql_table`不可用或导致验证问题，则退回到具有结构化文本的矩形。

# # #的关系

使用从属表到主表之间的定向边。

标签必须包括关系基数和已知的FK名称：```d2
Offers.ClientId -> Clients.Id: "N:1 FK_Offers_Clients_ClientId"
```
使用这些基数标签：

——`1:1`——`1:N`——`N:1`——`N:N`——`owned`###拥有的类型

拥有的类型默认为内联呈现。

内联的例子:```d2
Clients: {
  shape: sql_table
  Id: uuid {constraint: primary_key}
  Address.Street: text
  Address.ZipCode: text
  Address.City: text
}
```
如果用户选择`separate`，则将拥有的类型表示为可视的从属表，并使用`owned`关系。

# # #多对多

默认为显式连接表，因为EF Core创建了真实的表。

对于隐式多对多关系，创建一个生成的连接表节点，并将其标记为`implicit join`。

技术表

默认情况下隐藏技术表，除非请求。

例子:

——`__EFMigrationsHistory`-挂火桌
- ASP。NET标识表
—审计日志
-发件箱表

如果隐藏了技术表，请在图表后面的摘要中提及它们。

##分组模式

—`bounded-context`：按检测到的域区域或folder/module.分组
—`schema`：按数据库模式分组，如`public`、`auth`、`billing`。
-`namespace`：按c#命名空间分组。
-`flat`：没有容器，所有表在同一级别。

##样式规则

使用一致的样式：-主实体表：实心边框。
-连接表：虚线边界。
-拥有的类型：更轻的笔画或嵌套的内联字段。
-技术表：静音风格。
—外部表或仅用于迁移的表：虚线。
-所需关系：实线。
—可选关系：虚线。
—级联删除：标签后缀`cascade`。

交付前的质量检查

在交付图纸之前，请确认：—[]已清除选中的DbContext。
-[]考虑所有`DbSet<T>`实体。
—[]读取Fluent API配置。
-[]当存在迁移时进行检查。
—[]表名和模式名匹配EF Core映射。
-[]已存在主键。
-[]表示外键和基数。
—[]所属类型根据用户选择处理。
-[]除非用户要求，否则多对多连接表是显式的。
-[]隐藏的技术表在最后的总结中列出。
—[]D2语法适用于`d2 fmt`。
-[]边缘端点在容器内时使用完整的点符号。
-[]图保持可读性，避免交叉重布局。

##输出格式

当用户要求安装技能时，提供以下文件夹结构：```text
.github/
  skills/
    efcore-d2-db-diagram/
      SKILL.md
      references/
        efcore-model-extraction.md
        d2-erd-style.md
        relationship-rules.md
        grouping-modes.md
        quality-gate.md
```
当用户要求生成图表时，提供：

1.`.d2`源文件内容。
2. 使用所选布局引擎的呈现命令。
3. 假设和隐藏表格的简明总结。