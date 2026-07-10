#触发器和特权

工作流安全性最重要的一个问题是：外部贡献者是否可以触发
这个工作流，如果是，它得到什么令牌和秘密？** GitHub回答这个不同
触发器。

##信任矩阵

|触发|谁可以触发|`GITHUB_TOKEN`|可用秘密|风险|| --- | --- | --- | --- | --- |
|`push`|回购合作者|read/write| yes |低信任作者|
|`pull_request`（相同的回购分支）|合作者|read/write|是|低|
| **任何人** | **只读** | **没有** |低设计-即使是恶意代码也无法窃取任何东西|
|`pull_request_target`| **任何有分叉的人** | **read/write** | **yes** | **High** -运行在base-repo上下文中|
|`workflow_run`|触发另一个工作流| **read/write** | **yes** | **High** |
|`issue_comment`,`issues`| * *人* * | * *read/write* * | * *对* * | * *高* * |

陷阱：来自分叉的`pull_request`是“安全的”，因为GitHub故意剥离令牌
并保守秘密。那些发现“秘密在分支pr上不起作用”的维护者经常会切换
到`pull_request_target`，将它们取回——这样做时，将写令牌和每个秘密交给
任意的贡献者。

为什么`pull_request_target`是危险的`pull_request_target`检出**基**库的工作流定义（因此fork不能检出）
更改运行的内容)，但它以完全特权运行。危险之处在于当工作流出现时
显式地检查**分叉的**代码并执行它：```yaml
# DANGEROUS — RCE with a write token + secrets
on: pull_request_target
jobs:
  build:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@<sha>
        with:
          ref: ${{ github.event.pull_request.head.sha }}   # fork's code
      - run: npm install && npm test                        # runs the fork's code + scripts
```
`npm install`单独运行PR中的任意生命周期脚本
脚本可以读取`secrets.*`，并使用写令牌推送提交。

安全的双工作流模式

分裂的责任。**无特权**工作流运行不受信任的代码；* * * *的特权
工作流只消耗可信的*输出*。```yaml
# 1) Unprivileged: runs untrusted code, no secrets, read-only token
name: PR Build
on: pull_request
permissions:
  contents: read
jobs:
  build:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@<sha>
      - run: npm ci && npm run build
      - uses: actions/upload-artifact@<sha>
        with: { name: pr, path: dist/ }
```

```yaml
# 2) Privileged: triggered by the first, never runs fork code
name: PR Comment
on:
  workflow_run:
    workflows: ["PR Build"]
    types: [completed]
permissions:
  pull-requests: write
jobs:
  comment:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/download-artifact@<sha>   # data only, not executed
      # post results, using the trusted token — but never execute the artifact
```
# #规则

*将`pull_request_target`、`workflow_run`、`issue_comment`和`issues`视为特权。
在特权工作流程中，**永远**不检查并执行PR/fork代码。
*如果您只需要标记、评论或基于元数据的分类，那很好-只是不要运行
贡献者的代码。
*尽可能选择`pull_request`（其安全的read-only/no-secrets默认值）。