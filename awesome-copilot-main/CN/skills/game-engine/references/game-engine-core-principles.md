#游戏引擎核心设计原则

关于构建游戏引擎背后的基本架构和设计原则的综合参考。涵盖模块化、关注点分离、核心子系统和实际实现指南。

来源:https://www.gamedev.net/articles/programming/general-and-gameplay-programming/making-a-game-engine-core-design-principles-r3210/---

为什么要创建游戏引擎

游戏引擎是一个可重用的软件框架，它抽象了构建游戏所需的通用系统。而不是从头开始为每个项目编写呈现、物理、输入和音频代码，一个设计良好的引擎将这些作为模块化的、可配置的子系统提供。关键的动机:
**可重用性——在多个游戏项目中使用相同的代码库。
**引擎代码与游戏代码分离**——引擎开发者和游戏设计师可以独立工作。
-可维护性——结构良好的代码更容易调试、扩展和优化。
- **可扩展性** -无需重写现有系统即可添加新功能或平台。

---

核心设计原则

# # #模块化

引擎中的每个主要系统都应该是具有良好定义的接口的独立模块。模块应该通过干净的api进行通信，而不是触及彼此的内部。

重要性：**
-交换实现而不影响其他系统（例如，用Vulkan替换OpenGL渲染器）。
-单独测试各个系统。
-允许团队并行处理不同的模块。

* *例结构:* *```
engine/
  core/           -- Memory, logging, math, utilities
  platform/       -- OS abstraction, windowing, file I/O
  renderer/       -- Graphics API, shaders, materials
  physics/        -- Collision, rigid body dynamics
  audio/          -- Sound playback, mixing, spatial audio
  input/          -- Keyboard, mouse, gamepad, touch
  scripting/      -- Scripting language bindings
  scene/          -- Scene graph, entity management
  resources/      -- Asset loading, caching, streaming
```
关注点分离

每个系统应该有一个单一的、明确定义的责任。避免将渲染逻辑与物理混合，或将输入处理与游戏状态管理混合。

* *实用指南:* *
-渲染器不应该知道游戏机制。
物理引擎不应该知道实体是如何渲染的。
-输入处理应该将原始设备事件转换为游戏代码可以使用的抽象动作。
-游戏逻辑层位于引擎之上，无需修改即可使用引擎服务。

数据驱动设计

在可能的情况下，行为应该由数据而不是硬编码逻辑来控制。这使得设计师和美工无需重新编译代码就可以修改游戏行为。**数据驱动方法的例子
-在数据文件（JSON， XML，二进制）中定义关卡布局，而不是代码。
—通过组件数据配置的实体属性和行为。
-着色器参数暴露为可在工具中编辑的材料属性。
-在配置中定义动画状态机，而不是命令式代码。

最小化依赖

每个模块应该尽可能少地依赖于其他模块。依赖关系图应该是一个清晰的层次结构，而不是一个纠结的网络。```
Game Code
    |
    v
Engine High-Level Systems (Scene, Entity, Scripting)
    |
    v
Engine Low-Level Systems (Renderer, Physics, Audio, Input)
    |
    v
Engine Core (Memory, Math, Logging, Platform Abstraction)
    |
    v
Operating System / Hardware
```
模块之间的循环依赖是糟糕架构的标志，应该消除。

---

实体-组件-系统（ECS）模式

ECS是现代游戏引擎中广泛采用的架构模式，它更倾向于组合而不是继承。

核心概念

**Entity**——代表游戏对象的唯一标识符（通常是整数ID）。实体没有自己的行为或数据。
- **组件**——附加到实体的普通数据容器。每种组件类型存储实体状态的一个方面（位置、速度、精灵、生命值等）。
- **System**——处理具有一组特定组件的所有实体的函数或对象。系统包含逻辑；组件包含数据。

为什么ECS优于继承

传统的面向对象继承创建了严格的深层层次结构：```
GameObject
  -> MovableObject
    -> Character
      -> Player
      -> Enemy
        -> FlyingEnemy
        -> GroundEnemy
```
这种方法的问题：
-添加一个新的实体类型，结合多个分支的特征需要重组层次结构或使用多重继承。
-深层的等级制度是脆弱的；对基类的更改会波及所有后代。
-类积累未使用的行为随着时间的推移。

