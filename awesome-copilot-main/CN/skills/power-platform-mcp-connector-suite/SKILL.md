---
name: power-platform-mcp-connector-suite
description: 'Generate complete Power Platform custom connector with MCP integration for Copilot Studio - includes schema generation, troubleshooting, and validation'
---
# Power Platform MCP连接器套件

生成全面的电源平台自定义连接器实现与模型上下文协议集成为Microsoft Copilot Studio。

MCP功能在副驾驶工作室

* *目前支持:* *
-✅**Tools**: LLM可以调用的功能（需要用户批准）
-✅**资源**：代理可以读取的类文件数据（必须是工具输出）

**暂不支持：**
-❌**提示**：预写模板（为将来支持做准备）

##连接器生成

创建完整的电源平台连接器：

核心文件:* * * *
—`apiDefinition.swagger.json`与`x-ms-agentic-protocol: mcp-streamable-1.0`-`apiProperties.json`与连接器元数据和身份验证`script.csx`与自定义c#转换MCP JSON-RPC处理
-`readme.md`与连接器文档* * MCP集成:* *
- POST`/mcp`端点用于JSON-RPC 2.0通信
- McpResponse和McpErrorResponse模式定义
- Copilot Studio约束遵从性（无引用类型，单一类型）
-资源集成作为工具输出（支持资源和工具；尚未支持提示）

模式验证和故障排除

**验证模式以符合Copilot Studio:**
-✅在inputs/outputs工具中没有引用类型（`$ref`）
-✅只有单一类型的值（不是`["string", "number"]`）
-✅基本类型：字符串，数字，整数，布尔值，数组，对象
-✅资源作为工具输出，而不是单独的实体
-✅所有端点的完整uri

**常见问题和修复：**
-工具过滤→删除引用类型，使用原语
-类型错误→带有验证逻辑的单一类型
—资源不可用→纳入工具输出
—连接失败→验证`x-ms-agentic-protocol`报头

##上下文变量- **连接器名称**:[连接器的显示名称]
- **服务器用途**:[MCP服务器应该完成的功能]
- **所需工具**:[要实现的MCP工具列表]
- **资源**:[提供的资源类型]
- **认证**:[none， api-key, oauth2, basic]
- **主机环境**:[Azure功能，Express.js等]
- **目标api **:[要集成的外部api]

##生成模式

模式1：完成新建连接器
从头开始为新的Power Platform MCP连接器生成所有文件，包括CLI验证设置。

模式2：模式验证
使用paconn和验证工具分析和修复Copilot Studio合规性的现有模式。

模式3：集成故障排除
使用CLI调试工具诊断和解决Copilot Studio的MCP集成问题。模式4：混合连接器
通过适当的验证工作流程将MCP功能添加到现有的Power Platform连接器中。

模式5：认证准备
为具有完整元数据和验证遵从性的Microsoft认证提交准备连接器。

模式6:OAuth安全加固
通过MCP安全最佳实践和高级令牌验证实现增强的OAuth 2.0身份验证。

##预期输出

* * 1。apiDefinition.swagger.json* *
- Swagger 2.0格式与微软扩展
- MCP端点：`POST /mcp`与适当的协议头
兼容的模式定义（仅基本类型）
-McpResponse/McpErrorResponse定义

* * 2。apiProperties.json* *
-连接器元数据和品牌（需要`iconBrandColor`）
—认证配置
- MCP转换的策略模板* * 3。script.csx * *
- JSON-RPC 2.0消息处理
-Request/response变换
—MCP协议遵从逻辑
-错误处理和验证

* * 4。实现指导* *
-工具注册和执行模式
-资源管理策略
- Copilot Studio集成步骤
-测试和验证程序

验证检查表

技术合规性
- [] MCP endpoint中的`x-ms-agentic-protocol: mcp-streamable-1.0`-[]模式定义中没有引用类型
-[]所有类型字段都是单一类型（不是数组）
-[]作为工具输出的资源
- [] script.csx中的JSON-RPC 2.0兼容性
-[]完整的URI端点
-[]澄清副驾驶工作室代理的描述
-[]认证配置正确
- [] MCP转换的策略模板
-[]生成业务流程兼容性命令行验证
- [] **paconn validate**:`paconn validate --api-def apiDefinition.swagger.json`通过无错误
- [] **pac CLI ready**：连接器可以是created/updated和`pac connector create/update`-[] **脚本验证**：脚本。csx通过pac CLI上传过程中的自动验证
-[] **包验证**:`ConnectorPackageValidator.ps1`运行成功

OAuth和安全要求
- [] **OAuth 2.0增强**：标准OAuth 2.0与MCP安全最佳实践的实现
-[] **令牌验证**：实现令牌受众验证，防止直通攻击
-[] **自定义安全逻辑**：增强脚本验证。csx以符合MCP
-[] **状态参数保护**:CSRF防护的安全状态参数
—[]**HTTPS强制**：所有生产端只使用HTTPS
- [] **MCP安全实践**：在OAuth 2.0中实现混淆代理攻击防范认证要求
-[] **完整的元数据**:settings.json，包含产品和服务信息
-[] **图标遵从性**:PNG格式，230x230或500x500尺寸
-[] **文档**：包含全面示例的认证就绪自述文件
-[] **安全遵从性**:OAuth 2.0增强了MCP安全实践，隐私政策
-[] **认证流程**：正确配置自定义安全验证的OAuth 2.0

##使用示例```yaml
Mode: Complete New Connector
Connector Name: Customer Analytics MCP
Server Purpose: Customer data analysis and insights
Tools Needed:
  - searchCustomers: Find customers by criteria
  - getCustomerProfile: Retrieve detailed customer data
  - analyzeCustomerTrends: Generate trend analysis
Resources:
  - Customer profiles (JSON data)
  - Analysis reports (structured data)
Authentication: oauth2
Host Environment: Azure Function
Target APIs: CRM REST API
```
