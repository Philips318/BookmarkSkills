---
description: 'Guidelines and best practices for Apex development on the Salesforce Platform'
applyTo: '**/*.cls, **/*.trigger'
---
# Apex开发

##一般使用说明

-始终为Salesforce平台使用最新的Apex功能和最佳实践。
-为每个类和方法编写清晰简洁的注释，解释业务逻辑和任何复杂的操作。
—处理边缘情况，并通过有意义的错误消息实现适当的异常处理。
关注批量化——编写处理记录集合的代码，而不是处理单个记录的代码。
-注意调控器限制和设计有效扩展的解决方案。
-使用服务层、域类和选择器类实现适当的关注点分离。
在注释中记录外部依赖项、集成点及其目的。

命名约定—**类**：使用`PascalCase`作为类名。描述性地命名类以反映它们的用途。
控制器：后缀为`Controller`（例如，`AccountController`）
-触发处理程序：后缀`TriggerHandler`（例如，`AccountTriggerHandler`）
-服务类：后缀为`Service`（例如，`AccountService`）
-选择器类：后缀为`Selector`（例如，`AccountSelector`）
-测试类：后缀为`Test`（例如，`AccountServiceTest`）
批处理类：后缀为`Batch`（例如，`AccountCleanupBatch`）
-可排队类：后缀为`Queueable`（例如，`EmailNotificationQueueable`）

—**Methods**：使用`camelCase`作为方法名。用动词来表示动作。
-好：`getActiveAccounts()`，`updateContactEmail()`,`deleteExpiredRecords()`—避免使用缩写：`getAccs()`→`getAccounts()`—**变量**：使用`camelCase`作为变量名。使用描述性的名字。
-好：`accountList`、`emailAddress`、`totalAmount`—除了循环计数器外，避免使用单个字母：`a`→`account`—**常量**：使用`UPPER_SNAKE_CASE`作为常量。
-好：`MAX_BATCH_SIZE`，`DEFAULT_EMAIL_TEMPLATE`,`ERROR_MESSAGE_PREFIX`- **触发器**：将触发器命名为`ObjectName`+触发事件（例如，`AccountTrigger`,`ContactTrigger`）

最佳实践

# # # Bulkification

设计所有的代码来处理记录的集合，而不是单独的记录。
—避免在循环中使用SOQL查询和DML语句。
—使用集合（`List<>`、`Set<>`、`Map<>`）高效处理多条记录。```apex
// Good Example - Bulkified
public static void updateAccountRating(List<Account> accounts) {
    for (Account acc : accounts) {
        if (acc.AnnualRevenue > 1000000) {
            acc.Rating = 'Hot';
        }
    }
    update accounts;
}

// Bad Example - Not bulkified
public static void updateAccountRating(Account account) {
    if (account.AnnualRevenue > 1000000) {
        account.Rating = 'Hot';
        update account; // DML in a method designed for single records
    }
}
```
映射为O(1)查找

** -将列表转换为映射，用于O(1)个常量时间查找，而不是O(n)个列表迭代。
—使用`Map<Id, SObject>`构造函数将查询结果快速转换为映射。
-适合匹配相关记录，查找和避免嵌套循环。```apex
// Good Example - Using Map for O(1) lookup
Map<Id, Account> accountMap = new Map<Id, Account>([
    SELECT Id, Name, Industry FROM Account WHERE Id IN :accountIds
]);

for (Contact con : contacts) {
    Account acc = accountMap.get(con.AccountId);
    if (acc != null) {
        con.Industry__c = acc.Industry;
    }
}

// Bad Example - Nested loop with O(n²) complexity
List<Account> accounts = [SELECT Id, Name, Industry FROM Account WHERE Id IN :accountIds];

for (Contact con : contacts) {
    for (Account acc : accounts) {
        if (con.AccountId == acc.Id) {
            con.Industry__c = acc.Industry;
            break;
        }
    }
}

// Good Example - Map for grouping records
Map<Id, List<Contact>> contactsByAccountId = new Map<Id, List<Contact>>();
for (Contact con : contacts) {
    if (!contactsByAccountId.containsKey(con.AccountId)) {
        contactsByAccountId.put(con.AccountId, new List<Contact>());
    }
    contactsByAccountId.get(con.AccountId).add(con);
}
```
州长限制

注意Salesforce调控器的限制：SOQL查询（100），DML语句（150），堆大小（6MB）， CPU时间（10秒）。
- **使用`System.Limits`类主动监控调控器限制，在达到限制之前检查消耗。
-使用高效的SOQL查询，选择过滤器和适当的索引。
-实现SOQL for循环，用于处理大型数据集。
—使用**Batch Apex**进行大数据卷（>50,000条记录）的操作。
-利用**平台缓存**减少冗余的SOQL查询。```apex
// Good Example - SOQL for loop for large data sets
public static void processLargeDataSet() {
    for (List<Account> accounts : [SELECT Id, Name FROM Account]) {
        // Process batch of 200 records
        processAccounts(accounts);
    }
}

// Good Example - Using WHERE clause to reduce query results
List<Account> accounts = [SELECT Id, Name FROM Account WHERE IsActive__c = true LIMIT 200];
```
安全性和数据访问

