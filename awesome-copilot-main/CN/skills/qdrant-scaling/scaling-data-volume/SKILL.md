---
name: qdrant-scaling-data-volume
description: "Guides Qdrant data volume scaling decisions. Use when someone asks 'data doesn't fit on one node', 'too much data', 'need more storage', 'vertical or horizontal scaling', 'tenant scaling', 'time window rotation', or 'data growth exceeds capacity'."
allowed-tools:
  - Read
  - Grep
  - Glob
---
#扩展数据卷

本文档涵盖数据量扩展场景，
数据集的总大小超过单个节点的容量。

##租户扩展

如果用例是多租户的，这意味着每个用户只能访问数据的一个子集，
我们不需要查询所有数据，然后我们可以使用多租户模式进行扩展。

推荐的方法是使用带有负载分区、每个租户索引和分层多租户的多租户工作负载。

了解更多[租户扩展]（tenant-scaling/SKILL.md）

滑动时间窗口

一些用例基于滑动时间窗口，其中只有最近的数据是相关的。
例如，社交媒体帖子的索引，其中只有最近6个月的数据需要快速搜索。

了解更多[滑动时间窗口]（sliding-time-window/SKILL.md）

##全局搜索大多数通用用例需要对所有数据进行全局搜索。
在这种情况下，我们可能需要回到垂直缩放，
当我们达到垂直缩放的极限时，再进行水平缩放。


垂直缩放

当单个节点无法容纳数据时，第一种方法是扩展节点本身——更多的RAM、更好的磁盘、量化、mmap。
在进行水平扩展之前先耗尽垂直选项，因为水平扩展会增加永久性的操作复杂性。

了解更多[垂直缩放]（vertical-scaling/SKILL.md）

水平缩放

当单个节点即使使用量化和mmap也无法保存数据时，可以通过分片将数据分发到多个节点。

了解更多[水平缩放]（horizontal-scaling/SKILL.md）