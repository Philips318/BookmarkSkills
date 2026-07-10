# flushSync指南

# #进口```jsx
import { flushSync } from 'react-dom';
// NOT from 'react' - it lives in react-dom
```
如果文件已经从`react-dom`导入：```jsx
import ReactDOM from 'react-dom';
// Add named import:
import ReactDOM, { flushSync } from 'react-dom';
```
# #语法```jsx
flushSync(() => {
  this.setState({ ... });
});
// After this line, the re-render has completed synchronously
```
多个setState调用在一个flushSync批处理一起到一个同步渲染：```jsx
flushSync(() => {
  this.setState({ step: 'loading' });
  this.setState({ progress: 0 });
  // These batch together → one render
});
```
##何时使用

✅在异步操作开始之前，用户必须看到一个特定的UI状态时使用：```jsx
flushSync(() => this.setState({ loading: true }));
await expensiveAsyncOperation();
```
✅用于多步骤进度流程，其中每个步骤必须在下一个步骤之前视觉上完成：```jsx
flushSync(() => this.setState({ status: 'validating' }));
await validate();
flushSync(() => this.setState({ status: 'processing' }));
await process();
```
✅在必须同步断言中间UI状态的测试中使用（尽可能避免—首选`waitFor`）。

##不要使用

❌不要用它来“修复”读数。state-after-await bug——这是A类bug（应该重构）：```jsx
// WRONG - flushSync doesn't fix this
flushSync(() => this.setState({ loading: true }));
const data = await fetchData();
if (this.state.loading) { ... } // still a race condition
```
❌不要对每个setState都使用它来“安全”——它会挫败React 18的并发渲染：```jsx
// WRONG - excessive flushSync
async handleClick() {
  flushSync(() => this.setState({ clicked: true }));   // unnecessary
  flushSync(() => this.setState({ processing: true })); // unnecessary
  const result = await doWork();
  flushSync(() => this.setState({ result, done: true })); // unnecessary
}
```
❌不要在`useEffect`或`componentDidMount`中使用它来触发立即状态—它会导致嵌套的渲染周期。

##性能说明`flushSync`强制同步呈现，这会阻塞浏览器线程，直到呈现完成。在速度较慢的设备或复杂的组件树上，在异步方法中多次调用`flushSync`将导致可见的延迟。很少使用。

如果您发现自己向单个方法添加了超过2个`flushSync`调用，请重新考虑是否需要重新设计组件的状态模型。