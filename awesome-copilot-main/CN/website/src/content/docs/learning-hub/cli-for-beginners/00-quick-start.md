---
title: '00 · Quick Start'
description: 'Install GitHub Copilot CLI, authenticate, and verify your environment with the same flow as the source course.'
authors:
  - GitHub Copilot Learning Hub Team
lastUpdated: 2026-05-08
---
！[第00章：快速入门]（/images/learning-hub/copilot-cli-for-beginners/00/chapter-header.png）

欢迎光临!在本章中，您将安装GitHub CopilotCLI（命令行接口），使用您的GitHub帐户登录，并验证一切正常。这是一个快速设置章节。一旦你准备好并开始运行，真正的演示将从第01章开始！

##🎯学习目标

在本章结束时，您将拥有：

—安装GitHub CopilotCLI
-用你的GitHub账号登录
-验证它的工作与一个简单的测试

>⏱️**预计时间**:~10分钟（5分钟阅读+ 5分钟动手）

---

##✅前提条件

- **GitHub帐户**与副驾驶访问。[参见订阅选项]（https://github.com/features/copilot/plans）。Students/Teachers可以访问Copilot Pro[免费通过GitHub教育]（https://education.github.com/pack）。
- **终端基础**：熟悉`cd`和`ls`等命令

“副驾驶权限”是什么意思GitHub CopilotCLI需要激活副驾驶订阅。您可以在[github.com/settings/copilot]（https://github.com/settings/copilot）查看您的状态。你应该看到：

- **副驾驶个人** -个人订阅
- **副驾驶业务** -通过您的组织
- **副驾驶企业** -通过您的企业
- **GitHub教育** -免费验证students/teachers如果看到“您没有访问GitHub Copilot的权限”，则需要使用免费选项、订阅计划或加入提供访问权限的组织。

---

# #安装

>⏱️**时间预估**：安装耗时2-5分钟。验证会增加1-2分钟。

推荐：GitHub代码空间（零设置）

如果您不想安装任何先决条件，您可以使用GitHub Codespaces，它已经准备好了GitHub CopilotCLI（您需要登录），预安装Python 3.13， pytest和GitHub CLI。1. [Fork这个仓库]（https://github.com/github/copilot-cli-for-beginners/fork）到你的GitHub账户
2. 选择**代码** > **代码空间** > **在主**上创建代码空间
3. 等待几分钟，等待容器构建完成
4. 你准备好了！终端将在cospace环境中自动打开。

>💡**在代码空间中验证**：运行`cd samples/book-app-project && python book_app.py help`以确认Python和示例应用程序正在工作。

可选：本地安装

>💡**不知道该选哪个？**如果你安装了Node.js，请使用`npm`。否则，选择与您的系统匹配的选项。

>💡**演示所需的Python **：课程使用Python示例应用程序。如果您在本地工作，请在开始演示之前安装[Python 3.10+]（https://www.python.org/downloads/）。注意：**虽然整个课程中显示的主要示例使用Python (`samples/book-app-project`)，但如果您喜欢使用这些语言，也可以使用JavaScript （`samples/book-app-project-js`）和c# （`samples/book-app-project-cs`）版本。每个示例都有一个README，其中包含用该语言运行应用程序的说明。

选择适合您系统的方法：

所有平台（npm）```bash
# If you have Node.js installed, this is a quick way to get the CLI
npm install -g @github/copilot
```
###macOS/Linux（自制）```bash
brew install copilot-cli
```
Windows （WinGet）```bash
winget install GitHub.Copilot
```
###macOS/Linux安装脚本```bash
curl -fsSL https://gh.io/copilot-install | bash
```

<details>
<summary>Optional: Enable shell tab completion</summary>
Shell选项卡完成允许您按** tab **来完成`copilot`子命令、命令选项和一些选项值。这是可选的，但是一旦您习惯使用CLI，它就会很方便。

Copilot CLI目前支持Bash、Zsh和Fish的补全脚本：```shell
# Bash, current session only
source <(copilot completion bash)

# Bash, persistent on Linux
copilot completion bash | sudo tee /etc/bash_completion.d/copilot

# Zsh
copilot completion zsh > "${fpath[1]}/_copilot"

