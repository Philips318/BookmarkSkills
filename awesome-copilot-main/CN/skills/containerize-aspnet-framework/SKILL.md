---
name: containerize-aspnet-framework
description: 'Containerize an ASP.NET .NET Framework project by creating Dockerfile and .dockerfile files customized for the project.'
---
# ASP。网。. NET框架容器化提示符

将ASP容器化。净(。. NET Framework)项目，在下面的容器化设置中指定，专门关注应用程序在Windows Docker容器中运行所需的更改。容器化应该考虑这里指定的所有设置。

**记住：**这是一个。. NET Framework应用程序，而不是。净的核心。集装箱化过程将不同于a。. NET Core应用程序。

## Containerization设置提示符的这一部分包含容器化ASP所需的特定设置和配置。净(。. NET Framework)应用程序。在运行此提示之前，请确保设置中填写了必要的信息。注意，在许多情况下，只需要前几个设置。如果以后的设置不适用于容器化的项目，则可以保留默认设置。

任何未指定的设置将被设置为默认值。默认值在`[square brackets]`中提供。

项目基本信息
1. 集装箱化项目：
——`[ProjectName (provide path to .csproj file)]`2. 使用的Windows Server SKU：
——`[Windows Server Core (Default) or Windows Server Full]`3. 使用的Windows Server版本：
——`[2022, 2019, or 2016 (Default 2022)]`4. 自定义Docker镜像构建阶段的基本镜像（“None”表示使用标准的Microsoft基本镜像）：
——`[Specify base image to use for build stage (Default None)]`5. 自定义Docker镜像运行阶段的基本镜像（“None”表示使用标准的Microsoft基本镜像）：
——`[Specify base image to use for run stage (Default None)]`###容器配置
1. 必须在容器映像中公开的端口：
—HTTP主端口：`[e.g., 80]`—附加端口：`[List any additional ports, or "None"]`2. 容器运行的用户帐号如下：
——`[User account, or default to "ContainerUser"]`3. 必须在容器映像中配置的IIS设置：
——`[List any specific IIS settings, or "None"]`构建配置
1. 在构建容器映像之前必须执行的自定义构建步骤：
——`[List any specific build steps, or "None"]`2. 在构建容器映像后必须执行的自定义构建步骤：
——`[List any specific build steps, or "None"]`# # #依赖性
1。。. NET程序集应该在容器映像的GAC中注册：
——`[Assembly name and version, or "None"]`2. 必须复制到容器映像并安装的msi：
——`[MSI names and versions, or "None"]`3. 必须在容器镜像中注册的COM组件：
——`[COM component names, or "None"]`###系统配置
1. 必须添加到容器映像中的注册表项和值：
——`[Registry paths and values, or "None"]`2. 必须在容器映像中设置的环境变量：
——`[Variable names and values, or "Use defaults"]`3. 必须安装在容器映像中的Windows Server角色和特性：
——`[Role/feature names, or "None"]`###文件系统
1.Files/directories需要复制到容器镜像：
——`[Paths relative to project root, or "None"]`-目标在容器中的位置：`[Container paths, or "Not applicable"]`2.Files/directories排除在集装箱运输之外：
——`[Paths to exclude, or "None"]`# # #。dockerignore配置
1. 要包含在`.dockerignore`文件中的模式(。Dockerignore已经有了通用的默认值；这些是额外的模式)：
-附加图案：`[List any additional patterns, or "None"]`健康检查配置
1. 运行状况检查端点：
——`[Health check URL path, or "None"]`2. 健康检查间隔和超时：
——`[Interval and timeout values, or "Use defaults"]`附加说明
1. 集装箱化项目必须遵循的其他指示：
——`[Specific requirements, or "None"]`2. 需要解决的已知问题：
——`[Describe any known issues, or "None"]`# #范围

