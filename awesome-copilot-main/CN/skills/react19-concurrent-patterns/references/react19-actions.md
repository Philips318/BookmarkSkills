---
title: React 19 Actions Pattern Reference
---
# React 19动作模式参考

React 19引入了一种处理异步操作（如表单提交）的模式**Actions**，它具有内置的加载状态、错误处理和乐观更新。这将用一个更简单的API替换`useReducer + state`模式。

什么是动作？

**Action**是一个async函数，它：

-可以在表单提交或按钮点击时自动调用
—以自动loading/pending状态运行
-完成后自动更新UI
与服务器组件一起工作，直接进行服务器变更

---

# # useActionState ()`useActionState`是客户端Action钩子。它取代`useReducer + useEffect`进行表单处理。

### React 18模式```jsx
// React 18  form with useReducer + state:
function Form() {
  const [state, dispatch] = useReducer(
    (state, action) => {
      switch (action.type) {
        case 'loading':
          return { ...state, loading: true, error: null };
        case 'success':
          return { ...state, loading: false, data: action.data };
        case 'error':
          return { ...state, loading: false, error: action.error };
      }
    },
    { loading: false, data: null, error: null }
  );
  
  async function handleSubmit(e) {
    e.preventDefault();
    dispatch({ type: 'loading' });
    try {
      const result = await submitForm(new FormData(e.target));
      dispatch({ type: 'success', data: result });
    } catch (err) {
      dispatch({ type: 'error', error: err.message });
    }
  }
  
  return (
    <form onSubmit={handleSubmit}>
      <input name="email" />
      {state.loading && <Spinner />}
      {state.error && <Error msg={state.error} />}
      {state.data && <Success data={state.data} />}
      <button disabled={state.loading}>Submit</button>
    </form>
  );
}
```
### React 19 useActionState（）模式```jsx
// React 19  same form with useActionState:
import { useActionState } from 'react';

async function submitFormAction(prevState, formData) {
  // prevState = previous return value from this function
  // formData = FormData from <form action={submitFormAction}>
  
  try {
    const result = await submitForm(formData);
    return { data: result, error: null };
  } catch (err) {
    return { data: null, error: err.message };
  }
}

function Form() {
  const [state, formAction, isPending] = useActionState(
    submitFormAction,
    { data: null, error: null } // initial state
  );
  
  return (
    <form action={formAction}>
      <input name="email" />
      {isPending && <Spinner />}
      {state.error && <Error msg={state.error} />}
      {state.data && <Success data={state.data} />}
      <button disabled={isPending}>Submit</button>
    </form>
  );
}
```
* *差异:* *

-一个钩子代替`useReducer`+逻辑
-`formAction`取代`onSubmit`， form自动收集FormData
-`isPending`是一个布尔值，没有调度调用
—动作函数接收`(prevState, formData)`---

# # useFormStatus ()`useFormStatus`是一个子组件钩子，它从最近的窗体中读取挂起状态。它就像一个内置的`isPending`信号，没有prop钻井。```jsx
// React 18  must pass isPending as prop:
function SubmitButton({ isPending }) {
  return <button disabled={isPending}>Submit</button>;
}

function Form({ isPending, formAction }) {
  return (
    <form action={formAction}>
      <input />
      <SubmitButton isPending={isPending} />
    </form>
  );
}

// React 19  useFormStatus reads it automatically:
function SubmitButton() {
  const { pending } = useFormStatus();
  return <button disabled={pending}>Submit</button>;
}

function Form() {
  const [state, formAction] = useActionState(submitFormAction, {});
  
  return (
    <form action={formAction}>
      <input />
      <SubmitButton /> {/* No prop needed */}
    </form>
  );
}
```
**重点：**`useFormStatus`只在`<form action={...}>`内部工作，常规`<form onSubmit>`不会触发它。

---

# # useOptimistic ()`useOptimistic`在执行异步操作时立即更新UI。操作成功后，确认的数据将替换乐观值。如果失败，UI将恢复。

### React 18模式```jsx
// React 18  manual optimistic update:
function TodoList({ todos, onAddTodo }) {
  const [optimistic, setOptimistic] = useState(todos);
  
  async function handleAddTodo(text) {
    const newTodo = { id: Date.now(), text, completed: false };
    
    // Show optimistic update immediately
    setOptimistic([...optimistic, newTodo]);
    
    try {
      const result = await addTodo(text);
      // Update with confirmed result
      setOptimistic(prev => [
        ...prev.filter(t => t.id !== newTodo.id),
        result
      ]);
    } catch (err) {
      // Revert on error
      setOptimistic(optimistic);
    }
  }
  
  return (
    <ul>
      {optimistic.map(todo => (
        <li key={todo.id}>{todo.text}</li>
      ))}
    </ul>
  );
}
```
### React 19 useOptimistic（）模式```jsx
import { useOptimistic } from 'react';

