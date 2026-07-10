---
name: refactor
description: 'Surgical code refactoring to improve maintainability without changing behavior. Covers extracting functions, renaming variables, breaking down god functions, improving type safety, eliminating code smells, and applying design patterns. Less drastic than repo-rebuilder; use for gradual improvements.'
license: MIT
---
#重构

# #概述

在不改变外部行为的情况下改进代码结构和可读性。重构是渐进的进化，而不是革命。使用它来改进现有的代码，而不是从头开始重写。

##何时使用

在以下情况下使用此技能：

-代码难以理解或维护
-Functions/classes太大
代码气味需要寻址
-由于代码结构的原因，添加功能很困难
-用户要求“清理这段代码”，“重构这段代码”，“改进这段代码”

---

##重构原则

黄金法则1. **行为被保留**——重构不会改变代码做什么，只会改变如何做
2. **小步骤** -做微小的改变，每次改变后进行测试
3. **版本控制是你的朋友** -在每个安全状态之前和之后提交
4. **测试是必不可少的**——没有测试，你就不是在重构，而是在编辑
5. **一次只做一件事** -不要把重构和特性变更混在一起

什么时候不要重构```
- Code that works and won't change again (if it ain't broke...)
- Critical production code without tests (add tests first)
- When you're under a tight deadline
- "Just because" - need a clear purpose
```
---

##常见代码气味和修复

# # # 1。长Method/Function```diff
# BAD: 200-line function that does everything
- async function processOrder(orderId) {
-   // 50 lines: fetch order
-   // 30 lines: validate order
-   // 40 lines: calculate pricing
-   // 30 lines: update inventory
-   // 20 lines: create shipment
-   // 30 lines: send notifications
- }

# GOOD: Broken into focused functions
+ async function processOrder(orderId) {
+   const order = await fetchOrder(orderId);
+   validateOrder(order);
+   const pricing = calculatePricing(order);
+   await updateInventory(order);
+   const shipment = await createShipment(order);
+   await sendNotifications(order, pricing, shipment);
+   return { order, pricing, shipment };
+ }
```
# # # 2。重复的代码```diff
# BAD: Same logic in multiple places
- function calculateUserDiscount(user) {
-   if (user.membership === 'gold') return user.total * 0.2;
-   if (user.membership === 'silver') return user.total * 0.1;
-   return 0;
- }
-
- function calculateOrderDiscount(order) {
-   if (order.user.membership === 'gold') return order.total * 0.2;
-   if (order.user.membership === 'silver') return order.total * 0.1;
-   return 0;
- }

# GOOD: Extract common logic
+ function getMembershipDiscountRate(membership) {
+   const rates = { gold: 0.2, silver: 0.1 };
+   return rates[membership] || 0;
+ }
+
+ function calculateUserDiscount(user) {
+   return user.total * getMembershipDiscountRate(user.membership);
+ }
+
+ function calculateOrderDiscount(order) {
+   return order.total * getMembershipDiscountRate(order.user.membership);
+ }
```
# # # 3。大型Class/Module```diff
# BAD: God object that knows too much
- class UserManager {
-   createUser() { /* ... */ }
-   updateUser() { /* ... */ }
-   deleteUser() { /* ... */ }
-   sendEmail() { /* ... */ }
-   generateReport() { /* ... */ }
-   handlePayment() { /* ... */ }
-   validateAddress() { /* ... */ }
-   // 50 more methods...
- }

# GOOD: Single responsibility per class
+ class UserService {
+   create(data) { /* ... */ }
+   update(id, data) { /* ... */ }
+   delete(id) { /* ... */ }
+ }
+
+ class EmailService {
+   send(to, subject, body) { /* ... */ }
+ }
+
+ class ReportService {
+   generate(type, params) { /* ... */ }
+ }
+
+ class PaymentService {
+   process(amount, method) { /* ... */ }
+ }
```
# # # 4。长参数列表```diff
# BAD: Too many parameters
- function createUser(email, password, name, age, address, city, country, phone) {
-   /* ... */
- }

