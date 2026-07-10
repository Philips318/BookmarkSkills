---
name: qdrant-search-quality-diagnosis
description: "Diagnoses Qdrant search quality issues. Use when someone reports 'results are bad', 'wrong results', 'not relevant results', 'missing matches', 'recall is low', 'approximate search worse than exact', 'which embedding model', or 'quality dropped after quantization'. Also use when search quality degrades without obvious changes."
---
#如何诊断糟糕的搜索质量

在调优之前，建立基线。使用确切的KNN作为基础真理，与近似HNSW进行比较。目标>95% recall@K用于生产。

##还不知道怎么了

当结果不相关或缺少预期匹配并且需要隔离原因时使用。

-测试`exact=true`绕过HNSW近似[搜索API]（https://search.qdrant.tech/md/documentation/tutorials-search-engineering/retrieval-quality/?s=standard-mode-vs-exact-search）
-精确搜索=模型或搜索管道问题。确切的好，近似的坏=调HNSW。
-检查量化是否会降低质量（与没有进行比较）
-检查过滤器是否过于严格（那么你可能需要使用ACORN）
-如果从分块文档中产生重复，使用Grouping API去重复[Grouping]（https://search.qdrant.tech/md/documentation/search/search/?s=grouping-api）

负载过滤和稀疏向量搜索是不同的东西。元数据（日期、类别、标签）放入有效负载中进行过滤。文本内容放在稀疏向量中进行搜索。近似搜索比精确搜索更糟糕

使用时：精确搜索返回良好的结果，但HNSW近似错过它们。

增加查询时的`hnsw_ef`[搜索参数]（https://search.qdrant.tech/md/documentation/operations/optimize/?s=fine-tuning-search-parameters）
增加`ef_construct`（200+为高质量）[HNSW配置]（https://search.qdrant.tech/md/documentation/manage-data/indexing/?s=vector-index）
增加`m`（默认16，高召回32）[HNSW config]（https://search.qdrant.tech/md/documentation/manage-data/indexing/?s=vector-index）
-启用过采样+重分与量化[搜索与量化]（https://search.qdrant.tech/md/documentation/manage-data/quantization/?s=searching-with-quantization）
- ACORN过滤查询（v1.16+） [ACORN]（https://search.qdrant.tech/md/documentation/search/search/?s=acorn-search-algorithm）

二值量化需要重新计算。没有它，质量损失是严重的。使用过采样（最小3-5倍的二进制）来恢复召回。总是在生产前测试量化对数据的影响。(量化)(https://search.qdrant.tech/md/documentation/manage-data/quantization/)

错误的嵌入模型

当精确搜索也返回不好的结果时使用。在100-1000个样本查询上测试前3个MTEB模型，测量recall@10。特定于领域的模型通常优于一般模型。(托管推理)(https://search.qdrant.tech/md/documentation/inference/)

未优化的搜索管道

使用时：精确搜索也返回坏结果和模型选择由用户确认。

根据高级搜索策略技能优化搜索。

不要做什么-在验证模型是否适合任务之前调整Qdrant（大多数质量问题都是模型问题）
-使用无评分的二值量化（严重的质量损失）
-设置`hnsw_ef`低于要求的结果（保证不良召回）
-跳过过滤字段上的有效载荷索引，然后归咎于质量（HNSW不能遍历过滤掉的节点，只有在事先设置了有效载荷索引的情况下，才会构建可过滤的HNSW）
-部署时没有基线召回率或其他搜索相关性指标（无法衡量回归）
-将有效载荷过滤与稀疏向量搜索混淆（不同的东西，不同的配置）