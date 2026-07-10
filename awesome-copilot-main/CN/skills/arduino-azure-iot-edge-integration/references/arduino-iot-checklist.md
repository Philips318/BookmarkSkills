# Arduino Azure IoT检查表

在完成体系结构或实现指南之前，请使用此检查表。

官方Arduino基线

-从<https://www.arduino.cc/en/Guide>和<https://docs.arduino.cc/>审核的官方参考资料。
-根据<https://docs.arduino.cc/language-reference/>验证Language/API调用。
-从`references/arduino-official-best-practices.md`审查的最佳实践。

## 1)设备配置文件

- MCU模型和内存约束文件。
-定义传感器列表和采样策略。
-电源型号文档（电源，电池，睡眠周期）。

2)连接性

-选定的传输文件（MQTT优于TLS）。
-定义网络故障行为。
-当设备缺少RTC同步时，定义本地时间戳策略。

3)安全性

—每个设备的唯一标识。
-在源代码控制中没有秘密。
-证书轮岗计划文件。
-固件更新和回退计划文件。

## 4)边缘和云流-从边缘到物联网中心的路由记录。
—已定义离线缓冲限制。
-记录重复处理策略。
—定义告警阈值和目的地。

5)验证

—连通性浸泡测试场景。
—丢包重连测试。
—命令授权测试。
-固件版本和运行状况报告验证。