---
description: 'Razor Pages component and application patterns'
applyTo: '**/*.cshtml, **/*.cshtml.cs'
---
Razor页面的代码风格和结构

编写习惯的，高效的Razor页面和c#。
-坚持框架构建的约定：基于处理程序的PageModels，而不是硬塞进页面的MVC控制器模式。
保持PageModels专注于request/response编排；业务逻辑属于注入的域服务。
-简单的处理程序可以保持内联。对于具有大量处理程序和依赖项的页面，请使用MediatR之类的中介。
-使用async/await端到端，这样处理程序就不会阻塞请求管道。

命名约定- PageModel类，处理程序方法和公共成员（`CreateModel`,`OnPostAsync`,`OnPostDeleteAsync`）的PascalCase。
私有字段和局部字段的camelCase，私有字段的前缀为`_`。. NET约定（`_context`,`_logger`）。
—接口名称以“I”开头（`IEmailService`）。
命名处理程序在路由时删除`OnPost`/`Async`词缀。到达`OnPostJoinListAsync`为`handler=JoinList`。

模型绑定和叠加-不要把`[BindProperty]`直接放在EF域实体上。攻击者可以发布额外的字段，如`IsAdmin`或`Secret`，绑定器会很高兴地设置它们，即使表单不呈现它们。
-绑定到一个专用的输入模型或视图模型，只公开页面允许接受的属性，然后映射到实体。`TryUpdateModelAsync<T>`带有一个显式的属性允许列表是另一个选项，特别是在编辑场景中。
-避免使用`[Bind]`进行编辑。被排除的属性被重置为`default(T)`，而不是单独保留，这很少是您想要的。首选输入模型。
-不要广泛启用`[BindProperty(SupportsGet = true)]`。Razor Pages默认跳过GET绑定是有原因的；选择每个属性并验证输入的内容。
-对于自定义类型（包括强类型id），实现`TryParse`或`TypeConverter`，以便它们从路由和查询值绑定。如果没有，绑定器将它们视为复杂类型和绑定吴恩达默默地失败了。就是那种浪费一下午时间的虫子。
-`[BindRequired]`和`[Required]`不是一回事。`[BindRequired]`错误时，源值是*缺席*从提交的表单；`[Required]`验证绑定值不是null/empty.。`[BindRequired]`只适用于表单绑定，因为JSON和XML要通过输入格式化器。处理程序方法和请求流

—对于成功的post，始终使用Post-Redirect-Get。返回`RedirectToPage("./Index")`，不返回`Page()`。成功返回`Page()`意味着浏览器刷新会重新提交表单。```csharp
public async Task<IActionResult> OnPostAsync()
{
    if (!ModelState.IsValid) return Page();          // re-render on error
    await _service.CreateAsync(Input);
    return RedirectToPage("./Index");                // PRG on success
}
```
—使用`if (!ModelState.IsValid) return Page();`保护每个持久化路径。客户端验证可以被绕过；服务器是权威的。
—对于单请求路由或查询值，使用处理程序参数（`OnGetAsync(int id)`）。对于需要在验证错误时往返返回视图的POST数据，使用`[BindProperty]`。
-命名处理程序（`OnPostDeleteAsync`,`OnPostApproveAsync`）需要提交按钮上的`asp-page-handler`标签帮助器。如果没有它，普通按钮就会退回到`OnPostAsync`或404。
-如果`OnGet`做昂贵的工作，添加一个轻量级的`OnHead`。否则，对于HEAD请求，Razor Pages返回到`OnGet`，因此每个探测都要支付全部GET成本。
过滤器在这里的工作方式与MVC中的不同：`[ActionFilter]`属性在页面处理程序中被静默忽略。使用`IPageFilter`/`IAsyncPageFilter`，或者通过`options.Conventions`在`Program.cs`中注册全局约定。

项目结构和约定-共享布局，部分和模板在`Pages/Shared/`，而不是`Views/Shared/`。Razor Pages从页面文件夹到`Pages/`分层次解析视图，混合MVC约定只会与框架冲突。
—在“`Pages/_ViewStart.cshtml`”中设置“`Layout`”。对`@namespace`、`@addTagHelper`和共享指令使用`Pages/_ViewImports.cshtml`。
—保持“`.cshtml`”和“`.cshtml.cs`”在同一个位置。每个页面的局部性是使用Razor Pages的主要原因之一，而将它们分散到不同的文件夹就会失去这一点。

# #安全-信任Razor默认的`@`表达式HTML编码。不要在用户提供的内容上使用`@Html.Raw()`；它禁用编码，为XSS打开了大门。
-坚持使用`<form method="post">`和表单标签助手，以便自动注入防伪令牌。对于AJAX或`fetch`，使用`@Html.AntiForgeryToken()`呈现令牌，并将其作为`RequestVerificationToken`标头发送。
—不要向`appsettings.json`提交秘密。将`appsettings.{Environment}.json`用于环境覆盖，在本地使用用户秘密（`dotnet user-secrets`），在生产环境中使用Azure密钥库或环境变量。通过`IOptions<T>`绑定。

在PageModels中进行依赖注入

-注意单例范围内的强制依赖陷阱。如果一个单例包含对作用域服务的引用（比如EF`DbContext`），那么该实例会在请求之间泄漏。pagemodel邻近服务中的常见错误。
—不要将`DbContext`注册为`Singleton`。默认的`AddDbContext`注册为`Scoped`是有原因的。实体框架核心在页面处理程序

-在将EF实体返回到视图之前，使用`.Select(...)`将它们项目到dto或视图模型。直接传递带有导航属性的实体会导致延迟加载异常、N+1查询或视图呈现时的序列化周期。
-在只读查询中使用`.AsNoTracking()`，如列表页面或详细信息页面，无需编辑。变更跟踪器有您不需要的开销。
-当不使用`Include`的主键获取时，首选`FindAsync(key)`而不是`FirstOrDefaultAsync(x => x.Id == key)`。`FindAsync`首先检查变更跟踪器。

##状态管理-`TempData`是一次性的，交叉重定向的消息，如flash通知后的PRG。它是一次读取，默认情况下是cookie序列化的，不能替代会话存储。
—对于实际的每用户会话状态，使用`ISession`。对于每个请求数据，`HttpContext.Items`。对于单个请求中的共享状态，使用请求作用域的DI服务。
—当一个值需要在多次重定向中存活而不被消耗时，调用`TempData.Keep()`或`TempData.Peek()`。

# #测试

-直接对`PageModel`类进行单元测试。用模拟依赖（Moq, NSubstitute）实例化它们，并对返回的`IActionResult`进行断言：`PageResult`表示重新呈现，`RedirectToPageResult`表示成功的PRG，`NotFoundResult`表示404路径。
—对于执行路由、模型绑定和防伪的集成测试，使用`WebApplicationFactory<TEntryPoint>`和`Microsoft.AspNetCore.Mvc.Testing`。
当测试读取`ModelState`的处理程序时，手动填充`PageModel.ModelState.AddModelError(...)`。绑定管道不会在单元测试中运行。