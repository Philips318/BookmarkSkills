#场景挑战

将它们呈现为真实世界的情况。询问用户他们会使用什么commands/shortcuts。
使用`ask_user`对每个步骤进行选择。

场景1：在压力下进行修补程序审查
>一个生产错误修复已经准备好了。您需要检查diff，运行代码审查，并隐藏敏感数据，因为您在直播中。

**答案：**`/streamer-mode`→`/diff`→`/review @src/payment.ts`场景2：上下文窗口救援
你的会话是巨大的，模型质量正在下降。保持连续性，同时减少噪音。

**答：**`/context`→`/compact`→`/resume`（或用`--continue`重启）

场景3：自主重构冲刺
您希望代理以最少的提示执行重构，但只在检查计划和设置权限之后执行。

**答：**`Shift+Tab`（计划模式）→验证计划→`/allow-all`→自动驾驶模式执行场景4：企业入职
>为新的团队存储库设置自定义代理、回购指令和MCP集成。

**答：**添加代理配置文件到`.github/agents/`，验证`/instructions`，再验证`/mcp add`场景5:Power Editing Session
>您正在制作一个很长的提示符，需要在不丢失上下文的情况下快速编辑。

**答案：**`Ctrl+G`（在编辑器中打开），`Ctrl+A`（跳转开始），`Ctrl+K`（修剪）

场景6：座席编排
>你正在领导一个复杂的项目：理解代码，运行测试，重构，然后回顾。

**答案：**`explore`代理（理解）→`task`代理（测试）→`general-purpose`（重构）→`code-review`（验证）

场景7：新项目设置
>你克隆了一个新的回购，需要设置Copilot CLI最大的生产力。

**答：**`/init`→`/model`→`/mcp add`（如有需要）→`Shift+Tab`至Plan模式场景8：安全生产
>从样板工作切换到生产部署脚本。

**答：**`/reset-allowed-tools`→计划模式→每次提交前`/review`