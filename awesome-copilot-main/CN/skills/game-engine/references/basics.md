#游戏开发基础

这是一本涵盖网页游戏开发技术、游戏架构和游戏循环剖析的综合参考。

来源:- https://developer.mozilla.org/en-US/docs/Games/Introduction
- https://developer.mozilla.org/en-US/docs/Games/Anatomy
---

游戏开发的Web技术

图形和渲染

- **WebGL**——基于OpenGL ES 2.0的硬件加速2D和3D图形。为高性能渲染提供直接的GPU访问。
- **Canvas API** -通过`<canvas>`元素绘制2D绘图面。适用于2D游戏，精灵渲染和像素操作。
- **SVG**——用于独立于分辨率的视觉效果的可缩放矢量图形。对于UI元素和简单的矢量游戏非常有用。
- **HTML/CSS**——用于构建游戏UI，菜单，hud和覆盖的标准web技术。

# # #音频

- **Web音频API** -先进的音频引擎，支持实时回放，合成，空间音频，效果处理和动态混音。
- **HTML音频元素** -简单的声音播放背景音乐和基本的声音效果。

输入和控件- **手柄API**——支持游戏控制器和手柄，包括按键映射和模拟摇杆输入。
- **触摸事件API**——移动设备的多点触摸输入处理。
**指针锁定API**——锁定游戏区域内的鼠标光标，并为精确的camera/aiming控制提供原始坐标增量。
- **设备传感器** -加速度计和陀螺仪访问基于运动的输入。
- **全屏API**——实现沉浸式全屏游戏体验。

网络和多人模式

- **WebSockets API**——用于实时多人，聊天和实时更新的持久，双向通信通道。
- **WebRTC API** -点对点连接，用于低延迟多人，语音聊天和数据通道。
**Fetch API**——用于下载游戏资产、加载关卡数据和传输非实时游戏状态的HTTP请求。

###数据存储和性能**IndexedDB API**——客户端结构化存储，用于保存游戏、缓存资产和离线游戏支持。
- **类型化数组** -直接访问GL纹理，音频样本和紧凑游戏数据的原始二进制数据缓冲区。
**Web Workers API**——后台线程执行，用于卸载繁重的计算（物理，寻径，AI），而不会阻塞主线程。

语言和编译

**JavaScript**——网页游戏开发的主要语言。
** * xqz3xq++通过Emscripten**——将现有的本地游戏代码编译成JavaScript或WebAssembly用于web部署。
**WebAssembly (Wasm)**——性能关键型游戏代码的近乎原生的执行速度。

---

你可以创造的游戏类型

现代网络平台支持各种类型的游戏：- 3D动作游戏和射击游戏
-角色扮演游戏（rpg）
- 2D平台游戏和横向卷轴游戏
-益智和策略游戏
-纸牌和棋盘游戏
-休闲和手机友好型游戏
-实时联网的多人游戏体验

---

基于网页的游戏开发优势1. **普及性**——游戏可以通过浏览器在智能手机、平板电脑、pc和智能电视上运行。
2. **不依赖应用商店**——直接部署在网络上，无需商店审批流程。
3. **全面的收入控制**——没有强制性的收入分成；使用任何付款处理系统。
4. **即时更新** -即时推送更新，无需等待商店审查。
5. **拥有你的分析**——收集你自己的数据或选择任何分析提供商。
6. **直接的玩家关系**——无需中介就能吸引玩家。
7. **固有的可共享性**——游戏是通过标准的网络机制链接和发现的。

---

解析游戏循环

每款游戏都是通过一个连续的步骤循环来运行的：1. **Present**——向玩家显示当前游戏状态。
2. **Accept**——接收用户输入（键盘、鼠标、手柄、触摸）。
3. **Interpret**——将原始输入转化为有意义的游戏动作。
4. **计算**——根据动作、物理、AI和时间更新游戏状态。
5. **Repeat**——循环返回显示更新的状态。

游戏可能是事件驱动的（回合制，等待玩家行动），也可能是逐帧的（通过主循环不断更新）。

---

用requestAnimationFrame创建游戏循环

基本主循环```javascript
window.main = () => {
  window.requestAnimationFrame(main);

  // Your game logic here: update state, render frame
};

main(); // Start the cycle
```
重点:
-`requestAnimationFrame()`同步回调到浏览器的重绘计划（通常为60 Hz）。
-在执行循环工作之前安排下一帧，以最大化可用的计算时间。

自包含主循环（IIFE）```javascript
;(() => {
  function main() {
    window.requestAnimationFrame(main);

    // Game logic here
  }

  main();
})();
```
可停止的主循环```javascript
;(() => {
  function main() {
    MyGame.stopMain = window.requestAnimationFrame(main);

    // Game logic here
  }

  main();
})();

// To stop the loop:
window.cancelAnimationFrame(MyGame.stopMain);
```
---

定时和帧率

