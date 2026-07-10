# Arduino官方参考和最佳实践

在完成固件或硬件指导之前，请使用这些官方Arduino资源。

官方参考资料

- Arduino主指南：<https://www.arduino.cc/en/Guide>- Arduino文档主页：<https://docs.arduino.cc/>—入门路径：<https://docs.arduino.cc/learn/starting-guide/getting-started-arduino/>- Arduino IDE使用：<https://docs.arduino.cc/learn/starting-guide/the-arduino-software-ide/>- Arduino语言参考：<https://docs.arduino.cc/language-reference/>Arduino编程参考概述：<https://docs.arduino.cc/learn/programming/reference/>- Arduino内存指南：<https://docs.arduino.cc/learn/programming/memory-guide/>Arduino调试基础：<https://docs.arduino.cc/learn/microcontrollers/debugging/>- Arduino低功耗设计指南：<https://docs.arduino.cc/learn/electronics/low-power/>- Arduino通信协议索引：<https://docs.arduino.cc/learn/communication/>Arduino风格指南库：<https://docs.arduino.cc/learn/contributions/arduino-library-style-guide/>固件最佳实践-保持`loop()`不阻塞；避免在生产逻辑中长时间使用`delay()`。
—对于周期任务，使用基于`millis()`的调度策略。
—明确预算SRAM，避免热路径下的动态分配。
-验证传感器范围，并为无效读数提供安全的默认值。
—增加启动自检和健康周期心跳消息。
-版本每个遥测流中的有效载荷模式和固件版本。
-实现重试与指数回退和抖动的网络操作。
-存储源代码之外的凭据，并根据策略轮换它们。

##硬件和电源最佳实践

-记录每个外设的电压水平、引脚映射和电流限制。
—针对停电和电力波动场景进行设计。
-使用看门狗和安全恢复行为。
-为电池部署规划低功耗模式并验证唤醒周期。Azure物联网的集成最佳实践

—首选安全传输（MQTT优于TLS）和每个设备的身份。
-定义重复消息场景的幂等上游处理。
-包括设备健康指标（正常运行时间、复位原因、RSSI（如适用））。
-验证离线缓冲边界，以避免不受控制的内存增长。