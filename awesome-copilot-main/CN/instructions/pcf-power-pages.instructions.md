---
description: 'Using code components in Power Pages sites'
applyTo: '**/*.{ts,tsx,js,json,xml,pcfproj,csproj}'
---
#在Power Pages中使用代码组件

Power Pages现在支持为使用Power apps组件框架创建的模型驱动应用构建控件。要在Power Pages网站网页中使用代码组件：

！[使用组件框架创建代码组件，然后将代码组件添加到模型驱动的应用表单中，并在门户的基本表单中配置代码组件字段]（https://learn.microsoft.com/en-us/power-pages/configure/media/component-framework/steps.png）

完成这些步骤后，用户可以使用具有相应[form]（https://learn.microsoft.com/en-us/power-pages/getting-started/add-form）组件的网页与代码组件进行交互。

# #先决条件

—需要系统管理员权限才能在环境中启用代码组件特性
-您的Power Pages网站版本需要为[9.3.3]。（https://learn.microsoft.com/en-us/power-apps/maker/portals/versions/version-9.3.3.x）或更高
—您的起始站点包需要为[9.2.2103]。（https://learn.microsoft.com/en-us/power-apps/maker/portals/versions/package-version-9.2.2103）或更高

创建并打包代码组件要了解如何在Power Apps组件框架中创建和打包代码组件，请转到[创建您的第一个组件]（https://learn.microsoft.com/en-us/power-apps/developer/component-framework/implementing-controls-using-typescript）。

支持的字段类型和格式

Power Pages支持使用代码组件的受限字段类型和格式。下表列出了所有支持的字段数据类型和格式：

支持类型:* * * *
——货币
——DateAndTime。DateAndTime
——DateAndTime。DateOnly
——小数
——枚举
-浮点数
——多个
——OptionSet
——单行模式。电子邮件
——单行模式。电话
——单行模式。文本
——单行模式。文本区域
——单行模式。股票
——单行模式。URL
——TwoOptions
——整个

有关更多信息，请参见[属性列表和描述]（https://learn.microsoft.com/en-us/power-apps/developer/component-framework/manifest-schema-reference/property#remarks）。

Power Pages中不支持的代码组件不支持以下代码组件api：
——[Device.captureAudio] (https://learn.microsoft.com/en-us/power-apps/developer/component-framework/reference/device/captureaudio)
——[Device.captureImage] (https://learn.microsoft.com/en-us/power-apps/developer/component-framework/reference/device/captureimage)
——[Device.captureVideo] (https://learn.microsoft.com/en-us/power-apps/developer/component-framework/reference/device/capturevideo)
——[Device.getBarcodeValue] (https://learn.microsoft.com/en-us/power-apps/developer/component-framework/reference/device/getbarcodevalue)
——[Device.getCurrentPosition] (https://learn.microsoft.com/en-us/power-apps/developer/component-framework/reference/device/getcurrentposition)
——[Device.pickFile] (https://learn.microsoft.com/en-us/power-apps/developer/component-framework/reference/device/pickfile)
——[工具](https://learn.microsoft.com/en-us/power-apps/developer/component-framework/reference/utility)

* *额外的限制:* *
—[uses-feature]（https://learn.microsoft.com/en-us/power-apps/developer/component-framework/manifest-schema-reference/uses-feature）元素不能设置为true
—Power Apps组件框架中[不支持的值元素]（https://learn.microsoft.com/en-us/power-apps/developer/component-framework/manifest-schema-reference/property#value-elements-that-are-not-supported）
不支持在表单中绑定多个字段的Power Apps Component Framework （PCF）控件

在模型驱动的应用中添加一个代码组件到字段中

要了解如何在模型驱动的应用程序中向字段添加代码组件，请转到[向字段添加代码组件]（https://learn.microsoft.com/en-us/power-apps/developer/component-framework/add-custom-controls-to-a-field-or-entity#add-a-code-component-to-a-column）。

b> **重要**：使用** web **的客户端选项，web浏览器可以使用Power Pages的代码组件。

添加Using Data工作区您还可以使用[Data workspace]（https://learn.microsoft.com/en-us/power-pages/configure/data-workspace-forms）向表单添加代码组件。

1. 在Data工作区表单设计器中编辑Dataverse表单时，选择一个字段
2. 选择**+ Component**并为该字段选择合适的组件

！[添加组件到表单]（https://learn.microsoft.com/en-us/power-pages/configure/media/component-framework/add-component-to-form.png）

3. 选择**保存**和**发布表单**

##配置Power Pages Site for Code组件

将代码组件添加到模型驱动应用程序中的字段后，您可以配置Power Pages以在表单中使用代码组件。

有两种方法可以启用代码组件：

在Design Studio中启用代码组件

使用design studio在表单上启用代码组件：

1. 在[将表单添加到页面]（https://learn.microsoft.com/en-us/power-pages/getting-started/add-form）之后，选择添加代码组件的字段并选择**Edit field**
2. 选择**Enable custom component**字段

！[在设计工作室启用自定义组件]（https://learn.microsoft.com/en-us/power-pages/configure/media/component-framework/enable-code-component.png）3. 预览站点时，您应该看到启用了自定义组件

###在门户管理应用中启用代码组件

通过使用Portals Management应用程序向基本表单添加代码组件：

1. 打开[门户管理]（https://learn.microsoft.com/en-us/power-pages/configure/portal-management-app）应用程序
2. 在左侧窗格中，选择**Basic Forms**
3. 选择要向其添加代码组件的表单
4. 选择相关的* * * *
5. 选择**基本表单元数据**
6. 选择**新建基本表单元数据**
7. 选择**类型**作为**属性**
8. 选择**属性逻辑名**
9. 输入* * * *标签
10. 对于**控件样式，选择**代码组件**
11. 保存并关闭表单

使用Portal Web API编写组件代码可以构建代码组件并将其添加到可以使用[门户Web API]（https://learn.microsoft.com/en-us/power-pages/configure/web-api-overview）执行创建、检索、更新和删除操作的网页中。在开发门户解决方案时，此特性允许更多的自定义选项。有关更多信息，请参见[实现示例门户Web API组件]（https://learn.microsoft.com/en-us/power-pages/configure/implement-webapi-component）。

##下一步

[教程：在门户中使用代码组件]（https://learn.microsoft.com/en-us/power-pages/configure/component-framework-tutorial）

##参见Also

- [Power Apps组件框架概述]（https://learn.microsoft.com/en-us/power-apps/developer/component-framework/overview）
-[创建你的第一个组件]（https://learn.microsoft.com/en-us/power-apps/developer/component-framework/implementing-controls-using-typescript）
-[在模型驱动的应用中添加代码组件到列或表]（https://learn.microsoft.com/en-us/power-apps/developer/component-framework/add-custom-controls-to-a-field-or-entity）