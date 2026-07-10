# Context文件模板

新上下文模块的标准模板。复制并填写姓名。

# #模板```jsx
// src/contexts/[Name]Context.js
import React from 'react';

// ─── 1. Default Value ───────────────────────────────────────────────────────
// Shape must match what the provider will pass as `value`
// Used when a consumer renders outside any provider (edge case protection)
const defaultValue = {
  // fill in the shape
};

// ─── 2. Create Context ──────────────────────────────────────────────────────
export const [Name]Context = React.createContext(defaultValue);

// ─── 3. Display Name (for React DevTools) ───────────────────────────────────
[Name]Context.displayName = '[Name]Context';

// ─── 4. Optional: Custom Hook (strongly recommended) ────────────────────────
// Provides a clean import path and a helpful error if used outside provider
export function use[Name]() {
  const context = React.useContext([Name]Context);
  if (context === defaultValue) {
    // Only throw if defaultValue is a sentinel - skip if a real default makes sense
    // throw new Error('use[Name] must be used inside a [Name]Provider');
  }
  return context;
}
```
##填充示例- AuthContext```jsx
// src/contexts/AuthContext.js
import React from 'react';

const defaultValue = {
  user: null,
  isAuthenticated: false,
  login: () => Promise.resolve(),
  logout: () => {},
};

export const AuthContext = React.createContext(defaultValue);
AuthContext.displayName = 'AuthContext';

export function useAuth() {
  return React.useContext(AuthContext);
}
```
##填充示例- ThemeContext```jsx
// src/contexts/ThemeContext.js
import React from 'react';

const defaultValue = {
  theme: 'light',
  toggleTheme: () => {},
};

export const ThemeContext = React.createContext(defaultValue);
ThemeContext.displayName = 'ThemeContext';

export function useTheme() {
  return React.useContext(ThemeContext);
}
```
##在哪里放置上下文文件```
src/
  contexts/           ← preferred: dedicated folder
    AuthContext.js
    ThemeContext.js
```
其他可接受的地点：```
src/context/          ← singular is also fine
src/store/contexts/   ← if co-located with state management
```
不要把上下文文件放在组件文件夹中——上下文是横切的，不应该被任何一个组件拥有。

##提供商在应用中的位置

上下文提供程序包装了需要访问的组件。放置在树的尽可能低的位置，而不是总是在根部；```jsx
// App.js
import { ThemeProvider } from './ThemeProvider';
import { AuthProvider } from './AuthProvider';

function App() {
  return (
    // Auth wraps everything - login state is needed everywhere
    <AuthProvider>
      {/* Theme wraps only the UI shell - not needed in pure data providers */}
      <ThemeProvider>
        <Router>
          <AppShell />
        </Router>
      </ThemeProvider>
    </AuthProvider>
  );
}
```
