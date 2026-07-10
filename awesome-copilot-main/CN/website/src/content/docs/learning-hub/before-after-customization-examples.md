---
title: 'Before/After Customization Examples'
description: 'See real-world transformations showing how custom agents, skills, and instructions dramatically improve GitHub Copilot effectiveness.'
authors:
  - GitHub Copilot Learning Hub Team
lastUpdated: 2025-12-12
estimatedReadingTime: '12 minutes'
tags:
  - customization
  - examples
  - fundamentals
  - best-practices
relatedArticles:
  - ./what-are-agents-skills-instructions.md
  - ./creating-effective-skills.md
  - ./defining-custom-instructions.md
---
当您看到代理、技能和指令如何转换日常开发工作流的具体示例时，GitHub Copilot定制的强大功能就变得清晰起来。本文展示了真实世界的场景，展示了默认的Copilot行为和与您的团队的标准、工具和实践相一致的定制体验之间的巨大差异。

注意：以下示例说明了典型的前后对比场景。实际的前后代码可能会根据所使用的模型和生成时存在的任何其他上下文而变化。

示例1:API客户端代码生成

###以前：通用API代码

如果没有自定义，GitHub Copilot生成的通用HTTP请求代码可能不会遵循您团队的模式：```typescript
// user-api.ts
async function getUser(userId: string) {
  // Default Copilot suggestion
  const response = await fetch(`https://api.example.com/users/${userId}`);
  const data = await response.json();
  return data;
}
```
* * * *问题:
—无错误处理或重试逻辑
-不使用现有的HTTP客户端工具
-缺少类型安全
-没有记录或遥测
-硬编码的基础URL

之后：使用自定义说明

创建`.github/instructions/typescript-api.md`:````markdown
---
description: 'API client patterns for our application'
applyTo: '**/*-api.ts, **/services/**/*.ts'
---

When generating API client code:
- Always use the HttpClient class from `@/lib/http-client`
- Include proper TypeScript types for request/response
- Add structured error handling with our ApiError class
- Include retry logic for transient failures
- Use environment-based configuration for base URLs
- Add telemetry using our logger utility
- Never hardcode API endpoints

Example pattern:
```typescript
从‘@/lib/http-client’导入{httpClient}；
从‘@/lib/errors’导入{ApiError}；
从‘@/lib/logger’导入{logger}；

async函数fetchResource<T>（端点：字符串）：Promise<T>{
尝试{    const response = await httpClient.get<T>(endpoint);
    logger.debug('API call successful', { endpoint });
    return response.data;
} catch (error) {    logger.error('API call failed', { endpoint, error });
    throw new ApiError('Failed to fetch resource', error);
  }
}
```
````
现在，Copilot生成与团队模式一致的代码：```typescript
// user-api.ts
import { httpClient } from '@/lib/http-client';
import { ApiError } from '@/lib/errors';
import { logger } from '@/lib/logger';

interface User {
  id: string;
  name: string;
  email: string;
}

async function getUser(userId: string): Promise<User> {
  try {
    const response = await httpClient.get<User>(`/users/${userId}`);
    logger.debug('User fetched successfully', { userId });
    return response.data;
  } catch (error) {
    logger.error('Failed to fetch user', { userId, error });
    throw new ApiError('Unable to retrieve user data', error);
  }
}
```
* * * *好处:
-自动使用你的团队的HTTP客户端
-包括正确的错误处理和日志记录
-接口类型安全
-始终如一地遵循团队惯例
-无需手动修正

例2：测试生成

之前：基本测试结构

默认的Copilot测试建议通常是通用的，并且忽略了特定于项目的模式：```typescript
// user-service.test.ts
import { UserService } from './user-service';

