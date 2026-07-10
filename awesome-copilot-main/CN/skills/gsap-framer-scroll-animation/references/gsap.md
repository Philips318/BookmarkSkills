# GSAP ScrollTrigger -完全引用

##目录
1. [安装与注册]（# Installation—注册）
2. [ScrollTrigger配置参考]（# ScrollTrigger - Config - Reference）
3. [开始/结束语法解码]（# Start—End - Syntax - Decoded）
4. (toggleActions值)(# toggleactions-values)
5. [食谱与副驾驶提示]（# Recipes -with- Copilot - Prompts）
-淡入批量显示
-刷洗动画
-固定的时间线
-视差图层
-水平滚动
-字符错开文本
-滚动卡扣
-进度条
——ScrollSmoother
-滚动计数器
6. [React Integration (useGSAP)]（# React - Integration - useGSAP）
7. [Lenis Smooth Scroll]（# Lenis - Smooth - Scroll）
8. [Responsive with matchMedia]（# responsivewith-matchmedia）
9. (可访问性)(#可访问性)
10. [性能和清理]（# Performance - Cleanup）
11. [常见的副驾驶陷阱]（# Common - Copilot - traps）

---##安装和注册```bash
npm install gsap
# React
npm install gsap @gsap/react
```

```js
import gsap from 'gsap';
import { ScrollTrigger } from 'gsap/ScrollTrigger';
import { ScrollSmoother } from 'gsap/ScrollSmoother'; // optional
gsap.registerPlugin(ScrollTrigger, ScrollSmoother);
```
CDN(香草):```html
<script src="https://cdn.jsdelivr.net/npm/gsap@3.14/dist/gsap.min.js"></script>
<script src="https://cdn.jsdelivr.net/npm/gsap@3.14/dist/ScrollTrigger.min.js"></script>
```
---

## ScrollTrigger配置引用```js
gsap.to('.element', {
  x: 500,
  ease: 'none',          // Use 'none' for scrub animations
  scrollTrigger: {
    trigger: '.section',         // Element whose position triggers the animation
    start: 'top 80%',            // "[trigger edge] [viewport edge]"
    end: 'bottom 20%',           // Where animation ends
    scrub: 1,                    // Link progress to scroll; use true for instant scrub, or a number for smooth lag
    pin: true,                   // Pin trigger element during scroll; use a selector/element to pin something else
    pinSpacing: true,            // Add space below pinned element (default: true)
    markers: true,               // Debug markers — REMOVE in production
    toggleActions: 'play none none reverse', // onEnter onLeave onEnterBack onLeaveBack
    toggleClass: 'active',       // CSS class added/removed when active
    snap: { snapTo: 'labels', duration: 0.3, ease: 'power1.inOut' }, // Or use a number like 1 to snap to increments
    fastScrollEnd: true,         // Force completion if user scrolls past fast
    horizontal: false,           // true for horizontal scroll containers
    anticipatePin: 1,            // Reduces pin jump (seconds to anticipate)
    invalidateOnRefresh: true,   // Recalculate positions on resize
    id: 'my-trigger',            // For ScrollTrigger.getById()
    onEnter: () => {},
    onLeave: () => {},
    onEnterBack: () => {},
    onLeaveBack: () => {},
    onUpdate: self => console.log(self.progress), // 0 to 1
    onToggle: self => console.log(self.isActive),
  }
});
```
---

##开始/结束语法解码

格式:`"[trigger position] [viewport position]"`|值|含义||---|---|
|`"top bottom"`|触发器的顶部击中viewport的底部-进入视图|
|`"top 80%"`|触发器的顶部达到80%从viewport |的顶部向下
|`"top center"`|触发器顶部到达视口中心|
|`"top top"`|在viewport |顶部的触发器顶部
|`"center center"`|中心对齐|
|`"bottom top"`|视图口顶部触发器的底部-退出视图|
|`"+=200"`|在触发位置|后200像素
|`"-=100"`|触发位置|前100像素
|`"+=200%"`|触发|后视口高度200%

---

## toggleActions值```
toggleActions: "play pause resume reset"
                ^      ^      ^        ^
              onEnter onLeave onEnterBack onLeaveBack
```
|值|效果||---|---|
|`play`|从当前位置|
暂停在当前位置|
|`resume`|从暂停的位置恢复|
|`reverse`|倒放|
|`reset`|跳转启动|
|`restart`|从|开始播放
|`none`|什么都不做|

最常见的入口动画：`"play none none none"`（动画一次，不要反转）。

---

食谱与副驾驶提示

# # # 1。淡入批量显示

**副驾驶聊天提示：**```
Using GSAP ScrollTrigger.batch, animate all .card elements: 
fade in from opacity 0, y 50 when they enter the viewport at 85%.
Stagger 0.15s between cards. Animate once (no reverse).
```

```js
gsap.registerPlugin(ScrollTrigger);

ScrollTrigger.batch('.card', {
  onEnter: elements => {
    gsap.from(elements, {
      opacity: 0,
      y: 50,
      stagger: 0.15,
      duration: 0.8,
      ease: 'power2.out',
    });
  },
  start: 'top 85%',
});
```
为什么`batch`优于单个ScrollTriggers：批处理将进入一个动画调用的元素分组，这比为每个元素创建一个ScrollTrigger性能更高。

---

# # # 2。刷洗动画（滚动链接）

**副驾驶聊天提示：**```
GSAP scrub: animate .hero-image scale from 1 to 1.3 and opacity to 0
as the user scrolls past .hero-section. 
Perfectly synced to scroll position, no pin.
```

```js
gsap.to('.hero-image', {
  scale: 1.3,
  opacity: 0,
  ease: 'none',   // Critical: linear easing for scrub
  scrollTrigger: {
    trigger: '.hero-section',
    start: 'top top',
    end: 'bottom top',
    scrub: true,
  }
});
```
---

# # # 3。固定的时间表

**副驾驶聊天提示：**```
GSAP pinned timeline: pin .story-section while a sequence plays —
fade in .title (y: 60), scale .image to 1, slide .text from x: 80.
Total scroll distance 300vh. Scrub 1 for smoothness.
```

```js
const tl = gsap.timeline({
  scrollTrigger: {
    trigger: '.story-section',
    start: 'top top',
    end: '+=300%',
    pin: true,
    scrub: 1,
    anticipatePin: 1,
  }
});

tl
  .from('.title',  { opacity: 0, y: 60, duration: 1 })
  .from('.image',  { scale: 0.85, opacity: 0, duration: 1 }, '-=0.3')
  .from('.text',   { x: 80, opacity: 0, duration: 1 }, '-=0.3');
```
---

# # # 4。视差层

**副驾驶聊天提示：**```
GSAP parallax: background image moves yPercent -20 (slow),
foreground text moves yPercent -60 (fast). Both scrubbed to scroll, no pin.
Trigger is .parallax-section, start top bottom, end bottom top.
```

```js
// Slow background
gsap.to('.parallax-bg', {
  yPercent: -20,
  ease: 'none',
  scrollTrigger: {
    trigger: '.parallax-section',
    start: 'top bottom',
    end: 'bottom top',
    scrub: true,
  }
});

// Fast foreground
gsap.to('.parallax-fg', {
  yPercent: -60,
  ease: 'none',
  scrollTrigger: {
    trigger: '.parallax-section',
    start: 'top bottom',
    end: 'bottom top',
    scrub: true,
  }
});
```
---

# # # 5。水平滚动截面

**副驾驶聊天提示：**```
GSAP horizontal scroll: 4 .panel elements inside .panels-container.
Pin .horizontal-section, scrub 1, snap per panel.
End should use offsetWidth so it recalculates on resize.
```

```js
const sections = gsap.utils.toArray('.panel');

gsap.to(sections, {
  xPercent: -100 * (sections.length - 1),
  ease: 'none',
  scrollTrigger: {
    trigger: '.horizontal-section',
    pin: true,
    scrub: 1,
    snap: 1 / (sections.length - 1),
    end: () => `+=${document.querySelector('.panels-container').offsetWidth}`,
    invalidateOnRefresh: true,
  }
});
```
HTML:```html
<div class="horizontal-section">
  <div class="panels-container">
    <div class="panel">1</div>
    <div class="panel">2</div>
    <div class="panel">3</div>
    <div class="panel">4</div>
  </div>
</div>
```
必要的CSS:```css
.horizontal-section { overflow: hidden; }
.panels-container   { display: flex; flex-wrap: nowrap; width: 400vw; }
.panel              { width: 100vw; height: 100vh; flex-shrink: 0; }
```
---

# # # 6。字符错开文本显示

**副驾驶聊天提示：**```
Split .hero-title into characters using SplitType.
Animate each char: opacity 0→1, y 80→0, rotateX -90→0.
Stagger 0.03s, ease back.out(1.7). Trigger when heading enters at 85%.
```

```bash
npm install split-type
```

```js
import SplitType from 'split-type';

const text = new SplitType('.hero-title', { types: 'chars' });

gsap.from(text.chars, {
  opacity: 0,
  y: 80,
  rotateX: -90,
  stagger: 0.03,
  duration: 0.6,
  ease: 'back.out(1.7)',
  scrollTrigger: {
    trigger: '.hero-title',
    start: 'top 85%',
    toggleActions: 'play none none none',
  }
});
```
---

# # # 7。滚动捕捉部分

**副驾驶聊天提示：**```
GSAP: each full-height section scales from 0.9 to 1 when it enters view.
Also add global scroll snapping between sections using ScrollTrigger.create snap.
```

```js
const sections = gsap.utils.toArray('section');

sections.forEach(section => {
  gsap.from(section, {
    scale: 0.9,
    opacity: 0.6,
    scrollTrigger: {
      trigger: section,
      start: 'top 90%',
      toggleActions: 'play none none reverse',
    }
  });
});

ScrollTrigger.create({
  snap: {
    snapTo: (progress) => {
      const step = 1 / (sections.length - 1);
      return Math.round(progress / step) * step;
    },
    duration: { min: 0.2, max: 0.5 },
    ease: 'power1.inOut',
  }
});
```
---

# # # 8。滚动进度条

**副驾驶聊天提示：**```
GSAP: fixed progress bar at top of page. scaleX 0→1 linked to 
full page scroll, scrub 0.3 for slight smoothing. transformOrigin left center.
```

```js
gsap.to('.progress-bar', {
  scaleX: 1,
  ease: 'none',
  transformOrigin: 'left center',
  scrollTrigger: {
    trigger: document.body,
    start: 'top top',
    end: 'bottom bottom',
    scrub: 0.3,
  }
});
```

```css
.progress-bar {
  position: fixed; top: 0; left: 0;
  width: 100%; height: 4px;
  background: #6366f1;
  transform-origin: left;
  transform: scaleX(0);
  z-index: 999;
}
```
---

# # # 9。ScrollSmoother设置

**副驾驶聊天提示：**```
Set up GSAP ScrollSmoother with smooth: 1.5, effects: true.
Show the required wrapper HTML structure.
Add data-speed and data-lag to parallax elements.
```

```bash
# ScrollSmoother is part of gsap — no extra install needed
```

```js
import { ScrollSmoother } from 'gsap/ScrollSmoother';
gsap.registerPlugin(ScrollTrigger, ScrollSmoother);

ScrollSmoother.create({
  wrapper: '#smooth-wrapper',
  content: '#smooth-content',
  smooth: 1.5,
  effects: true,
  smoothTouch: 0.1,
});
```

```html
<div id="smooth-wrapper">
  <div id="smooth-content">
    <img data-speed="0.5" src="bg.jpg" />      <!-- 50% scroll speed -->
    <div data-lag="0.3" class="float">...</div> <!-- 0.3s lag -->
  </div>
</div>
```
---

# # # 10。动画数字计数器

**副驾驶聊天提示：**```
GSAP: animate .counter elements from 0 to their data-target value 
when they enter the viewport. Duration 2s, ease power2.out.
Format with toLocaleString. Animate once.
```

```js
document.querySelectorAll('.counter').forEach(el => {
  const obj = { val: 0 };
  gsap.to(obj, {
    val: parseInt(el.dataset.target, 10),
    duration: 2,
    ease: 'power2.out',
    onUpdate: () => { el.textContent = Math.round(obj.val).toLocaleString(); },
    scrollTrigger: {
      trigger: el,
      start: 'top 85%',
      toggleActions: 'play none none none',
    }
  });
});
```

```html
<span class="counter" data-target="12500">0</span>
```
---

React集成（useGSAP）```bash
npm install gsap @gsap/react
```

```jsx
import { useRef } from 'react';
import { useGSAP } from '@gsap/react';
import gsap from 'gsap';
import { ScrollTrigger } from 'gsap/ScrollTrigger';

gsap.registerPlugin(useGSAP, ScrollTrigger);
```
**为什么使用egsap而不是useEffect:**
当组件卸载时，`useGSAP`自动杀死在它内部创建的所有ScrollTriggers—防止内存泄漏。它还正确处理React严格模式的双重调用。可以把它看作是GSAP理解的`useLayoutEffect`的临时替代品。

**副驾驶聊天提示：**```
React: use useGSAP from @gsap/react to animate .card elements inside containerRef.
Fade in from y 60, opacity 0, stagger 0.12, scrollTrigger start top 80%.
Scope to containerRef so selectors don't match outside this component.
```

```jsx
export function AnimatedSection() {
  const containerRef = useRef(null);

  useGSAP(() => {
    gsap.from('.card', {
      opacity: 0,
      y: 60,
      stagger: 0.12,
      duration: 0.7,
      ease: 'power2.out',
      scrollTrigger: {
        trigger: containerRef.current,
        start: 'top 80%',
        toggleActions: 'play none none none',
      }
    });
  }, { scope: containerRef });

  return (
    <div ref={containerRef}>
      <div className="card">One</div>
      <div className="card">Two</div>
    </div>
  );
}
```
**固定在React中的时间轴：**```jsx
export function PinnedStory() {
  const sectionRef = useRef(null);

  useGSAP(() => {
    const tl = gsap.timeline({
      scrollTrigger: {
        trigger: sectionRef.current,
        pin: true, scrub: 1,
        start: 'top top', end: '+=200%',
      }
    });
    tl.from('.story-title', { opacity: 0, y: 40 })
      .from('.story-image', { scale: 0.85, opacity: 0 }, '-=0.2')
      .from('.story-text',  { opacity: 0, x: 40 }, '-=0.2');
  }, { scope: sectionRef });

  return (
    <section ref={sectionRef}>
      <h2 className="story-title">Chapter One</h2>
      <img className="story-image" src="/photo.jpg" alt="" />
      <p className="story-text">The story begins.</p>
    </section>
  );
}
```
**Next.js注：**运行`gsap.registerPlugin(ScrollTrigger)`在`useGSAP`或`useLayoutEffect`-或保护它：```js
if (typeof window !== 'undefined') gsap.registerPlugin(ScrollTrigger);
```
---

## Lenis平滑滚动```bash
npm install lenis
```
**副驾驶聊天提示：**```
Integrate Lenis smooth scroll with GSAP ScrollTrigger.
Add lenis.raf to gsap.ticker. Set lagSmoothing to 0.
Destroy lenis on unmount if in React.
```

```js
import Lenis from 'lenis';
import { useEffect } from 'react';

const lenis = new Lenis({ duration: 1.2, smoothWheel: true });

const raf = (time) => lenis.raf(time * 1000);
gsap.ticker.add(raf);
gsap.ticker.lagSmoothing(0);
lenis.on('scroll', ScrollTrigger.update);

// React cleanup
useEffect(() => {
  return () => {
    lenis.destroy();
    gsap.ticker.remove(raf);
  };
}, []);
```
---

##响应matchMedia

**副驾驶聊天提示：**```
Use gsap.matchMedia to animate x: 200 on desktop (min-width: 768px)
and y: 100 on mobile. Both should skip animation if prefers-reduced-motion is set.
```

```js
const mm = gsap.matchMedia();

mm.add({
  isDesktop: '(min-width: 768px)',
  isMobile:  '(max-width: 767px)',
  noMotion:  '(prefers-reduced-motion: reduce)',
}, context => {
  const { isDesktop, isMobile, noMotion } = context.conditions;
  if (noMotion) return;

  gsap.from('.box', {
    x: isDesktop ? 200 : 0,
    y: isMobile  ? 100 : 0,
    opacity: 0,
    scrollTrigger: { trigger: '.box', start: 'top 80%' }
  });
});
```
---

# #可访问性```js
// Guard all scroll animations with prefers-reduced-motion
const prefersReducedMotion = window.matchMedia('(prefers-reduced-motion: reduce)');

if (!prefersReducedMotion.matches) {
  gsap.from('.box', {
    opacity: 0, y: 50,
    scrollTrigger: { trigger: '.box', start: 'top 85%' }
  });
} else {
  // Show element immediately, no animation
  gsap.set('.box', { opacity: 1, y: 0 });
}
```
或者在`prefers-reduced-motion: reduce`条件下使用`gsap.matchMedia()`（见上文）。

---

##性能和清理```js
// Kill a specific trigger
const st = ScrollTrigger.create({ ... });
st.kill();

// Kill all triggers (e.g., on page transition)
ScrollTrigger.killAll();

// Refresh all trigger positions (after dynamic content loads)
ScrollTrigger.refresh();
```
* *性能规则:* *
-仅动画`transform`和`opacity`- gpu加速，没有布局重新计算
-避免动画化`width`，`height`,`top`,`left`,`box-shadow`,`filter`-对许多相似的元素使用`ScrollTrigger.batch()`-比每个元素一个触发器要好得多
-少量添加`will-change: transform`-仅在动态动画元素上
—在生产前一定要删除`markers: true`---

##常见的副驾驶陷阱

**忘记registerPlugin:** Copilot经常省略`gsap.registerPlugin(ScrollTrigger)`。
总是在使用ScrollTrigger之前添加它。

**错误的容易擦洗：**副驾驶默认为`power2.out`，即使在擦洗动画。
当使用`scrub: true`或`scrub: number`时，总是使用`ease: 'none'`。

**使用效果代替使用egsap在React:**副驾驶生成`useEffect`-总是交换到`useGSAP`。

**水平滚动的静态结束值：**副驾驶写`end: "+=" + container.offsetWidth`。
正确：`end: () => "+=" + container.offsetWidth`（函数形式重新计算调整大小）。**生产中留下的标记：**副驾驶加上`markers: true`，然后离开。总是删除。

**刷洗长动画没有钉：**刷洗长时间线没有钉的手段
元素滚动出视图。添加`pin: true`或缩短滚动距离。