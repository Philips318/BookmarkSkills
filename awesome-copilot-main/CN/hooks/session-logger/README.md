---
name: 'Session Logger'
description: 'Logs all Copilot coding agent session activity for audit and analysis'
tags: ['logging', 'audit', 'analytics']
---
# Session Logger钩子

对GitHub Copilot编码代理会话进行全面的日志记录，跟踪会话的开始、结束以及审计跟踪和使用分析的用户提示。

# #概述

这个钩子提供了副驾驶编码代理活动的详细日志记录：
-会话start/end次与工作目录上下文
—用户提示提交事件
—可配置日志级别

# #特性

—**会话跟踪**：记录会话开始和结束事件
—**提示日志**：记录提交用户提示时的日志
- **结构化日志**:JSON格式，便于解析
- **隐私意识**：可配置完全禁用日志记录

# #安装

1. 将这个钩子文件夹复制到存储库的`.github/hooks/`目录：   ```bash
   cp -r hooks/session-logger .github/hooks/
   ```
2. 创建logs目录：   ```bash
   mkdir -p logs/copilot
   ```
3. 确保脚本是可执行的：   ```bash
   chmod +x .github/hooks/session-logger/*.sh
   ```
4. 将钩子配置提交到存储库的默认分支

##日志格式

会话事件写入`logs/copilot/session.log`，并以JSON格式提示事件到`logs/copilot/prompts.log`：```json
{"timestamp":"2024-01-15T10:30:00Z","event":"sessionStart","cwd":"/workspace/project"}
{"timestamp":"2024-01-15T10:35:00Z","event":"sessionEnd"}
```
隐私和安全

—将“`logs/`”添加到“`.gitignore`”中，避免提交会话数据
—使用`LOG_LEVEL=ERROR`只记录错误
—将环境变量`SKIP_LOGGING=true`设置为“disable”
—日志只保存在本地