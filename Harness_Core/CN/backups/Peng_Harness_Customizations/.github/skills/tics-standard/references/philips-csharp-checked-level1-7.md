# Philips C# Checked Rules Levels 1-7

Source summary:

- Ruleset: Philips C# Coding Standard 5.33
- Set ID: `4T_Jr6-VSX6fp6egDIhGow`
- Filter: `Status = CHECKED`
- 建议 agent-delivered code 的默认 must-fix 范围：levels 1 through 7

按 severity 统计的 checked rule counts：

- Level 1: 9
- Level 2: 12
- Level 3: 5
- Level 4: 8
- Level 5: 5
- Level 6: 2
- Level 7: 4
- Level 8: 1
- Level 9: 7
- Level 10: 1

## Checked Rules By Severity 1-7

| 等级 | 规则 | 类别 | Synopsis |
|---|---|---|---|
| 1 | 10@406 | 数据类型 | 使用 composite formatting 时，必须提供 format string 引用的所有 objects |
| 1 | 5@121 | 对象生命周期 | 不要在 `using` statement 的 scope 外使用 "using" variables |
| 1 | 6@191 | 控制流 | 不要 dereference null |
| 1 | 7@502 | 面向对象 | 在 overloaded operator 的实现中，不要修改任何 operands 的值 |
| 1 | 7@520 | 面向对象 | 每当 override Equals method 时，也要 override GetHashCode method。 |
| 1 | 7@521 | 面向对象 | 每当实现 == operator 时，也要 override Equals method，并让二者执行相同语义 |
| 1 | 8@102 | 异常 | 不要从意外位置 throw exceptions |
| 1 | 8@110 | 异常 | 不要静默忽略 exceptions |
| 1 | 9@113 | 委托和事件 | 始终检查 event handler delegate 是否为 null |
| 2 | 10@401 | 数据类型 | Floating point values 不应使用 ==、!= operators 或 Equals method 比较。 |
| 2 | 4@101 | 注释 | 每个文件都应包含 header block |
| 2 | 5@108 | 对象生命周期 | 不要在 nested scope 中重新声明 visible name |
| 2 | 5@113 | 对象生命周期 | 如果 class 使用 unmanaged resources、拥有 disposable objects 或订阅其他 objects，则实现 IDisposable |
| 2 | 5@114 | 对象生命周期 | 不要在 finalizer 中访问任何 reference type members |
| 2 | 6@101 | 控制流 | 不要在 for loop block 中改变 loop variable |
| 2 | 6@105 | 控制流 | 确保 switch statements exhaustive |
| 2 | 7@101 | 面向对象 | 将所有 fields（data members）声明为 private |
| 2 | 7@531 | 面向对象 | 当 overload addition (+) operator 和/或 subtraction (-) operator 时，也要 overload equality operator (==) |
| 2 | 7@532 | 面向对象 | 如果实现任意 relational operator (<, <=, >, >=)，则实现全部 relational operators |
| 2 | 9@110 | 委托和事件 | 每个 subscribe 都必须有对应 unsubscribe |
| 2 | 9@114 | 委托和事件 | 不要在 events 中使用 callbacks 的 return values |
| 3 | 4@111 | 注释 | 不要注释掉代码 |
| 3 | 7@105 | 面向对象 | 在 abstract base class 上显式定义 protected constructor |
| 3 | 7@530 | 面向对象 | 实现 IComparable 时，为 equality (==)、not equal (!=)、less than (<) 和 greater than (>) operators 实现 operator overloading |
| 3 | 7@533 | 面向对象 | 不要使用 Equals method 比较不同 value types，而应使用 equality operators。 |
| 3 | 8@107 | 异常 | 使用 standard exceptions |
| 4 | 10@407 | 数据类型 | 使用 composite formatting 时，不要提供 format string 未引用的 object |
| 4 | 2@107 | 通用 | 不要在代码中 suppress compiler warnings |
| 4 | 5@111 | 对象生命周期 | 避免实现 finalizer |
| 4 | 6@119 | 控制流 | 避免 lock public type |
| 4 | 6@201 | 控制流 | method 的 cyclomatic complexity 不应超过配置的最大值。 |
| 4 | 7@106 | 面向对象 | 默认将所有 types 设为 internal |
| 4 | 7@107 | 面向对象 | 限制 source code file 内容为一个 type |
| 4 | 7@404 | 面向对象 | 不要用 new keyword 隐藏 inherited members |
| 5 | 5@119 | 对象生命周期 | 对不可变 collections 返回 interfaces |
| 5 | 6@115 | 控制流 | 不要在一个 expression 中多次访问 modified object |
| 5 | 7@102 | 面向对象 | 如果 class 只包含 static members，则防止其实例化 |
| 5 | 7@608 | 面向对象 | 使用 pattern matching，而不是 "as" keyword |
| 5 | 7@611 | 面向对象 | 适用时使用 generic constraints |
| 6 | 7@303 | 面向对象 | 如果必须提供 override method 的能力，只将最完整的 overload 设为 virtual，并用它定义其他 operations |
| 6 | 7@501 | 面向对象 | 不要在 class type 上 overload 任何 'modifying' operators |
| 7 | 10@301 | 数据类型 | 不要使用 'magic numbers' |
| 7 | 3@204 | 命名 | 不要使用容易与数字混淆的字母，反之亦然 |
| 7 | 3@504 | 命名 | source file 名称应匹配 main class |
| 7 | 7@609 | 面向对象 | 使用正确的 casting 方式 |

此文件只是 team reference。source of truth 仍然是 CSViewer，以及当前 branch 上的 TICS 或 CI rerun。
