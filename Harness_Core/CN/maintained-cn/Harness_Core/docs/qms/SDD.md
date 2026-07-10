# 软件设计文档

## 目的

<!-- GUIDANCE: <这个短段落应简单、简要地说明软件项做什么。它不是关于设计文档是什么的陈述；换句话说，不应写成“本文档识别某某软件项的设计”。> -->
<!-- GUIDANCE: [示例：CANopen Software Test Framework（“Framework”）提供所有 simple axis developers 用于开发 CANopen method call test stubs 的通用功能基础。这些 test stubs 使 developers 能够在没有 HW（例如 SIB、CAN bus、axis devices 等）的情况下测试其 axis software。] -->

## 范围

<!-- GUIDANCE: 本记录适用于 Philips CT/AMI。 -->
<!-- GUIDANCE: [在适用时，输入与文档实施受众或组织实体有关的信息，以及有助于理解范围的任何排除项。] -->

## 概述

<!-- GUIDANCE: <在此呈现软件项的简短历史、背景和用途。包含软件项如何适配产品的描述，并在适用时引用上级 SDS 或 SSDS。> -->
<!-- GUIDANCE: [示例：Scalable Gantry Architecture 由 SGA application 和 axis software 组成，如 SGA SSDS (DHF173114) 所识别。Software stubs 和 simulators 用于实现并测试 application 和 axis software，包括有和没有 axis 物理硬件的情况。考虑四个层级来模拟 axis，从 Simple Axis Stubs 到 CAN Bus Simulators，如下图所示： -->
<!-- GUIDANCE: Simple Axis Stubs 支持将 SGA application 向下到 simple axis interface 的有限测试；此方法用于早期 SGA 开发，在任何 hardware 和 axis software 可用前使用。CANopen SW Stubs 将这种可测试性扩展到包含 simple axis software。CAN I/O Driver Stubs 不会使用。CAN Bus Simulators 将可测试性扩展到包含 CANopen Stack、SIB & CAN driver 和 SIB hardware，从而支持将完整 Gantry PC 及其所有软件作为完整生产单元进行测试。] -->

#### 按 IEC 62304 – 第 5.4.1 节 – 强制适用于

<!-- GUIDANCE: Class B：可能造成非严重伤害 -->
<!-- GUIDANCE: Class C：可能造成死亡或严重伤害 -->
<!-- GUIDANCE: ] -->
<!-- GUIDANCE: [本节描述软件项的顶层设计，包括运行理论。如果描述的软件项不是一个单元，则包括分解为软件单元以及软件单元之间的关系。 -->
<!-- GUIDANCE: Design overview 描述整体图景，以帮助理解后续细节。图可以帮助解释整体设计以及 items 或其他 design Modules 之间的关系。内容和呈现方式必须有助于维护此设计的工程师以及预期目标受众。 -->
<!-- GUIDANCE: Design overview 或后续章节应包括对 software of unknown providence（SOUP，例如 off-the-shelf software / hardware、non-medical origins 的 subsystems 等）的识别，以及对 legacy Modules（software 或 hardware）和为纳入这些解决方案所需设计的识别。在上层设计或 requirements 中存在相关 identifiers 的范围内，应在此引用这些 identifiers。如果已知实际 supplier 或 component，应在本文档中识别。] -->

## 架构视图

<!-- GUIDANCE: <描述用于澄清 <Subject> 设计并为细化提供指导的相关 architectural views。 -->
<!-- GUIDANCE: 以下是指定 requirements 时可按适用情况考虑的 architecture views 示例（不是详尽列表）： -->
<!-- GUIDANCE: Functional View -->
<!-- GUIDANCE: Interface View -->
<!-- GUIDANCE: Physical View -->
<!-- GUIDANCE: Deployment View -->
<!-- GUIDANCE: Hardware View -->
<!-- GUIDANCE: Usage View -->
<!-- GUIDANCE: Network View -->
<!-- GUIDANCE: Interaction View -->
<!-- GUIDANCE: Communication/process View -->
<!-- GUIDANCE: Code Distribution View -->
<!-- GUIDANCE: Events View -->
<!-- GUIDANCE: Data Storage View -->
<!-- GUIDANCE: Segregation View -->
<!-- GUIDANCE: 为范围内每个 architecture view 添加一个 subsection 以详细说明该 view。 -->
<!-- GUIDANCE: Segregation：识别为风险控制所需的软件项隔离，并说明如何确保这种隔离有效。隔离示例之一是让软件项在不同 processors 上执行。此时可通过 processors 之间没有 shared resources 来确保隔离有效。也可以应用其他隔离手段，只要软件架构能够说明其有效性。 -->
<!-- GUIDANCE: > -->

### 软件架构视图

<!-- GUIDANCE: <描述 SW architecture，并添加 SW modules 及其连接关系的 block diagram> -->
<!-- GUIDANCE: repeatable example section: <View 2> -->
<!-- GUIDANCE: [如需要，在其他 view 中描述 SW architecture] -->

