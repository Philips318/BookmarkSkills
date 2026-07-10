#评估者：RAG系统

RAG有两个不同的组件，需要不同的评估方法。

两阶段评估```
RETRIEVAL                    GENERATION
─────────                    ──────────
Query → Retriever → Docs     Docs + Query → LLM → Answer
         │                              │
    IR Metrics              LLM Judges / Code Checks
```
**首先使用IR指标调试检索，然后处理生成质量。

检索评价（IR Metrics）

使用传统的信息检索指标：

|度量|测量什么|| ------ | ---------------- |
| Recall@k |在所有相关文档中，前k位有多少？|
| Precision@k |检索到的文档中，有多少是相关的？|
第一份相关文件有多高？|
| NDCG |质量按位置加权|```python
# Requires query-document relevance labels
def recall_at_k(retrieved_ids, relevant_ids, k=5):
    retrieved_set = set(retrieved_ids[:k])
    relevant_set = set(relevant_ids)
    if not relevant_set:
        return 0.0
    return len(retrieved_set & relevant_set) / len(relevant_set)
```
创建检索测试数据

合成生成查询文档对：```python
# Reverse process: document → questions that document answers
def generate_retrieval_test(documents):
    test_pairs = []
    for doc in documents:
        # Extract facts, generate questions
        questions = llm(f"Generate 3 questions this document answers:\n{doc}")
        for q in questions:
            test_pairs.append({"query": q, "relevant_doc_id": doc.id})
    return test_pairs
```
##生成评价

使用LLM判断代码无法度量的质量：

| Eval |问题|| ---- | -------- |
b| **信度** |是否所有的声明都得到检索上下文的支持？|
答案是否解决了问题？|
| **完整性** |答案是否涵盖了上下文中的要点？|```python
from phoenix.evals import ClassificationEvaluator, LLM

FAITHFULNESS_TEMPLATE = """Given the context and answer, is every claim in the answer supported by the context?

<context>{{context}}</context>
<answer>{{output}}</answer>

"faithful" = ALL claims supported by context
"unfaithful" = ANY claim NOT in context

Answer (faithful/unfaithful):"""

faithfulness = ClassificationEvaluator(
    name="faithfulness",
    prompt_template=FAITHFULNESS_TEMPLATE,
    llm=LLM(provider="openai", model="gpt-4o"),
    choices={"unfaithful": 0, "faithful": 1}
)
```
## RAG故障分类

常见的失效模式评估：```yaml
retrieval_failures:
  - no_relevant_docs: Query returns unrelated content
  - partial_retrieval: Some relevant docs missed
  - wrong_chunk: Right doc, wrong section

generation_failures:
  - hallucination: Claims not in retrieved context
  - ignored_context: Answer doesn't use retrieved docs
  - incomplete: Missing key information from context
  - wrong_synthesis: Misinterprets or miscombines sources
```
##评估顺序

1. **首先检索** -如果错误的文档，生成将失败
2. **忠诚** -答案是否基于上下文？
3. **回答质量** -回答是否针对问题？

在调试生成之前修复检索问题。