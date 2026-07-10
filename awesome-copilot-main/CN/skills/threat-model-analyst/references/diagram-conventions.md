#图表约定-威胁模型和架构的美人鱼图

此文件包含在威胁模型报告中创建美人鱼图的所有规则。它是自成一体的——生成正确图表所需的一切都在这里。

---

##⛔关键规则-在绘制任何图表之前阅读

这些规则是最常被违反的。首先阅读它们，并在每个图表之后重新检查。

规则1:Kubernetes Sidecar Co-location（强制性）

当目标系统在Kubernetes上运行时，共享Pod的容器必须表示在一起，而不是作为独立的独立组件。

这样做——在主容器的标签上加上注释：**```
InferencingFlow(("Inferencing Flow<br/>+ MISE, Dapr")):::process
IngestionFlow(("Ingestion Flow<br/>+ MISE, Dapr")):::process
VectorDbApi(("VectorDB API<br/>+ Dapr")):::process
```
**不要这样做-永远不要创建独立的侧车节点：**```
❌ MISE(("MISE Sidecar")):::process
❌ DaprSidecar(("Dapr Sidecar")):::process
❌ InferencingFlow -->|"localhost"| MISE
```
**原因：** Sidecars （Dapr，MISE/auth代理，Envoy， Istio代理，日志收集器）与它们的主容器共享Pod的网络命名空间，生命周期和安全上下文。它们不是独立的服务。

**此规则适用于所有图类型：**架构、威胁模型、摘要。

规则2：没有pod内部流（强制性）

**不要在主容器和副容器之间绘制数据流。**这些都是隐含在协同定位注释中的。```
❌ InferencingFlow -->|"localhost:3500"| DaprSidecar
❌ InferencingFlow -->|"localhost:8080"| MISE
```
pod内部通信发生在本地主机上——它没有安全边界，不应该出现在图中。

规则3：跨界Sidecar流源自宿主容器

当sidecar发出跨越信任边界的调用时（例如，MISE→Azure AD, Dapr→Redis），从主机容器节点**绘制箭头** -永远不要从独立的sidecar节点。```
✅ InferencingFlow -->|"HTTPS (MISE auth)"| AzureAD
✅ IngestionAPI -->|"HTTPS (MISE auth)"| AzureAD
✅ InferencingFlow -->|"TCP (Dapr)"| Redis

❌ MISESidecar -->|"HTTPS"| AzureAD
❌ DaprSidecar -->|"TCP"| Redis
```
如果多个pod有相同的侧车调用相同的外部目标，则为每个主机容器画一个箭头。多支箭射向同一个目标是正确的。

规则4：元素表-没有单独的Sidecar行

不要为侧车添加单独的元素表行。在主机容器的描述列中描述它们：```
✅ | Inferencing Flow | Process | API service + MISE auth proxy + Dapr sidecar | Backend Services |
❌ | MISE Sidecar     | Process | Auth proxy for Inferencing Flow              | Backend Services |
```
如果sidecar类有自己的威胁面（例如，MISE auth bypass），它在STRIDE分析中得到`## Component`部分-但它仍然不是一个单独的图节点。

---

预渲染检查表（在最终完成之前进行验证）

绘制任意图后，验证：-[] **每个K8s服务节点都标注了sidecars？** -每个pod的进程节点包括所有共存容器的`<br/>+ SidecarName`-[] **没有独立的侧车节点？** -搜索任何名为`MISE`，`Dapr`,`Envoy`,`Istio`，`Sidecar`的节点图-这些节点不能作为单独的节点存在
- [] ** pod内本地主机流量为零？** -在本地主机上容器和它的侧车之间没有箭头
-[] **东道国跨境侧车流量？** -所有指向外部目标（Azure AD， Redis等）的箭头都来自主机容器节点
-[] **背景强制白色？** -`%%{init}%%`块包括`'background': '#ffffff'`-[] **所有classDef包括`color:#000000`？** -每个元素上的黑色文本
- [] **`linkStyle default`present？** -`stroke:#666666,stroke-width:2px`-[] **所有标签均引用？** -`["Name"]`,`(("Name"))`,`-->|"Label"|`- [] **Subgraph/end对匹配？** -每个`subgraph`都有一个闭合的`end`-[] **是否应用了信任边界样式？* * -`stroke:#e31a1c,stroke-width:3px,stroke-dasharray: 5 5`---

