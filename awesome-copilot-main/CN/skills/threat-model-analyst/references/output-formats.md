#输出格式-报告文件模板

⛔**自我纠正指令：**使用本文档中的模板编写任何文件后，立即运行底部的自检部分。您对编排者的回复必须包括填满清单的✅/❌为每个项目。如果ANY项为❌，请先修复文件，再进行下一步。

该文件定义了威胁模型分析师生成的每个输出文件的结构和内容。每个部分都包含模板、规则和验证检查表。

**图表约定**在单独的文件中：[diagram-conventions.md]（./diagram-conventions.md）
**分析方法**在单独的文件：[analysis-principles.md]（./analysis-principles.md）

---

##输出文件夹

在分析开始时创建一个带时间戳的文件夹：
—格式：`threat-model-YYYYMMDD-HHmmss`（UTC时间）
—例如：`threat-model-20260130-073845`—将所有输出文件写入此文件夹

---

文件内容格式化-关键规则**永远不要在代码栏中包装`.md`文件内容。**使用`create_file`或`edit_file`时：
—该工具将原始内容写入磁盘。如果在开始时包含` `'`markdown `，它将成为文件中的文本。
- **错误**：内容以` `'`markdown `开头-文件将包含栅栏作为文本
- **正确**：内容直接以第一行的`# Heading`开头
-这适用于所有`.md`文件：`0.1-architecture.md`，`0-assessment.md`,`1-threatmodel.md`,`2-stride-analysis.md`,`3-findings.md`**永远不要在代码栏中包装`.mmd`文件内容。**`.mmd`文件是原始美人鱼的源代码：
- **错误**：内容以` `'`plaintext `或` `'`mermaid `开头
- **正确**：第一行以`%%{init:`开头，第二行以`flowchart`或`graph`开头

**每次写文件前自检：**查看内容的第一个字符。如果是` `'` `-停止并移除围栏。

---

##文件列表|文件|描述| Always？||------|-------------|---------|
|`0-assessment.md`|执行摘要、风险评级、行动计划、元数据|是|
|`0.1-architecture.md`|架构概述、组件、场景、技术栈|是|
|`1-threatmodel.md`|威胁模型DFD图+element/flow/boundary表|是|
|`1.1-threatmodel.mmd`|纯美人鱼DFD（详细图表的真相来源）|是|
|`1.2-threatmodel-summary.mmd`|汇总DFD（仅限>15个元素或>4个边界）|条件必选|
|`2-stride-analysis.md`|所有组件全STRIDE-A分析|是|
|`3-findings.md`|优先处理安全发现和修复|是|
|`threat-inventory.json`|用于比较匹配|的结构化JSON目录是|
|`incremental-comparison.html`|可视化HTML比较报告（仅支持增量模式）|条件必选|

---## 0.1-architecture.md
**目的：**高级架构概述-首先生成，在威胁建模开始之前。

**何时生成：**每次运行。没有条件。

**图表：**所有内联美人鱼在降价。没有单独的`.mmd`文件为0.1-architecture.md。

内容结构```markdown
# Architecture Overview

## System Purpose
<!-- 2-4 sentences: What is this system? What problem does it solve? Who are the users? -->

## Key Components
| Component | Type | Description |
|-----------|------|-------------|
| [Name] | [Process / Data Store / External Service / External Interactor] | [One-line role description] |

## Component Diagram
<!-- Architecture diagram using service/external/datastore classDef (NOT DFD circles). See diagram-conventions.md for styles. -->

## Top Scenarios
<!-- 3-5 most important workflows. First 3 MUST include sequence diagrams. -->

### Scenario 1: [Name]
[2-3 sentence description]
<!-- Mermaid sequenceDiagram here -->

### Scenario 2: [Name]
### Scenario 3: [Name]

## Technology Stack
| Layer | Technologies |
|-------|--------------|
| Languages | ... |
| Frameworks | ... |
| Data Stores | ... |
| Infrastructure | ... |
| Security | ... |

## Deployment Model
<!-- How deployed? On-prem, cloud, hybrid? Containers, VMs? -->

## Security Infrastructure Inventory
| Component | Security Role | Configuration | Notes |
|-----------|---------------|---------------|-------|
| [e.g., MISE Sidecar] | [e.g., Authentication proxy] | [e.g., Entra ID OIDC] | [e.g., All API pods] |

## Repository Structure
| Directory | Purpose |
|-----------|---------|
| [path/] | [Contents] |
```
处理规则1. 在**创建威胁模型图之前生成**
2. 从代码分析中获得所有内容-不要推测
3. 如果无法确定某个节，则应显式声明
4. 目标：**150-250行**最低之前的迭代只能生成100-150行，太细了。包括详细的组件描述、port/protocol信息和实质性的场景叙述。
5. 关键组件表应与威胁模型图元素保持一致
6. 使用**架构**图样式（不是DFD） -参见`diagram-conventions.md`7. 写入后，验证每个Mermaid块具有有效的语法
8. **顶级场景**：前3个场景必须包括美人鱼`sequenceDiagram`块显示交互流程。每个序列图应该显示实际的参与者、带有协议详细信息的消息，以及用于错误路径的alt/opt块。
9. **组件对齐**：在关键组件中列出的每个组件必须稍后显示为一个节On in`2-stride-analysis.md`10. **部署模型**：必须包括具体细节：端口、协议、绑定地址、网络暴露和部署拓扑（单机/集群/多层）
11. **安全基础设施清单**：填充代码中发现的每个安全相关组件（身份验证，加密，访问控制，日志记录，秘密管理）---

##1-threatmodel.md+ 1.1-threatmodel.mmd

**目的：**系统威胁模型作为数据流图（DFD）。

生成步骤

**步骤1:**创建`1.1-threatmodel.mmd`（source of truth）
-纯美人鱼代码，没有降价包装
-使用来自`diagram-conventions.md`的DFD形状和样式

**步骤2:**从`orchestrator.md`步骤4运行POST-DFD GATE评估并创建`1.2-threatmodel-summary.mmd`，如果阈值满足。模板请参见`skeletons/skeleton-summary-dfd.md`。

**步骤3:**创建`1-threatmodel.md`（如果生成了摘要，包括摘要视图部分）

###1-threatmodel.md内容```markdown
# Threat Model

## Data Flow Diagram
<!-- Copy EXACT diagram from 1.1-threatmodel.mmd wrapped in ```mermaid fence -->

## Element Table
| Element | Type | TMT Category | Description | Trust Boundary |
|---------|------|--------------|-------------|----------------|

- **Type** = high-level DFD category: `Process`, `External Interactor`, or `Data Store`
- **TMT Category** = specific TMT ID from tmt-element-taxonomy.md §1 (e.g. `SE.P.TMCore.WebSvc`, `SE.EI.TMCore.Browser`, `SE.DS.TMCore.SQL`)
- For Kubernetes-based applications where pods run sidecars, add an optional **Co-located Sidecars** column (e.g. `MISE, Dapr` or `—`)

## Data Flow Table
| ID | Source | Target | Protocol | Description |
|----|--------|--------|----------|-------------|

## Trust Boundary Table
| Boundary | Description | Contains |
|----------|-------------|----------|

## Summary View (only if summary diagram generated)
<!-- Copy from 1.2-threatmodel-summary.mmd -->

## Summary to Detailed Mapping
| Summary Element | Contains | Summary Flows | Maps to Detailed Flows |
```
* *主要规则:* *
-`.mmd`和`.md`中的图表必须相同（复制，不要重新生成）
-详细流程使用`DF01`、`DF02`；`SDF01`，`SDF02`为汇总流

