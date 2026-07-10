修改代码

对应用程序进行这些必要的更改。

-安装客户端库```
dotnet add package Azure.Monitor.OpenTelemetry.AspNetCore
```
-配置应用程序使用Azure Monitor
一个ASP。. NET Core应用程序通常有一个Program.cs文件来“构建”应用程序。找到这个文件并应用这些更改。
-在顶部添加`using Azure.Monitor.OpenTelemetry.AspNetCore;`—在拨打`builder.Build()`之前，请添加该线路`builder.Services.AddOpenTelemetry().UseAzureMonitor();`。

注：由于我们修改了app的代码，所以需要部署app才能生效。

##配置App Insights连接字符串

App Insights资源有一个连接字符串。添加连接字符串作为正在运行的应用程序的环境变量。您可以使用Azure CLI查询app Insights资源的连接字符串。请参阅[scripts/appinsights.ps1]（scripts/appinsights.ps1）了解要执行哪些Azure CLI命令来查询连接字符串。

获取连接字符串后，使用其值设置此环境变量。```
"APPLICATIONINSIGHTS_CONNECTION_STRING={your_application_insights_connection_string}"
```
如果应用程序有IaC模板，如Bicep或表示其云实例的terraform文件，则应将此环境变量添加到IaC模板中，以便在每次部署中应用。否则，请使用Azure CLI手动将环境变量应用于应用程序的云实例。请参阅[scripts/appinsights.ps1]（scripts/appinsights.ps1）了解设置该环境变量需要执行的Azure CLI命令。

重要：不要修改appsettings.json。这是一种过时的配置App Insights的方式。环境变量是新的推荐方法。