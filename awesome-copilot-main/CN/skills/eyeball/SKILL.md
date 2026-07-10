---
name: eyeball
description: 'Document analysis with inline source screenshots. When you ask Copilot to analyze a document, Eyeball generates a Word doc where every factual claim includes a highlighted screenshot from the source material so you can verify it with your own eyes.'
---
#眼球

用目视证据分析文件。激活后，“眼球”会在用户的桌面上生成一个Word文档，其中每个事实断言都包含一个内联截图，来自源材料，引用文本以黄色突出显示。

# #激活

当用户调用此技能时（例如，“使用眼球”，“在此运行眼球”，“查看此文档”），回应如下：

b> **眼球活跃。**我将分析文档并生成一个带有内联源代码截图的Word文档，这样您就可以亲眼验证每个声明。

然后按照下面的工作流程操作。

##支持的资源

—**本地文件：** Word文档（.docx、。doc）、pdf文件（.pdf）、RTF文件
- **Web url:**任何可公开访问的网页

##工具位置

眼球Python实用程序位于：```
<plugin_dir>/skills/eyeball/tools/eyeball.py
```
要查找实际路径，请运行：```bash
find ~/.copilot/installed-plugins -name "eyeball.py" -path "*/eyeball/*" 2>/dev/null
```
如果没有找到，请检查项目目录或用户的主目录以获取眼球repo。

##第一次运行设置

在第一次使用之前，检查是否安装了依赖项：```bash
python3 <path-to>/eyeball.py setup-check
```
如果缺少任何东西，安装所需的依赖项：```bash
pip3 install pymupdf pillow python-docx playwright
python3 -m playwright install chromium
```
在Windows上，也安装pywin32用于Word自动化：```bash
pip install pywin32
```
# #工作流程

严格按照以下步骤操作。顺序很重要。

第一步：阅读原文

在写任何分析之前，提取并阅读源文件的全文：```bash
python3 <path-to>/eyeball.py extract-text --source "<path-or-url>"
```
仔细阅读输出。确定实际的章节编号、标题、页码和关键语言。

**关键：**不要跳过此步骤。不要基于对文档结构的假设来写分析。阅读原文。

###步骤2：用精确的引用来写分析

对于你分析中的每一点，你必须：

1. **引用文档中出现的正确章节编号（例如，“章节9”而不是“章节8”，因为你假设了编号）。
2. **引用正确的页码**在摘录的文本中出现的部分。
3. **选择从原文中逐字逐句直接支持你观点的锚点。

###步骤3：正确选择锚

这是最重要的一步。锚决定在屏幕截图中突出显示什么。* *: * *
-从原文中逐字逐句地使用短语来直接支持你的主张
-使用多个锚点来跨越读者应该看到的全部文本
-使用特定的，不常见的短语，只出现在你想要的地方

* *不:* *
-在整个文档中使用通用主题标签（例如，“机密性”）
-当章节标题在别处作为交叉引用出现时，只使用章节标题
-使用在许多地方都匹配的单一常用词

* *例子:* *

错误——使用一个通用的主题标签来匹配任何地方：```json
{"anchors": ["User-Generated Content"], "target_page": 8}
```
RIGHT——使用支持该主张的特定语言：```json
{"anchors": ["retain ownership", "Ownership of Content, Right to Post"], "target_page": 8}
```
错误-章节标题在前面的页面上显示为交叉引用：```json
{"anchors": ["LIMITATION OF LIABILITY"]}
```
RIGHT——包括精确的节号，指向正确的页面；```json
{"anchors": ["12. LIMITATION OF LIABILITY", "INDIRECT", "CONSEQUENTIAL"], "target_page": 13}
```
步骤4：构建分析文档

构造一个JSON数组并调用build命令：```bash
python3 <path-to>/eyeball.py build \
  --source "<path-or-url>" \
  --output ~/Desktop/<title>.docx \
  --title "Analysis Title" \
  --subtitle "Source description" \
  --sections '[
    {
      "heading": "1. Section Title",
      "analysis": "Your analysis text here. Reference Section X on page Y...",
      "anchors": ["verbatim phrase 1", "verbatim phrase 2"],
      "target_page": 5,
      "context_padding": 40
    },
    {
      "heading": "2. Another Section",
      "analysis": "More analysis...",
      "anchors": ["exact quote from source"],
      "target_pages": [10, 11],
      "context_padding": 50
    }
  ]'
```
Section对象字段：
—`heading`（必选）：输出文档中的节标题
-`analysis`（必选）：您的分析文本
-`anchors`（必选）：要搜索和突出显示的源中的逐字短语列表
-`target_page`（可选）：要搜索的单个页码（1-索引）
-`target_pages`（可选）：要搜索的页码列表（屏幕截图垂直拼接）
-`context_padding`（可选）：PDF中的填充点above/below锚区域（默认值：40）。增加更多的上下文。

###步骤5：交付输出

将输出保存到用户的Desktop。告诉用户文件名，他们可以打开它，根据突出显示的源屏幕截图验证每个声明。

##发货前自检

在保存最终文档之前，请在心里验证：1. 每个部分的分析文本是否从源引用了正确的部分编号？
2. 锚是否与目标页面上出现的短语一字不差？
3. 每个锚点是否直接支持分析中的观点，而不是仅仅与同一个主题相关？
4. 如果截图和分析不匹配，是分析错了还是锚错了？修正不正确的地方。

# #笔记—输出文档包括动态大小的高亮屏幕截图。如果您提供多个锚，则屏幕截图会展开以覆盖所有锚。
-当没有找到搜索词时，输出文档会注意到这一点。如果发生这种情况，锚可能不是逐字逐句。调整和重建。
-对于网页，剧作家会先将页面呈现为PDF格式。生成的页码可能与您在浏览器中看到的不同。使用提取的文本输出（步骤1）来确定正确的页码。
—如果用户已经提供了源文本，或者您已经在当前对话中阅读了源文本，则可以跳过步骤1。但是，在写分析之前，一定要根据实际文本验证章节编号和页面引用。