ECS通过构图解决了这些问题：```javascript
// An entity is just an ID
const player = world.createEntity();

// Attach components to define what it is
world.addComponent(player, new Position(100, 200));
world.addComponent(player, new Velocity(0, 0));
world.addComponent(player, new Sprite("player.png"));
world.addComponent(player, new Health(100));
world.addComponent(player, new PlayerInput());

// A "flying enemy" is just a different combination of components
const flyingEnemy = world.createEntity();
world.addComponent(flyingEnemy, new Position(400, 50));
world.addComponent(flyingEnemy, new Velocity(0, 0));
world.addComponent(flyingEnemy, new Sprite("bat.png"));
world.addComponent(flyingEnemy, new Health(30));
world.addComponent(flyingEnemy, new AIBehavior("patrol_fly"));
world.addComponent(flyingEnemy, new Flying());
```
系统过程组件```javascript
// Movement system: processes all entities with Position + Velocity
function movementSystem(world, deltaTime) {
  for (const [entity, pos, vel] of world.query(Position, Velocity)) {
    pos.x += vel.x * deltaTime;
    pos.y += vel.y * deltaTime;
  }
}

// Render system: processes all entities with Position + Sprite
function renderSystem(world, context) {
  for (const [entity, pos, sprite] of world.query(Position, Sprite)) {
    context.drawImage(sprite.image, pos.x, pos.y);
  }
}

// Gravity system: only affects entities with Velocity but NOT Flying
function gravitySystem(world, deltaTime) {
  for (const [entity, vel] of world.query(Velocity).without(Flying)) {
    vel.y += 9.8 * deltaTime;
  }
}
```
ECS的好处

- **灵活的组合** -创建任何实体类型的混合组件，而无需修改代码。
- **缓存友好的数据布局**——将组件连续存储在内存中可以提高CPU缓存性能。
- **并行性**——操作不同组件集的系统可以并行运行。
- **易于序列化** -组件是纯数据，使save/load直接。

---

核心引擎子系统

内存管理

自定义内存管理对游戏引擎性能至关重要。默认的分配器（malloc/new）是通用的，没有针对游戏工作负载进行优化。

**常用分配策略：**- **堆栈分配器** -快速后进先出分配临时，帧范围的数据。在每一帧结束时重置堆栈指针。
- **Pool Allocator**——固定大小的块分配相同类型的对象（实体，组件，粒子）。零的碎片。
- **Frame Allocator**——一个线性分配器，重置每一帧。理想的逐帧临时数据。
- **双缓冲分配器**——两个帧分配器交替使用每帧，允许前一帧的数据持续存在。```cpp
// Conceptual frame allocator
class FrameAllocator {
    char* buffer;
    size_t offset;
    size_t capacity;

public:
    void* allocate(size_t size) {
        void* ptr = buffer + offset;
        offset += size;
        return ptr;
    }

    void reset() {
        offset = 0;  // All allocations freed instantly
    }
};
```
资源管理

资源管理器处理游戏资产的加载、缓存和生命周期管理。

* *主要职责:* *
-异步加载——在后台线程中加载资源，以避免拖延游戏循环。
- **引用计数** -跟踪有多少系统使用资产；不再被引用时卸载。
- **缓存**——保持最近使用的资产在内存中，以避免冗余的磁盘读取。
- **热重新加载**——检测磁盘上的资产更改并在开发期间运行时重新加载它们。
- **资源句柄**——使用句柄（id或智能指针）而不是原始指针来引用资源。```javascript
class ResourceManager {
  constructor() {
    this.cache = new Map();
    this.loading = new Map();
  }

  async load(path) {
    // Return cached resource if available
    if (this.cache.has(path)) {
      return this.cache.get(path);
    }

    // Avoid duplicate loads
    if (this.loading.has(path)) {
      return this.loading.get(path);
    }

    // Start async load
    const promise = this._loadFromDisk(path).then(resource => {
      this.cache.set(path, resource);
      this.loading.delete(path);
      return resource;
    });

    this.loading.set(path, promise);
    return promise;
  }

  unload(path) {
    this.cache.delete(path);
  }
}
```
渲染管道

