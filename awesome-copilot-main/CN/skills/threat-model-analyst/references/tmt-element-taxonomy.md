元素分类-威胁模型DFD参考代码

从源代码分析中识别DFD元素的完整参考。
与Microsoft威胁建模工具（TMT）元素类型保持一致，以实现TM7兼容性。
这是所有TMT类型分类的单一权威文件。

**图表样式和渲染规则**在：[diagram-conventions.md]（./diagram-conventions.md）
**该文件涵盖：**在代码中寻找什么，如何分类，以及如何命名。

---

# # 1。元素类型

**注：** TMT编号（如`SE.P.TMCore.OSProcess`）仅供分类参考。**不要使用TMT id作为美人鱼节点id。**使用简洁，可读的PascalCase id（例如，`WebServer`,`SqlDatabase`）。

1.1进程类型

| TMT ID |名称|识别|的代码模式|--------|------|---------------------------|
|`SE.P.TMCore.OSProcess`|操作系统进程|本机可执行程序，系统进程，衍生进程|
|`SE.P.TMCore.Thread`|线程|线程池，`Task`,`pthread`，工作线程|
|本地应用| Win32应用，C/C++可执行文件，桌面应用|
|`SE.P.TMCore.NetApp`|管理应用|。. NET应用程序，c#服务，f#程序b|
|`SE.P.TMCore.ThickClient`|厚客户端|桌面GUI应用程序，WPF, WinForms, Electron |
|`SE.P.TMCore.BrowserClient`|浏览器客户端| spa， JavaScript应用，WebAssembly |
|`SE.P.TMCore.WebServer`| Web Server | IIS, Apache, Nginx, Express, Kestrel |
| Web应用| ASP. NET, Django, Rails, Spring MVC |
|`SE.P.TMCore.WebSvc`| Web服务| REST api、SOAP、GraphQL端点|
|`SE.P.TMCore.VM`|虚拟机|虚拟机、容器、Docker |
|`SE.P.TMCore.Win32Service`| Win32服务| Windows服务，`ServiceBase`|
|`SE.P.TMCore.KernelThread`|内核线程|内核模块，驱动，ring-0代码|
|`SE.P.TMCore.Modern`| Windows Store进程| UWP应用，Windows Store应用，沙盒应用|
|浏览器和ActiveX插件|浏览器扩展，ActiveX， BHO插件|
|`SE.P.TMCore.NonMS`|应用程序运行在非Microsoft操作系统上| Linux应用程序，macOS应用程序，Unix进程|1.2外部交互器类型

| TMT ID |名称|识别|的代码模式|--------|------|---------------------------|
|`SE.EI.TMCore.Browser`|浏览器|浏览器客户端、用户代理、web UI消费者|
|`SE.EI.TMCore.AuthProvider`|授权提供商| OAuth服务器、OIDC提供商、IdP、SAML |
|`SE.EI.TMCore.WebSvc`|外部Web服务|外部api、供应商服务、SaaS端点|
|`SE.EI.TMCore.User`|人类用户|最终用户、操作员、管理员|
|`SE.EI.TMCore.Megaservice`| Megaservice |大型云平台（Azure、AWS、GCP服务）|
|`SE.EI.TMCore.WebApp`|外部Web应用|第三方Web应用，外部门户|
|`SE.EI.TMCore.CRT`| Windows运行时| WinRT api， Windows运行时组件|
|`SE.EI.TMCore.NFX`| Windows。. NET运行时|。. NET Framework， CLR, BCL |
|`SE.EI.TMCore.WinRT`| Windows RT运行时| Windows RT平台，ARM Windows应用|

1.3数据存储类型

| TMT ID |名称|识别|的代码模式|--------|------|---------------------------|
|`SE.DS.TMCore.CloudStorage`|云存储| Azure Blob、S3、GCS |
|`SE.DS.TMCore.SQL`| SQL数据库| PostgreSQL， MySQL, SQL Server, SQLite |
|`SE.DS.TMCore.NoSQL`|非关系型数据库| MongoDB、CosmosDB、Redis、Cassandra |
|`SE.DS.TMCore.FS`|文件系统|本地文件、NFS、共享驱动器|
|`SE.DS.TMCore.Cache`|缓存| Redis， Memcached，内存缓存|
|`SE.DS.TMCore.ConfigFile`|配置文件|`.env`，`appsettings.json`， YAML配置|
|`SE.DS.TMCore.Cookie`| Cookies | HTTP Cookies，会话Cookies |
|`SE.DS.TMCore.Registry`|注册表Hive | Windows注册表，系统配置存储|
|`SE.DS.TMCore.HTML5LS`| HTML5本地存储|`localStorage`，`sessionStorage`, IndexedDB |
|`SE.DS.TMCore.Device`|设备|硬件设备，USB，外围存储|

1.4数据流类型

