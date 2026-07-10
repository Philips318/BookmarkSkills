# 子系统设计规格

## 目的

<!-- GUIDANCE: 本文档的目的是为产品 <Product Name> 提供 <System or Sub-System Design Specification>。 -->

## 范围

<!-- GUIDANCE: 本记录适用于 Philips CT/AMI。 -->
<!-- GUIDANCE: [提供设计文档范围说明，以细化本文档描述的设计边界。] -->

## 架构设计目标

<!-- GUIDANCE: [作者：Lead Designer 或 Design Authority] -->
<!-- GUIDANCE: [为保持整个系统的一致性，最好由 Lead Designer 编写本节。但出于时间限制和工作分配考虑，由 Design Authority 在 Overview subsection 之后完成部分或全部 subsections 可能更实际。在这种情况下，Lead Designer 应是评审者之一。] -->

### 概述

<!-- GUIDANCE: [在此呈现 system 或 Sub-System 的简短历史、背景和用途。包含 system 或 Sub-System 如何适配产品的描述，并在适用时引用 parent PRS 或 SDS。] -->

### 假设和约束

<!-- GUIDANCE: [列出创建本文档时使用的任何 assumptions constraints。这可包括跨产品线影响、被强制“re-used”而非重新开发的现有 components 等。] -->

## 系统架构

### 设计概述

<!-- GUIDANCE: [本节描述系统顶层设计，包括运行理论和系统分解。呈现每个子系统及其关系。交互和行为在后续处理。可按需要添加其他 subheadings。] -->
<!-- GUIDANCE: Design overview 描述整体图景，以帮助理解后续细节。在此使用 diagram(s) 帮助解释整体系统设计以及子系统之间的关系。如果项目团队决定需要或不需要 Sub-System Design Specification documents 来恰当记录系统架构，应在本节说明该决定并提供适当理由。] -->

### 主要设计考虑

<!-- GUIDANCE: [说明与设计相关、需要理解的任何一般性问题。这可包括制造设计考虑、服务设计考虑、成本、可靠性、可用性等问题。] -->
<!-- GUIDANCE: [如适用，对于第一次在本 System 或 Sub-System design 中实现的新技术：引用对应 Proof of Concept Report(s)，以验证要采用的技术处于组织能力范围内。可在后续“issue”heading 下适当讨论。] -->
<!-- GUIDANCE: [按需要在下方添加其他 issues。] -->
<!-- GUIDANCE: repeatable example section: <Issue One> -->
<!-- GUIDANCE: <按需要添加细节> -->
<!-- GUIDANCE: repeatable example section: <Issue Two> -->
<!-- GUIDANCE: <按需要添加细节> -->

### 设计特性

<!-- GUIDANCE: [描述：] -->
<!-- GUIDANCE: [描述 dynamic model、workflows，或具体说明 Sub-Systems 或 components 如何协同实现所需 features。可酌情使用自然语言、图形模型和 use cases。Design features 通常聚焦 Clinical use，但新产品可能包括其他新设计特性，例如 License Keys、Remote Service 等。] -->
<!-- GUIDANCE: [可为每个 use case 或 intended use 使用单独 subsections。随着更多 features 添加到 system 或 Sub-System，本文档的新 revision 中会添加新的 subsections。新 features 可能需要也可能不需要改变系统或 Sub-System 使用的 components。] -->
<!-- GUIDANCE: [示例：] -->
<!-- GUIDANCE: [Axial Scans；Cardiac Arrhythmia Handling；Brain Perfusion；Remote Service；New Security Tools；等。] -->

### 硬件设计

<!-- GUIDANCE: [System Level：可选。Hardware design 是 System Architecture 的一部分，通常适用于 Sub-Systems details。如果适用于 system level，可包含对具体主要 HW design approaches 或 concepts 的简述，并引用后续更详细的 Child documents，或指向实施的 Sub-Systems。] -->
<!-- GUIDANCE: repeatable example section: <Hardware Sub-System One> -->
<!-- GUIDANCE: [按适用情况包含 System/Sub-system overview。如果已为该 sub-system 编写 SSDS 或 EDS，请包含 overview，提供对 SSDS 或 EDS 或其他 Child Detailed Design Documents 的引用，不要包含下方 detail sections，因为细节将在 SSDS、EDS 或其他 DHF child documents 中。] -->
<!-- GUIDANCE: repeatable example section: <Detail or Module One> -->
<!-- GUIDANCE: [描述 sub-system 细节。描述设计将如何满足 hardware sub-system 的 features、functionality 和 performance requirements。] -->
<!-- GUIDANCE: [酌情对设计关键元素执行 Make、Buy 或 Reuse analyses。例如，如果可选择内部开发 cable assembly 或外包，请描述该决定的理由。] -->
<!-- GUIDANCE: [或：] -->
<!-- GUIDANCE: [如果已为 module 编写 EDS，请包含 overview，提供对 EDS 的引用，不要包含 detail，因为它将在 EDS 中。] -->
<!-- GUIDANCE: repeatable example section: <Detail or Module Two> -->
<!-- GUIDANCE: [按需要添加其他 details 或 modules。] -->
<!-- GUIDANCE: repeatable example section: <Hardware Sub-System Two> -->
<!-- GUIDANCE: repeatable example section: [按需要添加其他 sub-systems。] -->

