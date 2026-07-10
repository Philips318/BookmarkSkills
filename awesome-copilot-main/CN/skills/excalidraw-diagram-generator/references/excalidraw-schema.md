# Excalidraw JSON模式引用

本文档描述了用于图表生成的Excalidraw`.excalidraw`文件的结构。

顶层结构```typescript
interface ExcalidrawFile {
  type: "excalidraw";
  version: number;           // Always 2
  source: string;            // "https://excalidraw.com"
  elements: ExcalidrawElement[];
  appState: AppState;
  files: Record<string, any>; // Usually empty {}
}
```
# # AppState```typescript
interface AppState {
  viewBackgroundColor: string; // Hex color, e.g., "#ffffff"
  gridSize: number;            // Typically 20
}
```
ExcalidrawElement基础属性

所有元素都有这些共同的属性：```typescript
interface BaseElement {
  id: string;                  // Unique identifier
  type: ElementType;           // See Element Types below
  x: number;                   // X coordinate (pixels from top-left)
  y: number;                   // Y coordinate (pixels from top-left)
  width: number;               // Width in pixels
  height: number;              // Height in pixels
  angle: number;               // Rotation angle in radians (usually 0)
  strokeColor: string;         // Hex color, e.g., "#1e1e1e"
  backgroundColor: string;     // Hex color or "transparent"
  fillStyle: "solid" | "hachure" | "cross-hatch";
  strokeWidth: number;         // 1-4 typically
  strokeStyle: "solid" | "dashed" | "dotted";
  roughness: number;           // 0-2, controls hand-drawn effect (1 = default)
  opacity: number;             // 0-100
  groupIds: string[];          // IDs of groups this element belongs to
  frameId: null;               // Usually null
  index: string;               // Stacking order identifier
  roundness: Roundness | null;
  seed: number;                // Random seed for deterministic rendering
  version: number;             // Element version (increment on edit)
  versionNonce: number;        // Random number changed on edit
  isDeleted: boolean;          // Should be false
  boundElements: any;          // Usually null
  updated: number;             // Timestamp in milliseconds
  link: null;                  // External link (usually null)
  locked: boolean;             // Whether element is locked
}
```
##元素类型

# # #矩形```typescript
interface RectangleElement extends BaseElement {
  type: "rectangle";
  roundness: { type: 3 };      // 3 = rounded corners
  text?: string;               // Optional text inside
  fontSize?: number;           // Font size (16-32 typical)
  fontFamily?: number;         // 1 = Virgil, 2 = Helvetica, 3 = Cascadia
  textAlign?: "left" | "center" | "right";
  verticalAlign?: "top" | "middle" | "bottom";
}
```
* *的例子:* *```json
{
  "id": "rect1",
  "type": "rectangle",
  "x": 100,
  "y": 100,
  "width": 200,
  "height": 100,
  "strokeColor": "#1e1e1e",
  "backgroundColor": "#a5d8ff",
  "text": "My Box",
  "fontSize": 20,
  "textAlign": "center",
  "verticalAlign": "middle",
  "roundness": { "type": 3 }
}
```
# # #椭圆```typescript
interface EllipseElement extends BaseElement {
  type: "ellipse";
  text?: string;
  fontSize?: number;
  fontFamily?: number;
  textAlign?: "left" | "center" | "right";
  verticalAlign?: "top" | "middle" | "bottom";
}
```
# # #钻石```typescript
interface DiamondElement extends BaseElement {
  type: "diamond";
  text?: string;
  fontSize?: number;
  fontFamily?: number;
  textAlign?: "left" | "center" | "right";
  verticalAlign?: "top" | "middle" | "bottom";
}
```
# # #箭头```typescript
interface ArrowElement extends BaseElement {
  type: "arrow";
  points: [number, number][];  // Array of [x, y] coordinates relative to element
  startBinding: Binding | null;
  endBinding: Binding | null;
  roundness: { type: 2 };      // 2 = curved arrow
}
```
* *的例子:* *```json
{
  "id": "arrow1",
  "type": "arrow",
  "x": 100,
  "y": 100,
  "width": 200,
  "height": 0,
  "points": [
    [0, 0],
    [200, 0]
  ],
  "roundness": { "type": 2 },
  "startBinding": null,
  "endBinding": null
}
```
* *点解释:* *
—第一个点`[0, 0]`相对于`(x, y)`—后续点是相对于第一个点的
—水平直箭头：`[[0, 0], [width, 0]]`—垂直箭头：`[[0, 0], [0, height]]`# # #线```typescript
interface LineElement extends BaseElement {
  type: "line";
  points: [number, number][];
  startBinding: Binding | null;
  endBinding: Binding | null;
  roundness: { type: 2 } | null;
}
```
# # #文本```typescript
interface TextElement extends BaseElement {
  type: "text";
  text: string;
  fontSize: number;
  fontFamily: number;          // 1-3
  textAlign: "left" | "center" | "right";
  verticalAlign: "top" | "middle" | "bottom";
  roundness: null;             // Text has no roundness
}
```
* *的例子:* *```json
{
  "id": "text1",
  "type": "text",
  "x": 100,
  "y": 100,
  "width": 150,
  "height": 25,
  "text": "Hello World",
  "fontSize": 20,
  "fontFamily": 1,
  "textAlign": "left",
  "verticalAlign": "top",
  "roundness": null
}
```
* *Width/Height计算:* *
—宽度≈`text.length * fontSize * 0.6`—高度≈`fontSize * 1.2 * numberOfLines`# #绑定

绑定将箭头连接到形状：```typescript
interface Binding {
  elementId: string;           // ID of bound element
  focus: number;               // -1 to 1, position along edge
  gap: number;                 // Distance from element edge
}
```
##常见颜色

|颜色名称|十六进制代码|用例||------------|----------|----------|
|黑色|`#1e1e1e`|默认描边|
|浅蓝色|`#a5d8ff`|主实体|
|浅绿色|`#b2f2bb`|处理步骤|
|黄色|`#ffd43b`|Important/Central|
|淡红色|`#ffc9c9`|Warnings/Errors|
|青色|`#96f2d7`|次要项|
|透明|`transparent`|无填充|
|白色|`#ffffff`|背景|

