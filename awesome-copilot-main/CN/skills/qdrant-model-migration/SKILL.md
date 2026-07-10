---
name: qdrant-model-migration
description: "Guides embedding model migration in Qdrant without downtime. Use when someone asks 'how to switch embedding models', 'how to migrate vectors', 'how to update to a new model', 'zero-downtime model change', 'how to re-embed my data', or 'can I use two models at once'. Also use when upgrading model dimensions, switching providers, or A/B testing models."
---
#改变嵌入模型时该怎么做

不同模型的向量是不兼容的。你不能在同一个向量空间中混合旧的和新的嵌入。也不能向现有集合添加新的命名向量字段。所有命名向量必须在集合创建时定义。下面的两种迁移策略都需要创建一个新集合。

-在选择策略之前了解集合别名[集合别名]（https://search.qdrant.tech/md/documentation/manage-data/collections/?s=collection-aliases）


我可以避免重新嵌入吗？

在提交完整迁移之前查找快捷方式时使用。

你必须重新嵌入：改变模型提供者（OpenAI到Cohere），改变架构（CLIP到BGE），不同模型之间不兼容的维度计数，或者将稀疏向量添加到仅密集集合。如果使用套娃模型（使用`dimensions`参数输出低维嵌入，从样本数据中学习线性变换，有一些召回损失，适用于100M+数据集），可以避免重新嵌入。或者改变量化（二进制到标量）：Qdrant自动重新量化。(量化)(https://search.qdrant.tech/md/documentation/manage-data/quantization/)


##需要零停机时间（别名交换）

当：生产必须保持可用时使用。建议大规模更换模型。

-用新模型的尺寸和距离度量创建一个新集合
-在后台将所有数据重新嵌入到新集合中
-将应用程序指向集合别名，而不是直接的集合名称
自动交换新集合的别名[Switch collection]（https://search.qdrant.tech/md/documentation/manage-data/collections/?s=switch-collection）
-验证搜索质量，然后删除旧集合

小心，别名交换只重定向查询。有效载荷必须单独重新上传。##需要两个模型同时运行（并排）

在A/B测试模型、多模态（密集+稀疏）或在提交前评估新模型时使用。

不能将命名向量添加到现有集合中。创建一个预先定义两个向量字段的新集合：

-创建新的集合与旧的和新的命名向量都定义[集合与多个向量]（https://search.qdrant.tech/md/documentation/manage-data/collections/?s=collection-with-multiple-vectors）
-从旧集合中迁移数据，保留旧命名字段中现有的向量
-使用`UpdateVectors`[更新向量]（https://search.qdrant.tech/md/documentation/manage-data/points/?s=update-vectors）增量地回填新模型嵌入
—通过查询`using: "old_model"`和`using: "new_model"`来比较质量
-交换别名到新的集合一旦满意将大型多向量（特别是ColBERT）与密集向量共同定位会降低所有查询的性能，即使是那些只使用密集的查询。在数百万个点上，用户报告在删除ColBERT后，延迟从13秒下降到25秒。在并行迁移期间将大向量放在磁盘上。

如果您预期将来的模型迁移，请在集合创建时预先定义这两个向量字段。


密集到混合搜索迁移

将sparse/BM25向量添加到现有的仅密集集合时使用。最常见的迁移模式。

不能将稀疏向量添加到现有的仅密集集合中。必须重新创建:

-创建新的集合与密集和稀疏的矢量配置定义
-重新嵌入所有数据与密集和稀疏模型
-迁移有效负载，交换别名块级稀疏向量具有不同于文档级的TF-IDF特征。测试迁移后的检索质量，特别是对于没有删除停止词的非英语文本。


重新嵌入太慢了

当：数据集很大，重新嵌入是瓶颈时使用。

-使用`update_mode: insert`（v1.17+）进行安全幂等迁移[更新模式]（https://search.qdrant.tech/md/documentation/manage-data/points/?s=update-mode）
-滚动旧集合与`with_vectors=False`，重新嵌入批量，向上插入到新的集合
-批量上传（每个请求64-256点，2-4个并行流）[批量上传]（https://search.qdrant.tech/md/documentation/tutorials-develop/bulk-upload/）
-在批量加载期间禁用HNSW（将`indexing_threshold_kb`设置得很高，之后恢复）
对于Qdrant Cloud inference，切换模型是一个配置更改，而不是一个管道更改[inference文档]（https://search.qdrant.tech/md/documentation/inference/）

对于400GB以上的数据集，预计需要几天。对于小数据集（<25MB），从源重新建立索引比使用迁移工具要快。


不要做什么-假设您可以将命名向量添加到现有集合中（必须在创建时定义）
—验证新采集前先删除旧采集
-忘记在应用程序代码中更新查询嵌入模型
-使用别名交换时跳过负载迁移（别名重定向查询，它们不复制数据）
-在长时间迁移期间保持ColBERT向量与密集向量共存（I/O成本降低了所有查询）
-迁移到混合搜索，而无需在块级别测试BM25质量