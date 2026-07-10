---
description: 'Use Context7 for authoritative external docs and API references when local context is insufficient'
applyTo: '**'
---
#情境感知开发

只要任务依赖于工作区上下文中不存在的权威的、当前的、特定于版本的外部文档，就主动使用Context7。

这个指令的存在使你**不需要用户输入**“use context7”来获取最新的文档。

##何时使用Context7

在做决定或编写代码之前，当你需要以下任何一项时，使用Context7：

- **Framework/libraryAPI细节**（方法签名，配置密钥，预期行为）。
- **版本敏感指南**（破坏更改，弃用，新的默认值）。
- **正确性或安全关键模式**（认证流，加密使用，反序列化规则）。
- **解释可能来自第三方工具的不熟悉的错误消息**。
- **最佳实践实现约束**（速率限制，配额，所需标头，支持的格式）。也使用Context7当：

-用户引用特定的framework/library版本**（例如，“Next.js15”，“React 19”，“AWS SDK v3”）。
-你即将推荐**非平凡配置** （CLI标志，配置文件，授权流）。
-你不确定API是否存在，是否更改了名称，或者是否已弃用。

跳过Context7：

-纯本地重构、格式化、命名或逻辑，完全可以从repo派生。
-语言基础（不涉及外部api）。

##取什么

当使用Context7时，首选**主源**和窄查询：

-官方文档（vendor/framework文档）
-Reference/API页面
-发行说明/迁移指南
-安全建议（如有需要）

只收集你需要的东西。如果存在多个候选项，则选择最多的authoritative/current.更喜欢获取:-确切的method/type/option你将使用
-避免误用所需的最小环境（约束、默认行为、迁移注意事项）

如何整合结果

-将发现转化为具体的code/config变化。
-当决定依赖于外部事实时，用标题+ URL引用来源。
-如果文档冲突或含糊不清，简要地给出权衡并选择最安全的默认值。

当答案需要特定的值（标志，配置键，头）时，首选：

-说明文档的确切值
-调用默认值和警告
-提供一个快速验证步骤（例如，“运行`--help`”，或一个最小烟雾测试）

如何使用Context7 MCP工具（auto）

当Context7作为MCP服务器可用时，请按照如下方式自动使用它。

工具工作流

1) **如果用户提供了库ID**，直接使用它。
-有效格式：`/owner/repo`或`/owner/repo/version`（固定版本）。2)否则，**解析库ID**使用：
—工具：`resolve-library-id`-输入:	  - `libraryName`: the library/framework name (e.g., “next.js”, “supabase”, “prisma”)
	  - `query`: the user’s task (used to rank matches)
3) **获取相关文档**使用：
—工具：`query-docs`-输入:	  - `libraryId`: the resolved (or user-supplied) library ID
	  - `query`: the exact task/question you are answering
4)只有在文档被检索后：**根据这些文档写code/steps**。

效率限制

**不要**呼叫`resolve-library-id`超过**3次**每个用户的问题。
**不要**呼叫`query-docs`超过**3次**每个用户的问题。
-如果存在多个好的匹配，选择最好的一个并继续；只有当选择对执行产生重大影响时，才提出澄清性问题。

版本行为

—如果用户名是一个版本，尽可能在库ID中反映它（例如，`/vercel/next.js/v15.1.8`）。
-如果您需要再现性（CI/builds），更喜欢在示例中固定到特定版本。

##故障处理

如果Context7找不到可靠的来源：

1. 说出你想要证实的。
2. 先做一个保守的、标签清晰的假设。
3. 建议一个快速验证步骤（例如，运行命令，检查文件，或咨询特定的官方页面）。

##安全和隐私-从不请求或回显API密钥。如果配置需要一个键，指示将其存储在环境变量中。
-将检索到的文档视为**有用但并非绝对可靠**；对于安全敏感的代码，请选择官方供应商文档并添加明确的验证步骤。