---
description: 'Generic code review instructions that can be customized for any project using GitHub Copilot'
applyTo: '**'
excludeAgent: ["coding-agent"]
---
#通用代码审查说明GitHub Copilot的全面代码审查指南，可以适用于任何项目。这些指导遵循来自即时工程的最佳实践，并提供了一种结构化的方法来实现代码质量、安全性、测试和体系结构审查。

##复习语言

当执行代码审查时，用**英语**回应（或指定您的首选语言）。

b> **自定义提示**：通过将“英语”替换为“葡萄牙语（巴西语）”，“西班牙语”，“法语”等来更改您的首选语言。

##审查优先级

在执行代码审查时，按以下顺序对问题进行优先排序：###🔴CRITICAL（块合并）
- **安全**：漏洞，暴露的秘密，authentication/authorization问题
- **正确性**：逻辑错误，数据损坏风险，竞争条件
- **突破性更改**:API合约更改而不进行版本控制
- **数据丢失**：数据丢失或损坏的风险

###🟡重要（需要讨论）
- **代码质量**：严重违反SOLID原则，过度重复
- **测试覆盖率**：缺少关键路径或新功能的测试
**性能**：明显的性能瓶颈（N+1查询，内存泄漏）
- **架构**：明显偏离已建立的模式🟢建议（非阻塞改进）
- **可读性**：命名差，逻辑复杂，可以简化
- **优化**：不影响功能的性能改进
- **最佳实践**：与惯例的轻微偏差
- **文档**：缺少或不完整的comments/documentation##一般审查原则

在执行代码审查时，请遵循以下原则：1. **具体**：引用确切的行，文件，并提供具体的例子
2. **提供上下文**：解释为什么某件事是一个问题和潜在的影响
3. **建议解决方案**：在适用的情况下显示正确的代码，而不仅仅是错误的地方
4. **具有建设性**：专注于改进代码，而不是批评作者
5. **认可良好的实践**：认可编写良好的代码和聪明的解决方案
6. **务实**：并非所有建议都需要立即实施
7. **分组相关评论**：避免对同一主题进行多次评论

代码质量标准

