---
applyTo: '**'
---
# Dataverse SDK for Python -错误处理和故障排除指南

基于Azure SDK错误处理模式和Dataverse SDK细节的微软官方文档。

# # 1。DataverseError类概述

用于Python的Dataverse SDK为健壮的错误处理提供了结构化的异常层次结构。

DataverseError构造函数```python
from PowerPlatform.Dataverse.core.errors import DataverseError

DataverseError(
    message: str,                          # Human-readable error message
    code: str,                             # Error category (e.g., "validation_error", "http_error")
    subcode: str | None = None,            # Optional specific error identifier
    status_code: int | None = None,        # HTTP status code (if applicable)
    details: Dict[str, Any] | None = None, # Additional diagnostic information
    source: str | None = None,             # Error source: "client" or "server"
    is_transient: bool = False             # Whether error may succeed on retry
)
```
关键属性```python
try:
    client.get("account", record_id="invalid-id")
except DataverseError as e:
    print(f"Message: {e.message}")           # Human-readable message
    print(f"Code: {e.code}")                 # Error category
    print(f"Subcode: {e.subcode}")           # Specific error type
    print(f"Status Code: {e.status_code}")   # HTTP status (401, 403, 429, etc.)
    print(f"Source: {e.source}")             # "client" or "server"
    print(f"Is Transient: {e.is_transient}") # Can retry?
    print(f"Details: {e.details}")           # Additional context
    
    # Convert to dictionary for logging
    error_dict = e.to_dict()
```
---

# # 2。常见错误操作场景

认证错误（401）

**原因**：无效的凭据、过期的令牌或错误配置的设置。```python
from PowerPlatform.Dataverse.client import DataverseClient
from PowerPlatform.Dataverse.core.errors import DataverseError
from azure.identity import InteractiveBrowserCredential

try:
    # Bad credentials or expired token
    credential = InteractiveBrowserCredential()
    client = DataverseClient(
        base_url="https://invalid-org.crm.dynamics.com",
        credential=credential
    )
    records = client.get("account")
except DataverseError as e:
    if e.status_code == 401:
        print("Authentication failed. Check credentials and token expiration.")
        print(f"Details: {e.message}")
        # Don't retry - fix credentials first
    else:
        raise
```
###授权错误（403）

**原因**：用户缺乏所请求操作的权限。```python
try:
    # User doesn't have permission to read contacts
    records = client.get("contact")
except DataverseError as e:
    if e.status_code == 403:
        print("Access denied. User lacks required permissions.")
        print(f"Request ID for support: {e.details.get('request_id')}")
        # Escalate to administrator
    else:
        raise
```
###资源未找到（404）

**原因**：记录、表或资源不存在。```python
try:
    # Record doesn't exist
    record = client.get("account", record_id="00000000-0000-0000-0000-000000000000")
except DataverseError as e:
    if e.status_code == 404:
        print("Resource not found. Using default data.")
        record = {"name": "Unknown", "id": None}
    else:
        raise
```
###速率限制（429）

**原因**：请求数超过服务保护限制。

**注**:SDK具有最小的内置重试支持。手动处理瞬态一致性问题。```python
import time

def create_with_retry(client, table_name, payload, max_retries=3):
    """Create record with retry logic for rate limiting."""
    for attempt in range(max_retries):
        try:
            result = client.create(table_name, payload)
            return result
        except DataverseError as e:
            if e.status_code == 429 and e.is_transient:
                wait_time = 2 ** attempt  # Exponential backoff
                print(f"Rate limited. Retrying in {wait_time}s...")
                time.sleep(wait_time)
            else:
                raise
    
    raise Exception(f"Failed after {max_retries} retries")
```
###服务器错误（500,502,503,504）

**原因**：临时服务问题或基础设施问题。```python
try:
    result = client.create("account", {"name": "Acme"})
except DataverseError as e:
    if 500 <= e.status_code < 600:
        print(f"Server error ({e.status_code}). Service may be temporarily unavailable.")
        # Implement retry logic with exponential backoff
    else:
        raise
```
验证错误（400）

**原因**：请求格式无效，缺少必需字段，或违反业务规则。```python
try:
    # Missing required field or invalid data
    client.create("account", {"telephone1": "not-a-phone-number"})
except DataverseError as e:
    if e.status_code == 400:
        print(f"Validation error: {e.message}")
        if e.details:
            print(f"Details: {e.details}")
        # Log validation issues for debugging
    else:
        raise
```
---

# # 3。错误处理最佳实践

使用特定的异常处理

