---
name: dependabot
description: >-
  Comprehensive guide for configuring and managing GitHub Dependabot. Use this skill when
  users ask about creating or optimizing dependabot.yml files, managing Dependabot pull requests,
  configuring dependency update strategies, setting up grouped updates, monorepo patterns,
  multi-ecosystem groups, security update configuration, auto-triage rules, or any GitHub
  Advanced Security (GHAS) supply chain security topic related to Dependabot. For pre-commit
  dependency vulnerability scanning in AI coding agents via the GitHub MCP Server, this skill
  references the Advanced Security plugin (`advanced-security@copilot-plugins`). Use this skill
  when an agent needs to scan dependencies for known vulnerabilities before committing.
---
# Dependabot配置与管理

# #概述

Dependabot是GitHub内置的依赖管理工具，有三个核心功能：

1. **Dependabot Alerts** -当依赖项有已知漏洞（cve）时通知
2. **Dependabot安全更新** -自动创建pr来修复易受攻击的依赖项
3. **Dependabot版本更新** -自动创建pr以保持当前的依赖关系

所有配置都保存在默认分支上的单个文件：`.github/dependabot.yml`中。GitHub **不**支持每个存储库中的多个`dependabot.yml`文件。

##配置流程

在创建或优化`dependabot.yml`时，请遵循以下过程：

步骤1：检测所有生态系统

扫描存储库查找依赖项清单。寻找:

|生态系统| YAML值| Manifest文件||---|---|---|
|npm/pnpm/yarn|`npm`|`package.json`,`package-lock.json`,`pnpm-lock.yaml`,`yarn.lock`|
|pip/pipenv/poetry|`pip`|`requirements.txt`,`Pipfile`,`pyproject.toml`,`setup.py`|
| uv |`uv`|`pyproject.toml`,`uv.lock`|
| Docker |`docker`|`Dockerfile`|
| Docker组成|`docker-compose`|`docker-compose.yml`|
|GitHub Actions|`github-actions`|`.github/workflows/*.yml`|
| Go模块|`gomod`|`go.mod`|
|捆绑器（Ruby） |`bundler`|`Gemfile`|
|货运（Rust） |`cargo`|`Cargo.toml`|
|作曲家（PHP） |`composer`|`composer.json`|
| NuGet （.）. NET) |`nuget`|`*.csproj`,`packages.config`|
|。. NET SDK |`dotnet-sdk`|`global.json`|
| Maven (Java) |`maven`|`pom.xml`|
Gradle (Java) |`gradle`|`build.gradle`|
| Terraform |`terraform`|`*.tf`|
|`opentofu`|`*.tf`|
| Helm |`helm`|`Chart.yaml`|
| Hex（酏剂）|`mix`|`mix.exs`|
| Swift |`swift`|`Package.swift`|
| Pub (Dart) |`pub`|`pubspec.yaml`|
| |`bun`|`bun.lockb`|
|开发容器|`devcontainers`|`devcontainer.json`|
| Git子模块|`gitsubmodule`|`.gitmodules`|
|预提交|`pre-commit`|`.pre-commit-config.yaml`|注:
- PNPM和yarn都使用`npm`生态系统值。
-当`uv.lock`存在时，更倾向于`uv`的生态价值；否则使用`pip`。

步骤2：映射目录位置

对于每个生态系统，确定清单所在的位置。将`directories`（复数）与glob模式一起用于单节点：```yaml
directories:
  - "/"           # root
  - "/apps/*"     # all app subdirs
  - "/packages/*" # all package subdirs
  - "/lib-*"      # dirs starting with lib-
  - "**/*"        # recursive (all subdirs)
```
重要：`directory`（单数）不支持globs。使用`directories`（复数）作为通配符。

步骤3：配置每个生态系统入口

每个条目至少需要：```yaml
- package-ecosystem: "npm"
  directory: "/"
  schedule:
    interval: "weekly"
```
###步骤4：优化分组，标签和调度

有关每种优化技术，请参见下面各节。

## Monorepo策略

工作空间覆盖的Glob模式

对于包含许多包的单节点，使用glob模式来避免列出每个目录：```yaml
- package-ecosystem: "npm"
  directories:
    - "/"
    - "/apps/*"
    - "/packages/*"
    - "/services/*"
  schedule:
    interval: "weekly"
```
跨目录分组

使用`group-by: dependency-name`在相同的依赖项跨多个目录更新时创建单个PR：```yaml
groups:
  monorepo-deps:
    group-by: dependency-name
```
这将在所有指定目录中为每个依赖项创建一个PR，从而减少CI成本和审查负担。

