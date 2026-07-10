#验证清单（第6阶段：验证）

在宣布质量手册完成之前，请检查下面的每个基准。如果有任何失败，回去修复它。

自我检查基准

# # # 1。测试数

计算启发式目标：（可测试规范部分）+ （QUALITY.md场景）+（步骤5中的防御模式）。

- **远低于目标**→你可能错过了规格要求或略读防御模式。回去检查一下。
- **接近目标**→检查是否检测了阴性病例和边界。
- **高于目标**→好，只要每次测试都是有意义的。不要输入数字。

# # # 2。场景覆盖

计算QUALITY.md中的场景。计算功能测试文件中的场景测试函数。这些数字必须完全匹配。

# # # 3。Cross-Variant覆盖

如果项目处理N个输入变量，测试执行全部N个变量的百分比是多少？计数：循环或参数化所有变量/总测试的测试。

* *启发式:~ 30%。**如果下面有，寻找应该参数化的单变量测试。常见候选项：结构完整性、身份验证、必需的字段存在、数据关系、语义正确性。确切的百分比比确保跨所有变体测试横切属性更重要。

# # # 4。边界和阴性检测计数

计算步骤5中的防御模式。数一下你的boundary/negative测试。比例应接近1:1。如果非常低，那么编写更多针对未经测试的防御模式的测试。

# # # 5。断言深度

扫描你的断言。存在检查和价值检查有多少？如果超过一半是仅存在的（`assert x is not None`、`assert x in output`），则加强它们以检查实际值。

# # # 6。层的正确性对于每个测试，都要问：“我是在测试需求还是机制？”如果任何测试只断言引发了特定的错误类型，而没有验证管道输出，那么它是在测试机制。重写以测试结果。

# # # 7。突变的有效性

对于更改fixture的每个测试，请验证步骤5b模式映射的“accept”列中的更改值。如果任何突变使用模式拒绝的类型，则测试失败并显示验证错误，而不是测试防御代码。解决它。

# # # 8。所有测试都通过——零失败和零错误

使用项目的测试运行器运行测试套件：- **Python:**`pytest quality/test_functional.py -v`—**Scala:**`sbt "testOnly *FunctionalSpec"`- **Java:**`mvn test -Dtest=FunctionalTest`或`gradle test --tests FunctionalTest`**TypeScript:**`npx jest functional.test.ts --verbose`**Go:**`go test -v`针对生成的测试文件的包-使用项目现有的模块和包布局
**Rust:**`cargo test`瞄准生成的测试——要么是`tests/`中的集成测试目标，要么是内联`#[cfg(test)]`测试目标，匹配项目的约定

**检查失败和错误。**大多数测试框架区分测试失败（断言错误）和测试错误（设置失败，缺少fixture，import/resolution错误，初始化期间的异常）。两者都是失败的测试。一个常见的错误：生成引用不存在的共享fixture或帮助程序的测试。这些显示为设置错误，而不是断言失败——但它们同样是错误的。**预期失败（xfail）测试不计入此基准测试。**`quality/test_regression.*`中的回归测试使用预期故障标记（`@pytest.mark.xfail(strict=True)`、`@Disabled`、`t.Skip`、`#[ignore]`）来确认已知bug仍然存在。这些测试“应该”会失败——这才是关键。“零失败和零错误”基准适用于`quality/test_functional.*`（功能测试套件），而不适用于`quality/test_regression.*`（bug确认套件）。如果测试运行器报告了xfail标记的回归测试的失败，那就是正确的行为，而不是违反基准。如果一个xfail测试意外地“通过”，那就意味着bug已经修复，xfail标记应该被删除——把它当作一个需要调查的发现，而不是测试失败。

运行后，检查：
—所有测试通过—count必须等于测试总数
-零故障
—errors/setup零故障如果有设置错误，可能是您忘记创建fixture/setup文件，或者您引用了不存在的帮助程序。返回并创建它们或重写测试以使其自包含。

# # # 9。现有测试未中断

运行项目的完整测试套件（不只是新测试）。你的新文件不应该破坏任何东西。

##文档验证

# # # 10。QUALITY.md场景参考真实代码和标签来源

每个场景都应该提到代码库中存在的实际函数名、文件名或模式。为每个引用执行Grep以确认其存在。

