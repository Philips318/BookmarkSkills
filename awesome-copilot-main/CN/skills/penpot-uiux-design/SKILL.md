---
name: penpot-uiux-design
description: 'Comprehensive guide for creating professional UI/UX designs in Penpot using MCP tools. Use this skill when: (1) Creating new UI/UX designs for web, mobile, or desktop applications, (2) Building design systems with components and tokens, (3) Designing dashboards, forms, navigation, or landing pages, (4) Applying accessibility standards and best practices, (5) Following platform guidelines (iOS, Android, Material Design), (6) Reviewing or improving existing Penpot designs for usability. Triggers: "design a UI", "create interface", "build layout", "design dashboard", "create form", "design landing page", "make it accessible", "design system", "component library".'
---
#笔筒设计指南

使用`penpot/penpot-mcp`MCP服务器和经过验证的UI/UX原则，在Penpot中创建专业的、以用户为中心的设计。

可用的MCP工具

|工具|用途|| ---- | ------- |
|`mcp__penpot__execute_code`|在Penpot插件上下文中运行JavaScript以create/modify设计|
|`mcp__penpot__export_shape`|导出形状为PNG/SVG目视检查|
导入图像（图标，照片，徽标）到设计|
|`mcp__penpot__penpot_api_info`|检索Penpot API文档|

MCP服务器设置

Penpot MCP工具需要在本地运行`penpot/penpot-mcp`服务器。有关详细的安装和故障排除，请参见[setup-troubleshooting.md]（references/setup-troubleshooting.md）。

安装前：检查是否已经运行

**在尝试安装之前，始终检查MCP服务器是否已经可用：**

1. **尝试先调用一个工具**：尝试`mcp__penpot__penpot_api_info`-如果它成功，服务器正在运行和连接。不需要设置。

2. **如果工具失败**，请询问用户：
> " Penpot MCP服务器似乎没有连接。服务器是否已经安装并运行？如果有，我可以帮忙排除故障。如果没有，我可以指导你完成设置。”3. **只有在用户确认服务器没有安装的情况下，才继续执行安装说明

###快速入门（仅在未安装时）```bash
# Clone and install
git clone https://github.com/penpot/penpot-mcp.git
cd penpot-mcp
npm install

# Build and start servers
npm run bootstrap
```
然后在彭波特：
1. 打开设计文件
2. 转到**插件**→**从URL**加载插件
3. 输入:`http://localhost:4400/manifest.json`4. 点击插件界面中的“连接到MCP服务器”**VS Code配置

添加到`settings.json`：```json
{
  "mcp": {
    "servers": {
      "penpot": {
        "url": "http://localhost:4401/sse"
      }
    }
  }
}
```
###故障排除（如果服务器已安装但不工作）

|问题|解决方案|| ----- | -------- |
|插件无法连接|检查服务器正在运行（`npm run start:all`在penpot-mcp目录）|
|浏览器阻止本地主机|允许本地网络访问提示，或禁用Brave Shield，或尝试Firefox |
|客户端没有出现工具|在配置更改后重新启动VS Code/Claude |
|工具执行fails/timesout |确保Penpot插件UI打开并显示“已连接”|
| “WebSocket连接失败” |检查防火墙是否允许4400,4401,4402端口|

##快速参考

|任务|参考文件|| ---- | -------------- |
| MCP服务器安装与故障排除| [setup-troubleshooting.md](references/setup-troubleshooting.md) |
|组件规格（按钮，表单，导航）| [component-patterns.md](references/component-patterns.md) |
无障碍（对比，触摸目标）| [accessibility.md](references/accessibility.md) |
|屏幕尺寸和平台规格| [platform-guidelines.md](references/platform-guidelines.md) |

核心设计原则

黄金法则

1. **清晰胜过聪明**：每个元素都必须有目的
2. **一致性建立信任**：重用模式、颜色和组件
3. **用户目标至上**：设计任务，而不是功能
4. **可访问性不是可选的**：为所有人设计
5. **用真实用户进行测试**：尽早验证假设

视觉层次（优先顺序）

