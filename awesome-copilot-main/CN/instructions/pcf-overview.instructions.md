---
description: 'Power Apps Component Framework overview and fundamentals'
applyTo: '**/*.{ts,tsx,js,json,xml,pcfproj,csproj}'
---
# Power Apps组件框架概述

Power Apps组件框架使专业开发人员和应用程序制造商能够为模型驱动和画布应用程序创建代码组件。这些代码组件可用于增强用户在表单、视图、仪表板和画布应用程序屏幕上处理数据的用户体验。

##关键功能

你可以使用PCF：
—用`dial`或`slider`代码组件替换显示数字文本值的表单上的列
-将列表转换为与数据集绑定的完全不同的视觉体验，如`Calendar`或`Map`##重要限制

- Power Apps组件框架只能在统一接口上工作，不能在传统的web客户端上工作
- Power Apps组件框架目前不支持本地环境

PCF与Web资源的区别与HTML web资源不同，代码组件是：
-作为同一上下文的一部分呈现
-与任何其他组件同时加载
-为用户提供无缝的体验

代码组件可以是：
-在Power Apps的全部功能中使用
-在不同的表和表单之间多次重用
-将所有HTML， CSS和TypeScript文件捆绑到一个解决方案包中
-跨环境移动
-通过AppSource提供

##主要优势

富框架api
-组件生命周期管理
—上下文数据和元数据访问
-通过Web API无缝访问服务器
-实用程序和数据格式化方法
-设备特性：摄像头、定位、麦克风
-用户体验元素：对话框，查找，全页渲染发展效益
-支持现代web实践
-优化性能
-高可重用性
-将所有文件捆绑到单个解决方案文件中
-处理被销毁和重新加载的性能原因，同时保留状态

##许可要求

Power Apps组件框架许可基于所使用的数据和连接类型：

高级代码组件
通过用户的浏览器客户端（而不是通过连接器）直接连接到外部服务或数据的代码组件：
-考虑的高级组件
-应用程序使用这些成为高级
—最终用户需要Power Apps许可

通过在舱单中添加：```xml
<external-service-usage enabled="true">
  <domain>www.microsoft.com</domain>
</external-service-usage>
```
标准代码组件
编写不连接到外部服务或数据的组件：
-使用这些具有标准功能的应用程序仍然是标准的
—最终用户需要最少的Office 365 license

**注**：如果在连接到Microsoft Dataverse的模型驱动应用程序中使用代码组件，最终用户将需要Power apps许可。

##相关资源

-[什么是代码组件？]] (https://learn.microsoft.com/en-us/power-apps/developer/component-framework/custom-controls-overview)
-为canvas应用程序编写组件代码
-[创建和构建代码组件]（https://learn.microsoft.com/en-us/power-apps/developer/component-framework/create-custom-controls-using-pcf）
-[学习Power Apps组件框架]（https://learn.microsoft.com/en-us/training/paths/use-power-apps-component-framework）
-[在功能页中使用代码组件]（https://learn.microsoft.com/en-us/power-apps/maker/portals/component-framework）

培训资源

-[使用Power Apps组件框架创建组件-培训]（https://learn.microsoft.com/en-us/training/paths/create-components-power-apps-component-framework/）
- [Microsoft Certified: Power Platform Developer Associate]（https://learn.microsoft.com/en-us/credentials/certifications/power-platform-developer-associate/）