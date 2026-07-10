---
description: 'Application lifecycle management (ALM) for PCF code components'
applyTo: '**/*.{ts,tsx,js,json,xml,pcfproj,csproj,sln}'
---
#代码组件应用生命周期管理（ALM）

ALM是一个用于描述软件应用程序生命周期管理的术语，它包括开发、维护和治理。更多信息：[使用Microsoft Power平台的应用程序生命周期管理（ALM）]（https://learn.microsoft.com/en-us/power-platform/alm/overview-alm）。

本文从Microsoft Dataverse代码组件的角度描述了处理生命周期管理的特定方面的注意事项和策略：

1. 开发和调试ALM事项
2. 代码组件解决方案策略
3. 版本控制和部署更新
4. Canvas应用程序ALM注意事项

开发和调试ALM注意事项

在开发代码组件时，您将遵循以下步骤：1. 使用`pac pcf init`从模板创建代码组件项目（`pcfproj`）。更多信息：[创建和构建代码组件]（https://learn.microsoft.com/en-us/power-apps/developer/component-framework/create-custom-controls-using-pcf）。
2. 实现代码组件逻辑。更多信息：[组件实现]（https://learn.microsoft.com/en-us/power-apps/developer/component-framework/custom-controls-overview#component-implementation）。
3. 使用本地测试工具调试代码组件。更多信息：[调试代码组件]（https://learn.microsoft.com/en-us/power-apps/developer/component-framework/debugging-custom-controls）。
4. 创建一个解决方案项目（`cdsproj`），并添加代码组件项目作为参考。更多信息：[Package a code component]（https://learn.microsoft.com/en-us/power-apps/developer/component-framework/import-custom-controls）。
5. 在发布模式下构建代码组件以进行分发和部署。

数据规避的两种部署方法

当你的代码组件准备好在模型驱动的应用程序、画布应用程序或门户中进行测试时：

1. **`pac pcf push`**：这将一次部署一个代码组件到由`--solution-unique-name`参数指定的解决方案，或者在没有指定解决方案时部署一个临时的PowerAppsTools解决方案。2. **使用`pac solution init`和`msbuild`**：构建一个引用一个或多个代码组件的`cdsproj`解决方案项目。每个代码组件都使用`pac solution add-reference`添加到`cdsproj`。解决方案项目可以包含对多个代码组件的引用，而代码组件项目只能包含单个代码组件。`cdsproj`和`pcfproj`项目的一对多关系如下图所示：

！[cdsproj和pcfproj项目的一对多关系]（https://learn.microsoft.com/en-us/power-apps/developer/component-framework/media/code-component-projects.png）

更多信息：[打包代码组件]（https://learn.microsoft.com/en-us/power-apps/developer/component-framework/import-custom-controls#package-a-code-component）。

构建pcfproj代码组件项目

在构建`pcfproj`项目时，生成的JavaScript依赖于用于构建的命令和`pcfproj`文件中的`PcfBuildMode`。您通常不会将在开发模式下构建的代码组件部署到Microsoft Dataverse中，因为它通常太大而无法导入，并且可能导致运行时性能降低。更多信息：[部署到Microsoft Dataverse后的调试]（https://learn.microsoft.com/en-us/power-apps/developer/component-framework/debugging-custom-controls#debugging-after-deploying-into-microsoft-dataverse）。

为了使`pac pcf push`产生一个发布版本，通过在`OutputPath`元素下添加一个新元素，`PcfBuildMode`被设置在`pcfproj`中：```xml
<PropertyGroup>
   <Name>my-control</Name>
   <ProjectGuid>6aaf0d27-ec8b-471e-9ed4-7b3bbc35bbab</ProjectGuid>
   <OutputPath>$(MSBuildThisFileDirectory)out\controls</OutputPath>
   <PcfBuildMode>production</PcfBuildMode>
</PropertyGroup>
```
###构建命令

|命令|默认行为|与PcfBuildMode=生产||---------|-----------------|------------------------------|
| npm start watch |总是开发| |
| pac pcf push |开发构建|发布构建|
| npm运行构建|开发构建|`npm run build -- --buildMode production`|

更多信息：[Package a code component]（https://learn.microsoft.com/en-us/power-apps/developer/component-framework/import-custom-controls#package-a-code-component）。

##建筑。cdsproj方案项目

在构建解决方案项目（`.cdsproj`）时，您可以选择将输出生成为托管或非托管解决方案。托管解决方案用于部署到任何不是该解决方案的开发环境的环境。这包括测试、UAT、SIT和生产环境。更多信息：[托管和非托管解决方案]（https://learn.microsoft.com/en-us/power-platform/alm/solution-concepts-alm#managed-and-unmanaged-solutions）。`SolutionPackagerType`包含在由`pac solution init`创建的`.cdsproj`文件中，但最初被注释掉了。取消该部分的注释，并将其设置为Managed、Unmanaged或两者都设置。```xml
<!-- Solution Packager overrides, un-comment to use: SolutionPackagerType (Managed, Unmanaged, Both) -->
<PropertyGroup>
   <SolutionPackageType>Managed</SolutionPackageType>
