---
title: Power Platform Connectors Schema Development Instructions
description: 'Comprehensive development guidelines for Power Platform Custom Connectors using JSON Schema definitions. Covers API definitions (Swagger 2.0), API properties, and settings configuration with Microsoft extensions.'
applyTo: '**/*.{json,md}'
---
#电源平台连接器模式开发说明

##项目概述
这个工作区包含电源平台自定义连接器的JSON模式定义，特别是`paconn`（电源应用程序连接器）工具。模式验证并提供智能感知：

**API定义** （Swagger 2.0格式）
- **API属性**（连接器元数据和配置）
—**设置**（环境和部署配置）

##文件结构理解### 1. apiDefinition.swagger.json
- **目的**：此文件包含带有Power Platform扩展的Swagger 2.0 API定义。
- **主要特点**：
-标准Swagger 2.0属性，包括信息、路径、定义等。
-以`x-ms-*`开头的microsoft特定扩展。
-自定义格式类型，专门为电力平台设计，如`date-no-tz`和`html`。
-动态模式支持，提供运行时灵活性。
—支持OAuth2、API Key和Basic Auth认证方式的安全定义。### 2. apiProperties.json
—**目的**：定义连接器元数据、认证配置和策略配置。
- **关键部件**：
—**连接参数**：支持多种认证类型，包括OAuth、API Key和Gateway配置。
—**策略模板实例**：处理连接器的数据转换和路由策略。
-连接器元数据：包括发布者信息、功能和品牌元素。### 3. settings.json
—**目的**：提供paconn工具的环境和部署配置信息。
- **配置选项**：
-针对特定Power Platform环境的环境GUID。
-连接器资产和配置文件的文件路径映射。
-生产和测试环境的API端点url （PROD/TIP1）。
—API版本规范，确保与Power Platform服务兼容。

##开发指南

使用API定义时（Swagger）
1. **始终根据Swagger 2.0规范进行验证** -该模式强制严格遵守Swagger 2.02. **Microsoft Extensions for Operations**：
-`x-ms-summary`：使用它来提供用户友好的显示名称，并确保使用标题大小写格式。
—`x-ms-visibility`：使用该参数控制参数可见性，值为`important`、`advanced`或`internal`。
—`x-ms-trigger`：使用该值将操作标记为触发器，值为`batch`或`single`。
-`x-ms-trigger-hint`：使用它来提供有用的提示文本，指导用户在使用触发器时。
-`x-ms-trigger-metadata`：使用它来定义触发器配置设置，包括类型和模式属性。
—`x-ms-notification`：用于配置webhook操作的实时通知。
-`x-ms-pageable`：通过指定`nextLinkName`属性来启用分页功能。
-`x-ms-safe-operation`：当POST操作没有副作用时，使用此标记为安全。
—`x-ms-no-generic-test`：禁用特定操作的自动测试。
-`x-ms-operation-context`:使用它来配置用于测试目的的操作模拟设置。3. **微软扩展参数**：
-`x-ms-dynamic-list`：使用它来启用从API调用填充的动态下拉列表。
-`x-ms-dynamic-values`：使用它来配置填充参数选项的动态值源。
-`x-ms-dynamic-tree`：使用它为嵌套的数据结构创建分层选择器。
—`x-ms-dynamic-schema`：允许基于用户选择更改运行时模式。
-`x-ms-dynamic-properties`：用于适应上下文的动态属性配置。
-`x-ms-enum-values`：使用它来提供带有显示名称的增强枚举定义，以获得更好的用户体验。
-`x-ms-test-value`：用于提供测试的样例值，但绝不包括机密或敏感数据。
—`x-ms-trigger-value`：使用它来指定带有`value-collection`和`value-path`属性的触发器参数的具体值。
-`x-ms-url-encoding`：使用它指定URL编码样式为`single`或`double`(默认为`single`)。
-`x-ms-parameter-location`：使用它为API （AutoRest扩展-被Power Platform忽略）提供参数位置提示。
—`x-ms-localizeDefaultValue`：用于启用默认参数值的本地化。
-`x-ms-skip-url-encoding`：使用此参数跳过路径参数的URL编码（AutoRest扩展-被Power Platform忽略）。4. **Microsoft扩展模式**：
-`x-ms-notification-url`：使用它来标记模式属性作为webhook配置的通知URL。
—`x-ms-media-kind`：用于指定内容的媒体类型，支持的值为`image`或`audio`。
-`x-ms-enum`：使用它来提供增强的enum元数据（AutoRest扩展-被Power Platform忽略）。
注意，上面列出的所有参数扩展也适用于模式属性，并且可以在模式定义中使用。5. * * * *根级扩展:
-`x-ms-capabilities`：使用它来定义连接器功能，如文件选择器和testConnection功能。
-`x-ms-connector-metadata`：使用它提供标准属性之外的附加连接器元数据。
—`x-ms-docs`：使用它来配置连接器的文档设置和引用。
—`x-ms-deployment-version`：用于跟踪版本信息，以便进行部署管理。
-`x-ms-api-annotation`：使用它来添加api级别的注释以增强功能。

