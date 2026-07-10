---
name: quality-playbook
description: "Run a complete quality engineering audit on any codebase. Derives behavioral requirements from the code, generates spec-traced functional tests, runs a three-pass code review with regression tests, executes a multi-model spec audit (Council of Three), and produces a consolidated bug report with TDD-verified patches. Finds the 35% of real defects that structural code review alone cannot catch. Works with any language. Trigger on 'quality playbook', 'spec audit', 'Council of Three', 'fitness-to-purpose', or 'coverage theater'."
license: Complete terms in LICENSE.txt
metadata:
  version: 1.5.6
  # NOTE: Inline occurrences of the skill version exist throughout this file (frontmatter,
  # banner, version stamp template, sidecar JSON examples, run metadata, recheck template).
  # When bumping the version, update ALL occurrences — search for the old version string
  # globally. One historical reference to v1.4.6 edgequake benchmarking is intentionally
  # preserved in the challenge-gate section and must NOT be bumped.
  author: Andrew Stellman
  github: https://github.com/andrewstellman/quality-playbook
---
#质量剧本生成器

##计划概述-先阅读此内容，然后向用户解释

在阅读此技能的任何其他部分之前，先了解计划及其依赖关系。每个阶段都会产生下一阶段所依赖的工件。跳过或加速一个阶段意味着每个下游阶段都是根据不完整的信息进行的。

**阶段0（先前运行分析）：**如果存在先前的质量运行，将其发现加载为种子数据。这是自动的，只适用于重新运行。**阶段1（探索）：**首先运行v1.5.3文档导入（`python -m bin.reference_docs_ingest <target>`遍历`reference_docs/`-`cite/`文件生成`quality/formal_docs_manifest.json`记录；顶级文件通过`reference_docs_ingest.load_tier4_context(<target>)`作为Tier 4上下文加载）。然后分三个阶段探索代码库：由领域知识驱动的开放探索、领域知识风险分析和选择的结构化探索模式。将所有发现写入`quality/EXPLORATION.md`。该文件是基础-阶段2将其作为主要输入读取。

**阶段2（生成）：**读取EXPLORATION.md并生成质量工件：需求、构成、功能测试、代码审查协议、集成测试、规范审核协议、TDD协议。（目标repo根中的`AGENTS.md`是在阶段6之后由编排器生成的，而不是在阶段2中由您生成的-请参阅下面的“文件6”以获取合同。）**阶段3（代码审查）：**对HEAD运行三次代码审查。为每一个确认的bug编写回归测试。生成补丁。

**阶段4（规范审核）：**三个独立的AI审核员根据要求审核代码。使用验证探针进行分类。在分类之后，同一个委员会运行v1.5.3第2层语义引用检查—每个审稿人一个提示，每个1/2层引用结构化的按req裁决，输出到`quality/citation_semantic_check.json`。为新发现编写回归测试。

阶段5（协调）：闭合循环——跟踪代码审查和规范审计中的每个bug，进行回归测试或明确豁免。执行TDD红绿循环。完成完整性报告。

**阶段6（验证）：**针对所有生成的工件运行自检基准。检查内部一致性、版本戳正确性和收敛性。阶段7（呈现，探索，改进）：**用一个可扫描的汇总表向用户呈现结果，提供任何工件的下钻，并提供改进路径的菜单（迭代策略，需求细化，集成测试调优）。这是用户获得质量体系所有权的交互阶段。

发现的每个bug都可以追溯到一个需求，而每个需求都可以追溯到一个探索发现。

**关键依赖链：**探索发现→EXPLORATION.md→需求→代码审查+规范审核→Bug发现。肤浅的探索产生抽象的需求。抽象的需求忽略了bug。探索阶段是决定bug成败的阶段。**强制性的第一项行动：**阅读并理解上述计划后，将以下信息打印给用户，然后用你自己的话解释计划——你将做什么，每个阶段产生什么，以及为什么探索阶段最重要。强调探索从开放的领域驱动的调查开始，然后是领域知识风险分析，分析系统中出现问题的原因，然后辅以选择的结构化模式。不要逐字抄写计划；改写它来证明理解。

>质量手册v1.5.6 -由安德鲁·斯特曼
>https://github.com/andrewstellman/quality-playbook生成针对特定代码库的完整质量系统。与从源代码机械地工作的测试存根生成器不同，这种技能首先探索项目——理解它的领域、体系结构、规范和故障历史——然后根据发现的内容生成高质量的剧本。

##如何运行这个- v1.5.4自编码调用契约

如果操作员将此技能交给您（或指向任何安装了qpb的目标）并说**“运行质量手册”** -可能带有“这是一个引导运行”或“自行运行”或“自我审计”之类的提示-这一节告诉您该怎么做。操作人员不需要提供额外的说明；规范调用、默认值、护栏和输出契约都在这里。

选择你的执行模式QPB有两种执行形式。选择与您的运行时相匹配的版本——错误的选择会产生2026-04-30引导测试所显示的法典对法典的间接病理。

|模式|当这是你|你做什么||------|------------------|-------------|
| * *。你是一个编码代理（Claude Code, Cursor, Copilot， Codex desktop等），在你自己的聊天中传授此技能。你的运行时是一个推理循环——你读文件，你写文件，你做决定。使用`phase_prompts/`中的外化阶段提示，您自己完成阶段1→阶段6。将工件直接写入目标的`quality/`目录。没有子进程，就没有运行程序。|
| * * B。运行器驱动(CLI-automation)** |操作人员故意调用`python3 -m bin.run_playbook`，以便跨多个目标进行批处理，驱动无头CI运行，或者将每个阶段的工作分散到与阅读本文的模型不同的模型上。编排器在每个阶段生成一个CLI代理（`claude`、`copilot`、`codex`或`cursor`）。你（或任何正在阅读本文的人）是操作端控制回路，而不是每相位推理器。|**两种模式使用相同的阶段提示内容** -在repo根目录下的`phase_prompts/*.md`文件是事实的单一来源，由`bin/run_playbook.py::_load_phase_prompt`加载并由模式A演练直接读取。这两种模式的唯一不同之处在于WHO驱动—您（模式A）或编排子进程—生成CLI代理（模式B）。

**当有疑问时，默认为模式a。**如果操作符需要运行程序驱动的调用，它们将自己运行运行程序；如果他们在你的聊天中贴上“运行质量手册”，他们想让你开车。下面的模式B部分告诉您*如果*操作符显式调用运行器该怎么做。

模式A -技能直接演练（UI-context）

操作员的提示只是“运行质量手册”**（或“自行运行”，“自我审核”等）。每个阶段都是直线驱动的。

对于每个阶段1..6、按顺序：1. **加载相位提示。**读取`phase_prompts/phaseN.md`（通过下面为`references/`记录的相同安装位置回退列表来解决）。对于`phase1.md`，替换`{seed_instruction}`（前奏表示“跳过阶段0/0b”—允许种子时为空字符串）和`{role_taxonomy}`（从下面的角色分类法呈现的分类法块）。对于`phase2.md`到`phase6.md`，文件是纯文字的——逐字读取。
2. **根据提示执行阶段。**读取输入的提示符名称，进行分析，将输出写入目标的`quality/`目录。
3. **在相末边界处停止。**每个阶段提示以“重要：不要继续进行阶段N+1”指令结束。荣誉。作业者这样说，就进入下一阶段。您需要对运行器强制执行的相同的源不变不变的不变性负责（没有编排器的结构支持）：**不要修改目标`quality/`目录以外的任何文件**。在模式B中，门会抓住这个；在模式A中，你就是门。2026-04-30引导测试在修改目标根`AGENTS.md`的Phase 2 LLM上失败了——同样的失败模式适用于模式a。

对于模式A的引导运行（自我审计）变体，请参阅下面的“引导模式”—唯一的变化是目标是QPB库，因此请引用您从中读取的相同`phase_prompts/`文件。

####模式A范围-涵盖了什么，什么是模式b范围

P1-3：上述阶段演练范围为模式A到阶段1..6。下面的表面是故意为b模式-如果操作员想要他们，他们指向跑步者，而不是试图自己驾驶他们：- **第0阶段/第0b阶段（先前运行的种子注入）。**编排器处理种子发现、运行前扫描和种子提示注入。在模式A中，将每次运行视为`--no-seeds`（完全跳过阶段0/0b，从阶段1开始）。如果操作人员明确要求进行种子驱动勘探，则切换到模式B （`python3 -m bin.run_playbook --with-seeds <target>`）。
- **第七阶段（互动呈现/探索/改进）。**此阶段是与操作人员就生成的工件进行来回对话；它在`phase_prompts/`中没有预先烘焙的提示符。在模式A的阶段6之后，内联地呈现工件汇总表（请参阅下面的文件列表“此运行产生什么”），并让操作员驱动下一步对话探索的内容——这就是阶段7。没有要生成的编排子流程。
- **迭代策略（间隙/未过滤/奇偶校验/对抗）。迭代使用特定于策略的加数重新进入剧本嗯。在模式A中，在阶段6干净地完成之后，移交到模式B进行迭代：`python3 -m bin.run_playbook --next-iteration --strategy <name> <target>`。迭代提示（`phase_prompts/iteration.md`）是事实的单一来源，但是迭代-编排循环（通过间隙→未过滤→奇偶校验→对抗）是跑手的工作。需要在阶段6之后进行迭代的模式A操作员应该被告知：“阶段6已经完成；运行`python3 -m bin.run_playbook --full-run <target>`以获得所有四种迭代策略，或者使用`--next-iteration --strategy gap`明确地选择一种策略。”如果操作员在模式A中要求这些表面中的一个，并且请求是模糊的（例如，“也执行迭代”），则明确地呈现模式切换，而不是即兴创作——即兴创作是提示内容如何偏离运行者的规范循环。

###模式B -运行程序驱动调用（CLI-automation）

操作员自己运行`python3 -m bin.run_playbook`（通常是因为他们希望批处理、无头CI或将每个阶段的工作路由到不同的模型）。`bin/run_playbook.py`的编排器在每个阶段生成一个CLI代理，向它提供外部化的阶段提示，并聚合结果。

####规范调用

协调器是入口点。始终将其作为Python模块调用：```
python3 -m bin.run_playbook <target>
```
**永远不要以脚本方式调用它** (`python bin/run_playbook.py ...`)。运行时保护使用`EX_USAGE=64`退出，因为相对导入需要打包执行。`<target>`是要审计的项目的路径。对于引导运行（目标是QPB repo），从repo根传递`.`。对于任何其他目标，将路径传递给该目标的repo根目录。

####默认行为（无标志）

裸调用触发一个完整的运行：所有6个阶段（Explore→Generate→Code Review→Spec Audit→Reconciliation→Verify），然后是所有4个迭代策略（gap→unfiltered→parity→adversarial），在同一个会话中同步执行。任何先前的`quality/`目录都会在新运行开始之前自动归档到`quality/previous_runs/<TIMESTAMP>/`。

这是规范操作符路径。添加标志时不要征求许可；默认值就是答案。当裸调用触发时，编排器会发出一行标准错误横幅，命名与v1.5.3相比的成本变化（约5 - 10倍于遗留的“仅在第一阶段”默认值）。这条横幅是信息；让它滚动。

####常见覆盖

仅当操作者要求特定的东西时使用：

|需要|标志|效果||------|------|--------|
|运行单相|`--phase N`（其中N∈1..）6) |使用`--phase 1`恢复v1.5.3“仅探索”模式。|
|省略`--iterations`，传递`--phase 1,2,3,4,5,6`|阶段运行；迭代不。|
|特定迭代|`--strategy <name> --next-iteration`|使用选定的策略对现有的`quality/`运行进行迭代。|
|多目标|传递多个位置目标|每个独立运行。|
|每阶段CLI代理|`--claude`/`--copilot`/`--codex`/`--cursor`|选择编排器生成哪个CLI运行器。默认为`--copilot`。V1.5.4增加了`--cursor`运行程序（cursor-cli 3.1+）。|

####从部分/流产的跑步者驱动的跑步中恢复P1-4：操作人员卫生指南：在中断运行后，在下面的**引导模式**部分（“引导运行操作人员卫生”）中进行清理-在模式B中恢复是相同的：`git restore quality/`以丢弃部分阶段1/2输出，然后重新调用。**不要**编辑`quality/`以外的文件来“整理”——源代码不变的不变量会在下次运行时失效。有关完整机制，请参见启动模式卫生段落；无论中止是在自我审计运行期间还是针对外部目标运行期间发生，它都适用。

###启动模式（在自身上运行QPB）

当操作员说“这是一个引导运行”或“我们正在运行QPB本身”或“自我审计”：1. 确认工作目录是QPB repo根目录（或`cd`）。
2. 调用`python3 -m bin.run_playbook .`-相同的规范形式，目标是`.`。
3. 编排器自动处理现有`quality/`树到`quality/previous_runs/<TIMESTAMP>/`的归档；你不需要手动清理任何东西。

逃跑的过程和其他目标一样。唯一的区别是审核主体是剧本本身，因此生成的工件描述了QPB自己的质量体系。**启动运行操作符卫生-从部分/中止运行中恢复。**如果先前的引导运行中途终止（例如，源不变的不变量被触发，相位提示错误，操作符按Ctrl-C），工作树可能包含一个写了一半的`quality/`目录加上一个标记废弃存档的`quality/previous_runs/<TIMESTAMP>/.partial`sentinel。在重新调用之前，运行`git restore quality/`（如果您想要一个干净的记录，还可以运行`git clean -fd quality/`），以从中止的运行中删除任何未提交的阶段1/2输出。编排器将重新归档现在处于原始状态的`quality/`树，并从头开始。**不要**编辑`quality/`以外的文件来“整理”-`quality/`以外的任何文件都是QPB源；触摸它进行清理将在下次运行时触发源不变的不变量。2026-04-30的启动测试暴露了这个确切的恢复问题：操作人员在中断的Phase 2中有一个写入了一半的`quality/`，并且没有重新运行t恢复留下的陈旧的阶段1工件，这些工件会混淆下次运行的归档。机制（指针式，不复制设计文件）

与v1.5.3相比，指针形式的新特性是什么（规范体系结构在`docs/design/QPB_v1.5.4_Design.md`第1部分中）：- **阶段1产生`quality/exploration_role_map.json`** -在探索过程中人工智能驱动的每个文件角色标记。每个作用域内文件都从分类法（`skill-prose`、`skill-reference`、`skill-tool`、`code`、`test`、`docs`、`config`、`fixture`、`formal-spec`、`playbook-output`）中获得一个角色。角色映射驱动每个下游管道激活决策。
- **`INDEX.md`使用`schema_version: "2.0"`**，`target_role_breakdown`字段携带每个角色的计数和百分比。v1.5.3`target_project_type`enum已退役（遗留档案仍然可读）。
- **管道从角色映射激活，而不是从项目类型标签激活。**四步技能派生管道运行在标记为`skill-prose`/`skill-reference`的文件上。代码审查管道在标记为`code`的文件上运行。从散文到代码的差异检查在标记为`skill-tool`的文件上运行。当角色映射显示角色为零时，该管道将干净地停止操作。没有Code/Skill/Hybrid三切分——两个管道都在表面时运行s是存在的（“总是混合下游”模型）。
- **存档目录为`quality/previous_runs/`**（版本为`quality/runs/`）；旧路径上的遗留档案仍然可读。
- **第6阶段结束的重组**将中间工件移到`quality/workspace/`下，因此顶级`quality/`目录由规范交付物（REQUIREMENTS.md，BUGS.md等）主导。门的路径解析器从两种布局中读取。你不需要在快速推理中重新推导这些；编排器的提示已经对它进行了编码。如果您遇到与这里总结的体系结构相冲突的阶段提示，请遵循阶段提示—它是每个阶段契约的规范源。

护栏（机器可检查；作为硬约束）

这些不是建议；编排器强制执行它们，违反则终止运行：1. **同步执行-没有子代理委托。**在同一会话中自己运行每个阶段。**不要使用任务工具**、子代理调度、后台代理调用或任何“将阶段2-6委托给worker”模式。B-15故障模式是真实存在的：阶段1完成，阶段2-6在失去父会话的委托代理中静默死亡，运行程序自标记`-PARTIAL`，操作员没有得到任何错误的信号。V1.5.4提示明确禁止此操作。
2. **不要在运行中修补QPB源代码。**如果在运行过程中遇到`bin/`、`.github/skills/`、`agents/`、`references/`、`SKILL.md`、`schemas.md`或`AGENTS.md`中的错误，**STOP并报告**：命名文件：行，描述失败，提出修复形状-但不要应用修复。编排器在运行开始时捕获git-SHA基线，并在每个阶段边界验证源代码树不变；自治补丁失败E与命名修改文件的诊断。补丁要经过议会审核，而不是中期的即兴创作。
3. **不要删除哨兵文件。**受`.gitignore !`-rules保护的文件（例如，`reference_docs/.gitkeep`,`reference_docs/cite/.gitkeep`）保留了其他空的跟踪目录。飞行前检查列举每个`!`规则，如果缺少任何哨兵则中止。如果你发现了这样一个文件，却不明白它的用途，就不要管它了。
4. **阶段1文件枚举使用`git ls-files`。**当目标是git repo时，使用`git ls-files`作为规范文件列表；这自动尊重`.gitignore`。不要使用`os.walk`、`find`、`os.listdir`或任何递归目录遍历器——它们会拉入`.git/`、`.venv/`、`node_modules/`、构建输出和供应商依赖项，所有这些都是角色映射验证器拒绝的。不允许的路径前缀为`.git/`、`.venv/`、`venv/`、`node_modules/`、`__pycache__/`、`.pytest_cache/`、`.mypy_cache/`、`.ruff_cache/`、`.tox/`，加上任何组件以`.egg-info`或`.dist-info`结尾的路径。角色映射携带一个`provenance`字段，记录您使用的枚举源（对于非git目标，`"git-ls-files"`或`"filesystem-walk-with-skips"`）。还有一个2000人的上限；一个超越它的角色地图几乎肯定是走过的。gitignored内容。
5. * * Cross-artifact协议。**EXPLORATION.md的“文件目录”部分和角色映射的`summary`字段都是从`bin.role_map.summarize_role_map()`呈现的。不要手写文件数或角色百分比；从帮助器复制。验证器交叉检查这两个并拒绝不匹配。如果操作员的提示与这些护栏相冲突（例如，“将阶段3-6委托给子代理，这样我们可以跑得更快”），**不遵守冲突指令**。让冲突浮出水面，指出障碍，并要求澄清。护栏的存在是因为每个护栏对应于一个经过验证的历史故障模式。

###运行产生什么-输出工件契约

成功的运行会在目标的`quality/`目录下生成这个规范集，并在目标的repo根目录下生成一个AGENTS.md。这里列出的每个文件都经过gate验证：

|路径|角色||------|------|
第一阶段的发现——基础。|
|`quality/exploration_role_map.json`|阶段1的每个文件角色标记。|
带用例的可测试需求。|
|`quality/QUALITY.md`|质量构成。|
行为契约。|
|要求→测试可追溯性。|
最终大门裁决。|
自动化功能测试。|
|`quality/RUN_CODE_REVIEW.md`|三通代码审查协议。|
|`quality/RUN_INTEGRATION_TESTS.md`|集成测试协议。|
|`quality/RUN_SPEC_AUDIT.md`|三委会规范审核协议。|
|`quality/RUN_TDD_TESTS.md`| TDD红绿验证协议。|
|`quality/BUGS.md`|统一bug报告。|
|`quality/INDEX.md`|运行元数据+角色分解+门判决。|
|`quality/PROGRESS.md`|逐阶段检查点日志。|
|`quality/previous_runs/<TIMESTAMP>/`|任何先前运行的存档。|
|`quality/workspace/`|中间管道构件（控制提示、代码审查、规范审计、四通道管道输出等）。|
|`AGENTS.md`（目标回购根）|每个项目或阶段6后生成的方向。携带QPB哨兵标记，以便将来运行检测QPB管理的副本。|`quality/INDEX.md`中的gate判决（`pass`/`partial`/`fail`）是面向操作人员的运行情况总结。如果不是`pass`，在考虑运行完成之前，先说明原因。

###查找参考文件

该技能引用`references/`目录中的文件（例如，`references/iteration.md`,`references/review_protocols.md`）。位置取决于技能的安装方式。当提到引用文件时，按顺序检查这些路径并使用第一个存在的路径来解决它：

1.`references/`（相对于SKILL.md-从技能目录运行时工作）
2.`.claude/skills/quality-playbook/references/`（Claude Code安装）
3.`.github/skills/references/`（GitHub Copilot平装）
4.`.github/skills/quality-playbook/references/`（备用副驾驶安装）

本技能中提到的所有参考文件都使用简短形式`references/filename.md`。如果相对路径无法解析，则遍历上面的回退列表。

##为什么存在大多数软件项目都有测试，但很少有质量系统。测试检查代码是否有效。质量体系回答了更难的问题：对于这个特定的项目，“正确工作”意味着什么？有哪些方法可能会失败而不会被测试发现？每个开发人员（人类或AI）在接触这些代码之前应该知道什么？

如果没有高质量的剧本，每个新的贡献者（以及每个新的AI会话）都将从头开始——猜测什么是重要的，编写看起来不错但没有捕获真正错误的测试，重新发现几个月前已经发现并修复的故障模式。一个高质量的剧本使条规明确、持久和继承。

##这个技能产生什么

九个文件共同构成一个可重复的质量体系：

|文件|目的|为什么重要|执行代码？||------|---------|----------------|----------------|
质量构成-覆盖目标，适合目的的场景，战区预防|每个AI会话首先读取此内容。它告诉他们“足够好”是什么意思，这样他们就不会猜测了。|不|
|`quality/REQUIREMENTS.md`|带有项目概述、用例和叙述的可测试需求——由五阶段管道生成（合同提取→派生→验证→完成→叙述）|代码审查第2和第3关的基础。如果没有要求，审查仅限于结构异常（~65%上限）。有了它们，审查就可以发现意图违反——缺失错误、跨文件矛盾和设计缺陷，这些对代码阅读来说是不可见的。|不|
|`quality/test_functional.*`|源自规范的自动化功能测试|安全网。测试与规范所说的应该发生的事情有关，而不仅仅是代码做了什么。使用项目语言：`test_functional.py`（python）n),`FunctionalSpec.scala`(Scala),`functional.test.ts`(TypeScript),`FunctionalTest.java`（Java）等| **Yes** |
|`quality/RUN_CODE_REVIEW.md`|三通代码审查协议：结构审查、需求验证、跨需求一致性|仅结构审查就遗漏了35%的实际缺陷。三通道管道增加了需求验证和一致性检查——实验证据表明，它发现了所有结构审查条件都看不见的bug。|不|
|`quality/RUN_INTEGRATION_TESTS.md`|集成测试协议——跨所有变体的端到端管道|单元测试通过了，但是系统实际上是端到端与真正的外部服务一起工作吗？| **是** |
每个已确认的bug都集中在一个地方，包括重现细节、规范基础、严重性和补丁参考。唯一的真相来源是什么出了问题以及如何验证它。|不|
|`quality/RUN_TDD_TESTS.md`| TDD红绿验证协议|证明每个bug是真实的（未打补丁的代码测试失败），每个修复工作（打补丁后测试通过）。比单独的bug报告更有力的证据——维护者信任FAIL→PASS演示。| **是** |
|`quality/RUN_SPEC_AUDIT.md`|三个多模型规范审计协议|没有一个单一的人工智能模型可以捕获一切。三个具有不同盲点的独立模型捕捉到任何一个单独模型都无法捕捉到的缺陷
|`AGENTS.md`|在这个项目上工作的任何AI会话的引导上下文|“先读这个”文件。如果没有它，AI会话就会浪费第一个小时去弄清楚发生了什么。|不|加上输出目录：`quality/code_reviews/`，`quality/spec_audits/`,`quality/results/`,`quality/history/`。

该管道还生成支持工件：`quality/PROGRESS.md`（带累积错误跟踪器的分阶段检查点日志）、`quality/CONTRACTS.md`（行为契约）、`quality/COVERAGE_MATRIX.md`（可追溯性）、`quality/COMPLETENESS_REPORT.md`（最终闸门）和`quality/VERSION_HISTORY.md`（审查日志）。阶段7可以额外生成`quality/REVIEW_REQUIREMENTS.md`（交互式评审协议）和`quality/REFINE_REQUIREMENTS.md`（细化通过协议），用于迭代改进。

两个关键的可交付成果是需求文件和功能测试文件。需求文件（`quality/REQUIREMENTS.md`）提供了代码审查协议的验证和一致性通过——这使得代码审查捕获的不仅仅是结构异常。功能测试文件（根据项目的语言和测试框架约定命名）是自动化的安全网。Markdown协议是人类和人工智能代理的文档。完成Artifact Contract

质量检验关（`quality_gate.py`）验证这些工件。如果门检查它，这个技能必须指导它的创建。这是一个规范列表——没有在这里列出的任何工件都不应该是门强制的，任何门检查都应该追溯到这里列出的工件。

|工件|位置|需要吗？|创建于||----------|----------|-----------|------------|
|正式文档清单（v1.5.3） |`quality/formal_docs_manifest.json`|是|阶段1 (`bin/reference_docs_ingest.py`) |
|需求清单（v1.5.3） |`quality/requirements_manifest.json`|是|第二阶段|
|用例清单（v1.5.3） |`quality/use_cases_manifest.json`|是|阶段2 |
| bug manifest (v1.5.3) |`quality/bugs_manifest.json`|如果发现bug |阶段3/4/5|
|引文语义检查（v1.5.3） |`quality/citation_semantic_check.json`|是|第4阶段（第2层理事会）|
|勘探发现|`quality/EXPLORATION.md`|是|第一阶段|
|质量构成|`quality/QUALITY.md`|是|第二阶段|
|需求（UC标识）|`quality/REQUIREMENTS.md`|是|第二阶段|
|行为契约|`quality/CONTRACTS.md`|是|第二阶段|
|功能测试|`quality/test_functional.*`|是|第二阶段|
|回归测试|`quality/test_regression.*`|如果发现|阶段3 |
|代码审查协议|`quality/RUN_CODE_REVIEW.md`|是|第二阶段|
|集成测试协议|`quality/RUN_INTEGRATION_TESTS.md`|是|第二阶段|
|规范审计协议|`quality/RUN_SPEC_AUDIT.md`|是|第二阶段|
| TDD验证协议|`quality/RUN_TDD_TESTS.md`|是|第二阶段|
| Bug追踪器|`quality/BUGS.md`|是|第三阶段|
|覆盖矩阵|`quality/COVERAGE_MATRIX.md`|是|第二阶段|
|完整性报告|`quality/COMPLETENESS_REPORT.md`|是|第二阶段（基线），第五阶段（最终裁决）|
|进度跟踪|`quality/PROGRESS.md`|是|贯穿|
| AI bootstrap |`AGENTS.md`（目标回购根）|是|在阶段6之后由编排器生成-不是阶段2可交付的|
| Bug写入|`quality/writeups/BUG-NNN.md`|如果发现Bug |阶段5 |
|回归补丁|`quality/patches/BUG-NNN-regression-test.patch`|如果发现|阶段3 |
|修复补丁|`quality/patches/BUG-NNN-fix.patch`|可选|第三阶段|
如果bug有红色阶段的结果|阶段5 |
| TDD sidecar |`quality/results/tdd-results.json`|如果发现|阶段5 |
| TDD红色阶段日志|`quality/results/BUG-NNN.red.log`|如果发现bug |第五阶段|
| TDD绿色阶段日志|`quality/results/BUG-NNN.green.log`|如果存在修复补丁|第五阶段|
|集成侧车|`quality/results/integration-results.json`|集成测试运行时|第五阶段|
|机械验证脚本|`quality/mechanical/verify.sh`|是（基准）|阶段2 |
|验证receipt |`quality/results/mechanical-verify.log`+`.exit`|是（基准）|第五阶段|
|分诊探查|`quality/spec_audits/triage_probes.sh`|分诊运行|阶段4 |
|代码审查报告|`quality/code_reviews/*.md`|是|第三阶段|
|规范审计报告|`quality/spec_audits/*auditor*.md`+`*triage*`|是|第四阶段|
| Recheck结果（JSON） |`quality/results/recheck-results.json`| Recheck运行时| Recheck |
| Recheck summary (MD) |`quality/results/recheck-summary.md`| Recheck运行时| Recheck |
|种子检查|`quality/SEED_CHECKS.md`|如果阶段0b运行|阶段0b |
|运行metadata |`quality/results/run-YYYY-MM-DDTHH-MM-SS.json`|是| Phase 1 (created), through (updated) |**Sidecar JSON生命周期：**在完成`tdd-results.json`之前编写所有bug - Sidecar的`writeup_path`字段必须指向现有文件，而不是占位符。类似地，在编写`integration-results.json`之前，运行集成测试并收集结果。

Sidecar JSON规范示例

**`quality/results/tdd-results.json`** -门验证字段名，而不仅仅是存在：```json
{
  "schema_version": "1.1",
  "skill_version": "1.5.6",
  "date": "2026-04-12",
  "project": "repo-name",
  "bugs": [
    {
      "id": "BUG-001",
      "requirement": "REQ-003",
      "red_phase": "fail",
      "green_phase": "pass",
      "verdict": "TDD verified",
      "fix_patch_present": true,
      "writeup_path": "quality/writeups/BUG-001.md"
    }
  ],
  "summary": {
    "total": 3, "confirmed_open": 1, "red_failed": 0, "green_failed": 0, "verified": 2
  }
}
```
`verdict`必须是：`"TDD verified"`、`"red failed"`、`"green failed"`、`"confirmed open"`、`"deferred"`之一。`date`必须是ISO 8601 (YYYY-MM-DD)，不是占位符，也不是将来的。

* *`quality/results/integration-results.json`: * *```json
{
  "schema_version": "1.1",
  "skill_version": "1.5.6",
  "date": "2026-04-12",
  "project": "repo-name",
  "recommendation": "SHIP",
  "groups": [{ "group": 1, "name": "Group 1", "use_cases": ["UC-01"], "result": "pass", "tests_passed": 3, "tests_failed": 0, "notes": "" }],
  "summary": { "total_groups": 12, "passed": 11, "failed": 1, "skipped": 0 },
  "uc_coverage": { "UC-01": "covered_pass", "UC-02": "not_mapped" }
}
```
`recommendation`必须是：`"SHIP"`、`"FIX BEFORE MERGE"`、`"BLOCK"`之一。`uc_coverage`将UC标识符从REQUIREMENTS.md映射到覆盖状态。

###运行Metadata

每次剧本运行都会在`quality/results/run-YYYY-MM-DDTHH-MM-SS.json`创建一个带有时间戳的元数据文件。这支持多模型比较和运行历史跟踪。

**生命周期：**在阶段1开始时创建此文件。在每个阶段完成时更新`phases_completed`、`bug_count`和`end_time`。最后的更新发生在终端门之后。```json
{
  "schema_version": "1.0",
  "skill_version": "1.5.6",
  "project": "repo-name",
  "model": "claude-sonnet-4-6",
  "model_provider": "anthropic",
  "runner": "claude-code",
  "start_time": "2026-04-16T10:30:00Z",
  "end_time": "2026-04-16T11:45:00Z",
  "duration_minutes": 75,
  "phases_completed": ["Phase 0b", "Phase 1", "Phase 2", "Phase 3", "Phase 4", "Phase 5"],
  "iterations_completed": ["gap", "unfiltered", "parity", "adversarial"],
  "bug_count": 12,
  "bug_severity": { "HIGH": 2, "MEDIUM": 5, "LOW": 5 },
  "gate_result": "PASS",
  "gate_fail_count": 0,
  "gate_warn_count": 2,
  "notes": ""
}
```
**必填字段：**`schema_version`，`skill_version`,`project`,`model`,`start_time`。在运行过程中填充所有其他字段。`model`应该是精确的模型字符串（例如，`"claude-sonnet-4-6"`,`"gpt-4.1"`,`"claude-opus-4-6"`）。`runner`标识用于执行剧本的工具（例如，`"claude-code"`,`"copilot-cli"`,`"cursor"`,`"cowork"`）。`duration_minutes`由`end_time - start_time`计算而来。如果不能确定型号或运行程序，请使用`"unknown"`。

##如何使用

剧本设计为每次只运行一个阶段。**每个阶段运行在自己的会话中，有一个干净的上下文窗口，在磁盘上产生文件，下一个阶段读取。这比一次运行所有阶段的结果要好得多——每个阶段都有完整的上下文窗口进行深入分析，而不是与其他阶段竞争空间。**默认行为：只运行阶段1。**当有人说“运行质量剧本”或“执行质量剧本”时，运行阶段1（探索）并停止。在第一阶段完成后，告诉用户发生了什么以及接下来要说什么。用户显式地向前驱动每个阶段。

交互协议——如何引导用户