</PropertyGroup>
```
生成配置结果

|命令| SolutionPackageType |结果||---------|-------------------|---------|
| msbuild |托管|开发构建内部托管解决方案|
| msbuild /p:configuration=发布|托管|在托管解决方案|内发布构建
| msbuild |非托管|非托管解决方案|内的开发构建
| msbuild /p:configuration=Release | Unmanaged |在Unmanaged Solution |内发布build

更多信息：[Package a code component]（https://learn.microsoft.com/en-us/power-apps/developer/component-framework/import-custom-controls#package-a-code-component）。

使用代码组件进行源代码控制

在开发代码组件时，建议您使用源代码控制提供程序，如Azure DevOps或GitHub。当使用git源代码控制提交更改时，`pac pcf init`模板提供的`.gitignore`文件将确保一些文件不会被添加到源代码控制中，因为它们要么是由`npm`恢复的，要么是作为构建过程的一部分生成的。```
# dependencies
/node_modules

# generated directory
**/generated

# output directory
/out

# msbuild output directories
/bin
/obj
```
由于`/out`文件夹被排除在外，因此生成的`bundle.js`文件（以及相关资源）不会被添加到源代码控制中。当您的代码组件是手动构建或作为自动构建管道的一部分构建时，将使用最新的代码构建`bundle.js`，以确保包含所有更改。

此外，在构建解决方案时，任何关联解决方案zip文件都不会提交到源代码控制中。相反，输出将作为二进制发布工件发布。

在代码组件中使用SolutionPackager除了对`pcfproj`和`cdsproj`进行源代码控制之外，还可以使用[SolutionPackager]（https://learn.microsoft.com/en-us/power-platform/alm/solution-packager-tool）增量地将解决方案解压缩为其各自的部分，作为一系列XML文件，这些文件可以提交到源代码控制中。这样做的好处是可以以人类可读的格式创建元数据的完整图像，这样您就可以使用拉请求或类似的方法跟踪更改。

**注**：此时，SolutionPackager与`pac solution clone`的不同之处在于，它可以增量地用于从Dataverse解决方案导出更改。

示例解决方案结构

一旦包含代码组件的解决方案使用`SolutionPackager /action: Extract`解包，它看起来类似于：```
.
├── Controls
│   └── prefix_namespace.ControlName
│       ├── bundle.js *
│       └── css
│          └── ControlName.css *
│       ├── ControlManifest.xml *
│       └── ControlManifest.xml.data.xml
├── Entities
│   └── Contact
│       ├── FormXml
│       │   └── main
│       │       └── {3d60f361-84c5-eb11-bacc-000d3a9d0f1d}.xml
│       ├── Entity.xml
│       └── RibbonDiff.xml
└── Other
    ├── Customizations.xml
    └── Solution.xml
