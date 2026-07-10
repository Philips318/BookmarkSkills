#按元数据分组文件

使用Copilot可以根据元数据智能地组织文件夹中的文件。

> **可运行示例：** [recipe/ManagingLocalFiles.java]（recipe/ManagingLocalFiles.java）
>
>“bash
> jbangrecipe/ManagingLocalFiles.java> ' ' '

示例场景

您有一个包含许多文件的文件夹，并且希望根据元数据（如文件类型、创建日期、大小或其他属性）将它们组织到子文件夹中。Copilot可以分析文件并建议或执行分组策略。

##示例代码

* *用法:* *```bash
# Use with a specific folder (recommended)
jbang recipe/ManagingLocalFiles.java /path/to/your/folder

# Or run without arguments to use a safe default (temp directory)
jbang recipe/ManagingLocalFiles.java
```
* *代码:* *```java
//DEPS com.github:copilot-sdk-java:0.2.1-java.1
import com.github.copilot.sdk.CopilotClient;
import com.github.copilot.sdk.events.AssistantMessageEvent;
import com.github.copilot.sdk.events.SessionIdleEvent;
import com.github.copilot.sdk.events.ToolExecutionCompleteEvent;
import com.github.copilot.sdk.events.ToolExecutionStartEvent;
import com.github.copilot.sdk.json.MessageOptions;
import com.github.copilot.sdk.json.PermissionHandler;
import com.github.copilot.sdk.json.SessionConfig;
import java.nio.file.Paths;
import java.util.concurrent.CountDownLatch;

public class ManagingLocalFiles {
    public static void main(String[] args) throws Exception {
        try (var client = new CopilotClient()) {
            client.start().get();

            // Create session
            var session = client.createSession(
                new SessionConfig().setOnPermissionRequest(PermissionHandler.APPROVE_ALL).setModel("gpt-5")).get();

            // Set up event handlers
            var done = new CountDownLatch(1);

            session.on(AssistantMessageEvent.class, msg -> 
                System.out.println("\nCopilot: " + msg.getData().content())
            );

            session.on(ToolExecutionStartEvent.class, evt -> 
                System.out.println("  → Running: " + evt.getData().toolName())
            );

            session.on(ToolExecutionCompleteEvent.class, evt -> 
                System.out.println("  ✓ Completed: " + evt.getData().toolCallId())
            );

            session.on(SessionIdleEvent.class, evt -> done.countDown());

            // Ask Copilot to organize files - using a safe example folder
            // For real use, replace with your target folder
            String targetFolder = args.length > 0 ? args[0] : 
                System.getProperty("java.io.tmpdir") + "/example-files";

            String prompt = String.format("""
                Analyze the files in "%s" and show how you would organize them into subfolders.

                1. First, list all files and their metadata
                2. Preview grouping by file extension
                3. Suggest appropriate subfolders (e.g., "images", "documents", "videos")
                
                IMPORTANT: DO NOT move any files. Only show the plan.
                """, targetFolder);

            session.send(new MessageOptions().setPrompt(prompt));

            // Wait for completion
            done.await();

            session.close();
        }
    }
}
```
分组策略

###通过文件扩展名```java
// Groups files like:
// images/   -> .jpg, .png, .gif
// documents/ -> .pdf, .docx, .txt
// videos/   -> .mp4, .avi, .mov
```
###按创建日期```java
// Groups files like:
// 2024-01/ -> files created in January 2024
// 2024-02/ -> files created in February 2024
```
###按文件大小```java
// Groups files like:
// tiny-under-1kb/
// small-under-1mb/
// medium-under-100mb/
// large-over-100mb/
```
##干式运行模式

为了安全起见，您可以要求Copilot只预览更改：```java
String prompt = String.format("""
    Analyze files in "%s" and show me how you would organize them
    by file type. DO NOT move any files - just show me the plan.
    """, targetFolder);

session.send(new MessageOptions().setPrompt(prompt));
```
使用AI分析自定义分组

让Copilot根据文件内容确定最佳分组：```java
String prompt = String.format("""
    Look at the files in "%s" and suggest a logical organization.
    Consider:
    - File names and what they might contain
    - File types and their typical uses
    - Date patterns that might indicate projects or events

    Propose folder names that are descriptive and useful.
    """, targetFolder);

session.send(new MessageOptions().setPrompt(prompt));
```
交互式文件组织```java
//DEPS com.github:copilot-sdk-java:0.2.1-java.1
import com.github.copilot.sdk.CopilotClient;
import com.github.copilot.sdk.events.AssistantMessageEvent;
import com.github.copilot.sdk.json.MessageOptions;
import com.github.copilot.sdk.json.PermissionHandler;
import com.github.copilot.sdk.json.SessionConfig;
import java.io.BufferedReader;
import java.io.InputStreamReader;

public class InteractiveFileOrganizer {
    public static void main(String[] args) throws Exception {
        try (var client = new CopilotClient();
             var reader = new BufferedReader(new InputStreamReader(System.in))) {
            
            client.start().get();

            var session = client.createSession(
                new SessionConfig().setOnPermissionRequest(PermissionHandler.APPROVE_ALL).setModel("gpt-5")).get();

            session.on(AssistantMessageEvent.class, msg -> 
                System.out.println("\nCopilot: " + msg.getData().content())
            );

            System.out.print("Enter folder path to organize: ");
            String folderPath = reader.readLine();

            String initialPrompt = String.format("""
                Analyze the files in "%s" and suggest an organization strategy.
                Wait for my confirmation before making any changes.
                """, folderPath);

            session.send(new MessageOptions().setPrompt(initialPrompt));

            // Interactive loop
            System.out.println("\nEnter commands (or 'exit' to quit):");
            String line;
            while ((line = reader.readLine()) != null) {
                if (line.equalsIgnoreCase("exit")) {
                    break;
                }
                session.send(new MessageOptions().setPrompt(line));
            }

            session.close();
        }
    }
}
```
##安全考虑

1. **移动前确认**：要求副驾驶在执行移动前确认
2. **处理重复**：考虑如果存在同名的文件会发生什么
3. **保存原件**：考虑复制而不是移动重要文件
4. **用试运行测试**：总是先用试运行测试来预览更改