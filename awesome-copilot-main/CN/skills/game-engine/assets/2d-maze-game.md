# 2D迷宫游戏模板

这是一款针对移动设备优化的2D迷宫游戏，玩家将引导一个球穿过迷宫般的障碍到达目标洞。游戏在移动设备上使用设备方向API进行倾斜运动控制，在桌面上使用键盘箭头键。使用**Phaser**框架（v2）构建。x与Arcade Physics)，它具有多层次的进展，碰撞检测，音频反馈，振动触觉和计时器系统。

**来源：** [MDN - HTML5 Gamedev Phaser Device Orientation]（https://developer.mozilla.org/en-US/docs/Games/Tutorials/HTML5_Gamedev_Phaser_Device_Orientation）
**现场演示：** [Cyber Orb]（https://orb.enclavegames.com/）
**源代码：** [GitHub -EnclaveGames/Cyber-Orb]（https://github.com/EnclaveGames/Cyber-Orb）

---

##游戏理念玩家通过倾斜移动设备或按箭头键来控制球（“球体”）。球滚过一个迷宫的水平和垂直的墙段。每个关卡的目标是在避开墙壁的同时将球导向屏幕顶部的一个洞。与墙壁的碰撞会触发弹跳、声音效果和可选振动。计时器记录玩家在每个关卡和整个游戏中花费的时间。

---

项目结构```
project/
  index.html
  src/
    phaser-arcade-physics.2.2.2.min.js
    Boot.js
    Preloader.js
    MainMenu.js
    Howto.js
    Game.js
  img/
    ball.png
    hole.png
    element-horizontal.png
    element-vertical.png
    button-start.png
    loading-bg.png
    loading-bar.png
  audio/
    bounce.ogg
    bounce.mp3
    bounce.m4a
```
---

相位器设置和初始化

HTML入口点```html
<!doctype html>
<html lang="en">
<head>
  <meta charset="utf-8" />
  <title>Cyber Orb</title>
  <style>
    body { margin: 0; background: #333; }
  </style>
  <script src="src/phaser-arcade-physics.2.2.2.min.js"></script>
  <script src="src/Boot.js"></script>
  <script src="src/Preloader.js"></script>
  <script src="src/MainMenu.js"></script>
  <script src="src/Howto.js"></script>
  <script src="src/Game.js"></script>
</head>
<body>
  <script>
    (() => {
      const game = new Phaser.Game(320, 480, Phaser.CANVAS, "game");
      game.state.add("Boot", Ball.Boot);
      game.state.add("Preloader", Ball.Preloader);
      game.state.add("MainMenu", Ball.MainMenu);
      game.state.add("Howto", Ball.Howto);
      game.state.add("Game", Ball.Game);
      game.state.start("Boot");
    })();
  </script>
</body>
</html>
```
—画布大小：`320 x 480`-渲染器：`Phaser.CANVAS`（替代品：`Phaser.WEBGL`，`Phaser.AUTO`）

---

##游戏状态架构

游戏遵循线性状态流：```
Boot --> Preloader --> MainMenu --> Howto --> Game
```
###启动状态

为加载屏幕加载最小的资产并配置缩放。```javascript
const Ball = {
  _WIDTH: 320,
  _HEIGHT: 480,
};

Ball.Boot = function (game) {};
Ball.Boot.prototype = {
  preload() {
    this.load.image("preloaderBg", "img/loading-bg.png");
    this.load.image("preloaderBar", "img/loading-bar.png");
  },
  create() {
    this.game.scale.scaleMode = Phaser.ScaleManager.SHOW_ALL;
    this.game.scale.pageAlignHorizontally = true;
    this.game.scale.pageAlignVertically = true;
    this.game.state.start("Preloader");
  },
};
```
预加载器状态

在加载所有游戏资源时显示一个视觉加载条。为了跨浏览器兼容，音频以多种格式加载。```javascript
Ball.Preloader = function (game) {};
Ball.Preloader.prototype = {
  preload() {
    this.preloadBg = this.add.sprite(
      (Ball._WIDTH - 297) * 0.5,
      (Ball._HEIGHT - 145) * 0.5,
      "preloaderBg"
    );
    this.preloadBar = this.add.sprite(
      (Ball._WIDTH - 158) * 0.5,
      (Ball._HEIGHT - 50) * 0.5,
      "preloaderBar"
    );
    this.load.setPreloadSprite(this.preloadBar);

    this.load.image("ball", "img/ball.png");
    this.load.image("hole", "img/hole.png");
    this.load.image("element-w", "img/element-horizontal.png");
    this.load.image("element-h", "img/element-vertical.png");
    this.load.spritesheet("button-start", "img/button-start.png", 146, 51);
    this.load.audio("audio-bounce", [
      "audio/bounce.ogg",
      "audio/bounce.mp3",
      "audio/bounce.m4a",
    ]);
  },
  create() {
    this.game.state.start("MainMenu");
  },
};
```
主菜单状态

