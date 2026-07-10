---
description: 'Comprehensive Vue 3 development standards and best practices: Composition API, `<script setup>`, the full reactivity system, compiler macros (defineModel/defineSlots/defineOptions), built-in components (Teleport/Suspense/Transition/KeepAlive), provide/inject, composables, Pinia, Vue Router, TypeScript, testing, performance, SSR, and security.'
applyTo: '**/*.vue, **/*.ts, **/*.js, **/*.css, **/*.scss'
---
# Vue 3开发说明

构建生产级Vue 3应用的权威指导。默认使用`<script setup lang="ts">`的**Composition API**，现代反应系统，以及官方生态系统（Pinia, Vue Router, Vite, Vitest）。比起遗留的Options API和Vue 2模式，更喜欢下面的习惯用法。

##项目背景
- Vue 3.4+（使用3.5+的特性，如`useTemplateRef`，`useId`，以及项目版本允许的反应性道具解构）
—默认为`<script setup lang="ts">`单文件组件（sfc）。
TypeScript无处不在：组件、可组合组件、存储库和路由器。
- Pinia用于国家管理；Vue路由器用于路由；请选build/dev.-用于测试的Vitest + Vue测试工具（或Vue测试库）。创作风格和组件设计
-使用`<script setup>`-它比`setup()`或Options API更简洁，更快，并且具有更好的类型推断。
-每个组成部分一项责任；将大型组件拆分为较小的集中组件和可组合组件。
-订购一个SFC为`<script setup>`，然后`<template>`，然后`<style scoped>`。
在PascalCase中命名组件；使用多字名称（例如`UserCard`，而不是`Card`）以避免与本地元素冲突。
-共同定位组件特定类型，并将共享类型提升到`types/`模块中。编译宏（不需要导入）
-`defineProps<T>()`-从TypeScript中声明类型化的道具interface/type来进行完整的推理。
-`withDefaults(defineProps<T>(), { ... })`-提供道具默认值（或在3.5+中使用响应式道具解构默认值）。
-`defineEmits<{ change: [id: number]; update: [value: string] }>()`-声明类型化事件。
-`defineModel<T>()`(3.4+) -在组件上实现`v-model`的规范方式；支持多个模型、参数和修饰符。
-`defineExpose({ ... })`-显式公开公共命令式API；默认情况下不公开任何内容。
-`defineSlots<{ default(props: { item: T }): any }>()`-类型为named/scoped槽位。
-`defineOptions({ name, inheritAttrs })`-设置`<script setup>`内部的组件选项。
-永远不要直接改变道具-发出一个事件，使用`defineModel`，或者用`computed`/`ref`派生本地状态。反应性系统
核心原语
-`ref()`用于原语和单个可替换引用；通过脚本中的`.value`访问（在模板中自动展开）。
-`reactive()`为深度反应型objects/collections；不要直接破坏它（破坏反应性）-使用`toRefs()`/`toRef()`。
-`computed()`表示派生值；保持清洁和无副作用。使用可写计算（`get`/`set`）的双向派生状态。
-当你*衍生*一个值而不是执行一个副作用时，更喜欢`computed`而不是`watch`。# # #观察家
-`watch(source, cb, options)`为显式依赖；`watchEffect(cb)`用于自动跟踪依赖项。
-当你需要首先更新DOM时，故意使用观察选项：`{ immediate: true }`，`{ deep: true }`,`{ once: true }`（3.4+）和`flush: 'post'`。
-使用`onCleanup`/`onWatcherCleanup`回调注册清理，以取消过时的异步工作（debounce, fetch, listeners）。
-当人工观察者超出其自然作用域时，通过其返回的句柄停止它们。

