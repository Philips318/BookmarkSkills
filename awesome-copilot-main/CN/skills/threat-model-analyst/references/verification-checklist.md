#验证清单-分析后质量门

此文件是在威胁模型报告最终确定之前必须通过的所有验证规则的单一事实来源。它被设计成与输出文件夹路径一起传递给验证子代理。

b> **权限层次：**该文件包含检查规则（pass/fail质量门标准）。生成被检查内容的AUTHORING规则位于`orchestrator.md`中。一些规则出现在两个文件中是为了可见性—如果它们发生冲突：`orchestrator.md`优先考虑创作决策（如何编写），这个文件优先考虑pass/fail标准（什么构成有效的输出）。不要从任何一个文件中删除规则以“重复数据删除”-重叠是有意的可见性。**使用时：**写完所有输出文件（0.1-architecture.md到0-assessment.md）后，运行该文件中的每个检查。如果任何检查失败，在结束之前修复问题。

**子代理委托：**编排器可以通过以下提示将整个文件委托给验证子代理：
> "读取[verification-checklist.md]（./verification-checklist.md）。对于每次检查，检查指定的输出文件，并报告带有证据的PASS/FAIL。修复任何故障。”

---

内联快速检查（每次文件写入后立即运行）目的：**这些是轻量级的自检，写代理在创建每个文件后立即运行-不延迟到步骤10。由于代理刚刚编写了文件，因此内容仍然处于活动上下文中，这使得这些检查非常有效。
>
> **使用方法：**在写入每个文件之前，从`skeletons/skeleton-*.md`读取相应的骨架。在每次`create_file`调用之后，扫描您刚刚为这些模式编写的内容。如果任何检查失败，在继续下一步之前立即修复该文件。
>
b> **框架遵从规则：**每个输出文件必须遵循其框架的节顺序，表列标题和标题名称。不要在骨架中添加sections/tables。不要重命名骨架标题。写完`3-findings.md`后：
-[]第一个查找标题以`### FIND-01:`开头（不是`F01`，`F-01`，或`Finding 1`）
-[]每个发现都有这些精确的行标签：`SDL Bugbar Severity`，`Remediation Effort`,`Mitigation Type`,`Exploitability Tier`,`Exploitation Prerequisites`,`Component`—[]每个CVSS值都包含前缀`CVSS:4.0/`-[]每个`Related Threats`单元格包含`](2-stride-analysis.md#`（超链接，非纯文本）
-[]每个发现都有`#### Description`，`#### Evidence`，`#### Remediation`和`#### Verification`子标题（不是`Recommendation`，不是`Impact`，不是`Mitigation`，不是粗体`**Description:**`段落）-正好4个子标题，没有额外的
[]每个`#### Description`章节至少有两句技术细节（不是单句存根）
[]每个`#### Evidence`部分都引用了特定的文件路径，行号或配置键（而不是像“在代码库中找到”这样的通用语句）
-[]每个发现都有所有10个强制属性行：`SDL Bugbar Severity`，`CVSS 4.0`,`CWE`,`OWASP`,`Exploitation Prerequisites`,`Exploitability Tier`,`Remediation Effort`,`Mitigation Type`,`Component`,`Related Threats`—[]每个CWE值都是一个超链接：包含`](https://cwe.mitre.org/`（不像`CWE-79`那样纯文本）
—[]每个OWASP值都使用后缀`:2025`，而不是`:2021`。
[]调查结果按等级分类（等级1/2/3标题），而不是按严重程度分类（no`## Critical Findings`）
- [] **Tier-Prerequisite consistency (inline)**：对于每个发现，使用规范映射：`None`→T1；`Authenticated User`/`Privileged User`/`Internal Network`/`Local Process Access`→T2;`Host/OS Access`/`Admin Credentials``Physical Access``{Component} Compromise`/组合→T3。⛔禁止使用`Application Access`和`Host Access`。
-[]计数查找标题-它们必须是连续的：FIND-01， FIND-02, FIND-03…
-[]无时间估计：搜索`~`，`Sprint`,`Phase`,`hour`,`day`，`week`-不得出现
-[] **威胁覆盖验证表**出现在文件末尾，列为`Threat ID | Finding ID | Status`-[] **覆盖率表状态值**使用表情符号前缀：`✅ Covered (FIND-XX)`，`✅ Mitigated (FIND-XX)`，`🔄 Mitigated by Platform`-不是纯文本，如“查找”，“减轻”、“覆盖”
-[] **覆盖率表的列名**应该是`Threat ID | Finding ID | Status`，而不是`Threat | Finding | Status`写完`0-assessment.md`后：
-[]第一个`## `标题是`## Report Files`-[]统计`## `标题-确切的7个名称：报告文件，执行摘要，行动摘要，分析背景和假设，参考文献，报告元数据，分类参考
—[]标题包含`&`，不包含`and`：搜索`Analysis Context & Assumptions`-[]计算`---`分隔线-至少5行
- []`### Quick Wins`标题存在
- []`### Priority by Tier and CVSS Score`标题存在于动作摘要下，在快速获胜之前
-[] **优先级表最多10行**：按优先级和CVSS评分表统计数据行-必须≤10
-[] **优先级表排序顺序**：所有Tier 1的结果首先出现，然后是Tier 2，然后是Tier 3。在每一层中，CVSS分数越高越优先。❌T2发现出现在T1发现之前→FAIL
-[] **优先级表查找超链接**：每个查找单元格都是一个超链接`[FIND-XX](3-findings.md#find-xx-title-slug)`。搜索对于每一行中的`](3-findings.md#`-必须存在。❌纯文本`FIND-XX`无链接→FAIL
-[] **优先级表锚定分辨率**：对于每个超链接，验证锚定段与3-findings.mdAS WRITTEN中的实际`### FIND-XX:`标题匹配。从标题文本（小写、空格到连字符、去掉特殊字符）计算锚。❌如果任何标题包含状态标签，如`[STILL PRESENT]`或`[NEW]`，那是一个失败-状态标签不能出现在标题中（见阶段2检查）。锚点应该从干净、无标签的标题文本中计算。
- [] **Action Summary层超链接**:Action Summary表中的第1、2、3层单元格是指向`3-findings.md#tier-N`锚点的超链接
- []`### Needs Verification`标题存在
- []`### Finding Overrides`标题存在
- [] **Action Summary有4行数据**:Tier 1， Tier 2, Tier 3, Total。在Action Summary表中搜索`| Mitigated |`或`| Platform |`或`| Fixed |`-如果找到则失败。这个它们不是分开的层。
—[]**Git Commit include date**:`| Git Commit |`行必须同时包含SHA和提交日期（例如`f49298ff`(`2026-03-04`)）。如果只显示哈希而不显示日期→FAIL。
- [] **Baseline/Target提交包含日期**（增量模式）：`| Baseline Commit |`和`| Target Commit |`行必须在SHA旁边每个包含一个日期。
[]`### Security Standards`和`### Component Documentation`标题存在（两个参考子节）
—报表元数据表中存在[]`| Model |`行
—“报表元数据表”中存在“[]`| Analysis Started |`”行
—“报表元数据”表中存在“[]`| Analysis Completed |`”行
—“报表元数据”表中存在“[]`| Duration |`”行
-[]元数据值：检查元数据值单元格中的‘` `’ '
-[] **报表文件表第一行**:`0-assessment.md`是第一行数据（不是`0.1-architecture.md`）
-[] **报告文件完整性**：输出文件夹中生成的每个`.md`和`.mmd`文件r在Report Files表中有相应的行（`threat-inventory.json`被有意排除在外）
-[] **报表文件条件行**:`1.2-threatmodel-summary.mmd`和`incremental-comparison.html`行只有在这些文件实际生成时才会显示
-[] **注意威胁计数blockquote**：执行摘要包含`> **Note on threat counts:**`段
-[] **边界计数**：执行摘要中的边界计数与`1-threatmodel.md`中的实际信任边界表行数匹配
-[] **行动摘要分级优先级**：一级=🔴严重风险，二级=🟠高风险，三级=🟡中度风险。这些是固定的-永远不会根据计数修改。
-[] **风险评级标题**没有表情符号：`### Risk Rating: Elevated`而不是`### Risk Rating: 🟠 Elevated`写完`0.1-architecture.md`后：
-[]计数`sequenceDiagram`出现次数-至少3次
[]前3个序列图有`participant`线和`->>`消息箭头（不是空的图块）
—[]关键组件表行数匹配组件图节点数
[]每个Key Components表行使用PascalCase名称（不是kebab-case`my-component`或snake_case`my_component`）
-[]每个关键组件类型单元格是：`Process`，`Data Store`,`External Service`，`External Interactor`之一-没有像`Role`，`Function`这样的特殊类型
-[]技术堆栈表填满了所有5行：语言，框架，数据存储，基础设施，安全
[]`## Security Infrastructure Inventory`部分存在（没有丢失）
[]`## Repository Structure`部分存在（没有缺失）写完`1.1-threatmodel.mmd`后：
-[]第一行以`%%{init:`开头
—[]包含`classDef process`、`classDef external`、`classDef datastore`-没有Chakra UI颜色（`#4299E1`,`#48BB78`,`#E53E3E`）
- [] [qh
- [] DFD使用`flowchart LR`（不是`flowchart TB`） -搜索`flowchart`并验证方向是`LR`-[] **增量DFD样式（仅限增量模式）**：如果存在新组件，则验证`classDef newComponent fill:#d4edda,stroke:#28a745`存在并且新组件节点使用`:::newComponent`（而不是`:::process`）。如果存在已删除的组件，请使用灰色虚线样式验证`classDef removedComponent`。❌`newComponent fill:#6baed6`（与进程相同的蓝色）→FAIL（视觉上不可见）。写完`2-stride-analysis.md`后：
[]`## Summary`出现在任何`## ComponentName`部分之前（检查行号）
—[]汇总表有列：`| Component | Link | S | T | R | I | D | E | A | Total | T1 | T2 | T3 | Risk |`—查找`| S | T | R | I | D | E | A |`进行验证
-[]汇总表S/T/R/I/D/E/A列包含数值（0,1,2,3…），不是每个组件都有相同的1
-[]每个组件都有`#### Tier 1`，`#### Tier 2`，`#### Tier 3`子标题
-[]在`## `标题中没有`&`，`/`,`(`,`)`,`:`-[] **标题中没有状态标签（任何文件）**：搜索所有`.md`文件中的`^##.+\[Existing\]`，`^##.+\[Fixed\]`,`^##.+\[Partial\]`,`^##.+\[New\]`,`^##.+\[Removed\]`，`###`标题相同。也可以查看旧版本：`^##.+\[STILL`，`^##.+\[NEW`,`^###.+\[STILL`,`^###.+\[NEW CODE`。❌标题中的标签破坏锚链接并污染ToC。状态必须作为blockquote （`> **[Tag]**`）出现在节体的第一行，而不是在标题中。
-[] **关键- A =滥用，从未授权**：搜索文件中的`| Authorization |`。如果任何匹配是STRIDE类别标签（不在威胁描述句子中）→立即用`| Abuse |`替换修复。STRIDE-A中的“A”代表“滥用”（业务逻辑滥用、工作流操纵、功能滥用）。这是观察到的最常见的错误。
- [] **N/A项不计算**：如果任何组件在STRIDE类别中有`N/A — {justification}`，请验证类别在Summary表中显示的是`0`（而不是`1`）
- [] **STRIDE状态值**：每个威胁行的状态列使用`Open`，`Mitigated`，`Platform`中的一个。没有`Partial`、`N/A`、`Accepted`或特别值。
-[] **平台比率**：计算具有`Platform`状态的威胁与总威胁。如果>为20%（独立）或>为35% （K8s运营商）→重新检查每个平台入口。
- [] **STRIDE列算术**：对于汇总表的每一行，验证S+T+R+I+D+E+A = Total ANDT1+T2+T3 =总
-[] **威胁表中的完整类别名称**：类别列使用全名(`Spoofing`,`Tampering`,`Information Disclosure`,`Denial of Service`,`Elevation of Privilege`,`Abuse`) -非缩写（`S`,`T`,`DoS`,`EoP`）
- [] **N/A表存在**：每个组件部分都有一个`| Category | Justification |`表，列出没有威胁的STRIDE类别-不是prose/bullet-point格式
—[]**链接列是独立的**：汇总表第二列为`Link`，值为`[Link](#anchor)`—组件名不包含内嵌的超链接
-[] **漏洞层第四列：层定义表必须有第四列命名为`Assignment Rule`（不是`Example`，`Description`,`Criteria`）写入`incremental-comparison.html`后（仅限增量模式）：
- [] HTML在指标栏中包含`Trust Boundaries`或`Boundaries`-搜索文本“边界”
- [] STRIDE热图有13列：Component， S， T， R， I， D， E， A, Total, divider, T1, T2， T3 -在HTML中搜索`T1`和`T2`和`T3`未识别的状态信息只出现在彩色状态卡中，而不是在指标栏中显示小的内联徽章
-[]在热图中没有`| Authorization |`作为STRIDE类别标签-在热图行中搜索“Authorization
- [] **HTML计数匹配标记计数**:HTML热图中的威胁总数必须等于从`2-stride-analysis.md`开始的总计行。如果它们不同，则从STRIDE汇总数据重新生成HTML热图。HTML中的T1+T2+T3总数也必须匹配。
-[] **比较卡存在**:HTML包含`comparison-cards`div与3卡：基线(哈希+日期+评级)，目标（哈希值+日期+评级），趋势（方向+持续时间）
- [] ** git日志中的提交日期**：比较卡中的基线和目标日期必须匹配实际提交日期（不是今天的日期，也不是分析运行日期）
-[] **代码更改框**：第5个指标框显示提交计数和PR计数（不是“时间间隔”）
-[] **没有时间间隔框**：搜索“时间间隔”-不能出现在指标栏中
—[]**状态卡简洁**：每个状态卡的`card-items`div只能包含一个简短的总结句。❌威胁id （T06. net）年代,T02。E)，查找id (FIND-14)，或卡片中列出的组件名称→FAIL。在`card-items`div中搜索`T\d+\.`和`FIND-\d+`。详细的项目细分属于Threat/Finding状态细分部分，而不属于汇总卡。在编写任何增量报告文件后（增量模式-内联检查）：
-[] **仅显示简化标签**：搜索所有`.md`文件，查找旧式标签：`[STILL PRESENT]`、`[NEW CODE]`、`[NEW IN MODIFIED]`、`[PREVIOUSLY UNIDENTIFIED]`、`[PARTIALLY MITIGATED]`、`[REMOVED WITH COMPONENT]`、`[MODIFIED]`。❌任何匹配→失败。替换为简化标签：`[Existing]`，`[Fixed]`,`[Partial]`,`[New]`,`[Removed]`。
-[] **有效的显示标签**：每个finding/threat注释使用5个简化标签中的一个：`[Existing]`，`[Fixed]`,`[Partial]`,`[New]`,`[Removed]`。标签必须以blockquote形式出现在正文的第一行：`> **[Tag]**`。
—[]**组件状态简化**：组件状态列只能使用：`Unchanged`，`Modified`,`New`,`Removed`。❌`Restructured`→FAIL（使用`Modified`代替）。
-[] **更改汇总表使用简化标签**：威胁状态表有4行（Existing/Fixed/New/Removed）。查找状态表有5行（Existing/Fixed/Partial/New/Removed）。❌像xqz2这样的旧式行56xqz,`New (Code)`,`Partially Mitigated`→FAIL写完`threat-inventory.json`（inline check）后：
—[]**JSON威胁计数匹配STRIDE文件**：计数`2-stride-analysis.md`中唯一的威胁id （grep`^\| T\d+\.`）。这个计数必须等于JSON中的`threats`数组长度。如果STRIDE有比JSON更多的威胁，则在序列化期间删除威胁。重新构建JSON。
- [] **JSON参数内部一致**:`metrics.total_threats`必须等于`threats`数组长度。`metrics.total_findings`必须等于`findings`数组长度。

