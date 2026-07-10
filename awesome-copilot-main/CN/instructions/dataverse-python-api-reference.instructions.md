---
applyTo: '**'
---
# Dataverse SDK for Python - API参考指南

DataverseClient类
与Dataverse交互的主要客户端。使用基本URL和Azure凭证进行初始化。

关键方法

#### create（table_schema_name, records）
创建单个或批量记录。返回guid列表。```python
# Single record
ids = client.create("account", {"name": "Acme"})
print(ids[0])  # First GUID

# Bulk create
ids = client.create("account", [{"name": "Contoso"}, {"name": "Fabrikam"}])
```
#### get（table_schema_name, record_id=None, select, filter, order, top, expand, page_size）
使用OData选项获取单个记录或查询多个记录。```python
# Single record
record = client.get("account", record_id="guid-here")

# Query with filter and paging
for batch in client.get(
    "account",
    filter="statecode eq 0",
    select=["name", "telephone1"],
    orderby=["createdon desc"],
    top=100,
    page_size=50
):
    for record in batch:
        print(record["name"])
```
#### update（table_schema_name, ids, changes）
更新单个或批量记录。```python
# Single update
client.update("account", "guid-here", {"telephone1": "555-0100"})

# Broadcast: apply same changes to many IDs
client.update("account", [id1, id2, id3], {"statecode": 1})

# Paired: one-to-one mapping
client.update("account", [id1, id2], [{"name": "A"}, {"name": "B"}])
```
#### delete（table_schema_name, ids, use_bulk_delete=True）
删除单个或批量记录。```python
# Single delete
client.delete("account", "guid-here")

# Bulk delete (async)
job_id = client.delete("account", [id1, id2, id3])
```
#### create_table（table_schema_name, columns, solution_unique_name=None, primary_column_schema_name=None）
创建自定义表。```python
from enum import IntEnum

class ItemStatus(IntEnum):
    ACTIVE = 1
    INACTIVE = 2
    __labels__ = {
        1033: {"ACTIVE": "Active", "INACTIVE": "Inactive"}
    }

info = client.create_table("new_MyTable", {
    "new_Title": "string",
    "new_Quantity": "int",
    "new_Price": "decimal",
    "new_Active": "bool",
    "new_Status": ItemStatus
})
print(info["entity_logical_name"])
```
#### create_columns（table_schema_name, columns）
向现有表添加列。```python
created = client.create_columns("new_MyTable", {
    "new_Notes": "string",
    "new_Count": "int"
})
```
#### delete_columns（table_schema_name, columns）
从表中删除列。```python
removed = client.delete_columns("new_MyTable", ["new_Notes", "new_Count"])
```
# # # # delete_table (table_schema_name)
删除自定义表（不可逆）。```python
client.delete_table("new_MyTable")
```
# # # # get_table_info (table_schema_name)
检索表元数据。```python
info = client.get_table_info("new_MyTable")
if info:
    print(info["table_logical_name"])
    print(info["entity_set_name"])
```
# # # # list_tables ()
列出所有自定义表。```python
tables = client.list_tables()
for table in tables:
    print(table)
```
# # # # flush_cache(类)
清除SDK缓存（例如，picklist标签）。```python
removed = client.flush_cache("picklist")
```
## DataverseConfig Class
配置客户端行为（超时、重试、语言）。```python
from PowerPlatform.Dataverse.core.config import DataverseConfig

cfg = DataverseConfig()
cfg.http_retries = 3
cfg.http_backoff = 1.0
cfg.http_timeout = 30
cfg.language_code = 1033  # English

client = DataverseClient(base_url=url, credential=cred, config=cfg)
```
##错误处理
捕获特定于sdk的异常`DataverseError`。检查`is_transient`以决定重试。```python
from PowerPlatform.Dataverse.core.errors import DataverseError

try:
    client.create("account", {"name": "Test"})
except DataverseError as e:
    print(f"Code: {e.code}")
    print(f"Message: {e.message}")
    print(f"Transient: {e.is_transient}")
    print(f"Details: {e.to_dict()}")
```
##数据过滤器提示
—在过滤表达式中使用精确的逻辑名称（小写）
—`select`中的列名会自动小写
—“`expand`”中的导航属性名称区分大小写

# #引用
—API文档：https://learn.microsoft.com/en-us/python/api/powerplatform-dataverse-client/powerplatform.dataverse.client.dataverseclient—配置文档：https://learn.microsoft.com/en-us/python/api/powerplatform-dataverse-client/powerplatform.dataverse.core.config.dataverseconfig—错误码：https://learn.microsoft.com/en-us/python/api/powerplatform-dataverse-client/powerplatform.dataverse.core.errors