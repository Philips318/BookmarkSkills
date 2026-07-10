---
name: code-tour
description: >
  Use this skill to create CodeTour .tour files — persona-targeted, step-by-step walkthroughs
  that link to real files and line numbers. Trigger for: "create a tour", "make a code tour",
  "generate a tour", "onboarding tour", "tour for this PR", "tour for this bug", "RCA tour",
  "architecture tour", "explain how X works", "vibe check", "PR review tour",
  "contributor guide", "help someone ramp up", or any request for a structured walkthrough
  through code. Supports 20 developer personas (new joiner, bug fixer, architect, PR reviewer,
  vibecoder, security reviewer, and more), all CodeTour step types (file/line, selection,
  pattern, uri, commands, view), and tour-level fields (ref, isPrimary, nextTour).
  Works with any repository in any language.
---
#代码巡游技能

您正在创建一个CodeTour——一个针对角色的、逐步的代码库演练
直接链接到文件和行号。CodeTour文件位于`.tours/`中，可以使用
[VS CodeCodeTour扩展]（https://github.com/microsoft/codetour）。`scripts/`中捆绑了两个脚本：

- **`scripts/validate_tour.py`** -运行后写任何旅游。检查JSON有效性、file/directory是否存在、边界内的行号、模式匹配、nextour交叉引用和叙述弧。运行：`python ~/.agents/skills/code-tour/scripts/validate_tour.py .tours/<name>.tour --repo-root .`- **`scripts/generate_from_docs.py`** -当用户要求从README/docs生成时，首先运行此命令以提取骨架，然后填充它。运行：`python ~/.agents/skills/code-tour/scripts/generate_from_docs.py --persona new-joiner --output .tours/skeleton.tour`捆绑了两个参考文件：- **`references/codetour-schema.json`** -权威JSON模式。读取它以验证任何字段名称或类型。您使用的每个字段都必须符合它。
**`references/examples.md`** - 8个真实世界的CodeTour之旅，从生产仓库带注释的技术。当您想了解如何在实践中使用特定功能（`commands`、`selection`、`view`、`pattern`、`isPrimary`、多线路系列）时，请阅读本文。

真实世界的`.tour`文件在GitHub

这些是已确认的生产`.tour`文件。当你需要一个具体步骤类型、tourlevel字段或叙述结构的工作示例时，获取一个——当真正的东西需要一次获取时，不要从记忆中编写。

通过GitHub代码搜索找到更多信息：https://github.com/search?q=path%3A**%2F*.tour+&type=code####按步骤类型/技术演示

|学习什么|文件URL ||---|---|
|`directory`+`file+line`（贡献者入职）|https://github.com/coder/code-server/blob/main/.tours/contributing.tour|
|`selection`+`file+line`+介绍内容步骤（无障碍项目）|https://github.com/a11yproject/a11yproject.com/blob/main/.tours/code-tour.tour|
|最小教程-紧凑的`file+line`叙述互动学习|https://github.com/lostintangent/rock-paper-scissors/blob/master/main.tour|
|多巡回回购与`nextTour`链（云原生OCI演练）|https://github.com/lucasjellema/cloudnative-on-oci-2021/blob/main/.tours/introduction.tour|
|`isPrimary: true`（标记入车入口）|https://github.com/nickvdyck/webbundlr/blob/main/.tours/getting-started.tour|
|`pattern`而不是`line`（regex锚定步骤）|https://github.com/nickvdyck/webbundlr/blob/main/.tours/architecture.tour|

**原始内容提示：**前缀`raw.githubusercontent.com`和删除`/blob/`的原始JSON访问。

一次伟大的旅行不只是带注释的文件。这是一种叙事——一个讲给特定的人听的故事
什么重要，为什么重要，下一步该怎么做。您的目标是编写旅行
当合适的人第一次打开这个回购时，他们会希望存在。

**关键：只创建`.tour`JSON文件。永远不要创建、修改或脚手架任何其他文件

---

##步骤1：发现repo在询问用户任何问题之前，先探索一下代码库：

—列出根目录，读取README，检查关键配置文件
(package.json,pyproject.toml，开始。Mod、Cargo.toml、composer.json等)
-确定语言、框架和项目的功能
—将文件夹结构映射1-2层
-查找入口点：主文件，索引文件，应用程序引导
- **注意哪些文件是实际存在的** -你在游览中写的每个路径必须是真实的

