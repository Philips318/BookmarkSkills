#游戏控制机制

本文涵盖了网页游戏的主要控制机制，包括手机触控、桌面键盘和鼠标、手柄控制器和非常规输入方法。

##移动触摸控制

手机触摸控制对于面向手机设备的网页游戏来说至关重要。移动优先的方法能够确保游戏能够在最广泛使用的HTML5游戏平台上运行。

关键事件和api

浏览器中可用的核心触摸事件有：

|事件|事件描述||-------|-------------|
|`touchstart`|当用户将手指放在屏幕上|
|`touchmove`|当用户移动手指触摸屏幕时触发|
|`touchend`|当用户从屏幕上抬起手指时触发|
|`touchcancel`|当触摸被取消或中断时触发（例如，手指移出屏幕）|

**注册触摸事件监听器：**```javascript
const canvas = document.querySelector("canvas");
canvas.addEventListener("touchstart", handleStart);
canvas.addEventListener("touchmove", handleMove);
canvas.addEventListener("touchend", handleEnd);
canvas.addEventListener("touchcancel", handleCancel);
```
**触摸事件属性：**

-`e.touches[0]`—访问第一个触摸点（多点触摸为零索引）。
-`e.touches[0].pageX`/`e.touches[0].pageY`—相对于页面的触摸坐标。
-总是减去画布偏移量来获得相对于画布元素的位置。

代码示例

**纯JavaScript触摸处理程序：**```javascript
document.addEventListener("touchstart", touchHandler);
document.addEventListener("touchmove", touchHandler);

function touchHandler(e) {
  if (e.touches) {
    playerX = e.touches[0].pageX - canvas.offsetLeft - playerWidth / 2;
    playerY = e.touches[0].pageY - canvas.offsetTop - playerHeight / 2;
    e.preventDefault();
  }
}
```
**相位器框架指针系统：**

Phaser通过代表单个手指的“指针”管理触摸输入：```javascript
// Access pointers
this.game.input.activePointer;       // Most recently active pointer
this.game.input.pointer1;            // First pointer
this.game.input.pointer2;            // Second pointer

// Add more pointers (up to 10 total)
this.game.input.addPointer();

// Global input events
this.game.input.onDown.add(itemTouched, this);
this.game.input.onUp.add(itemReleased, this);
this.game.input.onTap.add(itemTapped, this);
this.game.input.onHold.add(itemHeld, this);
```
**船舶移动的可拖动精灵：**```javascript
const player = this.game.add.sprite(30, 30, "ship");
player.inputEnabled = true;
player.input.enableDrag();
player.events.onDragStart.add(onDragStart, this);
player.events.onDragStop.add(onDragStop, this);

function onDragStart(sprite, pointer) {
  console.log(`Dragging at: ${pointer.x}, ${pointer.y}`);
}
```
**拍摄不可见触摸区域（屏幕右半部分）：**```javascript
this.buttonShoot = this.add.button(
  this.world.width * 0.5, 0,
  "button-alpha",    // transparent image
  null,
  this
);
this.buttonShoot.onInputDown.add(this.goShootPressed, this);
this.buttonShoot.onInputUp.add(this.goShootReleased, this);
```
**虚拟手柄插件：**```javascript
this.gamepad = this.game.plugins.add(Phaser.Plugin.VirtualGamepad);
this.joystick = this.gamepad.addJoystick(100, 420, 1.2, "gamepad");
this.button = this.gamepad.addButton(400, 420, 1.0, "gamepad");
```
最佳实践

-总是调用`preventDefault()`触摸事件，以避免不必要的滚动和默认的浏览器行为。
-使用不可见的按钮区域，而不是可见的按钮，以避免覆盖游戏玩法。
-利用自然的触摸手势，如拖动，这比屏幕上的按钮更直观。
-在计算位置时减去画布偏移量并考虑对象尺寸。
-使可触摸区域足够大，以实现舒适的交互。
-计划支持多点触控。相位器支持多达10个同时指针。
-使用像Phaser这样的框架来自动兼容桌面和移动设备。
-考虑虚拟gamepad/joystick插件，用于高级触摸控制UI。

