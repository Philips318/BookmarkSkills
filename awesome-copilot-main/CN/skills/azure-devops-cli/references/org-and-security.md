#组织、安全和管理

##目录
(项目)(#项目)
-[扩展管理]（# Extension - Management）
-[服务端点]（# Service -endpoint）
——(团队)(#团队)
-[用户](#用户)
-[安全组]（# Security - Groups）
-[安全权限]（# Security - Permissions）
- [wiki] (# wiki)
-[政府](#管理)
- [DevOps扩展]（# DevOps - Extensions）

---

# #项目

###列出项目```bash
az devops project list --organization https://dev.azure.com/{org}
az devops project list --top 10 --output table
```
###创建项目```bash
az devops project create \
  --name myNewProject \
  --organization https://dev.azure.com/{org} \
  --description "My new DevOps project" \
  --source-control git \
  --visibility private
```
显示项目详细信息```bash
az devops project show --project {project-name} --org https://dev.azure.com/{org}
```
###删除项目```bash
az devops project delete --id {project-id} --org https://dev.azure.com/{org} --yes
```
##扩展管理

###列表扩展```bash
# List available extensions
az extension list-available --output table

# List installed extensions
az extension list --output table
```
###管理Azure DevOps扩展```bash
# Install Azure DevOps extension
az extension add --name azure-devops

# Update Azure DevOps extension
az extension update --name azure-devops

# Remove extension
az extension remove --name azure-devops

# Install from local path
az extension add --source ~/extensions/azure-devops.whl
```
服务端点

列出服务端点```bash
az devops service-endpoint list --project {project}
az devops service-endpoint list --project {project} --output table
```
显示服务端点```bash
az devops service-endpoint show --id {endpoint-id} --project {project}
```
创建服务端点```bash
# Using configuration file
az devops service-endpoint create --service-endpoint-configuration endpoint.json --project {project}
```
删除服务端点```bash
az devops service-endpoint delete --id {endpoint-id} --project {project} --yes
```
# #团队

列出团队```bash
az devops team list --project {project}
```
展示团队```bash
az devops team show --team {team-name} --project {project}
```
###创建团队```bash
az devops team create \
  --name {team-name} \
  --description "Team description" \
  --project {project}
```
更新团队```bash
az devops team update \
  --team {team-name} \
  --project {project} \
  --name "{new-team-name}" \
  --description "Updated description"
```
删除团队```bash
az devops team delete --team {team-name} --project {project} --yes
```
展示团队成员```bash
az devops team list-member --team {team-name} --project {project}
```
# #用户

###列出用户```bash
az devops user list --org https://dev.azure.com/{org}
az devops user list --top 10 --output table
```
###显示用户```bash
az devops user show --user {user-id-or-email} --org https://dev.azure.com/{org}
```
###添加用户```bash
az devops user add \
  --email user@example.com \
  --license-type express \
  --org https://dev.azure.com/{org}
```
###更新用户```bash
az devops user update \
  --user {user-id-or-email} \
  --license-type advanced \
  --org https://dev.azure.com/{org}
```
###删除用户```bash
az devops user remove --user {user-id-or-email} --org https://dev.azure.com/{org} --yes
```
##安全组

###列表组```bash
# List all groups in project
az devops security group list --project {project}

# List all groups in organization
az devops security group list --scope organization

# List with filtering
az devops security group list --project {project} --subject-types vstsgroup
```
显示组的详细信息```bash
az devops security group show --group-id {group-id}
```
###创建组```bash
az devops security group create \
  --name {group-name} \
  --description "Group description" \
  --project {project}
```
###更新组```bash
az devops security group update \
  --group-id {group-id} \
  --name "{new-group-name}" \
  --description "Updated description"
```
###删除组```bash
az devops security group delete --group-id {group-id} --yes
```
###组成员```bash
# List memberships
az devops security group membership list --id {group-id}

# Add member
az devops security group membership add \
  --group-id {group-id} \
  --member-id {member-id}

# Remove member
az devops security group membership remove \
  --group-id {group-id} \
  --member-id {member-id} --yes
```
##安全权限

###列出命名空间```bash
az devops security permission namespace list
```
###显示命名空间详细信息```bash
# Show permissions available in a namespace
az devops security permission namespace show --namespace "GitRepositories"
```
###列出权限```bash
# List permissions for user/group and namespace
az devops security permission list \
  --id {user-or-group-id} \
  --namespace "GitRepositories" \
  --project {project}

# List for specific token (repository)
az devops security permission list \
  --id {user-or-group-id} \
  --namespace "GitRepositories" \
  --project {project} \
  --token "repoV2/{project}/{repository-id}"
```
###显示权限```bash
az devops security permission show \
  --id {user-or-group-id} \
  --namespace "GitRepositories" \
  --project {project} \
  --token "repoV2/{project}/{repository-id}"
```
###更新权限```bash
# Grant permission
az devops security permission update \
  --id {user-or-group-id} \
  --namespace "GitRepositories" \
  --project {project} \
  --token "repoV2/{project}/{repository-id}" \
  --permission-mask "Pull,Contribute"

# Deny permission
az devops security permission update \
  --id {user-or-group-id} \
  --namespace "GitRepositories" \
  --project {project} \
  --token "repoV2/{project}/{repository-id}" \
  --permission-mask 0
```
###重置权限```bash
# Reset specific permission bits
az devops security permission reset \
  --id {user-or-group-id} \
  --namespace "GitRepositories" \
  --project {project} \
  --token "repoV2/{project}/{repository-id}" \
  --permission-mask "Pull,Contribute"

# Reset all permissions
az devops security permission reset-all \
  --id {user-or-group-id} \
  --namespace "GitRepositories" \
  --project {project} \
  --token "repoV2/{project}/{repository-id}" --yes
```
# #维基

列出wiki```bash
# List all wikis in project
az devops wiki list --project {project}

# List all wikis in organization
az devops wiki list
```
###显示Wiki```bash
az devops wiki show --wiki {wiki-name} --project {project}
az devops wiki show --wiki {wiki-name} --project {project} --open
```
###创建Wiki```bash
# Create project wiki
az devops wiki create \
  --name {wiki-name} \
  --project {project} \
  --type projectWiki

# Create code wiki from repository
az devops wiki create \
  --name {wiki-name} \
  --project {project} \
  --type codeWiki \
  --repository {repo-name} \
  --mapped-path /wiki
```
###删除Wiki```bash
az devops wiki delete --wiki {wiki-id} --project {project} --yes
```
###维基页面```bash
# List pages
az devops wiki page list --wiki {wiki-name} --project {project}

# Show page
az devops wiki page show \
  --wiki {wiki-name} \
  --path "/page-name" \
  --project {project}

# Create page
az devops wiki page create \
  --wiki {wiki-name} \
  --path "/new-page" \
  --content "# New Page\n\nPage content here..." \
  --project {project}

# Update page
az devops wiki page update \
  --wiki {wiki-name} \
  --path "/existing-page" \
  --content "# Updated Page\n\nNew content..." \
  --project {project}

# Delete page
az devops wiki page delete \
  --wiki {wiki-name} \
  --path "/old-page" \
  --project {project} --yes
```
# #管理

###横幅管理```bash
# List banners
az devops admin banner list

# Show banner details
az devops admin banner show --id {banner-id}

# Add new banner
az devops admin banner add \
  --message "System maintenance scheduled" \
  --level info  # info, warning, error

# Update banner
az devops admin banner update \
  --id {banner-id} \
  --message "Updated message" \
  --level warning \
  --expiration-date "2025-12-31T23:59:59Z"

# Remove banner
az devops admin banner remove --id {banner-id}
```
## DevOps扩展

管理安装在Azure DevOps组织中的扩展（不同于CLI扩展）。```bash
# List installed extensions
az devops extension list --org https://dev.azure.com/{org}

# Search marketplace extensions
az devops extension search --search-query "docker"

# Show extension details
az devops extension show --ext-id {extension-id} --org https://dev.azure.com/{org}

# Install extension
az devops extension install \
  --ext-id {extension-id} \
  --org https://dev.azure.com/{org} \
  --publisher {publisher-id}

# Enable extension
az devops extension enable \
  --ext-id {extension-id} \
  --org https://dev.azure.com/{org}

# Disable extension
az devops extension disable \
  --ext-id {extension-id} \
  --org https://dev.azure.com/{org}

# Uninstall extension
az devops extension uninstall \
  --ext-id {extension-id} \
  --org https://dev.azure.com/{org} --yes
```