**在每个阶段和每个迭代之后，停止并打印指导。**使用`#`标题，使其在聊天中突出。指导必须包括：刚刚发生了什么（一行），关键输出是什么，以及继续的确切提示。请参阅下面每个阶段部分之后定义的阶段结束消息。如果用户说“继续”，“继续”，“下一阶段”，“下一个”，或类似的东西，按顺序运行下一阶段。如果所有阶段都完成了，建议第一个迭代策略（间隙）。如果迭代刚刚完成，建议推荐周期中的下一个策略。

**如果用户说“运行所有阶段”，“运行所有”，或“运行整个管道”**，在一个会话中依次运行所有阶段。这使用了更多的上下文，但有些用户更喜欢它。

**如果用户问“帮助”，“这是如何工作的”，“这是什么”，或任何类似的短语，用这个解释来回应（自然地调整措辞，不要逐字复制）：>质量手册发现了结构代码审查无法单独捕获的bug——35%的真正缺陷需要理解代码“应该”做什么。它一步一步地起作用：
>
阶段1（探索）：**了解代码库——架构、风险、故障模式、规范
> - **阶段2（生成）：**生成高质量工件——需求、测试、评审协议
> - **阶段3（代码审查）：**对每个确认的bug进行回归测试的三次审查
> - **阶段4（规范审核）：**三个独立的AI审核人员根据需求检查代码
> - **阶段5（协调）：**闭合循环- TDD对每个bug进行红绿验证
> - **阶段6（验证）：**自检基准验证所有生成的工件
>
>在编号的阶段完成后，您可以运行迭代策略（间隙、未过滤、奇偶校验、对抗性）来查找其他错误——迭代测试通常会在基线的基础上增加40-60%的已确认bug。
>
当你在代码旁边提供文档——规范、API文档、设计文档、社区文档时，剧本效果最好。当您单独运行每个阶段而不是一次运行所有阶段时，结果也会明显更好。
>
b>要开始，可以说：“在这个项目上运行质量剧本。**如果用户问“发生了什么”，“发生了什么”，“我们在哪里”，或者“我下一步该做什么”**，阅读`quality/PROGRESS.md`并给他们一个简洁的状态更新：哪些阶段已经完成，到目前为止发现了多少漏洞，下一步是什么。

文档警告

**在第1阶段开始时，在探索任何代码之前，检查文档。查找名为`docs/`、`reference_docs/`、`doc/`、`documentation/`的目录，或任何收集到的文档文件。还要检查用户是否在提示中提到了文档。

**如果没有找到文档，请立即（在继续之前）打印此警告：**> **重要：未找到项目文档。**质量手册在没有文档的情况下也能工作，但当你提供规范、API文档、设计文档或社区文档时，它会发现更多的bug——以及更高可信度的bug。在受控实验中，丰富文档的运行发现了不同的、更好的bug，而不是只有代码的基线。
>
>如果有可用的文档，可以将其添加到`reference_docs/`目录并重新运行阶段1。否则，我将继续进行仅代码分析。

然后进入第1阶段——不要阻止它，只要确保用户看到警告即可。

###运行特定阶段

用户可以请求任何单独的阶段：```
Run quality playbook phase 1.
Run quality playbook phase 3 — code review.
Run phase 5 reconciliation.
```
当运行一个特定的阶段时，检查其先决条件是否存在（例如，阶段3需要阶段2的工件）。如果缺少先决条件，告诉用户需要首先运行哪个阶段。

迭代模式-改进之前的运行

当存在以前的剧本运行并且您想要查找其他错误时，使用此方法。迭代模式用使用五种策略之一的目标探索取代了阶段1的从头开始的探索，然后将发现与之前的运行合并，并根据合并的结果重新运行阶段2-6。

**何时使用迭代模式：**在完整的剧本运行后，当您认为代码库比第一次运行发现的bug更多时。这对于大型代码库尤其有效，因为一次运行只能覆盖3-5个子系统，对于library/framework代码库，不同的探索路径会发现不同的bug类。**阅读`references/iteration.md`获得详细的策略说明。**该文件包含每个策略、共享规则、合并步骤和完成门的完整操作细节。下面的摘要描述了何时使用每种策略。

**TDD适用于迭代运行。**在迭代运行中，每个新确认的bug都必须经过完整的TDD红绿循环，并产生`quality/results/BUG-NNN.red.log`（如果存在修复补丁，则产生`.green.log`）。质量门强制执行这一点——缺少日志导致FAIL。请参阅`references/iteration.md`共享规则5和阶段5中的TDD日志关闭门。

* *迭代策略。**用户通过在提示符中命名策略来选择策略。如果未命名策略，则默认为`gap`。```
Run the next iteration of the quality playbook.                          # default: gap strategy
Run the next iteration of the quality playbook using the gap strategy.
Run the next iteration using the unfiltered strategy.
Run the next iteration using the parity strategy.
Run an iteration using the adversarial strategy.
```
**推荐周期：**间隙→未过滤→奇偶→对抗性。每种策略找到不同的bug类：- **`gap`**（默认）-扫描以前的覆盖，探索未覆盖的子系统和薄部分。当第一次运行在结构上是合理的，但只覆盖代码库的一个子集时，效果最好。
**`unfiltered`** -纯领域驱动的探索，没有结构约束。没有模式模板，没有适用性矩阵，没有节格式要求。修复结构化探索所抑制的bug。
- **`parity`** -系统地枚举同一合约的并行实现（传输变量、回退链、设置vs重置路径），并区分它们的不一致性。查找只在交叉路径比较中出现的错误。
- **`adversarial`** -重新调查dismissed/demoted分诊结果，并对不满意的判决提出质疑。从保守分类中恢复II型错误。
- **`all`** -运行级便利：按顺序执行间隙→未过滤→奇偶校验→对抗，每个都作为se独立代理会话。如果策略没有发现新的漏洞，就会提前停止。分阶段执行

每个阶段都会在磁盘上生成文件，下一阶段将读取这些文件。这就是上下文在不同阶段之间传递的方式——通过文件，而不是通过对话历史。关键的移交文件是：

- **`quality/EXPLORATION.md`** -阶段1写这个，阶段2读它。包含阶段2生成工件所需的所有内容，而无需重新探索代码库。
- **`quality/PROGRESS.md`** -每个阶段后更新。累积BUG跟踪器确保不会丢失任何发现。
- **生成的工件** （REQUIREMENTS.md，CONTRACTS.md等）-第2阶段编写这些，第3-5阶段读取它们以运行审查、审计和协调。每个阶段边界的模式：完成当前阶段，将所有内容写入磁盘，然后打印阶段结束消息并停止。当用户开始下一阶段时，在继续之前回读需要的文件。这种“先写后读”的循环是阶段边界——例如，它让你在加载复习上下文之前从工作记忆中删除探索上下文。

在继续之前，将第一阶段的勘探结果写入`quality/EXPLORATION.md`。该文件在所有模式下都是必需的。使它彻底：领域识别、架构图、现有测试、规范总结、质量风险、skeleton/dispatch分析、派生需求（REQ-NNN）和派生用例（UC-NN）。阶段2需要生成工件的所有东西都必须在这个文件中。将勘探结果写入磁盘的纪律是推动彻底分析的力量。没有它，模型会在工作记忆中留下模糊的印象，并产生宽泛的、抽象的需求，而忽略了功能级缺陷。编写强制专用性：文件路径、行号、精确的函数名、具体的行为规则。正是这种专一性使得需求足够精确，以便在代码审查期间捕获bug。

---

运行状态插装（v1.5.6 -随时编写事件）`quality/`中的两个文件在整个文件系统中跟踪这次运行的状态，因此运行在运行时是可观察的，在崩溃时是可恢复的，在崩溃后是可审计的。在整个跑步过程中保持这两种状态。- **`quality/run_state.jsonl`** -只追加机器可读的事件日志。每行一个JSON对象。编排器和任何监视器读取该文件以确切地知道运行的位置。
**`quality/PROGRESS.md`** -人类可读的状态文件，在每个事件上自动重写。

**权威模式：**`references/run_state_schema.md`。运行开始时读一次；它定义了完整的事件分类法、必需的字段、交叉验证规则和PROGRESS.md格式。

初始化（在任何阶段工作之前，包括阶段0）如果`quality/run_state.jsonl`不存在：
1. 如果不存在，则创建`quality/`。
2. 将`_index`事件附加到`quality/run_state.jsonl`。必选字段：`event=_index`，`ts`(ISO 8601 UTC与`Z`),`schema_version="1.5.6"`,`event_types`（数组列出本次运行将使用的所有事件类型-至少`_index`，`run_start`,`phase_start`,`pattern_walked`,`pass_started`,`pass_ended`,`finding_logged`,`artifact_written`,`gate_check`,`phase_end`,`error`,`run_end`），`benchmark`（目标名称），`lever_state`（例如正常运行的`"baseline"`），`started_at`。
3. 附加`run_start`事件。必选字段：`event=run_start`，`ts`,`runner`（`claude`/`codex`/`copilot`/`cursor`之一），`playbook_version`（读取自SKILL.md前奏`version`字段），`target_path`。
4. 根据`references/run_state_schema.md`中的格式规范编写`quality/PROGRESS.md`。包括标题（启动/基准/杠杆/跑步者/剧本版本），所有六个阶段的空阶段清单，空最近的事件/工件生产部分。如果`quality/run_state.jsonl`在运行开始时已经存在：这是一个**恢复运行**。参见下面的“简历语义”。

每阶段事件

在每个阶段边界（1到6），写入事件：

—**相位启动：**追加`{"event":"phase_start","ts":"<now>","phase":N}`到`quality/run_state.jsonl`。更新`quality/PROGRESS.md`：用当前时间戳将阶段N标记为正在进行中。
- **在阶段结束时：** *首先交叉验证阶段的预期工件（见下表）。如果验证失败，则追加`{"event":"error","ts":"<now>","phase":N,"message":"<what's missing>","recoverable":true}`并重新运行该阶段。如果验证通过，则追加`{"event":"phase_end","ts":"<now>","phase":N,"key_counts":{...},"artifacts_produced":[...]}`。更新PROGRESS.md：检查阶段N与汇总统计。

**阶段1子事件（除phase_start/phase_end外）：**
-行走七个探索模式后：附加`{"event":"pattern_walked","ts":"<now>","phase":1,"pattern":N,"findings_count":K}`。（每个模式一个事件，即使没有发现。）
—写“`quality/EXPLORATION.md`”时，添加“`{"event":"artifact_written","ts":"<now>","relative_path":"quality/EXPLORATION.md","byte_size":<size>,"line_count":<lines>}`”。

**第四阶段子事件：**
—每通起始点（A ~ D）：`{"event":"pass_started","ts":"<now>","phase":4,"pass":"A"}`。
—每通端：`{"event":"pass_ended","ts":"<now>","phase":4,"pass":"A","output_artifact":"<path>"}`。**第五阶段/第六阶段子事件：**
-每门检查完成：`{"event":"gate_check","ts":"<now>","gate_name":"<name>","verdict":"pass|fail|warn|skip","reason":"<short>"}`。

* *结束运行:* *
—在第六阶段`phase_end`之后：添加`{"event":"run_end","ts":"<now>","status":"success","total_findings":<N>,"final_verdict":"<gate verdict>"}`。对于`recoverable:false`故障，状态为`aborted`，对于不可恢复的运行时错误，状态为`failed`。

在phase_end的交叉验证规则

在编写每个`phase_end`事件之前，验证相应的工件：

|阶段|所需||---|---|
|`quality/EXPLORATION.md`和`quality/PROGRESS.md`满足在SKILL.md:1257-1273处记录的13道检查第1阶段门（6个必需的标题：`## Open Exploration Findings`，`## Quality Risks`,`## Pattern Applicability Matrix`,`## Pattern Deep Dive — *`×3+,`## Candidate Bugs for Phase 2`,`## Gate Self-Check`； PROGRESS第1阶段线标记`[x]`；文件发现≥8个：行引用；≥3个多位置发现；3-4个FULL模式矩阵行；≥2个多功能模式深潜；候选错误源混合≥2来自exploration/risks，≥1来自模式深潜）。`bin/run_state_lib.validate_phase_artifacts(quality_dir, phase=1)`强制全门。|
所有九个生成契约工件都在`quality/`下非空存在：`REQUIREMENTS.md`、`QUALITY.md`、`CONTRACTS.md`、`COVERAGE_MATRIX.md`、`COMPLETENESS_REPORT.md`、`RUN_CODE_REVIEW.md`、`RUN_INTEGRATION_TESTS.md`、`RUN_SPEC_AUDIT.md`、`RUN_TDD_TESTS.md`。加上至少一个非空的`quality/test_functional.<ext>`（扩展名因语言而异）。|
bbb3 |`quality/RUN_CODE_REVIEW.md`存在|
| 4 |`quality/REQUIREMENTS.md`非空且`quality/COVERAGE_MATRIX.md`存在。如果四步技能派生管道运行（即存在`quality/phase3/`），则`quality/phase3/pass_a_drafts.jsonl`、`quality/phase3/pass_b_citations.jsonl`、`quality/phase3/pass_c_formal.jsonl`和xqz30下的D步收件箱XQZ必须全部存在且非空。|
bbb5 |`quality/results/quality-gate.log`存在，非空|
bbb6 |`quality/BUGS.md`非空与`^##\s+BUG-`部分和`quality/INDEX.md`更新与`gate_verdict`字段|如果检查失败，则附加`error`事件（可恢复=true）并重新运行该阶段。不要** **编写`phase_end`来对付丢失的工件——这是v1.5.6构建时要捕获的故障模式。`bin/run_state_lib.validate_phase_artifacts(quality_dir, phase)`以编程方式执行这些检查—如果可用，从playbook会话内部调用它。

###恢复语义

如果剧本开始时`quality/run_state.jsonl`已经存在（前一个会话崩溃或在运行中暂停）：1. 读取所有事件。使用`bin/run_state_lib.last_in_progress_phase(events)`查找最后一个没有跟在匹配的`phase_end`后面的`phase_start`—称之为正在进行阶段。
2. 为该阶段运行上面的交叉验证规则。
**工件完成：**之前的会话完成了工作，但没有写`phase_end`。添加缺失的`phase_end`（使用当前的`ts`），然后进入下一阶段。
**工件不完整：**从头开始重新运行该阶段。
3. 如果所有六个`phase_end`事件都存在，但没有`run_end`：添加`run_end status=success`并完成。
4. 如果不存在`quality/run_state.jsonl`，则重新运行。根据上面的部分进行初始化。

策略：信任工件多于信任事件。**如果事件声称阶段4完成，但`REQUIREMENTS.md`不存在，重新运行阶段4。如果事件在中间阶段停止，但是工件已经完成，则要赶上事件。PROGRESS.md原子重写在每个事件上重写PROGRESS.md（不追加）。内容反映当前运行状态。jsonl: header（运行元数据）、阶段检查表（每个已完成阶段的汇总统计，当前阶段的进行中标记）、最近的事件（jsonl日志中的最近10个事件，以人类可读的形式）、生成的工件（编写的文件以字节大小运行）。有关确切的格式模板，请参阅`references/run_state_schema.md`。`bin/run_state_lib.write_progress_md(quality_dir, events, current_phase)`从事件列表中生成格式正确的PROGRESS.md—在每个事件之后调用它以保持文件同步。

---

阶段0：先验运行分析（自动）**此阶段仅在`quality/previous_runs/`存在并且包含先前的质量工件时运行。**如果没有先前的运行，则跳到阶段1。如果`quality/previous_runs/`存在，但是为空或者不包含一致性质量工件（没有`quality/BUGS.md`下的子目录），则跳过阶段0a，直接进入阶段0b。（以前版本1.5.4的`quality/runs/`的遗留存档为了向后兼容仍然是可读的——参见SKILL.md:149——但是规范存档根是`quality/previous_runs/`。）

当先前的运行存在时，剧本进入延续模式。这使得可以迭代地发现bug：每次运行都继承先前运行中确认的发现，机械地验证它们，并探索额外的bug。当运行发现零净新错误时，迭代收敛。步骤0a：构建种子列表。**读取所有先前运行的`quality/previous_runs/*/quality/BUGS.md`。对于每个确认的bug，提取：bug ID、文件行、摘要和回归测试断言。按文件：行重复数据删除（在多次运行中发现的相同错误计数一次）。将合并的种子列表写入`quality/SEED_CHECKS.md`，格式如下：```markdown
## Seed Checks (from N prior runs)

| Seed | Origin Run | File:Line | Summary | Assertion |
|------|-----------|-----------|---------|-----------|
| SEED-001 | run-1 | virtio_ring.c:3509-3529 | RING_RESET dropped | `"case VIRTIO_F_RING_RESET:" in func` |
```
**步骤0b：机械地执行种子检查。**对于每个种子，对当前源树运行断言。记录PASS（错误自上次运行后被修复）或FAIL（错误仍然存在）。一个失败的种子是一个确认的结转错误——它必须出现在这次运行的BUGS.md中，而不管是否有审计员独立地发现它。一个传递的种子意味着bug被修复了——在PROGRESS.md中将其标注为“seed - nn: resolved since prior run”。

**步骤0c：确定先前运行的范围。**阅读`quality/previous_runs/*/quality/PROGRESS.md`的范围声明。注意在之前的运行中涵盖了哪些子系统。在第1阶段的探索中，优先考虑之前运行未覆盖的区域，以最大化发现新漏洞的机会。如果在之前的运行中涵盖了所有的子系统，那么用不同的重点探索相同的范围（例如，不同的审查区域，不同的入口点）。**步骤0d：将种子注入下游阶段。**种子列表成为输入：
- **阶段3（代码审查）：**添加代码审查提示：“先前的运行确认了这些错误-验证它们仍然存在，并在相同的子系统中寻找其他发现。”
- **阶段4（规范审核）：**添加到`RUN_SPEC_AUDIT.md`：“以前运行的已知开放问题：[种子列表]。期望审计员发现这些。如果审核员没有标记已知的种子错误，那就是他们审查的覆盖范围存在差距，而不是错误被修复的证据。”**不确定的范围探索意味着不同的运行会注意到不同的bug。在跨版本测试中，4/8repos在某些版本中发现了bug，但在其他版本中没有发现——不是因为bug得到了修复，而是因为模型探索了代码库的不同部分。使用种子注入进行迭代解决了这一问题：已确认的漏洞可以机械地向前推进（不需要重新发现），并且每次新运行都可以专注于探索未发现的领域。

阶段0b：兄弟姐妹运行的种子发现（自动）

**此步骤仅在`quality/previous_runs/`不存在或`quality/previous_runs/`存在但不包含一致性质量工件**（即，阶段0a没有任何工作）**和**项目目录是版本化的（例如，`httpx-1.3.23/`位于`httpx-1.3.21/`旁边）时运行。如果`quality/previous_runs/`与一致性工件一起存在，则阶段0a已经处理了种子注入—跳过此步骤。**如果`quality/previous_runs/`存在但为空或只包含不符合标准的子目录**，则发出警告：“阶段0b:`quality/previous_runs/`存在但不包含符合标准的工件-请咨询同级版本目录以获取种子。”然后继续下面的兄弟姐妹发现。

当不存在`quality/previous_runs/`目录，但同级版本目录存在时，在同级目录中查找先前的高质量工件：1. * *发现兄弟姐妹。**列出相对于父目录匹配模式`<project-name>-<version>/quality/BUGS.md`的目录。排除当前目录。按版本降序排序（首先是最近的）。
2. **导入确认的bug作为种子。**对于每个具有`quality/BUGS.md`的兄弟，使用与步骤0a相同的格式提取已确认的错误。将它们写入`quality/SEED_CHECKS.md`，并将origin标记为同级目录名。
3. **机械地执行种子检查**（与阶段0a中的步骤0b相同）。对于每个导入的种子，针对当前源树运行断言并记录PASS/FAIL.4. **注入下游相**（与阶段0a中的步骤0d相同）。**为什么存在：**在v1.3.23基准测试中，尽管httpx-1.3.21发现了`Headers.__setitem__`非ascii编码错误，但httpx产生了零错误的结果。该模型只是探索不同的代码路径，而从不检查header区域。兄弟运行种子确保即使没有显式的`quality/previous_runs/`存档，也可以继续在以前版本的运行中确认错误。这是一种不同于机械篡改的故障类型——它解决的是探索的非确定性，而不是证据损坏。

---

第1阶段：探索代码库（随你去写）

**v1.5.6 instrumentation:**现在将`phase_start phase=1`追加到`quality/run_state.jsonl`。遍历每个勘探模式后，附加`pattern_walked phase=1 pattern=N findings_count=K`。在阶段结束时，交叉验证（`quality/EXPLORATION.md`≥200字节与查找段），然后附加`phase_end phase=1`。请参阅上面的“运行状态插装”。> **本阶段所需的参考资料** -在继续之前阅读这些：
> -`references/exploration_patterns.md`-开放探索后应用的七个bug查找模式

**第一个动作：创建运行元数据。**在进行任何探索之前，创建运行元数据文件：```bash
mkdir -p quality/results
cat > "quality/results/run-$(date -u +%Y-%m-%dT%H-%M-%S).json" <<'METADATA'
{
  "schema_version": "1.0",
  "skill_version": "1.5.6",
  "project": "<repo-name>",
  "model": "<model-string>",
  "model_provider": "<provider>",
  "runner": "<tool>",
  "start_time": "<ISO-8601-UTC>",
  "end_time": null,
  "duration_minutes": null,
  "phases_completed": [],
  "iterations_completed": [],
  "bug_count": 0,
  "bug_severity": { "HIGH": 0, "MEDIUM": 0, "LOW": 0 },
  "gate_result": null,
  "gate_fail_count": null,
  "gate_warn_count": null,
  "notes": ""
}
METADATA
```
填写`project`，`model`（精确模型字符串，例如，`"claude-sonnet-4-6"`），`model_provider`（例如，`"anthropic"`,`"openai"`,`"cursor"`），`runner`（例如，`"claude-code"`,`"copilot-cli"`,`"cursor"`）和`start_time`（UTC ISO 8601）。在每个阶段结束时更新此文件—将完成的阶段附加到`phases_completed`，并在确认bug时更新`bug_count`/`bug_severity`。终端闸填充`end_time`、`duration_minutes`和`gate_result`后的最后更新。

第二个动作：运行v1.5.3文档摄取（在探索任何代码之前）。**`bin/`中的一个stdlib-only模块产生第一阶段需求派生所依赖的权威文档记录：1. **`python -m bin.reference_docs_ingest <target>`** -在目标库中遍历`reference_docs/`一次。`reference_docs/cite/`下的文件被散列并根据`schemas.md`§4和§1.6清单包装器写入`quality/formal_docs_manifest.json`。`reference_docs/`顶层的文件不写入清单，但可以通过`bin.reference_docs_ingest.load_tier4_context(<target>)`作为第4层上下文使用，它返回`(path, text)`元组的排序列表。如果ingest命令失败（不支持的扩展，非utf -8字节），停止运行并将错误输出逐字呈现给用户—摄取错误是可操作的，必须在继续探索之前修复。**不需要挎斗。**文件夹位置是标志：顶级`reference_docs/<name>.<ext>`文件是Tier 4上下文；`reference_docs/cite/<name>.<ext>`下的文件是可引用的源代码。Tier 1是`cite/`内容的默认值；文件可以在第一个非空白行上使用可选的文件内标记覆盖到第2层：`<!-- qpb-tier: 2 -->`（Markdown）或`# qpb-tier: 2`（明文）。跳过任意文件夹下的`README.md`。

**当`reference_docs/`缺失或为空**时，阶段1必须打印此可操作消息并继续：

>阶段1在reference_docs/.中没有发现任何文档。剧本将继续进行
>仅使用第3层证据（源树本身）。为了获得更好的效果，请放弃
>明文文档：
AI聊天、设计笔记、回顾（Tier 4上下文）
>reference_docs/cite/←项目规范，rfc， API合同（可引用，字节验证）
b>请参阅README.md“步骤1：提供文档”了解详细信息。**纯文本-转换发生在剧本之外。**参考文档仅为`.txt`或`.md`（schemas.md§2）。pdf、DOCX、HTML等都会被拒绝，并提供一个可操作的转换提示（`pdftotext`、`pandoc -t plain`、`lynx -dump`）。不要试图解析二进制或格式化的文档内部的技能-运行转换外，并提交明文。

在第一阶段了解项目。质量手册必须以这个特定的代码库为基础——而不是一般的建议。**为什么先探索？**在人工智能生成的质量剧本中，最常见的失败是生成通用的内容——可以适用于任何项目的覆盖目标、描述理论失败的场景、练习语言构建而不是项目代码的测试。探索可以通过强制每个输出引用真实的东西来防止这种情况：特定的函数、特定的模式、特定的防御代码模式。如果您不能指出代码中的某些内容，那么您就是在猜测—而猜测产生的高质量剧本没有人信任。**扩展大型代码库：**对于拥有超过50个源文件的项目，不要尝试阅读所有内容。重点探索3-5个核心模块（处理主要数据流、最复杂的逻辑和最容易出错的操作的模块）。从每个子系统读取代表性测试，而不是从每个测试文件读取。目标是在重要的事情上有深度，而不是在所有事情上有广度。**深度大于广度（临界）。**具有功能级细节的窄范围比具有子系统级摘要的宽范围发现更多的错误。对于您探索的每个核心模块，确定实现关键行为的特定函数，并通过名称、文件路径和行号记录它们。来自“重置子系统应该处理错误”的需求不会捕获错误。从“`vm_reset()`at`virtio_mmio.c:256`必须在写入零后轮询状态寄存器”派生的需求将。有用的探索和无用的探索之间的区别在于特异性——文件路径、函数名、行号、精确的行为规则。

**三阶段探索：首先开放，然后领域风险，然后选择模式。**探索有三个阶段，顺序很重要：1. **开放探索（领域驱动）。**在应用任何结构化模式之前，以经验丰富的开发人员的方式探索代码库：阅读代码，理解架构，根据您对系统中出错的领域知识识别风险。问问你自己：“这个领域的专家首先会检查什么？”对于HTTP库，这意味着重定向处理、标头编码、连接生命周期。对于CLI框架，这意味着标志解析、帮助生成、completion/validation一致性。对于序列化库，这意味着类型覆盖、往返保真度、边缘情况处理。用文件路径和行号写出具体的发现。这个阶段必须产生至少8个具体的bug假设或可疑的发现——不是架构上的观察，而是具体的“文件行上的代码可能因为[原因]而出错”的发现。至少有4个必须引用difference租用模块或子系统。2. **领域知识风险分析。**在开放探索之后，从代码中退一步，思考你从训练中了解到的关于这样一个系统的知识。这是库和框架代码库的主要bug搜索通道。使用两个源完成下面第6步的问题——你刚刚研究的代码和你对类似系统的领域知识。生成至少5个分级的故障场景，每个场景命名一个特定的函数、文件和行，并解释为什么特定领域的边缘情况会产生错误的行为。你不需要观察这些失败——你从训练中知道它们发生在这种类型的系统中。在处理模式之前，将结果写入EXPLORATION.md的`## Quality Risks`部分。**列出代码已经具有的防御模式（代码做对的事情）的部分不是风险分析。列出有风险模块而没有具体故障场景的部分不是风险分析。结论是“这是一个成熟的、经过良好测试的库，所以不太可能有基本的bug”的部分是非常有害的——成熟的库有最细微的bug，正是因为那些明显的bug在几年前就被发现了。测试：代码审查者是否可以阅读每个场景并立即知道要检查什么？如果不是，那么这个场景就太抽象了。3. 模式驱动的探索（选定的，不是详尽的）。**将开放探索和领域风险分析写入磁盘后，使用模式适用性矩阵评估`exploration_patterns.md`中的所有七个分析模式。对于每个模式，评估它是否适用于此代码库以及它的目标是什么。然后选择3到4个模式进行深入处理——对于这个特定的代码库，这是产量最高的模式。剩下的模式会得到一个简短的“不适用”或“延迟”的注释，其中包含特定于代码库的基本原理。不要为所有七种图案制作深的部分-深度在3-4拍的浅覆盖7。当第四个模式具有明确的适用性，并且将覆盖其他三个模式未涉及的代码区域时，选择4；如果有疑问，默认为3。对于每个选择的模式深入研究，使用参考文件中的输出格式，并跨2个以上函数跟踪代码路径。深潜应该进行压力测试，改进或扩展开放勘探和风险分析的发现，而不是重复它们。

阶段1完成门检查所有三个阶段。开放勘探段、质量风险段、模式适用性矩阵和模式深潜段都必须存在。

增量写入-不将发现保存在内存中。**这是阶段1中最重要的执行规则。在探索每个子系统或应用每个模式之后，**立即将您的发现附加到磁盘上的`quality/EXPLORATION.md`，然后再移动到下一个子系统或模式。**不要试图在多个子系统的工作记忆中保存发现。随用随写原则有两个目的：1. * *深度恢复。**如果您探索PCI中断路由子系统并在`vp_find_vqs_intx()`中发现可疑代码，请立即将该发现写入EXPLORATION.md。然后，当您移动到管理队列子系统时，您的工作记忆可以自由地深入到那里。如果没有增量写入，来自第一个子系统的发现就会与来自第二个子系统的发现相互竞争，结果都很肤浅。

2. **什么都不会丢失。**在v1.3.41基准测试中，模型探索了8个模式部分，但每个部分只写了5-7行-完全均匀，完全浅。每个部分都通过了审核，但没有一个深入到发现需要在多个函数之间跟踪代码路径的bug。在阅读完所有内容后，模型试图在最后组成整个EXPLORATION.md，并且只能回忆起表面水平的发现。增量写可以防止这种情况。**顺序为：读一个子系统→将发现写入磁盘→读下一个子系统→追加发现→重复。**每个追加应该包括具体的函数名、文件路径、行号和具体的bug假设。“检查了跨执行一致性，发现了一个缺口”的5行文字只是一个过门的占位符，而不是探索发现。一个有用的部分跟踪代码路径：“函数A在file:line调用函数B在file:line，它执行X，但不执行Y；与同时做X和y的函数C相比：**强制合并步骤。**在对所有三个阶段（开放探索、质量风险和选择模式深度探索）进行探索并写入EXPLORATION.md之后，添加最后一个部分：`## Candidate Bugs for Phase 2`。本节将前面所有章节中最强的错误假设合并到一个优先级移交列表中。对于每个候选，包括：假设，特定的文件，行引用，哪个阶段出现了它（开放探索，质量风险，或模式），以及代码审查应该寻找什么。这个部分是探索和生成工件之间的桥梁——它告诉阶段3应该关注哪里。最少4个文件行引用的候选bug——至少2个来自开放探索或质量风险，至少1个来自模式深度挖掘。没有最大值。

**飞行前：大型存储库的作用域声明**在研究任何源代码之前，估算规模：大致的源文件数量（不包括测试、文档和生成的文件）、主要子系统数量和文档数量。注意PROGRESS.md中的计数。- **少于200个源文件：**继续充分探索。上面的深度vs广度指导仍然适用。
- ** 200-500源文件：**在探索之前声明您的预期作用域。编写`## Scope declaration`部分到PROGRESS.md，命名您将涉及的3-5个子系统，每个子系统的预期文件数，以及您将根据基本原理推迟哪些子系统。然后只继续探索声明的作用域。
- **超过500个源文件：**在读取任何源文件之前，停止并写一个强制性的作用域声明到PROGRESS.md。范围声明必须包括：(a)本次运行中覆盖的子系统，(b)显式延迟的子系统，(c)每个延迟子系统的排除原理，以及(d)后续运行的推荐子系统范围。在写完之前不要开始探索。覆盖“所有内容”的作用域声明对超过t的存储库无效他的阈值。**恢复之前的会话：**如果PROGRESS.md已经存在，并且显示已完成的阶段，请先读取它。不要重做已经标记为完成的阶段-从标记为未完成的第一个阶段开始。如果已经编写了作用域声明，请严格遵守它。如果前一个会话的范围声明延迟了子系统，不要扩展范围来覆盖它们，除非这次运行是延迟区域的显式后续。**规范-主要存储库：**一些存储库将规范、配置或协议文档作为其主要产品，并将可执行代码作为支持基础结构。示例：一个带有基准工具的技能定义，一个带有验证脚本的模式注册表，一个带有编排助手的管道配置。当主要产品是规范而不是可执行代码时，从规范的内部一致性、完整性和正确性中派生需求，而不仅仅是从可执行代码路径中派生需求。规范是用户依赖的东西；工具是次要的。如果您发现自己编写了80%以上关于helper脚本的需求，而不到20%关于主要规范的需求，那么您的关注点就颠倒了。

###步骤0：询问发展历史

在探索代码之前，问用户一个问题：“在开发这个项目的过程中，您是否导出了AI聊天记录- Claude导出，Gemini外卖，ChatGPT导出，Claude代码副本或类似内容？”如果有，告诉我文件夹在哪。这些讨论中的设计讨论、事件报告和质量决策将大大提高生成的质量剧本。”

