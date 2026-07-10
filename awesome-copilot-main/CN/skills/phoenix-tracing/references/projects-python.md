# Phoenix Tracing: Projects （Python）

**使用项目（Phoenix的顶级分组）按应用程序组织跟踪

# #概述

项目对单个应用程序或实验的跟踪进行分组。

**用于：**环境（dev/staging/prod），A/B测试，版本控制

# #设置

环境变量（推荐）```bash
export PHOENIX_PROJECT_NAME="my-app-prod"
```

```python
import os
os.environ["PHOENIX_PROJECT_NAME"] = "my-app-prod"
from phoenix.otel import register
register()  # Uses "my-app-prod"
```
# # #代码```python
from phoenix.otel import register
register(project_name="my-app-prod")
```
##用例

* *环境:* *```python
# Dev, staging, prod
register(project_name="my-app-dev")
register(project_name="my-app-staging")
register(project_name="my-app-prod")
```
* *A/B测试:* *```python
# Compare models
register(project_name="chatbot-gpt4")
register(project_name="chatbot-claude")
```
* *版本:* *```python
# Track versions
register(project_name="my-app-v1")
register(project_name="my-app-v2")
```
切换项目（仅限Python笔记本）```python
from openinference.instrumentation import dangerously_using_project
from phoenix.otel import register

register(project_name="my-app")

# Switch temporarily for evals
with dangerously_using_project("my-eval-project"):
    run_evaluations()
```
**⚠️仅用于notebooks/scripts，不用于生产