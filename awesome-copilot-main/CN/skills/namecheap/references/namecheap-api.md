# Namecheap API引用

##基础URL```
https://api.namecheap.com/xml.response
```
# #身份验证

所有请求都需要这些常见参数：

| |参数说明||-----------|-------------|
|`ApiUser`|命名堆用户名|
|`ApiKey`|来自https://ap.www.namecheap.com/settings/tools/apiaccess/|的API密钥
|`UserName`|与ApiUser |相同
|`ClientIp`|客户端|的公网IP地址白名单
|`Command`|以`namecheap.`|为前缀的API命令

##设置要求

1. 登录Namecheap
2. 到https://ap.www.namecheap.com/settings/tools/apiaccess/3. 启用API访问（切换到ON）
4. 将客户端公网IP地址加入白名单
5. 复制生成的API密钥

# #命令

---

# # # namecheap.domains.getList

列出该帐户中的所有域。

* *额外参数:* *

| |必选参数|描述||-----------|----------|-------------|
|`ListType`|否|`ALL`（默认）、`EXPIRING`、`EXPIRED`|
|`SearchTerm`|否| |过滤关键字
|`Page`|否|页面编号，默认为1
|`PageSize`|不|每页结果，10-100（默认值：20）|
|`SortBy`|不|`NAME`，`NAME_DESC`,`EXPIREDATE`,`EXPIREDATE_DESC`,`CREATEDATE`,`CREATEDATE_DESC`|

* *响应XML: * *```xml
<ApiResponse Status="OK">
  <CommandResponse Type="namecheap.domains.getList">
    <DomainGetListResult>
      <Domain ID="123" Name="example.com" User="user" Created="01/01/2020"
        Expires="01/01/2025" IsExpired="false" IsLocked="true" AutoRenew="true"
        WhoisGuard="ENABLED" />
    </DomainGetListResult>
    <Paging><TotalItems>5</TotalItems><CurrentPage>1</CurrentPage><PageSize>20</PageSize></Paging>
  </CommandResponse>
</ApiResponse>
```
---

# # # namecheap.domains.dns.getList

获取与域关联的DNS服务器列表（显示它是使用Namecheap DNS还是自定义名称服务器）。

* *额外参数:* *

| |必选参数|描述||-----------|----------|-------------|
|`SLD`|是|二级域（例如，`example.com`的`example`） |
|`TLD`|是|顶级域名（例如，`example.com`的`com`） |

* *响应XML: * *```xml
<ApiResponse Status="OK">
  <CommandResponse Type="namecheap.domains.dns.getList">
    <DomainDNSGetListResult Domain="example.com" IsUsingOurDNS="true">
      <Nameserver>dns1.registrar-servers.com</Nameserver>
      <Nameserver>dns2.registrar-servers.com</Nameserver>
    </DomainDNSGetListResult>
  </CommandResponse>
</ApiResponse>
```
---

# # # namecheap.domains.dns.getHosts

获取域的DNS主机记录。

* *额外参数:* *

| |必选参数|描述||-----------|----------|-------------|
|`SLD`|是|二级域（例如，`example.com`的`example`） |
|`TLD`|是|顶级域名（例如，`example.com`的`com`） |

* *响应XML: * *```xml
<ApiResponse Status="OK">
  <CommandResponse Type="namecheap.domains.dns.getHosts">
    <DomainDNSGetHostsResult Domain="example.com" IsUsingOurDNS="true">
      <host HostId="1" Name="@" Type="A" Address="1.2.3.4" MXPref="0" TTL="1800" />
      <host HostId="2" Name="www" Type="CNAME" Address="example.com." MXPref="0" TTL="1800" />
      <host HostId="3" Name="@" Type="MX" Address="mail.example.com." MXPref="10" TTL="1800" />
      <host HostId="4" Name="@" Type="TXT" Address="v=spf1 include:_spf.google.com ~all" MXPref="0" TTL="1800" />
    </DomainDNSGetHostsResult>
  </CommandResponse>
