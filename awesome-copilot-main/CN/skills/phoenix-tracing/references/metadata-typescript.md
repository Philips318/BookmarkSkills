# Phoenix Tracing：自定义元数据（TypeScript）

为跨度添加自定义属性以获得更丰富的可观察性。

##使用上下文（传播到所有子跨度）```typescript
import { context } from "@arizeai/phoenix-otel";
import { setMetadata } from "@arizeai/openinference-core";

context.with(
  setMetadata(context.active(), {
    experiment_id: "exp_123",
    model_version: "gpt-4-1106-preview",
    environment: "production",
  }),
  async () => {
    // All spans created within this block will have:
    // "metadata" = '{"experiment_id": "exp_123", ...}'
    await myApp.run(query);
  }
);
```
##在单个Span上```typescript
import { traceChain } from "@arizeai/openinference-core";
import { trace } from "@arizeai/phoenix-otel";

const myFunction = traceChain(
  async (input: string) => {
    const span = trace.getActiveSpan();

    span?.setAttribute(
      "metadata",
      JSON.stringify({
        experiment_id: "exp_123",
        model_version: "gpt-4-1106-preview",
        environment: "production",
      })
    );

    return result;
  },
  { name: "my-function" }
);

await myFunction("hello");
```
