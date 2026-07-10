#步骤4：构建数据集

**为什么此步骤**：数据集将所有内容联系在一起-可运行（步骤2），评估器（步骤3）和用例（步骤1c） -到具体的测试场景中。在测试时，`pixie test`用`input_data`调用可运行程序，包装注册表用`eval_input`填充，计算器对捕获的输出进行评分。

**在建筑条目**之前，请审查：

- **`pixie_qa/00-project-analysis.md`** -能力清单和失效模式。数据集条目应该涵盖能力清单中的条目，并包括针对列出的故障模式的条目。
- **`pixie_qa/02-eval-criteria.md`** -用例及其能力覆盖。确保每个列出的用例都有代表性的条目。

---

了解`input_data`、`eval_input`和`expectation`在构建数据集之前，了解这些术语的含义：- **`input_data`** =作为Pydantic模型传递给`Runnable.run()`的kwargs。这些是输入数据（用户消息、请求体、CLI参数）。键必须匹配为`run(args: T)`定义的Pydantic模型的字段。

- **`eval_input`** =`{"name": ..., "value": ...}`对象列表，对应于应用程序中的`wrap(purpose="input")`调用。在测试时，这些对象由wrap注册表自动注入；`wrap(purpose="input")`调用在应用程序中返回注册表值，而不是调用真正的外部依赖。

只有当应用程序没有`wrap(purpose="input")`调用时，`eval_input`**可能是一个空列表**。**如果应用程序有输入包装，每个数据集条目必须提供相应的`eval_input`值和预捕获的内容** -否则应用程序在eval期间进行实时外部调用，这是缓慢的，不稳定的，不可复制的。有关如何捕获此内容，请参见第4b节。每个项都是一个`NamedData`对象，具有`name`（str）和`value`（任何json可序列化的值）。

- **`expectation`**（可选）=具体情况的评估参考。对于这个场景，正确的输出应该是什么样子。用于将输出与引用进行比较的求值器（例如，`Factuality`,`ClosedQA`）。对于不需要参考的输出质量评估器不需要。

**eval输出** =应用程序实际产生的内容，在运行时由`wrap(purpose="output")`和`wrap(purpose="state")`调用捕获。**不存储在数据集中** -它是在`pixie test`运行应用程序时产生的。`pixie_qa/reference-trace.jsonl`的**引用跟踪**是数据形状的主要来源：

-过滤它以查看`eval_input`值的确切序列化格式
—读取`kwargs`记录，了解`input_data`结构
-读取`purpose="output"/"state"`事件以了解应用程序产生的输出，因此您可以编写有意义的`expectation`值

---# # 4。导出评估者分配

评估标准工件（`pixie_qa/02-eval-criteria.md`）将每个标准映射到用例。评估器映射工件（`pixie_qa/03-evaluator-mapping.md`）将每个标准映射到具体的评估器名称。把这些:

1. **数据集级默认求值器**：标记为适用于“所有”用例的标准→它们的求值器名称将在顶级`"evaluators"`数组中。
2. **项目级求值器**：仅适用于子集的标准→它们的求值器名称仅在`"evaluators"`的相关行中，使用`"..."`还包括默认值。

# # 4 b。使用`pixie format`检查数据形状

在参考跟踪中使用`pixie format`来查看准确的数据形状**和**以数据集输入格式输出的真实应用程序：```bash
uv run pixie format --input reference-trace.jsonl --output dataset-sample.json
```
输出如下所示：```json
{
  "input_data": {
    "user_message": "What are your business hours?"
  },
  "eval_input": [
    {
      "name": "customer_profile",
      "value": { "name": "Alice", "tier": "gold" }
    },
    {
      "name": "conversation_history",
      "value": [{ "role": "user", "content": "What are your hours?" }]
    }
  ],
  "expectation": null,
  "eval_output": {
    "response": "Our business hours are Monday to Friday, 9am to 5pm..."
  }
}
```
**重要**：本模板中的`eval_output`是运行应用程序产生的**完整真实输出**。不要将`eval_output`复制到您的数据集条目中-它会通过评估器提供真实答案，从而使测试轻松通过。而不是:

