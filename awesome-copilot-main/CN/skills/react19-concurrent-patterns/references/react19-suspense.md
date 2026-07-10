---
title: React 19 Suspense for Data Fetching Pattern Reference
---
# React 19数据获取模式引用的悬念

React 19新的数据获取的悬念集成是一个预览功能，允许组件挂起（暂停渲染）直到数据可用，不需要`useEffect + state`。

**重要：**这是一个**预览，它需要特定的设置，并且在生产环境中还不稳定，但是你应该知道React 19迁移计划的模式。

---

React 19有什么变化？

React 18悬念只支持**代码分割**（惰性组件）。如果满足某些条件，React 19将其扩展到**数据获取**：

**数据获取库必须实现悬念（例如，React Query 5+， SWR, Remix loaders）
- **或者你自己的承诺跟踪**以一种React可以跟踪其暂停的方式包装承诺
- **没有更多的“悬念之后没有钩子”**你可以直接在组件中使用悬念`use()`---## React 18悬念（仅限代码分裂）```jsx
// React 18  Suspense for lazy imports only:
const LazyComponent = React.lazy(() => import('./Component'));

function App() {
  return (
    <Suspense fallback={<Spinner />}>
      <LazyComponent />
    </Suspense>
  );
}
```
尝试在React 18中暂停数据需要hack或库：```jsx
// React 18 hack  not recommended:
const dataPromise = fetchData();
const resource = {
  read: () => {
    throw dataPromise; // Throw to suspend
  }
};

function Component() {
  const data = resource.read(); // Throws promise → Suspense catches it
  return <div>{data}</div>;
}
```
---

## React 19数据获取的悬念（预览）

React 19通过`use()`钩子为带有承诺的悬疑提供了一流的支持：```jsx
// React 19  Suspense for data fetching:
function UserProfile({ userId }) {
  const user = use(fetchUser(userId)); // Suspends if promise pending
  return <div>{user.name}</div>;
}

function App() {
  return (
    <Suspense fallback={<Spinner />}>
      <UserProfile userId={123} />
    </Suspense>
  );
}
```
**与React 18的主要区别

-`use()`打开承诺，组件自动挂起
-不需要`useEffect + state`技巧
更简洁的代码，更少的样板文件

---

模式1：简单的承诺悬念```jsx
// Raw promise (not recommended in production):
function DataComponent() {
  const data = use(fetch('/api/data').then(r => r.json()));
  return <pre>{JSON.stringify(data, null, 2)}</pre>;
}

function App() {
  return (
    <Suspense fallback={<Spinner />}>
      <DataComponent />
    </Suspense>
  );
}
```
**问题：**承诺是重新创建每次渲染。解决方案：用`useMemo`封装。

---

模式2：熟记承诺（更好）```jsx
function DataComponent({ id }) {
  // Only create promise once per id:
  const dataPromise = useMemo(() => 
    fetch(`/api/data/${id}`).then(r => r.json()),
    [id]
  );
  
  const data = use(dataPromise);
  return <pre>{JSON.stringify(data, null, 2)}</pre>;
}

function App() {
  const [id, setId] = useState(1);
  
  return (
    <Suspense fallback={<Spinner />}>
      <DataComponent id={id} />
      <button onClick={() => setId(id + 1)}>Next</button>
    </Suspense>
  );
}
```
---

模式3：库集成（React Query）

现代数据库直接支持悬念。React查询5+示例：```jsx
// React Query 5+ with Suspense:
import { useSuspenseQuery } from '@tanstack/react-query';

function UserProfile({ userId }) {
  // useSuspenseQuery throws promise if suspended
  const { data: user } = useSuspenseQuery({
    queryKey: ['user', userId],
    queryFn: () => fetchUser(userId),
  });
  
  return <div>{user.name}</div>;
}