如果从非正式需求着手，请验证每个场景和测试都包含一个使用规范格式的需求标签：`[Req: formal — README §3]`、`[Req: inferred — from validate_input() behavior]`、`[Req: user-confirmed — "must handle empty input"]`。推断的需求应该被标记出来，以便在阶段7的交互会话中进行用户评审。

# # # 11。RUN_CODE_REVIEW.md是自包含的没有事先背景的AI应该能够阅读它并执行有用的评论。检查：是否列出引导文件？它是否有特定的重点领域？护栏还在吗？

# # # 12。RUN_INTEGRATION_TESTS.md是可执行的和字段精确的

每个命令都应该有效。每次检查都应该有一个具体的pass/fail标准——不是“验证它看起来是正确的”，而是一个特定的预期结果。

**确认质量闸门是从字段参考表中编写的，而不是从内存中编写的。**检查：1. 在RUN_INTEGRATION_TESTS.md中存在一个字段引用表，每个模式中的每个字段都有一行
2. **字段计数检查：**对于每个模式，计算实际模式文件中的字段和表中的行数。如果数字不匹配，则说明您漏掉了字段或虚构了字段。最常见的错误是：模式有8个字段，但表只有2-3个“重要”字段。
3. **字符对字符检查：**现在重新读取每个模式文件，并将表中的每个字段名与文件内容进行比较。`document_id`≠`doc_id`。`sentiment_score`≠`sentiment`。`classification`≠`category`。
4. 每个类型和约束都匹配模式（`float 0-1`不是`int 0-100`，`string enum`不是`integer`）

如果任何字段名称、计数或类型错误，请在继续之前修复它。桌子是基础——如果桌子是错误的，那么每一个建立在它之上的质量门都是错误的。

# # # 13。RUN_SPEC_AUDIT.md提示符可复制粘贴最终审计提示应该在不修改的情况下粘贴到Claude Code、Cursor和Copilot中工作（除了文件引用语法）。

# # # 14。结构化的输出模式是有效和一致的

验证`RUN_TDD_TESTS.md`和`RUN_INTEGRATION_TESTS.md`都指示代理产生：
-使用框架的本机报告器输出JUnit XML （pytest`--junitxml`, gotestsum`--junitxml`， Maven Surefire报告，`jest-junit`,`cargo2junit`）
—一个sidecar JSON文件（`tdd-results.json`或`integration-results.json`）在`quality/results/`检查每个协议的JSON模式是否包含所有必填字段：
- **tdd-results.json:**`schema_version`,`skill_version`,`date`,`project`,`bugs`,`summary`。每个bug:`id`，`requirement`,`red_phase`,`green_phase`,`verdict`,`fix_patch_present`,`writeup_path`。
- **integration-results.json:**`schema_version`,`skill_version`,`date`,`project`,`recommendation`,`groups`,`summary`,`uc_coverage`。每组：`group`，`name`,`use_cases`,`result`。验证协议不包含平面命令列表模式（不包含`"groups"`的`"results"`或`"commands_run"`数组是不兼容的）。验证verdict/result枚举值仅使用SKILL.md中定义的允许值（例如，TDD判定为`"TDD verified"`、`"red failed"`、`"green failed"`、`"confirmed open"`；集成结果为`"pass"`、`"fail"`、`"skipped"`、`"error"`；推荐为`"SHIP"`、`"FIX BEFORE MERGE"`、`"BLOCK"`）。TDD判定`"skipped"`已被弃用——将`"confirmed open"`与`red_phase: "fail"`和`green_phase: "skipped"`一起使用。TDD摘要必须包括`confirmed_open`计数以及`verified`、`red_failed`和`green_failed`计数。

两个sidecar JSON模板都必须使用`schema_version: "1.1"`（v1.1更改：`verdict: "skipped"`已弃用，改为`"confirmed open"`）。这两个协议都必须包含一个写后验证步骤，指示代理在写完sidecar JSON后重新打开它，并验证所需的字段、枚举值和没有额外的未记录的根键。# # # 15。补丁验证门是可执行的

对于每个已确认的带有补丁的bug，请验证：
1. 在补丁验证门中指定的`git apply --check`命令使用正确的补丁路径（`quality/patches/BUG-NNN-*.patch`）
2.compile/syntax检查命令匹配项目的实际构建系统—而不是通用占位符
3. 对于解释性语言（Python、JavaScript），该门指定适当的语法检查（`python -m py_compile`、`node --check`、`pytest --collect-only`，或同等）
4. 该门包括一个临时工作树或存储-恢复指令，以符合源边界规则

