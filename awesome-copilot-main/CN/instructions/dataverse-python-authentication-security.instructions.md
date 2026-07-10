---
applyTo: '**'
---
# Dataverse SDK for Python -身份验证和安全模式

基于官方Microsoft Azure SDK身份验证文档和Dataverse SDK最佳实践。

# # 1。认证概述

Python的Dataverse SDK使用Azure Identity凭据进行基于令牌的身份验证。这种方法遵循最小特权原则，适用于本地开发、云部署和本地环境。

为什么是基于令牌的身份验证？

**相对于连接字符串的优势**：
-建立应用程序所需的特定权限（最小权限原则）
-凭据仅适用于预期的应用程序
-有了受管理的身份，没有秘密需要存储或泄露
-跨环境无缝工作，无需更改代码

---

# # 2。凭证类型和选择

交互式浏览器凭证（本地开发）**在本地开发期间用于**：开发人员工作站。```python
from azure.identity import InteractiveBrowserCredential
from PowerPlatform.Dataverse.client import DataverseClient

# Opens browser for authentication
credential = InteractiveBrowserCredential()
client = DataverseClient(
    base_url="https://myorg.crm.dynamics.com",
    credential=credential
)

# First use prompts for sign-in; subsequent calls use cached token
records = client.get("account")
```
**何时使用**：
-✅交互式开发和测试
-✅桌面应用程序与UI
-❌后台服务或调度任务

---

默认Azure凭证（推荐用于所有环境）

**用于**：在多个环境中运行的应用程序（开发→测试→生产）。```python
from azure.identity import DefaultAzureCredential
from PowerPlatform.Dataverse.client import DataverseClient

# Attempts credentials in this order:
# 1. Environment variables (app service principal)
# 2. Azure CLI credentials (local development)
# 3. Azure PowerShell credentials (local development)
# 4. Managed identity (when running in Azure)
credential = DefaultAzureCredential()

client = DataverseClient(
    base_url="https://myorg.crm.dynamics.com",
    credential=credential
)

records = client.get("account")
```
* * * *优势:
-单代码路径适用于任何地方
-不需要特定于环境的逻辑
—自动检测可用凭据
-首选生产应用程序

* * * *证书链:
1. 环境变量（`AZURE_CLIENT_ID`、`AZURE_TENANT_ID`、`AZURE_CLIENT_SECRET`）
2. Visual Studio Code登录
3. Azure CLI （`az login`）
4. Azure PowerShell （`Connect-AzAccount`）
5. 托管身份（在Azure vm、App Service、AKS等上）

---

客户端秘密凭证（服务主体）

**用于**：无人值守身份验证（计划作业、脚本、本地服务）。```python
from azure.identity import ClientSecretCredential
from PowerPlatform.Dataverse.client import DataverseClient
import os

credential = ClientSecretCredential(
    tenant_id=os.environ["AZURE_TENANT_ID"],
    client_id=os.environ["AZURE_CLIENT_ID"],
    client_secret=os.environ["AZURE_CLIENT_SECRET"]
)

client = DataverseClient(
    base_url="https://myorg.crm.dynamics.com",
    credential=credential
)

records = client.get("account")
```
* * * *设置步骤:
1. 在Azure AD中创建应用程序注册
2. 创建客户端秘密（保证安全！）
3. 授予应用程序Dataverse权限
4. 将凭证存储在环境变量或安全保管库中

* *安全问题* *:
-⚠️永远不要在源代码中硬编码凭证
-⚠️在Azure密钥库或环境变量中存储秘密
-⚠️定期轮换凭证
-⚠️使用最低要求的权限

---

托管身份凭证（Azure资源）

**用于**：托管在Azure中的应用程序（应用程序服务，Azure功能，AKS, vm）。```python
from azure.identity import ManagedIdentityCredential
from PowerPlatform.Dataverse.client import DataverseClient

# No secrets needed - Azure manages identity
credential = ManagedIdentityCredential()

client = DataverseClient(
    base_url="https://myorg.crm.dynamics.com",
    credential=credential
)

records = client.get("account")
```
* * * *好处:
-✅没有秘密要管理
-✅自动刷新token
-✅高度安全
-✅内建到Azure服务

* *设置* *:
1. 在Azure资源（应用程序服务、虚拟机等）上启用托管身份
2. 向受管理的标识授予数据厌恶权限
3. 代码自动使用标识

---