## 设计细节

<!-- GUIDANCE: <作为设计团队的起点，为 Module 提供所需 functionality、technical choices（hardware / software / mechanical / electrical）以及其他 design constraints。 -->
<!-- GUIDANCE: 如果进行了 module breakdown，请为 Module 中每个 Module 和 interface 提供所需 functionality、technical choices（hardware / software / mechanical / electrical）以及其他 design constraints。在这种情况下，在 4.2 Module detailed design section 中为每个 Module 创建单独章节，描述 functionality 和 design constraints。 -->
<!-- GUIDANCE: 如需要，也描述 Module 中各 parts 的 detailed design space。 -->
<!-- GUIDANCE: > -->
<!-- GUIDANCE: < 从 design output 到 design input requirements 的 traceability：在每节中包含对 SwRS 的引用（例如相关 SwRS section 或相关 requirement ids）> -->

### SW 质量属性分配

<!-- GUIDANCE: <将 quality aspects requirements 分配到 Module 上。 -->
<!-- GUIDANCE: 以下是描述设计时可按适用情况考虑的示例（不是详尽列表）： -->
<!-- GUIDANCE: Performance -->
<!-- GUIDANCE: Reliability -->
<!-- GUIDANCE: Memory Usage -->
<!-- GUIDANCE: > -->

### 详细模块设计

<!-- GUIDANCE: <描述 Module 的 detailed design space。如果进行了 module breakdown，请为每个 Module 提供 detailed design space。 -->
<!-- GUIDANCE: 以下是描述设计时可按适用情况考虑的示例（不是详尽列表）： -->
<!-- GUIDANCE: COTS -->
<!-- GUIDANCE: Start up -->
<!-- GUIDANCE: Processing execution models -->
<!-- GUIDANCE: Memory management -->
<!-- GUIDANCE: Logging, retrieving, and storing data -->
<!-- GUIDANCE: Exception handling -->
<!-- GUIDANCE: Interrupt handling -->
<!-- GUIDANCE: Self-test -->
<!-- GUIDANCE: State machines -->
<!-- GUIDANCE: Algorithms -->
<!-- GUIDANCE: User authentication -->
<!-- GUIDANCE: Security -->
<!-- GUIDANCE: SOUP -->
<!-- GUIDANCE: Serviceability（例如 diagnostics、logging） -->
<!-- GUIDANCE: 如果只描述一个 Module，请删除本节中所有 subsection titles <Module x> -->
<!-- GUIDANCE: > -->
<!-- GUIDANCE: repeatable example section: <Module 1> -->
<!-- GUIDANCE: [按 IEC 62304 – 第 5.4.1 节 – 强制适用于： -->
<!-- GUIDANCE: Class B：可能造成非严重伤害 -->
<!-- GUIDANCE: Class C：可能造成死亡或严重伤害] -->

##### 功能

<!-- GUIDANCE: <描述模块的 functionality> -->

##### 用例

<!-- GUIDANCE: <绘制与模块相关的所有 use cases> -->
<!-- GUIDANCE: repeatable example section: <Module 1> Design Details -->
<!-- GUIDANCE: <为模块描述并论证 technical design approach、constraints 和任何 design issue> -->
<!-- GUIDANCE: [IEC 62304 – 第 5.4.2 节：Class C] -->
<!-- GUIDANCE: [制造商应为软件项的每个软件单元开发并记录详细设计。] -->
<!-- GUIDANCE: [注意，本节应涵盖： -->
<!-- GUIDANCE: Performance related design -->
<!-- GUIDANCE: SW 使用的 equations 和 algorithm（可引用外部 algorithm spec） -->
<!-- GUIDANCE: 如适用，variables definitions 以及使用位置] -->

##### 类图

<!-- GUIDANCE: <绘制模块及其与其他模块（如有）连接关系的 class diagram> -->

##### 序列图

<!-- GUIDANCE: <为任何 use case 以及 module startup/power-up 绘制 sequence diagrams> -->

##### SW 安全分类

<!-- GUIDANCE: [识别软件项的 safety classification。如果 SWDS 描述的是 feature，说明本节不适用于 features。] -->
<!-- GUIDANCE: [IEC 62304 – 第 4.3.a 节：] -->
<!-- GUIDANCE: [software safety classes 应首先按严重性分配如下： -->
<!-- GUIDANCE: Class A：不可能造成伤害或健康损害 -->
<!-- GUIDANCE: Class B：可能造成非严重伤害 -->
<!-- GUIDANCE: Class C：可能造成死亡或严重伤害] -->
<!-- GUIDANCE: [示例：This is a Class C software item.] -->
<!-- GUIDANCE: [如果 SWDS 由多个 SW module 组成，请在模块本身写入 safety classification。例如： -->
<!-- GUIDANCE: SW Safety Classification 在本文档第 5.4 章下正在详细说明的任何模块中进行详述（作为 5.4.x.6）。] -->
<!-- GUIDANCE: repeatable example section: <Module n> -->
<!-- GUIDANCE: [结构与 Module 1 section 相同] -->

