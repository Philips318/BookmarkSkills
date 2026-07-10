#部署-完成参考

Aspire将“编排”（运行什么）与“部署”（在哪里运行）分开。`aspire publish`命令将AppHost资源模型转换为目标平台的部署清单。

---

##发布vs部署

|概念|它做什么||---|---|
| **`aspire publish`** |生成部署构件（Dockerfiles, Helm charts， Bicep等）|
通过CI/CD管道|运行生成的构件

Aspire不直接部署。它生成清单——您部署它们。

---

##支持的目标

# # #码头工人

* *包:* *`Aspire.Hosting.Docker````bash
aspire publish -p docker -o ./docker-output
```
生成:
-`docker-compose.yml`-与AppHost匹配的服务定义
-每个为`Dockerfile`。网项目
—环境变量配置
-卷挂载
—网络配置```csharp
// AppHost configuration for Docker publishing
var api = builder.AddProject<Projects.Api>("api")
    .PublishAsDockerFile();  // override default publish behavior
```
# # # Kubernetes

* *包:* *`Aspire.Hosting.Kubernetes````bash
aspire publish -p kubernetes -o ./k8s-output
```
生成:
- Kubernetes YAML清单（部署，服务，ConfigMaps，秘密）
-舵图（可选）
—入口配置
—基于AppHost配置的资源限制```csharp
// AppHost: customize K8s publishing
var api = builder.AddProject<Projects.Api>("api")
    .WithReplicas(3)                    // maps to K8s replicas
    .WithExternalHttpEndpoints();       // maps to Ingress/LoadBalancer
```
Azure容器应用程序

* *包:* *`Aspire.Hosting.Azure.AppContainers````bash
aspire publish -p azure -o ./azure-output
```
生成:
- Azure容器应用环境的Bicep模板
-每个服务的容器应用定义
- Azure容器注册表配置
—受管理的身份配置
- Dapr组件（如果使用Dapr集成）
- VNET配置```csharp
// AppHost: Azure-specific configuration
var api = builder.AddProject<Projects.Api>("api")
    .WithExternalHttpEndpoints()        // maps to external ingress
    .WithReplicas(3);                   // maps to min replicas

// Azure resources are auto-provisioned
var storage = builder.AddAzureStorage("storage");   // creates Storage Account
var cosmos = builder.AddAzureCosmosDB("cosmos");    // creates Cosmos DB account
var sb = builder.AddAzureServiceBus("messaging");   // creates Service Bus namespace
```
Azure应用服务

* *包:* *`Aspire.Hosting.Azure.AppService````bash
aspire publish -p appservice -o ./appservice-output
```
生成:
-应用程序服务计划和Web应用程序的二头肌模板
-连接字符串配置
-应用程序设置

---

资源模型到部署映射

| AppHost概念| Docker撰写| Kubernetes | Azure容器应用||---|---|---|---|
|`AddProject<T>()`|`service`与Dockerfile |`Deployment`+`Service`|`Container App`|
|`AddContainer()`|`service`与`image:`|`Deployment`+`Service`|`Container App`|
|`AddRedis()`|`service: redis`|`StatefulSet`|托管Redis |
|`AddPostgres()`|`service: postgres`|`StatefulSet`| Azure PostgreSQL |
|`.WithReference()`|`environment:`vars |`ConfigMap`/`Secret`|应用设置|
|`.WithReplicas(n)`|`deploy: replicas: n`|`replicas: n`|`minReplicas: n`|
|`.WithVolume()`|`volumes:`|`PersistentVolumeClaim`| Azure文件|
|`.WithHttpEndpoint()`|`ports:`|`Service`端口|入口|
|`.WithExternalHttpEndpoints()`|`ports:`（主机）|`Ingress`/`LoadBalancer`|外部入口|
|`.env`文件|`Secret`|密钥库参考|

---CI/CD集成

###GitHub Actions示例```yaml
name: Deploy
on:
  push:
    branches: [main]

jobs:
  deploy:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4

      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '10.0.x'

      - name: Install Aspire CLI
        run: curl -sSL https://aspire.dev/install.sh | bash

      - name: Generate manifests
        run: aspire publish -p azure -o ./deploy

      - name: Deploy to Azure
        uses: azure/arm-deploy@v2
        with:
          template: ./deploy/main.bicep
          parameters: ./deploy/main.parameters.json
```
Azure DevOps示例```yaml
trigger:
  branches:
    include: [main]

pool:
  vmImage: 'ubuntu-latest'

steps:
  - task: UseDotNet@2
    inputs:
      version: '10.0.x'

  - script: curl -sSL https://aspire.dev/install.sh | bash
    displayName: 'Install Aspire CLI'

  - script: aspire publish -p azure -o $(Build.ArtifactStagingDirectory)/deploy
    displayName: 'Generate deployment manifests'

  - task: AzureResourceManagerTemplateDeployment@3
    inputs:
      deploymentScope: 'Resource Group'
      templateLocation: '$(Build.ArtifactStagingDirectory)/deploy/main.bicep'
```
---

特定于环境的配置

###为秘密使用参数```csharp
// AppHost
var dbPassword = builder.AddParameter("db-password", secret: true);
var postgres = builder.AddPostgres("db", password: dbPassword);
```
在部署:
—**Docker:**从`.env`文件加载
**Kubernetes:**从`Secret`资源加载
- **Azure:**通过托管身份从密钥库加载

条件资源```csharp
// Use Azure services in production, emulators locally
if (builder.ExecutionContext.IsPublishMode)
{
    var cosmos = builder.AddAzureCosmosDB("cosmos");    // real Azure resource
}
else
{
    var cosmos = builder.AddAzureCosmosDB("cosmos")
        .RunAsEmulator();                                // local emulator
}
```
---

##开发容器和GitHub代码空间

Aspire模板包括`.devcontainer/`配置：```json
{
  "name": "Aspire App",
  "image": "mcr.microsoft.com/devcontainers/dotnet:10.0",
  "features": {
    "ghcr.io/devcontainers/features/docker-in-docker:2": {},
    "ghcr.io/devcontainers/features/node:1": {}
  },
  "postCreateCommand": "curl -sSL https://aspire.dev/install.sh | bash",
  "forwardPorts": [18888],
  "portsAttributes": {
    "18888": { "label": "Aspire Dashboard" }
  }
}
```
端口转发在代码空间中自动工作——仪表板和所有服务端点都可以通过转发的url访问。