# Noob模式插件

为非技术Copilot CLI用户提供的纯英文翻译层。激活后，Copilot会自动将每个许可请求、错误信息和技术输出翻译成清晰、无专业术语的语言，并带有彩色编码的风险指标。

这是给谁的？

任何使用Copilot CLI但不是软件开发人员的人：
-律师和法律专业人员
-产品经理和项目经理
-业务利益相关者和高管
-技术作家和内容创作者
-使用代码邻近工具的设计师
-任何不熟悉命令行的人

# #安装```bash
copilot plugin install noob-mode@awesome-copilot
```
包含的内容

###命令（斜杠命令）

|命令|描述||---------|-------------|
|`/noob-mode:noob-mode`|激活当前会话的Noob模式。副驾驶将用简单的英语解释一切——每一个动作，每一个许可请求，每一个结果。|

捆绑资产

|资产|描述||-------|-------------|
|`references/glossary.md`| 100多个用简单英语定义的技术术语，按类别（Git，文件系统，开发，Web, Copilot CLI）组织
15个示例，展示Noob模式如何将技术输出转化为清晰的解释|

# #特性

|功能|这对你意味着什么||---|---|
每次副驾驶请求许可时，它都会解释它想做什么，为什么，风险有多大，以及如果你同意或不同意会发生什么|
| **风险指标** |颜色编码的风险级别（🟢低，🟡中，🔴高，⛔严重），因此您可以立即看到一个行动是否安全|
| **术语检测** |专业术语在第一次出现时自动以简单的英语定义|
一步一步的计划一步一步的任务从一个简单的英文路线图开始，这样你就知道接下来要做什么了
| **输出翻译** |错误消息和命令结果被翻译成“here's what that means”|
完成摘要在每个任务之后，您会得到一个关于更改内容，创建内容以及如何撤消这些内容的摘要|
决策支持当您需要在选项之间做出选择时，每个选项都有权衡和建议来解释

# #的例子**无新手模式：**```
Allow tool: bash with command "grep -r 'indemnification' ./contracts/"?
[y/n]
```
**与新手模式：**```
📋 WHAT I'M ASKING TO DO:
I want to search all files in your "contracts" folder for the word "indemnification."

🎯 WHY:
You asked me to find every mention of indemnification across your contracts.

⚠️ RISK: 🔴 High (but safe in this case)
Running commands is generally high-risk, but this one only searches — it doesn't
change or delete anything.

✅ If you approve: I'll show you every file where "indemnification" appears.
❌ If you decline: I can read files one by one instead, but it'll take longer.
```
##如何关闭

在对话中说“关闭新手模式”，副驾驶就会恢复到默认的沟通方式。

# #源

这个插件是[Awesome Copilot]（https://github.com/github/awesome-copilot）的一部分，这是一个社区驱动的GitHub Copilot扩展集合。

# #许可证

麻省理工学院