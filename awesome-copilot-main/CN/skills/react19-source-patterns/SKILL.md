---
name: react19-source-patterns
description: 'Reference for React 19 source-file migration patterns, including API changes, ref handling, and context updates.'
---
# React 19源迁移模式

React 19所需的每个源文件迁移的参考。

##快速参考表

|模式|动作|参考||---|---|---|
|`ReactDOM.render(...)`|→`createRoot().render()`|参见references/api-migrations.md|
|`ReactDOM.hydrate(...)`|→`hydrateRoot(...)`|参见references/api-migrations.md|
|`unmountComponentAtNode`|→`root.unmount()`|内联固定|
|`ReactDOM.findDOMNode`|→直接ref |内联修复|
|`forwardRef(...)`包装|→ref as direct prop |参见references/api-migrations.md|
|`Component.defaultProps = {}`|→ES6默认参数|参见references/api-migrations.md|
|`useRef()`no arg |→`useRef(null)`|内联修复添加`null`|
|→`createContext`|[→api-migrations.md# Legacy - Context](references/api-migrations.md# Legacy - Context) |
| String refs`this.refs.x`|→`createRef()`|[→api-migrations.md# String -refs](references/api-migrations.md# String -refs) |
|`import React from 'react'`(unused) |删除|仅当文件|中没有`React.`的使用率时

## PropTypes规则

**不要**删除`.propTypes`赋值。`prop-types`包仍然作为一个独立的验证器工作。React 19只是从React包中移除内置的运行时检查，包本身仍然有效。

在任何`.propTypes`块上添加以下注释：```jsx
// NOTE: React 19 no longer runs propTypes validation at runtime.
// PropTypes kept for documentation and IDE tooling only.
```
##阅读参考资料

有关每次迁移的完整before/after代码，请阅读**`references/api-migrations.md`**。它包含完整的模式，包括`forwardRef`与`useImperativeHandle`、`defaultProps`null与未定义行为的边界用例，以及遗留上下文provider/consumer跨文件迁移。