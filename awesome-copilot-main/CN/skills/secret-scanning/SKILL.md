---
name: secret-scanning
description: 'Guide for configuring and managing GitHub secret scanning, push protection, custom patterns, and secret alert remediation. For pre-commit secret scanning in AI coding agents via the GitHub MCP Server, this skill references the Advanced Security plugin (`advanced-security@copilot-plugins`). Use this skill when enabling secret scanning, setting up push protection, defining custom patterns, triaging alerts, resolving blocked pushes, or when an agent needs to scan code for secrets before committing.'
---
#秘密扫描

此技能提供了配置GitHub秘密扫描的过程指导-检测泄露的凭据，防止秘密推送，定义自定义模式和管理警报。

何时使用此技能

当请求涉及：—为存储库或组织启用或配置秘密扫描
-设置推送保护，阻止秘密到达存储库之前
—使用正则表达式定义自定义秘密模式
-从命令行解决阻塞的推送
-筛选、驳回或修复秘密扫描警报
—配置推保护委托旁路
—通过`secret_scanning.yml`排除秘密扫描目录
-了解警报类型（用户、合作伙伴、推送保护）
—启用有效性检查或扩展元数据检查
-在提交之前扫描本地代码更改的秘密（通过MCP / AI编码代理）-参见**预提交扫描通过AI编码代理**部分下面推荐的插件

秘密扫描是如何工作的

秘密扫描自动检测暴露的凭据：-所有分支上的完整Git历史记录
-发布描述、评论和标题（开放和封闭）
-提取请求标题、描述和评论
- GitHub讨论标题，描述和评论
-维基和秘密专家

# # #的可用性

|存储库类型|可用性||---|---|
|公共回购|自动，免费|
|Private/internal（组织拥有）|需要GitHub秘密保护Team/Enterprise云|
|用户拥有|企业云与企业管理用户|

##核心工作流-启用秘密扫描

###步骤1：启用秘密保护

1. 导航到存储库**设置**→**高级安全**
2. 点击“秘密保护”旁边的“**启用**”
3. 点击**启用秘密保护**确认

对于组织，使用安全配置来大规模启用：
—设置→高级安全→全局设置→安全配置

###步骤2：启用推送保护

推送保护在推送过程中阻止秘密——在它们到达存储库之前。

1. 导航到存储库**设置**→**高级安全**
2. 开启“秘密保护”下的“推送保护”推保护块秘密在：
-命令行推送
- GitHub UI提交
-上传文件
- REST API请求
- REST API内容创建端点

###步骤3：配置排除（可选）

创建`.github/secret_scanning.yml`以自动关闭特定目录的警报：```yaml
paths-ignore:
  - "docs/**"
  - "test/fixtures/**"
  - "**/*.example"
```
* *限制:* *
—“`paths-ignore`”最多1000条
—文件大小不能超过1mb
—排除的路径也会跳过推送保护检查

* *最佳实践:* *
-尽可能明确排除路径
—添加注释，解释每个路径被排除的原因
-定期检查排除项-删除过时的条目
-通知安全团队有关排除

###步骤4：启用附加功能（可选）

**非提供者模式** -检测私钥，连接字符串，通用API密钥：
-设置→高级安全→启用“扫描非提供者模式”

**人工智能驱动的通用秘密检测** -使用Copilot检测密码等非结构化秘密：
-设置→高级安全→启用“使用AI检测”**有效性检查** -验证是否检测到的秘密仍然有效；
-设置→高级安全→启用“有效性检查”
- GitHub定期根据提供商api测试检测到的凭据
—告警状态：`active`、`inactive`、`unknown`**扩展元数据检查** -关于谁拥有一个秘密的额外上下文：
—要求先启用有效性检查
-帮助确定修复的优先级并确定责任团队

核心工作流-解决阻塞推送

当push保护阻止命令行推送时：

选项A：删除秘密

**如果秘密是在最近的提交：**```bash
# Remove the secret from the file
# Then amend the commit
git commit --amend --all
git push
```
**如果secret在之前的提交中：**```bash
# Find the earliest commit containing the secret
git log

# Start interactive rebase before that commit
git rebase -i <COMMIT-ID>~1

# Change 'pick' to 'edit' for the offending commit
# Remove the secret, then:
git add .
git commit --amend
git rebase --continue
git push
```
选项B：旁路推保护

