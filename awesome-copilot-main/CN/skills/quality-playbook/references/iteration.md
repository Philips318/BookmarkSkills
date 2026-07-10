#迭代模式参考

>该文件包含每个迭代策略的详细说明。
>代理在运行迭代时读取这个文件——所有操作细节都在这里。
>不在提示符或基准运行器中。

迭代周期

推荐的迭代顺序是：**间隙→未过滤→奇偶校验→对抗性**。每种策略都会找到不同的bug类，并按此顺序运行它们，从而使累积产量最大化。每次迭代之后，该技能都会打印出下一个策略的建议提示——遵循这个循环，直到你遇到收益递减或决定停止。```
Baseline run                                          # structured three-stage exploration
→ gap         scan previous coverage, explore gaps    # finds bugs in uncovered subsystems
→ unfiltered  pure domain-driven, no structure        # finds bugs that structure suppresses
→ parity      cross-path comparison and diffing       # finds inconsistencies between parallel implementations
→ adversarial challenge dismissed/demoted findings    # recovers Type II errors from previous triage
```
所有策略共享规则

这些规则适用于每一个迭代策略：

1. **ITER文件命名。**将结果写入`quality/EXPLORATION_ITER{N}.md`-检查哪些迭代文件已经存在并使用下一个数字（例如，`EXPLORATION_ITER2.md`用于第一次迭代，`EXPLORATION_ITER3.md`用于第二次迭代）。

2. **不要删除或存档quality/.**您是在现有运行的基础上构建，而不是替换它。

3. **环境预算纪律。**第一次运行的EXPLORATION.md可以是200-400行。在开始自己的探索之前，将所有内容放入上下文中，留给深入调查的空间太少了。前一次运行的扫描应该占用~ 20-30行上下文。目标深度读取应该总共消耗40-60行。这就将你的大部分预算留给了新的探索。4. * *合并。**完成特定策略的探索后，创建或更新结合所有迭代结果的`quality/EXPLORATION_MERGED.md`。对于每个部分，将结果与明确的归属（`[Iteration 1]`/`[Iteration 2: gap]`/`[Iteration 3: unfiltered]`/等）连接起来。在归因中包含策略名称，这样下游阶段就可以看到每个发现都采用了哪种方法。候选bug部分应该从所有迭代的所有发现中重新整合。如果`EXPLORATION_MERGED.md`在以前的迭代中已经存在，那么将新迭代的发现合并到其中，而不是从头开始。**降职候选人名单（必须在EXPLORATION_MERGED.md）。**重新整合候选bug部分后，在EXPLORATION_MERGED.md的末尾添加或更新`## Demoted Candidates`部分。这个部分跟踪在任何迭代过程中被忽略、降级或失去优先级的发现——它们是对抗策略的原始材料。对于每一位被降职的候选人，请记录：   ```
   ### DC-NNN: [short title]
   - **Source:** [which iteration and strategy first surfaced this]
   - **Dismissal reason:** [why it was demoted — e.g., "classified as design choice," "insufficient evidence," "needs runtime confirmation"]
   - **Code location:** [file:line references]
   - **Re-promotion criteria:** [specific evidence that would flip this to a confirmed candidate — e.g., "show that the permissive behavior violates a documented contract," "trace the code path to prove the edge case is reachable," "demonstrate that the output differs from what the spec requires"]
   - **Status:** DEMOTED | RE-PROMOTED [iteration] | FALSE POSITIVE [iteration]
   ```
再推广标准是最重要的领域——它们告诉对抗策略应该收集哪些证据。像“需要更多的调查”这样模糊的标准是不可接受的；编写不同代理会话可以在没有附加上下文的情况下执行的标准。如果随后的迭代重新提升或最终伪造降级的候选人，则更新其状态并添加解释解决方案的注释。