#### SOUP 项

<!-- GUIDANCE: [SOUP 表示 IEC 62304 术语中的 Software of Unknown Provenance。它包括此处描述的软件项使用的所有 third party solutions 和 legacy solutions。] -->

##### 第三方解决方案

<!-- GUIDANCE: [识别任何 third party solutions（off-the-shelf software / hardware、non-medical origins 的 subsystems 等）以及为纳入这些解决方案所需的设计。注意，这也应向下流入 lower level design specifications。每个此类解决方案都应在本 specification 中唯一识别（编号或命名），并在 lower level specifications 中引用。 -->
<!-- GUIDANCE: 如适用，将任何由 PHILIPS groups 提供但不遵循 QMS 的解决方案及其纳入设计识别为新 subsection。 -->
<!-- GUIDANCE: 按 IEC 62304 – 第 5.3.3、5.3.4 节 – 强制适用于： -->
<!-- GUIDANCE: Class B：可能造成非严重伤害 -->
<!-- GUIDANCE: Class C：可能造成死亡或严重伤害] -->
<!-- GUIDANCE: repeatable example section: <Third Party Solution 1> -->

####### 功能与性能设计

<!-- GUIDANCE: [列出 third party solution 的 functional & performance requirements] -->

####### 集成设计

<!-- GUIDANCE: [描述集成 third party tool 所需的设计。提及相关 interfaces 和 error handling mechanisms] -->
<!-- GUIDANCE: repeatable example section: <Third Party Solution 2> -->
<!-- GUIDANCE: [添加其他 third party solutions] -->

##### 遗留模块

<!-- GUIDANCE: [识别任何 legacy Modules（software 或 hardware）以及纳入它们所需的 design considerations。注意，这也应向下流入 lower level design specifications。每个此类 legacy Module 都应在本 specification 中唯一识别（编号或命名），并在 lower level specifications 中引用。] -->

### 接口设计

<!-- GUIDANCE: <如果进行了 module breakdown，请描述 Module 中每个 interface 的 detailed design space> -->
<!-- GUIDANCE: [以下是描述 interfaces 时可按适用情况考虑的示例（不是详尽列表）：protocol、deployment。否则删除本节。] -->
<!-- GUIDANCE: [按 IEC 62304 – 第 5.4.3 节 – 强制适用于：Class C：可能造成死亡或严重伤害] -->
<!-- GUIDANCE: [识别软件项与 external entities 交互的方式，例如 APIs、pipes、RPC methods、communication protocols、network topologies 或 hardware interfaces。按适用情况引用 HDD；按适用情况使用 diagrams。 -->
<!-- GUIDANCE: 同时描述软件项暴露的所有 public interfaces。] -->

### 测试设计

<!-- GUIDANCE: [按 IEC 62304 – 第 5.4.4 节 – 强制适用于：Class C：可能造成死亡或严重伤害] -->

#### 测试概述

<!-- GUIDANCE: [本节用于提供可能对 test developer 有用的信息。可包括测试实现建议、要测试的 key features 识别，以及 test developers 可能特别关注的 design details。] -->
<!-- GUIDANCE: [Test overview 描述整体图景，以帮助理解后续细节。图可以帮助解释测试方法或整体设计以及 modules 或其他 test Modules 之间的关系。内容和呈现方式必须有助于维护此测试的工程师以及预期目标受众。] -->

#### 主要测试考虑和设计

<!-- GUIDANCE: [按适用情况引用“Design”和“Implementation Specifications”章节。 -->
<!-- GUIDANCE: 在此描述如何测试 SW，例如使用 Simulator 来模拟系统的某个外部组件。] -->

### 设计环境

<!-- GUIDANCE: <本节用于提供开发环境信息。> -->
<!-- GUIDANCE: [以下是描述开发环境时可按适用情况考虑的示例（不是详尽列表）：IDE（Visual Studio、Eclipse）、programming languages（C#、C++）、intended OS 等。] -->

#### SW 开发工具

<!-- GUIDANCE: <描述用于开发 SW 的每个工具的版本，例如：Microsoft Visual Studio 2013 with dot NET framework 4.6.2> -->

#### 编程语言

<!-- GUIDANCE: <描述用于开发 SW 的 programming language，例如 C#、Java 等。> -->

#### 操作系统

<!-- GUIDANCE: <描述 SW 将在其上执行的 operating system，例如 Windows 7 64-bit and above、Ubuntu 16.04 64bit。> -->

### 安全特性

<!-- GUIDANCE: <在此描述 security features，包括未在 SW requirements 中处理的那些。> -->
<!-- GUIDANCE: [示例：All security features are inherited from the Console SW as we run this SW on the Host PC owned by the Console team] -->

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

| 签名原因 | 职能 | 姓名 |
| --- | --- | --- |

<!-- GUIDANCE: repeatable example section: 附录 <x> – <标题> -->
