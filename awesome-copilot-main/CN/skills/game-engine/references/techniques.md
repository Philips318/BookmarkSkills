#游戏开发技巧

这是一份全面的参考资料，涵盖了构建网页游戏的基本技术，编译自MDN Web Docs。

---

##异步脚本

**来源：** [MDN -异步脚本asm.js]（https://developer.mozilla.org/en-US/docs/Games/Techniques/Async_scripts）

它是什么

异步编译允许JavaScript引擎在游戏加载期间从主线程编译asm.js代码并缓存生成的机器码。这可以防止在后续加载时重新编译，并为浏览器提供最大的灵活性来优化编译过程。

###如何工作

当脚本异步加载时，浏览器可以在后台线程上编译它，而主线程继续处理渲染和用户交互。编译后的代码被缓存，因此以后的访问完全跳过重新编译。

###何时使用-编译asm.js代码的中型或大型游戏。
-任何游戏的启动性能（这几乎是所有的游戏）。
-当您希望浏览器跨会话缓存编译的机器代码时。

代码示例

**HTML属性方法：**```html
<script async src="file.js"></script>
```
**JavaScript动态创建（默认为async）```javascript
const script = document.createElement("script");
script.src = "file.js";
document.body.appendChild(script);
```
**重要：**内联脚本永远不会异步，即使使用`async`属性。它们立即编译并运行：```html
<!-- This is NOT async despite the attribute -->
<script async>
  // Inline JavaScript code
</script>
```
**使用Blob url异步编译基于字符串的代码```javascript
const blob = new Blob([codeString]);
const script = document.createElement("script");
const url = URL.createObjectURL(blob);
script.onload = script.onerror = () => URL.revokeObjectURL(url);
script.src = url;
document.body.appendChild(script);
```
关键在于设置`src`（而不是`innerHTML`或`textContent`）会触发异步编译。

---

优化启动性能

**来源：** [MDN -优化启动性能]（https://developer.mozilla.org/en-US/docs/Web/Performance/Guides/Optimizing_startup_performance）

它是什么

提高网页应用和游戏启动和响应速度的策略集合，防止应用、浏览器或设备对用户出现冻结。

###如何工作

核心原则是避免在启动期间阻塞主线程。工作被卸载到后台线程（Web Workers），启动代码被分解成小的微任务，主线程被用户事件和渲染所占用。事件循环必须保持连续循环。

###何时使用这是所有网页应用和游戏的普遍关注点。
-对于新应用来说至关重要，因为从一开始就更容易异步构建。
-移植原生应用时需要同步加载和重构。

关键技术

* * 1。脚本加载`defer`和`async`**

防止阻止HTML解析：```html
<script defer src="app.js"></script>
<script async src="helper.js"></script>
```
* * 2。Web Workers for Heavy Processing**

将数据获取、解码和计算移到工作程序。这将为UI和用户事件释放主线程。

* * 3。数据处理* *

-使用浏览器提供的解码器（图像，视频），而不是自定义实现。
-尽可能并行处理数据，而不是顺序处理。
-卸载资产解码（例如，JPEG到原始纹理数据）给工人。

* * 4。资源加载* *

-不要在启动HTML的关键渲染路径之外包含脚本或样式表——只在需要时加载它们。
—使用资源提示：`preconnect`、`preload`。

* * 5。代码大小和压缩**

-缩小JavaScript文件。
—使用Gzip或Brotli压缩。
—优化压缩数据文件。

* * 6。感知性能* *-显示启动画面以保持用户粘性。
—重站点显示进度指标。
-让时间感觉更快，即使绝对持续时间保持不变。

* * 7。Emscripten Main Loop Blockers（用于移植应用程序）**```javascript
emscripten_push_main_loop_blocker();
// Establish functions to execute before main thread continues
// Create queue of functions called in sequence
```
###性能目标

|度量|目标||---|---|
|初始内容出现| 1-2秒|
用户可感知延迟|小于等于50ms |
|慢速阈值|大于200ms |