---## 2-stride-analysis.md
**目的：**全面跨步+滥用案例威胁分析每个组件。

结构要求

1. 每个组件的威胁**必须分成一级，二级，三级子节**与单独的表
2. 汇总表**必须包含T1， T2， T3列**
3. 所有三个层子部分出现在每个组件中（即使为空-使用“*未识别N层威胁*”）

锚地安全标题（临界）

组件`## `标题成为`3-findings.md`的链接目标。
—只能使用字母、数字、空格和连字符
- **禁止在标题中出现：**`&`、`/`、`(`、`)`、`.`、`:`、`'`、`+`、`@`、`!`-替换：`&`→`and`，`/`→`-`，括号→去掉

锚规则：**标题→小写，空格→连字符，除连字符外的非字母数字。

# # #模板关键：`## Summary`表必须出现在文件的顶部，紧接在`## Exploitability Tiers`之后和任何单独的`## Component`段之前。它是导航辅助工具——读者首先需要它。模型总是把它移到底部——这是错误的。按照这个顺序：`# STRIDE + Abuse Cases — Threat Analysis`→`## Exploitability Tiers`→`## Summary`→`---`→`## Component 1`→`## Component 2`→…**

> **⛔严格的层定义-准确地应用这些。不要使用主观判断。**这是一个技能指令-不要将这一行复制到输出中。下面的层级表是报告中的内容，没有这个指令行。

> **⛔泄露指令检查：**输出文件必须不包含文本“刚性层定义”，“不要使用主观判断”，或任何行以`⛔`开始。这些是技能指导，而不是报告内容。如果在输出中看到它们，请在完成之前删除它们。```markdown
# STRIDE + Abuse Cases — Threat Analysis

## Exploitability Tiers

Threats are classified into three exploitability tiers based on the prerequisites an attacker needs:

| Tier | Label | Prerequisites | Assignment Rule |
|------|-------|---------------|----------------|
| **Tier 1** | Direct Exposure | `None` | Exploitable by unauthenticated external attacker with NO prior access. The prerequisite field MUST say `None`. |
| **Tier 2** | Conditional Risk | Single prerequisite: `Authenticated User`, `Privileged User`, `Internal Network`, or single `{Boundary} Access` | Requires exactly ONE form of access. The prerequisite field has ONE item. |
| **Tier 3** | Defense-in-Depth | `Host/OS Access`, `Admin Credentials`, `{Component} Compromise`, `Physical Access`, or MULTIPLE prerequisites joined with `+` | Requires significant prior breach, infrastructure access, or multiple combined prerequisites. |
```
> **⛔逐字复制层表。**第四列必须是`Assignment Rule`（不是`Example`，`Description`,`Criteria`，或任何其他名称）。单元格值必须与上面的文本完全相同——不要用特定于部署的示例替换它们。不要在表后添加“影响层分配的部署上下文”段落——部署上下文属于单个组件部分，而不是层定义。> **⛔STRIDE-A类别标签（强制-“A”是“滥用”，而不是“授权”）：**
在所有表（Summary， per-component Tier表，threat-inventory.json）中使用的7个STRIDE-A类别是：
> **S**poofing | **T**篡改| **R**认证| **I**信息披露| **D**服务终端| **E**权限提升| **A**总线
>“滥用”包括：业务逻辑滥用、工作流操纵、功能误用、无意中使用合法功能。
>模型经常为A列生成“Authorization”——这是错误的。如果您在任何地方看到“授权”作为STRIDE类别标签，请将其替换为“滥用”。威胁行中的类别列必须显示“滥用”（而不是“授权”）。N/A条目还必须显示“滥用-N/A”（而不是“授权-N/A”）。

# #总结
|组件|链路| S | T | R | I | D | E | A |总| T1 | T2 | T3 |风险||-----------|------|---|---|---|---|---|---|---|-------|----|----|----|------|
---

##组件名称

**信任边界：**[边界名称]
**角色：**【简介】
**数据流：** [DF id列表]
**Pod Co-location:** [sidecars if K8s - seediagram-conventions.md]

STRIDE-A分析

b> **⛔类别命名：7个STRIDE-A类别是：欺骗，篡改，拒绝，信息披露，拒绝服务，提升特权，滥用。“A”类总是“滥用”，而不是“授权”。授权问题属于特权提升(E)。这适用于N/A辩护标签、威胁表类别列和所有散文

####一级-直接暴露（无先决条件）
| ID |类别|威胁|先决条件|受影响流量|缓解|状态||----|----------|--------|---------------|---------------|------------|--------|
####二级-条件风险
| ID |类别|威胁|先决条件|受影响流量|缓解|状态|