# GOOD: Group related parameters
+ interface UserData {
+   email: string;
+   password: string;
+   name: string;
+   age?: number;
+   address?: Address;
+   phone?: string;
+ }
+
+ function createUser(data: UserData) {
+   /* ... */
+ }

# EVEN BETTER: Use builder pattern for complex construction
+ const user = UserBuilder
+   .email('test@example.com')
+   .password('secure123')
+   .name('Test User')
+   .address(address)
+   .build();
```
# # # 5。功能嫉妒```diff
# BAD: Method that uses another object's data more than its own
- class Order {
-   calculateDiscount(user) {
-     if (user.membershipLevel === 'gold') {
+       return this.total * 0.2;
+     }
+     if (user.accountAge > 365) {
+       return this.total * 0.1;
+     }
+     return 0;
+   }
+ }

# GOOD: Move logic to the object that owns the data
+ class User {
+   getDiscountRate(orderTotal) {
+     if (this.membershipLevel === 'gold') return 0.2;
+     if (this.accountAge > 365) return 0.1;
+     return 0;
+   }
+ }
+
+ class Order {
+   calculateDiscount(user) {
+     return this.total * user.getDiscountRate(this.total);
+   }
+ }
```
# # # 6。原始的痴迷```diff
# BAD: Using primitives for domain concepts
- function sendEmail(to, subject, body) { /* ... */ }
- sendEmail('user@example.com', 'Hello', '...');

- function createPhone(country, number) {
-   return `${country}-${number}`;
- }

# GOOD: Use domain types
+ class Email {
+   private constructor(public readonly value: string) {
+     if (!Email.isValid(value)) throw new Error('Invalid email');
+   }
+   static create(value: string) { return new Email(value); }
+   static isValid(email: string) { return /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email); }
+ }
+
+ class PhoneNumber {
+   constructor(
+     public readonly country: string,
+     public readonly number: string
+   ) {
+     if (!PhoneNumber.isValid(country, number)) throw new Error('Invalid phone');
+   }
+   toString() { return `${this.country}-${this.number}`; }
+   static isValid(country: string, number: string) { /* ... */ }
+ }
+
+ // Usage
+ const email = Email.create('user@example.com');
+ const phone = new PhoneNumber('1', '555-1234');
```
# # # 7。魔法Numbers/Strings```diff
# BAD: Unexplained values
- if (user.status === 2) { /* ... */ }
- const discount = total * 0.15;
- setTimeout(callback, 86400000);

# GOOD: Named constants
+ const UserStatus = {
+   ACTIVE: 1,
+   INACTIVE: 2,
+   SUSPENDED: 3
+ } as const;
+
+ const DISCOUNT_RATES = {
+   STANDARD: 0.1,
+   PREMIUM: 0.15,
+   VIP: 0.2
+ } as const;
+
+ const ONE_DAY_MS = 24 * 60 * 60 * 1000;
+
+ if (user.status === UserStatus.INACTIVE) { /* ... */ }
+ const discount = total * DISCOUNT_RATES.PREMIUM;
+ setTimeout(callback, ONE_DAY_MS);
```
# # # 8。嵌套条件```diff
# BAD: Arrow code
- function process(order) {
-   if (order) {
-     if (order.user) {
-       if (order.user.isActive) {
-         if (order.total > 0) {
-           return processOrder(order);
+         } else {
+           return { error: 'Invalid total' };
+         }
+       } else {
+         return { error: 'User inactive' };
+       }
+     } else {
+       return { error: 'No user' };
+     }
+   } else {
+     return { error: 'No order' };
+   }
+ }

# GOOD: Guard clauses / early returns
+ function process(order) {
+   if (!order) return { error: 'No order' };
+   if (!order.user) return { error: 'No user' };
+   if (!order.user.isActive) return { error: 'User inactive' };
+   if (order.total <= 0) return { error: 'Invalid total' };
+   return processOrder(order);
+ }

