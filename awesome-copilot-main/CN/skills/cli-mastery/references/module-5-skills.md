模块5：技能系统

什么是技能？

- AI可以调用的专门功能包
-将它们视为具有特定领域知识的“专家模式”
—通过`/skills`命令管理

##技能位置

|级别|位置||-------|----------|
|用户|`~/.copilot/skills/<name>/SKILL.md`或`~/.agents/skills/<name>/SKILL.md`|
|回购|`.github/skills/<name>/SKILL.md`|
| Org |通过Org -level config |共享

创建自定义技能

1. 创建目录：`mkdir -p ~/.copilot/skills/my-skill/`（或`mkdir -p ~/.agents/skills/my-skill/`）
2. 用YAML字体创建`SKILL.md`（`name`,`description`，可选`tools`）
3. 为AI的行为编写详细的指令
4. 用`/skills`验证

技能设计最佳实践

- **清晰的描述** -帮助AI自动匹配你的技能任务
- **聚焦范围** -每个技能应该做好一件事
- **包括说明** -具体说明技能应该如何操作
- **测试彻底** -使用`/skills`验证，然后调用和检查结果

# # Auto-matching

当你描述一项任务时，AI会检查是否有匹配的技能，并建议使用它。