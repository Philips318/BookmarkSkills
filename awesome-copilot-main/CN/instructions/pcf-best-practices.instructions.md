---
description: 'Best practices and guidance for developing PCF code components'
applyTo: '**/*.{ts,tsx,js,json,xml,pcfproj,csproj,css,html}'
---
#代码组件的最佳实践和指南

开发、部署和维护代码组件需要跨多个领域的知识组合。本文为开发代码组件的专业人员概述了已建立的最佳实践和指导。

Power Apps组件框架

避免将开发构建部署到Dataverse

代码组件可以在[生产或开发模式]（https://learn.microsoft.com/en-us/power-apps/developer/component-framework/code-components-alm#building-pcfproj-code-component-projects）中构建。避免将开发构建部署到Dataverse，因为它们会对性能产生不利影响，甚至可能由于它们的大小而无法部署。即使您计划稍后部署一个发布构建，如果您没有一个自动化的发布管道，也很容易忘记重新部署。更多信息：[调试自定义控件]（https://learn.microsoft.com/en-us/power-apps/developer/component-framework/debugging-custom-controls）。

避免使用不支持的框架方法其中包括使用`ComponentFramework.Context`上存在的未记录的内部方法。这些方法可能有效，但由于不支持，它们可能在将来的版本中停止工作。不支持使用访问主机应用程序HTML文档对象模型（DOM）的控制脚本。宿主应用程序DOM中代码组件边界之外的任何部分都可以随时更改，恕不另行通知。

使用`init`方法请求网络所需的资源

当托管上下文加载代码组件时，首先调用`init`方法。使用此方法请求元数据等任何网络资源，而不是等待`updateView`方法。如果在请求返回之前调用了`updateView`方法，那么代码组件必须处理这种状态并提供一个可视的加载指示器。

清理`destroy`方法中的资源当代码组件从浏览器DOM中移除时，托管上下文调用`destroy`方法。使用`destroy`方法关闭任何`WebSockets`，并删除添加到容器元素外部的事件处理程序。如果你正在使用React，在`destroy`方法中使用`ReactDOM.unmountComponentAtNode`。以这种方式清理资源可以防止在给定的浏览器会话中加载和卸载代码组件所导致的任何性能问题。

避免不必要的调用刷新数据集属性

如果您的代码组件属于数据集类型，则绑定的数据集属性将公开`refresh`方法，该方法将导致托管上下文重新加载数据。调用此方法会不必要地影响代码组件的性能。

最小化对`notifyOutputChanged`的调用在某些情况下，不希望对每个调用`notifyOutputChanged`的UI控件（例如按键或鼠标移动事件）进行更新，因为更多的调用将导致更多的事件传播到父上下文。相反，可以考虑在控件失去焦点或用户的触摸或鼠标事件完成时使用事件。

检查API可用性

在为不同的主机（模型驱动的应用程序、画布应用程序、门户）开发代码组件时，请始终检查您正在使用的api在这些平台上的可用性。例如，`context.webAPI`在画布应用程序中不可用。有关单个API的可用性，请参见[电源应用组件框架API参考]（https://learn.microsoft.com/en-us/power-apps/developer/component-framework/reference/）。

###管理传递给`updateView`的临时空属性值当数据没有准备好时，将空值传递给`updateView`方法。您的组件应该考虑到这种情况，并预期数据可能为空，并且随后的`updateView`循环可以包含更新的值。`updateView`可用于[standard]（https://learn.microsoft.com/en-us/power-apps/developer/component-framework/reference/control/updateview）和[React]（https://learn.microsoft.com/en-us/power-apps/developer/component-framework/reference/react-control/updateview）组件。

模型驱动的应用程序

不要直接与`formContext`互动

如果您有使用客户机API的经验，您可能习惯于与`formContext`进行交互，以访问属性、控件和调用API方法，如`save`、`refresh`和`setNotification`。代码组件应该跨各种产品工作，比如模型驱动的应用程序、画布应用程序和仪表板，因此它们不能依赖于`formContext`。一种解决方法是将代码组件绑定到列，并向该列添加`OnChange`事件处理程序。代码组件可以更新列值，`OnChange`事件处理程序可以访问`formContext`。对自定义事件的支持将在将来添加，这将允许在控件外部通信更改，而无需添加列配置。

限制调用`WebApi`的大小和频率

在使用`context.WebApi`方法时，限制调用的次数和数据量。每次调用`WebApi`时，它都会计入用户的API权限和服务保护限制。在对记录执行CRUD操作时，请考虑负载的大小。通常，请求负载越大，代码组件的速度就越慢。

