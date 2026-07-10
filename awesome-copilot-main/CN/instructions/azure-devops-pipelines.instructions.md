---
description: 'Best practices for Azure DevOps Pipeline YAML files'
applyTo: '**/azure-pipelines.yml, **/azure-pipelines*.yml, **/*.pipeline.yml'
---
# Azure DevOps Pipeline YAML最佳实践

##一般指引

-使用YAML语法，适当缩进（2个空格）
-始终包含管道、阶段、作业和步骤的有意义的名称和显示名称
—正确的错误处理和条件执行
-使用变量和参数使管道可重用和可维护
—业务连接和权限遵循最小权限原则
-包括全面的日志和诊断故障

##管道结构

-使用阶段组织复杂的管道，以便更好地可视化和控制
-使用作业对相关步骤进行分组，并在可能的情况下启用并行执行
-在阶段和作业之间实现适当的依赖关系
-为可重用的管道组件使用模板
-保持管道文件集中和模块化-将大型管道拆分为多个文件构建最佳实践

—为了保持一致性，请使用指定的代理池版本和虚拟机镜像
-缓存依赖项（npm, NuGet， Maven等）以提高构建性能
-使用有意义的名称和保留策略实现适当的工件管理
-使用版本号和构建元数据的构建变量
-包括代码质量检查（检查，测试，安全扫描）
-确保构建是可复制的和独立于环境的

测试集成

-将单元测试作为构建过程的一部分运行
-以标准格式发布测试结果（JUnit， VSTest等）
-包括代码覆盖率报告和质量检验关
-在适当的阶段实施集成和端到端测试
-使用测试影响分析来优化测试执行
-快速失败测试失败提供快速反馈

##安全考虑-使用Azure密钥库进行敏感配置和机密
—对可变组进行适当的保密管理
—以最小的权限使用服务连接
-启用安全扫描（依赖漏洞、静态分析）
-实施生产部署的审批闸门
-尽可能使用受管理的身份，而不是服务主体

##部署策略

-实施适当的环境提升（开发→登台→生产）
—使用具有适当环境目标的部署作业
—在适当的情况下实施蓝绿色或金丝雀部署策略
—包括回滚机制和健康检查
-使用基础设施作为代码（ARM, Bicep, Terraform）进行一致的部署
—根据环境进行适当的配置管理

变量和参数管理-使用变量组跨管道共享配置
—实现运行时参数，实现灵活的流水线执行
—根据分支或环境使用条件变量
—保护敏感变量并将其标记为机密
-记录可变目的和期望值
—复杂的变量逻辑使用变量模板

性能优化

-适当时使用并行作业和矩阵策略
-为依赖项和构建输出实现适当的缓存策略
-当不需要完整的历史记录时，对Git操作使用浅克隆
-优化Docker镜像构建与多阶段构建和层缓存
-监控管道性能并优化瓶颈
-有效地使用管道资源触发器

监控和可观察性-包括在整个管道中进行全面的记录
-使用Azure Monitor和Application Insights进行部署跟踪
-针对失败和成功实施适当的通知策略
—包括部署运行状况检查和自动回滚触发器
-使用流水线分析来识别改进机会
-记录管道行为和故障排除步骤

模板和可重用性

-为常见模式创建管道模板
-使用扩展模板来完成管道继承
-为可重用的任务序列实现步骤模板
—复杂的变量逻辑使用变量模板
-版本模板适当的稳定性
—文档模板参数及使用示例

分支和触发策略—针对不同的分支类型实现相应的触发器
-使用路径过滤器，仅在相关文件更改时触发构建
—为main/master分支配置合适的CI/CD触发器
-使用拉请求触发器进行代码验证
—执行维护任务的定时触发器
—考虑多存储库场景下的资源触发器

##示例结构```yaml
# azure-pipelines.yml
trigger:
  branches:
    include:
      - main
      - develop
  paths:
    exclude:
      - docs/*
      - README.md

variables:
  - group: shared-variables
  - name: buildConfiguration
    value: 'Release'

stages:
  - stage: Build
    displayName: 'Build and Test'
    jobs:
      - job: Build
        displayName: 'Build Application'
        pool:
          vmImage: 'ubuntu-latest'
        steps:
          - task: UseDotNet@2
            displayName: 'Use .NET SDK'
            inputs:
              version: '8.x'
          
          - task: DotNetCoreCLI@2
            displayName: 'Restore dependencies'
            inputs:
              command: 'restore'
              projects: '**/*.csproj'
          
          - task: DotNetCoreCLI@2
            displayName: 'Build application'
            inputs:
              command: 'build'
              projects: '**/*.csproj'
              arguments: '--configuration $(buildConfiguration) --no-restore'

  - stage: Deploy
    displayName: 'Deploy to Staging'
    dependsOn: Build
    condition: and(succeeded(), eq(variables['Build.SourceBranch'], 'refs/heads/main'))
    jobs:
      - deployment: DeployToStaging
        displayName: 'Deploy to Staging Environment'
        environment: 'staging'
        strategy:
          runOnce:
            deploy:
              steps:
                - download: current
                  displayName: 'Download drop artifact'
                  artifact: drop
                - task: AzureWebApp@1
                  displayName: 'Deploy to Azure Web App'
                  inputs:
                    azureSubscription: 'staging-service-connection'
                    appType: 'webApp'
                    appName: 'myapp-staging'
                    package: '$(Pipeline.Workspace)/drop/**/*.zip'
```
要避免的常见反模式

—直接在YAML文件中硬编码敏感值
-使用过于宽泛的触发器，导致不必要的构建
-在单个阶段中混合构建和部署逻辑
-没有实现正确的错误处理和清理
—使用已弃用的任务版本，没有升级计划
-创建难以维护的单片管道
-没有使用正确的命名约定
—忽略管道安全最佳实践