</ApiResponse>
```
---

# # # namecheap.domains.dns.setHosts

设置（替换）一个域的所有DNS主机记录。

**重要：**此命令将替换所有现有记录。总是先获取现有记录。

* *额外参数:* *

| |必选参数|描述||-----------|----------|-------------|
|`SLD`|是|二级域|
|`TLD`|是|顶级域|
|`HostNameN`|是|记录N的主机名（例如，`@`,`www`,`mail`） |
|`RecordTypeN`|是|记录N （A、AAAA、CNAME、MX、TXT等）的记录类型|
|`AddressN`|是|记录N的值（IP地址或目标主机名）|
|`MXPrefN`|否|记录N的MX优先级（MX记录需要）|
|`TTLN`|否|记录N的生存时间（默认为1800）

记录从1开始编号：`HostName1`、`RecordType1`、`Address1`、`HostName2`、`RecordType2`、`Address2`等。

* *响应XML: * *```xml
<ApiResponse Status="OK">
  <CommandResponse Type="namecheap.domains.dns.setHosts">
    <DomainDNSSetHostsResult Domain="example.com" IsSuccess="true" />
  </CommandResponse>
</ApiResponse>
```
---

# # # namecheap.domains.dns.setDefault

设置一个域使用Namecheap的默认DNS服务器。

* *额外参数:* *

| |必选参数|描述||-----------|----------|-------------|
|`SLD`|是|二级域|
|`TLD`|是|顶级域|

* *响应XML: * *```xml
<ApiResponse Status="OK">
  <CommandResponse Type="namecheap.domains.dns.setDefault">
    <DomainDNSSetDefaultResult Domain="example.com" Updated="true" />
  </CommandResponse>
</ApiResponse>
```
---

# # # namecheap.domains.dns.setCustom

设置一个域使用自定义名称服务器（例如，Cloudflare, Route53）。

* *额外参数:* *

| |必选参数|描述||-----------|----------|-------------|
|`SLD`|是|二级域|
|`TLD`|是|顶级域|
|`Nameservers`|是|以逗号分隔的域名服务器列表（最多12个，不支持空格）|

* *的例子:* *`Nameservers=ns1.cloudflare.com,ns2.cloudflare.com`* *响应XML: * *```xml
<ApiResponse Status="OK">
  <CommandResponse Type="namecheap.domains.dns.setCustom">
    <DomainDNSSetCustomResult Domain="example.com" Updated="true" />
  </CommandResponse>
</ApiResponse>
```
---

# # # namecheap.domains.dns.getEmailForwarding

获取域的电子邮件转发设置。

* *额外参数:* *

| |必选参数|描述||-----------|----------|-------------|
|`DomainName`|是|全域名（如`example.com`） |

* *响应XML: * *```xml
<ApiResponse Status="OK">
  <CommandResponse Type="namecheap.domains.dns.getEmailForwarding">
    <DomainDNSGetEmailForwardingResult Domain="example.com">
      <Forward mailboxid="1" mailbox="info" ForwardTo="user@gmail.com" />
      <Forward mailboxid="2" mailbox="support" ForwardTo="help@company.com" />
    </DomainDNSGetEmailForwardingResult>
  </CommandResponse>
</ApiResponse>
```
---

# # # namecheap.domains.dns.setEmailForwarding

为一个域设置邮件转发。替换所有现有的转发规则。

* *额外参数:* *

| |必选参数|描述||-----------|----------|-------------|
|`DomainName`|是|全域名（如`example.com`） |
|`MailBoxN`|是|规则N的邮箱名（例如，`info`,`support`） |
|`ForwardToN`|是|规则N |的目的邮件

规则从1开始编号：`MailBox1`、`ForwardTo1`、`MailBox2`、`ForwardTo2`等。
删除所有MailBox/ForwardTo参数将删除所有转发规则。

* *响应XML: * *```xml
<ApiResponse Status="OK">
  <CommandResponse Type="namecheap.domains.dns.setEmailForwarding">
    <DomainDNSSetEmailForwardingResult Domain="example.com" IsSuccess="true" />
  </CommandResponse>
