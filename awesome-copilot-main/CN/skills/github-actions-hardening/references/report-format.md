#报告格式

将此结构用于每个工作流强化审查。

# # 1。汇总表（总是在前面）```
GitHub Actions Hardening — <workflow file(s) reviewed>

| Severity   | Count |
| ---------- | ----- |
| 🔴 CRITICAL | 1     |
| 🟠 HIGH     | 2     |
| 🟡 MEDIUM   | 1     |
| 🔵 LOW      | 1     |
| ⚪ INFO     | 0     |
```
如果没有发现任何问题：‘ No issue found. ’已检查：触发器、注入接收器、权限、操作
钉住，秘密处理。”

# # 2。发现结果（按问题类型分组，而不是按文件分组）

对于每个发现，使用一张卡片：```
### 🔴 CRITICAL — Script injection via PR title on a privileged trigger

File: .github/workflows/triage.yml  (line 14)
Trigger: pull_request_target

Offending code:
    - run: echo "New PR: ${{ github.event.pull_request.title }}"

Risk: pull_request_target runs with a read/write token and repository secrets, and any
contributor can open a PR with a title like  "; <attacker-command> #  which is executed as shell.
This allows secret exfiltration and pushes with the workflow token.

Fix:
    - env:
        PR_TITLE: ${{ github.event.pull_request.title }}
      run: echo "New PR: $PR_TITLE"

Confidence: High
```
# # 3。修复块

每个CRITICAL和HIGH的发现都包含一个具体的before/after.保存作者的
缩进、步骤名称和周围结构-仅更改修复问题的内容，并添加
一行注释解释了不明显的变化。

# # 4。关闭请注意

以显式行结束：

>在提交之前检查每个更改。没有任何修改。

##样式规则

引用冒犯的那句话并指出出处。
*用通俗易懂的语言解释风险——攻击者实际上做了什么，而不仅仅是规则名称。
*每发现信心：高/中/低。
*不要夸大严重性：一个分叉`pull_request`（只读令牌，没有秘密）运行不受信任
代码本身并不重要。