---
description: 'How to use and run PCF sample components from the PowerApps-Samples repository'
applyTo: '**/*.{ts,tsx,js,json,xml,pcfproj,csproj}'
---
#如何使用示例组件

本节列出的所有示例组件都可以从[github.com/microsoft/PowerApps-Samples/tree/master/component-framework]（https://github.com/microsoft/PowerApps-Samples/tree/master/component-framework）下载，以便您可以在模型驱动或画布应用程序中试用它们。

本节下的各个示例组件主题为您提供了示例组件的概述、其可视化外观以及指向完整示例组件的链接。

##在你可以尝试样本组件之前

要尝试示例组件，您必须首先：

—[下载]（https://docs.github.com/repositories/working-with-files/using-files/downloading-source-code-archives#downloading-source-code-archives-from-the-repository-view）或[克隆]（https://docs.github.com/repositories/creating-and-managing-repositories/cloning-a-repository）此存储库[github.com/microsoft/PowerApps-Samples]（https://github.com/microsoft/PowerApps-Samples）。
—安装[Install Power Platform CLI for Windows]（https://learn.microsoft.com/en-us/power-platform/developer/cli/introduction#install-power-platform-cli-for-windows）。

##尝试示例组件

按照[README.md]（https://github.com/microsoft/PowerApps-Samples/blob/master/component-framework/README.md）中的步骤生成包含控件的解决方案，这样您就可以在模型驱动或画布应用程序中导入和尝试示例组件。

##如何运行示例组件使用以下步骤在模型驱动或画布应用程序中导入和尝试样例组件。

循序渐进的过程

1. **下载或克隆存储库**
—[Download]（https://docs.github.com/repositories/working-with-files/using-files/downloading-source-code-archives#downloading-source-code-archives-from-the-repository-view）或[clone](https://docs.github.com/repositories/creating-and-managing-repositories/cloning-a-repository) [github.com/microsoft/PowerApps-Samples]（https://github.com/microsoft/PowerApps-Samples）。

2. **打开开发者命令提示符**
-打开[Visual Studio的开发人员命令提示符]（https://learn.microsoft.com/visualstudio/ide/reference/command-prompt-powershell）并导航到`component-framework`文件夹。
—在Windows操作系统下，可以在“开始”中输入`developer command prompt`，打开开发人员命令提示符。

3. * * * *安装依赖关系
—导航到要尝试的组件，例如`IncrementControl`，并运行：   ```bash
   npm install
   ```
4. * * * *恢复项目
—命令执行完毕后，执行：   ```bash
   msbuild /t:restore
   ```
5. **创建解决方案文件夹**
-在示例组件文件夹中创建一个新文件夹：   ```bash
   mkdir IncrementControlSolution
   ```
6. 导航到解决方案文件夹**   ```bash
   cd IncrementControlSolution
   ```
7. * * * *初始化解决方案
—在创建的文件夹中，运行`pac solution init`命令：   ```bash
   pac solution init --publisher-name powerapps_samples --publisher-prefix sample
   ```
> **注**：该命令在文件夹中创建一个名为`IncrementControlSolution.cdsproj`的新文件。

8. **添加组件引用
—执行`pac solution add-reference`命令，将`path`设置为`.pcfproj`文件所在位置。   ```bash
   pac solution add-reference --path ../../IncrementControl
   ```
或   ```bash
   pac solution add-reference --path ../../IncrementControl/IncrementControl.pcfproj
   ```
b> **重要**：引用包含要添加控件的`.pcfproj`文件的文件夹。

9. **构建解决方案**
—要从解决方案项目生成zip文件，请执行以下三个命令：   ```bash
   msbuild /t:restore
   msbuild /t:rebuild /restore /p:Configuration=Release
   msbuild
   ```
—生成的解决方案zip文件在“`IncrementControlSolution\bin\debug`”文件夹下。

10. **导入解决方案**    - Now that you have the zip file, you have two options:
      - Manually [import the solution](https://learn.microsoft.com/powerapps/maker/data-platform/import-update-export-solutions) into your environment using [make.powerapps.com](https://make.powerapps.com/).
      - Alternatively, to import the solution using Power Apps CLI commands, see the [Connecting to your environment](https://learn.microsoft.com/powerapps/developer/component-framework/import-custom-controls#connecting-to-your-environment) and [Deployment](https://learn.microsoft.com/powerapps/developer/component-framework/import-custom-controls#deploying-code-components) sections.
11. **添加组件到应用程序**    - Finally, to add code components to your model-driven and canvas apps, see:
      - [Add components to model-driven apps](https://learn.microsoft.com/powerapps/developer/component-framework/add-custom-controls-to-a-field-or-entity)
      - [Add components to canvas apps](https://learn.microsoft.com/powerapps/developer/component-framework/component-framework-for-canvas-apps#add-components-to-a-canvas-app)
可用的样例组件

该存储库包含许多示例组件，包括：

——AngularJSFlipControl
——CanvasGridControl
——ChoicesPickerControl
——ChoicesPickerReactControl
——CodeInterpreterControl
——ControlStateAPI
——DataSetGrid
——DeviceApiControl
——FacepileReactControl
——FluentThemingAPIControl
——FormattingAPIControl
——IFrameControl
——ImageUploadControl
——IncrementControl
——LinearInputControl
——LocalizationAPIControl
——LookupSimpleControl
——MapControl
——ModelDrivenGridControl
——MultiSelectOptionSetControl
——NavigationAPIControl
——ObjectOutputControl
——PowerAppsGridCustomizerControl
——PropertySetTableControl
——ReactStandardControl
——TableControl
——TableGrid
——WebAPIControl

每个示例都演示了Power Apps组件框架的不同方面，可以作为您自己的组件的学习资源或起点。