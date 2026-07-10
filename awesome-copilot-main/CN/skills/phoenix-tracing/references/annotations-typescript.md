# TypeScript SDK注释模式

使用TypeScript客户端向跨度、跟踪、文档和会话添加反馈。

##客户端设置```typescript
import { createClient } from "@arizeai/phoenix-client";
const client = createClient();  // Default: http://localhost:6006
```
## Span注释

为单个跨度添加反馈：```typescript
import { addSpanAnnotation } from "@arizeai/phoenix-client/spans";

await addSpanAnnotation({
  client,
  spanAnnotation: {
    spanId: "abc123",
    name: "quality",
    annotatorKind: "HUMAN",
    label: "high_quality",
    score: 0.95,
    explanation: "Accurate and well-formatted",
    metadata: { reviewer: "alice" }
  },
  sync: true
});
```
## Span笔记

注释是针对自由格式文本的一种特殊类型的注释——对开放编码很有用，在开放编码中，审阅者在任何标题存在之前都会对一个跨度留下定性观察。之后，这些音符可以聚合并提炼成结构化的标签或分数。

注释只能追加：每次调用都会自动生成一个uidv4标识符，因此多个注释自然会在同一span上累积。结构化注释的关键字是`(name, spanId, identifier)`——通过提供不同的标识符（例如，每个审阅者一个），你可以在一个跨度上有许多同名的注释；写入相同的`(name, spanId, identifier)`将覆盖现有条目。```typescript
import { addSpanNote } from "@arizeai/phoenix-client/spans";

await addSpanNote({
  client,
  spanNote: {
    spanId: "abc123",
    note: "This span shows unexpected behavior, needs review"
  }
});
```
##文档注释

在retriver范围内对单个文档进行评级：```typescript
import { addDocumentAnnotation } from "@arizeai/phoenix-client/spans";

await addDocumentAnnotation({
  client,
  documentAnnotation: {
    spanId: "retriever_span",
    documentPosition: 0,  // 0-based index
    name: "relevance",
    annotatorKind: "LLM",
    label: "relevant",
    score: 0.95
  }
});
```
##跟踪注释

整个轨迹反馈：```typescript
import { addTraceAnnotation } from "@arizeai/phoenix-client/traces";

await addTraceAnnotation({
  client,
  traceAnnotation: {
    traceId: "trace_abc",
    name: "correctness",
    annotatorKind: "HUMAN",
    label: "correct",
    score: 1.0
  }
});
```
##跟踪笔记

整个跟踪的注释（每个跟踪允许多个注释）：```typescript
import { addTraceNote } from "@arizeai/phoenix-client/traces";

await addTraceNote({
  client,
  traceNote: {
    traceId: "abc123def456",
    note: "Needs follow-up — unexpected tool call sequence"
  }
});
```
##会话注释

多回合对话反馈：```typescript
import { addSessionAnnotation } from "@arizeai/phoenix-client/sessions";

await addSessionAnnotation({
  client,
  sessionAnnotation: {
    sessionId: "session_xyz",
    name: "user_satisfaction",
    annotatorKind: "HUMAN",
    label: "satisfied",
    score: 0.85
  }
});
```
## RAG管道示例```typescript
import { createClient } from "@arizeai/phoenix-client";
import { logDocumentAnnotations, addSpanAnnotation } from "@arizeai/phoenix-client/spans";
import { addTraceAnnotation } from "@arizeai/phoenix-client/traces";

const client = createClient();

// Document relevance (batch)
await logDocumentAnnotations({
  client,
  documentAnnotations: [
    { spanId: "retriever_span", documentPosition: 0, name: "relevance",
      annotatorKind: "LLM", label: "relevant", score: 0.95 },
    { spanId: "retriever_span", documentPosition: 1, name: "relevance",
      annotatorKind: "LLM", label: "relevant", score: 0.80 }
  ]
});

// LLM response quality
await addSpanAnnotation({
  client,
  spanAnnotation: {
    spanId: "llm_span",
    name: "faithfulness",
    annotatorKind: "LLM",
    label: "faithful",
    score: 0.90
  }
});

// Overall trace quality
await addTraceAnnotation({
  client,
  traceAnnotation: {
    traceId: "trace_123",
    name: "correctness",
    annotatorKind: "HUMAN",
    label: "correct",
    score: 1.0
  }
});
```
## API参考

- [TypeScript客户端API]（https://arize-ai.github.io/phoenix/）