---
description: 'Best practices and patterns for building Model Context Protocol (MCP) servers in Go using the official github.com/modelcontextprotocol/go-sdk package.'
applyTo: "**/*.go, **/go.mod, **/go.sum"
---
# Go MCP服务器开发指南

在Go中构建MCP服务器时，请使用官方Go SDK遵循这些最佳实践和模式。

##服务器设置

使用`mcp.NewServer`创建MCP服务器。```go
import "github.com/modelcontextprotocol/go-sdk/mcp"

server := mcp.NewServer(
    &mcp.Implementation{
        Name:    "my-server",
        Version: "v1.0.0",
    },
    nil, // or provide mcp.Options
)
```
##添加工具

为了类型安全，使用`mcp.AddTool`和基于结构的输入和输出：```go
type ToolInput struct {
    Query string `json:"query" jsonschema:"the search query"`
    Limit int    `json:"limit,omitempty" jsonschema:"maximum results to return"`
}

type ToolOutput struct {
    Results []string `json:"results" jsonschema:"list of search results"`
    Count   int      `json:"count" jsonschema:"number of results found"`
}

func SearchTool(ctx context.Context, req *mcp.CallToolRequest, input ToolInput) (
    *mcp.CallToolResult,
    ToolOutput,
    error,
) {
    // Implement tool logic
    results := performSearch(ctx, input.Query, input.Limit)
    
    return nil, ToolOutput{
        Results: results,
        Count:   len(results),
    }, nil
}

// Register the tool
mcp.AddTool(server, 
    &mcp.Tool{
        Name:        "search",
        Description: "Search for information",
    },
    SearchTool,
)
```
##添加资源

使用`mcp.AddResource`提供可访问的数据：```go
func GetResource(ctx context.Context, req *mcp.ReadResourceRequest) (*mcp.ReadResourceResult, error) {
    content, err := loadResourceContent(ctx, req.URI)
    if err != nil {
        return nil, err
    }
    
    return &mcp.ReadResourceResult{
        Contents: []any{
            &mcp.TextResourceContents{
                ResourceContents: mcp.ResourceContents{
                    URI:      req.URI,
                    MIMEType: "text/plain",
                },
                Text: content,
            },
        },
    }, nil
}

mcp.AddResource(server,
    &mcp.Resource{
        URI:         "file:///data/example.txt",
        Name:        "Example Data",
        Description: "Example resource data",
        MIMEType:    "text/plain",
    },
    GetResource,
)
```
##添加提示

使用`mcp.AddPrompt`作为可重用的提示模板：```go
type PromptInput struct {
    Topic string `json:"topic" jsonschema:"the topic to analyze"`
}

func AnalyzePrompt(ctx context.Context, req *mcp.GetPromptRequest, input PromptInput) (
    *mcp.GetPromptResult,
    error,
) {
    return &mcp.GetPromptResult{
        Description: "Analyze the given topic",
        Messages: []mcp.PromptMessage{
            {
                Role: mcp.RoleUser,
                Content: mcp.TextContent{
                    Text: fmt.Sprintf("Analyze this topic: %s", input.Topic),
                },
            },
        },
    }, nil
}

mcp.AddPrompt(server,
    &mcp.Prompt{
        Name:        "analyze",
        Description: "Analyze a topic",
        Arguments: []mcp.PromptArgument{
            {
                Name:        "topic",
                Description: "The topic to analyze",
                Required:    true,
            },
        },
    },
    AnalyzePrompt,
)
```
##传输配置

###演播室传输

对于通过stdin/stdout进行通信（最常见的桌面集成）：```go
if err := server.Run(ctx, &mcp.StdioTransport{}); err != nil {
    log.Fatal(err)
}
```
HTTP传输

对于基于http的通信：```go
import "github.com/modelcontextprotocol/go-sdk/mcp"

transport := &mcp.HTTPTransport{
    Addr: ":8080",
    // Optional: configure TLS, timeouts, etc.
}

if err := server.Run(ctx, transport); err != nil {
    log.Fatal(err)
}
```
##错误处理