渲染子系统将游戏的视觉状态转换成屏幕上的像素。

**典型渲染管道阶段：**1. **场景遍历**——遍历场景图或查询ECS中的可渲染实体。
2. **截锥体剔除**——丢弃相机视野之外的物体。
3. **遮挡剔除**——丢弃隐藏在其他几何体后面的物体。
4. **排序**——按材料、深度或透明度要求排序对象。
5. **批处理**——将具有相同材质的对象分组，以最小化绘制调用和状态更改。
6. **顶点处理**——将顶点从模型空间转换到屏幕空间（顶点着色器）。
7. 栅格化——将三角形转换为碎片（像素）。
8. **片段处理**——使用光照、纹理和效果（片段着色器）计算最终像素颜色。
9. **后处理** -应用屏幕空间效果，如绽放，色调映射和抗锯齿。

**渲染命令模式：**而不是直接进行绘制调用，构建一个渲染命令列表，可以在提交之前进行排序和批处理：```javascript
class RenderCommand {
  constructor(mesh, material, transform, sortKey) {
    this.mesh = mesh;
    this.material = material;
    this.transform = transform;
    this.sortKey = sortKey;
  }
}

class Renderer {
  constructor() {
    this.commandQueue = [];
  }

  submit(command) {
    this.commandQueue.push(command);
  }

  flush(context) {
    // Sort by material to minimize state changes
    this.commandQueue.sort((a, b) => a.sortKey - b.sortKey);

    for (const cmd of this.commandQueue) {
      this._bindMaterial(cmd.material);
      this._setTransform(cmd.transform);
      this._drawMesh(cmd.mesh, context);
    }

    this.commandQueue.length = 0;
  }
}
```
物理集成

物理子系统模拟物理行为并检测碰撞。

**主要设计考虑：**

- **固定时间步长** -物理应该以固定的速率（例如，50 Hz）独立于渲染帧速率更新。这确保了确定性的模拟行为。
- **碰撞阶段** -使用宽相位（空间划分，边界体层次）来快速消除非碰撞对，然后使用窄相位进行精确的交叉测试。
**物理世界分离**——物理世界应该保持自己的对象（物理实体）表示，与游戏实体分离。同步步骤在它们之间映射。```javascript
class PhysicsWorld {
  constructor(fixedTimestep = 1 / 50) {
    this.fixedTimestep = fixedTimestep;
    this.accumulator = 0;
    this.bodies = [];
  }

  update(deltaTime) {
    this.accumulator += deltaTime;

    while (this.accumulator >= this.fixedTimestep) {
      this.step(this.fixedTimestep);
      this.accumulator -= this.fixedTimestep;
    }
  }

  step(dt) {
    // Integrate velocities
    for (const body of this.bodies) {
      body.velocity.y += body.gravity * dt;
      body.position.x += body.velocity.x * dt;
      body.position.y += body.velocity.y * dt;
    }

    // Detect and resolve collisions
    this.broadPhase();
    this.narrowPhase();
    this.resolveCollisions();
  }
}
```
输入系统

输入系统将原始硬件事件转化为具有游戏意义的行动。

分层设计:* * * *

1. **硬件层**——接收来自操作系统的原始事件（按下按键，移动鼠标，按下按钮）。
2. 映射层——通过可配置的绑定将原始输入转换为命名动作（例如，“Space”映射为“Jump”，“W”映射为“MoveForward”）。
3. **动作层**——暴露游戏代码查询的抽象动作，与特定硬件输入完全分离。```javascript
class InputManager {
  constructor() {
    this.bindings = new Map();
    this.actionStates = new Map();
  }

  bind(action, key) {
    this.bindings.set(key, action);
  }

  handleKeyDown(event) {
    const action = this.bindings.get(event.code);
    if (action) {
      this.actionStates.set(action, true);
    }
  }

  handleKeyUp(event) {
    const action = this.bindings.get(event.code);
    if (action) {
      this.actionStates.set(action, false);
    }
  }

  isActionActive(action) {
    return this.actionStates.get(action) || false;
  }
}

// Usage
const input = new InputManager();
input.bind("Jump", "Space");
input.bind("MoveLeft", "KeyA");
input.bind("MoveRight", "KeyD");

// In game update:
if (input.isActionActive("Jump")) {
  player.jump();
}
```
事件系统