async function addTodoAction(prevTodos, formData) {
  const text = formData.get('text');
  const result = await addTodo(text);
  return [...prevTodos, result];
}

function TodoList({ todos }) {
  const [optimistic, addOptimistic] = useOptimistic(
    todos,
    (state, newTodo) => [...state, newTodo]
  );
  
  const [, formAction] = useActionState(addTodoAction, todos);
  
  async function handleAddTodo(formData) {
    const text = formData.get('text');
    // Optimistic update:
    addOptimistic({ id: Date.now(), text, completed: false });
    // Then call the form action:
    formAction(formData);
  }
  
  return (
    <>
      <ul>
        {optimistic.map(todo => (
          <li key={todo.id}>{todo.text}</li>
        ))}
      </ul>
      <form action={handleAddTodo}>
        <input name="text" />
        <button>Add</button>
      </form>
    </>
  );
}
```
* *重点:* *

——`useOptimistic(currentState, updateFunction)`-`updateFunction`接收`(state, optimisticInput)`并返回新状态
—调用`addOptimistic(input)`触发乐观更新
-服务器操作的返回值在完成时替换乐观状态

---

完整示例：带有所有钩子的待办事项列表```jsx
import { useActionState, useFormStatus, useOptimistic } from 'react';

// Server action:
async function addTodoAction(prevTodos, formData) {
  const text = formData.get('text');
  if (!text) throw new Error('Text required');
  const newTodo = await api.post('/todos', { text });
  return [...prevTodos, newTodo];
}

// Submit button with useFormStatus:
function AddButton() {
  const { pending } = useFormStatus();
  return <button disabled={pending}>{pending ? 'Adding...' : 'Add Todo'}</button>;
}

// Main component:
function TodoApp({ initialTodos }) {
  const [optimistic, addOptimistic] = useOptimistic(
    initialTodos,
    (state, newTodo) => [...state, newTodo]
  );
  
  const [todos, formAction] = useActionState(
    addTodoAction,
    initialTodos
  );
  
  async function handleAddTodo(formData) {
    const text = formData.get('text');
    // Optimistic: show it immediately
    addOptimistic({ id: Date.now(), text });
    // Then submit the form (which updates when server confirms)
    await formAction(formData);
  }
  
  return (
    <>
      <ul>
        {optimistic.map(todo => (
          <li key={todo.id}>{todo.text}</li>
        ))}
      </ul>
      <form action={handleAddTodo}>
        <input name="text" placeholder="Add a todo..." required />
        <AddButton />
      </form>
    </>
  );
}
```
---

##迁移策略

阶段1不需要更改

行动是可选择的。所有现有的`useReducer + onSubmit`模式继续工作。没有强制迁移。

阶段2确定重构候选

在React 19迁移稳定之后，`useReducer + async`模式的配置文件：```bash
grep -rn "useReducer.*case.*'loading\|useReducer.*case.*'success" src/ --include="*.js" --include="*.jsx"
```
值得重构的模式：

-表单提交与loading/error状态
—用户事件触发异步操作
—当前代码使用`dispatch({ type: '...' })`-简单状态形状（对象与`loading`，`error`,`data`）

阶段3重构为useActionState```jsx
// Before:
function LoginForm() {
  const [state, dispatch] = useReducer(loginReducer, { loading: false, error: null, user: null });
  
  async function handleSubmit(e) {
    e.preventDefault();
    dispatch({ type: 'loading' });
    try {
      const user = await login(e.target);
      dispatch({ type: 'success', data: user });
    } catch (err) {
      dispatch({ type: 'error', error: err.message });
    }
  }
  
  return <form onSubmit={handleSubmit}>...</form>;
}

// After:
async function loginAction(prevState, formData) {
  try {
    const user = await login(formData);
    return { user, error: null };
  } catch (err) {
    return { user: null, error: err.message };
  }
}

function LoginForm() {
  const [state, formAction] = useActionState(loginAction, { user: null, error: null });
  
  return <form action={formAction}>...</form>;
}
```
---

##对照表

|特性| React 18 | React 19 ||---|---|---|
|表单处理|`onSubmit`+ useReducer |`action`+ useActionState |
|加载状态|手动调度|自动调度`isPending`|
| Prop drilling |`useFormStatus`hook |
|乐观更新|手动状态舞蹈|`useOptimistic`钩子|
|错误处理|调度手动|操作|返回
|复杂性|更多样板|更少样板|