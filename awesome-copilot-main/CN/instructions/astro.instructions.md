---
description: 'Astro 7 development standards and best practices for content-driven websites'
applyTo: '**/*.astro, **/*.ts, **/*.js, **/*.md, **/*.mdx'
---
#宇航发展指南

根据内容驱动、服务器优先架构和现代最佳实践构建高质量Astro应用程序的说明。

> [!请注意)
本指南中的示例和api以Astro 7.x为目标。

##项目背景
——《Astro》x与岛屿架构和内容层API
- TypeScript的类型安全性和更好的DX与自动生成的类型
-内容驱动型网站（博客、营销、电子商务、文档）
-服务器优先渲染与选择性客户端水合
-支持多种UI框架（React, Vue, Svelte， Solid等）
-静态站点生成（SSG）默认带有可选的服务器端渲染（SSR）

##开发标准# # #架构
-拥抱岛屿架构：默认服务器渲染，选择性水合物
-使用内容集合组织内容，以实现类型安全的Markdown/MDX管理
-根据功能或内容类型构建项目以实现可伸缩性
-使用基于组件的架构，明确分离关注点
-实施渐进式增强模式
-采用多页面应用程序（MPA）方法，而不是单页面应用程序（SPA）模式

TypeScript集成
-扩展Astro的基础配置在`tsconfig.json`：```json
{
  "extends": "astro/tsconfigs/base",
  "include": [".astro/types.d.ts", "**/*"],
  "exclude": ["dist"]
}
```
-类型在`.astro/types.d.ts`中自动生成；在更改集合或配置后运行`astro sync`-用TypeScript接口定义组件道具
-利用内容集合和内容层API的自动生成类型

组件设计
-使用`.astro`组件用于静态的、服务器呈现的内容
-仅在需要交互时才导入框架组件（React, Vue, Svelte）
-遵循Astro的组件脚本结构：前文在顶部，模板在下面
-使用有意义的组件名称，遵循PascalCase约定
保持组件的重点和可组合性
-实现适当的道具验证和默认值
-编写有效的，完全封闭的HTML：编译器在未关闭的标签上错误，并且不会自动纠正无效的嵌套（例如`<p>`中的块元素）内容集合
-定义集合在`src/content.config.ts`与内容层API
-使用内置加载器：`glob()`用于基于文件的内容，`file()`用于单个数据文件
-从`astro/zod`（而不是从`astro:content`）导入`z`，首选顶级zd助手，如`z.email()`和`z.url()`—查询类型为“`getCollection()`”和“`getEntry()`”的内容
—示例集合定义：```typescript
import { defineCollection } from 'astro:content';
import { z } from 'astro/zod';
import { glob } from 'astro/loaders';

const blog = defineCollection({
  loader: glob({ pattern: '**/*.md', base: './src/content/blog' }),
  schema: z.object({
    title: z.string(),
    pubDate: z.date(),
    tags: z.array(z.string()).optional(),
  }),
});

export const collections = { blog };
```
视图转换和客户端路由
-启用`<ClientRouter />`组件在您的布局`<head>`—从`astro:transitions`导入：`import { ClientRouter } from 'astro:transitions'`-提供类似spa的导航，无需重新加载整个页面
-使用CSS和view-transition-name自定义过渡动画
-使用持久岛维护页面导航的状态
-使用`transition:persist`指令来保持组件状态

性能优化
-默认为零JavaScript -只在需要的地方添加交互性
策略性地使用客户端指令（`client:load`,`client:idle`,`client:visible`）
-实现图像和组件的延迟加载
-优化静态资产与Astro的内置优化
-利用内容层API更快的内容加载和构建
-通过避免不必要的客户端JavaScript最小化包的大小# # #样式
-默认在`.astro`组件中使用作用域样式
-在需要时实现CSS预处理（Sass, Less）
-使用CSS自定义属性的主题和设计系统
-遵循移动优先响应式设计原则
-确保可访问性与语义HTML和适当的ARIA属性
-考虑实用优先框架（顺风CSS）快速开发
Astro默认使用JSX规则去除空白（`compressHTML: 'jsx'`）；当需要一个可见的空格时，在内联元素之间添加显式的`{" "}`客户端交互性
-使用框架组件（React, Vue, Svelte）进行交互元素
-根据用户互动模式选择合适的补水策略
—在框架边界内实现状态管理
-认真处理客户端路由，维护MPA利益
使用Web组件实现与框架无关的交互性
-使用商店或自定义事件共享岛屿之间的状态

服务器孤岛
-使用`server:defer`按需渲染服务器岛，而不会阻塞页面的其余部分
-通过`slot="fallback"`为加载状态提供回退内容
—需要配置SSR适配器（按需渲染）
——例如:```astro
---
import Avatar from '../components/Avatar.astro';
---
<Avatar server:defer>
  <div slot="fallback">Loading…</div>
</Avatar>
```
# # #行动
在`src/actions/index.ts`中定义类型安全的服务器函数，并优先使用它们，而不是用于突变和表单处理的ad-hoc API路由
-用Zod模式验证输入；设置`accept: 'form'`来处理HTML表单提交
-通过`astro:actions`模块从客户端调用操作，并处理`{ data, error }`结果
——例如:```typescript
// src/actions/index.ts
import { defineAction } from 'astro:actions';
import { z } from 'astro/zod';

export const server = {
  subscribe: defineAction({
    accept: 'form',
    input: z.object({ email: z.email() }),
    handler: async ({ email }) => {
      // persist the subscription
      return { success: true };
    },
  }),
};
```
# # #会话
-使用`Astro.session`（`get`,`set`）读取和写入服务器端状态，而不是重载cookie
—需要一个配置会话存储的SSR适配器
-对购物车，flash消息和其他不应该存在于客户端的每个访问者数据有用

API路由和SSR
-创建API路由在`src/pages/api/`的动态功能
—使用正确的HTTP方法和状态码
-实现请求验证和错误处理
—启用SSR模式，满足动态内容需求
-使用中间件进行身份验证和请求处理
—安全处理环境变量SEO和元管理
-使用Astro内置的SEO组件和元标签管理
-实现适当的Open Graph和Twitter Card元数据
-自动生成站点地图更好的搜索索引
-使用语义HTML结构，以获得更好的可访问性和SEO
-为富片段实现结构化数据（JSON-LD）
-优化页面标题和描述的搜索引擎

图像优化
-使用Astro的`<Image />`组件自动优化
-通过适当的srcset生成实现响应式图像
-现代浏览器使用WebP和AVIF格式
-延迟加载图片下面的折叠
-为可访问性提供适当的alt文本
-在构建时优化映像以获得更好的性能###数据抓取
-在构建时从组件前端获取数据
—使用动态导入进行条件数据加载
-为外部API调用实现适当的错误处理
-在构建过程中缓存昂贵的操作
-使用Astro内置的自动获取TypeScript推理
-适当处理加载状态和回退