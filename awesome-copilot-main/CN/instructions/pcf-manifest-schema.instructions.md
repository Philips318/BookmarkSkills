---
description: 'Complete manifest schema reference for PCF components with all available XML elements'
applyTo: '**/*.xml'
---
# Manifest模式引用

清单文件（`ControlManifest.Input.xml`）是定义代码组件的元数据文档。此参考列出了所有可用的清单元素及其用途。

根元素

# # #清单

包含整个组件定义的根元素。

##核心元素

# # #代码

引用实现组件逻辑的资源文件。

* *属性:* *
—`path`:TypeScript/JavaScript实现文件的路径
-`order`：加载顺序（通常为“1”）

**可用性：**模型驱动的应用程序，画布应用程序，门户

# # #控制

定义组件本身，包括名称空间、版本和显示信息。* *关键属性:* *
—`namespace`：组件的命名空间
—`constructor`：构造函数名称
-`version`：语义版本（例如，“1.0.0”）
—`display-name-key`：显示名称的资源键
—`description-key`：用于描述的资源键
-`control-type`：控制类型（“标准”或“虚拟”）

**可用性：**模型驱动的应用程序，画布应用程序，门户

##属性元素

# # #财产

定义组件的输入或输出属性。

* *关键属性:* *
—`name`：属性名称
—`display-name-key`：显示名称的资源键
—`description-key`：用于描述的资源键
-`of-type`：数据类型（例如，“单行”）。文本”、“整体。None", "TwoOptions", “DateAndTime ”。DateOnly”)
-`usage`：属性使用（“bound”或“input”）
-`required`：是否需要属性（true/false）
—`of-type-group`：对类型组的引用
—`default-value`：该属性的默认值

**可用性：**模型驱动的应用程序，画布应用程序，门户# # #类型组

定义属性可以接受的一组类型。

用法：**允许一个属性接受多种数据类型

**可用性：**模型驱动的应用程序，画布应用程序，门户

##数据集元素

# # #先于

定义用于处理表格数据的数据集属性。

* *关键属性:* *
—`name`：数据集名称
—`display-name-key`：显示名称的资源键
—`description-key`：用于描述的资源键

**可用性：**模型驱动应用程序（有限制的画布应用程序）

##资源元素

# # #资源

所有资源定义（代码、CSS、图像、本地化）的容器。

**可用性：**模型驱动的应用程序，画布应用程序，门户

# # # css

引用CSS样式表文件。

* *属性:* *
—`path`: CSS文件路径
—`order`：加载顺序

**可用性：**模型驱动的应用程序，画布应用程序，门户

# # # img

引用图像资源。* *属性:* *
—`path`：镜像文件路径

**可用性：**模型驱动的应用程序，画布应用程序，门户

# # # resx

引用用于本地化的资源文件。

* *属性:* *
—`path`：路径。resx文件
—`version`：版本号

**可用性：**模型驱动的应用程序，画布应用程序，门户

特性使用元素

# # # uses-feature

声明组件使用特定的平台特性。

* *关键属性:* *
-`name`：功能名称（例如，“Device. captureimage”，“Device. captureimage”）。getCurrentPosition”、“实用程序。lookupObjects”、“WebAPI”)
-`required`：是否需要功能（true/false）

* *共同特点:* *
——Device.captureAudio
——Device.captureImage
——Device.captureVideo
——Device.getBarcodeValue
——Device.getCurrentPosition
——Device.pickFile
——Utility.lookupObjects
- - - - - -之前

**可用性：**因功能和平台而异

# # # feature-usage

用于特性声明的容器。**可用性：**模型驱动应用程序，画布应用程序

依赖元素

# # #的依赖

声明组件所需的外部依赖项。

**可用性：**模型驱动应用程序，画布应用程序

# # # external-service-usage

声明组件使用的外部服务。

* *关键属性:* *
—`enabled`：是否启用外部服务（true/false）

**可用性：**模型驱动应用程序，画布应用程序

##库元素

# # # platform-library

