---
description: 'Define and handle custom events in PCF components'
applyTo: '**/*.{ts,tsx,js,json,xml,pcfproj,csproj}'
---
#定义事件（预览）

[本主题是预发布文档，可能会有变化。]

在使用Power Apps组件框架构建自定义组件时，一个常见的需求是能够对控件内生成的事件做出反应。这些事件既可以通过用户交互调用，也可以通过代码以编程方式调用。例如，应用程序可以有一个允许用户构建产品包的代码组件。该组件还可以引发一个事件，该事件可以在应用程序的另一个区域显示产品信息。

组件数据流

代码组件的常见数据流是作为输入从托管应用程序流到控件的数据，以及从控件流到托管表单或页面的更新数据。该图显示了典型PCF组件的标准数据流模式：！显示从代码组件到绑定字段的数据更新触发了OnChange事件。

从代码组件到绑定字段的数据更新将触发`OnChange`事件。对于大多数组件场景，这就足够了，制造商只需添加一个处理程序来触发后续操作。但是，更复杂的控件可能需要引发非字段更新的事件。事件机制允许代码组件定义具有单独事件处理程序的事件。

##使用事件

PCF中的事件机制基于JavaScript中的标准事件模型。组件可以在清单文件中定义事件，并在代码中引发这些事件。托管应用程序可以监听这些事件并对它们作出反应。

###在Manifest中定义事件组件使用manifest文件中的[event元素]（https://learn.microsoft.com/en-us/power-apps/developer/component-framework/manifest-schema-reference/event）定义事件。这些数据允许各自的托管应用程序以不同的方式对事件作出反应。```xml
<property
  name="sampleProperty"
  display-name-key="Property_Display_Key"
  description-key="Property_Desc_Key"
  of-type="SingleLine.Text"
  usage="bound"
  required="true"
/>
<event
  name="customEvent1"
  display-name-key="customEvent1"
  description-key="customEvent1"
/>
<event
  name="customEvent2"
  display-name-key="customEvent2"
  description-key="customEvent2"
/>
```
Canvas Apps事件处理

Canvas应用程序使用Power Fx表达式对事件做出反应：

！[显示自定义事件在画布应用程序设计器]（https://learn.microsoft.com/en-us/power-apps/developer/component-framework/media/custom-events-in-canvas-designer.png）

模型驱动的应用程序事件处理

模型驱动应用程序使用[addEventHandler方法]（https://learn.microsoft.com/en-us/power-apps/developer/model-driven-apps/clientapi/reference/controls/addeventhandler）将事件处理程序与组件的自定义事件关联起来。```javascript
const controlName1 = "cr116_personid";

this.onLoad = function (executionContext) {
  const formContext = executionContext.getFormContext();

  const sampleControl1 = formContext.getControl(controlName1);
  sampleControl1.addEventHandler("customEvent1", this.onSampleControl1CustomEvent1);
  sampleControl1.addEventHandler("customEvent2", this.onSampleControl1CustomEvent2);
}
```
**注**：这些事件分别发生在应用中代码组件的每个实例中。

为模型驱动的应用定义事件

对于模型驱动的应用，你可以传递一个带有事件的有效负载，允许更复杂的场景。例如，在下面的图表中，组件在事件中传递一个回调函数，允许脚本处理回调组件。

！[在这个例子中，组件在事件中传递一个回调函数，允许脚本处理回调到组件]```javascript
this.onSampleControl1CustomEvent1 = function (params) {
   //alert(`SampleControl1 Custom Event 1: ${params}`);
   alert(`SampleControl1 Custom Event 1`);
}.bind(this);

this.onSampleControl2CustomEvent2 = function (params) {
  alert(`SampleControl2 Custom Event 2: ${params.message}`);
  // prevent the default action for the event
  params.callBackFunction();
}
```
为Canvas应用程序定义一个事件

制作者在属性窗格中的PCF控件上使用Power Fx配置事件。

##调用事件

查看如何在[Events]（https://learn.microsoft.com/en-us/power-apps/developer/component-framework/reference/events）中调用事件。

##下一步

[教程：在组件中定义自定义事件]