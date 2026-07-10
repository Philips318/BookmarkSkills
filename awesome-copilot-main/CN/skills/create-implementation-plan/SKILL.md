---
name: create-implementation-plan
description: 'Create a new implementation plan file for new features, refactoring existing code or upgrading packages, design, architecture or infrastructure.'
---
#创建实施计划

##主要指令

您的目标是为`${input:PlanPurpose}`创建一个新的实现计划文件。您的输出必须是机器可读的，确定性的，并且结构化，以便由其他AI系统或人类自主执行。

##执行环境

这个提示是为人工智能之间的通信和自动处理而设计的。所有指示必须按字面意思解释，系统地执行，不需要人工解释或澄清。

##核心要求

-生成可由AI代理或人类完全执行的实施计划
-使用零歧义的确定性语言
-为自动解析和执行构建所有内容
-确保完全自给自足，没有外部依赖

##规划结构要求计划必须由包含可执行任务的离散的原子阶段组成。除非显式声明，否则每个阶段必须由AI代理或人类独立处理，没有跨阶段依赖关系。

阶段架构

-每个阶段必须有可衡量的完成标准
-阶段内的任务必须并行执行，除非指定了依赖关系
—所有任务描述必须包括具体的文件路径、函数名和确切的实现细节
-任何任务都不需要人工解释或决策

##人工智能优化实施标准-使用明确、明确的语言，不需要任何解释
-将所有内容结构为机器可解析的格式（表，列表，结构化数据）
-包括具体的文件路径，行号和准确的代码引用
—明确定义所有变量、常量和配置值
-在每个任务描述中提供完整的上下文
-为所有标识符使用标准化前缀（REQ-， TASK-等）
—包含可自动验证的验证条件

##输出文件规格

—执行计划文件保存在“`/plan/`”目录下
—使用命名约定：`[purpose]-[component]-[version].md`—用途前缀：`upgrade|refactor|feature|data|infrastructure|process|architecture|design`—示例：`upgrade-system-command-4.md`、`feature-auth-module-1.md`-文件必须有效，正面内容结构合理

模板结构所有实施计划必须严格遵循以下模板。每个部分都是必需的，必须填充特定的、可操作的内容。AI代理必须在执行之前验证模板的遵从性。

模板验证规则-所有前内容字段必须存在并正确格式化
-所有节头必须完全匹配（区分大小写）
—所有标识符前缀必须遵循指定的格式
-表必须包含所有必需的列
-最终输出中不得保留占位符文本
- **标识符必须唯一声明。**每个标识符（`REQ-NNN`,`SEC-NNN`,`CON-NNN`,`GUD-NNN`,`PAT-NNN`,`GOAL-NNN`,`TASK-NNN`,`ALT-NNN`,`DEP-NNN`,`FILE-NNN`,`TEST-NNN`,`RISK-NNN`,`ASSUMPTION-NNN`）必须**声明一次**。声明是标识符引入一行的地方：TASK/GOAL表行的前导单元格，或者`- **REQ-001**: ...`等项目符号行的加粗前缀。然后，相同的标识符可以在计划的其他地方作为引用**出现任意次数(`TASK`主体引用`REQ`，`TASK`引用另一个`TASK`， Dependencies部分指向已经在上游声明的`DEP`，例如tc)。引用是必需的，而不是冲突。标识符唯一性检查

在完成计划之前，先进行这些检查。检查(1)和(2)的目标是声明，并且必须返回零行。检查(3)是一个广泛的信息扫描：它也会显示有效的参考，所以用它来意识而不是作为一个门。```bash
# Set PLAN_FILE to the plan being validated.
PLAN_FILE="/plan/<purpose>-<component>-<version>.md"

# 1) Duplicate TASK / GOAL declarations in table rows.
grep -oE '\| (TASK|GOAL)-[0-9]+ \|' "$PLAN_FILE" \
  | sed -E 's/.*((TASK|GOAL)-[0-9]+).*/\1/' \
  | sort | uniq -d

# 2) Duplicate declaration IDs in bullet-style spec lines.
grep -oE '^- \*\*(REQ|SEC|CON|GUD|RISK|ASSUMPTION|TASK|GOAL|FILE|TEST|PAT|ALT|DEP)-[0-9]+\*\*:' "$PLAN_FILE" \
  | sed -E 's/^- \*\*([A-Z]+-[0-9]+)\*\*:.*/\1/' \
  | sort | uniq -d

# 3) Broad duplicate scan (diagnostic only; may include valid references).
grep -oE '(REQ|SEC|CON|GUD|RISK|ASSUMPTION|TASK|GOAL|FILE|TEST|PAT|ALT|DEP)-[0-9]+' "$PLAN_FILE" \
  | sort | uniq -d
```
前提条件：具有`grep`、`sed`、`sort`和`uniq`的posix兼容shell （`sh`/`bash`）。在没有这些工具的Windows上，使用等效的平台原生命令并保留相同的声明vs引用逻辑。

