---
name: qdrant-scaling-qps
description: "Guides Qdrant query throughput (QPS) scaling. Use when someone asks 'how to increase QPS', 'need more throughput', 'queries per second too low', 'batch search', 'read replicas', or 'how to handle more concurrent queries'."
---
#扩展查询吞吐量（QPS）

吞吐量扩展意味着每秒处理更多的并行查询。
这与延迟不同——吞吐量和延迟是相反的调优方向，不能在同一节点上同时进行优化。

高吞吐量支持更少、更大的段，因此每个查询的开销更少。


更高RPS的性能调优

-使用更少，更大的段（`default_segment_number: 2`）[最大化吞吐量]（https://search.qdrant.tech/md/documentation/operations/optimize/?s=maximizing-throughput）
-启用量化与`always_ram=true`减少磁盘IO[量化]（https://search.qdrant.tech/md/documentation/manage-data/quantization/）
-使用批量搜索API来分摊开销[批量搜索]（https://search.qdrant.tech/md/documentation/search/search/?s=batch-search-api）

最小化更新工作负载的影响-配置更新吞吐量控制（v1.17+），以防止未优化的搜索降低读取[低延迟搜索]（https://search.qdrant.tech/md/documentation/search/low-latency-search/）
-设置`optimizer_cpu_budget`来限制索引cpu（例如，在8个cpu节点上，`2`为查询保留6个cpu）
—配置延迟读扇出（v1.17+），用于尾部延迟[延迟扇出]（https://search.qdrant.tech/md/documentation/search/low-latency-search/?s=use-delayed-fan-outs）



水平扩展吞吐量

如果在应用上述调优后，单个节点的CPU已饱和，请使用读副本水平扩展。

—分片副本提供来自复制的分片的查询，在节点之间分配读负载
—每个副本增加独立的查询能力，不需要重新分片
-使用`replication_factor: 2+`和路由读取副本[分布式部署]（https://search.qdrant.tech/md/documentation/operations/distributed_deployment/?s=replication）

参见[水平缩放]（../scaling-data-volume/horizontal-scaling/SKILL.md）了解一般的水平缩放指导。


磁盘I/O存在瓶颈如果不可能将所有向量都保存在RAM中，那么磁盘I/O可能成为吞吐量的瓶颈。
在这种情况下：

—先升级到provision IOPS或本地NVMe。请参阅[磁盘性能文章]中磁盘性能对矢量搜索的影响（https://qdrant.tech/articles/memory-consumption/）
在Linux （kernel 5.11+）上使用`io_uring`—在量化矢量的情况下，更倾向于全局评分而不是每段评分，以减少磁盘读取。[教程]中的示例（https://search.qdrant.tech/md/documentation/tutorials-operations/large-scale-search/?s=search-query）
—配置更高数量的搜索线程来并行磁盘读取。默认值是`cpu_count - 1`，这对于基于ram的搜索是最优的，但对于基于磁盘的搜索可能太低。参见[配置参考]（https://search.qdrant.tech/md/documentation/operations/configuration/?s=configuration-options）
—如果仍然饱和，水平扩展（每个节点增加独立的IOPS）


不要做什么—不要期望在同一节点上同时优化吞吐量和延迟
-不要为吞吐量工作负载使用许多小段（增加每次查询开销）
—当绑定iops时，不进行水平扩展，同时不升级磁盘层
-不要在>90%的RAM下运行（操作系统缓存退出=严重的性能下降）