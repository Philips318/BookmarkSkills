# minipro芯片编程实用程序规范

# # 1。概述

**minipro**是一个开源的命令行实用程序，用于**编程，读取，擦除和验证**各种**EEPROM， Flash， EPROM， SRAM， GAL和逻辑设备**，使用支持的通用程序员，如**T48**， **TL866II Plus**和兼容型号。

它广泛应用于**Linux**、**macOS**和**Windows**环境，特别是**逆向计算**、**固件开发**和**电子原型**。

---

# # 2。支持程序员

|程序员|笔记|| ------------ | -------------------------- |
| T48 |完全支持（推荐）|
| TL866II Plus |完全支持|
| TL866A / CS |有限公司/传统支持|

---

# # 3。支持的设备类型

3.1内存设备

并行EEPROM（如AT28C256）
*闪存（29xxx系列）
* EPROM （27xxx系列）
* SRAM（仅限read/verify）

3.2逻辑和pld

* gal16v8 / gal22v10
* PAL设备（有限）

3.3其他设备

*一些微控制器（设备相关）
*逻辑IC测试（选定型号）

---

# # 4。安装

4.1 Linux```bash
sudo apt install minipro
```
或者从来源：```bash
git clone https://github.com/vdudouyt/minipro.git
make
sudo make install
```
4.2 Windows

*通过MSYS2或预构建的二进制文件安装
*需要libbusb驱动（WinUSB）

---

# # 5。基本命令格式```bash
minipro [options]
```
常见的选项:

|选项|描述|| ------------- | ---------------------- |
|`-p <device>`|选择目标设备|
|`-r <file>`|读取设备到|文件
|`-w <file>`|向设备|写入文件
|`-e`|擦除设备|
|`-v`|校验|内容
|`-I`|设备信息|
|`-l`|列出支持的设备|

---

# # 6。常用编程操作

6.1列出支持的设备```bash
minipro -l
```
### 6.2识别设备```bash
minipro -p AT28C256 -I
```
读取芯片```bash
minipro -p AT28C256 -r rom_dump.bin
```
6.4写芯片```bash
minipro -p AT28C256 -w rom.bin
```
### 6.5仅验证```bash
minipro -p AT28C256 -v rom.bin
```
---

# # 7。eeprom编程（AT28C256示例）```bash
minipro -p AT28C256 -w monitor.bin
```
*自动处理软件数据保护
*写周期延迟是内部管理的
*编程后执行的验证

---

# # 8。闪存编程```bash
minipro -p SST39SF040 -e -w firmware.bin
```
* Flash设备需要擦除步骤
*扇区擦除自动处理

---

# # 9。EPROM操作```bash
minipro -p 27C256 -r eprom.bin
```
*重新编程前需要UV擦除
* minipro在写入前校验空白状态

---

# # 10。加编程```bash
minipro -p GAL22V10 -w logic.jed
```
*使用JEDEC文件
*支持读、写、校验
*保险丝地图可通过`-I`查看

---

# # 11。错误处理和消息

|消息|含义|| ---------------------- | -------------------------- |
|`Device not found`|设备选择错误|
|`Verification failed`|数据不匹配|
|`Chip protected`|开启写保护|
|`Overcurrent detected`|插入或接线错误|

---

# # 12。安全及最佳作业守则

*始终确认设备方向在ZIF插座
*使用正确的设备标识符（`-p`）
*操作时请勿热插芯片
*使用PLCC， SOP， TSOP包的适配器

---

# # 13。典型的逆向计算工作流程

1. 组装ROM映像
2. 使用minipro + T48编程EEPROM
3. 验证内容
4. 在SBC中安装芯片
5. 测试系统启动

---

# # 14。限制

*并非所有设备都支持
*一些微控制器需要专用工具
不支持在线编程（ISP）

---

# # 15。参考文献* <https://gitlab.com/DavidGriffith/minipro>
* <https://www.hadex.cz/spec/m545b.pdf>
* <https://github.com/mikeroyal/Firmware-Guide>
* <https://mike42.me/blog/2021-08-a-first-look-at-programmable-logic>
* <https://retrocomputingforum.com/>

---
