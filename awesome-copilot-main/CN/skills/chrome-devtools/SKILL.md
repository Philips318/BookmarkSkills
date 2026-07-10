---
name: chrome-devtools
description: 'Expert-level browser automation, debugging, and performance analysis using Chrome DevTools MCP. Use for interacting with web pages, capturing screenshots, analyzing network traffic, and profiling performance.'
license: MIT
---
# Chrome DevTools Agent

# #概述

用于控制和检查实时Chrome浏览器的专业技能。该技能利用`chrome-devtools`MCP服务器执行各种与浏览器相关的任务，从简单的导航到复杂的性能分析。

##何时使用

在以下情况下使用此技能：

- **浏览器自动化**：导航页面，点击元素，填写表单，和处理对话框。
- **视觉检测**：对网页进行截图或文字快照。
- **调试**：检查控制台消息，评估页面上下文中的JavaScript，分析网络请求。
- **性能分析**：记录和分析性能跟踪，以确定瓶颈和核心Web关键问题。
- **仿真**：调整视口大小或模拟network/CPU条件。

##工具分类

# # # 1。导航和页面管理—`new_page`：新建tab/page.-`navigate_page`：转到特定的URL，重新加载或浏览历史记录。
—`select_page`：在打开的页面之间切换上下文。
-`list_pages`：查看所有打开的页面及其id。
—`close_page`：关闭指定页面。
—`wait_for`：等待特定文本出现在页面上。

# # # 2。输入与交互

-`click`：单击一个元素（使用快照中的`uid`）。
-`fill`/`fill_form`：输入文本或一次填写多个字段。
-`hover`：将鼠标移动到元素上。
-`press_key`：发送键盘快捷键或特殊键（如“Enter”，“Control+C”）。
-`drag`：拖放元素。
-`handle_dialog`：接受或拒绝浏览器alerts/prompts.—`upload_file`：通过文件输入方式上传文件。

# # # 3。调试检查-`take_snapshot`：获得基于文本的可访问性树（最适合识别元素）。
-`take_screenshot`：捕获页面或特定元素的可视化表示。
—`list_console_messages`/`get_console_message`：检查页面的控制台输出。
-`evaluate_script`：在页面上下文中运行自定义JavaScript。
—`list_network_requests`/`get_network_request`：分析网络流量并请求详细信息。

# # # 4。仿真与性能

-`resize_page`：改变视口尺寸。
-`emulate`：限制CPU/Network或模拟地理位置。
—`performance_start_trace`：开始记录性能配置文件。
—`performance_stop_trace`：停止录制并保存跟踪。
-`performance_analyze_insight`：从记录的性能数据中获得详细的分析。

##工作流模式

模式A：识别元素（快照优先）

查找元素时，总是选择`take_snapshot`而不是`take_screenshot`。快照提供交互工具所需的`uid`值。```markdown
1. `take_snapshot` to get the current page structure.
2. Find the `uid` of the target element.
3. Use `click(uid=...)` or `fill(uid=..., value=...)`.
```
模式B：故障排除

当页面失败时，检查控制台日志和网络请求。```markdown
1. `list_console_messages` to check for JavaScript errors.
2. `list_network_requests` to identify failed (4xx/5xx) resources.
3. `evaluate_script` to check the value of specific DOM elements or global variables.
```
模式C：性能分析

确定页面慢的原因。```markdown
1. `performance_start_trace(reload=true, autoStop=true)`
2. Wait for the page to load/trace to finish.
3. `performance_analyze_insight` to find LCP issues or layout shifts.
```
最佳实践

- **上下文感知**：总是运行`list_pages`和`select_page`，如果你不确定哪个选项卡当前是活跃的。
- **快照**：在任何重大导航或DOM更改后，由于`uid`值可能会发生变化，因此需要进行新的快照。
- **超时**：为`wait_for`使用合理的超时，以避免挂起缓慢加载的元素。
- **截图**：使用`take_screenshot`节省视觉验证，但依赖于`take_snapshot`的逻辑。