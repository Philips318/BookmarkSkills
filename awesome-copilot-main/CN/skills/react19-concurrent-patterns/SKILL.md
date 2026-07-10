---
name: react19-concurrent-patterns
description: 'Preserve React 18 concurrent patterns and adopt React 19 APIs (useTransition, useDeferredValue, Suspense, use(), useOptimistic, Actions) during migration.'
---
# React 19并发模式

React 19引入了新的api来补充迁移工作。这项技能包括两个方面：

1. **保留现有的React 18并发模式，在迁移过程中不能被破坏
2. **在迁移稳定后采用新的React 19 api

第一部分：React 18个必须在迁移中存活的并发模式

这些模式存在于React 18代码库中，不能被意外删除或破坏：

createRoot已经被R18管弦乐团迁移了

如果R18管弦乐队已经运行，则`ReactDOM.render`→`createRoot`完成。验证它是否正确：```jsx
// CORRECT React 19 root (same as React 18):
import { createRoot } from 'react-dom/client';
const root = createRoot(document.getElementById('root'));
root.render(
  <React.StrictMode>
    <App />
  </React.StrictMode>
);
```
useTransition不需要迁移

来自React 18的`useTransition`在React 19中工作相同。在迁移期间不要触摸这些模式：```jsx
// React 18 useTransition  unchanged in React 19:
const [isPending, startTransition] = useTransition();

function handleClick() {
  startTransition(() => {
    setFilteredResults(computeExpensiveFilter(input));
  });
}
```
useDeferredValue不需要迁移```jsx
// React 18 useDeferredValue  unchanged in React 19:
const deferredQuery = useDeferredValue(query);
```
代码分割的悬念，不需要迁移```jsx
// React 18 Suspense with lazy  unchanged in React 19:
const LazyComponent = React.lazy(() => import('./LazyComponent'));

function App() {
  return (
    <Suspense fallback={<Spinner />}>
      <LazyComponent />
    </Suspense>
  );
}
```
---

## Part 2 React 19新api

这些都值得在迁移后的清理冲刺中采用。不要在迁移期间引入这些特性。

有关每个新API的完整模式，请阅读：
**`references/react19-use.md`**`use()`钩子用于承诺和上下文
- **`references/react19-actions.md`** Actions, useActionState, useFormStatus, useOptimistic
**`references/react19-suspense.md`**数据获取的悬念（新模式）

##迁移安全规则

在React 19迁移过程中，这些并发模式必须完全保持不变：```bash
# Verify nothing touched these during migration:
grep -rn "useTransition\|useDeferredValue\|Suspense\|startTransition" \
  src/ --include="*.js" --include="*.jsx" | grep -v "\.test\."
```
如果迁移者接触了这些文件中的任何一个，检查迁移应该只修改了React API表面（forwardRef， defaultProps等），而不是并发模式逻辑。