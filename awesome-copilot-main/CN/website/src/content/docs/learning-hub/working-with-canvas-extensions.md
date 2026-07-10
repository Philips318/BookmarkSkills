---
title: 'Working with Canvas Extensions'
description: 'Create and iterate on GitHub Copilot app canvases using /create-canvas, then shape them into reusable project or personal extensions.'
authors:
  - GitHub Copilot Learning Hub Team
lastUpdated: 2026-06-17
estimatedReadingTime: '8 minutes'
tags:
  - copilot-app
  - canvases
  - canvas-extensions
relatedArticles:
  - ./github-copilot-app.md
  - ./agents-and-subagents.md
  - ./using-copilot-coding-agent.md
prerequisites:
  - Access to the GitHub Copilot app
  - Basic familiarity with GitHub Copilot agent sessions
---
Canvas扩展在GitHub Copilot应用程序中为您提供共享的交互式工作界面。而不是在聊天中保持所有进展，您可以将工作移动到可见的工件（例如板，文档，检查表或面向浏览器的界面），人员和代理都可以更新。

本指南解释画布可以做什么，如何使用`/create-canvas`创建画布，以及如何使用该存储库中的模式作为参考实现。

画布能做什么

画布是一个双向表面：

-您可以通过UI控件（按钮，表单，过滤器，卡片等）进行交互。
-代理可以调用画布功能来更新相同的状态
-您可以通过请求代理添加或修改功能来快速迭代

这使得画布在可见性和转向问题的工作流程中特别有用，例如：-检伤分类板
-规划文件
-实时浏览器辅助工作流程
-释放协调面

##用`/create-canvas`创建画布

在GitHub Copilot应用程序中，使用`/create-canvas`技能从活动会话创建画布。

1. 打开或启动代理会话。
2. 在提示框中输入`/create-canvas`，描述如下：
-你想要的工作流程
人们应该在UI中做什么
-代理应该通过可调用的功能做什么
3. 让代理生成扩展并在右侧面板中打开它。
4. 通过请求功能或UI更改来继续迭代。

提示模式运行良好

在提示符中使用显式能力语言：```text
/create-canvas Create an issue triage canvas with list filtering, label editing, and quick-priority actions. Add capabilities for get_issues, update_priority, and apply_label.
```

```text
/create-canvas Create a release checklist canvas that tracks milestones and owners. Add capabilities for add_item, assign_owner, mark_done, and export_summary.
```

```text
/create-canvas Create a markdown planning canvas that combines my open PRs and issues, and lets me launch and track agent sessions from the canvas.
```
选择范围：项目或个人

创建画布扩展时，选择它应该驻留的位置：

- **项目范围**:`.github/extensions`（与存储库团队共享）
- **用户范围**:`~/.copilot/extensions`（个人到您的机器）

当工作流与团队相关时使用项目范围，而对于个人实验或私人工作流使用用户范围。

典型的扩展结构

画布扩展可以有所不同，但大多数包括：

-`package.json`为元数据和依赖项
-`extension.mjs`（或另一个入口模块）用于画布行为和功能
-可选的UI文件（`index.html`，资产）为更丰富的面板控件
—可选持久化artifacts/state文件

最佳实践

# # # 1。有意选择存储范围

默认画布状态通常是会话范围的。如果您只需要当前会话的状态，请将其保存在会话存储路径中，例如：

——`<copilot_home>/session-state/<sessionId>/files/<whatever>`如果您希望数据在相同扩展的多个会话中持久化，请使用扩展范围的存储，例如：

——`<copilot_home>/extensions/<extensionId>/<whatever>`这种分离使短暂的工作流数据与长期存在的用户数据分开。

# # # 2。使用`joinSession`处理程序作为画布代理契约

将`joinSession`+`createCanvas`视为UI交互和代理可调用操作之间的契约：

-在`createCanvas(...)`中定义清晰的画布动作和模式
保持动作名称动词导向和可预测（`get_*`,`apply_*`,`sync_*`）
-从处理程序返回结构化状态，以便UI和代理保持同步

参考实现:

- SDKdocs/source: [`joinSession`](https://github.com/github/copilot-sdk/blob/main/nodejs/docs/extensions.md), [`createCanvas`]（https://github.com/github/copilot-sdk/blob/main/nodejs/src/canvas.ts）
—回购示例：[`extensions/backlog-swipe-triage/extension.mjs`]（https://github.com/github/awesome-copilot/blob/main/extensions/backlog-swipe-triage/extension.mjs）
—持久的用户作用域路径示例：[`extensions/chromium-control-canvas/extension.mjs`]（https://github.com/github/awesome-copilot/blob/main/extensions/chromium-control-canvas/extension.mjs）

来自此存储库的示例

使用这些扩展文件夹作为具体的参考：- [`Backlog Swipe Triage`](../../extensions/#backlog-swipe-triage)：基于滑动的问题分类界面，用于快速做出积压决策。
- [`Release Notes Showcase`](../../extensions/#release-notes-showcase)：发布说明编写和审查画布模式。
- [`Chromium Control Canvas`](../../extensions/# Chromium -control-canvas)：高级画布，协调面板控件与真正的头铬窗口。
- [`Agent Arcade`](../../extensions/#agent-arcade-canvas)：带有agent可调用控件的复古街机画布，用于在agent工作时选择或重新启动小游戏。

这些例子显示了不同的复杂程度，从专注的工作流板到更丰富的UI +自动化集成。

##第一次创建后迭代

将第一个`/create-canvas`结果视为版本1。然后就地改进：-随着工作流程的发展添加或重命名功能
-简化很少使用的控件
-在敏感动作周围添加护栏
保持能力名称清晰，以行动为导向

最快的循环是：**使用画布**，注意摩擦，并要求代理进行目标更新。

##下一步

-查看[GitHub Copilot应用程序概述]（../github-copilot-app/）更广泛的会话和工作流概念。
-浏览[画布扩展页]（../../extensions/）为可发现的扩展。
- Fork上面的一个示例扩展文件夹，并使其适应您自己的工作流程。

---