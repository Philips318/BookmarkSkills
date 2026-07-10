#故障排除-诊断和常见问题

---

##诊断代码

Aspire针对常见问题发布诊断代码。这些出现在构建warnings/errors和IDE诊断中。

标准诊断

|代码|级别|描述|| ------------- | -------- | ---------------------------------------------------------- |
| **ASPIRE001** |警告|资源名称中包含非法字符|
| **ASPIRE002** |警告|检测到重复的资源名|
| **ASPIRE003** |错误|缺少所需的包引用|
| **ASPIRE004** |警告| API使用不当
| **ASPIRE005** |错误|终端配置无效|
| **ASPIRE006** |警告|未为`.WaitFor()`|配置健康检查
| **ASPIRE007** |警告|容器图像标签未指定（使用`latest`） |
| **ASPIRE008** |错误|资源图|检测到循环依赖

###实验诊断（ASPIREHOSTINGX\*）

这些代码表示experimental/previewapi的用法。如果您有意使用实验性功能，它们可能需要`#pragma warning disable`或`<NoWarn>`：|代码|区域|| ------------------------- | -------------------------------- |
| ASPIRE_HOSTINGX_0001-0005 |实验性托管api
| aspre_hostingx_0006 - 0010 |实验性集成api |
| aspre_hostingx_0011 - 0015 |实验性部署接口|
| aspre_hostingx_0016 - 0022 |实验资源模型api |

抑制实验警告：```xml
<!-- In .csproj -->
<PropertyGroup>
  <NoWarn>$(NoWarn);ASPIRE_HOSTINGX_0001</NoWarn>
</PropertyGroup>
```
或二:```csharp
#pragma warning disable ASPIRE_HOSTINGX_0001
var resource = builder.AddExperimentalResource("test");
#pragma warning restore ASPIRE_HOSTINGX_0001
```
---

##常见问题和解决方案

容器运行时

|解决方案|| --------------------------------- | ------------------------------------------------------------------------------------------------------ |
|启动Docker桌面/ Podman / Rancher桌面|
|容器启动失败|检查`docker ps -a`是否有退出码；检查仪表盘控制台日志|
|端口已被使用|另一个进程正在使用该端口；Aspire是自动分配的，但是`targetPort`在容器|上必须是空闲的
|容器镜像提取失败|检查网络连接；验证图像名称和标记|
| Linux上的“Permission denied” |添加用户到`docker`组：`sudo usermod -aG docker $USER`|

服务发现

|解决方案|| ----------------------------- | ---------------------------------------------------------------------------- |
|服务无法找到依赖|验证`.WithReference()`在AppHost；检查仪表板|中的环境变量
|连接字符串为空|参考资源名称不匹配；查看`ConnectionStrings__<name>`|
|检查`targetPort`与实际服务监听端口|
|环境变量未设置|重建AppHost；验证资源名称与|完全匹配

Python工作负载

|解决方案|| --------------------------------- | --------------------------------------------------------------- |
| “Python未找到” |确保Python在PATH上；在`AddPythonApp()`|中指定全路径
| venv未找到|使用`.WithVirtualEnvironment()`或手动创建venv |
| pip包安装|失败使用`.WithPipPackages()`或在`aspire run`|之前安装在venv
| ModuleNotFoundError | venv未激活；`.WithVirtualEnvironment()`处理这个|
检查`targetPort`-另一个实例可能正在运行|

JavaScript / TypeScript工作负载

|解决方案|| ----------------------------- | ---------------------------------------------------------------- |
|使用`.WithNpmPackageInstallation()`自动安装|
|检查`package.json`是否有效；检查NPM注册表连通性|
验证`vite`是否在devDependencies中；检查Vite配置|
确保`targetPort`与你的JS框架配置|中的端口匹配
TypeScript编译错误|这些发生在服务中，而不是Aspire -检查服务日志|

### Go工作负载

