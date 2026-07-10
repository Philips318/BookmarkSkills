特定于框架的修复指南

本文档解释了每种框架和样式方法的特定修复技术。

---

纯CSS / SCSS

修复布局溢出```css
/* Before: Overflow occurs */
.container {
  width: 100%;
}

/* After: Control overflow */
.container {
  width: 100%;
  max-width: 100%;
  overflow-x: hidden;
}
```
文本剪切预防```css
/* Single line truncation */
.text-truncate {
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

/* Multi-line truncation */
.text-clamp {
  display: -webkit-box;
  -webkit-line-clamp: 3;
  -webkit-box-orient: vertical;
  overflow: hidden;
}

/* Word wrapping */
.text-wrap {
  word-wrap: break-word;
  overflow-wrap: break-word;
  hyphens: auto;
}
```
###空格统一```css
/* Unify spacing with CSS custom properties */
:root {
  --spacing-xs: 0.25rem;
  --spacing-sm: 0.5rem;
  --spacing-md: 1rem;
  --spacing-lg: 1.5rem;
  --spacing-xl: 2rem;
}

.card {
  padding: var(--spacing-md);
  margin-bottom: var(--spacing-lg);
}
```
提高对比度```css
/* Before: Insufficient contrast */
.text {
  color: #999999;
  background-color: #ffffff;
}

/* After: Meets WCAG AA standards */
.text {
  color: #595959; /* Contrast ratio 7:1 */
  background-color: #ffffff;
}
```
---

##顺风CSS

布局修复```jsx
{/* Before: Overflow */}
<div className="w-full">
  <img src="..." />
</div>

{/* After: Overflow control */}
<div className="w-full max-w-full overflow-hidden">
  <img src="..." className="w-full h-auto object-contain" />
</div>
```
文本剪切预防```jsx
{/* Single line truncation */}
<p className="truncate">Long text...</p>

{/* Multi-line truncation */}
<p className="line-clamp-3">Long text...</p>

{/* Allow wrapping */}
<p className="break-words">Long text...</p>
```
响应式支持```jsx
{/* Mobile-first responsive */}
<div className="
  flex flex-col gap-4
  md:flex-row md:gap-6
  lg:gap-8
">
  <div className="w-full md:w-1/2 lg:w-1/3">
    Content
  </div>
</div>
```
###间距统一（顺风配置）```javascript
// tailwind.config.js
module.exports = {
  theme: {
    extend: {
      spacing: {
        '18': '4.5rem',
        '22': '5.5rem',
      },
    },
  },
}
```
可访问性改进```jsx
{/* Add focus state */}
<button className="
  bg-blue-500 text-white
  hover:bg-blue-600
  focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-2
">
  Button
</button>

{/* Improve contrast */}
<p className="text-gray-700 bg-white"> {/* Changed from text-gray-500 */}
  Readable text
</p>
```
---

# React + CSS模块

修复了模块范围```css
/* Component.module.css */

/* Before */
.container {
  display: flex;
}

/* After: Add overflow control */
.container {
  display: flex;
  flex-wrap: wrap;
  overflow: hidden;
  max-width: 100%;
}
```
组件端修复```jsx
// Component.jsx
import styles from './Component.module.css';

// Before
<div className={styles.container}>

// After: Add conditional class
<div className={`${styles.container} ${isOverflow ? styles.overflow : ''}`}>
```
---

## style -components / Emotion

样式修复```jsx
// Before
const Container = styled.div`
  width: 100%;
`;

// After
const Container = styled.div`
  width: 100%;
  max-width: 100%;
  overflow-x: hidden;
  
  @media (max-width: 768px) {
    padding: 1rem;
  }
`;
```
响应式支持```jsx
const Card = styled.div`
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: 1.5rem;
  
  @media (max-width: 1024px) {
    grid-template-columns: repeat(2, 1fr);
  }
  
  @media (max-width: 640px) {
    grid-template-columns: 1fr;
    gap: 1rem;
  }
`;
```
与主题的一致性```jsx
// theme.js
export const theme = {
  colors: {
    primary: '#2563eb',
    text: '#1f2937',
    textLight: '#4b5563', // Improved contrast
  },
  spacing: {
    sm: '0.5rem',
    md: '1rem',
    lg: '1.5rem',
  },
};