##调色板

> **⛔关键：只使用这些精确的十六进制代码。不要发明颜色，使用Chakra UI颜色（#4299E1, #48BB78, #E53E3E），顺风色，或任何其他调色板。下面的颜色来自ColorBrewer定性调色板，用于色盲访问。从这个文件逐字复制classDef

这些颜色在所有美人鱼图中都是共享的。颜色来自ColorBrewer定性调色板-专为色盲无障碍设计。

|颜色角色|填充|描边|用于||------------|------|--------|----------|
|蓝色|`#6baed6`|`#2171b5`|Services/Processes|
|琥珀|`#fdae61`|`#d94701`|外部互动|
|绿色|`#74c476`|`#238b45`|数据存储|
|红色|n/a|`#e31a1c`|信任边界（仅限威胁模型）|
|深灰色|n/a|`#666666`|Arrows/links|
|文本|所有：`color:#000000`| |黑色文本的每个元素|

设计原理

|元素|填充|描边|文本|为什么||---------|------|--------|------|-----|
|进程|`#6baed6`|`#2171b5`|`#000000`|中蓝色-两个主题都可见|
|外部互动器|`#fdae61`|`#d94701`|`#000000`|暖琥珀色-与blue/green|不同
|数据存储|`#74c476`|`#238b45`|`#000000`|中绿色—自然，适合存储|
|信任边界|无|`#e31a1c`|n/a|红色虚线- 3px可见|
|Arrows/Links|n/a|`#666666`|n/a|白底深灰色|
|背景|`#ffffff`|n/a|n/a|强制白色暗主题安全|

---

强制白色背景（必选）

每个美人鱼图-流程图和序列-必须包括一个`%%{init}%%`块，强制一个白色的背景。这可以确保图表在黑暗主题中正确呈现。> **⛔关键：不要添加`primaryColor`，`secondaryColor`,`tertiaryColor`，或任何自定义颜色键到themeVariables。init块只控制背景和线条颜色。所有元素的颜色都来自classDef lines，而不是来自themeVariables。如果你给themeVariables添加颜色覆盖，它们会破坏classDef调色板

###初始化块流程图

添加每个`.mmd`文件或` `'`mermaid `流程图的**第一行**：```
%%{init: {'theme': 'base', 'themeVariables': { 'background': '#ffffff', 'primaryColor': '#ffffff', 'lineColor': '#666666' }}}%%
```
以上是流程图中唯一允许的init块。**请勿修改。不要添加密钥。逐字抄写。

箭头/链接默认样式

添加在classDef lines之后：```
linkStyle default stroke:#666666,stroke-width:2px
```
序列图Init块

序列图不能使用`classDef`。使用这个init块：```
%%{init: {'theme': 'base', 'themeVariables': {
  'background': '#ffffff',
  'actorBkg': '#6baed6', 'actorBorder': '#2171b5', 'actorTextColor': '#000000',
  'signalColor': '#666666', 'signalTextColor': '#666666',
  'noteBkgColor': '#fdae61', 'noteBorderColor': '#d94701', 'noteTextColor': '#000000',
  'activationBkgColor': '#ddeeff', 'activationBorderColor': '#2171b5',
  'sequenceNumberColor': '#767676',
  'labelBoxBkgColor': '#f0f0f0', 'labelBoxBorderColor': '#666666', 'labelTextColor': '#000000',
  'loopTextColor': '#000000'
}}}%%
```
---

图类型：威胁模型（DFD）

用于：`1-threatmodel.md`，`1.1-threatmodel.mmd`,`1.2-threatmodel-summary.mmd``.mmd`文件格式-关键`.mmd`文件只包含**原始美人鱼源代码** -没有降价，没有代码围栏。文件必须从第1行开始：```
%%{init: {'theme': 'base', 'themeVariables': { 'background': '#ffffff', 'primaryColor': '#ffffff', 'lineColor': '#666666' }}}%%
```
然后是第2行的`flowchart LR`。永远不要使用`flowchart TB`。

**错误**：文件以` `'`plaintext `或` `'`mermaid `开头-这些是代码栅栏并损坏`.mmd`文件。

### ClassDef & Shapes```
classDef process fill:#6baed6,stroke:#2171b5,stroke-width:2px,color:#000000
classDef external fill:#fdae61,stroke:#d94701,stroke-width:2px,color:#000000
classDef datastore fill:#74c476,stroke:#238b45,stroke-width:2px,color:#000000
```
|元素类型|形状语法|示例||-------------|-------------|---------|
|进程|`(("Name"))`循环|`WebApi(("Web API")):::process`|
|外部互动器|`["Name"]`矩形|`User["User/Browser"]:::external`|
|数据存储|`[("Name")]`圆柱体|`Database[("PostgreSQL")]:::datastore`|

信任边界样式```
subgraph BoundaryId["Display Name"]
    %% elements inside
