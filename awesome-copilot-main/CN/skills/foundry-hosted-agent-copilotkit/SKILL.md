---
name: foundry-hosted-agent-copilotkit
description: 'Ongoing development guidance for agentic web apps that pair a CopilotKit frontend with Microsoft Agent Framework agents on Azure AI Foundry hosted agents over the AG-UI protocol - add and gate agent tools, wire human-in-the-loop approvals, build generative UI and shared state, debug the event stream, upgrade pre-1.0 packages safely, and deploy hosted agent updates.'
---
使用CopilotKit + AG-UI + Azure AI Foundry托管代理进行开发

将此技能用于构建在此堆栈上的现有应用程序中的开发工作：使用CopilotKit的React/Next.js前端，通过ags - ui协议连接到Microsoft代理框架（MAF）代理(Python或。. NET)作为Azure AI Foundry托管代理（付费Azure服务；使用可能会产生成本）运行或正在针对其开发。

不要用这个技能来支撑一个新项目。存在专用支架（CopilotKit CLI,`azd ai agent init`）；使用这些工具，然后返回这里进行以下所有工作：添加工具、将它们置于审批之后、生成UI、共享状态、调试、依赖项升级和部署代理更新。

##心理模型```text
CopilotKit hooks (React)            useFrontendTool / useHumanInTheLoop /
        │                           useRenderToolCall / useCoAgent
        ▼
CopilotKit Runtime (route handler)  agents: { <name>: new HttpAgent({ url }) }
        │  AG-UI events over SSE
        ▼
AG-UI endpoint                      ← WHERE this lives defines your architecture
        │
        ▼
