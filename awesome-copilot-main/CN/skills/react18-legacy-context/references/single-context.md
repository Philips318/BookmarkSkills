#单上下文迁移-完成Before/After完整示例：ThemeContext

这涵盖了最常见的模式——一个上下文包含一个提供者和多个消费者。

---

###第一步-之前状态（遗产）

* *ThemeProvider.js(供应商):* *```jsx
import PropTypes from 'prop-types';

class ThemeProvider extends React.Component {
  static childContextTypes = {
    theme: PropTypes.string,
    toggleTheme: PropTypes.func,
  };

  state = { theme: 'light' };

  toggleTheme = () => {
    this.setState(s => ({ theme: s.theme === 'light' ? 'dark' : 'light' }));
  };

  getChildContext() {
    return {
      theme: this.state.theme,
      toggleTheme: this.toggleTheme,
    };
  }

  render() {
    return this.props.children;
  }
}
```
**ThemedButton.js（类消费者）：**```jsx
import PropTypes from 'prop-types';

class ThemedButton extends React.Component {
  static contextTypes = {
    theme: PropTypes.string,
    toggleTheme: PropTypes.func,
  };

  render() {
    const { theme, toggleTheme } = this.context;
    return (
      <button className={`btn btn-${theme}`} onClick={toggleTheme}>
        Toggle Theme
      </button>
    );
  }
}
```
**ThemedHeader.js（函数消费者-如果有的话）：**```jsx
// Function components couldn't use legacy context cleanly
// They had to use a class wrapper or render prop
```
---

###步骤2 -创建上下文文件

**src/contexts/ThemeContext.js（新文件）：**```jsx
import React from 'react';

// Default value matches the shape of getChildContext() return
export const ThemeContext = React.createContext({
  theme: 'light',
  toggleTheme: () => {},
});

// Named export for the context - both provider and consumers import from here
```
---

###步骤3 -更新提供程序

* *ThemeProvider.js(后):* *```jsx
import React from 'react';
import { ThemeContext } from '../contexts/ThemeContext';

class ThemeProvider extends React.Component {
  state = { theme: 'light' };

  toggleTheme = () => {
    this.setState(s => ({ theme: s.theme === 'light' ? 'dark' : 'light' }));
  };

  render() {
    // React 19 JSX shorthand: <ThemeContext value={...}>
    // React 18: <ThemeContext.Provider value={...}>
    return (
      <ThemeContext.Provider
        value={{
          theme: this.state.theme,
          toggleTheme: this.toggleTheme,
        }}
      >
        {this.props.children}
      </ThemeContext.Provider>
    );
  }
}

export default ThemeProvider;
```
**在React 19中，你可以直接写`<ThemeContext value={...}>`（不是`.Provider`）。对于React 18.3.1使用`<ThemeContext.Provider value={...}>`。

---

###步骤4 -更新类消费者

* *ThemedButton.js(后):* *```jsx
import React from 'react';
import { ThemeContext } from '../contexts/ThemeContext';

class ThemedButton extends React.Component {
  // singular contextType (not contextTypes)
  static contextType = ThemeContext;

  render() {
    const { theme, toggleTheme } = this.context;
    return (
      <button className={`btn btn-${theme}`} onClick={toggleTheme}>
        Toggle Theme
      </button>
    );
  }
}

export default ThemedButton;
```
**与传统的主要区别：**

-`static contextType`（单数）而不是`contextTypes`（复数）
-不需要PropTypes声明
-`this.context`是一个完整的值对象（不是一个部分-无论你传递给`value`）
-通过`contextType`每个类组件只有一个上下文-使用`Context.Consumer`多个渲染道具

---

###步骤5 -更新功能消费者

**ThemedHeader.js（之后-现在直接挂钩）：**```jsx
import { useContext } from 'react';
import { ThemeContext } from '../contexts/ThemeContext';

function ThemedHeader({ title }) {
  const { theme } = useContext(ThemeContext);
  return <h1 className={`header-${theme}`}>{title}</h1>;
}
```
---

###步骤6 -在一个类组件中使用多个上下文

如果一个类组件使用了多个遗留上下文，它就会变得复杂。类组件只能有一个`static contextType`。对于多个上下文，使用render prop表单：```jsx
import { ThemeContext } from '../contexts/ThemeContext';
import { AuthContext } from '../contexts/AuthContext';

class Dashboard extends React.Component {
  render() {
    return (
      <ThemeContext.Consumer>
        {({ theme }) => (
          <AuthContext.Consumer>
            {({ user }) => (
              <div className={`dashboard-${theme}`}>
                Welcome, {user.name}
              </div>
            )}
          </AuthContext.Consumer>
        )}
      </ThemeContext.Consumer>
    );
  }
}
```
或者考虑将类组件迁移到函数组件，以便干净地使用`useContext`。

---

验证清单

迁移一个上下文后：```bash
# Provider - no legacy context exports remain
grep -n "childContextTypes\|getChildContext" src/ThemeProvider.js

# Consumers - no legacy context consumption remains
grep -rn "contextTypes\s*=" src/ --include="*.js" --include="*.jsx" | grep -v "ThemeContext\|\.test\."

# this.context usage - confirm it reads from contextType not legacy
grep -rn "this\.context\." src/ --include="*.js" | grep -v "\.test\."
```
对于迁移的上下文，每个都应该返回零命中值。