显示带有开始按钮的标题屏幕。```javascript
Ball.MainMenu = function (game) {};
Ball.MainMenu.prototype = {
  create() {
    this.add.sprite(0, 0, "screen-mainmenu");
    this.gameTitle = this.add.sprite(Ball._WIDTH * 0.5, 40, "title");
    this.gameTitle.anchor.set(0.5, 0);

    this.startButton = this.add.button(
      Ball._WIDTH * 0.5, 200, "button-start",
      this.startGame, this,
      2, 0, 1  // hover, out, down frames
    );
    this.startButton.anchor.set(0.5, 0);
    this.startButton.input.useHandCursor = true;
  },
  startGame() {
    this.game.state.start("Howto");
  },
};
```
###如何状态

在游戏开始前的一键指示屏幕。```javascript
Ball.Howto = function (game) {};
Ball.Howto.prototype = {
  create() {
    this.buttonContinue = this.add.button(
      0, 0, "screen-howtoplay",
      this.startGame, this
    );
  },
  startGame() {
    this.game.state.start("Game");
  },
};
```
---

设备定向API使用

设备朝向API提供有关设备物理倾斜的实时数据。使用两个轴：

|属性|轴|范围|效果||----------|------|-------|--------|
|`event.gamma`|Left/right倾斜| -90至90度|水平球速度|
|`event.beta`|Front/back倾斜| -180至180度|垂直球速度|

注册监听器```javascript
// In the Game state's create() method
window.addEventListener("deviceorientation", this.handleOrientation);
```
处理朝向事件```javascript
handleOrientation(e) {
  const x = e.gamma; // left-right tilt
  const y = e.beta;  // front-back tilt
  Ball._player.body.velocity.x += x;
  Ball._player.body.velocity.y += y;
}
```
倾斜行为

-倾斜装置左：负伽马，球滚左
-向右倾斜装置：正伽马，球向右滚动
-向前倾斜设备：正beta，球滚下来
-向后倾斜设备：负beta，球卷起来

倾斜角度直接映射到速度增量——倾斜越陡，每帧施加在球上的力就越大。

---

核心游戏机制

游戏状态结构```javascript
Ball.Game = function (game) {};
Ball.Game.prototype = {
  create() {},
  initLevels() {},
  showLevel(level) {},
  updateCounter() {},
  managePause() {},
  manageAudio() {},
  update() {},
  wallCollision() {},
  handleOrientation(e) {},
  finishLevel() {},
};
```
球的创造和物理```javascript
// In create()
this.ball = this.add.sprite(this.ballStartPos.x, this.ballStartPos.y, "ball");
this.ball.anchor.set(0.5);
this.physics.enable(this.ball, Phaser.Physics.ARCADE);
this.ball.body.setSize(18, 18);
this.ball.body.bounce.set(0.3, 0.3);
```
-锚定在中心`(0.5, 0.5)`，围绕中点旋转
-物理体：18x18像素
弹跳系数：0.3（撞墙后保持30%的速度）

键盘控制（桌面回退）```javascript
// In create()
this.keys = this.game.input.keyboard.createCursorKeys();

// In update()
if (this.keys.left.isDown) {
  this.ball.body.velocity.x -= this.movementForce;
} else if (this.keys.right.isDown) {
  this.ball.body.velocity.x += this.movementForce;
}
if (this.keys.up.isDown) {
  this.ball.body.velocity.y -= this.movementForce;
} else if (this.keys.down.isDown) {
  this.ball.body.velocity.y += this.movementForce;
}
```
###洞（球门）设置```javascript
this.hole = this.add.sprite(Ball._WIDTH * 0.5, 90, "hole");
this.physics.enable(this.hole, Phaser.Physics.ARCADE);
this.hole.anchor.set(0.5);
this.hole.body.setSize(2, 2);
```
该孔有一个微小的2x2碰撞体，用于精确的重叠检测。

---

##等级系统

###关卡数据格式

每一层都是一个包含位置和类型的墙段对象数组：```javascript
this.levelData = [
  [{ x: 96, y: 224, t: "w" }],                           // Level 1
  [
    { x: 72, y: 320, t: "w" },
    { x: 200, y: 320, t: "h" },
    { x: 72, y: 150, t: "w" },
  ],                                                       // Level 2
  // ... more levels
];
```
—`x, y`：以像素为单位的位置
-`t`：型号——`"w"`适用于水平墙，`"h"`适用于垂直墙

###建造关卡```javascript
initLevels() {
  for (let i = 0; i < this.maxLevels; i++) {
    const newLevel = this.add.group();
    newLevel.enableBody = true;
    newLevel.physicsBodyType = Phaser.Physics.ARCADE;

    for (const item of this.levelData[i]) {
      newLevel.create(item.x, item.y, `element-${item.t}`);
    }

    newLevel.setAll("body.immovable", true);
    newLevel.visible = false;
    this.levels.push(newLevel);
  }
}
```
显示关卡```javascript
showLevel(level) {
  const lvl = level || this.level;
  if (this.levels[lvl - 2]) {
    this.levels[lvl - 2].visible = false;
  }
  this.levels[lvl - 1].visible = true;
}
```
---

##碰撞检测

墙壁碰撞（弹跳）```javascript
// In update()
this.physics.arcade.collide(
  this.ball, this.borderGroup,
  this.wallCollision, null, this
);
this.physics.arcade.collide(
  this.ball, this.levels[this.level - 1],
  this.wallCollision, null, this
);
```
`collide`导致球弹到墙上并触发回调。

孔重叠（直通检测）```javascript
this.physics.arcade.overlap(
  this.ball, this.hole,
  this.finishLevel, null, this
);
```
`overlap`检测无物理碰撞响应的交叉。

墙碰撞回调```javascript
wallCollision() {
  if (this.audioStatus) {
    this.bounceSound.play();
  }
  if ("vibrate" in window.navigator) {
    window.navigator.vibrate(100);
  }
}
```
---

##音频系统```javascript
// In create()
this.bounceSound = this.game.add.audio("audio-bounce");

