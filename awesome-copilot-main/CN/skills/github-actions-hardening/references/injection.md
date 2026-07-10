#脚本注入

在shell运行**之前，`${{ <expr> }}`作为文本被替换到脚本**中。任何表达式
因此，解析外部贡献者控制的数据是命令注入接收器。

攻击者可控的上下文

这些可以由任何可以打开issue、PR或评论的人设置：

|背景信息|由|设置| --- | --- |
|`github.event.issue.title`/`.body`|问题作者|
|`github.event.pull_request.title`/`.body`| PR作者|
|`github.event.pull_request.head.ref`/`.head.label`| PR作者（分支名称）|
|`github.head_ref`| PR作者（分支名称）|
|`github.event.comment.body`|评论者|
|`github.event.review.body`/`.review_comment.body`|审稿人|
|`github.event.commits.*.message`/`head_commit.message`|提交作者|
|`github.event.commits.*.author.email`/`.name`|提交作者|
|`github.event.pages.*.page_name`|维基编辑器|

一个名为`$(<attacker-command>)`的分支或一个名为`"; <attacker-command> #`的问题成为shell
当插入到`run:`步长时。

##脆弱模式```yaml
# VULNERABLE
- run: |
    echo "Reviewing PR: ${{ github.event.pull_request.title }}"
    git checkout ${{ github.head_ref }}
```
安全模式-通过`env:`将不受信任的值绑定到环境变量，然后引用*shell*变量（引号）。
shell变量是data，永远不会被重新解析为工作流语法：```yaml
# SAFE
- env:
    PR_TITLE: ${{ github.event.pull_request.title }}
    HEAD_REF: ${{ github.head_ref }}
  run: |
    echo "Reviewing PR: $PR_TITLE"
    git checkout "$HEAD_REF"
```
`${{ }}`现在只出现在`env:`一侧，在那里它被赋值而不是拼接
变成命令。总是引用shell变量（`"$PR_TITLE"`）来防止分词和
匹配。## `actions/github-script`
同样的规则也适用。不要将`${{ }}`插入到`script:`体中-通过
环境和读取`process.env`：```yaml
# VULNERABLE
- uses: actions/github-script@<sha>
  with:
    script: console.log("${{ github.event.issue.title }}")

# SAFE
- uses: actions/github-script@<sha>
  env:
    TITLE: ${{ github.event.issue.title }}
  with:
    script: console.log(process.env.TITLE)
```
自定义动作输入

将不受信任的`${{ }}`传递到复合或JS操作的`with:`输入可能是安全的，也可能是不安全的
取决于动作本身是否将输入插入到shell中。当你有疑问的时候，跳过它`env:`并让操作读取环境，或者先读取sanitize/validate（例如分支名称）
应该匹配`^[A-Za-z0-9._/-]+$`)。

##快速审计检查表

1. 将每个`run:`和`script:`替换为`${{`。
2. 对于每个表达式，解析表达式指向什么。
3. 如果它可以由非合作者设置→通过`env:`用引号括起来的shell变量重写。
4.`github.actor`、`github.repository`、`github.sha`、`github.ref`（用于支路保护环境）
类似的服务器控制值不是攻击者设置的，而是深度防御的`env:`重写
花一分钱。