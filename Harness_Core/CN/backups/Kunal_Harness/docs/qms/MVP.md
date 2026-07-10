# 模块验证计划

## 目的

<!-- GUIDANCE: <本文档的目的是识别并记录模块验证或模块集成计划。这些计划包括定义、方法，以及按需要准备设备、材料和人员。> -->

## 范围

<!-- GUIDANCE: 本记录适用于 Philips、CT/AMI。 -->

## 技术管理信息

<!-- GUIDANCE: 日期 -->
<!-- GUIDANCE: 议程 -->
<!-- GUIDANCE: 出席情况 -->
<!-- GUIDANCE: [必须在此表中记录本次评审的所有受邀人员。评审可离线执行。可按需要增加行。] -->

| 评审者的能力/职能 | 出席情况 |
| --- | --- |

<!-- GUIDANCE: 评审结果 -->
<!-- GUIDANCE: [如有必要，列出未关闭问题或意见。如果计划评审期间没有意见，请写入“<Module verification/Module-Integration> Plan was reviewed and approved. There are no open items associated with this plan”] -->

## 内容

### 模块列表

<!-- GUIDANCE: 本节目的为： -->
<!-- GUIDANCE: 列出本计划范围内识别出的模块。 -->
<!-- GUIDANCE: 说明测试工作的原因（例如 NPI 或 sustaining effort）。 -->
<!-- GUIDANCE: 在适用时识别要验证的模块需求或规格文档。 -->
<!-- GUIDANCE: 指定要使用的方法（例如 by Test、by Inspection、by Analysis 等）。 -->
<!-- GUIDANCE: 注意：如果单个模块计划使用多种方法，请说明所有适用方法。 -->
<!-- GUIDANCE: 识别执行验证/集成活动所需人员的角色。 -->
<!-- GUIDANCE: 任何 Module Verification Plan 都必须包含 Module List Table。它不是可选项。 -->
<!-- GUIDANCE: Module List Table 用于识别并适用于要验证的模块、组件、单元或 Unit Under Test (UUT)。 -->
<!-- GUIDANCE: 注意：对于 SW Module verification，在 Software Module Decision Table 的“Is Formal Module verification Required?”列中标识为“Yes”的 Software Modules 应包含在 Module List Table 中。 -->
<!-- GUIDANCE: 可按需要添加或删除下表中的行。] -->
<!-- GUIDANCE: Module List Table: -->

| 模块名称 | 模块标识 | 验证类型 | 角色 | 备注 |
| --- | --- | --- | --- | --- |

<!-- GUIDANCE: [Methodology 可以是 Verification by Test、by Inspection 或 by Analysis。Software 的额外方法可包括 automated unit tests、Static Analysis 或 Dynamic analysis。 -->
<!-- GUIDANCE: 对于 module-integration，应描述要集成的模块或子系统以及测试活动。在第 4.5 - 4.6 节中列出主要集成步骤细节，以概述 module-integration plans。] -->
<!-- GUIDANCE: [注意：Software Module verification Decision Table 仅适用于 software modules。如果计划对象为非软件模块，应删除此表。对于 software modules 的 module verification，必须根据以下指导完成此表： -->
<!-- GUIDANCE: Module – 列出正在考虑的软件模块或软件项。 -->
<!-- GUIDANCE: Does the code “mitigate a safety related risk”? 用“Yes”或“No”表示此软件模块是否被认为属于系统安全相关风险缓解的一部分。缓解 safety related risk 的软件也被视为按 2003000438 (Risk Management Standard Operating Procedure) 或 2003000196 (PDLM Design Input SOP) 标记为 Class C Software Item。应在“Yes”或“No”后提供简短理由，或引用 Risk Management File 或 System/Sub-System Design Specifications。本列中的“Yes”表示 Code Review 和 Module Testing 都是必需的。 -->
<!-- GUIDANCE: Is “Code Review” Required? 用“Yes”或“No”表示。如果为“Yes”，请输入 Code Review document 的 part number。可在“Yes”或“No”后提供简短理由。如果“Does the Code mitigate a safety related risk?”行中为“Yes”，则 Code Review 为强制要求。 -->
<!-- GUIDANCE: Is formal “Module Testing” Required? 用“Yes”或“No”表示。如果为“Yes”，则 Module Test Report 会列入 Module List Table。可在“Yes”或“No”后提供简短理由。按 2003000438 (Risk Management Standard Operating Procedure) 或 2003000196 (PDLM Design Input SOP) 分类为 Class B 或 Class C 的 software modules (items) 需要 module test。 -->
<!-- GUIDANCE: Comments - 提供关于 software module testing 的任何特殊测试需求或其他备注。 -->
<!-- GUIDANCE: Feature / Functionality - 要测试的 feature / functionality 标题。注意，一个模块可能涉及多个 feature，因此可能在表中两个不同 feature 下列出（例如 module A）。此外，在某些情况下，可在某个 functionality 下测试多个模块（例如 Module A、Module B - 贡献于待测 feature / functionality 的模块列表）。] -->
<!-- GUIDANCE: [Software Module Testing Decision Table:] -->
<!-- GUIDANCE: [可按需要添加或删除下表中的行。] -->
<!-- GUIDANCE: [下表应针对非 Software modules 删除] -->

