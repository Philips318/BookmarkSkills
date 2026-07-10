---
name: interview-me
description: 提取用户真正想要的东西，而不是他们以为自己应该想要的东西。通过一次只问一个问题的访谈完成，直到对底层意图达到约 95% 信心。当需求描述不充分（“build me X” 但没有说明“为谁”或“为什么现在”）、用户显式调用（“interview me”、“grill me”、“are we sure?”、“stress-test my thinking”），或在任何计划、spec 或代码存在之前，你发现自己正在默默填补模糊需求时使用。
---

# 访谈我

## 概述

人们提出的要求和他们真正想要的东西并不总是一样。他们说“做个 dashboard”，是因为这像是该提出的要求，而不是因为 dashboard 真能解决问题。他们说“让它更快”，却没有给出要达到的数字。

发现这种差距最便宜的时刻，是在任何计划、spec 或代码存在之前。一旦开始构建，切换成本就是真实的，用户会把错误的东西合理化成“够好”的东西。不匹配会被锁死。

这个技能会在成本出现前弥合差距。其他 Define 阶段技能假设你已经大致知道自己想要什么：`idea-refine` 从一个想法生成变体，`spec-driven-development` 把需求写下来，`doubt-driven-development` 在你草拟计划后压力测试它。interview-me 位于这一切之前：一次只问一个问题，并附上你当前最佳猜测，直到你能在用户开口前预测他们会说什么。

## 何时使用

在这些情况下应用此技能：

- 请求至少缺少以下一项：用户是**谁**、他们**为什么**想要它、**成功**是什么样、绑定的**约束**是什么
- 请求是惯例化而非具体的（“build me X”、“make it faster”），并且不猜测就无法拆开这个惯例
- 你想从未暴露的假设开始
- 当两个合理价值发生张力（简单 vs. 灵活、成本 vs. 速度）时，用户没有说明自己在优化哪一个
- 用户显式调用：“interview me”、“grill me”、“before we start, are we sure?”、“stress-test my thinking”

**何时不使用：**

- 请求明确且自包含（“rename this variable”、“fix this typo”）
- 用户明确要求速度优先于验证
- 纯信息请求（“how does X work?”、“what does this code do?”）
- 机械操作（重命名、格式化、文件移动）
- 你已经有 ≥95% 信心；在假设自己没有之前，重读下面的停止条件

## 加载约束

这个技能需要实时、可响应的用户。**不要在非交互上下文中调用**，例如 CI pipelines、scheduled runs、`/loop` 或 autonomous-loop。如果你处于这些场景且请求描述不充分，请把它作为 blocker 提醒用户，而不是猜测。

## 流程

### 步骤 1：提出假设，并给出信心数字

在问任何问题前，用**一句话**写下你当前对用户想要什么的最佳理解，再给出诚实的信心数字（0-100%）：

```
HYPOTHESIS: You want a way to answer "how are we doing?" in standup, and "dashboard" was the convention that came to mind.
CONFIDENCE: ~30% — missing: who it's for, what "metrics" means in context, and what success looks like
```

数字会迫使诚实。如果你写了高数字，但实际上无法预测你接下来要问的三个问题中用户会如何反应，那这个数字就是错的。从你能 defend 的信心水平开始。

当信心低于约 70% 时，在同一行附上简短原因，说明仍未解决或缺失什么。这会准确告诉用户访谈需要挖出什么，并防止数字变成模糊信号。

### 步骤 2：一次问一个问题，每个问题都附上猜测

格式：

```
Q: <one focused question>
GUESS: <your hypothesis for the answer, with the reasoning that produced it>
```

等待用户回应后，再问下一个问题。

**为什么一次一个，而不是一批：**

- 如果你把假设埋在列表里，用户无法回应你的假设
- 批量问题鼓励略读和表面答案
- 第三个问题通常取决于第一个问题的答案；一次全问会锁定错误 framing
- 用户认真思考的能量有限，请一次花在一个问题上

**为什么要附上猜测：**

- 用户对错误猜测的反应，比从零生成答案更快
- 这让你承诺一个可能明显错误的假设，保持诚实
- 它暴露*你的*假设，而这正是访谈要揭示的东西

这里的风险是礼貌用户为了配合而同意你的猜测。通过明确表现出你愿意错来缓解，并偶尔朝你预计用户会反驳的方向猜测。

