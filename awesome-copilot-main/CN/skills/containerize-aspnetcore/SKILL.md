---
name: containerize-aspnetcore
description: 'Containerize an ASP.NET Core project by creating Dockerfile and .dockerfile files customized for the project.'
---
# ASP。. NET Core Docker容器化提示符

集装箱化请求

将ASP容器化。.NET Core （.NET）项目在下面的设置中指定，专门关注应用程序在Linux Docker容器中运行所需的更改。容器化应该考虑这里指定的所有设置。

遵守集装箱运输的最佳实践。. NET Core应用程序，确保容器在性能、安全性和可维护性方面进行了优化。

## Containerization设置提示符的这一部分包含容器化ASP所需的特定设置和配置。. NET Core应用程序。在运行此提示之前，请确保设置中填写了必要的信息。注意，在许多情况下，只需要前几个设置。如果以后的设置不适用于容器化的项目，则可以保留默认设置。

任何未指定的设置将被设置为默认值。默认值在`[square brackets]`中提供。

项目基本信息
1. 集装箱化项目：
——`[ProjectName (provide path to .csproj file)]`2。。. NET版本使用：
——`[8.0 or 9.0 (Default 8.0)]`3. 使用的Linux发行版：
——`[debian, alpine, ubuntu, chiseled, or Azure Linux (mariner) (Default debian)]`4. 自定义Docker镜像构建阶段的基本镜像（“None”表示使用标准的Microsoft基本镜像）：
——`[Specify base image to use for build stage (Default None)]`5. 自定义Docker镜像运行阶段的基本镜像（“None”表示使用标准的Microsoft基本镜像）：
——`[Specify base image to use for run stage (Default None)]`###容器配置
1. 必须在容器映像中公开的端口：
—HTTP主端口：`[e.g., 8080]`—附加端口：`[List any additional ports, or "None"]`2. 容器运行的用户帐号如下：
——`[User account, or default to "$APP_UID"]`3. 应用URL配置：
——`[Specify ASPNETCORE_URLS, or default to "http://+:8080"]`构建配置
1. 在构建容器映像之前必须执行的自定义构建步骤：
——`[List any specific build steps, or "None"]`2. 在构建容器映像后必须执行的自定义构建步骤：
——`[List any specific build steps, or "None"]`3. 必须配置的NuGet包源：
——`[List any private NuGet feeds with authentication details, or "None"]`# # #依赖性
1. 必须安装在容器镜像中的系统包：
——`[Package names for the chosen Linux distribution, or "None"]`2. 必须复制到容器镜像的本地库：
——`[Library names and paths, or "None"]`3. 额外的。必须安装的。NET工具：
——`[Tool names and versions, or "None"]`###系统配置
1. 必须在容器映像中设置的环境变量：
——`[Variable names and values, or "Use defaults"]`###文件系统
1.Files/directories需要复制到容器镜像：
——`[Paths relative to project root, or "None"]`-目标在容器中的位置：`[Container paths, or "Not applicable"]`2.Files/directories排除在集装箱运输之外：
——`[Paths to exclude, or "None"]`3. 应该配置的卷挂载点：
——`[Volume paths for persistent data, or "None"]`# # #。dockerignore配置
1. 要包含在`.dockerignore`文件中的模式(。Dockerignore已经有了通用的默认值；这些是额外的模式)：
-附加图案：`[List any additional patterns, or "None"]`健康检查配置
1. 运行状况检查端点：
——`[Health check URL path, or "None"]`2. 健康检查间隔和超时：
——`[Interval and timeout values, or "Use defaults"]`附加说明
1. 集装箱化项目必须遵循的其他指示：
——`[Specific requirements, or "None"]`2. 需要解决的已知问题：
——`[Describe any known issues, or "None"]`# #范围-✅修改应用配置，确保应用设置和连接字符串可以从环境变量中读取
-✅ASP. file的创建和配置核心应用
-✅在Dockerfile中指定多个阶段到build/publish应用程序，并将输出复制到最终镜像中
-✅Linux容器平台兼容性配置（Alpine、Ubuntu、Chiseled或Azure Linux (Mariner)）
-✅正确处理依赖关系（系统包，本机库，附加工具）
-❌没有基础设施设置（假设单独处理）
-❌除了容器化所需的代码更改之外，没有其他代码更改

