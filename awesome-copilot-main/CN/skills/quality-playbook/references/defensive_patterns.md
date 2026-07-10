#发现防御模式（第5步）

防御性代码模式是过去失败或已知风险的证据。每个空保护、try/catch、规范化函数和哨点检查都存在，因为出错了—或者因为有人预期会出错。您的工作是系统地找到这些模式，并将它们转换为适合目的的场景和边界测试。

##系统搜索

不要略读——要有条不紊地搜索代码库。确切的模式取决于项目的语言。以下是常见的防御代码指标，根据它们所保护的对象进行分组：

* *Null/nil警卫:* *

|语言| Grep模式||---|---|
| Python |`None`,`is None`,`is not None`|
| Java |`null`,`Optional`,`Objects.requireNonNull`|
| Scala |`Option`,`None`,`.getOrElse`,`.isEmpty`|
|`undefined`,`null`,`??`,`?.`|
b|`== nil`,`!= nil`,`if err != nil`|
| Rust |`Option`,`unwrap`,`.is_none()`,`?`|

* *Exception/error处理:* *

|语言| Grep模式||---|---|
| Python |`except`,`try:`,`raise`|
| Java |`catch`,`throws`,`try {`|
| Scala |`Try`,`catch`,`recover`,`Failure`|
|`catch`,`throw`,`.catch(`|
b|`if err != nil`,`errors.New`,`fmt.Errorf`|
| Rust |`Result`,`Err(`,`unwrap_or`,`match`|

**Internal/private助手（通常是防御性的）：**

