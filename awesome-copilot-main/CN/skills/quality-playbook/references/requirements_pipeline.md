#需求管道

# #概述

该文档为质量手册的步骤7定义了五个阶段的需求生成管道。管道将契约发现与需求派生分离开来，使用基于文件的外部内存，因此模型不需要同时保存上下文中的所有内容，并包含带有完整性门的机械验证。

**为什么是管道？**在70个需求之后，单次需求生成就不再受关注了，因为模型同时发现契约和编写正式需求。使用基于文件的移交将这些阶段划分为不同的阶段，可以产生更完整的覆盖。在Gson上测试（81个源文件，~21K行），单次通过产生48个需求；管道生产了110台。

##生成的文件

|文件|用途||------|---------|
|`quality/CONTRACTS.md`|从源|中提取的原始行为契约
|`quality/REQUIREMENTS.md`|可测试的需求与叙述（主要可交付的）|
|`quality/COVERAGE_MATRIX.md`|合同到需求的可追溯性
|`quality/COMPLETENESS_REPORT.md`|最终完备性评价与判定|
|`quality/VERSION_HISTORY.md`|查看带有版本表和出处的日志|
|`quality/REFINEMENT_HINTS.md`|评审进度和反馈（评审时创建的）|

版本化备份在`quality/history/vX.Y/`中。

---

阶段A：提取行为契约

**输入：**项目中的所有源文件（或范围子系统-参见下面的缩放检查）。
* *输出:* *`quality/CONTRACTS.md`缩放检查

在开始提取之前，对项目中的源文件进行计数（排除测试、生成的代码、供应商依赖项和构建工件）。- **标准项目（≤300个源文件）：**正常进行-从所有文件中提取合同。这个范围内的项目已经进行了端到端的测试（例如，Gson在大约81个源文件中产生了110个需求，完全覆盖）。
- **大型项目（301-500个源文件）：**重点关注第一阶段第二步确定的3-5个核心子系统。从这些模块及其内部依赖项中提取契约。注意CONTRACTS.md头中的作用域，以便审阅者知道所涵盖的内容。
- **非常大的项目（bbb500源文件）：**建议用户一次将管道范围扩展到一个子系统。每个子系统都有自己的管道运行，产生自己的REQUIREMENTS.md、CONTRACTS.md等。告诉用户：“这个项目有N个源文件。为了获得最佳结果，为每个主要子系统分别运行需求管道（例如，“为认证模块生成需求”）。一个单一的管道运行由于上下文限制，整个代码库将错过契约。”如果用户明确要求在大型代码库上进行整个项目范围的测试，那么就满足这个要求，但要注意覆盖范围将比子系统级别的运行要小。

初始传递的范围宽度

在第一次管道运行时，宽度比深度更重要。覆盖所有主要的子系统和模块，而不是深入其中的几个。目标是一个广泛的基线，自细化循环和后来的review/refinement可以深化该基线。如果你只关注3个模块而忽略了其他8个模块，那么完整性检查将无法发现它从未发现过的模块中的漏洞。对于同时包含核心库和支持模块（中间件、插件、适配器、扩展）的项目，在阶段a中至少包括核心模块和风险最高的支持模块。请注意CONTRACTS.md头文件中的范围，以便清楚地了解哪些是覆盖的，哪些是不覆盖的。细化传递可以在以后扩展范围，但是初始传递应该撒下上下文窗口允许的最宽的网。

合同提取

读取每个源文件（在范围内），列出它实现或应该实现的每个行为契约。行为契约是代码对调用者做出的任何承诺：**METHOD**：公共方法对返回值、副作用、异常、线程安全的保证
- **NULL**：当NULL被传递、返回或存储时会发生什么
—**CONFIG**：配置选项在其边界处的影响
- **ERROR**：抛出什么异常，何时抛出，以及使用什么诊断信息
- **INVARIANT**：必须始终保持的属性
- **COMPAT**：保留向后兼容的行为
- **ORDER**:output/iteration顺序是否稳定、有文件记录或未定义
- **生命周期**：资源creation/cleanup，初始化排序
- **THREAD**：线程安全保证或要求

合同提取规则- **要彻底。**对于200行文件，预计5-15个合同。对于1000行文件，预计有20-40行。如果您在一个文件中发现少于3个具有真正逻辑的契约，那么您就跳过了一些内容。
- **包含内部文件。**内部契约很重要，因为公共API依赖于它们。
**包括“应该存在”的契约——代码没有做但应该基于其领域做的事情。这些可以捕获缺勤错误。
**阅读代码，而不仅仅是Javadoc/docstrings.**当文档和代码不一致时，列出两者。
这是发现，不是判断。列出所有事情，即使看起来很明显。

