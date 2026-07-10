#错误处理指南

本参考文档记录了飞行前验证过程中的常见错误以及如何处理它们。

##核心原理

**失败继续。**在最终报告中捕获所有问题，而不是停留在第一个错误上。这为用户提供了一个需要修复的完整画面。

---

##认证错误

###未登录（Azure CLI）

* *检测:* *```
ERROR: Please run 'az login' to setup account.
ERROR: AADSTS700082: The refresh token has expired
```
**退出代码：**非零

* *处理:* *
1. 注意报告中的错误
2. 包括补救步骤
3. 跳过其余的Azure CLI命令
4. 如果可能的话，继续执行其他验证步骤

* *报告条目:* *```markdown
#### ❌ Azure CLI Authentication Required

- **Severity:** Error
- **Source:** az cli
- **Message:** Not logged in to Azure CLI
- **Remediation:** Run `az login` to authenticate, then re-run preflight validation
- **Documentation:** https://learn.microsoft.com/en-us/cli/azure/authenticate-azure-cli
```
###未登录（azd）

* *检测:* *```
ERROR: not logged in, run `azd auth login` to login
```
* *处理:* *
1. 注意报告中的错误
2. 跳过azd命令
3. 建议`azd auth login`* *报告条目:* *```markdown
#### ❌ Azure Developer CLI Authentication Required

- **Severity:** Error
- **Source:** azd
- **Message:** Not logged in to Azure Developer CLI
- **Remediation:** Run `azd auth login` to authenticate, then re-run preflight validation
```
### Token过期

* *检测:* *```
AADSTS700024: Client assertion is not within its valid time range
AADSTS50173: The provided grant has expired
```
* *处理:* *
1. 注意错误
2. 建议重复认证
3. 跳过Azure操作

---

##权限错误

RBAC权限不足

* *检测:* *```
AuthorizationFailed: The client '...' with object id '...' does not have authorization 
to perform action '...' over scope '...'
```
* *处理:* *
1. **第一次尝试：** Retry with`--validation-level ProviderNoRbac`2. 请注意报告中的权限限制
3. 如果ProviderNoRbac也失败，请报告丢失的特定权限

* *报告条目:* *```markdown
#### ⚠️ Limited Permission Validation

- **Severity:** Warning
- **Source:** what-if
- **Message:** Full RBAC validation failed; using read-only validation
- **Detail:** Missing permission: `Microsoft.Resources/deployments/write` on scope `/subscriptions/xxx`
- **Recommendation:** Request Contributor role on the target resource group, or verify deployment permissions with your administrator
```
没有找到资源组

* *检测:* *```
ResourceGroupNotFound: Resource group 'xxx' could not be found.
```
* *处理:* *
1. 报告附注
2. 建议创建资源组
3. 跳过此范围的假设

* *报告条目:* *```markdown
#### ❌ Resource Group Does Not Exist

- **Severity:** Error
- **Source:** what-if
- **Message:** Resource group 'my-rg' does not exist
- **Remediation:** Create the resource group before deployment:
  ```bash
Az group create——name my-rg——location eastus  ```
```
订阅访问被拒绝

* *检测:* *```
SubscriptionNotFound: The subscription 'xxx' could not be found.
InvalidSubscriptionId: Subscription '...' is not valid
```
* *处理:* *
1. 报告附注
2. 建议检查订阅ID
3. 列出可用订阅

---

##二头肌语法错误

编译错误

* *检测:* *```
/path/main.bicep(22,51) : Error BCP064: Found unexpected tokens
/path/main.bicep(10,5) : Error BCP018: Expected the "=" character at this location
```
* *处理:* *
1. 解析line/column数字的错误输出
2. 在报告中包含所有错误（不要一开始就停止）
3. 继续假设（可能提供额外的上下文）

* *报告条目:* *```markdown
#### ❌ Bicep Syntax Error

- **Severity:** Error
- **Source:** bicep build
- **Location:** `main.bicep:22:51`
- **Code:** BCP064
- **Message:** Found unexpected tokens in interpolated expression
- **Remediation:** Check the string interpolation syntax at line 22
- **Documentation:** https://learn.microsoft.com/en-us/azure/azure-resource-manager/bicep/diagnostics/bcp064
```
没有找到模块

* *检测:* *```
Error BCP091: An error occurred reading file. Could not find file '...'
Error BCP190: The module is not valid
```
* *处理:* *
1. 注释缺失模块
2. 检查是否需要`bicep restore`3. 验证模块路径

###参数文件问题

