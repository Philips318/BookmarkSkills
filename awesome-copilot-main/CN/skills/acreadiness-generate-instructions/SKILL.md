---
name: acreadiness-generate-instructions
description: 'Generate tailored AI agent instruction files via AgentRC instructions command. Produces .github/copilot-instructions.md (default, recommended for Copilot in VS Code) plus optional per-area .instructions.md files with applyTo globs for monorepos. Use after running /acreadiness-assess to close gaps in the AI Tooling pillar.'
argument-hint: "[--output .github/copilot-instructions.md|AGENTS.md] [--strategy flat|nested] [--areas | --area <name>] [--apply-to <glob>] [--claude-md] [--dry-run]"
---
# /acreadiness-generate-instructions -编写AI代理指令

当用户想要**创建**，**再生**或**刷新**他们的AI编码代理（副驾驶，克劳德等）的自定义指令时，使用此技能。这是AgentRC的**测量→生成→维持**循环中的**生成*步骤，也是**AI工具**支柱的最高杠杆动作。

##输出选项VS Code识别几种指令文件类型- AgentRC生成最常见的：

|文件|作用域|何时使用||---|---|---|
|`.github/copilot-instructions.md`|始终打开，整个工作空间| **默认** -VS Code副驾驶的本地指令文件|
|`AGENTS.md`|始终在线，整个工作空间|多代理备份（副驾驶+克劳德+其他人）|
|`.github/instructions/*.instructions.md`|的作用域为`applyTo`glob |在monorepos |中按区域/按语言规则
|`CLAUDE.md`|特定于线程的|通过`--claude-md`添加（仅嵌套）|

# #策略

- **`flat`** *（默认）* -单个`.github/copilot-instructions.md`在选择的路径。简单，容易复习。
**`nested`** - hub在`.github/copilot-instructions.md`+每个主题的详细文件在`.github/instructions/<topic>.instructions.md`，每个`applyTo`glob，所以VS Code只加载主题时，它是相关的。更适合大型或多堆栈的仓库。为什么是`.github/instructions/`而不是`.agents/`？** AgentRC的默认嵌套布局写入`.agents/`，这是*代理不可知* repos （Copilot + Claude + Cursor读取`AGENTS.md`）的正确位置。具体来说，对于VS CodeCopilot，原生位置是`.github/instructions/`和`applyTo`frontmatter——这是Copilot自动发现的。每当主输出为`.github/copilot-instructions.md`时，此技能将AgentRC的嵌套输出重写到VS Code-native位置。如果您选择`--output AGENTS.md`，则嵌套将保留AgentRC的默认`.agents/`布局。

对于单内核，使用`--areas`、`--area <name>`或`--areas-only`生成**区域范围的**指令。面积用`agentrc.config.json`定义。每个区域的输出写入为带有`applyTo`glob的VS Code`.instructions.md`文件（见下文）。

主题vs区域`.instructions.md`文件

它们都以`.github/instructions/`结尾，但它们回答了不同的问题：

|类型|文件名示例|`applyTo`示例| |的来源|---|---|---|---|
| **主题**（嵌套）|`testing.instructions.md`|`**/*.{test,spec}.{ts,tsx,js}`| AgentRC`--strategy nested`主题分割|
| **面积**（单点）|`frontend.instructions.md`|`apps/frontend/**`|`agentrc.config.json`面积+`--areas`|

您可以同时拥有这两种文件：一组嵌套的主题文件加上单个线程的每个区域文件。

##按区域文件`applyTo`当用户选择进入区域时，在`.github/instructions/<area>.instructions.md`上为每个区域发出一个VS Code本地`.instructions.md`文件。每个文件必须以frontmatter开头，声明规则适用的全局：```markdown
---
applyTo: "apps/frontend/**"
---

# Frontend area instructions

