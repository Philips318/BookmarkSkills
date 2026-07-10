核心模型提取

要检查的文件

按以下顺序检查：

1.`DbContext`类。
2.`DbSet<T>`声明。
3.`OnModelCreating`。
4.`IEntityTypeConfiguration<T>`类。
5. 实体类。
6. 迁移和模型快照。
7. 数据注释。

映射优先级

当来源冲突时，使用：

1. 最新的迁移/模型快照。
2. 流利的API。
3. 数据注释。
4. EF核心约定。
5. c#的形状。

重要的EF核心api

寻找:

——`ToTable`——`HasKey`——`HasAlternateKey`——`HasIndex`——`IsUnique`——`Property`——`HasColumnName`——`HasColumnType`——`IsRequired`——`HasMaxLength`——`HasConversion`——`HasOne`——`WithMany`——`WithOne`——`HasForeignKey`——`OnDelete`——`OwnsOne`——`OwnsMany`——`UsingEntity`——`Ignore`# #迁移

使用迁移来检测：

-实际表名。
-连接表。
-阴影FK列。
——索引。
—组合键。
—删除行为。
—仅支持迁移的表。