---
name: 'Session Auto-Commit'
description: 'Automatically commits and pushes changes when a Copilot coding agent session ends'
tags: ['automation', 'git', 'productivity']
---
# Session自动提交钩子

当GitHub Copilot编码代理会话结束时，自动提交和推送更改，确保始终保存和备份您的工作。

# #概述

这个钩子在每个Copilot编码代理会话结束时运行，并自动：
-检测是否有未提交的更改
-处理所有更改
—创建带时间戳的提交
—推送到远端存储库

# #特性

- **自动备份**：永远不会从副驾驶会话丢失工作
—**时间戳提交**：每次自动提交都包含会话结束时间
- **安全执行**：只在实际发生更改时提交
—**错误处理**：优雅地处理推送失败

# #安装

1. 将这个钩子文件夹复制到存储库的`.github/hooks/`目录：   ```bash
   cp -r hooks/session-auto-commit .github/hooks/
   ```
2. 确保脚本是可执行的：   ```bash
   chmod +x .github/hooks/session-auto-commit/auto-commit.sh
   ```
3. 将钩子配置提交到存储库的默认分支

# #配置

钩子在`hooks.json`中被配置为在`sessionEnd`事件上运行：```json
{
  "version": 1,
  "hooks": {
    "sessionEnd": [
      {
        "type": "command",
        "bash": ".github/hooks/session-auto-commit/auto-commit.sh",
        "timeoutSec": 30
      }
    ]
  }
}
```
##如何工作

1. 当Copilot编码代理会话结束时，钩子执行
2. 检查是否在Git存储库中
3. 使用`git status`检测未提交的更改
4. 使用`git add -A`分阶段进行所有更改
5. 创建一个格式为：`auto-commit: YYYY-MM-DD HH:MM:SS`的提交
6. 尝试推送到远程
7. 报告成功或失败

# #定制

你可以通过修改`auto-commit.sh`来定制钩子：

—**Commit Message Format**：修改时间戳格式或消息前缀
- **选择性分期**：使用特定的git添加模式，而不是`-A`—**分支选择**：只推送到指定分支
- **通知**：添加桌面通知或Slack消息

# #禁用

暂时禁用自动提交：

1. 删除或注释掉`hooks.json`中的`sessionEnd`钩子
2. 或者设置一个环境变量：`export SKIP_AUTO_COMMIT=true`# #笔记-钩子使用`--no-verify`来避免触发预提交钩子
失败的推送不会阻止会话终止
-需要配置适当的git凭证
-工作与副驾驶编码代理和GitHub CopilotCLI