…AgentRC-generated content for this area…
```
工作流程:

1. **读取`agentrc.config.json`**以发现声明的区域及其`paths`/ globs。如果缺少`paths`，请向用户请求全局变量（例如`src/api/**`）。
2. **运行`agentrc instructions --areas`**（或`--area <name>`）生成每个区域的正文内容。
3. **用取自该区域`paths`的`applyTo`正面内容包裹`.github/instructions/<area>.instructions.md`中的每个区域的内容**。如果用户在单区域调用中传递了`--apply-to <glob>`，则逐字使用该全局变量。
4. **保留主文件** -根`.github/copilot-instructions.md`保持为始终打开的指令；`.instructions.md`文件仅用于匹配路径。

命名：小写，串式区域名称。示例：`.github/instructions/frontend.instructions.md`、`.github/instructions/api.instructions.md`、`.github/instructions/infra.instructions.md`。

# #的步骤1. 选择目标文件**。**默认为`.github/copilot-instructions.md`。**切换到`AGENTS.md`只有当用户提到多代理/克劳德/光标支持。
2. **始终询问使用哪种策略** -`flat`或`nested`-除非用户已经在消息中指定了一个或通过`--strategy`。简要介绍这种权衡：
- **平面** *（默认）* -一个`.github/copilot-instructions.md`。简单，易于在单个PR中进行审查。最适合small/mediumrepos与一个堆栈。
-嵌套的** - hub`.github/copilot-instructions.md`+每个主题`.github/instructions/<topic>.instructions.md`文件（每个`applyTo`glob，所以VS Code只在相关时加载它们）。最适合大型或多堆栈的仓库。添加`--claude-md`也会发出`CLAUDE.md`。
当repo有bbb50个顶级目录、多个堆栈或已经使用单线程工具（turbo/nx/pnpm工作区）时，建议主动使用`nested`。
3. **通过读取`agentrc.config.json`检测单线区**。如果存在区域，询问用户是否需要每个区域`.instructions.md`fi **请在根文件之外加上`applyTo`**。当`agentrc.config.json`声明区域时，默认为“yes”。
4. **先运行干式运行**以便用户可以预览：   ```bash
   npx -y github:microsoft/agentrc instructions --output <file> --strategy <flat|nested> [--areas|--area <name>] [--claude-md] --dry-run
   ```
5. **显示一个简短的总结**什么会改变-文件将被创建或覆盖，面积计数+他们的`applyTo`globs，使用的模型（默认`claude-sonnet-4.6`）。
6. **在确认后，运行相同的命令，不带`--dry-run`**（如果文件已经存在，可以选择`--force`）。
7. **副驾驶输出后处理布局**：
- **如果`--output`以`copilot-instructions.md`结尾，策略为`nested`**:move/rewriteAgentRC的`.agents/<topic>.md`文件到`.github/instructions/<topic>.instructions.md`。使用适当的`applyTo`glob向每个文件添加标题（请参阅下面的“Topic applyTo defaults”）。删除现在为空的`.agents/`目录。
**如果`--areas`被使用**：也写`.github/instructions/<area>.instructions.md`为每个区域，使用每个区域的`paths`从`agentrc.config.json`作为`applyTo`glob（覆盖`--apply-to`为单区域调用）。
- **如果选择`--output AGENTS.md`**：保留AgentRC的原生`.agents/`布局，以便嵌套代理不可知的读者期望它在那里。
创建`.github/instructions/`目录失踪。### Topic`applyTo`defaults

当将AgentRC的嵌套主题文件提升到`.instructions.md`时，除非用户另有指定，否则使用这些默认值：

|主题|默认值`applyTo`||---|---|
|`testing`|`**/*.{test,spec}.{ts,tsx,js,jsx,mjs,cjs}`|
|`style`/`code-quality`/`formatting`|`**/*.{ts,tsx,js,jsx,mjs,cjs,py,go,rs,java,kt,cs}`|
|`build`/`ci`|`**/{package.json,turbo.json,nx.json,.github/workflows/**}`|
|`docs`|`**/*.md`|
|`security`|`**`|
|任何其他/集线器级|`**`|
8. **验证**通过读取生成的文件(s)回来，并向用户显示一个1段的摘要：堆栈检测，捕获约定，长度，列表的`.instructions.md`文件与他们的globs。
9. **建议下一步**：
-重新运行`assess`技能，以确认AI工具支柱得分提高
-如果用户已经拥有`copilot-instructions.md`和`AGENTS.md`，建议合并到一个单一的事实来源（AgentRC将其标记为成熟度级别2+）。

# #笔记- AgentRC读取你的实际代码-没有模板。输出反映检测到的语言、框架和约定。
-`--claude-md`（仅限嵌套策略）也发出`CLAUDE.md`。
—当激活文件匹配`applyTo`时，VS Code自动应用`.instructions.md`文件。总是加载根`.github/copilot-instructions.md`。
不要在CI中非交互式地运行这个技能说明书是回购的一部分，应该通过公关发布。