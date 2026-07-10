# AGENTS.md
##项目概述Awesome GitHub Copilot存储库是社区驱动的自定义代理和指令集合，旨在增强跨各种域、语言和用例的GitHub Copilot体验。项目内容包括：

- **代理** -与MCP服务器集成的专用GitHub Copilot代理
- **说明** -应用于特定文件模式的编码标准和最佳实践
- **技能** -独立的文件夹与指令和捆绑的资源为专门的任务
- **Hooks** -在开发过程中由特定事件触发的自动化工作流
- **工作流** -[代理工作流]（https://github.github.com/gh-aw）用于GitHub Actions中的ai驱动的存储库自动化
- **插件** -可安装的包，组相关的代理，命令和技能周围的特定主题

存储库结构```
.
├── agents/           # Custom GitHub Copilot agent definitions (.agent.md files)
├── instructions/     # Coding standards and guidelines (.instructions.md files)
├── skills/           # Agent Skills folders (each with SKILL.md and optional bundled assets)
├── hooks/            # Automated workflow hooks (folders with README.md + hooks.json)
├── workflows/        # Agentic Workflows (.md files for GitHub Actions automation)
├── plugins/          # Installable plugin packages (folders with plugin.json)
├── extensions/       # Canvas extensions (each with extension.mjs and plugin metadata)
├── docs/             # Documentation for different resource types
├── eng/              # Build and automation scripts
└── scripts/          # Utility scripts
```
##设置命令```bash
# Install dependencies
npm ci

# Build the project (generates README.md and marketplace.json)
npm run build

# Validate plugin manifests
npm run plugin:validate

# Generate marketplace.json only
npm run plugin:generate-marketplace

# Create a new plugin
npm run plugin:create -- --name <plugin-name>

# Validate agent skills
npm run skill:validate

