#游戏开发的Web api

这是一个全面的参考，涵盖了与创建基于浏览器的游戏最相关的web平台api。每个部分都描述了API是什么，为什么它对游戏很重要，它的关键接口和方法，并提供了简短的代码示例。

---## asm.js
它是什么asm.js是JavaScript的一个严格的、高度可优化的子集，旨在实现近乎原生的性能。它将JavaScript限制为一组狭窄的结构——整数、浮点数、算术、简单函数和堆访问——并且禁止对象、字符串、闭包和任何需要堆分配的东西。其结果是任何引擎都可以运行的完全有效的JavaScript，但支持引擎可以提前进行大量编译。

为什么这对游戏很重要- **接近原生速度**:emscripten编译的xqz1xq++游戏引擎以接近原生的性能跨浏览器运行。
- **可预测的性能**：受限制的功能集产生高度一致的帧率。
**C/C++可移植性**：现有的本地游戏引擎可以用Emscripten编译成asm.js并部署在web上。
- **不需要插件**：在每个现代浏览器中作为标准JavaScript运行。

关键概念

|概念|描述||---------|-------------|
允许构造|`while`，`if`，数字（严格的int/float），顶级命名函数，算术，函数调用，堆访问|
禁止构造|对象，字符串，闭包，动态类型强制转换，堆分配构造|
|编译工具链| Emscripten将C/C++编译为asm.js|
引擎识别|浏览器检测`"use asm"`指令并应用提前编译|

弃用通知asm.js已弃用。WebAssembly (Wasm)**是现代的继承者，提供了更好的性能、更广泛的工具和更广泛的行业支持。新项目应该以WebAssembly为目标。

代码示例```javascript
// asm.js module pattern (simplified)
function MyModule(stdlib, foreign, heap) {
  "use asm";

  var sqrt = stdlib.Math.sqrt;
  var HEAP32 = new stdlib.Int32Array(heap);

  function distance(x1, y1, x2, y2) {
    x1 = +x1; y1 = +y1; x2 = +x2; y2 = +y2;
    var dx = 0.0, dy = 0.0;
    dx = +(x2 - x1);
    dy = +(y2 - y1);
    return +sqrt(dx * dx + dy * dy);
  }

  return { distance: distance };
}
```
---

## Canvas API

它是什么

Canvas API提供了一种通过JavaScript和HTML`<canvas>`元素绘制2D图形的方法。它是基于浏览器的游戏的主要渲染界面之一，支持游戏图形、动画、图像处理和实时视频处理。

为什么这对游戏很重要

- **2D渲染面**：在浏览器游戏中绘制精灵、贴图、粒子和HUD元素的标准方式。
- **像素级控制：通过`ImageData`直接访问像素数据，用于自定义效果，碰撞地图和程序生成。
- **高性能**：硬件加速在现代浏览器，适用于60 fps游戏循环。
- **广泛的生态系统**：像Phaser，Konva.js， EaselJS和p5.js等库基于Canvas进行游戏开发。

关键接口

|接口|用途||-----------|---------|
|`HTMLCanvasElement`|`<canvas>`HTML元素|
|`CanvasRenderingContext2D`|主2D绘图界面|
|`ImageData`|直接操作的原始像素数据|
|`ImageBitmap`|位图图像数据，高效绘制|
|`Path2D`|可重用路径对象|
|`OffscreenCanvas`|屏幕外渲染，可用于Web Workers |
|`CanvasPattern`|重复图像模式|
|`CanvasGradient`|颜色渐变|
|`TextMetrics`|文本测量数据|

关键方法（CanvasRenderingContext2D）

-`fillRect()`,`strokeRect()`，`clearRect()`——矩形操作
-`drawImage()`—绘制图像，精灵或其他画布
—`beginPath()`,`arc()`,`lineTo()`,`fill()`，`stroke()`—路径绘制
-`getImageData()`，`putImageData()`——像素操作
—`save()`、`restore()`——状态管理
-`translate()`,`rotate()`,`scale()`，`transform()`—转换

代码示例```html
<canvas id="game" width="800" height="600"></canvas>
```

```javascript
const canvas = document.getElementById("game");
const ctx = canvas.getContext("2d");

// Clear the frame
ctx.clearRect(0, 0, canvas.width, canvas.height);

// Draw a filled rectangle (e.g., a platform)
ctx.fillStyle = "green";
ctx.fillRect(100, 400, 200, 20);

// Draw a sprite
const sprite = new Image();
sprite.src = "player.png";
sprite.onload = () => {
  ctx.drawImage(sprite, playerX, playerY, 32, 32);
};

// Game loop
function gameLoop(timestamp) {
  update(timestamp);
  render(ctx);
  requestAnimationFrame(gameLoop);
}
requestAnimationFrame(gameLoop);
```
---

CSS（层叠样式表）

它是什么

CSS是用来描述web文档表示的语言。在游戏开发的背景下，CSS处理UI覆盖、HUD元素、菜单、过渡、动画和视觉效果的样式，这些都位于游戏画布的顶部或旁边。

为什么这对游戏很重要**UI和HUD样式**：样式生命条，分数显示，库存面板，对话框和菜单，而无需触摸游戏画布。
- **CSS动画和过渡**：硬件加速动画UI元素（渐入，滑出，脉冲效果）与最小的JavaScript。
- **CSS转换**：翻译，旋转，缩放和倾斜DOM元素的视觉效果和UI定位。
**Flexbox和Grid**：响应式地布局复杂的游戏ui（设置面板、排行榜、大厅屏幕）。
- **自定义属性（CSS变量）**：通过在运行时改变变量值来动态地呈现主题游戏ui。
- **指针和光标控制**：自定义或隐藏游标，控制覆盖元素上的指针事件。
**媒体查询**：根据屏幕尺寸和设备类型调整游戏UI。

游戏的关键属性