1. 访问推送错误消息中返回的URL（以相同的用户）
2. 选择旁路原因：
- **它用于测试** -警报创建和自动关闭
- **这是一个误报** -警报创建和自动关闭
- **我以后会修复它** -打开警报创建
3. 点击**允许我推送这个秘密**
4. 3小时内重新推送

选项C：请求旁路权限

如果启用了委托旁路，而您缺乏旁路权限：
1. 访问来自推送错误的URL
2. 添加评论，解释为什么这个秘密是安全的
3. 单击**提交请求**
4. 等待approval/denial的邮件通知
5. 如果获得批准，则推动提交；如果被拒绝，就删除这个秘密

>详细的旁路和委派旁路工作流程，请搜索`references/push-protection.md`。

自定义模式

使用正则表达式定义特定于组织的秘密模式。

快速设置1. 设置→高级安全→自定义模式→**新模式**
2. 为秘密格式输入模式名称和正则表达式
3. 添加一个示例测试字符串
4. 点击**保存并预演**进行测试（多达1,000个结果）
5. 检查假阳性结果
6. 单击**发布模式**
7. 可选地为模式启用推送保护

# # #范围

自定义模式可以定义在：
- **存储库级别** -仅适用于该仓库
- **组织级别** -适用于所有启用了秘密扫描的仓库
-企业级** -适用于所有组织

辅助模式生成

使用Copilot秘密扫描从秘密类型的文本描述生成正则表达式，包括可选的示例字符串。

>有关详细的自定义模式配置，请搜索`references/custom-patterns.md`。

##警报管理

###警报类型

|类型|描述|可见性||---|---|---|
| **用户警报** |在存储库|安全选项卡|中发现的秘密
| **推送保护警报** |通过旁路|推送的秘密安全选项卡（过滤器：`bypassed: true`） |
| **合作伙伴警报** |向提供商|报告的秘密未在repo（仅限提供商）|中显示

###警报列表

- **默认警报**支持的提供者模式和自定义模式
- **一般警报** -非提供者模式和ai检测到的秘密（每次回购限制为5,000个）

修复优先级

1. **立即旋转证书** -这是关键操作
2. 查看警报的上下文（位置、提交、作者）
3. 校验有效性状态：`active`（紧急）、`inactive`（低优先级）、`unknown`4. 如果需要，从Git历史记录中删除（时间密集，在轮换之后通常不需要）

###取消警报以书面理由驳回：
- **误报** -检测到的字符串不是一个真正的秘密
- **已撤销** -证书已被撤销
- **用于测试** - secret仅用于测试代码

>详细的告警类型、有效性检查和REST API，请搜索`references/alerts-and-remediation.md`。

通过AI编码代理进行预提交扫描

为了在提交之前扫描AI编码代理内部的秘密代码更改，请安装**高级安全插件**，该插件提供`run_secret_scanning`MCP工具和专用扫描技能。

* *GitHub CopilotCLI: * *```bash
/plugin install advanced-security@copilot-plugins
```
**Visual Studio代码：**
-在Copilot聊天，打开**聊天：插件**（或使用`@agentPlugins`），并安装`advanced-security`插件
-然后在副驾驶聊天中运行`/secret-scanning`参见：[高级安全插件-秘密扫描技能]（https://github.com/github/copilot-plugins/blob/main/plugins/advanced-security/skills/secret-scanning/SKILL.md）

>在[通过GitHub MCP服务器秘密扫描AI编码代理]（https://github.blog/changelog/2026-03-17-secret-scanning-in-ai-coding-agents-via-the-github-mcp-server/）中宣布（2026年3月）

##参考文件

有关详细文档，请根据需要加载以下参考文件：-`references/push-protection.md`-推送保护机制，旁路工作流程，委托旁路，用户推送保护
-搜索模式：`bypass`、`delegated`、`bypass request`、`command line`、`REST API`、`user push protection`自定义模式创建，正则表达式语法，预演，Copilot正则表达式生成，作用域
-搜索模式：`custom pattern`、`regex`、`dry run`、`publish`、`organization`、`enterprise`、`Copilot`-`references/alerts-and-remediation.md`-警报类型，有效性检查，扩展元数据，通用警报，秘密删除，REST API
-搜索模式：`user alert`、`partner alert`、`validity`、`metadata`、`generic`、`remediation`、`git history`、`REST API`