#管道，构建和发布

##目录
——(管道)(#管道)
- [Pipeline Runs]（# Pipeline - Runs）
——(构建)(#构建)
-[构建定义]（# Build - Definitions）
——(释放)(#版本)
-[版本定义]（# Release - Definitions）
-[通用包（工件）]（# Universal - Packages - Artifacts）

---

# #管道

###列出管道```bash
az pipelines list --output table
az pipelines list --query "[?name=='myPipeline']"
az pipelines list --folder-path 'folder/subfolder'
```
创建管道```bash
# From local repository context (auto-detects settings)
az pipelines create --name 'ContosoBuild' --description 'Pipeline for contoso project'

# With specific branch and YAML path
az pipelines create \
  --name {pipeline-name} \
  --repository {repo} \
  --branch main \
  --yaml-path azure-pipelines.yml \
  --description "My CI/CD pipeline"

# For GitHub repository
az pipelines create \
  --name 'GitHubPipeline' \
  --repository https://github.com/Org/Repo \
  --branch main \
  --repository-type github

# Skip first run
az pipelines create --name 'MyPipeline' --skip-run true
```
显示管道```bash
az pipelines show --id {pipeline-id}
az pipelines show --name {pipeline-name}
```
更新管道```bash
az pipelines update --id {pipeline-id} --name "New name" --description "Updated description"
```
###删除管道```bash
az pipelines delete --id {pipeline-id} --yes
```
###运行Pipeline```bash
# Run by name
az pipelines run --name {pipeline-name} --branch main

# Run by ID
az pipelines run --id {pipeline-id} --branch refs/heads/main

# With parameters
az pipelines run --name {pipeline-name} --parameters version=1.0.0 environment=prod

# With variables
az pipelines run --name {pipeline-name} --variables buildId=123 configuration=release

# Open results in browser
az pipelines run --name {pipeline-name} --open
```
##管道运行

### List运行```bash
az pipelines runs list --pipeline {pipeline-id}
az pipelines runs list --name {pipeline-name} --top 10
az pipelines runs list --branch main --status completed
```
###显示运行详情```bash
az pipelines runs show --run-id {run-id}
az pipelines runs show --run-id {run-id} --open
```
管道构件```bash
# List artifacts for a run
az pipelines runs artifact list --run-id {run-id}

# Download artifact
az pipelines runs artifact download \
  --artifact-name '{artifact-name}' \
  --path {local-path} \
  --run-id {run-id}

# Upload artifact
az pipelines runs artifact upload \
  --artifact-name '{artifact-name}' \
  --path {local-path} \
  --run-id {run-id}
```
###管道运行标签```bash
# Add tag to run
az pipelines runs tag add --run-id {run-id} --tags production v1.0

# List run tags
az pipelines runs tag list --run-id {run-id} --output table
```
# #构建

###列表构建```bash
az pipelines build list
az pipelines build list --definition {build-definition-id}
az pipelines build list --status completed --result succeeded
```
队列构建```bash
az pipelines build queue --definition {build-definition-id} --branch main
az pipelines build queue --definition {build-definition-id} --parameters version=1.0.0
```
显示构建细节```bash
az pipelines build show --id {build-id}
```
###取消构建```bash
az pipelines build cancel --id {build-id}
```
###构建标签```bash
# Add tag to build
az pipelines build tag add --build-id {build-id} --tags prod release

# Delete tag from build
az pipelines build tag delete --build-id {build-id} --tag prod
```
##构建定义

###列出构建定义```bash
az pipelines build definition list
az pipelines build definition list --name {definition-name}
```
显示构建定义```bash
az pipelines build definition show --id {definition-id}
```
# #版本

###列表发布```bash
az pipelines release list
az pipelines release list --definition {release-definition-id}
```
###创建发布```bash
az pipelines release create --definition {release-definition-id}
az pipelines release create --definition {release-definition-id} --description "Release v1.0"
```
###显示释放```bash
az pipelines release show --id {release-id}
```
##发布定义

###列表发布定义```bash
az pipelines release definition list
```
显示版本定义```bash
az pipelines release definition show --id {definition-id}
```
通用包（工件）

###发布包```bash
az artifacts universal publish \
  --feed {feed-name} \
  --name {package-name} \
  --version {version} \
  --path {package-path} \
  --project {project}
```
下载包```bash
az artifacts universal download \
  --feed {feed-name} \
  --name {package-name} \
  --version {version} \
  --path {download-path} \
  --project {project}
```
