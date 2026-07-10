批处理类别-Before/After模式

## A类-这个。state Read After Await（静默Bug） {#category-a}

该方法在`await`之后读取`this.state`，以做出有条件的决策。在React 18中，中间setState还没有刷新——`this.state`仍然保持更新前的值。

**以前版本（在React 18中损坏）：**```jsx
async handleLoadClick() {
  this.setState({ loading: true });       // batched - not flushed yet
  const data = await fetchData();
  if (this.state.loading) {               // ← still FALSE (old value)
    this.setState({ data, loading: false });  // ← never called
  }
}
```
**后-删除这个。状态完全读取：**```jsx
async handleLoadClick() {
  this.setState({ loading: true });
  try {
    const data = await fetchData();
    this.setState({ data, loading: false }); // always called - no condition needed
  } catch (err) {
    this.setState({ error: err, loading: false });
  }
}
```
模式：**如果`this.state`上的条件在该点总是为真（您只需将其设置为真），则删除该条件。在`await`之前调用的setState最终将刷新—您不需要检查它。

---

A类变体-多步骤条件链```jsx
// Before (broken):
async initialize() {
  this.setState({ step: 'auth' });
  const token = await authenticate();
  if (this.state.step === 'auth') {        // ← wrong: still initial value
    this.setState({ step: 'loading', token });
    const data = await loadData(token);
    if (this.state.step === 'loading') {   // ← wrong again
      this.setState({ step: 'ready', data });
    }
  }
}
```

```jsx
// After - use local variables, not this.state, to track flow:
async initialize() {
  this.setState({ step: 'auth' });
  try {
    const token = await authenticate();
    this.setState({ step: 'loading', token });
    const data = await loadData(token);
    this.setState({ step: 'ready', data });
  } catch (err) {
    this.setState({ step: 'error', error: err });
  }
}
```
---

##类别B -独立的setState调用（重构，无flushSync） {# Category - B}

在Promise链中多次调用setState，其中顺序很重要，但不发生中间状态读取。这些电话只需要重新安排一下。

* *: * *```jsx
handleSubmit() {
  this.setState({ submitting: true });
  submitForm(this.state.formData)
    .then(result => {
      this.setState({ result });
      this.setState({ submitting: false });  // two setState in .then()
    });
}
```
**合并后的setState调用：**```jsx
async handleSubmit() {
  this.setState({ submitting: true, result: null, error: null });
  try {
    const result = await submitForm(this.state.formData);
    this.setState({ result, submitting: false });
  } catch (err) {
    this.setState({ error: err, submitting: false });
  }
}
```
规则：在同一个异步上下文中多个`setState`调用已经在React 18中批处理了。合并为更少的调用更简洁，但不是严格要求的。

---

##类别C -中间渲染必须是可见的（flushSync） {# Category - C}

在异步操作开始之前，用户必须看到一个中间UI状态（加载旋转器，进度步骤）。这是`flushSync`是正确答案的唯一情况。

**诊断问题：**“如果加载旋转器直到取回后才出现，用户体验是否有问题？”

—是→`flushSync`- NO→重构（A类或B类）

* *: * *```jsx
async processOrder() {
  this.setState({ status: 'validating' });   // user must see this
  await validateOrder(this.props.order);
  this.setState({ status: 'charging' });     // user must see this
  await chargeCard(this.props.card);
  this.setState({ status: 'complete' });
}
```
**后- flushSync为每个需要的中间渲染：**```jsx
import { flushSync } from 'react-dom';

async processOrder() {
  flushSync(() => {
    this.setState({ status: 'validating' });  // renders immediately
  });
  await validateOrder(this.props.order);

  flushSync(() => {
    this.setState({ status: 'charging' });    // renders immediately
  });
  await chargeCard(this.props.card);

  this.setState({ status: 'complete' });      // last - no flushSync needed
}
```
**简单加载转轮箱**（最常见）：```jsx
import { flushSync } from 'react-dom';

async handleSearch() {
  // User must see spinner before the fetch begins
  flushSync(() => this.setState({ loading: true }));
  const results = await searchAPI(this.state.query);
  this.setState({ results, loading: false });
}
```
---

## setTimeout模式```jsx
// Before (React 17 - setTimeout fired immediate re-renders):
handleAutoSave() {
  setTimeout(() => {
    this.setState({ saving: true });
    // React 17: re-render happened here
    saveToServer(this.state.formData).then(() => {
      this.setState({ saving: false, lastSaved: Date.now() });
    });
  }, 2000);
}
```

```jsx
// After (React 18 - all setState inside setTimeout batches):
handleAutoSave() {
  setTimeout(async () => {
    // If loading state must show before fetch - flushSync
    flushSync(() => this.setState({ saving: true }));
    await saveToServer(this.state.formData);
    this.setState({ saving: false, lastSaved: Date.now() });
  }, 2000);
}
```
---

测试由于批处理而中断的模式```jsx
// Before (React 17 - intermediate state was synchronously visible):
it('shows saving indicator', () => {
  render(<AutoSaveForm />);
  fireEvent.change(input, { target: { value: 'new text' } });
  expect(screen.getByText('Saving...')).toBeInTheDocument(); // ← sync check
});

// After (React 18 - use waitFor for intermediate states):
it('shows saving indicator', async () => {
  render(<AutoSaveForm />);
  fireEvent.change(input, { target: { value: 'new text' } });
  await waitFor(() => expect(screen.getByText('Saving...')).toBeInTheDocument());
  await waitFor(() => expect(screen.getByText('Saved')).toBeInTheDocument());
});
```
