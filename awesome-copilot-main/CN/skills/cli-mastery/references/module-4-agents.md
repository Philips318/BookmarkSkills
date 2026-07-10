模块4：代理系统

内置代理

|代理|模型|最适合|关键性状||-------|-------|----------|-----------|
|`explore`|俳句|快速代码库问答|只读，<300字，安全并行|
|`task`|俳句|运行命令（测试、构建、测试）|成功时简短，失败时详细|
|`general-purpose`|十四行诗|复杂的多步骤任务|完整的工具集，独立的上下文窗口|
|`code-review`|十四行诗|分析代码更改|从不修改代码，高信噪比|

自定义代理-在Markdown中定义自己的代理

|级别|位置|范围||-------|----------|-------|
|个人|`~/.copilot/agents/*.md`|你所有的项目|
|项目|`.github/agents/*.md`|每个人都在这个回购|
|组织|`.github-private/agents/`在org回购|整个org |

代理文件解剖```markdown
---
name: my-agent
description: What this agent does
tools:
  - bash
  - edit
  - view
---

# Agent Instructions
Your detailed behavior instructions here.
```
代理编排模式

1. **扇形探索** -并行启动多个`explore`代理，同时回答不同的问题
2. **管道** -`explore`→理解→`general-purpose`→实现→`code-review`→验证
3. **专家交接** -确定任务→`/agent`挑选专家→与`/fleet`或`/tasks`进行审查

关键洞察：AI在适当的时候自动委托给子代理。