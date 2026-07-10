# RUG代理工作流插件

用于编排软件交付的三代理工作流，其中包含一个编排器以及实现和QA子代理。

# #安装```bash
# Using Copilot CLI
copilot plugin install rug-agentic-workflow@awesome-copilot
```
包含的内容

# # #代理

|代理|描述||-------|-------------|
|`rug-orchestrator`|纯编排代理，分解请求，将所有工作委托给子代理，验证结果，并重复直到完成。|
|`swe-subagent`|高级软件工程师子代理，负责实现任务：特性开发、调试、重构和测试。|
|`qa-subagent`|细致的QA子代理，用于测试计划、bug搜索、边缘案例分析和实现验证。|

# #源

这个插件是[Awesome Copilot]（https://github.com/github/awesome-copilot）的一部分，这是一个社区驱动的GitHub Copilot扩展集合。

# #许可证

麻省理工学院