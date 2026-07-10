增量编排器-威胁模型更新工作流

此文件包含用于执行增量威胁模型分析的完整编排逻辑——生成基于现有基线报告的新威胁模型报告。当用户请求更新的分析并且存在先前的`threat-model-*`文件夹时调用它。

**与单一分析（`orchestrator.md`）的主要区别：**该工作流继承了旧报告的组件清单、id和约定，而不是从头开始发现组件。然后，它根据当前代码验证每个项并发现新项。

##⚡背景预算-选择性读取文件**阶段1（设置+变更检测）：**仅读取该文件（`incremental-orchestrator.md`）。旧的`threat-inventory.json`提供了结构骨架-还不需要阅读其他技能文件。
**阶段2（生成报告）：**在写入每个文件之前，读取`orchestrator.md`（对于强制性规则1-34），`output-formats.md`，`diagram-conventions.md`-加上`skeletons/`中的相关骨架。请参阅下面特定于增量的规则。
**阶段3（验证）：**用`verification-checklist.md`委托给子代理（所有9个阶段，包括用于比较HTML的阶段8）。

---

##何时使用此工作流

当满足所有这些条件时，使用增量分析：
1. 用户的请求涉及更新、重新运行或刷新威胁模型
2. 存储库中存在一个具有有效`threat-inventory.json`的先前`threat-model-*`文件夹
3. 用户提供或暗示两者：基线报告文件夹和目标提交（默认为HEAD）* *触发例子:* *
-“以threat-model-20260309-174425为基线更新威胁模型”
-“针对之前的报告运行增量威胁模型分析”
-“自上一个威胁模型以来，安全方面有什么变化？”
-“刷新最新提交的威胁模型”

**不是这个工作流：**
-首次分析（无基线）→使用`orchestrator.md`-“分析此回购的安全性”，不提及先前的报告→使用`orchestrator.md`---

# #输入

|输入|源|必选？||-------|--------|-----------|
|基线报表文件夹|`threat-model-*`目录路径|是|
|基线`threat-inventory.json`|`{baseline_folder}/threat-inventory.json`|是|
| Baseline commit SHA | From`{baseline_folder}/0-assessment.md`Report Metadata |是|
|目标提交|用户提供的SHA或默认为HEAD |是（默认：HEAD） |

---

**⛔子代理治理适用于所有阶段。**参见`orchestrator.md`子代理治理一节。子代理是只读的助手——它们从不调用`create_file`来获取报告文件。

阶段0：设置和验证

1. **记录开始时间：**   ```
   Get-Date -Format "yyyy-MM-dd HH:mm:ss" -AsUTC
   ```
存储为`START_TIME`。

2. **收集git信息   ```
   git remote get-url origin
   git branch --show-current
   git rev-parse --short HEAD
   hostname
   ```
3. * *验证输入:* *
—确认基线文件夹存在：`Test-Path {baseline_folder}/threat-inventory.json`—从`0-assessment.md`读取基线提交SHA：查找`| Git Commit |`行
—确认目标提交是可解析的：`git rev-parse {target_sha}`**获取提交日期：**`git log -1 --format="%ai" {baseline_sha}`和`git log -1 --format="%ai" {target_sha}`-不是今天的日期
- **获取代码更改计数**（用于HTML指标栏）：     ```
     git rev-list --count {baseline_sha}..{target_sha}
     git log --oneline --merges --grep="Merged PR" {baseline_sha}..{target_sha} | wc -l
     ```
     Store as `COMMIT_COUNT` and `PR_COUNT`.
4. 基线代码访问-重用或创建工作树：**   ```
   # Check for existing worktree
   git worktree list
   
   # If a worktree for baseline_sha exists → reuse it
   # Verify: git -C {worktree_path} rev-parse HEAD
   
   # If not → create one:
   git worktree add ../baseline-{baseline_sha_short} {baseline_sha}
   ```
将工作树路径存储为`BASELINE_WORKTREE`，以便在后面的阶段进行旧代码验证。

5. **创建输出文件夹：**   ```
   threat-model-{YYYYMMDD-HHmmss}/
   ```
---

