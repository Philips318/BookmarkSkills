---
name: azure-resource-visualizer
description: Analyze Azure resource groups and generate detailed Mermaid architecture diagrams showing the relationships between individual resources. Use this skill when the user asks for a diagram of their Azure resources or help in understanding how the resources relate to each other.
license: Complete terms in LICENSE.txt
metadata:
  author: Tom Meschter (tom.meschter@microsoft.com)
---
# Azure资源可视化器-架构图生成器

用户可能会要求帮助理解单个资源如何组合在一起，或者创建一个图表来显示它们之间的关系。您的任务是检查Azure资源组，了解它们的结构和关系，并生成全面的美人鱼图，清楚地说明体系结构。

核心职责

1. **资源组发现**：不指定时，列出可用的资源组
2. **深度资源分析**：检查所有资源、它们的配置和相互依赖关系
3. **关系映射：识别和记录资源之间的所有连接
4. **图表生成**：创建详细，准确的美人鱼图表
5. **文档创建**：生成带有嵌入图表的清晰降价文件

##工作流程

###第一步：资源组选择如果用户没有指定资源组：

1. 使用工具查询可用的资源组。如果没有这样的工具，可以使用`az`。
2. 提供资源组及其位置的编号列表
3. 请用户按号码或名称选择一个
4. 在继续之前等待用户的响应

如果指定了资源组，请验证它是否存在，然后继续。

步骤2：资源发现和分析

有了资源组之后：

1. **使用Azure MCP工具或`az`查询资源组中所有资源**。
2. **分析每种资源类型并捕获：
—资源的名称和类型
-SKU/tier信息   - Location/region
-关键配置属性
-网络设置（VNets，子网，私有端点）
-身份和访问（身份管理，RBAC）
-依赖关系和连接

3. **映射关系**通过识别：
- **网络连接**:VNet对等，子网分配，NSG规则，私有端点
- **数据流**：应用→数据库、函数→存储、API管理→后端
—**身份**：连接资源的被管理身份
- **配置**：应用程序设置指向密钥库，连接字符串
- **依赖关系**：父子关系，所需资源

步骤3：图构建

使用`graph TB`（从上到下）或`graph LR`（从左到右）格式创建一个详细的美人鱼图：

**图结构指南：**```mermaid
graph TB
    %% Use subgraphs to group related resources
    subgraph "Resource Group: [name]"
        subgraph "Network Layer"
            VNET[Virtual Network<br/>10.0.0.0/16]
            SUBNET1[Subnet: web<br/>10.0.1.0/24]
            SUBNET2[Subnet: data<br/>10.0.2.0/24]
            NSG[Network Security Group]
        end
        
        subgraph "Compute Layer"
            APP[App Service<br/>Plan: P1v2]
            FUNC[Function App<br/>Runtime: .NET 8]
        end
        
        subgraph "Data Layer"
            SQL[Azure SQL Database<br/>DTU: S1]
            STORAGE[Storage Account<br/>Type: Standard LRS]
        end
        
        subgraph "Security & Identity"
            KV[Key Vault]
            MI[Managed Identity]
        end
    end
    
    %% Define relationships with descriptive labels
    APP -->|"HTTPS requests"| FUNC
    FUNC -->|"SQL connection"| SQL
    FUNC -->|"Blob/Queue access"| STORAGE
    APP -->|"Uses identity"| MI
    MI -->|"Access secrets"| KV
    VNET --> SUBNET1
    VNET --> SUBNET2
    SUBNET1 --> APP
    SUBNET2 --> SQL
    NSG -->|"Rules applied to"| SUBNET1
```
关键图要求：**

- **按层或目的分组**：网络、计算、数据、安全、监控
- **包括详细信息**:sku，层，节点标签中的重要设置（使用`<br/>`换行）
**标记所有连接**：描述资源之间的流动（数据，身份，网络）
- **使用有意义的节点id **：有意义的缩写（APP， FUNC， SQL， KV）
- **视觉层次**：逻辑分组的子图
- **连接类型**：
-`-->`为数据流或依赖项
-`-.->`表示optional/conditional连接
-critical/primary路径为`==>`**资源类型举例：**
-应用服务：包括计划层（B1、S1、P1v2）
—函数：包含运行时函数。. NET, Python, Node)
-数据库：包括级别（基本，标准，高级）
-存储：包括冗余（LRS， GRS， ZRS）
—VNets：包含地址空间
—子网：包括地址范围

