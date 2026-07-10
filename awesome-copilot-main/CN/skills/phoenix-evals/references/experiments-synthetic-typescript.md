# Experiments：生成合成测试数据（TypeScript）

为评估创建多样化的、有针对性的测试数据。

基于维度的方法

定义变化轴，然后生成组合：```typescript
const dimensions = {
  issueType: ["billing", "technical", "shipping"],
  customerMood: ["frustrated", "neutral", "happy"],
  complexity: ["simple", "moderate", "complex"],
};
```
两步生成

1. **生成元组**（维度值的组合）
2. **转换为自然查询**（每个元组单独的LLM调用）```typescript
import { generateText } from "ai";
import { openai } from "@ai-sdk/openai";

// Step 1: Create tuples
type Tuple = [string, string, string];
const tuples: Tuple[] = [
  ["billing", "frustrated", "complex"],
  ["shipping", "neutral", "simple"],
];

// Step 2: Convert to natural query
async function tupleToQuery(t: Tuple): Promise<string> {
  const { text } = await generateText({
    model: openai("gpt-4o"),
    prompt: `Generate a realistic customer message:
    Issue: ${t[0]}, Mood: ${t[1]}, Complexity: ${t[2]}
    
    Write naturally, include typos if appropriate. Don't be formulaic.`,
  });
  return text;
}
```
##目标失效模式

维度应该针对错误分析中的已知故障：```typescript
// From error analysis findings
const dimensions = {
  timezone: ["EST", "PST", "UTC", "ambiguous"], // Known failure
  dateFormat: ["ISO", "US", "EU", "relative"], // Known failure
};
```
##质量控制

- **Validate**：检查占位符文本，最小长度
- **重复数据删除**：使用嵌入删除接近重复的查询
- **平衡**：确保跨维度值的覆盖```typescript
function validateQuery(query: string): boolean {
  const minLength = 20;
  const hasPlaceholder = /\[.*?\]|<.*?>/.test(query);
  return query.length >= minLength && !hasPlaceholder;
}
```
##何时使用

使用合成|使用真实数据|| ------------- | ------------- |
|生产数据有限|充足追踪|
|测试边缘情况|验证实际行为|
|启动前评估|启动后监控|

##样本大小

|用途|大小|| ------- | ---- |
初步勘探| 50-100 |
|综合评估| 100-500 |
每维|每组合| 10-20