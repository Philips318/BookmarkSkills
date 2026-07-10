---
description: 'Using dependent libraries in PCF components'
applyTo: '**/*.{ts,tsx,js,json,xml,pcfproj,csproj}'
---
#依赖库（预览）

[本主题是预发布文档，可能会有变化。]

使用模型驱动的应用程序，你可以重用包含在另一个组件中的预构建库，该组件被加载为多个组件的依赖项。

在多个控件中拥有预构建库的副本是不可取的。重用现有库可以减少使用库的所有组件的加载时间，从而提高性能，特别是当库很大时。库重用还有助于减少构建过程中的维护开销。

##前后

**之前**：每个PCF组件中包含的自定义库文件
！[显示每个pcf组件中包含的自定义库文件的图表]

**后**：从库控件调用共享函数的组件
！[显示组件从库控件调用共享函数的图表]##实现步骤

要使用依赖库，需要：

1. 创建一个包含库的Library组件。该组件可以提供某些功能，也可以只是库的容器。
2. 配置另一个组件，使其依赖于库组件加载的库。

默认情况下，库在依赖组件加载时加载，但您可以将其配置为按需加载。

这样，您就可以在library Control中独立地维护库，而依赖的控件不需要捆绑库的副本。

##如何工作

您需要将配置数据添加到组件项目中，以便构建过程以您希望的方式部署库。通过添加或编辑以下文件设置该配置数据：- **featureconfig.json**
- **webpack.config.js**
-将manifest模式编辑为**Register dependencies**### featureconfig.json
添加此文件以覆盖组件的默认特性标志，而无需修改`node_modules`文件夹中生成的文件。

* *特性标志:* *

|标志位|描述||------|-------------|
|`pcfResourceDependency`|允许组件使用库资源。|
|`pcfAllowCustomWebpack`|允许组件使用自定义web包。必须为定义库资源的组件启用此功能。|

默认情况下，这些值是`off`。将它们设置为`on`以覆盖默认值。

* *示例1:* *```json
{ 
  "pcfAllowCustomWebpack": "on" 
} 
```
* *示例2:* *```json
{ 
   "pcfResourceDependency": "on",
   "pcfAllowCustomWebpack": "off" 
} 
```

### webpack.config.js
组件的构建过程使用[Webpack]（https://webpack.js.org/）将代码和依赖捆绑到一个可部署的资产中。要将库排除在此捆绑之外，请将`webpack.config.js`文件添加到项目根文件夹中，该文件将库的别名指定为`externals`。[详细了解Webpack外部配置选项]（https://webpack.js.org/configuration/externals/）

当库别名为`myLib`时，该文件可能如下所示：```javascript
/* eslint-disable */ 
"use strict"; 

module.exports = { 
  externals: { 
    "myLib": "myLib" 
  }, 
}  
```
注册依赖项

使用清单模式的[resources]（https://learn.microsoft.com/en-us/power-apps/developer/component-framework/manifest-schema-reference/resources）中的[dependency元素]（https://learn.microsoft.com/en-us/power-apps/developer/component-framework/manifest-schema-reference/dependency）。```xml
<resources>
  <dependency
    type="control"
    name="samples_SampleNS.SampleStubLibraryPCF"
    order="1"
  />
  <code path="index.ts" order="2" />
</resources>
```
依赖作为组件的按需加载

您可以根据需要加载依赖库，而不是在加载组件时加载依赖库。按需加载为更复杂的控件提供了只在需要时加载依赖项的灵活性，特别是在依赖库很大的情况下。

！图中显示了库中函数的使用，其中库是按需加载的

要启用按需加载，您需要：

**步骤1**：将这些[platform-action element](https://learn.microsoft.com/en-us/power-apps/developer/component-framework/manifest-schema-reference/platform-action), [feature-usage element]（https://learn.microsoft.com/en-us/power-apps/developer/component-framework/manifest-schema-reference/feature-usage）和[uses-feature element]（https://learn.microsoft.com/en-us/power-apps/developer/component-framework/manifest-schema-reference/uses-feature）子元素添加到[control element](https://learn.microsoft.com/en-us/power-apps/developer/component-framework/manifest-schema-reference/control)：```xml
<platform-action action-type="afterPageLoad" />
<feature-usage>
   <uses-feature name="Utility"
      required="true" />
</feature-usage>
```
**步骤2**：设置[依赖元素]（https://learn.microsoft.com/en-us/power-apps/developer/component-framework/manifest-schema-reference/dependency）的`load-type`属性为`onDemand`。```xml
<dependency type="control"
      name="samples_SampleNamespace.StubLibrary"
      load-type="onDemand" />
```
##下一步

尝试一个指导您创建依赖库的教程：

[教程：在组件中使用依赖库]