6. * * * *等级扩展:
-`x-ms-notification-content`：用来定义webhook路径项的通知内容模式。

7. * * * *操作级别功能:
—`x-ms-capabilities`（在操作级别）：使用它来启用特定于操作的功能，例如用于大文件传输的`chunkTransfer`。8. * * * *安全注意事项:
您应该为API定义合适的`securityDefinitions`，以确保正确的身份验证。
- **允许多个安全定义** -您可以定义最多两个验证方法（例如，oauth2 + apiKey, basic + apiKey）。
- **例外：如果使用“None”身份验证，则在同一连接器中不能存在其他安全定义。
-应该将`oauth2`用于现代api，将`apiKey`用于简单的令牌身份验证，并将`basic`身份验证仅用于internal/legacy系统。
-每个安全定义必须是一种类型（此约束由oneOf验证强制执行）。9. **参数最佳实践**：
-您应该使用描述性的`description`字段来帮助用户理解每个参数的目的。
-你应该实现`x-ms-summary`以获得更好的用户体验（需要标题大小写）。
—必须正确标记所需参数，以确保正确验证。
-您应该使用适当的`format`值（包括Power Platform扩展）来启用正确的数据处理。
你应该利用动态扩展来获得更好的用户体验和数据验证。

10. **电源平台格式扩展**：
—`date-no-tz`：表示没有时间偏移信息的日期时间。
-`html`：该格式告诉客户端在编辑时发出HTML编辑器，在查看内容时发出HTML查看器。
—标准格式包括：`int32`、`int64`、`float`、`double`、`byte`、`binary`、`date`、`date-time`、`password`、`email`、`uri`、`uuid`。当使用API属性时
1. * * * *连接参数:
—应选择合适的参数类型，如`string`、`securestring`或`oauthSetting`。
-您应该使用正确的身份提供程序配置OAuth设置。
-你应该在适当的时候使用`allowedValues`作为下拉选项。
-您应该在需要条件参数时实现参数依赖。

2. * * * *政策模板:
-应该使用`routerequesttoendpoint`作为到不同API端点的后端路由。
—使用`setqueryparameter`来设置查询参数的默认值。
—您应该在分页场景中使用`updatenextlink`来正确处理分页。
—对于需要轮询行为的触发器操作，应该应用`pollingtrigger`。3. **品牌和元数据**：
-必须始终指定`iconBrandColor`，因为所有连接器都需要此属性。
-您应该定义适当的`capabilities`来指定连接器是否支持操作或触发器。
—您应该设置有意义的`publisher`和`stackOwner`值，以标识连接器的所有权。

###使用设置时
1. * * * *环境配置:
-应该为`environment`使用与验证模式匹配的GUID格式。
-您应该为目标环境设置正确的`powerAppsUrl`和`flowUrl`。
-您应该将API版本与您的特定需求相匹配。2. * * * *文件引用:
—您应该保持文件命名与默认值`apiProperties.json`和`apiDefinition.swagger.json`一致。
—本地开发环境应使用相对路径。
-你应该确保图标文件存在，并在你的配置中被正确引用。

模式验证规则

必需的属性
**API定义**:`swagger: "2.0"`，`info`（与`title`和`version`），`paths`- **API属性**:`properties`与`iconBrandColor`- **设置**：没有必要的属性（所有可选的默认值）