输出格式```
# Behavioral Contract Extraction
Generated: [date]
Source files analyzed: N
Total contracts extracted: N

## Summary by category
- METHOD: N
- NULL: N
- CONFIG: N
[etc.]

### path/to/file.ext (N contracts)

1. [METHOD] ClassName.methodName(): description of what it guarantees
2. [NULL] ClassName.methodName(): what happens when null is passed/returned
[etc.]
```
---

要求标题格式REQUIREMENTS.md中的所有需求都必须使用格式`### REQ-NNN: Title`，其中NNN是一个填零的三位数，而Title是一个简短的描述性名称。不要使用替代格式，如`### REQ-NNN — Title`、`### REQ-NNN. Title`、`**REQ-NNN**: Title`或不带数字的自由格式标题。一致的格式使自动化工具能够解析和交叉引用需求。

---

阶段B：从合同中派生需求

**输入：**`quality/CONTRACTS.md`，项目文档，SKILL.mdstep7模板。
* *输出:* *`quality/REQUIREMENTS.md`###如何工作

* * B。1 -集团相关合同。**跨不同文件的许多契约服务于相同的行为需求。根据他们的行为进行分类，而不是根据档案。不要合并不相关的契约，因为它们在同一个文件中。* * B。2 -用心充实。**对于每个组，从文档中找到用户故事：GitHub问题说明用户期望什么，用户指南说明预期行为，故障排除文档揭示已知的边缘情况，设计文档解释设计目标。“so that”从句必须来自于对谁在乎以及为什么在乎的理解。

* * B。3 -写需求。**使用SKILL.md步骤7中的7字段模板。满意的条件来自于群体中的个体契约——每一个契约都成为满意的条件。

* * B。4 -检查孤儿合同。**在写完所有需求后，验证CONTRACTS.md中的每个合同都被涵盖。未覆盖的契约成为新的需求，或者被添加到现有需求的满足条件中。

# # #规则- **不要限制需求计数。根据合同要求写多少就写多少。
- **每个合同必须至少映射到一个需求
- **每个不同的行为关注一个要求。**不要仅仅因为“线程安全”和“空处理”在同一个类中就合并它们。
- **不修改CONTRACTS.md。**只读它。

---

阶段C：验证覆盖率（循环，最多3次迭代）

**输入：**`quality/CONTRACTS.md`，`quality/REQUIREMENTS.md`**输出：**`quality/COVERAGE_MATRIX.md`，更新后的`quality/REQUIREMENTS.md`对于CONTRACTS.md中的每个合同，确定它是否被需求覆盖。如果需求的满足条件明确地测试了行为，则合同是“覆盖的”。如果合同只是被间接提及，暗示但没有说明，或者同一文件的不同方面被涵盖，但具体合同没有被涵盖，则合同不被涵盖。

输出格式```
# Contract Coverage Matrix
Generated: [date]
Total contracts: N
Covered: N (percentage)
Uncovered: N (percentage)
Partially covered: N (percentage)

## Fully covered contracts
[file]: [contract summary] → REQ-NNN (conditions of satisfaction #M)

## Partially covered contracts
[file]: [contract summary] → REQ-NNN covers the general area but misses [specific aspect]

## Uncovered contracts
[file]: [contract summary] → No requirement addresses this behavior
```
在编写矩阵之后，修复REQUIREMENTS.md中的空白：向现有需求添加缺失的条件或创建新需求。报告的变化。

**循环终止：**如果未覆盖计数达到0，则继续进行阶段d。否则，重新生成矩阵并再次检查。最多3次迭代。

---

阶段D：完整性检查

**输入：**`quality/REQUIREMENTS.md`，`quality/CONTRACTS.md`,`quality/COVERAGE_MATRIX.md`，源树。
**输出：**`quality/COMPLETENESS_REPORT.md`，更新`quality/REQUIREMENTS.md`这是故事结束前的最后一扇门。运行三次检查：

检查1：域完整性