|属性/特性|用例||--------------------|----------|
旋转，缩放，平移UI元素|
平滑属性改变(例如，生命条宽度
|`animation`/`@keyframes`|循环或触发UI动画|
覆盖和模态的渐变效果
让点击通过叠加层到画布|
|`cursor`|设置自定义游标或隐藏游标（`cursor: none`） |
|`z-index`|游戏画布上方的图层UI |
|将HUD元素锚定到viewport |
|`display: flex / grid`|菜单和面板响应式布局|
模糊，亮度，对比度效果的DOM元素|
混合覆盖效果与画布|
|提示浏览器优化动画属性|

代码示例```css
/* Game HUD overlay */
.hud {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  padding: 10px;
  pointer-events: none;       /* clicks pass through to canvas */
  z-index: 10;
  font-family: "Press Start 2P", monospace;
  color: white;
  text-shadow: 2px 2px 0 black;
}

/* Health bar with smooth transitions */
.health-bar {
  width: 200px;
  height: 20px;
  background: #333;
  border: 2px solid white;
}
.health-bar-fill {
  height: 100%;
  background: limegreen;
  transition: width 0.3s ease;
  will-change: width;
}

/* Pulsing damage indicator */
@keyframes damage-flash {
  0%, 100% { opacity: 0; }
  50% { opacity: 0.4; }
}
.damage-overlay {
  position: fixed;
  inset: 0;
  background: red;
  animation: damage-flash 0.3s ease;
  pointer-events: none;
}
```
---

##全屏API

它是什么

全屏API提供了在全屏模式下显示特定元素（及其后代）的方法，删除所有浏览器chrome和UI元素。它允许以编程方式进入和退出全屏，并报告当前的全屏状态。

为什么这对游戏很重要

- **身临其境的体验**：全屏消除所有浏览器干扰，提供类似主机的游戏体验。
- **最大屏幕实际空间**：整个显示可用于游戏视窗。
**游戏是主要用例**:MDN文档明确地将在线游戏列为目标应用程序。

关键接口和方法

|接口|描述||-----|-------------|
|`Element.requestFullscreen()`|进入全屏模式。返回一个`Promise`。|
|`Document.exitFullscreen()`|退出全屏模式。返回一个`Promise`。|
|当前全屏的元素，或`null`。|
|`Document.fullscreenEnabled`|布尔值，是否全屏。|
|`fullscreenchange`事件|全屏状态改变时触发。|
|entering/exiting全屏失败触发。|

代码示例```javascript
const gameContainer = document.getElementById("game-container");

// Enter fullscreen on button click
document.getElementById("fullscreenBtn").addEventListener("click", () => {
  if (document.fullscreenEnabled) {
    gameContainer.requestFullscreen().catch(err => {
      console.error("Fullscreen request failed:", err);
    });
  }
});

// Toggle fullscreen with a key press
document.addEventListener("keydown", (e) => {
  if (e.key === "F11") {
    e.preventDefault();
    if (!document.fullscreenElement) {
      gameContainer.requestFullscreen();
    } else {
      document.exitFullscreen();
    }
  }
});

// Respond to fullscreen changes (resize canvas, adjust UI)
document.addEventListener("fullscreenchange", () => {
  if (document.fullscreenElement) {
    resizeCanvasToFullscreen();
  } else {
    resizeCanvasToWindowed();
  }
});
```
# # #笔记

-全屏只能在响应用户手势（点击，按键）时请求。
—用户可以通过Escape键或F11退出。
—对于iframes中的嵌入式游戏，`allowfullscreen`属性是必需的。
-检查`Document.fullscreenEnabled`之前提供的功能在你的UI。

---

手柄API

它是什么

Gamepad API提供了一个标准化的接口，用于检测和读取来自手柄和游戏控制器的输入。它暴露了按键、模拟摇杆位置和控制器连接事件，在浏览器游戏中实现了控制台风格的控制。

为什么这对游戏很重要- **控制台质量输入**：支持Xbox， PlayStation和通用控制器在浏览器游戏。
- **多个控制器**：检测和处理多个手柄同时为本地多人游戏。
- **模拟输入**：读取模拟杆轴和压力敏感触发器进行细致的控制。
- **触觉反馈**：实验支持通过`GamepadHapticActuator`振动。

关键接口

| |接口描述||-----------|-------------|
|`Gamepad`|表示连接的控制器，包含按钮、轴和元数据|
|`GamepadButton`|表示单个按钮——`pressed`（布尔值）和`value`（压力0..）1) |
|`GamepadEvent`|事件对象`gamepadconnected`和`gamepaddisconnected`事件|
|`GamepadHapticActuator`|触觉反馈硬件接口（实验）|

关键方法和事件

|接口|描述||-----|-------------|
|`navigator.getGamepads()`|返回所有连接控制器|的`Gamepad`对象数组
|`gamepadconnected`event |控制器连接时`window`触发|
|`gamepaddisconnected`event |控制器断开连接触发`window`事件|

代码示例```javascript
// Detect controller connections
window.addEventListener("gamepadconnected", (e) => {
  console.log(`Gamepad connected: ${e.gamepad.id}`);
});

window.addEventListener("gamepaddisconnected", (e) => {
  console.log(`Gamepad disconnected: ${e.gamepad.id}`);
});

// Poll gamepad state each frame
function pollGamepads() {
  const gamepads = navigator.getGamepads();
  for (const gp of gamepads) {
    if (!gp) continue;

    // Read analog sticks (axes)
    const leftStickX = gp.axes[0]; // -1 (left) to 1 (right)
    const leftStickY = gp.axes[1]; // -1 (up) to 1 (down)

    // Read buttons
    if (gp.buttons[0].pressed) {
      // A button / Cross -- jump
      player.jump();
    }
    if (gp.buttons[7].value > 0.1) {
      // Right trigger -- accelerate (analog pressure)
      player.accelerate(gp.buttons[7].value);
    }
  }
  requestAnimationFrame(pollGamepads);
}
requestAnimationFrame(pollGamepads);
```
---

## IndexedDB API

它是什么

IndexedDB是内置在浏览器中的低级、异步、事务性客户端数据库。它使用键索引对象存储存储大量结构化数据（包括文件和blob），并支持高性能查询的索引。

为什么这对游戏很重要

- **保存游戏状态**：坚持玩家的进度，库存，角色统计，和关卡完成的会话。
- **本地缓存资产**：存储纹理，音频文件，关卡数据和其他资产，以减少网络请求并启用离线播放。
- **大的存储容量**：处理比`localStorage`更多的数据（上限在~ 5mb）。
- **非阻塞**：异步操作保持游戏循环在save/load操作期间平稳运行。
- **事务性**：原子read/write操作防止保存期间的数据损坏。

关键接口|接口|用途|游戏用例||-----------|---------|---------------|
|`indexedDB.open()`|打开或创建数据库|启动|时初始化游戏数据库
|`IDBDatabase`|数据库连接|管理连接生存期|`IDBTransaction`|范围和访问控制reads/writes|原子保存游戏操作|
|`IDBObjectStore`|主数据容器|存储玩家配置文件，级别数据，设置|
|`IDBIndex`|二级查找键|按类型、稀有度或其他属性查询项目|
|`IDBCursor`|对记录进行迭代|对游戏数据|进行批处理操作
|`IDBKeyRange`|为查询定义键范围|获取范围内的分数，最近保存槽|
|`IDBRequest`|异步操作句柄|管理所有数据库操作的回调|

