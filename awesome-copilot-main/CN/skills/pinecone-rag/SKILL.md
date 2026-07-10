---
name: pinecone-rag
description: >
  Build production RAG pipelines and persistent agent memory using Pinecone as
  the vector database backend. ALWAYS USE THIS SKILL when the user mentions
  Pinecone, wants to index documents for semantic search, build a
  retrieval-augmented generation system, store agent memory across sessions,
  implement hybrid search, or connect an LLM to a searchable knowledge base —
  even if they don't say "Pinecone" explicitly. Also use when the user asks
  about vector databases for RAG, namespace isolation for multi-tenant agents,
  embedding pipelines, or scaling a knowledge base beyond what local storage
  can handle. DO NOT use for local-only vector stores (Chroma, FAISS, pgvector)
  or pure keyword search with no semantic component.
license: Apache-2.0
compatibility: "pinecone>=6.0.0, Python 3.10+"
---
#松果RAG技能

该技能指导您构建生产RAG管道或持久性
代理存储系统采用松果。从头到尾遵循工作流程
在理解用户的实际需求之前，不要跳过步骤或直接跳到代码上
的需求。

在开始之前，先问一个问题

在编写任何代码之前，确定这两个用例中的哪一个适用：

**A - RAG over documents**：用户想要索引一个语料库(pdf，文档，代码，
网页)，并检索相关块，以地面LLM响应。

**B -代理内存**：用户希望代理记住事实，决策或
跨会话或跨多个代理共享知识库的上下文。

设置是相似的，但是名称空间策略和检索模式不同。
如果用户没有说，问：*“这是用于文档检索，代理内存，
还是两个?*然后按照下面的相关工作流程。

---##步骤1 -选择索引配置

在编写任何代码之前选择索引类型。说错了意思是
稍后重新创建索引。

**无服务器（推荐用于大多数情况）**```python
from pinecone import Pinecone, ServerlessSpec

pc = Pinecone(api_key="PINECONE_API_KEY")

if "my-index" not in pc.list_indexes().names():
    pc.create_index(
        name="my-index",
        dimension=1536,        # must match your embedding model exactly
        metric="cosine",
        spec=ServerlessSpec(cloud="aws", region="us-east-1")
    )
index = pc.Index("my-index")
```
**豆荚为基础（一致的高通量生产）**```python
from pinecone import PodSpec

pc.create_index(
    name="my-index-prod",
    dimension=1536,
    metric="cosine",
    spec=PodSpec(environment="us-east1-gcp", pod_type="p1.x1")
)
```
**尺寸快速参考-与您的嵌入模型完全匹配：**
|型号|尺寸||---|---|
|`text-embedding-3-small`| 1536 |
|`text-embedding-3-large`| 3072 |
|`voyage-3`/`voyage-multimodal-3`| 1024 |
|`BAAI/bge-large-en-v1.5`| 1024 |
|`intfloat/multilingual-e5-large`（阿拉伯语，马来语，中文）| 1024 |

b> **检查点**：索引存在，维度匹配嵌入模型，`index.describe_index_stats()`返回无错误。

---

##步骤2 -嵌入和更新文档

总是批量更新-永远不要一次更新一个向量。```python
from openai import OpenAI

client = OpenAI()

def embed(texts: list[str]) -> list[list[float]]:
    res = client.embeddings.create(model="text-embedding-3-small", input=texts)
    return [r.embedding for r in res.data]

def upsert_docs(index, docs: list[dict], namespace: str = "default"):
    """docs = [{"id": "...", "text": "...", "metadata": {...}}]"""
    BATCH = 100
    for i in range(0, len(docs), BATCH):
        batch = docs[i:i + BATCH]
        vecs = [
            {
                "id": d["id"],
                "values": emb,
                "metadata": {**d.get("metadata", {}), "text": d["text"]}
            }
            for d, emb in zip(batch, embed([d["text"] for d in batch]))
        ]
        index.upsert(vectors=vecs, namespace=namespace)
```
**始终将原始文本存储在元数据中** -这避免了第二次查找
在检索时。

> **检查点**:`index.describe_index_stats()`显示向量计数> 0在
>目标命名空间。

---

##步骤3 -选择检索策略

