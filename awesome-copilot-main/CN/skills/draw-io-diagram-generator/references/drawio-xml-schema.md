#画。io XML模式参考`.drawio`文件格式（mxGraph XML）的完整参考。在生成、解析或验证图文件时使用此方法。

---

顶层结构

每个`.drawio`文件都是XML的根结构：```xml
<!-- Set modified to the current ISO 8601 timestamp when generating a new file -->
<mxfile host="Electron" modified=""
        agent="draw.io" version="26.0.0" type="device">
  <diagram id="<unique-id>" name="<Page Name>">
    <mxGraphModel ...attributes...>
      <root>
        <mxCell id="0" />
        <mxCell id="1" parent="0" />
        <!-- All content cells here -->
      </root>
    </mxGraphModel>
  </diagram>
</mxfile>
```
###`<mxfile>`属性

|属性|必选|默认|描述|| ----------- | ---------- | --------- | ------------- |
|`host`|不|`"app.diagrams.net"`|起源编辑器（`"Electron"`为desktop/VS代码）|
|`modified`|否| - | ISO 8601时间戳|
|`agent`|否| - |用户代理字符串|
|`version`|不| - |抽。IO版本|
|`type`|否|`"device"`|存储类型|

###`<diagram>`属性

|属性|必选|描述|| ----------- | ---------- | ------------- |
|`id`|是|唯一页面标识符（任意字符串）|
|`name`|是|编辑器|中显示的标签`<mxGraphModel>`属性

