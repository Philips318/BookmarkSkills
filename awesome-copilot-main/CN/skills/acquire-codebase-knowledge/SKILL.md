---
name: acquire-codebase-knowledge
description: 'Use this skill when the user explicitly asks to map, document, or onboard into an existing codebase. Trigger for prompts like "map this codebase", "document this architecture", "onboard me to this repo", or "create codebase docs". Do not trigger for routine feature implementation, bug fixes, or narrow code edits unless the user asks for repository-level discovery.'
license: MIT
compatibility: 'Cross-platform. Requires Python 3.8+ and git. Run scripts/scan.py from the target project root.'
metadata:
  version: "1.3"
  enhancements:
    - Multi-language manifest detection (25+ languages supported)
    - CI/CD pipeline detection (10+ platforms)
    - Container & orchestration detection
    - Code metrics by language
    - Security & compliance config detection
    - Performance testing markers
argument-hint: 'Optional: specific area to focus on, e.g. "architecture only", "testing and concerns"'
---
#获取代码库知识

在`docs/codebase/`中生成7个已填充的文档，涵盖在项目中有效工作所需的所有内容。只记录可从文件或终端输出验证的内容-永远不要推断或假设。

输出合同（Required）

在结束之前，以下所有内容必须是正确的：

1. 正是这些文件存在于`docs/codebase/`中：`STACK.md`，`STRUCTURE.md`,`ARCHITECTURE.md`,`CONVENTIONS.md`,`INTEGRATIONS.md`,`TESTING.md`,`CONCERNS.md`。
2. 每个声明都可以追溯到源文件、配置或终端输出。
3. 未知数标记为`[TODO]`；与意图相关的决策标记为`[ASK USER]`。
4. 每个文件都包含一个带有具体文件路径的简短“证据”列表。
5. 最后的回答包括编号的`[ASK USER]`问题和意图与现实的分歧。

# #工作流程

复制并跟踪以下清单：```
- [ ] Phase 1: Run scan, read intent documents
- [ ] Phase 2: Investigate each documentation area
- [ ] Phase 3: Populate all seven docs in docs/codebase/
- [ ] Phase 4: Validate docs, present findings, resolve all [ASK USER] items
```
聚焦区域模式

如果用户提供了一个焦点区域（例如：“仅架构”或“测试和关注”）：

1. 始终全力运行阶段1。
2. 首先完全完成焦点区域文档。
3. 对于尚未分析的非重点文档，保留必需的部分，并将未知标记为`[TODO]`。
4. 在最终输出之前，仍然对所有7个文档运行第4阶段验证循环。

阶段1：扫描和读取意图

1. 从目标项目根目录运行扫描脚本：   ```bash
   python3 "$SKILL_ROOT/scripts/scan.py" --output docs/codebase/.codebase-scan.txt
   ```
其中`$SKILL_ROOT`是技能文件夹的绝对路径。适用于Windows， macOS和Linux。

**快速入门：**如果你有内联路径：   ```bash
   python3 /absolute/path/to/skills/acquire-codebase-knowledge/scripts/scan.py --output docs/codebase/.codebase-scan.txt
   ```
2. 搜索`PRD`、`TRD`、`README`、`ROADMAP`、`SPEC`、`DESIGN`文件并读取它们。
3. 在阅读任何源代码之前，总结项目意图。

第二阶段：调查

使用扫描输出回答七个模板中的每个模板的问题。加载[`references/inquiry-checkpoints.md`]（references/inquiry-checkpoints.md）获取完整的每个模板问题列表。

如果堆栈有歧义（多个清单文件、不熟悉的文件类型、没有`package.json`），则加载[`references/stack-detection.md`]（references/stack-detection.md）。

阶段3：填充模板

将每个模板从`assets/templates/`复制到`docs/codebase/`。请按以下顺序填写：1. [STACK.md](assets/templates/STACK.md) -语言，运行时，框架，所有依赖项
2. [STRUCTURE.md](assets/templates/STRUCTURE.md) -目录布局，入口点，密钥文件
3. [ARCHITECTURE.md]（assets/templates/ARCHITECTURE.md）—层、模式、数据流
4. [CONVENTIONS.md]（assets/templates/CONVENTIONS.md）—命名、格式化、错误处理、导入
5. [INTEGRATIONS.md](assets/templates/INTEGRATIONS.md) -外部api，数据库，认证，监控
6. [TESTING.md]（assets/templates/TESTING.md）—框架、文件组织、模拟策略
7. [CONCERNS.md](assets/templates/CONCERNS.md) -技术债务，漏洞，安全风险，性能瓶颈

对于不能从代码中确定的任何内容，使用`[TODO]`。在正确答案需要团队意图的地方使用`[ASK USER]`。