—使用`input_data`和`eval_input`作为数据键和格式的精确模板
-查看`eval_output`以了解应用程序产生的内容-然后编写简洁的`expectation`描述**，捕获每个场景的关键质量标准

**示例**：如果`eval_output.response`是`"Our business hours are Monday to Friday, 9 AM to 5 PM, and Saturday 10 AM to 2 PM."`，则将`expectation`写成`"Should mention weekday hours (Mon–Fri 9am–5pm) and Saturday hours"`——一个人类或LLM评估器可以比较的简短描述。

# # 4 b”。为`eval_input`捕获外部内容（强制）**CRITICAL**：如果应用程序有任何`wrap(purpose="input")`调用，每个数据集条目必须提供相应的`eval_input`值和**预捕获的真实内容**。一个空的`eval_input`列表意味着应用程序将在每次eval运行期间进行实时外部调用（HTTP请求、数据库查询、API调用）——这使得eval运行缓慢、不稳定且不可复制。

为什么这很重要

在`pixie test`期间，应用程序中的每个`wrap(purpose="input", name="X")`调用都会检查wrap注册表中名为`"X"`的值：

- **如果找到**：直接返回注册值（无需外部调用）
- **如果没有找到**：真正的外部调用执行（不确定，缓慢，可能失败）`eval_input: []`项表示注册表中没有任何内容，因此每个外部依赖项都运行。这违背了检测的目的。

如何捕获内容对于应用程序中的每个`wrap(purpose="input", name="X")`，您必须捕获一次真实数据并将其嵌入数据集中。选择以下方法之一：

**选项A -使用引用跟踪**（首选）：

来自步骤2c的引用跟踪已经包含了为每个`purpose="input"`包装捕获的值。提取:```bash
# View the reference trace to find input wrap values
grep '"purpose": "input"' pixie_qa/reference-trace.jsonl
```
或者使用`pixie format`查看数据集条目格式的数据—输出中的`eval_input`数组已经具有具有正确名称和形状的捕获值。

**选项B -直接获取内容**（对于不同输入的新条目）：

当创建具有不同输入源的数据集条目时（例如，不同的url，不同的查询），通过运行一次依赖代码来捕获内容：```python
# Example: for a web scraper, run the app's own fetch logic once
from myapp.fetcher import fetch_page
page_content = fetch_page(target_url)  # use the app's real code path
```
然后将捕获的内容包含在条目的`eval_input`中：```json
{
  "eval_input": [
    {
      "name": "fetch_result",
      "value": "<captured page content here>"
    }
  ]
}
```
**选项C -运行`pixie trace`与每个输入**（最彻底）：

对于每一组`input_data`，运行`pixie trace`来执行真正依赖的应用程序并捕获所有值：```bash
pixie trace --runnable pixie_qa/run_app.py:AppRunnable --input  trace-input.json
```
然后从结果跟踪中提取`purpose="input"`值，并将它们用作`eval_input`。

###内容格式`eval_input`值必须与`wrap()`调用返回的类型和格式精确匹配。检查引用跟踪以查看应用程序生成的格式：

-如果换行捕获字符串（例如，HTML内容，markdown文本），则该值为字符串
-如果wrap捕获一个dict（例如，数据库记录），该值是一个JSON对象
—如果换行捕获列表，则该值为JSON数组

**不要跳过此步骤。**每个`wrap(purpose="input")`在应用程序必须有一个相应的`eval_input`条目在每一个数据集行。如果在应用程序有输入包装时继续使用空的`eval_input`， eval将是不可靠的。

# # 4 c。生成数据集项

根据参考跟踪和用例创建不同的条目：- **`input_data`键**必须匹配`Runnable.run(args: T)`中使用的Pydantic模型的字段
**`eval_input`**必须是一个`{"name": ..., "value": ...}`对象的列表，该对象与应用程序中`wrap(purpose="input")`调用的`name`值相匹配
- **涵盖`pixie_qa/02-eval-criteria.md`的每个用例** -每个用例至少一个条目，每个条目具有有意义的不同输入

