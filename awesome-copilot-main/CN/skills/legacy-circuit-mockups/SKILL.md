---
name: legacy-circuit-mockups
description: 'Generate breadboard circuit mockups and visual diagrams using HTML5 Canvas drawing techniques. Use when asked to create circuit layouts, visualize electronic component placements, draw breadboard diagrams, mockup 6502 builds, generate retro computer schematics, or design vintage electronics projects. Supports 555 timers, W65C02S microprocessors, 28C256 EEPROMs, W65C22 VIA chips, 7400-series logic gates, LEDs, resistors, capacitors, switches, buttons, crystals, and wires.'
---
#遗留电路模型

为复古计算和电子项目创建面包板电路模型和可视化图表的技能。该技能利用HTML5 Canvas绘图机制来呈现具有6502微处理器，555定时器ic， eeprom和7400系列逻辑门等老式组件的交互式电路布局。

何时使用此技能

-用户要求“创建面包板布局”或“模拟电路”
-用户希望可视化组件放置在面包板上
-用户需要构建6502计算机的视觉参考
-用户要求“绘制电路”或“电子图”
-用户想要创建教育电子视觉
-用户提到Ben Eater教程或复古计算项目
-用户要求模拟555定时器电路或LED项目
-用户需要可视化组件之间的连线

# #先决条件-从捆绑的参考文件中理解组件的引脚
熟悉面包板布局规范（行，列，电源导轨）

支持的组件

微处理器和内存

|组件|引脚|描述||-----------|------|-------------|
| W65C02S | 40引脚DIP | 8位微处理器带16位地址总线|
| 28引脚DIP | 32KB并行EEPROM |
| W65C22 | 40引脚DIP |通用接口适配器（VIA） |
| 28引脚DIP | 32KB静态RAM |

逻辑和定时器ic

|组件|引脚|描述||-----------|------|-------------|
| NE555 | 8引脚DIP |定时器IC用于定时和振荡|
| 14引脚DIP |四路2输入NAND栅极|
| 14引脚DIP |四路2输入NOR门|
| 7404 | 14引脚DIP |十六进制逆变器（非栅极）|
bbb7408 | 14引脚DIP |四路2输入与门|
| 7432 | 14引脚DIP |四路2输入OR门|

被动和主动组件

|组件|描述||-----------|-------------|
| LED |发光二极管（各种颜色）|
|电阻|限流（可配置值）|
|电容|滤波和定时（ceramic/electrolytic） |
|晶体|时钟振荡器|
| Switch |拨动开关（锁存）|
|按钮|瞬间按钮|
|电位器|可变电阻|
|光敏电阻|光敏电阻|

网格系统```javascript
// Standard breadboard grid: 20px spacing
const gridSize = 20;
const cellX = Math.floor(x / gridSize) * gridSize;
const cellY = Math.floor(y / gridSize) * gridSize;
```
组件呈现模式```javascript
// All components follow this structure:
{
  type: 'component-type',
  x: gridX,
  y: gridY,
  width: componentWidth,
  height: componentHeight,
  rotation: 0,  // 0, 90, 180, 270
  properties: { /* component-specific data */ }
}
```
电线连接```javascript
// Wire connection format:
{
  start: { x: startX, y: startY },
  end: { x: endX, y: endY },
  color: '#ff0000'  // Wire color coding
}
```
##分步工作流程

创建基本的LED电路模型

1. 定义面包板尺寸和网格
2. 放置电源轨连接（+5V和GND）
3. 添加anode/cathode方向的LED组件
4. 放置限流电阻
5. 绘制组件之间的电线连接
6. 添加标签和注释

创建一个555定时器电路

1. 将NE555 IC放置在面包板上（引脚1-4左，5-8右）
2. 将引脚1 （GND）连接到地轨
3. 连接引脚8 （Vcc）到电源轨
4. 添加定时电阻和电容
5. 线触发和阈值连接
6. 将输出连接到LED或其他负载

创建6502微处理器布局1. 将W65C02S放在面包板中央
2. 增加28C256 EEPROM用于程序存储
3. 为I/O放置W65C22 VIA
4. 增加7400系列逻辑地址解码
5. 线地址总线（A0-A15）
6. 有线数据总线（D0-D7）
7. 连接控制信号（R/W, PHI2， RESB）
8. 增加复位按钮和时钟晶体

组件引脚快速参考

### 555定时器（8针DIP）

|引脚|名称|功能||:---:|:-----|:---------|
| 1 | GND |接地（0V） |
| . 2 | TRIG | Trigger (<1/3Vcc starting timing) | . 2
| 3 | OUT |输出（source/sink200mA） |
| 4 | RESET | Active-low复位|
| 5 | CTRL |控制电压（旁路10nF） |
| 6 | THR |阈值（>2/3Vcc复位）|
| 7 | DIS |放电（开路集电极）|
| 8 | Vcc |电源（+4.5V至+16V） |

W65C02S(40引脚DIP) -关键引脚