### 软件设计

<!-- GUIDANCE: [System：应提供 software architecture 的整体描述，包括整体 software design、相关 sub-systems 以及它们之间 interfaces 的图形表示。Software architecture 可在后续章节进一步详述。 -->
<!-- GUIDANCE: Sub-System：应提供 Sub-System software architecture 的整体描述，包括 software design、相关 sub-components 以及它们之间 interfaces 的图形表示。Sub-Systems software architecture 可在后续章节进一步详述。] -->
<!-- GUIDANCE: repeatable example section: <Software Sub-System One> -->
<!-- GUIDANCE: [System Element（仅 SDS）：显示包含本节讨论的 sub-system elements 的 system software architecture 顶层 block diagram，目的是识别 sub-system element 在整体 architecture 中的实现，以及 intra-sub-system interface information，例如 data flow methodology、interaction technology（如 RPC/P2P Protocol TCP/IP Shared Memory 等）。] -->
<!-- GUIDANCE: [Sub-system element（仅 SSDS）：包含 sub-system overview，包括 sub-system design、相关 modules 以及 interfaces 之间关系的图形表示。如果已为该 sub-system 编写 SSDS 或 SwDS，请包含 overview，提供对 SSDS 或 SwDS 的引用，不要包含下方 detail sections，因为细节将在 SSDS 或 SwDS 中。包含 software sub-system inputs 和 outputs 的定义和/或描述。] -->
<!-- GUIDANCE: repeatable example section: <Detail or Module One> -->
<!-- GUIDANCE: [描述 sub-system 细节。描述设计将如何满足 software sub-system 的 features、functionality 和 performance requirements。包括 ranges、limits、defaults 以及 specific values 的使用。] -->
<!-- GUIDANCE: [酌情对设计关键元素执行 Make、Buy 或 Reuse analyses。例如，如果可选择内部开发 software application 或外包，请描述该决定的理由。] -->
<!-- GUIDANCE: [或：] -->
<!-- GUIDANCE: [如果已为 module 编写 SwDS，请包含 overview，提供对 SwDS 的引用，不要包含 detail，因为它将在 SwDS 中。] -->
<!-- GUIDANCE: repeatable example section: <Detail or Module Two> -->
<!-- GUIDANCE: [按需要添加其他 details 或 modules。] -->
<!-- GUIDANCE: repeatable example section: <Software Sub-System Interface Detail> -->
<!-- GUIDANCE: [应定义并描述此 sub-system 与其他 software sub-systems 和/或相关 hardware sub-systems 之间的 interfaces。] -->
<!-- GUIDANCE: [或：] -->
<!-- GUIDANCE: [如果已为 interface 编写 SwDS，请包含 overview，提供对 SwDS 的引用，不要包含 detail，因为它将在 SwDS 中。] -->
<!-- GUIDANCE: repeatable example section: <Software Sub-System Two> -->
<!-- GUIDANCE: [按需要添加 sub-systems。] -->

#### 系统和子系统软件风险分类

<!-- GUIDANCE: [SW Classification 在 System level 显示，并带有 Sub-systems 的抽象分类图，以展示从 System 到 Sub-system 的 classification hierarchy。] -->
<!-- GUIDANCE: [Software 应按 Risk Management SOP (2003000438) 分类。SW Classification 应根据所选 complexity 和 decomposition approach，在 System Level (SDS) 和/或 Sub-System Level (SSDS and SwDS) 讨论。Software Classification 应显示在与 Software Module 关联的 product Risk Management Matrix 行项中。Software module 的 classification 将作为设计过程的一部分使用。] -->
<!-- GUIDANCE: Software Module Classification Table: -->
<!-- GUIDANCE: * 此 Software Module table 用于描述 system design。此表不执行 software classification；它只使用相关信息进一步描述 design decisions。 -->

