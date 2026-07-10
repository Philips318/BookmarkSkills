---
name: namecheap
description: 'Manage DNS records for domains registered with Namecheap via their API. List domains, view/add/update/remove DNS host entries (A, AAAA, CNAME, MX, TXT, etc.), and guide users through API setup including public IP detection and credential configuration. Use when the user mentions Namecheap, DNS records, domain management, or wants to add/change/remove A records, CNAME records, MX records, or TXT records for their domains.'
---
# Namecheap DNS管理

**实用技能** -通过Namecheap API管理DNS记录。
用于：“添加DNS记录”，“更新A记录”，“管理Namecheap域名”，“设置CNAME”，“添加MX记录”，“添加TXT记录”，“列出我的域名”，“显示DNS记录”，“Namecheap设置”，“配置Namecheap API”，“我的公共IP是什么”
不要用于：域名registration/purchase， SSL证书管理，主机配置，非namecheap DNS提供商

# #工作流程

###首次设置

在执行任何API命令之前，请验证凭证是否已配置：1. **检查现有配置** -查找`~/.namecheap-api`2. 如果没有配置，引导用户完成设置：
a. **显示公网IP地址** -执行命令`python3 namecheap.py public-ip`，显示用户的公网IP地址
b. **启动IP白名单** -告诉用户进入https://ap.www.namecheap.com/settings/tools/apiaccess/,使能API（选择ON），将显示的IP加入白名单
c. **让用户自己运行安装程序** -要求用户在自己的终端上直接**运行`python3 namecheap.py setup`。脚本提示输入用户名，并使用隐藏提示符（`getpass`）读取API密钥，用`chmod 600`写入`~/.namecheap-api`，然后验证连接。**永远不要要求用户将他们的API密钥粘贴到聊天中，永远不要记录，回显或显示API密钥值。**如果你不能为用户运行一个交互式终端，指示他们自己运行`setup`，或者在他们自己的shell中导出`NAMECHEAP_API_USER`和`NAMECHEAP_API_KEY`作为环境变量-而不是通过`ask_user`收集秘密。
d。**确认** -一旦用户报告安装成功，继续DNS操作。### DNS操作

使用`namecheap.py`脚本（绑定在此技能的目录中）进行所有API交互。它只需要Python 3（标准库-不需要`pip install`），在macOS， Linux和Windows上工作相同：```bash
# Show public IP (for setup)
python3 namecheap.py public-ip

# Run setup flow
python3 namecheap.py setup

# List domains
python3 namecheap.py domains.getList

# Get nameservers for a domain (shows if using Namecheap DNS or custom)
python3 namecheap.py domains.dns.getList --domain example.com

# Get DNS records for a domain
python3 namecheap.py domains.dns.getHosts --domain example.com

# Add a single record (preserves existing records)
python3 namecheap.py dns.addHost --domain example.com --type A --name www --address 1.2.3.4 --ttl 1800

# Remove a single record
python3 namecheap.py dns.removeHost --domain example.com --type A --name www --address 1.2.3.4

# Replace all records from a JSON file
python3 namecheap.py domains.dns.setHosts --domain example.com --hosts records.json

# Switch to Namecheap default DNS
python3 namecheap.py domains.dns.setDefault --domain example.com

# Switch to custom nameservers
python3 namecheap.py domains.dns.setCustom --domain example.com --nameservers ns1.cloudflare.com,ns2.cloudflare.com

# Get email forwarding rules
python3 namecheap.py domains.dns.getEmailForwarding --domain example.com

# Set email forwarding (single rule)
python3 namecheap.py domains.dns.setEmailForwarding --domain example.com --mailbox info --forward-to user@gmail.com

# Set email forwarding (from JSON file)
python3 namecheap.py domains.dns.setEmailForwarding --domain example.com --forwards forwards.json

# Create a child nameserver (glue record)
python3 namecheap.py domains.ns.create --domain example.com --nameserver ns1.example.com --ip 1.2.3.4

# Delete a child nameserver
python3 namecheap.py domains.ns.delete --domain example.com --nameserver ns1.example.com

# Get nameserver info
python3 namecheap.py domains.ns.getInfo --domain example.com --nameserver ns1.example.com

# Update nameserver IP
python3 namecheap.py domains.ns.update --domain example.com --nameserver ns1.example.com --old-ip 1.2.3.4 --ip 5.6.7.8
```
JSON文件格式`domains.dns.setHosts --hosts records.json`期望一个具有Namecheap API字段名的对象数组：```json
[
  { "HostName": "@", "RecordType": "A", "Address": "1.2.3.4", "TTL": 1800 },
  { "HostName": "www", "RecordType": "CNAME", "Address": "@", "TTL": 1800 },
  { "HostName": "@", "RecordType": "MX", "Address": "mail.example.com.", "TTL": 1800, "MXPref": 10 }
]
```
`domains.dns.setEmailForwarding --forwards forwards.json`期望一个邮箱规则数组：```json
[
  { "MailBox": "info", "ForwardTo": "team@example.net" },
  { "MailBox": "sales", "ForwardTo": "owner@example.net" }
]
```
# #行为- **总是先检查凭据。**在任何API操作之前，请验证`~/.namecheap-api`是否存在并且可读。如果没有，则运行设置流。
- **在修改前显示当前记录。**在添加或删除记录之前，始终获取并显示当前DNS记录，以便用户可以确认更改。
- **使用`ask_user`确认破坏性更改。**在删除记录或用`setHosts`替换所有记录之前，请与用户确认。
- ** Namecheap`setHosts`API替换所有记录。**永远不要直接调用`domains.dns.setHosts`，除非你已经获取了所有现有的记录。使用`dns.addHost`和`dns.removeHost`进行安全的单记录操作——它们在内部处理读取-修改-写入周期。
- **用人的语言解释TTL。**当用户询问TTL时，说明1800 = 30分钟，3600 = 1小时等。
- **处理多部分顶级域名。**域名如`example.co.uk`有SLD=example和TLD=co.uk。脚本可以识别常见二级后缀的内置列表（例如`co.uk`，`com.au`,`co.jp`,`com.br`）。此列表是尽力而为的结果，并非完整的公共后缀数据库—如果具有未列出的多部分后缀的域返回`2019166`（“domain not found”）错误，则SLD/TLD拆分可能是错误的。在这种情况下，请与用户确认已注册的域并报告限制。凭证存储

凭证存储在`~/.namecheap-api`中：```bash
NAMECHEAP_API_USER="username"
NAMECHEAP_API_KEY="api-key-here"
```
该文件必须具有`600`权限（所有者仅为read/write）。或者，脚本从`NAMECHEAP_API_USER`和`NAMECHEAP_API_KEY`环境变量读取凭据，当设置这两个变量时，它们优先于文件。

支持的记录类型

A, aaaa, cname, mx, mxe, txt, url, url301, frame

# #引用

请参阅`references/namecheap-api.md`获取完整的API文档，包括request/response格式。