## Canvas Apps

最小化屏幕上的组件数量每次你添加一个组件到你的画布应用程序，它需要有限的时间来渲染。渲染时间随着您添加的每个组件而增加。当您使用Developer performance工具向屏幕添加更多组件时，请仔细测量代码组件的性能。

目前，每个代码组件都捆绑了自己的共享库，比如Fluent UI和React。加载同一库的多个实例不会多次加载这些库。但是，加载多个不同的代码组件会导致浏览器加载这些库的多个捆绑版本。将来，这些库将能够被加载并与代码组件共享。

允许开发者为你的代码组件设计样式当应用程序开发者从canvas应用程序中使用代码组件时，他们希望使用与应用程序其他部分匹配的样式。使用输入属性为主题元素（如颜色和大小）提供自定义选项。当使用Microsoft Fluent UI时，将这些属性映射到库提供的主题元素。将来，主题化支持将被添加到代码组件中，以使这个过程更容易。

遵循Canvas Apps性能最佳实践

Canvas应用程序从应用程序和解决方案检查器内部提供了广泛的最佳实践。在添加代码组件之前，请确保您的应用程序遵循这些建议。有关更多信息，请参见：

-提高canvas应用性能的小技巧
-[电源应用优化性能的注意事项]（https://powerapps.microsoft.com/blog/considerations-for-optimized-performance-in-power-apps/）

## TypeScript和JavaScript

ES5 vs ES6默认情况下，代码组件以ES5为目标以支持旧的浏览器。如果不想支持这些旧的浏览器，可以在`pcfproj`文件夹的`tsconfig.json`中将目标更改为ES6。更多信息：[ES5 vs ES6]（https://learn.microsoft.com/en-us/power-apps/developer/component-framework/debugging-custom-controls#es5-vs-es6）。

模块导入

总是将需要作为代码组件一部分的模块捆绑在一起，而不是使用需要使用`SCRIPT`标签加载的脚本。例如，如果您想使用非microsoft制图API（示例显示将`<script type="text/javascript" src="somechartlibrary.js></script>`添加到页面中），则代码组件中不支持这种方式。绑定所有必需的模块将代码组件与其他库隔离开来，并且还支持在脱机模式下运行。

b> **注**：目前还不支持使用组件清单中的库节点跨组件共享库。

# # #产品毛羽检查是工具扫描代码以发现潜在问题的地方。`pac pcf init`使用的模板将`eslint`模块安装到项目中，并通过添加`.eslintrc.json`文件对其进行配置。

要配置，在命令行使用：```bash
npx eslint --init
```
然后根据提示回答以下问题：

- **你想如何使用ESLint？**答：检查语法，发现问题，加强代码风格
- **你的项目使用什么类型的模块？**答案：JavaScript模块（import/export）
- **你的项目使用哪个框架？**答案：React
你的项目使用TypeScript吗？答：是的
- **你的代码在哪里运行？**答案：浏览器
- **你想如何为你的项目定义一个风格？回答：回答有关你的风格的问题
- **你希望你的配置文件是什么格式？**答：JSON
- **你用什么样式的缩进？**答案：空格
- **字符串用什么引号？**答案：单身
- **你用什么行尾？**答案：Windows
- **需要分号吗？答：是的

在使用`eslint`之前，需要向`package.json`添加一些脚本：```json
"scripts": {
   ...
   "lint": "eslint MY_CONTROL_NAME --ext .ts,.tsx",
   "lint:fix": "npm run lint -- --fix"
}
```
现在在命令行中，你可以使用：```bash
npm run lint:fix
```
此外，您可以通过添加到`.eslintrc.json`来添加要忽略的文件：```json
"ignorePatterns": ["**/generated/*.ts"]
```
HTML浏览器用户界面开发

###使用Microsoft Fluent UI React

[Fluent UI React]（https://developer.microsoft.com/fluentui#/get-started/web）是官方的[开源](https://github.com/microsoft/fluentui) React前端框架，旨在构建无缝适应各种微软产品的体验。Power Apps本身使用Fluent UI，这意味着您可以创建与其他应用程序一致的UI。

####从Fluent中使用基于路径的导入来减少包的大小

目前，`pac pcf init`使用的代码组件模板不会使用摇树，在摇树过程中，`webpack`会检测导入的模块没有被使用，并将其删除。如果你使用下面的命令从Fluent UI导入，它会导入并捆绑整个库：```typescript
import { Button } from '@fluentui/react'
```
为了避免导入和绑定整个库，你可以使用基于路径的导入，其中使用显式路径导入特定的库组件：```typescript
import { Button } from '@fluentui/react/lib/Button';
```
使用特定路径可以减少开发和发布版本中的包大小。

