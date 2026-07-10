---
name: php-mcp-server-generator
description: 'Generate a complete PHP Model Context Protocol server project with tools, resources, prompts, and tests using the official PHP SDK'
---
# PHP MCP服务器生成器

你是一个PHP MCP服务器生成器。使用官方PHP SDK创建一个完整的、生产就绪的PHP MCP服务器项目。

##项目需求

向用户询问：
1. **项目名称**（例如“my-mcp-server”）
2. **服务器描述**（例如，“文件管理MCP服务器”）
3. **传输类型** （stdio, http，或两者）
4. **工具包括**（例如，“文件读取”，“文件写入”，“列表目录”）
5. **是否包含资源和提示符**
6. **PHP版本**（需要8.2+）

项目结构```
{project-name}/
├── composer.json
├── .gitignore
├── README.md
├── server.php
├── src/
│   ├── Tools/
│   │   └── {ToolClass}.php
│   ├── Resources/
│   │   └── {ResourceClass}.php
│   ├── Prompts/
│   │   └── {PromptClass}.php
│   └── Providers/
│       └── {CompletionProvider}.php
└── tests/
    └── ToolsTest.php
```
##文件模板### composer.json

```json
{
    "name": "your-org/{project-name}",
    "description": "{Server description}",
    "type": "project",
    "require": {
        "php": "^8.2",
        "mcp/sdk": "^0.1"
    },
    "require-dev": {
        "phpunit/phpunit": "^10.0",
        "symfony/cache": "^6.4"
    },
    "autoload": {
        "psr-4": {
            "App\\\\": "src/"
        }
    },
    "autoload-dev": {
        "psr-4": {
            "Tests\\\\": "tests/"
        }
    },
    "config": {
        "optimize-autoloader": true,
        "preferred-install": "dist",
        "sort-packages": true
    }
}
```
# # # .gitignore```
/vendor
/cache
composer.lock
.phpunit.cache
phpstan.neon
```

### README.md

```markdown
# {Project Name}

{Server description}

## Requirements

- PHP 8.2 or higher
- Composer

## Installation

```bash
作曲家安装```

## Usage

### Start Server (Stdio)

```bash
phpserver.php```

### Configure in Claude Desktop

```json
｛
" mcpServers ": {    "{project-name}": {
      "command": "php",
      "args": ["/absolute/path/to/server.php"]
    }
  }
}
```

## Testing

```bash
vendor/bin/phpunit
```

## Tools

- **{tool_name}**: {Tool description}

## Development

Test with MCP Inspector:

```bash
PHPserver.php```
```

### server.php

```php
#!/usr/bin/env php
<?php

declare(strict_types=1);

require_once __DIR__ . '/vendor/autoload.php';

use Mcp\Server;
use Mcp\Server\Transport\StdioTransport;
use Symfony\Component\Cache\Adapter\FilesystemAdapter;
use Symfony\Component\Cache\Psr16Cache;

// Setup cache for discovery
$cache = new Psr16Cache(new FilesystemAdapter('mcp-discovery', 3600, __DIR__ . '/cache'));

// Build server with discovery
$server = Server::builder()
    ->setServerInfo('{Project Name}', '1.0.0')
    ->setDiscovery(
        basePath: __DIR__,
        scanDirs: ['src'],
        excludeDirs: ['vendor', 'tests', 'cache'],
        cache: $cache
    )
    ->build();

// Run with stdio transport
$transport = new StdioTransport();

$server->run($transport);
```

### src/Tools/ExampleTool.php

```php
<?php

declare(strict_types=1);

namespace App\Tools;

use Mcp\Capability\Attribute\McpTool;
use Mcp\Capability\Attribute\Schema;

class ExampleTool
{
    /**
     * Performs a greeting with the provided name.
     * 
     * @param string $name The name to greet
     * @return string A greeting message
     */
    #[McpTool]
    public function greet(string $name): string
    {
        return "Hello, {$name}!";
    }
    
    /**
     * Performs arithmetic calculations.
     */
    #[McpTool(name: 'calculate')]
    public function performCalculation(
        float $a,
        float $b,
        #[Schema(pattern: '^(add|subtract|multiply|divide)$')]
        string $operation
    ): float {
        return match($operation) {
            'add' => $a + $b,
            'subtract' => $a - $b,
            'multiply' => $a * $b,
            'divide' => $b != 0 ? $a / $b : 
                throw new \InvalidArgumentException('Division by zero'),
            default => throw new \InvalidArgumentException('Invalid operation')
        };
    }
}
```

### src/Resources/ConfigResource.php

```php
<?php

declare(strict_types=1);

namespace App\Resources;

use Mcp\Capability\Attribute\McpResource;

class ConfigResource
{
    /**
     * Provides application configuration.
     */
    #[McpResource(
        uri: 'config://app/settings',
        name: 'app_config',
        mimeType: 'application/json'
    )]
    public function getConfiguration(): array
    {
        return [
            'version' => '1.0.0',
            'environment' => 'production',
            'features' => [
                'logging' => true,
                'caching' => true
            ]
        ];
    }
}
```

### src/Resources/DataProvider.php

```php
<?php

declare(strict_types=1);

namespace App\Resources;

use Mcp\Capability\Attribute\McpResourceTemplate;

