---
name: qdrant-clients-sdk
description: "Qdrant provides client SDKs for various programming languages, allowing easy integration with Qdrant deployments."
allowed-tools:
  - Read
  - Grep
  - Glob
  - Bash
---
# qrant客户端SDK

Qdrant有以下官方支持的客户端sdk：

- Python - [qdrant-client]（https://github.com/qdrant/qdrant-client）·安装方式：`pip install qdrant-client[fastembed]`JavaScript / TypeScript - [qdrant-js]（https://github.com/qdrant/qdrant-js）·安装：`npm install @qdrant/js-client-rest`- Rust - [Rust -client]（https://github.com/qdrant/rust-client）·安装：`cargo add qdrant-client`- Go - [Go -client]（https://github.com/qdrant/go-client）·安装：`go get github.com/qdrant/go-client`-。·安装：`dotnet add package Qdrant.Client`Java - [Java -client]（https://github.com/qdrant/java-client）·可在Maven Central上使用：https://central.sonatype.com/artifact/io.qdrant/client## API参考

与Qdrant的所有交互都可以通过REST API或gRPC API进行。如果您是第一次使用Qdrant或者正在制作原型，我们建议您使用REST API。

* REST API - [OpenAPI Reference](https://api.qdrant.tech/api-reference) - [GitHub]（https://github.com/qdrant/qdrant/blob/master/docs/redoc/master/openapi.json）
* gRPC API - [gRPC protobuf定义]（https://github.com/qdrant/qdrant/tree/master/lib/api/src/grpc/proto）

##代码示例要获取特定客户端和用例的代码示例，您可以向Qdrant客户端精心策划的代码片段库发送搜索请求。```bash
curl -X GET "https://snippets.qdrant.tech/search?language=python&query=how+to+upload+points"
```
支持语言：`python`、`typescript`、`rust`、`java`、`go`、`csharp`反应的例子:```markdown

## Snippet 1

*qdrant-client* (vlatest) — https://search.qdrant.tech/md/documentation/manage-data/points/

Uploads multiple vector-embedded points to a Qdrant collection using the Python qdrant_client (PointStruct) with id, payload (e.g., color), and a 3D-like vector for similarity search. It supports parallel uploads (parallel=4) and a retry policy (max_retries=3) for robust indexing. The operation is idempotent: re-uploading with the same id overwrites existing points; if ids aren’t provided, Qdrant auto-generates UUIDs.

client.upload_points(
    collection_name="{collection_name}",
    points=[
        models.PointStruct(
            id=1,
            payload={
                "color": "red",
            },
            vector=[0.9, 0.1, 0.1],
        ),
        models.PointStruct(
            id=2,
            payload={
                "color": "green",
            },
            vector=[0.1, 0.9, 0.1],
        ),
    ],
    parallel=4,
    max_retries=3,
)
```
默认响应格式为markdown，如果代码段输出需要JSON格式，则可以将`&format=json`添加到查询字符串中。