5. **继续第2-6阶段。**使用`EXPLORATION_MERGED.md`作为阶段2工件生成的主要输入。所有下游工件（REQUIREMENTS.md、代码审查、规范审计）都应该引用合并后的探索。**TDD对于迭代运行是强制性的（v1.3.49）。**对于每个新确认的bug，迭代运行必须执行完整的TDD红绿循环，就像基线运行一样。这意味着：对于在此迭代中确认的每个新的BUG-NNN，创建一个回归测试补丁，针对未打补丁的代码运行它以生成`quality/results/BUG-NNN.red.log`，如果存在修复补丁，则针对打补丁的代码运行它以生成`quality/results/BUG-NNN.green.log`。阶段5中的TDD日志关闭门同样适用于迭代运行——缺少日志文件将导致quality_gate.sh失败。不要因为TDD“只是一个迭代”或者之前的bug已经有日志而跳过它。新的bug需要新的日志。如果测试运行器对项目的语言不可用，那么在第一行创建带有`NOT_RUN`的日志文件和一个解释—该文件必须仍然存在。6. **迭代模式完成门。**在进入第二阶段之前（适用于所有策略）：
-`quality/ITERATION_PLAN.md`已存在，并命名所使用的策略
-`quality/EXPLORATION_ITER{N}.md`为这个迭代存在，至少有80行实质性内容
-`quality/EXPLORATION_MERGED.md`存在并包含所有迭代的结果
-合并的候选bug部分至少有2个在以前的迭代中没有出现的新候选
-至少有一个发现覆盖了之前迭代中未探索的代码区域，或者用新的证据重新证实了之前被驳回的发现7. **建议下次迭代。**在第6阶段结束时，在编写最终的PROGRESS.md摘要之后，打印出周期中下一个迭代策略的建议提示。如果当前的策略是：
- **差距**→建议：`Run the next iteration of the quality playbook using the unfiltered strategy.`- **未经过滤**→建议：`Run the next iteration of the quality playbook using the parity strategy.`—**奇偶校验**→建议：`Run the next iteration of the quality playbook using the adversarial strategy.`- **对抗性**→建议：`Run the quality playbook from scratch.`（循环完成）
- **基线（无策略）**→建议：`Run the next iteration of the quality playbook using the gap strategy.`清晰地格式化建议，以便用户可以复制粘贴。   ```
   ────────────────────────────────────────────────────────
   Next iteration suggestion:
   "Run the next iteration of the quality playbook using the [strategy] strategy."
   ────────────────────────────────────────────────────────
   ```
## Meta-strategy:`all`-按顺序运行每个策略`all`策略是一种跑步者级的便利，它按顺序执行间隙→未过滤→奇偶校验→对抗，每个都作为一个单独的代理会话。单个代理会话不能运行多个策略（上下文预算），因此`all`由编排代理或基准运行器作为迭代调用的循环来实现。如果任何策略没有发现任何新漏洞，那么尽早停止（收益递减）。

用法（编排代理）：“运行所有迭代”—代理按顺序运行间隙→未过滤→奇偶校验→对抗性。
用法（基准运行器）：`python3 bin/run_playbook.py --next-iteration --strategy all <targets>`（基准工具，不是随技能一起提供的）。`--strategy`也接受逗号分隔的有序子集，例如`--strategy unfiltered,parity,adversarial`。

---

##策略：`gap`（默认）-查找前一次运行遗漏的内容浏览前一轮的报道，并有意识地探索其他地方。当第一次运行在结构上是合理的，但只覆盖代码库的一个子集时，效果最好。

1. **覆盖扫描（轻量级）。**使用分治策略阅读前面的`quality/EXPLORATION.md`-不要一次将整个文件加载到上下文中。而不是:
-只阅读部分标题和每个部分的前2-3行来构建覆盖地图
-对于每个部分，记录：部分名称，涵盖的子系统，发现的数量，深度级别（浅=单一功能提及，深=多功能跟踪）
—将覆盖图写入`quality/ITERATION_PLAN.md`2. * *标识的差距。**从覆盖图中识别：
-根本没有探索的子系统或模块
-有浅层发现的部分（几行，只提到一个函数，没有代码路径跟踪）
-列出的质量风险场景，但从未追踪到特定代码
-可以应用但没有被选中的模式深潜（从适用性矩阵中）
-步骤6中未解决的领域知识问题

3. * * deep-read目标。**仅对于2-3个最薄或缝隙最丰富的部分，请阅读前面EXPLORATION.md的完整部分内容。这为您提供了关于已经发现的内容的特定上下文，而不会将您的整个上下文预算消耗在之前的发现上。4. * *差距探索。**只针对已确定的缺口进行有针对性的第一阶段勘探。使用相同的三阶段方法（开放探索→质量风险→选择模式），但将范围限定在未覆盖的区域。使用相同的模板结构将结果写入`quality/EXPLORATION_ITER{N}.md`。

---

策略：`unfiltered`—纯领域驱动的探索，没有结构约束

完全忽略三级门控结构。以经验丰富的开发人员的方式探索代码库——阅读代码、遵循直觉、跟踪可疑路径——没有模式模板、适用性矩阵或部分格式要求。这种策略有意地移除结构脚手架，让领域专家不受约束地推动发现。**在基准测试中，在技能版本v1.3.25-v1.3.26中使用的未经过滤的领域驱动方法发现了结构化三阶段方法一直遗漏的错误，特别是在web框架和HTTP库中。结构化方法擅长于系统覆盖，但可能过度限制探索，导致模型将上下文花费在格式遵从上，而不是深入的代码阅读上。未经过滤的策略恢复了失去的发现能力。

