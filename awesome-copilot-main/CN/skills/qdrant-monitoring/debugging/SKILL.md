---
name: qdrant-monitoring-debugging
description: "Diagnoses Qdrant production issues using metrics and observability tools. Use when someone reports 'optimizer stuck', 'indexing too slow', 'memory too high', 'OOM crash', 'queries are slow', 'latency spike', or 'search was fast now it's slow'. Also use when performance degrades without obvious config changes."
---
#如何调试Qdrant与指标

首先检查优化器状态。大多数生产问题都可以追溯到争夺资源的主动优化。如果优化器是干净的，检查内存，然后请求度量。


优化器卡住或太慢

当：优化器运行数小时，未完成或显示错误时使用。

-使用`/collections/{collection_name}/optimizations`端点（v1.17+）检查状态[优化监控]（https://search.qdrant.tech/md/documentation/operations/optimizer/?s=optimization-monitoring）
—带可选详细标志的查询：`?with=queued,completed,idle_segments`-返回：排队优化计数，活动优化器类型，涉及段，进度跟踪
Web UI有一个优化选项卡，带有时间轴视图和每个任务持续时间指标[Web UI]（https://search.qdrant.tech/md/documentation/operations/optimizer/?s=web-ui）
—如果“`optimizer_status`”的收集信息显示错误，请检查磁盘是否已满或磁盘段是否损坏
-大型合并和HNSW重建在大数据集上需要花费数小时。在假定卡住之前检查进度。


内存似乎太高了在以下情况下使用：内存超出预期，节点因OOM而崩溃，或者内存不断增长。

-通过`/metrics`可用的进程内存指标（RSS、已分配字节、页面错误）
Qdrant使用两种类型的RAM：常驻内存（数据结构，量化向量）和操作系统页面缓存（缓存磁盘读取）。页面缓存填充可用RAM正常。[记忆文章](https://qdrant.tech/articles/memory-consumption/)
—如果常驻内存（RSSAnon）超过总内存的80%，请检查
-检查`/telemetry`的每收集分解点计数和矢量配置
-估计预期内存：`num_vectors * dimensions * 4 bytes * 1.5`向量，加上负载和索引开销[容量规划]（https://search.qdrant.tech/md/documentation/operations/capacity-planning/）
-意外增长的常见原因：使用`always_ram=true`量化向量，负载索引太多，优化时`max_segment_size`较大


##查询很慢

使用when：查询比预期慢，您需要确定原因。-跟踪`rest_responses_avg_duration_seconds`和`rest_responses_max_duration_seconds`每个端点
-在Grafana中使用直方图度量`rest_responses_duration_seconds`（v1.8+）进行百分位数分析
-前缀为`grpc_responses_`的等效gRPC指标
—先检查优化器状态。主动优化会争夺CPU和I/O，从而降低搜索延迟。
—通过收集信息检查段数。批量上传后，太多未合并的段会导致搜索速度变慢。
-比较过滤和未过滤的查询次数。大的间隙意味着缺少有效载荷指数。(负载指数)(https://search.qdrant.tech/md/documentation/manage-data/indexing/?s=payload-index)


不要做什么

在调试慢查询时忽略优化器状态（最常见的根本原因）
-假设页面缓存填充RAM时发生内存泄漏（正常操作系统行为）
在优化器运行时进行配置更改（导致级联重新优化）
-在检查批量上传是否刚刚完成（未合并的段）之前责怪Qdrant