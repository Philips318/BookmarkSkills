---
name: msstore-cli
description: 'Microsoft Store Developer CLI (msstore) for publishing Windows applications to the Microsoft Store. Use when asked to configure Store credentials, list Store apps, check submission status, publish submissions, manage package flights, set up CI/CD for Store publishing, or integrate with Partner Center. Supports Windows App SDK/WinUI, UWP, .NET MAUI, Flutter, Electron, React Native, and PWA applications.'
license: MIT
---
# Microsoft Store Developer CLI （msstore）

Microsoft Store Developer CLI （`msstore`）是一个跨平台的命令行接口，用于发布和管理Microsoft Store中的应用程序。它与Partner Center api集成，并支持各种应用程序类型的自动发布工作流。

何时使用此技能

在需要时使用此技能：

—为API访问配置存储凭据
-列出应用程序在您的商店帐户
—查看提交状态
-发布提交到商店
-用于商店提交的软件包应用程序
-为Store发布初始化项目
-管理包裹航班（beta测试）
-为自动商店发布设置CI/CD管道
-管理提交内容的逐步推出
-以编程方式更新提交元数据

# #先决条件—Windows 10+、macOS、Linux
-。. NET 9桌面运行时（Windows）或。. NET 9 Runtime （macOS/Linux）
—Partner Center帐户，具有相应的权限
- Azure AD应用程序注册与合作伙伴中心API访问
-通过以下方法之一安装msstore CLI：
- **微软商店**:[下载]（https://www.microsoft.com/store/apps/9P53PC5S0PHJ）
- **WinGet**:`winget install "Microsoft Store Developer CLI"`- **手册**：从[GitHub发布]下载（https://aka.ms/msstoredevcli/releases）

合作伙伴中心设置

在使用msstore之前，您需要创建具有合作伙伴中心访问权限的Azure AD应用程序：

1. 转到[合作伙伴中心]（https://partner.microsoft.com/dashboard）
2. 导航到**帐户设置** > **用户管理** > **Azure AD应用程序**
3. 创建一个新应用程序，并记录**租户ID**、**客户ID**和**客户秘密**
4. 授予应用程序适当的权限（管理员或开发人员角色）

##核心命令参考

### info -打印配置显示当前凭据配置。```bash
msstore info
```
* *选择:* *

|选项|描述|| ------ | ----------- |
|`-v, --verbose`|打印详细输出|

### reconfigure -配置凭据

配置或更新Microsoft Store API凭据。```bash
msstore reconfigure [options]
```
* *选择:* *

|选项|描述|| ------ | ----------- |
|`-t, --tenantId`| Azure AD租户ID |
|`-s, --sellerId`|合作伙伴中心卖家ID |
|`-c, --clientId`| Azure AD应用客户端ID |
|`-cs, --clientSecret`|客户端鉴权密钥|
|`-ct, --certificateThumbprint`|证书指纹（替代客户端密钥）|
|`-cfp, --certificateFilePath`|证书文件路径（替代客户端密钥）|
|`-cp, --certificatePassword`|证书密码|
|`--reset`|重置凭证而不完全重新配置|

* *例子:* *```bash
# Configure with client secret
msstore reconfigure --tenantId $TENANT_ID --sellerId $SELLER_ID --clientId $CLIENT_ID --clientSecret $CLIENT_SECRET

