---
name: qdrant-deployment-options
description: "Guides Qdrant deployment selection. Use when someone asks 'how to deploy Qdrant', 'Docker vs Cloud', 'local mode', 'embedded Qdrant', 'Qdrant EDGE', 'which deployment option', 'self-hosted vs cloud', or 'need lowest latency deployment'. Also use when choosing between deployment types for a new project."
---
#我需要哪个Qdrant部署？

从你需要的开始：管理运营还是完全控制？网络延迟是否可以接受？生产还是原型制作？答案缩小到四个选项之一。


##开始或原型制作

在构建原型、运行测试、CI/CD管道或学习Qdrant时使用。

-使用本地模式（仅限Python）：零依赖，内存或磁盘持久化，不需要服务器[本地模式]（https://search.qdrant.tech/md/documentation/quickstart/）
—本地模式数据格式与服务器不兼容。不要用于生产或基准测试。
—对于本地实服务器，使用Docker [Quick start]（https://search.qdrant.tech/md/documentation/quickstart/?s=download-and-run）


##进入生产阶段（自托管）

在以下情况下使用：需要完全控制基础设施、数据驻留或自定义配置。—默认部署为Docker。完整的Qdrant开源特性集，最小的设置。快速启动(https://search.qdrant.tech/md/documentation/quickstart/?s=download-and-run)
-你自己的操作：升级，备份，扩展，监控
—多节点集群必须手动设置分布式模式[分布式部署]（https://search.qdrant.tech/md/documentation/operations/distributed_deployment/）
-如果你想在你的基础设施上进行Qdrant云管理，考虑混合云[混合云]（https://search.qdrant.tech/md/documentation/hybrid-cloud/）


##投入生产（零操作）

在以下情况下使用：您需要具有零停机更新、自动备份和重新分片的托管基础架构，而无需自己操作集群。

- Qdrant Cloud处理升级，扩展，备份和监控[Qdrant Cloud]（https://search.qdrant.tech/md/documentation/cloud-quickstart/）
—支持自动多版本升级
-提供自托管不提供的功能：`/sys_metrics`，管理重分片，预配置警报


需要尽可能低的延迟当无法接受到服务器的网络往返时使用。边缘设备、进程内搜索或延迟关键型应用程序。

Qdrant EDGE：进程内绑定到Qdrant分片级函数，没有网络开销[Qdrant EDGE]（https://search.qdrant.tech/md/documentation/edge/edge-quickstart/）
—数据格式与服务器相同。可以通过分片快照与服务器同步。
—仅支持单节点特性集。无分布式模式。


不要做什么

-使用本地模式进行生产或基准测试（未经优化，数据格式不兼容）
-没有监控和备份策略的自主机（您将丢失数据或错过停机）
-当您需要分布式搜索（仅限单节点）时，选择EDGE
-选择混合云，除非你有数据驻留要求（当Qdrant Cloud工作时，不必要的Kubernetes复杂性）