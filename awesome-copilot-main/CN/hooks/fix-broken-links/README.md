---
name: 'Fix Broken Links'
description: 'Checks changed web files for broken hyperlinks and SEO anchor issues after each Copilot tool use.'
tags: ['links', 'seo', 'html', 'markdown', 'post-tool-use']
---
修复坏链接钩子

扫描最近更改的web文件，在每个GitHub Copilot之后查找断开的超链接
工具的使用。对于每个损坏的URL，钩子会尝试常见的拼写变体，然后是hands
链接到Copilot CLI代理以获取建议的替换，并提供一个
交互式修复菜单。通用锚文本（`click here`、`read more`等）为
标记为搜索引擎优化问题。

# #概述

断开的链接在web项目中无声地积累。运行在`postToolUse`上
事件时，此钩子检查代理刚刚编辑的web文件-并且仅检查那些-
在每次更改之后，您可以修复、替换或删除每个断开的链接
相同的终端会话。

钩子有两种模式：- **带有文件路径**(从钩子负载注入的编辑文件，或路径
通过命令行传递)：它检查每个链接，查找替换
选项，并显示交互式修复菜单。
- **没有文件参数**：它只是列出它找到的坏链接-没有
替换查找，没有提示。

# #特性- **自包含的核心**:bash和PowerShell端口-没有运行时安装（可选代理）
切换重用你已经拥有的Copilot CLI)
- **编辑文件范围**：作为`postToolUse`钩子，它只检查代理刚刚更改的文件
从来没有一个完整的回购扫描
- **格式无关链接扫描**：提取每个`http(s)`URL与`grep`，包括HTML， Markdown，JS/TS， JSON， CSS， SQL和模板
- **自动修复URL **：尝试www， https和尾斜杠的变化
- **代理辅助建议**：将断开的链接交给Copilot CLI代理(一个轻量级，
低标记的`gpt-5-mini`提示（没有工具）用于替换候选；命令行缺失或
错误，它根本不提供
- **SEO审计**：标志锚文本太通用，有利于搜索排名
—**大文件保护**：链接超过50个的文件检查前提示
- **交互式修复菜单**：替换建议，输入自定义URL，剥离标签保留文本，或
跳过
**仅限标准工具**:`curl`，`grep`，`sed`-适用于任何POSIX系统# #安装

1. 将钩子文件夹复制到你的存储库：   ```bash
   cp -r hooks/fix-broken-links .github/hooks/
   ```
2. 使脚本可执行：   ```bash
   chmod +x .github/hooks/fix-broken-links/link-fix.sh
   ```
3. 将钩子配置提交到存储库的默认分支。

# #配置

钩子在`hooks.json`中被配置为在`postToolUse`事件上运行：```json
{
  "version": 1,
  "hooks": {
    "postToolUse": [
      {
        "type": "command",
        "bash": ".github/hooks/fix-broken-links/link-fix.sh",
        "powershell": ".github/hooks/fix-broken-links/link-fix.ps1",
        "cwd": ".",
        "timeoutSec": 120
      }
    ]
  }
}
```
支持的源类型

通过扫描每个文件查找`http(s)://`url来找到链接，因此逻辑相同
涵盖了嵌入绝对url的所有格式：

|源|匹配|的示例| --- | --- |
|`<a href>`,`<img src>`,`<script src>`,`<link href>`,`<iframe src>`|
|降价|`[text](url)`，`[text][ref]`，裸`<url>`|
| JS / TS / Vue / Svelte |`fetch()`,`XMLHttpRequest.open()`, jQuery, axios,`href:`/`url:`props |
| JSON / JSONL |任何URL的绝对字符串值|
| CSS |`url(...)`|
| SQL |查询字符串中的URL字面值|
|模板| Jinja2， ERB， EJS，车把，Pug |`d`（删除）操作理解HTML`<a>`包装器和Markdown`[text](url)`特别是链接，保持可见的文本。其他源代码类型支持`r`（替换）和`c`（自定义）通过文本URL替换。

##修复选项

对于每个断开的链接：

|键|动作|| --- | --- |
|`r`|替换为建议的URL（一个有效的变体，或代理建议的替代方案）|
|`d`|剥离链接包装器，将可见文本保留为纯文本|
|`c`|输入自定义的替换URL |
|`s`|跳过|

##输出示例```text
  Checking 2 link(s) in docs/guide.md ...
    BROKEN (404) https://example.com/old-page

------------------------------------------------------------
  SEO anchor issues (consider descriptive link text)
    docs/guide.md: <a href="https://example.com/old-page">click here</a>

============================================================
  fix-broken-links report
============================================================

  [1] docs/guide.md
    URL : https://example.com/old-page
    HTTP: 404

    r  Replace -> https://example.com/docs/install
    1  Replace -> https://example.com/docs/getting-started
    d  Remove link, keep text
    c  Custom replacement URL
    s  Skip
  > r
    replaced

  1 file(s) updated:
    docs/guide.md
```
如果没有文件参数（或者当编辑的文件没有可检查的链接时），则
钩子在断链列表之后停止——上面的菜单被跳过。

# #要求

-`curl`- HTTP状态检查（如果钩子不存在，则悄悄地退出）
-`grep`，`sed`-链接提取（任何POSIX系统的标准）
-`jq`- bash钩子解析postToolUse JSON有效负载和发现编辑的文件所需
- Bash 4+（用于`link-fix.sh`）；在Windows上使用Git Bash或WSL，或者运行PowerShell 7+端口 `link-fix.ps1`
—`copilot`（GitHub Copilot命令行）—可选；为代理人建议的替代品提供动力。没有它,
只提供经过验证的拼写变体
-`git`用于发现更改的文件；钩子在没有它的情况下退回到完整的回购扫描

##文件结构```
.github/hooks/fix-broken-links/
├── hooks.json      GitHub Copilot hook configuration
├── link-fix.sh     Bash hook implementation
├── link-fix.ps1    PowerShell 7+ port
└── README.md       This file
```
# #的局限性

-只检查绝对的`http://`和`https://`url；相对路径需要运行中的服务器
-运行时从数据库查询生成的动态链接无法仅从源检测到
-当启用`copilot`建议时，损坏的url被发送到Copilot服务作为提示输入
-代理商建议的替换是模型建议，没有经过现场验证；每一个确认之前
接受
-`d`（删除）行动的目标是HTML和Markdown链接语法；代码中的裸url最好处理`r`或`c`