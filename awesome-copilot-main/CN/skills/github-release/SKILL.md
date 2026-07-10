---
name: github-release
description: >
  Guides IA through releasing a new version of a GitHub library end-to-end.
  Handles SemVer versioning and Keep a Changelog formatting automatically.
compatibility: "requires: gh CLI and git"
---
# GitHub释放技能

这项技能自动化了单个包GitHub存储库的完整发布工作流，
从分析到撰写变更日志和公关创作。它完全依赖于`gh`（GitHub CLI）和`git`不需要其他工具。

步骤1 - 4是**只读侦察**什么都不会写入到repo，直到
步骤5，确认版本号后。

何时使用此技能

当用户想要剪辑一个新版本，发布一个新版本，
修改一个版本，创建一个发布分支，生成一个变更日志，或者打开一个发布PR
在GitHub存储库上。即使用户说“让我们。
发布一个新版本”或“该发布了”。

---

# #先决条件

下面的例子包括Bash和PowerShell变体；Windows用户应该更喜欢
PowerShell模块。

启动前，请进行环境检查：```bash
gh auth status                        # must be authenticated
gh repo view --json nameWithOwner     # must be inside a GitHub repo
git status                            # working tree should be clean
```
如果任何检查失败，停止并告诉用户在继续之前要修复什么。

然后问用户一个问题：

b> *"哪个目录包含库的面向公众的源代码？
>（例如`src/`，`lib/`,`pkg/`） -用于将差异集中在消费者上
我真的看到了。按Enter键扫描整个仓库

将答案存储为`PUBLIC_PATH`。如果为空，则`PUBLIC_PATH`为`.`（repo root）。
排除所有差异中的这些路径：`tests/`，`test/`,`spec/`，`__tests__/`,`docs/`,`*.lock`,`*-lock.json`,`*.sum`，生成文件
（带有“不要编辑”头注释的文件），并构建工件。

---

9步发布工作流

按顺序完成每一步。向用户显示您将要运行的命令和
它的输出。只有在明确指出的情况下才暂停并请求确认。

---

###步骤1 -确保主机是最新的```bash
git checkout main
git pull origin main
```
先看`main`。版本分支在步骤5中创建，在版本之后
是证实。

---

###步骤2 -获取最新版本标签

为什么不是`gh release list`？** GitHub Releases是Git之上的一个可选层
>标签。许多repos标签发布与`git tag`没有创建一个GitHub发布，
因此，即使存在版本标记，`gh release list`也可以返回空。阅读标签
直接来自git的>是可靠的真相来源。```bash
# Fetch all tags from remote to ensure local view is current
git fetch --tags

# Find the latest version tag, sorted semantically
# --sort=-version:refname handles 1.10.0 > 1.9.0 correctly (unlike alphabetical)
PREV_TAG=$(git tag --sort=-version:refname | grep -E '^v?[0-9]+\.[0-9]+\.[0-9]+' | head -1)
echo "Latest tag: $PREV_TAG"
```

```PowerShell
# Fetch all tags from remote to ensure local view is current
git fetch --tags

# Find the latest version tag, sorted semantically
# --sort=-version:refname handles 1.10.0 > 1.9.0 correctly (unlike alphabetical)
$prevTag = git tag --sort='-version:refname' | `
  Select-String '^[vV]?\d+\.\d+\.\d+' | `
  Select-Object -First 1 -ExpandProperty Line

if ($prevTag) {
  $prevSha = git rev-list -n 1 $prevTag
} else {
  $prevSha = git rev-list --max-parents=0 HEAD
}

Write-Output "Latest tag: $prevTag"
```
然后验证标签存在于远程（而不仅仅是本地）：```bash
git ls-remote --tags origin | grep "refs/tags/$PREV_TAG$"
```
如果远程检查没有返回任何结果，则警告用户标记似乎是local-only
而且还没有被强制执行，他们可能想在继续之前强制执行。

-`PREV_TAG`是找到的标签名称（例如`v1.4.2`）。去掉任何前导`v`做算术时；在命名事物时保留它。
—如果**不存在任何标签**，将`PREV_TAG`视为`(none)`，将`PREV_SHA`设置为
第一次提交，默认新版本为`1.0.0`(跳过步骤4版本控制逻辑；
直接到步骤5)。
-如果标签没有指向一个真正的提交（孤儿标签），则返回到`git rev-list --max-parents=0 HEAD`并警告用户。```bash
PREV_SHA=$(git rev-list -n 1 "$PREV_TAG" 2>/dev/null || git rev-list --max-parents=0 HEAD)
```
---

###步骤3 -分析自上次发布以来发生了哪些变化

这一步使用两个互补的信号。的主要源代码
真理;提交消息提供了关于意图的支持上下文。

#### 3a -码差（主信号）```bash
# Focused diff on the public source path, excluding noise
git diff "$PREV_SHA"..HEAD -- "$PUBLIC_PATH" \
  ':(exclude)tests/' ':(exclude)test/' ':(exclude)spec/' \
  ':(exclude)__tests__/' ':(exclude)docs/' \
  ':(exclude)*.lock' ':(exclude)*-lock.json' ':(exclude)*.sum'