end
style BoundaryId fill:none,stroke:#e31a1c,stroke-width:3px,stroke-dasharray: 5 5
```
###流标签```
Unidirectional:  A -->|"Label"| B
Bidirectional:   A <-->|"Label"| B
```
###数据流id

—详细流程：`DF01`，`DF02`,`DF03`…
-汇总流程：`SDF01`，`SDF02`,`SDF03`…

完成DFD模板```mermaid
%%{init: {'theme': 'base', 'themeVariables': { 'background': '#ffffff', 'primaryColor': '#ffffff', 'lineColor': '#666666' }}}%%
flowchart LR
    classDef process fill:#6baed6,stroke:#2171b5,stroke-width:2px,color:#000000
    classDef external fill:#fdae61,stroke:#d94701,stroke-width:2px,color:#000000
    classDef datastore fill:#74c476,stroke:#238b45,stroke-width:2px,color:#000000
    linkStyle default stroke:#666666,stroke-width:2px

    User["User/Browser"]:::external

    subgraph Internal["Internal Network"]
        WebApi(("Web API")):::process
        Database[("PostgreSQL")]:::datastore
    end

    User <-->|"HTTPS"| WebApi
    WebApi <-->|"SQL/TLS"| Database

    style Internal fill:none,stroke:#e31a1c,stroke-width:3px,stroke-dasharray: 5 5
```
Kubernetes DFD模板（带Sidecars）```mermaid
%%{init: {'theme': 'base', 'themeVariables': { 'background': '#ffffff', 'primaryColor': '#ffffff', 'lineColor': '#666666' }}}%%
flowchart LR
    classDef process fill:#6baed6,stroke:#2171b5,stroke-width:2px,color:#000000
    classDef external fill:#fdae61,stroke:#d94701,stroke-width:2px,color:#000000
    classDef datastore fill:#74c476,stroke:#238b45,stroke-width:2px,color:#000000
    linkStyle default stroke:#666666,stroke-width:2px

    User["User/Browser"]:::external
    IdP["Identity Provider"]:::external

    subgraph K8s["Kubernetes Cluster"]
        subgraph Backend["Backend Services"]
            ApiService(("API Service<br/>+ AuthProxy, Dapr")):::process
            Worker(("Worker<br/>+ Dapr")):::process
        end
        Redis[("Redis")]:::datastore
        Database[("PostgreSQL")]:::datastore
    end

    User -->|"HTTPS"| ApiService
    ApiService -->|"HTTPS"| User
    ApiService -->|"HTTPS"| IdP
    ApiService -->|"SQL/TLS"| Database
    ApiService -->|"Dapr HTTP"| Worker
    ApiService -->|"TCP"| Redis
    Worker -->|"SQL/TLS"| Database

    style K8s fill:none,stroke:#e31a1c,stroke-width:3px,stroke-dasharray: 5 5
    style Backend fill:none,stroke:#e31a1c,stroke-width:3px,stroke-dasharray: 5 5