| 软件模块 | 软件模块描述 | 安全分类（仅 B 或 C） |
| --- | --- | --- |

#### 子系统软件风险分类隔离理由

<!-- GUIDANCE: [此处为 A 和 B 的 SW Items 提供 SW segregation rationale。] -->
<!-- GUIDANCE: [从本行到本节末尾提供示例。] -->
<!-- GUIDANCE: <software ARCHITECTURE 应促进安全运行所需 SOFTWARE ITEMS 的隔离，并描述用于确保这些 SOFTWARE ITEMS 有效隔离的方法。隔离不限于物理（processor 或 memory partition）分离，还包括任何防止一个 SOFTWARE ITEM 对另一个造成负面影响的机制。隔离的充分性基于所涉及 RISKS 确定，并且需要记录 rationale。 -->
<!-- GUIDANCE: 下图说明 SOFTWARE SYSTEM 中 SOFTWARE ITEMS 的可能分区，以及 software safety classes 如何应用于分解中的 SOFTWARE ITEMS 组。 -->
<!-- GUIDANCE: Figure – Example of partitioning of SOFTWARE ITEMS -->
<!-- GUIDANCE: 在此示例中，MANUFACTURER 由于正在开发的 MEDICAL DEVICE SOFTWARE 类型，知道 SOFTWARE SYSTEM 的初始 software safety classification 是 software safety class C。在 software ARCHITECTURE design 期间，MANUFACTURER 决定按图所示将 SYSTEM 划分为 3 个 SOFTWARE ITEMS – X、W 和 Z。MANUFACTURER 能够将所有可能导致死亡或 SERIOUS INJURY 的 HAZARDS 和 HAZARDOUS SITUATIONS 的 SOFTWARE SYSTEM contributions 隔离到 SOFTWARE ITEM Z，并将所有其余可能导致 non-SERIOUS INJURY 的 HAZARDS 和 HAZARDOUS SITUATIONS 的 SOFTWARE SYSTEM contributions 隔离到 SOFTWARE ITEM W。SOFTWARE ITEM W 分类为 software safety class B，SOFTWARE ITEM Z 分类为 software safety class C。因此 SOFTWARE ITEM Y 必须分类为 Class C。根据此要求，SOFTWARE SYSTEM 也为 software safety class C。SOFTWARE ITEM X 已分类为 software safety class A。MANUFACTURER 能够记录 SOFTWARE ITEMS X 和 Y 之间以及 SOFTWARE ITEMS W 和 Z 之间隔离的 rationale，以确保隔离完整性。如果 SOFTWARE ITEMS X 和 Y 之间无法进行 partitioning segregation，则 SOFTWARE ITEM X 必须分类为 software safety class C。> -->

### 机械设计

<!-- GUIDANCE: [System Level：可选。Mechanical design concept 可以作为 System Architecture Overview 的一部分，细节通常适用于 Sub-Systems details。如果适用于 system level，可包含主要 mechanical design approaches 或 concepts 的简述，并引用后续更详细的 Child documents，或指向实施的 Sub-Systems。示例：Air Bearing、Tilt、显著增强性能的新 Couch、更快旋转、Spherical DMS 等。] -->
<!-- GUIDANCE: repeatable example section: <Mechanical Sub-System One> -->
<!-- GUIDANCE: [包含 sub-system overview。如果已为该 sub-system 编写 SSDS，请包含 overview，提供对 SSDS 或 MeDS 的引用，不要包含下方 detail sections，因为细节将在 SSDS 或 MeDS 中。] -->
<!-- GUIDANCE: repeatable example section: <Detail or Module One> -->
<!-- GUIDANCE: [描述 sub-system 细节。描述设计将如何满足 software sub-system 的 features、functionality 和 performance requirements。] -->
<!-- GUIDANCE: [酌情对设计关键元素执行 Make、Buy 或 Reuse analyses。例如，如果可选择内部开发和/或制造承重 bracket 或外包，请描述该决定的理由。] -->
<!-- GUIDANCE: [或：] -->
<!-- GUIDANCE: [如果已为 module 编写 MeDS，请包含 overview，提供对 MeDS 的引用，不要包含 detail，因为它将在 MeDS 中。] -->
<!-- GUIDANCE: repeatable example section: <Detail or Module Two> -->
<!-- GUIDANCE: [按需要添加其他 details 或 modules。] -->
<!-- GUIDANCE: repeatable example section: <Mechanical Sub-System Two> -->
<!-- GUIDANCE: [按需要添加其他 sub-systems。] -->