如果用户提供聊天记录文件夹：1. **先扫描索引文件。**查找名为`INDEX*`，`CONTEXT.md`，`README.md`的文件，或类似的导航设备。如果有的话，读一读——它会告诉你有什么，以及如何找到东西。
2. **搜索与质量相关的对话。**查找提到以下内容的消息：质量、测试、覆盖率、bug、失败、事故、崩溃、验证、重试、恢复、规格、适应性、审计、审查。还要搜索项目名称。
3. **提取设计决策和事件历史。**最有价值的内容是：(a)事件报告——哪里出了问题，有多少记录受到影响，问题是如何被发现的，(b)设计讨论——为什么选择了一种特定的方法，哪些替代方案被拒绝了，(c)质量框架讨论——覆盖目标，测试哲学，模型审查经验，(d)跨模型反馈——不同的人工智能模型对代码的意见不一致。
4. 不要试图逃避d一切。**聊天记录可能非常庞大。使用索引查找最相关的对话，然后在这些对话中搜索与质量相关的内容。10分钟的目标搜索胜过2小时的详尽阅读。这个背景是黄金。开发人员讨论“为什么我们选择这种并发模型”或“我们在生产中丢失1,693条记录的时间”的聊天历史记录将一般场景转换为权威场景。

如果用户没有聊天记录，那就正常进行——没有聊天记录，技能也能正常工作，只是缺少上下文。

**自主回退：**在基准模式下运行时，通过`bin/run_playbook.py`（基准运行程序，不随技能发货），或没有用户交互（例如，`--single-pass`），跳过步骤0的问题，直接进入步骤1。如果聊天记录文件夹在项目树中可见（例如，`AI Chat History/`,`.chat_exports/`），无需询问即可扫描它们。如果没有找到聊天记录，继续-不要阻止等待不会出现的响应。

步骤1：确定域、堆栈和规范

阅读README、现有文档和构建配置（`pyproject.toml`/`package.json`/`Cargo.toml`）。答:-这个项目是做什么的？(一个句子)。
-什么语言和关键依赖项？
-它与什么外部系统对话？
——主要产出是什么？

**查找规格。**规范是功能测试的真实来源。按顺序搜索：`AGENTS.md`/`CLAUDE.md`在根目录下，`specs/`,`docs/`,`spec/`,`design/`,`architecture/`,`adr/`，然后`.md`文件在根目录下。记录路径。

**如果没有正式的规范文档存在**，该技能仍然有效-但您需要从其他来源组装需求。按偏好顺序：1. **询问用户**——即使没有写下来，他们通常也知道需求。
2. **README和内联文档** -许多项目在其README、API文档或代码注释中嵌入需求。
3. **现有的测试套件**测试是隐式规范。如果测试断言`process(x) == y`，这是一个需求。
4. 类型签名和验证规则——模式、类型注释和验证器定义了系统接受和拒绝的内容。
5. **从代码行为中推断——作为最后的手段，阅读代码并推断它应该做什么。在QUALITY.md中将这些标记为“推断需求”，并标记它们以供用户确认。

当从非正式需求着手时，用一个需求标签来标记每个场景和测试，其中包括一个置信度层和来源：-`[Req: formal — README §3]`-由人类在规范文档中编写。权威。
-`[Req: user-confirmed — "must handle empty input"]`-由用户声明，但不是在正式的文档中。将其视为权威。
-`[Req: inferred — from validate_input() behavior]`-由代码推导。供用户审查的标志。

在QUALITY.md场景、功能测试文档和规范审计发现中使用这种精确的标记格式。它明确了哪些需求是权威的，哪些需要验证。

步骤1b：评估文档深度

如果存在`reference_docs/`，在决定关注哪个子系统之前，读取其中的每个文件。对于每个文档，对其深度进行分类：- **Deep** -包含内部契约、安全不变量、并发模型、防御模式、错误处理细节或行号级源代码引用。适用于推导需求。
- **一般** -涵盖架构和API表面以及一些实现细节。对定位有用，但单独对需求派生是不够的。
- **浅** - API目录，功能概述，或营销级摘要。列出了存在的东西，但没有列出它是如何工作的，它是如何失败的，或者它强制执行了什么契约。**不足以决定范围**范围规则：**不要将审计范围缩小到只有具有深度文档的子系统。如果最复杂或最容易发生故障的模块只有简单的文档，那么在PROGRESS.md**中标记这是一个**文档差距，而不是跳过该模块的理由。文档最少、风险最高的代码是隐藏bug的地方——只审计文档完备的区域会生成一个看起来安全的报告，而忽略了真正的缺陷。

当高风险区域的文件编制不够充分时：

1. 注意`## Documentation depth assessment`部分下的PROGRESS.md中显式的差距。
2. 直接从源代码中派生需求（文档注释、安全注释、防御模式、现有测试），并将它们标记为`[Req: inferred — from source]`。
3. 在完整性报告中标记更深层次的文档收集区域。在PROGRESS.md中记录每个`reference_docs/`文件的深度分类，以便审阅者可以评估文档是否适当地影响了范围。

**覆盖承诺表：**将所有`reference_docs/`文档分类后，在`## Documentation depth assessment`章节下的PROGRESS.md中生成此表：

|文档|深度|子系统|需求承诺|排除：理由||----------|-------|-----------|------------------------|---------------------------|
对于每个深层文档，将其映射到它所涵盖的子系统，然后要么致力于从它派生需求（“将在阶段2中涵盖”），要么提供命名权衡的特定理由。像“超出了这次运行的范围”这样的句子是不够的——证明必须说“为什么”，例如，“解释器JIT被排除在外，因为这次运行关注的是parser/compiler/GC管道；建议单独运行。”

**Gate:**在`reference_docs/`中深入记录的高风险子系统不能从需求集中无声地消失。如果一个深度文档有一个“将覆盖”的承诺，但是在第7步结束时产生零需求，那么需求管道是不完整的——在继续进行第2阶段工件生成之前，返回并为缺口导出需求。

步骤2：映射体系结构列出源目录及其用途。读取主入口点，跟踪执行流程。识别:

—3-5个主要子系统
-数据流（输入→处理→输出）
-最复杂的模块
-最脆弱的模块

###步骤3：读取现有测试

阅读现有的测试文件——所有这些文件都适用于small/medium项目，或者每个子系统的代表性样本适用于大型项目。识别：测试数量、覆盖模式、差距和任何覆盖范围（看起来不错但没有捕获真正bug的测试）。**紧急：记录导入模式。**现有测试如何导入项目模块？每种语言都有自己的约定（Python的`sys.path`操作，Java/Scala包导入，TypeScript的相对路径或别名，Go的package/module路径，Rust的`use crate::`或`use myproject::`）。您必须在功能测试中使用完全相同的模式—弄错了这个模式意味着每个测试都会失败，并出现import/resolution错误。请参阅`references/functional_tests.md`§“导入模式”了解完整的六语言矩阵。

**识别集成测试运行者。**查找脚本或测试文件，这些脚本或测试文件可以针对真实的外部服务（api、数据库等）对系统进行端到端测试。注意它们的模式——`RUN_INTEGRATION_TESTS.md`需要它们。

###第四步：阅读说明书一节一节地浏览每个规范文档。对于每个部分，问：“这说明了什么可测试的需求？”在没有相应测试的情况下记录规范需求——这些是功能测试必须填补的空白。

如果使用推断的需求（来自测试、类型或代码行为），使用步骤1中定义的`[Req: tier — source]`格式标记每个需求及其置信度层。推断出的需求将被输入到QUALITY.md场景中，并且应该在第7阶段为用户评审做标记。

###步骤4b：读取函数签名和实际数据

在编写任何测试之前，必须确切地知道如何调用每个函数。对于您在步骤2中确定的每个模块：1. **读取实际函数签名** -参数名称，类型，默认值。不要从使用上下文中猜测-阅读函数定义和任何文档（Python文档字符串，Java/ScalaJavadoc/ScalaDoc， TypeScript类型注释，Go godoc注释，Rust doc注释和类型签名）。
2. **读取实际数据文件** -如果项目中有项目文件、fixture文件、配置文件或样例数据（`pipelines/`、`fixtures/`、`test_data/`、`examples/`），读取它们。您的测试装置必须与实际数据形状完全匹配。
3. **读取现有的测试装置** -现有的测试如何创建测试数据？模仿他们的模式。如果他们使用特定的键来构建配置字典，那么就使用那些精确的键。
4. **检查库版本** -检查项目的依赖清单（`requirements.txt`,`build.sbt`,`package.json`,`pom.xml`/`build.gradle`,`go.mod`,`Cargo.toml`）以查看实际可用的版本。不要编写依赖库特性的测试未安装的Res。如果可能缺少依赖项，请使用测试框架的跳过机制——参见`references/functional_tests.md`§“库版本识别”，以获得特定于框架的示例。记录一个函数调用映射：对于你计划测试的每个函数，写下它的名字、模块、参数和它的返回值。这种映射可以防止最常见的测试失败：调用带有错误参数的函数。

步骤5：找到骷髅

这是最重要的一步。搜索防御性代码模式——每一个都是过去失败或已知风险的证据。

**为什么重要：**开发人员不会为了好玩而编写`try/except`块、null检查或重试逻辑。每一段防御代码的存在都是因为有人被烧死了。围绕JSON解析的`try/except`表示在生产环境中出现了格式错误的JSON。字段上的null检查意味着该字段在不应该存在的时候丢失了。这些模式是代码库在窃窃私语它的失败历史。每一个都成为一个适合目的的场景和一个边界测试。**请阅读`references/defensive_patterns.md`**，了解系统搜索方法、grep模式以及如何将结果转换为适合目的的场景和边界测试。

最低标准：每个核心源文件至少2-3个防御模式。如果你发现的更少，你是在略读函数体，而不仅仅是签名。

步骤5a：跟踪状态机

如果项目有任何类型的状态管理——状态字段、生命周期阶段、工作流阶段、模式标志——完全跟踪状态机。这捕获了防御性模式分析单独遗漏的一类错误：存在但未处理的状态。

**在模型、枚举或常量中搜索status/state字段（例如，`status`、`state`、`phase`、`mode`）。搜索在允许操作之前检查状态的保护（例如，`if status == "running"`、`match self.state`）。搜索状态转换（对状态字段的分配）。对于您找到的每个状态机

1. **列举所有可能的状态。**读取enum，常量，或grep为每个字段分配的值。把它们都列出来。
2. 对于状态的每个消费者（UI处理程序，API端点，控制流保护），检查：它是否处理所有可能的状态？没有有意义的默认值的`switch`/`match`，或者不涵盖所有状态的`if/elif`链，都是一个缺口。
3. **对于每个状态转换**，检查：你能到达每个状态吗？有没有可以进入却不能离开的州？是否存在阻塞应该可用的操作的状态？
4. **将差距记录为发现。**如果用户需要在被卡住的进程上执行操作X，允许“运行”而不允许“卡住”的状态保护是一个真正的bug。进入结束状态但从未触发清理的进程是一个真正的bug。**为什么这很重要：**状态机间隙产生的错误在正常操作期间是看不见的，但在压力或边缘条件下会显现出来-正是当您需要系统工作时。处于“卡住”状态时无法终止的批处理处理器，或在所有工作完成后从未自终止的监视器，或拒绝恢复“挂起”运行的UI，都是不完整状态处理的症状。这些错误不会在防御性模式分析中显示出来，因为代码没有对它们进行防御——它根本就没有处理它们。

###步骤5b：映射模式类型

如果项目有验证层（Python中的Pydantic模型、JSON Schema、TypeScriptinterfaces/Zod模式、Java Bean验证注释、Scala case类编解码器），现在就阅读模式定义。对于您发现的每个防御模式的字段，记录模式接受和拒绝的内容。**阅读`references/schema_mapping.md`**了解映射格式，以及为什么这对编写有效的边界测试很重要。

步骤6：领域知识风险分析（代码+领域知识）

**这是库和框架代码库的主要bug搜索通道。**在选择任何结构化模式之前完成它。立即将结果写入EXPLORATION.md的`## Quality Risks`部分—不要将它们保存在内存中。

每个项目都有不同的失败概况。这一步使用了两个源代码——不仅仅是代码探索，而是你在类似系统中出现问题的训练知识。

**从代码探索**，问：
在这个项目中“无声的错误”是什么样子的？
-哪些外部依赖可以在没有警告的情况下改变？
什么东西看起来简单，实际上很复杂？
-横切关注点隐藏在哪里？**根据领域知识**，问：
“这样的系统出了什么问题？”如果它是一个HTTP路由器，考虑报头解析边缘情况（质量值，令牌列表，大小写敏感性），中间件排序依赖关系和路径规范化。如果是HTTP客户端，请考虑重定向凭证剥离、编码检测和连接状态泄漏。如果它是一个序列化库，请考虑null处理不对称、直接方法和视图包装器之间的API表面一致性、延迟求值缓存错误和往返保真度。如果是web框架，请考虑响应助手边缘情况、配置编译链和中间件状态隔离。如果它是一个批处理处理器，请考虑崩溃恢复、幂等性、静默数据丢失和状态损坏。如果它处理的是随机性或统计数据，那就考虑播种、相关性、分布偏差。
——“什么产生co ？看起来正确的产出实际上是错误的？”-这是最危险的一类bug：输出通过了所有检查，但被微妙地破坏了。响应的`200 OK`，但错误的`Content-Type`。重定向成功，但泄露凭证。具有静默截断值的反序列化对象。
“在10倍的规模下会发生什么，而在1倍的规模下不会发生？”-块边界，速率限制，超时级联，内存压力。
-“如果这个进程在最糟糕的时刻被终止会发生什么？”-写入中期、事务中期、批提交中期。
“两个表面在边缘输入上应该有相同漂移的地方在哪里？”重载、别名、sync/asyncapi、构建器vs直接api、直接变异体vs实时views/wrappers、兼容标准库的包装器vs框架原生表面。对于Java/Kotlin:`add(null)`vs`asList().add(null)`，`put(key,null)`vs`asMap().put(key,null)`。对于Python：构造函数编码vs mutator编码，sync vS异步客户端行为。
-“什么会发出看似合理的输出，但元数据却有细微的错误？”-内容类型，字符集，路由模式，ETag强度，字节数，auth/header/cookie传播，状态码，缓存验证器。
-“使用特殊字符串逻辑解析的是什么标准语法或列表语法？”-质量值（`q=0`），逗号分隔的报头，摘要挑战，带参数的MIME类型，查询字符串，enum/keyword集，cookie合并。
“一个领域专家会选择什么样的边缘情况？”—HTTP代码：`Accept-Encoding: gzip;q=0`、`Connection: keep-alive, Upgrade`、`Content-Type: application/problem+json`。对于序列化代码：`null`通过不同的API表面，值在`Integer.MAX_VALUE + 1`，通过编码-解码往返。对于路由代码：重叠模式，挂载前缀传播，相同的路径与不同的方法。
-“用户在进行不可逆转或昂贵的操作之前需要哪些信息？”-运行前成本估算范围的确认（特别是当扇形输出或扩展会增加工作量时）、资源警告。如果系统可以悄无声息地将用户提交到几个小时的处理中，而不向他们显示他们将要做什么，那么这是一个缺失的保护措施。搜索启动长时间运行的进程、提交批处理作业或触发expansion/fan-out的操作，并检查用户是否在不返回点之前看到了预览、估计或实数确认。
“当一个长时间运行的进程结束时会发生什么——它真的停止了吗？”-轮询循环，监视器，后台线程和守护进程运行，直到完成应该有明确的终止条件。如果循环检查“是否还有更多工作？”但从不检查“是否所有工作都已完成？”，则它将在完成后永远运行。这在批处理处理器和队列消费者中尤其常见。根据这些知识生成至少5个排序的故障场景。你不需要观察这些失败——你从训练中知道它们发生在这种类型的系统中。将它们写成带有文件路径和行号引用的特定bug假设，按优先级排序。将它们定义为：“因为[file:line处的代码]执行[X]，所以[领域特定的边缘情况]将产生[错误的行为]而不是[正确的行为]。”然后将它们放在您探索的实际代码中：“读取persistence.py行~340 (save_state)：验证临时文件+重命名模式。”**失败的反模式：**质量风险部分列出代码已经拥有的防御模式（代码做对的事情）并不是风险分析——它是一种保证练习，不会发现bug。列出有风险模块而没有具体故障场景的部分是不可操作的。结论是“这是一个成熟的、经过良好测试的库，所以不太可能有基本的bug”的部分是非常有害的——成熟的库有最微妙的api契约和边缘情况的bug，正是因为明显的bug在几年前就被发现了。测试：代码审查者是否可以阅读每个场景并立即知道要打开什么功能以及要测试什么输入？如果不是，那么这个场景就太抽象了。

步骤7：导出可测试需求

**阅读`references/requirements_pipeline.md`**获得完整的五阶段管道，域检查表和版本控制协议。这是代码审查协议中最重要的一步。在探索过程中发现的所有东西——规范、ChangeLog条目、配置结构、源代码注释、聊天记录——都被提炼成一组可测试的需求，代码审查将对其进行验证。该管道将契约发现与需求派生分离开来，使用基于文件的外部存储器，并包含带有完整性门的机械验证。

**为什么重要：**结构代码审查捕获了大约65%的实际缺陷。剩下的35%是意图违反——缺失错误、跨文件矛盾和设计差距。这些对于代码读取来说是不可见的，因为那里的代码是正确的。您需要知道代码应该做什么，然后检查它是否做到了。这就是可测试需求所提供的。

**五相管道：**1. **阶段A -合同提取。**阅读所有源文件，列出每个行为契约。写到`quality/CONTRACTS.md`。这就是发现——列出所有东西，即使它看起来很明显。
2. 阶段B -需求推导。**阅读CONTRACTS.md和文档。整理相关合同，丰富用户意图，撰写正式要求。将REQ记录写入`quality/requirements_manifest.json`（真实源）并呈现到`quality/REQUIREMENTS.md`。对于每个需求，记录`tier`（1-5 /schemas.md§3.1）和`tier ∈ {1, 2}`-`bin/reference_docs_ingest`调用`bin/citation_verifier`/schemas.md§5.4 /§5.5产生的`citation`块。LLM不会直接支付给`citation_verifier`；摘录是摄取管道的产物，并在gate时由`quality_gate.py`重新验证。对于第3层req（代码即规范），请在`description`中引用源代码`file:line`；引用仅用于FORMAL_DOC参考，不得出现在Tier3/4/5req上。tier + citation pa它在跟踪链中创建了前向链接：reference_docs/cite→需求→bug→测试。请参阅本步骤后面的tier/citation帧块，了解完整字段列表和tier -1- over- tier -2规则。**可选REQs中的`Pattern:`字段。**需要第三阶段的需求
补偿网格应该声明它的模式类：

-`Pattern: whitelist`-权威项目列表，每个网站必须处理     each one.
-`Pattern: parity`-必须匹配的对称操作     (encode↔decode, setup↔teardown).
-`Pattern: compensation`-必须补偿共享间隙的站点。

缺少字段意味着没有网格。设置无效值失败`quality_gate.py`。

**保存规则（第二阶段）**而`Pattern:`在设计中是可选的
（一些req是单站点的，不需要网格），它是必需的
在第一阶段假设已经成立的情况下保持不变。第二阶段必须
将`Pattern:`从EXPLORATION.md转录为`quality/REQUIREMENTS.md`和`quality/requirements_manifest.json`。沉默的遗漏是一种
文档版本1.4.5-回归向量-第5阶段基数门不能
强制覆盖它不知道是模式标记的REQ。门的
结构支持（C13.7/Fix2）交叉检查携带每个站点UC的req
引用（由阶段1的笛卡尔UC规则发出的`UC-N.a`/`UC-N.b`形式）
如果在这样的REQ上缺少模式，则使门失败。**代码存在声明的主源提取规则。当编写断言特定常量、值或标签由特定函数处理的需求时（例如，“白名单必须保留X、Y和Z”），需求必须区分规范中所说的应该存在的内容和代码中实际包含的内容。从代码中提取实际内容（大小写标签、映射键、if-else分支），并与规范的列表进行比较。如果一个常量出现在规范中，但没有出现在代码中，则将需求写为“必须处理X - **[NOT in code]**：在header.h:NN中定义，但在file.c:NN-NN的function（）中不存在。”在没有验证X确实被保存之前，不要写“必须保存X”。这可以防止出现一个污染链，其中需求断言代码存在，代码审查复制断言，规范审计继承它，并且分类接受它——所有这些没有人阅读实际的代码。在v1.3.17版本测试中观察到了这个确切的链：REQUIREMENTS.md断言RING_RESET保留在一个开关中，代码审查复制了列表，三个规范审计员继承了声明，并且没有检测到错误。
**调度功能的机械验证工件（必选）。**当合约断言一个函数处理、保留或调度一组命名常量（特征位、枚举值、操作码表、事件类型、处理程序注册表）时，您必须生成并执行shell命令或脚本，在编写合约行**之前，从函数体**中机械地提取实际情况labels/branches。将原始输出保存到`quality/mechanical/<function>_cases.txt`。该命令必须是一个非交互式管道（例如，`awk`+`grep`），它不能产生幻觉-它读取文件字节并打印匹配项。例子:   ```bash
   awk '/void vring_transport_features/,/^}$/' drivers/virtio/virtio_ring.c \
     | grep -E '^\s*case\s+' > quality/mechanical/vring_transport_features_cases.txt
   ```
执行后，读取输出文件，并将其作为函数处理内容的唯一真实来源。除非`quality/mechanical/<function>_cases.txt`包含匹配的`case X:`行，否则声明“函数保留常数X”的合同行是**禁止的**。如果常量出现在规范或头文件中，但没有出现在机械输出中，则契约必须将其记录为缺席：`"must handle X — **[NOT IN CODE]**: defined in header.h:NN but absent from function() per mechanical check."`下游工件（`REQUIREMENTS.md`,`RUN_SPEC_AUDIT.md`，代码审查）在引用调度函数覆盖时必须引用机械文件路径-它们可能不会用手写的列表替换机械输出。

**机械工件完整性检查（强制性）。**对于每个机械提取命令，也将其附加到`quality/mechanical/verify.sh`作为验证步骤。脚本必须重新运行相同的提取管道，并将结果与保存的文件进行比较。用下面的结构生成`verify.sh`：   ```bash
   #!/bin/bash
   # Auto-generated: re-run mechanical extraction commands and verify saved artifacts
   set -euo pipefail
   FAIL=0
   
   # Verify <function>
   ACTUAL=$(awk '/void vring_transport_features/,/^}$/' drivers/virtio/virtio_ring.c | grep -nE '^\s*case\s+')
   SAVED=$(cat quality/mechanical/vring_transport_features_cases.txt)
   if [ "$ACTUAL" != "$SAVED" ]; then
     echo "MISMATCH: vring_transport_features_cases.txt"
     diff <(echo "$ACTUAL") <(echo "$SAVED") || true
     FAIL=1
   else
     echo "OK: vring_transport_features_cases.txt"
   fi
   
   exit $FAIL
   ```
**阶段6必须执行`bash quality/mechanical/verify.sh`**，如果任何工件不匹配，基准测试将失败。这捕获了在v1.3.19中观察到的失败模式：模型执行提取命令，但将自己期望的输出写入文件，而不是让shell重定向捕获它，插入实际命令不会产生的幻觉`case VIRTIO_F_RING_RESET:`行。在单独的步骤中重新运行相同的命令并对文件进行区分，以检测这种篡改。**即时完整性门（强制性，阶段2a）。**在编写每个`*_cases.txt`文件后**立即**运行`bash quality/mechanical/verify.sh`**，在**编写引用提取的任何契约、需求或散文工件之前**运行`bash quality/mechanical/verify.sh`**。如果退出代码≠0：停止，删除失败的`*_cases.txt`，用新的shell重定向重新运行提取命令（不要手工编辑输出），并重新验证。在verify.sh退出0之前，不要进入阶段3/2c。将verify.sh标准输出和退出代码保存到`quality/results/mechanical-verify.log`和`quality/results/mechanical-verify.exit`，作为持久收据文件。这个门的存在是因为v1.3.23表明将验证推迟到阶段6允许下游工件（CONTRACTS.md、REQUIREMENTS.md、分流探测）构建在伪造的提取上——模型通过破坏工件而不是纠正需求来调和需求和工件之间的差异。**机械制品在提取后是不可变的。**一旦`*_cases.txt`文件被shell重定向写入并被`verify.sh`验证，那么在运行的剩余时间里，它就不能被修改、覆盖或重新生成。如果下游步骤发现机械工件与需求或契约之间存在差异，则需求或契约是错误的——而不是工件。修复散文，而不是摘录。该规则防止了v1.3.23版本的失败模式，即模型用虚构的内容覆盖了正确的提取，以匹配自己的叙述。**禁止探查模式（分诊和验证）。**分类探测、验证探测和审计断言不能使用`open('quality/mechanical/...')`或`cat quality/mechanical/...`作为源文件在给定行中包含内容的唯一证据。为了验证函数F在第N行处理常量C，探测器必须：(a)直接读取源文件（使用行锚定断言的`open('drivers/virtio/virtio_ring.c')`），或者(b)重新执行`verify.sh`使用的相同提取管道并检查其输出。读取已保存的工件只能证明工件所说的内容，而不能证明代码所说的内容—这是循环验证。在v1.3.23中，Probe C验证了伪造的工件而不是源代码，并传递了伪造的数据。**不要创建一个空的机械/目录。**只有当项目的合同包含调度函数、注册表或需要机械提取的枚举检查时，才创建`quality/mechanical/`。如果不存在这样的契约，则完全跳过该目录，并在PROGRESS.md中记录：`Mechanical verification: NOT APPLICABLE — no dispatch/registry/enumeration contracts in scope.`创建一个空的机械/目录（或没有verify.sh的目录）是不符合标准的—它表示尝试提取并放弃了提取。在创建目录之前确定：这个项目是否有分派函数契约？如果没有，请不要`mkdir`。如果是，请将其填充满。**规范性与描述性的分裂。需求和契约必须使用规范的语言（“必须保留”，“应该处理”）来描述预期的行为。当机械验证工件确认声明时，他们可能只使用描述性语言（“保存”、“处理”）。如果一个需求说“实现保留了VIRTIO_F_RING_RESET”，而没有一个确认的机械工件是不符合的——写“实现**必须**保留VIRTIO_F_RING_RESET”，并引用机械检查结果来显示常量当前是否存在。3. **阶段C -覆盖验证。**根据每个要求对每个合同进行交叉参考。修复漏洞。循环最多3次，直到覆盖率达到100%。写到`quality/COVERAGE_MATRIX.md`。矩阵必须有**一行每个需求** （REQ-001， REQ-002等）-不分组范围，如“C-001到C-007 | REQ-001， REQ-003”。分组范围使机器验证不可能，并隐藏了差距。
4. **阶段D -完整性检查+自优化循环。**应用域检查表、可测试性审计和跨需求一致性检查。还要验证在覆盖承诺表中带有“将覆盖”承诺的每个深层文档至少有一个需求跟踪到它——如果没有，在继续之前为缺口添加需求。写入`quality/COMPLETENESS_REPORT.md`作为一个**基线**完整性报告（没有`## Verdict`部分—结论被延迟到调和后的第5阶段，该阶段产生的唯一结论算为结束）。然后运行3次自我改进迭代：阅读报告，修复漏洞，重新检查。当每次迭代少于3次更改时短路。
5. 阶段E -叙述通过。**添加项目概述（带有概述验证门），然后派生用例（带有用例派生门）。在进行分类叙述、横切关注点和最后的重新排序之前，这两扇门都必须通过。这种排序防止多通循环，其中失败的后期门强制重新推导。重新排序自顶向下的流程。重编号顺序。**REQUIREMENTS.md必须以一个人类可读的概述**开始，回答：这个项目是什么？它是做什么的？谁是参与者（用户、系统、硬件、协议）？哪些是风险最高的地区？这个概述对于以前从未见过这个项目的人应该很有用。如果项目是一个库或驱动程序，其中所有参与者都是系统，则描述系统参与者（内核维护者、协议同行、集成商、最终用户开发人员）及其交互。不要以原始的作用域元数据或HTML注释开始-以简单的语言描述开头。

**概述验证门（必选）。**撰写概述后，在进行用例派生之前执行以下自检：>这个概述是否以实际用户的方式描述了这个项目？具体:
> -它是否命名了项目的生态系统角色和现实意义？
b> -它是否确定谁依赖它，为了什么？
> -每天使用这个项目的开发人员会说“是的，这就是它，为什么它很重要”吗？
> -对于知名的项目，它是否反映了公开的采用（例如，Cobra→kubectl/Hugo/GitHubCLI； Express→数百万Node.jsAPI服务器；Zod→formvalidation/tRPC； Serde→默认的Rust序列化层）？

如果概述读起来像是由只阅读源代码而从未使用过该软件的人编写的，那么在继续之前修改它。概述为下游的所有内容设置了框架——面向功能的用例和内部关注的需求是概述只描述代码而不描述项目的症状。**用例派生（必须的，在概述门之后运行）。**从经过验证的概述和收集的文档中得出5-7个用例，然后根据代码验证它们。每个用例必须：

-描述真实的用户结果，而不是代码特性。“开发人员构建一个带有嵌套子命令、持久标志和shell完成的CLI工具”——而不是“框架支持命令树”。
-说出一个具体的参与者，以及他们正在努力完成的目标。参与者包括终端用户开发人员、系统管理员、内核维护者、协议对等体、集成商和自动化使用者。
-能够被软件的实际用户**识别。对于知名项目，根据模型自己对项目、社区文档、教程和实际采用模式的了解来验证用例。
-通过可测试的满足条件连接到至少一个需求。管道应该明确地问：“基于这个项目的概述、收集的文档和已知的用户群，真正的用户对这个软件做的最重要的5-7件事是什么？”从这个问题中获得用例——而不是通过扫描代码和将特性分组到类别中。

**用例对代码的验证：**从概述和文档中获得用例后，根据代码库验证每个用例。如果用例描述了代码实际上不支持的东西，那么修改或删除它。如果代码支持没有用例覆盖的重要用户结果，则添加一个。目标是用户可识别且基于代码的用例。验收标准范围检查（强制性的，在用例派生之后运行）。**在用例最终确定并针对代码进行验证之后，检查所有需求的满足条件是否总体上跨越了项目的主要行为：

这些验收标准合在一起是否涵盖了整个项目？是否在概述或用例中描述了一个主要的面向用户的行为，如果它崩溃了，需求的满足条件将无法捕捉到？对于每个用例，至少一个需求的满足条件必须是可追溯的，并且至少一个链接的需求必须是`specific`（而不是`architectural-guidance`）。没有关联特定需求的用例表明存在差距。当发现差距时，可以：(a)添加新的需求或强化现有的条件来覆盖差距，或者(b)修改用例，如果它没有反映需求实际保护的内容。将检查结果记录在完整性报告中。

遵循带有单个需求的用例。

**v1.5.3层和引用方案（schemas.md§3.1，§5）。**每个REQ携带一个`tier`整数1-5 /`schemas.md`§3.1：- **一级** -项目自己的正式规范（`FORMAL_DOC`记录与`tier=1`；最高权威）。
- **Tier 2** -外部正式标准（RFC, W3C， ISO，发布的API合同-`FORMAL_DOC`记录与`tier=2`）。
- **第3层-在没有正式规范的情况下使用真实源代码；代码就是规范。
**第4层** -由`bin/reference_docs_ingest.load_tier4_context`从顶层`reference_docs/`加载的非正式文档（AI聊天，设计笔记，回顾）。
-第5层-从代码行为推断，没有文档支持。对于`tier ∈ {1, 2}`， REQ还携带一个`citation`块，每个`schemas.md`§5包含`document`，`document_sha256`，`section`/`line`中的至少一个，以及一个机械提取的`citation_excerpt`。不要手写节选。该摘录是由`bin/reference_docs_ingest`根据`schemas.md`§5.4中的确定性算法调用`bin/citation_verifier`在摄取时产生的（章节分辨率根据§5.5）- LLM从`formal_docs_manifest.json`中消耗摘录；它永远不会直接发送给验证者。摄取时间提取是幻觉之门第一层的工作原理。如果不能引用`quality/formal_docs_manifest.json`中的文档（使用散列和定位器），则REQ最多为第3层。`page`-only定位器仅用于诊断，而且永远不够。* * Tier-1-wins-over-Tier-2规则。**当项目自己的规范（第1层）和外部标准（第2层）相互矛盾时，记录在第1层引用项目立场的REQ。项目对外部标准的文档化偏离是权威的意图，而不是缺陷——`upstream-spec-issue`处理只适用于项目规范对冲突保持沉默的情况。**规格间隙退化（有效输出状态）。**如果`formal_docs_manifest.json`包含零`FORMAL_DOC`记录，覆盖项目自己的行为，每个REQ在3/4/5层结束，并且运行优雅地降级为Spec Gap Analyzer。在完整性报告中将元发现“0层1/2需求”报告为一个度量，而不是一个失败。不要捏造引用来使层分布看起来更丰富——`quality_gate.py`在验证时根据§5.4重新调用`bin/citation_verifier`（通过`extract_excerpt`），并拒绝任何`citation_excerpt`不等于新鲜提取（schemas.md§10不变量#11）的1/2REQ层。**`functional_section`为必填字段。**每个REQ都带有一个简短的`functional_section`字符串（例如，`"Authentication"`,`"Bus enumeration"`），用于分组相关的REQ。这是llm，源自代码和文档；没有预定义的本体。第二阶段的渲染将这些章节下的req分组（每个章节有一个简短的介绍段落），第四阶段的理事会将审查分组的一致性。参见`schemas.md`§6.1。