- **在执行SOQL查询或DML操作之前，始终检查CRUD/FLS权限**。
—在SOQL查询中使用`WITH SECURITY_ENFORCED`来加强字段级安全性。
—使用`Security.stripInaccessible()`删除用户无法访问的字段。
—对强制共享规则的类实现`WITH SHARING`关键字。
-仅在必要时使用`WITHOUT SHARING`，并记录原因。
-对实用程序类使用`INHERITED SHARING`来继承调用上下文。```apex
// Good Example - Checking CRUD and using stripInaccessible
public with sharing class AccountService {
    public static List<Account> getAccounts() {
        if (!Schema.sObjectType.Account.isAccessible()) {
            throw new SecurityException('User does not have access to Account object');
        }

        List<Account> accounts = [SELECT Id, Name, Industry FROM Account WITH SECURITY_ENFORCED];

        SObjectAccessDecision decision = Security.stripInaccessible(
            AccessType.READABLE, accounts
        );

        return decision.getRecords();
    }
}

// Good Example - WITH SHARING for sharing rules
public with sharing class AccountController {
    // This class enforces record-level sharing
}
```
异常处理

-对于DML操作和调用总是使用try-catch块。
—针对特定的错误场景创建自定义异常类。
—适当记录异常，以便调试和监控。
—向用户提供有意义的错误提示。```apex
// Good Example - Proper exception handling
public class AccountService {
    public class AccountServiceException extends Exception {}

    public static void safeUpdate(List<Account> accounts) {
        try {
            if (!Schema.sObjectType.Account.isUpdateable()) {
                throw new AccountServiceException('User does not have permission to update accounts');
            }
            update accounts;
        } catch (DmlException e) {
            System.debug(LoggingLevel.ERROR, 'DML Error: ' + e.getMessage());
            throw new AccountServiceException('Failed to update accounts: ' + e.getMessage());
        }
    }
}
```
SOQL最佳实践

-对索引字段（`Id`,`Name`,`OwnerId`，自定义索引字段）使用选择性查询。
—在适当的时候使用`LIMIT`子句限制查询结果。
-使用`LIMIT 1`时，你只需要一个记录。
-避免使用`SELECT *`-始终指定必填字段。
—使用关系查询，尽量减少SOQL查询的数量。
-在可能的情况下按索引字段排序查询。
—**在SOQL查询中使用用户输入时，始终使用`String.escapeSingleQuotes()`**，防止SOQL注入攻击。
- **检查查询选择性** -目标为>10%的选择性（过滤器将结果减少到小于总记录的10%）。
—使用“**查询计划**”验证查询效率和索引使用情况。
-用真实的数据量测试查询，以确保性能。```apex
// Good Example - Selective query with indexed fields
List<Account> accounts = [
    SELECT Id, Name, (SELECT Id, LastName FROM Contacts)
    FROM Account
    WHERE OwnerId = :UserInfo.getUserId()
    AND CreatedDate = THIS_MONTH
    LIMIT 100
];

// Good Example - LIMIT 1 for single record
Account account = [SELECT Id, Name FROM Account WHERE Name = 'Acme' LIMIT 1];

// Good Example - escapeSingleQuotes() to prevent SOQL injection
String searchTerm = String.escapeSingleQuotes(userInput);
List<Account> accounts = Database.query('SELECT Id, Name FROM Account WHERE Name LIKE \'%' + searchTerm + '%\'');

// Bad Example - Direct user input without escaping (SECURITY RISK)
List<Account> accounts = Database.query('SELECT Id, Name FROM Account WHERE Name LIKE \'%' + userInput + '%\'');

// Good Example - Selective query with indexed fields (high selectivity)
List<Account> accounts = [
    SELECT Id, Name FROM Account
    WHERE OwnerId = :UserInfo.getUserId()
    AND CreatedDate = TODAY
    LIMIT 100
];

// Bad Example - Non-selective query (scans entire table)
List<Account> accounts = [
    SELECT Id, Name FROM Account
    WHERE Description LIKE '%test%'  // Non-indexed field
];

// Check query performance in Developer Console:
// 1. Enable 'Use Query Plan' in Developer Console
// 2. Run SOQL query and review 'Query Plan' tab
// 3. Look for 'Index' usage vs 'TableScan'
// 4. Ensure selectivity > 10% for optimal performance
```
触发最佳实践