### 接口

<!-- GUIDANCE: [System (SDS)：识别 system/sub-systems 彼此之间以及与 external entities 交互的方式。应针对每种 communication protocol、network topology、data coupling 或 hardware interface 使用单独 subsections 或 referenced child DHF documents。如果 System Architecture section 中已有足够细节，注意不要重复。 -->
<!-- GUIDANCE: Sub-Systems (SSDS)：识别 modules 彼此之间以及与 external entities 交互的方式。应针对每种 communication protocol、network topology、data coupling 或 hardware interface 使用单独 subsections 或 referenced child DHF documents。] -->
<!-- GUIDANCE: [良好实践是在此 (SDS) document 中仅做 high level mapping 和 description，并指向详细描述 interfaces 的 DHF “Child” documents。本文档作者将根据 complexity 和所需 breakdown 决定方法。] -->

### 数据库和持久化数据元素

<!-- GUIDANCE: [System/Sub-System - 定义任何 databases 或其他 data storage design elements，包括 data file formats 和 data management。良好实践是在此 (SDS) document 中仅做 high level mapping 和 description，并指向详细描述 design 或 interfaces 的 DHF “Child” documents。本文档作者将根据 complexity 和所需 breakdown 决定方法。] -->

### 错误处理和报告

<!-- GUIDANCE: [System/Sub-System：定义 errors 和 fault conditions 的任何 sub-system level design elements。良好实践是在此 (SDS) document 中仅做 high level mapping 和 description，并指向详细描述 interfaces 的 DHF “Child” documents。本文档作者将根据 complexity 和所需 breakdown 决定方法。] -->

### 第三方解决方案

<!-- GUIDANCE: [识别任何 third party solutions（off-the-shelf software / hardware、non-medical origins 的 sub-systems 等）以及为纳入这些解决方案所需的设计。注意，这也应流入 lower level design specifications。每个此类解决方案都应在本 specification 中唯一识别（编号或命名），并在 lower level specifications 中引用。 -->
<!-- GUIDANCE: 3rd Party Solutions 是 System/Sub-System 的一部分，因此其 features、performance、interfaces、serviceability、系统/Sub-System 内的 infrastructure，以及任何其他相关 design details 都属于 design documents 的一部分，尽管其 lower level internal design 可能对我们未知（例如 SOUP）。] -->
<!-- GUIDANCE: [注意与 Section 4.5 Software Design 的潜在重复。按需要避免重复并使用 references。] -->

### 遗留元素

<!-- GUIDANCE: [识别任何 legacy elements（software 或 hardware）以及纳入它们所需的 design considerations。注意，这也应流入 lower level design specifications。每个此类 legacy element 都应在本 specification 中唯一识别（编号或命名），并在 lower level specifications 中引用。] -->
<!-- GUIDANCE: [注意与 Section 4.5 (Software Design) 或 Section 4.10 (Third Party Solutions) 的潜在重复。按需要避免重复并使用 references。Legacy Elements 是 System/Sub-System 的一部分，因此其 features、performance、interfaces、serviceability、系统/Sub-System 内的 infrastructure，以及任何其他相关 design details 都属于 design documents 的一部分，尽管其 lower level internal design 可能对我们未知，因为这些元素很久以前定义和设计。在这种情况下，可将这些元素视为 SOUP。] -->

## 功能和性能规格

<!-- GUIDANCE: [列出上方定义的设计中描述的 functional 和 performance specifications。按适用情况考虑 system/Sub-System level software、hardware 和 mechanical interactions。这些 specifications 的示例包括： -->
<!-- GUIDANCE: Memory maps -->
<!-- GUIDANCE: Register assignments -->
<!-- GUIDANCE: Power consumption -->
<!-- GUIDANCE: Processing algorithms and throughput -->
<!-- GUIDANCE: Sample and data rates -->
<!-- GUIDANCE: Signal ranges and shaping -->
<!-- GUIDANCE: Timing of critical operations（acquisition 和 reconstruction with default protocols） -->
<!-- GUIDANCE: Error rates、standard deviations 等 -->
<!-- GUIDANCE: Sample and data rates -->
<!-- GUIDANCE: Computer architecture and performance -->
<!-- GUIDANCE: Safety considerations / required mitigations per Risk Management File -->
<!-- GUIDANCE: Weight supported -->
<!-- GUIDANCE: Positioning accuracy -->
<!-- GUIDANCE: Speed(s) -->
<!-- GUIDANCE: Acceleration(s) -->
<!-- GUIDANCE: Range(s) of travel -->
<!-- GUIDANCE: Simulators and / or designs for their use -->
<!-- GUIDANCE: Critical components] -->
<!-- GUIDANCE: 注意：上方许多项目可能已在其他章节覆盖。避免重复。] -->

