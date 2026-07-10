# 为 Agent Skills 做贡献

感谢你有兴趣贡献！本项目是面向 AI 编码代理的生产级工程技能集合。

## 添加新技能

### 提出新技能之前

这个包已经覆盖了开发生命周期的大部分内容，许多提案会与现有技能或其他开放 PR 重叠。打开 PR 之前，请先完成这些检查，避免评审者处理重复项：

1. **搜索目录。** 浏览 [README 中的技能列表](README.md)，并粗略查看 `skills/`，确认是否已有技能完整或部分覆盖你的想法。
2. **检查开放 PR。** 运行 `gh pr list --state open`（或浏览 PR 标签页），查找相同主题的提案。近似重复技能已经存在一些聚类；不要继续增加。
3. **阅读结构说明。** 确认你的想法符合 [docs/skill-anatomy.md](docs/skill-anatomy.md) 中的格式：它应是带验证的可执行工作流，而不是模糊建议。
4. **在 PR 描述中说明缺口。** 明确说明为什么现有技能或开放 PR 没有覆盖它。如果存在重叠，请提议扩展现有技能，而不是添加新技能。

如果你的想法是对现有技能的改进，请优先对该技能做聚焦编辑，而不是创建新目录。

### 创建技能

1. 在 `skills/` 下创建一个 kebab-case 名称的目录
2. 添加遵循 [docs/skill-anatomy.md](docs/skill-anatomy.md) 格式的 `SKILL.md`
3. 包含带有 `name` 和 `description` 字段的 YAML frontmatter
4. 确保 `description` 先以第三人称说明技能做什么，然后包含一个或多个 `Use when` 触发条件

### 技能质量标准

技能应当是：

- **具体的** — 可执行步骤，而不是模糊建议
- **可验证的** — 带有证据要求的清晰退出标准
- **实战检验过的** — 基于真实工程工作流，而不是理论理想
- **最小的** — 只包含正确引导代理所需的内容

### 结构

每个新技能必须有：

- 技能目录中的 `SKILL.md`
- 带有有效 `name` 和 `description` 的 YAML frontmatter
- `evals/cases/<skill-name>.json` 中的 eval case 文件 — 至少 3 个正向触发、2 个负向触发（尽可能带 `owner`），以及 1 个行为 eval（参见 [evals/README.md](evals/README.md)；在通过 [#352](https://github.com/addyosmani/agent-skills/issues/352) 提升之前为 warning-level）

新技能通常应遵循标准结构：

- **Overview** — 此技能做什么以及为什么重要
- **When to Use** — 触发条件
- **Process** — 分步工作流
- **Common Rationalizations** — 代理用来跳过步骤的借口及其反驳
- **Red Flags** — 表明技能被错误应用的警示信号
- **Verification** — 如何确认技能已正确应用

以上 frontmatter 字段是必需的。章节结构是推荐模式：只要保留相同意图并让技能易于遵循，使用 `How It Works`、`Workflow` 或 `Core Process` 等等效标题也可以。

### 不要做什么

- 不要在技能之间重复内容 — 改为引用其他技能
- 不要添加只是模糊建议而非可执行流程的技能
- 除非内容超过 100 行，否则不要创建支持文件
- 不要为了匹配其他技能而创建空的 `scripts/` 目录 — 只有当技能包含可运行辅助程序时才添加 `scripts/`
- 不要把参考资料放在技能目录内 — 请使用 `references/`

## 修改现有技能

- 保持改动聚焦且最小
- 保留现有结构和语气
- 编辑后测试 YAML frontmatter 仍然有效

## 测试 Hooks

session-start hook（`hooks/session-start.sh`）会将 `using-agent-skills` meta-skill 注入到每个新的 Claude Code 会话中。`hooks/session-start-test.sh` 中的回归测试会验证该 hook 的 JSON payload — 无论 `jq` 是否可用。

在打开任何触及以下内容的 PR 之前运行它：

- `hooks/session-start.sh`
- `skills/using-agent-skills/SKILL.md`（由 hook 嵌入的 meta-skill 内容）

```bash
bash hooks/session-start-test.sh
```

预期输出：`session-start JSON payload OK`。脚本会在任何断言失败时以非零状态退出。

### 复现 no-jq fallback

当 `jq` 不在 `PATH` 上时，hook 会优雅降级为 `INFO` 优先级 payload。要在本地测试该分支，请在测试调用时从 `PATH` 中移除 `jq` 所在目录：

```bash
JQ_DIR=$(dirname "$(command -v jq)")
PATH=$(echo "$PATH" | tr ':' '\n' | grep -v "^${JQ_DIR}$" | tr '\n' ':' | sed 's/:$//') \
  bash hooks/session-start-test.sh
```

当 `jq` 位于自己的目录中时（例如 Homebrew 的 `/opt/homebrew/bin`，或手动安装的 `/usr/local/bin`），这会顺利工作。如果你的 `jq` 与测试依赖的其他工具共享系统 bin（例如 `/usr/bin` 中的 `mktemp`），更简单的方法是通过单独的包管理器安装 `jq`，使它拥有自己的 bin 目录，然后重新运行。

hook 的 `command -v jq` 检查会在剥离后的 `PATH` 下失败，`INFO` 优先级 fallback 会运行，测试会断言 `jq is required` 指导消息，而不是正常 payload。

## 报告问题

如果发现以下问题，请打开 issue：

- 某个技能给出错误或过时的指导
- 缺少对常见工程工作流的覆盖
- 技能之间存在不一致

## 许可证

贡献即表示你同意你的贡献按 MIT License 授权。