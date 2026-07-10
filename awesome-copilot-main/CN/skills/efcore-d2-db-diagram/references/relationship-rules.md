#关系规则

# #一对多

检测:

——`HasOne(...).WithMany(...)`-依赖实体上的FK属性。
-主体上的集合导航。

使依赖于主体：```d2
Orders.ClientId -> Clients.Id: "N:1"
```
# #一对一

检测:

——`HasOne(...).WithOne(...)`—唯一的FK索引。
—共享主键关系。

使依赖于主体：```d2
ClientProfiles.ClientId -> Clients.Id: "1:1"
```
# #多对多

检测:

——`UsingEntity`-没有显式连接实体的两个集合导航。
—迁移创建了两个fk和复合键的连接表。

默认情况下显式呈现连接表。

##所属类型

检测:

——`OwnsOne`——`OwnsMany`——`[Owned]`默认情况下为内联，除非检测到表拆分或单独的表映射。

##可选关系

关系在以下情况下是可选的：

- FK可为空。
—配置`IsRequired(false)`。
—迁移列为空。

虚线表示可选关系。