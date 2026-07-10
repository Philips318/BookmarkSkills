#画。io形状库

所有内置形状库的参考指南。在绘图中通过`View > Shapes`启用。IO编辑器（或VS Code扩展形状面板）。

---

##图书馆目录

# # #一般

**启用**：默认始终激活

任何图表类型的通用形状。

|形状|风格键|使用|| ------- | ----------- | --------- |
| *（默认）* |盒子，步骤，组件|
|圆角矩形|`rounded=1;`|软处理盒|
|椭圆|`ellipse;`|状态，start/end|
|三角形|`triangle;`|箭头，门|
|钻石|`rhombus;`|决定|
|六边形|`shape=hexagon;`|标签，科技图标|
|云|`shape=cloud;`|云服务|
|气缸|`shape=cylinder3;`|数据库|
|注释|`shape=note;`|注释|
|文档|`shape=document;`|文件|
|箭头形状|各种`mxgraph.arrows2.*`|流动方向|
|标注|`shape=callout;`|语音泡泡|

---

# # #流程图

* *让* *:`View > Shapes > Flowchart`**形状前缀**:`mxgraph.flowchart.`标准ANSI/ISO流程图符号。

|符号|风格字符串| ANSI名称|| -------- | ------------- | ----------- |
|开始/结束|`ellipse;`|终端|
|进程（矩形）|`rounded=1;`| |进程
|决定|`rhombus;`|决定|
|I/O（平行四边形）|`shape=mxgraph.flowchart.io;`|数据|
|预定义进程|`shape=mxgraph.flowchart.predefined_process;`|预定义进程|
|手动操作|`shape=mxgraph.flowchart.manual_operation;`|手动操作|
|手动输入|`shape=mxgraph.flowchart.manual_input;`|手动输入|
|数据库|`shape=mxgraph.flowchart.database;`|直连存储|
|文档|`shape=mxgraph.flowchart.document;`|文档|
|多个文档|`shape=mxgraph.flowchart.multi-document;`|多个文档|
|页上连接器|`ellipse;`（小，30×30） |连接器|
|页外连接器|`shape=mxgraph.flowchart.off_page_connector;`|页外连接器|
|准备|`shape=mxgraph.flowchart.preparation;`|准备|
|延迟|`shape=mxgraph.flowchart.delay;`|延迟|
|显示|`shape=mxgraph.flowchart.display;`|显示|
|内部存储|`shape=mxgraph.flowchart.internal_storage;`|内部存储|
|排序|`shape=mxgraph.flowchart.sort;`|排序|
|提取|`shape=mxgraph.flowchart.extract;`|提取|
|合并|`shape=mxgraph.flowchart.merge;`|合并|
|或|`shape=mxgraph.flowchart.or;`|或|
|注释|`shape=mxgraph.flowchart.annotation;`|注释|
|卡|`shape=mxgraph.flowchart.card;`|打孔卡|**完整的流程图示例样式字符串：**```text
Process:          rounded=1;whiteSpace=wrap;html=1;
Decision:         rhombus;whiteSpace=wrap;html=1;
Start/End:        ellipse;whiteSpace=wrap;html=1;
Database:         shape=mxgraph.flowchart.database;whiteSpace=wrap;html=1;
Document:         shape=mxgraph.flowchart.document;whiteSpace=wrap;html=1;
I/O (Data):       shape=mxgraph.flowchart.io;whiteSpace=wrap;html=1;
```
---

# # # UML

* *让* *:`View > Shapes > UML`####用例图

|形状|风格字符串|| ------- | ------------- |
|演员|`shape=mxgraph.uml.actor;whiteSpace=wrap;html=1;`|
|用例（椭圆）|`ellipse;whiteSpace=wrap;html=1;`|
|系统边界|`swimlane;startSize=30;whiteSpace=wrap;html=1;`|

####类图

