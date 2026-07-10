# Dependabot配置示例

常见场景的实际`dependabot.yml`配置。

---

# # 1。基本单一生态系统

单个npm项目的最小配置：```yaml
version: 2
updates:
  - package-ecosystem: "npm"
    directory: "/"
    schedule:
      interval: "weekly"
```
---

# # 2。带有全局模式的MonorepoTurborepo/pnpmmonorepo带有多个工作空间包：```yaml
version: 2
updates:
  - package-ecosystem: "npm"
    directories:
      - "/"
      - "/apps/*"
      - "/packages/*"
      - "/services/*"
    schedule:
      interval: "weekly"
      day: "monday"
    groups:
      dev-dependencies:
        dependency-type: "development"
        update-types: ["minor", "patch"]
      production-dependencies:
        dependency-type: "production"
        update-types: ["minor", "patch"]
    labels:
      - "dependencies"
      - "npm"
    commit-message:
      prefix: "deps"
      include: "scope"
```
---

# # 3。分组开发与生产依赖关系

分离开发和生产更新，优先审查生产更改：```yaml
version: 2
updates:
  - package-ecosystem: "npm"
    directory: "/"
    schedule:
      interval: "weekly"
    groups:
      production-deps:
        dependency-type: "production"
      dev-deps:
        dependency-type: "development"
        exclude-patterns:
          - "eslint*"
      linting:
        patterns:
          - "eslint*"
          - "prettier*"
          - "@typescript-eslint*"
```
---

# # 4。跨目录分组（Monorepo）

跨目录为每个共享依赖创建一个PR：```yaml
version: 2
updates:
  - package-ecosystem: "npm"
    directories:
      - "/frontend"
      - "/admin-panel"
      - "/mobile-app"
    schedule:
      interval: "weekly"
    groups:
      monorepo-dependencies:
        group-by: dependency-name
```
当`lodash`在所有三个目录中更新时，Dependabot创建一个单独的PR。

---

# # 5。多生态系统组（Docker + Terraform）

将基础设施依赖项更新合并到单个PR中：```yaml
version: 2

multi-ecosystem-groups:
  infrastructure:
    schedule:
      interval: "weekly"
    labels: ["infrastructure", "dependencies"]
    assignees: ["@platform-team"]

updates:
  - package-ecosystem: "docker"
    directory: "/"
    patterns: ["nginx", "redis", "postgres"]
    multi-ecosystem-group: "infrastructure"

  - package-ecosystem: "terraform"
    directory: "/"
    patterns: ["aws*", "terraform-*"]
    multi-ecosystem-group: "infrastructure"
```
---

# # 6。仅提供安全更新（禁用版本更新）

监控没有版本更新pr的安全漏洞：```yaml
version: 2
updates:
  - package-ecosystem: "npm"
    directory: "/"
    schedule:
      interval: "daily"
    open-pull-requests-limit: 0  # disables version update PRs
    groups:
      security-all:
        applies-to: security-updates
        patterns: ["*"]
        update-types: ["patch", "minor"]

  - package-ecosystem: "pip"
    directory: "/"
    schedule:
      interval: "daily"
    open-pull-requests-limit: 0
```
---

# # 7。私有注册中心

访问私有的npm和Docker注册表：```yaml
version: 2

registries:
  npm-private:
    type: npm-registry
    url: https://npm.internal.example.com
    token: ${{secrets.NPM_PRIVATE_TOKEN}}

  docker-ghcr:
    type: docker-registry
    url: https://ghcr.io
    username: ${{secrets.GHCR_USER}}
    password: ${{secrets.GHCR_TOKEN}}

updates:
  - package-ecosystem: "npm"
    directory: "/"
    registries:
      - npm-private
    schedule:
      interval: "weekly"

  - package-ecosystem: "docker"
    directory: "/"
    registries:
      - docker-ghcr
    schedule:
      interval: "weekly"
```
---

# # 8。冷却时间

延迟更新新发布的版本，以避免早期采用者的错误：```yaml
version: 2
updates:
  - package-ecosystem: "npm"
    directory: "/"
    schedule:
      interval: "weekly"
    cooldown:
      default-days: 5
      semver-major-days: 30
      semver-minor-days: 14
      semver-patch-days: 3
      include: ["*"]
      exclude:
        - "security-critical-lib"
        - "@company/internal-*"
