---
description: 'Instructions for using LangChain with Python'
applyTo: "**/*.py"
---
# LangChain Python指令

这些说明指导GitHub Copilot在Python中为LangChain应用程序生成代码和文档。关注特定于langchain的模式、api和最佳实践。

##可运行接口（特定于langchain）

LangChain的`Runnable`接口是组合和执行链、聊天模型、输出解析器、检索器和LangGraph图的基础。它为调用、批处理、流处理、检查和组合组件提供了统一的API。

**关键的langchain特定功能：**所有主要的LangChain组件（聊天模型、输出解析器、检索器、图形）都实现了Runnable接口。
—支持同步执行（`invoke`,`batch`,`stream`）和异步执行（`ainvoke`,`abatch`,`astream`）。
批处理（`batch`,`batch_as_completed`）针对并行API调用进行了优化；在`RunnableConfig`中设置`max_concurrency`来控制并行度。
-流式api （`stream`,`astream`,`astream_events`）生成输出，对于响应式LLM应用程序至关重要。Input/output类型是特定于组件的（例如，聊天模型接受消息，检索器接受字符串，输出解析器接受模型输出）。
-使用`get_input_schema`，`get_output_schema`及其JSONSchema变体检查模式，以进行验证和OpenAPI生成。
-使用`with_types`来覆盖复杂LCEL链的推断input/output类型。
-用LCEL:`chain = prompt | chat_model | output_parser`声明性地编写Runnables。
-传播`RunnableConfig`（标签、元数据、回调、并发）在Python 3.11+中自动使用；手动在Python的异步代码3.9/3.10.-创建自定义运行与`RunnableLambda`（简单转换）或`RunnableGenerator`（流式转换）；避免直接子类化。
-为动态链和LangServe部署配置`configurable_fields`和`configurable_alternatives`的运行时属性和备选方案。**LangChain最佳实践：**

-使用批处理并行API调用llm或检索器；设置`max_concurrency`以避免速率限制。
-首选流api聊天ui和长输出。
-始终验证自定义链和部署端点的input/output模式。
-在`RunnableConfig`中使用标签和元数据来跟踪LangSmith和调试复杂的链。
对于自定义逻辑，用`RunnableLambda`或`RunnableGenerator`包装函数，而不是子类化。
-对于高级配置，通过`configurable_fields`和`configurable_alternatives`暴露字段和备选项。


-使用LangChain的聊天模型集成会话AI：

—从`langchain.chat_models`或`langchain_openai`导入（例如`ChatOpenAI`）。
—使用`SystemMessage`、`HumanMessage`、`AIMessage`撰写消息。
—工具调用，使用`bind_tools(tools)`方法。
—结构化输出：使用`with_structured_output(schema)`。

例子:```python
from langchain_openai import ChatOpenAI
from langchain.schema import HumanMessage, SystemMessage

chat = ChatOpenAI(model="gpt-4", temperature=0)
messages = [
    SystemMessage(content="You are a helpful assistant."),
    HumanMessage(content="What is LangChain?")
]
response = chat.invoke(messages)
print(response.content)
```
-将消息组成为`SystemMessage`、`HumanMessage`和`AIMessage`对象的列表。
-对于RAG，将聊天模型与retrievers/vectorstores结合起来进行上下文注入。
-使用`streaming=True`进行实时令牌流（如果支持）。
-使用`tools`参数function/tool调用（OpenAI， Anthropic等）。
-使用`response_format="json"`的结构化输出（OpenAI模型）。

最佳实践:

-在下游任务中使用模型输出之前，始终对其进行验证。
—为了清晰和可靠，首选显式消息类型。
-为副驾驶提供清晰、可操作的提示并记录预期输出。- LLM客户端工厂：集中提供商配置（API密钥），超时，重试和遥测。提供一个地方来切换提供程序或客户端设置。
-提示模板：将模板存储在`prompts/`下，并通过安全助手加载。保持模板小而可测试。
-链与代理：对于确定性管道（RAG，摘要），更倾向于链。当您需要规划或动态工具选择时，请使用代理。
-工具：实现工具的类型化适配器接口；严格验证输入和输出。
—内存：默认为无状态设计。当需要内存时，存储最少的上下文并文档retention/erasure策略。
-检索器：构建检索+重新排序管道。保持vectorstore模式稳定（id、文本、元数据）。

# # #模式-回调和跟踪：使用LangChain回调并与LangSmith或您的跟踪系统集成来捕获request/response的生命周期。
-关注点分离：保持提示构建、LLM布线和业务逻辑分离，以简化测试并减少意外提示更改。

嵌入和矢量存储

—使用一致的分块和元数据字段（source、page、chunk_index）。
-缓存嵌入，以避免未更改文档的重复成本。
-Local/dev：色度或FAISS。生产：根据规模和sla管理矢量db （Pinecone, Qdrant, Milvus, Weaviate）。