# Fish
copilot completion fish > ~/.config/fish/completions/copilot.fish
```
添加持久完成后重新启动shell。PowerShell支持在Windows上运行Copilot CLI，但`copilot completion`目前只支持Bash， Zsh和Fish。</details>
---

# #身份验证

在`copilot-cli-for-beginners`存储库的根目录下打开终端窗口，启动CLI并允许访问该文件夹。```bash
copilot
```
系统将要求您信任包含存储库的文件夹（如果您还没有信任）。你可以信任它一次，也可以信任以后所有的会话。<img src="/images/learning-hub/copilot-cli-for-beginners/00/copilot-trust.png" alt="Trusting files in a folder with the Copilot CLI" width="800"/>
信任文件夹后，您可以使用您的GitHub帐户登录。```
> /login
```
**接下来发生什么：**

1. Copilot CLI显示一次性代码（如`ABCD-1234`）
2. 浏览器打开GitHub的设备授权页面。如果你还没有登录GitHub。
3. 在提示时输入代码
4. 选择“授权”，授予GitHub CopilotCLI访问权限
5. 返回到您的终端-您现在已登录！<img src="/images/learning-hub/copilot-cli-for-beginners/00/auth-device-flow.png" alt="Device Authorization Flow - showing the 5-step process from terminal login to signed-in confirmation" width="800"/>
*设备授权流程：终端生成代码，用户在浏览器中验证，Copilot CLI通过认证

**提示**：登录在各个会话中持续存在。您只需要这样做一次，除非您的令牌过期或您明确退出。

---

##验证它是否有效

###步骤1：测试副驾驶CLI

现在你已经登录了，让我们验证一下Copilot CLI是否为你工作。在终端中，启动CLI（如果还没有）：```bash
> Say hello and tell me what you can help with
```
收到响应后，可以退出CLI：```bash
> /exit
```

---

<details>
<summary>🎬 See it in action!</summary>
！(你好演示)(/images/learning-hub/copilot-cli-for-beginners/00/hello-demo.gif)

*Demo输出不同。您的模型、工具和响应将与此处显示的有所不同</details>
---

**预期输出**：列出Copilot CLI功能的友好响应。

###步骤2：运行示例图书应用程序

本课程提供了一个示例应用程序，您将在整个课程中使用CLI *探索和改进（您可以在/samples/book-app-project中看到此代码）*。在开始之前，请检查*Python图书收藏终端应用程序*是否正常工作。根据您的系统运行`python`或`python3`。

**注：**虽然整个课程中显示的主要示例使用Python (`samples/book-app-project`)，但如果您喜欢使用这些语言，也可以使用JavaScript （`samples/book-app-project-js`）和c# （`samples/book-app-project-cs`）版本。每个示例都有一个README，其中包含用该语言运行应用程序的说明。```bash
cd samples/book-app-project
python book_app.py list
```
**期望输出**：包括《霍比特人》、《1984》、《沙丘》在内的5本书列表。

###步骤3：尝试Copilot CLI与图书应用程序

首先导航回存储库根目录（如果运行了步骤2）：```bash
cd ../..   # Back to the repository root if needed
copilot 
> What does @samples/book-app-project/book_app.py do?
```
**期望输出**：图书应用的主要功能和命令的总结。

如果看到错误，请检查下面的[故障排除部分]（#故障排除）。

完成后，您可以退出Copilot CLI：```bash
> /exit
```
---

##✅你准备好了！

这就是安装。真正有趣的是从第01章开始的，在那里你将：

-观察AI审查图书应用程序，并立即发现代码质量问题
-学习三种不同的方式使用副驾驶CLI
-从简单的英语生成工作代码

**[继续第01章：第一步→](../01-setup-and-first-steps/)**

---

# #故障排除

### “副驾驶：命令未找到”

没有安装命令行。尝试不同的安装方法：```bash
# If brew failed, try npm:
npm install -g @github/copilot

# Or the install script:
curl -fsSL https://gh.io/copilot-install | bash
```
###“你不能访问GitHub Copilot”

1. 确认你在[github.com/settings/copilot]（https://github.com/settings/copilot）有副驾驶订阅
2. 检查您的组织是否允许使用工作帐户访问CLI

### “Authentication failed”

认证:```bash
copilot
> /login
```
浏览器不会自动打开

手动访问[github.com/login/device]（https://github.com/login/device）并输入终端中显示的代码。

令牌过期

只需再次运行`/login`：```bash
copilot
> /login
```
还是卡住了吗？

-查看[GitHub CopilotCLI文档]（https://docs.github.com/copilot/concepts/agents/about-copilot-cli）
-搜索[GitHub问题]（https://github.com/github/copilot-cli/issues）

---

##🔑关键要点

1. ** GitHub代码空间是快速入门的方法** - Python， pytest和GitHub CopilotCLI都是预安装的，因此您可以直接进入演示
2. **多种安装方法** -选择适合您系统的方法（Homebrew, WinGet， npm或install script）
3. **一次性认证** -登录持续到令牌过期
4. **书籍应用程序工作** -您将在整个课程中使用`samples/book-app-project`>📚**官方文档**:[Install Copilot CLI]（https://docs.github.com/copilot/how-tos/copilot-cli/cli-getting-started）的安装选项和要求。

>📋**快速参考**：请参阅[GitHub CopilotCLI命令参考]（https://docs.github.com/en/copilot/reference/cli-command-reference）获得完整的命令和快捷方式列表。

---