---
applyTo: 'k8s/**/*.yaml,k8s/**/*.yml,manifests/**/*.yaml,manifests/**/*.yml,deploy/**/*.yaml,deploy/**/*.yml,charts/**/templates/**/*.yaml,charts/**/templates/**/*.yml'
description: 'Best practices for Kubernetes YAML manifests including labeling conventions, security contexts, pod security, resource management, probes, and validation commands'
---
# Kubernetes manifest Instructions

你的使命

创建生产就绪的Kubernetes清单，优先考虑安全性、可靠性和卓越的运营，并提供一致的标签、适当的资源管理和全面的健康检查。

标签约定

**必需的标签** （Kubernetes推荐）：
—`app.kubernetes.io/name`：应用名称
—`app.kubernetes.io/instance`：实例标识符
—`app.kubernetes.io/version`：版本号
—`app.kubernetes.io/component`：组件角色
—`app.kubernetes.io/part-of`：应用组
—`app.kubernetes.io/managed-by`：管理工具

* * * *额外的标签:
—`environment`：环境名称
-`team`：所属团队
—`cost-center`：用于计费

* * * *有用的注释:
-文件和所有权
—监控：`prometheus.io/scrape`、`prometheus.io/port`、`prometheus.io/path`-变更跟踪：git提交，部署日期

## SecurityContext默认值

* * Pod-level * *:
——`runAsNonRoot: true`—`runAsUser`、`runAsGroup`：指定id
—`fsGroup`：文件系统组
——`seccompProfile.type: RuntimeDefault`* * * *集装箱层面:
——`allowPrivilegeEscalation: false`-`readOnlyRootFilesystem: true`（带有TMPFS挂载的可写目录）
-`capabilities.drop: [ALL]`（只添加需要的内容）

Pod安全标准

使用Pod安全许可：
—**限制**（建议用于生产）：进行安全加固
- **基线**：最低安全要求
—在命名空间级别应用

资源请求和限制

* *总是定义* *:
-请求：保证最小（调度）
-限制：最大允许（防止耗尽）

* * * * QoS类:
**保证**：请求==限制（最适合关键应用）
- **Burstable**：请求<限制（灵活的资源使用）
**BestEffort**：没有资源定义（避免在生产中使用）

##健康探测器

**活性**：重新启动不健康的容器
**准备**：控制流量路由
**启动**：保护启动缓慢的应用程序

为每一项配置适当的延迟、周期、超时和阈值。##推出策略

* * * *部署策略:
—`RollingUpdate`，包含`maxSurge`和`maxUnavailable`—设置“`maxUnavailable: 0`”为零停机时间

* * * *:高可用性
-最少2-3个副本
-豆荚中断预算（PDB）
-反亲和规则（横跨nodes/zones）
-水平吊舱自动缩放器（HPA）可变负载

##验证命令

* *用于* *:
——`kubectl apply --dry-run=client -f manifest.yaml`——`kubectl apply --dry-run=server -f manifest.yaml`-`kubeconform -strict manifest.yaml`（模式验证）
-`helm template ./chart | kubeconform -strict`（掌舵）

* * * *政策验证:
- OPA Conftest， Kyverno或Datree

## Rollout & Rollback

* * * *部署:
——`kubectl apply -f manifest.yaml`——`kubectl rollout status deployment/NAME`* *回滚* *:
——`kubectl rollout undo deployment/NAME`——`kubectl rollout undo deployment/NAME --to-revision=N`——`kubectl rollout history deployment/NAME`* *重启* *:
——`kubectl rollout restart deployment/NAME`清单清单—[]标签：使用的标准标签
-[]注释：文档和监控
-[]安全：runAsNonRoot， readOnlyRootFilesystem，掉落的能力
—[]资源：定义的请求和限制
-[]探针：激活，准备，启动配置
-[]图片：特定标签（never:latest）
-[]副本：生产最少2-3个
-[]策略：滚动更新适当的surge/unavailable—[]PDB：为生产定义
—[]非亲和性：HA配置
-[]安全关机：terminationGracePeriodSeconds设置
-[]验证：Dry-run和kubeconform通过
—[]“Secrets”：在“Secrets”资源中，而不是在“ConfigMaps”中
- [] NetworkPolicy：最低权限访问（如果适用）

最佳实践总结1. 使用标准的标签和注释
2. 始终以非root身份运行，并删除功能
3. 定义资源请求和限制
4. 实现所有三种探测类型
5. 将图像标签固定到特定版本
6. 配置HA的反亲和性
7. 设定豆荚中断预算
8. 使用零不可用的滚动更新
9. 申请前验证清单
10. 尽可能启用只读根文件系统