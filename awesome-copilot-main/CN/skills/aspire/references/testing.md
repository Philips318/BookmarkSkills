#测试-完整参考

Aspire提供`Aspire.Hosting.Testing`，用于针对完整的AppHost运行集成测试。测试启动整个分布式应用程序（或一个子集），并针对实际服务运行断言。

---

# #包```xml
<PackageReference Include="Aspire.Hosting.Testing" Version="*" />
```
---

核心模式：DistributedApplicationTestingBuilder```csharp
// 1. Create a testing builder from your AppHost
var builder = await DistributedApplicationTestingBuilder
    .CreateAsync<Projects.MyAppHost>();

// 2. (Optional) Override resources for testing
// ... see customization section below

// 3. Build and start the application
await using var app = await builder.BuildAsync();
await app.StartAsync();

// 4. Create HTTP clients for your services
var client = app.CreateHttpClient("api");

// 5. Run assertions
var response = await client.GetAsync("/health");
Assert.Equal(HttpStatusCode.OK, response.StatusCode);
```
---

## xUnit示例

基本健康检查测试```csharp
public class HealthTests(ITestOutputHelper output)
{
    [Fact]
    public async Task AllServicesAreHealthy()
    {
        var builder = await DistributedApplicationTestingBuilder
            .CreateAsync<Projects.AppHost>();

        await using var app = await builder.BuildAsync();
        await app.StartAsync();

        // Test each service's health endpoint
        var apiClient = app.CreateHttpClient("api");
        var apiHealth = await apiClient.GetAsync("/health");
        Assert.Equal(HttpStatusCode.OK, apiHealth.StatusCode);

        var workerClient = app.CreateHttpClient("worker");
        var workerHealth = await workerClient.GetAsync("/health");
        Assert.Equal(HttpStatusCode.OK, workerHealth.StatusCode);
    }
}
```
API集成测试```csharp
public class ApiTests(ITestOutputHelper output)
{
    [Fact]
    public async Task CreateOrder_ReturnsCreated()
    {
        var builder = await DistributedApplicationTestingBuilder
            .CreateAsync<Projects.AppHost>();

        await using var app = await builder.BuildAsync();
        await app.StartAsync();

        var client = app.CreateHttpClient("api");

        var order = new { ProductId = 1, Quantity = 2 };
        var response = await client.PostAsJsonAsync("/orders", order);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var created = await response.Content.ReadFromJsonAsync<Order>();
        Assert.NotNull(created);
        Assert.Equal(1, created.ProductId);
    }
}
```
###测试等待准备就绪```csharp
[Fact]
public async Task DatabaseIsSeeded()
{
    var builder = await DistributedApplicationTestingBuilder
        .CreateAsync<Projects.AppHost>();

    await using var app = await builder.BuildAsync();
    await app.StartAsync();

    // Wait for the API to be fully ready (all dependencies healthy)
    await app.WaitForResourceReadyAsync("api");

    var client = app.CreateHttpClient("api");
    var response = await client.GetAsync("/products");

    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    var products = await response.Content.ReadFromJsonAsync<List<Product>>();
    Assert.NotEmpty(products);
}
```
---

## MSTest示例```csharp
[TestClass]
public class IntegrationTests
{
    [TestMethod]
    public async Task ApiReturnsProducts()
    {
        var builder = await DistributedApplicationTestingBuilder
            .CreateAsync<Projects.AppHost>();

        await using var app = await builder.BuildAsync();
        await app.StartAsync();

        var client = app.CreateHttpClient("api");
        var response = await client.GetAsync("/products");

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
    }
}
```
---

## NUnit示例```csharp
[TestFixture]
public class IntegrationTests
{
    [Test]
    public async Task ApiReturnsProducts()
    {
        var builder = await DistributedApplicationTestingBuilder
            .CreateAsync<Projects.AppHost>();

        await using var app = await builder.BuildAsync();
        await app.StartAsync();

        var client = app.CreateHttpClient("api");
        var response = await client.GetAsync("/products");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }
}
```
---

自定义测试AppHost

覆盖资源```csharp
var builder = await DistributedApplicationTestingBuilder
    .CreateAsync<Projects.AppHost>();

// Replace a real database with a test container
builder.Services.ConfigureHttpClientDefaults(http =>
{
    http.AddStandardResilienceHandler();
});

// Add test-specific configuration
builder.Configuration["TestMode"] = "true";

await using var app = await builder.BuildAsync();
await app.StartAsync();
```
###排除资源```csharp
var builder = await DistributedApplicationTestingBuilder
    .CreateAsync<Projects.AppHost>(args =>
    {
        // Don't start the worker for API-only tests
        args.Args = ["--exclude-resource", "worker"];
    });
```
在特定环境下进行测试```csharp
var builder = await DistributedApplicationTestingBuilder
    .CreateAsync<Projects.AppHost>(args =>
    {
        args.Args = ["--environment", "Testing"];
    });
```
---

##连接字符串访问```csharp
// Get the connection string for a resource in tests
var connectionString = await app.GetConnectionStringAsync("db");

// Use it to query the database directly in tests
using var conn = new NpgsqlConnection(connectionString);
await conn.OpenAsync();
var count = await conn.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM products");
Assert.True(count > 0);
```
---

最佳实践

1. **在发出请求之前使用`WaitForResourceReadyAsync`** -确保所有依赖项都是健康的
2. **每个测试应该是独立的** -不要依赖于以前测试的状态
3. **使用`await using`**的应用程序-确保清理，即使在测试失败
4. **测试真实的基础设施** - Aspire旋转真实的容器（Redis， PostgreSQL等），为您提供高保真的集成测试
5. **保持测试AppHost精简** -排除特定测试场景不需要的资源
6. **使用特定于测试的配置** -覆盖测试隔离设置
7. **超时保护** -设置合理的测试超时，因为容器需要时间启动：```csharp
[Fact(Timeout = 120_000)]  // 2 minutes
public async Task SlowIntegrationTest() { ... }
```
---

##项目结构```
MyApp/
├── src/
│   ├── MyApp.AppHost/           # AppHost project
│   ├── MyApp.Api/               # API service
│   ├── MyApp.Worker/            # Worker service
│   └── MyApp.ServiceDefaults/   # Shared defaults
└── tests/
    └── MyApp.Tests/             # Integration tests
        ├── MyApp.Tests.csproj   # References AppHost + Testing package
        └── ApiTests.cs          # Test classes
```

```xml
<!-- MyApp.Tests.csproj -->
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <IsAspireTestProject>true</IsAspireTestProject>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Aspire.Hosting.Testing" Version="*" />
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="*" />
    <PackageReference Include="xunit" Version="*" />
    <PackageReference Include="xunit.runner.visualstudio" Version="*" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\..\src\MyApp.AppHost\MyApp.AppHost.csproj" />
  </ItemGroup>
</Project>
```
