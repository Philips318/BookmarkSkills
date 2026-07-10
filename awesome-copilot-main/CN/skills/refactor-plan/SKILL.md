---
name: refactor-plan
description: 'Create a concrete plan before starting a multi-file refactor. Use when the user asks to plan, sequence, scope, or safely execute a refactor across multiple files; always investigate first, output the plan, and wait for confirmation before making code changes.'
---
#重构计划

在进行任何代码更改之前，创建一个详细的计划。

# #指令1. 在准备计划时不要编辑文件。
2. 搜索代码库以了解当前状态。阅读足够的实现、测试、配置和文档，以使计划特定于存储库。
3. 识别受影响的文件、所有权边界、依赖关系和可能隐藏的耦合。
4. 按照安全的顺序计划变更。首先选择契约和类型，然后是实现，然后是调用者，然后是测试，最后是清理。
5. 包括阶段之间的验证步骤和最终的验证命令。
6. 包括最危险阶段的回滚或恢复步骤。
7. 使用下面的格式输出完整的计划。
8. 在计划完成后停下来，并在实施前征求确认。如果用户已经要求你执行，仍然先制定计划并等待确认，除非他们明确表示在计划完成后不进行审查而继续执行。如果请求太模糊而无法安全计划，问简洁的澄清问题而不是编辑文件。

##输出格式```markdown
## Refactor Plan: [title]

### Current State
[Brief description of how things work now]

### Target State
[Brief description of how things will work after]

### Affected Files
| File | Change Type | Dependencies |
|------|-------------|--------------|
| path | modify/create/delete | blocks X, blocked by Y |

### Execution Plan

#### Phase 1: Types and Interfaces
- [ ] Step 1.1: [action] in `file.ts`
- [ ] Verify: [how to check it worked]

#### Phase 2: Implementation
- [ ] Step 2.1: [action] in `file.ts`
- [ ] Verify: [how to check]

#### Phase 3: Tests
- [ ] Step 3.1: Update tests in `file.test.ts`
- [ ] Verify: Run `npm test`

#### Phase 4: Cleanup
- [ ] Remove deprecated code
- [ ] Update documentation

### Rollback Plan
If something fails:
1. [Step to undo]
2. [Step to undo]

### Risks
- [Potential issue and mitigation]
```
计划完成后，问自己：“我要继续第一阶段吗？”