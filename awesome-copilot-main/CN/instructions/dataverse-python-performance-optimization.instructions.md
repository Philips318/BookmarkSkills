---
applyTo: '**'
---
# Dataverse SDK for Python - Performance & Optimization Guide

基于官方Microsoft Dataverse和Azure SDK性能指导。

# # 1。性能概述

Python的Dataverse SDK针对Python开发人员进行了优化，但在预览中有一些限制：
—**最小重试策略**：默认只重试网络错误
- **否deletemmultiple **：使用单个删除或更新状态代替
- **限制OData批处理**：不支持通用OData批处理
- **SQL限制**：没有join，限制WHERE/TOP/ORDERBY

变通方法和优化策略解决了这些限制。

---

# # 2。查询优化

###使用Select限制列```python
# ❌ SLOW - Retrieves all columns
accounts = client.get("account", top=100)

# ✅ FAST - Only retrieve needed columns
accounts = client.get(
    "account",
    select=["accountid", "name", "telephone1", "creditlimit"],
    top=100
)
```
**影响**：减少负载大小和内存使用30-50%。

---

###有效使用过滤器```python
# ❌ SLOW - Fetch all, filter in Python
all_accounts = client.get("account")
active_accounts = [a for a in all_accounts if a.get("statecode") == 0]

# ✅ FAST - Filter server-side
accounts = client.get(
    "account",
    filter="statecode eq 0",
    top=100
)
```
**OData过滤器示例**：```python
# Equals
filter="statecode eq 0"

# String contains
filter="contains(name, 'Acme')"

# Multiple conditions
filter="statecode eq 0 and createdon gt 2025-01-01Z"

# Not equals
filter="statecode ne 2"
```
---

可预测分页按顺序排序```python
# Ensure consistent order for pagination
accounts = client.get(
    "account",
    orderby=["createdon desc", "name asc"],
    page_size=100
)

for page in accounts:
    process_page(page)
```
---

# # 3。分页最佳实践

延迟分页（推荐）```python
# ✅ BEST - Generator yields one page at a time
pages = client.get(
    "account",
    top=5000,              # Total limit
    page_size=200          # Per-page size (hint)
)

for page in pages:  # Each iteration fetches one page
    for record in page:
        process_record(record)  # Process immediately
```
* * * *好处:
-内存效率高（按需加载页面）
-快速获得第一个结果
-如果需要，可以提前停止

避免将所有内容加载到内存中```python
# ❌ SLOW - Loads all 100,000 records at once
all_records = list(client.get("account", top=100000))
process(all_records)

# ✅ FAST - Process as you go
for page in client.get("account", top=100000, page_size=5000):
    process(page)
```
---

# # 4。批处理操作

批量创建（推荐）```python
# ✅ BEST - Single call with multiple records
payloads = [
    {"name": f"Account {i}", "telephone1": f"555-{i:04d}"}
    for i in range(1000)
]
ids = client.create("account", payloads)  # One API call for many records
```
###批量更新-广播模式```python
# ✅ FAST - Same update applied to many records
account_ids = ["id1", "id2", "id3", "..."]
client.update("account", account_ids, {"statecode": 1})  # One call
```
###批量更新-按记录模式```python
# ✅ ACCEPTABLE - Different updates for each record
account_ids = ["id1", "id2", "id3"]
updates = [
    {"telephone1": "555-0100"},
    {"telephone1": "555-0200"},
    {"telephone1": "555-0300"},
]
client.update("account", account_ids, updates)
```
批处理大小调优

基于表的复杂性（根据微软指南）：

|表类型|批大小|最大线程数||------------|-----------|-------------|
| OOB (Account, Contact, Lead) | 200-300 | 30 |
|简单（很少查找）|≤10| 50|
|中度复杂|≤100| 30|
|Large/complex（>100 cols, >20查找）| 10-20 | 10-20 |```python
def bulk_create_optimized(client, table_name, payloads, batch_size=200):
    """Create records in optimal batch size."""
    for i in range(0, len(payloads), batch_size):
        batch = payloads[i:i + batch_size]
        ids = client.create(table_name, batch)
        print(f"Created {len(ids)} records")
        yield ids
