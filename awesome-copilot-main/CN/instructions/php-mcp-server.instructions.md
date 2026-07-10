---
description: 'Best practices for building Model Context Protocol servers in PHP using the official PHP SDK with attribute-based discovery and multiple transport options'
applyTo: '**/*.php'
---
# PHP MCP服务器开发最佳实践

本指南提供了使用与PHP基金会合作维护的官方PHP SDK构建模型上下文协议（MCP）服务器的最佳实践。

##安装和设置

###通过Composer安装```bash
composer require mcp/sdk
```
项目结构

组织你的PHP MCP服务器项目：```
my-mcp-server/
├── composer.json
├── src/
│   ├── Tools/
│   │   ├── Calculator.php
│   │   └── FileManager.php
│   ├── Resources/
│   │   ├── ConfigProvider.php
│   │   └── DataProvider.php
│   ├── Prompts/
│   │   └── PromptGenerator.php
│   └── Server.php
├── server.php           # Server entry point
└── tests/
    └── ToolsTest.php
```
编写器配置```json
{
    "name": "your-org/mcp-server",
    "description": "MCP Server for...",
    "type": "project",
    "require": {
        "php": "^8.2",
        "mcp/sdk": "^0.1"
    },
    "require-dev": {
        "phpunit/phpunit": "^10.0"
    },
    "autoload": {
        "psr-4": {
            "App\\": "src/"
        }
    }
}
```
##服务器实现

基本服务器与属性发现

创建服务器入口点：```php
#!/usr/bin/env php
<?php

declare(strict_types=1);

require_once __DIR__ . '/vendor/autoload.php';

use Mcp\Server;
use Mcp\Server\Transport\StdioTransport;

$server = Server::builder()
    ->setServerInfo('My MCP Server', '1.0.0')
    ->setDiscovery(__DIR__, ['.'])
    ->build();

$transport = new StdioTransport();

$server->run($transport);
```
带缓存的服务器

使用PSR-16缓存以获得更好的性能：```php
use Symfony\Component\Cache\Adapter\FilesystemAdapter;
use Symfony\Component\Cache\Psr16Cache;

$cache = new Psr16Cache(new FilesystemAdapter('mcp-discovery'));

$server = Server::builder()
    ->setServerInfo('My MCP Server', '1.0.0')
    ->setDiscovery(
        basePath: __DIR__,
        scanDirs: ['.', 'src'],
        excludeDirs: ['vendor', 'tests'],
        cache: $cache
    )
    ->build();
```
手动注册

以编程方式注册功能：```php
use App\Tools\Calculator;
use App\Resources\Config;

$server = Server::builder()
    ->setServerInfo('My MCP Server', '1.0.0')
    ->addTool([Calculator::class, 'add'], 'add')
    ->addTool([Calculator::class, 'multiply'], 'multiply')
    ->addResource([Config::class, 'getSettings'], 'config://app/settings')
    ->build();
```
##工具开发

简单的工具与属性```php
<?php

namespace App\Tools;

use Mcp\Capability\Attribute\McpTool;

class Calculator
{
    /**
     * Adds two numbers together.
     * 
     * @param int $a The first number
     * @param int $b The second number
     * @return int The sum of the two numbers
     */
    #[McpTool]
    public function add(int $a, int $b): int
    {
        return $a + $b;
    }
}
```
自定义名称工具```php
use Mcp\Capability\Attribute\McpTool;

class FileManager
{
    /**
     * Reads file content from the filesystem.
     */
    #[McpTool(name: 'read_file')]
    public function readFileContent(string $path): string
    {
        if (!file_exists($path)) {
            throw new \InvalidArgumentException("File not found: {$path}");
        }
        
        return file_get_contents($path);
    }
}
```
带有验证和模式的工具```php
use Mcp\Capability\Attribute\{McpTool, Schema};

class UserManager
{
    #[McpTool(name: 'create_user')]
    public function createUser(
        #[Schema(format: 'email')]
        string $email,
        
        #[Schema(minimum: 18, maximum: 120)]
        int $age,
        
        #[Schema(
            pattern: '^[A-Z][a-z]+$',
            description: 'Capitalized first name'
        )]
        string $firstName
    ): array
    {
        return [
            'id' => uniqid(),
            'email' => $email,
            'age' => $age,
            'firstName' => $firstName
        ];
    }
}
```
复杂返回类型的工具```php
use Mcp\Schema\Content\{TextContent, ImageContent};

