#查询检查点

获取代码库-知识工作流阶段2的每个模板调查问题。对于每个模板区域，首先在扫描输出中查找答案，然后读取源文件以填补空白。

---

# # 1。STACK.md-技术栈

-主要语言和准确版本是什么？（检查`.nvmrc`，`go.mod`,`pyproject.toml`， Docker`FROM`线）
-使用什么包管理器？（`npm`,`yarn`,`pnpm`,`go mod`,`pip`,`uv`）
-核心运行时框架是什么？（web服务器、ORM、DI容器）
-`dependencies`（生产）和`devDependencies`（开发工具）包含什么？
-是否有Docker镜像和它使用的基本镜像？
—`package.json`/`Makefile`/`pyproject.toml`中的关键脚本有哪些？

# # 2。STRUCTURE.md-目录布局源代码在哪里？（通常是`src/`，`lib/`，或者Go的项目根目录）
-入口在哪里？（在`package.json`，`scripts.start`,`cmd/main.go`，`app.py`中检查`main`）
-每个顶级目录的目的是什么？
-是否有不明显的目录（例如，`eng/`,`platform/`,`infra/`）？
—是否有隐藏的配置目录（`.github/`,`.vscode/`,`.husky/`）？
-目录遵循什么命名约定？（骆驼案例、烤串案例、基于域vs基于层）

# # 3。ARCHITECTURE.md-图案代码是按层组织（控制器→服务→仓库）还是按功能组织？
—主数据流是什么？从入口到数据存储跟踪一个请求或命令。
是否存在单例、依赖注入模式或明确的初始化顺序要求？
是否存在后台工作器、队列或事件驱动组件？
-重复出现的设计模式是什么？（工厂、存储库、装饰器、策略）

# # 4。CONVENTIONS.md-编码标准—文件命名规范是什么？（检查10多个文件- camelCase， kebab-case, PascalCase）
—函数和变量的命名约定是什么？
—私有域名是否有前缀methods/fields（如`_methodName`、`#field`）？
—配置了什么linter和formatter ？（检查`.eslintrc`，`.prettierrc`,`golangci.yml`）
TypeScript的严格设置是什么？（`strict`、`noImplicitAny`等）
-每一层的错误是如何处理的？（抛出与返回结构化错误）
—使用什么日志库，日志消息格式是什么？
-进口是如何组织的？（桶导出、路径别名、分组规则）

# # 5。INTEGRATIONS.md—外部服务-调用哪些外部api ？（在常量中搜索`axios.`，`fetch(`,`http.Get(`，基本url）
-凭据如何存储和访问？（`.env`，秘密管理器，环境变量）
—连接了哪些数据库？（检查`pg`、`mongoose`、`prisma`、`typeorm`、`sqlalchemy`舱单）
在应用程序和外部服务之间是否有API网关、服务网格或代理？
-使用什么监控或观察工具？（APM，普罗米修斯，测井管道）
是否有消息队列或事件总线？（Kafka, RabbitMQ， SQS,Pub/Sub）

# # 6。TESTING.md-测试设置-配置了什么测试运行器？（在`package.json`，`pytest.ini`，`go test`中检查`scripts.test`）
-测试文件位于哪里？（在源代码旁边，在`tests/`，在`__tests__/`）
-使用什么断言库？（Jest expect, Chai, pytest assert）
-外部依赖是如何模拟的？(笑话。Mock、依赖注入、fixture)
是否存在针对真实服务的集成测试和针对模拟的单元测试？
是否有强制的覆盖率阈值？（检查`jest.config.js`，`.nycrc`,`pyproject.toml`）

# # 7。CONCERNS.md-已知问题生产代码中有多少TODOs/FIXMEs/HACKs？（见扫描输出）
-在过去90天内，哪些文件的git流失率最高？（见扫描输出）
-是否有超过500行的文件混合了多个职责？
-是否有任何服务进行可以并行化的顺序调用？
-是否有硬编码值（url, id，魔术数字），应该配置？
—存在哪些安全风险？（缺少输入验证、向客户端公开原始错误消息、缺少认证检查）
是否存在不可伸缩的性能模式？（N+1查询，多实例设置中的内存缓存）