限制:
—所有目录必须使用相同的包生态系统
—仅适用于版本更新
-不兼容的版本约束创建单独的pr

工作空间之外的独立包

如果一个目录有自己的lockfile，并且不是工作空间的一部分（例如，`.github/`中的脚本），那么为它创建一个单独的生态系统条目。

##依赖分组

通过将相关的依赖项分组到单个PR中来减少PR噪声。

###根据依赖类型```yaml
groups:
  dev-dependencies:
    dependency-type: "development"
    update-types: ["minor", "patch"]
  production-dependencies:
    dependency-type: "production"
    update-types: ["minor", "patch"]
```
###按名称模式```yaml
groups:
  angular:
    patterns: ["@angular*"]
    update-types: ["minor", "patch"]
  testing:
    patterns: ["jest*", "@testing-library*", "ts-jest"]
```
###安全更新```yaml
groups:
  security-patches:
    applies-to: security-updates
    patterns: ["*"]
    update-types: ["patch", "minor"]
```
关键行为:
—匹配多个组的依赖项到**first** match
—“`applies-to`”不在位时默认为“`version-updates`”
-未分组的依赖得到单独的pr

##多生态系统组

将跨不同包生态系统的更新合并到单个PR中：```yaml
version: 2

multi-ecosystem-groups:
  infrastructure:
    schedule:
      interval: "weekly"
    labels: ["infrastructure", "dependencies"]

updates:
  - package-ecosystem: "docker"
    directory: "/"
    patterns: ["nginx", "redis"]
    multi-ecosystem-group: "infrastructure"

  - package-ecosystem: "terraform"
    directory: "/"
    patterns: ["aws*"]
    multi-ecosystem-group: "infrastructure"
```
使用`multi-ecosystem-group`时需要`patterns`键。

## PR定制

# # #的标签```yaml
labels:
  - "dependencies"
  - "npm"
```
设置`labels: []`禁用所有标签，包括默认值。SemVer标签（`major`,`minor`,`patch`）如果在repo中存在，则始终应用。

提交消息```yaml
commit-message:
  prefix: "deps"
  prefix-development: "deps-dev"
  include: "scope"  # adds deps/deps-dev scope after prefix
```
任务分配和里程碑```yaml
assignees: ["security-team-lead"]
milestone: 4  # numeric ID from milestone URL
```
分支名称分隔符```yaml
pull-request-branch-name:
  separator: "-"  # default is /
```
目标分支```yaml
target-branch: "develop"  # PRs target this instead of default branch
```
注意：当设置`target-branch`时，安全更新仍然针对默认分支；所有生态系统配置仅适用于版本更新。

##调度优化

# # #的间隔

支持：`daily`、`weekly`、`monthly`、`quarterly`、`semiannually`、`yearly`、`cron````yaml
schedule:
  interval: "weekly"
  day: "monday"         # for weekly only
  time: "09:00"         # HH:MM format
  timezone: "America/New_York"
```
Cron表达式```yaml
schedule:
  interval: "cron"
  cronjob: "0 9 * * 1"  # Every Monday at 9 AM
```
冷却时间

延迟更新新发布的版本，以避免早期采用者的问题；```yaml
cooldown:
  default-days: 5
  semver-major-days: 30
  semver-minor-days: 7
  semver-patch-days: 3
  include: ["*"]
  exclude: ["critical-lib"]
```
冷却时间只适用于版本更新，而不适用于安全更新。

##安全更新配置

通过存储库设置启用

设置→高级安全→启用Dependabot警报、安全更新和分组安全更新。

在YAML中组安全更新```yaml
groups:
  security-patches:
    applies-to: security-updates
    patterns: ["*"]
    update-types: ["patch", "minor"]
```
###禁用版本更新（仅限安全）```yaml
open-pull-requests-limit: 0  # disables version update PRs
```
###自动分类规则

GitHub为开发依赖预置了自动解散低影响警报。自定义规则可以根据严重性、包名称、CWE等进行过滤。在存储库设置→高级安全中配置。

## PR评论命令

使用`@dependabot`注释与Dependabot pr交互。

> **注：**自2026年1月起，merge/close/reopen命令已弃用。
>使用GitHub的本地UI， CLI (`gh pr merge`)，或自动合并代替。

|命令|生效||---|---|
|`@dependabot rebase`|重置PR |
|`@dependabot recreate`|重新创建PR |
|`@dependabot ignore this dependency`|关闭并永不更新此依赖|
|`@dependabot ignore this major version`|忽略此主版本|
|`@dependabot ignore this minor version`|忽略这个次要版本|
|`@dependabot ignore this patch version`|忽略此补丁版本|

对于分组pr，附加命令：
-`@dependabot ignore DEPENDENCY_NAME`-忽略组中的特定依赖项
-`@dependabot unignore DEPENDENCY_NAME`-清除忽略，重新打开更新
-`@dependabot unignore *`-清除组中所有依赖项的忽略
-`@dependabot show DEPENDENCY_NAME ignore conditions`-忽略显示电流

完整的命令参考，请参见`references/pr-commands.md`。

忽略和允许规则

忽略特定的依赖项```yaml
ignore:
  - dependency-name: "lodash"
  - dependency-name: "@types/node"
    update-types: ["version-update:semver-patch"]
  - dependency-name: "express"
    versions: ["5.x"]
