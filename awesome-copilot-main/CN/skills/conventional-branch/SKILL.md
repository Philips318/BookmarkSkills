---
name: conventional-branch
description: 'Create Git branches following the Conventional Branch specification (feature/, bugfix/, hotfix/, release/, chore/). Use when creating a new branch, naming a branch, or checking whether a branch name complies with the spec.'
---
#常规分支

创建遵循[Conventional Branch]（https://conventional-branch.github.io）规范的Git分支——一个简单、一致的命名Git分支的约定。

分支名称格式```
<type>/<description>
```
分支类型

|类型|别名|用途||------|-------|---------|
|`feature/`|`feat/`| |的新特性或增强
修复|的Bug
|`hotfix/`| - |紧急生产修复|
|`release/`| - |发布准备（版本允许点：`release/v1.2.0`） |
|`chore/`| - |非代码任务（deps, docs, config） |

###主干分支`main`、`master`和`develop`是主干分支—它们不使用前缀。永远不要创建与主干分支名称相同的新分支；把它们分开。

##命名规则

—**只支持小写字母**—不支持大写字母
- **数字、连字符、点** -`a-z`、`0-9`、`-`、`.`**在`release/`版本描述中只允许**点（例如，`release/v1.2.0`）
—**禁止使用下划线、空格和特殊字符**
- **没有连续的连字符** (`--`)， **点** (`..`)或**连字符-点邻接** （`-.`或`.-`）
- **描述信息中不能有“-”和“**”##有效示例```
main
master
develop
feature/add-login-page
feat/add-login-page
bugfix/fix-header-bug
fix/header-bug
hotfix/security-patch
release/v1.2.0
chore/update-dependencies
feature/issue-123-new-login
```
##无效示例

|分支|问题||--------|---------|
|`Feature/Add-Login`|大写字母|
|`feature/new--login`|连续连字符|
|`feature/-new-login`|前导连字符|
|`feature/new-login-`|后连字符|
|`release/v1.-2.0`|点|相邻的连字符
|`fix/header bug`|空间|
|`fix/header_bug`|下划线|
|`unknown/some-task`|未知前缀类型|

##描述指南

-使用**烤肉盒**，2-5个单词
-描述性的，但要简洁（总共50个字符）
-好：`add-oauth-login`，`fix-header-overflow`,`update-ci-config`—坏：`fix-bug`、`new-feature`# #工作流程

请遵循以下步骤

**步骤1 -确定分支类型**

询问用户（如果还不清楚）：

- **分支类型** -不确定时默认为`feature`- **简要描述** -分支是用来做什么的

如果用户提到一个票据或问题号，在描述中包括它（例如，`feature/issue-123-add-oauth`）。

**步骤2 -验证名称**

根据上面的**命名规则**检查组装的名称。如果任何规则失败，修复它：-全部小写
—用连字符代替下划线和空格
-折叠连续的连字符
—条带“leading/trailing”连字符

**步骤3 -检测基础分支**

不同的仓库使用不同的主干分支。检测这个仓库使用的是哪一个：```bash
# Prefer the remote's default branch
git symbolic-ref --short refs/remotes/origin/HEAD 2>/dev/null | sed 's|^origin/||'
```
如果没有返回任何结果，检查本地存在哪个主干分支（优先级顺序：`develop`，`main`,`master`）：```bash
for b in develop main master; do
  git show-ref --verify --quiet "refs/heads/$b" && echo "$b" && break
done
```
**步骤4 -创建和签出**```bash
git checkout <base>
git pull origin <base>
git checkout -b <type>/<description>
```
**步骤5 -确认**

告诉用户：
—创建的分支名称
-他们现在在新的分支上
-提醒他们：`git push -u origin <branch-name>`准备好了

与常规提交的关系

常规分支补充[常规提交](https://www.conventionalcommits.org)：

|常规分支|典型常规提交||---------------------|----------------------------|
|`feature/add-login`|`feat: add login page`|
|`bugfix/fix-header`|`fix: header overflow on mobile`|
|`chore/update-deps`|`chore: bump lodash to 5.0`|
|`release/v1.2.0`|`chore: release v1.2.0`|

尽可能将分支类型与提交类型对齐（例如，`feature/*`分支与`feat:`提交）。