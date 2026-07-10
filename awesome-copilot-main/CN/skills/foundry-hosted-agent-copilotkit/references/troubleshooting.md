#故障排除：症状→根本原因→修复

所有条目都是经过实际验证的故障模式，具有确切的签名。特定于hitl的故障在hitl.md；这个文件逐层覆盖了其他所有内容。

##调试方法

在接触代码之前，在尽可能低的层进行复制：

1.`curl -N -X POST <agui-endpoint> -H 'Content-Type: application/json' -d '<minimal RunAgentInput>'`-读取原始AG-UI SSE事件。复制→前端无辜。
2. 对于托管代理，直接调用裸`/responses`（或`/invocations`）端点。复制→AG-UI适配器和CopilotKit无辜；错误在framework/hosting层。这种技术就是隔离重复执行错误（hitl.md）的方法。
3. 在两次测试之间重新启动本地运行的代理——从先前的测试进入内存状态使结果在两个方向上都存在。

## CopilotKit runtime / frontend

| |根本原因|修复|| --- | --- | --- |
|运行时`agents`密钥，`<CopilotKit agent>`prop和托管的`agent.yaml`名称之间的名称漂移；或者运行时配置中的single-endpoint/multi-endpoint路由不匹配|使用一个共享常量作为代理名；根据已安装版本的docs |检查运行时的端点模式选项
|请求运行时子路由（例如线程）404/405|路由处理程序注册在一个固定的路径，但运行时版本期望一个捕获所有路由服务于多个子路径|使用一个可选的捕获所有路由段（`[[...slug]]`在Next.js应用路由器），并导出处理程序支持的所有HTTP方法|
|`next build`类型错误：`HttpAgent`缺少属性（例如`pendingInterrupts`） |安装的`@ag-ui/client`版本不同于`@copilotkit/runtime`是针对| Pin`@ag-ui/client`安装的`@copilotkit/runtime`依赖的版本（检查其package.json） |
|控制台：“在‘窗口’上执行‘fetch’失败：非法调用”；ag)一个库捕获`fetch`作为一个裸引用，并用错误的`this`调用它（与CopilotKit v2线程存储+`@ag-ui/client``HttpAgent`看到）|在任何模块加载之前绑定获取，例如在根布局`<head>`:`if(!window.fetch.__bound){var f=window.fetch.bind(window);f.__bound=true;window.fetch=f;}`|中的内联脚本
|已知转发回归：注册的前端工具不包括在`RunAgentInput.tools`(CopilotKit/CopilotKit#5813, 1.62。x era) |升级到修复后；在任何CopilotKit升级后，重新测试前端工具的可见性|
|事件顺序错误在`RUN_ERROR`之后附加`TEXT_MESSAGE_END`(CopilotKit/CopilotKit#5812) |跟踪修复版本；避免依赖于错误后事件|
|Tool/approval卡在运行结束时消失|`MESSAGES_SNAPSHOT`在运行结束时表示回合不同于实时事件（例如多个工具调用合并为一个消息；UI只呈现第一个）|修复快照镜头构建（每个辅助消息一个工具调用）或升级UI层；总是验证运行后的DOM |
| CopilotKit在小版本之间移动API；根据已安装包中绑定的`.d.ts`文件（而不是docs或内存|）验证名称AG-UI /适配器层

| |根本原因|修复|| --- | --- | --- |
|原始AG-UI消息历史重播到管理其自己的历史的响应端点|派生每个回合的输入（最新的用户消息或批准决定）；永远不要重播完整的记录b|
| UI显示了在长时间运行的静默工具运行过程中的500，|Proxy/gateway丢弃了空闲的SSE连接|每隔~10秒从AG-UI端点|发出SSE keep-alive注释（`: ping`）
|`useCoAgent().state`总是空|代理上没有配置状态模式，没有工具写入状态键-或者没有状态合成的架构C（参见patterns.md） |配置状态模式+确保工具写入它；在响应桥上，确认状态合成存在于所有|

##铸造连接/认证

| |根本原因|修复|| --- | --- | --- |
|请求的令牌默认范围为`cognitiveservices.azure.com`|请求范围为`https://ai.azure.com/.default`|
尽管已登录并具有|`az`角色，但CLI的活动subscription/tenant与Foundry项目的（多租户帐户）不同。错误租户下的角色查找甚至无法解析受让人，模仿丢失的RBAC |将`az account show`与项目的tenant/subscription进行比较；`az account set --subscription <correct>`或`az login --tenant <correct>`。零代码更改-不要误认为是包回归|
|客户端发送`x-ms-user-isolation-key`；已部署代理使用entra派生的隔离|删除已部署代理|的标头
|异步`DefaultAzureCredential`桥接失败|缺少异步传输|`pip install aiohttp`|
|对新启动的本地代理的第一个请求404`DeploymentNotFound`尽管存在模型部署|托管运行时中的预热片|重试一次或使用相同的en重新启动V等于|
|新建`azd ai agent run`失败“地址已在使用”（混淆hypercorn回溯）|过期的本地主机代理进程占用8088端口|`ss -ltnp | grep 8088`，杀死过期进程，重试|Python依赖陷阱

| |根本原因|修复|| --- | --- | --- |
|依赖于`agent-framework-core`元包，它会拖放可选的附加组件|依赖于`agent-framework-core`，只依赖于你使用的特定附加组件（例如`agent-framework-foundry`，`agent-framework-ag-ui`） |
|`agent_framework_foundry_hosting`从`mcp`导入，但在远程构建中不传递。|添加一个显式的`mcp`pin到托管需求|
|`httpx`api丢失（`AsyncClient`消失）|安装与预发布的分辨率拉了一个httpx 1.0开发构建| Pin httpx到当前的稳定线|
托管代理快速失败：`RuntimeError: the hosted environment is running on protocol 1.0.0, but the agent requires protocol 2.0.0`|托管包的响应协议版本与`agent.yaml`/`agent.manifest.yaml`|中声明的`version:`不一致
当通过Foundry代理客户端调用时，Python`@tool`“没有在Foundry中运行”|客户端工具可调用程序执行客户端设计的；只有foundry原生工具在该路径上运行服务器端|预期行为-如果工具必须在那里执行，则托管代理（运行循环服务器端）|