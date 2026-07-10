---
name: csharp-docs
description: 'Ensure that C# types are documented with XML comments and follow best practices for documentation.'
---
# c#文档最佳实践

公众成员应该用XML注释记录。
-鼓励记录内部成员，特别是如果他们是复杂的或不自我解释的。

所有api的指南-使用`<summary>`提供一个简短的一句话，描述类型或成员的功能。用第三人称现在时的动词开始总结。
-使用`<remarks>`获取其他信息，这些信息可以包括实现细节、使用说明或任何其他相关上下文。
-使用`<see langword>`语言特定的关键字，如`null`，`true`,`false`,`int`，`bool`等。
-使用`<c>`内联代码片段。
—使用`<example>`作为成员的使用示例。
-代码块使用`<code>`。`<code>`标签应该放在`<example>`标签中。使用`language`属性添加代码示例的语言，例如`<code language="csharp">`。
—使用`<see cref>`内联引用其他类型或成员（在句子中）。
-使用`<seealso>`独立（不是在一个句子中）引用其他类型或成员在“See also”部分的在线文档。
—使用`<inheritdoc/>`来继承文档从基类或接口调用。
-除非有重大的行为改变，在这种情况下，你应该记录差异。# #的方法—使用“`<param>`”描述方法参数。
-描述应该是不指定数据类型的名词短语。
-从一篇介绍性文章开始。
—如果参数为标志enum，描述信息以“指定…的枚举值的按位组合”开头。
—如果参数为非标志enum，则以“指定…的枚举值之一”开头。
—如果参数是布尔值，则措辞应该是“`<see langword="true" />`to…；”否则,`<see langword="false" />`。”。
—如果参数为“out”，则格式为“When this method returns, contains ....”此参数被视为未初始化。
—使用“`<paramref>`”引用文档中的参数名称。
—使用`<typeparam>`描述泛型类型或方法中的类型参数。
—使用“`<typeparamref>`”引用文档中的类型参数。
—使用`<returns>`来描述方法返回的内容。
-描述应该是不指定数据类型的名词短语。
-从一篇介绍性文章开始。
-如果返回类型是布尔值，措辞应该是“`<see langword="true" />`If…；”否则,`<see langword="false" />`。”。# #构造函数

-摘要措辞应该是“初始化<Class>类[或结构]的新实例。”

# #属性

—“`<summary>`”的开头应该是：
-“获取或设置…”用于读写属性。
-“获取…”为只读属性。
-对于返回布尔值的属性，“获取[或设置]一个值，该值指示是否…”
—使用`<value>`来描述属性的值。
-描述应该是不指定数据类型的名词短语。
—如果该属性有默认值，则单独添加，例如：“默认值为`<see langword="false" />`”。
—如果值类型是布尔型，措辞应该是“`<see langword="true" />`If…；”否则,`<see langword="false" />`。默认值是…”。

# #例外-使用`<exception cref>`记录由构造函数、属性、索引器、方法、操作符和事件抛出的异常。
-记录由成员直接抛出的所有异常。
对于嵌套成员抛出的异常，只记录用户最有可能遇到的异常。
异常的描述描述了它被抛出的条件。
省略句首的“抛出if…”或“if…”。只需直接声明条件，例如“访问消息队列API时发生错误”。