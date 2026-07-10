# 2D平台游戏模板

使用Phaser （v2）构建2D平台游戏的完整分步指南。x / Phaser CE)与街机物理。该模板涵盖了开发的每个阶段：设置项目，根据JSON关卡数据创建平台，添加基于物理的移动和跳跃的英雄，可收集的硬币，行走的敌人，死亡和踩踏机制，计分板，精灵动画，带有door/key系统的获胜条件以及多层次进程。

**这是一款经典的横向卷轴平台游戏，主角在其中导航平台，收集硬币，避开或踩死蜘蛛敌人，找到钥匙打开门，并在多个关卡中前进——包括分数跟踪，动画和物理。**先决条件：**基本到中级JavaScript知识，熟悉HTML，以及用于开发的本地web服务器（例如，浏览器同步，live-server或Python的SimpleHTTPServer）。

**来源：**基于[Mozilla HTML5 Games Workshop - Platformer]（https://mozdevs.github.io/html5-games-workshop/en/guides/platformer/start-here/）。在研讨会存储库中可以获得项目启动器文件。

---

##从这里开始

本教程使用Phaser框架构建一个2D平台。相位器处理渲染，物理，输入，音频和资产加载，所以你可以专注于游戏逻辑。

你将创造什么

完成后的游戏特点：-玩家用键盘控制的英雄角色
-英雄可以在平台上行走和跳跃
-可收集的硬币，增加得分
-行走的蜘蛛敌人会杀死英雄（但可以从上面踩）
-钥匙和门系统：英雄必须拿起钥匙才能打开门并完成关卡
-从JSON数据文件加载多个级别
-记分牌显示收集到的硬币
英雄的精灵动画（空闲，奔跑，跳跃，坠落）

项目结构```
project/
  index.html
  js/
    phaser.min.js        (Phaser 2.6.2 or Phaser CE)
    main.js              (all game code goes here)
  audio/
    sfx/
      jump.wav
      coin.wav
      stomp.wav
      key.wav
      door.wav
  images/
    background.png
    ground.png
    grass:8x1.png        (platform tile images in various sizes)
    grass:6x1.png
    grass:4x1.png
    grass:2x1.png
    grass:1x1.png
    hero.png             (hero spritesheet: 36x42 per frame)
    hero_stopped.png     (single frame for initial steps)
    coin_animated.png    (coin spritesheet)
    spider.png           (spider spritesheet)
    invisible_wall.png   (invisible boundary for enemy AI)
    key.png              (key spritesheet)
    door.png             (door spritesheet)
    key_icon.png         (HUD icon for key)
    font:numbers.png     (bitmap font for score)
  data/
    level00.json
    level01.json
```
###关卡数据格式

每个级别都在JSON文件中定义。JSON结构描述了每个实体的位置：```json
{
    "hero": { "x": 21, "y": 525 },
    "door": { "x": 169, "y": 546 },
    "key": { "x": 750, "y": 524 },
    "platforms": [
        { "image": "ground", "x": 0, "y": 546 },
        { "image": "grass:8x1", "x": 208, "y": 420 },
        { "image": "grass:4x1", "x": 420, "y": 336 },
        { "image": "grass:2x1", "x": 680, "y": 252 }
    ],
    "coins": [
        { "x": 147, "y": 525 },
        { "x": 189, "y": 525 },
        { "x": 399, "y": 399 },
        { "x": 441, "y": 336 }
    ],
    "spiders": [
        { "x": 121, "y": 399 }
    ],
    "decoration": {
        "grass": [
            { "x": 84, "y": 504, "frame": 0 },
            { "x": 420, "y": 504, "frame": 1 }
        ]
    }
}
```
每个实体类型（英雄、门、钥匙、平台、硬币、蜘蛛）都有`x`和`y`坐标。平台还指定要为该平台贴图使用哪个`image`资产。

---

初始化相位器

第一步是设置HTML文件并创建《Phaser》游戏实例。

HTML入口点

创建一个`index.html`文件来加载Phaser和你的游戏脚本：```html
<!doctype html>
<html lang="en">
<head>
    <meta charset="utf-8">
    <title>Platformer Game</title>
    <style>
        html, body {
            margin: 0;
            padding: 0;
            background: #000;
        }
    </style>
    <script src="js/phaser.min.js"></script>
    <script src="js/main.js"></script>
</head>
<body>
    <div id="game"></div>
</body>
</html>
```
-`<div id="game">`是容器，相位器将插入游戏画布。
-首先加载相位器，然后加载游戏脚本。

创建游戏实例

在`js/main.js`中，创建Phaser游戏对象并注册游戏状态：```javascript
// Create a Phaser game instance
// Parameters: width, height, renderer, DOM element ID
window.onload = function () {
    let game = new Phaser.Game(960, 600, Phaser.AUTO, 'game');

    // Add and start the play state
    game.state.add('play', PlayState);
    game.state.start('play');
};
```
-`960, 600`设置游戏画布的像素大小。`Phaser.AUTO`让Phaser自动选择WebGL和Canvas渲染。
-`'game'`是包含画布的DOM元素的ID。

PlayState对象

将游戏状态定义为具有生命周期方法的对象：```javascript
PlayState = {};

PlayState.init = function () {
    // Called first when the state starts
};

PlayState.preload = function () {
    // Load all assets here
};

PlayState.create = function () {
    // Create game entities and set up the world
};

PlayState.update = function () {
    // Called every frame (~60 times per second)
    // Handle game logic, input, collisions here
};
```
—`init`——先运行；用于配置和接收参数。
-`preload`——用于在游戏开始前加载所有资产（图像，音频，JSON）。
-`create`-在资源加载后调用一次；用于创建精灵、群组和游戏对象。
-`update`-每帧调用~60fps；用于输入处理，物理检查和游戏逻辑。

此时，您应该看到页面上呈现了一个空白的黑色画布。

---

##游戏循环

Phaser使用了游戏循环架构。每一帧，Phaser调用`update()`，这是你处理输入、移动精灵和检查碰撞的地方。在循环开始之前，`preload()`加载资源，`create()`设置初始游戏状态。

加载和显示背景

首先加载并显示背景图像以验证游戏循环是否有效：```javascript
PlayState.preload = function () {
    this.game.load.image('background', 'images/background.png');
};

PlayState.create = function () {
    // Add the background image at position (0, 0)
    this.game.add.image(0, 0, 'background');
};
```
-`this.game.load.image(key, path)`加载映像并为其分配一个键供以后参考。
-`this.game.add.image(x, y, key)`在给定位置创建静态图像。

现在你应该看到游戏画布中呈现的背景图像。

了解帧周期```
preload() -> [assets loaded] -> create() -> update() -> update() -> update() -> ...
```
对`update()`的每次调用表示一帧。游戏的目标是每秒60帧。所有的移动、输入读取和碰撞检测都发生在`update()`内部。

---

##创建平台

平台是英雄行走和跳跃的表面。它们是从关卡JSON数据中加载的，并以一组物理精灵的形式创建。

加载平台资产