阶段1：加载旧的报告骨架

读取基线`threat-inventory.json`并提取结构骨架：```
From threat-inventory.json, load:
  - components[]  → all component IDs, types, boundaries, source_files, fingerprints
  - flows[]       → all flow IDs, from/to, protocols
  - boundaries[]  → all boundary IDs, contains lists
  - threats[]     → all threat IDs, component mappings, stride categories, tiers
  - findings[]    → all finding IDs, titles, severities, CWEs, component mappings
  - metrics       → totals for validation

Store as the "inherited inventory" — the structural foundation.
```
**不要从旧报告的降价文件中阅读全文**。只加载结构化数据。按需阅读旧报告散文时：
-验证之前是否分析过特定的代码模式
-解决组件角色或分类的歧义
-发现状态决定所需的历史背景

---

阶段2：每个组件的变更检测

对于继承清单中的每个组件，确定其更改状态：```
For EACH component in inherited inventory:

  1. Check source_files existence at target commit:
     git ls-tree {target_sha} -- {each source_file}
  
  2. If ALL source files missing:
     → change_status = "removed"
     → Mark all linked threats as "removed_with_component"
     → Mark all linked findings as "removed_with_component"
  
  3. If source files exist, check for changes:
     git diff --stat {baseline_sha} {target_sha} -- {source_files}
     
     If NO changes → change_status = "unchanged"
     
     If changes exist, check if security-relevant:
       Read the diff: git diff {baseline_sha} {target_sha} -- {source_files}
       Look for changes in:
       - Auth/credential patterns (tokens, passwords, certificates)
       - Network/API surface (new endpoints, changed listeners, port bindings)
       - Input validation (sanitization, parsing, deserialization)
       - Command execution patterns (shell exec, process spawn)
       - Config values (TLS settings, CORS, security headers)
       - Dependencies (new packages, version changes)
       
       If security-relevant → change_status = "modified"
       If cosmetic only (whitespace, comments, logging, docs) → change_status = "unchanged"
  
  4. If files moved or renamed:
     git log --follow --diff-filter=R {baseline_sha}..{target_sha} -- {source_files}
     → change_status = "restructured"
     → Update source_file references to new paths
```
**记录每个组件的分类** -这驱动所有下游决策。

---

阶段3：扫描新组件```
1. Enumerate source directories/files at {target_sha} that are NOT referenced
   by any existing component's source_files or source_directories.
   Focus on: new top-level directories, new *Service.cs/*Agent.cs/*Server.cs classes,
   new Helm deployments, new API controllers.

2. Apply the same component discovery rules from orchestrator.md:
   - Class-anchored naming (PascalCase from actual class names)
   - Component eligibility criteria (crosses trust boundary or handles security data)
   - Same naming procedure (primary class → script → config → directory → technology)

3. For each candidate new component:
   - Verify it didn't exist at baseline: git ls-tree {baseline_sha} -- {path}
   - If it existed at baseline → this is a "missed component" from the old analysis
     → Add to Needs Verification section with note: "Component existed at baseline
       but was not in the previous analysis. May indicate an analysis gap."
   - If genuinely new (files didn't exist at baseline):
     → change_status = "new"
     → Assign a new component ID following the same PascalCase naming rules
     → Full STRIDE analysis will be performed in Phase 4
```
---

阶段4：生成报告文件

现在生成所有报告文件。**在开始之前阅读相关技能文件：**
—`orchestrator.md`—强制规则1-34适用于所有报表文件
-`output-formats.md`-模板和格式规则
-`diagram-conventions.md`-图的颜色和样式
- **在写入每个文件之前，从`skeletons/skeleton-*.md`中读取相应的骨架** -逐字复制并填充`[FILL]`占位符

**⛔子代理治理（强制-防止双文件夹错误）：**父代理拥有所有文件创建。子代理是只读的助手，用于搜索代码、收集上下文和运行验证——它们永远不会为报告文件调用`create_file`。请参阅`orchestrator.md`中的完整子代理治理规则。唯一的例外是用于大型仓库的`threat-inventory.json`委托—即使这样，子代理提示符也必须包含精确的输出文件路径和只写该文件的显式指令。**⛔CRITICAL：增量报告是一个STANDALONE报告。**不看旧报告的人必须了解完整的安全态势。状态注释（[STILL PRESENT]、[FIXED]、[NEW CODE]等）是对完整内容的补充，而不是替代。