使用较旧或较慢设备的用户会比开发人员经历更长的延迟，因此请始终进行相应的优化。

---

## WebRTC数据通道

**来源：** [MDN - WebRTC数据通道]（https://developer.mozilla.org/en-US/docs/Games/Techniques/WebRTC_data_channels）

它是什么

WebRTC数据通道允许您通过活动连接向对等端发送文本或二进制数据。在游戏环境中，这使玩家能够相互发送数据进行文本聊天或游戏状态同步，而无需通过中央服务器。

###如何工作

WebRTC在两个浏览器之间建立点对点连接。一旦建立，就可以在该连接上打开数据通道。数据通道有两种类型：* *可靠渠道:* *
—保证消息到达对端。
-保持消息顺序——消息按照发送时的顺序到达。
—类似于TCP套接字。

* *不可靠的渠道:* *
—不保证消息的传递。
-消息可能不会以任何特定的顺序到达。
—消息可能根本无法到达。
—类似于UDP套接字。

###何时使用

**可靠的渠道：**回合制游戏，聊天，或任何情况下，每条消息必须按顺序到达。
**不可靠的渠道：**实时动作游戏，低延迟比保证交付更重要（例如，位置更新，陈旧的数据比丢失的数据更糟糕）。

游戏中的使用案例

-玩家间的文字聊天。
-玩家之间的游戏状态信息交换。
-实时游戏状态同步。
-没有专用游戏服务器的点对点多人游戏。实现说明

- WebRTC API主要用于音频和视频通信，但包括强大的点对点数据通道功能。
-建议使用库来简化实现并解决浏览器差异。
-完整的WebRTC文档可在[MDN WebRTC API]（https://developer.mozilla.org/en-US/docs/Web/API/WebRTC_API）。

---

网页游戏的音频

来源：** [MDN - Audio for Web Games]（https://developer.mozilla.org/en-US/docs/Games/Techniques/Audio_for_Web_Games）

它是什么

音频在网页游戏中提供反馈和氛围。该技术涵盖了在桌面和移动平台上实现音频，解决浏览器差异和优化策略。

###如何工作

有两个主要的api可用：

1. **HTMLMediaElement**——用于基本音频播放的标准`<audio>`元素。
2. **Web音频API**——用于动态音频操作，定位和精确定时的高级API。

###何时使用-使用`<audio>`元素简单，线性播放（背景音乐没有复杂的控制）。
-使用Web Audio API动态音乐，3D空间音频，精确定时和实时操作。
-当你瞄准手机或当你有很多短的声音效果时，使用音频精灵。

手机领域的主要挑战

- **自动播放策略：**浏览器限制声音自动播放。播放必须由用户通过点击或轻按发起。
- **音量控制：**移动浏览器可能会禁用编程音量控制，以保持操作系统级别的用户控制。
- **Buffering/preloading:**移动浏览器通常在播放开始之前禁用缓冲以减少数据使用。

技巧1：音频精灵

将多个音频剪辑合并到一个文件中，通过时间戳播放特定的部分，借用了CSS精灵的概念。

HTML: * * * *```html
<audio id="myAudio" src="mysprite.mp3"></audio>
<button data-start="18" data-stop="19">0</button>
<button data-start="16" data-stop="17">1</button>
<button data-start="14" data-stop="15">2</button>
<button data-start="12" data-stop="13">3</button>
<button data-start="10" data-stop="11">4</button>
<button data-start="8" data-stop="9">5</button>
<button data-start="6" data-stop="7">6</button>
<button data-start="4" data-stop="5">7</button>
<button data-start="2" data-stop="3">8</button>
<button data-start="0" data-stop="1">9</button>
```
JavaScript: * * * *```javascript
const myAudio = document.getElementById("myAudio");
const buttons = document.getElementsByTagName("button");
let stopTime = 0;

for (const button of buttons) {
  button.addEventListener("click", () => {
    myAudio.currentTime = button.dataset.start;
    stopTime = Number(button.dataset.stop);
    myAudio.play();
  });
}

myAudio.addEventListener("timeupdate", () => {
  if (myAudio.currentTime > stopTime) {
    myAudio.pause();
  }
});
```
**手机启动音频（在首次用户互动时触发）：**```javascript
const myAudio = document.createElement("audio");
myAudio.src = "my-sprite.mp3";
myAudio.play();
myAudio.pause();
```
技术2:Web Audio API多轨音乐

