---
name: mcp-copilot-studio-server-generator
description: 'Generate a complete MCP server implementation optimized for Copilot Studio integration with proper schema constraints and streamable HTTP support'
---
#电源平台MCP连接器发电机

生成一个完整的电源平台自定义连接器与模型上下文协议（MCP）集成为Microsoft Copilot Studio。该提示符根据Power Platform连接器标准创建所有必要的文件，并具有MCP可流式HTTP支持。

# #指令

创建一个完整的MCP服务器实现：

1. **使用Copilot Studio MCP模式：**
—执行`x-ms-agentic-protocol: mcp-streamable-1.0`—支持JSON-RPC 2.0通信协议
-在`/mcp`提供可流的HTTP端点
-采用Power Platform连接器结构2. **模式遵从性要求：**
- **在工具inputs/outputs中没有引用类型**（由Copilot Studio过滤）
- **只有单一类型的值**（不是多个类型的数组）
- **避免enum输入**（解释为字符串，而不是enum）
-使用基本类型：字符串，数字，整数，布尔值，数组，对象
—确保所有端点返回完整的uri

3. **MCP组件包括：**
- **Tools**：用于语言模型调用的函数（✅在Copilot Studio中支持）
- **资源**：类似文件的数据输出工具（✅支持在Copilot Studio -必须是工具输出是可访问的）
- **提示**：特定任务的预定义模板（❌Copilot Studio尚未支持）

4. * *实现结构:* *   ```
   /apiDefinition.swagger.json  (Power Platform connector schema)
   /apiProperties.json         (Connector metadata and configuration)
   /script.csx                 (Custom code transformations and logic)
   /server/                    (MCP server implementation)
   /tools/                     (Individual MCP tools)
   /resources/                 (MCP resource handlers)
   ```
##上下文变量

- **服务器用途**:[描述MCP服务器应该完成的功能]
- **所需工具**:[需要实施的具体工具清单]
- **资源**:[提供的资源类型]
- **认证**:[Auth method: none， api-key, oauth2]
- **主机环境**:[Azure Function，Express.js， FastAPI等]
- **目标api **:[要集成的外部api]

##预期输出

生成:

1. * *apiDefinition.swagger.json* *:
—正确的`x-ms-agentic-protocol: mcp-streamable-1.0`- MCP端点在POST`/mcp`兼容的模式定义（没有引用类型）
- McpResponse和McpErrorResponse定义

2. * *apiProperties.json* *:
-连接器元数据和品牌
—认证配置
-策略模板（如果需要）3. * *脚本。csx公司* *:
-自定义c#代码request/response转换
- MCP JSON-RPC消息处理逻辑
-数据验证和处理功能
-错误处理和日志记录功能

4. **MCP服务器代码**
- JSON-RPC 2.0请求处理程序
-工具注册和执行
-资源管理（作为工具输出）
-正确的错误处理
-副驾驶工作室兼容性检查

5. **单个工具**：
-只接受基本类型输入
-返回结构化输出
-在需要时将资源作为输出
-为Copilot Studio提供清晰的描述

6. **部署配置**适用于：
-电源平台环境
- Copilot Studio代理集成
-测试和验证

验证检查表确保生成的代码：
-[]模式中没有引用类型
-[]所有类型字段都是单一类型
-[]通过带验证的字符串处理Enum
-[]通过工具输出可获得的资源
—[]完整的URI端点
- [] JSON-RPC 2.0兼容性
-[]正确的x-ms-agent -protocol报头
- []McpResponse/McpErrorResponse模式
-[]清除Copilot Studio的工具描述
-[]生成业务流程兼容

##使用示例```yaml
Server Purpose: Customer data management and analysis
Tools Needed: 
  - searchCustomers
  - getCustomerDetails
  - analyzeCustomerTrends
Resources:
  - Customer profiles
  - Analysis reports
Authentication: oauth2
Host Environment: Azure Function
Target APIs: CRM System REST API
```
