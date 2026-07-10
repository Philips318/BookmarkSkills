---
name: planning-oracle-to-postgres-migration-integration-testing
description: 'Creates an integration testing plan for .NET data access artifacts during Oracle-to-PostgreSQL database migrations. Analyzes a single project to identify repositories, DAOs, and service layers that interact with the database, then produces a structured testing plan. Use when planning integration test coverage for a migrated project, identifying which data access methods need tests, or preparing for Oracle-to-PostgreSQL migration validation.'
---
规划oracle到postgresql迁移的集成测试

分析单个目标项目以确定需要集成测试的数据访问工件，然后生成结构化的、可操作的测试计划。

# #工作流程```
Progress:
- [ ] Step 1: Identify data access artifacts
- [ ] Step 2: Classify testing priorities
- [ ] Step 3: Write the testing plan
```
**步骤1：识别数据访问工件**

范围仅限于目标项目。查找直接与数据库交互的类和方法——存储库、dao、存储过程调用者、执行CRUD操作的服务层。

**步骤2：划分测试优先级**

根据迁移风险对工件进行排序。优先考虑使用oracle特定功能（refcursors,`TO_CHAR`，隐式类型强制转换，`NO_DATA_FOUND`）的方法，而不是简单的CRUD。

**第三步：编写测试计划**

写一个降价计划，包括：
-带有方法签名的可测试工件列表
-每个工件推荐的测试用例
-种子数据要求
-已知Oracle→PostgreSQL行为差异验证

# #输出

将计划写到：`.github/oracle-to-postgres-migration/Reports/{TARGET_PROJECT} Integration Testing Plan.md`##键约束- **单个项目范围** -仅为目标项目中的工件计划测试。
- **仅与数据库交互** -跳过不涉及数据库的业务逻辑。
- **Oracle是黄金源** -测试应该捕获Oracle的预期行为与PostgreSQL进行比较。
**迁移的应用程序被复制和重命名（例如，`MyApp.Postgres`），因此每个实例针对一个数据库。