---
name: vardoger-analyze
description: "Use when the user asks to personalize the GitHub Copilot CLI assistant, adapt Copilot to their style, use vardoger, or analyze their Copilot CLI conversation history. Reads the local session directory at `~/.copilot/session-state/`, extracts recurring preferences and conventions, and writes a fenced personalization block into `~/.copilot/copilot-instructions.md`. Runs entirely on the user's machine via the local `vardoger` CLI (`pipx install vardoger`); no network calls and no uploads. Triggers: 'personalize my copilot', 'analyze my copilot history', 'tailor copilot to me', 'run vardoger', 'update my copilot instructions from history', 'make copilot learn my style'."
license: Apache-2.0
---
#分析副驾驶CLI历史并生成个性化指令

驱动本地`vardoger`CLI读取用户的GitHub CopilotCLI对话历史记录，提取行为模式，并将个性化块写入`~/.copilot/copilot-instructions.md`。

##它是如何工作的`vardoger`批量准备历史。你（助手）总结每一批行为信号，然后综合所有的总结为最终的个性化。`vardoger`写入结果，由`<!-- vardoger:start -->`/`<!-- vardoger:end -->`标记隔离，因此保留同一文件中任何手工编写的规则。

##沙盒注释（在运行任何命令之前阅读）`vardoger`在当前工作空间之外读写文件：

-从`~/.copilot/session-state/`读取副驾驶CLI历史。
—写入检查点状态文件到`~/.vardoger/state.json`（第一次运行时创建）。
-将最终的个性化写入`~/.copilot/copilot-instructions.md`。当主机请求批准`vardoger`命令时，授予它在工作空间之外的写访问权限。否则，第一个`vardoger prepare`调用将在使用`PermissionError: ... ~/.vardoger/state.tmp`时失败，因为沙箱阻塞了当前工作目录之外的写操作。

# #工作流程

1. 验证`vardoger`CLI是否安装，如果没有安装，请使用安装指导快速失败。
2. 使用`vardoger status --platform copilot --json`检查是否过期，如果个性化仍然新鲜，请尽早停止。
3. 使用`vardoger prepare --platform copilot`获取批元数据以了解批的数量。
4. 对于每个批处理，运行`vardoger prepare --platform copilot --batch <N>`并编写行为信号的简明项目摘要。
5. 使用`vardoger prepare --platform copilot --synthesize`获得合成提示符。
6. 根据合成提示将所有批摘要合成为单个个性化。
7. 通过将个性化输入`vardoger write --platform copilot --scope global`（或`--scope project --project <path>`）来编写结果。
8. 向用户报告写了什么，在哪里写的，以及写的是幂等的。

# #的步骤# # # 1。验证vardoger是否已安装```bash
if ! command -v vardoger >/dev/null 2>&1; then
  cat <<'INSTALL_EOF'
vardoger CLI is not installed.

This skill calls the `vardoger` CLI to read your Copilot CLI history and
write a personalization file, so the CLI must be on PATH.

Install options:

  # Recommended:
  pipx install vardoger

  # Or run without installing:
  uvx vardoger --help

If you do not have pipx, see https://pipx.pypa.io/stable/installation/.

Project page: https://github.com/dstrupl/vardoger

After installing, re-run the personalization request.
INSTALL_EOF
  exit 1
fi
```
# # # 2。检查是否需要刷新```bash
vardoger status --platform copilot --json
```
如果输出显示`"is_stale": false`，则告诉用户他们的个性化设置是最新的，并询问他们是否希望重新运行。如果陈旧或从未生成，则继续分析。

# # # 3。获取批处理元数据```bash
vardoger prepare --platform copilot
```
这将输出像`{"batches": 3, "total_conversations": 29}`这样的JSON。注意批次的数量。告诉用户：“在M批中找到N个对话。分析……”

# # # 4。总结每批

对于从1到N的每个批号，运行：```bash
vardoger prepare --platform copilot --batch 1
```
输出包含一个摘要提示，后面跟着对话数据。仔细阅读输出，并对您在该批次中观察到的行为信号进行简洁的要点总结。把你的总结留到以后。

告诉用户你正在处理哪个批：“分析N的第1批……”

重复所有批次（`--batch 2`，`--batch 3`等）。

# # # 5。获得合成提示```bash
vardoger prepare --platform copilot --synthesize
```
# # # 6。综合个性化

按照合成提示，将所有批处理摘要合并到单个个性化中。输出应该是清晰的标记，并为人工智能助手提供可操作的指令。

# # # 7。写出结果

管道您的个性化到`vardoger`：```bash
echo "YOUR_PERSONALIZATION_HERE" | vardoger write --platform copilot --scope global
```
将`YOUR_PERSONALIZATION_HERE`替换为生成的实际个性化标记。`--scope global`写入到`~/.copilot/copilot-instructions.md`；使用`--scope project --project <path>`将写入范围限定为特定的存储库。

# # # 8。向用户报告

告诉用户写了什么和在哪里。提到它们可以要求您随时重新运行vardoger以更新个性化，并且写入是幂等的（围栏块被替换；它之外的任何内容都被保留）。

##何时使用

-当用户要求个性化他们的副驾驶CLI助手。
-当用户要求分析他们的Copilot CLI对话历史。
-当用户提到“vardoger”时。