#简单的2D平台引擎模板

这是一款基于网格的2D平台游戏引擎教程，作者是Sebastien Benard（《Dead Cells》的首席开发者）。这个模板涵盖了高性能平台游戏的基本架构：一个双坐标定位系统，它混合了具有亚像素精度的整数网格单元、速度和摩擦力学、重力，以及一个强大的碰撞检测和响应系统。该方法与语言无关，但示例使用Haxe。

* *引用来源:* *
-[第1部分-基础]（https://deepnight.net/tutorial/a-simple-platformer-engine-part-1-basics/）
-[第二部分-碰撞]（https://deepnight.net/tutorial/a-simple-platformer-engine-part-2-collisions/）

**作者：** [Sebastien Benard / deepnight]（https://deepnight.net）

---

引擎架构概述该引擎是围绕一个基于网格的世界构建的，其中每个单元都有固定的像素大小（例如，16x16）。实体使用双坐标系统存在于此网格中：整数单元坐标用于粗定位，浮点比例用于每个单元内的亚像素精度。这种设计可以对网格进行像素完美的碰撞检测，同时保持平滑、流畅的运动。

核心原则1. **网格是真理：**世界是一个二维网格的细胞。碰撞数据保存在网格中。
2. **实体跨单元：**实体的位置由它所占据的单元格（`cx`,`cy`）加上它进入该单元格的距离（`xr`,`yr`）来定义。
3. **移动delta （`dx`,`dy`）代表每一步单元格的分数，而不是原始像素。
4. **碰撞是网格查找：**引擎不是根据几何体测试精灵边界，而是检查实体即将进入的网格单元。

---

##第一部分：基础知识

网格

关卡是一个2D数组，其中每个单元格要么为空，要么为实。一个常量定义了单元格的大小（以像素为单位）：```haxe
static inline var GRID = 16;
```
碰撞数据存储为一个简单的二维布尔或整数映射：```haxe
// Check if a grid cell is solid
function hasCollision(cx:Int, cy:Int):Bool {
  // Look up cell value in the level data
  return level.getCollision(cx, cy) != 0;
}
```
实体定位：双坐标

每个实体使用四个值跟踪其位置：

|变量|类型|描述||----------|------|-------------|
|`cx`| Int |单元格X坐标（实体所在的列）|
|`cy`| Int |单元格Y坐标（实体所在的行）|
|`xr`|浮动|单元格内的X比率，范围为0.0到1.0 |
|`yr`|浮动|单元格内的Y比率，范围为0.0 ~ 1.0 |

位于`cx=5, cy=3, xr=0.5, yr=1.0`的实体水平居中于单元格（5,3）并位于底部边缘。

转换为像素坐标

要渲染实体，将网格坐标转换为像素位置：```haxe
// Pixel position for rendering
var pixelX : Float = (cx + xr) * GRID;
var pixelY : Float = (cy + yr) * GRID;
```
即使碰撞系统在离散的网格单元上运行，这也会产生平滑的、亚像素级精确的渲染位置。

速度和移动

速度以每固定步的单元比单位表示（不是每帧像素）：```haxe
var dx : Float = 0; // Horizontal velocity (cells per step)
var dy : Float = 0; // Vertical velocity (cells per step)
```
每次固定步长更新，速度被添加到比率：```haxe
// Apply horizontal movement
xr += dx;

// Apply vertical movement
yr += dy;
```
单元格溢出

当比值超过0…1范围内，实体已移动到相邻的单元格中：```haxe
// X overflow
while (xr > 1) { xr--; cx++; }
while (xr < 0) { xr++; cx--; }

// Y overflow
while (yr > 1) { yr--; cy++; }
while (yr < 0) { yr++; cy--; }
```
# # #摩擦

