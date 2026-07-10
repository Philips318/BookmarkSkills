# Phoenix Tracing: Projects （TypeScript）

**使用项目（Phoenix的顶级分组）按应用程序组织跟踪

# #概述

项目对单个应用程序或实验的跟踪进行分组。

**用于：**环境（dev/staging/prod），A/B测试，版本控制

# #设置

环境变量（推荐）```bash
export PHOENIX_PROJECT_NAME="my-app-prod"
```

```typescript
process.env.PHOENIX_PROJECT_NAME = "my-app-prod";
import { register } from "@arizeai/phoenix-otel";
register();  // Uses "my-app-prod"
```
# # #代码```typescript
import { register } from "@arizeai/phoenix-otel";
register({ projectName: "my-app-prod" });
```
##用例

* *环境:* *```typescript
// Dev, staging, prod
register({ projectName: "my-app-dev" });
register({ projectName: "my-app-staging" });
register({ projectName: "my-app-prod" });
```
* *A/B测试:* *```typescript
// Compare models
register({ projectName: "chatbot-gpt4" });
register({ projectName: "chatbot-claude" });
```
* *版本:* *```typescript
// Track versions
register({ projectName: "my-app-v1" });
register({ projectName: "my-app-v2" });
```
