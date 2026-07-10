---
name: typespec-create-api-plugin
description: 'Generate a TypeSpec API plugin with REST operations, authentication, and Adaptive Cards for Microsoft 365 Copilot'
---
#创建TypeSpec API插件

为Microsoft 365 Copilot创建一个完整的TypeSpec API插件，与外部REST API集成。

# #要求

使用以下命令生成TypeSpec文件：

# # #主要。tsp -座席定义```typescript
import "@typespec/http";
import "@typespec/openapi3";
import "@microsoft/typespec-m365-copilot";
import "./actions.tsp";

using TypeSpec.Http;
using TypeSpec.M365.Copilot.Agents;
using TypeSpec.M365.Copilot.Actions;

@agent({
  name: "[Agent Name]",
  description: "[Description]"
})
@instructions("""
  [Instructions for using the API operations]
""")
namespace [AgentName] {
  // Reference operations from actions.tsp
  op operation1 is [APINamespace].operationName;
}
```
# # #行动。tsp - API操作```typescript
import "@typespec/http";
import "@microsoft/typespec-m365-copilot";

using TypeSpec.Http;
using TypeSpec.M365.Copilot.Actions;

@service
@actions(#{
    nameForHuman: "[API Display Name]",
    descriptionForModel: "[Model description]",
    descriptionForHuman: "[User description]"
})
@server("[API_BASE_URL]", "[API Name]")
@useAuth([AuthType]) // Optional
namespace [APINamespace] {
  
  @route("[/path]")
  @get
  @action
  op operationName(
    @path param1: string,
    @query param2?: string
  ): ResponseModel;

  model ResponseModel {
    // Response structure
  }
}
```
##认证选项

根据API要求选择：

1. **无身份验证**（公共api）   ```typescript
   // No @useAuth decorator needed
   ```
2. * * API密匙* *   ```typescript
   @useAuth(ApiKeyAuth<ApiKeyLocation.header, "X-API-Key">)
   ```
3. * * OAuth2 * *   ```typescript
   @useAuth(OAuth2Auth<[{
     type: OAuth2FlowType.authorizationCode;
     authorizationUrl: "https://oauth.example.com/authorize";
     tokenUrl: "https://oauth.example.com/token";
     refreshUrl: "https://oauth.example.com/token";
     scopes: ["read", "write"];
   }]>)
   ```
4. **注册授权参考**   ```typescript
   @useAuth(Auth)
   
   @authReferenceId("registration-id-here")
   model Auth is ApiKeyAuth<ApiKeyLocation.header, "X-API-Key">
   ```
##功能功能

确认对话框```typescript
@capabilities(#{
  confirmation: #{
    type: "AdaptiveCard",
    title: "Confirm Action",
    body: """
    Are you sure you want to perform this action?
      * **Parameter**: {{ function.parameters.paramName }}
    """
  }
})
```
自适应卡响应```typescript
@card(#{
  dataPath: "$.items",
  title: "$.title",
  url: "$.link",
  file: "cards/card.json"
})
```
###推理和回应说明```typescript
@reasoning("""
  Consider user's context when calling this operation.
  Prioritize recent items over older ones.
""")
@responding("""
  Present results in a clear table format with columns: ID, Title, Status.
  Include a summary count at the end.
""")
```
最佳实践

1. **操作名称**：使用明确的、面向操作的名称（listProjects, createTicket）
2. **模型：为请求和响应定义类似typescript的模型
3. **HTTP方法：使用适当的动词（@get, @post, @patch, @delete）
4. **路径**：使用@route的RESTful路径约定
5. **参数**：适当使用@path， @query, @header, @body
6. **描述**：为理解模型提供清晰的描述
7. **确认**：添加破坏性操作（删除、更新关键数据）
8. **卡片**：用于多个数据项的丰富视觉响应

# #工作流程

询问用户：
1. API的基础URL和目的是什么？
2. 需要哪些操作（CRUD操作）？
3. API使用什么身份验证方法？
4. 任何操作都需要确认吗？
5. 响应需要自适应卡吗？然后生成:
-用代理定义完成`main.tsp`-完成`actions.tsp`的API操作和模型
—如果需要适配卡，可选`cards/card.json`