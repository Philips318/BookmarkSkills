---
name: excalidraw-diagram-generator
description: 'Generate Excalidraw diagrams from natural language descriptions. Use when asked to "create a diagram", "make a flowchart", "visualize a process", "draw a system architecture", "create a mind map", or "generate an Excalidraw file". Supports flowcharts, relationship diagrams, mind maps, and system architecture diagrams. Outputs .excalidraw JSON files that can be opened directly in Excalidraw.'
---
# Excalidraw图表生成器

一种从自然语言描述生成excaldraw格式图表的技能。这种技能有助于在不需要手工绘图的情况下创建过程、系统、关系和想法的可视化表示。

何时使用此技能

当用户请求时使用此技能：

-“创建一个图表显示…”
-“制作流程图……”
-“想象……的过程”
-“绘制…的系统架构”
-“生成关于……的思维导图”
-“创建一个Excalidraw文件…”
-“展示……之间的关系”
-“绘制……的工作流程图”**支持的图表类型：**
-📊**流程图**：顺序流程，工作流程，决策树
-🔗**关系图**：实体关系，系统组件，依赖关系
-🧠**思维导图**：概念层次结构，头脑风暴结果，主题组织
-️**架构图**：系统设计、模块交互、数据流
-📈**数据流图(DFD)**：数据流可视化，数据转换过程
-🏊**业务流程（泳道）**：跨职能工作流，基于角色的流程流
-📦**类图**：面向对象的设计，类结构和关系
-🔄**序列图**：对象随时间的交互，消息流
-🗃️**ER图**：数据库实体关系，数据模型

# #先决条件-清晰描述应该可视化的内容
-识别关键实体、步骤或概念
-理解元素之间的关系或流程

##分步工作流程

步骤1：理解请求

分析用户描述，确定：
1. **图表类型**（流程图、关系、思维导图、架构）
2. 关键要素（实体、步骤、概念）
3. **关系**（流程、连接、层次）
4. **复杂度**（元素数量）

步骤2：选择合适的图表类型

|用户意图|图类型|示例关键字||-------------|--------------|------------------|
|流程、步骤、程序| **流程图** |“工作流”、“过程”、“步骤”、“程序”|
|连接、依赖、关联| **关系图** |“关系”、“连接”、“依赖”、“结构”|
|概念层次，头脑风暴| **思维导图** |“思维导图”，“概念”，“想法”，“分解”|
|系统设计、组件| **架构图** |“架构”、“系统”、“组件”、“模块”|
|数据流、转换过程| **数据流程图(DFD)** |“数据流”、“数据处理”、“数据转换”|
|跨职能流程、参与者职责| **业务流程（泳道）** |“业务流程”、“泳道”、“参与者”、“职责”|
|面向对象设计、类结构| **类图** |“类”、“继承”、“面向对象”、“对象模型”|
|交互序列、消息流| **序列图** |“序列”，“交互”，“消息”，“时间轴”|
|数据库设计、实体关系| **ER图** |“数据库”、“实体”、“关系”、“数据模型”|步骤3：提取结构化信息

流程图的* *:* *
-顺序步骤列表
-决策点（如有）
—起始点和结束点

**关系图：**
-Entities/nodes（名称+可选描述）
-实体之间的关系（from→to，带标签）

**思维导图：**
-中心议题
-主要分支机构（建议3-6个）
-每个分支的子主题（可选）

**数据流程图（DFD）：**
-数据源和目标（外部实体）
-流程（数据转换）
-数据存储（数据库、文件）
-数据流（箭头显示数据从左到右或从左上到右下的移动）
- **重要**：不表示流程顺序，仅表示数据流**对于业务流程（泳道）：**
—Actors/roles（部门、系统、人员）—显示为标题列
-流程通道（每个角色下方的垂直通道）
-过程框（每个通道内的活动）
-流程箭头（连接流程框，包括跨车道切换）

对于类图：**
-带有名称的类
-可见属性（+,-,#）
-具有可见性和参数的方法
关系：继承（实线+白色三角形）、实现（虚线+白色三角形）、关联（实线）、依赖（虚线）、聚合（实线+白色菱形）、组合（实线+填充菱形）
-多重符号（1,0 ..）1、1 . .*, *)对于序列图：**
-Objects/actors（顶部水平排列）
-生命线（每个物体的垂直线）
-讯息（生命线之间的水平箭头）
-同步消息（实线箭头），异步消息（虚线箭头）
-返回值（虚线箭头）
激活框（执行时生命线上的矩形）
-时间从上到下

