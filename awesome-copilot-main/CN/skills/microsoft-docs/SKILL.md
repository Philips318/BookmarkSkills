---
name: microsoft-docs
description: 'Query official Microsoft documentation to find concepts, tutorials, and code examples across Azure, .NET, Agent Framework, Aspire, VS Code, GitHub, and more. Uses Microsoft Learn MCP as the default, with Context7 and Aspire MCP for content that lives outside learn.microsoft.com.'
---
#微软文档

微软技术生态系统的研究技能。涵盖learn.microsoft.com和它之外的文档（VS Code, GitHub, Aspire, Agent Framework repos）。

---

默认设置：Microsoft Learn MCP

将这些工具用于learn.microsoft.com**上的所有内容** - Azure，。NET、M365、Power Platform、Agent Framework、Semantic Kernel、Windows等等。这是绝大多数Microsoft文档查询的主要工具。

|工具|用途||------|---------|
搜索learn.microsoft.com -概念，指南，教程，配置|
|`microsoft_code_sample_search`|从学习文档找到工作代码片段。通过`language`（`python`，`csharp`等）获得最佳效果|
|`microsoft_docs_fetch`|从特定URL获取完整的页面内容（当搜索摘要不够时）|

当您需要完整的教程、所有配置选项或搜索摘要被截断时，在搜索后使用`microsoft_docs_fetch`。

### CLI选项

如果Learn MCP服务器不可用，请在终端或shell（例如Bash、PowerShell或cmd）中使用`mslearn`命令行：```bash
# Run directly (no install needed)
npx @microsoft/learn-cli search "BlobClient UploadAsync Azure.Storage.Blobs"

# Or install globally, then run
npm install -g @microsoft/learn-cli
mslearn search "BlobClient UploadAsync Azure.Storage.Blobs"
```
| MCP Tool | CLI命令||----------|-------------|
|`microsoft_docs_search(query: "...")`|`mslearn search "..."`|
|`microsoft_code_sample_search(query: "...", language: "...")`|`mslearn code-search "..." --language ...`|
|`microsoft_docs_fetch(url: "...")`|`mslearn fetch "..."`|

将`--json`传递给`search`或`code-search`以获得用于进一步处理的原始JSON输出。

---

例外：何时使用其他工具

下面的分类存在于** learn.microsoft.com之外。请使用指定的工具。

# # #。. NET Aspire -使用Aspire MCP服务器（首选）或Context7

Aspire的网站是** Aspire .dev**，而不是Learn **。最好的工具取决于你的Aspire CLI版本：

**CLI 13.2+**（推荐）- Aspire MCP服务器包括内置文档搜索工具：

| MCP工具|描述||----------|-------------|
|`list_docs`|列出来自aspire.dev |的所有可用文档
|`search_docs`|跨aspire.dev内容的加权词法搜索|
|`get_doc`|通过段代码|检索特定文档

这些在Aspire CLI 13.2 （[PR #14028](https://github.com/dotnet/aspire/pull/14028)）中发布。更新：`aspire update --self --channel daily`。裁判:https://davidpine.dev/posts/aspire-docs-mcp-tools/**CLI 13.1** - MCP服务器提供集成查找（`list_integrations`,`get_integration_docs`），但**不**文档搜索。回到上下文7：

|库ID |用于||---|---|
|`/microsoft/aspire.dev`|初级指南，集成，CLI参考，部署|
|`/dotnet/aspire`|运行时源代码- API内部，实现细节|
社区集成- Go， Java,Node.js, Ollama |

###VS Code-使用Context7VS Code文档位于**code.visualstudio.com**，而不是Learn。

|库ID |用于||---|---|
|`/websites/code_visualstudio`|用户文档-设置，功能，调试，远程开发|
|`/websites/code_visualstudio_api`|扩展API - webviews， TreeViews，命令，贡献点|

### GitHub -使用Context7

GitHub文档位于**docs.github.com**和**cli.github.com**。

|库ID |用于||---|---|
|`/websites/github_en`|动作，API，回购，安全，管理，副驾驶|
|`/websites/cli_github`| GitHub CLI （`gh`）命令和标志|

Agent框架-使用Learn MCP + Context7

代理框架教程在learn.microsoft.com上（使用`microsoft_docs_search`），但**GitHub repo**有API级别的详细信息，通常领先于已发布的文档-特别是DevUI REST API参考，CLI选项和。网络集成。

|库ID |用于||---|---|
|`/websites/learn_microsoft_en-us_agent-framework`|教程- DevUI指南，跟踪，工作流编排|
|`/microsoft/agent-framework`| API细节- DevUI REST端点，CLI标志，验证，。. NET`AddDevUI`/`MapDevUI`|

**DevUI提示：**查询Learn网站源代码的操作指南，然后repo源代码api级别的细节（端点模式，代理配置，认证令牌）。

---

## Context7设置

对于任何Context7查询，首先解析库ID（每个会话一次）：

1. 用技术名称调用`mcp_context7_resolve-library-id`2. 使用返回的库ID和特定查询调用`mcp_context7_query-docs`---

编写有效的查询

具体——包括版本、意图和语言；```
# ❌ Too broad
"Azure Functions"
"agent framework"

# ✅ Specific
"Azure Functions Python v2 programming model"
"Cosmos DB partition key design best practices"
"GitHub Actions workflow_dispatch inputs matrix strategy"
"Aspire AddUvicornApp Python FastAPI integration"
"DevUI serve agents tracing OpenTelemetry directory discovery"
"Agent Framework workflow conditional edges branching handoff"
```
包括上下文:
- **版本**相关时（`.NET 8`,`Aspire 13`,`VS Code 1.96`）
- **任务意图** （`quickstart`,`tutorial`,`overview`,`limits`,`API reference`）
**语言**多语言文档（`Python`,`TypeScript`,`C#`）