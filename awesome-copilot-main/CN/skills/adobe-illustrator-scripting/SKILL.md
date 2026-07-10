---
name: adobe-illustrator-scripting
description: 'Write, debug, and optimize Adobe Illustrator automation scripts using ExtendScript (JavaScript/JSX). Use when creating or modifying scripts that manipulate documents, layers, paths, text frames, colors, symbols, artboards, or any Illustrator DOM objects. Covers the complete JavaScript object model, coordinate system, measurement units, export workflows, and scripting best practices.'
---
# Adobe Illustrator脚本

通过ExtendScript （JavaScript/JSX）自动化Adobe Illustrator的专家指导。该技能涵盖了Illustrator脚本对象模型、所有主要API对象、代码模式以及编写生产质量`.jsx`脚本的最佳实践。

##资产捆绑

- [`references/object-model-quick-reference.md`](references/object-model-quick-reference.md)：在编写或调试脚本时，将此作为快速查找Illustrator脚本对象模型、通用文档和页面项类型以及相关DOM概念的工具。
-`scripts/`：包含示例Illustrator自动化脚本，您可以将其用作通用任务的起点或实现模式，例如文档操作，导出，批处理和DOM使用。当您需要使用JSX模式或希望在调试时比较行为时，请查看并调整这些示例。
何时使用此技能编写新的Illustrator自动化脚本（`.jsx`或`.js`文件）
-调试或修复现有的Illustrator ExtendScript代码
-以编程方式操作文档、图层、页面项、路径、文本或颜色
-批量处理Illustrator文件或从数据生成艺术品
-导出文件到各种格式（PDF， SVG， PNG， EPS等）
-使用Illustrator DOM（应用程序，文档，图层，PathItem，文本框架等）
-使用变量和数据集创建数据驱动图形
-自动化打印工作流程与脚本打印选项

# #先决条件-已安装Adobe Illustrator CC或更高版本
-基本的JavaScript知识（ExtendScript是基于es3与Adobe扩展）
—脚本通过File > Scripts >其他脚本、Scripts菜单或放在Startup Scripts文件夹中执行
- ExtendScript Toolkit （ESTK）或任何文本编辑器可用于编写`.jsx`文件

##脚本环境

语言和文件扩展名

|语言|扩展|平台||---|---|---|
|ExtendScript/JavaScript|`.jsx`、`.js`| Windows、macOS |
| AppleScript |`.scpt`| macOS仅|
| VBScript |`.vbs`|仅Windows |

**此技能重点关注ExtendScript/JavaScript**作为跨平台，最广泛使用的选项。

###执行脚本—**Scripts菜单**:File > Scripts列出了application Scripts文件夹中的脚本
- **其他脚本**：文件>脚本>其他脚本浏览和运行任何`.jsx`文件
—**Startup Scripts**：将脚本放在Startup Scripts文件夹中，以便在启动时自动运行
- **目标指令**：从ESTK或外部工具运行时，使用`#target illustrator`开始脚本
**`#targetengine`指令**：使用`#targetengine "session"`在脚本执行期间持久化变量
- **外部调用**：脚本经常从Illustrator外部启动-通过shell脚本，任务运行器，CI作业，ExtendScript Toolkit (`ExtendScript Toolkit.exe -run script.jsx`)，或`BridgeTalk`消息从其他Adobe应用程序。参见[外部调用&参数传递]（# External - Invocation——参数传递）。

命名约定（JavaScript）-对象和属性使用**camelCase**:`activeDocument`，`pathItems`,`textFrames`—`app`全局引用`Application`对象
—集合索引为**从零开始的**:`documents[0]`为最前面的文档
-使用`typename`属性在运行时识别对象类型

对象模型概述

Illustrator DOM遵循严格的包含层次结构：```
Application (app)
├── activeDocument / documents[]
│   ├── layers[]
│   │   ├── pageItems[] (all artwork)
│   │   ├── pathItems[]
│   │   ├── compoundPathItems[]
│   │   ├── textFrames[]
│   │   ├── placedItems[]
│   │   ├── rasterItems[]
│   │   ├── meshItems[]
│   │   ├── pluginItems[]
│   │   ├── graphItems[]
│   │   ├── symbolItems[]
│   │   ├── nonNativeItems[]
│   │   ├── legacyTextItems[]
│   │   └── groupItems[]
│   ├── artboards[]
│   ├── views[]
│   ├── selection (array of selected items)
│   ├── swatches[], spots[], gradients[], patterns[]
│   ├── graphicStyles[], brushes[], symbols[]
│   ├── textFonts[] (via app.textFonts)
│   ├── stories[], characterStyles[], paragraphStyles[]
│   ├── variables[], datasets[]
│   └── inkList[], printOptions
├── preferences
├── printerList[]
└── textFonts[]
```
顶级对象

