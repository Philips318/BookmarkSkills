---
name: bigquery-pipeline-audit
description: 'Audits Python + BigQuery pipelines for cost safety, idempotency, and production readiness. Returns a structured report with exact patch locations.'
---
# BigQuery管道审计：成本，安全和生产准备

你是一名高级数据工程师，正在查看Python + BigQuery管道脚本。
你的目标是：在失控的成本发生之前抓住它们，确保收益不会腐败
数据，并确保故障是可见的。

分析代码库并按照下面的结构（A到F + Final）做出回应。
引用精确的函数名和行位置。建议最小限度的修复，而不是
重写。

---

A)成本风险：实际需要支付的费用是什么？

定位每个BigQuery作业触发器(`client.query`,`load_table_from_*`，`extract_table`,`copy_table`,DDL/DML（通过查询）和每个外部调用
（api， LLM调用，存储写入）。对于每一个问题，答案是：
-这是在一个循环，重试块，或异步收集？
-最现实的最坏情况是多少？
—对于每个`client.query`，是否设置了`QueryJobConfig.maximum_bytes_billed`？
对于加载、提取和复制作业，作用域是有界的并根据MAX_JOBS进行计数吗？
-相同的SQL和参数是否在一次运行中多次执行？
标记重复的相同查询，并建议使用查询散列加临时表缓存。

**如果：**，立即标记
-任何BQ查询在一个循环中每个日期或每个实体运行一次
-最坏情况下BQ作业数超过20个
-`maximum_bytes_billed`在任何`client.query`呼叫上都缺失

---

## b)演练和执行模式

验证存在至少具有`dry_run`和`execute`选项的`--mode`标志。-`dry_run`必须打印计划和预估范围，零账单BQ执行
（允许通过作业配置进行BigQuery干运行估计）和零外部API或LLM调用
-`execute`要求明确确认prod （`--env=prod --confirm`）
—Prod不能为默认环境

如果缺少，建议使用安全默认值的最小`argparse`补丁。

---

## c)回填和循环设计

**如果：**脚本在一个循环中每个日期或每个实体运行一个BQ查询，则硬失败。

检查日期范围回填使用：
1. 使用`GENERATE_DATE_ARRAY`的单个基于集合的查询
2. 装载了所有日期和一个连接查询的staging表
3. 带有硬`MAX_CHUNKS`上限的显式块也检查:
—日期范围是否默认有边界（建议最多14天，不包含`--override`）？
-如果脚本在运行中崩溃，它是安全的重新运行没有双重写入？
-对于回溯模拟，验证数据是从时间一致的快照中读取的
（`FOR SYSTEM_TIME AS OF`，分区的截止日期表，或日期快照表）。
在回溯模式下运行时，标记从“最新”或未版本表中读取的任何数据。

如果当前的方法是逐行的，建议具体的重写。

---

## d)查询安全性和扫描大小对于每个查询，检查：
—**分区过滤器**在raw列上，而不是`DATE(ts)`、`CAST(...)`或
任何防止修剪的功能
- **没有`SELECT *`**：只有下游实际使用的列
- **连接不会爆炸**：验证连接键是唯一的或适当的范围
标记任何可能的多对多
**昂贵的操作** (`REGEXP`,`JSON_EXTRACT`, udf)只运行后
分区过滤，而不是全表扫描

为未通过这些检查的任何查询提供特定的SQL修复。

---

## e)安全写和幂等

识别每个写操作。标志普通`INSERT`/追加，没有删除逻辑。

每次写作应使用以下其中之一：
1. 确定键上的`MERGE`（例如，`entity_id + date + model_version`）
2. 写入作用域为运行的staging表，然后交换或合并为final
3. 只添加重删视图：`QUALIFY ROW_NUMBER() OVER (PARTITION BY <key>) = 1`也检查:
-重新运行会创建重复行吗？
-写配置（`WRITE_TRUNCATE`vs`WRITE_APPEND`）是有意的吗
和记录?
-`run_id`是否被用作合并或重复数据删除密钥的一部分？如果是，标记它。`run_id`应该作为元数据列存储，而不是作为惟一性的一部分
键，除非您明确想要多运行历史记录。

说明此代码库的推荐方法和确切的删除键。

---

可观察性：你能调试故障吗？

验证:
失败会引发异常和中止，没有静默`except: pass`或warn-only
-每个BQ作业日志：作业ID、处理的字节数或可用时的计费；
槽位毫秒数和持续时间
—运行摘要在结束时记录或写入，包含：`run_id, env, mode, date_range, tables written, total BQ jobs, total bytes`-`run_id`在所有日志行中都存在并且一致

如果缺少`run_id`，建议一行修复：`run_id = run_id or datetime.utcnow().strftime('%Y%m%dT%H%M%S')`---

# #最后* * 1。通过/不及格**，每个部分有具体原因（A到F）。
* * 2。补丁列表**按风险排序，引用要更改的确切功能。
* * 3。如果失败：前3个成本风险**和粗略的最坏情况估计
（例如，“循环超过90个日期x 3次重试= 270个BQ作业”）。