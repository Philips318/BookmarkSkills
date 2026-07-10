---
name: x-twitter-scraper
description: 'Build GitHub Copilot workflows with Xquik X API SDKs, REST endpoints, MCP tools, TweetClaw OpenClaw plugin installs, signed webhooks, tweet search, user lookup, follower exports, media actions, and agent automation.'
---
# X推特刮板

当用户想要将Xquik集成到应用程序、脚本、数据管道或用于X API和Twitter scraper任务的AI代理工作流时，使用此技能。

##用例

—搜索推文，获取推文详细信息，读取时间线，下载媒体。
—查找用户、查看关系、导出关注者或关注者。
-开始提取回复，转发，引用，喜欢，列表，社区，文章和搜索结果的工作。
—创建帐户监视器并验证hmac签名的webhook事件。
-添加TypeScript， Python, Go, Java, Kotlin, c#, Ruby， PHP， CLI或Terraform客户端。
—通过Xquik MCP服务器连接代理运行时。
-安装TweetClaw时，工作流属于OpenClaw内部，需要插件管理的批准X帐户的行动。

##源检查

在编写代码之前，检查当前的Xquik源代码：—REST接口文档：https://docs.xquik.com/api-reference/overview—SDK索引：https://docs.xquik.com/sdks—OpenAPI规范：https://xquik.com/openapi.json—MCP服务器文档：https://docs.xquik.com/mcp/overview—技能库：https://github.com/Xquik-dev/x-twitter-scraper- TweetClaw OpenClaw插件：https://github.com/Xquik-dev/tweetclaw- TweetClaw npm注册表元数据：https://registry.npmjs.org/@xquik%2Ftweetclaw不要发明端点名称、请求字段、响应字段、作用域、定价、限制或包名称。请先阅读相关SDK README和API参考页面。

##实现流程1. 确定工作流程：搜索、查找、提取、监控、webhook、媒体、写入操作、计费或MCP。
2. 选择集成面：为应用程序代码生成SDK，为自定义客户端提供REST，为代理提供MCP，为OpenClaw插件工作流提供TweetClaw，或为事件交付提供webhook。
3. 确认文档中的身份验证需求，并为API密钥使用环境变量。
4. 当存在针对用户语言的SDK时，使用类型化请求和响应模型。
5. 根据SDK或API文档添加重试和分页。
6. 在写操作、支付流或长时间运行的监视之前添加显式的用户确认。
7. 将webhook验证保持在服务器端，并在处理事件之前比较HMAC签名。
8. 将结构化数据返回给调用者，而不是抓取生成的UI输出。

## SDK模式当涉及到应用程序代码时，将SDK与用户的项目语言进行匹配：

-检查项目文件和包清单以确定语言和框架。
—打开SDK索引，在选择安装命令、包名、导入或客户端方法之前，先阅读相应的SDK README。
—如果检测到的语言有官方SDK，则优先使用官方SDK。
—仅当项目语言没有合适的官方SDK或用户要求自定义客户端时使用REST。
-将API密钥保存在环境变量或项目现有的秘密管理器中。

使用项目原生类型的请求和响应模型。将网络调用保留在服务器端代码中，除非SDK文档明确支持浏览器使用。

Webhook模式

添加webhook处理程序时：-阅读文档签名头名称和有效载荷格式。
—解析业务逻辑前需要验证HMAC签名。
—拒绝签名缺失、畸形、不匹配。
-使处理程序幂等，因为webhook传递可以重试。
-仅存储产品工作流所需的字段。

MCP模式

当用户希望代理直接探索或调用Xquik工具时，使用MCP服务器。当应用程序需要稳定类型的契约、测试或内部抽象时，将应用程序代码保留在REST或SDK客户端上。

OpenClaw插件模式

当用户在OpenClaw中工作，想要安装插件元数据，或者需要一个批准审查的路径来更改帐户X操作时，使用TweetClaw。当项目需要类型化契约、服务器端抽象或OpenClaw之外的长期后端作业时，将应用程序服务保持在REST或SDK客户端上。在建议安装命令或工具名称之前，请阅读TweetClaw README和包元数据。不要假设发布的npm版本与源代码HEAD匹配。

将创建、回复、引用、点赞、书签、转发、关注、删除、媒体和监控等操作视为值得批准的操作，除非当前的TweetClaw文档规定了更严格的政策。保持只读tweet搜索、回复搜索、个人资料查找、关注者导出和证据收集的低风险，同时仍然尊重速率限制和帐户授权。

安全性和准确性保持语言的中性和技术性。
—说明Xquik是第三方X数据和自动化API。
-切勿声称与X公司有关联。
—不要绕过访问控制或平台策略。
-不要暴露API密钥，webhook秘密，帐户cookie，令牌或原始签名。
不要在示例或测试中硬编码凭证。
-不记录私人基础设施的细节。
-首选官方Xquik文档、SDK readme和OpenAPI规范，而不是内存。