---
description: 'Best practices and patterns for building Model Context Protocol (MCP) servers in Ruby using the official MCP Ruby SDK gem.'
applyTo: "**/*.rb, **/Gemfile, **/*.gemspec, **/Rakefile"
---
# Ruby MCP服务器开发指南

在用Ruby构建MCP服务器时，请使用官方Ruby SDK遵循这些最佳实践和模式。

# #安装

将MCP gem添加到Gemfile中：```ruby
gem 'mcp'
```
然后运行:```bash
bundle install
```
##服务器设置

创建MCP服务器实例：```ruby
require 'mcp'

server = MCP::Server.new(
  name: 'my_server',
  version: '1.0.0'
)
```
##添加工具

使用类或块定义工具：

工具作为类```ruby
class GreetTool < MCP::Tool
  tool_name 'greet'
  description 'Generate a greeting message'
  
  input_schema(
    properties: {
      name: { type: 'string', description: 'Name to greet' }
    },
    required: ['name']
  )
  
  output_schema(
    properties: {
      message: { type: 'string' },
      timestamp: { type: 'string', format: 'date-time' }
    },
    required: ['message']
  )
  
  annotations(
    read_only_hint: true,
    idempotent_hint: true
  )
  
  def self.call(name:, server_context:)
    MCP::Tool::Response.new([{
      type: 'text',
      text: "Hello, #{name}! Welcome to MCP."
    }], structured_content: {
      message: "Hello, #{name}!",
      timestamp: Time.now.iso8601
    })
  end
end

server = MCP::Server.new(
  name: 'my_server',
  tools: [GreetTool]
)
```
###工具与块```ruby
server.define_tool(
  name: 'calculate',
  description: 'Perform mathematical calculations',
  input_schema: {
    properties: {
      operation: { type: 'string', enum: ['add', 'subtract', 'multiply', 'divide'] },
      a: { type: 'number' },
      b: { type: 'number' }
    },
    required: ['operation', 'a', 'b']
  },
  annotations: {
    read_only_hint: true,
    idempotent_hint: true
  }
) do |args, server_context|
  operation = args['operation']
  a = args['a']
  b = args['b']
  
  result = case operation
  when 'add' then a + b
  when 'subtract' then a - b
  when 'multiply' then a * b
  when 'divide'
    return MCP::Tool::Response.new([{ type: 'text', text: 'Division by zero' }], is_error: true) if b == 0
    a / b
  else
    return MCP::Tool::Response.new([{ type: 'text', text: "Unknown operation: #{operation}" }], is_error: true)
  end
  
  MCP::Tool::Response.new([{ type: 'text', text: "Result: #{result}" }])
end
```
##添加资源

定义数据访问的资源：```ruby
# Register resources
resource = MCP::Resource.new(
  uri: 'resource://data/example',
  name: 'example-data',
  description: 'Example resource data',
  mime_type: 'application/json'
)

server = MCP::Server.new(
  name: 'my_server',
  resources: [resource]
)

# Define read handler
server.resources_read_handler do |params|
  case params[:uri]
  when 'resource://data/example'
    [{
      uri: params[:uri],
      mimeType: 'application/json',
      text: { message: 'Example data', timestamp: Time.now }.to_json
    }]
  else
    raise "Unknown resource: #{params[:uri]}"
  end
end
```
##添加提示

定义提示模板：

提示为类```ruby
class CodeReviewPrompt < MCP::Prompt
  prompt_name 'code_review'
  description 'Generate a code review prompt'
  
  arguments [
    MCP::Prompt::Argument.new(
      name: 'language',
      description: 'Programming language',
      required: true
    ),
    MCP::Prompt::Argument.new(
      name: 'focus',
      description: 'Review focus area',
      required: false
    )
  ]
  
  def self.template(args, server_context:)
    language = args['language'] || 'Ruby'
    focus = args['focus'] || 'general quality'
    
    MCP::Prompt::Result.new(
      description: "Code review for #{language} with focus on #{focus}",
      messages: [
        MCP::Prompt::Message.new(
          role: 'user',
          content: MCP::Content::Text.new("Please review this #{language} code with focus on #{focus}.")
        ),
        MCP::Prompt::Message.new(
          role: 'assistant',
          content: MCP::Content::Text.new("I'll review the code focusing on #{focus}. Please share the code.")
        )
      ]
    )
  end
end