class ReportGenerator
{
    #[McpTool]
    public function generateReport(string $type): array
    {
        return [
            new TextContent('Report generated:'),
            TextContent::code($this->generateCode($type), 'php'),
            new TextContent('Summary: All checks passed.')
        ];
    }
    
    #[McpTool]
    public function getChart(string $chartType): ImageContent
    {
        $imageData = $this->generateChartImage($chartType);
        
        return new ImageContent(
            data: base64_encode($imageData),
            mimeType: 'image/png'
        );
    }
}
```
带有匹配表达式的工具```php
#[McpTool(name: 'calculate')]
public function performCalculation(float $a, float $b, string $operation): float
{
    return match($operation) {
        'add' => $a + $b,
        'subtract' => $a - $b,
        'multiply' => $a * $b,
        'divide' => $b != 0 ? $a / $b : 
            throw new \InvalidArgumentException('Division by zero'),
        default => throw new \InvalidArgumentException('Invalid operation')
    };
}
```
##资源实现

静态资源```php
<?php

namespace App\Resources;

use Mcp\Capability\Attribute\McpResource;

class ConfigProvider
{
    /**
     * Provides the current application configuration.
     */
    #[McpResource(
        uri: 'config://app/settings',
        name: 'app_settings',
        mimeType: 'application/json'
    )]
    public function getSettings(): array
    {
        return [
            'version' => '1.0.0',
            'debug' => false,
            'features' => ['auth', 'logging']
        ];
    }
}
```
带变量的资源模板```php
use Mcp\Capability\Attribute\McpResourceTemplate;

class UserProvider
{
    /**
     * Retrieves user profile information by ID and section.
     */
    #[McpResourceTemplate(
        uriTemplate: 'user://{userId}/profile/{section}',
        name: 'user_profile',
        description: 'User profile data by section',
        mimeType: 'application/json'
    )]
    public function getUserProfile(string $userId, string $section): array
    {
        // Variable order must match URI template order
        return $this->users[$userId][$section] ?? 
            throw new \InvalidArgumentException("Profile section not found");
    }
}
```
带有文件内容的资源```php
use Mcp\Schema\Content\{TextResourceContents, BlobResourceContents};

class FileProvider
{
    #[McpResource(uri: 'file://readme.txt', mimeType: 'text/plain')]
    public function getReadme(): TextResourceContents
    {
        return new TextResourceContents(
            uri: 'file://readme.txt',
            mimeType: 'text/plain',
            text: file_get_contents(__DIR__ . '/README.txt')
        );
    }
    
    #[McpResource(uri: 'file://image.png', mimeType: 'image/png')]
    public function getImage(): BlobResourceContents
    {
        $imageData = file_get_contents(__DIR__ . '/image.png');
        
        return new BlobResourceContents(
            uri: 'file://image.png',
            mimeType: 'image/png',
            blob: base64_encode($imageData)
        );
    }
}
```
##快速实现

基本提示符```php
<?php

namespace App\Prompts;

use Mcp\Capability\Attribute\McpPrompt;

class PromptGenerator
{
    /**
     * Generates a code review request prompt.
     */
    #[McpPrompt(name: 'code_review')]
    public function reviewCode(string $language, string $code, string $focus = 'general'): array
    {
        return [
            ['role' => 'assistant', 'content' => 'You are an expert code reviewer.'],
            ['role' => 'user', 'content' => "Review this {$language} code focusing on {$focus}:\n\n```{$language}\n{$code}\n```"]
        ];
    }
}
```
混合内容提示符```php
use Mcp\Schema\Content\{TextContent, ImageContent};
use Mcp\Schema\PromptMessage;
use Mcp\Schema\Enum\Role;

#[McpPrompt]
public function analyzeImage(string $imageUrl, string $question): array
{
    $imageData = file_get_contents($imageUrl);
    
    return [
        new PromptMessage(Role::Assistant, [
            new TextContent('You are an image analysis expert.')
        ]),
        new PromptMessage(Role::User, [
            new TextContent($question),
            new ImageContent(
                data: base64_encode($imageData),
                mimeType: 'image/jpeg'
            )
        ])
    ];
}
```
完成提供程序

值列表完成```php
use Mcp\Capability\Attribute\{McpPrompt, CompletionProvider};

