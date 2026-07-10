---
applyTo: '**'
---
# Dataverse SDK for Python -官方快速入门

本指令总结了Microsoft Learn关于Dataverse SDK for Python（预览版）的指导，并提供了可复制的代码片段。

# #先决条件
-数据环境与read/write- Python 3.10+
—PyPI的网络接入

# #安装```bash
pip install PowerPlatform-Dataverse-Client
```
# #连接```python
from azure.identity import InteractiveBrowserCredential
from PowerPlatform.Dataverse.client import DataverseClient
from PowerPlatform.Dataverse.core.config import DataverseConfig

cfg = DataverseConfig()  # defaults to language_code=1033
client = DataverseClient(
    base_url="https://<myorg>.crm.dynamics.com",
    credential=InteractiveBrowserCredential(),
    config=cfg,
)
```
—可选HTTP设置：`cfg.http_retries`、`cfg.http_backoff`、`cfg.http_timeout`。

## CRUD示例```python
# Create returns list[str] of GUIDs
account_id = client.create("account", {"name": "Acme, Inc.", "telephone1": "555-0100"})[0]

# Retrieve single
account = client.get("account", account_id)

# Update (returns None)
client.update("account", account_id, {"telephone1": "555-0199"})

# Delete
client.delete("account", account_id)
```
##批量操作```python
# Broadcast patch to many IDs
ids = client.create("account", [{"name": "Contoso"}, {"name": "Fabrikam"}])
client.update("account", ids, {"telephone1": "555-0200"})

# 1:1 list of patches
client.update("account", ids, [{"telephone1": "555-1200"}, {"telephone1": "555-1300"}])

# Bulk create
payloads = [{"name": "Contoso"}, {"name": "Fabrikam"}, {"name": "Northwind"}]
ids = client.create("account", payloads)
```
##文件上传```python
client.upload_file('account', record_id, 'sample_filecolumn', 'test.pdf')
client.upload_file('account', record_id, 'sample_filecolumn', 'test.pdf', mode='chunk', if_none_match=True)
```
##分页检索多个```python
pages = client.get(
    "account",
    select=["accountid", "name", "createdon"],
    orderby=["name asc"],
    top=10,
    page_size=3,
)
for page in pages:
    print(len(page), page[:2])
```
##表元数据快速入门```python
info = client.create_table("SampleItem", {
    "code": "string",
    "count": "int",
    "amount": "decimal",
    "when": "datetime",
    "active": "bool",
})
logical = info["entity_logical_name"]
rec_id = client.create(logical, {f"{logical}name": "Sample A"})[0]
client.delete(logical, rec_id)
client.delete_table("SampleItem")
```
# #引用
—入门：https://learn.microsoft.com/en-us/power-apps/developer/data-platform/sdk-python/get-started-处理数据：https://learn.microsoft.com/en-us/power-apps/developer/data-platform/sdk-python/work-data—SDKsource/examples:https://github.com/microsoft/PowerPlatform-DataverseClient-Python