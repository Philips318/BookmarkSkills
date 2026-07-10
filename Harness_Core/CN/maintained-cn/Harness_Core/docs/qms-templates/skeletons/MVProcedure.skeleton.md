# 模块验证程序

## 目的

<!-- GUIDANCE: 本文档定义 <insert Project Name here> <选择：Module Verification 或 Module-Integration> 的顺序步骤和预期测试结果。 -->

## 范围

<!-- GUIDANCE: <描述计划作为本程序一部分进行测试的模块列表。> -->
<!-- GUIDANCE: 本记录适用于 Philips、CT/AMI。 -->

## 技术管理信息

<!-- GUIDANCE: 日期 -->
<!-- GUIDANCE: 议程 -->
<!-- GUIDANCE: 出席情况 -->
<!-- GUIDANCE: [必须在此表中记录本次评审的所有受邀人员。评审可离线执行。可按需要增加行。] -->

| 评审者的能力/职能 | 出席情况 |
| --- | --- |

<!-- GUIDANCE: 评审结果 -->
<!-- GUIDANCE: [如有必要，列出未关闭问题或意见。如果程序评审期间没有意见，请写入“<Module verification/Module-Integration> Procedure was reviewed and approved. There are no open items associated with this procedure.”] -->

## 内容

<!-- GUIDANCE: [本节详述将作为模块验证和/或模块集成一部分执行的已评审和批准的验证或集成活动内容。如果测试通过管理工具管理和批准，则应删除第 3.1 章，并用对附录或外部文档的适当引用替换。本文档附录包含创建软件模块验证或模块集成测试用例时建议考虑事项的指导。] -->
<!-- GUIDANCE: [通用模板说明： -->
<!-- GUIDANCE: 根据需要添加任意数量的 Major Test Sections 和 sub test sections。 -->
<!-- GUIDANCE: 作者需要填写：Test Number、Operator Action 和 Expected Results 数据。 -->
<!-- GUIDANCE: 应有明确的退出单元测试活动标准（例如 coverage threshold、对结果的非同行评审等）。 -->
<!-- GUIDANCE: 应有识别预期结果的 checklist 或 procedure。对于 SW module verification 或 integration，仅执行代码是不充分的，因为有些缺陷不会导致 crash 或 hang。] -->
<!-- GUIDANCE: repeatable example section: <Major Test Section 1> -->
<!-- GUIDANCE: [Major Test Section 旨在包含模块或模块组的主要部分（例如 SW module 或 HW part），其规模大到需要单独的小型测试（sub-tests 或测试循环）以覆盖模块或模块组需求。例如：对于一个模块，Major Test Section 可以是 GUI presentation，然后是一组 Sub Test Sections，用于测试 GUI 的各个独立部分，例如每个 screen display。可按需要添加 Major Test sections。] -->

#### 测试前置条件

<!-- GUIDANCE: [本节列出运行测试前系统整体设置的信息。例如：以下测试用例假定 tester 已以“patient”身份登录 Server Workstation。 -->
<!-- GUIDANCE: Test Setup: <描述运行本测试章节前所需的任何设置。确保受测模块的样本量足以生成统计显著结果，或提供相反理由。> -->
<!-- GUIDANCE: [对于涉及软件的测试，在下表记录 programs/scripts 的 release IDs。否则说明 N/A。] -->
<!-- GUIDANCE: [可按需要添加或删除下表中的行。] -->
<!-- GUIDANCE: SW Executables: -->

| 索引 | 可执行文件 | ?? ID | 版本 |
| --- | --- | --- | --- |

<!-- GUIDANCE: repeatable example section: <Sub Test Section 1> -->
<!-- GUIDANCE: [Sub Test Sections 用于将模块或模块组拆分为更小区域以便测试。可按需要添加 Sub Test sections。 -->
<!-- GUIDANCE: 使用模板进行 SW WIP Sanity Testing 时，报告中将提及被测 WIP 覆盖的 AR 列表。] -->
<!-- GUIDANCE: Objective: <本测试章节目标的简短陈述> -->
<!-- GUIDANCE: AR: <此测试覆盖的 Defect 或 EI 编号（如适用）。否则说明 N/A> -->
<!-- GUIDANCE: Setup: <添加适用于 Sub Test 的具体设置细节。如果不需要其他细节，写 N/A> -->
<!-- GUIDANCE: Detailed Description: <如适用，添加与此特定测试章节 1 相关的详细信息。否则说明 N/A> -->
<!-- GUIDANCE: [可按需要添加或删除下表中的行。] -->

| Test ID/名称 | 操作者动作 | 预期结果 [注意：客观说明验收标准。只包含无需判断即可确定 pass 或 fail 的离散定量或定性验收标准。] |
| --- | --- | --- |

### 可追溯性

<!-- GUIDANCE: [提供 Module Verification 或 Module Integration procedure 到适用需求的初始追溯。对于 SW modules testing，从 procedure 到 SW Module Name 的追溯即可。] -->

| 需求 ID 或规格 ID [对于 SW Modules traceability，指定适用 requirements document DHF] | 模块/单元/组件 | Test ID/Test Name [对于 SW Modules traceability，指定 Module Test procedure section numbers] |
| --- | --- | --- |

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

<!-- GUIDANCE: 签名和日期会作为 document change order 的一部分在 PLM tool 中捕获。 -->

| 签名原因 | 职能 | 姓名 |
| --- | --- | --- |

<!-- GUIDANCE: repeatable example section: 附录 <X> – <附录名称> -->
<!-- GUIDANCE: 使用此模板前删除以下页面。 -->

## 附录 A – SW 模块测试建议指南

<!-- GUIDANCE: 创建软件模块测试时需要考虑事项的指导： -->
<!-- GUIDANCE: Unit testing 可在两个层级执行：white box（developer 视角）和 black box（requirements 视角）。 -->
<!-- GUIDANCE: Unit testing 在 module 和 class 层级执行。如果多个 module 相关且同时编码，则可以一起进行 unit test。 -->
<!-- GUIDANCE: 所有新代码以及任何高风险修改或复用代码都应进行 unit test。 -->
<!-- GUIDANCE: 所有测试都可以通过 debugger 运行；但一旦路径被执行，unit testing tool 对识别路径和 coverage 很有用。 -->
<!-- GUIDANCE: 与流行看法相反，如果使用 debugger 填充测试路径所需值，不需要编写 stubs 来 unit test 代码。 -->
<!-- GUIDANCE: 在 developers 的 schedule 中明确列出 Unit testing。 -->
<!-- GUIDANCE: Unit testing 可按 branch 和 line coverage 衡量。 -->
<!-- GUIDANCE: 应以表格形式提供 executive summary，显示 test case 和 pass/fail/not tested yet 状态。 -->
<!-- GUIDANCE: Software Development Plan 应具体描述每个 unit 要测试什么。 -->
<!-- GUIDANCE: 作为 module 或 module-integration testing 的一部分，应提供被测 software units 到 requirements 的映射。 -->
<!-- GUIDANCE: Unit testing 不仅 scope 应标准化，其结果也应由 Lead Software Engineer 评审。 -->
<!-- GUIDANCE: 如果工具测量 code coverage，测试完成后按 lines of code 或 branches 记录。 -->