###写入`0-assessment.md`（计数验证）后：
[]执行摘要中的元素计数与实际元素表行数匹配（如果需要，重新读取`1-threatmodel.md`）
-[]查找计数匹配实际的`### FIND-`标题计数在`3-findings.md`-[]威胁计数匹配`2-stride-analysis.md`汇总表中的总数

---

阶段0 -共偏差扫描这些是在以前所有运行中最常观察到的偏差。生成输出后，扫描每个输出文件以查找这些特定模式。每个检查都有一个要搜索的错误模式和一个正确模式。

**使用方法：**每次检查，grep/scan输出文件的错误模式。如果找到→失败。然后验证是否存在正确的模式。此阶段捕获生成模型不顾指令而经常犯的错误。

0.1结构偏差-[] **搜索`## Critical Findings`，`## Important Findings`,`## High Findings`。这些必须不存在。❌`## Critical Findings`→✅`## Tier 1 — Direct Exposure (No Prerequisites)`** -`2-stride-analysis.md`中的每个组件必须有`#### Tier 1`，`#### Tier 2`，`#### Tier 3`子标题。❌每个组件单个平面表→✅三个独立的层子部分
-[] **缺少可利用层或修复工作的发现** -每个`### FIND-`块在`3-findings.md`必须包含`Exploitability Tier`和`Remediation Effort`行。❌缺少任何一个字段→✅都是必选的
- [] **STRIDE summary缺少tier列** -`2-stride-analysis.md`中的汇总表必须包含`T1`，`T2`，`T3`列。❌只有S/T/R/I/D/E/A/Total→✅也必须有T1/T2/T3/Risk列
- [] **STRIDE Summary在底部** -搜索`## Summary`与第一个`## Component`的行号。❌各部件后汇总→✅各部件前汇总在`## Exploitability Tiers`之后
-[] **可利用层表列** -`2-stride-analysis.md`中的层定义表必须有这4列：`Tier | Label | Prerequisites | Assignment Rule`。❌`Example`,`Description`，`Criteria`作为第四列→✅仅`Assignment Rule`。分配规则单元格必须包含严格的规则文本，而不是特定于部署的示例。0.2文件格式偏差**检查是否有`.md`文件以` `'`markdown `或` `' '`markdown `开头。❌` `'`markdown\n# Title`→✅`# Title`在第一行
**检查`.mmd`文件是否以` `'`plaintext `或` `'`mermaid `开头。❌` `'`mermaid\n%%{init:`→✅`%%{init:`在第一行
-[] **在所有`.md`文件中搜索`⛔`、`RIGID TIER`、`Do NOT use subjective`、`MANDATORY`、`CRITICAL —`、`decision procedure`。这些是内部技能说明，不能出现在报告输出中。❌任意匹配→✅零匹配。删除任何泄露的指令行。
-[] **嵌套的重复输出文件夹** -检查输出文件夹是否包含同名的子文件夹（例如，`threat-model-20260307-081613/threat-model-20260307-081613/`）。❌子文件夹存在→✅删除嵌套副本。输出文件夹应该只包含文件，不包含子文件夹。
- [] **STRIDE-A“授权”** -在`| Authorization |`或`**Authorization**`中搜索`2-stride-analysis.md`作为STRIDE类别名称。STRIDE-A中的A总是“滥用”，而不是“授权”。❌授权被用作STRIDE类别的任何匹配→✅替换为“滥用”。注意：当“授权”出现在威胁描述中（例如，“授权头”，“缺乏授权检查”）时，不要替换“授权”。0.3评估部分偏差-[] **操作摘要名称错误** -搜索“`Priority Remediation Roadmap`”、“`Top Recommendations`”、“`Key Recommendations`”、“`Risk Profile`”。❌这些名字中的任何一个→✅`## Action Summary`-[] **单独的推荐部分** -搜索`### Key Recommendations`或`### Top Recommendations`作为独立的部分。❌单独部分→✅行动总结是建议
-[] **缺少Quick Wins小节** -在Action Summary下搜索`### Quick Wins`。❌缺失→✅存在（如果没有低努力T1发现，请注明）
-[] **缺少威胁计数上下文** -在执行摘要中搜索`> **Note on threat counts:**`blockquote。❌失踪→✅在场
-[] **缺少分析上下文和假设** -搜索`## Analysis Context & Assumptions`。❌缺失→✅有`### Needs Verification`和`### Finding Overrides`子节
-[] **缺少强制性评估部分** -验证所有7项的存在：报告文件、执行摘要、行动摘要、分析背景和假设、参考文献、报告元数据、分类的参考。❌缺人→✅7人全部到场参考和元数据偏差