server = MCP::Server.new(
  name: 'my_server',
  prompts: [CodeReviewPrompt]
)
```
###提示Block```ruby
server.define_prompt(
  name: 'analyze',
  description: 'Analyze a topic',
  arguments: [
    MCP::Prompt::Argument.new(name: 'topic', description: 'Topic to analyze', required: true),
    MCP::Prompt::Argument.new(name: 'depth', description: 'Analysis depth', required: false)
  ]
) do |args, server_context:|
  topic = args['topic']
  depth = args['depth'] || 'basic'
  
  MCP::Prompt::Result.new(
    description: "Analysis of #{topic} at #{depth} level",
    messages: [
      MCP::Prompt::Message.new(
        role: 'user',
        content: MCP::Content::Text.new("Please analyze: #{topic}")
      ),
      MCP::Prompt::Message.new(
        role: 'assistant',
        content: MCP::Content::Text.new("I'll provide a #{depth} analysis of #{topic}")
      )
    ]
  )
end
```
##传输配置

###演播室传输

对于本地命令行应用程序：```ruby
require 'mcp'

server = MCP::Server.new(
  name: 'my_server',
  tools: [MyTool]
)

transport = MCP::Server::Transports::StdioTransport.new(server)
transport.open
```
HTTP传输（Rails）

对于Rails应用程序：```ruby
class McpController < ApplicationController
  def index
    server = MCP::Server.new(
      name: 'rails_server',
      version: '1.0.0',
      tools: [SomeTool],
      prompts: [MyPrompt],
      server_context: { user_id: current_user.id }
    )
    
    render json: server.handle_json(request.body.read)
  end
end
```
可流的HTTP传输

对于服务器发送的事件：```ruby
server = MCP::Server.new(name: 'my_server')
transport = MCP::Server::Transports::StreamableHTTPTransport.new(server)
server.transport = transport

# When tools change, notify clients
server.define_tool(name: 'new_tool') { |**args| { result: 'ok' } }
server.notify_tools_list_changed
```
##服务器

将上下文信息传递给工具和提示：```ruby
server = MCP::Server.new(
  name: 'my_server',
  tools: [AuthenticatedTool],
  server_context: {
    user_id: current_user.id,
    request_id: request.uuid,
    auth_token: session[:token]
  }
)

class AuthenticatedTool < MCP::Tool
  def self.call(query:, server_context:)
    user_id = server_context[:user_id]
    # Use user_id for authorization
    
    MCP::Tool::Response.new([{ type: 'text', text: 'Authorized' }])
  end
end
```
# #配置

异常报告

配置异常报告：```ruby
MCP.configure do |config|
  config.exception_reporter = ->(exception, server_context) {
    # Report to your error tracking service
    Bugsnag.notify(exception) do |report|
      report.add_metadata(:mcp, server_context)
    end
  }
end
```
# # #仪器

监控MCP服务器性能：```ruby
MCP.configure do |config|
  config.instrumentation_callback = ->(data) {
    # Log instrumentation data
    Rails.logger.info("MCP: #{data.inspect}")
    
    # Or send to metrics service
    StatsD.timing("mcp.#{data[:method]}.duration", data[:duration])
    StatsD.increment("mcp.#{data[:method]}.count")
  }
end
```
仪器数据包括：
-`method`：被调用的协议方法（例如，“tools/call”）
—`tool_name`：被调用的工具名称
—`prompt_name`：被调用的提示符名称
-`resource_uri`：被调用资源的URI
-`error`：查找失败时的错误代码
—`duration`：持续时间，单位为秒

协议版本

覆盖协议版本：```ruby
configuration = MCP::Configuration.new(protocol_version: '2025-06-18')
server = MCP::Server.new(name: 'my_server', configuration: configuration)
```
##工具注释

提供关于工具行为的元数据：```ruby
class DataTool < MCP::Tool
  annotations(
    read_only_hint: true,      # Tool only reads data
    destructive_hint: false,   # Tool doesn't destroy data
    idempotent_hint: true,     # Same input = same output
    open_world_hint: false     # Tool operates in closed context
  )
  
  def self.call(**args, server_context:)
    # Implementation
  end
end
```
##工具输出模式

定义预期的输出结构：```ruby
class WeatherTool < MCP::Tool
  output_schema(
    properties: {
      temperature: { type: 'number' },
      condition: { type: 'string' },
      humidity: { type: 'integer' }
    },
    required: ['temperature', 'condition']
  )
  
  def self.call(location:, server_context:)
    weather_data = {
      temperature: 72.5,
      condition: 'sunny',
      humidity: 45
    }
    
    # Validate against schema
    output_schema.validate_result(weather_data)
    
    MCP::Tool::Response.new(
      [{ type: 'text', text: weather_data.to_json }],
      structured_content: weather_data
    )
  end
end
```
回复中的结构化内容

返回带有文本的结构化数据：```ruby
class APITool < MCP::Tool
  def self.call(endpoint:, server_context:)
    api_data = call_api(endpoint)
    
    MCP::Tool::Response.new(
      [{ type: 'text', text: api_data.to_json }],
      structured_content: api_data
    )
  end
