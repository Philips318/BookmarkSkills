# simplify-ignore hook

`/code-simplify` 的块级保护。标记永远不应被简化的代码 — 模型不会看到它。

## 设置

1. 注释你想保护的 blocks：

```js
/* simplify-ignore-start: perf-critical */
// manually unrolled XOR — 3x faster than a loop
result[0] = buf[0] ^ key[0];
result[1] = buf[1] ^ key[1];
result[2] = buf[2] ^ key[2];
result[3] = buf[3] ^ key[3];
/* simplify-ignore-end */
```

2. 将 hooks 添加到 `.claude/settings.json`：

```json
{
  "hooks": {
    "PreToolUse": [
      {
        "matcher": "Read",
        "hooks": [{ "type": "command", "command": "bash ${CLAUDE_PROJECT_DIR}/hooks/simplify-ignore.sh" }]
      }
    ],
    "PostToolUse": [
      {
        "matcher": "Edit|Write",
        "hooks": [{ "type": "command", "command": "bash ${CLAUDE_PROJECT_DIR}/hooks/simplify-ignore.sh" }]
      }
    ],
    "Stop": [
      {
        "hooks": [{ "type": "command", "command": "bash ${CLAUDE_PROJECT_DIR}/hooks/simplify-ignore.sh" }]
      }
    ]
  }
}
```

3. 运行 `/code-simplify` — protected blocks 会变成 `/* BLOCK_de115a1d: perf-critical */` placeholders。模型可以围绕周边代码推理，而不会看到受保护的实现。

> **Note:** 此 hook 会把临时 backups 存放在 `.claude/.simplify-ignore-cache/`。请确保该路径位于你的 `.gitignore` 中。

## 工作原理

一个 script，三个 hook events：

| Event | Action |
|---|---|
| `PreToolUse Read` | 备份文件，将 blocks 原地替换为 `BLOCK_<hash>` placeholders |
| `PostToolUse Edit\|Write` | 将 placeholders 展开回真实代码，保存模型的更改，再重新 filter |
| `Stop` | 会话结束时从 backup 恢复所有文件 |

每个 block 都按内容生成 hash（通过 `shasum`/`sha1sum` 生成 8 位 hex），因此即使模型复制或重排 placeholders，round-trip 也不会产生歧义。Cache 按项目隔离，以防跨会话干扰。

## Annotation syntax

```js
/* simplify-ignore-start */           // basic — hides the block
/* simplify-ignore-start: reason */   // with reason — appears in placeholder
/* simplify-ignore-end */
```

任何 comment style 都可以（`//`、`/*`、`#`、`<!--`）。支持每个文件多个 blocks，也支持单行 blocks。Placeholders 会保留原始 comment syntax（例如 Python 中的 `# BLOCK_xxx`，HTML 中的 `<!-- BLOCK_xxx -->`）。

## Crash recovery

如果 Claude Code 崩溃且没有触发 Stop hook，磁盘上的文件可能仍包含 `BLOCK_<hash>` placeholders。要手动恢复：

```bash
echo '{}' | bash hooks/simplify-ignore.sh
```

Backups 存放在项目目录内的 `.claude/.simplify-ignore-cache/` 中。

## 已知限制

- **单行 blocks 会隐藏整行。** 如果 `simplify-ignore-start` 和 `simplify-ignore-end` 与其他代码出现在同一行，整行都会对模型隐藏，而不仅是被注释的部分。请使用独立行放置 annotations。
- **Comment suffix detection 只覆盖 `*/` 和 `-->`。** 带非标准 comment closers 的模板引擎（ERB `%>`、Blade `--}}`）可能产生不平衡 placeholders。请改用 `#` 或 `//` 风格 comments。
- **Fallback expansion 是渐进式的，不是精确匹配。** 如果模型改变 placeholder 的格式（例如修改 reason text），hook 会尝试逐步更简单的匹配：full placeholder → prefix+hash+suffix → hash-only。hash-only fallback 可能留下外观碎片（例如多余的 `:` 或 reason text）。发生这种情况时会向 stderr 打印 warning。
- **文件重命名会留下 placeholders。** 如果模型通过 shell command 重命名或移动文件，新文件会保留 `BLOCK_<hash>` placeholders。会话停止时，原始代码会保存为 `<old-filename>.recovered`。你必须手动将 recovered code 还原到新文件中。

## Requirements

- `jq`、`shasum` or `sha1sum`（自动检测）、Bash 3.2+