# # # 16。回归测试跳跃防护存在

输入`quality/test_regression.*`以获得适合于语言的skip/xfail机制。每个测试函数都必须有一个守卫：
- Python:`@pytest.mark.xfail`或`@unittest.expectedFailure`—Go:`t.Skip(`—Java:`@Disabled`—Rust:`#[ignore]`—TypeScript/JavaScript:`test.failing(`、`test.fails(`、`it.skip(`当测试套件在未打补丁的代码上运行时，没有跳过保护的回归测试将导致意外的失败。每个守卫都必须引用错误ID （bug - nnn格式）和修复补丁路径。

# # # 17。集成组命令通过飞行前发现

对于`RUN_INTEGRATION_TESTS.md`中的每个集成测试组命令，验证该命令使用框架的干运行模式发现至少一个测试（`pytest --collect-only`、`go test -list`、`vitest list`、`jest --listTests`、`cargo test -- --list`）。命令发现失败的组将产生`covered_fail`结果，该结果将选择器错误掩盖为代码错误。如果无法验证命令（没有可用的干式运行模式），请注意该限制。

# # # 18。出现在所有生成的文件上的版本邮票为归属行`Generated by [Quality Playbook]`在`quality/`中创建每个生成的Markdown文件。Grep为`Generated by Quality Playbook`生成的每个代码文件。每个文件都必须有正确版本号的印章。没有戳记的文件不能追溯到创建它们的工具和版本。**豁免：** sidecar JSON文件（使用`skill_version`字段），JUnit XML文件（框架生成）和`.patch`文件（戳戳会破坏`git apply`）。对于带有shebang或encoding pragma的Python文件，请验证戳记位于pragma之后，而不是之前。

# # # 19。执行枚举完整性检查验证代码审查（第1步和第2步）在代码使用`switch`/`case`、`match`或if-else链对命名常量进行分派的地方执行了机械的双列表枚举检查。对于每个这样的检查，检查必须显示：(a)在headers/enums/specs中定义的常量列表，(b)代码中实际出现的case标签列表，(c)任何空白。声称“白名单涵盖所有价值”或“所有案件都得到处理”的审查，而没有显示两个名单的比较是不符合的——这是检查防止的特定幻觉模式。

# # # 20。为所有已确认的Bug生成的Bug记录对于`tdd-results.json`中的每个bug (`verdict: "TDD verified"`和`verdict: "confirmed open"`)，验证是否存在相应的`quality/writeups/BUG-NNN.md`文件，并且`tdd-results.json`对于该bug有一个非空的`writeup_path`。每一个书面报告必须包括：总结、规范参考、代码引用、可观察的结果、修复差异和测试描述。没有记录的确认bug是不完整的。

# # # 21。分类验证探测包括可执行证据打开分类报告（`quality/spec_audits/YYYY-MM-DD-triage.md`）。对于通过验证探测确认或拒绝的每个发现，验证分类条目是否包含测试断言（而不仅仅是散文推理）。拒绝必须包含一个pass断言来证明发现是错误的。确认必须包含一个FAILING断言来证明bug的存在。每个断言必须引用一个精确的行号。仅基于散文推理（“第3527-3528行显式保留X”）而没有机械断言的分类决策是不符合的。

# # # 22。枚举列表是从代码中提取的，而不是从需求中复制的当代码审查包括枚举检查（例如，“函数X中出现的case标签”）时，验证代码端列表包含来自实际源代码的每个项目行号。如果列表与需求列表逐字匹配，没有行号，那么枚举很可能是复制的，而不是提取的，必须重做。还要验证分类预审计抽查是否报告了引用行的实际内容（“行3527包含`default:`”），而不仅仅是确认声明（“行3527保留RING_RESET”）。

# # # 23。机械验证工件存在并通过完整性检查对于断言函数handles/preserves/dispatches的每个契约或需求，都有一组命名常量（特征位、枚举值、操作码表），验证是否存在相应的`quality/mechanical/<function>_cases.txt`文件，并由非交互式shell管道生成。引用分派功能覆盖而不引用机械工件的契约是不一致的。**完整性检查（必选）：**运行`bash quality/mechanical/verify.sh`。该脚本重新执行生成每个机械工件的相同提取命令，并改变结果。如果ANY diff非空，则工件被篡改了—模型可能编写了预期的输出，而不是捕获实际的shell输出。不匹配的工件必须通过重新运行提取命令（而不是通过编辑文件）来重新生成。之所以存在这种检查，是因为在v1.3.19中，模型执行了正确的awk/grep命令，但是在实际命令只产生8行时，将捏造的9行输出（包括虚构的`case VIRTIO_F_RING_RESET:`）写入文件。

