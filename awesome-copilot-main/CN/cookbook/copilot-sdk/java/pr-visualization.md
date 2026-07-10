#生成PR年龄图表

构建一个交互式CLI工具，使用Copilot的内置功能可视化GitHub存储库的拉取请求年龄分布。

> **可运行示例：** [recipe/PRVisualization.java]（recipe/PRVisualization.java）
>
>“bash
> jbangrecipe/PRVisualization.java> ' ' '

示例场景

您希望了解pr在存储库中打开了多长时间。这个工具检测当前的Git repo或接受一个repo作为输入，然后让Copilot通过GitHub MCP服务器获取PR数据并生成图表图像。

# #使用```bash
# Auto-detect from current git repo
jbang recipe/PRVisualization.java

# Specify a repo explicitly
jbang recipe/PRVisualization.java github/copilot-sdk
```
完整示例：PRVisualization.java```java
//DEPS com.github:copilot-sdk-java:0.2.1-java.1
import com.github.copilot.sdk.CopilotClient;
import com.github.copilot.sdk.events.AssistantMessageEvent;
import com.github.copilot.sdk.events.ToolExecutionStartEvent;
import com.github.copilot.sdk.json.MessageOptions;
import com.github.copilot.sdk.json.PermissionHandler;
import com.github.copilot.sdk.json.SessionConfig;
import com.github.copilot.sdk.json.SystemMessageConfig;
import java.io.BufferedReader;
import java.io.EOFException;
import java.io.IOException;
import java.io.InputStreamReader;
import java.util.regex.Pattern;

public class PRVisualization {

    public static void main(String[] args) throws Exception {
        System.out.println("🔍 PR Age Chart Generator\n");

        // Determine the repository
        String repo;
        if (args.length > 0) {
            repo = args[0];
            System.out.println("📦 Using specified repo: " + repo);
        } else if (isGitRepo()) {
            String detected = getGitHubRemote();
            if (detected != null && !detected.isEmpty()) {
                repo = detected;
                System.out.println("📦 Detected GitHub repo: " + repo);
            } else {
                System.out.println("⚠️  Git repo found but no GitHub remote detected.");
                repo = promptForRepo();
            }
        } else {
            System.out.println("📁 Not in a git repository.");
            repo = promptForRepo();
        }

        if (repo == null || !repo.contains("/")) {
            System.err.println("❌ Invalid repo format. Expected: owner/repo");
            System.exit(1);
        }

        String[] parts = repo.split("/", 2);
        String owner = parts[0];
        String repoName = parts[1];

        // Create Copilot client
        try (var client = new CopilotClient()) {
            client.start().get();

            String cwd = System.getProperty("user.dir");
            var systemMessage = String.format("""
                <context>
                You are analyzing pull requests for the GitHub repository: %s/%s
                The current working directory is: %s
                </context>

                <instructions>
                - Use the GitHub MCP Server tools to fetch PR data
                - Use your file and code execution tools to generate charts
                - Save any generated images to the current working directory
                - Be concise in your responses
                </instructions>
                """, owner, repoName, cwd);

            var session = client.createSession(
                new SessionConfig().setOnPermissionRequest(PermissionHandler.APPROVE_ALL)
                    .setModel("gpt-5")
                    .setSystemMessage(new SystemMessageConfig().setContent(systemMessage))
            ).get();

            // Set up event handling
            session.on(AssistantMessageEvent.class, msg -> 
                System.out.println("\n🤖 " + msg.getData().content() + "\n")
            );

            session.on(ToolExecutionStartEvent.class, evt -> 
                System.out.println("  ⚙️  " + evt.getData().toolName())
            );

            // Initial prompt - let Copilot figure out the details
            System.out.println("\n📊 Starting analysis...\n");

            String prompt = String.format("""
                Fetch the open pull requests for %s/%s from the last week.
                Calculate the age of each PR in days.
                Then generate a bar chart image showing the distribution of PR ages
                (group them into sensible buckets like <1 day, 1-3 days, etc.).
                Save the chart as "pr-age-chart.png" in the current directory.
                Finally, summarize the PR health - average age, oldest PR, and how many might be considered stale.
                """, owner, repoName);

            session.sendAndWait(new MessageOptions().setPrompt(prompt)).get();

            // Interactive loop
            System.out.println("\n💡 Ask follow-up questions or type \"exit\" to quit.\n");
            System.out.println("Examples:");
            System.out.println("  - \"Expand to the last month\"");
            System.out.println("  - \"Show me the 5 oldest PRs\"");
            System.out.println("  - \"Generate a pie chart instead\"");
            System.out.println("  - \"Group by author instead of age\"");
            System.out.println();

            try (var reader = new BufferedReader(new InputStreamReader(System.in))) {
                while (true) {
                    System.out.print("You: ");
                    String input = reader.readLine();
                    if (input == null) break;
                    input = input.trim();

                    if (input.isEmpty()) continue;
                    if (input.equalsIgnoreCase("exit") || input.equalsIgnoreCase("quit")) {
                        System.out.println("👋 Goodbye!");
                        break;
                    }

                    session.sendAndWait(new MessageOptions().setPrompt(input)).get();
                }
            }

            session.close();
        }
    }

