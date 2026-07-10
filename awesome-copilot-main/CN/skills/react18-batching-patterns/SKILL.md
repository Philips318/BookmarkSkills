---
name: react18-batching-patterns
description: 'Provides exact patterns for diagnosing and fixing automatic batching regressions in React 18 class components. Use this skill whenever a class component has multiple setState calls in an async method, inside setTimeout, inside a Promise .then() or .catch(), or in a native event handler. Use it before writing any flushSync call - the decision tree here prevents unnecessary flushSync overuse. Also use this skill when fixing test failures caused by intermediate state assertions that break after React 18 upgrade.'
---
# React 18自动批处理模式

用于诊断和修复React 18中类组件代码库中最危险的无声破坏更改的参考。

##核心改变

| setState的位置| React 17 | React 18 ||---|---|---|
| React事件处理程序| Batched | Batched（相同）|
| setTimeout | **立即重新渲染** | **批处理** |
|保证。然后()/ .catch() | * *立即重新呈现* * | * *成批的* * |
|async/await| **立即重新渲染** | **批处理** |
|本机addEventListener回调| **立即重新渲染** | **批处理** |

**批处理**意味着：在执行上下文中的所有setState调用在最后一次重新渲染中一起刷新。没有中间渲染发生。

##快速诊断

读取每个异步类方法。问：在`await`之后是否有代码读取`this.state`来做出决定？```
Code reads this.state after await?
  YES → Category A (silent state-read bug)
  NO, but intermediate render must be visible to user?
    YES → Category C (flushSync needed)
    NO → Category B (refactor, no flushSync)
```
要了解每个类别的完整模式，请阅读：
- **`references/batching-categories.md`** - A， B， C类完整的before/after代码
- **`references/flushSync-guide.md`** -何时使用flushSync，何时不使用，导入语法

## flushSync规则

**谨慎使用`flushSync`。**它强制同步重新渲染，绕过React 18的并发调度程序。过度使用它会抵消React 18的性能优势。

仅在以下情况下使用`flushSync`：
-在异步操作开始之前，用户必须看到一个中间UI状态
-一个spinner/loading状态必须在抓取开始之前呈现
-连续的UI步骤有不同的可见状态（进度向导，多步骤流）

在大多数情况下，修复方法是重构——重构代码，使其在`await`之后不读取`this.state`。阅读`references/batching-categories.md`了解每个类别的正确方法。