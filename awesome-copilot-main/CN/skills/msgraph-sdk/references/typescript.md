# Microsoft Graph SDK for TypeScript / JavaScript

当目标项目使用TypeScript或JavaScript （Node.js或浏览器）时，请使用此引用。

##权威来源

—SDK存储库：<https://github.com/microsoftgraph/msgraph-sdk-javascript>—示例：<https://github.com/microsoftgraph/msgraph-training-typescript>—SDK变更日志：<https://github.com/microsoftgraph/msgraph-sdk-javascript/blob/main/CHANGELOG.md># #包```bash
npm install @microsoft/microsoft-graph-client @azure/identity
npm install -D @microsoft/microsoft-graph-types   # TypeScript type definitions
```
对于Node.js环境，也安装fetch polyfill：```bash
npm install node-fetch
```
##客户端设置

管理身份（azure托管的应用程序-首选）```typescript
import { Client } from "@microsoft/microsoft-graph-client";
import { TokenCredentialAuthenticationProvider } from "@microsoft/microsoft-graph-client/authProviders/azureTokenCredentials/index.js";
import { DefaultAzureCredential } from "@azure/identity";

const credential = new DefaultAzureCredential();
const authProvider = new TokenCredentialAuthenticationProvider(credential, {
  scopes: ["https://graph.microsoft.com/.default"],
});

const graphClient = Client.initWithMiddleware({ authProvider });
```
客户端凭证（app-only / daemon）```typescript
import { ClientSecretCredential } from "@azure/identity";

const credential = new ClientSecretCredential(
  process.env.AZURE_TENANT_ID!,
  process.env.AZURE_CLIENT_ID!,
  process.env.AZURE_CLIENT_SECRET!
);

const authProvider = new TokenCredentialAuthenticationProvider(credential, {
  scopes: ["https://graph.microsoft.com/.default"],
});

const graphClient = Client.initWithMiddleware({ authProvider });
```
On-Behalf-Of (OBO) -代理/ API作为登录用户```typescript
import { OnBehalfOfCredential } from "@azure/identity";

// incomingToken is the bearer token received from the caller (e.g. from req.headers.authorization)
const credential = new OnBehalfOfCredential({
  tenantId: process.env.AZURE_TENANT_ID!,
  clientId: process.env.AZURE_CLIENT_ID!,
  clientSecret: process.env.AZURE_CLIENT_SECRET!,
  userAssertionToken: incomingToken,
});

const authProvider = new TokenCredentialAuthenticationProvider(credential, {
  scopes: ["https://graph.microsoft.com/.default"],
});

const graphClient = Client.initWithMiddleware({ authProvider });
```
对于OBO，为每个请求创建一个新客户端（凭据是用户范围的，不是单例安全的）。

交互式（本地开发/ CLI -Node.js）

当有浏览器时，使用`InteractiveBrowserCredential`。将`DeviceCodeCredential`用于无头环境（SSH、ci -邻域、WSL）：```typescript
import { InteractiveBrowserCredential, DeviceCodeCredential } from "@azure/identity";

// Opens a browser tab — requires redirect URI http://localhost in app registration
const credential = new InteractiveBrowserCredential({
  tenantId: process.env.AZURE_TENANT_ID!,
  clientId: process.env.AZURE_CLIENT_ID!,
});

// Prints a device code to the terminal — works in any environment
const credential = new DeviceCodeCredential({
  tenantId: process.env.AZURE_TENANT_ID!,
  clientId: process.env.AZURE_CLIENT_ID!,
  userPromptCallback: (info) => console.log(info.message),
});
```
两者都要求应用注册平台为“移动和桌面应用”**。两者都不使用客户端秘密。

##常见呼叫模式

获取带有字段选择的资源```typescript
import { User } from "@microsoft/microsoft-graph-types";

const user: User = await graphClient
  .api("/me")
  .select("displayName,mail,jobTitle")
  .get();