describe('UserService', () => {
  it('should get user by id', async () => {
    const service = new UserService();
    const user = await service.getUserById('123');
    expect(user).toBeDefined();
  });
});
```
* * * *问题:
-没有测试装置或工厂
—缺少setup/teardown-不使用测试工具
-没有嘲讽策略
-不完整的断言

之后：使用自定义测试技能

用`SKILL.md`创建一个技能文件夹`.github/skills/generate-tests/`：````markdown
---
name: generate-tests
description: 'Generate comprehensive test suites using our testing patterns, including fixtures, setup/teardown, and thorough assertions'
---

# generate-tests

Generate a comprehensive test suite for the selected code following these patterns:

**Setup Requirements**:
- Use our test factory functions from `@/test/factories`
- Set up database transactions with `setupTestDb()` and `cleanupTestDb()`
- Mock external services using our mock utilities from `@/test/mocks`

**Test Structure**:
- Group related tests in `describe` blocks
- Use descriptive test names that explain behavior: "should [action] when [condition]"
- Include setup/teardown in `beforeEach`/`afterEach`
- Test happy path, edge cases, and error scenarios

**Assertions**:
- Use specific assertions, not just `.toBeDefined()`
- Verify all relevant properties of returned objects
- Check error messages and types for failure cases
- Assert side effects (database changes, API calls, events)

See [references/test-patterns.md](references/test-patterns.md) for standard patterns and [templates/test-template.ts](templates/test-template.ts) for a starter structure.

**Example Pattern**:
```typescript
从‘@/test/setup’导入{setupTestDb， cleanupTestDb}；
从‘@/test/factories’中导入{createUser， createPost}；
从‘@/test/mocks’导入{mockApiClient}；

description ('UserService', () => {
beforeEach(async () => {    await setupTestDb();
});

afterEach(async () => {    await cleanupTestDb();
});

description ('getUserById', () => {    it('should return user when found', async () => {
      const user = await createUser({ name: 'Alice' });
      const service = new UserService();
      
      const result = await service.getUserById(user.id);
      
      expect(result).toEqual({
        id: user.id,
        name: 'Alice',
        email: expect.any(String)
      });
    });

    it('should throw NotFoundError when user does not exist', async () => {
      const service = new UserService();
      
      await expect(service.getUserById('nonexistent'))
        .rejects.toThrow(NotFoundError);
    });
  });
});
```

Generate tests following this pattern for the selected code.
````
现在，当您选择代码并使用`/generate-tests`时，或者当代理检测到测试需求时，Copilot会生成全面的测试套件：```typescript
// user-service.test.ts
import { setupTestDb, cleanupTestDb } from '@/test/setup';
import { createUser, createPost } from '@/test/factories';
import { mockApiClient } from '@/test/mocks';
import { UserService } from './user-service';
import { NotFoundError } from '@/lib/errors';

describe('UserService', () => {
  let service: UserService;

  beforeEach(async () => {
    await setupTestDb();
    service = new UserService();
  });

  afterEach(async () => {
    await cleanupTestDb();
  });

  describe('getUserById', () => {
    it('should return user with all properties when found', async () => {
      const user = await createUser({ 
        name: 'Alice Johnson',
        email: 'alice@example.com'
      });
      
      const result = await service.getUserById(user.id);
      
      expect(result).toEqual({
        id: user.id,
        name: 'Alice Johnson',
        email: 'alice@example.com',
        createdAt: expect.any(Date)
      });
    });

    it('should throw NotFoundError when user does not exist', async () => {
      await expect(service.getUserById('nonexistent-id'))
        .rejects.toThrow(NotFoundError);
    });

    it('should throw NotFoundError with correct message', async () => {
      await expect(service.getUserById('invalid'))
        .rejects.toThrow('User not found');
    });
  });

  describe('getUserPosts', () => {
    it('should return all posts for a user', async () => {
      const user = await createUser();
      const post1 = await createPost({ userId: user.id, title: 'First Post' });
      const post2 = await createPost({ userId: user.id, title: 'Second Post' });
      
      const result = await service.getUserPosts(user.id);
      
      expect(result).toHaveLength(2);
      expect(result[0]).toMatchObject({ title: 'First Post' });
      expect(result[1]).toMatchObject({ title: 'Second Post' });
    });

    it('should return empty array when user has no posts', async () => {
      const user = await createUser();
      
      const result = await service.getUserPosts(user.id);
      
      expect(result).toEqual([]);
    });
  });
});
```
* * * *好处:
-更丰富的测试覆盖，包括边缘情况
-使用团队的测试工具和模式
—正确的setup/teardown-综合断言
-无需修改即可运行

示例3：使用Terraform的基础设施作为代码

以前：基本地形资源

默认建议缺乏组织性、安全性最佳实践和团队约定：```hcl
# main.tf
resource "azurerm_resource_group" "rg" {
  name     = "my-rg"
  location = "eastus"
}

