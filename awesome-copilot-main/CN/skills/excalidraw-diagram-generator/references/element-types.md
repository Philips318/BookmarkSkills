# Excalidraw元素类型指南

每种Excalidraw元素类型的详细规格，包括可视化示例和用例。

元素类型概述

|类型|可视化|主要使用|文本支持||------|--------|-------------|--------------|
|`rectangle`|□|箱体、容器、工艺步骤|✅是|
|`ellipse`|〇|强调，终端，状态|✅是|
|`diamond`|◇|决策点，选择|✅是|
|`arrow`|→|定向流，关系|❌不（使用单独的文本）|
|`line`| - |连接，分压器|❌无|
|`text`| A |标签，注释，标题|✅（其目的）|

---

# #矩形

**最适用于：**流程步骤、实体、数据存储、组件

# # #属性```typescript
{
  type: "rectangle",
  roundness: { type: 3 },  // Rounded corners
  text: "Step Name",       // Optional embedded text
  fontSize: 20,
  textAlign: "center",
  verticalAlign: "middle"
}
```
用例

|场景|配置|----------|---------------|
| **进程步骤** |绿色背景（`#b2f2bb`），居中文本|
| **Entity/Object** |蓝色背景（`#a5d8ff`），中等大小|
| **系统组件** |浅色，说明文字|
| **数据存储** |Gray/white，类数据库标签|

###尺寸指南

|内容|宽度|高度||---------|-------|--------|
|单字| 120-150px | 60-80px |
|短短语（2-4个单词）| 180-220px | 80-100px |
| 250-300像素| 100-120像素|

# # #的例子```json
{
  "type": "rectangle",
  "x": 100,
  "y": 100,
  "width": 200,
  "height": 80,
  "backgroundColor": "#b2f2bb",
  "text": "Validate Input",
  "fontSize": 20,
  "textAlign": "center",
  "verticalAlign": "middle",
  "roundness": { "type": 3 }
}
```
---

# #椭圆

**最适合：**Start/end点，状态，强调圈

# # #属性```typescript
{
  type: "ellipse",
  text: "Start",
  fontSize: 18,
  textAlign: "center",
  verticalAlign: "middle"
}
```
用例

|场景|配置|----------|---------------|
| **流量启动** |浅绿色，“启动”文本|
| **流量结束** |亮红色，“结束”文本|
| **状态** |颜色柔和，状态名称|
| **高亮** |明亮的颜色，强调文字|

###尺寸指南

对于圆形，使用`width === height`：

|内容|直径||---------|----------|
| 60-80px |
|短文本| 100-120px |
|长文本| 150-180px |

# # #的例子```json
{
  "type": "ellipse",
  "x": 100,
  "y": 100,
  "width": 120,
  "height": 120,
  "backgroundColor": "#d0f0c0",
  "text": "Start",
  "fontSize": 18,
  "textAlign": "center",
  "verticalAlign": "middle"
}
```
---

# #钻石

**最适合：**决策点，条件分支

# # #属性```typescript
{
  type: "diamond",
  text: "Valid?",
  fontSize: 18,
  textAlign: "center",
  verticalAlign": "middle"
}
```
用例

|场景||----------|--------------|
| **Yes/No决定** |“是否有效？”，“存在？”|
| **多项选择** |“类型？”，“状态？”|
| **条件必选** | “评分> 50?”|

###尺寸指南

对于相同的文本，菱形比矩形需要更多的空间：

|内容|宽度|高度||---------|-------|--------|
| 120-140px | 120-140px |
|短问题| 160-180px | 160-180px |
| 200-220px | 200-220px |

# # #的例子```json
{
  "type": "diamond",
  "x": 100,
  "y": 100,
  "width": 150,
  "height": 150,
  "backgroundColor": "#ffe4a3",
  "text": "Valid?",
  "fontSize": 18,
  "textAlign": "center",
  "verticalAlign": "middle"
}
```
---

# #箭头

**最适合：**流向、关系、依赖

# # #属性```typescript
{
  type: "arrow",
  points: [[0, 0], [endX, endY]],  // Relative coordinates
  roundness: { type: 2 },          // Curved
  startBinding: null,              // Or { elementId, focus, gap }
  endBinding: null
}
```
###箭头方向

####水平（从左到右）```json
{
  "x": 100,
  "y": 150,
  "width": 200,
  "height": 0,
  "points": [[0, 0], [200, 0]]
}
```
####垂直（从上到下）```json
{
  "x": 200,
  "y": 100,
  "width": 0,
  "height": 150,
  "points": [[0, 0], [0, 150]]
}
```
# # # #对角线```json
{
  "x": 100,
  "y": 100,
  "width": 200,
  "height": 150,
  "points": [[0, 0], [200, 150]]
}
```
箭头样式

|样式|`strokeStyle`|`strokeWidth`|用例||-------|---------------|---------------|----------|
| **正常流量** |`"solid"`| 2 |标准连接|
| **Optional/Weak** |`"dashed"`| 2 |可选路径|
| **重要** |`"solid"`| 3-4 |强调流量|
| **点** |`"dotted"`| 2 |间接关系|

添加箭头标签

使用靠近箭头中点的单独文本元素：```json
[
  {
    "type": "arrow",
    "id": "arrow1",
    "x": 100,
    "y": 150,
    "points": [[0, 0], [200, 0]]
  },
  {
    "type": "text",
    "x": 180,      // Near midpoint
    "y": 130,      // Above arrow
    "text": "sends",
    "fontSize": 14
  }
]
```
---

# #线

**最适合：**非定向连接，分隔器，边界

# # #属性```typescript
{
  type: "line",
  points: [[0, 0], [x2, y2], [x3, y3], ...],
  roundness: null  // Or { type: 2 } for curved
}
```
用例

|场景|配置|----------|---------------|
| **分压器** |水平，细描|
| **边界** |封闭路径（多边形）|
| **连接** |多点路径|
| **下划线** |短水平线|

多点直线示例```json
{
  "type": "line",
  "x": 100,
  "y": 100,
  "points": [
    [0, 0],
    [100, 50],
    [200, 0]
  ]
}
```
---

# #文本

**最适合：**标签，标题，注释，独立文本

# # #属性```typescript
{
  type: "text",
  text: "Label text",
  fontSize: 20,
  fontFamily: 1,        // 1=Virgil, 2=Helvetica, 3=Cascadia
  textAlign: "left",
  verticalAlign: "top"
}
```
###字体大小按用途

|用途|字体大小||---------|-----------|
| **主标题** | 28-36 |
| **Section header** | 24- 28|
| **元素标签** | 18-22 |
| **注释** | 14-16 |
| **小音符** | 12-14 |Width/Height计算```javascript
// Approximate width
const width = text.length * fontSize * 0.6;

