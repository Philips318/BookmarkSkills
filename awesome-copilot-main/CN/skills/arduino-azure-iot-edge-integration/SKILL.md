---
name: arduino-azure-iot-edge-integration
description: 'Design and implement Arduino integration with Azure IoT Hub and IoT Edge, including secure provisioning, resilient telemetry, command handling, and production guardrails.'
---
# Arduino Azure IoT Edge集成

当用户需要将arduino类设备连接到Azure IoT时，特别是在边缘重场景（网关、间歇性网络、离线缓冲和本地驱动）时，使用此技能。

##何时使用

使用此技能处理以下请求：

-“我想将Arduino传感器连接到Azure”
-“如何将MQTT遥测数据发送到物联网中心？”
“我需要一个用于现场设备的边缘网关”
“我想要云到设备的命令和OTA配置更新”

强制文档审查

在推荐IoT Edge拓扑或运行时行为之前，请查看：- https://learn.microsoft.com/azure/iot-edge/
如果无法查阅文档，则继续进行明确的假设，并在专门的部分中突出显示它们。

官方Arduino参考资料和最佳实践（必选）

在提出固件、布线或通信实现细节之前，请先查阅官方Arduino源代码：- https://www.arduino.cc/en/Guide
- https://docs.arduino.cc/
- https://docs.arduino.cc/language-reference/
- references/arduino-official-best-practices.md
在选择实现方案时，优先考虑官方Arduino指南而不是社区代码，除非有明确的技术原因需要偏离。

# #目标

-生成从Arduino设备到云洞察的安全端到端参考路径。
-处理不稳定的链接（存储转发，重试，幂等）。
-定义可操作的设备和云待办事项。

集成模式

模式A: Arduino直接到IoT Hub

当连接稳定且云延迟可接受时使用。

—协议：MQTT over TLS。
-身份：每个设备的凭据（SAS或X.509）。
遥测有效载荷：压缩JSON与时间戳，设备ID，指标，和可选的质量标志。

模式B: Arduino到本地网关，然后是IoT Edge

当链接受到约束、需要本地控制或批处理改进cost/reliability.时使用- Arduino与本地网关（串行，BLE，本地MQTT， RS-485， Modbus桥）通信。
—网关通过IoT Edge运行时向上游发布数据，并将数据路由到IoT Hub。
—本地模块可以过滤、聚合和触发操作，即使在云中断期间。

##设计流程

设备合同

定义:

-传感器目录和单位。
-采样频率和预期吞吐量。
-消息模式版本控制策略。
-Desired/reported设备双属性控制运行时行为。

安全基线

要求:

—每个设备的唯一标识。
-在源代码或固件工件中没有硬编码的秘密。
-证书轮换策略。
-在可能的情况下，签名固件和受控更新过程。

可靠性和离线行为

计划和文件：-畏畏缩缩。
-有界大小的本地queue/buffer策略。
-重复抑制或下游幂等处理。
-回退到最后已知的配置。

### 4)云和边缘路由

定义路由：

-对冷库的原始遥测。
-策划遥测热分析。
—提醒操作通道。
—命令和配置返回到edge/device.5)可观察性

指定最小操作遥测：

—设备心跳和固件版本。
—连接状态转换。
—消息发送success/error计数器。
—网关模块运行状况和重启原因。

重用其他技能

相关时，结合：

-`azure-smart-city-iot-solution-builder`用于全市建筑和分阶段推出。
-`azure-resource-visualizer`表示关系图。
-`appinsights-instrumentation`用于应用程序和服务遥测模式。

还要使用`references/arduino-official-best-practices.md`作为固件和硬件推荐的质量基线。

##所需输出总是提供:

1. 选择的连接模式和基本原理。
2. 消息契约（字段、单元、样本有效负载）。
3.identity/credentials/updates.的安全检查表
4. 可靠性计划（重试、缓冲、重复数据删除）。
5. 实现积压（固件、网关、云）。

##输出模板

1. 情景和假设
2. 推荐的架构
3. 设备和网关合同
4. 安全性和可靠性控制
5. 部署计划和验证测试

# #指南

-不要建议在设备间使用共享凭证进行生产部署。
-不要假设在现场部署中始终保持连接。
—执行器场景下，不要忽略命令授权和审计。