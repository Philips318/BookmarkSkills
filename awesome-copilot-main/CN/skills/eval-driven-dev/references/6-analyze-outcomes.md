第六步：分析结果

**为什么这个步骤**:`pixie test`产生原始分数。现在，您分析这些结果以理解它们的含义——完成待完成的评估、识别模式、验证假设，并制定可操作的改进计划。分析分为三个相互依存的阶段：入门级→数据集级→行动计划。

---

结果目录结构

在`pixie test`之后，结果目录如下所示：```text
{PIXIE_ROOT}/results/<test_id>/
  meta.json
  dataset-{idx}/
    metadata.json
    entry-{idx}/
      config.json              # evaluators, description, expectation
      eval-input.jsonl         # input data fed to evaluators
      eval-output.jsonl        # output data captured from app
      evaluations.jsonl        # scored + pending evaluations
      trace.jsonl              # LLM call traces
```
读取`meta.json`以查找`<test_id>`。分析所需的所有数据都在这个目录中。

---

硬完成门

你是第六步的评分员。**待定的评估不是交给用户，web UI不是评分的替代品。**您可以使用web UI来浏览跟踪和输出，但完成是通过在磁盘上写入文件来完成的。

在满足以下所有条件之前，步骤6是不完整的：

-每个`evaluations.jsonl`中的每个`"status": "pending"`条目都被一个包含`score`和`reasoning`的评分条目所取代。
—每个数据集目录都包含“`analysis.md`”和“`analysis-summary.md`”。
—测试运行根目录包含“`action-plan.md`”和“`action-plan-summary.md`”。
—该技能的`resources/`目录中的验证器脚本传递到目标结果目录。

* *禁止快捷键* *:-保留任何`"status": "pending"`条目
-告诉用户在web UI中查看待处理的评估
-编写单个顶级替代文件，如`pixie_qa/06-analysis.md`-写短语，如“可能通过”或“可能失败”没有评分评估和更新`evaluations.jsonl`如果您做了以上任何一件事，步骤6就没有完成。

##迭代规则

如果跨多个fix/test循环进行迭代，则每次成功运行`pixie test`都会创建一个新的`pixie_qa/results/<test_id>`目录和一个新的第6步义务。一旦目录存在，它就成为当前周期的分析目标。

在编辑应用程序代码、提示符、数据集、求值器或重新运行`pixie test`之前，请为该结果目录完成步骤6。不要跳过早期的周期，只分析最后一次运行。

**额外的禁止快捷方式**：不要创建一个新的`pixie_qa/results/<test_id>`，而留下一个没有步骤6工件的旧的`pixie_qa/results/<test_id>`。

---

##写作原则

您生成的每个分析**详细**工件都必须遵循以下原则：- **数据驱动**：每个意见或陈述必须由评估运行的具体数据支持。引用分数，引用条目索引，引用具体evalinput/output内容。没有挥挥手。与其写些没有根据的东西，不如什么也不写。
- **证据优先**：在得出结论之前提供原始数据和证据。读者（另一个编码代理）应该能够独立地从你引用的证据中验证你的结论。
- **可追溯**：对于每一个结论，提供一个链条：数据源→观察→推理→结论。另一个代理应该能够沿着这条链向后验证或质疑任何索赔。
**不推销**：不要提倡、推广或使用带有价值的语言（“优秀”、“健壮”、“令人印象深刻”、“设计精良”）。说明数据显示了什么以及它暗示了什么行动。让读者形成质量判断。
- * *行为面向离子的**：每个分析都应该有助于对评估管道或应用程序进行具体改进的最终目标。不要写没有结果的评论。每个持久化的分析摘要工件必须遵循这些原则：

- **简明**：人类读者应该能够在2分钟内理解任何单一工件的关键发现和操作。
- **结论优先：以读者需要知道的内容（结果、发现、行动）开头，而不是方法论或背景。
- **语言简洁**：避免行话。非技术涉众应该能够理解摘要。
- **一致**：摘要结论必须与详细版本的证据相匹配。不要在摘要中添加详细版本中不支持的声明。

###双变量模式

这一步中的每个持久化分析工件都有两个文件：

|工件|详细文件（用于代理）|摘要文件（用于人类）|| ---------------- | --------------------------- | ----------------------------------- |
|`dataset-{idx}/analysis.md`|`dataset-{idx}/analysis-summary.md`|
|行动计划|`action-plan.md`|`action-plan-summary.md`|

总是先写详细的版本，然后从中得出总结。摘要是详细版本内容的严格子集——它不应该包含详细版本中没有的主张或结论。

---

阶段1：入门级分级通过

单独处理每个数据集条目。对于每个`dataset-{idx}/entry-{idx}/`：

# # # 1。读取条目数据

阅读以下文件获取条目：

-`config.json`-配置了什么评估器，描述，期望
-`eval-input.jsonl`-什么数据被馈送到app/evaluators-`eval-output.jsonl`-应用程序产生了什么
-`evaluations.jsonl`-当前评估结果（评分和待定）
-`trace.jsonl`- LLM调用的应用程序制作（如果可用）

