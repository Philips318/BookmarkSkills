---
name: qdrant-scaling
description: "Guides Qdrant scaling decisions. Use when someone asks 'how many nodes do I need', 'data doesn't fit on one node', 'need more throughput', 'cluster is slow', 'too many tenants', 'vertical or horizontal', 'how to shard', or 'need to add capacity'."
allowed-tools:
  - Read
  - Grep
  - Glob
---
#象限缩放

首先确定你的扩展目标：

-数据量
-查询吞吐量（QPS）
-查询延迟
-查询量

在确定了扩展目标后，我们可以根据权衡和假设来选择扩展策略。
每个都有不同的策略。吞吐量和延迟是相反的调优方向。


##扩展数据卷

当数据集的容量超过单个节点的容量时，这一点就变得相关。
有关扩展数据量的更多信息，请参阅[扩展数据量]（scaling-data-volume/SKILL.md）


##扩展查询吞吐量

如果你的系统需要处理比单个节点更多的并行查询，
然后，您需要扩展查询吞吐量。

有关查询吞吐量缩放的更多信息，请参阅[查询吞吐量缩放]（scaling-qps/SKILL.md）

##缩放查询延迟单个查询的延迟由查询执行路径中最慢的组件决定。
它有时与吞吐量相关，但并非总是如此。它可能需要不同的扩展策略。

有关查询延迟缩放的更多信息，请参阅[查询延迟缩放]（minimize-latency/SKILL.md）


##缩放查询卷

通过查询量，我们了解单个查询返回的结果数量。
如果查询量过大，可能会导致性能问题并增加延迟。

对查询量进行调优可能需要特殊的策略。

在[扩展查询量]（scaling-query-volume/SKILL.md）中了解更多关于扩展查询量的信息。