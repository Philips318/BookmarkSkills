---
name: creating-oracle-to-postgres-migration-integration-tests
description: 'Creates integration test cases for .NET data access artifacts during Oracle-to-PostgreSQL database migrations. Generates DB-agnostic xUnit tests with deterministic seed data that validate behavior consistency across both database systems. Use when creating integration tests for a migrated project, generating test coverage for data access layers, or writing Oracle-to-PostgreSQL migration validation tests.'
---
为oracle到postgresql的迁移创建集成测试

为单个目标项目中的数据访问工件生成集成测试用例。测试验证在Oracle或PostgreSQL上运行时的行为一致性。

# #先决条件

-测试项目必须已经存在并已编译（单独搭建）。
-在编写测试之前，阅读现有的基测试类和种子管理器约定。

# #工作流程```
Test Creation:
- [ ] Step 1: Discover the test project conventions
- [ ] Step 2: Identify testable data access artifacts
- [ ] Step 3: Create seed data
- [ ] Step 4: Write test cases
- [ ] Step 5: Review determinism
```
步骤1：发现测试项目约定

阅读基本测试类、种子管理器和项目文件，以理解继承模式、事务管理和种子文件约定。

**步骤2：识别可测试的数据访问工件**

范围仅限于目标项目。列出与数据库（存储库、dao、存储过程调用程序、查询构建程序）交互的数据访问方法。

**步骤3：创建种子数据**

-遵循现有项目的种子文件位置和命名约定。
-尽可能重用现有的种子文件。
—避免`TRUNCATE TABLE`—保持现有数据库数据完整。
-不要提交种子数据；测试在回滚的事务中运行。
-确保种子数据不与其他测试冲突。
-在断言依赖种子数据之前加载并验证种子数据。

步骤4：编写测试用例-继承基测试类以获得自动事务create/rollback.-断言逻辑输出（行、列、计数、错误类型），而不是特定于平台的消息。
-断言特定的期望值-当一个具体的值从种子数据中可用时，永远不要断言一个值仅仅是非空或非空。
避免测试不存在的代码路径或断言不可能发生的行为。
避免针对相同方法的测试中出现冗余断言。

第五步：回顾决定论

重新检查针对非空值的每个断言。确认每个种子数据都是确定的。修复任何依赖于测试控制之外的数据库状态的断言。

##键约束- **Oracle是黄金源** -测试捕获Oracle的预期行为。
**断言中没有特定于平台的错误消息或语法。
- **种子只针对Oracle** -测试项目将迁移到PostgreSQL稍后。
- **范围限于一个项目** -不要为目标项目之外的工件创建测试。