每一步的摩擦力都是一个乘数，使速度向零衰减：```haxe
var frictX : Float = 0.82; // Horizontal friction (0 = instant stop, 1 = no friction)
var frictY : Float = 0.82; // Vertical friction

// Applied each step after movement
dx *= frictX;
dy *= frictY;

// Clamp very small values to zero
if (Math.abs(dx) < 0.0005) dx = 0;
if (Math.abs(dy) < 0.0005) dy = 0;
```
典型摩擦值：
-`0.82`—标准地面摩擦（响应，快速停止）
-`0.94`—冰面或滑面（减速缓慢）
-`0.96`—空气摩擦（非常缓慢的水平减速）

# # #重力

重力是每一步添加到`dy`的常数：```haxe
static inline var GRAVITY = 0.05; // In cell-ratio units per step^2

// In fixedUpdate:
dy += GRAVITY;
```
由于`dy`累积并施加摩擦，实体达到自然终端速度。

渲染/精灵同步

在物理步骤之后，精灵被放置在计算的像素位置：```haxe
// In postUpdate, after physics is done:
sprite.x = (cx + xr) * GRID;
sprite.y = (cy + yr) * GRID;
```
对于平台游戏角色，锚点通常位于精灵的底部中心。`yr = 1.0`代表当前单元格的底部，精灵的脚与地板对齐。

基本实体模板```haxe
class Entity {
  // Grid coordinates
  var cx : Int = 0;
  var cy : Int = 0;
  var xr : Float = 0.5;
  var yr : Float = 1.0;

  // Velocity
  var dx : Float = 0;
  var dy : Float = 0;

  // Friction
  var frictX : Float = 0.82;
  var frictY : Float = 0.82;

  // Gravity
  static inline var GRAVITY = 0.05;

  // Grid size
  static inline var GRID = 16;

  // Pixel position (computed)
  public var attachX(get, never) : Float;
  inline function get_attachX() return (cx + xr) * GRID;

  public var attachY(get, never) : Float;
  inline function get_attachY() return (cy + yr) * GRID;

  public function fixedUpdate() {
    // Gravity
    dy += GRAVITY;

    // Apply velocity
    xr += dx;
    yr += dy;

    // Apply friction
    dx *= frictX;
    dy *= frictY;

    // Clamp small values
    if (Math.abs(dx) < 0.0005) dx = 0;
    if (Math.abs(dy) < 0.0005) dy = 0;

    // Cell overflow
    while (xr > 1) { xr--; cx++; }
    while (xr < 0) { xr++; cx--; }
    while (yr > 1) { yr--; cy++; }
    while (yr < 0) { yr++; cy--; }
  }

  public function postUpdate() {
    sprite.x = attachX;
    sprite.y = attachY;
  }
}
```
---

第2部分：碰撞

碰撞哲学

这个引擎不使用边界盒到边界盒的碰撞检测（这会因为斜坡、单向平台和边缘情况而变得复杂），而是直接检查网格单元。由于实体的位置已经用网格术语表示，因此碰撞检测变成了一系列简单的整数查找。

核心理念

在允许实体移动到相邻单元格之前，检查该单元格是否是实体。如果是，则夹紧实体的比率并将其在该轴上的速度归零。

轴分离

碰撞是按轴处理的——首先是X轴，然后是Y轴（反之亦然）。这简化了逻辑并避免了角落案例隧道问题。

x轴碰撞

在将`dx`应用到`xr`之后，在执行单元溢出步骤之前，检查碰撞：```haxe
// Apply X movement
xr += dx;

// Check collision to the RIGHT
if (dx > 0 && hasCollision(cx + 1, cy) && xr >= 0.7) {
  xr = 0.7;   // Clamp: stop before entering the solid cell
  dx = 0;     // Kill horizontal velocity
}

// Check collision to the LEFT
if (dx < 0 && hasCollision(cx - 1, cy) && xr <= 0.3) {
  xr = 0.3;   // Clamp: stop before entering the solid cell
  dx = 0;     // Kill horizontal velocity
}

// Cell overflow (after collision check)
while (xr > 1) { xr--; cx++; }
while (xr < 0) { xr++; cx--; }
```
为什么是0.7和0.3？**这些阈值表示实体在单元格内的碰撞半径。以`xr = 0.5`为中心且半宽度为0.3单元格的实体将在右侧的`xr = 0.7`和左侧的`xr = 0.3`处发生碰撞。根据实体宽度调整这些值。

