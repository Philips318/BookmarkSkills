---
name: qdrant-memory-usage-optimization
description: "Diagnoses and reduces Qdrant memory usage. Use when someone reports 'memory too high', 'RAM keeps growing', 'node crashed', 'out of memory', 'memory leak', or asks 'why is memory usage so high?', 'how to reduce RAM?'. Also use when memory doesn't match calculations, quantization didn't help, or nodes crash during recovery."
---
#了解内存使用情况

Qdrant操作两种类型的内存：

-常驻内存（又名RSSAnon） -用于内部数据结构的内存，如ID跟踪器，加上必须留在RAM中的组件，如量化向量时`always_ram=true`和有效负载索引。

—操作系统页面缓存—用于缓存磁盘读取，可以在需要时释放。原始向量通常存储在页面缓存中，因此如果RAM满了，服务不会崩溃，但性能可能会下降。

操作系统页面缓存占用所有可用的RAM是正常的，但是如果驻留内存超过总RAM的80%，这是一个问题的迹象。

内存使用监控

—Qdrant通过`/metrics`端点公开内存使用情况。参见[监控文档]（https://search.qdrant.tech/md/documentation/operations/monitoring/）。<!-- ToDo: Talk about memory usage of each components once API is available -->
Qdrant需要多少内存？

最佳内存使用取决于用例。

—对于常规搜索场景，参考[容量规划文档]（https://search.qdrant.tech/md/documentation/operations/capacity-planning/）。

有关大规模内存使用的详细细分，请参见[大规模内存使用示例]（https://search.qdrant.tech/md/documentation/tutorials-operations/large-scale-search/?s=memory-usage）。

有效负载索引和HNSW图也需要内存，以及向量本身，因此在计算中考虑它们是很重要的。

此外，Qdrant需要一些额外的内存来进行优化。在优化期间，优化的段被完全加载到RAM中，因此留下足够的空间是很重要的。`max_segment_size`越大，所需的净空空间就越大。


何时将HNSW索引放到磁盘上

将经常使用的组件（如HNSW索引）放在磁盘上可能会导致显著的性能下降。
然而，在某些情况下，它可能是一个不错的选择：—部署低延迟磁盘—本地NVMe或类似的。
-多租户部署，其中只有租户的子集经常被访问，因此一次只有一小部分数据和索引加载到RAM中。
—对于启用[inline storage]（https://search.qdrant.tech/md/documentation/operations/optimize/?s=inline-storage-in-hnsw-index）的部署。


如何最小化内存占用

主要的挑战是将那些很少被访问的数据部分放在磁盘上。
以下是实现这一目标的主要技巧：

-使用量化只在RAM中存储压缩向量[量化文档]（https://search.qdrant.tech/md/documentation/manage-data/quantization/）

-使用float16或int8数据类型分别减少2倍或4倍的向量内存使用，在精度上有一些折衷。在[文档]中阅读更多关于向量数据类型的信息（https://search.qdrant.tech/md/documentation/manage-data/vectors/?s=datatypes）-利用矩阵表示学习（MRL）在RAM中只存储小向量，同时在磁盘上保留大向量。如何在Qdrant云推理中使用MRL的示例：[MRL docs]（https://search.qdrant.tech/md/documentation/inference/?s=reduce-vector-dimensionality-with-matryoshka-models）

-对于具有小租户的多租户部署，矢量可能存储在磁盘上，因为相同租户的数据存储在一起[多租户文档]（https://search.qdrant.tech/md/documentation/manage-data/multitenancy/?s=calibrate-performance）

-对于具有快速本地存储和相对较低的搜索吞吐量要求的部署，可以将矢量存储的所有组件存储在磁盘上。有关磁盘存储的性能影响的更多信息，请参阅[文章]（https://qdrant.tech/articles/memory-consumption/）。

—对于低内存环境，考虑`async_scorer`config，它支持`io_uring`并行磁盘访问，这可以显著提高磁盘上存储的性能。在[文章]（https://qdrant.tech/articles/io_uring/）中了解更多关于`async_scorer`的信息（仅适用于内核5.11+的Linux）-考虑将稀疏向量和文本负载存储在磁盘上，因为它们通常比密集向量更适合磁盘。
配置存储在磁盘上的有效载荷索引[docs]（https://search.qdrant.tech/md/documentation/manage-data/indexing/?s=on-disk-payload-index）
-配置稀疏向量存储在磁盘上[docs]（https://search.qdrant.tech/md/documentation/manage-data/indexing/?s=sparse-vector-index）