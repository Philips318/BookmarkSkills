# 7 AG-UI交互模式在这个堆栈

这些是规范的AG-UI“dojo”模式（dojo.ag-ui.com有实时的Microsoft Agent Framework示例）。MAF AG-UI集成文档支持所有这七个方面。该表将每个模式映射到其代理端和copilotkit端实现；后面的注释涵盖了不明显的内容。

CopilotKit钩子命名：`useCopilotAction`仍然存在，但文档中将其作为遗留兼容性钩子。当前名称：`useFrontendTool`（可代理调用的前端工具）、`useHumanInTheLoop`（审批UI；继承`useCopilotAction`和`renderAndWaitForResponse`）、`useRenderToolCall`/`useRenderTool`（仅渲染生成UI）、`useCoAgent`/`useCoAgentStateRender`（共享状态）。根据已安装包的TypeScript声明来验证准确的导出——名称已经在未成年人之间移动了。

| # |模式|代理端（MAF Python） | CopilotKit端|| --- | --- | --- | --- |
|代理聊天+前端工具| plain`Agent`-前端工具通过`RunAgentInput.tools`|`useFrontendTool({ name, parameters, handler })`|到达
|后端工具呈现|`@tool`（执行服务器端）|`useRenderToolCall`/呈现工具名称|的条目
|人在环|`@tool(approval_mode="always_require")`+`AgentFrameworkAgent(require_confirmation=True)`|`useHumanInTheLoop({ name, render })`，解析通过`respond(...)`|
| |代理生成UI |长时间运行的工具发出进度状态|`useCoAgentStateRender`呈现正在进行状态|
|基于工具的生成式UI |仅声明工具（无可执行体）模型必须用`render`调用|`useFrontendTool`，通常是`followUp: false`|
bbb6 |共享状态|状态模式+工具状态更新|`useCoAgent`-读取`state`，通过`setState`|写入
|预测状态更新|工具参数流配置为乐观状态预测|`useCoAgent`+确认UI |

##节省时间的笔记模式1（前端工具）。**该工具在浏览器中执行；座席只发出呼叫。如果代理报告它看不到前端工具，检查CopilotKit运行时是否实际将注册的工具转发到当前包版本上的`RunAgentInput.tools`—这种转发在1.62中存在回归。X （CopilotKit/CopilotKit#5813，修复后不久）。升级或固定过去的修复比任何代码更改更重要。模式2（后端工具渲染）。**渲染组件接收流工具调用参数，然后接收结果。存在两个呈现阶段：运行期间的实时`TOOL_CALL_*`事件和运行结束时的`MESSAGES_SNAPSHOT`。如果快照表示不同的转弯，那么在流媒体期间呈现的卡片可能会在`RUN_FINISHED`处消失（值得注意的是：当UI只呈现第一个时，多个工具调用集中在一个辅助消息中）。在运行完成后，始终检查卡是否仍然存在。

模式3 （HITL）。**全面处理hitl.md-包括有效载荷形状合同和重复执行的危险。模式5（基于工具的生成式UI）。**代理端工具是一个没有实现的声明——模型“调用”它，前端将参数呈现为UI。当呼刀是回合的终端动作时使用`followUp: false`，否则座席会冗余叙述呼刀。如果模型必须总是生成UI，那么在代理端约束工具选择，而不是希望提示足够。**模式4/6/7（状态族）-架构依赖。**`STATE_SNAPSHOT`/`STATE_DELTA`（RFC 6902 JSON Patch）事件仅在AG-UI适配器包装进程内代理（architecture.md中的架构A和B）时才会本地发出。响应协议桥接（体系结构C）不从托管代理获取这些事件；它们必须由桥接器从工具参数增量中合成，并且必须将来自客户机的`setState`显式转发到代理的输入中。在实现共享状态特性之前，请确认代码库位于这条线的哪一边——否则您将编写针对永远不会到达的事件的前端代码。`useCoAgent().state`永久为空通常意味着代理没有配置状态模式，或者没有工具写入状态键，而不是前端错误。**参数重命名涟漪到UI。渲染组件通常解析流工具参数中的特定字段。当代理继续工作时，重命名Python工具参数会静默地破坏卡片（wrong/missing字段）—bridge/adapter会逐字转发参数。每当您重命名一个工具参数时，都要在前端添加旧的字段名。

**接地安全工具文档字符串。**不要在模型应该从实时数据（账号、id、金额）中导出的字段的参数描述中嵌入具体的示例值：模型会将描述中的文字示例复制到实际调用中。描述形状，使用占位符，并在工具中进行验证。