代码示例```javascript
// Open (or create) the game database
const request = indexedDB.open("MyGameDB", 1);

request.onupgradeneeded = (event) => {
  const db = event.target.result;
  // Create an object store for save data
  const saveStore = db.createObjectStore("saves", { keyPath: "slotId" });
  saveStore.createIndex("timestamp", "timestamp");
};

request.onsuccess = (event) => {
  const db = event.target.result;

  // Save game state
  function saveGame(slot, gameState) {
    const tx = db.transaction("saves", "readwrite");
    const store = tx.objectStore("saves");
    store.put({
      slotId: slot,
      timestamp: Date.now(),
      playerHealth: gameState.health,
      playerPosition: gameState.position,
      inventory: gameState.inventory,
    });
  }

  // Load game state
  function loadGame(slot) {
    return new Promise((resolve, reject) => {
      const tx = db.transaction("saves", "readonly");
      const store = tx.objectStore("saves");
      const req = store.get(slot);
      req.onsuccess = () => resolve(req.result);
      req.onerror = () => reject(req.error);
    });
  }
};
```
---

# # JavaScript

它是什么

JavaScript是一种轻量级的、动态类型的、基于原型的编程语言，具有一等函数。它是web的脚本语言，也是所有基于浏览器的游戏逻辑的基础语言，支持命令式、功能式和面向对象的范例。

为什么这对游戏很重要- **运行环境**:JavaScript是执行浏览器游戏逻辑的语言。
事件驱动架构：本地事件处理支持输入、计时器和异步资源加载。
- **一级函数**：回调和闭包启用模式，如游戏循环，事件处理程序，行为树，和状态机。
- **动态对象：运行时对象创建和修改支持实体组件系统和数据驱动设计。
- **现代类语法**:ES6+类为游戏实体提供干净的继承层次结构。
- **Async/await**：清洁异步控制流的资产加载，服务器通信，和场景转换。
- **垃圾收集**：自动内存管理（尽管意识到GC暂停对平滑帧率很重要）。

游戏的关键语言功能

|功能|游戏应用||---------|------------------|
|实体层次结构（游戏对象，玩家，敌人）|
|闭包|回调和事件处理程序中的封装状态|
|`requestAnimationFrame`|核心游戏循环驱动|
|承诺/ async-await |资产加载，服务器调用，场景转换|
|解构和传播|清洁配置和状态传递|
|`Map`和`Set`|实体查找表，唯一ID跟踪，碰撞集|
|模板字面值|调试输出，动态文本呈现|
|模块（import/export） |将游戏代码组织成系统和组件|

代码示例```javascript
// ES6+ game entity pattern
class GameObject {
  constructor(x, y) {
    this.x = x;
    this.y = y;
    this.active = true;
  }
  update(dt) { /* override in subclasses */ }
  render(ctx) { /* override in subclasses */ }
}

class Player extends GameObject {
  constructor(x, y) {
    super(x, y);
    this.health = 100;
    this.speed = 200;
  }
  update(dt) {
    if (input.left) this.x -= this.speed * dt;
    if (input.right) this.x += this.speed * dt;
  }
  render(ctx) {
    ctx.fillStyle = "blue";
    ctx.fillRect(this.x, this.y, 32, 32);
  }
}

// Game loop using requestAnimationFrame
let lastTime = 0;
function gameLoop(timestamp) {
  const dt = (timestamp - lastTime) / 1000;
  lastTime = timestamp;

  for (const entity of entities) {
    entity.update(dt);
  }
  ctx.clearRect(0, 0, canvas.width, canvas.height);
  for (const entity of entities) {
    entity.render(ctx);
  }
  requestAnimationFrame(gameLoop);
}
requestAnimationFrame(gameLoop);
```
---

指针锁定API

它是什么

指针锁定API（以前称为鼠标锁定API）提供对原始鼠标移动增量的访问，而不是对绝对光标位置的访问。它将鼠标事件锁定在单个元素上，移除光标移动边界，并隐藏光标——这对于第一人称摄像机控制和类似机制至关重要。

为什么这对游戏很重要

- **第一人称摄像机控制**：通过物理移动鼠标移动摄像机，没有屏幕边缘限制。
- **没有光标分心**：光标是隐藏的，创造沉浸感。
- **持久锁**：一旦参与，移动数据流连续不管鼠标按钮的状态。
- **原始输入选项**:`unadjustedMovement`标志禁用操作系统级鼠标加速，以在竞技游戏中保持一致的目标。
- **释放鼠标按钮**：通过delta单独处理移动，点击可以映射到游戏动作（射击，互动）。关键接口和方法

|接口|描述||-----|-------------|
|`element.requestPointerLock(options?)`|锁定元素指针。返回一个`Promise`。|
|`document.exitPointerLock()`|释放指针锁。|
|`document.pointerLockElement`|当前持有锁的元素，或`null`。|
自上次`mousemove`事件以来的水平增量。|
自上次`mousemove`事件以来的垂直增量。|
|`pointerlockchange`事件|锁状态改变时触发。|
|`pointerlockerror`event |锁定或解锁失败时触发。|

代码示例```javascript
const canvas = document.getElementById("game");

// Request pointer lock on click (user gesture required)
canvas.addEventListener("click", async () => {
  if (!document.pointerLockElement) {
    await canvas.requestPointerLock({
      unadjustedMovement: true, // raw input, no OS acceleration
    });
  }
});

// Respond to lock state changes
document.addEventListener("pointerlockchange", () => {
  if (document.pointerLockElement === canvas) {
    document.addEventListener("mousemove", handleMouseMove);
  } else {
    document.removeEventListener("mousemove", handleMouseMove);
  }
});

// Use movement deltas for camera rotation
const sensitivity = 0.002;
function handleMouseMove(e) {
  camera.yaw   += e.movementX * sensitivity;
  camera.pitch  += e.movementY * sensitivity;
  camera.pitch   = Math.max(-Math.PI / 2, Math.min(Math.PI / 2, camera.pitch));
}
```
# # #笔记

-指针锁定只能在响应用户手势（点击，按键）时请求。
—用户可以随时使用Escape键退出。
-沙盒框架需要`allow-pointer-lock`属性。

