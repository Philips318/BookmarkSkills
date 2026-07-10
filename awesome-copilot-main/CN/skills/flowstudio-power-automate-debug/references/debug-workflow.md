# FlowStudio MCP -调试工作流

端到端决策树诊断电力自动化流故障。

---

顶层决策树```
Flow is failing
│
├── Flow never starts / no runs appear
│   └── ► Check flow State: get_live_flow → properties.state
│       ├── "Stopped" → flow is disabled; enable in PA designer
│       └── "Started" + no runs → trigger condition not met (check trigger config)
│
├── Flow run shows "Failed"
│   ├── Step A: get_live_flow_run_error  → read error.code + error.message
│   │
│   ├── error.code = "InvalidTemplate"
│   │   └── ► Expression error (null value, wrong type, bad path)
│   │       └── See: Expression Error Workflow below
│   │
│   ├── error.code = "ConnectionAuthorizationFailed"
│   │   └── ► Connection owned by different user; fix in PA designer
│   │
│   ├── error.code = "ActionFailed" + message mentions HTTP
│   │   └── ► See: HTTP Action Workflow below
│   │
│   ├── parent action is Foreach / Apply to each
│   │   └── ► Inspect child actions; handled child failures can still fail the parent
│   │
│   └── Unknown / generic error
│       └── ► Walk actions backwards (Step B below)
│
└── Flow Succeeds but output is wrong
    └── ► Inspect intermediate actions with get_live_flow_run_action_outputs
        └── See: Data Quality Workflow below
```
---

表达式错误工作流```
InvalidTemplate error
│
├── 1. Read error.message — identifies the action name and function
│
├── 2. Get flow definition: get_live_flow
│   └── Find that action in definition["actions"][action_name]["inputs"]
│       └── Identify what upstream value the expression reads
│
├── 3. get_live_flow_run_action_outputs for the action BEFORE the failing one
│   └── Look for null / wrong type in that action's output
│       ├── Null string field → wrap with coalesce(): @coalesce(field, '')
│       ├── Null object → add empty check condition before the action
│       └── Wrong field name → correct the key (case-sensitive)
│
└── 4. Apply fix with update_live_flow, then resubmit
```
---

HTTP动作工作流```
ActionFailed on HTTP action
│
├── 1. get_live_flow_run_action_outputs on the HTTP action
│   └── Read: outputs.statusCode, outputs.body
│
├── statusCode = 401
│   └── ► Auth header missing or expired OAuth token
│       Check: action inputs.authentication block
│
├── statusCode = 403
│   └── ► Insufficient permission on target resource
│       Check: service principal / user has access
│
├── statusCode = 400
│   └── ► Malformed request body
│       Check: action inputs.body expression; parse errors often in nested JSON
│
├── statusCode = 404
│   └── ► Wrong URL or resource deleted/renamed
│       Check: action inputs.uri expression
│
└── statusCode = 500 / timeout
    └── ► Target system error; retry policy may help
        Add: "retryPolicy": {"type": "Fixed", "count": 3, "interval": "PT10S"}
```
---

##数据质量工作流```
Flow succeeds but output data is wrong
│
├── 1. Identify the first "wrong" output — which action produces it?
│
├── 2. get_live_flow_run_action_outputs on that action
│   └── Compare actual output body vs expected
│
├── Source array has nulls / unexpected values
│   ├── Check the trigger data — get_live_flow_run_action_outputs on trigger
│   └── Trace forward action by action until the value corrupts
│
├── Merge/union has wrong values
│   └── Check union argument order:
│       union(NEW, old) = new wins  ✓
│       union(OLD, new) = old wins  ← common bug
│
├── Foreach output missing items
│   ├── Check foreach condition — filter may be too strict
│   └── Check if parallel foreach caused race condition (add Sequential)
│
├── Filter/Query result unexpectedly matches nulls or returns empty
│   └── Guard lookup keys before the filter; do not compare null-to-null
│
└── Date/time values wrong timezone
    └── Use convertTimeZone() — utcNow() is always UTC
```
---

回溯分析（未知故障）

当错误信息没有明确指出根本原因时：```python
# 1. Get all action names from definition
defn = mcp("get_live_flow", environmentName=ENV, flowName=FLOW_ID)
actions = list(defn["properties"]["definition"]["actions"].keys())

# 2. Check status of each action in the failed run
for action in actions:
    actions_out = mcp("get_live_flow_run_action_outputs",
        environmentName=ENV, flowName=FLOW_ID, runName=RUN_ID,
        actionName=action)
    # Returns an array of action objects
    item = actions_out[0] if actions_out else {}
    status = item.get("status", "unknown")
    print(f"{action}: {status}")

# 3. Find the boundary between Succeeded and Failed/Skipped
# The first Failed action is likely the root cause (unless skipped by design)
```
Foreach / Condition分支中的动作可能出现嵌套
首先检查父操作以确认分支是否运行。

---

修复后验证清单

1.`update_live_flow`返回`error: null`-定义接受
2.`resubmit_live_flow_run`确认新运行开始
3. 等待运行完成（每15秒轮询`get_live_flow_runs`）
4. 确认新运行`status = "Succeeded"`5. 如果流有下游消费者（子流、电子邮件、SharePoint写入），
也要抽查一下