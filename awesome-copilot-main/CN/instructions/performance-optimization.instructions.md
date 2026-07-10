---
applyTo: '**'
description: 'Comprehensive web performance standards based on Core Web Vitals (LCP, INP, CLS), with 50+ anti-patterns, detection regex, framework-specific fixes for modern web frameworks, and modern API guidance.'
---
#性能标准

web应用程序开发的综合性能规则。每个反模式都包括严重性分类、检测方法、受影响的核心Web vital指标和纠正代码示例。

* *严重性级别:* *

- **CRITICAL** -直接降低核心Web Vital超过“差”阈值。必须在合并前修复。
- **重要** -显著影响用户体验。在相同的冲刺中修复。
-建议** -优化机会。为将来的迭代做计划。

---

##核心Web关键快速参考

LCP（最大含量油漆）

**好：< 2.5s |需要改进：2.5-4s |差：> 4s**

度量最大的可见内容元素何时完成呈现。四个连续阶段：

|阶段|目标|它测量什么||-------|--------|-----------------|
| TTFB | ~预算的40% |服务器响应时间|
|资源加载延迟| < 10% | TTFB到LCP取资源开始|的时间间隔
|资源加载时长| ~40% | LCP资源|的下载时间
|元素渲染延迟| < 10% |下载和绘制|之间的时间

INP（与下一幅画的交互）

**好：< 200ms |需要改进：200-500ms |差：> 500ms**

测量所有用户交互的延迟，报告最糟糕的情况。三个阶段:

|阶段|优化||-------|-------------|
|输入延迟|中断长任务，屈服于浏览器|
|处理时间|保持处理程序< 50ms |
最小化DOM大小，避免强制布局|

b> **诊断工具：**使用长动画帧（面包）API （Chrome 123+）调试INP问题。与遗留的Long Tasks API相比，LoAF提供了更好的归属，包括脚本源和呈现时间。

CLS（累积布局移位）

**良好：< 0.1 |需要改进：0.1-0.25 |差：> 0.25**

版面移位来源：无尺寸图片、动态注入内容、网页字体FOUT、延迟加载广告。用户交互500ms内的移位不受影响。

---

加载和LCP反模式（L1-L10）

### L1：渲染阻塞CSS没有关键提取

—**严重性**：紧急
- **检测**:`<link.*rel="stylesheet"`在`<head>`加载大CSS
—** cwv **: LCP```html
<!-- BAD -->
<link rel="stylesheet" href="/styles/main.css" />

<!-- GOOD — inline critical CSS (extracted at build time), preload the rest -->
<style>/* critical above-fold CSS, inlined by a tool like Critters/Beasties */</style>
<link rel="preload" href="/styles/main.css" as="style" />
<link rel="stylesheet" href="/styles/main.css" />
```
首选构建时关键CSS提取（例如，Critters, Beasties,Next.js`experimental.optimizeCss`）加上正常的`<link rel="stylesheet">`。避免旧的`media="print" onload="this.media='all'"`技巧：内联事件处理程序在严格的CSP下被阻塞（没有`'unsafe-inline'`/没有`script-src-attr 'unsafe-inline'`），这将阻止样式表被激活并导致样式回归。如果非关键CSS确实必须延迟，那么通过一个交换`media`的外部脚本加载它，而不是内联处理程序。

### L2：渲染阻塞同步脚本

—**严重性**：紧急
- **检测**:`<script.*src=`不带`async|defer|type="module"`—** cwv **: LCP```html
<!-- BAD -->
<script src="/vendor/analytics.js"></script>

<!-- GOOD -->
<script src="/vendor/analytics.js" defer></script>
```
### L3：缺少Preconnect到Critical Origins

- **严重性**：重要
—**检测**：不包含`<link rel="preconnect">`的第三方API/CDNurl
—** cwv **: LCP```html
<link rel="preconnect" href="https://api.example.com" />
<link rel="dns-prefetch" href="https://analytics.example.com" />
```
L4: LCP资源缺少预加载

