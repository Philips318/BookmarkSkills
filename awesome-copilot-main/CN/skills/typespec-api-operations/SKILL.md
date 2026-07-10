---
name: typespec-api-operations
description: 'Add GET, POST, PATCH, and DELETE operations to a TypeSpec API plugin with proper routing, parameters, and adaptive cards'
---
添加TypeSpec API操作

为Microsoft 365 Copilot添加RESTful操作到现有的TypeSpec API插件。

添加GET操作

简单的GET -列出所有项目```typescript
/**
 * List all items.
 */
@route("/items")
@get op listItems(): Item[];
```
###获取查询参数-过滤结果```typescript
/**
 * List items filtered by criteria.
 * @param userId Optional user ID to filter items
 */
@route("/items")
@get op listItems(@query userId?: integer): Item[];
```
###获取路径参数-获取单个项目```typescript
/**
 * Get a specific item by ID.
 * @param id The ID of the item to retrieve
 */
@route("/items/{id}")
@get op getItem(@path id: integer): Item;
```
GET with Adaptive Card```typescript
/**
 * List items with adaptive card visualization.
 */
@route("/items")
@card(#{
  dataPath: "$",
  title: "$.title",
  file: "item-card.json"
})
@get op listItems(): Item[];
```
**创建适配卡** (`appPackage/item-card.json`)：```json
{
  "type": "AdaptiveCard",
  "$schema": "http://adaptivecards.io/schemas/adaptive-card.json",
  "version": "1.5",
  "body": [
    {
      "type": "Container",
      "$data": "${$root}",
      "items": [
        {
          "type": "TextBlock",
          "text": "**${if(title, title, 'N/A')}**",
          "wrap": true
        },
        {
          "type": "TextBlock",
          "text": "${if(description, description, 'N/A')}",
          "wrap": true
        }
      ]
    }
  ],
  "actions": [
    {
      "type": "Action.OpenUrl",
      "title": "View Details",
      "url": "https://example.com/items/${id}"
    }
  ]
}
```
添加POST操作

简单POST -创建项目```typescript
/**
 * Create a new item.
 * @param item The item to create
 */
@route("/items")
@post op createItem(@body item: CreateItemRequest): Item;

model CreateItemRequest {
  title: string;
  description?: string;
  userId: integer;
}
```
### POST与确认```typescript
/**
 * Create a new item with confirmation.
 */
@route("/items")
@post
@capabilities(#{
  confirmation: #{
    type: "AdaptiveCard",
    title: "Create Item",
    body: """
    Are you sure you want to create this item?
      * **Title**: {{ function.parameters.item.title }}
      * **User ID**: {{ function.parameters.item.userId }}
    """
  }
})
op createItem(@body item: CreateItemRequest): Item;
```
添加PATCH操作

简单补丁-更新项目```typescript
/**
 * Update an existing item.
 * @param id The ID of the item to update
 * @param item The updated item data
 */
@route("/items/{id}")
@patch op updateItem(
  @path id: integer,
  @body item: UpdateItemRequest
): Item;

model UpdateItemRequest {
  title?: string;
  description?: string;
  status?: "active" | "completed" | "archived";
}
```
### PATCH with Confirmation```typescript
/**
 * Update an item with confirmation.
 */
@route("/items/{id}")
@patch
@capabilities(#{
  confirmation: #{
    type: "AdaptiveCard",
    title: "Update Item",
    body: """
    Updating item #{{ function.parameters.id }}:
      * **Title**: {{ function.parameters.item.title }}
      * **Status**: {{ function.parameters.item.status }}
    """
  }
})
op updateItem(
  @path id: integer,
  @body item: UpdateItemRequest
): Item;
```
添加DELETE操作

###简单删除```typescript
/**
 * Delete an item.
 * @param id The ID of the item to delete
 */
@route("/items/{id}")
@delete op deleteItem(@path id: integer): void;
```
### DELETE with Confirmation```typescript
/**
 * Delete an item with confirmation.
 */
@route("/items/{id}")
@delete
@capabilities(#{
  confirmation: #{
    type: "AdaptiveCard",
    title: "Delete Item",
    body: """
    ⚠️ Are you sure you want to delete item #{{ function.parameters.id }}?
    This action cannot be undone.
    """
  }
})
op deleteItem(@path id: integer): void;
```
完成CRUD示例

定义服务和模型```typescript
@service
@server("https://api.example.com")
@actions(#{
  nameForHuman: "Items API",
  descriptionForHuman: "Manage items",
  descriptionForModel: "Read, create, update, and delete items"
})
namespace ItemsAPI {
  