# EVEN BETTER: Using Result type
+ function process(order): Result<ProcessedOrder, Error> {
+   return Result.combine([
+     validateOrderExists(order),
+     validateUserExists(order),
+     validateUserActive(order.user),
+     validateOrderTotal(order)
+   ]).flatMap(() => processOrder(order));
+ }
```
# # # 9。死代码```diff
# BAD: Unused code lingers
- function oldImplementation() { /* ... */ }
- const DEPRECATED_VALUE = 5;
- import { unusedThing } from './somewhere';
- // Commented out code
- // function oldCode() { /* ... */ }

# GOOD: Remove it
+ // Delete unused functions, imports, and commented code
+ // If you need it again, git history has it
```
# # # 10。不合适的亲密```diff
# BAD: One class reaches deep into another
- class OrderProcessor {
-   process(order) {
-     order.user.profile.address.street;  // Too intimate
-     order.repository.connection.config;  // Breaking encapsulation
+   }
+ }

# GOOD: Ask, don't tell
+ class OrderProcessor {
+   process(order) {
+     order.getShippingAddress();  // Order knows how to get it
+     order.save();  // Order knows how to save itself
+   }
+ }
```
---

提取方法重构

###前后```diff
# Before: One long function
- function printReport(users) {
-   console.log('USER REPORT');
-   console.log('============');
-   console.log('');
-   console.log(`Total users: ${users.length}`);
-   console.log('');
-   console.log('ACTIVE USERS');
-   console.log('------------');
-   const active = users.filter(u => u.isActive);
-   active.forEach(u => {
-     console.log(`- ${u.name} (${u.email})`);
-   });
-   console.log('');
-   console.log(`Active: ${active.length}`);
-   console.log('');
-   console.log('INACTIVE USERS');
-   console.log('--------------');
-   const inactive = users.filter(u => !u.isActive);
-   inactive.forEach(u => {
-     console.log(`- ${u.name} (${u.email})`);
-   });
-   console.log('');
-   console.log(`Inactive: ${inactive.length}`);
- }

# After: Extracted methods
+ function printReport(users) {
+   printHeader('USER REPORT');
+   console.log(`Total users: ${users.length}\n`);
+   printUserSection('ACTIVE USERS', users.filter(u => u.isActive));
+   printUserSection('INACTIVE USERS', users.filter(u => !u.isActive));
+ }
+
+ function printHeader(title) {
+   const line = '='.repeat(title.length);
+   console.log(title);
+   console.log(line);
+   console.log('');
+ }
+
+ function printUserSection(title, users) {
+   console.log(title);
+   console.log('-'.repeat(title.length));
+   users.forEach(u => console.log(`- ${u.name} (${u.email})`));
+   console.log('');
+   console.log(`${title.split(' ')[0]}: ${users.length}`);
+   console.log('');
+ }
```
---

##介绍类型安全

从未类型化到类型化```diff
# Before: No types
- function calculateDiscount(user, total, membership, date) {
-   if (membership === 'gold' && date.getDay() === 5) {
-     return total * 0.25;
-   }
-   if (membership === 'gold') return total * 0.2;
-   return total * 0.1;
- }

# After: Full type safety
+ type Membership = 'bronze' | 'silver' | 'gold';
+
+ interface User {
+   id: string;
+   name: string;
+   membership: Membership;
+ }
+
+ interface DiscountResult {
+   original: number;
+   discount: number;
+   final: number;
+   rate: number;
+ }
+
+ function calculateDiscount(
+   user: User,
+   total: number,
+   date: Date = new Date()
+ ): DiscountResult {
+   if (total < 0) throw new Error('Total cannot be negative');
+
+   let rate = 0.1; // Default bronze
+
+   if (user.membership === 'gold' && date.getDay() === 5) {
+     rate = 0.25; // Friday bonus for gold
+   } else if (user.membership === 'gold') {
+     rate = 0.2;
+   } else if (user.membership === 'silver') {
+     rate = 0.15;
+   }
+
+   const discount = total * rate;
+
+   return {
+     original: total,
+     discount,
+     final: total - discount,
+     rate
+   };
+ }
```
---

设计重构模式

策略模式```diff
# Before: Conditional logic
- function calculateShipping(order, method) {
-   if (method === 'standard') {
-     return order.total > 50 ? 0 : 5.99;
-   } else if (method === 'express') {
-     return order.total > 100 ? 9.99 : 14.99;
+   } else if (method === 'overnight') {
+     return 29.99;
+   }
+ }