# # # 4。0.1-architecture.md- **先读取`skeletons/skeleton-architecture.md`** -用作结构模板
-复制旧报告的组件结构作为您的起始模板
- **未更改的组件：**使用当前代码重新生成描述（不是从旧报告复制粘贴）。相同的ID，相同的约定。
- **修改组件：**更新描述以反映代码更改。添加注释：`[MODIFIED — security-relevant changes detected]`- **新组件：**添加注释：`[NEW]`- **删除组件：**添加注释：`[REMOVED]`和简要说明
-技术栈，部署模型：如果改变，更新，否则继续⛔**部署分类是必选（即使是增量模式）：**`0.1-architecture.md`必须包含：
1.`**Deployment Classification:** \`(价值)\“` line (e.g., `K8S_SERVICE`, `LOCALHOST_DESKTOP”)
2.`### Component Exposure Table`，列：组件，监听，授权，可达性，最小先决条件，派生层
如果基线有这些，那么将它们向前推进并更新new/modified组件。
如果基线没有这些，**现在从代码中派生它们** -所有后续步骤都需要它们。
**如果没有这两个元素，不要继续步骤4b

-场景：保留旧场景，为新功能添加新场景
-适用`output-formats.md`的所有标准`0.1-architecture.md`规则

# # # 4 b。1.1 -threatmodel.mmd(过程)- **先读取`skeletons/skeleton-dfd.md`和`skeletons/skeleton-summary-dfd.md`—从旧DFD的逻辑布局开始
- **携带组件相同的节点ID **（对ID稳定性至关重要）
- **新组件：**添加独特的样式-使用`classDef newComponent fill:#d4edda,stroke:#28a745,stroke-width:3px`- **删除组件：**显示为虚线与灰色填充-使用`classDef removedComponent fill:#e9ecef,stroke:#6c757d,stroke-width:1px,stroke-dasharray:5`- **相同的流id **不变的流量
—**新流：**新id延续顺序
-适用`diagram-conventions.md`的所有标准DFD规则（流程图LR，调色板等）

⛔**POST-DFD GATE:**创建`1.1-threatmodel.mmd`后，计数元素和边界。如果元素> 15或边界> 4→创建`1.2-threatmodel-summary.mmd`使用`skeleton-summary-dfd.md`NOW。在做出决定之前，不要进行步骤4c。

# # # 4 c。1-threatmodel.md**先读取`skeletons/skeleton-threatmodel.md`** -使用表结构
-元素表：所有旧元素+新元素，增加了`Status`列
—取值：`Unchanged`、`Modified`、`New`、`Removed`、`Restructured`-流表：所有旧流+新流，`Status`列
-边界表：继承的边界+任何新的边界
—如果生成了`1.2-threatmodel-summary.mmd`，则需要在`## Summary View`部分中包含汇总图和映射表
-适用`output-formats.md`中的所有标准表规则

# # # 4 d。2-stride-analysis.md- **先读取`skeletons/skeleton-stride-analysis.md`** -使用汇总表和每组件结构**⛔对于渐进式跨步的关键提醒（这些规则来自`orchestrator.md`在这里同样适用）：**
1. ** STRIDE-A中的“A”总是“滥用”**（业务逻辑滥用、工作流操纵、功能滥用）。永远不要使用“授权”作为STRIDE-A类别名称。这适用于威胁ID后缀(T01。A)，N/A对齐标签，以及所有散文。授权问题属于特权提升(E)类别，而不是A类别。
2. **`## Summary`表必须出现在文件的顶部，紧接在`## Exploitability Tiers`之后，在任何单独的组件段之前。在顶部使用这个EXACT结构：```markdown
# STRIDE-A Threat Analysis

## Exploitability Tiers
| Tier | Label | Prerequisites | Assignment Rule |
|------|-------|---------------|----------------|
| **Tier 1** | Direct Exposure | `None` | Exploitable by unauthenticated external attacker with NO prior access. |
| **Tier 2** | Conditional Risk | Single prerequisite | Requires exactly ONE form of access. |
| **Tier 3** | Defense-in-Depth | Multiple prerequisites or infrastructure access | Requires significant prior breach or multiple combined prerequisites. |

## Summary
| Component | Link | S | T | R | I | D | E | A | Total | T1 | T2 | T3 | Risk |
|-----------|------|---|---|---|---|---|---|---|-------|----|----|----|------|
<!-- one row per component with numeric counts, then Totals row -->

---
## [First Component Name]
```
3. **跨步类别可能产生0、1、2、3+威胁**每个组件。每个类别不要限制在一个威胁。具有丰富安全表面的组件通常每个相关类别应该有2-4个威胁。如果Summary表中的每个STRIDE单元格都是0或1，则说明分析过于肤浅——请返回并识别其他威胁向量。汇总表列反映了实际的威胁计数。
4. **⛔PREREQUISITE FLOOR CHECK（每个威胁）：**在为任何威胁分配先决条件之前，请在组件暴露表（`0.1-architecture.md`）中查找组件的`Min Prerequisite`和`Derived Tier`。威胁的先决条件必须≥组件的地板。威胁的层级必须≥组件的派生层级。使用规范先决条件→来自`analysis-principles.md`的层映射。先决条件必须只使用规范值：`None`，`Authenticated User`,`Privileged User`,`Internal Network`,`Local Process Access`,`Host/OS Access`,`Admin Credentials`,`Physical Access`,`{Component} Compromise`。⛔`Application Access`和xqz14禁止使用xqz。**⛔标题锚规则（适用于所有输出文件）：**所有`##`和`###`标题在每个输出文件必须是纯文本-没有状态标签（`[Existing]`,`[Fixed]`,`[Partial]`,`[New]`,`[Removed]`，或任何旧风格的标签）在标题文本。标签破坏了锚链接并污染了目录。将状态注释放在section/finding正文的第一行：
-✅`## KmsPluginProvider`，第一行`> **[New]** Component added in this release.`-✅`### FIND-01: Missing Auth Check`第一行`> **[Existing]**`-❌`## KmsPluginProvider [New]`（打破`#kmspluginprovider`锚）
-❌`### FIND-01: Missing Auth Check [Existing]`（污染标题）

适用于：`0.1-architecture.md`、`2-stride-analysis.md`、`3-findings.md`、`1-threatmodel.md`。

对于每个组件，STRIDE分析方法取决于其变化状态：

|组件状态| STRIDE方法||-----------------|-----------------|
| **不变** |保留旧报告中所有带有`[STILL PRESENT]`注释的威胁条目。针对当前代码重新验证每个威胁的缓解状态。|
重新分析具有diff访问权限的组件。对于每个旧威胁：确定`still_present`，`fixed`，`mitigated`或`modified`。从代码更改中发现新的威胁→分类为`new_in_modified`。|
| **新** |完全新鲜的STRIDE-A分析（与单分析模式相同）。所有被归类为`new_code`的威胁。|
| **删除** |节头注释：“组件删除-所有威胁解决与`removed_with_component`状态。”|

**威胁ID连续性：**
-旧威胁保留其原始id（例如T01）。年代,T02。T)
-新威胁继续从旧报告的最高威胁数的顺序
-永远不要重新分配或重用旧威胁ID**N/A类别（来自PRD§3.7）：**
-每个组件都得到所有7个STRIDE-A类别的地址
—不适用类别：`N/A — {1-sentence justification}`-N/A条目不计入威胁总数

