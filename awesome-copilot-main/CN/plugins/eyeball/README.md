一个帮助验证AI语句的工具，没有（或至少更少）上下文切换的痛苦。

当人工智能分析一份文件并告诉你“第10条需要相互赔偿”时，你怎么知道第10条实际上是这么说的？眼球让你自己看。

这是一个Copilot CLI插件，可以生成Word文件形式的文档分析，其中包含源材料中相关部分的内联截图。分析中的每个事实断言都包括一个突出显示的原始文档摘录，因此您可以验证每个断言，而无需在文件之间切换或寻找正确的页面。

它的作用你给Copilot一个文档（Word文件、PDF文件或web URL），让它分析一些具体的东西。“眼球”会读取源代码，编写分析，并对每一项声明截取原始文档中相关部分的屏幕截图，其中引用的文本以黄色突出显示。输出是一个Word文档在您的桌面上，分析文本和源屏幕截图交错。

如果分析显示“第9.3条允许在30天的修复期内终止协议”，则下面的屏幕截图显示了实际文档中的第9.3条，并突出显示了该语言。如果截图显示了一些不同的东西，那么分析是错误的，您可以立即看到它。

# #安装

# # #先决条件—[Copilot CLI]（https://docs.github.com/copilot/concepts/agents/about-copilot-cli）已安装并认证
- Python 3.8或更高版本
- Word文档支持（pdf和web url没有这些）：
- Microsoft Word （macOS或Windows）
- LibreOffice（任何平台）

安装插件

通过Copilot CLI插件系统安装。在Copilot CLI对话中：```
install the eyeball plugin from github/awesome-copilot
```
安装依赖项

安装完插件后，安装Python依赖项：```bash
pip install pymupdf pillow python-docx playwright
python -m playwright install chromium
```
在Windows上，也安装pywin32用于Word自动化：```bash
pip install pywin32
```
验证安装```bash
python3 skills/eyeball/tools/eyeball.py setup-check
```
这将显示您的机器上支持哪些源类型。

##如何使用

在Copilot的CLI对话中，告诉它使用眼球和你想要分析的内容：```
use eyeball on ~/Desktop/vendor-agreement.docx -- analyze the indemnification
and liability provisions and flag anything unusual
```

```
run eyeball on https://example.com/terms-of-service -- identify the
developer-friendly aspects of these terms
```

```
use eyeball to analyze this NDA for non-compete provisions
```
眼球激活，读取源文档，写入分析与精确的部分引用，并在您的桌面上生成一个Word文档与源屏幕截图内联。

它支持什么

|源类型|要求||---|---|
| Python + PyMuPDF（包含在安装中）|
b|网页| Python +剧作家+铬（包括在设置）|
| Word文档（.docx） | Microsoft Word （macOS/Windows）或LibreOffice（任何平台）在Windows上，pywin32也是必需的（包含在设置中）。|

##它是如何工作的

1. “眼球”读取源文档的全文
2. 它使用精确的章节编号、页面引用和逐字引用来编写分析
3. 对于每个声明，它搜索呈现的源以查找引用的文本
4. 它捕获了周围区域的屏幕截图，引用的文本以黄色突出显示
5. 它将一个Word文档与分析段落和屏幕截图交织在一起
6. 输出结果显示在您的桌面上屏幕截图的大小是动态的：如果一段分析引用的文本跨越了一个很大的区域，屏幕截图就会展开以覆盖它。如果引用的文本出现在多个页面上，则屏幕截图将被拼接在一起。

##为什么要截图而不是引用文本？

在对幻觉敏感的情况下，有时我们需要看到收据。

引用的文本很容易捏造。模型可以生成一个听起来似乎合理的引用，但实际上并没有出现在源中，如果不进行检查，您永远不会知道。来自渲染源的截图更难以伪造；它们显示了原始文档的实际格式、布局和周围上下文。您可以一眼看到高亮显示的文本是否与声明相匹配，并且周围的文本提供了精心挑选的引用可能忽略的上下文。

# #的局限性—Word文档转换需要Microsoft Word或LibreOffice。没有这些功能，你仍然可以使用眼球来处理pdf和网页url。
—文本搜索是字符串匹配。如果源文档使用不寻常的编码、连接符或非标准字符，则某些搜索可能不匹配。技能指示告诉AI从提取的文本中逐字逐句地使用短语，这可以处理大多数情况。
-网页渲染依赖于剧作家，可能无法完美捕获所有动态内容（例如，页面加载后由JavaScript加载的内容，登录墙后的内容）。
—截图质量与源格式有关。密集的多栏布局或非常小的文本可能会产生可读性较差的屏幕截图。如果需要，增加DPI设置。

# #许可证

麻省理工学院