带有鼠标和键盘的桌面

桌面键盘和鼠标控制为网页游戏提供了精确的输入，是桌面浏览器的默认控制方案。

关键事件和api

* *键盘事件:* *```javascript
document.addEventListener("keydown", keyDownHandler);
document.addEventListener("keyup", keyUpHandler);
```
—`event.code`返回可读的关键字标识符，如`"ArrowLeft"`、`"ArrowRight"`、`"ArrowUp"`、`"ArrowDown"`。
-使用`requestAnimationFrame()`连续帧更新。

**相位键盘API:**```javascript
this.cursors = this.input.keyboard.createCursorKeys();  // Arrow key objects
this.keyLeft = this.input.keyboard.addKey(Phaser.KeyCode.A);  // Custom key binding
// Check key state with .isDown property
// Listen for press events with .onDown.add()
```
**相位鼠标API:**```javascript
this.game.input.mousePointer;                    // Mouse position and state
this.game.input.mousePointer.isDown;             // Is any mouse button pressed
this.game.input.mousePointer.x;                  // Mouse X coordinate
this.game.input.mousePointer.y;                  // Mouse Y coordinate
this.game.input.mousePointer.leftButton.isDown;  // Left mouse button
this.game.input.mousePointer.rightButton.isDown; // Right mouse button
this.game.input.activePointer;                   // Platform-independent (mouse + touch)
```
代码示例

**纯JavaScript键盘状态跟踪：**```javascript
let rightPressed = false;
let leftPressed = false;
let upPressed = false;
let downPressed = false;

function keyDownHandler(event) {
  if (event.code === "ArrowRight") rightPressed = true;
  else if (event.code === "ArrowLeft") leftPressed = true;
  if (event.code === "ArrowDown") downPressed = true;
  else if (event.code === "ArrowUp") upPressed = true;
}

function keyUpHandler(event) {
  if (event.code === "ArrowRight") rightPressed = false;
  else if (event.code === "ArrowLeft") leftPressed = false;
  if (event.code === "ArrowDown") downPressed = false;
  else if (event.code === "ArrowUp") upPressed = false;
}
```
**游戏循环与输入处理：**```javascript
function draw() {
  ctx.clearRect(0, 0, canvas.width, canvas.height);

  if (rightPressed) playerX += 5;
  else if (leftPressed) playerX -= 5;
  if (downPressed) playerY += 5;
  else if (upPressed) playerY -= 5;

  ctx.drawImage(img, playerX, playerY);
  requestAnimationFrame(draw);
}
```
**双控制支持（方向键+方向键）在相位：**```javascript
this.cursors = this.input.keyboard.createCursorKeys();
this.keyLeft = this.input.keyboard.addKey(Phaser.KeyCode.A);
this.keyRight = this.input.keyboard.addKey(Phaser.KeyCode.D);
this.keyUp = this.input.keyboard.addKey(Phaser.KeyCode.W);
this.keyDown = this.input.keyboard.addKey(Phaser.KeyCode.S);

// In update:
if (this.cursors.left.isDown || this.keyLeft.isDown) {
  // move left
} else if (this.cursors.right.isDown || this.keyRight.isDown) {
  // move right
}
if (this.cursors.up.isDown || this.keyUp.isDown) {
  // move up
} else if (this.cursors.down.isDown || this.keyDown.isDown) {
  // move down
}
```
**多个点火按钮：**```javascript
this.keyFire1 = this.input.keyboard.addKey(Phaser.KeyCode.X);
this.keyFire2 = this.input.keyboard.addKey(Phaser.KeyCode.SPACEBAR);

if (this.keyFire1.isDown || this.keyFire2.isDown) {
  // fire the weapon
}
```
* *特定于设备的指令:* *```javascript
if (this.game.device.desktop) {
  moveText = "Arrow keys or WASD to move";
  shootText = "X or Space to shoot";
} else {
  moveText = "Tap and hold to move";
  shootText = "Tap to shoot";
}
```
最佳实践

