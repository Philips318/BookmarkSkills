---
name: java-docs
description: 'Ensure that Java types are documented with Javadoc comments and follow best practices for documentation.'
---
# Java文档（Javadoc）最佳实践-公共成员和受保护成员应记录Javadoc注释。
-鼓励记录包私有和私有成员，特别是如果他们是复杂的或不自我解释的。
—Javadoc注释的第一句话是概要描述。它应该是对该方法所做的事情的简明概述，并以句号结束。
—方法参数使用“`@param`”。说明以小写字母开头，不能以句号结束。
—使用`@return`作为方法返回值。
-使用`@throws`或`@exception`记录方法抛出的异常。
—对其他类型或成员的引用使用`@see`。
-使用`{@inheritDoc}`从基类或接口继承文档。
-除非有重大的行为改变，在这种情况下，你应该记录差异。
—泛型类型或方法中的类型参数使用`@param <T>`。
-使用`{@code}`内联代码剪辑宠物。
—代码块使用`<pre>{@code ... }</pre>`。
-使用`@since`表示引入该特性的时间（例如，版本号）。
—使用`@version`指定成员的版本号。
—使用`@author`指定代码的作者。
-使用`@deprecated`将成员标记为已弃用并提供替代。