以下行为领域必须有需求。检查每一个。这个清单是最低限度的——如果你注意到一个没有列出的域应该有这个项目的域的需求，添加它。-[] **空处理：**显式Null，缺席字段，空键，集合中的空值
-[] **类型强制：** string↔数字、字符串↔布尔型、数字精度、溢出
-[] **原语与包装器：**原语与对象在反序列化过程中的空语义（对于有这种区别的语言）
-[]泛型类型：**擦除边界，通配符处理，递归泛型（对于具有泛型的语言）
-[] **线程安全：**并发访问，发布安全，缓存可见性
-[] **错误诊断：**异常类型、路径上下文、位置信息
-[] **资源管理：**流关闭，reader/writer生命周期
-[] **向后兼容性：**线格式稳定，API行为稳定
-[] **安全性：** DoS保护（嵌套深度，字符串长度），防止注入
-[] **编码：** Unicode， BOM，代理对，转义序列
- [] **Date/time:**格式优先级，时区处理，精度
-[] **集合：**数组，列表，集合，映射，队列-空，空元素，排序
—[]**枚举：**名称解析、别名、未知值
-[] **多态：**运行时类型vs声明类型，adapter/handler委托
-[] **树模型/中间表示：**突变语义，深度拷贝结构独立性，空归一化
-[] **配置：**构建器不变性，实例隔离，选项组合
-[] **入口点：**每个不同的公共入口点必须有自己的契约——基于字符串的、基于流的、基于树的、独立解析的、多值解析的。如果库有N种开始读或写的方式，那么就必须有N组契约。
-[] **输出转义：**默认情况下转义哪些字符，禁用转义会改变什么，构建器级和写入器级控件如何交互
- [] **内置类型处理程序契约：**对于每个处理标准库类型的内置处理程序，声明它对格式、精度、规范化和往返保真度的承诺。需求应该指定处理程序的承诺，而不仅仅是一个处理程序存在。
- [] **Field/property序列化顺序：**输出顺序是否遵循声明顺序、继承顺序、字母顺序或未定义。说明订购是承诺的契约还是仅仅是观察到的行为。
-[] **公共类型的身份契约：**`toString()`，`hashCode()`/`equals()`（或同等语言）这些是用户用于比较、记录和收集键使用的行为契约。
-[] **输入验证：**对于每个有域约束的配置字段，说明有效范围和是否存在验证。对于每个领域，要么引用覆盖该领域的REQ-NNN数字，要么将其标记为空白。

检查2：可测试性审计

对于每个需求，检查其满足条件是否实际上是可测试的。审稿人可以根据这种情况编写具体的测试用例吗？pass/fail是明确的吗？这个条件是否涵盖了失败模式，而不仅仅是快乐之路？

检查3：跨需求一致性

检查引用相同概念的需求对。范围是否一致？空处理规则是否一致？线程安全保证与生命周期契约冲突吗？配置默认值是否匹配不同的需求？

检查4：跨工件一致性（如果存在代码审查或规范审计结果）如果`quality/code_reviews/`或`quality/spec_audits/`包含先前或当前运行的结果，则读取它们。对于每一个状态为violation、BUG或INCONSISTENT的发现，检查需求是否处理了寻找目标的行为关注点。如果代码审查在压缩头解析中发现了需求没有涵盖的缺陷，那就是完全性缺口——添加一个需求或满足条件来弥补它。

**如果存在未处理的发现，完整性报告不能显示COMPLETE。**如果任何VIOLATED/BUG/INCONSISTENT从代码审查或规范审计中发现的目标行为不包括在需求中，那么结论必须是不完整的，并列出了具体的差距。这种检查之所以存在，是因为管道的早期版本产生的完整性报告说“COMPLETE”，而在同一运行中的代码审查发现了需求违反。完整性报告必须与所有其他质量工件一致。

审查后完整性刷新（强制）

**代码审查和规范审核完成后**重新阅读`quality/COMPLETENESS_REPORT.md`并更新。最初的完整性报告是在代码审查和规范审计运行之前编写的，因此它不能反映他们的发现。这个刷新步骤将完整性判定与实际的评审结果相协调。

* *程序:* *
1. 阅读`quality/code_reviews/`- count violate和BUG发现的综合摘要。
2. 阅读来自`quality/spec_audits/`的分类摘要-统计已确认的代码错误。
3. 对于每个发现，检查REQUIREMENTS.md是否有覆盖该行为的需求。
4. 添加`## Post-Review Reconciliation`段到COMPLETENESS_REPORT.md：```
## Post-Review Reconciliation
Updated: [date]

### Code review findings: N VIOLATED, M BUG
- [finding summary] → covered by REQ-NNN / NOT COVERED (gap)
- ...

### Spec audit findings: N confirmed code bugs
- [finding summary] → covered by REQ-NNN / NOT COVERED (gap)
- ...

### Updated verdict
[COMPLETE if all findings are covered by requirements, INCOMPLETE if gaps remain]
```
5. 如果原来的判决是完整的，但存在未解决的发现，将判决改为不完整。