** STRIDE表中的状态注释格式：**
在每个威胁表行中添加一个`Change`列，如下所示：
-`Existing`-当前代码中存在威胁，与之前一样（包括细节改动较小的威胁）`Fixed`漏洞已修复（引用具体的代码更改）
-`New`-来自新组件、代码更改或先前未识别的威胁
-`Removed`-组件被移除<!-- SIMPLIFIED DISPLAY TAGS: Only 5 tags for display in markdown body text.
[Existing] = still_present，修改，缓解（威胁仍然存在）
[修复]=修复（完全修复）
[Partial] = partially_mitigated（代码改变了，但漏洞仍然存在）
[New] = new_code, new_in_modified, previly_identified（本报告新增）
[已删除]= removed_with_component（组件已删除）
JSON change_status保留了编程使用的详细值。-->

⛔POST-STEP CHECK：写完所有威胁的Change列后，验证：
1. 每个威胁行只有一个：现有，固定，新，删除
2. 没有旧式标签：仍然存在，新的（代码），新的（修改），以前未识别
3. 固定的威胁引用了特定的代码更改

# # # 4 e。3-findings.md⛔**在写任何发现之前-现在重新阅读`skeletons/skeleton-findings.md`该框架为每个查找块定义了EXACT结构，包括`#### Evidence`部分中的强制性`**Prerequisite basis:**`行。每一个发现-无论是[现有的]，[新的]，[固定的]，或[部分的]-必须遵循这个骨架结构。⛔**部署上下文门（失败关闭）-适用于所有发现（新的和后续的）：**
阅读`0.1-architecture.md`部署分类和组件公开表。
如果分类为`LOCALHOST_DESKTOP`或`LOCALHOST_SERVICE`：
-零发现可能有`Exploitation Prerequisites`=`None`→修复到`Local Process Access`或`Host/OS Access``## Tier 1`→降级到T2/T3可能没有发现
-零CVSS矢量可能使用`AV:N`，除非组件有`Reachability = External`对于所有分类：
-每个发现的先决条件必须≥暴露表中其成分的`Min Prerequisite`-每个发现的层必须≥其组件的`Derived Tier`- **每个发现的`#### Evidence`部分必须以`**Prerequisite basis:**`行开始**引用确定先决条件的特定code/config（例如，“ClusterIP服务，没有入口-内部仅根据暴露表”）。这也适用于[现有的]发现——从当前代码重新派生。
-先决条件必须只使用规范值es。⛔`Application Access`、`Host Access`禁止使用。对于每个旧的发现，对照当前代码进行验证：