####三级纵深防御
| ID |类别|威胁|先决条件|受影响流量|缓解|状态|```

**⛔ STRIDE Status Column — Valid Values (must match Coverage table):**
The `Status` column in each threat row MUST use exactly one of these values:
- `Open` — Threat is not mitigated; MUST map to a finding (`✅ Covered` in Coverage table). The finding documents the vulnerability and remediation guidance.
- `Mitigated` — Threat is mitigated by the engineering team's own code, configuration, or design decisions in THIS repository. Maps to `✅ Mitigated (FIND-XX)` in Coverage table. A finding MUST be created that documents WHAT the team did, WHERE in the code, and HOW it mitigates the threat. This gives credit to the engineering team for security work they've already done.
- `Platform` — Threat is mitigated by an EXTERNAL platform that is NOT part of the analyzed codebase. See strict definition below. Maps to `🔄 Mitigated by Platform` in Coverage table. NO finding is created — the mitigation is outside this team's control.

**How to distinguish Mitigated vs Platform:**
| Question | If YES → | If NO → |
|----------|----------|---------|
| Is the mitigation implemented in code within THIS repository? | `Mitigated` | Check next |
| Is the mitigation in deployment config controlled by THIS team? | `Mitigated` | Check next |
| Is the mitigation provided by a completely external system? | `Platform` | `Open` (no mitigation exists) |

**Examples of `Mitigated` (team's own work — create finding to document it):**
- Auth middleware validating JWT tokens — the team wrote this code
- TLS certificate generation and configuration — the team implemented this
- File permissions set to 0600 in the code — the team chose secure defaults
- Input validation or sanitization functions — the team built defenses
- Rate limiting middleware — the team added throttling
- Localhost-only binding — the team made an architectural security decision

**The finding for a `Mitigated` threat documents the existing control:**
- Title: descriptive of what IS in place (e.g., "JWT Authentication Middleware on API Endpoints")
- Severity: Low (existing control) or Moderate (if control has gaps)
- Mitigation Type: `Existing Control`
- Remediation section: describes what's already implemented + any hardening recommendations
- This ensures the Coverage table shows the team's security work, not just gaps

**⛔ STRICT DEFINITION OF "PLATFORM" (MANDATORY):**
`Platform` status is ONLY valid when ALL of these conditions are true:
1. The mitigation is provided by a system **completely outside** the analyzed repository's code
2. The mitigation is **managed by a different team/organization** (e.g., Azure AD is managed by Microsoft Identity team, not by this repo's team)
3. The mitigation **cannot be disabled or weakened** by modifying code in this repository

**Examples of LEGITIMATE Platform mitigations:**
- Azure AD token signing (managed by Microsoft Identity, not this code)
- K8s RBAC (managed by K8s control plane, not this operator)
- Azure Arc tunnel encryption (managed by Arc team, not this agent)
- TPM hardware security (hardware, not software)

**Examples of things that are NOT "Platform" — they are `Mitigated` (team's work):**
- ✅ "Auth middleware on endpoints" → `Mitigated` — team wrote the auth code. Create finding documenting it.
- ✅ "TLS on localhost" → `Mitigated` — team implemented TLS. Create finding documenting the implementation.
- ✅ "File permissions 0600" → `Mitigated` — team set secure defaults. Create finding documenting the choice.
- ✅ "Localhost binding" → `Mitigated` — team made architectural security decision. Create finding.
- ✅ "Input validation" → `Mitigated` — team built defense. Create finding documenting what's validated.
- ✅ "Operation state machine" → `Mitigated` — team's logic prevents abuse. Create finding.

**⛔ MAXIMUM PLATFORM RATIO:** If more than 20% of threats are classified as "🔄 Mitigated by Platform", re-examine each. Many should be `Mitigated` (team's code) not `Platform` (external). In a typical application, 5-15% are genuinely platform-mitigated, 20-40% are mitigated by the team's own code, and the rest are `Open` (needing remediation).

**⛔ NEVER use these values:**
- ❌ `Partial` — ambiguous. If partially mitigated, it's `Open` (the remaining gap is the finding)
- ❌ `N/A` — every threat is applicable if it's in the table
- ❌ `Accepted` — the tool does not accept risks
- ❌ `Needs Review` — every threat must be either Covered, Mitigated, or Platform

**Consistency rule:** The STRIDE `Status` column and the Findings Coverage table `Status` MUST agree:
| STRIDE Status | Coverage Table Status | Meaning |
|---|---|---|
| `Open` | `✅ Covered (FIND-XX)` | Finding documents a vulnerability needing remediation |
| `Mitigated` | `✅ Mitigated (FIND-XX)` | Finding documents an existing control the team built — gives credit for security work |
| `Platform` | `🔄 Mitigated by Platform` | External platform handles it — no finding needed |

**⛔ "Accepted Risk" and "Needs Review" are FORBIDDEN.** The tool does NOT have authority to accept risks or defer threats. Every threat maps to either a finding (Covered or Mitigated) or a genuine external platform mitigation. There is no middle ground.

### Arithmetic Verification (MANDATORY)

After writing ALL component tables:
1. Count actual threat rows per component per category (S,T,R,I,D,E,A) — compare with summary table
2. Verify Total = S+T+R+I+D+E+A for each row
3. Verify T1+T2+T3 = Total for each row
4. Verify Totals row = column-wise sum
5. Row count cross-check: threat rows in detail = Total in summary

---

## 3-findings.md

**Purpose:** Prioritized security findings with evidence and remediation.

> **⛔ IMPORTANT: Before writing this file, read [skeleton-findings.md](./skeletons/skeleton-findings.md) and copy the skeleton VERBATIM for each finding. Fill in the `[FILL]` placeholders. This prevents template drift.**

### Structure Requirements

Organized by **Exploitability Tier** (NOT by severity):
1. `## Tier 1 — Direct Exposure (No Prerequisites)`
2. `## Tier 2 — Conditional Risk (Authenticated / Single Prerequisite)`
3. `## Tier 3 — Defense-in-Depth (Prior Compromise / Host Access)`

**DO NOT** use `## Critical Findings`, `## Important Findings`, etc.
Sort by severity **within** each tier, then by CVSS descending.

**Tier Assignment for Findings:**
- A finding's tier is determined by its `Exploitation Prerequisites` value, using the same rules as STRIDE-A tier assignment (see [analysis-principles.md](./analysis-principles.md))
- If a finding covers threats from multiple tiers (via Related Threats), assign it to the **highest-priority tier** (lowest tier number) among its related threats

**Ordering within each tier:** Sort findings by:
1. **SDL Bugbar Severity** descending: Critical → Important → Moderate → Low
2. **Within each severity band**, sort by CVSS 4.0 score descending (highest first)

**After writing all findings**, verify the sort order:
- List all findings with their severity, CVSS score, and tier
- Confirm no finding with higher CVSS appears after a lower CVSS finding within the same severity band and tier
- If misordered, renumber and reorder before finalizing

**Finding ID Numbering — MUST be sequential:**
- Use `FIND-01`, `FIND-02`, `FIND-03`, ... only. `F-01`, `F01`, or `Finding 1` formats are NOT allowed.
- IDs MUST appear in order in the document: FIND-01 before FIND-02 before FIND-03, etc.
- ❌ NEVER have FIND-06 appear before FIND-04 in the document. If reordering findings, renumber ALL IDs to maintain sequential order.
- After final sort, scan the document top-to-bottom: the first finding heading must be FIND-01, the next FIND-02, etc. No gaps, no out-of-order.

### Finding Attributes (ALL MANDATORY)

| Attribute | Description |
|-----------|-------------|
| SDL Bugbar Severity | Critical / Important / Moderate / Low |
| CVSS 4.0 | Score AND full vector string (e.g., `9.3 (CVSS:4.0/AV:N/AC:L/AT:N/PR:N/UI:N/VC:H/VI:H/VA:H/SC:N/SI:N/SA:N)`) — BOTH are mandatory |
| CWE | ID, name, AND hyperlink (e.g., `[CWE-306](https://cwe.mitre.org/data/definitions/306.html): Missing Authentication for Critical Function`) |
| OWASP | Top 10:2025 mapping (A01:2025 format — never :2021) |
| Exploitation Prerequisites | From tier definitions |
| Exploitability Tier | Tier 1 / Tier 2 / Tier 3 |
| Remediation Effort | Low / Medium / High |
| Mitigation Type | Redesign / Standard Mitigation / Custom Mitigation / Existing Control / Accept Risk / Transfer Risk |
| Component | Affected component |
| Related Threats | Individual links to `2-stride-analysis.md#component-anchor` |

### Full Finding Example

```markdown
### FIND-01: API缺少认证

|属性|值||-----------|-------|
| SDL错误级别|严重|
CVSS 4.0 | 9.3 (CVSS:4.0/AV:N/AC:L/AT:N/PR:N/UI:N/VC:H/VI:H/VA:H/SC:N/SI:N/SA:N) |
| CWE | [CWE-306](https://cwe.mitre.org/data/definitions/306.html): Missing Authentication for Critical Function
| OWASP | a7:2025 -认证失败
|攻击前提条件|无（外部攻击者）|
|可利用性层|层1 -直接暴露|
|修复力度|中等|
|缓解类型|标准缓解|
|组件| API网关|
|相关威胁| [T01. 01](2-stride-analysis.md# api网关),[T01。R] (2-stride-analysis.md# api网关)|

# # # #描述

API端点/api/v1/resources接受请求而不进行任何身份验证检查…

# # # #的证据

第45行-控制器上没有`[Authorize]`属性。

# # # #补救

将`[Authorize]`属性添加到控制器类中，并在`Program.cs`中配置JWT承载身份验证。

# # # #验证发送一个未经认证的GET请求到`/api/v1/resources`-应该返回401 Unauthorized。```

### Related Threats Link Format

> **⛔ CRITICAL: Related Threats MUST be hyperlinks, NOT plain text. The model consistently outputs plain text like `T-02, T-17` — this is WRONG. Each threat ID must link to the specific component section in stride analysis.**