加载和同步单独的音频轨道与精确的定时。

**创建音频上下文和加载文件：**```javascript
const audioCtx = new AudioContext();

async function getFile(filepath) {
  const response = await fetch(filepath);
  const arrayBuffer = await response.arrayBuffer();
  const audioBuffer = await audioCtx.decodeAudioData(arrayBuffer);
  return audioBuffer;
}
```
**轨道播放与同步：**```javascript
let offset = 0;

function playTrack(audioBuffer) {
  const trackSource = audioCtx.createBufferSource();
  trackSource.buffer = audioBuffer;
  trackSource.connect(audioCtx.destination);

  if (offset === 0) {
    trackSource.start();
    offset = audioCtx.currentTime;
  } else {
    trackSource.start(0, audioCtx.currentTime - offset);
  }

  return trackSource;
}
```
**处理播放处理程序中的自动播放策略：**```javascript
playButton.addEventListener("click", () => {
  if (audioCtx.state === "suspended") {
    audioCtx.resume();
  }

  playTrack(track);
  playButton.dataset.playing = true;
});
```
技术3：节拍同步轨道回放

无缝过渡，同步新的轨道，以击败边界：```javascript
const tempo = 3.074074076; // Time in seconds of your beat/bar

if (offset === 0) {
  source.start();
  offset = context.currentTime;
} else {
  const relativeTime = context.currentTime - offset;
  const beats = relativeTime / tempo;
  const remainder = beats - Math.floor(beats);
  const delay = tempo - remainder * tempo;
  source.start(context.currentTime + delay, relativeTime + delay);
}
```
技术4：位置音频（3D空间化）

使用`PannerNode`在3D空间中定位音频：

-在游戏世界空间中定位对象。
-设置音频源的方向和移动。
-应用环境效果（洞穴混响，水下消声等）。

对于WebGL 3D游戏来说，将音频与视觉对象和玩家的视角联系起来尤其有用。

###决策矩阵

|技术|使用时|优点|缺点||---|---|---|---|
|音频精灵|许多短声音，移动|减少HTTP请求，移动友好|搜索精度降低在低比特率|
|基本`<audio>`|简单线性播放|广泛支持|有限控制，自动播放限制|
|动态音乐，3D定位，精确定时|完全控制，实时操纵，同步|更复杂的代码|
|位置音频| 3D沉浸式游戏|真实感，玩家沉浸|需要WebGL上下文感知|

---

2D碰撞检测

**来源：** [MDN - 2D碰撞检测]（https://developer.mozilla.org/en-US/docs/Games/Techniques/2D_collision_detection）

它是什么2D碰撞检测算法根据游戏实体的形状类型（矩形到矩形、矩形到圆、圆到圆等）确定它们何时重叠或相交。比起像素完美的检测，游戏通常使用简单的通用形状“hitbox”来覆盖实体，平衡视觉准确性和性能。

###如何工作

每个算法检查两个形状之间的几何关系。如果检测到任何重叠，则报告碰撞。方法因形状类型而异。

###何时使用

-对没有旋转的简单矩形实体使用AABB。
-使用圆碰撞圆形实体或当你需要快速，简单的检查。
-对复杂凸多边形使用分离轴定理（SAT）。
-当你有很多实体时，使用宽相位缩小（四叉树，空间哈希图）。