##执行流程1. 查看上面的集装箱化设置以了解集装箱化需求
2. 创建一个`progress.md`文件来跟踪带有复选标记的更改
3. 确定。. NET版本。通过检查`TargetFramework`元素
4. 根据以下内容选择合适的Linux容器镜像：
-那个。从项目检测到的。NET版本
—在容器化设置中指定的Linux发行版（Alpine, Ubuntu， Chiseled或Azure Linux (Mariner)）
如果用户没有在容器化设置中请求特定的基本镜像，那么基本镜像必须是有效的mcr.microsoft.com/dotnet镜像，并带标签，如下面的Dockerfile示例或文档中所示
-微软官方。. NET映像用于构建和运行时阶段：      - SDK image tags (for build stage): https://github.com/dotnet/dotnet-docker/blob/main/README.sdk.md
      - ASP.NET Core runtime image tags: https://github.com/dotnet/dotnet-docker/blob/main/README.aspnet.md
      - .NET runtime image tags: https://github.com/dotnet/dotnet-docker/blob/main/README.runtime.md
5. 在项目目录的根目录下创建一个Dockerfile，将应用程序容器化
- Dockerfile应该使用多个阶段：     - Build stage: Use a .NET SDK image to build the application
       - Copy csproj file(s) first
       - Copy NuGet.config if one exists and configure any private feeds
       - Restore NuGet packages
       - Then, copy the rest of the source code and build and publish the application to /app/publish
     - Final stage: Use the selected .NET runtime image to run the application
       - Set the working directory to /app
       - Set the user as directed (by default, to a non-root user (e.g., `$APP_UID`))
         - Unless directed otherwise in containerization settings, a new user does *not* need to be created. Use the `$APP_UID` variable to specify the user account.
       - Copy the published output from the build stage to the final image
-确保考虑集装箱化设置中的所有要求：     - .NET version and Linux distribution
     - Exposed ports
     - User account for container
     - ASPNETCORE_URLS configuration
     - System package installation
     - Native library dependencies
     - Additional .NET tools
     - Environment variables
     - File/directory copying
     - Volume mount points
     - Health check configuration
6. 在项目目录的根目录下创建一个`.dockerignore`文件，以从Docker镜像中排除不必要的文件。`.dockerignore`文件**必须**至少包括以下元素以及在容器化设置中指定的其他模式：
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
7. 如果在容器化设置中指定，则配置运行状况检查：
—如果提供了健康检查端点，在Dockerfile中添加HEALTHCHECK指令
-使用curl或wget检查运行状况端点
8. 将任务标记为已完成：[]→[✓]
9. 继续，直到所有任务完成，Docker构建成功

构建和运行时验证一旦Dockerfile完成，确认Docker构建成功。使用以下命令构建Docker镜像：```bash
docker build -t aspnetcore-app:latest .
```
如果构建失败，检查错误消息并对Dockerfile或项目配置进行必要的调整。报告success/failure.进度跟踪

使用以下结构维护`progress.md`文件：```markdown
# Containerization Progress

## Environment Detection
- [ ] .NET version detection (version: ___)
- [ ] Linux distribution selection (distribution: ___)

## Configuration Changes
- [ ] Application configuration verification for environment variable support
- [ ] NuGet package source configuration (if applicable)

## Containerization
- [ ] Dockerfile creation
- [ ] .dockerignore file creation
- [ ] Build stage created with SDK image
- [ ] csproj file(s) copied for package restore
- [ ] NuGet.config copied if applicable
- [ ] Runtime stage created with runtime image
- [ ] Non-root user configuration
- [ ] Dependency handling (system packages, native libraries, tools, etc.)
- [ ] Health check configuration (if applicable)
- [ ] Special requirements implementation

## Verification
- [ ] Review containerization settings and make sure that all requirements are met
- [ ] Docker build success
```
在步骤之间不要暂停确认。有条不紊地继续，直到应用程序被容器化并且Docker构建成功。

**你没有完成，直到所有的复选框被标记！**这包括成功构建Docker镜像，并解决在构建过程中出现的任何问题。

##示例Dockerfile

