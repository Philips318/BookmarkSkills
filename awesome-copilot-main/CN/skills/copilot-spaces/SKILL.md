---
name: copilot-spaces
description: 'Use Copilot Spaces to provide project-specific context to conversations. Use this skill when users mention a "Copilot space", want to load context from a shared knowledge base, discover available spaces, or ask questions grounded in curated project documentation, code, and instructions.'
---
#副驾驶舱位

使用Copilot Spaces将精心策划的项目特定背景带入对话。空间是存储库、文件、文档和说明的共享集合，它将Copilot响应建立在团队实际代码和知识的基础上。

##可用工具

MCP工具（只读）

|工具|用途||------|---------|
|`mcp__github__list_copilot_spaces`|列出当前用户|可访问的所有空间
|`mcp__github__get_copilot_space`|按所有者和名称|加载空间的完整上下文

通过`gh api`的REST API（完全CRUD）

Spaces REST API支持创建、更新、删除空间和管理协作者。MCP服务器只公开读操作，因此使用`gh api`进行写操作。

用户空间:* * * *

|方法|终点|目的||--------|----------|---------|
|`POST`|`/users/{username}/copilot-spaces`|创建空间|
|`GET`|`/users/{username}/copilot-spaces`|列表空间|
|`GET`|`/users/{username}/copilot-spaces/{number}`|获取一个空间|
|`PUT`|`/users/{username}/copilot-spaces/{number}`|更新空间|
|`DELETE`|`/users/{username}/copilot-spaces/{number}`|删除空间|

**组织空间：**与`/orgs/{org}/copilot-spaces/...`相同

**合作者：**在`.../collaborators`添加、列出、更新和删除合作者

**作用域要求：** PAT读需要`read:user`，写需要`user`。加上`gh auth refresh -h github.com -s user`。

**注意：**这个API是功能性的，但还没有在公共REST API文档中。它可能需要`copilot_spaces_api`特性标志。

何时使用空格-用户提到“副驾驶空间”或要求“加载空间”
-用户希望得到基于特定项目文档、代码或标准的答案
-用户询问“有哪些空间可用？”或“为X找到一个空间”
-用户需要入职背景、架构文档或团队特定指导
-用户希望遵循在空间中定义的结构化工作流程（模板，清单，多步骤流程）

# #工作流程

# # # 1。发现的空间

当用户询问有哪些空间可用或您需要找到合适的空间时：```
Call mcp__github__list_copilot_spaces
```
这将返回用户可以访问的所有空间，每个空间都有一个`name`和`owner_login`。向用户呈现相关匹配。

要过滤特定用户的空格，请根据用户名匹配`owner_login`（例如，“show me my spaces”）。

# # # 2。加载空间

当用户指定了一个特定的空间或您已经确定了正确的空间时：```
Call mcp__github__get_copilot_space with:
  owner: "org-or-user"    (the owner_login from the list)
  name: "Space Name"      (exact space name, case-sensitive)
```
这将返回空间的完整内容：附加的文档、代码上下文、自定义说明和任何其他策划的材料。使用这个上下文来告知你的回答。

# # # 3。跟着面包屑走

空间内容通常引用外部资源：GitHub问题、仪表板、repos、讨论或其他工具。使用其他MCP工具主动获取这些内容以收集完整的上下文。例如:
-空间引用主动跟踪问题。使用`issue_read`获取最新的注释。
-一个空间链接到一个项目板。使用项目工具检查当前状态。
-一个空间提到了回购的总体规划。使用`get_file_contents`读取它。

# # # 4。回答还是执行

加载后，根据它所包含的内容使用空间内容：**如果空格包含参考资料**（文档、代码、标准）：
-回答有关项目架构、模式或标准的问题
-生成遵循团队惯例的代码
-使用项目特定知识调试问题

**如果空间包含工作流程说明**（模板，分步过程）：
-按照定义的工作流程，一步一步地进行
-从工作流指定的源收集数据
-以工作流定义的格式产生输出
-显示进度后，每一步，以便用户可以转向

# # # 5。管理空间（通过`gh api`）

当用户想要创建、更新或删除一个空间时，使用`gh api`。首先，从列表端点查找空格号。