####优化React渲染

在使用React时，请遵循有关最小化组件渲染的React特定最佳实践：

-仅在绑定属性或框架方面更改需要UI反映更改时，才在`updateView`方法中调用`ReactDOM.render`。您可以使用`updatedProperties`来确定发生了什么变化。
-使用`PureComponent`（类组件）或`React.memo`（功能组件），尽可能避免不必要的重新渲染。
-对于大型React组件，将UI分解为更小的组件以提高性能。
避免在渲染函数中使用箭头函数和函数绑定，因为这些做法会在每次渲染时创建一个新的回调闭包。

检查可访问性确保代码组件是可访问的，以便仅键盘和屏幕阅读器用户可以使用它们：

-为mouse/touch事件提供键盘导航选项
-确保设置`alt`和[ARIA](https://developer.mozilla.org/en-US/docs/Web/Accessibility/ARIA)（可访问的富互联网应用程序）属性，以便屏幕阅读器宣布代码组件接口的准确表示
-现代浏览器开发工具提供了检查可访问性的有用方法

更多信息：[在Power apps中创建可访问的画布应用程序]（https://learn.microsoft.com/en-us/powerapps/maker/canvas-apps/accessible-apps）。

总是使用异步网络调用

当进行网络调用时，永远不要使用同步阻塞请求，因为这会导致应用程序停止响应并导致性能变慢。更多信息：[与HTTP和HTTPS资源异步交互]（https://learn.microsoft.com/en-us/powerapps/developer/model-driven-apps/best-practices/business-logic/interact-http-https-resources-asynchronously）。

为多个浏览器编写代码模型驱动的应用程序、画布应用程序和门户都支持多个浏览器。确保只使用所有现代浏览器支持的技术，并针对目标受众使用一组具有代表性的浏览器进行测试。

-[限制和配置]（https://learn.microsoft.com/en-us/powerapps/maker/canvas-apps/limits-and-config）
-[支持的浏览器]（https://learn.microsoft.com/en-us/power-platform/admin/supported-web-browsers-and-mobile-devices）
- [office使用的浏览器]（https://learn.microsoft.com/en-us/office/dev/add-ins/concepts/browsers-used-by-office-web-add-ins）

代码组件应该计划支持多种客户端和屏幕格式

代码组件可以在多个客户端（模型驱动应用程序、画布应用程序、门户）和屏幕格式（手机、平板电脑、web）中呈现。-使用`trackContainerResize`允许代码组件响应可用宽度和高度的变化
—使用`allocatedHeight`和`allocatedWidth`可以与`getFormFactor`结合，以确定代码组件是否在移动，平板电脑或web客户端上运行
实现`setFullScreen`允许用户在空间有限的情况下扩展使用整个可用屏幕
如果代码组件不能在给定的容器大小中提供有意义的体验，它应该适当地禁用功能并向用户提供反馈

总是使用有作用域的CSS规则当您使用CSS对代码组件实现样式时，请使用应用于组件容器`DIV`元素的自动生成的CSS类，确保CSS的作用域适用于组件。如果CSS的作用域是全局的，那么它可能会破坏呈现代码组件的表单或屏幕的现有样式。

例如，如果你的命名空间是`SampleNamespace`，你的代码组件名称是`LinearInputComponent`，你可以添加一个自定义的CSS规则：```css
.SampleNamespace\.LinearInputComponent rule-name
```
避免使用Web存储对象

代码组件不应该使用HTML web存储对象（如`window.localStorage`和`window.sessionStorage`）来存储数据。本地存储在用户浏览器或移动客户端的数据不安全，也不能保证可靠地可用。

##ALM/AzureDevOps/GitHub有关使用ALM/AzureDevOps/GitHub.的代码组件的最佳实践，请参阅文章[代码组件应用程序生命周期管理（ALM）]（https://learn.microsoft.com/en-us/power-apps/developer/component-framework/code-components-alm）

##相关文章

-[什么是代码组件]（https://learn.microsoft.com/en-us/power-apps/developer/component-framework/custom-controls-overview）
-为canvas应用程序编写组件代码
-[创建和构建代码组件]（https://learn.microsoft.com/en-us/power-apps/developer/component-framework/create-custom-controls-using-pcf）
-[学习Power Apps组件框架]（https://learn.microsoft.com/en-us/training/paths/use-power-apps-component-framework）
-[在Power Pages中使用代码组件]（https://learn.microsoft.com/en-us/power-apps/maker/portals/component-framework）