#[McpPrompt]
public function generateContent(
    #[CompletionProvider(values: ['blog', 'article', 'tutorial', 'guide'])]
    string $contentType,
    
    #[CompletionProvider(values: ['beginner', 'intermediate', 'advanced'])]
    string $difficulty
): array
{
    return [
        ['role' => 'user', 'content' => "Create a {$difficulty} level {$contentType}"]
    ];
}
```
### Enum Completion```php
enum Priority: string
{
    case LOW = 'low';
    case MEDIUM = 'medium';
    case HIGH = 'high';
}

enum Status
{
    case DRAFT;
    case PUBLISHED;
    case ARCHIVED;
}

#[McpResourceTemplate(uriTemplate: 'tasks/{taskId}')]
public function getTask(
    string $taskId,
    
    #[CompletionProvider(enum: Priority::class)]
    string $priority,
    
    #[CompletionProvider(enum: Status::class)]
    string $status
): array
{
    return $this->tasks[$taskId] ?? [];
}
```
自定义完成提供程序```php
use Mcp\Capability\Prompt\Completion\ProviderInterface;

class UserIdCompletionProvider implements ProviderInterface
{
    public function __construct(
        private DatabaseService $db
    ) {}

    public function getCompletions(string $currentValue): array
    {
        return $this->db->searchUserIds($currentValue);
    }
}

#[McpResourceTemplate(uriTemplate: 'user://{userId}/profile')]
public function getUserProfile(
    #[CompletionProvider(provider: UserIdCompletionProvider::class)]
    string $userId
): array
{
    return $this->users[$userId] ?? 
        throw new \InvalidArgumentException('User not found');
}
```
##传输选项

###演播室传输

对于命令行集成（默认）：```php
use Mcp\Server\Transport\StdioTransport;

$transport = new StdioTransport();
$server->run($transport);
```
HTTP传输

对于基于web的集成：```php
use Mcp\Server\Transport\StreamableHttpTransport;
use Nyholm\Psr7\Factory\Psr17Factory;

$psr17Factory = new Psr17Factory();

$request = $psr17Factory->createServerRequestFromGlobals();

$transport = new StreamableHttpTransport(
    $request,
    $psr17Factory,  // Response factory
    $psr17Factory   // Stream factory
);

$response = $server->run($transport);

// Send response in your web framework
foreach ($response->getHeaders() as $name => $values) {
    foreach ($values as $value) {
        header("$name: $value", false);
    }
}

http_response_code($response->getStatusCode());
echo $response->getBody();
```
##会话管理

内存会话（默认）```php
$server = Server::builder()
    ->setServerInfo('My Server', '1.0.0')
    ->setSession(ttl: 7200) // 2 hours
    ->build();
```
基于文件的会话```php
use Mcp\Server\Session\FileSessionStore;

$server = Server::builder()
    ->setServerInfo('My Server', '1.0.0')
    ->setSession(new FileSessionStore(__DIR__ . '/sessions'))
    ->build();
```
自定义会话存储```php
use Mcp\Server\Session\InMemorySessionStore;

$server = Server::builder()
    ->setServerInfo('My Server', '1.0.0')
    ->setSession(new InMemorySessionStore(3600))
    ->build();
```
##错误处理

工具中的异常处理```php
#[McpTool]
public function divideNumbers(float $a, float $b): float
{
    if ($b === 0.0) {
        throw new \InvalidArgumentException('Division by zero is not allowed');
    }
    
    return $a / $b;
}

#[McpTool]
public function processFile(string $filename): string
{
    if (!file_exists($filename)) {
        throw new \InvalidArgumentException("File not found: {$filename}");
    }
    
    if (!is_readable($filename)) {
        throw new \RuntimeException("File not readable: {$filename}");
    }
    
    return file_get_contents($filename);
}
```
自定义错误响应

SDK自动将异常转换为MCP客户端能够理解的JSON-RPC错误响应。

# #测试

PHPUnit test for Tools```php
<?php

namespace Tests;

use PHPUnit\Framework\TestCase;
use App\Tools\Calculator;

class CalculatorTest extends TestCase
{
    private Calculator $calculator;
    
    protected function setUp(): void
    {
        $this->calculator = new Calculator();
    }
    
    public function testAdd(): void
    {
        $result = $this->calculator->add(5, 3);
        $this->assertSame(8, $result);
    }
    
