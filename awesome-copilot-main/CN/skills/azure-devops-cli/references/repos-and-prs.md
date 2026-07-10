# repository & Pull Requests

##目录
——(库)(#库)
- [Repository Import]（# Repository - Import）
- [Pull Requests]（# Pull - Requests）
- [Git参考]（# Git - References）
-[存储库策略]（# Repository - Policies）

---

# #库

列表存储库```bash
az repos list --org https://dev.azure.com/{org} --project {project}
az repos list --output table
```
显示存储库详细信息```bash
az repos show --repository {repo-name} --project {project}
```
创建存储库```bash
az repos create --name {repo-name} --project {project}
```
删除存储库```bash
az repos delete --id {repo-id} --project {project} --yes
```
更新存储库```bash
az repos update --id {repo-id} --name {new-name} --project {project}
```
##导入存储库

导入Git仓库```bash
# Import from public Git repository
az repos import create \
  --git-source-url https://github.com/user/repo \
  --repository {repo-name}

# Import with authentication
az repos import create \
  --git-source-url https://github.com/user/private-repo \
  --repository {repo-name} \
  --user {username} \
  --password {password-or-pat}
```
## Pull Requests

创建拉取请求```bash
# Basic PR creation
az repos pr create \
  --repository {repo} \
  --source-branch {source-branch} \
  --target-branch {target-branch} \
  --title "PR Title" \
  --description "PR description" \
  --open

# PR with work items
az repos pr create \
  --repository {repo} \
  --source-branch {source-branch} \
  --work-items 63 64

# Draft PR with reviewers
az repos pr create \
  --repository {repo} \
  --source-branch feature/new-feature \
  --target-branch main \
  --title "Feature: New functionality" \
  --draft true \
  --reviewers user1@example.com user2@example.com \
  --required-reviewers lead@example.com \
  --labels "enhancement" "backlog"
```
###列出Pull Requests```bash
# All PRs
az repos pr list --repository {repo}

# Filter by status
az repos pr list --repository {repo} --status active

# Filter by creator
az repos pr list --repository {repo} --creator {email}

# Output as table
az repos pr list --repository {repo} --output table
```
显示PR详细信息```bash
az repos pr show --id {pr-id}
az repos pr show --id {pr-id} --open  # Open in browser
```
更新PR （Complete/Abandon/Draft）```bash
# Complete PR
az repos pr update --id {pr-id} --status completed

# Abandon PR
az repos pr update --id {pr-id} --status abandoned

# Set to draft
az repos pr update --id {pr-id} --draft true

# Publish draft PR
az repos pr update --id {pr-id} --draft false

# Auto-complete when policies pass
az repos pr update --id {pr-id} --auto-complete true

# Set title and description
az repos pr update --id {pr-id} --title "New title" --description "New description"
```
本地签出PR```bash
# Checkout PR branch
az repos pr checkout --id {pr-id}

# Checkout with specific remote
az repos pr checkout --id {pr-id} --remote-name upstream
```
###公关投票```bash
az repos pr set-vote --id {pr-id} --vote approve
az repos pr set-vote --id {pr-id} --vote approve-with-suggestions
az repos pr set-vote --id {pr-id} --vote reject
az repos pr set-vote --id {pr-id} --vote wait-for-author
az repos pr set-vote --id {pr-id} --vote reset
```
PR评论者```bash
# Add reviewers
az repos pr reviewer add --id {pr-id} --reviewers user1@example.com user2@example.com

# List reviewers
az repos pr reviewer list --id {pr-id}

# Remove reviewers
az repos pr reviewer remove --id {pr-id} --reviewers user1@example.com
```
PR工作项```bash
# Add work items to PR
az repos pr work-item add --id {pr-id} --work-items {id1} {id2}

# List PR work items
az repos pr work-item list --id {pr-id}

# Remove work items from PR
az repos pr work-item remove --id {pr-id} --work-items {id1}
```
### PR政策```bash
# List policies for a PR
az repos pr policy list --id {pr-id}

# Queue policy evaluation for a PR
az repos pr policy queue --id {pr-id} --evaluation-id {evaluation-id}
```
## Git参考

列出引用（分支）```bash
az repos ref list --repository {repo}
az repos ref list --repository {repo} --query "[?name=='refs/heads/main']"
```
创建引用（分支）```bash
az repos ref create --name refs/heads/new-branch --object-type commit --object {commit-sha}
```
删除引用（分支）```bash
az repos ref delete --name refs/heads/old-branch --repository {repo} --project {project}
```
Lock/Unlock分支```bash
az repos ref lock --name refs/heads/main --repository {repo} --project {project}
az repos ref unlock --name refs/heads/main --repository {repo} --project {project}
```
存储库策略

###列出所有策略```bash
az repos policy list --repository {repo-id} --branch main
```
###Create/Update/Delete策略```bash
# Create from config file
az repos policy create --config policy.json

# Update
az repos policy update --id {policy-id} --config updated-policy.json

# Delete
az repos policy delete --id {policy-id} --yes
```
审批者计数策略```bash
az repos policy approver-count create \
  --blocking true \
  --enabled true \
  --branch main \
  --repository-id {repo-id} \
  --minimum-approver-count 2 \
  --creator-vote-counts true
```
###构建策略```bash
az repos policy build create \
  --blocking true \
  --enabled true \
  --branch main \
  --repository-id {repo-id} \
  --build-definition-id {definition-id} \
  --queue-on-source-update-only true \
  --valid-duration 720
```
工作项链接策略```bash
az repos policy work-item-linking create \
  --blocking true \
  --branch main \
  --enabled true \
  --repository-id {repo-id}
```
Required Reviewer Policy```bash
az repos policy required-reviewer create \
  --blocking true \
  --enabled true \
  --branch main \
  --repository-id {repo-id} \
  --required-reviewers user@example.com
```
合并策略策略```bash
az repos policy merge-strategy create \
  --blocking true \
  --enabled true \
  --branch main \
  --repository-id {repo-id} \
  --allow-squash true \
  --allow-rebase true \
  --allow-no-fast-forward true
```
案件执行政策```bash
az repos policy case-enforcement create \
  --blocking true \
  --enabled true \
  --branch main \
  --repository-id {repo-id}
```
注释需要的策略```bash
az repos policy comment-required create \
  --blocking true \
  --enabled true \
  --branch main \
  --repository-id {repo-id}
```
文件大小策略```bash
az repos policy file-size create \
  --blocking true \
  --enabled true \
  --branch main \
  --repository-id {repo-id} \
  --maximum-file-size 10485760  # 10MB in bytes
```
