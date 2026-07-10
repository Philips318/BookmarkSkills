---
name: appinsights-instrumentation
description: 'Instrument a webapp to send useful telemetry data to Azure App Insights'
---
# AppInsights仪表

此技能允许将web应用的遥测数据发送到Azure App Insights，以便更好地观察应用的健康状况。

何时使用此技能

当用户想要为他们的web应用启用遥测时，使用此技能。

# #先决条件

工作区中的应用程序必须是这些类型之一

- ASP。. NET Core应用程序托管在Azure中
-托管在Azure中的Node.js应用程序

# #指南

收集上下文信息找出用户试图添加遥测支持的应用程序的（编程语言、应用程序框架、托管）元组。这决定了如何检测应用程序。阅读源代码，做出有根据的猜测。与用户确认任何你不知道的事情。你必须始终询问用户应用程序托管在哪里（例如，在个人计算机上，在Azure应用服务中作为代码，在Azure应用服务中作为容器，在Azure容器应用程序中，等等）。

###如果可能，首选自动仪表

如果应用程序是c# ASP。. NET Core应用托管在Azure应用服务中，使用[AUTO guide]（references/AUTO.md）来帮助用户自动检测应用。

手动仪表

通过创建AppInsights资源手动检测应用，并更新应用的代码。

####创建AppInsights资源

使用以下适合环境的选项之一。-添加AppInsights到现有的二头肌模板。请参阅[examples/appinsights.bicep]（examples/appinsights.bicep）了解要添加的内容。如果工作区中有现有的Bicep模板文件，这是最好的选择。
—使用Azure CLI。请参阅[scripts/appinsights.ps1]（scripts/appinsights.ps1）了解要执行哪些Azure CLI命令来创建App Insights资源。

无论您选择哪种选项，都建议用户在有意义的资源组中创建App Insights资源，以便更轻松地管理资源。在Azure中包含托管应用程序的资源的同一资源组是一个很好的候选资源组。

####修改应用程序代码

—如果应用程序是ASP。. NET Core应用，参见[ASPNETCORE指南]（references/ASPNETCORE.md）了解如何修改c#代码。
—如果是Node.js类型的应用，请参考[NODEJS guide]（references/NODEJS.md）修改JavaScript/TypeScript代码。
—如果应用是Python应用，请参见[Python guide]（references/PYTHON.md）修改Python代码。