---
name: acreadiness-assess
description: 'Run the AgentRC readiness assessment on the current repository and produce a static HTML dashboard at reports/index.html. Wraps `npx github:microsoft/agentrc readiness` and hands off rendering to the @ai-readiness-reporter custom agent. Supports policies (--policy) for org-specific scoring. Use when asked to assess, audit, or score the AI readiness of a repo.'
argument-hint: "[--policy <path-or-pkg>] [--per-area] — e.g. /acreadiness-assess, /acreadiness-assess --policy ./policies/strict.json"
---
# /acreadiness- evaluate - ai就绪评估

无论何时用户要求进行ai就绪评估、就绪检查、审计，或者想要查看存储库的ai就绪程度，都可以使用此技能。

这个技能是AgentRC的**Measure→Generate→maintenance **循环中的*Measure*步骤。结果是一个自包含的HTML仪表板，用户可以使用`file://`打开或提交到repo。

# #的步骤

1. * *确认先决条件。**节点20+必须在PATH上。如果不确定，请运行`node --version`。

2. **决定一个政策**（可选但鼓励）：
—如果用户提供了`--policy <source>`，则捕获它。
—否则，检查`agentrc.config.json`是否为`policies`阵列。
—如果两者都不是，则不带策略运行（内置默认值）。
对于策略入门，建议学习`acreadiness-policy`技能。

3. **在repo根目录下运行readiness scan**，输出结构化的结果：   ```bash
   npx -y github:microsoft/agentrc readiness --json [--policy <source>] [--per-area]
   ```
`CommandResult<T>`JSON信封是下一步的输入。4. **交给`ai-readiness-reporter`自定义代理**来解释JSON并生成`reports/index.html`。代理通过捆绑的模板`report-template.html`（与此技能一起提供）呈现，因此每个报告都具有相同的外观和感觉。代理:
-读取捆绑的`report-template.html`，并用实际数据替换占位符。
-内联所有的CSS，船舶一个单一的静态文件（工作在`file://`）。
-呈现成熟度水平，总分，等级，通过率与门槛。
-分解所有9个支柱跨越**Repo健康**(8)和**AI设置**(1)*它测量什么*，*为什么它对AI重要*，*当前状态*和*具体建议*。
-标记每个支柱与AI相关的徽章（高/中/低）。
-表面**额外**分开（他们从不影响得分）。
—显示**Active Policy**，包括所有disabled/overridden标准和阈值。
-制定**优先补救计划**（🔴先修复/🟡后修复/🔵计划）。
-嵌入原始AgentRC JSON以供重用。5. **告诉用户报告所在的位置** (`reports/index.html`)以及如何打开它。在聊天中总结：成熟度级别，总体得分，前三个最低支柱，以及一个最高杠杆的下一步行动（几乎总是：运行`acreadiness-generate-instructions`技能）。

# #笔记

AgentRC也有一个内置的HTML渲染器（`--visual`/`--output report.html`），但它的输出是故意通用的。这项技能通过自定义代理生成一个定制的、固执己见的仪表板——更接近于代码审查，而不是指标转储。
—CI门控推荐使用`agentrc readiness --fail-level <n>`（1-5）。
-该技能从不修改存储库文件，除了创建`reports/index.html`。