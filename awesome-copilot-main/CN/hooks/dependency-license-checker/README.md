---
name: 'Dependency License Checker'
description: 'Scans newly added dependencies for license compliance (GPL, AGPL, etc.) at session end'
tags: ['compliance', 'license', 'dependencies', 'session-end']
---
#依赖许可证检查钩子

在GitHub Copilot编码代理会话结束时扫描新添加的依赖项以查看许可遵从性，在提交之前标记copyleft和限制性许可（GPL、AGPL、SSPL等）。

# #概述

AI编码代理可能会在会话期间添加新的依赖项而不考虑许可影响。通过检测跨多个生态系统的新依赖关系，查找它们的许可，并根据可配置的copyleft和限制性许可的阻止列表检查它们，该钩子充当了一个遵从性安全网。

# #特性- **多生态系统支持**:npm， pip, Go， Ruby和Rust依赖检测
- **两种模式**:`warn`（仅日志）或`block`（退出非零以防止提交）
- **可配置的封锁列表**：默认copyleft设置与完整的SPDX变体覆盖
- **Allowlist支持**：通过`LICENSE_ALLOWLIST`跳过已知可接受的包
- **智能检测**：使用`git diff`只检测新添加的依赖项
- **多个查找策略**：本地缓存，包管理器CLI，回退到UNKNOWN
- **结构化日志**:JSON行输出与监控工具集成
- **超时保护**：每次license查找都有5秒的超时
- **零强制依赖**：使用标准的Unix工具；可选的`jq`用于更好的JSON解析

# #安装

1. 将钩子文件夹复制到你的存储库：   ```bash
   cp -r hooks/dependency-license-checker .github/hooks/
   ```
2. 确保脚本是可执行的：   ```bash
   chmod +x .github/hooks/dependency-license-checker/check-licenses.sh
   ```
3. 创建logs目录并将其添加到`.gitignore`：   ```bash
   mkdir -p logs/copilot/license-checker
   echo "logs/" >> .gitignore
   ```
4. 将钩子配置提交到存储库的默认分支。

# #配置

钩子在`hooks.json`中被配置为在`sessionEnd`事件上运行：```json
{
  "version": 1,
  "hooks": {
    "sessionEnd": [
      {
        "type": "command",
        "bash": ".github/hooks/dependency-license-checker/check-licenses.sh",
        "cwd": ".",
        "env": {
          "LICENSE_MODE": "warn"
        },
        "timeoutSec": 60
      }
    ]
  }
}
```
环境变量

|变量|值|默认值|描述||----------|--------|---------|-------------|
|`LICENSE_MODE`|`warn`，`block`|`warn`|`warn`仅日志违规；`block`退出非零，以防止自动提交|
|`SKIP_LICENSE_CHECK`|`true`| unset |完全禁用检查器|
|`LICENSE_LOG_DIR`|路径|`logs/copilot/license-checker`|检查日志写入目录|
|`BLOCKED_LICENSES`|逗号分隔的SPDX id | copyleft设置|许可证标志为违规|
|`LICENSE_ALLOWLIST`|逗号分隔|不设置|要跳过的包名（例如，`linux-headers,glibc`） |

##如何工作1. 当Copilot编码代理会话结束时，钩子执行
2. 根据清单文件运行`git diff HEAD`（package.json,requirements.txt, go）。国防部等)
3. 从diff输出中提取新添加的包名
4. 使用本地缓存和包管理器cli查找每个包的许可证
5. 使用不区分大小写的子字符串匹配，根据被阻止的列表检查每个许可证
6. 在标记之前跳过allow列表中的包
7. 以带有包、生态系统、许可和状态的格式化表报告发现结果
8. 为审计目的编写结构化JSON日志条目
9. 在`block`模式中，退出非零以通知代理在提交之前停止

支持的生态系统

|生态系统|清单文件|主查找|后备||-----------|--------------|----------------|----------|
|npm/yarn/pnpm|`package.json`|`node_modules/<pkg>/package.json`license字段|`npm view <pkg> license`|
| pip |`requirements.txt`，`pyproject.toml`|`pip show <pkg>`License字段| UNKNOWN |
|进入|`go.mod`|模块缓存中的LICENSE文件（关键字match） | UNKNOWN |
| Ruby |`Gemfile`|`gem spec <pkg> license`|未知|
| Rust |`Cargo.toml`|`cargo metadata`license字段| UNKNOWN |

##默认阻止许可证