-[] **参考文献参考平面表格** -搜索`| Reference | Usage |`模式❌两列平面表格→✅两个子部分：`### Security Standards`与`| Standard | URL | How Used |`和`### Component Documentation`与`| Component | Documentation URL | Relevant Section |`-[] **参考文献缺少URL ** -参考文献咨询表中的每一行都必须有一个完整的`https://`URL。❌缺失URL列或空URL→✅每行完整URL
-[] **报告缺少元数据模型** -搜索`| **Model** |`或`| Model |`行。❌缺失→✅有实际型号名称
-[] **报告缺少时间戳的元数据** -搜索`Analysis Started`、`Analysis Completed`、`Duration`行。❌任何缺失→✅所有三个都有计算值

发现质量偏差- [] **CVSS得分没有向量或缺少前缀** - Grep每个发现的CVSS领域。取值必须匹配模式：`\d+\.\d+ \(CVSS:4\.0/AV:`。特别检查`CVSS:4.0/`前缀—最常见的偏差是输出没有这个前缀的向量（`AV:N/AC:L/...`）。❌`9.3`（仅限分数）→❌`9.3 (AV:N/AC:L/...)`（无前缀）→✅`9.3 (CVSS:4.0/AV:N/AC:L/AT:N/PR:N/UI:N/VC:H/VI:H/VA:H/SC:N/SI:N/SA:N)`- [] **CWE不带超链接** - Grep为`CWE-\d+`，不带前面的`[`。❌`CWE-78: OS Command Injection`→✅`[CWE-78](https://cwe.mitre.org/data/definitions/78.html): OS Command Injection`- [] **OWASP`:2021`后缀** -`:2021`的Grep。❌`A01:2021`→✅`A01:2025`-[] **相关威胁为纯文本** - Grep`Related Threats`行模式没有`](`。❌`T-02, T-17, T-23`→✅`[T02.S](2-stride-analysis.md#component-name), [T17.I](2-stride-analysis.md#other-component)`-[] **发现id顺序紊乱** -检查FIND-NN的id顺序：FIND-01、FIND-02、FIND-03…❌`FIND-06`出现在`FIND-04`之前→✅从上到下顺序编号
- [] **CVSS AV:L或PR:H与一级** - Grep每一级发现的CVSS矢量R表示`AV:L`或`PR:H`。❌Tier 1， local-only访问→✅降级为T2/T3- [] ** localhost -only或admin-only在Tier 1中发现** -检查部署上下文：air- gapping， localhost， single-admin服务不应该是Tier 1。❌一级管理员级→✅T2/T3-[] **输出时间估计** - Grep for`~1 hour`，`Sprint`,`Phase 1`,`(hours)`,`(days)`,`(weeks)`,`Immediate`。❌任意调度语言→✅仅支持`Low`/`Medium`/`High`工作标签
-[] **承保表中“已接受风险”** -为`Accepted Risk`填写`3-findings.md`。❌任何匹配→失败。该工具没有接受风险的权限。每个`Open`威胁必须有一个发现。将所有`⚠️ Accepted Risk`替换为`✅ Covered`并创建相应的结果。0.6图偏差-[] **错误的调色板** - Grep所有`#[0-9a-fA-F]{6}`在`.mmd`文件和美人鱼块。❌`#4299E1`,`#48BB78`,`#E53E3E`,`#2B6CB0`,`#2D3748`,`#2F855A`,`#C53030`（Chakra UI）→✅只允许：`#6baed6`，`#2171b5`,`#fdae61`,`#74c476`,`#238b45`,`#e31a1c`,`#666666`,`#ffffff`,`#000000`-[] **自定义themeVariables颜色** -搜索初始化块`secondaryColor`，`tertiaryColor`，或`primaryTextColor`。❌`"primaryColor": "#2D3748", "secondaryColor": "#4299E1"`→✅只有`'background': '#ffffff', 'primaryColor': '#ffffff', 'lineColor': '#666666'`在themeVariables
-[] **缺少摘要MMD** -统计`1.1-threatmodel.mmd`中的节点和子图。如果元素>5或子图> 4，则`1.2-threatmodel-summary.mmd`必须存在。❌达到阈值，但缺少文件→✅创建带有摘要图的文件
-[] **独立侧车节点（仅限K8s）** -搜索名为`MISE`，`Dapr`,`Envoy`,`Istio`，`Sidecar`的节点图作为单独的条目。❌`MISE(("MISE Sidecar"))`→✅`InferencingFlow(("Inferencing Flow<br/>+ MISE"))`- [] ** pod内localhost流（仅K8s）** -搜索xqz共存容器之间的425xqz箭头。❌出席→✅缺席（隐式）
-[] **缺少序列图** -`0.1-architecture.md`中的前3个场景必须每个都有一个`sequenceDiagram`块。❌少于3→✅至少3个
-[] **技术特定的差距** -对于repo中的每一项技术（Redis, PostgreSQL, Docker, K8s,ML/LLM， NFS等），验证至少一个发现或文档缓解存在。❌有技术但没有覆盖→✅每种技术规范模式检查