1. **轻量级先前运行扫描。**只读取`## Candidate Bugs for Phase 2`部分和`quality/BUGS.md`，从之前的运行中知道已经找到了什么。不要阅读完整的EXPLORATION.md-你想要一个新鲜的视角，而不是锚定在以前的探索路径。给`quality/ITERATION_PLAN.md`写一个简短的说明，列出之前运行发现的内容，并确认您正在使用未过滤策略。2. * *未过滤的探索。**使用纯领域知识从头开始探索代码库。无要求断面，无模式适用性矩阵，无浇口自检。而不是:
-深入阅读源代码-入口点，热路径，错误处理，边缘情况
-遵循你的领域专长：“这个领域的专家会发现什么是可疑的？”
对于每个可疑的发现，用file:line引用跟踪2+函数的代码路径
-直接生成bug假设-不是“要调查的领域”，而是“文件行中的特定代码产生错误行为是因为[原因]”
-将发现写入`quality/EXPLORATION_ITER{N}.md`，作为发现的平面列表，每个都有文件行引用和错误假设。不需要结构模板——深度和专一性很重要，而不是章节格式。
-最少：10个具体的发现与文件行引用，其中至少5个跟踪代码路径acRoss 2+功能3. * *领域知识的问题。**使用你刚刚探索的代码和你的领域知识完成这些问题。将你的答案与你的发现联系起来，而不是单独写在一个封闭的部分。
-类似方法之间存在哪些API表面不一致？
-代码在哪里对结构化格式进行临时字符串解析？
领域专家会尝试哪些开发人员可能不会测试的输入？
-什么元数据或配置值可能静默错误？

---

##策略：`parity`-交叉路径比较和区分

系统地列举同一契约的并行实现，并区分它们的不一致性。这种策略通过比较应该以相同方式运行但却没有的代码路径来发现bug。**在基准测试中，v1.3.40技能版本使用“回退路径奇偶性”和“跨实现一致性”作为明确的探索模式，在virtio中发现了5个bug。其中三个bug （MSI-X slow_virtqueues reattach, GFP_KERNEL under spinlock, INTx admin queue_idx）是通过排列并行代码路径并发现差异而发现的，而不是通过探索单个子系统。差距策略、未过滤策略和对抗策略都是探索领域或挑战决策，但没有一个明确地比较平行路径。这一策略填补了这一空白。1. **列举并行路径。**扫描代码库中实现相同合约或通过不同路径处理相同逻辑操作的代码组。常见的类别:
**Transport/backend变体：**相同接口的多个实现（例如，PCI vs MMIO vs vDPA，同步vs异步，HTTP/1.1vsHTTP/2）
- **回退链：**主路径→回退→最后手段（例如，MSI-X→共享→INTx，富错误→通用错误）
- **Setup vsteardown/reset:**初始化路径vs相同资源的cleanup/reset路径
- **快乐路径vs错误路径：**正常流程vsexception/error处理相同的操作
- **公共API变体：**重载方法，便利包装器，应该产生等效结果的格式特定解析器
-将枚举写入`quality/ITERATION_PLAN.md`，并对每个并行组进行简要描述。2. * *成对比较。**对于每个并行组，并排阅读代码路径并系统地检查下面的每个比较子类型。并不是每一个子类型都适用于每一个平行群体——但明确地考虑每一个子类型可以防止策略只发现“明显的”差异而忽略结构性差异。

**比较子类型检查表**（检查每个平行组的每个子类型）：- **资源生命周期奇偶性：**比较setup/init对资源的处理与teardown/reset/cleanup对相同资源的处理。在setup中获得的每个资源都必须在teardown中释放——并且以相同的顺序，具有相同的作用域。查找setup创建但reset忘记的资源（例如，在探测期间填充但在重置期间未耗尽的列表）。
- **分配上下文奇偶性：**比较分配标志，锁上下文，并在并行路径中断状态。如果一条路径分配了`GFP_KERNEL`（可睡眠），但在另一条路径不支持的自旋锁下运行，那就是一个bug。检查：持有哪些锁？在该上下文中，哪些分配标志是有效的？平行路径是否一致？
—**标识符和索引奇偶校验：**比较并行路径对同一逻辑实体计算索引、偏移量或标识符的方式。如果setup使用`queue_index + admin_offset`，而reset使用`raw_queue_index`，则不匹配是a错误的候选人。
- **Capability/feature-bit奇偶性：**比较每个并行路径检查或设置的特征位，标志或功能。如果MSI-X路径检查慢路径向量列表，而INTx回退路径没有检查，则回退后向量可能会路由错误。
—**Error/exception奇偶校验：**比较不同路径间的错误处理。如果主路径优雅地处理错误，但备用路径允许错误传播，则备用路径不如主路径健壮——主路径是向后的。
- **Iteration/collectionparity:**比较每个路径迭代的集合。如果setup在`all_queues`上迭代，而reset在`active_queues`上迭代，则用于非活动队列的资源会泄漏。对于发现的每个差异，使用文件行引用跟踪两个代码路径，并确定差异是故意的（记录的、测试的或结构上必要的）还是错误。