// Usage
const Text = styled.p`
  color: ${({ theme }) => theme.colors.text};
  margin-bottom: ${({ theme }) => theme.spacing.md};
`;
```
---

## Vue（限定样式）

修复作用域样式```vue
<template>
  <div class="container">
    <p class="text">Content</p>
  </div>
</template>

<style scoped>
/* Applied only to this component */
.container {
  max-width: 100%;
  overflow: hidden;
}

.text {
  /* Fix: Improve contrast */
  color: #374151; /* Was: #9ca3af */
}

/* Responsive */
@media (max-width: 768px) {
  .container {
    padding: 1rem;
  }
}
</style>
```
深度选择器（影响子组件）```vue
<style scoped>
/* Override child component styles (use cautiously) */
:deep(.child-class) {
  margin-bottom: 1rem;
}
</style>
```
---

##Next.js/ App路由器

全局样式修复```css
/* app/globals.css */
:root {
  --foreground: #171717;
  --background: #ffffff;
}

/* Prevent layout overflow */
html, body {
  max-width: 100vw;
  overflow-x: hidden;
}

/* Prevent image overflow */
img {
  max-width: 100%;
  height: auto;
}
```
修复了布局组件```tsx
// app/layout.tsx
export default function RootLayout({ children }) {
  return (
    <html lang="en">
      <body className="min-h-screen flex flex-col">
        <header className="sticky top-0 z-50">
          {/* Header */}
        </header>
        <main className="flex-1 container mx-auto px-4 py-8">
          {children}
        </main>
        <footer>
          {/* Footer */}
        </footer>
      </body>
    </html>
  );
}
```
---

##常见模式

修复Flexbox布局问题```css
/* Before: Items overflow */
.flex-container {
  display: flex;
  gap: 1rem;
}

/* After: Wrap and size control */
.flex-container {
  display: flex;
  flex-wrap: wrap;
  gap: 1rem;
}

.flex-item {
  flex: 1 1 300px; /* grow, shrink, basis */
  min-width: 0; /* Prevent flexbox overflow issues */
}
```
修复网格布局问题```css
/* Before: Fixed column count */
.grid-container {
  display: grid;
  grid-template-columns: repeat(4, 1fr);
}

/* After: Auto-adjust */
.grid-container {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
  gap: 1.5rem;
}
```
组织z-index```css
/* Systematize z-index */
:root {
  --z-dropdown: 100;
  --z-sticky: 200;
  --z-modal-backdrop: 300;
  --z-modal: 400;
  --z-tooltip: 500;
}

.modal {
  z-index: var(--z-modal);
}
```
添加焦点状态```css
/* Add focus state to all interactive elements */
button:focus-visible,
a:focus-visible,
input:focus-visible,
select:focus-visible,
textarea:focus-visible {
  outline: 2px solid #2563eb;
  outline-offset: 2px;
}

/* Customize focus ring */
.custom-focus:focus-visible {
  outline: none;
  box-shadow: 0 0 0 3px rgba(37, 99, 235, 0.5);
}
```
---

##调试技术

可视化元素边界```css
/* Use only during development */
* {
  outline: 1px solid red !important;
}
```
###检测溢出```javascript
// Run in console to detect overflow elements
document.querySelectorAll('*').forEach(el => {
  if (el.scrollWidth > el.clientWidth) {
    console.log('Horizontal overflow:', el);
  }
  if (el.scrollHeight > el.clientHeight) {
    console.log('Vertical overflow:', el);
  }
});
```
检查对比度```javascript
// Use Chrome DevTools Lighthouse or axe DevTools
// Or check at the following site:
// https://webaim.org/resources/contrastchecker/
```
