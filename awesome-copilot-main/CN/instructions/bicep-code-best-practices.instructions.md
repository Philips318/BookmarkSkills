---
description: 'Infrastructure as Code with Bicep'
applyTo: '**/*.bicep'
---
命名约定

当编写Bicep代码时，对所有名称（变量，参数，资源）使用lowerCamelCase
-使用资源类型描述性符号名（例如，‘storageAccount’而不是‘storageAccountName’）
-避免在符号名称中使用‘name’，因为它代表资源，而不是资源的名称
—避免使用后缀来区分变量和参数

结构和声明

-总是在文件的顶部用@description装饰符声明参数
-所有资源使用最新稳定的API版本
-对所有参数使用描述性的@description装饰器
—指定命名参数的最小和最大字符长度

# #参数-设置对测试环境安全的默认值（使用低成本定价层）
-谨慎使用@allowed装饰器，以避免阻塞有效的部署
—在部署之间更改的设置使用参数

# #变量

—变量自动从解析值推断类型
—使用变量来包含复杂的表达式，而不是将它们直接嵌入到资源属性中

##资源参考

—对资源引用使用符号名，而不是reference（）或resourceId（）函数
-通过符号名（resourceA.id）创建资源依赖，而不是显式的dependsOn
对于从其他资源访问属性，使用‘existing’关键字，而不是通过输出传递值

##资源名称—使用模板表达式uniqueString（）创建有意义且唯一的资源名
-为uniqueString（）结果添加前缀，因为有些资源不允许名称以数字开头

##儿童资源

—避免过多的子资源嵌套
-使用父属性或嵌套，而不是为子资源构造资源名称

# #安全

-不要在输出中包含秘密或密钥
-直接在输出中使用资源属性（例如，storageAccount.properties.primaryEndpoints）

# #文档

-包括有用的//注释在你的二头肌文件，以提高可读性