算法1：轴对齐边界框（AABB）两个轴线对齐的矩形之间的碰撞检测（不旋转）。通过确保矩形的4个边之间没有间隙来检测碰撞。```javascript
class BoxEntity extends BaseEntity {
  width = 20;
  height = 20;

  isCollidingWith(other) {
    return (
      this.position.x < other.position.x + other.width &&
      this.position.x + this.width > other.position.x &&
      this.position.y < other.position.y + other.height &&
      this.position.y + this.height > other.position.y
    );
  }
}
```
算法2：圆碰撞

两个圆之间的碰撞检测。取两个圆的中心点并检查它们之间的距离是否小于它们的半径之和。```javascript
class CircleEntity extends BaseEntity {
  radius = 10;

  isCollidingWith(other) {
    const dx =
      this.position.x + this.radius - (other.position.x + other.radius);
    const dy =
      this.position.y + this.radius - (other.position.y + other.radius);
    const distance = Math.sqrt(dx * dx + dy * dy);
    return distance < this.radius + other.radius;
  }
}
```
注意：圆的`x`和`y`坐标指的是它们的左上角，因此必须添加半径来比较它们的实际中心。

算法3：分离轴定理（SAT）

一种检测任意两个凸多边形之间碰撞的碰撞算法。它的工作原理是将每个多边形投影到每个可能的轴上并检查重叠。如果任何轴显示间隙，则多边形没有碰撞。

SAT实现起来更复杂，但可以处理任意凸多边形形状。

碰撞性能：宽相位和窄相位

将每个实体与其他实体进行对比测试在计算上非常昂贵（O(n^2)）。游戏将碰撞检测分为两个阶段：

**宽相位**——使用空间数据结构快速识别可能发生碰撞的实体：
-四方树
- r - tree
-空间散列映射**窄相位** -应用精确的碰撞算法（AABB, Circle， SAT）仅适用于来自宽相位的小候选列表。

基础引擎代码

**碰撞可视化CSS:**```css
.entity {
  display: inline-block;
  position: absolute;
  height: 20px;
  width: 20px;
  background-color: blue;
}

.movable {
  left: 50px;
  top: 50px;
  background-color: red;
}

.collision-state {
  background-color: green !important;
}
```
**JavaScript碰撞检查器和实体系统：**```javascript
const collider = {
  moveableEntity: null,
  staticEntities: [],
  checkCollision() {
    const isColliding = this.staticEntities.some((staticEntity) =>
      this.moveableEntity.isCollidingWith(staticEntity),
    );
    this.moveableEntity.setCollisionState(isColliding);
  },
};

const container = document.getElementById("container");

class BaseEntity {
  ref;
  position;
  constructor(position) {
    this.position = position;
    this.ref = document.createElement("div");
    this.ref.classList.add("entity");
    this.ref.style.left = `${this.position.x}px`;
    this.ref.style.top = `${this.position.y}px`;
    container.appendChild(this.ref);
  }
  shiftPosition(dx, dy) {
    this.position.x += dx;
    this.position.y += dy;
    this.redraw();
  }
  redraw() {
    this.ref.style.left = `${this.position.x}px`;
    this.ref.style.top = `${this.position.y}px`;
  }
  setCollisionState(isColliding) {
    if (isColliding && !this.ref.classList.contains("collision-state")) {
      this.ref.classList.add("collision-state");
    } else if (!isColliding) {
      this.ref.classList.remove("collision-state");
    }
  }
  isCollidingWith(other) {
    throw new Error("isCollidingWith must be implemented in subclasses");
  }
}

document.addEventListener("keydown", (e) => {
  e.preventDefault();
  switch (e.key) {
    case "ArrowLeft":
      collider.moveableEntity.shiftPosition(-5, 0);
      break;
    case "ArrowUp":
      collider.moveableEntity.shiftPosition(0, -5);
      break;
    case "ArrowRight":
      collider.moveableEntity.shiftPosition(5, 0);
      break;
    case "ArrowDown":
      collider.moveableEntity.shiftPosition(0, 5);
      break;
  }
  collider.checkCollision();
});
```
---

