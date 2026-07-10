# Phoenix Tracing：自定义元数据（Python）

为跨度添加自定义属性以获得更丰富的可观察性。

# #安装```bash
pip install arize-phoenix-otel  # context managers and SpanAttributes re-exported since 0.16.0
```
# #会话```python
from phoenix.otel import using_session

with using_session(session_id="my-session-id"):
    # Spans get: "session.id" = "my-session-id"
    ...
```
# #用户```python
from phoenix.otel import using_user

with using_user("my-user-id"):
    # Spans get: "user.id" = "my-user-id"
    ...
```
# #元数据```python
from phoenix.otel import using_metadata

with using_metadata({"key": "value", "experiment_id": "exp_123"}):
    # Spans get: "metadata" = '{"key": "value", "experiment_id": "exp_123"}'
    ...
```
# #标签```python
from phoenix.otel import using_tags

with using_tags(["tag_1", "tag_2"]):
    # Spans get: "tag.tags" = '["tag_1", "tag_2"]'
    ...
```
##组合（using_attributes）```python
from phoenix.otel import using_attributes

with using_attributes(
    session_id="my-session-id",
    user_id="my-user-id",
    metadata={"environment": "production"},
    tags=["prod", "v2"],
    prompt_template="Answer: {question}",
    prompt_template_version="v1.0",
    prompt_template_variables={"question": "What is Phoenix?"},
):
    # All attributes applied to spans in this context
    ...
```
##在单个Span上```python
span.set_attribute("metadata", json.dumps({"key": "value"}))
span.set_attribute("user.id", "user_123")
span.set_attribute("session.id", "session_456")
```
作为装饰师

所有的上下文管理器都可以用作装饰器：```python
from phoenix.otel import using_session, using_user, using_metadata

@using_session(session_id="my-session-id")
@using_user("my-user-id")
@using_metadata({"env": "prod"})
def my_function():
    ...
```
