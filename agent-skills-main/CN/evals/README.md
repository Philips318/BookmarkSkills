# Skill Evals

本 repo 如何衡量它的 skills 是否真的有效：它们应在该触发时**触发**，彼此之间保持**区分**，并以每个 skill 承诺的方式**改变 agent 行为**。

## 先前工作（以及我们采用了什么）

目前社区尚未形成评估 `SKILL.md` skills 的单一标准，但有两种方法领先：

- **Anthropic's skill-creator v2** 为每个 skill 定义一个 `evals.json`（prompt + `expectations[]`，从 transcript 评分），并针对 descriptions 与样例 prompts 做触发准确率测试。我们在 behavioral tier 中**逐字采用**其 [`evals.json` schema](https://github.com/anthropics/skills/tree/main/skills/skill-creator)，因此它的任何 tooling（`run_eval.py`、benchmark comparisons、eval viewer）都可不经修改地用于我们的 eval files。
- **Superpowers**（obra）使用 bash + `claude -p` + prompt fixtures 和 grader scripts 测试 skills。我们的 behavioral runner 遵循同样的 headless-`claude` 模式，评分 rubric 来自 `expectations[]`。

二者都没有提供一个针对多 skill *catalog* 的**确定性、CI 安全**检查：每个 skill 的 description 是否包含用户真实会说的词汇？两个 skills 的 descriptions 是否发生碰撞？这就是下面的 Tier 2，也是本 repo 的新增部分。

## 三个层级

| Tier | What it checks | Runs | Cost |
|---|---|---|---|
| 1. Structural | Frontmatter、命名、必需 sections、command parity | CI（`validate-skills.js`、`validate-commands.js`） | 免费 |
| 2. Trigger & routing | 正向 prompts 能让对应 skill 排进 top-k；负向 prompts 不会；任意两个 descriptions 不接近碰撞 | CI（`run-evals.js`） | 免费 |
| 3. Behavioral | 遵循该 skill 的 agent 满足其 `expectations[]` | 按需（`run-evals.js --behavioral`） | Tokens |

Tier 2 是 routing 的**词法近似**（基于 descriptions 的 stemmed TF-IDF）。它不能判断语义 — 那是 Tier 3 的工作 — 但它能捕获真实触发 bug 中最常见的两类失败：description 缺少用户实际会说的词汇（false negative），以及过宽 description 排在正确 skill 前面（false positive）。Tier-2 失败通常意味着*修 description*，不是修 eval。

## 运行

```bash
# Tier 2 — deterministic, runs in CI
node scripts/run-evals.js

# Tier 3 — behavioral, runs each eval through headless claude, then grades it
node scripts/run-evals.js --behavioral test-driven-development            # spends tokens
node scripts/run-evals.js --behavioral test-driven-development --dry-run  # prints the plan only
```

Tier 3 会在一次性 workspace 中运行每个 eval（来自 `files[]` 的 fixtures 会从 `evals/fixtures/` materialize 出来），捕获完整的 `--output-format stream-json --verbose` 执行 trace，并对**trace**（包括 tool calls）评分，而不是对模型最终 prose 评分，因此像“修复前先运行了失败测试”这样的 expectations 会根据实际发生的事判断，而不是根据叙述判断。executor 使用显式 permission mode（`--permission-mode acceptEdits` 加预批准 tool list），因此 agent 可以真正编辑文件并在 workspace 中运行命令，而不是被拒绝后只做叙述。trace 在 grader prompt 中被 fenced 为不可信数据，并通过 stdin 传给 grader（traces 可能有数 MB；argv 会撞到 OS 参数长度限制），executor 和 grader 调用都有 timeouts，grader output 在写入 `evals/results/`（gitignored）前会被验证为 JSON，格式为 skill-creator 的 `grading.json` shape。

没有 fixtures 的 behavioral evals 具有 provisional trust level：把结果视为 sanity checks，而不是证据。Graduation criteria 位于 [#352](https://github.com/addyosmani/agent-skills/issues/352)。

## Eval case format

每个 skill 一个文件：`evals/cases/<skill-name>.json`。

```json
{
  "skill_name": "test-driven-development",
  "trigger": {
    "positive": [
      { "prompt": "Write a failing test for this bug before fixing it", "top_k": 3 }
    ],
    "negative": [
      { "prompt": "Update the architecture diagram in the docs", "owner": "documentation-and-adrs" }
    ]
  },
  "evals": [
    {
      "id": 1,
      "prompt": "Fix the reported rounding bug in the invoice totals, test-first.",
      "expected_output": "A failing test demonstrating the bug, a minimal fix turning it green, full suite passing",
      "expectations": [
        "A failing test is written and shown failing before the fix",
        "The implementation is the minimum needed to pass",
        "The full suite is run after the fix to catch regressions"
      ],
      "trust_level": "provisional"
    }
  ]
}
```

- `evals[]` 完全是 skill-creator 的 schema（`id`、`prompt`、`expected_output`、可选 `files[]`、`expectations[]`）。Expectations 是 grader 根据 transcript 检查的可验证陈述 — 是行为，不是措辞。
- `trigger` 是本 repo 的扩展。`positive` prompts 是真实用户可能提出、且应路由到这里的请求（`top_k` 默认 3；对某 skill 的标志性请求可收紧到 1）。`negative` prompts 属于*另一个* skill；这个 skill 不应在它们上排名第一。能写时请在 `owner` 中声明那个 skill：runner 随后会断言 owner **排名高于**此 skill，把 negative 变成真正的 pairwise routing test，而不是一个在 prompt 什么都匹配不到时也可能空过的测试。
- `trust_level: "provisional"` 标记尚无 fixtures 的 behavioral eval；behavioral runner 会标记它们，其通过率不应被引用为证据（见 [#352](https://github.com/addyosmani/agent-skills/issues/352)）。

**编写好的 trigger prompts：** 改写用户真实说话方式；不要复制 description（那是在作弊 eval）。如果真实 prompt 无法排名，是因为 description 缺少对应词汇，那就是一个真实发现 — 改进 description。

## 添加 skill

每个 skill 都随附一个 eval file。当你添加 `skills/<name>/` 时，请添加 `evals/cases/<name>.json`，其中至少包含 3 个 positive triggers、2 个 negative triggers 和 1 个 behavioral eval；当文件低于这些 minimums 或完全缺失时，runner 会发出 warning。在过渡窗口期间，这两个检查都是 warning-level，并会通过 [#352](https://github.com/addyosmani/agent-skills/issues/352) 提升为 errors。

## 要关注的指标

Tier-2 运行会打印 **trigger rank-1 rate**（正向 prompts 将其 skill 排第一的比例，而不仅仅是 top-k）。它尚未 gate；待 baseline 稳定后，计划引入 `--min-rank1` CI ratchet（[#352](https://github.com/addyosmani/agent-skills/issues/352)）。数字下降意味着 descriptions 正在相互漂移靠近。collision check 会在 pairwise description similarity ≥75% 时报错，在 ≥50% 时警告。这些 evals 暴露出的已知 description-vocabulary gaps 记录在 [#351](https://github.com/addyosmani/agent-skills/issues/351)。
