---
description: 'Specification-Driven Workflow v1 provides a structured approach to software development, ensuring that requirements are clearly defined, designs are meticulously planned, and implementations are thoroughly documented and validated.'
applyTo: '**'
---
# Spec驱动工作流v1

* * Specification-Driven工作流:* *
在需求和实现之间架起桥梁。

**始终维护这些工件：**

- **`requirements.md`**：结构化ear符号的用户故事和验收标准。
- **`design.md`**：技术架构，序列图，实现注意事项。
- **`tasks.md`**：详细、可跟踪的实施计划。

通用文档框架

* *文档规则:* *
使用详细的模板作为所有文档的主要事实来源。

* *摘要格式:* *
仅用于简洁的工件，如变更日志和拉取请求描述。

详细的文档模板

####动作文档模板（AllSteps/Executions/Tests）```bash
### [TYPE] - [ACTION] - [TIMESTAMP]
**Objective**: [Goal being accomplished]
**Context**: [Current state, requirements, and reference to prior steps]
**Decision**: [Approach chosen and rationale, referencing the Decision Record if applicable]
**Execution**: [Steps taken with parameters and commands used. For code, include file paths.]
**Output**: [Complete and unabridged results, logs, command outputs, and metrics]
**Validation**: [Success verification method and results. If failed, include a remediation plan.]
**Next**: [Automatic continuation plan to the next specific action]
```
####决策记录模板（所有决策）```bash
### Decision - [TIMESTAMP]
**Decision**: [What was decided]
**Context**: [Situation requiring decision and data driving it]
**Options**: [Alternatives evaluated with brief pros and cons]
**Rationale**: [Why the selected option is superior, with trade-offs explicitly stated]
**Impact**: [Anticipated consequences for implementation, maintainability, and performance]
**Review**: [Conditions or schedule for reassessing this decision]
```
摘要格式（用于报告）

####精简操作日志

用于生成简洁的变更日志。每个日志条目都来源于一个完整的Action Document。`[TYPE][TIMESTAMP] Goal: [X] → Action: [Y] → Result: [Z] → Next: [W]`####压缩决策记录

用于拉取请求摘要或执行摘要。`Decision: [X] | Rationale: [Y] | Impact: [Z] | Review: [Date]`执行流程（6阶段循环）

**不要跳过任何步骤。使用一致的术语。减少了不确定性。* *

阶段1：分析

* *目的:* *

-理解问题所在。
—分析现有系统。
-生成一组清晰的、可测试的需求。
-思考可能的解决方案及其含义。

* *清单:* *

-阅读所有提供的代码、文档、测试和日志。      - Document file inventory, summaries, and initial analysis results.
-[]在**EARS符号中定义要求**：      - Transform feature requests into structured, testable requirements.
      - Format: `WHEN [a condition or event], THE SYSTEM SHALL [expected behavior]`
-[]识别依赖和约束。      - Document a dependency graph with risks and mitigation strategies.
-[]映射数据流和交互。      - Document system interaction diagrams and data models.
-[]分类边缘情况和故障。      - Document a comprehensive edge case matrix and potential failure points.
-[]评估信心。      - Generate a **Confidence Score (0-100%)** based on clarity of requirements, complexity, and problem scope.
      - Document the score and its rationale.
* *关键约束:* *

- **在所有要求都明确并记录下来之前，不要进行任何操作

第二阶段：设计

* *目的:* *

-制定全面的技术设计和详细的实施计划。

* *清单:* *

-[] **根据置信度评分定义自适应执行策略：**
- **高置信度(>85%)**    - Draft a comprehensive, step-by-step implementation plan.
    - Skip proof-of-concept steps.
    - Proceed with full, automated implementation.
    - Maintain standard comprehensive documentation.
- **中等置信度(66-85%)**    - Prioritize a **Proof-of-Concept (PoC)** or **Minimum Viable Product (MVP)**.
    - Define clear success criteria for PoC/MVP.
    - Build and validate PoC/MVP first, then expand plan incrementally.
    - Document PoC/MVP goals, execution, and validation results.
- **低置信度(<66%)**    - Dedicate first phase to research and knowledge-building.
    - Use semantic search and analyze similar implementations.
    - Synthesize findings into a research document.
    - Re-run ANALYZE phase after research.
    - Escalate only if confidence remains low.
-[] **文档技术设计在`design.md`:**
- **架构：**组件和交互的高级概述。
—**数据流：**图表和说明。
**接口：** API契约，模式，面向公众的函数签名。
- **数据模型：**数据结构和数据库模式。

-[] **文档错误处理：**
-建立一个包含程序和预期反应的错误矩阵。

-[] **定义单元测试策略

-[] **在“`tasks.md`:**”中创建实施计划
-对于每个任务，包括描述、预期结果和依赖关系。

* *关键约束:* *

- **在设计和计划完成并得到验证之前，不要进行实施

阶段3：实现**

* *目的:* *

根据设计和计划编写生产质量代码。

* *清单:* *

-[]以小的、可测试的增量编写代码。      - Document each increment with code changes, results, and test links.
-[]从依赖项向上实现。      - Document resolution order, justification, and verification.
-[]遵循惯例。      - Document adherence and any deviations with a Decision Record.
-[]添加有意义的注释。      - Focus on intent ("why"), not mechanics ("what").
-[]按规划创建文件。      - Document file creation log.
-[]实时更新任务状态。

* *关键约束:* *

- **不要合并或部署代码，直到所有的实现步骤都被记录和测试

### **阶段4：验证

* *目的:* *

-验证执行符合所有要求和质量标准。

* *清单:* *

-[]执行自动化测试。      - Document outputs, logs, and coverage reports.
      - For failures, document root cause analysis and remediation.
-[]如有需要，可手动验证。      - Document procedures, checklists, and results.
-[]测试边缘情况和错误。      - Document results and evidence of correct error handling.
-[]性能验证。      - Document metrics and profile critical sections.
-[]日志执行轨迹。      - Document path analysis and runtime behavior.
* *关键约束:* *

- **在所有验证步骤完成并解决所有问题之前不要继续

阶段5：反思

* *目的:* *

改进代码库，更新文档，分析性能。

* *清单:* *

-[]为了可维护性而重构。      - Document decisions, before/after comparisons, and impact.
-[]更新所有项目文档。      - Ensure all READMEs, diagrams, and comments are current.
-[]确定可能的改进。      - Document backlog with prioritization.
-[]验证成功标准。      - Document final verification matrix.
-[]进行meta分析。      - Reflect on efficiency, tool usage, and protocol adherence.
-[]自动创建技术债务问题      - Document inventory and remediation plans.
* *关键约束:* *

- **在所有的文件和改进行动被记录下来之前，不要关闭该阶段

阶段6：切换

* *目的:* *

打包工作以供审查和部署，并过渡到下一个任务。

* *清单:* *

-[]生成执行摘要。      - Use **Compressed Decision Record** format.
-[]准备拉取请求（如适用）：    1. Executive summary.
    2. Changelog from **Streamlined Action Log**.
    3. Links to validation artifacts and Decision Records.
    4. Links to final `requirements.md`, `design.md`, and `tasks.md`.
-[]完成工作区。      - Archive intermediate files, logs, and temporary artifacts to `.agent_work/`.
-[]继续执行下一个任务。      - Document transition or completion.
* *关键约束:* *

- **在所有移交步骤完成并记录下来之前，不要认为任务已经完成

##故障排除和重试协议

**如果遇到错误、歧义或阻塞：**

* *清单:* *

1. * * Re-analyze * *:
-重新访问ANALYZE阶段。
—确认所有的要求和约束都是清晰完整的。
2. 重新设计* * * *:
-重新审视设计阶段。
-根据需要更新技术设计、计划或依赖关系。
3. * *重新计划* *:
-调整`tasks.md`中的实施方案，以应对新发现。
4. * *重试执行* *:
-用正确的参数或逻辑重新执行失败的步骤。
5. * * * *升级:
—重试后问题仍然存在，请按照升级协议处理。

* *关键约束:* *

- **永远不要处理未解决的错误或歧义。始终记录故障排除步骤和结果技术债务管理（自动化）

识别和文档

- **代码质量**：使用静态分析在实现过程中持续评估代码质量。
**快捷方式**：在决策记录中明确记录所有速度超过质量的决策及其后果。
- **工作区**：监控组织漂移和命名不一致。
- **文档**：跟踪不完整、过时或缺失的文档。

自动发布创建模板```text
**Title**: [Technical Debt] - [Brief Description]
**Priority**: [High/Medium/Low based on business impact and remediation cost]
**Location**: [File paths and line numbers]
**Reason**: [Why the debt was incurred, linking to a Decision Record if available]
**Impact**: [Current and future consequences (e.g., slows development, increases bug risk)]
**Remediation**: [Specific, actionable resolution steps]
**Effort**: [Estimate for resolution (e.g., T-shirt size: S, M, L)]
```
修复（自动优先级）

-基于风险的优先级排序和依赖分析。
-努力评估，以帮助未来的计划。
为大型重构工作提出迁移策略。

质量保证（自动化）

###持续监控

- **静态分析**：检查代码风格、质量、安全漏洞和架构规则遵守情况。
—**动态分析**：监控暂存环境中的运行时行为和性能。
- **文档**：自动检查文档的完整性和准确性（如链接、格式）。

质量指标（自动跟踪）

-代码覆盖率百分比和差距分析。
-每function/method.的圈复杂度评分
-可维护性指标评估。
-技术负债比率（例如，估计补救时间与开发时间）。
-文档覆盖率百分比（例如，带有注释的公共方法）。耳朵符号参考

**EARS (Easy Approach to Requirements Syntax)** -需求的标准格式：

- **无处不在**:`THE SYSTEM SHALL [expected behavior]`—**事件驱动**:`WHEN [trigger event] THE SYSTEM SHALL [expected behavior]`—**状态驱动**:`WHILE [in specific state] THE SYSTEM SHALL [expected behavior]`- **不需要的行为**:`IF [unwanted condition] THEN THE SYSTEM SHALL [required response]`—**可选**:`WHERE [feature is included] THE SYSTEM SHALL [expected behavior]`- **Complex**：上述模式的组合，以满足复杂的需求

每项要求必须是：

- **可测试**：可通过自动或手动测试进行验证
- **明确**：可能的单一解释
- **必要**：有助于系统的目的
- **可行**：可以在限制范围内实现
-可追溯性：与用户需求和设计元素相关联