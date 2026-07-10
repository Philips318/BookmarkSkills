---
name: qdrant-monitoring
description: "Guides Qdrant monitoring and observability setup. Use when someone asks 'how to monitor Qdrant', 'what metrics to track', 'is Qdrant healthy', 'optimizer stuck', 'why is memory growing', 'requests are slow', or needs to set up Prometheus, Grafana, or health checks. Also use when debugging production issues that require metric analysis."
allowed-tools:
  - Read
  - Grep
  - Glob
---
# Qdrant监控

Qdrant监视允许跟踪部署的性能和运行状况，并在问题变成中断之前识别问题。首先确定是否需要设置监视或诊断活动问题。

-了解可用的指标[监控文档]（https://search.qdrant.tech/md/documentation/operations/monitoring/）


##监控设置

普罗米修斯数据抓取、健康探测、混合云细节、警报和日志集中。(监控设置)(setup/SKILL.md)


##调试指标

优化器卡住，内存增长，请求缓慢。使用指标诊断生产活动问题。[参数调试]（debugging/SKILL.md）