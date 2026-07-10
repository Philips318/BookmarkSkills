---
name: ai-team-orchestration
description: 'Bootstrap and run a multi-agent AI development team. Use when: starting a new software project with AI agents, setting up parallel dev/QA teams, creating sprint plans, writing brainstorm prompts with distinct agent voices, recovering a project workflow, or planning sprints.'
---
# AI团队编排

##何时使用
-启动一个需要规划、开发、测试和部署的新项目
-建立并行的AI代理团队（dev， QA, DevOps）
撰写能够引发真正辩论的头脑风暴提示（而不是泛泛的输出）
-创建sprint计划与交叉聊天上下文生存
-从sprint中期的上下文溢出中恢复

团队角色

|代理|名称|角色|焦点||-------|------|------|-------|
|制作人| **人头马** | Sprint计划，协调，合并pr |范围控制，交接，问题分类|
|产品设计师| **Kira** |用户体验、机制、用户体验|乐趣因素、用户流程、功能设计|
| **Milo** | CSS，动画，视觉识别|设计系统，润色，可达性|
| **Nova** | UI框架，状态管理，组件|React/Vue/Svelte，客户端逻辑|
| **Sage** | API、数据库、鉴权、安全|服务器端逻辑、基础设施|
| **Dash** |CI/CD，云部署，管道|GitHub Actions，Azure/AWS/GCP|
| **Ivy** | E2E测试，自动化，游戏测试|Playwright/Cypress， bug归档，签字|

为项目自定义名称和角色。并非每个项目都需要所有角色。

##聊天架构

人（CEO）是并行聊天之间的消息总线：```
┌────────────────────────────────────────┐
│  @ai-team-producer — Plans, merges     │
│  NEVER writes code                     │
└────────────────┬───────────────────────┘
                 │ Human carries messages
      ┌──────────┼──────────┐
      ▼          ▼          ▼
┌──────────┐ ┌────────┐ ┌────────┐
│@ai-team  │ │@ai-team│ │DevOps  │
│-dev      │ │-qa     │ │(on     │
│          │ │        │ │demand) │
│ Nova     │ │ Ivy    │ │        │
│ Sage     │ │        │ │        │
│ Milo     │ │        │ │        │
│          │ │feature/│ │feature/│
│ feature/ │ │qa-N    │ │devops-N│
│ sprint-N │ └────────┘ └────────┘
└──────────┘
```
每个团队工作在一个独立的VS Code窗口**与自己的克隆：```bash
git clone <repo> project-dev    # Dev team
git clone <repo> project-qa     # QA
git clone <repo> project-devops # DevOps (only when needed)
```
##项目引导

# # # 1。创建PROJECT_BRIEF.md所有聊天的唯一真相来源。请参阅[项目简要模板]（./references/project-brief-template.md）。

**要求的部分（不要缩写）：**
1. 项目概述
2. 概念/产品描述
3. 技术堆栈
4. 架构（ASCII图）
5. 关键文件映射
6. 团队角色
7. Sprint状态（每次Sprint更新）
8. 当前状态（每个sprint都重写）
9. 安全规则
10. 如何在本地运行
11. 如何部署
12. **跨聊天切换协议** -上下文如何在聊天之间存活
13. **错误和修复跟踪** - GitHub问题作为真相的单一来源
14. **多repo设置** -独立克隆，分支策略，合并规则

# # # 2。头脑风暴

参见[头脑风暴格式]（./references/brainstorm-format.md）。关键：明确地为每个代理命名，并赋予其独特的个性和视角。至少需要两次真正的分歧来防止群体思维。

# # # 3。制定冲刺计划参见[冲刺计划模板]（./references/sprint-plan-template.md）。每个冲刺得到：
-`docs/sprint-N/plan.md`-优先任务，成功标准
-`docs/sprint-N/progress.md`-实时跟踪器，启用恢复
-`docs/sprint-N/done.md`-在sprint结束时编写的切换文档

# # # 4。执行冲刺```
Read PROJECT_BRIEF.md, then read docs/sprint-N/plan.md. Execute Sprint N.

First: git pull origin main && git checkout -b feature/sprint-N

Close GitHub Issues in commits: "fix: description (Fixes #NN)"
Update docs/sprint-N/progress.md after each phase.
When done, push and create PR: git push origin feature/sprint-N
Follow Sections 12-14 of PROJECT_BRIEF.md.
```
# # # 5。QA签字

在开发合并后，QA会进行一次完整的攻关：```
Read PROJECT_BRIEF.md. You are Ivy (QA).
Sprint N is merged to main. Do full playthrough.
File bugs as GitHub Issues. Write docs/qa/sprint-N-signoff.md.
```
##恢复

当聊天变得很长（bbb100条消息），保存状态并重新开始：

关闭前* *:* *
1. 用当前状态更新`docs/sprint-N/progress.md`2. 更新`PROJECT_BRIEF.md`第7+8节
3. 写`docs/sprint-N/done.md`**冷启动提示：**```
Read PROJECT_BRIEF.md and docs/sprint-N/progress.md.
Continue from where it left off.
```
# #反模式

请参阅[反模式参考]（./references/anti-patterns.md）了解完整的列表。前5名:

|不要|要||-------|------------|
| Rebase特性分支|合并（Rebase丢失提交）|
|生产者写代码|生产者只计划，合并，文件问题|
|批量“修复一切”提交|每个修复一次提交，问题引用|
|模糊的头脑风暴提示|用不同的视角命名每个代理|
|只在聊天|文件GitHub问题（聊天上下文死亡）|

更好的结果提示

- **“慢慢来，做正确的事”**在提示中产生的输出比匆忙更好
**合并前测试** -你测试，文件问题，开发修复，然后合并
- **在主要冲刺前进行团队会议** -每个代理从他们的角度审查计划
- **保存教训的记忆**后，每个里程碑