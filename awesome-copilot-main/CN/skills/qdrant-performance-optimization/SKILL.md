---
name: qdrant-performance-optimization
description: "Different techniques to optimize the performance of Qdrant, including indexing strategies, query optimization, and hardware considerations. Use when you want to improve the speed and efficiency of your Qdrant deployment."
allowed-tools:
  - Read
  - Grep
  - Glob
---
# qrant性能优化

Qdrant性能有不同的方面，本文档作为Qdrant性能优化的不同方面的导航中心。


搜索速度优化

搜索速度有两个不同的标准：延迟和吞吐量。
延迟是获得单个查询响应所需的时间，而吞吐量是在给定时间范围内可以处理的查询数量。
根据您的用例，您可能希望对这些指标中的一个或两个进行优化。

更多关于搜索速度优化的信息可以在[搜索速度优化]（search-speed-optimization/SKILL.md）技能中找到。


索引性能优化

Qdrant需要建立一个向量索引来执行高效的相似度搜索。构建索引所需的时间取决于数据集的大小、硬件和配置。更多关于索引性能优化的信息可以在[索引性能优化]（indexing-performance-optimization/SKILL.md）技能中找到。


内存使用优化

向量搜索可能会占用大量内存，特别是在处理大型数据集时。
Qdrant有一个灵活的内存管理系统，它允许您精确地控制存储的哪些部分保存在内存中，哪些存储在磁盘上。这可以帮助您在不牺牲性能的情况下优化内存使用。

更多关于内存使用优化的信息可以在[内存使用优化]（memory-usage-optimization/SKILL.md）技能中找到。