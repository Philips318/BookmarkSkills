---
name: qdrant-tenant-scaling
description: "Guides Qdrant multi-tenant scaling. Use when someone asks 'how to scale tenants', 'one collection per tenant?', 'tenant isolation', 'dedicated shards', or reports tenant performance issues. Also use when multi-tenant workloads outgrow shared infrastructure."
---
扩展多租户Qdrant时该怎么做

不要为每个租户创建一个集合。不能扩展超过几百个并且浪费资源。有一家公司在采用每次回收的模式一年后达到了1000个回收限制，不得不迁移到有效负载分区。使用带有租户密钥的共享集合。

-了解多租户模式[multitenancy]（https://search.qdrant.tech/md/documentation/manage-data/multitenancy/）

以下是对这些模式的简短总结：

##租户数量约为1万

通过负载过滤使用默认的多租户策略。

有关索引和查询性能的最佳实践，请阅读[按负载划分]（https://search.qdrant.tech/md/documentation/manage-data/multitenancy/?s=partition-by-payload）和[校准性能]（https://search.qdrant.tech/md/documentation/manage-data/multitenancy/?s=calibrate-performance）。


##租户数量约为10万或更多在这种规模下，集群可能由几个对等节点组成。
为了将租户数据本地化并提高性能，可以使用[custom sharding]（https://search.qdrant.tech/md/documentation/operations/distributed_deployment/?s=user-defined-sharding）根据租户ID散列将租户分配到特定的分片。
这将使租户请求本地化到特定节点，而不是将它们广播到所有节点，从而提高性能并减少每个节点上的负载。

##如果租户大小不均匀

如果一些租户比其他租户大得多，可以使用[分级多租户]（https://search.qdrant.tech/md/documentation/manage-data/multitenancy/?s=tiered-multitenancy）将大租户提升到专用的分片，同时将小租户保留在共享的分片上。这样可以为不同规模的租户优化资源分配和性能。

需要严格的租户隔离

在以下情况下使用：legal/compliance要求对每个租户进行加密或严格隔离，超出了负载过滤所提供的范围。-每个租户的加密密钥可能需要多个集合
-限制集合计数，并在每个集合内使用有效负载过滤
-这是例外，不是默认的。仅在遵从性要求时使用。


不要做什么

-在没有合规性证明的情况下，不要为每个租户创建一个集合（不能扩展到数百个）
—不要跳过租户索引上的`is_tenant=true`，这会影响顺序读性能。
-不要为多租户集合构建全局HNSW（浪费，使用`payload_m`代替）