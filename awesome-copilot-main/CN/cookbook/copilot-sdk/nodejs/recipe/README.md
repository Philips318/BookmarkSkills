# Runnable Recipe Examples

这个文件夹包含了每个食谱的独立的、可执行的TypeScript示例。每个文件都可以直接使用`tsx`或通过npm脚本运行。

# #先决条件

-Node.js18及以上版本
-安装依赖项（链接到repo中的本地SDK）：```bash
npm install
```
##运行示例

每个`.ts`文件都是一个完整的、可运行的程序。你可以用两种方式运行它们：

使用npm脚本：```bash
npm run <script-name>
```
直接使用tsx：```bash
npx tsx <filename>.ts
```
可用的食谱

|配方| npm脚本|直接命令|描述|| -------------------- | ------------------------------ | --------------------------------- | ------------------------------------------ |
|错误处理|`npm run error-handling`|`npx tsx error-handling.ts`|演示错误处理模式|
|多会话|`npm run multiple-sessions`|`npx tsx multiple-sessions.ts`|管理多个独立会话|
|本地文件管理|`npm run managing-local-files`|`npx tsx managing-local-files.ts`|使用AI分组|对文件进行组织
| PR可视化|`npm run pr-visualization`|`npx tsx pr-visualization.ts`|生成PR年龄图表|
|持久化会话|`npm run persisting-sessions`|`npx tsx persisting-sessions.ts`|跨重启保存和恢复会话|

带参数的例子

**PR可视化与特定的回购：**```bash
npx tsx pr-visualization.ts --repo github/copilot-sdk
```
**管理本地文件（编辑文件以更改目标文件夹）：**```bash
# Edit the targetFolder variable in managing-local-files.ts first
npx tsx managing-local-files.ts
```
本地SDK开发`package.json`使用`"*"`引用本地Copilot SDK，解析为本地SDK源。这意味着:

—对SDK源代码的更改立即可用
-无需从npm发布或安装
-非常适合测试和开发

如果修改了SDK源代码，可能需要重新构建：```bash
cd ../../src
npm run build
```
## TypeScript特性

这些例子使用了现代TypeScript/Node.js特性：

-顶级等待（需要`"type": "module"`在package.json）
- ESM导入
TypeScript的类型安全
-async/await模式

学习资源

- [TypeScript文档]（https://www.typescriptlang.org/docs/）
- [Node.js文档]（https://nodejs.org/docs/latest/api/）
- [GitHub CopilotSDK forNode.js]（https://github.com/github/copilot-sdk/blob/main/nodejs/README.md）
-[家长食谱]（../README.md）