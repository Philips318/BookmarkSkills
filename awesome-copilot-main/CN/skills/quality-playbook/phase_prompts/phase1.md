你是一个质量工程师。对于这个阶段，只读到阶段1之前的部分（在“阶段2”之前的“——”行停止）。还要读取与探索相关的参考文件（在与您解析的安装路径匹配的任何参考/目录下）。

{seed_instruction}

执行阶段1：探索代码库。reference_docs/目录包含收集到的文档—阅读它以补充您的探索。顶级文件是Tier 4上下文（AI聊天、设计笔记、回顾）。reference_docs/cite/下的文件是可引用的源代码（项目规范、rfc）。如果reference_docs/缺失或为空，则单独处理第3层证据（源代码树），并在EXPLORATION.md中记录这一点。

强制文件-角色标记（v1.5.4第1部分）在编写EXPLORATION.md之前（或作为编写EXPLORATION.md的一部分），先生成`quality/exploration_role_map.json`。首先在存储库根读取`SKILL.md`（如果存在的话，还要检查任何其他顶级技能型条目文件—指示符是内容+名称，而不是扩展名；仅仅因为`README.md`位于根，它就不是技能型条目）。散文上下文通知每个后续文件的角色标记。**文件源（v1.5.4阶段3.6.1，代码预防）。**当目标是git repo时，使用`git ls-files`作为规范文件列表-这会自动尊重`.gitignore`，并且是唯一支持的枚举源。不要使用`os.walk`、`find`、`os.listdir`，或者任何递归目录遍历器——这些将拉入`.git/`、`.venv/`、`node_modules/`、构建输出和供应商依赖项，所有这些在角色映射中都是被禁止的（验证器会拒绝它们并中止运行）。当目标不是git repo时，使用显式跳过下面列出的不允许路径的文件系统遍历；将此回退记录在角色映射的`provenance`字段中。**不允许的路径（绝对不能出现在任何角色下的角色映射中）：**`.git/`,`.venv/`,`venv/`,`node_modules/`,`__pycache__/`,`.pytest_cache/`,`.mypy_cache/`,`.ruff_cache/`,`.tox/`，加上任何以`.egg-info`或`.dist-info`结尾的组件路径。`bin/role_map.py::DISALLOWED_PATH_PREFIXES`的验证器强制执行此操作—如果您的角色映射包含任何这样的路径，则运行中止。此外，报名人数上限为2000人；有更多的角色图被视为第一阶段走过的证据。gitignored内容。

**来源（v1.5.4阶段3.6.1）。**角色映射的顶级字段`provenance`必须是以下字段之一：
-`"git-ls-files"`-首选。目标是一个git回购；您运行`git ls-files`来枚举。
-`"filesystem-walk-with-skips"`-回退。Target不是git回购；您对上面的禁止路径列表中的每个条目都使用显式跳过来遍历文件系统。
-`"unknown"`-只接受遗留角色映射不要在新跑的时候释放这个。对于每个作用域内文件，发出一个具有以下角色分类法的记录。判断是基于内容的：读取文件（或足够多的文件来判断），不要仅对扩展名或目录名进行模式匹配。

**哨兵文件（v1.5.4阶段3.6.1）。**存储库跟踪树中名为`.gitkeep`（或类似的空目录标记）的文件绝对不能删除。它们在git历史记录中保留其他空目录。如果您找到了这样一个文件，但不了解它的用途，那就不要管它。飞行前检查验证所有`.gitignore !`-rule哨兵是否存在，如果缺少任何哨兵，则中止运行。**如果你在运行过程中遇到QPB本身的错误**（例如，`bin/run_playbook.py`的异常，丢失的导入，QPB源中的错误断言），立即停止运行并报告：
1. 准确的错误及其发生的位置（文件：line + traceback）
2. 诊断出可能的根本原因
3. 建议的固定形状（不要使用它）

不要自己修补QPB源代码。QPB源更改经过理事会审查（见`~/Documents/AI-Driven Development/CLAUDE.md`）。结构backstop在运行开始时捕获QPB源树的git SHA，并在每个阶段边界验证它不变；自治源补丁将通过命名修改文件的诊断使门失败。

角色分类（单一事实来源：`bin/role_map.py::ROLE_DESCRIPTIONS`）：
{role_taxonomy}