## ID生成

id应该是唯一的字符串。常见的模式:```javascript
// Timestamp-based
const id = Date.now().toString(36) + Math.random().toString(36).substr(2);

// Sequential
const id = "element-" + counter++;

// Descriptive
const id = "step-1", "entity-user", "arrow-1-to-2";
```
##种子生成

种子用于手绘效果的确定性随机性：```javascript
const seed = Math.floor(Math.random() * 2147483647);
```
## Version和VersionNonce```javascript
const version = 1;  // Increment when element is edited
const versionNonce = Math.floor(Math.random() * 2147483647);
```
坐标系统

—原点`(0, 0)`为左上角
- X向右增大
- Y向下递增
-所有单位以像素为单位

##推荐间距

|背景信息|间距||---------|---------|
元素之间的水平间隙| 200-300px |
|行之间的垂直间距| 100-150px |
|从|到|的最小边距为50px
箭头到框间隙| 20-30px |

##字体系列

| ID |名称|描述||----|------|-------------|
| 1 |维吉尔|手绘风格（默认）|
| | Helvetica |清洁无衬线字体|
| 3 | Cascadia | Monospace |

验证规则

✅* *要求:* *
—所有id不能重复
-`type`必须匹配实际的元素类型
—`version`必须为≥1的整数
—`opacity`必须为0 ~ 100

⚠️推荐* *:* *
—保持`roughness`为1，以保持一致性
-使用2的`strokeWidth`清晰
—“`isDeleted`”设置为“`false`”
—“`locked`”设置为“`false`”
—保持“`frameId`”、“`boundElements`”、“`link`”为“`null`”

完成最小示例```json
{
  "type": "excalidraw",
  "version": 2,
  "source": "https://excalidraw.com",
  "elements": [
    {
      "id": "box1",
      "type": "rectangle",
      "x": 100,
      "y": 100,
      "width": 200,
      "height": 100,
      "angle": 0,
      "strokeColor": "#1e1e1e",
      "backgroundColor": "#a5d8ff",
      "fillStyle": "solid",
      "strokeWidth": 2,
      "strokeStyle": "solid",
      "roughness": 1,
      "opacity": 100,
      "groupIds": [],
      "frameId": null,
      "index": "a0",
      "roundness": { "type": 3 },
      "seed": 1234567890,
      "version": 1,
      "versionNonce": 987654321,
      "isDeleted": false,
      "boundElements": null,
      "updated": 1706659200000,
      "link": null,
      "locked": false,
      "text": "Hello",
      "fontSize": 20,
      "fontFamily": 1,
      "textAlign": "center",
      "verticalAlign": "middle"
    }
  ],
  "appState": {
    "viewBackgroundColor": "#ffffff",
    "gridSize": 20
  },
  "files": {}
}
```