使用泳道容器存放班级箱子；```xml
<!-- Class container -->
<mxCell value="«interface»&#xa;IOrderService" 
        style="swimlane;fontStyle=1;align=center;startSize=30;whiteSpace=wrap;html=1;"
        vertex="1" parent="1">
  <mxGeometry x="200" y="100" width="200" height="160" as="geometry" />
</mxCell>

<!-- Attributes (child of class) -->
<mxCell value="+ id: string&#xa;+ status: string" 
        style="text;strokeColor=none;fillColor=none;align=left;verticalAlign=top;spacingLeft=4;overflow=hidden;html=1;"
        vertex="1" parent="classId">
  <mxGeometry y="30" width="200" height="60" as="geometry" />
</mxCell>

<!-- Method separator line -->
<mxCell value="" style="line;strokeWidth=1;fillColor=none;" vertex="1" parent="classId">
  <mxGeometry y="90" width="200" height="10" as="geometry" />
</mxCell>

<!-- Methods (child of class) -->
<mxCell value="+ create(): Order&#xa;+ cancel(): void"
        style="text;strokeColor=none;fillColor=none;align=left;verticalAlign=top;spacingLeft=4;overflow=hidden;html=1;"
        vertex="1" parent="classId">
  <mxGeometry y="100" width="200" height="60" as="geometry" />
</mxCell>
```
#### UML关系箭头

|关系|风格字符串|| ------------- | ------------- |
继承（扩展）|`edgeStyle=orthogonalEdgeStyle;html=1;endArrow=block;endFill=0;`|
| Implementation (implements) |`edgeStyle=orthogonalEdgeStyle;dashed=1;html=1;endArrow=block;endFill=0;`|
|关联|`edgeStyle=orthogonalEdgeStyle;html=1;endArrow=open;endFill=0;`|
|依赖|`edgeStyle=orthogonalEdgeStyle;dashed=1;html=1;endArrow=open;endFill=0;`|
|汇聚|`edgeStyle=orthogonalEdgeStyle;html=1;startArrow=diamond;startFill=0;endArrow=none;`|
|合成|`edgeStyle=orthogonalEdgeStyle;html=1;startArrow=diamond;startFill=1;endArrow=none;`|

####组件图

|形状|风格字符串|| ------- | ------------- |
|组件|`shape=component;align=left;spacingLeft=36;whiteSpace=wrap;html=1;`|
|接口（棒棒糖）|`ellipse;whiteSpace=wrap;html=1;aspect=fixed;`（小圆圈）|
|端口|`shape=mxgraph.uml.port;`|
|节点|`shape=mxgraph.uml.node;whiteSpace=wrap;html=1;`|
|工件|`shape=mxgraph.uml.artifact;whiteSpace=wrap;html=1;`|

####顺序图

|形状|风格字符串|| ------- | ------------- |
|演员|`shape=mxgraph.uml.actor;whiteSpace=wrap;html=1;`|
|生命线（对象）|`shape=umlLifeline;startSize=40;whiteSpace=wrap;html=1;`|
|激活箱|`shape=umlActivation;whiteSpace=wrap;html=1;`|
|同步消息|`edgeStyle=elbowEdgeStyle;elbow=vertical;html=1;endArrow=block;endFill=1;`|
|异步消息|`edgeStyle=elbowEdgeStyle;elbow=vertical;html=1;endArrow=open;endFill=0;`|
|返回|`edgeStyle=elbowEdgeStyle;elbow=vertical;dashed=1;html=1;endArrow=open;endFill=0;`|
|自称|`edgeStyle=elbowEdgeStyle;elbow=vertical;exitX=1;exitY=0.3;entryX=1;entryY=0.5;html=1;`|

####状态图

|形状|风格字符串|| ------- | ------------- |
初始状态（实圆）|`ellipse;html=1;aspect=fixed;fillColor=#000000;strokeColor=#000000;`|
|状态|`rounded=1;whiteSpace=wrap;html=1;arcSize=50;`|
|最终状态|`shape=doubleEllipse;fillColor=#000000;strokeColor=#000000;`|
|转换|`edgeStyle=orthogonalEdgeStyle;html=1;endArrow=block;endFill=1;`|
|Fork/Join|`shape=mxgraph.uml.fork_or_join;html=1;fillColor=#000000;`|