y轴碰撞

同样，将`dy`应用到`yr`后：```haxe
// Apply Y movement
yr += dy;

// Check collision BELOW (floor)
if (dy > 0 && hasCollision(cx, cy + 1) && yr >= 1.0) {
  yr = 1.0;   // Clamp: land on top of the solid cell
  dy = 0;     // Kill vertical velocity
}

// Check collision ABOVE (ceiling)
if (dy < 0 && hasCollision(cx, cy - 1) && yr <= 0.3) {
  yr = 0.3;   // Clamp: stop before entering ceiling cell
  dy = 0;     // Kill vertical velocity
}

// Cell overflow
while (yr > 1) { yr--; cy++; }
while (yr < 0) { yr++; cy--; }
```
对于底层碰撞，`yr = 1.0`意味着实体恰好位于当前单元格的下边缘，也就是它下面单元格的上边缘。这是自然的“站在地上”的姿势。

地面探测

确定实体是否站在坚实的地面上（对于跳跃逻辑，动画等）：```haxe
function isOnGround() : Bool {
  return hasCollision(cx, cy + 1) && yr >= 0.98;
}
```
阈值`0.98`而不是`1.0`允许轻微的浮点不精度。

完成实体与碰撞```haxe
class Entity {
  var cx : Int = 0;
  var cy : Int = 0;
  var xr : Float = 0.5;
  var yr : Float = 1.0;
  var dx : Float = 0;
  var dy : Float = 0;
  var frictX : Float = 0.82;
  var frictY : Float = 0.82;

  static inline var GRID = 16;
  static inline var GRAVITY = 0.05;

  // Collision radius (half-width in cell-ratio units)
  var collRadius : Float = 0.3;

  function hasCollision(testCx:Int, testCy:Int):Bool {
    return level.isCollision(testCx, testCy);
  }

  function isOnGround():Bool {
    return hasCollision(cx, cy + 1) && yr >= 0.98;
  }

  public function fixedUpdate() {
    // --- Gravity ---
    dy += GRAVITY;

    // --- X Axis ---
    xr += dx;

    // Right collision
    if (dx > 0 && hasCollision(cx + 1, cy) && xr >= 1.0 - collRadius) {
      xr = 1.0 - collRadius;
      dx = 0;
    }

    // Left collision
    if (dx < 0 && hasCollision(cx - 1, cy) && xr <= collRadius) {
      xr = collRadius;
      dx = 0;
    }

    // X cell overflow
    while (xr > 1) { xr--; cx++; }
    while (xr < 0) { xr++; cx--; }

    // --- Y Axis ---
    yr += dy;

    // Floor collision
    if (dy > 0 && hasCollision(cx, cy + 1) && yr >= 1.0) {
      yr = 1.0;
      dy = 0;
    }

    // Ceiling collision
    if (dy < 0 && hasCollision(cx, cy - 1) && yr <= collRadius) {
      yr = collRadius;
      dy = 0;
    }

    // Y cell overflow
    while (yr > 1) { yr--; cy++; }
    while (yr < 0) { yr++; cy--; }

    // --- Friction ---
    dx *= frictX;
    dy *= frictY;

    if (Math.abs(dx) < 0.0005) dx = 0;
    if (Math.abs(dy) < 0.0005) dy = 0;
  }

  public function postUpdate() {
    sprite.x = (cx + xr) * GRID;
    sprite.y = (cy + yr) * GRID;
  }
}
```
---

