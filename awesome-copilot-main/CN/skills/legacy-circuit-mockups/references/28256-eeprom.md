# AT28C256 256K （32K x 8）并行EEPROM规格

# # 1。概述

AT28C256是由Atmel（现为Microchip）制造的非易失性，电可擦除和可编程只读存储器（EEPROM）。它提供**256 Kbits**的存储空间，组织为**32,768 ** 8位**，通常用于基于**6502**、**Z80**和类似cpu的8位微处理器系统。

设备支持字节级写操作、快速读访问和软件控制的数据保护。

---

# # 2。一般特征

|特性|描述|| -------------- | ------------------------------ |
|内存大小| 256kbits (32kb) |
|组织| 32,768 × 8比特|
|数据总线| 8位|
|地址总线| 15位（A0-A14） |
|技术| EEPROM |
|续航力|写周期≥10万次|
|数据保留|≥10年|
|访问时间| 150- 250ns(取决于变量
封装类型| DIP-28， plc -32， TSOP

---

# # 3。引脚配置（逻辑）

3.1地址引脚（A0-A14）

*从32,768个内存位置中选择一个

3.2数据引脚（I/O0-I/O7）

*双向三态数据总线
*在读取周期内输出有效

控制引脚

|引脚|描述|| --- | -------------------------- |
| CE | Chip Enable (active low) |
| OE | Output使能（active low） |
| WE | Write Enable (active low) |
| VCC | + 5v电源|
|接地|接地|

---

# # 4。内存的组织

*线性地址空间从`$0000`到`$7FFF`*每个地址对应一个8位字节```text
Address Range: 0000h - 7FFFh
Data Width:    8 bits
```
---

# # 5。读操作

5.1读循环条件

|信号|状态|| ------ | ----- |
| ce | low |
b|低b|
|我们|高|

*数据在访问时间后出现在I/O引脚上
*当CE和OE被断言时，输出仍然有效
*当CE或OE为HIGH时输出为高阻抗

---

# # 6。写操作

### 6.1字节写周期

|信号|状态|| ------ | --------- |
| ce | low |
| oe | high |
| WE |低脉冲|

*地址和数据必须在WE低脉冲期间稳定
*内部写周期时间≈10ms （max）
*设备自动处理写前擦除

---

# # 7。软件数据保护（SDP）

AT28C256包括可选的**软件数据保护**，以防止意外写入。

7.1 SDP使能顺序```text
Write $AA to address $5555
Write $55 to address $2AAA
Write $A0 to address $5555
```
7.2 SDP禁用顺序```text
Write $AA to address $5555
Write $55 to address $2AAA
Write $80 to address $5555
Write $AA to address $5555
Write $55 to address $2AAA
Write $20 to address $5555
```
---

# # 8。写周期定时说明

*写是内部定时的；不需要外部轮询
*在写周期中，读返回未定义的数据
*设备在繁忙时忽略额外的写尝试

---

# # 9。数据轮询（可选）

*I/O7可能在写过程中被监视
*当I/O7匹配写数据时，写完成

---

# # 10。复位和电源行为

*没有明确的复位引脚
*在上电和断电期间写入被抑制
*输出默认为高阻抗，直到CE和OE断言

---

# # 11。典型系统集成（6502示例）```text
Address Range: $8000 - $FFFF
A15 used as chip select
OE  R/W?
WE  inverted R/W?
```
---

# # 12。绝对最高评级（摘要）

|参数|评级|| ------------- | --------------------- |
| VCC | -0.6 V至+6.25 V |
|输入电压| -0.6 V转VCC + 0.6 V |
|存储温度| -65℃~ +150℃|

---

# # 13。变体和兼容设备

|设备| Notes || ---------------- | ---------------------------- |
| AT28C256 |原Atmel |
| AT28C256F |更快的接入时间|
| SST28SF256 | flash兼容备选|
| 28C256（通用）|普通引脚兼容的EEPROM |

---

# # 14。常用用例

*在复古系统中更换ROM
*固件存储
*微型计算机监视器和BASIC只读存储器
*原型制作和电脑爱好者

---

# # 15。参考文献* <https://www.utmel.com/components/at28bv256-eeproms-pinout-equivalent-and-datasheet?id=1019>
* <https://www.futurlec.com/Memory/28C256.shtml>
* <https://ww1.microchip.com/downloads/en/DeviceDoc/doc0006.pdf>
* <https://bread80.com/2020/08/10/the-ben-eater-eeprom-programmer-28c256-and-software-data-protection/>

---
