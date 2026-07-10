---
name: microsoft-code-reference
description: Look up Microsoft API references, find working code samples, and verify SDK code is correct. Use when working with Azure SDKs, .NET libraries, or Microsoft APIs—to find the right method, check parameters, get working examples, or troubleshoot errors. Catches hallucinated methods, wrong signatures, and deprecated patterns by querying official docs.
compatibility: Works best with Microsoft Learn MCP Server (https://learn.microsoft.com/api/mcp). Can also use the mslearn CLI as a fallback.
---
#微软代码参考

# #工具

|需要|工具|示例||------|------|---------|
APImethod/class查找|`microsoft_docs_search`|`"BlobClient UploadAsync Azure.Storage.Blobs"`|
工作代码样例|`microsoft_code_sample_search`|`query: "upload blob managed identity", language: "python"`|
|完整API引用|`microsoft_docs_fetch`|从`microsoft_docs_search`获取URL（用于过载，完整签名）|

查找代码示例

使用`microsoft_code_sample_search`获取官方的工作示例：```
microsoft_code_sample_search(query: "upload file to blob storage", language: "csharp")
microsoft_code_sample_search(query: "authenticate with managed identity", language: "python")
microsoft_code_sample_search(query: "send message service bus", language: "javascript")
```
**何时使用：**
在写代码之前，找到一个工作模式
-在错误之后-将你的代码与已知的良好样本进行比较
-不确定initialization/setup-样品显示完整的上下文

## API查找```
# Verify method exists (include namespace for precision)
"BlobClient UploadAsync Azure.Storage.Blobs"
"GraphServiceClient Users Microsoft.Graph"

# Find class/interface
"DefaultAzureCredential class Azure.Identity"

# Find correct package
"Azure Blob Storage NuGet package"
"azure-storage-blob pip package"
```
当方法有多个重载或需要完整的参数详细信息时获取整页。

##错误排除

使用`microsoft_code_sample_search`查找工作代码示例，并与您的实现进行比较。对于特定的错误，使用`microsoft_docs_search`和`microsoft_docs_fetch`：

|错误类型|查询||------------|-------|
|方法未找到|`"[ClassName] methods [Namespace]"`|
|类型未找到|`"[TypeName] NuGet package namespace"`|
|签名错误|`"[ClassName] [MethodName] overloads"`→取整页|
|已弃用警告|`"[OldType] migration v12"`|
|认证失败|`"DefaultAzureCredential troubleshooting"`|
| 403禁止|`"[ServiceName] RBAC permissions"`|

##何时验证

请务必在以下情况下进行验证：
-方法名称似乎“太方便”（`UploadFile`vs实际的`Upload`）
-混合SDK版本（v11`CloudBlobClient`vs v12`BlobServiceClient`）
-包名不遵循约定(`Azure.*`for。. NET,`azure-*`for Python)
—首次使用API

##验证工作流

在使用微软sdk生成代码之前，请验证它是正确的：

1. **确认方法或包存在** -`microsoft_docs_search(query: "[ClassName] [MethodName] [Namespace]")`2. **获取完整的细节**（为overloads/complex参数）-`microsoft_docs_fetch(url: "...")`3. **查找工作样例** -`microsoft_code_sample_search(query: "[task]", language: "[lang]")`对于简单的查找，仅第1步就足够了。对于复杂的API使用，请完成所有三个步骤。

## CLI替代如果没有可用的Learn MCP服务器，可以在终端或shell（例如Bash、PowerShell或cmd）中使用`mslearn`命令行：```sh
# Run directly (no install needed)
npx @microsoft/learn-cli search "BlobClient UploadAsync Azure.Storage.Blobs"

# Or install globally, then run
npm install -g @microsoft/learn-cli
mslearn search "BlobClient UploadAsync Azure.Storage.Blobs"
```
| MCP Tool | CLI命令||----------|-------------|
|`microsoft_docs_search(query: "...")`|`mslearn search "..."`|
|`microsoft_code_sample_search(query: "...", language: "...")`|`mslearn code-search "..." --language ...`|
|`microsoft_docs_fetch(url: "...")`|`mslearn fetch "..."`|

将`--json`传递给`search`或`code-search`以获得用于进一步处理的原始JSON输出。