—**严重性**：紧急
- **检测**:LCPimage/font未预加载
—** cwv **: LCP```html
<link rel="preload" as="image" href="/hero.webp" fetchpriority="high" />
```
### L5：获取主内容的客户端数据

—**严重性**：紧急
—**检测**:`useEffect.*fetch|useEffect.*axios|ngOnInit.*subscribe`—** cwv **: LCP```tsx
// BAD — content appears after JS execution + API call
'use client';
function Page() {
  const [data, setData] = useState(null);
  useEffect(() => { fetch('/api/data').then(r => r.json()).then(setData); }, []);
  return <div>{data?.title}</div>;
}

// GOOD — Server Component fetches data before HTML is sent
async function Page() {
  const data = await fetch('https://api.example.com/data').then(r => r.json());
  return <div>{data.title}</div>;
}
```
过多的重定向链

- **严重性**：重要
- **检测**：多个顺序重定向（HTTP301/302链）
—** cwv **: LCP

每次重定向增加200-300ms。最多一个重定向。

L7: LCP元素缺少fetchpriority

- **严重性**：重要
- **检测**：上面的英雄图像没有`fetchpriority="high"`或`priority`道具
—** cwv **: LCP```tsx
// Next.js
<Image src="/hero.webp" alt="Hero" width={1200} height={600} priority />

// Angular
<img ngSrc="/hero.webp" alt="Hero" width="1200" height="600" priority>

// Plain HTML
<img src="/hero.webp" alt="Hero" width="1200" height="600" fetchpriority="high" />
```
### L8：第三方脚本头没有Async/Defer- **严重性**：重要
- **检测**:`<script.*src="https://`不带`async|defer`—** cwv **: LCP

推迟非必要的脚本。为聊天小部件使用facade模式。

### L9：超大的初始HTML （>14KB）

—**严重性**：建议
- **检测**：服务器端呈现的HTML大于14KB
—** cwv **: LCP

减少内联CSS/JS，删除空白，使用带有悬念边界的流SSR。

### L10：缺少压缩

- **严重性**：重要
- **检测**：服务器不返回`content-encoding: br`或`gzip`—** cwv **: LCP

在CDN/server级别启用Brotli（比gzip好15-25%）。

---

渲染和水化反模式（R1-R8）

R1：整个组件树标记为“use client”

—**严重性**：紧急
- **检测**:`"use client"`在顶层布局或页面组件
- ** cwv **: LCP + inp

将`"use client"`下推到需要交互性的叶组件。### R2：异步数据缺少悬念边界

- **严重性**：重要
- **检测**：服务器组件做数据抓取没有`<Suspense>`—** cwv **: LCP```tsx
// GOOD — stream shell immediately, fill in data progressively
async function Page() {
  const user = await getUser();
  return (
    <div>
      <Header user={user} />
      <Suspense fallback={<PostsSkeleton />}>
        <Posts />
      </Suspense>
    </div>
  );
}
```
### R3：动态客户端内容的水合不匹配

- **严重性**：重要
—**Detection**:`Date.now()|Math.random()|window\.innerWidth`—** cwv **: CLS

对于仅针对客户机的值使用`useEffect`，对于已知的差异使用`suppressHydrationWarning`。

### R4：慢速数据源缺少流

- **严重性**：重要
- **检测**：在发送HTML之前等待所有数据的页面
- ** cwv **: LCP （ttfb）

使用带有悬念边界的流SSR。壳立即流；缓慢的数据逐渐填充。

不稳定的引用导致重新渲染

- **严重性**：重要
- **检测**:`style=\{\{|onClick=\{\(\) =>`内联在JSX
—** cwv **: inp

启用React编译器的React 19+（单独的babel/SWC构建插件）：自动记忆。无需编译器：使用`useMemo`/`useCallback`提取或记忆。角:OnPush。Vue:`computed()`。

### R6：长列表缺少虚拟化- **严重性**：重要
- **检测**:`.map(`渲染>100项无虚拟滚动
—** cwv **: inp

