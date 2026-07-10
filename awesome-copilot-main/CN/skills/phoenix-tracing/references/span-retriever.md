#猎犬跨度

# #目的

retriver跨表示document/context检索操作（向量DB查询、语义搜索、关键字搜索）。

##必需属性

|属性|类型|描述|必选||-----------|------|-------------|----------|
|`openinference.span.kind`| String |必须为“retriver”|是|

##属性引用

# # #查询

|属性|类型|描述||-----------|------|-------------|
|`input.value`|字符串|搜索查询文本|

文档模式

|属性模式|类型|描述||-------------------|------|-------------|
|`retrieval.documents.{i}.document.id`| String |文档唯一标识|
|`retrieval.documents.{i}.document.content`|字符串|文档文本内容|
|`retrieval.documents.{i}.document.score`|浮动|关联评分（0-1或距离）|
|`retrieval.documents.{i}.document.metadata`| String (JSON) |文档元数据|

文档扁平化模式

使用零索引表示法对文档进行扁平化：```
retrieval.documents.0.document.id
retrieval.documents.0.document.content
retrieval.documents.0.document.score
retrieval.documents.1.document.id
retrieval.documents.1.document.content
retrieval.documents.1.document.score
...
```
文档元数据

常用元数据字段（存储为JSON字符串）：```json
{
  "source": "knowledge_base.pdf",
  "page": 42,
  "section": "Introduction",
  "author": "Jane Doe",
  "created_at": "2024-01-15",
  "url": "https://example.com/doc",
  "chunk_id": "chunk_123"
}
```
**元数据示例：**```json
{
  "retrieval.documents.0.document.id": "doc_123",
  "retrieval.documents.0.document.content": "Machine learning is a method of data analysis...",
  "retrieval.documents.0.document.score": 0.92,
  "retrieval.documents.0.document.metadata": "{\"source\": \"ml_textbook.pdf\", \"page\": 15, \"chapter\": \"Introduction\"}"
}
```
# # #订购

文档按索引（0,1,2，…）排序。通常:
—索引0 =得分最高的文档
-指数1 =第二高
——等等。

在您的扁平属性中保持检索顺序。

###大型文档处理

对于非常长的文件：
—考虑将`document.content`截断为前N个字符
-在单独的文档存储中存储完整的内容
—使用“`document.id`”引用完整内容

# #的例子

基本矢量搜索```json
{
  "openinference.span.kind": "RETRIEVER",
  "input.value": "What is machine learning?",
  "retrieval.documents.0.document.id": "doc_123",
  "retrieval.documents.0.document.content": "Machine learning is a subset of artificial intelligence...",
  "retrieval.documents.0.document.score": 0.92,
  "retrieval.documents.0.document.metadata": "{\"source\": \"textbook.pdf\", \"page\": 42}",
  "retrieval.documents.1.document.id": "doc_456",
  "retrieval.documents.1.document.content": "Machine learning algorithms learn patterns from data...",
  "retrieval.documents.1.document.score": 0.87,
  "retrieval.documents.1.document.metadata": "{\"source\": \"article.html\", \"author\": \"Jane Doe\"}",
  "retrieval.documents.2.document.id": "doc_789",
  "retrieval.documents.2.document.content": "Supervised learning is a type of machine learning...",
  "retrieval.documents.2.document.score": 0.81,
  "retrieval.documents.2.document.metadata": "{\"source\": \"wiki.org\"}",
  "metadata.retriever_type": "vector_search",
  "metadata.vector_db": "pinecone",
  "metadata.top_k": 3
}
```