    // ============================================================================
    // Git & GitHub Detection
    // ============================================================================

    private static boolean isGitRepo() {
        try {
            Process proc = Runtime.getRuntime().exec(new String[]{"git", "rev-parse", "--git-dir"});
            return proc.waitFor() == 0;
        } catch (Exception e) {
            return false;
        }
    }

    private static String getGitHubRemote() {
        try {
            Process proc = Runtime.getRuntime().exec(new String[]{"git", "remote", "get-url", "origin"});
            try (BufferedReader reader = new BufferedReader(new InputStreamReader(proc.getInputStream()))) {
                String remoteURL = reader.readLine();
                if (remoteURL == null) return null;
                remoteURL = remoteURL.trim();

                // Handle SSH: git@github.com:owner/repo.git
                var sshPattern = Pattern.compile("git@github\\.com:(.+/.+?)(?:\\.git)?$");
                var sshMatcher = sshPattern.matcher(remoteURL);
                if (sshMatcher.find()) {
                    return sshMatcher.group(1);
                }

                // Handle HTTPS: https://github.com/owner/repo.git
                var httpsPattern = Pattern.compile("https://github\\.com/(.+/.+?)(?:\\.git)?$");
                var httpsMatcher = httpsPattern.matcher(remoteURL);
                if (httpsMatcher.find()) {
                    return httpsMatcher.group(1);
                }
            }
        } catch (Exception e) {
            // Ignore
        }
        return null;
    }

    private static String promptForRepo() throws IOException {
        BufferedReader reader = new BufferedReader(new InputStreamReader(System.in));
        System.out.print("Enter GitHub repo (owner/repo): ");
        String line = reader.readLine();
        if (line == null) {
            throw new EOFException("End of input while reading repository name");
        }
        return line.trim();
    }
}
```
##它是如何工作的

1. **库检测**：检查命令行参数→git remote→提示用户
2. **没有自定义工具**：完全依赖于Copilot CLI的内置功能：
- **GitHub MCP服务器** -从GitHub获取PR数据
- **文件工具** -保存生成的图表图像
**代码执行** -生成图表使用Python/matplotlib或其他方法
3. **互动环节**：初步分析后，用户可要求调整

为什么采用这种方法？

|方面|自定义工具|内置副驾驶|| --------------- | ----------------- | --------------------------------- |
|代码复杂度|高| **最小** |
|维护|你维护| **副驾驶维护** |
|灵活性|固定逻辑| **AI决定最佳方法** |
|图表类型|你编码的| **任何类型的副驾驶都可以生成** |
|数据分组|硬编码桶| **智能分组** |

最佳实践1. **从自动检测开始**：让工具在提示用户之前从git远程检测存储库
2. **使用系统消息**：提供关于repo和工作目录的上下文，以便Copilot可以自主操作
3. **批准工具执行**：使用`PermissionHandler.APPROVE_ALL`允许Copilot运行工具，如GitHub MCP服务器，无需手动批准
4. **交互式跟踪**：让用户通过对话来完善分析，而不需要重新启动
5. **本地保存工件**：直接Copilot将生成的图表保存到当前目录，以便于访问