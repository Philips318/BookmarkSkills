修改代码

对应用程序进行这些必要的更改。

-安装客户端库```
npm install @azure/monitor-opentelemetry
```
-配置应用程序使用Azure MonitorNode.js应用程序通常有一个作为package.json中的“主”属性列出的条目文件。找到该文件并在其中应用这些更改。
—需要顶部的客户端库。`const { useAzureMonitor } = require("@azure/monitor-opentelemetry");`—调用setup方法。`useAzureMonitor();`注意：应该尽早调用setup方法，但必须在环境变量配置后调用，因为它需要来自环境变量的App Insights连接字符串。例如，如果应用程序使用dotenv来加载环境变量，那么setup方法应该在它之后而在其他任何方法之前调用。
注：由于我们修改了app的代码，所以需要部署后才能生效。

##配置App Insights连接字符串App Insights资源有一个连接字符串。添加连接字符串作为正在运行的应用程序的环境变量。您可以使用Azure CLI查询app Insights资源的连接字符串。请参阅[scripts/appinsights.ps1]了解要执行哪些Azure CLI命令来查询连接字符串。

获取连接字符串后，使用其值设置此环境变量。```
"APPLICATIONINSIGHTS_CONNECTION_STRING={your_application_insights_connection_string}"
```
如果应用程序有IaC模板，如Bicep或表示其云实例的terraform文件，则应将此环境变量添加到IaC模板中，以便在每次部署中应用。否则，请使用Azure CLI手动将环境变量应用于应用程序的云实例。请参阅要执行哪些Azure CLI命令来设置此环境变量。