* *检测:* *```
Error BCP032: The value must be a compile-time constant
Error BCP035: The specified object is missing required properties
```
* *处理:* *
1. 注意参数问题
2. 指出哪些参数有问题
3. 建议修复

---

##工具未安装

没有找到Azure CLI

* *检测:* *```
'az' is not recognized as an internal or external command
az: command not found
```
* *处理:* *
1. 报告附注
2. 提供安装说明。
—如果可用，请使用Azure MCP`extension_cli_install`工具获取安装说明。
-否则，请查看https://learn.microsoft.com/en-us/cli/azure/install-azure-cli.的说明
3. 跳过az命令

* *报告条目:* *```markdown
#### ⏭️ Azure CLI Not Installed

- **Severity:** Warning
- **Source:** environment
- **Message:** Azure CLI (az) is not installed or not in PATH
- **Remediation:** Install the Azure CLI <ADD INSTALLATION INSTRUCTIONS HERE>
- **Impact:** What-if validation using az commands was skipped
```
没有找到肱二头肌命令行

* *检测:* *```
'bicep' is not recognized as an internal or external command
bicep: command not found
```
* *处理:* *
1. 报告附注
2. Azure CLI可能有内置的肱二头肌-试试`az bicep build`3. 提供安装链接

* *报告条目:* *```markdown
#### ⏭️ Bicep CLI Not Installed

- **Severity:** Warning
- **Source:** environment
- **Message:** Bicep CLI is not installed
- **Remediation:** Install Bicep CLI: https://learn.microsoft.com/en-us/azure/azure-resource-manager/bicep/install
- **Impact:** Syntax validation was skipped; Azure will validate during what-if
```
没有找到Azure开发人员命令行

* *检测:* *```
'azd' is not recognized as an internal or external command
azd: command not found
```
* *处理:* *
1. 如果存在`azure.yaml`，则需要这样做
2. 如果可能，请回退到az CLI命令
3. 报告附注

---

##如果特定错误

嵌套模板限制

* *检测:* *```
The deployment exceeded the nested template limit of 500
```
* *处理:* *
1. 作为警告的注释（不是错误）
2. 将受影响的资源显示为“忽略”
3. 建议手工审核

不支持模板链接

* *检测:* *```
templateLink references in nested deployments won't be visible in what-if
```
* *处理:* *
1. 注：作为警告
2. 解释限制
3. 资源将在实际部署时进行验证

未求值表达式

**检测：**属性显示函数名如`[utcNow()]`而不是值

* *处理:* *
1. 作为参考的注释
2. 解释这些是在部署时评估的
3. 不是错误

---

##网络错误

# # #超时

* *检测:* *```
Connection timed out
Request timed out
```
* *处理:* *
1. 建议重试
2. 检查网络连接
3. 可能指示Azure服务问题SSL/TLS错误

* *检测:* *```
SSL: CERTIFICATE_VERIFY_FAILED
unable to get local issuer certificate
```
* *处理:* *
1. 报告附注
2. 可能指示代理或公司防火墙
3. 建议检查SSL设置

---

##后退策略

当主验证失败时，按顺序尝试回退：```
Provider (full RBAC validation)
    ↓ fails with permission error
ProviderNoRbac (validation without write permission check)
    ↓ fails
Template (static syntax only)
    ↓ fails
Report all failures and skip what-if analysis
```
**始终继续生成报告**，即使所有验证步骤都失败。

---

错误报告聚合

当出现多个错误时，逻辑地将它们聚合起来：

1. **按源分组**（二头肌，假设，权限）
2. **按严重程度排序**（错误优先于警告）
3. 重复数据删除**类似错误
4. **在顶部提供汇总计数**

例子:```markdown
## Issues

Found **3 errors** and **2 warnings**

### Errors (3)

1. [Bicep Syntax Error - main.bicep:22:51](#error-1)
2. [Bicep Syntax Error - main.bicep:45:10](#error-2)
3. [Resource Group Not Found](#error-3)

### Warnings (2)

1. [Limited Permission Validation](#warning-1)
2. [Nested Template Limit Reached](#warning-2)
```
---

##退出代码引用

|工具|退出代码|含义||------|-----------|---------|
| az | 0 | Success |
| az | 1 |一般错误|
| az | 2 |命令未找到|
| az | 3 |必需参数缺失|
| azd | 0 | Success |
| azd | 1 | |错误
|肱二头肌| 0 |构建成功|
|二头肌| 1 |构建失败（错误）|
构建成功，提示|