|情况|变化状态|动作||-----------|---------------|--------|
|代码不变，漏洞完整|`still_present`|体|第一行`> **[Existing]**`结转
|`fixed`|标记为`> **[Fixed]**`，引用具体代码更改|
|代码部分更改|`partially_mitigated`|标记`> **[Partial]**`，解释更改的内容和保留的|
|组件完全删除|`removed_with_component`|标记为`> **[Removed]**`|

有关新发现：

|状态|变化状态|标签||-----------|---------------|-------|
|新增组件，新增漏洞|`new_code`|`> **[New]**`|
|现有组件，漏洞引入代码更改|`new_in_modified`|`> **[New]**`-引用具体更改|
|现有组件，漏洞存在于旧代码中，但遗漏了|`previously_unidentified`|`> **[New]**`-根据基线工作树|进行验证<!-- ⛔ POST-STEP CHECK: After writing all finding annotations:
1. 每一个发现体都以以下其中一个开头：[现有]，[固定]，[部分]，[新]，[删除]
2. 标签在正文中以blockquote （b> **[Tag]**）的形式出现，而不是在###标题中
3. 没有旧式标签：[仍然存在]，[新代码]，[新修改]，[先前未识别]，[部分缓解]，[与组件一起删除]
4. JSON change_status使用详细值（still_present， new_code等）进行编程比较——>**寻找ID连续性：**
-旧的发现保留原来的id （FIND-01到FIND-N）
-新的发现继续这个顺序：FIND-N+1， FIND-N+2，…
-没有空白，没有重复
-固定的发现保留但注释-它们不会从报告中删除
- **文件顺序**：调查结果按层（1→2→3）排序，然后按严重程度（危急→重要→中等→低），然后由CVSS下降-与独立分析相同。因为保留了旧的ID，所以ID号在文档中可能不是按数字升序排列的。这在增量模式下是可以接受的——交叉报告跟踪的ID稳定性优先于顺序排序。`### FIND-XX:`标题将以tier/severity顺序出现，而不是ID顺序。**先前未确定的验证程序：**
1. 确定调查结果的组成部分和证据文件
2. 在基线提交时读取相同的文件：`cat {BASELINE_WORKTREE}/{file_path}`3. 如果旧代码中存在漏洞模式→`previously_unidentified`4. 如果旧代码中不存在漏洞模式→`new_in_modified`# # # 4 f。threat-inventory.json- **先读取`skeletons/skeleton-inventory.md`** -使用精确的字段名和模式结构

