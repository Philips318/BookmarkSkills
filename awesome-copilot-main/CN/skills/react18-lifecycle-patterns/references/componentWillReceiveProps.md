# componentWillReceiveProps迁移引用

核心决策```
Does componentWillReceiveProps trigger async work or side effects?
  YES → componentDidUpdate
  NO (pure state derivation only) → getDerivedStateFromProps
```
当有疑问时：使用`componentDidUpdate`。它总是安全的。
当逻辑不是纯同步状态派生时，`getDerivedStateFromProps`具有陷阱（请参阅本文件底部），使其成为错误的选择。

---

## Case A -异步副作用/获取道具更改{# Case - A}

该方法在道具更改时获取数据、取消请求、更新外部状态或运行任何异步操作。

* *: * *```jsx
class UserProfile extends React.Component {
  componentWillReceiveProps(nextProps) {
    if (nextProps.userId !== this.props.userId) {
      this.setState({ loading: true, profile: null });
      fetchProfile(nextProps.userId)
        .then(profile => this.setState({ profile, loading: false }))
        .catch(err => this.setState({ error: err, loading: false }));
    }
  }
}
```
**后组件didupdate:**```jsx
class UserProfile extends React.Component {
  componentDidUpdate(prevProps) {
    if (prevProps.userId !== this.props.userId) {
      // Use this.props (not nextProps - the update already happened)
      this.setState({ loading: true, profile: null });
      fetchProfile(this.props.userId)
        .then(profile => this.setState({ profile, loading: false }))
        .catch(err => this.setState({ error: err, loading: false }));
    }
  }
}
```
**关键区别：**`componentDidUpdate`接收`prevProps`-你比较`prevProps.x !== this.props.x`而不是`this.props.x !== nextProps.x`。更新已经应用。

**取消模式**（对async很重要）：```jsx
class UserProfile extends React.Component {
  _requestId = 0;

  componentDidUpdate(prevProps) {
    if (prevProps.userId !== this.props.userId) {
      const requestId = ++this._requestId;
      this.setState({ loading: true });
      fetchProfile(this.props.userId).then(profile => {
        // Ignore stale responses if userId changed again
        if (requestId === this._requestId) {
          this.setState({ profile, loading: false });
        }
      });
    }
  }
}
```
---

##案例B -纯状态派生自Props {# Case - B}

该方法只同步地从新道具中派生状态值。没有异步工作，没有副作用，没有外部调用。

* *: * *```jsx
class SortedList extends React.Component {
  componentWillReceiveProps(nextProps) {
    if (nextProps.items !== this.props.items) {
      this.setState({
        sortedItems: [...nextProps.items].sort((a, b) => a.name.localeCompare(b.name)),
      });
    }
  }
}
```
**后- getDerivedStateFromProps:**```jsx
class SortedList extends React.Component {
  // Must track previous prop to detect changes
  static getDerivedStateFromProps(props, state) {
    if (props.items !== state.prevItems) {
      return {
        sortedItems: [...props.items].sort((a, b) => a.name.localeCompare(b.name)),
        prevItems: props.items, // ← always store the prop you're comparing
      };
    }
    return null; // null = no state change
  }

  constructor(props) {
    super(props);
    this.state = {
      sortedItems: [...props.items].sort((a, b) => a.name.localeCompare(b.name)),
      prevItems: props.items, // ← initialize in constructor too
    };
  }
}
```
---

## getDerivedStateFromProps -陷阱和警告

陷阱1：它在每次渲染时触发，而不仅仅是道具改变

与`componentWillReceiveProps`不同，`getDerivedStateFromProps`在每次渲染之前被调用——包括`setState`调用。始终与存储在状态中的先前值进行比较。```jsx
// WRONG - fires on every render, including setState triggers
static getDerivedStateFromProps(props, state) {
  return { sortedItems: sort(props.items) }; // re-sorts on every setState!
}

// CORRECT - only updates when items reference changes
static getDerivedStateFromProps(props, state) {
  if (props.items !== state.prevItems) {
    return { sortedItems: sort(props.items), prevItems: props.items };
  }
  return null;
}
```
Trap 2：无法访问`this``getDerivedStateFromProps`是一个静态方法。没有`this.props`，没有`this.state`，没有实例方法。```jsx
// WRONG - no this in static method
static getDerivedStateFromProps(props, state) {
  return { value: this.computeValue(props) }; // ReferenceError
}

// CORRECT - pure function of props + state
static getDerivedStateFromProps(props, state) {
  return { value: computeValue(props) }; // standalone function
}
```
陷阱3：不要因为副作用而使用它

如果您需要在道具更改时获取-使用`componentDidUpdate`。`getDerivedStateFromProps`必须是纯的。

当getDerivedStateFromProps实际上是错误的工具时

如果您发现自己在`getDerivedStateFromProps`中执行复杂的逻辑，请考虑是否应该将消费组件作为道具接收预处理数据。该模式适用于狭窄的用例，而不是一般的从支持到状态同步。