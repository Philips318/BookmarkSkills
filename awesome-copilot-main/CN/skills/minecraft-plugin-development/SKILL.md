---
name: minecraft-plugin-development
description: 'Use this skill when building or modifying Minecraft server plugins for Paper, Spigot, or Bukkit, including plugin.yml setup, commands, listeners, schedulers, player state, team or arena systems, persistent progression, economy or profile data, configuration files, Adventure text, and version-safe API usage. Trigger for requests like "build a Minecraft plugin", "add a Paper command", "fix a Bukkit listener", "create plugin.yml", "implement a minigame mechanic", "add a perk or quest system", or "debug server plugin behavior".'
---
# Minecraft插件开发

将此技能用于Paper， Spigot和Bukkit生态系统中的Minecraft服务器插件工作。

这一技能对于游戏玩法重的插件尤其有用，如战斗系统、波浪或boss遭遇、战争或团队模式、竞技场、装备系统、基于冷却时间的能力、计分板和配置驱动的游戏规则。

对于从真正的Paper插件中绘制的基础实现模式，根据需要加载这些引用：- [`references/project-patterns.md`]（references/project-patterns.md）用于在真实游戏插件中看到的高级架构模式
- [`references/bootstrap-registration.md`]（references/bootstrap-registration.md）用于`onEnable`、命令连接、侦听器注册和关机预期
- [`references/state-sessions-and-phases.md`]（references/state-sessions-and-phases.md）用于玩家会话建模，游戏阶段，比赛状态和重新连接安全逻辑
- [`references/config-data-and-async.md`]（references/config-data-and-async.md）用于配置管理器，数据库支持的球员数据，异步刷新和UI刷新任务
- [`references/maps-heroes-and-feature-modules.md`]（references/maps-heroes-and-feature-modules.md）用于地图旋转，英雄或职业系统，以及模块化功能增长
- [`references/minigame-instance-flow.md`]（references/minigame-instance-flow.md）用于竞技场实例，倒计时，战利品刷新，波系统，可见性隔离和实体到游戏的所有权
- [`references/persistent-progression-and-events.md`]（references/persistent-progression-and-events.md）用于长期运行的PvP服务器，具有个人资料，津贴，buff，任务，经济，自定义域事件和扩展注册表
- [`references/build-test-and-runtime-validation.md`]（references/build-test-and-runtime-validation.md）用于Maven或Gradle打包，阴影依赖，生成的资源，软依赖，配置验证命令，以及第一轮服务器测试计划# #范围

-范围：Paper， Spigot， Bukkit插件开发
-范围：`plugin.yml`，命令，标签完成，监听器，调度程序，配置，权限，冒险文本，玩家状态，小游戏流程，竞技场实例，地图副本，战利品，波浪，持久配置文件，津贴，buff，任务，经济，和PvP/PvE游戏循环
-范围：基于java的服务器插件架构、调试、重构和特性实现
-默认超出范围：织物模型，锻造模型，客户端模型，基岩附加组件

如果用户说“Minecraft插件”，但堆栈不清楚，首先确定该项目是Paper/Spigot/Bukkit还是一个mod堆栈。

默认工作方式

当此技能触发时：1. 确定服务器API和版本目标。
2. 确定构建系统和Java版本。
3. 检查`plugin.yml`，主插件类，以及命令或侦听器注册。
4. 在编辑代码之前绘制游戏流程：
-球员生命周期
-游戏阶段
-计时器和计划任务
-球队、竞技场或比赛状态
-配置和持久化
5. 做最小的一致更改，保持注册、配置和运行时行为一致。

如果插件是重玩法或有状态的，请在编辑前阅读[`references/project-patterns.md`]（references/project-patterns.md）和[`references/state-sessions-and-phases.md`]（references/state-sessions-and-phases.md）。

