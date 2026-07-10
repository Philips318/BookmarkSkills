#游戏开发算法

一个全面的参考涵盖基本算法的游戏开发，包括
线条绘制，光线投射，碰撞检测，物理模拟，矢量数学。

---

Bresenham的线算法——光线投射、视线和寻径

>来源：https://deepnight.net/tutorial/bresenham-magic-raycasting-line-of-sight-pathfinding/它是什么

布里森汉姆线算法是确定网格中哪些单元格的有效方法
沿着两点之间的直线躺下。最初用于绘制像素
栅格显示，它已经成为游戏开发中用于光线投射的基本工具，
视线检查，以及基于网格的寻路。该算法只使用整数
算术（加法，减法和位移位），使其非常快。

数学/算法概念其核心思想是沿长轴（距离较大的轴）行走
单元格，累积一个误差项，跟踪真实线偏离的距离
从当前的小轴位置。当误差超过阈值时，小轴
坐标递增。

关键特性:
- **纯整数算术**：不需要浮点除法或乘法。
- **增量误差积累**：分数斜率通过整数误差跟踪
术语，避免漂移。
- **对称**：通过调整，无论线的方向如何，算法都是相同的
一步的迹象。

给定两个网格点`(x0, y0)`和`(x1, y1)`：```
dx = abs(x1 - x0)
dy = abs(y1 - y0)
```
每个步骤初始化和更新错误项。当它穿过零时，仲离子
轴是阶梯的。

# # #的伪代码```
function bresenham(x0, y0, x1, y1):
    dx = abs(x1 - x0)
    dy = abs(y1 - y0)
    sx = sign(x1 - x0)   // -1 or +1
    sy = sign(y1 - y0)   // -1 or +1
    err = dx - dy

    while true:
        visit(x0, y0)          // process or record this cell

        if x0 == x1 AND y0 == y1:
            break

        e2 = 2 * err

        if e2 > -dy:
            err = err - dy
            x0  = x0 + sx

        if e2 < dx:
            err = err + dx
            y0  = y0 + sy
```
### Haxe实现（从源代码）```haxe
public function hasLineOfSight(x0:Int, y0:Int, x1:Int, y1:Int):Bool {
    var dx = hxd.Math.iabs(x1 - x0);
    var dy = hxd.Math.iabs(y1 - y0);
    var sx = (x0 < x1) ? 1 : -1;
    var sy = (y0 < y1) ? 1 : -1;
    var err = dx - dy;

    while (true) {
        if (isBlocking(x0, y0))
            return false;

        if (x0 == x1 && y0 == y1)
            return true;

        var e2 = 2 * err;
        if (e2 > -dy) {
            err -= dy;
            x0 += sx;
        }
        if (e2 < dx) {
            err += dx;
            y0 += sy;
        }
    }
}
```
实用游戏开发应用

- **视线(LOS)**：从实体走到目标的布里森汉姆线；如果有任何
细胞沿路径是墙或障碍物，视线被阻挡。
- **网格上的光线投射**：从多个方向的光源投射光线来计算
可见性地图或视场锥。
- **基于网格的寻路验证**：计算路径后（例如，通过a *），验证
使用布雷森姆检查可以畅通无阻地到达航路点之间的直线捷径。
- **弹丸追踪**：确定子弹或弹丸通过的瓷砖
一款基于贴图的游戏。
- **照明和阴影投射**：跟踪光线从光源计算光照vs
2D网格上的阴影细胞。

---

碰撞检测和响应系统

>来源：https://medium.com/@erikkubiak/dev-log-1-custom-engine-writing-my-collision-system-2a97856f9a93它是什么碰撞系统负责检测游戏对象何时重叠或相交
然后解决这些重叠，以便物体做出物理响应(弹跳，停止，
滑动)。构建自定义碰撞系统需要选择合适的边界
形状，实现重叠测试，并设计分辨率策略。

数学/算法概念

####边界形状

- **AABB (Axis-Aligned Bounding Box)**：一个矩形，它的边与
坐标轴。由位置（中心或左上角）和半宽度定义。
快速重叠测试，但对旋转或不规则形状不精确。
- **圆/球体碰撞器**：由中心和半径定义。重叠测试很简单
距离的比较。
- **OBB（定向边界框）**：一个旋转的矩形。使用分离轴定理
用于重叠测试。

#### AABB与AABB重叠测试两个轴对齐的边界框当且仅当它们在每个轴上重叠时重叠：```
overlapX = (a.x - a.halfW < b.x + b.halfW) AND (a.x + a.halfW > b.x - b.halfW)
overlapY = (a.y - a.halfH < b.y + b.halfH) AND (a.y + a.halfH > b.y - b.halfH)
collision = overlapX AND overlapY
```
####圆与圆重叠测试```
dx = a.x - b.x
dy = a.y - b.y
distSquared = dx * dx + dy * dy
collision = distSquared < (a.radius + b.radius) ^ 2
```
比较距离的平方可以避免昂贵的平方根运算。

####分离轴定理（SAT）

如果两个凸形状至少存在一个轴，那么它们不会碰撞
投影不重叠。对于矩形，测试两个矩形的边法线。
如果所有投影都重叠，则形状发生碰撞。

####清扫及剪枝（大阶段）

与其测试每一对对象（O(n^2)），不如沿着一个轴对对象进行排序
它们的最小范围。在该轴上没有重叠的对象不能碰撞，并且是
从详细检查中修剪。

伪代码——碰撞检测和解决```
// Broad phase: spatial hash or sweep-and-prune
candidates = broadPhase(allObjects)

