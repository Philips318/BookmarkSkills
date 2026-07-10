#纯代码模式

*最后更新日期：2026-05-03 （v1.5.6第三阶段-首次发布）

当Quality Playbook运行在`reference_docs/`目录不存在或为空的目标repo上时，它以纯代码模式运行。本文解释了这意味着什么，为什么它很重要，以及如何将只运行代码的运行升级为下一轮的完整文档运行。

##“纯代码模式”的含义

剧本的第一阶段推导通常包含两种证据：

-代码证据（第3层+）-源代码树本身，加上内联注释、防御模式、测试和与代码共存的任何内联文档。
**文档证据（1/2层）** -运营商放入`reference_docs/`（自由格式注释，设计文档，回顾，AI聊天）和`reference_docs/cite/`（项目规范，rfc， API合同，需求应该追溯到）的明文文件。纯代码模式是没有可用文档证据的运行状态。剧本继续进行——它没有中止——但它派生的每一个需求都完全依赖于代码证据。阶段1EXPLORATION.md有一个“文档状态：仅代码模式”的开头部分，该部分显示了该模式，以便审阅者在第一次阅读时看到它。

##纯代码运行的期望

在我们的基准测试运行中，纯代码传递始终产生：- **总体派生的需求更少。**没有规范语言锚定，阶段1没有1/2层的证据可以引用，因此需求集完全回落到第3层（代码即规范）。
- **可能会发现更少的bug。**当审稿人知道代码“应该”做什么时，代码审查（第3阶段）是最有效的——违反文档意图的bug比隐藏在模糊的代码规范后面的bug更容易暴露。在没有文档的情况下，审阅者必须从代码本身推断意图，这就留下了一类未检测到的违反意图的缺陷。
- **对代码内部信号的依赖性更高。**在没有外部文档的情况下，防御模式（错误检查、验证）、测试名和注释风格的注释更有分量。仅代码模式下的bug计数仍然有用——它们反映了仅从代码中发现的内容——但它们是完整文档运行所产生的结果的下限。

如何升级到完整文档运行

在重新运行阶段1之前，将明文文档文件放在目标repo的`reference_docs/`树中：```
<target-repo>/
  reference_docs/
    project_notes.md         # Tier 4 — informal notes, AI chats
    design_overview.md       # Tier 3-4 — internal design decisions
    cite/
      api_spec.md            # Tier 1/2 — citable specs, RFCs, contracts
      protocol_v3.txt        # Tier 1/2 — formal specifications
```
`reference_docs/`顶层的文件算作非正式上下文（第4层）。`reference_docs/cite/`下的文件算作可引用证据（一级或二级，取决于来源的权威-参见`schemas.md`§3.1）。可以识别`.md`和`.txt`；其他格式将被忽略。

输入文档后，重新运行剧本。阶段1将检测已填充的`reference_docs/`，并跳过仅代码模式降级。新运行的EXPLORATION.md、REQUIREMENTS.md和BUGS.md将反映更丰富的证据基础。

##退出：`--require-docs`想要终止运行而不是以纯代码模式继续运行的操作符可以将`--require-docs`传递给`python3 -m bin.run_playbook`（v1.5.6+）。当设置了`--require-docs`，并且在第一阶段进入时`reference_docs/`为空时，剧本：

1. 将`aborted_missing_docs`事件追加到`quality/run_state.jsonl`（在`references/run_state_schema.md`中注册的事件类型）。
2. 将一个清晰的`ERROR: aborted_missing_docs — reference_docs/ empty and --require-docs set`块写入`quality/PROGRESS.md`。
3. 在任何LLM工作之前终止（退出非零，与gate-fail相同）。默认情况下，该标志是关闭的。将它用于compliance/policy上下文中，在这种情况下，安静的纯代码模式降级会掩盖真正的进程差距（例如，“每个发布运行都必须引用一个规范；没有规范意味着运行不应该开始”）。该标志是`--no-formal-docs`的opt-OUT对应的opt-IN（对于相同的纯代码模式情况，它抑制WARN标志，但允许继续运行）。

# #交叉引用

- **README** -“如何使用质量手册”的第1步将文档描述为首先要提供的东西。
- **`SKILL.md`** -第一阶段散文描述了在探索过程中如何使用文件证据。
- **`bin/reference_docs_ingest.py`** -摄取`reference_docs/`树的实现。
- **`references/run_state_schema.md`** -定义剧本在纯代码模式触发时发出的`documentation_state`事件，因此降级在审计跟踪中是可搜索的。