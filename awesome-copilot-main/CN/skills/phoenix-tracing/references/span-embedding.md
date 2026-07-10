#嵌入跨度

# #目的

嵌入跨度表示向量生成操作（用于语义搜索的文本到向量转换）。

##必需属性

|属性|类型|描述|必选||-----------|------|-------------|----------|
|`openinference.span.kind`| String |必须为“EMBEDDING”|是|
|`embedding.model_name`| String |嵌入模型标识|推荐使用|

##属性引用

单个嵌入

|属性|类型|描述||-----------|------|-------------|
|`embedding.model_name`|字符串|嵌入模型标识|
|`embedding.text`| String |用于嵌入|的输入文本
|`embedding.vector`| String （JSON数组）|生成嵌入向量|

* *的例子:* *```json
{
  "embedding.model_name": "text-embedding-ada-002",
  "embedding.text": "What is machine learning?",
  "embedding.vector": "[0.023, -0.012, 0.045, ..., 0.001]"
}
```
批嵌入

|属性模式|类型|描述||-------------------|------|-------------|
|`embedding.embeddings.{i}.embedding.text`| String |索引i |的文本
|`embedding.embeddings.{i}.embedding.vector`|字符串（JSON数组）|索引i的向量|

* *的例子:* *```json
{
  "embedding.model_name": "text-embedding-ada-002",
  "embedding.embeddings.0.embedding.text": "First document",
  "embedding.embeddings.0.embedding.vector": "[0.1, 0.2, 0.3, ..., 0.5]",
  "embedding.embeddings.1.embedding.text": "Second document",
  "embedding.embeddings.1.embedding.vector": "[0.6, 0.7, 0.8, ..., 0.9]"
}
```
矢量格式

存储为JSON数组字符串的向量：
-尺寸：通常为384、768、1536或3072
—格式：`"[0.123, -0.456, 0.789, ...]"`—精度：通常为小数点后3-6位

* *存储注意事项:* *
-大向量可以显著增加跟踪大小
-考虑在生产中省略向量（保留`embedding.text`用于调试）
-使用单独的矢量数据库进行实际的相似度搜索

# #的例子

单个嵌入```json
{
  "openinference.span.kind": "EMBEDDING",
  "embedding.model_name": "text-embedding-ada-002",
  "embedding.text": "What is machine learning?",
  "embedding.vector": "[0.023, -0.012, 0.045, ..., 0.001]",
  "input.value": "What is machine learning?",
  "output.value": "[0.023, -0.012, 0.045, ..., 0.001]"
}
```
批嵌入```json
{
  "openinference.span.kind": "EMBEDDING",
  "embedding.model_name": "text-embedding-ada-002",
  "embedding.embeddings.0.embedding.text": "First document",
  "embedding.embeddings.0.embedding.vector": "[0.1, 0.2, 0.3]",
  "embedding.embeddings.1.embedding.text": "Second document",
  "embedding.embeddings.1.embedding.vector": "[0.4, 0.5, 0.6]",
  "embedding.embeddings.2.embedding.text": "Third document",
  "embedding.embeddings.2.embedding.vector": "[0.7, 0.8, 0.9]"
}
```
