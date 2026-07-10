# Framer Motion (Motion v12) -完全参考

> Framer Motion在2025年中期更名为**Motion**。npm包现在是`motion`，
>导入路径为`motion/react`。所有api都是相同的。`framer-motion`仍然有效。##目录
1. [Package & Import Paths]（# Package——Import - Paths）
2. [两种类型的滚动动画]（# Two - Types -of- Scroll - Animation）
3. [useScroll - Options - Reference]（# useScroll - Options - Reference）
4. [useTransform - Full - Reference]（# useTransform - Full - Reference）
5. [usspring for Smoothing]（# usspring -for- Smoothing）
6. [食谱与副驾驶提示]（# Recipes -with- Copilot - Prompts）
-滚动进度条
-可重复使用的ScrollReveal包装
-视差图层
-水平滚动部分
-图像显示与clipPath
-滚动链接导航条（hide/show）
-交错的卡片网格
- 3D倾斜滚动
7. （# variables - Pattern -for- Stagger）
8. [运动值事件]（# Motion - Value - Events）
9. [Next.js& App Router Notes]（#nextjs—App - Router - Notes）
10. (可访问性)(#可访问性)
11. [常见的副驾驶陷阱]（# Common - Copilot - traps）

---##包和导入路径```bash
npm install motion          # recommended (renamed 2025)
npm install framer-motion   # still works — same API
```

```js
// Recommended (Motion v12+)
import { motion, useScroll, useTransform, useSpring, useMotionValueEvent } from 'motion/react';

// Legacy — still valid
import { motion, useScroll, useTransform } from 'framer-motion';
```
**运动v12新功能（2025）：**
-硬件加速滚动通过浏览器ScrollTimeline API
-`useScroll`和`scroll()`现在默认为gpu加速
-新的颜色类型：`oklch`，`oklab`，`color-mix`可直接动画
-全React 19 +并发渲染支持

---

两种类型的滚动动画

滚动触发（当元素进入viewport时触发）```jsx
<motion.div
  initial={{ opacity: 0, y: 50 }}
  whileInView={{ opacity: 1, y: 0 }}
  viewport={{ once: true, margin: '-80px' }}
  transition={{ duration: 0.6, ease: [0.21, 0.47, 0.32, 0.98] }}
>
  Content
</motion.div>
```
`viewport.margin`-负值在元素完全进入视图之前触发动画。`viewport.once`-`true`表示动画一次，永远不要反转。

滚动链接（连续的，与滚动位置绑定）```jsx
const { scrollYProgress } = useScroll();
const opacity = useTransform(scrollYProgress, [0, 1], [0, 1]);
return <motion.div style={{ opacity }}>Content</motion.div>;
```
在每个滚动帧上更新值-必须使用`style`prop，而不是`animate`。

---

## usesscroll -选项参考```js
const {
  scrollX,          // Absolute horizontal scroll (pixels)
  scrollY,          // Absolute vertical scroll (pixels)
  scrollXProgress,  // Horizontal progress 0→1 between offsets
  scrollYProgress,  // Vertical progress 0→1 between offsets
} = useScroll({
  // Track a scrollable element instead of the viewport
  container: containerRef,

  // Track an element's position within the container
  target: targetRef,

  // Define when tracking starts and ends
  // Format: ["target position container position", "target position container position"]
  offset: ['start end', 'end start'],
  // Common offset pairs:
  // ['start end', 'end start']    = track while element is anywhere in view
  // ['start end', 'end end']      = track from element entering to bottom of page
  // ['start start', 'end start']  = track while element exits top
  // ['center center', 'end start']= track from center-center to exit

  // Update when content size changes (small perf cost, false by default)
  trackContentSize: false,
});
```
**偏移字符串值：**
-`start`=`0`=top/leftedge
-`center`=`0.5`= middle
-`end`=`1`=bottom/rightedge
-数字0-1也可以：`[0, 1]`=`['start', 'end']`---

##使用变换-完全引用```js
// Map a motion value from one range to another
const y = useTransform(scrollYProgress, [0, 1], [0, -200]);

// Multi-stop interpolation
const opacity = useTransform(
  scrollYProgress,
  [0, 0.2, 0.8, 1],
  [0, 1, 1, 0]
);

// Non-numeric values (colors, strings)
const color = useTransform(
  scrollYProgress,
  [0, 0.5, 1],
  ['#6366f1', '#ec4899', '#f97316']
);

// CSS string values
const clipPath = useTransform(
  scrollYProgress,
  [0, 1],
  ['inset(0% 100% 0% 0%)', 'inset(0% 0% 0% 0%)']
);

// Disable clamping (allow values outside output range)
const y = useTransform(scrollYProgress, [0, 1], [0, -200], { clamp: false });

// Transform from multiple inputs
const combined = useTransform(
  [scrollX, scrollY],
  ([x, y]) => Math.sqrt(x * x + y * y)
);
```
**规则：**`useTransform`输出为`MotionValue`。它必须进入`motion.*`元素的`style`属性。普通的`<div style={{ y }}>`不能工作-必须是`<motion.div style={{ y }}>`。

---

##使用spring平滑

在`useSpring`中包装任何MotionValue以添加弹簧物理-非常适合感觉活跃的进度条。```js
const { scrollYProgress } = useScroll();

const smooth = useSpring(scrollYProgress, {
  stiffness: 100,   // Higher = faster/snappier response
  damping: 30,      // Higher = less bounce
  restDelta: 0.001  // Precision threshold for stopping
});

return <motion.div style={{ scaleX: smooth }} />;
```
对于一个微妙的延迟（不是物理），使用`useTransform`和`clamp: false`和一个宽松的范围代替。

---

食谱与副驾驶提示

# # # 1。滚动进度条

**副驾驶聊天提示：**```
Framer Motion: fixed scroll progress bar at top of page.
useScroll for page scroll progress, useSpring to smooth scaleX.
stiffness 100, damping 30. Grows left to right.
```

```tsx
'use client';
import { useScroll, useSpring, motion } from 'motion/react';

export function ScrollProgressBar() {
  const { scrollYProgress } = useScroll();
  const scaleX = useSpring(scrollYProgress, {
    stiffness: 100, damping: 30, restDelta: 0.001,
  });

  return (
    <motion.div
      style={{ scaleX }}
      className="fixed top-0 left-0 right-0 h-1 bg-indigo-500 origin-left z-50"
    />
  );
}
```
---

# # # 2。可重用的ScrollReveal包装器

**副驾驶聊天提示：**```
Framer Motion: reusable ScrollReveal component that wraps children with 
fade-in-up entrance animation using whileInView. Props: delay (default 0), 
duration (default 0.6), once (default true). viewport margin -80px.
TypeScript. 'use client'.
```

```tsx
'use client';
import { motion } from 'motion/react';

interface ScrollRevealProps {
  children: React.ReactNode;
  delay?: number;
  duration?: number;
  once?: boolean;
  className?: string;
}

export function ScrollReveal({
  children, delay = 0, duration = 0.6, once = true, className
}: ScrollRevealProps) {
  return (
    <motion.div
      initial={{ opacity: 0, y: 40 }}
      whileInView={{ opacity: 1, y: 0 }}
      viewport={{ once, margin: '-80px' }}
      transition={{ duration, delay, ease: [0.21, 0.47, 0.32, 0.98] }}
      className={className}
    >
      {children}
    </motion.div>
  );
}

// Usage:
// <ScrollReveal delay={0.2}><h2>Section Title</h2></ScrollReveal>
```
---

# # # 3。视差层

**副驾驶聊天提示：**```
Framer Motion parallax section: background moves y from 0% to 30% (slow),
foreground text moves y from 50 to -50px (fast). 
Both use target ref with offset ['start end', 'end start'].
Fade out at top and bottom using opacity useTransform [0, 0.3, 0.7, 1] → [0,1,1,0].
```

```tsx
'use client';
import { useRef } from 'react';
import { motion, useScroll, useTransform } from 'motion/react';

export function ParallaxSection() {
  const ref = useRef<HTMLDivElement>(null);
  const { scrollYProgress } = useScroll({
    target: ref,
    offset: ['start end', 'end start'],
  });

  const backgroundY = useTransform(scrollYProgress, [0, 1], ['0%', '30%']);
  const textY        = useTransform(scrollYProgress, [0, 1], [50, -50]);
  const opacity      = useTransform(scrollYProgress, [0, 0.3, 0.7, 1], [0, 1, 1, 0]);

  return (
    <section ref={ref} className="relative h-screen overflow-hidden flex items-center justify-center">
      <motion.div
        className="absolute inset-0 bg-cover bg-center"
        style={{ backgroundImage: 'url(/hero-bg.jpg)', y: backgroundY, scale: 1.2 }}
      />
      <motion.div style={{ y: textY, opacity }} className="relative z-10 text-center text-white">
        <h2 className="text-6xl font-bold">Parallax Title</h2>
        <p className="text-xl mt-4">Scrolls at a different speed</p>
      </motion.div>
    </section>
  );
}
```
---

# # # 4。水平滚动截面

**副驾驶聊天提示：**```
Framer Motion horizontal scroll: 4 cards scroll horizontally as user scrolls vertically.
Outer container ref height 300vh controls speed (sticky pattern).
useScroll tracks outer container, useTransform maps scrollYProgress to x '0%' → '-75%'.
```

```tsx
'use client';
import { useRef } from 'react';
import { motion, useScroll, useTransform } from 'motion/react';

const cards = [
  { id: 1, title: 'Card One',   color: 'bg-indigo-500' },
  { id: 2, title: 'Card Two',   color: 'bg-pink-500'   },
  { id: 3, title: 'Card Three', color: 'bg-amber-500'  },
  { id: 4, title: 'Card Four',  color: 'bg-teal-500'   },
];

export function HorizontalScroll() {
  const containerRef = useRef<HTMLDivElement>(null);
  const { scrollYProgress } = useScroll({
    target: containerRef,
    offset: ['start start', 'end end'],
  });

  const x = useTransform(scrollYProgress, [0, 1], ['0%', '-75%']);

  return (
    <div ref={containerRef} className="relative h-[300vh]">
      <div className="sticky top-0 h-screen overflow-hidden">
        <motion.div
          style={{ x, width: `${cards.length * 100}vw` }}
          className="flex gap-6 h-full items-center px-8"
        >
          {cards.map(card => (
            <div
              key={card.id}
              className={`${card.color} w-screen h-[70vh] rounded-2xl flex items-center justify-center flex-shrink-0`}
            >
              <h3 className="text-white text-4xl font-bold">{card.title}</h3>
            </div>
          ))}
        </motion.div>
      </div>
    </div>
  );
}
```
---

# # # 5。图像显示与clipPath

**副驾驶聊天提示：**```
Framer Motion: image reveals left to right as it scrolls into view.
useScroll target ref, offset ['start end', 'center center'].
useTransform clipPath from 'inset(0% 100% 0% 0%)' to 'inset(0% 0% 0% 0%)'.
Also scale from 1.15 to 1.
```

```tsx
'use client';
import { useRef } from 'react';
import { motion, useScroll, useTransform } from 'motion/react';

export function ImageReveal({ src, alt }: { src: string; alt: string }) {
  const ref = useRef<HTMLDivElement>(null);
  const { scrollYProgress } = useScroll({
    target: ref,
    offset: ['start end', 'center center'],
  });

  const clipPath = useTransform(
    scrollYProgress,
    [0, 1],
    ['inset(0% 100% 0% 0%)', 'inset(0% 0% 0% 0%)']
  );
  const scale = useTransform(scrollYProgress, [0, 1], [1.15, 1]);

  return (
    <div ref={ref} className="overflow-hidden rounded-xl">
      <motion.img
        src={src} alt={alt}
        style={{ clipPath, scale }}
        className="w-full h-full object-cover"
      />
    </div>
  );
}
```
---

# # # 6。滚动链接的导航条（向下滚动隐藏）

**副驾驶聊天提示：**```
Framer Motion navbar: transparent when at top, white with shadow after 80px.
Hide by sliding up when scrolling down, reveal when scrolling up.
Use useScroll, useMotionValueEvent to detect direction.
Animate y, backgroundColor, boxShadow with motion.nav.
```

```tsx
'use client';
import { useRef, useState } from 'react';
import { motion, useScroll, useMotionValueEvent } from 'motion/react';

export function Navbar() {
  const { scrollY } = useScroll();
  const [scrolled, setScrolled] = useState(false);
  const [hidden,   setHidden]   = useState(false);
  const prevRef = useRef(0);

  useMotionValueEvent(scrollY, 'change', latest => {
    const nextScrolled = latest > 80;
    const nextHidden = latest > prevRef.current && latest > 200;
    setScrolled(current => (current === nextScrolled ? current : nextScrolled));
    setHidden(current => (current === nextHidden ? current : nextHidden));
    prevRef.current = latest;
  });

  return (
    <motion.nav
      animate={{
        y: hidden ? -80 : 0,
        backgroundColor: scrolled ? 'rgba(255,255,255,0.95)' : 'rgba(255,255,255,0)',
        boxShadow: scrolled ? '0 1px 24px rgba(0,0,0,0.08)' : 'none',
      }}
      transition={{ duration: 0.3, ease: 'easeInOut' }}
      className="fixed top-0 left-0 right-0 z-50 backdrop-blur-sm"
    >
      {/* nav links */}
    </motion.nav>
  );
}
```
---

# # # 7。交错卡片网格

**副驾驶聊天提示：**```
Framer Motion: card grid with stagger entrance. Use variants: 
container has staggerChildren 0.1, delayChildren 0.2.
Each card: hidden (opacity 0, y 40, scale 0.96) → visible (opacity 1, y 0, scale 1).
Trigger with whileInView on the container. Once.
```

```tsx
'use client';
import { motion } from 'motion/react';

const containerVariants = {
  hidden: {},
  visible: {
    transition: { staggerChildren: 0.1, delayChildren: 0.2 }
  }
};

const cardVariants = {
  hidden:  { opacity: 0, y: 40, scale: 0.96 },
  visible: {
    opacity: 1, y: 0, scale: 1,
    transition: { duration: 0.5, ease: [0.21, 0.47, 0.32, 0.98] }
  }
};

export function CardGrid({ cards }: { cards: { id: number; title: string }[] }) {
  return (
    <motion.div
      variants={containerVariants}
      initial="hidden"
      whileInView="visible"
      viewport={{ once: true, margin: '-50px' }}
      className="grid grid-cols-3 gap-6"
    >
      {cards.map(card => (
        <motion.div key={card.id} variants={cardVariants}
          className="bg-white rounded-xl p-6 shadow-sm border"
        >
          <h3>{card.title}</h3>
        </motion.div>
      ))}
    </motion.div>
  );
}
```
---

# # # 8。滚动时的3D倾斜

**副驾驶聊天提示：**```
Framer Motion: 3D perspective card that rotates on X axis as it scrolls through view.
rotateX 15→0→-15, scale 0.9→1→0.9, opacity 0→1→0.
Target ref with offset ['start end', 'end start']. Wrap in perspective container.
```

```tsx
'use client';
import { useRef } from 'react';
import { motion, useScroll, useTransform } from 'motion/react';

export function TiltCard({ children }: { children: React.ReactNode }) {
  const ref = useRef<HTMLDivElement>(null);
  const { scrollYProgress } = useScroll({
    target: ref,
    offset: ['start end', 'end start'],
  });

  const rotateX = useTransform(scrollYProgress, [0, 0.5, 1], [15,  0, -15]);
  const scale   = useTransform(scrollYProgress, [0, 0.5, 1], [0.9, 1,  0.9]);
  const opacity = useTransform(scrollYProgress, [0, 0.2, 0.8, 1], [0, 1, 1, 0]);

  return (
    <div ref={ref} style={{ perspective: '1000px' }}>
      <motion.div
        style={{ rotateX, scale, opacity }}
        className="bg-white rounded-2xl p-8 shadow-lg"
      >
        {children}
      </motion.div>
    </div>
  );
}
```
---

##变体错开模式

变体会自动从父级传播到子级——您不需要手动传递它们。```tsx
const parent = {
  hidden: {},
  visible: {
    transition: {
      staggerChildren: 0.1,   // Delay between each child
      delayChildren: 0.2,      // Initial delay before first child
      when: 'beforeChildren',  // Parent animates before children
    }
  }
};

const child = {
  hidden: { opacity: 0, y: 20 },
  visible: { opacity: 1, y: 0, transition: { duration: 0.5 } }
};

// Children with `variants={child}` automatically get the stagger
// when the parent transitions between 'hidden' and 'visible'
```
---

##动态值事件```tsx
import { useScroll, useMotionValueEvent } from 'motion/react';

const { scrollY } = useScroll();

// Fires on every change — use for imperative side effects
useMotionValueEvent(scrollY, 'change', latest => {
  console.log('scroll position:', latest);
});

// Detect scroll direction
const [direction, setDirection] = useState<'up' | 'down'>('down');

useMotionValueEvent(scrollY, 'change', current => {
  const diff = current - scrollY.getPrevious()!;
  setDirection(diff > 0 ? 'down' : 'up');
});
```
**何时使用`useMotionValueEvent`vs`useTransform`:**
-使用`useTransform`当你想要一个动画平滑的CSS值（y，不透明度，颜色）
-使用`useMotionValueEvent`当你想要触发React状态变化或副作用

---

##Next.js&应用路由器说明```tsx
// Every file using motion hooks must be a Client Component
'use client';

// For page-level scroll tracking in App Router, use useScroll in a layout
// that's already a client component — don't try to use it in Server Components

// If you need SSR-safe scroll animations, gate with:
import { useEffect, useState } from 'react';
const [mounted, setMounted] = useState(false);
useEffect(() => setMounted(true), []);
if (!mounted) return null; // or a skeleton
```
**Next.js应用路由器的推荐模式：**
1. 将所有`motion.*`组件保存在单独的`'use client'`文件中
2. 将它们导入到服务器组件中——它们将被客户端自动渲染
3. 在布局级别使用`AnimatePresence`进行页面转换

---

# #可访问性```tsx
import { useReducedMotion } from 'motion/react';

export function AnimatedCard() {
  const prefersReducedMotion = useReducedMotion();

  return (
    <motion.div
      initial={{ opacity: 0, y: prefersReducedMotion ? 0 : 50 }}
      whileInView={{ opacity: 1, y: 0 }}
      transition={{ duration: prefersReducedMotion ? 0 : 0.6 }}
    >
      Content
    </motion.div>
  );
}
```
或禁用所有滚动链接转换时，减少运动为首选：```tsx
const prefersReducedMotion = useReducedMotion();
const y = useTransform(
  scrollYProgress, [0, 1],
  prefersReducedMotion ? [0, 0] : [100, -100]  // no movement if reduced motion
);
```
---

##常见的副驾驶陷阱

**缺少“使用客户端”：** Copilot忘记为Next.js应用路由器文件添加此功能。
每个使用`useScroll`、`useTransform`、`motion.*`或任何钩子的文件都需要在顶部使用`'use client'`。

**在普通div上使用样式道具：** Copilot有时会写`<div style={{ y }}>`，其中`y`是MotionValue。
它什么也不做。一定是`<motion.div style={{ y }}>`。

**旧的导入路径：** Copilot仍然生成`from 'framer-motion'`（有效，但遗留）。
当前规范：`from 'motion/react'`。

没有`offset`，`scrollYProgress`跟踪整个页面
从0到1 -不是元素的位置。总是传递`target`+`offset`用于元素级跟踪。

**缺少ref目标：** Copilot有时会写`target: ref`，但忘记将`ref`附加到DOM元素。```tsx
const ref = useRef(null);
const { scrollYProgress } = useScroll({ target: ref }); // ← ref passed
return <div ref={ref}>...</div>;                          // ← ref attached
```
**使用动画道具滚动链接值：**滚动链接值必须使用`style`，而不是`animate`。`animate`在mount/unmount上运行，而不是在滚动上运行。```tsx
// ❌ Wrong
<motion.div animate={{ opacity }} />

// ✅ Correct
<motion.div style={{ opacity }} />
```
**滚动过程不平滑：** Raw`scrollYProgress`在精细运动时感觉机械。
用`useSpring`包装进度条和UI元素，这些元素需要抛光的感觉。