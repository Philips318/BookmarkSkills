# React Router v5→v6 - Scope Assessment

##为什么这是一个独立的Sprint

React Router v5→v6是一个完整的API重写。不像大多数React 18的升级步骤只涉及单个模式，路由器迁移影响：

—每个`<Route>`组件
-每个`<Switch>`（由`<Routes>`取代）
-每个`useHistory()`（由`useNavigate()`取代）
-每个`useRouteMatch()`（由`useMatch()`取代）
-每个`<Redirect>`（由`<Navigate>`取代）
-嵌套路由定义（全新模型）
—路由参数访问
-查询字符串处理

尝试将此作为React 18升级冲刺的一部分，将显著影响迁移的范围。

##推荐方法

###选项A -延迟路由器迁移（推荐）

在React 18升级期间使用`react-router-dom@5.3.4`和`--legacy-peer-deps`。这是React -router团队为了在旧根上兼容React 18而明确记录的一个支持的解决方案。```bash
# In the React 18 dep surgeon:
npm install react-router-dom@5.3.4 --legacy-peer-deps
```
文档在package.json：```json
"_legacyPeerDepsReason": {
  "react-router-dom@5.3.4": "Router v5→v6 migration deferred to separate sprint. React 18 peer dep mismatch only - no API incompatibility on legacy root."
}
```
然后在React 18升级稳定后，将v5→v6的迁移安排为自己的冲刺。

###选项B -迁移路由器作为React 18 Sprint的一部分

只有在以下情况下才选择：

-应用程序有最小的路由（< 10条路由，没有嵌套的路由，没有复杂的导航逻辑）
团队有足够的带宽，并且sprint时间表允许

范围评估扫描

在决定之前运行这个命令来了解路由器迁移范围：```bash
echo "=== Route definitions ==="
grep -rn "<Route\|<Switch\|<Redirect" src/ --include="*.js" --include="*.jsx" | grep -v "\.test\." | wc -l

echo "=== useHistory calls ==="
grep -rn "useHistory()" src/ --include="*.js" --include="*.jsx" | grep -v "\.test\." | wc -l

echo "=== useRouteMatch calls ==="
grep -rn "useRouteMatch()" src/ --include="*.js" --include="*.jsx" | grep -v "\.test\." | wc -l

echo "=== withRouter HOC ==="
grep -rn "withRouter" src/ --include="*.js" --include="*.jsx" | grep -v "\.test\." | wc -l

echo "=== history.push / history.replace ==="
grep -rn "history\.push\|history\.replace\|history\.go" src/ --include="*.js" --include="*.jsx" | grep -v "\.test\." | wc -l
```
* *决策指南:* *

-总命中数< 30→路由器迁移在这个sprint中是可行的
-总命中30-100→强烈建议延迟
-总命中数bbb100→必须延迟-需要单独冲刺

## v5→v6 API变更汇总

| v5 | v6 | Notes ||---|---|---|
|`<Switch>`|`<Routes>`|直接替代|
|`<Route path="/" component={C}>`|`<Route path="/" element={<C />}>`|元素支柱，不是组件|
|`<Route exact path="/">`|`<Route path="/">`|完全是v6 |的默认值
|`<Redirect to="/new">`|`<Navigate to="/new" />`|组件更名为|
|`useHistory()`|`useNavigate()`|返回一个函数，而不是对象|
|`history.push('/path')`|`navigate('/path')`|直拨|
|`history.replace('/path')`|`navigate('/path', { replace: true })`|选项对象|
|`useRouteMatch()`|`useMatch()`|不同的返回形状|
|`match.params`|`useParams()`|挂钩代替道具|
|嵌套路由内联|配置中的嵌套路由|布局路由概念|
|`withRouter`HOC |`useNavigate`/`useParams`挂钩| HOC删除|