# Configure with certificate
msstore reconfigure --tenantId $TENANT_ID --sellerId $SELLER_ID --clientId $CLIENT_ID --certificateFilePath ./cert.pfx --certificatePassword MyPassword
```
### settings - CLI设置

修改Microsoft Store Developer命令行设置。```bash
msstore settings [options]
```
* *选择:* *

|选项|描述|| ------ | ----------- |
|`-t, --enableTelemetry`|启用（true）或禁用（false）遥测|

####设置Publisher Display Name```bash
msstore settings setpdn <publisherDisplayName>
```
为`init`命令设置默认的发布者显示名称。

### apps -应用管理

列出并检索应用程序信息。

####应用列表```bash
msstore apps list
```
列出合作伙伴中心帐户中的所有应用程序。

####获取申请详情```bash
msstore apps get <productId>
```
* *参数:* *

|参数|描述|| -------- | ----------- |
|`productId`|商店产品ID（例如，9NBLGGH4R315） |

* *的例子:* *```bash
# Get details of a specific app
msstore apps get 9NBLGGH4R315
```
### submission -提交管理

管理商店提交。

|子命令|描述|| ----------- | ----------- |
|`status`|获取提交状态|
|`get`|获取提交元数据和包信息|
|`getListingAssets`|获取提交|的列出资产
|`updateMetadata`|更新提交元数据|
|`poll`|投票提交状态直到完成|
|`publish`|发布提交|
|`delete`|删除提交|

####获取提交状态```bash
msstore submission status <productId>
```
####获取提交详情```bash
msstore submission get <productId>
```
####更新元数据```bash
msstore submission updateMetadata <productId> <metadata>
```
其中`<metadata>`是包含更新元数据的JSON字符串。因为JSON包含shell解释的字符（引号，大括号等），你必须适当地引用and/or转义值：

**Bash/Zsh**：将JSON包在单引号中，这样shell就可以直接传递它。  ```bash
  msstore submission updateMetadata 9NBLGGH4R315 '{"description":"My updated app"}'
  ```
- **PowerShell**：使用单引号（或者在双引号字符串中使用转义双引号）。  ```powershell
  msstore submission updateMetadata 9NBLGGH4R315 '{"description":"My updated app"}'
  ```
- **cmd.exe**：用反斜杠转义每个内部双引号。  ```cmd
  msstore submission updateMetadata 9NBLGGH4R315 "{\"description\":\"My updated app\"}"
  ```
**提示：**对于复杂或多行元数据，将JSON保存到一个文件中，并传递其内容以避免引用问题：
>“bash
> msstore提交updateMetadata 9NBLGGH4R315 “$(catmetadata.json)”
> ' ' '

* *选择:* *

|选项|描述|| ------ | ----------- |
|`-s, --skipInitialPolling`|跳过初始状态轮询|

####发布提交```bash
msstore submission publish <productId>
```
####民意调查提交```bash
msstore submission poll <productId>
```
轮询，直到提交状态为已发布或失败。

####删除提交```bash
msstore submission delete <productId>
```
* *选择:* *

|选项|描述|| ------ | ----------- |
|`--no-confirm`|跳过确认提示|

### init -初始化存储项目

初始化用于Microsoft Store发布的项目。自动检测项目类型并配置存储标识。```bash
msstore init <pathOrUrl> [options]
```
* *参数:* *

|参数|描述|| -------- | ----------- |
|`pathOrUrl`|项目目录路径或PWA URL |

* *选择:* *

|选项|描述|| ------ | ----------- |
|`-n, --publisherDisplayName`|发布者显示名称|
|`--package`|也打包项目|
|`--publish`|打包并发布（暗含——Package） |
|`-f, --flightId`|发布到特定航班|
|`-prp, --packageRolloutPercentage`|逐步推出百分比（0-100）|
|`-a, --arch`|架构(s): x86、x64、arm64 |
|`-o, --output`| |包的输出目录
|`-ver, --version`|构建|时使用的版本

**支持的项目类型：**

- Windows App SDK / WinUI
——UWP
-。净毛伊岛
——颤振
——电子
- React Native for Desktop
- PWA （Progressive Web Apps）

* *例子:* *```bash
# Initialize WinUI project
msstore init ./my-winui-app

# Initialize PWA
msstore init https://contoso.com --output ./pwa-package

# Initialize and publish
msstore init ./my-app --publish
```
### package -用于存储的包

打包应用程序以供Microsoft Store提交。```bash
msstore package <pathOrUrl> [options]
```
* *参数:* *

|参数|描述|| -------- | ----------- |
|`pathOrUrl`|项目目录路径或PWA URL |

* *选择:* *

|选项|描述|| ------ | ----------- |
|`-o, --output`| |包的输出目录
|`-a, --arch`|架构(s): x86、x64、arm64 |
|`-ver, --version`| |包的版本

* *例子:* *```bash
# Package for default architecture
msstore package ./my-app

