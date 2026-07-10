托管代理开发和部署循环

Foundry托管代理由`agent.yaml`（身份：`kind: hosted`、模型、协议、环境变量、资源——模式位于microsoft.github.io/AgentSchema/）、可选的`agent.manifest.yaml`（参数化模板）和`azure.yaml`（azd配置+服务配置）定义。这些文件中的`${VAR}`是一个azd环境变量；`{{ param }}`为初始化模板参数。

内部循环：针对本地REAL代理开发

没有嘲弄。`azure.ai.agents`azd扩展在本地运行实际的托管代理：1.`az login`导入到拥有Foundry项目的租户中（参见下面的403陷阱）。
2.`azd ai agent run`—针对已配置的项目在本地启动代理（默认端口8088）。
3.`azd ai agent invoke --local "<message>"`—一次性测试（`-p responses|invocations`选择协议，`-f payload.json`选择结构化输入）。
4. 通过代码库为direct/local模式使用的任何环境变量，将堆栈的其余部分（AG-UI端点或桥接器）指向本地代理URL，并通过真正的UI练习功能。

内循环纪律:- **在独立验证通过**之间重新启动代理，如果它在内存中播种数据-每个approve/reject都会改变共享状态，因此针对脏进程的第二次测试运行通过或因错误原因失败。
-一个陈旧的代理进程占用端口产生一个令人困惑的超玉米“地址已在使用”的跟踪下一个`azd ai agent run`-先杀死它。
新启动的代理可以在第一次请求时使用`DeploymentNotFound`404，即使存在模型部署（热身片）-在调查之前重试一次。

外循环：部署更新1.`azd deploy`（或`azd up`用于provision + deploy）。代码部署（ZIP） vs容器部署是由`agent.yaml`中的字段选择的；容器构建默认为远程ACR构建——不需要本地Docker。
2. **每次部署都会创建一个新的代理版本。**`azd ai agent show`确认现场直播。固定到特定版本的客户端不会看到更新；客户使用“最新”将。
3. 从行为上验证已部署的代理：发送读取查询并确认后续操作仍暂停等待批准。部署成功输出证明了打包工作成功，仅此而已。

##部署陷阱

|找到了|细节|| --- | --- |
|`azd provision`单独部署占位符| Provision仅创建基础架构；如果没有部署步骤，就会得到hello-world代理。使用`azd up`或按照规定使用`azd deploy`|
|基本镜像必须来自MCR |`az acr build`从Docker Hub匿名拉出达到`toomanyrequests`速率限制。使用`mcr.microsoft.com/...`基础映像|
如果代理代码导入代理目录之外的模块，那么`azure.yaml`中的docker构建上下文必须到达它们——并且azd版本在是否接受父目录`project:`/上下文路径上存在差异（1.27.0时代的回归拒绝`..`）。在azd升级|后测试封装
|宿主容器env |`FOUNDRY_PROJECT_ENDPOINT`和`APPLICATIONINSIGHTS_CONNECTION_STRING`自动注入宿主容器；不要硬编码它们，b|
|托管代理在空闲~15分钟后计算分配；空闲后的第一个请求很慢——不是bug |
|历史复制（响应协议）|平台存储会话历史；如果代理自己的聊天客户端也存储（`store=True`），则转副本。设置client/host选项为不存储|
任何在内存中持有每个线程响应id或会话缓存的服务都必须运行单个副本或外部化缓存|
尽管正确的RBAC通常意味着az CLI的活动subscription/tenant不是项目的，但`Microsoft.MachineLearningServices/workspaces/agents/action`被拒绝。修复`az account set`/`az login --tenant`；没有代码更改|生产前端布线

部署AG-UI端点（架构A/C服务，或依赖于B的托管调用端点），CopilotKit运行时可以到达服务器端；相应地设置运行时的代理URL env变量。与`https://ai.azure.com/.default`用户保持无钥匙（Entra）。浏览器只与CopilotKit运行时路由对话——从不向客户端公开Foundry端点或凭据。