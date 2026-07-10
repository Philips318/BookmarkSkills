---
name: react18-dep-compatibility
description: 'React 18.3.1 and React 19 dependency compatibility matrix.'
---
# React依赖兼容性矩阵

React 18.3.1和React 19兼容性所需的最低版本。

无论何时检查依赖项是否支持目标React版本，解决对等依赖冲突，决定是否升级或使用`legacy-peer-deps`，或评估`react-router`v5到v6迁移的风险，都可以使用此技能。

在React升级期间运行`npm install`之前，以及在接受npm依赖冲突解决方案之前，检查这个矩阵，特别是在并发模式兼容性可能受到影响的情况下。
##核心升级目标

| Package | React 17 (current) | React 18.3.1 (min) | React 19 (min) | Notes ||---|---|---|---|---|
|`react`|x | **18.3.1** | **19.0.0** |引脚精确到18.3.1的R18管弦乐队|
|`react-dom`|x | **18.3.1** | **19.0.0** |必须完全匹配react版本|

##测试库

|包|反应18分钟|反应19分钟|笔记||---|---|---|---|
|`@testing-library/react`| **14.0.0** | **16.0.0** | RTL 13使用ReactDOM。渲染内部破碎在r18|
|`@testing-library/jest-dom`| **6.0.0** | **6.0.0** | v5工作，但v6有React 18匹配器更新|
|`@testing-library/user-event`| **14.0.0** | **14.0.0** | v13为同步，v14为异步-需要更改|的API
|`jest`| **X ** | **27。** | jest 27+与jdom 16+为React 18 |
|`jest-environment-jsdom`| **X ** | **27。x** |必须匹配jest版本|

阿波罗客户端

|包|反应18分钟|反应19分钟|笔记||---|---|---|---|
|`@apollo/client`| **3.8.0** | **3.11.0** | 3.8增加`useSyncExternalStore`为并发模式|
|`graphql`| **X ** | **16。x** |阿波罗3.8+ peer需要图形15或16 |

读取**`references/apollo-details.md`**以了解并发模式问题和MockedProvider更改。

# #情感

|包|反应18分钟|反应19分钟|笔记||---|---|---|---|
|`@emotion/react`| **11.10.0** | **11.13.0** | 11.10添加React 18并发模式支持|
|`@emotion/styled`| **11.10.0** | **11.13.0** |必须匹配@emotion/react版本|
|`@emotion/cache`| **11.10.0** | **11.13.0** |如果直接使用|

## React Router

|包|反应18分钟|反应19分钟|笔记||---|---|---|---|
|`react-router-dom`| **v6.0.0** | **v6.8.0** | v5→v6是一个突破性的迁移-参见|下面的详细信息
|`react-router-dom`v5 | 5.3.4（解决方案）|❌不支持|参见legacy peer deps note |

react-router v5→v6是一个单独的迁移冲刺。**读`references/router-migration.md`。

# #回来的

|包|反应18分钟|反应19分钟|笔记||---|---|---|---|
|`react-redux`| **8.0.0** | **9.0.0** | v7仅在R18遗留根上工作-在并发模式|上中断
|`redux`| **X ** | **5。Redux本身与框架无关——react-redux的版本很重要
|`@reduxjs/toolkit`| **1.9.0** | **2.0.0** | RTK 1.9在React 18 |测试

##其他常用包

|包|反应18分钟|反应19分钟|笔记||---|---|---|---|
|`react-query`/`@tanstack/react-query`| **4.0.0** | **5.0.0** | v3不支持并发模式|
|`react-hook-form`| **7.0.0** | **7.43.0** | v6有并发模式问题|
|`formik`| **2.2.9** | **2.4.0** | v2.2.9补丁为React 18 |
|`react-select`| **5.0.0** | **5.8.0** | v4与R18 |存在peer deep冲突
|`react-datepicker`| **4.8.0** | **6.0.0** | v4.8+添加了React 18支持|
|`react-dnd`| **16.0.0** | **16.0.0** | v15及以下有R18并发模式问题|
|`prop-types`| any | any |独立-不受React版本|的影响

---

冲突解决决策树```
npm ls shows peer conflict for package X
         │
         ▼
Does package X have a version that supports React 18?
  YES → npm install X@[min-compatible-version]
  NO  ↓
         │
Is the package critical to the app?
  YES → check GitHub issues for React 18 branch/fork
      → check if maintainer has a PR open
      → last resort: --legacy-peer-deps (document why)
  NO  → consider removing the package
```
-legacy-peer-deps规则

仅在以下情况下使用`--legacy-peer-deps`：
-该包没有React 18兼容的版本
-包被积极维护（而不是被抛弃）
-冲突只是对等层深层声明不匹配（不是实际的API不兼容）

**记录每次`--legacy-peer-deps`的使用**在package.json顶部的注释或在MIGRATION.md文件中解释为什么需要它。