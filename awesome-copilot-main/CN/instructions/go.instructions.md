---
description: 'Instructions for writing Go code following idiomatic Go practices and community standards'
applyTo: '**/*.go,**/go.mod,**/go.sum'
---
# Go开发指南

在编写Go代码时遵循惯用的Go实践和社区标准。这些指令是基于[Effective Go](https://go.dev/doc/effective_go), [Go Code Review Comments](https://go.dev/wiki/CodeReviewComments)，和[b谷歌's Go Style Guide]（https://google.github.io/styleguide/go/）。

##一般使用说明-编写简单，清晰，习惯的Go代码
-比起聪明，更喜欢清晰和简单
-遵循最少意外原则
保持快乐路径左对齐（尽量减少缩进）
-尽早返回以减少嵌套
-比起if-else链，更倾向于尽早返回；使用`if condition { return }`模式来避免else块
-使零值有用
-用清晰、描述性的名称编写自文档代码
—导出的文档类型、函数、方法和包
-使用Go模块进行依赖管理
-利用Go标准库而不是重新发明轮子（例如，使用`strings.Builder`用于字符串连接，`filepath.Join`用于路径构建）
-当功能存在时，首选标准库解决方案而不是自定义实现
-默认用英文写评论；只在用户要求时翻译
避免在代码和注释中使用表情符号

命名约定

# # #包—使用小写的单字包名
—避免使用下划线、连字符或混合大写字母
-选择描述包提供的内容的名称，而不是包包含的内容
避免使用像`util`、`common`或`base`这样的通用名称
-包名应该是单数，而不是复数####包声明规则（关键）：
- **永远不要重复`package`声明** -每个Go文件必须只有一个`package`行
—编辑已存在的`.go`文件时：
- **保留**现有的`package`声明-不添加另一个
—如果需要替换整个文件内容，请从已有的包名开始
—创建新的`.go`文件时：
在编写任何代码之前，请检查同一目录下的其他`.go`文件使用的包名
—使用与该目录下已有文件相同的包名
—如果是新建目录，则使用目录名作为包名
-写**正好一个**`package <name>`行在文件的最顶部
—使用文件创建或替换工具时：
- **在添加一个`package`声明之前，总是验证**目标文件没有已经有一个`package`声明
—如果替换文件内容，只包含一个`package`decLaration in new content
- **永远不要**创建多个`package`行或重复声明的文件变量和函数

—使用mixedCaps或mixedCaps (camelCase)，而不是下划线
-保持名字简短但具有描述性
-仅在非常短的范围内使用单字母变量（如循环索引）
—导出的名称以大写字母开头
—未导出的名称以小写字母开头
-避免口吃（例如，避免`http.HTTPServer`，更喜欢`http.Server`）

# # #接口

-尽可能使用-er后缀命名接口（例如，`Reader`,`Writer`,`Formatter`）
-单方法接口应以方法命名（例如：`Read`→`Reader`）
-保持界面小而集中

# # #常量

-使用MixedCaps输出常量
-对未导出的常量使用mixedCaps
-使用`const`块对相关常量进行分组
-考虑使用类型常量以获得更好的类型安全性

代码样式和格式

# # #格式化—始终使用`gofmt`来格式化代码
—使用“`goimports`”自动管理导入
-保持行长度合理（没有硬性限制，但要考虑可读性）
-添加空白行来分隔代码的逻辑组

# # #评论-努力实现自文档化代码；比起注释，更喜欢清晰的变量名、函数名和代码结构
-只在需要解释复杂逻辑、业务规则或非明显行为时才写注释
-默认用英文写完整的句子
-只在特定用户要求时将评论翻译成其他语言
-以被描述事物的名称开始句子
-包注释应该以“Package [name]”开头
-大多数注释使用行注释（`//`）
-谨慎使用块注释（`/* */`），主要用于包文档
-记录“为什么”，而不是“什么”，除非“什么”很复杂
-避免在注释和代码中使用表情符号

错误处理—在函数调用后立即检查错误
不要忽略使用`_`的错误，除非你有一个很好的理由（记录为什么）
-使用`fmt.Errorf`和`%w`动词将错误与上下文包装起来
—当需要检查特定错误时，创建自定义错误类型
—将错误返回作为最后一个返回值
—将错误变量命名为`err`-错误信息要小写，不要以标点符号结尾

体系结构和项目结构

###包装组织

-遵循标准的Go项目布局约定
—“`main`”软件包保存在“`cmd/`”目录下
—将可重复使用的包放在`pkg/`或`internal/`中
—对于不应该被外部项目导入的包，使用`internal/`-将相关功能分组到包中
-避免循环依赖

依赖管理-使用Go模块（`go.mod`和`go.sum`）
保持最小的依赖关系
—定期更新安全补丁依赖项
-使用`go mod tidy`清理未使用的依赖项
-仅在必要时依赖供应商

类型安全和语言特性

类型定义

-定义类型以增加意义和类型安全性
-对JSON， XML，数据库映射使用struct标签
-首选显式类型转换
—谨慎使用类型断言，并检查第二个返回值
-更喜欢泛型而不是无约束类型；当确实需要不受约束的类型时，使用预先声明的别名`any`而不是`interface{}`（Go 1.18+）

指针vs值-在大型结构体或需要修改接收器时使用指针接收器
-在小结构体和需要不变性时使用值接收器
—当需要修改实参或大型结构体时，使用指针形参
—对于小型结构体和希望防止修改的情况，使用值形参
-在类型的方法集中保持一致
-在选择指针和值接收器时考虑零值

接口和组合

-接受接口，返回具体类型
保持接口小（1-3个方法是理想的）
-使用嵌入合成
-在使用接口的地方附近定义接口，而不是在实现接口的地方
—除非必要，否则不要导出接口

# #并发

# # #了goroutine-在库中创建程序时要谨慎；最好让调用方控制并发性
-如果你必须在库中创建程序，提供清晰的文档和清理机制
-总是知道一个程序将如何退出
—使用`sync.WaitGroup`或通道等待例程
-通过确保清理来避免常规泄漏

# # #通道

—使用通道在程序间通信
-不要通过共享内存进行交流；通过通信共享内存
-关闭发送方的通道，而不是接收方
—当您知道容量时，使用缓冲通道
—非阻塞操作使用`select`# # #同步—使用“`sync.Mutex`”保护共享状态
保持临界区较小
—读卡器多时使用`sync.RWMutex`—根据用例选择通道和互斥锁：使用通道进行通信，使用互斥锁进行状态保护
—使用`sync.Once`进行一次性初始化
- WaitGroup使用的Go版本：	- If `go >= 1.25` in `go.mod`, use the new `WaitGroup.Go` method ([documentation](https://pkg.go.dev/sync#WaitGroup)):
		```go
		var wg sync.WaitGroup
		wg.Go(task1)
		wg.Go(task2)
		wg.Wait()
		```
	- If `go < 1.25`, use the classic `Add`/`Done` pattern
错误处理模式

###创建错误

—使用`errors.New`表示简单的静态错误
—使用`fmt.Errorf`表示动态错误
—为特定于域的错误创建自定义错误类型
—导出前哨错误的错误变量
—使用`errors.Is`和`errors.As`进行错误检查

错误传播

-在堆栈上传播错误时添加上下文
-不记录和返回错误（选择一个）
—在适当的级别处理错误
-考虑使用结构化错误进行更好的调试

## API设计

HTTP处理程序

-使用`http.HandlerFunc`简单的处理程序
为需要状态的处理程序实现`http.Handler`-对横切关注点使用中间件
—设置合适的状态码和报头
-优雅地处理错误，并返回适当的错误响应
-路由器使用的Go版本：	- If `go >= 1.22`, prefer the enhanced `net/http` `ServeMux` with pattern-based routing and method matching
	- If `go < 1.22`, use the classic `ServeMux` and handle methods/paths manually (or use a third-party router when justified)
JSON api

-使用struct标签来控制JSON封送
-验证输入数据
—对可选字段使用指针
—考虑使用`json.RawMessage`进行延迟解析
-妥善处理JSON错误

HTTP客户端保持客户端结构只关注配置和依赖关系（例如，基本URL，`*http.Client`, auth，默认标头）。它不能存储任何每个请求的状态
-不要在客户端结构中存储或缓存`*http.Request`，也不要跨调用持久化特定于请求的状态；相反，应该为每个方法调用构造一个新的请求
方法应该接受`context.Context`和输入参数，在本地组装`*http.Request`（或通过每次调用创建的短期builder/helper），然后调用`c.httpClient.Do(req)`-如果请求构建逻辑被重用，将其纳入未导出的辅助函数或每个调用的构建器类型；永远不要将`http.Request`（URL参数、正文、报头）作为长期客户端的字段
-确保底层的`*http.Client`被配置（超时，传输）并且是安全的并发使用；第一次使用后避免变异`Transport`-总是在你发送的请求实例上设置报头，并关闭响应体（`defer resp.Body.Close()`），适当地处理错误性能优化

内存管理

—减少热路径的分配
-尽可能重用对象（考虑`sync.Pool`）
-对小结构使用值接收器
-当大小已知时，预分配切片
—避免不必要的字符串转换

###I/O：读取和缓冲区

大多数`io.Reader`流都是一次性消费的阅读促进状态。不要假定阅读器可以在没有特殊处理的情况下重新读取
-如果你必须多次读取数据，缓冲一次，并根据需要重新创建读取器。	- Use `io.ReadAll` (or a limited read) to obtain `[]byte`, then create fresh readers via `bytes.NewReader(buf)` or `bytes.NewBuffer(buf)` for each reuse
	- For strings, use `strings.NewReader(s)`; you can `Seek(0, io.SeekStart)` on `*bytes.Reader` to rewind
—对于HTTP请求，不要重用已使用的`req.Body`。而不是:	- Keep the original payload as `[]byte` and set `req.Body = io.NopCloser(bytes.NewReader(buf))` before each send
	- Prefer configuring `req.GetBody` so the transport can recreate the body for redirects/retries: `req.GetBody = func() (io.ReadCloser, error) { return io.NopCloser(bytes.NewReader(buf)), nil }`
—要在读取时复制流，请使用`io.TeeReader`（在通过时复制到缓冲区）或使用`io.MultiWriter`写入多个sink
-重用缓存的读取器：调用`(*bufio.Reader).Reset(r)`来附加到一个新的底层读取器；不要指望它“倒带”，除非来源支持查找
-对于较大的有效载荷，避免无限制的缓冲；考虑流、`io.LimitReader`或磁盘上的临时存储来控制内存

-使用`io.Pipe`流没有缓冲整个有效载荷：	- Write to `*io.PipeWriter` in a separate goroutine while the reader consumes
	- Always close the writer; use `CloseWithError(err)` on failures
	- `io.Pipe` is for streaming, not rewinding or making readers reusable
—**警告：**当使用`io.Pipe`时（特别是使用多部分写器时），所有写操作必须严格按照顺序执行。不要并发写或乱序写——必须保留多部分边界和块顺序。乱序或并行写会破坏流并导致错误。

-流multipart/form-data与`io.Pipe`：	- `pr, pw := io.Pipe()`; `mw := multipart.NewWriter(pw)`; use `pr` as the HTTP request body
	- Set `Content-Type` to `mw.FormDataContentType()`
	- In a goroutine: write all parts to `mw` in the correct order; on error `pw.CloseWithError(err)`; on success `mw.Close()` then `pw.Close()`
	- Do not store request/in-flight form state on a long-lived client; build per call
	- Streamed bodies are not rewindable; for retries/redirects, buffer small payloads or provide `GetBody`
# # #分析

-使用内置的分析工具（`pprof`）
-基准测试关键代码路径
-优化前的配置文件
-首先关注算法改进
-考虑使用`testing.B`作为基准测试

# #测试

测试组织

-将测试保持在同一个包中（白盒测试）
—使用`_test`包后缀进行黑盒测试
—以“`_test.go`”后缀命名测试文件
-将测试文件放在他们测试的代码旁边

编写测试

-对多个测试用例使用表驱动测试
-使用`Test_functionName_scenario`对测试进行描述性命名
-使用`t.Run`的子测试以更好地组织
—测试成功案例和错误案例
-考虑使用`testify`或类似的库，当它们增加价值时，但不要使简单的测试过于复杂

###测试助手—用`t.Helper()`标记helper函数
-为复杂的设置创建测试夹具
-使用`testing.TB`接口用于测试和基准测试中使用的函数
—使用`t.Cleanup()`清理资源

安全最佳实践

输入验证

-验证所有外部输入
—使用强类型防止无效状态
—在SQL查询中使用前对数据进行清理
—注意用户输入的文件路径
-为不同的上下文（HTML， SQL, shell）验证和转义数据

# # #密码学

—使用标准库加密包
-不要实现自己的加密
—使用crypto/rand生成随机数
-使用bcrypt、scrypt或argon2存储密码（考虑使用golang.org/x/crypto作为其他选项）
—使用TLS协议进行网络通信

# #文档

###代码文档-通过清晰的命名和结构优先考虑自文档代码
—将导出的所有符号记录下来，说明清楚、简明
-以符号名开始文档
-默认使用英文编写文档
-在文档中使用有用的示例
保持文档和代码的一致性
-当代码更改时更新文档
-避免在文档和注释中使用表情符号

### README和文档文件

-包括清晰的安装说明
-文档依赖关系和需求
-提供用法示例
-文档配置选项
-包括故障排除部分

##工具和开发工作流

基本工具

—`go fmt`：格式代码
-`go vet`：查找可疑结构
-`golangci-lint`：额外的检查（不赞成使用golint）
—`go test`：运行测试
—`go mod`：管理依赖关系
—`go generate`：代码生成

开发实践—在提交前运行测试
-使用预提交钩子进行格式化和检查
保持提交的重点和原子性
—写有意义的提交消息
-提交前检查差异

要避免的常见陷阱

-不检查错误
-忽略竞争条件
-造成常规泄漏
-不使用defer进行清理
-同时修改地图
-不理解nil接口和nil指针
忘记关闭资源（文件、连接）
-不必要地使用全局变量
-过度使用无约束类型（例如，`any`）；首选带有约束的特定类型或泛型类型参数。如果需要不受约束的类型，请使用`any`而不是`interface{}`-不考虑类型的零值
**创建重复的`package`声明** -这是一个编译错误；在添加包声明之前，始终检查现有文件