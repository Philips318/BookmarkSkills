#步骤2c：捕获并验证引用跟踪

**目标**：通过Runnable运行应用程序，捕获跟踪，并验证仪表和Runnable是否正常工作。跟踪证明一切都连接好了，并提供了在步骤4中创建数据集所需的确切数据形状。

---

##选择跟踪输入

跟踪输入确定捕获哪些代码路径。一个微不足道的输入会产生一个微不足道的跟踪，而忽略了应用程序的真实行为。

输入必须反映“实际输入特性”部分，根据您在步骤2b中阅读的`pixie_qa/00-project-analysis.md`。

输入有两个部分-理解它们之间的边界：- **用户提供的参数**（由您编写）：真正的用户类型或配置的内容——提示、查询、配置标志、url、模式定义。编写这些以代表实际用法。
- **世界数据**（从生产代码中捕获，而不是编造）：应用程序在执行期间从外部来源获取的内容-数据库记录，API响应，文件等。运行一次生产代码，将此数据捕获到跟踪中。只有在以下情况下才使用合成数据生成：
-用户明确指示您使用合成数据OR
-从真实资源中获取是不切实际的（获取太多，会产生实际的货币成本，或者花费不合理的时间-超过~30分钟）

**输入前快速检查：“是真正的用户创建数据，还是应用程序从其他地方获取数据？”如果应用程序获得了它，让生产代码运行并捕获它。|应用类型|用户提供（你的作者）|世界提供（你的来源）|| -------------------- | ------------------------------------- | ------------------------------------------------------------------ |
b| Web scraper | URL +提示符+架构定义| HTML页面内容|
|研究代理|研究问题+范围约束|源文档，搜索结果|
|客户支持bot |客户的语音消息|来自CRM的客户配置文件，会话存储|的会话历史
|代码审查工具| PR URL +审查标准|实际差异，文件内容，CI结果|

###捕获多个轨迹

在构建数据集之前，捕获至少2条具有不同输入特征的迹线。

-不同的复杂性（简单案例vs复杂案例）
-不同的能力（见`00-project-analysis.md`能力清单）
-不同的边缘条件（缺少可选数据，异常大的输入）这种校准防止了数据集的同质性——你可以看到应用程序实际上是如何处理不同的输入的。

---

##运行`pixie trace`**首先**验证app是否可以导入：`python -c "from <module> import <class>"`。在进入跟踪-安装-重试循环之前捕获丢失的包。```bash
# Create a JSON file with input data
echo '{"user_message": "a realistic sample input"}' > pixie_qa/sample-input.json

uv run pixie trace --runnable pixie_qa/run_app.py:AppRunnable \
  --input pixie_qa/sample-input.json \
  --output pixie_qa/reference-trace.jsonl
```
`--input`标志将**文件路径**获取到JSON文件（不是内联JSON）。JSON键变成Pydantic模型的键值。

对于其他跟踪：```bash
uv run pixie trace --runnable pixie_qa/run_app.py:AppRunnable \
  --input pixie_qa/sample-input-complex.json \
  --output pixie_qa/trace-complex.jsonl
```
---

验证跟踪

###快速检查

跟踪JSONL包含每个`wrap()`事件一行和每个LLM跨度一行：```jsonl
{"type": "kwargs", "value": {"user_message": "What are your hours?"}}
{"type": "wrap", "name": "customer_profile", "purpose": "input", "data": {...}, ...}
{"type": "llm_span", "request_model": "gpt-4o", "input_messages": [...], ...}
{"type": "wrap", "name": "response", "purpose": "output", "data": "Our hours are...", ...}
```
检查:

-出现预期的`wrap`项（代码中每个`wrap()`调用一个）
-至少出现一个`llm_span`条目（确认真正的LLM调用）
—缺少条目表示执行路径与预期不同—在继续之前修复

格式化并验证覆盖率

运行`pixie format`查看数据集条目格式的数据：```bash
pixie format --input trace.jsonl --output dataset_entry.json
```
输出显示：

-`input_data`：可运行参数的确切keys/values—`eval_input`：来自`wrap(purpose="input")`调用的数据
-`eval_output`：实际的应用程序输出（从`wrap(purpose="output")`）

对于来自`pixie_qa/02-eval-criteria.md`的每个eval标准，验证格式输出是否包含所需的数据。如果缺少数据点，请返回步骤2a并添加`wrap()`调用。

跟踪审计

在继续步骤3之前，审计每个跟踪：

1. **世界数据检查**：对于每个`wrap(purpose="input")`字段，数据是否实际复杂？与`00-project-analysis.md`“实际输入特性”进行比较。如果分析显示输入为5KB - 500kb，而您的输入低于5KB，则不具有代表性。

2. **LLM跨度检查**：是否出现`llm_span`条目？如果没有，则应用程序的LLM调用没有触发- Runnable可能配置错误或LLM可能是mocked/faked.在继续之前修复这个问题。3. **复杂性检查**：跟踪是否执行了`00-project-analysis.md`中的难题？如果它只执行快乐路径，则使用更困难的输入捕获额外的跟踪。

如果任何检查失败，返回并修复输入或Runnable，然后重新捕获。

---

# #输出

-`pixie_qa/reference-trace.jsonl`-包含所有预期包装事件和LLM跨度的引用跟踪
-不同输入的附加跟踪文件