-[] **查找标题模式** -所有查找标题匹配`^### FIND-\d{2}: `（从不匹配`F01`，`F-01`,`Finding 1`）
- [] **CVSS前缀模式** -所有CVSS字段匹配`\d+\.\d+ \(CVSS:4\.0/AV:`（从不匹配`AV:N/AC:L/...`）
-[] **相关威胁链接模式** -每个相关威胁令牌匹配`\[T\d{2}\.[STRIDEA]\]\(2-stride-analysis\.md#[a-z0-9-]+\)`-[] **评估部分标题精确设置** -正是`0-assessment.md`中的`##`标题：报告文件，执行摘要，行动摘要，分析上下文与假设，参考文献，报告元数据，分类参考
-[] **没有禁止标题**没有`##`或`###`标题包含：严重性分布，架构风险区域，方法说明，可交付成果，优先修复路线图，关键建议，顶级建议

---

阶段1 -每个文件的结构检查这些检查独立地验证每个文件。它们可以并行运行。

所有`.md`文件

-[] **没有代码围栏包装**：没有`.md`文件以` `'`markdown `或` `' '`markdown `开头。每个`.md`文件的第一行必须以`# Heading`开头。如果任何文件被包装在栅栏中，立即剥离第一行和最后一行。
-[] **不允许`.mmd`代码-fence包装**:`.mmd`文件不能以` `'`plaintext `或` `'`mermaid `开头。它必须以`%%{init:`开头作为第一个字符。如果包裹，剥去栅栏线。
-[] **没有空文件**：每个文件都有标题以外的实质性内容。

### 1.2`0.1-architecture.md`-[] **要求的部分呈现**：系统目的、关键组件、组件图、顶级场景、技术堆栈、部署模型、存储库结构
-[] **组件图存在**作为美人鱼`flowchart`在` `'`mermaid `代码围栏
-[] **使用的架构样式** -不使用DFD圆圈`(("Name"))`。必须使用`["Name"]`或`(["Name"])`与`service`/`external`/`datastore`classDef名称
-[] **至少3个场景**有美人鱼`sequenceDiagram`块
-[] **没有为0.1-architecture.md创建单独的`.mmd`文件** -所有的图表都是内联的
-[] **组件图元素匹配关键组件表** -表中的每一行在图中有一个对应的节点，反之亦然。计数并验证计数是否相等。
-[]顶级场景反映了实际的代码路径，而不是假设的用例
-[] **部署模型包含网络详细信息** -必须至少提到：端口号ber OR绑定地址或网络拓扑1.3`1.1-threatmodel.mmd`-[] **文件存在**与纯美人鱼代码（没有标记包装，没有` `'`mermaid `栅栏）
—[]**以**`%%{init:`块开头
-[] **包含**`classDef process`、`classDef external`、`classDef datastore`-[] **使用DFD形状**：圆形`(("Name"))`表示进程，矩形`["Name"]`表示外部，圆柱体`[("Name")]`表示数据存储

