# Experiments: TypeScript中的数据集

创建和管理评估数据集。

##创建数据集`createDataset()`upserts：如果已经存在具有相同名称的数据集，则更新该数据集以匹配提供的示例。使用相同的输入重新运行是无操作的。```typescript
import { createClient } from "@arizeai/phoenix-client";
import { createDataset } from "@arizeai/phoenix-client/datasets";

const client = createClient();

const { datasetId } = await createDataset({
  client,
  name: "qa-test-v1",
  examples: [
    {
      input: { question: "What is 2+2?" },
      output: { answer: "4" },
      metadata: { category: "math" },
    },
  ],
});

// With stable example IDs for targeted updates across uploads
const { datasetId } = await createDataset({
  client,
  name: "qa-test-v1",
  examples: [
    {
      id: "q-001",                        // stable ID — server updates this row, not inserts
      input: { question: "What is 2+2?" },
      output: { answer: "4" },
      metadata: { category: "math" },
    },
  ],
});
```
##示例结构```typescript
interface Example {
  input: Record<string, unknown>;    // Task input
  output?: Record<string, unknown> | null;  // Expected output
  metadata?: Record<string, unknown> | null; // Additional context
  splits?: string | string[] | null; // Split assignment ("train", ["train", "easy"], etc.)
  spanId?: string | null;            // OTEL span ID to link back to source trace
  id?: string | null;                // Stable user-provided ID; server updates matching row
}
```
##从生产轨迹```typescript
import { getSpans } from "@arizeai/phoenix-client/spans";

const { spans } = await getSpans({
  project: { projectName: "my-app" },
  parentId: null, // root spans only
  limit: 100,
});

const examples = spans.map((span) => ({
  input: { query: span.attributes?.["input.value"] },
  output: { response: span.attributes?.["output.value"] },
  metadata: { spanId: span.context.span_id },
}));

await createDataset({ client, name: "production-sample", examples });
```
##检索数据集```typescript
import { getDataset, listDatasets } from "@arizeai/phoenix-client/datasets";

const dataset = await getDataset({ client, datasetId: "..." });
const all = await listDatasets({ client });
```
最佳实践

- **默认为Upsert **：重新上传到相同的名称以更新原位；在示例中使用`id`，以便服务器针对特定的行，而不是将每次上传都视为新数据
- **Versioning**：当你想要一个干净的快照时，使用新名称的版本（例如，`qa-test-v2`），而不仅仅是增量编辑
- **元数据**：跟踪来源，类别，出处
- **类型安全**：使用`@arizeai/phoenix-client/datasets`中的`Example`类型