```
---

# # 5。连接管理

重用客户端实例```python
# ❌ BAD - Creates new connection each time
def process_batch():
    for batch in batches:
        client = DataverseClient(...)  # Expensive!
        client.create("account", batch)

# ✅ GOOD - Reuse connection
client = DataverseClient(...)  # Create once

def process_batch():
    for batch in batches:
        client.create("account", batch)  # Reuse
```
全局客户端实例```python
# singleton_client.py
from azure.identity import DefaultAzureCredential
from PowerPlatform.Dataverse.client import DataverseClient

_client = None

def get_client():
    global _client
    if _client is None:
        _client = DataverseClient(
            base_url="https://myorg.crm.dynamics.com",
            credential=DefaultAzureCredential()
        )
    return _client

# main.py
from singleton_client import get_client

client = get_client()
records = client.get("account")
```
###连接超时配置```python
from PowerPlatform.Dataverse.core.config import DataverseConfig

cfg = DataverseConfig()
cfg.http_timeout = 30         # Request timeout
cfg.connection_timeout = 5    # Connection timeout

client = DataverseClient(
    base_url="https://myorg.crm.dynamics.com",
    credential=credential,
    config=cfg
)
```
---

# # 6。异步操作（未来能力）

当前是同步的，但准备异步：```python
# Recommended pattern for future async support
import asyncio

async def get_accounts_async(client):
    """Pattern for future async SDK."""
    # When SDK supports async:
    # accounts = await client.get("account")
    # For now, use sync with executor
    loop = asyncio.get_event_loop()
    accounts = await loop.run_in_executor(
        None, 
        lambda: list(client.get("account"))
    )
    return accounts

# Usage
accounts = asyncio.run(get_accounts_async(client))
```
---

# # 7。文件上传优化

###小文件（<128 MB）```python
# ✅ FAST - Single request
client.upload_file(
    table_name="account",
    record_id=record_id,
    column_name="document_column",
    file_path="small_file.pdf"
)
```
###大文件（>128 MB）```python
# ✅ OPTIMIZED - Chunked upload
client.upload_file(
    table_name="account",
    record_id=record_id,
    column_name="document_column",
    file_path="large_file.pdf",
    mode='chunk',
    if_none_match=True
)

# SDK automatically:
# 1. Splits file into 4MB chunks
# 2. Uploads chunks in parallel
# 3. Assembles on server
```
---

# # 8。OData查询优化

### SQL Alternative（简单查询）```python
# ✅ SOMETIMES FASTER - Direct SQL for SELECT only
# Limited support: single SELECT, optional WHERE/TOP/ORDER BY
records = client.get(
    "account",
    sql="SELECT accountid, name FROM account WHERE statecode = 0 ORDER BY name"
)
```
复杂查询```python
# ❌ NOT SUPPORTED - JOINs, complex WHERE
sql="SELECT a.accountid, c.fullname FROM account a JOIN contact c ON a.accountid = c.parentcustomerid"

# ✅ WORKAROUND - Get accounts, then contacts for each
accounts = client.get("account", select=["accountid", "name"])
for account in accounts:
    contacts = client.get(
        "contact",
        filter=f"parentcustomerid eq '{account['accountid']}'"
    )
    process(account, contacts)
```
---

# # 9。内存管理

增量处理大型数据集```python
import gc

def process_large_table(client, table_name):
    """Process millions of records without memory issues."""
    
    for page in client.get(table_name, page_size=5000):
        for record in page:
            result = process_record(record)
            save_result(result)
        
        # Force garbage collection between pages
        gc.collect()
```
DataFrame与Chunking的集成```python
import pandas as pd