```
* *重点:* *
—AuthProxy和Dapr在主机节点（`+ AuthProxy, Dapr`）上标注，而不是单独的节点
-`ApiService -->|"HTTPS"| IdP`=授权代理的跨边界调用，从主机容器绘制
-`ApiService -->|"TCP"| Redis`= Dapr的跨界调用，从主机容器绘制
-没有绘制pod内部流

---

图类型：架构

仅用于`0.1-architecture.md`### ClassDef & Shapes```
classDef service fill:#6baed6,stroke:#2171b5,stroke-width:2px,color:#000000
classDef external fill:#fdae61,stroke:#d94701,stroke-width:2px,color:#000000
classDef datastore fill:#74c476,stroke:#238b45,stroke-width:2px,color:#000000
```
|元素类型|形状语法|注释||-------------|-------------|-------|
|Services/Processes|`["Name"]`或`(["Name"])`|圆角矩形或体育场|
|外部演员|`(["Name"])`与`external`类|琥珀区分他们|
|数据存储|`[("Name")]`圆柱体|与DFD |相同
| **不要**使用圆圈`(("Name"))`| |保留用于DFD威胁模型图|

图层分组样式（不信任边界）```
style LayerId fill:#f0f4ff,stroke:#2171b5,stroke-width:2px,stroke-dasharray: 5 5
```
层颜色:
-后端：`fill:#f0f4ff,stroke:#2171b5`（浅蓝色）
-数据：`fill:#f0fff0,stroke:#238b45`（浅绿色）
-外部：`fill:#fff8f0,stroke:#d94701`（淡琥珀色）
-基础设施：`fill:#f5f5f5,stroke:#666666`（浅灰色）

流程约定

-用**标注沟通内容**:`"User queries"`，`"Auth tokens"`,`"Log data"`—协议可以插入：`"Queries (gRPC)"`-比DFD更简单的箭头-使用`-->`不需要双向流

架构图中的Kubernetes Pods

展示豆荚及其完整的集装箱组成：```
inf["Inferencing Flow<br/>+ MISE + Dapr"]:::service
ing["Ingestion Flow<br/>+ MISE + Dapr"]:::service
```
与DFD的关键区别

架构图显示了系统的功能（逻辑组件和交互）。威胁模型DFD显示了可能受到攻击的内容（信任边界、带有协议的数据流、元素类型）。它们共享许多组件，但服务于不同的目的。

完整的架构图模板```mermaid
%%{init: {'theme': 'base', 'themeVariables': { 'background': '#ffffff', 'primaryColor': '#ffffff', 'lineColor': '#666666' }}}%%
flowchart LR
    classDef service fill:#6baed6,stroke:#2171b5,stroke-width:2px,color:#000000
    classDef external fill:#fdae61,stroke:#d94701,stroke-width:2px,color:#000000
    classDef datastore fill:#74c476,stroke:#238b45,stroke-width:2px,color:#000000
    linkStyle default stroke:#666666,stroke-width:2px

    User(["User"]):::external

    subgraph Backend["Backend Services"]
        Api["API Service"]:::service
        Worker["Worker"]:::service
    end

    subgraph Data["Data Layer"]
        Db[("Database")]:::datastore
        Cache[("Cache")]:::datastore
    end

    User -->|"HTTPS"| Api
    Api --> Worker
    Worker --> Db
    Api --> Cache

    style Backend fill:#f0f4ff,stroke:#2171b5,stroke-width:2px,stroke-dasharray: 5 5
    style Data fill:#f0fff0,stroke:#238b45,stroke-width:2px,stroke-dasharray: 5 5
```
Kubernetes架构模板```mermaid
%%{init: {'theme': 'base', 'themeVariables': { 'background': '#ffffff', 'primaryColor': '#ffffff', 'lineColor': '#666666' }}}%%
flowchart LR
    classDef service fill:#6baed6,stroke:#2171b5,stroke-width:2px,color:#000000
    classDef external fill:#fdae61,stroke:#d94701,stroke-width:2px,color:#000000
    classDef datastore fill:#74c476,stroke:#238b45,stroke-width:2px,color:#000000
    linkStyle default stroke:#666666,stroke-width:2px

    User(["User"]):::external
    IdP(["Azure AD"]):::external

    subgraph K8s["Kubernetes Cluster"]
        Inf["Inferencing Flow<br/>+ MISE + Dapr"]:::service
        Ing["Ingestion Flow<br/>+ MISE + Dapr"]:::service
        Redis[("Redis")]:::datastore
    end

    User -->|"HTTPS"| Inf
    Inf -->|"Auth (MISE)"| IdP
    Ing -->|"Auth (MISE)"| IdP
    Inf -->|"State (Dapr)"| Redis

    style K8s fill:#f0f4ff,stroke:#2171b5,stroke-width:2px,stroke-dasharray: 5 5
```
---

