#工作项，区域路径和迭代

##目录
-[工作项（板）]（# Work - Items - Boards）
-[区域路径]（# Area - Paths）
——(迭代)(#迭代)

---

工作项（板）

查询工作项```bash
# WIQL query
az boards query \
  --wiql "SELECT [System.Id], [System.Title], [System.State] FROM WorkItems WHERE [System.AssignedTo] = @Me AND [System.State] = 'Active'"

# Query with output format
az boards query --wiql "SELECT * FROM WorkItems" --output table
```
显示工作项```bash
az boards work-item show --id {work-item-id}
az boards work-item show --id {work-item-id} --open
```
创建工作项```bash
# Basic work item
az boards work-item create \
  --title "Fix login bug" \
  --type Bug \
  --assigned-to user@example.com \
  --description "Users cannot login with SSO"

# With area and iteration
az boards work-item create \
  --title "New feature" \
  --type "User Story" \
  --area "Project\\Area1" \
  --iteration "Project\\Sprint 1"

# With custom fields
az boards work-item create \
  --title "Task" \
  --type Task \
  --fields "Priority=1" "Severity=2"

# With discussion comment
az boards work-item create \
  --title "Issue" \
  --type Bug \
  --discussion "Initial investigation completed"

# For a long --discussion body on Windows, see references/long-comments-on-windows.md.
# Short version: use azps.ps1 in PowerShell, or fall back to 'az devops invoke'
# with --in-file when no native --file-path flag is available.

# Open in browser after creation
az boards work-item create --title "Bug" --type Bug --open
```
更新工作项```bash
# Update state, title, and assignee
az boards work-item update \
  --id {work-item-id} \
  --state "Active" \
  --title "Updated title" \
  --assigned-to user@example.com

# Move to different area
az boards work-item update \
  --id {work-item-id} \
  --area "{ProjectName}\\{Team}\\{Area}"

# Change iteration
az boards work-item update \
  --id {work-item-id} \
  --iteration "{ProjectName}\\Sprint 5"

# Add comment/discussion
az boards work-item update \
  --id {work-item-id} \
  --discussion "Work in progress"

# Long comment on Windows: read the body into a PowerShell variable and call
# azps.ps1 instead of az.cmd, or fall back to 'az devops invoke' with --in-file.
# Full guidance in references/long-comments-on-windows.md.
#
# PowerShell example:
#   $body = Get-Content -Raw .\comment.md
#   azps.ps1 boards work-item update --id 1234 --discussion $body

# Update with custom fields
az boards work-item update \
  --id {work-item-id} \
  --fields "Priority=1" "StoryPoints=5"
```
删除工作项```bash
# Soft delete (can be restored)
az boards work-item delete --id {work-item-id} --yes

# Permanent delete
az boards work-item delete --id {work-item-id} --destroy --yes
```
工作项关系```bash
# List relations
az boards work-item relation list --id {work-item-id}

# List supported relation types
az boards work-item relation list-type

# Add relation
az boards work-item relation add --id {work-item-id} --relation-type parent --target-id {parent-id}

# Remove relation
az boards work-item relation remove --id {work-item-id} --relation-id {relation-id}
```
##区域路径

列出项目的区域```bash
az boards area project list --project {project}
az boards area project show --path "Project\\Area1" --project {project}
```
###创建区域```bash
az boards area project create --path "Project\\NewArea" --project {project}
```
更新区域```bash
az boards area project update \
  --path "Project\\OldArea" \
  --new-path "Project\\UpdatedArea" \
  --project {project}
```
###删除区域```bash
az boards area project delete --path "Project\\AreaToDelete" --project {project} --yes
```
区域团队管理```bash
# List areas for team
az boards area team list --team {team-name} --project {project}

# Add area to team
az boards area team add \
  --team {team-name} \
  --path "Project\\NewArea" \
  --project {project}

# Remove area from team
az boards area team remove \
  --team {team-name} \
  --path "Project\\AreaToRemove" \
  --project {project}

# Update team area
az boards area team update \
  --team {team-name} \
  --path "Project\\Area" \
  --project {project} \
  --include-sub-areas true
```
# #迭代

列出项目的迭代```bash
az boards iteration project list --project {project}
az boards iteration project show --path "Project\\Sprint 1" --project {project}
```
创建迭代```bash
az boards iteration project create --path "Project\\Sprint 1" --project {project}
```
更新迭代```bash
az boards iteration project update \
  --path "Project\\OldSprint" \
  --new-path "Project\\NewSprint" \
  --project {project}
```
删除迭代```bash
az boards iteration project delete --path "Project\\OldSprint" --project {project} --yes
```
团队迭代```bash
# List iterations for team
az boards iteration team list --team {team-name} --project {project}

# Add iteration to team
az boards iteration team add \
  --team {team-name} \
  --path "Project\\Sprint 1" \
  --project {project}

# Remove iteration from team
az boards iteration team remove \
  --team {team-name} \
  --path "Project\\Sprint 1" \
  --project {project}

# List work items in iteration
az boards iteration team list-work-items \
  --team {team-name} \
  --path "Project\\Sprint 1" \
  --project {project}
```
默认迭代和积压迭代```bash
# Set default iteration for team
az boards iteration team set-default-iteration \
  --team {team-name} \
  --path "Project\\Sprint 1" \
  --project {project}

# Show default iteration
az boards iteration team show-default-iteration \
  --team {team-name} \
  --project {project}

# Set backlog iteration for team
az boards iteration team set-backlog-iteration \
  --team {team-name} \
  --path "Project\\Sprint 1" \
  --project {project}

# Show backlog iteration
az boards iteration team show-backlog-iteration \
  --team {team-name} \
  --project {project}

# Show current iteration
az boards iteration team show --team {team-name} --project {project} --timeframe current
```