**对于ER图：**
-实体（带有实体名称的矩形）
-属性（列在实体内）
-主键（带下划线或标有PK）
-外键（以FK标记）
-关系（连接实体的线）
—基数：1:1（一对一）、1:n（一对多）、N:M（多对多）
-Junction/associative实体用于多对多关系（虚线矩形）

###步骤4：生成Excalidraw JSON

用适当的元素创建`.excalidraw`文件：**可用的元素类型：**
-`rectangle`：实体、步骤、概念的方框
-`ellipse`：用于强调的可选形状
—`diamond`：决策点
—`arrow`：方向连接
-`text`：标签和注释

**关键属性设置：**
- **位置**:`x`，`y`坐标
- **尺寸**:`width`，`height`- **样式**:`strokeColor`，`backgroundColor`,`fillStyle`- **字体**:`fontFamily: 5`（Excalifont - **要求所有文本元素**）
—**文本**：标签内嵌文本
—**连接**：箭头为`points`数组

**重要**：所有文本元素必须使用`fontFamily: 5`（Excalifont）以保持一致的视觉外观。

###步骤5：格式化输出

构建完整的excaldraw文件：```json
{
  "type": "excalidraw",
  "version": 2,
  "source": "https://excalidraw.com",
  "elements": [
    // Array of diagram elements
  ],
  "appState": {
    "viewBackgroundColor": "#ffffff",
    "gridSize": 20
  },
  "files": {}
}
```
###步骤6：保存并提供说明

1. 保存为`<descriptive-name>.excalidraw`2. 告知用户如何打开：
-访问https://excalidraw.com-点击“打开”或拖放文件
-或使用excaldrawVS Code扩展

最佳实践

元素计数指南

|图类型|推荐计数|最大||--------------|-------------------|---------|
流程图步骤| 3-10 | 15 |
|关系实体| 3-8 | 12 |
|思维导图分支| 4-6 | 8 |
|每个分支的思维导图子主题| 2-4 | 6 |

###布局提示

1. **起始位置**：将重要元素居中，使用一致的间距
2. * * * *间隔:
-水平间距：元素之间200-300px
-行间垂直间距：100-150px
3. **颜色**：使用一致的配色方案
-主要元素：浅蓝色（`#a5d8ff`）
-次要元素：浅绿色（`#b2f2bb`）
-Important/Central：黄色（`#ffd43b`）
-Alerts/Warnings：浅红色（`#ffc9c9`）
4. **文本大小**:16-24px可读性
5. **字体**：所有文本元素始终使用`fontFamily: 5`（Excalifont）
6. **箭头样式：对于简单的流程使用直箭头，对于复杂的关系使用曲线箭头

复杂性管理**如果用户请求包含太多元素：**
-建议分解成多个图
-首先关注主要元素
-提供创建详细的子图

* *示例响应:* *```
"Your request includes 15 components. For clarity, I recommend:
1. High-level architecture diagram (6 main components)
2. Detailed diagram for each subsystem

Would you like me to start with the high-level view?"
```
提示和响应

示例1：简单流程图

**用户：**“创建用户注册流程图”

* *代理生成:* *
1. 提取步骤：“输入邮箱”→“验证邮箱”→“设置密码”→“完成”
2. 用4个矩形+ 3个箭头创建流程图
3. 保存为`user-registration-flow.excalidraw`例2：关系图

**User:**“用户、帖子和评论实体之间的关系图”

* *代理生成:* *
1. 实体：用户、帖子、评论
2. 关系：用户→发布（“创建”），用户→评论（“写”），发布→评论（“包含”）
3. 另存为`user-content-relationships.excalidraw`例子3：思维导图

**用户：**“关于机器学习概念的思维导图”

* *代理生成:* *
1. 中心：“机器学习”
2. 分支：监督学习，无监督学习，强化学习，深度学习
3. 每个分支下的子主题
4. 另存为`machine-learning-mindmap.excalidraw`# #故障排除

|问题|解决方案||-------|----------|
|元素重叠|增加坐标|之间的间距
|文本不适合盒子|增加盒子宽度或减小字体|
元素太多|分解成多个图|
|使用网格布局（rows/columns）或径向布局（思维导图）|
根据元素类型预先定义调色板

##先进技术

网格布局（用于关系图）```javascript
const columns = Math.ceil(Math.sqrt(entityCount));
const x = startX + (index % columns) * horizontalGap;
const y = startY + Math.floor(index / columns) * verticalGap;
```
放射状布局（适用于思维导图）```javascript
const angle = (2 * Math.PI * index) / branchCount;
const x = centerX + radius * Math.cos(angle);
const y = centerY + radius * Math.sin(angle);
```
自动生成的id
使用时间戳+随机字符串作为唯一的id：```javascript
const id = Date.now().toString(36) + Math.random().toString(36).substr(2);
```
##输出格式