resource "azurerm_storage_account" "data" {
  name                     = "mystorage"
  resource_group_name      = azurerm_resource_group.rg.name
  location                 = azurerm_resource_group.rg.location
  account_tier             = "Standard"
  account_replication_type = "LRS"
  # Defaults used; public network access allowed
}
```
* * * *问题:
-没有与Azure约定一致的命名
-缺少标签
-公共网络访问不受阻；没有私有端点或网络规则
—没有审计诊断设置
—不带变量的硬编码值

之后：使用Terraform Custom Agent

使用来自存储库[`agents/terraform-azure-implement.agent.md`]（https://github.com/github/awesome-copilot/blob/main/agents/terraform-azure-implement.agent.md）的自定义代理（点击查看完整定义）。

现在，Copilot可以访问一个自定义代理，该代理可以在Azure上执行您组织的Terraform最佳实践。```hcl
# storage.tf
# Azure Storage Account for application data with secure networking and diagnostics

locals {
  storage_account_name = lower(replace("${var.environment}-${var.project_name}-st-app-data", "-", ""))

  common_tags = {
    Environment = var.environment
    Project     = var.project_name
    ManagedBy   = "Terraform"
    Owner       = var.owner_team
    CostCenter  = var.cost_center
  }
}

resource "azurerm_resource_group" "rg" {
  name     = "${var.environment}-${var.project_name}-rg"
  location = var.location

  tags = merge(local.common_tags, { Name = "${var.environment}-${var.project_name}-rg" })
}

resource "azurerm_storage_account" "app_data" {
  name                     = local.storage_account_name
  resource_group_name      = azurerm_resource_group.rg.name
  location                 = azurerm_resource_group.rg.location
  account_tier             = "Standard"
  account_replication_type = "ZRS"
  min_tls_version          = "TLS1_2"
  public_network_access_enabled = false

  tags = merge(local.common_tags, {
    Name        = local.storage_account_name
    Description = "Application data storage account"
  })
}

resource "azurerm_log_analytics_workspace" "law" {
  name                = "${var.environment}-${var.project_name}-law"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  sku                 = "PerGB2018"
  retention_in_days   = 30

  tags = local.common_tags
}

resource "azurerm_monitor_diagnostic_setting" "sa_diagnostics" {
  name                           = "${local.storage_account_name}-diag"
  target_resource_id             = azurerm_storage_account.app_data.id
  log_analytics_workspace_id     = azurerm_log_analytics_workspace.law.id

  enabled_log {
    category = "StorageRead"
  }

  enabled_log {
    category = "StorageWrite"
  }

  enabled_log {
    category = "StorageDelete"
  }

  metric {
    category = "AllMetrics"
    enabled  = true
  }
}

output "storage_account_name" {
  description = "Name of the application data Storage Account"
  value       = azurerm_storage_account.app_data.name
}