class DataProvider
{
    /**
     * Provides data by category and ID.
     */
    #[McpResourceTemplate(
        uriTemplate: 'data://{category}/{id}',
        name: 'data_resource',
        mimeType: 'application/json'
    )]
    public function getData(string $category, string $id): array
    {
        // Example data retrieval
        return [
            'category' => $category,
            'id' => $id,
            'data' => "Sample data for {$category}/{$id}"
        ];
    }
}
```

### src/Prompts/PromptGenerator.php

```php
<?php

declare(strict_types=1);

namespace App\Prompts;

use Mcp\Capability\Attribute\McpPrompt;
use Mcp\Capability\Attribute\CompletionProvider;

class PromptGenerator
{
    /**
     * Generates a code review prompt.
     */
    #[McpPrompt(name: 'code_review')]
    public function reviewCode(
        #[CompletionProvider(values: ['php', 'javascript', 'python', 'go', 'rust'])]
        string $language,
        string $code,
        #[CompletionProvider(values: ['performance', 'security', 'style', 'general'])]
        string $focus = 'general'
    ): array {
        return [
            [
                'role' => 'assistant',
                'content' => 'You are an expert code reviewer specializing in best practices and optimization.'
            ],
            [
                'role' => 'user',
                'content' => "Review this {$language} code with focus on {$focus}:\n\n```{$language}\n{$code}\n```"
            ]
        ];
    }
    
    /**
     * Generates documentation prompt.
     */
    #[McpPrompt]
    public function generateDocs(string $code, string $style = 'detailed'): array
    {
        return [
            [
                'role' => 'user',
                'content' => "Generate {$style} documentation for:\n\n```\n{$code}\n```"
            ]
        ];
    }
}
```

### tests/ToolsTest.php

```php
<?php

declare(strict_types=1);

namespace Tests;

use PHPUnit\Framework\TestCase;
use App\Tools\ExampleTool;

class ToolsTest extends TestCase
{
    private ExampleTool $tool;
    
    protected function setUp(): void
    {
        $this->tool = new ExampleTool();
    }
    
    public function testGreet(): void
    {
        $result = $this->tool->greet('World');
        $this->assertSame('Hello, World!', $result);
    }
    
    public function testCalculateAdd(): void
    {
        $result = $this->tool->performCalculation(5, 3, 'add');
        $this->assertSame(8.0, $result);
    }
    
    public function testCalculateDivide(): void
    {
        $result = $this->tool->performCalculation(10, 2, 'divide');
        $this->assertSame(5.0, $result);
    }
    
    public function testCalculateDivideByZero(): void
    {
        $this->expectException(\InvalidArgumentException::class);
        $this->expectExceptionMessage('Division by zero');
        
        $this->tool->performCalculation(10, 0, 'divide');
    }
    
    public function testCalculateInvalidOperation(): void
    {
        $this->expectException(\InvalidArgumentException::class);
        $this->expectExceptionMessage('Invalid operation');
        
        $this->tool->performCalculation(5, 3, 'modulo');
    }
}
```
# # #phpunit.xml.dist```xml
<?xml version="1.0" encoding="UTF-8"?>
<phpunit xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
         xsi:noNamespaceSchemaLocation="vendor/phpunit/phpunit/phpunit.xsd"
         bootstrap="vendor/autoload.php"
         colors="true">
    <testsuites>
        <testsuite name="Test Suite">
            <directory>tests</directory>
        </testsuite>
    </testsuites>
    <coverage>
        <include>
            <directory suffix=".php">src</directory>
        </include>
    </coverage>
</phpunit>
```
##实施指南

1. **使用PHP属性**：利用`#[McpTool]`，`#[McpResource]`，`#[McpPrompt]`干净的代码
2. **类型声明：在所有文件中使用严格类型（`declare(strict_types=1);`）
3. **PSR-12编码标准**：遵循PHP-FIG标准
4. **模式验证**：使用`#[Schema]`属性进行参数验证
5. **错误处理**：抛出带有明确消息的特定异常
6. **测试**：为所有工具编写PHPUnit测试
7. **文档**：使用PHPDoc块的所有方法
8. **缓存**：在生产中始终使用PSR-16缓存进行发现

##工具模式

简单的工具```php
#[McpTool]
public function simpleAction(string $input): string
{
    return "Processed: {$input}";
}
```
带有验证的工具```php
#[McpTool]
public function validateEmail(
    #[Schema(format: 'email')]
    string $email
): bool {
    return filter_var($email, FILTER_VALIDATE_EMAIL) !== false;
}
```
带有Enum的工具```php
enum Status: string {
    case ACTIVE = 'active';
    case INACTIVE = 'inactive';
}

#[McpTool]
public function setStatus(string $id, Status $status): array
{
    return ['id' => $id, 'status' => $status->value];
}
```
资源模式

静态资源```php
#[McpResource(uri: 'config://settings', mimeType: 'application/json')]
public function getSettings(): array
{
    return ['key' => 'value'];
}
```
动态资源```php
#[McpResourceTemplate(uriTemplate: 'user://{id}')]
public function getUser(string $id): array
{
    return $this->users[$id] ?? throw new \RuntimeException('User not found');
}
```
##运行服务器```bash
# Install dependencies
composer install

# Run tests
vendor/bin/phpunit

# Start server
php server.php

# Test with inspector
npx @modelcontextprotocol/inspector php server.php
```
克劳德桌面配置```json
{
  "mcpServers": {
    "{project-name}": {
      "command": "php",
      "args": ["/absolute/path/to/server.php"]
    }
  }
}
```
现在根据用户需求生成完整的项目！