如果回购是稀疏的或空的，那么就这样说，并使用现有的回购。

**如果用户说“从README生成”或“使用文档”：**先运行骨架生成器，然后通过读取实际文件来填充每个`[TODO: ...]`：```bash
python skills/code-tour/scripts/generate_from_docs.py \
  --persona new-joiner \
  --output .tours/skeleton.tour
```
入口点由language/framework不要阅读所有内容——从这里开始，然后跟随导入。

|堆栈|读取第一个|的入口点|-------|---------------------------|
| **Node.js/ TS** |`index.js/ts`,`server.js`,`app.js`,`src/main.ts`,`package.json`（脚本）|
| **Python** |`main.py`,`app.py`,`__main__.py`,`manage.py`(Django),`app/__init__.py`(Flask/FastAPI) |
| **Go** |`main.go`,`cmd/<name>/main.go`,`internal/`|
| **Rust** |`src/main.rs`,`src/lib.rs`,`Cargo.toml`|
| **Java / Kotlin** |`*Application.java`,`src/main/java/.../Main.java`,`build.gradle`|
| **Ruby** |`config/application.rb`,`config/routes.rb`,`app/controllers/application_controller.rb`|
| **PHP** |`index.php`,`public/index.php`,`bootstrap/app.php`(Laravel) |

Repo类型的变体-相应地调整焦点

相同的角色要求不同的东西，这取决于这是什么类型的回购：

|回购类型|强调什么|典型的锚文件||-----------|-------------------|----------------------|
|请求生命周期，授权，错误契约|路由器，中间件，处理器，模式|
| **库/ SDK** |公共API接口，扩展点，版本控制|index/exports，类型，变更日志|
| **命令行工具** |命令解析，配置加载，输出格式化| main，命令/，config |
| **Monorepo** |包边界，共享契约，构建图|根package.json/pnpm-workspace，共享/，包/ |
| **框架** |插件系统，生命周期钩子，逃生口|核心/，插件/，生命周期|
| **数据管道** |源→转换→汇聚，模式所有权|摄取/，转换/，模式/，dbt模型|
| **前端应用** |组件层次结构，状态管理，路由|页面/，存储/，路由器，api/ |对于单一目标：确定与角色目标最相关的2-3个包。不要试图浏览所有的东西——用一个解释如何浏览工作区的步骤打开浏览，然后保持专注。

大型回购策略

对于拥有100多个文件的repos：不要试图阅读所有文件。

1. 首先读取入口点和README
2. 建立前5-7个模块的心理模型
3. 对于所要求的角色，确定** 2-3个最重要的模块，并深入阅读
4. 对于未涉及的模块，请在介绍步骤中将其列为“超出本指南的范围”。
5. 使用`directory`步骤来定位您绘制但没有阅读的区域-它们定向而不需要完全了解

集中10步浏览正确的文件胜过分散的25步浏览所有文件。

---

第二步：阅读意图——推断你能推断的一切，只问你不能问的**来自用户的一条信息就足够了。**阅读他们的请求并推断角色；
在问任何问题之前要有深度和专注。

意图映射

|用户说|→角色|→深度|→动作||-----------|-----------|---------|----------|
|“tour for this PR”/“PR review”/“#123”| PR -reviewer |标准|为PR添加`uri`步骤；使用`ref`作为分支|
| "why did X break" / "RCA" / "incident" | RCA -investigator |标准|追踪失败因果链|
| "debug X" / "bug tour" / "find the bug" | bug-fixer | standard |入口→故障点→测试|
| "onboarding" / "new joiner" / "ramp up" | new-joiner | standard |目录、设置、业务上下文|
| "quick tour" / "vibe check" / "just the gist" | vibeccoder | quick | 5-8步，快速路径只有|
| “explain X是如何工作的” / "feature tour" | feature-explainer | standard | UI→API→后端→存储|
|“架构”/“技术主管”/“系统设计”|架构师|深度|边界，决策，权衡|
|“安全”/“认证审查”/“信任边界”|安全审查|标准|认证流、验证、敏感汇|
| "refactor" / "safe to e .xtract ?”|重构器|标准|接缝，隐藏深度，提取顺序|
| "performance" / "bottleneck " / "slow path" | performance-optimizer | standard | Hot path, N+1,I/O，缓存|
| "contributor" / "open source onboarding" |外部贡献者| quick |安全区域，公约，地雷|
|“概念”/“解释模式X”|概念-学习者|标准|概念→实现→原理|
| “test coverage“ / ”在哪里添加测试” | test-writer | standard | contract, seam, coverage gaps |
“我如何调用API”| API消费者|标准|公共表面，认证，错误语义|**默默地推断：**人物，深度，焦点区域，是否添加`uri`/`ref`，`isPrimary`。

