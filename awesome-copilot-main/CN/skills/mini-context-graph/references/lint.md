# Lint说明

此文件定义wiki运行状况检查工作流。

定期（或在大量摄取之后）运行此操作以保存wiki
干净、准确。这个模式来自Karpathy的法学硕士维基：发现矛盾，
孤儿，断裂的链接，陈旧的索赔和数据缺口。

---

##什么时候跑

-摄取5+文件后
-当用户询问“检查wiki”或“健康检查”时
-当答案似乎不一致或矛盾时
-在一个重要的综合或陈述之前

---

##步骤1：运行自动运行状况检查```python
from scripts.tools import wiki_store

issues = wiki_store.lint_wiki()
# Returns:
# {
#   "orphan_pages": [list of slugs in files but not in index],
#   "missing_pages": [list of slugs in index but file deleted],
#   "broken_wikilinks": {slug: [broken link targets]},
#   "isolated_pages": [slugs with no wikilinks at all],
# }
```
---

##步骤2：分类每个问题类型

###孤儿页
页存在于磁盘上，但不在索引中。他们是看不见的搜索。
**修复**：添加到索引或删除，如果陈旧。```python
# To add to index, re-write the page (this auto-updates the index):
wiki_store.write_page(category="...", title="...", content=existing_content)

# To delete (manual step — confirm with user first):
# rm wiki/{category}/{slug}.md
```
###丢失的页面
在索引中，但是文件被删除了。悬空的引用。
**修复**：要么从知识中重新创建页面，要么从索引中删除。

破损的维基链接`[[slug]]`引用指向不存在的页面。
**修复**：创建缺失的页面，或更正链接。

隔离的页面
没有`[[wikilinks]]`的页面—它们无法通过链接遍历访问。
**修正**：添加链接from/to相关页面。

---

步骤3：检查矛盾

阅读维基索引，扫描可能相互矛盾的页面：```python
pages = wiki_store.list_pages()
# Returns [{slug, category, summary, date}, ...]
```
寻找:
—同一实体，不同页面的`type`冲突
-相同的关系，不同的页面不同的方向
-新摄取的update/supersede旧声称

当你发现矛盾时：**
-添加一个`## Contradictions`部分到相关的entity/topic页面：  ```markdown
  ## Contradictions
  - doc_001 says X; doc_003 says not-X — unresolved
  ```
-在日志中标记它：  ```python
  # Handled by wiki_store.write_page which auto-appends to log.md
  ```
---

##步骤4：检查过期索赔

回顾超过N天前摄取的页面（使用索引中的`date`字段）。
问：“是否有新的文件取代了这一声明？”

**声明过期时：**
-更新页面：添加`## Superseded`部分或更新正文。
-用_（由[newer-doc-summary]]取代）_标记旧声明。

---

步骤5：检查缺失的交叉引用

对于每个实体页面，检查：它是否链接到提到它的所有摘要页面？
对于每个摘要页面，检查：它是否链接到它提取的所有实体页面？

**修复**：阅读页面并添加缺失的`[[slug]]`链接。

---

##步骤6：识别数据缺口

检查缺少的实体页面：
-适当的描述（只是存根）
-任意`## Relations`截面
-任何`## Mentioned in`链接

这些都是更深入研究或新摄入的候选者。

---

##步骤7：记录Lint Pass```python
# wiki_store.write_page automatically logs the activity.
# For a manual lint summary, append to log.md via write_page on a topic:
wiki_store.write_page(
    category="topic",
    title="Lint Pass YYYY-MM-DD",
    content="# Lint Pass\n\n## Issues Found\n\n...\n\n## Fixed\n\n...",
    summary="Lint pass results",
)
```
---

##快速检测命令```python
from scripts.tools import wiki_store

# Full health check
issues = wiki_store.lint_wiki()

# Get recent history
log = wiki_store.get_log(last_n=10)

# List all pages
all_pages = wiki_store.list_pages()

# Search for a concept across wiki
results = wiki_store.search_wiki("memory leak")
```
---

# #规则

-未经用户确认绝不删除页面
-永远不要自动解决矛盾-标记它供人工审查
-将所有lint结果作为wiki的主题页面（因此历史是可见的）
-喜欢添加交叉引用重写现有的内容