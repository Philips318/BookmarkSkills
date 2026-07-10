# BDD 规格

此目录包含按计划 slug 组织的 Gherkin `.feature` 文件。

```
.harness/specs/
├── {plan-slug}/
│   ├── task-1-short-name.feature
│   ├── task-2-short-name.feature
│   └── task-3-short-name.feature
└── README.md
```

## 目的

Feature 文件同时作为**规格说明和可执行验收测试**：
- `@planner` agent 将它们写成行为规格
- `@executor` agent 实现步骤定义，使其通过
- `@evaluator` agent 将它们作为主要验证门运行

## 规则

- Feature 文件由 `@planner` 创建，绝不能由 `@executor` 修改
- 每个 feature 文件都精确映射到计划 JSON 中的一个任务
- 场景必须彼此独立（无共享可变状态）
- 步骤应使用领域语言，而不是实现细节

## BDD 框架

本项目使用 **Reqnroll**（SpecFlow 的继任者）执行 .NET BDD。
