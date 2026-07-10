# ax配置文件设置

当身份验证失败（401、缺少配置文件、缺少API密钥）时，请参考此文件。不要主动运行这些检查。

当没有配置文件，或者配置文件有不正确的设置（错误的API密钥，错误的区域等）时使用此选项。

# # 1。检查当前状态```bash
ax profiles show
```
查看输出以了解配置的内容：
—“`API Key: (not set)`”或“missing→”键必须为“created/updated”
-没有配置文件输出或“No profiles found”→没有配置文件存在
-连接，但得到`401 Unauthorized`→键错误或过期
—已连接但错误的endpoint/region→区域需要更新

# # 2。修复配置错误的概要文件

如果一个配置文件存在，但一个或多个设置是错误的，只修补损坏的部分。

**永远不要将原始API键值作为标志传递。**始终通过`ARIZE_API_KEY`环境变量引用它。如果这个变量还没有在shell中设置好，指示用户先设置好，然后运行命令：```bash
# If ARIZE_API_KEY is already exported in the shell:
ax profiles update --api-key $ARIZE_API_KEY

# Fix the region (no secret involved — safe to run directly)
ax profiles update --region us-east-1b

# Fix both at once
ax profiles update --api-key $ARIZE_API_KEY --region us-east-1b
```
`update`仅更改您指定的字段—保留所有其他设置。如果没有给出配置文件名称，则更新活动配置文件。

# # 3。创建一个新的概要文件

如果没有配置文件存在，或者如果现有的配置文件需要指向一个完全不同的设置（不同的组织，不同的区域）：

**始终通过`$ARIZE_API_KEY`引用键，永远不要内联原始值```bash
# Requires ARIZE_API_KEY to be exported in the shell first
ax profiles create --api-key $ARIZE_API_KEY

# Create with a region
ax profiles create --api-key $ARIZE_API_KEY --region us-east-1b

# Create a named profile
ax profiles create work --api-key $ARIZE_API_KEY --region us-east-1b
```
要将命名配置文件与任何`ax`命令一起使用，请添加`-p NAME`：```bash
ax spans export PROJECT -p work
```
# # 4。获取API密钥

**永远不要要求用户粘贴他们的API密钥到聊天。不要记录、回显或显示API键值

如果`ARIZE_API_KEY`还没有设置，指示用户在shell中导出它：```bash
export ARIZE_API_KEY="..."   # user pastes their key here in their own terminal
```
他们可以通过导航到设置页面在https://app.arize.com找到他们的键。建议他们创建一个**作用域的服务密钥**（而不是个人用户密钥）——服务密钥不与个人帐户绑定，对于编程使用更安全。键是空间作用域的-确保它们为正确的空间复制键。

一旦用户确认设置了变量，就按照上面的描述继续使用`ax profiles create --api-key $ARIZE_API_KEY`或`ax profiles update --api-key $ARIZE_API_KEY`。

# # 5。验证

在创建或更新之后：```bash
ax profiles show
```
确认API密钥和区域正确，然后重试原命令。

# #空间

没有空间的配置文件标志。将其保存为环境变量—接受一个空间**名称**（例如，`my-workspace`）或一个base64空间**ID**（例如，`U3BhY2U6...`）。用`ax spaces list -o json`找到你的。

**macOS/Linux** -添加到`~/.zshrc`或`~/.bashrc`：```bash
export ARIZE_SPACE="my-workspace"    # name or base64 ID
```
然后`source ~/.zshrc`（或重启终端）。

Windows (PowerShell): * * * *```powershell
[System.Environment]::SetEnvironmentVariable('ARIZE_SPACE', 'my-workspace', 'User')
```
重启终端使其生效。

##保存凭据以备将来使用

在会话结束时，如果用户在此会话中手动提供了任何凭据，并且这些值尚未从保存的配置文件或环境变量中加载，建议保存它们。

**如果：**，请跳过此部分
- API密钥已从现有配置文件或`ARIZE_API_KEY`env var加载
—空间已经通过`ARIZE_SPACE`env var设置
-用户只使用base64项目id（不需要空格）

**如何提供：**使用**AskQuestion**：“您是否想保存您的alize凭据，以便下次不必输入它们？”*可选`"Yes, save them"`/`"No thanks"`。

**如果用户说是：**

1. **API密钥** -运行`ax profiles show`查看当前状态。然后运行`ax profiles create --api-key $ARIZE_API_KEY`或`ax profiles update --api-key $ARIZE_API_KEY`（键必须已经作为env变量导出—永远不要传递原始键值）。2. **Space** -请参阅上面的Space部分，将其持久化为环境变量。