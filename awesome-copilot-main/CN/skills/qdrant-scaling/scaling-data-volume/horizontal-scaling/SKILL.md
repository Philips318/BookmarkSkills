---
name: qdrant-horizontal-scaling
description: "Diagnoses and guides Qdrant horizontal scaling decisions. Use when someone asks 'vertical or horizontal?', 'how many nodes?', 'how many shards?', 'how to add nodes', 'resharding', 'data doesn't fit', or 'need more capacity'. Also use when data growth outpaces current deployment."
---
#当Qdrant需要更多容量时该怎么办

垂直优先：操作更简单，没有网络开销，根据维度和量化，每个节点的向量可达~100M。当数据超过单节点容量、需要容错、需要隔离租户或IOPS绑定（节点越多=独立IOPS越多）时，采用水平模式。

最基本的分布式配置

- 3个节点，3个分片，`replication_factor: 2`用于零停机扩展

至少3个节点对于共识和容错很重要。对于3个节点，您可以在不停机的情况下丢失1个节点。对于2个节点，丢失1个节点将导致收集操作停机。
复制因子为2意味着每个分片有1个副本，所以你有2个数据副本。这允许零停机时间的扩展和维护。使用`replication_factor: 1`，即使对于点级操作，也不能保证零停机时间，而集群维护需要停机时间。##选择分片数量

分片是数据分布的单位。
更多的分片允许更多的节点和更好的分布，但增加了开销。更少的分片减少了开销，但限制了水平扩展。

当集群规模为3 ~ 6节点时，建议设置为6 ~ 12个分片。
这允许每个节点使用2-4个分片，从而平衡分布和开销。

##更改分片数

当：分片计数不能被节点计数平均整除，导致分布不均匀，或需要重新平衡时使用。

重新分片既昂贵又耗时，如果无法进行常规数据分发，则应将其作为最后的手段。
重分片被设计成对用户操作透明，在重分片期间，更新和搜索应该仍然可以工作，但性能影响很小。

但是，重分片操作本身非常耗时，并且需要在节点之间移动大量数据。Qdrant Cloud [Resharding]（https://search.qdrant.tech/md/documentation/operations/distributed_deployment/?s=resharding）
-自托管部署不支持重新分片。

更好的选择：最初过度供应分片，或者使用正确的配置启动新集群并迁移数据。


不要做什么

-在垂直跳完之前不要跳到水平（没有增加复杂性）
-不要设置`shard_number`，它不是节点数的倍数（不均匀分布）
—如果需要容错，请勿在生产环境中使用`replication_factor: 1`不要在没有重新平衡碎片的情况下添加节点（使用碎片移动API重新分配）
-在没有负载测试的情况下不要缩减RAM（缓存删除会导致长达数天的延迟事件）
—每个租户使用一个收集，不要超过收集限制（使用有效负载分区）