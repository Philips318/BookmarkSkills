#错误处理模式

在您的Copilot SDK应用程序中优雅地处理错误。

> **可运行示例：** [recipe/error-handling.ts]（recipe/error-handling.ts）
>
>“bash
> CD配方&& NPM安装
> NPX TSXerror-handling.ts> #或：NPM运行错误处理
> ' ' '

示例场景

您需要处理各种错误情况，如连接失败、超时和无效响应。

##基本的试接```typescript
import { CopilotClient, approveAll } from "@github/copilot-sdk";

const client = new CopilotClient();

try {
    await client.start();
    const session = await client.createSession({
        onPermissionRequest: approveAll,
        model: "gpt-5",
    });

    const response = await session.sendAndWait({ prompt: "Hello!" });
    console.log(response?.data.content);

    await session.destroy();
} catch (error) {
    console.error("Error:", error.message);
} finally {
    await client.stop();
}
```
处理特定的错误类型```typescript
try {
    await client.start();
} catch (error) {
    if (error.message.includes("ENOENT")) {
        console.error("Copilot CLI not found. Please install it first.");
    } else if (error.message.includes("ECONNREFUSED")) {
        console.error("Could not connect to Copilot CLI server.");
    } else {
        console.error("Unexpected error:", error.message);
    }
}
```
##超时处理```typescript
const session = await client.createSession({
    onPermissionRequest: approveAll,
    model: "gpt-5",
});

try {
    // sendAndWait with timeout (in milliseconds)
    const response = await session.sendAndWait(
        { prompt: "Complex question..." },
        30000 // 30 second timeout
    );

    if (response) {
        console.log(response.data.content);
    } else {
        console.log("No response received");
    }
} catch (error) {
    if (error.message.includes("timeout")) {
        console.error("Request timed out");
    }
}
```
##终止请求```typescript
const session = await client.createSession({
    onPermissionRequest: approveAll,
    model: "gpt-5",
});

// Start a request
session.send({ prompt: "Write a very long story..." });

// Abort it after some condition
setTimeout(async () => {
    await session.abort();
    console.log("Request aborted");
}, 5000);
```
##安全关机```typescript
process.on("SIGINT", async () => {
    console.log("Shutting down...");

    const errors = await client.stop();
    if (errors.length > 0) {
        console.error("Cleanup errors:", errors);
    }

    process.exit(0);
});
```
##强制停止```typescript
// If stop() takes too long, force stop
const stopPromise = client.stop();
const timeout = new Promise((_, reject) => setTimeout(() => reject(new Error("Timeout")), 5000));

try {
    await Promise.race([stopPromise, timeout]);
} catch {
    console.log("Forcing stop...");
    await client.forceStop();
}
```
最佳实践

1. **总是清理**：使用try-finally来确保`client.stop()`被调用
2. **处理连接错误**:CLI可能未安装或未运行
3. **设置适当的超时时间**：长时间运行的请求应该有超时时间
4. **日志错误**：捕获错误细节以便调试