在`preload`中加载关卡JSON数据和所有平台平铺图像：```javascript
PlayState.preload = function () {
    this.game.load.image('background', 'images/background.png');

    // Load level data
    this.game.load.json('level:1', 'data/level01.json');

    // Load platform images
    this.game.load.image('ground', 'images/ground.png');
    this.game.load.image('grass:8x1', 'images/grass_8x1.png');
    this.game.load.image('grass:6x1', 'images/grass_6x1.png');
    this.game.load.image('grass:4x1', 'images/grass_4x1.png');
    this.game.load.image('grass:2x1', 'images/grass_2x1.png');
    this.game.load.image('grass:1x1', 'images/grass_1x1.png');
};
```
从关卡数据中生成平台

创建一种方法来加载关卡并将每个平台作为物理组中的精灵生成：```javascript
PlayState.create = function () {
    // Add the background
    this.game.add.image(0, 0, 'background');

    // Load level data and spawn entities
    this._loadLevel(this.game.cache.getJSON('level:1'));
};

PlayState._loadLevel = function (data) {
    // Create a group for platforms
    this.platforms = this.game.add.group();

    // Spawn each platform from the level data
    data.platforms.forEach(this._spawnPlatform, this);
};

PlayState._spawnPlatform = function (platform) {
    // Add a sprite at the platform's position using the specified image
    let sprite = this.platforms.create(platform.x, platform.y, platform.image);

    // Enable physics on this platform
    this.game.physics.enable(sprite);

    // Make platform immovable so it doesn't get pushed by the hero
    sprite.body.allowGravity = false;
    sprite.body.immovable = true;
};
```
-`this.game.add.group()`创建一个相位组-相关精灵的容器，使批处理操作和碰撞检测。
-`this.platforms.create(x, y, key)`在组内创建一个雪碧。
-`sprite.body.immovable = true`防止平台被其他物理实体推动
-`sprite.body.allowGravity = false`防止平台因重力而下落。

现在你应该看到地面和草地平台瓷砖呈现在屏幕上。

---

主角精灵

现在添加玩家将控制的英雄角色。

###加载英雄图像

将英雄图像添加到`preload`。最初，我们使用单个静态图像；稍后我们将切换到精灵表来制作动画：```javascript
// In PlayState.preload:
this.game.load.image('hero', 'images/hero_stopped.png');
```
生成英雄

将英雄添加到`_loadLevel`并创建一个spawn方法：```javascript
PlayState._loadLevel = function (data) {
    this.platforms = this.game.add.group();
    data.platforms.forEach(this._spawnPlatform, this);

    // Spawn the hero at the position defined in level data
    this._spawnCharacters({ hero: data.hero });
};

PlayState._spawnCharacters = function (data) {
    // Create the hero sprite
    this.hero = this.game.add.sprite(data.hero.x, data.hero.y, 'hero');

    // Set the anchor to the bottom-center for easier positioning
    this.hero.anchor.set(0.5, 1);
};
```
-`anchor.set(0.5, 1)`设置精灵的原点为水平中心和垂直底部。这样可以更容易地将英雄放置在平台的顶部，因为`y`的位置指的是英雄的脚，而不是左上角。

---

##键盘控制

捕捉键盘输入，这样玩家就可以左右移动英雄和跳跃。

设置输入键

在`init`中，配置键盘控件：```javascript
PlayState.init = function () {
    // Force integer rendering for pixel-art crispness
    this.game.renderer.renderSession.roundPixels = true;

    // Capture arrow keys
    this.keys = this.game.input.keyboard.addKeys({
        left: Phaser.KeyCode.LEFT,
        right: Phaser.KeyCode.RIGHT,
        up: Phaser.KeyCode.UP
    });
};
```
-`addKeys()`捕获指定的键并返回一个带有键状态引用的对象。
—`Phaser.KeyCode.LEFT`、`RIGHT`、`UP`对应方向键。
-`renderSession.roundPixels = true`防止像素美术精灵由于亚像素渲染而显得模糊。

在Update中读取输入

处理`update`中的关键状态。现在，只需记录方向；下一步添加基于物理的运动：```javascript
PlayState.update = function () {
    this._handleInput();
};

PlayState._handleInput = function () {
    if (this.keys.left.isDown) {
        // Move hero left
    } else if (this.keys.right.isDown) {
        // Move hero right
    } else {
        // Stop (no key held)
    }
};
```
-`this.keys.left.isDown`在按住左箭头键时返回`true`。`else`子句处理既不按左也不按右的情况（英雄应该停止）。

---

##用物理移动精灵

启用街机物理，这样英雄就可以快速移动，并通过碰撞与平台互动。

###启用物理引擎

在`init`中启用街机物理：```javascript
PlayState.init = function () {
    this.game.renderer.renderSession.roundPixels = true;

    this.keys = this.game.input.keyboard.addKeys({
        left: Phaser.KeyCode.LEFT,
        right: Phaser.KeyCode.RIGHT,
        up: Phaser.KeyCode.UP
    });

    // Enable Arcade Physics
    this.game.physics.startSystem(Phaser.Physics.ARCADE);
};
```
为英雄添加物理体

在`_spawnCharacters`中启用英雄精灵的物理：```javascript
PlayState._spawnCharacters = function (data) {
    this.hero = this.game.add.sprite(data.hero.x, data.hero.y, 'hero');
    this.hero.anchor.set(0.5, 1);

    // Enable physics body on the hero
    this.game.physics.enable(this.hero);
};
```
###移动速度

现在更新`_handleInput`来设置英雄的速度基于按键：```javascript
const SPEED = 200; // pixels per second

PlayState._handleInput = function () {
    if (this.keys.left.isDown) {
        this.hero.body.velocity.x = -SPEED;
    } else if (this.keys.right.isDown) {
        this.hero.body.velocity.x = SPEED;
    } else {
        this.hero.body.velocity.x = 0;
    }
};
```
-`body.velocity.x`设置水平速度，单位为像素每秒。
负值会使精灵向左移动；积极使它正确。
-当没有按键时，将速度设为`0`，英雄会立即停止。

英雄现在可以左右移动，但会从平台上掉下来，离开屏幕，因为还没有重力或碰撞处理。

---

# #重力

添加重力，让英雄向下坠落并与平台相撞。

设置全局重力

在`init`中为整个物理世界启用重力：```javascript
PlayState.init = function () {
    this.game.renderer.renderSession.roundPixels = true;

    this.keys = this.game.input.keyboard.addKeys({
        left: Phaser.KeyCode.LEFT,
        right: Phaser.KeyCode.RIGHT,
        up: Phaser.KeyCode.UP
    });

    this.game.physics.startSystem(Phaser.Physics.ARCADE);

    // Set global gravity
    this.game.physics.arcade.gravity.y = 1200;
};
```
-`gravity.y = 1200`应用1200像素每秒平方的向下加速到所有启用物理的精灵（除非他们选择退出`allowGravity = false`）。

英雄和平台之间的碰撞检测

在`update`中增加碰撞检测，这样英雄就会落在平台上而不是掉下去```javascript
PlayState.update = function () {
    this._handleCollisions();
    this._handleInput();
};

PlayState._handleCollisions = function () {
    // Make the hero collide with the platform group
    this.game.physics.arcade.collide(this.hero, this.platforms);
};
```
`arcade.collide(spriteA, groupB)`检查英雄和平台组中所有精灵之间的物理碰撞当英雄降落在一个平台上时，物理引擎会阻止它通过并解决重叠问题。
在调用`_handleInput()`之前调用`_handleCollisions()`是很重要的，这样当我们处理输入时，碰撞数据（比如英雄是否触地）是最新的。