阶段4：验证、修复、验证

在结束之前运行这个强制验证循环：1. 根据`references/inquiry-checkpoints.md`验证每个文档。
2. 对于每个非琐碎的主张，确认至少存在一个证据参考。
3. 如果任何必需的部分缺失或不支持：
—修复文档。
—重新运行验证。
4. 重复，直到所有七个医生都通过。

然后呈现所有七个文档的摘要，将每个`[ASK USER]`项作为编号问题列出，并突出显示与阶段1的意图与现实的任何分歧。

验证通过标准：

-没有未经证实的主张。
-没有空的必需部分。
-未知数使用`[TODO]`而不是假设。
-团队意图间隔明确标记为`[ASK USER]`。

---

# #陷阱

**单节点：**根`package.json`可能对`workspaces`、`packages/`或`apps/`目录没有源代码检查。每个工作区可能有独立的依赖项和约定。分别映射每个子包。**过时的README:** README通常描述的是预期的体系结构，而不是当前的体系结构。在将任何README声明视为事实之前，请与实际文件结构进行交叉参考。

**TypeScript路径别名：**`tsconfig.json``paths`config意味着像`@/foo`这样的导入不会直接映射到文件系统。在记录结构之前将别名映射到实际路径。

**Generated/compiled输出：**永远不要记录`dist/`、`build/`、`generated/`、`.next/`、`out/`或`__pycache__/`的模式。这些都是工件——仅仅是文档源约定。

**`.env.example`显示需要的配置：**秘密永远不会提交。读取`.env.example`、`.env.template`或`.env.sample`以发现所需的环境变量。

**`devDependencies`≠生产栈：**只有`dependencies`（或等同的，例如`[tool.poetry.dependencies]`）在生产中运行。文档编辑器、格式化器和测试框架分别作为开发工具。**测试todo≠生产债务：`test/`、`tests/`、`__tests__/`、`spec/`中的todo是覆盖缺口，而不是生产技术债务。用`CONCERNS.md`分隔它们。

**高流失率文件=脆弱区域：**在最近的git历史中出现最多的文件具有最高的修改率和可能隐藏的复杂性。总是在`CONCERNS.md`中标注它们。

---

# #反模式

|❌不要|✅要||---------|--------------|
|“使用干净的架构与Domain/Data层。”（当不存在这样的目录时）|只说明实际显示的目录结构。|
|“这是一个Next.js项目。”（不检查`package.json`） |先检查`dependencies`。陈述实际存在的东西。|
|从变量名猜测数据库，如`dbUrl`|检查清单`pg`，`mysql2`,`mongoose`，`prisma`等|
|文档`dist/`或`build/`命名模式作为约定|源文件。|

---

增强扫描输出部分

除了原始输出，`scan.py`脚本现在还生成以下部分：** -总文件数，按语言划分的代码行数，最大文件数（复杂度信号）
**检测GitHub Actions， GitLab CI, Jenkins， CircleCI等
- Docker, Docker Compose, Kubernetes， Vagrant配置
- Snyk, Dependabot,SECURITY.md， SBOM，安全策略
-性能和测试** -基准配置，分析标记，负载测试工具

在阶段2中使用这些部分来告知调查问题并确定特定于工具的模式。

---

##资产捆绑

|资产|何时加载||-------|-------------|
| [`scripts/scan.py`](scripts/scan.py) |第一阶段-先运行，然后再读取任何代码（需要Python 3.8+） |

| [`references/inquiry-checkpoints.md`](references/inquiry-checkpoints.md) |第二阶段-加载每个模板调查问题|
| [`references/stack-detection.md`](references/stack-detection.md) |阶段2 -仅当堆栈是模糊|
| [`assets/templates/STACK.md`](assets/templates/STACK.md) |第三阶段步骤1 |
| [`assets/templates/STRUCTURE.md`](assets/templates/STRUCTURE.md) |第三阶段第二步|
| [`assets/templates/ARCHITECTURE.md`](assets/templates/ARCHITECTURE.md) |第三阶段第三步|
| [`assets/templates/CONVENTIONS.md`](assets/templates/CONVENTIONS.md) |第三阶段第四步|
| [`assets/templates/INTEGRATIONS.md`](assets/templates/INTEGRATIONS.md) |第三阶段步骤5 |
| [`assets/templates/TESTING.md`](assets/templates/TESTING.md) |第三阶段步骤6 |
| [`assets/templates/CONCERNS.md`](assets/templates/CONCERNS.md) |第三阶段步骤7 |

模板使用方式：

-默认模式：只完成每个模板中的“Core Sections （Required）”。
-扩展模式：添加可选的部分，只有当回购的复杂性证明他们。