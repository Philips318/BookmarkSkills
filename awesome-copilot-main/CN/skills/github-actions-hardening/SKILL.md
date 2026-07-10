---
name: github-actions-hardening
description: Security hardening reviewer for GitHub Actions workflow files (.github/workflows/*.yml). Reasons about the Actions threat model that pattern matchers and general code linters miss — untrusted-input script injection, privileged triggers running fork code, mutable action references, and over-scoped tokens. Use this skill when asked to review, audit, harden, or secure a GitHub Actions workflow, when writing a new workflow, or for any request like "is this workflow safe?", "review my CI for security issues", "why is pull_request_target dangerous here?", "pin my actions", or "lock down GITHUB_TOKEN permissions". Covers script injection via ${{ }} interpolation, pull_request_target / workflow_run privilege escalation, SHA-pinning of third-party actions, least-privilege permissions, GITHUB_ENV/GITHUB_OUTPUT injection, secret exposure, OIDC over long-lived credentials, and self-hosted runner exposure on public repositories.
---
#GitHub Actions加固

专注于GitHub Actions工作流的安全审查。它解释了*具体的行动*
威胁模型——信任边界存在于触发器类型、令牌范围和字符串中
插值——而不是一般安全扫描程序所查找的应用程序代码漏洞
对。大多数工作流风险对于语言检查器来说是不可见的，因为危险的代码就是YAML
本身以及GitHub在脚本运行之前将`${{ }}`表达式扩展到shell中的方式。

何时使用此技能

当请求涉及：*审查、审计或加固`.github/workflows/`下的任何文件
*创建一个新的工作流并希望它在默认情况下是安全的
*使用`pull_request_target`，`workflow_run`，或`issue_comment`触发器的工作流
*关于`GITHUB_TOKEN`权限或`permissions:`键的问题
*绑定动作提交sha vs标签vs分支
*在`run:`步骤中处理不可信的输入（issue标题，PR主体，分支名称，提交消息）
*来自action的OIDC /云认证，或CI中的秘密处理
*公共存储库上的自托管运行程序
*任何请求，如“这个工作流程安全吗？”，“保护我的CI”，或“审查这个GitHub行动”

##核心洞察力

在工作流中，**`${{ <expr> }}`被运行器扩展到shell之前的脚本中
执行它。**这样的步骤：```yaml
- run: echo "Title: ${{ github.event.issue.title }}"
```
不是传递一个变量-它是*粘贴攻击者控制的文本直接到您的shell
命令*。标题为`"; <attacker-command> #`的问题被连接到脚本中并执行。
这种单一机制是现实世界中最常见的Actions漏洞，并且经常建模
生成它。对待每一个`${{ }}`，它包含外部贡献者可以影响的数据，作为代码注入接收器。

##执行流程

按照以下步骤** **审查每个工作流程。

步骤1 -映射触发器和信任级别

读取每个`on:`触发器并对工作流的权限进行分类：*`push`,`pull_request`（来自同一个repo）→在贡献者自己的信任下运行
*`pull_request`从**分叉**→运行**只读**令牌，**无秘密**（安全的设计）
*`pull_request_target`,`workflow_run`,`issue_comment`，`issues`→在
**基存储库**带有**read/write令牌和对秘密的完全访问**，但是可以
**由外部贡献者**触发。这些都是危险的诱因。

请阅读`references/triggers-and-privilege.md`了解完整的信任矩阵。

###步骤2 -寻找脚本注入

对于每个`run:`块，`actions/github-script`中的每个`script:`，以及每个自定义输入
动作时，列出`${{ }}`表达式，检查是否有解析到攻击者可控的数据。
高风险环境包括：

*`github.event.issue.title`,`github.event.issue.body`*`github.event.pull_request.title`,`github.event.pull_request.body`,`.head.ref`,`.head.label`*`github.event.comment.body`,`github.event.review.body`*`github.event.pages.*.page_name`,`github.event.commits.*.message`,`github.event.head_commit.*`*`github.head_ref`和任何`github.event.*`字段的分叉作者可以设置阅读`references/injection.md`获取完整的接收列表和安全模式修复。

###步骤3 -检查特权触发器不执行不受信任的代码

如果`pull_request_target`或`workflow_run`工作流检出PR/fork代码
（`ref: ${{ github.event.pull_request.head.sha }}`） **，然后运行它**(构建，测试，安装
脚本，`npm install`与生命周期脚本，等等)，这是远程代码执行
享有特权的令牌。将其标记为CRITICAL。安全的模式是分成两个工作流：一个
非特权`pull_request`工作流，运行不受信任的代码，以及特权
仅使用其结果的`workflow_run`工作流。

###步骤4 -审计`permissions:`如果有**没有**`permissions:`块，工作流继承存储库默认值，这可能
对每件事都要谨慎。国旗。
*推荐一个顶级`permissions: {}`（deny-all）或`contents: read`，然后授予最小值
每个作业（例如，`pull-requests: write`仅适用于注释的作业）。
*标记任何`permissions: write-all`或`write`范围，这些步骤实际上不需要。

请阅读`references/permissions-and-tokens.md`了解每个作用域的指导和OIDC设置。

###步骤5 -审计行动参考（供应链）

对于每一个`uses:`：** *第三方操作**（不是`actions/*`或`github/*`）必须固定为完整的40个字符
提交SHA，而不是标签或分支。标签和分支是可变的；妥协的上游操作
可以将`v1`重写为带有令牌和密钥的恶意代码。
*第一方`actions/*`风险较低，但sha -pin仍然是强化的建议。
*标志`@main`，`@master`，或任何分支引用为HIGH -这是“最新的”，可以更改
你随时都可以。
*请在后面的注释中注意人类可读的版本：`uses: foo/bar@<sha> # v2.1.0`。

请阅读`references/supply-chain.md`以了解固定，阅读Dependabot以了解操作，阅读artifact/cache以了解风险。

###步骤6 -检查秘密和输出处理*没有秘密的回声，打印或写入日志；触摸步骤中没有`set -x`/`bash -x`的秘密。
*机密不能传递给运行不受信任代码的步骤或不受信任的第三方操作。
*不受信任的多行数据写入`$GITHUB_ENV`或`$GITHUB_OUTPUT`会注入环境
变量或步骤输出-使用随机分隔符heredoc格式，不要写入原始用户输入。
*`actions/checkout`默认在磁盘上留下一个令牌；设置`persist-credentials: false`作业之后运行不受信任的代码。

###第7步-生成报告

使用`references/report-format.md`格式输出结果：首先是严重性汇总表，
然后将结果分组为文件、确切的违规YAML、简单的风险和a
混凝土before/after修复。永远不要自动应用更改-呈现它们以供审查。

##严重性指南

|级别|含义|示例|| --- | --- | --- |
|🔴临界|Token/secret盗窃或RCE可由外部贡献者|`pull_request_target`检查和运行分叉代码；`${{ github.event.* }}`中的`run:`在特权触发器|上
|🟠HIGH |可利用的供应链或范围问题|第三方对可变tag/branch的操作；`write-all`权限;`issue_comment`|上的注入sink
|🟡MEDIUM |链接|条件下的风险缺失`permissions:`块；非分叉PR作者|可访问的秘密
|🔵低|硬化缺口，低直接风险|第一方行为非sha -pin；`persist-credentials`在非特权作业|上留下默认值
|⚪INFO |观察，不是一个漏洞|版本注释丢失旁边的固定SHA |

##输出规则** *总是**先显示调查结果汇总表（按严重性计数）。
** *按问题类型**分组，而不是按文件分组。
** *要准确** -引用冒犯的行并给出行位置。
** *总是**将每个CRITICAL/HIGH与一个具体的更正的YAML代码片段配对。
** *永远不要仅仅因为一个分支`pull_request`运行了不受信任的代码就声称它是危险的——它确实是危险的
没有秘密和只读令牌。为特权触发器保留CRITICAL。
*如果工作流程已经加固，说明并列出已检查的内容。

##参考文件

根据需要加载这些：*`references/triggers-and-privilege.md`-信任矩阵为每个触发，为什么`pull_request_target`而`workflow_run`则享有特权，且采用双工作流安全模式。
+搜索模式：`pull_request_target`、`workflow_run`、`issue_comment`、`fork`、`secrets`、`read-only token`、`trust boundary`*`references/injection.md`-攻击者可控的`${{ }}`上下文的完整列表
每个接收器的`env:`变量安全模式（`run`,`github-script`，动作输入）。
+搜索模式：`script injection`、`github.event`、`head_ref`、`issue title`、`env`、`intermediate variable`、`actions/github-script`*`references/permissions-and-tokens.md`-`GITHUB_TOKEN`范围，最小权限`permissions:`每个作业类型的食谱，以及OIDC的云认证，而不是长期存在的秘密。
+搜索模式：`permissions`、`GITHUB_TOKEN`、`write-all`、`contents: read`、`id-token`、`OIDC`、`least privilege`*`references/supply-chain.md`- sha -pin第三方动作，依赖于`github-actions`跨`workflow_run`的工件和缓存中毒，以及自托管运行程序暴露。
+搜索模式：`SHA pin`、`uses`、`mutable tag`、`Dependabot`、`download-artifact`,`cache`,`self-hosted runner`*`references/report-format.md`-输出模板：汇总表，查找卡，和before/after修复块。
+搜索模式：`report`、`format`、`finding`、`summary`、`remediation`、`before`、`after`