英雄现在由于重力而下落并落在平台上。你可以在站台上左右行走。

---

# #跳跃

允许英雄在按下向上箭头键时跳跃——但仅限于站在平台上时（不能在半空中跳跃）。

执行跳跃机制

添加跳跃常数并更新`_handleInput`：```javascript
const SPEED = 200;
const JUMP_SPEED = 600;

PlayState._handleInput = function () {
    if (this.keys.left.isDown) {
        this.hero.body.velocity.x = -SPEED;
    } else if (this.keys.right.isDown) {
        this.hero.body.velocity.x = SPEED;
    } else {
        this.hero.body.velocity.x = 0;
    }

    // Handle jumping
    if (this.keys.up.isDown) {
        this._jump();
    }
};

PlayState._jump = function () {
    let canJump = this.hero.body.touching.down;

    if (canJump) {
        this.hero.body.velocity.y = -JUMP_SPEED;
    }

    return canJump;
};
```
`this.hero.body.touching.down`是`true`，当英雄的物理身体碰到另一个身体的底部时，意思是英雄站在什么东西上。
-将`velocity.y`设置为负值将使英雄向上（y轴在屏幕坐标中指向向下）。`canJump`检查防止英雄在空中跳跃，强制执行单跳行为。
-该方法返回是否执行了跳跃，这在稍后播放声音效果时很有用。

添加跳跃音效

加载跳跃声音并在成功跳跃时播放：```javascript
// In PlayState.preload:
this.game.load.audio('sfx:jump', 'audio/sfx/jump.wav');

// In PlayState.create:
this.sfx = {
    jump: this.game.add.audio('sfx:jump')
};

// In PlayState._jump, after setting velocity:
PlayState._jump = function () {
    let canJump = this.hero.body.touching.down;

    if (canJump) {
        this.hero.body.velocity.y = -JUMP_SPEED;
        this.sfx.jump.play();
    }

    return canJump;
};
```
---

##可拾取硬币

添加可收集的钱币，玩家可以捡起来提高分数。

加载硬币资产

在`preload`中加载硬币精灵表和硬币音效：```javascript
// In PlayState.preload:
this.game.load.spritesheet('coin', 'images/coin_animated.png', 22, 22);
this.game.load.audio('sfx:coin', 'audio/sfx/coin.wav');
```
-`load.spritesheet(key, path, frameWidth, frameHeight)`加载精灵表并将其切片为22x22像素的单独帧用于动画。

从关卡数据中生成金币

更新`_loadLevel`以创建一个硬币组并生成每个硬币：```javascript
PlayState._loadLevel = function (data) {
    this.platforms = this.game.add.group();
    this.coins = this.game.add.group();

    data.platforms.forEach(this._spawnPlatform, this);
    data.coins.forEach(this._spawnCoin, this);

    this._spawnCharacters({ hero: data.hero });
};

PlayState._spawnCoin = function (coin) {
    let sprite = this.coins.create(coin.x, coin.y, 'coin');
    sprite.anchor.set(0.5, 0.5);

    // Add a tween animation to make the coin bob up and down
    this.game.physics.enable(sprite);
    sprite.body.allowGravity = false;

    // Coin bobbing animation with a tween
    sprite.animations.add('rotate', [0, 1, 2, 1], 6, true); // 6fps, looping
    sprite.animations.play('rotate');
};
```
-每个硬币都在`coins`组内创建，以便于碰撞检测。
-`allowGravity = false`防止硬币掉落。
-`animations.add`创建一个帧动画使用精灵表帧0,1,2,1在6fps，连续循环。

收集硬币

将硬币的声音添加到sfx对象中，并检测英雄和硬币之间的重叠：```javascript
// In PlayState.create, add to the sfx object:
this.sfx = {
    jump: this.game.add.audio('sfx:jump'),
    coin: this.game.add.audio('sfx:coin')
};

// In PlayState._handleCollisions:
PlayState._handleCollisions = function () {
    this.game.physics.arcade.collide(this.hero, this.platforms);

    // Detect overlap between hero and coins (no physical collision, just overlap)
    this.game.physics.arcade.overlap(
        this.hero, this.coins, this._onHeroVsCoin, null, this
    );
};

PlayState._onHeroVsCoin = function (hero, coin) {
    this.sfx.coin.play();
    coin.kill();  // Remove the coin from the game
    this.coinPickupCount++;
};
```
-`arcade.overlap()`检查两个sprites/groups是否重叠而不解决物理冲突。当检测到重叠时，它调用回调函数（`_onHeroVsCoin`）。
-`coin.kill()`从游戏世界中移除硬币精灵。
-`this.coinPickupCount`跟踪收集的硬币数量（在`_loadLevel`中初始化）。

初始化硬币计数器```javascript
PlayState._loadLevel = function (data) {
    this.platforms = this.game.add.group();
    this.coins = this.game.add.group();

    data.platforms.forEach(this._spawnPlatform, this);
    data.coins.forEach(this._spawnCoin, this);

    this._spawnCharacters({ hero: data.hero });

    // Initialize coin counter
    this.coinPickupCount = 0;
};
```
---

行走的敌人

添加在平台上来回行走的蜘蛛敌人。英雄可以从上面踩他们，但是如果从侧面碰到他们就会死亡。

加载敌人资产```javascript
// In PlayState.preload:
this.game.load.spritesheet('spider', 'images/spider.png', 42, 32);
this.game.load.image('invisible-wall', 'images/invisible_wall.png');
this.game.load.audio('sfx:stomp', 'audio/sfx/stomp.wav');
```
蜘蛛精灵表有一个爬行动画的帧。
在平台边缘放置隐形墙，以防止蜘蛛走掉——它们没有视觉渲染，但有物理实体。

生成敌人

更新`_loadLevel`并为蜘蛛添加一个产卵方法：```javascript
PlayState._loadLevel = function (data) {
    this.platforms = this.game.add.group();
    this.coins = this.game.add.group();
    this.spiders = this.game.add.group();
    this.enemyWalls = this.game.add.group();

    data.platforms.forEach(this._spawnPlatform, this);
    data.coins.forEach(this._spawnCoin, this);
    data.spiders.forEach(this._spawnSpider, this);

    this._spawnCharacters({ hero: data.hero });

    // Make enemy walls invisible
    this.enemyWalls.visible = false;

    this.coinPickupCount = 0;
};
```
在平台上创造隐形墙

修改`_spawnPlatform`，在每个平台的两边添加隐形墙：```javascript
PlayState._spawnPlatform = function (platform) {
    let sprite = this.platforms.create(platform.x, platform.y, platform.image);
    this.game.physics.enable(sprite);
    sprite.body.allowGravity = false;
    sprite.body.immovable = true;

    // Spawn invisible walls at the left and right edges of this platform
    this._spawnEnemyWall(platform.x, platform.y, 'left');
    this._spawnEnemyWall(platform.x + sprite.width, platform.y, 'right');
};

PlayState._spawnEnemyWall = function (x, y, side) {
    let sprite = this.enemyWalls.create(x, y, 'invisible-wall');

    // Anchor to the bottom of the wall and adjust position based on side
    sprite.anchor.set(side === 'left' ? 1 : 0, 1);

    this.game.physics.enable(sprite);
    sprite.body.immovable = true;
    sprite.body.allowGravity = false;
};
```
每个平台都有两堵看不见的墙，两边各有一面。
-墙壁起到屏障的作用，防止蜘蛛从边缘走下来。
-锚定，使墙壁对准平台的正确一侧。

