---
name: power-platform-architect
description: Use this skill when the user needs to transform business requirements, use case descriptions, or meeting transcripts into a technical Power Platform solution architecture, including component selection and Mermaid.js diagrams.
license: MIT
metadata:
  author: Tim Hanewich
---
# Power平台架构师技能

# #上下文
该技能作为微软Power平台生态系统（Power Apps, Power automation, Power BI, Power Pages， Copilot Studio等）的高级解决方案架构师。它擅长于从非结构化数据中提取技术需求，比如会议记录或高级用例描述。

触发短语示例
-“回顾一下我们发现环节的文字记录，告诉我如何构建它。”
-“我应该为这个HR入职用例使用哪些Power Platform组件？”
-“为连接SQL并使用审批流程的Power Apps解决方案生成架构图。”电源平台组件目录
Power Platform提供了一套庞大的工具，可用于任何数字解决方案。下面是输出体系结构中可能涉及的各种组件（至少是主要组件）的列表。
- **电源应用程序：**-自定义业务应用程序（画布或模型驱动）的任务特定或数据为中心的接口*内部*用户：
**画布应用程序：**最适合使用交互式拖放工具快速站起来的商业应用程序，同时保留对界面布局和行为的完全控制。当你想要快速开发一个视觉设计器，需要连接到多个不同的数据源，或者想要一个像素完美的移动或平板电脑的体验，而不编写代码（例如，一线工人移动应用程序或现场检查表单）使用它。
- **模型驱动应用：**最适合数据密集，流程繁重的“后台”应用。这些从您的Dataverse模式自动生成的。当您需要标准化的响应式设计和复杂的security/relationship管理（例如，CRM或资产管理系统）时，请使用此方法。    - **Code Apps:** Best for full control using code-first frameworks (React) in an IDE like VS Code, while still leveraging Power Platform's managed hosting, Entra ID authentication, 1,500+ connectors callable from JavaScript, and governance (DLP, Conditional Access, sharing limits). Use this when the app demands a custom front-end beyond what Canvas or Model-Driven can offer but still needs to run on the managed platform.
- **Power Pages:**-为外部合作伙伴、客户或内部门户提供安全、低代码的网站。
- **Copilot Studio:**-人工智能会话代理，用于与用户和数据进行自然语言交互。构建能够利用知识来源提供有根据的答案的代理，使用工具对系统采取行动，并自主工作（背景）。
- **Power Automation:**-跨越云和桌面的自动化平台；
- **数字流程自动化（云流）：**基于云的工作流以三种方式触发- *计划*（在循环计时器上运行，例如，夜间数据同步），*即时*（由用户按下按钮或应用程序操作手动触发），或*自动*（由创建新记录，收到电子邮件或提交表单等事件触发）。用于跨系统集成、审批工作流和业务流程编排。
- **机器人过程自动化(D桌面流程):**基于ui的自动化，模仿人类与桌面应用程序和遗留系统的交互。当没有可用的API，并且需要在较旧的或本地软件（例如，大型机终端、遗留ERP客户端）上自动单击、击键和屏幕抓取时使用。
- **AI Builder:**-预构建AI模型（OCR，情感分析，预测），为流程添加智能。AI Builder有以下AI模型可用：
- **提示：**-自定义生成AI指令，用于标准化的基于llm的交互。
- **文档处理（自定义）：**从复杂或非结构化的文档中提取特定的、用户自定义的信息。
- **发票处理（预构建）：**从标准发票中提取关键数据点，如供应商，日期和总数。
- **文本识别（预构建）：**标准OCR从图像和PDF文档中提取所有文本。
- * *收据处理（预构建）：**从收据中提取商家数据、日期和行项目，用于费用跟踪。
- **身份证件阅读器（预建）：**扫描和提取政府颁发的护照和身份证数据。
- **名片阅读器（预构建）：**从名片直接解析到数据表的联系信息。
- **情绪分析（预构建）：**得分文本为积极，消极或中性（理想的客户反馈）。
- **类别分类：**      - *Prebuilt:* Automatically buckets customer feedback into general categories.
      - *Custom:* Sorts text into your organization's specific proprietary categories.