1.4`1-threatmodel.md`-与`1.1-threatmodel.mmd`相同的图表内容** -美人鱼区块内容的逐字节比较（不包括` `'`mermaid `围栏包装）
-[] **元素表**呈现列：元素，类型，TMT类别，描述，信任边界
-[] **数据流表**显示：ID、来源、目标、协议、描述
-[] **信任边界表**目前的列：边界，描述，包含
-[] **使用的TMT类别id ** -元素表的TMT类别列使用`tmt-element-taxonomy.md`中的特定TMT元素id（例如，`SE.P.TMCore.WebSvc`,`SE.EI.TMCore.Browser`）。不是通用的标签，如`Process`，`External`。
-[] **流ID匹配DF\d{2}模式** -数据流表中的每个流ID使用`DF01`，`DF02`等格式。不是`F1`，`Flow-1`,`DataFlow1`。
-[] **如果>有15个元素或>有4个边界**:`1.2-threatmodel-summary.mmd`必须存在并且`1-threatmodel.md`必须包含一个“Summary View”部分总结性图表和“详细映射的总结性”表。**验证：**计数节点（行匹配`[A-Z]\d+`与形状语法）和子图在`1.1-threatmodel.mmd`。如果count超过阈值，但`1.2-threatmodel-summary.mmd`不存在→**FAIL -在继续**之前创建汇总图。1.5`2-stride-analysis.md`-[]可利用层部分**出现在顶部的层定义表
[]总结表**出现在各个组件部分之前（紧接在可利用等级之后，而不是在文件底部）
—[]**汇总表**包括：Component、Link、S、T、R、I、D、E、A、Total、T1、T2、T3、Risk
-[] **每个组件**都有`## Component Name`标题，后面跟着1级，2级，3级子节（即使是空的，也都有）
-[] **空层**使用“*未识别此组件的N层威胁。*”
-[] **锚定安全标题**：此文件中的`## `标题不包含以下字符：`&`，`/`,`(`,`)`,`.`,`:`,`'`,`"`,`+`,`@`,`!`。替换：`&`→`and`，`/`→`-`，括号→省略，`:`→省略。
- [] **Pod Co-location line**为K8s comp提供列出并列侧边车的组件
- [] **STRIDE状态值** -每个威胁行的状态列使用确切的一个：`Open`，`Mitigated`,`Platform`。没有`Partial`、`N/A`或其他特别值。
-[] **一个标签为滥用的类别** -搜索`2-stride-analysis.md`为`| Authorization |`作为STRIDE类别标签如果找到失败。STRIDE-A中的“A”永远是“滥用”（业务逻辑滥用、工作流操纵、功能滥用），而不是“授权”。还要检查N/A条目：`Authorization — N/A`是错误的，必须是`Abuse — N/A`。
- [] **STRIDE-Coverage Consistency** -对于每个威胁ID， STRIDE Status和Coverage表Status必须一致：
- STRIDE`Open`→覆盖`✅ Covered (FIND-XX)`（查找需要修复的文档漏洞）
- STRIDE`Mitigated`→Coverage`✅ Mitigated (FIND-XX)`（查找现有控制团队构建的文档）
-步幅`Platform`→覆盖`🔄 Mitigated by Platform`-如果STRIDE显示`Partial`，但覆盖范围为s`Mitigated by Platform`→**冲突。修好它。* *
-如果STRIDE显示`Open`，而Coverage显示`⚠️ Needs Review`→只有当先决条件≠`None`时才有效1.6`3-findings.md`-[] **按层组织**精确使用：`## Tier 1 — Direct Exposure (No Prerequisites)`，`## Tier 2 — Conditional Risk (...)`,`## Tier 3 — Defense-in-Depth (...)`-[] **不按严重程度组织** -没有`## Critical Findings`或`## Important Findings`标题
-[] **每个发现**都具有所有强制性属性：SDL bug严重性，CVSS 4.0， CWE， OWASP（后缀为`:2025`），利用先决条件，可利用层，补救努力，缓解类型，组件，相关威胁
-[] **缓解类型有效值** -每个发现的`Mitigation Type`行都是：`Redesign`，`Standard Mitigation`,`Custom Mitigation`,`Existing Control`,`Accept Risk`，`Transfer Risk`之一。❌缩写形式（`Custom`,`Accept`,`Standard`）或虚构值→FAIL
- [] **SDL严重性有效值** -每个发现的严重性是：`Critical`，`Important`,`Moderate`，`Low`之一。❌`High`,`Medium`,`Info`→FAIL
-[] **修复工作有效值** -每个发现的努力是：`Low`，`Medium`，`High`之一。❌时间估算ates， sprint标签→FAIL
- [] **CVSS 4.0有完整的向量**：每个发现的CVSS值包括数字得分和完整的向量字符串（例如，`9.3 (CVSS:4.0/AV:N/AC:L/AT:N/PR:N/UI:N/VC:H/VI:H/VA:H/SC:N/SI:N/SA:N)`）。只考虑分数是不被接受的。
- [] **CWE格式**：每个CWE使用`CWE-NNN: Name`格式（不只是数字）
- [] **OWASP格式**：每个OWASP使用`A0N:2025`格式（从不使用`:2021`）
-[] **相关威胁**使用单个链接每个威胁ID:`[T01.S](2-stride-analysis.md#component-name)`-不像`[T01.S, T01.T](2-stride-analysis.md)`组链接
-[] **存在开发先决条件** -每个`### FIND-`块都有一行`| Exploitation Prerequisites |`-[] **组件字段当前** -每个`### FIND-`块有一行`| Component |`-[] **没有AV:L或PR:H的Tier 1 ** -对于每个Tier 1发现，验证其CVSS向量不包含`AV:L`或`PR:H`。如果发现→分级必须降级为T2/T3.-[] **层-先决条件一致性（必选）** -对于每个发现和每个威胁行，层MUST使用规范映射机械地遵循先决条件：
-`None`→T1（仅当组件的可达性=外部和Auth = No时有效）
-`Authenticated User`,`Privileged User`,`Internal Network`,`Local Process Access`→T2
-`Host/OS Access`,`Admin Credentials`,`Physical Access`,`{Component} Compromise`，任意`A + B`→T3
—**⛔禁止取值：**`Application Access`、`Host Access`→FAIL。替换为`Local Process Access`（T2）或`Host/OS Access`（T3）。
—**部署上下文规则（规则20）：**如果“部署分类”为`LOCALHOST_DESKTOP`或`LOCALHOST_SERVICE`，则禁止所有组件使用`None`。修复`Local Process Access`或`Host/OS Access`的先决条件，然后派生层。
- **暴露表交叉检查：**对于每个发现，在组件暴露表中查找其组件。发现的先决条件必须≥组件的`Min Prerequisite`。发现的层必须≥组件的`Derived Tier`。
- **不匹配=失败。**通过调整先决条件以匹配部署证据来修复E，则从prerequisite导出tier。
- **常见违规：**`None`在本地主机组件；`Application Access`(模糊的);T1以`Internal Network`为前提；T2以`None`为先决条件。
-[] **威胁覆盖验证表**存在于映射每个威胁ID的文件末尾→查找具有状态的ID
- [] **Coverage表的有效状态ONLY** - Coverage表中的每一行必须使用以下三种状态中的一种：`✅ Covered (FIND-XX)`，`✅ Mitigated (FIND-XX)`，或`🔄 Mitigated by Platform`。❌`⚠️ Accepted Risk`→FAIL（工具无法接受风险）。❌`⚠️ Needs Review`→FAIL（每个威胁都必须解决）。❌`—`无状态→FAIL（未解释的威胁）。
-[] **缓解与平台的区别** -对于每个`✅ Mitigated (FIND-XX)`条目：验证工程团队构建的现有安全控制（认证中间件，TLS，输入验证，文件权限）的查找文档。对于每个`🔄 Mitigated by Platform`：验证缓解来自a真正的外部系统（Azure AD, K8s RBAC， TPM）。如果“平台”描述了这个回购的代码→重新分类为`✅ Mitigated`并创建一个查找。
-[] **平台缓解率审计（必选）** -标记为`🔄 Mitigated by Platform`的威胁与总威胁的计数。如果平台> 20%→**警告：可能过度使用平台状态。**对于每个平台缓解的威胁，请验证所有三个条件：(1)缓解是在此repo的代码之外，(2)由不同的团队管理，(3)不能通过修改此代码来禁用。常见的违规：“认证中间件”（即此代码→应该是`Mitigated`），“本地主机上的TLS”（此代码→应该是`Mitigated`），“文件权限”（此代码→应该是`Mitigated`）。
-[] **覆盖率反馈回路验证** -覆盖率表编写完成后，验证：(1)每个STRIDE状态为`Open`的威胁在表中都有相应的发现。(2)无`—`破折号没有身份。(3)如果存在空白，就会产生新的发现来填补空白。Coverage表是一个FEEDBACK LOOP——它的目的是捕捉遗漏的发现并强制创建它们。如果在表写入后仍然存在空白，则没有执行循环。
-[] **承保表中“已接受风险”** -为`Accepted Risk`填写`3-findings.md`。❌任何匹配→失败。该工具没有接受风险的权限。每个`Open`威胁必须有一个发现。每个`Mitigated`威胁必须有一个发现记录团队的控制。
-[] **覆盖率表中的“需求审查”** -将`3-findings.md`替换为`Needs Review`。❌任何匹配→失败。“需求审查”已经被取代：威胁要么被覆盖（漏洞），要么被缓解（团队构建控制），要么被平台（外部系统）。没有延期的类别。`0-assessment.md`-[] **章节顺序**：报告文件→执行摘要→行动摘要→分析背景与假设→参考文献→报告元数据→分类参考（最后）
-[] **报告文件部分**是标题后的第一个部分
-[] **风险评级标题**没有表情符号：`### Risk Rating: Elevated`而不是`### Risk Rating: 🟠 Elevated`-[] **威胁计数上下文段落**以blockquote形式出现在执行摘要末尾
-[] **没有单独的建议部分** -行动摘要是建议
-[] **行动汇总表**提供级别，描述，威胁，发现，优先级列
-[] **行动总结为唯一名称**：没有标题为“优先补救路线图”、“首要建议”、“关键建议”或“风险概况”的部分。
-[] **快速获胜小节**存在（如果没有低成本的T1发现，则明确省略）
-[] **需要验证部分**当前资金er分析背景和假设
-[] **有两个子部分：`### Security Standards`和`### Component Documentation`-[] **参考资料表**使用三列完整的url:`| Standard | URL | How Used |`和`| Component | Documentation URL | Relevant Section |`-不是一个平面的`| Reference | Usage |`表
-[] **查找覆盖**使用表格格式，即使是空的（从不纯文本）
—[]**报告元数据**是分类参考前的最后一节，包含所有必填字段
-[] **元数据时间戳**来自实际的命令执行（不是从文件夹名称派生的）
-[] **模型**字段的现值与正在使用的模型匹配（例如，`Claude Opus 4.6`,`GPT-5.3 Codex`,`Gemini 3 Pro`）
-[] **分析开始**和**分析完成**字段显示UTC时间戳从`Get-Date`命令
-[] **持续时间**字段当前-从分析开始和分析完成时间戳计算
-[] **元数据值以反引号表示** -每个值ce报表元数据表中的所有内容都必须用反引号括起来。抽查至少5排。
-[] **段落之间的水平规则** -计算文件中匹配`---`的行数。必须≥6个（7个`## `截面每对1个）。
-[] **分类参考是最后一节** -`## Classification Reference`作为最后的`## `标题出现。包含一个单独的2列表（`Classification | Values`），其中包含以下行：可利用层、跨步+滥用、SDL严重性、补救努力、缓解类型、威胁状态、CVSS、CWE、OWASP。❌缺少部分或格式错误→失败。
-[] **分类引用是静态的** -表中的值必须完全匹配骨架（逐字复制）。没有额外的行，没有修改的描述。比较`skeleton-assessment.md`分类参考部分。
-[] **搜索：`Severity Distribution`，`Architecture Risk Areas`,`Methodology Notes`,`Deliverables`,`Priority Remediation Roadmap`,`Key Recommendations`,`Top Recommendations`。必须返回0匹配项。
- [] **Action Summary层优先级固定** -在`0-assessment.md`的Action Summary表中，检查“Priority”列：tier1 =`🔴 Critical Risk`, tier2 =`🟠 Elevated Risk`, tier3 =`🟡 Moderate Risk`。❌Tier 1 withLow/Moderate/Elevated→FAIL。❌Tier 2 withCritical/Low→FAIL。这些是固定的标签，无论threat/finding计数如何，都不会改变。
-[] **行动总结有所有3层** -行动总结表必须有1层，2层和3层的行，即使一个层有0个威胁和0个发现。缺失关卡→FAIL。---

阶段2 -图表渲染检查

在所有文件中运行所有美人鱼块。可以作为集中的子任务进行委派。

2.1 Init block

-[] **每个流程图**都有`%%{init}%%`块，`'background': '#ffffff'`为第一行
-[] **每个序列图**有完整的`%%{init}%%`主题变量块与`'background': '#ffffff'`- [] @ @ @ @ @ @ @ @ @ @ @ @ @ @ @ @ @ @ @ @ @ @ @ @ @ @ @ @ @ @ @ @ @ @ @ @ @ @ @ @ @ @ @ @所有元素的颜色都来自classDef。

2.2类定义和调色板-[] **每个`classDef`**包含`color:#000000`（明确黑色文本）
- [] **DFD图**使用`process`/`external`/`datastore`类名
-[] **架构图**使用`service`/`external`/`datastore`类名
- [] **EXACT使用的十六进制代码** - grep`.mmd`文件中的所有`#[0-9a-fA-F]{6}`值。唯一允许的填充颜色是：`#6baed6`，`#fdae61`,`#74c476`,`#ffffff`,`#000000`。唯一允许的笔画颜色是：`#2171b5`，`#d94701`,`#238b45`,`#e31a1c`,`#666666`。如果出现任何其他十六进制颜色（例如，`#4299E1`,`#48BB78`,`#E53E3E`,`#2B6CB0`），则图表未通过此检查。

### 2.3样式

-[] **每个流程图**有`linkStyle default stroke:#666666,stroke-width:2px`-[] **信任边界样式**使用`stroke:#e31a1c,stroke-width:3px`（不是`#ff0000`或`stroke-width:2px`）
-[] **架构图层样式**使用匹配边界的光填充（不是红色虚线信任边界）

2.4语法验证-[] **所有标签引号**:`["Name"]`，`(("Name"))`,`[("Name")]`,`-->|"Label"|`,`subgraph ID["Title"]`- [] **Subgraph/end对匹配**：每个`subgraph`都有一个闭合的`end`-[] **没有多余的字符**或未闭引号在任何美人鱼块