只有当你真的无法推断时才问：**
-“bug导览”但没有bug描述→要求bug描述
-“功能导览”但没有功能名称→询问哪个功能
-明确要求的“特定文件”→按照要求停止

不要询问`nextTour`、`commands`、`when`或`stepMarker`，除非用户提到它们。

###公关之旅食谱

对于PR之旅：将`"ref"`设置为分支，使用`uri`步骤打开PR，首先覆盖已更改的文件，然后是未更改但重要的文件，以审阅者检查清单结束。

用户提供的定制——始终遵守这些

|用户说|做什么||-----------|-----------|
|“覆盖`src/auth.ts`和`config/db.yml`”|这些文件需要停止|
| “引脚到`v2.3.0`标签” / "this commit: abc123" |设置`"ref": "v2.3.0"`|
| “链接到PR #456” /粘贴URL |在适当的叙述时刻添加`uri`步骤|
|设置`"nextTour": "Security Review"`|
|“这是主要的登船旅游”|设置`"isPrimary": true`|
|添加`"commands": ["workbench.action.terminal.focus"]`|
|“deep”/“thorough”/“5 steps”/“quick”|相应地覆盖深度|

---

##步骤3：读取实际文件-没有例外

**每个文件路径和行号必须通过读取文件来验证
指向错误文件或不存在的行的遍历比没有遍历更糟糕。

对于每一个计划的步骤：
1. 读取文件
2. 找到要高亮显示的代码的确切行
3. 充分理解它，以便向目标角色解释它如果用户请求的文件不存在，就说出来——不要悄悄地替换另一个文件。

---

##步骤4：编写游览

保存到`.tours/<persona>-<focus>.tour`。阅读`references/codetour-schema.json`权威字段列表。您使用的每个字段都必须出现在该模式中。

###游览根```json
{
  "$schema": "https://aka.ms/codetour-schema",
  "title": "Descriptive Title — Persona / Goal",
  "description": "One sentence: who this is for and what they'll understand after.",
  "ref": "main",
  "isPrimary": false,
  "nextTour": "Title of follow-up tour",
  "steps": []
}
```
省略任何不适用于本次旅行的字段。

**`when`** -条件显示。在运行时求值的JavaScript表达式。只展示这次巡演
如果条件为真。可用于特定于角色的自动启动或隐藏高级游览
直到一个更简单的完成。```json
{ "when": "workspaceFolders[0].name === 'api'" }
```
**`stepMarker`** -直接在源代码注释中嵌入步骤锚。设置后，代码绕道
查找文件中的`// <stepMarker>`注释，并使用它们作为步骤位置，而不是
（或旁边）行号。用于主动更改行号的代码
不断转变。示例：设置`"stepMarker": "CT"`，并将`// CT`放在源文件中。
除非用户要求，否则不要建议这样做——这需要编辑源文件，这是不寻常的。

---

###步骤类型-完整参考

所有步骤类型：**内容** （intro/closing，最多2个），**目录**，**文件+行**（工作马），**选择**（代码块），**模式** （regex匹配），**uri**（外部链接），**视图**（关注VS Code面板），**命令**（运行VS Code命令）。

> **路径规则：**`"file"`和`"directory"`必须与repo root相对。没有绝对路径，没有前导`./`。

---

###何时使用每个步骤类型

|情况|步骤类型||-----------|-----------|
|旅游介绍或关闭|内容|
| “这是这个文件夹中的内容” |目录|
|文件+ |行
|一个function/class体是点|选择|
|行号移位，文件易失|模式|
| PR / issue / doc给出了“为什么”| uri |
阅读器应该打开终端或资源管理器|视图或命令|

---

步长计数校准

将步骤与深度和角色相匹配。这些是目标，而不是硬性限制。

|深度|总步数|核心路径步数| Notes ||-------|-------------|-----------------|-------|
|快速| 5-8 | 3-5 | vibeccoder，快速资源管理器-无情切割|
|标准| 9-13 | 6-9 |大多数人物角色-宽度+足够的细节|
|深层| 14-18 | 10-13 |架构师，RCA -每一个权衡都浮出|

