---
name: optimize-simplicite-logs
description: capability to parse Simplicité logs from a raw `.txt` file, filter fields to reduce noise, and output the result as structured JSON.
---
优化Simplicite日志

该技能提供了从原始`.txt`文件解析simplicit<e:1>日志、过滤字段以减少干扰并将结果输出为结构化JSON的功能。这对于优化AI上下文大小（节省约56%的令牌）和为故障排除提供结构化、可预测的数据至关重要。

何时使用此技能

在需要时使用此技能：
—分析用户提供的simplicit<e:1>日志文件，格式为`.txt`。
-避免摄取大量的原始日志文件到您的上下文窗口。
-从详细的多行日志输出中提取结构化字段（如`timestamp`，`level`,`body`）。

**重要：**不是直接读取用户使用文件读取工具提供的原始`.txt`日志文件，您**必须**使用日志转换脚本之一（PowerShell或Python）首先将文件解析为JSON格式，可选择只提取所需的字段。# #先决条件

—访问PowerShell脚本（`/scripts/SimpliciteLog2Json.ps1`）或Python脚本（`/scripts/simplicite-log2json.py`）。

##核心能力

# # # 1。环境优化
通过只提取相关的日志字段（如`body`，`timestamp`,`level`）和丢弃不相关的结构化日志数据（如`app`，`endpoint`,`contextPath`），减少大型simplicit<e:1>日志消耗的令牌。

# # # 2。支持多行
正确捕获JSON结构的`body`字段内的堆栈跟踪和多行错误，这是简单的文本搜索可能错过的。

# # # 3。Stdout支持
如果没有为JSON文件提供输出路径（例如省略`--output`或`-Output`），解析后的JSON将直接打印到stdout，允许您将输出管道输出到其他工具。

##输出摘要

处理完成后，工具打印摘要到stderr（或控制台）：```
Processed: 123 entries, Skipped: 2 entries
```
##用法示例

示例1:Python版本（推荐）
将日志文件转换为JSON，只保留最重要的字段：```sh
python /absolute/path/to/skills/optimize-simplicite-logs/scripts/simplicite-log2json.py <input.txt> --include timestamp,level,body --output <output.json>
```
示例2:PowerShell版本```powershell
/python /absolute/path/to/skills/optimize-simplicite-logs/scripts/SimpliciteLog2Json.ps1 -InputPath "<input.txt>" -Output "<output.json>" -Include "body,timestamp,level"
```
在生成`<output.json>`之后，您可以安全地读取结果文件来执行分析。

# #指南

1. **永远不要使用标准的文本阅读工具直接从simplicit<e:1>读取`.txt`日志文件。始终使用可用的脚本将它们转换为JSON。
2. **过滤字段：**使用`--include`（Python）或`-Include`（PowerShell）将字段限制为诊断问题绝对必要的内容（通常是`timestamp,level,body`）。
3. **可筛选字段：**可筛选字段包括：`timestamp`、`app`、`level`、`endpoint`、`contextPath`、`event`、`user`、`class`、`function`、`rowId`、`body`。

##常见模式

模式：快速上下文故障排除```sh
# 1. Run the script to generate a minified JSON output in the current directory
python /absolute/path/to/skills/optimize-simplicite-logs/scripts/simplicite-log2json.py logs.txt --include timestamp,level,body --output logs_minified.json

# 2. Then read logs_minified.json to understand the context.
```
# #的局限性

解析器依赖于与标准simplicit<e:1>日志输出匹配的固定正则表达式模式。如果对日志格式进行了大量定制，那么解析可能会失败或降级。