向量存储（特定于langchain）-使用LangChain的矢量存储集成来进行语义搜索、检索增强生成（RAG）和文档相似度工作流。
-始终使用支持的嵌入模型初始化vectorstores（例如，OpenAIEmbeddings, HuggingFaceEmbeddings）。
-首选官方集成（例如，Chroma， FAISS, Pinecone, Qdrant, Weaviate）用于生产；使用InMemoryVectorStore进行测试和演示。
-使用`page_content`和`metadata`将文档存储为LangChain`Document`对象。
—使用“`add_documents(documents, ids=...)`”~“add/update”格式的文档。始终为更新提供唯一的id。
—使用“`delete(ids=...)`”按ID删除文档。
—使用`similarity_search(query, k=4, filter={...})`检索top-k相似文档。使用元数据过滤器进行范围搜索。
-对于RAG，将你的vectorstore连接到一个检索器和一个LLM链（见LangChain检索器和RAGChain文档）。
-高级搜索，使用vectorstore特定的选项：Pinecone支持混合搜索和元数据过滤;Chroma支持过滤和自定义距离度量。
-始终在您的环境中验证vectorstore集成和API版本；破坏性变更在LangChain版本之间很常见。
示例（InMemoryVectorStore）：```python
from langchain_core.vectorstores import InMemoryVectorStore
from langchain_openai import OpenAIEmbeddings
from langchain_core.documents import Document

embedding_model = OpenAIEmbeddings()
vector_store = InMemoryVectorStore(embedding=embedding_model)

documents = [Document(page_content="LangChain content", metadata={"source": "doc1"})]
vector_store.add_documents(documents=documents, ids=["doc1"])

results = vector_store.similarity_search("What is RAG?", k=2)
for doc in results:
    print(doc.page_content, doc.metadata)
```
-对于生产环境，首选持久矢量存储（Chroma, Pinecone, Qdrant, Weaviate），并根据供应商文档配置身份验证，缩放和备份。
—参考编号：https://python.langchain.com/docs/integrations/vectorstores/提示工程和治理

-在`prompts/`下存储规范提示，并通过文件名从代码中引用它们。
编写单元测试，断言需要的占位符存在，并且呈现的提示符符合预期的模式（长度、存在的变量）。
维护变更日志，记录影响行为的提示和模式变更。

##聊天模型

LangChain为聊天模型提供了一致的界面，并提供了用于监控、调试和优化的附加功能。

# # #集成

集成包括：

1. 官方：由LangChain团队或提供商维护的打包`langchain-<provider>`集成。
2. 社区：贡献的集成（在`langchain-community`中）。聊天模型通常遵循带有`Chat`前缀的命名约定（例如，`ChatOpenAI`、`ChatAnthropic`、`ChatOllama`）。没有`Chat`前缀（或带有`LLM`后缀）的模型通常实现较旧的string-in/string-out接口，并且不太适合现代聊天工作流。

# # #界面

聊天模型实现`BaseChatModel`并支持Runnable接口：流、异步、批处理等等。许多操作接受并返回LangChain`messages`（角色如`system`、`user`、`assistant`）。有关详细信息，请参阅BaseChatModel API参考。

主要方法包括：

-`invoke(messages, ...)`-发送消息列表并接收响应。
-`stream(messages, ...)`-流部分输出作为令牌到达。
-`batch(inputs, ...)`批量处理多个请求。
-`bind_tools(tools)`-附加工具适配器，用于工具调用。
-`with_structured_output(schema)`-请求结构化响应的助手。

输入和输出- LangChain支持自己的消息格式和OpenAI的消息格式；在你的代码库中选择一个一致的。
-消息包括`role`和`content`块；在支持的情况下，内容可以包括结构化的或多模式的有效载荷。

标准参数

常用支持的参数（依赖于提供程序）：

-`model`：型号标识符(例如：`gpt-4o``gpt-3.5-turbo`)。
-`temperature`：随机控制（0.0确定性- 1.0创造性）
—`timeout`：取消前等待的秒数。
—`max_tokens`：响应令牌限制。
—`stop`：停止序列。
—`max_retries`:network/limit失败后重试。
—`api_key`、`base_url`：提供程序认证和端点配置。
—`rate_limiter`：可选的BaseRateLimiter，用于对请求进行空间化，避免提供商配额错误。

>注意：并非所有参数都由每个提供程序实现。始终查阅提供商集成文档。

工具调用聊天模型可以调用工具（api、db、系统适配器）。使用LangChain的工具调用api：

—注册严格使用input/output类型的工具。
-观察和记录工具调用请求和结果。
-在将工具输出传递回模型或执行副作用之前验证它们。

请参阅LangChain文档中的工具调用指南，以获取示例和安全模式。

结构化输出

使用`with_structured_output`或模式强制方法从模型请求JSON或类型化输出。结构化输出对于可靠的提取和下游处理（解析器、数据库写入、分析）是必不可少的。

# # #多峰性

一些模型支持多模态输入（图像、音频）。查看提供商文档了解支持的输入类型和限制。多模态输出是罕见的-将它们视为实验性的并严格验证。

###上下文窗口模型有一个有限的上下文窗口，以令牌表示。在设计会话流程时：

-保持信息简洁，并优先考虑重要的背景。
-当旧上下文超出窗口时，在模型外修剪旧上下文（摘要或存档）。
-使用检索器+ RAG模式来显示相关的长篇上下文，而不是将大型文档粘贴到聊天中。

##高级主题

# # #病原

—初始化聊天模型到空间呼叫时使用`rate_limiter`。
-实现重试与指数回退，并考虑回退模型或降级模式时节流。

# # #缓存

-对话的精确输入缓存通常是无效的。考虑为重复的意义级查询使用语义缓存（基于嵌入）。
语义缓存引入了对嵌入的依赖，并不是普遍适用的。
-仅在降低成本和满足正确性要求的地方缓存（例如，FAQ bots）。最佳实践

-为公共api使用类型提示和数据类。
-在调用llm或工具之前验证输入。
-从秘密管理器加载秘密；永远不要记录秘密或未编辑的模型输出。
-确定性测试：模拟llm和嵌入调用。
-缓存嵌入和频繁检索结果。
-可观察性：日志request_id，模型名称，延迟，和净化令牌计数。
-对外部调用实施指数回退和幂等。

##安全和隐私

-将模型输出视为不可信。在执行生成的代码或系统命令之前进行清理。
验证任何用户提供的url和输入，以避免SSRF和注入攻击。
-文档数据保留和添加API，以删除用户数据的要求。
—限制存储PII，并对敏感字段进行加密。