**如果用户在提示符**中指定了一个数据集或数据源（例如，一个带有研究问题或对话场景的JSON文件），读取该文件，将每个条目适应`input_data`/`eval_input`形状，并将它们合并到数据集中。不要忽略指定的数据。

###入口质量检查表

在最终确定数据集之前，根据以下标准验证每个条目：

* *输入现实主义* *:-`eval_input`包含尊重合成边界的世界数据（见步骤2c）？用户创建的参数很好；世界数据应该是有来源的，而不是凭空捏造的。`eval_input`中的世界数据是否符合`00-project-analysis.md`“现实输入特性”中描述的规模和复杂性？如果分析说输入通常是5KB-500KB，那么200个字符的输入是不现实的。
-从输入中提取提示的答案是否不平凡？如果答案是在一个明确标记的HTML标签或第一句话中，那么测试就不能测试提取的质量。

* * * *场景多样性:参赛作品是否涵盖了有意义的不同难度水平，而不仅仅是相同难度的不同主题？`00-project-analysis.md`中至少有一个目标失败模式，你认为可能会导致分数下降（不是保证通过）吗？
-条目是否使用不同的输入数据结构模式（不只是不同的内容倒进同一个模板）？

* *校准困难* *:

-至少有一个条目你真的不确定是否应用程序将正确处理？如果您确信每个条目都可以轻松通过，那么数据集就太简单了。
-考虑加入一个具有挑战性的条目，探索已知的限制-“压力测试”条目。如果通过了，太好了。如果失败，则表明eval可以捕获真正的问题。

数据集条目的反模式- **制作世界数据**：手工制作应用程序通常从外部来源获取的内容（例如，为web scraper编写HTML，为RAG系统编写“检索文档”）。这消除了现实世界的复杂性。
- **统一难度**：所有条目具有相同的复杂程度。真实的工作负载有一个分布——一些简单，一些困难，一些边缘情况。
- **显而易见的答案**：每个条目都有清晰标记和明确的目标信息。真实数据的答案往往是分散的，部分存在的，有变化的重复的，或者嵌入噪声的。
- **双向作者身份**：输入和预期输出都是你写的，所以你确切地知道那里有什么。一个真正的评估者测试应用程序是否能找到它以前没有见过的信息。
- **只有快乐路径**：没有条目测试错误条件、边缘情况或已知的故障模式。
- **从t构建所有条目**：如果所有条目都有类似的`input_data`和类似的`eval_input`数据，则数据集测试没有任何意义。每个条目应该代表一个有意义的不同场景。
**重用项目自己的测试fixture作为eval数据**：项目的`tests/`、`fixtures/`、`examples/`和`mock_server/`目录包含为unit/integration测试设计的数据——小、干净、确定且非常容易。使用它们作为`eval_input`数据可以保证100%的通过率和零质量信号。即使这些固定装置看起来很方便，但它们绕过了让应用程序工作困难的所有现实困难。**运行生产代码来捕获真实的数据**，或者从`00-project-analysis.md`生成匹配scale/complexity的合成数据。
- **使用项目的mock/fake实现**：如果项目在其测试基础设施中包含模拟llm，假HTTP服务器或存根服务，不要在你的eval管道中使用它们。你的eval必须用实际复杂的数据来测试应用程序的真实代码路径——而不是项目自己的测试快捷方式。# # 4 c”。根据项目分析验证覆盖范围

在编写最终的数据集JSON之前，打开`pixie_qa/00-project-analysis.md`并检查：1. **真实的输入特征**：对于列出的每个特征（大小，复杂性，噪声，多样性），确认至少一个数据集条目反映了它。如果分析显示“带有导航和广告的混乱输入”，则至少有一个条目的`eval_input`应该包含带有导航和广告的混乱数据。
2. **故障模式**：对于列出的每种故障模式，请确认至少有一个数据集条目被设计用于执行它。条目不需要保证失败——但是它应该创建一个条件，使失败模式_可以显示出来。如果当前的仪器设置不能执行故障模式，请在`02-eval-criteria.md`中添加注释，解释原因。
3. **能力覆盖**：确认数据集涵盖了评估标准（步骤1c）中列出的能力。每个涵盖的功能应该至少有一个条目。

