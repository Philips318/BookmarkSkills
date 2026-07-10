---
description: 'Svelte 5 and SvelteKit 2 development standards and best practices for component-based user interfaces and full-stack applications'
applyTo: '**/*.svelte, **/*.ts, **/*.js, **/*.css, **/*.scss, **/*.json'
---
# svelte5和SvelteKit开发说明

使用基于runes的现代反应性、TypeScript和性能优化构建高质量的Svelte 5和SvelteKit应用程序的说明。

> [!请注意)
本指南中的示例和api以Svelte 5为目标。和SvelteKit 2.x。被标记为实验性的特性（`await`表达式和远程函数）需要选择加入配置标志，并且在稳定之前可能会更改。

##项目背景
-苗条5。X带有符文系统（$state, $derived, $effect, $props, $bindable）
- SvelteKit基于文件路由的全栈应用程序
- TypeScript的类型安全性和更好的开发体验
-组件范围的样式与CSS自定义属性
-渐进式增强和性能优先的方法
-现代构建工具（Vite）与优化

##核心概念# # #架构
-所有的反应都使用5号符文系统，而不是传统的商店
-按功能或领域组织组件以实现可伸缩性
-将表示组件与逻辑繁重的组件分开
将可重用的逻辑提取为可组合的函数
-使用插槽和片段实现适当的组件组合
-使用SvelteKit的基于文件的路由与适当的加载函数组件设计
-遵循部件单一责任原则
—默认使用带有符文语法的`<script lang="ts">`-保持组件较小并专注于一个关注点
用TypeScript注解实现正确的道具验证
-在组件内使用`{#snippet}`块用于可重用的模板逻辑
-使用插槽进行组件组合和内容投影
-传递`children`片段以实现灵活的父子组合
-设计可测试和可重用的组件
-对于DOM交互和第三方库集成，首选附件（`{@attach}`, Svelte 5.29+）而不是动作(`use:`) -附件对状态和组合做出干净的反应
遵循命名约定：组件用PascalCase命名，函数和变量用camelCase命名
-用JSDoc注释记录复杂的组件和逻辑

反应性和状态5个符文系统
—使用`$state()`进行响应式本地状态管理
-实现`$derived()`计算值和昂贵的计算
-使用`$derived.by()`的复杂计算超越简单的表达式
-谨慎使用`$effect()`-更倾向于使用`$derived`或函数绑定进行状态同步
—实现`$effect.pre()`在DOM更新之前运行代码
-使用`untrack()`防止无限循环时，reading/writing相同的状态在效果
用`$props()`定义组件道具，用TypeScript注释定义解构
—使用`$bindable()`进行组件间的双向数据绑定
-当绑定需要派生或验证值时，使用函数绑定（`bind:value={() => value, (v) => (value = v)}`）
-从遗留存储迁移到符文以获得更好的性能
-直接覆盖乐观UI模式的派生值（Svelte 5.25+）状态管理
—本地组件状态使用`$state()`-使用`createContext()`helper在原始`setContext`/`getContext`上实现类型安全上下文
-使用上下文API在组件树中共享响应状态
—避免为SSR使用全局`$state`模块—使用上下文来防止交叉请求数据泄漏
-从`$app/state`读取SvelteKit应用程序和导航状态（`page`,`navigating`,`updated`）；`$app/stores`是SvelteKit < 2.12的遗留等效版本
—对于复杂的数据结构，保持状态规范化
-计算值首选`$derived()`，而不是`$effect()`-为客户端数据实现适当的状态持久化效果最佳实践
- **避免**使用`$effect()`同步状态，请使用`$derived()`** ** ** *使用`$effect()`的副作用：分析，日志记录，DOM操作
- **做**返回清理功能的效果，以适当的拆除
-当代码必须在DOM更新之前运行时，使用`$effect.pre()`（例如，滚动位置）
-在组件生命周期之外使用`$effect.root()`手动控制效果
-使用`untrack()`读取状态，而不会在效果中创建依赖关系
-记住：异步代码在效果不跟踪`await`之后的依赖关系

SvelteKit模式