---

SVG（可缩放矢量图形）

它是什么

SVG是一种基于xml的标记语言，用于描述二维矢量图形。与栅格格式（PNG、JPEG）不同，SVG图像可以缩放到任何分辨率而不会损失质量。SVG集成了CSS、DOM和JavaScript，使元素可脚本化并具有交互性。

为什么这对游戏很重要- **分辨率独立性**：单个SVG资产在任何屏幕尺寸或像素密度下看起来都很清晰——这是响应式游戏ui的理想选择。
- **轻量级**：基于文本和可压缩，减少下载大小的UI艺术和图标。
- **可通过DOM脚本**:SVG元素可以创建，修改，并与JavaScript实时动画。
- **CSS样式**:SVG形状接受填充，笔画，不透明度，转换，过滤器和动画的CSS规则。
- **内置动画**:SMIL动画元素（`<animate>`,`<animateTransform>`,`<animateMotion>`）用于声明性运动。
- **滤镜和效果**：高斯模糊，阴影，颜色矩阵，并通过SVG滤镜原语混合模式。

关键元素

元素|游戏用例||---------|---------------|
生命条，UI面板，平台
|`<circle>`，`<ellipse>`|目标，粒子，指标|
复杂的矢量艺术，自定义形状|
|`<polygon>`，`<polyline>`|网格叠加，线框元素|
|`<g>`|集合变换的群元素|
|`<defs>`,`<use>`，`<symbol>`|可重用的精灵定义|
|`<text>`，`<tspan>`|分数显示，标签，对话框|
模糊，阴影和颜色效果
|`<clipPath>`，`<mask>`|视口剪辑，显示效果|`<linearGradient>`，`<radialGradient>`|阴影和深度效果|
|`<animate>`，`<animateTransform>`|声明式UI动画|

代码示例```html
<!-- A simple SVG health bar -->
<svg width="220" height="30" xmlns="http://www.w3.org/2000/svg">
  <defs>
    <linearGradient id="healthGrad" x1="0" y1="0" x2="1" y2="0">
      <stop offset="0%" stop-color="limegreen" />
      <stop offset="100%" stop-color="green" />
    </linearGradient>
  </defs>
  <!-- Background -->
  <rect x="1" y="1" width="218" height="28" rx="5" fill="#333" stroke="#fff" stroke-width="1" />
  <!-- Health fill (width controlled via JS) -->
  <rect id="health-fill" x="3" y="3" width="160" height="24" rx="4" fill="url(#healthGrad)">
    <animate attributeName="width" from="214" to="60" dur="3s" fill="freeze" />
  </rect>
</svg>
```

```javascript
// Update health bar programmatically
function setHealth(percent) {
  const maxWidth = 214;
  document.getElementById("health-fill")
    .setAttribute("width", maxWidth * (percent / 100));
}
```
---

类型数组

它们是什么

类型化数组是原始二进制数据缓冲区（`ArrayBuffer`）上的类似数组的视图。与常规JavaScript数组不同，每个类型化数组都有固定的元素类型和大小，从而提供可预测的内存布局和高效的数据访问。没有单一的`TypedArray`构造函数；取而代之的是使用特定的构造函数，如`Float32Array`、`Uint8Array`和`Uint16Array`。

为什么它们对游戏很重要- **WebGL顶点和索引缓冲区**:WebGL方法直接接受位置、法线、纹理坐标、颜色和索引的类型数组。
- **Web音频缓冲区**：音频样本数据存储和操作为`Float32Array`。
- **二进制资产加载**：直接解析二进制文件格式（模型，纹理，关卡数据）。
- **内存效率**：固定大小的元素，没有装箱开销。
- **WebAssembly互操作**：通过`SharedArrayBuffer`和类型数组视图共享JavaScript和Wasm模块之间的内存。
- **网络序列化**：有效地包装游戏状态多人传输。

###键类型

|类型|字节|范围|游戏用例||------|-------|-------|---------------|
|`Float32Array`| 4 | ~3.4e38 |顶点位置，法线，紫外线，物理值|
|`Float64Array`| 8 | ~1.8e308 |高精度计算，仿真|
|`Uint8Array`| 1 | 0—255 |Texture/pixel数据，颜色通道|
|`Uint8ClampedArray`| 1 | 0 - 255（夹紧）|`ImageData`像素操作|
|`Uint16Array`| 2 | 0—65535 |索引缓冲区（小网格）|
|`Uint32Array`| 4 | 0—~ 43亿|索引缓冲区（大网格），id |
|`Int16Array`| 2 | -32768—32767 |音频样本，量化正常值|
|`Int32Array`| 4 | ~- 21亿—~ 21亿|整数游戏数据|

关键属性和方法```javascript
const verts = new Float32Array([0, 0, 0,  1, 0, 0,  0, 1, 0]);

verts.buffer;             // The underlying ArrayBuffer
verts.byteLength;         // Total size in bytes
verts.byteOffset;         // Byte offset into the buffer
verts.length;             // Number of elements
verts.BYTES_PER_ELEMENT;  // 4 for Float32Array

// Write data
verts.set([1, 2, 3], 0);            // Copy values at offset
verts.copyWithin(6, 0, 3);          // Duplicate first vertex to third slot

// Read sub-views (no copy)
const firstTriangle = verts.subarray(0, 9);

// Functional methods
const scaled = verts.map(v => v * 2);
const max = verts.reduce((a, v) => Math.max(a, v), -Infinity);
```
代码示例```javascript
// Build a quad for WebGL rendering
const positions = new Float32Array([
  -0.5, -0.5, 0,   // bottom-left
   0.5, -0.5, 0,   // bottom-right
   0.5,  0.5, 0,   // top-right
  -0.5,  0.5, 0,   // top-left
]);

const indices = new Uint16Array([
  0, 1, 2,  // first triangle
  0, 2, 3,  // second triangle
]);

// Upload to WebGL
const posBuf = gl.createBuffer();
gl.bindBuffer(gl.ARRAY_BUFFER, posBuf);
gl.bufferData(gl.ARRAY_BUFFER, positions, gl.STATIC_DRAW);

const idxBuf = gl.createBuffer();
gl.bindBuffer(gl.ELEMENT_ARRAY_BUFFER, idxBuf);
gl.bufferData(gl.ELEMENT_ARRAY_BUFFER, indices, gl.STATIC_DRAW);

// Generate a 440 Hz sine wave for Web Audio
const sampleRate = 44100;
const audioBuffer = new Float32Array(sampleRate); // 1 second
for (let i = 0; i < sampleRate; i++) {
  audioBuffer[i] = Math.sin(2 * Math.PI * 440 * i / sampleRate);
}
```
---