-支持多种输入法：提供方向键和方向键用于移动，以及多个射击键（例如，X和空格）。
-使用`activePointer`而不是`mousePointer`来无缝地支持鼠标和触摸输入。
-检测设备类型并显示适当的控制指令给玩家。
-使用`requestAnimationFrame()`平滑动画，检查游戏循环中的键状态，而不是对单个按键做出反应。
-允许键盘快捷键跳过非游戏屏幕（例如，Enter开始，任何键跳过介绍）。
-使用Phaser或类似的跨浏览器兼容性框架，因为它们会自动处理边缘情况和浏览器差异。

带有手柄的桌面

Gamepad API使网页游戏能够检测并响应手柄和控制器输入，为浏览器带来类似主机的体验。

关键事件和api

核心事件:* * * *```javascript
window.addEventListener("gamepadconnected", gamepadHandler);
window.addEventListener("gamepaddisconnected", gamepadHandler);
```
**手柄对象属性：**

-`controller.id`——设备标识字符串。
-`controller.buttons[]`——按钮对象的数组，每个都有一个`.pressed`布尔属性。
-`controller.axes[]`——模拟摇杆值的数组，范围从-1到1。

**标准button/axes映射（Xbox 360布局）：**

|输入|索引|类型||-------|-------|------|
| A按钮| 0 |按钮|
| B按钮| 1 |按钮|
| X按钮| 2 |按钮|
| Y键| 3 |键|
|按d键向上| 12 |按|键
|按下键| 13 |按下键|
| D-Pad左| 14 |按钮|
| D-Pad右| 15 |按钮|
|左摇杆X |轴[0]|轴|
|左操纵杆Y |轴[1]|轴|
|右摇杆X |轴[2]|轴|
|右操纵杆Y |轴[3]|轴|

代码示例

**纯JavaScript连接处理程序：**```javascript
let controller = {};
let buttonsPressed = [];

function gamepadHandler(e) {
  controller = e.gamepad;
  console.log(`Gamepad: ${controller.id}`);
}

window.addEventListener("gamepadconnected", gamepadHandler);
```
**轮询按钮状态每帧：**```javascript
function gamepadUpdateHandler() {
  buttonsPressed = [];
  if (controller.buttons) {
    for (const [i, button] of controller.buttons.entries()) {
      if (button.pressed) {
        buttonsPressed.push(i);
      }
    }
  }
}

function gamepadButtonPressedHandler(button) {
  return buttonsPressed.includes(button);
}
```
**游戏循环整合```javascript
function draw() {
  ctx.clearRect(0, 0, canvas.width, canvas.height);
  gamepadUpdateHandler();

  if (gamepadButtonPressedHandler(12)) playerY -= 5;  // D-Pad Up
  else if (gamepadButtonPressedHandler(13)) playerY += 5;  // D-Pad Down
  if (gamepadButtonPressedHandler(14)) playerX -= 5;  // D-Pad Left
  else if (gamepadButtonPressedHandler(15)) playerX += 5;  // D-Pad Right
  if (gamepadButtonPressedHandler(0)) alert("BOOM!");  // A Button

  ctx.drawImage(img, playerX, playerY);
  requestAnimationFrame(draw);
}
```
**可重复使用的GamepadAPI库与保持和按压检测：**```javascript
const GamepadAPI = {
  active: false,
  controller: {},

  connect(event) {
    GamepadAPI.controller = event.gamepad;
    GamepadAPI.active = true;
  },

  disconnect(event) {
    delete GamepadAPI.controller;
    GamepadAPI.active = false;
  },

  update() {
    GamepadAPI.buttons.cache = [...GamepadAPI.buttons.status];
    GamepadAPI.buttons.status = [];

    const c = GamepadAPI.controller || {};
    const pressed = [];

    if (c.buttons) {
      for (let b = 0; b < c.buttons.length; b++) {
        if (c.buttons[b].pressed) {
          pressed.push(GamepadAPI.buttons.layout[b]);
        }
      }
    }

    const axes = [];
    if (c.axes) {
      for (const ax of c.axes) {
        axes.push(ax.toFixed(2));
      }
    }

    GamepadAPI.axes.status = axes;
    GamepadAPI.buttons.status = pressed;
    return pressed;
  },

  buttons: {
    layout: ["A", "B", "X", "Y", "LB", "RB", "LT", "RT",
             "Back", "Start", "LS", "RS",
             "DPad-Up", "DPad-Down", "DPad-Left", "DPad-Right"],
    cache: [],
    status: [],
    pressed(button, hold) {
      let newPress = false;
      if (GamepadAPI.buttons.status.includes(button)) {
        newPress = true;
      }
      if (!hold && GamepadAPI.buttons.cache.includes(button)) {
        newPress = false;
      }
      return newPress;
    }
  },

  axes: {
    status: []
  }
};

window.addEventListener("gamepadconnected", GamepadAPI.connect);
window.addEventListener("gamepaddisconnected", GamepadAPI.disconnect);
```
**模拟操纵杆运动与死区阈值：**```javascript
if (GamepadAPI.axes.status) {
  if (GamepadAPI.axes.status[0] > 0.5) playerX += 5;       // Right
  else if (GamepadAPI.axes.status[0] < -0.5) playerX -= 5; // Left
  if (GamepadAPI.axes.status[1] > 0.5) playerY += 5;       // Down
  else if (GamepadAPI.axes.status[1] < -0.5) playerY -= 5; // Up
}
```
**上下文感知控制显示：**```javascript
if (this.game.device.desktop) {
  if (GamepadAPI.active) {
    moveText = "DPad or left Stick to move";
    shootText = "A to shoot, Y for controls";
  } else {
    moveText = "Arrow keys or WASD to move";
    shootText = "X or Space to shoot";
  }
} else {
  moveText = "Tap and hold to move";
  shootText = "Tap to shoot";
}
```
最佳实践