事件系统支持引擎子系统和游戏代码之间的分离通信，而无需直接引用。

* *发布-订阅模式:* *```javascript
class EventBus {
  constructor() {
    this.listeners = new Map();
  }

  on(eventType, callback) {
    if (!this.listeners.has(eventType)) {
      this.listeners.set(eventType, []);
    }
    this.listeners.get(eventType).push(callback);
  }

  off(eventType, callback) {
    const callbacks = this.listeners.get(eventType);
    if (callbacks) {
      const index = callbacks.indexOf(callback);
      if (index !== -1) callbacks.splice(index, 1);
    }
  }

  emit(eventType, data) {
    const callbacks = this.listeners.get(eventType);
    if (callbacks) {
      for (const callback of callbacks) {
        callback(data);
      }
    }
  }
}

// Usage
const events = new EventBus();

events.on("collision", (data) => {
  console.log(`${data.entityA} collided with ${data.entityB}`);
});

events.on("entityDestroyed", (data) => {
  spawnExplosion(data.position);
  addScore(data.points);
});

// Emit from physics system
events.emit("collision", { entityA: player, entityB: wall });
```
* *延迟事件:* *

为了性能和确定性，事件可以在一个帧中排队，并在更新周期的特定点调度：```javascript
class DeferredEventBus extends EventBus {
  constructor() {
    super();
    this.eventQueue = [];
  }

  queue(eventType, data) {
    this.eventQueue.push({ type: eventType, data });
  }

  dispatchQueued() {
    for (const event of this.eventQueue) {
      this.emit(event.type, event.data);
    }
    this.eventQueue.length = 0;
  }
}
```
场景管理

场景管理器将游戏内容组织成逻辑组，并管理不同游戏状态之间的转换。

共同模式:* * * *

- **场景图**——一个层次树的节点，其中子转换相对于父转换。移动父节点会移动所有的子节点。
- **场景堆栈** -场景可以推和弹出。暂停菜单将推动游戏玩法；忽略它会回到游戏玩法上。
- **场景加载**—场景定义加载哪些资产和实体。场景管理器协调加载、初始化和清理。```javascript
class SceneManager {
  constructor() {
    this.scenes = new Map();
    this.activeScene = null;
  }

  register(name, scene) {
    this.scenes.set(name, scene);
  }

  async switchTo(name) {
    if (this.activeScene) {
      this.activeScene.onExit();
      this.activeScene.unloadResources();
    }

    this.activeScene = this.scenes.get(name);
    await this.activeScene.loadResources();
    this.activeScene.onEnter();
  }

  update(deltaTime) {
    if (this.activeScene) {
      this.activeScene.update(deltaTime);
    }
  }

  render(context) {
    if (this.activeScene) {
      this.activeScene.render(context);
    }
  }
}
```
---

##平台抽象

设计良好的引擎将特定于平台的代码抽象到统一的接口后面。这使得引擎可以在多个操作系统、图形api和硬件配置上运行。

**需要抽象的领域：**

|关注点|示例||---|---|
|窗口| Win32， X11, Cocoa， SDL, GLFW |
图形API | OpenGL， Vulkan, DirectX, Metal, WebGL |
|文件I/O| POSIX， Win32，虚拟文件系统|
|线程| pthreads， Win32线程，Web worker |
|音频输出| WASAPI， CoreAudio， ALSA, Web Audio |
|输入设备| DirectInput， XInput, evdev，手柄API |```javascript
// Abstract file system interface
class FileSystem {
  async readFile(path) { throw new Error("Not implemented"); }
  async writeFile(path, data) { throw new Error("Not implemented"); }
  async exists(path) { throw new Error("Not implemented"); }
}

// Web implementation
class WebFileSystem extends FileSystem {
  async readFile(path) {
    const response = await fetch(path);
    return response.arrayBuffer();
  }
}

// Node.js implementation
class NodeFileSystem extends FileSystem {
  async readFile(path) {
    const fs = require("fs").promises;
    return fs.readFile(path);
  }
}
```
---

