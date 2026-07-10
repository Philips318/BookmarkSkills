---
name: aspire
description: 'Aspire skill covering the Aspire CLI, AppHost orchestration, service discovery, integrations, MCP server, VS Code extension, Dev Containers, GitHub Codespaces, templates, dashboard, and deployment. Use when the user asks to create, run, debug, configure, deploy, or troubleshoot an Aspire distributed application.'
---
# Aspire -多语言分布式应用程序编排

Aspire是一个代码优先、多语言的工具链，用于构建可观察的、生产就绪的分布式应用程序。它编排了来自单个AppHost项目的容器、可执行文件和云资源——无论工作负载是c#、Python、JavaScript/TypeScript、Go、Java、Rust、Bun、Deno还是PowerShell。

心智模型：** AppHost是一个*指挥*——它不演奏乐器，它告诉每个服务何时启动，如何找到彼此，并观察问题。

详细的参考资料在`references/`文件夹中-按需加载。

---

# #引用

|参考|何时加载||---|---|
| [CLI参考](references/cli-reference.md) |命令标志、选项或详细用法|
| [MCP服务器](references/mcp-server.md) |为AI助手设置MCP，可用工具|
|[集成目录](references/integrations-catalog.md) |通过MCP工具发现集成，连接模式|
|[多语言api](references/polyglot-apis.md) |方法签名，链接选项，特定于语言的模式|
|[架构](references/architecture.md) | DCP内部，资源模型，服务发现，联网，遥测|
| [Dashboard](references/dashboard.md) | Dashboard功能，独立模式，GenAI Visualizer |
|[部署](references/deployment.md) | Docker, Kubernetes， Azure容器应用，应用服务|
|[测试](references/testing.md) |针对AppHost |的集成测试
| [Troubleshooting](references/troubleshooting.md) |诊断代码，常见错误，修复|

---

# # 1。研究Aspire文档Aspire团队提供了一个MCP服务器，它可以直接在你的AI助手中提供文档工具。请参阅[MCP服务器]（references/mcp-server.md）了解安装细节。

Aspire CLI 13.2+（推荐-内置文档搜索）

如果运行的是Aspire CLI **13.2及以上版本** (`aspire --version`)， MCP服务器包含文档搜索工具：

|工具|描述||---|---|
|`list_docs`|列出来自aspire.dev |的所有可用文档
|`search_docs`|跨索引文档|执行加权词法搜索
|`get_doc`|通过其段代码|检索特定文档

