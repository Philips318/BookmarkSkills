#技能模板

针对不同类型的Microsoft技术的现成模板。

MCP工具的替代方案

下面的所有模板都使用MCP工具调用（例如，`microsoft_docs_search`,`microsoft_docs_fetch`,`microsoft_code_sample_search`）。如果无法使用Learn MCP服务器，请将其替换为等效的CLI：

| MCP Tool | CLI命令||----------|-------------|
|`microsoft_docs_search(query: "...")`|`mslearn search "..."`|
|`microsoft_code_sample_search(query: "...", language: "...")`|`mslearn code-search "..." --language ...`|
|`microsoft_docs_fetch(url: "...")`|`mslearn fetch "..."`|

直接使用`npx @microsoft/learn-cli <command>`运行或使用`npm install -g @microsoft/learn-cli`全局安装。

模板1:SDK/Library技能

用于客户端库、sdk和编程框架。```markdown
---
name: {sdk-name}
description: {What it does}. Use when agents need to {primary task} with {technology context}. Supports {languages/platforms}.
---

# {SDK Name}

{One paragraph: what it is, why it exists, when to use it}

## Installation

{Package manager commands for supported languages}

## Key Concepts

{3-5 essential concepts, one paragraph each max}

### {Concept 1}
{Brief explanation}

### {Concept 2}
{Brief explanation}

## Quick Start

{Minimal working example - inline if <30 lines, otherwise reference sample_codes/}

## Common Patterns

### {Pattern 1: e.g., "Basic CRUD"}
```{language}
{代码}```

### {Pattern 2: e.g., "Error Handling"}
```{language}
{代码}```

## API Quick Reference

| Class/Method | Purpose | Example |
|--------------|---------|---------|
| {name} | {what it does} | `{usage}` |

For full API documentation:
- `microsoft_docs_search(query="{sdk} {class} API reference")`
- `microsoft_docs_fetch(url="{url}")`

## Best Practices

- **Do**: {recommendation}
- **Do**: {recommendation}
- **Avoid**: {anti-pattern}

See [best-practices.md](references/best-practices.md) for detailed guidance.

## Learn More

| Topic | How to Find |
|-------|-------------|
| {Advanced topic 1} | `microsoft_docs_search(query="{sdk} {topic}")` |
| {Advanced topic 2} | `microsoft_docs_fetch(url="{url}")` |
| {Code examples} | `microsoft_code_sample_search(query="{sdk} {scenario}", language="{lang}")` |
```
---

模板2:Azure服务技能

用于Azure服务和云资源。```markdown
---
name: {service-name}
description: Work with {Azure Service}. Use when agents need to {primary capabilities}. Covers provisioning, configuration, and SDK usage.
---

# {Azure Service Name}

{One paragraph: what the service does, primary use cases}

## Overview

- **Category**: {Compute/Storage/AI/Networking/etc.}
- **Key capability**: {main value proposition}
- **When to use**: {scenarios}

## Getting Started

### Prerequisites
- Azure subscription
- {Other requirements}

### Provisioning
{CLI/Portal/Bicep snippet for creating the resource}

## SDK Usage ({Language})

### Installation
```
{包安装命令}```

### Authentication
```{language}
{验证码模式}```

### Basic Operations
```{language}
{CRUD或主操作}```

## Key Configurations

| Setting | Purpose | Default |
|---------|---------|---------|
| {setting} | {what it controls} | {value} |

## Pricing & Limits

- **Pricing model**: {consumption/tier-based/etc.}
- **Key limits**: {important quotas}

For current pricing: `microsoft_docs_search(query="{service} pricing")`

## Common Patterns

### {Pattern 1}
{Code or configuration}

### {Pattern 2}
{Code or configuration}

## Troubleshooting

| Issue | Solution |
|-------|----------|
| {Common error} | {Fix} |

For more issues: `microsoft_docs_search(query="{service} troubleshoot {symptom}")`

## Learn More

| Topic | How to Find |
|-------|-------------|
| REST API | `microsoft_docs_fetch(url="{url}")` |
| ARM/Bicep | `microsoft_docs_search(query="{service} bicep template")` |
| Security | `microsoft_docs_search(query="{service} security best practices")` |
```
---

