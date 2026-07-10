---
description: "通过预先测试过的 Export-Qms.py 脚本（pandoc + PDLM templates）将现行 QMS markdown 文档转换为 Word (.docx)。在文档准备好进入正式评审时运行。"
mode: "agent"
---

> **仅手动使用的 prompt。** 这不是运行时 harness 循环的一部分。运行时 agents 会直接维护 markdown QMS 文件；Word 导出是单独由人工触发的步骤，用于正式评审提交。不要在自动化编排期间调用此 prompt。

# 将 QMS Markdown 转换为 Word

所有转换机制都位于预先测试过的脚本 `.github/scripts/Export-Qms.py` 中（filename→template 映射、严格 Mermaid 预渲染、pandoc 调用、封面页合并、失败处理）。批处理包装器 `.github/scripts/Export-Qms.bat` 会使用仓库 venv Python 自动调用它。不要手写 pandoc 命令 — 始终通过批处理包装器调用脚本。

## 步骤 1 — 确定要转换哪些文档

如果用户没有明确命名要转换的文档，**先使用 ask-questions 工具询问，再做任何事**。不要假设“全部”，也不要根据最近编辑猜测。

将六个 QMS 文档作为多选选项提供：
- `docs/qms/SwRS.md`
- `docs/qms/SSDS.md`
- `docs/qms/SDD.md`
- `docs/qms/MVP.md`
- `docs/qms/MVProcedure.md`
- `docs/qms/MVReport.md`

只有在用户至少选择一个文档后才继续。

## 步骤 2 — 前置条件

如果缺少任何工具，`Export-Qms.py` 会快速失败：
- PATH 上的 `pandoc` — markdown→docx 引擎
- PATH 上的 `mmdc` (mermaid-cli) — Mermaid 预渲染
- 仓库 venv (`venv\`) 中的 `python-docx` — 封面页合并

要从零开始一次性安装/验证**所有**这些内容，请运行 bootstrap 脚本（幂等 — 可以安全重复运行）。在空机器上，它还会通过 winget 安装 Python 和 Node.js/npm：

```bat
.github\scripts\Setup-QmsEnv.bat
```

如果导出随后因缺少工具而快速失败，请先运行上面的 setup 脚本，然后**重新打开终端**以刷新 PATH，再重试导出。setup 脚本无法自举 winget 本身 — 如果 winget 不存在，它会打印指导并退出。不要用临时的 `pip`/`winget` 命令手动安装；始终使用 setup 脚本，确保 python-docx 安装到导出器查找的仓库 venv 中。

Mermaid 处理按设计是**严格的**：每个图都会预渲染为 PNG；如果任何单个图失败或超时，整个运行都会失败 — QMS 文档必须完整，不能部分完成。

## 步骤 3 — 运行导出

将用户选择的文件传给批处理包装器（从仓库根目录运行）：

```bat
.github\scripts\Export-Qms.bat docs/qms/SDD.md docs/qms/MVReport.md
```

可选参数：
- `--output-dir DIR`（默认：与每个源 `.md` 文件相同的文件夹）
- `--templates-dir DIR`（默认 `docs/qms-templates`）
- `--mermaid-timeout SEC`（默认 `600`；对于很大或复杂的图可以调高）

## 步骤 4 — 报告

报告转换了哪些文档及其输出路径（脚本会打印）。如果脚本抛错，将确切错误呈现给用户 — 不要盲目重试，也不要退回到手写 pandoc 命令。
