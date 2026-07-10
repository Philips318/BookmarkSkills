---
name: lsp-setup
description: 'Enable code intelligence (go-to-definition, find-references, hover, type info) for any programming language by installing and configuring an LSP server for Copilot CLI. Detects the OS, installs the right server, and generates the JSON configuration (user-level or repo-level). Use when you need deeper code understanding and no LSP server is configured, or when the user asks to set up, install, or configure an LSP server.'
---
# LSP Setup forGitHub CopilotCLI

**实用技能** -安装和配置语言服务器协议服务器的Copilot CLI。
适用于：“setup LSP”， “install language server”， “configure LSP FOR Java”， “add TypeScript LSP”， “enable code intelligence”， “I need to-definition”， “find references not working”， “需要更好的代码理解”
不要用于：一般编码任务，IDE/editorLSP配置，非copilot - cli设置

# #工作流程1. **询问语言** -使用`ask_user`询问用户希望支持哪种编程语言
2. **检测操作系统** -运行`uname -s`（或通过`$env:OS`/`%OS%`检查Windows）来确定macOS， Linux或Windows
3. **查找LSP服务器** -读取已知服务器的`references/lsp-servers.md`，安装命令和配置片段
4. **询问作用域** -使用`ask_user`询问配置应该是用户级（`~/.copilot/lsp-config.json`）还是回购级（`lsp.json`在回购根或`.github/lsp.json`）
5. **安装服务器** -根据检测到的操作系统，执行相应的Install命令
6. **写入配置** -将新的服务器条目合并到所选的配置文件中（`~/.copilot/lsp-config.json`为用户级；`lsp.json`或`.github/lsp.json`为仓库级）。如果一个仓库级别的配置已经存在，继续使用该位置；否则，询问用户他们更喜欢哪个仓库级别的位置。如果缺少，则创建文件并保留现有条目。
7. **验证** - con确认LSP二进制文件在`$PATH`上，并且配置文件是有效的JSON##配置格式

Copilot CLI从用户级或仓库级位置读取LSP配置，仓库级配置优先于用户级配置：

- **用户级**:`~/.copilot/lsp-config.json`- ** repo -level**:`lsp.json`（repo root）或`.github/lsp.json`JSON结构：```json
{
  "lspServers": {
    "<server-key>": {
      "command": "<binary>",
      "args": ["--stdio"],
      "fileExtensions": {
        ".<ext>": "<languageId>",
        ".<ext2>": "<languageId>"
      }
    }
  }
}
```
关键规则

—`command`为二进制名（必须在`$PATH`上）或绝对路径。
-`args`几乎总是包括`"--stdio"`使用标准的I/O传输。
-`fileExtensions`将每个文件扩展名（带前导点）映射到[语言ID]（https://code.visualstudio.com/docs/languages/identifiers#_known-language-identifiers）。
—“`lspServers`”中可以同时存在多个服务器。
-合并到现有文件时，**永远不会覆盖**其他服务器条目-只添加或更新目标语言键。

# #行为-在要求用户选择语言或范围时，始终使用`ask_user`和`choices`。
—如果“`references/lsp-servers.md`”中没有语言，请在web上搜索“<language>LSP服务器”，并指导用户手动配置。
-如果包管理器不可用（例如macOS上没有Homebrew），从参考文件中建议其他安装方法。
—安装完成后，执行命令`which <binary>`(Windows操作系统为`where.exe`)，确认二进制文件可以正常访问。
—在写入配置JSON之前，向用户显示最终的配置JSON。
-如果配置文件已经存在，先读取它，然后合并-不要合并。

# #验证

设置完成后，告诉用户：1. 输入`/exit`以退出Copilot CLI -这是**必需的**，以便在下次启动时加载新的LSP配置
2. 在项目中使用配置语言的文件重新启动`copilot`3. 执行`/lsp`命令，查看服务器状态
4. 尝试代码智能功能，如进入定义或悬停