—**Application** (`app`)：根对象。提供对文档、首选项、字体和打印机的访问。关键属性：`activeDocument`，`documents`,`textFonts`,`printerList`,`userInteractionLevel`,`version`。
—**Document**：表示打开的`.ai`文件。关键属性：`layers`、`pageItems`、`selection`、`activeLayer`、`width`、`height`、`rulerOrigin`、`documentColorSpace`。关键方法：`saveAs()`，`exportFile()`,`close()`,`print()`。
- **Layer**：绘图层。关键属性：`pageItems`、`pathItems`、`textFrames`、`visible`、`locked`、`opacity`、`name`、`zOrderPosition`、`color`。

测量单位和坐标

# # #单位

所有脚本API值使用**点**（72点= 1英寸）。转换其他单位：

|单位|转换||---|---|
|英寸|乘以72 |
厘米|乘以28.346 |
毫米乘以2.834645
Picas乘以12|

字距、跟踪和`aki`属性使用em单位（em的千分之一，与字体大小成比例）。

坐标系统

-对于**脚本文档**，原点`(0,0)`位于画板的**左下方**
- X从左到右增加；Y从下往上递增
-页面项的`position`属性是它的边界框的左上角`[x, y]`-最大页面条目width/height: 16348点

艺术项目边界

每个页面项有三个边界矩形：

—`geometricBounds`：不包括笔画宽度`[left, top, right, bottom]`-`visibleBounds`：包括笔画宽度
—`controlBounds`：包括control/direction点

处理文档

创建和打开```javascript
// Create a new document
var doc = app.documents.add();

// Create with a preset
var preset = new DocumentPreset();
preset.width = 612;  // 8.5 inches
preset.height = 792; // 11 inches
preset.colorMode = DocumentColorSpace.CMYK;
var doc = app.documents.addDocument("Print", preset);

// Open an existing file
var fileRef = new File("/path/to/file.ai");
var doc = app.open(fileRef);
```
###保存和导出```javascript
// Save as Illustrator format
var saveOpts = new IllustratorSaveOptions();
saveOpts.compatibility = Compatibility.ILLUSTRATOR17; // CC
doc.saveAs(new File("/path/to/output.ai"), saveOpts);

// Export as PDF
var pdfOpts = new PDFSaveOptions();
pdfOpts.compatibility = PDFCompatibility.ACROBAT7;
pdfOpts.preserveEditability = false;
doc.saveAs(new File("/path/to/output.pdf"), pdfOpts);

// Export as PNG
var pngOpts = new ExportOptionsPNG24();
pngOpts.horizontalScale = 300;
pngOpts.verticalScale = 300;
pngOpts.transparency = true;
doc.exportFile(new File("/path/to/output.png"), ExportType.PNG24, pngOpts);

// Export as SVG
var svgOpts = new ExportOptionsSVG();
svgOpts.fontType = SVGFontType.OUTLINEFONT;
doc.exportFile(new File("/path/to/output.svg"), ExportType.SVG, svgOpts);
```
使用路径和形状

内置形状方法`pathItems`集合为常见形状提供了方便的方法：```javascript
var doc = app.activeDocument;
var layer = doc.activeLayer;

// Rectangle: rectangle(top, left, width, height)
var rect = layer.pathItems.rectangle(500, 100, 200, 150);

// Rounded rectangle: roundedRectangle(top, left, width, height, hRadius, vRadius)
var rrect = layer.pathItems.roundedRectangle(500, 100, 200, 150, 20, 20);

// Ellipse: ellipse(top, left, width, height)
var oval = layer.pathItems.ellipse(400, 200, 100, 100);

// Polygon: polygon(centerX, centerY, radius, sides)
var hex = layer.pathItems.polygon(300, 300, 50, 6);

// Star: star(centerX, centerY, radius, innerRadius, points)
var star = layer.pathItems.star(300, 300, 50, 25, 5);
```
使用坐标数组自由生成路径```javascript
var doc = app.activeDocument;
var path = doc.pathItems.add();
path.setEntirePath([[100, 100], [200, 200], [300, 100]]);
path.closed = false;
path.stroked = true;
path.strokeWidth = 2;
```
使用路径点对象自由生成路径```javascript
var doc = app.activeDocument;
var path = doc.pathItems.add();

var point1 = path.pathPoints.add();
point1.anchor = [100, 100];
point1.leftDirection = [100, 100];
point1.rightDirection = [150, 150];
point1.pointType = PointType.SMOOTH;

