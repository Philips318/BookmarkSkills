---
description: 'Rust programming language coding conventions and best practices'
applyTo: '**/*.rs'
---
Rust编码约定和最佳实践

在编写Rust代码时遵循惯用的Rust实践和社区标准。

这些指令基于[The Rust Book](https://doc.rust-lang.org/book/), [Rust API Guidelines](https://rust-lang.github.io/api-guidelines/)， [RFC 430命名约定](https://github.com/rust-lang/rfcs/blob/master/text/0430-finalizing-naming-conventions.md)，以及更广泛的Rust社区[users.rust-lang.org]（https://users.rust-lang.org）。

##一般使用说明-始终优先考虑可读性、安全性和可维护性。
-使用强类型并利用Rust的所有权系统来保证内存安全。
-将复杂的功能分解成更小、更易于管理的功能。
-对于与算法相关的代码，包括对所使用方法的解释。
编写具有良好可维护性的代码，包括对做出某些设计决策的原因的注释。
-使用`Result<T, E>`优雅地处理错误，并提供有意义的错误消息。
-对于外部依赖，在文档中提及它们的用法和目的。
—使用符合[RFC 430]（https://github.com/rust-lang/rfcs/blob/master/text/0430-finalizing-naming-conventions.md）的一致命名约定。
编写遵循借用检查器规则的习惯、安全、高效的Rust代码。
-确保代码编译没有警告。

##要遵循的模式—使用模块（`mod`）和公共接口（`pub`）封装逻辑。
—使用`?`、`match`或`if let`正确处理错误。
—使用`serde`进行序列化，使用`thiserror`或`anyhow`进行自定义错误。
-实现特征来抽象服务或外部依赖。
-结构异步代码使用`async/await`和`tokio`或`async-std`。
-为了类型安全，首选枚举而不是标志和状态。
-使用构建器创建复杂的对象。
-分割二进制和库代码（`main.rs`vs`lib.rs`）的可测试性和重用性。
—对于数据并行性和cpu密集型任务，使用`rayon`。
-使用迭代器代替基于索引的循环，因为它们通常更快更安全。
-当不需要所有权时，使用`&str`代替`String`作为函数参数。
-首选借用和零拷贝操作，避免不必要的分配。

所有权、借用和寿命-首选借用（`&T`）克隆，除非所有权转移是必要的。
—当需要修改借用数据时，使用`&mut T`。
-当编译器无法推断生存期时显式注释它们。
使用`Rc<T>`进行单线程引用计数，使用`Arc<T>`进行线程安全引用计数。
-在单线程上下文中使用`RefCell<T>`实现内部可变性，在多线程上下文中使用`Mutex<T>`或`RwLock<T>`。

要避免的模式-除非绝对必要，否则不要使用`unwrap()`或`expect()`-首选正确的错误处理。
-避免在库代码中出现恐慌，而是返回`Result`。
不要依赖全局可变状态，使用依赖注入或线程安全容器。
避免使用函数或组合子进行深度嵌套逻辑重构。
不要忽视警告——在CI期间将其视为错误。
-避免使用`unsafe`，除非有必要并有充分的文件证明。
-不要过度使用`clone()`，除非需要所有权转移，否则使用借用而不是克隆。
-避免过早的`collect()`，让迭代器保持惰性，直到你真正需要收集。
-避免不必要的分配-首选借用和零拷贝操作。

代码样式和格式-遵循Rust风格指南，使用`rustfmt`自动格式化。
-尽可能将行数控制在100个字符以下。
-使用`///`将函数和结构文件放在项目前面。
-使用`cargo clippy`来捕获常见错误并执行最佳实践。

##错误处理

—对于可恢复的错误使用`Result<T, E>`，对于不可恢复的错误使用`panic!`。
-对于错误传播，`?`操作符优于`unwrap()`或`expect()`。
—使用`thiserror`创建自定义错误类型或实现`std::error::Error`。
—对于可能存在也可能不存在的值，使用`Option<T>`。
-提供有意义的错误消息和上下文。
错误类型应该是有意义的和行为良好的（实现标准特征）。
-验证函数参数并对无效输入返回适当的错误。

API设计指南###通用特性实现
在适当的地方热切地实现共同的特征：
-`Copy`,`Clone`,`Eq`,`PartialEq`,`Ord`,`PartialOrd`,`Hash`,`Debug`,`Display`,`Default`-使用标准转换特性：`From`，`AsRef`,`AsMut`-集合应该实现`FromIterator`和`Extend`-注意：`Send`和`Sync`在安全的情况下由编译器自动实现；避免手工实现，除非使用`unsafe`代码

类型安全性和可预测性
-使用newtypes提供静态区别
-参数应该通过类型传达意义；选择特定类型而不是通用的`bool`参数
-适当地使用`Option<T>`作为真正可选的值
-具有明确接收者的函数应该是方法
-只有智能指针应该实现`Deref`和`DerefMut`未来的证明
-使用密封特性来防止下游实现
-结构应该有私有字段
-函数应该验证它们的参数
—所有public类型必须使用`Debug`##测试和文档-使用`#[cfg(test)]`模块和`#[test]`注释编写全面的单元测试。
-使用测试模块和他们测试的代码（`mod tests { ... }`）。
-在`tests/`目录中编写集成测试，并使用描述性文件名。
-为每个函数，结构体，枚举和复杂逻辑编写清晰简洁的注释。
-确保功能有描述性的名称，并包括全面的文档。
-按照[API指南]（https://rust-lang.github.io/api-guidelines/）用rustdoc （`///`注释）记录所有公共API。
-使用`#[doc(hidden)]`隐藏公共文档的实现细节。
—记录错误情况、紧急情况和安全注意事项。
—示例应该使用`?`操作符，而不是`unwrap()`或已弃用的`try!`宏。

##项目组织-在`Cargo.toml`中使用语义版本控制。
-包括全面的元数据：`description`，`license`,`repository`,`keywords`,`categories`。
-对可选功能使用特性标志。
-使用`mod.rs`或命名文件将代码组织成模块。
—保持`main.rs`或`lib.rs`最小-移动逻辑到模块。

质量检查表

在发布或审查Rust代码之前，请确保：

核心需求
—[]**命名**：遵循RFC 430命名约定
-[] **特性**：在适当的地方实现`Debug`，`Clone`,`PartialEq`—[]**错误处理**：使用`Result<T, E>`，提供有意义的错误类型
-[] **文档**：所有公共项都有带有示例的rustdoc注释
-[] **测试**：全面的测试覆盖，包括边缘情况安全与质量
-[] **安全**：没有不必要的`unsafe`代码，正确的错误处理
-[] **性能**：有效使用迭代器，最小的分配
- [] **API设计：函数是可预测的，灵活的，类型安全的
-[] **未来证明**：结构中的私有字段，适当的密封特征
—[]**工具**：代码传递`cargo fmt`、`cargo clippy`和`cargo test`