生成和动画蜘蛛```javascript
PlayState._spawnSpider = function (spider) {
    let sprite = this.spiders.create(spider.x, spider.y, 'spider');
    sprite.anchor.set(0.5, 1);

    // Add the crawl animation
    sprite.animations.add('crawl', [0, 1, 2], 8, true);
    sprite.animations.add('die', [0, 4, 0, 4, 0, 4, 3, 3, 3, 3, 3, 3], 12);
    sprite.animations.play('crawl');

    // Enable physics
    this.game.physics.enable(sprite);

    // Set initial movement speed
    sprite.body.velocity.x = Spider.SPEED;
};

// Spider speed constant
const Spider = { SPEED: 100 };
```
蜘蛛有两个动画：`crawl`（循环）和`die`（死亡时播放一次）。
-`velocity.x = 100`以每秒100像素的速度启动蜘蛛向右移动。

让蜘蛛在墙上弹跳

添加碰撞处理，使蜘蛛在击中不可见的墙壁或平台边缘时逆转方向：```javascript
// In PlayState._handleCollisions:
PlayState._handleCollisions = function () {
    this.game.physics.arcade.collide(this.hero, this.platforms);
    this.game.physics.arcade.collide(this.spiders, this.platforms);
    this.game.physics.arcade.collide(this.spiders, this.enemyWalls);

    this.game.physics.arcade.overlap(
        this.hero, this.coins, this._onHeroVsCoin, null, this
    );
    this.game.physics.arcade.overlap(
        this.hero, this.spiders, this._onHeroVsEnemy, null, this
    );
};
```
为了让蜘蛛在与墙壁碰撞时改变方向，每帧检查它们的速度并翻转它们：```javascript
// In PlayState.update, after collision handling, update spider directions:
PlayState.update = function () {
    this._handleCollisions();
    this._handleInput();

    // Update spider facing direction based on velocity
    this.spiders.forEach(function (spider) {
        if (spider.body.touching.right || spider.body.blocked.right) {
            spider.body.velocity.x = -Spider.SPEED; // Turn left
        } else if (spider.body.touching.left || spider.body.blocked.left) {
            spider.body.velocity.x = Spider.SPEED; // Turn right
        }
    }, this);
};
```
当蜘蛛碰到它右边的墙时，它会向左转，反之亦然。
-`body.touching`是在碰撞解决后由相位器设置的。

---

# #死亡

实现英雄在接触敌人时死亡和杀死敌人时的踩踏机制。

英雄vs敌人：跺脚或死亡

当英雄与蜘蛛重叠时，检查英雄是否在坠落（跺脚）：```javascript
PlayState._onHeroVsEnemy = function (hero, enemy) {
    if (hero.body.velocity.y > 0) {
        // Hero is falling -> stomp the enemy
        enemy.body.velocity.x = 0; // Stop enemy movement
        enemy.body.enable = false; // Disable enemy physics

        // Play die animation then remove the enemy
        enemy.animations.play('die');
        enemy.events.onAnimationComplete.addOnce(function () {
            enemy.kill();
        });

        // Bounce the hero up after stomping
        hero.body.velocity.y = -JUMP_SPEED / 2;

        this.sfx.stomp.play();
    } else {
        // Hero touched enemy from side or below -> die
        this._killHero();
    }
};

PlayState._killHero = function () {
    this.hero.kill();
    // Restart the level after a short delay
    this.game.time.events.add(500, function () {
        this.game.state.restart(true, false, { level: this.level });
    }, this);
};
```
—如果是`hero.body.velocity.y > 0`，表示英雄正在向下移动（落下），表示被踩。
-踩：敌人停止，播放死亡动画，并被移除。男主角跳了起来。
-如果英雄没有下落，英雄就会死亡。`this.hero.kill()`从游戏中移除英雄。
- 500ms后，整个状态重新启动，有效地重新加载关卡。

添加跺脚声音```javascript
// In PlayState.create, add to sfx:
this.sfx = {
    jump: this.game.add.audio('sfx:jump'),
    coin: this.game.add.audio('sfx:coin'),
    stomp: this.game.add.audio('sfx:stomp')
};
```
为英雄添加死亡动画

使英雄在死亡时闪光并从屏幕上掉下来：```javascript
PlayState._killHero = function () {
    this.hero.alive = false;

    // Play a "dying" visual: the hero jumps up and falls off screen
    this.hero.body.velocity.y = -JUMP_SPEED / 2;
    this.hero.body.velocity.x = 0;
    this.hero.body.allowGravity = true;

    // Disable collisions so the hero falls through platforms
    this.hero.body.collideWorldBounds = false;

    // Restart after a delay
    this.game.time.events.add(1000, function () {
        this.game.state.restart(true, false, { level: this.level });
    }, this);
};
```
死亡时保护输入

防止输入控制英雄死后：```javascript
PlayState._handleInput = function () {
    if (!this.hero.alive) { return; }

    if (this.keys.left.isDown) {
        this.hero.body.velocity.x = -SPEED;
    } else if (this.keys.right.isDown) {
        this.hero.body.velocity.x = SPEED;
    } else {
        this.hero.body.velocity.x = 0;
    }

    if (this.keys.up.isDown) {
        this._jump();
    }
};
```
-`this.hero.alive`在`_killHero`中被设置为`false`，因此在英雄死亡后输入将被忽略，并且英雄自然地从屏幕上消失。

---

# #记分板

使用位图字体在屏幕上显示收集到的硬币数量。

加载位图字体```javascript
// In PlayState.preload:
this.game.load.image('font:numbers', 'images/numbers.png');
this.game.load.image('icon:coin', 'images/coin_icon.png');
```
创建HUD

创建一个固定的HUD（平视显示器），显示硬币图标和计数：```javascript
PlayState._createHud = function () {
    let coinIcon = this.game.make.image(0, 0, 'icon:coin');

    // Create a dynamic text label for the coin count
    this.hud = this.game.add.group();

    // Use a retroFont or a regular text object for the score
    let scoreStyle = {
        font: '30px monospace',
        fill: '#fff'
    };
    this.coinFont = this.game.add.text(
        coinIcon.width + 7, 0, 'x0', scoreStyle
    );

    this.hud.add(coinIcon);
    this.hud.add(this.coinFont);

    this.hud.position.set(10, 10);
    this.hud.fixedToCamera = true;
};
```
或者，使用相位器的`RetroFont`进行像素级数字渲染：```javascript
PlayState._createHud = function () {
    // Bitmap-based number rendering using RetroFont
    this.coinFont = this.game.add.retroFont(
        'font:numbers', 20, 26,
        '0123456789X ', 6
    );

    let coinIcon = this.game.make.image(0, 0, 'icon:coin');

    let coinScoreImg = this.game.make.image(
        coinIcon.x + coinIcon.width + 7, 0, this.coinFont
    );

    this.hud = this.game.add.group();
    this.hud.add(coinIcon);
    this.hud.add(coinScoreImg);
    this.hud.position.set(10, 10);
    this.hud.fixedToCamera = true;
};
```
-`retroFont`从包含字符字形的精灵表创建位图字体。
-参数：图像键，字符宽度，字符高度，字符集字符串，每行字符数。