for each pair (a, b) in candidates:
    overlap = narrowPhaseTest(a, b)

    if overlap:
        // Compute penetration vector
        penetration = computePenetration(a, b)

        // Resolve: push objects apart along the minimum penetration axis
        if a.isStatic:
            b.position += penetration
        else if b.isStatic:
            a.position -= penetration
        else:
            a.position -= penetration * 0.5
            b.position += penetration * 0.5

        // Optional: apply impulse for velocity response
        relativeVelocity = a.velocity - b.velocity
        impulse = computeImpulse(relativeVelocity, penetration.normal, a.mass, b.mass)
        a.velocity -= impulse / a.mass
        b.velocity += impulse / b.mass
```
####最小穿透矢量（适用于aabs）```
function computePenetration(a, b):
    overlapX_left  = (a.x + a.halfW) - (b.x - b.halfW)
    overlapX_right = (b.x + b.halfW) - (a.x - a.halfW)
    overlapY_top   = (a.y + a.halfH) - (b.y - b.halfH)
    overlapY_bot   = (b.y + b.halfH) - (a.y - a.halfH)

    minOverlapX = min(overlapX_left, overlapX_right)
    minOverlapY = min(overlapY_top, overlapY_bot)

    if minOverlapX < minOverlapY:
        return Vector(sign * minOverlapX, 0)
    else:
        return Vector(0, sign * minOverlapY)