var point2 = path.pathPoints.add();
point2.anchor = [300, 100];
point2.leftDirection = [250, 150];
point2.rightDirection = [300, 100];
point2.pointType = PointType.SMOOTH;

path.closed = false;
```
###路径属性```javascript
var item = doc.pathItems[0];
item.filled = true;
item.stroked = true;
item.strokeWidth = 1.5;
item.strokeCap = StrokeCap.ROUNDENDCAP;
item.strokeJoin = StrokeJoin.ROUNDENDJOIN;
item.opacity = 80;
item.closed = true;
```
##使用颜色

颜色对象```javascript
// RGB Color (values 0-255)
var red = new RGBColor();
red.red = 255;
red.green = 0;
red.blue = 0;

// CMYK Color (values 0-100)
var cyan = new CMYKColor();
cyan.cyan = 100;
cyan.magenta = 0;
cyan.yellow = 0;
cyan.black = 0;

// Grayscale (0-100, 0 = black)
var gray = new GrayColor();
gray.gray = 50;

// Lab Color
var lab = new LabColor();
lab.l = 50;
lab.a = 20;
lab.b = -30;

// No color (transparent)
var none = new NoColor();
```
应用颜色```javascript
var item = doc.pathItems[0];
item.fillColor = red;
item.strokeColor = cyan;

// Gradient fill
var gradient = doc.gradients.add();
gradient.type = GradientType.LINEAR;
gradient.gradientStops[0].color = red;
gradient.gradientStops[1].color = cyan;

var gradColor = new GradientColor();
gradColor.gradient = gradient;
item.fillColor = gradColor;
```
专色和色板```javascript
// Create a spot color
var spot = doc.spots.add();
spot.name = "My Spot Color";
spot.color = red; // Base color definition

var spotColor = new SpotColor();
spotColor.spot = spot;
spotColor.tint = 100;

item.fillColor = spotColor;

// Access a swatch by name
var swatch = doc.swatches.getByName("PANTONE 185 C");
item.fillColor = swatch.color;
```
##使用文本

文本框架类型```javascript
var doc = app.activeDocument;

// Point text
var pointText = doc.textFrames.add();
pointText.contents = "Hello World!";
pointText.position = [100, 500];

// Area text (text inside a path)
var rectPath = doc.pathItems.rectangle(500, 100, 200, 100);
var areaText = doc.textFrames.areaText(rectPath);
areaText.contents = "Text inside a rectangle shape.";

// Path text (text along a path)
var curvePath = doc.pathItems.add();
curvePath.setEntirePath([[50, 300], [150, 400], [250, 300]]);
var pathText = doc.textFrames.pathText(curvePath);
pathText.contents = "Text on a path";
```
字符和段落格式```javascript
var tf = doc.textFrames[0];
var textRange = tf.textRange;

// Character attributes
var charAttr = textRange.characterAttributes;
charAttr.size = 24;           // Font size in points
charAttr.textFont = app.textFonts.getByName("ArialMT");
charAttr.fillColor = red;
charAttr.tracking = 50;       // Em units
charAttr.horizontalScale = 100;
charAttr.verticalScale = 100;
charAttr.baselineShift = 0;

// Paragraph attributes
var paraAttr = textRange.paragraphAttributes;
paraAttr.justification = Justification.CENTER;
paraAttr.firstLineIndent = 0;
paraAttr.leftIndent = 0;
paraAttr.spaceBefore = 0;
paraAttr.spaceAfter = 0;
```
访问文本内容```javascript
var tf = doc.textFrames[0];

// Access sub-ranges
var firstChar = tf.characters[0];
var firstWord = tf.words[0];
var firstPara = tf.paragraphs[0];
var firstLine = tf.lines[0];

// Modify specific ranges
tf.words[0].characterAttributes.size = 36;
tf.paragraphs[0].paragraphAttributes.justification = Justification.LEFT;
```
线程文本框架```javascript
var frame1 = doc.textFrames.areaText(path1);
var frame2 = doc.textFrames.areaText(path2);

// Link frames so text flows from frame1 to frame2
frame1.nextFrame = frame2;

// Stories represent the full text across threaded frames
var storyCount = doc.stories.length;
var fullText = doc.stories[0].textRange.contents;
```
##使用图层```javascript
var doc = app.activeDocument;

// Create a layer
var newLayer = doc.layers.add();
newLayer.name = "Background";
newLayer.visible = true;
newLayer.locked = false;
newLayer.opacity = 100;

// Access existing layers
var topLayer = doc.layers[0];
var layerByName = doc.layers.getByName("Background");

