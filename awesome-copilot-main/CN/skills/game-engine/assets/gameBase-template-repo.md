# GameBase模板库

一个功能丰富，固执己见的2D游戏项目启动模板，使用**Haxe**和**Heaps**游戏引擎。由**Sebastien Benard** (deepnight)创建和维护，他是*Dead Cells*的主要开发者。GameBase提供了一个经过生产测试的基础，包括实体管理、通过LDtk进行的关卡整合、渲染管道和游戏循环架构——所有这些都是为了让开发者跳过样板文件，直接进入游戏特定逻辑。

* *存储库:* * (github.com/deepnight/gameBase) (https://github.com/deepnight/gameBase)
**作者：** [Sebastien Benard / deepnight]（https://deepnight.net）
**技术：** Haxe +堆（HashLink或JS目标）
**关卡编辑器集成：** [LDtk]（https://ldtk.io）

---

# #目的GameBase的存在是为了解决“空白项目”问题。开发者无需从头开始设置渲染、实体系统、摄像机控制、调试叠加和关卡加载，而是复制这个存储库，并立即开始执行特定于游戏的机制。它反映了商业游戏开发的模式，尤其是《死亡细胞》的开发模式。

关键好处:
-预置实体系统，基于网格定位，亚像素精度
- LDtk关卡编辑器集成的视觉关卡设计
-内置调试工具和覆盖层
-帧率独立的游戏循环与固定步骤更新
-相机系统与跟随，摇，变焦，和夹具
—可配置Controller/input管理
-可扩展的渲染管道与堆

---

存储库结构```
gameBase/
  src/
    game/
      App.hx              -- Application entry point and initialization
      Game.hx             -- Main game process, holds level and entities
      Entity.hx           -- Base entity class with grid coords, velocity, animation
      Level.hx            -- Level loading and collision map from LDtk
      Camera.hx           -- Camera follow, shake, zoom, clamping
      Fx.hx               -- Visual effects (particles, flashes, etc.)
      Types.hx            -- Enums, typedefs, and constants
      en/
        Hero.hx            -- Player entity (example implementation)
        Mob.hx             -- Enemy entity (example implementation)
    import.hx             -- Global imports (available everywhere)
  res/
    atlas/                 -- Sprite sheets and texture atlases
    levels/                -- LDtk level project files
    fonts/                 -- Bitmap fonts
  .ldtk                   -- LDtk project file (root)
  build.hxml              -- Haxe compiler configuration
  Makefile                -- Build/run shortcuts
  README.md
```
---

密钥文件及其角色`src/game/App.hx`—应用程序入口点

扩展`dn.Process`的主应用程序类。处理:
-Window/display初始化
-场景管理（根场景图）
-全局输入控制器设置
-调试开关和控制台```haxe
class App extends dn.Process {
  public static var ME : App;

  override function init() {
    ME = this;
    // Initialize rendering, controller, assets
    new Game();
  }
}
```
——游戏过程

管理活动的游戏会话；
—保存当前`Level`的引用
-管理所有活跃的`Entity`实例（通过一个全局链表）
-处理暂停，游戏结束和重启逻辑
-坐标相机和效果```haxe
class Game extends dn.Process {
  public var level : Level;
  public var hero : en.Hero;
  public var fx : Fx;
  public var camera : Camera;

  public function new() {
    super(App.ME);
    level = new Level();
    fx = new Fx();
    camera = new Camera();
    hero = new en.Hero();
  }
}
```
###`src/game/Entity.hx`—基础实体

核心实体类的特点：
- **基于网格的定位：**`cx`，`cy`（整数单元坐标）加上`xr`，`yr`（子单元比0.0到1.0），实现平滑的亚像素移动
- **速度和摩擦力：**`dx`，`dy`（速度）可配置`frictX`，`frictY`- **重力：**可选的每个实体重力
- **精灵管理：**动画精灵通过堆`h2d.Anim`或`dn.heaps.HSprite`- **生命周期：**`update()`，`fixedUpdate()`,`postUpdate()`,`dispose()`- **碰撞助手：**`hasCollision(cx, cy)`检查对水平碰撞地图```haxe
class Entity {
  // Grid position
  public var cx : Int = 0;   // Cell X
  public var cy : Int = 0;   // Cell Y
  public var xr : Float = 0.5; // X ratio within cell (0..1)
  public var yr : Float = 1.0; // Y ratio within cell (0..1)

  // Velocity
  public var dx : Float = 0;
  public var dy : Float = 0;

  // Pixel position (computed)
  public var attachX(get,never) : Float;
  inline function get_attachX() return (cx + xr) * Const.GRID;
  public var attachY(get,never) : Float;
  inline function get_attachY() return (cy + yr) * Const.GRID;

  // Physics step
  public function fixedUpdate() {
    xr += dx;
    dx *= frictX;

    // X collision
    if (xr > 1) { cx++; xr--; }
    if (xr < 0) { cx--; xr++; }

    yr += dy;
    dy *= frictY;

    // Y collision
    if (yr > 1) { cy++; yr--; }
    if (yr < 0) { cy--; yr++; }
  }
}
```
###`src/game/Level.hx`—级别管理

从LDtk项目文件加载和管理关卡数据：
-解析贴图层、实体层和网格层
-建立一个碰撞网格（`hasCollision(cx, cy)`）
—提供查询级别结构的辅助方法```haxe
class Level {
  var data : ldtk.Level;
  var collisions : Map<Int, Bool>;

  public function new(ldtkLevel) {
    data = ldtkLevel;
    // Parse IntGrid layer for collision marks
    for (cy in 0...data.l_Collisions.cHei)
      for (cx in 0...data.l_Collisions.cWid)
        if (data.l_Collisions.getInt(cx, cy) == 1)
          collisions.set(coordId(cx, cy), true);
  }

  public inline function hasCollision(cx:Int, cy:Int) : Bool {
    return collisions.exists(coordId(cx, cy));
  }
}
```
###`src/game/Camera.hx`—摄像机系统

提供:
- **目标跟踪：**跟随一个实体顺利配置死区
- **震动：**屏幕震动与衰减
—**缩放：**动态缩放in/out- **夹紧：**保持相机在水平范围内`src/game/Fx.hx`—效果系统

粒子和视觉效果管理：
-粒子池
-屏幕闪光
-慢动作助手
-颜色叠加效果

---

##技术栈

# # #可

一种跨平台的高级编程语言，可以编译成多个目标：
- **HashLink (HL):**本机字节码虚拟机用于桌面（主要开发目标）
- **JavaScript (JS):**Browser/web目标
**C/C++:**通过HXCPP原生构建

###堆（堆.io）高性能、跨平台的2D/3D游戏引擎：
- gpu加速渲染通过OpenGL/DirectX/WebGL-场景图架构与`h2d.Object`层次结构
-精灵批处理和纹理地图集
-位图字体渲染
-输入抽象

# # # LDtk

Sebastien Benard创造的现代开源2D关卡编辑器：
-基于贴图的视觉关卡设计
-碰撞和元数据的IntGrid层
-游戏对象放置的实体层
—自动平铺规则
- Haxe API从项目文件自动生成

---

##安装说明

# # #先决条件

1. **安装Haxe** (4.0+): [haxe.org]（https://haxe.org/download/）
2. **安装HashLink**（桌面目标）：[hashlink.haxe.org]（https://hashlink.haxe.org/）
3. **安装LDtk**（用于关卡编辑）：[LDtk .io]（https://ldtk.io/）

###开始```bash
# Clone the repository
git clone https://github.com/deepnight/gameBase.git my-game
cd my-game

# Install Haxe dependencies
haxelib install heaps
haxelib install deepnightLibs
haxelib install ldtk-haxe-api

# Build and run (HashLink target)
haxe build.hxml
hl bin/client.hl

# Or use the Makefile (if available)
make run
```
###使用作为起点

1. **克隆或使用模板**——不要分叉；克隆到一个带有游戏名称的新目录。
2. **重命名包**——更新`src/game/`包声明和项目引用以匹配你的游戏。
3. 编辑`build.hxml`**——根据需要调整主类、输出路径和目标。
4. **在LDtk中设计关卡**——打开`.ldtk`文件，定义你的图层和实体，并导出。
5. **实现实体**——在`src/game/en/`扩展`Entity`创建新的实体类。
6. **迭代**——使用调试控制台（在游戏中切换）进行实时检查和调整。

---

##构建目标

|目标|命令|输出|用例||--------|---------|--------|----------|
| HashLink |`haxe build.hxml`|`bin/client.hl`|开发，桌面版|
|`haxe build.js.hxml`|`bin/client.js`|Web/browser构建|
|DirectX/OpenGL| Via HL本机|本机可执行|生产桌面版本|

---

##调试特性

GameBase包括内置调试工具：
- **调试覆盖：**切换一个键显示实体边界，网格，速度，碰撞地图
- **控制台：**游戏内的命令控制台切换旗帜，传送，产卵实体
- **FPS计数器：**可见帧率和更新率监视器
- **进程检查器：**查看活动进程及其层次结构

---

游戏循环架构

GameBase使用固定时间步长的游戏循环模式：```
Each frame:
  1. preUpdate()    -- Input polling, pre-frame logic
  2. fixedUpdate()  -- Physics, movement, collisions (fixed timestep)
     - May run 0-N times per frame to catch up
  3. update()       -- General per-frame logic
  4. postUpdate()   -- Sprite position sync, camera update, rendering prep
```
这确保了无论帧率如何，物理行为都是一致的，同时渲染和视觉更新保持平滑。

---

实体生命周期```
Constructor  -->  init()  -->  [game loop: fixedUpdate/update/postUpdate]  -->  dispose()
```
- **构造函数：**设置初始位置，创建精灵，在全局实体列表中注册
- fixeduupdate():**物理步骤（速度，摩擦，重力，碰撞）
- **update():** AI，状态机，动画触发器
**postUpdate():**同步精灵位置到网格坐标，应用视觉效果
- **dispose():**从实体列表中删除，销毁精灵，清理引用