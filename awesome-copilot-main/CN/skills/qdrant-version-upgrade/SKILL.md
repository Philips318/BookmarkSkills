---
name: qdrant-version-upgrade
description: "Guidance on how to upgrade your Qdrant version without interrupting the availability of your application and ensuring data integrity."
---
# Qdrant版本升级

Qdrant对版本兼容性有以下保证：

- Qdrant和SDK的主要和次要版本预计会匹配。例如，Qdrant 1.17。x兼容SDK 1.17.x。

Qdrant测试了小版本之间的向后兼容性。例如，Qdrant 1.17。x应该与SDK 1.16.x兼容。qrant服务器1.16。x也有望与SDK 1.17兼容。但仅适用于1.16.x中可用的功能子集。

—如果要迁移到下一个次要版本，建议先将SDK升级到下一个次要版本，然后再升级Qdrant服务器。—存储兼容性只保证一个小版本。例如，使用Qdrant 1.16存储的数据。预计将与Qdrant 1.17.x兼容。如果需要迁移多个次要版本，则需要一步一步地进行升级，每次一个次要版本。例如，从1.15迁移。X = 1.17。X，你需要先升级到1.16。X和1.17.x。注意：Qdrant Cloud自动化了这个过程，所以你可以直接从1.15升级。X = 1.17。X没有中间步骤。

—复制因子为2或更高的Qdrant集群可以通过执行滚动升级而无需停机。这意味着您可以一次升级一个节点，而其他节点继续为请求提供服务。这允许您在升级过程中保持应用程序的可用性。关于复制因子的更多信息：[复制因子]（https://search.qdrant.tech/md/documentation/operations/distributed_deployment/?s=replication-factor）在Qdrant Cloud中管理Qdrant版本升级，您可以使用[qcloud](https://github.com/qdrant/qcloud-cli) CLI工具。