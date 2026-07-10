---
description: 'Instructions for developing Power Platform custom connectors with Model Context Protocol (MCP) integration for Microsoft Copilot Studio'
applyTo: '**/*.{json,csx,md}'
---
#电源平台MCP定制连接器开发

# #指令

MCP协议集成
-始终执行JSON-RPC 2.0标准的MCP通信
-使用`x-ms-agentic-protocol: mcp-streamable-1.0`头为副驾驶工作室兼容性
-构建端点以支持标准REST操作和MCP工具调用
-转换响应以符合Copilot Studio的约束（没有引用类型，只有单一类型）

模式设计最佳实践
-从JSON模式中删除`$ref`和其他引用类型，因为Copilot Studio无法处理它们
—在模式定义中使用单个类型而不是类型数组
- Flatten`anyOf`/`oneOf`构造为单一模式的副驾驶工作室兼容性
—确保所有工具输入模式都是自包含的，没有外部引用身份验证和安全
-在电源平台的约束下实施OAuth 2.0和MCP安全最佳实践
—使用连接参数集进行灵活的认证配置
-验证令牌受众，防止直通攻击
-添加mcp特定的安全头以增强验证
-支持多种认证方法（OAuth标准，OAuth增强，API密钥回退）

自定义脚本实现
-在自定义脚本（script.csx）中处理JSON-RPC转换
-使用JSON-RPC错误响应格式实现正确的错误处理
-在认证流程中添加令牌验证和受众检查
-转换MCP服务器响应的Copilot Studio兼容性
—使用连接参数进行动态安全配置Swagger定义指南
-使用Swagger 2.0规范以兼容电源平台
-为每个端点实现适当的`operationId`值
—定义清晰的参数模式，包含适当的类型和描述
—为所有成功和错误案例添加综合响应模式
-包括适当的HTTP状态码和响应头

资源和工具管理
-结构MCP资源是消耗性的工具输出在副驾驶工作室
确保资源内容有适当的MIME类型声明
-为更好的Copilot Studio集成添加观众和优先级注释
-实施资源转换，以满足副驾驶工作室的要求###连接参数配置
—使用enum下拉菜单选择OAuth版本和安全级别
—提供清晰的参数说明和约束条件
—支持多种认证参数集，适用于不同的部署场景
-在适当的地方包括验证规则和默认值
—通过连接参数值进行动态配置

错误处理和日志记录
—按照JSON-RPC 2.0错误格式实现全面的错误响应
-为身份验证、验证和转换步骤添加详细的日志记录
—提供清晰的错误信息，帮助排除故障
-包括正确的HTTP状态码与错误条件对齐测试和验证
-测试连接器与实际的MCP服务器实现
-验证模式转换与Copilot Studio正确工作
—验证所有支持的参数集的认证流
—针对不同的故障场景，确保正确的错误处理
-测试连接参数配置和动态行为

##附加指南

电源平台认证要求
-包括全面的文档（readme.md,CUSTOMIZE.md）
-提供清晰的设置和配置说明
-记录所有认证选项和安全考虑事项
-包括适当的出版商和堆栈所有者信息
—确保符合Power Platform连接器认证标准MCP服务器兼容性
-设计兼容标准的MCP服务器实现
-支持常用的MCP方法，如`tools/list`，`tools/call`,`resources/list`-处理流响应适当的`mcp-streamable-1.0`协议
—进行适当的协议协商和能力检测

Copilot Studio Integration
确保工具定义在Copilot Studio的约束下正确工作
-测试资源访问和工具调用从Copilot Studio接口
-验证转换后的模式在对话中产生预期的行为
-确认与Copilot Studio代理框架的适当整合