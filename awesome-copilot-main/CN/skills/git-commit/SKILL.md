---
name: git-commit
description: 'Execute git commit with conventional commit message analysis, intelligent staging, and message generation. Use when user asks to commit changes, create a git commit, or mentions "/commit". Supports: (1) Auto-detecting type and scope from changes, (2) Generating conventional commit messages from diff, (3) Interactive commit with optional type/scope/description overrides, (4) Intelligent file staging for logical grouping'
license: MIT
allowed-tools: Bash
---
# Git Commit with Conventional Commits

# #概述

使用常规提交规范创建标准化、语义化的git提交。分析实际困难以确定适当的类型、范围和消息。

常规提交格式```
<type>[optional scope]: <description>

[optional body]

[optional footer(s)]
```
##提交类型

|类型|用途|| ---------- | ------------------------------ |
|`feat`|新增特性|
|`fix`|修复|
|`docs`|仅|文档
|`style`|Formatting/style（无逻辑）|
|`refactor`|代码重构(没有feature/fix|`perf`|性能提升|
|`test`|Add/update测试|
|`build`|构建system/dependencies|
|`ci`|CI/config改变|
|`chore`|Maintenance/misc|
|`revert`|恢复提交|

##打破改变```
# Exclamation mark after type/scope
feat!: remove deprecated endpoint

# BREAKING CHANGE footer
feat: allow config to extend other configs

BREAKING CHANGE: `extends` key behavior changed
```
# #工作流程

# # # 1。分析差异```bash
# If files are staged, use staged diff
git diff --staged

# If nothing staged, use working tree diff
git diff

# Also check status
git status --porcelain
```
# # # 2。舞台文件（如有需要）

如果没有阶段性更改，或者您想以不同的方式对更改进行分组：```bash
# Stage specific files
git add path/to/file1 path/to/file2

# Stage by pattern
git add *.test.*
git add src/components/*

# Interactive staging
git add -p
```
永远不要隐瞒秘密。Env,credentials.json，私钥)。

# # # 3。生成提交消息

分析困难来确定：

- **类型**：这是什么样的变化？
- **范围**:area/module受影响的范围是什么？
- **描述**：一行关于变化的总结（现在时，祈使语气，<72字符）

# # # 4。执行提交```bash
# Single line
git commit -m "<type>[scope]: <description>"

# Multi-line with body/footer
git commit -m "$(cat <<'EOF'
<type>[scope]: <description>

<optional body>

<optional footer>
EOF
)"
```
最佳实践

-每次提交一个逻辑更改
-现在时：“add”而不是“added”
命令式语气：“修复bug”而不是“修复bug”
—参考号：`Closes #123`、`Refs #456`—描述不超过72个字符

Git安全协议

-永远不要更新git配置
-不要在没有明确请求的情况下运行破坏性命令（——force, hard reset）
-永远不要跳过钩子（- no-verify），除非用户要求
-永远不要强行推到main/master如果由于钩子导致提交失败，修复并创建新提交（不要修改）