# Create a new skill
npm run skill:create -- --name <skill-name>
```
##开发流程

使用代理，指令，技能和钩子

所有代理文件（`*.agent.md`）和指令文件（`*.instructions.md`）必须包含适当的降价前置事项。Agent Skills是包含`SKILL.md`文件的文件夹，该文件带有标题和可选的捆绑资产。钩子是包含一个`README.md`和一个`hooks.json`配置文件的文件夹。

#### Agent Files （\*.agent.md）

-必须有`description`字段（用单引号括起来）
—文件名必须是小写字母，单词之间用连字符分隔
—建议包含“`tools`”字段
—强烈建议指定`model`字段

####指令文件（\*.instructions.md）

-必须有`description`字段（用单引号括起来，不能为空）
-必须有`applyTo`字段指定文件模式（例如，`'**.js, **.ts'`）
—文件名必须是小写字母，单词之间用连字符分隔

####座席技能（Skills /\*/SKILL.md）—每个技能是一个文件夹，包含一个`SKILL.md`文件
—SKILL.md必须有`name`字段（小写带连字符，匹配文件夹名称，最多64个字符）
-SKILL.md必须有`description`字段（用单引号括起来，10-1024个字符）
—文件夹名称必须是小写字母，并用连字符分隔
-技能可以包括捆绑资产（脚本，模板，数据文件）
-捆绑资产应在SKILL.md说明中引用
-资产文件的大小应合理（每个文件不超过5MB）
-技能遵循[座席技能规范]（https://agentskills.io/specification）

#### Canvas Extensions （Extensions /\*）—每个扩展文件夹必须包含“`extension.mjs`”
—扩展元数据必须位于`.github/plugin/plugin.json`—扩展名`plugin.json`**必须遵循以下约定：
—必须为`name`、`description`、`version`-`logo`**必须**恰好是`"assets/preview.png"`（强制约定）
-`extensions`**必须**完全是`"."`（per [copilot-agent-runtime#9929](https://github.com/github/copilot-agent-runtime/pull/9929)）
—可选：`author`、`keywords`字段
**不能**包含`x-awesome-copilot`字段（只能使用基于约定的`assets/preview.png`）
-每个扩展必须有`assets/preview.png`作为主要的视觉资产
-不添加`canvas.json`；网站元数据来源于`.github/plugin/plugin.json`####钩子文件夹（hooks/\*/README.md）-每个钩子是一个文件夹，包含一个`README.md`文件与frontmatter
-README.md必须有`name`字段（人类可读的名称）
-README.md必须有`description`字段（用单引号括起来，不能为空）
必须包含一个带有钩子配置的`hooks.json`文件（从这个文件中提取钩子事件）
—文件夹名称必须是小写字母，并用连字符分隔
-可以包括捆绑资产（脚本，实用程序，配置文件）
—捆绑式脚本应在README.md和hooks.json中引用
-遵循[GitHub Copilot挂钩规格]（https://docs.github.com/en/copilot/how-tos/use-copilot-agents/coding-agent/use-hooks）
-可选包括`tags`字段进行分类

####工作流文件（workflows/\*.md）—每个工作流在`workflows/`目录下是一个独立的`.md`文件
-必须有`name`字段（人类可读的名称）
-必须有`description`字段（用单引号括起来，不能为空）
-包含代理工作流前端内容（`on`,`permissions`,`safe-outputs`）和自然语言指令
—文件名必须是小写字母，单词之间用连字符分隔
—只接受`.md`文件。—CI禁止接收`.yml`、`.yaml`和`.lock.yml`文件
-遵循[GitHub代理工作流规范]（https://github.github.com/gh-aw/reference/workflow-structure/）

####插件文件夹（plugins/\*）—每个插件是一个文件夹，包含一个包含元数据的`.github/plugin/plugin.json`文件
-plugin.json必须有`name`字段（与文件夹名称匹配）
-plugin.json必须有`description`字段（描述插件的用途）
-plugin.json必须有`version`字段（语义版本，例如“1.0.0”）
插件内容在plugin.json中使用Claude Code规范字段（`agents`,`commands`,`skills`）进行声明式定义。源文件位于顶级目录中，并由CI具体化到插件中。
—`marketplace.json`文件是在构建过程中自动从所有插件生成的
-插件可以通过GitHub CopilotCLI发现和安装

添加新资源

当添加新的代理、指令、技能、钩子、工作流或插件时：

**代理商及说明：**1. 用合适的标题创建文件
2. 将文件添加到适当的目录中
3. 运行命令`npm run build`更新README.md4. 验证资源出现在生成的README中

钩子的* *:* *

1. 在`hooks/`中创建一个具有描述性名称的新文件夹
2. 用合适的标题创建`README.md`（名称、描述、钩子、标签）
3. 根据GitHub Copilot钩子规范，使用钩子配置创建`hooks.json`4. 将任何绑定的脚本或资源添加到文件夹中
5. 使脚本可执行：`chmod +x script.sh`6. 通过运行：`npm run build`更新README.md7. 验证钩子出现在生成的README中

为工作流* *:* *1. 在`workflows/`中创建一个具有描述性名称的新`.md`文件（例如，`daily-issues-report.md`）
2. 包括`name`和`description`的前端内容，以及代理工作流字段（`on`,`permissions`,`safe-outputs`）
3. 用`gh aw compile --validate`编译以验证它是否有效
4. 运行命令`npm run build`更新README.md5. 验证工作流出现在生成的README中

为技能:* * * *

1. 运行`npm run skill:create`构建一个新的技能文件夹
2. 按照您的说明编辑生成的SKILL.md文件
3. 将任何绑定的资产（脚本、模板、数据）添加到技能文件夹中
4. 运行`npm run skill:validate`来验证技能结构
5. 运行命令`npm run build`更新README.md6. 验证生成的README中显示的技能

为插件:* * * *1. 运行`npm run plugin:create -- --name <plugin-name>`来构建一个新插件
2. 使用Claude Code规范字段在`plugin.json`中定义代理、命令和技能
3. 用元数据编辑生成的`plugin.json`4. 运行`npm run plugin:validate`来验证插件结构
5. 执行命令`npm run build`更新README.md和marketplace.json6. 验证插件是否出现在`.github/plugin/marketplace.json`中

**对于画布扩展：**

1.Create/update扩展在`extensions/<extension-id>/`与`extension.mjs`2. 添加`.github/plugin/plugin.json`元数据（必选：`name`，`description`,`version`,`logo: "assets/preview.png"`,`extensions: "."`；可选：`author`，`keywords`）
3. 确保`assets/preview.png`作为主要可视资产存在
4. 运行`npm run plugin:validate`验证插件和扩展元数据
5. 运行`npm run build`重新生成网站数据和市场输出

**外部插件：**1. 不打开一个直接的PR编辑`plugins/external.json`公开第三方插件提交
2. 公共外部插件提交使用记录在[CONTRIBUTING.md]中的外部插件问题工作流（CONTRIBUTING.md# add-external_plugins）
3. 在v1版本中，只有github托管的插件才接受公开提交，使用公共repo加上不可变的`ref`，`sha`，或两者兼而有之
4.`eng/external-plugin-validation.mjs`中的共享验证器是外部插件数据规则的标准真实性来源；重用它，而不是在脚本或工作流中重复检查
5. 提交问题通过`external-plugin`+`awaiting-review`，然后`ready-for-review`或`requires-submitter-fixes`基于自动化质量闸门
6. issue编辑后，issue作者或维护者可以评论`/rerun-intake`以重新运行自动输入和质量闸门，而无需打开新的提交issue
7. 维护人员可以用`/mark-ready-for-review [optional reason]`显式地覆盖质量门阻塞器，将问题移至`ready-for-review`8. 一旦问题在`ready-for-review`中，维护人员就会用`/approve`或`/reject <reason>`问题注释做出决定；已批准的问题被关闭，并用作六个月的重新审查锚点
9. 审批自动化根据`main`创建或更新PR，更新`plugins/external.json`，并重新生成市场输出
10. 每晚重新审查自动化发现至少六个月前关闭的`external-plugin`+`approved`问题，应用`re-review-due`，并为维护人员打开或更新跟踪问题
11. 维护人员使用`/re-review-keep`、`/re-review-needs-changes`或`/re-review-remove`完成对原始批准的提交问题的重新审查；keep重置问题`closed_at`， remove打开针对`main`的PR测试说明```bash
# Run all validation checks
npm run plugin:validate
npm run skill:validate