1. **尺寸**：越大=越重要
2. **Color/Contrast**：高对比度吸引注意力
3. **位置**：首先看到左上角（LTR）
4. **空格**：隔离强调重要性
5. **字体粗细**：粗体突出

##设计流程1. **首先检查设计系统**：询问用户是否有现有的tokens/specs，或者从当前的Penpot文件中发现
2. **理解页面**：用`penpotUtils.shapeStructure()`调用`mcp__penpot__execute_code`来查看层次结构
3. **查找元素**：使用`penpotUtils.findShapes()`按类型或名称查找元素
4. **Create/modify**：使用`penpot.createBoard()`、`penpot.createRectangle()`、`penpot.createText()`等。
5. **应用布局**：使用`addFlexLayout()`响应容器
6. **Validate**：调用`mcp__penpot__export_shape`来直观地检查您的工作

##设计系统处理

**在创建设计之前，确定用户是否有现有的设计系统

1. 问用户：“你有什么设计体系或品牌准则可循吗？”
2. **发现从笔盆**：检查现有的组件，颜色和图案```javascript
// Discover existing design patterns in current file
const allShapes = penpotUtils.findShapes(() => true, penpot.root);

// Find existing colors in use
const colors = new Set();
allShapes.forEach(s => {
  if (s.fills) s.fills.forEach(f => colors.add(f.fillColor));
});

// Find existing text styles (font sizes, weights)
const textStyles = allShapes
  .filter(s => s.type === 'text')
  .map(s => ({ fontSize: s.fontSize, fontWeight: s.fontWeight }));

// Find existing components
const components = penpot.library.local.components;

return { colors: [...colors], textStyles, componentCount: components.length };
```
**如果用户有一个设计系统

-使用他们指定的颜色，间距，排版
-匹配他们现有的组件模式
-遵循他们的命名习惯

**如果用户没有设计系统：**

-使用下面的默认令牌作为起点
-提供帮助建立一致的模式
-参考规格在[component-patterns.md]（references/component-patterns.md）

## Key Penpot API get tchas

—`width`/`height`为READ-ONLY
—`parentX`/`parentY`为READ-ONLY = >使用`penpotUtils.setParentXY(shape, x, y)`-使用`insertChild(index, shape)`进行z排序（而不是`appendChild`）
-`dir="column"`或`dir="row"`的Flex子数组顺序颠倒
—在“`text.resize()`”后，将“`growType`”重置为“`"auto-width"`”或“`"auto-height"`”

定位新板

**在创建新板子之前总是检查现有板子，以避免重叠；```javascript
// Find all existing boards and calculate next position
const boards = penpotUtils.findShapes(s => s.type === 'board', penpot.root);
let nextX = 0;
const gap = 100; // Space between boards

if (boards.length > 0) {
  // Find rightmost board edge
  boards.forEach(b => {
    const rightEdge = b.x + b.width;
    if (rightEdge + gap > nextX) {
      nextX = rightEdge + gap;
    }
  });
}

// Create new board at calculated position
const newBoard = penpot.createBoard();
newBoard.x = nextX;
newBoard.y = 0;
newBoard.resize(375, 812);
```
**板间距指南：**

-在相关屏幕之间使用100px的间隙（相同的流程）
-使用200px+间距不同的sections/flows—垂直排列电路板（相同的y），以便视觉组织
-按用户流顺序水平分组相关屏幕

默认设计令牌

**只有当用户没有设计系统时才使用这些默认值。如果可用，总是使用用户的令牌

###间距缩放（8px底）

|令牌|值|用途|| ----- | ----- | ----- |
|`spacing-xs`| 4px |紧密内联元素|
|`spacing-sm`| 8px |相关元素|
|`spacing-md`| 16px |默认填充|
|`spacing-lg`| 24px |截面间距|
|`spacing-xl`| 32px |主要部分|
|`spacing-2xl`| 48px |页间距|

###字体规模