- Individual links per threat ID: `[T01.S](2-stride-analysis.md#component-name)`
- **WRONG**: `T-02, T-17, T-23` (plain text, no links)
- **WRONG**: `[T08.S, T08.T](2-stride-analysis.md)` (grouped, no anchor)
- **CORRECT**: `[T08.S](2-stride-analysis.md#redis-state-store), [T08.T](2-stride-analysis.md#redis-state-store)`
- Every `| **Related Threats** |` cell must contain ONLY `[Txx.Y](2-stride-analysis.md#anchor)` format links separated by commas

### Post-Write Checks

1. **Anchor spot-check**: Verify 3+ Related Threats links resolve to real `##` headings
2. **Threat coverage check**: Every threat ID in `2-stride-analysis.md` must be referenced by at least one finding
3. **Sort order check**: Within each tier, no higher-CVSS finding appears after a lower-CVSS finding in the same severity band
4. **CVSS-to-Tier consistency**: Scan every finding — if CVSS has `AV:L` or `PR:H`, finding MUST NOT be in Tier 1. Fix by downgrading the tier, not by changing the CVSS.
5. **Threat Coverage Verification table**: At the end of `3-findings.md`, include:

```markdown
威胁覆盖验证

|威胁ID |查找ID |状态||-----------|------------|--------|
| T01。S | FIND-01 |✅Covered |
| T01。T | FIND-05 |✅缓解（团队实现TLS） |
| T02。I | - |🔄由平台（Azure AD） |缓解```

Every threat from `2-stride-analysis.md` must appear in this table. Status is one of:
- `✅ Covered (FIND-XX)` — finding documents a vulnerability that needs remediation
- `✅ Mitigated (FIND-XX)` — finding documents an existing control the team built (gives credit for security work done)
- `🔄 Mitigated by Platform` — external system handles it (only for genuinely external platforms)

**⛔ THIS TABLE IS A FEEDBACK LOOP, NOT DOCUMENTATION:**
The purpose of this table is to force you to check your work. After filling it out:
1. If ANY threat has a `—` dash in the Finding ID column with status other than `🔄 Mitigated by Platform` → **you missed a finding. Go back and create one.**
2. If Platform count > 20% of total threats → **you are overusing Platform as an escape hatch. Re-examine.**
3. If any threat is listed as `⚠️ Accepted Risk` or `⚠️ Needs Review` → **VIOLATION. Create a finding or verify it's genuinely Platform.**

The table should drive you to 100% coverage: every threat maps to either a finding (`✅ Covered`) or a legitimate external platform mitigation (`🔄 Mitigated by Platform`). There is no third option.

**⛔ FINDING GENERATION RULE:**
If a threat in `2-stride-analysis.md` has a non-empty `Mitigation` column, it MUST become a finding. The mitigation text provides the remediation — use it. The only exception is threats genuinely mitigated by an EXTERNAL platform (Azure AD, K8s RBAC, TPM hardware) that this code cannot disable.

---

## 0-assessment.md

**Purpose:** Executive summary, risk rating, action plan, and metadata. The "front page" of the report.

> **⛔ IMPORTANT: Before writing this file, read [skeleton-assessment.md](./skeletons/skeleton-assessment.md) and copy the skeleton VERBATIM. Fill in the `[FILL]` placeholders. This prevents template drift.**

### Section Order (MANDATORY — ALL 7 sections REQUIRED, do NOT skip any)

1. **Report Files** (REQUIRED) — Links to all report deliverables
2. **Executive Summary** (REQUIRED) — Risk rating + coverage. NO separate "Key Recommendations" subsection
3. **Action Summary** (REQUIRED) — Tier-based prioritized action plan with `### Quick Wins` subsection
4. **Analysis Context & Assumptions** (REQUIRED) — Scope, infrastructure context, `### Needs Verification` table, finding overrides
5. **References Consulted** (REQUIRED) — Security standards + component documentation
6. **Report Metadata** (REQUIRED) — Model, timestamps, duration, git info
7. **Classification Reference** (REQUIRED) — MUST be the last section. Static table copied from skeleton.

⚠️ **Enforcement:** If a section has no data, include it with empty tables or "N/A" notes — NEVER omit the section entirely. The agent in previous iterations skipped sections 1, 4, 5, and 6 entirely. ALL SEVEN must be present.

### Report Files Template

The Report Files table MUST list `0-assessment.md` (this file) as the FIRST row, followed by the other files:

```markdown
##报告文件

|文件|描述| . ||------|-------------|
| [0-assessment.md](0-assessment.md) |本文件-执行摘要、风险评级、行动计划、元数据|
| [0.1-architecture.md](0.1-architecture.md) |架构概述、组件、场景、技术栈|
| [1-threatmodel.md](1-threatmodel.md) |带有元素表、流表和边界表的威胁模型DFD图|
| [1.1-threatmodel.mmd](1.1-threatmodel.mmd) |纯美人鱼DFD源文件|
| [1.2-threatmodel-summary.mmd](1.2-threatmodel-summary.mmd) | Summary DFD（仅当生成时）|
| [2-stride-analysis.md](2-stride-analysis.md) |所有成分全STRIDE-A分析|
| [3-findings.md](3-findings.md) |优先处理安全发现和修复|```

⚠️ **`0-assessment.md` MUST be the first row.** The model consistently lists `0.1-architecture.md` first — that is WRONG. This file IS the front page of the report and lists itself first.

### Risk Rating

The heading must be plain text with NO emojis: `### Risk Rating: Elevated`, NOT `### Risk Rating: 🟠 Elevated`

### Threat Count Context Paragraph

Include at end of Executive Summary:

```markdown
**关于威胁数量的说明：**该分析在[M]个组件中识别了[N]个威胁。这个数字反映了全面的STRIDE-A覆盖范围，而不是系统性的不安全。其中，**[T1计数]是直接利用**没有先决条件（1级）。其余的[T2+T3计数]代表有条件的风险和纵深防御的考虑。```

### Action Summary Template

> **⛔ FIXED PRIORITY MAPPING — The Priority column values are DETERMINISTIC, not judgment-based:**
> | Tier | Priority | Always |
> |------|----------|--------|
> | Tier 1 | 🔴 Critical Risk | ALWAYS — regardless of threat/finding count |
> | Tier 2 | 🟠 Elevated Risk | ALWAYS — regardless of threat/finding count |
> | Tier 3 | 🟡 Moderate Risk | ALWAYS — regardless of threat/finding count |
>
> **NEVER change the priority based on how many threats or findings exist in that tier.** Even if Tier 1 has 0 threats and 0 findings, the priority is still 🔴 Critical Risk — because IF a Tier 1 threat existed, it would be critical. The priority reflects the tier's inherent severity, not the count. A report with Tier 1 = "🟢 Low Risk" is WRONG and must be fixed.

```markdown
##行动总结

|分级|描述|威胁|发现|优先级||------|-------------|---------|----------|----------|
|一级|可直接利用| 5 | 3 |🔴严重风险|
|二级|需要认证访问| 8 | 4 |🟠高风险|
|三级|需要事先妥协| 12 | 5 |🟡中度风险|
| **总** | | **25** | **12** | |```

> **⛔ EXACTLY 4 ROWS: The Action Summary table MUST have exactly 4 data rows: Tier 1, Tier 2, Tier 3, and Total. Do NOT add rows for "Mitigated", "Platform", "Fixed", "Accepted", or any other status. Mitigated threats are distributed across their respective tiers — they are NOT a separate tier. If you find yourself adding a "Mitigated" row, STOP and remove it.**

