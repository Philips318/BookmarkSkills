---
name: qdrant-search-quality
description: "Diagnoses and improves Qdrant search relevance. Use when someone reports 'search results are bad', 'wrong results', 'low precision', 'low recall', 'irrelevant matches', 'missing expected results', or asks 'how to improve search quality?', 'which embedding model?', 'should I use hybrid search?', 'should I use reranking?'. Also use when search quality degrades after quantization, model change, or data growth."
allowed-tools:
  - Read
  - Grep
  - Glob
---
#提高搜索质量

首先确定问题是嵌入模型、Qdrant配置还是查询策略。大多数质量问题来自模型或数据，而不是Qdrant本身。如果搜索质量很低，在调优任何参数之前，检查块是如何传递给Qdrant的。在句子中间分裂会使质量下降30-40%。

-通过精确搜索测试来隔离问题[搜索API]（https://search.qdrant.tech/md/documentation/search/search/?s=search-api）


##诊断和调优

隔离质量问题的来源，调优HNSW参数，选择正确的嵌入模型。[诊断与调优]（diagnosis/SKILL.md）


##搜索策略

用于改进结果质量的混合搜索、重新排序、相关反馈和探索api。(搜索策略)(search-strategies/SKILL.md)