# GitHub API怪癖（已验证）

对Copilot评审循环很重要的API行为。所有验证
与当前API界面的对比——在找到
替代API或修改绑定的脚本。

## GraphQL触发器-`requestReviewsByLogin`是支持的路径```graphql
mutation($p: ID!) {
  requestReviewsByLogin(input: {
    pullRequestId: $p,
    botLogins: ["copilot-pull-request-reviewer"]
  }) {
    pullRequest { number }
  }
}
```
经验验证对个人回购没有副驾驶专业和
org repos与Copilot企业。适用于初始添加和
重新请求（没有特殊的重新请求突变）。

三个GraphQL陷阱：

1. 突变是**`requestReviewsByLogin`**，而不是`requestReviews`。`RequestReviewsInput`（由`requestReviews`使用）不公开`botLogins`字段，因此它根本不能请求一个bot审阅者`botLogins`是`requestReviewsByLogin`的中心场。
2. 字段是**`botLogins`**，而不是`userLogins`。后者返回`Could not resolve user with login 'Copilot'`。
3. 弹头是**`copilot-pull-request-reviewer`** （App弹头）。这个
显示登录`Copilot`返回“无法解析带有slug的bot”
“副驾驶”。通过问题上的新`copilot_work_started`事件验证是否成功
事件提要-`GET /repos/{o}/{r}/issues/{n}/events`（参见SKILL.md）
“HTTP 200 / exit 0是NOT proof”)。从经验上看，这个事件
类型在`/events`端点上公开（通过20多个验证）
触发轮（PR 236）；它不只是时间轴。`01-request-review.ps1`通过比较事件`id`来加强这一点
（单调）触发前后。

其他触发路径-请勿使用

**`requestReviews`与`botLogins`**→输入类型拒绝
字段。不要尝试变体。
- **REST`POST /pulls/<n>/requested_reviewers`with`reviewers[]=Copilot`**→可以静默返回HTTP 201
放下机器人。没有被脚本使用。
- **`gh pr edit --add-reviewer Copilot`**→返回“副驾驶”不是
found` on current `gh”。没有被脚本使用。

## GraphQL`latestReviews`-过时的缓存，不使用```graphql
# DO NOT — stale projection:
pullRequest(number:$pr){ latestReviews(first:50){ nodes{...} } }

# USE INSTEAD — always current:
pullRequest(number:$pr){ reviews(last:100){ nodes{...} } }
```
`latestReviews`是带有过期缓存的“每用户最新”投影
行为：一次新的副驾驶检查可能会缺席几分钟
提交后，而`reviews(last:100)`立即反映它。
使用`latestReviews`进行飞行或收敛检查会导致
脚本操作一个过时的提交OID -错误
宣布收敛或暂停审查，已经
的存在。`02-check-review-status.ps1`使用`reviews(last:100)`过滤
客户端到副驾驶审稿人登录。它还会发出一个错误
当结果恰好是100个评论时发出警告，以便调用者知道
边界被击中，最新的副驾驶审查可能会更老
比窗口-实际上只可能在100+非副驾驶
评论在副驾驶最后一次评论之后发布，这是不可能的
在正常使用中。如果你看到警告并且循环行为不正常，
手动获取完整的评审列表：```bash
gh pr view <n> --json reviews --jq '.reviews[] | select(.author.login | test("copilot-pull-request-reviewer"))'
```
多个副驾驶评审的平局

当多个副驾驶审查共享相同的`submittedAt`（突发重新触发下罕见的服务器端时钟冲突）
脚本首先倾向于审查其`commit.oid == HEAD`，然后
回落到一个稳定的类型。目的是“审查……
匹配当前代码的是代理应该回复的代码“-”
防止老旧审核胜出，虚报
将`ReviewAtHead`翻转为假。

##回复+解决突变-两者都有效```graphql
mutation($tid: ID!, $body: String!) {
  addPullRequestReviewThreadReply(input: {
    pullRequestReviewThreadId: $tid,
    body: $body
  }) { comment { id } }
}

