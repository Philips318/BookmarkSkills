---
name: memory-merger
description: 'Merges mature lessons from a domain memory file into its instruction file. Syntax: `/memory-merger >domain [scope]` where scope is `global` (default), `user`, `workspace`, or `ws`.'
---
#内存合并

您将领域内存文件中的成熟知识整合到它的指令文件中，以最小的冗余确保知识保存。

**使用待办事项列表**通过流程步骤跟踪您的进度，并随时通知用户。

# #范围

内存指令可以存储在两个作用域中：

- **全局** (`global`或`user`) -存储在`<global-prompts>`(`vscode-userdata:/User/prompts/`)，并适用于所有VS Code项目
- **工作区** (`workspace`或`ws`) -存储在`<workspace-instructions>`（`<workspace-root>/.github/instructions/`）中，仅适用于当前项目

默认作用域是**global**。

在这个提示符中，`<global-prompts>`和`<workspace-instructions>`引用这些目录。

# #语法```
/memory-merger >domain-name [scope]
```
-`>domain-name`-必选参数。要合并的域（例如，`>clojure`,`>git-workflow`,`>prompt-engineering`）
-`[scope]`-可选。其中之一：`global`、`user`（两者都表示全局）、`workspace`或`ws`。默认为`global`* *例子:* *
-`/memory-merger >prompt-engineering`-合并全局提示工程内存
-`/memory-merger >clojure workspace`-合并工作空间闭包内存
-`/memory-merger >git-workflow ws`-合并工作区git-工作流内存

# #过程

# # # 1。解析输入和读取文件

- **从用户输入中提取**域和范围
- **确定**文件路径：
—全局：`<global-prompts>/{domain}-memory.instructions.md`→`<global-prompts>/{domain}.instructions.md`—工作空间：`<workspace-instructions>/{domain}-memory.instructions.md`→`<workspace-instructions>/{domain}.instructions.md`-用户可以输入错误的域名，如果你没有找到内存文件，glob目录，并确定是否可能有一个匹配。如果有疑问，请用户输入。
- **读取**两个文件（内存文件必须存在，指令文件可能不存在）

# # # 2。分析和建议

检查所有内存部分，并将其提交合并考虑：```
## Proposed Memories for Merger

### Memory: [Headline]
**Content:** [Key points]
**Location:** [Where it fits in instructions]

[More memories]...
```
说：“请回顾这些记忆。用‘go’批准所有内容，或者指定跳过哪些内容。”

**停止并等待用户输入

# # # 3。定义质量标准

建立10/10标准，以确定合并后的结果指令是什么：
1. **零知识损失** -保留每个细节，示例和细微差别
2. **最小冗余** -重叠指导合并
3. **最大可扫描性** -清晰的层次结构，平行结构，战略大胆，逻辑分组

# # # 4。合并和迭代

开发最终合并的指令**而不更新文件**：

1. 起草合并后的指令，包括批准的记忆
2. 根据质量标准进行评估
3. 完善结构、措辞和组织
4. 重复操作，直到合并的指令满足10/10条件

# # # 5。更新文件

一旦最终合并的指令满足10/10标准：- **创建或更新**指令文件与最终合并的内容
-如果创建新文件，包括适当的标题
- **合并`applyTo`模式**从内存和指令文件，如果两者都存在，确保全面覆盖，没有重复
- **从内存文件中删除**合并的部分

# #的例子```
User: "/memory-merger >clojure"

Agent:
1. Reads clojure-memory.instructions.md and clojure.instructions.md
2. Proposes 3 memories for merger
3. [STOPS]

User: "go"

Agent:
4. Defines quality bar for 10/10
5. Merges new instructions candidate, iterates to 10/10
6. Updates clojure.instructions.md
7. Cleans clojure-memory.instructions.md
```