根据回购规模进行调整。3个文件的CLI没有15个步骤。不应该将200个文件压缩到5个文件中。

|回购规模|推荐标准深度||-----------|---------------------------|
|小（< 20个文件）| 5-8步|
|小（20-80个文件）| 8-11个步骤|
|中等（80-300个文件）| 10-13步|
|大（300+文件）| 12-15个步骤（范围到相关子系统）|

---

写出优秀的描述——SMIG公式

每个描述应该依次回答四个问题。你不需要四段话，但是每个描述都需要这四个要素，即使是简短的。

**S -情境：读者在看什么？用一句话把它们放在上下文中。
**M -机制**：这段代码是如何工作的？是什么模式、规则或设计在起作用？
**I -暗示**：为什么这对这个角色的目标特别重要？
**G - Gotcha**：聪明的人在这里会犯什么错误？什么是不明显的、脆弱的或令人惊讶的？描述应该告诉读者一些他们无法通过自己阅读文件了解到的东西。命名模式，解释设计决策，标记故障模式，并交叉引用相关上下文。

---

##叙述弧-每一次旅行，每一个角色

1. **方向** - **必须是`file`或`directory`步长，绝不是只包含内容的
使用`"file": "README.md", "line": 1`或`"directory": "src"`，并将您的欢迎文本放在描述中。
只有内容的第一步（没有`file`、`directory`或`uri`）在VS CodeCodeTour中呈现为空白页面—这是已知的VS Code扩展行为，不可配置。

2. **高级映射**（1-3个目录或uri步骤）-主要模块及其相互关系。
不是每个文件夹——只是这个角色需要知道的。

3. **核心路径** （file/line，选择，模式，uri步骤）-重要的特定代码。
这是这次旅行的核心。阅读和叙述。不要浏览。4. **结束语**（内容）——读者现在理解的内容，他们下一步可以做什么，
建议2-3次后续参观。如果设置了`nextTour`，则在这里通过名称引用它。

###结束步骤

不要总结——读者只是阅读。相反，告诉他们现在可以做什么，应该避免什么，并建议他们进行2-3次后续参观。

---

## 20个角色

|人物|目标|必须覆盖|避免||---------|------|------------|-------|
| **Vibecoder** |获得vibe快速|入口点，请求流，主要模块。最多8步。|深度潜水，边缘情况|
| **新加入者** |结构化提升|目录、设置、业务上下文、服务边界。|高级内部组件
| **Bug修复** |根本原因快速|用户动作→触发→故障点。Repro提示+测试位置。|建筑之旅|
RCA调查员** |为什么失败|因果链，副作用，竞争条件，可观察性。|快乐的道路|
| **功能说明** |端到端| UI→API→后端→存储。特征标志，边缘情况。不相关的特性|
| **PR审稿人** |正确审查变更|变更故事，不变量，风险区域，审稿人清单。PR的URI步骤|不相关的上下文|
| **安全审查** |信任边界|认证流，输入验证，秘密处理，敏感汇。b|不相关的业务逻辑c |
| ** refacrer ** |安全重构|接缝，隐藏深度，耦合热点，安全提取顺序。|特性说明|
| **外部贡献者** |不破坏|安全区域，代码风格，架构地雷的贡献。b|深层内部b|
| **技术主管/架构师** |形状和原理|模块边界，设计权衡，风险热点。b|逐行演练---

设计一个旅游系列

当一个代码库足够复杂，以至于一个教程无法很好地涵盖它时，设计一个系列。`nextTour`字段将它们链接起来：当读者完成一次访问时，VS Code提供
自动启动下一个。

**在写任何游览之前计划好这个系列。**一个优秀的系列有：
-清晰的升级路径（宽→窄，定向→深）
-行程之间没有重复的步骤
-每个旅游独立足够有用的自己

将每个循环中的`nextTour`设置为下一个循环的`title`（必须完全匹配）。每个旅行都应该是独立的，足以独立使用。

---

## CodeTour做不到的

如果被要求使用其中任何一种，请明确表示不支持-不要提出不存在的解决方案：

