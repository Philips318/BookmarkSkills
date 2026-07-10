---
applyTo: '**/*.{cs,ts,java}'
description: Enforces Object Calisthenics principles for business domain code to ensure clean, maintainable, and robust code
---
#对象健美操规则

>⚠️**警告：**此文件包含9个原始对象健美操规则。不能添加任何额外的规则，也不能替换或删除这些规则。
>如果需要，可以稍后添加示例。

# #目标
该规则强制Object Calisthenics原则，以确保后端代码（主要用于业务域代码）干净、可维护和健壮。

##范围和应用
- **主要关注**：业务领域类（聚合、实体、值对象、领域服务）
-次要焦点：应用层服务和用例处理程序
- * *豁免* *:
- dto（数据传输对象）
—APImodels/contracts-配置类
-没有业务逻辑的简单数据容器
-需要灵活性的基础设施代码

##关键原则1. **每个方法一层缩进**：
—确保方法简单，不要超过一层缩进。   ```csharp
   // Bad Example - this method has multiple levels of indentation
   public void SendNewsletter() {
         foreach (var user in users) {
            if (user.IsActive) {
               // Do something
               mailer.Send(user.Email);
            }
         }
   }
   // Good Example - Extracted method to reduce indentation
   public void SendNewsletter() {
       foreach (var user in users) {
           SendEmail(user);
       }
   }
   private void SendEmail(User user) {
       if (user.IsActive) {
           mailer.Send(user.Email);
       }
   }

   // Good Example - Filtering users before sending emails
   public void SendNewsletter() {
       var activeUsers = users.Where(user => user.IsActive);

       foreach (var user in activeUsers) {
           mailer.Send(user.Email);
       }
   }
   ```
2. **不要使用ELSE关键字**：

—避免使用`else`关键字，以减少复杂性和提高可读性。
-使用早期返回来处理条件。
—使用快速失败原则
-使用Guard子句来验证方法开头的输入和条件。   ```csharp
   // Bad Example - Using else
   public void ProcessOrder(Order order) {
       if (order.IsValid) {
           // Process order
       } else {
           // Handle invalid order
       }
   }
   // Good Example - Avoiding else
   public void ProcessOrder(Order order) {
       if (!order.IsValid) return;
       // Process order
   }
   ```
快速失效原则：   ```csharp
   public void ProcessOrder(Order order) {
       if (order == null) throw new ArgumentNullException(nameof(order));
       if (!order.IsValid) throw new InvalidOperationException("Invalid order");
       // Process order
   }
   ```
3. **包装所有原语和字符串**：
避免在代码中直接使用基本类型。
-将它们包装在类中以提供有意义的上下文和行为。   ```csharp
   // Bad Example - Using primitive types directly
   public class User {
       public string Name { get; set; }
       public int Age { get; set; }
   }
   // Good Example - Wrapping primitives
   public class User {
       private string name;
       private Age age;
       public User(string name, Age age) {
           this.name = name;
           this.age = age;
       }
   }
   public class Age {
       private int value;
       public Age(int value) {
           if (value < 0) throw new ArgumentOutOfRangeException(nameof(value), "Age cannot be negative");
           this.value = value;
       }
   }
   ```   
4. **一级收藏**：
-使用集合封装数据和行为，而不是暴露原始数据结构。
第一类集合：包含数组作为属性的类不应该包含任何其他属性```csharp
   // Bad Example - Exposing raw collection
   public class Group {
      public int Id { get; private set; }
      public string Name { get; private set; }
      public List<User> Users { get; private set; }

      public int GetNumberOfUsersIsActive() {
         return Users
            .Where(user => user.IsActive)
            .Count();
      }
   }

   // Good Example - Encapsulating collection behavior
   public class Group {
      public int Id { get; private set; }
      public string Name { get; private set; }

      public GroupUserCollection userCollection { get; private set; } // The list of users is encapsulated in a class

      public int GetNumberOfUsersIsActive() {
         return userCollection
            .GetActiveUsers()
            .Count();
      }
   }
   ```
5. **一行一个点**：
-避免违反得墨忒耳定律，每条线只有一个点。   ```csharp
   // Bad Example - Multiple dots in a single line
   public void ProcessOrder(Order order) {
       var userEmail = order.User.GetEmail().ToUpper().Trim();
       // Do something with userEmail
   }
   // Good Example - One dot per line
   public class User {
     public NormalizedEmail GetEmail() {
       return NormalizedEmail.Create(/*...*/);       
     }
   }
   public class Order {
     /*...*/
     public NormalizedEmail ConfirmationEmail() {
       return User.GetEmail();         
     }
   }
   public void ProcessOrder(Order order) {
       var confirmationEmail = order.ConfirmationEmail();
       // Do something with confirmationEmail
   }
   ```