3. **跨文件合同跟踪。**对于最有可能的差异，跟踪跨文件的调用链以验证：
-每个路径运行的lock/interrupt上下文
-在该上下文中哪些分配标志是有效的
-合同（在规格，注释或标题中记录）是否需要奇偶校验
-将发现写入`quality/EXPLORATION_ITER{N}.md`，并为每个发现引用两个代码路径。

4. **最少输出：**至少枚举5个并行组，至少8个用file:line引用跟踪的两两比较，至少3个具体的差异发现。

---

##策略：`adversarial`-挑战之前运行的结论重新调查上一轮解雇、降级或标记为满意的内容。该策略假设之前的运行犯了第二类错误（由于过于保守而错过了真正的bug），并系统地挑战这些决策。

**这种策略存在的原因：**在基准测试中，分类步骤通过要求过多的证据，将模棱两可的案例标记为“设计选择”，或者在没有深入验证的情况下接受代码审查满意的结论，从而可靠地降低了合法的发现。对抗策略专门针对这些失效模式。1. **载入之前的决定。**从之前的运行中读取这些文件（首先使用分而治之- section头，然后是目标深度读取）：
-`quality/EXPLORATION_MERGED.md`-特别是`## Demoted Candidates`部分（这是您的主要输入-它包含每个驳回发现的结构化重新推广标准）
-`quality/BUGS.md`-确认了什么（为了避免再次发现相同的bug）
-`quality/spec_audits/*triage*`-在分诊过程中被解雇或降级的人员，以及原因
-通过2个SATISFIED/VIOLATED裁判
-`quality/EXPLORATION.md`-只是`## Candidate Bugs for Phase 2`部分，看看哪些候选的bug没有被确认
-在`quality/ITERATION_PLAN.md`上写一份摘要，列出：(a)从舱单中降级的候选人及其重新晋升的标准，(b)舱单中尚未列出的其他被驳回的分类结果，(c)未被提升的候选人，(d)证据不足但标记为满足的要求2. **以较低的证据标准重新调查被驳回的调查结果。**对抗策略故意使用较低的证据标准。基线和差距战略正确地要求强有力的证据，以避免在最初发现时出现误报。但是通过对抗性迭代，遗留下来的未被发现的bug恰恰是那些保守的分类一直拒绝的bug——它们看起来模棱两可，它们可能是“设计选择”，它们缺乏戏剧性的运行时失败。对于这些发现：
-代码路径跟踪显示可观察到的语义漂移（输出与规范或合同要求的不同）足以确认-您不需要运行时崩溃或戏剧性的失败
“允许行为”不是一个自动的设计选择——检查规范、文档或API合约是否定义了预期的行为。如果代码偏离了文档化的合同，不管偏差是否“允许”，它都是一个bug
-如果降职候选人名单中包括重新晋升的标准，请尝试具体满足这些标准。每个标准都是可操作的——遵循它
-阅读查找结果中引用的特定代码位置
-独立跟踪代码路径-不依赖于前一次运行的分析
-用新的证据明确确定CONFIRMED/FALSE-POSITIVE-更新降级候选人清单：将迭代属性的状态更改为重新提升或FALSE POSITIVE3. **挑战已满足的判决。**对于每个需求，代码评审用单行证据标记为满意（单行引用，没有代码路径跟踪，或在一个引用下与3+其他需求分组）：
-通过阅读引用的代码和跟踪行为来重新验证需求
-检查需求是否被满足，或者审查是否肤浅

4. **探索相邻代码。**对于每一个重新确认或新确认的发现，探索周围的代码中相关的bug - bug集群。如果一个函数有一个bug，那么它的调用者和同级函数可能有相关的问题。

5. 将所有发现写入`quality/EXPLORATION_ITER{N}.md`。每个发现必须包括：原始来源（分类解雇，候选人降级，或满意的挑战），新的证据，和新的决定。