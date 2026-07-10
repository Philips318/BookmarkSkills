---
name: qdrant-scaling-query-volume
description: "Guides Qdrant query volume scaling. Use when someone asks 'query returns too many results', 'scroll performance', 'large limit values', 'paginating search results', 'fetching many vectors', or 'high cardinality results'."
---
#缩放查询量

问题：当一个查询有很大的限制（例如1000）并且有多个分片（例如10）时，天真地每个分片必须返回完整的1000个结果-总共10,000个得分点转移和合并。这是一种浪费，因为数据是随机分布在自动分片上的。

##核心理念

与其向每个分片请求完整的限制，不如向每个分片请求一个通过泊松分布统计计算的较小的限制，然后合并。这是安全的，因为自动分片保证了随机、独立的数据分布。

##激活时

—大于1个分片
—正在使用自动分片（所有查询的分片共享相同的分片键）
-请求的限制+偏移>= SHARD_QUERY_SUBSAMPLING_LIMIT （128）
—查询不准确

关键权衡该策略以小概率的略微不完整结果换取了分片间数据传输的大幅减少，特别是对于跨多个分片的高限制查询。1.2倍的安全系数和99.9%的泊松阈值使错误率非常低，与近似向量指数（如HNSW）已经引入的不准确性相当。