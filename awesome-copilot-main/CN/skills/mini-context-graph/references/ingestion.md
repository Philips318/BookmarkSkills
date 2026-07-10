#摄入说明

该文件定义代理如何从原始文档中提取实体和关系。

---

步骤1：阅读文档

仔细阅读所提供的文本。识别:
- **实体**：名词短语，指的是现实世界中的对象、系统、组件、参与者、概念或事件。
-关系：描述一个实体如何影响、包含、原因、使用或与另一个实体相关的动词短语。

---

步骤2：提取实体

对于每个实体：
-记录其**名称**（规范化：小写，条带leading/trailing空格）
—分配一个**类型**：一个短标签（1-3个字），用于对实体进行分类

实体类型示例

|实体名称|建议类型||-------------|---------------|
| Python解释器|软件|
|内存泄漏|问题|
|操作系统|系统|
|数据库|基础架构|
|用户|演员|
| API端点|接口|
|服务器|基础架构|

* *规则:* *
类型必须足够通用，以便跨文档重用
不要为每个实体创建唯一的类型（例如，避免使用`python-interpreter-type`）
—使用`ontology.md`规范化规则对类型进行规范化

---

步骤3：提取关系

对于文本中具有显式连接的每一对实体：
—记录**源**实体名称
—记录**目标**实体名称
—记录**关系类型**：动词或动词短语（规范化：小写）
-在0到1之间分配一个**信心**分数：
- 1.0 =明确陈述（“A导致B”）
- 0.8 =强烈暗示（“A链接到B”）
- 0.6 =弱暗示（“A可能影响B”）
- < 0.6 =不包括

---

##步骤4：输出格式按照下面的格式生成一个JSON对象：```json
{
  "entities": [
    { "name": "entity name", "type": "entity type", "supporting_text": "exact quote mentioning this entity" }
  ],
  "relations": [
    {
      "source": "source entity name",
      "target": "target entity name",
      "type": "relation type",
      "confidence": 0.9,
      "supporting_text": "exact quote that justifies this relation"
    }
  ]
}
```
`supporting_text`字段是**来源**所必需的。它必须是一字不差或几乎一字不差地引用提到或支持entity/relation.的文档，这是将图节点和边链接回其源的方法。

---

# #规则

—所有名称和类型必须是小写的**
-在实体列表中只包含两个实体同时存在的关系
-不要虚构文本不支持的实体或关系
-优先重用本体中已有的实体和关系类型，而不是创建新的
一个实体可以出现在多个关系中（作为源或目标）
-始终包括`supporting_text`-这使证据检索和审计跟踪

---

步骤5：编写Wiki页面（必填项）

调用`skill.ingest_with_content(...)`之后，你必须编写wiki页面：

# # # 5。为文档写一页摘要```python
from scripts.tools import wiki_store

wiki_store.write_page(
    category="summary",
    title=f"{title} Summary",
    content=f"""---
title: {title}
source_document: {doc_id}
tags: [summary]
---

# {title}

**Source:** {source}

## Key Claims

{chr(10).join(f'- [[{r["source"].replace(" ", "-")}]] {r["type"]} [[{r["target"].replace(" ", "-")}]] (confidence: {r["confidence"]})' for r in relations)}

## Entities

{chr(10).join(f'- [[{e["name"].replace(" ", "-")}]] ({e["type"]})' for e in entities)}

## Open Questions

- (Add questions from reading the document here)
""",
    summary=f"Summary of {title}",
)
```
# # # 5 b。编写或更新实体页

对于每个尚未在wiki中的新实体，编写一个实体页面：```python
wiki_store.write_page(
    category="entity",
    title=entity_name,
    content=f"""---
title: {entity_name}
type: {entity_type}
source_document: {doc_id}
tags: [{entity_type}]
---

# {entity_name}

(Description from the document or prior knowledge.)

## Relations

(List any wikilinks to related entities extracted from relations.)

## Mentioned in

- [[{doc_id}-summary]]
""",
    summary=f"{entity_name}: {entity_type}",
)
```
对于**现有的**实体页面，读取当前页面并添加新信息、更新关系或标记矛盾。

---

# #的例子

输入文档:* * * *```
System crashes due to memory leaks.
Memory leaks occur when objects are not released.
```
**期望提取输出：**```json
{
  "entities": [
    { "name": "system crash", "type": "issue",     "supporting_text": "system crashes due to memory leaks" },
    { "name": "memory leak",  "type": "issue",     "supporting_text": "memory leaks occur when objects are not released" },
    { "name": "object",       "type": "component", "supporting_text": "objects are not released" }
  ],
  "relations": [
    {
      "source": "memory leak",
      "target": "system crash",
      "type": "causes",
      "confidence": 1.0,
      "supporting_text": "System crashes due to memory leaks."
    },
    {
      "source": "object",
      "target": "memory leak",
      "type": "contributes to",
      "confidence": 0.9,
      "supporting_text": "Memory leaks occur when objects are not released."
    }
  ]
}
```