高级反应性（有意使用）
-`shallowRef`/`shallowReactive`用于大型或外部管理的数据，跳过深度跟踪。
-`readonly()`分发共享状态的不可变视图。
-`toRef`/`toRefs`在破坏时保持反应性；`toRaw`/`markRaw`选择退出非响应性对象（例如类实例，第三方客户端）。
-`effectScope()`组和处置相关的效果在一起（在composables/libraries有用）。
-`customRef`用于debounced/throttled或存储支持的refs。可组合（可重用逻辑）
-将有状态、可重用的逻辑提取到`composables/`下的`useXxx()`函数中。
-接受refs/getters作为输入，返回refs/computed；使用`toValue()`/`MaybeRefOrGetter`规范化reff -or-plain输入。
-在可组合（`onMounted`/`onUnmounted`或`tryOnScopeDispose`）内设置和拆除，以便调用者不会泄漏。
-在设置阶段保持可组合组件同步；将异步操作公开为返回函数。
-达到VueUse的共同需求，而不是重新实现（例如`useStorage`，`useEventListener`,`useDebounceFn`）。

生命周期和效果
—使用`onMounted`、`onBeforeMount`、`onUpdated`、`onBeforeUnmount`、`onUnmounted`、`onActivated`/`onDeactivated`（与`<KeepAlive>`配合使用）和`onErrorCaptured`。
-始终清理`onUnmounted`中的计时器、监听器、观察器和订阅。
-为SSR保护仅浏览器api (`window`,`document`)；在`onMounted`中运行它们。模板最佳实践
-总是在`v-for`上设置一个稳定的，唯一的`:key`；当项可以重新排序或变化时，永远不要使用数组索引。
-永远不要把`v-if`和`v-for`放在同一个元素上-通过`computed`过滤器代替。
-`v-show`用于频繁切换的元素；`v-if`用于条件安装。
-使用`v-memo`跳过重新渲染昂贵的静态子树，`v-once`的内容，渲染一次。
-使用`:`（v-bind）和`@`（v-on）速记一致；用`<template>`对节点进行分组以避免包装器元素。
—避免在模板中使用沉重的表达式—将它们移动到`computed`或方法。

# #槽
-使用命名槽进行布局扩展和作用域槽（`<slot :item="item" />`+`#default="{ item }"`）向父节点公开数据。
-提供合理的后备插槽内容。
—在适当的地方使用`v-slot`（`#`）简写和动态槽名。##内置组件
-`<Teleport to="body">`用于必须转义overflow/stacking上下文的情态、祝酒词和工具提示。
-`<Suspense>`与`#default`/`#fallback`异步设置和惰性组件；与错误处理配对。
-`<Transition>`/`<TransitionGroup>`用于enter/leave和列表动画（在分组项目上设置`:key`）。
-`<KeepAlive>`（与`include`/`exclude`/`max`）缓存组件状态跨切换；处理`onActivated`/`onDeactivated`。
-`<component :is="...">`为动态元件；`defineAsyncComponent(() => import('...'))`用于code-split/lazy加载loading/error组件。

##提供/注入（依赖注入）
-为安全起见，型号为`InjectionKey<T>`(`Symbol`):`provide(key, value)`/`inject(key)`。
-提供默认值或断言存在以避免`undefined`。
-在提供子代不应该变异的状态时，首选`readonly()`而是公开显式更新函数。
-对横切问题使用注射；使用Pinia来获取应用范围内的共享状态。自定义指令和插件
-作者指令作为带有`mounted`/`updated`（等）钩子的对象；保持它们以dom为中心（例如`v-focus`，`v-click-outside`）。
-封装全局设置（路由器，pinia, i18n， UI库）作为插件通过`app.use(...)`；在`main.ts`中注册全局配置。

##国家管理与皮尼亚
-使用Pinia为shared/cross-component状态；使用`ref`/`reactive`保持仅组件状态在本地。
-首选**设置存储**:`defineStore('user', () => { const user = ref(...); const isLoggedIn = computed(...); function login(){}; return { user, isLoggedIn, login } })`。
-每个域名一个存储；保持async/side效果和getter的动作纯粹和同步。
-用`storeToRefs()`分解以保持反应性；`$patch`用于批处理突变，`$reset`用于恢复状态，`$subscribe`/`$onAction`用于横切关注点。
正确处理SSR水合，保持库存序列化。##使用Vue路由器路由
—使用lazy`component: () => import('...')`定义路由，用于自动代码分割。
-使用导航守卫（`beforeEach`,`beforeEnter`,`beforeRouteLeave`）进行验证和未保存更改检查；总是精确解析/`next()`一次。
-使用`route.meta`（类型）为每路由配置像`requiresAuth`。
-通过`useRoute()`读取params/query，并通过`useRouter()`导航；将`route.params`视为响应性的（注意它，不要缓存）。
-配置`scrollBehavior`可预测的滚动恢复；在可用的情况下启用类型化路由。