总是在常规异常之前捕获特定异常：```python
from PowerPlatform.Dataverse.core.errors import DataverseError
from azure.core.exceptions import AzureError

try:
    records = client.get("account", filter="statecode eq 0", top=100)
except DataverseError as e:
    # Handle Dataverse-specific errors
    if e.status_code == 401:
        print("Re-authenticate required")
    elif e.status_code == 404:
        print("Resource not found")
    elif e.is_transient:
        print("Transient error - may retry")
    else:
        print(f"Operation failed: {e.message}")
except AzureError as e:
    # Handle Azure SDK errors (network, auth, etc.)
    print(f"Azure error: {e}")
except Exception as e:
    # Catch-all for unexpected errors
    print(f"Unexpected error: {e}")
```
###实现智能重试逻辑

**不要重试**：
- 401未授权（认证失败）
- 403禁止（授权失败）
400个错误请求（客户端错误）
404未找到（除非资源最终应该出现）

**考虑重新尝试**：
- 408请求超时
- 429请求太多（带指数回退）
- 500服务器内部错误
- 502坏网关
- 503服务不可用
- 504网关超时时间```python
def should_retry(error: DataverseError) -> bool:
    """Determine if operation should be retried."""
    if not error.is_transient:
        return False
    
    retryable_codes = {408, 429, 500, 502, 503, 504}
    return error.status_code in retryable_codes

def call_with_exponential_backoff(func, *args, max_attempts=3, **kwargs):
    """Call function with exponential backoff retry."""
    for attempt in range(max_attempts):
        try:
            return func(*args, **kwargs)
        except DataverseError as e:
            if should_retry(e) and attempt < max_attempts - 1:
                wait_time = 2 ** attempt  # 1s, 2s, 4s...
                print(f"Attempt {attempt + 1} failed. Retrying in {wait_time}s...")
                time.sleep(wait_time)
            else:
                raise
```
提取有意义的错误信息```python
import json
from datetime import datetime

def log_error_for_support(error: DataverseError):
    """Log error with diagnostic information."""
    error_info = {
        "timestamp": datetime.utcnow().isoformat(),
        "error_type": type(error).__name__,
        "message": error.message,
        "code": error.code,
        "subcode": error.subcode,
        "status_code": error.status_code,
        "source": error.source,
        "is_transient": error.is_transient,
        "details": error.details
    }
    
    print(json.dumps(error_info, indent=2))
    
    # Save to log file or send to monitoring service
    return error_info
```
###优雅地处理批量操作```python
def bulk_create_with_error_tracking(client, table_name, payloads):
    """Create multiple records, tracking which succeed/fail."""
    results = {
        "succeeded": [],
        "failed": []
    }
    
    for idx, payload in enumerate(payloads):
        try:
            record_ids = client.create(table_name, payload)
            results["succeeded"].append({
                "payload": payload,
                "ids": record_ids
            })
        except DataverseError as e:
            results["failed"].append({
                "index": idx,
                "payload": payload,
                "error": {
                    "message": e.message,
                    "code": e.code,
                    "status": e.status_code
                }
            })
    
    return results
```
---

# # 4。启用诊断日志

配置日志```python
import logging
import sys

# Set up root logger
logging.basicConfig(
    level=logging.DEBUG,
    format='%(asctime)s - %(name)s - %(levelname)s - %(message)s',
    handlers=[
        logging.FileHandler('dataverse_sdk.log'),
        logging.StreamHandler(sys.stdout)
    ]
)

# Configure specific loggers
logging.getLogger('azure').setLevel(logging.DEBUG)
logging.getLogger('PowerPlatform').setLevel(logging.DEBUG)

# HTTP logging (careful with sensitive data)
logging.getLogger('azure.core.pipeline.policies.http_logging_policy').setLevel(logging.DEBUG)
```
###启用sdk级别日志```python
from PowerPlatform.Dataverse.client import DataverseClient
from PowerPlatform.Dataverse.core.config import DataverseConfig
from azure.identity import InteractiveBrowserCredential

cfg = DataverseConfig()
cfg.logging_enable = True  # Enable detailed logging

client = DataverseClient(
    base_url="https://myorg.crm.dynamics.com",
    credential=InteractiveBrowserCredential(),
    config=cfg
)

# Now SDK will log detailed HTTP requests/responses
records = client.get("account", top=10)
```
解析错误响应```python
import json

try:
    client.create("account", invalid_payload)
except DataverseError as e:
    # Extract structured error details
    if e.details and isinstance(e.details, dict):
        error_code = e.details.get('error', {}).get('code')
        error_message = e.details.get('error', {}).get('message')
        
        print(f"Error Code: {error_code}")
        print(f"Error Message: {error_message}")
        
        # Some errors include nested details
        if 'error' in e.details and 'details' in e.details['error']:
            for detail in e.details['error']['details']:
                print(f"  - {detail.get('code')}: {detail.get('message')}")
