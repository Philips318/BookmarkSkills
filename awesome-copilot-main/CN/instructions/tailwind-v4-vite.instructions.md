---
description: 'Tailwind CSS v4+ installation and configuration for Vite projects using the official @tailwindcss/vite plugin'
applyTo: 'vite.config.ts, vite.config.js, **/*.css, **/*.tsx, **/*.ts, **/*.jsx, **/*.js'
---
#顺风CSS v4+安装与Vite

使用官方Vite插件安装和配置顺风CSS版本4及以上的说明。顺风CSS v4引入了一种简化的设置，在大多数情况下消除了对PostCSS配置和tailwind.config.js的需要。

在顺风CSS v4的关键变化

- **使用Vite插件时不需要PostCSS配置
- **不需要tailwind.config.js** -配置通过CSS完成
**新的@tailwindcss/vite插件**取代了基于postcss的方法
** css优先配置**使用`@theme`指令
- **自动内容检测** -无需指定内容路径

##安装步骤

###步骤1：安装依赖项

安装`tailwindcss`和`@tailwindcss/vite`插件：```bash
npm install tailwindcss @tailwindcss/vite
```
###步骤2：配置Vite插件

将`@tailwindcss/vite`插件添加到您的Vite配置文件中：```typescript
// vite.config.ts
import { defineConfig } from 'vite'
import tailwindcss from '@tailwindcss/vite'

export default defineConfig({
  plugins: [
    tailwindcss(),
  ],
})
```
对于使用Vite的React项目：```typescript
// vite.config.ts
import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import tailwindcss from '@tailwindcss/vite'

export default defineConfig({
  plugins: [
    react(),
    tailwindcss(),
  ],
})
```
步骤3：导入顺风CSS

添加顺风CSS导入到你的主CSS文件（例如，`src/index.css`或`src/App.css`）：```css
@import "tailwindcss";
```
###步骤4：验证入口点的CSS导入

确保你的主CSS文件被导入到你的应用程序入口点：```typescript
// src/main.tsx or src/main.ts
import './index.css'
```
###步骤5：启动开发服务器

运行开发服务器验证安装：```bash
npm run dev
```
在顺风v4中不要做的事情

###不创建tailwind.config.jsTailwind v4使用css优先配置。除非有特定的遗留需求，否则不要创建`tailwind.config.js`文件。```javascript
// ❌ NOT NEEDED in Tailwind v4
module.exports = {
  content: ['./src/**/*.{js,ts,jsx,tsx}'],
  theme: {
    extend: {},
  },
  plugins: [],
}
```
###不要为Tailwind创建postcss.config.js使用`@tailwindcss/vite`插件时，不需要为Tailwind配置PostCSS。```javascript
// ❌ NOT NEEDED when using @tailwindcss/vite
module.exports = {
  plugins: {
    tailwindcss: {},
    autoprefixer: {},
  },
}
```
不要使用旧指令

旧的`@tailwind`指令被一个导入取代：```css
/* ❌ OLD - Do not use in Tailwind v4 */
@tailwind base;
@tailwind components;
@tailwind utilities;

/* ✅ NEW - Use this in Tailwind v4 */
@import "tailwindcss";
```
CSS-First配置（Tailwind v4）

自定义主题配置

在你的CSS中使用`@theme`指令来定制你的设计令牌：```css
@import "tailwindcss";

@theme {
  --color-primary: #3b82f6;
  --color-secondary: #64748b;
  --font-sans: 'Inter', system-ui, sans-serif;
  --radius-lg: 0.75rem;
}
```
添加自定义实用程序

直接在CSS中定义自定义实用程序：```css
@import "tailwindcss";

@utility content-auto {
  content-visibility: auto;
}

@utility scrollbar-hidden {
  scrollbar-width: none;
  &::-webkit-scrollbar {
    display: none;
  }
}
```
添加自定义变体

在CSS中定义自定义变量：```css
@import "tailwindcss";

@variant hocus (&:hover, &:focus);
@variant group-hocus (:merge(.group):hover &, :merge(.group):focus &);
```
##验证清单

安装完成后，请验证：

—[]`tailwindcss`和`@tailwindcss/vite`依赖于`package.json`- []`vite.config.ts`包含`tailwindcss()`插件
—[]主CSS文件包含`@import "tailwindcss";`—在应用入口点导入了[]CSS文件
-[]开发服务器正常运行
[]顺风工具类（如`text-blue-500`，`p-4`）正确渲染

##使用示例

用一个简单的组件测试安装情况：```tsx
export function TestComponent() {
  return (
    <div className="min-h-screen bg-gray-100 flex items-center justify-center">
      <h1 className="text-3xl font-bold text-blue-600 underline">
        Hello, Tailwind CSS v4!
      </h1>
    </div>
  )
}
```
# #故障排除

样式不适用

1. 验证CSS导入语句是`@import "tailwindcss";`（不是旧的指令）
2. 确保CSS文件导入到您的入口点
3. 检查Vite配置是否包含`tailwindcss()`插件
4. 清除虚拟缓存：`rm -rf node_modules/.vite && npm run dev`插件未找到错误

如果你看到“Cannot find module '@tailwindcss/vite'”：```bash
npm install @tailwindcss/vite
```
TypeScript错误

如果TypeScript找不到Vite插件的类型，确保你导入的是正确的：```typescript
import tailwindcss from '@tailwindcss/vite'
```
##从顺风v3迁移

如果从Tailwind v3迁移：

1. 删除`tailwind.config.js`（移动自定义到CSS`@theme`）
2. 移除`postcss.config.js`（如果只用于顺风）
3. 卸载旧软件包：`npm uninstall postcss autoprefixer`4. 安装新软件包：`npm install tailwindcss @tailwindcss/vite`5. 将`@tailwind`指令替换为`@import "tailwindcss";`6. 更新Vite配置以使用`@tailwindcss/vite`插件

# #参考

—官方文档：https://tailwindcss.com/docs/installation/using-vite—顺风CSS v4升级指南：https://tailwindcss.com/docs/upgrade-guide