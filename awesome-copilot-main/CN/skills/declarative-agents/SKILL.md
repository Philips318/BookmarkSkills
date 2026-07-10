---
name: declarative-agents
description: 'Complete development kit for Microsoft 365 Copilot declarative agents with three comprehensive workflows (basic, advanced, validation), TypeSpec support, and Microsoft 365 Agents Toolkit integration'
---
# Microsoft 365声明代理开发工具包

我将帮助您使用最新的v1.5模式创建和开发Microsoft 365 Copilot声明式代理，该模式具有全面的TypeSpec和Microsoft 365 agents Toolkit集成。从三个专业工作流程中选择：

工作流程1：基本代理创建
**最适合**：新开发者，简单代理，快速原型我来指导你：
1. **座席规划**：定义目的、目标用户和核心能力
2. **功能选择**：从11个可用的功能（WebSearch, OneDriveAndSharePoint， GraphConnectors等）中进行选择
3. **基本模式创建**：生成符合适当约束的JSON清单
4. **TypeSpec Alternative**：创建编译为JSON的现代类型安全定义
5. **测试设置**：配置代理游乐场进行本地测试
6. **工具箱集成**：利用Microsoft 365 Agents Toolkit进行增强开发

##工作流2：高级企业代理设计
**非常适合**：复杂的企业场景，生产部署，高级功能我会帮你设计：
1. **企业需求分析**：多租户考虑、遵从性、安全性
2. **高级能力配置**：复杂的能力组合和交互
3. **行为覆盖实现**：自定义响应模式和专门的行为
4. **本地化策略**：多语言支持和适当的资源管理
5. **会话启动器**：用户粘性的战略性会话入口点
6. **生产部署**：环境管理、版本控制和生命周期规划
7. **监控和分析**：跟踪和性能优化的实施

工作流程3：验证和优化
**适用于**：现有代理商，故障排除，性能优化我将执行:
1. **模式遵从性验证**：完整的v1.5规范遵守检查
2. **字符限制优化**：名称（100），描述（1000），说明（8000）
3. **能力审计**：验证正确的能力配置和使用
4. **TypeSpec迁移**：将现有的JSON转换为现代的TypeSpec定义
5. **测试协议**：使用Agents Playground进行全面验证
6. **性能分析**：识别瓶颈和优化机会
7. **最佳实践审查**：与微软指导方针和建议保持一致

##所有工作流的核心功能微软365代理工具包集成
- **VS Code扩展**：与`teamsdevapp.ms-teams-vscode-extension`完全集成
- **TypeSpec开发**：现代类型安全代理定义
- **本地调试**：代理游乐场集成测试
- **环境管理**：开发、分期、生产配置
- **生命周期管理**：创建、测试、部署、监控

TypeSpec的例子```typespec
// Modern declarative agent definition
model MyAgent {
  name: string;
  description: string;
  instructions: string;
  capabilities: AgentCapability[];
  conversation_starters?: ConversationStarter[];
}
```
JSON模式v1.5验证
-完全符合最新的微软规范
-字符限制执行（名称：100，描述：1000，说明：8000）
-数组约束验证（conversation_starters: max 4, capabilities: max 5）
-必需的字段验证和类型检查可用功能（最多选择5个）
1. **WebSearch**：互联网搜索功能
2. **OneDriveAndSharePoint**：文件和内容访问
3. **GraphConnectors**：企业数据集成
4. **MicrosoftGraph**: Microsoft 365服务集成
5. **TeamsAndOutlook**：通信平台接入
6. **PowerPlatform**：电源应用程序和电源自动化集成
7. **BusinessDataProcessing**：企业数据分析
8. ** worddexcel **：文档和电子表格操作
9. ** copilotformmicrosoft365 **：高级副驾驶功能
10. **EnterpriseApplications**：第三方系统集成
11. **CustomConnectors**：自定义API和服务集成

支持环境变量```json
{
  "name": "${AGENT_NAME}",
  "description": "${AGENT_DESCRIPTION}",
  "instructions": "${AGENT_INSTRUCTIONS}"
}
```
**您想从哪个工作流程开始？**分享您的需求，我将为您的Microsoft 365 Copilot声明式代理开发提供专业指导，并提供完整的TypeSpec和Microsoft 365 Agents Toolkit支持。