总是提供:
1. ✅完整的`.excalidraw`JSON文件
2. 📊创建的摘要
3. 📝元素计数
4. 💡opening/editing的使用说明

* *例总结:* *```
Created: user-workflow.excalidraw
Type: Flowchart
Elements: 7 rectangles, 6 arrows, 1 title text
Total: 14 elements

To view:
1. Visit https://excalidraw.com
2. Drag and drop user-workflow.excalidraw
3. Or use File → Open in Excalidraw VS Code extension
```
验证检查表

发货前：
—[]所有元素具有唯一的id
-[]坐标防止重叠
-[]文字可读（字体大小16+）
-[] **所有文本元素使用`fontFamily: 5`(Excalifont)**
-[]箭头逻辑连接
—[]配色方案一致
—[]文件是有效的JSON
[]元素数量合理（<20）

图标库（可选增强）

对于专门的图表（例如，AWS/GCP/Azure架构图），您可以使用Excalidraw中的预制图标库。这提供了专业的，标准化的图标，而不是基本的形状。

当用户请求图标时

**如果用户要求AWS/cloud架构图或提到要使用特定的图标：**1. **检查库是否存在**：查找`libraries/<library-name>/reference.md`2. **如果库存在**：继续使用图标（参见下面的AI助手工作流程）
3. **如果库不存在**：返回安装说明：   ```
   To use [AWS/GCP/Azure/etc.] architecture icons, please follow these steps:
   
   1. Visit https://libraries.excalidraw.com/
   2. Search for "[AWS Architecture Icons/etc.]" and download the .excalidrawlib file
   3. Create directory: skills/excalidraw-diagram-generator/libraries/[icon-set-name]/
   4. Place the downloaded file in that directory
   5. Run the splitter script:
      python skills/excalidraw-diagram-generator/scripts/split-excalidraw-library.py skills/excalidraw-diagram-generator/libraries/[icon-set-name]/
   
   This will split the library into individual icon files for efficient use.
   After setup is complete, I can create your diagram using the actual AWS/cloud icons.
   
   Alternatively, I can create the diagram now using simple shapes (rectangles, ellipses) 
   which you can later replace with icons manually in Excalidraw.
   ```
###用户设置说明（详细）

步骤1：创建图书馆目录**```bash
mkdir -p skills/excalidraw-diagram-generator/libraries/aws-architecture-icons
```
**第二步：下载库**
—访问：https://libraries.excalidraw.com/-搜索您想要的图标集（例如，“AWS架构图标”）
—单击“下载”获取`.excalidrawlib`文件
-示例类别（可用性不同，请在现场确认）：
-云服务图标
-UI/MaterialICONS
-流程图符号

**步骤3：放置库文件**
-重命名下载的文件以匹配目录名称（例如，`aws-architecture-icons.excalidrawlib`）
—移动到步骤1中创建的目录

**步骤4：运行Splitter Script**```bash
python skills/excalidraw-diagram-generator/scripts/split-excalidraw-library.py skills/excalidraw-diagram-generator/libraries/aws-architecture-icons/
```
**步骤5：验证安装**
运行脚本后，检查是否存在如下结构：```
skills/excalidraw-diagram-generator/libraries/aws-architecture-icons/
  aws-architecture-icons.excalidrawlib  (original)
  reference.md                          (generated - icon lookup table)
  icons/                                (generated - individual icon files)
    API-Gateway.json
    CloudFront.json
    EC2.json
    Lambda.json
    RDS.json
    S3.json
    ...
```
AI助手工作流

**当图标库在`libraries/`:**中可用时

**推荐方法：使用Python脚本（高效可靠）**

该存储库包含自动处理图标集成的Python脚本：

1. **创建基本图结构**：
-创建`.excalidraw`文件与基本布局（标题，框，区域）
-这建立了画布和整体结构

2. **使用Python脚本添加图标**：   ```bash
   python skills/excalidraw-diagram-generator/scripts/add-icon-to-diagram.py \
     <diagram-path> <icon-name> <x> <y> [--label "Text"] [--library-path PATH]
   ```
-通过`.excalidraw.edit`编辑默认是启用的，以避免覆盖问题；将`--no-use-edit-suffix`设置为禁用。

