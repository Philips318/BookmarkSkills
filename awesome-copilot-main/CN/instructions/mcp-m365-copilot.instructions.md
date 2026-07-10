---
description: 'Best practices for building MCP-based declarative agents and API plugins for Microsoft 365 Copilot with Model Context Protocol integration'
applyTo: '**/{*mcp*,*agent*,*plugin*,declarativeAgent.json,ai-plugin.json,mcp.json,manifest.json}'
---
基于mcp的M365副驾驶开发指南

##核心原则

###模型上下文协议首先
—利用MCP服务器进行外部系统集成
—从服务器端点导入工具，而不是手动定义
-让MCP处理模式发现和函数生成
—在agent Toolkit中使用点击式工具选择

陈述句优于祈使句
—通过配置而不是代码定义座席行为
-使用declarativeAgent.json表示指令和功能
—在ai-plugin.json中指定工具和操作
—配置“mcp.json”中的MCP服务器

安全性和治理
—始终使用OAuth 2.0或SSO进行认证
—选择工具时遵循最小权限原则
—验证MCP服务器端点是否安全
—部署前检查合规性要求以用户为中心的设计
-创建自适应卡丰富的视觉反应
-提供清晰的对话开头
-设计跨中心的响应体验
-在组织部署前进行彻底的测试

MCP服务器设计

###服务器选择
选择以下MCP服务器：
-为用户任务公开相关工具
—支持安全认证（OAuth 2.0， SSO）
—提供可靠的运行时间和性能
-遵循MCP规范标准
—返回结构良好的响应数据

工具导入策略
-只导入必要的工具（避免范围过大）
—将同一服务器上的相关工具分组
—组合使用前，请分别进行测试
—在选择多个工具时考虑令牌限制

###认证配置
**OAuth 2.0静态注册：**```json
{
  "type": "OAuthPluginVault",
  "reference_id": "YOUR_AUTH_ID",
  "client_id": "github_client_id",
  "client_secret": "github_client_secret",
  "authorization_url": "https://github.com/login/oauth/authorize",
  "token_url": "https://github.com/login/oauth/access_token",
  "scope": "repo read:user"
}
```
**单点登录（Microsoft Entra ID）：**```json
{
  "type": "OAuthPluginVault",
  "reference_id": "sso_auth",
  "authorization_url": "https://login.microsoftonline.com/common/oauth2/v2.0/authorize",
  "token_url": "https://login.microsoftonline.com/common/oauth2/v2.0/token",
  "scope": "User.Read"
}
```
##文件组织

项目结构```
project-root/
├── appPackage/
│   ├── manifest.json           # Teams app manifest
│   ├── declarativeAgent.json   # Agent config (instructions, capabilities)
│   ├── ai-plugin.json          # API plugin definition
│   ├── color.png               # App icon color
│   └── outline.png             # App icon outline
├── .vscode/
│   └── mcp.json               # MCP server configuration
├── .env.local                  # Credentials (NEVER commit)
└── teamsapp.yml               # Teams Toolkit config
```
###关键文件

* *declarativeAgent.json: * *
—座席的名称和描述
-行为说明
-开始谈话
-功能（插件的操作）

* *ai-plugin.json: * *
—MCP服务器工具导入
-响应语义（data_path, properties）
—静态自适应卡模板
-函数定义（自动生成）

* *mcp.json: * *
- MCP服务器URL
-服务器元数据端点
—认证参考

* * .env.local: * *
- OAuth客户端凭据
- API密钥和秘密
-特定于环境的配置
- **CRITICAL**：添加到。gitignore

响应语义最佳实践

###数据路径配置
使用JSONPath提取相关数据：```json
{
  "data_path": "$.items[*]",
  "properties": {
    "title": "$.name",
    "subtitle": "$.description", 
    "url": "$.html_url"
  }
}
```
模板选择
对于动态模板：```json
{
  "data_path": "$",
  "template_selector": "$.templateType",
  "properties": {
    "title": "$.title",
    "url": "$.url"
  }
}
```
静态模板
在ai-plugin.json中定义一致的格式：
-当所有响应遵循相同结构时使用
—性能优于动态模板
-更容易维护和版本控制

自适应卡片指南

设计原则
- **单列布局**：元素垂直堆叠
- **灵活的宽度**：使用“拉伸”或“自动”，而不是固定像素
- **响应式设计**：测试聊天，团队，Outlook
- **最小的复杂性**：保持卡片简单和可扫描

模板语言模式
* *条件:* *```json
{
  "type": "TextBlock",
  "text": "${if(status == 'active', '✅ Active', '❌ Inactive')}"
}
```
数据绑定:* * * *```json
{
  "type": "TextBlock",
  "text": "${title}",
  "weight": "bolder"
}
```
* *数字格式:* *```json
{
  "type": "TextBlock",
  "text": "Score: ${formatNumber(score, 0)}"
}
```
条件呈现:* * * *```json
{
  "type": "Container",
  "$when": "${count(items) > 0}",
  "items": [ ... ]
}
```
卡元素使用
- **TextBlock**：标题，描述，元数据
—**FactSet**：键值对（状态、日期、id）
- **图像**：图标，缩略图（使用大小：“小”）
—**容器**：对相关内容进行分组
- **ActionSet**：用于后续操作的按钮