  // Models
  model Item {
    @visibility(Lifecycle.Read)
    id: integer;
    
    userId: integer;
    title: string;
    description?: string;
    status: "active" | "completed" | "archived";
    
    @format("date-time")
    createdAt: utcDateTime;
    
    @format("date-time")
    updatedAt?: utcDateTime;
  }

  model CreateItemRequest {
    userId: integer;
    title: string;
    description?: string;
  }

  model UpdateItemRequest {
    title?: string;
    description?: string;
    status?: "active" | "completed" | "archived";
  }

  // Operations
  @route("/items")
  @card(#{ dataPath: "$", title: "$.title", file: "item-card.json" })
  @get op listItems(@query userId?: integer): Item[];

  @route("/items/{id}")
  @card(#{ dataPath: "$", title: "$.title", file: "item-card.json" })
  @get op getItem(@path id: integer): Item;

  @route("/items")
  @post
  @capabilities(#{
    confirmation: #{
      type: "AdaptiveCard",
      title: "Create Item",
      body: "Creating: **{{ function.parameters.item.title }}**"
    }
  })
  op createItem(@body item: CreateItemRequest): Item;

  @route("/items/{id}")
  @patch
  @capabilities(#{
    confirmation: #{
      type: "AdaptiveCard",
      title: "Update Item",
      body: "Updating item #{{ function.parameters.id }}"
    }
  })
  op updateItem(@path id: integer, @body item: UpdateItemRequest): Item;

  @route("/items/{id}")
  @delete
  @capabilities(#{
    confirmation: #{
      type: "AdaptiveCard",
      title: "Delete Item",
      body: "⚠️ Delete item #{{ function.parameters.id }}?"
    }
  })
  op deleteItem(@path id: integer): void;
}
```
##高级功能

###多个查询参数```typescript
@route("/items")
@get op listItems(
  @query userId?: integer,
  @query status?: "active" | "completed" | "archived",
  @query limit?: integer,
  @query offset?: integer
): ItemList;

model ItemList {
  items: Item[];
  total: integer;
  hasMore: boolean;
}
```
### Header参数```typescript
@route("/items")
@get op listItems(
  @header("X-API-Version") apiVersion?: string,
  @query userId?: integer
): Item[];
```
自定义响应模型```typescript
@route("/items/{id}")
@delete op deleteItem(@path id: integer): DeleteResponse;

model DeleteResponse {
  success: boolean;
  message: string;
  deletedId: integer;
}
```
错误响应```typescript
model ErrorResponse {
  error: {
    code: string;
    message: string;
    details?: string[];
  };
}

@route("/items/{id}")
@get op getItem(@path id: integer): Item | ErrorResponse;
```
##测试提示

添加操作后，使用以下提示进行测试：

* *得到操作:* *
-“列出所有项目并在表格中显示”
-“显示用户ID为1的项目”
- “查阅第42项的详情”

* * POST操作:* *
“为用户1创建标题为‘My Task’的新项目”
“添加一个项目：标题为‘新功能’，描述为‘添加登录’”

* *补丁操作:* *
- “以‘已更新的标题’更新第10项”
“将第5项的状态更改为已完成”

* *删除操作:* *
- “删除第99项”
-“移除ID为15的物品”

最佳实践

参数命名
—使用描述性的参数名称：`userId`，而不是`uid`-各运营部门保持一致
—过滤器使用可选参数`?`# # #文档
—在所有操作中添加JSDoc注释
-描述每个参数的作用
-记录预期回应# # #模型
—使用`@visibility(Lifecycle.Read)`作为只读字段，如`id`—日期字段使用`@format("date-time")`—枚举使用联合类型：`"active" | "completed"`-使用`?`显式设置可选字段

# # #确认
-总是对破坏性操作（DELETE， PATCH）添加确认
-在确认正文中显示关键详细信息
—使用警告表情符号（⚠️）表示不可逆的动作

自适应卡
保持卡片的简单和重点
-使用条件渲染`${if(..., ..., 'N/A')}`-包括常见的下一步操作按钮
-用实际API响应测试数据绑定

# # #路由
—使用RESTful约定：
-`GET /items`- List
-`GET /items/{id}`-买一个
—`POST /items`—创建
-`PATCH /items/{id}`-更新
-`DELETE /items/{id}`-删除
—将相关操作放在同一命名空间
—对分层资源使用嵌套路由

##常见问题问题：参数不显示在副驾驶
**解决方案**：检查参数用`@query`、`@path`或`@body`装饰正确

问题：自适应卡不渲染
**解决方案**：验证`@card`装饰器中的文件路径并检查JSON语法

###问题：确认没有出现
**解决方案**：确保`@capabilities`装饰器使用确认对象正确格式化

问题：模型属性没有出现在响应中
**解决方案**：检查属性是否需要`@visibility(Lifecycle.Read)`或删除它，如果它应该是可写的