#项目文档插件

生成专业的项目文件与图纸。io架构图和嵌入PNG图像的Word （.docx）输出。适用于任何软件项目-自动发现技术堆栈，架构和代码结构。

# #安装```bash
# Using Copilot CLI
copilot plugin install project-documenter@awesome-copilot
```
它的作用

将**Project documententer **代理指向任何存储库，它会产生：

1. **Markdown文档** -带有嵌入式图表参考的10节项目摘要
2. * *。io图** - C4上下文、管道和组件关系图（`.drawio`+`.drawio.png`）
3. **Word文档** -专业格式的`.docx`与标题页，目录，并嵌入PNG架构图像

包含的内容

# # #代理

|代理|描述||-------|-------------|
|`project-documenter`|生成专业项目文档与draw。io架构图和Word文档输出嵌入图像。自动发现任何项目的技术栈和架构。|

# # #技能

|技能|描述||-------|-------------|
|`drawio`|生成绘图。io图表为`.drawio`文件，并通过捆绑的Node.js脚本导出为PNG。io CLI或无头浏览器)|
转换Markdown到Word （`.docx`）嵌入PNG图像-纯JavaScript，不需要Pandoc |

##如何工作

第一步：发现

代理扫描您的存储库以了解：
-技术栈（`.csproj`、`package.json`、`pom.xml`、`go.mod`等）
架构模式（API、worker service、CLI、库）
-设计模式（工厂、策略、存储库、管道）
-接口、实现、模型、配置
-依赖项，Docker设置，CI/CD步骤2：生成图表

创建3-5个专业图纸。遵循C4模型的io图：

|图| C4级别|显示||---------|----------|-------|
|高级架构|上下文|系统在其环境-上游，下游，外部深度|
|处理管道|容器|内部数据流——入口点→阶段→输出|
|组件关系|组件|接口、实现、工厂、DI图|
|部署（可选）|基础设施| Docker、Kubernetes、可伸缩、云服务|
|数据模型（可选）|组件|Entity/DTO层次结构（如果重要）|

使用捆绑的`drawio-to-png.mjs`脚本将每个图表导出为PNG。

###步骤3：写下Markdown

生成`docs/project-summary.md`，共10节：

1. 执行概要
2. 体系结构概述（带有嵌入式图表）
3. 加工流水线（内嵌图）
4. 核心组件（带嵌入式图表）
5. API契约/消息架构
6. 基础设施与部署
7. 扩展模式
8. 规则和反模式
9. 依赖关系
10. 代码结构步骤4:Word文档

使用捆绑的`md-to-docx.mjs`脚本将Markdown转换为格式化的`.docx`：

-标题页与项目名称，日期，版本，观众
-自动生成的目录
- **PNG图表图像嵌入内联**在Word文档
- Calibri字体，彩色标题，样式表与交替行
-控制台中带有阴影背景的代码块

###步骤5：验证

根据实际代码库抽查类名、文件路径和图的准确性。报告所有生成的文件。

##生成的输出```
docs/
├── project-summary.md                     # Source document (Markdown)
├── project-summary.docx                   # Word document with embedded images
└── diagrams/
    ├── high-level-architecture.drawio     # C4 Context diagram (editable)
    ├── high-level-architecture.drawio.png # Rendered PNG
    ├── processing-pipeline.drawio         # C4 Container diagram
    ├── processing-pipeline.drawio.png
    ├── component-relationships.drawio     # C4 Component diagram
    └── component-relationships.drawio.png
```
# #先决条件

|需求|用途|必选？||-------------|---------|-----------|
|Node.js18+ |运行捆绑导出脚本|是|
| Edge或Chrome |无头浏览器的图表渲染|之一：这或绘制。IO桌面|
|。|可选（浏览器回退可用）| . io desktop |命令行关系图导出(更快的替代方案

技术不可知论者

适用于任何堆栈。代理自动检测：
- * *。净* * (`.csproj``.sln`), Java * * * * (`pom.xml``build.gradle`), * *Node.js* * (`package.json`), * * Python * * (`pyproject.toml`), * * * * (`go.mod`), * *锈* * (`Cargo.toml`)
- Docker, Kubernetes,GitHub Actions, GitLab CI
-任何消息系统（SQS, RabbitMQ, Kafka， Azure服务总线）
-任何数据库ORM （EF, Hibernate, Prisma, SQLAlchemy）

# #源

这个插件是[Awesome Copilot]（https://github.com/github/awesome-copilot）的一部分，这是一个社区驱动的GitHub Copilot扩展集合。

# #许可证

麻省理工学院