模板3:Framework/Platform技能

对于开发框架和平台(例如，ASP。NET， MAUI, Blazor)。```markdown
---
name: {framework-name}
description: Build {type of apps} with {Framework}. Use when agents need to create, modify, or debug {framework} applications.
---

# {Framework Name}

{One paragraph: what it is, what you build with it, why choose it}

## Project Structure

```
{典型项目}/
├──{文件夹}/ #{目的}
├──{文件}#{目的}
流星──{档案}#{目的}```

## Getting Started

### Create New Project
```bash
{CLI命令到脚手架}```

### Project Configuration
{Key files to configure and what they control}

## Core Concepts

### {Concept 1: e.g., "Components"}
{Explanation with minimal code example}

### {Concept 2: e.g., "Routing"}
{Explanation with minimal code example}

### {Concept 3: e.g., "State Management"}
{Explanation with minimal code example}

## Common Patterns

### {Pattern 1}
```{language}
{代码}```

### {Pattern 2}
```{language}
{代码}```

## Configuration Options

| Setting | File | Purpose |
|---------|------|---------|
| {setting} | {file} | {what it does} |

## Deployment

{Brief deployment guidance or reference}

For detailed deployment: `microsoft_docs_search(query="{framework} deploy {target}")`

## Learn More

| Topic | How to Find |
|-------|-------------|
| {Advanced feature} | `microsoft_docs_search(query="{framework} {feature}")` |
| {Integration} | `microsoft_docs_fetch(url="{url}")` |
| {Samples} | `microsoft_code_sample_search(query="{framework} {scenario}")` |
```
---

##模板4:API/Protocol技能

对于api、协议和规范（例如，Microsoft Graph、OOXML）。```markdown
---
name: {api-name}
description: Interact with {API/Protocol}. Use when agents need to {primary operations}. Covers authentication, endpoints, and common operations.
---

# {API/Protocol Name}

{One paragraph: what it provides access to, primary use cases}

## Authentication

{Auth method and code pattern}

## Base Configuration

- **Base URL**: `{url}`
- **Version**: `{version}`
- **Format**: {JSON/XML/etc.}

## Common Endpoints/Operations

### {Operation 1: e.g., "List Items"}
```
{HTTP方法}{端点}```
```{language}
{SDK代码}```

### {Operation 2: e.g., "Create Item"}
```
{HTTP方法}{端点}```
```{language}
{SDK代码}```

## Request/Response Patterns

### Pagination
{How to handle pagination}

### Error Handling
{Error format and common codes}

## Quick Reference

| Operation | Endpoint/Method | Notes |
|-----------|-----------------|-------|
| {op} | `{endpoint}` | {note} |

## Permissions/Scopes

| Operation | Required Permission |
|-----------|---------------------|
| {op} | `{permission}` |

## Learn More

| Topic | How to Find |
|-------|-------------|
| Full endpoint reference | `microsoft_docs_fetch(url="{url}")` |
| Permissions | `microsoft_docs_search(query="{api} permissions {resource}")` |
| SDKs | `microsoft_docs_search(query="{api} SDK {language}")` |
```
---

选择模板

|技术类型|模板|样例||-----------------|----------|----------|
|客户端库，NuGet/npm包|SDK/Library|语义内核，Azure SDK, MSAL |
| Azure资源| Azure服务| Cosmos DB、Azure Functions、App Service |
|应用开发框架|Framework/Platform| ASP。. NET Core, Blazor, MAUI b|
| REST API，协议，规范|API/Protocol| Microsoft Graph， OOXML, FHIR |

自定义指南

模板是起点。自定义:

1. **增加部分**的独特方面的技术
2. **删除不适用的部分**
3. **根据复杂性调整深度**（复杂技术有更多概念）
4. **为不适合SKILL.md的详细内容添加参考文件**
5. **添加sample_codes/**用于内联代码段以外的工作示例