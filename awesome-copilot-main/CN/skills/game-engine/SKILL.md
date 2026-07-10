---
name: game-engine
description: 'Expert skill for building web-based game engines and games using HTML5, Canvas, WebGL, and JavaScript. Use when asked to create games, build game engines, implement game physics, handle collision detection, set up game loops, manage sprites, add game controls, or work with 2D/3D rendering. Covers techniques for platformers, breakout-style games, maze games, tilemaps, audio, multiplayer via WebRTC, and publishing games.'
---
#游戏引擎技能

使用HTML5 Canvas， WebGL和JavaScript创建基于网页的游戏和游戏引擎。该技能包括初学者模板，参考文档和逐步的工作流程，用于2D和3D游戏开发框架，如Phaser，Three.js，Babylon.js和A-Frame。

何时使用此技能

-使用web技术从头开始构建游戏引擎或游戏
-实现游戏循环，物理，碰撞检测，或渲染
-使用HTML5 Canvas， WebGL或SVG制作游戏图像
-增加游戏控制（键盘，鼠标，触摸，手柄）
-创建2D平台游戏、突破式游戏、迷宫游戏或3D体验
-使用瓦片图，精灵或动画
-为网页游戏添加音频
-实现多人功能与webbrtc或WebSockets
-优化游戏性能
-发行和分销网页游戏

# #先决条件具备基本的HTML、CSS和JavaScript知识
-一个现代的web浏览器与Canvas/WebGL支持
-文本编辑器或IDE
—可选：Node.js用于构建工具和本地开发服务器

##核心概念

以下概念构成了所有网页游戏引擎的基础。

游戏循环

每个游戏引擎都围绕着游戏循环——一个连续的循环：

1. **进程输入** -读取键盘，鼠标，触摸或手柄输入
2. **更新状态** -更新游戏对象位置，物理，AI和逻辑
3. **渲染** -绘制当前游戏状态到屏幕上

使用`requestAnimationFrame`进行平滑的浏览器优化渲染。

# # #呈现

- **Canvas 2D** -最适合2D游戏，基于精灵的渲染，和瓦片图
- **WebGL** -硬件加速3D和高级2D渲染
**SVG** -基于矢量的图形，适用于UI元素
**CSS** -用于基于dom的游戏元素和过渡物理和碰撞检测

- **2D碰撞检测** -基于AABB，圆和sat的碰撞
- **3D碰撞检测** -边界框，边界球，和光线投射
- **速度和加速度** -运动的基本牛顿物理学
- **重力** -平台游戏的恒定向下加速度

# # #控制

- **键盘** -方向键，方向键和自定义键绑定
- **鼠标** -点击，移动，指针锁定fps风格的控制
- **触摸** -移动触摸事件和虚拟操纵杆
- **手柄** -手柄API控制器支持

# # #音频

- **Web音频API** -程序化的声音生成和空间音频
- **HTML5音频** -简单的音频播放音乐和声音效果

##分步工作流程

创造一款基本的2D游戏1. 用`<canvas>`元素设置一个HTML文件
2. 获取2D渲染上下文
3. 使用`requestAnimationFrame`实现游戏循环
4. 创建具有位置、速度和大小属性的游戏对象
5. 处理玩家控制的keyboard/mouse输入
6. 实现游戏对象之间的碰撞检测
7. 添加得分、生命和win/lose条件
8. 添加音效和音乐

制作3D游戏

1. 选择一个框架（Three.js,Babylon.js, a - frame，或PlayCanvas）
2. 设置场景、相机和渲染器
3. 加载或创建3D模型和纹理
4. 实现照明和着色器
5. 添加物理和碰撞检测
6. 执行玩家控制和镜头移动
7. 添加音频和视觉效果

发行游戏1. 优化资产（压缩图像，减少代码）
2. 跨浏览器和设备进行测试
3. 选择发行平台（网页、应用商店、游戏门户）
4. 必要时实施货币化
5. 通过游戏社区和社交媒体进行推广

##游戏模板

在`assets/`文件夹中可以获得Starter模板。每个模板都提供了一个完整的工作示例，可以用作新项目的起点。

|模板|描述||----------|-------------|
2D突破风格的游戏，纯JavaScript |
|`2d-maze-game.md`|迷宫游戏与设备方向控制|`2d-platform-game.md`|平台游戏使用Phaser框架|
|游戏基础模板库结构|
带有碰撞的简单2D平台引擎

##参考文档

详细的参考资料可在`references/`文件夹中找到。有关特定主题的深入报道，请参阅这些文件。

|参考|主题涵盖||-----------|---------------|
|`basics.md`|游戏开发介绍与剖析|
|`web-apis.md`| Canvas, WebGL, Web Audio， Gamepad和其他Web api |
碰撞检测，贴图，异步脚本，音频|
|`3d-web-games.md`| 3D理论，框架，着色器，WebXR |
|`game-control-mechanisms.md`|触摸、键盘、鼠标和手柄控制|
发行、推广和盈利|
射线投射，碰撞，物理，矢量数学
游戏开发术语|
游戏引擎的核心设计原则

# #故障排除

|问题|解决方案||-------|----------|
|画布是空白|检查你正在调用绘图方法后获得上下文和游戏循环|
|游戏以不同的速度运行|在更新计算中使用增量时间而不是固定值|
|碰撞检测不一致|对快速运动物体使用连续碰撞检测或减少时间步长|
|浏览器在播放音频前需要用户交互；触发点击处理程序|的回放
|配置文件与浏览器开发工具，减少绘制调用，使用对象池，并优化资产大小|
防止默认的触摸行为，并将触摸事件与鼠标事件分开处理|
|处理`webglcontextlost`事件并恢复`webglcontextrestored`|上的状态