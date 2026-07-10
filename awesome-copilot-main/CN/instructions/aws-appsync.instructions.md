---
description: 'Production-grade guidance for AWS AppSync Event API handlers using APPSYNC_JS runtime restrictions, utilities, modules, and datasource patterns'
applyTo: '**/*.{graphql,gql,vtl,ts,js,mjs,cjs,json,yml,yaml}'
---
# AWS AppSync事件API指令

在使用`APPSYNC_JS`运行时实现AWS AppSync **Event API**处理程序（`onPublish`,`onSubscribe`）时使用这些说明。

##范围和合同

-围绕通道命名空间流设计处理程序：`onPublish`在广播之前运行，`onSubscribe`在订阅尝试时运行。
保持事件契约的明确和稳定。将通道路径和有效载荷形状视为API契约。
-更喜欢有效载荷字段的附加更改，避免破坏现有订阅者。

数据源映射（事件API）

根据事件工作流的需要有意地使用数据源：- Lambda：自定义计算，转换，编排，外部AWS/service集成。
—DynamoDB：低延迟event/state持久性和基于密钥的reads/writes.- RDS (Aurora)：关系检查、连接和更强的关系完整性用例。
EventBridge：将事件路由到更广泛的事件驱动架构中。
- OpenSearch：对事件数据进行搜索和分析。
—HTTP端点：基于HTTP的外部api或AWS服务api。
-基石：事件管道中的模型推理和AI丰富。

只有当每个跳都有明确的原因（验证、持久化、充实、路由）时，才倾向于组合多个数据源。

##数据源设置和IAM（必选）-在Event API级别创建数据源，然后将它们附加为名称空间集成。
—如果使用服务角色，只授予所需的操作（最小权限）。
—信任策略主体必须允许`appsync.amazonaws.com`承担角色。
-在可能的情况下使用条件限制信任：
-`aws:SourceAccount`到你的账户。
-`aws:SourceArn`到一个特定的AppSync API ARN（或紧密作用域模式）。
—不要重用广泛的、跨服务的IAM角色来访问AppSync数据源。

##运行时间限制（必须遵守）`APPSYNC_JS`运行时是一个受限的JavaScript子集。为这个环境编写代码，而不是为完整的Node.js编写代码。-不要使用异步模式：没有承诺，`async/await`，或后台异步工作流。
-不要使用不支持的statements/operators:`try/catch/finally`，`throw`,`while`， c型`for(;;)`，`continue`，标签，不支持的一元运算符。
不要依赖运行时代码对网络或文件系统的访问。I/O.使用AppSync数据源
-不要使用递归或传递函数作为函数参数。
不要依赖文档支持之外的类或高级运行时特性。
-当需要迭代时，首选`for-of`/`for-in`循环。

处理程序流模式-对于没有数据源集成的处理程序，直接返回转换后的`ctx.events`。
-对于具有数据源的处理程序，使用`request(ctx)`和`response(ctx)`的对象形式。
—当业务逻辑决定跳过数据源调用和响应映射时，使用`runtime.earlyReturn(...)`。
—使用`ctx.info.channel.path`、`ctx.info.channel.segments`、`ctx.info.channelNamespace.name`、`ctx.info.operation`驱动路由逻辑。
—对于数据源集成的`onPublish`，返回事件列表以从`response(ctx)`广播。
—对于数据源集成的`onSubscribe`，包含一个`response(ctx)`函数（不需要后续映射时可以为空）。`ctx.prev.result`vs`ctx.stash`（管道指南）—如果“resolver/functions”执行分步执行，且“下一步”依赖于上一步的输出，则使用“`ctx.prev.result`”。
—使用`ctx.prev.result`作为连续管道函数之间的默认数据切换机制。
-当您需要跨多个管道阶段共享数据时，使用`ctx.stash`，而不仅仅是直接的先前结果。
-在`ctx.stash`中只存储小的，有意的元数据（例如标志，id，关联上下文），而不是大的有效负载副本。
—当`ctx.prev.result`已经提供了所需的值时，不要将之前的全部结果复制到`ctx.stash`中。

##错误和授权流-不要在处理程序中使用`throw`。使用AppSync运行时支持的`util.error(...)`和`util.appendError(...)`模式。
—对于发布失败，返回带有安全消息的显式运行时错误（没有内部消息）。
-对于处理程序级别的业务级授权拒绝，请使用处理程序代码中记录的未授权实用程序。
-保持错误载荷不敏感。永远不要公开秘密、原始堆栈跟踪或内部标识符。

内置实用程序

使用`util`作为运行时安全的帮助程序。

-编码工具：
—`util.urlEncode`、`util.urlDecode`-`util.base64Encode`,`util.base64Decode`-运行时实用程序：
-`runtime.earlyReturn(obj)`停止当前处理程序执行并跳过数据源+响应评估。

##内置模块

使用来自`@aws-appsync/utils`的官方模块，并保持代码声明性。

—DynamoDB模块导入：
——`import * as ddb from '@aws-appsync/utils/dynamodb'`—RDS模块导入：
——`import { ... } from '@aws-appsync/utils/rds'`DynamoDB使用情况在可能的情况下，选择模块帮助器而不是手写请求对象。

