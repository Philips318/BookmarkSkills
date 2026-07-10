---
name: microsoft-skill-creator
description: Create agent skills for Microsoft technologies using Learn MCP tools. Use when users want to create a skill that teaches agents about any Microsoft technology, library, framework, or service (Azure, .NET, M365, VS Code, Bicep, etc.). Investigates topics deeply, then generates a hybrid skill storing essential knowledge locally while enabling dynamic deeper investigation.
context: fork
compatibility: Works best with Microsoft Learn MCP Server (https://learn.microsoft.com/api/mcp). Can also use the mslearn CLI as a fallback.
---
# Microsoft Skill Creator

为Microsoft技术创建混合技能，将基本知识存储在本地，同时支持动态Learn MCP查找以获取更深入的细节。

##关于技能

技能是模块化的包，可以用专门的知识和工作流扩展代理的功能。技能将通用代理转换为特定领域的专用代理。

技能结构```
skill-name/
├── SKILL.md (required)     # Frontmatter (name, description) + instructions
├── references/             # Documentation loaded into context as needed
├── sample_codes/           # Working code examples
└── assets/                 # Files used in output (templates, etc.)
```
关键原则

- **前奏至关重要**:`name`和`description`决定技能何时触发-要清晰和全面
- **简洁是关键**：只包括代理还不知道的内容；共享上下文窗口
- **没有重复**：信息存在于SKILL.md或参考文件中，而不是两者都存在

学习MCP工具

|工具|用途|何时使用||------|---------|-------------|
|`microsoft_docs_search`|搜索官方文档|首先通过发现，找到主题|
|`microsoft_docs_fetch`|获得完整的页面内容|深入到重要页面|
|`microsoft_code_sample_search`|查找代码示例|获取实现模式|

### CLI选项

如果没有可用的Learn MCP服务器，可以在终端或shell（例如Bash、PowerShell或cmd）中使用`mslearn`命令行：```bash
# Run directly (no install needed)
npx @microsoft/learn-cli search "semantic kernel overview"

# Or install globally, then run
npm install -g @microsoft/learn-cli
mslearn search "semantic kernel overview"
```
| MCP Tool | CLI命令||----------|-------------|
|`microsoft_docs_search(query: "...")`|`mslearn search "..."`|
|`microsoft_code_sample_search(query: "...", language: "...")`|`mslearn code-search "..." --language ...`|
|`microsoft_docs_fetch(url: "...")`|`mslearn fetch "..."`|

生成的技能应该包含相同的CLI回退表，以便代理可以使用任何路径。

创建过程

第一步：调查主题

使用Learn MCP工具分三个阶段建立深刻的理解：

第一阶段-范围发现：**```
microsoft_docs_search(query="{technology} overview what is")
microsoft_docs_search(query="{technology} concepts architecture")
microsoft_docs_search(query="{technology} getting started tutorial")
```
**第二阶段-核心内容：**```
microsoft_docs_fetch(url="...")  # Fetch pages from Phase 1
microsoft_code_sample_search(query="{technology}", language="{lang}")
```
**第三阶段-深度：**```
microsoft_docs_search(query="{technology} best practices")
microsoft_docs_search(query="{technology} troubleshooting errors")
```
####调查清单

调查后，核实：
-[]可以用一段话解释这项技术的作用
-[]确定3-5个关键概念
-[]有基本使用的工作代码
-[]了解最常见的API模式
-[]有更深层次的主题搜索查询

步骤2：与用户澄清

提出调查结果并问：
1. “我发现了这些关键领域：[列表]。哪些是最重要的？”
2. “使用这种技能，代理主要执行哪些任务？”
3. “代码样本应该优先考虑哪种编程语言？”

步骤3：生成技能

使用[skill-templates.md]（references/skill-templates.md）中的适当模板：

|技术类型|模板||-----------------|----------|
|客户端库，NuGet/npm包|SDK/Library|
| Azure资源| Azure服务|
|应用开发框架|Framework/Platform|
| REST接口，协议|API/Protocol|

####生成技能结构```
{skill-name}/
├── SKILL.md                    # Core knowledge + Learn MCP guidance
├── references/                 # Detailed local documentation (if needed)
└── sample_codes/               # Working code examples
    ├── getting-started/
    └── common-patterns/
```
步骤4：平衡本地内容和动态内容

**本地存储：**
-基础（任何任务都需要）
-频繁访问
稳定（不会改变）
-很难通过搜索找到

**保持动态：**
-详尽的参考资料（太大）
——特定于版本的
-情境（仅限特定任务）
-索引良好（易于搜索）

####内容指南

|内容类型|本地|动态||--------------|-------|---------|
|核心概念（3-5）|✅完整| |
| Hello world code |✅Full | |
|常见模式（3-5）|✅满| |
|顶级API方法|签名+示例|完整文档通过fetch |
|最佳实践|前5个项目|搜索更多|
| |故障处理|查询|
|完整API参考| |文档链接|

###步骤5：验证

1. 检讨：本地内容是否足以应付一般任务？
2. 测试：建议的搜索查询是否返回有用的结果？
3. 验证：代码示例运行时没有错误吗？

##常见调查模式

###为SDKs/Libraries```
"{name} overview" → purpose, architecture
"{name} getting started quickstart" → setup steps
"{name} API reference" → core classes/methods
"{name} samples examples" → code patterns
"{name} best practices performance" → optimization
```
对于Azure服务```
"{service} overview features" → capabilities
"{service} quickstart {language}" → setup code
"{service} REST API reference" → endpoints
"{service} SDK {language}" → client library
"{service} pricing limits quotas" → constraints
```
###为Frameworks/Platforms```
"{framework} architecture concepts" → mental model
"{framework} project structure" → conventions
"{framework} tutorial walkthrough" → end-to-end flow
"{framework} configuration options" → customization
```
例子：创造一个“语义核心”技能

# # #调查```
microsoft_docs_search(query="semantic kernel overview")
microsoft_docs_search(query="semantic kernel plugins functions")
microsoft_code_sample_search(query="semantic kernel", language="csharp")
microsoft_docs_fetch(url="https://learn.microsoft.com/semantic-kernel/overview/")
```
生成技能```
semantic-kernel/
├── SKILL.md
└── sample_codes/
    ├── getting-started/
    │   └── hello-kernel.cs
    └── common-patterns/
        ├── chat-completion.cs
        └── function-calling.cs
```
生成SKILL.md```markdown
---
name: semantic-kernel
description: Build AI agents with Microsoft Semantic Kernel. Use for LLM-powered apps with plugins, planners, and memory in .NET or Python.
---

# Semantic Kernel

Orchestration SDK for integrating LLMs into applications with plugins, planners, and memory.

## Key Concepts

- **Kernel**: Central orchestrator managing AI services and plugins
- **Plugins**: Collections of functions the AI can call
- **Planner**: Sequences plugin functions to achieve goals
- **Memory**: Vector store integration for RAG patterns

## Quick Start

See [getting-started/hello-kernel.cs](sample_codes/getting-started/hello-kernel.cs)

## Learn More

| Topic | How to Find |
|-------|-------------|
| Plugin development | `microsoft_docs_search(query="semantic kernel plugins custom functions")` |
| Planners | `microsoft_docs_search(query="semantic kernel planner")` |
| Memory | `microsoft_docs_fetch(url="https://learn.microsoft.com/en-us/semantic-kernel/frameworks/agent/agent-memory")` |

## CLI Alternative

If the Learn MCP server is not available, use the `mslearn` CLI instead:

| MCP Tool | CLI Command |
|----------|-------------|
| `microsoft_docs_search(query: "...")` | `mslearn search "..."` |
| `microsoft_code_sample_search(query: "...", language: "...")` | `mslearn code-search "..." --language ...` |
| `microsoft_docs_fetch(url: "...")` | `mslearn fetch "..."` |

Run directly with `npx @microsoft/learn-cli <command>` or install globally with `npm install -g @microsoft/learn-cli`.
```
