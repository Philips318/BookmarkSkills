---
name: fabric-lakehouse
description: 'Use this skill to get context about Fabric Lakehouse and its features for software systems and AI-powered functions. It offers descriptions of Lakehouse data components, organization with schemas and shortcuts, access control, and code examples. This skill supports users in designing, building, and optimizing Lakehouse solutions using best practices.'
metadata:
  author: tedvilutis
  version: "1.0"
---
#何时使用此技能

在需要时使用此技能：
-生成文档或解释，包括Fabric Lakehouse及其功能的定义和上下文。
-使用最佳实践设计、构建和优化Lakehouse解决方案。
-了解Microsoft Fabric中Lakehouse的核心概念和组件。
-学习如何在Lakehouse中管理表格和非表格数据。

#织物湖屋

##核心概念

什么是湖屋？

Microsoft Fabric中的Lakehouse是一个项目，它为用户提供了一个存储表格数据（如表）和非表格数据（如文件）的地方。它结合了数据湖的灵活性和数据仓库的管理能力。它提供了:—**在OneLake中统一存储结构化和非结构化数据
**Delta Lake格式**用于ACID事务、版本控制和时间旅行
-用于T-SQL查询的SQL分析端点
- Power BI集成的语义模型
-支持其他表格格式，如CSV， Parquet
-支持任何文件格式
-表优化和数据管理工具

关键组件

- **Delta Tables**：具有ACID遵从和模式强制的管理表
—**Files**：“文件”区域中的Unstructured/semi-structured数据
—**SQL Endpoint**：自动生成只读SQL查询接口
—**快捷方式**：不复制external/internal数据的虚拟链接
- **Fabric Materialized Views**：预计算表，快速查询性能

Lakehouse中的表格数据表格形式的表格数据存储在“tables”文件夹下。在Lakehouse中，表的主要格式是Delta。Lakehouse可以存储其他格式的表格数据，如CSV或Parquet，这些格式只能用于Spark查询。
表可以是内部的，当数据存储在“Tables”文件夹下时，也可以是外部的，当只有对表的引用存储在“Tables”文件夹下，但数据本身存储在引用位置时。表可以通过快捷键引用，快捷键可以是内部的（指向Fabric中的另一个位置），也可以是外部的（指向存储在Fabric之外的数据）。

Lakehouse中表的模式在创建lakehouse时，用户可以选择启用模式。模式用于组织Lakehouse表。模式被实现为“Tables”文件夹下的文件夹，并在这些文件夹中存储表。默认模式是“dbo”，它不能被删除或重命名。所有其他模式都是可选的，可以创建、重命名或删除。用户可以使用模式快捷方式引用位于另一个lakehouse中的模式，从而用一个快捷方式引用目标模式中的所有表。

文件在一个湖屋

文件存储在“Files”文件夹下。用户可以创建文件夹和子文件夹来组织他们的文件。任何文件格式都可以存储在Lakehouse中。

### Fabric Materialized Views根据计划自动更新的一组预先计算的表。它们为复杂的聚合和连接提供快速查询性能。物化视图是使用PySpark或Spark SQL定义的，并存储在相关的Notebook中。

### Spark视图

由SQL查询定义的逻辑表。它们不存储数据，而是提供一个用于查询的虚拟层。视图是使用Spark SQL定义的，并存储在湖边的表旁边。

# #安全

项目访问或控制平面安全性

用户可以拥有工作空间角色（Admin、Member、Contributor、Viewer），这些角色提供对Lakehouse及其内容的不同级别的访问。用户也可以通过Lakehouse的共享功能获得访问权限。

数据访问或OneLake安全对于数据访问，使用OneLake安全模型，该模型基于Microsoft Entra ID（以前的Azure Active Directory）和基于角色的访问控制（RBAC）。湖屋数据存储在OneLake中，因此通过OneLake权限控制对数据的访问。除了对象级权限，lakhouse还支持表的列级和行级安全性，允许对谁可以查看表中的特定列或行进行细粒度控制。


##湖屋捷径

快捷键创建数据的虚拟链接而不复制：

快捷方式的类型

- **内部**：链接到其他FabricLakehouses/tables，跨工作区数据共享
- **ADLS Gen2**：链接到Azure中的ADLS Gen2容器
—**Amazon S3**: AWS S3桶，跨云数据访问
- **Dataverse**: Microsoft Dataverse，商业应用数据
- **谷歌云存储**:GCS桶，跨云数据访问

性能优化V-Order优化

为了使用语义模型更快地读取数据，在Delta表上启用V-Order优化。它以一种提高常见访问模式查询性能的方式呈现数据。

表优化

还可以使用OPTIMIZE命令对表进行优化，该命令可以将小文件压缩成大文件，还可以应用z排序来提高特定列上的查询性能。定期优化有助于在数据被摄取并随时间更新时保持性能。Vacuum命令可用于清理旧文件并释放存储空间，特别是在更新和删除之后。

# #血统lakhouse项目支持沿袭，它允许用户跟踪数据的来源和转换。沿袭信息会自动为Lakehouse中的表和文件捕获，显示数据如何从源流向目的地。这有助于调试、审计和理解数据依赖关系。

## PySpark代码示例

详细信息请参见[PySpark code]（references/pyspark.md）。

将数据输入Lakehouse

详细信息请参见[获取数据]（references/getdata.md）。