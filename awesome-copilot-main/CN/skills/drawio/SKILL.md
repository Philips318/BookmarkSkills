---
name: drawio
description: Generate draw.io diagrams as .drawio files and export to PNG/SVG/PDF with embedded XML
---
#画。io图技能

产生吸引。io图表作为原生`.drawio`文件，并将其导出为可以嵌入Word文档中的PNG图像。

如何创建一个图表

1. * *产生吸引。`mxGraphModel`格式的XML**
2. **使用create/edit文件工具将XML**写入`.drawio`文件
3. **导出为PNG**使用捆绑的导出脚本

##绑定导出脚本

该技能包括`drawio-to-png.mjs`，Node.js导出脚本与两个渲染后端：

1. * *。io CLI**（像素完美，最快）-自动使用，如果绘制。IO桌面安装完成
2. * *官方画。io查看器在无头浏览器**（像素完美，需要Chromium/Edge） -回退时CLI不可用

# # #使用```bash
# Install dependencies (one-time, from the scripts folder)
cd skills/drawio/scripts && npm install

# Export a single diagram
node skills/drawio/scripts/drawio-to-png.mjs <input.drawio> [output.png]

# Export all .drawio files in a directory
node skills/drawio/scripts/drawio-to-png.mjs --dir <directory>

# Force a specific renderer
node skills/drawio/scripts/drawio-to-png.mjs --renderer=cli|viewer|auto <input.drawio>
```
技能文件夹内容

|文件|用途||------|---------|
|`SKILL.md`|该指令文件|
|`scripts/drawio-to-png.mjs`|Node.js导出脚本(CLI +浏览器回退
|`scripts/package.json`|依赖项（`puppeteer-core`） |

支持的导出格式

|格式|嵌入XML | Notes ||--------|-----------|-------|
|`png`|是|随处可见，可在draw中编辑。io |
|`svg`|是|可缩放，可在绘图中编辑。io |
|`pdf`|是|可打印，可在绘图中编辑。io |

# #。io XML样式约定

使用这些风格来制作一致的专业图表：```xml
<!-- Primary service (highlighted) -->
<mxCell style="rounded=1;whiteSpace=wrap;html=1;fillColor=#dae8fc;strokeColor=#6c8ebf;strokeWidth=2;arcSize=12;shadow=1;" />

<!-- External system -->
<mxCell style="rounded=1;whiteSpace=wrap;html=1;fillColor=#f5f5f5;strokeColor=#666666;" />

<!-- Success/processing stage -->
<mxCell style="rounded=1;whiteSpace=wrap;html=1;fillColor=#d5e8d4;strokeColor=#82b366;" />

<!-- Warning/quality gate -->
<mxCell style="rounded=1;whiteSpace=wrap;html=1;fillColor=#fff2cc;strokeColor=#d6b656;" />

<!-- Error/failure path -->
<mxCell style="rounded=1;whiteSpace=wrap;html=1;fillColor=#f8cecc;strokeColor=#b85450;" />

<!-- Data store (cylinder) -->
<mxCell style="shape=cylinder3;whiteSpace=wrap;html=1;fillColor=#fff2cc;strokeColor=#d6b656;" />

<!-- Arrow -->
<mxCell style="edgeStyle=orthogonalEdgeStyle;rounded=1;strokeColor=#6c8ebf;strokeWidth=2;" />
```
##定位图纸。io CLI

先尝试`drawio`（如果在PATH上有效），然后返回：

—**Windows**:`"C:\Program Files\draw.io\draw.io.exe"`—**macOS**:`/Applications/draw.io.app/Contents/MacOS/draw.io`**Linux**:`drawio`（通过snap/apt/flatpak）

### CLI导出命令```bash
drawio -x -f png -e -b 10 -o <output.png> <input.drawio>
```
标志：`-x`（导出），`-f`（格式），`-e`（嵌入图XML），`-b`（边界），`-o`（输出路径）。