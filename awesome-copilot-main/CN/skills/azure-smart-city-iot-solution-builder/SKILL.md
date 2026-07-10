---
name: azure-smart-city-iot-solution-builder
description: 'Design and plan end-to-end Azure IoT and Smart City solutions: requirements, architecture, security, operations, cost, and a phased delivery plan with concrete implementation artifacts.'
---
# Azure智慧城市物联网解决方案构建器

使用此技能可为Azure物联网和智慧城市解决方案重建和标准化完整的工作流程。

##何时使用

当用户提出以下要求时使用此技能：

“我想在Azure上构建一个物联网解决方案”
-“交通、照明或废物的智慧城市建筑”
-“如何连接设备、分析和警报？”
“我需要一个城市平台的路线图和待办事项”

# #目标

将高层次的想法转化为可部署的架构。
尽可能重用现有的专注于azure的技能。
-生成团队可以实现的具体工件。

# #工作流程

强制文档审查（在任何架构之前）

在提出涉及边缘计算的架构或技术决策之前，请先查看Azure IoT edge文档：- https://learn.microsoft.com/azure/iot-edge/
最少审查页数：

-什么是Azure IoT Edge
-运行时架构
-支持的系统
-history/release版本说明
—场景相关的Linux/Windows快速入门

如果无法查阅文档，请明确说明这一点，并继续明确标记假设。

### 1)范围和约束

收集并确认：

-城市领域：交通、停车、空气质量、水、能源、公共安全、废物处理等。
-规模：设备数量，遥测频率，保留，区域。
-延迟和可用性目标。
-监管和隐私约束。
-整合现有系统（SCADA， GIS， ERP，票务，api）。

能力图

将平台分成几层：-设备和边缘：入职、身份、固件、OTA、边缘处理。
-摄取和消息传递：命令和控制，事件路由，缓冲。
-数据和分析：热路径vs冷路径，仪表板，历史分析。
-操作：可观察性，事件流，slo。
-治理：RBAC、机密、策略、网络隔离。

Azure服务选择（参考）

-设备连接：Azure IoT Hub， Azure IoT Operations, IoT Edge。
-事件流：事件中心，服务总线，事件网格。
-存储：Blob存储，数据湖，Cosmos DB， SQL。
-分析：Azure数据资源管理器，流分析，Fabric/Synapse.- API和应用：API管理，应用服务，容器应用，函数。
-监控：Azure监控，应用程序洞察，日志分析。
-安全：密钥库，物联网防御者，私有端点，管理身份。

4)非功能性设计

定义并记录：-可靠性模型（zones/regions，重试，死信处理，重放）。
-安全控制（零信任、加密、秘密轮换、最小权限）。
-成本控制（保留层、适当调整规模、自动扩展、工作负载调度）。
-数据生命周期（原始、策划、聚合、存档）。

5)交付计划

创建分阶段执行：

-第一阶段：试验区或单一用例。
-阶段2：多域集成。
-第三阶段：城市规模的推广和优化。

对于每个阶段，包括：

-退出标准
- - - - - -依赖关系
-风险和缓解措施
- KPI设置

首先重用其他技能

技能有两种来源：

-运行时提供的技能（在此存储库外部）：仅当Copilot主机环境公开它们时可用。
—本地存储库技能（本存储库）：作为`skills/`下的本地文件提供。

运行时提供的Azure技能（可选）如果它们在执行环境中可用，请委托这些专门技能以获得更深入的指导：

——`azure-kubernetes`——`azure-messaging`——`azure-observability`——`azure-storage`——`azure-rbac`——`azure-cost`——`azure-validate`——`azure-deploy`本地存储库替代方案（在此repo中使用）

当运行时技能不可用时，优先考虑此存储库中现有的本地技能：

-`azure-architecture-autopilot`用于架构生成和细化。
-`azure-resource-visualizer`为资源关系图。
-`azure-role-selector`为角色选择指导。
-`az-cost-optimize`和`azure-pricing`的成本和定价分析。
—`azure-deployment-preflight`用于部署前检查。
-`appinsights-instrumentation`为遥测仪表模式。

如果没有专门的技能可用，继续使用该技能并保持假设明确。

需要的输出工件

始终提供这些输出：1. 智慧城市解决方案总结（范围、假设、约束）。
2. 参考体系结构（组件和数据流）。
3. 安全和治理检查表。
4. 成本和规模策略。
5. 分阶段实现待办事项（史诗和里程碑）。

##输出模板

使用这个回应结构：

1. 背景和目标
2. 建议的体系结构
3. 技术决策和权衡
4. 安全、操作和成本控制
5. 分阶段实施计划
6. 风险和悬而未决的问题

# #指南

—在验证先决条件之前，不要跳转到部署。
—对于关键的城市工作负载，不建议单区域生产。
不要忽略运营所有权（谁处理事件、sla、变更窗口）。
-明确区分假设和已证实的事实。