模式验证
- **供应商扩展**：非微软扩展必须匹配`^x-(?!ms-)`模式
—**路径项**:API路径必须以`/`开头
—**环境GUID**：必须匹配UUID格式模式`^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$`—** url **：终端配置必须是有效的url
—**主机模式**：必须匹配`^[^{}/ :\\\\]+(?::\\d+)?$`（无空格、协议、路径）类型约束
- **安全定义**：
在`securityDefinitions`对象中最多允许两个安全定义
-每个单独的安全定义必须是一种类型（oneOf验证：`basic`，`apiKey`,`oauth2`）
—**例外**:None认证不能与其他安全定义共存
—**参数类型**：只支持指定enum取值（`string`、`number`、`integer`、`boolean`、`array`、`file`）
—**策略模板**：针对类型的参数要求
—**格式值**：扩展集，包括Power Platform格式
—**可见性值**：必须为`important`、`advanced`或`internal`中的一种
—**触发类型**：必须为`batch`或`single`附加的验证规则
- **$ref引用**：应该只指向`#/definitions/`，`#/parameters/`，或`#/responses/`—**路径参数**：必须标记为`required: true`—**信息对象**：描述不能与标题相同
—**联系对象**:Email必须是有效的Email格式，URL必须是有效的URI
—**License对象**：必须输入名称，URL必须是有效的URI
—**External Docs**：必选URL，必须是有效的URI
—**标签**：数组中的标签名称必须唯一
—**方案**：必须是有效的HTTP方案（`http`,`https`,`ws`,`wss`）
—**MIME类型**：必须遵循`consumes`和`produces`的合法MIME类型格式

常见模式和示例

API定义示例

