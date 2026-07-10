# Schema Type Mapping（步骤5b）

如果项目具有模式验证层，则需要在编写边界测试之前了解每个字段接受的内容。按语言划分的常见验证层：Pydantic模型（Python）， JSON模式（any）， TypeScriptinterfaces/Zod模式（TypeScript）， Bean验证注释（Java）， case类codecs/Circe解码器（Scala）， serde属性（Rust）。如果没有这种映射，您将编写在模式到达您要测试的代码之前就被拒绝的突变，从而产生验证错误，而不是有意义的边界测试。

##为什么这很重要

想想这个常见的错误：```typescript
// TypeScript — WRONG: tests the validation mechanism, not the requirement
test('bad value rejected', () => {
    fixture.field = 'invalid';  // Zod schema rejects this before processing!
    expect(() => process(fixture)).toThrow(ZodError);
    // Tells you nothing about the output
});

// TypeScript — RIGHT: tests the requirement using a schema-valid mutation
test('bad value not in output', () => {
    fixture.field = undefined;  // Schema accepts undefined for optional fields
    const output = process(fixture);
    expect(output).not.toContain(badProperty);  // Bad data absent
    expect(output).toContain(expectedType);      // Rest still works
});
```

```python
# Python — WRONG: tests the validation mechanism, not the requirement
def test_bad_value_rejected(fixture):
    fixture.field = "invalid"  # Pydantic rejects this before processing!
    with pytest.raises(ValidationError):
        process(fixture)
    # Tells you nothing about the output

# Python — RIGHT: tests the requirement using a schema-valid mutation
def test_bad_value_not_in_output(fixture):
    fixture.field = None  # Schema accepts None for Optional fields
    output = process(fixture)
    assert field_property not in output  # Bad data absent
    assert expected_type in output  # Rest still works
```

```java
// Java — WRONG: tests Bean Validation, not the requirement
@Test
void testBadValueRejected() {
    fixture.setField("invalid");  // @NotNull/@Pattern rejects this!
    assertThrows(ConstraintViolationException.class, () -> process(fixture));
}

// Java — RIGHT: tests the requirement using a schema-valid mutation
@Test
void testBadValueNotInOutput() {
    fixture.setField(null);  // nullable String field accepts null
    var output = process(fixture);
    assertFalse(output.contains(badProperty));
    assertTrue(output.contains(expectedType));
}
```

```scala
// Scala — WRONG: tests the decoder, not the requirement
"bad value" should "be rejected" in {
    val input = fixture.copy(field = "invalid")  // Circe decoder fails!
    a [DecodingFailure] should be thrownBy process(input)
}

// Scala — RIGHT: tests the requirement using a schema-valid mutation
"missing optional field" should "not produce bad output" in {
    val input = fixture.copy(field = None)  // Option[String] accepts None
    val output = process(input)
    output should not contain badProperty
}
```

```go
// Go — WRONG: tests validation, not the requirement
func TestBadValueRejected(t *testing.T) {
    fixture.Field = "invalid"  // Struct tag validator rejects this!
    _, err := Process(fixture)
    if err == nil { t.Fatal("expected validation error") }
    // Tells you nothing about the output
}

// Go — RIGHT: tests the requirement using a valid zero value
func TestBadValueNotInOutput(t *testing.T) {
    fixture.Field = ""  // Zero value is valid for optional string fields
    output, err := Process(fixture)
    if err != nil { t.Fatalf("unexpected error: %v", err) }
    // Assert bad data absent, rest still works
}
```

```rust
// Rust — WRONG: tests serde deserialization, not the requirement
#[test]
fn test_bad_value_rejected() {
    let input = Fixture { field: "invalid".into(), ..default() };
    // serde rejects before processing!
    assert!(process(&input).is_err());
}

// Rust — RIGHT: tests the requirement using a schema-valid mutation
#[test]
fn test_bad_value_not_in_output() {
    let input = Fixture { field: None, ..default() };  // Option<String> accepts None
    let output = process(&input).expect("should succeed");
    assert!(!output.contains(bad_property));
    assert!(output.contains(expected_type));
}
```
错误测试失败并显示validation/decoding错误，因为突变值不是模式有效的。RIGHT测试使用模式接受的值（null、None、nil、零值、empty Option），因此更改会到达实际的处理逻辑。

如何构建地图

对于在步骤5中发现的每个防御模式的字段，记录：

|字段|模式类型|接受|拒绝||-------|-----------|---------|---------|
|`metadata`|可选对象（`Optional[MetadataObject]`/`MetadataObject?`/`MetadataObject \| null`） |有效对象，`null`/`undefined`|`string`,`number`,`array`|
|`count_field`|可选整数（`Optional[int]`/`number?`/`Integer`） |整数，`null`|`string`,`object`|
|`child_list`|对象数组（`List[Child]`/`Child[]`/`Seq[Child]`） |对象数组，`[]`|`[null, "invalid"]`,`null`|
|`optional_object`|可选对象|`{"key": value}`，`null`|`"bad"`,`[1,2]`|

选择突变值的规则

在编写边界测试时，始终使用“接受”列中的值。习惯用法“missing/empty”值因语言而异：**Optional/nullable字段：** Python`None`， Java`null`, Scala`None`（适用于`Option`）， TypeScript`undefined`/`null`， Go零值（`""`,`0`，指针`nil`）， Rust`None`（适用于`Option<T>`）
- **数字字段：**`0`，负值或边界值-语言无关
**Arrays/lists:** Python`[]`, Java`List.of()`, Scala`Seq.empty`, TypeScript`[]`， Go`nil`或空切片，Rust`Vec::new()`- **字符串：**`""`（空字符串）-语言无关
**Objects/structs:** Python`{}`, Java`new Obj()`, Scala`copy()`, TypeScript`{}`， Go零值结构，Rust`Default::default()`或builder缺少字段

永远不要使用“rejected”列中的值——它们测试的是模式验证器，而不是业务逻辑。

何时跳过此步骤如果项目没有模式验证层（数据流直接进入处理而不进行类型检查），则可以跳过映射并使用任何突变值。但是大多数现代项目都有某种形式的验证，所以先检查一下。