# # # 1 b。完成待完成的评估

如果`evaluations.jsonl`包含带有`"status": "pending"`的条目，则必须对它们进行评分：1. 读取待执行求值的`criteria`字段
2. 将标准应用于条目的eval输入、eval输出和跟踪数据
3. 给**评分**在0.0到1.0之间：
-`1.0`-完全符合条件
-`0.5`-`0.9`-部分符合标准（解释什么缺失）
—`0.0`-`0.4`-不符合条件
4. 写一个推理字符串（从输出或跟踪中引用具体证据的1-3句话）
5. 将`evaluations.jsonl`中的挂起条目替换为记分结果。**不要添加第二行，保留挂起的行。覆盖挂起行本身

* *在* *(待定):```json
{
  "evaluator": "ResponseQuality",
  "status": "pending",
  "criteria": "The response should..."
}
```
* *在* *(得分):```json
{
  "evaluator": "ResponseQuality",
  "score": 0.85,
  "reasoning": "Response addresses the main question but omits..."
}
```
* *分级指南* *:

-以证据为基础-每个分数必须参考特定的输出或跟踪内容
-从字面上使用标准-不要在书面内容之外扩展或重新解释
-考虑跟踪-区分应用程序逻辑问题和LLM质量问题
-被校准-为真正完全满足标准的输出保留1.0
-不惩罚法学硕士的非决定论-正确答案的不同措辞不是失败
-不服从用户-如果证据足以写“可能通过”，则足以分配分数并更新`evaluations.jsonl`# # # 1 c。不持久化入门级分析文件

在这个精简的工作流程中，**不要写`entry-{idx}/analysis.md`或`entry-{idx}/analysis-summary.md`**。阶段1仅用于读取证据并将每个待处理的评估转换为`evaluations.jsonl`中的评分行。你可能会在推理的时候做一些临时的草稿笔记，但它们不是可交付的。坚持只:

-更新了每个条目目录中的`evaluations.jsonl`-第二阶段的数据集级分析文件
-阶段3的运行级行动计划文件

---

阶段2：数据集级分析

在分析了数据集中的所有条目之后，生成数据集级别的分析。将`analysis.md`写入数据集目录（`dataset-{idx}/analysis.md`）。

# # # 2 a。汇总数据

汇总数据集中的所有条目：

-Pass/fail计数和总体通过率
-每个评估者的统计数据（通过率，min/max/mean分数）
-哪些条目没有通过哪些评估器（失败集群）

# # # 2 b。形成并验证假设

在这三个维度上提出3个高可信度的假设：1. **测试用例质量** -测试用例集是否充分有效地验证了应用程序的功能？它是否涵盖了重要的失效模式？是否存在盲点？

2. **评估criteria/evaluator质量** -评估人员是否有适当的粒度和分级来捕捉真正的问题？是否存在橡皮图章评估器（都是1.0）？是否存在不稳定的评估者（没有代码更改的高方差）？标准是太模糊还是太严格？

3. **应用程序质量** -根据评估结果，应用程序的优点和缺点是什么？它在哪里生产高质量的产品？它在哪里失败了？

对于每个假设：- **用一句话清楚地陈述假设
-引用证据** -条目索引，评估者名称，分数，推理引用，跟踪数据
- **Validate或invalidate** -查看实际的evalinput/output数据和代码来确认或驳斥
- **结论** -这个假设暗示了什么作用？

即使数据有限，也总是有可能产生3个假设。如果评估数据没有给出关于应用程序质量的结论性答案，那么它本身就是一个关于测试用例或评估人员差距的信号。

# # # 2 c。编写数据集分析（两个文件）

生成两个文件用于数据集分析。先写详细版本，然后得出总结。

####详细版本：`dataset-{idx}/analysis.md`这个文件是供代理使用的——它提供了完整的数据聚合、带有证据链的假设形成以及编码代理可以直接采取行动的经过验证的结论。

* *写作原则:* *- **在解释之前显示所有数据。**在做出任何假设之前，先从原始聚合（pass/fail，每个评估者的统计数据，失败集群）开始。数据应该是独立的。
- **对于每个假设，呈现：数据→推理→结论。读者应该能够一步一步地遵循你的逻辑，并独立得出相同的结论。
- **直接交叉参考原始录入证据。**引用证据时，引用特定的条目索引和底层的files/data点（例如：`entry-3/evaluations.jsonl`，`entry-3/eval-output.jsonl`，或`entry-3/trace.jsonl`）。
- **区分因果关系。**如果两个条目在同一个求值器中失败，这是一个模式。但根本原因可能不同—通过检查实际输出数据来验证，不要假设。
- **不要在没有标记的情况下推测。**如果结论是不确定的，说“假设（未经验证）：…”，并解释哪些额外的数据会证实坚定或反驳它。* *内容:* *

