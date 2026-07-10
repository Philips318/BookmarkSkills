# ranker跨度

# #目的

RERANKER跨表示检索文档的重新排序（coherence Rerank，交叉编码器模型）。

##必需属性

|属性|类型|描述|必选||-----------|------|-------------|----------|
|`openinference.span.kind`| String |必须为“RERANKER” |是|

##属性引用

###重新排序参数

|属性|类型|描述||-----------|------|-------------|
|`reranker.model_name`| String | rerank模型标识符|
|`reranker.query`| String | |重排序查询
|`reranker.top_k`| Integer |要返回|的文档数

输入文档

|属性模式|类型|描述||-------------------|------|-------------|
|`reranker.input_documents.{i}.document.id`|字符串|输入文档ID |
|`reranker.input_documents.{i}.document.content`|字符串|输入文档内容|
|`reranker.input_documents.{i}.document.score`|浮动|原始检索分数|
|`reranker.input_documents.{i}.document.metadata`| String (JSON) |文档元数据|

输出文档

|属性模式|类型|描述||-------------------|------|-------------|
|`reranker.output_documents.{i}.document.id`| String |输出文档ID（重新排序）|
|`reranker.output_documents.{i}.document.content`|字符串|输出文档内容|
|`reranker.output_documents.{i}.document.score`|浮动|新排名分数|
|`reranker.output_documents.{i}.document.metadata`| String (JSON) |文档元数据|

###分数比较

输入分数（来自寻回者）与输出分数（来自重新排序者）：```json
{
  "reranker.input_documents.0.document.id": "doc_A",
  "reranker.input_documents.0.document.score": 0.7,
  "reranker.input_documents.1.document.id": "doc_B",
  "reranker.input_documents.1.document.score": 0.9,
  "reranker.output_documents.0.document.id": "doc_B",
  "reranker.output_documents.0.document.score": 0.95,
  "reranker.output_documents.1.document.id": "doc_A",
  "reranker.output_documents.1.document.score": 0.85
}
```
在这个例子中：
—输入：doc_B（0.9）排名高于doc_A （0.7）
-输出：doc_B仍然是最高的，但两个分数都增加了
重新排序者确认了寻回犬的排序，但修改了分数

# #的例子

完成重新排名示例```json
{
  "openinference.span.kind": "RERANKER",
  "reranker.model_name": "cohere-rerank-v2",
  "reranker.query": "What is machine learning?",
  "reranker.top_k": 2,
  "reranker.input_documents.0.document.id": "doc_123",
  "reranker.input_documents.0.document.content": "Machine learning is a subset...",
  "reranker.input_documents.1.document.id": "doc_456",
  "reranker.input_documents.1.document.content": "Supervised learning algorithms...",
  "reranker.input_documents.2.document.id": "doc_789",
  "reranker.input_documents.2.document.content": "Neural networks are...",
  "reranker.output_documents.0.document.id": "doc_456",
  "reranker.output_documents.0.document.content": "Supervised learning algorithms...",
  "reranker.output_documents.0.document.score": 0.95,
  "reranker.output_documents.1.document.id": "doc_123",
  "reranker.output_documents.1.document.content": "Machine learning is a subset...",
  "reranker.output_documents.1.document.score": 0.88
}
```
