---
description: 'Limitations and restrictions of Power Apps Component Framework'
applyTo: '**/*.{ts,tsx,js,json,xml,pcfproj,csproj}'
---
#局限性

使用Power Apps组件框架，您可以创建自己的代码组件来改善Power Apps和Power Pages的用户体验。尽管您可以创建自己的组件，但仍有一些限制限制了开发人员在代码组件中实现某些功能。以下是一些限制：

# # 1。依赖于数据的api不适用于画布应用程序

依赖于Microsoft Dataverse的api，包括WebAPI，还不能用于Power Apps的canvas应用程序。有关单个API的可用性，请参见[Power Apps组件框架API参考]（https://learn.microsoft.com/en-us/power-apps/developer/component-framework/reference/）。

# # 2。捆绑外部库或使用平台库

代码组件应该要么使用[React控件和平台库](https://learn.microsoft.com/en-us/power-apps/developer/component-framework/react-controls-platform-libraries)，要么将包括外部库内容在内的所有代码捆绑到主代码捆绑包中。要查看Power Apps命令行界面如何帮助将外部库内容捆绑到特定于组件的bundle中的示例，请参见[Angular flip component](https://learn.microsoft.com/en-us/power-apps/developer/component-framework/sample-controls/angular-flip-control) example。

# # 3。不使用HTML Web存储对象

代码组件不应该使用HTML web存储对象（如`window.localStorage`和`window.sessionStorage`）来存储数据。本地存储在用户浏览器或移动客户端的数据不安全，不能保证可靠可用。

# # 4。Canvas应用程序不支持自定义验证

Power Apps画布应用程序不支持代码组件中的自定义验证。使用连接器来获取数据并执行操作。

##相关主题

- [Power Apps组件框架API参考]（https://learn.microsoft.com/en-us/power-apps/developer/component-framework/reference/）
- [Power Apps组件框架概述]（https://learn.microsoft.com/en-us/power-apps/developer/component-framework/overview）