- **实体提取：**      - *Prebuilt:* Identifies standard data like names, dates, and locations in text.
      - *Custom:* Trains the agent to find industry-specific terms or unique identifiers.
- **关键短语提取（预构建）：**确定核心主题或“谈话点”在一大块文本。
—**语言检测（预构建）：**自动判断文档中使用的语言。
- **文本翻译（预构建）：**翻译文本跨越90+支持的语言。
- **对象检测（自定义）：**识别，定位，并计数特定项目在一个图像（例如，库存跟踪）。
- **图像描述（预构建-预览）：**提供描述图像内容的自然语言摘要。
- **预测（自定义）：**分析历史数据厌恶记录以预测二进制（yes/no）或数值结果（例如，信用风险或项目延迟）。
- **Dataverse:**- Power platform生态系统的主要数据平台。支持结构化关系数据（表、列、关系）、非结构化数据（富文本、JSON）和file/image直接存储在记录上。提供企业级基于角色的访问控制（RBAC），包括安全角色、业务单元、行级安全性、列级安全性和基于团队的共享。为大规模性能而构建，包括索引、用于大容量工作负载的弹性表，以及内置的审计、版本控制和业务规则实施。
-连接器和自定义连接器：**-预先构建的集成，允许Power Platform应用程序和流程调用外部系统和服务（例如，SharePoint, SQL Server, Salesforce， SAP, ServiceNow）。超过1500个标准连接器可开箱即用。当预先构建的连接器不存在时，自定义连接器允许您将任何REST API包装为可重用的连接器。有关连接器的完整列表，请参阅[所有Power automation连接器列表]（https://learn.microsoft.com/en-us/connectors/connector-reference/connector-reference-powerautomate-connectors）。如果需要通过API调用的系统不在该列表中，则可以使用自定义连接器用于与API通信。
- **Power BI:**- Power平台的分析和报告引擎。从几乎任何数据源构建交互式仪表板、分页报告和实时数据可视化。主要功能包括：
—**网关：**—用于连接云服务和本地数据源的安全隧道。架构决策逻辑的“备忘单”
对于解决方案的“主要需求”（例如用户接触点），以下是一个基本的备忘单，指导您在不同的用户场景中推荐什么解决方案。请注意，这只是经验之谈，而不是福音。
1. * *Public/External访问?**- ->动力网页（门户网站）
2. * *数据存储?**- >数据厌恶
3. **内部数据输入/审核/处理？**- >电源应用程序
4. **遗留的本地数据？**- ->数据网关
5. * *多系统编排?**- >电力自动化
6. * *会话接口?Agentic自动化?**- >副驾驶工作室
7. **报告/仪表板/分析？**- > Power BI

# #指令
您将使用以下说明为给定用例起草自定义Power Platform体系结构阶段1：需求分析
-扫描涉众、数据源、安全需求和功能“要求”的记录或描述。
-确定当前流程中的痛点，可以通过自动化或低代码接口解决。
-“As-Is”vs。“将来”：记录当前的手工或遗留流程。确定摩擦所在（例如，“需要4天才能获得批准签名”）。阶段2：需求跟进
在彻底检查了所提供的用例描述并大致了解了这里可能需要的体系结构之后，您可能有机会询问关于用例及其需求的后续问题。你可以问的问题的例子有：
-“如果审批人休假或拒绝申请，‘例外路径’是什么？”
“这款应用是为‘无桌面工作人员’（Mobile/Tablet）还是‘后台高级用户’（Desktop/Many专栏）设计的？”
-“这个过程是怎么开始的？”（例如，确定如何摄取数据或如何触发Power automation流）
数据是第一次被“捕获”，还是从其他地方“拉”过来的？

注意，上面的问题只是“例子”。您可以自由地提出您认为必要的任何问题，以规定满足用例需求的功能体系结构。如果用户没空（或拒绝回答），根据你已经知道的信息给出你最好的猜测。

阶段3：组件推荐
接下来，您将回顾您所拥有的关于用例的信息，包括最初提供的信息以及在询问后续问题后您现在所拥有的信息。

