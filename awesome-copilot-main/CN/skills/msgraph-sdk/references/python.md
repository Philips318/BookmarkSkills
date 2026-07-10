# Microsoft Graph SDK for Python

当目标项目是用Python编写时，请使用此引用。

##权威来源

—SDK存储库：<https://github.com/microsoftgraph/msgraph-sdk-python>—示例：<https://github.com/microsoftgraph/msgraph-training-python>—SDK变更日志：<https://github.com/microsoftgraph/msgraph-sdk-python/blob/main/CHANGELOG.md># #包```bash
pip install msgraph-sdk azure-identity
```
或者在`requirements.txt`/`pyproject.toml`中：```
msgraph-sdk>=1.0.0
azure-identity>=1.15.0
```
##客户端设置

管理身份（azure托管的应用程序-首选）```python
from azure.identity.aio import DefaultAzureCredential
from msgraph import GraphServiceClient

credential = DefaultAzureCredential()
graph_client = GraphServiceClient(credential)
```
Python SDK是async-first （`asyncio`）。使用`azure.identity.aio`（异步变体），而不是`azure.identity`。

客户端凭证（app-only / daemon）```python
import os
from azure.identity.aio import ClientSecretCredential
from msgraph import GraphServiceClient

credential = ClientSecretCredential(
    tenant_id=os.environ["AZURE_TENANT_ID"],
    client_id=os.environ["AZURE_CLIENT_ID"],
    client_secret=os.environ["AZURE_CLIENT_SECRET"],
)

graph_client = GraphServiceClient(credential)
```
在生产环境中，首选`CertificateCredential`而不是`ClientSecretCredential`。

On-Behalf-Of (OBO) -代理/ API作为登录用户```python
from azure.identity.aio import OnBehalfOfCredential

# incoming_token is the bearer token from the caller
credential = OnBehalfOfCredential(
    tenant_id=os.environ["AZURE_TENANT_ID"],
    client_id=os.environ["AZURE_CLIENT_ID"],
    client_secret=os.environ["AZURE_CLIENT_SECRET"],
    user_assertion=incoming_token,
)

graph_client = GraphServiceClient(credential)
```
为OBO的每个请求构造一个新的`GraphServiceClient`——凭据是用户范围的。

###设备代码（CLI / local dev）```python
from azure.identity.aio import DeviceCodeCredential

credential = DeviceCodeCredential(
    client_id=os.environ["AZURE_CLIENT_ID"],
    tenant_id=os.environ["AZURE_TENANT_ID"],
)
graph_client = GraphServiceClient(credential, scopes=["User.Read", "Mail.Read"])
```
##常见呼叫模式

Python中所有的Graph SDK调用都是异步的。始终在异步上下文中运行。

获取带有字段选择的资源```python
import asyncio
from msgraph.generated.me.me_request_builder import MeRequestBuilder
from kiota_abstractions.base_request_configuration import RequestConfiguration

async def get_my_profile():
    query_params = MeRequestBuilder.MeRequestBuilderGetQueryParameters(
        select=["displayName", "mail", "jobTitle"]
    )
    config = RequestConfiguration(query_parameters=query_params)
    user = await graph_client.me.get(request_configuration=config)
    return user

asyncio.run(get_my_profile())
```
###使用过滤器和选择列表消息```python
from msgraph.generated.me.messages.messages_request_builder import MessagesRequestBuilder

async def get_unread_messages():
    query_params = MessagesRequestBuilder.MessagesRequestBuilderGetQueryParameters(
        filter="isRead eq false",
        select=["subject", "from", "receivedDateTime"],
        top=25,
        orderby=["receivedDateTime desc"],
    )
    config = RequestConfiguration(query_parameters=query_params)
    result = await graph_client.me.messages.get(request_configuration=config)
    return result
```
###分页与PageIterator```python
from msgraph.generated.models.message import Message
from msgraph.core import PageIterator

async def get_all_messages():
    first_page = await graph_client.me.messages.get()
    all_messages: list[Message] = []

    async def process_message(message: Message) -> bool:
        all_messages.append(message)
        return True  # return False to stop early

    page_iterator = PageIterator(
        response=first_page,
        request_adapter=graph_client.request_adapter,
        constructor=Message,
    )
    await page_iterator.iterate(callback=process_message)
    return all_messages
