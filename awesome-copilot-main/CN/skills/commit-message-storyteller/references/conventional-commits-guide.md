#常规提交-快速参考指南

完整规格：https://www.conventionalcommits.org/en/v1.0.0/---

# #格式```
<type>(<scope>): <description>

[optional body]

[optional footer(s)]
```
---

##类型示例

###`feat`-新功能```
feat(payments): add Apple Pay support for checkout flow

Users in supported regions can now complete purchases using Apple Pay.
This reduces checkout friction and is expected to improve mobile conversion rates.

Closes #88
```
Bug修复```
fix(api): correct off-by-one error in pagination offset

The results page was skipping the last item on every page because the
offset calculation used `>` instead of `>=`. Users were missing records
silently with no error.

Fixes #201
```
###`refactor`-代码重构（没有行为改变）```
refactor(user-service): extract email validation into shared utility

Email validation logic was duplicated across 4 modules. Extracted into
`utils/validators.ts` so future changes only need to happen in one place.
```
###`perf`-性能改进```
perf(dashboard): lazy-load chart components to reduce initial bundle size

The dashboard was importing all chart types upfront, adding ~180KB to the
initial load. Charts are now loaded on demand, cutting initial load time
by ~40% on slow connections.
```
###`docs`-仅限文档```
docs(readme): add local development setup instructions

New contributors were struggling to get the dev environment running.
Added step-by-step instructions covering Node version, env vars, and
the database seed command.
```
###`test`-新增或更新测试```
test(auth): add coverage for concurrent login edge cases

The login flow had no tests for simultaneous requests from the same
session. Added tests that verify only one session token is issued
when multiple requests arrive in the same tick.
```
###`chore`-维护/配置```
chore(deps): upgrade eslint from v8 to v9

v8 reached end-of-life. Migrated config to flat config format required
by v9. No rule changes — this is a tooling-only update.
```
`ci`-CI/CD管道```
ci: add caching for node_modules in GitHub Actions

Cold CI runs were taking 4+ minutes due to repeated installs.
Adding cache restore on lockfile hash reduces this to ~90 seconds.
```
###`revert`-恢复提交```
revert: feat(notifications): add push notification opt-in

Reverts commit a3f92bc.

The push notification feature caused a crash on Android 12 devices.
Rolling back until the root cause is identified.
```
---

##范围指南`scope`是可选的，但强烈推荐使用。应该是：
-识别代码库区域的短名词：`auth`，`api`,`dashboard`,`payments`-整个项目的一致性（不要混合`user`和`users`）
-如果更改确实是全局的，则省略

---

##打破改变

在页脚添加`BREAKING CHANGE:`（或者在类型后面使用`!`）：```
feat(api)!: remove v1 endpoints

All v1 REST endpoints have been removed following the 6-month deprecation
notice. Consumers must migrate to v2 before upgrading.

BREAKING CHANGE: /api/v1/* routes no longer exist. See migration guide at docs/v2-migration.md
```
---

##身体写作技巧

在写正文前问问自己：
- **在此更改之前有什么损坏/丢失？**
- **为什么选择这种方法而不是其他方法？**
**用户和开发者在此之后会有什么不同？**

避免:
-重申diff已经显示的内容（“更改了变量名称”）
-语言模糊（“各种改进”，“各种修复”）
将来时（“this will fix…”）-用present/past时态书写

---

提交消息反模式

|❌坏|✅好||--------|----------|
|`fix bug`|`fix(cart): prevent duplicate items on rapid add-to-cart clicks`|
|`updates`|`feat(profile): allow users to update display name`|
不要提交WIP，把它藏起来
|`misc changes`|分割成独立的、有意义的提交|
描述改变了什么，而不是谁改变了它