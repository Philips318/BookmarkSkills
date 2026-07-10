---
applyTo: '**'
---
# Dataverse SDK for Python -入门

-安装Dataverse Python SDK和先决条件。
—配置Dataverse租户、客户端ID、secret和资源URL的环境变量。
—使用SDK进行OAuth认证和CRUD操作。

# #设置
- Python 3.10+
—推荐：虚拟环境

# #安装```bash
pip install dataverse-sdk
```
## Auth基础
-使用OAuth与Azure AD应用程序注册。
-存储秘密在`.env`和加载通过`python-dotenv`。

##常见任务
-查询表
—Create/update行
-批处理操作
-处理分页和节流

# #提示
-重用客户端；避免频繁的重新认证。
—增加暂时性失败的重试次数。
—记录故障处理请求。