使用TanStack Virtual、react-window、Angular CDK Virtual Scroll或vue- Virtual - Scroll。

### R7：即时隐藏内容的SSR

—**严重性**：建议
- **检测**：服务器端渲染`display: none`组件
- ** cwv **: LCP （ttfb）

对模态、抽屉和下拉框使用客户端渲染。角:`@defer`。反应:`React.lazy`。

### R8：清单上缺少`key`道具

- **严重性**：重要
- **检测**:`.map(`无`key=`prop
—** cwv **: inp```tsx
// GOOD — stable unique key
{items.map(item => <Row key={item.id} data={item} />)}
```
如果列表可以重新排序，不要使用数组索引作为键。

---

JavaScript运行时和INP反模式（J1-J8）

### J1：事件处理程序中的长同步任务

—**严重性**：紧急
- **检测**：计算量大的事件处理程序（bbb50毫秒）
—** cwv **: inp```typescript
// GOOD — yield to browser
async function handleClick() {
  setLoading(true);
  await (globalThis.scheduler?.yield?.() ?? new Promise(r => setTimeout(r, 0)));
  const result = expensiveComputation(data);
  setResult(result);
}
```
将繁重的工作转移到Web Worker以获得最佳效果。

**注：**`scheduler.yield()`支持Chrome 129+， Firefox 129+，但不支持Safari作为2026年4月。回退:`await (globalThis.scheduler?.yield?.() ?? new Promise(r => setTimeout(r, 0)))`。

### J2：布局抖动

—**严重性**：紧急
- **检测**:`offsetHeight|offsetWidth|getBoundingClientRect|clientHeight`在循环
—** cwv **: inp```typescript
// GOOD — batch reads then batch writes
const heights = elements.map(el => el.offsetHeight);
elements.forEach((el, i) => { el.style.height = `${heights[i] + 10}px`; });
```
### J3:setInterval/setTimeout不清理

- **严重性**：重要
- **检测**:`setInterval|setTimeout`不清理
—**影响**：内存```tsx
useEffect(() => {
  const id = setInterval(() => fetchData(), 5000);
  return () => clearInterval(id);
}, []);
```
### J4: addEventListener没有removeEventListener

- **严重性**：重要
- **检测**:`addEventListener`不清理
—**影响**：内存```tsx
useEffect(() => {
  const controller = new AbortController();
  window.addEventListener('resize', handleResize, { signal: controller.signal });
  return () => controller.abort();
}, []);
```
### J5：分离DOM节点引用

—**严重性**：建议
- **检测**：保存被删除的DOM元素引用的变量
—**影响**：内存

当删除元素时，将引用设置为`null`。

### J6：同步XHR

—**严重性**：紧急
- **检测**：带同步标志的`XMLHttpRequest`—** cwv **: inp

使用`fetch()`（总是异步的）。

### J7：主线程的繁重计算

- **严重性**：重要
—**检测**：组件代码中的cpu密集型操作
—** cwv **: inp

移动到Web Worker或分解成块`scheduler.yield()`。

### J8：缺失的效果清理

- **严重性**：重要
**检测**:`useEffect`无返回清理；`subscribe`不退订
—**影响**：内存

React：从`useEffect`返回清理。角:`takeUntilDestroyed()`。Vue:`onUnmounted`。

---

CSS性能反模式（C1-C7）

### C1：动画使用布局触发属性—**严重性**：紧急
- **检测**:`animation:|transition:`与`top|left|width|height|margin|padding`—** cwv **: inp```css
/* BAD — main thread, <60fps */
.card { transition: width 0.3s, height 0.3s; }

/* GOOD — GPU compositor, 60fps */
.card { transition: transform 0.3s, opacity 0.3s; }
.card:hover { transform: scale(1.05); }
```
### C2：屏幕外部分缺少内容可见性

—**严重性**：建议
- **检测**：没有`content-visibility: auto`的长页
—** cwv **: inp```css
.below-fold-section {
  content-visibility: auto;
  contain-intrinsic-size: auto 500px;
}
```
将更改永久应用

