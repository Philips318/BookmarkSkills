---
description: 'React controls and platform libraries for PCF components'
applyTo: '**/*.{ts,tsx,js,json,xml,pcfproj,csproj}'
---
# React控件和平台库

当你使用React和平台库时，你使用的是与Power Apps平台相同的基础设施。这意味着你不再需要为每个控件单独打包React和Fluent库。所有控件共享一个公共库实例和版本，以提供无缝和一致的体验。

# #好处

通过重用现有的平台React和Fluent库，您可以期望：

- **减少控制包大小**
- **优化解决方案包装**
- **更快的运行时传输，脚本和控件渲染**
- **与Power Apps流畅设计系统进行设计和主题对齐**

> **注**：与GA发布，所有现有的虚拟控制将继续发挥作用。但是，它们应该使用最新的CLI版本（>=1.37）进行重建和部署，以方便未来平台React版本的升级。

# #先决条件与任何组件一样，您必须安装[Visual Studio Code]（https://code.visualstudio.com/Download）和[Microsoft Power Platform CLI]（https://learn.microsoft.com/en-us/power-apps/developer/data-platform/powerapps-cli#install-microsoft-power-platform-cli）。

b> **注**：如果您已经安装了Windows的Power Platform CLI，请通过`pac install latest`命令确保运行的是最新版本。Visual Studio Code的Power Platform Tools应该自动更新。

##创建React组件

注意：这些指令要求您以前已经创建过代码组件。如果没有，请参见[创建第一个组件]（https://learn.microsoft.com/en-us/power-apps/developer/component-framework/implementing-controls-using-typescript）。`pac pcf init`命令有一个新的`--framework`（`-fw`）参数。设置为“`react`”。

###命令参数

| |值||-----------|-------|
|——name | ReactSample |
|——namespace | SampleNamespace |
|——模板|字段|
|——框架|反应|
——run-npm-install | true（默认）

PowerShell命令

下面的PowerShell命令使用参数快捷键创建一个React组件项目并运行`npm-install`：```powershell
pac pcf init -n ReactSample -ns SampleNamespace -t field -fw react -npm
```
现在可以像往常一样使用`npm start`在测试工具中构建和查看控件。

在你构建控件之后，你可以将其打包到解决方案中，并将其用于模型驱动的应用程序（包括自定义页面）和画布应用程序（如标准代码组件）。

与标准组件的区别### ControlManifest.Input.xml
[控制元素](https://learn.microsoft.com/en-us/power-apps/developer/component-framework/manifest-schema-reference/control)`control-type`属性设置为`virtual`，而不是`standard`。

> **备注**：修改此值不会将组件从一种类型转换为另一种类型。

在[resources元素]（https://learn.microsoft.com/en-us/power-apps/developer/component-framework/manifest-schema-reference/resources）中，找到两个新的[platform-library element]（https://learn.microsoft.com/en-us/power-apps/developer/component-framework/manifest-schema-reference/platform-library）子元素：```xml
<resources>
  <code path="index.ts" order="1" />
  <platform-library name="React" version="16.14.0" />
  <platform-library name="Fluent" version="9.46.2" />
</resources>
```
> **注**：有关有效平台库版本的详细信息，请参见支持的平台库列表。

**建议**：我们建议使用Fluent 8和9的平台库。如果不使用Fluent，则应该删除`platform-library`元素，其中`name`属性值为`Fluent`。### Index.ts
(ReactControl。（https://learn.microsoft.com/en-us/power-apps/developer/component-framework/reference/react-control/init）方法的控件初始化没有`div`参数，因为React控件不直接渲染DOM。而不是[ReactControl。（https://learn.microsoft.com/en-us/power-apps/developer/component-framework/reference/react-control/updateview）返回一个ReactElement，其中包含React格式的实际控件的详细信息。### bundle.js
React和Fluent库不包含在包中，因为它们是共享的，因此bundle.js的大小更小。

##样本控制

示例中包括以下控件。它们的功能与标准版本相同，但性能更好，因为它们是虚拟控件。

|示例|描述|链路||--------|-------------|------|
| ChoicesPickerReact |标准的ChoicesPickerControl转换成一个React控件| ChoicesPickerReact样本|
| FacepileReact | ReactStandardControl转换为一个React Control | FacepileReact |

支持的平台库列表

平台库在构建和运行时对使用平台库功能的控件都是可用的。目前，平台提供了以下版本，是目前支持的最高版本。

|库|包|构建版本|运行版本||---------|---------|---------------|-----------------|
| React | 16.14.0 | 17.0.2（模型），16.14.0（画布）|
|流畅| @fluentui/react| 8.29.0 | 8.29.0 |
|流畅| @fluentui/react| 8.121.1 | 8.121.1 |
|流利| @fluentui/react-components| >=9.4.0 <=9.46.2 | 9.68.0 |

> **注**：应用程序可能在运行时加载平台库的更高兼容版本，但该版本可能不是可用的最新版本。分别支持Fluent 8和Fluent 9，但不能在同一个清单中指定它们。

# #常见问题解答

Q：我可以使用平台库将现有的标准控件转换为React控件吗？

答:不是。您必须使用新模板创建一个新控件，然后更新清单和index.ts方法。作为参考，请比较上述标准样品和反应样品。

问：我可以在Power Pages中使用React控件和平台库吗？答:不是。React控件和平台库目前只支持画布和模型驱动的应用程序。在Power Pages中，React控件不会根据其他字段的更改进行更新。

##相关文章

-[什么是代码组件？]] (https://learn.microsoft.com/en-us/power-apps/developer/component-framework/custom-controls-overview)
-为canvas应用程序编写组件代码
-[创建和构建代码组件]（https://learn.microsoft.com/en-us/power-apps/developer/component-framework/create-custom-controls-using-pcf）
-[学习Power Apps组件框架]（https://learn.microsoft.com/en-us/training/paths/use-power-apps-component-framework）
-[在Power Pages中使用代码组件]（https://learn.microsoft.com/en-us/power-apps/maker/portals/component-framework）