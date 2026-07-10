#会话持久和恢复

跨应用程序重新启动保存和恢复会话会话。

> **可运行示例：** [recipe/PersistingSessions.java]（recipe/PersistingSessions.java）
>
>“bash
> jbangrecipe/PersistingSessions.java> ' ' '

示例场景

您希望用户能够在关闭并重新打开应用程序后继续对话。Copilot SDK将会话状态自动保存到磁盘-您只需要提供一个稳定的会话ID并稍后恢复。

##使用自定义ID创建会话```java
//DEPS com.github:copilot-sdk-java:0.2.1-java.1
import com.github.copilot.sdk.CopilotClient;
import com.github.copilot.sdk.events.AssistantMessageEvent;
import com.github.copilot.sdk.json.MessageOptions;
import com.github.copilot.sdk.json.PermissionHandler;
import com.github.copilot.sdk.json.SessionConfig;

public class CreateSessionWithId {
    public static void main(String[] args) throws Exception {
        try (var client = new CopilotClient()) {
            client.start().get();

            // Create session with a memorable ID
            var session = client.createSession(
                new SessionConfig()
                    .setOnPermissionRequest(PermissionHandler.APPROVE_ALL)
                    .setSessionId("user-123-conversation")
                    .setModel("gpt-5")
            ).get();

            session.on(AssistantMessageEvent.class, msg ->
                System.out.println(msg.getData().content())
            );

            session.sendAndWait(new MessageOptions()
                .setPrompt("Let's discuss TypeScript generics")).get();

            // Session ID is preserved
            System.out.println("Session ID: " + session.getSessionId());

            // Close session but keep data on disk
            session.close();
        }
    }
}
```
##恢复会话```java
//DEPS com.github:copilot-sdk-java:0.2.1-java.1
import com.github.copilot.sdk.CopilotClient;
import com.github.copilot.sdk.events.AssistantMessageEvent;
import com.github.copilot.sdk.json.MessageOptions;
import com.github.copilot.sdk.json.PermissionHandler;
import com.github.copilot.sdk.json.ResumeSessionConfig;

public class ResumeSession {
    public static void main(String[] args) throws Exception {
        try (var client = new CopilotClient()) {
            client.start().get();

            // Resume the previous session
            var session = client.resumeSession(
                "user-123-conversation",
                new ResumeSessionConfig()
                    .setOnPermissionRequest(PermissionHandler.APPROVE_ALL)
            ).get();

            session.on(AssistantMessageEvent.class, msg ->
                System.out.println(msg.getData().content())
            );

            // Previous context is restored
            session.sendAndWait(new MessageOptions()
                .setPrompt("What were we discussing?")).get();

            session.close();
        }
    }
}
```
列出可用的会话```java
//DEPS com.github:copilot-sdk-java:0.2.1-java.1
import com.github.copilot.sdk.CopilotClient;

public class ListSessions {
    public static void main(String[] args) throws Exception {
        try (var client = new CopilotClient()) {
            client.start().get();

            var sessions = client.listSessions().get();
            for (var sessionInfo : sessions) {
                System.out.println("Session: " + sessionInfo.getSessionId());
            }
        }
    }
}
```
永久删除会话```java
//DEPS com.github:copilot-sdk-java:0.2.1-java.1
import com.github.copilot.sdk.CopilotClient;

public class DeleteSession {
    public static void main(String[] args) throws Exception {
        try (var client = new CopilotClient()) {
            client.start().get();

            // Remove session and all its data from disk
            client.deleteSession("user-123-conversation").get();
            System.out.println("Session deleted");
        }
    }
}
```
获取会话历史记录```java
//DEPS com.github:copilot-sdk-java:0.2.1-java.1
import com.github.copilot.sdk.CopilotClient;
import com.github.copilot.sdk.events.AssistantMessageEvent;
import com.github.copilot.sdk.events.UserMessageEvent;
import com.github.copilot.sdk.json.PermissionHandler;
import com.github.copilot.sdk.json.ResumeSessionConfig;

public class SessionHistory {
    public static void main(String[] args) throws Exception {
        try (var client = new CopilotClient()) {
            client.start().get();

            var session = client.resumeSession(
                "user-123-conversation",
                new ResumeSessionConfig()
                    .setOnPermissionRequest(PermissionHandler.APPROVE_ALL)
            ).get();

            var messages = session.getMessages().get();
            for (var event : messages) {
                if (event instanceof AssistantMessageEvent msg) {
                    System.out.printf("[assistant] %s%n", msg.getData().content());
                } else if (event instanceof UserMessageEvent userMsg) {
                    System.out.printf("[user] %s%n", userMsg.getData().content());
                } else {
                    System.out.printf("[%s]%n", event.getType());
                }
            }

            session.close();
        }
    }
}
```
完整的会话管理示例

这个交互式示例允许您从命令行创建、恢复或列出会话。```java
//DEPS com.github:copilot-sdk-java:0.2.1-java.1
import com.github.copilot.sdk.CopilotClient;
import com.github.copilot.sdk.events.AssistantMessageEvent;
import com.github.copilot.sdk.json.*;
import java.util.Scanner;