```
---

# # 5。特定于数据的错误处理

处理OData查询错误```python
try:
    # Invalid OData filter
    records = client.get(
        "account",
        filter="invalid_column eq 0"
    )
except DataverseError as e:
    if "invalid column" in e.message.lower():
        print("Check OData column names and syntax")
    else:
        print(f"Query error: {e.message}")
```
处理文件上传错误```python
try:
    client.upload_file(
        table_name="account",
        record_id=record_id,
        column_name="document_column",
        file_path="large_file.pdf"
    )
except DataverseError as e:
    if e.status_code == 413:
        print("File too large. Use chunked upload mode.")
    elif e.status_code == 400:
        print("Invalid column or file format.")
    else:
        raise
```
处理表元数据操作```python
try:
    # Create custom table
    table_def = {
        "SchemaName": "new_CustomTable",
        "DisplayName": "Custom Table"
    }
    client.create("EntityMetadata", table_def)
except DataverseError as e:
    if "already exists" in e.message:
        print("Table already exists")
    elif "permission" in e.message.lower():
        print("Insufficient permissions to create tables")
    else:
        raise
```
---

# # 6。监控和警报

用监控封装客户端调用```python
from functools import wraps
import time

def monitor_operation(operation_name):
    """Decorator to monitor SDK operations."""
    def decorator(func):
        @wraps(func)
        def wrapper(*args, **kwargs):
            start_time = time.time()
            try:
                result = func(*args, **kwargs)
                duration = time.time() - start_time
                print(f"✓ {operation_name} completed in {duration:.2f}s")
                return result
            except DataverseError as e:
                duration = time.time() - start_time
                print(f"✗ {operation_name} failed after {duration:.2f}s")
                print(f"  Error: {e.code} ({e.status_code}): {e.message}")
                raise
        return wrapper
    return decorator

@monitor_operation("Fetch Accounts")
def get_accounts(client):
    return client.get("account", top=100)

# Usage
try:
    accounts = get_accounts(client)
except DataverseError:
    print("Operation failed - check logs for details")
```
---

# # 7。常见故障处理检查清单

|问题|诊断|解决方案||-------|-----------|----------|
| 401未授权|令牌过期或凭证错误|用有效凭证|重新认证
| 403禁止|用户权限不足|请求管理员|访问
| 404未找到|Record/table不存在|验证模式名和记录ID |
b| 429速率限制|请求过多|实现指数回退重试|
| 500+服务器错误|服务问题|重试指数回退；检查状态页|
| 400错误请求|请求格式无效|检查OData语法、字段名、必填字段|
|网络超时|连接问题|检查网络，在DataverseConfig |中增加超时
| InvalidOperationException |Plugin/workflowerror |查看Dataverse |插件日志

---

# # 8。日志记录最佳实践```python
import logging
import json
from datetime import datetime

class DataverseErrorHandler:
    """Centralized error handling and logging."""
    
    def __init__(self, log_file="dataverse_errors.log"):
        self.logger = logging.getLogger("DataverseSDK")
        handler = logging.FileHandler(log_file)
        formatter = logging.Formatter(
            '%(asctime)s - %(levelname)s - %(message)s'
        )
        handler.setFormatter(formatter)
        self.logger.addHandler(handler)
        self.logger.setLevel(logging.ERROR)
    
    def log_error(self, error: DataverseError, context: str = ""):
        """Log error with context for debugging."""
        error_record = {
            "timestamp": datetime.utcnow().isoformat(),
            "context": context,
            "error": error.to_dict()
        }
        
        self.logger.error(json.dumps(error_record, indent=2))
    
    def is_retryable(self, error: DataverseError) -> bool:
        """Check if error should be retried."""
        return error.is_transient and error.status_code in {408, 429, 500, 502, 503, 504}

# Usage
error_handler = DataverseErrorHandler()

try:
    client.create("account", payload)
except DataverseError as e:
    error_handler.log_error(e, "create_account_batch_1")
    if error_handler.is_retryable(e):
        print("Will retry this operation")
    else:
        print("Operation failed permanently")
```
---

# # 9。看到也

- [DataverseError API Reference]（https://learn.microsoft.com/en-us/python/api/powerplatform-dataverse-client/powerplatform.dataverse.core.errors.dataverseerror）
[Azure SDK错误处理]（https://learn.microsoft.com/en-us/azure/developer/python/sdk/fundamentals/errors）
- [Dataverse SDK入门]（https://learn.microsoft.com/en-us/power-apps/developer/data-platform/sdk-python/get-started）
-[服务保护API限制]（https://learn.microsoft.com/en-us/power-apps/developer/data-platform/optimize-performance-create-update）