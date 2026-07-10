# 在 Codex 中使用 agent-skills

此仓库也是一个 [Codex plugin](https://developers.openai.com/codex/plugins/build)。Claude Code 使用的同一个根级 `skills/` 目录会被 Codex 使用，因此不会复制或重复文件。

## 安装（一个命令）

```bash
codex plugin marketplace add addyosmani/agent-skills
```

> 需要 Codex CLI v0.122 或更高版本。在更早的版本中，命令是 `codex marketplace add`。见 [Codex CLI docs](https://developers.openai.com/codex/cli)。

Codex 会把仓库 clone 到 `~/.codex/plugins/agent-skills/`，在 `~/.codex/config.toml` 中注册 marketplace，并让 plugin 可用。如果 Codex 已经在运行，请重启 Codex。

本地 clone 也可用：

```bash
codex plugin marketplace add /path/to/your/clone
```

## 使用

安装后，在 Codex chat 中用 `@` 调用 skill（例如 `@spec-driven-development`），或者只描述任务，让 Codex 选择正确的 skill。`skills/` 下的全部 24 个 skills 都可用。

## 工作原理

- `.codex-plugin/plugin.json` — 位于仓库根目录的 Codex plugin manifest。它把 `skills` 指向 `./skills/`，并声明一个空的 Codex hook config，因此 Codex 不会自动加载 `hooks/hooks.json` 中面向 Claude 的 hooks。
- `.agents/plugins/marketplace.json` — marketplace 条目，声明仓库根目录（`./`）是 plugin source。
- `skills/<name>/SKILL.md` — 保持不变。Codex 和 Claude Code 共享相同的 `name` + `description` frontmatter 格式，因此同一个文件服务两个平台。

`.claude/commands/` 中的 slash commands 和 `agents/` 中的 personas 仍然是 Claude Code 专用的 — Codex 没有二者的原生等价物。在 Codex 上，请直接调用底层 skill，而不是 slash command（例如用 `@spec-driven-development`，而不是 `/spec`）。