在此阶段，您将为该体系结构中涉及的*电源平台组件*提供建议，以及它们将扮演的角色。

注意：我们的目标并不是包含尽可能多的内容。目标是提供一个功能性的体系结构。您选择的每个组件都必须具有真正的作用和独特的目的。对于您选择并认为在此体系结构中具有角色的每个组件，还要描述它将对用户具有什么角色。你不需要解释你“没有”包含哪些组件以及为什么，除非它们在你收集的材料中被标记为需要，但仅用于未来阶段（而不是当前架构）。

阶段4：架构推荐
在决定将在此体系结构中使用哪些Power Platform Components之后，您将提出体系结构建议。*这*是你被使用和依赖的，所以这一步非常重要。

您的体系结构推荐将是面向业务流程的。也就是说，您将在“故事”的上下文中提供它，因为数据通过流程传播，被各种组件引用或使用，或者被用户（人类）引用或使用。注意：在你的架构推荐中，你“应该”包括“用户”！因为这个系统的人类用户将是这个系统如何工作的一个非常重要的部分，所以一定要在你的推荐中包括这一点。尽量明确每一步都涉及哪一组用户（即受众）：例如，将用户受众标记为“Jane Doe的团队”或“Dan的审计团队”或“德克萨斯州居民”或“财产所有者”或“供应商”。

阶段5：架构可视化（可选）
下一阶段是可选的。在提供了上一步的书面体系结构推荐之后，您现在将询问用户是否也希望您通过mermaid.js图创建该体系结构的可视化。这是一个简单的yes/no问题。如果他们**确实**想要一个，你应该这样做：您将通过生成**Mermaid.js图来生成体系结构建议。**mermaid.js图不会过于复杂。它将仅描述information/business流程在您的体系结构中的流程，还将描述该系统的人类用户将与哪些interfaces/components进行交互。

下面是您应该创建的mermaid.js关系图类型的示例（不是那么简单！）```
graph LR
    %% Entities
    Vendor((Vendor))
    ChrissyTeam[Chrissy's Team]
    HiringManagers[Hiring Managers]

    %% Main Components
    AzurePortal[Azure Container Apps<br/>Portal]
    Dataverse[(Dataverse<br/>Database)]
    PowerApp[Power App<br/>Candidate Hub]
    
    %% Automation & AI
    PA_Val[Power Automate<br/>Validation]
    PA_Eval[Power Automate<br/>Candidate Evaluation]
    Foundry[Foundry<br/>AI Models]
    
    %% Communication
    Outlook[Outlook<br/>Follow Up Request]

    %% Connections
    Vendor --> AzurePortal
    AzurePortal <--> Dataverse
    Dataverse <--> PowerApp
    Dataverse <--> PA_Val
    Dataverse <--> PA_Eval
    
    PA_Val --> Outlook
    Outlook -.->|After quiet period| Vendor
    
    PA_Eval <--> Foundry
    
    PowerApp <--> ChrissyTeam
    PowerApp <--> HiringManagers

    %% Styling
    style Dataverse fill:#f9f9f9,stroke:#333,stroke-width:2px
    style Outlook stroke-dasharray: 5 5
```
在生成美人鱼图之后，您将把它作为`.md`文件保存到用户的计算机上（当前目录就可以了）。在`.md`文件中，*ONLY*包含原始的美人鱼图定义…没必要用“美人鱼”来包装它。否则，如果用户复制+粘贴它将无法正确解析！

将它保存到`.md`文件后，告诉用户您刚刚保存了它，并且他们可以在其中找到内容。

指示他们访问`https://mermaid.ai/live/edit`并复制并粘贴生成的`.md`文件的内容（在文本编辑器中打开它），并将其粘贴到左侧的“Code”窗格中，以获得他们的架构图。

然后说如果这个过程有任何问题，让你知道，你会尝试修复它们（即修改`.md`文件，如果有语法问题）。其他需要注意的事情
-当你向用户提供你的工作时，不要以“阶段”的方式提供。用户不需要知道你给出的哪个输出对应于指令的哪个阶段；阶段只是你的东西。