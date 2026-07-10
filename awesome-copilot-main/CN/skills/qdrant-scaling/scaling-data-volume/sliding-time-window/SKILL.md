---
name: qdrant-sliding-time-window
description: "Guides sliding time window scaling in Qdrant. Use when someone asks 'only recent data matters', 'how to expire old vectors', 'time-based data rotation', 'delete old data efficiently', 'social media feed search', 'news search', 'log search with retention', or 'how to keep only last N months of data'."
---
#缩放与滑动时间窗口

当只有最近的数据需要快速搜索时使用-社交媒体帖子，新闻文章，支持票，日志，工作列表。旧数据要么变得无关紧要，要么可以容忍较慢的访问速度。

三种策略：**碎片旋转**（推荐），**收集旋转**（当每个周期的配置不同时），和**filter-and-delete**（最简单，用于连续清理）。


##碎片旋转（推荐）

在以下情况下使用：数据有自然的时间边界（每天、每周、每月）。首选，因为查询跨越一个请求中的所有时间段，没有应用程序级别的扇出。(用户定义的分片)(https://search.qdrant.tech/md/documentation/operations/distributed_deployment/?s=user-defined-sharding)1. 创建一个启用了用户定义分片的集合
2. 每个时间段创建一个分片键（例如，`2025-01`,`2025-02`，…）`2025-06`)
3. 将数据摄取到当前时间段的分片键中
4. 当一个新的时间段开始时，创建一个新的分片键并重定向写操作
5. 删除保留窗口外最旧的shard key

-删除一个分片键会立即回收所有资源（没有碎片，没有优化器开销）
-在轮换之前预先创建下一周期的分片键，以避免写中断
—为了提高效率，在查询时使用`shard_key_selector`只搜索特定时间段
Shard key可以放置在hot/cold分级的特定节点上


##集合轮换（别名交换）

当您需要按周期收集配置（例如，不同的量化或存储设置）时使用。(收集别名)(https://search.qdrant.tech/md/documentation/manage-data/collections/?s=collection-aliases)1. 每个时间段创建一个集合，将写入别名指向最新的集合
2. 并行查询所有活动集合，合并客户端结果
3. 当一个新周期开始时，创建新的集合并交换写别名[Switch collection]（https://search.qdrant.tech/md/documentation/manage-data/collections/?s=switch-collection）
4. 把最旧的收藏放在窗外

权衡与分片轮换：允许每个集合的配置差异，但需要应用程序级的扇出和更多的操作开销。


# # Filter-and-Delete

当数据连续到达而没有明确的时间界限时使用，或者您想要最简单的设置。

1. 在每个点上存储一个`timestamp`有效载荷，并在其上创建一个有效载荷索引[payload index]（https://search.qdrant.tech/md/documentation/manage-data/indexing/?s=payload-index）
2. 在查询时使用`range`条件[Range Filter]（https://search.qdrant.tech/md/documentation/search/filtering/?s=range）筛选到所需的窗口
3. 使用delete-by-filter [delete points]定期删除过期点（https://search.qdrant.tech/md/documentation/manage-data/points/?s=delete-points）-在非高峰时段分批（10k-50k点）运行清理，以避免优化器锁定
-删除不是免费的：墓碑点降低搜索，直到优化器压缩段
-不会立即回收磁盘（压缩是异步的）


##Hot/Cold分级

在以下情况下使用：最近的数据需要快速的内存搜索，较旧的数据应该在较低的性能下保持可搜索性。

- **Shard rotation:**将当前Shard key放在快速存储节点上，通过Shard placement将旧的Shard key移动到更便宜的节点。所有查询仍然通过单个集合。
- **集合旋转：**保持当前的集合在RAM (`always_ram: true`)，移动旧的集合到mmap/on-disk向量。(量化)(https://search.qdrant.tech/md/documentation/manage-data/quantization/)


不要做什么-对于每天有数百万次删除的大容量时间序列，不要使用filter-and-delete（使用轮换）
不要忘记索引时间戳字段（没有索引的范围过滤器会导致完全扫描）
-当碎片旋转足够时，不要使用收集旋转（不必要的扇出复杂性）
—在确认shard key或collection的保存周期已满之前，不要删除该shard key或collection
—不要跳过预创建下一个周期的shard key或collection（在轮询过程中写失败很难恢复）