#编码约定

##核心部分（必选）

1)命名规则

|项目|规则|示例|证据||------|------|---------|----------|
|文件| [RULE] | [EXAMPLE] | [FILE] |
|Functions/methods| [RULE] |[示例]| [FILE] |
|Types/interfaces|[规则]|[示例]|[文件]|
|Constants/envvars | [RULE] | [EXAMPLE] | [FILE] |

### 2)格式和线条

-格式化器：[工具+配置文件]
- Linter: [TOOL + CONFIG FILE]
-最相关的强制规则：[RULE_1]， [RULE_2], [RULE_3]
—运行命令：[commands]

导入和模块约定

-导入grouping/order: [RULE]
-别名与相对进口策略：[规则]
-公共exports/barrel政策：[规则]

错误和日志约定

-分层错误策略：[简短总结]
-日志样式和所需的上下文字段：
敏感数据编校规则：[摘要]

5)测试约定

-测试文件naming/location规则：[rule]
嘲讽策略规范：[规则]
-覆盖率预期：[RULE或TODO]

6)证据- [path/to/lint-config]
- [path/to/format-config]
- [path/to/representative-source-file]
##扩展节（可选）

仅为大型或不一致的代码库添加：

-特定层的错误处理矩阵
-特定于语言的严格性选项
-特定于repo的commit/branching约定
-已知的违反公约的行为需要清理