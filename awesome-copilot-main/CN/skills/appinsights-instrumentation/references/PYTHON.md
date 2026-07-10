修改代码

对应用程序进行这些必要的更改。

-安装客户端库```
pip install azure-monitor-opentelemetry
```
-配置应用程序使用Azure Monitor
Python应用程序通过Python标准库中的logger类发送遥测信息。创建一个模块来配置和创建一个可以发送遥测数据的记录器。```python
import logging
from azure.monitor.opentelemetry import configure_azure_monitor

configure_azure_monitor(
    logger_name="<your_logger_namespace>"
)
logger = logging.getLogger("<your_logger_namespace>")
```
注：由于我们修改了app的代码，所以需要部署后才能生效。

##配置App Insights连接字符串

App Insights资源有一个连接字符串。添加连接字符串作为正在运行的应用程序的环境变量。您可以使用Azure CLI查询app Insights资源的连接字符串。请参阅[scripts/appinsights.ps1]了解要执行哪些Azure CLI命令来查询连接字符串。

获取连接字符串后，使用其值设置此环境变量。```
"APPLICATIONINSIGHTS_CONNECTION_STRING={your_application_insights_connection_string}"
```
如果应用程序有IaC模板，如Bicep或表示其云实例的terraform文件，则应将此环境变量添加到IaC模板中，以便在每次部署中应用。否则，请使用Azure CLI手动将环境变量应用于应用程序的云实例。请参阅要执行哪些Azure CLI命令来设置此环境变量。

##发送数据

创建一个配置为发送遥测的记录器。```python
logger = logging.getLogger("<your_logger_namespace>")
logger.setLevel(logging.INFO)
```
然后通过调用其日志方法发送遥测事件。```python
logger.info("info log")
```
