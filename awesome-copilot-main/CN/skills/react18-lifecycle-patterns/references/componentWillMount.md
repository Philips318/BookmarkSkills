# componentWillMount迁移引用

## Case -初始化状态{# Case - A}

该方法仅使用不依赖于异步操作的静态或计算值调用`this.setState()`。

* *: * *```jsx
class UserList extends React.Component {
  componentWillMount() {
    this.setState({ items: [], loading: false, page: 1 });
  }
  render() { ... }
}
```
**后移动到构造函数：**```jsx
class UserList extends React.Component {
  constructor(props) {
    super(props);
    this.state = { items: [], loading: false, page: 1 };
  }
  render() { ... }
}
```
**如果构造函数已经存在**，合并状态：```jsx
class UserList extends React.Component {
  constructor(props) {
    super(props);
    // Existing state merged with componentWillMount state:
    this.state = {
      ...this.existingState,  // whatever was already here
      items: [],
      loading: false,
      page: 1,
    };
  }
}
```
---

## Case B -运行副作用{# Case - B}

该方法获取数据、设置订阅、与外部api交互或访问DOM。

* *: * *```jsx
class UserDashboard extends React.Component {
  componentWillMount() {
    this.subscription = this.props.eventBus.subscribe(this.handleEvent);
    fetch(`/api/users/${this.props.userId}`)
      .then(r => r.json())
      .then(user => this.setState({ user, loading: false }));
    this.setState({ loading: true });
  }
}
```
**后-移动到componentDidMount:**```jsx
class UserDashboard extends React.Component {
  constructor(props) {
    super(props);
    this.state = { loading: true, user: null }; // initial state here
  }

  componentDidMount() {
    // All side effects move here - runs after first render
    this.subscription = this.props.eventBus.subscribe(this.handleEvent);
    fetch(`/api/users/${this.props.userId}`)
      .then(r => r.json())
      .then(user => this.setState({ user, loading: false }));
  }

  componentWillUnmount() {
    // Always pair subscriptions with cleanup
    this.subscription?.unsubscribe();
  }
}
```
**为什么安全：**在React 18并发模式下，`componentWillMount`可以在挂载前多次调用。里面的副作用可以发射多次。`componentDidMount`保证在安装后精确射击一次。

---

## Case C -从Props {# Case - C}派生初始状态

该方法读取`this.props`以计算初始状态值。

* *: * *```jsx
class PriceDisplay extends React.Component {
  componentWillMount() {
    this.setState({
      formattedPrice: `$${this.props.price.toFixed(2)}`,
      isDiscount: this.props.price < this.props.originalPrice,
    });
  }
}
```
**带道具的后构造函数：**```jsx
class PriceDisplay extends React.Component {
  constructor(props) {
    super(props);
    this.state = {
      formattedPrice: `$${props.price.toFixed(2)}`,
      isDiscount: props.price < props.originalPrice,
    };
  }
}
```
**注：**如果这个初始状态需要更新当道具改变后，这是一个`getDerivedStateFromProps`的情况-参见`componentWillReceiveProps.md`情况B。

---

在一个方法中使用多个模式

如果单个`componentWillMount`同时状态init和副作用：```jsx
// Mixed - state init + fetch
componentWillMount() {
  this.setState({ loading: true, items: [] });              // Case A
  fetch('/api/items').then(r => r.json())                   // Case B
    .then(items => this.setState({ items, loading: false }));
}
```
将它们:```jsx
constructor(props) {
  super(props);
  this.state = { loading: true, items: [] }; // Case A → constructor
}

componentDidMount() {
  fetch('/api/items').then(r => r.json())    // Case B → componentDidMount
    .then(items => this.setState({ items, loading: false }));
}
```