1. **概述** -数据集名称，入口计数，总体通过率
2. **原始聚合数据**
-评估人员统计表（通过率、分数范围、平均值、标准差）
-失败矩阵：条目×评估者显示分数，突出失败
—失败集群：由共享的失败评估器分组的条目
3. **假设1：测试用例** -假设陈述，证据与entry/evaluator参考，验证步骤，结论与具体行动
4. **假设2：评估者** -结构相同
5. 假设3：应用** -结构相同
6. **开放性问题** -任何数据不能决定性回答的问题，并建议哪些额外的数据会有所帮助

####摘要版本：`dataset-{idx}/analysis-summary.md`该文件用于**人工审查** -数据集结果、关键发现和建议操作的可扫描概述。

模板:* * * *```markdown
# Dataset Analysis — Summary

**Dataset**: <name> | **Entries**: <N> | **Pass rate**: <X/N (Y%)>

## Results at a glance

| Evaluator | Pass rate | Avg score | Notes                  |
| --------- | --------- | --------- | ---------------------- |
| ...       | ...       | ...       | <one-liner if notable> |

## Key findings

1. <Finding>: <1-2 sentences with the conclusion and its implication>
2. ...
3. ...

## Recommended actions (priority order)

1. <Action>: <what to do and expected impact, 1-2 sentences>
2. ...
3. ...
```
摘要最多40行。

---

阶段3：行动计划（两个文件）

在分析了所有数据集之后，制定行动计划。在测试运行根目录下写两个文件。先写详细版本，然后得出总结。

详细版本：`{PIXIE_ROOT}/results/<test_id>/action-plan.md`这个文件是为代理使用的——它提供了具体的、可实现的改进项目和完整的证据跟踪，因此编码代理可以选择任何项目并执行它，而无需额外的上下文收集。

* *写作原则:* *- **每个项目必须是独立的。**只读取一个优先项的编码代理应该有足够的上下文（证据引用，文件路径，预期更改）来实现它。
- **追查每件物品的证据。**每个优先级必须引用：哪个假设（来自哪个数据集分析），哪个entries/evaluators提供了证据，以及具体数据显示了什么。
- **具体说明“如何”。不要说“改进提示符”，而是说“在`scrapegraphai/prompts/generate_answer.py`第45行，添加指令：‘…’”。越具体，越具有可操作性。
- **不包括投机项目。**每个项目必须有有效的证据。如果一个项目是基于一个未经验证的假设，要么先验证它，要么排除它。

* *结构:* *```markdown
# Action Plan (Detailed)

## Summary

- X datasets analyzed, Y total entries, Z% overall pass rate
- [1-2 sentence high-level assessment]

## Priority 1: [Most impactful improvement]

- **What**: [specific change to make]
- **Why**: [which hypothesis from which dataset analysis, with entry/evaluator references]
- **Evidence**: [specific scores, output excerpts, trace data that support this]
- **Expected impact**: [which entries/evaluators this will improve, and predicted score change]
- **How**: [concrete implementation steps with file paths and line numbers]
- **Verification**: [how to verify the fix worked — which entries to re-run, what scores to expect]

## Priority 2: ...

...
```
摘要版本：`{PIXIE_ROOT}/results/<test_id>/action-plan-summary.md`这个文件是供**人类审查**的——一个人类可以在2分钟内理解和批准的优先级改进列表。

模板:* * * *```markdown
# Action Plan — Summary

**Overall**: <X entries, Y% pass rate. 1-sentence assessment.>

## Actions (priority order)

1. **<Action title>**: <What to change and why, 2-3 sentences. Expected impact.>
2. **<Action title>**: <What to change and why, 2-3 sentences. Expected impact.>
3. ...
```
摘要最多~30行。

* * * *优先级标准:

-系统问题（影响多个entries/datasets）在孤立的问题之前
-在推测性证据之前提出明确、有效的证据
-在评估人员改进测试用例之前的应用程序质量差距
-大型重构之前的快速修复

行动计划应该包含3-5个项目。每一个都必须追溯到阶段2的一个经过验证的假设。不包括推测性或缺乏证据的项目。

---

##流程总结

1. **阶段1**（每个条目）：读取数据→评分等待评估→更新`evaluations.jsonl`2. **阶段2**（每个数据集）：聚合→形成3个假设→验证→写入`dataset-{idx}/analysis.md`+`dataset-{idx}/analysis-summary.md`3. **阶段3**（每次测试运行）：合成→优先级→写入`action-plan.md`+`action-plan-summary.md`并发处理数据集中的条目（如果可用，使用子代理）。工艺阶段依次进行——阶段2取决于阶段1的输出，阶段3取决于阶段2的输出。

---

##最终验证

在结束您的回合之前，根据您分析的测试运行目录，运行该技能的`resources/`目录中`setup.sh`旁边附带的第6步验证器脚本。

例子:```bash
python /path/to/eval-driven-dev/resources/verify_step6_completion.py pixie_qa/results/<test_id>
```
如果验证者报告任何错误，继续工作。直到验证者通过，步骤6才算完成。