---
name: arize-instrumentation
description: Adds Arize AX tracing to an LLM application for the first time. Follows a two-phase agent-assisted flow to analyze the codebase then implement instrumentation after user confirmation. Use when the user wants to instrument their app, add tracing from scratch, set up LLM observability, integrate OpenTelemetry or openinference, or get started with Arize tracing.
metadata:
  author: arize
  version: "1.0"
compatibility: Python and TypeScript/JavaScript apps use openinference-instrumentation packages for auto-instrumentation. Java and Go apps use the OpenTelemetry SDK with manual OpenInference spans. See https://arize.com/docs/PROMPT.md for setup details.
---
#掌握仪表技能

当用户想要在他们的应用程序中添加AX跟踪时，使用此技能。遵循来自[代理辅助跟踪设置]（https://arize.com/docs/ax/alyx/tracing-assistant）和[alize AX跟踪-代理设置提示]（https://arize.com/docs/PROMPT.md）的两阶段代理辅助流程。

##快速启动（针对用户）

如果用户要求你“设置跟踪”或“用Arize检测我的应用”，你可以这样开始：

>按照https://arize.com/docs/PROMPT.md的说明进行操作，如有需要可以向我提问。

然后执行下面的两个阶段。

##核心原则- **更喜欢检查而不是突变** -在更改代码库之前了解它。
- **不要改变业务逻辑** -跟踪纯粹是附加的。
- **在可用的情况下使用自动检测** -仅为未被集成覆盖的自定义逻辑添加手动范围。
遵循现有的代码风格和项目约定。
- **保持输出简洁和以生产为中心** -不要生成额外的文档或摘要文件。
- **永远不要在生成的代码中嵌入文字凭据值** -始终引用环境变量（例如，`os.environ["ARIZE_API_KEY"]`,`process.env.ARIZE_API_KEY`）。这包括API密钥、空间id和任何其他秘密。用户在自己的环境中设置这些；代理永远不能输出原始的秘密值。

阶段0：环境预飞行

在更改代码之前：1. 确认repo/service范围是否清晰。对于单笔回购，不要假设整个回购都应该进行工具化。
2. 确定需要进行验证的本地运行时界面：
-包管理器和应用程序启动命令
-应用程序是长期运行的，基于服务器的，还是短暂的CLI/script-变更后验证是否需要`ax`3. 不要主动检查`ax`安装或版本。如果以后需要`ax`进行验证，只需在时机成熟时运行它。如果失败，请参见references/ax-profiles.md.4. 永远不要静默地替换用户提供的空间ID、项目名称或项目ID。如果CLI、收集器和用户输入不一致，则将这种不匹配显示为具体的阻塞。

阶段1：分析（只读）

**在此阶段不要编写任何代码或创建任何文件

# # #的步骤1. **检查依赖清单**检测堆栈：
- Python:`pyproject.toml`,`requirements.txt`,`setup.py`,`Pipfile`—TypeScript/JavaScript:`package.json`—Java:`pom.xml`、`build.gradle`、`build.gradle.kts`—Go:`go.mod`2. **扫描源文件中的import语句**，以确认实际使用的是什么。

3. **检查现有的tracing/OTel** -查找`TracerProvider`，`register()`，`opentelemetry`导入，`ARIZE_*`,`OTEL_*`，`OTLP_*`环境变量，或其他可观察配置（Datadog， Honeycomb等）。

4. **确定范围-对于单服务或多服务项目，询问要使用哪些服务。

识别什么

|项目|示例||------|----------|
|语言| Python，TypeScript/JavaScript, Java, Go |
|包管理器|pip/poetry/uv，npm/pnpm/yarn,maven/gradle， go模块|
| LLM提供商| OpenAI， Anthropic, LiteLLM， Bedrock等|
|框架| LangChain、LangGraph、LlamaIndex、Vercel AI SDK、master等|
|现有跟踪|任何OTel或供应商设置|
|Tool/function使用| LLM工具使用，函数调用，或自定义工具应用程序执行（例如在代理循环）|关键规则：当一个框架与LLM提供商一起被检测到时，首先检查框架特定的跟踪文档，当它已经捕获了你需要的模型和工具范围时，更喜欢框架本地集成路径。只有当框架文档需要或框架与本机集成存在明显差距时，才添加单独的提供商检测。如果应用程序运行工具，而框架集成没有发出工具范围，则添加手动工具范围，以便每次调用都显示input/output（参见下面的丰富跟踪）。