```
带有筛选、选择和排序的列表```typescript
const result = await graphClient
  .api("/me/messages")
  .filter("isRead eq false")
  .select("subject,from,receivedDateTime")
  .top(25)
  .orderby("receivedDateTime desc")
  .get();
```
###分页与PageIterator```typescript
import { PageIterator } from "@microsoft/microsoft-graph-client";
import { Message } from "@microsoft/microsoft-graph-types";

const firstPage = await graphClient.api("/me/messages").top(25).get();

const allMessages: Message[] = [];

const pageIterator = new PageIterator(
  graphClient,
  firstPage,
  (message: Message) => {
    allMessages.push(message);
    return true; // return false to stop early
  }
);

await pageIterator.iterate();
```
###发送邮件```typescript
await graphClient.api("/me/sendMail").post({
  message: {
    subject: "Hello from Graph",
    body: { contentType: "Text", content: "Test message" },
    toRecipients: [{ emailAddress: { address: "user@contoso.com" } }],
  },
});
```
发布一个Teams频道消息```typescript
await graphClient.api(`/teams/${teamId}/channels/${channelId}/messages`).post({
  body: { contentType: "html", content: "<b>Hello from Graph!</b>" },
});
```
上传文件到OneDrive（小文件≤4mb）```typescript
const content = Buffer.from("file contents");
await graphClient
  .api(`/me/drive/root:/${fileName}:/content`)
  .putStream(content);
```
对于> 4 MB的文件，使用一个上传会话（`createUploadSession`）。

批处理请求```typescript
const batchRequestBody = {
  requests: [
    { id: "1", method: "GET", url: "/me" },
    { id: "2", method: "GET", url: "/me/messages?$top=5&$select=subject" },
  ],
};

const batchResponse = await graphClient.api("/$batch").post(batchRequestBody);

const meResponse = batchResponse.responses.find((r: any) => r.id === "1");
const messagesResponse = batchResponse.responses.find((r: any) => r.id === "2");
```
##增量查询```typescript
// First sync
let response = await graphClient.api("/users/delta").get();
const users: any[] = [];

while (response["@odata.nextLink"]) {
  users.push(...response.value);
  response = await graphClient.api(response["@odata.nextLink"]).get();
}
users.push(...response.value);

const deltaLink: string = response["@odata.deltaLink"];
// Store deltaLink durably for next sync run

// Next sync — only changes
const changesResponse = await graphClient.api(deltaLink).get();
```
节流/重试中间件

SDK默认包含重试中间件。对于显式配置：```typescript
import {
  Client,
  RetryHandlerOptions,
  RetryHandler,
  MiddlewareFactory,
} from "@microsoft/microsoft-graph-client";

const retryOptions = new RetryHandlerOptions({ maxRetries: 5 });
const middleware = MiddlewareFactory.getDefaultMiddlewareChain(authProvider);

const graphClient = Client.initWithMiddleware({ middleware });
```
始终遵守`Retry-After`报头值-当Graph指定等待时间时不要使用固定的backoff。

## typescript特有的指导

-从`@microsoft/microsoft-graph-types`导入类型，以实现对图形资源的完全智能感知。
-`.api()`链返回从`@microsoft/microsoft-graph-types`转换为适当类型的`any`。
-对于ESM项目，在深度导入时使用`/index.js`路径后缀（例如，`azureTokenCredentials/index.js`）。
-使用`async`/`await`一致-所有的图形调用返回承诺。
-在应用级代码中单例化`graphClient`（例如Express app init）；对于OBO流，按请求构造。
-在Node.js18+中，`fetch`是本地可用的-不需要填充。```typescript
// Type-safe response example
import { MessageCollectionResponse } from "@microsoft/microsoft-graph-types";

const response: MessageCollectionResponse = await graphClient
  .api("/me/messages")
  .select("subject,from")
  .get();

const messages = response.value ?? [];
```