默认情况下，以下许可证被阻止（copyleft和限制性）：

- * * GPL * *: GPL - 2.0, GPL - 2.0, GPL - 2.0 -或-之后,GPL - 3.0, GPL - 3.0, GPL - 3.0 -或-
- * * AGPL * *: AGPL - 1.0, AGPL - 3.0, AGPL - 3.0, AGPL - 3.0 -或-
—**LGPL**: LGPL-2.0、LGPL-2.1、LGPL-2.1-only、LGPL-2.1及以上版本、LGPL-3.0、LGPL-3.0-only、LGPL-3.0及以上版本
——* * * *:sspl - 1.0, eupl - 1.1, eupl - 1.2, osl - 3.0, cpl cpal - 1.0, - 1.0
- **创作共用（限制性）**:CC-BY-SA-4.0, CC-BY-NC-4.0, CC-BY-NC-SA-4.0

使用`BLOCKED_LICENSES`覆盖以自定义。

##输出示例清除扫描（没有新的依赖项）```
✅ No new dependencies detected
```
清洁扫描（所有兼容）```
🔍 Checking licenses for 3 new dependency(ies)...

  PACKAGE                        ECOSYSTEM    LICENSE                        STATUS
  -------                        ---------    -------                        ------
  express                        npm          MIT                            OK
  lodash                         npm          MIT                            OK
  axios                          npm          MIT                            OK

✅ All 3 dependencies have compliant licenses
```
###检测到违规（警告模式）```
🔍 Checking licenses for 2 new dependency(ies)...

  PACKAGE                        ECOSYSTEM    LICENSE                        STATUS
  -------                        ---------    -------                        ------
  react                          npm          MIT                            OK
  readline-sync                  npm          GPL-3.0                        BLOCKED

⚠️  Found 1 license violation(s):

  - readline-sync (npm): GPL-3.0

💡 Review the violations above. Set LICENSE_MODE=block to prevent commits with license issues.
```
检测到违规（阻塞模式）```
🔍 Checking licenses for 2 new dependency(ies)...

  PACKAGE                        ECOSYSTEM    LICENSE                        STATUS
  -------                        ---------    -------                        ------
  flask                          pip          BSD-3-Clause                   OK
  copyleft-lib                   pip          AGPL-3.0                       BLOCKED

⚠️  Found 1 license violation(s):

  - copyleft-lib (pip): AGPL-3.0

🚫 Session blocked: resolve license violations above before committing.
   Set LICENSE_MODE=warn to log without blocking, or add packages to LICENSE_ALLOWLIST.
```
##日志格式

检查事件以JSON行格式写入`logs/copilot/license-checker/check.log`：```json
{"timestamp":"2026-03-17T10:30:00Z","event":"license_check_complete","mode":"warn","dependencies_checked":3,"violation_count":1,"violations":[{"package":"readline-sync","ecosystem":"npm","license":"GPL-3.0","status":"BLOCKED"}]}
```

```json
{"timestamp":"2026-03-17T10:30:00Z","event":"license_check_complete","mode":"warn","status":"clean","dependencies_checked":0}
```
与其他钩子配对

这个钩子很适合搭配：

—**秘密扫描**：在自动提交之前，先运行秘密扫描，然后进行许可证检查
—**Session Auto-Commit**：当两者都安装时，命令它们首先运行`dependency-license-checker`。设置`LICENSE_MODE=block`以防止在检测到违规时自动提交。

# #定制

—**修改被阻止的license **：设置`BLOCKED_LICENSES`为自定义的SPDX id列表，以逗号分隔
- **Allowlist packages**：使用`LICENSE_ALLOWLIST`用于已知可接受的带有copyleft许可的软件包
—**更改日志位置**：设置`LICENSE_LOG_DIR`，将日志路由到您喜欢的目录
- **添加生态系统**：扩展`check-licenses.sh`的检测和查找部分

# #禁用

暂时禁用检查器：

—在hook环境中设置`SKIP_LICENSE_CHECK=true`-或者从`hooks.json`中删除`sessionEnd`条目

# #的局限性- License检测依赖于manifest文件差异；没有检测到在标准清单文件之外添加的依赖项
—查找License需要包管理器CLI或本地缓存可用
-复合SPDX表达式（例如，`MIT OR GPL-3.0`）被标记，如果任何组件匹配阻止列表
-不执行深度传递依赖许可证分析
-网络查找（npm视图等）可能会在离线或受限环境中失败
—要求执行环境中存在“`git`”