如果发现有任何间隙，添加条目将其关闭，然后继续进行4d。# # 4 c”。停止检查-数据集真实性审计（硬门）

这是一扇硬门。**在每个检查通过之前不要进行4d。如果检查失败，请修改数据集并重新审核。

在编写最终的数据集JSON之前，执行以下自我审计：

1. **交叉参考`00-project-analysis.md`**：打开“逼真输入特性”部分。对于每个特征（大小、复杂性、噪声、结构），验证至少一个数据集条目的`eval_input`反映了它。如果分析说“5KB-500KB的HTML页面与导航chrome和广告”，你最大的`eval_input`是1KB干净的HTML， **数据集是不现实的-添加更难的条目2. **计数不同的数据源**：数据集中有多少唯一的`eval_input`数据源？如果超过50%的条目共享相同的`eval_input`内容（即使使用不同的提示），则数据集缺乏多样性。相同输入的提示变量测试的是LLM的解释，而不是应用程序的数据处理。

3. **难度分布（强制阈值）**：对于每个条目，将其标记为“常规”（相信它会通过），“中等”（可能通过但并非微不足道）或“具有挑战性”（真正不确定或针对已知的失败模式）。

- **最多60%的“例程”条目。**如果你有5个条目，最多3个可以是常规的。
**至少有一个“具有挑战性”的条目**，目标是`00-project-analysis.md`的失败模式，在那里你真的不确定结果。如果每个条目都是保证通过的，那么数据集就无法区分好应用程序和坏应用程序。4. **能力覆盖（强制阈值）**：计算至少一个数据集条目执行了`00-project-analysis.md`中的多少能力。

- **必须覆盖所列功能的50%以上。**如果分析列出6个功能，数据集必须至少执行3个。
—如果覆盖率低于阈值，则添加针对未覆盖功能的条目。

5. **项目夹具污染检查**：扫描每个`eval_input`值。是否有任何数据来自项目的`tests/`、`fixtures/`、`examples/`或模拟服务器目录？如果是，请**将其替换为实际数据。**这些夹具是为了开发方便而设计的，而不是为了评估现实性。6. **同义性检查**：测试管道会产生有意义的分数，还是一个闭环？如果输入数据和求值器逻辑都是由你编写的，这样传递是由构造保证的（例如，在手工编写的HTML上，正则表达式提取器+精确匹配求值器），那么这个管道是重复的，无法捕捉到真正的问题。应用程序的真正LLM应该产生输出，评估人员应该评估可能真正失败的质量维度。

7. **`eval_input`完整性检查**：对于仪器化应用程序代码中的每个`wrap(purpose="input", name="X")`调用，验证每个数据集条目是否提供了一个对应的`eval_input`项，其中包含`"name": "X"`和一个非空的`"value"`。如果任何条目有`eval_input: []`，而应用程序有输入包装，**数据集是不完整的-捕获的内容丢失。**回到步骤4b '并捕获内容。

# # 4 d。构建数据集JSON文件

在`pixie_qa/datasets/<name>.json`创建数据集：```json
{
  "name": "qa-golden-set",
  "runnable": "pixie_qa/run_app.py:AppRunnable",
  "evaluators": ["Factuality", "pixie_qa/evaluators.py:ConciseVoiceStyle"],
  "entries": [
    {
      "input_data": {
        "user_message": "What are your business hours?"
      },
      "description": "Customer asks about business hours with gold tier account",
      "eval_input": [
        {
          "name": "customer_profile",
          "value": { "name": "Alice Johnson", "tier": "gold" }
        }
      ],
      "expectation": "Should mention Mon-Fri 9am-5pm and Sat 10am-2pm"
    },
    {
      "input_data": {
        "user_message": "I want to change something"
      },
      "description": "Ambiguous change request from basic tier customer",
      "eval_input": [
        {
          "name": "customer_profile",
          "value": { "name": "Bob Smith", "tier": "basic" }
        }
      ],
      "expectation": "Should ask for clarification",
      "evaluators": ["...", "ClosedQA"]
    },
    {
      "input_data": {
        "user_message": "I want to end this call"
      },
      "description": "User requests call end after failed verification",
      "eval_input": [
        {
          "name": "customer_profile",
          "value": { "name": "Charlie Brown", "tier": "basic" }
        }
      ],
      "expectation": "Agent should call endCall tool and end the conversation",
      "eval_metadata": {
        "expected_tool": "endCall",
        "expected_call_ended": true
      },
      "evaluators": ["...", "pixie_qa/evaluators.py:tool_call_check"]
    }
  ]
}
```
关键字段

