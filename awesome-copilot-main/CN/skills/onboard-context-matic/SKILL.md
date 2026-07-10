---
name: onboard-context-matic
description: 'Interactive onboarding tour for the context-matic MCP server. Walks the user through what the server does, shows all available APIs, lets them pick one to explore, explains it in their project language, demonstrates model_search and endpoint_search live, and ends with a menu of things the user can ask the agent to do. USE FOR: first-time setup; "what can this MCP do?"; "show me the available APIs"; "onboard me"; "how do I use the context-matic server"; "give me a tour". DO NOT USE FOR: actually integrating an API end-to-end (use integrate-context-matic instead).'
---
# Onboarding: ContextMatic MCP

该技能提供了`context-matic`MCP服务器的向导式交互式导览。遵循每一个
按顺序相。在每个交互点之后停止，等待用户的回复再继续。

> **座席行为规则-贯穿整个技能：**
> - **永远不要叙述技能结构。**不要说阶段名称，步骤号，或任何
>听起来像是你在阅读说明（例如，“在第一阶段我将……”，“步骤1a：”，“根据……”）
>技能……”)。以自然对话的方式进行参观。
> - **在调用每个工具之前宣布。一个简短的句子就足够了——告诉用户
>您要查找的内容及其原因，然后调用该工具。“让我调出列表
您的项目语言的可用api。*这让用户了解和防止
>沉默的、无法解释的停顿。

---阶段0 -开始陈述和工具演练

首先用简单的语言解释一下服务器的功能。用你自己的话说
基于以下事实：

上下文化的MCP服务器解决了ai辅助编码的一个基本问题：通用
>模型是在公共代码上训练的，这些代码通常是过时的、不正确的，或者对于更新的代码来说完全缺失
> SDK版本。该服务器充当一个实时的、版本感知的接地层。而不是代理
>从训练数据中猜测SDK的使用情况，它向服务器查询“精确的”SDK模型，
>端点、验证模式和可运行代码示例，以匹配当前API版本和
>项目的编程语言。在解释了服务器解决的问题之后，按照下面的方法逐一介绍这四个工具
把他们介绍给第一次使用服务器的人。对于每个工具，说明：
- **它是什么** -给它一个难忘的一行描述
- **何时使用** -一个具体的、相关的场景
- **返回** -用户将看到的输出类型

