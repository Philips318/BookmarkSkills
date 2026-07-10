---
name: azure-static-web-apps
description: Helps create, configure, and deploy Azure Static Web Apps using the SWA CLI. Use when deploying static sites to Azure, setting up SWA local development, configuring staticwebapp.config.json, adding Azure Functions APIs to SWA, or setting up GitHub Actions CI/CD for Static Web Apps.
---
# #概述

Azure静态Web应用程序（SWA）承载静态前端和可选的无服务器API后端。SWA CLI （`swa`）提供本地开发模拟和部署功能。

* *主要特点:* *
-本地模拟器与API代理和认证仿真
-框架自动检测和配置
-直接部署到Azure
-数据库连接支持

配置文件:* * * *
-`swa-cli.config.json`- CLI设置，**由`swa init`**创建（从不手动创建）
-`staticwebapp.config.json`-运行时配置（路由，认证，报头，API运行时）-可以手动创建

##一般使用说明

# # #的安装```bash
npm install -D @azure/static-web-apps-cli
```
验证:`npx swa --version`###快速启动工作流

**重要：始终使用`swa init`来创建配置文件。不要手动创建`swa-cli.config.json`.**

1.`swa init`- **必需的第一步-自动检测框架并创建`swa-cli.config.json`2.`swa start`-在`http://localhost:4280`上运行本地模拟器
3.`swa login`-使用Azure进行身份验证
4.`swa deploy`-部署到Azure

配置文件

**swa-cli.config.json** -由`swa init`创建，不需要手动创建：
-运行`swa init`交互式设置与框架检测
—执行`swa init --yes`命令接受自动检测的默认值
—初始化后，只对生成的文件进行编辑，以便自定义设置

生成的配置示例（仅供参考）：```json
{
  "$schema": "https://aka.ms/azure/static-web-apps-cli/schema",
  "configurations": {
    "app": {
      "appLocation": ".",
      "apiLocation": "api",
      "outputLocation": "dist",
      "appBuildCommand": "npm run build",
      "run": "npm run dev",
      "appDevserverUrl": "http://localhost:3000"
    }
  }
}
```
**staticwebapp.config.json**（在应用程序源或输出文件夹中）-该文件可以手动创建用于运行时配置：```json
{
  "navigationFallback": {
    "rewrite": "/index.html",
    "exclude": ["/images/*", "/css/*"]
  },
  "routes": [
    { "route": "/api/*", "allowedRoles": ["authenticated"] }
  ],
  "platform": {
    "apiRuntime": "node:20"
  }
}
```
##命令行参考

### swa登录

使用Azure进行身份验证以进行部署。```bash
swa login                              # Interactive login
swa login --subscription-id <id>       # Specific subscription
swa login --clear-credentials          # Clear cached credentials
```
**标志：**`--subscription-id, -S`|`--resource-group, -R`|`--tenant-id, -T`|`--client-id, -C`|`--client-secret, -CS`|`--app-name, -n`### swa init

基于现有的前端和（可选的）API配置新的SWA项目。自动检测框架。```bash
swa init                    # Interactive setup
swa init --yes              # Accept defaults
```
### swa build

构建前端and/orAPI。```bash
swa build                   # Build using config
swa build --auto            # Auto-detect and build
swa build myApp             # Build specific configuration
```
**标志：**`--app-location, -a`|`--api-location, -i`|`--output-location, -O`|`--app-build-command, -A`|`--api-build-command, -I`### swa start

启动本地开发模拟器。```bash
swa start                                    # Serve from outputLocation
swa start ./dist                             # Serve specific folder
swa start http://localhost:3000              # Proxy to dev server
swa start ./dist --api-location ./api        # With API folder
swa start http://localhost:3000 --run "npm start"  # Auto-start dev server
```
**框架通用端口：**
|框架|端口||-----------|------|
|React/Vue/Next.js| 3000 |
| Angular | 4200 |
| Vite | 5173 |

* *关键标志:* *
-`--port, -p`-模拟器端口（默认：4280）
-`--api-location, -i`- API文件夹路径
—`--api-port, -j`—API端口（默认为7071）
-`--run, -r`—启动dev server命令
-`--open, -o`-自动打开浏览器
-`--ssl, -s`-开启HTTPS

### swa部署

部署到Azure静态Web应用程序。```bash
swa deploy                              # Deploy using config
swa deploy ./dist                       # Deploy specific folder
swa deploy --env production             # Deploy to production
swa deploy --deployment-token <TOKEN>   # Use deployment token
swa deploy --dry-run                    # Preview without deploying
```
**获取部署令牌：**
- Azure门户：静态Web应用→概述→管理部署令牌
—命令行：`swa deploy --print-token`—环境变量：`SWA_CLI_DEPLOYMENT_TOKEN`* *关键标志:* *
-`--env`-目标环境（`preview`或`production`）
-`--deployment-token, -d`-部署令牌
-`--app-name, -n`- Azure SWA资源名称

