---
name: 'Tool Guardian'
description: 'Blocks dangerous tool operations (destructive file ops, force pushes, DB drops) before the Copilot coding agent executes them'
tags: ['security', 'safety', 'preToolUse', 'guardrails']
---
#工具守护钩

在GitHub Copilot编码代理执行危险的工具操作之前，阻止它们，充当防止破坏性命令、强制推送、数据库删除和其他高风险操作的安全网。

# #概述

AI编码代理可以自主执行shell命令、文件操作和数据库查询。如果没有护栏，一个错误的指令可能会导致不可逆转的损害。这个钩子在`preToolUse`事件中拦截每个工具调用，并针对6类中的约20种威胁模式扫描它：- **破坏文件ops**:`rm -rf /`，删除`.env`或`.git`- **破坏性git ops**:`git push --force`到main/master，`git reset --hard`- **数据库破坏**:`DROP TABLE`，`DROP DATABASE`,`TRUNCATE`,`DELETE FROM`，不含`WHERE`- **权限滥用**:`chmod 777`，递归全域可写权限
- **网络泄露**:`curl | bash`，`wget | sh`，通过`curl --data @`上传文件
- **系统危险**:`sudo`、`npm publish`# #特性- **两种保护模式**:`block`（退出非零以防止执行）或`warn`（仅日志）
- **更安全的替代方案**：每个被阻止的模式都包含一个更安全的命令建议
- **Allowlist支持**：跳过特定的模式通过`TOOL_GUARD_ALLOWLIST`- **结构化日志**:JSON行输出与监控工具集成
- **快速执行**:10秒超时；没有外部网络呼叫
- **零依赖**：仅使用标准Unix工具（`grep`,`sed`）；用于输入解析的可选`jq`# #安装

1. 将钩子文件夹复制到你的存储库：   ```bash
   cp -r hooks/tool-guardian your-repo/hooks/
   ```
2. 确保脚本是可执行的：   ```bash
   chmod +x hooks/tool-guardian/guard-tool.sh
   ```
3. 创建logs目录并将其添加到`.gitignore`：   ```bash
   mkdir -p .github/logs/copilot/tool-guardian
   echo ".github/logs/" >> .gitignore
   ```
4. 将钩子配置提交到存储库的默认分支。

# #配置

钩子在`hooks.json`中被配置为在`preToolUse`事件上运行：```json
{
  "version": 1,
  "hooks": {
    "preToolUse": [
      {
        "type": "command",
        "bash": "hooks/tool-guardian/guard-tool.sh",
        "cwd": ".",
        "env": {
          "GUARD_MODE": "block"
        },
        "timeoutSec": 10
      }
    ]
  }
}
```
环境变量

|变量|值|默认值|描述||----------|--------|---------|-------------|
|`GUARD_MODE`|`warn`，`block`|`block`|`warn`仅记录威胁日志；`block`退出非零，以防止工具执行|
|`SKIP_TOOL_GUARD`|`true`| unset |完全禁用守护|
|`TOOL_GUARD_LOG_DIR`|路径|`.github/logs/copilot/tool-guardian`|保护日志写入目录|
|`TOOL_GUARD_ALLOWLIST`|逗号分隔|取消|模式跳过（例如，`git push --force,npm publish`） |

##如何工作1. 在Copilot编码代理执行工具之前，钩子在stdin上以JSON的形式接收工具调用
2. 提取`toolName`和`toolInput`字段（如果可用，通过`jq`，否则通过regex回退）
3. 根据允许列表检查组合文本-如果匹配，则跳过所有扫描
4. 针对6个严重性类别的约20个正则表达式威胁模式扫描组合文本
5. 报告发现的类别、严重程度、匹配的文本和更安全的替代方案
6. 为审计目的编写结构化JSON日志条目
7. 在`block`模式下，退出非零以防止工具执行
8. 在`warn`模式下，记录威胁并允许继续执行

##威胁类别

