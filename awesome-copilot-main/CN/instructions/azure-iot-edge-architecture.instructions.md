---
description: 'Require Azure IoT Edge documentation review before proposing edge IoT architectures or Azure implementation guidance.'
applyTo: '**/*.bicep,**/*.tf,**/*iot*.md,**/*smart-city*.md,**/*edge*.md'
---
## Azure物联网边缘架构指令

当任务包括Azure物联网、智慧城市、边缘处理、网关设计或断开连接的边缘场景时，请在提供架构建议之前执行此操作：

1. 首先查看Azure IoT Edge文档：   - https://learn.microsoft.com/azure/iot-edge/
   - https://learn.microsoft.com/es-es/azure/iot-edge/
2. 确认文件中的关键约束：
-运行时架构
-支持的系统
-Version/releasestatus
—相关的Linux/Windows快速入门路径
3. 明确声明你已经审阅了文档，或者声明无法查阅。
4. 如果无法访问文档，请继续明确标记假设。

响应规则

-在没有验证边缘适用性之前，不要直接跳转到服务列表。
-始终解释为什么需要或不需要物联网边缘。
-包括操作影响：更新策略，可观察性和支持模型。
—安全默认值优先级：受管身份、最小权限、秘密管理和网络隔离。