-每个对象使用一个触发器，以保持清晰度并避免冲突。
在处理程序类中实现触发器逻辑，而不是直接在触发器中实现。
—使用触发器框架进行一致的触发器管理。
-利用触发上下文变量：`Trigger.new`，`Trigger.old`,`Trigger.newMap`,`Trigger.oldMap`。
-检查触发上下文：`Trigger.isBefore`，`Trigger.isAfter`，`Trigger.isInsert`等。```apex
// Good Example - Trigger with handler pattern
trigger AccountTrigger on Account (before insert, before update, after insert, after update) {
    new AccountTriggerHandler().run();
}

// Handler Class
public class AccountTriggerHandler extends TriggerHandler {
    private List<Account> newAccounts;
    private List<Account> oldAccounts;
    private Map<Id, Account> newAccountMap;
    private Map<Id, Account> oldAccountMap;

    public AccountTriggerHandler() {
        this.newAccounts = (List<Account>) Trigger.new;
        this.oldAccounts = (List<Account>) Trigger.old;
        this.newAccountMap = (Map<Id, Account>) Trigger.newMap;
        this.oldAccountMap = (Map<Id, Account>) Trigger.oldMap;
    }

    public override void beforeInsert() {
        AccountService.setDefaultValues(newAccounts);
    }

    public override void afterUpdate() {
        AccountService.handleRatingChange(newAccountMap, oldAccountMap);
    }
}
```
代码质量最佳实践- **使用`isEmpty()`** -检查集合是否为空使用内置方法而不是大小比较。
- **使用自定义标签** -在自定义标签中存储面向用户的文本，以实现国际化和可维护性。
- **使用常量** -定义硬编码值、错误消息和配置值的常量。
- **使用`String.isBlank()`和`String.isNotBlank()`** -检查null或空字符串正确。
—**使用`String.valueOf()`**—将值安全转换为字符串，避免空指针异常。
- **使用安全导航操作符`?.`** -安全访问属性和方法，没有空指针异常。
- **使用空合并运算符`??`** -为空表达式提供默认值。
- **避免在循环中使用`+`进行字符串连接** -使用`String.join()`以获得更好的性能。
- **使用收集方法** -利用`List.clone()`，`Set.addAll()`，`Map.keySet()`更干净的代码。
- **使用三元操作符** - For简单的条件赋值，提高可读性。
- **使用开关表达式** - if-else链的现代替代品，具有更好的可读性和性能。
- **使用SObject克隆方法** -在需要时适当克隆SObject以避免意外引用。```apex
// Good Example - Switch expression (modern Apex)
String rating = switch on account.AnnualRevenue {
    when 0 { 'Cold'; }
    when 1, 2, 3 { 'Warm'; }
    when else { 'Hot'; }
};

// Good Example - Switch on SObjectType
String objectLabel = switch on record {
    when Account a { 'Account: ' + a.Name; }
    when Contact c { 'Contact: ' + c.LastName; }
    when else { 'Unknown'; }
};

// Bad Example - if-else chain
String rating;
if (account.AnnualRevenue == 0) {
    rating = 'Cold';
} else if (account.AnnualRevenue >= 1 && account.AnnualRevenue <= 3) {
    rating = 'Warm';
} else {
    rating = 'Hot';
}

// Good Example - SObject clone methods
Account original = new Account(Name = 'Acme', Industry = 'Technology');

// Shallow clone with ID and relationships
Account clone1 = original.clone(true, true);

// Shallow clone without ID or relationships
Account clone2 = original.clone(false, false);

// Deep clone with all relationships
Account clone3 = original.deepClone(true, true, true);

// Good Example - isEmpty() instead of size comparison
if (accountList.isEmpty()) {
    System.debug('No accounts found');
}

// Bad Example - size comparison
if (accountList.size() == 0) {
    System.debug('No accounts found');
}

// Good Example - Custom Labels for user-facing text
final String ERROR_MESSAGE = System.Label.Account_Update_Error;
final String SUCCESS_MESSAGE = System.Label.Account_Update_Success;

// Bad Example - Hardcoded strings
final String ERROR_MESSAGE = 'An error occurred while updating the account';

// Good Example - Constants for configuration values
public class AccountService {
    private static final Integer MAX_RETRY_ATTEMPTS = 3;
    private static final String DEFAULT_INDUSTRY = 'Technology';
    private static final String ERROR_PREFIX = 'AccountService Error: ';

    public static void processAccounts() {
        // Use constants
        if (retryCount > MAX_RETRY_ATTEMPTS) {
            throw new AccountServiceException(ERROR_PREFIX + 'Max retries exceeded');
        }
    }
}

// Good Example - isBlank() for null and empty checks
if (String.isBlank(account.Name)) {
    account.Name = DEFAULT_NAME;
}

// Bad Example - multiple null checks
if (account.Name == null || account.Name == '') {
    account.Name = DEFAULT_NAME;
}

// Good Example - String.valueOf() for safe conversion
String accountId = String.valueOf(account.Id);
String revenue = String.valueOf(account.AnnualRevenue);

// Good Example - Safe navigation operator (?.)
String ownerName = account?.Owner?.Name;
Integer contactCount = account?.Contacts?.size();

// Bad Example - Nested null checks
String ownerName;
if (account != null && account.Owner != null) {
    ownerName = account.Owner.Name;
}

// Good Example - Null-coalescing operator (??)
String accountName = account?.Name ?? 'Unknown Account';
Integer revenue = account?.AnnualRevenue ?? 0;
String industry = account?.Industry ?? DEFAULT_INDUSTRY;

// Bad Example - Ternary with null check
String accountName = account != null && account.Name != null ? account.Name : 'Unknown Account';

// Good Example - Combining ?. and ??
String email = contact?.Email ?? contact?.Account?.Owner?.Email ?? 'no-reply@example.com';

// Good Example - String concatenation in loops
List<String> accountNames = new List<String>();
for (Account acc : accounts) {
    accountNames.add(acc.Name);
}
String result = String.join(accountNames, ', ');

// Bad Example - String concatenation in loops
String result = '';
for (Account acc : accounts) {
    result += acc.Name + ', '; // Poor performance
}

// Good Example - Ternary operator
String status = isActive ? 'Active' : 'Inactive';

// Good Example - Collection methods
List<Account> accountsCopy = accountList.clone();
Set<Id> accountIds = new Set<Id>(accountMap.keySet());
```
递归预防

- **使用静态变量**跟踪递归调用，防止无限循环。
-实现**断路器**模式，在阈值后停止执行。
-记录递归限制和潜在风险。```apex
// Good Example - Recursion prevention with static variable
public class AccountTriggerHandler extends TriggerHandler {
    private static Boolean hasRun = false;

    public override void afterUpdate() {
        if (!hasRun) {
            hasRun = true;
            AccountService.updateRelatedContacts(Trigger.newMap.keySet());
        }
    }
}

// Good Example - Circuit breaker with counter
public class OpportunityService {
    private static Integer recursionCount = 0;
    private static final Integer MAX_RECURSION_DEPTH = 5;

    public static void processOpportunity(Id oppId) {
        recursionCount++;

        if (recursionCount > MAX_RECURSION_DEPTH) {
            System.debug(LoggingLevel.ERROR, 'Max recursion depth exceeded');
            return;
        }

        try {
            // Process opportunity logic
        } finally {
            recursionCount--;
        }
    }
}
```
方法可见性和封装

