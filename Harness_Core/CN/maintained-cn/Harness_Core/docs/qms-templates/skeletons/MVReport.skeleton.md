# 模块验证报告

## 目的

<!-- GUIDANCE: <描述本报告的目的。> -->
<!-- GUIDANCE: [示例：本文档总结 <Project Name> Module Test 或 Integration Plan 的执行结果。] -->

## 范围

<!-- GUIDANCE: <描述作为本报告一部分进行测试的模块列表。对于 Module-Integration，列出被测试的模块；对于 module verification，列出被测试的单个模块。> -->
<!-- GUIDANCE: 本记录适用于 Philips、CT/AMI。 -->

## 技术管理信息

<!-- GUIDANCE: 日期 -->
<!-- GUIDANCE: 议程 -->
<!-- GUIDANCE: 出席情况 -->
<!-- GUIDANCE: [必须在此表中记录本次评审的所有受邀人员。评审可离线执行。可按需要增加行。] -->

| 评审者的能力/职能 | 出席情况 |
| --- | --- |

<!-- GUIDANCE: 评审结果 -->
<!-- GUIDANCE: [如有必要，列出未关闭问题或意见。如果没有意见，请写入“<Module verification/Module-Integration> Report was reviewed and approved. There are no open items associated with this report.”] -->

## 内容

<!-- GUIDANCE: [本节应记录已完成测试的执行情况和结果。如果测试执行在本文档外部完成，则应删除本节，并用对外部文档或附录的适当引用替换。可按需要添加任意数量的 major test sections 或 test sub sections。Major Test 和 Sub Test section numbering 应与 Module Test 或 module Integration Procedure 对齐。] -->
<!-- GUIDANCE: repeatable example section: <Major Test Section 1> -->
<!-- GUIDANCE: [Major Test Section 会包含模块或模块组的主要部分，其规模大到需要生成独立的小型（sub-tests）以提供对模块或模块组的充分覆盖。 -->
<!-- GUIDANCE: Major Test Section 1 可关联特定 SW Module 或 HW part，其中专用 Sub Test sections 可指定计划用于完成 coverage 的测试组或测试循环。可为另一个模块或 HW part 建立 Major Test Section 2。] -->

#### 测试前置条件

<!-- GUIDANCE: [本节列出运行测试前系统整体设置的信息。例如：以下测试用例假定 tester 已以“patient”身份登录 Server Workstation。] -->
<!-- GUIDANCE: [填写 “Test Run” header 旁边的 n，使其匹配上方 Timeline table。] -->
<!-- GUIDANCE: 对于涉及软件的测试，记录 programs/scripts 的 release IDs： -->
<!-- GUIDANCE: [可按需要添加或删除下表中的行。] -->
<!-- GUIDANCE: SW Executables: -->

| Test Run <n> |
| --- |

<!-- GUIDANCE: Test Equipment/setup: -->
<!-- GUIDANCE: [本节应列出本 Test Run 使用的各类测试设备。所有设备都应经过校准，并在下方提供上次校准日期。] -->
<!-- GUIDANCE: [填写 “Test Run” header 旁边的 n，使其匹配上方 Timeline table。] -->
<!-- GUIDANCE: [如果本节不适用，例如 SW testing，可以删除本节。] -->
<!-- GUIDANCE: [可按需要添加或删除下表中的行。] -->

| Test Run <n> |
| --- |

<!-- GUIDANCE: <Sub Test Section 1> -->
<!-- GUIDANCE: [Sub Test Sections 用于将模块或模块组拆分为更小区域以便测试。例如：对于一个模块，Major Test Section 可以是 GUI presentation，然后是一组 Sub Test Sections，用于测试 GUI 的各个独立部分，例如每个 screen display。使用模板进行 SW WIP Sanity Testing 时，报告中需要列出被测 WIP 覆盖的 AR。] -->
<!-- GUIDANCE: Objective: <本测试章节目标的简短陈述> -->
<!-- GUIDANCE: AR: <此测试覆盖的 Defect 或 EI 编号（如适用）。否则说明 N/A> -->
<!-- GUIDANCE: Setup: <添加适用于 Sub Test 的具体设置细节。如果不需要其他细节，写 N/A> -->
<!-- GUIDANCE: Detailed Description: <如适用，添加与此特定测试章节 1 相关的详细信息。否则说明 N/A> -->
<!-- GUIDANCE: [可按需要添加或删除下表中的行。] -->

| Test ID/名称 | 操作者动作 | 预期结果 [注意：客观说明验收标准。只包含无需判断即可确定 pass 或 fail 的离散定量或定性验收标准。] | 实际结果和证据 | Tester ID 或姓名 | 执行日期 | 状态 (Pass / Fail / Not Run) | 备注 | Change Request Number 或 AR number（如适用） |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |

<!-- GUIDANCE: <注意：Test Results 的 Status outcome 可以是“Pass”“Not Run”或“Fail”。应在 Comments 列中提供“Not Run”的理由和“Fail”的说明。> -->

### 时间线