在create中调用createHud```javascript
PlayState.create = function () {
    this.game.add.image(0, 0, 'background');

    this._loadLevel(this.game.cache.getJSON('level:1'));

    // Create the HUD
    this._createHud();
};
```
更新分数显示

每当收集到硬币时更新分数文本：```javascript
PlayState._onHeroVsCoin = function (hero, coin) {
    this.sfx.coin.play();
    coin.kill();
    this.coinPickupCount++;

    // Update the HUD
    this.coinFont.text = 'x' + this.coinPickupCount;
};
```
---

##主角动画

用精灵表替换静态英雄图像，并添加不同状态的动画：空闲（停止）、奔跑、跳跃和坠落。

加载英雄精灵表

用`preload`中的精灵表替换单个图像加载：```javascript
// Replace: this.game.load.image('hero', 'images/hero_stopped.png');
// With:
this.game.load.spritesheet('hero', 'images/hero.png', 36, 42);
```
-英雄精灵表是每帧36像素宽42像素高
-帧包括空闲，步行周期，跳跃和跌倒姿势。

定义动画

在`_spawnCharacters`中，在创建英雄精灵后添加动画定义：```javascript
PlayState._spawnCharacters = function (data) {
    this.hero = this.game.add.sprite(data.hero.x, data.hero.y, 'hero');
    this.hero.anchor.set(0.5, 1);
    this.game.physics.enable(this.hero);

    // Define animations
    this.hero.animations.add('stop', [0]);               // Single frame: idle
    this.hero.animations.add('run', [1, 2], 8, true);    // 2 frames at 8fps, looping
    this.hero.animations.add('jump', [3]);                // Single frame: jumping up
    this.hero.animations.add('fall', [4]);                // Single frame: falling down
};
```
-`animations.add(name, frames, fps, loop)`以给定的名称注册动画。
-单帧动画，如`stop`，`jump`，和`fall`有效地设置静态姿势。
-`run`动画在第1帧和第2帧之间以8fps交替。

播放正确的动画

添加一个方法，根据英雄的当前状态判断并播放正确的动画：```javascript
PlayState._getAnimationName = function () {
    let name = 'stop'; // Default: standing still

    if (!this.hero.alive) {
        name = 'stop'; // Use idle frame when dead
    } else if (this.hero.body.velocity.y < 0) {
        name = 'jump'; // Moving upward
    } else if (this.hero.body.velocity.y > 0 && !this.hero.body.touching.down) {
        name = 'fall'; // Moving downward and not on ground
    } else if (this.hero.body.velocity.x !== 0 && this.hero.body.touching.down) {
        name = 'run';  // Moving horizontally on the ground
    }

    return name;
};
```
根据方向翻转精灵

更新英雄的面向方向，并在`update`播放动画：```javascript
PlayState.update = function () {
    this._handleCollisions();
    this._handleInput();

    // Flip sprite based on movement direction
    if (this.hero.body.velocity.x < 0) {
        this.hero.scale.x = -1; // Face left
    } else if (this.hero.body.velocity.x > 0) {
        this.hero.scale.x = 1;  // Face right
    }

    // Play the appropriate animation
    this.hero.animations.play(this._getAnimationName());

    // Update spider directions
    this.spiders.forEach(function (spider) {
        if (spider.body.touching.right || spider.body.blocked.right) {
            spider.body.velocity.x = -Spider.SPEED;
        } else if (spider.body.touching.left || spider.body.blocked.left) {
            spider.body.velocity.x = Spider.SPEED;
        }
    }, this);
};
```
-`this.hero.scale.x = -1`将雪碧水平地向左翻转。设置为`1`向右。因为锚点在`(0.5, 1)`，翻转看起来很自然。
-`animations.play()`只在名称改变时重启动画，所以每帧调用它是安全有效的。

---

##获胜条件

添加门和钥匙机制：英雄必须收集钥匙，然后到达门处完成关卡。

装载门和钥匙资产```javascript
// In PlayState.preload:
this.game.load.spritesheet('door', 'images/door.png', 42, 66);
this.game.load.spritesheet('key', 'images/key.png', 20, 22);  // Key bobbing animation
this.game.load.image('icon:key', 'images/key_icon.png');

this.game.load.audio('sfx:key', 'audio/sfx/key.wav');
this.game.load.audio('sfx:door', 'audio/sfx/door.wav');
```
刷出门和钥匙

更新`_loadLevel`和`_spawnCharacters`：```javascript
PlayState._loadLevel = function (data) {
    this.platforms = this.game.add.group();
    this.coins = this.game.add.group();
    this.spiders = this.game.add.group();
    this.enemyWalls = this.game.add.group();
    this.bgDecoration = this.game.add.group();

    // Must spawn decorations first (background layer)
    // Spawn door before hero so it renders behind the hero
    data.platforms.forEach(this._spawnPlatform, this);
    data.coins.forEach(this._spawnCoin, this);
    data.spiders.forEach(this._spawnSpider, this);

    this._spawnDoor(data.door.x, data.door.y);
    this._spawnKey(data.key.x, data.key.y);
    this._spawnCharacters({ hero: data.hero });

    this.enemyWalls.visible = false;

    this.coinPickupCount = 0;
    this.hasKey = false;
};

PlayState._spawnDoor = function (x, y) {
    this.door = this.bgDecoration.create(x, y, 'door');
    this.door.anchor.setTo(0.5, 1);

    this.game.physics.enable(this.door);
    this.door.body.allowGravity = false;
};

PlayState._spawnKey = function (x, y) {
    this.key = this.bgDecoration.create(x, y, 'key');
    this.key.anchor.set(0.5, 0.5);

    this.game.physics.enable(this.key);
    this.key.body.allowGravity = false;

    // Add a bobbing up-and-down tween to the key
    this.key.y -= 3;
    this.game.add.tween(this.key)
        .to({ y: this.key.y + 6 }, 800, Phaser.Easing.Sinusoidal.InOut)
        .yoyo(true)
        .loop()
        .start();
};
```
-门被放置在背景装饰组中，因此它呈现在英雄的后面。
-键有一个正弦振荡间，移动它6像素上下超过800ms，循环永远。

收集钥匙，打开门

向sfx对象添加钥匙和门的声音效果：```javascript
// In PlayState.create sfx:
this.sfx = {
    jump: this.game.add.audio('sfx:jump'),
    coin: this.game.add.audio('sfx:coin'),
    stomp: this.game.add.audio('sfx:stomp'),
    key: this.game.add.audio('sfx:key'),
    door: this.game.add.audio('sfx:door')
};
```
在`_handleCollisions`中增加钥匙和门的重叠检测：```javascript
PlayState._handleCollisions = function () {
    this.game.physics.arcade.collide(this.hero, this.platforms);
    this.game.physics.arcade.collide(this.spiders, this.platforms);
    this.game.physics.arcade.collide(this.spiders, this.enemyWalls);

    this.game.physics.arcade.overlap(
        this.hero, this.coins, this._onHeroVsCoin, null, this
    );
    this.game.physics.arcade.overlap(
        this.hero, this.spiders, this._onHeroVsEnemy, null, this
    );
    this.game.physics.arcade.overlap(
        this.hero, this.key, this._onHeroVsKey, null, this
    );
    this.game.physics.arcade.overlap(
        this.hero, this.door, this._onHeroVsDoor,
        // Only trigger if the hero has the key
        function (hero, door) {
            return this.hasKey && hero.body.touching.down;
        }, this
    );
};
```
-门重叠有一个进程回调（第四个参数），只有当`this.hasKey`为真并且英雄站在某物上时才会触发重叠回调。这可以防止英雄在摔倒或没有钥匙的情况下进入大门。

