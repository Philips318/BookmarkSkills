#组件架构参考

该参考定义了`src/components`中的分类、文件布局和依赖关系方向。

设计意图和原则

该技能的目标不仅是添加React组件，而且要始终如一地应用Container/Presentation模式，明确职责分离和依赖方向。

此参考没有定义完整的应用程序范围的体系结构。它关注组件边界的设计质量。

-将呈现职责与逻辑职责分开。
不要将状态管理、副作用或业务决策放在表示层。
避免跨边界混合责任，保持依赖方向明确。

# #分类

-将所有组件放在`src/components`下。
-只使用两个类别：
-`ui`：仅呈现的无状态组件。
—`features`：包含逻辑的组件。重新分类规则

如果用户请求`ui`，但实现包含以下任何一个，将其视为`features`，并在创建文件之前请求确认：

—`useState`、`useReducer`、`useEffect`。
-异步行为（API调用，计时器，订阅）。
—读写context/store.-Business/data转换逻辑。

使用以下选项提问：

——`Create as features`——`Keep ui and move logic/state to parent or features`##层职责

此技能分为两个阶段定义图层。

# # # 1。组件类型

|类型|职责|| ---------- | ------------------------------------------------------------------------------------------------------ |
|`ui`|可重用的仅呈现组件。必须不包括业务逻辑、副作用或状态管理。|
|`features`|面向用例的组件。处理状态转换、事件解释和异步编排。|

# # # 2。`features`的内层

|层|职责|主文件|| -------------- | ----------------------------------------------------------------------------------- | -------------------------------------------------------------------------- |
|`container`|处理状态管理、副作用、事件处理和数据获取。|`index.tsx``use<ComponentName>.tsx``types.ts`|
|只接收道具和渲染UI。不能执行外部I/O或状态更新。|`presentation.tsx`,`presentation.module.scss`,`presentation.stories.tsx`|

注:

-`ui`仅由表示组成。
-`features`必须将容器和外观分开。

##实现规则

# # # ui

—保持组件无状态。
-通过props接受数据和回调。
—不要增加副作用和数据抓取。
-优先选择来自Mantine或其他UI库的原语；仅在需要时使用自定义JSX/SCSS。

# # #特性

—使用Container/Presentation模式。
—保持逻辑在`use<ComponentName>.tsx`。
-遵循下面的`Container/Presentation Separation Rules (Anti-patterns and Decision Examples)`了解详细的职责边界和反模式。

分离规则（反模式和决策示例）

原则:容器负责状态管理、副作用、事件解释和异步处理。
-呈现只负责从接收到的道具呈现。
保持业务决策和数据转换在容器端代码中，而不是在表示中。

放置规则:

-放置于容器：`useState`/`useReducer`/`useEffect`， API调用，context/store读写，业务规则应用。
-放置在演示中：JSX渲染和仅显示分支（例如：空视图，加载视图，错误视图）。
-使用`types.ts`来定义容器和表示之间的I/O契约。

反模式:

-从表示调用api或突变。
-从演示文稿直接更新context/store。
-在表示中实现业务决策（授权检查、状态转换决策、数据塑造）。
-正式拆分文件，同时保持实际的逻辑在表示。好/坏例子：

—Bad:`presentation.tsx`直接获取数据并管理加载状态。
-好：`use<ComponentName>.tsx`管理数据获取和状态，`presentation.tsx`仅从`isLoading`，`items`和`onAction`等道具呈现。

依赖方向

—`features`—>`ui`：允许。
—`ui`—>`features`：禁止。

##文件结构

# # # ui- `index.tsx`
- `presentation.tsx`
- `presentation.stories.tsx`
- `presentation.module.scss`
# # #特性- `index.tsx`
——`use<ComponentName>.tsx`- `presentation.tsx`
- `types.ts`
- `presentation.stories.tsx`
- `presentation.module.scss`
最小的故事书

—总是创建`Default`。
-只有当不同的状态存在时才添加特定于状态的故事。
-偏好基于行为的故事集：
-交互控件：`Hover`。
—输入类型：`Focus`、`Error`、`Disabled`。
-Layout/open-close:`Open`,`Closed`,`Empty`。