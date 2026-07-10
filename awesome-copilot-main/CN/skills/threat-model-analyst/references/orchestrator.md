# Orchestrator -威胁模型分析工作流

此文件包含用于执行威胁模型分析的完整编排逻辑。
它是`/threat-model-analyst`技能的主要工作流文档。

##⚡背景预算-选择性读取文件

**不要在会话开始时读取所有10个技能文件。**只阅读每个阶段需要的内容。这为实际的代码库分析保留了上下文窗口。**阶段1（上下文收集）：**读取这个文件(`orchestrator.md`) +`analysis-principles.md`+`tmt-element-taxonomy.md`**第二阶段（写报告）：**在写每个文件之前，从`skeletons/`中读取相关的骨架。阅读`output-formats.md`+`diagram-conventions.md`获取规则—但是使用骨架作为结构模板。
—“`0.1-architecture.md`”前写为“`skeletons/skeleton-architecture.md`”
—“`1.1-threatmodel.mmd`”前写为“`skeletons/skeleton-dfd.md`”
—“`1-threatmodel.md`”前写为“`skeletons/skeleton-threatmodel.md`”
—“`2-stride-analysis.md`”前写为“`skeletons/skeleton-stride-analysis.md`”
—“`3-findings.md`”前写为“`skeletons/skeleton-findings.md`”
—“`0-assessment.md`”前写为“`skeletons/skeleton-assessment.md`”
—“`threat-inventory.json`”前写为“`skeletons/skeleton-inventory.md`”
—“`incremental-comparison.html`”前写为“`skeletons/skeleton-incremental-html.md`”
**阶段3（验证）：**委托给子代理，并在子代理提示符中包含`verification-checklist.md`。子代理用一个新的上下文窗口读取完整的检查表——父代理不需要读取它。

**关键原理：**子代理获得新的上下文窗口。将验证和JSON生成委托给子代理，而不是将所有内容保留在父上下文中。---

##✅强制性规则-开始前阅读

这些是每个威胁模型报告所需的行为。严格遵守每条规则：1. 按**可利用性层** （1/2/3层）组织发现，而不是按严重性级别
2. 将每个组件的STRIDE表拆分为Tier 1、Tier 2、Tier 3子部分
3. 在每个发现中包含`Exploitability Tier`和`Remediation Effort`-两者都是必选的
4. STRIDE汇总表必须包含T1， T2， T3列
4 b。**STRIDE +滥用案例的确切类别是：** **S**poofing， **T**篡改，**R**认证，**I**信息泄露，**D**提供服务，**E**提升特权，**A**滥用（业务逻辑滥用，工作流操纵，功能滥用-标准STRIDE的扩展，涵盖合法功能的滥用）。A总是“滥用”，而不是“AI安全”、“授权”或任何其他解释。授权问题属于E（提升特权）。
5.`.md`文件：从第1行中的`# Heading`开始。`create_file`工具编写原始内容—没有代码栅栏
6.`.mmd`文件：*在第一行用`%%{init:`表示。原始美人鱼的来源，没有围栏
7. Section必须精确地命名为`## Action Summary`。包括`### Quick Wins`小节和Tier 1低工作量发现表
8. K8s侧车：用`<br/>+ Sidecar`注释主机容器-永远不要创建单独的侧车节点（参见`diagram-conventions.md`规则1）
9. pod内部的localhost流是隐式的-不要在图表中绘制它们
10. 行动总结是建议-没有单独的`### Key Recommendations`部分
11. 在执行摘要中包含`> **Note on threat counts:**`blockquote
12. 每个发现必须有CVSS 4.0（分数和全矢量字符串），CWE（带超链接）和OWASP （`:2025`后缀）
13. OWASP后缀总是`:2025`（例如，`A01:2025 – Broken Access Control`）
14. 在`3-findings.md`的末尾包含威胁覆盖验证表，映射每个威胁→查找
15.`0.1-architecture.md`中的每个组件必须出现在`2-stride-analysis.md`中
16.`0.1-architecture.md`中的前3个场景必须有美人鱼序列图
17.`0-assessment.md`必须包含E`## Analysis Context & Assumptions`与`### Needs Verification`和`### Finding Overrides`表
18.`### Quick Wins`子节必须在动作摘要下（如果没有，包括标题和注释）
19.`0-assessment.md`中的所有7个部分都是强制性的：报告文件，执行摘要，行动摘要，分析背景和假设，参考文献，报告元数据，分类参考
20.。**部署分类为BINDING。**在`0.1-architecture.md`中，设置`Deployment Classification`并填写组件暴露表。如果分类为`LOCALHOST_DESKTOP`或`LOCALHOST_SERVICE`: T1结果为零，`Prerequisites = None`为零，非侦听器组件为零。请参见`analysis-principles.md`部署上下文表。
21. 查找id必须从上到下顺序：FIND-01， FIND-02, FIND-03…排序后重新编号
22. CWE必须包含超链接：`[CWE-306](https://cwe.mitre.org/data/definitions/306.html): Missing Authentication`23. 在STRIDE之后，在`analysis-principles.md`中运行特定于技术的安全检查表。回购中的每项技术都需要至少一个发现或记录的缓解措施24. CVSS`AV:L`或`PR:H`→查找不能是一级。降级到T2/T3.参见`analysis-principles.md`中的CVSS-to-Tier一致性检查
25. 只使用`Low`/`Medium`/`High`工作标签。永远不要生成时间估计、冲刺阶段或日程安排。参见`output-formats.md`禁止内容
26. 参考资料：使用`output-formats.md`-`### Security Standards`（带有完整url的3列表）和`### Component Documentation`（带有url的3列表）中的精确的两小节格式
27. 报表元数据：包括`output-formats.md`模板中的所有字段—模型、已开始分析、已完成分析、持续时间。在第1步运行`Get-Date -Format "yyyy-MM-dd HH:mm:ss" -AsUTC`，然后再写入`0-assessment.md`28.`2-stride-analysis.md`中的`## Summary`表必须出现在顶部，紧接在`## Exploitability Tiers`之后，在各个组件段之前
29. 相关威胁：每个威胁ID必须是指向`2-stride-analysis.md#component-anchor`的超链接。格式:`[T02.S](2-stride-analysis.md#component-name)`30.。图表颜色：从`diagram-conventions.md`逐字复制classDef lines。只允许填充：`#6baed6`（进程）、`#fdae61`（外部）、`#74c476`（数据存储）。只允许笔画：`#2171b5`，`#d94701`,`#238b45`,`#e31a1c`。只使用`%%{init: {'theme': 'base', 'themeVariables': { 'background': '#ffffff', 'primaryColor': '#ffffff', 'lineColor': '#666666' }}}%%`-不使用其他的themeVariables键
31. DFD：创建`1.1-threatmodel.mmd`后，运行步骤4中的POST-DFD GATE。gate和`skeleton-summary-dfd.md`控制是否生成`1.2-threatmodel-summary.mmd`。
32.`0-assessment.md`中的Report Files表：将`0-assessment.md`（本文档）列为第一行，然后是0.1-architecture.md、1-threatmodel.md等。使用来自`output-formats.md`的精确模板
33.`threat-inventory.json`必须在每次分析运行时生成（步骤8b）。该文件允许将来进行比较。有关模式，请参阅`output-formats.md`。
34. **永远不要删除、修改或删除存储库中任何现有的`threat-model-*`或`threat-model-compare-*`文件夹**。只写入您自己的带时间戳的输出文件夹。允许清理你创建的临时git工作树；禁止删除其他报表文件夹。规则优先级（当指导冲突时）