# After: Strategy pattern
+ interface ShippingStrategy {
+   calculate(order: Order): number;
+ }
+
+ class StandardShipping implements ShippingStrategy {
+   calculate(order: Order) {
+     return order.total > 50 ? 0 : 5.99;
+   }
+ }
+
+ class ExpressShipping implements ShippingStrategy {
+   calculate(order: Order) {
+     return order.total > 100 ? 9.99 : 14.99;
+   }
+ }
+
+ class OvernightShipping implements ShippingStrategy {
+   calculate(order: Order) {
+     return 29.99;
+   }
+ }
+
+ function calculateShipping(order: Order, strategy: ShippingStrategy) {
+   return strategy.calculate(order);
+ }
```
责任链```diff
# Before: Nested validation
- function validate(user) {
-   const errors = [];
-   if (!user.email) errors.push('Email required');
+   else if (!isValidEmail(user.email)) errors.push('Invalid email');
+   if (!user.name) errors.push('Name required');
+   if (user.age < 18) errors.push('Must be 18+');
+   if (user.country === 'blocked') errors.push('Country not supported');
+   return errors;
+ }

# After: Chain of responsibility
+ abstract class Validator {
+   abstract validate(user: User): string | null;
+   setNext(validator: Validator): Validator {
+     this.next = validator;
+     return validator;
+   }
+   validate(user: User): string | null {
+     const error = this.doValidate(user);
+     if (error) return error;
+     return this.next?.validate(user) ?? null;
+   }
+ }
+
+ class EmailRequiredValidator extends Validator {
+   doValidate(user: User) {
+     return !user.email ? 'Email required' : null;
+   }
+ }
+
+ class EmailFormatValidator extends Validator {
+   doValidate(user: User) {
+     return user.email && !isValidEmail(user.email) ? 'Invalid email' : null;
+   }
+ }
+
+ // Build the chain
+ const validator = new EmailRequiredValidator()
+   .setNext(new EmailFormatValidator())
+   .setNext(new NameRequiredValidator())
+   .setNext(new AgeValidator())
+   .setNext(new CountryValidator());
```
---

##重构步骤

安全的重构过程```
1. PREPARE
   - Ensure tests exist (write them if missing)
   - Commit current state
   - Create feature branch

2. IDENTIFY
   - Find the code smell to address
   - Understand what the code does
   - Plan the refactoring

3. REFACTOR (small steps)
   - Make one small change
   - Run tests
   - Commit if tests pass
   - Repeat

4. VERIFY
   - All tests pass
   - Manual testing if needed
   - Performance unchanged or improved

5. CLEAN UP
   - Update comments
   - Update documentation
   - Final commit
```
---

##重构清单

代码质量

[]函数很小（< 50行）
-[]函数只做一件事
-[]无重复代码
-[]描述性名称（变量、函数、类）
-[]没有魔法numbers/strings-移除死代码

# # #结构

—[]相关代码在一起
-[]清除模块边界
-[]依赖项是单向流动的
-[]没有循环依赖

类型安全

-[]所有公共api定义的类型
-[]无`any`类型
-[]明确标记的可空类型

# # #测试

-[]对重构的代码进行测试
-[]测试涵盖边缘情况
-[]所有测试通过

---

##常见重构操作

|操作名称|描述|| --------------------------------------------- | ------------------------------------- |
将代码片段转换为方法|
|提取类|移动行为到新类|
|提取接口|从实现|创建接口
将方法体移回调用者|
将类行为移动到调用者|
将方法移动到超类|
将方法移动到子类|
|重命名Method/Variable|提高清晰度|
|引入参数对象|组相关参数|
|将条件替换为多态性|使用多态性代替switch/if|
|取代命名常量|
|分解条件|分解复杂条件|
| consolidation条件必选|组合重复条件|
|用保护子句替换嵌套条件|早期返回|
|引入空对象|消除空检查|
|将类型代码替换为Class/Enum|强类型|
|用委托代替继承|组合优于继承|