```markdown
###快速获胜<!-- Tier 1 findings with Low remediation effort — high impact, quick fixes -->
|寻找|标题|为什么快速||---------|-------|-----------|
| FIND-XX | [title] | [reason] |```

⚠️ **Quick Wins is a REQUIRED subsection.** The `### Quick Wins` heading and table MUST appear after the tier summary table inside Action Summary. If no low-effort findings exist, write: `### Quick Wins\n\nNo low-effort findings identified. All findings require Medium or High effort.`

**Processing Rules for Action Summary:**
1. Populate the tier table with actual counts from `3-findings.md` (findings per tier) and `2-stride-analysis.md` (threats per tier from T1/T2/T3 columns in summary table)
2. Quick Wins lists only Tier 1 findings with `Remediation Effort: Low` — highest-impact, lowest-effort items
3. If no Tier 1 Low-effort findings exist, show Tier 2 Low-effort findings instead, with a note: "No Tier 1 quick wins identified. These Tier 2 items offer the best effort-to-impact ratio:"
4. If no Low-effort findings exist at all, keep `### Quick Wins` heading and add: `No low-effort findings identified. All findings require Medium or High effort.`
5. Verify: Findings column sums must equal total findings count in `3-findings.md`
6. Verify: Threats column sums must equal total threats count in `2-stride-analysis.md` summary table

### ⛔ PROHIBITED Content in Action Summary and All Output Files

**NEVER generate ANY of the following:**
- `### Priority Remediation by Phase` or any phase-based remediation roadmap
- Sprint references (`Sprint 1-2`, `Sprint 3-4`, etc.)
- Time-based phases (`Phase 1 — Immediate`, `Phase 2 — Short-term`, `Phase 3 — Medium-term`, `Phase 4 — Long-term`, `Backlog`)
- Time-to-fix estimates (`~1 hour`, `~2 hours`, `~4 hours`, `1-2 days`, etc.)
- Timeline or scheduling language (`immediately`, `next quarter`, `within 30 days`, `addressed within`)
- Effort duration labels (`(hours)`, `(days)`, `(weeks)`) after Low/Medium/High effort levels

**The report identifies WHAT to fix and WHY (tier + severity + effort level). It does NOT prescribe WHEN to fix it.** Scheduling is the team's responsibility. Only use `Low`, `Medium`, `High` for remediation effort — never attach time durations.

### Analysis Context & Assumptions Template

⚠️ **This ENTIRE section is REQUIRED.** Previous iterations skipped it entirely. Include ALL sub-sections below, even if tables are empty.

```markdown
分析背景和假设

分析范围
|约束条件|描述信息||------------|-------------|
|范围|[全部回购或特定区域]|
|排除|[排除的内容]|
|[特别关注，如果有的话]|

###基础设施上下文
|类别|从代码库发现|发现影响||----------|--------------------------|-------------------|
**“从代码库中发现”的每个条目必须包含一个指向源文件或文档的相对链接，从这些文件或文档中推断出信息。* *的例子:```
| Deployment Model | Air-gapped, single-admin workstation ([daemon.json](src/Container/Moby/daemon.json), [InstallAzureEdgeDiagnosticTool.ps1](src/Setup/InstallArtifacts/InstallAzureEdgeDiagnosticTool.ps1)) | All findings — no Tier 1 |
| Network Exposure | All services bind to localhost:80 only ([KustoContainerHelper.psm1](src/Container/Kusto/KustoContainerHelper.psm1)) | FIND-01, FIND-03 |
```
需要验证
|项目|问题|检查什么|为什么不确定||------|----------|---------------|---------------|
###查找覆盖
|查找ID |原始级别|覆盖|理由|新状态||------------|-------------------|----------|---------------|------------|
| - | - | - |没有覆盖应用。检查后更新此部分。| - |

###附加说明<!-- Any other context from the user's prompt -->
[用户提供的自由格式注释]```

### References Consulted Template

> **⛔ CRITICAL: This section MUST have TWO subsections with THREE-column tables including full URLs. Do NOT flatten into a simple 2-column `| Reference | Usage |` table. The model ALWAYS tries to simplify this — do NOT simplify it.**

```markdown
参考文献

安全标准
|标准| URL |如何使用||----------|-----|----------|
|微软SDL漏洞栏|https://www.microsoft.com/en-us/msrc/sdlbugbar|严重性分类|
| OWASP Top 10:2025 |https://owasp.org/Top10/2025/|威胁分类|
| CVSS 4.0 |https://www.first.org/cvss/v4.0/specification-document|风险评分|
| CWE |https://cwe.mitre.org/|弱点分级|
| STRIDE |https://learn.microsoft.com/en-us/azure/security/develop/threat-modeling-tool-threats|威胁枚举方法
| NIST SP 800-53 Rev. 5 |https://csrc.nist.gov/pubs/sp/800-53/r5/upd1/final|控制映射|

组件文档
|组件|文档URL |相关章节||-----------|------------------|------------------|
(例如,Dapr) | |(例如,https://docs.dapr.io/operations/security/) | |(例如,mtl配置)
(例如,复述)| |(例如,https://redis.io/docs/management/security/) | |(例如,身份验证)```