// Move items between layers
var item = doc.pathItems[0];
item.move(newLayer, ElementPlacement.PLACEATBEGINNING);

// Reorder layers
newLayer.zOrder(ZOrderMethod.SENDTOBACK);
```
##使用选择```javascript
// Get current selection
var sel = app.activeDocument.selection;

// Iterate selected items
for (var i = 0; i < sel.length; i++) {
    var item = sel[i];
    // Check type using typename
    if (item.typename === "PathItem") {
        item.fillColor = red;
    } else if (item.typename === "TextFrame") {
        item.contents = "Modified";
    }
}

// Select an item programmatically
doc.pathItems[0].selected = true;

// Deselect all
doc.selection = null;
```
##使用符号```javascript
// Place a symbol instance
var sym = doc.symbols.getByName("MySymbol");
var instance = doc.symbolItems.add(sym);
instance.position = [200, 400];

// Access symbol definition
var symDef = instance.symbol;

// Break link to symbol (expand to regular art)
instance.breakLink();
```
# #转换```javascript
var item = doc.pathItems[0];

// Rotate 45 degrees around center
item.rotate(45);

// Scale to 50% width, 75% height
item.resize(50, 75);

// Translate (move) by 100 points right and 50 points up
item.translate(100, 50);

// Using a transformation matrix
var matrix = app.getIdentityMatrix();
matrix = app.concatenateRotationMatrix(matrix, 30);
matrix = app.concatenateScaleMatrix(matrix, 150, 150);
item.transform(matrix);
```
##使用画板```javascript
var doc = app.activeDocument;

// Access artboards
var ab = doc.artboards[0];
var rect = ab.artboardRect; // [left, top, right, bottom]

// Create a new artboard
var newAB = doc.artboards.add([0, 0, 612, 792]); // Letter size
newAB.name = "Page 2";

// Set active artboard
doc.artboards.setActiveArtboardIndex(1);
```
数据驱动图形（变量和数据集）```javascript
// Variables link document items to data fields
var v = doc.variables.add();
v.kind = VariableKind.TEXTUAL;
v.name = "headline";

// Link a text frame to the variable
var tf = doc.textFrames[0];
tf.contentVariable = v;

// Create datasets for batch content
var ds = doc.dataSets.add();
ds.name = "Version 1";
// Dataset captures current variable bindings

// Switch datasets to swap content
doc.dataSets[0].display();
```
# #印刷```javascript
var doc = app.activeDocument;
var opts = new PrintOptions();

opts.printPreset = "Default";

// Paper options
var paperOpts = new PrintPaperOptions();
paperOpts.name = "Letter";
opts.paperOptions = paperOpts;

// Job options
var jobOpts = new PrintJobOptions();
jobOpts.copies = 1;
jobOpts.designation = PrintArtworkDesignation.VISIBLELAYERS;
opts.jobOptions = jobOpts;

doc.print(opts);
```
##用户交互级别

控制Illustrator在脚本执行期间是否显示对话框：```javascript
// Suppress all dialogs
app.userInteractionLevel = UserInteractionLevel.DONTDISPLAYALERTS;

// Perform operations that might prompt dialogs...
doc.close(SaveOptions.DONOTSAVECHANGES);

// Restore dialog display
app.userInteractionLevel = UserInteractionLevel.DISPLAYALERTS;
```
使用方法（javascript特有的）

当调用具有多个可选参数的方法时，使用`undefined`跳过中间参数：```javascript
// rotate(angle, [changePositions], [changeFillPatterns], [changeFillGradients], ...)
item.rotate(30, undefined, undefined, true);
```
外部调用和参数传递

Illustrator脚本通常是从应用程序外部启动的
shell脚本、调度器、构建管道、ExtendScript Toolkit或
来自其他Creative Cloud应用程序的`BridgeTalk`消息。执行
这些启动器下的环境不同于应用程序中的*File >
脚本以几种方式路径，经常破坏其他正确的代码。

在外部发射装置下不可靠

ExtendScript Toolkit的`-run`调用和`BridgeTalk.send()`则不会
将任意启动器参数转发到脚本的顶层`arguments[]`数组。在许多配置中，数组包含单个`[object BridgeTalk]`元素，而不是调用者传递的值，如
证明如下:```javascript
// At top of script
var passed = (typeof arguments !== "undefined") ? arguments : [];
for (var i = 0; i < passed.length; i++) {
    $.writeln("arg[" + i + "] = " + passed[i]);
    // Often prints: arg[0] = [object BridgeTalk]
}
```
**不要依赖于`arguments[]`所需的输入时，脚本是
启动外部。**使用以下较可靠的渠道之一。

Sidecar参数文件

当脚本在外部启动器和错误源下失败时
是不是很明显，倒回一个挎斗文件：有没有调用者写一个小的
文本文件在一个已知的绝对路径，并在启动时读取它。这是
不管启动器的怪癖，很容易检查失败后运行。```javascript
var SIDECAR_PATH = "C:/Users/userName/job.args.txt";