可追溯性是单向的：REQ→UC。** REQ携带一个`use_cases[]`的UC-NN id列表。UC记录不携带`requirements[]`反向链接——相反的方向是在呈现时通过查询REQ记录来匹配条目（schemas.md§7）。不要在UC记录上填充`requirements[]`字段。

**对于每个要求，提供所有这些字段—**ID**:`REQ-NNN`（填充零的三位数序列）。
—**Title**：简短的一行语句。
- **层**：整数1-5每schemas.md§3.1。
- **函数段**：短llm派生字符串（见上文）。
- **引用**（当`tier ∈ {1, 2}`时需要）：由`bin/reference_docs_ingest`调用`bin/citation_verifier`产生；从未手工编写，也从未由LLM直接调用。形状符合schemas.md§5.1。
- **摘要/描述**：将需求描述为可测试的断言：“X必须满足Y”或“当a时，系统必须B”。
- **用户故事**：从调用者的角度构建：“作为一个[角色]做[行动]，我期望[行为]**，以便**[结果]。”“so that”从句是强制性的——它迫使您阐明需求背后的意图。
实现说明：代码是如何实现这个需求的——机制，相关的代码路径，设计选择。
- **满足条件**：具体，测试证明满足此需求的Ble场景。包括理想路径、边缘情况和失败模式。阶段A中的每个单独的合同都被分组到这个需求中，成为满足的条件。
- **可选路径**：必须满足需求的多个代码路径、模式或入口点。替代路径是bug隐藏的地方。
- **用例**:`use_cases[]`-该REQ参与的`UC-NN`id列表。单向转发链路。
- **参考文献**：引用源规范部分，ChangeLog条目，配置字段定义，源评论，issue号或领域知识。对于层1/2REQs，`citation`块携带权威定位器；自由格式的引用是补充的。
- **专用性**:**专用性**（可测试的）-必须具有满足条件，代码审查者可以根据特定的代码位置或行为进行检查；这是默认值并将其计算到覆盖指标中)或**架构指导**（不能针对单个代码路径进行测试——涵盖了诸如“保持轻量级和标准库兼容”或“no_std支持”之类的横切属性；告知质量构成，但不计算在覆盖指标中；大多数项目应该有0-3个架构指导需求——超过3个会触发下面的强制性自检）。“定向”这一类别已被淘汰。任何“指向性”的需求都必须明确（带有可测试的条件），或者明确地归类为架构指导。**架构指导自检（强制性的，在需求派生之后运行）。**计算标记为`architectural-guidance`的需求。应用两个边界：—**最大绑定（>3）：**如果计数超过3，停止并重新检查每一个。对于每一种情况，都要问：“我是否可以添加一个可测试的满意条件，以便代码审查者可以根据特定的代码位置进行验证？”是，重新分类为`specific`，并添加条件。只有那些确实无法针对任何特定代码路径进行验证的需求才应该保持`architectural-guidance`。如果最终计数超过3个，则需要对每个超额要求提供明确的理由，解释为什么不能具体说明。
- **最小边界（0对15+需求）：**如果总需求计数为15或更多，`architectural-guidance`计数为0，重新检查横切设计不变量的需求。跨协议层、管理资源生命周期、强制排序保证或维护兼容性契约（例如，“保持stdlib兼容”、“保持no_std支持”、“维护有线格式”）的库向后兼容性”)通常有1-3个架构指导需求。在完整性报告中用一句话解释为什么没有需求符合体系结构指导，或者重新分类适当的需求。在完整性报告中记录计数和任何重新分类。

**不要限制需求计数。**根据项目需要获取尽可能多的数据。一个小型实用程序可能有20个。一个成熟的库可能有100多个。目标是完整性。

步骤7a：文档到需求的协调

从PROGRESS.md重新阅读覆盖率承诺表。对于您承诺覆盖的每个深层文档（“将在阶段2中覆盖”），验证至少有一个需求可以追溯到它所记录的子系统。如果您的需求只覆盖了一些已提交的子系统，那么在完成步骤7之前为这些缺口添加需求。

对于每个子系统，在PROGRESS.md中记录以下内容之一：
-覆盖它的需求id，或者
明确排除，包括理由、风险确认和建议随访具有“将覆盖”承诺和零映射需求的深度文档子系统是一个过程失败，而不是一个合法的范围选择。在每个承诺都得到满足或显式地转换为合理的排除之前，不要继续生成工件。

**步骤7b：代码路径→REQ反向可追溯性审核（强制性）**

时间：在阶段E完成后执行步骤7a和7b（即，在概述验证门、用例派生和验收标准范围检查全部运行之后）。审计依赖于最终的需求和最终的用例。在需求派生完成之后，运行反向跟踪审计。前向跟踪（收集文档→需求→bug→测试）已经内置到管道中。这一步在代码路径粒度上检查相反的方向：重要的代码路径是否映射回需求条件？这是审计活动，而不是结构性的双向链接。（v1.5.3中的结构可追溯性是`schemas.md`§7中的单向REQ→UC，并由模式强制执行；这种审计根据REQ检查代码覆盖率，这是一个单独的关注点。）

这在**path/branch/helper粒度**上运行，而不是文件级别。在v1.3.13中，文件级别的覆盖率是100%，但仍然遗漏了两个真正的bug。问题不是“这个文件是否映射到某些需求？”而是“这个重要的分支是否映射到一个需求子句，该子句陈述了这里必须保留的内容？”**范围为四个类别**（不是开放式分支审计）：

1. **需求中已经命名的备选路径。如果一个需求提到了回退或备选路径（例如，“主模式vs降级模式”，“协商模式vs默认配置”，“同步模式vs异步模式”），每个备选必须有一个显式的对称条件——一个声明在两条路径上必须保持什么不变式。一个需求说“系统处理X和Y”，而没有指定“处理”对每个“处理”意味着什么，这是不完整的。

2. **将公共常量转换为运行时行为的帮助程序。**如果helper函数在定义的常量和运行时行为（例如，特性标志门，编解码器注册表查找，功能白名单helper）之间进行白名单，过滤器或转换，它必须有一个特定于helper的需求，枚举期望的preserved/translated值。3. **能力-协商和回退逻辑。**系统与外部对等体协商功能（协议版本协商、特征检测、优雅降级）的代码路径必须具有涵盖协商的向上和向下路径的需求。

4. **先前BUGS.md，VERSION_HISTORY.md或spec审计输出中命名的函数。**如果之前的运行在一个特定的函数中发现了一个bug，以后的运行必须显示明确的重新检查该函数的证据（“已知的bug类哨兵”）。这可以防止“丢失的需求”回归类。如果在`quality/spec_audits/`中存在先前的规范审计输出，请在运行哨兵检查之前阅读它们—来自委员会审查的跨模型发现是已知缺陷表面的高价值来源。对于每个类别，检查需求中是否包含针对已识别路径的具体条件。孤立路径——没有需求覆盖的重要代码路径——会在完整性报告中触发一个“覆盖缺口”标记。在完整性报告能够声明需求充足之前，必须解决这些差距（通过添加需求条件或提供明确的证明）。

**结转规则：**当先前运行的REQUIREMENTS.md存在于质量目录中时，管道必须读取它并检查是否删除了先前版本的任何条件。如果条件被删除，管道必须：(a)用更新的理由重新派生它们，或者(b)记录为什么条件不再相关。无声滴是不允许的——它们是导致回归的直接原因，在这种情况下，之前学习到的需求会在运行之间丢失。**管道后：**阶段7可以生成`quality/REVIEW_REQUIREMENTS.md`（交互评审协议）和`quality/REFINE_REQUIREMENTS.md`（细化通过协议）。这些不是阶段2的工件——它们支持阶段7的交互式改进路径。用户可以交互式地审查需求，使用不同的模型运行细化过程，并保留每个迭代的版本备份。有关完整的版本控制协议和备份结构，请参见`references/requirements_pipeline.md`。

用结构化的格式记录所有的需求。这些直接提供给代码审查协议的验证和一致性通过。

检查点：在阶段1之后更新PROGRESS.md**v1.5.6更新-PROGRESS.md现在在运行启动时初始化，而不是在阶段1之后。**根据本文件前面的“运行状态检测”一节，在任何阶段工作开始之前编写`quality/PROGRESS.md`和`quality/run_state.jsonl`。到阶段1的这个时候，两个文件都已经存在了。这个检查点是对PROGRESS.md的第1阶段完成更新，而不是初始化。PROGRESS.md格式结合了运行状态标头（已启动/基准测试/杠杆/运行者/ Playbook版本）、阶段检查表（现在由`quality/run_state.jsonl`中的`phase_start`/`phase_end`事件驱动）和下面的遗留内容部分（运行元数据、工件库存、累积错误跟踪器等）——它们是互补的，而不是竞争的。**第一阶段完成行动：**在阶段检查表中将第一阶段标记为`[x]`，并提供汇总统计（发现计数，走过的模式）；将阶段1工件（EXPLORATION.md和任何子工件）添加到工件库存中。根据运行状态检测一节中的交叉验证规则，将`phase_end phase=1`事件附加到`run_state.jsonl`。

**PROGRESS.md存在的原因：**在单会话运行时，代理将上下文保存在内存中。但是上下文在长时间的会话中会退化——阶段1的发现会被阶段6遗忘，BUG计数会漂移，规范审计的BUG会孤立，因为闭包检查从来没有看到它们。PROGRESS.md通过让每个阶段将其状态写入磁盘来解决这个问题。代理在每个阶段之前都会读回它，所以它总是对到目前为止发生的事情有一个准确的了解。作为附带的好处，它使技能能够正确地工作，即使在多个回合中运行。**长期运行的检查点原则：**在每个需求管道阶段（合同、需求、覆盖矩阵、完整性、叙述）之后，用：完成的阶段、工件路径、当前范围的子系统、剩余工作和精确的恢复点来更新`quality/PROGRESS.md`。这可确保恢复的会话可以从上次完成的检查点继续，而无需重做工作。对于v1.5.6，还将相应的`pass_started`/`pass_ended`事件附加到`run_state.jsonl`。**时间戳原则：**在开始下一个阶段之前，在完成一个阶段后立即将每个阶段完成条目写入PROGRESS.md。不要在事后批量写入或回填时间戳。时间戳是审计跟踪——如果阶段2显示的完成时间早于阶段1，则审阅者无法验证阶段是否按正确的顺序运行。如果您意识到忘记编写检查点，那么现在就写入检查点，并附上时间戳和解释间隔的注释。

v1.5.6初始化文件的完整PROGRESS.md格式将在阶段1完成时填充，包括以下部分（遗留模板，保留，因为阶段5+依赖于其累积错误跟踪器和终端门验证部分）：```markdown
# Quality Playbook Progress

## Run metadata
Started: [date/time]
Project: [project name]
Skill version: [read from SKILL.md metadata using the reference file resolution order — must match exactly]
With docs: [yes/no]

## Phase completion
- [x] Phase 1: Exploration — completed [date/time]
- [ ] Phase 2: Artifact generation (QUALITY.md, REQUIREMENTS.md, tests, protocols, RUN_TDD_TESTS.md) — `AGENTS.md` is generated by the orchestrator after Phase 6, not here
- [ ] Phase 3: Code review + regression tests
- [ ] Phase 4: Spec audit + triage
- [ ] Phase 5: Post-review reconciliation + closure verification
- [ ] TDD logs: red-phase log for every confirmed bug, green-phase log for every bug with fix patch
- [ ] Phase 6: Verification benchmarks
- [ ] Phase 7: Present, Explore, Improve (interactive)

## Artifact inventory
| Artifact | Status | Path | Notes |
|----------|--------|------|-------|
| QUALITY.md | pending | | |
| REQUIREMENTS.md | pending | | |
| CONTRACTS.md | pending | | |
| COVERAGE_MATRIX.md | pending | | |
| COMPLETENESS_REPORT.md | pending | | |
| Functional tests | pending | | |
| RUN_CODE_REVIEW.md | pending | | |
| RUN_INTEGRATION_TESTS.md | pending | | |
| BUGS.md | pending | | |
| RUN_TDD_TESTS.md | pending | | |
| RUN_SPEC_AUDIT.md | pending | | |
| tdd-results.json | pending | quality/results/ | Structured TDD output |
| integration-results.json | pending | quality/results/ | Structured integration output |
| Bug writeups | pending | quality/writeups/ | One per TDD-verified bug |

## Cumulative BUG tracker
<!-- Every confirmed BUG from code review and spec audit goes here.
     Each entry tracks closure status: regression test reference or explicit exemption.
     The closure verification step reads this list to ensure nothing is orphaned. -->

| # | Source | File:Line | Description | Severity | Closure Status | Test/Exemption |
|---|--------|-----------|-------------|----------|----------------|----------------|
<!-- Closure Status values:
     - "confirmed open (xfail)" — bug exists, regression test confirms it, fix pending
       Language equivalents: Python "xfail", TypeScript/JS "test.fails", Go "t.Skip",
       Java "@Disabled", Rust "compile_fail" (for compile-time bugs). Use the
       language-appropriate term in parentheses, e.g. "confirmed open (@Disabled)"
     - "TDD verified (FAIL→PASS)" — full red-green cycle: test fails on unpatched, passes after fix patch
     - "fixed (test passes)" — bug fixed, regression test now passes, xfail marker removed
     - "exempt (reason)" — no regression test possible, reason documented -->


## Terminal Gate Verification
<!-- Filled in during Phase 5. Must match BUG tracker counts exactly. -->

## Exploration summary
[Brief notes on architecture, key modules, spec sources, defensive patterns found]
```
在每个阶段之后更新此文件。累积BUG跟踪器是最重要的部分——它确保无论哪个阶段产生的发现都不会孤立。

将探测结果写入磁盘

初始化PROGRESS.md之后，将完整的探索结果写入`quality/EXPLORATION.md`。该文件捕获您在阶段1中学到的所有内容，因此它可以在上下文边界（会话中断、多通道切换或长期运行的内存退化）下保存下来。将其构建为：```markdown
# Exploration Findings

## Domain and Stack
[Language, framework, build system, deployment target]

## Architecture
[Key modules with file paths, entry points, data flow, layering]

## Existing Tests
[Test framework, test count, coverage areas, gaps]

## Specifications
[What reference_docs/ contains, key spec sections, behavioral rules]

## Open Exploration Findings
[At least 8 concrete findings from domain-driven investigation.
Each must have a file path, line number, and specific bug hypothesis.
At least 4 must reference different modules or subsystems.
At least 3 must trace a behavior across 2+ functions.]

## Quality Risks
[At least 5 domain-driven failure scenarios ranked by priority.
Each must name a specific function, file, and line and explain the failure
mechanism using domain knowledge of what goes wrong in systems like this.
These are hypotheses, not confirmed bugs — they tell Phase 2 where to look.
Frame each as: "Because [code at file:line] does [X], a [domain-specific
edge case] will produce [wrong behavior] instead of [correct behavior]."
A section that lists defensive patterns the code already has does NOT belong here.]

## Skeletons and Dispatch
[State machines, dispatch tables, feature registries — with file:line citations]

## Pattern Applicability Matrix
| Pattern | Decision (`FULL` / `SKIP`) | Target modules | Why |
|---|---|---|---|
| Fallback and Degradation Path Parity | | | |
| Dispatcher Return-Value Correctness | | | |
| Cross-Implementation Consistency | | | |
| Enumeration and Representation Completeness | | | |
| API Surface Consistency | | | |
| Spec-Structured Parsing Fidelity | | | |

[3 to 4 patterns must be marked FULL. The rest are SKIP with codebase-specific rationale. Select 4 when a fourth pattern clearly applies and covers different code areas.]

## Pattern Deep Dive — [Pattern Name]
[Use the output format from `exploration_patterns.md`.
Trace the relevant code path across 2+ functions, implementations, or API surfaces.
Each deep dive should pressure-test, refine, or extend findings from the open
exploration and quality risks stages.]

## Pattern Deep Dive — [Pattern Name]
[Repeat for each selected FULL pattern. 3 to 4 deep-dive sections total.]

## Pattern Deep Dive — [Pattern Name]
[Third and final deep dive.]

## Candidate Bugs for Phase 2
[Consolidated from ALL earlier sections — open exploration, quality risks, AND patterns.
Minimum 4 candidates with file:line references. At least 2 from open exploration or
quality risks, at least 1 from a pattern deep dive. For each candidate include the
source stage and what the Phase 2 code review should inspect.]

## Derived Requirements
[REQ-001 through REQ-NNN, each with spec basis and tier]

## Derived Use Cases
[UC-01 through UC-NN, each with actor, trigger, expected outcome]

## Notes for Artifact Generation
[Anything the next phase needs to know — naming conventions, test patterns, framework quirks]

## Gate Self-Check
[Written by the Phase 1 gate. Each check 1–12 with PASS/FAIL and one-line evidence.
This section proves the gate was executed. Do not write this section until you have
actually verified each check against the file contents.]
```
**最小深度期望：**EXPLORATION.md必须包含至少120行实质性内容-不是填充或模板头，而是实际发现（文件路径，行为规则，派生需求，架构观察）。列出带有一行占位符的节头的骨架不是有效的移交工件。如果文件比这个薄，返回并添加阶段2需要的细节。

**写完后重读（强制性）。**在写入EXPLORATION.md之后，在继续到阶段2之前显式地从磁盘读取文件。这有两个目的：(1)它确认文件被正确地写入，(2)它将结构化的发现加载到生成工件的工作内存中。不要跳过这一步，依靠你记得写过的东西——“写然后读”的循环是上下文桥梁。该文件在所有模式下都是必不可少的。在单遍模式中，它迫使模型在生成工件之前阐明特定的发现（文件路径、函数名、行号）。在多通道模式下，它也是通道之间的切换工件。无论哪种方式，写-读周期都是勘探深度的质量把关。

**阶段1完成门（强制性-在阶段2之前停止在这里）。**您必须在进入阶段2之前执行此门。这不是可选的。从磁盘重新读取`quality/EXPLORATION.md`，并运行下面的每个检查。检查之后，将`## Gate Self-Check`部分附加到EXPLORATION.md的底部，其中列出每个检查号（1-12），带有PASS或FAIL和一行证据注释。如果任何检查失败，修复EXPLORATION.md并重新运行门。在所有检查通过并且门自检部分被写入磁盘之前，不要进入阶段2。**在v1.3.43基准测试中，两个repos （chi, zod）产生的EXPLORATION.md文件具有完全错误的部分结构-像“架构摘要”，“行为契约”，“存储库和架构映射”这样的部分而不是所需的部分。该模型从不运行门检查，直接进入阶段2，产生零错误。如果您的EXPLORATION.md不包含下面列出的EXACT标题的部分，则它不符合标准，必须在继续之前重写。1. 该文件存在于磁盘上，并且包含至少120行实质性内容。
2.`quality/PROGRESS.md`存在并标志着阶段1完成。
3. 派生需求部分包含至少一个具有特定文件路径和函数名的REQ-NNN，而不是抽象的子系统描述。
4. 存在一个标题为**exactly**`## Open Exploration Findings`的部分，其中包含至少8个具体的错误假设或可疑发现，每个都有文件路径和行号。这些必须来自领域驱动的调查，而不仅仅来自应用模式。至少4个必须引用不同的模块或子系统。
5. **至少3个发现在`## Open Exploration Findings`必须追踪一个行为跨越2个或更多的功能或2个具体的代码位置。一份孤立的单一功能怀疑清单是不够深入的。
6. 存在一个标题为**exactly**`## Quality Risks`的章节，其中包含至少5个域驱动的故障场景按优先级排序。每个场景必须：(a)命名一个特定的函数、文件和行，(b)描述一个特定领域的边缘情况或故障模式，以及(c)解释为什么代码会产生错误的行为。这些必须来自于对系统中出现问题的领域知识，而不仅仅是对代码的结构分析。列出代码已经具有的防御模式（代码正确执行的事情）的部分不能满足这一要求。没有具体故障场景的高危模块列表部分不满足此门要求。结论库是成熟的，不太可能有基本错误的部分不满足这一要求。
7. 有一个名为**exactly**`## Pattern Applicability Matrix`的部分，它评估了`exploration_patterns.md`的所有六个模式，用目标模块和特定于代码库的基本原理将每个模式标记为`FULL`或`SKIP`。
8. 在3到4个图案（包括）之间标记为`FULL`在适用性矩阵中。
9. 有3到4个章节（包括），它们的标题以`## Pattern Deep Dive — `开头。每个都必须包含具体的文件行证据，而不仅仅是模式名占位符。计数必须与矩阵中`FULL`模式的数量匹配。
10. **模式深度检查：**至少有2个模式深度部分必须在2个或更多函数之间跟踪代码路径。一个说“函数X在文件：行有一个间隙”的部分是一个表面发现。一节说“函数X在file:line调用函数Y在file:line，它执行A但不执行B；与函数Z（两者都做）相比，是深度查找。
11. 存在一个标题为** ** **`## Candidate Bugs for Phase 2`的部分，并且包含至少4个按优先顺序排列的错误假设和文件：行引用、每个假设出现的阶段（开放探索、质量风险或模式），以及代码审查应该寻找的内容。
12. * *整体平衡检查：**至少有2个候选bug必须源于开放探索或质量风险，并且至少有1个必须源于模式深度挖掘或通过模式深度挖掘得到实质性加强。这确保了领域知识和结构分析结果都能进入阶段2。在所有12个检查都通过并且`## Gate Self-Check`段被写入磁盘上的EXPLORATION.md之前，不要开始阶段2。阶段1是您深入理解代码库的唯一机会。在这里错过的每一个需求都是在阶段3中找不到的缺陷。投入时间。

**如果你发现自己要开始第2阶段而没有编写门自检部分，请停止。回去把门打开。这条指令之所以存在，是因为当模型对自己的探索有信心时，它们就会跳过这扇门——而这种信心恰恰是在遗漏错误的时候。

**阶段结束消息（强制性-在阶段1完成后打印此消息，然后停止）：**```
# Phase 1 Complete — Exploration

I've finished exploring the codebase and written my findings to `quality/EXPLORATION.md`.
[Summarize: how many candidate bugs, which subsystems explored, key risks identified.]

To continue to Phase 2 (Generate quality artifacts), say:

    Run quality playbook phase 2.

Or say "keep going" to continue automatically.
```
**打印此消息后，请停止。除非用户明确要求，否则不要进入第二阶段

---

阶段2：生成质量剧本

**v1.5.6 instrumentation:**现在将`phase_start phase=2`追加到`quality/run_state.jsonl`。在阶段结束时，通过调用`bin/run_state_lib.validate_phase_artifacts(quality_dir, 2)`进行交叉验证——它检查完整的Generate合约（REQUIREMENTS.md、QUALITY.md、CONTRACTS.md、COVERAGE_MATRIX.md、COMPLETENESS_REPORT.md、RUN_CODE_REVIEW.md、RUN_INTEGRATION_TESTS.md、RUN_SPEC_AUDIT.md、RUN_TDD_TESTS.md，加上一个非空的`quality/test_functional.<ext>`）。如果验证通过，则追加`phase_end phase=2`。如果失败，用`recoverable: true`附加一个`error`事件，并重新运行缺失的工件生成。（BUG-014修复：v1.5.6之前版本，此注释引用了从未发布的v1.5.5设计分类模型。）> **本阶段所需的参考资料** -在继续之前阅读这些：
> -`quality/EXPLORATION.md`-你的第一阶段发现（架构、需求、用例、模式分析）
> -`references/requirements_pipeline.md`-需求推导的五相管道
> -`references/defensive_patterns.md`- grep模式查找防御代码
> -`references/schema_mapping.md`-模式感知测试的字段映射格式
> -`references/constitution.md`-QUALITY.md模板
> -`references/functional_tests.md`-测试结构和反模式
代码审查和集成测试模板**第2阶段源代码修改护栏（强制性-硬停止）。**阶段2只写`quality/`。请勿创建、修改或删除目标repo的`quality/`目录之外的任何文件，包括（但不限于）`AGENTS.md`、`CLAUDE.md`、`README.md`、`bin/**`、`.github/**`、`.claude/**`、`agents/**`、`SKILL.md`、`schemas.md`、源代码或测试文件。目标回购的`AGENTS.md`是在阶段6完成后由编排器生成的；如果您在阶段2中编写`AGENTS.md`，您将触发编排器的源不变不变量并中止运行。2026-04-30 codex引导测试正是以这种方式失败的：阶段2 LLM更新了现有的`AGENTS.md`（因为早期的SKILL.md命令它这样做），源未更改的门检测到修改，并且运行在阶段3之前终止。如果生成的工件自然地位于`quality/`之外，则将其写在`quality/`下，并让编排器在完成过程中移动或复制它。唯一允许的异常是`quality/PROGRESS.md`，它本身位于`quality/`中。**第二阶段入口门（强制-硬停止）。**在生成任何工件之前，从磁盘读取`quality/EXPLORATION.md`，并验证以下所有确切的节标题存在（grep或search -不依赖于内存）：1.`quality/EXPLORATION.md`必须至少有120行—较短的文件表示未完成探索
2.`## Open Exploration Findings`-必须逐字存在
3.`## Quality Risks`-必须逐字存在
4.`## Pattern Applicability Matrix`-必须逐字存在
5. 至少有3个以`## Pattern Deep Dive — `-开头的部分必须一字不差地存在
6.`## Candidate Bugs for Phase 2`-必须逐字存在
7.`## Gate Self-Check`-必须存在（证明阶段1门已运行）
8. 存在`quality/PROGRESS.md`，其一期线标记为`[x]`-证明一期已经完成，而不是刚刚开始
9.`## Open Exploration Findings`部分包含至少8个具体的错误假设-用文件行引用计数行数
10. 在`## Open Exploration Findings`跟踪行为中至少发现3个跨2+函数的发现-寻找多位置跟踪
11. 在`## Pattern Applicability Matrix`- count FULL条目中，3到4个模式被标记为`FULL`12. 至少有2个模式深入部分跟踪2+函数之间的代码路径-寻找多功能跟踪
13.`## Candidate Bugs for Phase 2`有2个以上的bugnexploration/risksAND≥1从一个模式深潜-检查源级标签如果文件不存在，少于120行，或者缺少这些确切的部分标题，请停止并返回阶段1。不要试图以不同的名称继续进行“等效”部分-需要上述确切的标题。现在编写EXPLORATION.md，从领域驱动的开放探索开始，然后进行领域知识风险分析，然后从`references/exploration_patterns.md`中选择3-4个模式进行深度挖掘。在EXPLORATION.md通过第一阶段完成门之前，不要进行第二阶段。此检查存在是因为单次通过执行可以跳过阶段1门-这是备份。在v1.3.43中，两个repos绕过了这两个门，并且没有产生任何bug。使用`quality/EXPLORATION.md`作为此阶段的主要源代码—不要从头开始重新探索代码库。探索发现包含驱动下面每个工件的架构图、派生的需求、用例和风险分析。如果您发现自己需要通过阅读源文件来了解项目的功能，那么请回到EXPLORATION.md。重新探索会浪费上下文，并在阶段1的发现和阶段2的生成之间产生不一致。

现在编写阶段2工件。上面的需求管道产生了REQUIREMENTS.md、CONTRACTS.md、COVERAGE_MATRIX.md和COMPLETENESS_REPORT.md。下面的七个文件完成了这个集合。对于每一个，遵循下面的结构，并参考相关的参考文件以获得详细的指导。**版本戳（必须在每个生成的文件上）。**剧本生成的每个Markdown文件必须以以下属性行开始，紧接文件的标题标题：```
> Generated by [Quality Playbook](https://github.com/andrewstellman/quality-playbook) v1.5.3 — Andrew Stellman
> Date: YYYY-MM-DD · Project: <project name>
```
每个生成的代码文件（测试文件，脚本）必须以注释头开始：```
# Generated by Quality Playbook v1.5.6 — https://github.com/andrewstellman/quality-playbook
# Author: Andrew Stellman · Date: YYYY-MM-DD · Project: <project name>
```
使用适合该语言的注释语法（`#`、`//`、`/* */`等）。戳记中的版本必须与该技能前端内容中的`metadata.version`匹配。这个标记使得每个生成的工件都可以追溯到创建它的工具、版本和运行——当文件通过电子邮件发送、附加到票据或在存储库上下文之外进行审查时，这是必不可少的。使用剧本生成开始的日期，而不是每个单独文件被写入的日期。**邮票放置及豁免：**
-对于带有编码pragma （`# -*- coding: utf-8 -*-`）或shebang （`#!/usr/bin/env python`）的Python文件，将戳记注释*放在pragma/shebang之后，而不是之前-将其推过第2行会导致`SyntaxWarning`。
—对于sidecar JSON文件（`tdd-results.json`,`integration-results.json`），`skill_version`字段已经用作版本戳。JSON不支持注释——不要注入注释。
-对于JUnit XML文件，不需要戳记-这些是框架生成的。
-对于`.patch`文件，不要在diff体中注入戳记-它会破坏`git apply`。依赖于周围的工件元数据（BUGS.md,tdd-results.json）来获取出处。**工件依赖规则
-`quality/RUN_CODE_REVIEW.md`Pass 2依赖于一个稳定的`quality/REQUIREMENTS.md`-薄的需求产生薄的Pass 2审查。如果代码表面的需求计数似乎很低（每个核心模块少于3-4个需求），请在第2阶段报告的开始处注意这一点。
-功能测试依赖于`quality/REQUIREMENTS.md`和`quality/QUALITY.md`-在任何需求细化之后，重新验证`test_functional.*`仍然涵盖所有需求。
—`quality/RUN_SPEC_AUDIT.md`取决于需求、质量场景和文档验证。
-`quality/COMPLETENESS_REPORT.md`有两个阶段：基线阶段（预审查，无裁决部分）和最终阶段（第五阶段后和解，有权威裁决）。
-`quality/PROGRESS.md`是权威状态文件，必须在每个下游工件开始之前更新。**为什么只有9个文件而不是测试？测试捕获回归，但不能阻止新类型的bug。质量构成（`QUALITY.md`）在以后的会议开始编写代码之前告诉他们“正确”是什么意思。协议（`RUN_*.md`）为审查、集成测试和产生可重复结果的规范审计提供了结构化的流程——而不是任由AI想要检查的东西来检查质量。这些文件一起创建了一个质量系统，其中每个部分都加强了其他部分：QUALITY.md中的场景映射到功能测试文件中的测试，这些测试由集成协议验证，并由Council of Three审核。

JSON清单规则（在写任何工件之前阅读）阶段2编写每个派生记录的两个并行呈现：**JSON清单**（机器可读，门验证-事实来源）和**Markdown工件**（人类可读，从清单呈现）。只更新其中一个而不更新另一个的阶段脚本是错误的。

舱单在：

-`quality/formal_docs_manifest.json`-由`bin/reference_docs_ingest.py`在第一阶段编写。不要重写。
-`quality/requirements_manifest.json`-权威REQ记录按schemas.md§6。
-`quality/use_cases_manifest.json`-权威UC记录，符合schemas.md§7。
-`quality/bugs_manifest.json`-权威BUG记录每schemas.md§8。在阶段3/4/5确认bug后编写。
-`quality/citation_semantic_check.json`-第4阶段委员会第2层裁决（见下文第4阶段）。

每个清单都遵循§1.6包装器，带有`schema_version`，`generated_at`和一个顶级记录数组。四个记录形状的清单（`formal_docs_manifest.json`、`requirements_manifest.json`、`use_cases_manifest.json`、`bugs_manifest.json`）使用`records`作为数组键：```json
{
  "schema_version": "<from SKILL.md metadata.version>",
  "generated_at": "<ISO 8601 with explicit Z timezone>",
  "records": [ /* per-schema records, per schemas.md §4–§8 */ ]
}
```
**异常-`citation_semantic_check.json`使用`reviews`而不是`records`**。相同的包装形状，不同的数组键；里面的记录是委员会审查条目，不是REQ/UC/BUG记录。如果您发现自己正在将`records`写入`citation_semantic_check.json`，请停止并重新读取schemas.md§9—当键错误时，gate会拒绝此文件，因为违反了清单包装（schemas.md§10不变量#13）。