引用平台提供的库（如React、Fluent UI）。

* *关键属性:* *
-`name`：库名（例如，“React”，“Fluent”）
—`version`：库版本

**可用性：**模型驱动应用程序，画布应用程序

##事件元素

# # #事件

定义组件可以引发的自定义事件。

* *关键属性:* *
—`name`：事件名称
—`display-name-key`：显示名称的资源键
—`description-key`：用于描述的资源键**可用性：**模型驱动应用程序，画布应用程序

##动作元素

# # # platform-action

定义组件可以调用的平台操作。

**可用性：**模型驱动的应用

示例Manifest结构```xml
<?xml version="1.0" encoding="utf-8" ?>
<manifest>
  <control namespace="SampleNamespace" 
           constructor="SampleControl" 
           version="1.0.0" 
           display-name-key="Sample_Display_Key" 
           description-key="Sample_Desc_Key" 
           control-type="standard">
    
    <!-- Properties -->
    <property name="sampleProperty" 
              display-name-key="Property_Display_Key" 
              description-key="Property_Desc_Key" 
              of-type="SingleLine.Text" 
              usage="bound" 
              required="true" />
    
    <!-- Type Group Example -->
    <type-group name="numbers">
      <type>Whole.None</type>
      <type>Currency</type>
      <type>FP</type>
      <type>Decimal</type>
    </type-group>
    
    <property name="numericProperty"
              display-name-key="Numeric_Display_Key"
              of-type-group="numbers"
              usage="bound" />
    
    <!-- Data Set Example -->
    <data-set name="dataSetProperty" 
              display-name-key="Dataset_Display_Key">
    </data-set>
    
    <!-- Events -->
    <event name="onCustomEvent"
           display-name-key="Event_Display_Key"
           description-key="Event_Desc_Key" />
    
    <!-- Resources -->
    <resources>
      <code path="index.ts" order="1" />
      <css path="css/SampleControl.css" order="1" />
      <img path="img/icon.png" />
      <resx path="strings/SampleControl.1033.resx" version="1.0.0" />
    </resources>
    
    <!-- Feature Usage -->
    <feature-usage>
      <uses-feature name="WebAPI" required="true" />
      <uses-feature name="Device.captureImage" required="false" />
    </feature-usage>
    
    <!-- Platform Library -->
    <platform-library name="React" version="16.8.6" />
    <platform-library name="Fluent" version="8.29.0" />
    
  </control>
</manifest>
```
##舱单验证

清单架构在构建过程中被验证：
缺少必需的元素将导致构建错误
-无效的属性值将被标记
—使用`pac pcf`命令验证manifest结构

最佳实践1. **语义版本控制：对组件版本使用语义版本控制（major.minor.patch）
2. **本地化键**：始终使用资源键而不是硬编码字符串
3. **特性声明**：声明组件使用的所有特性
4. **必填项与可选项：只有在真正需要的时候才把属性和特性标记为必填项
5. **类型组**：对接受多个数字类型的属性使用类型组
6. **数据类型**：选择最具体的数据类型，以满足您的需求
7. **CSS作用域**：作用域CSS以避免与主机应用冲突
8. **资源组织**：将资源组织在单独的文件夹中（css/, img/, strings/）

##数据类型引用

属性常用的`of-type`值：—**文本**：单行。文本，多重，单行。文本区域,单行模式。电子邮件、单行模式。电话,单行模式。Url,单行模式。股票
—**数字**：整。无，货币，FP，十进制
- **Date/Time**：日期和时间。DateAndTime DateAndTime。DateOnly
—**布尔值**：两个选项
—**查找**：查找。简单的
—**OptionSet**: OptionSet, MultiSelectOptionSet
—**其他**:Enum

平台可用性图例

-✅**模型驱动的应用**：完全支持
-✅**Canvas应用**：支持（可能有限制）
-✅**门户**：支持在电源页面

大多数清单元素在所有平台上都可用，但有些功能（如某些设备api或平台操作）可能是平台特定的。始终在目标环境中进行测试。