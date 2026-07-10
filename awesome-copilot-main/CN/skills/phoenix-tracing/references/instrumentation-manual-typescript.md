# Manual Instrumentation （TypeScript）

使用方便的包装器或withSpan添加自定义跨度，以实现细粒度跟踪控制。

# #设置```bash
npm install @arizeai/phoenix-otel @arizeai/openinference-core
```

```typescript
import { register } from "@arizeai/phoenix-otel";
register({ projectName: "my-app" });
```
##快速参考

| Span类型|方法|用例||-----------|--------|----------|
| CHAIN |`traceChain`|工作流、管道、业务流程|
| AGENT |`traceAgent`|多步推理，规划|
| TOOL |`traceTool`|外部api，函数调用|
| retriver |`withSpan`|矢量搜索，文档检索|
| LLM |`withSpan`| LLM API调用（首选自动检测）|
|嵌入|`withSpan`|嵌入生成|
| RERANKER |`withSpan`|文档重新排序|
|护栏|`withSpan`|安全检查、内容审核|
| EVALUATOR |`withSpan`| LLM评估|

##方便包装```typescript
import { traceChain, traceAgent, traceTool } from "@arizeai/openinference-core";

// CHAIN - workflows
const pipeline = traceChain(
  async (query: string) => {
    const docs = await retrieve(query);
    return await generate(docs, query);
  },
  { name: "rag-pipeline" }
);

// AGENT - reasoning
const agent = traceAgent(
  async (question: string) => {
    const thought = await llm.generate(`Think: ${question}`);
    return await processThought(thought);
  },
  { name: "my-agent" }
);

// TOOL - function calls
const getWeather = traceTool(
  async (city: string) => fetch(`/api/weather/${city}`).then(r => r.json()),
  { name: "get-weather" }
);
```
## withSpan for Other Kinds```typescript
import { withSpan, getInputAttributes, getRetrieverAttributes } from "@arizeai/openinference-core";

// RETRIEVER with custom attributes
const retrieve = withSpan(
  async (query: string) => {
    const results = await vectorDb.search(query, { topK: 5 });
    return results.map(doc => ({ content: doc.text, score: doc.score }));
  },
  {
    kind: "RETRIEVER",
    name: "vector-search",
    processInput: (query) => getInputAttributes(query),
    processOutput: (docs) => getRetrieverAttributes({ documents: docs })
  }
);
```
* *选择:* *```typescript
withSpan(fn, {
  kind: "RETRIEVER",              // OpenInference span kind
  name: "span-name",              // Span name (defaults to function name)
  processInput: (args) => {},     // Transform input to attributes
  processOutput: (result) => {},  // Transform output to attributes
  attributes: { key: "value" }    // Static attributes
});
```
##捕获Input/Output**总是捕获I/O为求值准备的跨度。**使用`getInputAttributes`和`getOutputAttributes`助手自动MIME类型检测：```typescript
import {
  getInputAttributes,
  getOutputAttributes,
  withSpan,
} from "@arizeai/openinference-core";

const handleQuery = withSpan(
  async (userInput: string) => {
    const result = await agent.generate({ prompt: userInput });
    return result;
  },
  {
    name: "query.handler",
    kind: "CHAIN",
    // Use helpers - automatic MIME type detection
    processInput: (input) => getInputAttributes(input),
    processOutput: (result) => getOutputAttributes(result.text),
  }
);

await handleQuery("What is 2+2?");
```
**捕获的内容：**```json
{
  "input.value": "What is 2+2?",
  "input.mime_type": "text/plain",
  "output.value": "2+2 equals 4.",
  "output.mime_type": "text/plain"
}
```
* *辅助行为:* *
—字符串→`text/plain`-Objects/Arrays→`application/json`（自动序列化）
-`undefined`/`null`→不设置属性

**为什么重要：**
-凤凰评估器需要`input.value`和`output.value`Phoenix UI突出显示I/O以便调试
—启用数据导出功能，用于微调数据集

自定义I/O处理

在标准I/O属性旁边添加自定义元数据：```typescript
const processWithMetadata = withSpan(
  async (query: string) => {
    const result = await llm.generate(query);
    return result;
  },
  {
    name: "query.process",
    kind: "CHAIN",
    processInput: (query) => ({
      "input.value": query,
      "input.mime_type": "text/plain",
      "input.length": query.length,  // Custom attribute
    }),
    processOutput: (result) => ({
      "output.value": result.text,
      "output.mime_type": "text/plain",
      "output.tokens": result.usage?.totalTokens,  // Custom attribute
    }),
  }
);
```
##参见Also

—**Span属性：**`span-chain.md`、`span-retriever.md`、`span-tool.md`等。
- **属性帮助：**https://docs.arize.com/phoenix/tracing/manual-instrumentation-typescript#attribute-helpers**自动检测：**`instrumentation-auto-typescript.md`用于框架集成