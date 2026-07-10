---
description: 'Oqtane Module patterns'
applyTo: '**/*.razor, **/*.razor.cs, **/*.razor.css'
---
Blazor代码风格和结构

-编写习惯和高效的Blazor和c#代码。
-跟着。. NET和Blazor约定。
-在基于组件的UI开发中适当地使用Razor组件。
-适当地使用Blazor组件进行基于组件的UI开发。
-对于较小的组件，更倾向于使用内联函数，但将复杂的逻辑分离到代码隐藏或服务类中。
-Async/await应在适用的情况下使用，以确保非阻塞UI操作。


命名约定

—组件名、方法名和公共成员遵循PascalCase。
-私有字段和局部变量使用camelCase。
-以“I”作为接口名称的前缀（例如，IUserService）。

## Blazor和。NET具体指引-利用Blazor的组件生命周期内置功能（例如，OnInitializedAsync, OnParametersSetAsync）。
-通过@bind有效地使用数据绑定。
-在Blazor中为服务使用依赖注入。
-按照关注点分离来构建Blazor组件和服务。
-始终使用最新版本的c#，目前c# 13的特性，如记录类型，模式匹配和全局使用。##特定的指导方针
-参见[主Oqtane repo]中的基类和模式（https://github.com/oqtane/oqtane.framework）
-遵循客户端-服务器模式进行模块开发。
- Client项目在modules文件夹中有各种模块。
客户端模块中的每个动作都是一个独立的razor文件，它继承自带有索引的ModuleBase。剃刀是默认动作。
-对于复杂的客户端处理，如获取数据，创建一个继承自ServiceBase并位于services文件夹中的服务类。每个模块一个服务类。
客户端服务应该使用ServiceBase方法调用服务器端点
-服务器项目包含MVC控制器，每个模块一个匹配客户端服务调用。每个控制器将调用由DI管理的服务器端服务或存储库
服务器项目对模块使用存储库模式，每个模块使用一个存储库类来匹配控制器。错误处理和验证

-为Blazor页面和API调用实现适当的错误处理。
-从基类中使用内置的Oqtane日志方法。
-在后端使用日志进行错误跟踪，并考虑在Blazor中使用ErrorBoundary等工具捕获ui级错误。
-在表单中使用FluentValidation或DataAnnotations实现验证。

## Blazor API和性能优化-根据项目需求优化利用Blazor服务器端或WebAssembly。
-使用异步方法（async/await）的API调用或UI操作，可能会阻塞主线程。
通过减少不必要的渲染和有效地使用stathaschanged（）来优化Razor组件。
-通过避免重新渲染来最小化组件渲染树，除非必要，在适当的地方使用ShouldRender（）。
-使用eventcallback有效地处理用户交互，在触发事件时只传递最小的数据。

##缓存策略-实现在内存中缓存频繁使用的数据，特别是对于Blazor服务器应用程序。使用immemorycache作为轻量级缓存解决方案。
-对于Blazor WebAssembly，利用localStorage或sessionStorage在用户会话之间缓存应用程序状态。
对于需要跨多个用户或客户端共享状态的大型应用程序，考虑分布式缓存策略（如Redis或SQL Server Cache）。
—通过存储响应来缓存API调用，避免数据不太可能改变时的冗余调用，从而改善用户体验。

##状态管理库-使用Blazor的内置级联参数和eventcallback来实现组件间的基本状态共享。
-在适当的时候，在PageState和SiteState等基类中使用内置的Oqtane状态管理。
-避免添加额外的依赖，如Fluxor或BlazorState，当应用程序的复杂性增长。
对于Blazor WebAssembly中的客户端状态持久化，可以考虑使用Blazor。LocalStorage或blazered。SessionStorage在页面重新加载之间保持状态。
对于服务器端Blazor，使用Scoped Services和StateContainer模式来管理用户会话中的状态，同时最小化重新渲染。

API设计和集成

-使用服务基方法与外部api或服务器项目后端进行通信。
使用try-catch实现API调用的错误处理，并在UI中提供适当的用户反馈。

在Visual Studio中进行测试和调试-所有的单元测试和集成测试都应该在Visual Studio Enterprise中完成。
-使用xUnit、NUnit或MSTest测试Blazor组件和服务。
-在测试期间使用Moq或NSubstitute来模拟依赖项。
-使用浏览器开发工具和Visual Studio的后端和服务器端调试工具调试Blazor UI问题。
对于性能分析和优化，依赖于Visual Studio的诊断工具。

安全性和身份验证

-使用内置的Oqtane基类成员（如User.Roles）实现身份验证和授权。
-使用HTTPS进行所有web通信，并确保执行适当的CORS策略。