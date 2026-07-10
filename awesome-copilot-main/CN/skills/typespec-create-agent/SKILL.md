---
name: typespec-create-agent
description: 'Generate a complete TypeSpec declarative agent with instructions, capabilities, and conversation starters for Microsoft 365 Copilot'
---
创建TypeSpec声明式代理

为Microsoft 365 Copilot创建一个完整的TypeSpec声明式代理，结构如下：

# #要求

生成一个`main.tsp`文件：

1. * * * *代理声明
-使用带有描述性名称和描述的`@agent`装饰器
—名称不超过100个字符
-描述不超过1000个字符

2. * * * *的指令
-使用带有明确行为指南的`@instructions`装饰器
-定义座席的角色、专业知识和个性
-指定代理应该和不应该做什么
-字数控制在8000字以内

3. * * * *开始交谈
-包括2-4名`@conversationStarter`装饰师
-每个都有一个标题和示例查询
-让他们多样化，展示不同的能力4. **功能**（根据用户需求）
-`WebSearch`-用于可选网站范围的web内容
-`OneDriveAndSharePoint`-用于URL过滤的文档访问
-`TeamsMessages`-用于团队channel/chat访问
-`Email`-用于文件夹过滤的电子邮件访问
-`People`-用于组织人员搜索
-`CodeInterpreter`-用于Python代码执行
-`GraphicArt`-用于图像生成
-`GraphConnectors`-用于副驾驶连接器内容
-`Dataverse`-用于Dataverse数据访问
—`Meetings`—会议内容接入

模板结构```typescript
import "@typespec/http";
import "@typespec/openapi3";
import "@microsoft/typespec-m365-copilot";

using TypeSpec.Http;
using TypeSpec.M365.Copilot.Agents;

@agent({
  name: "[Agent Name]",
  description: "[Agent Description]"
})
@instructions("""
  [Detailed instructions about agent behavior, role, and guidelines]
""")
@conversationStarter(#{
  title: "[Starter Title 1]",
  text: "[Example query 1]"
})
@conversationStarter(#{
  title: "[Starter Title 2]",
  text: "[Example query 2]"
})
namespace [AgentName] {
  // Add capabilities as operations here
  op capabilityName is AgentCapabilities.[CapabilityType]<[Parameters]>;
}
```
最佳实践

-使用描述性的、基于角色的座席名称（例如，“客户支持助理”、“研究助理”）
-用第二人称写说明（“你是……”）
-具体说明代理的专业知识和局限性
-包括不同的对话开头，展示不同的功能
—只包括座席实际需要的功能
-范围功能（url，文件夹等）在可能的情况下，以获得更好的性能
-多行指令使用三引号字符串

# #的例子

询问用户：
1. 代理的目的和作用是什么？
2. 它需要什么功能？
3. 它应该访问哪些知识来源？
4. 典型的用户交互是什么？

然后生成完整的TypeSpec代理定义。