---
title: React 19 API Migrations Reference
---
# React 19 API迁移参考

完成before/after模式的所有React 19突破性的变化和删除的api。

---

## ReactDOM根API迁移

React 19对所有应用都要求`createRoot()`或`hydrateRoot()`。如果React 18迁移已经运行，那么就完成了。验证它是正确的。

模式1:createRoot() CSR App```jsx
// Before (React 18 or earlier):
import ReactDOM from 'react-dom';
ReactDOM.render(<App />, document.getElementById('root'));

// After (React 19):
import { createRoot } from 'react-dom/client';
const root = createRoot(document.getElementById('root'));
root.render(<App />);
```
模式2:hydrateRoot()SSR/StaticApp```jsx
// Before (React 18 server-rendered app):
import ReactDOM from 'react-dom';
ReactDOM.hydrate(<App />, document.getElementById('root'));

// After (React 19):
import { hydrateRoot } from 'react-dom/client';
hydrateRoot(document.getElementById('root'), <App />);
```
模式3:unmountComponentAtNode（）已移除```jsx
// Before (React 18):
import ReactDOM from 'react-dom';
ReactDOM.unmountComponentAtNode(container);

// After (React 19):
const root = createRoot(container); // Save the root reference
// later:
root.unmount();
```
**警告：**如果根引用从未保存，您必须重构以传递它或使用全局注册表。

---

## findDOMNode（）删除

模式1：直接参考```jsx
// Before (React 18):
import { findDOMNode } from 'react-dom';
const domNode = findDOMNode(componentRef);

// After (React 19):
const domNode = componentRef.current; // refs point directly to DOM
```
模式2：类组件引用```jsx
// Before (React 18):
import { findDOMNode } from 'react-dom';
class MyComponent extends React.Component {
  render() {
    return <div ref={ref => this.node = ref}>Content</div>;
  }
  
  getWidth() {
    return findDOMNode(this).offsetWidth;
  }
}

// After (React 19):
// Note: findDOMNode() is removed in React 19. Eliminate the call entirely
// and use direct refs to access DOM nodes instead.
class MyComponent extends React.Component {
  nodeRef = React.createRef();
  
  render() {
    return <div ref={this.nodeRef}>Content</div>;
  }
  
  getWidth() {
    return this.nodeRef.current.offsetWidth;
  }
}
```
---

## forwardRef() -可选的现代化

模式1：函数组件直接引用```jsx
// Before (React 18):
import { forwardRef } from 'react';

const Input = forwardRef((props, ref) => (
  <input ref={ref} {...props} />
));

function App() {
  const inputRef = useRef(null);
  return <Input ref={inputRef} />;
}

// After (React 19):
// Simply accept ref as a regular prop:
function Input({ ref, ...props }) {
  return <input ref={ref} {...props} />;
}

function App() {
  const inputRef = useRef(null);
  return <Input ref={inputRef} />;
}
```
模式2:forwardRef + useImperativeHandle```jsx
// Before (React 18):
import { forwardRef, useImperativeHandle } from 'react';

const TextInput = forwardRef((props, ref) => {
  const inputRef = useRef();
  
  useImperativeHandle(ref, () => ({
    focus: () => inputRef.current.focus(),
    clear: () => { inputRef.current.value = ''; }
  }));
  
  return <input ref={inputRef} {...props} />;
});

function App() {
  const textRef = useRef(null);
  return (
    <>
      <TextInput ref={textRef} />
      <button onClick={() => textRef.current.focus()}>Focus</button>
    </>
  );
}

// After (React 19):
function TextInput({ ref, ...props }) {
  const inputRef = useRef(null);
  
  useImperativeHandle(ref, () => ({
    focus: () => inputRef.current.focus(),
    clear: () => { inputRef.current.value = ''; }
  }));
  
  return <input ref={inputRef} {...props} />;
}

function App() {
  const textRef = useRef(null);
  return (
    <>
      <TextInput ref={textRef} />
      <button onClick={() => textRef.current.focus()}>Focus</button>
    </>
  );
}
```
**注：**`useImperativeHandle`仍然有效；只删除`forwardRef`包装器。

---

删除defaultProps

模式1：带有defaultProps的函数组件```jsx
// Before (React 18):
function Button({ label = 'Click', disabled = false }) {
  return <button disabled={disabled}>{label}</button>;
}

// WORKS BUT is removed in React 19:
Button.defaultProps = {
  label: 'Click',
  disabled: false
};

// After (React 19):
// ES6 default params are now the ONLY way:
function Button({ label = 'Click', disabled = false }) {
  return <button disabled={disabled}>{label}</button>;
}

// Remove all defaultProps assignments
```
模式2：类组件defaultProps```jsx
// Before (React 18):
class Button extends React.Component {
  static defaultProps = {
    label: 'Click',
    disabled: false
  };
  
  render() {
    return <button disabled={this.props.disabled}>{this.props.label}</button>;
  }
}

// After (React 19):
// Use default params in constructor or class field:
class Button extends React.Component {
  constructor(props) {
    super(props);
    this.label = props.label || 'Click';
    this.disabled = props.disabled || false;
  }
  
  render() {
    return <button disabled={this.disabled}>{this.label}</button>;
  }
}

// Or simplify to function component with ES6 defaults:
function Button({ label = 'Click', disabled = false }) {
  return <button disabled={disabled}>{label}</button>;
}
```
模式3:defaultProps为null```jsx
// Before (React 18):
function Component({ value }) {
  // defaultProps can set null to reset a parent-passed value
  return <div>{value}</div>;
}

Component.defaultProps = {
  value: null
};

// After (React 19):
// Use explicit null checks or nullish coalescing:
function Component({ value = null }) {
  return <div>{value}</div>;
}

// Or:
function Component({ value }) {
  return <div>{value ?? null}</div>;
}
```
---

