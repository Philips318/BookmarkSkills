---
name: qdrant-search-strategies
description: "Guides Qdrant search strategy selection. Use when someone asks 'should I use hybrid search?', 'BM25 or sparse vectors?', 'how to rerank?', 'results are not relevant', 'I don't get needed results from my dataset but they're there', 'retrieval quality is not good enough', 'results too similar', 'need diversity', 'MMR', 'relevance feedback', 'recommendation API', 'discovery API', 'ColBERT reranking', or 'missing keyword matches'"
---
#如何提高搜索结果与先进的策略

这些策略是对基本向量搜索的补充。在确认嵌入模型适合任务和HNSW配置正确后使用它们。如果精确搜索返回不好的结果，首先验证嵌入模型（检索器）的选择。
如果用户想使用较弱的嵌入模型，因为它小、快、便宜，那么使用重新排名或相关反馈来提高搜索质量。

缺少明显的关键字匹配

纯向量搜索缺少包含明显关键字匹配的结果时使用。领域术语未在训练数据中嵌入，关键字精确匹配至关重要（品牌名称、sku），缩略语常见。跳过：纯语义查询，所有数据在训练集中，延迟预算非常紧张。-密集+稀疏与`prefetch`和融合[混合搜索]（https://search.qdrant.tech/md/documentation/search/hybrid-queries/?s=hybrid-search）
-如果适用，首选学习稀疏（[miniCOIL](https://search.qdrant.tech/md/documentation/fastembed/fastembed-minicoil/)， SPLADE， GTE）而不是原始BM25（当用户需要智能关键字匹配并且学习稀疏模型知道领域的词汇表时）
—对于非英语语言，[相应配置稀疏BM25参数]（https://search.qdrant.tech/md/documentation/search/text-search/?s=language-specific-settings）
- RRF：良好的默认，支持加权（v1.17+） [RRF]（https://search.qdrant.tech/md/documentation/search/hybrid-queries/?s=reciprocal-rank-fusion-rrf）
具有非对称限制（sparse_limit=250, dense_limit=100）的DBSF可以优于RRF的技术文档[DBSF]（https://search.qdrant.tech/md/documentation/search/hybrid-queries/?s=distribution-based-score-fusion-dbsf）
-融合也可以通过重新排名来完成

找到正确的文件，但顺序错误

当：查全率高但查准率低时使用（正确的文档排在前100名，而不是前10名）。-通过FastEmbed [rerankers]交叉编码器重新排序（https://search.qdrant.tech/md/documentation/fastembed/fastembed-rerankers/）
-查看如何在qrant中使用[Multistage queries]（https://search.qdrant.tech/md/documentation/search/hybrid-queries/?s=multi-stage-queries）
由于后期的交互机制，ColBERT和ColPali/ColQwen的重排名特别精确，但是很重。在不构建HNSW的情况下配置和存储多向量以节省资源是很重要的。参见[多向量表示]（https://search.qdrant.tech/md/documentation/tutorials-search-engineering/using-multivector-representations/）

没有找到正确的文件，但它们在那里

当基本检索已经到位，但检索器缺少您知道存在于数据集中的相关项时使用。适用于任何可嵌入的数据（文本，图像等）。相关性反馈（RF）查询使用反馈模型对检索结果的分数来引导检索器在随后的迭代中遍历整个向量空间，比如通过检索器对整个集合重新排序。与重新排名互补：重新排名者看到的是有限的子集，射频利用反馈信号的收集范围。甚至3-5个反馈分数也足够了。可以运行多个迭代。

反馈模型是任何产生每个文档相关性分数的东西：双编码器，交叉编码器，后期交互模型，llm作为裁判。模糊相关性评分是有效的，而不仅仅是二进制的（good/bad,relevant/irrelevant），因为反馈被表示为分级相关性评分（越高=越相关）。

跳过以下情况：如果检索者已经有很强的回忆，或者如果检索者和反馈模型在相关性上非常一致。- RF查询目前基于[3-parameter naive formula](https://search.qdrant.tech/md/documentation/search/search-relevance/?s=naive-strategy)，没有通用默认值，因此必须对每个数据集，检索器和反馈模型进行调优
-使用[qdrant-relevance-feedback]（https://pypi.org/project/qdrant-relevance-feedback/）来调整参数，与Evaluator一起评估影响，并检查检索者反馈的一致性。有关安装说明，请参阅README。不需要gpu，该框架还提供了预定义的检索器和反馈模型选项。
—检查[关联反馈查询API]（https://search.qdrant.tech/md/documentation/search/search-relevance/?s=relevance-feedback）的配置
-使用这个作为端到端文本检索助手的例子，带参数调优和求值，以了解如何使用API和运行`qdrant-relevance-feedback`框架：

结果太相似

在以下情况下使用：顶级结果是冗余的，几乎重复的，或者缺乏多样性。常见于密集内容领域（学术论文、产品目录）。-使用MMR （v1.15+）作为`diversity`的查询参数，以平衡相关性和多样性[MMR]（https://search.qdrant.tech/md/documentation/search/search-relevance/?s=maximal-marginal-relevance-mmr）
-从`diversity=0.5`开始，越低越精确，越高越探索
- MMR比标准搜索慢。只有当冗余是一个实际问题时才使用。

知道什么是好的结果，但却得不到

在以下情况下使用：你可以提供积极和消极的例子点来引导搜索更接近积极而远离消极。

-推荐API:positive/negative示例推荐拟合向量[推荐API]（https://search.qdrant.tech/md/documentation/search/explore/?s=recommendation-api）
最高分策略：更适合不同的例子，只支持负数[最高分]（https://search.qdrant.tech/md/documentation/search/explore/?s=best-score-strategy）
-发现API：上下文对（positive/negative）约束搜索区域，没有请求目标[发现]（https://search.qdrant.tech/md/documentation/search/explore/?s=discovery-api）在相关性背后有业务逻辑
在以下情况下使用：结果应该根据一些基于数据的业务逻辑（如近距或距离）进行额外排名。

检查如何设置[分数提升文档]（https://search.qdrant.tech/md/documentation/search/search-relevance/?s=score-boosting）

不要做什么

-在验证纯矢量质量之前使用混合搜索（增加复杂性，可能掩盖模型问题）
-在非英语文本上使用BM25而没有正确配置特定语言的停止词删除（严重降低结果）
-在添加相关性反馈时跳过评估（最好检查真实的查询，它实际上会有所帮助）