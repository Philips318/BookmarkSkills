---
description: 'ColdFusion Coding Standards for CFC component and application patterns'
applyTo: "**/*.cfc"
---
# ColdFusion编码标准的CFC文件

-使用CFScript在可能的地方更干净的语法。
—避免使用不推荐的标签和函数。
—遵循一致的变量和组件命名约定。
—使用“`cfqueryparam`”防止SQL注入。
使用##转义<cfoutput>块内的CSS散列符号

#其他最佳实践-适当时使用`this`作用域用于组件属性和方法。
—记录所有函数的用途、参数和返回值（使用Javadoc或类似的风格）。
—函数和变量使用访问修饰符（`public`、`private`、`package`、`remote`）。
-更倾向于依赖注入组件协作。
-避免业务逻辑在setters/getters；保持简单。
-验证并清理public/remote方法中的所有输入参数。
-根据需要使用`cftry`/`cfcatch`来处理方法中的错误。
-避免在CFCs中硬编码配置或凭证。
-使用一致的缩进（2个空格，根据全球标准）。
-在组件内对相关方法进行逻辑分组。
对方法和属性使用有意义的描述性名称。
—避免使用不推荐或不需要的`cfcomponent`属性。

-尽可能使用三元操作符
-确保标签对齐一致。