# # 3。特定于环境的配置

地方发展```python
# .env file (git-ignored)
DATAVERSE_URL=https://myorg-dev.crm.dynamics.com

# Python code
import os
from azure.identity import DefaultAzureCredential
from PowerPlatform.Dataverse.client import DataverseClient

# Uses your Azure CLI credentials
credential = DefaultAzureCredential()
client = DataverseClient(
    base_url=os.environ["DATAVERSE_URL"],
    credential=credential
)
```
**设置**:`az login`与您的开发人员帐户

---

Azure应用服务/ Azure功能```python
from azure.identity import ManagedIdentityCredential
from PowerPlatform.Dataverse.client import DataverseClient

# Automatically uses managed identity
credential = ManagedIdentityCredential()
client = DataverseClient(
    base_url="https://myorg.crm.dynamics.com",
    credential=credential
)
```
**设置**：在App Service中启用身份管理，在Dataverse中授予权限

---

本地/第三方托管```python
import os
from azure.identity import ClientSecretCredential
from PowerPlatform.Dataverse.client import DataverseClient

credential = ClientSecretCredential(
    tenant_id=os.environ["AZURE_TENANT_ID"],
    client_id=os.environ["AZURE_CLIENT_ID"],
    client_secret=os.environ["AZURE_CLIENT_SECRET"]
)

client = DataverseClient(
    base_url="https://myorg.crm.dynamics.com",
    credential=credential
)
```
**设置**：创建服务主体，安全存储凭证，授予Dataverse权限

---

# # 4。客户端配置和连接设置

###基本配置```python
from PowerPlatform.Dataverse.core.config import DataverseConfig
from azure.identity import DefaultAzureCredential
from PowerPlatform.Dataverse.client import DataverseClient

cfg = DataverseConfig()
cfg.logging_enable = True  # Enable detailed logging

client = DataverseClient(
    base_url="https://myorg.crm.dynamics.com",
    credential=DefaultAzureCredential(),
    config=cfg
)
```
HTTP调优```python
from PowerPlatform.Dataverse.core.config import DataverseConfig

cfg = DataverseConfig()

# Timeout settings
cfg.http_timeout = 30          # Request timeout in seconds

# Retry configuration
cfg.http_retries = 3           # Number of retry attempts
cfg.http_backoff = 1           # Initial backoff in seconds

# Connection reuse
cfg.connection_timeout = 5     # Connection timeout

client = DataverseClient(
    base_url="https://myorg.crm.dynamics.com",
    credential=credential,
    config=cfg
)
```
---

# # 5。安全最佳实践

# # # 1。不要硬编码凭证```python
# ❌ BAD - Don't do this!
credential = ClientSecretCredential(
    tenant_id="your-tenant-id",
    client_id="your-client-id",
    client_secret="your-secret-key"  # EXPOSED!
)

# ✅ GOOD - Use environment variables
import os
credential = ClientSecretCredential(
    tenant_id=os.environ["AZURE_TENANT_ID"],
    client_id=os.environ["AZURE_CLIENT_ID"],
    client_secret=os.environ["AZURE_CLIENT_SECRET"]
)
```
# # # 2。安全地存储秘密

* * * *发展:```bash
# .env file (git-ignored)
AZURE_TENANT_ID=your-tenant-id
AZURE_CLIENT_ID=your-client-id
AZURE_CLIENT_SECRET=your-secret-key
```
生产* * * *:```python
from azure.keyvault.secrets import SecretClient
from azure.identity import DefaultAzureCredential

# Retrieve secrets from Azure Key Vault
credential = DefaultAzureCredential()
client = SecretClient(
    vault_url="https://mykeyvault.vault.azure.net",
    credential=credential
)

secret = client.get_secret("dataverse-client-secret")
```
# # # 3。实现最小特权原则```python
# Grant minimal permissions:
# - Only read if app only reads
# - Only specific tables if possible
# - Time-limit credentials (auto-rotation)
# - Use managed identity instead of shared secrets
```
# # # 4。监控认证事件```python
import logging

logger = logging.getLogger("dataverse_auth")

try:
    client = DataverseClient(
        base_url="https://myorg.crm.dynamics.com",
        credential=credential
    )
    logger.info("Successfully authenticated to Dataverse")
except Exception as e:
    logger.error(f"Authentication failed: {e}")
    raise
```
# # # 5。处理令牌过期```python
from azure.core.exceptions import ClientAuthenticationError
import time

