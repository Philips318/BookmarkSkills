#生成PR年龄图表

构建一个交互式CLI工具，使用Copilot的内置功能可视化GitHub存储库的拉取请求年龄分布。

> **可运行示例：** [recipe/pr-visualization.ts]（recipe/pr-visualization.ts）
>
>“bash
> CD配方&& NPM安装
> #从当前git仓库自动检测
> NPX TSXpr-visualization.ts>
> #明确指定一个回购pr-visualization.ts——回购github/copilot-sdk> #或：NPM运行pr-可视化
> ' ' '

示例场景

您希望了解pr在存储库中打开了多长时间。这个工具检测当前的Git repo或接受一个repo作为输入，然后让Copilot通过GitHub MCP服务器获取PR数据并生成图表图像。

# #先决条件```bash
npm install @github/copilot-sdk
npm install -D typescript tsx @types/node
```
# #使用```bash
# Auto-detect from current git repo
npx tsx pr-visualization.ts

# Specify a repo explicitly
npx tsx pr-visualization.ts --repo github/copilot-sdk
```
完整示例：pr-visualization.ts```typescript
#!/usr/bin/env npx tsx

import { execSync } from "node:child_process";
import * as readline from "node:readline";
import { CopilotClient, approveAll } from "@github/copilot-sdk";

// ============================================================================
// Git & GitHub Detection
// ============================================================================

function isGitRepo(): boolean {
    try {
        execSync("git rev-parse --git-dir", { stdio: "ignore" });
        return true;
    } catch {
        return false;
    }
}

function getGitHubRemote(): string | null {
    try {
        const remoteUrl = execSync("git remote get-url origin", {
            encoding: "utf-8",
        }).trim();

        // Handle SSH: git@github.com:owner/repo.git
        const sshMatch = remoteUrl.match(/git@github\.com:(.+\/.+?)(?:\.git)?$/);
        if (sshMatch) return sshMatch[1];

        // Handle HTTPS: https://github.com/owner/repo.git
        const httpsMatch = remoteUrl.match(/https:\/\/github\.com\/(.+\/.+?)(?:\.git)?$/);
        if (httpsMatch) return httpsMatch[1];

        return null;
    } catch {
        return null;
    }
}

function parseArgs(): { repo?: string } {
    const args = process.argv.slice(2);
    const repoIndex = args.indexOf("--repo");
    if (repoIndex !== -1 && args[repoIndex + 1]) {
        return { repo: args[repoIndex + 1] };
    }
    return {};
}

async function promptForRepo(): Promise<string> {
    const rl = readline.createInterface({
        input: process.stdin,
        output: process.stdout,
    });
    return new Promise((resolve) => {
        rl.question("Enter GitHub repo (owner/repo): ", (answer) => {
            rl.close();
            resolve(answer.trim());
        });
    });
}

// ============================================================================
// Main Application
// ============================================================================

