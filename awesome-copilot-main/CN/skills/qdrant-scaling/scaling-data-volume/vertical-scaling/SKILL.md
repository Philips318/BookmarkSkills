---
name: qdrant-vertical-scaling
description: "Guides Qdrant vertical scaling decisions. Use when someone asks 'how to scale up a node', 'need more RAM', 'upgrade node size', 'vertical scaling', 'resize cluster', 'scale up vs scale out', or when memory/CPU is insufficient on current nodes. Also use when someone wants to avoid the complexity of horizontal scaling."
---
#当象限需要垂直缩放时该怎么办

垂直扩展意味着在现有节点上增加CPU、RAM或磁盘，而不是增加更多节点。这是考虑水平扩展之前的第一步。垂直扩展更简单，避免了分布式系统的复杂性，并且是可逆的。

Qdrant Cloud的垂直缩放是通过[Qdrant Cloud Console]（https://cloud.qdrant.io/）完成的
—对于自托管部署，调整底层虚拟机或容器资源的大小

何时垂直缩放

在以下情况下使用：当前节点资源（RAM、CPU、磁盘）不足，但工作负载还不需要分配。- RAM使用率接近可用内存的80%（操作系统页面缓存启动，严重的性能下降）
—查询服务或索引时CPU饱和
-磁盘空间运行低磁盘矢量和有效载荷
-根据维度和量化，单个节点可以处理多达~100M的向量
-对于非生产工作负载，可以容忍单点故障，不需要高可用性


##如何在Qdrant Cloud中垂直缩放

垂直扩展是通过Qdrant云控制台管理的。

—登录[Qdrant云控制台]（https://cloud.qdrant.io/）或使用[命令行工具]（https://github.com/qdrant/qcloud-cli）
—选择需要调整大小的集群
—选择更大的节点配置（更多的RAM或CPU，或两者兼而有之）
—如果配置了复制，升级过程为滚动重启，不会停机
—在调整大小之前，请确保`replication_factor: 2`或更高，以保持滚动重启期间的可用性**重要：**扩大规模很简单。缩小规模需要小心——如果工作集在缩小规模后不再适合RAM，则由于缓存驱逐，性能将严重降低。在缩减规模之前总是要进行负载测试。


RAM大小指南

RAM是Qdrant性能最关键的资源。使用这些指南来调整大小。

-准确估计内存使用是困难的；使用这个简单的近似公式：`num_vectors * dimensions * 4 bytes * 1.5`用于RAM中的全精度向量
-使用标量量化：除以4 （INT8将每个float32减少到1字节）[量化]（https://search.qdrant.tech/md/documentation/manage-data/quantization/）
-带二进制量化：除以32[二进制量化]（https://search.qdrant.tech/md/documentation/manage-data/quantization/?s=binary-quantization）
-增加HNSW索引（约占矢量数据的20-30%）、有效载荷索引和WAL的开销
-为优化器操作和操作系统缓存预留20%的空间
-通过Grafana/Prometheus监控调整大小前后的实际使用情况[监控]（../../../qdrant-monitoring/SKILL.md）当垂直缩放不再足够时

认识到这些信号，是时候横向发展了：

-即使使用量化和mmap，数据量也超过单个节点所能容纳的数据量
—IOPS已饱和（节点越多=独立硬盘越多I/O）
-需要容错（需要跨节点复制）
-需要通过专用分片隔离租户
—单节点CPU达到最大值，查询时延过高
—下一个垂直扩展步骤是可用的最大节点大小。您可能需要能够临时扩展到更大的节点大小来执行批处理操作或恢复。如果您已经拥有最大的节点大小，那么您将无法做到这一点。

当您达到这些限制时，请参阅[水平缩放]（../horizontal-scaling/SKILL.md）以获得有关分片和节点规划的指导。


不要做什么-在没有负载测试的情况下不要缩减RAM（缓存删除=严重的延迟退化，可能持续数天）
-不要忽略80%的RAM阈值（性能悬崖，而不是逐渐下降）
-在云中调整大小之前不要跳过复制（没有副本的滚动重启=停机）
-在用尽垂直选项之前不要跳到水平扩展（增加永久的操作复杂性）
-不要认为更多的CPU总是有帮助（更多的内核不会改善iops绑定的工作负载）