### 步骤 3：倾听“想要 vs. 应该想要”

最危险的答案，是用户说出一个听起来深思熟虑的答案，而不是他们真正想要的东西。注意这些信号：

- 答案在套最佳实践话术（“I want it to be scalable”、“clean architecture”），但没有具体内容
- 答案诉诸惯例（“the way most apps do it”、“the standard approach”）
- 诸如 “I should probably…”，“I think I'm supposed to…”，“good engineering practice says…” 的短语
- 把 buzzwords 当目标：当 “modern”、“scalable”、“robust” 是答案，而不是具体结果

听到这些时，要问的问题是：

> *“如果你不需要向任何人证明这个选择合理，你真正想要的是什么？”*

这个问题常常比前五个问题做更多工作。

### 步骤 4：用用户自己的话重述意图

当你的信心很高时，写回你现在认为用户想要什么。保持紧凑（5-8 行），尽量使用他们的语言，并组织成用户可以逐行确认或纠正的形式：

```
Here's what I now think you want:

- Outcome:      <one line>
- User:         <one line — who benefits>
- Why now:      <one line — what changed>
- Success:      <one line — how we know it worked>
- Constraint:   <one line — the binding limit>
- Out of scope: <one line — what we're explicitly not doing>

Yes / no / refine?
```

包含 “Out of scope” 是不可协商的。错位的一半来自对*不*构建什么的沉默分歧。

### 步骤 5：确认，必须是明确的 yes，而不是“你觉得好就行”

门槛是一个明确的 “yes”。以下都**不是** yes：

- “Whatever you think is best.” → 用户在委托，这意味着他们也没有 95% 信心。用两个具体选项作为选择重新询问。
- “Sounds good.” → 模糊。问：“Anything you'd refine?” 沉默不是确认。
- “Sure, let's go.” → 往往是礼貌退出，而不是认可。同样追问。
- 沉默后说 “okay let's start.” → 用户是放弃了访谈，而不是达成了收敛。停下并问你是否漏掉了什么。

如果他们纠正你，把纠正折进去并重新陈述。循环直到得到明确 yes。

### 95% 信心停止条件

当你能对这个问题回答 yes 时，就完成了：

> *我能预测用户对我接下来要问的三个问题的反应吗？*

如果可以，你们已有共同理解。停止访谈并产出重述。如果不行，你还没完成；问下一个问题。

这是一个可检查测试，不是感觉。它也有下限：如果你已经问了几轮，仍然无法预测，这说明请求本身有信息，不是继续硬问的理由。停下并告诉用户：“我已经问了 X 个问题，但仍无法预测你的反应。这里缺了某个基础信息。想退一步吗？”

## 输出

这个技能的输出是一个**已确认的意图陈述**：步骤 4 的重述，加上步骤 5 中用户明确的 yes。这就是交付物。Specs、计划和任务清单都在下游，它们消费这个技能产出的意图。

如果用户希望持久化意图（多会话项目、交接给其他协作者），可以提议保存到 `docs/intent/[topic].md`。只有在用户确认后才保存。

## 示例

一个简短的前后对比。

**没有 `interview-me`：**

```
User:  build me a dashboard for our metrics
Agent: <starts proposing chart libraries and layouts>
```

agent 已经假设了：这是为谁、哪些 metrics、“for our team” 是什么意思、成功是什么样。这些都没有被说明。每个假设都可能成为用户之后说“这不是我想要的”的地方。

**使用 `interview-me`：**

```
User: build me a dashboard for our metrics

Agent:
HYPOTHESIS: You want a way to answer "how are we doing?" in standup, and "dashboard" came to mind because that's the conventional answer.
CONFIDENCE: ~30% — missing: who it's for, what "metrics" are, and what success looks like

Q:     When you say "how are we doing?", who's asking — you alone, the engineering team in standup, or up the chain (e.g., your manager, an exec, a board)?
GUESS: engineering team in standup, because "we" usually scopes that way and standups are where this question gets asked. If it's for execs the metrics and the framing change a lot.

User: It's actually for me. I keep losing track of which experiments are running and what their early signal looks like.

Agent:
Updated read: not a team dashboard at all. It's a personal experiment tracker. Confidence: ~60% — still missing: what "early signal" means and what done looks like.

Q:     Is the gap that you don't know which experiments exist, or that you can't see their results in one place?
GUESS: the second one. You have a list of experiments somewhere, but their results live in five different tools, and reconciling them by hand is what you're tired of.

User: First one actually. I literally don't have a list. They're spread across different docs.

Agent: <continues>
```

