---
name: ci-cd-and-automation
description: 自动化 CI/CD pipeline setup。使用场景：设置或修改 build 与 deployment pipelines。使用场景：需要自动化 quality gates、在 CI 中配置 test runners，或建立 deployment strategies。
---

# CI/CD 与自动化

## 概述

自动化 quality gates，确保任何变更在通过 tests、lint、type checking 和 build 之前都无法进入生产环境。CI/CD 是其他每项技能的执行机制，它能捕获人类和 agents 漏掉的问题，并且在每一次变更上保持一致执行。

**Shift Left:** 尽可能早地在 pipeline 中发现问题。在 linting 中发现 bug 的成本是几分钟；同一个 bug 在生产环境中发现的成本是几小时。将 checks 前移，static analysis 先于 tests，tests 先于 staging，staging 先于 production。

**Faster is Safer:** 更小的批次和更频繁的发布会降低风险，而不是增加风险。包含 3 个变更的 deployment 比包含 30 个变更的 deployment 更容易调试。频繁发布会增强对发布流程本身的信心。

## 使用场景

- 设置新项目的 CI pipeline
- 添加或修改 automated checks
- 配置 deployment pipelines
- 当某项变更应触发 automated verification 时
- 调试 CI failures

## Quality Gate Pipeline

每个变更在合并前都要经过这些 gates：

```
Pull Request Opened
    │
    ▼
┌─────────────────┐
│   LINT CHECK     │  eslint, prettier
│   ↓ pass         │
│   TYPE CHECK     │  tsc --noEmit
│   ↓ pass         │
│   UNIT TESTS     │  jest/vitest
│   ↓ pass         │
│   BUILD          │  npm run build
│   ↓ pass         │
│   INTEGRATION    │  API/DB tests
│   ↓ pass         │
│   E2E (optional) │  Playwright/Cypress
│   ↓ pass         │
│   SECURITY AUDIT │  npm audit
│   ↓ pass         │
│   BUNDLE SIZE    │  bundlesize check
└─────────────────┘
    │
    ▼
  Ready for review
```

**任何 gate 都不能跳过。** 如果 lint 失败，就修复 lint，不要禁用规则。如果 test 失败，就修复代码，不要跳过测试。

## GitHub Actions Configuration

### Basic CI Pipeline

```yaml
# .github/workflows/ci.yml
name: CI

on:
  pull_request:
    branches: [main]
  push:
    branches: [main]

jobs:
  quality:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4

      - uses: actions/setup-node@v4
        with:
          node-version: '22'
          cache: 'npm'

      - name: Install dependencies
        run: npm ci

      - name: Lint
        run: npm run lint

      - name: Type check
        run: npx tsc --noEmit

      - name: Test
        run: npm test -- --coverage

      - name: Build
        run: npm run build

      - name: Security audit
        run: npm audit --audit-level=high
```

### With Database Integration Tests

```yaml
  integration:
    runs-on: ubuntu-latest
    services:
      postgres:
        image: postgres:16
        env:
          POSTGRES_DB: testdb
          POSTGRES_USER: ci_user
          POSTGRES_PASSWORD: ${{ secrets.CI_DB_PASSWORD }}
        ports:
          - 5432:5432
        options: >-
          --health-cmd pg_isready
          --health-interval 10s
          --health-timeout 5s
          --health-retries 5

    steps:
      - uses: actions/checkout@v4
      - uses: actions/setup-node@v4
        with:
          node-version: '22'
          cache: 'npm'
      - run: npm ci
      - name: Run migrations
        run: npx prisma migrate deploy
        env:
          DATABASE_URL: postgresql://ci_user:${{ secrets.CI_DB_PASSWORD }}@localhost:5432/testdb
      - name: Integration tests
        run: npm run test:integration
        env:
          DATABASE_URL: postgresql://ci_user:${{ secrets.CI_DB_PASSWORD }}@localhost:5432/testdb
```

> **注意：** 即使是仅用于 CI 的测试数据库，也要使用 GitHub Secrets 保存凭据，而不是硬编码值。这会建立良好习惯，并防止测试凭据在其他上下文中被意外复用。

### E2E Tests

```yaml
  e2e:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - uses: actions/setup-node@v4
        with:
          node-version: '22'
          cache: 'npm'
      - run: npm ci
      - name: Install Playwright
        run: npx playwright install --with-deps chromium
      - name: Build
        run: npm run build
      - name: Run E2E tests
        run: npx playwright test
      - uses: actions/upload-artifact@v4
        if: failure()
        with:
          name: playwright-report
          path: playwright-report/
```

## 将 CI Failures 反馈给 Agents

CI 与 AI agents 结合的力量来自反馈循环。当 CI 失败时：

```
CI fails
    │
    ▼
Copy the failure output
    │
    ▼
Feed it to the agent:
"The CI pipeline failed with this error:
[paste specific error]
Fix the issue and verify locally before pushing again."
    │
    ▼
Agent fixes → pushes → CI runs again
```

**关键模式：**

```
Lint failure → Agent runs `npm run lint --fix` and commits
Type error  → Agent reads the error location and fixes the type
Test failure → Agent follows debugging-and-error-recovery skill
Build error → Agent checks config and dependencies
```

## Deployment Strategies

### Preview Deployments

每个 PR 都获得一个 preview deployment，用于手动测试：

```yaml
# Deploy preview on PR (Vercel/Netlify/etc.)
deploy-preview:
  runs-on: ubuntu-latest
  if: github.event_name == 'pull_request'
  steps:
    - uses: actions/checkout@v4
    - name: Deploy preview
      run: npx vercel --token=${{ secrets.VERCEL_TOKEN }}
```

