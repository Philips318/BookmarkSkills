---
name: scaffolding-oracle-to-postgres-migration-test-project
description: 'Scaffolds an xUnit integration test project for validating Oracle-to-PostgreSQL database migration behavior in .NET solutions. Creates the test project, transaction-rollback base class, and seed data manager. Use when setting up test infrastructure before writing migration integration tests, or when a test project is needed for Oracle-to-PostgreSQL validation.'
---
搭建一个oracle到postgresql迁移的集成测试项目

为单个目标项目创建一个带有事务管理和种子数据基础设施的可编译的空xUnit测试项目。在编写测试之前，每个项目运行一次。

# #工作流程```
Progress:
- [ ] Step 1: Inspect the target project
- [ ] Step 2: Create the xUnit test project
- [ ] Step 3: Implement transaction-rollback base class
- [ ] Step 4: Implement seed data manager
- [ ] Step 5: Verify the project compiles
```
**第一步：检查目标项目**

读取目标项目的`.csproj`以确定。. NET版本和现有包引用。完全匹配这些版本-不升级。

步骤2：创建xUnit测试项目

-目标相同。. NET版本作为被测应用程序。
—增加Oracle数据库连接和xUnit的NuGet包。
—仅向目标项目添加项目引用，不能添加其他应用项目。
—添加Oracle数据库连接配置的`appsettings.json`。

步骤3：实现事务回滚基类

-创建一个基测试类，在每次测试之前打开事务，并在测试之后回滚事务。
—捕获并处理所有异常，保证回滚。
-使模式可被所有下游测试类继承。

**步骤4：实现种子数据管理器**-创建一个全局种子管理器，用于在事务范围内加载测试数据。
—不提交种子数据—每次测试后回滚事务。
—不使用`TRUNCATE TABLE`—保留已有数据库数据。
-重用现有的种子文件，如果可用。
-建立种子文件位置的命名约定，下游测试创建将遵循。

**步骤5：验证项目编译**

构建测试项目，并在完成之前确认它的编译没有错误。

##键约束

- Oracle是golden behavior source - scaffold for Oracle first。
-保持现有。. NET和c#版本；不要引入较新的语言或运行时特性。
-输出是一个空的测试项目，只有基础设施-没有测试用例。