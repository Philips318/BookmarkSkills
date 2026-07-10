---
name: arize-ai-provider-integration
description: Creates, reads, updates, and deletes Arize AI integrations that store LLM provider credentials used by evaluators and other Arize features. Supports any LLM provider (e.g. OpenAI, Anthropic, Azure OpenAI, AWS Bedrock, Vertex AI, Gemini, NVIDIA NIM). Use when the user mentions AI integration, LLM provider credentials, create integration, list integrations, update credentials, delete integration, or connecting an LLM provider to Arize.
metadata:
  author: arize
  version: "1.0"
compatibility: Requires the ax CLI and a configured Arize profile.
---
#掌握AI集成技能

> **`SPACE`** -大多数`--space`标志和`ARIZE_SPACE`env变量接受一个空间**名称**（例如，`my-workspace`）或一个base64空间**ID**（例如，`U3BhY2U6...`）。用`ax spaces list`找到你的。
b> **注：**`ai-integrations create`** *不**接受`--space`- AI集成是账户范围的。`--space`只能与`list`、`get`、`update`和`delete`一起使用。

# #的概念- **AI Integration** =存储在Arize中注册的LLM提供商凭据；由评估人员调用判断模型和其他需要代表您调用LLM的alize功能使用
**提供商** =支持集成的LLM服务（例如，`openAI`,`anthropic`,`awsBedrock`）
- **集成ID** =一个base64编码的集成全局标识符（例如，`TGxtSW50ZWdyYXRpb246MTI6YUJjRA==`）；评估器创建和其他下游操作所需
- **作用域** =可视性规则，控制哪些空间或用户可以使用集成
- **认证类型** =如何与提供者进行身份验证：`default`（提供者API密钥），`proxy_with_headers`（通过自定义报头代理），或`bearer_token`（承载令牌认证）

# #先决条件

直接执行任务—运行所需的`ax`命令。不要预先检查版本、环境变量或配置文件。如果执行命令`ax`失败，请根据错误提示进行处理：
-`command not found`或版本错误→参见references/ax-setup.md-`401 Unauthorized`/缺少API密钥→运行`ax profiles show`检查当前配置文件。如果配置文件丢失或API密钥错误，请按照references/ax-profiles.md到create/update。如果用户没有他们的密钥，将他们引导到https://app.arize.com/admin> API Keys
-空间未知→运行`ax spaces list`按名称选择，或询问用户
LLM提供程序调用失败（缺少OPENAI_API_KEY / ANTHROPIC_API_KEY）→运行`ax ai-integrations list --space SPACE`检查平台管理的凭据。如果不存在，请要求用户提供密钥或通过alize -ai-provider-integration技能创建集成
**安全：**永远不要读取`.env`文件或搜索文件系统的凭据。使用`ax profiles`作为alize凭证，使用`ax ai-integrations`作为LLM提供程序密钥。如果凭据无法通过这些渠道获得，请询问用户。

---##列出AI集成

列出一个空间中可访问的所有集成：```bash
ax ai-integrations list --space SPACE
```
按名称过滤（不区分大小写的子字符串匹配）：```bash
ax ai-integrations list --space SPACE --name "openai"
```
分页大型结果集：```bash
# Get first page
ax ai-integrations list --space SPACE --limit 20 -o json

# Get next page using cursor from previous response
ax ai-integrations list --space SPACE --limit 20 --cursor CURSOR_TOKEN -o json
```
* *关键标志:* *

|标志位|描述||------|-------------|
|`--space`|集成过滤的空间名称或ID |
|`--name`|集成名称|不区分大小写的子字符串过滤器
|`--limit`|最大结果（1-100，默认15）|
|`--cursor`|来自前一个响应|的分页令牌
|`-o, --output`|输出格式：默认为`table`或`json`|

* *响应领域:* *

|字段|描述||-------|-------------|
|`id`| Base64集成ID -复制到下游命令|
|`name`|人类可读名称|
|`provider`| LLM provider enum（请参见下文支持的provider） |
|`has_api_key`|`true`如果凭证存储|
|`model_names`|允许的型号列表，如果所有型号都启用，则为`null`|
|`enable_default_models`|是否允许此提供程序的默认模型|
|`function_calling_enabled`|是否开启tool/function呼叫|
|`auth_type`|鉴权方式：`default`、`proxy_with_headers`、`bearer_token`|

---

获得特定的集成```bash
ax ai-integrations get NAME_OR_ID
ax ai-integrations get NAME_OR_ID -o json
ax ai-integrations get NAME_OR_ID --space SPACE   # required when using name instead of ID
```
使用它来检查集成的完整配置或在创建后确认其ID。

---

创建AI集成

在创建之前，总是先列出集成-用户可能已经有一个合适的集成：```bash
ax ai-integrations list --space SPACE
```
如果不存在合适的集成，则创建一个。所需的标志取决于提供程序。

# # # OpenAI```bash
ax ai-integrations create \
  --name "My OpenAI Integration" \
  --provider openAI \
  --api-key $OPENAI_API_KEY
```
# # #人择```bash
ax ai-integrations create \
  --name "My Anthropic Integration" \
  --provider anthropic \
  --api-key $ANTHROPIC_API_KEY