mutation($tid: ID!) {
  resolveReviewThread(input: { threadId: $tid }) {
    thread { isResolved }
  }
}
```
##`isOutdated`≠`isResolved`-当前未解析状态为真

一个线程可以是`isOutdated: true`(副驾驶的注释点在行
这已经改变了)，而仍然是`isResolved: false`。这些
线程:

-仍然需要回复+解析在每轮循环。一根线可以
当你自己的修正改变了被引用的内容时，你就变得过时了
行。对`!isOutdated`进行过滤将静默地删除这些内容
线程，甚至让PR的开放对话列表非空
在底层代码固定之后。
-`03-list-open-threads.ps1`因此列出所有未解决的
没有`isOutdated`过滤器的线程。
-`10-cleanup-outdated.ps1`只是一个安全网-为罕见的
当线程在你的最后一轮之后过时时
取回。

审查延迟-轮询不要超过3分钟副驾驶的评论通常在请求后3-6分钟发布，
偶尔长达10分钟。没有进度信号；
轮询次数超过每3分钟一次会浪费API预算
使审查更快到达。

##`gh api graphql -F`强制字符串-`String!`使用`-f``gh`命令行区分了它的两种标志形式：

-`-F key=value`-型推理。值解析为int、bool或
null作为JSON文本发送。
-`-f key=value`-总是作为原始字符串发送。

对于任何声明为`String!`的GraphQL变量(例如`owner`，`repo`，`body`,`tid`,`after`)，在呼叫站点使用**`-f`**。一个回复体
碰巧是`"true"`，`"null"`，或者所有数字都是
强制调用，调用失败并出现类型错误。只保留`-F`真正的数字或布尔变量（例如`pr: Int!`）。注意：共享的`Invoke-Gh`包装器可能会在内部重写
>`-f field=<body>`入`-F field=@<tempfile>`时体含`"`(Windows PowerShell 5.1原生参数引用错误-参见
>)。即使通过`@file`，`-F`仍然对
>文件内容（gh记录的行为）——重写是安全的
>只是因为重写触发器（“主体包含`"`”）保证
>的内容是一个字符串，没有JSON字面量(`123`,`true`，
>`null`等)将匹配。将这种`-F ...=@file`用法视为
>包装器的内部传输详细信息，而不是权限
>使用`-F ...=@file`在调用站点的任意字符串。```powershell
# Wrong — body could be coerced AND, under Windows PowerShell 5.1,
# any embedded `"` in $Body will be mis-split by the native-arg
# passer (gh sees a truncated body or a "received N args" error).
gh api graphql -f query=$q -F body=$Body

# Right — go through Invoke-Gh / Invoke-GhGraphQL. The shared helper
# auto-rewrites `-f field=<body>` and `-F field=<body>` pairs whose
# body contains `"` to `-F field=@<tempfile>` so the value is read
# from disk and never appears on the command line. This works
# identically on Windows PowerShell 5.1 and PowerShell 7+.
Invoke-GhGraphQL -GhArgs @('-f',"query=$q",'-f',"body=$Body") -Context 'reply body'
```
直接调用`gh`（例如通过`& gh ...`或raw`gh api graphql`）
绕过跨版本的tempfile重写-如果您的值包含`"`您将重新引入仅限powershell 5.1的分割错误。总是
漏斗`gh`呼叫通过`Invoke-Gh`/`Invoke-GhGraphQL`。

原生`gh`退出码绕过`$ErrorActionPreference``gh`是本地可执行文件，而不是PowerShell的cmd命令，所以是非零的
Exit不抛出，即使`$ErrorActionPreference = 'Stop'`。
如果没有明确的检查，脚本将打印误导性的成功
消息，循环将错误地声明
对真实问题、速率限制或瞬态5xx的收敛。

附加陷阱：`gh api graphql`可以为HTTP 200退出0
JSON主体携带一个顶级的`errors`数组。把它当作失败
调用。[scripts/_lib.ps1]中的共享帮助程序（../scripts/_lib.ps1）
（`Invoke-Gh`和`Invoke-GhGraphQL`）通过`& gh @args`运行`gh`将stderr重定向到一个临时文件（`2>$errFile`），然后读取`$LASTEXITCODE`，返回`{ExitCode, Stdout, Stderr}`。`Invoke-GhGraphQL`还解析GraphQL`errors`数组
并在任一失效模式下抛出。所有
捆绑脚本。source`_lib.ps1`并使用这些包装器- do
在任何新脚本中都是一样的。

参数顺序```bash
git stash push -m "local-build" -- src/path/a src/path/b   # correct
git stash push -- src/path/a src/path/b -m "local-build"   # SILENTLY drops -m
```
`-m`必须出现在`--`路径分隔符之前。