# Build and verify README generation
npm run build

# Fix line endings (required before committing)
bash eng/fix-line-endings.sh
```
之前:

-确保所有降价前事项格式正确
—验证文件名是否遵循小写带连字符的约定
—执行`npm run build`命令更新README
- **总是运行`bash eng/fix-line-endings.sh`**来规格化行尾（CRLF→LF）
-检查你的新资源是否正确地出现在README中

代码风格指南

Markdown文件

-使用合适的标题和必要的字段
-保持描述的简洁和信息丰富
-将描述字段值用单引号括起来
—使用小写的文件名和连字符作为分隔符

###JavaScript/Node.js脚本

—位于`eng/`和`scripts/`目录下
-遵循Node.jsES模块约定（`.mjs`扩展）
—使用清晰、描述性的函数和变量名

拉取请求指南

创建拉取请求时：

**重要：**所有的pull请求应该针对**`main`**分支，而不是`staged`。1. **README更新**：当您运行`npm run build`时，新文件应该自动添加到README中
2. **前内容验证**：确保所有标记文件都具有所需的前内容字段
3. **文件命名**：验证所有新文件遵循小写带连字符的命名约定
4. **构建检查**：在提交之前运行`npm run build`来验证README的生成
5. **行结束符**:**始终运行`bash eng/fix-line-endings.sh`**将行结束符归一化为LF （unix风格）
6. **描述**：提供agent/instruction功能的清晰描述
7. **测试**：如果添加插件，运行`npm run plugin:validate`以确保有效性

预提交检查表

在提交PR之前，请确保您具备：-[]执行命令`npm install`（或`npm ci`）安装依赖项
-[]执行命令`npm run build`，生成更新后的README.md-[]执行`bash eng/fix-line-endings.sh`命令，将行尾归一化
-[]验证所有新文件有正确的正面内容
-[]测试你的贡献与GitHub Copilot工作
-[]检查文件名是否遵循命名约定

代码审查检查表

对于指令文件（\*.instructions.md）：

-[]有降价前的问题
-[]非空的`description`字段用单引号括起来
-[]有`applyTo`字段和文件模式
-[]文件名为小写带连字符的文件

对于代理文件（\*.agent.md）：-[]有降价前的问题
-[]非空的`description`字段用单引号括起来
[]有`name`字段与人类可读的名称（例如，“Address Comments”而不是“Address - Comments”）
-[]文件名为小写带连字符的文件
-[]包含`model`字段（强烈推荐）
-[]考虑使用`tools`字段

对于技能（skills/\*/）：

—[]文件夹中包含一个名为SKILL.md的文件
- []SKILL.md有降价的正面问题
—[]有`name`字段匹配文件夹名称（小写带连字符，最多64个字符）
-[]有非空的`description`字段用单引号包装（10-1024个字符）
—[]文件夹名称为连字符的小写字母
-[]任何绑定的资产在SKILL.md中被引用
-[]捆绑资产每个文件不超过5MB

对于钩子文件夹（hooks/\*/）：-[]文件夹包含一个带有markdown front matter的README.md文件
-[]有`name`字段与人类可读的名称
-[]非空的`description`字段用单引号括起来
[]`hooks.json`文件中有有效的钩子配置（钩子事件从这个文件中提取）
—[]文件夹名称为连字符的小写字母
-[]任何绑定的脚本都是可执行的，并在README.md中引用
-遵循[GitHub Copilot挂钩规格]（https://docs.github.com/en/copilot/how-tos/use-copilot-agents/coding-agent/use-hooks）
-[]可选包含`tags`array字段用于分类

对于工作流文件（workflows/\*.md）：-[]文件有降价前事项
-[]有`name`字段和人类可读的名称
-[]非空的`description`字段用单引号括起来
-[]文件名为小写带连字符的文件
- [] frontmatter中包含`on`和`permissions`—[]使用最小权限和安全输出
—[]不包含`.yml`、`.yaml`和`.lock.yml`文件
-[]遵循[GitHub代理工作流规范]（https://github.github.com/gh-aw/reference/workflow-structure/）

插件（plugins/\*/）：—[]目录中包含一个`.github/plugin/plugin.json`文件
—[]目录中包含一个`README.md`文件
—[]`plugin.json`有匹配目录名的`name`字段（小写带连字符）
—[]`plugin.json`有非空字段`description`[]`plugin.json`有`version`字段（语义版本，例如“1.0.0”）
—[]目录名称为小写字母加连字符
—[]如果存在`keywords`，它是一个由小写连字符组成的数组
—[]当存在“`agents`”、“`commands`”或“`skills`”数组时，每个表项都是有效的相对路径
-[]插件不会引用不存在的文件
-[]执行`npm run build`命令，验证marketplace.json是否更新正确

# #贡献

这是一个社区驱动的项目。欢迎投稿！请参阅:

—[CONTRIBUTING.md]（CONTRIBUTING.md）为贡献指南
-社区标准[CODE_OF_CONDUCT.md]（CODE_OF_CONDUCT.md）
—安全策略为[SECURITY.md]（SECURITY.md）

MCP服务器该存储库包括一个MCP（模型上下文协议）服务器，用于直接从该存储库搜索和安装资源。运行服务器需要Docker。

# #许可证

MIT许可证-详见[许可证]（License）