# Performance Checklist

Web 应用性能的快速参考 checklist。与 `performance-optimization` skill 搭配使用。

## Table of Contents

- [Core Web Vitals Targets](#core-web-vitals-targets)
- [TTFB Diagnosis](#ttfb-diagnosis)
- [Frontend Checklist](#frontend-checklist)
- [Backend Checklist](#backend-checklist)
- [Measurement Commands](#measurement-commands)
- [Common Anti-Patterns](#common-anti-patterns)

## Core Web Vitals Targets

| Metric | Good | Needs Work | Poor |
|--------|------|------------|------|
| LCP (Largest Contentful Paint) | ≤ 2.5s | ≤ 4.0s | > 4.0s |
| INP (Interaction to Next Paint) | ≤ 200ms | ≤ 500ms | > 500ms |
| CLS (Cumulative Layout Shift) | ≤ 0.1 | ≤ 0.25 | > 0.25 |

## TTFB Diagnosis

当 TTFB 很慢（> 800ms）时，在 DevTools Network waterfall 中检查每个组成部分：

- [ ] **DNS resolution** slow → 为已知 origins 添加 `<link rel="dns-prefetch">` 或 `<link rel="preconnect">`
- [ ] **TCP/TLS handshake** slow → 启用 HTTP/2，考虑 edge deployment，验证 keep-alive
- [ ] **Server processing** slow → profile backend，检查 slow queries，添加 caching

## Frontend Checklist

### Images
- [ ] Images 使用现代格式（WebP、AVIF）
- [ ] Images 响应式尺寸设置（`srcset` 和 `sizes`）
- [ ] Images 和 `<source>` elements 有显式 `width` 和 `height`（在 art direction 中防止 CLS）
- [ ] Below-the-fold images 使用 `loading="lazy"` 和 `decoding="async"`
- [ ] Hero/LCP images 使用 `fetchpriority="high"` 且不 lazy load

### JavaScript
- [ ] Bundle size 小于 200KB gzipped（initial load）
- [ ] 对 routes 和重型 features 使用 dynamic `import()` 做 code splitting
- [ ] Tree shaking 已启用（验证 dependency 提供 ESM 且标记 `sideEffects: false`）
- [ ] `<head>` 中没有 blocking JavaScript（使用 `defer` 或 `async`）
- [ ] Heavy computation 转移到 Web Workers（如适用）
- [ ] 对以相同 props 重渲染的 expensive components 使用 `React.memo()`
- [ ] 仅在 profiling 显示有收益时使用 `useMemo()` / `useCallback()`
- [ ] Long tasks（> 50ms）被拆分以保持 main thread 可用 — 这是 INP 的主要 lever
- [ ] 在 long-running loops 中使用 `yieldToMain` pattern，让 input events 能在 chunks 间运行
- [ ] 在可用时使用现代 scheduling APIs：`scheduler.yield()`（首选）、带 priorities 的 `scheduler.postTask()`、`isInputPending()` 用于仅在需要时 yield
- [ ] 对可延后、非紧急工作使用 `requestIdleCallback`（analytics flush、prefetch、warmup）
- [ ] 非关键工作从 event handlers 中延后（例如 analytics、logging），避免拖慢 interaction response
- [ ] 第三方 scripts 使用 `async` / `defer` 加载，审计 size，并在很重时用 facade 包装（chat widgets、embeds）

### CSS
- [ ] Critical CSS 已 inline 或 preload
- [ ] 非关键 styles 没有 render-blocking CSS
- [ ] 生产中没有 CSS-in-JS runtime cost（使用 extraction）

### Fonts
- [ ] 限制为 2–3 个 font families，每个 2–3 个 weights（每个额外 weight 都是另一个 request）
- [ ] 仅使用 WOFF2 格式（最小、普遍支持 — 跳过 WOFF/TTF/EOT）
- [ ] 尽可能 self-host（第三方 font CDNs 会增加 DNS + TCP + TLS round-trips）
- [ ] LCP-critical fonts 已 preload：`<link rel="preload" as="font" type="font/woff2" crossorigin>`
- [ ] 使用 `font-display: swap`（非关键字体可用 `optional`）避免 FOIT 阻塞 render
- [ ] 通过 `unicode-range` subset，只交付每个页面需要的 glyphs
- [ ] 需要多个 weights/styles 时考虑 variable fonts（一个文件替代多个）
- [ ] 使用 `size-adjust`、`ascent-override`、`descent-override` 调整 fallback font metrics，减少 font swap 时的 CLS
- [ ] 在使用任何 custom font 前考虑 system font stack

### Network
- [ ] Static assets 使用 long `max-age` + content hashing 缓存
- [ ] API responses 在适当位置缓存（`Cache-Control`）
- [ ] 启用 HTTP/2 或 HTTP/3
- [ ] 为已知 origins 预连接 resources（`<link rel="preconnect">`）
- [ ] 对关键非图片 resources 使用 `fetchpriority`（例如 key `<link rel="preload">`、above-the-fold `<script>`）— 不仅用于 `<img>`
- [ ] 没有不必要 redirects

### Rendering
- [ ] 没有 layout thrashing（强制同步 layouts）
- [ ] Animations 使用 `transform` 和 `opacity`（GPU-accelerated）
- [ ] Long lists 使用 virtualization（例如 `react-window`）
- [ ] 没有不必要的 full-page re-renders
- [ ] Off-screen sections 使用 `content-visibility: auto` 和 `contain-intrinsic-size`，跳过不可见区域的 layout/paint
- [ ] 没有 `unload` event handlers，HTML responses 上没有 `Cache-Control: no-store` — 保持 back/forward cache (bfcache) eligibility

## Backend Checklist

### Database
- [ ] 没有 N+1 query patterns（使用 eager loading / joins）
- [ ] Queries 有合适 indexes
- [ ] List endpoints 已 pagination（绝不 `SELECT * FROM table`）
- [ ] Connection pooling 已配置
- [ ] Slow query logging 已启用

### API
- [ ] Response times < 200ms（p95）
- [ ] Request handlers 中没有同步 heavy computation
- [ ] 使用 bulk operations，而不是 individual calls 循环
- [ ] Response compression（gzip/brotli）
- [ ] 合适的 caching（in-memory、Redis、CDN）

### Infrastructure
- [ ] Static assets 使用 CDN
- [ ] Server 靠近 users（或 edge deployment）
- [ ] 已配置 horizontal scaling（如需要）
- [ ] Load balancer 有 health check endpoint

## Measurement Commands

### INP field data 和 DevTools workflow

1. **Field data first** — 优化前先检查 [CrUX Vis](https://developer.chrome.com/docs/crux/vis) 或你的 RUM tool 中真实用户的 INP
2. **Identify slow interactions** — 打开 DevTools → Performance panel → 交互时 record；查找由 clicks/keystrokes 触发的 long tasks
3. **Test on mid-range Android** — INP 问题通常只在较慢硬件上暴露；使用真实设备或 DevTools CPU throttling（4×–6× slowdown）

```bash
# Lighthouse CLI
npx lighthouse https://localhost:3000 --output json --output-path ./report.json

# Bundle analysis
npx webpack-bundle-analyzer stats.json
# or for Vite:
npx vite-bundle-visualizer

# Check bundle size
npx bundlesize

# Web Vitals in code
import { onLCP, onINP, onCLS } from 'web-vitals';
onLCP(console.log);
onINP(console.log);
onCLS(console.log);

# INP with interaction-level detail (attribution build)
import { onINP } from 'web-vitals/attribution';
onINP(({ value, attribution }) => {
  const { interactionTarget, inputDelay, processingDuration, presentationDelay } = attribution;
  console.log({ value, interactionTarget, inputDelay, processingDuration, presentationDelay });
});
```

## Common Anti-Patterns

| Anti-Pattern | Impact | Fix |
|---|---|---|
| N+1 queries | DB load 线性增长 | 使用 joins、includes 或 batch loading |
| Unbounded queries | 内存耗尽、timeouts | 始终 paginate，添加 LIMIT |
| Missing indexes | 数据增长时读取变慢 | 为 filtered/sorted columns 添加 indexes |
| Layout thrashing | Jank、dropped frames | 批量 DOM reads，再批量 writes |
| Unoptimized images | LCP 慢，浪费 bandwidth | 使用 WebP、responsive sizes、lazy load |
| Large bundles | Time to Interactive 慢 | Code split、tree shake、audit deps |
| Blocking main thread | INP 差、UI 无响应 | 用 `scheduler.yield()` / `yieldToMain` 拆分 long tasks，offload 到 Web Workers |
| Memory leaks | 内存增长，最终 crash | 清理 listeners、intervals、refs |
