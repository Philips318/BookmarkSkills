#阿波罗客户端- React 18兼容性细节

为什么阿波罗3.8+是必需的

Apollo Client 3.7及以下版本使用的内部订阅模型与React 18的并发呈现不兼容。在并发模式下，React可以中断和重放渲染，这会导致Apollo的存储订阅在不正确的时间触发——产生过时的数据或错过的更新。

Apollo 3.8是第一个采用`useSyncExternalStore`的版本，React 18需要`useSyncExternalStore`才能使外部存储在并发渲染下正常工作。

##版本概述

|阿波罗版本|支持| React 19支持| Notes ||---|---|---|---|
| < 3.7 |❌|❌|并发模式数据撕裂|
| 3.7。x |⚠️|⚠️|仅适用于遗留根(ReactDOM。渲染)|
| * * 3.8。x** |✅|✅|第一个完全兼容的版本|
| 3.9+ |✅|✅|推荐|
| 3.11+ |✅|✅（已确认）|显式React 19测试添加|

如果你在阿波罗3.7上使用Legacy Root

如果应用程序仍然使用`ReactDOM.render`（旧根），并且还没有迁移到`createRoot`，阿波罗3.7在技术上可以工作-但这意味着你不会获得任何React 18并发特性（包括自动批处理）。这只是部分升级。

一旦使用`createRoot`，将Apollo升级到3.8+。

测试中的MockedProvider——React 18

阿波罗的`MockedProvider`与React 18兼容，但异步行为改变了：```jsx
// Old pattern - flushing with setTimeout:
await new Promise(resolve => setTimeout(resolve, 0));
wrapper.update();

// React 18 pattern - use waitFor or findBy:
await waitFor(() => {
  expect(screen.getByText('Alice')).toBeInTheDocument();
});
// OR:
expect(await screen.findByText('Alice')).toBeInTheDocument();
```
##升级阿波罗```bash
npm install @apollo/client@latest graphql@latest
```
如果graphql peer deep与其他包冲突：```bash
npm ls graphql  # check what version is being used
npm info @apollo/client peerDependencies  # check what apollo requires
```
Apollo 3.8+同时支持`graphql@15`和`graphql@16`。

## InMemoryCache -不需要更改`InMemoryCache`配置不受React 18升级的影响。以下情况无需迁移：

——`typePolicies`——`fragmentMatcher`——`possibleTypes`-自定义字段策略

## useQuery / useMutation / usesubption -没有变化

Apollo钩子的API没有改变。这次升级完全是在Apollo如何与React的渲染模型集成的内部。