---
description: 'Code components for model-driven apps implementation and configuration'
applyTo: '**/*.{ts,tsx,js,json,xml,pcfproj,csproj}'
---
为模型驱动的应用编写组件代码

Power Apps组件框架使开发人员能够在模型驱动的应用中扩展可视化。专业开发人员可以使用[Microsoft Power Platform CLI]（https://learn.microsoft.com/en-us/power-apps/developer/component-framework/get-powerapps-cli）创建、调试、导入和添加代码组件到模型驱动的应用程序。

##组件使用情况

您可以将代码组件添加到：
——列
——网格
-子网格

在模型驱动的应用中。

b> **重要**:Power Apps组件框架默认为模型驱动的应用启用。参见[canvas apps的组件代码]（https://learn.microsoft.com/en-us/power-apps/developer/component-framework/component-framework-for-canvas-apps）了解如何为canvas应用启用Power apps组件框架。

##实现代码组件

在开始创建代码组件之前，请确保已经安装了使用Power Apps组件框架开发组件所需的所有[先决条件]（https://learn.microsoft.com/en-us/power-apps/developer/component-framework/create-custom-controls-using-pcf#prerequisites）。文章[创建您的第一个代码组件]（https://learn.microsoft.com/en-us/power-apps/developer/component-framework/implementing-controls-using-typescript）演示了逐步创建代码组件的过程。

添加代码组件到模型驱动的应用程序

要在模型驱动应用程序中向列或表添加代码组件，请参见[向模型驱动应用程序添加代码组件]（https://learn.microsoft.com/en-us/power-apps/developer/component-framework/add-custom-controls-to-a-field-or-entity）。

# # #的例子

**线性滑块控制：**

！[添加线性滑块控制]（https://learn.microsoft.com/en-us/power-apps/maker/model-driven-apps/media/add-slider.png）

**数据集网格组件：**

！[数据集网格组件]（https://learn.microsoft.com/en-us/power-apps/developer/component-framework/media/add-dataset-component.png）

更新现有的代码组件

每当更新代码组件并希望在运行时查看更改时，都需要在清单文件中选中version属性。

**最佳实践**：建议无论何时进行更改，都要碰撞组件的版本。

##参见Also

- [Power Apps组件框架概述]（https://learn.microsoft.com/en-us/power-apps/developer/component-framework/overview）
-[创建你的第一个代码组件]
-[学习Power Apps组件框架]（https://learn.microsoft.com/en-us/training/paths/use-power-apps-component-framework）