解决代码审查与规范审计的冲突

当代码审查和规范审计对相同的行为声明产生分歧时——一个说是BUG，另一个说是设计选择或误报——协调必须解决冲突，而不是掩盖它。

* *解析过程:* *
1. 找出分歧中心的事实主张。代码实际上是做什么的？
2. 部署一个验证探测器：给一个模型有争议的主张和相关的源代码，并要求它报告地面真相。（参见`spec_audit.md`§“验证探针”）
3. 将解决方案记录在回顾后的对账部分：   ```
   ### Conflicts resolved
   - [finding description]: Code review said [X], spec audit said [Y].
     Verification probe: [what the code actually does].
     Resolution: [BUG CONFIRMED / FALSE POSITIVE / DESIGN CHOICE]. [Explanation.]
   ```
4. 如果解决方案确认了BUG，请确保它具有回归测试。如果解决方案推翻了一个BUG，那么按照`review_protocols.md`§“在规范审计逆转之后进行清理”清理回归测试。

**不要默认使用一个源来解决冲突。**代码审查和规范审计都不会自动变得更权威——它们使用不同的方法（结构阅读和规范比较），并且有不同的盲点。验证探针是决定性因素。

**此刷新不是可选的。**在代码审查之前的完整性报告是时间戳，而不是质量检验关。刷新将其转换为实际的对账。

输出格式```
# Completeness Report
Generated: [date]

## Domain coverage
[For each domain: COVERED (REQ-NNN, REQ-NNN) or GAP (description)]

## Testability issues
[For each vague requirement: REQ-NNN — condition N is not testable because...]

## Consistency issues
[For each conflict: REQ-NNN and REQ-NNN disagree about...]

## Cross-artifact gaps (if code review/spec audit results exist)
[For each unaddressed finding: finding summary → missing requirement or condition]

## Verdict
COMPLETE or INCOMPLETE with recommended actions
```
然后修复您所能修复的：为领域差距添加需求，细化模糊的条件，解决一致性问题，并关闭跨工件的差距。

**重要：**这是最后的检查。是敌对的。假设之前的通过并不完美。对于每一个标有COVERED的领域，请确认所引用的需求实际上满足了清单上的项目——不要只是勾选复选框。

自优化循环（最多3次迭代）

在最初的完整性检查之后，运行3次细化迭代，以缩小阶段D确定的差距：1. **阅读完整性报告。**识别所有GAP条目、可测试性问题和一致性问题。
2. **修正了REQUIREMENTS.md的间隙。**对于每个GAP：使用7字段模板添加新需求，或在现有需求中添加满足条件。对于可测试性问题：提高条件。对于一致性问题：解决冲突。
3. **重新运行所有三项检查**（域完整性、可测试性审计、跨需求一致性）。将更新后的结果写入COMPLETENESS_REPORT.md。
4. **数一下delta。**在此迭代中添加了多少新需求或修改了多少现有需求？
5. **短路检查：**如果delta少于3个变化，停止-你已经达到了递减收益。进入E阶段。**为什么这样做：**最初的完整性检查确定了差距，但模型可能不会一次修复所有的差距，特别是概念上的差距，模型需要重新读取源文件以了解缺失的内容。每次迭代都会缩小差距。三次迭代足以弥补机械差距；剩下的概念差距是跨模型审计和人工审查的地方。

**为什么有限制：**这是自我改进-相同的模型检查自己的工作。它可以捕捉到一旦被指出模型就能看到的空白（未覆盖的领域、模糊的条件、数字不一致），但不会捕捉到模型不能识别为空白的盲点。这是设计好的。审查和改进协议的存在是为了通过不同的模型或人工输入来缩小这些更深层次的差距。

回路完成（或短路）后，进入阶段E。

---阶段E：叙述通过

**输入：**`quality/REQUIREMENTS.md`，`quality/CONTRACTS.md`，项目文档，源代码树。
**输出：**重组`quality/REQUIREMENTS.md`**启动前：**保存备份：`cp quality/REQUIREMENTS.md quality/REQUIREMENTS_pre_narrative.md`此阶段将规范转换为指南。添加解释性组织，以便新的团队成员、代码审阅者或AI代理可以从头到尾阅读文档并理解软件。

### E.1 -项目概述（新建，位于文档顶部）

