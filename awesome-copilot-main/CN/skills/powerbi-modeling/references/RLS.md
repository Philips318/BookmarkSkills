Power BI中的行级安全（RLS）

# #概述

行级安全性根据用户身份在行级别限制数据访问。用户只能看到他们被授权查看的数据。

设计原则

# # # 1。维度表过滤
将RLS应用于维度，而不是事实表。
-更高效（更小的表）
—过滤器通过关系传播
-更容易维护```dax
// On Customer dimension - filters propagate to Sales
[Region] = "West"
```
# # # 2。创建最小角色
避免多种角色组合：
—每个角色=单独的缓存
角色是相加的（并并，而不是交集）
-尽可能合并

# # # 3。尽可能使用动态RLS
数据驱动的规则可扩展性更好：
—表中的用户映射
- USERPRINCIPALNAME（）作为身份
—用户变更时，角色不发生变化

静态RLS vs动态RLS

静态RLS
固定每个角色的规则：```dax
// Role: West Region
[Region] = "West"

// Role: East Region  
[Region] = "East"
```
优点：简单明了
**缺点：**不可伸缩，需要每个组的角色

动态RLS
用户身份驱动过滤：```dax
// Single role filters based on logged-in user
[ManagerEmail] = USERPRINCIPALNAME()
```
**优点：**天平，自我维护
**缺点：**需要用户映射数据

##实现模式

模式1：直接用户映射
维度表中的用户邮箱：```dax
// On Customer table
[CustomerEmail] = USERPRINCIPALNAME()
```
模式2：安全表
将用户映射到数据的单独表：```
SecurityMapping table:
| UserEmail | Region |
|-----------|--------|
| joe@co.com | West  |
| sue@co.com | East  |
```

```dax
// On Region dimension
[Region] IN 
    SELECTCOLUMNS(
        FILTER(SecurityMapping, [UserEmail] = USERPRINCIPALNAME()),
        "Region", [Region]
    )
```
模式3：管理者层次结构
用户看到他们的数据和下属：```dax
// Using PATH functions for hierarchy
PATHCONTAINS(Employee[ManagerPath], 
    LOOKUPVALUE(Employee[EmployeeID], Employee[Email], USERPRINCIPALNAME()))
```
模式4：多规则
结合条件:```dax
// Users see their region OR if they're a global viewer
[Region] = LOOKUPVALUE(Users[Region], Users[Email], USERPRINCIPALNAME())
|| LOOKUPVALUE(Users[IsGlobal], Users[Email], USERPRINCIPALNAME()) = TRUE()
```
##通过MCP创建角色

###列出现有角色```
security_role_operations(operation: "List")
```
创建具有权限的角色```
security_role_operations(
  operation: "Create",
  definitions: [{
    name: "Regional Sales",
    modelPermission: "Read",
    description: "Restricts sales data by region"
  }]
)
```
添加表权限（Filter）```
security_role_operations(
  operation: "CreatePermissions",
  permissionDefinitions: [{
    roleName: "Regional Sales",
    tableName: "Customer",
    filterExpression: "[Region] = USERPRINCIPALNAME()"
  }]
)
```
###获取有效权限```
security_role_operations(
  operation: "GetEffectivePermissions",
  references: [{ name: "Regional Sales" }]
)
```
测试RLS

在Power BI Desktop中
1. 建模选项卡>视图
2. 选择要测试的角色
3. 可选地指定用户身份
4. 验证数据过滤

###测试意外值
对于动态RLS，测试：
-有效用户
未知用户（应该不会看到任何错误）
—NULL/blank值```dax
// Defensive pattern - returns no data for unknown users
IF(
    USERPRINCIPALNAME() IN VALUES(SecurityMapping[UserEmail]),
    [Region] IN SELECTCOLUMNS(...),
    FALSE()
)
```
常见错误

# # # 1。RLS仅适用于事实表
**问题：**大表扫描，性能差
解决方案：**应用于维度表，让关系传播

# # # 2。使用LOOKUPVALUE代替关系
**问题：**昂贵，不可扩展
解决方案：**创建适当的关系，让过滤器流动

# # # 3。期望交叉行为
**问题：**多个角色= UNION (additive)，而不是交集
解决方案：**设计角色时考虑到联合行为

# # # 4。忘记DirectQuery
**问题：** RLS过滤器变成WHERE子句
解决方案：**确保源数据库可以处理查询模式

# # # 5。不测试边缘情况
问题：**用户看到意外数据
解决方案：**测试使用：有效用户，无效用户，多角色

双向RLS

对于与RLS的双向关系：```
Enable "Apply security filter in both directions"
```
仅在下列情况下使用：
—RLS需要通过多对多进行过滤
-需要维度到维度的安全性

**注意：**每条路径只允许一个双向关系。

性能考虑

- RLS为每个查询添加WHERE子句
-过滤器中复杂的DAX会影响性能
-测试实际的用户数量
-考虑大型模型的聚合

对象级安全（OLS）

限制对整个表或列的访问：```
// Via XMLA/TMSL - not available in Desktop UI
```
用途:
-隐藏敏感列（工资，SSN）
-限制整个表
-与RLS结合，实现全面安全

验证检查表

[] RLS适用于维度表（非事实表）
-[]过滤器通过关系正确传播
—[]动态RLS使用USERPRINCIPALNAME（）
-[]使用有效和无效用户进行测试
[]边缘情况处理（NULL，未知用户）
-[]负载下性能测试
-[]角色映射记录
[]工作区角色被理解（管理员绕过RLS）