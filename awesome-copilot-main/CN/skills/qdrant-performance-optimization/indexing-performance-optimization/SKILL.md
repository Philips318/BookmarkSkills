---
name: qdrant-indexing-performance-optimization
description: "Diagnoses and fixes slow Qdrant indexing and data ingestion. Use when someone reports 'uploads are slow', 'indexing takes forever', 'optimizer is stuck', 'HNSW build time too long', or 'data uploaded but search is bad'. Also use when optimizer status shows errors, segments won't merge, or indexing threshold questions arise."
---
# Qdrant索引太慢怎么办

Qdrant不会立即建立HNSW指数。小段使用暴力，直到它们超过`indexing_threshold_kb`（默认：20 MB）。在此窗口期间的搜索速度较慢，这是设计原因，而不是错误。

-了解索引优化器[索引优化器]（https://search.qdrant.tech/md/documentation/operations/optimizer/?s=indexing-optimizer）


太慢了

当：上传或upsert API调用缓慢时使用。
识别瓶颈：客户端（网络、批处理）vs服务器端（CPU、磁盘I/O）

对于客户端，优化批处理和并行性：

-使用批量启动（每个请求64-256分）[积分API]（https://search.qdrant.tech/md/documentation/manage-data/points/?s=upload-points）
—使用2-4个并行上传流

对于服务器端，优化Qdrant配置和索引策略：

-创建更多的分片（3-12），每个分片有一个独立的更新工作者[Sharding]（https://search.qdrant.tech/md/documentation/operations/distributed_deployment/?s=sharding）
-在HNSW构建之前创建有效载荷索引（需要过滤向量索引）[有效载荷索引]（https://search.qdrant.tech/md/documentation/manage-data/indexing/?s=payload-index）适用于大数据集的初始批量负载：

-在批量加载期间禁用HNSW（将`indexing_threshold_kb`设置为非常高，之后恢复）[收集参数]（https://search.qdrant.tech/md/documentation/manage-data/collections/?s=update-collection-parameters）
-设置`m=0`来禁用HNSW是遗留的，使用高`indexing_threshold_kb`代替

小心、快速的无索引上传可能会暂时使用更多的RAM，并降低搜索性能，直到优化器赶上来。

看到https://search.qdrant.tech/md/documentation/tutorials-develop/bulk-upload/优化器卡住或花费太长时间

使用时：优化器运行了几个小时，没有完成。

-通过优化端点检查实际进度（v1.17+）[优化监控]（https://search.qdrant.tech/md/documentation/operations/optimizer/?s=optimization-monitoring）
-大型合并和HNSW重建在大数据集上需要花费数小时
-检查CPU和磁盘I/O（HNSW是CPU绑定，合并是I/O-bound，硬盘不可用）
—如果“`optimizer_status`”显示错误，请检查磁盘是否已满或磁盘段是否损坏


## HNSW构建时间太高

当：HNSW索引构建占总索引时间的大部分时使用。-减少`m`（默认16，大多数情况下都很好，很少需要32+）[HNSW参数]（https://search.qdrant.tech/md/documentation/manage-data/indexing/?s=vector-index）
-减少`ef_construct`（100-200足够）[HNSW config]（https://search.qdrant.tech/md/documentation/manage-data/collections/?s=indexing-vectors-in-hnsw）
保持`max_indexing_threads`与CPU核数成比例[Configuration]（https://search.qdrant.tech/md/documentation/operations/configuration/）
-使用GPU索引[GPU索引]（https://search.qdrant.tech/md/documentation/operations/running-with-gpu/）

多租户集合的HNSW索引

如果你有一个多租户用例，其中所有数据都被一些有效负载字段分割（例如`tenant_id`），你可以避免构建全局HNSW索引，而是依赖`payload_m`仅为数据子集构建HNSW索引。
跳过全局HNSW索引可以显著减少索引时间。

详细信息请参见[多租户集合]（https://search.qdrant.tech/md/documentation/manage-data/multitenancy/）。

附加负载索引太慢Qdrant为所有有效载荷索引构建额外的HNSW链接，以确保过滤向量搜索的质量不会降低。
一些有效负载索引（例如带有长文本的`text`字段）每个点可能有非常多的唯一值，这可能导致HNSW构建时间很长。

您可以禁用为特定负载索引构建额外的HNSW链接，而依赖于ACORN等稍慢的查询时间策略。

在[文档]中阅读更多关于禁用额外HNSW链接的信息（https://search.qdrant.tech/md/documentation/manage-data/indexing/?s=disable-the-creation-of-extra-edges-for-payload-fields）

在[文档]中阅读更多关于ACORN的信息（https://search.qdrant.tech/md/documentation/search/search/?s=acorn-search-algorithm）


不要做什么

-在HNSW构建后不要创建有效载荷索引（破坏可过滤向量索引）
-不要使用`m=0`批量上传到现有的集合，它可能会丢失现有的HNSW，并导致长索引
-不要一次上传一个点（每个请求的开销占主导地位）