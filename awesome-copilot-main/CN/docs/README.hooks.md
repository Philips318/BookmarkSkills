#🪝钩子

钩子支持在GitHub Copilot编码代理会话期间由特定事件触发的自动化工作流，例如会话开始、会话结束、用户提示和工具使用。
###如何贡献

请参阅[CONTRIBUTING.md]（../CONTRIBUTING.md# addinghooks）了解有关如何贡献新钩子、改进现有钩子和共享用例的指导方针。

如何使用钩子

* *包括:* *
—每个钩子是一个文件夹，包含一个`README.md`文件和一个`hooks.json`配置
钩子可能包括帮助脚本、实用程序或其他捆绑资产
-钩子遵循[GitHub Copilot钩子规范]（https://docs.github.com/en/copilot/how-tos/use-copilot-agents/coding-agent/use-hooks）

安装:* * * *
-将钩子文件夹复制到存储库的`.github/hooks/`目录
-确保任何捆绑的脚本都是可执行的（`chmod +x script.sh`）
-将钩子提交到存储库的默认分支Activate/Use* *: * *
钩子在副驾驶编码代理会话期间自动执行
—在`hooks.json`文件中配置钩子事件
—可选事件：`sessionStart`、`sessionEnd`、`userPromptSubmitted`、`preToolUse`、`postToolUse`、`errorOccurred`**何时使用：**
-自动记录会话和审计跟踪
—在会话结束时自动提交更改
跟踪使用情况分析
—与外部工具和服务集成
—自定义会话工作流

|名称|描述|事件|捆绑资产|| ---- | ----------- | ------ | -------------- |
|[依赖项许可证检查器](../hooks/dependency-license-checker/README.md) |在会话结束时扫描新添加的依赖项是否符合许可证（GPL， AGPL等）| sessionEnd |`check-licenses.sh`<br />`hooks.json`|
|[修复断开的链接](../hooks/fix-broken-links/README.md) |检查更改的网页文件的断开的超链接和SEO锚问题后，每次副驾驶工具的使用。| postToolUse |`hooks.json`<br />`link-fix.ps1`<br />`link-fix.sh`|
| [Governance Audit](../hooks/governance-audit/README.md) |扫描副驾驶代理提示的威胁信号和日志治理事件| sessionStart， sessionEnd, userPromptSubmitted |`audit-prompt.sh`<br />`audit-session-end.sh`<br />`audit-session-start.sh`<br />`hooks.json`|
| [Secrets Scanner](../hooks/secrets-scanner/README.md) |扫描Copilot编码代理会话期间修改的文件，以查找泄露的机密、凭据和敏感数据| sessionEnd |`hooks.json`<br />`scan-secrets.sh`|
| [Session Auto-Commit](../hooks/session-auto-commit/README.md) |当Copilot编码代理会话结束时自动提交和推送更改| sessionEnd |`auto-commit.sh`<br />`hooks.json`|
| (（../hooks/session-logger/README.md） |记录所有Copilot编码代理会话活动，用于审计和分析| sessionStart， sessionEnd, userPromptSubmitted |`hooks.json`<br />`log-prompt.sh`<br />`log-session-end.sh`<br />`log-session-start.sh`|
| [Tool Guardian](../hooks/tool-guardian/README.md) |在Copilot编码代理执行危险的工具操作（破坏性文件操作，强制推送，DB掉落）之前阻止它们| preToolUse |`guard-tool.sh`<br />`hooks.json`|