```

```PowerShell
# Focused diff on the public source path, excluding noise
git diff "$($prevSha)..HEAD" -- $publicPath `
  ':(exclude)tests/' ':(exclude)test/' ':(exclude)spec/' `
  ':(exclude)__tests__/' ':(exclude)docs/' `
  ':(exclude)*.lock' ':(exclude)*-lock.json' ':(exclude)*.sum'
```
读取完整的diff输出。对于每个更改的文件，确定：

1. **删除了符号** -函数，类，方法，常量，导出名称
以前存在过，现在消失了。？ 给少校强信号。
2. **修改了签名** -两个版本中都存在但不同的函数
参数、返回类型或抛出的错误。？ 给少校强信号。
3. **新的导出符号** -不存在的公共函数，类，常量
之前。？ MINOR信号。
4. **内部更改-不涉及任何公共接口的修改
（私有助手，未导出的函数，算法内部）。？ 补丁。
5. **Bug修复-对可证明错误的逻辑进行修正(例如off-by-one，
null检查，错误的条件)，而不改变公共API。？ 补丁。如果差异非常大（数千行），首先运行统计摘要
确定要完整读取哪些文件的优先级：```bash
git diff "$PREV_SHA"..HEAD --stat -- "$PUBLIC_PATH"
```
将详细阅读的重点放在更改最多的文件和文件名
建议他们定义公共接口(例如`index.*`，`api.*`,`exports.*`，`public.*`,`mod.*`,`__init__.*`)。

#### 3b -提交日志（次要信号）```bash
git log "$PREV_SHA"..HEAD --oneline --no-merges
```
用这个来：
-理解代码更改背后的意图，这些更改不是自解释的
单独的差异（例如，标记为这样的一行安全修复）。
-捕获可能在`PUBLIC_PATH`之外的路径中，但仍然是用户可见的更改
（例如，在`cmd/`目录中更改CLI标志）。
-填写更改日志条目的上下文，其中代码本身不能说明全部内容
的故事。

请参阅`references/commit-classification.md`，了解将消息模式映射为更改类型。

#### 3c -调和两个信号

什么时候信号一致？自信地使用这个分类。

什么时候信号冲突？**更喜欢代码diff**。例子:
-提交说`fix: typo`，但差异显示一个删除的公共方法？视其为少校。
-提交说`feat: new API`，但不同点只触及私人内部？视之为补丁。
-提交说`chore: refactor`，但不同点增加了新的导出符号？视其为未成年人。记录你注意到的任何冲突——在变更日志审查期间向用户标记它们
在步骤6中。

---

###步骤4 -确定下一个SemVer版本

将这些规则应用到步骤3中的分析中（完整规则在`references/semver-rules.md`中）：

|状态|碰撞||---|---|
|任何对公共API的破坏性更改（删除，签名更改，行为更改）| MAJOR |
|新导出的符号或功能，没有突破性的变化| MINOR |
Bug修复，性能改进，安全修复，文档，只有|补丁|

当一个版本包含mix时，优先级最高的将胜出。`MAJOR > MINOR > PATCH`。

计算`NEXT_VERSION`:
—将`PREV_TAG`拆分为`MAJOR.MINOR.PATCH`整数。
-应用适当的凹凸。
—格式为`vMAJOR.MINOR.PATCH`。

**向用户提供建议的版本，并简要说明引用的基本原理
具体的代码发现，而不仅仅是提交消息。例子:

> *“我建议v2.1.0。diff显示了两个新的导出函数(`NewClient`和
>`WithTimeout`)在`src/client.go`中，并且没有删除现有的公共符号或
>改变。提交消息作为功能添加证实了这一点

你可以这样问：“这个版本看起来对吗，还是你想调整一下？”*
等待确认后再继续。

---步骤5—创建发布分支

现在版本已经确认，从一开始就用正确的名称创建分支：```bash
git checkout -b release/vX.Y.Z
git push -u origin release/vX.Y.Z
```
---

###步骤6 -更新CHANGELOG.md读取现有的`CHANGELOG.md`（如果不存在则创建它）。遵循
[保持变更日志]（https://keepachangelog.com/en/1.1.0/）格式严格。

**在顶部插入**的结构（就在`# Changelog`标题下面）：```markdown
## [X.Y.Z] - YYYY-MM-DD

### Added
- ...

### Changed
- ...

### Deprecated
- ...

### Removed
- ...

### Fixed
- ...

