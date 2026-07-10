# Evaluators: TypeScript中的代码评估器

没有LLM的确定性评估器。快速、廉价、可复制。

##基本模式```typescript
import { createEvaluator } from "@arizeai/phoenix-evals";

const containsCitation = createEvaluator<{ output: string }>(
  ({ output }) => /\[\d+\]/.test(output) ? 1 : 0,
  { name: "contains_citation", kind: "CODE" }
);
```
与完整的结果（asexperimenttevaluator）```typescript
import { asExperimentEvaluator } from "@arizeai/phoenix-client/experiments";

const jsonValid = asExperimentEvaluator({
  name: "json_valid",
  kind: "CODE",
  evaluate: async ({ output }) => {
    try {
      JSON.parse(String(output));
      return { score: 1.0, label: "valid_json" };
    } catch (e) {
      return { score: 0.0, label: "invalid_json", explanation: String(e) };
    }
  },
});
```
##参数类型```typescript
interface EvaluatorParams {
  input: Record<string, unknown>;
  output: unknown;
  expected: Record<string, unknown>;
  metadata: Record<string, unknown>;
}
```
##常见模式

—**正则表达式**:`/pattern/.test(output)`—**JSON**:`JSON.parse()`+ zod模式
- **关键词**:`output.includes(keyword)`- **相似性**:`fastest-levenshtein`