def load_to_dataframe_chunked(client, table_name, chunk_size=10000):
    """Load data to DataFrame in chunks."""
    
    dfs = []
    for page in client.get(table_name, page_size=1000):
        df_chunk = pd.DataFrame(page)
        dfs.append(df_chunk)
        
        # Combine when chunk threshold reached
        if len(dfs) >= chunk_size // 1000:
            df = pd.concat(dfs, ignore_index=True)
            process_chunk(df)
            dfs = []
    
    # Process remaining
    if dfs:
        df = pd.concat(dfs, ignore_index=True)
        process_chunk(df)
```
---

# # 10。限速处理

SDK具有最小的重试支持-手动实现：```python
import time
from PowerPlatform.Dataverse.core.errors import DataverseError

def call_with_backoff(func, max_retries=3):
    """Call function with exponential backoff for rate limits."""
    
    for attempt in range(max_retries):
        try:
            return func()
        except DataverseError as e:
            if e.status_code == 429:  # Too Many Requests
                if attempt < max_retries - 1:
                    wait_time = 2 ** attempt  # 1s, 2s, 4s
                    print(f"Rate limited. Waiting {wait_time}s...")
                    time.sleep(wait_time)
                else:
                    raise
            else:
                raise

# Usage
ids = call_with_backoff(
    lambda: client.create("account", payload)
)
```
---

# # 11。事务一致性（已知限制）

SDK没有事务性保证：```python
# ⚠️ If bulk operation partially fails, some records may be created

def create_with_consistency_check(client, table_name, payloads):
    """Create records and verify all succeeded."""
    
    try:
        ids = client.create(table_name, payloads)
        
        # Verify all records created
        created = client.get(
            table_name,
            filter=f"isof(Microsoft.Dynamics.CRM.{table_name})"
        )
        
        if len(ids) != count_created:
            print(f"⚠️ Only {count_created}/{len(ids)} records created")
            # Handle partial failure
    except Exception as e:
        print(f"Creation failed: {e}")
        # Check what was created
```
---

# # 12。监控性能

###日志操作时长```python
import time
import logging

logger = logging.getLogger("dataverse")

def monitored_operation(operation_name):
    """Decorator to monitor operation performance."""
    def decorator(func):
        def wrapper(*args, **kwargs):
            start = time.time()
            try:
                result = func(*args, **kwargs)
                duration = time.time() - start
                logger.info(f"{operation_name}: {duration:.2f}s")
                return result
            except Exception as e:
                duration = time.time() - start
                logger.error(f"{operation_name} failed after {duration:.2f}s: {e}")
                raise
        return wrapper
    return decorator

@monitored_operation("Bulk Create Accounts")
def create_accounts(client, payloads):
    return client.create("account", payloads)
```
---

# # 13。性能检查表

|项目|状态|备注||------|--------|-------|
|重用客户端实例| 8.8.|创建一次，重用|
|使用select限制列| 8.8.|只检索需要的数据|
|用OData过滤服务器端| 8.8.|不取全部，过滤|
|使用page_size为|的分页
|批量操作| 8.8.|多个|使用create/update|按表类型调整批大小| 8.8.| OOB=200-300, Simple=≤10 |
|处理速率限制（429）| 8.8.|实现指数回退|
|对大文件使用分块上传| 8.8.| SDK句柄>128MB |
|监控操作持续时间| 8.8.|分析日志定时|
|使用类似生产的数据进行测试| 8.8.|性能随数据量|而变化

---

# # 14。看到也

- [Dataverse Web API性能]（https://learn.microsoft.com/en-us/power-apps/developer/data-platform/optimize-performance-create-update）
- [OData查询选项]（https://learn.microsoft.com/en-us/power-apps/developer/data-platform/webapi/query-data-web-api）
- [SDK处理数据]（https://learn.microsoft.com/en-us/power-apps/developer/data-platform/sdk-python/work-data）