# # # 24。执行源代码检查回归测试（编号`run=False`）Grep`quality/test_regression.*`为`run=False`(Python)，`t.Skip`带有源代码检查注释，或等效的跳过机制。任何旨在验证源结构的回归测试（函数体中的字符串是否存在、大小写标签是否存在、枚举提取）都必须执行——它不能使用`run=False`。这些测试是安全的、确定的字符串匹配操作。实际失败的`xfail(strict=True)`测试报告为XFAIL（预期），这是正确的行为。使用`run=False`进行源代码检查测试是最糟糕的可能状态：存在正确的检查，但从未触发。

# # # 25。矛盾之门已过（实证vs散文）在闭包时验证没有已执行的工件与散文工件相矛盾。具体来说：(a)如果任何`quality/mechanical/*`文件显示常量不存在，则没有散文工件（`CONTRACTS.md`、`REQUIREMENTS.md`、代码审查、分类）可以声称它存在；(b)如果`xfail`的任何回归测试实际上失败（XFAIL），`BUGS.md`在没有提交引用的情况下可能不会声称错误“在工作树中固定”；(c)如果TDD可追溯性显示红色阶段故障，则分类可能不会声称相应的代码是合规的。任何矛盾都必须在结束之前解决。

# # # 26。版本邮票一致性从SKILL.md元数据中读取`version:`字段（在技能安装目录中找到SKILL.md—通常是`.github/skills/SKILL.md`或`.claude/skills/quality-playbook/SKILL.md`）。检查每个生成的工件：PROGRESS.md的`Skill version:`字段、每个`> Generated by`归属行、每个代码文件头戳和每个侧车JSON`skill_version`字段。每个版本戳必须与SKILL.md元数据完全匹配。单个不匹配就是基准测试失败。这种检查的存在是因为在v1.3.21基准测试中，由于硬编码模板，9个repos中有5个具有来自旧技能版本的版本戳。

# # # 27。机械目录一致性如果存在`quality/mechanical/`，则必须至少包含一个`verify.sh`文件。不支持使用空目录`quality/mechanical/`。如果不存在分派函数契约，则不应该存在该目录—而是在PROGRESS.md中记录`Mechanical verification: NOT APPLICABLE`。如果该目录存在提取工件，那么`verify.sh`必须为每个保存的文件包含一个验证块（而不仅仅是一个）。当存在多个工件时，只检查一个工件的verify.sh是不完整的。

# # # 28。TDD工件闭包

如果`quality/BUGS.md`包含任何已确认的bug，则`quality/results/tdd-results.json`为必选。如果任何bug有红色阶段的结果，`quality/TDD_TRACEABILITY.md`也是强制性的。零bug库可以省略这两个文件。对于无法执行TDD的repos，tdd-results.json必须存在，并带有`verdict: "deferred"`和一个解释原因的`notes`字段。

# # # 29。Triage-to-BUGS.md同步在规范审计分类之后，每一个被确认为代码错误的发现都必须出现在`quality/BUGS.md`中。带有已确认的代码错误而没有相应的BUGS.md条目的分类报告是不一致的。当确认bug存在时，如果BUGS.md不存在，则必须创建该bug。

# # # 30。所有已确认的bug的说明

每个确认的bug （TDD-verified或confirmed-open）都必须在`quality/writeups/BUG-NNN.md`上有一个书面记录。对于没有修复补丁的已确认打开的bug，撰文指出缺少fix/green-phase证据。已确认错误且没有写入目录的运行是不完整的。

# # # 31。第4阶段分类文件存在

在`quality/spec_audits/YYYY-MM-DD-triage.md`上存在分类文件之前，阶段4还没有完成。如果只有审计师报告而没有分类综合，则阶段4是不完整的。

