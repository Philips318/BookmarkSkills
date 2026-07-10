---
name: msgraph-sdk
description: 'Integrate Microsoft Graph SDK into any project — .NET, TypeScript/JavaScript, or Python. Covers auth patterns (client credentials, OBO, managed identity), SDK setup, calling Graph APIs, batching, delta queries, change notifications, throttling, and permission scopes. Use when accessing Microsoft 365 data (users, mail, calendar, Teams, files, SharePoint) from any application type.'
---
# Microsoft Graph SDK

在将Microsoft Graph集成到应用程序中以访问Microsoft 365数据和服务时使用此技能。

始终在当前的Microsoft Graph SDK文档和目标语言的SDK版本中进行基础实现，而不是单独依赖内存。

首先确定目标语言1. 使用**。当项目包含`.cs`，`.csproj`，或`.sln`文件时，或当用户要求c#指导时。NET**工作流。遵循[references/dotnet.md] (references/dotnet.md)。
2. 当项目包含`package.json`、`.ts`或`.js`文件，或者当用户要求Node.js/浏览器指导时，使用**TypeScript / JavaScript**工作流。遵循[references/typescript.md] (references/typescript.md)。
3. 当项目包含`.py`、`pyproject.toml`或`requirements.txt`，或者当用户要求Python指导时，使用**Python**工作流。遵循[references/python.md] (references/python.md)。
4. 如果存在多种语言，则匹配正在编辑的文件的语言或询问用户。

总是查阅实时文档

-微软图形概述：<https://learn.microsoft.com/graph/overview>-图形资源管理器（尝试调用live）：<https://developer.microsoft.com/graph/graph-explorer>—图形权限参考：<https://learn.microsoft.com/graph/permissions-reference>-使用Microsoft Docs MCP工具获取当前API形状和SDK示例。##认证-选择正确的模式

选择错误的认证流是最常见的图集成错误。在编写任何验证代码之前应用此决策树：

|场景|使用|的流程|---|---|
|后台服务/守护进程，没有用户| **客户端凭据**（仅应用程序）|
|代表已登录用户的代理或API | ** on - behalf - of (OBO)** |
|在Azure中运行的应用程序（功能，容器应用程序，虚拟机）| **托管身份**（优先于秘密）|
|命令行工具或本地开发脚本| **设备代码**或**交互浏览器** |
|单页应用（仅限浏览器）| **授权码+ PKCE** |

-当需要用户上下文时，永远不要使用客户端凭据- Graph在权限级别强制执行这一点（应用程序与委托）。
-在azure托管应用程序中首选`DefaultAzureCredential`；它首先尝试托管身份，然后优雅地退回到本地开发。
-永远不要硬编码秘密。使用环境变量、Azure密钥库或秘密管理器。

核心SDK使用模式

构建客户端

总是构造一次`GraphServiceClient`并重用它（它在内部管理令牌缓存）。从Azure身份库传递凭证-永远不要手动构建原始HTTP客户端。

###打电话

-使用fluent builder API:`client.Users[userId].Messages.GetAsync(...)`。
—总是`await`异步调用。
-指定`$select`来限制返回的字段- Graph返回大的默认有效负载。
—在服务器端使用`$filter`，而不是在内存中过滤返回的集合。
—当关系较小时，使用`$expand`在单个调用中获取相关资源。

# # #分页

图形分页集合。永远不要假设所有的邮件都是一个回复：
-检查响应上的`@odata.nextLink`。
-使用SDK的`PageIterator`助手（在所有三个SDK中都可用）自动遍历页面。
—设置`$top`来控制页面大小（最大值因资源而异，通常为999）。

##高级模式

批处理请求使用`$batch`端点将最多20个独立的Graph调用合并为单个HTTP请求。使用批处理时：
—初始化需要多个资源的仪表板或代理的数据。
—减少高调用计数操作的延迟。

批响应是乱序到达的——通过分配给每个请求的`id`字段来匹配它们。

增量查询

使用增量查询来增量同步更改，而不是轮询完整集合：
-第一次调用：`GET /users/delta`返回所有项+一个`@odata.deltaLink`。
-后续调用：使用`deltaLink`只接收自上次同步以来更改的内容。
-支持的：用户，组，消息，日历事件，团队频道，和更多。
—在两次同步之间持久地存储`deltaLink`（数据库，blob）。

更改通知（webhooks）使用`POST /subscriptions`订阅资源更改：
- Graph提供更改事件到您的HTTPS通知URL。
-订阅到期-在`expirationDateTime`之前更新它们（最长时间因资源而异；mail/calendar通常为1-3天，users/groups最多为4230分钟）。
—验证订阅握手：Graph在创建时发送一个`validationToken`查询参数-使用HTTP 200以纯文本形式回显它。
-使用生命周期通知（`notificationUrl`+`lifecycleNotificationUrl`）来处理错过的事件和重新授权。
-对于大容量场景，更喜欢带有资源数据的更改通知（需要额外的加密设置）。

# # #节流图猛油门。总是处理HTTP 429：
-读取`Retry-After`头-它指定等待的确切秒数，而不是固定的后退。
- SDK内置的重试中间件在配置时自动处理429；显式地启用它。
-避免扇形模式击中图形与数百个并行请求；使用批处理或排队代替。

# #权限

在编写验证代码之前获得正确的权限——错误的作用域会导致403错误，以后很难调试。—应用程序权限不需要用户（daemon / service）运行。需要管理员同意。
—授权权限在已登录用户的上下文中运行。有些需要管理员同意。
-请求**所需的**最低权限。Graph的权限引用列出了每个操作的最小权限选项。
-在编码之前，使用图形资源管理器测试调用实际需要的权限。
-在Azure应用程序注册中：授予API权限→Microsoft Graph→选择类型（应用程序或委托）→在需要时授予管理员许可。

##通用图形资源-快速参考

|目标|资源路径||---|---|
获取登录用户的配置文件|`GET /me`|
|用户邮箱消息列表|`GET /me/messages`|
|发送邮件|`POST /me/sendMail`|
|列出日历事件|`GET /me/events`|
|获取用户的OneDrive根|`GET /me/drive/root/children`|
| List用户在|`GET /me/joinedTeams`|中的Teams
|发布一个Teams频道消息|`POST /teams/{id}/channels/{id}/messages`|
|列出SharePoint站点列表|`GET /sites/{siteId}/lists`|
|搜索M365 |`POST /search/query`|
|列出租户（app-only） |`GET /users`|中的所有用户
|获取组成员|`GET /groups/{id}/members`|

以类似的方式，使用SDK的流畅API在代码中导航到这些资源。

# #工作流程1. 确定目标语言并读取匹配的参考文件。
2. 识别真实场景，并从上表中选择正确的流。
3. 在做出实现选择之前，获取当前SDK文档和图形资源管理器示例。
4. 应用最低权限权限-在图权限参考中确认。
5. 从一开始就实现分页——不要假设单页面响应。
6. 从第一天起启用重试中间件进行节流。
7. 对于同步场景，更倾向于增量查询而不是轮询。
8. 使用来自所选参考文件的特定于语言的包名、验证提供程序设置和代码模式。

##完成标准-验证流匹配场景（不默认为用户上下文调用的客户端凭据）。
-`GraphServiceClient`构造一次并重用。
-所有集合读取处理分页。
节流（429）通过重试中间件或显式`Retry-After`逻辑处理。
—权限范围限制到所需的最低限度。
-没有秘密或凭证是硬编码的。
—代码匹配所选语言的当前SDK版本模式。