| 软件模块名称 | 代码是否“缓解安全相关风险”？ | 是否需要“Code Review”？ | 是否需要正式“Module Testing”？ | 备注 |
| --- | --- | --- | --- | --- |

### 测试设备和测试设置

<!-- GUIDANCE: [识别模块验证或模块集成所需的重要系统资产（例如工具、测量仪器和/或与测试设置相关的系统、子系统或额外部件）。在 Comments 部分中，在适用时标识预计时间范围。说明是否需要任何定制工具或设备。 -->
<!-- GUIDANCE: 可按需要添加或删除下表中的行。] -->

| 索引 | 测试设备名称 | 测试设备描述 [设置描述，例如系统、子系统或 SW tool 的 P/N 和 revision（如适用）。对于由 Equipment Lifecycle SOP 控制的 DVM 或 Oscilloscope 等常规测试工具，在此填写 N/A] | 备注 |
| --- | --- | --- | --- |

### 测试单元合理性说明

<!-- GUIDANCE: <必须考虑并论证所执行测试的测试单元数量和测试重复次数。测试单元是在对硬件或机械部件/模块进行抽样测试时使用的模块实例。应在适用时描述并论证所使用的统计方法。详情见 Statistical Techniques SOP。如果本计划不包含硬件或机械部件/模块的抽样测试，请在本节输入“N/A”。> -->

### 人员

<!-- GUIDANCE: [识别设置和执行测试所需人员。只有当特定个人必须执行测试时才使用个人姓名；否则，用职位表示所需技能，并在 Personnel Name or ID 列填写 N/A。在 Comments 列中，至少识别并列出需要 R&D 以外部门提供人员的技能和时间范围。如有 Project Management Plan，可引用该计划。] -->
<!-- GUIDANCE: [可按需要添加或删除下表中的行。] -->

| 索引 | 人员姓名或 ID | 人员职务 | 备注 |
| --- | --- | --- | --- |

### 要执行的模块验证

<!-- GUIDANCE: [对于 Module Integration Plans，指定“N/A”。简要描述将在所列模块上执行的模块验证活动。可按需要添加或删除下表中的行。] -->

| 模块名称 | 模块标识 | 所需子系统和/或附件 | 验证描述 |
| --- | --- | --- | --- |

### 要执行的模块集成测试

<!-- GUIDANCE: [对于 Module Verification Plans，指定“N/A”。简要描述将在模块上执行的 module-integration test。可按需要添加或删除下表中的行。] -->

| 模块组名称 | 模块料号（如适用） | Module Test Report 文档编号 | Module-Integration Test Procedure 作者 | 所需子系统和/或附件 | 描述 |
| --- | --- | --- | --- | --- | --- |

## 术语和缩写

| 术语 / 缩写 | 描述 |
| --- | --- |

## 附录

| 附录 | 标题 |
| --- | --- |

## 参考资料

### 外部参考

| 文档 ID | 文档标题 |
| --- | --- |

### 内部参考

| 文档 ID | 文档标题 |
| --- | --- |

## 变更记录摘要

| 版本 | 文档变更号 | 文档编辑者 | 变更描述 |
| --- | --- | --- | --- |

## 批准记录

<!-- GUIDANCE: [在此列出本文档的批准者。可按需要添加额外行。] -->
<!-- GUIDANCE: [签名和日期 - 对纸质记录，填写手写签名和日期。 -->
<!-- GUIDANCE: 对电子记录，填写“Refer to PLM tool”，签名和日期会作为 document change order 的一部分在 PLM tool 中捕获。] -->
<!-- GUIDANCE: 签名和日期会作为 document change order 的一部分在 PLM tool 中捕获。 -->

| 签名原因 | 职能 | 姓名 |
| --- | --- | --- |

<!-- GUIDANCE: 附录 <x> – <附录名称> -->
