带有AT28C256 EEPROM的汇编语言

编写**6502/65C02汇编语言程序**的实用规范，旨在存储在单板计算机（sbc）和复古系统中的**AT28C256 （32 KB）并行EEPROM**中并从**执行。

---

# # 1。范围及假设

本文档假设：

* A **6502系列CPU** （6502, 65co2，或兼容）
*程序代码存储在**AT28C256 (32K x 8) EEPROM**
*内存映射I/O（例如6522 VIA）
复位和中断向量位于EEPROM
*外部RAM映射到其他地方（例如，62256 SRAM）

---

# # 2。AT28C256 EEPROM概述

| |值|| -------------- | ------------------- |
|容量| 32kb（32768字节）|
|地址线| A0-A14 |
|数据线| D0-D7 |
|接入时间| ~ 150ns |
|电源电压| 5v |
| DIP-28 / PLCC |

典型的内存映射使用

|地址范围|用途|| ------------- | ----------------------- |
|`$8000-$FFFF`| EEPROM（代码+向量）|
|`$FFFA-$FFFF`|中断向量|

---

# # 3。6502内存映射示例```
$0000-$00FF  Zero Page (RAM)
$0100-$01FF  Stack
$0200-$7FFF  RAM / I/O
$8000-$FFFF  AT28C256 EEPROM
```
---

# # 4。复位和中断矢量

6502从**内存顶部**读取向量：

|向量|地址|描述|| ------- | ------------- | ---------------------- |
| NMI |`$FFFA-$FFFB`|不可屏蔽中断|
| RESET |`$FFFC-$FFFD`|复位入口点|
|IRQ/BRK|`$FFFE-$FFFF`|可屏蔽中断|

向量定义示例```asm
        .org $FFFA
        .word nmi_handler
        .word reset
        .word irq_handler
```
---

# # 5。汇编程序结构

典型布局```asm
        .org $8000

reset:
        sei             ; Disable IRQs
        cld             ; Clear decimal mode
        ldx #$FF
        txs             ; Initialize stack

main:
        jmp main
```
---

# # 6。基本6502说明

# # #寄存器

|注册|目的|| -------- | ---------------- |
| A |蓄电池|
| X， Y |索引寄存器|
| SP |堆栈指针|
| PC |程序计数器|
| P |处理器状态|

###常用说明

|指令|功能|| ----------- | ---------------------- |
|LDA/STA|Load/store累加器|
|LDX/LDY|加载索引寄存器|
|JMP/JSR|跳转/子程序|
| RTS |从子程序|返回
|BEQ/BNE|条件必选分支|
|SEI/CLI|Disable/enableIRQ |

---

# # 7。寻址模式（通用）

|模式|示例|备注|| --------- | ------------- | ------------ |
|即时|`LDA #$01`|固定|
|零页|`LDA $00`|快速|
|绝对|`LDA $8000`|完整地址|
|索引|`LDA $2000,X`|表|
|间接|`JMP ($FFFC)`|矢量|

---

# # 8。编写EEPROM执行代码

关键注意事项

代码在运行时是只读的
*不推荐自修改代码
*在EEPROM中放置跳转表和常量
*使用RAM存储变量和堆栈

零页变量示例```asm
counter = $00

        lda #$00
        sta counter
```
---

# # 9。时间和性能

* EEPROM访问时间必须满足CPU时钟要求
* AT28C256舒适支持~1 MHz
*更快的时钟可能需要等待状态或ROM阴影

---

# # 10。示例：简单的LED切换（内存映射I/O）```asm
PORTB = $6000
DDRB  = $6002

        .org $8000
reset:
        sei
        ldx #$FF
        txs

        lda #$FF
        sta DDRB

loop:
        lda #$FF
        sta PORTB
        jsr delay
        lda #$00
        sta PORTB
        jsr delay
        jmp loop
```
---

# # 11。装配与编程工作流程

1. 写源（`.asm`）
2. 汇编成二进制
3. 填充或重新定位到`$8000`4. 通过T48 / minipro编程AT28C256
5. 插入EEPROM并复位CPU

---

# # 12。汇编指令（通用）

|指令|目的|| ---------- | --------------------------- |
|`.org`|设置程序原点|
|`.byte`|定义字节|
|`.word`|定义单词（小端）|
|`.include`|包含|文件
|`.equ`|常量定义|

---

# # 13。常见的错误

|问题|结果|| -------------------------- | ------------------ |
|缺少矢量| CPU挂起复位|
|错误`.org`|代码未执行|
|在ROM中使用RAM地址|崩溃|
|未定义行为|

---

# # 14。参考链接

* [https://www.masswerk.at/6502/6502_instruction_set.html] (https://www.masswerk.at/6502/6502_instruction_set.html)
* [https://www.nesdev.org/wiki/6502] (https://www.nesdev.org/wiki/6502)
* [https://www.westerndesigncenter.com/wdc/documentation] (https://www.westerndesigncenter.com/wdc/documentation)
* [https://en.wikipedia.org/wiki/MOS_Technology_6502] (https://en.wikipedia.org/wiki/MOS_Technology_6502)

---

**文档范围：** 6502组件存储在AT28C256 EEPROM
**受众：**复古计算，SBC设计师，嵌入式爱好者
**状态：**稳定引用