使用以下事实作为你的来源，但要在对话中说出来——不要出示原始表格：> |工具|它做什么|什么时候使用|你得到什么|
> |---|---|---|---|
> |`fetch_api`|返回API`key`/标识符和语言的精确匹配，或列出给定语言的所有API。`key`是`fetch_api`返回的机器可读标识符（例如，`paypal`），而不是人类可读的显示名称（例如，“PayPal Server SDK”）。|“我可以使用哪些api ?”/开始一个新项目/“你有PayPal SDK吗？”|一个带有简短描述的可用API的命名列表（完整目录），或者当您提供它的identifier/key和语言|时，提供一个精确匹配的API
> |`ask`|通过版本准确的指导和代码示例回答集成问题|“我如何认证？”，“向我展示快速入门”，“做X的正确方法是什么？”逐步指导和基于实际SDK版本|的可运行代码示例
> |`model_search`|查找SDKmodel/object定义on及其类型化属性| “ Order有哪些字段？”，“这个属性是必需的吗？”模型的名称、描述和完整的类型化属性列表(必需的、可选的、嵌套的类型
> |`endpoint_search`|查找一个端点方法，它的参数，响应类型，和一个可运行的代码示例|“告诉我如何调用createOrder”，“getTrack返回什么？”|方法签名、参数类型、响应类型和可复制-粘贴的代码示例|在本节结束时，告诉用户您将演示四个核心发现和
集成工具将在参观期间上线，现在从`fetch_api`开始。弄清楚
本教程主要关注那些核心的ContextMatic服务器工具，而不是每一个可能的辅助工具
更广泛的工作流可能会使用。


---

阶段1 -显示可用的api

# # # 1。检测项目语言

在调用`fetch_api`之前，通过检查工作空间文件确定项目的主要语言：

-查找`package.json`+`.ts`/`.tsx`文件→`typescript`-查找`*.csproj`或`*.sln`→`csharp`-查找`requirements.txt`，`pyproject.toml`，或`*.py`→`python`-查找`pom.xml`或`build.gradle`→`java`-查找`go.mod`→`go`-查找`Gemfile`或`*.rb`→`ruby`-查找`composer.json`或`*.php`→`php`—如果未找到项目文件，则静默回退到`typescript`。存储检测到的语言-您将把它传递给每个后续的工具调用。

# # # 1 b。获取可用api

告诉用户您检测到的语言以及您正在获取可用的api
“我可以看到这是一个TypeScript项目。让我来取TypeScript可用的api

使用`language`=检测到的语言和`key`= “”调用**`fetch_api`**，以便工具返回可用api的完整列表。

将结果显示为格式化的列表，其中显示每个API的**名称**和一个句子摘要
其* * * *描述。不要截断或跳过任何条目。

示例显示格式（根据实际结果调整）：```
Here are the APIs currently available through this server:

1. PayPal Server SDK   — Payments, orders, subscriptions, and vault via PayPal REST APIs.
2. Spotify Web API     — Music/podcast discovery, playback control, and library management.
....
```
---

阶段2 - API选择（交互）

询问用户：

>“您想探索这些api中的哪个？”只要说出名字或电话号码就行了。”

**等待用户回复后再继续

从`fetch_api`响应中存储所选API的`key`值—您将把它传递给所有API
后续的工具调用。还要注意在解释性文本中使用的API名称。

---

阶段3 -解释所选择的API

在调用之前，可以这样说：“很好的选择——让我为你概述一下[API名称]。

调用**`ask`**
-`key`=所选API的密钥
-`language`=检测语言
-`query`=“给我一个这个API的高级概述：它做什么，主控制器或什么
模块是什么，身份验证是如何工作的，以及开始使用它的第一步是什么。以对话的方式呈现回应。亮点:
API能做什么（用例）
-身份验证如何工作（凭据，OAuth流等）
—SDK主控制器或命名空间
—待安装的NPM/pip/NuGet/etc.包名

---

阶段4 -项目语言集成（交互）

询问用户：

>“你是否想要学习如何使用[API名称]的特定部分-例如，
>创建订单、搜索音轨或管理订阅？还是我给你看
>完整的集成快速入门？

**等待用户的回复

打电话之前，你可以这样说：“好的，让我查一下。”或者“当然，让我来快速入门。调用**`ask`**
-`key`=所选API的密钥
-`language`=检测语言
-`query`=用户指定的目标，或者“向我展示一个完整的集成快速入门：安装
SDK，配置凭据，并进行第一个API调用。如果他们要完整的指南。

呈现响应，包括返回的所有代码示例。

---

##阶段5 -演示`model_search`告诉用户：

“现在让我向你展示`model_search`是如何工作的。此工具允许您查找任何SDK模型或
>对象定义——它的类型属性，是必需的还是可选的，以及它们使用的类型。
>它适用于部分、区分大小写的名称。”

在调用之前，可以这样说：“让我搜索`[model name]`模型，这样您就可以看到结果是什么样子了。从所选API（如下示例）中选择一个**代表性模型**，并使用以下方式调用**`model_search`**：
-`key`=先前选择的API密钥（例如，`paypal`或`spotify`）
-`language`=检测到的项目语言
-`query`=您选择的代表性模型名称

| API key |良好的演示查询||---|---|
|`paypal`|`Order`|
|`spotify`|`TrackObject`|

显示结果，并指出：
-准确的型号名称及其描述
-一些有趣的类型属性（突出显示可选和必需）
-任何嵌套的模型引用（例如，`PurchaseUnit[] | undefined`）

告诉用户：

你可以通过名字搜索任何模型——部分匹配也可以。试着让我查一个
当你需要知道它的形状时，从[API名称]的>特定模型。”

---

##阶段6 -演示`endpoint_search`告诉用户：

>“类似地，`endpoint_search`查找任何SDK方法-确切的参数，它们的类型，
>响应类型，以及一个完全可运行的代码示例，您可以直接放入项目中。”

在调用之前，可以这样说：“让我获取`[endpoint name]`端点，这样您就可以看到参数和活动代码示例。

为所选API选择一个具有代表性的端点，并使用显式参数对象调用**`endpoint_search`**：-`key`=您正在演示的API密钥（例如，`paypal`或`spotify`）
-`query`=要查找的端点/ SDK方法名称（例如，`createOrder`或`getTrack`）
-`language`=用户的项目语言（例如，`"typescript"`或`"python"`）

例如:

| API密钥（`key`） |端点名称（`query`） |示例`language`||---|---|---|
|`paypal`|`createOrder`|用户的项目语言|
|`spotify`|`getTrack`|用户的项目语言|
显示结果，并指出：
—方法名称和描述
—请求参数及其类型
-响应类型
-完整的代码示例（与返回的完全相同）

告诉用户：

请注意，代码样例已经可以使用了——它从正确的SDK导入，初始化
>客户端，调用端点，并处理错误。您可以通过它来搜索任何端点
>方法名称或部分区分大小写的片段。

---

阶段7 -收尾：你可以问什么

以用户现在可以要求代理做的事情的摘要列表结束旅程。表示为
格式化的菜单：

---

你能用这个MCP做什么

快速入门：您的第一个API调用**```
/integrate-context-matic Set up the Spotify TypeScript SDK and fetch my top 5 tracks.
Show me the complete client initialization and the API call.
```
```
/integrate-context-matic How do I authenticate with the Twilio API and send an SMS?
Give me the full PHP setup including the SDK client and the send call.
```
```
/integrate-context-matic Walk me through initializing the Slack API client in a Python script and posting a message to a channel.
```
* * * *特定于框架的集成```
/integrate-context-matic I'm building a Next.js app. Integrate the Google Maps Places API
to search for nearby restaurants and display them on a page. Use the TypeScript SDK.
```
```
/integrate-context-matic I'm using Laravel. Show me how to send a Twilio SMS when a user
registers. Include the PHP SDK setup, client initialization, and the controller code.
```
```
/integrate-context-matic I have an ASP.NET Core app. Add Twilio webhook handling so I can receive delivery status callbacks when an SMS is sent.
```
**链接工具完全集成**```
/integrate-context-matic I want to add real-time order shipping notifications to my
Next.js store. Use Twilio to send an SMS when the order status changes to "shipped". Show me
the full integration: SDK setup, the correct endpoint and its parameters, and the TypeScript code.
```
```
/integrate-context-matic I need to post a Slack message every time a Spotify track changes
in my playlist monitoring app. Walk me through integrating both APIs in TypeScript — start by
discovering what's available, then show me the auth setup and the exact API calls.
```
```
/integrate-context-matic In my ASP.NET Core app, I want to geocode user addresses using
Google Maps and cache the results. Look up the geocode endpoint and response model, then
generate the C# code including error handling.
```
**调试和错误处理**```
/integrate-context-matic My Spotify API call is returning 401. What OAuth flow should I
be using and how does the TypeScript SDK handle token refresh automatically?
```
```
/integrate-context-matic My Slack message posts are failing intermittently with rate limit
errors. How does the Python SDK expose rate limit information and what's the recommended retry
pattern?
```
---

“这就是我们的旅行！”问我以上任何一个问题，或者只是告诉我你想建立什么——我会的
>使用此服务器为您提供准确的，特定于版本的指导。

---

##代理注意事项

-如果用户选择的API不在`fetch_api`结果中，告诉他们当前不是
如果有空位，我们可以继续旅行。
-此技能中的所有工具调用都是只读的-它们不会修改项目，安装包，
或者编写文件，除非用户明确要求您继续进行集成。
-当显示来自`endpoint_search`或`ask`的代码示例时，将它们呈现在围栏代码块中
使用正确的语言标签。