- **默认使用`private`** -只公开需要公开的方法。
-对子类需要访问的方法使用`protected`。
-仅对其他类需要调用的api使用`public`。
- **在适当的时候使用`final`关键字**来防止方法覆盖。
-标记类为`final`，如果他们不应该被扩展。```apex
// Good Example - Proper encapsulation
public class AccountService {
    // Public API
    public static void updateAccounts(List<Account> accounts) {
        validateAccounts(accounts);
        performUpdate(accounts);
    }

    // Private helper - not exposed
    private static void validateAccounts(List<Account> accounts) {
        for (Account acc : accounts) {
            if (String.isBlank(acc.Name)) {
                throw new IllegalArgumentException('Account name is required');
            }
        }
    }

    // Private implementation - not exposed
    private static void performUpdate(List<Account> accounts) {
        update accounts;
    }
}

// Good Example - Final keyword to prevent extension
public final class UtilityHelper {
    // Cannot be extended
    public static String formatCurrency(Decimal amount) {
        return '$' + amount.setScale(2);
    }
}

// Good Example - Final method to prevent override
public virtual class BaseService {
    // Can be overridden
    public virtual void process() {
        // Implementation
    }

    // Cannot be overridden
    public final void validateInput() {
        // Critical validation that must not be changed
    }
}
```
设计模式

- **服务层模式：将业务逻辑封装在服务类中。
- **断路器模式**：通过在阈值后停止执行来防止重复故障。
- **选择器模式：为SOQL查询创建专用类。
—**Domain Layer Pattern**：实现特定于记录的逻辑的域类。
- **触发器处理程序模式**：使用一致的框架进行触发器管理。
- **Builder模式**：用于复杂对象的构造。
- **策略模式**：根据条件实现不同的行为。```apex
// Good Example - Service Layer Pattern
public class AccountService {
    public static void updateAccountRatings(Set<Id> accountIds) {
        List<Account> accounts = AccountSelector.selectByIds(accountIds);

        for (Account acc : accounts) {
            acc.Rating = calculateRating(acc);
        }

        update accounts;
    }

    private static String calculateRating(Account acc) {
        if (acc.AnnualRevenue > 1000000) {
            return 'Hot';
        } else if (acc.AnnualRevenue > 500000) {
            return 'Warm';
        }
        return 'Cold';
    }
}

// Good Example - Circuit Breaker Pattern
public class ExternalServiceCircuitBreaker {
    private static Integer failureCount = 0;
    private static final Integer FAILURE_THRESHOLD = 3;
    private static DateTime circuitOpenedTime;
    private static final Integer RETRY_TIMEOUT_MINUTES = 5;

    public static Boolean isCircuitOpen() {
        if (circuitOpenedTime != null) {
            // Check if retry timeout has passed
            if (DateTime.now() > circuitOpenedTime.addMinutes(RETRY_TIMEOUT_MINUTES)) {
                // Reset circuit
                failureCount = 0;
                circuitOpenedTime = null;
                return false;
            }
            return true;
        }
        return failureCount >= FAILURE_THRESHOLD;
    }

    public static void recordFailure() {
        failureCount++;
        if (failureCount >= FAILURE_THRESHOLD) {
            circuitOpenedTime = DateTime.now();
            System.debug(LoggingLevel.ERROR, 'Circuit breaker opened due to failures');
        }
    }

    public static void recordSuccess() {
        failureCount = 0;
        circuitOpenedTime = null;
    }

    public static HttpResponse makeCallout(String endpoint) {
        if (isCircuitOpen()) {
            throw new CircuitBreakerException('Circuit is open. Service unavailable.');
        }

        try {
            HttpRequest req = new HttpRequest();
            req.setEndpoint(endpoint);
            req.setMethod('GET');
            HttpResponse res = new Http().send(req);

            if (res.getStatusCode() == 200) {
                recordSuccess();
            } else {
                recordFailure();
            }
            return res;
        } catch (Exception e) {
            recordFailure();
            throw e;
        }
    }

    public class CircuitBreakerException extends Exception {}
}

// Good Example - Selector Pattern
public class AccountSelector {
    public static List<Account> selectByIds(Set<Id> accountIds) {
        return [
            SELECT Id, Name, AnnualRevenue, Rating
            FROM Account
            WHERE Id IN :accountIds
            WITH SECURITY_ENFORCED
        ];
    }

    public static List<Account> selectActiveAccountsWithContacts() {
        return [
            SELECT Id, Name, (SELECT Id, LastName FROM Contacts)
            FROM Account
            WHERE IsActive__c = true
            WITH SECURITY_ENFORCED
        ];
    }
}
```
###配置管理

####自定义元数据类型与自定义设置