// Approximate height (single line)
const height = fontSize * 1.2;

// Multi-line
const lines = text.split('\n').length;
const height = fontSize * 1.2 * lines;
```
文本定位

|位置|文本对齐|垂直对齐|用例||----------|-----------|---------------|----------|
| **左上** |`"left"`|`"top"`|默认标签|
| **居中** |`"center"`|`"middle"`|标题|
| **右下** |`"right"`|`"bottom"`|脚注|

示例：标题```json
{
  "type": "text",
  "x": 100,
  "y": 50,
  "width": 400,
  "height": 40,
  "text": "System Architecture",
  "fontSize": 32,
  "fontFamily": 2,
  "textAlign": "center",
  "verticalAlign": "top"
}
```
示例：注释```json
{
  "type": "text",
  "x": 150,
  "y": 200,
  "width": 100,
  "height": 20,
  "text": "User input",
  "fontSize": 14,
  "fontFamily": 1,
  "textAlign": "left",
  "verticalAlign": "top"
}
```
---

##组合元素

###模式：标签盒```json
[
  {
    "type": "rectangle",
    "id": "box1",
    "x": 100,
    "y": 100,
    "width": 200,
    "height": 100,
    "text": "Component",
    "textAlign": "center",
    "verticalAlign": "middle"
  }
]
```
模式：连接的盒子```json
[
  {
    "type": "rectangle",
    "id": "box1",
    "x": 100,
    "y": 100,
    "width": 150,
    "height": 80,
    "text": "Step 1"
  },
  {
    "type": "arrow",
    "id": "arrow1",
    "x": 250,
    "y": 140,
    "points": [[0, 0], [100, 0]]
  },
  {
    "type": "rectangle",
    "id": "box2",
    "x": 350,
    "y": 100,
    "width": 150,
    "height": 80,
    "text": "Step 2"
  }
]
```
模式：决策树```json
[
  {
    "type": "diamond",
    "id": "decision",
    "x": 100,
    "y": 100,
    "width": 140,
    "height": 140,
    "text": "Valid?"
  },
  {
    "type": "arrow",
    "id": "yes-arrow",
    "x": 240,
    "y": 170,
    "points": [[0, 0], [60, 0]]
  },
  {
    "type": "text",
    "id": "yes-label",
    "x": 250,
    "y": 150,
    "text": "Yes",
    "fontSize": 14
  },
  {
    "type": "rectangle",
    "id": "yes-box",
    "x": 300,
    "y": 140,
    "width": 120,
    "height": 60,
    "text": "Process"
  }
]
```
---

# #总结

当你需要…使用这个元素||------------------|------------------|
|处理框|`rectangle`与文本|
|决策点|`diamond`问题|
|流向|`arrow`|
|Start/End|`ellipse`|
|Title/Header|`text`（大字）|
|注释|`text`（小字体）|
|无向链接|`line`|
|分压器|`line`（水平）|