-在处理手柄输入之前总是检查`GamepadAPI.active`。
-通过缓存前一帧按钮状态来区分“保持”（连续）和“按”（单次新按）。
-为模拟杆值应用死区阈值（例如，0.5）以避免无意的漂移输入。
—创建按钮映射系统，因为不同的设备可能有不同的按钮布局。
-通过调用`requestAnimationFrame`内部的更新函数，每帧轮询手柄状态。
-当手柄连接时，在屏幕上显示一个指示，以及适当的控制说明。
-浏览器支持约63%的全球；始终提供回退keyboard/mouse控件。

其他控制机制

非传统的控制机制可以提供独特的游戏体验，并利用传统输入设备之外的新兴硬件。

电视遥控器**说明：**智能电视遥控器发出标准键盘事件，允许网页游戏在电视屏幕上运行而无需修改。

**关键事件和api:**

-远程方向键映射到标准箭头键代码。
—自定义远程按钮具有制造商特定的键代码。

* *代码示例:* *```javascript
// Standard arrow key controls work automatically with TV remotes
this.cursors = this.input.keyboard.createCursorKeys();
if (this.cursors.right.isDown) {
  // move player right
}

// Discover manufacturer-specific remote key codes
window.addEventListener("keydown", (event) => {
  console.log(event.keyCode);
});

// Handle custom remote buttons (codes vary by manufacturer)
window.addEventListener("keydown", (event) => {
  switch (event.keyCode) {
    case 8:   // Pause (Panasonic example)
      break;
    case 588: // Custom action
      break;
  }
});
```
* *最佳实践:* *

-在开发期间将密钥代码记录到控制台，以发现远程按钮映射。
-重用现有的键盘控制实现，因为远程发出键盘事件。
-关键代码映射请参考制造商文档或备忘单。

Leap Motion（手势识别）

**描述：**检测手的位置，旋转和握力的手势为基础的控制没有物理接触使用跳跃运动传感器。

**关键事件和api:**

