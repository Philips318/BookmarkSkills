---
name: mini-context-graph
description: |
  A persistent, compounding knowledge base combining Karpathy's LLM Wiki pattern
  with a structured knowledge graph. Ingest documents once — the LLM writes wiki
  pages, extracts entities/relations into the graph, and stores raw content for
  evidence retrieval. Knowledge accumulates and cross-references; it is never
  re-derived from scratch.
---
#迷你情境图技能

##核心理念

标准RAG在每个查询中从头开始重新发现知识。这个技能是不同的：

1. **Wiki层** - LLM编写和维护持久的标记页面（摘要，实体页面，主题合成）。交叉参考已经存在了。维基的内容越来越丰富。
2. **图层-实体和关系被提取一次并存储为可导航的知识图。BFS遍历在不重新读取源的情况下回答结构查询。
3. **原始源层** -原始文档不可变地存储与块。出处链接将每个图节点和边连接回支持它的确切文本。

> LLM写；Python工具处理所有的记账。

---

三层

|层|其中| LLM做什么| Python做什么||-------|-------|-------------------|-----------------|
| **原始资源** |`data/documents.json`|读取（从不修改）|存储块+元数据|
| **Wiki** |`wiki/`(markdown) |Writes/updates页面|管理index.md+log.md|
| **图** |`data/graph.json`|提取实体+关系|持久化，重复数据删除，遍历|

---

##⚡代理快速入门```python
from scripts.contextgraph import ContextGraphSkill
from scripts.tools import wiki_store

skill = ContextGraphSkill()

# ===== INGEST WITH FULL RAG + WIKI =====
# 1. Read references/ingestion.md and references/ontology.md first
# 2. Extract entities and relations (LLM reasoning step)
entities = [
    {"name": "memory leak",   "type": "issue",  "supporting_text": "memory leaks cause crashes"},
    {"name": "system crash",  "type": "issue",  "supporting_text": "system crashes due to memory leaks"},
]
relations = [
    {"source": "memory leak", "target": "system crash", "type": "causes",
     "confidence": 1.0, "supporting_text": "System crashes due to memory leaks."},
]

result = skill.ingest_with_content(
    doc_id="doc_001",
    title="System Crash Analysis",
    source="/docs/incident_report.pdf",
    raw_content="System crashes due to memory leaks. Memory leaks occur when objects are not released.",
    entities=entities,
    relations=relations,
)
# result = {"doc_id": "doc_001", "chunk_count": 1, "nodes_added": 2, "edges_added": 1}

# 3. Write a wiki summary page for this document
wiki_store.write_page(
    category="summary",
    title="System Crash Analysis Summary",
    content="""---
title: System Crash Analysis
source_document: doc_001
tags: [summary, incident]
---

# System Crash Analysis

**Source:** incident_report.pdf

## Key Claims

- [[memory-leak]] causes [[system-crash]] (confidence: 1.0)

## Entities

- [[memory-leak]] (issue)
- [[system-crash]] (issue)
""",
    summary="Incident report: memory leaks cause system crashes.",
)

# ===== QUERY WITH EVIDENCE =====
result = skill.query_with_evidence("Why does the system crash?")
# Returns: {"query": ..., "subgraph": ..., "supporting_documents": [...], "evidence_chain": ...}