<!-- GUIDANCE: [本节为可选项。如果不使用下表，请指定“N/A”。] -->
<!-- GUIDANCE: [本节为每个 Test Run 列出一个数字值 (n)，以及该 run 的开始和结束日期。Test Run number (n) 在后续表格中用于引用本摘要中的顺序测试运行。] -->
<!-- GUIDANCE: [可按需要添加或删除下表中的行。] -->

| 测试运行 | 实际测试运行开始日期 | 实际测试运行完成日期 | 描述 [可选] |
| --- | --- | --- | --- |

### 模块验证配置

#### 系统/子系统配置

<!-- GUIDANCE: [本报告通常假设使用默认的生产配置系统进行模块验证。如果不可用，则在下方提供足够参考信息，以描述将用于测试的系统（或系统一部分）配置。] -->
<!-- GUIDANCE: [填写 “Test Run” header 旁边的数字，使其匹配上方 Timeline table（如适用）。] -->
<!-- GUIDANCE: [可按需要添加或删除下表中的行。] -->

| Test Run <n> |
| --- |

### 模块集成测试覆盖率

<!-- GUIDANCE: [本节仅适用于 Module-Integration Report。如果目的不包含 Integration activities，请指定“N/A”。] -->
<!-- GUIDANCE: [此表是否使用由 Development Engineer / Test Engineer 自行决定。] -->
<!-- GUIDANCE: [如果生成 Module-Integration Report，则可在下表中列出相关信息。此表用于指示针对 Module-Integration Plan 中“Modules to be Tested”章节识别的 Module Groups 所提供的测试覆盖量和类型。] -->
<!-- GUIDANCE: [填写 “Test Run” header 旁边的 n，使其匹配上方 Timeline table。] -->
<!-- GUIDANCE: [可按需要添加或删除下表中的行。] -->

| Test Run <n> |
| --- |

### 识别出的异常记录 (AR) 和变更请求

<!-- GUIDANCE: [为任何“Failed”的 test case，提供作为本 Test Run 结果生成的 change requests 摘要列表。] -->
<!-- GUIDANCE: [填写 “Test Run” header 旁边的 n，使其匹配上方 Timeline table（如适用）。] -->
<!-- GUIDANCE: [可按需要添加或删除下表中的行。] -->

| Test Run <n> |
| --- |

### 测试偏差

<!-- GUIDANCE: [指出所有偏离 test script 的情况及其发生原因。当对 script 的偏离阻止步骤按 test script 指定程度执行时，应重新设计、重新批准并执行该测试。] -->
<!-- GUIDANCE: [填写 “Test Run” header 旁边的 n，使其匹配上方 Timeline table。] -->
<!-- GUIDANCE: [可按需要添加或删除下表中的行。] -->

| Test Run <n> |
| --- |

### 可追溯性

<!-- GUIDANCE: [应展示 Module verification 到 requirements 的 traceability。Traceability 可作为附录附加到本文档，也可由随附 TRR 提供。如果 Traceability 由单独文档或附录提供，则可删除下表，并添加对附录或 TRR 的引用。] -->
<!-- GUIDANCE: [report level Traceability Matrix 至少应捕获下方信息。对于 SW Modules，procedure/report 到 SW Module name 的 traceability 即可。] -->

| 需求 ID 或规格 ID [对于 SW Modules traceability，指定 design document DHF] | 模块/单元/组件 | Test ID/Test Name | 测试状态 |
| --- | --- | --- | --- |

### 结论

<!-- GUIDANCE: [在本段总结 module verification 或 module integration 的结果。说明测试是否被认为成功以及此判断的理由。可谨慎给出建议，说明 testing process 中的下一步（例如后续 test run、System Integration、Verification 等）是否可基于本报告及其他适用 Module 和/或 Module-Integration Test Reports 的结果开始。] -->
<!-- GUIDANCE: [可按需要添加或删除下表中的行，下方仅为示例。] -->
<!-- GUIDANCE: <示例：DHFXXXX 中描述的所有测试均已成功通过。> -->
<!-- GUIDANCE: <示例：Test XXX 已成功通过，Test Number XXX 失败并需要重新运行。> -->

## 术语和缩写

<!-- GUIDANCE: [按需要包含适用定义。可按需要添加或删除下表中的行。] -->

| 术语 / 缩写 | 描述 |
| --- | --- |

## 附录

<!-- GUIDANCE: [附录应只作为额外支持信息或指导。如果没有附录，请在下表中填入“N/A”或“Not Applicable”。] -->
<!-- GUIDANCE: [可按需要添加或删除下表中的行。] -->

| 附录 | 标题 |
| --- | --- |

## 参考资料

<!-- GUIDANCE: [按需要包含适用参考。可按需要添加或删除下表中的行。] -->

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

<!-- GUIDANCE: 所有手工记录测试（未在 ALM 中记录）的 testers 都应签署 DCO，并在下表中标识。他们的签名确认测试由他们在所列日期记录。 -->
<!-- GUIDANCE: 签名和日期会作为 document change order 的一部分在 PLM tool 中捕获。 -->

| 签名原因 | 职能 | 姓名 |
| --- | --- | --- |

<!-- GUIDANCE: repeatable example section: 附录 <X> – <附录名称> -->
