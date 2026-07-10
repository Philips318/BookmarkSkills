---
name: react19-test-patterns
description: 'Provides before/after patterns for migrating test files to React 19 compatibility, including act() imports, Simulate removal, and StrictMode call count changes.'
---
测试迁移模式

参考React 19所需的所有测试文件迁移。

##优先顺序

按此顺序修复测试文件；每一层都依赖于前一层：

1. **`act`首先导入** fix，它将解锁其他所有内容
2. **`Simulate`→`fireEvent`**修复后立即行动
3. **完全react-dom/test-utils清理**删除剩余的导入
4. **StrictMode调用计数**测量实际，不要猜测
5. **异步行为包装**用于剩余的“未在行为中包装”警告
6. **自定义渲染助手**每个代码库验证一次，而不是每个测试

---

# # 1。act（）导入修复```jsx
// Before  REMOVED in React 19:
import { act } from 'react-dom/test-utils';

// After:
import { act } from 'react';
```
如果与其他test-utils导入混合使用：```jsx
// Before:
import { act, Simulate, renderIntoDocument } from 'react-dom/test-utils';

// After  split the imports:
import { act } from 'react';
import { fireEvent, render } from '@testing-library/react'; // replaces Simulate + renderIntoDocument
```
---

# # 2。模拟→fireEvent```jsx
// Before  Simulate REMOVED in React 19:
import { Simulate } from 'react-dom/test-utils';
Simulate.click(element);
Simulate.change(input, { target: { value: 'hello' } });
Simulate.submit(form);
Simulate.keyDown(element, { key: 'Enter', keyCode: 13 });

// After:
import { fireEvent } from '@testing-library/react';
fireEvent.click(element);
fireEvent.change(input, { target: { value: 'hello' } });
fireEvent.submit(form);
fireEvent.keyDown(element, { key: 'Enter', keyCode: 13 });
```
---

# # 3。react-dom/test-utils完整API映射

|旧（react-dom/test-utils） |新位置||---|---|
|`act`|`import { act } from 'react'`|
|`Simulate`|`fireEvent`从`@testing-library/react`|
|`renderIntoDocument`|`render`从`@testing-library/react`|
|`findRenderedDOMComponentWithTag`|`getByRole`，`getByTestId`来自RTL |
|`findRenderedDOMComponentWithClass`|`getByRole`或`container.querySelector`|
|`scryRenderedDOMComponentsWithTag`|`getAllByRole`来自RTL |
|`isElement`，`isCompositeComponent`|删除不需要与RTL |
|`isDOMComponent`|移除|

---

# # 4。修复了StrictMode呼叫计数

React 19 StrictMode在开发中不再重复调用`useEffect`。间谍断言计数效果调用必须更新。

**策略总是衡量，而不是猜测```bash
# Run the failing test, read the actual count from the error:
npm test -- --watchAll=false --testPathPattern="[filename]" --forceExit 2>&1 | grep -E "Expected|Received"
```

```jsx
// Before (React 18 StrictMode  effects ran twice):
expect(mockFn).toHaveBeenCalledTimes(2);  // 1 call × 2 (strict double-invoke)

// After (React 19 StrictMode  effects run once):
expect(mockFn).toHaveBeenCalledTimes(1);
```

```jsx
// Render-phase calls (component body)  still double-invoked in React 19 StrictMode:
expect(renderSpy).toHaveBeenCalledTimes(2);  // stays at 2 for render body calls
