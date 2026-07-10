#骨架：1-threatmodel.md> **⛔复制下面的模板内容（不包括外部代码栏）。替换`[FILL]`占位符。“`.md`”和“`.mmd`”中的“图”必须相同
> **⛔数据流表列：`ID | Source | Target | Protocol | Description`。不要将`Target`重命名为`Destination`。不要重新排序列
> **⛔信任边界表列：`Boundary | Description | Contains`（3列）。不要添加`Name`列或将`Contains`重命名为`Components Inside`.**

---````markdown
# Threat Model

## Data Flow Diagram

```mermaid
[FILL：从1.1-threatmodel.mmd复制精确内容]```

## Element Table

| Element | Type | TMT Category | Description | Trust Boundary |
|---------|------|--------------|-------------|----------------|
[CONDITIONAL: For K8s apps with sidecars, add a `Co-located Sidecars` column after Trust Boundary]
[REPEAT: one row per element]
| [FILL] | [FILL: Process / External Interactor / Data Store] | [FILL: SE.P.TMCore.* / SE.EI.TMCore.* / SE.DS.TMCore.*] | [FILL] | [FILL] |
[END-REPEAT]

## Data Flow Table

| ID | Source | Target | Protocol | Description |
|----|--------|--------|----------|-------------|
[REPEAT: one row per data flow]
| [FILL: DF##] | [FILL] | [FILL] | [FILL] | [FILL] |
[END-REPEAT]

## Trust Boundary Table

| Boundary | Description | Contains |
|----------|-------------|----------|
[REPEAT: one row per trust boundary]
| [FILL] | [FILL] | [FILL: comma-separated component list] |
[END-REPEAT]

[CONDITIONAL: Include ONLY if summary diagram was generated (elements > 15 OR boundaries > 4)]

## Summary View

```mermaid
[FILL：从1.2-threatmodel-summary.mmd中复制精确内容]```

## Summary to Detailed Mapping

| Summary Element | Contains | Summary Flows | Maps to Detailed Flows |
|-----------------|----------|---------------|------------------------|
[REPEAT]
| [FILL] | [FILL] | [FILL: SDF##] | [FILL: DF##, DF##] |
[END-REPEAT]

[END-CONDITIONAL]
````
固定规则:* * * *
-详细流程使用`DF01`，`DF02`；`SDF01`，`SDF02`表示汇总流
—“元素类型”：只能是`Process`、`External Interactor`或`Data Store`—TMT类别：必须是tmt-element-taxonomy.md的特定ID（例如，`SE.P.TMCore.WebSvc`）。