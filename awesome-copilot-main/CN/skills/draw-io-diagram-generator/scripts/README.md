# draw-io脚本

用于处理cxp-bu-order-ms项目中的`.drawio`图文件的实用程序脚本。

# #要求

- Python 3.8+
-无外部依赖（仅使用标准库：`xml.etree.ElementTree`，`argparse`,`json`,`sys`,`pathlib`）

# #脚本### `validate-drawio.py`
根据所需的约束验证`.drawio`文件的XML结构。

使用* * * *```bash
python scripts/validate-drawio.py <path-to-diagram.drawio>
```
* * * *例子```bash
# Validate a single file
python scripts/validate-drawio.py docs/architecture.drawio

# Validate all drawio files in a directory
for f in docs/**/*.drawio; do python scripts/validate-drawio.py "$f"; done
```
执行* * * *检查

|查看|告警描述||-------|-------------|
|根单元格|验证id=“0“和id=”1”单元格是否存在于每个图表页面|中
|唯一id |所有`mxCell`id值在一个图中是唯一的|
每条边都有有效的`source`和`target`属性，指向现有的单元格|
每个顶点单元都有一个`mxGeometry`子元素|
每个单元格的`parent`属性引用一个现有的单元格id |
| XML格式良好|文件是有效的XML |

* * * *的退出代码

-`0`-验证通过
-`1`-发现一个或多个验证错误（错误打印到标准输出）

---### `add-shape.py`
将新形状（顶点单元）添加到现有的`.drawio`图文件中。

使用* * * *```bash
python scripts/add-shape.py <diagram.drawio> <label> <x> <y> [options]
```
* * * *参数

|参数|必选|描述||----------|----------|-------------|
|`diagram`|是|`.drawio`文件所在路径|
|`label`|是|新形状|的文本标签
|`x`|是| X坐标（从左上角开始像素）|
|`y`|是| Y坐标（从左上角开始像素）|

* * * *选项

|可选项|默认值|描述||--------|---------|-------------|
|`--width`|`120`|形状宽度，以像素为单位|
|`--height`|`60`|形状高度，以像素为单位|
|`--style`|`"rounded=1;whiteSpace=wrap;html=1;"`|抽奖。IO样式字符串|
|`--diagram-index`|`0`|图页索引（0为基数）|
|`--dry-run`| false |打印新的单元格XML，而不修改文件|

* * * *例子```bash
# Add a basic rounded box
python scripts/add-shape.py docs/flowchart.drawio "New Step" 400 300

# Add a custom styled shape
python scripts/add-shape.py docs/flowchart.drawio "Decision" 400 400 \
  --width 160 --height 80 \
  --style "rhombus;whiteSpace=wrap;html=1;fillColor=#fff2cc;strokeColor=#d6b656;"

# Preview without writing
python scripts/add-shape.py docs/architecture.drawio "Service X" 600 200 --dry-run
```
* * * *输出

成功时打印新的单元格id：```
Added shape id="auto_abc123" to page 0 of docs/flowchart.drawio
```
---

##通用工作流

提交前验证```bash
# Validate all diagrams
find . -name "*.drawio" -not -path "*/node_modules/*" | \
  xargs -I{} python scripts/validate-drawio.py {}
```
快速添加一个占位符节点```bash
python scripts/add-shape.py docs/architecture.drawio "TODO: Service" 800 400 \
  --style "rounded=1;whiteSpace=wrap;html=1;fillColor=#f8cecc;strokeColor=#b85450;"
```
检查模板是否有效```bash
python scripts/validate-drawio.py .github/skills/draw-io-diagram-generator/templates/flowchart.drawio
```