# ===== WIKI SEARCH (read wiki before answering) =====
pages = wiki_store.search_wiki("memory leak")
# Returns: [{slug, category, path, snippet}, ...]
```
---

# #操作

# # #摄取

当用户提供新文档时：

1. 读取`references/ingestion.md`-entity/relation提取规则。
2. 读取`references/ontology.md`类型的规范化规则。
3. 使用LLM推理提取实体和关系。
4. 调用`skill.ingest_with_content(...)`-存储原始内容+块+图节点+来源。
5. **使用`wiki_store.write_page(category="summary", ...)`编写wiki摘要页面**。
6. **更新实体页** -为每个new/updated实体，写或更新`wiki_store.write_page(category="entity", ...)`。
7. **更新主题页**如果文档触及一个现有的合成主题。
8. 一个文档摄取通常会触及3-10个wiki页面。

# # #查询

当用户提出问题时：1. **先查看维基** -`wiki_store.search_wiki(query)`查找相关页面。读它们。
2. 如果wiki有一个好的答案，从wiki页面合成（快速路径）。
3. 如果需要更深入的图遍历，调用`skill.query_with_evidence(query)`。
4. 返回答案和来自`supporting_documents`的证据引用。
5. 如果答案是有价值的，把它作为一个新的维基主题页归档。

# # #线头

定期检查wiki的运行状况：```python
from scripts.tools import wiki_store
issues = wiki_store.lint_wiki()
# Returns: {orphan_pages, missing_pages, broken_wikilinks, isolated_pages}
```
请法学硕士审查和修复：断裂的链接，孤儿页面，陈旧的索赔，缺少交叉引用。查看`references/lint.md`了解完整的lint工作流程。

---

##摄入限制

-❌不要产生文本中没有出现的幻觉
-❌不要在没有明确的文本证据的情况下添加关系
-❌不要添加置信度< 0.6的边缘
-✅为每个实体和关系提供`supporting_text`-这允许出处
-✅为每个摄取的文档写一个wiki摘要页面
-✅更新现有的实体页面时，新的信息到达
-✅当新数据与旧声明冲突时，在wiki页面中标记矛盾

---

##检索约束

-🔒遍历深度不能超过2（配置：MAX_GRAPH_DEPTH）
-🔒仅限置信度≥0.6的边（config: MIN_CONFIDENCE）
-🔒最多返回50个节点（config: MAX_NODES）
-❌不要在图中构造节点或边缘

---

完整的Python API参考|方法|用途|何时使用||--------|---------|-------------|
|`skill.ingest_with_content(doc_id, title, source, raw_content, entities, relations)`|完整的RAG摄取：原始文档+图表+出处|每个新文档|
|`skill.add_node(name, node_type)`|添加单个实体（没有来源）|快速添加，没有源文档|
|`skill.add_edge(source_name, target_name, relation, confidence)`|添加单个关系|快速添加，不需要源文档|
|`skill.query(query)`|纯图检索→子图|结构查询|
|`skill.query_with_evidence(query)`|图+出处→子图+源块|需要引用的查询|
|`wiki_store.write_page(category, title, content, summary)`|Write/updatea wiki页面|每次摄取后；在回答查询|之后
|`wiki_store.read_page(category, title)`|阅读维基页面|再回答；用于交叉引用|
|`wiki_store.search_wiki(query)`|跨wiki关键字搜索|图遍历之前的快速路径|
|`wiki_store.list_pages(category)`|列出所有wiki页面|获取概述|
|`wiki_store.get_log(last_n)`|读取最近操作|了解wiki历史|
|`wiki_store.lint_wiki()`|健康检查|定期维护|
|`documents_store.list_documents()`|列出所有摄取的原始来源|审计/来源检查|
|`documents_store.search_chunks(query)`|块级搜索b|寻找具体证据b|---

##设计理念

b>“wiki是一个持久的复合工件。交叉参考已经在那里了。这种综合已经反映了你所读到的一切。”——Karpathy

|发生了什么|谁拥有它||-------|-----------|-------------|
| **LLM Reasoning** |提取、合成、编写wiki页面| Agent (. .Md指导文件)|
| **Wiki持久性** |索引、日志、文件I/O|`wiki_store.py`|
| **图持久化** |删除、索引、BFS遍历|`graph_store.py`、`retrieval_engine.py`|
|不可变文档+块+出处|`documents_store.py`|

人类整理资料并提出问题。法学硕士编写wiki，提取图表，并引用答案。Python处理所有的记账。