2.5 Kubernetes Sidecar规则

如果目标系统没有部署在Kubernetes上，则跳过本节。-[] **每个K8s业务节点**在节点标签中标注sidecars:`<br/>+ SidecarName`-[] **零独立侧车节点**：搜索所有命名为`MISE`，`Dapr`,`Envoy`,`Istio`，`Sidecar`的节点-这些节点不能作为单独的节点存在
-[] **无pod内部localhost流**：容器和它的侧车之间没有箭头（没有`-->|"localhost"`模式）
-[] **跨界sidecar流源自主机容器**：所有指向外部目标（Azure AD， Redis等）的箭头都来自主机容器节点，而不是来自独立的sidecar节点
-[] **元素表**：没有独立的sidecars行-在主机容器的描述列中描述

---

阶段3 -跨文件一致性检查

这些检查验证文件之间的关系。它们需要一起读取多个文件。

组件覆盖（架构→STRIDE→发现）- [] **`0.1-architecture.md`Key Components表中的每个组件**在`2-stride-analysis.md`中都有相应的`## Component`部分
- [] **`1-threatmodel.md`元素表中的每个元素**都是一个进程，在`2-stride-analysis.md`中有一个相应的`## Component`段
-[] **在`2-stride-analysis.md`中没有没有出现在元素表中的孤立组件**
-[] **汇总表组件计数**匹配文件中的`## Component`节数
-[] **组件计数精确匹配** -计算`0.1-architecture.md`关键组件表中的行数（不包括header/separator）。在`2-stride-analysis.md`中计算`## `组件部分（不包括`## Exploitability Tiers`、`## Summary`）。这些计数必须相等。

3.2数据流覆盖（STRIDE↔DFD）-[] **来自`1-threatmodel.md`数据流表的每个数据流ID** （`DF01`,`DF02`，…）至少出现在`2-stride-analysis.md`的一个“受影响的流”单元格中
-[] **在STRIDE分析中没有在数据流表中定义的孤立流id **

威胁到发现的可追溯性（STRIDE↔Findings）

这是最关键的交叉文件检查。它确保没有已识别的威胁被悄无声息地丢弃。

- [] **`2-stride-analysis.md`中的所有威胁ID**（例如：T01）。年代,T01。T1, T02。I)在`3-findings.md`的相关威胁字段中被至少一个发现所引用
[] **从`2-stride-analysis.md`的所有层表中收集所有威胁id **
-[] **收集“`3-findings.md`”中“相关威胁”字段中引用的所有威胁id **
-[] **覆盖率差距报告**：列出STRIDE中存在但在调查结果中缺失的任何威胁ID。如果存在空白→添加发现或将威胁分组到现有的相关发现中find -to-STRIDE锚完整性（finding→STRIDE）

-[] **所有相关威胁链接**在`3-findings.md`使用格式`[ThreatID](2-stride-analysis.md#component-anchor)`-[] **每个`#component-anchor`**都会在`2-stride-analysis.md`中解析为一个实际的`## Heading`-[] **锚结构验证**：标题→小写→空格→连字符→除连字符外的非字母数字
-[] **通过跟踪链接并确认该标题下存在威胁ID，抽查至少3个锚**

3.5计数一致性（评估↔所有文件）-[] **执行摘要中的元素计数**与`1-threatmodel.md`中元素表的实际行数匹配
-[] **执行摘要中的查找计数**与`3-findings.md`中的实际查找计数匹配
-[] **执行摘要中的威胁计数**与`2-stride-analysis.md`汇总表中的总数匹配
-[] **层计数**在威胁计数上下文段落匹配实际T1/T2/T3总数从`2-stride-analysis.md`- [] **Action Summary tier table** count匹配`3-findings.md`（finding列）和`2-stride-analysis.md`（threats列）的实际每层计数

**计数校验方法：**
-元素计数：计算`1-threatmodel.md`元素表中的`|`行数，减去2（标题+分隔符）
-查找计数：计数`### FIND-`标题在`3-findings.md`—威胁计数：读取“`Total`汇总表”中的“总数”行，取“`Total`”列值
-层计数：从相同的总计行，取T1， T2， T3列值3.6 STRIDE汇总表算法-[] **每行**:S + T + R + I + D + E + A =每个组件的总数
-[] **逐行**:T1 + T2 + T3 =每个组件的总数
-[] **合计行**：所有组件行的每列和等于合计行值
-[] **行数交叉检查**：每个组件的详细表中的威胁行数等于汇总表中的威胁行总数
-[] **没有人为的全1模式**：检查总结表中每个STRIDE列（S，T，R，I，D，E，A）对于每个组件都是1的模式。如果所有组件在每个STRIDE类别中都有一个威胁→FAIL（表示公式化的“每个类别至少有一个”膨胀，而不是真正的分析）。有效的分析应该具有反映实际攻击面的每个类别的不同计数：有些类别可能为0（使用N/A证明），其他类别可能为2-3。所有部件上均匀的15是人工填充的强烈信号．
- [] **N/A表项从总数中排除**：如果任何组件有`N/A — {justification}`表项用于STRIDE类别，请确认这些类别在汇总表中显示为0（而不是1）。N/A表项不算作威胁。### 3.7 Sort Order （finding）

-[] **在每一层中**：调查结果按严重→重要→中等→低的顺序出现
-[] **在每个严重级别内**：高cvss结果出现在低cvss结果之前
-[] **无乱序**：按顺序扫描，确认无反转

### 3.8报表文件表（评估↔输出文件夹）

-[] **在`0-assessment.md`的“Report Files”表中**列出的所有文件都存在于输出文件夹中
—“报表文件”表中的“**`0.1-architecture.md`”为**
-[] **如果没有生成`1.2-threatmodel-summary.mmd`**：从报表文件表中省略它（没有列出“N/A”注释）

---

第4阶段-证据质量检查

这些检查验证发现的实质，而不仅仅是结构。理想情况下，由具有代码访问权限的子代理运行。

### 4.1寻找证据-[] **每个发现**都有一个证据部分引用特定的files/lines/configs-[] **证据是具体的**：显示实际的代码或配置，而不仅仅是“没有配置”
-[] **对于“缺失的安全”声明**：证据证明平台默认是不安全的（不仅仅是显式配置缺失）

### 4.2标记前验证合规性

-[] **在STRIDE分析之前执行安全基础设施清单**（检查发现中的平台安全默认值验证）
-[] **无假阳性模式**：当Dapr哨兵存在时，没有发现“缺失mTLS”，或K8s≥1.6时“缺失RBAC”等。
-[] **应用发现分类**：每个记录的发现都是“确认”的（不是“需要验证”-那些属于`0-assessment.md`）

需求验证放置-[] **所有“需求验证”项目**在`0-assessment.md`的分析上下文和假设下-不在`3-findings.md`中
-[] **没有模棱两可的发现**:`3-findings.md`的发现有明确的漏洞证据

---

验证摘要模板

在运行所有检查之后，生成摘要。

子代理输出必须包括：
-阶段名称
-检查总数、通过、失败
-对于每个故障：检查ID，文件，证据，准确的修复说明
-修复后的重新运行状态

不要在没有计数的情况下返回“看起来不错”。```markdown
## Verification Results

| Phase | Checks | Passed | Failed | Notes |
|-------|--------|--------|--------|-------|
| 0 — Common Deviation Scan | [N] | [N] | [N] | [pattern matches] |
| 1 — Per-File Structural | [N] | [N] | [N] | [files with issues] |
| 2 — Diagram Rendering | [N] | [N] | [N] | [specific failures] |
| 3 — Cross-File Consistency | [N] | [N] | [N] | [gaps found] |
| 4 — Evidence Quality | [N] | [N] | [N] | [false positive risks] |
| 5 — JSON Schema | [N] | [N] | [N] | [schema issues] |

### Failed Checks Detail
<!-- For each failed check, list: check ID, file(s), what's wrong, suggested fix -->
```
---

阶段5 -threat-inventory.json模式验证

这些检查将验证步骤8b中生成的JSON库存文件。该文件对于比较模式至关重要。

