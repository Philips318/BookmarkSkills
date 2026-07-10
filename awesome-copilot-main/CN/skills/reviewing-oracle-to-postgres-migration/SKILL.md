---
name: reviewing-oracle-to-postgres-migration
description: 'Identifies Oracle-to-PostgreSQL migration risks by cross-referencing code against known behavioral differences (empty strings, refcursors, type coercion, sorting, timestamps, concurrent transactions, etc.). Use when planning a database migration, reviewing migration artifacts, or validating that integration tests cover Oracle/PostgreSQL differences.'
---
# oracle到postgresql数据库迁移

显示迁移风险，并根据`references/`文件夹中记录的已知Oracle/PostgreSQL行为差异验证迁移工作。

##何时使用

1. **规划** -在过程、触发器、查询或refcursor客户端上开始迁移工作之前。确定哪些参考见解适用，以便提前解决风险。
2. **验证** -在迁移工作完成后，确认每个适用的洞察都被解决了，并且集成测试涵盖了新的PostgreSQL语义。

# #工作流程

确定任务类型：

**计划迁移？**遵循风险评估工作流程。
**验证已完成的工作？**遵循验证工作流程。

风险评估工作流程（计划）```
Risk Assessment:
- [ ] Step 1: Identify the migration scope
- [ ] Step 2: Screen each insight for applicability
- [ ] Step 3: Document risks and recommended actions
```
**步骤1：确定迁移范围**

列出受影响的数据库对象（过程、触发器、查询、视图）和调用它们的应用程序代码。

**步骤2：筛选每个见解的适用性**

查看[references/REFERENCE.md]（references/REFERENCE.md）中的引用索引。对于每个条目，确定迁移范围是否包含受该洞察影响的模式。只有当洞察可能相关时，才阅读完整的参考文件。

**步骤3：记录风险和建议的行动**

对于每个适用的洞察，请注意参考文件中的特定风险和推荐的修复模式。标记任何需要设计决策的洞察（例如，是否保留Oracle的空字符串为null语义或采用PostgreSQL的行为）。

验证工作流（迁移后）```
Validation:
- [ ] Step 1: Map the migration artifact
- [ ] Step 2: Cross-check applicable insights
- [ ] Step 3: Verify integration test coverage
- [ ] Step 4: Gate the result
```
步骤1：映射迁移工件

识别迁移的对象并总结更改集。

**步骤2：交叉检查适用的见解**

对于[references/REFERENCE.md]（references/REFERENCE.md）中的每个引用，确认在迁移工作中确认并处理了行为或测试需求。

**步骤3：验证集成测试覆盖率**

确认测试同时执行在可应用的见解（异常、排序、refcursor消耗、并发事务、时间戳等）中突出显示的正常路径和失败场景。

**步骤4：检查结果**

返回一个检查表，声明每个可应用的洞察都被处理了，迁移脚本运行了，集成测试通过了。