# # Tilemaps

**来源：** [MDN - Tilemaps]（https://developer.mozilla.org/en-US/docs/Games/Techniques/Tilemaps）

它是什么

瓦片地图是2D游戏开发中的一项基本技术，它使用被称为瓦片的小而规则的图像来构建游戏世界。游戏世界不是存储大型的单一关卡图像，而是由可重复使用的贴图图形网格组合而成，提供了显著的性能和内存优势。

###如何工作

核心结构:* * * *

1. **瓦片图集（精灵表）：**所有瓦片图像存储在一个单一的图集文件。每个tile被分配一个索引作为它的标识符。
2. **瓷砖数据对象：**包含瓷砖大小（像素尺寸），图像图集参考，地图尺寸（瓷砖或像素），一个视觉网格（瓷砖索引数组），和一个可选的逻辑网格（碰撞，寻径，刷出数据）。

特殊值（负数、0或null）表示空tile。

###何时使用-创建任何类型的2D游戏世界（平台游戏，rpg，策略游戏，益智游戏）。
-受经典游戏如《超级马里奥》、《吃豆人》、《塞尔达传说》、《星际争霸》或《模拟城市》启发的游戏。
-任何基于网格的世界为寻路、碰撞或关卡编辑提供逻辑优势的场景。

渲染静态磁贴图

对于完全适合屏幕的地图：```javascript
for (let column = 0; column < map.columns; column++) {
  for (let row = 0; row < map.rows; row++) {
    const tile = map.getTile(column, row);
    const x = column * map.tileSize;
    const y = row * map.tileSize;
    drawTile(tile, x, y);
  }
}
```
带有相机的滚动磁贴图

在世界坐标（关卡位置）和屏幕坐标（渲染位置）之间进行转换：```javascript
// These functions assume camera points to top-left corner

function worldToScreen(x, y) {
  return { x: x - camera.x, y: y - camera.y };
}

function screenToWorld(x, y) {
  return { x: x + camera.x, y: y + camera.y };
}
```
关键原则：只呈现可见的贴图以优化性能。在渲染过程中应用相机偏移变换。

###贴图类型

**方形瓷砖（最常见）：**
- rpg和策略游戏（魔兽争霸2，最终幻想）的自上而下视图。
-平台游戏（超级马里奥兄弟）的侧视图。

* *等距Tilemaps: * *
-创造3D环境的幻觉。
-在模拟和策略游戏（模拟城市2000，法老，最终幻想战术）中很受欢迎。

# # #层

多个视觉层支持：
-重用不同背景类型的磁贴。
-人物出现在地形后面或前面（走在树后面）。
-更丰富的世界和更少的贴图变化。

示例：在草地、沙子或砖块背景上的单独图层上渲染的岩石瓷砖。

逻辑网格用于非视觉游戏逻辑的独立网格：
- **碰撞检测：**标记可行走与阻挡瓷砖。
- **角色产卵：**定义产卵点的位置。
- **寻径：**创建导航图。
- **瓷砖组合：**检测有效的模式（俄罗斯方块，宝石迷阵）。

性能优化

1. **只渲染可见的贴图**—完全跳过屏幕外的贴图。
2. **预渲染到画布**——渲染地图到屏幕外的画布元素和blit作为一个单一的操作。
3. **离线缓冲**——绘制一个比可见区域更大的部分（2x2瓷砖大），以减少滚动期间的重绘。
4. **分块**——将大型贴图划分为多个部分（例如，10x10的贴图块），将每个部分预渲染为“大贴图”。

---

控制：手柄API

**来源：** [MDN -控制手柄API]（https://developer.mozilla.org/en-US/docs/Games/Techniques/Controls_Gamepad_API）