**Processing Rules:**
1. Always include the Security Standards table — populate with actual standards consulted
2. Every row MUST have a full URL (https://...) — never omit the URL column
3. Populate Component Documentation with technologies actually consulted during analysis
4. Do not add documentation that was not used

### Report Metadata Template

> **⛔ CRITICAL: ALL fields below are MANDATORY. Do NOT skip Model, Analysis Started, Analysis Completed, or Duration. The previous run omitted these — that is a critical failure. Run `Get-Date` at start and end to compute Duration.**

```markdown
##报告元数据

|字段|值||-------|-------|
|源位置|`[Full path]`|
| Git Repository |`[Remote URL or "Unavailable"]`|
| Git分支|`[Branch name or "Unavailable"]`|
| Git Commit |`[Short SHA]`（`[YYYY-MM-DD]`-运行`git log -1 --format="%cs" [SHA]`获取提交日期）|
|型号|`[Model name — ask the system or state the model you are running as]`|
|机器名称|`[hostname]`|
|分析开始|`[UTC timestamp from command]`|
|分析完成|`[UTC timestamp from command]`|
|持续时间|`[Computed difference between started and completed]`|
|输出文件夹|`[folder name]`|
|提示符|`[The user's prompt text that triggered this analysis]`|```

**Gathering rules:**
- START_TIME: Run `Get-Date -Format "yyyy-MM-dd HH:mm:ss" -AsUTC` at workflow Step 1
- END_TIME: Run again before writing 0-assessment.md
- Git fields: `git remote get-url origin`, `git branch --show-current`, `git rev-parse --short HEAD`
- If any command fails → "Unavailable"
- **NEVER estimate timestamps** from folder names
- Model: State the model you are currently running as (e.g., `Claude Opus 4.6`, `GPT-5.3 Codex`, `Gemini 3 Pro`)
- Machine: run `hostname`

### Coverage Counts Consistency

Before writing 0-assessment.md:
- Count elements from `1-threatmodel.md` Element Table
- Count findings from `3-findings.md`
- Count threats from `2-stride-analysis.md` summary table
- Use these exact numbers in Executive Summary and Action Summary

### Formatting Rules

1. `---` horizontal rules between every `##` section
2. Report Metadata values all wrapped in backticks
3. Finding Overrides always uses table format (even when empty)
4. Report Files section always first
5. `0.1-architecture.md` always listed in Report Files table

---

## Common Mistakes Checklist

These are the most observed deviations. Check after writing each file:

1. ❌ Organizing by severity → ✅ Organize by **Exploitability Tier**
2. ❌ Flat STRIDE tables → ✅ Split into Tier 1/2/3 sub-sections per component
3. ❌ Missing `Exploitability Tier` and `Remediation Effort` → ✅ MANDATORY on every finding
4. ❌ STRIDE summary missing T1/T2/T3 columns → ✅ Include T1|T2|T3 columns
5. ❌ Wrapping `.md` in ` ```markdown ` code fences → ✅ Start with `# Heading` on line 1. The `create_file` tool writes raw content — fences become literal text in the file.
6. ❌ Wrapping `.mmd` in ` ```plaintext ` or ` ```mermaid ` → ✅ Start with `%%{init:` on line 1. The `.mmd` file is raw Mermaid source.
7. ❌ Missing Action Summary → ✅ Section MUST be titled exactly `## Action Summary`. MUST include `### Quick Wins` subsection with table of Tier 1 low-effort findings.
8. ❌ Missing threat count context paragraph → ✅ Include `> **Note on threat counts:**` blockquote in Executive Summary
9. ❌ Omitting empty tier sections → ✅ Always include all three tiers per component
10. ❌ Adding separate `### Key Recommendations` or `### Top Recommendations` or `### Priority Remediation Roadmap` → ✅ Action Summary IS the recommendations — no other name.
11. ❌ Drawing sidecars as separate nodes → ✅ See `diagram-conventions.md` Rule 1
12. ❌ Missing CVSS 4.0 vector string → ✅ Every finding MUST have both score AND full vector (e.g., `CVSS:4.0/AV:N/AC:L/...`)
13. ❌ Missing CWE or OWASP on findings → ✅ MANDATORY on every finding
14. ❌ Using OWASP `:2021` suffix → ✅ ALWAYS use `:2025` (e.g., `A01:2025 – Broken Access Control`). The 2025 edition is current.
15. ❌ Missing Threat Coverage Verification table → ✅ Required at end of `3-findings.md`
16. ❌ Architecture component not in STRIDE analysis → ✅ Every component in 0.1-architecture.md must have a STRIDE section
17. ❌ Missing sequence diagrams for top scenarios → ✅ First 3 scenarios in 0.1-architecture.md MUST have Mermaid sequence diagrams
18. ❌ Missing Needs Verification section in 0-assessment.md → ✅ Include under Analysis Context & Assumptions
19. ❌ Missing `## Analysis Context & Assumptions` section entirely → ✅ REQUIRED. Previous iterations skipped this section. Must include Scope, Needs Verification, and Finding Overrides sub-tables.
20. ❌ Missing `### Quick Wins` subsection → ✅ REQUIRED under Action Summary. List Tier 1 low-effort findings; if none, include heading with note.
21. ❌ Skipping `## Report Files`, `## References Consulted`, or `## Report Metadata` → ✅ ALL 7 sections in 0-assessment.md are MANDATORY. Never omit any.
22. ❌ Finding IDs out of order (FIND-06 before FIND-04) → ✅ Finding IDs MUST be sequential top-to-bottom: FIND-01, FIND-02, FIND-03, ... Renumber after sorting.
23. ❌ CWE without hyperlink → ✅ CWE MUST include hyperlink: `[CWE-306](https://cwe.mitre.org/data/definitions/306.html): Missing Authentication for Critical Function`
24. ❌ Time estimates or scheduling in output → ✅ NEVER generate `~1 hour`, `Sprint 1-2`, `Phase 1 — Immediate`, `(hours)`, or any timeline/duration in ANY output file. The report says WHAT to fix, not WHEN.

---

## threat-inventory.json

**Purpose:** Structured JSON inventory of all components, data flows, boundaries, threats, and findings.
This file enables automated comparison between two threat model runs.

**When to generate:** Every run (Step 8b). Generated AFTER all markdown files are written.

**NOT linked in `0-assessment.md`** — this is a machine-readable artifact, not a human-readable report file.

### Schema

```json
｛
“schema_version”:“1.0”,
“提交”:“abc1234”,
“commit_date”:“2025-08-15”,
“分支”:“主要”,
“analysis_timestamp”:“2025 - 08 - 15 - t14:30:00z”
“库”:“https://github.com/org/repo",“report_folder”:“威胁-模型- 20250815 - 143000”,

“组件”:(    {
      "id": "RedisStateStore",
      "display": "Redis State Store",
      "aliases": ["Redis", "StateStoreRedis"],
      "type": "data_store",
      "tmt_type": "SE.DS.TMCore.NoSQL",
      "boundary": "DataLayer",
      "boundary_kind": "ClusterBoundary",
      "source_files": ["helmchart/myapp/templates/redis-statefulset.yaml"],
      "fingerprint": {
        "component_type": "data_store",
        "boundary_kind": "ClusterBoundary",
        "source_files": ["helmchart/myapp/templates/redis-statefulset.yaml"],
        "source_directories": ["helmchart/myapp/templates/"],
        "class_names": [],
        "namespace": "",
        "api_routes": [],
        "config_keys": ["REDIS_HOST", "REDIS_PORT"],
        "dependencies": [],
        "inbound_from": ["InferencingFlow"],
        "outbound_to": [],
        "protocols": ["TCP"]
      },
      "sidecars": []
    }
]，

“边界”:(    {
      "id": "DataLayer",
      "display": "Data Layer",
      "aliases": ["Data Boundary", "Persistence Layer"],
      "kind": "ClusterBoundary",
      "contains": ["RedisStateStore", "VectorDB"],
      "contains_fingerprint": "RedisStateStore|VectorDB"
    }
]，

“流”:[    {
      "id": "DF_InferencingFlow_to_Redis",
      "display": "DF25: InferencingFlow → Redis",
      "from": "InferencingFlow",
      "to": "RedisStateStore",
      "protocol": "TCP",
      "label": "State store operations",
      "bidirectional": true,
      "security": {
        "encryption": "none",
        "authentication": "none"
      }
    }
]，

“威胁”:[    {
      "id": "T05.I",
      "identity_key": {
        "component_id": "RedisStateStore",
        "stride_category": "I",
        "attack_surface": "helmchart/values.yaml:redis.tls.enabled",
        "data_flow_id": "DF_InferencingFlow_to_Redis"
      },
      "title": "Information Disclosure — Redis unencrypted traffic",
      "description": "Redis state store transmits data without TLS...",
      "tier": 1,
      "prerequisites": "None",
      "affected_flow": "DF25",
      "mitigation": "Enable TLS on Redis connections",
      "status": "Open"
    }
]，

“发现”:(    {
      "id": "FIND-01",
      "identity_key": {
        "component_id": "RedisStateStore",
        "vulnerability": "CWE-306",
        "attack_surface": "helmchart/values.yaml:redis.auth"
      },
      "title": "Redis state store has no authentication",
      "severity": "Critical",
      "cvss_score": 9.4,
      "cvss_vector": "CVSS:4.0/AV:N/AC:L/AT:N/PR:N/UI:N/VC:H/VI:H/VA:H/SC:N/SI:N/SA:N",
      "cwe": "CWE-306",
      "owasp": "A07:2025",
      "tier": 1,
      "effort": "Low",
      "related_threats": ["T05.I", "T05.T"],
      "evidence_files": ["helmchart/myapp/values.yaml"],
      "component": "Redis State Store"
    }
]，

“指标”:{    "total_components": 15,
    "total_flows": 30,
    "total_boundaries": 7,
    "total_threats": 97,
    "total_findings": 18,
    "threats_by_tier": { "T1": 12, "T2": 53, "T3": 32 },
    "findings_by_tier": { "T1": 7, "T2": 7, "T3": 4 },
    "findings_by_severity": { "Critical": 4, "Important": 8, "Moderate": 6 },
    "threats_by_stride": { "S": 14, "T": 19, "R": 8, "I": 20, "D": 15, "E": 14, "A": 7 }
  }
}
```

> **⛔ stride_category MUST be a SINGLE LETTER:** `S`, `T`, `R`, `I`, `D`, `E`, or `A`. NEVER use full names like `"Spoofing"` or `"Denial of Service"`. The heatmap computation and comparison matching depend on single-letter codes. If you write `"stride_category": "Denial of Service"` instead of `"stride_category": "D"`, the heatmap will show all zeros for STRIDE columns while tier columns have correct values — this is a critical data integrity bug.

### Incremental Analysis Extensions

When generating `threat-inventory.json` for an **incremental analysis** (see `incremental-orchestrator.md`), add these fields:

**Top-level fields:**
- `"incremental": true` — marks this as an incremental report
- `"baseline_report": "threat-model-20260309-174425"` — path to baseline report folder
- `"baseline_commit": "2dd84ab"` — the commit SHA of the baseline report
- `"target_commit": "abc1234"` — the commit being analyzed
- `"schema_version": "1.1"` — incremental reports use schema version 1.1

**Per-component:** `"change_status"` — one of:
- `"unchanged"` — source files identical or cosmetic-only changes
- `"modified"` — security-relevant source file changes
- `"restructured"` — files moved/renamed, same logical component
- `"removed"` — source files deleted
- `"new"` — component didn't exist at baseline
- `"merged_into:{id}"` — merged into another component
- `"split_into:{id1},{id2}"` — split into multiple components

**Per-threat:** `"change_status"` — one of:
- `"still_present"` — threat exists in current code, same as before
- `"fixed"` — vulnerability was remediated (must cite code change)
- `"mitigated"` — partial remediation applied
- `"modified"` — threat still exists but details changed
- `"new_code"` — threat from a genuinely new component
- `"new_in_modified"` — threat introduced by code changes in existing component
- `"previously_unidentified"` — threat existed in baseline code but wasn't in old report
- `"removed_with_component"` — component was removed

**Per-finding:** `"change_status"` — same values as per-threat, plus:
- `"partially_mitigated"` — code changed partially, vulnerability partially remains

**metrics.status_summary** — counts per `change_status` for components, threats, and findings. See `incremental-orchestrator.md` §4f for the full schema.

### Canonical Naming Rules

**Component IDs** — Derived from actual class/file names, PascalCase:
- `SupportabilityAgent.cs` → `SupportabilityAgent`
- `PowerShellCommandExecutor.cs` → `PowerShellCommandExecutor`
- "Redis State Store" → `RedisStateStore`
- "Ingress-NGINX" → `IngressNginx`

**Flow IDs** — Deterministic from endpoints:
- Format: `DF_{Source}_to_{Target}`
- `DF_Operator_to_TerminalUI`
- `DF_InferencingFlow_to_RedisStateStore`

**Identity Keys** — Each threat and finding gets a canonical identity key:
- Threats: `component_id` + `stride_category` + `attack_surface` + `data_flow_id`
- Findings: `component_id` + `vulnerability` (CWE) + `attack_surface`
- These keys are independent of LLM-generated prose — they anchor to code artifacts

### Deterministic Identity Rules (MANDATORY)

Use these rules so repeated runs on unchanged code produce comparable inventories.

1. **Canonical ID vs display name**
  - `id` is stable identity; `display` is presentation text
  - Never derive identity from prose wording in findings or diagram labels

2. **Alias capture**
  - Every component and boundary must include an `aliases` array
  - Include discovered synonyms from architecture/DFD/STRIDE/findings (deduplicated, sorted)
  - Keep canonical `id` stable even if display wording changes across runs

3. **Boundary kind taxonomy (TMT-aligned)**
  - Use `boundary_kind`/`kind` from this set — describes the NATURE of the trust transition, not what's inside:
    - `MachineBoundary` — between different hosts/VMs (e.g., host ↔ guest, VM1 ↔ VM2)
    - `NetworkBoundary` — between network zones (e.g., corporate LAN ↔ internet, DMZ ↔ internal)
    - `ClusterBoundary` — between K8s/container cluster and outside (e.g., cluster ↔ external services)
    - `ProcessBoundary` — between OS processes or containers on same host (e.g., sidecar ↔ main container)
    - `PrivilegeBoundary` — between different privilege levels (e.g., user mode ↔ kernel, unprivileged ↔ admin)
    - `SandboxBoundary` — between sandboxed and unsandboxed execution (e.g., browser sandbox, WASM)
  - Each value answers: "what changes when you cross this line?" (different machine, network, cluster, process, privilege, sandbox)
  - Do NOT use component-grouping labels (DataStorage, ApplicationCore, AgentExecution) as boundary kinds — those describe WHAT's inside, not the nature of the trust transition

3b. **Boundary ID derivation** (MANDATORY — apply the same deterministic naming as components)
  - Derive boundary IDs from deployment/infrastructure names, NOT abstract concepts:
    - Docker host → `Docker` (never `DockerEnvironment` or `ContainerRuntime`)
    - Kubernetes cluster → `K8sCluster` (never `KubernetesEnvironment`)
    - Operator's machine → `OperatorWorkstation` (never `HostOS` or `LocalMachine`)
    - External cloud services → `ExternalServices` (never `CloudBoundary`)
    - Data storage grouped → `DataStorage` (never `DataLayer` or `PersistenceLayer`)
    - Backend application services → `BackendServices` (never `AppBoundary` or `ApplicationCore`)
    - ML/AI inference models → `MLModels` (never `InferenceModels` or `ModelBoundary`)
    - DMZ/public zone → `PublicZone` (never `DMZBoundary` or `IngressZone`)
    - Agent execution → `AgentExecution` (keep this exact ID)
    - Tool execution → `ToolExecution` (keep this exact ID)
  - Once a boundary ID is chosen in Step 1, use it EVERYWHERE (DFD, tables, JSON)
  - Never restructure containment between runs on the same code (same component → same boundary)

4. **Component fingerprint**
  - `fingerprint` must be built from stable evidence:
    - sorted `source_files` — full file paths to primary source files
    - sorted `source_directories` — parent directory paths of source files (more stable than filenames across refactors)
    - sorted `class_names` — primary class, struct, or interface names defined in the component's source files (e.g., `["HealthServer", "IHealthService"]`). For non-code components (datastores, external services), leave empty.
    - `namespace` — the primary namespace/package (e.g., `"MCP.Core.Servers.Health"` for C#, `"ragapp.src.ingestflow"` for Python). Empty for non-code components.
    - sorted `api_routes` — HTTP API endpoint patterns exposed by this component (e.g., `["/api/health", "/api/v1/chat"]`). Empty if not an HTTP service.
    - sorted `config_keys` — environment variables and configuration keys consumed by this component (e.g., `["AZURE_OPENAI_ENDPOINT", "REDIS_HOST"]`). Extract from appsettings.json, .env files, Helm values, or code that reads env vars.
    - sorted `dependencies` — external package/library dependencies specific to this component (e.g., `["Microsoft.SemanticKernel", "Azure.AI.OpenAI"]` for NuGet, `["pymilvus", "fastapi"]` for pip). Only include packages that are characteristic of this component, not framework-wide dependencies.
    - sorted `inbound_from` and `outbound_to` component IDs
    - sorted `protocols`
    - `component_type` and `boundary_kind`
  - Do not include mutable prose in the fingerprint
  - **Deterministic matching priority:** `source_directories` > `class_names` > `namespace` > `api_routes` > `config_keys` are all highly stable signals that survive component renames. Two components sharing any of these are almost certainly the same real component.

  **Fingerprint Field → Comparison Matching Signal Map:**
  | Fingerprint Field | Comparison Signal | Max Points | Stability |
  |---|---|---|---|
  | `source_files` | Signal 2 — Source file/directory overlap | +30 | High (files rarely move) |
  | `source_directories` | Signal 2 — Source file/directory overlap | +25 | Very High (directories almost never change) |
  | `class_names` | Signal 3 — Class/Namespace match | +25 | Very High (classes rarely rename) |
  | `namespace` | Signal 3 — Class/Namespace match | +20 | Very High (namespaces are structural) |
  | `api_routes` | Signal 4 — API route / Config key overlap | +15 | High (API contracts are versioned) |
  | `config_keys` | Signal 4 — API route / Config key overlap | +10 | High (config keys are stable) |
  | `dependencies` | Signal 4 — API route / Config key overlap | +5 | Medium (packages change with upgrades) |
  | `inbound_from` / `outbound_to` | Signal 5 — Topology overlap | +15 | Low (uses component IDs which may drift) |
  | `component_type` + `boundary_kind` | Signal 6 — Type + boundary kind | +10 | Medium (boundary naming may vary) |
  | `protocols` | (Not directly scored — used as tiebreaker) | — | Medium |

  **Every field in this table MUST be populated during analysis (Step 8b).** Empty arrays `[]` are acceptable when the field genuinely doesn't apply (e.g., `api_routes` for a datastore). But `source_directories` and `class_names` must NEVER be empty for process-type components — these are the primary matching anchors.

5. **Boundary containment fingerprint**
  - `contains_fingerprint` = sorted `contains` joined with `|`
  - Use this for boundary rename detection during comparison

6. **Deterministic ordering**
  - Sort all arrays and nested list fields before writing JSON
  - This makes diffs stable and prevents accidental churn

### Processing Rules

1. Generate AFTER all markdown files are written (Step 8b)
2. Populate from the same analysis data used to write the markdown files
3. Ensure component IDs use PascalCase derived from actual class/file names
4. Ensure flow IDs use the canonical `DF_{Source}_to_{Target}` format
5. All threat and finding identity keys must reference actual code artifacts (file paths, config keys)
6. Include git metadata from Step 1 (commit, branch, date)
7. The `metrics` object must match the counts in the markdown reports
8. This file is NOT listed in the Report Files table of `0-assessment.md`
9. Populate `aliases`, `boundary_kind`/`kind`, `fingerprint`, and `contains_fingerprint` for deterministic matching
10. If a component has multiple observed names in the same run, keep one canonical `id` and store all alternates in `aliases`

> **⚠️ CRITICAL — Array completeness:**
> The `threats` array MUST contain one entry for every threat listed in `2-stride-analysis.md`.
> The `findings` array MUST contain one entry for every finding in `3-findings.md`.
> The `components` array MUST contain one entry for every component in the Element Table.
> **Verify:** `threats.length == metrics.total_threats`, `findings.length == metrics.total_findings`,
> `components.length == metrics.total_components`. If mismatched, the JSON is incomplete — go back
> and add the missing entries. Do NOT truncate arrays to save space.

---

## Self-Check — Run After Writing Each File

⛔ **MANDATORY:** After writing each file, verify these checks and report results. Fix any ❌ before proceeding.

### After `2-stride-analysis.md`:
- [ ] Summary table appears BEFORE individual component sections
- [ ] 3 tier sub-sections per component (Tier 1, Tier 2, Tier 3)
- [ ] Status column uses only: `Open`, `Mitigated`, `Platform` (no `Accepted Risk`, no `Needs Review`)
- [ ] Platform ratio within limit (≤20% standalone, ≤35% K8s operator)
- [ ] Every threat has single-letter STRIDE category (S/T/R/I/D/E/A)

### After `3-findings.md`:
- [ ] 3 tier headings: `## Tier 1`, `## Tier 2`, `## Tier 3` (all present)
- [ ] Zero occurrences of "Accepted Risk" anywhere in the file
- [ ] Every finding has CVSS 4.0 vector string
- [ ] Action Summary: T1=Critical, T2=Elevated, T3=Moderate priorities
- [ ] 4th column header is "Assignment Rule" (not "Example")

### After `threat-inventory.json`:
- [ ] `threats.length == metrics.total_threats` (zero tolerance)
- [ ] `findings.length == metrics.total_findings` (zero tolerance)
- [ ] If threats > 50, used sub-agent/Python/chunked — NOT single `create_file`
- [ ] Every component has non-empty `fingerprint.source_directories`
- [ ] Arrays sorted by canonical key
- [ ] **Field names match schema exactly:** components use `display` (NOT `display_name`), threats use `stride_category` (NOT `category`), threat→component link is inside `identity_key.component_id` (NOT top-level `component_id`), threats have BOTH `title` (short name) AND `description` (longer prose) — NOT just `description` alone

### After `0-assessment.md`:
- [ ] Exactly 7 sections: Report Files, Executive Summary, Action Summary, Analysis Context & Assumptions, References Consulted, Report Metadata, Classification Reference
- [ ] `---` horizontal rule between every pair of `##` sections

---

## Enumeration Reference

All reports MUST use these exact values. Do NOT abbreviate, substitute, or invent alternatives.

**Component Types:** `process` | `data_store` | `external_service` | `external_interactor`

**Boundary Kinds (TMT-aligned):** `MachineBoundary` | `NetworkBoundary` | `ClusterBoundary` | `ProcessBoundary` | `PrivilegeBoundary` | `SandboxBoundary`

**Exploitability Tiers:** `Tier 1` (Direct Exposure — no prerequisites) | `Tier 2` (Conditional Risk — single prerequisite) | `Tier 3` (Defense-in-Depth — multiple prerequisites)

**STRIDE + Abuse Categories:** `S` Spoofing | `T` Tampering | `R` Repudiation | `I` Information Disclosure | `D` Denial of Service | `E` Elevation of Privilege | `A` Abuse

**SDL Bugbar Severity:** `Critical` | `Important` | `Moderate` | `Low`

**Remediation Effort:** `Low` | `Medium` | `High`

**Mitigation Type (OWASP-aligned):** `Redesign` | `Standard Mitigation` | `Custom Mitigation` | `Existing Control` | `Accept Risk` | `Transfer Risk`

**Threat Status:** `Open` | `Mitigated` | `Platform`

**Finding Change Status (incremental):** `Still Present` | `Fixed` | `New` | `New (Code)` | `New (Previously Unidentified)` | `Removed`

**OWASP Top 10:2025 suffix:** Always `:2025` (e.g., `A01:2025 – Broken Access Control`)
- [ ] Quick Wins, Needs Verification, Finding Overrides subsections present
- [ ] Deployment pattern documented (K8s operator vs standalone)
- [ ] All metadata values in backticks

**Also verify (applies to ALL files):** No leaked directives (⛔, RIGID, NON-NEGOTIABLE in output), no time estimates, no nested output folders. See `verification-checklist.md` Phase 0 for the full common deviation list.
