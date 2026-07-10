---
name: qdrant-search-speed-optimization
description: "Diagnoses and fixes slow Qdrant search. Use when someone reports 'search is slow', 'high latency', 'queries take too long', 'low QPS', 'throughput too low', 'filtered search is slow', or 'search was fast but now it's slow'. Also use when search performance degrades after config changes or data growth."
---
#诊断问题

搜索性能下降有多种可能的原因。最常见的是：

*内存压力：如果工作集超过可用的RAM
*复杂的请求（例如高`hnsw_ef`，没有有效负载索引的复杂过滤器）
*竞争的后台进程（例如，批量上传后优化器仍在运行）
*集群问题（例如网络问题，硬件退化）


##单个查询太慢（Latency）

在以下情况下使用：无论负载如何，单个查询花费的时间都太长。

诊断步骤：

-检查相同请求的第二次运行是否明显更快（表明内存压力）
-对`with_payload: false`和`with_vectors: false`尝试相同的查询，看看负载检索是否是瓶颈
—如果请求使用过滤器，尝试逐个删除它们，以确定特定的过滤条件是否是瓶颈

###常见修复：-调整HNSW参数：[微调搜索]（https://search.qdrant.tech/md/documentation/operations/optimize/?s=fine-tuning-search-parameters）
-启用内存量化：[标量量化]（https://search.qdrant.tech/md/documentation/manage-data/quantization/?s=scalar-quantization）
-用套娃模型降低向量维数：[套娃模型]（https://search.qdrant.tech/md/documentation/inference/?s=reduce-vector-dimensionality-with-matryoshka-models）
-对高维向量使用过采样+重分[量化搜索]（https://search.qdrant.tech/md/documentation/manage-data/quantization/?s=searching-with-quantization）
—在Linux系统上启用io_uring (io_uring) （https://qdrant.tech/articles/io_uring/）


无法处理足够的QPS（吞吐量）

当系统无法在负载下每秒提供足够的查询时使用。

-减少段数（`default_segment_number`到2）[最大化吞吐量]（https://search.qdrant.tech/md/documentation/operations/optimize/?s=maximizing-throughput）
-使用批量搜索API代替单个查询[批量搜索]（https://search.qdrant.tech/md/documentation/search/search/?s=batch-search-api）
—启用量化降低CPU成本[标量量化]（https://search.qdrant.tech/md/documentation/manage-data/quantization/?s=scalar-quantization）
-添加副本来分配读负载[Replication]（https://search.qdrant.tech/md/documentation/operations/distributed_deployment/?s=replication）


过滤搜索很慢

当过滤后的搜索速度明显慢于未过滤时使用。记忆后最常见的SA症状。-在过滤字段[payload index]（https://search.qdrant.tech/md/documentation/manage-data/indexing/?s=payload-index）上创建负载索引
-使用`is_tenant=true`为主过滤条件：[租户索引]（https://search.qdrant.tech/md/documentation/manage-data/indexing/?s=tenant-index）
-尝试ACORN算法的复杂过滤器：[ACORN]（https://search.qdrant.tech/md/documentation/search/search/?s=acorn-search-algorithm）
—避免使用`nested`过滤条件作为主过滤器。它可能会强制qdrant读取原始有效负载值，而不是使用index。
—如果在HNSW构建后添加了负载索引，则触发重新索引以创建可过滤的子图链接


通过并行更新优化搜索性能

诊断步骤

-尝试使用`indexed_only=true`参数运行相同的查询，如果查询明显更快，这意味着优化器仍在运行，尚未对所有段进行索引。
—如果CPU或IO使用率很高，即使没有查询，也表明优化器仍在运行。

推荐的配置更改-减少`optimizer_cpu_budget`，为查询预留更多的CPU
—使用`prevent_unoptimized=true`来防止创建具有大量未索引数据的段用于搜索。相反，一旦段达到所谓的indexing_threshold，所有额外的点都将以“延迟状态”添加。

了解更多[在这里]（https://search.qdrant.tech/md/documentation/search/low-latency-search/?s=query-indexed-data-only）


不要做什么

-将`always_ram=false`设置为量化（每次搜索时磁盘抖动）
-将HNSW放在磁盘上用于延迟敏感的生产（仅用于冷存储）
增加吞吐量的段数（相反：越少=越好）
在每个字段上创建有效负载索引（浪费内存）
—在检查优化器状态之前责怪Qdrant