### Security
- ...
```
规则:
-以`YYYY-MM-DD`格式使用今天的日期。
-省略没有条目的部分-不要留下空标题。
-从用户的角度用简单的英语写条目，主要是派生的
从代码diff显示的内容中，补充了提交消息上下文。
好：*“添加`WithTimeout`选项到HTTP客户端构造器。
错误：*“feat: add timeout cfg参数”
-将调查结果映射到各段：
-新的导出符号？添加
-破坏移除？删除
-对现有API的破坏性更改？已更改（将其标记为中断）
-Bug/logic修复，完美？固定
-安全修复？安全
-内部重构，文档，家务，测试？除非用户可见，否则省略
-如果提交消息显示了单独的代码差异无法传达的意图
（例如，伪装成一行更改的安全修复），包括该上下文
变更日志条目。
-同时更新文件底部的diff链接：  ```markdown
  [X.Y.Z]: https://github.com/OWNER/REPO/compare/vPREV...vNEXT
  ```
**在将建议的变更日志写入磁盘之前，向用户显示建议的变更日志
如果在步骤3c中发现任何信号冲突，请在这里标记它们，以便用户进行验证。
问：“这个变更日志看起来准确吗？”有要添加、删除或改写的条目吗？＂＊
合并反馈，然后写入磁盘。

---

###第7步-提交和推送```bash
git add CHANGELOG.md
git commit -m "chore: release vX.Y.Z"
git push origin release/vX.Y.Z
```
确认推送成功后再继续。

---

###第8步-打开拉请求

* * ? ?重要：**总是使用`--body-file`来传递PR正文文本，永远不要使用`--body`与内联文本。
像`\n`这样的内联转义序列不会被PowerShell解释为换行符，并且会出现
作为PR中的文本。使用文件可确保正确的标记格式。```bash
gh pr create \
  --base main \
  --head release/vX.Y.Z \
  --title "Release vX.Y.Z" \
  --body "$(cat <<'EOF'
## Release vX.Y.Z

This PR prepares the **vX.Y.Z** release.

### What's included
<!-- paste the changelog section here -->

### Checklist
- [ ] Changelog reviewed
- [ ] Version bump verified
- [ ] CI passing

After merging, create the tag on the merge commit:
\`\`\`
git tag vX.Y.Z <merge-commit-sha>
git push origin vX.Y.Z
\`\`\`
EOF
)"
```

```PowerShell
# Create PR body using here-string (preserves actual newlines, not escape sequences)
$prBody = @"
## Release vX.Y.Z

This PR prepares the **vX.Y.Z** release.

### What's included
<paste changelog here>

### Checklist
- [ ] Changelog reviewed
- [ ] Version bump verified
- [ ] CI passing

After merging, create the tag on the merge commit:
``````
git标签vX.Y.Z<merge-commit-sha>git push原点vX.Y.Z``````
"@

# Write to file and use --body-file (do NOT use inline --body with escape sequences)
$prBody | Out-File -FilePath release_pr_body.md -Encoding utf8 -NoNewline
gh pr create --base main --head release/vX.Y.Z --title "Release vX.Y.Z" --body-file release_pr_body.md
```
将变更日志部分粘贴到PR主体的“包含内容”块中（或者为手动审查留下占位符）。


---

###第9步-交给用户

告诉用户：

b> **发布公告已开启！?？**
>
>新版本：**vX.Y.Z**
>
一旦PR被审查和合并，你需要自己创建标签
>合并提交：
>
>“bash
> git标签vX.Y.Z<merge-commit-sha>> git push原点vX.Y.Z
> ' ' '
>
>然后去GitHub发布，从那个标签发布版本。你可以复制
> changelog部分直接放到发行说明中。

---

##错误处理

情景|该做什么||---|---|
|`gh auth status`失败|停止；告诉用户运行`gh auth login`|
|停止；告诉用户`cd`到他们的repo |
|工作树脏|警告；问他们是想藏起来还是放弃b|
告诉用户没有什么要发布的|
|使用第一次提交作为diff基数；警告用户|
|最新标签在本地存在但在远端不存在|警告用户；询问他们是想先推送标签还是继续b|
| Diff是空的`PUBLIC_PATH`，但提交存在|警告；所有的变化都可能是内部的；问他们是否还想继续b|
|`git push`失败（例如受保护的分支规则）|逐字报告错误；建议检查分支保护设置|

---

## PowerShell故障排除-如果一个命令在本地工作打印gh用法或将子命令作为单独的令牌，请确保您是
在PATH上调用gh.exe （Get-Command gh）并避免传递未展开的嵌套替换；使用PowerShell
上面的模式。
-推荐测试：gh——version；Git fetch——tags；运行PowerShell代码段设置$prevTag，然后运行git diff——name-only $prevSha..HEAD——src/

---

# #的局限性

—需要安装并认证`gh`命令行。
-需要git标签来确定当前版本。

---

##参考文件

-`references/semver-rules.md`-扩展SemVer决策规则和边缘情况
-`references/commit-classification.md`-将提交消息分类为更改类型的启发式方法