—**对于可部署的配置数据，首选CMT (Custom Metadata Types)**。
—使用“**自定义设置**”用于用户特定或组织特定的数据，这些数据因环境而异。
CMT是可打包的，可部署的，并且可以在验证规则和公式中使用。
—自定义设置支持层次结构（组织、配置文件、用户），但不可部署。```apex
// Good Example - Using Custom Metadata Type
List<API_Configuration__mdt> configs = [
    SELECT Endpoint__c, Timeout__c, Max_Retries__c
    FROM API_Configuration__mdt
    WHERE DeveloperName = 'Production_API'
    LIMIT 1
];

if (!configs.isEmpty()) {
    String endpoint = configs[0].Endpoint__c;
    Integer timeout = Integer.valueOf(configs[0].Timeout__c);
}

// Good Example - Using Custom Settings (user-specific)
User_Preferences__c prefs = User_Preferences__c.getInstance(UserInfo.getUserId());
Boolean darkMode = prefs.Dark_Mode_Enabled__c;

// Good Example - Using Custom Settings (org-level)
Org_Settings__c orgSettings = Org_Settings__c.getOrgDefaults();
Integer maxRecords = Integer.valueOf(orgSettings.Max_Records_Per_Query__c);
```
####命名凭据和HTTP标注

- **始终使用命名凭据**外部API端点和身份验证。
避免在代码中硬编码url、令牌或凭据。
—使用`callout:NamedCredential`语法进行安全的、可部署的集成。
- **始终检查HTTP状态码**并优雅地处理错误。
—设置适当的超时时间，以防止长时间的调出。
-为Queueable和Batchable类使用`Database.AllowsCallouts`接口。```apex
// Good Example - Using Named Credentials
public class ExternalAPIService {
    private static final String NAMED_CREDENTIAL = 'callout:External_API';
    private static final Integer TIMEOUT_MS = 120000; // 120 seconds

    public static Map<String, Object> getExternalData(String recordId) {
        HttpRequest req = new HttpRequest();
        req.setEndpoint(NAMED_CREDENTIAL + '/api/records/' + recordId);
        req.setMethod('GET');
        req.setTimeout(TIMEOUT_MS);
        req.setHeader('Content-Type', 'application/json');

        try {
            Http http = new Http();
            HttpResponse res = http.send(req);

            if (res.getStatusCode() == 200) {
                return (Map<String, Object>) JSON.deserializeUntyped(res.getBody());
            } else if (res.getStatusCode() == 404) {
                throw new NotFoundException('Record not found: ' + recordId);
            } else if (res.getStatusCode() >= 500) {
                throw new ServiceUnavailableException('External service error: ' + res.getStatus());
            } else {
                throw new CalloutException('Unexpected response: ' + res.getStatusCode());
            }
        } catch (System.CalloutException e) {
            System.debug(LoggingLevel.ERROR, 'Callout failed: ' + e.getMessage());
            throw new ExternalAPIException('Failed to retrieve data', e);
        }
    }

    public class ExternalAPIException extends Exception {}
    public class NotFoundException extends Exception {}
    public class ServiceUnavailableException extends Exception {}
}

// Good Example - POST request with JSON body
public static String createExternalRecord(Map<String, Object> data) {
    HttpRequest req = new HttpRequest();
    req.setEndpoint(NAMED_CREDENTIAL + '/api/records');
    req.setMethod('POST');
    req.setTimeout(TIMEOUT_MS);
    req.setHeader('Content-Type', 'application/json');
    req.setBody(JSON.serialize(data));

    HttpResponse res = new Http().send(req);

    if (res.getStatusCode() == 201) {
        Map<String, Object> result = (Map<String, Object>) JSON.deserializeUntyped(res.getBody());
        return (String) result.get('id');
    } else {
        throw new CalloutException('Failed to create record: ' + res.getStatus());
    }
}
```
###通用注解