|级别|大小|重量|用途|| ----- | ---- | ------ | ----- |
|显示| 48-64px | Bold |英雄标题|
| H1 | 32-40px |粗体|页标题|
| H2 | 24-28px |半粗体|部分标题|
| H3 | 20-22px |半粗|分段|
|正文| 16px |常规|主要内容|
|小| 14px |普通|辅助文本|
|标题| 12px |常规|标签，提示|

颜色使用

|目的|推荐|| ------- | -------------- |
|原色|主品牌色，cta |
|备用|支持动作|
|成功| #22C55E范围（确认）|
|警告| #F59E0B范围（警告）|
|错误| #EF4444范围（错误）|
|中性|text/borders|的灰度

##常用布局

###移动屏幕（375×812）```text
┌─────────────────────────────┐
│ Status Bar (44px)           │
├─────────────────────────────┤
│ Header/Nav (56px)           │
├─────────────────────────────┤
│                             │
│ Content Area                │
│ (Scrollable)                │
│ Padding: 16px horizontal    │
│                             │
├─────────────────────────────┤
│ Bottom Nav/CTA (84px)       │
└─────────────────────────────┘

```
桌面仪表板（1440×900）```text
┌──────┬──────────────────────────────────┐
│      │ Header (64px)                    │
│ Side │──────────────────────────────────│
│ bar  │ Page Title + Actions             │
│      │──────────────────────────────────│
│ 240  │ Content Grid                     │
│ px   │ ┌─────┐ ┌─────┐ ┌─────┐ ┌─────┐ │
│      │ │Card │ │Card │ │Card │ │Card │ │
│      │ └─────┘ └─────┘ └─────┘ └─────┘ │
│      │                                  │
└──────┴──────────────────────────────────┘

```
##部件检查表

# # #按钮

-[]清晰，以行动为导向的标签（2-3字）
—[]最小接触目标：44×44px
-[]视觉状态：默认，悬停，激活，禁用，加载
-[]足够的对比度（3:1与背景）
-[]整个应用的边界半径一致

# # #形式

-[]以上输入的标签（不只是占位符）
-[]必选字段指示灯
-[]字段旁边的错误信息
-[]标签逻辑顺序
-[]输入类型匹配内容（email， tel等）

# # #导航

-[]明确当前位置
-[]跨屏位置一致
-[]最多7±2个顶级项目
[]手机上支持触摸（48px目标）

可访问性快速检查1. **颜色对比度**：小字4.5:1，大字3:1
2. **触摸目标**：最少44×44px
3. **焦点状态**：键盘焦点显示
4. **Alt text**：对图片的有意义的描述
5. **层次结构**：适当的标题层次（H1→H2→H3）
6. **色彩独立性**：永远不要仅仅依赖于色彩

##设计评审清单

在完成任何设计之前：

-[]视觉层次清晰
-[]一致的间距和对齐
-[]字体可读（16px+正文）
-[]颜色对比度符合WCAG AA标准
-[]互动元素很明显
-[]移动友好的触摸目标
- []Loading/empty/error考虑的状态
-[]与设计体系一致

验证设计

对`mcp__penpot__execute_code`使用这些验证方法：

|检查|方法|| ----- | ------ |
|边界外的元素|`penpotUtils.analyzeDescendants()`与`isContainedIn()`|
|文本太小（<12px） |`penpotUtils.findShapes()`过滤`fontSize`|
呼叫`mcp__penpot__export_shape`目视检查|
|层次结构|`penpotUtils.shapeStructure()`回顾嵌套|

###导出CSS

使用`penpot.generateStyle(selection, { type: 'css', includeChildren: true })`通过`mcp__penpot__execute_code`从设计中提取CSS。

优秀设计的技巧

1. **从内容开始**：真正的内容揭示了布局需求
2. **设计移动优先**：限制催生创造力
3. **使用网格**:8px的基础网格保持对齐
4. **限制颜色**:1初级+ 1次级+中性
5. **限制字体**：最多1-2个字体
6. **拥抱空白**：呼吸空间提高理解能力
7. **保持一致**：相同的动作=相同的外观无处不在
8. **提供反馈**：每个行动都需要回应