```
空间分区策略

|策略|最适合|描述||---|---|---|
| **均匀网格** |均匀分布的物体|将世界划分为固定单元；对象在其单元格中注册。|
| **四叉树** |非均匀分布|递归将空间细分为4个象限。高效的稀疏场景。|
| **空间哈希** |动态场景|哈希对象到桶的位置。O(1)查找邻居。|
| **扫描和修剪** |许多移动对象|按轴排序；只测试重叠的间隔。|

实用游戏开发应用- **平台物理：解决玩家与地形的碰撞，使角色着陆
平台，不能穿墙。
- **弹丸命中检测**：确定弹丸（通常是一个小AABB或圆）
接触敌人或障碍物。
- **触发区域**：检测当一个球员进入一个区域(重叠测试没有物理
Resolution)来触发事件。
- **实体堆叠**：处理对象堆叠在彼此使用迭代
具有多个通道的分辨率。

---

速度和速度

>来源：https://www.gamedev.net/tutorials/programming/math-and-physics/a-quick-lesson-in-velocity-and-speed-r6109/它是什么

速度和速度是游戏中移动物体的基本概念。**速度**是一个
标量（仅大小），而**速度**是矢量（大小和方向）。
理解这种区别对于执行正确的运动、物理、
以及人工智能操纵行为。

数学/算法概念

# # # #定义- **速度**：一个标量量表示有多快的对象移动，不管
方向。  ```
  speed = |velocity| = sqrt(vx^2 + vy^2)
  ```
- **速度**：表示速度和方向的矢量。  ```
  velocity = (vx, vy)
  ```
- **加速度**：速度随时间的变化率。  ```
  acceleration = (ax, ay)
  velocity += acceleration * deltaTime
  ```
####用速度更新位置

每一帧中，物体的位置由其速度更新，并按时间步长缩放：```
position.x += velocity.x * deltaTime
position.y += velocity.y * deltaTime
```
这是欧拉积分，最简单的（一阶）积分方法。

####正规化方向

在给定的方向上以固定的速度移动，将方向矢量和归一化
乘以所需的速度：```
direction = target - current
length = sqrt(direction.x^2 + direction.y^2)
if length > 0:
    direction.x /= length
    direction.y /= length
velocity = direction * speed
```
这可以防止“对角线移动问题”，即全速对角线移动
在两个轴上结果约1.414倍的预期速度。

####帧率独立性

没有`deltaTime`，移动速度取决于帧速率：```
// WRONG: frame-rate dependent
position += velocity

// CORRECT: frame-rate independent
position += velocity * deltaTime
```
`deltaTime`是自上次帧更新以来经过的时间（以秒为单位）。

伪代码——完成移动更新```
function update(entity, deltaTime):
    // Apply acceleration (gravity, thrust, friction, etc.)
    entity.velocity.x += entity.acceleration.x * deltaTime
    entity.velocity.y += entity.acceleration.y * deltaTime

    // Clamp speed to a maximum
    currentSpeed = magnitude(entity.velocity)
    if currentSpeed > entity.maxSpeed:
        entity.velocity = normalize(entity.velocity) * entity.maxSpeed

    // Apply friction / drag
    entity.velocity.x *= (1 - entity.friction * deltaTime)
    entity.velocity.y *= (1 - entity.friction * deltaTime)

    // Update position
    entity.position.x += entity.velocity.x * deltaTime
    entity.position.y += entity.velocity.y * deltaTime
```
实用游戏开发应用

- **角色移动**：应用速度每帧移动玩家顺利，
夹紧到最大速度，以保持一致的感觉。
- **射弹**：给子弹或箭头一个初始速度矢量；更新位置
每一帧。
- **重力**：应用一个恒定的向下加速速度每帧模拟
下降。
- **摩擦和阻力**：减少速度随时间乘以阻尼系数
模拟表面摩擦或空气阻力
- **AI转向**：计算一个理想的速度向一个目标，然后顺利调整
当前朝着它的速度（寻找、逃离、到达行为）。

---

物理引擎基础

>来源：https://winter.dev/articles/physics-engine它是什么物理引擎模拟真实世界的物理行为——重力、碰撞、刚性
身体动力学——让游戏对象移动和互动逼真。的核心循环
物理引擎包括：施加力，整合运动，检测碰撞，
解决碰撞。

数学/算法概念

####物理循环

物理引擎运行固定时间步长的更新循环：```
accumulator = 0
fixedDeltaTime = 1 / 60  // 60 Hz physics

function physicsUpdate(frameDeltaTime):
    accumulator += frameDeltaTime

    while accumulator >= fixedDeltaTime:
        step(fixedDeltaTime)
        accumulator -= fixedDeltaTime