钥匙和门的回调```javascript
PlayState._onHeroVsKey = function (hero, key) {
    this.sfx.key.play();
    key.kill();
    this.hasKey = true;
};

PlayState._onHeroVsDoor = function (hero, door) {
    this.sfx.door.play();

    // Freeze the hero and play the door opening animation
    hero.body.velocity.x = 0;
    hero.body.velocity.y = 0;
    hero.body.enable = false;

    // Play door open animation (transition from closed to open frame)
    door.frame = 1; // Switch to "open" frame

    // Advance to the next level after a short delay
    this.game.time.events.add(500, this._goToNextLevel, this);
};

PlayState._goToNextLevel = function () {
    this.camera.fade('#000');
    this.camera.onFadeComplete.addOnce(function () {
        this.game.state.restart(true, false, {
            level: this.level + 1
        });
    }, this);
};
```
—当英雄触摸该键时，该键将被移除，并将`hasKey`设置为`true`。
-当英雄到达门（带着钥匙）时，英雄会冻结，门会打开，一段时间后游戏会切换到下一关。
-`camera.fade()`创建一个渐变到黑色的过渡抛光电平开关。

在HUD中显示键图标

更新`_createHud`显示英雄是否收集到钥匙：```javascript
PlayState._createHud = function () {
    this.keyIcon = this.game.make.image(0, 19, 'icon:key');
    this.keyIcon.anchor.set(0, 0.5);

    // ... existing coin HUD code ...

    this.hud.add(this.keyIcon);
    this.hud.add(coinIcon);
    this.hud.add(coinScoreImg);
    this.hud.position.set(10, 10);
    this.hud.fixedToCamera = true;
};
```
更新`update`中每帧的关键图标外观：```javascript
// In PlayState.update, add:
this.keyIcon.frame = this.hasKey ? 1 : 0;
```
-第0帧显示一个灰色的键图标；帧1显示了收集到的密钥图标。

---

##切换电平

通过基于级别索引加载不同的JSON文件来支持多个级别。

###通过init传递级别号

修改`init`以接受级别参数：```javascript
PlayState.init = function (data) {
    this.game.renderer.renderSession.roundPixels = true;

    this.keys = this.game.input.keyboard.addKeys({
        left: Phaser.KeyCode.LEFT,
        right: Phaser.KeyCode.RIGHT,
        up: Phaser.KeyCode.UP
    });

    this.game.physics.startSystem(Phaser.Physics.ARCADE);
    this.game.physics.arcade.gravity.y = 1200;

    // Store the current level number (default to 0)
    this.level = (data.level || 0) % LEVEL_COUNT;
};

const LEVEL_COUNT = 2; // Total number of levels
```
—`data`是从`game.state.start()`或`game.state.restart()`传入的对象。
-模操作（`% LEVEL_COUNT`）在最后一关之后环绕到0级，创建一个无限循环的关卡。

动态加载关卡数据

更新`preload`以加载基于`this.level`的正确关卡：```javascript
PlayState.preload = function () {
    this.game.load.image('background', 'images/background.png');

    // Load the current level's JSON data
    this.game.load.json('level:0', 'data/level00.json');
    this.game.load.json('level:1', 'data/level01.json');

    // ... load all other assets ...
};
```
更新`create`以使用正确的关卡数据：```javascript
PlayState.create = function () {
    this.sfx = {
        jump: this.game.add.audio('sfx:jump'),
        coin: this.game.add.audio('sfx:coin'),
        stomp: this.game.add.audio('sfx:stomp'),
        key: this.game.add.audio('sfx:key'),
        door: this.game.add.audio('sfx:door')
    };

    this.game.add.image(0, 0, 'background');

    // Load level data based on current level number
    this._loadLevel(this.game.cache.getJSON('level:' + this.level));

    this._createHud();
};
```
从0级开始游戏

更新初始状态start通过level 0：```javascript
window.onload = function () {
    let game = new Phaser.Game(960, 600, Phaser.AUTO, 'game');
    game.state.add('play', PlayState);
    game.state.start('play', true, false, { level: 0 });
};
```
—第三和第四个`start`参数控制world/cache的清除。`true, false`在重启之间保留缓存（因此不需要重新加载资产），但清除世界。
—`{ level: 0 }`作为`data`参数传递给`init`。

关卡转换流程

完整的关卡流程为：

1. 英雄收集键->`hasKey = true`2. 英雄到达门口->`_onHeroVsDoor`开火
3. 摄像机逐渐变黑->`_goToNextLevel`开火
4. State以`{ level: this.level + 1 }`重启
5.`init`接收到新的级别号
6. 正确的关卡JSON被加载，游戏继续

---

##前进

恭喜你——你已经创造了一款完整的2D平台游戏。以下是进一步扩展游戏的想法：

建议的改进- **移动/触摸控制：**添加屏幕上的按钮或滑动手势使用`game.input.onDown`触控设备。
- **更多的水平：**创建额外的JSON水平文件与新的平台布局，硬币的位置，和敌人的配置。
- **菜单屏幕：**添加一个`MenuState`与标题屏幕和开始按钮之前进入`PlayState`。
- **游戏结束屏幕：**而不是立即重新启动，显示一个“游戏结束”屏幕与得分。
**生命系统：**给英雄多次生命而不是立即重启。
- **能量提升：**添加像速度提升，双跳，或无敌的项目。
- **移动平台：**创建沿着路径使用渐变的平台。
- **不同的敌人类型：**添加飞行的敌人，敌人射击投射物，或敌人与不同的运动模式。
- **视差滚动：**添加多个背景层，以不同的速度滚动深度。
- * *凸轮**对于比屏幕宽的关卡，使用`game.camera.follow(this.hero)`与英雄一起滚动。
- **声音和音乐：**添加背景音乐和额外的音效，以获得更抛光的体验。
- **粒子效果：**使用相位器的粒子发射器硬币收集火花，敌人死亡的效果，或灰尘着陆时。完整的游戏源代码参考