—**严重性**：建议
- **检测**:`will-change:`在基础CSS（不是`:hover|:focus`）
—**影响**：内存

仅应用于交互或让浏览器自动优化。

### C4：大型未使用的CSS

- **严重性**：重要
—**检测**:CSS中>50%的规则未使用
—** cwv **: LCP

使用PurgeCSS，顺风净化，或者小生物。每个路由代码拆分CSS。

### C5：热路径中的通用选择器

—**严重性**：建议
—**检测**:CSS中的`\* \{`—** cwv **: inp```css
/* GOOD — zero-specificity reset */
:where(*, *::before, *::after) { box-sizing: border-box; }
```
### C6：缺少CSS包容

—**严重性**：建议
- **检测**：无`contain`属性的复杂组件
—** cwv **: inp```css
.sidebar { contain: layout style paint; }
```
### C7：路由转换没有视图转换API

—**严重性**：建议
—**Detection**：没有View Transitions API的SPA路由改变
- **CWV**: CLS（感知）```javascript
// Use View Transitions for smooth route changes (with feature check)
if (document.startViewTransition) {
  document.startViewTransition(() => {
    // update DOM / navigate
  });
} else {
  // fallback: update DOM directly
}
```
所有主流浏览器都支持相同的文档转换。Chrome/Edge126+、Safari 18.5+支持跨文档。在调用之前一定要进行特性检查——不支持的浏览器会在没有保护的情况下抛出错误。

---

图片，媒体和字体反图案（I1-I8）

### 1：没有尺寸的图像

—**严重性**：紧急
- **检测**:`<img`不含`width=`和`height=`—** cwv **: CLS

总是在图像上设置`width`和`height`，或者在CSS中使用`aspect-ratio`。

### I2：延迟加载上面的图片

—**严重性**：紧急
- **检测**:`loading="lazy"`对hero/banner图像
—** cwv **: LCP```html
<!-- GOOD — eager load with high priority -->
<img src="/hero.webp" alt="Hero" fetchpriority="high" />
```
### I3：仅支持传统格式（JPEG/PNG）

- **严重性**：重要
- **检测**：没有WebP/AVIF选项的图像
—** cwv **: LCP```html
<picture>
  <source srcset="/hero.avif" type="image/avif" />
  <source srcset="/hero.webp" type="image/webp" />
  <img src="/hero.jpg" alt="Hero" width="1200" height="600" />
</picture>
```
### I4：缺少响应srcset/sizes- **严重性**：重要
- **检测**:`<img`不带`srcset`—** cwv **: LCP```html
<img src="/hero-800.jpg" alt="Hero"
     srcset="/hero-400.jpg 400w, /hero-800.jpg 800w, /hero-1200.jpg 1200w"
     sizes="(max-width: 600px) 400px, (max-width: 1024px) 800px, 1200px" />
```
### I5：没有Font -display的字体

- **严重性**：重要
- **检测**:`@font-face`不带`font-display`—** cwv **: CLS```css
@font-face {
  font-family: 'CustomFont';
  src: url('/fonts/custom.woff2') format('woff2');
  font-display: swap; /* or "optional" for best CLS */
}
```
临界字体未预加载

- **严重性**：重要
- **检测**：自定义字体，不带`<link rel="preload">`- ** cwv **: LCP + CLS```html
<link rel="preload" href="/fonts/main.woff2" as="font" type="font/woff2" crossorigin />
```
### I7：当子集足够时，完全字体加载

—**严重性**：建议
- **检测**：字体文件> 50KB WOFF2
—** cwv **: LCP

使用`unicode-range`（带有字形悬挂符的子集）或`next/font`（自动子集谷歌字体）。

### 8：未优化的svg

—**严重性**：建议
- **Detection**：带有编辑器元数据的svg
- **CWV**: LCP（未成年）```bash
npx svgo input.svg -o output.svg
```
---

##束和树摇反模式（B1-B6）