总是返回正确的错误并使用上下文来取消：```go
func MyTool(ctx context.Context, req *mcp.CallToolRequest, input MyInput) (
    *mcp.CallToolResult,
    MyOutput,
    error,
) {
    // Check context cancellation
    if ctx.Err() != nil {
        return nil, MyOutput{}, ctx.Err()
    }
    
    // Return errors for invalid input
    if input.Query == "" {
        return nil, MyOutput{}, fmt.Errorf("query cannot be empty")
    }
    
    // Perform operation
    result, err := performOperation(ctx, input)
    if err != nil {
        return nil, MyOutput{}, fmt.Errorf("operation failed: %w", err)
    }
    
    return nil, result, nil
}
```
JSON模式标签

使用`jsonschema`标签来记录你的结构，以获得更好的客户端集成：```go
type Input struct {
    Name     string   `json:"name" jsonschema:"required,description=User's name"`
    Age      int      `json:"age" jsonschema:"minimum=0,maximum=150"`
    Email    string   `json:"email,omitempty" jsonschema:"format=email"`
    Tags     []string `json:"tags,omitempty" jsonschema:"uniqueItems=true"`
    Active   bool     `json:"active" jsonschema:"default=true"`
}
```
##背景信息使用

始终尊重上下文取消和截止日期：```go
func LongRunningTool(ctx context.Context, req *mcp.CallToolRequest, input Input) (
    *mcp.CallToolResult,
    Output,
    error,
) {
    select {
    case <-ctx.Done():
        return nil, Output{}, ctx.Err()
    case result := <-performWork(ctx, input):
        return nil, result, nil
    }
}
```
##服务器选项

使用选项配置服务器行为：```go
options := &mcp.Options{
    Capabilities: &mcp.ServerCapabilities{
        Tools:     &mcp.ToolsCapability{},
        Resources: &mcp.ResourcesCapability{
            Subscribe: true, // Enable resource subscriptions
        },
        Prompts: &mcp.PromptsCapability{},
    },
}

server := mcp.NewServer(
    &mcp.Implementation{Name: "my-server", Version: "v1.0.0"},
    options,
)
```
# #测试

使用标准Go测试模式测试你的MCP工具：```go
func TestSearchTool(t *testing.T) {
    ctx := context.Background()
    input := ToolInput{Query: "test", Limit: 10}
    
    result, output, err := SearchTool(ctx, nil, input)
    if err != nil {
        t.Fatalf("SearchTool failed: %v", err)
    }
    
    if len(output.Results) == 0 {
        t.Error("Expected results, got none")
    }
}
```
##模块设置

正确初始化Go模块：```bash
go mod init github.com/yourusername/yourserver
go get github.com/modelcontextprotocol/go-sdk@latest
```
你的`go.mod`应该包括：```go
module github.com/yourusername/yourserver

go 1.23

require github.com/modelcontextprotocol/go-sdk v1.0.0
```
##常见模式

# # #日志

使用结构化日志：```go
import "log/slog"

logger := slog.Default()
logger.Info("tool called", "name", req.Params.Name, "args", req.Params.Arguments)
```
# # #配置

使用环境变量或配置文件：```go
type Config struct {
    ServerName string
    Version    string
    Port       int
}

func LoadConfig() *Config {
    return &Config{
        ServerName: getEnv("SERVER_NAME", "my-server"),
        Version:    getEnv("VERSION", "v1.0.0"),
        Port:       getEnvInt("PORT", 8080),
    }
}
```
###优雅关机

正确处理关机信号：```go
ctx, cancel := context.WithCancel(context.Background())
defer cancel()

sigCh := make(chan os.Signal, 1)
signal.Notify(sigCh, os.Interrupt, syscall.SIGTERM)

go func() {
    <-sigCh
    cancel()
}()

if err := server.Run(ctx, transport); err != nil {
    log.Fatal(err)
}
```