下面是包含所有步骤的完整`main.js`文件，以供参考。这代表了带有所有功能的游戏的最终状态：```javascript
// =============================================================================
// Constants
// =============================================================================

const SPEED = 200;
const JUMP_SPEED = 600;
const LEVEL_COUNT = 2;
const Spider = { SPEED: 100 };

// =============================================================================
// Game State: PlayState
// =============================================================================

PlayState = {};

// -----------------------------------------------------------------------------
// init
// -----------------------------------------------------------------------------

PlayState.init = function (data) {
    this.game.renderer.renderSession.roundPixels = true;

    this.keys = this.game.input.keyboard.addKeys({
        left: Phaser.KeyCode.LEFT,
        right: Phaser.KeyCode.RIGHT,
        up: Phaser.KeyCode.UP
    });

    this.game.physics.startSystem(Phaser.Physics.ARCADE);
    this.game.physics.arcade.gravity.y = 1200;

    this.level = (data.level || 0) % LEVEL_COUNT;
};

// -----------------------------------------------------------------------------
// preload
// -----------------------------------------------------------------------------

PlayState.preload = function () {
    // Background
    this.game.load.image('background', 'images/background.png');

    // Level data
    this.game.load.json('level:0', 'data/level00.json');
    this.game.load.json('level:1', 'data/level01.json');

    // Platform tiles
    this.game.load.image('ground', 'images/ground.png');
    this.game.load.image('grass:8x1', 'images/grass_8x1.png');
    this.game.load.image('grass:6x1', 'images/grass_6x1.png');
    this.game.load.image('grass:4x1', 'images/grass_4x1.png');
    this.game.load.image('grass:2x1', 'images/grass_2x1.png');
    this.game.load.image('grass:1x1', 'images/grass_1x1.png');

    // Characters
    this.game.load.spritesheet('hero', 'images/hero.png', 36, 42);
    this.game.load.spritesheet('spider', 'images/spider.png', 42, 32);
    this.game.load.image('invisible-wall', 'images/invisible_wall.png');

    // Collectibles
    this.game.load.spritesheet('coin', 'images/coin_animated.png', 22, 22);
    this.game.load.spritesheet('key', 'images/key.png', 20, 22);
    this.game.load.spritesheet('door', 'images/door.png', 42, 66);

    // HUD
    this.game.load.image('icon:coin', 'images/coin_icon.png');
    this.game.load.image('icon:key', 'images/key_icon.png');
    this.game.load.image('font:numbers', 'images/numbers.png');

    // Audio
    this.game.load.audio('sfx:jump', 'audio/sfx/jump.wav');
    this.game.load.audio('sfx:coin', 'audio/sfx/coin.wav');
    this.game.load.audio('sfx:stomp', 'audio/sfx/stomp.wav');
    this.game.load.audio('sfx:key', 'audio/sfx/key.wav');
    this.game.load.audio('sfx:door', 'audio/sfx/door.wav');
};

// -----------------------------------------------------------------------------
// create
// -----------------------------------------------------------------------------

PlayState.create = function () {
    // Sound effects
    this.sfx = {
        jump: this.game.add.audio('sfx:jump'),
        coin: this.game.add.audio('sfx:coin'),
        stomp: this.game.add.audio('sfx:stomp'),
        key: this.game.add.audio('sfx:key'),
        door: this.game.add.audio('sfx:door')
    };

    // Background
    this.game.add.image(0, 0, 'background');

    // Load level
    this._loadLevel(this.game.cache.getJSON('level:' + this.level));

    // HUD
    this._createHud();
};

// -----------------------------------------------------------------------------
// update
// -----------------------------------------------------------------------------

PlayState.update = function () {
    this._handleCollisions();
    this._handleInput();

    // Update hero sprite direction and animation
    if (this.hero.body.velocity.x < 0) {
        this.hero.scale.x = -1;
    } else if (this.hero.body.velocity.x > 0) {
        this.hero.scale.x = 1;
    }
    this.hero.animations.play(this._getAnimationName());

    // Update spider directions when hitting walls
    this.spiders.forEach(function (spider) {
        if (spider.body.touching.right || spider.body.blocked.right) {
            spider.body.velocity.x = -Spider.SPEED;
        } else if (spider.body.touching.left || spider.body.blocked.left) {
            spider.body.velocity.x = Spider.SPEED;
        }
    }, this);

    // Update key icon in HUD
    this.keyIcon.frame = this.hasKey ? 1 : 0;
};

// -----------------------------------------------------------------------------
// Level Loading
// -----------------------------------------------------------------------------

PlayState._loadLevel = function (data) {
    // Create groups (order matters for rendering layers)
    this.bgDecoration = this.game.add.group();
    this.platforms = this.game.add.group();
    this.coins = this.game.add.group();
    this.spiders = this.game.add.group();
    this.enemyWalls = this.game.add.group();

    // Spawn entities from level data
    data.platforms.forEach(this._spawnPlatform, this);
    data.coins.forEach(this._spawnCoin, this);
    data.spiders.forEach(this._spawnSpider, this);

    this._spawnDoor(data.door.x, data.door.y);
    this._spawnKey(data.key.x, data.key.y);
    this._spawnCharacters({ hero: data.hero });

    // Hide invisible walls
    this.enemyWalls.visible = false;

    // Initialize game state
    this.coinPickupCount = 0;
    this.hasKey = false;
};

// -----------------------------------------------------------------------------
// Spawn Methods
// -----------------------------------------------------------------------------

PlayState._spawnPlatform = function (platform) {
    let sprite = this.platforms.create(platform.x, platform.y, platform.image);
    this.game.physics.enable(sprite);
    sprite.body.allowGravity = false;
    sprite.body.immovable = true;

    // Add invisible walls at both edges for enemy AI
    this._spawnEnemyWall(platform.x, platform.y, 'left');
    this._spawnEnemyWall(platform.x + sprite.width, platform.y, 'right');
};

PlayState._spawnEnemyWall = function (x, y, side) {
    let sprite = this.enemyWalls.create(x, y, 'invisible-wall');
    sprite.anchor.set(side === 'left' ? 1 : 0, 1);
    this.game.physics.enable(sprite);
    sprite.body.immovable = true;
    sprite.body.allowGravity = false;
};

PlayState._spawnCharacters = function (data) {
    this.hero = this.game.add.sprite(data.hero.x, data.hero.y, 'hero');
    this.hero.anchor.set(0.5, 1);
    this.game.physics.enable(this.hero);
    this.hero.body.collideWorldBounds = true;

    // Hero animations
    this.hero.animations.add('stop', [0]);
    this.hero.animations.add('run', [1, 2], 8, true);
    this.hero.animations.add('jump', [3]);
    this.hero.animations.add('fall', [4]);
};

PlayState._spawnCoin = function (coin) {
    let sprite = this.coins.create(coin.x, coin.y, 'coin');
    sprite.anchor.set(0.5, 0.5);
    this.game.physics.enable(sprite);
    sprite.body.allowGravity = false;

    sprite.animations.add('rotate', [0, 1, 2, 1], 6, true);
    sprite.animations.play('rotate');
};

PlayState._spawnSpider = function (spider) {
    let sprite = this.spiders.create(spider.x, spider.y, 'spider');
    sprite.anchor.set(0.5, 1);
    this.game.physics.enable(sprite);

    sprite.animations.add('crawl', [0, 1, 2], 8, true);
    sprite.animations.add('die', [0, 4, 0, 4, 0, 4, 3, 3, 3, 3, 3, 3], 12);
    sprite.animations.play('crawl');

    sprite.body.velocity.x = Spider.SPEED;
};

PlayState._spawnDoor = function (x, y) {
    this.door = this.bgDecoration.create(x, y, 'door');
    this.door.anchor.setTo(0.5, 1);
    this.game.physics.enable(this.door);
    this.door.body.allowGravity = false;
};

PlayState._spawnKey = function (x, y) {
    this.key = this.bgDecoration.create(x, y, 'key');
    this.key.anchor.set(0.5, 0.5);
    this.game.physics.enable(this.key);
    this.key.body.allowGravity = false;

    // Bobbing tween
    this.key.y -= 3;
    this.game.add.tween(this.key)
        .to({ y: this.key.y + 6 }, 800, Phaser.Easing.Sinusoidal.InOut)
        .yoyo(true)
        .loop()
        .start();
};

// -----------------------------------------------------------------------------
// Input
// -----------------------------------------------------------------------------

PlayState._handleInput = function () {
    if (!this.hero.alive) { return; }

    if (this.keys.left.isDown) {
        this.hero.body.velocity.x = -SPEED;
    } else if (this.keys.right.isDown) {
        this.hero.body.velocity.x = SPEED;
    } else {
        this.hero.body.velocity.x = 0;
    }

    if (this.keys.up.isDown) {
        this._jump();
    }
};

PlayState._jump = function () {
    let canJump = this.hero.body.touching.down;
    if (canJump) {
        this.hero.body.velocity.y = -JUMP_SPEED;
        this.sfx.jump.play();
    }
    return canJump;
};

// -----------------------------------------------------------------------------
// Collisions
// -----------------------------------------------------------------------------

PlayState._handleCollisions = function () {
    // Physical collisions
    this.game.physics.arcade.collide(this.hero, this.platforms);
    this.game.physics.arcade.collide(this.spiders, this.platforms);
    this.game.physics.arcade.collide(this.spiders, this.enemyWalls);

    // Overlap detection (no physical push)
    this.game.physics.arcade.overlap(
        this.hero, this.coins, this._onHeroVsCoin, null, this
    );
    this.game.physics.arcade.overlap(
        this.hero, this.spiders, this._onHeroVsEnemy, null, this
    );
    this.game.physics.arcade.overlap(
        this.hero, this.key, this._onHeroVsKey, null, this
    );
    this.game.physics.arcade.overlap(
        this.hero, this.door, this._onHeroVsDoor,
        function (hero, door) {
            return this.hasKey && hero.body.touching.down;
        }, this
    );
};

// -----------------------------------------------------------------------------
// Collision Callbacks
// -----------------------------------------------------------------------------

PlayState._onHeroVsCoin = function (hero, coin) {
    this.sfx.coin.play();
    coin.kill();
    this.coinPickupCount++;
    this.coinFont.text = 'x' + this.coinPickupCount;
};

PlayState._onHeroVsEnemy = function (hero, enemy) {
    if (hero.body.velocity.y > 0) {
        // Stomp: hero is falling onto the enemy
        enemy.body.velocity.x = 0;
        enemy.body.enable = false;
        enemy.animations.play('die');
        enemy.events.onAnimationComplete.addOnce(function () {
            enemy.kill();
        });
        hero.body.velocity.y = -JUMP_SPEED / 2;
        this.sfx.stomp.play();
    } else {
        // Hero dies
        this._killHero();
    }
};

PlayState._onHeroVsKey = function (hero, key) {
    this.sfx.key.play();
    key.kill();
    this.hasKey = true;
};

PlayState._onHeroVsDoor = function (hero, door) {
    this.sfx.door.play();
    hero.body.velocity.x = 0;
    hero.body.velocity.y = 0;
    hero.body.enable = false;

    door.frame = 1; // Open door

    this.game.time.events.add(500, this._goToNextLevel, this);
};

// -----------------------------------------------------------------------------
// Death and Level Transitions
// -----------------------------------------------------------------------------

PlayState._killHero = function () {
    this.hero.alive = false;
    this.hero.body.velocity.y = -JUMP_SPEED / 2;
    this.hero.body.velocity.x = 0;
    this.hero.body.allowGravity = true;
    this.hero.body.collideWorldBounds = false;

    this.game.time.events.add(1000, function () {
        this.game.state.restart(true, false, { level: this.level });
    }, this);
};

PlayState._goToNextLevel = function () {
    this.camera.fade('#000');
    this.camera.onFadeComplete.addOnce(function () {
        this.game.state.restart(true, false, {
            level: this.level + 1
        });
    }, this);
};

// -----------------------------------------------------------------------------
// Animations
// -----------------------------------------------------------------------------

PlayState._getAnimationName = function () {
    let name = 'stop';

    if (!this.hero.alive) {
        name = 'stop';
    } else if (this.hero.body.velocity.y < 0) {
        name = 'jump';
    } else if (this.hero.body.velocity.y > 0 && !this.hero.body.touching.down) {
        name = 'fall';
    } else if (this.hero.body.velocity.x !== 0 && this.hero.body.touching.down) {
        name = 'run';
    }

    return name;
};

// -----------------------------------------------------------------------------
// HUD
// -----------------------------------------------------------------------------

PlayState._createHud = function () {
    this.keyIcon = this.game.make.image(0, 19, 'icon:key');
    this.keyIcon.anchor.set(0, 0.5);

    let coinIcon = this.game.make.image(
        this.keyIcon.width + 7, 0, 'icon:coin'
    );

    let scoreStyle = { font: '24px monospace', fill: '#fff' };
    this.coinFont = this.game.add.text(
        coinIcon.x + coinIcon.width + 7, 0, 'x0', scoreStyle
    );

    this.hud = this.game.add.group();
    this.hud.add(this.keyIcon);
    this.hud.add(coinIcon);
    this.hud.add(this.coinFont);
    this.hud.position.set(10, 10);
    this.hud.fixedToCamera = true;
};

// =============================================================================
// Entry Point
// =============================================================================

window.onload = function () {
    let game = new Phaser.Game(960, 600, Phaser.AUTO, 'game');
    game.state.add('play', PlayState);
    game.state.start('play', true, false, { level: 0 });
};
```
关键概念总结