### B1：桶文件导入整个模块

- **严重性**：重要
—**检测**:`from '\.\/(?:.*\/index|components)'`—** cwv **: inp```typescript
// BAD
import { Button } from './components';

// GOOD — direct import
import { Button } from './components/Button';
```
### B2: CommonJS require（）防止摇树

- **严重性**：重要
- **检测**：前端代码中的`require(`—** cwv **: inp

使用ESM`import/export`。将`require`替换为`import`。

### B3：小实用程序的大依赖

- **严重性**：重要
- **检测**:`from "moment"|from "lodash"`（全进口）
—** cwv **: inp```typescript
// GOOD — tree-shakeable alternatives
import { format } from 'date-fns';
import { pick } from 'lodash-es';

// BEST — native JS
const formatted = new Intl.DateTimeFormat('en').format(date);
```
### B4：缺少路由分割的动态导入

—**严重性**：紧急
—**检测**：静态导入所有路由组件
—** cwv **: inp```tsx
// Next.js: automatic with file-based routing
// React:
const Page = React.lazy(() => import('./pages/Page'));
// Angular:
{ path: 'settings', loadComponent: () => import('./pages/settings.component') }
// Vue:
const Page = defineAsyncComponent(() => import('./pages/Page.vue'));
```
遗漏的副作用在package.json—**严重性**：建议
- **检测**：库package.json不带`"sideEffects"`字段
—** cwv **: inp```json
{ "sideEffects": false }
```
### B6：复制依赖项

—**严重性**：建议
- **检测**：同一库在多个版本
—** cwv **: inp```bash
npm dedupe
```
---Next.js（NX1-NX6）

NX1：不使用next/image- **严重性**：重要
- **检测**:`<img `在`.tsx`而不是`<Image>`- ** cwv **: LCP + CLS```tsx
import Image from 'next/image';
<Image src="/hero.jpg" alt="Hero" width={1200} height={600} priority />
```
### NX2：不使用部分预渲染缓存组件

- **严重性**：重要
- **检测**:Next.js16 +项目中没有`"use cache"`指令的页面
—** cwv **: LCP```typescript
// BAD — entire page is dynamic
export default async function Page() {
  const data = await fetchData(); // blocks full page render
  return <div>{data.title}</div>;
}

// GOOD — enable Partial Prerendering with "use cache"
// next.config.ts: { cacheComponents: true }
"use cache";
export default async function Page() {
  const data = await fetchData(); // static shell renders instantly, dynamic holes stream
  return <div>{data.title}</div>;
}
```
在`next.config.ts`中启用`cacheComponents: true`。在文件、组件或函数级别使用`"use cache"`。静壳瞬间荷载；动态内容流通过悬念边界。

### NX3：不必要的“使用客户端”在服务器可渲染组件

- **严重性**：重要
- **检测**:`"use client"`组件没有钩子或浏览器api
—** cwv **: inp

从只呈现静态内容的组件中删除`"use client"`。

### NX4：数据提取在useEffect而不是服务器端

—**严重性**：紧急
- **检测**:`useEffect`+`fetch`在Next.js应用路由器页面
—** cwv **: LCP

直接从服务器组件中获取数据（异步函数体）。

### NX5：缺少next/font- **严重性**：重要
- **检测**:`fonts.googleapis|fonts.gstatic`在CSS/HTML- ** cwv **: CLS + LCP```tsx
import { Inter } from 'next/font/google';
const inter = Inter({ subsets: ['latin'] });
```
### NX6：缺少“使用缓存”的可缓存服务器功能

- **严重性**：重要
- **检测**：异步服务器功能没有`"use cache"`在Next.js16 +与`cacheComponents: true`—** cwv **: LCP```typescript
// BAD — data fetched on every request
async function getProducts() {
  return await db.products.findMany();
}

// GOOD — cached with revalidation
"use cache";
import { cacheLife } from 'next/cache';
async function getProducts() {
  cacheLife('hours');
  return await db.products.findMany();
}
```
`"use cache"`取代旧的`unstable_cache`和`fetch`缓存选项。使用`cacheLife()`和`cacheTag()`进行细粒度控制。