function App() {
  return (
    <Suspense fallback={<Spinner />}>
      <UserProfile userId={123} />
    </Suspense>
  );
}
```
**优点：**库处理缓存，重试和缓存无效。

---

模式4：错误边界集成

结合悬疑和错误边界来处理加载和错误：```jsx
function UserProfile({ userId }) {
  const user = use(fetchUser(userId)); // Suspends while loading
  return <div>{user.name}</div>;
}

function App() {
  return (
    <ErrorBoundary fallback={<ErrorScreen />}>
      <Suspense fallback={<Spinner />}>
        <UserProfile userId={123} />
      </Suspense>
    </ErrorBoundary>
  );
}

class ErrorBoundary extends React.Component {
  state = { error: null };
  
  static getDerivedStateFromError(error) {
    return { error };
  }
  
  render() {
    if (this.state.error) return this.props.fallback;
    return this.props.children;
  }
}
```
---

嵌套悬念边界

在等待不同数据时，使用多个悬念边界来显示部分UI：```jsx
function App({ userId }) {
  return (
    <div>
      <Suspense fallback={<UserSpinner />}>
        <UserProfile userId={userId} />
      </Suspense>
      
      <Suspense fallback={<PostsSpinner />}>
        <UserPosts userId={userId} />
      </Suspense>
    </div>
  );
}

function UserProfile({ userId }) {
  const user = use(fetchUser(userId));
  return <h1>{user.name}</h1>;
}

function UserPosts({ userId }) {
  const posts = use(fetchUserPosts(userId));
  return <ul>{posts.map(p => <li key={p.id}>{p.title}</li>)}</ul>;
}
```
现在:

-用户配置文件在加载时显示旋转器
- Posts独立显示旋转器
-两者都可以渲染，因为他们完成

---

顺序悬疑vs平行悬疑

###顺序（等待第一个获取第二个）```jsx
function App({ userId }) {
  const user = use(fetchUser(userId)); // Must complete first
  
  return (
    <Suspense fallback={<PostsSpinner />}>
      <UserPosts userId={user.id} /> {/* Depends on user */}
    </Suspense>
  );
}

function UserPosts({ userId }) {
  const posts = use(fetchUserPosts(userId));
  return <ul>{posts.map(p => <li>{p.title}</li>)}</ul>;
}
```
### Parallel（同时读取）```jsx
function App({ userId }) {
  return (
    <div>
      <Suspense fallback={<UserSpinner />}>
        <UserProfile userId={userId} />
      </Suspense>
      
      <Suspense fallback={<PostsSpinner />}>
        <UserPosts userId={userId} /> {/* Fetches in parallel */}
      </Suspense>
    </div>
  );
}
```
---

## React 18→React 19的迁移策略

阶段1不需要更改

对于数据获取来说，悬念仍然是可选的和实验性的。所有现有的`useEffect + state`模式继续工作。

###第二阶段等待稳定

在生产中采用悬念数据提取之前：

-等待React 19发布（不是预览版）
-验证您的数据库支持悬念
-应用在React 19核心稳定后计划迁移

第三阶段重构为悬念（可选，后期预览）

一旦稳定，profile候选人：```bash
grep -rn "useEffect.*fetch\|useEffect.*axios\|useEffect.*graphql" src/ --include="*.js" --include="*.jsx"
```

```jsx
// Before (React 18):
function UserProfile({ userId }) {
  const [user, setUser] = useState(null);
  
  useEffect(() => {
    fetchUser(userId).then(setUser);
  }, [userId]);
  
  if (!user) return <Spinner />;
  return <div>{user.name}</div>;
}

// After (React 19 with Suspense):
function UserProfile({ userId }) {
  const user = use(fetchUser(userId));
  return <div>{user.name}</div>;
}

// Must be wrapped in Suspense:
<Suspense fallback={<Spinner />}>
  <UserProfile userId={123} />
</Suspense>
```
---

##重要警告

1. **暂态为实验性数据，行为可能改变
2. **性能**承诺在每次渲染时重新创建而不需要记忆；使用`useMemo`3. **缓存**`use()`不缓存；在生产应用中使用React Query或类似的方法
4. **SSR**悬念SSR支持有限；检查Next.js版本要求