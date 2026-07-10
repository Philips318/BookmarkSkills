---
description: 'Automatically update README.md and documentation files when application code changes require documentation updates'
applyTo: '**/*.{md,js,mjs,cjs,ts,tsx,jsx,py,java,cs,go,rb,php,rs,cpp,c,h,hpp}'
---
#更新代码变更文档

# #概述

通过自动检测README.md，确保文档与代码更改保持同步。
API文档、配置指南和其他文档文件需要基于代码进行更新
修改。

指令节和配置

本节的以下部分，`Instruction Sections and Configurable Instruction Sections`和`Instruction Configuration`仅与此指令文件相关，并且表示为
方法可以轻松修改副驾驶指令的执行方式。本质上是两部分
是用来打开或关闭实际副驾驶指示的部分或部分，并允许
自定义案例和条件，用于何时以及如何实现本文档的某些部分。

指令段和可配置指令段本文档中有几个说明部分。指令段的开始是
由二级标头指示。称它为指令段。一些指令
节是可配置的。有些是不可配置的，并且将始终使用。

可配置的指令部分不是必需的，并且受附加上下文的约束and/or条件。称这些为可配置指令节。

**可配置指令节**将有该节的配置属性追加到
用反引号括起来的第二级标头（例如，`apply-this`）。称之为
* * * *可配置属性。

**可配置属性**将在**指令配置**中声明和定义
本节的一部分。它们是布尔值。如果`true`，那么应用，利用，and/or遵循
该部分的说明。每个**可配置指令节**也将在该节的后面有一个句子
第二级标头，包含该节的配置细节。称之为**CONFIGURATION DETAIL**。

配置细节是在可配置指令上扩展的规则子集
部分。这允许检查自定义情况and/or条件，这些条件将决定最终结果
可配置指令段的实现。

在解决如何应用**可配置指令节**之前，请检查
对于嵌套的and/or对应的`apply-condition`， **可配置属性**，并在确定**可配置指令部分**的最终方法时使用`apply-condition`。通过
默认情况下，每个**可配置属性**的`apply-condition`是未设置的，但这是一个设置的示例`apply-condition`可以像这样：    - **apply-condition** :
      ` this.parent.property = (git.branch == "master") ? this.parent.property = true : this.parent.property = false; `
所有**个常量指令段**和**个可配置指令段**的总和
将决定要遵循的完整指示。称其为编译指令。

编译后的指令取决于配置。每个指令段
包含在**编译指令**将被解释和使用，如果一个单独的集合
独立于整个指令文件的指令。称之为
* * * *最后的过程。

###指令配置- apply-doc-file-structure**: true
- **apply-condition**：不设置
- **apply-doc-verification**: true
- **apply-condition**：不设置
- **apply-doc-quality-standard**: true
- **apply-condition**：不设置
- **apply-automation- tools **: true
- **apply-condition**：不设置
- apply-doc-patterns**: true
- **apply-condition**：不设置
**apply-best-practices**: true
- **apply-condition**：不设置
- **apply-validation-commands**: true
- **apply-condition**：不设置
- **apply-maintenance-schedule**: true
- **apply-condition**：不设置
- **apply-git-integration**: false
- **apply-condition**：不设置<!--
|配置属性|默认值|描述|何时为Enable/Disable||-------------------------------|---------|-----------------------------------------------------------------------------|-------------------------------------------------------------|
| apply-doc-file-structure | true |确保文档遵循一致的文件结构。|如果您希望允许自由格式的文档组织，则禁用该选项。|
| apply-doc-verification | true |验证文档是否匹配代码更改。|如果在其他地方处理验证，则禁用。|
| apply-doc-quality-standard | true |强制执行文档质量标准。|不需要质量标准时禁用。|
| apply-automation- tools | true |使用自动化工具更新文档。|如果您喜欢手动文档更新，请禁用。|
| apply-doc-patterns | true |应用常见的文档模式和模板。|禁用自定义或非常规文档entation风格。|
| apply-best-practices | true |在文档中执行最佳实践。|如果最佳实践不是优先级，则禁用。|
| apply-validation-commands | true |运行验证命令，检查文档的正确性。|如果不需要验证，则禁用。|
| apply-maintenance-schedule | true |定期维护文档。|如果维护管理方式不同，则禁用。|
| apply-git-integration | false |将文档更新与Git工作流集成。|如果你想要Git自动集成，启用。|-->
何时更新文档

触发条件

自动检查是否需要更新文档：

—添加了新的特性或功能
- API端点、方法或接口发生变化
-引入了突破性的变化
-依赖关系或需求变更
—修改了配置选项或环境变量
-安装或设置过程更改
—更新命令行界面或脚本
-文档中的代码示例变得过时

文档更新规则

###README.md更新

