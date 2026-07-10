---
description: Comprehensive development guidelines for Microsoft 365 Copilot declarative agents with schema v1.5, TypeSpec integration, and Microsoft 365 Agents Toolkit workflows
applyTo: "**.json, **.ts, **.tsp, **manifest.json, **agent.json, **declarative-agent.json"
---
# Microsoft 365声明代理开发指南

# #概述

Microsoft 365 Copilot声明式代理是功能强大的自定义AI助手，它扩展了Microsoft 365 Copilot的专业功能、企业数据访问和自定义行为。这些指南为使用最新的v1.5 JSON模式规范和完整的Microsoft 365 agents Toolkit集成创建生产就绪代理提供了全面的开发实践。

模式规范v1.5

核心属性```json
{
  "$schema": "https://developer.microsoft.com/json-schemas/copilot/declarative-agent/v1.5/schema.json",
  "version": "v1.5",
  "name": "string (max 100 characters)",
  "description": "string (max 1000 characters)", 
  "instructions": "string (max 8000 characters)",
  "capabilities": ["array (max 5 items)"],
  "conversation_starters": ["array (max 4 items, optional)"]
}
```
###字符限制和约束
—**名称**：长度不超过100个字符
—**描述**：最多1000个字符
—**说明**：最多8000个字符
- **功能**：最多5项，最少1项
- **对话启动器**：最多4个项目，可选

##可用功能

核心能力
1. **WebSearch**：互联网搜索和实时信息访问
2. **OneDriveAndSharePoint**：文件访问，文档搜索，内容管理
3. **GraphConnectors**：来自第三方系统的企业数据集成
4. **MicrosoftGraph**：访问Microsoft 365服务和数据

沟通与协作
5. **TeamsAndOutlook**：团队聊天，会议，电子邮件集成
6. ** copilotformmicrosoft365 **：高级副驾驶功能和工作流程业务应用程序
7. **PowerPlatform**: Power Apps, Power automation， Power BI集成
8. **BusinessDataProcessing**：高级数据分析和处理
9. ** worddexcel **：文档创建、编辑、分析
10. **EnterpriseApplications**：第三方业务系统集成
11. **CustomConnectors**：自定义API和服务集成

Microsoft 365 Agents Toolkit集成VS Code扩展设置```bash
# Install Microsoft 365 Agents Toolkit
# Extension ID: teamsdevapp.ms-teams-vscode-extension
```
TypeSpec开发工作流

# # # # 1。现代代理人定义```typespec
import "@typespec/json-schema";

using TypeSpec.JsonSchema;

@jsonSchema("/schemas/declarative-agent/v1.5/schema.json")
namespace DeclarativeAgent;

/** Microsoft 365 Declarative Agent */
model Agent {
  /** Schema version */
  @minLength(1)
  $schema: "https://developer.microsoft.com/json-schemas/copilot/declarative-agent/v1.5/schema.json";
  
  /** Agent version */
  version: "v1.5";
  
  /** Agent name (max 100 characters) */
  @maxLength(100)
  @minLength(1)
  name: string;
  
  /** Agent description (max 1000 characters) */
  @maxLength(1000)
  @minLength(1)  
  description: string;
  
  /** Agent instructions (max 8000 characters) */
  @maxLength(8000)
  @minLength(1)
  instructions: string;
  
  /** Agent capabilities (1-5 items) */
  @minItems(1)
  @maxItems(5)
  capabilities: AgentCapability[];
  
  /** Conversation starters (max 4 items) */
  @maxItems(4)
  conversation_starters?: ConversationStarter[];
}

/** Available agent capabilities */
union AgentCapability {
  "WebSearch",
  "OneDriveAndSharePoint", 
  "GraphConnectors",
  "MicrosoftGraph",
  "TeamsAndOutlook",
  "PowerPlatform",
  "BusinessDataProcessing",
  "WordAndExcel",
  "CopilotForMicrosoft365",
  "EnterpriseApplications",
  "CustomConnectors"
}