序列图规则

适用于：`0.1-architecture.md`顶级场景

**前3个场景必须**每个都包括一个美人鱼`sequenceDiagram`—4-5场景可选择包含一个
-使用**序列图Init块**上面的每个顶部
—使用与Key Components表匹配的`participant`别名
-显示请求-响应模式的激活（`activate`/`deactivate`）
-包括`Note`块用于安全相关的步骤（例如，“验证JWT令牌”）
-保持图表的重点-核心工作流程，而不是每个错误路径

完整序列图示例```mermaid
%%{init: {'theme': 'base', 'themeVariables': {
  'background': '#ffffff',
  'actorBkg': '#6baed6', 'actorBorder': '#2171b5', 'actorTextColor': '#000000',
  'signalColor': '#666666', 'signalTextColor': '#666666',
  'noteBkgColor': '#fdae61', 'noteBorderColor': '#d94701', 'noteTextColor': '#000000',
  'activationBkgColor': '#ddeeff', 'activationBorderColor': '#2171b5'
}}}%%
sequenceDiagram
    actor User
    participant Api as API Service
    participant Db as Database

    User->>Api: POST /resource
    activate Api
    Note over Api: Validates JWT token
    Api->>Db: INSERT query
    Db-->>Api: Result
    Api-->>User: 201 Created
    deactivate Api
```
---

##概要图规则

用于：`1.2-threatmodel-summary.mmd`（仅当详细图具有>5个元素或>4个信任边界时生成）

1. **必须保留所有信任边界** -永远不要合并或省略
2. **只组合非**的组件：入口点、核心流组件、安全关键服务、主数据存储
3. **聚合的候选对象**：支持基础设施，辅助缓存，同一信任级别的多个外部
4. **组合元素标签必须列出内容：**   ```
   DataLayer[("Data Layer<br/>(UserDB, OrderDB, Redis)")]
   SupportServices(("Supporting<br/>(Logging, Monitoring)"))
   ```
5. 对汇总数据流使用`SDF`前缀：`SDF01`，`SDF02`，…
6. 在`1-threatmodel.md`中包含映射表：   ```
   | Summary Element | Contains | Summary Flows | Maps to Detailed Flows |
   ```
---

命名约定

|项目名称|约定|示例||------|-----------|---------|
|元素ID | PascalCase，无空格|`WebApi`，`UserDb`|
|人类可读的引号|`"Web API"`，`"User Database"`|
|带引号的协议或动作|`"HTTPS"`，`"SQL"`,`"gRPC"`|
|流量ID |唯一短标识|`DF01`，`DF02`|
|边界ID | PascalCase |`InternalNetwork`，`PublicDMZ`|

**关键：始终引用美人鱼图表中的所有文本：**
—元素标签：`["Name"]`、`(("Name"))`、`[("Name")]`—流程标签：`-->|"Label"|`-子图标题：`subgraph ID["Title"]`---

快速参考-形状```
External Interactor:  ["Name"]     → Rectangle
Process:              (("Name"))   → Circle (double parentheses)
Data Store:           [("Name")]   → Cylinder
```
##快速参考-流程```
Unidirectional:  A -->|"Label"| B
Bidirectional:   A <-->|"Label"| B
```
##快速参考-边界```
subgraph BoundaryId["Display Name"]
    %% elements inside
end
style BoundaryId fill:none,stroke:#e31a1c,stroke-width:3px,stroke-dasharray: 5 5
```
---

跨步分析- Sidecar的含义

虽然侧车不是单独的图节点，但它们确实出现在STRIDE分析中：

-具有不同威胁面的侧车（例如，MISE auth bypass, Dapr mTLS）在`2-stride-analysis.md`中有自己的`## Component`部分
-组件标题说明了它们共同位于哪个pod中
-与pod内部通信相关的威胁（本地主机绕过，共享命名空间）在**主容器的**组件部分
- STRIDE模板中的Pod Co-location行：列出Co-location Sidecar（例如，“MISE Sidecar, Dapr Sidecar”）