output "storage_account_id" {
  description = "ID of the application data Storage Account"
  value       = azurerm_storage_account.app_data.id
}
```
* * * *好处:
-默认安全配置（静态加密，TLS 1.2+）
-遵循Azure命名和标记约定
—启用诊断后阻止公共访问
——资源和产出的完全分离
-准备扩展私有端点和CMK，如果需要的话

例4：代码评审注释

之前：手动评审过程

开发人员手动审核拉取请求并编写评论，这既耗时又不一致：

* *手动过程* *:
1. 通读代码更改
2. 思考潜在的问题
3. 写详细的反馈
4. 格式代码建议
5. 确保建设性语气

时间投入：每个PR 20-30分钟

之后：具备代码审查技能

用`SKILL.md`创建一个技能文件夹`skills/review-pr/`：````markdown
---
name: review-pr
description: 'Generate comprehensive code review with actionable feedback, covering correctness, security, performance, and maintainability'
---

# review-pr

Analyze the current git diff and provide a structured code review with:

**Structure**:
1. **Summary**: Brief overview of changes
2. **Positive Feedback**: What's done well
3. **Issues Found**: Problems categorized by severity (Critical/Major/Minor)
4. **Suggestions**: Specific improvements with code examples
5. **Questions**: Clarifications needed

**Review Focus**:
- Code correctness and logic errors
- Security vulnerabilities
- Performance implications
- Test coverage
- Code style and maintainability
- Error handling
- Documentation needs

**Tone**:
- Be constructive and helpful
- Explain the "why" behind suggestions
- Offer specific fixes, not just critique
- Acknowledge good patterns

**Format**:
Use markdown with code blocks for suggestions. For each issue, provide:
- Clear description of the problem
- Why it matters
- Suggested fix with code example

See [references/review-checklist.md](references/review-checklist.md) for the standard review checklist.

Example format:
## 🔴 Critical Issues

### Missing Input Validation
**Problem**: The `processUser` function doesn't validate the email format.
**Impact**: Could lead to invalid data in the database.
**Fix**:
```typescript
函数processUser(user: user) {
如果(! isValidEmail(用户。电子邮件)){    throw new ValidationError('Invalid email format');
}
/ /……函数的剩余部分
}```
````
在PR上使用此技能（通过`/review-pr`或代理调用）生成结构化的、可操作的反馈：````markdown
## Summary
This PR adds user authentication with JWT tokens. The implementation includes login, token generation, and middleware for protected routes.

## 🎉 Positive Feedback
- Good separation of concerns with separate auth service and middleware
- Comprehensive error handling in the login flow
- Clear function naming that makes the code self-documenting

## 🔴 Critical Issues

### 1. Hardcoded Secret Key
**Problem**: The JWT secret is hardcoded in `auth.service.ts`:
```typescript
Const secret = 'my-secret-key'；//❌硬编码```
**Impact**: Security vulnerability - secrets should never be in source code.
**Fix**:
```typescript
const secret = process.env.JWT_SECRET；
如果(!秘密){
抛出新的错误（‘JWT_SECRET环境变量是必需的’）；
}```

### 2. Missing Password Hashing Salt Rounds
**Problem**: Password hashing uses default bcrypt rounds which may be too low.
**Impact**: Weak password protection if defaults are insufficient.
**Fix**:
```typescript
const SALT_ROUNDS = 12；//显示配置
const hashhedpassword =等待密码。散列(密码,SALT_ROUNDS);```

## ⚠️ Major Issues

### 3. No Token Expiration Validation
**Problem**: The auth middleware doesn't check token expiration explicitly.
**Impact**: Expired tokens might be accepted if JWT library doesn't verify.
**Fix**: Add explicit expiration check in the middleware.

## 💡 Suggestions

### 4. Consider Adding Refresh Tokens
The current implementation uses only access tokens. Consider implementing refresh tokens for better security and user experience:
- Short-lived access tokens (15 minutes)
- Longer-lived refresh tokens (7 days)
- Token rotation on refresh

### 5. Add Rate Limiting
The login endpoint should have rate limiting to prevent brute force attacks:
```typescript
import rateLimit from 'express-rate-limit'；

const loginlimit = rateLimit({
窗口：15 * 60 * 1000，// 15分钟
Max: 5， // 5次尝试
消息：“登录尝试太多，请稍后再试”
});

app.post('/login', loginlimit, authController.login)；```

## ❓ Questions
1. What's the intended token expiration time for production?
2. Should we implement token revocation for logout functionality?
3. Are there plans for multi-factor authentication in the future?
````
* * * *好处:
-一致的评审质量
-更快的审查过程（5-10分钟）
-结构化的、可操作的反馈
-包括修复的代码示例
—系统地识别安全问题
-保持建设性的语气

##关键要点

以下示例演示了定制如何将GitHub Copilot从通用助手转变为特定于团队的专家：

1. **指令**将团队的模式自动嵌入到每个建议中
2. **技能**标准化工作流程与捆绑的资源和启用代理发现
3. **代理**为复杂领域带来专业知识
4. **三者的结合**创造了一个全面的开发助手在创建定制方面的投资通过以下方式获得回报：
-更快的开发，更少的手动更正
-整个团队一致的代码质量
-自动遵循最佳实践
-减少新团队成员的入职时间
—更好的安全性和可维护性