| TMT ID |名称|识别|的代码模式|--------|------|---------------------------|
|`SE.DF.TMCore.HTTP`| HTTP |`fetch()`,`axios`,`HttpClient`， REST无TLS |
|`SE.DF.TMCore.HTTPS`| HTTPS | tls安全REST，`https://`端点|
|`SE.DF.TMCore.Binary`|二进制| gRPC， Protobuf，原始二进制协议|
|`SE.DF.TMCore.NamedPipe`|命名管道|通过命名管道|进行IPC
|`SE.DF.TMCore.SMB`| SMB |SMB/CIFS文件共享|
|`SE.DF.TMCore.UDP`| UDP | UDP套接字，数据报协议|
|`SE.DF.TMCore.SSH`| SSH | SSH隧道、SFTP、SCP |
|`SE.DF.TMCore.LDAP`| LDAP | LDAP查询，AD查找|
|`SE.DF.TMCore.LDAPS`| LDAPS |基于TLS的安全LDAP |
|`SE.DF.TMCore.IPsec`| IPsec | VPN隧道，IPsec安全连接|
|`SE.DF.TMCore.RPC`| RPC或DCOM | COM+， DCOM， RPC调用，WCF网。tcp |
|`SE.DF.TMCore.ALPC`| ALPC |高级本地过程调用，Windows IPC |
|`SE.DF.TMCore.IOCTL`| IOCTL接口|设备I/O控制，驱动通讯|

信任边界类型

* *边界行:* *

| TMT ID |名称|代码指示灯||--------|------|-----------------|
|`SE.TB.L.TMCore.Internet`|互联网边界|公共端点、API网关|
|`SE.TB.L.TMCore.Machine`|机器边界|进程边界，虚拟机分离|
|`SE.TB.L.TMCore.Kernel`|Kernel/User模式|驱动，环0/3转换|
|`SE.TB.L.TMCore.AppContainer`| AppContainer | UWP沙箱，应用容器|

* *边境界限:* *

| TMT ID |名称|代码指示灯||--------|------|-----------------|
|`SE.TB.B.TMCore.CorpNet`| CorpNet |企业网络，VPN外围|
|`SE.TB.B.TMCore.Sandbox`|沙盒|沙盒执行环境|
|`SE.TB.B.TMCore.IEB`| IE浏览器边界| IE区域、IE安全设置|
|`SE.TB.B.TMCore.NonIEB`|其他浏览器边界| Chrome， Firefox， Edge安全上下文|

---

# # 2。信任边界检测

当代码交叉时创建信任边界（`subgraph`）：

|边界类型|代码指标||---------------|-----------------|
| **Internet/Public** |公共端点、API网关、负载均衡器|
| **机器** |进程边界，主机分离|
| **Kernel/User模式** |内核调用，驱动程序，系统调用|
| **AppContainer** | UWP沙箱，容器化应用|
| **CorpNet** |企业网络外围，VPN |
| **沙盒** |沙盒执行环境|

---

# # 3。数据流检测

寻找以下模式来识别流：

|流程类型|代码模式||-----------|---------------|
| **HTTP/HTTPS** |`fetch()`,`axios`,`HttpClient`， REST调用|
| **SQL数据库** | ORM查询，SQL连接，`DbContext`|
| **消息队列** |Pub/sub，队列send/receive， Daprpub/sub|
| **文件I/O** |文件read/write， blobupload/download|
| **gRPC** | Protobuf调用，gRPC流|
| **命名管道** | IPC通过命名管道|
| **SSH** | SSH隧道、SFTP、SCP传输|
| **LDAP/LDAPS** |目录查询，AD查找|

---

# # 4。代码分析检查表

在分析代码时，系统地识别：

1. **入口点**→外部交互器+入站流
- API控制器，事件处理程序，webhook端点

2. **Services/Logic**→进程
-业务逻辑类、服务层、工人

3. **数据访问**→数据存储+数据流
-存储库类，数据库上下文，缓存客户端4. **外部调用**→外部交互器+出站流
- HTTP客户端，SDK集成，第三方api

5. **安全边界**→信任边界
-认证中间件、网段、部署单元

6. **Kubernetes Pod Composition**→Sidecar co-location
-查找舵图，k8舱单，部署清单
-常用侧车：Dapr， MISE, Envoy, Istio proxy, linkd，日志采集器
- **应用`diagram-conventions.md`规则1** -注释主机节点，永远不要创建独立的sidecar节点

---

# # 5。命名约定

请参阅[diagram-conventions.md]（./diagram-conventions.md）命名约定一节，了解带有引用规则的完整表。

---

# # 6。输出文件

生成**两个文件**以获得最大的灵活性：

###文件1:Pure Mermaid （`.mmd`）
-只有原始美人鱼代码，没有降价包装
—用于：CLI工具、编辑器、CI/CD、直接渲染文件2:Markdown （`.md`）
-美人鱼在` `'`mermaid `代码围栏
-包括元素、流程和边界汇总表
-用于：GitHub，VS Code，文档

格式比较

|格式|扩展|内容|最适合||--------|-----------|----------|----------|
|纯美人鱼|`.mmd`|原始图代码| CLI，编辑器，工具|
| Markdown |`.md`|图+表| GitHub，文档，查看|