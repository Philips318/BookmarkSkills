---
description: 'Ensure documentation and examples use only generic, cliche placeholder data — never real or sensitive data sourced from local scripts, configuration, task files, or prompt context.'
applyTo: '**/*.{md,js,mjs,cjs,ts,tsx,jsx,py,json}'
---
#在文档中使用陈词滥调数据

在更新或编写工具文档时，**永远不会包含提示符、本地配置、脚本、任务文件或任何其他特定于实现的源中提供的真实数据**。文档必须只使用通用的、公认的、不能暴露敏感信息的占位符数据。

##为什么这很重要

工具的源代码和本地配置通常包含真实的名称、真实的电子邮件地址、真实的组织详细信息和真实的域名。这些值对于工具的功能是必需的，但是它们在面向公众的文档中没有位置。将真实数据泄露到文档中可能会暴露：

—内部业务名称和联系方式
—邮件地址和域名
-客户或客户标识符
—帐户名称和凭据
-揭示私有操作的特定于组织的术语

核心规则> **如果数据来自一个提示，一个本地文件，一个脚本，一个配置，或一个任务-它不会进入文档
>
>文档示例仅使用众所周知的、虚构的或明显占位符数据。

什么算真实数据

任何源自：

- **本地配置文件**（如`config.json`、`.env`、account模块）
- **脚本和任务文件**（例如，批处理脚本，shell脚本，任务运行器）
-提示上下文（例如，用户在要求代理构建或更新工具时提供的数据）
- **映射或过滤文件**（例如，JSON映射，数据提取规则）
- ** git忽略的文件**（例如，包含环境特定值的版本控制排除的文件）

经批准的文档占位符数据

在所有文档和示例中使用这些通用的、陈词滥调的替代品：

|类别|批准占位符示例|| --- | --- |
| **人们** |简·多伊，约翰·史密斯，爱丽丝，鲍勃|
| **电子邮件地址** |`jane.doe@example.com`，`admin@example.org`|
| **组织** | Acme Corp， Contoso, Northwind Traders |
| **域** |`example.com`，`example.org`,`example.net`|
| **地址** | 123 Main Street， Suite 100, Springfield |
| **电话号码** |`(555) 123-4567`|
| **帐户/用户名** |`demo-user`，`test-account`|
| **文件路径** |`accounts/acme.mjs`，`config/reports.json`|
| **项目名称** |我的项目，示例应用程序，演示工具|

##将占位符与上下文匹配

占位符只有在它在周围的上下文中似乎合理时才正确。违反操作系统约定、工具规范或所描述的工作流的通用名称与实际值一样具有误导性。选择适合平台、工具和价值所扮演角色的替代品。

选择与平台匹配的路径

|操作系统/环境|使用|避免使用|| --- | --- | --- |
| Windows，每用户数据|`C:\Users\<user>\AppData\Local\AcmeApp\`|`/home/user/...`，`~/.config/...`|
| Windows，每台机器共享数据|`C:\ProgramData\AcmeApp\`|`C:\Users\<user>\...`|
| Windows，临时|`%TEMP%\acme\`或`C:\Users\<user>\AppData\Local\Temp\acme\`|`/tmp/acme/`|
| POSIX，每用户数据|`~/.config/acme/`，`~/.local/share/acme/`|`C:\Users\<user>\...`|
| POSIX，临时|`/tmp/acme/`|`%TEMP%\acme\`|
|跨平台示例|都显示，或者使用`<config-dir>/acme/`|静默选择一个|

如果周围的文本或代码是特定于操作系统的（`.bat`文件、运行在Windows上的`.jsx`、`bash`片段），则路径占位符必须与该操作系统匹配。当文档与平台无关时，要么显示这两种形式，要么使用一个清晰抽象的标记（`<install-dir>`,`<config-dir>`）。

将Scope匹配到工作流

占位符必须位于对它所代表的数据类型有意义的位置：

|数据角色|合理的占位符位置|| --- | --- |
|用户配置文件文件夹（`C:\Users\<user>\AppData\Local\<App>\logs\`,`~/.local/state/<app>/`） | . |用户日志和运行时输出
|用户配置|用户配置文件夹（`%APPDATA%\<App>\`,`~/.config/<app>/`） |
|全机共享状态|`C:\ProgramData\<App>\`，`/var/lib/<app>/`|
|存储库-相对路径（`./build/`,`./tmp/`） |
|项目输出文件夹（`./dist/`,`./out/`） |

编写调试日志的用户驱动脚本不应该将该日志放在`C:\ProgramData\…`（机器共享）中；维护共享状态的服务不应该将其放在`~/.config/…`（每用户）中。选择该角色的实际实现将选择的位置。

###匹配标识符到域

当示例使用标识符（帐户名称、项目名称、数据集键）时，请选择与周围域词汇表一致的占位符。—CRM举例：`acme-corp`、`northwind-traders`。
—地理数据集示例：`springfield`，`region-west`。
一个开发者工具示例：`demo-app`，`sample-project`。

不要混合域（在地理数据示例中，`acme-corp`读起来是错误的，即使这两个名称都是通用的）。

# # #自检

在提交占位符之前，请询问：

—路径语法是否与同一代码块中显示的操作系统匹配？
位置是否匹配数据的角色（用户vs.机器，运行时vs.配置，本地vs.共享）？
—标识符是否与周围示例的**域**匹配？

如果有任何答案是否定的，将占位符替换为合适的占位符。

如何应用此规则

###添加功能时

如果您使用真实帐户数据添加功能（例如，以真实客户端命名的脚本），请使用虚构的帐户名称来记录该功能。**真实实现文件：**一个针对特定业务配置的账户模块

* *文档的例子:* *```javascript
// accounts/acme.mjs — Example account configuration
export default {
  name: 'Acme Corp',
  email: 'reports@example.com',
  folder: 'INBOX',
};
```
更新配置文档时

如果配置文件引用了真实的域、真实的路径或真实的凭据，那么在将其包含在文档中之前，请将每个真实值替换为占位符。

* *文档的例子:* *```json
{
  "host": "imap.example.com",
  "user": "admin@example.com",
  "folder": "INBOX/Reports",
  "outputDir": "./downloads"
}
```
当编写脚本示例时

如果脚本自动执行特定组织的任务，则文档示例必须使用通用组织名称和通用参数。

* *文档的例子:* *```batch
@echo off
REM Example: Run the extraction task for Acme Corp
node extractEmail.mjs --account acme --task download
```
代码和文档之间的界限

|允许使用真实数据？|| --- | --- |
|运行时使用的本地脚本和配置文件|是|
|带有环境特定值的git忽略文件|是|
|为构建或配置工具提供的提示数据|是（仅在代码中）|
|README.md， docs/文件夹和示例模板| **禁止使用占位符** |
|CHANGELOG.md条目| **无-描述一般变化** |
|提交源文件中的代码注释| **不保留泛型** |

##一个例外

来自真实数据的单词可能出现在文档**中，如果它是在其普通意义上使用的普通英语单词，并且在示例上下文中**而不是**。例如，“开发”这个词在“这个工具正在积极开发中”这样的句子中是可以接受的，即使它也出现在一个真实的组织名称中。

# #总结文档是公开的。实现数据是私有的。把它们分开。每个文档文件中的每个示例都应该通过一个简单的测试：*一个陌生人读了这篇文章，对这个工具背后的真实用户、客户或组织一无所知吗？*如果答案是否定的，那就用老套的占位符替换数据。