-核心助手包括：`get`，`put`,`remove`,`update`,`query`,`scan`,`sync`。
批处理帮助程序：`batchGet`，`batchPut`,`batchDelete`。
-事务助手：`transactGet`，`transactWrite`。
-对于`update`，更喜欢像increment/append/add/remove这样的操作助手来进行安全的补丁式突变。
—对查询优先访问的键和索引进行建模。除非合理，否则避免使用`scan`。
-在需要时使用正确性和乐观并发性条件。
—对于突发发布流，首选`batchPut`/`batchDelete`（或者在需要原子性时使用`transactWrite`）而不是许多单项操作。
保持DynamoDB批大小在service/API限制和块输入确定性。

### Lambda用法

对于Event API Lambda数据源请求，使用：

——`operation: 'Invoke'`-可选`invocationType: 'RequestResponse' | 'Event'`-`payload`为Lambda合约显式成形

指导:-当处理程序流依赖于Lambda输出时，使用`RequestResponse`。
-使用`Event`仅用于即开即弃的副作用。
-在`response(ctx)`中验证`ctx.result`，并映射到确切的传出事件形状。
在事件API处理程序中，Lambda操作支持是`Invoke`；这里不要依赖graphql样式的`BatchInvoke`。
-如果你需要在Event API流中使用Lambda进行批处理，在一个`Invoke`中发送一个数组有效负载，并在Lambda中实现项目级aggregation/partial-failure处理。

直接Lambda集成（无处理程序代码）

您可以使用直接Lambda集成（`Behavior: DIRECT`）来配置名称空间处理程序，而不是编写`onPublish`/`onSubscribe`代码。—`REQUEST_RESPONSE`模式：
-`onPublish`Lambda返回`{ events?: OutgoingEvent[], error?: string }`。
-`onSubscribe`Lambda返回`null`表示成功，返回`{ error: string }`表示拒绝。
—`EVENT`模式：
-调用是异步的；AppSync不等待Lambda响应。
-对于publish，事件照常播放。
—如果Lambda以request/response模式返回`error`，则在启用日志记录时将其记录下来，而不是作为详细的内部错误负载发送回去。

当整个命名空间行为可以集中在Lambda中并且不需要APPSYNC_JSrequest/response映射逻辑时，建议直接集成Lambda。### HTTP/EventBridge/RDS/OpenSearch/Bedrock
当使用非dynamodb数据源时：

- HTTP：返回`resourcePath`，`method`，可选`params`(`headers`,`query`,`body`)；检查`ctx.result.statusCode`、`ctx.result.body`和`ctx.error`。
EventBridge：使用`operation: 'PutEvents'`并从`ctx.events`构建确定性事件条目。
- RDS：更喜欢SQL助手和`createPgStatement`/`createMySQLStatement`；不要插入不安全的SQL。
- OpenSearch：保持请求path/params明确和映射只需要的字段从`ctx.result`。
- Bedrock：明确定义`operation`(`InvokeModel`或`Converse`)，并包括及时注入保护。

批处理操作（需要指导）-首选批处理，目标数据源本机支持批处理，事件语义允许分组。
- DynamoDB:
—非原子批量操作使用`batchGet`，`batchPut`,`batchDelete`。
—当需要原子全有或全无行为时，使用`transactGet`，`transactWrite`。
-验证和限制每个请求项目的数量；大块批量。
-λ:
事件API JS处理程序请求使用`operation: 'Invoke'`和可选的`invocationType`。
-在处理程序请求对象中没有事件API`BatchInvoke`操作。
对于伪批Lambda模式，将列表有效负载发送到一个调用，并返回确定性的每项结果结构。
保持明确的订购保证：如果下游消费者依赖于订单，保存并记录订购键。

安全与数据安全—将`ctx.identity`、报头和有效载荷字段视为不可信的输入。
—对每个数据源执行最少权限IAM。
—在写操作和转发转换后的事件之前添加验证。
-永远不要在处理程序代码中硬编码秘密。
—对于公共使用，保持默认值保守（无效状态为deny/unauthorized）。

##工具、TypeScript和构建

—使用`@aws-appsync/eslint-plugin`（最少`plugin:@aws-appsync/base`）。
-配置TypeScript工具时使用`plugin:@aws-appsync/recommended`。
TypeScript不会被AppSync运行时直接执行。在部署前将其转换为支持的JavaScript。
捆绑外部化`@aws-appsync/utils`导入和源映射用于调试。

可观察性和操作-为处理程序和数据源集成启用CloudWatch日志。
-日志具有结构化，低基数字段（通道namespace/path，操作，请求id）。
-增加可报警信号：处理程序错误，数据源错误，延迟回归。
-保持响应转换的确定性，并使用多事件有效负载进行测试。

最低质量检查表

-[]只使用appsync_js支持的运行时特性。
-[]不使用`throw`，不使用async/promise，不支持loop/control结构。
-[]错误流使用运行时支持的实用程序并返回非敏感消息。
- []`onPublish`和`onSubscribe`的行为是明确的和经过测试的。
-[]数据源request/response映射是确定性的，模式安全的。
- []Lambda/DynamoDB合同被记录和验证。
—[]启用与`@aws-appsync/eslint-plugin`联动。