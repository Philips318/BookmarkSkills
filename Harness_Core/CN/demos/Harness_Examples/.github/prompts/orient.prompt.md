---
name: "orient"
description: "会话定向检查清单 — 在每个 developer 或 evaluator 会话开始时运行，用于在接触任何代码之前建立上下文。"
---

# 会话定向检查清单

在实现或评估任何内容之前完成全部步骤。即使你认为自己已经了解上下文，也不要跳过步骤。

## 1. 读取 Harness 状态

- [ ] 读取 `.harness/progress.md` — 上一次会话做了什么，是否有阻塞项或未解决决策
- [ ] 读取 `.harness/backlogs/` 中的活动 backlog 文件 — 哪些任务是 `pending`、`in_progress`、`complete` 或 `blocked`

## 2. 识别并理解你的任务

- [ ] 识别要处理的任务（由 orchestrator 指定，或最高优先级且没有未满足 `depends_on` 的 `pending` 任务）
- [ ] 读取任务的 `spec_file`（Gherkin feature 文件）— 准确理解哪些场景必须通过
- [ ] 读取任务的 `design_note` — 理解架构约束；这些是强制约束，不是建议
- [ ] 读取 `design_note` 中引用的 `.harness/architecture/adr/` 下的 ADR 文件

## 3. 验证基线

运行项目验证命令：

```
Build\Verify-Baseline.cmd
```

这个规范脚本会自动解析 `Src/` 下的 `*Impl.sln`，并运行 `dotnet build` + `dotnet test --no-build`。

- [ ] 构建无错误通过
- [ ] 所有现有测试通过（零个既有回归）

**如果基线失败：**先修复现有问题，然后继续。

## 4. 确认就绪

- [ ] 我确切知道自己正在实现或评估哪个任务
- [ ] 我已经阅读并理解 Gherkin spec
- [ ] 我已经阅读并理解架构指导（design_note + ADRs）
- [ ] 基线为绿色

**只有在所有复选框都满足后才继续。**