## useRef没有初始值

模式1:useRef（）```jsx
// Before (React 18):
const ref = useRef(); // undefined initially

// After (React 19):
// Explicitly pass null as initial value:
const ref = useRef(null);

// Then use current:
ref.current = someElement; // Set it manually later
```
模式2:useRef带有DOM元素```jsx
// Before:
function Component() {
  const inputRef = useRef();
  return <input ref={inputRef} />;
}

// After:
function Component() {
  const inputRef = useRef(null); // Explicit null
  return <input ref={inputRef} />;
}
```
---

删除了遗留上下文API

模式1：反应。createContext vs contextTypes```jsx
// Before (React 18  not recommended but worked):
// Using contextTypes (old PropTypes-style context):
class MyComponent extends React.Component {
  static contextTypes = {
    theme: PropTypes.string
  };
  
  render() {
    return <div style={{ color: this.context.theme }}>Text</div>;
  }
}

// Provider using getChildContext (old API):
class App extends React.Component {
  static childContextTypes = {
    theme: PropTypes.string
  };
  
  getChildContext() {
    return { theme: 'dark' };
  }
  
  render() {
    return <MyComponent />;
  }
}

// After (React 19):
// Use createContext (modern API):
const ThemeContext = React.createContext(null);

function MyComponent() {
  const theme = useContext(ThemeContext);
  return <div style={{ color: theme }}>Text</div>;
}

function App() {
  return (
    <ThemeContext.Provider value="dark">
      <MyComponent />
    </ThemeContext.Provider>
  );
}
```
模式2：类组件使用createContext```jsx
// Before (class component consuming old context):
class MyComponent extends React.Component {
  static contextType = ThemeContext;
  
  render() {
    return <div style={{ color: this.context }}>Text</div>;
  }
}

// After (still works in React 19):
// No change needed for static contextType
// Continue using this.context
```
**重要：**如果您仍在使用旧的`contextTypes`+`getChildContext`模式（不是现代的`createContext`），您**必须**迁移到`createContext`，旧模式将被完全删除。

---

##删除字符串引用

模式1：这个。refs字符串引用数```jsx
// Before (React 18):
class Component extends React.Component {
  render() {
    return (
      <>
        <input ref="inputRef" />
        <button onClick={() => this.refs.inputRef.focus()}>Focus</button>
      </>
    );
  }
}

// After (React 19):
class Component extends React.Component {
  inputRef = React.createRef();
  
  render() {
    return (
      <>
        <input ref={this.inputRef} />
        <button onClick={() => this.inputRef.current.focus()}>Focus</button>
      </>
    );
  }
}
```
模式2：回调Refs（推荐）```jsx
// Before (React 18):
class Component extends React.Component {
  render() {
    return (
      <>
        <input ref="inputRef" />
        <button onClick={() => this.refs.inputRef.focus()}>Focus</button>
      </>
    );
  }
}

// After (React 19  callback is more flexible):
class Component extends React.Component {
  constructor(props) {
    super(props);
    this.inputRef = null;
  }
  
  render() {
    return (
      <>
        <input ref={(el) => { this.inputRef = el; }} />
        <button onClick={() => this.inputRef?.focus()}>Focus</button>
      </>
    );
  }
}
```
---

删除未使用的React导入

模式1:JSX转换后的React导入```jsx
// Before (React 18):
import React from 'react'; // Needed for JSX transform

function Component() {
  return <div>Text</div>;
}

// After (React 19 with new JSX transform):
// Remove the React import if it's not used:
function Component() {
  return <div>Text</div>;
}

// BUT keep it if you use React.* APIs:
import React from 'react';

function Component() {
  return <div>{React.useState ? 'yes' : 'no'}</div>;
}
```
###扫描未使用的React导入```bash
# Find imports that can be removed:
grep -rn "^import React from 'react';" src/ --include="*.js" --include="*.jsx"
# Then check if the file uses React.*, useContext, etc.
```
---

完成迁移检查表```bash
# 1. Find all ReactDOM.render calls:
grep -rn "ReactDOM.render" src/ --include="*.js" --include="*.jsx"
# Should be converted to createRoot

# 2. Find all ReactDOM.hydrate calls:
grep -rn "ReactDOM.hydrate" src/ --include="*.js" --include="*.jsx"
# Should be converted to hydrateRoot

# 3. Find all forwardRef usages:
grep -rn "forwardRef" src/ --include="*.js" --include="*.jsx"
# Check each one to see if it can be removed (most can)

# 4. Find all .defaultProps assignments:
grep -rn "\.defaultProps\s*=" src/ --include="*.js" --include="*.jsx"
# Replace with ES6 default params

# 5. Find all useRef() without initial value:
grep -rn "useRef()" src/ --include="*.js" --include="*.jsx"
# Add null: useRef(null)

# 6. Find old context (contextTypes):
grep -rn "contextTypes\|childContextTypes\|getChildContext" src/ --include="*.js" --include="*.jsx"
# Migrate to createContext

# 7. Find string refs (ref="name"):
grep -rn 'ref="' src/ --include="*.js" --include="*.jsx"
# Migrate to createRef or callback ref

# 8. Find unused React imports:
grep -rn "^import React from 'react';" src/ --include="*.js" --include="*.jsx"
# Check if React is used in the file
```
