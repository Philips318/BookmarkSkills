#检索说明

这个文件定义了代理如何使用两层检索策略回答查询：
**维基优先**（快速路径），然后**带证据的图遍历**（深度路径）。

---

# #概述

检索有7个步骤：

1. 解析查询
2. **先检查wiki **（快速路径）
3. 在图中找到种子节点
4. 通过BFS展开图形
5. 处理噪声节点
6. 构建带有出处的子图
7. 返回结构化上下文

---

##第一步：解析查询

读取查询字符串并识别：
关键名词短语：潜在的实体名称（例如，“系统崩溃”，“内存泄漏”）
- **关键字**：个别有意义的单词（例如，“crash”，“leak”，“memory”）
-将所有术语规范化为小写的**

忽略stopwords(例如,“”,“”,“是”,“为什么”,“”,“如何”,“什么”)。

---

##第二步：先检查Wiki（快速路径）在触摸图表之前，搜索wiki。维基包含汇编的知识
交叉参考已经解决了，矛盾标记了，综合资料写好了。```python
from scripts.tools import wiki_store

results = wiki_store.search_wiki(query)
```
对于每个相关结果，请阅读以下页面：```python
content = wiki_store.read_page_by_slug(result["slug"])
```
**如果维基有足够的答案：**
-从wiki页面合成。
-引用源页面（例如，“根据[[memory-leak]]和[[system-crash]]…”）。
-如果答案有价值且尚未被捕获，则将其归档为新的wiki主题页；  ```python
  wiki_store.write_page(category="topic", title="Why System Crashes", content=..., summary=...)
  ```
- **返回早期** -不需要遍历图。

**如果维基答案不完整或缺失：**继续步骤3。

---

步骤3：找到种子节点

使用原始查询字符串调用`index_store.search(query)`。

这将返回与实体名称或关键字匹配的节点id。

如果没有找到种子节点：
—尝试使用步骤1中的单个关键字进行搜索。
-如果仍然没有结果，返回一个空子图：“ no relevant entities found.”

---

##步骤4：展开图形（BFS）`retrieval_engine.retrieve(seed_node_ids, depth=2)`打电话。

种子节点BFS：
- **深度1**：直接邻居
- **深度2**：邻居的邻居

规则:
-只遍历置信度≥MIN_CONFIDENCE的边（来自config.py）
-不要越过深度2
—收集所有访问节点id

---

步骤5：修剪节点限制节点总数为MAX_NODES（从config.py）
——优先考虑:
1. 种子节点（总是包含）
2. 深度1的节点
3. 深度为2的节点（如空间允许）
-删除弱连接的节点（边缘置信度< MIN_CONFIDENCE）

---

步骤6：用出处构建子图

对于标准查询，请调用：```python
subgraph = skill.query(query)
# Returns: {"nodes": {node_id: {name, type, source_document, source_chunks}},
#           "edges": [{source, target, type, confidence, source_document, supporting_text, chunk_id}]}
```
查询需要证据（引用，事实核查），请致电：```python
result = skill.query_with_evidence(query)
# Returns:
# {
#   "query": str,
#   "subgraph": {"nodes": {...}, "edges": [...]},
#   "supporting_documents": [
#     {
#       "doc_id": str,
#       "doc_title": str,
#       "supporting_chunks": [{"chunk_id": str, "text": str}, ...]
#     }
#   ],
#   "evidence_chain": "memory leak --[causes]--> system crash"
# }
```
---

步骤7：返回结构化上下文

返回结果：
- **子图**：节点+边（图的答案）
- **支持文档**：证明每个关系的源块
- **证据链**：人类可读的路径摘要
- **Wiki引用**：链接到在步骤2中找到的相关Wiki页面

**如果有价值，将答案提交回wiki:**```python
wiki_store.write_page(
    category="topic",
    title=query,
    content=f"# {query}\n\n**Evidence chain:** {result['evidence_chain']}\n\n...",
    summary="...",
)
```
这样，以后对同一主题的查询可以立即在wiki中找到答案。

---

# #规则

-永远不要制造图中不存在的节点或边
-永远不要穿越深度超过深度2
-总是在图表之前检查wiki （wiki-first）
-总是在结果中包含种子节点，即使它们没有边
-在修剪时更喜欢具有较高信心的边缘
-将有价值的答案作为主题页归档到wiki中
—如果没有找到相关节点，返回空子图（不是错误）