测试和部署

本地测试工作流
1. **Provision**: Teams Toolkit→Provision
2. **Deploy**: Teams Toolkit→Deploy
3. **Sideload**：上传至团队的应用程序
4. **测试**：访问[m365.cloud.microsoft/chat]（https://m365.cloud.microsoft/chat）
5. **迭代**：修复问题并重新部署部署前检查表
-[]所有MCP服务器工具单独测试
-[]认证流程端到端工作
-[]自适应卡可以正确的跨中心渲染
-[]响应语义提取预期数据
—[]错误处理提供明确的信息
-[]对话的开头要相关且清晰
-[]代理指令指导正确的行为
-[]合规和安全审查

部署选项
* *组织部署:* *
—IT管理员部署到所有或选定的用户
—需要在Microsoft 365管理中心审批
-最适合内部业务代理

* *代理商店:* *
-提交给合作伙伴中心进行验证
-所有副驾驶用户的公共可用性
-需要严格的安全审查

##常见模式

多工具代理
从多个MCP服务器导入工具：```json
{
  "mcpServers": {
    "github": {
      "url": "https://github-mcp.example.com"
    },
    "jira": {
      "url": "https://jira-mcp.example.com"
    }
  }
}
```
###搜索和显示
1. 工具从MCP服务器检索数据
2. 响应语义提取相关字段
3. 自适应卡显示格式化的结果
4. 用户可以通过卡片按钮进行操作

经过验证的操作
1. 用户触发需要授权的工具
2. OAuth流重定向以获得同意
3. 访问令牌存储在插件库中
4. 后续请求使用存储的令牌

##错误处理

MCP服务器错误
—在座席响应中提供清晰的错误提示
-如果可用，退回到其他工具
—记录错误信息，方便调试
-引导用户重试或替代方法

认证失败
—检查.env.local中的OAuth凭据
-验证范围是否匹配所需的权限
-先在副驾驶外测试认证流程
—确保令牌刷新逻辑正常工作响应解析失败
-验证响应语义中的JSONPath表达式
—优雅地处理丢失或空数据
-在适当的地方提供默认值
-测试不同的API反应

性能优化

###工具选择
-只导入必要的工具（减少令牌的使用）
—避免来自多个服务器的冗余工具
-测试每个工具对响应时间的影响

响应大小
—使用data_path过滤不需要的数据
-尽可能限制结果集
-考虑大型数据集的分页
-保持自适应卡片的轻量级

缓存策略
- MCP服务器应该在适当的地方缓存
—座席响应可能会被M365缓存
-考虑对时间敏感数据的缓存失效

安全最佳实践凭证管理
- **NEVER** commit .env。本地到源代码控制
—对所有机密使用环境变量
—定期轮换OAuth凭据
—为dev/prod使用单独的凭据

###数据隐私
-只要求最小的必要范围
—避免记录敏感用户数据
-审查数据驻留要求
-遵循合规政策（GDPR等）

###服务器验证
—验证MCP服务器是否可信且安全
—只检查HTTPS端点
-审查服务器的隐私政策
-测试注入漏洞

治理和遵从性

### Admin控件
代理可以是：
- **Blocked**：禁止使用
—**部署**：分配给指定的users/groups- **发布**：在全组织范围内提供

# # #监控
跟踪:
-代理的使用和采用
—错误率和性能
-用户反馈和满意度
-保安事故审计要求
维护:
—修改座席配置的历史记录
—敏感操作的访问日志
—部署审批记录
-合规性证明

##资源和参考资料

官方文件
-[使用MCP构建声明式代理（DevBlogs）]（https://devblogs.microsoft.com/microsoft365dev/build-declarative-agents-for-microsoft-365-copilot-with-mcp/）
-[构建MCP插件（学习）]（https://learn.microsoft.com/en-us/microsoft-365-copilot/extensibility/build-mcp-plugins）
- [API插件自适应卡（学习）]（https://learn.microsoft.com/en-us/microsoft-365-copilot/extensibility/api-plugin-adaptive-cards）
-[管理副驾驶代理人（学习）]（https://learn.microsoft.com/en-us/microsoft-365/admin/manage/manage-copilot-agents-integrated-apps）

###工具和sdk
微软365代理工具包（VS Code扩展v6.3.x+）
- Teams代理打包工具包
-自适应卡片设计师
- MCP规范文件

合作伙伴的例子
- monday.com：任务管理集成
- Canva：设计自动化
—Sitecore：内容管理