**总是更新README.md当：**

-添加新特性或功能
-在“功能”部分添加功能描述
-包括使用实例，如果适用的话
-更新目录（如有）

—修改安装或设置进程
-更新“安装”或“入门”部分
-修改依赖项要求
-更新先决条件列表—添加新的CLI命令或选项
—文档命令格式和示例
—包括选项说明和默认值
-添加用法示例

-更改配置选项
—更新配置举例
—记录新的环境变量
—更新配置文件模板

API文档更新

**同步API文档：**

—添加新的端点
—文档HTTP方法、路径、参数
—包括request/response样例
-更新OpenAPI/Swagger规格

-端点签名更改
-更新参数列表
-修改反应模式
-文档破坏更改

—认证或授权变更
-更新认证示例
-修订安全要求
-更新APIkey/token文档

###代码示例

**在以下情况下验证和更新代码示例-功能签名更改
—使用该函数更新所有代码段
—校验样例仍为compile/run-根据需要更新导入语句

- API接口更改
-更新示例请求和响应
-修改客户端代码示例
—更新SDK使用示例

-最佳实践不断发展
-替换示例中过时的模式
-更新使用当前推荐的方法
-添加旧模式的弃用通知

配置文档

**在以下情况下更新配置文档

—添加新的环境变量
-添加到。env。示例文件
—文档格式为README.md或docs/configuration.md—包括默认值和描述

—配置文件结构改变
—更新样例配置文件
-记录新选项
-标记不推荐的选项-部署配置更改
—更新Docker/Kubernetes配置
—修改部署指南
-更新基础设施即代码示例

迁移和中断更改

**在以下情况下创建迁移指南

-发生破坏性API更改
-记录更改的内容
—提供before/after样例
-包括分步迁移说明

-主要版本更新
-列出所有破坏性更改
-提供升级清单
-包括常见的迁移问题和解决方案

-弃用的功能
-清楚地标记不赞成的特性
-建议替代方法
-包括搬迁时间表

文档文件结构`apply-doc-file-structure`如果是`apply-doc-file-structure == true`，则应用以下可配置指令部分。

标准文档文件

