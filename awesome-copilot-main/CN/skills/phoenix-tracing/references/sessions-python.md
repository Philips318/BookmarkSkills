# Sessions （Python）

通过使用会话id分组跟踪来跟踪多回合会话。

# #设置```python
from phoenix.otel import using_session

with using_session(session_id="user_123_conv_456"):
    response = llm.invoke(prompt)
```
最佳实践

**坏：只有父span获得会话ID**```python
from phoenix.otel import SpanAttributes
from opentelemetry import trace

span = trace.get_current_span()
span.set_attribute(SpanAttributes.SESSION_ID, session_id)
response = client.chat.completions.create(...)
```
**好：所有子跨度继承会话ID**```python
with using_session(session_id):
    response = client.chat.completions.create(...)
    result = my_custom_function()
```
**原因：**`using_session()`自动传播会话ID到所有嵌套的跨度。

会话ID模式```python
import uuid

session_id = str(uuid.uuid4())
session_id = f"user_{user_id}_conv_{conversation_id}"
session_id = f"debug_{timestamp}"
```
好：`str(uuid.uuid4())`，`"user_123_conv_456"`坏：`"session_1"`，`"test"`，空字符串

多回合聊天机器人示例```python
import uuid
from phoenix.otel import using_session

session_id = str(uuid.uuid4())
messages = []

def send_message(user_input: str) -> str:
    messages.append({"role": "user", "content": user_input})

    with using_session(session_id):
        response = client.chat.completions.create(
            model="gpt-4",
            messages=messages
        )

    assistant_message = response.choices[0].message.content
    messages.append({"role": "assistant", "content": assistant_message})
    return assistant_message
```
##附加属性```python
from phoenix.otel import using_attributes

with using_attributes(
    user_id="user_123",
    session_id="conv_456",
    metadata={"tier": "premium", "region": "us-west"}
):
    response = llm.invoke(prompt)
```
LangChain集成

LangChain线程被自动识别为会话：```python
from langchain.chat_models import ChatOpenAI

response = llm.invoke(
    [HumanMessage(content="Hi!")],
    config={"metadata": {"thread_id": "user_123_thread"}}
)
```
Phoenix可以识别：`thread_id`，`session_id`,`conversation_id`##参见Also

**TypeScript会话：**`sessions-typescript.md`- **会话文档：**https://docs.arize.com/phoenix/tracing/sessions