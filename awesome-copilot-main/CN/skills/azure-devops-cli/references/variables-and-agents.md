#管道变量，变量组和代理

##目录
-[管道变量]（# Pipeline - Variables）
-[变量组]（# Variable - Groups）
- [Pipeline Folders]（# Pipeline - Folders）
- [Agent Pools]（# Agent - Pools）
- [Agent Queues]（# Agent - Queues）
(代理)(#代理)

---

##管道变量

###列出变量```bash
az pipelines variable list --pipeline-id {pipeline-id}
```
###创建变量```bash
# Non-secret variable
az pipelines variable create \
  --name {var-name} \
  --value {var-value} \
  --pipeline-id {pipeline-id}

# Secret variable
az pipelines variable create \
  --name {var-name} \
  --secret true \
  --pipeline-id {pipeline-id}

# Secret with prompt
az pipelines variable create \
  --name {var-name} \
  --secret true \
  --prompt true \
  --pipeline-id {pipeline-id}
```
###更新变量```bash
az pipelines variable update \
  --name {var-name} \
  --value {new-value} \
  --pipeline-id {pipeline-id}

# Update secret variable
az pipelines variable update \
  --name {var-name} \
  --secret true \
  --value "{new-secret-value}" \
  --pipeline-id {pipeline-id}
```
###删除变量```bash
az pipelines variable delete --name {var-name} --pipeline-id {pipeline-id} --yes
```
##变量组

###列出变量组```bash
az pipelines variable-group list
az pipelines variable-group list --output table
```
###显示变量组```bash
az pipelines variable-group show --id {group-id}
```
###创建变量组```bash
az pipelines variable-group create \
  --name {group-name} \
  --variables key1=value1 key2=value2 \
  --authorize true
```
###更新变量组```bash
az pipelines variable-group update \
  --id {group-id} \
  --name {new-name} \
  --description "Updated description"
```
###删除变量组```bash
az pipelines variable-group delete --id {group-id} --yes
```
###变量组变量```bash
# List variables
az pipelines variable-group variable list --group-id {group-id}

# Create non-secret variable
az pipelines variable-group variable create \
  --group-id {group-id} \
  --name {var-name} \
  --value {var-value}

# Create secret variable (will prompt for value if not provided)
az pipelines variable-group variable create \
  --group-id {group-id} \
  --name {var-name} \
  --secret true

# Create secret with environment variable
export AZURE_DEVOPS_EXT_PIPELINE_VAR_MySecret=secretvalue
az pipelines variable-group variable create \
  --group-id {group-id} \
  --name MySecret \
  --secret true

# Update variable
az pipelines variable-group variable update \
  --group-id {group-id} \
  --name {var-name} \
  --value {new-value} \
  --secret false

# Delete variable
az pipelines variable-group variable delete \
  --group-id {group-id} \
  --name {var-name}
```
##管道文件夹

###列出文件夹```bash
az pipelines folder list
```
###创建文件夹```bash
az pipelines folder create --path 'folder/subfolder' --description "My folder"
```
###删除文件夹```bash
az pipelines folder delete --path 'folder/subfolder'
```
###更新文件夹```bash
az pipelines folder update --path 'old-folder' --new-path 'new-folder'
```
##代理池

###列出代理池```bash
az pipelines pool list
az pipelines pool list --pool-type automation
az pipelines pool list --pool-type deployment
```
###显示座席池```bash
az pipelines pool show --pool-id {pool-id}
```
##代理队列

###列出代理队列```bash
az pipelines queue list
az pipelines queue list --pool-name {pool-name}
```
###显示座席队列```bash
az pipelines queue show --id {queue-id}
```
# #代理

###列出代理池```bash
az pipelines agent list --pool-id {pool-id}
```
显示座席详细信息```bash
az pipelines agent show --agent-id {agent-id} --pool-id {pool-id}
```