---

实体关系（ER图）

* *让* *:`View > Shapes > Entity Relation`####现代ER表（鱼尾纹表示法）```xml
<!-- Table container -->
<mxCell id="tbl-orders" value="orders"
        style="shape=table;startSize=30;container=1;collapsible=1;childLayout=tableLayout;fillColor=#dae8fc;strokeColor=#6c8ebf;fontStyle=1;"
        vertex="1" parent="1">
  <mxGeometry x="80" y="80" width="240" height="210" as="geometry" />
</mxCell>

<!-- Column row -->
<mxCell id="col-id" value=""
        style="shape=tableRow;horizontal=0;startSize=0;swimmilaneHead=0;swimlaneBody=0;fillColor=none;collapsible=0;dropTarget=0;points=[[0,0.5],[1,0.5]];portConstraint=eastwest;"
        vertex="1" parent="tbl-orders">
  <mxGeometry y="30" width="240" height="30" as="geometry" />
</mxCell>

<!-- PK marker cell -->
<mxCell value="PK" style="shape=partialRectangle;connectable=0;fillColor=none;top=0;left=0;bottom=0;right=0;fontStyle=1;overflow=hidden;"
        vertex="1" parent="col-id">
  <mxGeometry width="40" height="30" as="geometry" />
</mxCell>

<!-- Column name cell -->
<mxCell value="id" style="shape=partialRectangle;connectable=0;fillColor=none;top=0;left=0;bottom=0;right=0;overflow=hidden;"
        vertex="1" parent="col-id">
  <mxGeometry x="40" width="140" height="30" as="geometry" />
</mxCell>

<!-- Data type cell -->
<mxCell value="UUID" style="shape=partialRectangle;connectable=0;fillColor=none;top=0;left=0;bottom=0;right=0;overflow=hidden;fontStyle=2;"
        vertex="1" parent="col-id">
  <mxGeometry x="180" width="60" height="30" as="geometry" />
