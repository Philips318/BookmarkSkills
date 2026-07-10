#步骤2a：仪器`wrap`有关完整的`wrap()`API参考，请参见`wrap-api.md`。

**目标**：在数据边界添加`wrap()`调用，这样eval就可以(1)注入受控的输入来代替真正的外部依赖，(2)捕获输出进行评分。

---

数据流分析

从LLM调用站点开始，通过代码前后跟踪找到：

依赖输入：来自外部系统的数据（数据库，api，缓存，文件系统，网络获取）
- **App输出**：向用户或外部系统输出的数据
-中间状态：与评估相关的内部决策（路由，工具调用）

您不需要包装LLM调用参数或响应-这些已经被OpenInference自动检测捕获。

添加`wrap()`调用

对于找到的每个数据点，在应用程序代码中添加一个`wrap()`调用：```python
import pixie

# External dependency data — function form (prevents the real call in eval mode)
profile = pixie.wrap(db.get_profile, purpose="input", name="customer_profile",
    description="Customer profile fetched from database")(user_id)

# External dependency data — function form (prevents the real call in eval mode)
history = pixie.wrap(redis.get_history, purpose="input", name="conversation_history",
    description="Conversation history from Redis")(session_id)

# App output — what the user receives
response = pixie.wrap(response_text, purpose="output", name="response",
    description="The assistant's response to the user")

# Intermediate state — internal decision relevant to evaluation
selected_agent = pixie.wrap(selected_agent, purpose="state", name="routing_decision",
    description="Which agent was selected to handle this request")
```
值与函数包装```python
# Value form: wrap a data value (result already computed)
profile = pixie.wrap(db.get_profile(user_id), purpose="input", name="customer_profile")

# Function form: wrap the callable — in eval mode the original function is
# NOT called; the registry value is returned instead.
profile = pixie.wrap(db.get_profile, purpose="input", name="customer_profile")(user_id)
```
**关键：总是使用函数形式的`purpose="input"`包装的外部调用** - HTTP请求，数据库查询，API调用，文件读取，缓存查找。函数形式防止在eval模式下执行实际调用，因此直接返回数据集值，而无需进行实时网络请求或数据库查询。值形式仍然首先执行实际调用，然后才替换结果——这浪费了时间，创建了不可靠的测试，并使评估依赖于外部服务的可用性。

唯一可以接受`purpose="input"`的值形式的情况是，包装后的值是重新计算成本较低的局部计算（没有I/O，没有副作用）。

放置规则1. **在数据边界处包装——数据进入或退出应用程序的地方，而不是在实用程序函数的深处。
2. **名称在整个应用程序中必须是唯一的（用作注册表项和数据集字段名）。
3. **使用`lower_snake_case`**作为名称。
4. **不要改变函数的接口** -`wrap()`是纯粹的加法，返回相同的类型。

###按目的放置

####`purpose="input"`-外部数据进入

将输入换行放在外部数据进入应用程序的边界处，而不是中间处理阶段。在流水线架构中（获取→处理→提取→格式化）：- **正确**:`wrap(fetch_page, purpose="input", name="fetched_page")(url)`使用**函数形式**在HTTP获取边界-在eval模式下，获取被完全跳过，并返回数据集值；在跟踪模式下，将运行实际的获取并捕获结果。
- **不正确**:`wrap(html_content, purpose="input", name="fetched_page")`使用值形式- HTTP获取仍然在eval模式下运行（浪费时间和创建片状测试），之后只有结果被替换。
- **不正确**：解析后的`wrap(processed_chunks, purpose="input", name="chunks")`- eval模式完全绕过解析和分块。

**原理**:`wrap(purpose="input")`在执行_maximum内部逻辑_的同时取代了_minimum外部依赖_。把边界推到尽可能远的上游。对于外部调用的输入包装总是使用函数形式——这可以防止真正的调用在eval模式下执行。

####`purpose="output"`-处理过的数据存在的地方跟踪LLM响应的下游，以找到数据离开应用程序的位置——发送给用户，写入存储，在UI中呈现，或传递给外部系统。在出口边界处进行包装。

-不要包装原始的LLM响应-那些已经被OpenInference自动检测捕获为`llm_span`条目。
-包装应用程序的**最终处理的结果** -任何后处理，格式化，或转换应用程序适用于LLM输出。
-如果应用程序有多个输出通道（例如，对用户的响应和对数据库的副作用写入），分别包装每个通道。```python
# Final response after the app's formatting pipeline
response = pixie.wrap(formatted_response, purpose="output", name="response",
    description="Final response sent to the user")

# Side-effect output — data written to external storage
pixie.wrap(saved_record, purpose="output", name="saved_summary",
    description="Summary record saved to the database")
```
**原则**：输出包装仅用于观察-它们捕获应用程序产生的内容，以便评估者可以对其进行评分。它们永远不会在eval运行期间被模拟或注入。

####`purpose="state"`-与评价相关的内部决策

需要一些评估标准来判断应用程序的内部推理——不仅仅是输入或输出的内容，而是应用程序如何做出决策。当eval条件要求并且数据在输入或输出中不可见时，包装内部状态。

常见的例子:

—**代理路由**：选择哪个子代理或工具来处理请求
**Plan/stepdecisions**：代理选择执行哪些步骤
- **内存更新**：代理从其工作内存中添加或删除的内容
- **检索结果**：哪些documents/chunks在被馈送到LLM之前被检索到```python
# Agent routing decision
selected_agent = pixie.wrap(selected_agent, purpose="state", name="routing_decision",
    description="Which agent was selected to handle this request")

# Retrieved context fed to LLM
pixie.wrap(retrieved_chunks, purpose="state", name="retrieved_context",
    description="Document chunks retrieved by RAG before LLM call")
```
**原则**：只包装eval条件实际需要的状态。不要包装每个变量——状态包装是用于内部数据的，求值器必须看到这些数据，但这些数据不会出现在应用的输入或输出中。

覆盖率检查

在添加了所有`wrap()`调用之后，遍历`pixie_qa/02-eval-criteria.md`的每个eval条件并验证：

1. 每个判断**进入**的标准都有相应的`input`或`entry`包装。
2. 判断**结果**的每一个标准都有一个相应的`output`包装。
3. 判断应用程序如何决定的每一个标准都有一个相应的`state`包装。

如果一个标准需要的数据没有被捕获，那么现在就添加包装——不要推迟。

---

# #输出

在数据边界使用`wrap()`调用修改了应用程序源文件。