```
Azure OpenAI```bash
ax ai-integrations create \
  --name "My Azure OpenAI Integration" \
  --provider azureOpenAI \
  --api-key $AZURE_OPENAI_API_KEY \
  --base-url "https://my-resource.openai.azure.com/"
```
AWS Bedrock

AWS Bedrock使用基于角色的IAM认证。通过`--provider-metadata`提供角色的ARN：```bash
ax ai-integrations create \
  --name "My Bedrock Integration" \
  --provider awsBedrock \
  --provider-metadata '{"role_arn": "arn:aws:iam::123456789012:role/ArizeBedrockRole"}'
```
顶点AI

Vertex AI使用GCP服务帐户凭据。通过`--provider-metadata`提供GCP项目和区域：```bash
ax ai-integrations create \
  --name "My Vertex AI Integration" \
  --provider vertexAI \
  --provider-metadata '{"project_id": "my-gcp-project", "location": "us-central1"}'
```
# # #双子座```bash
ax ai-integrations create \
  --name "My Gemini Integration" \
  --provider gemini \
  --api-key $GEMINI_API_KEY
```
nvidia nim```bash
ax ai-integrations create \
  --name "My NVIDIA NIM Integration" \
  --provider nvidiaNim \
  --api-key $NVIDIA_API_KEY \
  --base-url "https://integrate.api.nvidia.com/v1"
```
自定义（openai兼容端点）```bash
ax ai-integrations create \
  --name "My Custom Integration" \
  --provider custom \
  --base-url "https://my-llm-proxy.example.com/v1" \
  --api-key $CUSTOM_LLM_API_KEY
```
支持的提供者

|提供程序|需要额外的标志||----------|---------------------|
|`openAI`|`--api-key <key>`|
|`anthropic`|`--api-key <key>`|
|`azureOpenAI`|`--api-key <key>`,`--base-url <azure-endpoint>`|
|`awsBedrock`|`--provider-metadata '{"role_arn": "<arn>"}'`|
|`vertexAI`|`--provider-metadata '{"project_id": "<gcp-project>", "location": "<region>"}'`|
|`gemini`|`--api-key <key>`|
|`nvidiaNim`|`--api-key <key>`,`--base-url <nim-endpoint>`|
|`custom`|`--base-url <endpoint>`|

任何提供程序的可选标志

|标志位|描述||------|-------------|
|`--model-name`|允许的模型名称（重复多个，例如`--model-name gpt-4o --model-name gpt-4o-mini`）；省略以允许所有模型|
|`--enable-default-models`|启用提供程序的默认模型列表|
|`--function-calling-enabled`|开启tool/function呼叫支持|
|`--auth-type`|鉴权类型：`default`、`proxy_with_headers`、`bearer_token`|
|`--headers`|自定义头作为JSON对象或文件路径（用于代理认证）|
|`--provider-metadata`|提供程序特定的元数据作为JSON对象或文件路径|

###创建后

捕获返回的集成ID（例如，`TGxtSW50ZWdyYXRpb246MTI6YUJjRA==`）—创建求值器和其他下游命令需要它。如果你错过了它，找回它；```bash
ax ai-integrations list --space SPACE -o json
# or by name/ID directly:
ax ai-integrations get NAME_OR_ID
```
---

更新AI集成`update`是部分更新—仅更改您提供的标志。省略的字段保持原样。```bash
# Rename
ax ai-integrations update NAME_OR_ID --name "New Name"

# Rotate the API key
ax ai-integrations update NAME_OR_ID --api-key $OPENAI_API_KEY

# Change the model list (replaces all existing model names)
ax ai-integrations update NAME_OR_ID --model-name gpt-4o --model-name gpt-4o-mini

# Update base URL (for Azure, custom, or NIM)
ax ai-integrations update NAME_OR_ID --base-url "https://new-endpoint.example.com/v1"
```
当使用名称而不是ID时，添加`--space SPACE`。`create`接受的任何标志都可以传递给`update`。

---

##删除AI集成

**警告：**删除是永久性的。引用此集成的求值器将不再能够运行。```bash
ax ai-integrations delete NAME_OR_ID --force
ax ai-integrations delete NAME_OR_ID --space SPACE --force   # required when using name instead of ID
```
省略`--force`以获得确认提示，而不是立即删除。

---

# #故障排除

|解决方案||---------|----------|
|`ax: command not found`|参见references/ax-setup.md|
|`401 Unauthorized`| API密钥可能无法访问此空间。在https://app.arize.com/admin> API Keys |中验证密钥和空间ID
|运行`ax profiles show --expand`；设置`ARIZE_API_KEY`env var或写入`~/.arize/config.toml`|
|`Integration not found`|用`ax ai-integrations list --space SPACE`|验证
创建|后没有保存凭据-用正确的`--api-key`或`--provider-metadata`|重新运行`update`|使用`ax ai-integrations get INT_ID`检查集成凭据；如果需要，请旋转API密钥|
|`provider`mismatch |创建后不能更改提供程序-删除并使用正确的提供程序|重新创建

---

相关技能

- ** alize -evaluator**：创建使用AI集成的LLM-as-judge evaluator→使用`arize-evaluator`- ** ize-experiment**：使用AI集成支持的评估器运行实验→使用`arize-experiment`---

##保存凭据以备将来使用

参见references/ax-profiles.md§保存凭据以备将来使用。