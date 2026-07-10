# React 18升级插件

用于将React16/17类组件代码库迁移到React 18.3.1的企业工具包。包括6个专门的代理和7个技能，针对升级遗留的重类应用程序的特定挑战。

# #安装```bash
copilot plugin install react18-upgrade@awesome-copilot
```
包含的内容

# # #代理

1. **react18-commander**—主编排器，通过审计、依赖关系、类组件操作、自动批处理修复和测试验证阶段协调整个迁移管道。

2. **react18-auditor** -深度扫描专家，识别每一个React 18的破坏性变化：不安全的生命周期方法、遗留上下文、字符串引用、批处理漏洞和所有弃用模式。

3. **react18-dep-surgeon** -依赖升级专家，精确地固定react@18.3.1，将测试库升级到v14+，解决所有对等冲突，并返回GO/NO-GO确认。4. **react18-生命周期和API迁移专家，执行语义迁移：
-`componentWillMount`→`componentDidMount`或构造函数
-`componentWillReceiveProps`→`getDerivedStateFromProps`或`componentDidUpdate`-`componentWillUpdate`→`getSnapshotBeforeUpdate`或`componentDidUpdate`- Legacy Context→`createContext`-字符串refs→`React.createRef()`-`findDOMNode`→直接裁判
-`ReactDOM.render`→`createRoot`5. **react18-batching-fixer** -自动批处理回归专家，识别并修复React 18中#1的静默运行时中断：setState调用依赖于即时中间重新渲染的异步方法。

6. **react18-test-guardian** -测试套件修复器，处理酶到RTL重写、RTL v14 API更新、自动批处理测试回归、StrictMode双调用更改，并运行测试直到零失败。

# # #技能

1. ** React -audit-grep-patterns** -引用grep模式来跨类组件审计React 18的弃用。2. 用于识别和修复自动批处理回归的模式和策略。

3. **react18- deep -compatibility** - React 18的依赖兼容性矩阵，包含test -library、Apollo、Emotion、React -router的迁移路径。

4. **react18-enzyme-to-rtl** -完整的指南重写酶测试反应测试库（RTL v14+）。

5. **react18-legacy-context** -遗留上下文API的迁移模式→`createContext`。

6. **react18-lifecycle-patterns** -所有三种不安全生命周期方法的详细迁移模式。

7. **react18-string-refs** -迁移字符串refs到`React.createRef()`的参考实现。

##快速入门```
Ask: "Start implementing React 18 migration for my class-component codebase"
```
react18指挥官将指导你：

1. 审核→识别所有破坏性变更
2. 升级到react@18.3.1+兼容库
3. 类手术→迁移生命周期方法和api
4. 批处理修复→修复自动批处理回归
5. 测试→迁移测试套件并运行到绿色

为什么是React 18.3.1？

React 18.3.1发布后，会对React 19将要移除的每个API发出显式警告。一个干净的18.3.1运行零警告是React 19迁移的直接先决条件。

##主要特性-✅目标是类组件密集型的代码库（不仅仅是功能组件模式）
-✅自动批处理问题检测和`flushSync`建议
-✅酶测试检测与完整的RTL重写能力
-✅基于内存的可恢复管道-在中断中存活
-✅对不完全迁移零容忍-运行到完全成功
-✅严格模式感知测试修复
-✅阿波罗客户端，情感，反应路由器兼容性处理

# #源

这个插件是[Awesome Copilot]（https://github.com/github/awesome-copilot）的一部分。

# #许可证

麻省理工学院