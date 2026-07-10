---
name: qdrant-minimize-latency
description: "Guides Qdrant query latency optimization. Use when someone asks 'search is slow', 'how to reduce latency', 'p99 is too high', 'tail latency', 'single query too slow', 'how to make search faster', or 'latency spikes'."
---
#伸缩查询延迟

单个查询的延迟由查询执行路径中最慢的组件决定。它有时与吞吐量相关，但并非总是如此——吞吐量和延迟是相反的调优方向。

低延迟优化的目标是最大限度地利用单个查询的资源饱和，而吞吐量优化的目标是最小化每个查询的资源使用，以允许更多的并行查询。

低延迟的性能调优

-增加段数以匹配CPU内核（`default_segment_number: 16`）[最小化延迟]（https://search.qdrant.tech/md/documentation/operations/optimize/?s=minimizing-latency）
-在RAM中保持量化向量和HNSW （`always_ram=true`）
-减少`hnsw_ef`在查询时间（交易召回速度）[搜索参数]（https://search.qdrant.tech/md/documentation/operations/optimize/?s=fine-tuning-search-parameters）
—使用本地NVMe，避免使用网络附加存储

内存压力和延迟RAM是延迟最关键的资源。如果工作集超过可用RAM，操作系统缓存驱逐将导致严重的、持续的延迟退化。

-垂直刻度RAM优先。如果工作设置>80%，则紧急。
-使用量化：标量（4倍缩减）或二进制（16倍缩减）[量化]（https://search.qdrant.tech/md/documentation/manage-data/quantization/）
-如果过滤不频繁，将有效载荷索引移动到磁盘[磁盘上的有效载荷索引]（https://search.qdrant.tech/md/documentation/manage-data/indexing/?s=on-disk-payload-index）
—设置“`optimizer_cpu_budget`”，限制后台优化cpu个数
—调度索引：在高峰时段设置高`indexing_threshold`垂直缩放延迟

更多的RAM和更快的CPU直接减少延迟。参见[垂直缩放]（../scaling-data-volume/vertical-scaling/SKILL.md）了解节点大小指南。


不要做什么—不要期望在同一节点上同时优化延迟和吞吐量
—对于延迟敏感的工作负载，不要使用少量的大段（每个段的搜索时间较长）。
-不要在>90%的RAM下运行（缓存删除会导致严重的延迟退化，可能持续数天）
—调试性能时不要忽略优化器状态
-在没有负载测试的情况下不要缩减RAM（缓存删除会导致长达数天的延迟事件）