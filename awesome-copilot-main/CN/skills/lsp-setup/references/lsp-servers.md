# Copilot CLI的已知LSP服务器`lsp-setup`技能的参考数据。每个部分都包含每个操作系统的安装命令和一个现成的配置片段。

b> **配置代码段格式**：下面的每个代码段显示要插入的对象作为顶级`lspServers`键下的值。完整的配置文件如下所示：`{ "lspServers": { <snippet here> } }`。在添加多种语言时，将它们的代码片段合并为`lspServers`下的兄弟键。

---

## TypeScript / JavaScript

* *服务器* *:[typescript-language-server] (https://github.com/typescript-language-server/typescript-language-server)

# # #安装

|操作系统|命令||---------|-------------------------------------------------------|
|任意|`npm install -g typescript typescript-language-server`|

配置代码片段```json
{
  "typescript": {
    "command": "typescript-language-server",
    "args": ["--stdio"],
    "fileExtensions": {
      ".ts": "typescript",
      ".tsx": "typescriptreact",
      ".js": "javascript",
      ".jsx": "javascriptreact"
    }
  }
}
```
---

# # Java

**服务器**:[Eclipse JDT语言服务器（jdtls）]（https://github.com/eclipse-jdtls/eclipse.jdt.ls）

需要**Java 21+**在`JAVA_HOME`或`$PATH`。

# # #安装

|操作系统|命令||---------|-----------------------------------|
| macOS |`brew install jdtls`|
检查`jdtls`或`eclipse.jdt.ls`的发行版；或者从https://download.eclipse.org/jdtls/milestones/|下载
从https://download.eclipse.org/jdtls/milestones/下载并将`bin/`添加到`PATH`|

在带有Homebrew的macOS上，二进制文件被安装为`jdtls`上的`$PATH`。

配置代码片段```json
{
  "java": {
    "command": "jdtls",
    "args": [],
    "fileExtensions": {
      ".java": "java"
    }
  }
}
```
**注意**:`jdtls`包装器脚本在内部处理`--stdio`模式。如果使用手动安装，您可能需要直接调用启动器jar -请参阅[jdtls README]（https://github.com/eclipse-jdtls/eclipse.jdt.ls#running-from-command-line-with-wrapper-script）了解详细信息。

---

# # Python

* *服务器* *:[pyright] (https://github.com/microsoft/pyright)

# # #安装

|操作系统|命令||---------|----------------------------|
|任意|`npm install -g pyright`|
|任意|`pip install pyright`|

配置代码片段```json
{
  "python": {
    "command": "pyright-langserver",
    "args": ["--stdio"],
    "fileExtensions": {
      ".py": "python"
    }
  }
}
```
---

# #去

* *服务器* *:[gopls] (https://github.com/golang/tools/tree/master/gopls)

# # #安装

|操作系统|命令||---------|--------------------------------------------|
|任意|`go install golang.org/x/tools/gopls@latest`|
| macOS |`brew install gopls`|

配置代码片段```json
{
  "go": {
    "command": "gopls",
    "args": ["serve"],
    "fileExtensions": {
      ".go": "go"
    }
  }
}
```
---

# #生锈

* *服务器* *:[rust-analyzer] (https://github.com/rust-lang/rust-analyzer)

# # #安装

|操作系统|命令||---------|--------------------------------|
|任意|`rustup component add rust-analyzer`|
| macOS |`brew install rust-analyzer`|
| Linux |发行包或`rustup`|
| Windows |`rustup component add rust-analyzer`或从GitHub下载发布|

配置代码片段```json
{
  "rust": {
    "command": "rust-analyzer",
    "args": [],
    "fileExtensions": {
      ".rs": "rust"
    }
  }
}
```
---

## c / c++

* *服务器* *:[clangd] (https://clangd.llvm.org/)

# # #安装

|操作系统|命令||---------|----------------------------------------|
| macOS |`brew install llvm`（包括clangd）或Xcode命令行工具|
| Linux |`apt install clangd`/`dnf install clang-tools-extra`|
| Windows |从https://releases.llvm.org/|下载LLVM

配置代码片段```json
{
  "cpp": {
    "command": "clangd",
    "args": ["--background-index"],
    "fileExtensions": {
      ".c": "c",
      ".h": "c",
      ".cpp": "cpp",
      ".cxx": "cpp",
      ".cc": "cpp",
      ".hpp": "cpp",
      ".hxx": "cpp"
    }
  }
}
```
---

## c # （）净)

**服务器**:[Roslyn语言服务器](https://github.com/dotnet/roslyn)（通过`dotnet dnx`）

# # #安装

|操作系统|命令||---------|----------------------------------------------------------------|
|任意|需要[。. NET SDK]（https://dot.net/download）安装|

配置代码片段```json
{
  "csharp": {
    "command": "dotnet",
    "args": ["dnx", "roslyn-language-server", "--yes", "--prerelease", "--", "--stdio", "--autoLoadProjects"],
    "fileExtensions": {
      ".cs": "csharp"
    }
  }
}
```
---

# #红宝石

* *服务器* *:[solargraph] (https://github.com/castwide/solargraph)

# # #安装

|操作系统|命令||---------|---------------------------|
|任意|`gem install solargraph`|

配置代码片段```json
{
  "ruby": {
    "command": "solargraph",
    "args": ["stdio"],
    "fileExtensions": {
      ".rb": "ruby",
      ".rake": "ruby",
      ".gemspec": "ruby"
    }
  }
}
```
---

# # PHP

* *服务器* *:[intelephense] (https://github.com/bmewburn/vscode-intelephense)

# # #安装

|操作系统|命令||---------|--------------------------------------------|
|任意|`npm install -g intelephense`|

配置代码片段```json
{
  "php": {
    "command": "intelephense",
    "args": ["--stdio"],
    "fileExtensions": {
      ".php": "php"
    }
  }
}
```
---

# #芬兰湾的科特林

* *服务器* *:[kotlin-language-server] (https://github.com/fwcd/kotlin-language-server)

# # #安装

|操作系统|命令||---------|---------------------------------------------------|
| macOS |`brew install kotlin-language-server`|
|任何|从GitHub版本下载并添加到`PATH`|

配置代码片段```json
{
  "kotlin": {
    "command": "kotlin-language-server",
    "args": [],
    "fileExtensions": {
      ".kt": "kotlin",
      ".kts": "kotlin"
    }
  }
}
```
---

# #迅速

**服务器**:[sourcekit-lsp](https://github.com/swiftlang/sourcekit-lsp)（与Swift工具链捆绑）

# # #安装

|操作系统|命令||---------|----------------------------------------------------------------|
| macOS |包含Xcode；二进制在`xcrun sourcekit-lsp`|
| Linux |包含Swift工具链；从https://swift.org|安装
| Windows |包含Swift工具链；从https://swift.org|安装

配置代码片段```json
{
  "swift": {
    "command": "sourcekit-lsp",
    "args": [],
    "fileExtensions": {
      ".swift": "swift"
    }
  }
}
```
在macOS上，您可能需要使用完整路径：`/usr/bin/sourcekit-lsp`或将`command`设置为`xcrun`，并使用`args: ["sourcekit-lsp"]`。

---

# # Lua

* *服务器* *:[lua-language-server] (https://github.com/LuaLS/lua-language-server)

# # #安装

|操作系统|命令||---------|--------------------------------------|
| macOS |`brew install lua-language-server`|
| Linux |从GitHub下载发布|
| Windows |从GitHub下载发布|

配置代码片段```json
{
  "lua": {
    "command": "lua-language-server",
    "args": [],
    "fileExtensions": {
      ".lua": "lua"
    }
  }
}
```
---

# # YAML

* *服务器* *:[yaml-language-server] (https://github.com/redhat-developer/yaml-language-server)

# # #安装

|操作系统|命令||---------|----------------------------------------------|
|任意|`npm install -g yaml-language-server`|

配置代码片段```json
{
  "yaml": {
    "command": "yaml-language-server",
    "args": ["--stdio"],
    "fileExtensions": {
      ".yaml": "yaml",
      ".yml": "yaml"
    }
  }
}
```
---

## Bash / Shell

* *服务器* *:[bash-language-server] (https://github.com/bash-lsp/bash-language-server)

# # #安装

|操作系统|命令||---------|-----------------------------------------------|
|任意|`npm install -g bash-language-server`|

配置代码片段```json
{
  "bash": {
    "command": "bash-language-server",
    "args": ["start"],
    "fileExtensions": {
      ".sh": "shellscript",
      ".bash": "shellscript",
      ".zsh": "shellscript"
    }
  }
}
```
