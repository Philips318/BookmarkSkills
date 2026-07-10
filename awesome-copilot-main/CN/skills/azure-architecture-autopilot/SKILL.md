---
name: azure-architecture-autopilot
description: >
  Design Azure infrastructure using natural language, or analyze existing Azure resources
  to auto-generate architecture diagrams, refine them through conversation, and deploy with Bicep.

  When to use this skill:
  - "Create X on Azure", "Set up a RAG architecture" (new design)
  - "Analyze my current Azure infrastructure", "Draw a diagram for rg-xxx" (existing analysis)
  - "Foundry is slow", "I want to reduce costs", "Strengthen security" (natural language modification)
  - Azure resource deployment, Bicep template generation, IaC code generation
  - Microsoft Foundry, AI Search, OpenAI, Fabric, ADLS Gen2, Databricks, and all Azure services
---
# Azure架构构建器

一个使用自然语言设计Azure基础架构的管道，或分析现有资源以可视化架构并进行修改和部署。

图表引擎**嵌入在技能** （`scripts/`文件夹）中。
不需要`pip install`-它直接使用捆绑的Python脚本
生成带有605+ Azure官方图标的交互式HTML图表。
可以立即使用，无需网络访问或软件包安装。

自动用户语言检测

**🚨检测用户第一条消息的语言，并提供该语言的所有后续响应。这是最高优先级的原则-如果用户用韩语写→用韩语回复
-如果用户用英文写→**用英文回复** （ask_user，进度更新，报告，Bicep评论-全部用英文）
—本文档中的说明和示例均为英文，**所有面向用户的输出必须与用户使用的语言**匹配

**⚠️请勿将本文档中的示例逐字复制给用户
只使用结构作为参考，并根据用户的语言调整文本。

工具使用指南（GHCP环境）

|特性|工具名称|备注||---------|-----------|-------|
|获取URL内容|`web_fetch`|用于MS Docs查找等|
| Web搜索|`web_search`| URL发现|
|请求用户|`ask_user`|`choices`必须为字符串数组|
|子代理|`task`|explore/task/general-purpose|
| Shell命令执行|`powershell`| Windows PowerShell |

>所有子代理（explore/task/general-purpose）不能使用`web_fetch`或`web_search`。
需要MS Docs查找的事实检查必须**直接由主代理**执行。

外部工具路径发现`az`、`python`、`bicep`等通常不在PATH上。
**在开始一个阶段之前发现一次并缓存结果。不要每次都重新发现

b> **⚠️请勿使用`Get-Command python`** - Windows Store别名风险。
>直接文件系统发现（`$env:LOCALAPPDATA\Programs\Python`）优先。

az命令行路径：```powershell
$azCmd = $null
if (Get-Command az -ErrorAction SilentlyContinue) { $azCmd = 'az' }
if (-not $azCmd) {
  $azExe = Get-ChildItem -Path "$env:ProgramFiles\Microsoft SDKs\Azure\CLI2\wbin", "$env:LOCALAPPDATA\Programs\Azure CLI\wbin" -Filter "az.cmd" -ErrorAction SilentlyContinue | Select-Object -First 1 -ExpandProperty FullName
  if ($azExe) { $azCmd = $azExe }
}
```
Python路径+嵌入式图表引擎：参考`references/phase1-advisor.md`中的图表生成部分。

需要进度更新

使用blockquote + emoji +粗体格式：```markdown
> **⏳ [Action]** — [Reason]
> **✅ [Complete]** — [Result]
> **⚠️ [Warning]** — [Details]
> **❌ [Failed]** — [Cause]
```
平行预紧原理

在通过`ask_user`等待用户输入时，并行地预加载下一步所需的信息。

| ask_user问题| Preload同时||---|---|
|项目名称/扫描范围|参考文件，MS Docs， Python路径发现，**图模块路径验证** |
|Model/SKU选择| MS文档下一个问题的选择|
|架构确认|`az account show/list`，`az group list`|
|订阅选择|`az group list`|

---

路径分支-根据用户请求自动确定

路径A：新设计（新构建）

* *引发* *:“创造”、“设置”、“部署”,“构建”,等等。```
Phase 1 (references/phase1-advisor.md) — Interactive architecture design + diagram
    ↓
Phase 2 (references/bicep-generator.md) — Bicep code generation
    ↓
Phase 3 (references/bicep-reviewer.md) — Code review + compilation verification
    ↓
Phase 4 (references/phase4-deployer.md) — validate → what-if → deploy
```
路径B：现有分析+修改（分析和修改）

**触发：“分析”，“当前资源”，“扫描”，“绘制图表”，“显示我的基础设施”等。```
Phase 0 (references/phase0-scanner.md) — Existing resource scan + diagram
    ↓
Modification conversation — "What would you like to change here?" (natural language modification request → follow-up questions)
    ↓
Phase 1 (references/phase1-advisor.md) — Confirm modifications + update diagram
    ↓
Phase 2~4 — Same as above
```
当路径确定是模糊的

直接询问用户：```
ask_user({
  question: "What would you like to do?",
  choices: [
    "Design a new Azure architecture (Recommended)",
    "Analyze + modify existing Azure resources"
  ]
})
```
---

##相变规则

—每个阶段读取并遵循其相应的`references/*.md`文件中的说明
-在阶段之间转换时，总是通知用户下一步该做什么
-不要跳过阶段（特别是阶段3→阶段4之间的假设）
- **🚨阶段1→阶段2过渡所需条件**:`01_arch_diagram_draft.html`必须使用嵌入式图表引擎生成并显示给用户。**不要在没有图表的情况下继续生成二头肌。**单独完成规格收集并不意味着第一阶段已经完成——第一阶段包括图表生成+用户确认。
-部署后的修改请求→返回阶段1，而不是阶段0 （Delta确认规则）

##服务覆盖和后备

优化服务
Microsoft Foundry, Azure OpenAI, AI Search, ADLS Gen2, Key Vault, Microsoft Fabric, Azure Data Factory,VNet/PrivateEndpoint,AML/AIHub其他Azure服务
所有支持的- MS文档自动咨询，以产生相同的质量标准。
**不要发送引起用户焦虑的信息，如“超出范围”或“尽力而为”

稳定与动态信息处理

|类别|处理方法|样例||----------|----------------|---------|
| **稳定** |参考文件首先|`isHnsEnabled: true`， PE triple set |
| **动态** | **始终获取MS Docs** | API版本，型号可用性，SKU，区域|

##快速参考

|文件|角色||------|------|
|`references/phase0-scanner.md`|现有资源扫描+关系推断+图|
|`references/phase1-advisor.md`|交互式架构设计+事实检查|
|`references/bicep-generator.md`|二头肌代码生成规则|
|`references/bicep-reviewer.md`|代码审查清单|
|`references/phase4-deployer.md`|验证→假设→部署|
|`references/service-gotchas.md`|所需属性，PE映射|
|`references/azure-dynamic-sources.md`| MS Docs URL注册表|
|`references/azure-common-patterns.md`|PE/security/naming模式|
|`references/ai-data.md`|AI/Data业务指南|