按以下顺序应用规则：
1.`skeletons/skeleton-*.md`中的文字骨架-精确的section/table标题和属性行
2.`orchestrator.md`中的强制规则（此列表）
3.`output-formats.md`中的示例（示例是说明性的，如果它们与字面骨架不同，则不具有权威性）

如果检测到任何冲突，则遵循优先级最高的项。

**生成后：**验证子代理将扫描`verification-checklist.md`阶段0中列出的所有已知偏差的输出。在完成之前修复任何失败。

---

# #工作流程

**排除：**跳过这些目录：
-`threat-model-*`（以前的报告）
-`node_modules`,`.git`,`dist`,`build`,`vendor`,`__pycache__`**前期工作：**在写入任何输出文件之前，扫描`verification-checklist.md`阶段1（逐文件结构检查）和阶段2（图表渲染检查）。这使质量门内部化，因此在第一次通过时输出是正确的-防止昂贵的返工。不要运行完整的验证；这发生在步骤10。

⛔子代理治理（强制-防止重复工作）

子代理是独立的执行上下文——它们没有父代理的状态、指令或其他子代理的内存。如果没有严格的治理，子代理将独立执行整个分析，创建重复的报告文件夹，每个副本浪费约15分钟的计算时间+约100K令牌。

**规则1 -父节点拥有所有文件创建权。**父座席是唯一呼叫`create_file`的座席（0.1-architecture.md、stride-analysis.md、findings.md等）。子代理从不编写报告文件。**规则2 -子代理是只读助手。**子代理可以：
-搜索特定模式的源代码（例如，“查找所有auth相关代码”）
—读取并分析文件，然后将结构化数据返回给父节点
—执行校验检查，返回结果为PASS/FAIL—执行终端命令（git diff, grep）并返回输出**规则3 -子代理提示必须是NARROW和SPECIFIC。**永远不要告诉子代理“执行威胁模型分析”或“生成报告”。而不是:
-✅“阅读这5个Go文件并列出处理凭据的每个函数。返回一个包含函数名，文件，行号的表格。
-✅"对{folder}中的文件执行校验清单。每次检查返回PASS/FAIL。”
-✅“从{path}读取threat-inventory.json并验证所有数组长度匹配度量。回报不匹配。”
-❌“分析此代码库并编写威胁模型文件。”
-❌“为该组件生成0.1-architecture.md和stride-analysis.md”

**规则4 -输出文件夹路径。**父程序在步骤1中创建带有时间戳的输出文件夹，并为所有`create_file`调用使用该确切路径。如果子代理需要读取以前编写的报告文件，则在子代理提示符中传递文件夹路径。**规则5 -唯一的例外是`threat-inventory.json`生成（步骤8b），如果数据太大，父代理可以将JSON写入委托给子代理。在这种情况下，子代理提示符必须包括：(a)准确的输出文件路径，(b)要序列化的数据，以及(c)明确的指令：“只写这一个文件。”请勿创建任何其他文件或文件夹。”

# # #的步骤

1. **记录开始时间并收集上下文**
—执行命令`Get-Date -Format "yyyy-MM-dd HH:mm:ss" -AsUTC`，保存为“`START_TIME`”
-获取git信息：`git remote get-url origin`，`git branch --show-current`,`git rev-parse --short HEAD`,`git log -1 --format="%ai" HEAD`（提交日期-不是今天的日期），`hostname`-映射系统：识别组件，信任边界，数据流
- **参考：**`analysis-principles.md`为安全基础设施清单**⛔部署分类（必选-在分析威胁代码之前执行此操作）：**
根据代码证据确定系统的部署类（有关值，请参阅`skeleton-architecture.md`）。
记录在`0.1-architecture.md`→部署模型一节中。然后填写组件公开表-每个组件一行显示监听地址、认证屏障、外部可达性和最低先决条件。
这个表是先决条件的唯一真实来源。任何威胁或发现的先决条件都不可能低于其成分的暴露表所允许的条件。

**⛔确定性命名-在写入任何文件之前应用：**

在标识组件时，为每个组件分配一个规范的PascalCase`id`。命名必须是确定的——在同一代码库上的两次独立运行必须产生相同的组件id。**⛔绝对规则：每个组件ID必须锚定到一个真正的代码工件
对于您标识的每个组件，您必须能够指向代码库中作为该组件“锚”的特定类、文件或清单。如果不存在这样的工件，则该组件不存在。

