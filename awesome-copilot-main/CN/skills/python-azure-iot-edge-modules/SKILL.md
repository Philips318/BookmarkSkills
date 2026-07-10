---
name: python-azure-iot-edge-modules
description: 'Build and operate Python Azure IoT Edge modules with robust messaging, deployment manifests, observability, and production readiness checks.'
---
# Python Azure IoT Edge模块

使用此技能设计、实现和验证基于python的IoT Edge模块，用于遥测处理、本地推理、协议转换和边缘到云集成。

##何时使用

使用此技能的请求如下：

-“quiero创建无模Python para IoT Edge”
-“como despliego模块边缘显示”
-“必要的filtrar/agregar遥测设备”
-“共同的管理方式通过保留的方式来消除约束”

强制文档审查

在推荐运行时行为或部署决策之前，请查看：- https://learn.microsoft.com/azure/iot-edge/
- https://learn.microsoft.com/es-es/azure/iot-edge/
最小的检查:

-运行时架构和模块生命周期。
—支持的主机操作系统及版本。
—部署模型和配置流程。
-当前release/version指导。

如果无法获取文档，则进行明确的假设并清楚地标记它们。

Python官方参考和最佳实践（必选）

在提出Python实现细节之前，请查阅官方Python源代码：- https://www.python.org/
- https://docs.python.org/3/
- https://docs.python.org/3/reference/
- https://docs.python.org/3/library/
- references/python-official-best-practices.md
除非有特定的兼容性原因，否则更喜欢官方文档而不是社区代码。

# #目标

交付以生产为中心的模块架构和实施计划。
-确保在网络可变性下可靠的边缘消息传递。
-提供部署、可观察性和验证工件。

模块用例

—协议适配器（serial/Modbus/OPC-UA到IoT消息格式）。
-遥测丰富和规范化。
—本地异常检测或推断。
-命令编排和本地执行器控制。

##交付流程

契约和接口

定义:

—模块输入输出。
—消息模式和版本控制策略。
-正常和关键遥测的路由和优先级。
—动态配置所需的属性。

### 2)运行时和打包

指定:- Python运行时版本目标。
-容器映像策略（基本映像、小占用空间、CVE卫生）。
—资源配置文件（CPU/memory边界）。
—启动和健康检查。

3)可靠性设计

实施和验证：

-重试与指数回退和抖动。
—上游故障的优雅降级。
-需要时使用本地排队策略。
-对重播消息进行幂等处理。

4)安全控制

要求:

-在代码或清单中没有明文秘密。
-最小权限模块行为。
-安全传输和可信证书链处理。
-命令处理和状态变化的可追溯性。

### 5)部署和操作

定义:

—特定于环境的部署清单。
-推广策略（试点，分阶段，广泛）。
—回滚条件。
—SLOs和告警条件。

重用其他技能

相关时，结合：-`azure-smart-city-iot-solution-builder`用于平台级架构。
-`appinsights-instrumentation`为遥测仪器方法。
-`azure-resource-visualizer`用于架构图和依赖关系映射。

还可以使用`references/python-official-best-practices.md`作为模块设计和实现指导的基线质量标准。

##所需输出

总是提供:

1. 模块设计概述（目的、输入、输出）。
2. 部署模型（映像、清单、环境设置）。
3. 可靠性和错误处理策略。
4. 安全和操作清单。
5. 测试矩阵（功能、混乱、性能、回滚）。

##输出模板

1. 背景和假设
2. 模块架构
3. 部署和配置
4. 可靠性、安全性、可观察性
5. 验证和推出计划

# #护栏-不建议在没有试验阶段的情况下直接生产。
-不要在Dockerfiles、source或manifest中嵌入机密文件。
—不要忽略健康探测、重启行为和回滚条件。