end
```
自定义方法

定义自定义JSON-RPC方法：```ruby
server = MCP::Server.new(name: 'my_server')

# Custom method with result
server.define_custom_method(method_name: 'add') do |params|
  params[:a] + params[:b]
end

# Custom notification (returns nil)
server.define_custom_method(method_name: 'notify') do |params|
  puts "Notification: #{params[:message]}"
  nil
end
```
# #通知

发送列表更改通知：```ruby
server = MCP::Server.new(name: 'my_server')
transport = MCP::Server::Transports::StreamableHTTPTransport.new(server)
server.transport = transport

# Notify when tools change
server.define_tool(name: 'new_tool') { |**args| { result: 'ok' } }
server.notify_tools_list_changed

# Notify when prompts change
server.define_prompt(name: 'new_prompt') { |args, **_| MCP::Prompt::Result.new(...) }
server.notify_prompts_list_changed

# Notify when resources change
server.notify_resources_list_changed
```
##资源模板

用URI模板定义动态资源：```ruby
resource_template = MCP::ResourceTemplate.new(
  uri_template: 'users://{user_id}/profile',
  name: 'user-profile',
  description: 'User profile data',
  mime_type: 'application/json'
)

server = MCP::Server.new(
  name: 'my_server',
  resource_templates: [resource_template]
)
```
##错误处理

在工具中正确处理错误：```ruby
class RiskyTool < MCP::Tool
  def self.call(data:, server_context:)
    begin
      result = risky_operation(data)
      MCP::Tool::Response.new([{ type: 'text', text: result }])
    rescue ValidationError => e
      MCP::Tool::Response.new(
        [{ type: 'text', text: "Invalid input: #{e.message}" }],
        is_error: true
      )
    rescue => e
      # Will be caught and reported by exception_reporter
      raise
    end
  end
end
```
# #测试

为您的MCP服务器编写测试：```ruby
require 'minitest/autorun'
require 'mcp'

class MyToolTest < Minitest::Test
  def test_greet_tool
    response = GreetTool.call(name: 'Ruby', server_context: {})
    
    assert_equal 1, response.content.length
    assert_match(/Ruby/, response.content.first[:text])
    refute response.is_error
  end
  
  def test_invalid_input
    response = CalculateTool.call(operation: 'divide', a: 10, b: 0, server_context: {})
    
    assert response.is_error
  end
end
```
客户端使用情况

构建MCP客户端连接到服务器：```ruby
require 'mcp'
require 'faraday'

# HTTP transport
http_transport = MCP::Client::HTTP.new(
  url: 'https://api.example.com/mcp',
  headers: { 'Authorization' => "Bearer #{token}" }
)

client = MCP::Client.new(transport: http_transport)

# List tools
tools = client.tools
tools.each do |tool|
  puts "Tool: #{tool.name}"
  puts "Description: #{tool.description}"
end

# Call a tool
response = client.call_tool(
  tool: tools.first,
  arguments: { message: 'Hello, world!' }
)
```
最佳实践

1. **为复杂的工具使用类** -更好的组织和可测试性
2. **定义input/output模式** -确保类型安全和验证
3. **添加注释** -帮助客户理解工具的行为
4. **包含结构化内容** -同时提供文本和结构化数据
5. **使用server_context** -通过认证和请求上下文
6. **配置异常报告** -监控生产环境中的错误
7. **实施工具** -跟踪性能指标
8. **发送通知** -保持客户端更新的变化
9. **验证输入** -处理前检查参数
10. **遵循Ruby约定** -使用snake_case，适当缩进

##常见模式

###认证工具```ruby
class AuthenticatedTool < MCP::Tool
  def self.call(**args, server_context:)
    user_id = server_context[:user_id]
    raise 'Unauthorized' unless user_id
    
    # Process authenticated request
    MCP::Tool::Response.new([{ type: 'text', text: 'Success' }])
  end
end
```
###分页资源```ruby
server.resources_read_handler do |params|
  uri = params[:uri]
  page = params[:page] || 1
  
  data = fetch_paginated_data(page)
  
  [{
    uri: uri,
    mimeType: 'application/json',
    text: data.to_json
  }]
end
```
动态提示```ruby
class DynamicPrompt < MCP::Prompt
  def self.template(args, server_context:)
    user_id = server_context[:user_id]
    user_data = User.find(user_id)
    
    MCP::Prompt::Result.new(
      description: "Personalized prompt for #{user_data.name}",
      messages: generate_messages_for(user_data)
    )
  end
end
```
