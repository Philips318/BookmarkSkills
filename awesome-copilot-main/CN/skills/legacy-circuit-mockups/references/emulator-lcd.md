# DFRobot FIT0127液晶字符显示仿真规范

# #概述

本文档详细说明了如何对DFRobot FIT0127液晶字符显示模块进行仿真。FIT0127是一款**16x2字符LCD**兼容**HD44780控制器**，常用于6502和单片机面包板系统。

目标是SBC仿真器、固件测试和UI可视化的功能正确性，而不是电信号仿真。

---

##模块总结

|特性|值|| ---------------- | ----------------------- |
|显示类型|字符LCD |
| 16列x 2行|
|控制器| hd44780兼容|
|接口| 8位或4位并行|
|字符矩阵| 5x8点|
|电源电压| 5V |

---

引脚定义

|引脚|名称|功能|| ---- | ----- | ---------------- |
| 1 |地|地|
| | VCC | + 5v |
| 3 | VO |对比度控制|
| 4 | RS | Register选择|
| 5 |R/W|读写|
| 6 | E |开启|
| 7 ~ 14 | d0 ~ d7 |数据总线|
| 15 | A |背光+ |
| 16 | K |背光- |

---

##逻辑寄存器

| RS |注册|
| -- | -------------------- |
| 0 |指令寄存器|
|数据寄存器|

---

指令集（子集）

|命令|代码|描述|| --------------- | ----------- | ------------------------- |
| Clear Display |`0x01`|清空DDRAM，光标指向|
|返回Home |`0x02`|光标到Home位置|
|输入模式设置|`0x04-0x07`|光标方向|
|显示控制|`0x08-0x0F`|显示、光标、闪烁|
|Cursor/Shift|`0x10-0x1F`| Shiftcursor/display|
|功能设置|`0x20-0x3F`|数据长度，行数|
|设置CGRAM地址|`0x40-0x7F`|自定义字符|
|设置DDRAM地址|`0x80-0xFF`|光标位置|

---

内部内存模型

DDRAM（显示数据RAM）

*大小：80字节
* Line 1 base:`0x00`* Line 2 base:`0x40`模拟器映射:```text
Row 0: DDRAM[0x00-0x0F]
Row 1: DDRAM[0x40-0x4F]
```
CGRAM（字符生成器RAM）

*存储多达8个自定义字符
*每个字符8字节

---

##数据写周期

写发生在：```
RS = 1
R/W = 0
E: HIGH  LOW
```
仿真器行为

*在`E`下降沿，锁存数据
*根据地址模式将数据写入DDRAM或CGRAM
*根据入口模式自动增加或减少地址

---

##指令写周期

命令写发生在：```
RS = 0
R/W = 0
E: HIGH  LOW
```
---

##读取周期（可选）

阅读在爱好系统中并不常见。```
RS = 0/1
R/W = 1
E: HIGH
```
模拟器可以简化：

*完全忽略读取
*或返回繁忙标志+地址计数器

---

忙标志仿真

真正的硬件

*忙标志= D7
*命令时间为37-1520µs

模拟器选项

|模式|行为|| ---------- | ----------------- |
|简化|随时准备|
|定时|忙N次|

建议默认值：**Always ready**

---

##通电状态

重置:

*显示关闭
*光标关闭
* DDRAM已清除或未定义
*地址计数器= 0

模拟器:

*清除DDRAM
*设置游标为（0,0）
*启用显示

---

光标和显示模型

状态变量:```text
cursor_row
cursor_col
display_on
cursor_on
blink_on
```
根据写操作的方式，光标自动移动。

---

4位vs 8位接口

8位模式

在D0-D7上传输的全字节

4位模式

*高咬先送
*每字节两个使能脉冲

仿真器的简化:

*接受全字节写
*忽略咀嚼时间

---

渲染模型（仿真器UI）

推荐方法:

*保持16x2字符缓冲
*呈现ASCII子集
*替换不支持的字形
*可选渲染自定义CGRAM字符

---

仿真器API模型```c
typedef struct {
    uint8_t ddram[80];
    uint8_t cgram[64];
    uint8_t addr;
    bool display_on;
    bool cursor_on;
    bool blink_on;
    uint8_t entry_mode;
} FIT0127_LCD;
```
---

6502系统中的常见布线```
VIA Port  LCD D4-D7 (4-bit mode)
RS  VIA bit
E   VIA bit
R/W  GND
```
---

##测试清单

*清晰显示命令
*光标定位通过DDRAM地址
*顺序字符写入
*行换行行为
*自定义字符显示

---

# #引用

* [HD44780U Datasheet (Hitachi)]（https://academy.cba.mit.edu/classes/output_devices/44780.pdf）
* [Ben Eater LCD接口说明]（https://hackaday.io/project/174128-db6502/log/181838-adventures-with-hd44780-lcd-controller）
* [Ben Eater的6502电脑]（https://github.com/tedkotz/be6502）
*[构建一台6502计算机]（https://eater.net/6502）

---

# #笔记

本规范有意优先考虑固件可见行为**电气精度，使其理想的：

* SBC模拟器
* ROM和监视器的开发
* LCD输出自动测试
*教育CPU项目