/** Conversation starter definition */
model ConversationStarter {
  /** Starter text (max 100 characters) */
  @maxLength(100)
  @minLength(1)
  text: string;
}
```
# # # # 2。编译成JSON```bash
# Compile TypeSpec to JSON manifest
tsp compile agent.tsp --emit=@typespec/json-schema
```
环境配置

####开发环境```json
{
  "name": "${DEV_AGENT_NAME}",
  "description": "Development version: ${AGENT_DESCRIPTION}",
  "instructions": "${AGENT_INSTRUCTIONS}",
  "capabilities": ["${REQUIRED_CAPABILITIES}"]
}
```
####生产环境```json
{
  "name": "${PROD_AGENT_NAME}",
  "description": "${AGENT_DESCRIPTION}",
  "instructions": "${AGENT_INSTRUCTIONS}",
  "capabilities": ["${PRODUCTION_CAPABILITIES}"]
}
```
开发最佳实践

# # # 1。模式验证```typescript
// Validate against v1.5 schema
const schema = await fetch('https://developer.microsoft.com/json-schemas/copilot/declarative-agent/v1.5/schema.json');
const validator = new JSONSchema(schema);
const isValid = validator.validate(agentManifest);
```
# # # 2。字符限制管理```typescript
// Validation helper functions
function validateName(name: string): boolean {
  return name.length > 0 && name.length <= 100;
}

function validateDescription(description: string): boolean {
  return description.length > 0 && description.length <= 1000;
}

function validateInstructions(instructions: string): boolean {
  return instructions.length > 0 && instructions.length <= 8000;
}
```
# # # 3。能力选择策略
- **简单开始**：从1-2个核心能力开始
—**增量添加**：根据用户反馈增加功能
- **性能测试**：彻底测试每个功能组合
- **企业准备**：考虑合规性和安全性影响

代理游乐场测试

本地测试设置```bash
# Start Agents Playground
npm install -g @microsoft/agents-playground
agents-playground start --manifest=./agent.json
```
测试场景
1. **能力验证**：测试每个声明的能力
2. **会话流程**：验证会话启动器
3. **错误处理**：测试无效输入和边缘情况
4. **性能**：测量响应时间和可靠性

部署和生命周期管理

# # # 1。开发生命周期```mermaid
graph LR
    A[TypeSpec Definition] --> B[JSON Compilation]
    B --> C[Local Testing]
    C --> D[Validation]
    D --> E[Staging Deployment]
    E --> F[Production Release]
```
# # # 2。版本管理```json
{
  "name": "MyAgent v1.2.0",
  "description": "Production agent with enhanced capabilities",
  "version": "v1.5",
  "metadata": {
    "version": "1.2.0",
    "build": "20241208.1",
    "environment": "production"
  }
}
```
# # # 3。环境促进
- **开发**：全面调试，详细日志
- **Staging**：类似于生产的测试、性能监控
- **生产**：优化的性能，最小的日志记录

##高级功能

###行为覆盖```json
{
  "instructions": "You are a specialized financial analyst agent. Always provide disclaimers for financial advice.",
  "behavior_overrides": {
    "response_tone": "professional",
    "max_response_length": 2000,
    "citation_requirements": true
  }
}
```
本地化支持```json
{
  "name": {
    "en-US": "Financial Assistant",
    "es-ES": "Asistente Financiero",
    "fr-FR": "Assistant Financier"
  },
  "description": {
    "en-US": "Provides financial analysis and insights",
    "es-ES": "Proporciona análisis e insights financieros",
    "fr-FR": "Fournit des analyses et insights financiers"
  }
}
```
##监控和分析

###性能指标
-每个功能的响应时间
-用户参与对话启动
-错误率和故障模式
-能力利用率统计

日志策略```typescript
// Structured logging for agent interactions
const log = {
  timestamp: new Date().toISOString(),
  agentName: "MyAgent",
  version: "1.2.0",
  userId: "user123",
  capability: "WebSearch",
  responseTime: 1250,
  success: true
};
```
安全性和合规性

###数据隐私
-对敏感信息进行适当的数据处理
-确保遵守GDPR、CCPA和组织政策
-为企业功能使用适当的访问控制

安全考虑
-验证所有输入和输出
-实施速率限制和滥用预防
-监控可疑活动模式
-定期进行安全审计和更新

# #故障排除

###常见问题
1. **模式验证错误**：检查字符限制和必填字段
2. **能力冲突**：验证是否支持能力组合
3. **性能问题**：监控响应时间和优化指令
4. **部署失败**：验证环境配置和权限调试工具
- TypeSpec编译器诊断
- agent Playground调试
—Microsoft 365 agent Toolkit日志
-模式验证实用程序

这个全面的指南确保了强大的，可扩展的和可维护的Microsoft 365 Copilot声明性代理与完整的TypeSpec和Microsoft 365 agents Toolkit集成。