一个用于ASP的Dockerfile示例。.NET Core （.NET）应用程序使用Linux基础映像。```dockerfile
# ============================================================
# Stage 1: Build and publish the application
# ============================================================

# Base Image - Select the appropriate .NET SDK version and Linux distribution
# Possible tags include:
# - 8.0-bookworm-slim (Debian 12)
# - 8.0-noble (Ubuntu 24.04)
# - 8.0-alpine (Alpine Linux)
# - 9.0-bookworm-slim (Debian 12)
# - 9.0-noble (Ubuntu 24.04)
# - 9.0-alpine (Alpine Linux)
# Uses the .NET SDK image for building the application
FROM mcr.microsoft.com/dotnet/sdk:8.0-bookworm-slim AS build
ARG BUILD_CONFIGURATION=Release

WORKDIR /src

# Copy project files first for better caching
COPY ["YourProject/YourProject.csproj", "YourProject/"]
COPY ["YourOtherProject/YourOtherProject.csproj", "YourOtherProject/"]

# Copy NuGet configuration if it exists
COPY ["NuGet.config", "."]

# Restore NuGet packages
RUN dotnet restore "YourProject/YourProject.csproj"

# Copy source code
COPY . .

# Perform custom pre-build steps here, if needed
# RUN echo "Running pre-build steps..."

# Build and publish the application
WORKDIR "/src/YourProject"
RUN dotnet build "YourProject.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish the application
RUN dotnet publish "YourProject.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Perform custom post-build steps here, if needed
# RUN echo "Running post-build steps..."

# ============================================================
# Stage 2: Final runtime image
# ============================================================

# Base Image - Select the appropriate .NET runtime version and Linux distribution
# Possible tags include:
# - 8.0-bookworm-slim (Debian 12)
# - 8.0-noble (Ubuntu 24.04)
# - 8.0-alpine (Alpine Linux)
# - 8.0-noble-chiseled (Ubuntu 24.04 Chiseled)
# - 8.0-azurelinux3.0 (Azure Linux)
# - 9.0-bookworm-slim (Debian 12)
# - 9.0-noble (Ubuntu 24.04)
# - 9.0-alpine (Alpine Linux)
# - 9.0-noble-chiseled (Ubuntu 24.04 Chiseled)
# - 9.0-azurelinux3.0 (Azure Linux)
# Uses the .NET runtime image for running the application
FROM mcr.microsoft.com/dotnet/aspnet:8.0-bookworm-slim AS final

# Install system packages if needed (uncomment and modify as needed)
# RUN apt-get update && apt-get install -y \
#     curl \
#     wget \
#     ca-certificates \
#     libgdiplus \
#     && rm -rf /var/lib/apt/lists/*

# Install additional .NET tools if needed (uncomment and modify as needed)
# RUN dotnet tool install --global dotnet-ef --version 8.0.0
# ENV PATH="$PATH:/root/.dotnet/tools"

WORKDIR /app

# Copy published application from build stage
COPY --from=build /app/publish .

# Copy additional files if needed (uncomment and modify as needed)
# COPY ./config/appsettings.Production.json .
# COPY ./certificates/ ./certificates/

# Set environment variables
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:8080

# Add custom environment variables if needed (uncomment and modify as needed)
# ENV CONNECTIONSTRINGS__DEFAULTCONNECTION="your-connection-string"
# ENV FEATURE_FLAG_ENABLED=true

# Configure SSL/TLS certificates if needed (uncomment and modify as needed)
# ENV ASPNETCORE_Kestrel__Certificates__Default__Path=/app/certificates/app.pfx
# ENV ASPNETCORE_Kestrel__Certificates__Default__Password=your_password

# Expose the port the application listens on
EXPOSE 8080
# EXPOSE 8081  # Uncomment if using HTTPS

# Install curl for health checks if not already present
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

# Configure health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

# Create volumes for persistent data if needed (uncomment and modify as needed)
# VOLUME ["/app/data", "/app/logs"]

# Switch to non-root user for security
USER $APP_UID

# Set the entry point for the application
ENTRYPOINT ["dotnet", "YourProject.dll"]
```
##调整这个例子

**注：**可根据集装箱化设置中的具体要求自定义此模板。

当适应这个例子Dockerfile：

1. 将`YourProject.csproj`、`YourProject.dll`等替换为您实际的项目名称
2. 调整。NET版本和Linux发行版
3. 根据您的需求修改依赖项安装步骤，并删除任何不必要的步骤
4. 配置特定于应用程序的环境变量
5. 根据需要为您的特定工作流添加或删除阶段
6. 更新运行状况检查端点以匹配应用程序的运行状况检查路由

## Linux发行版的变体

### Alpine Linux
对于较小的映像大小，您可以使用Alpine Linux：```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
# ... build steps ...

FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS final
# Install packages using apk
RUN apk update && apk add --no-cache curl ca-certificates
```
### Ubuntu凿
对于最小的攻击面，考虑使用轮廓分明的图像：```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0-jammy-chiseled AS final
# Note: Chiseled images have minimal packages, so you may need to use a different base for additional dependencies
```
Azure Linux （Mariner）
对于azure优化的容器：```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0-azurelinux3.0 AS final
# Install packages using tdnf
RUN tdnf update -y && tdnf install -y curl ca-certificates && tdnf clean all
```
##舞台命名注意事项

-`AS stage-name`语法为每个阶段提供一个名称
—使用`--from=stage-name`从上一个阶段复制文件
-你可以有多个中间阶段，不使用在最终图像
—`final`阶段将成为最终的容器映像

安全最佳实践

—在生产环境中始终以非root用户运行
-使用特定的图像标签，而不是`latest`—尽量减少安装包的数量
-保持基本镜像更新
-使用多阶段构建从最终映像中排除构建依赖项