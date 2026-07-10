#🔌插件

围绕特定主题、工作流程或用例组织的相关代理和技能的策划插件。插件可以通过GitHub CopilotCLI或VS Code直接安装。

b> **Awesome Copilot是一个默认的插件市场** -在Copilot CLI或VS Code中无需设置。
###如何贡献

请参阅[CONTRIBUTING.md]（../CONTRIBUTING.md# addingplugins）获取有关如何贡献新插件、改进现有插件和分享用例的指南。

如何使用插件

* *浏览插件:* *
-⭐特色插件突出显示，并出现在列表的顶部
-探索主题插件，组相关的自定义
-每个插件包括特定工作流程的代理和技能
-插件可以很容易地为特定场景采用全面的工具包**查找并安装在Copilot命令行：**
-在交互式副驾驶会话中浏览市场：\`/plugin marketplace browse awesome-copilot\`-安装插件：\`copilot plugin install <plugin-name>@awesome-copilot\`**查找并安装在VS Code:**
-打开扩展搜索视图，输入\`@agentPlugins\`来浏览可用的插件
-或者打开命令面板并运行\`Chat: Plugins\`-发布的市场清单（面向工具）：`https://raw.githubusercontent.com/github/awesome-copilot/marketplace/.github/plugin/marketplace.json`-源插件内容（人工编写）：`https://github.com/github/awesome-copilot/tree/main/plugins`|名称|描述|项目|标签|| ---- | ----------- | ----- | ---- |
从副驾驶聊天中驱动Microsoft AgentRC：评估AI准备情况，生成副驾驶指令（用于单节点的扁平或嵌套的applyTo globs），并管理策略。在reports/index.html.|生成一个自包含的静态HTML仪表板，包含4个项目：|代理、ai-readiness、副驾驶员指令、readiness-report、monorepo、policy、仪表板|
引导并运行一个多代理AI开发团队，并指定角色（制作人，开发团队，QA）。Sprint计划，头脑风暴提示与不同的代理声音，交叉聊天上下文生存，并行团队工作流程。基于一个经过验证的模板，该模板在5天内发布了一款30款游戏的应用，无需人工编写代码。| ai-team, multi-agent, sprint-planning, brainstorm, project management, orchestration, developer-workflow
| [arch](../plugins/arch/README.md) |建筑与现代化工具箱：produce为本地克隆的repo引用架构文档，并生成一个分阶段的现代化计划，该计划在需要时自动运行Documentation模式。|体系结构、现代化、文档化、迁移、登录|
掌握用于LLM可观察性、评估和优化的AX平台技能。包括跟踪导出、仪器仪表、数据集、实验、评估器、AI提供程序集成、注释、提示优化和到Arize UI的深度链接。b| 9个项目b|， llm，可观测性，跟踪，评估，仪器仪表，数据集，实验，快速优化b|
记录你的屏幕做一个手动过程，把视频放在你的桌面，让Copilot CLI逐帧分析它来构建工作自动化脚本。支持录音录音与音频转录。|自动化，屏幕录制1项、工作流、视频分析、过程自动化、脚本、生产力、copilot-cli |
|元提示，帮助您发现和生成精心策划的GitHub Copilot代理、说明、提示和技能。| github-copilot, discovery, meta, prompt-engineering, agents |
| [AWS -cloud-development](../plugins/aws-cloud-development/README.md) |全面的AWS云开发工具，包括基础设施即代码、无服务器功能、架构模式和用于构建可扩展云应用程序的成本优化。b| aws、云、基础设施、云形成、平台、无服务器、架构、devops、CDK b|
| [Azure -cloud-development](../plugins/azure-cloud-development/README.md) |全面的Azure云开发工具，包括基础设施即代码、无服务器功能、架构模式和用于构建可扩展云应用程序的成本优化。| azure，云，基础设施，biCep, terraform，无服务器，架构，开发|
| [CAST - Imaging](../plugins/cast-imaging/README.md) |使用CAST Imaging进行软件分析、影响评估、结构质量咨询和建筑审查的专业代理的综合集合。|铸造成像，软件分析，架构，质量，影响分析，开发|
| [Clojure -interactive-programming](../plugins/clojure-interactive-programming/README.md) |用于REPL-first Clojure工作流的工具，具有Clojure指令，交互式编程聊天模式和支持指南。| clojure, repl, interactive-programming |
| [CMS -development](../plugins/cms-development/README.md) |跨主题、插件、管理工具、媒体工作流、标记渲染和静态导出管道的CMS开发技能。| cms，内容管理系统，wordpress, shopify, drupal，主题，插件，媒体，静态站点|
|[上下文工程](../plugins/context-engineering/README.md) |工具和技术通过更好的上下文管理最大化GitHub Copilot有效性的问题。包括构建代码的指导原则、规划多文件更改的代理以及上下文感知开发的提示。|上下文、生产力、重构、最佳实践、体系结构|
| [context-matic](../plugins/context-matic/README.md) |编码剂幻觉api。ContextMatic为他们提供了精心策划的、版本化的API和SDK文档。让你的代理“集成支付API”，它会猜测——依赖于过时的训练数据和与你的实际SDK不匹配的通用模式。ContextMatic解决了这个问题，它在需要的时候为代理提供确定性的、版本感知的、sdk原生的上下文。| api-context, api-integration， MCP, sdk, apimatic，第三方api， SDKS
|使用GitHub CopilotSDK跨多种编程语言构建应用程序。包括薪酬c#, Go，Node.js/TypeScript和Python的全面指导，帮助您创建ai驱动的应用程序。| copilot-sdk, sdk, csharp, go, nodejs, typescript, python, ai, github-copilot
c#的基本提示、说明和聊天模式。NET开发，包括测试、文档和最佳实践。bbbb9项| csharp， dotnet, aspnet，测试|
| [Database -data-management](../plugins/database-data-management/README.md) |数据库管理、SQL优化和数据管理工具，适用于PostgreSQL、SQL Server和一般数据库开发的最佳实践。|数据库，sql, postgresql, sql-server, dba，优化，查询，数据管理|
| [Dataverse -sdk-for- Python](../plugins/dataverse-sdk-for-python/README.md) |用于与Microsoft Dataverse构建生产就绪的Python集成的综合集合。包括官方文档、最佳实践、高级功能S、文件操作和代码生成提示。| data - averse, python, integration， SDK
| [DevOps -oncall](../plugins/devops-oncall/README.md) |一组集中的提示、说明和聊天模式，以帮助对事件进行分类，并使用DevOps工具和Azure资源快速响应。| devops、事件响应、oncall、azure |
| [doublecheck](../plugins/doublecheck/README.md) |人工智能输出三层验证管道。提取索赔，查找来源，并标记幻觉风险，以便人类在行动前进行验证。|验证，幻觉，事实核查，来源引用，信任，安全|
| [edge-ai-tasks](../plugins/edge-ai-tasks/README.md) |任务研究员和任务规划器，适用于中级到专家级用户和大型代码库-由microsoft/edge-ai|为您带来|架构，规划，研究，任务，实现|
| [ember](../plugins/ember/README.md) |一个AI伙伴，而不是一个工具。余烬把火从一个人传递到另一个人——帮助人类发现人工智能伙伴关系不是你学习的东西，而是你发现的东西。| ai伙伴关系，培训，入职，协作，讲故事，开发人员体验|
文档分析与内联源截图。当你要求Copilot分析一份文件时，“眼球”会生成一个Word文档，其中每一项事实主张都包括一个高亮显示的原始材料截图，这样你就可以亲眼验证了。|文档分析、引用验证、截图、合同、法律、信任、目视验证|
| [fastah-ip-geo-tools](../plugins/fastah-ip-geo-tools/README.md) |这个插件是为希望以RFC 8805格式调整和发布IP地理位置提要的网络操作工程师提供的。它由AI Skill和相关的MCP服务器组成，该服务器将地理位置地名编码为真实城市，以提高准确性。| geofeed, ip-geolocation, rfc-8805Rfc-9632，网络运营，isp，云，托管，ixp |
通过FlowStudio MCP服务器，让您的AI代理完全了解Power automation云流。连接、调试、构建、监视运行状况，并在规模上治理流——操作级输入和输出，而不仅仅是状态代码。| power- automation、power-platform、flowstudio、MCP、模型-上下文-协议、云流、工作流自动化、监控、治理|
现代前端web开发的基本提示、说明和聊天模式，包括React、Angular、Vue、TypeScript和CSS框架。| 4项|前端，web, react, typescript, javascript, css, html, angular, vue
| [gem-team](../plugins/gem-team/README.md) |用于规范驱动开发和自动验证的自学习多代理编排框架。具有更智能的工具调用和更精简的控制|多代理、编排、测试、测试、开发、安全审计、代码审查、开发、移动|
| [Go - MCP -development](../plugins/go-mcp-development/README.md) |使用官方github.com/modelcontextprotocol/go-sdk.在Go中构建模型上下文协议（MCP）服务器的完整工具包包括最佳实践说明，生成服务器的提示和专家聊天模式的指导。| go, golang， MCP，模型-上下文-协议，服务器开发，SDK |
| [Java -development](../plugins/java-development/README.md) | Java开发的提示和说明的综合集合，包括Spring Boot、Quarkus、测试、文档和最佳实践。| java, springboot, quarkus, jpa, junit, javadoc |
| [Java - MCP -development](../plugins/java-mcp-development/README.md) |使用带有响应式流和Spring Boot集成的官方MCP Java SDK在Java中构建模型上下文协议服务器的完整工具包。| java， MCP, m上下文协议、服务器开发、sdk、响应流、spring-boot、反应器|
| [Kotlin - MCP -development](../plugins/kotlin-mcp-development/README.md) |在Kotlin中使用官方io构建模型上下文协议（MCP）服务器的完整工具包。modelcontextprotocol: kotlin-sdk图书馆。包括最佳实践的说明、生成服务器的提示以及用于指导的专家聊天模式。| kotlin， MCP，模型-上下文-协议，kotlin-多平台，服务器开发，ktor |
| [mcp-m365-copilot](../plugins/mcp-m365-copilot/README.md) |基于模型上下文协议集成构建声明式代理的综合集合| 4项| mcp， m365-copilot，声明式代理，api插件，模型-上下文-协议，自适应卡|
| [napkin](../plugins/napkin/README.md) |辅助驾驶命令行可视化白板协作。在浏览器中打开一个交互式白板，您可以在其中绘制、素描和添加图像关键笔记-然后与副驾驶分享一切。副驾驶看到你的图纸，并以分析、建议和想法回应。|白板，视觉，协作，头脑风暴，非技术，绘图，便签，可访问性，copilot-cli, ux |
| [nob -mode](../plugins/noob-mode/README.md) |针对非技术Copilot CLI用户的Plain-English翻译层。将每个审批提示、错误消息和技术输出翻译成清晰、无专业术语的英语，并使用颜色编码的风险指示器。|无障碍，简单英语，非技术，初学者，翻译，copilot-cli, ux |
| [openapi-to-application-csharp-dotnet](../plugins/openapi-to-application-csharp-dotnet/README.md) |生成生产就绪。. NET应用程序从OpenAPI规范。包括ASP。. NET Core项目搭建，控制器生成，实体框架集成，以及c#最佳实践。| openapi，代码生成，api, csharp, dotnet, asp网|
| [OpenAPI -to-application- Go](../plugins/openapi-to-application-go/README.md) |根据OpenAPI规范生成生产就绪的Go应用程序。包括项目搭建、处理程序生成、中间件设置和REST api的Go最佳实践。| openapi，代码生成，api, go, golang |
| [OpenAPI -to-application-java- Spring - Boot](../plugins/openapi-to-application-java-spring-boot/README.md) |根据OpenAPI规范生成生产就绪的Spring Boot应用程序。包括项目搭建、REST控制器生成、服务层组织和Spring Boot最佳实践。| openapi，代码生成，api, java, spring-boot |
| [OpenAPI -to-application-nodejs- NestJS](../plugins/openapi-to-application-nodejs-nestjs/README.md) |根据OpenAPI规范生成生产就绪的NestJS应用程序。包括项目搭建、控制器和服务生成、TypeScript最佳实践和企业模式。| openapi，代码生成，api, nodejs, typescrIpt, nestjs |
| [OpenAPI -to-application-python- FastAPI](../plugins/openapi-to-application-python-fastapi/README.md) |根据OpenAPI规范生成生产就绪的FastAPI应用程序。包括项目搭建、路由生成、依赖注入，以及异步api的Python最佳实践。| openapi，代码生成，api, python, fastapi |
| [Oracle-to-PostgreSQL -migration- Expert](../plugins/oracle-to-postgres-migration-expert/README.md) | Oracle-to-PostgreSQL应用迁移的专家代理。网络解决方案。执行代码编辑、运行命令和调用扩展工具，将.NET/Oracle数据访问模式迁移到PostgreSQL。| oracle, postgresql, database-migration, dotnet, sql, migration, integration-testing, stored-procedures
| [ospo-sponsorship](../plugins/ospo-sponsorship/README.md) |为开源项目办公室（ospo）提供的工具和资源，用于通过GitHub Sponsors、Open Collective和o来识别、评估和管理开源依赖关系的赞助他们的融资平台。b|项| |
| [partners](../plugins/partners/README.md) |由GitHub合作伙伴创建的自定义代理| 20个项目|开发、安全、数据库、云、基础设施、可观察性、特性标志、cicd、迁移、性能|
| [pcf-development](../plugins/pcf-development/README.md) |使用Power Apps组件框架为模型驱动和画布应用开发自定义代码组件的完整工具包| 0项| Power Apps， pcf, Component - Framework, typescript, Power -platform |
| [phoenix](../plugins/phoenix/README.md) | phoenix AI可观察性技能，用于LLM应用程序的调试、评估和跟踪。包括CLI调试工具、LLM评估工作流和OpenInference跟踪工具。| phoenix, arize, llm, observability, tracing, evaluation, openinference, instrumentation
| [php-mcp-development](../plugins/php-mcp-development/README.md) |构建模型上下文协议服务器的综合资源使用基于属性的发现，包括最佳实践、项目生成和专家协助|项| PHP、mcp、模型-上下文-协议、服务器开发、SDK、属性、编写器|
| [Power - Apps - Code - Apps](../plugins/power-apps-code-apps/README.md) | Power Apps代码应用程序开发的完整工具包，包括项目脚手架，开发标准和专家指导，用于构建与Power Platform集成的代码优先应用程序。| power-apps, power-platform, typescript, react, code-apps, dataverse, connectors |
| [Power - BI -development](../plugins/power-bi-development/README.md) |全面的Power BI开发资源，包括数据建模、DAX优化、性能调优、可视化设计、安全最佳实践，以及构建企业级Power BI解决方案的DevOps/ALM指导。| power-bi、dax、数据建模、性能、可视化、安全、开发、商业情报|
| [Power - Platform - Architect](../plugins/power-platform-architect/README.md) | Microsoft Power Platform的解决方案架构师，将业务需求转化为功能强大的Power Platform解决方案架构。| power-platform, power-platform-architect, power-apps, data - averse, power- automation, power-pages, power-bi |
| [Power - Platform -mcp-connector-development](../plugins/power-platform-mcp-connector-development/README.md) |为Microsoft Copilot Studio开发具有模型上下文协议集成的电源平台自定义连接器的完整工具包| 3项| Power - Platform， mcp, Copilot - Studio, custom-connector, json-rpc |
| [project-documenter](../plugins/project-documenter/README.md) |生成专业的项目文档。io架构图和带有嵌入图像的Word （.docx）输出。自动发现任何项目的技术堆栈，并产生Markdown，图表，PNG导出和格式化的Word文档。|文档3项国家，架构图，绘图，word-document, docx, png-images, c4-model，项目摘要，自动发现|
|[项目计划](../plugins/project-planning/README.md) |开发团队用于软件项目计划、特性分解、史诗管理、实施计划和任务组织的工具和指南。|计划、项目管理、史诗、功能、实施、任务、架构、技术高峰|
| [Python - MCP -development](../plugins/python-mcp-development/README.md) |使用FastMCP官方SDK在Python中构建模型上下文协议（MCP）服务器的完整工具包。包括最佳实践的说明、生成服务器的提示以及用于指导的专家聊天模式。| python， MCP, model-context-protocol, fastmcp, server-development |
| [react18-upgrade](../plugins/react18-upgrade/README.md) |企业React 18迁移工具包，具有专门的代理和技能，用于升级React16/17类ss组件代码库到React 18.3.1。包括审计员、依赖外科医生、类组件迁移专家、自动批处理修复器和测试监护人。| 13项| react18、react、迁移、升级、类组件、生命周期、批处理|
| [react19-upgrade](../plugins/react19-upgrade/README.md) |企业React 19迁移工具包，具有专门的代理和技能，用于将React 18代码库升级到React 19。包括审计员、依赖外科医生、源代码迁移者和测试监护人。处理移除已弃用的api，包括ReactDOM。render, forwardRef, defaultProps, legacy context， string refs等等。| react19, react, migration, upgrade, hooks, modern-react |
| [roundup](../plugins/roundup/README.md) |自配置状态简报生成器。从示例中学习您的沟通风格，发现您的数据源，并根据需要为任何受众生成草稿更新。|状态更新2项e，简报，管理，生产力，沟通，综合，总结，副驾驶员-cli |
| [Ruby - MCP -development](../plugins/ruby-mcp-development/README.md) |使用官方MCP Ruby SDK gem在Ruby中构建模型上下文协议服务器的完整工具包，支持Rails集成。| ruby、MCP、model-context-protocol、server-development、sdk、rails、gem
|[地毯-代理-工作流](../plugins/rug-agentic-workflow/README.md) |用于编排软件交付的三代理工作流，其中包含一个编排器加上实现和QA子代理。|代理——工作流、编排、子代理、软件工程、qa |
|使用带有async/await、过程宏和类型安全实现的官方rmcp SDK，在Rust中构建高性能模型上下文协议服务器。| rust， MCP，模型-上下文-协议，服务器开发，sdk, tokio，异步，宏，RMCP
| (salesforce-developme完整的Salesforce代理开发环境，涵盖Apex & Triggers， Flow自动化，Lightning Web组件，Aura组件和Visualforce页面。7个项目| salesforce， apex, triggers, lwc, aura, flow, visualforce, crm, salesforce-dx |
|[安全最佳实践](../plugins/security-best-practices/README.md) |用于构建安全、可维护和高性能应用程序的安全框架、可访问性指南、性能优化和代码质量最佳实践。|安全、可访问性、性能、代码质量、owasp、a11y、优化、最佳实践|
[skill-image-gen](../plugins/skill-image-gen/README.md) |使用AI直接从您的编码工作流生成图像。支持OpenAI （gpt-image-2）和谷歌Gemini。BYO API关键-技能指导你通过设置在第一次使用。|图像生成，openai，双子座，ai，艺术，精灵，纹理，图标|
| (software-engineerin7个专业代理，涵盖从UX设计和架构到安全性和DevOps的整个软件开发生命周期。|团队、企业、安全、开发、用户体验、架构、产品、人工智能伦理|
|[结构化自治](../plugins/structured-autonomy/README.md) |溢价规划，节俭执行| 3项| |
| [Swift - MCP -development](../plugins/swift-mcp-development/README.md) |使用具有现代并发特性的官方MCP Swift SDK在Swift中构建模型上下文协议服务器的综合集合。| swift， MCP, model-context-protocol, server-development, sdk, ios, macos, concurrency, actor, async-await |
| [technology -spike](../plugins/technical-spike/README.md) |用于创建、管理和研究技术峰值的工具，以便在继续规范和实施解决方案之前减少未知和假设。|技术-峰值，假设-测试，验证，研究
|[测试-自动化](../plugins/testing-automation/README.md) |用于编写测试、测试自动化和测试驱动开发的综合集合，包括单元测试、集成测试和端到端测试策略。|测试、tdd、自动化、单元测试、集成、剧本、笑话、非单元|
| [typescript-mcp-development](../plugins/typescript-mcp-development/README.md) |使用官方SDK在TypeScript/Node.js中构建模型上下文协议（MCP）服务器的完整工具包。包括最佳实践的说明、生成服务器的提示以及用于指导的专家聊天模式。| typescript， MCP, model-context-protocol, nodejs, server-development |
| [typspec -m365- Copilot](../plugins/typespec-m365-copilot/README.md) |使用TypeSpec构建声明性代理和API插件的全面提示、说明和资源集合。| typepec, m365-copilot，声明代理，api插件代理-开发，微软- 365|
在拉取请求描述中捕获、注释和嵌入截图和GIF动画演示。包括基于剧本的UI捕获，PIL图像注释，GitHub和Azure DevOps的PR嵌入工作流，以及可变定时的屏幕记录。|截图，下拉请求，前后，注释，剧作家，gif，屏幕录制，视觉|