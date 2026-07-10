# Dependabot PR注释命令

通过注释`@dependabot <command>`与Dependabot拉取请求交互。Dependabot会竖起大拇指回应命令。

> **弃用通知（2026年1月27日）：**以下命令已删除：`@dependabot merge`,`@dependabot squash and merge`,`@dependabot cancel merge`，
>`@dependabot close`和`@dependabot reopen`。
使用GitHub的原生UI、CLI （`gh pr merge`）、API或自动合并功能代替。

单个pr的命令

|命令|描述||---|---|
|`@dependabot rebase`|根据目标分支|重新设置PR
|`@dependabot recreate`|从头开始重新创建PR，覆盖任何手动编辑|
|`@dependabot ignore this dependency`|关闭PR并停止此依赖项|的所有未来更新
|`@dependabot ignore this major version`|关闭并停止此主要版本|的更新
|`@dependabot ignore this minor version`|关闭并停止此次要版本|的更新
|`@dependabot ignore this patch version`|关闭并停止此补丁版本|的更新
|`@dependabot show DEPENDENCY_NAME ignore conditions`|显示依赖性|当前所有忽略条件的表

用于分组更新的命令

这些命令用于由分组版本或安全更新创建的Dependabot pr。

|命令|描述||---|---|
|`@dependabot ignore DEPENDENCY_NAME`|关闭PR并停止更新组|中的此依赖项
|`@dependabot ignore DEPENDENCY_NAME major version`|停止更新此依赖的主版本|
|`@dependabot ignore DEPENDENCY_NAME minor version`|停止更新此依赖的次要版本|
|`@dependabot ignore DEPENDENCY_NAME patch version`|停止更新此依赖的补丁版本|
|`@dependabot unignore *`|关闭当前PR，清除所有忽略组中所有依赖项的条件，打开一个新的PR |
|`@dependabot unignore DEPENDENCY_NAME`|关闭当前的PR，清除所有对特定依赖项的忽略，打开一个带有更新|的新PR
|`@dependabot unignore DEPENDENCY_NAME IGNORE_CONDITION`|关闭当前PR，清除特定的忽略条件，打开一个新的PR |

##用法示例

合并后的CI（使用原生GitHub功能）

建议使用自动合并来替换已弃用的`@dependabot merge`命令：```bash
# Enable auto-merge via GitHub CLI
gh pr merge <PR_NUMBER> --auto --squash

# Or enable auto-merge via the GitHub UI:
# PR → "Enable auto-merge" → select merge method → confirm
```
一旦所有必需的CI检查通过，GitHub将自动合并PR。

###忽略一个主要的版本碰撞```
@dependabot ignore this major version
```
当主版本有重大变更且尚未计划迁移时非常有用。

检查活动忽略条件```
@dependabot show express ignore conditions
```
显示一个表，其中显示当前为`express`依赖项存储的所有忽略条件。

取消忽略组中的依赖项```
@dependabot unignore lodash
```
关闭当前分组的PR，清除`lodash`的所有忽略条件，并打开一个新的PR，其中包括可用的`lodash`更新。

取消忽略特定条件```
@dependabot unignore express [< 1.9, > 1.8.0]
```
仅为`express`清除指定的版本范围ignore。

# #提示- **Rebase vs重新创建**：使用`rebase`来解决冲突，同时保持您的审查状态。如果PR明显偏离，使用`recreate`重新开始。
- **强制推送额外的提交**：如果你已经将提交推送到一个Dependabot分支，并且想要Dependabot在这些提交上进行重基，在提交消息中包括`[dependabot skip]`。
- **持久忽略**：忽略通过PR注释集中存储的命令。为了团队回购的透明度，更喜欢在`dependabot.yml`中使用`ignore`。
- **合并依赖的pr **：使用GitHub的本地自动合并功能，CLI （`gh pr merge`）或web UI。旧的`@dependabot merge`命令已于2026年1月弃用。
—**Closing/Reopening**：使用GitHub UI或CLI。旧的`@dependabot close`和`@dependabot reopen`命令已于2026年1月弃用。
- **分组命令**：当使用`@dependabot unignore`时，Dependabot关闭当前的PR并打开一个带有更新的依赖集的新PR。