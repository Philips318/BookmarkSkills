---
description: 'Blazor component and application patterns'
applyTo: '**/*.razor, **/*.razor.cs, **/*.razor.css'
---
Blazor代码风格和结构

-编写习惯和高效的Blazor和c#代码。
-跟着。. NET和Blazor约定。
-在基于组件的UI开发中适当地使用Razor组件。
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
-始终使用最新版本的c#，目前c# 14的特性，如记录类型，模式匹配和全局使用。

错误处理和验证

-为Blazor页面和API调用实现适当的错误处理。
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
-当应用程序变得复杂时，使用Fluxor或BlazorState等库实现高级状态管理解决方案。
对于Blazor WebAssembly中的客户端状态持久化，可以考虑使用Blazor。LocalStorage或blazered。SessionStorage在页面重新加载之间保持状态。
对于服务器端Blazor，使用Scoped Services和StateContainer模式来管理用户会话中的状态，同时最小化重新渲染。

API设计和集成

-使用HttpClient或其他适当的服务与外部api或您自己的后端进行通信。
使用try-catch实现API调用的错误处理，并在UI中提供适当的用户反馈。

##测试和调试-所有的单元测试和集成测试都应该跨ide运行（Visual Studio,VS Code, JetBrains Rider），这样贡献者就不会被付费SKU限制。
-使用xUnit、NUnit或MSTest测试Blazor组件和服务。
-在测试期间使用Moq或NSubstitute来模拟依赖项。
-使用浏览器开发工具调试Blazor UI问题，并使用IDE的调试器处理后端和服务器端问题。
对于性能分析和优化，使用IDE的诊断工具或`dotnet-trace`/`dotnet-counters`进行跨平台分析。

安全性和身份验证

-在必要时使用ASP在Blazor应用程序中实现身份验证和授权。. NET标识或JWT令牌用于API身份验证。
-使用HTTPS进行所有web通信，并确保执行适当的CORS策略。

## API文档和Swagger-使用Swagger/OpenAPI作为后端API服务的API文档。
确保模型和API方法的XML文档，以增强Swagger文档。