-✅应用配置修改，以确保配置生成器用于读取应用设置和连接字符串从环境变量
-✅ASP. file的创建和配置网络应用程序
-✅在Dockerfile中指定多个阶段到build/publish应用程序，并将输出复制到最终镜像中
-✅配置Windows容器平台兼容性（Windows Server Core或Full）
-✅正确处理依赖关系（GAC组件，msi， COM组件）
-❌没有基础设施设置（假设单独处理）
-❌除了容器化所需的代码更改之外，没有其他代码更改

##执行流程1. 查看上面的集装箱化设置以了解集装箱化需求
2. 创建一个`progress.md`文件来跟踪带有复选标记的更改
3. 确定。. NET框架版本从项目的。通过检查`TargetFrameworkVersion`元素
4. 根据以下内容选择合适的Windows Server容器映像：
-那个。从项目中检测到的。NET框架版本
-在containerization设置中指定的Windows Server SKU （Core或Full）
—在容器化设置中指定的Windows Server版本（2016、2019或2022）
- Windows Server Core标签可以在：https://github.com/microsoft/dotnet-framework-docker/blob/main/README.aspnet.md#full-tag-listing找到
5. 确保已安装所需的NuGet软件包。**不要**安装这些，如果他们丢失。如果未安装，则用户必须手动安装。如果没有安装，请暂停执行此提示，并要求用户使用Visual Studio NuGet Pac安装它们kage Manager或Visual Studio包管理器控制台。以下包是必需的：
——`Microsoft.Configuration.ConfigurationBuilders.Environment`6. 修改`web.config`文件，添加配置构建器部分和设置，以便从环境变量中读取应用程序设置和连接字符串：
-在configSections中增加ConfigBuilders部分
—在根目录中添加configBuilders部分
—为appSettings和connectionStrings配置EnvironmentConfigBuilder
-示例模式：     ```xml
     <configSections>
       <section name="configBuilders" type="System.Configuration.ConfigurationBuildersSection, System.Configuration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a" restartOnExternalChanges="false" requirePermission="false" />
     </configSections>
     <configBuilders>
       <builders>
         <add name="Environment" type="Microsoft.Configuration.ConfigurationBuilders.EnvironmentConfigBuilder, Microsoft.Configuration.ConfigurationBuilders.Environment" />
       </builders>
     </configBuilders>
     <appSettings configBuilders="Environment">
       <!-- existing app settings -->
     </appSettings>
     <connectionStrings configBuilders="Environment">
       <!-- existing connection strings -->
     </connectionStrings>
     ```
7. 在将要创建Dockerfile的文件夹中创建一个`LogMonitorConfig.json`文件，方法是在提示结束时复制引用的`LogMonitorConfig.json`文件。文件的内容**绝对不能**不能修改，并且应该与引用内容完全匹配，除非在容器化设置中另有说明。
-特别是，要确保要记录的问题的级别没有改变，因为使用`Information`级别的EventLog源将导致不必要的噪音。
8. 在项目目录的根目录下创建一个Dockerfile，将应用程序容器化
- Dockerfile应该使用多个阶段：     - Build stage: Use a Windows Server Core image to build the application
       - The build stage MUST use a `mcr.microsoft.com/dotnet/framework/sdk` base image unless a custom base image is specified in the settings file
       - Copy sln, csproj, and packages.config files first
       - Copy NuGet.config if one exists and configure any private feeds
       - Restore NuGet packages       
       - Then, copy the rest of the source code and build and publish the application to C:\publish using MSBuild
     - Final stage: Use the selected Windows Server image to run the application
       - The final stage MUST use a `mcr.microsoft.com/dotnet/framework/aspnet` base image unless a custom base image is specified in the settings file
       - Copy the `LogMonitorConfig.json` file to a directory in the container (e.g., C:\LogMonitor)
       - Download LogMonitor.exe from the Microsoft repository to the same directory
           - The correct LogMonitor.exe URL is: https://github.com/microsoft/windows-container-tools/releases/download/v2.1.1/LogMonitor.exe
       - Set the working directory to C:\inetpub\wwwroot
       - Copy the published output from the build stage (in C:\publish) to the final image
       - Set the container's entry point to run LogMonitor.exe with ServiceMonitor.exe to monitor the IIS service
           - `ENTRYPOINT [ "C:\\LogMonitor\\LogMonitor.exe", "C:\\ServiceMonitor.exe", "w3svc" ]`