这些工具是在[PR #14028]（https://github.com/dotnet/aspire/pull/14028）中添加的。更新：`aspire update --self --channel daily`。

有关这种方法的更多信息，请参阅David Pine的文章：https://davidpine.dev/posts/aspire-docs-mcp-tools/Aspire CLI 13.1（仅支持集成工具）

在13.1，MCP服务器提供集成查找，但**不**文档搜索：

|工具|描述||---|---|
|`list_integrations`|列出可用的Aspire托管集成|
|`get_integration_docs`|获取特定集成包|的文档

对于13.1的一般文档查询，使用**Context7**作为主要来源（见下文）。

###退一步：Context7

当Aspire MCP docs工具不可用（13.1）或MCP服务器未运行时，使用**Context7** (`mcp_context7`)：

**步骤1 -解析库ID**（每次会话一次）：

用`libraryName: ".NET Aspire"`调用`mcp_context7_resolve-library-id`。

| Rank |库ID | |时使用|---|---|---|
| 1 |`/microsoft/aspire.dev`|主源。指南、集成、CLI参考、部署。|
||`/dotnet/aspire`| API内部，源代码级实现细节。|`/communitytoolkit/aspire`|非微软多语言集成（Go, Java,Node.js, Ollama）。|

**步骤2 -查询文档：**```
libraryId: "/microsoft/aspire.dev", query: "Python integration AddPythonApp service discovery"
libraryId: "/communitytoolkit/aspire", query: "Golang Java Node.js community integrations"
```
###后退：GitHub搜索（当Context7也不可用时）

在GitHub上搜索官方文档：
- **Docs repo:**`microsoft/aspire.dev`- path:`src/frontend/src/content/docs/`- **源代码：**`dotnet/aspire`- **样本库：**`dotnet/aspire-samples`- **社区整合：**`CommunityToolkit/Aspire`---

# # 2。前提条件与安装

|需求|详细信息||---|---|
| * *。. NET SDK** | 10.0+(即使非。. NET工作负载- AppHost是。净)|
| **容器运行时** | Docker Desktop、Podman或Rancher Desktop |
| **IDE（可选）** |VS Code+ c#开发工具包，Visual Studio 2022, JetBrains Rider |```bash
# Linux / macOS
curl -sSL https://aspire.dev/install.sh | bash

# Windows PowerShell
irm https://aspire.dev/install.ps1 | iex

# Verify
aspire --version

# Install templates
dotnet new install Aspire.ProjectTemplates
```
---

# # 3。项目模板

|模板|命令|描述||---|---|---|
| **aspire-starter** |`aspire new aspire-starter`| ASP。. NETCore/Blazorstarter + AppHost + tests |
** |`aspire new aspire-ts-cs-starter`| ASP. NETCore/Reactstarter + AppHost |
| **aspire-py-starter** |`aspire new aspire-py-starter`|FastAPI/Reactstarter + AppHost |
| **aspire-apphost-singlefile** |`aspire new aspire-apphost-singlefile`|空单文件AppHost |

---

# # 4。AppHost快速入门（多语言）

AppHost编排所有服务。非。. NET工作负载作为容器或可执行文件运行。```csharp
var builder = DistributedApplication.CreateBuilder(args);

// Infrastructure
var redis = builder.AddRedis("cache");
var postgres = builder.AddPostgres("pg").AddDatabase("catalog");

// .NET API
var api = builder.AddProject<Projects.CatalogApi>("api")
    .WithReference(postgres).WithReference(redis);

// Python ML service
var ml = builder.AddPythonApp("ml-service", "../ml-service", "main.py")
    .WithHttpEndpoint(targetPort: 8000).WithReference(redis);

// React frontend (Vite)
var web = builder.AddViteApp("web", "../frontend")
    .WithHttpEndpoint(targetPort: 5173).WithReference(api);

// Go worker
var worker = builder.AddGolangApp("worker", "../go-worker")
    .WithReference(redis);

builder.Build().Run();
```
有关完整的API签名，请参见[Polyglot API]（references/polyglot-apis.md）。

---

# # 5。核心概念（摘要）

|概念|重点||---|---|
| **Run vs Publish** |`aspire run`= local dev （DCP引擎）。`aspire publish`=生成部署清单。|
| **服务发现** |自动通过环境变量：`ConnectionStrings__<name>`，`services__<name>__http__0`|
| **资源生命周期** | DAG排序-依赖关系首先开始。`.WaitFor()`健康检查门。|
| **资源类型** |`ProjectResource`、`ContainerResource`、`ExecutableResource`、`ParameterResource`|
| **集成** | 144+跨越13个类别。主机包(AppHost) +客户端包（服务）。|
| **仪表板** |实时日志，跟踪，指标，GenAI可视化。使用`aspire run`自动运行。|
| **MCP服务器** |人工智能助手可以通过CLI （STDIO）查询正在运行的应用程序和搜索文档。|
| **测试** |`Aspire.Hosting.Testing`-旋转完整的AppHost在xUnit/MSTest/NUnit.|
| Docker, Kubernetes， Azure容器应用，Azure应用服务。|

---

# # 6。CLI快速参考

Aspire CLI 13.1中的有效命令：

|命令|描述|状态||---|---|---|
|`aspire new <template>`|从模板|创建稳定|
|`aspire init`|在现有项目|中初始化，稳定|
|`aspire run`|本地启动所有资源|稳定|
|`aspire add <integration>`|添加集成| Stable |
|`aspire publish`|生成部署清单|预览|
|`aspire config`|管理配置设置|稳定|
|`aspire cache`|管理磁盘缓存|稳定|
|`aspire deploy`|部署到定义的目标|预览|
|`aspire do <step>`|执行管道步骤|预览|
|`aspire update`|更新集成（或`--self`为CLI） |预览|
|`aspire mcp init`|配置AI助手MCP | Stable |
|`aspire mcp start`|启动MCP服务器| Stable |

带标志的完整命令参考：[CLI参考]（references/cli-reference.md）。

---

# # 7。常见的模式

添加新服务

1. 创建您的服务目录（任何语言）
2. 添加到AppHost:`Add*App()`或`AddProject<T>()`3. 连线依赖关系：`.WithReference()`4. 健康门：如果需要，`.WaitFor()`5. 运行:`aspire run`###从Docker Compose迁移

1.`aspire new aspire-apphost-singlefile`（空AppHost）
2. 用Aspire资源替换每个`docker-compose`服务
3.`depends_on`→`.WithReference()`+`.WaitFor()`4.`ports`→`.WithHttpEndpoint()`5.`environment`→`.WithEnvironment()`或`.WithReference()`---

# # 8。关键的url

|资源| URL ||---|---|
| **文档** |https://aspire.dev|
| **运行时repo** |https://github.com/dotnet/aspire|
| **文件回购** |https://github.com/microsoft/aspire.dev|
| **Samples** |https://github.com/dotnet/aspire-samples|
| **社区工具包** |https://github.com/CommunityToolkit/Aspire|
| **仪表盘图像** |`mcr.microsoft.com/dotnet/aspire-dashboard`|
| **不和谐** |https://aka.ms/aspire/discord|
| **Reddit** |https://www.reddit.com/r/aspiredotdev/|