初始化和关机顺序

引擎子系统必须按依赖顺序初始化，并按相反顺序关闭。

典型的初始化顺序：**

1. 核心系统（日志、内存、配置）
2. 平台层（窗口创建、输入设备）
3. 渲染系统（图形上下文，默认资源）
4. 音频系统
5. 物理系统
6. 资源管理器（加载default/shared资产）
7. 现场经理
8. 脚本系统
9. 游戏初始化

**关机颠倒了这个顺序**，以确保系统在它们所依赖的系统之前被清理。```javascript
class Engine {
  async initialize() {
    this.logger = new Logger();
    this.config = new Config("engine.json");
    this.platform = new Platform();
    await this.platform.createWindow(this.config.window);

    this.renderer = new Renderer(this.platform.canvas);
    this.audio = new AudioSystem();
    this.physics = new PhysicsWorld();
    this.resources = new ResourceManager();
    this.input = new InputManager(this.platform.window);
    this.events = new EventBus();
    this.scenes = new SceneManager();

    this.logger.info("Engine initialized");
  }

  shutdown() {
    this.scenes.cleanup();
    this.resources.unloadAll();
    this.input.cleanup();
    this.physics.cleanup();
    this.audio.cleanup();
    this.renderer.cleanup();
    this.platform.cleanup();
    this.logger.info("Engine shutdown complete");
  }

  run() {
    let lastTime = performance.now();

    const loop = (currentTime) => {
      const deltaTime = (currentTime - lastTime) / 1000;
      lastTime = currentTime;

      this.input.poll();
      this.physics.update(deltaTime);
      this.scenes.update(deltaTime);
      this.events.dispatchQueued();
      this.scenes.render(this.renderer);
      this.renderer.present();

      requestAnimationFrame(loop);
    };

    requestAnimationFrame(loop);
  }
}
```
---

性能原则

避免过早抽象

虽然模块化很重要，但在理解实际需求之前过度设计接口会导致不必要的复杂性。从简单、具体的实现开始，并在实际用例需要时向抽象方向重构。

优化前的配置文件

在花时间进行优化之前，使用分析工具测量实际的性能瓶颈。关于时间花在哪里的直觉常常是错误的。

面向数据的设计

根据访问数据的方式组织数据，而不是通过面向对象的抽象。在内存中连续存储相同类型的组件（Structure of Arrays而不是Array of Structures）可以显著提高CPU缓存命中率。```javascript
// Array of Structures (cache-unfriendly for position-only iteration)
const entities = [
  { position: {x: 0, y: 0}, sprite: "hero.png", health: 100 },
  { position: {x: 5, y: 3}, sprite: "bat.png", health: 30 },
];

// Structure of Arrays (cache-friendly for position-only iteration)
const positions = { x: [0, 5], y: [0, 3] };
const sprites = ["hero.png", "bat.png"];
const healths = [100, 30];
```
最小化热路径分配

避免在每帧更新期间创建新对象或分配内存。预分配缓冲区、使用对象池和重用临时对象。

批处理操作

将类似的操作组合在一起，以减少上下文切换、绘制调用设置和缓存丢失带来的开销。在移动到下一个类型之前处理给定类型的所有实体。

---

关键原则总结

|原理描述|描述||---|---|
|模块化|接口清晰的独立子系统|
|关注点分离|每个系统有一个单独的职责|
|数据驱动设计|由数据控制的行为，而不是硬编码逻辑|
|复合继承|灵活实体构建的ECS模式|
|清晰的分层依赖图|
|平台抽象|基于平台特定代码的统一接口|
|固定时间步长物理|独立于帧率的确定性仿真|
|事件驱动通信|通过发布-订阅实现的解耦交互|
|面向数据的性能|为访问模式|优化内存布局
|在优化|配置文件之前进行测量，以确定实际的瓶颈|