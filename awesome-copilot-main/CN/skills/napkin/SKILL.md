---
name: napkin
description: 'Visual whiteboard collaboration for Copilot CLI. Creates an interactive whiteboard that opens in your browser — draw, sketch, add sticky notes, then share everything back with Copilot. Copilot sees your drawings and text, and responds with analysis, suggestions, and ideas.'
---
#餐巾纸-辅助驾驶CLI的可视化白板

Napkin为用户提供了一个基于浏览器的白板，他们可以在上面画画、素描和添加便利贴，以直观地思考想法。代理读取白板内容（通过PNG快照和可选的JSON数据），并以对话方式响应分析、建议和下一步。

目标受众是律师、项目经理和业务干系人——而不是软件开发人员。让每件事都平易近人，没有行话。

---

# #激活

当用户调用这个技能时，比如说“让我们用餐巾纸”、“打开餐巾纸”、“开始写白板”或使用斜杠命令，请执行以下操作：1. **将绑定的HTML模板**从技能资产复制到用户的桌面。
—相对于这个SKILL.md文件，模板位于`assets/napkin.html`。
—拷贝到“`~/Desktop/napkin.html`”。
—如果`~/Desktop/napkin.html`已经存在，则在覆盖之前询问用户是要打开现有的还是重新开始。

2. **在默认浏览器中打开：**
—macOS:`open ~/Desktop/napkin.html`—Linux:`xdg-open ~/Desktop/napkin.html`—Windows:`start ~/Desktop/napkin.html`3. **告诉用户下一步该做什么。**说一些温暖而简单的话：   ```
   Your napkin is open in your browser!

   Draw, sketch, or add sticky notes — whatever helps you think through your idea.

   When you're ready for my input, click the green "Share with Copilot" button on the whiteboard, then come back here and say "check the napkin."
   ```
---

##阅读餐巾

当用户说“检查餐巾纸”、“看看餐巾纸”、“你觉得怎么样”、“读一下我的餐巾纸”或类似的话时，请遵循以下步骤：

###第一步-读取PNG快照（主）

查找名为`napkin-snapshot.png`的PNG文件。按顺序检查这些位置（浏览器将其保存到用户的默认下载文件夹中，各不相同）：

1.`~/Downloads/napkin-snapshot.png`2.`~/Desktop/napkin-snapshot.png`使用`view`工具读取PNG文件。这将图像作为base64编码的数据发送到模型，模型可以直观地解释它。PNG是代理理解用户绘制内容的主要方式——它捕获手绘草图、箭头、空间布局、注释、圈出或划掉的项目，以及画布上的任何其他内容。

如果在任何位置都找不到PNG，不要静默地跳过它。相反，告诉用户：```
I don't see a snapshot from your napkin yet. Here's what to do:

1. Go to your whiteboard in the browser
2. Click the green "Share with Copilot" button
3. Come back here and say "check the napkin" again

The button saves a screenshot that I can look at.
```
###步骤2 -读取剪贴板中的结构化JSON（补充）

还要尝试从系统剪贴板抓取结构化JSON数据。白板会自动将其复制到PNG文件旁边。

—macOS:`pbpaste`—Linux:`xclip -selection clipboard -o`—Windows:`powershell -command "Get-Clipboard"`JSON包含便利贴和文本标签的确切文本内容、它们的位置和颜色。这通过提供可能难以从截图中读取的精确文本来补充PNG。

如果剪贴板不包含JSON数据，那也没关系—仅PNG就可以为模型提供足够的工作空间。不要将丢失的剪贴板视为错误。

###步骤3 -一起解释两个来源

将视觉快照和结构化文本综合起来，形成对用户想法或计划的连贯理解：**描述你所看到的-草图，图表，流程图，分组，箭头，空间布局，注释，圈项目，划掉项目，强调标记。
- **来自JSON:**读取便利贴和标签的确切文本内容，注意它们的位置和颜色。
- **结合**到一个单一的，会话的解释。

###第四步-对话式回应

不要转储原始数据或技术摘要。以合作者的身份看待别人的白板草图。例子:-“我可以看出你已经勾勒出了一个三阶段的过程——看起来你在考虑让[X]流入[Y]，然后再流入[Z]。角落里的便利贴上写着‘文字’——你想让我解决这个问题吗？”
-“看起来你把这四个想法放在左边，把它们和右边的两个分开了。你认为这是两个不同的类别吗？”
-“我看到你画了箭头连接[A]到[B]到[C] -这是你设想的工作流程吗？”

###第五步-问下一步是什么

总是以提出下一步计划作为结尾：

-“想让我在此基础上继续发展吗？”
“我应该把它变成一个结构化的文档吗？”
-“要我把建议写在餐巾纸上吗？”

---

在餐巾纸上回应

当用户希望代理将内容添加回白板时：-代理**不能**直接修改HTML文件的画布状态-这是由运行在浏览器中的JavaScript管理。
-相反，提供切实可行的替代方案：
-在CLI中提供响应，并建议用户手动添加到餐巾纸上。
根据从餐巾纸上理解的内容，主动提出创建一个单独的文档（价目表、备忘录、清单等）。
-如果有意义，创建一个带有预加载内容的`napkin.html`的更新副本。

---

语气和风格

-使用与新手模式技能相同的平易近人，非技术语气。
-不要使用开发者术语而不用简单的英语解释。
把餐巾纸当成一个创造性的协作空间，而不是一个正式的输入机制。
-无论美术质量如何，都要鼓励用户的草图。
-将回应定义为“基于你的思考”，而不是“分析你的输入”。

---

##错误处理**未找到PNG快照：**```
I don't see a snapshot from your napkin yet. Here's what to do:

1. Go to your whiteboard in the browser
2. Click the green "Share with Copilot" button
3. Come back here and say "check the napkin" again

The button saves a screenshot that I can look at.
```
**桌面不存在白板文件：**```
It looks like we haven't started a napkin yet. Want me to open one for you?
```
---

##重要事项

- PNG解释是**主**通道。多模态模型可以读取和解释`view`工具返回的base64图像数据。
- JSON剪贴板数据是**补充** -它提供精确的文本，但不捕获手绘。
-总是先检查PNG。如果没有找到，提示用户点击“与副驾驶共享”。
-如果剪贴板没有JSON数据，则单独使用PNG。
—相对于SKILL.md文件，HTML模板位于`assets/napkin.html`。
—如果noob模式技能也处于激活状态，则在请求文件或bash权限时使用其风险指示格式（green/yellow/red）。