# # # DOMHighResTimeStamp`requestAnimationFrame`将一个`DOMHighResTimeStamp`传递给回调函数，为1/1000th提供一毫秒的计时精度。```javascript
;(() => {
  function main(tFrame) {
    MyGame.stopMain = window.requestAnimationFrame(main);

    // tFrame is a high-resolution timestamp in milliseconds
    // Use it for delta-time calculations
  }

  main();
})();
```
框架时间预算

在60 Hz时，每帧大约有16.67ms的可用处理时间。浏览器的帧周期是：

1. 开始新帧（前一帧显示在屏幕上）
2. 执行`requestAnimationFrame`回调
3. 执行垃圾收集和逐帧浏览器任务
4. 一直睡到垂直同步，然后重复

---

简单的更新和渲染模式

当你的游戏能够维持目标帧率时，最简单的方法是：```javascript
;(() => {
  function main(tFrame) {
    MyGame.stopMain = window.requestAnimationFrame(main);

    update(tFrame); // Process game logic
    render();       // Draw the frame
  }

  main();
})();
```
假设:
-每一帧可以在时间预算内处理输入和更新状态。
-模拟以与显示器刷新相同的速率运行（通常为~60 FPS）。
-不需要帧插值。

---

解耦更新和渲染固定的时间步长

对于可变刷新率和一致的模拟行为的稳健处理：```javascript
;(() => {
  function main(tFrame) {
    MyGame.stopMain = window.requestAnimationFrame(main);
    const nextTick = MyGame.lastTick + MyGame.tickLength;
    let numTicks = 0;

    // Calculate how many simulation updates are needed
    if (tFrame > nextTick) {
      const timeSinceTick = tFrame - MyGame.lastTick;
      numTicks = Math.floor(timeSinceTick / MyGame.tickLength);
    }

    queueUpdates(numTicks);
    render(tFrame);
    MyGame.lastRender = tFrame;
  }

  function queueUpdates(numTicks) {
    for (let i = 0; i < numTicks; i++) {
      MyGame.lastTick += MyGame.tickLength;
      update(MyGame.lastTick);
    }
  }

  MyGame.lastTick = performance.now();
  MyGame.lastRender = MyGame.lastTick;
  MyGame.tickLength = 50; // 20 Hz simulation rate (50ms per tick)

  setInitialState();
  main(performance.now());
})();
```
好处:
- **确定性模拟** -游戏逻辑运行在一个固定的频率，无论显示刷新率。
- **平滑渲染** -渲染可以在模拟状态之间插入视觉平滑。
- **便携行为** -游戏在60hz， 120hz和144hz显示器上表现相同。

---

可选择的体系结构模式

###单独setInterval用于更新```javascript
// Game logic updates at a fixed rate
setInterval(() => {
  update();
}, 50); // 20 Hz

// Rendering synchronized to display
requestAnimationFrame(function render(tFrame) {
  requestAnimationFrame(render);
  draw();
});
```
缺点：即使选项卡不可见，`setInterval`也会继续运行，浪费资源。

Web Worker用于更新```javascript
// Heavy game logic runs in a background thread
const updateWorker = new Worker('game-update-worker.js');

requestAnimationFrame(function render(tFrame) {
  requestAnimationFrame(render);
  updateWorker.postMessage({ ticks: numTicksNeeded });
  draw();
});
```
优点：不会阻塞主线程。非常适合物理或ai密集型游戏。
缺点：工作线程和主线程之间的通信开销。

requestAnimationFrame驱动Web Worker```javascript
;(() => {
  function main(tFrame) {
    MyGame.stopMain = window.requestAnimationFrame(main);

    // Signal worker to compute updates
    updateWorker.postMessage({
      lastTick: MyGame.lastTick,
      numTicks: calculatedNumTicks
    });

    render(tFrame);
  }

  main();
})();
```
优点：不依赖遗留计时器。Worker并行执行计算。

---

处理标签焦点丢失

当浏览器选项卡失去焦点时，`requestAnimationFrame`减慢或完全停止。策略:

|策略|描述|最适合||---|---|---|
|跳过经过的时间；不更新|单人游戏|
|模拟差距|运行所有错过的更新重新获得|简单的模拟|
|同步从server/peer|获取权威状态|多人游戏|

在焦点恢复事件之后监视`numTicks`值。一个非常大的值表示游戏被暂停，可能需要特殊处理，而不是试图模拟所有丢失的帧。

---

计时方法的比较

|接近|优点|缺点||---|---|---|
|简单的update/render每帧|易于实现，响应|中断在slow/fast硬件|
|固定时间步长+插值|模拟一致，视觉效果流畅|实现|更复杂
|质量缩放|动态保持帧率|需要自适应质量系统|

---

性能最佳实践从主循环中分离非帧关键代码。对UI、网络响应和其他异步操作使用事件和回调。
**使用Web Workers**计算昂贵的任务，如物理，寻径和AI。
- **利用GPU加速**通过WebGL进行渲染。
- **保持在帧预算内** -监控你的更新和渲染时间，以保持它在16.67毫秒60 FPS。
- **通过重用对象和避免逐帧分配来控制垃圾收集压力。
**尽早计划你的时间策略**——在开发过程中改变游戏循环架构是很困难且容易出错的。

---

流行的3D框架和库- **Three.js**——具有大型生态系统的通用3D库。
- **Babylon.js**——全功能的3D游戏引擎，具有物理，音频和场景管理。
- **A-Frame**——基于Three.js的声明式3D/VR框架。
**PlayCanvas**——带有可视化编辑器的云托管3D游戏引擎。
相位器——带有物理和输入处理的流行2D游戏框架。