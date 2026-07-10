# Runnable Recipe Examples

此文件夹包含每个食谱食谱的独立的、可执行的Go示例。每个文件都是一个完整的程序，可以直接使用`go run`运行。

# #先决条件

—1.21及以上版本
-GitHub CopilotSDK for Go```bash
go get github.com/github/copilot-sdk/go
```
##运行示例

每个`.go`文件都是一个完整的、可运行的程序。简单的使用方法:```bash
go run <filename>.go
```
可用的食谱

|配方|命令|描述|| -------------------- | -------------------------------- | ------------------------------------------ |
|错误处理|`go run error-handling.go`|演示错误处理模式|
|多会话|`go run multiple-sessions.go`|管理多个独立会话|
|本地文件管理|`go run managing-local-files.go`|通过AI分组|对文件进行组织
| PR可视化|`go run pr-visualization.go`|生成PR年龄图表|
|持久化会话|`go run persisting-sessions.go`|跨重启保存和恢复会话|

带参数的例子

**PR可视化与特定的回购：**```bash
go run pr-visualization.go -repo github/copilot-sdk
```
**管理本地文件（编辑文件以更改目标文件夹）：**```bash
# Edit the targetFolder variable in managing-local-files.go first
go run managing-local-files.go
```
##采用最佳实践

以下示例遵循Go约定：

-正确的错误处理与显式检查
-使用`defer`进行清理
-习惯命名（局部变量的camelCase）
-适当时使用标准库
-清晰的关注点分离

学习资源

- [Go文档]（https://go.dev/doc/）
- [GitHub CopilotSDK for Go]（https://github.com/github/copilot-sdk/blob/main/go/README.md）
-[家长食谱]（../README.md）