概览和痕迹和跨度

本文档介绍了Phoenix中OpenInference trace和span的基本概念。

# #概述

OpenInference是一组基于opentelementetry的AI和LLM应用的语义约定。Phoenix使用这些约定来捕获、存储和分析来自AI应用程序的痕迹。

* *关键概念:* *

- **跟踪**表示通过应用程序的端到端请求
- span表示跟踪中的单个操作（LLM调用，检索，工具调用）
- **属性**是键值对，使用扁平的点符号路径附加到跨度上
**分类操作类型（LLM, retriver， TOOL等）

跟踪和跨度

跟踪层次结构

trace是一个由**span **组成的树，代表一个完整的请求：```
Trace ID: abc123
├─ Span 1: CHAIN (root span, parent_id = null)
│  ├─ Span 2: RETRIEVER (parent_id = span_1_id)
│  │  └─ Span 3: EMBEDDING (parent_id = span_2_id)
│  └─ Span 4: LLM (parent_id = span_1_id)
│     └─ Span 5: TOOL (parent_id = span_4_id)
```
###上下文传播

span通过以下方式维护父子关系：

-`trace_id`-跟踪中的所有跨度相同
-`span_id`-此跨度的唯一标识符
-`parent_id`-引用父跨度的`span_id`（根跨度为空）

Phoenix使用这些关系来：

-在UI中构建跨度树可视化
-计算累计指标（令牌，错误）的树
-启用嵌套查询（例如，“查找包含错误的LLM跨度的CHAIN跨度”）

###跨度生命周期

每个跨度有：

-`start_time`-操作开始的时间（Unix时间戳，以纳秒为单位）
-`end_time`-操作完成时间
-`status_code`- OK、ERROR或UNSET
-`status_message`-可选错误信息
-`attributes`-具有所有语义约定属性的对象