路由和布局
-使用`+page.svelte`页面组件与适当的SEO
-实现`+layout.svelte`共享布局和导航
-使用SvelteKit的基于文件的系统处理路由数据加载和突变
—使用`+page.server.ts`进行服务器端数据加载和API调用
-在`+page.server.ts`中实现表单动作，用于数据突变
—使用`+server.ts`作为API端点和服务器端逻辑
-使用SvelteKit的加载函数用于服务器端和通用数据获取
—实现正确的加载、错误和成功状态
-处理流数据与承诺在服务器负载函数
—缓存管理使用“`invalidate()`”和“`invalidateAll()`”
-实现乐观的更新，以更好的用户体验
—妥善处理离线场景和网络故障远程功能（实验性）
-使用远程函数（SvelteKit 2.27+）的类型安全的客户端-服务器调用总是在服务器上运行；在`.remote.ts`文件中将它们定义为`query`、`form`、`command`或`prerender`中的一种
—在“`svelte.config.js`”中输入“`kit.experimental.remoteFunctions`”和“`compilerOptions.experimental.async`”
-使用`query`读取数据，并直接在标记中使用`await getPosts()`进行解析```ts
// src/routes/blog/data.remote.ts
import { query } from '$app/server';
import * as db from '$lib/server/database';

export const getPosts = query(async () => {
  return await db.sql`SELECT title, slug FROM post ORDER BY published_at DESC`;
});
```
—远程文件可以导入`$lib/server`模块用于秘密和数据库访问，但不能位于`src/lib/server`中

表单和验证
-使用SvelteKit的表单操作进行服务器端表单处理
-实现渐进式增强`use:enhance`-使用`bind:value`控制表单输入
-验证客户端和服务器端数据
—处理文件上传和复杂表单场景
-通过标签和ARIA属性实现适当的可访问性

UI和样式

# # #样式
-对`<style>`块使用组件作用域样式
-实现CSS自定义属性的主题和设计系统
-使用`class:`指令进行条件样式
-遵循BEM或实用程序优先的CSS约定
-采用移动优先的方法实施响应式设计
-使用`:global()`为真正的全局风格###过渡和动画
-使用`transition:`指令enter/exit动画（淡出，滑动，缩放，飞行）
-使用`in:`和`out:`单独的enter/exit过渡
-实现`animate:`指令与`flip`平滑列表重新排序
-为品牌运动设计创建自定义过渡
-使用`|local`修饰符仅在直接更改时触发过渡
-结合过渡与关键的`{#each}`块列表动画

TypeScript集成
—在`tsconfig.json`中启用严格模式，以获得最大的类型安全性
-用TypeScript注释道具：`let { name }: { name: string } = $props()`-类型事件处理程序，refs和SvelteKit生成的类型
-对可重用组件使用泛型类型
-利用SvelteKit生成的`$types.ts`文件
-使用`svelte-check`进行适当的类型检查
在可能的情况下使用类型推断来减少样板文件

代码质量性能优化
-使用关键的`{#each}`块高效的列表渲染
-实现惰性加载动态`import()`；在Svelte 5中，组件默认是动态的——将导入的组件赋值给一个大写的变量，并呈现`<Component />`（在符文模式下不再需要`<svelte:component>`）
—对于昂贵的计算，使用`$derived()`，避免不必要的重新计算
—对于需要多条语句的复杂派生值，使用`$derived.by()`-避免使用`$effect()`作为派生状态-它比`$derived()`效率低
-利用SvelteKit的自动代码分割和预加载
-通过摇树和适当的导入优化包的大小
-在抽象中使用`$effect.tracking()`来有条件地创建响应式侦听器错误处理
-实现`+error.svelte`页面的路由级错误边界
-使用`<svelte:boundary>`（Svelte 5.3+）在组件级别包含渲染和效果错误，为备用UI提供`failed`片段（与`reset`一起）或`onerror`处理程序
-在加载函数和表单操作中使用try/catch块
-提供有意义的错误消息和回退UI
—正确记录错误，以便调试
-处理表单中的验证错误，并提供适当的用户反馈
-使用SvelteKit的`error()`和`redirect()`助手进行适当的响应
-使用实验性`await`语法（Svelte 5.36+，通过`experimental.async`选择加入），显示第一次渲染UI与`<svelte:boundary>``pending`片段和后来加载状态与`$effect.pending()`# # #安全
—对用户输入进行消毒，防止跨站攻击
-谨慎使用`@html`指令并验证HTML内容
-验证和清理加载函数和表单操作中的数据# # #可访问性
-使用语义HTML元素和适当的标题层次结构
-为所有交互元素实现键盘导航
-提供适当的ARIA标签和描述
确保颜色对比符合WCAG的要求
-对动态内容进行焦点管理