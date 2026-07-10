#画。io风格参考`<mxCell>`元素上的`style`属性的完整引用。样式是用分号分隔的`key=value`对。

---

## Style格式```text
style="key1=value1;key2=value2;key3=value3;"
```
—键和值区分大小写
—可选，但推荐使用分号
—默认忽略未知键
-丢失的键使用draw。io违约

---

通用样式键

适用于所有形状和边缘。

|关键字|值|默认值|描述|| ----- | -------- | --------- | ------------- |
|`fillColor`|`#hex`/`none`|`#FFFFFF`|形状填充颜色(绘制。io违约;为项目图使用语义调色板)|
|`strokeColor`|`#hex`/`none`|`#000000`|Border/line颜色（绘制）io违约;为项目图使用语义调色板)|
|`fontColor`|`#hex`|`#000000`|文本颜色|
|`fontSize`| integer |`11`| pt字体大小|
|`fontStyle`|位掩码（见下文）|`0`|Bold/italic/underline|
|`fontFamily`|字符串|`Helvetica`|字体家族名称|
|`align`|`left`/`center`/`right`|`center`|水平文本对齐|
|`verticalAlign`|`top`/`middle`/`bottom`|`middle`|垂直文本对齐|
|`opacity`| 0-100 |`100`|形状不透明度(%
|`shadow`|`0`/`1`|`0`|投影|
|`dashed`|`0`/`1`|`0`|虚线边框|
|`dashPattern`|例如`8 8`| - |自定义dash/gap模式（px） |
|`strokeWidth`|浮动|`2`|Border/line宽度在px |
|`spacing`|整数|`2`|填充文本（px） |
|`spacingTop`|整数|`0`|顶部文本填充|
|`spacingBottom`|整数|`0`|底部文本填充|
|`spacingLeft`|整数|`4`|左文本填充|
|`spacingRight`|整数|`4`|右文本填充|
|`html`|`0`/`1`|`0`|允许标签|中的HTML
|`whiteSpace`|`wrap`/`nowrap`|`nowrap`|文本换行|
|`overflow`|`visible`/`hidden`/`fill`|`visible`|文本溢出行为|
|`rotatable`|`0`/`1`|`1`|允许在编辑器|中旋转
|`movable`|`0`/`1`|`1`|允许在编辑器|中移动
|`resizable`|`0`/`1`|`1`|允许在编辑器|中调整大小
|`deletable`|`0`/`1`|`1`|允许在|编辑器中删除
|`editable`|`0`/`1`|`1`|允许在编辑器|中编辑标签
|`locked`|`0`/`1`|`0`|锁定所有编辑|
|`nolabel`|`0`/`1`|`0`|完全隐藏标签|
|`noLabel`|`0`/`1`|`0`|`nolabel`|的别名
|`labelPosition`|`left`/`center`/`right`|`center`|标签锚定水平|
|`verticalLabelPosition`|`top`/`middle`/`bottom`|`middle`|标签锚垂直|
|`imageAlign`|`left`/`center`/`right`|`center`|图像对齐|`fontStyle`位掩码值

|值|效果|| ------- | -------- |
|`0`|正常|
|`1`| Bold |
|`2`|斜体|
|`4`|下划线|
|`8`|穿透|

通过加法组合：`3`=粗体+斜体，`5`=粗体+下划线，`7`=粗体+斜体+下划线。

---

形状键（仅限顶点）

|关键字|值|描述信息|| ----- | -------- | ------------- |
|`shape`|见形状目录|覆盖默认矩形形状|
|`rounded`|`0`/`1`|矩形|上的圆角
|`arcSize`| 0-50 |角半径%（当`rounded=1`） |
|`perimeter`|功能名称|连接周长类型|
|`aspect`|`fixed`|锁定宽高比在调整|
|`rotation`|浮动|旋转度|
|`fixedSize`|`0`/`1`|防止编辑标签|时自动大小
|`container`|`0`/`1`|视形状为儿童容器|
|`collapsible`|`0`/`1`|允许collapse/expand切换|
|`startSize`| integer |swimlane/container(px) |的标头大小
|`swimlaneHead`|`0`/`1`|显示泳道头|
|`swimlaneBody`|`0`/`1`|显示泳道体|
|`fillOpacity`| 0-100 |仅填充不透明度（独立于`opacity`） |
|`strokeOpacity`| 0-100 |只描边不透明度|
|`gradientColor`|`#hex`/`none`|渐变结束色|
|`north`/`south`/`east`/`west`|梯度方向|
|`sketch`|`0`/`1`|粗糙的手绘风格|
|`comic`|`0`/`1`|Comic/cartoonline style |
|`glass`|`0`/`1`|玻璃反射效果|---

##形状目录

基本形状

|形状|风格字符串|视觉|| ------- | ------------- | -------- |
| *（不需要形状键）* |□|
|圆角矩形|`rounded=1;`|
|椭圆/圆形|`ellipse;`|〇|
|钻石|`rhombus;`|◇|
|三角形|`triangle;`|△|
|六边形|`shape=hexagon;`|⬡|
|五角大楼|`shape=mxgraph.basic.pentagon;`|⬠|
| * |`shape=mxgraph.basic.star;`|★|
|交叉|`shape=mxgraph.basic.x;`|✕|
|云|`shape=cloud;`|
|注释/标注|`shape=note;folded=1;`|📝|
|文档|`shape=document;`|📄|
|气缸（数据库）|`shape=cylinder3;`|🗄|
|磁带|`shape=tape;`| - |
|平行四边形|`shape=parallelogram;perimeter=parallelogramPerimeter;`|可写|

###流程图形状（`mxgraph.flowchart.*`）

|形状|样式字符串|用于|| ------- | ------------- | ---------- |
|进程|`shape=mxgraph.flowchart.process;`|标准进程|
|Start/End（终端）|`ellipse;`或`shape=mxgraph.flowchart.terminate;`|流量start/end|
|决策|`rhombus;`|Yes/No分支|
| Data (I/O) |`shape=mxgraph.flowchart.io;`|Input/Output|
|预定义进程|`shape=mxgraph.flowchart.predefined_process;`|子程序|
|手动输入|`shape=mxgraph.flowchart.manual_input;`|手动输入|
|手动操作|`shape=mxgraph.flowchart.manual_operation;`|手动步骤|
|数据库|`shape=mxgraph.flowchart.database;`|数据存储|
|内部存储|`shape=mxgraph.flowchart.internal_storage;`|内部数据|
|直接数据|`shape=mxgraph.flowchart.direct_data;`|磁盘存储|
|文档|`shape=mxgraph.flowchart.document;`|文档|
|多文档|`shape=mxgraph.flowchart.multi-document;`|多文档|
|页上连接器|`ellipse;`（小）|页上连接器|
|离页连接器|`shape=mxgraph.flowchart.off_page_connector;`|离页ref |
|准备|`shape=mxgraph.flowchart.preparation;`|初始化|
|延迟|`shape=mxgraph.flowchart.delay;`|等待状态|
|显示|`shape=mxgraph.flowchart.display;`|显示|
|排序|`shape=mxgraph.flowchart.sort;`|排序操作|
|提取|`shape=mxgraph.flowchart.extract;`|提取操作|
|合并|`shape=mxgraph.flowchart.merge;`|合并路径|
|或|`shape=mxgraph.flowchart.or;`|或门|
和|`shape=mxgraph.flowchart.and;`|与栅极|
|`shape=mxgraph.flowchart.annotation;`|Comment/note|UML形状（`mxgraph.uml.*`）

|形状|样式字符串|用于|| ------- | ------------- | ---------- |
|参与者|`shape=mxgraph.uml.actor;`|用例参与者|
|边界|`shape=mxgraph.uml.boundary;`|系统边界|
|控制器|`shape=mxgraph.uml.control;`|控制器对象|
|实体|`shape=mxgraph.uml.entity;`|实体对象|
|组件|`shape=component;`|组件盒|
| Package |`shape=mxgraph.uml.package;`| Package |
|注释|`shape=note;`| UML注释|
|生命线|`shape=umlLifeline;startSize=40;`|序列生命线|
|激活|`shape=umlActivation;`|激活盒|
|销毁|`shape=mxgraph.uml.destroy;`|销毁标记|
|状态|`ellipse;`|状态节点|
|初始状态|`ellipse;fillColor=#000000;`| UML初始状态|
|最终状态|`shape=doubleEllipse;fillColor=#000000;`| UML最终状态|
|Fork/Join|`shape=mxgraph.uml.fork_or_join;`|Fork/joinbar |

网络形状（`mxgraph.network.*`）

|形状|风格字符串|| ------- | ------------- |
|服务器|`shape=server;`|
|数据库服务器|`shape=mxgraph.network.database;`|
|防火墙|`shape=mxgraph.cisco.firewalls.firewall;`|
|路由器|`shape=mxgraph.cisco.routers.router;`|
|交换机|`shape=mxgraph.cisco.switches.workgroup_switch;`|
|云|`shape=cloud;`|
|互联网|`shape=mxgraph.network.internet;`|
|笔记本|`shape=mxgraph.network.laptop;`|
|桌面|`shape=mxgraph.network.desktop;`|
|移动|`shape=mxgraph.network.mobile;`|

AWS形状（`mxgraph.aws4.*`）

使用AWS4库。常见的形状:

|形状|风格字符串|| ------- | ------------- |
| EC2 |`shape=mxgraph.aws4.resourceIcon;resIcon=mxgraph.aws4.ec2;`|
| |`shape=mxgraph.aws4.resourceIcon;resIcon=mxgraph.aws4.lambda;`|
| S3 |`shape=mxgraph.aws4.resourceIcon;resIcon=mxgraph.aws4.s3;`|
| RDS |`shape=mxgraph.aws4.resourceIcon;resIcon=mxgraph.aws4.rds;`|
| API网关|`shape=mxgraph.aws4.resourceIcon;resIcon=mxgraph.aws4.api_gateway;`|
| CloudFront |`shape=mxgraph.aws4.resourceIcon;resIcon=mxgraph.aws4.cloudfront;`|
|负载均衡器|`shape=mxgraph.aws4.resourceIcon;resIcon=mxgraph.aws4.elb;`|
| SQS |`shape=mxgraph.aws4.resourceIcon;resIcon=mxgraph.aws4.sqs;`|
| SNS |`shape=mxgraph.aws4.resourceIcon;resIcon=mxgraph.aws4.sns;`|
| DynamoDB |`shape=mxgraph.aws4.resourceIcon;resIcon=mxgraph.aws4.dynamodb;`|
| ECS |`shape=mxgraph.aws4.resourceIcon;resIcon=mxgraph.aws4.ecs;`|
|`shape=mxgraph.aws4.resourceIcon;resIcon=mxgraph.aws4.eks;`|
| VPC |`shape=mxgraph.aws4.group;grIcon=mxgraph.aws4.group_vpc;`|
|区域|`shape=mxgraph.aws4.group;grIcon=mxgraph.aws4.group_region;`|

### Azure形状（`mxgraph.azure.*`）

|形状|风格字符串|| ------- | ------------- |
|应用服务|`shape=mxgraph.azure.app_service;`|
|功能App |`shape=mxgraph.azure.function_apps;`|
| SQL数据库|`shape=mxgraph.azure.sql_database;`|
| Blob存储|`shape=mxgraph.azure.blob_storage;`|
| API管理|`shape=mxgraph.azure.api_management;`|
|服务总线|`shape=mxgraph.azure.service_bus;`|
| AKS |`shape=mxgraph.azure.aks;`|
|容器注册表|`shape=mxgraph.azure.container_registry_registries;`|

### GCP形状（`mxgraph.gcp2.*`）

|形状|风格字符串|| ------- | ------------- |
|云运行|`shape=mxgraph.gcp2.cloud_run;`|
|云功能|`shape=mxgraph.gcp2.cloud_functions;`|
|云SQL |`shape=mxgraph.gcp2.cloud_sql;`|
|云存储|`shape=mxgraph.gcp2.cloud_storage;`|
| GKE |`shape=mxgraph.gcp2.container_engine;`|
|Pub/Sub|`shape=mxgraph.gcp2.cloud_pubsub;`|
| BigQuery |`shape=mxgraph.gcp2.bigquery;`|

---

边缘样式键

|关键字|值|描述信息|| ----- | -------- | ------------- |
|`edgeStyle`|见下文|连接路由算法|
|`rounded`|`0`/`1`|直角边的圆角|
|`curved`|`0`/`1`|曲线段|
|`orthogonal`|`0`/`1`|强制正交路由|
|`jettySize`|`auto`/整数|Source/target喷射尺寸|
|`exitX`| 0.0-1.0 |源出口点X（0=左，0.5=中，1=右）|
|`exitY`| 0.0-1.0 |源出口点Y（0=顶部，0.5=中心，1=底部）|
|`exitDx`|浮动|源出口X偏移量（px） |
|`exitDy`| float |源出口Y偏移量（px） |
|`entryX`| 0.0-1.0 |目标入口点X |
|`entryY`| 0.0-1.0 |目标入口点Y |
|`entryDx`| float |目标条目X偏移量（px） |
|`entryDy`|浮动|目标条目Y偏移量（px） |
|`endArrow`|见箭头类型|箭头指向目标|
|`startArrow`|见箭头类型|箭头尾部在源|
|`endFill`|`0`/`1`|填充端箭头|
|`startFill`|`0`/`1`|填充起始箭头|
|`endSize`| integer |结束箭头头大小（px） |
|`startSize`| integer |起始箭头大小（px） |
|`labelBackgroundColor`|`#hex`/`none`|标签背景填充|
|`labelBorderColor`|`#hex`/`none`|标签边框颜色|`edgeStyle`值

|值|路由|当|时使用| ------- | --------- | ---------- |
|`none`|直线|简单直连|
|`orthogonalEdgeStyle`|直角转弯|流程图，架构|
|`elbowEdgeStyle`|单弯头|清洁方向图|
| ER式路由| ER图|
|`segmentEdgeStyle`|分段处理|微调路由|
|`isometricEdgeStyle`|等距网格|等距图|

箭头类型（`endArrow`/`startArrow`）

|值|形状|用于|| ------- | ------- | --------- |
|`block`|填充三角形|标准定向箭头|
|`open`|打开chevron→|Open/light箭头|
|`classic`|经典箭头|默认绘制。IO箭头|
|`classicThin`|薄经典|紧凑图|
|`none`|无箭头|无向线|
|`oval`|圆点|聚合开始|
|`diamond`|空心金刚石|聚集|
|`diamondThin`|细菱形|细图|
|`ERone`|`\|`bar | ER基数“一”|
|`ERmany`|乌鸦脚| ER基数“许多”|
|`ERmandOne`|`\|\|`| ER必选1 |
|`ERzeroToOne`|`o\|`| ER 0或1 |
|`ERzeroToMany`|`o<`| ER零或多|
|`ERoneToMany`|`\|<`| ER单对多|

---

##调色板

语义颜色（建议用于一致性图）

|含义|填充|行程|用途|| --------- | ------ | -------- | ------- |
|用户/客户端|`#dae8fc`|`#6c8ebf`|浏览器、客户端应用|
|服务/进程|`#d5e8d4`|`#82b366`|后端服务|
|数据库/存储|`#f5f5f5`|`#666666`|数据库，文件|
|决策/警告|`#fff2cc`|`#d6b656`|决策节点，警报|
|错误/紧急|`#f8cecc`|`#b85450`|路径错误，紧急|
|外部/合作伙伴|`#e1d5e7`|`#9673a6`|第三方，外部|
|队列/异步|`#ffe6cc`|`#d79b00`|消息队列|
|网关/代理|`#dae8fc`|`#0050ef`| API网关，代理|

黑色背景形状

对于深色主题的图表，切换到：

-填充：`#1e4d78`（深蓝色），`#1a4731`（墨绿色）
—行程：`#4aa3df`、`#67ab9f`—字体：`#ffffff`---

完成风格示例

圆形蓝盒子```text
rounded=1;whiteSpace=wrap;html=1;fillColor=#dae8fc;strokeColor=#6c8ebf;
```
###绿色进程步骤```text
rounded=1;whiteSpace=wrap;html=1;fillColor=#d5e8d4;strokeColor=#82b366;
```
黄色决策钻石```text
rhombus;whiteSpace=wrap;html=1;fillColor=#fff2cc;strokeColor=#d6b656;
```
红色错误框```text
rounded=1;whiteSpace=wrap;html=1;fillColor=#f8cecc;strokeColor=#b85450;
```
数据库圆柱体```text
shape=cylinder3;whiteSpace=wrap;html=1;boundedLbl=1;backgroundOutline=1;fillColor=#f5f5f5;strokeColor=#666666;
```
泳道容器```text
shape=pool;startSize=30;horizontal=1;fillColor=#f5f5f5;strokeColor=#999999;
```
泳道```text
swimlane;startSize=30;fillColor=#ffffff;strokeColor=#999999;
```
正交连接器```text
edgeStyle=orthogonalEdgeStyle;rounded=0;html=1;
```
###方向箭头（粗体）```text
edgeStyle=orthogonalEdgeStyle;rounded=0;html=1;endArrow=block;endFill=1;strokeWidth=2;
```
虚线依赖线```text
edgeStyle=orthogonalEdgeStyle;dashed=1;endArrow=open;endFill=0;strokeColor=#666666;
```
ER关系线（一对多）```text
edgeStyle=entityRelationEdgeStyle;html=1;endArrow=ERmany;startArrow=ERmandOne;endFill=1;startFill=1;
```
UML继承箭头（中空三角形）```text
edgeStyle=orthogonalEdgeStyle;html=1;endArrow=block;endFill=0;
```
UML合成（填充菱形）```text
edgeStyle=orthogonalEdgeStyle;html=1;startArrow=diamond;startFill=1;endArrow=none;
```
UML聚合（开放菱形）```text
edgeStyle=orthogonalEdgeStyle;html=1;startArrow=diamond;startFill=0;endArrow=none;
```
UML依赖关系（虚线箭头）```text
edgeStyle=orthogonalEdgeStyle;dashed=1;html=1;endArrow=open;endFill=0;
```
隐形连接器（用于对齐）```text
edgeStyle=none;strokeColor=none;endArrow=none;
```
