# Dependabot YAML选项参考`.github/dependabot.yml`中所有配置选项的完整参考。

##文件结构```yaml
version: 2                    # Required, always 2

registries:                   # Optional: private registry access
  REGISTRY_NAME:
    type: "..."
    url: "..."

multi-ecosystem-groups:       # Optional: cross-ecosystem grouping
  GROUP_NAME:
    schedule:
      interval: "..."

updates:                      # Required: list of ecosystem configurations
  - package-ecosystem: "..."  # Required
    directory: "/"            # Required (or directories)
    schedule:                 # Required
      interval: "..."
```
##必需的键

# # #`version`总是`2`。一定是最高级别的。

# # #`package-ecosystem`定义要监视哪个包管理器。每个生态系统一个条目（同一生态系统可以有多个条目，但目录不同）。

|包管理器| YAML值| Manifest文件||---|---|---|
| Bazel |`bazel`|`MODULE.bazel`,`WORKSPACE`|
| |`bun`|`bun.lockb`|
|捆绑器（Ruby） |`bundler`|`Gemfile`,`Gemfile.lock`|
|货运（Rust） |`cargo`|`Cargo.toml`,`Cargo.lock`|
|`composer`|`composer.json`,`composer.lock`|
| Conda |`conda`|`environment.yml`|
|开发容器|`devcontainers`|`devcontainer.json`|
| Docker |`docker`|`Dockerfile`|
| Docker组成|`docker-compose`|`docker-compose.yml`|
|。. NET SDK |`dotnet-sdk`|`global.json`|
|榆树|`elm`|`elm.json`|
| Git子模块|`gitsubmodule`|`.gitmodules`|
|GitHub Actions|`github-actions`|`.github/workflows/*.yml`|
| Go模块|`gomod`|`go.mod`，`go.sum`|
| Gradle |`gradle`|`build.gradle`,`build.gradle.kts`|
| Helm |`helm`|`Chart.yaml`|
| Hex（酏剂）|`mix`|`mix.exs`,`mix.lock`|
| Julia |`julia`|`Project.toml`,`Manifest.toml`|
| Maven |`maven`|`pom.xml`|
|npm/pnpm/yarn|`npm`|`package.json`, lockfiles |
| NuGet |`nuget`|`*.csproj`,`packages.config`|
| OpenTofu |`opentofu`|`*.tf`|
|pip/pipenv/poetry/uv|`pip`|`requirements.txt`，`Pipfile`,`pyproject.toml`|
|预提交|`pre-commit`|`.pre-commit-config.yaml`|
| Pub (Dart/Flutter) |`pub`|`pubspec.yaml`|
| Rust工具链|`rust-toolchain`|`rust-toolchain.toml`|
| Swift |`swift`|`Package.swift`|
| Terraform |`terraform`|`*.tf`|
| uv |`uv`|`uv.lock`,`pyproject.toml`|
| VCPKG |`vcpkg`|`vcpkg.json`|###`directory`/`directories`包清单相对于repo根的位置。

-`directory`-单路径（不支持glob）
-`directories`-路径列表（支持`*`和`**`globs）```yaml
# Single directory
directory: "/"

# Multiple directories with globs
directories:
  - "/"
  - "/apps/*"
  - "/packages/*"
```
对于GitHub Actions，使用`/`—Dependabot会自动搜索`.github/workflows/`。

# # #`schedule`检查更新的频率。

| |参数值|备注||---|---|---|
|`interval`|`daily`,`weekly`,`monthly`,`quarterly`,`semiannually`,`yearly`，`cron`|必选|
|`day`|`monday`-`sunday`|每周仅|
|`time`|`HH:MM`|默认为UTC时间|
|`timezone`| IANA时区字符串|，例如`America/New_York`|
|`cronjob`| Cron表达式|当interval为`cron`|时需要```yaml
schedule:
  interval: "weekly"
  day: "tuesday"
  time: "09:00"
  timezone: "Europe/London"
```
##分组选项

# # #`groups`将依赖项分组到更少的pr中。

| |参数用途|取值||---|---|---|
|`IDENTIFIER`|组名（用于branch/PR标题）|字母、管道、下划线、连字符|
|`applies-to`|更新类型|`version-updates`（默认），`security-updates`|
|`dependency-type`|按类型|`development`，`production`|过滤
|`patterns`|包含匹配名称|带有`*`通配符|的字符串列表
|`exclude-patterns`|排除匹配的名称|带有`*`通配符|的字符串列表
|`update-types`| SemVer过滤器|`major`，`minor`,`patch`|
|`group-by`|跨目录分组|`dependency-name`|```yaml
groups:
  dev-deps:
    dependency-type: "development"
    update-types: ["minor", "patch"]
  angular:
    patterns: ["@angular*"]
    exclude-patterns: ["@angular/cdk"]
  monorepo:
    group-by: dependency-name
```
###`multi-ecosystem-groups`（顶级）

将跨不同生态系统的更新组合到一个PR中。```yaml
multi-ecosystem-groups:
  GROUP_NAME:
    schedule:
      interval: "weekly"
    labels: ["infrastructure"]
    assignees: ["@platform-team"]
```
在每个`updates`条目中使用`multi-ecosystem-group: "GROUP_NAME"`分配生态系统。在使用此特性时，每个生态系统条目都需要`patterns`键。

##过滤选项

# # #`allow`显式定义要维护的依赖项。

|参数名称参数含义|用途||---|---|
|`dependency-name`|按名称匹配（支持`*`通配符）|
|`dependency-type`|`direct`,`indirect`,`all`,`production`,`development`|```yaml
allow:
  - dependency-type: "production"
  - dependency-name: "express"
```
# # #`ignore`从更新中排除依赖项或版本。

|参数名称参数含义|用途||---|---|
|`dependency-name`|按名称匹配（支持`*`通配符）|
|`versions`|特定版本或范围（例如，`["5.x"]`,`[">=2.0.0"]`） |`update-types`| SemVer级别：`version-update:semver-major`，`version-update:semver-minor`,`version-update:semver-patch`|```yaml
ignore:
  - dependency-name: "lodash"
  - dependency-name: "@types/node"
    update-types: ["version-update:semver-patch"]
  - dependency-name: "express"
    versions: ["5.x"]
```
规则：如果一个依赖项同时匹配`allow`和`ignore`，则**忽略**。

# # #`exclude-paths`在清单扫描期间忽略特定的目录或文件。```yaml
exclude-paths:
  - "vendor/**"
  - "test/fixtures/**"
  - "*.lock"
```
支持全局模式：`*`（单段）、`**`（递归）、特定文件路径。

## PR定制选项

# # #`labels````yaml
labels:
  - "dependencies"
  - "npm"
```
设置`labels: []`禁用所有标签。如果repo中存在SemVer标签，则总是应用SemVer标签。

# # #`assignees````yaml
assignees:
  - "user1"
  - "user2"
```
受让人必须具有写访问权限（或org repos的读访问权限）。

# # #`milestone````yaml
milestone: 4  # numeric ID from milestone URL
```
# # #`commit-message````yaml
commit-message:
  prefix: "deps"              # up to 50 chars; colon auto-added if ends with letter/number
  prefix-development: "deps-dev"  # separate prefix for dev dependencies
  include: "scope"            # adds deps/deps-dev after prefix
```
# # #`pull-request-branch-name````yaml
pull-request-branch-name:
  separator: "-"  # options: "-", "_", "/"
```
# # #`target-branch````yaml
target-branch: "develop"
```
设置后，版本更新配置仅适用于版本更新。安全更新总是针对默认分支。

##调度和速率限制

# # #`cooldown`对于新发布的版本，延迟版本更新：

|参数名称参数含义|用途||---|---|
|`default-days`|默认冷却时间（1-90天）|
|`semver-major-days`|主要更新的冷却时间
|`semver-minor-days`|冷却小更新|
|`semver-patch-days`|补丁更新冷却时间|
|`include`|依赖应用冷却（高达150，支持`*`） |
|`exclude`|依赖免除冷却（最多150，优先）|```yaml
cooldown:
  default-days: 5
  semver-major-days: 30
  semver-minor-days: 7
  semver-patch-days: 3
  include: ["*"]
  exclude: ["critical-security-lib"]
```
# # #`open-pull-requests-limit````yaml
open-pull-requests-limit: 10  # default: 5 for version updates
```
设置为`0`以完全禁用版本更新。安全更新有单独的10个内部限制。

##高级选项

# # #`versioning-strategy`支持：`bundler`、`cargo`、`composer`、`mix`、`npm`、`pip`、`pub`、`uv`。

|值|行为||---|---|
|`auto`|默认值：应用增大，库增大|
|`increase`|始终增加最低版本|
|`increase-if-necessary`|仅当当前范围不包括新版本|时更改
|`lockfile-only`|只更新lockfiles |
|`widen`|扩大范围，包括新旧版本|

# # #`rebase-strategy````yaml
rebase-strategy: "disabled"
```
默认行为：依赖于根据冲突自动重置pr。PR开启后30天停止重新部署。

允许Dependabot通过在提交消息中包含`[dependabot skip]`来强制推送额外的提交。

# # #`vendor`支持：`bundler`，`gomod`。```yaml
vendor: true  # maintain vendored dependencies
```
Go模块自动检测供应商的依赖。

# # #`insecure-external-code-execution`支持：`bundler`、`mix`、`pip`。```yaml
insecure-external-code-execution: "allow"
```
允许Dependabot在更新期间执行清单中的代码。一些在解析过程中运行代码的生态系统需要。

私有注册表

顶级注册表定义```yaml
registries:
  npm-private:
    type: npm-registry
    url: https://npm.example.com
    token: ${{secrets.NPM_TOKEN}}

  maven-central:
    type: maven-repository
    url: https://repo.maven.apache.org/maven2
    username: ""
    password: ""

  docker-ghcr:
    type: docker-registry
    url: https://ghcr.io
    username: ${{secrets.GHCR_USER}}
    password: ${{secrets.GHCR_TOKEN}}

  python-private:
    type: python-index
    url: https://pypi.example.com/simple
    token: ${{secrets.PYPI_TOKEN}}
```
###将注册表与生态系统关联起来```yaml
updates:
  - package-ecosystem: "npm"
    directory: "/"
    registries:
      - npm-private
    schedule:
      interval: "weekly"
```
使用`registries: "*"`允许访问所有已定义的注册中心。