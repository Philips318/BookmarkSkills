---
name: source-driven-development
description: 将每个实现决策建立在官方文档之上。当你想要权威、有来源引用、避免过时模式的代码时使用。用于使用任何框架或库构建且正确性重要时。
---

# 来源驱动开发

## 概述

每个框架相关的代码决策都必须由官方文档支持。不要凭记忆实现：验证、引用，并让用户看到你的来源。训练数据会过时，API 会废弃，最佳实践会演进。这个技能确保用户得到可信代码，因为每个模式都能追溯到他们可以检查的权威来源。

## 何时使用

- 用户想要遵循某个框架当前最佳实践的代码
- 构建 boilerplate、starter code，或会被复制到项目各处的模式
- 用户明确要求 documented、verified 或 “correct” implementation
- 实现框架推荐方式很重要的功能（forms、routing、data fetching、state management、auth）
- 审查或改进使用框架特定模式的代码
- 任何你即将凭记忆编写框架特定代码的时候

**何时不使用：**

- 正确性不依赖具体版本（重命名变量、修 typo、移动文件）
- 跨所有版本都相同的纯逻辑（loops、conditionals、data structures）
- 用户明确要求速度优先于验证（“just do it quickly”）

## 流程

```
DETECT ──→ FETCH ──→ IMPLEMENT ──→ CITE
  │          │           │            │
  ▼          ▼           ▼            ▼
 What       Get the    Follow the   Show your
 stack?     relevant   documented   sources
            docs       patterns
```

### 步骤 1：检测技术栈和版本

读取项目依赖文件，识别精确版本：

```
package.json    → Node/React/Vue/Angular/Svelte
composer.json   → PHP/Symfony/Laravel
requirements.txt / pyproject.toml → Python/Django/Flask
go.mod          → Go
Cargo.toml      → Rust
Gemfile         → Ruby/Rails
```

明确说明你发现了什么：

```
STACK DETECTED:
- React 19.1.0 (from package.json)
- Vite 6.2.0
- Tailwind CSS 4.0.3
→ Fetching official docs for the relevant patterns.
```

如果版本缺失或模糊，**询问用户**。不要猜，版本决定哪些模式是正确的。

### 步骤 2：获取官方文档

获取你正在实现的功能对应的具体文档页。不是首页，不是完整文档，而是相关页面。

**来源层级（按权威性排序）：**

| 优先级 | 来源 | 示例 |
|----------|--------|---------|
| 1 | 官方文档 | react.dev, docs.djangoproject.com, symfony.com/doc |
| 2 | 官方博客 / changelog | react.dev/blog, nextjs.org/blog |
| 3 | Web standards references | MDN, web.dev, html.spec.whatwg.org |
| 4 | Browser/runtime compatibility | caniuse.com, node.green |

**不权威，永远不要作为主要来源引用：**

- Stack Overflow answers
- Blog posts 或 tutorials（即使很流行）
- AI-generated documentation 或 summaries
- 你自己的训练数据（这正是要验证的原因）

**精确获取你需要的内容：**

```
BAD:  Fetch the React homepage
GOOD: Fetch react.dev/reference/react/useActionState

BAD:  Search "django authentication best practices"
GOOD: Fetch docs.djangoproject.com/en/6.0/topics/auth/
```

获取后，提取关键模式，并记录任何 deprecation warnings 或 migration guidance。

当官方来源互相冲突时（例如 migration guide 与 API reference 矛盾），把分歧暴露给用户，并根据检测到的版本验证哪种模式实际有效。

### 步骤 3：按文档模式实现

编写与文档展示一致的代码：

- 使用 docs 中的 API signatures，而不是凭记忆
- 如果 docs 展示了新方式，就使用新方式
- 如果 docs 废弃了某个模式，不要使用 deprecated version
- 如果 docs 没有覆盖某件事，把它标记为未验证

**当 docs 与现有项目代码冲突时：**

```
CONFLICT DETECTED:
The existing codebase uses useState for form loading state,
but React 19 docs recommend useActionState for this pattern.
(Source: react.dev/reference/react/useActionState)

Options:
A) Use the modern pattern (useActionState) — consistent with current docs
B) Match existing code (useState) — consistent with codebase
→ Which approach do you prefer?
```

暴露冲突。不要静默选择。

### 步骤 4：引用来源

每个框架特定模式都要有引用。用户必须能够验证每个决策。

**代码注释中：**

```typescript
// React 19 form handling with useActionState
// Source: https://react.dev/reference/react/useActionState#usage
const [state, formAction, isPending] = useActionState(submitOrder, initialState);
```

**对话中：**

```
I'm using useActionState instead of manual useState for the
form submission state. React 19 replaced the manual
isPending/setIsPending pattern with this hook.

Source: https://react.dev/blog/2024/12/05/react-19#actions
"useTransition now supports async functions [...] to handle
pending states automatically"
```

**引用规则：**

- 使用完整 URLs，不要短链接
- 尽量使用带 anchors 的深链接（例如 `/useActionState#usage` 优于 `/useActionState`）：anchors 比顶层页面更能抵抗文档重组
- 当来源支持非显而易见的决策时，引用相关段落
- 推荐平台特性时，包含 browser/runtime support data
- 如果找不到某个模式的文档，请明确说明：

```
UNVERIFIED: I could not find official documentation for this
pattern. This is based on training data and may be outdated.
Verify before using in production.
```

诚实说明无法验证的内容，比虚假自信更有价值。

## 常见合理化借口

| 合理化借口 | 现实 |
|---|---|
| “我对这个 API 很有信心” | 信心不是证据。训练数据包含看起来正确但对当前版本会破坏的过时模式。验证。 |
| “获取 docs 浪费 tokens” | 幻觉 API 浪费更多。用户调试一小时后才发现函数签名变了。一次 fetch 避免数小时返工。 |
| “Docs 不会有我需要的内容” | 如果 docs 没覆盖，这本身就是有价值信息：该模式可能不是官方推荐。 |
| “我只要说它可能过时就行” | 免责声明没有帮助。要么验证并引用，要么明确标记为未验证。含糊是最糟选择。 |
| “这是简单任务，不需要检查” | 使用错误模式的简单任务会变成模板。用户在发现现代方式前，可能已把 deprecated form handler 复制到十个组件里。 |

## 危险信号

- 没有为该版本检查 docs 就写框架特定代码
- 用 “I believe” 或 “I think” 讲 API，而不是引用来源
- 不知道模式适用于哪个版本就实现
- 引用 Stack Overflow 或 blog posts，而不是官方文档
- 使用训练数据中出现的 deprecated APIs
- 实现前没有阅读 `package.json` / dependency files
- 交付代码时没有为框架特定决策提供来源引用
- 只需要一个页面却抓取整个 docs site

## 验证

使用来源驱动开发实现后：

- [ ] 已从依赖文件识别框架和库版本
- [ ] 已为框架特定模式获取官方文档
- [ ] 所有来源都是官方文档，而不是 blog posts 或训练数据
- [ ] 代码遵循当前版本文档展示的模式
- [ ] 非平凡决策包含带完整 URL 的来源引用
- [ ] 未使用 deprecated APIs（已对照 migration guides 检查）
- [ ] Docs 与现有代码之间的冲突已暴露给用户
- [ ] 任何无法验证的内容都已明确标记为未验证