MAF Agent (tools, approval modes)   → model deployment
```
最重要的一个事实是：在默认情况下，已部署的Foundry托管代理端点不会使用AG-UI。**它暴露了OpenAI响应端点（`.../protocols/openai/responses`）and/or一个原始的`.../protocols/invocations`端点。AG-UI必须在某个地方生成，而它的生成位置决定了每个功能（尤其是human-in- loop）的行为。这三种接线在[references/architecture.md]（references/architecture.md）中描述。

# #工作流程

对于该堆栈上的每个任务，请遵循以下步骤：1. **先确认接线。**在更改任何内容之前检查代码库：
-`add_agent_framework_fastapi_endpoint(...)`（Python）或`MapAGUI(...)`（. net）包装进程内代理→体系结构A（进程内AG-UI端点）。
-一个托管代理，它自己的容器服务AG-UI，在`agent.yaml`→架构B中使用`protocol: invocations`声明。
-在AG-UI端点和托管代理的`/responses`端点之间进行单独的服务转换（在代码中查找`previous_response_id`、`mcp_approval_response`或Foundry`conversation`对象）→架构C（转换桥）。
—确认前端代理名称：运行时`agents`配置中的关键字、`<CopilotKit>`提供程序上的`agent`prop和`agent.yaml`中的托管代理名称必须一致。
2. **现场文件接地。**这里的每个层都是1.0之前或预览版，并在次要版本之间移动。永远不要相信记住的api：
- MAF和Foundry托管代理：使用Microsoft Docs MCP工具时可用，否则learn.microsoft.com （`/agent-framework/integrations/ag-ui/`,`/azure/foundry/`）。
- CopilotKit: docs.copilotkit.ai （Microsoft Agent Framework部分）。根据已安装的`@copilotkit/*`包中绑定的TypeScript声明来验证钩子和运行时API名称——名称已经发生了变化（`useCopilotAction`是遗留的；目前的名称包括`useFrontendTool`、`useHumanInTheLoop`、`useRenderToolCall`、`useCoAgent`）。
- AG-UI协议：docs.ag-ui.com（事件参考，dojo模式）。
3. **使用下面的匹配引用执行任务**。
4. * *验证敌对的。**一个编译构建，一个启动的开发服务器，或者一个成功的聊天回复都不能证明。在本技能结束时应用完成标准。# #引用

按需负荷；每个都是独立的：

|参考| |时加载| --- | --- |
| [references/architecture.md](references/architecture.md) |接线选择或理解；local-vs-deployed模式;为什么存在翻译桥？它必须处理什么
| [references/patterns.md](references/patterns.md) |实现7种AG-UI交互模式中的任意一种（前端工具、后端工具渲染、HITL、生成式UI、共享状态、预测状态）|
| [references/hitl.md](references/hitl.md) |添加或调试人在循环中的批准，包括已知的重复执行危险|
| [references/troubleshooting.md](references/troubleshooting.md) |任何故障：症状→根本原因→修复每层表|
| [references/upgrading.md](references/upgrading.md) |碰撞任何依赖；版本兼容性规则；跟踪上游问题|
| [references/deploy-loop.md](references/deploy-loop.md) |使用`azd ai agent run`在本地运行代理，部署更新，部署问题|

任务剧本

添加或修改代理工具1. 在代理上定义工具（Python中的`@tool`；`AIFunctionFactory.Create`）。. NET)，带有类型化的描述参数。
2. 保持文档字符串接地安全：不要在模型必须从实际数据派生的字段的参数描述中放置具体的示例值-模型复制文字示例。在工具中使用占位符并进行验证。
3. 返回紧凑的、模型可消耗的值；丰富的格式属于UI呈现，而不是工具结果。
4. 现确定审批方式：副作用工具得`approval_mode="always_require"`（见[references/hitl.md](references/hitl.md)）；只读工具保持不受限制。
5. 如果工具调用应该在UI中呈现，那么为它添加一个`useRenderToolCall`/呈现条目（[references/patterns.md](references/patterns.md)）。
6. 现场验证：通过聊天UI触发工具，确认调用和结果流为`TOOL_CALL_*`事件，并确认重命名或重新键入的参数没有破坏解析参数的任何前端组件。将human-in-the-loop连接到现有工具上

跟着[references/hitl.md]（references/hitl.md）从头到尾。总结：标记工具（`approval_mode="always_require"`/`ApprovalRequiredAIFunction`），在AG-UI包装器上启用确认，在前端注册审批UI挂钩，并使响应有效负载形状与服务器检测期望的匹配。然后测试批准和拒绝，并在批准后进行后续操作（参见重复执行的危险）。

构建生成式UI或共享状态

遵循[references/patterns.md]中的模式表（references/patterns.md）。了解诚实警告：当AG-UI适配器包装进程内代理（ArchitectureA/B）时，状态同步模式是本机的；通过响应协议桥接（架构C），它们需要明确的综合工作——在承诺特性之前检查代码库实际实现了什么。

调试出错的流程1. 首先在最低层复制：`curl -N`AG-UI端点，使用最小的`RunAgentInput`JSON主体，并读取原始SSE事件。如果错误在那里复制，前端是无辜的。
2. 对于托管代理，再低一层：直接调用代理的`/responses`端点。这就是为什么已知的重新执行错误被隔离到框架而不是UI堆栈。
3. 将症状与[references/troubleshooting.md]（references/troubleshooting.md）进行匹配—列出精确的错误字符串。
4. 如果代理保持内存状态，则在验证通过之间重新启动本地运行的托管代理（`azd ai agent run`）；陈旧状态使测试通过或由于错误的原因而失败。

升级依赖项遵循[references/upgrading.md] (references/upgrading.md)。永远不要孤立地碰撞单个包：那里的版本关系规则（运行时↔AG-UI客户端、代理-框架线路一致性、托管协议↔清单版本）必须同时有效，并且在移除之前，必须针对其跟踪的上游问题重新验证任何本地解决方案。

部署代理更新

遵循[references/deploy-loop.md](references/deploy-loop.md)：使用`azd ai agent run`对实际代理进行本地迭代，然后使用`azd deploy`（每次部署都会创建一个新的代理版本），然后在声明成功之前验证已部署的代理——包括批准暂停。

##完成标准

只有当所有这些条件都满足时，才会对堆栈进行更改：1.read/query路径通过真正的UI工作（不只是通过curl）。
2. 每个经过批准的工具都经过两种方式的测试：批准→工具执行服务器端并状态可见的更改；拒绝→工具不运行，代理确认。
3. 在批准后，在同一线程中发送了至少一个后续回合，并且闸门工具没有再次静默执行（[references/hitl.md](references/hitl.md)，重复执行的危险）。
4. 工具调用在流端正确呈现，而不仅仅是在流期间（消息快照可能与实时事件不同）。
5. 对于已部署的更改：上面的检查是针对已部署的端点运行的，而不仅仅是在本地运行——部署成功并不是行为的证明。