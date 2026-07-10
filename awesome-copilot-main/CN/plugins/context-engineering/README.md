#上下文工程插件

通过更好的上下文管理最大化GitHub Copilot有效性的工具和技术。包括构建代码的指导原则、规划多文件更改的代理以及上下文感知开发的提示。

# #安装```bash
# Using Copilot CLI
copilot plugin install context-engineering@awesome-copilot
```
包含的内容

###命令（斜杠命令）

|命令|描述||---------|-------------|
|`/context-engineering:context-map`|在进行更改之前生成与任务相关的所有文件的映射|
问副驾驶在回答问题之前需要看哪些文件
|`/context-engineering:refactor-plan`|计划一个具有适当排序和回滚步骤的多文件重构|

# # #代理

|代理|描述||-------|-------------|
|`context-architect`|通过识别相关上下文和依赖关系来帮助计划和执行多文件更改的代理|

# #源

这个插件是[Awesome Copilot]（https://github.com/github/awesome-copilot）的一部分，这是一个社区驱动的GitHub Copilot扩展集合。

# #许可证

麻省理工学院