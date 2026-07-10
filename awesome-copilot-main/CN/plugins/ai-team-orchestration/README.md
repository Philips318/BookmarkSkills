# AI团队编排

引导并运行一个多代理AI开发团队，并指定角色（制作人，开发团队，QA）。计划冲刺，用不同的代理声音运行头脑风暴，协调并行的dev/QA工作流，并使用结构化的移交模板在上下文溢出中存活下来。

包含的内容

# # #代理

|代理|提及|角色|工具访问||-------|---------|------|-------------|
| **制作人**（人头马）|`@ai-team-producer`| Sprint规划、协调、PR合并|只读（无代码编辑）|
| **开发团队** (Nova, Sage, Milo) |`@ai-team-dev`|前端，后端，可视化实现|全编码工具|
| **QA** (Ivy) |`@ai-team-qa`|测试，bug归档，签署|阅读+测试（不编辑源代码）|

# # #技能`/ai-team-orchestration`提供以下模板：
- **PROJECT_BRIEF.md** - 14节跨聊天真实的单一来源
- **头脑风暴形式** -多智能体辩论不同的声音
- **Sprint计划** -优先任务，进度跟踪，交接文档
- **反模式** -来自真实多代理项目的19个记录陷阱

##快速入门

# # # 1。引导项目```
@ai-team-producer I want to build [describe your project].
Use /ai-team-orchestration to bootstrap this project.
Start with a brainstorm, then create PROJECT_BRIEF.md with ALL sections (1-14).
```
# # # 2。计划冲刺```
@ai-team-producer Create Sprint 1 plan. Scope: [what to build].
Run a team consilium to validate the plan.
```
# # # 3。执行（单独的VS Code窗口）```
@ai-team-dev Read PROJECT_BRIEF.md, then docs/sprint-1/plan.md. Execute Sprint 1.
```
# # # 4。测试（另一个VS Code窗口）```
@ai-team-qa Sprint 1 is merged to main. Do full playthrough.
File bugs as GitHub Issues. Write docs/qa/sprint-1-signoff.md.
```
##如何工作

人类充当并行聊天之间的消息总线。每个团队在一个单独的VS Code窗口中工作，并拥有自己的repo克隆：

**@ai-team-producer** -不能编辑代码（由工具限制强制执行）
**@ai-team-qa** -不能编辑源文件，只能编辑reads/tests/filesbug
**@ai-team-dev** -完整的工具，构建为Nova（前端），Sage（后端），Milo（设计）

# #起源

编写了《Arcade After Dark》（https://github.com/denis-a-evdokimov/guess-and-get）的工作流程，这是一款由7个AI代理在5天内完成的30款生日礼物应用。