```
###发送邮件```python
from msgraph.generated.models.message import Message
from msgraph.generated.models.item_body import ItemBody
from msgraph.generated.models.body_type import BodyType
from msgraph.generated.models.recipient import Recipient
from msgraph.generated.models.email_address import EmailAddress
from msgraph.generated.me.send_mail.send_mail_post_request_body import SendMailPostRequestBody

async def send_email():
    body = SendMailPostRequestBody(
        message=Message(
            subject="Hello from Graph",
            body=ItemBody(content_type=BodyType.Text, content="Test message"),
            to_recipients=[
                Recipient(email_address=EmailAddress(address="user@contoso.com"))
            ],
        )
    )
    await graph_client.me.send_mail.post(body)
```
发布一个Teams频道消息```python
from msgraph.generated.models.chat_message import ChatMessage
from msgraph.generated.models.item_body import ItemBody
from msgraph.generated.models.body_type import BodyType

async def post_channel_message(team_id: str, channel_id: str):
    message = ChatMessage(
        body=ItemBody(content_type=BodyType.Html, content="<b>Hello from Graph!</b>")
    )
    await graph_client.teams.by_team_id(team_id).channels.by_channel_id(channel_id).messages.post(message)
```
批处理请求```python
from kiota_http.middleware.options import ResponseHandlerOption
import json

async def batch_example():
    batch_body = {
        "requests": [
            {"id": "1", "method": "GET", "url": "/me"},
            {"id": "2", "method": "GET", "url": "/me/messages?$top=5&$select=subject"},
        ]
    }
    # Use the raw HTTP client for batch
    response = await graph_client.request_adapter.send_primitive_async(
        # Alternatively, use the requests library with a token from the credential
    )
```
对于Python中的批处理，当批处理助手尚未完全支持时，使用`httpx`和获得的令牌通常更简单：```python
import httpx
from azure.identity.aio import ClientSecretCredential

async def batch_with_httpx(credential):
    token = await credential.get_token("https://graph.microsoft.com/.default")
    async with httpx.AsyncClient() as client:
        response = await client.post(
            "https://graph.microsoft.com/v1.0/$batch",
            headers={"Authorization": f"Bearer {token.token}"},
            json={
                "requests": [
                    {"id": "1", "method": "GET", "url": "/me"},
                    {"id": "2", "method": "GET", "url": "/me/messages?$top=5"},
                ]
            },
        )
    return response.json()
```
##增量查询```python
async def delta_sync(stored_delta_link: str | None = None):
    if stored_delta_link:
        # Use delta link directly
        response = await graph_client.request_adapter.send_async(...)
    else:
        response = await graph_client.users.delta.get()

    users = []
    async def collect(user):
        users.append(user)
        return True

    page_iterator = PageIterator(response=response, request_adapter=graph_client.request_adapter, constructor=...)
    await page_iterator.iterate(callback=collect)

    delta_link = page_iterator.delta_link  # store this for next run
    return users, delta_link
```
##节流/重试

SDK的HTTP传输在使用默认的`GraphClientFactory`时自动处理429次重试。对于显式控制：```python
import asyncio
import httpx

async def call_with_retry(graph_client, call_fn, max_retries=5):
    for attempt in range(max_retries):
        try:
            return await call_fn()
        except Exception as e:
            if "429" in str(e):
                retry_after = int(getattr(e, "retry_after", 10))
                await asyncio.sleep(retry_after)
            else:
                raise
```
特定于python的指导

- Python Graph SDK是**async-first** -使用`asyncio.run()`或async框架（FastAPI, aiohttp）。
-对于异步上下文总是使用`azure.identity.aio`（而不是`azure.identity`）。
-关闭凭证完成时：`await credential.close()`或使用作为异步上下文管理器。
Python SDK模型类使用`snake_case`作为属性（Graph JSON使用`camelCase`- SDK自动映射）。
-使用`asyncio.gather()`并发但独立的图形调用（思想节流限制）。
-对于FastAPI：使用寿命事件初始化`GraphServiceClient`一次，并在关机时关闭凭据。```python
# FastAPI integration example
from contextlib import asynccontextmanager
from fastapi import FastAPI

@asynccontextmanager
async def lifespan(app: FastAPI):
    credential = DefaultAzureCredential()
    app.state.graph_client = GraphServiceClient(credential)
    yield
    await credential.close()

app = FastAPI(lifespan=lifespan)
```