## 安全和法规

<!-- GUIDANCE: [描述用于处理 OSHPD、UL、EMC 等 regulatory concerns 的任何 design elements。如果其他地方未覆盖，本节也可用于描述设计如何处理 Risk Management related requirements。 -->
<!-- GUIDANCE: SDS 中只提供主要 design guidelines。大多数详细描述自然会出现在继承 SRS 到 SSRS 这些 requirements 的 SSDS 中。] -->

## 现场部署

### 支持的配置

<!-- GUIDANCE: [与产品现场部署相关，定义 supported configurations，包括 peripherals、与其他 systems 的 interaction、network topologies 等。] -->

### 安装

<!-- GUIDANCE: [System/Sub-Systems：与产品现场部署相关，定义 hardware 和 software installation 的 design elements。包括 room configurations、environmental considerations、special shipping and packing、documentation、calibration and adjustment、tools and equipment、installation safety、installation time、user documentation、localization、training，以及 performance testing and software。] -->

### 升级

<!-- GUIDANCE: [System/Sub-Systems：与产品现场部署相关，定义允许产品升级的 design elements。] -->

## 制造、服务和支持

<!-- GUIDANCE: [System/Sub-Systems：在本节识别任何 diagnostics 和其他 tools，包括 documentation。Testing access points、diagnostics、test fixtures、calibrations 等都应涵盖。可按需要添加其他 subsections。] -->

### 系统校准和质量保证

<!-- GUIDANCE: [定义优化系统并在制造、安装和现场保持其性能所需的 test beds、calibrations、test fixtures、software tools 等，包括 serviceability。] -->

### 面向制造的设计

<!-- GUIDANCE: [SSDS：大多数 manufactured parts 关联 Sub-Systems。SDS：某些 System approaches 和 highlights 可属于 SDS。SDS/SSDS 可指向带有更详细 design specifications 的 DHF Child Documents。] -->

### 面向可服务性的设计

#### 系统/子系统安装和升级

<!-- GUIDANCE: [定义 System & Sub-System installation design elements。] -->

#### 远程服务和监控

<!-- GUIDANCE: [定义用于 remote connectivity elements、monitoring 和 remote export data mining 的 System & Sub-System design elements。] -->

#### 面向诊断和系统配置的设计

<!-- GUIDANCE: [本节涵盖主题例如： -->
<!-- GUIDANCE: Diagnostics & Trouble shooting - 定义 Visual Diagnostics 和其他 troubleshooting tools（包括 remote access）的 design elements。 -->
<!-- GUIDANCE: System Logs and Log Viewer(s) - 定义捕获和管理任何 persistent logs 的 design elements。 -->
<!-- GUIDANCE: System Configuration (Service View) - 定义捕获 System Configuration design 的 design elements。] -->

#### 系统维护工具以及支持可服务性的 HW 和 SW 设计

<!-- GUIDANCE: [定义捕获 System Maintenance tools（包括 remote access 和 O-Level Access）的 design elements。指向用于 serviceability HW design 和 serviceability SW requirements 的 sub-system hardware design specification。] -->

## 设计元素 – 需求规格可追溯性

<!-- GUIDANCE: [本章仅适用于 SSDS；如果不存在 SSDS documents，才应包含在 SDS 中。如果不适用于本文档，请说明 N/A。 -->
<!-- GUIDANCE: 对于 sub-system main elements（main design output parts，例如 FRUs），定义此 element 被设计用于满足哪些 sub-system requirements（Design Input）。] -->
<!-- GUIDANCE: DMS sub-system 示例： -->

| 子系统元素名称 | 元素类型 | 料号 / 唯一 ID | SSRS Requirements ID(s) |
| --- | --- | --- | --- |

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

<!-- GUIDANCE: 签名和日期会作为 document change order 的一部分在 PLM Tool 中捕获。 -->

| 签名原因 | 职能 | 姓名 |
| --- | --- | --- |

<!-- GUIDANCE: 附录 <x> – <标题> -->
