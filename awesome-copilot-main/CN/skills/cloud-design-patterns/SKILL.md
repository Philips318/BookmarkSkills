---
name: cloud-design-patterns
description: 'Cloud design patterns for distributed systems architecture covering 42 industry-standard patterns across reliability, performance, messaging, security, and deployment categories. Use when designing, reviewing, or implementing distributed system architectures.'
---
#云设计模式

架构师通过集成平台服务、功能和代码来设计工作负载，以满足功能性和非功能性需求。要设计有效的工作负载，您必须了解这些需求，并选择解决工作负载约束挑战的拓扑和方法。云设计模式为许多常见的挑战提供了解决方案。

系统设计很大程度上依赖于已建立的设计模式。您可以通过使用这些模式的组合来设计基础设施、代码和分布式系统。这些模式对于在云中构建可靠、高度安全、成本优化、操作高效和高性能的应用程序至关重要。以下云设计模式与技术无关，这使得它们适用于任何分布式系统。您可以跨Azure、其他云平台、本地设置和混合环境应用这些模式。

云设计模式如何增强设计过程

云工作负载容易受到分布式计算谬误的影响，这是关于分布式系统如何运行的常见但错误的假设。这些谬论的例子包括：

—网络可靠。
—延迟为零。
—带宽无限。
—网络安全。
-拓扑不会改变。
-只有一个管理员。
-组件版本控制很简单。
—可观察性的实现可能会延迟。这些误解可能导致有缺陷的工作负载设计。设计模式并不能消除这些误解，但有助于提高认识，提供补偿策略，并提供缓解措施。每种云设计模式都有利弊。关注为什么应该选择特定的模式，而不是如何实现它。

---

# #引用

|参考|何时加载||---|---|
|[可靠性和弹性模式](references/reliability-resilience.md) |大使、隔离墙、断路器、补偿事务、重试、健康端点监控、Leader选举、Saga、顺序护航|
|[性能模式](references/performance.md) |异步请求-应答、缓存预留、CQRS、索引表、物化视图、优先队列、基于队列的负载均衡、速率限制、分片、节流|
|[消息传递和集成模式](references/messaging-integration.md) |编排、声明检查、竞争消费者、消息传递桥、管道和过滤器、发布者-订阅者、调度程序代理监督|
|[架构与设计模式](references/architecture-design.md) |反腐败层，后端为前端，网关Aggregation/Offloading/Routing， Sidecar， Strangler图|
|[部署和操作模式](references/deployment-operational.md) |计算资源整合，部署戳记，外部配置存储，Geode，静态内容托管|
|[安全模式](references/security.md) | Feder关联身份、隔离、代客钥匙|
|[事件驱动架构模式](references/event-driven.md) |事件溯源|
|[最佳实践和模式选择](references/best-practices.md) |选择合适的模式，良好架构框架对齐，文档，监控|
| [Azure服务映射](references/azure-service-mappings.md) |每个模式类别的公共Azure服务|---

一目了然的模式分类

|类别|模式|焦点||---|---|---|
|可靠性与弹性| 9种模式|容错、自愈、优雅退化|
|缓存，扩展，负载管理，数据优化|
|解耦、事件驱动通信、工作流协调|
|系统边界，API网关，迁移策略|
|基础设施管理、地理分布、配置|
|安全| 3模式|身份、访问控制、内容验证|
|事件驱动架构| 1模式|事件溯源和审计跟踪|

##外部链接

-[云设计模式- Azure架构中心]（https://learn.microsoft.com/azure/architecture/patterns/）
- [Azure架构良好的框架]（https://learn.microsoft.com/azure/architecture/framework/）