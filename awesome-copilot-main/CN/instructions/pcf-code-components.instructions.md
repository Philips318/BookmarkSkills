---
description: 'Understanding code components structure and implementation'
applyTo: '**/*.{ts,tsx,js,json,xml,pcfproj,csproj}'
---
#代码组件

代码组件是一种解决方案组件，可以包含在解决方案文件中并导入到不同的环境中。它们既可以添加到模型驱动的应用程序中，也可以添加到画布应用程序中。

三个核心元素

代码组件由三个元素组成：

1. * * * *
2. * * * *组件实现
3. * * * *的资源

**注**：使用Power Apps组件框架的代码组件的定义和实现对于模型驱动和画布驱动的应用程序是相同的。唯一的区别是配置部分。

# #清单

清单是定义组件的`ControlManifest.Input.xml`元数据文件。它是一个XML文档，描述：—组件名称
—可配置的数据类型：`field`和`dataset`—添加组件时可以在应用程序中配置的任何属性
-组件所需的资源文件列表

明确目标

当用户配置代码组件时，清单文件中的数据会筛选可用的组件，以便只有上下文的有效组件可用于配置。在清单文件中定义的属性呈现为配置列，以便用户可以指定值。然后，这些属性值在运行时对组件可用。

更多信息：[清单模式参考]（https://learn.microsoft.com/en-us/power-apps/developer/component-framework/manifest-schema-reference/）

##组件实现代码组件是用TypeScript实现的。每个代码组件必须包括实现代码组件接口中描述的方法的对象。[Power Platform CLI]（https://learn.microsoft.com/en-us/power-platform/developer/cli/introduction）使用`pac pcf init`命令自动生成带有存根实现的`index.ts`文件。

必需的方法

组件对象实现了这些生命周期方法：

- **init**（必选）-在页面加载时调用
- **updateView**（必选）-当应用数据发生变化时调用
- **getOutputs**（可选）—当用户修改数据时返回值
- **destroy**（必选）-在页面关闭时调用

组件生命周期

####页面加载

当页面加载时，应用程序使用清单中的数据创建一个对象：```typescript
var obj = new <"namespace on manifest">.<"constructor on manifest">();
```
例子:```typescript
var controlObj = new SampleNameSpace.LinearInputComponent();
```
然后，页面初始化组件：```typescript
controlObj.init(context, notifyOutputChanged, state, container);
```
* * Init参数:* *

| |参数说明||-----------|-------------|
|`context`|包含该组件的所有配置信息和参数信息。通过`context.parameters.<property name from manifest>`访问输入属性。包括Power Apps组件框架api。|
|`notifyOutputChanged`|当组件有可以异步检索的新输出时，通知框架。|
|`state`|如果使用`setControlState`方法显式存储，则包含来自前一个页面加载的组件数据。|
|`container`|一个HTML div元素，开发人员可以向其添加用于UI的HTML元素。|

####用户数据变更

当用户与组件交互以更改数据时，调用在`init`方法中传递的`notifyOutputChanged`方法。平台通过调用`getOutputs`方法进行响应，该方法返回用户所做更改的值。对于`field`组件，这通常是新值。

####应用更改数据如果平台更改了数据，它将调用组件的`updateView`方法，并将新的上下文对象作为参数传递。应该实现此方法来更新组件中显示的值。

####页面关闭

当用户导航离开页面时，代码组件将失去作用域，并清除为对象分配的所有内存。然而，一些方法（如事件处理程序）可能会根据浏览器实现而保留并消耗内存。

* *最佳实践:* *
—使用`setControlState`方法存储同一会话中下一次使用的信息
-实现`destroy`方法来删除清理代码，如页面关闭时的事件处理程序

# #资源清单文件中的资源节点指的是组件实现其可视化所需的资源。每个代码组件都必须有一个资源文件来构造其可视化。工具生成的`index.ts`文件是一个`code`资源。必须至少有一个代码资源。

额外资源

您可以在清单中定义其他资源文件：

- CSS文件
-图片网页资源
- Resx web资源进行本地化

更多信息：[资源元素]（https://learn.microsoft.com/en-us/power-apps/developer/component-framework/manifest-schema-reference/resources）

##相关资源

-[创建和构建代码组件]（https://learn.microsoft.com/en-us/power-apps/developer/component-framework/create-custom-controls-using-pcf）
-[学习如何使用解决方案打包和分发扩展]