---
name: aspnet-minimal-api-openapi
description: 'Create ASP.NET Minimal API endpoints with proper OpenAPI documentation'
---
# ASP。. NET最小API与OpenAPI

你的目标是帮助我创建结构良好的ASP。具有正确类型和全面的OpenAPI/Swagger文档的NET最小API端点。

## API组织

—使用`MapGroup()`扩展名对相关端点进行分组
-对横切关注点使用端点过滤器
-使用单独的端点类构建更大的api
-考虑为复杂的api使用基于特性的文件夹结构

请求和响应类型

-定义明确的请求和响应DTOs/models-创建具有适当验证属性的清晰模型类
-为不可变的request/response对象使用记录类型
-使用符合API设计标准的有意义的属性名
-应用`[Required]`和其他验证属性来强制约束
-使用ProblemDetailsService和StatusCodePages来获得标准错误响应

##类型处理—使用带有显式类型绑定的强类型路由参数
—使用`Results<T1, T2>`表示多种响应类型
对于强类型响应，返回`TypedResults`而不是`Results`-利用c# 10+的特性，如可空注释和仅初始化属性

## OpenAPI文档

—使用内置的OpenAPI文档支持。网9
-定义操作总结和描述
—通过`WithName`扩展方式添加operationid
—使用`[Description()]`添加属性和参数的描述
—设置正确的请求和响应内容类型
-使用文档转换器来添加服务器、标签和安全方案等元素
-使用模式转换器将自定义应用到OpenAPI模式