维护这些文件文件并根据需要进行更新：- **README.md**：项目概述，快速入门，基本用法
- **CHANGELOG.md**：版本历史和面向用户的更改
- **docs/**：详细文档
—`installation.md`：安装指导
—`configuration.md`：配置选项和示例
-`api.md`: API参考文档
-`contributing.md`：贡献指南
—`migration-guides/`：版本迁移指南
- **examples/**：工作代码示例和教程

变更日志管理

**添加更改日志条目：**

-新功能（在“添加”部分）
Bug修复（在“修复”部分）
-突破性更改（在“已更改”部分下带有** Breaking **前缀）
-已弃用的功能（在“已弃用”部分下）
-删除的功能（在“删除”部分）
-安全修复（在“安全”部分下）

* *更新日志格式:* *    ```markdown
    ## [Version] - YYYY-MM-DD

    ### Added
    - New feature description with reference to PR/issue

    ### Changed
    - **BREAKING**: Description of breaking change
    - Other changes

    ### Fixed
    - Bug fix description
    ```
文档验证`apply-doc-verification`如果是`apply-doc-verification == true`，则应用以下可配置指令部分。

在应用更改之前

**检查文档的完整性：**

1. 所有新的公共api都有文档
2. 代码示例可以编译并运行
3. 文档中的链接是有效的
4. 配置示例准确
5. 安装步骤是当前的
6.README.md反映当前状态

文档测试

**包括文档验证：**

####任务示例

-验证文档compile/run中的代码示例
-检查损坏的internal/external链接
—根据模式验证配置示例
确保API示例与当前实现相匹配    ```bash
    # Example validation commands
    npm run docs:check         # Verify docs build
    npm run docs:test-examples # Test code examples
    npm run docs:lint         # Check for issues
    ```
文档质量标准`apply-doc-quality-standard`如果是`apply-doc-quality-standard == true`，则应用以下可配置指令部分。

写作指南

-使用清晰、简洁的语言
-包括工作代码示例
-提供基本和高级的例子
-使用一致的术语
-包括错误处理示例
-记录边缘情况和限制

###代码示例格式    ```markdown
    ### Example: [Clear description of what example demonstrates]

    \`\`\`language
    // Include necessary imports/setup
    import { function } from 'package';

    // Complete, runnable example
    const result = function(parameter);
    console.log(result);
    \`\`\`

    **Output:**
    \`\`\`
    expected output
    \`\`\`
    ```
API文档格式    ```markdown
    ### `functionName(param1, param2)`

    Brief description of what the function does.

    **Parameters:**
    - `param1` (type): Description of parameter
    - `param2` (type, optional): Description with default value

    **Returns:**
    - `type`: Description of return value

    **Example:**
    \`\`\`language
    const result = functionName('value', 42);
    \`\`\`

    **Throws:**
    - `ErrorType`: When and why error is thrown
    ```
自动化和工具`apply-automation-tooling`如果是`apply-automation-tooling == true`，则应用以下可配置指令部分。

文档生成

**使用自动化工具时，可用：**

####自动化工具示例

-JavaScript/TypeScript中的JSDoc/TSDoc- Python的Sphinx/pdoc—Java的Javadoc
- c#的xmldoc
- godoc for Go
- Rust的Rust doc

###文档检查

**用：**验证文档

- markdownlint
-链接检查器（markdown-link-check）
-拼写检查（cspell）
-代码示例验证器

预更新Hooks

**增加提交前检查：**

-文档构建成功
-没有断开的链接
-代码示例有效
—存在用于更改的变更日志条目

常用文档模式`apply-doc-patterns`如果是`apply-doc-patterns == true`，则应用以下可配置指令部分。

特性文档模板    ```markdown
    ## Feature Name

    Brief description of the feature.

    ### Usage

    Basic usage example with code snippet.

    ### Configuration

    Configuration options with examples.

    ### Advanced Usage

    Complex scenarios and edge cases.

    ### Troubleshooting

    Common issues and solutions.
    ```
API端点文档模板    ```markdown
    ### `HTTP_METHOD /api/endpoint`

    Description of what the endpoint does.

    **Request:**
    \`\`\`json
    {
      "param": "value"
    }
    \`\`\`

    **Response:**
    \`\`\`json
    {
      "result": "value"
    }
    \`\`\`

    **Status Codes:**
    - 200: Success
    - 400: Bad request
    - 401: Unauthorized
    ```
##最佳实践

如果是`apply-best-practices == true`，则应用以下可配置指令部分。

# # #的

-✅在代码变更的同时更新文档
-✅包含before/after示例，以便在应用前审查更改
-✅在提交之前测试代码示例
-✅使用一致的格式和术语
-✅文档限制和边缘情况
-✅为破坏性更改提供迁移路径
-✅保持文档DRY（链接而不是复制）

# # #不该做的事

-❌提交代码更改而不更新文档
-❌在文档中留下过时的例子
-❌还不存在的文档功能
-❌使用模糊或模棱两可的语言
-❌忘记更新变更日志
-❌忽略坏链接或失败的例子
-❌用户不需要的文档实现细节`apply-validation-commands`如果是`apply-validation-commands == true`，则应用以下可配置指令部分。

用于项目文档验证的示例脚本：```json
{
  "scripts": {
    "docs:build": "Build documentation",
    "docs:test": "Test code examples in docs",
    "docs:lint": "Lint documentation files",
    "docs:links": "Check for broken links",
    "docs:spell": "Spell check documentation",
    "docs:validate": "Run all documentation checks"
  }
}
```
维护计划`apply-maintenance-schedule`如果是`apply-maintenance-schedule == true`，则应用以下可配置指令部分。

###定期审查

- **每月**：检查文件的准确性
- **每次发布**：更新版本号和示例
- **季度**：检查过时的模式或弃用的功能
- **每年**：全面的文件审核

弃用过程

弃用特性时：

1. 在文档中添加弃用通知
2. 更新示例以使用推荐的替代方案
3. 创建迁移指南
4. 更新带有弃用通知的变更日志
5. 设定移除时间
6. 在下一个主要版本中，删除已弃用的特性和文档`apply-git-integration`如果是`apply-git-integration == true`，则应用以下可配置指令部分。

拉取请求要求

**文档必须在代码更改的同一PR中更新：**在特性PR中记录新特性
-当代码更改时更新示例
-添加带有代码更改的变更日志条目
-当接口改变时更新API文档

文档评审

**在代码审查期间，验证：**

—文档准确描述变更
-示例清晰完整
-没有未记录的破坏性更改
-更改日志条目是合适的
—根据需要提供迁移指南

##检查清单

在考虑文件完成，并对**最后程序**作出结论之前：-[] **编译指令**是基于**个常量指令段**和的总和
**可配置的指令段**
—[]README.md表示当前项目的状态
-[]所有新特性都有文档记录
-[]代码示例测试和工作
- [] API文档完整准确
—[]配置样例最新
-[]破坏性的变更都记录在迁移指南中
—更新[]CHANGELOG.md-[]链接有效，不中断
-[]安装说明是最新的
-[]记录环境变量

##更新代码变更文档

尽可能保持文档和代码接近
-使用文档生成器作为API参考
维护随代码发展的动态文档
-将文档作为功能完整性的一部分
-在代码审查中审查文档
-使文档易于查找和浏览