def create_with_auth_retry(client, table_name, payload, max_retries=2):
    """Create record, retrying if token expired."""
    for attempt in range(max_retries):
        try:
            return client.create(table_name, payload)
        except ClientAuthenticationError:
            if attempt < max_retries - 1:
                logger.warning("Token expired, retrying...")
                time.sleep(1)
            else:
                raise
```
---

# # 6。多租户应用程序

租户感知客户端```python
from azure.identity import DefaultAzureCredential
from PowerPlatform.Dataverse.client import DataverseClient

def get_client_for_tenant(tenant_id: str) -> DataverseClient:
    """Get DataverseClient for specific tenant."""
    credential = DefaultAzureCredential()
    
    # Dataverse URL contains tenant-specific org
    base_url = f"https://{get_org_for_tenant(tenant_id)}.crm.dynamics.com"
    
    return DataverseClient(
        base_url=base_url,
        credential=credential
    )

def get_org_for_tenant(tenant_id: str) -> str:
    """Map tenant to Dataverse organization."""
    # Implementation depends on your multi-tenant strategy
    # Could be database lookup, configuration, etc.
    pass
```
---

# # 7。故障排除的身份验证

###错误：“Access Denied” （403）```python
try:
    client.get("account")
except DataverseError as e:
    if e.status_code == 403:
        print("User/app lacks Dataverse permissions")
        print("Ensure Dataverse security role is assigned")
```
###错误：“Invalid Credentials” （401）```python
# Check credential source
from azure.identity import DefaultAzureCredential

try:
    cred = DefaultAzureCredential(exclude_cli_credential=False, 
                                  exclude_powershell_credential=False)
    # Force re-authentication
    import subprocess
    subprocess.run(["az", "login"])
except Exception as e:
    print(f"Authentication failed: {e}")
```
###错误：“Invalid Tenant”```python
# Verify tenant ID
import json
from azure.identity import DefaultAzureCredential

credential = DefaultAzureCredential()
token = credential.get_token("https://dataverse.dynamics.com/.default")

# Decode token to verify tenant
import base64
payload = base64.b64decode(token.token.split('.')[1] + '==')
claims = json.loads(payload)
print(f"Token tenant: {claims.get('tid')}")
```
---

# # 8。证书生命周期

### Token刷新

Azure Identity自动处理令牌刷新：```python
# Tokens are cached and refreshed automatically
credential = DefaultAzureCredential()

# First call acquires token
client.get("account")

# Subsequent calls reuse cached token
client.get("contact")

# If token expires, SDK automatically refreshes
```
###会话管理```python
class DataverseSession:
    """Manages DataverseClient lifecycle."""
    
    def __init__(self, base_url: str):
        from azure.identity import DefaultAzureCredential
        
        self.client = DataverseClient(
            base_url=base_url,
            credential=DefaultAzureCredential()
        )
    
    def __enter__(self):
        return self.client
    
    def __exit__(self, exc_type, exc_val, exc_tb):
        # Cleanup if needed
        pass

# Usage
with DataverseSession("https://myorg.crm.dynamics.com") as client:
    records = client.get("account")
```
---

# # 9。Dataverse-Specific安全

行级安全（RLS）

用户的Dataverse安全角色决定可访问的记录：```python
from azure.identity import InteractiveBrowserCredential
from PowerPlatform.Dataverse.client import DataverseClient

# Each user gets client with their credentials
def get_user_client(user_username: str) -> DataverseClient:
    # User must already be authenticated
    credential = InteractiveBrowserCredential()
    
    client = DataverseClient(
        base_url="https://myorg.crm.dynamics.com",
        credential=credential
    )
    
    # User only sees records they have access to
    return client
```
安全角色

分配最少需要的角色：
- **系统管理员**：完全访问（避免应用程序）
- **销售经理**：销售表格+报表
- **服务代表**：服务案例+知识
—**自定义**：创建具有指定表权限的角色

---

# # 10。看到也

- [Azure身份客户端库]（https://learn.microsoft.com/en-us/python/api/azure-identity）
-[认证到Azure服务]（https://learn.microsoft.com/en-us/azure/developer/python/sdk/authentication/overview）
- [Azure密钥库的秘密]（https://learn.microsoft.com/en-us/azure/key-vault/general/overview）
- [data - averse Security Model]（https://learn.microsoft.com/en-us/power-platform/admin/security/security-overview）