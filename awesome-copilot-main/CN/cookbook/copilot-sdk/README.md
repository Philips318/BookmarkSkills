#GitHub CopilotSDK Cookbook

这本食谱收集了一些重点突出的小食谱，展示了如何跨语言使用GitHub CopilotSDK完成常见任务。每个配方都有意设计得简短实用，带有可复制粘贴的片段和指向更完整示例和测试的指针。

按语言分类的食谱

# # #。净(c#)- [Ralph Loop](dotnet/ralph-loop.md)：构建具有每次迭代新鲜上下文的自主AI编码循环，planning/building模式和背压。
—[错误处理](dotnet/error-handling.md)：优雅地处理错误，包括连接失败、超时和清理。
—[Multiple Sessions](dotnet/multiple-sessions.md)：同时管理多个独立会话。
-[管理本地文件](dotnet/managing-local-files.md)：使用人工智能分组策略通过元数据组织文件。
-[公关可视化](dotnet/pr-visualization.md)：生成交互式公关年龄图表使用GitHub MCP服务器。
—[persistent Sessions](dotnet/persisting-sessions.md)：保存和恢复会话。
—[无障碍报告](dotnet/accessibility-report.md)：使用剧作家MCP服务器生成WCAG无障碍报告。

###Node.js/ TypeScript- [Ralph Loop](nodejs/ralph-loop.md)：构建具有每次迭代新鲜上下文的自主AI编码循环，planning/building模式和背压。
—[错误处理](nodejs/error-handling.md)：优雅地处理错误，包括连接失败、超时和清理。
—[Multiple Sessions](nodejs/multiple-sessions.md)：同时管理多个独立会话。
-[管理本地文件](nodejs/managing-local-files.md)：使用人工智能分组策略通过元数据组织文件。
-[公关可视化](nodejs/pr-visualization.md)：生成交互式公关年龄图表使用GitHub MCP服务器。
—[persistent Sessions](nodejs/persisting-sessions.md)：保存和恢复会话。
—[无障碍报告](nodejs/accessibility-report.md)：使用剧作家MCP服务器生成WCAG无障碍报告。

# # # Python- [Ralph Loop](python/ralph-loop.md)：构建具有每次迭代新鲜上下文的自主AI编码循环，planning/building模式和背压。
—[错误处理](python/error-handling.md)：优雅地处理错误，包括连接失败、超时和清理。
—[Multiple Sessions](python/multiple-sessions.md)：同时管理多个独立会话。
-[管理本地文件](python/managing-local-files.md)：使用人工智能分组策略通过元数据组织文件。
- [PR Visualization](python/pr-visualization.md)：使用GitHub MCP Server生成交互式PR年龄图表。
—[persistent Sessions](python/persisting-sessions.md)：保存和恢复会话。
—[无障碍报告](python/accessibility-report.md)：使用剧作家MCP服务器生成WCAG无障碍报告。

# # #去- [Ralph Loop](go/ralph-loop.md)：构建具有每次迭代新鲜上下文的自主AI编码循环，planning/building模式和背压。
—[错误处理](go/error-handling.md)：优雅地处理错误，包括连接失败、超时和清理。
—[Multiple Sessions](go/multiple-sessions.md)：同时管理多个独立会话。
-[管理本地文件](go/managing-local-files.md)：使用人工智能分组策略通过元数据组织文件。
-[公关可视化](go/pr-visualization.md)：生成交互式公关年龄图表使用GitHub MCP服务器。
—[persistent Sessions](go/persisting-sessions.md)：保存和恢复会话。
—[无障碍报告](go/accessibility-report.md)：使用剧作家MCP服务器生成WCAG无障碍报告。

# # # Java- [Ralph Loop](java/ralph-loop.md)：构建具有每次迭代新鲜上下文的自主AI编码循环，planning/building模式和背压。
—[错误处理](java/error-handling.md)：优雅地处理错误，包括连接失败、超时和清理。
—[Multiple Sessions](java/multiple-sessions.md)：同时管理多个独立会话。
-[管理本地文件](java/managing-local-files.md)：使用人工智能分组策略通过元数据组织文件。
-[公关可视化](java/pr-visualization.md)：生成交互式公关年龄图表使用GitHub MCP服务器。
—[persistent Sessions](java/persisting-sessions.md)：保存和恢复会话。
—[无障碍报告](java/accessibility-report.md)：使用剧作家MCP服务器生成WCAG无障碍报告。

##如何使用-浏览上面的语言部分并打开食谱链接
-每个配方包括`recipe/`子文件夹中的可运行示例，具有特定于语言的工具
-参阅现有示例和测试以获得工作参考：
—Node.js示例：`nodejs/examples/basic-example.ts`-端到端测试：`go/e2e`、`python/e2e`、`nodejs/test/e2e`、`dotnet/test/Harness`##运行示例

# # #。网```bash
cd dotnet/cookbook/recipe
dotnet run <filename>.cs
```

### Node.js

```bash
cd nodejs/cookbook/recipe
npm install
npx tsx <filename>.ts
```
# # # Python```bash
cd python/cookbook/recipe
pip install -r requirements.txt
python <filename>.py
```
# # #去```bash
cd go/cookbook/recipe
go run <filename>.go
```
# # # Java```bash
cd java/recipe
jbang <FileName>.java
```
# #贡献

-建议或添加一个新的配方创建一个markdown文件在你的语言的`cookbook/`文件夹和一个可运行的例子在`recipe/`-遵循[CONTRIBUTING.md]（../../CONTRIBUTING.md）中的存储库指导

# #状态

Cookbook结构完整，支持5种语言的7种食谱。每个配方都包括降价文档和可运行示例。