```
使用固定的时间步长确保了确定性，稳定的模拟，而不考虑渲染
帧速率。

####集成方法

**半隐式欧拉**（辛欧拉）——游戏物理标准：```
velocity += acceleration * dt
position += velocity * dt
```
这比显式欧拉（它首先更新位置）更稳定，因为速度
在用于更新位置之前进行更新。

**Verlet集成**——不显式存储速度的替代方案：```
newPosition = 2 * position - oldPosition + acceleration * dt * dt
oldPosition = position
position = newPosition
```
Verlet对于约束（布，布娃娃）特别有用，因为位置可以
在保持动量的同时被直接操纵。

####刚体属性

每个刚体有：

|属性|描述||---|---|
|`position`|世界空间的质心|
线速度向量
所有力/质量之和|
|`mass`|抗线性加速度|
|`inverseMass`|`1 / mass`（0为静态对象）|
|`angle`|旋转角度|
|`angularVelocity`|旋转速率|
|`inertia`|抗角加速度|
|`restitution`|弹性（0 =无弹性，1 =完全弹性）|
表面摩擦系数|

####力积累

力每帧累积，然后转换为加速度：```
function applyForce(body, force):
    body.forceAccumulator += force

function integrate(body, dt):
    body.acceleration = body.forceAccumulator * body.inverseMass
    body.velocity += body.acceleration * dt
    body.position += body.velocity * dt
    body.forceAccumulator = (0, 0)  // reset
```
####碰撞检测管道

检测阶段分为两个阶段：

1. **宽相位**：快速消除对不可能碰撞使用边界
体块（aabb）和空间结构（网格、BVH树、清扫和修剪）。

2. **窄相位**：对于候选对，执行精确的形状对形状测试
确定它们是否重叠并计算接触信息(碰撞法线，
渗透深度，接触点)。

####带有脉冲的碰撞分辨率

当两个物体碰撞时，沿碰撞法线施加一个脉冲使其分离
调整它们的速度：```
function resolveCollision(a, b, normal, penetration):
    // Relative velocity at the contact point
    relVel = b.velocity - a.velocity
    velAlongNormal = dot(relVel, normal)

    // Do not resolve if objects are separating
    if velAlongNormal > 0:
        return

    // Coefficient of restitution (take minimum)
    e = min(a.restitution, b.restitution)

    // Impulse magnitude
    j = -(1 + e) * velAlongNormal
    j /= a.inverseMass + b.inverseMass

    // Apply impulse
    impulse = j * normal
    a.velocity -= impulse * a.inverseMass
    b.velocity += impulse * b.inverseMass

    // Positional correction (prevent sinking)
    correction = max(penetration - slop, 0) / (a.inverseMass + b.inverseMass) * percent
    a.position -= correction * a.inverseMass * normal
    b.position += correction * b.inverseMass * normal
```
关键常量:
-`slop`：一个小的公差（例如0.01），以防止微穿透引起的抖动。
-`percent`：通常0.2到0.8；控制位置修正的力度
应用。

####旋转动力学

对于二维旋转，扭矩为力的旋转当量：```
torque = cross(contactPoint - centerOfMass, impulse)
angularAcceleration = torque * inverseInertia
angularVelocity += angularAcceleration * dt
angle += angularVelocity * dt
```
转动惯量取决于形状：
—**圈**:`I = 0.5 * m * r^2`- **矩形**:`I = (1/12) * m * (w^2 + h^2)`伪代码——完成物理步骤```
function step(dt):
    // 1. Apply external forces (gravity, player input, etc.)
    for each body in world.bodies:
        if not body.isStatic:
            body.applyForce(gravity * body.mass)

    // 2. Integrate velocities and positions
    for each body in world.bodies:
        if not body.isStatic:
            body.velocity += (body.forceAccumulator * body.inverseMass) * dt
            body.position += body.velocity * dt
            body.angularVelocity += body.torque * body.inverseInertia * dt
            body.angle += body.angularVelocity * dt
            body.forceAccumulator = (0, 0)
            body.torque = 0

    // 3. Broad-phase collision detection
    pairs = broadPhase(world.bodies)

    // 4. Narrow-phase collision detection
    contacts = []
    for each (a, b) in pairs:
        contact = narrowPhase(a, b)
        if contact:
            contacts.append(contact)

    // 5. Resolve collisions (iterative solver)
    for i in range(solverIterations):   // typically 4-10 iterations
        for each contact in contacts:
            resolveCollision(contact.a, contact.b,
                             contact.normal, contact.penetration)
