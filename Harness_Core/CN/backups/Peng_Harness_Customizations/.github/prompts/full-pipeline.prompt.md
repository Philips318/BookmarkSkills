---
name: Full Pipeline
description: 'Run the full Orchestrator pipeline with 7 role Agents: Requirement → Architecture → Code → Test → Review → Doc → CI/CD'
agent: 'Orchestrator'
---

执行全链路多Agent编排。Orchestrator 将依次委派给 7 个角色 Agent：

1. **RequirementsAnalyst** → 分析需求，产出需求包 + BDD + 手动测试用例
2. **Architect** → 架构设计，产出设计方案 + 接口定义
3. **Developer** → 实施代码变更
4. **Tester** → 生成单元测试(xUnit/AAA) + 手动测试用例 + 需求追溯
5. **Reviewer** → 代码审查（FAIL 则循环修复）
6. **DocWriter** → 生成文档
7. **CiCd** → 构建、测试、部署就绪验证

在需求分析和架构设计后等待用户确认再继续。
所有产出写入 `artifacts/<feature>/` 子目录，按功能名隔离，避免多次运行覆盖。

用户的需求描述如下：

$input
