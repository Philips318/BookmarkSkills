#按元数据分组文件

使用Copilot可以根据元数据智能地组织文件夹中的文件。

> **可运行示例：** [recipe/managing-local-files.ts]（recipe/managing-local-files.ts）
>
>“bash
> CD配方&& NPM安装
> NPX TSXmanaging-local-files.ts> #或：NPM运行management -local-files
> ' ' '

示例场景

您有一个包含许多文件的文件夹，并且希望根据元数据（如文件类型、创建日期、大小或其他属性）将它们组织到子文件夹中。Copilot可以分析文件并建议或执行分组策略。

##示例代码```typescript
import { CopilotClient, approveAll } from "@github/copilot-sdk";
import * as os from "node:os";
import * as path from "node:path";

// Create and start client
const client = new CopilotClient();
await client.start();

// Create session
const session = await client.createSession({
    onPermissionRequest: approveAll,
    model: "gpt-5",
});

// Event handler
session.on((event) => {
    switch (event.type) {
        case "assistant.message":
            console.log(`\nCopilot: ${event.data.content}`);
            break;
        case "tool.execution_start":
            console.log(`  → Running: ${event.data.toolName} ${event.data.toolCallId}`);
            break;
        case "tool.execution_complete":
            console.log(`  ✓ Completed: ${event.data.toolCallId}`);
            break;
    }
});

// Ask Copilot to organize files
const targetFolder = path.join(os.homedir(), "Downloads");

await session.sendAndWait({
    prompt: `
Analyze the files in "${targetFolder}" and organize them into subfolders.

1. First, list all files and their metadata
2. Preview grouping by file extension
3. Create appropriate subfolders (e.g., "images", "documents", "videos")
4. Move each file to its appropriate subfolder

Please confirm before moving any files.
`,
});

await session.destroy();
await client.stop();
```
分组策略

###通过文件扩展名```typescript
// Groups files like:
// images/   -> .jpg, .png, .gif
// documents/ -> .pdf, .docx, .txt
// videos/   -> .mp4, .avi, .mov
```
###按创建日期```typescript
// Groups files like:
// 2024-01/ -> files created in January 2024
// 2024-02/ -> files created in February 2024
```
###按文件大小```typescript
// Groups files like:
// tiny-under-1kb/
// small-under-1mb/
// medium-under-100mb/
// large-over-100mb/
```
##干式运行模式

为了安全起见，您可以要求Copilot只预览更改：```typescript
await session.sendAndWait({
    prompt: `
Analyze files in "${targetFolder}" and show me how you would organize them
by file type. DO NOT move any files - just show me the plan.
`,
});
```
使用AI分析自定义分组

让Copilot根据文件内容确定最佳分组：```typescript
await session.sendAndWait({
    prompt: `
Look at the files in "${targetFolder}" and suggest a logical organization.
Consider:
- File names and what they might contain
- File types and their typical uses
- Date patterns that might indicate projects or events

Propose folder names that are descriptive and useful.
`,
});
```
##安全考虑

1. **移动前确认**：要求副驾驶在执行移动前确认
2. **处理重复**：考虑如果存在同名的文件会发生什么
3. **保存原件**：考虑复制而不是移动重要文件