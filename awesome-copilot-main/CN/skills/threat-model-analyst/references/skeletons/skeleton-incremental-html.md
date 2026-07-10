#骨架：incremental-comparison.html> **⛔自包含的HTML -所有CSS内联。无CDN链接。遵循这个精确的8节结构

---

HTML报告按照这个顺序有8个部分。每个部分都必须呈现。

第1部分：头和比较卡```html
<div class="header">
  <div class="report-badge">INCREMENTAL THREAT MODEL COMPARISON</div>
  <h1>[FILL: repo name]</h1>
</div>
<div class="comparison-cards">
  <div class="compare-card baseline">
    <div class="card-label">BASELINE</div>
    <div class="card-hash">[FILL: baseline SHA]</div>
    <div class="card-date">[FILL: baseline commit date from git log]</div>
    <div class="risk-badge [FILL: old-class]">[FILL: old rating]</div>
  </div>
  <div class="compare-arrow">→</div>
  <div class="compare-card target">
    <div class="card-label">TARGET</div>
    <div class="card-hash">[FILL: target SHA]</div>
    <div class="card-date">[FILL: target commit date from git log]</div>
    <div class="risk-badge [FILL: new-class]">[FILL: new rating]</div>
  </div>
  <div class="compare-card trend">
    <div class="card-label">TREND</div>
    <div class="trend-direction [FILL: color]">[FILL: Improving / Worsening / Stable]</div>
    <div class="trend-duration">[FILL: N months]</div>
  </div>
</div>
```
<!-- SKELETON INSTRUCTION: Section 2 (Risk Shift) is merged into Section 1 above. The old separate risk-shift div is removed. The comparison-cards div replaces both the old subtitle + risk-shift + time-between box. -->
第2部分：参数条（5个框）```html
<div class="metrics-bar">
  [FILL: Components: old → new (±N)]
  [FILL: Trust Boundaries: old → new (±N)]
  [FILL: Threats: old → new (±N)]
  [FILL: Findings: old → new (±N)]
  [FILL: Code Changes: N commits, M PRs — use git rev-list --count and git log --oneline --merges --grep="Merged PR"]
</div>
```
**必须包括信任边界作为5个指标之一。第五个方框是代码更改（不是间隔时间）。**

第三部分：状态总结卡（彩色）```html
<div class="status-cards">
  <!-- Green card --> Fixed: [FILL: count] [FILL: 1-sentence summary, NO IDs]
  <!-- Red card --> New: [FILL: count] [FILL: 1-sentence summary, NO IDs]
  <!-- Amber card --> Previously Unidentified: [FILL: count] [FILL: 1-sentence summary, NO IDs]
  <!-- Gray card --> Still Present: [FILL: count] [FILL: 1-sentence summary, NO IDs]
</div>
```
<!-- SKELETON INSTRUCTION: Status cards show COUNT + a short human-readable sentence ONLY.
不包括威胁id （T06）。年代,T02。E)，查找id (FIND-14)，或组件名称。
好：“1个凭证处理漏洞已修复”
好：“确定了4个新组件和21个新威胁”
好：“没有引入新的威胁或发现”
坏:“T06。S: DefaultAzureCredential→manageddentitycredential
坏：“ConfigurationOrchestrator - 5个威胁（T16）。*)， LLMService - 6个威胁（T17.*）”
带有id的详细逐项细分属于第5节（Threat/Finding状态细分）。-->
**状态信息只出现在这里-也不是在指标栏

第4节：组件状态网格```html
<table class="component-grid">
  <tr><th>Component</th><th>Type</th><th>Status</th><th>Source Files</th></tr>
  [REPEAT: one row per component with color-coded status badge]
  <tr><td>[FILL]</td><td>[FILL]</td><td><span class="badge-[FILL: status]">[FILL]</span></td><td>[FILL]</td></tr>
  [END-REPEAT]
</table>
```
第5节：Threat/Finding状态分解```html
<div class="status-breakdown">
  [FILL: Grouped by status — Fixed items, New items, etc.]
  [REPEAT: Each item: ID | Title | Component | Status]
  [END-REPEAT]
</div>
```
第6部分：带delta的STRIDE热图```html
<table class="stride-heatmap">
  <thead>
    <tr>
      <th>Component</th>
      <th>S</th><th>T</th><th>R</th><th>I</th><th>D</th><th>E</th><th>A</th>
      <th>Total</th>
      <th class="divider"></th>
      <th>T1</th><th>T2</th><th>T3</th>
    </tr>
  </thead>
  <tbody>
    [REPEAT: one row per component]
    <tr>
      <td>[FILL: component]</td>
      <td>[FILL: S value] [FILL: delta indicator ▲/▼]</td>
      ... [same for T, R, I, D, E, A, Total] ...
      <td class="divider"></td>
      <td>[FILL: T1]</td><td>[FILL: T2]</td><td>[FILL: T3]</td>
    </tr>
    [END-REPEAT]
  </tbody>
</table>
```
**必须有13列：组件+ S + T + R + I + D + E + A +总+除法+ T1 + T2 + T3**

第7部分：需求验证```html
<div class="needs-verification">
  [REPEAT: items where analysis disagrees with old report]
  [FILL: item description]
  [END-REPEAT]
</div>
```
第8节：页脚```html
<div class="footer">
  Model: [FILL] | Duration: [FILL]
  Baseline: [FILL: folder] at [FILL: SHA]
  Generated: [FILL: timestamp]
</div>
```
---

**固定CSS变量（使用在`<style>`块）：**```css
--red: #dc3545;    /* new vulnerability */
--green: #28a745;  /* fixed/improved */
--amber: #fd7e14;  /* previously unidentified */
--gray: #6c757d;   /* still present */
--accent: #2171b5; /* modified/info */
```
固定规则:* * * *
-所有的CSS在内联`<style>`块-没有外部样式表
-包括`@media print`样式
-热图必须有T1/T2/T3列后分隔
-指标栏必须包括信任边界
-状态数据在卡片-不重复在指标栏
- HTMLthreat/finding总数必须匹配markdown STRIDE汇总总数