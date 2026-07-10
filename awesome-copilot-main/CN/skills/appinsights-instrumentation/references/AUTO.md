#自动仪表应用

使用Azure Portal自动检测托管在Azure App Service中的web应用程序，无需更改任何代码。只有以下类型的应用程序可以被自动检测。参见[支持的环境和资源提供程序]（https://learn.microsoft.com/azure/azure-monitor/app/codeless-overview#supported-environments-languages-and-resource-providers）。

- ASP。. NET Core应用程序托管在Azure应用程序服务
-Node.js应用托管在Azure应用服务

构建一个url，将用户带到Azure门户中的应用服务应用程序的应用程序洞察刀片。```
https://portal.azure.com/#resource/subscriptions/{subscription_id}/resourceGroups/{resource_group_name}/providers/Microsoft.Web/sites/{app_service_name}/monitoringSettings
```
使用上下文或要求用户获取承载web应用程序的subscription_id、resource_group_name和app_service_name。