---
description: "Best practices for building Next.js (App Router) apps with modern caching, tooling, and server/client boundaries (aligned with Next.js 16.1.1)."
applyTo: "**/*.tsx, **/*.ts, **/*.jsx, **/*.js, **/*.css"
---
法学硕士最佳实践（2026）

最后更新：2026年1月（与Next.js16.1.1对齐）_

本文档总结了构建、构建和维护Next.js应用程序的最新、权威的最佳实践。llm和开发人员可以使用它来确保代码质量、可维护性和可伸缩性。

---

# # 1。项目结构与组织- **使用`app/`目录**（应用路由器）的所有新项目。与传统的`pages/`目录相比，最好使用它。
—**顶级文件夹：**
-`app/`-路由，布局，页面和路由处理程序
-`public/`-静态资产（图像，字体等）
-`lib/`-共享实用程序、API客户端和逻辑
-`components/`-可重用的UI组件
-`contexts/`- React上下文提供者
-`styles/`-全局和模块化样式表
-`hooks/`-自定义React钩子
-`types/`- TypeScript类型定义
**放置：**将文件（组件、样式、测试）放置在使用它们的地方附近，但避免深度嵌套结构。
—**路由组：**使用括号（如`(admin)`）对路由进行分组，不影响URL路径。
- **私人文件夹：**前缀`_`（例如，`_internal`）选择退出路由和信号实现细节。
- **功能文件夹：**对于大型应用程序，按功能分组(例如，`app/dashboard/``app/auth/`)。
- **使用`src/`**（可选）：将所有源代码放在`src/`中，与配置文件分开。# # 2。16+应用路由器最佳实践

# # # 2.1。服务器和客户端组件集成（应用路由器）

**不要在服务器组件中同时使用`next/dynamic`和`{ ssr: false }`。**这是不支持的，将导致build/runtime错误。

* *正确的方法:* *

如果你需要在服务器组件中使用客户端组件（例如，一个使用钩子、浏览器api或客户端专用库的组件），你必须：
1. 将所有仅针对客户端的logic/UI移动到专用的客户端组件中（`'use client'`位于顶部）。
2. 直接在服务器组件中导入并使用该客户端组件（不需要`next/dynamic`）。
3. 如果你需要组合多个客户端专属元素（例如，带配置文件下拉菜单的导航栏），创建一个包含所有元素的客户端组件。

* *的例子:* *```tsx
// Server Component
import DashboardNavbar from "@/components/DashboardNavbar";

export default async function DashboardPage() {
  // ...server logic...
  return (
    <>
      <DashboardNavbar /> {/* This is a Client Component */}
      {/* ...rest of server-rendered page... */}
    </>
  );
}
```
* *原因:* *

—服务器组件不能使用client-only特性或禁用SSR的动态导入。
客户端组件可以在服务器组件中渲染，但不能在服务器组件中渲染。

* *摘要:* *
始终将仅客户端UI移动到客户端组件中，并将其直接导入到服务器组件中。永远不要在服务器组件中使用`next/dynamic`和`{ ssr: false }`。

# # # 2.2。Next.js16+异步请求api（应用路由器）- **假设请求绑定的数据在服务器组件和路由处理程序中是异步的。**在Next.js16中，像`cookies()`、`headers()`和`draftMode()`这样的api在App Router中是异步的。
**注意路由道具：**`params`/`searchParams`可能是服务器组件中的承诺。更喜欢xqz9xqing它们，而不是将它们视为普通对象。
- **避免意外动态渲染：**访问请求数据（cookies/headers/searchParams）将路由选择为动态行为。有意地阅读它们，并在适当的时候将动态部分隔离在`Suspense`边界后面。

---

# # 3。组件最佳实践—**组件类型：**
- **服务器组件**（默认）：用于数据获取，繁重的逻辑和非交互式UI。
- **客户端组件：**在顶部添加`'use client'`。用于交互性、状态或浏览器api。
- **何时创建组件：**
—如果一个UI模式被重用了不止一次。
-如果页面的某个部分复杂或独立。
-如果它提高了可读性或可测试性。
- **命名约定：**
-使用`PascalCase`用于组件文件和导出（例如，`UserCard.tsx`）。
-钩子使用`camelCase`（例如，`useUser.ts`）。
-静态资产使用`snake_case`或`kebab-case`（例如，`logo_dark.svg`）。
-将上下文提供程序命名为`XyzProvider`（例如，`ThemeProvider`）。
—**文件命名：**
—将组件名称与文件名匹配。
—对于单导出文件，默认导出组件。
—对应多个组件时，使用“`index.ts`”桶状文件。
—**组件位置：**
—放置共享组件NTS在`components/`。
-将特定于路由的组件放在相关的路由文件夹中。
- * *道具:* *
-为道具使用TypeScript接口。
-首选显式的道具类型和默认值。
- * *测试:* *
-与组件（例如，`UserCard.test.tsx`）共同定位测试。# # 4。命名约定（一般）