与单个分析相同的模式，具有额外的字段：```json
{
  "schema_version": "1.1",
  "incremental": true,
  "baseline_report": "threat-model-20260309-174425",
  "baseline_commit": "2dd84ab",
  "target_commit": "abc1234",
  
  "components": [
    {
      "id": "McpHost",
      "change_status": "unchanged",
      ...existing fields...
    }
  ],
  
  "threats": [
    {
      "id": "T01.S",
      "change_status": "still_present",
      ...existing fields...
    }
  ],
  
  "findings": [
    {
      "id": "FIND-01",
      "change_status": "still_present",
      ...existing fields...
    }
  ],
  
  "metrics": {
    ...existing fields...,
    "status_summary": {
      "components": {
        "unchanged": 15,
        "modified": 2,
        "new": 1,
        "removed": 1,
        "restructured": 0
      },
      "threats": {
        "still_present": 80,
        "fixed": 5,
        "mitigated": 3,
        "new_code": 10,
        "new_in_modified": 4,
        "previously_unidentified": 2,
        "removed_with_component": 8
      },
      "findings": {
        "still_present": 12,
        "fixed": 2,
        "partially_mitigated": 1,
        "new_code": 3,
        "new_in_modified": 2,
        "previously_unidentified": 1,
        "removed_with_component": 1
      }
    }
  }
}
```
0-assessment.md- **先读取`skeletons/skeleton-assessment.md`** -使用section顺序和表结构

标准评估部分（全部7项强制性）加上增量特定部分：

**标准切片（与单一分析相同）：**
1. 报告文件
2. 执行摘要（附`> **Note on threat counts:**`blockquote）
3. 动作总结（带`### Quick Wins`）
4. 分析背景和假设（使用`### Needs Verification`和`### Finding Overrides`）
5. 参考咨询
6. 报告元数据
7. 分类参考（从骨架复制的静态表）

**额外的增量部分（插入在行动总结和分析上下文之间）：**```markdown
## Change Summary

### Component Changes
| Status | Count | Components |
|--------|-------|------------|
| Unchanged | X | ComponentA, ComponentB, ... |
| Modified | Y | ComponentC, ... |
| New | Z | ComponentD, ... |
| Removed | W | ComponentE, ... |

### Threat Status
| Status | Count |
|--------|-------|
| Still Present | X |
| Fixed | Y |
| New (Code) | Z |
| New (Modified) | M |
| Previously Unidentified | W |
| Removed with Component | V |

### Finding Status
| Status | Count |
|--------|-------|
| Still Present | X |
| Fixed | Y |
| Partially Mitigated | P |
| New (Code) | Z |
| New (Modified) | M |
| Previously Unidentified | W |
| Removed with Component | V |

### Risk Direction
[Improving / Worsening / Stable] — [1-2 sentence justification based on status distribution]

---

## Previously Unidentified Issues

These vulnerabilities were present in the baseline code at commit `{baseline_sha}` but were not identified in the prior analysis:

| Finding | Title | Component | Evidence |
|---------|-------|-----------|----------|
| FIND-XX | [title] | [component] | Baseline code at `{file}:{line}` |
```
**报告元数据添加：**```markdown
| Baseline Report | `{baseline_folder}` |
| Baseline Commit | `{baseline_sha}` (`{baseline_commit_date}` — run `git log -1 --format="%cs" {baseline_sha}`) |
| Target Commit | `{target_sha}` (`{target_commit_date}` — run `git log -1 --format="%cs" {target_sha}`) |
| Baseline Worktree | `{worktree_path}` |
| Analysis Mode | `Incremental` |
```
# # # 4 h。incremental-comparison.html- **先读取`skeletons/skeleton-incremental-html.md`** -使用8段结构和CSS变量

