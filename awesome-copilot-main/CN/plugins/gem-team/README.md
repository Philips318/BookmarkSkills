#宝石团队<p align="center">
  <img src="https://img.shields.io/badge/APM-mubaidr/gem--team-blue?style=flat-square" alt="APM package: mubaidr/gem-team">
  <img src="https://img.shields.io/github/v/release/mubaidr/gem-team?style=flat-square&color=important" alt="Latest release">
  <img src="https://img.shields.io/badge/license-Apache%202.0-green?style=flat-square" alt="Apache-2.0 license">
  <img src="https://img.shields.io/badge/PRs-welcome-brightgreen?style=flat-square" alt="Pull requests welcome">
</p>
将AI编码转变为一个精心策划的循环：计划、构建、审查、调试、学习——使用更智能的工具调用和更精简的上下文。

>规范驱动的多代理编排，用于软件开发、验证、调试、可重用知识和无上下文膨胀的执行。

* * TL;DR:** Gem Team安装了16个专家代理，将AI编码转化为工程过程。使用结构化波、依赖项解析、集成门和渐进式上下文管理来计划、实现和审查——同时避免上下文膨胀，通过输出卫生和发现深度缩放节省令牌，并通过模型路由和目标上下文快照提高工具调用精度。与副驾驶，克劳德代码，光标，OpenCode, Codex， Gemini CLI和Windsurf一起工作。

为什么是Gem团队？Gem Team用一个有纪律的工程交付系统包装你的AI：计划、构建、审查、调试、学习。下面的[Features]（# Features）部分详细介绍了每个功能。要点如下：

- **更好的交付流程**：规格驱动的执行，基于波的并行性，验证门，可恢复计划。
- **更好的代码质量**:16个专家代理，默认TDD，先诊断后修复，安全性和可访问性审计。
- **更好的上下文管理**：渐进式上下文信封，三层记忆，技能提取，PRD管理-上下文膨胀避免内置。
- **更好的成本控制**：模型路由，输出卫生，上下文修剪，发现深度缩放-更少的令牌，相同的结果。
- **更好的工具调用**：每个代理的目标上下文快照，输出卫生规则-精确而不会导致浪费。

##快速入门