# Package for multiple architectures
msstore package ./my-app --arch x64,arm64 --output ./packages

# Package with specific version
msstore package ./my-app --version 1.2.3.0
```
### publish -发布到存储

将应用程序发布到Microsoft Store。```bash
msstore publish <pathOrUrl> [options]
```
* *参数:* *

|参数|描述|| -------- | ----------- |
|`pathOrUrl`|项目目录路径或PWA URL |

* *选择:* *

|选项|描述|| ------ | ----------- |
|`-i, --inputFile`|已有路径。或者。上传文件|
|`-id, --appId`|应用程序ID（如果未初始化）|
|`-nc, --noCommit`|保持提交在草案状态|
|`-f, --flightId`|发布到特定航班|
|`-prp, --packageRolloutPercentage`|逐步推出百分比（0-100）|

* *例子:* *```bash
# Publish project
msstore publish ./my-app

# Publish existing package
msstore publish ./my-app --inputFile ./packages/MyApp.msixupload

# Publish as draft
msstore publish ./my-app --noCommit

# Publish with gradual rollout
msstore publish ./my-app --packageRolloutPercentage 10
```
###航班-包航班管理

管理打包航班（beta测试组）。

|子命令|描述|| ----------- | ----------- |
|`list`|列出一个应用程序的所有航班|
|`get`|获取航班详情|
|`delete`|删除航班|
|`create`|创建新的航班|
|`submission`|管理航班提交|

####航班列表```bash
msstore flights list <productId>
```
####查询航班详情```bash
msstore flights get <productId> <flightId>
```
####创建航班```bash
msstore flights create <productId> <friendlyName> --group-ids <group-ids>
```
* *选择:* *

|选项|描述|| ------ | ----------- |
|`-g, --group-ids`|飞行组id（逗号分隔）|
|`-r, --rank-higher-than`|航班ID排名高于|

####删除航班```bash
msstore flights delete <productId> <flightId>
```
####航班申请```bash
# Get flight submission
msstore flights submission get <productId> <flightId>

# Publish flight submission
msstore flights submission publish <productId> <flightId>

# Check flight submission status
msstore flights submission status <productId> <flightId>

# Poll flight submission
msstore flights submission poll <productId> <flightId>

# Delete flight submission
msstore flights submission delete <productId> <flightId>
```
####航班上线管理```bash
# Get rollout status
msstore flights submission rollout get <productId> <flightId>

# Update rollout percentage
msstore flights submission rollout update <productId> <flightId> <percentage>

# Halt rollout
msstore flights submission rollout halt <productId> <flightId>

# Finalize rollout (100%)
msstore flights submission rollout finalize <productId> <flightId>
```
##通用工作流

工作流程1：首次商店设置```bash
# 1. Install the CLI
winget install "Microsoft Store Developer CLI"

# 2. Configure credentials (get these from Partner Center)
msstore reconfigure --tenantId $TENANT_ID --sellerId $SELLER_ID --clientId $CLIENT_ID --clientSecret $CLIENT_SECRET

# 3. Verify configuration
msstore info

# 4. List your apps to confirm access
msstore apps list
```
###工作流程2：初始化和发布新的应用程序```bash
# 1. Navigate to project
cd my-winui-app

# 2. Initialize for Store (creates/updates app identity)
msstore init .

# 3. Package the application
msstore package . --arch x64,arm64

# 4. Publish to Store
msstore publish .

# 5. Check submission status
msstore submission status <productId>
```
###工作流程3：更新现有应用程序```bash
# 1. Build your updated application
dotnet publish -c Release

# 2. Package and publish
msstore publish ./my-app