两个问题内，agent 就发现真实请求不是“dashboard”。而是“list”。不同 artifact、不同范围、不同工作。dashboard 会是错的。

## 与其他技能的交互

- **`idea-refine`**：下游。如果确认后的意图是“我想要 X，但不知道如何限定范围”，交给 `idea-refine`，针对已明确意图生成变体。
- **`spec-driven-development`**：下游。如果确认后的意图很具体（“我想为 Y 用户实现 X，并以 Z 作为成功标准”），交给 `spec-driven-development` 写下来。
- **`planning-and-task-breakdown`**：在此技能之后两个环节（spec 之后）。
- **`doubt-driven-development`**：时间线的另一端。interview-me 是决策前的意图提取；doubt-driven 是决策后 artifact 审查。两者都捕捉偏离，但时机不同。
- **`source-driven-development`**：正交。interview-me 澄清用户想要什么；SDD 验证框架事实。它们不竞争。

## 常见合理化借口

| 合理化借口 | 现实 |
|---|---|
| “请求已经够清楚了” | 如果你现在写不出用户期望结果的一句话，请求就不够清楚。先运行步骤 1。 |
| “问太多问题浪费他们时间” | 4-6 个有针对性问题浪费的时间很少。构建错误东西浪费的时间巨大，而且成本由用户承担。 |
| “我会边做边弄明白” | 代码存在后的切换成本是现在的 10 倍。实现中的发现就是返工。 |
| “他们说‘你觉得好就行’，所以我应该决定” | “Whatever you think” 是委托，不是决定。用两个具体选项作为选择重新询问。 |
| “我应该给他们几个选项挑” | 选项适用于用户知道自己想要什么并在取舍间选择的情况。他们还不知道自己想要什么。列选项会扩大搜索；提问会收窄它。 |
| “附上我的猜测会引导他们” | 引导正是重点。回应比从零生成更快。风险是 sycophancy，不是引导；通过明显愿意出错来缓解。 |
| “我们聊够了，我懂了” | 测试它：你能预测他们对接下来三个问题的反应吗？如果不能，你还不懂。 |
| “用户说 yes，我们完成了” | 如果 yes 跟在模糊重述或开放式 “sounds good” 后面，这个 yes 是空的。具体重述并重新确认。 |

## 危险信号

- 单条消息里有三个或更多问题：那是批量询问，不是访谈
- 问题没有附上你的假设：那是调查，不是承诺
- 接受 “whatever you think is best” 作为终止答案
- 在用户明确确认重述前，产出 spec、计划或任务清单
- 问题被表述成“best practice 是什么？”而不是“你真正想要什么？”
- 用户给出 sophistication-signaling 答案（“scalable”、“clean”、“modern”），你没有探查这是否是他们真正想要的
- 三轮或更多后信心没有明显上升：你问错了问题，退一步重新 framing
- 信心数字低于约 70%，但没有附上原因：如果用户不知道缺什么，就无法帮你弥合差距
- 在用户确认前保存 intent 文档（文档本身暗示了用户没给出的 yes）
- 跳过 “Out of scope” 行（关于非目标的沉默分歧是错位的一半）

## 验证

应用 interview-me 后：

- [ ] 第一轮陈述了明确假设和信心数字
- [ ] 每个低于约 70% 的信心数字都附带一行原因（仍未解决或缺失什么）
- [ ] 问题一次一个，每个都附上 agent 的猜测
- [ ] 当用户给出 sophistication-signaling 或 convention-signaling 答案时，至少运行一次“如果不需要证明合理，你真正想要什么？”探查
- [ ] 向用户写回具体重述（Outcome / User / Why now / Success / Constraint / Out of scope）
- [ ] 用户用明确 yes 确认了重述（不是“whatever you think”，不是“sounds good”，不是沉默）
- [ ] 在停止点，agent 能预测用户对接下来三个问题的反应
- [ ] 任何到下游技能（`idea-refine`、`spec-driven-development`）的交接，都以确认后的意图为框架，而不是原始的欠说明请求
