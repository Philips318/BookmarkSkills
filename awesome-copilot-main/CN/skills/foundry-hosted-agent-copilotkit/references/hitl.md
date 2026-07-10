#人在循环审批

HITL是这个堆栈中风险最高的特性：它会阻止相应的、副作用的操作，并且故障模式是无声的（一个工具未经批准运行，或者运行两次）。将每个HITL更改视为安全关键，并验证所有三个结果：批准执行一次，拒绝执行零次，后续回合执行零次。

##布线（Python，进程内AG-UI端点）```python
from agent_framework import Agent, tool
from agent_framework_ag_ui import AgentFrameworkAgent, add_agent_framework_fastapi_endpoint

@tool(approval_mode="always_require")
def transfer_money(from_account: str, to_account: str, amount: float) -> str:
    """Transfer money between accounts."""
    ...

agent = Agent(name="assistant", instructions="...", client=chat_client,
              tools=[transfer_money, check_balance])
wrapped = AgentFrameworkAgent(agent=agent, require_confirmation=True)
add_agent_framework_fastapi_endpoint(app, wrapped, "/")
```
这两部分都是必需的：工具上的`approval_mode="always_require"`和包装器上的`require_confirmation=True`。注意`approval_mode`只控制*审批* -`never_require`并不意味着工具是只读的；这是实现者的责任。

**`.NET`**使用与Python的包装器标志不同的机制：将工具包装在`ApprovalRequiredAIFunction`中，然后将*代理*包装在`DelegatingAIAgent`中，该`DelegatingAIAgent`连接审批内容to/from和`request_approval`客户端工具调用（官方示例[`Step04_HumanInLoop`]（https://github.com/microsoft/agent-framework/tree/main/dotnet/samples/02-agents/AGUI/Step04_HumanInLoop）中的模式）：```csharp
// Tool requires approval; wrap the agent so the UI can render an approval card.
AITool[] tools = [new ApprovalRequiredAIFunction(AIFunctionFactory.Create(ApproveExpenseReport))];
var baseAgent = openAIChatClient.AsAIAgent(name: "assistant", instructions: "...", tools: tools);
var agent = new ServerFunctionApprovalAgent(baseAgent, jsonOptions.SerializerOptions);
app.MapAGUI("/", agent);

internal sealed class ServerFunctionApprovalAgent(AIAgent inner, JsonSerializerOptions json)
    : DelegatingAIAgent(inner)
{
    protected override async IAsyncEnumerable<AgentResponseUpdate> RunCoreStreamingAsync(...)
    {
        // INBOUND: convert the client's `request_approval` call + result back into a matched
        // ToolApprovalRequestContent / ToolApprovalResponseContent pair before the inner agent
        // runs. Leaving a raw request_approval tool_call without its paired result in history
        // makes Azure OpenAI 400: "tool_calls must be followed by tool messages...".
        var processed = ProcessIncomingFunctionApprovals(messages.ToList(), json);
        await foreach (var update in InnerAgent.RunStreamingAsync(processed, ...))
            // OUTBOUND: convert ToolApprovalRequestContent -> a `request_approval` client tool
            // call so the frontend renders the approval card.
            yield return ProcessOutgoingApprovalRequests(update, json);
    }
}
```
内容类型是`ToolApprovalRequestContent`/`ToolApprovalResponseContent`（不是`FunctionApprovalRequestContent`），修复是*转换*批准call/result-保持它们配对-不要从消息历史中删除它们，或者Azure OpenAI失败与“tool_calls必须跟着工具消息响应每个‘tool_call_id’”。

# #前端```tsx
useHumanInTheLoop({
  name: "confirm_changes",           // must match what the server surfaces
  render: ({ args, respond, status }) => (
    <ApprovalCard
      args={args}
      onApprove={() => respond?.({ accepted: true })}
      onReject={() => respond?.({ accepted: false })}
    />
  ),
});
```
**有效载荷形状是一个契约，而不是一个框架特性。** CopilotKit的`respond(...)`接受任何JSON值；服务器端代码决定什么算“批准”。读取服务器的检测逻辑并精确匹配它—解析`{ approved: true }`的UI与检查`accepted`密钥的服务器会无声地失败：单击不执行任何操作，任何地方都不会出现错误。每当批准“什么都不做”时，首先将解析的有效负载与服务器的检测进行比较。

服务器表面的审批工具名称（例如`confirm_changes`）必须通过`useHumanInTheLoop`注册，否则不会出现卡。

托管代理：批准实际上是如何传递的当代理作为响应协议后面的Foundry托管代理运行时，经过批准的工具将作为响应流中的`mcp_approval_request`项出现。决策必须作为`mcp_approval_response`输入项发回；然后，托管代理在获得批准后重新在服务器端执行该工具。两个后果:1. **现有的AG-UI适配器不会将审批转发给远程代理**——它在本地解析`confirm_changes`，所以审批似乎成功了，但门控工具永远不会重新执行，状态也永远不会改变（跟踪为microsoft/agent-framework#6652，在2026年年中打开）。到托管代理的桥接需要显式的批准转发代码。症状签名：审批卡起作用，审批返回正常回复，但副作用始终没有发生。
2. Approve意味着重新执行发生在UI的视线之外。通过观察状态变化来验证（之后查询受影响的记录），而不是通过聊天记录来验证。

桥接器显式地提供转发——出站它将托管代理的`mcp_approval_request`转换为前端的批准工具调用；它将UI的`{accepted}`结果转换回`mcp_approval_response`输入项：```python
# OUTBOUND: hosted agent emits mcp_approval_request -> surface it as the frontend's
# `confirm_changes` tool call so CopilotKit's useHumanInTheLoop renders a card.
elif item["type"] == "mcp_approval_request":
    _PENDING_APPROVAL[thread_id] = item["id"]          # remember the request id
    yield tool_call(APPROVAL_TOOL, {                   # APPROVAL_TOOL == "confirm_changes"
        "function_name": item.get("name", ""),
        "function_arguments": item.get("arguments", ""),
    })

