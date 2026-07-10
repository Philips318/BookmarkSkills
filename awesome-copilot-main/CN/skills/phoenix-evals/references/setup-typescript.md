# Setup: TypeScript

凤凰评估和实验所需的包。

# #安装```bash
# Using npm
npm install @arizeai/phoenix-client @arizeai/phoenix-evals @arizeai/phoenix-otel

# Using pnpm
pnpm add @arizeai/phoenix-client @arizeai/phoenix-evals @arizeai/phoenix-otel
```
LLM提供商

对于作为法官的llm评估器，安装Vercel AI SDK提供商：```bash
npm install ai @ai-sdk/openai      # Vercel AI SDK + OpenAI
npm install @ai-sdk/anthropic      # Anthropic
npm install @ai-sdk/google         # Google
```
或者使用直接提供的sdk：```bash
npm install openai                 # OpenAI direct
npm install @anthropic-ai/sdk      # Anthropic direct
```
##快速验证```typescript
import { createClient } from "@arizeai/phoenix-client";
import { createClassificationEvaluator } from "@arizeai/phoenix-evals";
import { registerPhoenix } from "@arizeai/phoenix-otel";

// All imports should work
console.log("Phoenix TypeScript setup complete");
```
