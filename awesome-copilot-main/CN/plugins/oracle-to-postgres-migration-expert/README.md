# Oracle-to-PostgreSQL迁移专家插件

专家代理oracle到postgresql应用程序的迁移。网络解决方案。执行代码编辑、运行命令和调用扩展工具，将.NET/Oracle数据访问模式迁移到PostgreSQL。

# #安装```bash
# Using Copilot CLI
copilot plugin install oracle-to-postgres-migration-expert@awesome-copilot
```
包含的内容

# # #代理

|代理|描述||-------|-------------|
|`Oracle-to-PostgreSQL Migration Expert`|专家代理Oracle→PostgreSQL迁移。直接进行代码编辑和运行命令，教育用户迁移概念和陷阱，并在用户确认后调用扩展工具。|

# # #技能

|技能|描述||-------|-------------|
|`reviewing-oracle-to-postgres-migration`|通过对已知行为差异（空字符串、refcursors、类型强制转换、排序、时间戳、并发事务等）的交叉引用代码来识别oracle到postgresql的迁移风险。|
|`creating-oracle-to-postgres-master-migration-plan`|发现a中的所有项目。. NET解决方案，对每个oracle到postgresql的迁移资格进行分类，并生成持久的主迁移计划。|
|`migrating-oracle-to-postgres-stored-procedures`|将OraclePL/SQL存储过程迁移到PostgreSQLPL/pgSQL.翻译Oracle特定的语法，保留方法签名和类型锚定参数，并应用`COLLATE "C"`进行Oracle兼容的文本排序。|
|`planning-oracle-to-postgres-migration-integration-testing`|为。. NET数据访问构件，识别需要验证覆盖的存储库、dao和服务层。|
用事务回滚基类和种子数据管理器构建xUnit集成测试项目或者oracle到postgresql的迁移验证。|
|`creating-oracle-to-postgres-migration-integration-tests`|生成与数据库无关的xUnit集成测试，该测试使用确定性种子数据验证跨两个数据库系统的行为一致性。|
|`creating-oracle-to-postgres-migration-bug-report`|为在oracle到postgresql迁移验证期间发现的缺陷创建结构化的错误报告，包括严重性、根本原因和修复步骤。|# #特性

教育指导

专家代理在整个迁移过程中指导用户：

- **迁移概念**：解释Oracle→PostgreSQL的差异（空字符串vs NULL， NO_DATA_FOUND异常，排序顺序，TO_CHAR转换，类型强制转换严格，REF CURSOR处理，并发事务，timestamp/timezone行为）
- **陷阱参考**：从迁移知识中获得见解，以便用户理解为什么需要更改
—**最佳实践**：建议尽量减少更改，保留逻辑，并确保模式不变性
- **工作流指南**：提供了一个四阶段的迁移工作流程作为指南，用户可以按照自己的节奏进行

建议-行动模式

专家建议可操作的后续步骤，并只在用户确认后进行：1. **教育**关于迁移主题及其重要性
2. **建议**行动和预期结果
3. **确认**用户想要继续
4. **Act** -直接编辑、运行命令或调用扩展工具
5. **总结**所产生的内容并建议下一步

没有自动链接-用户控制速度和顺序。

##迁移流程

专家通过四个阶段的工作流程指导用户：

**第一阶段-发现和规划**

1. 创建主迁移计划（对解决方案中的所有项目进行分类）
2. 设置Oracle和PostgreSQL DDL构件

**阶段2 -代码迁移** *（每个项目）*
3. 迁移应用程序代码库（通过`ms-ossdata.vscode-pgsql`扩展）
4. 迁移存储过程（OraclePL/SQL→PostgreSQLPL/pgSQL）**第三阶段-验证** *（每个项目）*
5. 计划集成测试
6. 支撑xUnit测试项目
7. 创建集成测试
8. 对Oracle（基线）和PostgreSQL（目标）运行测试
9. 验证测试结果
10. 为任何失败创建错误报告

**第四阶段-报告**
11. 生成最终迁移报告（通过`ms-ossdata.vscode-pgsql`扩展）

# #先决条件

Visual Studio代码与GitHub Copilot- PostgreSQL扩展(`ms-ossdata.vscode-pgsql`) -需要应用程序代码迁移和报告生成
-。. NET解决方案与Oracle依赖关系的迁移

目录结构

代理期望并在您的存储库中创建以下结构：```
.github/
└── oracle-to-postgres-migration/
    ├── Reports/
    │   ├── Master Migration Plan.md
    │   ├── {Project} Integration Testing Plan.md
    │   ├── {Project} Application Migration Report.md
    │   ├── BUG_REPORT_*.md
    │   └── TestResults/
    └── DDL/
        ├── Oracle/      # Oracle DDL scripts (pre-migration)
        └── Postgres/    # PostgreSQL DDL scripts (post-migration)
```
# #使用

1. **寻求指导**：向专家提出迁移问题或情况(例如，“我应该如何迁移我的。. NET解决方案的PostgreSQL？“*或*”什么是Oracle做与PostgreSQL不同的空字符串？“*)
2. **学习和计划**：专家解释概念，表面陷阱的见解，并提出建议的工作流程步骤
3. **选择下一步**：决定要处理哪个任务（总体计划、代码迁移、测试等）
4. **确认和行动**：告诉专家继续，它直接进行编辑，运行命令或调用扩展工具
5. **Review & Continue**：检查结果并要求下一步

# #源

这个插件是[Awesome Copilot]（https://github.com/github/awesome-copilot）的一部分，这是一个社区驱动的GitHub Copilot扩展集合。

# #许可证

麻省理工学院