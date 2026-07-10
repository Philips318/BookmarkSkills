---
name: salesforce-component-standards
description: 'Quality standards for Salesforce Lightning Web Components (LWC), Aura components, and Visualforce pages. Covers SLDS 2 compliance, accessibility (WCAG 2.1 AA), data access pattern selection, component communication rules, XSS prevention, CSRF enforcement, FLS/CRUD in AuraEnabled methods, view state management, and Jest test requirements. Use this skill when building or reviewing any Salesforce UI component to enforce platform-specific security and quality standards.'
---
# Salesforce组件质量标准

将这些检查应用于您编写或审查的每个LWC、Aura组件和Visualforce页面。

第1节- LWC质量标准

1.1数据访问模式选择

在编写JavaScript控制器代码之前，选择正确的数据访问模式：

|用例|模式|为什么||---|---|---|
|响应式读取一条记录（跟随导航）|`@wire(getRecord, { recordId, fields })`|闪电数据服务-缓存，响应|
|单个对象的标准CRUD形式|`<lightning-record-form>`或`<lightning-record-edit-form>`|内置FLS、CRUD和可访问性|
|复杂的服务器查询或过滤列表|`@wire(apexMethodName, { param })`上的`cacheable=true`方法|允许缓存；参数改变|时电线重新点燃
|用户触发的动作、DML或不可缓存的服务器调用|命令式`apexMethodName(params).then(...).catch(...)`| DML连接的方法需要`@AuraEnabled`，没有`cacheable=true`|就不能是`@AuraEnabled`|跨组件通信（无共享父节点）|闪电消息服务（LMS） |解耦，跨DOM边界工作|
|多对象图关系| GraphQL`@wire(gql, { query, variables })`|复杂相关数据的单次往返|

1.2安全规则

|规则|执行||---|---|
在模板中使用`{expression}`绑定-框架会自动转义。永远不要使用`this.template.querySelector('.el').innerHTML = userValue`|
在SOQL中使用`WITH USER_MODE`或显式`Schema.sObjectType`检查|
在组件JavaScript中没有硬编码的特定于org的id |查询或作为prop传递-从不在源|中嵌入记录id
|`@api`父属性：在使用前验证|父属性可以传递任何东西-在使用作为查询参数|之前验证类型和范围

1.3 SLDS 2和样式标准- **永远不要**硬编码颜色：`color: #FF3366`→使用`color: var(--slds-c-button-brand-color-background)`或语义SLDS令牌。
- **永远不要**使用`!important`重写SLDS类- compose与自定义CSS属性。
-使用`<lightning-*>`基础组件：`lightning-button`，`lightning-input`,`lightning-datatable`，`lightning-card`等。
-基本组件包括内置的SLDS 2，暗模式和可访问性-避免重新实现它们的行为
-如果使用自定义CSS，在声明完成之前，在**亮模式**和**暗模式**下进行测试。

1.4可访问性要求（WCAG 2.1 AA）

每个LWC组件在被认为完成之前必须通过所有这些：-[]所有表单输入都有`<label>`或`aria-label`-永远不要使用占位符作为唯一的标签
-[]所有图标按钮都有`alternative-text`或`aria-label`来描述动作
[]所有交互元素都可以通过键盘（Tab, Enter, Space, Escape）访问和操作
-[]颜色不是传达状态的唯一手段-与文本，图标或`aria-*`属性配对
-[]错误信息通过`aria-describedby`与输入相关联
[]焦点管理在情态中是正确的-焦点在打开时移到情态中，在关闭时移回

1.5组件通信规则

|方向|机制||---|---|
|`@api`属性或调用`@api`方法|
|孩子→父母|`CustomEvent`-`this.dispatchEvent(new CustomEvent('eventname', { detail: data }))`|
|同级/不相关组件| LMS (Lightning Message Service) |
永远不要使用|、`document.querySelector`、`window.*`或Pub/Sub库

对于流量筛组件：
—需要到达Flow运行时的事件必须设置`bubbles: true`和`composed: true`。
-公开`@api value`，以便与Flow变量进行双向绑定。

1.6 JavaScript性能规则

- **`connectedCallback`**没有副作用：它运行在每个DOM附加-避免DML，繁重的计算，或呈现状态突变在这里。
**保护`renderedCallback`**：总是使用布尔保护来防止无限渲染循环。
- **避免反应性属性陷阱**：在`renderedCallback`内部设置一个反应性属性会导致重新渲染-只有在必要的时候才使用它。
- **不要将大型数据集存储在组件状态** -分页或流大型结果代替。1.7 Jest测试要求