async function main() {
    console.log("🔍 PR Age Chart Generator\n");

    // Determine the repository
    const args = parseArgs();
    let repo: string;

    if (args.repo) {
        repo = args.repo;
        console.log(`📦 Using specified repo: ${repo}`);
    } else if (isGitRepo()) {
        const detected = getGitHubRemote();
        if (detected) {
            repo = detected;
            console.log(`📦 Detected GitHub repo: ${repo}`);
        } else {
            console.log("⚠️  Git repo found but no GitHub remote detected.");
            repo = await promptForRepo();
        }
    } else {
        console.log("📁 Not in a git repository.");
        repo = await promptForRepo();
    }

    if (!repo || !repo.includes("/")) {
        console.error("❌ Invalid repo format. Expected: owner/repo");
        process.exit(1);
    }

    const [owner, repoName] = repo.split("/");

    // Create Copilot client - no custom tools needed!
    const client = new CopilotClient({ logLevel: "error" });

    const session = await client.createSession({
        onPermissionRequest: approveAll,
        model: "gpt-5",
        systemMessage: {
            content: `
<context>
You are analyzing pull requests for the GitHub repository: ${owner}/${repoName}
The current working directory is: ${process.cwd()}
</context>

<instructions>
- Use the GitHub MCP Server tools to fetch PR data
- Use your file and code execution tools to generate charts
- Save any generated images to the current working directory
- Be concise in your responses
</instructions>
`,
        },
    });

    // Set up event handling
    const rl = readline.createInterface({
        input: process.stdin,
        output: process.stdout,
    });

    session.on((event) => {
        if (event.type === "assistant.message") {
            console.log(`\n🤖 ${event.data.content}\n`);
        } else if (event.type === "tool.execution_start") {
            console.log(`  ⚙️  ${event.data.toolName}`);
        }
    });

    // Initial prompt - let Copilot figure out the details
    console.log("\n📊 Starting analysis...\n");

    await session.sendAndWait({
        prompt: `
      Fetch the open pull requests for ${owner}/${repoName} from the last week.
      Calculate the age of each PR in days.
      Then generate a bar chart image showing the distribution of PR ages
      (group them into sensible buckets like <1 day, 1-3 days, etc.).
      Save the chart as "pr-age-chart.png" in the current directory.
      Finally, summarize the PR health - average age, oldest PR, and how many might be considered stale.
    `,
    });

    // Interactive loop
    const askQuestion = () => {
        rl.question("You: ", async (input) => {
            const trimmed = input.trim();

            if (trimmed.toLowerCase() === "exit" || trimmed.toLowerCase() === "quit") {
                console.log("👋 Goodbye!");
                rl.close();
                await session.destroy();
                await client.stop();
                process.exit(0);
            }

            if (trimmed) {
                await session.sendAndWait({ prompt: trimmed });
            }

            askQuestion();
        });
    };

    console.log('💡 Ask follow-up questions or type "exit" to quit.\n');
    console.log("Examples:");
    console.log('  - "Expand to the last month"');
    console.log('  - "Show me the 5 oldest PRs"');
    console.log('  - "Generate a pie chart instead"');
    console.log('  - "Group by author instead of age"');
    console.log("");

    askQuestion();
}

main().catch(console.error);
```
##它是如何工作的

1. **库检测**：检查`--repo`标志→git remote→提示用户
2. **没有自定义工具**：完全依赖于Copilot CLI的内置功能：    - **GitHub MCP Server** - Fetches PR data from GitHub
    - **File tools** - Saves generated chart images
    - **Code execution** - Generates charts using Python/matplotlib or other methods
3. **互动环节**：初步分析后，用户可要求调整

##示例交互```
🔍 PR Age Chart Generator

📦 Using specified repo: CommunityToolkit/Aspire

📊 Starting analysis...

  ⚙️  github-mcp-server-list_pull_requests
  ⚙️  powershell

🤖 I've analyzed 23 open PRs for CommunityToolkit/Aspire:

**PR Age Distribution:**
- < 1 day: 3 PRs
- 1-3 days: 5 PRs
- 3-7 days: 8 PRs
- 1-2 weeks: 4 PRs
- > 2 weeks: 3 PRs

**Summary:**
- Average age: 6.2 days
- Oldest: PR #142 (18 days) - "Add Redis caching support"
- Potentially stale (>7 days): 7 PRs

Chart saved to: pr-age-chart.png

💡 Ask follow-up questions or type "exit" to quit.

You: Expand to the last month and show by author

  ⚙️  github-mcp-server-list_pull_requests
  ⚙️  powershell

🤖 Updated analysis for the last 30 days, grouped by author:

| Author        | Open PRs | Avg Age |
|---------------|----------|---------|
| @contributor1 | 5        | 12 days |
| @contributor2 | 3        | 4 days  |
| @contributor3 | 2        | 8 days  |
| ...           |          |         |

New chart saved to: pr-age-chart.png

You: Generate a pie chart showing the age distribution

  ⚙️  powershell

🤖 Done! Pie chart saved to: pr-age-chart.png
```
为什么采用这种方法？

|方面|自定义工具|内置副驾驶|| --------------- | ----------------- | --------------------------------- |
|代码复杂度|高| **最小** |
|维护|你维护| **副驾驶维护** |
|灵活性|固定逻辑| **AI决定最佳方法** |
|图表类型|你编码的| **任何类型的副驾驶都可以生成** |
|数据分组|硬编码桶| **智能分组** |