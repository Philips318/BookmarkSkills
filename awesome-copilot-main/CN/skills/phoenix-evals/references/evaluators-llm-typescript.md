# Evaluators: TypeScript中的LLM评估器

LLM评估人员使用语言模型来判断输出。使用Vercel AI SDK。

##快速入门```typescript
import { createClassificationEvaluator } from "@arizeai/phoenix-evals";
import { openai } from "@ai-sdk/openai";

const helpfulness = await createClassificationEvaluator<{
  input: string;
  output: string;
}>({
  name: "helpfulness",
  model: openai("gpt-4o"),
  promptTemplate: `Rate helpfulness.
<question>{{input}}</question>
<response>{{output}}</response>
Answer (helpful/not_helpful):`,
  choices: { not_helpful: 0, helpful: 1 },
});
```
##模板变量

使用XML标记：`<question>{{input}}</question>`、`<response>{{output}}</response>`、`<context>{{context}}</context>`使用asexperimenttevaluator自定义评估器```typescript
import { asExperimentEvaluator } from "@arizeai/phoenix-client/experiments";

const customEval = asExperimentEvaluator({
  name: "custom",
  kind: "LLM",
  evaluate: async ({ input, output }) => {
    // Your LLM call here
    return { score: 1.0, label: "pass", explanation: "..." };
  },
});
```
预构建评估器```typescript
import { createFaithfulnessEvaluator } from "@arizeai/phoenix-evals";

const faithfulnessEvaluator = createFaithfulnessEvaluator({
  model: openai("gpt-4o"),
});
```
最佳实践

-明确标准
-在提示中包含示例
-用`<thinking>`来表达思维链