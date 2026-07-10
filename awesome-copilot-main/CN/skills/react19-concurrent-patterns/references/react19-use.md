---
title: React 19 use() Hook Pattern Reference
---
# React 19使用（）钩子模式参考`use()`钩子是React 19在React组件中展开承诺和上下文的答案。它支持直接在组件主体中使用更简洁的异步模式，避免了以前需要单独惰性组件或复杂状态管理的体系结构复杂性。

use（）是什么？`use()`是一个钩子：

- **接受**一个承诺或上下文对象
- **返回**解析值或上下文值
- **处理**悬念自动承诺
**可以在组件内部有条件地调用**（而不是在承诺的顶层）
- **抛出错误**，其中悬疑+错误边界可以捕获

##使用（）与承诺

### React 18模式```jsx
// React 18 approach 1  lazy load a component module:
const UserComponent = React.lazy(() => import('./User'));

function App() {
  return (
    <Suspense fallback={<Spinner />}>
      <UserComponent />
    </Suspense>
  );
}

// React 18 approach 2  fetch data with state + useEffect:
function App({ userId }) {
  const [user, setUser] = useState(null);
  
  useEffect(() => {
    fetchUser(userId).then(setUser);
  }, [userId]);
  
  if (!user) return <Spinner />;
  return <User user={user} />;
}
```
### React 19 use() Pattern```jsx
// React 19  use() directly in component:
function App({ userId }) {
  const user = use(fetchUser(userId)); // Suspends automatically
  return <User user={user} />;
}

// Usage:
function Root() {
  return (
    <Suspense fallback={<Spinner />}>
      <App userId={123} />
    </Suspense>
  );
}
```
* *主要差异:* *

-`use()`直接在组件主体中展开承诺
悬疑边界仍然需要，但是可以放在应用的根目录（而不是每个组件）
-简单的异步数据不需要state或useEffect
-组件内部允许条件包装

## use() with Promises——条件抓取```jsx
// React 18  conditional with state
function SearchResults() {
  const [results, setResults] = useState(null);
  const [query, setQuery] = useState('');
  
  useEffect(() => {
    if (query) {
      search(query).then(setResults);
    } else {
      setResults(null);
    }
  }, [query]);
  
  if (!results) return null;
  return <Results items={results} />;
}

// React 19  use() with conditional
function SearchResults() {
  const [query, setQuery] = useState('');
  
  if (!query) return null;
  
  const results = use(search(query)); // Only fetches if query is truthy
  return <Results items={results} />;
}
```
##使用（）与上下文`use()`可以在组件根目录下展开上下文。不像promise那样常用，但对条件上下文读取很有用：```jsx
// React 18  always in component body, only works at top level
const theme = useContext(ThemeContext);

// React 19  can be conditional
function Button({ useSystemTheme }) {
  const theme = useSystemTheme ? use(ThemeContext) : defaultTheme;
  return <button style={theme}>Click</button>;
}
```
---

##迁移策略

阶段1不需要更改

React 19`use()`是可选的。所有现有的悬疑+组件分裂模式继续工作：```jsx
// Keep this as-is if it's working:
const Lazy = React.lazy(() => import('./Component'));
<Suspense fallback={<Spinner />}><Lazy /></Suspense>
```
阶段2迁移后清理（可选）

在React 19迁移稳定之后，为`useEffect + state`异步模式配置代码库。这些都是`use()`重构的好选择：

识别模式:```bash
grep -rn "useEffect.*\(.*fetch\|async\|promise" src/ --include="*.js" --include="*.jsx"
```
目标:

-简单的抓取挂载模式
—没有复杂的依赖数组
-每个组件一个承诺
-悬念已经在应用程序的其他地方使用

重构示例:```jsx
// Before:
function Post({ postId }) {
  const [post, setPost] = useState(null);
  
  useEffect(() => {
    fetchPost(postId).then(setPost);
  }, [postId]);
  
  if (!post) return <Spinner />;
  return <PostContent post={post} />;
}

​// After:
function Post({ postId }) {
  const post = use(fetchPost(postId));
  return <PostContent post={post} />;
}

// And ensure Suspense at app level:
<Suspense fallback={<AppSpinner />}>
  <Post postId={123} />
</Suspense>
```
---

##错误处理`use()`抛出错误，悬疑错误边界捕获：```jsx
function Root() {
  return (
    <ErrorBoundary fallback={<ErrorScreen />}>
      <Suspense fallback={<Spinner />}>
        <DataComponent />
      </Suspense>
    </ErrorBoundary>
  );
}

function DataComponent() {
  const data = use(fetchData()); // If fetch rejects, error boundary catches it
  return <Data data={data} />;
}
```
---

当不使用use（）时

- **在迁移期间避免**首先稳定React 19
- **复杂的依赖关系**如果多个承诺或复杂的排序逻辑，坚持使用`useEffect`**重试逻辑**`use()`不处理重试；带状态的`useEffect`更清晰
**有瑕疵的更新**`use()`在每次道具更改时重新抓取；带有清理功能的`useEffect`更好