|请求|现实||---|---|
| ** X秒后自动进入下一步** |不支持。导航总是手动的——读者点击下一步。在CodeTour中没有计时器、延迟或自动播放步骤机制。|
| **在步骤中嵌入视频或GIF ** |不支持。描述仅为降价文本。|
| **运行任意shell命令** |不支持。`commands`只执行VS Code命令（例如`workbench.action.terminal.focus`），不执行shell命令。|
| **分支/条件必选下一步** |不支持。旅行是线性的。`when`控制是否显示游览，而不是哪个步骤跟随哪个步骤。|
|部分内容的步骤工作，但步骤1必须有一个`file`或`directory`锚或VS Code显示一个空白页面。|

---

# #反模式

|反模式|修复||---|---|
| **文件列表** -访问文件时显示“此文件包含…”|讲故事；每一步都应该依赖于前一步
| **一般描述** |名称特定的pattern/gotcha唯一的*此*代码库|
| **行号猜测** |永远不要写入没有通过读取文件|验证的行号
| * *忽略了人格* * |每一步,不为他们的特定目标|
| **幻觉文件** |如果文件不存在，请跳过步骤|

---

质量检查表-在写入文件之前进行验证-[]每个`file`路径**相对于repo根**（没有`/`或`./`）
-[]读取并确认每个`file`路径存在
[]每个通过读取文件验证的`line`号（不是猜测）
—[]每个`directory`都是相对于回购根**的**，并且确认存在
-[]每个`pattern`正则表达式将匹配文件中的实际行
[]每个`uri`都是一个完整的，真实的URL （https://...）
—[]`ref`是一个真正的branch/tag/commit- []`nextTour`精确匹配另一个`.tour`文件的`title`-[]只创建`.tour`JSON文件-不涉及源代码
-[]第一步有一个`file`或`directory`锚（只有内容的第一步=空白页在VS Code）
-[]导览以结尾内容步骤结束，告诉读者下一步可以做什么
-[]每个描述都回答SMIG -情况，机制，暗示，明白
-[]角色的优先级驱动步骤自我选择（删去所有不符合目标的内容）
[]步数匹配请求的深度和回购大小（见校准表）
-[]最多2个只包含内容的步骤（介绍+结束）
-[]所有字段遵从`references/codetour-schema.json`格式---

##第5步：验证旅行

**总是在写入向导文件后立即运行验证器。不要跳过这一步```bash
python ~/.agents/skills/code-tour/scripts/validate_tour.py .tours/<name>.tour --repo-root .
```
验证器检查：
- JSON有效性
—每个`file`路径都存在，每个`line`路径都在文件边界内
—每个`directory`都存在
—每个`pattern`正则表达式编译并匹配文件中的至少一行
—每个`uri`都以`https://`开头
-`nextTour`匹配现有的旅游标题在`.tours/`-仅限内容的步数（如果> 2则警告）
-叙述弧（如果没有方向或结束步骤会发出警告）

**在继续之前修复每个错误。**重新运行，直到验证程序报告“✓”或仅报告“警告”。警告是建议——用你的判断。在验证通过之前，不要向用户显示游览。

**常见的VS Code问题：**只有内容的第一步呈现空白（锚定到file/directory）。绝对路径或`./`前缀路径静默失败。越界行号无法滚动。如果不能运行脚本，请手动验证：步骤1有`file`/`directory`，所有路径都存在，所有行号都有边界，`nextTour`完全匹配。

**自动播放：**`isPrimary: true`+`.vscode/settings.json`与`{ "codetour.promptForPrimaryTour": true }`提示回购打开。对于应该出现在任何分支上的行程，省略`ref`。

**共享：**对于公共repos，用户可以在`https://vscode.dev/github.com/<owner>/<repo>`打开tour，无需安装。

---

第六步：总结

写完导览后，告诉用户：
-文件路径（`.tours/<name>.tour`）
-用一段话概括这次旅行的内容和对象
-`vscode.dev`的URL，如果回购是公开的（所以他们可以立即分享它）
-建议进行2-3次后续巡回演出（如果计划进行的话，也可以进行后续巡回演出）
-任何用户请求的不存在的文件（要明确-不要悄悄替换）

---

##文件命名`<persona>-<focus>.tour`- kebab-case，通信两者：```
onboarding-new-joiner.tour
bug-fixer-payment-flow.tour
architect-overview.tour
vibecoder-quickstart.tour
pr-review-auth-refactor.tour
security-auth-boundaries.tour
concept-dependency-injection.tour
rca-login-outage.tour
```