TypeScript集成
通过泛型编译器宏输入props/emits/slots，而不是运行时对象语法。
-类型refs显式当推理太窄：`ref<User | null>(null)`。
-类型模板参考`useTemplateRef<HTMLInputElement>('input')`（3.5+）或`ref<HTMLInputElement | null>(null)`。
-使用`<script setup lang="ts" generic="T">`构建通用组件。
-provide/inject型配`InjectionKey<T>`；type Pinia通过推断的返回类型存储。# #样式
—默认为`<style scoped>`；慎重地使用`:deep()`、`:slotted()`和`:global()`选择器。
-使用`v-bind()`在`<style>`驱动CSS从反应状态；更喜欢CSS自定义属性的主题。
-考虑CSS模块（`<style module>`）在较大的团队中进行类名隔离。

##表单和验证
-绑定输入与`v-model`（和`defineModel`自定义输入）；使用修饰符`.lazy`、`.number`、`.trim`。
-使用模式库（Zod/Yup）和表单库（VeeValidate或FormKit）验证非平凡表单。
-客户端验证仅用于用户体验-始终在服务器上验证和清理。

##错误处理
-使用`onErrorCaptured`作为组件树边界，`app.config.errorHandler`作为全局钩子。
-用`<Suspense>`+一个错误回退包async/lazy边界。
-用户界面错误；将诊断记录到监视管道中。# #性能
—代码拆分路由和重组件（`defineAsyncComponent`，动态`import()`）。`computed`用于缓存，`v-memo`/`v-once`用于静态子树，`shallowRef`/`shallowReactive`用于大数据集。
-虚拟长列表（例如`vue-virtual-scroller`）；分页或窗口大数据。
-避免不必要的深度反应，避免在模板中创建新的object/array文字。
-在3.5+版本中，考虑SSR的惰性水合策略，以减少交互时间。

## SSR /元框架
-SSR/SSG/hybrid渲染首选next，除非你有手工卷SSR的理由。
-保持代码同构：保护浏览器api，避免跨请求的模块级共享可变状态，并确保存储是请求范围的。
-匹配服务器和客户端输出，以防止水合不匹配。# #可访问性
-首先使用语义HTML；添加ARIA只是为了填补真正的空白。
-确保键盘的可操作性和可见的焦点状态。
-管理焦点在路由变化和opening/closing对话框（陷阱焦点在情态）。
-提供仅图标控件可访问的名称（`aria-label`）；将标签与输入相关联。

# #安全
-永远不要用`v-html`呈现不可信的输入；如果不可避免，消毒（例如DOMPurify）。
-避免来自不可信来源的动态`:is`/`:href`/`:src`；验证url（块`javascript:`模式）。
-保密服务器端；只公开有意公开的以`VITE_`为前缀的环境变量。
—在app层应用内容安全策略和标准CSRF/XSS保护。# #测试
-将可组合组件作为普通函数进行单元测试；使用Vue测试工具/测试库进行组件测试。
测试可观察的行为和呈现的输出，而不是内部实现细节。
-模拟商店`createTestingPinia`；Stub路由器和异步边界。
-通过端到端测试（剧作家或赛普拉斯）覆盖关键用户旅程。

# #工具
-使用Vite与官方Vue插件；在CI中启用`vue-tsc`进行类型检查。
-使用ESLint （`eslint-plugin-vue`）和Prettier；启用Volar/Vue官方扩展为最好的DX。
-管理环境通过`import.meta.env`与`VITE_`前缀变量。要避免的反模式
-在同一代码库中任意混合选项API和组合API。
-直接改变道具，或破坏`reactive()`/Pinia商店没有`toRefs`/`storeToRefs`。
—使用`watch`的值应该是`computed`。
-`v-if`和`v-for`在一个元素上；使用数组索引为`:key`。
-模板内部逻辑太重；大数据上的无界深度反应性。
-跳过`onUnmounted`清理，泄漏timers/listeners。
-通过`v-html`呈现不受信任的HTML。