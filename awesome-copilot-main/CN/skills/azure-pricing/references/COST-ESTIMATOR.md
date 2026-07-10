成本估算参考

用于将Azure单位价格转换为每月和年度成本估算的公式和模式。

##标准基于时间的计算

每月工作时间

Azure使用**730hours/month**作为标准计费周期（365天× 24小时/ 12个月）。```
Monthly Cost = Unit Price per Hour × 730
Annual Cost  = Monthly Cost × 12
```
###普通乘数

|周期|小时|计算方式||--------|-------|-------------|
| 1小时| 1 |单价|
| 24|单价× 24|
| 1周| 168 |单价× 168 |
| 1个月| 730 |单价× 730 |
| 1年| 8760 |单价× 8760 |

服务特定的公式

虚拟机（计算）```
Monthly Cost = hourly price × 730
```
对于只运行业务时间的虚拟机（8h/day, 22days/month）：```
Monthly Cost = hourly price × 176
```
Azure函数```
Execution Cost = price per execution × number of executions
Compute Cost   = price per GB-s × (memory in GB × execution time in seconds × number of executions)
Total Monthly  = Execution Cost + Compute Cost
```
免费拨款：执行100万次，每月400,000 gb。

Azure Blob存储```
Storage Cost   = price per GB × storage in GB
Transaction Cost = price per 10,000 ops × (operations / 10,000)
Egress Cost    = price per GB × egress in GB
Total Monthly  = Storage Cost + Transaction Cost + Egress Cost
```
### Azure Cosmos数据库

####预置吞吐量```
Monthly Cost = (RU/s / 100) × price per 100 RU/s × 730
```
# # # # Serverless```
Monthly Cost = (total RUs consumed / 1,000,000) × price per 1M RUs
```
Azure SQL数据库

#### DTU模型```
Monthly Cost = price per DTU × DTUs × 730
```
#### vCore模型```
Monthly Cost = vCore price × vCores × 730  +  storage price per GB × storage GB
```
Azure Kubernetes服务（AKS）```
Monthly Cost = node VM price × 730 × number of nodes
```
控制平面是免费的标准层。

Azure应用服务```
Monthly Cost = plan price × 730 (for hourly-priced plans)
```
或者按月固定价格购买固定套餐。

Azure OpenAI```
Monthly Cost = (input tokens / 1000) × input price per 1K tokens
             + (output tokens / 1000) × output price per 1K tokens
```
预订vs.现收现付比较

当提供定价选项时，总是显示比较：```
| Pricing Model | Monthly Cost | Annual Cost | Savings vs. PAYG |
|---------------|-------------|-------------|------------------|
| Pay-As-You-Go | $X | $Y | — |
| 1-Year Reserved | $A | $B | Z% |
| 3-Year Reserved | $C | $D | W% |
| Savings Plan (1yr) | $E | $F | V% |
| Savings Plan (3yr) | $G | $H | U% |
| Spot (if available) | $I | N/A | T% |
```
节省百分比公式：```
Savings % = ((PAYG Price - Reserved Price) / PAYG Price) × 100
```
成本汇总表模板

总是以这种格式呈现结果：```markdown
| Service | SKU | Region | Unit Price | Unit | Monthly Est. | Annual Est. |
|---------|-----|--------|-----------|------|-------------|-------------|
| Virtual Machines | Standard_D4s_v5 | East US | $0.192/hr | 1 Hour | $140.16 | $1,681.92 |
```
# #提示

-在估算之前，总是要明确**的使用模式** （24/7vs.营业时间vs.零星时间）。
—对于**存储**，询问期望的数据量和访问模式。
—对于**数据库**，询问吞吐量需求（RU/s、dtu或vCores）。
—对于无服务器服务，询问预期调用次数和持续时间。
-四舍五入到小数点后2位显示。
-请注意，除非另有说明，价格以**美元**计价。