```
在`Controls`文件夹下，您可以看到解决方案中包含的每个代码组件都有子文件夹。在将此文件夹结构提交到源代码控件时，应该排除上面标有星号（*）的文件，因为在为相应组件构建`pcfproj`项目时将输出这些文件。

唯一需要的文件是`*.data.xml`文件，因为它们包含描述打包过程所需资源的元数据。

更多信息：[SolutionPackager命令行参数]（https://learn.microsoft.com/en-us/power-platform/alm/solution-packager-tool#solutionpackager-command-line-arguments）。

代码组件解决方案策略

使用Dataverse解决方案将代码组件部署到下游环境中。在解决方案中部署代码组件有两种策略：

# # # 1。分段的解决方案使用`pac solution init`创建解决方案项目，然后使用`pac solution add-reference`添加一个或多个代码组件。然后，这个解决方案可以导出并导入到下游环境中，而其他分段解决方案将依赖于代码组件解决方案，因此必须首先将其部署到该环境中。

**采用分段解决方法的原因：**

1. **版本控制生命周期** -您希望在独立的生命周期中开发、部署和版本控制代码组件，而不是解决方案的其他部分。这在“融合团队”场景中很常见，即开发者构建的代码组件被应用开发者使用。2. **共享使用**——您希望在多个环境之间共享代码组件，因此不希望将代码组件与任何其他解决方案组件耦合在一起。如果您是ISV或正在开发供组织的不同部分使用的代码组件，则可能会出现这种情况。

# # # 2。单一的解决方案

在Dataverse环境中创建单个解决方案，然后将代码组件与其他解决方案组件（如表、模型驱动的应用程序或画布应用程序）一起添加，这些组件依次引用这些代码组件。该解决方案可以导出并导入到下游环境中，而不需要任何解决方案之间的依赖关系。

解决方案生命周期概述

！(解决方案策略)(https://learn.microsoft.com/en-us/power-apps/developer/component-framework/media/solution-strategies.png)

更多信息：[使用解决方案打包和分发扩展]（https://learn.microsoft.com/en-us/powerapps/developer/data-platform/introduction-solutions）。

代码组件和自动构建管道除了手动构建和部署代码组件解决方案之外，您还可以使用自动构建管道构建和打包代码组件。

-如果你使用Azure DevOps，你可以使用[Microsoft Power Platform Build Tool for Azure DevOps]（https://learn.microsoft.com/en-us/power-platform/alm/devops-build-tools）。
-如果你使用GitHub，你可以使用[电源平台GitHub Actions]（https://learn.microsoft.com/en-us/power-platform/alm/devops-github-actions）。

自动化构建管道的优势

- **省时** -删除手动任务使构建和包装更快
- **可重复** -执行相同的每次，不依赖于团队成员
- **版本一致性** -相对于以前的版本进行自动版本控制
- **可维护** -构建所需的一切都包含在源代码控制中

版本控制和部署更新在部署和更新代码组件时，拥有一致的版本控制策略非常重要。常见的版本控制策略是[语义版本控制](https://semver.org/)，其格式为：`MAJOR.MINOR.PATCH`。

###更新补丁版本`ControlManifest.Input.xml`将代码组件版本存储在控件元素中：```xml
<control namespace="..." constructor="..." version="1.0.0" display-name-key="..." description-key="..." control-type="...">
```
在将更新部署到代码组件时，`ControlManifest.Input.xml`中的版本必须至少增加其PATCH（版本的最后一部分）才能检测到更改。

**版本更新命令：**```bash
# Advance the PATCH version by one
pac pcf version --strategy manifest

# Specify an exact PATCH value (e.g., in automated build pipeline)
pac pcf version --patchversion <PATCH VERSION>
```
何时增加MAJOR和MINOR版本

建议代码组件版本的MAJOR和MINOR版本与分发的Dataverse解决方案保持同步。

[Dataverse解决方案有四个部分](https://learn.microsoft.com/en-us/powerapps/maker/data-platform/update-solutions#understanding-version-numbers-for-updates):`MAJOR.MINOR.BUILD.REVISION`。

|代码组件|数据规避解决方案|注释||----------------|-------------------|--------|
| MAJOR | MAJOR |使用管道变量或最后提交的值|设置
| MINOR | MINOR |使用管道变量或最后提交的值|设置
| PATCH | BUILD | $BuildId) |
|—| $(Rev:r

## Canvas Apps ALM注意事项

在画布应用程序中使用代码组件与在模型驱动的应用程序中使用代码组件是不同的。代码组件必须通过在插入面板上选择**获取更多组件**来显式添加到应用程序中。一旦代码组件被添加到canvas应用程序中，它就会作为内容包含在应用程序定义中。要在部署后更新到新版本的代码组件（控件版本增加），应用开发者必须首先在Power Apps Studio中打开应用，并在“更新代码组件”对话框中出现提示时选择“** update **”。然后必须保存并发布应用程序，以便用户在播放应用程序时使用新版本。

！[更新代码组件]（https://learn.microsoft.com/en-us/power-apps/developer/component-framework/media/upgrade-code-component.png）

如果应用程序未更新或使用了**Skip**，则应用程序将继续使用旧版本的代码组件，即使它已被新版本覆盖，因此在环境中不存在。由于应用程序包含代码组件的副本，因此可以在不同的canvas应用程序中在单个环境中并排运行不同版本的代码组件。但是，你不能在同一个应用中同时运行不同版本的代码组件。

**注**：虽然，此时，你可以导入一个canvas应用，而不需要将匹配的代码组件部署到该环境中，但建议你始终确保应用已更新为使用最新版本的代码组件，并且该版本首先部署到该环境中，或者作为相同解决方案的一部分。

##相关文章

-[应用程序生命周期管理（ALM）与Microsoft Power平台]（https://learn.microsoft.com/en-us/power-platform/alm/overview-alm）
- [Power Apps组件框架API参考]（https://learn.microsoft.com/en-us/power-apps/developer/component-framework/reference/）
-[创建你的第一个组件]（https://learn.microsoft.com/en-us/power-apps/developer/component-framework/implementing-controls-using-typescript）
-[调试代码组件]（https://learn.microsoft.com/en-us/power-apps/developer/component-framework/debugging-custom-controls）