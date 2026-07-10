---
name: shipping-and-launch
description: 准备生产发布。用于准备部署到生产环境时。用于需要预发布清单、设置监控、规划分阶段 rollout，或需要回滚策略时。
---

# 发布与上线

## 概述

带着信心发布。目标不只是部署，而是安全部署：监控已经就位、回滚计划准备好，并且清楚知道成功是什么样。每次发布都应该可回滚、可观测、可增量推进。

## 何时使用

- 首次将功能部署到生产环境
- 向用户发布重大变更
- 迁移数据或基础设施
- 开放 beta 或 early access program
- 任何有风险的部署（也就是所有部署）

## 预发布清单

### 代码质量

- [ ] 所有测试通过（unit、integration、e2e）
- [ ] 构建成功且没有 warnings
- [ ] Lint 和类型检查通过
- [ ] 代码已审查并批准
- [ ] 没有应在发布前解决的 TODO comments
- [ ] 生产代码中没有 `console.log` 调试语句
- [ ] 错误处理覆盖预期失败模式

### 安全

- [ ] 代码或版本控制中没有 secrets
- [ ] `npm audit` 没有 critical 或 high vulnerabilities
- [ ] 所有面向用户 endpoints 都有输入验证
- [ ] Authentication 和 authorization checks 已就位
- [ ] Security headers 已配置（CSP、HSTS 等）
- [ ] Authentication endpoints 上有 rate limiting
- [ ] CORS 配置为具体 origins（不是 wildcard）

### 性能

- [ ] Core Web Vitals 在 “Good” 阈值内
- [ ] 关键路径中没有 N+1 queries
- [ ] 图片已优化（压缩、responsive sizes、lazy loading）
- [ ] Bundle size 在预算内
- [ ] 数据库 queries 有适当 indexes
- [ ] 静态资源和重复 queries 已配置缓存

### 可访问性

- [ ] 所有交互元素都支持键盘导航
- [ ] 屏幕阅读器能传达页面内容和结构
- [ ] 颜色对比度满足 WCAG 2.1 AA（文本 4.5:1）
- [ ] Modals 和动态内容的焦点管理正确
- [ ] 错误消息具有描述性，并与表单字段关联
- [ ] axe-core 或 Lighthouse 中没有可访问性 warnings

### 基础设施

- [ ] 环境变量已在生产环境设置
- [ ] 数据库迁移已应用（或已准备好应用）
- [ ] DNS 和 SSL 已配置
- [ ] CDN 已为静态资源配置
- [ ] Logging 和 error reporting 已配置
- [ ] Health check endpoint 存在并响应

### 文档

- [ ] README 已更新任何新的 setup requirements
- [ ] API 文档是最新的
- [ ] 任何架构决策都有 ADRs
- [ ] Changelog 已更新
- [ ] 面向用户文档已更新（如适用）

## 功能开关策略

在 feature flags 后发布，以解耦部署和发布：

```typescript
// Feature flag check
const flags = await getFeatureFlags(userId);

if (flags.taskSharing) {
  // New feature: task sharing
  return <TaskSharingPanel task={task} />;
}

// Default: existing behavior
return null;
```

**功能开关生命周期：**

```
1. DEPLOY with flag OFF     → Code is in production but inactive
2. ENABLE for team/beta     → Internal testing in production environment
3. GRADUAL ROLLOUT          → 5% → 25% → 50% → 100% of users
4. MONITOR at each stage    → Watch error rates, performance, user feedback
5. CLEAN UP                 → Remove flag and dead code path after full rollout
```

**规则：**
- 每个 feature flag 都有 owner 和 expiration date
- 完全 rollout 后 2 周内清理 flags
- 不要嵌套 feature flags（会制造指数级组合）
- 在 CI 中测试 flag 的两种状态（on 和 off）

## 分阶段 Rollout

### Rollout 顺序

```
1. DEPLOY to staging
   └── Full test suite in staging environment
   └── Manual smoke test of critical flows

2. DEPLOY to production (feature flag OFF)
   └── Verify deployment succeeded (health check)
   └── Check error monitoring (no new errors)

3. ENABLE for team (flag ON for internal users)
   └── Team uses the feature in production
   └── 24-hour monitoring window

4. CANARY rollout (flag ON for 5% of users)
   └── Monitor error rates, latency, user behavior
   └── Compare metrics: canary vs. baseline
   └── 24-48 hour monitoring window
   └── Advance only if all thresholds pass (see table below)

5. GRADUAL increase (25% -> 50% -> 100%)
   └── Same monitoring at each step
   └── Ability to roll back to previous percentage at any point

6. FULL rollout (flag ON for all users)
   └── Monitor for 1 week
   └── Clean up feature flag
```

### Rollout 决策阈值

使用这些阈值决定每个阶段是推进、暂停调查还是回滚：