|相位器API |用途||---------|-----------|---------|
|游戏实例|`new Phaser.Game(w, h, renderer, container)`|创建游戏画布和引擎|
|游戏状态|`game.state.add()`/`game.state.start()`|组织代码到init/preload/create/update生命周期|
|加载图片|`game.load.image(key, path)`|加载静态图片资源|
|加载精灵表|`game.load.spritesheet(key, path, fw, fh)`|加载动画精灵表|
|加载JSON |`game.load.json(key, path)`|加载JSON数据（级别定义）|
|加载音频|`game.load.audio(key, path)`|加载音效|
|精灵组|`game.add.group()`|相关精灵的容器；启用批量冲突检测|
|物理体|`game.physics.enable(sprite)`|为精灵添加街机物理体|
|重力|`game.physics.arcade.gravity.y`|全球向下加速度|
|碰撞|`arcade.collide(a, b)`|物理碰撞分辨率（精灵相互推动）|
|重叠|`arcade.overlap(a, b, callback)`|检测无物理推动（拾取）|
|速度|`sprite.body.velocity.x/y`|以像素每秒为单位的移动速度|
|不可移动|`sprite.body.immovable = true`|防止精灵被碰撞推动|
|生命tions |`sprite.animations.add(name, frames, fps, loop)`|定义帧动画|
|渐变|`game.add.tween(target).to(props, duration, easing)`|平滑属性动画|
|键盘输入|`game.input.keyboard.addKeys({...})`|捕获特定键盘键|
|摄像机|`this.camera.fade()`|屏幕过渡效果|
|锚|`sprite.anchor.set(x, y)`|设置定位和旋转|的原点
|精灵翻转|`sprite.scale.x = -1`|水平镜像精灵|