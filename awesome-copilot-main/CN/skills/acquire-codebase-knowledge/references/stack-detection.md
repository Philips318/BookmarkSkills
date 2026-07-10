#堆栈检测参考

当技术栈不明确时加载这个文件——例如，存在多个清单文件，不熟悉的文件扩展名，或者没有明显的`package.json`/`go.mod`。

---

## Manifest文件→生态系统

|文件|生态系统|关键字段读取||------|-----------|--------------------|
|`dependencies`,`devDependencies`,`scripts`,`main`,`type`,`engines`|
|`go.mod`| Go |模块路径，Go版本，`require`块|
|`requirements.txt`| Python (pip) |固定版本的包列表|
|`Pipfile`| Python (pipenv) |`[packages]`,`[dev-packages]`，`[requires]`Python版本|
|`pyproject.toml`| Python (poetry / uv / hatch) |`[tool.poetry.dependencies]`,`[project]`,`[build-system]`|
|`setup.py`/`setup.cfg`| Python (setuptools, legacy) |`install_requires`,`python_requires`|
|`Cargo.toml`| Rust |`[dependencies]`,`[[bin]]`,`[lib]`|
|`pom.xml`| Java / Kotlin (Maven) |`<dependencies>`,`<artifactId>`,`<groupId>`,`<java.version>`|
|`build.gradle`/`build.gradle.kts`| Java / Kotlin (Gradle) |`dependencies {}`,`sourceCompatibility`|
|`composer.json`| PHP |`require`,`require-dev`|
|`Gemfile`| Ruby |`gem`声明，`ruby`版本约束|`deps/0`,`elixir: "~> X.Y"`|
|`pubspec.yaml`| Dart / Flutter |`dependencies`,`dev_dependencies`,`environment.sdk`|
|`*.csproj`|. NET / c# |`<PackageReference>`,`<TargetFramework>`|
|`*.sln`|。引用多个`.csproj`项目|
|`deno.json`/`deno.jsonc`| Deno （TypeScript运行时）|`imports`,`tasks`|
|`bun.lockb`| Bun （JavaScript运行时）|二进制锁定文件-检查`package.json`的深度|---

语言运行时版本检测

|语言|在哪里可以找到|版本|----------|--------------------------|
|Node.js|`.nvmrc`,`.node-version`，`engines.node`在`package.json`， Docker`FROM node:X`|
| Python |`.python-version`,`pyproject.toml [requires-python]`, Docker`FROM python:X`|
| Go |第一行`go.mod`(`go 1.21`) |
b| Java |`<java.version>`在`pom.xml`，`sourceCompatibility`在`build.gradle`， Docker`FROM eclipse-temurin:X`|
| Ruby |`.ruby-version`,`Gemfile``ruby 'X.Y.Z'`|
| Rust |`rust-toolchain.toml`，`rust-toolchain`文件|
|。. NET |`<TargetFramework>`in`.csproj`（例如，`net8.0`） |

---

##框架检测（Node.js/ TypeScript）

|`package.json`|框架中的依赖|-----------------------------|-----------|
|`express`|Express.js(最小HTTP服务器
|`fastify`| fasttify（高性能HTTP服务器）|
|`next`|Next.js（SSR/SSGReact -检查`pages/`或`app/`目录）|
|`nuxt`|Nuxt.js(SSR/SSGVue) |
|`@nestjs/core`| NestJS（固执己见的Node.js框架与DI） |
|`koa`| Koa（中间件为中心，没有内置路由器）|
|`@hapi/hapi`|快乐|
| tRPC（没有REST/GraphQL模式的类型安全API） |
|`routing-controllers`|路由控制器(基于装饰的Express包装器
|`typeorm`| TypeORM (SQL ORM with decorator
|`prisma`| Prisma（类型安全ORM，检查`prisma/schema.prisma`） |
|`mongoose`| Mongoose (MongoDB ODM) |
|`sequelize`| Sequelize (SQL ORM) |
|`drizzle-orm`| Drizzle（轻量级SQL ORM） |
|`react`没有`next`|香草反应SPA（检查`react-router-dom`） |
|`vue`无`nuxt`|香草Vue SPA |

---

框架检测（Python）

|包|框架||---------|-----------|
|`fastapi`| FastAPI(异步REST，自动OpenAPI文档
| Flask(最小的WSGI web框架
|`django`| Django（电池包括，检查`settings.py`） |
Starlette （ASGI，通常用作FastAPI基础）|
|`aiohttp`| aiohttp(异步HTTP客户端和服务端
|`sqlalchemy`| SQLAlchemy (SQL ORM；检查`alembic`迁移
|`alembic`| Alembic (SQLAlchemy迁移工具
|`pydantic`| Pydantic（数据验证；FastAPI核心）|
|`celery`|芹菜（分布式任务队列）|

---

单点检测

按顺序检查这些信号：

1.`pnpm-workspace.yaml`- PNPM工作区
2.`lerna.json`- Lerna monorepo
3.`nx.json`- Nx单通道（也检查`workspace.json`）
4.`turbo.json`-涡轮增压
5.`rush.json`- Rush（微软单线程管理器）
6.`moon.yml`-月亮
7.`package.json`与`"workspaces": [...]`-npm/yarn工作区
8. 存在`packages/`、`apps/`、`libs/`或`services/`目录以及它们自己的`package.json`如果检测到monorepo：每个工作区可能有独立的依赖项和约定。在`STACK.md`中分别映射每个子包，并注意`STRUCTURE.md`中的单线程结构。

---

TypeScript路径别名检测

如果`tsconfig.json`有一个`paths`键，那么带有非相对前缀的导入就是别名。在记录结构之前映射它们。```json
// tsconfig.json example
"paths": {
  "@/*": ["./src/*"],
  "@components/*": ["./src/components/*"],
  "@utils/*": ["./src/utils/*"]
}
```
像`import { foo } from '@/utils/bar'`这样的导入解析为`src/utils/bar`。文档名称为`src/utils/bar`，而不是`@/utils/bar`。

---

Docker基础镜像→运行时

如果没有清单文件存在，但存在`Dockerfile`，`FROM`行显示运行时：

| FROM line pattern | Runtime ||------------------|---------|
|`FROM node:X`|Node.jsX |
|`FROM python:X`| Python X |
|`FROM golang:X`| Go X |
|`FROM eclipse-temurin:X`| Java X (Eclipse Temurin JDK
|`FROM mcr.microsoft.com/dotnet/aspnet:X`|。净x |
|`FROM ruby:X`| Ruby X |
|`FROM rust:X`| Rust X |
|`FROM alpine`(alone) |检查通过`RUN apk add`|安装了什么