public class SessionManager {
    public static void main(String[] args) throws Exception {
        try (var client = new CopilotClient();
             var scanner = new Scanner(System.in)) {

            client.start().get();

            System.out.println("Session Manager");
            System.out.println("1. Create new session");
            System.out.println("2. Resume existing session");
            System.out.println("3. List sessions");
            System.out.print("Choose an option: ");

            int choice = scanner.nextInt();
            scanner.nextLine();

            switch (choice) {
                case 1 -> {
                    System.out.print("Enter session ID: ");
                    String sessionId = scanner.nextLine();
                    var session = client.createSession(
                        new SessionConfig()
                            .setOnPermissionRequest(PermissionHandler.APPROVE_ALL)
                            .setSessionId(sessionId)
                            .setModel("gpt-5")
                    ).get();

                    session.on(AssistantMessageEvent.class, msg ->
                        System.out.println("\nCopilot: " + msg.getData().content())
                    );

                    System.out.println("Created session: " + sessionId);
                    chatLoop(session, scanner);
                    session.close();
                }

                case 2 -> {
                    System.out.print("Enter session ID to resume: ");
                    String resumeId = scanner.nextLine();
                    try {
                        var session = client.resumeSession(
                            resumeId,
                            new ResumeSessionConfig()
                                .setOnPermissionRequest(PermissionHandler.APPROVE_ALL)
                        ).get();

                        session.on(AssistantMessageEvent.class, msg ->
                            System.out.println("\nCopilot: " + msg.getData().content())
                        );

                        System.out.println("Resumed session: " + resumeId);
                        chatLoop(session, scanner);
                        session.close();
                    } catch (Exception ex) {
                        System.err.println("Failed to resume session: " + ex.getMessage());
                    }
                }

                case 3 -> {
                    var sessions = client.listSessions().get();
                    System.out.println("\nAvailable sessions:");
                    for (var s : sessions) {
                        System.out.println("  - " + s.getSessionId());
                    }
                }

                default -> System.out.println("Invalid choice");
            }
        }
    }

    static void chatLoop(Object session, Scanner scanner) throws Exception {
        System.out.println("\nStart chatting (type 'exit' to quit):");
        while (true) {
            System.out.print("\nYou: ");
            String input = scanner.nextLine();
            if (input.equalsIgnoreCase("exit")) break;

            // Use reflection-free approach: cast to the session type
            var s = (com.github.copilot.sdk.CopilotSession) session;
            s.sendAndWait(new MessageOptions().setPrompt(input)).get();
        }
    }
}
```
检查会话是否存在```java
//DEPS com.github:copilot-sdk-java:0.2.1-java.1
import com.github.copilot.sdk.CopilotClient;
import com.github.copilot.sdk.json.*;

public class CheckSession {
    public static boolean sessionExists(CopilotClient client, String sessionId) {
        try {
            var sessions = client.listSessions().get();
            return sessions.stream()
                .anyMatch(s -> s.getSessionId().equals(sessionId));
        } catch (Exception ex) {
            return false;
        }
    }

    public static void main(String[] args) throws Exception {
        try (var client = new CopilotClient()) {
            client.start().get();

            String sessionId = "user-123-conversation";

            if (sessionExists(client, sessionId)) {
                System.out.println("Session exists, resuming...");
                var session = client.resumeSession(
                    sessionId,
                    new ResumeSessionConfig()
                        .setOnPermissionRequest(PermissionHandler.APPROVE_ALL)
                ).get();
                // ... use session ...
                session.close();
            } else {
                System.out.println("Session doesn't exist, creating new one...");
                var session = client.createSession(
                    new SessionConfig()
                        .setOnPermissionRequest(PermissionHandler.APPROVE_ALL)
                        .setSessionId(sessionId)
                        .setModel("gpt-5")
                ).get();
                // ... use session ...
                session.close();
            }
        }
    }
}
```
最佳实践

1. **使用有意义的会话ID **：在会话ID中包含用户ID或上下文（例如，`"user-123-chat"`,`"task-456-review"`）
2. **处理丢失的会话**：在恢复之前检查会话是否存在-使用`listSessions()`或从`resumeSession()`捕获异常
3. **清理旧会话**：定期删除不再需要`deleteSession()`的会话
4. **错误处理：总是在try-catch块中包装恢复操作-会话可能已被删除或过期
5. **工作空间感知**：会话绑定到工作空间路径；确保跨环境恢复时的一致性