如果一个文件确实不适合上述任何一个，您可以添加一个新角色——但是要在角色映射的第一个条目中记录添加的内容，作为注释风格的基本原理。输出文件`quality/exploration_role_map.json`必须符合这个模式：```
{{
  "schema_version": "1.0",
  "timestamp_start": "<ISO 8601 UTC timestamp at the start of Phase 1>",
  "provenance": "git-ls-files",
  "files": [
    {{
      "path": "<repo-relative POSIX path>",
      "role": "<one of the role taxonomy values>",
      "size_bytes": <int>,
      "rationale": "<one or two sentences justifying the tag, content-based>"
    }}
    // ... one entry per in-scope file. When role == "skill-tool", also
    // include a "skill_prose_reference" string pointing at the SKILL.md /
    // reference-file location that names this script (e.g., "SKILL.md:47"
    // or "references/forms.md:section-3"); the prose-to-code divergence
    // check in Phase 4 reads this back to find the cited prose.
  ]
}}
```
**你只生产`files[]`和`provenance`。**两个可机械衍生的字段-`breakdown`和`summary`-由阶段1 LLM出口和阶段2入口之间的运行器计算（v1.5.6集群047架构修复）。运行程序调用`bin.role_map.compute_breakdown(files)`和`bin.role_map.summarize_role_map(...)`，并在验证之前将规范值写入磁盘上的文件。不要在输出中包含`breakdown`或`summary`—即使这样做，运行程序也会覆盖它们。您的工作是分析工作（按文件标记`files[]`+`provenance`）；确定性聚合是运行者所有的。（在v1.5.6之前，LLM也被指示计算这些，这产生了一类故障，LLM从严格的机械契约中恢复到直观的总结；运行端计算消除了故障模式。）标签纪律:
1.`skill-tool`和`code`是承重的区别。脚本只有在SKILL.md（或SKILL.md引用的文档）显式命名并告诉代理调用它时才是`skill-tool`。独立的代码模块—即使是`scripts/`目录中的小模块—如果没有SKILL.md散文指示代理使用它们，则使用`code`。
2. 任何来自先前剧本运行的内容（目标的`quality/`子树，或从QPB本身安装的`quality_gate.py`-安装程序在SKILL.md旁边复制的文件，无论使用哪种ai工具安装布局）都是`playbook-output`，如果它是目标自己的表面，则永远不会有它的作用。这防止了v1.5.3版本的loc污染故障模式，即目标的表面代码被QPB自己的基础设施夸大了。
3. 如果在根目录中不存在SKILL.md，并且不存在其他技能形状的条目文件，则角色映射将没有`skill-prose`条目。这是很好-四遍派生管道将对该目标不起作用。处理边缘案例（v1.5.4阶段1边缘案例纪律）：
- **没有SKILL.md在根，没有其他技能形状的入口。**像往常一样按内容标记每个文件。角色映射将携带0个`skill-prose`和`skill-reference`条目；四通道管道将不起作用。不要发明一个合成的SKILL.md，或者为一个真正没有技能表面的项目贴上`skill-prose`的标签。
—**SKILL.md引用了不存在的脚本。**在角色映射中添加一个顶级数组`broken_references`，其中包含`{{"prose_location": "<file>:<line>", "missing_script": "<path-as-cited>"}}`条目。不要为缺少的脚本添加合成文件条目。注意EXPLORATION.md中的破碎引用，因此阶段4的散文到代码差异检查可以将其注册为已知的间隙。（这个字段是附加的；大门的角色映射验证器不需要它。）
- **目标与一个非常大的文件计数（1000+）。**批量处理。在遍历树时，`files`数组可以增量增长；一旦你做了所有的Ile判断，写一次文件。不要在遍历过程中编写部分角色映射—当文件出现时，验证器会认为文件已经完成，并且在退出阶段1后，运行端`normalize_role_map_for_gate`步骤（v1.5.6 cluster 047）会计算`breakdown`和`summary`。
- **模棱两可的散文（“帮助脚本”，“验证器”）。**默认为`code`。`skill-tool`需要一个明确的引用：SKILL.md或被引用的文档必须命名文件（或唯一标识它的路径后缀）并指示代理调用它。当有疑问时，标记`code`并捕获`rationale`中的歧义—最好标记下`skill-tool`，而不是扩大阶段4的“从文章到代码”检查操作的表面区域。
- **生成文件（构建输出，供应商依赖项，锁定文件）。**在忽略规则层跳过它们；不要将它们包含在角色映射中。如果无法判断文件是否已生成，请查找生成标记Ker（命名生成器的头注释，同级`.generated`文件，存在于`.gitignore`中）；如果生成，则从角色映射中省略。当第一阶段完成后，将您的全部勘探发现写到`quality/EXPLORATION.md`。该文件必须包含以下所有内容
章节标题逐字逐句（SKILL.md:1257-1273执行阶段1门）
每个机械;`bin/run_state_lib.validate_phase_artifacts(quality_dir, phase=1)`是程序化的执行者——您的工件必须先通过它吗
阶段2将开始)。确切的标题是承重的-不要
代替“同等”标题：