5.1模式字段- [] **`schema_version`字段** -存在并等于`"1.0"`（独立）或`"1.1"`（增量）。如果报告包含`"incremental": true`， schema_version必须是`"1.1"`。否则`"1.0"`。
- [] **`commit`字段** -存在（简称SHA或`"Unknown"`）
—[]**`components`array**—非空，至少有一个表项
-[] **组件id ** -每个组件有`id`(PascalCase),`display`,`type`,`boundary`-[] **组件字段名遵从性** -组件使用`"display"`（不是`"display_name"`）。Grep:`"display_name"`必须返回0个匹配。
-[] **威胁字段名称遵从性** -威胁使用`"stride_category"`（不是`"category"`）。威胁有`"title"`和`"description"`（不仅仅是`description`，也不是`"name"`）。威胁→组件链接在`"identity_key"."component_id"`内部（不是威胁对象上的顶级`"component_id"`）。Grep：顶级`"category":`在identity_key之外必须返回0匹配。Grep：每个威胁对象必须包含`"title":`。
- [] **`boundaries`array** -存在（对于平面系统可以为空）
- [] **`flows`数组** -目前，每个流都有规范的ID格式`DF_{Source}_to_{Target}`- [] **`threats`array** -非空
- [] **`findings`array** -非空
- [] **`metrics`对象** -目前`total_components`，`total_threats`,`total_findings`5.2度量一致性

- [] **`metrics.total_components == components.length`** -数组长度匹配次数
- [] **`metrics.total_threats == threats.length`** -数组长度匹配次数
- [] **`metrics.total_findings == findings.length`** -数组长度匹配次数
-[] **指标匹配markdown报告** -`total_threats`等于STRIDE汇总表中的Total，`total_findings`等于`3-findings.md`中的`### FIND-`count
-[] **截断恢复门** -如果上面检测到任何数组长度不匹配，请验证文件是否重新生成（未打补丁）。检查：文件大小>0 kb与bbb40威胁的repos；威胁数组包含出现在`2-stride-analysis.md`中的每个组件的条目
-[] **预写策略遵从** -如果`metrics.total_threats > 50`，验证JSON是通过子代理委托，Python脚本或块追加编写的-而不是单个`create_file`调用。证据：检查`agent`调用或`_extract.py`脚本或对JSON文件进行多次`replace_string_in_file`操作的日志。确定性同一性稳定性（用于比较准备）-[] **组件包含确定性标识字段** -每个组件都有`aliases`(array)，`boundary_kind`和`fingerprint`- [] **`boundary_kind`有效值** -每个组件的`boundary_kind`是：`MachineBoundary`，`NetworkBoundary`,`ClusterBoundary`,`ProcessBoundary`,`PrivilegeBoundary`，`SandboxBoundary`之一。❌其他值（如`DataStorage`、`ApplicationCore`、`deployment`、`trust`）→FAIL
-[] **边界包括确定性身份域** -每个边界都有`kind`，`aliases`（array）和`contains_fingerprint`-[] **边界`kind`有效值** -每个边界的`kind`是`boundary_kind`相同的6个tmt对齐值之一。❌其他值→FAIL
-[] **没有重复的规范组件id ** -`components[].id`归一化后的值是唯一的
-[] **别名映射是一致的** -在同一库存中，两个不相关的组件id下不会出现别名
-[] **指纹证据字段仅稳定** -`fingerprint`使用源files/topology/type/protocols，而不是自由格式散文
-应用确定性排序** -按标准键排序的数组（`components.id`,`boundaries.id`,`flows.id`,`threats.id`,`findings.id`）比较漂移护栏（验证比较输出时）

-[] **高置信度的重命名候选对象不保留为add/remove** -具有强alias/source-file/topology重叠的组件对被分类为`renamed`/`modified`-[] **边界重命名候选使用包含重叠** -相同的`kind`+高`contains`重叠被分类为边界`renamed`，而不是`added`+`removed`- [] **Split/merge边界转换识别** -一对多和多对一的包含转换映射到`split`/`merged`类别

比较完整性检查（验证比较输出时）-[] **基线≠当前提交** -`metadata.json`→`baseline.commit`不能和`current.commit`相同。同一提交比较无效（没有实际代码更改来进行比较）。
-[] **文件更改> ** -`metadata.json`→`git_diff_stats.files_changed`必须为> 0。与0个文件的比较没有代码增量，也没有意义。
-[] **持续时间> 0** -`metadata.json`→`duration`不能为`"0m 0s"`或小于2分钟的任何值。真正的比较需要读取两个清单、执行多信号匹配、计算热图和生成HTML——这需要实时的时间。
-[] **没有外部文件夹引用** -`metadata.json`和所有输出文件不能包含对`D:\One\tm`或任何被分析的存储库外部文件夹的引用。报告应该只引用当前repo中的文件夹。
-[] **反重用验证** -比较输出必须是新生成的，不能从先前的`threat-model-compare-*`文件夹复制。版本检查`metadata.json`时间戳是否来自当前运行。
-[] **方法学漂移率** -如果`diff-result.json`→`metrics.methodology_drift_ratio`> 0.50，检查HTML报告是否包含方法学漂移警告横幅。如果没有计算比率，但是50%的组件重命名共享相同的aliases/fingerprints，则标记为验证失败。---

阶段6 -确定性身份和命名稳定性

这些检查验证component/boundary/flow命名遵循确定性规则，从而在相同代码的独立运行之间实现可重复的输出。

6.1组件ID确定性-[] **从代码工件派生的组件ID ** -`threat-inventory.json`中的每个组件ID必须追溯到实际的类名，文件路径，部署清单`metadata.name`或配置键。没有抽象概念（`ConfigurationStore`,`DataLayer`,`LocalFileSystem`）。根据源文件名和类名查找组件id—至少80%应该有直接匹配。
-[] **组件锚验证** -`threat-inventory.json`中的每个进程型组件必须具有非空的`fingerprint.source_files`或`fingerprint.source_directories`。如果两者都为空→FAIL（组件没有代码锚）。
- [] **Helm/K8s工作负载命名**—对于k8s部署的组件，检查组件ID是否匹配Deployment/StatefulSetYAML中的`metadata.name`，而不是Helm模板文件名或目录。例如：`DevPortal`（来自部署名称），而不是`templates-knowledge-deployment`（来自文件路径）。
-[]外部服务锚定-外部服务（没有源代码）必须锚定到它们的集成点nt：客户端类名、配置密钥或SDK依赖项。验证是否填充了`fingerprint.config_keys`或`fingerprint.class_names`。
-[] **禁止命名模式不存在** -组件ID不是通用标签：grep for`ConfigurationStore`，`DataLayer`,`LocalFileSystem`,`SecurityModule`,`NetworkLayer`,`DatabaseAccess`。→必须返回0个匹配项。
—[]**缩写一致性**—passalcase id中知名缩写必须为全大写：`API`、`NFS`、`LLM`、`SQL`、`DB`、`AD`、`UI`。Grep为`Api`（应为`API`），`Nfs`（应为`NFS`），`Llm`（应为`LLM`）。→必须返回0个匹配项。
-[] **通用技术命名准确性** -验证这些准确的id:`Redis`（不是`RedisCache`），`Milvus`（不是`MilvusDB`），`NginxIngress`（不是`IngressNginx`），`AzureAD`（不是`AzureAd`），`PostgreSQL`（不是`Postgres`）。6.2边界命名稳定性

—[]**边界ID为PascalCase**—`threat-inventory.json`中的每个边界ID使用部署拓扑（例如`K8sCluster`，`External`,`Application`）派生的PascalCase。非代码架构层（`PresentationLayer`,`BusinessLogic`）。
-[] **没有单进程应用程序的代码层边界** -如果系统是一个单进程（一个。exe，一个容器），应该有1个`Application`边界-而不是Presentation/Business/Data层的4个+边界。计算边界并验证比例。
- [] **K8s多业务子边界**—对于有多个部署的K8s命名空间，验证存在子边界：`BackendServices`、`DataStorage`、`MLModels`、`Agentic`（根据实际情况）。

6.3数据流完整性- [] **ingress/reverse代理的双向流** -如果入口组件（Nginx, Traefik）路由到后端，验证两个方向都存在：`DF_Ingress_to_Backend`和`DF_Backend_to_Ingress`。计数通过入口的前向流，并验证匹配的响应流。
-[] **数据库双向流** -对于每个`DF_Service_to_Datastore`流，检查是否存在一个对应的`DF_Datastore_to_Service`读流。数据存储：Redis， Milvus, PostgreSQL， MongoDB等。
-[] **流量计数稳定性** -计数流量在`threat-inventory.json`。对相同代码进行两次独立运行应该产生相同的计数（±3是可以接受的）。如果对于未更改的组件，旧的和HEAD分析之间的流量计数相差bbb50，则标记为命名漂移。

6.4计数稳定性（交叉运行确定性）-[] **元器件数量在公差范围内** -如果比较同一代码的两次分析，元器件数量必须在±1范围内。差值≥3 = FAIL。
-[] **边界计数在公差范围内** -相同代码→边界计数在±1范围内。
-[] **进程组件指纹完整性** -每个带有`type: "process"`的组件必须有非空的`fingerprint.source_directories`和`fingerprint.class_names`。工艺组件空数组→FAIL。
- [] **STRIDE类别单字母强制** - JSON中的每个`threats[].stride_category`都是一个字母：S， T， R， I， D， E或a。全称（`"Spoofing"`,`"Tampering"`,`"Denial of Service"`）的Grep→必须返回0匹配。这可以防止热图计算错误。