```
实用游戏开发应用

- **平台游戏**：重力、地面接触、跳跃弧线和移动平台。
- **自上而下的游戏**：沿着墙壁滑动，击退攻击。
- **布娃娃物理**：由约束连接的刚体链。
- **车辆仿真**：悬架弹簧、轮胎摩擦、发动机受力。
- **破坏**：打破物体碎片与个人物理体。

---

游戏开发中的矢量数学

>来源：https://www.gamedev.net/tutorials/programming/math-and-physics/vector-maths-for-game-dev-beginners-r5442/它是什么

向量是游戏开发的数学构建块。一个向量表示
有大小和方向的量。在2D游戏中，向量是成对`(x, y)`；
在3D中，是`(x, y, z)`的三倍。几乎所有的游戏系统——移动，物理，渲染，
人工智能——依赖于向量运算。

数学/算法概念

####矢量表示

二维矢量：```
v = (x, y)
```
三维矢量：```
v = (x, y, z)
```
矢量可以表示位置、方向、速度、力或任何与
大小和方向。

####矢量加法

特定组件的添加。用于对位置施加速度、合力等。```
a + b = (a.x + b.x, a.y + b.y)
```
**示例**：通过速度移动角色：```
position = position + velocity * deltaTime
```
####矢量减法

特定组件的减法。用来计算从一点到的方向和距离
另一个地方。```
a - b = (a.x - b.x, a.y - b.y)
```
**例子**：从敌人到玩家的方向：```
directionToPlayer = player.position - enemy.position
```
####标量乘法

在不改变矢量方向的情况下缩放矢量的大小：```
s * v = (s * v.x, s * v.y)
```
**示例**：设置移动速度：```
velocity = normalizedDirection * speed
```
####大小（长度）

由勾股定理计算的向量的长度：```
|v| = sqrt(v.x^2 + v.y^2)
```
在3 d:```
|v| = sqrt(v.x^2 + v.y^2 + v.z^2)
```
**优化**：当只比较距离（不需要实际值）时，使用
平方大小，以避免昂贵的平方根：```
|v|^2 = v.x^2 + v.y^2
```
# # # #正常化

生成指向相同方向的单位向量（长度为1）：```
normalize(v) = v / |v| = (v.x / |v|, v.y / |v|)
```
归一化向量表示纯方向。总是检查`|v| > 0`之前
通过除法来避免被0除法。

**示例**：获取实体所面对的方向：```
facing = normalize(target - self.position)
```
####点积

向量：编码两个向量之间角关系的标量结果：```
a . b = a.x * b.x + a.y * b.y
```
在3 d:```
a . b = a.x * b.x + a.y * b.y + a.z * b.z
```
几何解释:```
a . b = |a| * |b| * cos(theta)
```
其中`theta`是两个向量之间的夹角。对于单位矢量：```
a . b = cos(theta)
```
关键特性:
-`a . b > 0`：向量指向大致相同的方向（角度< 90度）。
-`a . b == 0`：向量垂直（角度= 90度）。
-`a . b < 0`：向量指向大致相反的方向（角度> 90度）。

**游戏开发使用**
-视野检查：玩家是否在敌人的前面？
—照明：计算漫射光强度（`max(0, dot(normal, lightDir))`）。
-投影：投影一个矢量到另一个。

####叉积（3D）

产生一个垂直于两个输入向量的向量：```
a x b = (
    a.y * b.z - a.z * b.y,
    a.z * b.x - a.x * b.z,
    a.x * b.y - a.y * b.x
)
```
叉乘的大小等于：```
|a x b| = |a| * |b| * sin(theta)
```
在2D中，“叉乘”是一个标量（3D叉乘的z分量）：```
a x b = a.x * b.y - a.y * b.x
```
**游戏开发使用**
-确定上弦顺序（顺时针和逆时针）。
-计算照明的表面法线。
-确定一个点在一条线的左边还是右边。

####垂直矢量（2D）

为了得到一个垂直于`(x, y)`的向量：```
perp = (-y, x)    // 90 degrees counter-clockwise
perp = (y, -x)    // 90 degrees clockwise
```
用于计算二维边缘和墙壁的法线。

# # # #投影

将向量`a`投影到向量`b`上：```
proj_b(a) = (a . b / b . b) * b
```
如果`b`已经是单位向量：```
proj_b(a) = (a . b) * b
```
**游戏开发使用**
-确定沿表面法线的速度分量（对于bounce/reflection）。
-沿墙滑动：从速度中减去法向分量。

# # # #的倒影

向量`v`在法线为`n`的表面上反射（其中`n`是单位向量）：```
reflected = v - 2 * (v . n) * n
```
**游戏开发使用**
-球在墙上反弹。
-光反射计算。
-弹跳轨迹。

伪代码—Vector2D类```
class Vector2D:
    x, y

    function add(other):
        return Vector2D(x + other.x, y + other.y)

    function subtract(other):
        return Vector2D(x - other.x, y - other.y)

    function scale(scalar):
        return Vector2D(x * scalar, y * scalar)

    function magnitude():
        return sqrt(x * x + y * y)

    function magnitudeSquared():
        return x * x + y * y

    function normalize():
        mag = magnitude()
        if mag > 0:
            return Vector2D(x / mag, y / mag)
        return Vector2D(0, 0)

    function dot(other):
        return x * other.x + y * other.y

    function cross(other):
        return x * other.y - y * other.x

    function perpendicular():
        return Vector2D(-y, x)

    function reflect(normal):
        d = dot(normal)
        return Vector2D(x - 2 * d * normal.x, y - 2 * d * normal.y)

    function angleTo(other):
        return acos(normalize().dot(other.normalize()))

    function distanceTo(other):
        return subtract(other).magnitude()

    function lerp(other, t):
        return Vector2D(
            x + (other.x - x) * t,
            y + (other.y - y) * t
        )