-`@AuraEnabled`-将方法暴露给闪电Web组件和Aura组件。
-`@AuraEnabled(cacheable=true)`-启用只读方法的客户端缓存。
-`@InvocableMethod`-使方法可以从流和过程生成器中调用。
为可调用的方法定义`@InvocableVariable`参数。
-`@TestVisible`-只向测试类公开私有成员。
-`@SuppressWarnings('PMD.RuleName')`-抑制特定的PMD警告。
-`@RemoteAction`-为Visualforce JavaScript远程操作（遗留）暴露方法。
-`@Future`异步执行方法。
-`@Future(callout=true)`-在未来的方法中允许HTTP调出。```apex
// Good Example - AuraEnabled for LWC
public with sharing class AccountController {
    @AuraEnabled(cacheable=true)
    public static List<Account> getAccounts() {
        return [SELECT Id, Name FROM Account WITH SECURITY_ENFORCED LIMIT 10];
    }

    @AuraEnabled
    public static void updateAccount(Id accountId, String newName) {
        Account acc = new Account(Id = accountId, Name = newName);
        update acc;
    }
}

// Good Example - InvocableMethod for Flow
public class FlowActions {
    @InvocableMethod(label='Send Email Notification' description='Sends email to account owner')
    public static List<Result> sendNotification(List<Request> requests) {
        List<Result> results = new List<Result>();

        for (Request req : requests) {
            Result result = new Result();
            try {
                // Send email logic
                result.success = true;
                result.message = 'Email sent successfully';
            } catch (Exception e) {
                result.success = false;
                result.message = e.getMessage();
            }
            results.add(result);
        }
        return results;
    }

    public class Request {
        @InvocableVariable(required=true label='Account ID')
        public Id accountId;

        @InvocableVariable(label='Email Template')
        public String templateName;
    }

    public class Result {
        @InvocableVariable
        public Boolean success;

        @InvocableVariable
        public String message;
    }
}

// Good Example - TestVisible for testing private methods
public class AccountService {
    @TestVisible
    private static Boolean validateAccountName(String name) {
        return String.isNotBlank(name) && name.length() > 3;
    }
}
```
异步Apex-使用**@future**方法进行简单的异步操作和调用。
-使用**Queueable Apex**复杂的异步操作，需要链接。
—使用**Batch Apex**处理大数据量（>50,000条记录）。
-使用`Database.Stateful`来维护跨批处理执行的状态（例如，计数器，聚合）。
—如果没有`Database.Stateful`，批处理类是无状态的，并且实例变量在批处理之间重置。
-使用有状态批时要注意调控器限制。
-使用**Scheduled Apex**进行经常性操作。
-创建一个单独的**可调度类**来调度批处理作业。
-永远不要在同一个类中实现`Database.Batchable`和`Schedulable`。
-使用平台事件进行事件驱动架构和解耦集成。
-使用`EventBus.publish()`发布事件，用于异步、即发即弃的通信。
-使用平台事件对象上的触发器订阅事件。
-非常适合集成、微服务和跨组织通信。
- **优化批大小**基于处理复杂性和总督的限制。
—批量大小默认为200，可在1 ~ 2000之间进行调整。
-小批量（50-100）的复杂加工或标注。
-批量（200）进行简单的DML操作。
-测试实际的数据量，以找到最佳的大小。```apex
// Good Example - Platform Events for decoupled communication
public class OrderEventPublisher {
    public static void publishOrderCreated(List<Order> orders) {
        List<Order_Created__e> events = new List<Order_Created__e>();

        for (Order ord : orders) {
            Order_Created__e event = new Order_Created__e(
                Order_Id__c = ord.Id,
                Order_Amount__c = ord.TotalAmount,
                Customer_Id__c = ord.AccountId
            );
            events.add(event);
        }

        // Publish events
        List<Database.SaveResult> results = EventBus.publish(events);

        // Check for errors
        for (Database.SaveResult result : results) {
            if (!result.isSuccess()) {
                for (Database.Error error : result.getErrors()) {
                    System.debug('Error publishing event: ' + error.getMessage());
                }
            }
        }
    }
}

// Good Example - Platform Event Trigger (Subscriber)
trigger OrderCreatedTrigger on Order_Created__e (after insert) {
    List<Task> tasksToCreate = new List<Task>();

    for (Order_Created__e event : Trigger.new) {
        Task t = new Task(
            Subject = 'Follow up on order',
            WhatId = event.Order_Id__c,
            Priority = 'High'
        );
        tasksToCreate.add(t);
    }

    if (!tasksToCreate.isEmpty()) {
        insert tasksToCreate;
    }
}

// Good Example - Batch size optimization based on complexity
public class ComplexProcessingBatch implements Database.Batchable<SObject>, Database.AllowsCallouts {
    public Database.QueryLocator start(Database.BatchableContext bc) {
        return Database.getQueryLocator([
            SELECT Id, Name FROM Account WHERE IsActive__c = true
        ]);
    }

    public void execute(Database.BatchableContext bc, List<Account> scope) {
        // Complex processing with callouts - use smaller batch size
        for (Account acc : scope) {
            // Make HTTP callout
            HttpResponse res = ExternalAPIService.getAccountData(acc.Id);
            // Process response
        }
    }

    public void finish(Database.BatchableContext bc) {
        System.debug('Batch completed');
    }
}

// Execute with smaller batch size for callout-heavy processing
Database.executeBatch(new ComplexProcessingBatch(), 50);

// Good Example - Simple DML batch with default size
public class SimpleDMLBatch implements Database.Batchable<SObject> {
    public Database.QueryLocator start(Database.BatchableContext bc) {
        return Database.getQueryLocator([
            SELECT Id, Status__c FROM Order WHERE Status__c = 'Draft'
        ]);
    }

    public void execute(Database.BatchableContext bc, List<Order> scope) {
        for (Order ord : scope) {
            ord.Status__c = 'Pending';
        }
        update scope;
    }

    public void finish(Database.BatchableContext bc) {
        System.debug('Batch completed');
    }
}

// Execute with larger batch size for simple DML
Database.executeBatch(new SimpleDMLBatch(), 200);

// Good Example - Queueable Apex
public class EmailNotificationQueueable implements Queueable, Database.AllowsCallouts {
    private List<Id> accountIds;

    public EmailNotificationQueueable(List<Id> accountIds) {
        this.accountIds = accountIds;
    }

    public void execute(QueueableContext context) {
        List<Account> accounts = [SELECT Id, Name, Email__c FROM Account WHERE Id IN :accountIds];

        for (Account acc : accounts) {
            sendEmail(acc);
        }

        // Chain another job if needed
        if (hasMoreWork()) {
            System.enqueueJob(new AnotherQueueable());
        }
    }

    private void sendEmail(Account acc) {
        // Email sending logic
    }

    private Boolean hasMoreWork() {
        return false;
    }
}

// Good Example - Stateless Batch Apex (default)
public class AccountCleanupBatch implements Database.Batchable<SObject> {
    public Database.QueryLocator start(Database.BatchableContext bc) {
        return Database.getQueryLocator([
            SELECT Id, Name FROM Account WHERE LastActivityDate < LAST_N_DAYS:365
        ]);
    }

    public void execute(Database.BatchableContext bc, List<Account> scope) {
        delete scope;
    }

    public void finish(Database.BatchableContext bc) {
        System.debug('Batch completed');
    }
}

// Good Example - Stateful Batch Apex (maintains state across batches)
public class AccountStatsBatch implements Database.Batchable<SObject>, Database.Stateful {
    private Integer recordsProcessed = 0;
    private Integer totalRevenue = 0;

    public Database.QueryLocator start(Database.BatchableContext bc) {
        return Database.getQueryLocator([
            SELECT Id, Name, AnnualRevenue FROM Account WHERE IsActive__c = true
        ]);
    }

    public void execute(Database.BatchableContext bc, List<Account> scope) {
        for (Account acc : scope) {
            recordsProcessed++;
            totalRevenue += (Integer) acc.AnnualRevenue;
        }
    }

    public void finish(Database.BatchableContext bc) {
        // State is maintained: recordsProcessed and totalRevenue retain their values
        System.debug('Total records processed: ' + recordsProcessed);
        System.debug('Total revenue: ' + totalRevenue);

        // Send summary email or create summary record
    }
}

// Good Example - Schedulable class to schedule a batch
public class AccountCleanupScheduler implements Schedulable {
    public void execute(SchedulableContext sc) {
        // Execute the batch with batch size of 200
        Database.executeBatch(new AccountCleanupBatch(), 200);
    }
}

// Schedule the batch to run daily at 2 AM
// Execute this in Anonymous Apex or in setup code:
// String cronExp = '0 0 2 * * ?';
// System.schedule('Daily Account Cleanup', cronExp, new AccountCleanupScheduler());
```
# #测试