---

第7阶段-基于证据的先决条件和覆盖完整性

这些检查确认先决条件、层次和覆盖遵循确定性的基于证据的规则。

7.1先决条件确定证据-[] **无部署证据无前提条件** -对于每一个发现`Exploitation Prerequisites`≠`None`，验证前提条件是否反映实际部署配置（Helm值，Dockerfile，服务类型，入口规则）。如果先决条件是`Internal Network`，但没有证据表明存在网络限制→FAIL。
-[] **相同代码的前提一致性** -如果对相同代码的两次分析对相同漏洞产生不同的前提，则技能规则不足。调查标志。

### 7.1b部署分类门（强制）-[] **目前部署分类** -`0.1-architecture.md`必须包含一个`**Deployment Classification:**`行，其中包含：`LOCALHOST_DESKTOP`，`LOCALHOST_SERVICE`,`AIRGAPPED`,`K8S_SERVICE`,`NETWORK_SERVICE`。❌失踪→失败。
-[] **组件暴露表当前** -`0.1-architecture.md`必须包含一个`### Component Exposure Table`，列：组件，监听，Auth Required，可达性，最小先决条件，派生层。❌失踪→失败。
-[] **公开表的完整性** - Key Components表中的每个组件在组件公开表中都有对应的一行。❌缺行→失败。
- [] ** T1强制部署分类**—如果部署分类为`LOCALHOST_DESKTOP`或`LOCALHOST_SERVICE`：
-计数发现`Exploitation Prerequisites`=`None`。❌Count > 0→FAIL（必须为`Local Process Access`或`Host/OS Access`最小值）。
-计数发现在`## Tier 1`。❌计数> 0→失败（必须是T2+localhost/desktop应用程序）。
-对于CVSS中`AV:N`的每个发现，检查组件的`Reachability`列。❌`AV:N`with`Reachability ≠ External`→FAIL。
-[] **强制执行的先决条件** -对于每个发现，在暴露表中查找发现的`Component`。结果的`Exploitation Prerequisites`必须≥表中的`Min Prerequisite`。发现的层级必须≥`Derived Tier`。❌发现有`None`，但表显示`Local Process Access`→FAIL。
-[] **证据的先决条件基础** -每个发现的`#### Evidence`部分必须包含一个`**Prerequisite basis:**`行，引用确定先决条件的特定code/config。❌缺失或泛型（“在代码库中找到”）→失败。7.2覆盖完整性-[] **技术覆盖检查** -对于repo中的每个主要技术（Redis, PostgreSQL, Docker, K8s,ML/LLM， NFS等），验证至少一个发现或文档缓解解决它。扫描`0.1-architecture.md`技术堆栈表→对于每种技术，grep`3-findings.md`查找匹配结果。
-[] **最小发现阈值** -小型repo（<20个文件）：≥8个发现；中（20-100）：≥12；大型（100+）：≥18。计数`### FIND-`标题并根据回购大小进行验证。
-[] **在上下文感知限制内的平台比例** -检测部署模式：如果go。mod包含`controller-runtime`/`kubebuilder`/`operator-sdk`→K8s算子（极限≤35%）；否则→单机App（限制≤20%）。计数平台状态威胁/总威胁。超过限制→失败。在评估中记录检测到的模式。
- [] **DoS没有先决条件=发现** -每个DoS威胁（`.D`）与`Prerequisites: None`必须有一个对应的ng发现。在没有先决条件的情况下，对`.D`威胁进行跨步分析，并验证每个映射到Coverage表中的查找ID。7.3安全基础设施意识

-[] **提到的安全基础设施清单** -验证`0.1-architecture.md`或`2-stride-analysis.md`引用的安全组件（服务网格、证书管理、认证中间件）是否存在于代码库中。如果部署了Dapr哨兵，mTLS不能被标记为“缺失”。
-[] **缺失安全声明的举证责任** -每个发现声明“缺失X”必须证明平台默认值是不安全的，而不仅仅是显式配置缺失。抽查最严重的“失踪”发现。

---

##阶段8 -比较HTML报告结构（仅比较输出）

这些检查验证HTML比较报告结构。

8.1 HTML比较报表结构-[] **确切4`<h2>`节** - HTML必须有确切这些`<h2>`标题顺序：“执行摘要”，“威胁层分布”，“STRIDE-A热图（与Delta指标）”，“比较基础-组件映射”。❌额外的部分，如“整体风险转移”、“关键增量指标”、“指标概述”、“发现差异”，如`<h2>`→FAIL（这些要么是内联元素，要么被删除）。❌错过任何一个4→FAIL。
-[] **没有发现Diff部分** - HTML不能包含“发现Diff”`<h2>`部分或任何发现Diff子部分（固定，删除，分析差距，新，改变，不变）。如果存在→失败。
-[] **没有delta metric卡** - HTML不能包含`.risk-delta`卡（发现固定，新发现，净变化，删除，分析差距，代码验证）。如果存在→失败。
-[] **风险转移和指标栏作为内联元素** -风险转移和指标csbar （Components/Threats/Boundaries/Flows/Time）是内联卡元素，而不是`<h2>`节。如果它们显示为`<h2>`→FAIL。
-[] **指标栏包括信任边界** -指标栏必须显示信任边界计数（例如，`2 → 2`）。如果指标栏中缺少边界，则→FAIL。组件、威胁、信任边界、发现和代码更改是5个必需的度量框。
-[] **指标栏第5个框是Code Changes** -第5个指标框必须显示提交计数和PR计数（例如，`142 commits, 23 PRs`）。❌“Time Between”→FAIL。duration/dates现在在比较卡中（第1节），而不是在指标栏中。
-[] **比较卡结构** -第1节必须包含一个`comparison-cards`div与3个子卡：基线（哈希，日期，评级），目标（哈希，日期，评级），趋势（方向，持续时间）。❌旧式`subtitle`div与`Baseline: SHA → Target: SHA`→FAIL。❌单独`risk-shift`div→FAIL（合并成对比卡）．
-[] **状态信息（Fixed/New/Previously未知计数）必须只出现在一个地方：彩色状态汇总卡。它们也不能在指标栏中显示为小的内联徽章或文本。如果在指标栏和彩色卡片中出现相同的计数→失败（从指标栏中删除，保留彩色卡片）。
-[] **分级标签匹配分析报告** - HTML中的威胁分级分布部分必须使用以下标签：“一级-直接暴露”，“二级-条件风险”，“三级-纵深防御”。❌“可能暴露”，“理论”，“高风险”，或任何虚构的变体→失败。
-[] **章节标题为“比较基础”，而不是“架构变更”** -组件映射章节标题必须为“比较基础-组件映射”，而不是“架构变更”。
-[] **热图有13列** - STRIDE-A热图网格必须有：组件| S | T | R | I | D | E | A |总|分压器| T1 | T2 | T3。如果缺少T1/T2/T3列→FAIL。热图标题必须包括“（与增量指标）”。8.2热图精度（比较输出）-[] **热图不全为零** -`baseline.Total`和`current.Total`在`stride_heatmap.components`求和。如果任何一个和为0，但相应的库存有威胁→失败（热图计算错误）。
-[] **没有重复的重命名组件行** -对于`components_diff.renamed`中的每个条目，验证热图中只有一行重命名组件（使用当前名称），而不是两行（一个全零基线，一个全零电流）。
-[] **热图异常检测执行** -对于每个热图行`baseline.Total > 0, current.Total == 0`（消失）和每一行`baseline.Total == 0, current.Total > 0`（出现）：验证指纹交叉检查执行。如果消失的对共享源文件、类名或名称空间→这是一个遗漏的重命名，必须重新分类。热图不应该与共享源文件匹配all-zero/all-new对。
-[] **比较置信度评分存在** -`diff-result.json`必须包含`comparison_confidence`字段（“高”或“低”））. 如果存在超过3个未解决的热图异常→置信度必须为“低”，并在HTML中显示警告横幅。
-[] **每个组件跨步算法** -对于每个热图行：`S+T+R+I+D+E+A == Total`和`T1+T2+T3 == Total`基线和电流。任何不匹配→FAIL。
- [] **Delta箭头匹配JSON数据** -对于每个热图单元格，`delta = current - baseline`。如果== 0，没有箭头。如果δ > 0，▲。如果< 0，▼。抽查至少3个组件。
-[] **组件移除源文件验证** -对于`components_diff.removed`中的每个组件，验证其`source_files`在当前提交中确实不存在。如果源文件仍然存在→重新分类为重命名或方法差距。