# # # 32。机械执行种子检查（延续模式）当`quality/previous_runs/`存在并且运行阶段0时，验证生成的`quality/SEED_CHECKS.md`对先前运行的每个唯一错误都有一个条目。每个种子必须有一个通过实际运行断言获得的机械验证结果（FAIL = bug仍然存在，PASS = bug已修复），而不是通过读取先前运行的散文。如果种子的回归测试存在于先前的运行中，则必须针对当前源树重新执行断言。标记为FAIL而不执行断言的种子是不一致的。此基准测试仅在延续模式处于活动状态（存在先前运行）时应用。

# # # 33。PROGRESS.md记录的收敛状态（延续模式）当阶段0运行时，验证PROGRESS.md包含`## Convergence`部分，其中包含：运行数、种子计数、净新bug计数和CONVERGED/NOT聚合结论。net-new计数必须等于BUGS.md中不匹配任何文件：行种子的错误数。当`SEED_CHECKS.md`存在时，一个缺失的收敛段是不符合的。此基准测试仅在延续模式处于活动状态时应用。

# # # 34。BUGS.md始终存在每次完成运行必须产生`quality/BUGS.md`。如果运行确认了源代码错误，BUGS.md必须列出它们。如果运行没有发现任何源代码错误，BUGS.md必须包含一个带有肯定断言的`## Summary`：“没有发现已确认的源代码错误”，并计算和消除候选程序的计数。没有BUGS.md的已完成运行（阶段5标记为已完成）是不合格的。这个基准测试之所以存在，是因为在v1.3.22基准测试中，express完成了所有阶段，源代码错误为零，但没有产生BUGS.md，这使得该文件是故意省略还是意外跳过变得不明确。

# # # 35。即时机械完整性闸（第2a期）如果存在`quality/mechanical/`，请验证在编写每个`*_cases.txt`之后立即执行了`bash quality/mechanical/verify.sh`—在任何合约、需求或分类工件引用提取之前。证据：存在`quality/results/mechanical-verify.log`和`quality/results/mechanical-verify.exit`，且退出文件中包含`0`。如果这些收据文件丢失或退出代码非零，则在创建时未验证机械提取。这个基准测试之所以存在，是因为v1.3.23将验证延迟到阶段6，允许下游工件（CONTRACTS.md、REQUIREMENTS.md、分流探测）在不匹配被捕获（未被捕获）之前构建在整个运行的伪造提取上。

# # # 36。在分诊探查中不用作证据的机械文物查找`open('quality/mechanical/`或`cat quality/mechanical/`的所有分类和验证探测文件（`quality/spec_audits/*`）。如果任何探针读取`quality/mechanical/*.txt`文件作为源文件所包含内容的唯一证据，则这是循环验证，基准测试将失败。探测器必须直接读取源文件或重新执行提取管道。这个基准测试之所以存在，是因为v1.3.23 Probe C验证了伪造的机械工件而不是源代码，传递了伪造的数据。

# # # 37。第6阶段机械闭包使用Bash（不是Python替代）如果`quality/mechanical/`存在，请验证阶段6是否将`bash quality/mechanical/verify.sh`作为一个文字shell命令运行—而不是一个读取工件文件的Python脚本。证据：`quality/results/mechanical-verify.log`包含bash脚本的输出（像“OK：…”或“MISMATCH：…”这样的行），而不是Python回溯或`pathlib`输出。PROGRESS.md必须包含一个带有记录的标准输出和退出代码的`## Phase 6 Mechanical Closure`标题。这个基准测试之所以存在，是因为v1.3.23将Python`Path.read_text()`替换为`bash verify.sh`，创建了一个循环检查，尽管工件被制造出来，但仍然通过了检查。

# # # 38。存在单独的审计员报告构件如果运行了第4阶段（规范审计），请验证`quality/spec_audits/YYYY-MM-DD-auditor-N.md`上是否存在单个审计员报告文件（每个审计员一个），而不仅仅是分类综合。没有单独报告的单一分类文件将发现与核对混为一谈。该基准的存在是为了确保保留对账前的调查结果以供独立核查。

# # # 39。BUGS.md使用规范标题格式BUGS.md中的每个确认bug都必须使用标题级别`### BUG-NNN`。Grep for`^### BUG-`和count；查找其他错误标题模式（`^## BUG-`、`^\*\*BUG-`、`^- BUG-`）并验证零匹配。不一致的标题级别导致机器可读计数与文档不一致。

