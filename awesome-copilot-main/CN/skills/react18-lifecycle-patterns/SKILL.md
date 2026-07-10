---
name: react18-lifecycle-patterns
description: 'Provides exact before/after migration patterns for the three unsafe class component lifecycle methods - componentWillMount, componentWillReceiveProps, and componentWillUpdate - targeting React 18.3.1. Use this skill whenever a class component needs its lifecycle methods migrated, when deciding between getDerivedStateFromProps vs componentDidUpdate, when adding getSnapshotBeforeUpdate, or when fixing React 18 UNSAFE_ lifecycle warnings. Always use this skill before writing any lifecycle migration code - do not guess the pattern from memory, the decision trees here prevent the most common migration mistakes.'
---
# React 18生命周期模式

将三个不安全的类组件生命周期方法迁移到React 18.3.1兼容模式的参考。

快速决策指南

在迁移任何生命周期方法之前，确定该方法所做的工作的语义类别。错误的类别=错误的迁移。下表将您路由到正确的参考文件。

componentWillMount——它是做什么的？

|功能|正确迁移|参考||---|---|---|
|设置初始状态（`this.setState(...)`） |移动到`constructor`|[→componentWillMount.md](references/componentWillMount.md#case-a) |
|移动到`componentDidMount`|[→componentWillMount.md](references/componentWillMount.md#case-b) |
用|[→componentWillMount.md](references/componentWillMount.md#case-c) |移动到`constructor`componentWillReceiveProps——它是做什么的？

|功能|正确迁移|参考||---|---|---|
|异步副作用触发的道具更改（取，取消）|`componentDidUpdate`|[→componentWillReceiveProps.md](references/componentWillReceiveProps.md#case-a) |
|新道具纯状态派生（无副作用）|`getDerivedStateFromProps`|[→componentWillReceiveProps.md](references/componentWillReceiveProps.md#case-b) |

componentWillUpdate -它做什么？

|功能|正确迁移|参考||---|---|---|
|在更新（滚动，大小，位置）之前读取DOM |`getSnapshotBeforeUpdate`|[→componentWillUpdate.md](references/componentWillUpdate.md#case-a) |
|在更新|之前取消请求/运行效果`componentDidUpdate`与先前的比较|[→componentWillUpdate.md](references/componentWillUpdate.md#case-b) |

---

UNSAFE_前缀规则

**永远不要使用`UNSAFE_componentWillMount`、`UNSAFE_componentWillReceiveProps`或`UNSAFE_componentWillUpdate`作为永久修复

前缀会抑制React 18.3.1警告，但不会：
修复并发模式的安全问题
-准备React 19的代码库（删除这些，带或不带前缀）
-修复迁移要解决的底层语义问题

UNSAFE_前缀只适合在调度真正的迁移冲刺时用作临时保留。将UNSAFE_前缀添加标记为：```jsx
// TODO: React 19 will remove this. Migrate before React 19 upgrade.
// UNSAFE_ prefix added temporarily - replace with componentDidMount / getDerivedStateFromProps / etc.
```
---

##参考文件

请阅读您正在迁移的生命周期方法的完整参考文件：

- **`references/componentWillMount.md`** - 3个完整的before/after代码
**`references/componentWillReceiveProps.md`** - getDerivedStateFromProps陷阱警告，完整示例
- **`references/componentWillUpdate.md`** - getSnapshotBeforeUpdate + componentDidUpdate配对

在编写任何迁移代码之前，请阅读相关文件。