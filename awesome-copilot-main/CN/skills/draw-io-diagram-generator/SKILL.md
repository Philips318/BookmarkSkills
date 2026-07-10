---
name: draw-io-diagram-generator
description: Use when creating, editing, or generating draw.io diagram files (.drawio, .drawio.svg, .drawio.png). Covers mxGraph XML authoring, shape libraries, style strings, flowcharts, system architecture, sequence diagrams, ER diagrams, UML class diagrams, network topology, layout strategy, the hediet.vscode-drawio VS Code extension, and the full agent workflow from request to a ready-to-open file.
---
#画。io图生成器

该技能使您能够生成、编辑和验证绘图。IO （`.drawio`）关系图文件
正确的mxGraph XML结构中打开所有生成的文件
(画。[VS Code扩展]（https://marketplace.visualstudio.com/items?itemName=hediet.vscode-drawio）
（`hediet.vscode-drawio`），不需要任何手动修复。您也可以打开绘图中的文件。IO web应用程序或桌面应用程序，如果你喜欢。

---

# # 1。何时使用此技能

**触发短语（加载此技能当你看到这些）**

-“创建图表”，“绘制流程图”，“生成架构图”
-“设计序列图”、“制作UML类图”、“构建ER图”
-”加一个。“绘制文件”，“更新图表”，“可视化流程”
-“记录架构”，“显示数据模型”，“绘制服务交互图”
-任何生成或修改`.drawio`、`.drawio.svg`或`.drawio.png`文件的请求

**支持的图表类型**|图表类型|模板可选|描述||---|---|---|
流程图|`assets/templates/flowchart.drawio`|带有决策和分支的流程|
|系统架构|`assets/templates/architecture.drawio`|多层次业务架构|
|序列图|`assets/templates/sequence.drawio`| Actor生命线和定时消息流|
| ER图|`assets/templates/er-diagram.drawio`|数据库表与关系|
UML类图|`assets/templates/uml-class.drawio`|类、接口、枚举、关系|
|网络拓扑|（使用形状库）|路由器、服务器、防火墙、子网|
| BPMN工作流|（使用形状库）|业务流程事件、任务、网关|
思维导图|（手动）|中心主题与辐射分支|

---

# # 2。先决条件

-如果运行与VS Code集成启用，安装drawio扩展：**draw。IOVS Codeextension** -`hediet.vscode-drawio`（extension id）。安装:  ```
  ext install hediet.vscode-drawio
  ```
- **支持的文件扩展名**:`.drawio`、`.drawio.svg`、`.drawio.png`- **Python 3.8+**（可选）-用于`scripts/`中的验证和形状插入脚本

---

# # 3。分步代理工作流程

按照以下步骤完成每个图表生成任务。

###步骤1 -理解请求

询问或推断：
1. **图表类型** -哪种图表？（流程图、体系结构、UML、ER、序列、网络……）
2. **实体/演员** -什么是主要的组件，演员，类，或表？
3. **关系** -它们是如何联系在一起的？哪个方向?基数?
4. **输出路径** -`.drawio`文件应该保存在哪里？
5. **现有文件** -我们是在创建新文件还是编辑现有文件？

如果请求是模糊的，从上下文推断出最合理的图类型(例如：“显示表”→ER图，“显示API调用流”→序列图)。###步骤2 -选择模板或重新开始

- **当图表类型匹配`assets/templates/`时，使用模板**。复制模板结构并替换占位符值。
- **开始新鲜**新颖的布局。从最小有效骨架开始：```xml
<!-- Set modified="" to the current ISO 8601 timestamp when generating a new file -->
<mxfile host="Electron" modified="" version="26.0.0">
  <diagram id="page-1" name="Page-1">
    <mxGraphModel dx="1422" dy="762" grid="1" gridSize="10" guides="1"
                  tooltips="1" connect="1" arrows="1" fold="1"
                  page="1" pageScale="1" pageWidth="1169" pageHeight="827"
                  math="0" shadow="0">
      <root>
        <mxCell id="0" />
        <mxCell id="1" parent="0" />
        <!-- Your cells go here -->
      </root>
    </mxGraphModel>
  </diagram>
</mxfile>
```
b> **规则**:id`0`和`1`总是必需的，并且必须是前两个单元格。永远不要重复使用它们。

###步骤3 -规划布局

在生成XML之前，先勾画一下逻辑位置：
-组织成**行**或**层**（使用泳道层）
- **水平间距**:40-60px之间的同行形状
- **垂直间距**:80-120px层行之间
-标准形状尺寸：处理箱`120x60`px，泳道`160x80`px
-默认画布：A4横向=`1169 x 827`px

###步骤4 -生成mxGraph XML

**顶点单元**（每个形状）：```xml
<mxCell id="unique-id" value="Label"
        style="rounded=1;whiteSpace=wrap;html=1;fillColor=#dae8fc;strokeColor=#6c8ebf;"
        vertex="1" parent="1">
  <mxGeometry x="100" y="100" width="120" height="60" as="geometry" />
</mxCell>
```
**边缘单元**（每个连接器）：```xml
<mxCell id="edge-id" value="Label (optional)"
        style="edgeStyle=orthogonalEdgeStyle;html=1;"
        edge="1" source="source-id" target="target-id" parent="1">
  <mxGeometry relative="1" as="geometry" />
</mxCell>
```
* * * *重要规则:
—每个单元格id在文件中必须是全局唯一的
-每个顶点必须有`mxGeometry`子节点，包括`x`，`y`,`width`,`height`,`as="geometry"`-每个边必须有`source`和`target`匹配现有顶点id - **例外**：浮动边（如序列图生命线）使用`sourcePoint`/`targetPoint`在`<mxGeometry>`代替；见§4时序图
-每个单元格的`parent`必须引用一个现有的单元格id
-当标签包含HTML时使用`html=1`样式（`<b>`,`<i>`,`<br>`）
-转义标签中的XML特殊字符：`&`=>`&amp;`,`<`=>`&lt;`,`>`=>`&gt;`###步骤5 -应用正确的样式

使用标准的语义调色板来保持一致性：

|目的| fillColor | strokecol||---|---|---|
|主/信息|`#dae8fc`|`#6c8ebf`|
| Success / Start |`#d5e8d4`|`#82b366`|
|`#fff2cc`|`#d6b656`|
|错误/结束|`#f8cecc`|`#b85450`|
|中性|`#f5f5f5`|`#666666`|
|外部/合作伙伴|`#e1d5e7`|`#9673a6`|

按图表类型划分的常见样式字符串：```
# Rounded process box (flowchart)
rounded=1;whiteSpace=wrap;html=1;fillColor=#dae8fc;strokeColor=#6c8ebf;

# Decision diamond
rhombus;whiteSpace=wrap;html=1;fillColor=#fff2cc;strokeColor=#d6b656;

# Start/End terminal
ellipse;whiteSpace=wrap;html=1;fillColor=#d5e8d4;strokeColor=#82b366;

# Database cylinder
shape=mxgraph.flowchart.database;whiteSpace=wrap;html=1;fillColor=#f8cecc;strokeColor=#b85450;

# Swimlane container (tier)
swimlane;startSize=30;fillColor=#dae8fc;strokeColor=#6c8ebf;fontStyle=1;

# UML class box
swimlane;fontStyle=1;align=center;startSize=40;fillColor=#dae8fc;strokeColor=#6c8ebf;

# Interface / stereotype box
swimlane;fontStyle=3;align=center;startSize=40;fillColor=#f5f5f5;strokeColor=#666666;

# ER table container
shape=table;startSize=30;container=1;collapsible=1;childLayout=tableLayout;

# Orthogonal connector
edgeStyle=orthogonalEdgeStyle;html=1;

# ER relationship (crow's foot)
edgeStyle=entityRelationEdgeStyle;html=1;endArrow=ERmany;startArrow=ERone;
```
b>请参阅`references/style-reference.md`了解完整的样式键目录，并参阅`references/shape-libraries.md`了解所有形状库名称。

###步骤6 -保存并验证

1. **将文件**写入请求的路径，扩展名为`.drawio`2. **运行验证器**（可选但推荐）：   ```bash
   python .github/skills/draw-io-diagram-generator/scripts/validate-drawio.py <path-to-file.drawio>
   ```
3. **告诉用户**如何打开文件：
打开`<filename>`在VS Code-它会自动渲染与绘制。io扩展。你可以用draw。如果你愿意，也可以选择Io的网页应用或桌面应用。”
4. **对图表中的内容提供简短的描述，以便用户知道将会发生什么。

---

# # 4。图表类型的食谱

# # #流程图

关键元素：Start（椭圆）=> Process（圆角矩形）=> Decision（菱形）=> End（椭圆）```xml
<!-- Start node -->
<mxCell id="start" value="Start"
        style="ellipse;whiteSpace=wrap;html=1;fillColor=#d5e8d4;strokeColor=#82b366;"
        vertex="1" parent="1">
  <mxGeometry x="500" y="80" width="120" height="60" as="geometry" />
</mxCell>

<!-- Process -->
<mxCell id="p1" value="Process Step"
        style="rounded=1;whiteSpace=wrap;html=1;fillColor=#dae8fc;strokeColor=#6c8ebf;"
        vertex="1" parent="1">
  <mxGeometry x="500" y="200" width="120" height="60" as="geometry" />
</mxCell>

<!-- Decision -->
<mxCell id="d1" value="Condition?"
        style="rhombus;whiteSpace=wrap;html=1;fillColor=#fff2cc;strokeColor=#d6b656;"
        vertex="1" parent="1">
  <mxGeometry x="460" y="320" width="200" height="100" as="geometry" />
</mxCell>

<!-- Arrow: start to p1 -->
<mxCell id="e1" value=""
        style="edgeStyle=orthogonalEdgeStyle;html=1;"
        edge="1" source="start" target="p1" parent="1">
  <mxGeometry relative="1" as="geometry" />
</mxCell>
```
架构图（3层）

每层使用泳道容器。所有的服务台都是泳道的孩子。```xml
<!-- Tier swimlane -->
<mxCell id="tier1" value="Client Layer"
        style="swimlane;startSize=30;fillColor=#dae8fc;strokeColor=#6c8ebf;fontStyle=1;"
        vertex="1" parent="1">
  <mxGeometry x="60" y="100" width="1050" height="130" as="geometry" />
</mxCell>

<!-- Service inside tier (parent="tier1", coords are relative to tier) -->
<mxCell id="webapp" value="Web App"
        style="rounded=1;whiteSpace=wrap;html=1;fillColor=#dae8fc;strokeColor=#6c8ebf;"
        vertex="1" parent="tier1">
  <mxGeometry x="80" y="40" width="120" height="60" as="geometry" />
</mxCell>
```
>层之间的连接器使用绝对坐标`parent="1"`。

序列图

关键元素：角色（顶部）、生命线（虚线）、激活框、消息箭头。

-生命线：`edge="1"`与`endArrow=none`和`dashed=1`，没有source/target-使用`sourcePoint`/`targetPoint`在几何
—同步消息：`endArrow=block;endFill=1`—返回消息：`endArrow=open;endFill=0;dashed=1`-自我调用：通过两个数组点向右和向后循环边缘

**最小的XML片段：**```xml
<!-- Actor (stick figure) -->
<mxCell id="actorA" value="Client"
        style="shape=mxgraph.uml.actor;pointerEvents=1;dashed=0;whiteSpace=wrap;html=1;aspect=fixed;"
        vertex="1" parent="1">
  <mxGeometry x="110" y="80" width="60" height="80" as="geometry" />
</mxCell>

<!-- Service box -->
<mxCell id="actorB" value="API Server"
        style="rounded=1;whiteSpace=wrap;html=1;fillColor=#fff2cc;strokeColor=#d6b656;"
        vertex="1" parent="1">
  <mxGeometry x="480" y="100" width="160" height="60" as="geometry" />
</mxCell>

<!-- Lifeline — floating edge: uses sourcePoint/targetPoint, NOT source/target attributes -->
<mxCell id="lifA" value=""
        style="edgeStyle=none;dashed=1;endArrow=none;"
        edge="1" parent="1">
  <mxGeometry relative="1" as="geometry">
    <mxPoint x="140" y="160" as="sourcePoint" />
    <mxPoint x="140" y="700" as="targetPoint" />
  </mxGeometry>
</mxCell>

<!-- Activation box (thin rectangle on lifeline) -->
<mxCell id="actA1" value=""
        style="fillColor=#dae8fc;strokeColor=#6c8ebf;"
        vertex="1" parent="1">
  <mxGeometry x="130" y="220" width="20" height="180" as="geometry" />
</mxCell>

<!-- Synchronous message -->
<mxCell id="msg1" value="POST /orders"
        style="edgeStyle=elbowEdgeStyle;elbow=vertical;html=1;endArrow=block;endFill=1;"
        edge="1" source="actA1" target="actorB" parent="1">
  <mxGeometry relative="1" as="geometry" />
</mxCell>

<!-- Return message (dashed) -->
<mxCell id="msg2" value="201 Created"
        style="edgeStyle=elbowEdgeStyle;elbow=vertical;dashed=1;html=1;endArrow=open;endFill=0;"
        edge="1" source="actorB" target="actA1" parent="1">
  <mxGeometry relative="1" as="geometry" />
</mxCell>
```
**注：**生命线是浮动边，使用`sourcePoint`/`targetPoint`在`<mxGeometry>`而不是`source`/`target`属性。这是标准的画法。序列图的IO模式。

### ER图

使用`shape=table`容器和`childLayout=tableLayout`。行是带有`portConstraint=eastwest`的`shape=tableRow`单元格。每行中的列是`shape=partialRectangle`。

关系箭头使用`edgeStyle=entityRelationEdgeStyle`：
—一对一：`startArrow=ERone;endArrow=ERone`—一对多：`startArrow=ERone;endArrow=ERmany`—多对多：`startArrow=ERmany;endArrow=ERmany`—必选：`ERmandOne`，可选：`ERzeroToOne`UML类图

类箱是泳道容器。属性和方法是纯文本单元格。分隔器是零高度泳道儿童。

箭头样式按关系类型：

|关系|风格字符串||---|---|
继承（扩展）|`edgeStyle=orthogonalEdgeStyle;html=1;endArrow=block;endFill=0;`|
|实现|`edgeStyle=orthogonalEdgeStyle;dashed=1;html=1;endArrow=block;endFill=0;`|
|合成|`edgeStyle=orthogonalEdgeStyle;html=1;startArrow=diamond;startFill=1;endArrow=none;`|
|汇聚|`edgeStyle=orthogonalEdgeStyle;html=1;startArrow=diamond;startFill=0;endArrow=none;`|
|依赖|`edgeStyle=orthogonalEdgeStyle;dashed=1;html=1;endArrow=open;endFill=0;`|
|关联|`edgeStyle=orthogonalEdgeStyle;html=1;endArrow=open;endFill=0;`|

---

# # 5。多页的图

为复杂系统添加多个`<diagram>`元素：```xml
<mxfile host="Electron" version="26.0.0">
  <diagram id="overview" name="Overview">
    <!-- overview mxGraphModel -->
  </diagram>
  <diagram id="detail" name="Detail View">
    <!-- detail mxGraphModel -->
  </diagram>
</mxfile>
```
每个页面都有自己独立的单元格id名称空间。相同的id值可以出现在不同的页面中而不会发生冲突。

---

# # 6。编辑现有图表

修改已有的`.drawio`文件时：

1. **先读取**文件，以了解现有的单元格id，位置和父层次结构
2. **确定目标图表页面** -通过索引或`name`属性
3. **分配新的唯一id **，不与现有id冲突
4. **尊重容器层次结构**——泳道的子节点使用相对于父节点的坐标
5. **验证边缘** -重新定位节点后，确认边缘source/targetid仍然有效

使用`scripts/add-shape.py`可以在不编辑原始XML的情况下安全地添加单个形状：```bash
python .github/skills/draw-io-diagram-generator/scripts/add-shape.py docs/arch.drawio "New Service" 700 380
```
---

# # 7。最佳实践

* * * *布局
-将形状对齐到10px网格（所有坐标都能被10整除）
-在泳道容器内分组相关形状
-每页一个图表主题；对复杂的系统使用多页文件
-每页40个或更少的单元格，以提高可读性

* * * *标签
-在每个页面的顶部添加标题文本单元格（`text;strokeColor=none;fillColor=none;fontSize=18;fontStyle=1`）
-总是在顶点形状上设置`whiteSpace=wrap;html=1`-保持标签简洁-尽可能每个形状不超过3个字

* * * *风格的一致性
-在整个项目中始终如一地使用第3节第5步中的语义调色板
-清洁直角连接器首选`edgeStyle=orthogonalEdgeStyle`-除非必要，否则不要在标签中插入任意HTML

文件命名* * * *
—使用串式：`order-service-flow.drawio`，`database-schema.drawio`-将图表放在它们所记录的代码旁边：`docs/`或`architecture/`---

# # 8。故障排除

|问题|可能原因|修复||---|---|---|
|文件在VS Code中打开空白|缺少id=0或id=1单元格|在任何其他单元格|之前添加两个根单元格
|形状位置错误|在容器内的子-坐标是相对的|检查`parent`；调整x/y相对于容器|
|边缘不可见|源或目标id与任何顶点不匹配|验证两个id完全存在，写入|
|图显示“压缩”| mxGraphModel是base64编码|在draw中打开。| . io web, File >导出> XML（未压缩）| . io web
|形状样式未呈现|形状= name |中的错字检查`references/shape-libraries.md`以获得确切的样式字符串|
|标签显示转义HTML | HTML =0上的单元格与HTML标签|添加`html=1;`的单元格样式|
|子容器重叠容器边缘|容器高度太小|在mxGeometry中增加容器高度|

---

# # 9。验证清单

在交付任何生成的`.drawio`文件之前，请验证：-[]文件以`<mxfile>`根元素开头
-[]每个`<diagram>`都有一个非空的`id`属性
- []`<mxCell id="0" />`是每个图中的第一个单元格
- []`<mxCell id="1" parent="0" />`是每个图中的第二个单元格
-[]所有单元格`id`值在每个图中都是唯一的
-[]每个顶点单元都有`vertex="1"`和子单元`<mxGeometry as="geometry">`-[]每个边缘单元都有`edge="1"`，并且：(a)`source`/`target`指向现有的顶点id，或者(b)`<mxPoint as="sourcePoint">`和`<mxPoint as="targetPoint">`在其`<mxGeometry>`（浮动边缘-用于序列图生命线）
-[]每个单元格（除了id=0）都有一个`parent`指向一个已经存在的id
- []`html=1`是任何包含HTML标签的标签的样式
- [] XML格式良好（属性值中没有未关闭的标签，没有未转义的`&`，`<`,`>`）
-[]每个页面的顶部都有一个标题标签单元格

运行自动验证器：```bash
python .github/skills/draw-io-diagram-generator/scripts/validate-drawio.py <file.drawio>
```
---

# # 10。输出格式

在交付图表时，始终提供：

1. **将`.drawio`文件**写入请求的路径
2. 用一句话总结图表所显示的内容
3. **如何打开**：
打开`<filename>`在VS Code-平局。IO扩展将自动渲染它。或者你可以在抽屉里打开。IO网页应用程序或桌面应用程序，如果你喜欢的话。”
4. **如何编辑它**（如果用户可能自定义）：
点击任意形状来选择它。双击编辑标签。拖动以重新定位。”
5. **验证状态** -验证脚本是否运行并通过

---

# # 11。参考文献

所有的配套文件都在`.github/skills/draw-io-diagram-generator/`：

|文件|内容||---|---|
|`references/drawio-xml-schema.md`|完整的mxfile / mxGraphModel / mxCell属性参考，坐标系，保留单元格，验证规则|
|`references/style-reference.md`|所有允许值的样式键，顶点和边缘样式键，形状目录，语义调色板|
|`references/shape-libraries.md`|具有样式字符串|的所有形状库类别（通用，流程图，UML， ER，网络，BPMN, Mockup, k8）
|`assets/templates/flowchart.drawio`|准备使用流程图模板|
|`assets/templates/architecture.drawio`|四层系统架构模板|
|`assets/templates/sequence.drawio`| 3-actor序列图模板|
带鱼尾纹关系的三表ER图
|`assets/templates/uml-class.drawio`|接口+ 2个类+带关系箭头的enum |
|`scripts/validate-drawio.py`| Python脚本验证任何XML结构。绘制文件|
|`scripts/add-shape.py`| Python CLI添加一个新的形状到现有的图表|
|`scripts/README.md`|带样例脚本使用说明|