|类别|严重性|关键模式|建议||----------|----------|-------------|------------|
|`destructive_file_ops`| critical |`rm -rf /`、`rm -rf ~`、`rm -rf .`， delete`.env`/`.git`|使用目标路径或`mv`备份|
|`destructive_git_ops`|critical/high|`git push --force`至main/master，`git reset --hard`，`git clean -fd`|使用`--force-with-lease`，`git stash`，干跑|
|`database_destruction`|critical/high|`DROP TABLE`,`DROP DATABASE`,`TRUNCATE`，`DELETE FROM`without WHERE |使用迁移，备份，添加WHERE子句|
|`permission_abuse`|高|`chmod 777`，`chmod -R 777`|使用`755`为dirs，`644`为files |
|`network_exfiltration`|critical/high|`curl \| bash`,`wget \| sh`，`curl --data @file`|先下载，检查，然后执行|
|`system_danger`|高|`sudo`，`npm publish`|使用最小权限；`--dry-run`first |

# #的例子

###安全命令（退出0）```bash
echo '{"toolName":"bash","toolInput":"git status"}' | bash hooks/tool-guardian/guard-tool.sh
```
###命令被阻塞（exit 1）```bash
echo '{"toolName":"bash","toolInput":"git push --force origin main"}' | \
  GUARD_MODE=block bash hooks/tool-guardian/guard-tool.sh
```

```
🛡️  Tool Guardian: 1 threat(s) detected in 'bash' invocation

  CATEGORY                 SEVERITY   MATCH                                    SUGGESTION
  --------                 --------   -----                                    ----------
  destructive_git_ops      critical   git push --force origin main             Use 'git push --force-with-lease' or push to a feature branch

🚫 Operation blocked: resolve the threats above or adjust TOOL_GUARD_ALLOWLIST.
   Set GUARD_MODE=warn to log without blocking.
```
###警告模式（退出0，威胁记录）```bash
echo '{"toolName":"bash","toolInput":"rm -rf /"}' | \
  GUARD_MODE=warn bash hooks/tool-guardian/guard-tool.sh
```
### Allowlisted command （exit 0）```bash
echo '{"toolName":"bash","toolInput":"git push --force origin main"}' | \
  TOOL_GUARD_ALLOWLIST="git push --force" bash hooks/tool-guardian/guard-tool.sh
```
##日志格式

守卫事件以JSON行格式写入`.github/logs/copilot/tool-guardian/guard.log`：```json
{"timestamp":"2026-03-16T10:30:00Z","event":"threats_detected","mode":"block","tool":"bash","threat_count":1,"threats":[{"category":"destructive_git_ops","severity":"critical","match":"git push --force origin main","suggestion":"Use 'git push --force-with-lease' or push to a feature branch"}]}
```

```json
{"timestamp":"2026-03-16T10:30:00Z","event":"guard_passed","mode":"block","tool":"bash"}
```

```json
{"timestamp":"2026-03-16T10:30:00Z","event":"guard_skipped","reason":"allowlisted","tool":"bash"}
```
# #定制

- **添加自定义模式**：编辑`PATTERNS`数组在`guard-tool.sh`添加项目特定的威胁模式
- **调整严重性**：更改需要不同处理的模式的严重性级别
- **Allowlist已知命令**：使用`TOOL_GUARD_ALLOWLIST`命令在您的上下文中是安全的
—**更改日志位置**：设置`TOOL_GUARD_LOG_DIR`，将日志路由到您喜欢的目录

# #禁用

暂时使监护人失效：

—设置钩子环境中的`SKIP_TOOL_GUARD=true`-或者从`hooks.json`中删除`preToolUse`条目

# #的局限性-基于模式的检测；不执行命令意图的语义分析
-可能对在安全上下文中匹配模式的命令产生误报（使用allowlist来抑制这些）
-扫描工具输入的文本表示；无法检测混淆或编码的命令
-要求工具调用作为JSON在stdin上传递`toolName`和`toolInput`字段