```
###只允许特定类型```yaml
allow:
  - dependency-type: "production"
  - dependency-name: "express"
```
规则：如果一个依赖项同时匹配`allow`和`ignore`，则**忽略**。

###排除路径```yaml
exclude-paths:
  - "vendor/**"
  - "test/fixtures/**"
```
##高级选项

版本控制策略

控制Dependabot编辑版本约束的方式：

|值|行为||---|---|
|`auto`|默认-应用程序增加，库扩大|
|`increase`|始终增加最低版本|
|`increase-if-necessary`|仅当当前范围不包括新版本|时更改
|`lockfile-only`|只更新lockfiles，忽略manifest |
|`widen`|扩大范围，包括新旧版本|

Rebase策略```yaml
rebase-strategy: "disabled"  # stop auto-rebasing
```
通过在提交消息中包含`[dependabot skip]`，允许对额外的提交进行重基。

###打开公关限制```yaml
open-pull-requests-limit: 10  # default is 5 for version, 10 for security
```
设置为`0`以完全禁用版本更新。

私有注册表```yaml
registries:
  npm-private:
    type: npm-registry
    url: https://npm.example.com
    token: ${{secrets.NPM_TOKEN}}

updates:
  - package-ecosystem: "npm"
    directory: "/"
    registries:
      - npm-private
```
# #常见问题解答

**我可以有多个`dependabot.yml`文件？**
否。GitHub只支持一个文件在`.github/dependabot.yml`。在该文件中为不同的生态系统和目录使用多个`updates`条目。

** Dependabot支持pnpm吗？**
是。使用`package-ecosystem: "npm"`- Dependabot自动检测`pnpm-lock.yaml`。

**如何减少单笔交易中的PR噪音？**
使用`groups`进行批处理更新，使用`directories`与globs进行覆盖，使用`group-by: dependency-name`进行跨目录分组。考虑低优先级生态系统的`monthly`或`quarterly`间隔。

**如何处理工作区外的依赖关系？**
创建一个单独的生态系统条目，使用它自己的`directory`指向该位置。

通过AI编码代理进行预提交依赖项扫描为了在提交之前扫描AI编码代理中易受攻击的依赖项的代码更改，GitHub MCP服务器的`dependabot`工具集可以根据GitHub咨询数据库检查您的依赖项添加，并返回包含受影响包、严重性和建议的修复版本的结构化结果。要进行更彻底的提交后检查，还可以在本地运行Dependabot CLI来区分更改前后的依赖关系图。

安装**高级安全插件**，它提供了专用的依赖项扫描工具和`/dependency-scanning`技能。

**GitHub Copilot命令行（shell）：**```bash
# Enable the dependabot toolset for the GitHub MCP Server
copilot --add-github-mcp-toolset dependabot
```
**GitHub Copilot命令行（`copilot`内部）：**```text
> /plugin install advanced-security@copilot-plugins
```
**Visual Studio代码：**
-添加`"X-MCP-Toolsets": "dependabot"`到你的GitHub MCP服务器头，或选择**Dependabot**从工具集选择器在Copilot聊天
-安装`advanced-security`插件，然后在副驾驶聊天中使用`/dependency-scanning`* *例提示:* *
>扫描我在此分支上添加的依赖项以查找已知漏洞，并在提交之前告诉我要升级到哪个版本。

参见：[高级安全插件-依赖扫描技能]（https://github.com/github/copilot-plugins/blob/main/plugins/advanced-security/skills/dependency-scanning/SKILL.md）

>在[GitHub MCP服务器的依赖项扫描公开预览](https://github.blog/changelog/2026-05-05-dependency-scanning-with-github-mcp-server-is-in-public-preview/)（2026年5月）中宣布

# #资源

-`references/dependabot-yml-reference.md`-完成YAML选项参考
-`references/pr-commands.md`-完整的PR注释命令参考
-`references/example-configs.md`-实际配置示例