### Feature Flags

Feature flags 将 deployment 与 release 解耦。把未完成或高风险功能部署在 flags 后面，这样你可以：

- **发布代码而不启用它。** 尽早合并到 main，准备好后再启用。
- **无需重新部署即可回滚。** 禁用 flag，而不是 revert code。
- **Canary 新功能。** 先为 1% 的用户启用，然后 10%，再到 100%。
- **运行 A/B tests。** 比较有无该功能时的行为。

```typescript
// Simple feature flag pattern
if (featureFlags.isEnabled('new-checkout-flow', { userId })) {
  return renderNewCheckout();
}
return renderLegacyCheckout();
```

**Flag lifecycle:** 创建 → 为测试启用 → Canary → 全量 rollout → 移除 flag 和 dead code。永久存在的 flags 会变成 technical debt。创建时就设置清理日期。

### Staged Rollouts

```
PR merged to main
    │
    ▼
  Staging deployment (auto)
    │ Manual verification
    ▼
  Production deployment (manual trigger or auto after staging)
    │
    ▼
  Monitor for errors (15-minute window)
    │
    ├── Errors detected → Rollback
    └── Clean → Done
```

### Rollback Plan

每次 deployment 都应可逆：

```yaml
# Manual rollback workflow
name: Rollback
on:
  workflow_dispatch:
    inputs:
      version:
        description: 'Version to rollback to'
        required: true

jobs:
  rollback:
    runs-on: ubuntu-latest
    steps:
      - name: Rollback deployment
        run: |
          # Deploy the specified previous version
          npx vercel rollback ${{ inputs.version }}
```

## Environment Management

```
.env.example       → Committed (template for developers)
.env                → NOT committed (local development)
.env.test           → Committed (test environment, no real secrets)
CI secrets          → Stored in GitHub Secrets / vault
Production secrets  → Stored in deployment platform / vault
```

CI 永远不应持有 production secrets。CI 测试应使用独立 secrets。

## CI 之外的自动化

### Dependabot / Renovate

```yaml
# .github/dependabot.yml
version: 2
updates:
  - package-ecosystem: npm
    directory: /
    schedule:
      interval: weekly
    open-pull-requests-limit: 5
```

### Build Cop Role

指定某个人负责保持 CI green。当 build 破坏时，Build Cop 的职责是修复或回滚，而不是由造成破坏的变更作者负责。这能避免 broken builds 越积越多，而所有人都以为其他人会修。

### PR Checks

- **Required reviews:** 合并前至少 1 个 approval
- **Required status checks:** CI 必须通过后才能合并
- **Branch protection:** 禁止 force-pushes 到 main
- **Auto-merge:** 如果所有 checks 通过且已批准，自动合并

## CI Optimization

当 pipeline 超过 10 分钟时，按影响大小依次应用这些策略：

```
Slow CI pipeline?
├── Cache dependencies
│   └── Use actions/cache or setup-node cache option for node_modules
├── Run jobs in parallel
│   └── Split lint, typecheck, test, build into separate parallel jobs
├── Only run what changed
│   └── Use path filters to skip unrelated jobs (e.g., skip e2e for docs-only PRs)
├── Use matrix builds
│   └── Shard test suites across multiple runners
├── Optimize the test suite
│   └── Remove slow tests from the critical path, run them on a schedule instead
└── Use larger runners
    └── GitHub-hosted larger runners or self-hosted for CPU-heavy builds
```

**示例：caching and parallelism**
```yaml
jobs:
  lint:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - uses: actions/setup-node@v4
        with: { node-version: '22', cache: 'npm' }
      - run: npm ci
      - run: npm run lint

  typecheck:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - uses: actions/setup-node@v4
        with: { node-version: '22', cache: 'npm' }
      - run: npm ci
      - run: npx tsc --noEmit

  test:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - uses: actions/setup-node@v4
        with: { node-version: '22', cache: 'npm' }
      - run: npm ci
      - run: npm test -- --coverage
```

## Common Rationalizations

| Rationalization | Reality |
|---|---|
| “CI 太慢了” | 优化 pipeline（见下方 CI Optimization），不要跳过它。5 分钟的 pipeline 能避免数小时调试。 |
| “这个变更很小，跳过 CI” | 小变更也会破坏 builds。CI 对小变更通常也很快。 |
| “测试不稳定，重新跑就行” | Flaky tests 会掩盖真实 bug，并浪费所有人的时间。修复不稳定性。 |
| “我们稍后再加 CI” | 没有 CI 的项目会积累 broken states。从第一天就设置好。 |
| “手动测试足够了” | 手动测试无法扩展，也不可重复。能自动化的都自动化。 |

## Red Flags

- 项目没有 CI pipeline
- CI failures 被忽略或静默处理
- 为了让 pipeline 通过而在 CI 中禁用 tests
- Production deploys 没有 staging verification
- 没有 rollback mechanism
- Secrets 存放在代码或 CI config files 中（而不是 secrets manager）
- CI 时间很长且没有优化努力

## Verification

设置或修改 CI 后：

- [ ] 所有 quality gates 都存在（lint、types、tests、build、audit）
- [ ] Pipeline 在每个 PR 和 push to main 上运行
- [ ] Failures 阻止 merge（branch protection 已配置）
- [ ] CI results 反馈回 development loop
- [ ] Secrets 存储在 secrets manager，而不是代码中
- [ ] Deployment 有 rollback mechanism
- [ ] Pipeline 对 test suite 的运行时间少于 10 分钟