Web Audio API

它是什么

Web Audio API是一个用于在Web上控制音频的高级系统。它使用模块化路由图，其中音频源通过效果节点连接到目的地（扬声器）。它提供高精度定时、低延迟和基于源-侦听器模型的内置3D空间音频。

为什么这对游戏很重要- **低延迟播放**：声音效果以最小的延迟响应游戏事件。
- **3D空间音频**：定位声音在3D空间相对于player/listener的方向和基于距离的音频。
- **模块化效果管道**：链增益，混响，过滤器，压缩，和动态音景失真节点。
- **精确的调度**：调度声音，以精确采样准确的时间节奏游戏，顺序音乐，和定时事件。
- **实时分析**:`AnalyserNode`为音频反应视觉提供频率和波形数据。
- **程序音频**:`OscillatorNode`生成波形合成的声音效果和UI音调。

关键接口

|接口|用途||-----------|---------|
|`AudioContext`|主音频处理图；必须首先创建b|
|`AudioBufferSourceNode`|从`AudioBuffer`|播放预加载的音频（SFX，音乐）
|`OscillatorNode`|产生波形（正弦，方形，三角形，锯齿）|
|`GainNode`|控制音量/振幅|
|低通，高通，带通滤波器|
使用脉冲响应的卷积混响|
|`DelayNode`|延迟线效果（回声，合唱）|
|`DynamicsCompressorNode`|防止剪辑时，混合许多声音|
|`PannerNode`|在3D空间|中定位声源
|`AudioListener`|表示玩家的耳朵在3D空间|
|`StereoPannerNode`|简单left/right规划|
|`AnalyserNode`|实时频域分析|
|`AudioWorkletNode`|自定义音频处理关闭主线程|

通用路由模式

用例|路由图||----------|---------------|
|背景音乐|`BufferSource`->`GainNode`->`Destination`|
|位置SFX |`BufferSource`->`PannerNode`->`GainNode`->`Destination`|
|混响环境|`BufferSource`->`ConvolverNode`->`GainNode`->`Destination`|
|用户界面反馈音|`OscillatorNode`->`GainNode`->`Destination`|
|主混音|多源>单独`GainNode`->`DynamicsCompressorNode`->`Destination`|

代码示例```javascript
const audioCtx = new AudioContext();

// Load and play a sound effect
async function playSFX(url) {
  const response = await fetch(url);
  const arrayBuffer = await response.arrayBuffer();
  const audioBuffer = await audioCtx.decodeAudioData(arrayBuffer);

  const source = audioCtx.createBufferSource();
  source.buffer = audioBuffer;

  // Add gain control
  const gainNode = audioCtx.createGain();
  gainNode.gain.value = 0.8;

  // Connect: source -> gain -> speakers
  source.connect(gainNode);
  gainNode.connect(audioCtx.destination);

  source.start(0);
}

// 3D positional audio
function playPositionalSound(buffer, x, y, z) {
  const source = audioCtx.createBufferSource();
  source.buffer = buffer;

  const panner = audioCtx.createPanner();
  panner.panningModel = "HRTF";
  panner.distanceModel = "inverse";
  panner.refDistance = 1;
  panner.maxDistance = 100;
  panner.positionX.value = x;
  panner.positionY.value = y;
  panner.positionZ.value = z;

  source.connect(panner);
  panner.connect(audioCtx.destination);
  source.start(0);
}

// Update listener position each frame (matches camera/player)
function updateListener(playerPos, playerForward, playerUp) {
  const listener = audioCtx.listener;
  listener.positionX.value = playerPos.x;
  listener.positionY.value = playerPos.y;
  listener.positionZ.value = playerPos.z;
  listener.forwardX.value = playerForward.x;
  listener.forwardY.value = playerForward.y;
  listener.forwardZ.value = playerForward.z;
  listener.upX.value = playerUp.x;
  listener.upY.value = playerUp.y;
  listener.upZ.value = playerUp.z;
}
```
---

## WebGL API

它是什么

WebGL （Web Graphics Library）是一个JavaScript API，用于在浏览器中呈现硬件加速的2D和3D图形。它实现了一个符合OpenGL ES 2.0 （WebGL 1）和OpenGL ES 3.0 （WebGL 2）的配置文件，通过HTML`<canvas>`元素操作，并使用设备GPU进行渲染。

为什么这对游戏很重要

- **GPU加速渲染**：使用设备GPU以高帧率实时3D图形。
- **着色器编程**:GLSL中的顶点和片段着色器支持自定义视觉效果，照明，阴影和后期处理。
- **3D和2D**：适用于全3D游戏和高性能2D渲染。
- **无插件**：在所有现代浏览器中本机运行。
- **丰富的生态系统**:three.js、Babylon.js、PlayCanvas和Pixi.js等库简化了WebGL游戏开发。

关键接口

