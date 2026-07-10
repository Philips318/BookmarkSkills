---
name: react18-string-refs
description: 'Provides exact migration patterns for React string refs (ref="name" + this.refs.name) to React.createRef() in class components. Use this skill whenever migrating string ref usage - including single element refs, multiple refs in a component, refs in lists, callback refs, and refs passed to child components. Always use this skill before writing any ref migration code - the multiple-refs-in-list pattern is particularly tricky and this skill prevents the most common mistakes. Use it for React 18.3.1 migration (string refs warn) and React 19 migration (string refs removed).'
---
# React 18 String Refs Migration

字符串refs （`ref="myInput"`+`this.refs.myInput`）在React 16.3中已弃用，在React 18.3.1中发出警告，并在React 19**中被删除。

快速模式映射

|模式|参考||---|---|
|[→patterns.md# Single -ref](references/patterns.md# Single -ref) |
|[→patterns.md# Multiple -refs](references/patterns.md# Multiple -refs) |
|[→patterns.md#list- Refs](references/patterns.md#list- Refs) |
|[→patterns.md# Callback -refs](references/patterns.md# Callback -refs) |
|[→patterns.md#forward -refs](references/patterns.md#forward -refs) |

##扫描命令```bash
# Find all string ref assignments in JSX
grep -rn 'ref="' src/ --include="*.js" --include="*.jsx" | grep -v "\.test\."

# Find all this.refs accessors
grep -rn "this\.refs\." src/ --include="*.js" --include="*.jsx" | grep -v "\.test\."
```
两者应该一起迁移—查找每个组件的`ref="name"`和`this.refs.name`访问对。

##迁移规则

每个字符串重新迁移到`React.createRef()`：

1. 添加`refName = React.createRef();`作为类字段（或在构造函数中）
2. 替换JSX中的`ref="refName"`→`ref={this.refName}`3. 替换`this.refs.refName`→`this.refName.current`阅读`references/patterns.md`以获得每种情况的完整before/after。