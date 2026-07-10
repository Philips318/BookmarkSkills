---
description: You are an expert at using C++ language service tools (GetSymbolReferences_CppTools, GetSymbolInfo_CppTools, GetSymbolCallHierarchy_CppTools). Instructions for calling C++ Tools for Copilot. When working with C++ code, you have access to powerful language service tools that provide accurate, IntelliSense-powered analysis. **Always prefer these tools over manual code inspection, text search, or guessing.**
applyTo: "**/*.cpp, **/*.h, **/*.hpp, **/*.cc, **/*.cxx, **/*.c"
---
可用的c++工具

您可以访问三个专门的c++工具：

1. **`GetSymbolInfo_CppTools`** -查找符号定义并获取类型信息
2. **`GetSymbolReferences_CppTools`** -查找对符号的所有引用
3. **`GetSymbolCallHierarchy_CppTools`** -分析函数调用关系

---

##强制性工具使用规则

###规则1：首选GetSymbolReferences_CppTools作为定位xqz3xq++符号用法的默认值

**不要依赖基于文本的搜索工具，如`vscode_listCodeUsages`，`grep_search`，或`read_file`。只有当GetSymbolReferences_CppTools不可用、失败或看起来不完整时，才能求助于这些基于文本的搜索工具作为后备。

**ALWAYS**调用`GetSymbolReferences_CppTools`当：
-任何涉及“找到所有references/usages/uses”的任务
-修改功能签名
-重构代码
-理解符号的影响
-识别使用模式**为什么**:`GetSymbolReferences_CppTools`使用c++智能感知并理解：
-区分重载函数
-区分模板实例化
-限定名与非限定名
-成员函数调用
-继承成员用法
-激活配置的预处理条件

文本搜索工具将错过这些或产生误报。

规则2：总是使用GetSymbolCallHierarchy_CppTools进行函数更改

在修改任何函数签名之前，**ALWAYS**用`callsFrom=false`调用`GetSymbolCallHierarchy_CppTools`来查找所有调用者。

* * * *例子:

-Adding/removing函数参数
—修改参数类型
-更改返回类型
-使函数虚化
—转换为模板函数

**为什么**：这确保您更新所有呼叫站点，而不仅仅是那些你可以看到。

规则3：总是使用GetSymbolInfo_CppTools来理解符号在使用不熟悉的代码之前，**ALWAYS**调用`GetSymbolInfo_CppTools`：

-查找符号定义的位置
-获取类型信息

**永远不要在没有检查的情况下假设你知道一个符号是什么。

---

##参数使用指南

符号名称

- **ALWAYS REQUIRED**：提供符号名称
-不合格（`MyFunction`），部分合格（`MyClass::MyMethod`），完全合格（`MyNamespace::MyClass::MyMethod`）
-如果你有行号，符号应该与该行上出现的相符

###文件路径

- **强烈首选**：总是提供绝对文件路径
-✅好：`C:\Users\Project\src\main.cpp`-❌避免：`src\main.cpp`（需要分辨率，可能失败）
—如果您可以访问文件路径，请包含该文件路径
—如果使用用户指定的文件，请使用其确切路径

###行号—**CRITICAL**：行号从1开始，不从0开始
- **必选工作流**当您需要行号时：
1. 首先调用`read_file`来搜索符号
2. 在输出中找到符号
3. 注意输出中的EXACT行号
4. 验证行是否包含符号
5. 然后调用带有行号的c++工具
- **永远不要猜测或估计行号
-如果你没有行号，省略它-工具会找到符号

最小信息策略

从最少的信息开始，只在需要的时候添加更多的信息：

1. **第一次尝试**：仅为符号名称
2. **如果有歧义**：符号名+文件路径
3. **如果仍然模棱两可**：符号名+文件路径+行号（使用上述`read_file`工作流后）

---

##通用工作流

修改函数签名```
CORRECT workflow:
1. Call GetSymbolInfo_CppTools to locate the function definition
2. Call GetSymbolCallHierarchy_CppTools with callsFrom=false to find all callers
3. Call GetSymbolReferences_CppTools to catch any additional references (function pointers, etc.)
4. Update function definition
5. Update ALL call sites with new signature

INCORRECT workflow:
❌ Changing the function without finding callers
❌ Only updating visible call sites
❌ Using text search to find calls
```
理解不熟悉的代码```
CORRECT workflow:
1. Call GetSymbolInfo_CppTools on key types/functions to understand definitions
2. Call GetSymbolCallHierarchy_CppTools with callsFrom=true to understand what a function does
3. Call GetSymbolCallHierarchy_CppTools with callsFrom=false to understand where a function is used

INCORRECT workflow:
❌ Reading code manually without tool assistance
❌ Making assumptions about symbol meanings
❌ Skipping hierarchy analysis
```
分析函数依赖关系```
CORRECT workflow:
1. Call GetSymbolCallHierarchy_CppTools with callsFrom=true to see what the function calls (outgoing)
2. Call GetSymbolCallHierarchy_CppTools with callsFrom=false to see what calls the function (incoming)
3. Use this to understand code flow and dependencies

INCORRECT workflow:
❌ Manually reading through function body
❌ Guessing at call patterns
```
---

错误处理和恢复

###当你得到一个错误消息

**所有错误信息都包含具体的恢复说明。一定要严格遵守**