如果任务涉及竞技场隔离，地图实例，箱子或资源填充，波浪生成，路线投票，观众可见度或游戏特定聊天，也请阅读[`references/minigame-instance-flow.md`]（references/minigame-instance-flow.md）。如果任务涉及持续的玩家进程，个人资料保存，经济奖励，津贴，buff，任务，自定义战斗事件，或长期运行的共享PvP服务器，也请阅读[`references/persistent-progression-and-events.md`]（references/persistent-progression-and-events.md）。

如果任务涉及构建文件、`plugin.yml`元数据、阴影依赖项、生成的资源输出、部署到测试服务器、可选插件集成或发布验证，也读取[`references/build-test-and-runtime-validation.md`]（references/build-test-and-runtime-validation.md）。

##项目发现清单

出现时先检查以下内容：- `plugin.yml`
—`pom.xml`、`build.gradle`、`build.gradle.kts`插件主类扩展`JavaPlugin`-命令执行器和TAB补全器
-监听器类
-为`config.yml`，消息，套件，竞技场或自定义YAML文件配置引导代码
—生成资源输出，如`target/classes`，`build/resources`，或复制插件jar
-通过Bukkit scheduler api使用scheduler
-任何玩家数据、团队状态、竞技场状态或比赛状态容器

核心规则

###首选repo中的具体服务器API

-如果项目已经针对Paper api，继续使用Paper-first api，而不是降级到通用Bukkit，除非明确要求兼容性。
-不要假设一个API存在于所有版本。首先检查现有的依赖项和周围的代码样式。

保持注册同步

当添加命令、权限或监听器时，在相同的更改中更新相关的注册点：- `plugin.yml`
-插件启动注册在`onEnable`-代码中的任何权限检查
-任何相关的配置或消息键

尊重主线程边界

-除非API明确允许，否则不要从异步任务中触摸世界状态，实体，库存，计分板或大多数Bukkit API对象。
-在外部I/O，繁重的计算或数据库工作中使用异步任务，然后在应用游戏玩法更改之前切换回主线程。

将游戏玩法作为状态模型，而不是分散的布尔值

对于玩法插件，更喜欢显式状态对象而不是重复的标志：

-比赛或游戏阶段
-玩家角色或类别
-冷却状态
-团队成员
-场地分配
-活的、淘汰的、观察的或排队的状态

当该功能影响到重匹配的迷你游戏或持续的打斗玩法时，在修补症状之前先寻找隐藏状态转换。对于多竞技场插件，隔离每场比赛可见性，聊天收件人，计分板，战利品和实体所有权。不要让一个竞技场意外地观察或改变另一个竞技场。

###支持配置驱动值

当功能包含伤害、冷却时间、奖励、持续时间、信息、地图设置或切换时：

-优先选择配置支持的值而不是硬编码
-提供合理的默认值
-保持关键字名称稳定和可读
-验证或清理缺失的值

###小心重载行为

避免承诺安全热重载，除非代码已经很好地支持它。
-在配置重新加载时，确保内存缓存，计划任务和游戏状态被一致地处理。

##实现模式

# # #命令

对于新命令：-添加命令到`plugin.yml`-在需要时实现执行器和TAB完成
-在转换到`Player`之前验证发送方类型
-独立的解析，权限检查和游戏逻辑
-对无效的使用发送明确的玩家反馈

最小配准形状：```yaml
commands:
  arena:
    description: Join or leave an arena
    usage: /arena <join|leave>
```

```java
@Override
public void onEnable() {
    ArenaCommand command = new ArenaCommand(gameService);
    PluginCommand arena = getCommand("arena");
    if (arena != null) {
        arena.setExecutor(command);
        arena.setTabCompleter(command);
    }
}
```
# # #的听众

对于事件侦听器：

——早防早归
-检查当前的玩家，竞技场或游戏阶段是否应该处理事件
-避免在热点事件中做昂贵的工作，如移动，损坏或互动垃圾邮件
-在可行的情况下集中重复检查

###计划任务