-确保考虑集装箱化设置中的所有要求：     - Windows Server SKU and version
     - Exposed ports
     - User account for container
     - IIS settings
     - GAC assembly registration
     - MSI installation
     - COM component registration
     - Registry keys
     - Environment variables
     - Windows roles and features
     - File/directory copying
—按照本提示最后提供的示例建模Dockerfile，但要确保它是根据具体项目的需求和设置定制的。
- **重要：**使用Windows Server核心基本映像，除非用户在设置文件中**特别要求**完整的Windows Server映像
9. 在项目目录的根目录下创建一个`.dockerignore`文件，以从Docker镜像中排除不必要的文件。`.dockerignore`文件**必须**至少包括以下元素以及在容器化设置中指定的其他模式：
-包/
- - - - - - bin /
- obj /
——.dockerignore
——Dockerfile
- . /
- .github /
- .vs /
- .vscode /
- * * / node_modules /
- * .user
- * .suo
- * * /。DS_Store
- * * / Thumbs.db
-在集装箱化设置中指定的任何其他模式
10. 如果在设置中指定，则配置运行状况检查：—如果提供了健康检查端点，在Dockerfile中添加HEALTHCHECK指令
11. 将dockerfile添加到项目中，在项目文件中添加如下内容：`<None Include="Dockerfile" />`12. 将任务标记为已完成：[]→[✓]
13. 继续，直到所有任务完成，Docker构建成功构建和运行时验证

一旦Dockerfile完成，确认Docker构建成功。使用以下命令构建Docker镜像：```bash
docker build -t aspnet-app:latest .
```
如果构建失败，检查错误消息并对Dockerfile或项目配置进行必要的调整。报告success/failure.进度跟踪

使用以下结构维护`progress.md`文件：```markdown
# Containerization Progress

## Environment Detection
- [ ] .NET Framework version detection (version: ___)
- [ ] Windows Server SKU selection (SKU: ___)
- [ ] Windows Server version selection (Version: ___)

## Configuration Changes
- [ ] Web.config modifications for configuration builders
- [ ] NuGet package source configuration (if applicable)
- [ ] Copy LogMonitorConfig.json and adjust if required by settings

## Containerization
- [ ] Dockerfile creation
- [ ] .dockerignore file creation
- [ ] Build stage created with SDK image
- [ ] sln, csproj, packages.config, and (if applicable) NuGet.config copied for package restore
- [ ] Runtime stage created with runtime image
- [ ] Non-root user configuration
- [ ] Dependency handling (GAC, MSI, COM, registry, additional files, etc.)
- [ ] Health check configuration (if applicable)
- [ ] Special requirements implementation

## Verification
- [ ] Review containerization settings and make sure that all requirements are met
- [ ] Docker build success
```
在步骤之间不要暂停确认。有条不紊地继续，直到应用程序被容器化并且Docker构建成功。

**你没有完成，直到所有的复选框被标记！**这包括成功构建Docker镜像，并解决在构建过程中出现的任何问题。

##参考资料

###示例Dockerfile

一个用于ASP的Dockerfile示例。净(。. NET Framework)应用程序使用Windows Server Core基础映像。```dockerfile
# escape=`
# The escape directive changes the escape character from \ to `
# This is especially useful in Windows Dockerfiles where \ is the path separator

# ============================================================
# Stage 1: Build and publish the application
# ============================================================