-`Leap.loop()`——基于帧的手部跟踪回调。
-`hand.roll()`——以弧度为单位的水平旋转。
-`hand.pitch()`—以弧度为单位的垂直旋转。
-`hand.grabStrength`-握力从0（张开的手）到1（握紧的拳头）浮动。

* *代码示例:* *```html
<script src="https://js.leapmotion.com/leap-0.6.4.min.js"></script>
```

```javascript
const toDegrees = 1 / (Math.PI / 180);
let horizontalDegree = 0;
let verticalDegree = 0;
const degreeThreshold = 30;
let grabStrength = 0;

Leap.loop({
  hand(hand) {
    horizontalDegree = Math.round(hand.roll() * toDegrees);
    verticalDegree = Math.round(hand.pitch() * toDegrees);
    grabStrength = hand.grabStrength;
  },
});

function draw() {
  ctx.clearRect(0, 0, canvas.width, canvas.height);

  if (horizontalDegree > degreeThreshold) playerX -= 5;
  else if (horizontalDegree < -degreeThreshold) playerX += 5;

  if (verticalDegree > degreeThreshold) playerY += 5;
  else if (verticalDegree < -degreeThreshold) playerY -= 5;

  if (grabStrength === 1) fireWeapon();

  ctx.drawImage(img, playerX, playerY);
  requestAnimationFrame(draw);
}
```
* *最佳实践:* *

-使用度阈值（例如30度）过滤掉轻微的手部运动和噪音。
-在开发过程中输出诊断数据以校准灵敏度。
-限制简单的操作，如转向和射击，而不是复杂的多输入方案。
-需要安装Leap Motion驱动程序。

多普勒效应（基于麦克风的手势检测）

**描述：**通过分析设备麦克风拾取的声波中的频率变化来检测手部运动方向和幅度。发出的声音从用户的手上反射回来，频率差指示移动方向。

**关键事件和api:**

—使用多普勒效应检测库。
—“`bandwidth.left`”和“`bandwidth.right`”提供频率分析值。

* *代码示例:* *```javascript
doppler.init((bandwidth) => {
  const diff = bandwidth.left - bandwidth.right;
  // Positive diff = movement in one direction
  // Negative diff = movement in the other direction
});
```
* *最佳实践:* *

-最适合简单的单轴控制，如滚动或up/down移动。
-不如Leap Motion或手柄输入精确。
—通过left/right频差比较提供方向性信息。

Makey Makey（物理对象控制器）

**描述：**将导电物体（香蕉，粘土，绘制电路，水等）连接到模拟键盘和鼠标输入的板上，为游戏提供创造性的物理界面。

**关键事件和api（通过Cylon.js定制硬件）：**

-`makey-button`驱动程序自定义设置与Arduino或树莓派。
-`"push"`事件监听器按钮激活。
Makey Makey板本身通过USB工作，无需自定义代码即可发出标准键盘事件。

**代码示例（使用Cylon.js自定义设置）```javascript
const Cylon = require("cylon");

Cylon.robot({
  connections: {
    arduino: { adaptor: "firmata", port: "/dev/ttyACM0" },
  },
  devices: {
    makey: { driver: "makey-button", pin: 2 },
  },
  work(my) {
    my.makey.on("push", () => {
      console.log("Button pushed!");
      // Trigger game action
    });
  },
}).start();
```
* *最佳实践:* *

- Makey Makey板通过USB连接并发出标准键盘事件，因此现有的键盘控件开箱即用。
-在自定义设置中使用10 MOhm电阻用于GPIO连接。
-支持创造性的物理游戏体验，特别适合展览和装置。

非常规控制的一般建议-实施多种控制机制，以达到尽可能广泛的受众。
-建立在键盘和手柄的基础上，因为大多数非常规控制器模拟或补充标准输入。
-使用阈值来过滤噪声和来自不精确硬件的意外输入。
-在开发过程中提供控制台输出和屏幕值的可视化诊断。
-匹配控制复杂性的游戏需要。并非所有机制都适合所有游戏。
-在执行游戏逻辑之前彻底测试硬件设置。