对于计时器，回合，倒计时，冷却时间或定期检查：

-存储任务处理时的取消事项
在插件禁用和比赛或竞技场结束时取消任务
避免为了相同的玩法而出现多个重叠的任务，除非有明确的意图
-比起许多松散协调的重复任务，更喜欢一个权威的游戏循环
-确保倒计时或补充任务自取消当游戏离开预期的状态

主线程切换形状：```java
Bukkit.getScheduler().runTaskAsynchronously(plugin, () -> {
    PlayerData data = repository.load(playerId);
    Bukkit.getScheduler().runTask(plugin, () -> {
        Player player = Bukkit.getPlayer(playerId);
        if (player != null && player.isOnline()) {
            scoreboard.update(player, data);
        }
    });
});
```
玩家和比赛状态

对于每个玩家或每场比赛状态：

-清楚界定所有权
-清理退出，踢，死亡，比赛结束，插件禁用
-避免由`Player`键控的过时映射造成内存泄漏
-首选`UUID`作为持久跟踪，除非一个实时的玩家对象是严格需要的

文本和消息

当项目使用Adventure或MiniMessage时：

-遵循现有的格式化方法
避免毫无理由地混合传统颜色代码和冒险风格
-当消息是面向游戏时，保持消息模板可配置

##高风险地区

编辑时要格外注意：-伤害处理和自定义战斗逻辑
-死亡，重生，观众和消除流
-竞技场加入和离开流
-计分板或老板栏更新
-库存突变和试剂盒分发
—异步访问数据库或文件
-经济，任务，perk和配置文件突变
-自定义事件调度或扩展注册表
-版本敏感API调用
-关闭和清理在`onDisable`-跨竞技场可见性，聊天和广播隔离
-映射复制、卸载和文件夹删除逻辑
-怪物、NPC、投射物或临时实体所有权
-箱子或资源补充系统

##预期输出

在实现或修改插件代码时：-产生可运行的Java代码，而不是伪代码，除非用户只要求设计
-提及`plugin.yml`、配置文件、构建文件或资源所需的任何更新
-明确地调用版本假设
-指出存在的线程安全或api兼容性风险
-保留项目现有的约定和文件夹结构

当请求的更改涉及插件启动、异步数据、匹配流、类系统或旋转映射时，请在编辑前查阅匹配的参考文件。

验证检查表

在完成之前，尽可能多地验证这些任务：—命令、监听器或特性注册正确
—`plugin.yml`匹配实现行为
-导入和API类型匹配目标服务器堆栈
-调度程序的使用是安全的
-代码中引用的config键存在或有默认值
-状态清理路径存在于比赛结束，球员退出，和插件禁用
-每个竞技场聊天，可见性，记分牌和广播是隔离的
-临时世界，生物，任务和生成的资源被清理
-没有明显的null、cast或生命周期危害

##常见陷阱—不检查将`CommandSender`转换为`Player`-从异步任务更新Bukkit状态
-忘记在`plugin.yml`中注册监听器或声明命令
-当`UUID`更安全时，使用`Player`对象作为长寿命的映射键
-在一个回合，竞技场或插件关闭后留下重复的任务
-硬编码的游戏常数应该存在于配置
-假设在spigot目标插件中只有纸质api
-即使有状态插件经常在重新加载时中断，也将重新加载视为免费
-广播，显示球员，或应用计分板的变化在不相关的游戏实例
-在块可用之前加载或改变chest/container块
忘记从游戏中注销生成的生物或临时实体
—编辑`target/classes`或`build/resources`下的生成文件，而不是`src/main/resources`下的源文件

##首选响应形状对于实质性的请求，结构是这样的：

1. 当前插件上下文和假设
2. 玩法或生命周期影响
3. 代码更改
4. 需要注册或配置更新
5. 验证和剩余风险

对于小请求，保持答案简洁，但仍然提到任何需要的`plugin.yml`、配置或生命周期更新。