**条目结构** -所有字段在每个条目上都是顶级的（扁平结构-没有嵌套）：```
entry:
  ├── input_data    (required) — args for Runnable.run()
  ├── eval_input      (optional) — list of {"name": ..., "value": ...} objects (default: [])
  ├── description     (required) — human-readable label for the test case
  ├── expectation     (optional) — reference for comparison-based evaluators
  ├── eval_metadata   (optional) — extra per-entry data for custom evaluators
  └── evaluators      (optional) — evaluator names for THIS entry
```
* *顶级领域:* *

- **`runnable`**（必选）：`filepath:ClassName`对步骤2中的`Runnable`类的引用（例如，`"pixie_qa/run_app.py:AppRunnable"`）。路径相对于项目根目录。
- **`evaluators`**（数据集级别，可选）：应用于每个条目的默认评估器名称-适用于所有用例的标准的评估器。

**每个条目字段（每个条目的所有顶级字段）：**- **`input_data`**（必选）：键匹配`Runnable.run(args: T)`的Pydantic模型字段。这些是应用程序的输入数据。
- **`eval_input`**（可选，默认为`[]`）：`{"name": ..., "value": ...}`对象列表。名称匹配应用程序中的`wrap(purpose="input")`名称。运行程序在构建`Evaluable`时自动添加`input_data`。
- **`description`**（必选）：`pixie_qa/02-eval-criteria.md`的用例一行。
- **`expectation`**（可选）：用于需要引用的求值器的特定于案例的期望文本。
- **`eval_metadata`**（可选）：为自定义求值器提供额外的每个条目数据-例如，预期的工具名称，布尔标志，阈值。在求值器中以`evaluable.eval_metadata`的形式访问。
- **`evaluators`**（可选）：行级求值器覆盖。

求值器分配规则1. 应用于所有项的求值器放在顶级的`"evaluators"`数组中。
2. 需要**额外**求值器的项使用`"evaluators": ["...", "ExtraEval"]`-`"..."`展开为默认值。
3. 需要完全不同设置的项目使用`"evaluators": ["OnlyThis"]`，而不使用`"..."`。
4. 只使用默认值的项：省略`"evaluators"`字段。

---

数据集创建参考

使用`eval_input`值`eval_input`值是`{"name": ..., "value": ...}`对象。使用参考跟踪作为模板-从相关的`purpose="input"`事件中复制`"data"`字段并调整其值：

* *简单的dict * *:```json
{ "name": "customer_profile", "value": { "name": "Alice", "tier": "gold" } }
```
**字典列表**（例如，会话记录）：```json
{
  "name": "conversation_history",
  "value": [
    { "role": "user", "content": "Hello" },
    { "role": "assistant", "content": "Hi there!" }
  ]
}
```
**重要提示：确切的格式取决于`wrap(purpose="input")`调用捕获的内容。总是从引用跟踪中复制，而不是从头开始构造。

制作不同的评估场景

覆盖每个用例的不同方面。能力清单和失效模式参见**`pixie_qa/00-project-analysis.md`**：

- **覆盖每个能力** -每个能力至少有一个条目，而不仅仅是主要的能力
- **目标失效模式** -包括项目分析中列出的疑难问题/失效模式的条目（例如，错误输入、边缘情况、复杂场景）
—相同请求的不同用户措辞
边缘情况（输入不明确、信息缺失、错误条件）
-压力测试特定评估标准的条目
-步骤1c中每个用例至少有一个条目

---

# #输出`pixie_qa/datasets/<name>.json`-数据集文件。