---
name: qdrant-monitoring-setup
description: "Guides Qdrant monitoring setup including Prometheus scraping, health probes, Hybrid Cloud metrics, alerting, and log centralization. Use when someone asks 'how to set up monitoring', 'Prometheus config', 'Grafana dashboard', 'health check endpoints', 'how to scrape Hybrid Cloud', 'what alerts to set', 'how to centralize logs', or 'audit logging'."
---
#如何设置象限监控

首先让普罗米修斯刮刀工作，然后是健康探测器，然后是警报。在进入生产环境之前，不要跳过监控设置。


##普罗米修斯度量

在以下情况下使用：第一次设置度量集合或添加新的部署。

-节点指标在`/metrics`端点[监控文档]（https://search.qdrant.tech/md/documentation/operations/monitoring/）
-集群指标在`/sys_metrics`（仅限Qdrant Cloud）
-前缀定制通过`service.metrics_prefix`config或`QDRANT__SERVICE__METRICS_PREFIX`env var
- Prometheus + Grafana自托管设置示例[Prometheus -monitoring repo]（https://github.com/qdrant/prometheus-monitoring）


混合云抓取

当：运行Qdrant混合云并且需要集群级可见性时使用。

不要只刮Qdrant节点。在混合云中，您管理Kubernetes数据平面。您还必须抓取集群导出器和操作符pod，以获得完整的集群可见性和操作符状态。混合云普罗米修斯安装教程[混合云普罗米修斯]（https://search.qdrant.tech/md/documentation/tutorials-and-examples/hybrid-cloud-prometheus/）
-官方的Grafana仪表盘[Grafana仪表盘回购]（https://github.com/qdrant/qdrant-cloud-grafana-dashboard）


活跃度和准备度探测器

当：配置Kubernetes健康检查时使用。

-使用`/healthz`，`/livez`，`/readyz`的基本状态，活动和准备[Kubernetes健康端点]（https://search.qdrant.tech/md/documentation/operations/monitoring/?s=kubernetes-health-endpoints）


# #报警

当：为生产或混合云部署设置警报时使用。

混合云提供了~11个预先配置的Prometheus警报[云集群监控]（https://search.qdrant.tech/md/documentation/cloud/cluster-monitoring/）
-使用AlertmanagerConfig将警报路由到Slack， PagerDuty或其他基于标签的目标
-至少，警报：优化器错误，节点未准备好，复制因子低于目标，磁盘使用率>80%


日志集中和审计日志

在以下情况下使用：企业遵从性需要集中的日志或审计跟踪。-启用JSON日志格式进行结构化分析：在config [Configuration]（https://search.qdrant.tech/md/documentation/operations/configuration/）中设置`logger.format`为`json`—使用“FluentD/OpenSearch”进行日志聚合
—审计日志（v1.17+）写入本地文件系统（`/qdrant/storage/audit/`），而不是stdout。挂载一个Persistent Volume并部署一个sidecar容器来跟踪这些文件到stdout，以便DaemonSets可以接收它们。(审计日志记录)(https://search.qdrant.tech/md/documentation/operations/security/?s=audit-logging)


不要做什么

-在自托管上抓取`/sys_metrics`（仅适用于Qdrant Cloud）
-只抓取混合云中的Qdrant节点（遗漏集群导出器和操作器指标）
-在投入生产之前跳过监控设置（你会后悔的）
-警告页面缓存内存使用（它应该填满可用RAM，正常的操作系统行为）