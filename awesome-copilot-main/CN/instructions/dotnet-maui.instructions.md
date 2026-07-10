---
description: '.NET MAUI component and application patterns'
applyTo: '**/*.xaml, **/*.cs'
---
#。净毛伊岛

# #。NET MAUI代码风格和结构

-写作习惯、高效。NET MAUI和c#代码。
-跟着。NET和。NET毛伊岛公约。
保持UI（视图）专注于布局和绑定；将逻辑保存在ViewModels和services中。
对I/O和长时间运行的工作使用async/await，以保持UI响应。

命名约定

—组件名、方法名和公共成员遵循PascalCase。
-私有字段和局部变量使用camelCase。
-以“I”作为接口名称的前缀（例如，IUserService）。

# #。NET毛伊岛和。NET具体指引-利用。NET MAUI组件生命周期的内置特性（例如onappear， ondisappear）。
-通过`{Binding}`和MVVM模式有效地使用数据绑定。
-结构。NET MAUI组件和服务遵循关注点分离。
-使用repo的目标支持的语言版本。. NET SDK和设置；避免要求预览语言特性，除非项目已经为它们配置好了。

关键规则（一致性）永远不要使用ListView（已弃用）。使用CollectionView。
永远不要使用TableView（已弃用）。首选CollectionView或布局，如Grid/VerticalStackLayout.-永远不要使用Frame（已弃用）。使用Border代替。
永远不要使用`*AndExpand`布局选项（已弃用）。使用网格和显式大小。
-永远不要在StackLayout/VerticalStackLayout/HorizontalStackLayout内放置ScrollView或CollectionView（会破坏滚动和虚拟化）。使用Grid作为父布局。
-永远不要在运行时引用图像为`.svg`。使用PNG/JPG资源。
-永远不要将Shell导航与NavigationPage/TabbedPage/FlyoutPage.混合
-永远不要使用渲染器。使用处理程序。
-永远不要设置`BackgroundColor`；使用`Background`（支持gradients/brushes，是首选的现代API）。

布局和控制选择-`VerticalStackLayout`/`HorizontalStackLayout`优于`StackLayout Orientation="..."`（性能更好）。
-使用`BindableLayout`小，不可滚动的列表（≤20个项目）。对于较大的或可滚动的列表，使用`CollectionView`。
-更喜欢`Grid`复杂的布局，当你需要细分空间。
-对于带有borders/backgrounds.的容器，首选`Border`而不是`Frame`## Shell导航

—使用Shell作为主导航主机。
-用`Routing.RegisterRoute(...)`注册路由，用`Shell.Current.GoToAsync(...)`导航。
-启动时设置一次`MainPage`；避免频繁更换。
-不要在Shell中嵌套标签。

错误处理和验证

-对以下错误进行适当的处理。NET MAUI页面和API调用。
-使用日志记录应用级错误；日志和表面用户友好的消息，可恢复的故障。
-在表单中使用FluentValidation或DataAnnotations实现验证。

MAUI API和性能优化-为了性能和正确性，首选编译绑定。	- In XAML, set `x:DataType` on pages/views/templates.
	- Prefer expression-based bindings in C# where possible.
	- Consider enabling stricter XAML compilation in project settings (for example `MauiStrictXamlCompilation=true`), especially in CI.
-避免深度布局嵌套（特别是嵌套的StackLayouts）。对于复杂的布局，首选网格。
-保持有意绑定：	- Use `OneTime` when values don't change.
	- Use `TwoWay` only for editable values.
	- Avoid binding static constants; set them directly.
-使用`Dispatcher.Dispatch()`或`Dispatcher.DispatchAsync()`从后台工作更新UI	- Prefer `BindableObject.Dispatcher` when you have a reference to a Page, View, or other BindableObject.
	- Inject `IDispatcher` via DI when working in services or ViewModels without direct BindableObject access.
	- Use `MainThread.BeginInvokeOnMainThread(...)` as a fallback only when no Dispatcher is available.
	- **Avoid** obsolete `Device.BeginInvokeOnMainThread` patterns.
资源和资产

-将图像放在`Resources/Images/`，字体放在`Resources/Fonts/`，原始资产放在`Resources/Raw/`。
-参考映像为PNG/JPG（例如，`<Image Source="logo.png" />`），而不是`.svg`。
-使用适当大小的图像，以避免内存膨胀。

##状态管理

-对于共享状态和交叉关注点，更倾向于采用di管理的服务；保持ViewModels的生存期为navigation/page。

API设计和集成

-使用HttpClient或其他适当的服务与外部api或您自己的后端进行通信。
使用try-catch实现API调用的错误处理，并在UI中提供适当的用户反馈。

##存储和秘密

-使用`SecureStorage`的秘密（令牌，刷新令牌），并处理异常（不支持的设备，密钥更改，损坏）由clearing/resetting和重新认证。
-避免在首选项中存储秘密。

##测试和调试—使用xUnit、NUnit或MSTest测试组件和服务。
-在测试期间使用Moq或NSubstitute来模拟依赖项。

安全性和身份验证

-在MAUI应用中实现身份验证和授权，必要时使用OAuth或JWT令牌进行API身份验证。
-使用HTTPS进行所有web通信，并确保执行适当的CORS策略。

##常见陷阱

-频繁更改`MainPage`可能会导致导航问题。
-父视图和子视图上的手势识别器可能会发生冲突；在需要的地方使用`InputTransparent = true`。
-未订阅事件导致的内存泄漏；始终取消订阅并处置资源。
深度嵌套的布局会影响性能；使视觉层次扁平化。
只在模拟器上测试会错过真实设备的边缘情况在物理设备上测试。