|属性|类型|默认值|描述|| ----------- | ------ | --------- | ------------- |
|`dx`| int |`1422`|滚动X偏移|
|`dy`| int |`762`|滚动Y偏移|
|`grid`|`0`/`1`|`1`|显示网格|
|`gridSize`| int |`10`|网格快照大小在px |
|`guides`|`0`/`1`|`1`|显示对齐导轨|
|`tooltips`|`0`/`1`|`1`|启用工具提示|
|`connect`|`0`/`1`|`1`|启用悬停|上的连接箭头
|`arrows`|`0`/`1`|`1`|显示方向箭头|
|`fold`|`0`/`1`|`1`|启用组fold/collapse|
|`page`|`0`/`1`|`1`|显示页面边界|
|`pageScale`|浮动|`1`|页面缩放比例|
|`pageWidth`| int |`1169`|页面宽度以px为单位（A4横向）|
|`pageHeight`| int |`827`|页面高度，单位为px (A4横向
|`math`|`0`/`1`|`0`|启用LaTeX数学渲染|
|`shadow`|`0`/`1`| xqz48x形状|的全局阴影**常用页面大小（px在96dpi）：**

|格式|宽度|高度|| -------- | ------- | -------- |
| A4景观|`1169`|`827`|
| A4纵向|`827`|`1169`|
| A3景观|`1654`|`1169`|
|字母横向|`1100`|`850`|
|字母肖像|`850`|`1100`|
|屏幕（16:9）|`1654`|`931`|

---

##保留单元格（总是必需的）```xml
<mxCell id="0" />                 <!-- Root cell — never omit, never add attributes -->
<mxCell id="1" parent="0" />     <!-- Default layer — all cells are children of this -->
```
这两个单元格必须是`<root>`中的第一个条目。id`0`和`1`是保留的，不能用于任何其他单元格。

---

顶点（形状）元素```xml
<mxCell
  id="2"
  value="Label Text"
  style="rounded=1;whiteSpace=wrap;html=1;"
  vertex="1"
  parent="1">
  <mxGeometry x="200" y="160" width="120" height="60" as="geometry" />
</mxCell>
```
`<mxCell>`顶点属性

|属性|必选|类型|描述|| ----------- | ---------- | ------ | ------------- |
|`id`|是|字符串|此图中的唯一标识符|
|`value`|是|字符串|标签文本（HTML允许，如果样式有`html=1`） |
|`style`|是| string |分号分隔键=值样式字符串|
|`vertex`|是|`"1"`|必须是`"1"`来声明这个形状|
|`parent`|是| string |父单元格ID（默认层为`"1"`） |

顶点属性

|属性|必选|类型|描述|| ----------- | ---------- | ------ | ------------- |
|`x`|是|浮动|形状的左边缘（距画布原点px） |
|`y`|是|浮动|形状的顶部边缘（距离画布原点px） |
|`width`|是|浮动|形状宽度在px |
|`height`|是|浮动|形状的高度在px |
|`as`|是|`"geometry"`|总是`"geometry"`|

---

Edge （Connector）元素```xml
<mxCell
  id="5"
  value="Label"
  style="edgeStyle=orthogonalEdgeStyle;rounded=0;html=1;"
  edge="1"
  source="2"
  target="3"
  parent="1">
  <mxGeometry relative="1" as="geometry" />
</mxCell>
```
###`<mxCell>`边缘属性

|属性|必选|类型|描述|| ----------- | ---------- | ------ | ------------- |
|`id`|是| string |唯一标识符|
|`value`|是| string |连接器标签（无标签为空字符串）|
|`style`|是| string | Style string（参见Edge Styles） |
|`edge`|是|`"1"`|必须是`"1"`才能声明为连接器|
|`source`|否| string |源顶点|的ID
|`target`|否| string |目标顶点|的ID
|`parent`|是| string |父单元格ID（通常为`"1"`） |`<mxGeometry>`边缘属性

|属性|必选|类型|描述|| ----------- | ---------- | ------ | ------------- |
|`relative`|不|`"1"`|边|总是`"1"`|`as`|是|`"geometry"`|总是`"geometry"`|

带有标签偏移的边缘```xml
<mxGeometry x="-0.1" y="10" relative="1" as="geometry">
  <mxPoint as="offset" />
</mxGeometry>
```
相对几何上的`x`沿着边缘移动标签（-1到1）。`y`在px中垂直偏移。

带有手动路径点（控制点）的边缘```xml
<mxGeometry relative="1" as="geometry">
  <Array as="points">
    <mxPoint x="340" y="80" />
    <mxPoint x="340" y="200" />
  </Array>
</mxGeometry>
```
---

##多页图```xml
<mxfile>
  <diagram id="page-1" name="Overview">
    <mxGraphModel>...</mxGraphModel>
  </diagram>
  <diagram id="page-2" name="Detail">
    <mxGraphModel>...</mxGraphModel>
  </diagram>
</mxfile>
```
每个`<diagram>`都是一个单独的page/tab.单元格ID的作用域为它们自己的`<diagram>`—相同的ID值可以出现在不同的页面中而不会发生冲突。

---

##图层单元格

图层替换默认的`id="1"`层。通过`parent`将单元格分配给一个层：```xml
<mxCell id="0" />
<mxCell id="1" value="Background" parent="0" />        <!-- layer 1 -->
<mxCell id="layer2" value="Services" parent="0" />     <!-- layer 2 -->
<mxCell id="layer3" value="Connectors" parent="0" />   <!-- layer 3 -->

<!-- Assign layer via parent attribute -->
<mxCell id="10" value="API" ... parent="layer2">
  <mxGeometry ... />
</mxCell>
```
切换图层可见性：```xml
<mxCell id="layer2" value="Services" parent="0" visible="0" />
```
---

泳道容器```xml
<!-- Swimlane container -->
<mxCell id="swim1" value="Process" style="shape=pool;startSize=30;horizontal=1;" 
        vertex="1" parent="1">
  <mxGeometry x="40" y="40" width="800" height="340" as="geometry" />
</mxCell>

<!-- Lane 1 (child of swimlane container) -->
<mxCell id="lane1" value="Customer" style="swimlane;startSize=30;" 
        vertex="1" parent="swim1">
  <mxGeometry x="0" y="30" width="800" height="150" as="geometry" />
</mxCell>

<!-- Shape inside lane (child of lane) -->
<mxCell id="step1" value="Place Order" style="rounded=1;whiteSpace=wrap;html=1;" 
        vertex="1" parent="lane1">
  <mxGeometry x="80" y="50" width="120" height="60" as="geometry" />
</mxCell>
```
b> **键**：泳道内的单元格将`parent`设置为**泳道的ID**，而不是`"1"`。
>车道内坐标**相对于车道原点**。

---

##分组细胞```xml
<!-- Invisible group container -->
<mxCell id="group1" value="" style="group;" vertex="1" parent="1">
  <mxGeometry x="100" y="100" width="300" height="200" as="geometry" />
</mxCell>

<!-- Children relative to group origin -->
<mxCell id="child1" value="A" style="rounded=1;" vertex="1" parent="group1">
  <mxGeometry x="20" y="20" width="100" height="60" as="geometry" />
</mxCell>
```
---

##标签

当`html=1`为样式时，`value`可以包含HTML：```xml
<mxCell value="&lt;b&gt;OrderService&lt;/b&gt;&lt;br&gt;&lt;i&gt;:8080&lt;/i&gt;"
        style="rounded=1;html=1;" vertex="1" parent="1">
  <mxGeometry x="100" y="100" width="160" height="60" as="geometry" />
</mxCell>
```
HTML必须是xml转义的：

-`<`→`&lt;`-`>`→`&gt;`-`&`→`&amp;`-`"`→`&quot;`支持常见HTML标签：`<b>`，`<i>`,`<u>`,`<br>`,`<font color="#hex">`,`<span style="...">`,`<hr/>`---

##工具提示/元数据```xml
<mxCell value="Service Name" tooltip="Handles order processing" style="..." vertex="1" parent="1">
  <mxGeometry ... />
</mxCell>
```
---

ID生成规则

|规则|详细信息|| ------ | -------- |
| id`0`和`1`|保留-始终是根层和默认层|
|所有其他id |必须在其`<diagram>`|中唯一
|安全模式|从`2`开始的顺序整数，或UUID字符串|
|跨页面| id不需要在不同的`<diagram>`页面|中是唯一的

**安全顺序ID示例：**```text
id="2", id="3", id="4", ...
```
* * UUID-style例子:* *```text
id="a1b2c3d4-e5f6-7890-abcd-ef1234567890"
```
---

坐标系统

-原点`(0, 0)`是**左上角**的画布
-`x`向右增加**
-`y`向下增加**
—所有单位为**像素**

---

##推荐间距

|背景信息|取值|| --------- | ------- |
|形状之间最小间隙|`40px`|
|舒适间隙|`80px`|
|泳道内垫|`20px`|
|页边距|`40px`|
|连接器路由间隙|`10px`|

---

最小有效的`.drawio`文件```xml
<mxfile host="Electron" modified="2026-03-25T00:00:00.000Z" version="26.0.0">
  <diagram id="main" name="Page-1">
    <mxGraphModel dx="1422" dy="762" grid="1" gridSize="10" guides="1"
                  tooltips="1" connect="1" arrows="1" fold="1"
                  page="1" pageScale="1" pageWidth="1169" pageHeight="827"
                  math="0" shadow="0">
      <root>
        <mxCell id="0" />
        <mxCell id="1" parent="0" />
      </root>
    </mxGraphModel>
  </diagram>
</mxfile>
```
---

验证规则

必须通过

- []`id="0"`和`id="1"`单元格总是作为`<root>`的前两个子单元格出现
-[]没有其他单元格使用`id="0"`或`id="1"`-[]所有`id`值在每个`<diagram>`中是唯一的
-[]每个`<mxCell>`都有一个`<mxGeometry>`子节点
- []`<mxGeometry>`具有`as="geometry"`属性
-[]顶点单元有`vertex="1"`，边缘单元有`edge="1"`-[]边缘`source`/`target`id引用同一图中已有的顶点id
-[]泳道子节点`parent`设置为swimlane/laneID，而不是`"1"`—`value`属性中的[]HTML是xml转义的

# # #推荐

[]形状不会重叠，除非是故意的（使用≥40px的间距）
-[]边缘标签短（≤4个字）
-[]层单元有描述性的`value`名称
-[]所有形状都在`pageWidth``pageHeight`范围内