    public function testDivideByZero(): void
    {
        $this->expectException(\InvalidArgumentException::class);
        $this->expectExceptionMessage('Division by zero');
        
        $this->calculator->divide(10, 0);
    }
}
```
###测试服务器发现```php
public function testServerDiscoversTools(): void
{
    $server = Server::builder()
        ->setServerInfo('Test Server', '1.0.0')
        ->setDiscovery(__DIR__ . '/../src', ['.'])
        ->build();
    
    $capabilities = $server->getCapabilities();
    
    $this->assertArrayHasKey('tools', $capabilities);
    $this->assertNotEmpty($capabilities['tools']);
}
```
性能最佳实践

###使用发现缓存

在生产环境中始终使用缓存：```php
use Symfony\Component\Cache\Adapter\RedisAdapter;
use Symfony\Component\Cache\Psr16Cache;

$redis = new \Redis();
$redis->connect('127.0.0.1', 6379);

$cache = new Psr16Cache(new RedisAdapter($redis));

$server = Server::builder()
    ->setServerInfo('My Server', '1.0.0')
    ->setDiscovery(
        basePath: __DIR__,
        scanDirs: ['src'],
        excludeDirs: ['vendor', 'tests', 'var'],
        cache: $cache
    )
    ->build();
```
###优化扫描目录

只扫描必要的目录：```php
$server = Server::builder()
    ->setDiscovery(
        basePath: __DIR__,
        scanDirs: ['src/Tools', 'src/Resources'],  // Specific dirs
        excludeDirs: ['vendor', 'tests', 'var', 'cache']
    )
    ->build();
```
###使用OPcache

在生产环境中启用OPcache以获得更好的PHP性能：```ini
; php.ini
opcache.enable=1
opcache.memory_consumption=256
opcache.interned_strings_buffer=16
opcache.max_accelerated_files=10000
opcache.validate_timestamps=0
```
框架集成

Laravel集成```php
// app/Console/Commands/McpServer.php
namespace App\Console\Commands;

use Illuminate\Console\Command;
use Mcp\Server;
use Mcp\Server\Transport\StdioTransport;

class McpServer extends Command
{
    protected $signature = 'mcp:serve';
    protected $description = 'Start MCP server';
    
    public function handle()
    {
        $server = Server::builder()
            ->setServerInfo('Laravel MCP Server', '1.0.0')
            ->setDiscovery(app_path(), ['Tools', 'Resources'])
            ->build();
        
        $transport = new StdioTransport();
        $server->run($transport);
    }
}
```
Symfony集成```php
// Use symfony/mcp-bundle for native integration
composer require symfony/mcp-bundle
```
# #部署

Docker部署```dockerfile
FROM php:8.2-cli

# Install extensions
RUN docker-php-ext-install pdo pdo_mysql

# Install Composer
COPY --from=composer:latest /usr/bin/composer /usr/bin/composer

# Set working directory
WORKDIR /app

# Copy application
COPY . /app

# Install dependencies
RUN composer install --no-dev --optimize-autoloader

# Make server executable
RUN chmod +x /app/server.php

CMD ["php", "/app/server.php"]
```
Systemd服务```ini
[Unit]
Description=MCP PHP Server
After=network.target

[Service]
Type=simple
User=www-data
WorkingDirectory=/var/www/mcp-server
ExecStart=/usr/bin/php /var/www/mcp-server/server.php
Restart=always

[Install]
WantedBy=multi-user.target
```
MCP客户端配置

克劳德桌面配置```json
{
  "mcpServers": {
    "php-server": {
      "command": "php",
      "args": ["/absolute/path/to/server.php"]
    }
  }
}
```
MCP检查器测试```bash
npx @modelcontextprotocol/inspector php /path/to/server.php
```
##其他资源

-[官方PHP SDK库]（https://github.com/modelcontextprotocol/php-sdk）
- [MCP元件文档]（https://github.com/modelcontextprotocol/php-sdk/blob/main/docs/mcp-elements.md）
- [Server Builder文档]（https://github.com/modelcontextprotocol/php-sdk/blob/main/docs/server-builder.md）
-[运输文件]（https://github.com/modelcontextprotocol/php-sdk/blob/main/docs/transports.md）
(例子)(https://github.com/modelcontextprotocol/php-sdk/blob/main/docs/examples.md)
- [MCP规范]（https://spec.modelcontextprotocol.io/）
-[模型上下文协议]（https://modelcontextprotocol.io/）