1.`## Open Exploration Findings`—至少8个编号条目
（`1.`,`2.`，…）每个条目至少有一个文件：行引用
在主体中（例如，`bin/foo.py:120-135`）。至少有三个
条目跟踪跨2个或多个不同文件行的行为
位置（多位置跟踪）-条目引用了两个或多个位置
不同的文件：行范围)。

2.`## Quality Risks`—领域知识风险分析。编号或
项目符号;引用文件：风险具体可见的行
代码或文档。3.`## Pattern Applicability Matrix`—包含一行的Markdown表
根据`references/exploration_patterns.md`的勘探模式。
决策列值为`FULL`或`SKIP`。在3到4之间
图案必须标记为`FULL`（包括-栅极拒绝）
低于3是因为探索没有选择足够的模式
因为探索运行了所有模式，而不是
选择)。跳过的模式仍然以`SKIP`和a列出
原因简单，所以矩阵是详尽的。4.`## Pattern Deep Dive — <pattern-name>`-至少3节，
每个`FULL`图案一个。每一次深潜都列举了具体的东西
发现与文件：行引用。至少有两个章节
跟踪代码路径跨越2个或更多不同的标识符(例如，
函数名或符号名如`\`docs_present\ ' '，`\`_evaluate_documentation_state\ ' ')跨两个或多个不同的OR
文件：行位置-这就是门检测“多功能”的方式
而不是一个锚点的发现。5.`## Candidate Bugs for Phase 2`-编号的错误列表
从深潜+开放探索中提出的假说。每一个
条目有一个`Stage:`行，用于标识源代码(例如，' Stage：
open exploration`, `Stage：质量风险，或`Stage: <Pattern Name>`)。至少2个条目必须来自`open exploration`/`quality risks`AND必须至少有一个条目
来源于一个模式深度潜水。组合阶段
(`Stage: open exploration + Cross-Implementation Consistency`)
向两个桶数。

6.`## Gate Self-Check`-证明你运行了第一阶段的门。每个列表
13张检查（≥120行+ 6个要求的标题+≥3个图案）
深潜切片+PROGRESS.md标记+≥8项发现及引用
+≥3个多位置发现+ 3-4个FULL pattern矩阵行+≥2
多功能深度潜水+候选错误源混合)和标记
工件是否满足每一个。此外，确保`quality/PROGRESS.md`存在，并确保它的阶段1
在声明阶段1之前，标记为`[x]`（闸的检查8）
完成了。此提示的早期版本要求的探索内容
(领域和堆栈识别，架构图，现有测试
库存，规格总结，skeleton/dispatch分析，
衍生需求`REQ-NNN`，衍生用例`UC-NN`，
文件-角色标记摘要)存在于这些必需的部分-
例如，体系结构映射和模块枚举属于
下`## Open Exploration Findings`为多位置发现；
文件-角色标记摘要和`exploration_role_map.json`细分汇总属于`## Open Exploration Findings`或`## Quality Risks`为分析内容；推导出REQ-NNN和UC-NN
章节可能在`## Gate Self-Check`之后作为附加部分出现
剧本下游阶段消耗的分析材料。不
使用这些可选名称作为顶级节标题-门
需要以上六个确切的标题和模式深潜
前缀;除了这些之外，还允许有其他`## `节
分析扩展但是六个门所要求的头衔必须出现
一字不差。强制笛卡尔UC规则（杠杆1，v1.5.2）

对于`References`字段命名≥2个文件（或≥2个文件：不同文件中的行范围）的每个要求，在决定是否发出单个伞形UC或每个站点的UC之前，应用**笛卡尔资格检查**。

**门1 -路径后缀匹配。**至少两个引用必须共享一个路径后缀角色：扩展名前的最后一个段，或者在文件中出现的匹配的function-name模式。
—匹配示例：`virtio_mmio.c`、`virtio_vdpa.c`、`virtio_pci_modern.c`都实现了`_finalize_features`。`_finalize_features`函数是共享角色。
-一个不匹配的例子：`CONFIG_FOO`，`CONFIG_BAR`标志在同一个kconfig文件-同样的事情，但不是并行实现。**门2 -函数级相似性。**每个匹配引用必须引用一个相似大小的行范围（在中位数的2倍之内），并且每个范围必须在函数体中-而不是文件头，kconfig块或宏展开列表。

