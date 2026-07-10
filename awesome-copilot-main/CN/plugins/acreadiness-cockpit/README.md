# acreadiness-cockpit

从副驾驶聊天中驱动[Microsoft AgentRC]（https://github.com/microsoft/agentrc）。框架AgentRC的**Measure→Generate→maintenance **循环中的每个交互。

##插件里有什么

自定义代理

|代理|它做什么||---|---|
运行`agentrc readiness --json`，根据9柱/ 5级模型解释每个结果，然后从固定的HTML/CSS模板呈现自包含的`reports/index.html`，以便每个用户获得相同风格的仪表板。奖励策略（禁用标准、覆盖、通过率阈值）并单独显示额外内容。|

# # #技能

|技能|步骤|它做什么||---|---|---|
|`/acreadiness-assess`| **Measure** |运行就绪扫描并将手交给`@ai-readiness-reporter`以生成静态HTML仪表板。接受`--policy <path-or-pkg>`和`--per-area`。|
|`/acreadiness-generate-instructions`| **生成** |封装`agentrc instructions`。默认输出为`.github/copilot-instructions.md`（Copilot-native）。问`flat`vs`nested`。对于单节点，还使用`applyTo`globs发出每个区域的`.github/instructions/<area>.instructions.md`文件。|
|`/acreadiness-policy`| **维护** |选择、支撑或应用AgentRC策略。了解模式（`criteria.disable`、`criteria.override`、`extras`、`thresholds`）、影响权重表，以及使用`--fail-level`进行CI控制。|

##生产什么`reports/index.html`-从固定模板（`skills/acreadiness-assess/report-template.html`）呈现的单个自包含HTML文件，因此每个用户都获得相同的外观和感觉。它包含:-成熟标志（L1-L5）及总分/等级（A-F）
-通过率vs阈值（当策略设置一个时）
-成熟期进度表
- **活动策略**总结（disabled/overridden标准，阈值）
- **Repo健康**分解（8个支柱），每个都有**AI相关性**徽章（High/Medium/Low）， *它衡量什么*，*为什么它对AI很重要*，*当前状态*，*建议*
- **AI设置**分解（AI工具支柱）
- **额外信息**（仅提供信息- agents-doc， pr-template, pre-commit, architecture-doc）
- **优先修复计划**（🔴先修复/🟡后修复/🔵计划）
-嵌入原始AgentRC JSON供重用

# #先决条件

- **Node.js20+** on PATH （AgentRC要求）
-VS Code与副驾驶代理插件启用

# #使用

在副驾驶聊天中：```text
/acreadiness-assess                                 # measure → reports/index.html
/acreadiness-assess --policy ./policies/strict.json
/acreadiness-generate-instructions                  # asks flat or nested
/acreadiness-generate-instructions --strategy flat
/acreadiness-generate-instructions --strategy nested
/acreadiness-generate-instructions --areas          # per-area applyTo files
/acreadiness-policy new my-policy
@ai-readiness-reporter
```
扁平指令vs嵌套指令

| | **平面** *（默认）* | **嵌套** ||---|---|---|
| Hub文件|`.github/copilot-instructions.md`|`.github/copilot-instructions.md`|
|详细文件| - |`.github/instructions/<topic>.instructions.md`（每个`applyTo`glob） |
|最适合|小型/中型回购，单堆叠|大型或多堆叠回购，单堆叠|
|令牌成本|整个文件总是加载|VS Code只加载`applyTo`匹配|的主题

当主要输出为`.github/copilot-instructions.md`时，该技能将AgentRC的嵌套输出重写为VS Code的原生`.instructions.md`布局（Copilot自动发现）。对于`--output AGENTS.md`，嵌套为代理无关工具保留了AgentRC的默认`.agents/`布局。

###概念（小抄）- **成熟度**:L1功能→L2文档化→L3标准化→L4优化→L5自治
- **支柱** （Repo健康）：风格·构建·测试·文档·开发环境·代码质量·可观察性·安全性
- **支柱** （AI设置）：AI工具
- **冲击权重**：临界5·高4·中3·低2·高0
- * *年级* *:≥0.9 B≥0.8·C·≥0.7·D·F < 0.6≥0.6

# #许可证

麻省理工学院