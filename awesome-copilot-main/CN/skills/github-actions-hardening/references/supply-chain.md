#供应链

工作流每次执行一个操作时都会运行其他人的代码。这些操作使用
您的令牌和（在特权触发器上）您的秘密，因此它们的完整性就是您的完整性。

将第三方操作固定到提交SHA

标签（`@v4`）和分支（`@main`）是可变的——上游所有者（或任何妥协者）
它们)可以将它们重新指向新的代码，而无需更改一行。一个完整的40个字符提交SHA是
不可变的。```yaml
# Mutable — the tag can be moved to malicious code
- uses: some-org/some-action@v3

# Pinned — this exact tree, forever
- uses: some-org/some-action@3f1e0a9c8b7d6e5f4a3b2c1d0e9f8a7b6c5d4e3f # v3.2.1
```
规则:

*第三方操作（任何不是`actions/*`或`github/*`）→**必须**被sha -pin。国旗标签
枝叶高大。
*`@main`/`@master`→高，无论出版商；这是未版本的“最新”。
*第一方`actions/*`→sha -pin是强化的建议（低，如果只有标签-pin）。
*保留`# vX.Y.Z`注释，以便人类和Dependabot可以阅读预期的版本。

这不是理论上的：在实际事件中，流行行为的标签被重新指向代码
从引用可变标记的每个工作流中泄露的秘密。

##让Dependabot更新引脚

SHA引脚失效。为`github-actions`生态系统启用Dependabot，以便更新以
可审阅的pr（它理解`# vX.Y.Z`注释并碰撞SHA）：```yaml
# .github/dependabot.yml
version: 2
updates:
  - package-ecosystem: github-actions
    directory: /
    schedule:
      interval: weekly
```
工件和缓存中毒

*不受信任的`pull_request`版本上传的工件是**不受信任的数据**。一个特权`workflow_run`可以下载它，但必须将其仅视为数据—永远不要执行它，并进行验证
提取时的路径（精心制作的工件可以包含`../`路径遍历条目）。
*缓存是键控的，可以由较少特权的运行填充；不相信缓存的构建输出
在特权环境中不被篡改。

##在公共Repos上自托管运行程序

默认的（github托管的）运行程序是短暂的-每个作业一个新的VM，之后销毁。* *自托管
运行程序坚持**，所以不受信任的分支PR代码运行在一个可以：

*把tools/backdoors留给下一份工作，
*读取同一机器上其他存储库的签出或凭据；
*以你的网络为中心。永远不要对公共分支可能触发的工作流使用自托管运行程序。如有必要，请使用
短暂的、孤立的、一次性的运行程序，并且永远不会将秘密暴露给分叉触发的作业。

凭证持久化

默认情况下，`actions/checkout`将令牌写入`.git/config`，以便以后的`git`步骤可以推送。
如果作业随后运行不受信任的代码，则该代码可以读取令牌。集`persist-credentials: false`时不需要推，尤其是在运行build/test之前
不受信任的代码。