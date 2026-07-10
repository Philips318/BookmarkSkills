#骨架：1.1-threatmodel.mmd

> **⛔这是一个原始的美人鱼文件-没有标记包装。第一行必须以`%%{init:`.**开头
> ** init block， classDefs和linkStyle是固定的-永远不要改变colors/strokes.**
b> **图的方向永远是`flowchart LR`-从不是`flowchart TB`.**
> **⛔下面的模板是在一个代码栅栏内显示的，只是为了可读性-不要在输出文件中包含栅栏

---```
%%{init: {'theme': 'base', 'themeVariables': { 'background': '#ffffff', 'primaryColor': '#ffffff', 'lineColor': '#666666' }}}%%
flowchart LR
    classDef process fill:#6baed6,stroke:#2171b5,stroke-width:2px,color:#000000
    classDef external fill:#fdae61,stroke:#d94701,stroke-width:2px,color:#000000
    classDef datastore fill:#74c476,stroke:#238b45,stroke-width:2px,color:#000000
    [CONDITIONAL: incremental mode — include BOTH lines below]
    classDef newComponent fill:#d4edda,stroke:#28a745,stroke-width:3px,color:#000000
    classDef removedComponent fill:#e9ecef,stroke:#6c757d,stroke-width:1px,stroke-dasharray:5,color:#6c757d
    [END-CONDITIONAL]

    [REPEAT: one line per external actor/interactor — outside all subgraphs]
    [FILL: NodeID]["[FILL: Display Name]"]:::external
    [END-REPEAT]

    [REPEAT: one subgraph per trust boundary]
    subgraph [FILL: BoundaryID]["[FILL: Boundary Display Name]"]
        [REPEAT: processes and datastores inside this boundary]
        [FILL: NodeID](("[FILL: Process Name]")):::process
        [FILL: NodeID][("[FILL: DataStore Name]")]:::datastore
        [END-REPEAT]
    end
    [END-REPEAT]

    [REPEAT: one line per data flow — use <--> for bidirectional request-response]
    [FILL: SourceID] <-->|"[FILL: DF##: description]"| [FILL: TargetID]
    [END-REPEAT]

    [REPEAT: one style line per trust boundary subgraph]
    style [FILL: BoundaryID] fill:none,stroke:#e31a1c,stroke-width:3px,stroke-dasharray: 5 5
    [END-REPEAT]

    linkStyle default stroke:#666666,stroke-width:2px
```
**永远不要改变这些固定的元素
-`%%{init:`themeVariables：只有`background`、`primaryColor`、`lineColor`-`flowchart LR`-绝不是TB
- classDef color: process=#6baed6/#2171b5, external=#fdae61/#d94701, datastore=#74c476/#238b45
-增量类定义（当适用时）：newComponent=#d4edda/#28a745（浅绿色），removedComponent=#e9ecef/#6c757d（灰色虚线）
-新组件必须使用`:::newComponent`（不是`:::process`）。移除的组件必须使用`:::removedComponent`。
—信任边界样式：`fill:none,stroke:#e31a1c,stroke-width:3px,stroke-dasharray: 5 5`—linkStyle:`stroke:#666666,stroke-width:2px`* *中的形状:* *
-进程：`(("Name"))`（双括号=圆）
—数据存储：`[("Name")]`（括号=圆柱体）
-外部：`["Name"]`（括号=矩形）
-所有标签必须用`""`引号
—所有子图id:`subgraph ID["Title"]`<!-- ⛔ POST-DFD GATE — IMMEDIATELY after creating this file:
1. 计数元素节点：行（“…”）")), [("...")]、[”…”形状
2. 计数边界：带有‘subgraph’的线
3. 如果元素> 15或边界> 4：     → OPEN skeleton-summary-dfd.md and create 1.2-threatmodel-summary.mmd NOW
     → Do NOT proceed to 1-threatmodel.md until summary exists
4. 如果阈值未满足→跳过总结，继续1-threatmodel.md这是最常被跳过的步骤。这扇门是强制性的。-->