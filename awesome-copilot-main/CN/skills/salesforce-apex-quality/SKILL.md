---
name: salesforce-apex-quality
description: 'Apex code quality guardrails for Salesforce development. Enforces bulk-safety rules (no SOQL/DML in loops), sharing model requirements, CRUD/FLS security, SOQL injection prevention, PNB test coverage (Positive / Negative / Bulk), and modern Apex idioms. Use this skill when reviewing or generating Apex classes, trigger handlers, batch jobs, or test classes to catch governor limit risks, security gaps, and quality issues before deployment.'
---
# Salesforce Apex质量护栏

将这些检查应用于您编写或审查的每个Apex类、触发器和测试文件。

##步骤1 -总督限制安全检查

在声明任何可接受的Apex文件之前，先扫描这些模式：

SOQL和DML在循环中-自动失败```apex
// ❌ NEVER — causes LimitException at scale
for (Account a : accounts) {
    List<Contact> contacts = [SELECT Id FROM Contact WHERE AccountId = :a.Id]; // SOQL in loop
    update a; // DML in loop
}

// ✅ ALWAYS — collect, then query/update once
Set<Id> accountIds = new Map<Id, Account>(accounts).keySet();
Map<Id, List<Contact>> contactsByAccount = new Map<Id, List<Contact>>();
for (Contact c : [SELECT Id, AccountId FROM Contact WHERE AccountId IN :accountIds]) {
    if (!contactsByAccount.containsKey(c.AccountId)) {
        contactsByAccount.put(c.AccountId, new List<Contact>());
    }
    contactsByAccount.get(c.AccountId).add(c);
}
update accounts; // DML once, outside the loop
```
规则：如果在`for`循环体中看到`[SELECT`或`Database.query`、`insert`、`update`、`delete`、`upsert`、`merge`，请停止并在继续之前进行重构。

##步骤2 -共享模型验证

每个类都必须显式声明其共享意图。未声明的共享继承自调用者——不可预测的行为。

|何时使用||---|---|
|`public with sharing class Foo`|所有服务、处理程序、选择器和控制器类的默认值|
|`public without sharing class Foo`|仅当类必须高架运行时（例如，系统级日志记录，触发旁路）。需要一个代码注释来解释原因。|
|`public inherited sharing class Foo`|应该尊重调用者的共享上下文的框架入口点|

如果一个类没有这三个声明中的任何一个，在编写其他内容之前添加它。

##步骤3 - CRUD / FLS强制执行

代表用户读写记录的Apex代码必须验证对象和字段访问。平台不会在Apex中自动执行FLS或CRUD。```apex
// Check before querying a field
if (!Schema.sObjectType.Contact.fields.Email.isAccessible()) {
    throw new System.NoAccessException();
}

// Or use WITH USER_MODE in SOQL (API 56.0+)
List<Contact> contacts = [SELECT Id, Email FROM Contact WHERE AccountId = :accId WITH USER_MODE];

// Or use Database.query with AccessLevel
List<Contact> contacts = Database.query('SELECT Id, Email FROM Contact', AccessLevel.USER_MODE);
```
规则：任何可从UI组件、REST端点或`@InvocableMethod`调用的Apex方法**必须**强制执行CRUD/FLS.。仅从可信上下文调用的内部服务方法可以使用`with sharing`。

##步骤4 -防止SOQL注入```apex
// ❌ NEVER — concatenates user input into SOQL string
String soql = 'SELECT Id FROM Account WHERE Name = \'' + userInput + '\'';

// ✅ ALWAYS — bind variable
String soql = [SELECT Id FROM Account WHERE Name = :userInput];

// ✅ For dynamic SOQL with user-controlled field names — validate against a whitelist
Set<String> allowedFields = new Set<String>{'Name', 'Industry', 'AnnualRevenue'};
if (!allowedFields.contains(userInput)) {
    throw new IllegalArgumentException('Field not permitted: ' + userInput);
}
```
##第5步-现代顶点习语

首选当前语言特性（API 62.0 / Winter '25+）：

|旧模式|现代替代||---|---|
|`if (obj != null) { x = obj.Field__c; }`|`x = obj?.Field__c;`|
|`x = (y != null) ? y : defaultVal;`|`x = y ?? defaultVal;`|
|`System.assertEquals(expected, actual)`|`Assert.areEqual(expected, actual)`|
|`System.assert(condition)`|`Assert.isTrue(condition)`|
|`[SELECT ... WHERE ...]`没有共享上下文|`[SELECT ... WHERE ... WITH USER_MODE]`|

##步骤6 - PNB测试覆盖检查表

每个功能都必须在所有三个路径上进行测试。缺少其中任何一个都是质量的失败：

###积极的道路
—期望输入→期望输出。
-断言确切的字段值，记录计数或返回值-不仅仅是没有抛出异常。

消极路径
—无效输入、空值、空集合和错误条件。
-断言抛出的异常具有正确的类型和消息。
-当操作应该干净地失败时，断言没有记录发生突变。###批量路径
-Insert/update/delete** 200-251条记录**在单个测试事务。
-断言所有的记录处理正确-没有部分失败从总督的限制。
—使用`Test.startTest()`/`Test.stopTest()`隔离异步工作的调控器限制计数器。

测试类规则```apex
@isTest(SeeAllData=false)   // Required — no exceptions without a documented reason
private class AccountServiceTest {

    @TestSetup
    static void makeData() {
        // Create all test data here — use a factory if one exists in the project
    }

    @isTest
    static void givenValidInput_whenProcessAccounts_thenFieldsUpdated() {
        // Positive path
        List<Account> accounts = [SELECT Id FROM Account LIMIT 10];
        Test.startTest();
        AccountService.processAccounts(accounts);
        Test.stopTest();
        // Assert meaningful outcomes — not just no exception
        List<Account> updated = [SELECT Status__c FROM Account WHERE Id IN :accounts];
        Assert.areEqual('Processed', updated[0].Status__c, 'Status should be Processed');
    }
}
```
##步骤7 -触发架构检查表

-[]每个对象触发一次。如果存在第二个触发器，则合并到处理程序中。
-[]触发器主体只包含：上下文检查、处理程序调用和路由逻辑。
-[]触发器体中没有直接的业务逻辑、SOQL或DML。
-[]如果一个触发器框架（触发器动作框架，off -apex-common，自定义基类）已经在使用-扩展它。不要创建平行模式。
-[]处理程序类为`with sharing`，除非触发器需要提升访问权限。

快速参考-硬编码反模式摘要

|模式|动作||---|---|
| SOQL inside`for`循环|重构：在循环前查询，在集合上操作|
| DML内`for`循环|重构：收集突变，DML一次后循环|
|添加`with sharing`（或说明为什么`without sharing`） |
|`escape="false"`对用户数据（VF） |删除-自动转义强制XSS防护|
|空`catch`块|添加日志记录和适当的重新抛出或错误处理|
|带用户输入的字符串连接SOQL |替换为绑定变量或白名单验证|
|添加一个有意义的`Assert.*`调用|
|`System.assert`/`System.assertEquals`style |升级为`Assert.isTrue`/`Assert.areEqual`|
|硬编码记录ID (`'001...'`) |替换为查询或插入的测试记录ID |