|接口|用途||-----------|---------|
|`WebGLRenderingContext`| WebGL 1渲染上下文（OpenGL ES 2.0） |
|`WebGL2RenderingContext`| WebGL 2渲染上下文（OpenGL ES 3.0） |
|链接顶点+碎片着色程序|
|`WebGLShader`|单个顶点或片段着色|
|`WebGLBuffer`| GPU内存缓冲区（顶点，索引）|
|`WebGLTexture`|表面纹理数据|
|`WebGLFramebuffer`|屏幕外渲染目标（阴影贴图，后期处理）|
|`WebGLRenderbuffer`|非纹理渲染缓冲区（深度，模板）|
|`WebGLVertexArrayObject`|缓存顶点属性配置（WebGL 2） |
|`WebGLUniformLocation`|对着色器统一变量|的引用
|`WebGLSampler`|纹理采样参数(WebGL 2
|`WebGLTransformFeedback`| gpu到gpu数据流（WebGL 2） |

WebGL 2对游戏的重要功能- **3D纹理**：体积渲染，查找表。
- **实例渲染**:`drawArraysInstanced()`/`drawElementsInstanced()`绘制数以千计的相同的对象有效。
- **多个渲染目标**:`drawBuffers()`延迟渲染管道。
- **统一缓冲对象**：在绘制调用中有效地共享着色器数据。
- **变换反馈**：捕获顶点着色器输出gpu驱动的粒子系统和模拟。
- **顶点数组对象**：缓存顶点状态以减少每次绘制的设置开销。

###上下文管理事件

|事件|事件描述||-------|-------------|
|`webglcontextlost`| GPU上下文丢失（设备断开，资源限制）。游戏必须巧妙地处理这个问题。|
|`webglcontextrestored`| GPU上下文恢复。游戏应该重新加载GPU资源。|
|`webglcontextcreationerror`|初始化上下文失败。|

代码示例```javascript
const canvas = document.getElementById("game");
const gl = canvas.getContext("webgl2");

// Vertex shader
const vsSource = `#version 300 es
  in vec4 aPosition;
  uniform mat4 uModelViewProjection;
  void main() {
    gl_Position = uModelViewProjection * aPosition;
  }
`;

// Fragment shader
const fsSource = `#version 300 es
  precision mediump float;
  out vec4 fragColor;
  void main() {
    fragColor = vec4(1.0, 0.5, 0.2, 1.0);
  }
`;

function compileShader(gl, source, type) {
  const shader = gl.createShader(type);
  gl.shaderSource(shader, source);
  gl.compileShader(shader);
  if (!gl.getShaderParameter(shader, gl.COMPILE_STATUS)) {
    console.error(gl.getShaderInfoLog(shader));
    gl.deleteShader(shader);
    return null;
  }
  return shader;
}

const vs = compileShader(gl, vsSource, gl.VERTEX_SHADER);
const fs = compileShader(gl, fsSource, gl.FRAGMENT_SHADER);

const program = gl.createProgram();
gl.attachShader(program, vs);
gl.attachShader(program, fs);
gl.linkProgram(program);
gl.useProgram(program);

// Upload vertex data
const positions = new Float32Array([0, 0.5, 0, -0.5, -0.5, 0, 0.5, -0.5, 0]);
const buffer = gl.createBuffer();
gl.bindBuffer(gl.ARRAY_BUFFER, buffer);
gl.bufferData(gl.ARRAY_BUFFER, positions, gl.STATIC_DRAW);

const aPos = gl.getAttribLocation(program, "aPosition");
gl.enableVertexAttribArray(aPos);
gl.vertexAttribPointer(aPos, 3, gl.FLOAT, false, 0, 0);

// Render
gl.clearColor(0, 0, 0, 1);
gl.clear(gl.COLOR_BUFFER_BIT);
gl.drawArrays(gl.TRIANGLES, 0, 3);
```
推荐的库

|库|描述||---------|-------------|
|three.js|全功能3D引擎|
完整的游戏引擎与物理，音频，和网络|
PlayCanvas |基于云的游戏引擎|
|轻量级2D渲染器|
矩阵和向量数学库

---

## WebRTC API

它是什么

WebRTC （Web Real-Time Communication）支持浏览器之间的点对点通信，用于音频、视频和任意数据交换——不需要插件或中间中继服务器（尽管信令服务器和STUN/TURN用于连接设置和NAT穿越）。

为什么这对游戏很重要- **点对点多人游戏：在玩家之间建立直接连接，减少延迟，消除小型游戏的专用游戏服务器。
- **低延迟数据通道**:`RTCDataChannel`以最小的开销发送二进制游戏状态更新，支持可靠和不可靠的交付模式。
- **语音聊天**：内置的audio/video流媒体支持游戏内语音通信。
- **降低服务器成本**：直接对等连接卸载带宽和处理从集中式服务器。

关键接口

|接口|用途||-----------|---------|
|`RTCPeerConnection`|管理两个对等体之间的连接，包括媒体流和数据通道|
|`RTCDataChannel`|双向通道任意数据（游戏状态，命令，聊天）|
|`RTCSessionDescription`|通过SDP进行会话协商（offer/answer模型）|
|`RTCIceCandidate`|NAT/firewall遍历的连接候选|
|`RTCRtpSender`/`RTCRtpReceiver`|管理audio/video编码和传输|
|`RTCStatsReport`|用于优化的连接统计（时延、丢包、带宽）|

关键事件

|事件|事件描述||-------|-------------|
|`datachannel`|对端打开数据通道|
|`connectionstatechange`|对端连接状态改变|
|`icecandidate`|新的ICE候选可用|
|`track`|传入媒体轨道（audio/video） |

连接生命周期

1. 在每个对等点上创建`RTCPeerConnection`。
2. 通过信令服务器（通常是WebSocket）交换SDPoffers/answers。
3. 交换候选ICE进行NAT穿越。
4. 对等体直接连接。
5. 打开`RTCDataChannel`为游戏数据and/or添加媒体轨道的声音。
6. 使用`RTCStatsReport`监视性能。
7. 会话结束时关闭通道和连接。

代码示例```javascript
// Peer A: Create connection and data channel
const peerA = new RTCPeerConnection({
  iceServers: [{ urls: "stun:stun.l.google.com:19302" }]
});

const gameChannel = peerA.createDataChannel("game", {
  ordered: false,       // Allow out-of-order delivery (lower latency)
  maxRetransmits: 0,    // Unreliable mode (like UDP)
});

gameChannel.onopen = () => {
  // Send game state updates
  gameChannel.send(JSON.stringify({ type: "move", x: 10, y: 20 }));
};

gameChannel.onmessage = (event) => {
  const data = JSON.parse(event.data);
  applyRemoteGameState(data);
};

// Peer B: Receive the data channel
const peerB = new RTCPeerConnection({
  iceServers: [{ urls: "stun:stun.l.google.com:19302" }]
});

peerB.ondatachannel = (event) => {
  const channel = event.channel;
  channel.onmessage = (e) => {
    const data = JSON.parse(e.data);
    applyRemoteGameState(data);
  };
};

// Signaling (offer/answer exchange via your signaling server)
async function connect() {
  const offer = await peerA.createOffer();
  await peerA.setLocalDescription(offer);
  // Send offer to Peer B via signaling server...

  // Peer B receives offer, sets remote description, creates answer
  await peerB.setRemoteDescription(offer);
  const answer = await peerB.createAnswer();
  await peerB.setLocalDescription(answer);
  // Send answer back to Peer A via signaling server...

  await peerA.setRemoteDescription(answer);
}
```
---

## WebSockets API

它是什么

WebSocket API支持在浏览器和服务器之间通过单个TCP连接进行持久的全双工通信。与HTTP请求-响应不同，WebSocket连接保持打开状态，允许服务器随时向客户端推送数据。

为什么这对游戏很重要

- **实时多人游戏：流玩家的位置，游戏事件，和世界状态之间的客户端和服务器最小的延迟。
- **服务器推送更新**：服务器可以广播游戏状态的变化立即到所有连接的玩家没有轮询。
- **低开销**：在每条消息上没有重复的HTTP头；只是在持久连接上帧数据。
- **二进制数据支持**：发送`ArrayBuffer`和`Blob`数据有效的游戏状态序列化。
**Web Worker兼容**：在后台线程中运行WebSocket通信，以保持游戏循环畅通。关键接口：WebSocket

|成员|描述信息||--------|-------------|
|`new WebSocket(url, protocols?)`|打开到服务器|的连接
|`send(data)`|传输数据（string, ArrayBuffer, Blob） |
|`close(code?, reason?)`|安全关闭连接|
|`readyState`|当前状态：连接(0)，打开(1)，关闭(2)，关闭(3)|
|`bufferedAmount`|排队但尚未发送（流量控制）|
|`binaryType`|二进制数据|设置为`"arraybuffer"`或`"blob"`# # #事件

|事件|事件描述||-------|-------------|
|`open`|连接建立并就绪|
|`message`|从服务器接收的数据（通过`event.data`访问）|
|`close`|连接关闭（通过`CloseEvent`访问code/reason） |
|`error`| |发生错误

代码示例```javascript
// Connect to the game server
const socket = new WebSocket("wss://game.example.com/ws");
socket.binaryType = "arraybuffer";

socket.addEventListener("open", () => {
  // Authenticate and join a game room
  socket.send(JSON.stringify({
    type: "join",
    room: "room-42",
    playerId: "player-1"
  }));
});

socket.addEventListener("message", (event) => {
  if (typeof event.data === "string") {
    const msg = JSON.parse(event.data);
    switch (msg.type) {
      case "state":
        updateWorldState(msg.state);
        break;
      case "playerJoined":
        addRemotePlayer(msg.player);
        break;
      case "playerLeft":
        removeRemotePlayer(msg.playerId);
        break;
    }
  } else {
    // Binary data -- e.g., compressed game state
    const view = new DataView(event.data);
    processRawGameState(view);
  }
});

socket.addEventListener("close", (event) => {
  console.log(`Disconnected: ${event.code} ${event.reason}`);
  showReconnectPrompt();
});

// Send player input to the server each tick
function sendInput(input) {
  if (socket.readyState === WebSocket.OPEN) {
    socket.send(JSON.stringify({
      type: "input",
      keys: input.keys,
      mouseX: input.mouseX,
      mouseY: input.mouseY,
      timestamp: performance.now(),
    }));
  }
}
```
# # #笔记

-关闭WebSocket连接时，玩家导航离开，以避免阻塞浏览器的向后缓存。
-对于需要不可靠（类似udp）传输的游戏，考虑使用WebRTC数据通道或更新的WebTransport API。
-流行的服务器库：Socket。IO, ws (Node.js), Gorilla WebSocket (Go), SignalR （. net）。

---

WebVR API（已弃用）

它是什么

WebVR API提供了从浏览器访问虚拟现实设备（头戴式显示器，如Oculus Rift和HTC Vive）的接口。它为沉浸式VR体验提供了显示属性、头部跟踪姿势数据和立体渲染功能。

为什么这对游戏很重要- **沉浸式VR游戏**：通过实时头部跟踪驱动渲染立体3D场景。
- **房间级体验**:`VRStageParameters`描述了物理游戏区域的尺寸。
- **控制器集成**:VR控制器可通过Gamepad API访问，通过`gamepad.displayId`将每个控制器连接到`VRDisplay`。

弃用通知

WebVR API **已弃用且不标准**。它从未被批准为web标准，并已被**WebXR设备API**所取代，该API支持VR和AR，具有更广泛的浏览器支持，并且正在标准化。所有新的VR游戏开发都应该瞄准WebXR。

关键接口

|接口|用途||-----------|---------|
|`VRDisplay`|表示VR头显。核心方法：`requestPresent()`、`requestAnimationFrame()`、`getFrameData()`、`submitFrame()`。|
|`VRFrameData`|当前帧的姿态、视图矩阵和投影矩阵。|
|`VRPose`|在给定时间戳的位置、方向、速度和加速度。|
每只眼睛的视野和渲染偏移。|
|房间大小的游戏区域尺寸和转换。|
|`VRDisplayCapabilities`|设备能力标志（具有位置跟踪，具有外部显示等）。|
|`Navigator.getVRDisplays()`|返回一个承诺，解析到一个连接的`VRDisplay`对象数组。|

关键事件

|事件|事件描述||-------|-------------|
|`vrdisplayconnect`|连接VR头显|
|`vrdisplaydisconnect`| VR耳机断开|
|`vrdisplaypresentchange`|耳机进入或退出演示模式|
|`vrdisplayactivate`|耳机准备呈现|

代码示例```javascript
// Check for WebVR support
if (navigator.getVRDisplays) {
  navigator.getVRDisplays().then(displays => {
    if (displays.length === 0) return;
    const vrDisplay = displays[0];

    // Start presenting to the headset
    vrDisplay.requestPresent([{ source: canvas }]).then(() => {
      const frameData = new VRFrameData();

      function renderLoop() {
        vrDisplay.requestAnimationFrame(renderLoop);
        vrDisplay.getFrameData(frameData);

        // Render left eye
        gl.viewport(0, 0, canvas.width / 2, canvas.height);
        renderScene(frameData.leftProjectionMatrix, frameData.leftViewMatrix);

        // Render right eye
        gl.viewport(canvas.width / 2, 0, canvas.width / 2, canvas.height);
        renderScene(frameData.rightProjectionMatrix, frameData.rightViewMatrix);

        vrDisplay.submitFrame();
      }
      renderLoop();
    });
  });
}
```
迁移到WebXR

对于新项目，请使用WebXR设备API。支持WebXR的框架包括：

- **A-Frame**——声明式实体组件VR框架
**Babylon.js**——全功能的3D/game引擎与WebXR的支持
- **three.js**——轻量级3D库与WebXR集成
- **WebXR Polyfill** -旧浏览器的向后兼容层

---

Web Workers API

它是什么

Web Workers API支持在与主线程分离的后台线程中运行JavaScript。工作线程在自己的全局作用域中操作（`DedicatedWorkerGlobalScope`或`SharedWorkerGlobalScope`），不能直接访问DOM，只能通过消息传递（`postMessage`/`onmessage`）与主线程通信。

为什么这对游戏很重要- **卸载繁重的计算**：移动物理模拟，寻径，AI，程序生成和碰撞检测到后台线程，使主线程保持在60 fps。
- **并行资产处理**：解码图像，解压缩数据，或解析级别文件不阻塞渲染。
- **OffscreenCanvas**：渲染到画布从一个工人，启用并行渲染管道。
- **非阻塞网络**：执行`fetch()`或XHR调用在一个工作者，以保持游戏循环顺利。

###工作者类型

|类型|描述|游戏用例||------|-------------|---------------|
|专用工作者（`Worker`） |单一所有者后台线程|物理，AI，寻径为一个游戏实例|
|共享Worker (`SharedWorker`) |多个windows/tabs|多选项卡或多帧游戏场景|
| Service Worker |网络代理，支持离线|资产缓存，离线播放|

关键接口

|接口|描述||-----|-------------|
|`new Worker(scriptURL)`|从脚本文件|创建一个专用的worker
|`worker.postMessage(data)`|向工作线程|发送数据
|`worker.onmessage`|从worker接收数据（通过`event.data`） |
|`worker.terminate()`|立即停止工人|
|发送数据回主线程|`self.onmessage`|从主线程|接收数据

# # #的局限性

-工人不能访问DOM。
-无`window`对象；有限的全球范围。
—默认复制数据（结构化克隆）；使用`Transferable`对象（ArrayBuffer, OffscreenCanvas）进行零拷贝传输。
—工作脚本必须是同源的。

代码示例

**主线程（game.js）：**```javascript
// Create a physics worker
const physicsWorker = new Worker("physics-worker.js");

// Send world state to the worker each frame
function updatePhysics(entities) {
  // Transfer the buffer for zero-copy performance
  const buffer = serializeEntities(entities);
  physicsWorker.postMessage({ type: "step", buffer }, [buffer]);
}

// Receive results from the worker
physicsWorker.onmessage = (event) => {
  const { type, buffer } = event.data;
  if (type === "result") {
    applyPhysicsResults(buffer);
  }
};
```
**工作线程（physics-worker.js）：**```javascript
self.onmessage = (event) => {
  const { type, buffer } = event.data;
  if (type === "step") {
    const positions = new Float32Array(buffer);

    // Run physics simulation
    for (let i = 0; i < positions.length; i += 3) {
      positions[i + 1] -= 9.8 * (1 / 60); // gravity on Y axis
    }

    // Send results back, transferring the buffer
    self.postMessage({ type: "result", buffer: positions.buffer }, [positions.buffer]);
  }
};
```
---

# # XMLHttpRequest

它是什么`XMLHttpRequest`（XHR）是一个内置的浏览器API，用于向服务器发出HTTP请求而无需重新加载页面。尽管它的名字，它可以检索任何数据类型——JSON、二进制（ArrayBuffer、Blob）、纯文本、XML和HTML。它在很大程度上已经被用于新代码的Fetch API所取代，但仍然被广泛使用和完全支持。

为什么这对游戏很重要-资产加载：在不阻塞游戏循环的情况下异步检索游戏资产（图像，音频，JSON关卡数据，二进制模型文件）。
- **二进制数据支持**：将`responseType`设置为`"arraybuffer"`或`"blob"`，以将二进制资产直接加载到WebGL或Web Audio的类型数组中。
—**进度跟踪**:`progress`事件报告下载进度，启用加载条。
- **服务器通信**：提交分数，认证玩家，获取排行榜，并与后端服务同步游戏状态。
**兼容Web Worker **: XHR可以在Web Workers内部用于后台资源加载。

关键方法

|方法|描述||--------|-------------|
|`open(method, url, async?)`|初始化请求（GET， POST等）|
|`send(body?)`|发送请求`body`可以是string， FormData, ArrayBuffer, Blob |
|`setRequestHeader(name, value)`|设置HTTP报头（在`open`之后，在`send`之前调用）|
|`abort()`|取消正在执行的请求|
|`getResponseHeader(name)`|检索特定的响应头值|

关键属性

|属性|描述||----------|-------------|
|`response`|响应体为`responseType`|指定的类型
|`responseType`|预期响应格式：`""`、`"text"`、`"json"`、`"arraybuffer"`、`"blob"`、`"document"`|
|`status`| HTTP状态码(200、404等
|`readyState`|请求生命周期状态（0 = UNSENT到4 = DONE） |
|`timeout`|请求自动中止|前的毫秒数
|`withCredentials`|跨域请求是否包含cookie |

# # #事件

|事件|事件描述||-------|-------------|
|`load`|请求成功完成|
|`error`|请求失败|
|`progress`|下载过程中定时更新进度|
|`abort`|请求终止
|`readystatechange`|`readyState`改变|

代码示例```javascript
// Load a JSON level file
function loadLevel(url) {
  return new Promise((resolve, reject) => {
    const xhr = new XMLHttpRequest();
    xhr.open("GET", url);
    xhr.responseType = "json";

    xhr.onload = () => {
      if (xhr.status === 200) {
        resolve(xhr.response);
      } else {
        reject(new Error(`Failed to load level: ${xhr.status}`));
      }
    };
    xhr.onerror = () => reject(new Error("Network error"));
    xhr.send();
  });
}

// Load a binary asset with progress tracking
function loadBinaryAsset(url, onProgress) {
  return new Promise((resolve, reject) => {
    const xhr = new XMLHttpRequest();
    xhr.open("GET", url);
    xhr.responseType = "arraybuffer";

    xhr.onprogress = (event) => {
      if (event.lengthComputable && onProgress) {
        onProgress(event.loaded / event.total);
      }
    };

    xhr.onload = () => {
      if (xhr.status === 200) {
        resolve(xhr.response); // ArrayBuffer
      } else {
        reject(new Error(`Failed to load asset: ${xhr.status}`));
      }
    };
    xhr.onerror = () => reject(new Error("Network error"));
    xhr.send();
  });
}

// Usage
loadLevel("levels/level1.json").then(data => initLevel(data));
loadBinaryAsset("models/tank.bin", pct => updateLoadingBar(pct))
  .then(buf => parseModel(new Float32Array(buf)));
```
关于Fetch API的注意事项

对于新项目，**Fetch API** (`fetch()`)通常优于XHR。它提供了一个更简洁的基于承诺的接口，支持通过`ReadableStream`进行流传输，并且与async/await.集成得很好。但是，当您需要上传的进度事件或需要与遗留代码库更广泛的兼容性时，XHR仍然是相关的。