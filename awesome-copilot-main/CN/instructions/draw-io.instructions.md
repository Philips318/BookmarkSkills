---
description: "Use when creating, editing, or reviewing draw.io diagrams and mxGraph XML in .drawio, .drawio.svg, or .drawio.png files."
applyTo: "**/*.drawio,**/*.drawio.svg,**/*.drawio.png"
---
#画。io图标准

b> **技能**：在生成或编辑任何`.drawio`文件之前，加载`.github/skills/draw-io/SKILL.md`以获得完整的工作流程，XML配方和故障排除。

---

##需要的工作流程

每次抽签都遵循以下步骤。io的任务:

1. 识别图表类型（流程图/架构/序列/ ER / UML /网络/ BPMN）
2. **选择**匹配模板从`.github/skills/draw-io/templates/`和适应它，或从最小骨架开始
3. 在编写XML之前，先在纸上规划布局——首先定义层、角色或实体
4. **按照以下规则生成有效的mxGraph XML
5. **验证**使用`python .github/skills/draw-io/scripts/validate-drawio.py <file>`6. **确认**文件渲染打开它在VS Code与绘制。IO扩展（`hediet.vscode-drawio`）

---

XML结构规则（不可协商）```xml
<!-- Set modified to the current ISO 8601 timestamp when generating a new file -->
<mxfile host="Electron" modified="" version="26.0.0">
  <diagram id="unique-id" name="Page Name">
    <mxGraphModel ...>
      <root>
        <mxCell id="0" />                          <!-- REQUIRED: always first -->
        <mxCell id="1" parent="0" />               <!-- REQUIRED: always second -->
        <!-- all other cells go here -->
      </root>
    </mxGraphModel>
  </diagram>
</mxfile>
```
-`id="0"`和`id="1"`**必须**存在，并且必须是前两个单元格-没有例外
—图中的每个单元格`id`必须是唯一的
-每个顶点（`vertex="1"`）必须有一个子顶点`<mxGeometry x y width height as="geometry">`-每条边（`edge="1"`）必须有`source`/`target`指向现有的顶点id - **例外**：浮动边（序列图生命线）使用`<mxPoint as="sourcePoint">`和`<mxPoint as="targetPoint">`在`<mxGeometry>`而不是`source`/`target`属性
—除了id=0之外的每个单元格必须有`parent`指向一个已存在的id
-容器（泳道）的子元素使用相对于父元素的坐标，而不是画布

---

强制样式约定

语义调色板-在整个项目中一致使用

|角色| fillColor | strokecol||---|---|---|
|主/ Info（默认值）|`#dae8fc`|`#6c8ebf`|
| Success / Start / Positive |`#d5e8d4`|`#82b366`|
|`#fff2cc`|`#d6b656`|
|错误/结束/危险|`#f8cecc`|`#b85450`|
|中性/接口|`#f5f5f5`|`#666666`|
|外部/合作伙伴|`#e1d5e7`|`#9673a6`|

总是包含顶点形状```
whiteSpace=wrap;html=1;
```
当标签包含HTML标签时使用`html=1`（`<b>`,`<i>`,`<br>`）

标准连接器```
edgeStyle=orthogonalEdgeStyle;html=1;
```
---

图表类型快速参考

|类型|容器|键形状|连接器样式||---|---|---|---|
|流程图|无|`ellipse`(start/end),`rounded=1`(process),`rhombus`(decision) |`orthogonalEdgeStyle`|
|架构|`swimlane`每层|`rounded=1`服务，cloud/DB形状|`orthogonalEdgeStyle`标签|
|序列|无|`mxgraph.uml.actor`，虚线生命线边缘|`endArrow=block`(sync),`endArrow=open;dashed=1`(return) |
|`shape=table;childLayout=tableLayout`|`shape=tableRow`,`shape=partialRectangle`|`entityRelationEdgeStyle;endArrow=ERmany;startArrow=ERone`|
| UML类|`swimlane`每类|文本行为attributes/methods|`endArrow=block;endFill=0`（继承），`dashed=1`（实现）|

---

##布局最佳实践

-将所有坐标对齐到** 10px网格**（值可被10整除）
- **水平**:40-60像素的间距相同的行形状
- **垂直**:80-120像素间距的层行
-标准形状尺寸：`120 × 60`px（加工），`200 × 100`px（决策菱形）
-默认画布：A4横向`1169 × 827`px
-最大**每页40单元格** -分割成多个页面较大的图表
-在每页的顶部添加一个标题文本单元格：  ```
  style="text;strokeColor=none;fillColor=none;fontSize=18;fontStyle=1;align=center;"
  ```
---

文件和命名约定

扩展名：`.drawio`用于版本控制图，`.drawio.svg`用于嵌入Markdown中的文件
-命名：`kebab-case`-例如`order-flow.drawio`，`database-schema.drawio`-位置：`docs/`或`architecture/`旁边的代码他们的文档
-多页面：在同一个`<mxfile>`中，每个逻辑视图使用一个`<diagram>`元素

---

验证检查表（每次提交前运行）

—[]`<mxCell id="0" />`和`<mxCell id="1" parent="0" />`为前两个单元格
-[]所有单元格id在其图表中是唯一的
-[]所有边缘`source`/`target`id解析到现有的顶点
-[]所有顶点单元都有`<mxGeometry as="geometry">`-[]所有单元格（id=0除外）都有一个有效的`parent`- [] XML格式良好-没有未闭合标签，属性值中没有`&`，`<`,`>`-[]语义调色板使用一致
-[]标题单元格出现在每一页```bash
# Run automated validation
python .github/skills/draw-io/scripts/validate-drawio.py <file.drawio>
```
---

##参考文件

|文件|用于||---|---|
|`.github/skills/draw-io/SKILL.md`|完整代理工作流程，食谱，故障排除|
|`.github/skills/draw-io/references/drawio-xml-schema.md`|完整的mxCell属性引用|
|`.github/skills/draw-io/references/style-reference.md`|所有样式键，形状名称，边缘类型|
|`.github/skills/draw-io/references/shape-libraries.md`|形状库目录与样式字符串|
|`.github/skills/draw-io/templates/`|每个图类型|准备使用的`.drawio`模板
|`.github/skills/draw-io/scripts/validate-drawio.py`| XML结构验证器|
|`.github/skills/draw-io/scripts/add-shape.py`|命令行：添加形状到已有的图|