**更新空格的指令：**```bash
gh api users/{username}/copilot-spaces/{number} \
  -X PUT \
  -f general_instructions="New instructions here"
```
**一起更新名称、描述或说明：**```bash
gh api users/{username}/copilot-spaces/{number} \
  -X PUT \
  -f name="Updated Name" \
  -f description="Updated description" \
  -f general_instructions="Updated instructions"
```
**创建一个新空间：**```bash
gh api users/{username}/copilot-spaces \
  -X POST \
  -f name="My New Space" \
  -f general_instructions="Help me with..." \
  -f visibility="private"
```
**附加资源（取代整个资源列表）：**```json
{
  "resources_attributes": [
    { "resource_type": "free_text", "metadata": { "name": "Notes", "text": "Content here" } },
    { "resource_type": "github_issue", "metadata": { "repository_id": 12345, "number": 42 } },
    { "resource_type": "github_file", "metadata": { "repository_id": 12345, "file_path": "docs/guide.md" } }
  ]
}
```
**删除空格：**```bash
gh api users/{username}/copilot-spaces/{number} -X DELETE
```
**可更新字段：**`name`，`description`,`general_instructions`,`icon_type`,`icon_color`,`visibility`("private"/"public"),`base_role`("no_access"/"reader"),`resources_attributes`# #的例子

例1：用户请求一个空间

**用户**：“加载辅助驾驶空间”

* *行动* *:
1. 用所有者`"github"`调用`mcp__github__get_copilot_space`，名称`"Accessibility"`2. 使用返回的上下文回答有关可访问性标准、MAS等级、遵从性流程等的问题。

例2：用户想要查找空格

**用户**：“我们的团队有哪些副驾驶舱位？”

* *行动* *:
1. 叫`mcp__github__list_copilot_spaces`2.Filter/present与用户组织或兴趣相关的空间
3. 主动提供他们感兴趣的空间

示例3：基于情境的问题

**用户**：“使用安全空间，我们的秘密扫描策略是什么？”* *行动* *:
1. 使用适当的所有者和名称调用`mcp__github__get_copilot_space`2. 在空格内容中查找相关政策
3. 根据实际的内部文档来回答

示例4：空间作为工作流引擎

**用户**：“使用PM每周更新空间编写我的每周更新”

* *行动* *:
1. 调用`mcp__github__get_copilot_space`来加载空间。它包含模板格式和逐步说明。
2. 遵循空间的工作流程：从附加的主动性问题中提取数据，收集指标，起草每个部分。
3. 使用其他MCP工具获取空间引用的外部资源（跟踪问题，仪表板）。
4. 在每个部分之后显示草稿，以便用户可以回顾和填补空白。
5. 以空格定义的格式生成最终输出。

例5：以编程方式更新空间指令

**用户**：“更新我的PM每周更新空间，包括一个新的写作指南”* *行动* *:
1. 调用`mcp__github__list_copilot_spaces`并找到空间号（例如，19）。
2. 调用`mcp__github__get_copilot_space`读取当前指令。
3. 根据需要修改说明文本。
4. 推送更新：```bash
gh api users/labudis/copilot-spaces/19 -X PUT -f general_instructions="updated instructions..."
```
# #提示—空间名是**区分大小写的**。使用`list_copilot_spaces`中的确切名称。
—空间可以归用户所有，也可以归组织所有。总是同时提供`owner`和`name`。
—空间内容可以很大（20KB+）。如果作为临时文件返回，请使用grep或view_range来查找相关部分，而不是一次读取所有内容。
—如果没有找到空格，建议列出可用的空格以找到正确的名称。
-空格自动更新为基础的仓库变化，所以上下文始终是最新的。
-一些空格包含应该指导您行为的自定义说明（编码标准，首选模式，工作流程）。把这些当成指示，而不是建议。
- **写操作** （`gh api`对create/update/delete）需要`user`PAT作用域。如果写操作出现404，运行`gh auth refresh -h github.com -s user`。
-资源更新**替换整个数组**。要添加资源，请包括所有现有资源和新资源。对再保险第一步，在数组中包含`{ "id": 123, "_destroy": true }`。