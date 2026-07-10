# Architecture：生成AG-UI的地方

该堆栈有三层必须一致：CopilotKit (React hooks + runtime)， AG-UI协议（SSE事件流）和Microsoft代理框架（MAF）代理，可选地作为Azure AI Foundry托管代理运行。部署的Foundry托管代理端点使用OpenAI响应（`{project_endpoint}/agents/{name}/endpoint/protocols/openai/responses`）或原始调用协议(`.../protocols/invocations`) - **而不是AG-UI**。从`@ag-ui/client`直接将`HttpAgent`指向托管代理的响应端点不起作用。

有三种可行的线路。在更改任何内容之前，确定代码库使用的是哪一个。

A -进程内AG-UI端点（代理运行在您的服务内部）

代理对象位于与AG-UI HTTP端点相同的进程中。

Python:```python
from agent_framework import Agent
from agent_framework_ag_ui import AgentFrameworkAgent, add_agent_framework_fastapi_endpoint
from fastapi import FastAPI

agent = Agent(name="assistant", instructions="...", client=chat_client, tools=[...])
wrapped = AgentFrameworkAgent(agent=agent, require_confirmation=True)  # HITL on
app = FastAPI()
add_agent_framework_fastapi_endpoint(app, wrapped, "/")
```
．. NET:`builder.Services.AddAGUI()`+`app.MapAGUI("/", agent)`from`Microsoft.Agents.AI.Hosting.AGUI.AspNetCore`，带有审批中间件（参见hitl.md）。

所有7种AG-UI模式（包括状态snapshots/deltas）都是本地工作的——适配器看到代理的内部事件。
模型客户端仍然可以是Foundry模型部署“进程内”指的是*代理循环*运行的位置，而不是模型。
-这就是CopilotKit CLI和MAF示例所产生的，当您不需要平台管理对话，每用户隔离或foundry管理计算时，这是正确的选择。

架构B -托管代理服务AG-UI本身（调用协议）

托管代理自己的容器使用AG-UI，部署在Foundry的`invocations`协议下（根据Foundry托管代理文档“自定义流协议（AG-UI等）→调用”）。`agent.yaml`声明：```yaml
protocols:
  - protocol: invocations
    version: 2.0.0
```
容器在`/invocations`提供AG-UI请求（微软的`foundry-samples`存储库在`samples/python/hosted-agents/bring-your-own/invocations/ag-ui/`下有一个自带调用AG-UI示例）。CopilotKit运行时的`HttpAgent`指向部署的调用端点。

AG-UI特性的行为类似于架构A，因为适配器仍然封装了进程内代理——它只是在foundry管理的计算中运行。
-你放弃响应协议的平台管理的对话历史；对话状态由您来管理。
-调用部署的端点需要Entra auth (`DefaultAzureCredential`)，所以CopilotKit运行时通常仍然需要一个瘦服务器端代理来附加令牌-浏览器不能直接调用它。

架构C -到响应协议托管代理的转换桥接托管代理使用`responses`协议（平台管理的会话历史记录、代理版本控制、每个用户隔离）部署，并且在AG-UI和响应流之间有一个单独的桥接服务进行转换。这是最费力的布线；只有当您特别需要响应平台特性时才选择它。

这座桥至少要能承受：1. **流转换**:OpenAI响应SSE事件→AG-UI事件（`response.output_text.delta`→`TEXT_MESSAGE_CONTENT`，函数调用项→`TOOL_CALL_*`，`response.completed`→`RUN_FINISHED`等）。
2. **回合派生，而不是历史回放**：从最新的用户消息（或批准决定）派生每个回合的输入。将完整的原始AG-UI消息历史重播到响应端点失败，有400个关于孤立工具调用的错误。
3. **HITL转发**：将托管代理的`mcp_approval_request`呈现给UI，并将用户的决策作为`mcp_approval_response`输入项转发回来——批准的工具然后重新执行*服务器端*。现有的AG-UI适配器在本地解析批准，从不将它们转发给远程代理（跟踪为microsoft/agent-framework#6652），因此桥需要为该路径提供显式代码。在编写自定义路由之前，根据当前包版本验证是否仍然需要这样做。
4. **对话连续性**:`previous_response_id`chaining （local/direct模式）或Foundry`conversation`对象（deployed/platform模式）。请参阅hitl.md，了解`previous_response_id`跨审批弯链的危险。
5. **状态合成警告：`STATE_SNAPSHOT`/`STATE_DELTA`事件不是由响应流产生的。共享状态和预测状态模式需要桥接器来综合它们（例如从`response.function_call_arguments.delta`）；如果桥没有实现这一点，那么这些模式就不会工作。在承诺该特性之前进行检查。桥接状态（响应id或会话缓存）通常位于内存中：在向外扩展之前运行单个副本或外部化缓存。

##地方发展模式

- **架构A/B**：直接运行FastAPI/ASP.NET服务；将CopilotKit运行时的`HttpAgent`指向`http://localhost:<port>/`。
- **托管代理(B/C)**:`azd ai agent run`在本地运行REAL托管代理（默认端口8088），使用您的`az login`凭据和预置的Foundry项目-没有模拟。`azd ai agent invoke --local "..."`发送单个测试有效负载。本地模式下的桥接器指向裸本地端点，而不是已部署的端点（通常由包含直接URL的单个环境变量进行切换）。

## CopilotKit运行时连接（所有架构）

AG-UI端点，无论它在哪里，在CopilotKit运行时注册为`HttpAgent`：```ts
import { HttpAgent } from "@ag-ui/client";
import { CopilotRuntime } from "@copilotkit/runtime";

const runtime = new CopilotRuntime({
  agents: { "my_agent": new HttpAgent({ url: process.env.AGUI_BACKEND_URL! }) },
});
```
提供者通过名称选择它：`<CopilotKit runtimeUrl="/api/copilotkit" agent="my_agent">`。在`agents`键、`agent`prop和（对于托管代理）`agent.yaml`中的名称之间的名称漂移是一个反复出现的错误—保持一个常量。

使用`useFrontendTool`注册的前端工具流经运行时进入AG-UI`RunAgentInput.tools`数组，并由代理调用；这在所有三种体系结构中都是原生的。

##验证到Foundry端点

-令牌受众是`https://ai.azure.com/.default`-默认的`cognitiveservices.azure.com`范围产生401“受众是不正确的”。
-无钥匙（Entra /`DefaultAzureCredential`）是标准；异步Python凭据路径需要安装`aiohttp`。
-永远不要发送`x-ms-user-isolation-key`到已部署的代理-已部署的代理从Entra身份中获得隔离，并拒绝带有400的报头。