---

框架特定：Angular （NG1-NG6）

### NG1：表示组件的默认更改检测

- **严重性**：重要
- **检测**：组件没有`ChangeDetectionStrategy.OnPush`（Angular <19）或没有信号（Angular 19+）
—** cwv **: inp```typescript
// Angular <19: Use OnPush
@Component({
  changeDetection: ChangeDetectionStrategy.OnPush,
  ...
})

// Angular 19+: Prefer zoneless with signals
// app.config.ts: provideZonelessChangeDetection()
@Component({ ... })
export class ProductCard {
  product = input.required<Product>(); // signal input
  price = computed(() => this.product().price * 1.19); // derived signal
}
```
Angular 19+：更喜欢带信号的无区变更检测。当使用基于信号的反应性时，OnPush是不必要的。Angular 20+有稳定的无区支持。

### NG2：不使用ngoptimizeimage

- **严重性**：重要
- **检测**:`.component.html`中没有`ngSrc`- ** cwv **: LCP + CLS```html
<img ngSrc="/hero.jpg" alt="Hero" width="1200" height="600" priority />
```
### NG3：以下内容缺少@defer

—**严重性**：建议
- **检测**：重载重载组件（Angular 17+）
—** cwv **: inp```html
@defer (on viewport) {
  <app-heavy-chart [data]="chartData" />
} @placeholder {
  <div class="chart-skeleton"></div>
}
```
### NG4：不使用信号的反应状态

—**严重性**：建议
- **检测**：在Angular 19+中没有信号的类属性
—** cwv **: inp

使用`signal()`表示反应状态，`computed()`表示派生值。信号api （`signal()`、`computed()`、`effect()`）从Angular 20开始就稳定了。

### NG5：完全水化而不增加水化

- **严重性**：重要
- **Detection**：在Angular 19+中没有`withIncrementalHydration()`的SSR应用
- ** cwv **: lcp, inp```typescript
// BAD — full hydration blocks interactivity
provideClientHydration()

// GOOD — incremental hydration with triggers
provideClientHydration(withIncrementalHydration())
```
使用`@defer`触发器（`on viewport`,`on interaction`）按需水合物组件。通过延缓非关键成分的水化来减少TTI。

### NG6：在Angular 20+项目中仍然使用zone.js—**严重性**：建议
- **检测**：在polyfills数组`zone.js`，没有`provideZonelessChangeDetection()`在Angular 20+
—** cwv **: inp```typescript
// app.config.ts
export const appConfig = {
  providers: [
    provideZonelessChangeDetection(), // removes ~15-30KB from bundle
    // ...
  ]
};
```
使用信号进行无区更改检测可以减小包大小并提高运行时性能。从Angular 20开始稳定。

---

框架特定：React （RX1-RX4）

### RX1：没有采用React编译器

—**严重性**：建议
- **检测**：手动`useMemo|useCallback`在React 19+项目
—** cwv **: inp

启用React Compiler （v19+）进行自动记忆。拆下手动包装。

### RX2：为昂贵的更新缺少useTransition

- **严重性**：重要
- **检测**：状态更新导致昂贵的重新渲染没有`useTransition`—** cwv **: inp```tsx
const [isPending, startTransition] = useTransition();
function handleFilter(value) {
  startTransition(() => setFilter(value));
}
```
### RX3：缺少useDeferredValue的昂贵渲染

- **严重性**：重要
- **检测**：昂贵的渲染从快速变化的输入
—** cwv **: inp```tsx
const deferredQuery = useDeferredValue(query);
const results = expensiveFilter(items, deferredQuery);
```
### RX4：缺少React。懒惰的路由分裂

- **严重性**：重要
—**检测**：静态导入路由组件
—** cwv **: inp```tsx
const Settings = React.lazy(() => import('./pages/Settings'));
```
---

