# String Refs -所有迁移模式

##单独引用DOM元素{# Single - Ref}

最常见的情况是一个引用一个DOM节点。```jsx
// Before:
class SearchBox extends React.Component {
  handleSearch() {
    const value = this.refs.searchInput.value;
    this.props.onSearch(value);
  }

  focusInput() {
    this.refs.searchInput.focus();
  }

  render() {
    return (
      <div>
        <input ref="searchInput" type="text" placeholder="Search..." />
        <button onClick={() => this.handleSearch()}>Search</button>
      </div>
    );
  }
}
```

```jsx
// After:
class SearchBox extends React.Component {
  searchInputRef = React.createRef();

  handleSearch() {
    const value = this.searchInputRef.current.value;
    this.props.onSearch(value);
  }

  focusInput() {
    this.searchInputRef.current.focus();
  }

  render() {
    return (
      <div>
        <input ref={this.searchInputRef} type="text" placeholder="Search..." />
        <button onClick={() => this.handleSearch()}>Search</button>
      </div>
    );
  }
}
```
---

##多个ref在一个组件{# Multiple -ref}

每个字符串ref成为它自己的命名的`createRef()`字段。```jsx
// Before:
class LoginForm extends React.Component {
  handleSubmit(e) {
    e.preventDefault();
    const email = this.refs.emailField.value;
    const password = this.refs.passwordField.value;
    this.props.onSubmit({ email, password });
  }

  render() {
    return (
      <form onSubmit={this.handleSubmit}>
        <input ref="emailField" type="email" />
        <input ref="passwordField" type="password" />
        <button type="submit">Log in</button>
      </form>
    );
  }
}
```

```jsx
// After:
class LoginForm extends React.Component {
  emailFieldRef = React.createRef();
  passwordFieldRef = React.createRef();

  handleSubmit(e) {
    e.preventDefault();
    const email = this.emailFieldRef.current.value;
    const password = this.passwordFieldRef.current.value;
    this.props.onSubmit({ email, password });
  }

  render() {
    return (
      <form onSubmit={this.handleSubmit}>
        <input ref={this.emailFieldRef} type="email" />
        <input ref={this.passwordFieldRef} type="password" />
        <button type="submit">Log in</button>
      </form>
    );
  }
}
```
---

##引用列表/动态引用{# List - Refs}

字符串引用为map/loop-这是最棘手的情况。每个项目都需要自己的参考。```jsx
// Before:
class TabPanel extends React.Component {
  focusTab(index) {
    this.refs[`tab_${index}`].focus();
  }

  render() {
    return (
      <div>
        {this.props.tabs.map((tab, i) => (
          <button key={tab.id} ref={`tab_${i}`}>
            {tab.label}
          </button>
        ))}
      </div>
    );
  }
}
```

```jsx
// After - use a Map to store refs dynamically:
class TabPanel extends React.Component {
  tabRefs = new Map();

  getOrCreateRef(id) {
    if (!this.tabRefs.has(id)) {
      this.tabRefs.set(id, React.createRef());
    }
    return this.tabRefs.get(id);
  }

  focusTab(index) {
    const tab = this.props.tabs[index];
    this.tabRefs.get(tab.id)?.current?.focus();
  }

  render() {
    return (
      <div>
        {this.props.tabs.map((tab) => (
          <button key={tab.id} ref={this.getOrCreateRef(tab.id)}>
            {tab.label}
          </button>
        ))}
      </div>
    );
  }
}
```
**备选-列表的回调ref（更简单）：**```jsx
class TabPanel extends React.Component {
  tabRefs = {};

  focusTab(index) {
    this.tabRefs[index]?.focus();
  }

  render() {
    return (
      <div>
        {this.props.tabs.map((tab, i) => (
          <button
            key={tab.id}
            ref={el => { this.tabRefs[i] = el; }}  // callback ref stores DOM node directly
          >
            {tab.label}
          </button>
        ))}
      </div>
    );
  }
}
// Note: callback refs store the DOM node directly (not wrapped in .current)
// this.tabRefs[i] is the element, not this.tabRefs[i].current
```
---

## Callback Refs（替代createRef） {# Callback - Refs}

回调引用是`createRef()`的替代方法。它们对于列表（上面）很有用，当你需要运行代码时，refattaches/detaches.```jsx
// Callback ref syntax:
class MyComponent extends React.Component {
  // Callback ref - called with the element when it mounts, null when it unmounts
  setInputRef = (el) => {
    this.inputEl = el; // stores the DOM node directly (no .current needed)
  };

  focusInput() {
    this.inputEl?.focus(); // direct DOM node access
  }

  render() {
    return <input ref={this.setInputRef} />;
  }
}
```
**何时使用回调refs vs createRef:**

-`createRef()`-在组件定义时已知的固定数量的refs（大多数情况下）
-回调refs -对于动态列表，当你需要对attach/detach作出反应时，或者当refs可能改变时

**重要：**内联回调refs（在render中定义）在每次渲染时重新创建一个新函数，这会导致ref在每个渲染周期中被`null`调用，然后是元素。使用绑定方法或类字段箭头函数代替：```jsx
// AVOID - new function every render, causes ref flicker:
render() {
  return <input ref={(el) => { this.inputEl = el; }} />;  // inline - bad
}

// PREFER - stable reference:
setInputRef = (el) => { this.inputEl = el; };  // class field - good
render() {
  return <input ref={this.setInputRef} />;
}
```
---

## Ref传递给子组件{#forwarded-refs}

如果将字符串ref传递给自定义组件（不是DOM元素），则迁移还需要更新子组件。```jsx
// Before:
class Parent extends React.Component {
  handleClick() {
    this.refs.myInput.focus(); // Parent accesses child's DOM node
  }
  render() {
    return (
      <div>
        <MyInput ref="myInput" />
        <button onClick={() => this.handleClick()}>Focus</button>
      </div>
    );
  }
}

// MyInput.js (child - class component):
class MyInput extends React.Component {
  render() {
    return <input className="my-input" />;
  }
}
```

```jsx
// After:
class Parent extends React.Component {
  myInputRef = React.createRef();

  handleClick() {
    this.myInputRef.current.focus();
  }

  render() {
    return (
      <div>
        {/* React 18: forwardRef needed. React 19: ref is a direct prop */}
        <MyInput ref={this.myInputRef} />
        <button onClick={() => this.handleClick()}>Focus</button>
      </div>
    );
  }
}

// MyInput.js (React 18 - use forwardRef):
import { forwardRef } from 'react';
const MyInput = forwardRef(function MyInput(props, ref) {
  return <input ref={ref} className="my-input" />;
});

// MyInput.js (React 19 - ref as direct prop, no forwardRef):
function MyInput({ ref, ...props }) {
  return <input ref={ref} className="my-input" />;
}
```

---