**命名程序（按照顺序-在第一场比赛停止）：**
1. **主类名** -使用源代码中的EXACT类名。不要缩写、扩展或改写它。      - `TaskProcessor.cs` → `TaskProcessor` (NOT `TaskServer`, NOT `TaskService`)
      - `SessionStore.cs` → `SessionStore` (NOT `FileSessionStore`, NOT `SessionService`)
      - `TerminalUserInterface.cs` → `TerminalUserInterface` (NOT `TerminalUI`)
      - `PowerShellCommandExecutor.cs` → `PowerShellCommandExecutor` (NOT `PowerShellExecutor`)
      - `ResponsesAPIService.cs` → `ResponsesAPIService` (NOT `LLMService` — that's a DIFFERENT class)
      - `MCPHost.cs` → `MCPHost` (NOT `OrchestrationHost`)
2. **主脚本名称**→`Import-Images.ps1`→`ImportImages`3. **主config/manifest名称**→`Dockerfile`→`DockerContainer`，`values.yaml`→`HelmChart`4. **目录名称**（如果组件跨越多个文件）→`src/ParquetParsing/`→`ParquetParser`5. **技术名称**（用于外部services/datastores）→“Azure OpenAI”→`AzureOpenAI`，“Redis”→`Redis`6. **外部actor角色**→`Operator`，`EndUser`（不要丢掉这些）**⛔Helm/Kubernetes部署命名（对比较稳定性至关重要）：**
当组件通过Helm图表或Kubernetes清单部署时，使用**Kubernetes工作负载名称**（来自Deployment/StatefulSet元数据。name）作为组件ID -而不是Helm模板文件名或目录结构：
-查看部署YAML中的`metadata.name`→使用它作为组件ID （PascalCase规范化）
—示例：“`templates/knowledge/devportal-deployment.yml`”中的“`metadata.name: devportal`”→组件ID为“`DevPortal`”
—示例：“`templates/knowledge/phi-deployment.yml`”中的“`metadata.name: phi-model`”→组件ID为“`PhiModel`”
- **原因：** Helm模板经常被重新组织（例如，从`templates/`移动到`templates/knowledge/`），但Kubernetes工作负载名称保持不变。使用工作负载名称可确保组件ID在目录重组中幸存下来。
-`source_files`必须包括部署YAML路径和应用程序源代码路径（例如，`helmchart/myapp/templates/knowledge/devportal-deployment.yml`和`developer-portal/src/`）
-`source_directories`必须包含两个tHelm模板目录和源代码目录**外部服务锚定（没有repo源代码的组件）：**
外部服务（云api、托管数据库、SaaS端点）在存储库中没有源文件。将它们固定在代码库中的集成点上：`source_files`→定义连接的客户端类或配置文件（例如，`src/MCP/appsettings.json`用于Azure OpenAI连接配置，`helmchart/values.yaml`用于Redis端点配置）
-`source_directories`→包含集成代码的目录（例如，LLM客户端的`src/MCP/Core/Services/LLM/`）`class_names`→你的repo中与服务对话的CLIENT类（例如`ResponsesAPIService`），而不是供应商的SDK类（例如，不是`OpenAIClient`）。如果不存在专用的客户端类，则留空。`namespace`→留空`""`（外部服务没有repo命名空间）
-`config_keys`→服务连接的env vars / config密钥（例如，`["AZURE_OPENAI_ENDPOINT", "RESPONSES_API_DEPLOYMENT"]`）。这些是最稳定的外部服务的Chors。`api_routes`→留空（外部服务暴露他们自己的路由，而不是你的）
-`dependencies`→使用的SDK包（例如，NuGet的`["Azure.AI.OpenAI"]`， pip的`["pymilvus"]`）

**为什么这很重要：**外部服务经常在LLM运行期间更改显示名称（例如，“Azure OpenAI”vs“GPT-4 Endpoint”vs“LLM Backend”）。`config_keys`和`dependencies`字段使它们在运行中匹配。**⛔禁止的命名模式-永远不要使用这些：**
永远不要发明与真实类不对应的抽象名称：`ConfigurationStore`，`LocalFileSystem`,`DataLayer`,`IngestionPipeline`,`BackendServer`永远不要缩写类名：`TerminalUI`表示`TerminalUserInterface`，`PSExecutor`表示`PowerShellCommandExecutor`-永远不要用同义词代替：`TaskServer`代替`TaskProcessor`，`LLMService`代替`ResponsesAPIService`永远不要将两个独立的类合并为一个组件：`ResponsesAPIService`和`LLMService`是两个不同的类→两个不同的组件
-永远不要为代码中不存在的东西创建组件：如果没有Windows注册表访问代码，不要创建`WindowsRegistry`组件
-不要在两次运行之间重命名：如果你在第一次运行中叫它`TaskProcessor`，那么在第二次运行中必须叫它`TaskProcessor`**⛔组件锚验证（强制性-在步骤2之前执行此操作）：**
在确定了所有组成部分之后，创建一个心理清单：   ```
   For EACH component:
     Q: What is the EXACT filename or class that anchors this component?
     A: [must cite a real file path, e.g., "src/Core/TaskProcessor.cs"]
     If you cannot cite a real file → DELETE the component from your list
   ```
此验证捕获了虚构的组件，如`WindowsRegistry`（不存在注册表代码）、`ConfigurationStore`（不存在这样的类）、`LocalFileSystem`（抽象概念，而不是类）。**⛔组件选择稳定性（当存在多个相关类时）：**
许多系统都有相关类的集群（例如，`CredentialManager`、`AzureCredentialProvider`、`AzureAuthenticationHandler`）。确保确定性选择：
- **选择拥有安全相关行为的类-做出信任决策，持有凭证或处理数据的类
- **优先选择在依赖注入**中注册的类，而不是helpers/utilities- **优先使用高级编排器**而不是其内部实现类
- **一旦你选择一个类，它的替代品成为别名** -将它们添加到`aliases`数组，而不是作为单独的组件
—**示例**：如果`CredentialManager`协调凭证查找，并且在内部使用`AzureCredentialProvider`，则`CredentialManager`为组件，`AzureCredentialProvider`为别名
- **示例**：不包含`SessionStore`和`SessionFiles`-`SessionStore`是类，`SessionFiles`是抽象概念
- * *清纯甜美t规则**：在同一代码上运行两次必须产生相同数量的组件（对于边缘情况±1）。差异≥3个分量表示未遵循选择规则。**⛔稳定性锚（用于比较匹配）：**
当在`threat-inventory.json`中记录每个组件时，`fingerprint`字段`source_directories`、`class_names`和`namespace`充当稳定性锚——即使在以下情况下也会持续存在的不可变标识符：
类被重命名（目录保持不变）
-文件被移动到不同的目录（类名保持不变）
-组件ID在分析运行之间改变（命名空间保持不变）
比较匹配算法更多地依赖于这些锚，而不是组件`id`字段。因此:
必须为每个进程类型组件填充`source_directories`（永远不要为空`[]`）
-`class_names`必须至少包含主类名
-`namespace`必须是实际的代码命名空间（例如，`MyApp.Core.Servers.Health`），而不是一个编造的分组
-这些字段是使组件在独立分析运行中可识别的，即使两个llm p不同的显示名称**⛔组件资格-什么资格作为威胁模型组件：**
只有满足以下所有条件，class/service才能成为威胁模型组件：
1. **它跨越信任边界或处理安全敏感数据**（凭据、用户输入、网络I/O、文件I/O、进程执行）
2. **它是一个顶级服务**，而不是一个内部助手（在DI中注册，或主要入口点，或有自己责任的代理）。
3. **它会出现在部署图中**——你可以指着它说“这个在这里运行，和那个对话”。

**总是包含这些组件类型（如果它们存在于代码中）
-所有代理类（HealthAgent, InfrastructureAgent, InvestigatorAgent， SupportabilityAgent等）
-所有MCP服务器类（HealthServer， InfrastructureServer等）
-主host/orchestrator（MCPHost等）
—所有外部服务连接ns （AzureOpenAI， AzureAD等）
-所有credential/auth管理器
—用户界面入口点
-所有工具执行服务（PowerShellCommandExecutor等）
—所有session/state持久性服务
-所有LLM服务类（ResponsesAPIService， LLMService -如果它们是单独的类，它们是单独的组件）
-外部参与者（操作员、最终用户）

**永远不要将这些作为单独的组件：**
-记录器(LocalFileLogger, TelemetryLogger) -这些是横切关注点，而不是威胁模型组件
-静态助手类
-Model/DTO类
-配置构建器（除非他们处理机密）
-运行时不存在的基础设施即代码类（AzureStackHCI集群参考，部署脚本）

**目标：**每次运行相同的代码应该识别相同的一组~12-20个组件。如果要包括记录器或排除探员，你做错了。**边界命名规则：**
-边界id必须是PascalCase（不能有`Layer`，`Zone`,`Group`，`Tier`后缀）
-从部署拓扑派生，而不是从代码架构层派生
—**部署拓扑决定边界：**     - Single-process app → **EXACTLY 2 boundaries**: `Application` (the process) + `External` (external services). NEVER use 1 boundary. NEVER use 3+ boundaries. This is mandatory for single-process apps.
     - Multi-container app → boundaries per container/pod
     - K8s deployment → `K8sCluster` + per-namespace boundaries if relevant
     - Client-server → `Client` + `Server`
** k8的多服务部署（对微服务架构至关重要）：**     When a K8s namespace contains multiple Deployments/StatefulSets with DIFFERENT security characteristics, create sub-boundaries based on workload type:
     - `BackendServices` — API services (FastAPI, Express, etc.) that handle user requests
     - `DataStorage` — Databases and persistent storage (Redis, Milvus, PostgreSQL, NFS) — these have different access controls, persistence, and backup policies
     - `MLModels` — ML model servers running on GPU nodes — these have different compute resources, attack surfaces (adversarial inputs), and scaling characteristics
     - `Agentic` — Agent runtime/manager services if present
     - The outer `K8sCluster` contains these sub-boundaries
     - **This is NOT "code layers"** — each sub-boundary represents a different Kubernetes Deployment/StatefulSet with its own security context, resource limits, and network policies
     - **Test**: If two components are in DIFFERENT Kubernetes Deployments with different service accounts, different network exposure, or different resource requirements → they SHOULD be in different sub-boundaries
**禁止边界方案（仅适用于单进程应用程序）：**     - Do NOT create boundaries based on code layers: `PresentationBoundary`, `OrchestrationBoundary`, `AgentBoundary`, `ServiceBoundary` are CODE LAYERS, not deployment boundaries. All these run in the SAME process.
     - Do NOT split a single process into 4+ boundaries. If all components run in one .exe, they are in ONE boundary.
- **示例**:`TerminalUserInterface`，`MCPHost`,`HealthAgent`，`ResponsesAPIService`在同一个进程中运行→它们都在`Application`中。像`AzureOpenAI`这样的外部服务位于`External`中。
-两次运行相同的代码必须产生相同数目的边界（±1）。差值≥2个边界是错误的。
-永远不要基于代码层创建边界(Presentation/Business/Data) -边界代表部署信任边界，而不是代码架构

**边界计数锁定：**
—确定边界后，锁定计数。两次运行相同的代码必须产生相同数量的边界（±1可以接受，如果一次运行识别边缘边界，另一次没有）
-在同一代码上，4边界与7边界的差异是错误的，表明没有遵守命名规则**附加的命名规则：**
-无论哪个LLM模型运行分析或运行多少次，相同的组件必须获得相同的`id`-外部演员（`Operator`，`AzureDataStudio`等）总是包括在内-永远不要放弃他们
-代表不同存储（文件，数据库）的数据存储总是单独的组件-永远不要合并它们
—锁定步骤2前的组件列表。在所有后续文件（架构、DFD、STRIDE、findings、JSON）中使用这些确切的id。
-如果两个类作为独立的文件存在（例如，`ResponsesAPIService.cs`和`LLMService.cs`），即使它们看起来相关，它们也是两个组件**⛔数据流完整性（必选-确保跨运行一致的流枚举）：**
必须详尽地列举数据流。对相同代码库的两个独立分析必须产生相同的一组流。要做到这一点：

**⛔回流流量建模规则（地址24%的流量计数方差）：**
- **不要建立单独的回流模型。**请求-响应对是一个双向流（使用`<-->`在美人鱼）。
—示例：`DF01: Operator <--> TUI`（一个输入输出流）
—示例：`DF03: MCPHost <--> HealthAgent`（一个流程的委托和结果）
DO模型只有在两个方向使用不同的协议或语义时才会分离流（例如，HTTP请求与WebSocket推回）。
- **为什么：**当运行独立决定是否创建1流或2流每次交互，流量计数变化20-30%。这条规则消除了这种差异。
- **流量计数公式：**`# flows ≈ # unique component-to-component interactions`。如果组件A与组件B对话，那就是1个流，而不是2个。

**流量完整性检查表（根据上述回流规则使用`<-->`双向流）：**
1. **Ingress/reverse代理流**:`DF_EndUser_to_NginxIngress`（双向`<-->`）、`DF_NginxIngress_to_Backend`（双向`<-->`）。每个都是一个流，而不是两个。
2. **Database/datastore流量**:`DF_Service_to_Redis`（双向`<-->`）。每个服务-数据存储对一个流。
3. **认证提供者流**:`DF_Service_to_AzureAD`（双向`<-->`）。每个服务认证对一个流。
4. **管理员访问流**:`DF_Operator_to_Service`（双向`<-->`）。每个管理员交互一个。
5. **流计数锁定**：枚举完流后，锁定计数。在同一代码上运行两次必须产生相同数量的流（±3个可接受）。相差>5个流表示枚举不完全。

**⛔外部实体包含规则（解决外部建模的差异）：**- **始终包括`AzureAD`（或`EntraID`）作为外部实体**如果代码从Azure AD / Microsoft Entra ID获取令牌（查找`ChainedTokenCredential`，`ManagedIdentityCredential`,`AzureCliCredential`， MSAL，或任何OAuth2/OIDC流）。
- **始终包括基础设施目标**（例如，`OnPremInfra`,`HCICluster`）作为一个外部实体，如果代码发送命令到外部基础设施通过PowerShell， REST，或WMI。
- **总是包含`AzureOpenAI`**（或等效LLM端点），如果代码调用云LLM API。
- **始终包含`Operator`**作为CLI/TUI工具，管理工具或操作控制台的外部actor。
- **经验法则：**如果代码中有一个服务的客户端类或配置，那么该服务就是一个外部实体。

**⛔TMT类别规则（地址跨运行的类别不一致）：**`SE.P.TMCore.WebSvc`（不是`SE.P.TMCore.NetApp`）
- **网络级服务**处理connections/sockets→`SE.P.TMCore.NetApp`—**执行操作系统命令的服务** （PowerShell、bash）→`SE.P.TMCore.OSProcess`- **将数据存储到磁盘的服务** (SessionStore, FileLogger)→`SE.DS.TMCore.FS`（分类为数据存储，而不是进程）
- **规则：**如果一个类的主要目的是保存数据，它就是一个数据存储。如果它执行计算或编排，它就是一个进程。不要在跑步之间切换。

**⛔DFD方向（必选-地址布局变化）：**
-所有DFDs必须使用`flowchart LR`（从左到右）。永远不要使用`flowchart TB`。
-所有摘要DFDs也必须使用`flowchart LR`。
-这是不可变的-不要根据美学或图表形状改变。** PascalCase的缩写规则：**
—保留熟悉的缩写为全大写：`API`，`NFS`,`LLM`,`SQL`,`HCI`,`AD`,`UI`,`DB`-示例：`IngestionAPI`（不是`IngestionApi`）、`NFSServer`（不是`NfsServer`）、`AzureAD`（不是`AzureAd`）、`VectorDBAPI`（不是`VectorDbApi`）
-单字技术保持标准的大小写：`Redis`，`Milvus`,`PostgreSQL`,`Nginx`**通用技术命名（使用这些id为众所周知的基础设施）：**
- Rediscache/state:`Redis`（从来不是`DaprStateStore`，`RedisCache`,`StateStore`）
- Milvus矢量DB:`Milvus`（从来不是`MilvusVectorDb`，`VectorDB`）
NGINX入口：`NginxIngress`（从来不是`IngressNginx`）
- AzureAD/Entra:`AzureAD`（从来不是`AzureAd`，`EntraID`）
- PostgreSQL:`PostgreSQL`（从来不是`PostgresDb`，`Postgres`）
—User/Operator：管理员用户`Operator`，终端用户`EndUser`- Azure OpenAI:`AzureOpenAI`（从来不是`OpenAIService`，`LLMEndpoint`）
- NFS:`NFSServer`（never`NfsServer`,`FileShare`）
-如果两个LLM模型是独立部署的，保持它们独立（不要将`MistralLLM`+`PhiLLM`合并为`LocalLlm`）

**BUT：对于特定于应用程序的类，使用代码中的确切类名，而不是技术标签：**
-`ResponsesAPIService.cs`→`ResponsesAPIService`（不是`OpenAIService`-类被命名为ResponsesAPIService）
-`TaskProcessor.cs`→`TaskProcessor`(不是`LocalLLM`-类命名为TaskProcessor)
-`SessionStore.cs`→`SessionStore`（不是`StatePersistence`-类被命名为SessionStore）
**组件粒度规则（对稳定性至关重要）：**
-在**technology/service级别**建模组件，而不是script/file级别
-运行Kusto的Docker容器是`KustoContainer`- NOT分解为`KustoService`+`IngestLogs`+`KustoDataDirectory`Moby Docker引擎是`MobyDockerEngine`，而不是`InstallMoby`（安装脚本是证据，不是组件）
-一个工具的安装程序是`SetupInstaller`-不重命名为`InstallAzureEdgeDiagnosticTool`（脚本文件名）
-规则：如果一个组件有一个主要功能（例如，“运行Kusto查询”），将其建模为一个组件，不管有多少scripts/files实现它
脚本是组件的证据，而不是组件本身
-在运行期间保持相同的粒度-永远不要将单个组件拆分为子组件或在运行之间合并子组件**⛔组件ID格式（必选-解决大小写差异）：**
-所有组件id必须是PascalCase。不要使用kebab-case、snake_case或camelCase。
-示例：`HealthAgent`（不是`health-agent`），`AzureAD`（不是`azure-ad`），`MCPHost`（不是`mcp-host`）
-这适用于所有工件：0.1-architecture.md，1-threatmodel.md， DFD美人鱼，STRIDE, finding， JSON。**⛔跨范围规则（解决外部实体分析差异）：**`2-stride-analysis.md`中的跨步分析必须包括元素表中除外部参与者（Operator, EndUser）之外的所有元素的section。
-外部服务（AzureOpenAI, AzureAD, OnPremInfra）确实得到STRIDE节-他们是攻击表面从你的系统的角度来看。
-外部参与者（人类用户）不会得到STRIDE部分-他们是威胁来源，而不是目标。
-这意味着：如果你总共有20个元素，其中1个是外部角色，你要写19个STRIDE部分。**⛔跨步深度一致性（地址威胁计数方差）：**
-每个组件必须分析所有7个STRIDE-A类别（S， T， R， I， D， E， A）。
-每个STRIDE类别必须明确地针对每个组件：要么有一个或多个具体的威胁，要么有一个明确的`N/A — {1-sentence justification}`行解释为什么该类别不适用于该特定组件。
—一个类别可能产生0、1、2、3或更多威胁，威胁数量取决于组件的实际攻击面。每个类别不要限制在一个威胁。具有丰富安全界面的组件（API服务、身份验证管理器、命令执行器、LLM客户端）每个相关STRIDE类别通常应该有2-4个威胁。只有简单的组件（静态配置，只读数据存储）应该大多为0-1。
- **预期分布：**对于15组分系统：~30%的STRIDE单元应该是0（与N/A）， ~40%应该D = 1， ~25%应为2，~5%应为3+。如果所有单元格都是0或1（二进制模式）→分析太浅。返回并识别其他威胁载体。
-N/A条目不计入汇总表中的威胁总数。只有具体的威胁行才算数。
-汇总表S/T/R/I/D/E/A列显示每个类别的具体威胁计数（如果N/A是合理的，则0为有效）。
-这确保了全面的覆盖，同时产生准确的，非膨胀的威胁计数。2. **编写架构概述** （`0.1-architecture.md`）
- **先读取`skeletons/skeleton-architecture.md`** -复制骨架结构，填充`[FILL]`占位符
-系统用途，关键组件，顶级场景，技术堆栈，部署
- **使用步骤1中锁定的组件id ** -不要重命名或合并组件
- **参考：**`output-formats.md`为模板，`diagram-conventions.md`为架构图样式

3. **库存安全基础设施**
-在标记漏洞之前识别启用安全的组件
- **参考：**`analysis-principles.md`安全基础设施清单表

4. **生成威胁模型DFD** （`1.1-threatmodel.mmd`,`1.2-threatmodel-summary.mmd`,`1-threatmodel.md`）
- **先读取`skeletons/skeleton-dfd.md`，`skeletons/skeleton-summary-dfd.md`和`skeletons/skeleton-threatmodel.md`**
- **参考：**`diagram-conventions.md`用于DFD样式，`tmt-element-taxonomy.md`用于元素分类
-⚠️**在完成之前：**运行预渲染检查表从`diagram-conventions.md`⛔**POST-DFD GATE -创建`1.1-threatmodel.mmd`:**后立即运行
1. 在`1.1-threatmodel.mmd`中计数元素（带有`((...))`、`[(...)`、`["..."]`的节点）
2. 计数边界（`subgraph`lines）
3. 如果元素> 15或边界> 4：      → You MUST create `1.2-threatmodel-summary.mmd` using `skeleton-summary-dfd.md` NOW
      → Do NOT proceed to `1-threatmodel.md` until the summary file exists
4. 如果阈值未满足→跳过总结，继续`1-threatmodel.md`5. 创建`1-threatmodel.md`（如果生成了摘要，则包括摘要视图部分）

5. **使用STRIDE-A （`2-stride-analysis.md`）枚举每个元素和流的威胁**
- **先读取`skeletons/skeleton-stride-analysis.md`** -使用汇总表和每组件结构
- **参考：**`analysis-principles.md`用于层定义，`output-formats.md`用于STRIDE模板
- **⛔PREREQUISITE FLOOR CHECK（每个威胁）：**在为任何威胁分配先决条件之前，请在组件暴露表（`0.1-architecture.md`）中查找组件的`Min Prerequisite`和`Derived Tier`。威胁的先决条件必须≥组件的地板。威胁层必须≥组件的派生层（即，如果组件为T2，则没有威胁可以是T1）。使用规范先决条件→来自`analysis-principles.md`的层映射。

6. **对于每个威胁：**引用files/functions/endpoints，提出缓解措施，提供验证步骤7. **验证发现** -在记录文件之前，根据实际配置确认每个发现
- **参考：**`analysis-principles.md`查找验证清单

7 b。**技术扫描** -从`analysis-principles.md`运行技术特定的安全检查表
-对于在repo中找到的每一项技术（Redis, Milvus, PostgreSQL, Docker, K8s， ML模型，llm， NFS，CI/CD等），确保你至少有一个发现或明确的缓解
-此步骤捕获组件级STRIDE遗漏的漏洞（例如，数据库认证默认值，容器加固，密钥管理）
—添加所有缺失的查找结果，然后继续执行步骤8

8. **编译结果** （`3-findings.md`）
- **参考：**`output-formats.md`的调查结果模板和相关威胁链接格式
- **参考：**`skeletons/skeleton-findings.md`-阅读这个骨架，逐字复制，为每个发现填写`[FILL]`占位符⛔**预写门-在`3-findings.md`:**调用`create_file`之前进行验证
1. 查找id:`### FIND-01:`，`### FIND-02:`-顺序，`FIND-`前缀（不是`F01`或`F-01`）
2. CVSS前缀：每个向量以`CVSS:4.0/`开始（不是裸`AV:N/AC:L/...`）
3. 相关威胁：每个威胁ID是一个单独的超链接`[TNN.X](2-stride-analysis.md#anchor)`（非纯文本）
4. 子章节：`#### Description`，`#### Evidence`,`#### Remediation`,`#### Verification`（非`Recommendation`）
5. 排序：每层内→危急→重要→中等→低→高CVSS优先
6. 每个发现都显示所有10个强制属性行
7. **部署上下文门（FAIL-CLOSED）：**读取`0.1-architecture.md`部署分类和组件公开表。      If classification is `LOCALHOST_DESKTOP` or `LOCALHOST_SERVICE`:
      - ZERO findings may have `Exploitation Prerequisites` = `None` → fix to `Local Process Access` (T2) or `Host/OS Access` (T3)
      - ZERO findings may be in `## Tier 1` → downgrade to T2/T3 based on prerequisite
      - ZERO CVSS vectors may use `AV:N` unless the **specific component** has `Reachability = External` in the Component Exposure Table → fix to `AV:L`
      For ALL deployment classifications:
      - For EACH finding, look up its Component in the exposure table. The finding's prerequisite MUST be ≥ the component's `Min Prerequisite`. The finding's tier MUST be ≥ the component's `Derived Tier`.
      - Prerequisites MUST use only canonical values: `None`, `Authenticated User`, `Privileged User`, `Internal Network`, `Local Process Access`, `Host/OS Access`, `Admin Credentials`, `Physical Access`, `{Component} Compromise`. ⛔ `Application Access` and `Host Access` are FORBIDDEN.
      If ANY violation exists → **DO NOT WRITE THE FILE.** Fix all violations first.
⛔**快速故障门：**立即写入后，从`verification-checklist.md`运行内联快速检查`3-findings.md`。修复后再继续。

⛔**必选：所有3层部分必须存在。**即使一层没有发现，也要在标题中注明：
-`## Tier 1 — Direct Exposure (No Prerequisites)`→`*No Tier 1 findings identified for this repository.*`-这确保了比较匹配和验证的结构一致性。⛔**覆盖验证反馈回路（必选）：**
编写完`3-findings.md`末尾的威胁覆盖率验证表后：
1. **扫描你刚刚写的表格。**计算有多少威胁状态`✅ Covered`vs`🔄 Mitigated by Platform`vs`⚠️ Needs Review`vs`⚠️ Accepted Risk`。
2. **如果任何威胁有`⚠️ Accepted Risk`**→FAIL。这个工具不能接受风险。回去为每一个创建一个发现。
3. **如果平台比率> 20%**→怀疑。重新检查每个`🔄 Mitigated by Platform`条目：缓解真的来自由不同团队管理的外部系统吗？如果缓解是repo自己的代码（授权中间件、文件权限、TLS配置、本地主机绑定），则将其重新分类为`Open`并创建查找项。
4. **如果`2-stride-analysis.md`中的任何`Open`威胁没有相应的发现**→现在创建一个发现。使用威胁的描述作为查找标题，将缓解列作为补救指导，并分配严重性基于STRIDE类别。
5. **更新`3-findings.md`**与新创建的发现。重编号顺序。更新Coverage表以显示每个表的`✅ Covered`。
6. **这个循环是覆盖表的整个点**——它不是文档，它是强制完成覆盖的自检。如果你写了表格而没有对空白采取行动，那么你的努力就白费了。8 b。**生成威胁清单** （`threat-inventory.json`）
- **先读取`skeletons/skeleton-inventory.md`** -使用精确的字段名和模式结构
-写完所有的降价报告后，编译一个结构化的JSON清单，包括所有组件、边界、数据流、威胁和发现    - Use canonical PascalCase IDs for components (derived from class/file names) and keep display labels separate
—使用规范流id:`DF_{Source}_to_{Target}`    - Include identity keys on every threat and finding for future matching
    - Include deterministic identity fields for component and boundary matching across runs:
       - Component: `aliases`, `boundary_kind`, `fingerprint`
       - Boundary: `kind`, `aliases`, `contains_fingerprint`
    - Build `fingerprint` from stable evidence (source files, endpoint neighbors, protocols, type) — never from prose wording
    - Normalize synonyms to the same canonical component ID (example: `SupportAgent` and `SupportabilityAgent` → `SupportabilityAgent`) and store alternate names in `aliases`
    - Sort arrays deterministically before writing JSON:
       - `components` by `id`
       - `boundaries` by `id`
       - `flows` by `id`
       - `threats` by `id` then `identity_key.component_id`
       - `findings` by `id` then `identity_key.component_id`
-提取指标（总数，每层计数，每个stride类别计数）
-包括git元数据（提交SHA，分支，日期）和分析元数据（模型，时间戳）
—**参考：`threat-inventory.json`模式：**`output-formats.md`- **该文件不链接在0-assessment.md**中，但总是存在于输出文件夹中

⛔**预写大小检查（必选-在JSON调用`create_file`之前）：**
在编写`threat-inventory.json`之前，计算您计划包含的数据：
-统计来自`2-stride-analysis.md`的总威胁（grep`^\| T\d+\.`）
-计算`3-findings.md`的总结果（grep`### FIND-`）
-从`0.1-architecture.md`计算组件总数
- **如果威胁> 50或发现> 15:**不要使用单个`create_file`呼叫。     Instead, use one of: (a) delegate to sub-agent, (b) Python extraction script, (c) chunked write strategy.
- **如果威胁≤50且发现≤15:**单个`create_file`是可以接受的，但要保持条目最少（1句description/mitigation字段）。⛔**POST-WRITE验证（必选- JSON数组完整性）：**
写入`threat-inventory.json`后，立即验证：
-`threats.length == metrics.total_threats`-如果不匹配，威胁数组在生成时被截断。通过重新读取`2-stride-analysis.md`并提取每个威胁行来重建。
-`findings.length == metrics.total_findings`-如果不匹配，从`3-findings.md`重建。
-`components.length == metrics.total_components`-如果不匹配，从architecture/element表重建。

⛔**跨文件威胁计数验证（强制-捕获掉落的威胁）：**
JSON`threats.length`可以匹配`metrics.total_threats`，但如果在JSON生成期间删除了威胁，则两者都可能是错误的。为了抓住这个：
—统计“`2-stride-analysis.md`: grep”中的威胁行数，并统计唯一的威胁id
—将此计数与JSON中的`threats.length`进行比较
-如果markdown有比JSON更多的威胁→JSON丢弃的威胁。通过从`2-stride-analysis.md`重新提取所有威胁来重新构建JSON。
这是在测试中发现的第二大质量问题Ng（截断后）。当子代理从内存写入JSON而不是重新读取STRIDE文件时，大型repos（114+威胁）通常会删除1-3个威胁。⛔**字段名合规门（必选-在数组检查后立即运行）：**
从JSON中读取第一个组件和第一个威胁，并验证这些确切的字段名称：
-`components[0]`有键`"display"`（不是`"display_name"`，不是`"name"`）→如果错误，找到替换所有出现
-`threats[0]`有键`"stride_category"`（不是`"category"`）→如果错误，找到-替换所有出现
-`threats[0].identity_key`有关键字`"component_id"`（威胁→组件链接必须在`identity_key`内部，而不是威胁上的顶级`component_id`字段）→如果错误，重构
-`threats[0]`有两个`"title"`（短名称，例如，“信息披露- Redis未加密的流量”）和`"description"`（长散文）。如果只存在`description`而不存在`title`，则从`description`的第一句创建`title`。如果存在`name`或`threat_name`而不是`title`，则查找替换为`title`- **为什么这很重要：**下游工具依赖于这些e精确的字段名称。错误的名称会导致零值热图、组件匹配中断以及比较报告中的空显示标签。
- **如果任何字段名是错误的：**在继续之前，在JSON文件上使用find-replace修复它。不要留下验证。- **这是在测试中观察到的头号质量问题。**大型仓库（20+组件，80+威胁）经常有截断JSON数组，因为模型耗尽了输出令牌。如果任何数组被截断，你必须在继续之前重建它。不要用不匹配的计数完成。

⛔**硬门截断恢复（必选）：**
如果写后验证检测到任何数组不匹配：
1. **立即删除截断的`threat-inventory.json`2. **不要试图修补**截断的文件-部分JSON是不可靠的
3. **使用以下策略之一重新生成**（按优先顺序）：      a. **Delegate to a sub-agent** — hand the sub-agent the output folder path and instruct it to read `2-stride-analysis.md` and `3-findings.md`, then write `threat-inventory.json`. The sub-agent has a fresh context window.
      b. **Python extraction script** — write a Python script that reads the markdown files, extracts threats/findings via regex, and writes the JSON. Run the script via terminal.
      c. **Chunked write** — use the Large Repo Strategy below.
4. **再生后重新验证** -如果仍然不匹配，重复下一个策略
5. **如果计数不匹配，不要进行步骤9（评估）或步骤10（验证）**

⛔**大型回购策略（强制回购bbb60威胁）：**
对于生成超过60个威胁的repos，如果一次生成，JSON文件可能会超过输出令牌限制。使用这种分块方法：
1. **先写元数据+组件+边界+流+指标** -这些都是小数组
2. **批量追加威胁** -写入威胁数组，每次追加操作约20个威胁。使用`replace_string_in_file`将批处理添加到现有文件中，而不是在一次`create_file`调用中写入整个JSON。
3. **追加发现** -如果>15发现类似批处理
4. **最终验证** -读取完整的文件并验证所有数组长度匹配度量

**替代方法：**如果分块写入是不可行的，保持每个threat/finding条目最少：
-`description`字段：最多1个句子（不是完整的散文段落）
-`mitigation`字段：最多1句
-删除重复降价内容的冗余字段
- JSON是用来匹配的，而不是用来阅读的-简洁是关键9. **撰写评估** （`0-assessment.md`）
- **参考：**`output-formats.md`评估模板
**参考：**`skeletons/skeleton-assessment.md`-阅读这个骨架，逐字复制，填写`[FILL]`占位符
-⚠️**所有7部分都是强制性的：**报告文件，执行摘要，行动摘要（快速获胜），分析背景和假设（需要验证+查找覆盖），参考文献咨询，报告元数据，分类参考
-不要添加额外的部分，如“严重性分布”、“架构风险区域”、“方法说明”或“可交付成果”——这些不在模板中⛔**预写门-在`0-assessment.md`:**调用`create_file`之前进行验证
1. 整整7个部分：报告文件，执行摘要，行动摘要，分析背景和假设（与`&`），参考文献，报告元数据，分类参考
2.`---`每对`## `段之间的水平线（最少6条）
3.`### Quick Wins`,`### Needs Verification`，`### Finding Overrides`都在
4. 参考：两个小节（`### Security Standards`+`### Component Documentation`），包含3列表和完整的url
5. 所有元数据值都用反引号括起来；所有字段（模型、已开始分析、已完成分析、持续时间）
6.Element/finding/threat计数与其他文件中的实际计数匹配

⛔**快速故障门：**立即写入后，从`verification-checklist.md`运行内联快速检查`0-assessment.md`。修复后再继续。

10. **最终验证** -迭代校正回路    This step runs verification and fixes in a loop until all checks pass. Do NOT finalize with any failures remaining.

    **Pass 1 — Comprehensive verification:**
    - Delegate to a verification sub-agent with the content of `verification-checklist.md` + the output folder path
    - Sub-agent runs ALL Phase 0–5 checks and reports PASS/FAIL with evidence
    - If ANY check fails:
      1. Fix the failed file(s) using the available file-edit tool
      2. Re-run ONLY the failed checks against the fixed file(s)
      3. Repeat until the failed checks pass

    **Pass 2 — Regression check (if Pass 1 had fixes):**
    - Re-run Phase 3 (cross-file consistency) to ensure fixes didn’t break other files
    - If new failures appear, fix and re-verify

    **Exit condition:** ALL phases report 0 failures. Only then mark the analysis as complete.

    **Sub-agent context management:**
    - Include the relevant phase content from `verification-checklist.md` in the sub-agent prompt
    - Include the output folder path so the sub-agent can read files
    - Sub-agent output MUST include: phase name, total checks, passed, failed, and for each failure: check ID, file, evidence, exact fix instruction. Do not return "looks good" without counts.
---

##工具使用

进度跟踪（待办事项）
-在每个主要阶段开始时创建待办事项
-在每个阶段开始前标记进行中
-在完成每个阶段后立即标记完成

子任务委派（agent）
将窄的、只读的任务委派给子代理（参见上面的子代理治理）。允许代表团:
- **上下文收集：**“在这些目录中搜索验证模式并返回摘要”
- **代码分析：**“读取这些文件并识别与安全相关的api、凭据和信任边界”
—**校验：**将`verification-checklist.md`文件的内容和输出文件夹路径交给校验子代理。它读取文件并返回PASS/FAIL结果。PARENT修复任何失败。
- **JSON生成（例外）：**对于大型repos，委托`threat-inventory.json`写入精确的文件路径和预计算的数据**绝对不要委托：**“写0.1-architecture.md”、“生成STRIDE分析”、“执行威胁模型分析”，或任何可能导致子代理独立生成报告文件的提示。

---

验证清单（最后一步）

完整的验证清单见`verification-checklist.md`。它包含9个阶段：

b> **权限层次结构：**`orchestrator.md`定义了authororing规则（编写报告时应该做什么）。`verification-checklist.md`定义了CHECKING规则（写入后要验证的内容）。一些规则出现在两个文件中是为了可见性——如果它们发生冲突，`orchestrator.md`规则优先于创作决策，`verification-checklist.md`优先于pass/fail标准。对于所有结构、图表和一致性检查的完整列表，请始终查阅`verification-checklist.md`-它是质量门的唯一真实来源。0. **阶段0 -常见偏差扫描**：已知偏差模式与错误→正确的例子
1. **阶段1 -每个文件结构检查**：节顺序，所需的内容，格式
2. **阶段2 -图渲染检查**：美人鱼init块，classDef，样式，语法
3. **阶段3 -跨文件一致性检查**：组件覆盖，DF映射，从威胁到发现的可追溯性
4. **第4阶段-证据质量检查**：证据具体，标记前验证合规性
5. 阶段5 - JSON模式验证**：模式字段，数组完整性，度量一致性
6. 阶段6 -确定性身份：组件ID稳定性，边界命名，流ID一致性
7. **第7阶段-基于证据的先决条件**：先决部署证据，覆盖完整性
8. **阶段8 -比较HTML**（仅增量）：HTML结构，更改注释，CSS**内联快速检查：**`verification-checklist.md`还包含内联快速检查，必须在写入每个文件后立即运行（在步骤10之前）。当内容仍处于活动上下文中时，这些捕获错误。

* *双行程用法:* *
- **写作前（工作流前期工作）：**扫描第一阶段和第二阶段，内化结构和图表质量门。这可以防止返工。
- **写入后（步骤10）：**运行所有阶段0-4全面检查已完成的输出。阶段0是最关键的——它捕获在运行期间持续存在的偏差。在完成之前修复任何失败。

**委托：**将`verification-checklist.md`的内容和输出文件夹交给验证子代理。它将运行所有检查并生成PASS/FAIL摘要。在完成之前修复任何失败。

---

##开始分析

如果没有提供文件夹路径，则从其根分析整个存储库。