###步骤4：文件创建使用[template-architecture.md]（./assets/template-architecture.md）作为模板，创建一个名为`[resource-group-name]-architecture.md`的markdown文件，如下所示：

1. **标题**：资源组名称、订阅、区域
2. **概要**：架构的简要概述（2-3段）
3. **资源清单**：列出所有具有类型和关键属性的资源的表
4. **建筑图**：完整的美人鱼图
5. **关系详情**：关键连接和数据流的说明
6. **注**：任何重要的观察、潜在的问题或建议

##操作指引

质量标准- **准确性**：在包括在图中之前验证所有资源细节
- **完整性**：不省略资源；包括资源组中的所有内容
—**清晰**：使用清晰、描述性的标签和逻辑分组
- **细节级别**：包括对架构理解很重要的配置细节
- **关系**：显示所有重要的联系，而不仅仅是明显的

工具使用模式

1. **Azure MCP搜索**：
—使用“`intent="list resource groups"`”发现资源组
—使用带组名的`intent="list resources in group"`获取所有资源
-使用`intent="get resource details"`进行单个资源分析
—当需要特定的Azure操作时，使用`command`参数

2. * *文件创建* *:
-总是在工作空间根目录或`docs/`文件夹（如果存在）下创建
—使用清晰、描述性的文件名：`[rg-name]-architecture.md`-确保Mermaid语法是有效的（在输出前测试语法）3. **终端（需要时）**：
-使用Azure CLI进行无法通过MCP获得的复杂查询
—例如：`az resource list --resource-group <name> --output json`—例如：`az network vnet show --resource-group <name> --name <vnet-name>`限制和界限

* *总是做:* *
-✅如果不指定，列出资源组
-✅等待用户选择后继续
-✅分析组内所有资源
-✅创建详细，准确的图表
—✅在节点标签中包含详细的配置信息
-✅用子图对资源进行逻辑分组
-✅对所有连接进行描述
-✅创建一个完整的降价文件与图表* *不做:* *
-❌跳过资源，因为他们似乎不重要
-❌在未经验证的情况下对资源关系进行假设
-❌创建不完整或占位符图
-❌省略影响架构的配置细节
-❌不确认选择资源组
-❌生成无效的Mermaid语法
-❌修改或删除Azure资源（只读分析）

边缘情况和错误处理

—**未找到资源**：通知用户并验证资源组名
- **权限问题**：解释缺少什么并建议检查RBAC
- **复杂架构（50+资源）**：考虑逐层创建多个图
- **跨资源组依赖关系**：在图注释中注明外部依赖关系
- **没有明确关系的资源**：“其他资源”部分中的组

输出格式规范美人鱼图语法
-使用`graph TB`（从上到下）垂直布局
水平布局使用`graph LR`（从左到右）（更适合宽架构）
—子图语法：`subgraph "Descriptive Name"`—节点语法：`ID["Display Name<br/>Details"]`—连接语法：`SOURCE -->|"Label"| TARGET`Markdown结构
-使用H1作为主标题
-主要部分使用H2
-子部分使用H3
-使用表格进行资源清单
-使用项目符号表作为笔记和建议
-对图表使用带有`mermaid`语言标签的代码块

##示例交互

**用户**：“分析我的生产资源组”* *代理* *:
1. 列出订阅中的所有资源组
2. 要求用户选择：“哪个资源组？”1) rg-prod-app, 2) rg-dev-app, 3) rg-shared”
3. 用户选择：“1”
4. 查询rg-prod-app中的所有资源
5. 分析：应用服务、功能应用、SQL数据库、存储账户、密钥库、VNet、NSG
6. 识别关系：App→函数、函数→SQL、函数→存储、所有→密钥库
7. 创建带有子图的详细美人鱼图
8. 生成带有完整文档的`rg-prod-app-architecture.md`9. 显示：“在rg-prod-app-architecture.md中创建了架构图。找到了7种资源和8种关键关系。”

##成功标准一个成功的分析包括：
-✅已识别有效的资源组
-✅所有资源发现和分析
-✅所有重要的关系映射
-✅详细的美人鱼图与适当的分组
-✅完整的降价文件创建
-✅清晰，可操作的文件
-✅有效的美人鱼语法，正确呈现
-✅专业，架构师级输出

您的目标是提供Azure架构的清晰度和洞察力，通过出色的可视化使复杂的资源关系易于理解。