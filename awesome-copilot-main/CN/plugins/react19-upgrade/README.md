# React 19升级插件

用于将React 18代码库迁移到React 19的企业工具包。包括五个专门的代理和三个技能，针对升级到React 19的现代API表面的具体挑战。

# #安装```bash
copilot plugin install react19-upgrade@awesome-copilot
```
包含的内容

# # #代理

1. 通过审核、依赖关系、源代码迁移和测试验证阶段协调整个迁移管道的主编排器。

2. **react19-auditor**深度扫描专家，识别每个React 19的破坏性更改和弃用模式：
-删除api:`ReactDOM.render`，`ReactDOM.hydrate`,`unmountComponentAtNode`,`findDOMNode`,`createFactory`，`react-dom/test-utils`出口
-遗留上下文API （`contextTypes`,`childContextTypes`,`getChildContext`）
-字符串refs （`this.refs.x`）
-已弃用模式：`forwardRef`，`defaultProps`有功能组件，`useRef()`没有初始值
-特定于测试的问题：`act`导入位置，`Simulate`使用，StrictMode更改

3. **依赖升级专家，升级到react@19，处理@testing-library/react@16+，解决所有对等冲突，并返回GO/NO-GO确认。4. **react19-migrator**源代码迁移引擎，重写所需的React 19更改，并可以为已弃用的模式应用可选的现代化：
-`ReactDOM.render`→`createRoot`-`ReactDOM.hydrate`→`hydrateRoot`-`unmountComponentAtNode`→`root.unmount()`-`findDOMNode`→直接裁判
-可选现代化：`forwardRef`→refas direct prop
-`defaultProps`→ES6默认值
—Legacy Context→`createContext`-字符串refs→`createRef`-`useRef()`→`useRef(null)`-`propTypes`→文档注释

5. **react19-test-guardian**测试套件修复器处理：
-`act`导入修复（react-dom/test-utils→react）
-`Simulate`→`fireEvent`迁移
- StrictMode间谍调用计数增量（不再在React 19中重复调用）
-`useRef`形状更新
-自定义渲染助手验证
-错误边界测试更新
—运行测试，直到失败为零

# # #技能1. React 19并发特性的深层模式，包括悬念、use（）钩子、服务器组件集成和并发批处理。

2. **源API更改的迁移模式，包括DOM/rootAPI、refs和上下文更新。

3. **全面的测试迁移指南，涵盖`act()`语义、错误边界测试和StrictMode行为更改。

##快速入门```
Ask: "Start implementing React 19 migration for my codebase"
```
react19指挥官将指导您：

1. 审核→识别所有破坏性变更
2. Deps→升级到react@19 +兼容库
3. 迁移→修复所有弃用的api和模式
4. 测试→迁移测试套件并运行到绿色

React 18的重大变化

删除了api

—`ReactDOM.render()`使用`createRoot()`—`ReactDOM.hydrate()`使用`hydrateRoot()`-`ReactDOM.unmountComponentAtNode()`使用`root.unmount()`-`ReactDOM.findDOMNode()`使用直接参考
-`React.createFactory()`使用JSX
-`react-dom/test-utils`exports
-遗留上下文API
-字符串参考

已弃用的模式（仍然有效，但应该迁移）

-`forwardRef`ref现在是一个直接道具
-功能组件中的`defaultProps`使用ES6默认值
—不带初始值的`useRef()`传递`null`行为改变

- StrictMode不再重复调用效果（影响测试调用计数断言）
删除`propTypes`运行时验证（保留文档，但没有运行时检查）

##主要特性-✅全面移除8个以上已弃用的React api
-✅处理复杂模式：遗留上下文，forwardRef, defaultProps
-✅基于内存的可恢复管道在中断中存活
-✅零容忍不完整的迁移运行到完全成功
-✅严格模式感知测试修复
-✅测试库v16+兼容性验证
-✅错误边界和异步测试模式更新

# #的先决条件

这个插件假设你是从**React 18**代码库迁移过来的。如果你使用的是React16/17，首先使用**react18-upgrade**插件来升级到React 18.3.1，然后使用这个插件进行React 19的最终升级。

# #源

这个插件是[Awesome Copilot]（https://github.com/github/awesome-copilot）的一部分。

# #许可证

麻省理工学院