写一篇400-600字的连贯散文，解释软件是什么，谁使用它，为什么使用它（主要人物和目标），数据如何流经主要组件，以及设计哲学（关键的架构决策和为什么做出这些决策）。

E.2 -用例（新的，经过概述）

以应用软件项目管理（Stellman & Greene）的风格编写6-8个用例。每个人都有:—**名称**：简短的描述性名称
- **演员**：由谁发起
- **先决条件**：在此开始之前必须为真
- **步骤**：编号为actor/system的动作顺序
- **后置条件**：成功时的真实情况
—**备选路径**：变体和错误案例
- **需求**：这个用例练习的REQ-NNN编号

涵盖主要的使用模式。用例是“软件做什么”和“需求指定什么”之间的桥梁。

E.3 -横切关注点（新的，在用例之后）

记录跨越多个类别的体系结构不变量：线程模型、空契约、错误哲学、向后兼容策略、配置组合。每个引用特定的REQ-NNN编号。把段落写成散文。

E.4 -类别叙述（扩充现有）对于每个需求类别，在第一个需求之前添加2-4个句子，解释该类别涵盖的内容，它与其他类别的关系，以及审稿人应该记住的内容。

### E.5 -重新排序自顶向下的流程

从面向用户（入口点、配置）到基础结构（错误处理、向后兼容性）重新排序类别。将所有的部分折叠到适当的类别中。

### E.6 -按顺序重新编号

重新订购后，按照文件顺序将所有要求重新编号为REQ-001至REQ-NNN。更新所有内部交叉引用。

# # #规则

- **不要删除、合并或削弱任何现有的需求
- **不要在此阶段添加新的需求
- **从用户的角度写概述和用例
- **用例必须引用特定的REQ号

---

版本控制协议

版本方案：major.minor- **重大**碰撞：结构变化（新的管道架构，叙事通道添加，主要范围扩展）。被用户撞了。
- **次要**颠簸：细化通过，间隙填充，锐化条件。在每次管道运行或细化过程中自动递增。### VERSION_HISTORY.md
在`quality/VERSION_HISTORY.md`维护一个版本历史文件：```markdown
# Requirements Version History

## Current version: vX.Y

| Version | Date | Model | Author | Reqs | Summary |
|---------|------|-------|--------|------|---------|
| v1.0 | YYYY-MM-DD | [model] | Quality Playbook | N | Initial pipeline generation |
| v1.1 | YYYY-MM-DD | [model] | [author] | N | [what changed] |

## Pending review
[status from REFINEMENT_HINTS.md if review is in progress]
```
**Author**列记录出处：“Quality Playbook”用于自动管道运行，一个人的名字用于手动编辑，一个模型的名字用于细化通过。

备份协议

每次版本变更前，将所有质量文件拷贝到`quality/history/vX.Y/`：```
quality/history/
├── v1.0/
│   ├── REQUIREMENTS.md
│   ├── CONTRACTS.md
│   ├── COVERAGE_MATRIX.md
│   └── COMPLETENESS_REPORT.md
├── v1.1/
│   └── ...
└── v2.0/
    └── ...
```
每个版本文件夹都是一个完整的快照。用户可以区分任意两个版本。

###版本冲压REQUIREMENTS.md报头包含当前版本：```markdown
# Behavioral Requirements — [Project Name]
Version: vX.Y
Generated: [date]
Pipeline: contract-extraction v2 with narrative pass
```
---

在管道之后：审查和细化

管道产生了一个坚实的基线，但人工智能并不是100%可靠。该技能为迭代改进提供了两个独立的工具：

需求评审（`quality/REVIEW_REQUIREMENTS.md`）

通过用例组织的需求的交互式或有指导的评审。三种模式:
- **自我引导**：选择要深入的用例
- **完全引导**：按顺序遍历用例
- **跨模型审计**：不同的模型对完整性报告进行事实检查

在`quality/REFINEMENT_HINTS.md`中跟踪进度和反馈。有关完整协议，请参阅生成的`quality/REVIEW_REQUIREMENTS.md`。

需求细化（`quality/REFINE_REQUIREMENTS.md`）

读取`quality/REFINEMENT_HINTS.md`并更新`quality/REQUIREMENTS.md`以关闭已识别的差距。可以运行与任何模型。备份当前版本，升级次要版本，报告所有更改。有关完整协议，请参阅生成的`quality/REFINE_REQUIREMENTS.md`。

多模型细化用户可以使用不同的模型运行改进通道，以捕获不同的盲点。每一关：备份→优化→版本升级→登录VERSION_HISTORY.md。运行尽可能多的模型，直到收益递减。