在生成时，`schema_version`必须等于这个技能的`metadata.version`-从SKILL.md读取，不要硬编码。`generated_at`使用`datetime.now(timezone.utc).isoformat(timespec="seconds").replace("+00:00", "Z")`。记录形状和不变量在`schemas.md`中定义-不要在本技能中重新定义它们。`quality_gate.py`（阶段5/6）根据这些模式逐个字段验证清单。**REQUIREMENTS.md渲染约定。**REQUIREMENTS.md由`functional_section`组织。每个部分都以一个简短的llm写的介绍段落开始，描述该功能区域的功能，然后列出其下的REQ（按REQ id排序）。用例呈现它们的`formal_doc_refs`，但不要列出`requirements[]`——可追溯性是单向的REQ→UC，在呈现时通过查询REQ记录派生出相反的方向。

###文件1:`quality/QUALITY.md`-质量构成

**阅读`references/constitution.md`**获取完整的模板和示例。

宪法有六个部分：1. **目的** -质量对本项目意味着什么，基于Deming（内置，未经检查），Juran（适合使用），Crosby（质量免费）。具体应用这些：“适合使用”对*本系统*意味着什么？不是“测试通过”，而是实际的操作需求。
2. **覆盖目标** -将每个子系统映射到目标的表，并参考实际风险的基本原理。每个目标都必须有一个基于特定场景的“为什么”——如果没有它，未来的AI会话将会争论目标。
3. **覆盖剧院预防** -项目特定的假测试示例，源自您在探索期间看到的内容。（原因：人工智能生成的测试经常填充覆盖率数字，而没有捕捉到真正的bug——断言导入有效，字典有键，或者mock返回它们配置返回的内容。）显式地调用它会停止模式。)
4. * * Fitness-to-Pur场景** -它的核心。每个场景都记录了一个实际的故障模式，包括代码引用和验证方法。目标是每个核心模块有2个以上的场景——一般来说，中型项目总共有8-10个场景，小型项目少一些，复杂项目多一些。质量比数量更重要：精确捕获真正架构漏洞的场景比三个通用的场景更有价值。(为什么：覆盖率百分比告诉您运行了多少代码，而不是它是否正确运行。一个系统可以有95%的覆盖率，但仍然无声地丢失记录。适应度场景定义了“正常工作”的具体含义，没有人能反驳。)
5. **AI会话质量纪律** -每个AI会话必须遵循的规则
6. **人类之门** -需要人类判断的事物**场景语音至关重要。**将“发生了什么”写为具有特定数量、级联结果和检测难度的架构漏洞分析，而不是抽象的规范。由于`save_state()`缺乏原子重命名模式，在10,000条记录的批处理过程中，写入中途崩溃将留下一个损坏的状态文件—下一次运行得到JSONDecodeError并且无法恢复。在没有检测机制的情况下，这可能会无声地丢失1693多条记录。”人工智能会话阅读不会降低标准。使用您对类似系统的了解来生成实际的故障场景，然后将它们置于您所探索的实际代码中。场景来自于代码探索和领域知识，关于这样的系统中哪里出了问题。

每个场景的“如何验证”必须映射到功能测试文件中的至少一个测试。

文件2：功能测试**这是最重要的交付成果。**阅读完整的指南`references/functional_tests.md`。

将测试组织成三个逻辑组（类、描述块、模块或测试框架使用的任何东西）：

- **规格要求** -每个可测试规格部分一个测试。每个测试的文档都引用了它所验证的规范需求。
- **适应度场景** -每个QUALITY.md场景测试一次。1:1映射，命名为match。
- **边界和边缘情况** -从步骤5每个防御模式一个测试。关键的规则:
- **完全匹配现有的导入模式。**阅读现有测试如何导入项目模块并执行相同的操作。错了就意味着每次测试都失败了。
- **在调用每个函数之前阅读它的签名。**读取实际的`def`行-参数名称，类型，默认值。从项目中读取实际数据文件以理解数据形状。不要猜测函数参数或夹具结构。
- **没有占位符测试。**每个测试必须导入并调用实际的项目代码。如果主体是`pass`，或者断言不重要（`assert isinstance(x, list)`），则删除它。不执行项目代码的测试会使计数膨胀，并产生错误的信心。
- **测试计数启发式** =（可测试规范部分）+ （QUALITY.md场景）+（防御模式）。对于一个中等规模的项目（5-15个源文件），这通常会产生35-50个测试。更少的建议遗漏需求或shallow探索。如果每个测试都是有意义的，那么多一点是可以的——不要急于达到一个数字。
- **跨变量启发式：~30%** -如果项目处理多种输入类型，目标是在所有变量中参数化大约30%的测试。比起确保在所有变体中测试每个横切属性，准确的百分比更重要。
**测试结果，而不是机制** -断言规范说应该发生什么，而不是代码如何实现它。
- **使用模式有效的突变** -边界测试必须使用模式接受的值（从步骤5b），而不是它拒绝的值。###文件3:`quality/RUN_CODE_REVIEW.md`**读取模板为`references/review_protocols.md`**。

代码审查协议有三个步骤。每个传递都独立运行——一个除了需求文档之外没有共享上下文的新会话。这种清晰的分离防止了结构审查和基于需求的审查之间的交叉污染。

**通过1 -结构审查。**阅读代码并发现异常。这是每个AI代码审查工具已经做得很好的事情。没有需求，没有重点领域——只有模型自己对代码正确性的认识。保持这些强制性护栏：

-行号是强制性的-没有行号，没有发现
-读取函数体，而不仅仅是签名
-如果不确定：标记为问题，而不是BUG
-在认领失踪前先Grep
-不要建议改变风格-只标记不正确的东西

**最低要求通过1个审查区域（明确指出每个区域）：**1. **输入验证和边界处理-检查外部或调用者提供的数据进入代码的每个信任边界。每个字符串解析器、枚举查找和二进制格式解析器都必须拒绝与有效令牌共享有效前缀但包含额外字符的输入。
2. **资源生命周期** -分配，重新计数管理，错误路径清理，失败时锁释放，文件descriptor/handle生命周期。每个获取引用或资源的函数必须在每个早期退出路径上释放它，或者必须在获取资源之前完成所有验证。
3. 并发性和状态管理——锁排序、原子操作正确性（共享状态字的每个原子修改都必须使用读-修改-写语义，并保留预期修改之外的位）、状态机完整性（所有消费者处理的所有状态）。
4. **单位和编码正确完整性-从硬件、协议结构或用户输入中读取的每个字段，在用于计算或比较之前必须正确转换。
5. **枚举和白名单完整性** -当函数使用`switch`/`case`，`match`， if-else链或任何分支结构来处理一组命名常量（特征位，枚举值，事件类型，命令代码，权限标志）时，执行**机械枚举检查**：(a) **列表a（代码提取）：**如果这个函数存在一个`quality/mechanical/<function>_cases.txt`工件，使用它作为权威的代码端列表-不要手动重新提取。如果不存在机械工件，则提取代码中实际存在的每个branch/case标签。列出每一行的确切行号：“行3511:`case VIRTIO_RING_F_INDIRECT_DESC`”、“行3513:`case VIRTIO_RING_F_EVENT_IDX`”等等。**仅从代码中提取此列表—不要从REQUIREMENTS.md、CONTRACTS.md或任何其他生成的工件中复制。**如果不能为case标签引用行号，则表示不存在。

(b) **List b （spec提取）：**列出每个在相关头文件、枚举或spec中定义的“应该”处理的常量。

(c) **Diff:**比较两个列表。对于列表B中的每个常量，将其标记为“FOUND （line NNN）”或“NOT in CODE”。报告任何已定义但未处理的常量。**不要断言白名单“涵盖所有值”或“保留支持的位”而不执行此双列表比较。** AI模型可靠地幻觉了switch/case构造的完整性——模型看到函数，看到其他地方定义的常量，并假设覆盖而不检查每个case标签。这种幻觉最危险的形式是从断言存在常量的上游工件（如REQUIREMENTS.md）进行复制，而不是从代码中提取。在v1.3.17中，代码审查的“case labels present”列表与需求列表一字不差——证明它是复制的而不是提取的。每个标签行号的机械检查是解决办法。这五个区域必须在Pass 1报告中作为标记的子部分出现。如果一个项目没有有意义的并发性，明确地说明并记录原因，而不是省略这一部分。根据需要，在这五个领域之外添加特定于项目的审查领域。

第1关捕获了约65%的实际缺陷：竞争条件、空指针危险、资源泄漏、过一错误、类型不匹配——代码中可见的结构性问题。

