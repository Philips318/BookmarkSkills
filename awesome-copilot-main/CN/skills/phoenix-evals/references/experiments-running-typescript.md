# Experiments：在TypeScript中运行实验

使用`runExperiment`执行实验。

##基本用法```typescript
import { createClient } from "@arizeai/phoenix-client";
import {
  runExperiment,
  asExperimentEvaluator,
} from "@arizeai/phoenix-client/experiments";

const client = createClient();

const task = async (example: { input: Record<string, unknown> }) => {
  return await callLLM(example.input.question as string);
};

const exactMatch = asExperimentEvaluator({
  name: "exact_match",
  kind: "CODE",
  evaluate: async ({ output, expected }) => ({
    score: output === expected?.answer ? 1.0 : 0.0,
    label: output === expected?.answer ? "match" : "no_match",
  }),
});

const experiment = await runExperiment({
  client,
  experimentName: "qa-experiment-v1",
  dataset: { datasetId: "your-dataset-id" },
  task,
  evaluators: [exactMatch],
});
```
##任务函数```typescript
// Basic task
const task = async (example) => await callLLM(example.input.question as string);

// With context (RAG)
const ragTask = async (example) => {
  const prompt = `Context: ${example.input.context}\nQ: ${example.input.question}`;
  return await callLLM(prompt);
};
```
##评估器参数```typescript
interface EvaluatorParams {
  input: Record<string, unknown>;
  output: unknown;
  expected: Record<string, unknown>;
  metadata: Record<string, unknown>;
}
```
# #选项```typescript
const experiment = await runExperiment({
  client,
  experimentName: "my-experiment",
  dataset: { datasetName: "qa-test-v1" },
  task,
  evaluators,
  repetitions: 3, // Run each example 3 times
  maxConcurrency: 5, // Limit concurrent executions
});
```
# #稳定

当任务或评估器是不确定的（LLM调用、工具使用、流输出、LLM作为裁判）时，单次运行的分数是嘈杂的。在一个小数据集上，每次运行的噪声可能会淹没提示变化的信号。

重复的平均可以让你报告的分数反映提示而不是采样噪声：```typescript
await runExperiment({
  // ...
  repetitions: 3,
});
```
需要考虑的事项：

-当任务或评估器是LLM调用并且数据集很小时，需要进行重复。
-当每个样本的成本较低并且你主要想要结算分数时更喜欢重复当您还需要覆盖更多行为时，更喜欢增长数据集。
-当任务和评估器都是确定的时跳过重复（例如，与基础真理进行字符串比较）-一次运行就是答案。

考虑在以下情况下增加稳定性：

-重复运行相同的实验漂移的方式感觉比你试图测量的差异更大。
-提示式更改以不跟踪输出实际变化的方式翻转示例标签。
-法官对同一输出的推理从一次运行到下一次运行的读数不同。

重复也是`repetitions: 1`（默认）默默地依赖的—不要相信基于单个10个示例运行的调优决策。##稍后添加评估```typescript
import { evaluateExperiment } from "@arizeai/phoenix-client/experiments";

await evaluateExperiment({ client, experiment, evaluators: [newEvaluator] });
```