|解决方案|| -------------------------- | ---------------------------------------------------------- |
| "go not found" |确保go已安装在PATH |上
|检查工作目录|中是否存在`go.mod`|验证`workingDir`指向`main.go`|所在的目录

Java工作负载

|解决方案|| ------------------------ | ------------------------------------------------------- |
| "java not found" |确保JDK已安装，`JAVA_HOME`设置为|
|Maven/Gradle构建失败|验证构建文件存在检查构建工具安装|
| Spring Boot不会启动|检查`application.properties`；验证主类|

Rust工作负载

|解决方案|| -------------------- | -------------------------------------------------------------------- |
| "cargo not found" |通过Rust安装|
| Rust编译时间正常；将`.WithCargoBuild()`用于预构建|

###健康检查和启动

|解决方案|| ---------------------------- | ------------------------------------------------------------------------------ |
|资源卡在“正在启动”中，|运行状况检查端点没有响应；查看业务日志|
|`.WaitFor()`timeout |增加超时或修复运行状况端点；默认值为30秒
|验证端点路径（默认：`/health`）；检查业务绑定到正确的端口|
|级联启动失败|依赖失败；首先检查根资源|

# # #仪表盘

|解决方案|| ------------------------------------- | ------------------------------------------------------------------------- |
|检查终端的URL；使用`--dashboard-port`作为固定端口|
|服务可能没有写入到stdout/stderr；检查控制台输出|
|没有非-的痕迹。在服务中配置OpenTelemetry SDK；参见[仪表盘](dashboard.md) |
|传播跟踪上下文头（`traceparent`,`tracestate`） |

构建和配置

|解决方案|| ----------------------------------------- | ------------------------------------------------------------------- |
| "Project not found" for`AddProject<T>()`|确保`.csproj`在解决方案中并被AppHost |引用
|软件包版本冲突|将所有Aspire软件包绑定到同一个版本|
| AppHost不会构建|检查`Aspire.AppHost.Sdk`在项目中；执行`dotnet restore`|命令
|先修复构建错误；`aspire run`需要一个成功的构建|

# # #部署

|解决方案|| ---------------------------------------- | -------------------------------------------------------------------- |
|`aspire publish`失败|检查publisher包是否安装（如`Aspire.Hosting.Docker`） |
|生成的肱二头肌错误|检查不支持的资源配置|
|容器镜像推送失败|验证注册表凭据和权限|
|部署中缺少连接字符串|检查生成的ConfigMaps/Secrets匹配资源名|

---

##调试策略

# # # 1。先检查仪表盘

仪表板显示资源状态、日志、跟踪和度量。任何问题都从这里开始。

# # # 2。检查环境变量

在指示板中，单击资源以查看所有注入的环境变量。验证连接字符串和服务url是否正确。

# # # 3。读取控制台日志

仪表板→控制台日志→按故障资源过滤。原始stdout/stderr通常包含根本原因。# # # 4。检查DAG

如果服务启动失败，请检查依赖顺序。失败的依赖会阻塞所有下游资源。

# # # 5。使用MCP进行ai辅助调试

如果配置了MCP（参见[MCP服务器](mcp-server.md)），请询问您的AI助手：

-“哪些资源正在失效？”
-“显示[服务]的日志”
-“什么痕迹显示错误？”

# # # 6。隔离问题

通过在AppHost中注释掉其他资源，只运行失败的资源。这缩小了问题是资源本身还是依赖项的范围。

---

##寻求帮助

|频道| URL || ----------------------- | ---------------------------------------------- |
| GitHub问题（运行时）|https://github.com/dotnet/aspire/issues|
GitHub问题（文档）|https://github.com/microsoft/aspire.dev/issues|
|不和|https://aka.ms/aspire/discord|
|堆叠溢出|标签：`dotnet-aspire`|
| Reddit |https://www.reddit.com/r/aspiredotdev/|