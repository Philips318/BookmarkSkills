---
description: 'Code components for canvas apps implementation, security, and configuration'
applyTo: '**/*.{ts,tsx,js,json,xml,pcfproj,csproj}'
---
#为Canvas应用编写组件代码

专业开发人员可以使用Power Apps组件框架来创建可以在画布应用中使用的代码组件。应用程序开发者可以使用Power Apps组件框架来创建、导入和添加代码组件到canvas应用程序，使用[Microsoft Power Platform CLI]（https://learn.microsoft.com/en-us/power-apps/developer/component-framework/get-powerapps-cli）。

> **注**：某些api可能无法在canvas应用程序中使用。我们建议您检查每个API，以确定它在哪里可用。

##安全考虑

b> **警告**：代码组件包含可能不是由Microsoft生成的代码，并且在Power Apps Studio中呈现时可能会访问安全令牌和数据。当向canvas应用程序添加代码组件时，请确保代码组件解决方案来自可信任的来源。在运行canvas应用程序时不存在此漏洞。

Power Apps Studio中的安全警告当你在Power Apps Studio中打开一个包含代码组件的canvas应用时，会出现一条关于潜在不安全代码的警告消息。Power Apps Studio环境中的代码组件可以访问安全令牌；因此，只应该打开来自可信来源的组件。

* *最佳实践:* *
在将所有代码组件导入环境之前，管理员和系统定制者应该检查并验证它们
-零部件只有在验证后才能提供给制造商
—当您使用非托管解决方案导入代码组件或使用`pac pcf push`安装代码组件时，将显示`Default`发布器

！(安全警告)(https://learn.microsoft.com/en-us/power-apps/developer/component-framework/media/canvas-app-safety-warning.png)

# #先决条件—需要Power Apps许可。更多信息：[Power Apps组件框架许可]（https://learn.microsoft.com/en-us/power-apps/developer/component-framework/overview#licensing）
—在环境中启用Power Apps组件框架特性需要系统管理员权限

##启用Power Apps组件框架特性

要将代码组件添加到应用程序中，你需要在每个想要使用它们的环境中启用Power Apps组件框架特性。默认情况下，Power Apps组件特性是为模型驱动的应用启用的。

###步骤启用画布应用程序：

1. 登录到[Power Apps]（https://powerapps.microsoft.com/）
2. 选择**Settings** ！[设置](https://learn.microsoft.com/en-us/power-apps/developer/component-framework/media/settings.png)，然后选择**Admin Center**

！[设置和管理中心]（https://learn.microsoft.com/en-us/power-apps/developer/component-framework/media/select-admin-center-from-settings.png）3. 在左侧窗格中，选择**Environments**，选择要启用该特性的环境，然后选择**Settings**
4. 展开**产品**，选择**功能**
5. 从可用功能列表中，打开** canvas应用的Power Apps组件框架**，然后选择**Save**

！[启用Power Apps组件框架]（https://learn.microsoft.com/en-us/power-apps/developer/component-framework/media/enable-pcf-feature.png）

##实现代码组件

在环境中启用Power Apps组件框架特性之后，就可以开始为代码组件实现逻辑了。有关一步一步的教程，请转到[创建您的第一个代码组件]（https://learn.microsoft.com/en-us/power-apps/developer/component-framework/implementing-controls-using-typescript）。

**建议**：在开始实现之前，检查canvas应用中代码组件的[限制]（https://learn.microsoft.com/en-us/power-apps/developer/component-framework/limitations）。

##添加组件到Canvas应用程序

1. 进入Power Apps Studio
2. 创建一个新的canvas应用程序，或者编辑一个你想要添加代码组件的现有应用程序b> **重要**：在进行下一步之前，请确保包含代码组件的解决方案.zip文件已经[导入]（https://learn.microsoft.com/en-us/power-apps/maker/data-platform/import-update-export-solutions）到Microsoft Dataverse中。

3. 在左侧窗格中，选择**Add(+)**，然后选择**Get more components**

！(插入组件)(https://learn.microsoft.com/en-us/power-apps/developer/component-framework/media/insert-code-components-using-get-more-components.png)

4. 选择**Code**选项卡，从列表中选择一个组件，然后选择**Import**

！[导入组件]（https://learn.microsoft.com/en-us/power-apps/developer/component-framework/media/insert-component-add-sample-component.png）

5. 在左侧窗格中，选择**+**，展开**Code components**，然后选择要添加到应用中的组件

！[添加组件]（https://learn.microsoft.com/en-us/power-apps/developer/component-framework/media/add-sample-component-from-list.png）

> **注**：也可以通过选择“**插入>自定义>导入组件**”添加组件。该选项将在将来的版本中删除，因此我们建议使用上面描述的流程。

组件属性

在Properties选项卡上，您将注意到显示了代码组件属性。！[默认代码组件属性面板]（https://learn.microsoft.com/en-us/power-apps/developer/component-framework/media/property-pane-with-parameters.png）

**注**：如果您希望属性在默认属性选项卡中可用，则可以通过更新代码组件的清单版本来重新导入现有代码组件。和以前一样，这些属性将继续在Advanced properties选项卡上可用。

##从Canvas应用中删除代码组件

1. 打开添加了代码组件的应用程序
2. 在左侧窗格中，选择**Tree view**，然后选择添加了代码组件的屏幕
3. 在组件旁边，选择**More（…）**，然后选择**删除**

！[删除代码组件]（https://learn.microsoft.com/en-us/power-apps/developer/component-framework/media/delete-code-component.png）

4. 保存应用程序以查看更改

更新现有的代码组件每当您更新代码组件并希望查看运行时更改时，您都需要更改清单文件中的`version`属性。我们建议您在进行更改时更改组件的版本。

b> **注**：现有的代码组件只有在应用程序关闭或重新打开Power Apps Studio时才会更新。当你重新打开应用程序时，它会要求你更新代码组件。简单地删除或添加代码组件到应用程序中不会更新组件。首先发布更新后的解决方案中的所有自定义，否则将不会显示对代码组件所做的更新。

##参见Also

- [Power Apps组件框架概述]（https://learn.microsoft.com/en-us/power-apps/developer/component-framework/overview）
-[创建第一个代码组件]（https://learn.microsoft.com/en-us/power-apps/developer/component-framework/implementing-controls-using-typescript）
-[学习Power Apps组件框架]（https://learn.microsoft.com/en-us/training/paths/use-power-apps-component-framework）