**生产代码始终达到100%的代码覆盖率**（最低要求75%）。
编写有意义的测试来验证业务逻辑，而不仅仅是代码覆盖率。
-使用`@TestSetup`方法创建跨测试方法共享的测试数据。
—使用`Test.startTest()`和`Test.stopTest()`重置调控器限制。
-测试**阳性场景**，**阴性场景**，**批量场景**（200+记录）。
-使用`System.runAs()`测试不同的用户上下文和权限。
-模拟外部callouts使用`Test.setMock()`。
-永远不要使用`@SeeAllData=true`-总是在测试中创建测试数据。
- **使用`Assert`类方法**来代替已弃用的`System.assert*()`方法。
-始终在断言中添加描述性失败消息以保持清晰。```apex
// Good Example - Comprehensive test class
@IsTest
private class AccountServiceTest {
    @TestSetup
    static void setupTestData() {
        List<Account> accounts = new List<Account>();
        for (Integer i = 0; i < 200; i++) {
            accounts.add(new Account(
                Name = 'Test Account ' + i,
                AnnualRevenue = i * 10000
            ));
        }
        insert accounts;
    }

    @IsTest
    static void testUpdateAccountRatings_Positive() {
        // Arrange
        List<Account> accounts = [SELECT Id FROM Account];
        Set<Id> accountIds = new Map<Id, Account>(accounts).keySet();

        // Act
        Test.startTest();
        AccountService.updateAccountRatings(accountIds);
        Test.stopTest();

        // Assert
        List<Account> updatedAccounts = [
            SELECT Id, Rating FROM Account WHERE AnnualRevenue > 1000000
        ];
        for (Account acc : updatedAccounts) {
            Assert.areEqual('Hot', acc.Rating, 'Rating should be Hot for high revenue accounts');
        }
    }

    @IsTest
    static void testUpdateAccountRatings_NoAccess() {
        // Create user with limited access
        User testUser = createTestUser();

        List<Account> accounts = [SELECT Id FROM Account LIMIT 1];
        Set<Id> accountIds = new Map<Id, Account>(accounts).keySet();

        Test.startTest();
        System.runAs(testUser) {
            try {
                AccountService.updateAccountRatings(accountIds);
                Assert.fail('Expected SecurityException');
            } catch (SecurityException e) {
                Assert.isTrue(true, 'SecurityException thrown as expected');
            }
        }
        Test.stopTest();
    }

    @IsTest
    static void testBulkOperation() {
        List<Account> accounts = [SELECT Id FROM Account];
        Set<Id> accountIds = new Map<Id, Account>(accounts).keySet();

        Test.startTest();
        AccountService.updateAccountRatings(accountIds);
        Test.stopTest();

        List<Account> updatedAccounts = [SELECT Id, Rating FROM Account];
        Assert.areEqual(200, updatedAccounts.size(), 'All accounts should be processed');
    }

    private static User createTestUser() {
        Profile p = [SELECT Id FROM Profile WHERE Name = 'Standard User' LIMIT 1];
        return new User(
            Alias = 'testuser',
            Email = 'testuser@test.com',
            EmailEncodingKey = 'UTF-8',
            LastName = 'Testing',
            LanguageLocaleKey = 'en_US',
            LocaleSidKey = 'en_US',
            ProfileId = p.Id,
            TimeZoneSidKey = 'America/Los_Angeles',
            UserName = 'testuser' + DateTime.now().getTime() + '@test.com'
        );
    }
}
```
常见的代码气味和反模式

- **DML/SOQLin loops** -总是将你的代码膨胀以避免调控器限制异常。
- **硬编码id ** -使用自定义设置，自定义元数据，或动态查询代替。
- **深度嵌套条件** -提取逻辑到单独的方法清晰。
- **大型方法** -让方法专注于单个职责（最多30-50行）。
- **魔术数字** -使用命名常量为清晰度和可维护性。
- **重复代码** -将公共逻辑提取到可重用的方法或类中。
- **缺少空检查** -始终验证输入参数和查询结果。```apex
// Bad Example - DML in loop
for (Account acc : accounts) {
    acc.Rating = 'Hot';
    update acc; // AVOID: DML in loop
}

// Good Example - Bulkified DML
for (Account acc : accounts) {
    acc.Rating = 'Hot';
}
update accounts;

// Bad Example - Hardcoded ID
Account acc = [SELECT Id FROM Account WHERE Id = '001000000000001'];

// Good Example - Dynamic query
Account acc = [SELECT Id FROM Account WHERE Name = :accountName LIMIT 1];

// Bad Example - Magic number
if (accounts.size() > 200) {
    // Process
}

// Good Example - Named constant
private static final Integer MAX_BATCH_SIZE = 200;
if (accounts.size() > MAX_BATCH_SIZE) {
    // Process
}
```
文档和注释