* *决定:* *
**两个门都通过→**每个站点发出一个UC，编号为`UC-N.a`，`UC-N.b`,`UC-N.c`，…每个站点UC有自己的actor，前置条件，流程，后设条件。父REQ-N仍然作为保护伞。
- **只有1号门通过→**保持一个单一的伞形UC，并在UC主体的`<!-- cluster: heterogeneous -->`HTML注释中标记参考集群`heterogeneous`。阶段3仍然可以覆盖，如果它发现每个站点的分歧。
- **无门通→**单伞UC，无特殊标识。

工作示例- REQ-010 / VIRTIO_F_RING_RESET （virtio）

假设阶段1衍生出：    ### REQ-010: Virtio transports must honor VIRTIO_F_RING_RESET negotiation
    - References: drivers/virtio/virtio_mmio.c, drivers/virtio/virtio_vdpa.c, drivers/virtio/virtio_pci_modern.c
    - Pattern: whitelist
应用笛卡尔检验：
—门1：三个文件都包含`_finalize_features`函数—matches。
-门2：每个引用范围都在一个大小相似的函数体内-匹配。

两个门都通过→发出每个站点的UCs：    ### UC-10.a: VIRTIO_F_RING_RESET on PCI modern transport
    - Actors: virtio_pci_modern driver, guest kernel
    - Preconditions: device advertises VIRTIO_F_RING_RESET
    - Flow: vp_modern_finalize_features propagates bit through config space …
    - Postconditions: feature_bit reflected in final config

    ### UC-10.b: VIRTIO_F_RING_RESET on MMIO transport
    - Actors: virtio_mmio driver, guest kernel
    - Preconditions: device advertises VIRTIO_F_RING_RESET
    - Flow: vm_finalize_features must mirror PCI modern behavior …
    - Postconditions: feature_bit survives finalize call

    ### UC-10.c: VIRTIO_F_RING_RESET on vDPA transport
    - Actors: virtio_vdpa driver, vdpa device backend
    - Preconditions: device advertises VIRTIO_F_RING_RESET
    - Flow: virtio_vdpa_finalize_features forwards through set_driver_features …
    - Postconditions: feature_bit visible to vdpa backend
确认清单（笛卡尔UC规则）

在完成阶段1之前，在EXPLORATION.md中“笛卡尔UC规则确认”一节下明确确认每一项：

1. 对于每个具有≥2个引用的REQ，我运行Gate 1（路径-后缀匹配）。
2. 对于通过门1的每个REQ，我运行门2（功能级相似性）。
3. 在两个门通过的地方，我发射每个站点的UCs （UC-N）。UC-N。b,……)。
4. 在只有Gate 1通过的地方，我将集群标记为`<!-- cluster: heterogeneous -->`。
5. 两扇门都没有经过的地方，我只拿了一把伞，没有标记。
6. 对于Gate 1中具有模式匹配的每个REQ，我将`Pattern: whitelist|parity|compensation`添加到REQ块中。还要用下面的EXACT复选框格式的运行元数据和阶段跟踪器初始化quality/PROGRESS.md。这种格式是一种硬契约：阶段5门在允许对账开始之前检查子字符串`- [x] Phase 4`，并且它只匹配复选框形式。不要替换Markdown表格，项目符号格式，或任何其他布局-表格格式的运行在管道中中止，因为gate在表格单元格中没有看到“完成”。PROGRESS.md的相位跟踪器部分模板（从SKILL.md元数据中填写技能版本）：```
# Quality Playbook Progress

Skill version: <vX.Y.Z>
Date: <YYYY-MM-DD>

## Phase tracker

- [x] Phase 1 - Explore
- [ ] Phase 2 - Generate
- [ ] Phase 3 - Code Review
- [ ] Phase 4 - Spec Audit
- [ ] Phase 5 - Reconciliation
- [ ] Phase 6 - Verify
```
当每个后期阶段完成时，它将把自己的`- [ ]`翻转到`- [x]`-保持行文本（包括破折号后面的阶段名称）稳定，以便在阶段5门和下游工具中匹配子字符串。

重要提示：不要进入第二阶段。您唯一的工作就是探索并将发现写入磁盘。编写彻底、详细的发现—下一阶段将读取EXPLORATION.md以生成工件，因此必须在该文件中捕获所有重要的内容。