它是什么Gamepad API提供了一个在没有插件的web浏览器中检测和使用Gamepad控制器的接口。它通过JavaScript显示按钮按压和轴的改变，允许像主机一样控制基于浏览器的游戏。

###如何工作

处理控制器生命周期的两个基本事件：

-`gamepadconnected`——连接手柄时触发。
-`gamepaddisconnected`——断开连接时触发（物理上或由于不活动）。

安全注意事项：当事件触发页面可见时，需要用户与控制器交互（防止指纹识别）。

**手柄对象属性：**

|属性|描述||---|---|
|`id`|包含控制器信息的字符串|
|`index`|接入设备|的唯一标识符
|`connected`|布尔值，表示连接状态|
|`mapping`|布局类型（“标准”是常见选项）|
|`axes`|表示模拟摇杆位置的浮点数数组（-1到1）|
带有`pressed`和`value`属性的GamepadButton对象数组|

###何时使用

-当创建应该与主机控制器一起工作的游戏时。
—支持Windows和macOS上的Xbox 360、Xbox One、PS3或PS4控制器。
-当你想要双输入支持（键盘+手柄）。

代码示例

**基本设置结构：**```javascript
const gamepadAPI = {
  controller: {},
  turbo: false,
  connect() {},
  disconnect() {},
  update() {},
  buttonPressed() {},
  buttons: [],
  buttonsCache: [],
  buttonsStatus: [],
  axesStatus: [],
};
```
**按钮布局（Xbox 360）：**```javascript
const gamepadAPI = {
  buttons: [
    "DPad-Up", "DPad-Down", "DPad-Left", "DPad-Right",
    "Start", "Back", "Axis-Left", "Axis-Right",
    "LB", "RB", "Power", "A", "B", "X", "Y",
  ],
};
```
* *事件监听器:* *```javascript
window.addEventListener("gamepadconnected", gamepadAPI.connect);
window.addEventListener("gamepaddisconnected", gamepadAPI.disconnect);
```
**连接和断开处理程序：**```javascript
connect(evt) {
  gamepadAPI.controller = evt.gamepad;
  gamepadAPI.turbo = true;
  console.log("Gamepad connected.");
},

disconnect(evt) {
  gamepadAPI.turbo = false;
  delete gamepadAPI.controller;
  console.log("Gamepad disconnected.");
},
```
更新方法（每帧调用）：**```javascript
update() {
  // Clear the buttons cache
  gamepadAPI.buttonsCache = [];

  // Move the buttons status from the previous frame to the cache
  for (let k = 0; k < gamepadAPI.buttonsStatus.length; k++) {
    gamepadAPI.buttonsCache[k] = gamepadAPI.buttonsStatus[k];
  }

  // Clear the buttons status
  gamepadAPI.buttonsStatus = [];

  // Get the gamepad object
  const c = gamepadAPI.controller || {};

  // Loop through buttons and push the pressed ones to the array
  const pressed = [];
  if (c.buttons) {
    for (let b = 0; b < c.buttons.length; b++) {
      if (c.buttons[b].pressed) {
        pressed.push(gamepadAPI.buttons[b]);
      }
    }
  }

  // Loop through axes and push their values to the array
  const axes = [];
  if (c.axes) {
    for (const ax of c.axes) {
      axes.push(ax.toFixed(2));
    }
  }

  // Assign received values
  gamepadAPI.axesStatus = axes;
  gamepadAPI.buttonsStatus = pressed;

  return pressed;
},
```
**按钮检测与保持支持：**```javascript
buttonPressed(button, hold) {
  let newPress = false;
  if (gamepadAPI.buttonsStatus.includes(button)) {
    newPress = true;
  }
  if (!hold && gamepadAPI.buttonsCache.includes(button)) {
    newPress = false;
  }
  return newPress;
},
```
参数:
-`button`——要侦听的按钮名称。
-`hold`——如果为真，按住按钮算作连续动作；如果为false，则只有新的印刷机注册。