**文件夹：**`kebab-case`（例如：`user-profile/`）
**文件：**`PascalCase`为组件，`camelCase`为utilities/hooks，`kebab-case`为静态资产
- **Variables/Functions:**`camelCase`- **Types/Interfaces:**`PascalCase`- **常数：**`UPPER_SNAKE_CASE`# # 5。API路由（路由处理程序）- **优先选择API路由而不是边缘功能**，除非你需要超低延迟或地理分布。
- **位置：**将API路由放置在`app/api/`中（例如`app/api/users/route.ts`）。
- **HTTP方法：**导出以HTTP动词命名的异步函数（`GET`，`POST`等）。
—**Request/Response:**使用Web`Request`和`Response`接口。使用`NextRequest`/`NextResponse`获得高级功能。
—**动态段：**动态API路由使用`[param]`（如`app/api/users/[id]/route.ts`）。
- **验证：**始终验证和消毒输入。使用像`zod`或`yup`这样的库。
—**错误处理：**返回适当的HTTP状态码和错误消息。
- **认证：**使用中间件或服务器端会话检查保护敏感路由。

路由处理程序使用说明（性能）**不要仅仅为了重用逻辑而从服务器组件调用自己的路由处理程序**（例如，`fetch('/api/...')`）。更倾向于将共享逻辑提取到模块中（例如，`lib/`）并直接调用它，以避免额外的服务器跳转。

# # 6。一般最佳实践- **TypeScript:**所有代码都使用TypeScript。在`tsconfig.json`中启用`strict`模式。
- **ESLint & Prettier:**强制代码风格和检查。使用官方的Next.jsESLint配置。在Next.js16中，更喜欢通过ESLint命令行运行ESLint（而不是`next lint`）。
—**环境变量：**保存在`.env.local`中。永远不要把秘密提交给版本控制。
—在Next.js16中，删除了`serverRuntimeConfig`/`publicRuntimeConfig`。使用环境变量代替。`NEXT_PUBLIC_`变量在构建时内联（在构建后更改它们不会影响已部署的构建）。
-如果你确实需要在动态上下文中对env进行运行时评估，请遵循Next.js指导（例如，在阅读`process.env`之前调用`connection()`）。
**测试：**使用Jest， React测试库或剧作家。为所有关键逻辑和组件编写测试。
**可访问性：**使用语义HTML和ARIA属性。使用屏幕阅读器进行测试。
- * *性能模式:* *
-使用内置的图像和字体优化。
-优先选择**缓存组件** (`cacheComponents`+`use cache`)而不是传统的缓存模式。
-使用悬疑和加载状态的异步数据。
-避免大型客户端包；将大部分逻辑放在服务器组件中。
- * *安全:* *
—清除所有用户输入。
—在生产环境中使用HTTPS。
—设置安全HTTP头。
-首选服务器端授权的服务器操作和路由处理程序；永远不要相信客户的输入。
- * *文档:* *
写清楚的自述文件和代码注释。
-记录公共api和组件。# # 7。缓存和重新验证（Next.js16缓存组件）

- **在应用路由器中优先选择memoization/caching**的缓存组件。
—通过`cacheComponents: true`在`next.config.*`中启用。
-使用**`use cache`指令**选择component/function进入缓存。
- **有意使用缓存标签和生命周期：**
-使用`cacheTag(...)`将缓存结果与标签关联。
-使用`cacheLife(...)`来控制缓存生存期（预置或配置的配置文件）。
- **重新验证指南：**
-大多数情况下首选`revalidateTag(tag, 'max')`（stale-while-revalidate）。
—单参数形式`revalidateTag(tag)`为legacy/deprecated.-使用`updateTag(...)`在**服务器操作**当你需要“读你的写”/即时一致性。
- **避免新代码使用`unstable_cache`**；将其视为遗留并迁移到缓存组件。

# # 8。工具更新（Next.js16）- ** turbpack是默认的开发打包器。**通过`next.config.*`中的顶级`turbopack`字段进行配置（不要使用删除的`experimental.turbo`）。
**键入路由是稳定的**通过`typedRoutes`（需要TypeScript）。

# # 9。避免不必要的示例文件

不要在主代码库中创建example/demo文件（如ModalExample.tsx），除非用户特别要求一个实际示例、故事书故事或显式文档组件。默认情况下，保持存储库整洁并以生产为中心。

# # 10。始终使用最新的文档和指南

对于每个与Next.js相关的请求，首先搜索最新的Next.js文档、指南和示例。
-使用以下工具获取和搜索文档（如果有的话）：
-`resolve_library_id`解析文档中的package/library名称。
-`get_library_docs`为最新的文件。