# INBOUND (next turn): the UI's {accepted} result -> an mcp_approval_response input
# item the hosted agent understands. The stock AG-UI adapter never does this step.
pending = _PENDING_APPROVAL.get(thread_id)
if pending and last_tool_result and "accepted" in last_tool_result:
    _PENDING_APPROVAL.pop(thread_id, None)
    turn_input = [{"type": "mcp_approval_response",
                   "approval_request_id": pending,
                   "approve": bool(last_tool_result["accepted"])}]
```
重复执行的危险（在发布任何html更改之前阅读）

**症状：**一次批准工作正常，然后在同一对话中进行LATER，不相关的转向默默地重新执行相同的门控制工具-副作用应用两次，没有批准卡，没有可见的指示。

**根本原因（通过调用托管代理的裸`/responses`端点与curl隔离-没有agui，没有CopilotKit在循环中）：**链接`previous_response_id`通过一个响应，解决了一个`mcp_approval_response`使托管运行时重新执行批准的工具在下一个回合，不管该回合的内容。错误存在于agent-framework/Foundry托管层，而不是AG-UI适配器或CopilotKit。跟踪为microsoft/agent-framework#6851（重复执行）和#6828（相关审批状态症状）；截至2026年7月，两者仍然开放-在依赖框架行为之前检查当前状态。**使用`previous_response_id`链的桥梁缓解：**在一个回合的输入包含`mcp_approval_response`后，不存储该响应id用于链-让下一个回合开始没有`previous_response_id`。这将消耗少量会话内存，并保证门控操作永远不会静默执行两次。平台模式会话（Foundry`conversation`对象而不是响应id链）具有不同的机制-不要认为它们是免疫的；明确测试。```python
# On response.completed we normally store the id to chain the next turn via
# previous_response_id. But if THIS turn resolved an approval, do NOT store it:
# chaining through an approval-resolving response makes the hosted runtime silently
# re-execute the approved tool on the next, unrelated turn (agent-framework #6851).
if approval_turn:
    _LAST_RESPONSE.pop(thread_id, None)   # break the chain -> no duplicate exec
else:
    _LAST_RESPONSE[thread_id] = response_id
```
**永远保持回归测试：**批准后，在同一个线程中发送几个不相关的后续回合，并断言门控制工具的副作用没有复发（例如，计数器精确地增加一次）。只有当上游问题被关闭，并且测试通过时，才删除任何本地缓解——永远不要只在版本碰撞上删除。

##调试决策树

自上而下的工作;每个步骤都有一个独特的签名：1. **批准→400/500“找不到函数调用的工具输出…”**→座席的模型客户端是基于聊天完成的。托管代理上的审批恢复需要响应协议客户端（MAF Python中的`FoundryChatClient`）。交换客户端。
2. **没有出现批准卡**→`useHumanInTheLoop`没有注册到表面的工具名称，或者工具缺少`approval_mode="always_require"`（它立即执行-检查服务器日志的工具运行）。
3. **点击批准不做任何事情，没有错误**→有效载荷形状不匹配`respond(...)`和服务器的检测（见上面的合同）。
4. **审批被解析，回复流，但状态从未改变**→审批被本地解析，从未到达远程代理（#6652-class）。确认bridge/adapter实际上转发`mcp_approval_response`。
5. **工作一次，然后以后的回合双重执行**→重复执行以上的危险。
6. **卡在运行期间呈现，但在xqz7时消失xqz**→消息快照表示不同于实时事件（多工具调用被集中到一个消息中；一些UI版本只呈现第一个工具调用）。修复快照构建或升级UI层；验证运行后的DOM，而不仅仅是在运行中。
7. 这时才会出现可疑的环境：租户不匹配的404、错误的令牌受众401、本地运行代理中过时的内存数据（在测试通过之间重新启动`azd ai agent run`）。