|语言| Grep模式||---|---|
| Python |`def _`,`__`|
|Java/Scala|`private`,`protected`|
| TypeScript |`private`,`#`(私有字段
|去|小写的函数名（未导出）|
| Rust |`pub(crate)`，非`pub`函数|

**搜索`== 0`，`< 0`,`default`,`fallback`,`else`,`match`，`switch`-这些是语言无关的。

##除了Grep还有什么- Git历史记录，TODO注释，变通方法，检查“不应该发生的事情”的防御代码
**设计决策**解释“为什么”而不仅仅是“做什么”的注释。可以硬编码但没有的配置。抽象的存在是有原因的。
- **外部数据怪癖** -代码规范化，验证或拒绝来自外部系统的输入的任何地方
-解析功能** -每个解析器（regex，字符串分割，格式检测）都有失败模式。格式错误的输入会发生什么？空输入?意想不到的类型?
**边界条件** -零值，空字符串，最大范围，first/last元素，类型边界

##将发现转换为场景

对于每一种防御模式，都要问：“这能防止什么失败？”什么输入会触发这个代码路径？”

答案变成了一个适合目的的场景：```markdown
### Scenario N: [Memorable Name]

**Requirement tag:** [Req: inferred — from function_name() behavior] *(use the canonical `[Req: tier — source]` format from SKILL.md Phase 1, Step 1)*

**What happened:** [The failure mode this code prevents. Reference the actual function, file, and line. Frame as a vulnerability analysis, not a fabricated incident.]

**The requirement:** [What the code must do to prevent this failure.]

**How to verify:** [A concrete test that would fail if this regressed.]
```
将结果转换为边界测试

每个防御模式也映射到一个边界测试：```python
# Python (pytest)
def test_defensive_pattern_name(fixture):
    """[Req: inferred — from function_name() guard] guards against X."""
    # Mutate fixture to trigger the defensive code path
    # Assert the system handles it gracefully
```

```java
// Java (JUnit 5)
@Test
@DisplayName("[Req: inferred — from methodName() guard] guards against X")
void testDefensivePatternName() {
    fixture.setField(null);  // Trigger defensive code path
    var result = process(fixture);
    assertNotNull(result);  // Assert graceful handling
}
```

```scala
// Scala (ScalaTest)
// [Req: inferred — from methodName() guard]
"defensive pattern: methodName()" should "guard against X" in {
  val input = fixture.copy(field = None)  // Trigger defensive code path
  val result = process(input)
  result should equal (defined)  // Assert graceful handling
}
```

```typescript
// TypeScript (Jest)
test('[Req: inferred — from functionName() guard] guards against X', () => {
    const input = { ...fixture, field: null };  // Trigger defensive code path
    const result = process(input);
    expect(result).toBeDefined();  // Assert graceful handling
});
```

```go
// Go (testing)
func TestDefensivePatternName(t *testing.T) {
    // [Req: inferred — from FunctionName() guard] guards against X
    t.Helper()
    fixture.Field = nil  // Trigger defensive code path
    result, err := Process(fixture)
    if err != nil {
        t.Fatalf("expected graceful handling, got error: %v", err)
    }
    // Assert the system handled it
}
```

```rust
// Rust (cargo test)
#[test]
fn test_defensive_pattern_name() {
    // [Req: inferred — from function_name() guard] guards against X
    let input = Fixture { field: None, ..default_fixture() };
    let result = process(&input);
    assert!(result.is_ok(), "expected graceful handling");
}
```
状态机模式

状态机是一种特殊的防御模式。当您找到状态字段、生命周期阶段或模式标志时，请跟踪整个状态机—请参阅SKILL.md步骤5a了解完整的过程。

如何找到状态机：**

|语言| Grep模式||---|---|
| Python |`status`,`state`,`phase`,`mode`,`== "running"`,`== "pending"`|
| Java |`enum.*Status`,`enum.*State`,`.getStatus()`,`switch.*status`|
| Scala |`sealed trait.*State`,`case object`,`status match`|
|`status:`,`state:`,`Status =`,`switch.*status`|
|`Status`,`State`,`type.*Phase`,`switch.*status`|
| Rust |`enum.*State`,`enum.*Status`,`match.*state`|

对于找到的每个状态机：**

1. 列出每个可能的状态值（读取enum或grep以获得赋值）
2. 对于每个检查状态的handler/consumer，验证它处理所有状态
3. 寻找你可以进入但永远不能离开的状态（没有清理的终端状态）
4. 查找应该在某个状态中可用但被不完全保护阻止的操作

枚举和白名单完整性当函数使用`switch`/`case`、`match`、if-else链或任何分派结构来处理一组命名常量（特征位、枚举值、命令代码、事件类型、权限标志）时，执行**双表枚举检查**：

1. **List A（已定义）：**从相关的头、枚举或规范中提取代码应该处理的每个常量。使用grep -不从内存中列出。
2. **列表B（已处理）：**从调度代码中提取每个case标签，分支条件或映射键。使用grep或逐行读取-不要总结。
3. **差异：**比较两个列表。A中而不是B中的任何常数都是势能差。任何在B中但不在A中的常数都是一个潜在的死例子。**为什么存在：** AI模型可靠地幻觉了switch/case结构的完备性。模型看到一个带有许多case标签的函数，看到在其他地方定义的常量，并在没有实际检查的情况下得出所有常量都被处理的结论。在一个观察到的案例中，该模型断言内核特征位白名单“保留了支持的环传输位，包括VIRTIO_F_RING_RESET”，而该常数在开关中完全不存在——模型产生了覆盖幻觉，因为该常数存在于函数调用者使用的标头中。机械双表检查是唯一可靠的对策。**分类验证探针必须产生可执行的证据。**当triage通过验证探针确认或拒绝枚举发现时，仅靠散文推理是不够的。探测器必须为每个常数生成一个测试断言：`assert "case VIRTIO_F_RING_RESET:" in source_of("vring_transport_features"), "RING_RESET at line NNN"`。这条规则之所以存在，是因为在v1.3.16中，分诊系统正确地收到了关于RING_RESET的少数发现，但却以“行3527-3528显式保留RING_RESET”的幻觉声明拒绝了它——这些行实际上是`default:`分支。如果分诊组被迫写一个断言，它就会失败，从而暴露出幻觉。代码端列表必须从代码中提取，而不是从需求中复制。**当在代码审查或规范审计中执行双列表检查时，“处理过的”列表必须直接从函数体中提取每个项目的行号。不要从REQUIREMENTS.md、CONTRACTS.md、审计提示符或任何其他生成的构件中复制。如果这两个列表（代码提取的和需求声明的）一字不差，这是一个危险信号，说明代码列表被复制了——重做提取。在v1.3.17中，代码评审的“case labels present”列表与需求列表相同，证明它是复制的而不是提取的。三个规格审核员随后继承了这个错误的列表，没有一个独立验证。按项目行号引用可以防止这种情况：当行3527实际上包含`default:`时，您不能引用“行3527:`case VIRTIO_F_RING_RESET:`”。**机械验证工件高于散文列表。**如果调度函数存在`quality/mechanical/<function>_cases.txt`，则使用它作为函数处理的权威源。不要用手写的清单代替。如果不存在机械工件，在编写关于功能覆盖的契约或需求之前，使用非交互式shell管道（例如，`awk`+`grep`）生成一个。

**工件完整性风险：**在v1.3.19测试中，模型执行了正确的提取命令，但将自己制作的输出写入文件，而不是让shell重定向捕获它。这个虚构的文件包括一个虚构的`case VIRTIO_F_RING_RESET:`行，真实的命令不会产生这个行。为了缓解这种情况：`quality/mechanical/verify.sh`重新运行每个提取命令并对保存的文件进行diffs。如果任何diff非空，则工件已被篡改，必须重新生成。**适用于：**功能位协商函数，协议消息调度程序，权限检查开关，配置选项处理程序，codec/format注册表，HTTPmethod/status代码处理程序，以及`default:`或`else`子句静默丢弃不可识别值的任何函数。

**将状态机间隙转换为场景：**```markdown
### Scenario N: [Status] blocks [operation]

**Requirement tag:** [Req: inferred — from handler() status guard]

**What happened:** The [handler] only allows [operation] when status is "[allowed_states]", but the system can enter "[missing_state]" status (e.g., due to [condition]). When this happens, the user cannot [operation] and has no workaround through the interface.

**The requirement:** [operation] must be available in all states where the user would reasonably need it, including [missing_state].

**How to verify:** Set up a [entity] in "[missing_state]" status. Attempt [operation]. Assert it succeeds or provides a clear error with a workaround.
```
缺少保护模式

搜索那些将用户提交给昂贵的、不可逆的或长时间运行的工作而没有充分预览或确认的操作：

|模式|寻找什么||---|---|
|预提交信息缺口|启动批处理作业、扇形展开或API调用而不显示估计成本、范围或持续时间的操作|
|无声扩展|扇形输出或乘法步骤，直到运行时才知道最终的工作计数，不显示任何警告|
|无终止条件|轮询循环、监视程序或守护进程检查新工作，但从不检查是否完成了所有工作|
|重试不回退|立即重试或在固定间隔内重试而没有指数回退的错误处理，有可能导致速率限制溢出|

**将缺少的保障转换为场景：**```markdown
### Scenario N: No [safeguard] before [operation]

**Requirement tag:** [Req: inferred — from init_run()/start_watch() behavior]

**What happened:** [Operation] commits the user to [consequence] without showing [missing information]. In practice, a [example] fanned out from [small number] to [large number] units with no warning, resulting in [cost/time consequence].

**The requirement:** Before committing to [operation], display [safeguard] showing [what the user needs to see].

**How to verify:** Initiate [operation] and assert that [safeguard information] is displayed before the point of no return.
```
最低Bar

您应该在核心逻辑模块中找到每个源文件至少2-3个防御模式。如果发现的更少，请更仔细地阅读函数体——而不仅仅是签名和注释。

对于一个中等规模的项目（5-15个源文件），预计总共可以找到15-30个防御模式。每一个都应该产生至少一个边界测试。此外，如果项目具有status/state字段，则至少跟踪一个状态机，并检查至少一个长时间运行的操作是否缺少安全措施。