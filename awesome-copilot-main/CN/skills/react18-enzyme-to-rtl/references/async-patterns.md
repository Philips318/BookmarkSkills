#异步测试模式-酶→RTL迁移

使用React 18兼容模式将酶异步测试重写到React测试库的参考。

##核心问题

酶的异步测试通常使用以下方法之一：

—状态改变后的`wrapper.update()`-`setTimeout`/`Promise.resolve()`刷新微任务
-`setImmediate`刷新异步队列
-直接调用实例方法，后跟`wrapper.update()`这些在劳教中都不起作用。RTL提供了`waitFor`、`findBy*`和`act`。

---

模式1 -状态改变后的wrapper.update(

Enzyme需要`wrapper.update()`来强制在异步状态改变后重新呈现。```jsx
// Enzyme:
it('loads data', async () => {
  const wrapper = mount(<UserList />);
  await Promise.resolve(); // flush microtasks
  wrapper.update();        // force Enzyme to sync with DOM
  expect(wrapper.find('li')).toHaveLength(3);
});
```

```jsx
// RTL - waitFor handles re-renders automatically:
import { render, screen, waitFor } from '@testing-library/react';

it('loads data', async () => {
  render(<UserList />);
  await waitFor(() => {
    expect(screen.getAllByRole('listitem')).toHaveLength(3);
  });
});
```
---

模式2 -由用户交互触发的异步操作```jsx
// Enzyme:
it('fetches user on button click', async () => {
  const wrapper = mount(<UserCard />);
  wrapper.find('button').simulate('click');
  await new Promise(resolve => setTimeout(resolve, 0));
  wrapper.update();
  expect(wrapper.find('.user-name').text()).toBe('John Doe');
});
```

```jsx
// RTL:
import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';

it('fetches user on button click', async () => {
  render(<UserCard />);
  await userEvent.setup().click(screen.getByRole('button', { name: /load/i }));
  // findBy* auto-waits up to 1000ms (configurable)
  expect(await screen.findByText('John Doe')).toBeInTheDocument();
});
```
---

模式3 -加载状态断言```jsx
// Enzyme - asserted loading state synchronously then final state after flush:
it('shows loading then result', async () => {
  const wrapper = mount(<SearchResults query="react" />);
  expect(wrapper.find('.spinner').exists()).toBe(true);
  await new Promise(resolve => setTimeout(resolve, 100));
  wrapper.update();
  expect(wrapper.find('.spinner').exists()).toBe(false);
  expect(wrapper.find('.result')).toHaveLength(5);
});
```

```jsx
// RTL:
it('shows loading then result', async () => {
  render(<SearchResults query="react" />);
  // Loading state - check it appears
  expect(screen.getByRole('progressbar')).toBeInTheDocument();
  // Or if loading is text:
  expect(screen.getByText(/loading/i)).toBeInTheDocument();

  // Wait for results to appear (loading disappears, results show)
  await waitFor(() => {
    expect(screen.queryByRole('progressbar')).not.toBeInTheDocument();
  });
  expect(screen.getAllByRole('listitem')).toHaveLength(5);
});
```
---

模式4 - Apollo MockedProvider异步测试```jsx
// Enzyme with Apollo - used to flush with multiple ticks:
it('renders user from query', async () => {
  const wrapper = mount(
    <MockedProvider mocks={mocks} addTypename={false}>
      <UserProfile id="1" />
    </MockedProvider>
  );
  await new Promise(resolve => setTimeout(resolve, 0)); // flush Apollo queue
  wrapper.update();
  expect(wrapper.find('.username').text()).toBe('Alice');
});
```

```jsx
// RTL with Apollo:
import { render, screen, waitFor } from '@testing-library/react';
import { MockedProvider } from '@apollo/client/testing';

it('renders user from query', async () => {
  render(
    <MockedProvider mocks={mocks} addTypename={false}>
      <UserProfile id="1" />
    </MockedProvider>
  );

  // Wait for Apollo to resolve the query
  expect(await screen.findByText('Alice')).toBeInTheDocument();
  // OR:
  await waitFor(() => {
    expect(screen.getByText('Alice')).toBeInTheDocument();
  });
});
```
** RTL中的阿波罗加载状态：**```jsx
it('shows loading then data', async () => {
  render(
    <MockedProvider mocks={mocks} addTypename={false}>
      <UserProfile id="1" />
    </MockedProvider>
  );
  // Apollo loading state - check immediately after render
  expect(screen.getByText(/loading/i)).toBeInTheDocument();
  // Then wait for data
  expect(await screen.findByText('Alice')).toBeInTheDocument();
});
```
---

模式5 -异步操作的错误状态```jsx
// Enzyme:
it('shows error on failed fetch', async () => {
  server.use(rest.get('/api/user', (req, res, ctx) => res(ctx.status(500))));
  const wrapper = mount(<UserCard />);
  wrapper.find('button').simulate('click');
  await new Promise(resolve => setTimeout(resolve, 0));
  wrapper.update();
  expect(wrapper.find('.error-message').text()).toContain('Something went wrong');
});
```

```jsx
// RTL:
it('shows error on failed fetch', async () => {
  // (assuming MSW or jest.mock for fetch)
  render(<UserCard />);
  await userEvent.setup().click(screen.getByRole('button', { name: /load/i }));
  expect(await screen.findByText(/something went wrong/i)).toBeInTheDocument();
});
```
---

模式6 - act（）用于手动异步控制

当你需要显式控制异步计时时（很少使用RTL，但偶尔需要类组件测试）：```jsx
// RTL with act() for fine-grained async control:
import { act } from 'react';

it('handles sequential state updates', async () => {
  render(<MultiStepForm />);

  await act(async () => {
    fireEvent.click(screen.getByRole('button', { name: /next/i }));
    await Promise.resolve(); // flush microtask queue
  });

  expect(screen.getByText('Step 2')).toBeInTheDocument();
});
```
---

RTL异步查询指南

|方法|行为| |时使用|---|---|---|
|`getBy*`|同步-如果找不到则抛出|元素总是立即存在|
|`queryBy*`|同步-如果找不到则返回null |检查元素不存在|
|`findBy*`|异步-等待长达1000ms，如果没有找到则拒绝|元素异步出现|
|`getAllBy*`|同步-如果0找到|抛出多个元素总是存在|
|`queryAllBy*`|同步-如果没有找到|检查计数或不存在|返回[]
|`findAllBy*`|异步-等待元素出现|多个元素异步出现|
|`waitFor(fn)`|重试fn，直到没有错误或超时|需要轮询|的自定义断言
|`waitForElementToBeRemoved(el)`|等待直到元素消失|加载状态，移除|

**默认超时：** 1000ms。在`jest.config.js`中全局配置：```js
// Increase timeout for slow CI environments
// jest.config.js
module.exports = {
  testEnvironmentOptions: {
    asyncUtilTimeout: 3000,
  },
};
```
---

常见的迁移错误```jsx
// WRONG - mixing async query with sync assertion:
const el = await screen.findByText('Result');
// el is already resolved here - findBy returns the element, not a promise
expect(await el).toBeInTheDocument(); // unnecessary second await

// CORRECT:
const el = await screen.findByText('Result');
expect(el).toBeInTheDocument();
// OR simply:
expect(await screen.findByText('Result')).toBeInTheDocument();
```

```jsx
// WRONG - using getBy* for elements that appear asynchronously:
fireEvent.click(button);
expect(screen.getByText('Loaded!')).toBeInTheDocument(); // throws before data loads

// CORRECT:
fireEvent.click(button);
expect(await screen.findByText('Loaded!')).toBeInTheDocument(); // waits
```