**在游戏循环中的使用：**```javascript
if (gamepadAPI.turbo) {
  if (gamepadAPI.buttonPressed("A", "hold")) {
    this.turbo_fire();
  }
  if (gamepadAPI.buttonPressed("B")) {
    this.managePause();
  }
}
```
**模拟杆输入与阈值（防止杆漂移）：**```javascript
if (gamepadAPI.axesStatus[0].x > 0.5) {
  this.player.angle += 3;
  this.turret.angle += 3;
}
```
**连接所有手柄：**```javascript
const gamepads = navigator.getGamepads();
// Returns an array where unavailable/disconnected slots contain null
// Example with one device at index 1: [null, [object Gamepad]]
```
---

清晰的像素艺术外观

**来源：** [MDN - Crisp Pixel Art Look]（https://developer.mozilla.org/en-US/docs/Games/Techniques/Crisp_pixel_art_look）

它是什么

一种在高分辨率显示器上通过将单个图像像素映射到屏幕像素块而不进行平滑插值来呈现像素艺术的技术。复古的像素艺术需要在缩放过程中保留硬边，但现代浏览器默认使用平滑算法来混合颜色并产生模糊。

###如何工作

CSS`image-rendering`属性控制浏览器缩放图像的方式。将其设置为`pixelated`可强制执行最近邻缩放，从而保留像素艺术的清晰块状外观，而不是应用双线性或双三次平滑。

** CSS关键值：**
-`pixelated`-保留像素艺术的清晰边缘。
-`crisp-edges`——某些浏览器支持的替代选项。

###何时使用-复古风格的游戏与像素艺术资产。
-任何游戏，你想要一个故意块，像素化的视觉风格。
-当缩放小精灵图像到更大的显示尺寸时。

技术1：用CSS缩放`<img>`元素```html
<img
  src="character.png"
  alt="pixel art character, upscaled with CSS, appearing crisp" />
```

```css
img {
  width: 48px;
  height: 136px;
  image-rendering: pixelated;
}
```
技巧2：画布中的像素艺术

将画布`width`/`height`属性设置为原始像素图像分辨率，然后使用CSS`width`/`height`进行缩放（例如，4倍缩放：128像素到512px CSS宽度）。```html
<canvas id="game" width="128" height="128">A cat</canvas>
```

```css
canvas {
  width: 512px;
  height: 512px;
  image-rendering: pixelated;
}
```

```javascript
const ctx = document.getElementById("game").getContext("2d");

const image = new Image();
image.onload = () => {
  ctx.drawImage(image, 0, 0);
};
image.src = "cat.png";
```
技术3：任意画布缩放与校正

对于非整数比例因子，图像像素必须以整数倍对齐画布像素：```javascript
const ctx = document.getElementById("game").getContext("2d");
ctx.scale(0.8, 0.8);

const image = new Image();
image.onload = () => {
  // Correct formula: dWidth = sWidth / xScale * n (where n is an integer)
  ctx.drawImage(image, 0, 0, 128, 128, 0, 0, 128 / 0.8, 128 / 0.8);
};
image.src = "cat.png";
```
使用`drawImage(image, sx, sy, sWidth, sHeight, dx, dy, dWidth, dHeight)`时：
—`dWidth`必须等于`sWidth / xScale * n`—`dHeight`必须等于`sHeight / yScale * m`—其中`n`和`m`为正整数（1、2、3等）

已知的限制

当`devicePixelRatio`不是一个整数（例如，在110%的浏览器缩放），像素可能呈现不均匀，因为CSS像素不能完美地映射到设备像素。这造成了不均匀的外观，没有一个简单的解决方案。

最佳实践

1. 尽可能使用整数比例因子（2x, 3x, 4x）。
2. 保持宽高比——宽度和高度相等。
3. 测试在不同的浏览器缩放级别。
4. 避免分割画布比例因子或drawImage尺寸。
5. 在画布元素上包含描述性的`aria-label`属性以提高可访问性。