# Or publish from existing package
msstore publish ./my-app --inputFile ./artifacts/MyApp.msixupload
```
工作流程4：逐步推出```bash
# 1. Publish with initial rollout percentage
msstore publish ./my-app --packageRolloutPercentage 10

# 2. Monitor and increase rollout
msstore submission poll <productId>

# 3. (After validation) Finalize to 100%
# This completes via Partner Center or submission update
```
###工作流程5：测试与航班```bash
# 1. Create a flight group in Partner Center first
# Then create a flight
msstore flights create <productId> "Beta Testers" --group-ids "group-id-1,group-id-2"

# 2. Publish to the flight
msstore publish ./my-app --flightId <flightId>

# 3. Check flight submission status
msstore flights submission status <productId> <flightId>

# 4. After testing, publish to production
msstore publish ./my-app
```
###工作流6:CI/CD管道集成```yaml
# GitHub Actions example
name: Publish to Store

on:
  release:
    types: [published]

jobs:
  publish:
    runs-on: windows-latest
    steps:
      - uses: actions/checkout@v4
      
      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '9.0.x'
      
      - name: Install msstore CLI
        run: winget install "Microsoft Store Developer CLI" --accept-package-agreements --accept-source-agreements
      
      - name: Configure Store credentials
        run: |
          msstore reconfigure --tenantId ${{ secrets.TENANT_ID }} --sellerId ${{ secrets.SELLER_ID }} --clientId ${{ secrets.CLIENT_ID }} --clientSecret ${{ secrets.CLIENT_SECRET }}
      
      - name: Build application
        run: dotnet publish -c Release
      
      - name: Publish to Store
        run: msstore publish ./src/MyApp
```
与winapp CLI集成

winapp CLI （v0.2.0+）通过`winapp store`子命令与msstore集成：```bash
# These commands are equivalent:
msstore reconfigure --tenantId xxx --clientId xxx --clientSecret xxx
winapp store reconfigure --tenantId xxx --clientId xxx --clientSecret xxx

# List apps
msstore apps list
winapp store apps list

# Publish
msstore publish ./my-app
winapp store publish ./my-app
```
当您需要统一的打包和发布CLI体验时，请使用`winapp store`。

# #故障排除

|问题|解决方案|| ----- | -------- |
|用`msstore info`验证凭据；重新运行`msstore reconfigure`|
| App未找到|确保产品ID正确；执行`msstore apps list`命令验证|
|权限不足|检查合作伙伴中心中的Azure AD应用程序角色（需要Manager或Developer） |
|确保包装符合存储要求；详细信息请查看合作伙伴中心|
|提交卡住|运行`msstore submission poll <productId>`检查状态|
|航班未找到|验证航班ID`msstore flights list <productId>`|
|失效滚出百分比|取值范围为0 ~ 100|
确保URL是可公开访问的，并且具有有效的web应用程序清单|

##环境变量

CLI支持凭据的环境变量：

|变量|描述|| -------- | ----------- |
|`MSSTORE_TENANT_ID`| Azure AD租户ID |
|`MSSTORE_SELLER_ID`|合作伙伴中心卖家ID |
|`MSSTORE_CLIENT_ID`| Azure AD应用客户端ID |
|`MSSTORE_CLIENT_SECRET`|客户端秘密|

# #引用

- [Microsoft Store Developer CLI文档]（https://learn.microsoft.com/windows/apps/publish/msstore-dev-cli/overview）
- [CLI命令参考]（https://learn.microsoft.com/windows/apps/publish/msstore-dev-cli/commands）
- [GitHub Repository]（https://github.com/microsoft/msstore-cli）
-[合作伙伴中心API]（https://learn.microsoft.com/windows/uwp/monetize/using-windows-store-services）
-[应用程序提交API]（https://learn.microsoft.com/windows/uwp/monetize/create-and-manage-submissions-using-windows-store-services）
-[套餐航班概览]（https://learn.microsoft.com/windows/uwp/publish/package-flights）
-[渐进式升级]（https://learn.microsoft.com/windows/uwp/publish/gradual-package-rollout）