</ApiResponse>
```
---

# # # namecheap.domains.ns.create

为域创建子名称服务器（粘合记录）。

* *额外参数:* *

| |必选参数|描述||-----------|----------|-------------|
|`SLD`|是|二级域|
|`TLD`|是|顶级域|
|`Nameserver`|是|要创建的域名服务器主机名（例如，`ns1.example.com`） |
|`IP`|是| |名称服务器的IP地址

* *响应XML: * *```xml
<ApiResponse Status="OK">
  <CommandResponse Type="namecheap.domains.ns.create">
    <DomainNSCreateResult Domain="example.com" Nameserver="ns1.example.com" IP="1.2.3.4" IsSuccess="true" />
  </CommandResponse>
</ApiResponse>
```
---

# # # namecheap.domains.ns.delete

删除子名称服务器。

* *额外参数:* *

| |必选参数|描述||-----------|----------|-------------|
|`SLD`|是|二级域|
|`TLD`|是|顶级域|
|`Nameserver`|是|删除|的域名服务器主机名

* *响应XML: * *```xml
<ApiResponse Status="OK">
  <CommandResponse Type="namecheap.domains.ns.delete">
    <DomainNSDeleteResult Domain="example.com" Nameserver="ns1.example.com" IsSuccess="true" />
  </CommandResponse>
</ApiResponse>
```
---

# # # namecheap.domains.ns.getInfo

获取有关子名称服务器的信息。

* *额外参数:* *

| |必选参数|描述||-----------|----------|-------------|
|`SLD`|是|二级域|
|`TLD`|是|顶级域|
|`Nameserver`|是|查询|的域名服务器主机名

* *响应XML: * *```xml
<ApiResponse Status="OK">
  <CommandResponse Type="namecheap.domains.ns.getInfo">
    <DomainNSInfoResult Domain="example.com" Nameserver="ns1.example.com" IP="1.2.3.4">
      <NameserverStatuses>
        <Status>OK</Status>
      </NameserverStatuses>
    </DomainNSInfoResult>
  </CommandResponse>
</ApiResponse>
```
---

# # # namecheap.domains.ns.update

更新子名称服务器的IP地址。

* *额外参数:* *

| |必选参数|描述||-----------|----------|-------------|
|`SLD`|是|二级域|
|`TLD`|是|顶级域|
|`Nameserver`|是|更新|的域名服务器主机名
|`OldIP`|是|名称服务器|的当前IP地址
|`IP`|是| |名称服务器的新IP地址

* *响应XML: * *```xml
<ApiResponse Status="OK">
  <CommandResponse Type="namecheap.domains.ns.update">
    <DomainNSUpdateResult Domain="example.com" Nameserver="ns1.example.com" IsSuccess="true" />
  </CommandResponse>
</ApiResponse>
```
##错误响应```xml
<ApiResponse Status="ERROR">
  <Errors>
    <Err Code="2019166">Domain not found</Err>
  </Errors>
</ApiResponse>
```
常见错误码：
-`1011102`-无效的API密钥
-`1011148`- IP不在白名单中
-`2019166`-域名未找到
-`2016166`-域名不使用Namecheap DNS

记录类型

|类型|描述|地址格式||------|-------------|---------------|
|`A`| IPv4地址|`1.2.3.4`|
|`AAAA`| IPv6地址|`2001:db8::1`|
|`CNAME`|规范名称|`target.example.com.`|
|`MX`|邮件交换|`mail.example.com.`（需要MXPref） |
|`MXE`| MX等效（IP） |`1.2.3.4`|
|`TXT`|文本记录|任意文本值|
|`URL`| URL重定向（未屏蔽）|`http://example.com`|
|`URL301`|永久重定向|`http://example.com`|
|`FRAME`| URL重定向（屏蔽）|`http://example.com`|

## TTL值

|秒|人读||---------|---------------|
| 60 | 1分钟|
| 300 | 5分钟|
| 1800 | 30分钟（默认值）|
| 3600 | 1小时|
| 14400 | 4小时|
| 43200 | 12小时|
| 86400 | 1天|