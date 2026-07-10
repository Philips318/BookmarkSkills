#多个遗留上下文-迁移参考

##识别多个上下文

React16/17代码库通常有几个用于不同关注点的遗留上下文：```bash
# Find distinct context names used in childContextTypes
grep -rn "childContextTypes" src/ --include="*.js" --include="*.jsx" | grep -v "\.test\."
# Each hit is a separate context to migrate
```
重类代码库中的常见模式：

- **主题上下文** -dark/light模式，调色板
- **授权上下文** -当前用户，login/logout函数
- **路由器上下文** -当前路由，导航（如果使用旧的react-router）
- Redux存储，调度（如果使用旧的连接模式）
**Locale/i18n上下文** -语言，翻译功能
- **Toast/notification上下文** -show/hide通知

---

##移民顺序

每次迁移一个上下文。每一个都是独立的迁移：```
For each legacy context:
  1. Create src/contexts/[Name]Context.js
  2. Update the provider
  3. Update all consumers
  4. Run the app - verify no warning for this context
  5. Move to the next context
```
不要先迁移所有的提供商，然后再迁移所有的消费者——这会让应用处于一个破碎的中间状态。

---

同一个提供程序中有多个上下文

有些应用在一个provider组件中合并了多个上下文：```jsx
// Before - one provider exports multiple context values:
class AppProvider extends React.Component {
  static childContextTypes = {
    theme: PropTypes.string,
    user: PropTypes.object,
    locale: PropTypes.string,
    notifications: PropTypes.array,
  };

  getChildContext() {
    return {
      theme: this.state.theme,
      user: this.state.user,
      locale: this.state.locale,
      notifications: this.state.notifications,
    };
  }
}
```
**迁移方法-分成不同的上下文：**```jsx
// src/contexts/ThemeContext.js
export const ThemeContext = React.createContext('light');

// src/contexts/AuthContext.js
export const AuthContext = React.createContext({ user: null, login: () => {}, logout: () => {} });

// src/contexts/LocaleContext.js
export const LocaleContext = React.createContext('en');

// src/contexts/NotificationContext.js
export const NotificationContext = React.createContext([]);
```

```jsx
// AppProvider.js - now wraps with multiple providers
import { ThemeContext } from './contexts/ThemeContext';
import { AuthContext } from './contexts/AuthContext';
import { LocaleContext } from './contexts/LocaleContext';
import { NotificationContext } from './contexts/NotificationContext';

class AppProvider extends React.Component {
  render() {
    const { theme, user, locale, notifications } = this.state;
    return (
      <ThemeContext.Provider value={theme}>
        <AuthContext.Provider value={{ user, login: this.login, logout: this.logout }}>
          <LocaleContext.Provider value={locale}>
            <NotificationContext.Provider value={notifications}>
              {this.props.children}
            </NotificationContext.Provider>
          </LocaleContext.Provider>
        </AuthContext.Provider>
      </ThemeContext.Provider>
    );
  }
}
```
---

具有多个上下文的消费者（类组件）

类组件只能使用一个`static contextType`。对于多个，使用`Consumer`渲染道具或转换为函数组件。

选项A -渲染道具（保留为类组件）```jsx
import { ThemeContext } from '../contexts/ThemeContext';
import { AuthContext } from '../contexts/AuthContext';

class UserPanel extends React.Component {
  render() {
    return (
      <ThemeContext.Consumer>
        {(theme) => (
          <AuthContext.Consumer>
            {({ user, logout }) => (
              <div className={`panel panel-${theme}`}>
                <span>{user?.name}</span>
                <button onClick={logout}>Sign out</button>
              </div>
            )}
          </AuthContext.Consumer>
        )}
      </ThemeContext.Consumer>
    );
  }
}
```
选项B -转换为功能组件（首选）```jsx
import { useContext } from 'react';
import { ThemeContext } from '../contexts/ThemeContext';
import { AuthContext } from '../contexts/AuthContext';

function UserPanel() {
  const theme = useContext(ThemeContext);
  const { user, logout } = useContext(AuthContext);

  return (
    <div className={`panel panel-${theme}`}>
      <span>{user?.name}</span>
      <button onClick={logout}>Sign out</button>
    </div>
  );
}
```
如果转换为函数组件超出了这个迁移冲刺的范围，那么使用选项a。如果类组件很简单（主要只是呈现），那么选项B值得稍微重写一下。

---

上下文文件命名约定

在代码库中使用一致的命名：```
src/
  contexts/
    ThemeContext.js      → exports: ThemeContext, ThemeProvider (optional)
    AuthContext.js       → exports: AuthContext, AuthProvider (optional)
    LocaleContext.js     → exports: LocaleContext
```
每个文件导出上下文对象。提供者可以保留其原始文件，只导入上下文。

---

所有上下文迁移后的验证```bash
# Should return zero hits for legacy context patterns
echo "=== childContextTypes ==="
grep -rn "childContextTypes" src/ --include="*.js" --include="*.jsx" | grep -v "\.test\." | wc -l

echo "=== contextTypes (legacy) ==="
grep -rn "^\s*static contextTypes\s*=\|contextTypes\.propTypes" src/ --include="*.js" | grep -v "\.test\." | wc -l

echo "=== getChildContext ==="
grep -rn "getChildContext" src/ --include="*.js" --include="*.jsx" | grep -v "\.test\." | wc -l

echo "All three should be 0"
```
注意：`static contextType`（单数）是MODERN API -这是正确的。只有`contextTypes`（复数）是遗留的。