# # # 40。神器文件存在门通过在标记阶段5完成之前，验证所有需要的工件都以文件的形式存在于磁盘上—而不仅仅是在PROGRESS.md中引用。所需文件：EXPLORATION.md，BUGS.md,REQUIREMENTS.md,QUALITY.md,PROGRESS.md,COVERAGE_MATRIX.md,COMPLETENESS_REPORT.md,CONTRACTS.md, test_functional。*(或适合语言的替代：FunctionalSpec。*, FunctionalTest。*, functional.test。*)、RUN_CODE_REVIEW.md、RUN_INTEGRATION_TESTS.md、RUN_SPEC_AUDIT.md、RUN_TDD_TESTS.md和AGENTS.md（项目根）。如果阶段3运行：code_reviews/.中至少有一个文件；如果阶段4运行：spec_audits/.中至少有一个审计员文件和一个分流文件；如果阶段0或阶段b运行：SEED_CHECKS.md作为独立文件运行。如果已确认的bug存在：tdd-results.jsoninresults/.如果任何bug有一个红色阶段的结果：TDD_TRACEABILITY.md。这个基准测试之所以存在，是因为v1.3.24基准测试显示，向PROGRESS.md编写了一个终端闸段，声称有1个已确认的bug，但BUGS.md、代码审查文件和规范审计文件从未编写过到磁盘。# # # 41。Sidecar JSON Post-Write验证在写入`tdd-results.json`and/or`integration-results.json`之后，验证每个文件是否包含所有必需的具有一致性值的键。对于`tdd-results.json`：所需的根密钥为`schema_version`、`skill_version`、`date`、`project`、`bugs`、`summary`。每个`bugs`条目必须有`id`、`requirement`、`red_phase`、`green_phase`、`verdict`、`fix_patch_present`、`writeup_path`。`summary`必须包含“`confirmed_open`”。对于`integration-results.json`：所需的根密钥为`schema_version`、`skill_version`、`date`、`project`、`recommendation`、`groups`、`summary`、`uc_coverage`。两个都必须是`schema_version: "1.1"`。缺少必需键、非标准根键或无效enum值的sidecar JSON是不一致的。这个基准测试之所以存在，是因为v1.3.25的基准测试显示，8个版本中有6个版本使用了不一致的sidecar JSON——httpx发明了一种替代模式，serde使用了遗留形状，javalin省略了`summary`和per-bug字段，express使用了无效的阶段值，其他版本使用了invidverdict/resultenum值。# # # 42。通过脚本验证的闭包门

在标记阶段5完成之前，必须从项目根目录执行`quality_gate.sh`，并且必须退出0。脚本的完整输出必须保存到`quality/results/quality-gate.log`。没有`quality-gate.log`或日志显示FAIL结果的第5阶段完井是不合格的。这个基准测试的存在是因为v1.3.21-v1.3.25完全依赖于工件一致性检查的模型自认证，并且基准测试显示了脚本机械地捕获的持续的不一致性（标题格式、侧车模式、用例标识符、版本戳）。

# # # 43。存在规范用例标识符REQUIREMENTS.md必须包含用规范标识符标记的用例，格式为`UC-01`、`UC-02`等。搜索`UC-[0-9]`并计数匹配。带有用例内容但没有规范标识符的repo是不一致的。这个基准测试之所以存在，是因为v1.3.25基准测试显示，8个repos中有7个带有用例部分，但没有机器可读的标识符——下游工具无法计数或交叉引用没有规范格式的用例。

# # # 44。每个确认的Bug都有回归测试补丁对于每个已确认的bug （BUGS.md中的任何bug - nnn条目），验证`quality/patches/BUG-NNN-regression-test.patch`是否存在。没有回归测试补丁的已确认bug是不完整的——补丁是证明bug存在的最有力的独立证据。修复补丁（`BUG-NNN-fix.patch`）是可选的，但强烈建议用于简单修复。这个基准测试之所以存在，是因为v1.3.25和v1.3.26基准测试显示4/8repos没有补丁文件，尽管已经确认了错误，并且这些记录描述了在不生成实际补丁文件的情况下修复应该是什么样子。

# # # 45。编写内联修复差异在`quality/writeups/BUG-NNN.md`的每个写入必须包含一个` `'`diff `隔离代码块，并以统一的diff格式建议修复。这是writeup模板的第6节（“修复”）。没有内联diff的“查看补丁文件”或“不包含修复补丁”的写入是不完整的—内联diff使写入具有可操作性，对于只读写入而不访问补丁目录的维护者来说。这个基准测试之所以存在，是因为v1.3.27基准测试显示，尽管在`quality/patches/`中有修复补丁，但virtio产生了4次写入，没有内联差异。该模型对修复进行了简单的描述，而不是粘贴实际的差异。

