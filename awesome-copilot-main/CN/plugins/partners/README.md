# Partners插件

GitHub合作伙伴创建的自定义代理

# #安装```bash
# Using Copilot CLI
copilot plugin install partners@awesome-copilot
```
包含的内容

# # #代理

|代理|描述||-------|-------------|
这个定制代理使用振幅的MCP工具在振幅内部部署新的实验，实现无缝的变体测试能力和产品特性的推出。|
|`apify-integration-expert`|专家代理，用于将Apify Actors集成到代码库。处理Actor选择、工作流设计、跨JavaScript/TypeScript和Python的实现、测试和生产就绪部署。|
Arm云迁移助手加速将x86工作负载迁移到Arm基础架构。它扫描存储库中的架构假设、可移植性问题、容器基础映像和依赖不兼容性，并推荐arm优化的更改。它可以驱动多arch容器构建，验证性能并指导优化，从而直接在GitHub中实现平滑的跨平台部署。|
|`diffblue-cover`|专家代理，用于使用Diffblue Cover为java应用程序创建单元测试。||`droid`|提供Droid CLI的安装指导、使用示例和自动化模式，重点介绍用于CI/CD的Droid exec和非交互式自动化|
Dynatrace Expert Agent将可观察性和安全功能直接集成到GitHub工作流程中，使开发团队能够通过自主分析跟踪、日志和Dynatrace发现来调查事件、验证部署、分类错误、检测性能退化、验证发布和管理安全漏洞。这支持直接在存储库中对已确定的问题进行有针对性和精确的修复。|
我们的专家AI助手调试代码（O11y），优化向量搜索（RAG），并使用实时弹性数据修复安全威胁。|
|`jfrog-sec`|用于自动安全修复的专用应用程序安全代理。验证方案和版本遵从性，并建议使用JFrog安全智能修复漏洞。|
一个专门的GitHub Copilot代理，使用LaunchDarkly MCP服务器安全地自动执行功能标志清理工作流程。该代理确定删除准备情况，识别正确的转发值，并创建pr，在删除过时标志和更新过时默认值的同时保留生产行为。|
擅长使用系统的、清单驱动的方法在web应用程序中实现国际化。|
从Monday.com平台数据中丰富任务上下文的精英bug修复代理。收集相关的项目、文档、评论、史诗和需求，以交付具有全面pr的产品质量修复。|
|`mongodb-performance-advisor`|分析MongoDB数据库性能，提供查询和索引优化见解，并提供可操作的建议S来提高数据库的整体使用率。|
|`neo4j-docker-client-generator`| AI代理，从GitHub生成简单，高质量的Python Neo4j客户端库，具有适当的最佳实践|
使用Neon的分支工作流程进行零停机的安全Postgres迁移。在独立的数据库分支中测试模式更改，彻底验证，然后应用到生产中——所有这些都是通过支持Prisma、Drizzle或您喜欢的ORM而自动化的。|
使用Neon的分支工作流自动识别和修复缓慢的Postgres查询。分析执行计划，在独立的数据库分支中测试优化，并提供清晰的before/after性能指标和可操作的代码修复。|
|`octopus-deploy-release-notes-mcp`|在Octopus Deploy中为某个版本生成发布说明。该MCP服务器的工具提供对Octopus Deploy api的访问。|
|`stackhawk-security-onboarding`|自动设置StackHawk安全测试对于生成配置和GitHub Actions工作流的存储库|
拥有自动化HCP Terraform工作流程的Terraform基础设施专家。利用Terraform MCP服务器进行注册中心集成、工作空间管理和运行编排。使用最新的provider/module版本生成兼容的代码，管理私有注册中心，自动化变量集，并通过适当的验证和安全实践协调基础设施部署。|
|`pagerduty-incident-responder`|通过分析事件上下文，识别最近的代码更改，并通过GitHub pr建议修复来响应PagerDuty事件。|
|`comet-opik`|统一Comet Opik代理，用于检测LLM应用程序，管理prompts/projects，审计提示，并通过最新的Opik MCP服务器调查traces/metrics。|# #源

这个插件是[Awesome Copilot]（https://github.com/github/awesome-copilot）的一部分，这是一个社区驱动的GitHub Copilot扩展集合。

# #许可证

麻省理工学院