密集（语义）搜索-大多数情况下使用```python
def search(index, query: str, top_k: int = 5, namespace: str = "default",
           filter: dict = None) -> list[dict]:
    [q_emb] = embed([query])
    results = index.query(
        vector=q_emb, top_k=top_k, namespace=namespace,
        include_metadata=True, filter=filter
    )
    return [{"text": m.metadata["text"], "score": m.score, "id": m.id}
            for m in results.matches]
```
混合搜索（语义+ BM25关键字）-当语料库有确切的术语时使用
当领域包含语义搜索遗漏的精确术语时，使用hybrid：
法律引用，医疗代码，产品sku， API方法名称。```python
from pinecone_text.sparse import BM25Encoder

bm25 = BM25Encoder().default()
bm25.fit([d["text"] for d in docs])  # fit once on your corpus

def hybrid_search(index, query: str, top_k: int = 5, alpha: float = 0.7):
    """alpha=1.0 is pure dense; alpha=0.0 is pure sparse."""
    dense = [v * alpha for v in embed([query])[0]]
    sparse_raw = bm25.encode_queries(query)
    sparse = {
        "indices": sparse_raw["indices"],
        "values": [v * (1 - alpha) for v in sparse_raw["values"]]
    }
    return index.query(vector=dense, sparse_vector=sparse,
                       top_k=top_k, include_metadata=True).matches
```
元数据过滤——在语义排序之前对结果进行范围划分```python
# Exact match
results = index.query(vector=emb, filter={"source": {"$eq": "confluence"}})

# Combined filter
results = index.query(vector=emb, filter={
    "$and": [
        {"category": {"$eq": "engineering"}},
        {"language": {"$in": ["en", "ar"]}}
    ]
})
```
> **检查点**：一个测试查询返回分数> 0.7的相关结果
>清晰匹配的内容。

---

步骤4A -完整的RAG管道（文档用例）```python
def rag_answer(index, question: str, namespace: str = "default",
               model: str = "gpt-4o-mini") -> str:
    hits = search(index, question, top_k=5, namespace=namespace)
    context = "\n\n".join(h["text"] for h in hits)

    return client.chat.completions.create(
        model=model,
        messages=[
            {
                "role": "system",
                "content": (
                    "Answer using only the provided context. "
                    "If the answer isn't in the context, say so.\n\n"
                    f"Context:\n{context}"
                )
            },
            {"role": "user", "content": question}
        ]
    ).choices[0].message.content
```
---

##步骤4B -代理内存（内存用例）

使用名称空间完全隔离每个代理或用户的内存。
每个代理命名空间可防止跨用户或会话的内存溢出。```python
import time, hashlib

def remember(index, agent_id: str, content: str,
             memory_type: str = "fact"):
    """Store a memory for an agent."""
    mem_id = hashlib.md5(
        f"{agent_id}{content}{time.time()}".encode()
    ).hexdigest()
    [emb] = embed([content])
    index.upsert(
        vectors=[{
            "id": mem_id,
            "values": emb,
            "metadata": {
                "text": content,
                "type": memory_type,
                "timestamp": time.time(),
                "agent_id": agent_id
            }
        }],
        namespace=f"agent_{agent_id}"
    )

def recall(index, agent_id: str, query: str,
           top_k: int = 5) -> list[str]:
    """Recall relevant memories for an agent."""
    return [h["text"] for h in
            search(index, query, top_k=top_k,
                   namespace=f"agent_{agent_id}")]

def forget(index, agent_id: str):
    """Wipe all memories for an agent (e.g., on user request)."""
    index.delete(delete_all=True, namespace=f"agent_{agent_id}")
```
---

##步骤5 -连接在一起，测试端到端

在集成到更大的系统之前，运行一个快速冒烟测试：```python
# Smoke test
upsert_docs(index, [
    {"id": "t1", "text": "Pinecone is a vector database for semantic search."},
    {"id": "t2", "text": "RAG combines retrieval with language model generation."},
])

hits = search(index, "What is Pinecone?")
assert hits[0]["score"] > 0.7, f"Expected high similarity, got {hits[0]['score']}"
print("Smoke test passed:", hits[0]["text"])
```
b> **检查点**：冒烟测试通过。端到端：index→upsert→query→
> LLM响应工作无错误。

---

常见的陷阱-在它们变成bug之前修复它们

- **尺寸不匹配**：始终验证`len(embed(["test"])[0])`匹配
第一次反转前的索引维度。
- **元数据中缺少文本**：如果你不在元数据中存储`"text"`，
您将需要第二次查找以在查询时获得实际内容。
- **循环中的单向量翻转**：总是以100块为块进行批处理。
- **没有命名空间策略**：预先决定-每个user/agent一个命名空间
防止以后难以修复的跨租户数据泄漏。
- **在小型语料库上拟合BM25 **: BM25需要一个具有代表性的语料库
建立良好的术语频率。至少容纳几百个文档。

何时不使用此技能在以下情况下使用不同的方法：
-数据集适合内存和延迟无关→使用FAISS或Chroma
-你已经在PostgreSQL上，想要避免一个新的服务→使用pgvector
-您需要低于5ms的p99延迟，没有外部API调用→本地矢量存储
-用户明确希望使用不同的矢量DB （Weaviate， Qdrant等）