**通过2级-需求验证。**对于第1阶段第7步衍生的每个可测试需求，检查代码是否满足它。对于每个需求，要么显示满足它的代码，要么具体解释为什么不满足它。这是一个纯粹的验证通过——审稿人的唯一工作是“代码满足这个要求吗？”不是一般的回顾。而不是寻找其他的bug。只是验证。**最低证据规则：**通过2必须引用至少一个代码位置（文件：行或文件：功能）**每个要求**。像“REQ-003到REQ-012——通过审查期间审查的客户端路径得到了满足”这样的总体满意度声明，如果没有每个需求的代码引用，就不能满足第二阶段。如果同一函数满足两个或三个需求，则引用该函数一次并列出这些特定需求—但是每个需求必须单独出现，并附带自己的SATISFIED/VIOLATED结论，而不是作为未经验证的范围的一部分。如果在一个引用中包含三项以上的要求，则表明验证是肤浅的。关键在于可追溯性——阅读第2阶段的审阅者应该能够遵循证据链，从任何单个需求到满足需求的代码，而无需重新阅读整个代码库。**枚举完整性声明需要机械证明。**当评估一个涉及到白名单、查找表、特性位集、处理程序注册表或任何形式的声明“所有X都被Y覆盖”的需求时，审稿人必须执行来自第1阶段审查区域5的双列表枚举检查：从代码中提取每一项（带行号），从规范中提取每一项，然后修改。**代码端列表必须从源代码中提取——不要重用来自REQUIREMENTS.md、CONTRACTS.md、代码审查提示符或任何其他生成的工件的任何列表。**如果代码端列表与需求列表逐字匹配，则表明该列表是复制的，而不是提取的，并且必须重新进行检查。不要在读取函数并认为它处理了所有事情的基础上标记满足这样的需求——这是该规则防止的特定幻觉模式。例如：一个需求说“传输特性白名单必须保留所有支持的环特性”。审稿人阅读`vring_transport_features()`，看到它有一个switch/case.正确的检查：提取每个箱子标签及其行号(`line 3511: INDIRECT_DESC`,`line 3513: EVENT_IDX`，…（`line 3527: default`），然后列出头常量，然后diff.幻觉：“白名单保留支持的位，包括VIRTIO_F_RING_RESET”，而不检查RING_RESET是否实际显示为大小写标签。这种精确的故障模式已经在多个版本的实践中被观察到——模型断言了交换机中不存在的常量的覆盖范围，并且在v1.3.17中，代码审查的“case labels present”列表是从需求中复制而来的，而不是额外的从代码中删除，导致三个独立的规范审计员继承了错误的声明。第2步捕获了对个别需求的违反——代码没有按照规范要求做的情况。这会发现结构审查遗漏的bug，因为那里的代码是正确的；bug是缺失的部分或与规范不匹配的部分。

**第3关-跨需求一致性。**比较引用相同字段、常量、范围或安全策略的需求对。对于每一对，验证它们的约束是否相互一致。数字范围是否与位宽度匹配？安全策略是否传播到所有连接类型？一个文件中的验证边界是否与另一个文件中的编码限制一致？第3步捕获了两个单独正确的代码片段在共享约束上不一致的矛盾。这些错误对于结构审查和每个需求验证都是不可见的，因为每个需求都是单独满足的——只有在比较它们时才会出现错误。这是捕获跨文件算术错误和设计漏洞的通道，其中安全配置没有传播到所有连接路径。

**源代码边界规则：**剧本不能修改`quality/`目录以外的文件。所有源代码树的更改——bug修复、项目自己的测试套件的测试添加——必须表示为保存在`quality/patches/`下的`git diff`格式的补丁文件。这确保了原始的源代码树保持不变，补丁是可审查的和可逆的，剧本的发现与它审计的代码是完全分离的。**BUGS.md:**在所有审查和审计阶段之后，生成`quality/BUGS.md`-一个合并的bug报告，其中包含每个已确认的bug的完整复制细节。对于每个bug，包括：bug ID、源代码（代码审查或规范审计）、文件行、描述、严重性、最小重现场景（触发bug的输入或序列）、预期与实际行为、对回归测试的引用和任何建议的修复补丁，以及**规范基础**。

**BUGS.md- v1.5.3 BUG记录字段（schemas.md§8）。**每个BUGS.md条目（以及每个`quality/writeups/BUG-NNN.md`）对应一个写入`quality/bugs_manifest.json`的BUG记录。除了上述叙述约定，每个BUG还包含：-`id`-`BUG-NNN`补零三位数序列。
-`divergence_description`-文档化意图和代码行为之间分歧的一段总结。
-`documented_intent`- REQ / spec语言的直接引用或近似释义。
-`code_behavior`-代码实际做什么，与`file:line`引用。
-`disposition`- enum源自schemas.md§3.2:`code-fix`|`spec-fix`|`upstream-spec-issue`|`mis-read`|`deferred`。要求,非空。不要发明新的价值观。
-`disposition_rationale`-一段解释为什么这个处置而不是相邻的处置（例如，为什么`code-fix`而不是`upstream-spec-issue`，或者为什么`spec-fix`而不是`mis-read`）。公式化的理由（“代码是错的，因为规范是这么说的”）未能通过理事会的审查。
-`req_id`-单数。主要的REQ揭示了分歧。如果一个错误似乎触及多个REQ，在`disposition_rationale`（schemas.md§8.1）中，将每个REQ拆分为一个bug，共享一个根本原因和交叉链接。不要走私多个REQ id到一个条目。
-`proposed_fix`-必需，除非`disposition == "mis-read"`。当`fix_type`∈{`code`，`both`}；文本红线时`fix_type == "spec"`。对于`mis-read`记录，该字段是可选的，当出现时，记录重新读取的内容（剧本读错了什么，以及如何建立正确的读取），而不是发布的更改。
-`fix_type`- enum源自schemas.md§3.4:`code`|`spec`|`both`。`disposition × fix_type`的组合受到§3.4中的合法组合矩阵的约束（由§10不变式#12强制执行）。非法配对：`code-fix`×`spec`，`spec-fix`×`code`,`upstream-spec-issue`×`code`,`mis-read`×`both`。在创建记录之前，请参考矩阵-门拒绝非法组合。**发散帧（书面声音，schemas.mdv1.5.3）。**缺陷是文档化的意图和代码实现之间的分歧——而不是对代码是否“好”的判断。文章的开头部分（摘要、规范参考、代码）是`divergence_description`、`documented_intent`和`code_behavior`的人工呈现。把它们写成并列的句子来读，而不是叙述。读者应该能够扫描代码行为旁边的REQ/spec语言，并立即看到差距。对抗性的语气（“代码是草率的”）或标题中的价值判断（“草率的拖车处理”）无法实现这种框架——bug是对分歧的观察，而不是指责。上游维护者参与处理（代码修复vs规范修复vs上游规范问题），而不是针对批评维护代码。**确认bug的充分证据（关键）。**证明特定行为违规的代码路径跟踪是确认bug的充分证据。您不需要已执行的请求级证据、正在运行的测试或集成级复制来促进从候选到确认的发现。具体:-代码路径跟踪显示函数A调用函数B执行X，但应该执行Y，与file:line引用- **足以确认**。
-缺失的case/branch通过枚举比较确定（规范说X应该处理，代码没有X的处理程序）- **足以确认**。
-在步骤2中发现的需求违反，其中代码明显没有实现指定的行为-足以确认。
-一个领域知识发现，你可以跟踪从输入到特定的代码到错误的输出-足以确认。不要要求“已执行的请求级证据”或推迟发现，因为“它们需要运行时测试来区分实现选择和规格差距”。如果规范或文档说行为应该是X，而代码明显地产生了Y（通过代码路径可跟踪），那就是一个已确认的bug——而不是等待运行时验证的候选bug。回归测试和TDD协议的存在是为了在确认之后提供运行时证据，而不是作为确认的先决条件。**在v1.3.43 javalin基准测试中，代码审查和分类都通过代码路径跟踪和需求违反确定了4个合法的候选bug，然后将它们全部降级，因为“最高置信度的项目仍然需要执行请求级别的证据”。这在代码库中产生了0个已确认的bug，而以前的版本发现了5个。证据标准是在运行时验证之前设置的，这是反向的——剧本的设计是先确认，然后用tdd进行验证。

**严重性校准：**无论评估的可能性如何，凭证泄漏，身份验证绕过和注入类错误始终是高严重性的。跨信任边界的授权报头暴露（例如，跨域重定向）是凭证泄漏。当对安全相关的严重性有疑问时，默认为高。开发脚手架排除（在确认任何安全类发现之前的早期过滤）。**上述严重性校准规则自动升级凭据泄漏和身份验证绕过的发现。在确认任何此类发现为bug之前，应用以下机械测试：

b> **检查下的值是否包含一个自文档标记——像`change-me`、`replace-me`、`placeholder`、`example`、`default`、`dummy`、`TODO`、`FIXME`、`your-secret-here`、`insert-`或`set-this-`这样的词——明确地告诉部署者替换它？**

如果是：**这不是一个bug。**不要记录。自文档化开发占位符是脚手架，而不是缺陷——它的存在是为了使项目无需配置即可在本地构建和运行。同样的逻辑也适用于测试装置、示例配置、种子数据以及任何其名称或周围注释声明其为非生产的值。这是一个早期过滤器，可以在确认时捕获最明显的误报。阶段5中的挑战门（参见`references/challenge_gate.md`）是捕获更细微情况的更广泛的机制——记录的特性差距、带有WHY注释的设计决策，以及审计人员发明的“预期行为”的发现。任何通过脚手架排除的安全类发现，如果与自动触发模式匹配，仍然会在阶段5中受到挑战。**规范基础（每个bug的必选字段）：**引用建立预期行为的特定文档段落-收集的文档文件名，section/page，以及它定义的行为契约。如果没有收集到的文档涵盖该行为，请检查项目自己的注释、README或API文档是否定义了该行为。如果没有针对预期行为的文档存在，那么将错误分类为“代码不一致”而不是“规范违反”，并在严重性评估中注明这一点。违反规范是一个比代码不一致更重要的发现——它意味着代码与权威来源相矛盾，而不仅仅是代码看起来是错误的。在向上游报告时，这种区别很重要：维护者对“你的代码违反了你自己规范的X.Y部分”的反应与“这看起来可能是一个bug”的反应不同。**补丁文件（必须为每个确认的错误）。**对于每个确认的bug，生成：`quality/patches/BUG-NNN-regression-test.patch`-一个`git diff`，增加了一个测试来证明这个bug。**此补丁是必选的，不是可选的。**这是一个bug存在的最有力的证据——独立于任何关于修复的意见。没有回归测试补丁的已确认错误是不完整的，将导致`quality_gate.py`失败。在确认错误后立即生成此补丁，然后再移动到下一个错误。
-`quality/patches/BUG-NNN-fix.patch`（可选，但强烈建议）-带有建议修复的`git diff`。对于只需一行或几行修改即可修复的bug（例如，添加case标签，修复参数），生成修复补丁-这些是低工作量和高价值的。

**如何生成补丁文件。**使用`git diff`格式。最简单的方法：将补丁内容直接写入统一的diff。```
--- /dev/null
+++ b/quality/test_regression_virtio.c
@@ -0,0 +1,15 @@
+// Generated by Quality Playbook v1.5.6
+// Regression test for BUG-004: VIRTIO_F_RING_RESET missing from vring_transport_features()
+#include <assert.h>
+#include <string.h>
+...
```
对于修改现有源文件的修复补丁，请使用具有正确行偏移量的`--- a/path`/`+++ b/path`格式。如果你不能确定确切的线偏移量，生成补丁内容并注明“偏移量近似值”——一个近似的补丁比没有补丁更有价值。

补丁必须使用`git apply`清晰地应用于原始源代码树。不要直接修改源代码树。

**补丁验证门（必选）。**在用补丁确认任何bug之前，运行这个gate：

1. **应用测试：**`git apply --check quality/patches/BUG-NNN-regression-test.patch`-必须退出0。
2. **应用测试+修复：**`git apply --check quality/patches/BUG-NNN-fix.patch`-必须退出0（针对干净树进行测试，而不是针对回归测试应用树进行测试，除非修复补丁依赖于回归测试）。
3. **编译检查：**应用两个补丁后，运行项目的build/compile命令（例如，`go build ./...`,`mvn compile`,`cargo check`,`tsc --noEmit`）。必须成功。**步骤3的临时工作树。**步骤1-2使用`--check`（无损）。步骤3需要实际应用补丁并进行编译，这会修改源代码树。为了遵守源代码边界规则（“永远不要修改`quality/`以外的文件”），在一次性工作树中运行步骤3：```bash
git worktree add /tmp/qpb-patch-check HEAD --quiet
cd /tmp/qpb-patch-check
git apply quality/patches/BUG-NNN-regression-test.patch quality/patches/BUG-NNN-fix.patch
<compile command>
cd -
git worktree remove /tmp/qpb-patch-check --force
```
如果`git worktree`不可用（浅克隆、分离的HEAD），则使用`git stash && git apply ... && <compile> && git checkout . && git stash pop`作为后备，或者接受仅`--check`验证，并注意其局限性。

**编译检查解释语言。**编译命令因生态系统而异：
- **Go:**`go build ./...`**锈：**`cargo check`- **Java/Kotlin(Maven):**`mvn compile -q`- **Java/Kotlin(Gradle):**`./gradlew compileJava compileTestJava -q`**TypeScript:**`tsc --noEmit`**Python:**`python -m py_compile <changed_files>`语法，然后`pytest --collect-only -q`验证import/discovery**JavaScript (Node.js):**`node --check <changed_files>`语法；如果项目使用ESLint，`npx eslint <changed_files>`用于结构问题
- **JavaScript (Mocha/Jest):**在发现模式下运行特定的测试（`mocha --dry-run`或`jest --listTests`），以验证其加载没有错误

如果对项目的语言没有可行的compile/syntax检查，则将其记录在补丁条目中，并依赖TDD红色阶段来捕获语法错误。如果任何步骤失败，请修复补丁，然后记录已确认的错误。一个带有损坏补丁的bug不能应用，这不是一个被证实的bug——这是一个证据不完整的假设。TDD红绿循环不能在不适用的补丁上运行，并且用不适用的补丁报告错误会破坏上游维护者的可信度。常见的补丁失败：截断块（缺少闭括号），错误的行偏移（针对修改树而不是干净树生成补丁），以及生成的测试代码中的语法错误。

**修复补丁要求。**每个已确认的bug必须具有以下任一项：
-通过上述验证门或的`quality/patches/BUG-NNN-fix.patch`-在BUGS.md中明确解释为什么没有提供修复补丁（例如，“修复需要超出补丁范围的架构更改”，“多个有效的修复策略-服从维护者的判断”，“错误位于上游依赖”）。有回归测试但没有修复补丁和理由的bug是不完整的。回归测试证明了bug的存在；修复补丁（或证明其缺失的理由）完成了证据链。没有修复补丁的bug无法达到“TDD验证（FAIL→PASS）”状态——它们保持在“确认打开（xfail）”状态，直到提供修复。

**TDD验证周期：**每个带有修复补丁的确认bug都要经过红绿TDD周期（未打补丁的代码测试失败，修复后通过）。这是通过`quality/RUN_TDD_TESTS.md`协议执行的（文件7），而不是在代码审查期间内联执行的。该协议生成基于规范的测试，其中每个断言消息、变量名和注释都可以追溯到收集的文档。**经过三次检查后：**合并发现。用`quality/test_regression.*`编写回归测试，重现每个确认的bug。使用与`test_functional.*`相同的测试框架—如果功能测试使用pytest，则回归测试使用pytest（与`@pytest.mark.xfail(strict=True)`一起使用）；如果功能测试使用单元测试，则回归测试使用单元测试（使用`@unittest.expectedFailure`）。以确认表的形式报告结果（BUG确认/误报/需求调查）。请参阅`references/review_protocols.md`了解完整的三步通过模板和回归测试协议。

**回归测试跳过保护（强制性）。**`quality/test_regression.*`中的每个回归测试都必须包含skip/xfail保护，以便在未打补丁的代码上运行完整的测试套件不会产生意外的失败。该保护必须是框架**最早的语法保护——惯用的修饰符或注释，否则是测试体中的第一个可执行行。使用适合语言的机制：**Python (pytest):**`@pytest.mark.xfail(strict=True, reason="BUG-NNN: [description]")`-作为**装饰符放置在**`def test_...():`之上，而不是在函数体中。当存在错误时，测试失败→XFAIL（预期）。当错误被修复，但标记没有被删除时，测试通过→XPASS→严格模式使其失败，表明应该删除保护。
**Python (unittest):**`@unittest.expectedFailure`-测试方法上面的装饰器。
- **Go:**`t.Skip("BUG-NNN: [description] — unskip after applying quality/patches/BUG-NNN-fix.patch")`-测试函数内的第一行。注意：Go的`t.Skip`完全隐藏测试（报告SKIP，而不是FAIL），这是比Python的xfail更弱的证据。这是已知的Go测试原语的限制。
**Java (JUnit 5):**`@Disabled("BUG-NNN: [description]")`-注释上面的测试方法。
**Rust:**测试函数的`#[ignore]`属性（标准的“不要在默认套件中运行”机制）。`#[should_panic]`仅用于显示为恐慌的bug；仅在编译时错误时使用`compile_fail`doctest注释。
-**TypeScript/JavaScript（开玩笑）：**`test.failing("BUG-NNN: [description]", () => { ... })`- **TypeScript/JavaScript(Vitest):**`test.fails("BUG-NNN: [description]", () => { ... })`**JavaScript (Mocha):**`it.skip("BUG-NNN: [description]", () => { ... })`或`this.skip()`在测试体中的条件跳过。当一个bug被修复（永久应用修复补丁）时，移除跳过保护并将bug跟踪器关闭状态从“确认打开”更新为“修复（测试通过）”。跳过保护消息必须引用错误ID和修复补丁路径，以便遇到跳过测试的人确切地知道如何解决它。**必须执行源代码检查测试（无`run=False`）。**验证源文件结构的回归测试——函数体中的字符串是否存在、大小写标签是否存在、枚举提取、生成代码形状检查——是安全、确定和快速的。它们读取存储库文件并执行字符串匹配。对于这些测试，使用启用执行的`@pytest.mark.xfail(strict=True)`。**不要使用`run=False`**，除非测试会改变外部状态、挂起或需要不可用的基础架构。使用`run=False`进行源代码检查测试是可能出现的最坏状态：存在正确的检查，但没有反应。在v1.3.18中，bug -004 （`test_bug_004_transport_feature_whitelist_keeps_ring_reset`）的回归测试包含正确的断言`assert "case VIRTIO_F_RING_RESET:" in func`，但标记为`run=False`—因此测试从未执行，断言从未触发，尽管测试套件“通过”，但错误仍然未被检测到。当`xfail(strict=True)`测试实际执行并失败时，测试套件将其报告为aXFAIL（预期失败）——这是正确的行为，而不是套件失败。**TDDred/green与skip guard交互。**在TDD验证周期中，红色和绿色阶段必须暂时绕过跳过保护以实际执行测试。协议应该指示代理：
- **红色阶段（从未跳过）：**删除或禁用skip/xfail保护，然后对未打补丁的代码运行测试。它必须失败。记录结果后重新启用保护。**红色阶段是强制性的每一个确认的错误，即使没有修复补丁存在。**没有红阶段证据的bug是未验证的-如果没有失败的红运行，不要记录`verdict: "skipped"`。如果红色阶段由于已记录的原因（编译失败、环境不可用）而无法执行，请记录`red_phase: "error"`，并在`notes`中给出解释。
- **绿色阶段：**删除或禁用防护，应用修复补丁，运行测试。它必须过去。如果修复将被恢复，请重新启用保护。**如果没有修复补丁首先，记录`green_phase: "skipped"`-但红色阶段必须仍然运行
—** TDD周期后：**保护保留在提交的回归测试文件中。只有当修复被合并到源代码树中时，它才会被永久删除。**TDD执行强制（强制性）。**回归测试必须在TDD验证周期中实际执行，而不仅仅是作为补丁文件生成。对于每一个确认的bug，红色阶段的测试运行必须在`quality/results/BUG-NNN.red.log`上生成一个日志文件，以捕获测试输出。绿色阶段（如果存在修复补丁）必须生成`quality/results/BUG-NNN.green.log`。每个日志文件的第一行必须是一个状态标记：`RED`（测试如预期失败）、`GREEN`（修复后通过的测试）、`NOT_RUN`（无法执行测试—有解释）或`ERROR`（测试基础结构失败—有解释）。

**语言感知测试执行命令。**使用项目的本机测试运行器执行回归测试。检测项目语言并使用适当的命令：- **Go:**`go test -v -run TestBugNNN ./path/to/package`- **Python (pytest):**`python -m pytest -xvs quality/test_regression.py::test_bug_nnn`- **Python (unittest):**`python -m unittest quality.test_regression.TestRegression.test_bug_nnn`- **Java (Maven + JUnit):**`mvn test -pl module -Dtest=RegressionTest#testBugNnn`**Java (Gradle + JUnit):**`./gradlew test --tests RegressionTest.testBugNnn`- **Rust:**`cargo test bug_nnn -- --nocapture`**TypeScript/JavaScript（开玩笑）：**`npx jest --verbose --testNamePattern="BUG-NNN"`- **TypeScript/JavaScript(Vitest):**`npx vitest run --reporter=verbose --testNamePattern="BUG-NNN"`- **C (kernel/make-based):**源代码检查测试通过shell脚本（grep/awk源文件）-记录脚本输出。

如果项目使用上面没有列出的语言或测试框架，请使用项目已经使用的任何测试运行器（检查`Makefile`、`package.json`、`build.gradle`、`Cargo.toml`、`go.mod`、`setup.py`、`pyproject.toml`等）并调整模式。如果没有可用的测试运行器或者没有安装语言运行时，记录`NOT_RUN`并给出解释—不要完全跳过日志文件。

**日志捕获格式。**每个`BUG-NNN.red.log`和`BUG-NNN.green.log`必须遵循以下格式：```
RED
--- Test output for BUG-NNN red phase ---
Command: [exact command run]
Exit code: [exit code]
[full stdout/stderr from test execution]
```
第一行的状态标记（`RED`、`GREEN`、`NOT_RUN`、`ERROR`）是机器可读的—`quality_gate.py`将检查它的存在。当测试运行器不可用时，`NOT_RUN`状态是可以接受的（例如，在不存在内核构建环境的C项目中），但是日志文件必须仍然存在，并解释为什么不能执行测试。

**准备运行的TDD日志模板。**对于每个确认的BUG-NNN，执行以下顺序（根据上表调整项目语言的test命令）：```bash
# ── Red phase: revert fix, run test, expect FAIL ──
git apply -R quality/patches/BUG-NNN-fix.patch 2>/dev/null   # revert fix if applied
TEST_CMD="python -m pytest -xvs quality/test_regression.py::test_bug_nnn"  # adapt per language
OUTPUT=$($TEST_CMD 2>&1); EXIT=$?
printf 'RED\n--- Test output for BUG-NNN red phase ---\nCommand: %s\nExit code: %d\n%s\n' \
  "$TEST_CMD" "$EXIT" "$OUTPUT" > quality/results/BUG-NNN.red.log

# ── Green phase: apply fix, run test, expect PASS ──
git apply quality/patches/BUG-NNN-fix.patch
OUTPUT=$($TEST_CMD 2>&1); EXIT=$?
printf 'GREEN\n--- Test output for BUG-NNN green phase ---\nCommand: %s\nExit code: %d\n%s\n' \
  "$TEST_CMD" "$EXIT" "$OUTPUT" > quality/results/BUG-NNN.green.log
```
对每个确认的bug运行此操作。如果测试运行器不可用，那么在第一行创建带有`NOT_RUN`的日志文件并给出解释。不要跳过这一步——如果缺少日志，阶段5中的TDD日志关闭门将阻止完成。

**TDD执行门。**在阶段5的终端门前，验证`quality/BUGS.md`中每确认一个bug，就存在一个对应的`quality/results/BUG-NNN.red.log`。没有红色阶段日志的bug是不完整的——回归测试补丁存在，但从未被证明能够检测到bug。这个门的存在是因为v1.3.45的基准测试表明，大多数repos生成回归测试补丁，但从不执行它们，从而使TDD结论未经验证。

###文件4:`quality/RUN_INTEGRATION_TESTS.md`**读取`references/review_protocols.md`**作为模板。必须包括：安全约束、飞行前检查、具有特定通过标准的测试矩阵、执行UX部分和结构化报告格式。涵盖快乐路径、跨变量一致性、输出正确性和组件边界。

**用例可追溯性（强制性）。测试矩阵必须包含一个用例跟踪列。每个集成测试组必须：

1. **映射到用例** -命名它验证的用例（例如，UC-03），并描述测试如何从该用例中练习用户结果。这些是主要的集成测试——它们验证用例中描述的端到端行为实际上是有效的。2. **被标记为基础设施** -不映射到用例的测试（构建验证，竞争检测，兼容性检查，现有测试套件回归保护）在可追溯性列中显式标记为`[Infrastructure]`。它们有价值，但不计入用例覆盖率。

在生成测试矩阵之后，检查：REQUIREMENTS.md中的每个用例是否至少有一个集成测试映射到它？如果没有，将未覆盖的用例标记为空白。映射到用例的集成测试应该测试用例中描述的端到端行为——而不仅仅是运行碰巧触及相同代码路径的现有单元测试。例如，如果一个用例说“开发人员在不泄露机密的情况下验证并遵循重定向”，那么集成测试应该跨带有auth头的域执行重定向，并验证它们被剥离了——而不仅仅是运行`pytest -k auth`。**每个uc组拆分（必选）。**每个集成测试组必须映射至多**2个用例**。映射到3个以上uc的组太粗糙了——当测试中断时，它不能区分哪个用例失败了。如果一个单独的测试命令（例如，`mvn test`,`go test ./...`）将执行多个用例，那么用目标测试选择器（`-Dtest=`,`-run`,`-k`,`--tests`,`-- test_name`，等等）将它分成单独的组，这样每个组隔离1-2个UCs。在一个未区分的命令中覆盖所有uc的组是明确禁止的——它们在发生故障时不提供诊断值。* * No-selector撤退。**如果项目的测试框架不能以分割所需的粒度选择测试（例如，不支持tag/filter的单片测试套件），请记录集成协议中的限制，并使用最窄的可行命令。记录该组涵盖哪些UCs，以及为什么不可能进一步划分。**单命令项目仍然必须使用分组JSON模式** -将命令包装在一个组中，并使用`use_cases`列表覆盖命令执行的所有UCs。命令的平面列表永远不是`groups[]`结构的有效替代品。**飞行前命令验证（强制性）。**在最终确定`RUN_INTEGRATION_TESTS.md`之前，请验证每个组的test命令是否实际发现并运行了测试。使用框架的dry-run或list模式来确认：
- **Python:**`pytest --collect-only -q <selector>`-必须列出至少一个测试
—**Go:**`go test -list "." <package>`—必须列出至少一个测试名
- **Java/Kotlin:**`mvn -Dtest=<selector> test -pl <module> --batch-mode -DfailIfNoTests=true`**TypeScript (Vitest):**`vitest list <file> --config <config>`-必须列出至少一个测试
**TypeScript (Jest):**`jest --listTests <pattern>`-必须列出至少一个文件
**Rust:**`cargo test <selector> -- --list`-必须列出至少一个测试
**JavaScript (Mocha):**`mocha --dry-run <file>`-必须列出至少一个测试如果运行结束时显示“未找到测试”、“未找到测试文件”或测试计数为零，请在记录组之前修复选择器。常见修复：添加`--config`或`--root`标志，使用文件路径代替`-t`名称模式，将正则表达式模式锚定到正确的包。不要记录命令发现失败的组—它将产生`covered_fail`结果，将选择器错误掩盖为代码错误。

如果运行失败，出现构建错误（编译失败、导入错误、缺少依赖项、测试设置异常）而不是“未找到测试”，请在组的`notes`字段中将失败记录为`"pre_flight_error": "environment"`，不要尝试修复选择器。飞行前的环境错误需要环境设置，而不是选择器的改变。**基础设施组定义。**单个`[Infrastructure]`组可以涵盖构建验证，竞争检测，静态分析和平台兼容性检查，而无需UC映射。基础设施测试验证构建工具链和平台支持，而不是用户可观察到的行为。基础设施组:
** *不计入用例覆盖率（UC覆盖率检查忽略它们）
-必须包含一行解释他们验证的基本原理
-可能**不能**用于重新标记广泛的用户工作流命令，以避免分裂-如果测试执行用例中描述的面向用户的行为，则无论测试如何组织，它们都必须映射到该UC**所有命令必须使用相对路径。**生成的协议应该在顶部包含一个“工作目录”部分，说明所有命令都使用相对路径从项目根运行。永远不要生成`cd`为绝对路径的命令—当协议从不同的机器或目录运行时，这种情况会中断。使用`./scripts/`、`./pipelines/`、`./quality/`等。**包括执行UX部分。**当有人告诉AI代理“运行集成测试”时，代理需要知道如何呈现其工作。协议应该指定三个阶段：(1)在运行任何操作之前将计划显示为编号表，(2)在每次测试运行（`✓`/`✗`/`⧗`）时报告一行进度更新，(3)显示带有pass/fail计数和建议的汇总表。有关模板和示例，请参阅`references/review_protocols.md`一节“执行UX”。如果不这样做，代理将转储原始输出或保持沉默—两者都没有用。

**结构化输出（必选）。**协议必须指示代理生成机器可读的结果以及Markdown报告，使用**JUnit XML**用于测试执行，**sidecar JSON**用于qpb特定的元数据。**JUnit XML输出：**每个测试组应该使用框架的本机JUnit XML报告器运行：
—Python:`pytest --junitxml=quality/results/integration-group-N.xml`—Go:`gotestsum --junitxml quality/results/integration-group-N.xml -- -run "TestPattern"`—Java/Kotlin：复制Surefire XML报告到`quality/results/`- TypeScript:`jest --reporters=jest-junit`with`JEST_JUNIT_OUTPUT_DIR=quality/results/`- Rust:`cargo test 2>&1 | cargo2junit > quality/results/integration-group-N.xml`（如果可用）

如果JUnit XML报告器不可用，则跳过XML，并在sidecar JSON中注明`"junit_available": false`。

**Sidecar JSON:**通过逐字复制下面的模板并只填写值来生成`quality/results/integration-results.json`。不要发明字段、重命名键或重构模式。没有`groups`数组的命令列表是无效的—即使项目通过单个命令运行所有测试，也要将其包装在一个组中。```json
{
  "schema_version": "1.1",
  "skill_version": "<current skill version>",
  "date": "YYYY-MM-DD",
  "project": "<project name>",
  "recommendation": "SHIP",
  "groups": [
    {
      "group": 1,
      "name": "Core routing dispatch",
      "use_cases": ["UC-01", "UC-02"],
      "result": "pass",
      "tests_passed": 5,
      "tests_failed": 0,
      "junit_file": "integration-group-1.xml",
      "junit_available": true,
      "notes": ""
    }
  ],
  "summary": {
    "total_groups": 9,
    "passed": 8,
    "failed": 1,
    "skipped": 0
  },
  "uc_coverage": {
    "UC-01": "covered_pass",
    "UC-02": "covered_pass",
    "UC-03": "not_mapped"
  }
}
```
**必填顶级字段：**`schema_version`、`skill_version`、`date`、`project`、`recommendation`、`groups`、`summary`、`uc_coverage`。如果您的输出中缺少这些字段中的任何一个，则结果是不一致的。

**无效的例子（不要发出这些）：**
-平面`"results": [{"command": "go test ./...", "result": "pass"}]`-这不是分组模式。
—用`"commands_run"`代替`"groups"`的模式-键名错误。
-缺少`"uc_coverage"`的模式-必须出现REQUIREMENTS.md中的每个用例。
-模式用`"use_case_traceability"`代替`"use_cases"`-字段名错误。有效的`result`值：`"pass"`、`"fail"`、`"skipped"`、`"error"`。有效的`recommendation`值：`"SHIP"`（所有组都通过）、`"FIX BEFORE MERGE"`（非阻塞组失败）、`"BLOCK"`（关键组失败）。`uc_coverage`部分将每个用例从REQUIREMENTS.md映射到以下用例之一：`"covered_pass"`（至少有一个映射组通过了）、`"covered_fail"`（映射组但都失败了）或`"not_mapped"`（没有集成测试组映射到这个用例）。`"covered_fail"`和`"not_mapped"`之间的区别很重要：前者意味着测试存在，但代码被破坏了；第二种情况意味着没有测试。

运行器脚本和CI工具应该读取sidecar JSON以获取结果，而不是抓取Markdown报告。这消除了基于grep计数在匹配散文中的单词时产生错误数字的错误。**写后验证（必选）。**写完`integration-results.json`后，重新打开文件并验证：(1)所有必需的顶级字段都存在，(2)每个`groups[]`条目都有`group`，`name`,`use_cases`，`result`和`notes`，(3)所有`result`和`recommendation`值只使用上面列出的允许的enum值，(4)`uc_coverage`映射了REQUIREMENTS.md的每个用例，(5)没有额外的未记录的根键存在。如果检查失败，请在继续之前修复文件。**该协议必须执行真正的外部依赖。**如果项目与api、数据库或外部服务对话，那么集成测试协议将针对这些服务运行真正的端到端执行——而不仅仅是本地验证检查。围绕项目的实际执行模式和外部依赖关系设计测试矩阵。在探索过程中寻找API密钥、提供者抽象和现有的集成测试脚本，并在它们的基础上进行构建。

**从代码中获得质量检验关，而不是一般的检查。**在探索过程中读取验证规则、模式枚举和生成逻辑。将它们转换为具有特定字段和可接受值范围的每个管道质量检查。“所有单元都已验证”是不够的——协议必须验证特定于域的正确性。**脚本并行性，不要只是描述它。**组运行，使独立的执行（不同的提供者）并发运行。在`&`和`wait`中包含实际的bash命令。每次对每个提供商运行一次，以避免速率限制。

**根据项目校准单元计数。**读取`chunk_size`或等效配置。使用足够的单元来跨越至少2个块，并使用足够的单元来验证分发检查。集成测试通常是10-30。

**深度运行后验证。不要止步于“流程完成”。每次运行都要验证日志文件、清单状态、输出数据是否存在、样本记录内容和任何现有的质量检查脚本。**查找并使用现有的验证工具。**搜索验证输出质量的现有脚本（例如，`integration_checks.py`，验证脚本，质量门函数）。如果它们存在，从协议中调用它们。如果项目有TUI或仪表板，在运行后检查表中包括TUI验证命令（例如，`--dump`标志）。**在编写质量检验关之前建立一个字段参考表。**这是协议准确性最重要的一步。AI模型甚至在读了模式之后还自信地写错了字段名——`document_id`变成了`doc_id`，`sentiment_score`变成了`sentiment`，`float 0-1`变成了`int 0-100`。修复是过程性的：在写入每个表行之前立即重新读取每个模式文件。**不要依赖你之前在对话中读到的内容——你对字段名的记忆会漂移到成千上万的标记上。从文件内容逐字复制字段名。包括每个模式中的所有字段（如果模式有8个字段，则表有8行）。请参阅`references/review_protocols.md`部分“字段参考表”了解完整的过程和格式。不要跳过这一步-它可以防止最常见的协议不准确。

文件5:`quality/RUN_SPEC_AUDIT.md`- Council of Three

**阅读`references/spec_audit.md`**获取完整协议。三个独立的AI模型根据规范审核代码。为什么三个?因为每种模型都有不同的盲点——在实践中，不同的审计员会发现不同的问题。交叉引用可以捕捉到任何单一模型所遗漏的东西。

该协议定义了：带有保护的可复制粘贴的审计提示、特定于项目的审查区域、分类过程（按置信级别合并发现）和修复执行规则（按子系统进行小批量处理，而不是大型提示）。

次要重点镜头：**可选地为每个审计模型分配一个次要重点——例如，一个从输入验证开始，一个从资源生命周期开始，一个从并发开始。每个模型仍然执行完整的独立审计；这种强调偏向了注意力，但并不限制报道。不要根据bug类将模型划分为不一致的所有权。**少数查找规则：**在分类过程中，任何只有三个审计员中的一个标记它的查找结果（少数查找结果）都需要重新调查—读取特定的代码位置并做出显式的CONFIRMED/FALSE-POSITIVE确定，而不是默认丢弃。少数发现不成比例地可能是两个模型遗漏的真正错误。**分类不能提高代码路径分析之上的证据标准。**分诊步骤确认或拒绝发现-它不会延迟它们等待运行时证据。如果发现包含代码路径跟踪，显示行为违规（函数调用、缺少分支、带有file:line引用的错误返回值），则分类应该确认它。不要将代码路径跟踪的结果降级为“候选”或“需要运行时验证”。TDD协议（阶段5）在确认后提供运行时证据。关于完整的证据标准，请参阅BUGS.md一节中的“什么算是确认bug的充分证据”。**代码审查与规范审计冲突：**如果代码审查和规范审计在同一发现上不一致，规范审计的发现不会自动正确。部署验证探针——读取特定的代码位置并确定哪个评估是准确的。在BUG跟踪器中记录解决方案。没有被任何规范审核员标记的代码审查BUG仍然得到确认，但应该在结束之前用目标探针进行验证。

验证探针必须产生可执行的证据。**当分诊步骤通过验证探针确认或拒绝发现时，仅靠散文推理是不够的。探针必须产生一个测试断言，机械地证明该决定：- **对于拒绝**（发现是假阳性）：写一个通过的断言，证明发现是错误的。例如：如果拒绝“function X is missing null check”，则写`assert "if (ptr == NULL)" in source_of("X"), "X has null check at line NNN"`。如果你不能写一个证明你拒绝的断言，不要拒绝这个发现，把它升级为确认，或者标记为手动审查。

- **对于确认**（发现是一个真正的错误）：写一个失败的断言（预期的失败），证明错误存在。示例：如果确认“RING_RESET missing from switch”，则写入`assert "case VIRTIO_F_RING_RESET:" in source_of("vring_transport_features"), "RING_RESET should be in the switch but is not"`。

- **每个断言必须引用一个确切的行号**为它所引用的证据。不是“lines 3527-3528”，而是“line 3527:`default:`”——显示该行实际包含的内容。没有行号引用的断言是不够的。**该规则存在的原因：**在v1.3.16版本测试中，分流器正确地接收到`VIRTIO_F_RING_RESET`从switch/case白名单中丢失的少数发现。这个分类执行了一个“验证探测”，声称第3527-3528行“显式地保留了VIRTIO_F_RING_RESET”——但是这些行实际上包含了`default:`分支。分诊的人以为他们遵守了规定。如果要求编写`assert "case VIRTIO_F_RING_RESET:" in source`，则断言将失败，从而暴露出幻觉。要求拒绝的可执行证据会使虚幻的拒绝自我挫败：模型不能为代码中没有的东西编写一个传递的断言。检伤证据必须写入磁盘。**验证探测断言必须出现在磁盘上的文件中——要么追加到`quality/mechanical/verify.sh`，要么写入专用的`quality/spec_audits/triage_probes.sh`。在分类报告散文中描述但从未写入可执行文件的断言不是可执行证据。gate检查在分类输出中是否存在探测断言；在可执行文件中没有相应断言的“验证探测确认…”分类报告是不符合的。这防止了故障模式，即模型在没有实际运行的情况下描述探针将显示的内容。

文件6:`AGENTS.md`（编排器生成的；您不需要在阶段2中编写此文件）**v1.5.4合同：`AGENTS.md`是在阶段6成功后由`bin/run_playbook.py`生成的，而不是你在阶段2期间生成的。**编排器的`_safe_write_agents_md`助手使用您在阶段2中生成的`quality/`工件+阶段6门判定作为输入，将文件写入目标的repo根** （`<target>/AGENTS.md`，而不是`quality/AGENTS.md`）。生成器在第一个非空行上携带一个`<!-- generated by QPB v… -->`哨兵，以便后续运行检测qpb管理的副本。

**你（第二阶段法学硕士）不能做的事情：**1. **不要创建`<target>/AGENTS.md`。编曲者拥有它。从阶段2创建它与编排器的幂等再生路径相冲突，并且可以由源不变的不变量标记。
2. **不要修改已存在的`<target>/AGENTS.md`。**如果在目标的repo根目录中存在QPB哨兵，并且缺少QPB哨兵，那么它是由操作人员编写的——不要管它（编排器将保留它，并根据`_safe_write_agents_md`的`"preserved"`结果发出警告）。如果它携带QPB哨兵，编排器将在阶段6之后重新生成它；中途剪辑是没有用的。
3. **不要写`quality/AGENTS.md`。**这不是合同；AGENTS.md位于回购根目录，而不是`quality/`。如果你发现自己想要“向现有的AGENTS.md添加一个质量文档部分”-停止。协调器为您完成这些工作，这些工作来自于阶段6门验证的规范路径。您的第2阶段交付物是`quality/`工件，而不是其他。

**此错误修复防止了在2026-04-30上出现的引导测试失败模式**：阶段2 LLM读取v1.5.3时代的“如果`AGENTS.md`已经存在，请更新它”指令，在此部分添加质量文档部分到项目的根AGENTS.md，并触发源不变的不变量-中止运行并丢弃20分钟的阶段2工作。

文件7:`quality/RUN_TDD_TESTS.md`- TDD验证协议

该协议在代码审查和规范审计确认了错误并生成了修复补丁之后执行。它为每个确认的bug运行红绿TDD周期：未打补丁的代码测试失败，应用修复，测试通过。**为什么要单独的协议？**代码审查发现bug并使用`xfail`标记编写回归测试。TDD协议进行这些测试，并证明它们确实检测到了错误——并且修复确实修复了错误。这比“我们发现了一个bug并编写了一个测试”更有力。它是“这个测试没有补丁就失败了，有了补丁就通过了。”在上游报告bug时，这种区别很重要：维护者更信任FAIL→PASS演示，而不是bug描述。

生成的协议必须包括：1. **规范接地测试要求。**对于`quality/BUGS.md`中的每个bug，协议指示代理：
-阅读bug的“规范基础”字段，以确定定义预期行为的文档段落
-在引用部分阅读收集的文档（来自`reference_docs/`或项目自己的文档）
-使用规范中的**语言编写测试断言-变量名、常量、函数名和断言消息应该与规范的术语相呼应，而不是代码的内部命名
-在每个测试引用中包含注释块：需求ID（来自REQUIREMENTS.md）， bug ID（来自BUGS.md）和规范段落（文档名称，章节和行为契约的≤15个单词的引用）2. **红-绿执行步骤。**对于每个带有修复补丁的错误：
—**红色：**针对未打补丁的源运行回归测试。它必须失败。如果通过了，则测试没有检测到错误——使用规范基础重写它，以了解要断言的行为。
—**绿色：**应用修复补丁（`git apply quality/patches/BUG-NNN-fix.patch`），运行相同的测试。它必须过去。
- **记录：**在BUG跟踪器中记录两个结果，关闭状态为“TDD验证（FAIL→PASS）”。3. * *适应框架。**协议必须检测项目的测试框架并生成惯用的测试：
-带有测试基础设施的项目（pytest, JUnit， Go测试，Jest， cargo测试等）：在项目自己的框架中编写测试，遵循在探索过程中发现的现有测试惯例。
- **没有测试基础设施的项目**（如Linux内核，嵌入式C）：用`sed`提取目标函数，编写一个带有最小类型shims的自包含C测试文件，直接编译并运行。在测试文件的头注释中包含提取命令，这样它就可以自我记录了。4. **上游报告格式。**对于每个经过tdd验证的bug，生成一个包含以下内容的准备发送报告块：
-引用违反规范部分的一句话描述
- FAIL→PASS输出（可复制粘贴的终端会话）
-测试文件（作为附件或内联）
-修复补丁（作为附件或内联）

5. * *跟踪表。**协议生成一个`quality/TDD_TRACEABILITY.md`文件映射：

| Bug ID |需求ID |规范文档|规范章节|行为契约|测试文件：功能|红色结果|绿色结果|   |--------|---------------|----------|-------------|--------------------|--------------------|------------|--------------|
每一行都必须完全填充。没有规范文档条目的bug是代码不一致，而不是违反规范——在表中注意到这一点，并相应地调整上游报告语言。

6. **结构化输出（必选）。**协议必须与Markdown报告一起产生机器可读的结果。对于测试执行结果使用**JUnit XML**，对于JUnit XML不能表示的qpb特定元数据使用**sidecar JSON**文件。

**JUnit XML输出：**对于每个红-绿阶段，使用框架的本机JUnit XML输出标志运行测试：
—Python:`pytest --junitxml=quality/results/tdd-red-BUG-NNN.xml`—Go:`gotestsum --junitxml quality/results/tdd-red-BUG-NNN.xml -- -run TestRegression_BUG_NNN`-Java/Kotlin: Maven Surefire报告在`target/surefire-reports/`中自动生成；将相关XML复制到`quality/results/`- Rust:`cargo test --test regression 2>&1 | cargo2junit > quality/results/tdd-red-BUG-NNN.xml`（如果cargo2junit可用，否则跳过XML for Rust）
- TypeScript:`jest --reporters=default --reporters=jest-junit`with`JEST_JUNIT_OUTPUT_DIR=quality/results/`如果框架的JUnit XML报告程序不可用或需要缺少依赖项，则跳过该语言的XML输出，并将其记录在sidecar JSON （`"junit_available": false`）中。不要因为缺少XML工具而使TDD运行失败。

**Sidecar JSON（严格的模式强制）：**通过逐字复制**下面的模板并只填写值来生成`quality/results/tdd-results.json`。不要发明字段、重命名键或重构模式。模板就是模式——任何偏差（额外的键、丢失的键、重命名的键、重新构造的嵌套）都会导致输出不一致。首先将模板复制粘贴到编辑器中，然后填写值。不要从内存中写入JSON。   ```json
   {
     "schema_version": "1.1",
     "skill_version": "<current skill version>",
     "date": "YYYY-MM-DD",
     "project": "<project name>",
     "bugs": [
       {
         "id": "BUG-001",
         "requirement": "REQ-003",
         "red_phase": "fail",
         "green_phase": "pass",
         "verdict": "TDD verified",
         "regression_patch": "quality/patches/BUG-001-regression-test.patch",
         "fix_patch": "quality/patches/BUG-001-fix.patch",
         "fix_patch_present": true,
         "patch_gate_passed": true,
         "writeup_path": "quality/writeups/BUG-001.md",
         "junit_red": "tdd-red-BUG-001.xml",
         "junit_green": "tdd-green-BUG-001.xml",
         "junit_available": true,
         "notes": ""
       }
     ],
     "summary": {
       "total": 6,
       "verified": 4,
       "confirmed_open": 1,
       "red_failed": 1,
       "green_failed": 0
     }
   }
   ```
**必需的顶级字段：**`schema_version`，`skill_version`,`date`,`project`,`bugs`,`summary`。**每个bug需要的字段：**`id`，`requirement`,`red_phase`,`green_phase`,`verdict`,`fix_patch_present`,`writeup_path`。如果缺少任何必需的字段，则结果是不一致的。**可选的每个bug字段**（显示在上面的模板中，但不经过门检查）：`regression_patch`,`fix_patch`,`patch_gate_passed`,`junit_red`,`junit_green`,`junit_available`,`notes`。当数据可用时，包括这些；省略它们不会受到惩罚。

**要求的概要子键：**`summary`对象必须包含以下键：`total`，`verified`,`confirmed_open`,`red_failed`,`green_failed`。所有五个都是必需的——遗漏其中任何一个（尤其是`red_failed`或`green_failed`）都会使摘要不符合要求。**规范补丁文件名：**回归测试补丁必须命名为`BUG-NNN-regression-test.patch`。修复补丁必须命名为`BUG-NNN-fix.patch`。这些精确模式的gate脚本globs—像`BUG-001-regression.patch`或`BUG-001-test.patch`这样的创造性变体将不被计算在内。

**日期字段：**使用此会话的实际日期（例如，`"2026-04-12"`），而不是模板占位符`"YYYY-MM-DD"`。该门验证日期是否为真正的ISO 8601日期，并拒绝占位符字符串和未来日期。

**无效的例子（不要发出这些）：**
-`"runs": [{"phase": "red", "command": "...", "result": "4 xfailed"}]`-这是一个平面运行数组，而不是bug索引的`"bugs"`模式。
-具有特定根键的模式，如`"generated"`，`"scope"`,`"status"`，`"testsRun"`-这些不是标准的模式字段。
-`"verdict": "skipped"`-该值已弃用；使用`"confirmed open"`与`red_phase: "fail"`和`green_phase: "skipped"`。
-根目录缺少`"schema_version"`-每个tdd-results.json必须包含该字段。有效的`verdict`值：`"TDD verified"`(FAIL→PASS),`"red failed"`（测试通过未打补丁的代码-测试未检测到错误），`"green failed"`（修复后测试仍然失败-修复不完整或补丁损坏），`"confirmed open"`（红色阶段运行并确认错误，没有可用的修复补丁），`"deferred"`（TDD无法在此环境中执行-使用`notes`解释原因）。**不要使用`"skipped"`作为判定-每个确认的bug都必须有一个红色阶段的结果。`verdict: "confirmed open"`的bug必须有`red_phase: "fail"`（红色运行并确认了bug）和`green_phase: "skipped"`（没有修复应用）。有效的`red_phase`/`green_phase`值：`"fail"`，`"pass"`,`"error"`（compile/apply失败），`"skipped"`（只有绿色-永远不会跳过红色）。`patch_gate_passed`字段记录补丁验证门（apply-check + compile）是否成功，如果门失败并修复补丁，则为`false`，如果没有修复补丁，则为`null`。`writeup_path`字段指向per-bug writeup文件（参见下面的“Bug writeup generation”）-`null`，如果没有为这个Bug生成writeup。运行器脚本和CI工具应该读取sidecar JSON中的pass/fail计数，而不是删除Markdown报告。

**写后验证（必选）。**写完`tdd-results.json`后，重新打开文件并验证：(1)所有必需的顶级字段都存在，(2)每个`bugs[]`条目中都存在每个必需的每个bug字段，(3)所有`verdict`，`red_phase`和`green_phase`值只使用上面列出的允许的enum值，(4)不存在额外的未记录的根键。如果检查失败，请在继续之前修复文件。这一步捕获了最常见的失败模式：代理从内存中改写模式，而不是复制模板，从而产生看似合理但不一致的输出。**TDD工件关闭门（强制）。**如果`quality/BUGS.md`包含任何已确认的错误，`quality/results/tdd-results.json`是必选的-不是可选的。如果任何bug有红色阶段的结果（无论是TDD-verified还是confirmed-open），`quality/TDD_TRACEABILITY.md`也是强制性的。零bug库可以省略这两个文件。如果运行确认了bug，但没有产生tdd-results.json，则不完整—该阶段无法关闭。对于无法执行TDD的repos（环境阻塞，没有测试基础结构），用`verdict: "deferred"`和一个解释原因的`notes`字段生成tdd-results.json（例如，`"environment_blocked: missing workspace Cargo.toml"`,`"no_test_infrastructure: kernel C code without userspace harness"`）。延迟判决使空白可见，而不是静默地忽略文件。

**执行UX:**与集成测试相同的三阶段模式—(1)将计划显示为一个编号的bug表以进行验证，(2)在每个红绿循环运行时报告一行进度（`FAIL ✓ → PASS ✓`或`FAIL ✗ — test passes on unpatched code, rewriting`），(3)显示带有verified/failed/rewritten计数的汇总表。7. Bug编写生成（针对所有已确认的Bug）。**成功完成红→绿循环（`verdict: "TDD verified"`）或确认无修复（`verdict: "confirmed open"`）后，在`quality/writeups/BUG-NNN.md`生成一个独立的写入。该文件的目的是通过电子邮件发送给维护者，附加到Jira票据上，或者在存储库之外进行审查——它必须独立存在，而不需要读者浏览其余的质量工件。

**模板（每次编写都需要1-4、6、7节，深度判断触发时加5，存在相关bug时加8）：**1. **总结** -一段：什么是错的，在哪里（文件：行），在实践中有什么问题。
2. **规范引用** -被违反的特定规范部分，如果有URL的话。引用代码不能满足的行为契约（≤15个字）。
3. **代码** -带有文件：行引用的错误代码。从规范的角度解释为什么它是错误的，而不仅仅是“它看起来很奇怪”。
4. **可观察的结果** -什么真正打破。不是“理论上可能失败”而是什么会失败，在什么条件下，以什么症状。
5. **深度判断** *（仅包括在需要扩展时）* -在起草第1-4节后，评估：仅从代码和测试来看，结果是否不言而喻？如果读者会合理地问“为什么没有人注意到这一点？”或“这对所有配置都有影响吗？”跟踪有bug的函数的调用者。显示哪个代码部分他暴露了漏洞并掩盖了它。具体的扩展触发器：transport/config-dependent行为、掩盖某些路径上的错误的特性标志、间接调度隐藏调用者、negotiation/initialization代码中仅在特定运行时条件下才会出现的错误。如果直接代码的结果很明显（例如，null解引用，off-by- 1），保持第1-4节紧凑并省略这一节。
6. **修复** -建议修复为内联diff（统一diff格式），并简要解释为什么这是正确的修复。**始终包含一个具体的diff** -即使对于已确认打开的错误，也没有单独的`.patch`文件。如果修复是一行更改（添加case标签，修复参数），则编写diff。如果修复需要更广泛的更改，则编写解决核心缺陷的最小diff，并注意完整修复将需要哪些额外更改。写入中的内联差异使可操作的写程序——写程序说“不包含修复补丁”是不完整的，对维护者没有用处。示例格式:      ```diff
      --- a/drivers/virtio/virtio_ring.c
      +++ b/drivers/virtio/virtio_ring.c
      @@ -3527,6 +3527,7 @@ void vring_transport_features(...)
       	case VIRTIO_F_ORDER_PLATFORM:
       	case VIRTIO_F_IN_ORDER:
      +	case VIRTIO_F_RING_RESET:
       	default:
      ```
7. **测试** -测试证明了什么，如何运行它，以及在未打补丁和打补丁的代码上期望输出什么。
8. **相关问题** *（仅在存在相关错误时包含）* -同一类中的其他错误，如果有的话。即使还没有确认，也要标记它们。如果没有发现相关问题，则省略此部分。

在writeup文件的顶部包含版本戳**（与所有其他生成的文件格式相同）。**生成所有已确认的bug（强制性）。**在`quality/writeups/BUG-NNN.md`为每一个确认的bug生成一个书面报告——TDD-verified和confirmed-open。使用上面的编号部分模板（第1-8节）。对于已确认打开的bug，遵循相同的模板，包括第6节中建议的修复diff（即使没有单独的`.patch`文件，也总是需要diff）。写入阈值是bug确认，而不是TDD完成。已确认错误且没有写入目录的运行是不完整的。**内联diff是门强制的。**`quality_gate.py`脚本检查每个写入包含一个` `'`diff `块。没有内联diff的写入将导致门失败。不要写“see patch file”—将实际的diff内联粘贴到writeup主体中，在一个封闭的` `'`diff `代码块中。这是编写过程中最重要的一个元素，因为它使得只读编写过程的维护者可以对bug进行操作。

检查点：在生成工件后更新PROGRESS.md重读`quality/PROGRESS.md`。更新:
-用时间戳标记阶段2完成
-更新工件目录：将每个生成的工件设置为“已生成”的文件路径
-添加探索总结笔记，如果还没有出现**第二阶段完成门（强制性）。**在进行第3阶段之前，请核实：
1. 所有核心工件都存在于`quality/`（`QUALITY.md`、`CONTRACTS.md`、`REQUIREMENTS.md`、`COVERAGE_MATRIX.md`、`COMPLETENESS_REPORT.md`、`test_functional.*`、`RUN_CODE_REVIEW.md`、`RUN_INTEGRATION_TESTS.md`、`RUN_SPEC_AUDIT.md`、`RUN_TDD_TESTS.md`）下的磁盘上。`AGENTS.md`不在此列表中—编排器在阶段6之后将其写入目标回购根目录，而不是在阶段2之后。
2.`REQUIREMENTS.md`包含引用实际代码（文件路径、函数名、行号）的特定满足条件的需求，而不是抽象的行为描述。
3. 如果存在dispatch/enumeration合约：存在`quality/mechanical/verify.sh`合约，且已执行。
4.PROGRESS.md用时间戳标记阶段2完成。

在开始阶段3之前，重新阅读`quality/PROGRESS.md`和`quality/REQUIREMENTS.md`。需求是代码审查的目标列表——如果代码不满足它的条件，每个需求都是一个潜在的bug。**阶段结束消息（强制性-在阶段2完成后打印此消息，然后停止）：**```
# Phase 2 Complete — Quality Artifacts Generated

I've generated the quality infrastructure for this project:
[List the key artifacts created: REQUIREMENTS.md with N requirements and N use cases,
QUALITY.md with N scenarios, functional tests, review protocols, etc.]

The requirements are now the target list for Phase 3's code review — every requirement
is a potential bug if the code doesn't satisfy it.

To continue to Phase 3 (Code review with regression tests), say:

    Run quality playbook phase 3.

Or say "keep going" to continue automatically.
```
**打印此消息后，请停止。除非用户明确要求，否则不要进入第三阶段。**

---

阶段3：代码审查和回归测试

**v1.5.6 instrumentation:**现在将`phase_start phase=3`追加到`quality/run_state.jsonl`。在阶段结束时，交叉验证（存在`quality/RUN_CODE_REVIEW.md`；每个确定的bug都有一个写入），然后附加`phase_end phase=3`。

> **此阶段所需的引用：**
> -`quality/REQUIREMENTS.md`-代码审查的目标列表
三遍协议和回归测试约定

如文件3中所述，运行代码审查协议（全部三次通过）。在产生结果之后，根据`references/review_protocols.md`中的闭包命令为每个确认的BUG编写回归测试。**更新PROGRESS.md:**将每个确认的BUG添加到带有源代码“Code Review”的累积BUG跟踪器中，文件：行引用，描述，严重性，关闭状态（回归测试函数名称或豁免原因）。标志阶段3（代码审查+回归测试）完成。

**阶段结束消息（强制性-在阶段3完成后打印此消息，然后停止）：**```
# Phase 3 Complete — Code Review

The three-pass code review is done. [Summarize: N bugs confirmed, N regression test
patches generated, N fix patches generated. List the bug IDs and one-line summaries.]

To continue to Phase 4 (Spec audit — Council of Three), say:

    Run quality playbook phase 4.

Or say "keep going" to continue automatically.
```
**打印此消息后，请停止。除非用户明确要求，否则不要进入第四阶段

---

阶段4：规范审核和分类

**v1.5.6 instrumentation:**现在附加`phase_start phase=4`。对于每个传递A/B/C/D，附加`pass_started phase=4 pass=X`和`pass_ended phase=4 pass=X`。在阶段结束时，交叉验证（`quality/REQUIREMENTS.md`非空且`quality/COVERAGE_MATRIX.md`存在），然后附加`phase_end phase=4`。

> **此阶段所需的引用：**
理事会三协议，分诊过程，验证探针运行文件5中描述的spec审计协议。分类报告**必须**包括`## Pre-audit docs validation`部分（完整模板请参阅`references/spec_audit.md`）。即使`reference_docs/`为空，也需要此部分—在这种情况下，请注意审计人员使用的基线。根据上面的“验证探测必须产生可执行证据”规则，分类中的每个验证探测都必须产生可执行证据（带有行号引用的测试断言）。分诊后，对每一个确认的发现进行分类。**有效的理事会控制枚举检查。**如果有效委员会少于3/3（少于3名审核员返回可用报告），并且运行包括任何whitelist/enumeration/dispatch-function检查或任何结转种子检查，审计可能不会对未执行机械证明工件的检查得出“无确认缺陷”的结论。有机械验证的不完整的理事会是可以接受的。不完整的委员会依赖于对代码存在声明的仅散文验证，而不是升级到“需要验证”，并在结束之前运行机械检查。**预审计抽查必须从代码中提取，而不是从文档中断言。**当规范审计提示符包括预验证的抽查声明（例如，“验证函数X在Z行处理常数Y”）时，分类必须通过提取引用行中的实际代码来验证每个声明-而不是通过确认声明听起来合理。对于每个关于代码内容的抽查声明，预验证必须报告引用的行实际包含的内容：“第3527行包含`default:`-而不是声明的`case VIRTIO_F_RING_RESET:`。”如果抽查是从需求或收集的文档中生成的，而不是从代码本身生成的，那么将其视为需要测试的假设，而不是需要确认的事实。该规则防止了在v1.3.17中观察到的污染链，其中错误的抽查声明（“RING_RESET at 3527-3528”）被认为是“准确的”，而没有读取实际行，然后通过分类和传播进入每个下游工件。**更新PROGRESS.md:**从规格审核中添加每个确认的代码bug**到带有源代码“spec audit”的累积bug跟踪器。这是至关重要的——如果没有将规范审计错误添加到闭包验证所读取的跟踪器中，它们就会被系统地孤立。

第2层-语义引用检查（v1.5.3 Council sub-pass）

在主要规格审核分类之后，每个理事会成员针对每个Tier1/2REQ的`citation_excerpt`运行每个REQ裁决。这是幻觉门的第2层：第1层是机械字节相等性检查——`bin/citation_verifier`在摄取时间被`bin/reference_docs_ingest`调用，在入口时间被`quality_gate.py`重新调用；法学硕士从来不会直接支付给它。第2层是语义层——审稿人决定摘录是否真正支持所述的需求，或者需求是否超出了摘录所说的。

* *协议。* *1. **每个理事会成员一个提示，所有Tier1/2REQs都批量处理。**一次没有一个REQ （3×N提示太多）。不是散文式的回应（模式匹配的风险）。审稿人收到`(req_id, citation_excerpt, REQ description)`元组的完整列表，并返回一个结构化的按req的JSON响应。

2. **结构化响应模式（schemas.md§9.2）。**对于每个REQ，审稿人记录`{"req_id": "REQ-NNN", "reviewer": "<stable string>", "verdict": "supports" | "overreaches" | "unclear", "notes": "<reasoning>"}`。有效的`verdict`值在schemas.md§3.5中枚举。

3. * *配料阈值。**当运行产生超过15个Tier1/2req时，每个理事会成员每个提示最多15个req分成批次。同一审稿人按顺序查看每个批次；它们的响应项被连接到同一个`reviewer`字符串下的一个`reviews[]`数组中。4. **审稿人标识符的稳定性。**使用固定字符串，如`"claude-opus-4.7"`，`"gpt-5.4"`,`"gemini-2.5-pro"`。schemas.md§10不变式#17在这个字段上的多数计算——一个错字无声地变成了第四个审稿人，并打破了2-of-3的多数检查。

5. * *输出。**使用§1.6 manifest包装器将所有Council成员的响应连接到`quality/citation_semantic_check.json`，除了记录数组被命名为`reviews`而不是`records`（schemas.md§9.1）。每次运行一个文件，在每次审核通过时重新生成。

**多数决定原则（门强制）。**对于每个Tier1/2REQ，门组由`req_id`进行评审，当3名评审者中有2人记录到`verdict == "overreaches"`时，门组运行失败。单个成员的`overreaches`或`unclear`判决表面作为警告，但不会通过门。少于三个审阅者条目（缺少审阅者，跳过批次）的REQ没有足够的证据-门将其视为失败。**无操作的Spec Gap运行。**如果运行产生0层1/2req，`citation_semantic_check.json`仍然用一个空的`reviews`数组来写——文件的存在是工件契约的一部分，即使检查没有什么要评估的。

规范审计后回归测试

在规范审计分类之后，检查PROGRESS.md中的累积BUG跟踪器。任何没有回归测试的规范审计BUG现在都需要一个回归测试。为规范审计确认的代码bug编写回归测试，使用与代码审查回归测试相同的约定（预期失败标记、测试查找对齐、可执行源文件）。

**代码审查错误会立即进行回归测试，因为测试是在审查之后编写的。规范审计在编写测试之后运行，因此已确认的bug是孤立的——它们出现在分类报告中，但从未得到测试。这一步缩小了差距。**单个审计员工件（必须）。**规范审计必须在`quality/spec_audits/`生成单独的审计报告文件，文件名包含`auditor`（规范格式：`YYYY-MM-DD-auditor-N.md`，例如：`2026-04-12-auditor-1.md`；也接受：`auditor_<model>_<date>.md`）。`*auditor*`的gate globs -任何一致性名称都将匹配。每个审核员一个文件，而不仅仅是分类合成。每一份审计报告都记录了审计人员在分类核对前独立发现的内容。如果只有分类文件存在，而没有单独的审计员工件，则审计是不完整的—无法验证分类，因为没有预调节结果的记录。这一要求的存在是因为单个分类文件将发现与核对合并在一起，因此无法判断发现是独立确认的还是从单个来源合成的。**第4阶段完工门。**在`quality/spec_audits/YYYY-MM-DD-triage.md`**中存在分类文件并且存在**个单独的审计员报告之前，阶段4才算完成。如果只有审计报告存在而没有分类合成，则在PROGRESS.md中将阶段4标记为“部分分类待处理”，并在继续之前完成分类。如果只有分类存在而没有单独的报告，那么将阶段4标记为“部分审计员工件丢失”并重新生成它们。在确认分类文件和审计报告都存在之前，不能设置PROGRESS.md复选框。

用回归测试引用更新BUG跟踪条目。标志阶段4（规格审核+分类）完成。

**阶段结束消息（强制性-在阶段4完成后打印此消息，然后停止）：**```
# Phase 4 Complete — Spec Audit

The Council of Three spec audit is done. [Summarize: N auditors ran, N net-new bugs
confirmed from triage, total bugs now at N. List any new bug IDs and summaries.]

To continue to Phase 5 (Reconciliation — TDD verification, writeups, closure), say:

    Run quality playbook phase 5.

Or say "keep going" to continue automatically.
```
**打印此消息后，请停止。除非用户明确要求，否则不要进入第五阶段。**

---

阶段5：审查后核对和结案验证

**v1.5.6 instrumentation:**现在附加`phase_start phase=5`。对于每个门检查，附加`gate_check gate_name=X verdict=pass|fail|warn|skip`。在阶段结束时，交叉验证（`quality/results/quality-gate.log`非空），然后附加`phase_end phase=5`。**源代码编辑护栏（必选）。第5阶段在`quality/patches/<BUG-NNN>-fix.patch`和`quality/patches/<BUG-NNN>-regression-test.patch`产生*建议的*修复作为补丁工件。阶段5不能将这些补丁应用于`quality/`以外的源文件。改变目标源树的自我审计运行是一种缺陷，而不是合法的第5阶段输出——操作员在一个单独的、受监督的步骤中选择何时应用补丁。在运行结束时，剧本调用`bin.run_state_lib.validate_no_source_edits(target_dir)`；如果该helper报告任何非`quality/`路径脏，则附加一个`error recoverable:false`事件，引用违规，并使用`run_end status=aborted`结束运行。在Codex引导运行2026-05-02在第5阶段脱轨并编辑了`quality/`之外的五个源文件之后，该规则在v1.5.6中得到了重申。> **此阶段所需的引用：**
> -`quality/PROGRESS.md`-累积BUG跟踪器（权威查找列表）
> -`references/challenge_gate.md`-假阳性检测的两轮挑战协议
> -`references/requirements_pipeline.md`-审查后对账流程
> -`references/review_protocols.md`-回归测试清理逆转后
> -`references/spec_audit.md`-冲突验证探测协议

**第5阶段入口门（强制-硬停止）。**在继续之前，请确认以下所有第4阶段工件的存在：

1. 存在`quality/spec_audits/`目录，并包含至少一个`*triage*`文件（分类合成）
2.`quality/spec_audits/`包含至少一个`*auditor*`文件（单个审计员报告）
3. 存在`quality/PROGRESS.md`，其第4期线标记为`[x]`如果其中任何一个缺失，停止并返回到第4阶段。在确认规范审核工件之前不要进行核对——没有分类数据的核对会产生不完整的结束报告。

重新阅读`quality/PROGRESS.md`-特别是累积错误跟踪器。这是代码审查和规范审计中所有发现的权威列表。

**挑战门（和解前必须）。**在运行闭包验证之前，对每个匹配自动触发模式的已确认bug应用挑战门。阅读`references/challenge_gate.md`获取完整协议。总而言之:1. 扫描BUG跟踪器，寻找与任何自动触发模式匹配的BUG（安全类发现、在引用位置带有设计决策注释的代码、没有规范基础的发现、以不同方式处理相同关注点的兄弟代码路径、关于缺失功能的发现）。
2. 对于每个触发的bug，使用参考中描述的新子代理运行两轮挑战。
3. 记录判决在`quality/challenge/BUG-NNN-challenge.md`。
4. 应用裁决：确认的bug正常进行。降级bug的严重性得到调整。被拒绝的BUG将从BUG跟踪器中删除，并重新定位到BUGS.md中的“已审查并驳回”附录中，并给出挑战推理。**始终运用常识。**挑战门的主要目的是捕捉模式匹配压倒判断的发现。如果一个bug会让你把它报告给上游维护者看起来很愚蠢——一个自我记录的占位符被标记为一个严重的漏洞，一个记录的设计决策被标记为一个缺陷，一个故意的特性差距被标记为一个安全漏洞——它不应该在挑战中幸存下来。常识测试不是众多因素中的一个；这是整个审查的框架。**为什么这个门存在：**在v1.4.6 edquake基准测试中，代码审查确认了42个bug，其中7个被评为CRITICAL。在手工检查之后，最强的发现（BUG-001， source_ids覆盖）是HIGH，而不是CRITICAL。六个“关键的”租户隔离错误用显式的WHY-OODA81注释记录了特性缺口。一个“关键的”JWT发现（BUG-041）是一个包含文字字符串“change-me-in-production”的自文档开发占位符。该模型通过多轮推回来捍卫这些发现，因为它的本能是发现并捍卫bug，而不是应用构成缺陷的常识。挑战之门迫使常识性审查在调查结果最终确定之前进行。1. **运行`references/requirements_pipeline.md`中描述的Post-Review Reconciliation**。更新COMPLETENESS_REPORT.md。
2. **运行闭包验证：**对于BUG跟踪器中的每一行，验证它有回归测试引用或显式豁免。如果有BUG两者都不具备，那么现在就编写测试或豁免。
3. **Triage-to-BUGS.md同步门（必选）。**重新阅读分诊报告（`quality/spec_audits/*-triage.md`）。对于每一个确认为代码错误的发现，验证它是否出现在`quality/BUGS.md`中。如果BUGS.md不存在，现在创建它。如果BUGS.md存在，但在分类中缺少已确认的bug，则附加它们。带有已确认的代码错误而没有相应的BUGS.md条目的分类报告是不一致的—在它们被同步之前，该阶段不能被标记为完成。这个门的存在是因为在v1.3.21基准测试中，javalin的分类确认了2个bug，但BUGS.md从未创建过。
4. **清理spec-audit反转后：**如果spec-audit reclassi确认任何代码审查BUG为设计选择或误报，删除或重新定位每个`references/review_protocols.md`对应的回归测试。
5. **解决CR与规范审计的冲突：**如果代码审查和规范审计在相同的发现上有分歧（一个说是BUG，另一个说是设计选择），部署每个`references/spec_audit.md`的验证探针，并在BUG跟踪器中记录解决方案。**TDD侧车到日志一致性检查（必选）。**对于`tdd-results.json`中的每个bug条目，验证相应的日志文件是否存在并同意。如果`tdd-results.json`包含`verdict: "TDD verified"`的bug，那么`quality/results/BUG-NNN.red.log`必须以第一行`RED`存在，`quality/results/BUG-NNN.green.log`必须以第一行`GREEN`存在。如果侧车声明“TDD已验证”，但不存在红色阶段日志，则判定是未经证实的——要么通过运行测试创建日志，要么将判定降级为`"confirmed open"`。这个检查之所以存在，是因为v1.3.46基准测试显示代理在没有执行测试的情况下，根据叙述推理在JSON中编写“TDD验证”的结论。**执行的证据高于叙事的人工制品（矛盾门）。**在运行终端门之前，检查执行的证据和散文工件之间的矛盾。执行的证据包括：机械验证工件（`quality/mechanical/*`）、验证收据文件（`quality/results/mechanical-verify.log`、`quality/results/mechanical-verify.exit`）、回归测试结果（`test_regression.*`和`xfail`结果）、TDD红阶段日志文件（`quality/results/BUG-NNN.red.log`），以及管道期间保存的任何shell命令输出。散文工件包括：`REQUIREMENTS.md`、`CONTRACTS.md`、代码审查、规范审计分类和`BUGS.md`。如果执行的工件显示常量不存在（机械检查），测试失败（回归测试），或者红色阶段确认了一个bug （TDD可追溯性）——但是一个散文工件声称常量存在，bug已经修复，或者代码是兼容的——执行的结果获胜。在继续之前重新打开并纠正矛盾的散文神器。Speci官方的：如果`mechanical-verify.exit`包含非零值，PROGRESS.md可能不会声明“机械验证：通过”，并且终端门可能不会通过-无论任何其他工件说什么。在v1.3.18中，分类声称RING_RESET保留（`spec_audits/triage.md`），BUGS.md声称“在工作树中固定”，但是TDD可追溯性显示断言`assert "case VIRTIO_F_RING_RESET:" in func`在当前源上失败。这三条不可能都是真的——执行失败才是根本的事实。这扇门会抓住这个矛盾。**版本戳一致性检查（必选）。**从SKILL.md元数据中读取`version:`字段（使用参考文件解析顺序）。然后检查每个生成的工件：PROGRESS.md的`Skill version:`字段、每个`> Generated by`归属行、每个代码文件头戳和每个侧车JSON`skill_version`字段。每个版本戳必须与SKILL.md元数据完全匹配。单个不匹配是基准测试失败-在继续之前修复戳。这种检查的存在是因为在v1.3.21基准测试中，由于PROGRESS.md模板包含硬编码的版本号，9个repos中有5个具有来自较旧技能版本（v1.3.16或v1.3.20）的版本戳。**机械目录一致性检查。**如果存在`quality/mechanical/`，必须至少包含一个`verify.sh`文件。一个空的`quality/mechanical/`目录是不一致的——它意味着尝试了这个步骤，但是放弃了。如果在这个项目的范围内不存在分派函数契约，那么根本就不要创建`mechanical/`目录。相反，在PROGRESS.md中记录：`Mechanical verification: NOT APPLICABLE — no dispatch/registry/enumeration contracts in scope.`如果分派契约确实存在，`verify.sh`必须在`quality/mechanical/`下每个保存的提取文件中包含一个验证块（而不仅仅是一个）。当存在多个工件时，只检查一个工件的verify.sh是不完整的。

**验证收据门（必须在终端门前）。**如果`quality/mechanical/`存在，终端闸门运行前必须存在以下收据文件：
-`quality/results/mechanical-verify.log`-完整的stdout/stderr从`bash quality/mechanical/verify.sh`-`quality/results/mechanical-verify.exit`-单行包含退出代码（例如，`0`）如果其中一个文件丢失，现在运行`bash quality/mechanical/verify.sh > quality/results/mechanical-verify.log 2>&1; echo $? > quality/results/mechanical-verify.exit`。如果出口代码不是`0`，则终端门失败-在解决机械不匹配之前不要继续进行（通过修复提取，而不是通过编辑verify.sh或收据）。除非`mechanical-verify.exit`中包含`0`，否则PROGRESS.md不得声明“机械验证：通过”。这个门的存在是因为v1.3.23PROGRESS.md声称当verify.sh实际返回出口1时所有的验证都通过了—收据文件使这个声明可审计。**TDD日志关闭门（必须在终端门之前）。**在进入终端大门之前，从`quality/BUGS.md`中枚举所有确认的bug id并验证：
1.`quality/results/BUG-NNN.red.log`存在于每一个确认的bug。
2. 如果该bug存在`quality/patches/BUG-NNN-fix.patch`，则`quality/results/BUG-NNN.green.log`也存在。
3. 每个日志文件的第一行是：`RED`、`GREEN`、`NOT_RUN`、`ERROR`。
如果任何检查失败，现在就停止并使用来自TDD执行强制部分的语言感知测试执行命令生成缺失的日志。不要在缺少TDD日志的情况下进入终端大门-tdd-results.json中有“TDD验证”判决的bug，但没有相应的红阶段日志是矛盾的。

**终端门（在标记第5阶段完成之前必须）：****前提检查：**终端闸可能只运行，如果阶段3（代码审查）和阶段4（规范审核）都完成，或明确标记跳过的理由在PROGRESS.md。只有当代码审查和规范审计工件存在时（例如，`quality/code_reviews/`和`quality/spec_audits/`目录包含报告文件），零bug结果才有效。如果这些工件缺失，并且阶段没有明确跳过，则终端门失败—不标记阶段5完成。**BUGS.md总是必需的。**每次完成的运行必须产生`quality/BUGS.md`，无论是否发现错误。如果代码审查和规范审计确认没有源代码错误，则创建BUGS.md，其中BUGS.md声明“未发现已确认的源代码错误”，并列出评估和消除了多少候选项（例如，“代码审查评估了N个候选项；规范审计评估了M个候选项；所有候选项都被重新分类为设计选择、仅测试问题或误报”）。这提供了一个明确的结果断言，而不是模棱两可的文件缺失。没有BUGS.md的已完成运行是不合格的。**BUGS.md标题格式**每个确认的bug必须使用标题级别`### BUG-NNN`（例如，`### BUG-001`或`### BUG-H1`）。数字id （`BUG-001`）和带严重前缀的id （`BUG-H1`,`BUG-M3`,`BUG-L6`）都有效。这是规范的标题格式——不是`## BUG-001`，也不是`**BUG-001**`，不是一个项目符号。`### BUG-NNN`标题是下游工具在计算bug时查找的内容，也是tdd-results.json`id`字段必须匹配的内容。不一致的标题级别导致机器可读计数与文档不一致。

重读`quality/PROGRESS.md`。计算BUG跟踪条目。然后:

1. 打印以下语句给用户（这是强制性的，不是可选的）：

> "BUG跟踪器有N个条目。N个有回归测试，N个有豁免，N个没有解决。代码审查确认了M个bug。规范审核确认了K代码bug （L net-new）。预计总数：M + l。2. 将相同的语句写入PROGRESS.md下的新`## Terminal Gate Verification`节（紧接在BUG跟踪表之后）。这将gate持久化到工件中，这样审阅者就可以在不读取会话日志的情况下验证它。

如果跟踪器条目计数不等于M + L，则停止并协调——跟踪器孤立了一个BUG。在计数匹配之前不要标记第5阶段完成。这个门的存在是因为v1.3.5引导显示代理在规范审核后可靠地跳过跟踪器更新，导致30-50%的已确认错误丢失。

**回归测试函数名验证：**对于引用回归测试的每个BUG跟踪条目，在回归测试文件中查找测试函数名并确认它存在。代理可以在跟踪器中编写测试名称，而无需实际创建测试。如果任何引用的测试函数不存在，在通过门之前编写它。3. 验证PROGRESS.md中的`With docs`元数据字段是否符合现实：如果`reference_docs/`存在并且包含文件，则应该显示`yes`；否则`no`。如果错了，就把它修好。

**工件文件存在门（在标记阶段5完成之前必须）。**在写入阶段5完成复选框之前，请确认每个所需的工件都以文件的形式存在于磁盘上——而不仅仅是在PROGRESS.md中提到的。运行这些检查（使用`ls`或等效）：存在`quality/BUGS.md`（所有完成的运行都需要，每个基准34）
—`quality/REQUIREMENTS.md`已存在
—`quality/QUALITY.md`已存在
-`quality/PROGRESS.md`是存在的（很明显-你在给它写信）
—`quality/COVERAGE_MATRIX.md`已存在
—`quality/COMPLETENESS_REPORT.md`已存在
-`quality/formal_docs_manifest.json`存在（v1.5.3 -由`bin/reference_docs_ingest.py`在第一阶段编写；空`records[]`在没有正式文档时有效）
-`quality/requirements_manifest.json`存在（v1.5.3 -权威REQ记录，呈现为REQUIREMENTS.md）
-`quality/use_cases_manifest.json`存在（v1.5.3 -权威UC记录，呈现为USE_CASES.md/REQUIREMENTS.md叙述）
-`quality/citation_semantic_check.json`存在（v1.5.3 -第4阶段第2层输出；空`reviews[]`在Spec Gap运行时有效）
—如果“Phase 3:`quality/code_reviews/`”包含至少一个“`.md`”文件
—如果阶段4运行：`quality/spec_audits/`包含一个分类文件和单个审计员文件
-如果阶段0或阶段b运行：`quality/SEED_CHECKS.md`作为独立文件存在（未内联在PROGRESS.md中）
-如果确认bug存在：`quality/bugs_manifest.json`存在(v1.5.3 -权威BUG记录（按schemas.md§8）
-如果确认bug存在：`quality/results/tdd-results.json`存在
—如果已确认的bug存在：`quality/results/BUG-NNN.red.log`中每个已确认的bug ID都存在
-如果确认的bug存在，修复补丁：`quality/results/BUG-NNN.green.log`对应每个有`quality/patches/BUG-NNN-fix.patch`的bug对于每个丢失的文件，现在创建它。不要用缺失的工件标记第5阶段完成——如果PROGRESS.md中引用的文件在磁盘上不存在，那么终端门验证就没有意义。这个门的存在是因为v1.3.24基准测试显示express完成了PROGRESS.md中的所有阶段并写入了终端门部分，但是BUGS.md、SEED_CHECKS.md和代码review/spec审计文件从未写入磁盘。**Sidecar JSON写后验证（必选）。**写入`quality/results/tdd-results.json`and/or`quality/results/integration-results.json`后，立即重新打开每个文件并验证它包含所有所需的密钥。对于`tdd-results.json`，所需的根密钥为：`schema_version`、`skill_version`、`date`、`project`、`bugs`、`summary`。`bugs`中的每个条目必须有：`id`、`requirement`、`red_phase`、`green_phase`、`verdict`、`fix_patch_present`、`writeup_path`。`summary`对象必须包括`confirmed_open`以及`verified`、`red_failed`、`green_failed`。对于`integration-results.json`，所需的根密钥为：`schema_version`、`skill_version`、`date`、`project`、`groups`、`summary`、`uc_coverage`。两个文件都必须是`schema_version: "1.1"`。如果缺少任何键，现在就添加它——不要在磁盘上留下不一致的JSON文件。之所以存在这种验证，是因为v1.3.25的基准测试显示，在8个版本中有6个版本存在不一致的侧车JSON: httpx发明了一种替代模式，serde使用了遗留模式，javalin使用了JSON支持`summary`和per-bug字段，以及其他使用无效枚举值的字段。**经过脚本验证的关闭门（强制性的，标志阶段5完成前的最后一步）。**使用与参考文件相同的回退来定位`quality_gate.py`-按顺序遍历这六个规范的安装布局，首先执行：`quality_gate.py`，`.claude/skills/quality-playbook/quality_gate.py`,`.github/skills/quality_gate.py`,`.cursor/skills/quality-playbook/quality_gate.py`,`.continue/skills/quality-playbook/quality_gate.py`,`.github/skills/quality-playbook/quality_gate.py`。从项目根目录运行它。这个脚本机械地验证：文件存在，BUGS.md标题格式，sidecar JSON所需的键和每个错误字段名称（`id`,`requirement`,`red_phase`,`green_phase`,`verdict`,`fix_patch_present`,`writeup_path`）和enum值和摘要一致性，用例标识符，终端门部分，机械验证收据，版本戳，writeup完整性，**每个确认错误的回归测试补丁存在**，以及**每个writeup中的内联修复差异**（每个`quality/writeups/BUG-NNN.md`必须包含` `'`diff `块）。如果脚本报告任何FAIL结果，fi最常见的失败是：(1)缺少`quality/patches/BUG-NNN-regression-test.patch`文件，(2)非规范的JSON字段名，如`bug_id`而不是`id`，(3)在TDD摘要中缺少`confirmed_open`，(4)没有内联修复差异的编写（第6节必须包括一个具体的差异，而不仅仅是“查看补丁文件”）。在`quality_gate.py`退出0之前，不要标记阶段5完成。将脚本的完整输出附加到`quality/results/quality-gate.log`。**v1.5.3第1层机械检查（schemas.md§10不变量#1 - #18）。**除了上面的遗留门检查之外，v1.5.3中的`quality_gate.py`还强制执行`schemas.md`§10中定义的Layer-1不变量。每个不变量所覆盖的紧凑映射：- **#1 - #10 -核心合同检查。**引文层控制，引文文档存在，引文散列匹配，引文摘录存在+可定位性（仅section/line；页面永远不够），bug→REQ解析，前向链接解析，配置完整性，功能部分存在，无孤儿正式文档，INDEX.md字段存在。
- **#11 -引文摘录字节相等。**闸门根据schemas.md§5.4重新运行`bin/citation_verifier.extract_excerpt`，并拒绝任何存储的不等于新提取的`citation_excerpt`。这是第一层防幻觉机制——即使定位器是真实的，它也能捕捉到捏造或改写的摘录。
- **#12 -合法的`fix_type × disposition`组合**根据schemas.md§3.4。
- **#13 -清单包装有效性**根据schemas.md§1.6
- **#14 - REQ层绑定到引用的FORMAL_DOC层**（一级REQ不能引用二级FORMAL_DOC）。
- * * # 15 -每个清单内的ID唯一性**。
- **#16 -冗余引用元数据** (`version`,`date`,`url`,`retrieved`)必须匹配存在时的FORMAL_DOC。
- **#17 -语义检查多数原则。**对于同一层1/2REQ， 3个`overreaches`判定中≥2个不通过门（见阶段4的第2层子通）。
- **#18 -数组值唯一性**在`REQ.use_cases`和`UC.formal_doc_refs`。**`citation_stale`是gate-report标记，而不是引文记录中的字段。**当存储的`citation.document_sha256`与活动的`FORMAL_DOC.document_sha256`偏离时，`quality_gate.py`将一个`citation_stale`条目写入`quality_gate_report.json`（或等效的）。不要将`citation_stale`写入引用记录本身-记录保持纯输入，并且陈旧标记是根据schemas.md§5.1 /§10不变式#3的门报告输出。

**不要在这篇文章中实现门。**上面的第1层检查列表是`quality_gate.py`强制的摘要-权威定义存在于schemas.md中。门的实现（v1.5.3实现的第5阶段）位于`quality_gate.py`；SKILL.md描述了协议，但没有重新声明不变量。**用例标识符格式。**REQUIREMENTS.md必须对所有派生用例使用格式为`UC-01`、`UC-02`等的规范用例标识符。每个用例必须用它的标识符来标记。这对于机器可读的可追溯性是必需的——标识符格式使`quality_gate.py`和下游工具能够以编程方式计数和交叉引用用例。用例写成没有标识符的散文段落是不一致的。

更新PROGRESS.md：标记第五阶段完成。BUG跟踪器现在应该显示每个条目的关闭状态。

**阶段结束消息（强制性-在第5阶段完成后打印此消息，然后停止）：**```
# Phase 5 Complete — Reconciliation and TDD Verification

All confirmed bugs now have regression tests, writeups, and TDD red-green verification.
[Summarize: N total confirmed bugs, N with TDD verified status, N with fix patches.
List all bug IDs with one-line summaries and their TDD verdicts.]

To continue to Phase 6 (Final verification and quality gate), say:

    Run quality playbook phase 6.

Or say "keep going" to continue automatically.
```
**打印此消息后，请停止。除非用户明确要求，否则不要进入第6阶段

---

阶段6：验证

**v1.5.6 instrumentation:**现在附加`phase_start phase=6`。在阶段结束时，交叉验证（`quality/BUGS.md`非空与`^## BUG-`段和`quality/INDEX.md`更新与`gate_verdict`字段），然后附加`phase_end phase=6`。在阶段6结束后，追加`run_end status=success`（或`aborted`/`failed`，如果适用）。

> **此阶段所需的引用：**
> -`references/verification.md`- 45自检基准**为什么是验证阶段？** ai生成的输出可能看起来很精致，但也可能有细微的错误。引用未定义fixture的测试报告0个失败，但有16个错误——“0个失败”听起来像是成功。集成协议可以列出实际模式中不存在的字段名。验证阶段在用户发现问题之前捕获这些问题，这很重要，因为对生成的质量剧本的信任是脆弱的——一个错误的字段名称会破坏对其他所有内容的信任。第6阶段执行模型：增量的，而不是整体的。**阶段6作为一系列独立的验证步骤运行，每个步骤只读取所需的文件，检查一件事，并在移动到下一步之前将其结果写入`quality/results/phase6-verification.log`。不要一次将所有工件加载到上下文中。在读取工件时，不要试图在内存中保存完整的验证检查表。下面的每一步都是独立的——读取文件、检查条件、附加结果、删除上下文。

###步骤6.1：机械验证关闭（强制性的第一步）

如果`quality/mechanical/`存在，则阶段6的第一个动作**是：```bash
bash quality/mechanical/verify.sh > quality/results/mechanical-verify.log 2>&1
echo $? > quality/results/mechanical-verify.exit
```
在shell中执行该命令。不要替换Python脚本，不要读取工件文件并断言其内容，不要跳过此步骤。该命令必须是`bash quality/mechanical/verify.sh`—不是`python3 -c "..."`，不是`cat quality/mechanical/... | grep ...`，也不是任何其他等价的命令。

记录退出代码。如果非零，**阶段6立即失效。**不要进行进一步的步骤。回到提取步骤：删除不匹配的`*_cases.txt`，用新的shell重定向重新运行提取命令，重新验证，并更新引用旧提取的所有下游工件。

在PROGRESS.md中记录在`## Phase 6 Mechanical Closure`下，并附加到`quality/results/phase6-verification.log`：```
[Step 6.1] Mechanical verification: PASS (exit 0)
```
**为什么不可替换：**在v1.3.23中，模型用读取（伪造的）工件文件并断言其内容的`python3 -c "from pathlib import Path; ..."`替换了`bash verify.sh`——尽管工件是伪造的，但仍然通过了循环检查。唯一可靠的验证是重新运行产生工件的相同shell管道并更改结果。任何其他方法都可能被损坏的中间文件所欺骗。

###步骤6.2：运行quality_gate.py（脚本验证检查）

运行机械验证门：```bash
python3 quality_gate.py . > quality/results/quality-gate.log 2>&1  # locate via fallback (six canonical layouts, in order): quality_gate.py, .claude/skills/quality-playbook/quality_gate.py, .github/skills/quality_gate.py, .cursor/skills/quality-playbook/quality_gate.py, .continue/skills/quality-playbook/quality_gate.py, .github/skills/quality-playbook/quality_gate.py
echo $? >> quality/results/phase6-verification.log
```
读`quality/results/quality-gate.log`。如果它报告任何FAIL结果，在继续之前修复每个失败检查。最常见的失败是：(1)缺少`quality/patches/BUG-NNN-regression-test.patch`文件，(2)非规范的JSON字段名，如`bug_id`而不是`id`，(3)在TDD摘要中缺少`confirmed_open`，(4)没有内联修复差异的编写，(5)缺少TDDred/green日志文件。在`quality_gate.py`退出0之前不要继续。

附加到`quality/results/phase6-verification.log`：```
[Step 6.2] quality_gate.py: PASS (exit 0) — N checks passed, 0 FAIL, 0 WARN
```
此步骤涵盖验证基准：14（侧车JSON）、17（测试文件扩展名）、18（用例计数）、20（写入）、23（机械工件）、26（版本戳）、27（机械目录）、29（分类到bug同步）、34 （BUGS.md存在）、38（单个审计员报告）、39 （BUGS.md标题格式）、40（工件文件存在）、41（侧车JSON验证）、42（脚本验证闭包）、43（用例标识符）、44（回归测试补丁）、45（写入内联差异）。**v1.5.3 Layer-1不变量也在这里运行。**`quality_gate.py`额外执行schemas.md§10不变量#1 - #18（总结在上面的第5阶段）。特别是，脚本根据schemas.md§5.4对每个1/2引用重新运行`bin/citation_verifier.extract_excerpt`，并拒绝任何存储的不等于新提取输出字节的`citation_excerpt`—这是摄取后篡改捕获。如果任何第1层不变式在此失败，则修复底层清单记录（不是门，不是摘录）并重新运行。

步骤6.3：测试执行验证

运行功能测试套件。读取`quality/test_functional.*`来确定test命令：

- **Python:**`pytest quality/test_functional.py -v 2>&1 | tail -20`- **Java:**`mvn test -Dtest=FunctionalTest`或`gradle test --tests FunctionalTest`**Go:**`go test -v`瞄准生成的测试文件的包
**TypeScript:**`npx jest functional.test.ts --verbose`- **Rust:**`cargo test`- **Scala:**`sbt "testOnly *FunctionalSpec"`检查失败和错误。缺少fixture、导入失败或未解决的依赖项所导致的错误将被视为失败的测试。预期失败（xfail）回归测试不计入此检查。

附加到`quality/results/phase6-verification.log`：```
[Step 6.3] Functional tests: PASS — N tests, 0 failures, 0 errors
```
这包括基准测试8（所有测试都通过）和9（现有测试未中断）。

步骤6.4：验证检查表-逐个文件检查

以小批量处理`references/verification.md`中剩余的验证基准。对于每个批处理，只读取所需的文件，检查条件，并附加结果。**每批读取的文件不要超过2个

**批A -QUALITY.md（基准测试1- 2,10）：**读取`quality/QUALITY.md`。计算场景。验证每个场景引用了实际代码（用grep查看引用的函数名）。附加结果。

**批B -功能测试文件（基准3-7）：**读取`quality/test_functional.*`。检查跨变量覆盖率（~30%）、边界测试计数、断言深度（值检查vs存在检查）、层正确性（结果vs机制）、突变有效性。**批处理C -协议文件（基准11-13）：**读取`quality/RUN_CODE_REVIEW.md`，然后`quality/RUN_INTEGRATION_TESTS.md`，然后`quality/RUN_SPEC_AUDIT.md`-一次一个。检查每一个都是独立的和可执行的。验证集成测试中的字段参考表。

**批D -回归测试（基准15- 16,24）：**读取`quality/test_regression.*`，如果它存在。验证跳过守卫参考错误id，验证补丁验证门命令，验证源代码检查测试不使用`run=False`。

**批E -枚举和分类检查（基准19,21 - 22,25,36）：**读取`quality/code_reviews/*.md`（只是枚举部分）。读取`quality/spec_audits/*triage*`（只是验证探测部分）。检查双表比对，可执行探测证据，无圆形机械工件参考，矛盾门。

**批F -延续模式（基准32-33）：**仅当`quality/SEED_CHECKS.md`存在时。阅读它，验证机械执行，验证收敛部分在PROGRESS.md。将每个批处理结果附加到`quality/results/phase6-verification.log`：```
[Step 6.4A] QUALITY.md scenarios: PASS — 8 scenarios, all reference real code
[Step 6.4B] Functional test quality: PASS — 30% cross-variant, assertion depth OK
[Step 6.4C] Protocol files: PASS — all self-contained and executable
[Step 6.4D] Regression tests: PASS — all skip guards present
[Step 6.4E] Enumeration/triage: PASS — two-list checks present, probes have assertions
[Step 6.4F] Continuation mode: SKIP — no SEED_CHECKS.md
```
如果任何批处理失败，在进行下一批处理之前立即修复问题。

步骤6.5：元数据一致性检查

读取`quality/PROGRESS.md`（只是元数据和工件目录部分）。然后抽查:
-需求计数在REQUIREMENTS.md头、PROGRESS.md工件库存和COVERAGE_MATRIX.md头之间是一致的。所有三个必须陈述相同的数字。
—`With docs`字段准确反映`reference_docs/`是否存在
-终端门验证部分已填写

然后阅读`quality/COMPLETENESS_REPORT.md`（只是判决部分）。验证没有过时的预调和文本-如果同时存在`## Verdict`和`## Updated verdict`（或`## Post-Review Reconciliation`）节，则完全删除原始的`## Verdict`节**。最终文档必须只有一个`## Verdict`标题。

附加到`quality/results/phase6-verification.log`：```
[Step 6.5] Metadata consistency: PASS — requirement counts match, version stamps consistent
```
如果任何元数据过时，请立即修复它。

检查点：完成PROGRESS.md重读`quality/PROGRESS.md`。更新:
-标记阶段6（验证基准）完成时间戳
-验证BUG跟踪器对每个条目都有关闭
-添加最后的总结行：“运行完成。发现N个bug （N个来自代码审查，N个来自规范审核）。写了N个回归测试。给予N个豁免。”
- **向用户打印建议的下一个提示（必选，所有运行）。**这适用于每次运行，包括基线-它不是迭代特定的。打印下面的代码块，这样用户就可以复制粘贴它来开始下一次迭代：

对于基线运行（没有迭代策略）：  ```
  ────────────────────────────────────────────────────────
  Next iteration suggestion:
  "Run the next iteration of the quality playbook using the gap strategy."
  ────────────────────────────────────────────────────────
  ```
对于迭代运行，使用此映射来确定下一个策略：
- **间隙**→建议不过滤
- **未过滤**→建议校验
- **奇偶性**→建议对抗
- **对抗**→建议“从头开始运行质量剧本。”(周期完成)

完成的PROGRESS.md是一个永久的审计跟踪。它记录了该技能做了什么，发现了什么，以及如何解决每个发现。用户可以阅读它来理解运行、调试故障，并在不同运行之间进行比较。

收敛性检查（仅限延续模式）

> **适用范围：**仅适用于本小节。上述建议的下一个提示步骤是无条件的，无论是否跳过此收敛检查，每次运行都必须执行。

**此步骤仅在阶段0执行时运行**（即，从先前运行的分析中存在`quality/SEED_CHECKS.md`）。如果这是第一次运行，没有先前的历史，跳到第7阶段。将此运行的bug列表与种子列表进行比较：

1. **计算net-new bug:**运行的BUGS.md中不匹配任何种子（通过file:line）的bug。如果在之前的运行中没有发现错误，则该错误为“net-new”。
2. **计数种子结转：在此运行中重新确认的**种子（在步骤0b中失败结果）。
3. **计数种子分辨率：**种子现在正在通过（错误已修复自上次运行）。

将`## Convergence`段写入PROGRESS.md：```markdown
## Convergence

Run number: N (N prior runs in quality/previous_runs/)
Seeds from prior runs: S (S confirmed, R resolved)
Net-new bugs this run: K
Convergence: [CONVERGED | NOT CONVERGED]

Net-new bugs:
- BUG-NNN: [summary] (file:line) — not in any prior run
```
**收敛准则：**如果**net-new bugs = 0，则运行是收敛的** -在此运行中发现的每个bug都已经从先前的运行中知道。这意味着进一步运行不太可能在声明的作用域中发现其他错误。

**如果聚合：**打印给用户：“本次运行没有发现除之前运行中已知的N个之外的新错误。Bug发现已经汇聚到这个范围。所有运行中确认的bug总数为：t。”然后进入第7阶段。

**如果不收敛-自动重新迭代。**当收敛检查显示net-new bugs > 0且迭代计数未达到最大值（默认为5）时，技能自动重新迭代：1. 在PROGRESS.md中记录迭代次数和net-new计数。
2. 通过`bin/run_playbook.archive_previous_run(repo_dir, timestamp)`（或者在阶段6成功时使用`bin.archive_lib.archive_run()`）归档当前的`quality/`目录。它们将`quality/`快照到`quality/previous_runs/<timestamp>/quality/`中，并写入每次运行的`INDEX.md`加上`RUN_INDEX.md`行。
3. 从**阶段0**重新启动（现在将在`quality/previous_runs/`中找到新存档的运行）。
4. 打印给用户：“迭代N发现了K个网络新bug。存档和开始迭代N+1（最多M）。”

第一次运行时，迭代计数器从1开始。每次存档和重启都会增加它。当计数器达到最大值时，即使没有收敛也停止迭代，并打印：“达到最大迭代(M)，没有收敛。”在上次运行中发现的新bug。所有运行中确认的bug总数为：t。”* *迭代的限制。**默认最大值为5次迭代。如果用户的提示包含明确的限制（例如，“运行剧本3次迭代”），则使用该限制代替。如果用户提示“单次运行”或“不迭代”，则完全跳过重新迭代，并将未收敛视为与迭代前行为相同：打印净新计数并建议重新运行。**上下文窗口感知。**如果在重新迭代过程中的任何一点，您检测到上下文窗口基本上被消耗了（例如，您产生的输出明显比以前的迭代更短或质量更低），请停止迭代，将当前状态写入PROGRESS.md，并打印：“由于上下文约束而停止迭代。完成M次迭代中的N次重新运行剧本继续——阶段0将从quality/previous_runs/.中获取种子列表。“这是一个安全阀，而不是一个目标——大多数代码库在2-3次迭代中收敛。**为什么这很重要：**一个剧本运行探索一个子集的代码库不确定性。第一次在版本上运行可能会找到BUG-001和BUG-004，但找不到BUG-005。第二次运行可能会发现BUG-005和BUG-006。到第三次运行时，如果没有出现新的漏洞，则勘探可能已经覆盖了高价值领域。种子列表确保以前发现的错误不会在运行之间丢失，收敛性检查告诉用户何时额外运行的收益递减。自动重复迭代意味着技能是自包含的——调用者不需要外部脚本或手动重新运行来实现收敛。

**阶段结束消息（强制性-在第6阶段完成后打印此消息，然后停止）：**```
# Phase 6 Complete — All Phases Done

The quality playbook baseline run is complete. Here's the summary:

[Include: total confirmed bugs, quality gate pass/fail/warn counts,
list of all bug IDs with one-line summaries and severities.]

Key output files:
- quality/BUGS.md — all confirmed bugs with spec basis and patches
- quality/results/tdd-results.json — structured TDD verification results
- quality/patches/ — regression test and fix patches for every bug

You can now run iteration strategies to find additional bugs. Iterations typically
add 40-60% more confirmed bugs on top of the baseline. The recommended cycle is:
gap → unfiltered → parity → adversarial.

To run all four iterations automatically, say:

    Run all iterations.

I'll orchestrate each strategy as a separate sub-agent with its own context window.

To run one iteration at a time, say:

    Run the next iteration of the quality playbook.

Or ask me about the results: "Tell me about BUG-001" or "Which bugs are highest priority?"

After you fix the bugs, say "recheck" to verify the fixes were applied correctly.
```
**打印此消息后，请停止。除非用户明确要求，否则不要进行迭代

**迭代结束消息（强制-在每次迭代完成后打印此消息，然后停止）：**```
# Iteration Complete — [Strategy Name]

[Summarize: N net-new bugs found in this iteration, total now at N.
List new bug IDs with one-line summaries.]

[If there are remaining strategies in the recommended cycle, suggest the next one:]
The next recommended strategy is [next strategy]. To run it, say:

    Run the next iteration using the [next strategy] strategy.

[If all four strategies have been run:]
All four iteration strategies have been run. Total confirmed bugs: N.
You can review the results, ask about specific bugs, or re-run any strategy.

After you fix the bugs, say "recheck" to verify the fixes were applied correctly.

Or say "keep going" to run the next iteration automatically.
```
**打印此消息后，请停止。除非用户明确要求，否则不要进行下一次迭代

---

## Recheck模式-验证Bug修复

Recheck模式是一种轻量级验证通过，用于检查以前运行中的错误是否已修复。recheck不是重新运行完整的六阶段管道（60-90分钟），而是读取现有的`quality/BUGS.md`，根据当前源代码树检查每个bug，并报告哪些bug已经修复，哪些bug仍然打开。一次典型的复查需要2-10分钟。

**何时使用recheck模式：**在用户（或其他代理）对剧本发现的错误进行修复后。用户说“recheck”或“verify The bug fixes”或“check哪些bug被修复了”。

**不要使用复核模式**来代替运行完整的剧本。Recheck只验证以前发现的错误—它不会发现新的错误。

###重新检查程序步骤1：阅读漏洞清单

读取`quality/BUGS.md`并解析每个`### BUG-NNN`条目。对于每个bug，提取：
- Bug ID（例如Bug -001）
—`**File:**`字段中的文件路径和行号
-描述总结（`**Description:**`的第一句话）
——严重程度
修复`**Fix patch:**`字段的补丁路径（例如，`quality/patches/BUG-001-fix.patch`）
-回归测试路径从`**Regression test:**`字段

**步骤2：根据当前源代码检查每个bug

对于每个bug，按顺序执行以下检查：

1. **修复补丁检查。**如果在引用的路径上存在修复补丁，对当前树运行`git apply --check --reverse quality/patches/BUG-NNN-fix.patch`。如果反向应用成功（退出0），则修复补丁已经应用—错误可能已经修复。如果失败，则表示没有应用修复或代码已更改。2. * *检查来源。**按引用行号打开文件。阅读周围的上下文（±20行）。将您所看到的与bug描述进行比较。有问题的代码已经更改了吗？修复是否解决了bug报告中描述的根本原因？

3. **回归测试执行。**如果存在回归测试补丁：
—应用：`git apply quality/patches/BUG-NNN-regression-test.patch`-运行测试（使用项目的测试运行器）。如果测试通过，则错误被修复。如果失败，则错误仍然存在。
—反转补丁：`git apply -R quality/patches/BUG-NNN-regression-test.patch`如果回归测试补丁不能干净地应用（因为源代码已经更改），请注意这一点，并退回到源代码检查。4. * *裁决。**分配以下状态之一：
-修复补丁应用和回归测试通过（或源代码检查确认修复，如果测试不能运行）
**PARTIALLY_FIXED** -有问题的代码已经改变，但回归测试仍然失败，或者修复解决了一些但不是所有方面的bug
- **STILL_OPEN** -原始问题代码未改变，或者回归测试仍然失败
-无法确定状态（文件移动，代码严重重构，补丁不适用）

**步骤3：生成复核结果

用下面的模式编写`quality/results/recheck-results.json`：

注意：重新检查模式使用`"schema_version": "1.0"`（而不是`"1.1"`），因为它具有与TDD侧车不同的结构—`source_run`和每个bug的`status`/`evidence`字段对于重新检查模式是唯一的。质量检验关验证这个值为`"1.0"`。```json
{
  "schema_version": "1.0",
  "skill_version": "1.5.6",
  "date": "YYYY-MM-DD",
  "project": "<project name>",
  "source_run": {
    "bugs_md_date": "<date from BUGS.md header>",
    "total_bugs": <N>
  },
  "results": [
    {
      "id": "BUG-001",
      "severity": "HIGH",
      "summary": "<one-line summary>",
      "status": "FIXED",
      "evidence": "<what confirmed the fix — e.g., 'reverse-apply succeeded + regression test passes'>"
    }
  ],
  "summary": {
    "total": <N>,
    "fixed": <N>,
    "partially_fixed": <N>,
    "still_open": <N>,
    "inconclusive": <N>
  }
}
```
另外，在`quality/results/recheck-summary.md`中编写一个人类可读的摘要：```markdown
# Recheck Results

> Recheck of quality/BUGS.md from <date>
> Recheck run: <today's date>
> Skill version: <version>

## Summary

| Status | Count |
|--------|-------|
| Fixed | N |
| Partially fixed | N |
| Still open | N |
| Inconclusive | N |
| **Total** | **N** |

## Per-Bug Results

| Bug | Severity | Status | Evidence |
|-----|----------|--------|----------|
| BUG-001 | HIGH | FIXED | Reverse-apply succeeded, regression test passes |
| BUG-002 | MEDIUM | STILL_OPEN | Original code unchanged at quality_gate.py:125 |
| ... | ... | ... | ... |

## Still Open — Details

[For each STILL_OPEN or PARTIALLY_FIXED bug, include a brief explanation of what remains to be fixed.]
```
**第四步：打印复核摘要

将汇总表打印给用户，然后单击STOP。例子:```
# Recheck Complete

Checked 19 bugs from quality/BUGS.md against current source.

| Status | Count |
|--------|-------|
| Fixed | 17 |
| Still open | 2 |
| **Total** | **19** |

Fixed bugs: BUG-001, BUG-002, BUG-003, BUG-004, BUG-005, BUG-006, BUG-007,
BUG-008, BUG-009, BUG-010, BUG-011, BUG-013, BUG-014, BUG-015, BUG-016,
BUG-017, BUG-018

Still open: BUG-012 (stale .orig file still present), BUG-019 (benchmark 40
artifact list not updated)

Results saved to:
- quality/results/recheck-results.json (machine-readable)
- quality/results/recheck-summary.md (human-readable)
```
###触发重检模式

当用户说：“Recheck”、“verify the bug fixes”、“check哪些bug被修复了”、“Recheck the bug”、“run Recheck mode”或类似的措辞，明确表示他们想要验证修复而不是寻找新的bug时，Recheck模式就会激活。当触发时，完全跳过阶段1-7，只执行上面的复核过程。

---

第7阶段：呈现，探索，改进（互动）

在生成和验证之后，清晰地呈现结果，并让用户控制接下来发生的事情。这个阶段有三个部分：可扫描的摘要、按需深入挖掘和改进路径的菜单。**不要跳过此阶段。**阶段1-6的自主输出是一个坚实的起点，但用户需要了解生成了什么，探索对他们重要的是什么，并选择如何改进它。高质量的剧本只有在项目所有者信任并理解它的情况下才有用。在没有解释的情况下转储6个文件会产生无人阅读的工件。

第一部分：汇总表

呈现用户可以在10秒内扫描的单个表：```
Here's what I generated:

| File | What It Does | Key Metric | Confidence |
|------|-------------|------------|------------|
| REQUIREMENTS.md | Testable requirements with use cases | N requirements, N use cases | ██████░░ Medium — solid baseline from 5-phase pipeline, improves with refinement passes |
| QUALITY.md | Quality constitution | 10 scenarios | ██████░░ High — grounded in code, but scenarios are inferred, not from real incidents |
| Functional tests | Automated tests | 47 passing | ████████ High — all tests pass, 35% cross-variant |
| RUN_CODE_REVIEW.md | Three-pass code review | 3 passes | ████████ High — structural + requirement verification + consistency |
| RUN_INTEGRATION_TESTS.md | Integration test protocol | 9 runs × 3 providers | ██████░░ Medium — quality gates need threshold tuning |
| RUN_SPEC_AUDIT.md | Council of Three audit | 10 scrutiny areas | ████████ High — guardrails included |
| RUN_TDD_TESTS.md | TDD verification protocol | N bugs to verify | ████████ High — mechanical red-green cycle with spec traceability |
```
根据实际生成的内容调整表格——文件名、度量标准和置信度会因项目而异。信心栏是最重要的：它告诉用户应该把注意力集中在哪里。

* *信心水平:* *
- **高** -直接从代码，规范或模式派生。不太可能需要修改。
- **中等** -合理的推断，但可能是错误的。受益于用户输入。
- **低** -最佳猜测。绝对需要用户输入才有用。

在表之后，添加一个“Quick Start”块，其中包含执行每个工件的准备复制提示：```
To use these artifacts, start a new AI session and try one of these prompts:

• Run a code review:
  "Read quality/RUN_CODE_REVIEW.md and follow its instructions to review [module or file]."

• Run the functional tests:
  "[test runner command, e.g. pytest quality/ -v, mvn test -Dtest=FunctionalTest, etc.]"

• Run the integration tests:
  "Read quality/RUN_INTEGRATION_TESTS.md and follow its instructions."

• Start a spec audit (Council of Three):
  "Read quality/RUN_SPEC_AUDIT.md and follow its instructions using [model name]."

• Run TDD verification for confirmed bugs:
  "Read quality/RUN_TDD_TESTS.md and follow its instructions to verify all confirmed bugs."
```
根据实际项目调整测试运行器命令和模块名称。关键是给用户提供可复制粘贴的提示—不是描述他们可以做什么，而是他们要输入的实际文本。

在“快速开始”块之后，添加一行：

>“您可以向我询问这些问题中的任何一个，以查看细节——例如，‘向我展示场景3’或‘引导我完成集成测试矩阵’。”

第2部分：按需深入

当用户询问一个特定的项目时，给出一个有重点的总结——不是整个文件，而是关键的决定和你不确定的地方。例子:- **“告诉我关于场景4”**→显示场景文本，解释它来自哪里（哪种防御模式或领域知识），并标记你推断的与你知道的。
**“显示集成测试矩阵”**→显示运行组，解释并行策略，并注意您从模式中获得的质量检验关和猜测的质量检验关。
- **“功能测试是如何工作的？”**→显示三个测试组，解释到规格和场景的映射，并突出显示您最不自信的任何测试。

在准备改进任何东西之前，用户可能会进行多次深入研究。这很好——让他们按照自己的节奏去探索。

第3部分：改进菜单

在用户看到总结（并可选择深入细节）后，提出改进选项：b>“让它变得更好的五个方法：”
>
> * * 1。交互式地审查需求** -阅读`quality/REVIEW_REQUIREMENTS.md`，以获得按用例组织的需求的指导浏览。您可以选择特定的用例来深入研究，或者按顺序遍历所有用例。不同的模型也可以对完整性报告进行事实检查（跨模型审计）。优点：发现管道遗漏的间隙。
>
> * * 2。使用不同的模型细化需求** -读取`quality/REFINE_REQUIREMENTS.md`并运行细化过程。你可以在任何AI模型上运行这个——Claude， GPT， Gemini——每个模型都会捕捉到不同的间隙。运行尽可能多的模型，直到达到收益递减。每次传递都会备份当前版本并记录`quality/VERSION_HISTORY.md`中的更改。优点：将需求从基线推向完整性。
>
> * * 3。检查和强化其他项目** -选择任何场景、测试或协议部分，我将与您一起完成它。咕D for：收紧特定的质量闸门，固定推断的场景，添加缺失的边缘情况。
>
> * * 4。指导性问答**——我会问你3-5个有针对性的问题，这些问题是我无法从代码中推断出来的：事件历史、预期分布、成本容忍、模型偏好。优点：填补知识空白，使场景更具权威性。
>
> * * 5。提供额外的文档**——有更多的意图源，需求管道工作得更好。给我指出其中的任何一个，我将使用它们来完善需求和质量构成：
> -导出AI聊天记录（Claude, Gemini， ChatGPT导出，Claude Code transcript）
> -讨论项目的Slack或Teams频道
> -电子邮件线程，Jira/Linear票，或GitHub关于项目的问题
> -设计文档、架构决策记录或会议记录
> -新闻组帖子，论坛讨论，或邮件列表档案
>
你可以使用像Claude Cowork、GitHub Copilot或OpenClaw这样的工具来连接这些资源，并将它们收集到一个文件夹中，然后指向文件夹。适用于：基于真实项目历史的场景和需求，而不是推断。
>
>“你可以以任何顺序将这些组合起来。你想从哪个开始？”执行每个改进路径

路径1：交互式评审需求。**指向用户`quality/REVIEW_REQUIREMENTS.md`，并提出一起走过它。该协议支持自引导（选择用例）、完全引导（顺序演练）和跨模型审计（不同模型的事实检查完整性报告）。在`quality/REFINEMENT_HINTS.md`中跟踪进度，因此用户可以从他们停止的地方继续。

路径2：用不同的模型细化需求。**将用户指向`quality/REFINE_REQUIREMENTS.md`。每次细化过程：将当前版本备份到`quality/history/vX.Y/`，读取来自REFINEMENT_HINTS.md的反馈，进行有针对性的改进，碰撞次要版本，并记录VERSION_HISTORY.md中的更改。用户可以使用Claude、GPT、Gemini或任何其他模型来运行这个模型——每个模型捕获不同的盲点。运行直到收益递减。**路径3：审查和强化其他项目。**用户选择场景、测试或协议部分。通读一遍：展示当前文本，解释你的推理，询问它是否准确。根据他们的反馈进行修改。如果功能测试发生更改，请重新运行测试。

**路径四：引导问答。**根据你在探索过程中的实际发现提出3-5个问题。这些类别涵盖了最常见的高杠杆缺口：- **场景的事件历史记录。“我找到了（特定的防御代码）。是什么失败导致了这一切？有多少记录受到了影响？”
- **质量门阈值。**“我正在检查[字段]是否包含[值]。什么是正态分布？什么是问题的信号？”
- **集成测试规模和成本。**“该协议运行[N]个测试，大约花费[X]美元。我应该增加还是减少覆盖率？”
- **测试范围。“我生成了[N]个功能测试。你现有的套件覆盖了[其他领域]。有差距吗？”
- **规格审计的模型偏好。**“你们使用哪些AI模型？你有没有注意到自己的长处？”

用户回答后，修改生成的文件并重新运行测试。路径5：提供额外的文档。**用户指向你额外的意图来源-聊天历史，Slack出口，电子邮件线程，Jira票，设计文档，会议记录，论坛档案。这些文件包含了设计决策、事件历史和质量讨论，这些内容都没有成为正式文档。1. 扫描索引文件并导航到与质量相关的内容（与步骤0相同的方法，但现在有了特定的目标—您知道哪些需求需要基础，哪些场景需要阈值，哪些差距需要缩小）。
2. 摘录：带有特定数字的事件故事、防御模式的设计原理、质量框架讨论、跨模型审计结果，以及不能单独从代码中看到的行为契约。
3. 将发现作为新的反馈项提供给`quality/REFINEMENT_HINTS.md`，然后运行一个细化过程来更新需求。
4. 用真实的事件细节修改QUALITY.md场景。使用实际值更新集成测试阈值。修订后重新运行测试。如果用户已经在第0步中提供了聊天记录，那么您已经挖掘了它——但是他们可能希望将您指向特定的对话，连接额外的资源，或者要求您深入挖掘特定的主题。

# # #迭代

用户可以在这些路径中循环多次。每通过一次都使质量剧本更加扎实。当他们感到满意时，他们会自然地继续前进——没有明确的“完成”步骤。

---

夹具策略`quality/`文件夹与项目的单元测试文件夹是分开的。为项目的语言创建适当的测试设置：**Python:**`quality/conftest.py`用于pytest fixture。如果fixture是内联定义的（在pytest的`tmp_path`模式中很常见），那么比起共享fixture，更倾向于内联定义。
**一个带有`@BeforeEach`/`@BeforeAll`设置方法的测试类，或一个共享的测试实用程序类。
- **Scala:**混合到测试规范（例如，`trait FunctionalTestFixtures`）或内联数据构建器中的trait。
**TypeScript/JavaScript:**`quality/setup.ts`带有`beforeAll`/`beforeEach`挂钩，或内联测试工厂。
—**Go:** Helper函数在同一个`_test.go`文件或共享的`testutil_test.go`。使用`t.Helper()`作为测试助手。Go约定更喜欢内联测试设置而不是共享fixture。
- **Rust:**`#[cfg(test)] mod tests`块中的Helper函数，或共享的`test_utils.rs`模块。为测试数据使用构建器模式。

检查现有的测试文件，了解它们是如何设置测试数据的。无论现有测试使用什么模式，都要复制它。研究真实数据形状的现有夹具模式。

---

# #术语- **功能测试** -代码产生的输出规范说它应该？不同于单元测试（独立的单个功能）。
-集成测试-组件是否端到端一起工作，包括真正的外部服务？
- **规格审计** - AI模型读取代码并与规格进行比较。不执行任何代码。捕获代码与文档不匹配的地方。
- **覆盖率剧场** -测试产生高覆盖率数字，但没有捕捉到真正的bug。示例：在不检查其输出的情况下断言函数不会抛出。
- **适用性** -代码在现实世界的条件下做它应该做的事情吗？一个系统可以有95%的覆盖率，但仍然无声地丢失记录。

---

# #原则1. 适用性高于覆盖率百分比
2. 场景来自代码探索和领域知识
3. 具体的失效模式使标准不可协商，而抽象的要求则需要合理化
4. 护栏改造AI审核质量（行号、读体、grep前索赔）
5. 在修复之前进行分类——许多“缺陷”是规范错误或设计决策
6. 结构复核有上限（~65%）。剩下的约35%是意图违反——缺失bug、跨文件矛盾、设计缺口——任何只读取代码的工具都看不见。需求使不可见变为可见。
7. 规范是唯一的贡献，而不是审查结构。相对于从意图源获得正确的可测试需求，重点领域和审查协议是次要的。
8. 跨需求一致性检查是必要的。虫子常常生活在两个个体之间的空隙里正确的代码片段。单独的需求验证无法找到这些。
9. 保留所有派生的需求-不要过滤。检查额外需求的成本很低；由于删减了可能捕获错误的需求而错过错误的代价是很高的。
10. 失败的测试是bug存在的最有力证据。运行红-绿TDD周期（在有bug的代码上测试失败，通过修复的代码），为每个已确认的bug提供修复补丁。显示FAIL→PASS输出——审查者可以不同意你的修复，但不能反对一个重现测试。---

##参考文件

当你在每个阶段工作时，请阅读以下内容：

|文件|何时读取|包含||------|-------------|----------|
|`references/exploration_patterns.md`|阶段1（探索）|模式适用性矩阵，深度模板，领域知识问题|
|`references/defensive_patterns.md`|步骤5（查找骨架）| Grep模式，如何将发现转换为场景|
|`references/schema_mapping.md`|步骤5b（模式类型）|字段映射格式，突变有效性规则|
|`references/requirements_pipeline.md`|第二阶段（需求）|五阶段管道，版本化协议，套用规则|
|`references/constitution.md`|文件1 (QUALITY.md) |完整模板，逐节指导|
|`references/functional_tests.md`|文件2（功能测试）|测试结构、反模式、跨变体策略|
|`references/review_protocols.md`|文件3-4（代码审查，集成）|两个协议的模板，补丁验证，跳过守卫|
|`references/spec_audit.md`|文件5（三人委员会）|完整的审计协议，分类过程，修复执行|
迭代（阶段6之后）|四种迭代策略：间隙、未过滤、奇偶校验、对抗性|
|`references/verification.md`|相位6（验证）|完整的自检清单（45个基准），包括结构化输出、补丁门、跳过保护验证、飞行前发现、版本戳、bug编写、枚举完整性、分类可执行证据、代码提取的枚举列表、机械验证工件、源代码检查测试执行、矛盾门、种子检查执行、收敛跟踪、侧车JSON模式验证、脚本验证闭包门、规范用例标识符。并编写内联修复差异|