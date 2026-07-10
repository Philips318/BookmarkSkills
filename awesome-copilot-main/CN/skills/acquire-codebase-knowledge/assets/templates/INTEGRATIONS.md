#外部集成

##核心部分（必选）

1)整合清单

|系统|类型（API/DB/Queue/etc） |用途|认证模型|临界|证据||--------|---------------------------|---------|------------|-------------|----------|
|【名称】|【类型】|【目的】|【认证】|【high/med/low】| |(文件)

2)数据存储

|存储|角色|访问层|关键风险|证据||-------|------|--------------|----------|----------|
[role], [db/cache/etc] | | |(模块)|(风险)| |(文件)

### 3)秘密和凭证处理

-凭证来源：[env/secretsmanager/config]
-硬编码检查：[result]
-旋转或生命周期注释：[known/unknown]

4)可靠性和失效行为

-Retry/backoff行为：[implemented/none/partial]
-超时策略：[配置时]
-断路器或回退行为：[如果有的话]

5)集成的可观察性

-记录外部调用：[yes/no+ where]
-Metrics/tracing覆盖范围：[yes/no+ where]
-缺失的可见性差距：[list]

6)证据- [path/to/integration-wrapper]
- [path/to/config-or-env-template]
- [path/to/monitoring-or-logging-config]
##扩展节（可选）

只在需要时添加：

-逐端点目录
-验证流程序列图
-每积分SLA/SLO-Region/failover拓扑说明