对类和方法使用javadoc风格的注释。
-包括`@author`和`@date`标签跟踪。
-包括`@description`，`@param`，`@return`和`@throws`标签。
-包括`@param`，`@return`，和`@throws`标签**仅**适用时。
-不要将`@return void`用于不返回任何结果的方法。
-记录复杂的业务逻辑和设计决策。
-保持注释与代码更改同步。```apex
/**
 * @author Your Name
 * @date 2025-01-01
 * @description Service class for managing Account records
 */
public with sharing class AccountService {

    /**
     * @author Your Name
     * @date 2025-01-01
     * @description Updates the rating for accounts based on annual revenue
     * @param accountIds Set of Account IDs to update
     * @throws AccountServiceException if user lacks update permissions
     */
    public static void updateAccountRatings(Set<Id> accountIds) {
        // Implementation
    }
}
```
##部署和DevOps

-使用**Salesforce CLI**进行源代码驱动开发。
-利用scratch组织进行开发和测试。
-使用Salesforce CLI、GitHub Actions或Jenkins等工具实现CI/CD管道。
-使用**解锁包**进行模块化部署。
-运行**Apex测试**作为部署验证的一部分。
-使用Salesforce代码分析器对代码进行质量和安全问题扫描。```bash
# Salesforce CLI commands (sf)
sf project deploy start                    # Deploy source to org
sf project deploy start --dry-run          # Validate deployment without deploying
sf apex run test --test-level RunLocalTests # Run local Apex tests
sf apex get test --test-run-id <id>        # Get test results
sf project retrieve start                  # Retrieve source from org

# Salesforce Code Analyzer commands
sf code-analyzer rules                     # List all available rules
sf code-analyzer rules --rule-selector eslint:Recommended  # List recommended ESLint rules
sf code-analyzer rules --workspace ./force-app             # List rules for specific workspace
sf code-analyzer run                       # Run analysis with recommended rules
sf code-analyzer run --rule-selector pmd:Recommended       # Run PMD recommended rules
sf code-analyzer run --rule-selector "Security"           # Run rules with Security tag
sf code-analyzer run --workspace ./force-app --target "**/*.cls"  # Analyze Apex classes
sf code-analyzer run --severity-threshold 3               # Run analysis with severity threshold
sf code-analyzer run --output-file results.html           # Output results to HTML file
sf code-analyzer run --output-file results.csv            # Output results to CSV file
sf code-analyzer run --view detail                        # Show detailed violation information
```
性能优化

—使用**选择性SOQL查询**索引字段。
-实现**延迟加载**昂贵的操作。
—对长时间运行的操作使用异步处理。
-监控**调试日志**和**事件监控**。
-使用**ApexGuru**和**Scale Center**进行性能洞察。

###平台缓存

—使用**平台缓存**存储访问频繁的数据，减少SOQL查询。
-`Cache.OrgPartition`-在组织中的所有用户和会话共享。
-`Cache.SessionPartition`指定用户的会话。
—采用合适的缓存失效策略。
-通过回退数据库查询优雅地处理缓存丢失。```apex
// Good Example - Using Org Cache
public class AccountCacheService {
    private static final String CACHE_PARTITION = 'local.AccountCache';
    private static final Integer TTL_SECONDS = 3600; // 1 hour

    public static Account getAccount(Id accountId) {
        Cache.OrgPartition orgPart = Cache.Org.getPartition(CACHE_PARTITION);
        String cacheKey = 'Account_' + accountId;

        // Try to get from cache
        Account acc = (Account) orgPart.get(cacheKey);

        if (acc == null) {
            // Cache miss - query database
            acc = [
                SELECT Id, Name, Industry, AnnualRevenue
                FROM Account
                WHERE Id = :accountId
                LIMIT 1
            ];

            // Store in cache with TTL
            orgPart.put(cacheKey, acc, TTL_SECONDS);
        }

        return acc;
    }

    public static void invalidateCache(Id accountId) {
        Cache.OrgPartition orgPart = Cache.Org.getPartition(CACHE_PARTITION);
        String cacheKey = 'Account_' + accountId;
        orgPart.remove(cacheKey);
    }
}

// Good Example - Using Session Cache
public class UserPreferenceCache {
    private static final String CACHE_PARTITION = 'local.UserPrefs';

    public static Map<String, Object> getUserPreferences() {
        Cache.SessionPartition sessionPart = Cache.Session.getPartition(CACHE_PARTITION);
        String cacheKey = 'UserPrefs_' + UserInfo.getUserId();

        Map<String, Object> prefs = (Map<String, Object>) sessionPart.get(cacheKey);

        if (prefs == null) {
            // Load preferences from database or custom settings
            prefs = new Map<String, Object>{
                'theme' => 'dark',
                'language' => 'en_US'
            };
            sessionPart.put(cacheKey, prefs);
        }

        return prefs;
    }
}
```
构建和验证

-在添加或修改代码后，验证项目是否继续成功构建。
-运行所有相关的Apex测试类，以确保没有回归。
—使用Salesforce命令行：`sf apex run test --test-level RunLocalTests`确保代码覆盖率达到75%的最低要求（目标是100%）。
-使用Salesforce代码分析器检查代码质量问题：`sf code-analyzer run --severity-threshold 2`-审查违规行为并在部署前解决。