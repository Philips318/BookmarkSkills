#警报和补救参考

关于秘密扫描警报类型、有效性检查、补救工作流程和API访问的详细参考。

##警报类型

###用户警报

当秘密扫描检测到存储库中受支持的秘密时生成。

—显示在“存储库**安全**”页签中
-为提供者模式，非提供者模式，自定义模式和ai检测到的秘密创建
-扫描覆盖所有分支上的整个Git历史

###推送保护警报

当贡献者绕过push保护推送秘密时生成。

—显示在“Security”页签中（filter:`bypassed: true`）。
—记录投稿人选择的旁路原因
-包括提交和文件的秘密被推

**旁路原因及其报警行为：**

|旁路原因|告警状态||---|---|
|关闭（解析为“在测试中使用”）|
这是一个假阳性|关闭（解析为“假阳性”）|
|打开|

###合作伙伴警报

当GitHub检测到泄露的秘密匹配合作伙伴的模式时生成。

-直接发送给服务提供商（例如，AWS, Stripe, GitHub）
—“存储库安全”页签中不显示“**”
-提供者可以自动撤销凭证
—存储库所有者无需处理

##警报列表

###默认警报列表

显示以下警告的主视图：
-支持的提供商模式（例如，GitHub PATs， AWS密钥，Stripe密钥）
-在repo/org/enterprise级别定义的自定义模式

###通用警报列表

单独的视图（从默认列表切换）显示：
-非提供者模式（私钥，连接字符串）
- ai检测通用秘密（密码）* *限制:* *
-每个存储库（打开+关闭）最多5,000个警报
-只有前5个检测到的位置显示非提供者模式
- ai探测到的秘密只显示第一次探测到的位置
—在安全概述汇总视图中不显示

##配对凭证

当资源需要配对凭证（例如，访问密钥+秘密密钥）时：
—警报仅在同一文件中检测到两个部分时创建
-防止局部泄漏产生噪音
-减少误报

有效性检查

有效性检查验证检测到的秘密是否仍处于活动状态。

###如何工作

1. 在repository/organization设置中启用有效性检查
2. GitHub定期将这个秘密发送到发行者的API
3. 验证结果显示在警报中

###验证状态

|状态|含义|优先级||---|---|---|
|`Active`|秘密被确认有效并可被利用|🔴即时|
|`Inactive`|密码已被撤销或过期|🟡低优先级|
|`Unknown`| GitHub无法确定有效性|🟠调查|

###按需验证

单击单个警报上的验证按钮以触发立即检查。

# # #隐私

GitHub对侵入性最小的端点进行最小的API调用（通常是GET请求），选择不返回个人信息的端点。

扩展元数据检查

在启用有效性检查时，提供有关检测到的秘密的附加上下文。

可用的元数据

取决于服务提供商共享的内容：
-秘密所有者信息
-保密的范围和权限
—创建日期和截止日期
—关联的帐号或项目

# # #好处- **更深刻的洞察力** -知道谁拥有秘密
- **优先考虑补救措施** -了解范围和影响
- **改进事件响应** -快速识别责任团队
- **增强合规性-确保机密与治理政策保持一致
- **减少误报** -额外的上下文有助于确定是否需要采取行动

# # #启用

—要求先启用有效性检查
—可以在存储库、组织或企业级别启用
-可通过安全配置批量启用

修复工作流程

###优先级：旋转凭证

**始终首先轮换（撤销和重新颁发）暴露的凭据。**这比从Git历史中删除secret更重要。

###逐步修复1. **接收警报** -通过安全选项卡，电子邮件通知，或webhook
2. **评估严重性** -检查有效性状态（活动=紧急）
3. **旋转证书** -撤销旧的证书并生成一个新的
4. **更新引用** -更新所有使用旧凭证的code/config5. **调查影响** -检查在暴露窗口期间未经授权使用的日志
6. **关闭警告** -标记，以适当的原因解决
7. **可选清除Git历史记录** -从提交历史中删除（耗时）

###从Git历史中删除秘密

如果需要，使用`git filter-repo`（推荐）或`BFG Repo-Cleaner`：```bash
# Install git-filter-repo
pip install git-filter-repo

# Remove a specific file from all history
git filter-repo --path secrets.env --invert-paths

# Force push the cleaned history
git push --force --all
```
b> **注：**重写历史是破坏性的-它使现有的克隆和pr无效。只有在绝对必要的情况下，并且在轮换证书之后才这样做。

###取消警报

选择合适的理由：

|原因|何时使用||---|---|
| **误报** |检测到的字符串不是真正的秘密|
| **已撤销** |凭证已为revoked/rotated|
| **用于测试** | Secret仅在具有可接受风险|的测试代码中使用

为审计跟踪添加解雇注释。

##警报通知

警报通过以下方式生成通知：
- **电子邮件** -存储库管理员，组织所有者，安全经理
- **Webhooks** -`secret_scanning_alert`事件
- **GitHub Actions** -`secret_scanning_alert`事件触发
- **安全概述** -组织级别的聚合视图

# rest API

###列出警报```
GET /repos/{owner}/{repo}/secret-scanning/alerts
```
查询参数：`state`（open/resolved）、`secret_type`、`resolution`、`sort`、`direction`###获取警报细节```
GET /repos/{owner}/{repo}/secret-scanning/alerts/{alert_number}
```
返回：秘密类型，秘密值（如果允许），位置，有效性，解析状态，`dismissed_comment`###更新警报```
PATCH /repos/{owner}/{repo}/secret-scanning/alerts/{alert_number}
```
正文：`state`(open/resolved),`resolution`(false_positive/revoked/used_in_tests/wont_fix),`resolution_comment`###列出警报位置```
GET /repos/{owner}/{repo}/secret-scanning/alerts/{alert_number}/locations
```
返回：文件路径，行号，提交SHA， blob SHA

组织级端点```
GET /orgs/{org}/secret-scanning/alerts
```
列出组织中所有存储库中的警报。

## Webhook事件

# # #`secret_scanning_alert`当秘密扫描警报为：
——创建
——解决
——重新开放
-已验证（有效性状态更改）

有效负载包括：警报号、秘密类型、解析、提交SHA和位置详细信息。

排除配置### `secret_scanning.yml`
设置为`.github/secret_scanning.yml`以自动关闭特定路径的警报：```yaml
paths-ignore:
  - "docs/**"              # Documentation with example secrets
  - "test/fixtures/**"     # Test fixture data
  - "**/*.example"         # Example configuration files
  - "samples/credentials"  # Sample credential files
```
* *限制:* *
-最多1000个条目
—文件大小不能超过1mb
—排除路径也排除在推送保护之外

**排除路径的警报被关闭为“被配置忽略”