* * * *例子:   ```bash
   # Add EC2 icon at position (400, 300) with label
   python scripts/add-icon-to-diagram.py diagram.excalidraw EC2 400 300 --label "Web Server"
   
   # Add VPC icon at position (200, 150)
   python scripts/add-icon-to-diagram.py diagram.excalidraw VPC 200 150
   
   # Add icon from different library
   python scripts/add-icon-to-diagram.py diagram.excalidraw Compute-Engine 500 200 \
     --library-path libraries/gcp-icons --label "API Server"
   ```
3. **添加连接箭头**：   ```bash
   python skills/excalidraw-diagram-generator/scripts/add-arrow.py \
     <diagram-path> <from-x> <from-y> <to-x> <to-y> [--label "Text"] [--style solid|dashed|dotted] [--color HEX]
   ```
-通过`.excalidraw.edit`编辑默认是启用的，以避免覆盖问题；将`--no-use-edit-suffix`设置为禁用。

* * * *例子:   ```bash
   # Simple arrow from (300, 250) to (500, 300)
   python scripts/add-arrow.py diagram.excalidraw 300 250 500 300
   
   # Arrow with label
   python scripts/add-arrow.py diagram.excalidraw 300 250 500 300 --label "HTTPS"
   
   # Dashed arrow with custom color
   python scripts/add-arrow.py diagram.excalidraw 400 350 600 400 --style dashed --color "#7950f2"
   ```
4. * * * *工作流程总结:   ```bash
   # Step 1: Create base diagram with title and structure
   # (Create .excalidraw file with initial elements)
   
   # Step 2: Add icons with labels
   python scripts/add-icon-to-diagram.py my-diagram.excalidraw "Internet-gateway" 200 150 --label "Internet Gateway"
   python scripts/add-icon-to-diagram.py my-diagram.excalidraw VPC 250 250
   python scripts/add-icon-to-diagram.py my-diagram.excalidraw ELB 350 300 --label "Load Balancer"
   python scripts/add-icon-to-diagram.py my-diagram.excalidraw EC2 450 350 --label "EC2 Instance"
   python scripts/add-icon-to-diagram.py my-diagram.excalidraw RDS 550 400 --label "Database"
   
   # Step 3: Add connecting arrows
   python scripts/add-arrow.py my-diagram.excalidraw 250 200 300 250  # Internet → VPC
   python scripts/add-arrow.py my-diagram.excalidraw 300 300 400 300  # VPC → ELB
   python scripts/add-arrow.py my-diagram.excalidraw 400 330 500 350  # ELB → EC2
   python scripts/add-arrow.py my-diagram.excalidraw 500 380 600 400  # EC2 → RDS
   ```
** Python脚本方法的好处**：
-✅**没有令牌消耗**：图标JSON数据（每个200-1000行）永远不会进入AI上下文
-✅**精确的转换**：坐标计算处理确定性
—✅**ID管理**：自动生成UUID，避免冲突
-✅**可靠**：无坐标误算或ID冲突风险
-✅**快速**：直接文件操作，没有解析开销
-✅**可重用**：与您提供的任何Excalidraw库一起工作

**可选：手动图标集成（不推荐）**

仅在Python脚本不可用时使用：

1. **检查库**：   ```
   List directory: skills/excalidraw-diagram-generator/libraries/
   Look for subdirectories containing reference.md files
   ```
2. * *读reference.md* *:   ```
   Open: libraries/<library-name>/reference.md
   This is lightweight (typically <300 lines) and lists all available icons
   ```
3. **查找相关图标**：   ```
   Search the reference.md table for icon names matching diagram needs
   Example: For AWS diagram with EC2, S3, Lambda → Find "EC2", "S3", "Lambda" in table
   ```
4. **加载特定图标数据**（警告：大文件）：   ```
   Read ONLY the needed icon files:
   - libraries/aws-architecture-icons/icons/EC2.json (200-300 lines)
   - libraries/aws-architecture-icons/icons/S3.json (200-300 lines)
   - libraries/aws-architecture-icons/icons/Lambda.json (200-300 lines)
   Note: Each icon file is 200-1000 lines - this consumes significant tokens
   ```
5. **提取和转换元素**：   ```
   Each icon JSON contains an "elements" array
   Calculate bounding box (min_x, min_y, max_x, max_y)
   Apply offset to all x/y coordinates
   Generate new unique IDs for all elements
   Update groupIds references
   Copy transformed elements into your diagram
   ```
6. **定位图标并添加连接**：   ```
   Adjust x/y coordinates to position icons correctly in the diagram
   Update IDs to ensure uniqueness across diagram
   Add connecting arrows and labels as needed
   ```
**手动集成挑战**：
-⚠️高标记消耗（每个图标200-1000行×图标数量）
-⚠️复坐标变换计算
-⚠️处理不当有ID冲突的风险
-⚠️耗时的图表与许多图标