框架特定：Vue （VU1-VU4）

VU1：大型数据结构上的reactive（）

- **严重性**：重要
- **检测**:`reactive(`对大数组或深度对象
—** cwv **: inp

对于大数据使用`shallowRef()`或`shallowReactive()`。

VU2：在昂贵的列表渲染上缺少v-memo

—**严重性**：建议
- **检测**：大列表没有`v-memo`—** cwv **: inp```vue
<div v-for="item in items" :key="item.id" v-memo="[item.id, item.updatedAt]">
  <ExpensiveItem :data="item" />
</div>
```
### VU3：缺少defineAsyncComponent

- **严重性**：重要
- **检测**：重型部件静态导入
—** cwv **: inp```typescript
const HeavyChart = defineAsyncComponent(() => import('./HeavyChart.vue'));
```
VU4：对性能关键部件不使用蒸汽模式

—**严重性**：建议
- **检测**：在Vue 3.6+中使用虚拟DOM的性能关键组件
—** cwv **: inp

Vue 3.6+蒸汽模式编译模板直接DOM操作，绕过虚拟DOM。用于性能关键的子树。可与标准组分混合使用。

---

资源提示快速参考

|提示|用途|何时使用||------|---------|-------------|
|`preconnect`| DNS + TCP + TLS早期|关键第三方来源（API， CDN，字体）|
|`preload`|立即获取，高优先级| LCP图像，关键字体|
|`prefetch`|低优先级的未来导航|下一页资产|
|`dns-prefetch`|仅DNS解析|非关键第三方来源|
|`modulepreload`|预加载+解析ES模块|关键JS模块|
|`<script type="speculationrules">`|Prefetch/prerender下一个导航|可能的下一页（Chrome 121+，渐进式增强）|

---

图片优化快速参考

|方面|推荐||--------|---------------|
| WebP（缩小25-34%），AVIF（缩小50%）|
| LCP镜像|`fetchpriority="high"`或框架`priority`prop |
|下面|`loading="lazy"`|
|尺寸|总是设置为`width`+`height`|
|响应|`srcset`+`sizes`或框架图像组件|
|压缩|质量75-85的照片|

---

字体加载快速参考

|策略|最适合| CLS影响||----------|---------|-----------|
|`font-display: swap`|正文|轻微FOUT，最小CLS |
|`font-display: optional`|所有字体（最好的CLS） |没有FOUT，没有CLS |
|`next/font`|Next.js项目|零CLS |
|可变字体|多个权重|所有权重|的单一文件

规则：只预加载1-2个关键字体，使用WOFF2，子集到所需字符，如果可能的话自托管。

---

性能检查表（CWV）

### LCP （< 2.5s）
- [] LCP图像有`fetchpriority="high"`或`priority`prop
- [] LCP图像预加载，如果不是在HTML源
-[]上面的图片没有`loading="lazy"`-[]关键CSS被内联或提取
-没有渲染阻塞脚本（使用`defer`或`async`）
-[]预连接关键第三方
[]主内容服务器端呈现（不是客户端获取）
-[]图像在现代格式（WebP/AVIF）与响应`srcset`-[]启用压缩功能（首选Brotli）
-[]字体预装`font-display: swap`或`optional`### INP （< 200ms）
-[]事件处理程序在50ms内完成
-[]将长任务分成小块
-[]实现基于路由的代码拆分
-[]繁重的计算转移到Web worker
-[] > 100虚拟化列表
-[]没有桶文件导入（直接导入组件）
[]使用ESM导入（不是CommonJS`require`）
- []`"use client"`仅适用于需要交互的组件
-[]布局触发CSS属性不动画
[]效果清理实现（不泄漏listeners/timers）

CLS （< 0.1）
-[]所有图片都有`width`和`height`属性
-[]字体使用`font-display: swap`或`optional`-[]没有在现有内容之上动态注入内容
—[]Ads/embeds已预留空间
-[]没有水合不匹配
- []`content-visibility: auto`有`contain-intrinsic-size`