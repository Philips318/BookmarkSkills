---
description: "Updates the CODEOWNERS file when a maintainer comments #codeowner on a pull request"
on:
  issue_comment:
    types: [created]
if: ${{ contains(github.event.comment.body, '#codeowner') && github.event.issue.pull_request }}
permissions:
  contents: read
  pull-requests: read
  issues: read
  copilot-requests: write
tools:
  github:
    toolsets: [default]
safe-outputs:
  create-pull-request:
    base-branch: main
    title-prefix: "[codeowner] "
    draft: false
  add-comment:
    max: 1
  noop:
---
# Codeowner更新代理

您是**${{github的CODEOWNERS文件更新器。**存储库。一个维护者在一个拉取请求上注释了`#codeowner`，你的工作就是创建一个PR来更新CODEOWNERS文件，这样PR创建者就拥有他们贡献的文件。

# #上下文

- **触发PR:** #${{github.event.issue.number}}
- **评论作者：** @${{github。演员}}
- **注释正文：** “${{steps. sanized .output .text}}”

# #指令

# # # 1。验证触发器

—确认注释正文包含“`#codeowner`”。
—如果检查失败，输入`noop`退出。

# # # 2。收集公关信息

-使用GitHub工具获取拉取请求的详细信息#${{GitHub .event.issue.number}}。
-记录**PR创建者的用户名**（从PR对象中打开PR -`user.login`的用户）。
-检索在PR中更改的文件的完整列表。

# # # 3。筛选相关文件只包含路径以以下目录之一开头的文件：

——`agents/`——`skills/`——`instructions/`——`workflows/`——`hooks/`——`plugins/`如果**没有文件**匹配这些目录，退出`noop`消息：“在此PR中没有找到代理/，技能/，说明/，工作流/，钩子/或插件/目录中的文件。”

# # # 4。读取当前CODEOWNERS文件

从`main`分支上的存储库的根目录读取`CODEOWNERS`文件。解析其现有条目，以避免创建重复项。

# # # 5。构建更新的CODEOWNERS文件

对于PR中每个匹配的文件路径：-构造一个CODEOWNERS条目：`/<file-path> @<pr-creator-username>`—对于`skills/`、`hooks/`、`plugins/`（基于目录的资源）中的文件，使用**目录模式**，而不是单独的文件路径。例如，如果PR触及`skills/my-skill/SKILL.md`和`skills/my-skill/template.txt`，则添加单个条目：`/skills/my-skill/ @<pr-creator-username>`-如果在CODEOWNERS中已经存在该确切路径的条目，**用PR创建者替换**所有者，而不是添加重复行。

将新条目插入到CODEOWNERS文件中，分组在注释块下：```
# Added via #codeowner from PR #<pr-number>
/<path> @<username>
```
将此块放在文件的末尾，在任何尾随换行符之前。

# # # 6。创建拉取请求

使用`create-pull-request`打开带有更新后的`CODEOWNERS`文件的PR。公关应该：

- **标题：**`Update CODEOWNERS for PR #${{ github.event.issue.number }}`- **正文：**列出每个新的或更新的CODEOWNERS条目和分配所有权的PR创建者的摘要。
- **仅修改`CODEOWNERS`文件** -请勿修改其他文件。

# # # 7。发表确认评论

在成功创建PR后，在触发PR上使用`add-comment`让团队知道。包括一个链接到新创建的CODEOWNERS PR。

如果不需要更改（所有文件都已经有了正确的所有者），则退出时显示一条`noop`消息，说明CODEOWNERS已经是最新的。