// Toggle
manageAudio() {
  this.audioStatus = !this.audioStatus;
}
```
---

##振动API```javascript
if ("vibrate" in window.navigator) {
  window.navigator.vibrate(100); // 100ms vibration pulse
}
```
在调用之前进行特征检测。在支持的移动设备上提供触觉反馈。

---

##定时系统```javascript
// In create()
this.timer = 0;
this.totalTimer = 0;
this.timerText = this.game.add.text(15, 15, "Time: 0", this.fontBig);
this.totalTimeText = this.game.add.text(120, 30, "Total time: 0", this.fontSmall);
this.time.events.loop(Phaser.Timer.SECOND, this.updateCounter, this);

// Counter callback
updateCounter() {
  this.timer++;
  this.timerText.setText(`Time: ${this.timer}`);
  this.totalTimeText.setText(`Total time: ${this.totalTimer + this.timer}`);
}
```
---

##完成关卡```javascript
finishLevel() {
  if (this.level >= this.maxLevels) {
    this.totalTimer += this.timer;
    alert(`Congratulations, game completed!\nTotal time: ${this.totalTimer}s`);
    this.game.state.start("MainMenu");
  } else {
    alert(`Level ${this.level} completed!`);
    this.totalTimer += this.timer;
    this.timer = 0;
    this.level++;
    this.timerText.setText(`Time: ${this.timer}`);
    this.totalTimeText.setText(`Total time: ${this.totalTimer}`);
    this.levelText.setText(`Level: ${this.level} / ${this.maxLevels}`);
    this.ball.body.x = this.ballStartPos.x;
    this.ball.body.y = this.ballStartPos.y;
    this.ball.body.velocity.x = 0;
    this.ball.body.velocity.y = 0;
    this.showLevel();
  }
}
```
---

完成更新循环```javascript
update() {
  // Keyboard input
  if (this.keys.left.isDown) {
    this.ball.body.velocity.x -= this.movementForce;
  } else if (this.keys.right.isDown) {
    this.ball.body.velocity.x += this.movementForce;
  }
  if (this.keys.up.isDown) {
    this.ball.body.velocity.y -= this.movementForce;
  } else if (this.keys.down.isDown) {
    this.ball.body.velocity.y += this.movementForce;
  }

  // Wall collisions
  this.physics.arcade.collide(
    this.ball, this.borderGroup, this.wallCollision, null, this
  );
  this.physics.arcade.collide(
    this.ball, this.levels[this.level - 1], this.wallCollision, null, this
  );

  // Hole overlap
  this.physics.arcade.overlap(
    this.ball, this.hole, this.finishLevel, null, this
  );
}
```
---

相位器API快速参考

|功能|用途||----------|---------|
|`this.add.sprite(x, y, key)`|创建一个游戏对象|
|`this.add.group()`|为对象|创建容器
|`this.add.button(x, y, key, cb, ctx, over, out, down)`|创建交互式按钮|
|`this.add.text(x, y, text, style)`|创建文本显示|
|`this.physics.enable(obj, system)`|使物理对象|
|`this.physics.arcade.collide(a, b, cb)`|检测碰撞与反弹|
|`this.physics.arcade.overlap(a, b, cb)`|检测重叠无反弹|
|`this.load.image(key, path)`|加载图像资产|
|`this.load.spritesheet(key, path, w, h)`|加载精灵动画表|
|`this.load.audio(key, paths[])`|加载音频与格式回落|
|`this.game.add.audio(key)`|实例化音频对象|
|`this.time.events.loop(interval, cb, ctx)`|创建重复定时器|