# Base Image - Select the appropriate .NET Framework version and Windows Server Core version
# Possible tags include:
# - 4.8.1-windowsservercore-ltsc2025 (Windows Server 2025)
# - 4.8-windowsservercore-ltsc2022 (Windows Server 2022)
# - 4.8-windowsservercore-ltsc2019 (Windows Server 2019)
# - 4.8-windowsservercore-ltsc2016 (Windows Server 2016)
# - 4.7.2-windowsservercore-ltsc2019 (Windows Server 2019)
# - 4.7.2-windowsservercore-ltsc2016 (Windows Server 2016)
# - 4.7.1-windowsservercore-ltsc2016 (Windows Server 2016)
# - 4.7-windowsservercore-ltsc2016 (Windows Server 2016)
# - 4.6.2-windowsservercore-ltsc2016 (Windows Server 2016)
# - 3.5-windowsservercore-ltsc2025 (Windows Server 2025)
# - 3.5-windowsservercore-ltsc2022 (Windows Server 2022)
# - 3.5-windowsservercore-ltsc2019 (Windows Server 2019)
# - 3.5-windowsservercore-ltsc2019 (Windows Server 2016)
# Uses the .NET Framework SDK image for building the application
FROM mcr.microsoft.com/dotnet/framework/sdk:4.8-windowsservercore-ltsc2022 AS build
ARG BUILD_CONFIGURATION=Release

# Set the default shell to PowerShell
SHELL ["powershell", "-command"]

WORKDIR /app

