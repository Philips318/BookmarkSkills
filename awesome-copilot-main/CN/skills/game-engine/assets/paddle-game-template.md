#划桨游戏模板（2D Breakout）

使用纯JavaScript和HTML5 Canvas API构建2D Breakout游戏的完整分步指南。该模板涵盖了开发的每个阶段，从设置画布到执行生命系统和优化游戏循环。

**一个经典的breakout/paddle游戏，玩家控制一个桨来弹起一个球并摧毁一块砖，带有分数跟踪，win/lose条件，键盘和鼠标控制，以及生命系统。

**先决条件：**基本到中级JavaScript知识和熟悉HTML。

**来源：**基于【MDN 2D Breakout游戏教程】（https://developer.mozilla.org/en-US/docs/Games/Tutorials/2D_Breakout_game_pure_JavaScript）。

---

步骤1：创建画布并在其上绘制

第一步是使用`<canvas>`元素设置HTML文档，并学习使用2D呈现上下文绘制基本形状。

### HTML结构用一个嵌入的canvas元素创建你的基本HTML文件：```html
<!doctype html>
<html lang="en-US">
  <head>
    <meta charset="utf-8" />
    <title>Gamedev Canvas Workshop</title>
    <style>
      * {
        padding: 0;
        margin: 0;
      }
      canvas {
        background: #eeeeee;
        display: block;
        margin: 0 auto;
      }
    </style>
  </head>
  <body>
    <canvas id="myCanvas" width="480" height="320"></canvas>

    <script>
      // JavaScript code goes here
    </script>
  </body>
</html>
```
获取画布参考和2D上下文

canvas元素提供了一个绘图表面。你可以通过2D渲染上下文访问它：```javascript
const canvas = document.getElementById("myCanvas");
const ctx = canvas.getContext("2d");
```
-`canvas`是对HTML`<canvas>`元素的引用。`ctx`是2D渲染上下文对象，它提供了所有的绘图方法。

绘制一个填充矩形

用`rect()`定义一个矩形，用`fill()`渲染它：```javascript
ctx.beginPath();
ctx.rect(20, 40, 50, 50);
ctx.fillStyle = "red";
ctx.fill();
ctx.closePath();
```
—前两个参数（`20, 40`）设置左上角坐标。
—后两个参数`50, 50`设置宽度和高度。
-`fillStyle`设置填充颜色。
-`fill()`将形状渲染为实体填充。

画一个圆

使用`arc()`定义一个圆：```javascript
ctx.beginPath();
ctx.arc(240, 160, 20, 0, Math.PI * 2, false);
ctx.fillStyle = "green";
ctx.fill();
ctx.closePath();
```
-`240, 160`——中心x， y坐标。
-`20`——半径。
-`0`—起始角（弧度）。
-`Math.PI * 2`——端角（整圆）。
-`false`——顺时针绘制。

绘制描边矩形（仅限轮廓）

使用`stroke()`代替`fill()`的轮廓，`strokeStyle`的轮廓颜色：```javascript
ctx.beginPath();
ctx.rect(160, 10, 100, 40);
ctx.strokeStyle = "rgb(0 0 255 / 50%)";
ctx.stroke();
ctx.closePath();
```
-使用具有50% alpha透明度的RGB颜色。
-`stroke()`只绘制轮廓，而不是实体填充。

关键方法参考

|方法|目的||--------|---------|
|`beginPath()`|启动新的绘图路径|
|`closePath()`|关闭当前路径|
定义一个矩形|
定义圆或弧|
设置填充颜色|
填充形状，填充颜色为|
设置描边（轮廓）颜色|
绘制形状|的轮廓

完成步骤1的代码```html
<canvas id="myCanvas" width="480" height="320"></canvas>

