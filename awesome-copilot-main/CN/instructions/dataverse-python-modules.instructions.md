---
applyTo: '**'
---
# Dataverse SDK for Python -完整的模块参考

##包层次```
PowerPlatform.Dataverse
├── client
│   └── DataverseClient
├── core
│   ├── config (DataverseConfig)
│   └── errors (DataverseError, ValidationError, MetadataError, HttpError, SQLParseError)
├── data (OData operations, metadata, SQL, file upload)
├── extensions (placeholder for future extensions)
├── models (placeholder for data models and types)
└── utils (placeholder for utilities and adapters)
```
core.config模块

管理客户端连接和行为设置。

### DataverseConfig类

容器的语言，超时，重试。不可变的。```python
from PowerPlatform.Dataverse.core.config import DataverseConfig

cfg = DataverseConfig(
    language_code=1033,        # Default English (US)
    http_retries=None,         # Reserved for future
    http_backoff=None,         # Reserved for future
    http_timeout=None          # Reserved for future
)

# Or use default static builder
cfg_default = DataverseConfig.from_env()
```
* *关键属性:* *
-`language_code: int = 1033`-用于本地化标签和消息的LCID。
—`http_retries: int | None`—（保留）暂态错误重试次数上限。
-`http_backoff: float | None`-（保留）重试次数。
—`http_timeout: float | None`—（预留）请求超时时间，单位为秒。

# #核心。错误的模块

SDK操作的结构化异常层次结构。

### DataverseError （Base）

SDK错误的基本异常。```python
from PowerPlatform.Dataverse.core.errors import DataverseError

try:
    # SDK call
    pass
except DataverseError as e:
    print(f"Code: {e.code}")                # Error category
    print(f"Subcode: {e.subcode}")          # Specific error
    print(f"Message: {e.message}")          # Human-readable
    print(f"Status: {e.status_code}")       # HTTP status (if applicable)
    print(f"Transient: {e.is_transient}")   # Retry-worthy?
    details = e.to_dict()                  # Convert to dict
```
# # # ValidationError

数据操作过程中验证失败。```python
from PowerPlatform.Dataverse.core.errors import ValidationError
```
# # # MetadataErrorTable/column创建、删除、巡检失败。```python
from PowerPlatform.Dataverse.core.errors import MetadataError

try:
    client.create_table("MyTable", {...})
except MetadataError as e:
    print(f"Metadata issue: {e.message}")
```
# # # HttpError

Web API HTTP请求失败（4xx， 5xx等）。```python
from PowerPlatform.Dataverse.core.errors import HttpError

try:
    client.get("account", record_id)
except HttpError as e:
    print(f"HTTP {e.status_code}: {e.message}")
    print(f"Service error code: {e.service_error_code}")
    print(f"Correlation ID: {e.correlation_id}")
    print(f"Request ID: {e.request_id}")
    print(f"Retry-After: {e.retry_after} seconds")
    print(f"Transient (retry?): {e.is_transient}")  # 429, 503, 504
```
# # # SQLParseError

使用`query_sql()`时出现SQL查询语法错误。```python
from PowerPlatform.Dataverse.core.errors import SQLParseError

try:
    client.query_sql("INVALID SQL HERE")
except SQLParseError as e:
    print(f"SQL parse error: {e.message}")
```
##数据包

低级OData协议、元数据、SQL和文件操作（内部委托）。`data`包主要是内部的；`client`模块中的高级`DataverseClient`封装并暴露：
-通过OData进行CRUD操作
-元数据管理（create/update/delete表和列）
- SQL查询执行
-文件上传处理

用户通过`DataverseClient`方法与它们交互（例如，`create()`、`get()`、`update()`、`delete()`、`create_table()`、`query_sql()`、`upload_file()`）。

## extensions Package（占位符）

为将来的扩展点保留（例如，自定义适配器、中间件）。

目前空;对当前功能使用核心和客户端模块。

## models Package（占位符）

为将来的数据模型定义和类型定义保留。

目前是空的。数据结构返回为`dict`(OData)，并且是json序列化的。

utils包（占位符）为实用程序适配器和帮助程序保留。

目前是空的。在以后的版本中可能会添加辅助函数。

## client模块

主要面向用户的API。

DataverseClient类

所有数据规避操作的高级客户端。```python
from azure.identity import InteractiveBrowserCredential
from PowerPlatform.Dataverse.client import DataverseClient
from PowerPlatform.Dataverse.core.config import DataverseConfig

# Create credential
credential = InteractiveBrowserCredential()

# Optionally configure
cfg = DataverseConfig(language_code=1033)

# Create client
client = DataverseClient(
    base_url="https://org.crm.dynamics.com",
    credential=credential,
    config=cfg  # optional
)
```
#### CRUD方法

—`create(table_schema_name, records)`→`list[str]`—创建记录，返回guid。
-`get(table_schema_name, record_id=None, select, filter, orderby, top, expand, page_size)`→记录。
-`update(table_schema_name, ids, changes)`→`None`-更新记录。
-`delete(table_schema_name, ids, use_bulk_delete=True)`→`str | None`-删除记录。

####元数据方法

-`create_table(table_schema_name, columns, solution_unique_name, primary_column_schema_name)`→元数据字典。
-`create_columns(table_schema_name, columns)`→`list[str]`。
-`delete_columns(table_schema_name, columns)`→`list[str]`-`delete_table(table_schema_name)`→`None`-`get_table_info(table_schema_name)`→元数据字典或`None`。
-`list_tables()`→`list[str]`#### SQL &实用程序

-`query_sql(sql)`→`list[dict]`—执行只读SQL。
-`upload_file(table_schema_name, record_id, file_name_attribute, path, mode, mime_type, if_none_match)`→`None`-上传至文件列。
-`flush_cache(kind)`→`int`-清除SDK缓存（例如`"picklist"`）。

##导入摘要```python
# Main client
from PowerPlatform.Dataverse.client import DataverseClient

# Configuration
from PowerPlatform.Dataverse.core.config import DataverseConfig

# Errors
from PowerPlatform.Dataverse.core.errors import (
    DataverseError,
    ValidationError,
    MetadataError,
    HttpError,
    SQLParseError,
)
```
# #引用

—模块文档：https://learn.microsoft.com/en-us/python/api/powerplatform-dataverse-client/—Core:https://learn.microsoft.com/en-us/python/api/powerplatform-dataverse-client/powerplatform.dataverse.core—数据：https://learn.microsoft.com/en-us/python/api/powerplatform-dataverse-client/powerplatform.dataverse.data—扩展名：https://learn.microsoft.com/en-us/python/api/powerplatform-dataverse-client/powerplatform.dataverse.extensions—型号：https://learn.microsoft.com/en-us/python/api/powerplatform-dataverse-client/powerplatform.dataverse.models—Utils:https://learn.microsoft.com/en-us/python/api/powerplatform-dataverse-client/powerplatform.dataverse.utils—客户端：https://learn.microsoft.com/en-us/python/api/powerplatform-dataverse-client/powerplatform.dataverse.client