function readSidecar(path) {
    var f = new File(path);
    if (!f.exists || !f.open("r")) return null;
    var lines = [];
    while (!f.eof) {
        var ln = f.readln();
        if (ln && !/^\s*$/.test(ln)) lines.push(ln);
    }
    f.close();
    return {
        input:  lines[0],
        output: lines[1],
        mode:   lines[2]
    };
}
```
`key=value`格式同样可行，并避免了位置脆弱性：```text
input=C:/path/to/input.ai
output=C:/path/to/output.pdf
mode=preview
```
环境变量`$.getenv("NAME")`返回对**Illustrator可见的环境变量
进程**，而不是启动器的。如果启动器需要Illustrator来显示
值时，它必须在系统范围内或在Illustrator的父类中设置变量
启动前的环境。对于每个调用值，建议使用sidecar
文件。`$.fileName`和`File($.fileName).parent`在应用程序内执行时，`$.fileName`是控件的绝对路径
运行script和`File($.fileName).parent`将生成脚本的文件夹。
在一些外部发射装置下（特别是ESTK`-run`）`$.fileName`可以
空，导致相对路径解析静默失败。```javascript
// Fragile: returns null under some launchers
var here = $.fileName ? File($.fileName).parent : null;
var sidecar = here ? new File(here.fsName + "/job.args.txt") : null;

// Robust: hardcode a known absolute path or fall back to a stable location
var sidecar = new File("C:/Users/userName/job.args.txt");
if (!sidecar.exists) sidecar = new File(Folder.temp.fsName + "/job.args.txt");
```
诊断日志记录到绝对路径

无声失败是常见的，因为对话框被抑制，启动器
不得表面`$.writeln`输出。将纯文本日志写入已知的
绝对路径，以便事后检查运行。创建父组件
文件夹按需调用，因此第一次调用不会因为丢失的目录而失败。```javascript
var LOG_PATH = "C:/Users/userName/logs/job.log";

