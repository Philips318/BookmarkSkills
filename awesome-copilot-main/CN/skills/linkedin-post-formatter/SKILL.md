---
name: linkedin-post-formatter
description: 'Format and draft compelling LinkedIn posts using Unicode bold/italic styling, visual separators, structured sections, and engagement-optimized patterns. USE FOR: draft LinkedIn post, format text for LinkedIn, create social media post, write thought leadership post, convert content to LinkedIn format, LinkedIn carousel text, Unicode bold italic formatting.'
---
# LinkedIn Post Formatter

使用Unicode排版和经过验证的结构模式，将原始内容、想法或技术材料转换为精美的、引人入胜的LinkedIn帖子。

# #概述

LinkedIn只支持纯文本-没有Markdown渲染，没有丰富的格式。该技能使用Unicode数学字母数字符号来模拟在LinkedIn编辑器中原生呈现的粗体、斜体和粗体-斜体文本，而无需任何外部工具。

Unicode排版参考

在将纯文本转换为unicode风格的LinkedIn文本时，首先加载并使用`references/unicode-charmap.md`作为权威字符映射引用。

应用这些字符映射在纯文本中创建视觉强调：

###粗体（数学Sans-Serif粗体）

关键短语、章节标题和强调词使用粗体。

| Plain | Unicode Bold ||-------|-------------|
| a-z |𝗔-𝗭|
| a-z |𝗮-𝘇|
| -9 | <e:1> -𝟵|

###斜体（数学无衬线斜体）

用斜体字来表示微妙的强调、专业术语或引用。

|普通| Unicode斜体||-------|---------------|
| a-z |𝘈-𝘡|
| a-z | <e:1> -𝘻|

###粗体斜体（数学无衬线粗体斜体）

尽量少用，以达到最大的强调效果。

|普通| Unicode粗体-斜体||-------|-------------------|
| a-z |𝘼-𝙕|
| a-z |𝙖-𝙯|

##视觉分隔符

使用这些字符来创建视觉结构：

- **分段器**:`━━━━━━━━━━━━━━━━━━━━━━`（拉框重水平）
- **项目符号**:`◈`（菱形带点）或`◎`（靶心）
- **箭头流**:`↓`为垂直流，`→`为水平延续
- **子点**:`↳`缩进子项
- **编号项目**：使用粗体Unicode数字`𝟭. 𝟮. 𝟯.`等。

## Post Structure Patterns

模式1：钩子→内容→CTA（通用）```
[Bold hook line — provocative statement or question]

[1-2 lines of context setting the stage]

━━━━━━━━━━━━━━━━━━━━━━

[Main content with bold section headers]
[Bullet points using ◈ or numbered with bold digits]

━━━━━━━━━━━━━━━━━━━━━━

[Bold takeaway or summary]

[Call to action — repost, comment, or grab resource]

#Hashtags
```
模式2：列表（编号见解）```
[Bold opening line with a strong claim]

[Setup line explaining what follows]

𝟭. [Bold item title]
   [Supporting detail]

𝟮. [Bold item title]
   [Supporting detail]

...

𝗧𝗵𝗲 𝗸𝗲𝘆 𝘁𝗮𝗸𝗲𝗮𝘄𝗮𝘆: [Summary in italic]

#Hashtags
```
模式3：故事→教训（思想领导力）```
[Italic opening with a personal or observed moment]

[2-3 short paragraphs telling the story]

━━━━━━━━━━━━━━━━━━━━━━

𝗧𝗵𝗲 𝗹𝗲𝘀𝘀𝗼𝗻:

[Bold lesson or principle extracted from the story]

[CTA]

#Hashtags
```
模式4：资源共享（Cheatsheet/Guide/Tool）```
[Hook: "If you do X, you cannot miss this..."]

[Brief description of what the resource covers]

━━━━━━━━━━━━━━━━━━━━━━

[Bold section count]. [Bold section titles as numbered list]

━━━━━━━━━━━━━━━━━━━━━━

𝗧𝗵𝗲 𝗿𝗲𝗮𝗹 𝘁𝗮𝗸𝗲𝗮𝘄𝗮𝘆:

[Why this resource matters — bold key phrase]

[Grab it / Share it CTA]

♻️ 𝗥𝗲𝗽𝗼𝘀𝘁 if this is useful to your network.

#Hashtags
```
##格式化规则1. **换行很重要**:LinkedIn可以折叠多个空白行。段落之间使用单空行。
2. **吸引眼球**：前2-3行必须让读者点击“查看更多”。实施价值。
3. **短段落**：每段最多1-3句话。文本墙会破坏用户粘性。
4. **少量加粗**：加粗关键短语和标题，而不是整个段落。
5. **用斜体字表示细微差别**：用斜体字表示专业术语、内心想法或微妙的强调。
6. **结尾标签**：最后一行5-8个相关标签。没有中间标签。
7. **除非用户明确要求，否则正文中没有表情符号**。例外：CTA中的一个战略性表情符号（♻️用于转发）。
8. **字符限制**:LinkedIn帖子最多可达3000个字符。以1500-2500为目标以获得最佳接触。
9. **正文中没有url **: LinkedIn抑制了带有链接的帖子的访问。在评论中添加链接代替。提到“评论中的链接”或“在下面下载”作为CTA。用户粘性优化

- **有效的开场白：问题，大胆的声明，“如果你做X…”，相反的选择，令人惊讶的统计数据。
- **关闭cta工作**：“♻️𝗥𝗲𝗽𝗼𝘀𝘁如果…”，“保存这个以后”，“标记需要这个的人”，“你的看法是什么？”👇”
- **空格是你的朋友**：密集的文本被滚动过去。通风，可扫描的布局获胜。
- **“看到更多”挂钩**:LinkedIn在桌面上截断约210个字符后的帖子。确保前两行足够吸引人去点击。

# #过程1. 分析源内容（文本、HTML、图像或想法）。
2. 确定最佳的帖子结构模式（链接→内容→CTA、列表、故事→课程、资源分享）。
3. 提炼出核心信息和3-5个要点。
4. 使用`references/unicode-charmap.md`对标头和强调字应用Unicodebold/italic格式。
5. 在各部分之间添加可视分隔符。
6. 写一个引人注目的开场白。
7. 最后添加CTA和标签。
8. 验证帖子是复制粘贴准备领英。