```
实用游戏开发应用- **运动和转向**：添加速度矢量的位置；规范化的方向
矢量，并乘以速度，以保持一致的运动。
- **距离检查**：使用平方量级的性能友好的半径检查
（例如，“这个敌人在射程内吗？”）。
- **视场**：使用实体的前向向量和
指向目标，以确定目标是否在视锥内。
- **墙滑动**：项目的速度到墙的切线(垂直于
（正常）允许沿表面平滑滑动。
- **反射和弹跳**：使用反射公式时，抛射或球
碰到一个表面。
- **插值**：使用`lerp`（线性插值）之间的两个向量平滑
运动、摄像机跟踪和动画。
- **旋转**：旋转一个矢量的角度使用三角函数：  ```
  rotated.x = v.x * cos(angle) - v.y * sin(angle)
  rotated.y = v.x * sin(angle) + v.y * cos(angle)
  ```
---

##快速参考表

|算法/概念|主要用例|复杂性||---|---|---|
|布里森汉姆线|网格光线投射，视线| 0 （max(dx, dy)）每条光线|
| AABB重叠|快速碰撞检测| 0(1)每对|
|圆重叠|圆对撞机检测| 0(1)每对|
|分离轴定理|凸多边形碰撞|每对（n =边）O(n) |
|空间哈希|宽相位碰撞剔除| 0(1)平均查找|
欧拉积分|简单物理步进|每步步|每体0 (1
| Verlet积分|基于约束的物理|每个身体每步|
|脉冲分辨率|碰撞响应| 0（迭代*触点）|
|向量归一化|方向提取| 0 (1)|
|点积|Angle/projection查询| 0 (1)|
垂直度/缠绕度| O(1) |
|反射|反弹/跳起| 0 (1)|