### swa db

初始化数据库连接。```bash
swa db init --database-type mssql
swa db init --database-type postgresql
swa db init --database-type cosmosdb_nosql
```
# #场景

从现有的前端和后端创建SWA

**总是在`swa start`或`swa deploy`之前运行`swa init`。不需要手动创建`swa-cli.config.json`.**```bash
# 1. Install CLI
npm install -D @azure/static-web-apps-cli

# 2. Initialize - REQUIRED: creates swa-cli.config.json with auto-detected settings
npx swa init              # Interactive mode
# OR
npx swa init --yes        # Accept auto-detected defaults

# 3. Build application (if needed)
npm run build

# 4. Test locally (uses settings from swa-cli.config.json)
npx swa start

# 5. Deploy
npx swa login
npx swa deploy --env production
```
添加Azure功能后端

1. **创建API文件夹：**```bash
mkdir api && cd api
func init --worker-runtime node --model V4
func new --name message --template "HTTP trigger"
```
2. **函数示例** (`api/src/functions/message.js`)：```javascript
const { app } = require('@azure/functions');

app.http('message', {
    methods: ['GET', 'POST'],
    authLevel: 'anonymous',
    handler: async (request) => {
        const name = request.query.get('name') || 'World';
        return { jsonBody: { message: `Hello, ${name}!` } };
    }
});
```
3. **设置API运行时间**在`staticwebapp.config.json`：```json
{
  "platform": { "apiRuntime": "node:20" }
}
```
4. **更新`swa-cli.config.json`的CLI配置**：```json
{
  "configurations": {
    "app": { "apiLocation": "api" }
  }
}
```
5. 测试本地* *:* *```bash
npx swa start ./dist --api-location ./api
# Access API at http://localhost:4280/api/message
```
**支持的API运行时：**`node:18`，`node:20`,`node:22`,`dotnet:8.0`,`dotnet-isolated:8.0`,`python:3.10`,`python:3.11`设置GitHub Actions部署

1. **在Azure Portal或通过Azure CLI创建SWA资源**
2. **链接GitHub仓库** -工作流自动生成，或手动创建：`.github/workflows/azure-static-web-apps.yml`:```yaml
name: Azure Static Web Apps CI/CD

on:
  push:
    branches: [main]
  pull_request:
    types: [opened, synchronize, reopened, closed]
    branches: [main]

jobs:
  build_and_deploy:
    if: github.event_name == 'push' || (github.event_name == 'pull_request' && github.event.action != 'closed')
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      - name: Build And Deploy
        uses: Azure/static-web-apps-deploy@v1
        with:
          azure_static_web_apps_api_token: ${{ secrets.AZURE_STATIC_WEB_APPS_API_TOKEN }}
          repo_token: ${{ secrets.GITHUB_TOKEN }}
          action: upload
          app_location: /
          api_location: api
          output_location: dist

  close_pr:
    if: github.event_name == 'pull_request' && github.event.action == 'closed'
    runs-on: ubuntu-latest
    steps:
      - uses: Azure/static-web-apps-deploy@v1
        with:
          azure_static_web_apps_api_token: ${{ secrets.AZURE_STATIC_WEB_APPS_API_TOKEN }}
          action: close
```
3. **添加secret:**复制部署令牌到存储库secret`AZURE_STATIC_WEB_APPS_API_TOKEN`工作流设置:* * * *
-`app_location`-前端源路径
-`api_location`- API源路径
-`output_location`-构建的输出文件夹
-`skip_app_build: true`-跳过如果预构建
-`app_build_command`-自定义构建命令

# #故障排除

|问题|解决方案||-------|----------|
添加`navigationFallback`和`rewrite: "/index.html"`到`staticwebapp.config.json`|
|验证`api`文件夹结构，确保设置了`platform.apiRuntime`，检查函数导出|
|验证`output_location`与实际构建输出目录|匹配
|使用`/.auth/login/<provider>`访问认证模拟器UI |
| api在`/api/*`下同源；外部api需要CORS头|
|部署令牌过期|在Azure Portal中重新生成→静态Web应用程序→管理部署令牌|
|确保`staticwebapp.config.json`在`app_location`或`output_location`|中
|本地API超时|默认为45秒；优化函数或检查阻塞调用|

* *调试命令:* *```bash
swa start --verbose log        # Verbose output
swa deploy --dry-run           # Preview deployment
swa --print-config             # Show resolved configuration
```