首先安装[APM](https://microsoft.github.io/apm/)：```bash
# macOS / Linux
curl -sSL https://aka.ms/apm-unix | sh

# Windows PowerShell
irm https://aka.ms/apm-windows | iex

# Verify
apm --version
```
将Gem Team安装到当前项目中：```bash
apm install mubaidr/gem-team --target copilot,claude,cursor,opencode,codex,gemini,windsurf
```
或者只安装一个目标：```bash
apm install mubaidr/gem-team --target copilot
```
在第一次安装之后，提交属于repo的生成APM文件，特别是`apm.yml`、`apm.lock.yaml`，以及生成的线束目录，如`.github/`、`.claude/`、`.cursor/`、`.opencode/`、`.codex/`、`.gemini/`或`.windsurf/`。**不要**提交`apm_modules/`。

b> APM可以从现有的线束目录中自动检测目标，但是对于可预测的安装和新的存储库，建议使用显式的`--target`。

# #内容

为什么是Gem团队？) (# why-gem-team)
-[功能](#功能)
——(比较)(#比较)
-[核心概念]（# Core - Concepts）
——(工作流)(#工作流)
- [The Agent Team]（# The - Agent - Team）
——(安装)(#安装)
-[兼容工具]（# Compatible - Tools）
——(配置)(#配置)
-[操作说明]（#operation - Notes）
——(贡献)(#贡献)
——(许可证)(#许可证)
-[支持](#支持)

# #特性

智能工作流引擎-基于阶段的可预测管道：初始化→路由→计划→执行→输出。
- **复杂性自适应路由**：琐碎的任务得到一次委托。LOW获得内存规划。MEDIUM/HIGH获得持久计划、验证门和基于dag的波执行。
- **集成门**：审查者在继续之前检查波输出。中等风险等级；高闸门每波。
- **可恢复计划**：计划id，基于文件的工件和上下文信封使长任务暂停，检查和继续干净。

专家代理团队- **16个专注的代理**：规划师、研究员、实现者、实现者-移动、审查者、评论家、调试者、浏览器测试者、移动测试者、Devops、文档撰写者、设计师、移动设计师、代码简化者、技能创造者：以及协调他们所有人的协调者。
-默认的TDD：实现者遵循红-绿-重构和6类测试覆盖（快乐路径、不变量、边界、错误路径、输入变化、状态转换）。bug修复模式需要在接触代码之前进行调试器诊断。
- **诊断-修复**：调试器诊断→实现者修复→审核者重新验证。在计划者、编排者、实现者和审查者级别强制执行。

背景与知识管理- **上下文信封**：跨所有代理共享的渐进式缓存。技术堆栈、惯例、约束、架构快照、研究摘要、先前的决策：在每一波浪潮之后都有所丰富。
- **三层内存**:Repo（工作区范围），会话（会话范围），全局（用户范围）。置信度控制的持久性（≥0.85）。
- **稳定缓存**：高置信度事实（≥0.90，稳定，≥3次使用）提升为持久缓存。90天后未使用自动驱逐。
- **重用注释**：代理跳过重新验证的可信文件路径和模式。
- **技能提取**：高可信度的工作流通过gem-skill-creator成为可重用的`SKILL.md`剧本。
- **产品开发管理**：结构化产品需求，包括ear语法、验收标准、决策和变更历史。

质量和验证- **计划验证**：审查者检查计划正确性、时间悖论、波序和合约完整性。
- **评论家审查**：挑战假设，发现边缘情况，标记过度工程：对于高复杂性和影响架构的更改。
- **每波集成检查**：审查者在每波之后验证契约、冲突和集成点。
- **安全审计**:OWASP扫描，secrets/PII检测，移动8向量扫描（钥匙链，证书钉住，深度链接，生物识别认证，网络安全）。
- **可访问性审计**:WCAG 2.1 AA对比度检查，ARIA标签，焦点指示器，触摸目标，减少运动支持。
- **视觉回归**：屏幕截图比较与可配置的阈值。
- **可配置审计深度**:`none`、`basic`、`full`a11y扫描。

###🔧测试- **端到端浏览器测试**：基于流程的场景设置，断言，视觉证据，console/network捕获。
- **移动端到端测试**:iOS + Android与Detox， Maestro, Appium。手势测试，生命周期测试，推送通知，设备农场支持。
- **性能测试：冷启动TTI，内存分析，帧率分析，包大小跟踪。
- **平台特定测试**：安全区域，键盘行为，系统权限，黑暗模式，触觉，后退键，电池优化。

# # #设计- **UI/UX设计系统创建**：调色板，排版规模，间距，阴影，设计运动（野兽派，玻璃形态，极简主义，新野兽派，粘土形态，复古未来主义，最大化主义）。
- **移动平台设计**:iOS HIG， Android Material 3，安全区域，动态孤岛，触摸目标（44pt/48dp），平台选择模式。
- **无障碍优先**：对比度4.5:1，触摸目标，减少运动，语义HTML/ARIA.- **设计输出**:9节`DESIGN.md`带令牌，组件规格，响应行为，代理提示指导。

### DevOps & Deployment- **基础设施配置：Docker， Kubernetes, cloud （AWS/GCP/Azure）。
- **CI/CD管道管理**:PR→分段→排烟→生产流程。
- **审批门**：可配置每个环境的审批要求。
—**健康检查**：端点验证、资源监控、回滚策略（滚动、蓝绿色、金丝雀）。
- **移动部署**:EASBuild/Update， Fastlane, TestFlight， b谷歌Play分阶段推出。
- **幂等操作**：所有的操作都被设计成可以安全地重新运行。

成本控制

- **模型路由**：用于日常工作的廉价模型（实现者，文档）。强大的计划、调试、审查和评论模型。
- **输出卫生**：代理仅限于本地工具标志，管道截断，maxResults搜索。
- **上下文重用**：每个代理过滤信封（仅相关部分）。
- **预算控制**：研究员有`max_searches`，`max_files_to_read`，`max_depth`每个任务。学习和重用

- **坚持高自信的学习**：事实、模式、陷阱、失败模式、决策≥0.95的自信自动持续。
- **批授权**：产品决策→产品开发。技术决策→AGENTS.md/architecture文档。模式→memory/envelope.工作流→技能
- **Git检查点**：可选的波级提交集成门通过干净的审计跟踪和回滚诊断。

# #比较

gem-team并不打算取代Copilot、Cursor、Claude Code、Cline或Roo Code。

它关注的是缺失的工作流层：

-计划
-并行工作的subagent委派优先策略
-上下文信封，避免重复的源读取
-reviewer/debugger循环
-专业代理
-可重复执行工件

当你希望AI编码遵循工程流程而不是单一的聊天提示时，使用gem-team。自信，结构化的交付和持久的知识，而不是临时的一次性输出。

##核心概念

系统智商倍增器

Gem Team用一个有纪律的交付系统包装了您选择的模型：任务分类、计划、授权、验证、调试和学习。目标是提高代理软件工作的可靠性，而不依赖于单一的长提示。

知识层

|层|位置|用途|| :----------------- | :------------------------------- | :------------------------------------------------------------------------- |
| **PRD** |`docs/PRD.yaml`|产品要求和批准决策。|
| **AGENTS.md** |`AGENTS.md`|稳定项目约定、规则、代理说明。|
| **计划工件** |`docs/plan/{plan_id}/`|每任务计划、上下文信封、任务注册表、证据和结果。|
| **内存** |内存工具/配置后端|持久的事实、决策、陷阱、模式和故障模式。|
| **Skills** |`docs/skills/`|从成功的重复工作流中提取的可重用程序。|
| **派生文档** |`docs/knowledge/`|参考笔记、外部文档、摘要和研究成果。|

# #工作流程

架构流程

执行模型

Gem Team根据任务复杂性调整工作流深度：- **琐碎：**直接执行与一个小清单。
- **LOW:**轻量级内存规划和执行。
- **MEDIUM/HIGH:**持久规划、上下文包络、验证、波执行和集成评审。

系统对独立的工作进行批处理，只序列化真正的依赖，并为将来的运行保留高可信度的学习。```text
User Input
    ↓
Phase 0: Init & Clarify
    • Read provided context
    • Load config and relevant memory
    • Detect intent and plan state
    • Classify complexity
    • Ask only for blocking clarification
    ↓
Phase 1: Route
    • Continue existing plan
    • Revise existing plan
    • Start new task
    ↓
Phase 2: Plan
    • TRIVIAL → tiny checklist
    • LOW → lightweight in-memory plan
    • MEDIUM/HIGH → durable planner-generated plan
    • Analyze requirements for inconsistencies (MEDIUM/HIGH)
    • Validate higher-risk plans before execution
    ↓
Phase 3: Execute
    • Prepare context based on complexity
    • Run unblocked work in waves
    • Delegate tasks to suitable agents
    • Respect dependencies and conflicts
    • Review/integrate higher-risk waves
    ↓
Learn & Persist
    • Save reusable decisions, patterns, gotchas, and skills
    • Update memory, docs, PRD, AGENTS.md, or skills as appropriate
    ↓
Loop / Replan
    • Continue next wave
    • Replan if scope changes
    • Escalate if blocked
    ↓
Phase 4: Output
    • Present final status using configured output format
```
##特工团队

推荐的模型路由

使用快速高效的模型作为默认值，并为需要更深入分析的任务保留更强的推理模型。

|角色|样例型号|建议使用|| :-------------------------------------- | :------------------------------ | :--------------------------------------------------------------------------------------------- |
| **默认代理** |`mimoi-2.5/deepseek-v4-flash`|日常实现、文档、研究总结、简单检查。|
| **Planner, Debugger, Critic， Reviewer** |`mimoi-2.5-pro/deepseek-v4-pro`|计划，根本原因分析，合规性检查，关键审查，高风险验证。|

如果需要，用您自己的提供商提供的等效模型替换这些模型。

核心代理

|代理|描述|| :--------------- | :---------------------------------------------------------------------------------------------------------------------------------------------- |
| **ORCHESTRATOR** |协调工作流程，委派工作，跟踪计划，并执行验证门。运行0-4阶段管道。永远不要直接执行工作。|
| **研究员** |探索代码库模式、依赖、架构和文档。支持5种模式（扫描，深度，审计，跟踪，问题）与预算控制。|
| **PLANNER** |创建基于dag的执行计划，包括任务分解、波调度、依赖映射、风险分析和验收标准。|
| **IMPLEMENTER** |使用TDD（红-绿-重构）实现特性、修复和重构。错误修复模式需要调试器诊断。仅限外科编辑。|

质量和审查|代理|描述|| :------------------ | :--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
|审查实现质量、安全性、可维护性、契约和测试覆盖率。计划验证（lightweight/full）。波整合检查。OWASP +秘密+移动8向量安全扫描。可访问性审计（none/basic/full）。|
| **CRITIC** |审查产品开发需求的不一致性和模糊性。挑战假设，发现边缘情况，标记过度工程或错过的约束。评估分解、依赖关系、复杂性、耦合和未来验证。提供选择。|
| **DEBUGGER** |根本原因分析，堆栈跟踪诊断，回归平分，错误再现。当输入不足时要求澄清。证明模式（先进行复制测试）。从不实现修复。|
| **浏览器测试** | E2E浏览器检查，UI流验证，视觉回归(scr（快照比较），console/network捕获，a11y审计。可配置的阈值。|
| **CODE SIMPLIFIER** |删除死代码，降低圈复杂度，合并重复，改进命名。保留行为：在每次更改后运行测试。切斯特顿栅栏原理。|###专业代理

|代理|描述|| :--------------------- | :--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
|基础设施部署，CI/CD管道，容器管理（Docker/K8s）。产品运行状况检查、回滚的批准门（rolling/blue-green/canary）。移动部署（EAS, Fastlane,TestFlight/PlayStore）。|
|技术文档，readme， API文档，图表，演练。编写和维护珠三角。上下文信封更新。AGENTS.md管理。覆盖矩阵。|
|UI/UX布局，主题，配色方案，设计系统。Create/validate模式。设计运动（粗野主义、玻璃形态主义、极简主义等）。9段`DESIGN.md`输出。Wcag 2.1 aa。|
|面向React Native、Expo、Flutter的移动TDD。使用Platform.select编写特定于平台的代码。SafeAreaView, FlatList, Reanimated。bug修复模式。|
| MobileUI/UX适用于iOS （HIG）和Android （Material 3）。安全区，触摸目标（44pt/48dp），动态岛，平台特定规格。|
| **移动测试仪** |移动E2E与Detox， Maestro, Appium。iOS + Android。手势、生命周期、推送通知、设备场测试。性能（冷启动、内存、帧率）。|
| **SKILL CREATOR** |从高置信度（≥0.95，≥2次使用）模式中提取可重用的`SKILL.md`文件。创建脚本、引用和交叉链接的资产。|# #安装

# # # 1。安装APM```bash
# macOS / Linux
curl -sSL https://aka.ms/apm-unix | sh

# Windows PowerShell
irm https://aka.ms/apm-windows | iex

# Verify
apm --version
```
# # # 2。安装Gem团队

项目范围内的安装，建议团队使用：```bash
apm install mubaidr/gem-team --target copilot,claude,cursor,opencode,codex,gemini,windsurf
```
全局用户作用域安装，用于个人使用：```bash
apm install -g mubaidr/gem-team
```
为可复制的安装固定一个版本：```bash
apm install mubaidr/gem-team#v1.20.0 --target copilot
```
# # # 3。验证安装```bash
apm list
apm view mubaidr/gem-team
apm audit
```
工具相关的检查:```bash
copilot plugin list   # GitHub Copilot CLI, if used
/plugin list          # Claude Code, inside Claude Code
```
有用的APM标志```bash
# Preview without writing files
apm install mubaidr/gem-team --target copilot --dry-run

# Install only selected targets
apm install mubaidr/gem-team --target claude,cursor

# Install all supported harness targets
apm install mubaidr/gem-team --target all

# Exclude one target from auto-detection
apm install mubaidr/gem-team --exclude codex

# Reinstall from the existing apm.yml manifest
apm install
```
##兼容工具

APM根据所选择的目标和包中包含的原语写入不同的文件。

| APM target |工具/线束|典型输出|| :--------- | :----------------------------------- | :------------------------------------------------------------------------------------------------------ |
|`copilot`|VS Code副驾驶/GitHub CopilotCLI |`.github/agents/`、`.github/instructions/`、`.github/prompts/`、VS CodeMCP配置（如适用）。|
|`claude`| Claude Code |`.claude/agents/`,`.claude/rules/`，命令，技能，钩子，和MCP配置时适用。|
|`cursor`|游标|`.cursor/agents/`，`.cursor/rules/`，技能，命令，钩子，MCP配置（如果适用）。|
|`opencode`| OpenCode |`.opencode/agents/`，命令，技能，MCP，编译说明。|
|`codex`| Codex CLI |`.codex/agents/`、`AGENTS.md`和Codex config（适用时）。|
|`gemini`| Gemini CLI |`GEMINI.md`,skills/instructions（如果支持），Gemini配置（如果适用）。|
|`windsurf`|风帆/级联|`.windsurf/rules/`、技能、命令、钩子和MCP配置。|有些马具不支持每个原始。例如，并不是每个工具都有本机代理、钩子或项目范围的MCP。APM根据目标编译或跳过不支持的原语。

##市场安装

建议安装路径为APM。直接市场安装是可选的，并且需要此存储库为目标工具发布正确的市场元数据。

###GitHub CopilotCLI```bash
copilot plugin marketplace add mubaidr/gem-team
copilot plugin marketplace browse gem-team
copilot plugin install gem-team@gem-team
```
GitHub CopilotCLI还包括默认的市场，如`awesome-copilot`；如果Gem Team在那里发布了，那么安装它：```bash
copilot plugin install gem-team@awesome-copilot
```
克劳德代码```bash
/plugin marketplace add mubaidr/gem-team
/plugin
/plugin install gem-team@gem-team
/reload-plugins
```
##地方发展

克隆存储库并将其安装到测试项目中：```bash
git clone https://github.com/mubaidr/gem-team.git
cd gem-team
apm install . --target claude,cursor --dry-run
```
然后从本地路径运行一个真正的安装：```bash
apm install /absolute/path/to/gem-team --target claude,cursor
```
对于包编写和发布验证：```bash
apm audit
apm compile --target copilot,claude,cursor --validate
apm pack
```
# #配置

Gem Team可以在你的项目根目录下配置`.gem-team.yaml`。```yaml
orchestrator:
  max_concurrent_agents: 2
  default_complexity_threshold: auto # auto | TRIVIAL | LOW | MEDIUM | HIGH
  git_commit_on_gate_pass: true

planning:
  enable_critic_for: [HIGH]

quality:
  visual_regression_enabled: true
  visual_diff_threshold: 0.95
  a11y_audit_level: basic # none | basic | full

devops:
  approval_required_for: [production]
  auto_rollback_on_failure: false

testing:
  screenshot_on_failure: true
```
设置引用

# # # #协调器

|设置|类型|默认值|描述|| :------------------------------------------ | :----- | :------ | :----------------------------------------------------------------------- |
|`orchestrator.max_concurrent_agents`| number |`2`|最大并行代理执行数。|
|`orchestrator.default_complexity_threshold`| enum |`auto`|强制复杂度路由：`auto`、`TRIVIAL`、`LOW`、`MEDIUM`、`HIGH`。|
|`orchestrator.git_commit_on_gate_pass`| bool |`true`|集成闸通过时Git提交波输出。|

# # # #的计划

|设置|类型|默认值|描述|| :--------------------------- | :----- | :------- | :------------------------------------------------ |
|`planning.enable_critic_for`| enum[] |`[HIGH]`|需要评论家验证的复杂度级别。|

# # # #质量

|设置|类型|默认值|描述|| :---------------------------------- | :------ | :------ | :----------------------------------------------------- |
|`quality.visual_regression_enabled`|布尔值|`true`|启用截图比较检查。|
|`quality.visual_diff_threshold`|数字|`0.95`|从`0.0`到`1.0`的视觉比较阈值。|
|`quality.a11y_audit_level`| enum |`basic`|可访问性审计深度：`none`、`basic`或`full`。|

# # # # DevOps

|设置|类型|默认值|描述|| :-------------------------------- | :------ | :------------- | :------------------------------------------- |
|`devops.approval_required_for`| enum[] |`[production]`|需要显式审批的环境。|
|`devops.auto_rollback_on_failure`|布尔型|`false`|部署失败后尝试回退。|

# # # #测试

|设置|类型|默认值|描述|| :------------------------------ | :------ | :------ | :---------------------------------------------- |
|`testing.screenshot_on_failure`| boolean |`true`|browser/UI测试失败时截图。|

完整注释的默认文件可在[`.gem-team.yaml`]（.gem-team.yaml）处获得。

##操作说明

-更喜欢项目范围的团队安装，所以`apm.yml`和`apm.lock.yaml`使设置可复制。
-让`apm_modules/`远离git；这是一个安装缓存。
- Pin版本与`#vX.Y.Z`稳定的CI和团队入职。
—在发布前和CI中运行`apm audit`。
-在提交大型更新之前检查生成的文件。
-将DevOps、生产部署、数据迁移和破坏性操作视为批准的任务。
-保持项目规则在`AGENTS.md`；在`docs/plan/{plan_id}/`中保留特定于任务的上下文。

# #贡献

欢迎投稿。打开拉取请求前请先阅读[CONTRIBUTING.md]（./CONTRIBUTING.md）。

推荐的出资流程：1. 打开或选择一个问题。
2. 创建一个集中的分支。
3. 保持更改较小且可查看。
4. 添加或更新相关的tests/docs。
5. 在打开PR之前运行验证。

# #许可证

Gem Team在[Apache License 2.0]（./LICENSE）下获得许可。

# #支持

如果您遇到bug或有功能请求，请[打开问题]（https://github.com/mubaidr/gem-team/issues）。