如果check(1)或(2)返回任何行，则重新编号副本，以便每个标识符只声明一次，然后重新运行检查，直到两者都为空。

# #状态

执行计划的状态必须在前端事项中明确定义，并且必须反映计划的当前状态。状态可以是以下选项之一（括号中的status_color）：`Completed`（亮绿色标识）、`In progress`（黄色标识）、`Planned`（蓝色标识）、`Deprecated`（红色标识）、`On Hold`（橙色标识）。它还应该在介绍部分显示为一个徽章。```md
---
goal: [Concise Title Describing the Package Implementation Plan's Goal]
version: [Optional: e.g., 1.0, Date]
date_created: [YYYY-MM-DD]
last_updated: [Optional: YYYY-MM-DD]
owner: [Optional: Team/Individual responsible for this spec]
status: 'Completed'|'In progress'|'Planned'|'Deprecated'|'On Hold'
tags: [Optional: List of relevant tags or categories, e.g., `feature`, `upgrade`, `chore`, `architecture`, `migration`, `bug` etc]
---

# Introduction

![Status: <status>](https://img.shields.io/badge/status-<status>-<status_color>)

[A short concise introduction to the plan and the goal it is intended to achieve.]

## 1. Requirements & Constraints

[Explicitly list all requirements & constraints that affect the plan and constrain how it is implemented. Use bullet points or tables for clarity.]

- **REQ-001**: Requirement 1
- **SEC-001**: Security Requirement 1
- **[3 LETTERS]-001**: Other Requirement 1
- **CON-001**: Constraint 1
- **GUD-001**: Guideline 1
- **PAT-001**: Pattern to follow 1

## 2. Implementation Steps

### Implementation Phase 1

- GOAL-001: [Describe the goal of this phase, e.g., "Implement feature X", "Refactor module Y", etc.]

| Task | Description | Completed | Date |
|------|-------------|-----------|------|
| TASK-001 | Description of task 1 | ✅ | 2025-04-25 |
| TASK-002 | Description of task 2 | |  |
| TASK-003 | Description of task 3 | |  |

### Implementation Phase 2

- GOAL-002: [Describe the goal of this phase, e.g., "Implement feature X", "Refactor module Y", etc.]

| Task | Description | Completed | Date |
|------|-------------|-----------|------|
| TASK-004 | Description of task 4 | |  |
| TASK-005 | Description of task 5 | |  |
| TASK-006 | Description of task 6 | |  |

## 3. Alternatives

[A bullet point list of any alternative approaches that were considered and why they were not chosen. This helps to provide context and rationale for the chosen approach.]

- **ALT-001**: Alternative approach 1
- **ALT-002**: Alternative approach 2

## 4. Dependencies

[List any dependencies that need to be addressed, such as libraries, frameworks, or other components that the plan relies on.]

- **DEP-001**: Dependency 1
- **DEP-002**: Dependency 2

## 5. Files

[List the files that will be affected by the feature or refactoring task.]

- **FILE-001**: Description of file 1
- **FILE-002**: Description of file 2

## 6. Testing

[List the tests that need to be implemented to verify the feature or refactoring task.]

- **TEST-001**: Description of test 1
- **TEST-002**: Description of test 2

## 7. Risks & Assumptions

[List any risks or assumptions related to the implementation of the plan.]

- **RISK-001**: Risk 1
- **ASSUMPTION-001**: Assumption 1

## 8. Related Specifications / Further Reading

[Link to related spec 1]
[Link to relevant external documentation]
```
