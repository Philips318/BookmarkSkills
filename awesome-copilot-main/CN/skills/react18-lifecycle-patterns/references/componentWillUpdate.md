# componentWillUpdate迁移引用

核心决策```
Does componentWillUpdate read the DOM (scroll, size, position, selection)?
  YES → getSnapshotBeforeUpdate (paired with componentDidUpdate)
  NO (side effects, request cancellation, etc.) → componentDidUpdate
```
---

## Case -在重新渲染之前读取DOM {# Case - A}

该方法在React应用下一次更新之前捕获DOM测量（滚动位置、元素大小、光标位置），因此可以在之后恢复或调整它。

* *: * *```jsx
class MessageList extends React.Component {
  componentWillUpdate(nextProps) {
    if (nextProps.messages.length > this.props.messages.length) {
      this.savedScrollHeight = this.listRef.current.scrollHeight;
      this.savedScrollTop = this.listRef.current.scrollTop;
    }
  }

  componentDidUpdate(prevProps) {
    if (prevProps.messages.length < this.props.messages.length) {
      const scrollDelta = this.listRef.current.scrollHeight - this.savedScrollHeight;
      this.listRef.current.scrollTop = this.savedScrollTop + scrollDelta;
    }
  }
}
```
** getSnapshotBeforeUpdate + componentDidUpdate:**```jsx
class MessageList extends React.Component {
  // Called right before DOM updates are applied - perfect timing to read DOM
  getSnapshotBeforeUpdate(prevProps, prevState) {
    if (prevProps.messages.length < this.props.messages.length) {
      return {
        scrollHeight: this.listRef.current.scrollHeight,
        scrollTop: this.listRef.current.scrollTop,
      };
    }
    return null; // Return null when snapshot is not needed
  }

  // Receives the snapshot as the third argument
  componentDidUpdate(prevProps, prevState, snapshot) {
    if (snapshot !== null) {
      const scrollDelta = this.listRef.current.scrollHeight - snapshot.scrollHeight;
      this.listRef.current.scrollTop = snapshot.scrollTop + scrollDelta;
    }
  }
}
```
**为什么这比componentWillUpdate更好：**在React 18并发模式下，`componentWillUpdate`运行和DOM实际更新之间可能存在差距。`componentWillUpdate`中的DOM读取可能过时。`getSnapshotBeforeUpdate`在DOM提交之前同步运行——读取总是准确的。

合同* *:* *

—从`getSnapshotBeforeUpdate`返回一个值→该值在`componentDidUpdate`中变为`snapshot`—返回`null`→`snapshot`，其中`componentDidUpdate`为`null`—总是在`componentDidUpdate`中检查`if (snapshot !== null)`-`getSnapshotBeforeUpdate`必须与`componentDidUpdate`配对

---

##案例B -更新前的副作用{# Case - B}

该方法在道具或状态即将更改时取消正在执行的请求、清除计时器或运行一些预备副作用。

* *: * *```jsx
class SearchResults extends React.Component {
  componentWillUpdate(nextProps) {
    if (nextProps.query !== this.props.query) {
      this.currentRequest?.cancel();
      this.setState({ loading: true, results: [] });
    }
  }
}
```
**后-移动到componentDidUpdate（运行后更新）：**```jsx
class SearchResults extends React.Component {
  componentDidUpdate(prevProps) {
    if (prevProps.query !== this.props.query) {
      // Cancel the stale request
      this.currentRequest?.cancel();
      // Start the new request for the updated query
      this.setState({ loading: true, results: [] });
      this.currentRequest = searchAPI(this.props.query)
        .then(results => this.setState({ results, loading: false }));
    }
  }
}
```
**注意：**的副作用现在运行后渲染，而不是之前。在大多数情况下，这是正确的——你想对实际显示的状态做出反应，而不是显示的状态。如果你真的需要在渲染之前同步运行一些东西，重新考虑设计——这通常表明应该以不同的方式管理状态。

---

两种情况在一个组件中

如果组件在`componentWillUpdate`中同时具有dom读取和副作用：```jsx
// Before: does both
componentWillUpdate(nextProps) {
  // DOM read
  if (isExpanding(nextProps)) {
    this.savedHeight = this.ref.current.offsetHeight;
  }
  // Side effect
  if (nextProps.query !== this.props.query) {
    this.request?.cancel();
  }
}
```
后：分为两种模式：```jsx
// DOM read → getSnapshotBeforeUpdate
getSnapshotBeforeUpdate(prevProps, prevState) {
  if (isExpanding(this.props)) {
    return { height: this.ref.current.offsetHeight };
  }
  return null;
}

// Side effect → componentDidUpdate
componentDidUpdate(prevProps, prevState, snapshot) {
  // Handle snapshot if present
  if (snapshot !== null) { /* ... */ }

  // Handle side effect
  if (prevProps.query !== this.props.query) {
    this.request?.cancel();
    this.startNewRequest();
  }
}
```