####微软扩展的基本操作```json
{
  "get": {
    "operationId": "GetItems",
    "summary": "Get items",
    "x-ms-summary": "Get Items",
    "x-ms-visibility": "important",
    "description": "Retrieves a list of items from the API",
    "parameters": [
      {
        "name": "category",
        "in": "query",
        "type": "string",
        "x-ms-summary": "Category",
        "x-ms-visibility": "important",
        "x-ms-dynamic-values": {
          "operationId": "GetCategories",
          "value-path": "id",
          "value-title": "name"
        }
      }
    ],
    "responses": {
      "200": {
        "description": "Success",
        "x-ms-summary": "Success",
        "schema": {
          "type": "object",
          "properties": {
            "items": {
              "type": "array",
              "x-ms-summary": "Items",
              "items": {
                "$ref": "#/definitions/Item"
              }
            }
          }
        }
      }
    }
  }
}
```
####触发操作配置```json
{
  "get": {
    "operationId": "WhenItemCreated",
    "x-ms-summary": "When an Item is Created",
    "x-ms-trigger": "batch",
    "x-ms-trigger-hint": "To see it work now, create an item",
    "x-ms-trigger-metadata": {
      "kind": "query",
      "mode": "polling"
    },
    "x-ms-pageable": {
      "nextLinkName": "@odata.nextLink"
    }
  }
}
```
####动态模式示例```json
{
  "name": "dynamicSchema",
  "in": "body",
  "schema": {
    "x-ms-dynamic-schema": {
      "operationId": "GetSchema",
      "parameters": {
        "table": {
          "parameter": "table"
        }
      },
      "value-path": "schema"
    }
  }
}
```
####文件选择功能```json
{
  "x-ms-capabilities": {
    "file-picker": {
      "open": {
        "operationId": "OneDriveFilePickerOpen",
        "parameters": {
          "dataset": {
            "value-property": "dataset"
          }
        }
      },
      "browse": {
        "operationId": "OneDriveFilePickerBrowse",
        "parameters": {
          "dataset": {
            "value-property": "dataset"
          }
        }
      },
      "value-title": "DisplayName",
      "value-collection": "value",
      "value-folder-property": "IsFolder",
      "value-media-property": "MediaType"
    }
  }
}
```
####测试连接能力（注意：不支持自定义连接器）```json
{
  "x-ms-capabilities": {
    "testConnection": {
      "operationId": "TestConnection",
      "parameters": {
        "param1": "literal-value"
      }
    }
  }
}
```
####仿真操作背景```json
{
  "x-ms-operation-context": {
    "simulate": {
      "operationId": "SimulateOperation",
      "parameters": {
        "param1": {
          "parameter": "inputParam"
        }
      }
    }
  }
}
```
基本OAuth配置```json
{
  "type": "oauthSetting",
  "oAuthSettings": {
    "identityProvider": "oauth2",
    "clientId": "your-client-id",
    "scopes": ["scope1", "scope2"],
    "redirectMode": "Global"
  }
}
```
####多个安全定义示例```json
{
  "securityDefinitions": {
    "oauth2": {
      "type": "oauth2",
      "flow": "accessCode",
      "authorizationUrl": "https://api.example.com/oauth/authorize",
      "tokenUrl": "https://api.example.com/oauth/token",
      "scopes": {
        "read": "Read access",
        "write": "Write access"
      }
    },
    "apiKey": {
      "type": "apiKey",
      "name": "X-API-Key",
      "in": "header"
    }
  }
}
```
**注**：最多可以同时使用两种安全定义，但“None”认证不能与其他方式组合使用。

动态参数设置```json
{
  "x-ms-dynamic-values": {
    "operationId": "GetItems",
    "value-path": "id",
    "value-title": "name"
  }
}
```
路由策略模板```json
{
  "templateId": "routerequesttoendpoint",
  "title": "Route to backend",
  "parameters": {
    "x-ms-apimTemplate-operationName": ["GetData"],
    "x-ms-apimTemplateParameter.newPath": "/api/v2/data"
  }
}
```
最佳实践

1. **使用IntelliSense**：这些模式提供丰富的自动完成和验证功能，在开发过程中提供帮助。
2. **遵循命名约定**：为操作和参数使用描述性名称，以提高代码的可读性。
3. **实现错误处理**：定义适当的响应模式和错误码，以正确处理故障场景。
4. **彻底测试**：在部署之前验证模式，以便在开发过程的早期发现问题。
5. **文档扩展**：注释微软特定的扩展，以便团队理解和将来维护。
6. **版本管理：在API信息中使用语义版本控制来跟踪更改和兼容性。
7. **安全第一：始终实现适当的身份验证机制来保护您的API端点。

# #故障排除常见模式违规
- **缺少必需的属性**:`swagger: "2.0"`，`info.title`,`info.version`,`paths`- **无效的模式格式**：
- guid必须匹配精确格式`^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$`—url必须是有效的url，且格式合理
—路径必须以“`/`”开头
—Host不能包含协议、路径和空格
- **不正确的供应商扩展名**：使用`x-ms-*`微软扩展，`^x-(?!ms-)`为其他
- **安全定义类型不匹配**：每个安全定义只能是一种类型
—**无效的enum值**：检查`x-ms-visibility`、`x-ms-trigger`、参数类型允许取值
**$ref指向无效位置**：必须指向`#/definitions/`、`#/parameters/`或`#/responses/`—**路径参数不指定**：所有路径参数必须为“`required: true`”
**：只允许在`formData`参数中使用，schema中不允许API定义的特定问题
- **动态模式冲突**：不能使用固定模式属性的`x-ms-dynamic-schema`- **触发配置错误**:`x-ms-trigger-metadata`需要`kind`和`mode`**分页设置**:`x-ms-pageable`需要`nextLinkName`属性
**文件选择器配置错误**：必须包括`open`操作和所需的属性
—**能力冲突**：部分能力可能与某些参数类型冲突
- **测试值安全**：永远不要在`x-ms-test-value`中包含秘密或PII
**操作上下文设置**:`x-ms-operation-context`需要`simulate`对象与`operationId`—**通知内容模式**：路径级`x-ms-notification-content`必须定义合适的模式结构
—**媒体类型限制**:`x-ms-media-kind`仅支持`image`或`audio`值
—**触发值配置**:`x-ms-trigger-value`必须至少有一个属性（`value-collection`或`value-path`）验证工具
-使用JSON模式验证器来检查模式定义的遵从性。
-利用VS Code的内置模式验证来捕获开发过程中的错误。
-部署前使用paconn CLI测试：`paconn validate --api-def apiDefinition.swagger.json`-根据电源平台连接器要求进行验证以确保兼容性。
-在目标环境中使用Power Platform Connector门户进行验证和测试。
检查操作响应是否与预期的模式匹配，以防止运行时错误。

请记住：这些模式确保Power Platform连接器格式正确，并能在Power Platform生态系统中正常工作。