<style>
  * { padding: 0; margin: 0; }
  canvas { background: #eeeeee; display: block; margin: 0 auto; }
</style>

<script>
  const canvas = document.getElementById("myCanvas");
  const ctx = canvas.getContext("2d");

  // Filled red square
  ctx.beginPath();
  ctx.rect(20, 40, 50, 50);
  ctx.fillStyle = "red";
  ctx.fill();
  ctx.closePath();

  // Filled green circle
  ctx.beginPath();
  ctx.arc(240, 160, 20, 0, Math.PI * 2, false);
  ctx.fillStyle = "green";
  ctx.fill();
  ctx.closePath();

  // Stroked blue rectangle (semi-transparent)
  ctx.beginPath();
  ctx.rect(160, 10, 100, 40);
  ctx.strokeStyle = "rgb(0 0 255 / 50%)";
  ctx.stroke();
  ctx.closePath();
</script>
```
---

步骤2：移动球

现在我们通过创建一个游戏循环，在每一帧上重新绘制画布并使用速度变量更新球的位置，从而使球动画化。

创建绘制循环

定义一个使用`setInterval`重复执行的`draw()`函数：```javascript
function draw() {
  // drawing code
}
setInterval(draw, 10);
```
`setInterval(draw, 10)`每隔10毫秒调用`draw`函数，每秒创建大约100帧。

画球

在`draw()`函数内，在固定位置画一个球（圆）：```javascript
ctx.beginPath();
ctx.arc(50, 50, 10, 0, Math.PI * 2);
ctx.fillStyle = "#0095DD";
ctx.fill();
ctx.closePath();
```
添加位置变量

而不是硬编码的位置，使用变量，这样我们可以更新他们每一帧。将这些放在`draw()`函数之上：```javascript
let x = canvas.width / 2;
let y = canvas.height - 30;
```
这将从水平中心开始，靠近画布的底部。

添加速度变量

定义水平（`dx`）和垂直（`dy`）移动的速度和方向：```javascript
let dx = 2;
let dy = -2;
```
-`dx = 2`每帧向右移动2像素。
-`dy = -2`将球每帧向上移动2个像素（负y在画布上向上）。

每帧更新位置

在`draw()`函数的末尾添加位置更新：```javascript
x += dx;
y += dy;
```
###清除画布

没有清理，球就会留下痕迹。在每一帧的开始添加`clearRect()`：```javascript
ctx.clearRect(0, 0, canvas.width, canvas.height);
```
重构成一个单独的drawBall（）函数

为了使代码干净，易于维护，将球绘制逻辑分开：```javascript
function drawBall() {
  ctx.beginPath();
  ctx.arc(x, y, 10, 0, Math.PI * 2);
  ctx.fillStyle = "#0095DD";
  ctx.fill();
  ctx.closePath();
}
```
完成步骤2的代码```javascript
const canvas = document.getElementById("myCanvas");
const ctx = canvas.getContext("2d");

let x = canvas.width / 2;
let y = canvas.height - 30;
let dx = 2;
let dy = -2;

function drawBall() {
  ctx.beginPath();
  ctx.arc(x, y, 10, 0, Math.PI * 2);
  ctx.fillStyle = "#0095DD";
  ctx.fill();
  ctx.closePath();
}

function draw() {
  ctx.clearRect(0, 0, canvas.width, canvas.height);
  drawBall();
  x += dx;
  y += dy;
}

setInterval(draw, 10);
```
* *关键概念:* *
- **动画循环**:`setInterval(draw, 10)`连续重绘场景。
**位置变量**:`x`和`y`跟踪球的当前位置。
- **速度变量**:`dx`和`dy`决定每帧的移动。
- **画布清除**:`clearRect()`在绘制新帧之前删除前一帧。

---

##步骤3：弹开墙壁

我们添加了碰撞检测，这样球就会从画布边缘反弹而不是消失。

定义球半径

将球半径提取为一个命名常量，以便在碰撞计算中重用：```javascript
const ballRadius = 10;
```
更新`drawBall()`来使用这个变量：```javascript
function drawBall() {
  ctx.beginPath();
  ctx.arc(x, y, ballRadius, 0, Math.PI * 2);
  ctx.fillStyle = "#0095DD";
  ctx.fill();
  ctx.closePath();
}
```
基本墙体碰撞（无半径调整）

最简单的方法是检查下一个球的位置是否超出了画布的边界：```javascript
// Left and right walls
if (x + dx > canvas.width || x + dx < 0) {
  dx = -dx;
}

// Top and bottom walls
if (y + dy > canvas.height || y + dy < 0) {
  dy = -dy;
}
```
反转`dx`或`dy`（乘以-1）会改变球的方向。

改进碰撞（计算球半径）

最基本的版本是让球在弹跳之前下沉到墙的一半。为了解决这个问题，考虑球的半径：```javascript
// Left and right walls
if (x + dx > canvas.width - ballRadius || x + dx < ballRadius) {
  dx = -dx;
}

// Top and bottom walls
if (y + dy > canvas.height - ballRadius || y + dy < ballRadius) {
  dy = -dy;
}
```
碰撞检测条件

|墙|条件|动作||------|-----------|--------|
| **左** |`x + dx < ballRadius`|`dx = -dx`|
| **右** |`x + dx > canvas.width - ballRadius`|`dx = -dx`|
| **Top** |`y + dy < ballRadius`|`dy = -dy`|
| **底部** |`y + dy > canvas.height - ballRadius`|`dy = -dy`|

完成步骤3的代码```javascript
const canvas = document.getElementById("myCanvas");
const ctx = canvas.getContext("2d");
const ballRadius = 10;

let x = canvas.width / 2;
let y = canvas.height - 30;
let dx = 2;
let dy = -2;

function drawBall() {
  ctx.beginPath();
  ctx.arc(x, y, ballRadius, 0, Math.PI * 2);
  ctx.fillStyle = "#0095DD";
  ctx.fill();
  ctx.closePath();
}

function draw() {
  ctx.clearRect(0, 0, canvas.width, canvas.height);
  drawBall();

  // Collision detection - left and right walls
  if (x + dx > canvas.width - ballRadius || x + dx < ballRadius) {
    dx = -dx;
  }

  // Collision detection - top and bottom walls
  if (y + dy > canvas.height - ballRadius || y + dy < ballRadius) {
    dy = -dy;
  }

  x += dx;
  y += dy;
}

setInterval(draw, 10);
```
---

##步骤4：球拍和键盘控制

现在我们在屏幕底部添加一个玩家控制的桨，并连接键盘输入（left/right方向键）。

定义桨变量```javascript
const paddleHeight = 10;
const paddleWidth = 75;
let paddleX = (canvas.width - paddleWidth) / 2;
```
-`paddleHeight`和`paddleWidth`定义桨叶尺寸。
-`paddleX`开始桨水平居中。它是`let`，因为它会随着玩家移动而改变。

画桨

创建一个`drawPaddle()`函数。桨位于画布的最底部；```javascript
function drawPaddle() {
  ctx.beginPath();
  ctx.rect(paddleX, canvas.height - paddleHeight, paddleWidth, paddleHeight);
  ctx.fillStyle = "#0095DD";
  ctx.fill();
  ctx.closePath();
}
```
- y位置为`canvas.height - paddleHeight`，与底部边缘齐平。

键盘状态变量

跟踪当前方向键是否被按下：```javascript
let rightPressed = false;
let leftPressed = false;
```
按键事件监听器

注册`keydown`（按下键）和`keyup`（释放键）的处理程序：```javascript
document.addEventListener("keydown", keyDownHandler);
document.addEventListener("keyup", keyUpHandler);
```
键处理函数

根据按下或释放哪个键来设置布尔标志：```javascript
function keyDownHandler(e) {
  if (e.key === "Right" || e.key === "ArrowRight") {
    rightPressed = true;
  } else if (e.key === "Left" || e.key === "ArrowLeft") {
    leftPressed = true;
  }
}

function keyUpHandler(e) {
  if (e.key === "Right" || e.key === "ArrowRight") {
    rightPressed = false;
  } else if (e.key === "Left" || e.key === "ArrowLeft") {
    leftPressed = false;
  }
}
```
检查`"ArrowRight"`（现代浏览器）和`"Right"`（传统的IE/Edge）的兼容性。

划桨运动逻辑（带有边界检查）

将此添加到`draw()`函数中，以根据键状态移动桨，同时将其保持在画布范围内：```javascript
if (rightPressed) {
  paddleX = Math.min(paddleX + 7, canvas.width - paddleWidth);
} else if (leftPressed) {
  paddleX = Math.max(paddleX - 7, 0);
}
```
-桨每帧移动7像素。
-`Math.min`防止桨越过右边缘。
-`Math.max`防止它越过左边缘。

完成步骤4的代码```javascript
const canvas = document.getElementById("myCanvas");
const ctx = canvas.getContext("2d");
const ballRadius = 10;

let x = canvas.width / 2;
let y = canvas.height - 30;
let dx = 2;
let dy = -2;

const paddleHeight = 10;
const paddleWidth = 75;
let paddleX = (canvas.width - paddleWidth) / 2;

let rightPressed = false;
let leftPressed = false;

document.addEventListener("keydown", keyDownHandler);
document.addEventListener("keyup", keyUpHandler);

function keyDownHandler(e) {
  if (e.key === "Right" || e.key === "ArrowRight") {
    rightPressed = true;
  } else if (e.key === "Left" || e.key === "ArrowLeft") {
    leftPressed = true;
  }
}

function keyUpHandler(e) {
  if (e.key === "Right" || e.key === "ArrowRight") {
    rightPressed = false;
  } else if (e.key === "Left" || e.key === "ArrowLeft") {
    leftPressed = false;
  }
}

function drawBall() {
  ctx.beginPath();
  ctx.arc(x, y, ballRadius, 0, Math.PI * 2);
  ctx.fillStyle = "#0095DD";
  ctx.fill();
  ctx.closePath();
}

function drawPaddle() {
  ctx.beginPath();
  ctx.rect(paddleX, canvas.height - paddleHeight, paddleWidth, paddleHeight);
  ctx.fillStyle = "#0095DD";
  ctx.fill();
  ctx.closePath();
}

function draw() {
  ctx.clearRect(0, 0, canvas.width, canvas.height);
  drawBall();
  drawPaddle();

  if (x + dx > canvas.width - ballRadius || x + dx < ballRadius) {
    dx = -dx;
  }
  if (y + dy > canvas.height - ballRadius || y + dy < ballRadius) {
    dy = -dy;
  }

  if (rightPressed) {
    paddleX = Math.min(paddleX + 7, canvas.width - paddleWidth);
  } else if (leftPressed) {
    paddleX = Math.max(paddleX - 7, 0);
  }

  x += dx;
  y += dy;
}

setInterval(draw, 10);
```
---

步骤5：游戏结束

我们用实际的游戏逻辑替换了底壁反弹：球应该从球拍上反弹，但如果没有击中，游戏就结束了。

存储间隔引用

要在游戏结束时停止游戏循环，请存储间隔ID：```javascript
let interval = 0;
```
然后赋值`setInterval`的返回值：```javascript
interval = setInterval(draw, 10);
```
执行游戏结束和划桨碰撞

更换底壁碰撞检查。我们现在检查球是否击中或未击中球拍，而不是从底部边缘反弹：```javascript
if (y + dy < ballRadius) {
  // Ball hits top wall -- bounce
  dy = -dy;
} else if (y + dy > canvas.height - ballRadius) {
  // Ball reaches bottom edge
  if (x > paddleX && x < paddleX + paddleWidth) {
    // Ball hits paddle -- bounce
    dy = -dy;
  } else {
    // Ball missed the paddle -- game over
    alert("GAME OVER");
    document.location.reload();
    clearInterval(interval);
  }
}
```
**桨碰撞如何工作：**
-`x > paddleX`-球越过球拍的左边缘。
-`x < paddleX + paddleWidth`——球在球拍右边缘之前。
-如果两者都是正确的，球在球拍上方，所以它会反弹。
-如果球到达底部而没有击中球拍，游戏结束。

完成步骤5的代码```javascript
const canvas = document.getElementById("myCanvas");
const ctx = canvas.getContext("2d");
const ballRadius = 10;

let x = canvas.width / 2;
let y = canvas.height - 30;
let dx = 2;
let dy = -2;

const paddleHeight = 10;
const paddleWidth = 75;
let paddleX = (canvas.width - paddleWidth) / 2;

let rightPressed = false;
let leftPressed = false;
let interval = 0;

document.addEventListener("keydown", keyDownHandler);
document.addEventListener("keyup", keyUpHandler);

function keyDownHandler(e) {
  if (e.key === "Right" || e.key === "ArrowRight") {
    rightPressed = true;
  } else if (e.key === "Left" || e.key === "ArrowLeft") {
    leftPressed = true;
  }
}

function keyUpHandler(e) {
  if (e.key === "Right" || e.key === "ArrowRight") {
    rightPressed = false;
  } else if (e.key === "Left" || e.key === "ArrowLeft") {
    leftPressed = false;
  }
}

function drawBall() {
  ctx.beginPath();
  ctx.arc(x, y, ballRadius, 0, Math.PI * 2);
  ctx.fillStyle = "#0095DD";
  ctx.fill();
  ctx.closePath();
}

function drawPaddle() {
  ctx.beginPath();
  ctx.rect(paddleX, canvas.height - paddleHeight, paddleWidth, paddleHeight);
  ctx.fillStyle = "#0095DD";
  ctx.fill();
  ctx.closePath();
}

function draw() {
  ctx.clearRect(0, 0, canvas.width, canvas.height);
  drawBall();
  drawPaddle();

  // Left and right wall collision
  if (x + dx > canvas.width - ballRadius || x + dx < ballRadius) {
    dx = -dx;
  }

  // Top wall collision
  if (y + dy < ballRadius) {
    dy = -dy;
  } else if (y + dy > canvas.height - ballRadius) {
    // Bottom edge: paddle collision or game over
    if (x > paddleX && x < paddleX + paddleWidth) {
      dy = -dy;
    } else {
      alert("GAME OVER");
      document.location.reload();
      clearInterval(interval);
    }
  }

  // Paddle movement
  if (rightPressed) {
    paddleX = Math.min(paddleX + 7, canvas.width - paddleWidth);
  } else if (leftPressed) {
    paddleX = Math.max(paddleX - 7, 0);
  }

  x += dx;
  y += dy;
}

interval = setInterval(draw, 10);
```
---

##步骤6：建立砖场

现在我们创建球将摧毁的砖块网格。砖块存储在二维数组中，并以行和列的形式绘制。

砖配置变量

定义控制brick字段布局的常量：```javascript
const brickRowCount = 3;
const brickColumnCount = 5;
const brickWidth = 75;
const brickHeight = 20;
const brickPadding = 10;
const brickOffsetTop = 30;
const brickOffsetLeft = 30;
```
-`brickRowCount`/`brickColumnCount`—砖块的行数和列数。
-`brickWidth`/`brickHeight`——每块砖的尺寸。
-`brickPadding`—砖块之间的空间。
-`brickOffsetTop`/`brickOffsetLeft`—从顶部和左侧画布边缘到第一块砖的距离。

创建砖块2D数组

使用嵌套循环创建一个2D数组。每个砖块存储它的`x`和`y`位置（最初`0`，绘图时计算）：```javascript
const bricks = [];
for (let c = 0; c < brickColumnCount; c++) {
  bricks[c] = [];
  for (let r = 0; r < brickRowCount; r++) {
    bricks[c][r] = { x: 0, y: 0 };
  }
}
```
drawBricks（）函数

循环遍历每个砖块，计算它的位置，存储它，并绘制它：```javascript
function drawBricks() {
  for (let c = 0; c < brickColumnCount; c++) {
    for (let r = 0; r < brickRowCount; r++) {
      const brickX = c * (brickWidth + brickPadding) + brickOffsetLeft;
      const brickY = r * (brickHeight + brickPadding) + brickOffsetTop;
      bricks[c][r].x = brickX;
      bricks[c][r].y = brickY;
      ctx.beginPath();
      ctx.rect(brickX, brickY, brickWidth, brickHeight);
      ctx.fillStyle = "#0095DD";
      ctx.fill();
      ctx.closePath();
    }
  }
}
```
**位置计算公式：**
——`brickX = column * (brickWidth + brickPadding) + brickOffsetLeft`——`brickY = row * (brickHeight + brickPadding) + brickOffsetTop`这将创建具有一致填充和边距的均匀间隔网格。

在游戏循环中调用drawBricks（）

在清除画布后，在`draw()`函数的开头添加调用：```javascript
function draw() {
  ctx.clearRect(0, 0, canvas.width, canvas.height);
  drawBricks();
  drawBall();
  drawPaddle();
  // ... rest of draw function
}
```
完成步骤6的代码```javascript
const canvas = document.getElementById("myCanvas");
const ctx = canvas.getContext("2d");
const ballRadius = 10;

let x = canvas.width / 2;
let y = canvas.height - 30;
let dx = 2;
let dy = -2;

const paddleHeight = 10;
const paddleWidth = 75;
let paddleX = (canvas.width - paddleWidth) / 2;

let rightPressed = false;
let leftPressed = false;
let interval = 0;

const brickRowCount = 3;
const brickColumnCount = 5;
const brickWidth = 75;
const brickHeight = 20;
const brickPadding = 10;
const brickOffsetTop = 30;
const brickOffsetLeft = 30;

const bricks = [];
for (let c = 0; c < brickColumnCount; c++) {
  bricks[c] = [];
  for (let r = 0; r < brickRowCount; r++) {
    bricks[c][r] = { x: 0, y: 0 };
  }
}

document.addEventListener("keydown", keyDownHandler);
document.addEventListener("keyup", keyUpHandler);

function keyDownHandler(e) {
  if (e.key === "Right" || e.key === "ArrowRight") {
    rightPressed = true;
  } else if (e.key === "Left" || e.key === "ArrowLeft") {
    leftPressed = true;
  }
}

function keyUpHandler(e) {
  if (e.key === "Right" || e.key === "ArrowRight") {
    rightPressed = false;
  } else if (e.key === "Left" || e.key === "ArrowLeft") {
    leftPressed = false;
  }
}

function drawBall() {
  ctx.beginPath();
  ctx.arc(x, y, ballRadius, 0, Math.PI * 2);
  ctx.fillStyle = "#0095DD";
  ctx.fill();
  ctx.closePath();
}

function drawPaddle() {
  ctx.beginPath();
  ctx.rect(paddleX, canvas.height - paddleHeight, paddleWidth, paddleHeight);
  ctx.fillStyle = "#0095DD";
  ctx.fill();
  ctx.closePath();
}

function drawBricks() {
  for (let c = 0; c < brickColumnCount; c++) {
    for (let r = 0; r < brickRowCount; r++) {
      const brickX = c * (brickWidth + brickPadding) + brickOffsetLeft;
      const brickY = r * (brickHeight + brickPadding) + brickOffsetTop;
      bricks[c][r].x = brickX;
      bricks[c][r].y = brickY;
      ctx.beginPath();
      ctx.rect(brickX, brickY, brickWidth, brickHeight);
      ctx.fillStyle = "#0095DD";
      ctx.fill();
      ctx.closePath();
    }
  }
}

function draw() {
  ctx.clearRect(0, 0, canvas.width, canvas.height);
  drawBricks();
  drawBall();
  drawPaddle();

  if (x + dx > canvas.width - ballRadius || x + dx < ballRadius) {
    dx = -dx;
  }
  if (y + dy < ballRadius) {
    dy = -dy;
  } else if (y + dy > canvas.height - ballRadius) {
    if (x > paddleX && x < paddleX + paddleWidth) {
      dy = -dy;
    } else {
      alert("GAME OVER");
      document.location.reload();
      clearInterval(interval);
    }
  }

  if (rightPressed) {
    paddleX = Math.min(paddleX + 7, canvas.width - paddleWidth);
  } else if (leftPressed) {
    paddleX = Math.max(paddleX - 7, 0);
  }

  x += dx;
  y += dy;
}

interval = setInterval(draw, 10);
```
---

步骤7：碰撞检测

对于屏幕上的砖块，我们需要检测球何时击中砖块并使其消失。每个砖块都有一个`status`属性：`1`表示可见，`0`表示销毁。

为砖块添加Status属性

更新块初始化以包含`status`标志：```javascript
const bricks = [];
for (let c = 0; c < brickColumnCount; c++) {
  bricks[c] = [];
  for (let r = 0; r < brickRowCount; r++) {
    bricks[c][r] = { x: 0, y: 0, status: 1 };
  }
}
```
碰撞检测（）函数

循环遍历每个砖块并检查球的中心是否在砖块的边界框内：```javascript
function collisionDetection() {
  for (let c = 0; c < brickColumnCount; c++) {
    for (let r = 0; r < brickRowCount; r++) {
      const b = bricks[c][r];
      if (b.status === 1) {
        if (
          x > b.x &&
          x < b.x + brickWidth &&
          y > b.y &&
          y < b.y + brickHeight
        ) {
          dy = -dy;
          b.status = 0;
        }
      }
    }
  }
}
```
**碰撞条件（四个必须同时为真）：**
-`x > b.x`—球中心在砖的左边缘的右边。
-`x < b.x + brickWidth`—球的中心在砖的右边缘的左边。
-`y > b.y`—球的中心在砖的上边缘以下。
-`y < b.y + brickHeight`——球的中心在砖的底边之上。

当检测到碰撞时：
-`dy = -dy`反转球的垂直方向（反弹）。
-`b.status = 0`表示砖被破坏。

更新drawBricks（）以尊重状态

只绘制仍然活动的砖块（`status === 1`）：```javascript
function drawBricks() {
  for (let c = 0; c < brickColumnCount; c++) {
    for (let r = 0; r < brickRowCount; r++) {
      if (bricks[c][r].status === 1) {
        const brickX = c * (brickWidth + brickPadding) + brickOffsetLeft;
        const brickY = r * (brickHeight + brickPadding) + brickOffsetTop;
        bricks[c][r].x = brickX;
        bricks[c][r].y = brickY;
        ctx.beginPath();
        ctx.rect(brickX, brickY, brickWidth, brickHeight);
        ctx.fillStyle = "#0095DD";
        ctx.fill();
        ctx.closePath();
      }
    }
  }
}
```
在游戏循环中调用collisionDetection（）

在绘制所有元素后，在`draw()`函数中添加调用：```javascript
function draw() {
  ctx.clearRect(0, 0, canvas.width, canvas.height);
  drawBricks();
  drawBall();
  drawPaddle();
  collisionDetection();
  // ... rest of draw function
}
```
---

##步骤8：追踪得分并取得胜利

我们添加了一个分数计数器，每当砖块被摧毁时，它就会增加，当所有砖块都消失时，就会触发一个胜利条件。

初始化分数```javascript
let score = 0;
```
drawScore（）函数

使用文本渲染在画布上显示当前分数：```javascript
function drawScore() {
  ctx.font = "16px Arial";
  ctx.fillStyle = "#0095DD";
  ctx.fillText(`Score: ${score}`, 8, 20);
}
```
-`ctx.font`设置字体大小和字体族（类似CSS）。
-`ctx.fillText(text, x, y)`以给定坐标呈现文本。
-位置`(8, 20)`将分数放在左上角。

增加分数

在`collisionDetection()`函数中，当砖块被击中时增加分数：```javascript
dy = -dy;
b.status = 0;
score++;
```
添加胜利条件

在增加分数后，检查玩家是否摧毁了所有砖块：```javascript
score++;
if (score === brickRowCount * brickColumnCount) {
  alert("YOU WIN, CONGRATULATIONS!");
  document.location.reload();
  clearInterval(interval);
}
```
砖块的总数为`brickRowCount * brickColumnCount`。当分数达到这个数字时，所有砖块都被摧毁了。

###完成collisionDetection(```javascript
function collisionDetection() {
  for (let c = 0; c < brickColumnCount; c++) {
    for (let r = 0; r < brickRowCount; r++) {
      const b = bricks[c][r];
      if (b.status === 1) {
        if (
          x > b.x &&
          x < b.x + brickWidth &&
          y > b.y &&
          y < b.y + brickHeight
        ) {
          dy = -dy;
          b.status = 0;
          score++;
          if (score === brickRowCount * brickColumnCount) {
            alert("YOU WIN, CONGRATULATIONS!");
            document.location.reload();
            clearInterval(interval);
          }
        }
      }
    }
  }
}
```
在游戏循环中调用drawScore（）

在`draw()`函数中添加调用：```javascript
function draw() {
  ctx.clearRect(0, 0, canvas.width, canvas.height);
  drawBricks();
  drawBall();
  drawPaddle();
  drawScore();
  collisionDetection();
  // ... rest of draw function
}
```
###画布文本方法引用

|Method/Property| ||-----------------|---------|
|`ctx.font`|设置字体大小和家庭|
|`ctx.fillStyle`|设置文本颜色|
在坐标|处绘制填充文本

---

步骤9：鼠标控制

除了键盘控制，我们还添加了鼠标支持，这样玩家就可以通过移动鼠标来移动桨。

添加鼠标移动事件监听器

将处理程序与现有的键盘侦听器一起注册：```javascript
document.addEventListener("mousemove", mouseMoveHandler);
```
mouseMoveHandler函数

计算鼠标相对于画布的水平位置并更新桨的位置：```javascript
function mouseMoveHandler(e) {
  const relativeX = e.clientX - canvas.offsetLeft;
  if (relativeX > 0 && relativeX < canvas.width) {
    paddleX = relativeX - paddleWidth / 2;
  }
}
```
**原理：**
-`e.clientX`——鼠标在浏览器视窗中的水平位置。`canvas.offsetLeft`——从画布左边缘到视口左边缘的距离。`relativeX`——鼠标相对于画布的位置（不是视口）。
边界检查（`relativeX > 0 && relativeX < canvas.width`）确保桨只在鼠标在画布上时移动。
-`paddleX = relativeX - paddleWidth / 2`通过减去一半桨的宽度使鼠标光标下的桨居中。

完成事件监听器设置（键盘+鼠标）```javascript
document.addEventListener("keydown", keyDownHandler);
document.addEventListener("keyup", keyUpHandler);
document.addEventListener("mousemove", mouseMoveHandler);
```
两种控制方法同时工作。玩家可以使用方向键或鼠标，或者随时在它们之间切换。

---

步骤10：收尾

最后一步是添加生命系统（让玩家获得多次机会），并将游戏循环从`setInterval`升级到`requestAnimationFrame`，以获得更流畅的渲染。

添加生命变量```javascript
let lives = 3;
```
drawLives（）函数

在右上角显示剩余生命数：```javascript
function drawLives() {
  ctx.font = "16px Arial";
  ctx.fillStyle = "#0095DD";
  ctx.fillText(`Lives: ${lives}`, canvas.width - 65, 20);
}
```
###实施生命系统

用基于生命的系统取代直接的游戏结束逻辑。当球没有击中球拍时：```javascript
if (y + dy < ballRadius) {
  dy = -dy;
} else if (y + dy > canvas.height - ballRadius) {
  if (x > paddleX && x < paddleX + paddleWidth) {
    dy = -dy;
  } else {
    lives--;
    if (!lives) {
      alert("GAME OVER");
      document.location.reload();
    } else {
      // Reset ball and paddle positions
      x = canvas.width / 2;
      y = canvas.height - 30;
      dx = 2;
      dy = -2;
      paddleX = (canvas.width - paddleWidth) / 2;
    }
  }
}
```
**当一条生命逝去会发生什么：**
-`lives--`减少生命计数器。
-如果`lives`达到`0`，游戏以警告和页面重新加载结束。
—否则，球复位到中间底部，速度复位，球拍复位到中间。

升级到requestAnimationFrame

将`setInterval`替换为`requestAnimationFrame`，以获得更流畅的浏览器优化游戏循环：

**旧方法（删除）：**```javascript
interval = setInterval(draw, 10);
```
新方法:* * * *
在`draw()`函数的末尾添加`requestAnimationFrame(draw)`：```javascript
function draw() {
  // ... all drawing and logic ...
  requestAnimationFrame(draw);
}

// Start the game by calling draw() once:
draw();
```
`requestAnimationFrame`允许浏览器以最佳帧速率（通常为60fps）调度呈现，这比固定的10ms间隔更有效。

在游戏循环中调用drawLives（）```javascript
function draw() {
  ctx.clearRect(0, 0, canvas.width, canvas.height);
  drawBricks();
  drawBall();
  drawPaddle();
  drawScore();
  drawLives();
  collisionDetection();
  // ... rest of logic ...
  requestAnimationFrame(draw);
}
```
---

完成最终游戏代码

下面是一个完整的HTML文件中的整个游戏。这是所有10个步骤结合起来的最终产物。```html
<!doctype html>
<html lang="en-US">
  <head>
    <meta charset="utf-8" />
    <title>2D Breakout Game</title>
    <style>
      * {
        padding: 0;
        margin: 0;
      }
      canvas {
        background: #eeeeee;
        display: block;
        margin: 0 auto;
      }
    </style>
  </head>
  <body>
    <canvas id="myCanvas" width="480" height="320"></canvas>

    <script>
      const canvas = document.getElementById("myCanvas");
      const ctx = canvas.getContext("2d");

      // --- Ball ---
      const ballRadius = 10;
      let x = canvas.width / 2;
      let y = canvas.height - 30;
      let dx = 2;
      let dy = -2;

      // --- Paddle ---
      const paddleHeight = 10;
      const paddleWidth = 75;
      let paddleX = (canvas.width - paddleWidth) / 2;

      // --- Controls ---
      let rightPressed = false;
      let leftPressed = false;

      // --- Bricks ---
      const brickRowCount = 3;
      const brickColumnCount = 5;
      const brickWidth = 75;
      const brickHeight = 20;
      const brickPadding = 10;
      const brickOffsetTop = 30;
      const brickOffsetLeft = 30;

      const bricks = [];
      for (let c = 0; c < brickColumnCount; c++) {
        bricks[c] = [];
        for (let r = 0; r < brickRowCount; r++) {
          bricks[c][r] = { x: 0, y: 0, status: 1 };
        }
      }

      // --- Score and Lives ---
      let score = 0;
      let lives = 3;

      // =====================
      // Event Listeners
      // =====================
      document.addEventListener("keydown", keyDownHandler);
      document.addEventListener("keyup", keyUpHandler);
      document.addEventListener("mousemove", mouseMoveHandler);

      function keyDownHandler(e) {
        if (e.key === "Right" || e.key === "ArrowRight") {
          rightPressed = true;
        } else if (e.key === "Left" || e.key === "ArrowLeft") {
          leftPressed = true;
        }
      }

      function keyUpHandler(e) {
        if (e.key === "Right" || e.key === "ArrowRight") {
          rightPressed = false;
        } else if (e.key === "Left" || e.key === "ArrowLeft") {
          leftPressed = false;
        }
      }

      function mouseMoveHandler(e) {
        const relativeX = e.clientX - canvas.offsetLeft;
        if (relativeX > 0 && relativeX < canvas.width) {
          paddleX = relativeX - paddleWidth / 2;
        }
      }

      // =====================
      // Collision Detection
      // =====================
      function collisionDetection() {
        for (let c = 0; c < brickColumnCount; c++) {
          for (let r = 0; r < brickRowCount; r++) {
            const b = bricks[c][r];
            if (b.status === 1) {
              if (
                x > b.x &&
                x < b.x + brickWidth &&
                y > b.y &&
                y < b.y + brickHeight
              ) {
                dy = -dy;
                b.status = 0;
                score++;
                if (score === brickRowCount * brickColumnCount) {
                  alert("YOU WIN, CONGRATULATIONS!");
                  document.location.reload();
                }
              }
            }
          }
        }
      }

      // =====================
      // Drawing Functions
      // =====================
      function drawBall() {
        ctx.beginPath();
        ctx.arc(x, y, ballRadius, 0, Math.PI * 2);
        ctx.fillStyle = "#0095DD";
        ctx.fill();
        ctx.closePath();
      }

      function drawPaddle() {
        ctx.beginPath();
        ctx.rect(
          paddleX,
          canvas.height - paddleHeight,
          paddleWidth,
          paddleHeight
        );
        ctx.fillStyle = "#0095DD";
        ctx.fill();
        ctx.closePath();
      }

      function drawBricks() {
        for (let c = 0; c < brickColumnCount; c++) {
          for (let r = 0; r < brickRowCount; r++) {
            if (bricks[c][r].status === 1) {
              const brickX =
                c * (brickWidth + brickPadding) + brickOffsetLeft;
              const brickY =
                r * (brickHeight + brickPadding) + brickOffsetTop;
              bricks[c][r].x = brickX;
              bricks[c][r].y = brickY;
              ctx.beginPath();
              ctx.rect(brickX, brickY, brickWidth, brickHeight);
              ctx.fillStyle = "#0095DD";
              ctx.fill();
              ctx.closePath();
            }
          }
        }
      }

      function drawScore() {
        ctx.font = "16px Arial";
        ctx.fillStyle = "#0095DD";
        ctx.fillText(`Score: ${score}`, 8, 20);
      }

      function drawLives() {
        ctx.font = "16px Arial";
        ctx.fillStyle = "#0095DD";
        ctx.fillText(`Lives: ${lives}`, canvas.width - 65, 20);
      }

      // =====================
      // Main Game Loop
      // =====================
      function draw() {
        ctx.clearRect(0, 0, canvas.width, canvas.height);
        drawBricks();
        drawBall();
        drawPaddle();
        drawScore();
        drawLives();
        collisionDetection();

        // Left and right wall collision
        if (
          x + dx > canvas.width - ballRadius ||
          x + dx < ballRadius
        ) {
          dx = -dx;
        }

        // Top wall collision
        if (y + dy < ballRadius) {
          dy = -dy;
        } else if (y + dy > canvas.height - ballRadius) {
          // Bottom edge: paddle collision or lose a life
          if (x > paddleX && x < paddleX + paddleWidth) {
            dy = -dy;
          } else {
            lives--;
            if (!lives) {
              alert("GAME OVER");
              document.location.reload();
            } else {
              x = canvas.width / 2;
              y = canvas.height - 30;
              dx = 2;
              dy = -2;
              paddleX = (canvas.width - paddleWidth) / 2;
            }
          }
        }

        // Paddle movement (keyboard)
        if (rightPressed) {
          paddleX = Math.min(
            paddleX + 7,
            canvas.width - paddleWidth
          );
        } else if (leftPressed) {
          paddleX = Math.max(paddleX - 7, 0);
        }

        x += dx;
        y += dy;
        requestAnimationFrame(draw);
      }

      draw();
    </script>
  </body>
</html>
```
---

快速参考：所有游戏变量

|变量|类型|用途||----------|------|---------|
|`canvas`| const |引用HTML画布元素|
|`ctx`| const | 2D渲染上下文|
|`ballRadius`| const |球的半径（10）|
|`x`，`y`| let |当前球位置|
|`dx`，`dy`| let |球速度（像素每帧）|
|`paddleHeight`| const |桨的高度（10）|
|`paddleWidth`| const |桨的宽度（75）|
|`paddleX`| let |桨叶当前水平位置|
|`rightPressed`| let |是否按下右箭头键|
|`leftPressed`| let |是否按下左箭头键|
|`brickRowCount`| const |砖行数(3)|
|`brickColumnCount`| const |砖列数(5)|
|`brickWidth`| const |每块砖的宽度（75）|
|`brickHeight`| const |每块砖的高度（20）|
|`brickPadding`| const |砖之间的空间（10）|
|`brickOffsetTop`| const |从画布顶部到第一个砖行（30）|
|`brickOffsetLeft`| const |左距帆布到第一个砖柱（30）|
|`bricks`| const | 2D数组保存所有砖块对象|
|`score`| let |当前玩家得分|
|`lives`| let |剩余生命（从3开始）|快速参考：所有函数

|功能|用途||----------|---------|
|`keyDownHandler(e)`|按|键设置`rightPressed`或`leftPressed`为`true`|`keyUpHandler(e)`|按键释放|，设置`rightPressed`或`leftPressed`为`false`|`mouseMoveHandler(e)`|移动桨以跟随鼠标水平位置|
|`collisionDetection()`|检查球对所有活动砖；摧毁击中的砖块，增加分数，检查获胜|
|`drawBall()`|将球呈现在当前位置`(x, y)`|
|`drawPaddle()`|呈现当前位置`paddleX`|
|`drawBricks()`|渲染所有砖块`status === 1`|
|`drawScore()`|在左上角呈现分数文本|
|`drawLives()`|在右上角呈现生活文本|
主游戏循环：清除画布，绘制所有内容，处理碰撞，更新位置|