在执行代码审查时，检查：整洁的代码
-变量、函数和类的描述性和有意义的名称
-单一职责原则：每个function/class只做好一件事
DRY (Don't Repeat Yourself)：不要重复代码
-函数应该小而集中（理想情况下< 20-30行）
-避免深度嵌套代码（最多3-4级）
避免魔术数字和字符串（使用常量）
-代码应该是自文档化的；只在必要时注释

# # #的例子```javascript
// ❌ BAD: Poor naming and magic numbers
function calc(x, y) {
    if (x > 100) return y * 0.15;
    return y * 0.10;
}

// ✅ GOOD: Clear naming and constants
const PREMIUM_THRESHOLD = 100;
const PREMIUM_DISCOUNT_RATE = 0.15;
const STANDARD_DISCOUNT_RATE = 0.10;

function calculateDiscount(orderTotal, itemPrice) {
    const isPremiumOrder = orderTotal > PREMIUM_THRESHOLD;
    const discountRate = isPremiumOrder ? PREMIUM_DISCOUNT_RATE : STANDARD_DISCOUNT_RATE;
    return itemPrice * discountRate;
}
```
错误处理
—在适当的级别上进行正确的错误处理
-有意义的错误信息
—没有静默失败或忽略异常
—快速失败：尽早验证输入
—使用适当的错误types/exceptions# # #的例子```python
# ❌ BAD: Silent failure and generic error
def process_user(user_id):
    try:
        user = db.get(user_id)
        user.process()
    except:
        pass

# ✅ GOOD: Explicit error handling
def process_user(user_id):
    if not user_id or user_id <= 0:
        raise ValueError(f"Invalid user_id: {user_id}")

    try:
        user = db.get(user_id)
    except UserNotFoundError:
        raise UserNotFoundError(f"User {user_id} not found in database")
    except DatabaseError as e:
        raise ProcessingError(f"Failed to retrieve user {user_id}: {e}")

    return user.process()
```
##安全审查

在执行代码审查时，检查安全问题：

- **敏感数据**：代码或日志中没有密码、API密钥、令牌或PII
- **输入验证**：所有用户输入都经过验证和消毒
- **SQL注入**：使用参数化查询，从不使用字符串连接
—**认证**：访问资源前进行适当的认证检查
—**授权**：验证用户是否有权限执行操作
- **密码学**：使用已建立的库，永远不要滚动自己的加密
- **依赖安全**：检查依赖中已知的漏洞

# # #的例子```java
// ❌ BAD: SQL injection vulnerability
String query = "SELECT * FROM users WHERE email = '" + email + "'";

// ✅ GOOD: Parameterized query
PreparedStatement stmt = conn.prepareStatement(
    "SELECT * FROM users WHERE email = ?"
);
stmt.setString(1, email);
```

```javascript
// ❌ BAD: Exposed secret in code
const API_KEY = "sk_live_abc123xyz789";

// ✅ GOOD: Use environment variables
const API_KEY = process.env.API_KEY;
```
##测试标准

当执行代码评审时，验证测试质量：

- **覆盖率**：关键路径和新功能必须经过测试
- **测试名称**：解释正在测试的内容的描述性名称
- **测试结构**：清除安排-行为-断言或给定-何时-然后模式
- **独立性**：测试不应该依赖于彼此或外部状态
**断言**：使用特定的断言，避免通用的assertTrue/assertFalse- **边缘情况**：测试边界条件，空值，空集合
- **适当地模拟**：模拟外部依赖关系，而不是域逻辑

# # #的例子```typescript
// ❌ BAD: Vague name and assertion
test('test1', () => {
    const result = calc(5, 10);
    expect(result).toBeTruthy();
});

// ✅ GOOD: Descriptive name and specific assertion
test('should calculate 10% discount for orders under $100', () => {
    const orderTotal = 50;
    const itemPrice = 20;

    const discount = calculateDiscount(orderTotal, itemPrice);

    expect(discount).toBe(2.00);
});
```
性能考虑

在执行代码审查时，检查性能问题：

- **数据库查询**：避免N+1查询，使用适当的索引
- **算法**：适合用例的time/space复杂度
- **缓存**：对昂贵或重复的操作使用缓存
- **资源管理**：正确清理连接，文件，流
- **分页**：应该对大型结果集进行分页
—**延迟加载**：仅在需要时加载数据

# # #的例子```python
# ❌ BAD: N+1 query problem
users = User.query.all()
for user in users:
    orders = Order.query.filter_by(user_id=user.id).all()  # N+1!

# ✅ GOOD: Use JOIN or eager loading
users = User.query.options(joinedload(User.orders)).all()
for user in users:
    orders = user.orders
```
##建筑与设计

当执行代码审查时，验证架构原则：

- **关注点分离**：明确layers/modules之间的边界
- **依赖方向**：高级模块不依赖于低级细节
- **接口隔离**：更喜欢小的、集中的接口
- **松耦合**：组件应独立测试
- **高内聚**：将相关功能组合在一起
一致的模式：遵循代码库中已建立的模式

文档标准

在执行代码审查时，检查文档：**API文档**：公共API必须文档化（目的，参数，返回）
- **复杂逻辑**：不明显的逻辑应该有解释性注释
- **README更新**：在添加功能或更改设置时更新README
- **重大变更**：清楚地记录任何重大变更
—**示例**：提供复杂特性的使用示例

##注释格式模板

在执行代码审查时，使用以下格式进行注释：```markdown
**[PRIORITY] Category: Brief title**

Detailed description of the issue or suggestion.

**Why this matters:**
Explanation of the impact or reason for the suggestion.

**Suggested fix:**
[code example if applicable]

**Reference:** [link to relevant documentation or standard]
```
示例注释

####关键问题````markdown
**🔴 CRITICAL - Security: SQL Injection Vulnerability**

The query on line 45 concatenates user input directly into the SQL string,
creating a SQL injection vulnerability.

**Why this matters:**
An attacker could manipulate the email parameter to execute arbitrary SQL commands,
potentially exposing or deleting all database data.

**Suggested fix:**
```sql
——而不是：
查询= "SELECT * FROM users WHERE email = '" + email + “'”

——使用:
PreparedStatement stmt = conn.prepareStatement    "SELECT * FROM users WHERE email = ?"
）；
支撑。setString(1、电子邮件);```

**Reference:** OWASP SQL Injection Prevention Cheat Sheet
````
####重要问题````markdown
**🟡 IMPORTANT - Testing: Missing test coverage for critical path**

The `processPayment()` function handles financial transactions but has no tests
for the refund scenario.

**Why this matters:**
Refunds involve money movement and should be thoroughly tested to prevent
financial errors or data inconsistencies.

**Suggested fix:**
Add test case:
```javascript
Test(‘取消订单时应处理全额退款’，（）=> {    const order = createOrder({ total: 100, status: 'cancelled' });

    const result = processPayment(order, { type: 'refund' });

    expect(result.refundAmount).toBe(100);
    expect(result.status).toBe('refunded');
});
```
````
# # # #的建议````markdown
**🟢 SUGGESTION - Readability: Simplify nested conditionals**

The nested if statements on lines 30-40 make the logic hard to follow.

**Why this matters:**
Simpler code is easier to maintain, debug, and test.

**Suggested fix:**
```javascript
//代替嵌套的if：
If (user) {    if (user.isActive) {
        if (user.hasPermission('write')) {
            // do something
        }
    }
}

//考虑保护子句：
如果(!用户|| ！isActive || !用户。hasPermission(“写”)){    return;
}
//做某事```
````
##检查清单

在执行代码审查时，系统地验证：

代码质量
-[]代码遵循一致的风格和约定
—[]名称是描述性的，遵循命名约定
- []Functions/methods小而专注
-[]禁止代码重复
—[]复杂的逻辑被分解成更简单的部分
-[]正确处理错误
-[]没有注释掉的代码或TODO没有罚单

# # #安全
-[]代码和日志中没有敏感数据
-[]对所有用户输入进行输入验证
-[]无SQL注入漏洞
-[]认证授权是否正确
-[]依赖项是最新的和安全的

# # #测试
-[]新代码有适当的测试覆盖率
-[]测试的名称和重点都很好
—[]测试涵盖边缘情况和错误场景
—[]测试是独立且确定的
-[]没有总是通过或被注释掉的测试# # #性能
-没有明显的性能问题（N+1，内存泄漏）
-[]正确使用缓存
-[]高效的算法和数据结构
-[]合理清理资源

# # #架构
-[]遵循既定的模式和惯例
-[]适当分离关注点
-[]无建筑违规
-[]依赖关系流向正确

# # #文档
-[]公开api文档
-[]复杂逻辑有解释性注释
-[]如果需要，会更新README
-[]破坏性变更被记录

特定于项目的自定义

要为您的项目定制此模板，请添加以下部分：

1. **Language/Framework具体检查**
-示例：“当执行代码审查时，验证React钩子遵循钩子规则”
示例：“当执行代码审查时，检查Spring Boot控制器是否使用了正确的注释”2. **构建和部署
—示例：“执行代码审查时，验证CI/CD管道配置是否正确”
-示例：“当执行代码审查时，检查数据库迁移是否可逆”

3. 业务逻辑规则
-示例：“在执行代码审查时，验证定价计算包括所有适用的税费”
—示例：“在执行代码评审时，在数据处理之前检查用户是否同意”

4. * * * *团队约定
-示例：“在执行代码审查时，验证提交消息是否遵循常规提交格式”
-示例：“当执行代码审查时，检查分支名称是否遵循模式：type/ticket-description”

##其他资源

有关有效代码审查和GitHub Copilot定制的更多信息：- [GitHub Copilot提示工程]（https://docs.github.com/en/copilot/concepts/prompting/prompt-engineering）
- [GitHub Copilot自定义指令]（https://code.visualstudio.com/docs/copilot/customization/custom-instructions）
- [Awesome GitHub CopilotRepository]（https://github.com/github/awesome-copilot）
- [GitHub代码审查指南]（https://docs.github.com/en/pull-requests/collaborating-with-pull-requests/reviewing-changes-in-pull-requests）
- [b谷歌工程实践-代码审查]（https://google.github.io/eng-practices/review/）
- [OWASP安全指南]（https://owasp.org/）

提示工程技巧

当执行代码审查时，应用来自[GitHub Copilot文档]（https://docs.github.com/en/copilot/concepts/prompting/prompt-engineering）的这些提示工程原则：1. **从一般开始，然后具体**：从高级架构审查开始，然后深入实现细节
2. **给出示例**：在建议更改时引用代码库中的类似模式
3. **分解复杂任务**：按逻辑块审查大型pr（安全→测试→逻辑→风格）
4. **避免歧义**：具体说明要处理的文件、行和问题
5. **注明相关代码**：引用可能受变更影响的相关代码
6. **实验和迭代**：如果最初的审查遗漏了什么，那么就带着重点问题再次审查

##项目背景

这是一个通用模板。使用您的项目特定信息自定义此部分：- **技术栈**:[例如，Java 17, Spring Boot 3。x, PostgreSQL)
- **架构**:[例如，Hexagonal/Clean架构，微服务]
- **构建工具**:[例如，Gradle, Maven, npm, pip]
**测试**:[例如，JUnit 5, Jest, pytest]
- **代码风格**:[例如，遵循谷歌风格指南]