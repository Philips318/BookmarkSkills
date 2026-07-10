---
name: gsap-framer-scroll-animation
description: >-
  Use this skill whenever the user wants to build scroll animations, scroll effects,
  parallax, scroll-triggered reveals, pinned sections, horizontal scroll, text animations,
  or any motion tied to scroll position — in vanilla JS, React, or Next.js.
  Covers GSAP ScrollTrigger (pinning, scrubbing, snapping, timelines, horizontal scroll,
  ScrollSmoother, matchMedia) and Framer Motion / Motion v12 (useScroll, useTransform,
  useSpring, whileInView, variants). Use this skill even if the user just says
  "animate on scroll", "fade in as I scroll", "make it scroll like Apple",
  "parallax effect", "sticky section", "scroll progress bar", or "entrance animation".
  Also triggers for Copilot prompt patterns for GSAP or Framer Motion code generation.
  Pairs with the premium-frontend-ui skill for creative philosophy and design-level polish.
metadata:
  author: 'Utkarsh Patrikar'
  author_url: 'https://github.com/utkarsh232005'
---
# GSAP &帧运动-滚动动画技能

带有GitHub Copilot提示的生产级滚动动画、现成的代码配方和深度API引用。

**设计同伴：**这个技能提供了滚动驱动运动的*技术实现*。
>为*创意理念*，设计原则，和优质的美学，应该指导**如何**
>和**当**动画时，总是交叉引用**premium-front -ui**技能。
它们一起构成了一个完整的方法：premium-front -ui决定做什么和为什么做；
b>这个技能提供了**如何**。

快速库选择器

|需要|使用||---|---|
|香草JS， Webflow, Vue | **GSAP** |
|固定，水平滚动，复杂的时间线| **GSAP** |
| React /Next.js，声明式| **Framer Motion** |
|当inview入口动画| **帧运动** |
|在同一个Next.js应用程序|参见参考资料|中的说明

阅读相关的参考文件，了解完整的食谱和副驾驶提示：

- **GSAP**→`references/gsap.md`- ScrollTrigger API，所有食谱，React集成
- **帧运动**→`references/framer.md`-使用滚动，使用变换，所有的食谱

##设置（总是先做）

# # # GSAP```bash
npm install gsap
```
```js
import gsap from 'gsap';
import { ScrollTrigger } from 'gsap/ScrollTrigger';
gsap.registerPlugin(ScrollTrigger); // MUST call before any ScrollTrigger usage
```
帧运动（Motion v12, 2025）```bash
npm install motion   # new package name since mid-2025
# or: npm install framer-motion  — still works, same API
```
```js
import { motion, useScroll, useTransform, useSpring } from 'motion/react';
// legacy: import { motion } from 'framer-motion'  — also valid
```
# #工作流程

1. 解释用户的意图，以确定GSAP或Framer Motion是最合适的。
2. 请阅读`references/`中的相关参考文档，了解详细的api和模式。
3. 如果还没有安装，建议安装所需的软件包。
4. 按照请求的格式（React组件、钩子需求或普通JS），为动画结构实现脚手架。
5. 应用正确的工具（滚动与视图元素），确保可访问性选项的存在，钩子不会导致无限的重新渲染。

5种最常见的滚动模式

快速参考-完整的食谱与副驾驶提示是在参考文件。

# # # 1。进入后淡入（GSAP）```js
gsap.from('.card', {
  opacity: 0, y: 50, stagger: 0.15, duration: 0.8,
  scrollTrigger: { trigger: '.card', start: 'top 85%' }
});
```
# # # 2。进入时渐隐（帧运动）```jsx
<motion.div
  initial={{ opacity: 0, y: 40 }}
  whileInView={{ opacity: 1, y: 0 }}
  viewport={{ once: true, margin: '-80px' }}
  transition={{ duration: 0.6 }}
/>
```
# # # 3。刷洗/滚动链接（GSAP）```js
gsap.to('.hero-img', {
  scale: 1.3, opacity: 0, ease: 'none',
  scrollTrigger: { trigger: '.hero', start: 'top top', end: 'bottom top', scrub: true }
});
```
# # # 4。滚动链接（帧运动）```jsx
const { scrollYProgress } = useScroll({ target: ref, offset: ['start end', 'end start'] });
const y = useTransform(scrollYProgress, [0, 1], [0, -100]);
return <motion.div style={{ y }} />;
```
# # # 5。固定时间轴（GSAP）```js
const tl = gsap.timeline({
  scrollTrigger: { trigger: '.section', pin: true, scrub: 1, start: 'top top', end: '+=200%' }
});
tl.from('.title', { opacity: 0, y: 60 }).from('.img', { scale: 0.85 });
```
关键规则（始终适用）

- **GSAP**：在使用它之前总是调用`gsap.registerPlugin(ScrollTrigger)`- **GSAP擦洗**：总是使用`ease: 'none'`-放松感觉错误时，擦洗是活跃的
- **GSAP React**：使用`useGSAP`从`@gsap/react`，而不是普通的`useEffect`-它会自动清除ScrollTriggers
- **GSAP调试**：开发时添加`markers: true`；生产前移除
**Framer**:`useTransform`输出必须进入`motion.*`元素的`style`prop，而不是普通的div
**帧Next.js**：总是添加`'use client'`在任何文件的顶部使用运动钩子
- ** **：只动画`transform`和`opacity`-避免`width`，`height`,`box-shadow`- **可访问性**：总是检查`prefers-reduced-motion`-查看每个参考文件的模式
**高级润色**：遵循**高级前端ui**技能原则的运动定时，舒缓曲线，和约束-动画应该增强，而不是压倒

##副驾驶提示提示-给副驾驶提供完整的选择器，基本图像和滚动范围-模糊的提示产生模糊的代码
-对于GSAP，总是指定：selector，start/end字符串，无论你是想要scrub还是toggleActions
-对于Framer，总是指定：哪个钩子（usesscroll vs whileInView），偏移值，转换什么
-在询问`/fix`时粘贴准确的错误信息-副驾驶对真实错误的修复明显更好
-在副驾驶聊天中使用`@workspace`作用域，以便它读取您现有的组件结构

##参考文件

|文件|内容||---|---|
|`references/gsap.md`|完整的ScrollTrigger API参考，10个食谱，React (useGSAP), Lenis, matchMedia, accessibility |
|`references/framer.md`|完整的usesscroll / useTransform API， 8个食谱，变体，运动v12笔记，Next.js提示|

相关技能

|技能|关系||---|---|
创意理念，设计原则和美学准则-定义“何时”和“为什么”动画|