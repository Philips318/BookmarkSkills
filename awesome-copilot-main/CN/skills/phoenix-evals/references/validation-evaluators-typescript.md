#验证求值器（TypeScript）

在部署LLM评估器之前，根据人工标记的示例验证它。
目标：** b> 80% TPR和b> 80% TNR**。

与正常任务实验相比，角色倒置：

|正常实验|评估者验证||---|---|
| Task =代理逻辑| Task =运行测试|下的评估器
|评估器=判断输出|评估器=精确匹配与人类地面真相|
|数据集= agent样例|数据集=金色手工标记样例|

##黄金数据集

使用单独的数据集名称，这样验证实验就不会与Phoenix中的任务实验混在一起。
在`metadata.groundTruthLabel`中存储人类地面真相。目标为~50/50平衡：```typescript
import type { Example } from "@arizeai/phoenix-client/types/datasets";

const goldenExamples: Example[] = [
  { input: { q: "Capital of France?" }, output: { answer: "Paris" },       metadata: { groundTruthLabel: "correct" } },
  { input: { q: "Capital of France?" }, output: { answer: "Lyon" },        metadata: { groundTruthLabel: "incorrect" } },
  { input: { q: "Capital of France?" }, output: { answer: "Major city..." }, metadata: { groundTruthLabel: "incorrect" } },
];

const VALIDATOR_DATASET = "my-app-qa-evaluator-validation"; // separate from task dataset
const POSITIVE_LABEL = "correct";
const NEGATIVE_LABEL = "incorrect";
```
验证实验```typescript
import { createClient } from "@arizeai/phoenix-client";
import { createOrGetDataset, getDatasetExamples } from "@arizeai/phoenix-client/datasets";
import { asExperimentEvaluator, runExperiment } from "@arizeai/phoenix-client/experiments";
import { myEvaluator } from "./myEvaluator.js";

const client = createClient();

const { datasetId } = await createOrGetDataset({ client, name: VALIDATOR_DATASET, examples: goldenExamples });
const { examples } = await getDatasetExamples({ client, dataset: { datasetId } });
const groundTruth = new Map(examples.map((ex) => [ex.id, ex.metadata?.groundTruthLabel as string]));

// Task: invoke the evaluator under test
const task = async (example: (typeof examples)[number]) => {
  const result = await myEvaluator.evaluate({ input: example.input, output: example.output, metadata: example.metadata });
  return result.label ?? "unknown";
};

// Evaluator: exact-match against human ground truth
const exactMatch = asExperimentEvaluator({
  name: "exact-match", kind: "CODE",
  evaluate: ({ output, metadata }) => {
    const expected = metadata?.groundTruthLabel as string;
    const predicted = typeof output === "string" ? output : "unknown";
    return { score: predicted === expected ? 1 : 0, label: predicted, explanation: `Expected: ${expected}, Got: ${predicted}` };
  },
});

const experiment = await runExperiment({
  client, experimentName: `evaluator-validation-${Date.now()}`,
  dataset: { datasetId }, task, evaluators: [exactMatch],
});

// Compute confusion matrix
const runs = Object.values(experiment.runs);
const predicted = new Map((experiment.evaluationRuns ?? [])
  .filter((e) => e.name === "exact-match")
  .map((e) => [e.experimentRunId, e.result?.label ?? null]));

let tp = 0, fp = 0, tn = 0, fn = 0;
for (const run of runs) {
  if (run.error) continue;
  const p = predicted.get(run.id), a = groundTruth.get(run.datasetExampleId);
  if (!p || !a) continue;
  if (a === POSITIVE_LABEL && p === POSITIVE_LABEL) tp++;
  else if (a === NEGATIVE_LABEL && p === POSITIVE_LABEL) fp++;
  else if (a === NEGATIVE_LABEL && p === NEGATIVE_LABEL) tn++;
  else if (a === POSITIVE_LABEL && p === NEGATIVE_LABEL) fn++;
}
const total = tp + fp + tn + fn;
const tpr = tp + fn > 0 ? (tp / (tp + fn)) * 100 : 0;
const tnr = tn + fp > 0 ? (tn / (tn + fp)) * 100 : 0;
console.log(`TPR: ${tpr.toFixed(1)}%  TNR: ${tnr.toFixed(1)}%  Accuracy: ${((tp + tn) / total * 100).toFixed(1)}%`);
```
结果和质量规则

|指标|目标|低值表示||---|---|---|
| TPR（灵敏度）| >80% |错过真实故障（假阴性）|
| TNR（特异性）| >80% |标记良好输出（假阳性）|
|准确度| >80% |一般弱点|

**黄金数据集规则：** ~50/50平衡·包括边缘情况·仅人工标记·永不突变（附加新版本）·20-50个示例就足够了。

**重新验证当：**提示模板更改·判断模型更改·标准更新·生产FP/FN峰值。

##参见Also

-`validation.md`-度量定义和概念
-`experiments-running-typescript.md`-`runExperiment`API
-`experiments-datasets-typescript.md`-`createOrGetDataset`/`getDatasetExamples`