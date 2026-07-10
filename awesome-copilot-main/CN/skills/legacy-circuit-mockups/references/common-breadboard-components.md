#常见面包板电子元件

一个实用的参考**基于面包板的电子**，涵盖常见的组件，公式，表，引脚，和学习参考。适合初学者到中级硬件建设者。

---

# # 1。电阻

# # #描述

电阻限制电流，划分电压，设置偏置点。

###常用类型

|类型|备注|典型用途|| ------------- | ----------- | ------------------ |
碳膜|便宜，±5% |通用|
稳定，±1% |精密电路|
|绕线|大功率|功耗|
|电位器|可调|音量，调谐|

标准值（E12系列）`10, 12, 15, 18, 22, 27, 33, 39, 47, 56, 68, 82 x 10^n`颜色代码

|频段|含义|| ---- | ------------ |
| 1 |第一位数字|
| 2 |第二位|
| 3 |乘数|
| 4 |公差|

###额定功率

|额定|典型通径|| ------ | ------------------- |
|1/8W |小|
|1/4W |面包板标准|
|1/2W |大|

# # #公式

欧姆定律：**`V = I x R`** *功率：**`P = V × I = I² × R = V² / R`# # #参考

* [https://en.wikipedia.org/wiki/Resistor] (https://en.wikipedia.org/wiki/Resistor)

---

# # 2。电容器

# # #描述

电容器储存电能并在通过交流时阻挡直流电。

###常用类型

|类型|极化|典型用途|| ------------ | --------- | ------------------ |
|陶瓷|无|去耦，旁路|
|电解|是|大容量过滤|
|钽|是|紧凑滤波|
|薄膜|无|信号耦合|

###共同价值

|范围|示例|| ----- | ---------------- |
| pF | 10pF, 100pF |
| 100nF(0.1µF
|µf | 1µf, 10µf, 100µf |

电压额定值规则

>电容器额定电压≥**2x电路电压**

# # #公式

** *容抗：**`Xc = 1 / (2πfC)`** *储存能量：**`E = ½ C V²`# # #参考

* [https://en.wikipedia.org/wiki/Capacitor] (https://en.wikipedia.org/wiki/Capacitor)

---

# # 3。电感器

# # #描述

电感器在磁场中储存能量并抵抗电流的变化。

###常用类型

|类型|使用|| ------------ | ----------------- |
|空芯|射频电路|
|铁氧体铁芯|电源滤波|
|节流|噪声抑制|

# # #公式

** *感应抗：**`XL = 2πfL`# # #参考

* [https://en.wikipedia.org/wiki/Inductor] (https://en.wikipedia.org/wiki/Inductor)

---

# # 4。二极管

# # #描述

二极管只允许电流向一个方向流动。

###常用类型

|类型|典型部件|使用|| --------- | ------------ | --------------------- |
|整流器| 1N4007 |电源|
|信号| 1N4148 |逻辑，快速切换|
|稳压| 1N4733A |稳压|
|肖特基| 1N5819 |低滴|

关键参数

| |典型|| --------------- | -------------------------- |
|正向电压| 0.7V (Si), 0.3V（肖特基）|
|反向电压|设备专用|

# # #参考

* [https://en.wikipedia.org/wiki/Diode] (https://en.wikipedia.org/wiki/Diode)

---

# # 5。led（发光二极管）

# # #描述

led在正向偏置时发光。

典型正向电压

|颜色| Vf || ----- | -------- |
|红色| 1.8-2.2V |
|绿色| 2.0 ~ 3.0 v |
|蓝色| 3.0-3.3V |

限流电阻`R = (V_supply - V_LED) / I_LED`# # #参考

* [https://en.wikipedia.org/wiki/Light-emitting_diode] (https://en.wikipedia.org/wiki/Light-emitting_diode)

---

# # 6。晶体管

双极结晶体管

|类型|示例|使用|| ---- | ------- | ------------------------ |
| NPN | 2N3904 |开关、放大|
| PNP | 2N3906 |高侧交换|

* *关键公式:* *

*`Ic ≈ β × Ib`# # # MOSFET

|类型|示例|使用|| --------- | ------- | --------------------- |
| n通道| IRLZ44N |逻辑电平切换|
| p通道| IRF9540 |高侧控制|

# # #参考

* [https://en.wikipedia.org/wiki/Transistor] (https://en.wikipedia.org/wiki/Transistor)

---

# # 7。集成电路（DIP）

通用逻辑族

|家族|电压|音符|| ----------- | ------- | ------------------- |
| TTL (74LS) | 5V | Legacy |
| CMOS (74HC) | 2-6V |面包板友好|

解耦规则

>在每个IC的Vcc和GND上放置0.1µF陶瓷电容器

# # #参考

* [https://en.wikipedia.org/wiki/Integrated_circuit] (https://en.wikipedia.org/wiki/Integrated_circuit)

---

# # 8。开关和按钮

|类型|使用|| ------- | --------------- |
|触觉|瞬时输入|
|滑动|模式选择|
|打开|电源|

### Debounce （RC逼近）

*典型：`10kΩ + 100nF`---

# # 9。电路试验板

# # #描述

无焊原型板与内部总线连接。

内部线路

*行：水平连接（5孔）
*导轨：垂直连接（电源）

# # #的局限性

*不适用于高频或大电流电路

# # #参考

* [https://en.wikipedia.org/wiki/Breadboard] (https://en.wikipedia.org/wiki/Breadboard)

---

# # 10。常见的配件

|名称|用途|| ---------------- | ----------------- |
|跳线|连接|
| USB电源模块| 5V / 3.3V电源|
|万用表|测量|
|逻辑探头|数字调试|

---

# # 11。快速参考公式

|公式|描述|| --------------- | -------------------- |
欧姆定律
|`P = VI`|电源|
|`Xc = 1/(2πfC)`|容抗|
|`XL = 2πfL`|电感抗|
|`E = ½CV²`|电容能量|

---

# # 12。学习资源

* [https://learn.sparkfun.com] (https://learn.sparkfun.com)
* [https://www.allaboutcircuits.com] (https://www.allaboutcircuits.com)
* [https://www.electronics-tutorials.ws] (https://www.electronics-tutorials.ws)
* [https://en.wikipedia.org/wiki/Electronics] (https://en.wikipedia.org/wiki/Electronics)

---

**文档用途：**实用面包板电子参考资料
**范围：**爱好者，教育，原型