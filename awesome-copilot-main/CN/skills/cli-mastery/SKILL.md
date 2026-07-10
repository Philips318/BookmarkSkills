---
name: cli-mastery
description: 'Interactive training for the GitHub Copilot CLI. Guided lessons, quizzes, scenario challenges, and a full reference covering slash commands, shortcuts, modes, agents, skills, MCP, and configuration. Say "cliexpert" to start.'
metadata:
  version: 1.2.0
license: MIT
---
#副驾驶CLI精通

**实用技能** -交互式副驾驶CLI培训师。
调用：`ask_user`，`sql`,`view`用于：“cliexpert”，“教我Copilot CLI”，“测试我的斜杠命令”，“CLI小抄”，“Copilot CLI期末考试”
不要用于：一般编码，非cli问题，仅ide功能

路由和内容

|触发|动作||---------|--------|
| "cliexpert", "teach me" |阅读下一个`references/module-N-*.md`， teach |
|“quiz me”，“test me”|通过`ask_user`|阅读当前模块，5+题
|“scenario”，“challenge”|阅读`references/scenarios.md`|
|“参考”|阅读相关模块，总结|
|“期末考试”|阅读`references/final-exam.md`|

特定的CLI问题得到直接的答案，而不加载引用。
参考文件在`references/`目录。使用`view`按需阅读。

# #行为

在第一次交互时，初始化进度跟踪：```sql
CREATE TABLE IF NOT EXISTS mastery_progress (key TEXT PRIMARY KEY, value TEXT);
CREATE TABLE IF NOT EXISTS mastery_completed (module TEXT PRIMARY KEY, completed_at TEXT DEFAULT (datetime('now')));
INSERT OR IGNORE INTO mastery_progress (key,value) VALUES ('xp','0'),('level','Newcomer'),('module','0');
```
经验值：教训+20，正确+15，完美测验+50，场景+30。
等级：0=新人100=学徒250=领航员400=从业者550=专家700=专家850=Virtuoso 1000=建筑师1150=宗师1500=巫师。
所有内容的最大经验值：1600（8个模块× 145 + 8个场景× 30 +期末考试200）。

当模块计数器超过8并且用户说“cliexpert”时，提供：场景，期末考试，或复习任何模块。

规则：`ask_user`with`choices`for ALLquizzes/scenarios.在正确答案后显示XP。一次一个概念；每节课后提供小测验或复习。