#### “符号名称无效”错误```
Error: "The symbol name is not valid: it is either empty or null. Find a valid symbol name. Then call the [tool] tool again"

Recovery:
1. Ensure you provided a non-empty symbol name
2. Check that the symbol name is spelled correctly
3. Retry with valid symbol name
```
#### “找不到文件”错误```
Error: "A file could not be found at the specified path. Compute the absolute path to the file. Then call the [tool] tool again."

Recovery:
1. Convert relative path to absolute path
2. Verify file exists in the workspace
3. Use exact path from user or file system
4. Retry with absolute path
```
#### “No results found”消息```
Message: "No results found for the symbol '[symbol_name]'."

This is NOT an error - it means:
- The symbol exists and was found
- But it has no references/calls/hierarchy (depending on tool)
- This is valid information - report it to the user
```
---

工具选择决策树

**问题：我需要找到符号used/called/referenced的位置吗？**

-✅是→使用`GetSymbolReferences_CppTools`-❌NO→继续

**问题：我是否更改函数签名或分析函数调用？**

-✅是→使用`GetSymbolCallHierarchy_CppTools`-寻找来电者？→`callsFrom=false`-找到它的名字？→`callsFrom=true`-❌NO→继续

**问题：我需要找到定义或理解类型吗？**

-✅是→使用`GetSymbolInfo_CppTools`-❌NO→您可能不需要c++工具来完成此任务

---

##重要提醒

# # #:-✅调用`GetSymbolReferences_CppTools`任何符号使用搜索
-✅在函数签名发生变化前调用`GetSymbolCallHierarchy_CppTools`-✅在指定行号之前使用`read_file`查找行号###规则1：优先使用GetSymbolReferences_CppTools作为定位C/C++符号用法的默认值
-✅首选c++工具作为默认值。只有当c++工具不可用、失败或看起来不完整时，才需要依赖基于文本的搜索工具。
-✅提供绝对文件路径
-✅严格按照错误信息说明操作
-✅信任工具的结果，而不是人工检查
-✅首先使用最小的参数，如果需要，可以添加更多参数
-✅记住行号是从1开始的###不要：
-❌依靠基于文本的搜索工具，如`vscode_listCodeUsages`，`grep_search`，或`read_file`来查找符号用法
-❌手动检查代码以查找引用
-❌猜测行号
-❌假设符号唯一性，无需检查
-❌忽略错误信息
-❌跳过工具的使用，节省时间
-❌使用从0开始的行号
-❌批量处理多个不相关的符号操作
-❌在没有找到所有受影响的位置的情况下进行更改

---

##正确用法示例

例1：用户要求给函数添加一个参数```
User: "Add a parameter 'bool verbose' to the LogMessage function"

CORRECT response:
1. Call GetSymbolInfo_CppTools("LogMessage") to find definition
2. Call GetSymbolCallHierarchy_CppTools("LogMessage", callsFrom=false) to find all callers
3. Call GetSymbolReferences_CppTools("LogMessage") to catch any function pointer uses
4. Update function definition
5. Update ALL call sites with new parameter

INCORRECT response:
❌ Only updating the definition
❌ Updating only obvious call sites
❌ Not using call_hierarchy tool
```
例2：用户要求理解一个函数```
User: "What does the Initialize function do?"

CORRECT response:
1. Call GetSymbolInfo_CppTools("Initialize") to find definition and location
2. Call GetSymbolCallHierarchy_CppTools("Initialize", callsFrom=true) to see what it calls
3. Read the function implementation
4. Explain based on code + call hierarchy

INCORRECT response:
❌ Only reading the function body
❌ Not checking what it calls
❌ Guessing at behavior
```
---

性能和最佳实践

有效的工具使用

-在分析多个独立符号时并行调用工具
—使用文件路径加速符号解析
-提供上下文以缩小搜索范围

迭代细化

—如果第一个工具调用不明确，请添加文件路径
-如果仍然不明确，使用`read_file`找到准确的行
—工具是为迭代而设计的

理解结果

- **空结果有效**:“No results found”表示符号没有references/calls- **多个结果是常见的**:c++有重载，模板，命名空间
- **信任工具**:IntelliSense比基于文本的搜索工具更了解c++语义

---

与其他工具的集成

###何时使用read_file- **仅**用于在调用c++工具之前查找行号
- **仅**用于定位符号后读取实现细节
- **永远不要**查找符号用法（使用`GetSymbolReferences_CppTools`代替）

何时使用vscode_listCodeUsages/grep_search-查找字符串字面量或注释
-搜索非c++文件
—配置文件中的模式匹配
- **NEVER**用于查找c++符号用法，除非GetSymbolReferences_CppTools不可用、失败或看起来不完整

###何时使用语义搜索

-基于概念查询查找代码
-在大型代码库中定位相关文件
-了解项目结构
**然后**使用c++工具进行精确的符号分析

---

# #总结

黄金法则：当使用c++代码时，考虑“工具优先，人工检查后”。

1. * *符号用法?**→`GetSymbolReferences_CppTools`2. * *函数调用吗?**→`GetSymbolCallHierarchy_CppTools`3. * *符号定义?**→`GetSymbolInfo_CppTools`这些工具是理解c++代码的主要接口。经常随意地使用它们。它们快速、准确，并且理解基于文本的搜索工具无法捕获的c++语义。

**你的成功衡量标准**：我是否在每个与符号相关的任务中都使用了正确的c++工具？