##碰撞边缘案例和解决方案

###对角线移动/角裁剪

因为碰撞是按顺序按轴检查的，所以沿对角线移动到角落的实体自然会首先针对一个轴进行解决。这可以防止实体陷入角落，并消除对复杂对角碰撞逻辑的需求。

高速隧道

如果`dx`或`dy`足够大，可以在一步中跳过整个单元格，则实体可以“隧穿”墙壁。解决方案:

1. **夹紧`dx`和`dy`至最大0.5（每步半单元）
2. **细分步骤：**如果速度超过阈值，以较小的增量运行碰撞检查
3. **射线移动网格：**检查移动路径上的每个细胞```haxe
// Simple velocity cap
if (dx > 0.5) dx = 0.5;
if (dx < -0.5) dx = -0.5;
if (dy > 0.5) dy = 0.5;
if (dy < -0.5) dy = -0.5;
```
单向平台

平台，实体可以跳起来，但从上面降落；```haxe
// In Y collision, check for one-way platform
if (dy > 0 && isOneWayPlatform(cx, cy + 1) && yr >= 1.0 && prevYr < 1.0) {
  yr = 1.0;
  dy = 0;
}
```
关键：只有当实体向下移动（`dy > 0`）并且之前在平台上方（`prevYr < 1.0`）时才会发生碰撞。

# # #斜坡

对于基本的坡度支持，不使用二进制碰撞检查，而是在单元格内实体的x位置查询坡度高度：```haxe
// Pseudocode for slope collision
var slopeHeight = getSlopeHeight(cx, cy + 1, xr);
if (yr >= slopeHeight) {
  yr = slopeHeight;
  dy = 0;
}
```
---

# #跳

跳跃只是一个负的`dy`脉冲：```haxe
function jump() {
  if (isOnGround()) {
    dy = -0.5; // Jump impulse (in cell-ratio units)
  }
}
```
重力自然地使向上运动减速，形成抛物线。允许可变高度跳跃（按住按钮越长=跳得越高）：```haxe
// On jump button release, reduce upward velocity
function onJumpRelease() {
  if (dy < 0) {
    dy *= 0.5; // Cut remaining upward velocity
  }
}
```
---

坐标系图```
  Cell (cx, cy)           Next Cell (cx+1, cy)
  +-------------------+   +-------------------+
  |                   |   |                   |
  |  xr=0.0    xr=1.0 --> |  xr=0.0           |
  |                   |   |                   |
  |         *         |   |                   |
  |     (xr=0.5,      |   |                   |
  |      yr=0.5)      |   |                   |
  |                   |   |                   |
  +-------------------+   +-------------------+
  yr=0.0      yr=1.0 = top of cell below

  Pixel position = (cx + xr) * GRID, (cy + yr) * GRID
```
---

更新订单摘要```
fixedUpdate():
  1. Apply gravity          dy += GRAVITY
  2. Apply X velocity       xr += dx
  3. Check X collisions     Clamp xr, zero dx if colliding
  4. Handle X cell overflow cx/xr normalization
  5. Apply Y velocity       yr += dy
  6. Check Y collisions     Clamp yr, zero dy if colliding
  7. Handle Y cell overflow cy/yr normalization
  8. Apply friction         dx *= frictX, dy *= frictY
  9. Zero out tiny values   Threshold check

postUpdate():
  1. Sync sprite position   sprite.x/y = pixel coords
  2. Update animation       Based on state/velocity
  3. Camera follow          Track entity
```
---

##设计优势

|功能|好处||---------|---------|
基于网格的碰撞|每次检查O(1)查找，不需要宽相位|
双坐标|亚像素平滑渲染，整数碰撞|
简单的逻辑，自然处理拐角|
|基于比例的速度|与分辨率无关的运动|
摩擦倍增器|每个表面类型可调的感觉|
|细胞溢出while-loops |处理多细胞安全移动|