# Copy the solution and project files
COPY YourSolution.sln .
COPY YourProject/*.csproj ./YourProject/
COPY YourOtherProject/*.csproj ./YourOtherProject/

# Copy packages.config files
COPY YourProject/packages.config ./YourProject/
COPY YourOtherProject/packages.config ./YourOtherProject/

# Restore NuGet packages
RUN nuget restore YourSolution.sln

# Copy source code
COPY . .

# Perform custom pre-build steps here, if needed

# Build and publish the application to C:\publish
RUN msbuild /p:Configuration=$BUILD_CONFIGURATION `
            /p:WebPublishMethod=FileSystem `
            /p:PublishUrl=C:\publish `
            /p:DeployDefaultTarget=WebPublish

# Perform custom post-build steps here, if needed

# ============================================================
# Stage 2: Final runtime image
# ============================================================

# Base Image - Select the appropriate .NET Framework version and Windows Server Core version
# Possible tags include:
# - 4.8.1-windowsservercore-ltsc2025 (Windows Server 2025)
# - 4.8-windowsservercore-ltsc2022 (Windows Server 2022)
# - 4.8-windowsservercore-ltsc2019 (Windows Server 2019)
# - 4.8-windowsservercore-ltsc2016 (Windows Server 2016)
# - 4.7.2-windowsservercore-ltsc2019 (Windows Server 2019)
# - 4.7.2-windowsservercore-ltsc2016 (Windows Server 2016)
# - 4.7.1-windowsservercore-ltsc2016 (Windows Server 2016)
# - 4.7-windowsservercore-ltsc2016 (Windows Server 2016)
# - 4.6.2-windowsservercore-ltsc2016 (Windows Server 2016)
# - 3.5-windowsservercore-ltsc2025 (Windows Server 2025)
# - 3.5-windowsservercore-ltsc2022 (Windows Server 2022)
# - 3.5-windowsservercore-ltsc2019 (Windows Server 2019)
# - 3.5-windowsservercore-ltsc2019 (Windows Server 2016)
# Uses the .NET Framework ASP.NET image for running the application
FROM mcr.microsoft.com/dotnet/framework/aspnet:4.8-windowsservercore-ltsc2022

# Set the default shell to PowerShell
SHELL ["powershell", "-command"]

WORKDIR /inetpub/wwwroot

# Copy from build stage
COPY --from=build /publish .

# Add any additional environment variables needed for your application (uncomment and modify as needed)
# ENV KEY=VALUE

# Install MSI packages (uncomment and modify as needed)
# COPY ./msi-installers C:/Installers
# RUN Start-Process -Wait -FilePath 'msiexec.exe' -ArgumentList '/i', 'C:\Installers\your-package.msi', '/quiet', '/norestart'

# Install custom Windows Server roles and features (uncomment and modify as needed)
# RUN dism /Online /Enable-Feature /FeatureName:YOUR-FEATURE-NAME

# Add additional Windows features (uncomment and modify as needed)
# RUN Add-WindowsFeature Some-Windows-Feature; `
#    Add-WindowsFeature Another-Windows-Feature

# Install MSI packages if needed (uncomment and modify as needed)
# COPY ./msi-installers C:/Installers
# RUN Start-Process -Wait -FilePath 'msiexec.exe' -ArgumentList '/i', 'C:\Installers\your-package.msi', '/quiet', '/norestart'

# Register assemblies in GAC if needed (uncomment and modify as needed)
# COPY ./assemblies C:/Assemblies
# RUN C:\Windows\Microsoft.NET\Framework64\v4.0.30319\gacutil -i C:/Assemblies/YourAssembly.dll

# Register COM components if needed (uncomment and modify as needed)
# COPY ./com-components C:/Components
# RUN regsvr32 /s C:/Components/YourComponent.dll

# Add registry keys if needed (uncomment and modify as needed)
# RUN New-Item -Path 'HKLM:\Software\YourApp' -Force; `
#     Set-ItemProperty -Path 'HKLM:\Software\YourApp' -Name 'Setting' -Value 'Value'

# Configure IIS settings if needed (uncomment and modify as needed)
# RUN Import-Module WebAdministration; `
#     Set-ItemProperty 'IIS:\AppPools\DefaultAppPool' -Name somePropertyName -Value 'SomePropertyValue'; `
#     Set-ItemProperty 'IIS:\Sites\Default Web Site' -Name anotherPropertyName -Value 'AnotherPropertyValue'

# Expose necessary ports - By default, IIS uses port 80
EXPOSE 80
# EXPOSE 443  # Uncomment if using HTTPS

# Copy LogMonitor from the microsoft/windows-container-tools repository
WORKDIR /LogMonitor
RUN curl -fSLo LogMonitor.exe https://github.com/microsoft/windows-container-tools/releases/download/v2.1.1/LogMonitor.exe

# Copy LogMonitorConfig.json from local files
COPY LogMonitorConfig.json .

# Set non-administrator user
USER ContainerUser

# Override the container's default entry point to take advantage of the LogMonitor
ENTRYPOINT [ "C:\\LogMonitor\\LogMonitor.exe", "C:\\ServiceMonitor.exe", "w3svc" ]
```
##调整这个例子

**注：**可根据集装箱化设置中的具体要求自定义此模板。

当适应这个例子Dockerfile：

1. 用您实际的文件名替换`YourSolution.sln`、`YourProject.csproj`等
2. 调整Windows Server和。. NET框架版本
3. 根据您的需求修改依赖项安装步骤，并删除任何不必要的步骤
4. 根据需要为您的特定工作流添加或删除阶段

##舞台命名注意事项

-`AS stage-name`语法给每个阶段一个名称
—使用`--from=stage-name`从上一阶段复制文件
-你可以有多个中间阶段，不使用在最终图像### LogMonitorConfig.json
应该在项目目录的根目录中创建LogMonitorConfig.json文件。它用于配置LogMonitor工具，该工具监视容器中的日志。这个文件的内容应该完全像这样，以确保正确的日志功能：```json
{
  "LogConfig": {
    "sources": [
      {
        "type": "EventLog",
        "startAtOldestRecord": true,
        "eventFormatMultiLine": false,
        "channels": [
          {
            "name": "system",
            "level": "Warning"
          },
          {
            "name": "application",
            "level": "Error"
          }
        ]
      },
      {
        "type": "File",
        "directory": "c:\\inetpub\\logs",
        "filter": "*.log",
        "includeSubdirectories": true,
        "includeFileNames": false
      },
      {
        "type": "ETW",
        "eventFormatMultiLine": false,
        "providers": [
          {
            "providerName": "IIS: WWW Server",
            "providerGuid": "3A2A4E84-4C21-4981-AE10-3FDA0D9B0F83",
            "level": "Information"
          },
          {
            "providerName": "Microsoft-Windows-IIS-Logging",
            "providerGuid": "7E8AD27F-B271-4EA2-A783-A47BDE29143B",
            "level": "Information"
          }
        ]
      }
    ]
  }
}
```