每个处理用户交互或检索Apex数据的组件都必须有一个Jest测试：```javascript
// Minimum test coverage expectations
it('renders the component with correct title', async () => { ... });
it('calls apex method and displays results', async () => { ... });  // Wire mock
it('dispatches event when button is clicked', async () => { ... });
it('shows error state when apex call fails', async () => { ... }); // Error path
```
使用`@salesforce/sfdx-lwc-jest`模拟实用程序：
-`wire`适配器模拟：`setImmediate`+`emit({ data, error })`- Apex方法模拟：`jest.mock('@salesforce/apex/MyClass.myMethod', ...)`---

第2节-光环组件标准

何时使用Aura vs LWC

- **新组件：总是LWC**，除非目标上下文是Aura-only（例如扩展`force:appPage`，在遗留管理包中使用特定于aura的事件）。
- **将Aura迁移到LWC**：首选LWC，逐个组件迁移；LWC可以嵌入到Aura组件中。

2.2 Aura安全规则

-`@AuraEnabled`控制器方法必须声明`with sharing`并执行CRUD/FLS- Aura不会自动执行它们。
-永远不要在`<div>`非绑定帮助程序中使用`{!v.something}`和未转义的用户数据-使用`<ui:outputText value="{!v.text}" />`或`<c:something>`进行转义。
-在SOQL / Apex逻辑中使用组件属性之前，验证组件属性的所有输入。

2.3光环事件设计- **组件事件**用于父子通信-最低范围。
只有当组件事件无法到达目标时，应用程序才会发生事件——它们会广播到整个应用程序，可能会造成性能和维护问题。
-对于混合LWC + Aura堆栈：使用闪电消息服务来解耦通信-不依赖于Aura应用程序事件到达LWC组件。

---

第3节- Visualforce安全标准

XSS预防```xml
<!-- ❌ NEVER — renders raw user input as HTML -->
<apex:outputText value="{!userInput}" escape="false" />

<!-- ✅ ALWAYS — auto-escaping on -->
<apex:outputText value="{!userInput}" />
<!-- Default escape="true" — platform HTML-encodes the output -->
```
规则：对于用户控制的数据，`escape="false"`是不可接受的。如果必须呈现富文本，请在输出之前使用白名单对服务器端进行消毒。

3.2 CSRF保护

将`<apex:form>`用于所有回发操作—平台将自动向表单中注入CSRF令牌。**不要**使用原始的`<form method="POST">`HTML元素，这会绕过CSRF保护。

控制器中的SOQL注入预防```apex
// ❌ NEVER
String soql = 'SELECT Id FROM Account WHERE Name = \'' + ApexPages.currentPage().getParameters().get('name') + '\'';
List<Account> results = Database.query(soql);

// ✅ ALWAYS — bind variable
String nameParam = ApexPages.currentPage().getParameters().get('name');
List<Account> results = [SELECT Id FROM Account WHERE Name = :nameParam];
```
查看状态管理检查表

[]查看状态小于135 KB（检查在浏览器开发工具或Salesforce视图状态选项卡）
—[]仅用于服务器端计算的字段声明为`transient`-[]大型集合不会在不必要的回发中持久化
—[]`readonly="true"`在`<apex:page>`上设置为只读页面跳过视图状态序列化

Visualforce控制器中的FLS / CRUD```apex
// Before reading a field
if (!Schema.sObjectType.Account.fields.Revenue__c.isAccessible()) {
    ApexPages.addMessage(new ApexPages.Message(ApexPages.Severity.ERROR, 'You do not have access to this field.'));
    return null;
}

// Before performing DML
if (!Schema.sObjectType.Account.isDeletable()) {
    throw new System.NoAccessException();
}
```
标准控制器自动为绑定字段执行FLS。**自定义控制器不** - FLS必须手动执行。

---

快速参考-组件反模式摘要

|反模式|技术|风险|修复||---|---|---|---|
| LWC | XSS |使用模板绑定`{expression}`|
|硬编码十六进制颜色|LWC/Aura|暗模式/ SLDS 2打破|使用SLDS CSS自定义属性|
|图标按钮上缺少`aria-label`|LWC/Aura/VF|可访问性失败|添加`alternative-text`或`aria-label`|
|`renderedCallback`| LWC |无限渲染循环|添加`hasRendered`布尔保护|
|父子应用事件| Aura |不必要的广播作用域|使用组件事件代替|
|`escape="false"`on用户数据| Visualforce | XSS |删除-使用默认转义|
| Raw`<form>`postback | Visualforce | CSRF漏洞|使用`<apex:form>`|
|无`with sharing`自定义控制器| VF / Apex |数据暴露|添加`with sharing`声明|
|自定义控制器未检查FLS | VF / Apex |特权升级|添加`Schema.sObjectType`检查|
| SOQL连接URL参数| VF / Apex | SOQL注入|使用绑定变量|