| 指标 | 推进（green） | 暂停并调查（yellow） | 回滚（red） |
|--------|-----------------|-------------------------------|-----------------|
| Error rate | Within 10% of baseline | 10-100% above baseline | >2x baseline |
| P95 latency | Within 20% of baseline | 20-50% above baseline | >50% above baseline |
| Client JS errors | No new error types | New errors at <0.1% of sessions | New errors at >0.1% of sessions |
| Business metrics | Neutral or positive | Decline <5% (may be noise) | Decline >5% |

### 何时回滚

在以下情况立即回滚：
- Error rate 增加到 baseline 的 2 倍以上
- P95 latency 增加超过 50%
- 用户报告问题激增
- 检测到数据完整性问题
- 发现安全漏洞

## 监控与可观测性

### 监控什么

```
Application metrics:
├── Error rate (total and by endpoint)
├── Response time (p50, p95, p99)
├── Request volume
├── Active users
└── Key business metrics (conversion, engagement)

Infrastructure metrics:
├── CPU and memory utilization
├── Database connection pool usage
├── Disk space
├── Network latency
└── Queue depth (if applicable)

Client metrics:
├── Core Web Vitals (LCP, INP, CLS)
├── JavaScript errors
├── API error rates from client perspective
└── Page load time
```

### 错误上报

```typescript
// Set up error boundary with reporting
class ErrorBoundary extends React.Component {
  componentDidCatch(error: Error, info: React.ErrorInfo) {
    // Report to error tracking service
    reportError(error, {
      componentStack: info.componentStack,
      userId: getCurrentUser()?.id,
      page: window.location.pathname,
    });
  }

  render() {
    if (this.state.hasError) {
      return <ErrorFallback onRetry={() => this.setState({ hasError: false })} />;
    }
    return this.props.children;
  }
}

// Server-side error reporting
app.use((err: Error, req: Request, res: Response, next: NextFunction) => {
  reportError(err, {
    method: req.method,
    url: req.url,
    userId: req.user?.id,
  });

  // Don't expose internals to users
  res.status(500).json({
    error: { code: 'INTERNAL_ERROR', message: 'Something went wrong' },
  });
});
```

### 发布后验证

发布后的第一个小时：

```
1. Check health endpoint returns 200
2. Check error monitoring dashboard (no new error types)
3. Check latency dashboard (no regression)
4. Test the critical user flow manually
5. Verify logs are flowing and readable
6. Confirm rollback mechanism works (dry run if possible)
```

## 回滚策略

每次部署前都需要回滚计划：

```markdown
## Rollback Plan for [Feature/Release]

### Trigger Conditions
- Error rate > 2x baseline
- P95 latency > [X]ms
- User reports of [specific issue]

### Rollback Steps
1. Disable feature flag (if applicable)
   OR
1. Deploy previous version: `git revert <commit> && git push`
2. Verify rollback: health check, error monitoring
3. Communicate: notify team of rollback

### Database Considerations
- Migration [X] has a rollback: `npx prisma migrate rollback`
- Data inserted by new feature: [preserved / cleaned up]

### Time to Rollback
- Feature flag: < 1 minute
- Redeploy previous version: < 5 minutes
- Database rollback: < 15 minutes
```
## 另请参阅

- 每个变更在这个清单前都必须满足项目级 Definition of Done，见 `references/definition-of-done.md`
- 安全预发布检查见 `references/security-checklist.md`
- 性能预发布清单见 `references/performance-checklist.md`
- 发布前可访问性验证见 `references/accessibility-checklist.md`

## 常见合理化借口

| 合理化借口 | 现实 |
|---|---|
| “它在 staging 工作，production 也会工作” | Production 有不同数据、流量模式和边界情况。部署后要监控。 |
| “这个不需要 feature flags” | 每个功能都能从 kill switch 中受益。即使“简单”变更也可能破坏东西。 |
| “监控是开销” | 没有监控意味着你从用户投诉而不是 dashboard 中发现问题。 |
| “我们之后再加监控” | 发布前添加。你看不见的东西就无法调试。 |
| “回滚就是承认失败” | 回滚是负责任的工程。发布破损功能才是失败。 |

## 危险信号

- 没有回滚计划就部署
- 生产中没有监控或错误上报
- Big-bang releases（一次性全部发布，无 staging）
- Feature flags 没有 expiration 或 owner
- 发布后的第一个小时没人监控
- 生产环境配置靠记忆，而不是代码
- “It's Friday afternoon, let's ship it”

## 验证

部署前：

- [ ] 预发布清单已完成（所有 sections green）
- [ ] Feature flag 已配置（如适用）
- [ ] 回滚计划已记录
- [ ] 监控 dashboards 已设置
- [ ] 团队已收到部署通知

部署后：

- [ ] Health check 返回 200
- [ ] Error rate 正常
- [ ] Latency 正常
- [ ] 关键用户流程工作正常
- [ ] Logs 正在流入
- [ ] 回滚已测试或确认随时可用