|引脚|名称|功能||:---:|:-----|:---------|
| 8 | VDD |电源|
| 21 | VSS |接地|
| 37 | PHI2 |系统时钟输入|
| 40 | RESB | Active-low reset |
| 34 | RWB |Read/Write信号|
| 9-25 | A0-A15 |地址总线|
| 26-33 | D0-D7 |数据总线|

EEPROM(28引脚DIP) -关键引脚

|引脚|名称|功能||:---:|:-----|:---------|
| 14 |地|地|
| 28 | VCC |电源|
| 20 | CE | Chip enable (active-low) |
| 22 | OE | Output enable (active-low) |
| 27 | WE | Write enable (active-low) |
| 1 ~ 10,21 ~ 26 | a0 ~ a14 |地址输入|
| 11-19 |I/O0-I/O7|数据总线|

##公式参考

###电阻计算

**欧姆定律：** V = I × R
- **LED电流：** R = (Vcc - Vled) / Iled
- **功率：** P = V × I = I²× R

### 555定时器公式

* *不稳模式:* *

-频率：f = 1.44 / （（R1 + 2×R2） × C）
—高时刻：t₁= 0.693 × （R1 + R2） × C
—低时间：t₂= 0.693 × R2 × C
—占空比：D = (R1 + R2) / （R1 + 2×R2） × 100%

* *单稳态模式:* *

—脉冲宽度：T = 1.1 × R × C

###电容器计算

-容抗：Xc = 1 / （2πfC）
-储存能量：E = 1 / 2 × C × V²

颜色编码约定

###电线颜色

|颜色|用途||-------|---------|
|红色| +5V /电源|
|黑色|地面|
|黄色|时钟/定时|
|蓝色|地址总线|
|绿色|数据总线|
|橙色|控制信号|
|白色|通用|

LED颜色

|颜色|正向电压||-------|-----------------|
|红色| 1.8V - 2.2V |
|绿色| 2.0V - 2.2V |
|黄色| 2.0V - 2.2V |
|蓝色| 3.0V - 3.5V |
|白色| 3.0V - 3.5V |

##构建示例

###构建1 -单个LED

**组成：**红色LED， 220Ω电阻，跳线，电源

* *步骤:* *

1. 从电源GND插入黑色跳线至A5排
2. 从电源+5V插入红色跳线至J5排
3. 将带阴极（短腿）的LED排在与GND对齐的位置
4. 在电源和LED阳极之间放置220Ω电阻

Build 2 - 555 stable Blinker

**组件：** NE555， LED，电阻（10kΩ， 100kΩ），电容（10µF）

* *步骤:* *

1. 放置555集成电路横跨中心通道
2. 引脚1连接到GND，引脚8连接到+5V
3. 连接引脚4到引脚8（禁用复位）
4. 引脚7和+5V之间的导线10kΩ
5. 引脚6和7之间的导线100kΩ
6. 引脚6和GND之间的导线为10µF
7. 连接引脚3（输出）到LED电路

# #故障排除

|问题|解决方案||-------|----------|
检查极性（阳极到+，阴极到-）|
|电路不供电|检查电源轨连接|
| IC不工作|检查VCC和GND引脚连接|
bbb555不振荡|验证threshold/trigger电容接线|
|微处理器卡住|复位脉冲|后检查RESB为HIGH

# #引用

详细的组件规格可在捆绑的参考文件中获得：- [555.md](references/555.md) -完成555定时器IC规格
- [6502.md](references/6502.md) - MOS 6502微处理器细节
- [6522.md](references/6522.md) - W65C22 VIA接口适配器
- [28256-eeprom.md](references/28256-eeprom.md) - AT28C256 EEPROM规范
- [6C62256.md](references/6C62256.md) - 62256 SRAM详细信息
—[7400-series.md]（references/7400-series.md）—TTL逻辑门引脚
- [assembly-compiler.md](references/assembly-compiler.md) -汇编编译器规范
- [assembly-language.md](references/assembly-language.md) -汇编语言规范
- [basic-electronic-components.md](references/basic-electronic-components.md) -电阻器，电容器，开关
- [breadboard.md](references/breadboard.md) -面包板规格
- [common-breadboard-components.md](references/common-breadboard-components.md) -综合组件参考
- [connecting-electronic-components.md](references/connecting-electronic-components.md) -分步构建指南
- [emulator-28256-eeprom.md](references/emulator-28256-eeprom.md) -模拟28256-eeprom规范
- [emulator-6502.md](references/emulator-6502.md) -模拟6502规范
- [emulator-6522.md](references/emulator-6522.md) -模拟6522规范
- [emulator-6C62256.md](references/emulator-6C62256.md) -模拟6C62256规范
- [emulator-lcd.md](references/emulator-lcd.md) -仿真LCD规格
- [lcd.md](references/lcd.md) - LCD显示接口
- [minipro.md](references/minipro.md) - EEPROM编程器使用
- [t48eeprom-programmer.md](references/t48eeprom-programmer.md) - T48程序员参考