# Visual PR插件

当你改变了某些东西的外观——布局、图表、表单——PR描述应该显示出这些变化。而不是用语言来描述。表现出来。before/after屏幕截图会准确地告诉您的审阅者发生了什么，并且他们不需要检出分支来查看它。

这个插件教Copilot捕捉您的web应用程序（或任何UI）的屏幕截图，用标注标注它们，并将它们嵌入到PR描述中。一旦你习惯了每次视觉变化都有最新的截图，回到纯文本pr就像闭着眼睛审查代码一样。

##演示🎬

现在是2009年。您刚刚添加了在问题上显示标签的功能。你最好包括截图，因为这是巨大的。

下面是这样的PR描述看起来像我们的插件：> **之前** -问题列表不带标签：
>
> <img src="demo/before.png" width="600" alt=" issue list without labels">
>
> ** ** -标签直接显示在每个问题行：
>
> <img src="demo/after.png" width="600" alt=" issue list with labels annotated">

---

##这并不只是针对pr

相同的注释引擎适用于任何屏幕截图。这里有一个提示：

> *“去github/awesome-copilot.的GitHub问题页面截图问题列表和注释：任何问题图标，准备审查标签，回购名称，固定问题，我的头像，最多评论的问题，新问题按钮。

！[多用途注释-一个提示6个标注]（demo/multi.png）

代理捕获页面，识别7个请求元素中的6个，并对它们全部进行注释。它正确地报告了第7个（用户头像）不可见，因为页面是在没有身份验证的情况下捕获的——没有幻觉注释。

调试模式运行的每个注释都可以生成一个调试热图，显示算法如何选择标签位置-对比评分，禁区和候选排名：

！[调试显示放置算法的热图]（demo/debug.png）

---

包含的技能

b|技能|它做什么||-------|-------------|
| [UI截图](../../skills/ui-screenshots/SKILL.md) |捕获web UI截图与剧作家+ PIL作物工作流|
| [image-annotations](../../skills/image-annotations/SKILL.md) |用标注矩形，箭头，标签和彩色编码高光注释任何图像|
| [PR截图](../../skills/pr-screenshots/SKILL.md) |在PR描述中嵌入before/after图像（GitHub + Azure DevOps） |
| [screen-recording](../../skills/screen-recording/SKILL.md) |创建带注释的动画GIF演示与可变的定时|

##用例

-可视化pr -在不签出分支的情况下，向审阅者显示更改的内容
- **发布说明** -嵌入GIF演示的新功能
-before/after截图证明修复
- **文档** -带标注的屏幕截图，突出显示关键区域

# #要求这个插件需要一个可以查看图像的模型——工作流程依赖于查看截图来查找裁剪坐标和验证注释。本自述文件中的演示图像是用Claude Opus 4.6**生成的。