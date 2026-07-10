---
name: phoenix-tracing
description: OpenInference semantic conventions and instrumentation for Phoenix AI observability. Use when implementing LLM tracing, creating custom spans, or deploying to production.
license: Apache-2.0
compatibility: Requires Phoenix server. Python skills need arize-phoenix-otel; TypeScript skills need @arizeai/phoenix-otel.
metadata:
  author: oss@arize.com
  version: "1.0.0"
  languages: "Python, TypeScript"
---
#凤凰追踪

在Phoenix中使用OpenInference跟踪检测LLM应用程序的综合指南。包含涵盖设置、检测、跨度类型和生产部署的参考文件。

何时申请

以下情况可参考本指南：

-设置Phoenix跟踪（Python或TypeScript）
-为LLM操作创建自定义跨度
-添加属性遵循OpenInference约定
-将跟踪部署到生产环境
—跟踪数据查询和分析

##参考类别

|优先级|类别|描述|前缀|| -------- | --------------- | ------------------------------ | -------------------------- |
| 1 |安装|安装配置|`setup-*`|
|自动和手动跟踪|`instrumentation-*`|
| | Span Types | 9 Span Types with attributes |`span-*`|
| |组织|项目和会议|`projects-*`，`sessions-*`|
bbb5 |浓缩|自定义元数据|`metadata-*`|
bbb6 |生产|批量加工，屏蔽|`production-*`|
bbb7 |反馈|注释和评估|`annotations-*`|

##快速参考

# # # 1。设置（从这里开始）

—[setup-python]（references/setup-python.md）—安装phoenix-otel，配置endpoint
-安装@arizeai/phoenix-otel，配置终端

# # # 2。仪表- [instrumentation-auto-python](references/instrumentation-auto-python.md) - Auto-instrument OpenAI， LangChain等
- [instrumentation-auto-typescript](references/instrumentation-auto-typescript.md) -自动仪表支持的框架
- [instrumentation-manual-python](references/instrumentation-manual-python.md) -使用装饰器自定义跨度
- [instrumentation-manual-typescript](references/instrumentation-manual-typescript.md) -使用包装器自定义跨度

# # # 3。跨度类型（具有完整的属性模式）

- [span-llm](references/span-llm.md) - LLM API调用（模型，令牌，消息，成本）
- [span-chain](references/span-chain.md) -多步骤工作流和管道
- [span- retriver](references/span-retriever.md) -文档检索（文档，分数）
- [span-tool](references/span-tool.md) -Function/API调用（名称，参数）
- [span-agent](references/span-agent.md) -多步推理代理
- [span-embedding](references/span-embedding.md) -向量生成
- [span-reranker](references/span-reranker.md) -文件重新排序
- [span-guardrail](references/span-guardrail.md) -安全检查
- [span-evaluator](references/span-evaluator.md) - LLM评估

# # # 4。组织- [projects-python](references/projects-python.md) / [projects-typescript](references/projects-typescript.md) -按应用程序分组跟踪
- [sessions-python](references/sessions-python.md) / [sessions-typescript](references/sessions-typescript.md) -跟踪会话

# # # 5。浓缩

- [metadata-python](references/metadata-python.md) / [metadata-typescript](references/metadata-typescript.md) -自定义属性

# # # 6。生产(关键)

- [production-python](references/production-python.md) / [production-typescript](references/production-typescript.md) -批处理，PII屏蔽

# # # 7。反馈

-[注解-概述](references/annotations-overview.md) -反馈概念
- [annotations-python](references/annotations-python.md) / [annotations-typescript](references/annotations-typescript.md) -添加反馈到跨度

参考文件-[基础-概述](references/fundamentals-overview.md) -痕迹，跨度，属性基础
- [basics - Required -attributes](references/fundamentals-required-attributes.md) -每个跨度类型的必填字段
- [fundamentals-universal-attributes](references/fundamentals-universal-attributes.md) -通用属性（user. properties）。id、session.id)
- [basics -扁平化](references/fundamentals-flattening.md) - JSON扁平化规则
- [attributes-messages](references/attributes-messages.md) -聊天消息格式
- [attributes-metadata](references/attributes-metadata.md) -自定义元数据模式
- [attributes-graph](references/attributes-graph.md) - Agent工作流属性
- [attributes-exceptions](references/attributes-exceptions.md) -错误跟踪

##通用工作流

- **快速入门**:setup-{lang}→instrumentation-auto-{lang}→检查凤凰
- **自定义跨度**:setup-{lang}→instrumentation-manual-{lang}→span-{type}
- **会话跟踪**：会话-{lang}用于会话分组模式
—**生产**：生产-{lang}用于批处理、屏蔽和部署

如何使用这个技能导航模式:* * * *```bash
# By category prefix
references/setup-*              # Installation and configuration
references/instrumentation-*    # Auto and manual tracing
references/span-*               # Span type specifications
references/sessions-*           # Session tracking
references/production-*         # Production deployment
references/fundamentals-*       # Core concepts
references/attributes-*         # Attribute specifications

# By language
references/*-python.md          # Python implementations
references/*-typescript.md      # TypeScript implementations
```
* *阅读顺序:* *
1. 从您的语言的setup-{lang}开始
2. 选择instrumentation-auto-{lang}或instrumentation-manual-{lang}
3. 参考span-{type}文件，以满足特定操作的需要
4. 有关属性规范，请参见fundamentals-*文件

# #引用

凤凰文档:* * * *

-[凤凰文档]（https://docs.arize.com/phoenix）
- [OpenInference规范]（https://github.com/Arize-ai/openinference/tree/main/spec）

**Python API文档：**

- [Python OTEL Package](https://arize-phoenix.readthedocs.io/projects/otel/en/latest/) -`arize-phoenix-otel`API参考
- [Python客户端包](https://arize-phoenix.readthedocs.io/projects/client/en/latest/) -`arize-phoenix-client`API参考

**TypeScript API文档：**

- [TypeScript Packages](https://arize-ai.github.io/phoenix/) -`@arizeai/phoenix-otel`,`@arizeai/phoenix-client`，和其他TypeScript包