示例：创建带有图标的AWS图表

**请求**：“创建包含Internet网关、VPC、ELB、EC2和RDS的AWS架构图”

**推荐工作流程（使用Python脚本）**：
**请求**：“创建包含Internet网关、VPC、ELB、EC2和RDS的AWS架构图”

**推荐工作流程（使用Python脚本）**：```bash
# Step 1: Create base diagram file with title
# Create my-aws-diagram.excalidraw with basic structure (title, etc.)

# Step 2: Check icon availability
# Read: libraries/aws-architecture-icons/reference.md
# Confirm icons exist: Internet-gateway, VPC, ELB, EC2, RDS

# Step 3: Add icons with Python script
python scripts/add-icon-to-diagram.py my-aws-diagram.excalidraw "Internet-gateway" 150 100 --label "Internet Gateway"
python scripts/add-icon-to-diagram.py my-aws-diagram.excalidraw VPC 200 200
python scripts/add-icon-to-diagram.py my-aws-diagram.excalidraw ELB 350 250 --label "Load Balancer"
python scripts/add-icon-to-diagram.py my-aws-diagram.excalidraw EC2 500 300 --label "Web Server"
python scripts/add-icon-to-diagram.py my-aws-diagram.excalidraw RDS 650 350 --label "Database"

# Step 4: Add connecting arrows
python scripts/add-arrow.py my-aws-diagram.excalidraw 200 150 250 200  # Internet → VPC
python scripts/add-arrow.py my-aws-diagram.excalidraw 265 230 350 250  # VPC → ELB
python scripts/add-arrow.py my-aws-diagram.excalidraw 415 280 500 300  # ELB → EC2
python scripts/add-arrow.py my-aws-diagram.excalidraw 565 330 650 350 --label "SQL" --style dashed

# Result: Complete diagram with professional AWS icons, labels, and connections
```
* * * *好处:
—无需手动坐标计算
-图标数据没有令牌消耗
-确定性的、可靠的结果
-易于迭代和调整位置

**可选择的工作流程（手动，如果脚本不可用）**：
1. 检查：`libraries/aws-architecture-icons/reference.md`存在→是
2. 读取reference.md→查找internet网关，VPC， ELB, EC2， RDS的条目
3. 负载:
-`icons/Internet-gateway.json`（298行）
-`icons/VPC.json`（550行）
-`icons/ELB.json`（363行）
-`icons/EC2.json`（231行）
-`icons/RDS.json`（大小相近）
**总共：~2000+行JSON来处理**
4. 从每个JSON中提取元素
5. 计算每个图标的边界框和偏移量
6. 变换所有坐标（x, y）进行定位
7. 为所有元素生成唯一的id
8. 添加显示数据流的箭头
9. 添加文本标签
10. 生成最终的`.excalidraw`文件**手动方法的挑战**：
-高token消耗（~2000-5000行）
-复杂的坐标数学
- ID冲突的风险

支持的图标库（示例-验证可用性）

-此工作流适用于您提供的任何有效的`.excalidrawlib`文件。
你可以在https://libraries.excalidraw.com/:上找到一些库类别的例子
-云服务图标
- Kubernetes / infrastructure图标
- UI /材质图标
-流程图/图表符号
-网络图图标
-可用性和命名可以改变；在使用之前，请在网站上确认库的确切名称。

###后退：没有图标可用**如果没有设置图标库：**
-使用基本形状（矩形，椭圆，箭头）创建图表
—使用颜色编码和文本标签来区分组件
-通知用户以后可以添加图标或为将来的图表设置库
-图表仍然是功能性和清晰的，只是在视觉上不那么抛光

# #引用

参见捆绑参考资料：
-`references/excalidraw-schema.md`-完整的excaldraw JSON模式
-`references/element-types.md`-详细的元素类型规范
-`templates/flowchart-template.json`-基本流程图starter
-`templates/relationship-template.json`-关系图启动器
-`templates/mindmap-template.json`-思维导图启动器
-`scripts/split-excalidraw-library.py`-分割`.excalidrawlib`文件的工具
-`scripts/README.md`-库工具文档
-`scripts/.gitignore`-防止提交本地Python工件

# #的局限性—复杂曲线简化为straight/basic曲线
-手绘粗糙度设置为默认值(1)
-在自动生成中不支持嵌入图像
—最大推荐元素：每张图20个
-没有自动碰撞检测（使用间距指南）

未来的增强

潜在的改进:
-自动布局优化算法
—从Mermaid/PlantUML语法导入
-模板库扩展
-生成后的交互式编辑