6. * * * *别缩写:
—为类、方法和变量使用有意义的名称。
-避免使用可能导致混淆的缩写。   ```csharp
   // Bad Example - Abbreviated names
   public class U {
       public string N { get; set; }
   }
   // Good Example - Meaningful names
   public class User {
       public string Name { get; set; }
   }
   ```
7. **保持实体小（类、方法、命名空间或包）**：
限制类和方法的大小，以提高代码的可读性和可维护性。
-每个类应该有一个单一的职责，并尽可能小。

约束:
-每个类最多10个方法
-每个职业最多50行
—每个包或命名空间最多10个类   ```csharp
   // Bad Example - Large class with multiple responsibilities
   public class UserManager {
       public void CreateUser(string name) { /*...*/ }
       public void DeleteUser(int id) { /*...*/ }
       public void SendEmail(string email) { /*...*/ }
   }

   // Good Example - Small classes with single responsibility
   public class UserCreator {
       public void CreateUser(string name) { /*...*/ }
   }
   public class UserDeleter {
       public void DeleteUser(int id) { /*...*/ }
   }

   public class UserUpdater {
       public void UpdateUser(int id, string name) { /*...*/ }
   }
   ```
8. **没有超过两个实例变量的类**：
-通过限制实例变量的数量来鼓励类具有单一的职责。
-将实例变量的数量限制为两个以保持简单性。
-不要将ILogger或任何其他记录器作为实例变量。   ```csharp
   // Bad Example - Class with multiple instance variables
   public class UserCreateCommandHandler {
      // Bad: Too many instance variables
      private readonly IUserRepository userRepository;
      private readonly IEmailService emailService;
      private readonly ILogger logger;
      private readonly ISmsService smsService;

      public UserCreateCommandHandler(IUserRepository userRepository, IEmailService emailService, ILogger logger, ISmsService smsService) {
         this.userRepository = userRepository;
         this.emailService = emailService;
         this.logger = logger;
         this.smsService = smsService;
      }
   }

   // Good: Class with two instance variables
   public class UserCreateCommandHandler {
      private readonly IUserRepository userRepository;
      private readonly INotificationService notificationService;
      private readonly ILogger logger; // This is not counted as instance variable

      public UserCreateCommandHandler(IUserRepository userRepository, INotificationService notificationService, ILogger logger) {
         this.userRepository = userRepository;
         this.notificationService = notificationService;
         this.logger = logger;
      }
   }
   ```
9. ** Domain Classes中没有Getters/Setters-避免暴露域类中属性的设置器。
-使用私有构造函数和静态工厂方法创建对象。
- **注**：此规则主要适用于域类，不适用于dto或数据传输对象。   ```csharp
   // Bad Example - Domain class with public setters
   public class User {  // Domain class
       public string Name { get; set; } // Avoid this in domain classes
   }
   
   // Good Example - Domain class with encapsulation
   public class User {  // Domain class
       private string name;
       private User(string name) { this.name = name; }
       public static User Create(string name) => new User(name);
   }
   
   // Acceptable Example - DTO with public setters
   public class UserDto {  // DTO - exemption applies
       public string Name { get; set; } // Acceptable for DTOs
   }
   ```
##实施指南
- **域类**：
-使用私有构造函数和静态工厂方法创建实例。
-避免暴露属性的设置。
—业务域代码严格执行9条规则。

- **应用层**：
-将这些规则应用于用例处理程序和应用程序服务。
-专注于维护单一的职责和清晰的抽象。

- ** dto和数据对象**：
-规则3（包装原语），8（两个实例变量）和9（没有getters/setters）可以放宽dto。
-数据传输对象可以接受带有getters/setters的公共属性。

- * *测试* *:
-确保测试验证对象的行为，而不是它们的状态。
测试类可能对可读性和可维护性有宽松的规则。- **代码审查**：
-在对域和应用程序代码进行代码审查时执行这些规则。
-对基础设施和DTO代码保持务实。

# #引用
-[物体健美操- Jeff Bay原创9条规则]（https://www.cs.helsinki.fi/u/luontola/tdd-2009/ext/ObjectCalisthenics.pdf）
- [ThoughtWorks - Object健美操]（https://www.thoughtworks.com/insights/blog/object-calisthenics）
-[简洁代码：敏捷软件工艺手册- Robert C. Martin]（https://www.oreilly.com/library/view/clean-code-a/9780136083238/）