---
applyTo: '**/.copilot-tracking/changes/*.md'
description: 'Instructions for implementing task plans with progressive tracking and change record - Brought to you by microsoft/edge-ai'
---
#任务计划实施说明

您将实现位于`.copilot-tracking/plans/**`和`.copilot-tracking/details/**`中的特定任务计划。您的目标是逐步地、完全地实现计划文件中的每一步，以创建满足所有指定需求的高质量、可工作的软件。

实现进度必须在位于`.copilot-tracking/changes/**`的相应更改文件中进行跟踪。

##核心实施过程

# # # 1。计划分析及准备**必须在开始实施前完成：**
- **必选**：阅读并充分理解完整的计划文件，包括范围、目标、所有阶段和每个检查表项
- **必选**：完全阅读并充分理解相应的更改文件-如果上下文缺少任何部分，请使用`read_file`重新读取整个文件
- **必选**：识别计划中提到的所有引用文件，并检查它们的上下文
- **必选**：了解当前的项目结构和惯例

# # # 2。系统实施流程

**系统地执行计划中的各项任务：**1. **按顺序处理任务** -严格按照计划顺序执行，一次执行一项任务
2. **在执行任何任务之前必须执行：**
**始终确保执行与计划中的特定任务相关联**
- **总是从`.copilot-tracking/details/**`**中的相关详细信息标记文件中读取该任务的整个详细信息部分
- **在进行**之前充分了解所有实施细节
-根据需要收集任何其他所需的上下文

3. **用工作代码完成任务：**
-遵循工作区中现有的代码模式和约定
-创建满足细节中指定的所有任务要求的工作功能
-包括适当的错误处理，文档，并遵循最佳实践4. **标记任务完成和更新更改跟踪：**
—更新计划文件：将已完成任务的“`[ ]`”修改为“`[x]`”
- **完成每个任务后必须执行**：通过在相应的add， Modified或Removed部分添加相关文件路径和实现内容的一句话总结来更新更改文件
- **强制性**：如果任何更改与任务计划和细节不符，请在相关部分明确指出更改是在计划之外进行的，并包括具体原因
—如果一个阶段中的所有任务都是complete`[x]`，则将该阶段标头标记为complete`[x]`# # # 3。实施质量标准**每个实现必须：**
-遵循现有的工作空间模式和惯例（检查`copilot/`文件夹中的标准）
-实现完整的工作功能，满足所有任务要求
-包括适当的错误处理和验证
-使用工作区中一致的命名约定和代码结构
-为复杂逻辑添加必要的文档和注释
-确保与现有系统和依赖关系的兼容性

# # # 4。持续进展和验证**每个任务完成后：**
1. 根据详细信息文件中的任务需求验证所做的更改
2. 在移动到下一个任务之前解决任何问题
3. **必选**：更新计划文件，标记已完成的任务`[x]`4. **每次任务完成后必须执行**：通过添加，修改或删除相关文件路径和实现内容的一句话总结来更新更改文件
5. 继续执行下一个未检查的任务

* *持续到:* *
—计划中所有任务都标有“完成`[x]`”
-所有指定的文件已创建或更新工作代码
-已验证计划的所有成功标准

# # # 5。参考资料收集指引**收集外部参考时：**
-注重实际的实现示例，而不是理论文档
-确认外部资源包含实际可用的模式
-调整外部模式以符合工作空间的惯例和标准

**实现from引用时
-首先遵循工作空间模式和惯例，其次才是外部模式
-实现完整的、可工作的功能，而不仅仅是示例
-确保所有依赖项和配置都正确集成
-确保在现有项目结构下实施工作

# # # 6。竣工和文档

**实现完成时：**
—所有规划任务均标有“完成`[x]`”
—所有指定的文件都存在并有工作代码
—验证计划中的所有成功标准
—没有实现错误**最后一步-更新更改文件与发布摘要：**
-仅在所有阶段标记完成后添加发布摘要部分`[x]`编制完整的文件清单和发布文件的总体实施总结

# # # 7。解决问题

**遇到执行问题时：**
—明确记录具体问题
-尝试其他方法或搜索词
当外部引用失败时，使用工作空间模式作为回退
-继续使用可用的信息，而不是完全停止
-在计划文件中注明任何未解决的问题，以备将来参考

##实现流程```
1. Read and fully understand plan file and all checklists completely
2. Read and fully understand changes file completely (re-read entire file if missing context)
3. For each unchecked task:
   a. Read entire details section for that task from details markdown file
   b. Fully understand all implementation requirements
   c. Implement task with working code following workspace patterns
   d. Validate implementation meets task requirements
   e. Mark task complete [x] in plan file
   f. Update changes file with Added, Modified, or Removed entries
   g. Call out any divergences from plan/details within relevant sections with specific reasons
4. Repeat until all tasks complete
5. Only after ALL phases are complete [x]: Add final Release Summary to changes file
```
##成功标准

实现完成时：
—✅所有的规划任务都标有“完成`[x]`”
-✅所有指定的文件都包含工作代码
-✅代码遵循工作区模式和约定
-✅所有功能在项目中按预期工作
-✅每次任务完成后，都会更新“添加”、“修改”或“删除”条目
-✅更改文件记录了所有阶段的详细发布准备文档和最终发布摘要

## Template更改文件

使用以下模板作为跟踪发布的实现进度的更改文件。
用合适的值替换`{{ }}`。在`./.copilot-tracking/changes/`中创建这个文件，文件名为：`YYYYMMDD-task-description-changes.md`**重要**：更新此文件后，每个任务完成追加到添加，修改，或删除部分。
**必选**：始终在更改文件的顶部包含以下内容：`<!-- markdownlint-disable-file -->`<!-- <changes-template> -->
```markdown
<!-- markdownlint-disable-file -->
# Release Changes: {{task name}}

**Related Plan**: {{plan-file-name}}
**Implementation Date**: {{YYYY-MM-DD}}

## Summary

{{Brief description of the overall changes made for this release}}

## Changes

### Added

- {{relative-file-path}} - {{one sentence summary of what was implemented}}

### Modified

- {{relative-file-path}} - {{one sentence summary of what was changed}}

### Removed

- {{relative-file-path}} - {{one sentence summary of what was removed}}

## Release Summary

**Total Files Affected**: {{number}}

### Files Created ({{count}})

- {{file-path}} - {{purpose}}

### Files Modified ({{count}})

- {{file-path}} - {{changes-made}}

### Files Removed ({{count}})

- {{file-path}} - {{reason}}

### Dependencies & Infrastructure

- **New Dependencies**: {{list-of-new-dependencies}}
- **Updated Dependencies**: {{list-of-updated-dependencies}}
- **Infrastructure Changes**: {{infrastructure-updates}}
- **Configuration Updates**: {{configuration-changes}}

### Deployment Notes

{{Any specific deployment considerations or steps}}
```
<!-- </changes-template> -->