function log(msg) {
    try {
        var f = new File(LOG_PATH);
        try { if (!f.parent.exists) f.parent.create(); } catch (eDir) {}
        if (f.open("a")) {
            f.writeln("[" + new Date() + "] " + msg);
            f.close();
        }
    } catch (e) {}
}
```
###在`try { ... } catch`中包装入口点

外部启动的脚本经常在没有任何可见迹象的情况下失败。一个
将错误写入日志文件转换的顶级`try`/`catch`无声故障变成一条可检查的线。```javascript
try {
    main();
} catch (err) {
    log("FATAL: " + err + (err && err.line ? " line=" + err.line : ""));
}
```
抑制用户交互

外部呼叫者无法接听对话。在任何DOM工作之前禁用它们
避免在脚本中使用`alert()`/`confirm()`/`prompt()`他一头栽了下去。```javascript
app.userInteractionLevel = UserInteractionLevel.DONTDISPLAYALERTS;
```
###显式保存

关闭或让Illustrator返回其空闲状态不会保存
工作文件。在所有DOM编辑之后，调用`doc.saveAs(...)`（或`doc.save()`）
显式地并记录它是否成功。```javascript
var opts = new IllustratorSaveOptions();
opts.compatibility = Compatibility.ILLUSTRATOR17;
doc.saveAs(new File(doc.fullName.fsName), opts);
```
##常见模式

迭代文档中的所有页面项```javascript
function processAllItems(doc) {
    for (var i = 0; i < doc.pageItems.length; i++) {
        var item = doc.pageItems[i];
        // Process based on type
        switch (item.typename) {
            case "PathItem":
                // handle path
                break;
            case "TextFrame":
                // handle text
                break;
            case "GroupItem":
                // handle group (may contain nested items)
                break;
        }
    }
}
```
在编辑前递归地解锁图层和组

锁定层或任何锁定的祖先层（父组、剪辑组、子层）
将导致编辑抛出`Error: Target layer cannot be modified`。走在
完整的层次结构，并在执行DOM之前清除`locked`/`hidden`标志
修改。```javascript
function unlockAll(doc) {
    function visitLayers(layers) {
        for (var i = 0; i < layers.length; i++) {
            var lyr = layers[i];
            try { lyr.locked = false; lyr.visible = true; } catch (e) {}
            visitItems(lyr);
            if (lyr.layers && lyr.layers.length) visitLayers(lyr.layers);
        }
    }
    function visitItems(container) {
        var items = container.pageItems;
        for (var j = 0; j < items.length; j++) {
            var it = items[j];
            try { it.locked = false; it.hidden = false; } catch (e) {}
            if (it.typename === "GroupItem") visitItems(it);
        }
    }
    visitLayers(doc.layers);
}
```
###替换链接图像后面的文件（Relink）`PlacedItem.file = newFile`替换链接的图像，同时保留
父节点，堆叠顺序，以及（在重新应用之后）边界。* *`RasterItem`不公开可写的`file`属性**，所以当占位符是
栅格你必须添加一个新的`PlacedItem`在相同的父，复制边界，
然后去掉原来的。```javascript
function relinkOrRebuild(item, newFile) {
    var bounds = item.geometricBounds.slice();
    var parent = item.parent;
    var name   = item.name;

    if (item.typename === "PlacedItem") {
        item.file = newFile;
        item.geometricBounds = bounds;
        return item;
    }

    // RasterItem path: rebuild as a linked PlacedItem in the same parent.
    var fresh = parent.placedItems.add();
    fresh.file = newFile;
    fresh.geometricBounds = bounds;
    if (name) try { fresh.name = name; } catch (e) {}
    fresh.move(item, ElementPlacement.PLACEBEFORE);
    item.remove();
    return fresh;
}
```
放置SVG内容（Copy/Paste模式）`PlacedItem.file`接受光栅格式和AI/PDF， **但不接受SVG**。设置
它对一个`.svg`文件抛出'无法设置放置项的文件，是文件
路径是否有效？将SVG图像带入文档的可靠方法
是将SVG作为单独的文档打开、全选、复制、关闭和粘贴
放入工作文档中。```javascript
function placeSVG(targetDoc, svgFile, targetLayer) {
    var donor = app.open(svgFile);
    app.executeMenuCommand("selectall");
    app.executeMenuCommand("copy");
    donor.close(SaveOptions.DONOTSAVECHANGES);

    app.activeDocument = targetDoc;
    targetDoc.activeLayer = targetLayer;
    app.executeMenuCommand("pasteFront");

    var sel = targetDoc.selection;
    if (!sel || sel.length === 0) return null;
    if (sel.length === 1) return sel[0];

    // Multiple pasted items: group them so callers get a single handle.
    var group = targetLayer.groupItems.add();
    for (var i = sel.length - 1; i >= 0; i--) {
        sel[i].move(group, ElementPlacement.PLACEATBEGINNING);
    }
    return group;
}
```
在蒙版组内寻找剪切路径

剪辑组将其剪辑形状公开为子`PathItem`（或更小）
通常，`CompoundPathItem`)与`clipping === true`的子节点。这个
剪辑的`geometricBounds`给出了可视框架的大小或居中内容
反对。```javascript
function findClipPath(group) {
    var items = group.pageItems;
    for (var i = 0; i < items.length; i++) {
        var it = items[i];
        try {
            if (it.typename === "PathItem" && it.clipping) return it;
            if (it.typename === "CompoundPathItem") {
                for (var j = 0; j < it.pathItems.length; j++) {
                    if (it.pathItems[j].clipping) return it;
                }
            }
        } catch (e) {}
    }
    return null;
}
```
### Cover-Fit和include - fit大小

要使图像完全覆盖矩形（任何被蒙版隐藏的溢出），使用width/height比值中较大的一个。要使其完全装入，请使用
越小。溢出因子（例如`1.10`）可以让封面图像稍微延伸
越过夹边。```javascript
function fitItemToRect(item, rect, mode, bleed) {
    // rect = [L, T, R, B] (Illustrator: T > B)
    var rw = rect[2] - rect[0];
    var rh = rect[1] - rect[3];
    var ib = item.geometricBounds;
    var iw = ib[2] - ib[0];
    var ih = ib[1] - ib[3];
    if (iw <= 0 || ih <= 0) return;

    var sx = rw / iw;
    var sy = rh / ih;
    var s  = (mode === "cover" ? Math.max(sx, sy) : Math.min(sx, sy))
           * (bleed || 1);
    item.resize(s * 100, s * 100);

    var cx = (rect[0] + rect[2]) / 2;
    var cy = (rect[1] + rect[3]) / 2;
    var b  = item.geometricBounds;
    var w  = b[2] - b[0];
    var h  = b[1] - b[3];
    item.position = [cx - w / 2, cy + h / 2];
}
```
批处理文件夹中的文件```javascript
var folder = Folder.selectDialog("Select folder of .ai files");
if (folder) {
    var files = folder.getFiles("*.ai");
    for (var i = 0; i < files.length; i++) {
        var doc = app.open(files[i]);
        // Process each document...
        doc.close(SaveOptions.DONOTSAVECHANGES);
    }
}
```
错误处理```javascript
try {
    var doc = app.activeDocument;
    var layer = doc.layers.getByName("NonExistentLayer");
} catch (e) {
    alert("Error: " + e.message);
    // e.message, e.line, e.fileName available
}
```
# #故障排除- **"undefined is not a object"**：通常表示集合为空或索引越界。访问项目前检查`.length`。
- **脚本运行，但没有改变视觉**：调用`app.redraw()`强制屏幕刷新修改后。
- **颜色模式不匹配**：文档颜色空间（RGB vs CMYK）必须匹配颜色对象。使用`doc.documentColorSpace`检查。
- **位置似乎不对**：记住脚本文档使用左下角原点，Y向上递增。`position`属性位于边界框的左上角。
- **文本不出现**：确保文本框架具有非零大小。对于点文本，设置`position`；对于区域文本，提供`areaText()`的有效路径。
- ** Windows上的文件路径**：在路径字符串中使用正斜杠（`/`）或双反斜杠（`\\`），或使用`File`对象构造函数。
—**批处理脚本中断对话框**：批量操作前设置`app.userInteractionLevel = UserInteractionLevel.DONTDISPLAYALERTS`。**集合使用`getByName()`**：许多集合对象支持`getByName("name")`，如果没有找到则抛出错误；用try/catch.包裹
- **“目标层不能修改”**：一个锁定的层，子层，或父组（通常是一个剪辑组，如`Cover_Mask`）正在阻止编辑。在修改之前，递归地清除文档中的`locked`和`hidden`。参见[递归解锁图层和组]（#recursive - Unlock Layers -and- Groups -before-editing）。
- **“无法设置放置项目的文件，文件路径是否有效？”**：文件存在且路径正确，但`PlacedItem.file`不接受该格式。SVG是最常见的原因—使用[打开/复制/粘贴模式]（# plac-svg -content- copyppaste -pattern）代替。
**`RasterItem.file = newFile`不做任何事情或抛出**:`RasterItem`不暴露可写的`file`属性。添加一个新的`PlacedItem`到相同的父类，恢复边界和名称，然后`.remove()`的r紫菀属植物。
- **`arguments[0]`是`[object BridgeTalk]`**（或空）：脚本通过ESTK`-run`或`BridgeTalk`消息启动；位置参数不会被转发。在已知的绝对路径下使用sidecar文件。参见[外部调用&参数传递]（# External - Invocation——参数传递）。
**`$.fileName`为空**：相同的外部启动器原因。不要在脚本中从`$.fileName`派生资源路径，因为这些路径可能会被任意调用——使用绝对路径或`Folder.temp`。
- **脚本似乎什么都不做**：几乎总是一个锁定的祖先，抑制对话框吞下错误，或编辑后缺少显式的`saveAs`。添加一个记录绝对路径的顶级`try`/`catch`，以确认执行并捕获错误。
**`item.resize(sx, sy)`意外地重新进入艺术品**:`resize`默认为围绕项目的中心缩放（`Transformation.CENTER`）。传递显式的`scaleAbout`参数，或者后跟`translate(dx, dy)`重新定位。脚本常量参考

跨API使用的常见枚举常量：

|类别|常量||---|---|
| **色彩空间** |`DocumentColorSpace.RGB`，`DocumentColorSpace.CMYK`|
| **对齐** |`Justification.LEFT`，`Justification.CENTER`,`Justification.RIGHT`,`Justification.FULLJUSTIFY`|
| **点类型** |`PointType.SMOOTH`，`PointType.CORNER`|
| **行程帽** |`StrokeCap.BUTTENDCAP`，`StrokeCap.ROUNDENDCAP`,`StrokeCap.PROJECTINGENDCAP`|
| **Stroke Join** |`StrokeJoin.MITERENDJOIN`,`StrokeJoin.ROUNDENDJOIN`,`StrokeJoin.BEVELENDJOIN`|
**混合模式** |`BlendModes.NORMAL`，`BlendModes.MULTIPLY`,`BlendModes.SCREEN`,`BlendModes.OVERLAY`|
| **保存选项** |`SaveOptions.SAVECHANGES`，`SaveOptions.DONOTSAVECHANGES`,`SaveOptions.PROMPTTOSAVECHANGES`|
| **导出类型** |`ExportType.PNG24`、`ExportType.PNG8`、`ExportType.JPEG`、`ExportType.SVG`、`ExportType.TIFF`、`ExportType.PHOTOSHOP`、`ExportType.AUTOCAD`、`ExportType.FLASH`|
|`ElementPlacement.PLACEATBEGINNING`,`ElementPlacement.PLACEATEND`,`ElementPlacement.PLACEBEFORE`,`ElementPlacement.PLACEAFTER`,`ElementPlacement.INSIDE`|
| **Z-Order** |`ZOrderMethod.BRINGTOFRONT`,`ZOrderMethod.SENDTOBACK`,`ZOrderMethod.BRINGFORWARD`,`ZOrderMethod.SENDBACKWARD`|
| **梯度类型** |`GradientType.LINEAR`，`GradientType.RADIAL`|
| **文本框架类型** |`TextType.POINTTEXT`，`TextType.AREATEXT`,`TextType.PATHTEXT`|
| **可变类型** |`VariableKind.TEXTUAL`，`VariableKind.IMAGE`,`VariableKind.VISIBILITY`,`VariableKind.GRAPH`|
| **用户交互** |`UserInteractionLevel.DISPLAYALERTS`，`UserInteractionLevel.DONTDISPLAYALERTS`|
| **兼容性** |`Compatibility.ILLUSTRATOR10`至`Compatibility.ILLUSTRATOR24`|

JavaScript对象引用（完整的API对象列表）Illustrator JavaScript API包含以下对象，按类别分组：

核心对象`Application`,`Document`,`Documents`,`DocumentPreset`,`Layer`,`Layers`,`PageItem`,`PageItems`,`View`,`Views`,`Preferences`路径和形状对象`PathItem`,`PathItems`,`PathPoint`,`PathPoints`,`CompoundPathItem`,`CompoundPathItems`,`GroupItem`,`GroupItems`文本对象`TextFrame`,`TextRange`,`TextRanges`,`TextPath`,`Characters`,`Words`,`Paragraphs`,`Lines`,`InsertionPoint`,`Story`,`Stories`,`CharacterAttributes`,`ParagraphAttributes`,`CharacterStyle`,`ParagraphStyle`,`ParagraphStyles`,`TextFont`,`TextFonts`,`TabStopInfo`颜色对象`RGBColor`,`CMYKColor`,`GrayColor`,`LabColor`,`NoColor`,`SpotColor`,`Spot`,`PatternColor`,`GradientColor`,`Gradient`,`Gradients`,`GradientStop`,`GradientStops`Swatch和Style对象`Swatch`,`Swatches`,`SwatchGroup`,`SwatchGroups`,`GraphicStyle`,`GraphicStyles`,`Patterns`,`Brush`,`Brushes`符号对象`Symbol``Symbols``SymbolItem``SymbolItems`画板对象`Artboard`,`Artboards`放置和光栅对象`PlacedItem`,`PlacedItems`,`RasterItem`,`RasterItems`,`MeshItem`,`MeshItems`,`GraphItem`,`GraphItems`,`PluginItem`,`NonNativeItem`,`NonNativeItems`,`LegacyTextItem`,`LegacyTextItems`数据驱动对象`Variable``Variables``Dataset``Datasets`矩阵和变换对象`Matrix`###标签对象`Tag`,`Tags`跟踪对象`TracingObject`,`TracingOptions`###保存和导出选项`IllustratorSaveOptions`,`EPSSaveOptions`,`PDFSaveOptions`,`FXGSaveOptions`,`ExportOptionsAutoCAD`,`ExportOptionsFlash`,`ExportOptionsJPEG`,`ExportOptionsPhotoshop`,`ExportOptionsPNG24`,`ExportOptionsSVG`,`ExportOptionsTIFF`###打开选项`OpenOptions`,`OpenOptionsAutoCAD`,`OpenOptionsFreeHand`,`OpenOptionsPhotoshop`,`PDFFileOptions`,`PhotoshopFileOptions`打印对象`PrintOptions`,`PrintJobOptions`,`PrintPaperOptions`,`PrintColorManagementOptions`,`PrintColorSeparationOptions`,`PrintCoordinateOptions`,`PrintPostScriptOptions`,`PrintFontOptions`,`Paper`,`PaperInfo`,`PPDFile`,`InkInfo`,`Screen`,`ScreenInfo`,`ScreenSpotFunction`图像和栅格化选项`ImageCaptureOptions`,`RasterEffectOptions`,`RasterizeOptions`# #引用

-最近的脚本API更改（CC 2020增加了`Document.getPageItemFromUuid`和`PageItem.uuid`； CC 2017增加了`Application.getIsFileOpen`）
- [Illustrator脚本指南](https://ai-scripting.docsforadobe.dev/) -完整的社区维护文档