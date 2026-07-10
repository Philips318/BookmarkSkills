Ralph Loop：自主AI任务循环

构建自主的编码循环，其中AI代理选择任务，实现它们，对反压力进行验证（测试，构建），提交和重复-每次迭代都在一个新的上下文窗口中。

> **可运行示例：** [recipe/ralph-loop.cs]（recipe/ralph-loop.cs）
>
>“bash
> CD dotnet
运行recipe/ralph-loop.cs> ' ' '

什么是Ralph Loop？

[Ralph loop]（https://ghuntley.com/ralph/）是一种自主开发工作流，其中AI代理在孤立的上下文窗口中迭代任务。关键洞察：**状态驻留在磁盘上，而不是在模型的上下文中**。每次迭代都重新开始，从文件中读取当前状态，执行一个任务，将结果写回磁盘，然后退出。```
┌─────────────────────────────────────────────────┐
│                   loop.sh                       │
│  while true:                                    │
│    ┌─────────────────────────────────────────┐  │
│    │  Fresh session (isolated context)       │  │
│    │                                         │  │
│    │  1. Read PROMPT.md + AGENTS.md          │  │
│    │  2. Study specs/* and code              │  │
│    │  3. Pick next task from plan            │  │
│    │  4. Implement + run tests               │  │
│    │  5. Update plan, commit, exit           │  │
│    └─────────────────────────────────────────┘  │
│    ↻ next iteration (fresh context)             │
└─────────────────────────────────────────────────┘
```
核心原则:* * * *

- **每次迭代的新上下文**：每个循环创建一个新的会话-没有上下文积累，总是在“智能区域”
—**磁盘作为共享状态**:`IMPLEMENTATION_PLAN.md`在迭代之间持续存在，并作为协调机制
- **背压控制质量**：测试、构建和lint拒绝不良工作——代理必须在提交之前修复问题
- **两种模式**:PLANNING（差距分析→生成计划）和BUILDING（根据计划实施）

##简单版

最小的Ralph循环——相当于SDK中的`while :; do cat PROMPT.md | copilot ; done`：```csharp
using GitHub.Copilot;

var client = new CopilotClient();
await client.StartAsync();

try
{
    var prompt = await File.ReadAllTextAsync("PROMPT.md");
    var maxIterations = 50;

    for (var i = 1; i <= maxIterations; i++)
    {
        Console.WriteLine($"\n=== Iteration {i}/{maxIterations} ===");

        // Fresh session each iteration — context isolation is the point
        var session = await client.CreateSessionAsync(
            new SessionConfig
            {
                Model = "gpt-5.1-codex-mini",
                OnPermissionRequest = PermissionHandler.ApproveAll
            });
        try
        {
            var done = new TaskCompletionSource<string>();
            session.On(evt =>
            {
                if (evt is AssistantMessageEvent msg)
                    done.TrySetResult(msg.Data.Content);
            });

            await session.SendAsync(new MessageOptions { Prompt = prompt });
            await done.Task;
        }
        finally
        {
            await session.DisposeAsync();
        }

        Console.WriteLine($"Iteration {i} complete.");
    }
}
finally
{
    await client.StopAsync();
}
```
这是所有你需要的开始。提示文件告诉代理该做什么；代理读取项目文件、执行工作、提交和退出。这个循环从头开始。

##理想版本

完整的拉尔夫模式与规划和建筑模式，匹配[拉尔夫Playbook]（https://github.com/ClaytonFarr/ralph-playbook）架构：```csharp
using GitHub.Copilot;

// Parse args: dotnet run [plan] [max_iterations]
var mode = args.Contains("plan") ? "plan" : "build";
var maxArg = args.FirstOrDefault(a => int.TryParse(a, out _));
var maxIterations = maxArg != null ? int.Parse(maxArg) : 50;
var promptFile = mode == "plan" ? "PROMPT_plan.md" : "PROMPT_build.md";

var client = new CopilotClient();
await client.StartAsync();

Console.WriteLine(new string('━', 40));
Console.WriteLine($"Mode:   {mode}");
Console.WriteLine($"Prompt: {promptFile}");
Console.WriteLine($"Max:    {maxIterations} iterations");
Console.WriteLine(new string('━', 40));

try
{
    var prompt = await File.ReadAllTextAsync(promptFile);

    for (var i = 1; i <= maxIterations; i++)
    {
        Console.WriteLine($"\n=== Iteration {i}/{maxIterations} ===");

        // Fresh session — each task gets full context budget
        var session = await client.CreateSessionAsync(
            new SessionConfig
            {
                Model = "gpt-5.1-codex-mini",
                // Pin the agent to the project directory
                WorkingDirectory = Environment.CurrentDirectory,
                // Auto-approve tool calls for unattended operation
                OnPermissionRequest = PermissionHandler.ApproveAll,
            });
        try
        {
            var done = new TaskCompletionSource<string>();
            session.On(evt =>
            {
                // Log tool usage for visibility
                if (evt is ToolExecutionStartEvent toolStart)
                    Console.WriteLine($"  ⚙ {toolStart.Data.ToolName}");
                else if (evt is AssistantMessageEvent msg)
                    done.TrySetResult(msg.Data.Content);
            });

            await session.SendAsync(new MessageOptions { Prompt = prompt });
            await done.Task;
        }
        finally
        {
            await session.DisposeAsync();
        }

        Console.WriteLine($"\nIteration {i} complete.");
    }

    Console.WriteLine($"\nReached max iterations: {maxIterations}");
}
finally
{
    await client.StopAsync();
}
```
所需的项目文件

理想的版本在你的项目中需要这样的文件结构：```
project-root/
├── PROMPT_plan.md              # Planning mode instructions
├── PROMPT_build.md             # Building mode instructions
├── AGENTS.md                   # Operational guide (build/test commands)
├── IMPLEMENTATION_PLAN.md      # Task list (generated by planning mode)
├── specs/                      # Requirement specs (one per topic)
│   ├── auth.md
│   └── data-pipeline.md
└── src/                        # Your source code
```
示例`PROMPT_plan.md````markdown
0a. Study `specs/*` to learn the application specifications.
0b. Study IMPLEMENTATION_PLAN.md (if present) to understand the plan so far.
0c. Study `src/` to understand existing code and shared utilities.

1. Compare specs against code (gap analysis). Create or update
   IMPLEMENTATION_PLAN.md as a prioritized bullet-point list of tasks
   yet to be implemented. Do NOT implement anything.

IMPORTANT: Do NOT assume functionality is missing — search the
codebase first to confirm. Prefer updating existing utilities over
creating ad-hoc copies.
```
示例`PROMPT_build.md````markdown
0a. Study `specs/*` to learn the application specifications.
0b. Study IMPLEMENTATION_PLAN.md.
0c. Study `src/` for reference.

1. Choose the most important item from IMPLEMENTATION_PLAN.md. Before
   making changes, search the codebase (don't assume not implemented).
2. After implementing, run the tests. If functionality is missing, add it.
3. When you discover issues, update IMPLEMENTATION_PLAN.md immediately.
4. When tests pass, update IMPLEMENTATION_PLAN.md, then `git add -A`
   then `git commit` with a descriptive message.

5. When authoring documentation, capture the why.
6. Implement completely. No placeholders or stubs.
7. Keep IMPLEMENTATION_PLAN.md current — future iterations depend on it.
```
示例`AGENTS.md`尽量简短（60行左右）。每次迭代都会加载它，所以臃肿浪费了上下文。```markdown
## Build & Run

dotnet build

## Validation

- Tests: `dotnet test`
- Build: `dotnet build --no-restore`
```
最佳实践1. **每次迭代的新上下文**：永远不要在迭代中积累上下文——这就是关键所在
2. **磁盘是您的数据库**:`IMPLEMENTATION_PLAN.md`是隔离会话之间的共享状态
3. **反压力是必不可少的**:`AGENTS.md`中的测试、构建、测试——代理必须在提交之前通过它们
4. **从PLANNING模式开始**：先生成计划，然后切换到BUILDING模式
5. **观察和调优**：观察早期迭代，在代理以特定方式失败时为提示添加护栏
6. **计划是一次性的**：如果座席偏离轨道，删除`IMPLEMENTATION_PLAN.md`，重新规划
7. **保持`AGENTS.md`简短**：它在每次迭代中加载-只有操作信息，没有进度记录
8. **使用沙箱**：代理在完全工具访问下自主运行-隔离它
9. **设置`WorkingDirectory`**：将会话固定到您的项目根，以便工具操作正确解析路径
10. **自动审批权限**：使用`OnPermissionRequest`允许工具调用而不中断循环何时使用Ralph Loop

* *好:* *

-通过测试驱动验证实现规范中的功能
-大型重构分解成许多小任务
-有明确需求的无人值守、长期运行的开发
-任何背压（tests/builds）可以验证正确性的工作

**不适合**

-需要人工判断的任务
-不能从迭代中获益的一次性操作
-没有可测试验收标准的模糊需求
-方向不明确的探索性原型

##参见Also

—[错误处理]（error-handling.md）—长时间运行会话的超时模式和优雅关闭
—[持久化会话]（persisting-sessions.md）—保存和恢复会话跨重启