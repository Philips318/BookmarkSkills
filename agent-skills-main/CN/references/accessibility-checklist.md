# Accessibility Checklist

WCAG 2.1 AA 合规的快速参考。与 `frontend-ui-engineering` skill 搭配使用。

## Table of Contents

- [Essential Checks](#essential-checks)
- [Common HTML Patterns](#common-html-patterns)
- [Testing Tools](#testing-tools)
- [Quick Reference: ARIA Live Regions](#quick-reference-aria-live-regions)
- [Common Anti-Patterns](#common-anti-patterns)

## Essential Checks

### Keyboard Navigation
- [ ] 所有交互元素都可通过 Tab 键聚焦
- [ ] Focus 顺序遵循视觉/逻辑顺序
- [ ] Focus 可见（聚焦元素上有 outline/ring）
- [ ] 自定义 widgets 支持键盘操作（Enter 激活，Escape 关闭）
- [ ] 没有 keyboard traps（用户始终可以 Tab 离开组件）
- [ ] 页面顶部有 skip-to-content link — 至少在键盘 focus 时可见
- [ ] Modals 打开时 trap focus，关闭时返回 focus

### Screen Readers
- [ ] 所有图片都有 `alt` 文本（装饰性图片使用 `alt=""`）
- [ ] 所有表单 inputs 都有关联 labels（`<label>` 或 `aria-label`）
- [ ] Buttons 和 links 有描述性文本（不是 “Click here”）
- [ ] 仅图标 buttons 有 `aria-label`
- [ ] 页面有一个 `<h1>`，并且 headings 不跳级
- [ ] 动态内容变化会被宣布（`aria-live` regions）
- [ ] Tables 有带 scope 的 `<th>` headers

### Visual
- [ ] 文本对比度 ≥ 4.5:1（普通文本）或 ≥ 3:1（大文本，18px+）
- [ ] UI components 与背景的对比度 ≥ 3:1
- [ ] 颜色不是传达信息的唯一方式
- [ ] 文本可放大到 200% 而不破坏布局
- [ ] 没有每秒闪烁超过 3 次的内容

### Forms
- [ ] 每个 input 都有可见 label
- [ ] Required fields 有标识（不能只靠颜色）
- [ ] Error messages 具体，并与对应 field 关联
- [ ] Error state 不只靠颜色可见（icon、text、border）
- [ ] 表单提交错误有 summary 且可 focus
- [ ] 已知字段使用 autocomplete（例如 `type="email" autocomplete="email"`）

### Content
- [ ] 声明语言（`<html lang="en">`）
- [ ] 页面有描述性 `<title>`
- [ ] Links 能与周围文本区分（不能只靠颜色）
- [ ] 移动端 touch targets ≥ 44x44px
- [ ] 有意义的 empty states（不是空白屏幕）

## Common HTML Patterns

### Buttons vs. Links

```html
<!-- Use <button> for actions -->
<button onClick={handleDelete}>Delete Task</button>

<!-- Use <a> for navigation -->
<a href="/tasks/123">View Task</a>

<!-- NEVER use div/span as buttons -->
<div onClick={handleDelete}>Delete</div>  <!-- BAD -->
```

### Form Labels

```html
<!-- Explicit label association -->
<label htmlFor="email">Email address</label>
<input id="email" type="email" required />

<!-- Implicit wrapping -->
<label>
  Email address
  <input type="email" required />
</label>

<!-- Hidden label (visible label preferred) -->
<input type="search" aria-label="Search tasks" />
```

### ARIA Roles

```html
<!-- Navigation -->
<nav aria-label="Main navigation">...</nav>
<nav aria-label="Footer links">...</nav>

<!-- Status messages -->
<div role="status" aria-live="polite">Task saved</div>

<!-- Alert messages -->
<div role="alert">Error: Title is required</div>

<!-- Modal dialogs -->
<dialog aria-modal="true" aria-labelledby="dialog-title">
  <h2 id="dialog-title">Confirm Delete</h2>
  ...
</dialog>

<!-- Loading states -->
<div aria-busy="true" aria-label="Loading tasks">
  <Spinner />
</div>
```

### Accessible Lists

```html
<ul role="list" aria-label="Tasks">
  <li>
    <input type="checkbox" id="task-1" aria-label="Complete: Buy groceries" />
    <label htmlFor="task-1">Buy groceries</label>
  </li>
</ul>
```

## Testing Tools

```bash
# Automated audit
npx axe-core          # Programmatic accessibility testing
npx pa11y             # CLI accessibility checker

# In browser
# Chrome DevTools → Lighthouse → Accessibility
# Chrome DevTools → Elements → Accessibility tree

# Screen reader testing
# macOS: VoiceOver (Cmd + F5)
# Windows: NVDA (free) or JAWS
# Linux: Orca
```

## Quick Reference: ARIA Live Regions

| Value | Behavior | Use For |
|-------|----------|---------|
| `aria-live="polite"` | 在下一次停顿时宣布 | 状态更新、保存确认 |
| `aria-live="assertive"` | 立即宣布 | 错误、时间敏感 alerts |
| `role="status"` | 与 `polite` 相同 | 状态消息 |
| `role="alert"` | 与 `assertive` 相同 | 错误消息 |

## Common Anti-Patterns

| Anti-Pattern | Problem | Fix |
|---|---|---|
| 用 `div` 当 button | 不可 focus，没有键盘支持 | 使用 `<button>` |
| 缺少 `alt` 文本 | 图片对 screen readers 不可见 | 添加描述性 `alt` |
| 只靠颜色表示状态 | 色盲用户不可见 | 添加 icons、text 或 patterns |
| Autoplaying media | 造成迷失感，无法停止 | 添加 controls，不要 autoplay |
| 没有 ARIA 的自定义 dropdown | 键盘/screen reader 无法使用 | 使用原生 `<select>` 或正确的 ARIA listbox |
| 移除 focus outlines | 用户看不到自己在哪里 | 样式化 outlines，不要移除 |
| 空 links/buttons | 被宣布为“Link”但没有描述 | 添加文本或 `aria-label` |
| `tabindex > 0` | 破坏自然 tab 顺序 | 只使用 `tabindex="0"` 或 `-1` |
