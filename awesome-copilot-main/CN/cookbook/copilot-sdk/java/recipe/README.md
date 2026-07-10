# Runnable Recipe Examples

这个文件夹包含每个食谱食谱的独立的、可执行的Java示例。每个文件都可以直接使用[JBang]（https://www.jbang.dev/）运行——不需要任何项目设置。

# #先决条件

—Java 17及以上版本
-安装了JBang：```bash
# macOS (using Homebrew)
brew install jbangdev/tap/jbang

# Linux/macOS (using curl)
curl -Ls https://sh.jbang.dev | bash -s - app setup

# Windows (using Scoop)
scoop install jbang
```
其他安装方法请参见[JBang安装指南]（https://www.jbang.dev/download/）。

##运行示例

每个`.java`文件都是一个完整的、可运行的程序。简单的使用方法:```bash
jbang <FileName>.java
```
可用的食谱

|配方|命令|描述|| -------------------- | ------------------------------------ | ------------------------------------------ |
|错误处理|`jbang ErrorHandling.java`|演示错误处理模式|
|多会话|`jbang MultipleSessions.java`|管理多个独立会话|
|本地文件管理|`jbang ManagingLocalFiles.java`|通过AI分组|对文件进行组织
| PR可视化|`jbang PRVisualization.java`|生成PR年龄图表|
|持久化会话|`jbang PersistingSessions.java`|跨重启保存和恢复会话|
|拉尔夫循环|`jbang RalphLoop.java`|自主AI任务循环|
|无障碍报告|`jbang AccessibilityReport.java`| WCAG无障碍报告生成器|

带参数的例子

**PR可视化与特定的回购：**```bash
jbang PRVisualization.java github/copilot-sdk
```
**管理本地文件与特定的文件夹：**```bash
jbang ManagingLocalFiles.java /path/to/your/folder
```
**Ralph Loop与自定义提示文件：**```bash
jbang RalphLoop.java PROMPT_build.md 20
```
为什么是JBang？

JBang允许您将Java文件作为脚本运行——没有`pom.xml`，没有`build.gradle`，没有项目脚手架。依赖关系通过`//DEPS`注释内联声明，并自动解析。

学习资源

- [JBang文档]（https://www.jbang.dev/documentation/guide/latest/）
- [GitHub CopilotJava SDK]（https://github.com/github/copilot-sdk-java）
-[家长食谱]（../README.md）