#骨架：1.2-threatmodel-summary.mmd

> **⛔总是在创建`1.1-threatmodel.mmd`后评估这个骨架
>在详细DFD中计数元素（带有`(("..."))`、`[("...")]`、`["..."]`的节点）和边界（`subgraph`）。
> -如果元素> 15或边界> 4→这个文件是**REQUIRED**。填充下面的模板。
> -如果元素≤15且边界≤4→**跳过**该文件。继续到`1-threatmodel.md`。
> **⛔这是一个原始的美人鱼文件。下面的模板是在一个代码栅栏内显示的，只是为了可读性-不要在输出文件中包括栅栏。`.mmd`文件必须在第1行以`%%{init:`开头

---```
%%{init: {'theme': 'base', 'themeVariables': { 'background': '#ffffff', 'primaryColor': '#ffffff', 'lineColor': '#666666' }}}%%
flowchart LR
    classDef process fill:#6baed6,stroke:#2171b5,stroke-width:2px,color:#000000
    classDef external fill:#fdae61,stroke:#d94701,stroke-width:2px,color:#000000
    classDef datastore fill:#74c476,stroke:#238b45,stroke-width:2px,color:#000000

    [FILL: External actors — keep all, do not aggregate]
    [FILL: ExternalActor]["[FILL: Name]"]:::external

    [REPEAT: one subgraph per trust boundary — ALL boundaries MUST be preserved]
    subgraph [FILL: BoundaryID]["[FILL: Boundary Name]"]
        [FILL: Aggregated and individual nodes]
    end
    [END-REPEAT]

    [REPEAT: summary data flows using SDF prefix]
    [FILL: Source] <-->|"[FILL: SDF##: description]"| [FILL: Target]
    [END-REPEAT]

    [REPEAT: boundary styles]
    style [FILL: BoundaryID] fill:none,stroke:#e31a1c,stroke-width:3px,stroke-dasharray: 5 5
    [END-REPEAT]

    linkStyle default stroke:#666666,stroke-width:2px
```
##聚合规则

**参考：**`diagram-conventions.md`→总结图规则的全部细节。

1. **必须保留所有信任边界-永远不要合并或省略边界。
2. **单独保留：**入口点，核心流组件，安全关键服务，主数据存储，所有外部参与者。
3. **仅聚合：**支持基础设施，辅助缓存，相同信任级别的多个外部。
4. **聚合元素标签必须列出内容：**   ```
   DataLayer[("Data Layer<br/>(UserDB, OrderDB, Redis)")]
   SupportServices(("Supporting<br/>(Logging, Monitoring)"))
   ```
5. **流量id:**使用`SDF`前缀：`SDF01`，`SDF02`，…

##`1-threatmodel.md`必须

当这个文件生成时，`1-threatmodel.md`必须包括：
-在` `'`mermaid `围栏中使用此图的`## Summary View`部分
-一个`## Summary to Detailed Mapping`表：```markdown
| Summary Element | Contains | Summary Flows | Maps to Detailed Flows |
|----------------|----------|---------------|------------------------|
| [FILL] | [FILL: list of detailed elements] | [FILL: SDF##] | [FILL: DF## list] |
```
