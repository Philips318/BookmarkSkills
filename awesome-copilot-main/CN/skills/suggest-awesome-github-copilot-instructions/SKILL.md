---
name: suggest-awesome-github-copilot-instructions
description: 'Suggest relevant GitHub Copilot instruction files from the awesome-copilot repository based on current repository context and chat history, avoiding duplicates with existing instructions in this repository, and identifying outdated instructions that need updates.'
---
建议Awesome GitHub Copilot指令

分析当前存储库上下文，并从[GitHub awesome-copilot存储库]（https://github.com/github/awesome-copilot/blob/main/docs/README.instructions.md）中建议相关的copilot指令文件，这些文件在此存储库中尚未可用。

# #过程1. **获取可用指令**：从[awesome-copilotREADME.instructions.md]（https://github.com/github/awesome-copilot/blob/main/docs/README.instructions.md）中提取指令列表和描述。必须使用`#fetch`工具。
2. **扫描本地指令**：发现现有的指令文件在`.github/instructions/`文件夹
3. **提取描述**：从本地指令文件中读取前端内容以获得描述和`applyTo`模式
4. **获取远程版本**：对于每个本地指令，使用原始GitHub url（例如，`https://raw.githubusercontent.com/github/awesome-copilot/main/instructions/<filename>`）从awesome-copilot存储库中获取相应的版本
5. **比较版本**：将本地指令内容与远程版本进行比较，以确定：
-最新的指令（完全匹配）
-说明过时（内容不同）
-过时指令的主要差异（描述，applyTo模式，内容）
6. **分析上下文**：查看聊天记录、存储库文件和当前项目需求
7. * *比较Existing**：检查此存储库中已有的说明
8. **匹配相关性**：将可用的指令与已识别的模式和需求进行比较
9. **当前选项**：显示相关的说明，基本原理和可用性状态，包括过时的说明
10. **验证**：确保建议的指令会增加现有指令未涵盖的价值
11. **输出**：提供结构化的表格，建议，描述，以及链接到令人敬畏的副驾驶说明和类似的本地说明
**等待**用户请求继续安装或更新特定说明。不要安装或更新，除非指示这样做。
12. **Download/UpdateAssets**：对于请求的指令，自动：    - Download new instructions to `.github/instructions/` folder
    - Update outdated instructions by replacing with latest version from awesome-copilot
    - Do NOT adjust content of the files
    - Use `#fetch` tool to download assets, but may use `curl` using `#runInTerminal` tool to ensure all content is retrieved
    - Use `#todos` tool to track progress
##上下文分析标准

🔍**存储库模式**：
-使用的编程语言（.cs, .js, .py, .js）。ts,等等)。
-框架指标(ASP。. NET, React, Azure，Next.js等)
-项目类型（web应用程序，api，库，工具）
-开发工作流程需求（测试，CI/CD，部署）

🗨️**聊天记录上下文**：
-最近的讨论和痛点
-特定于技术的问题
-编码标准讨论
-开发工作流程要求

##输出格式

在结构化表中显示分析结果，比较awesome-copilot指令与现有存储库指令。

|棒极了-副驾驶指令|描述|已经安装|类似的本地指令|建议理由||------------------------------|-------------|-------------------|---------------------------|---------------------|
| [blazor.instructions.md](https://github.com/github/awesome-copilot/blob/main/instructions/blazor.instructions.md) | Blazor开发指南|✅是|blazor.instructions.md|已经涵盖了现有的Blazor指令|
| [reactjs.instructions.md](https://github.com/github/awesome-copilot/blob/main/instructions/reactjs.instructions.md) | ReactJS开发标准|❌无|无|将增强已有模式的React开发|
| [java.instructions.md](https://github.com/github/awesome-copilot/blob/main/instructions/java.instructions.md) | Java开发最佳实践|⚠️过时的|java.instructions.md| applyTo模式不同：远程使用`'**/*.java'`与本地`'*.java'`-更新推荐|

本地指令发现过程

1. 列出`instructions/`目录下的所有`*.instructions.md`文件
2. 对于每个发现的文件，读取前内容以提取`description`和`applyTo`模式
3. 构建具有适用文件模式的现有指令的全面清单
4. 使用这个清单来避免重复

版本比较过程1. 对于每个本地指令文件，构造原始的GitHub URL来获取远程版本：
—模式：`https://raw.githubusercontent.com/github/awesome-copilot/main/instructions/<filename>`2. 使用`#fetch`工具获取远程版本
3. 比较整个文件内容（包括标题和正文）
4. 确定具体的差异：
- **前端内容更改**（描述，applyTo模式）
- **内容更新**（指南、示例、最佳实践）
5. 记录过时指令的关键差异
6. 计算相似度以确定是否需要更新

文件结构要求

根据GitHub文档，copilot-instructions文件应该是：
- **库范围指令**:`.github/copilot-instructions.md`（适用于整个库）
- **路径特定指令**:`.github/instructions/NAME.instructions.md`（适用于通过`applyTo`frontmatter的特定文件模式）
- **社区说明**:`instructions/NAME.instructions.md`（用于分享和分发）

前物质结构指令文件在awesome-copilot使用这种前置事项格式：```markdown
---
description: 'Brief description of what this instruction provides'
applyTo: '**/*.js,**/*.ts' # Optional: glob patterns for file matching
---
```
# #要求-使用`githubRepo`工具从awesome-copilot存储库说明文件夹中获取内容
—扫描本地文件系统中“`.github/instructions/`”目录下的现有指令
-从本地指令文件中读取YAML前端内容，提取描述和`applyTo`模式
—将本地指令与远程版本进行比较，检测过时指令
-与此存储库中的现有指令进行比较，以避免重复
-关注当前指令库覆盖的差距
-验证建议的指令是否符合存储库的目的和标准
-为每个建议提供清晰的理由
-包括链接到令人敬畏的副驾驶说明和类似的本地说明
-清楚地识别过时的指令，并注明具体差异
-考虑技术栈兼容性和项目特定需求
-不要在表格之外提供任何额外的信息或背景E和分析##图标参考

-✅已安装和最新
-⚠️已安装但已过时（可更新）
-❌没有安装在回购

##更新处理

当发现过时的指令时：
1. 将它们包含在具有⚠️状态的输出表中
2. 在“建议理由”一栏中记录具体的差异
3. 根据所注意到的关键更改提供更新建议
4. 当用户请求更新时，将整个本地文件替换为远程版本
5. 在`.github/instructions/`目录中保留文件位置