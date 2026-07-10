---
name: Quick Test
description: 'Generate tests for existing code: test-generator + code-review'
agent: 'Orchestrator'
---

从 Stage 5（测试生成）开始执行。

1. 分析用户指定的代码文件/类/方法
2. 使用 `test-generator` skill 生成单元测试
3. 使用 `code-review` skill 审查测试代码质量

目标代码：$input