快速检查表格式

用这个作为最后的落款：[]启发式目标附近的测试计数（规格部分+场景+防御模式）
—[]场景测试计数匹配QUALITY.md场景计数
-[]交叉变量测试~总数的30%（涵盖的每个交叉属性）
-[]边界测试≈防御模式计数
-[]大多数断言检查值，而不仅仅是存在
-[]所有测试都断言结果，而不是机制
-[]所有的突变都使用模式有效的值
-所有新测试通过（零失败和零错误-检查夹具错误）
-[]所有现有测试仍然通过
- []QUALITY.md场景参考真实代码，包含`[Req: tier — source]`标签
-[]如果使用推断需求：所有`[Req: inferred — ...]`项目被标记供用户审查
—[]代码审查协议是自包含的
[]集成测试质量门是根据字段参考表编写的（不是内存）
-[]集成测试有特定的通过标准
- []规范审计提示符是可复制粘贴的，并使用`[Req: tier — source]`标记格式
—[]结构化输出模式包括所有必选字段和有效的enum值
-[]补丁验证门为项目的构建系统使用正确的命令
[]每个回归测试都有一个skip/xfail守卫引用bug ID
[]集成组命令通过飞行前发现（试运行发现测试）
-[]每个生成的文件都有一个带有正确版本号的版本戳
-[]枚举完整性检查显示两个列表比较（不仅仅是覆盖率断言）
[]每个经过tdd验证的bug都会在`quality/writeups/BUG-NNN.md`上写出来
-[]分类验证探测包括用于确认和拒绝的测试断言（而不仅仅是散文）
[]枚举代码端列表包括每个项目的行号（不是从需求中复制的）
[]调度函数契约引用`quality/mechanical/`工件（不是手写列表）
-[]`bash quality/mechanical/verify.sh`通过（工件匹配重新提取的输出）
-执行源代码检查回归测试（字符串匹配测试不使用`run=False`）
[]没有被执行的神器在关闭时与任何散文神器相矛盾（矛盾门通过）
-[]所有生成的工件版本戳与SKILL.md元数据版本完全匹配
- []`quality/mechanical/`要么不存在（没有分派契约），要么包含verify.sh+所有提取工件
[]如果BUGS.md已经确认bug:tdd-results.jsonexists（必选）；如果任何bug有红阶段结果，则存在TDD_TRACEABILITY.md[]所有确认的分诊错误都会出现在BUGS.md（triage-to-BUGS.mdsync）中
[]每一个被确认的bug （TDD-verified或confirmed-open）都会在`quality/writeups/BUG-NNN.md`上被写出来
-[]阶段4有一个分类文件在`quality/spec_audits/YYYY-MM-DD-triage.md`[]（延续模式）`SEED_CHECKS.md`的种子检查是机械执行的，而不是从散文中推断出来的
-[]机械验证收据当`quality/mechanical/`存在时，存在Iles （`mechanical-verify.log`+`mechanical-verify.exit`）
-[]没有分诊探针读取`quality/mechanical/*.txt`作为源代码内容的唯一证据
-[]阶段6机械封闭使用`bash verify.sh`（不是Python替代）
[]个别审计员报告存在于`quality/spec_audits/*-auditor-N.md`（不只是分类）
-[]所有BUGS.mdbug标题使用`### BUG-NNN`格式
- []quality/BUGS.md存在（零bug运行包括候选评估和消除的摘要）
[]在阶段5标记完成之前，所有需要的工件文件都存在磁盘上（不只是在PROGRESS.md中引用）
-[]（延续模式）PROGRESS.md包含`## Convergence`段，包含新计数和判决
[]`quality/BUGS.md`存在（零bug运行包括候选评估和消除的摘要）
- [] Sidecar JSON文件（`tdd-results.json`,`integration-results.json`）包含`schema_version: "1.1"`所需的所有键
- []`quality_gate.sh`已执行并退出0；输出保存到`quality/results/quality-gate.log`- []REQUIREMENTS.md包含规范用例标识符（`UC-01`、`UC-02`等）
[]每个确认的bug都有`quality/patches/BUG-NNN-regression-test.patch`[]每次写入都有一个内联修复diff （` `'`diff `block在第6部分）