生成一个可视化比较的自包含HTML文件。所有数据都来自已经在`threat-inventory.json`中计算过的`change_status`字段。

* *结构:* *```html
<!-- Section 1: Header + Comparison Cards -->
<div class="header">
  <div class="report-badge">INCREMENTAL THREAT MODEL COMPARISON</div>
  <h1>{{repo_name}}</h1>
</div>
<div class="comparison-cards">
  <div class="compare-card baseline">
    <div class="card-label">BASELINE</div>
    <div class="card-hash">{{baseline_sha}}</div>
    <div class="card-date">{{baseline_commit_date from git log}}</div>
    <div class="risk-badge">{{old_risk_rating}}</div>
  </div>
  <div class="compare-arrow">→</div>
  <div class="compare-card target">
    <div class="card-label">TARGET</div>
    <div class="card-hash">{{target_sha}}</div>
    <div class="card-date">{{target_commit_date from git log}}</div>
    <div class="risk-badge">{{new_risk_rating}}</div>
  </div>
  <div class="compare-card trend">
    <div class="card-label">TREND</div>
    <div class="trend-direction">{{Improving|Worsening|Stable}}</div>
    <div class="trend-duration">{{N months}}</div>
  </div>
</div>

<!-- Section 2: Metrics Bar (5 boxes — NO Time Between, use Code Changes) -->
<div class="metrics-bar">
  Components: {{old_count}} → {{new_count}} (±N)
  Trust Boundaries: {{old_boundaries}} → {{new_boundaries}} (±N)
  Threats: {{old_count}} → {{new_count}} (±N)  
  Findings: {{old_count}} → {{new_count}} (±N)
  Code Changes: {{COMMIT_COUNT}} commits, {{PR_COUNT}} PRs
</div>

<!-- Section 3: Status Summary Cards (colored cards — primary visualization) -->
<div class="status-cards">
  <!-- Green card: Fixed (count + list of fixed items) -->
  <!-- Red card: New (code + modified) (count + list of new items) -->
  <!-- Amber card: Previously Unidentified (count + list) -->
  <!-- Gray card: Still Present (count) -->
</div>

<!-- Section 4: Component Status Grid -->
<table class="component-grid">
  <!-- Row per component: ID | Type | Status (color-coded) | Source Files -->
</table>

<!-- Section 5: Threat/Finding Status Breakdown -->
<div class="status-breakdown">
  <!-- Grouped by status: Fixed items, New items, etc. -->
  <!-- Each item: ID | Title | Component | Status -->
</div>

<!-- Section 6: STRIDE Heatmap with Deltas -->
<!-- ⛔ MANDATORY: Heatmap MUST have 13 columns including T1/T2/T3 after a divider -->
<table class="stride-heatmap">
  <thead>
    <tr>
      <th>Component</th>
      <th>S</th><th>T</th><th>R</th><th>I</th><th>D</th><th>E</th><th>A</th>
      <th>Total</th>
      <th class="divider"></th>
      <th>T1</th><th>T2</th><th>T3</th>
    </tr>
  </thead>
  <tbody>
    <!-- Row per component. Each STRIDE cell: value (▲+N or ▼-N delta from baseline) -->
    <!-- The divider column is a thin visual separator between STRIDE totals and tier breakdown -->
  </tbody>
</table>

<!-- Section 7: Needs Verification -->
<div class="needs-verification">
  <!-- Items where analysis disagrees with old report -->
</div>

<!-- Section 8: Footer -->
<div class="footer">
  Model: {{model}} | Duration: {{duration}}
  Baseline: {{baseline_folder}} at {{baseline_sha}}
  Generated: {{timestamp}}
</div>
```
* *样式规则:* *
—自包含：所有CSS在内联`<style>`块。无CDN链接。
-颜色约定：绿色(#28a745) =固定，红色(#dc3545) =新的漏洞，琥珀色(#fd7e14) =以前未识别，灰色(#6c757d) =仍然存在，蓝色(#2171b5) =修改
-打印友好：包括`@media print`样式
-使用相同的CSS颜色约定上面定义的视觉一致性

---

阶段5：验证

# # # 5。标准的验证

针对新报告运行标准`verification-checklist.md`（阶段0-9）。增量报告必须通过所有标准质量检查，因为它是一个独立的报告。将输出文件夹的绝对路径委托给子代理，以便它可以读取报告文件。

# # # 5 b。增量式验证在标准验证通过之后，从`experiment-history/mode-c-verification-suite.md`运行特定于增量的检查（阶段1 - 9,33检查）。这些验证:
-结构连续性（每一个旧项目都被考虑在内）
-代码验证状态的准确性（例如，“fixed”实际上是根据代码差异进行验证的）
-以前未确定的分类（根据基线工作树进行验证）
- DFD一致性（旧节点存在，新节点区分）
-独立质量（不引用旧报告）
-比较摘要的准确性（计数与库存相符）
-需求验证的完整性
-边缘情况（合并，拆分，重写）
-Metrics/JSON完整性

# # # 5 c。修正工作流1. 收集所有PASS/FAIL结果
2. 对于每个FAIL→应用检查的“失败补救”操作
3. 重新运行失败的检查以确认它们已通过
4. 在两次纠正尝试之后，将剩余的失败升级为需要验证
5. 记录结束时间并生成执行摘要

---

⛔增量分析特有的规则

这些规则是对`orchestrator.md`中34条强制性规则的补充（而不是替代）：

规则十一：保留旧的报告评估判断

当新的分析将分配与旧报告不同的TMT类别、组件类型、层或威胁相关性时→保留旧报告的值。将分歧记录在需求验证中：
-旧值
-新的分析建议值
- 1-2句推理
-用户应该检查什么

**例外：**事实更正（文件路径，git元数据，算术）被静默更正并在报告元数据中注明。规则2：没有静默覆盖

报告主体使用OLD值进行评估判断。分歧需要确认。用户必须明确确认任何重新分类。

规则I3：必须验证先前未识别

每个`previously_unidentified`分类必须包括来自基线工作树的证据。分析人员必须实际阅读引用的file/line处的旧代码，并确认存在漏洞模式。不要基于“它可能在那里”来猜测。

规则I4: Fixed必须经过代码验证

每个`fixed`分类都必须引用解决漏洞的特定代码更改。像“团队解决了这个问题”这样的一般性陈述是不可接受的——表明差异。

规则I5: new_in_modified要求更改归属每个`new_in_modified`发现必须识别引入漏洞的特定代码更改。引用造成问题的diff块、新函数、新配置值或新依赖项。

规则I6：不要删除基线工作树

基线工作树可以被将来的增量分析重用。不要在上面运行`git worktree remove`。工作树路径记录在报表元数据中，供参考。

规则I7：改变状态一致性

组件的`change_status`必须与其威胁和发现状态一致：
-`unchanged`组件→其威胁应该是`still_present`（或`previously_unidentified`对于新发现的威胁在未更改的代码）
—`removed`组件→所有threats/findings必须为`removed_with_component`-`modified`组件→至少一个威胁应该是`modified`，`fixed`，或`new_in_modified`-`new`组件→其所有威胁必须为`new_code`规则I8：发扬光大，不要复制“结转”意味着重新生成一个表示相同内容的threat/finding条目—而不是直接复制粘贴旧的报告文本。重新生成的条目应：
—使用相同的ID
-引用当前文件路径（即使未更改）
-使用现在时描述当前代码
—包含`[STILL PRESENT]`注释

---

##总结：阶段性检查表

|阶段|动作|成功标准||-------|--------|-----------------|
|0 |设置，验证输入，工作树|所有输入存在，工作树可访问|
| 1 |加载旧库存骨架|所有数组已填充，指标匹配|
|每个组件有一个`change_status`|
| 3 |扫描新组件|新组件识别，遗漏的组件标记|
| 4 |生成所有报表文件| 8-9写入输出文件夹|的文件
| 5 |验证（标准+增量）|所有检查通过或升级到需要验证|