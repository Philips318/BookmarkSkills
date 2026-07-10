# 6502 SBC汇编和ROM构建规范

# #概述

本文档定义了为单板计算机编译6502汇编语言的完整规范，包括：

** * mos 6502 cpu **
** * mos 6522 via **
** * as6c62256 (32kb sram)**
** * at28c256 (32 KB eeprom / rom)**
** *DFRobot FIT0127 (hd44780兼容16x2 LCD)**

重点是工具链行为、内存布局、ROM构造和固件约定，而不是电气布线。

---

目标系统架构

内存映射（Canonical）```
$0000-$00FF  Zero Page (RAM)
$0100-$01FF  Stack (RAM)
$0200-$7FFF  General RAM (AS6C62256)
$8000-$8FFF  6522 VIA I/O space
$9000-$FFFF  ROM (AT28C256)
```
>地址解码可以镜像设备；Assembler假定这种规范布局。

---

## ROM组织（AT28C256）

|地址|用途|| ----------- | -------------------- |
| $9000-$FFEF |程序代码+数据|
| $FFF0-$FFF9 |可选系统数据|
| $FFFA-$FFFB | NMI向量|
| $FFFC-$FFFD |复位向量|
| $FFFE-$FFFF |IRQ/BRK向量|

ROM映像大小：**32,768字节**

---

##重置和启动约定

重置:

1. CPU在`$FFFC`取RESET向量
2. 代码初始化堆栈指针
3. 初始化零页变量
4. 通过配置
5. 液晶初始化
6. 进入主程序

---

##汇编要求

汇编**必须**支持：`.org`绝对寻址
*符号标签
*二进制输出（`.bin`）
*小端字发射
*零页优化

建议汇编:

** *ca65** （cc65工具链）
* * * vasm6502 * *
* * * * * 64一杯的量

---

汇编源结构```asm
;---------------------------
; Reset Vector Entry Point
;---------------------------
        .org $9000
RESET:
        sei
        cld
        ldx #$FF
        txs
        jsr init_via
        jsr init_lcd
MAIN:
        jsr lcd_print
        jmp MAIN
```
---

向量表定义```asm
        .org $FFFA
        .word nmi_handler
        .word RESET
        .word irq_handler
```
---

VIA编程模型

寄存器映射（Base = $8000）

|偏移量|寄存器|| ------ | -------- |
| $0 | orb |
| $1 |或|
| $2 | DDRB |
| $3 | ddra |
| $4 | t1cl |
| $5 | t1ch |
| $6 | $ 11 |
| $7 | t1lh |
| $8 | $ 2 | $ 2 |
| $9 | t2ch |
| $ b | acr |
| PCR |
| $ d | ifr |
| $ e | ier |

---

LCD接口约定

LCD接线假设

| LCD |到|| ----- | ------- |
| d4-d7 | pb4-pb7 |
| rs | pa0 |
| e | pa1 |
|R/W| GND |

假定为4位模式。

---

LCD初始化顺序```asm
lcd_init:
        lda #$33
        jsr lcd_cmd
        lda #$32
        jsr lcd_cmd
        lda #$28
        jsr lcd_cmd
        lda #$0C
        jsr lcd_cmd
        lda #$06
        jsr lcd_cmd
        lda #$01
        jsr lcd_cmd
        rts
```
---

LCDCommand/Data接口

|操作| RS |数据|
| --------- | -- | --------------- |
|命令| 0 |指令|
|数据| 1 | ASCII字符|

---

零页使用约定

|地址|用途|| ------- | ------------ |
| $00-$0F | Scratch |
| $10-$1F | LCD例程|
| $20-$2F | VIA state |
| $30-$FF |用户自定义|

---

RAM Usage （AS6C62256）

*栈使用页`$01`*所有RAM假定为易失性
*没有ROM阴影

---

##构建管道

第一步：组装```bash
ca65 main.asm -o main.o
```
步骤2：链接```bash
ld65 -C rom.cfg main.o -o rom.bin
```
###步骤3:Pad ROM

确保`rom.bin`恰好是**32768字节**。

---

EEPROM编程

*目标设备：**AT28C256**
**程序员：**MiniPro / T48**
*写入后验证

---

模拟器期望

模拟器必须:

*在`$9000-$FFFF`加载ROM
*模拟VIAI/O的副作用
*渲染LCD输出
*荣誉复位矢量

---

##测试清单

*复位矢量执行
* VIA寄存器写入
* LCD显示正确的文本
*栈操作有效
* ROM映像映射正确

---

# #引用

* [MOS 6502编程手册]（http://archive.6502.org/datasheets/synertek_programming_manual.pdf）
* [MOS 6522 VIA Datasheet]（http://archive.6502.org/datasheets/mos_6522_preliminary_nov_1977.pdf）
* [AT28C256 Datasheet]（https://ww1.microchip.com/downloads/aemDocuments/documents/MPD/ProductDocuments/DataSheets/AT28C256-Industrial-Grade-256-Kbit-Paged-Parallel-EEPROM-Data-Sheet-DS20006386.pdf）
* [HD44780 LCD Datasheet]（https://www.futurlec.com/LED/LCD16X2BLa.shtml）
* [cc65工具链文档]（https://cc65.github.io/doc/cc65.html）

---

# #笔记

本规范有意**端到端**：从汇编源到EEPROM映像到运行硬件或模拟器。它定义了一个稳定的契约，使rom、模拟器和实际sbc的行为相同。