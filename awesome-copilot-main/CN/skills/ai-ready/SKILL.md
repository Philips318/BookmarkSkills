---
name: ai-ready
description: 'Make any repo AI-ready — analyzes your codebase and generates AGENTS.md, copilot-instructions.md, CI workflows, issue templates, and more. Mines your PR review patterns and creates files customized to your stack. USE THIS SKILL when the user asks to "make this repo ai-ready", "set up AI config", or "prepare this repo for AI contributions".'
---
# AI就绪

该技能帮助用户安装由[John Papa]（https://github.com/johnpapa）提供的最新[ai-ready]（https://github.com/johnpapa/ai-ready）技能。

*为什么?*：完整的ai准备技能是大约600行详细的指令，经常进化。这个包装使它在这里可以被发现，而真相的来源仍然在[johnpapa/ai-ready]（https://github.com/johnpapa/ai-ready）中——总是最新的。

# #的步骤

1. 告诉用户通过在Copilot CLI中运行这个命令来添加技能：   ```
   /skills add johnpapa/ai-ready
   ```
这将下载最新版本的技能到他们的个人技能目录。重新运行命令将更新到最新版本。

2. 提醒用户在加载技能前查看技能。他们可以用：   ```bash
   head -20 ~/.copilot/skills/ai-ready/SKILL.md
   ```
3. 在用户确认他们已经检查并安装了它之后，告诉他们用`/skills reload`重新加载技能，然后说`make this repo ai-ready`。
4. **不要**代表用户执行该命令。用户必须自己运行它。