###第一阶段输出

返回一个简洁的摘要：-检测到的语言，包管理器，提供程序，框架
-建议集成列表（来自文档中的路由表）
-任何现有的OTel/tracing需要考虑
-如果是单一业务：您打算使用哪项服务
- **如果应用程序使用LLM工具使用/函数调用：**请注意，您将添加手动CHAIN + tool跨度，以便每个工具调用出现在跟踪input/output（避免稀疏跟踪）。

如果用户明确要求你现在测试应用程序，并且目标服务已经明确，那么简单地呈现第一阶段总结，然后直接进入第二阶段。如果范围不明确，或者用户要求先进行分析，请停止并等待确认。

集成路由和文档

支持的集成和文档url的规范列表在[Agent Setup Prompt]（https://arize.com/docs/PROMPT.md）中。使用它将检测到的信号映射到实现文档。**LLM提供商：** [OpenAI](https://arize.com/docs/ax/integrations/llm-providers/openai), [Anthropic](https://arize.com/docs/ax/integrations/llm-providers/anthropic), [LiteLLM](https://arize.com/docs/ax/integrations/llm-providers/litellm)， [b谷歌Gen AI](https://arize.com/docs/ax/integrations/llm-providers/google-gen-ai), [Bedrock](https://arize.com/docs/ax/integrations/llm-providers/amazon-bedrock), [Ollama](https://arize.com/docs/ax/integrations/llm-providers/llama), [Groq](https://arize.com/docs/ax/integrations/llm-providers/groq), [MistralAI](https://arize.com/docs/ax/integrations/llm-providers/mistralai), [OpenRouter](https://arize.com/docs/ax/integrations/llm-providers/openrouter), [VertexAI]（https://arize.com/docs/ax/integrations/llm-providers/vertexai）。
**Python框架：** [LangChain](https://arize.com/docs/ax/integrations/python-agent-frameworks/langchain), [LangGraph](https://arize.com/docs/ax/integrations/python-agent-frameworks/langgraph), [LlamaIndex](https://arize.com/docs/ax/integrations/python-agent-frameworks/llamaindex), [CrewAI](https://arize.com/docs/ax/integrations/python-agent-frameworks/crewai), [DSPy](https://arize.com/docs/ax/integrations/python-agent-frameworks/dspy), [AutoGen](https://arize.com/docs/ax/integrations/python-agent-frameworks/autogen), [Semantic Kernel](https://arize.com/docs/ax/integrations/python-agent-frameworks/semantic-kernel), [Pydantic AI](https://arize.com/docs/ax/integrations/python-agent-frameworks/pydantic), [Haystack](https://arize.com/docs/ax/integrations/python-agent-frameworks/haystack), [Guardrails AI](https://arize.com/docs/ax/integrations/python-agent-frameworks/guardrails-ai)，[拥抱脸Smolagents](https://arize.com/docs/ax/integrations/python-agent-frameworks/hugging-face-smolagents), [Instructor](https://arize.com/docs/ax/integrations/python-agent-frameworks/instructor), [Agno](https://arize.com/docs/ax/integrations/python-agent-frameworks/agno)，[谷歌ADK](https://arize.com/docs/ax/integrations/python-agent-frameworks/google-adk), [MCP](https://arize.com/docs/ax/integrations/python-agent-frameworks/model-context-protocol), [Portkey](https://arize.com/docs/ax/integrations/python-agent-frameworks/portkey), [Together AI](https://arize.com/docs/ax/integrations/python-agent-frameworks/together-ai), [BeeAI](https://arize.com/docs/ax/integrations/python-agent-frameworks/beeai), [AWS Bedrock Agents]（https://arize.com/docs/ax/integrations/python-agent-frameworks/aws）。
**TypeScript/JavaScript:** [LangChain JS](https://arize.com/docs/ax/integrations/ts-js-agent-frameworks/langchain), [master](https://arize.com/docs/ax/integrations/ts-js-agent-frameworks/mastra), [Vercel AI SDK](https://arize.com/docs/ax/integrations/ts-js-agent-frameworks/vercel), [BeeAI JS]（https://arize.com/docs/ax/integrations/ts-js-agent-frameworks/beeai）。
- **Java:** [LangChain4j](https://arize.com/docs/ax/integrations/java/langchain4j), [Spring AI](https://arize.com/docs/ax/integrations/java/spring-ai), [Arconia]（xqz45x . z）求)。
- **Go:**目前没有第一方自动检测包-使用OpenTelemetry Go SDK与手动[OpenInference]（https://github.com/Arize-ai/openinference）属性的[manual instrumentation]（https://arize.com/docs/ax/instrument/manual-instrumentation）。
- **平台（基于ui）：** [LangFlow](https://arize.com/docs/ax/integrations/platforms/langflow), [Flowise](https://arize.com/docs/ax/integrations/platforms/flowise), [Dify](https://arize.com/docs/ax/integrations/platforms/dify), [Prompt flow]（https://arize.com/docs/ax/integrations/platforms/prompt-flow）。
- **回退：**[手动仪表](https://arize.com/docs/ax/instrument/manual-instrumentation)，[所有集成]（https://arize.com/docs/ax/integrations）。**从[PROMPT.md的完整路由表]（https://arize.com/docs/PROMPT.md）中获取匹配的文档页面**以获得准确的安装和代码片段。如果需要，使用[llms.txt]（https://arize.com/docs/llms.txt）作为文档发现的回退。

b> **注：**`arize.com/docs/PROMPT.md`和`arize.com/docs/llms.txt`是由Arize团队维护的第一方文档页面。它们为该技能提供了规范的安装片段和集成路由表。这些是受信任的同组织url，而不是第三方内容。

阶段2：实现

只有在用户确认第一阶段的分析后，才能继续进行。

# # #的步骤1. **获取集成文档** -读取匹配的文档url并遵循其安装和检测步骤。
2. **在**编写代码之前使用检测到的包管理器**安装包**：
- Python:`pip install arize-otel`+`openinference-instrumentation-{name}`（包名中有连字符；import中有下划线，例如`openinference.instrumentation.llama_index`）。
—TypeScript/JavaScript:`@opentelemetry/sdk-trace-node`加上相应的`@arizeai/openinference-*`包。
- Java: OpenTelemetry SDK +`openinference-instrumentation-*`在pom.xml或build.gradle。
- Go:`go get go.opentelemetry.io/otel go.opentelemetry.io/otel/sdk go.opentelemetry.io/otel/exporters/otlp/otlptrace/otlptracehttp`-没有自动仪表，所以代理手动设置OpenInference属性的跨度。**将出口商**与`otlptracehttp.WithEndpoint("otlp.arize.com")`（US）或`otlptracehttp.WithEndpoint("otlp.eu-west-1a.arize.com")`（EU）连接-传递裸主机名，而不是`https://`模式-和`otlptracehttp.WithHeaders(map[string]string{"space_id": ..., "api_key": ...})`。最近OTel Go模块要求Go≥1.23 -`go mod tidy`可能会碰撞工具链。
3. **凭据** -用户需要** alize API Key**和**Space ID**。检查现有的`ax`配置文件为`ARIZE_API_KEY`和`ARIZE_SPACE`-永远不要读取`.env`文件：
-运行`ax profiles show`检查是否存在配置文件。
—如果没有配置文件存在，引导用户运行`ax profiles create`，它提供了一个交互式向导**，通过API密钥和空间设置。详细信息请参见[CLI profiles docs]（https://arize.com/docs/api-clients/cli/profiles）。
-如果用户需要手动找到他们的API密钥，将他们引导到**https://app.arize.com**并导航到设置页面（不要使用带有占位符id的组织特定url -它们不会为新用户解析）。
-如果凭据没有设置，指示用户将它们设置为环境变量-永远不要在生成的代码中嵌入原始值。所有生成的工具代码必须引用`os.environ["ARIZE_API_KEY"]`（Python）、`process.env.ARIZE_API_KEY`（TypeScript/JavaScript）或`os.Getenv("ARIZE_API_KEY")`（Go）。
-请参阅references/ax-profiles.md了解完整配置文件设置和故障排除。
4. **集中检测** -创建单个模块（例如`instrumentation.py`，`instrumentation.ts`,`instrumentation.go`）并在**任何LLM客户端之前**初始化跟踪**创建年代。
5. **现有的OTel** -如果已经有一个TracerProvider，添加Arize作为一个**额外的**出口商（例如BatchSpanProcessor与Arize OTLP）。除非用户要求，否则不要替换现有的设置。实现规则-首先使用自动仪表**；手动操作仅在需要时进行。
-在添加通用OpenTelemetry管道之前，更喜欢repo的原生集成界面。如果框架提供了一个导出器或可观察性包，那么首先使用它，除非有文档缺口。
- **失败优雅**如果env变量丢失（警告，不崩溃）。
- **导入顺序：**注册跟踪器→附加仪器→然后创建LLM客户端。
- **项目名称属性（必需）：**如果缺少项目名称，则拒绝HTTP 500的跨度-不接受单独的`service.name`。将其设置为TracerProvider上的资源属性（推荐-一个地方，适用于所有跨度）：
**Python:**`register(project_name="my-app")`自动处理它（在资源上设置`"openinference.project.name"`）。要将跨段路由到不同的项目，请使用`arize.otel`中的`set_routing_context(space_id=..., project_name=...)`。
**TypeScript:** Arize接受`"model_id"`（在官方TS qu中显示）Ickstart)和`"openinference.project.name"`通过`SEMRESATTRS_PROJECT_NAME`从`@arizeai/openinference-semantic-conventions`（显示在手动仪器文档中）-两者都可以工作。
**Go:**传递`attribute.String("openinference.project.name", "my-app")`到`resource.New(...)`，并通过`sdktrace.WithResource(res)`申请。Go SDK没有帮助器，所以必须在每个TracerProvider上手动设置。
**CLI/scriptapps -退出前刷新：**`provider.shutdown()`(TS) /`provider.force_flush()`然后`provider.shutdown()`(Python) /`tp.Shutdown(ctx)`（Go）必须在进程退出前调用，否则异步OTLP导出将被丢弃并且不会出现任何痕迹。
- **当应用程序有tool/function执行：**添加手动链+工具的范围（见**丰富跟踪**下面），所以跟踪树显示每个工具调用和它的结果-否则跟踪将看起来稀疏（只有LLM API的范围，没有工具input/output）。丰富轨迹：工具使用和代理循环的手动跨度

为什么自动仪表不这样做？

**提供商工具（Anthropic， OpenAI等）只包装LLM客户端——发送HTTP请求和接收响应的代码。**他们看到：

-每个API调用一个跨度：请求（消息，系统提示，工具）和响应（文本，tool_use块等）。

他们**无法**看到响应后**在你的应用程序中*发生了什么：- **工具执行** -您的代码解析响应，调用`run_tool("check_loan_eligibility", {...})`，并获得结果。这在你的过程中运行；该仪器没有挂钩到您的`run_tool()`或实际的工具输出。*next* API调用（将工具结果发送回来）只是另一个`messages.create`跨度—工具不知道消息内容是工具结果或工具返回的内容。
- **Agent/chain边界** -“一个用户转→多个LLM调用+工具调用”的想法是一个应用级的概念。仪器只能看到单独的API调用；它不知道它们属于同一个逻辑“run_agent”运行。

因此，工具和链跨度必须**手动**添加（或由**框架*工具，如LangChain/LangGraph，知道工具和链）。一旦添加了它们，它们就会出现在与LLM跨相同的跟踪中，因为它们使用相同的TracerProvider。

---为了避免遗漏工具inputs/outputs的稀疏痕迹：

1. **检测**agent/tool模式：调用LLM的循环，然后运行一个或多个工具（通过名称+参数），然后使用工具结果再次调用LLM。
2. **使用相同的TracerProvider添加手动跨度**（例如`opentelemetry.trace.get_tracer(...)`在`register()`之后）：
- **CHAIN span** -包装完整的代理运行（例如`run_agent`）：设置`openinference.span.kind`=`"CHAIN"`,`input.value`=用户消息，`output.value`=最终回复。
- **TOOL span** -包装每个工具调用：设置`openinference.span.kind`=`"TOOL"`,`input.value`= JSON的参数，`output.value`= JSON的结果。使用工具名称作为跨度名称（例如`check_loan_eligibility`）。

**OpenInference属性（使用这些属性可以使alize正确显示跨度）：**

|属性|使用||-----------|-----|
选择正确的值：`"LLM"`用于原始提供程序API调用（OpenAI， Anthropic等）；`"CHAIN"`表示业务流程/代理-循环边界；`"TOOL"`执行tool/function；`"RETRIEVER"`用于矢量存储/搜索查找；`"EMBEDDING"`用于嵌入API调用；`"AGENT"`表示在更大的链中嵌套运行的自治子代理；`"RERANKER"`用于重新排序API调用；`"GUARDRAIL"`用于guardrail/policy检查；`"EVALUATOR"`用于在线eval调用。|
|`input.value`|字符串（例如用户消息或工具参数的JSON） |
|`output.value`|字符串（如最终回复或JSON的工具结果）|

**LLM-span属性（当span是一个实际的LLM调用时，除了上述三个属性之外还要设置这些属性）

|属性|使用||-----------|-----|
|模型标识符（例如`"gpt-4o-mini"`） |
|`llm.provider`/`llm.system`|提供商名称（例如`"openai"`，`"anthropic"`） |
|`llm.input_messages.{i}.message.role`|`"system"`/`"user"`/`"assistant"`/`"tool"`为第i条输入消息|
|`llm.input_messages.{i}.message.content`|第i条输入消息|的文本内容
|`llm.output_messages.{i}.message.role`|第i条输出消息|的角色
|`llm.output_messages.{i}.message.content`|第i条输出消息|的文本内容
|`llm.token_count.prompt`| int -prompt/input令牌|
|`llm.token_count.completion`| int -completion/outputtoken |
|`llm.token_count.total`| int - total tokens |

在Python和TypeScript中，这些名称通过`openinference-semantic-conventions`包公开；在Go语言中，它们必须像上面的字符串一样手工输入。

**Python模式：**获取全局跟踪器（与Arize相同的提供商），然后使用上下文管理器，使工具跨度成为CHAIN跨度的子跨度，并出现在与LLM跨度相同的跟踪中：```python
from opentelemetry.trace import get_tracer

tracer = get_tracer("my-app", "1.0.0")

# In your agent entrypoint:
with tracer.start_as_current_span("run_agent") as chain_span:
    chain_span.set_attribute("openinference.span.kind", "CHAIN")
    chain_span.set_attribute("input.value", user_message)
    # ... LLM call ...
    for tool_use in tool_uses:
        with tracer.start_as_current_span(tool_use["name"]) as tool_span:
            tool_span.set_attribute("openinference.span.kind", "TOOL")
            tool_span.set_attribute("input.value", json.dumps(tool_use["input"]))
            result = run_tool(tool_use["name"], tool_use["input"])
            tool_span.set_attribute("output.value", result)
        # ... append tool result to messages, call LLM again ...
    chain_span.set_attribute("output.value", final_reply)
```
**Go模式：**从全局TracerProvider（通过`otel.SetTracerProvider`注册）获取跟踪器，然后使用`tracer.Start`嵌套跨度，因此工具跨度成为CHAIN跨度的子跨度。

b> **对于短寿命进程至关重要：**永远不要在跨度开始后调用`log.Fatalf`/`os.Exit`-它们跳过延迟的`tp.Shutdown(ctx)`，并且正在运行的CHAIN/LLM跨度永远不会刷新。使用`log.Printf`+`return`来代替`main`，并将`tp.Shutdown(ctx)`保留在`main`的顶部。```go
import (
    "context"
    "encoding/json"
    "go.opentelemetry.io/otel"
    "go.opentelemetry.io/otel/attribute"
)

var tracer = otel.Tracer("my-app")

func runAgent(ctx context.Context, userMessage string) string {
    ctx, chainSpan := tracer.Start(ctx, "run_agent")
    defer chainSpan.End()
    chainSpan.SetAttributes(
        attribute.String("openinference.span.kind", "CHAIN"),
        attribute.String("input.value", userMessage),
    )

    // ... LLM call ...
    for _, toolUse := range toolUses {
        ctx, toolSpan := tracer.Start(ctx, toolUse.Name)
        argsJSON, err := json.Marshal(toolUse.Input)
        if err != nil {
            toolSpan.RecordError(err)
        }
        toolSpan.SetAttributes(
            attribute.String("openinference.span.kind", "TOOL"),
            attribute.String("input.value", string(argsJSON)),
        )
        result := runTool(toolUse.Name, toolUse.Input)
        toolSpan.SetAttributes(attribute.String("output.value", result))
        toolSpan.End()
        // ... append tool result to messages, call LLM again ...
    }

    chainSpan.SetAttributes(attribute.String("output.value", finalReply))
    return finalReply
}
```
请参阅[Manual instrumentation]（https://arize.com/docs/ax/instrument/manual-instrumentation）了解更多的跨度类型和属性。

# #验证

只有在满足以下所有条件时，才将仪器仪表视为完整的：

1. 在跟踪更改之后，应用程序仍然会进行构建或类型检查。
2. 应用程序使用新的跟踪配置成功启动。
3. 您至少触发一个应该产生span的实际请求或运行。
4. 你要么在alize中验证结果跟踪，要么提供一个精确的拦截器来区分应用程序端成功和应用程序端失败。

后实现:1. 运行应用程序并触发至少一个LLM调用。
2. **使用`arize-trace`技能**确认到达的痕迹。如果为空，请稍后重试。验证范围有预期的`openinference.span.kind`、`input.value`/`output.value`和亲子关系。
3. 如果没有跟踪：验证`ARIZE_SPACE`和`ARIZE_API_KEY`，确保在仪器和客户机之前初始化跟踪程序，检查到`otlp.arize.com:443`的连接性，并检查app/runtime导出器日志，这样您就可以判断是否在本地发出了跨，但在远程拒绝了。对于调试，设置`GRPC_VERBOSITY=debug`或将`log_to_console=True`传递给`register()`。常见问题：(a)缺少项目名称资源属性导致HTTP 500被拒绝-仅`service.name`是不够的；Python：传递`project_name`给`register()`；在资源上设置`"model_id"`或`SEMRESATTRS_PROJECT_NAME`；执行：将`attribute.String("openinference.project.name", "my-app")`添加到`resource.New(...)`；(b)在OTLP导出flush之前，CLI/script进程退出——在退出之前先调用`provider.force_flush()`，然后调用`provider.shutdown()`（Python/TS）或`tp.Shutdown(ctx)`(Go)；(c) CLI-vspaces/projects可以不同意收集器目标的空间ID——报告不匹配，而不是静默重写凭证。
4. 如果应用程序使用工具：确认链和工具跨度出现与`input.value`/`output.value`，所以工具调用和结果是可见的。当验证被CLI或账户问题阻止时，以具体状态结束：

-应用程序检测状态
-最新的本地跟踪ID或运行ID
-出口商日志是否显示本地跨度发射
-故障是凭据、space/project解析、网络拒绝还是采集器拒绝

利用跟踪助手（MCP）

要在IDE中获得更深入的仪表指导，用户可以启用：

** ** ** ** ** ** ** ** ** ** ** ** ** ** ** ** ** ** ** **光标：**设置→MCP→添加**和使用：  ```json
  "arize-tracing-assistant": {
    "command": "uvx",
    "args": ["arize-tracing-assistant@latest"]
  }
  ```
- ** ** ** ** ** ** * -可搜索的文件。在游标:  ```json
  "arize-ax-docs": {
    "url": "https://arize.com/docs/mcp"
  }
  ```
然后用户可以问这样的问题：“使用Arize AX来测量这个应用程序”，“你能使用手动测量吗，这样我就能更好地控制我的轨迹？”*, *“我如何从我的数据库中编辑敏感信息？”*

请参阅[代理辅助跟踪设置]（https://arize.com/docs/ax/alyx/tracing-assistant）中的完整设置。

##参考链接

|资源| URL ||----------|-----|
|代理辅助跟踪设置|https://arize.com/docs/ax/alyx/tracing-assistant|
| Agent Setup Prompt (full routing + phases) |https://arize.com/docs/PROMPT.md|https://arize.com/docs/ax|
|完全集成列表|https://arize.com/docs/ax/integrations|
|文档索引（llms.txt） |https://arize.com/docs/llms.txt|

##保存凭据以备将来使用

参见references/ax-profiles.md§保存凭据以备将来使用。