</mxCell>
```
#### ER关系连接器（鱼尾纹）

|基数|样式字符串|| ------------- | ------------- |
|一对一|`edgeStyle=entityRelationEdgeStyle;html=1;startArrow=ERmandOne;endArrow=ERmandOne;startFill=1;endFill=1;`|
|一对多|`edgeStyle=entityRelationEdgeStyle;html=1;startArrow=ERmandOne;endArrow=ERmany;startFill=1;endFill=1;`|
0到多|`edgeStyle=entityRelationEdgeStyle;html=1;startArrow=ERmandOne;endArrow=ERzeroToMany;startFill=1;endFill=0;`|
|到1 |`edgeStyle=entityRelationEdgeStyle;html=1;startArrow=ERmandOne;endArrow=ERzeroToOne;startFill=1;endFill=0;`|
|多对多|`edgeStyle=entityRelationEdgeStyle;html=1;startArrow=ERmany;endArrow=ERmany;startFill=1;endFill=1;`|

---

网络/基础设施

* *让* *:`View > Shapes > Networking`|形状|风格字符串|| ------- | ------------- |
|通用服务器|`shape=server;html=1;whiteSpace=wrap;`|
| Web服务器|`shape=mxgraph.network.web_server;`|
|数据库服务器|`shape=mxgraph.network.database;`|
|笔记本|`shape=mxgraph.network.laptop;`|
|桌面|`shape=mxgraph.network.desktop;`|
|手机|`shape=mxgraph.network.mobile;`|
|路由器|`shape=mxgraph.cisco.routers.router;`|
|开关|`shape=mxgraph.cisco.switches.workgroup_switch;`|
|防火墙|`shape=mxgraph.cisco.firewalls.firewall;`|
| Cloud (generic) |`shape=cloud;`|
|互联网|`shape=mxgraph.network.internet;`|
|负载均衡器|`shape=mxgraph.network.load_balancer;`|

---

BPMN 2.0

* *让* *:`View > Shapes > BPMN`**形状前缀**:`shape=mxgraph.bpmn.*`|形状|风格字符串|| ------- | ------------- |
|启动事件|`shape=mxgraph.bpmn.shape;perimeter=mxPerimeter.ellipsePerimeter;symbol=general;verticalLabelPosition=bottom;`|
|结束事件|`shape=mxgraph.bpmn.shape;perimeter=mxPerimeter.ellipsePerimeter;symbol=terminate;verticalLabelPosition=bottom;`|
|任务|`shape=mxgraph.bpmn.shape;perimeter=mxPerimeter.rectanglePerimeter;symbol=task;`|
|独占网关|`shape=mxgraph.bpmn.shape;perimeter=mxPerimeter.rhombusPerimeter;symbol=exclusiveGw;`|
|并行网关|`shape=mxgraph.bpmn.shape;perimeter=mxPerimeter.rhombusPerimeter;symbol=parallelGw;`|
|子进程|`shape=mxgraph.bpmn.shape;perimeter=mxPerimeter.rectanglePerimeter;symbol=subProcess;`|
|序列流|`edgeStyle=orthogonalEdgeStyle;html=1;endArrow=block;endFill=1;`|
|消息流|`edgeStyle=orthogonalEdgeStyle;dashed=1;html=1;endArrow=block;endFill=0;`|
|池|`shape=pool;startSize=30;horizontal=1;`|
| Lane |`swimlane;startSize=30;`|

---

模型/线框图

* *让* *:`View > Shapes > Mockup`|形状|风格字符串|| ------- | ------------- |
|按钮|`shape=mxgraph.mockup.forms.button;`|
|输入字段|`shape=mxgraph.mockup.forms.text1;`|
|复选框|`shape=mxgraph.mockup.forms.checkbox;`|
|下拉|`shape=mxgraph.mockup.forms.comboBox;`|
浏览器窗口|`shape=mxgraph.mockup.containers.browser;`|
|手机屏幕|`shape=mxgraph.mockup.containers.smartphone;`|
|列出|`shape=mxgraph.mockup.containers.list;`|
表|`shape=mxgraph.mockup.containers.table;`|

---

# # # Kubernetes

* *让* *:`View > Shapes > Kubernetes`|资源|样式字符串|| ---------- | ------------- |
| Pod |`shape=mxgraph.kubernetes.pod;`|
|部署|`shape=mxgraph.kubernetes.deploy;`|
|业务|`shape=mxgraph.kubernetes.svc;`|
|入口|`shape=mxgraph.kubernetes.ing;`|
| ConfigMap |`shape=mxgraph.kubernetes.cm;`|
|秘密|`shape=mxgraph.kubernetes.secret;`|
| PersistentVolume |`shape=mxgraph.kubernetes.pv;`|
|命名空间|`shape=mxgraph.kubernetes.ns;`|
|节点|`shape=mxgraph.kubernetes.node;`|

---

##启用VS Code中的库

库在绘制中启用。IO编辑器（VS Code嵌入）：

1. 在VS Code中打开任何`.drawio`或`.drawio.svg`文件
2. 单击形状面板（左侧栏）中的`+`图标→`Search Shapes`或`More Shapes`3. 选中要激活的库
4. 形状出现在面板中，用于拖放

库按用户存储在draw中。IO设置（不是每个项目）。

---

自定义形状库创建

自定义库是一个扩展名为`.xml`的XML文件，通过`File > Open Library`加载。```xml
<mxlibrary>
  [
    {
      "xml": "&lt;mxCell value=\"Component\" style=\"rounded=1;whiteSpace=wrap;html=1;fillColor=#dae8fc;\" vertex=\"1\"&gt;&lt;mxGeometry width=\"120\" height=\"60\" as=\"geometry\" /&gt;&lt;/mxCell&gt;",
      "w": 120,
      "h": 60,
      "aspect": "fixed",
      "title": "My Component"
    }
  ]
</mxlibrary>
```
每个形状条目包含：
-`xml`: xml转义的单元格定义
—`w`/`h`：默认为width/height—`aspect`:`"fixed"`对锁比
—`title`：在面板中显示的名称    }
  ]
</mxlibrary>
```