```
---

# # 9。Cron调度

使用cron表达式在指定时间运行更新：```yaml
version: 2
updates:
  - package-ecosystem: "npm"
    directory: "/"
    schedule:
      interval: "cron"
      cronjob: "0 9 * * 1"  # Every Monday at 9:00 AM
      timezone: "America/New_York"

  - package-ecosystem: "github-actions"
    directory: "/"
    schedule:
      interval: "cron"
      cronjob: "0 6 1 * *"  # First day of each month at 6:00 AM
```
---

# # 10。全功能的配置

结合多种优化的综合示例：```yaml
version: 2

registries:
  npm-private:
    type: npm-registry
    url: https://npm.example.com
    token: ${{secrets.NPM_TOKEN}}

updates:
  # npm — monorepo workspaces
  - package-ecosystem: "npm"
    directories:
      - "/"
      - "/apps/*"
      - "/packages/*"
      - "/services/*"
    registries:
      - npm-private
    schedule:
      interval: "weekly"
      day: "monday"
      time: "09:00"
      timezone: "America/New_York"
    groups:
      dev-dependencies:
        dependency-type: "development"
        update-types: ["minor", "patch"]
      production-dependencies:
        dependency-type: "production"
        update-types: ["minor", "patch"]
      angular:
        patterns: ["@angular*"]
        update-types: ["minor", "patch"]
      security-patches:
        applies-to: security-updates
        patterns: ["*"]
        update-types: ["patch", "minor"]
    ignore:
      - dependency-name: "aws-sdk"
        update-types: ["version-update:semver-major"]
    cooldown:
      default-days: 3
      semver-major-days: 14
    labels:
      - "dependencies"
      - "npm"
    commit-message:
      prefix: "deps"
      prefix-development: "deps-dev"
      include: "scope"
    assignees:
      - "security-lead"
    open-pull-requests-limit: 15

  # GitHub Actions
  - package-ecosystem: "github-actions"
    directory: "/"
    schedule:
      interval: "weekly"
      day: "monday"
    groups:
      actions:
        patterns: ["*"]
    labels:
      - "dependencies"
      - "ci"
    commit-message:
      prefix: "ci"

  # Docker
  - package-ecosystem: "docker"
    directories:
      - "/services/*"
    schedule:
      interval: "weekly"
    labels:
      - "dependencies"
      - "docker"
    commit-message:
      prefix: "deps"

  # pip
  - package-ecosystem: "pip"
    directory: "/scripts"
    schedule:
      interval: "monthly"
    labels:
      - "dependencies"
      - "python"
    versioning-strategy: "increase-if-necessary"
    commit-message:
      prefix: "deps"

  # Terraform
  - package-ecosystem: "terraform"
    directory: "/infra"
    schedule:
      interval: "weekly"
    labels:
      - "dependencies"
      - "terraform"
    commit-message:
      prefix: "infra"
```
---

# # 11。忽略模式和版本控制策略

准确控制更新的内容和方式：```yaml
version: 2
updates:
  - package-ecosystem: "npm"
    directory: "/"
    schedule:
      interval: "daily"
    versioning-strategy: "increase"
    ignore:
      # Never auto-update to Express 5.x (breaking changes)
      - dependency-name: "express"
        versions: ["5.x"]
      # Skip patch updates for type definitions
      - dependency-name: "@types/*"
        update-types: ["version-update:semver-patch"]
      # Ignore all updates for a vendored package
      - dependency-name: "legacy-internal-lib"
    allow:
      - dependency-type: "all"
    exclude-paths:
      - "vendor/**"
      - "test/fixtures/**"
```
---

# # 12。目标非默认分支

在生产前测试开发分支上的更新：```yaml
version: 2
updates:
  - package-ecosystem: "npm"
    directory: "/"
    schedule:
      interval: "weekly"
    target-branch: "develop"
    labels:
      - "dependencies"
      - "staging"

  - package-ecosystem: "pip"
    directory: "/"
    schedule:
      interval: "weekly"
    target-branch: "develop"
```
注意：安全更新总是针对默认分支，而不管`target-branch`是什么。