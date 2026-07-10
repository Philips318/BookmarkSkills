#特工街机画布

一个GitHub Copilot画布，在侧面面板打开一个复古的街机。它服务于Agent Arcade Phaser前端，允许用户或代理在5个迷你游戏之间切换。

# #游戏

- **外星人猛攻** -太空入侵者风格的街机行动与前进的外星人，盾牌，和神秘的船。
- **宇宙岩石** -小行星风格的矢量射手推力物理和分裂小行星。
-银河爆破** -加拉加风格的空间射击与阵型敌人，攻击模式，和双枪电源。
- **忍者跑者** -经典的平台游戏与双重跳跃，能量，翘曲管道，和敌人。
- **星球守护者** -防守风格的横向卷轴射击与人形救援和六种敌人类型。

# #文件-`extension.mjs`-画布声明，环回游戏服务器，静态资产处理，和代理动作。
-`game/`-编译相位游戏前端内服务的画布。
-`assets/`-游戏精灵，声音，应用程序图标，和`preview.png`扩展库。
-`package.json`-声明Copilot SDK依赖和ESM入口点。`copilot-extension.json`-副驾驶扩展name/version元数据。
-`canvas.json`-很棒的副驾驶画廊元数据。

# #先决条件

- **Node.js20.19或更新**因为副驾驶SDK需要`node ^20.19.0 || >=22.12.0`。
-GitHub Copilot应用画布/ ui扩展实验启用。

# #安装

对于用户范围，将此文件夹放到`~/.copilot/extensions/arcade-canvas/`处，对于项目范围，将此文件夹放到`.github/extensions/arcade-canvas/`处的存储库中。然后从复制的文件夹中安装依赖项：```sh
# User scope
cd ~/.copilot/extensions/arcade-canvas

# Or project scope, from the repository root
cd .github/extensions/arcade-canvas

npm install
```
在GitHub Copilot应用程序中重新加载扩展，然后打开`arcade-canvas`画布。画布接受一个可选的`defaultGame`输入，它带有以下键之一：`cosmic-rocks`、`alien-onslaught`、`galaxy-blaster`、`ninja-runner`或`defender`。

代理操作

-`list_games`-列出可用的小游戏和当前选择的游戏。
-`select_game { gameKey }`-将打开的街机画布切换到特定的小游戏。
-`restart_game`-重新加载打开的街机画布重新开始当前游戏。

# #发展

在Agent Arcade存储库中，在前端或资产更改后重新构建提交的画布包：```sh
npm run build:canvas
```
该命令构建前端，将`dist/game`复制到`game/`，将`dist/assets`复制到`assets/`，为Awesome Copilot画廊编写`assets/preview.png`，并将`assets/canvas-background.webp`捆绑到仅用于画布的空间背景中。

# #学分-精灵资产：JuhoSprite的[Simple Platformer 16]（https://juhosprite.itch.io/simple-platformer-16）。
-太空射击游戏资产：[太空射击游戏]（https://opengameart.org/content/space-shooter-redux）由kennedy .nl。
- galaga风格的游戏机制：[WesleyEdwards/galaga]（https://github.com/WesleyEdwards/galaga）
-小行星风格的游戏机制：[phaser3-typescript]（https://github.com/digitsensitive/phaser3-typescript）由数字敏感。
-防守风格的游戏机制和音效：[OpenDefender]（https://github.com/mkinney/Opendefender）由mkinney。
-复古游戏音效：[“复古游戏音效”]（https://opengameart.org/content/retro-game-sound-effects）由Vircon32 (Carra)，发布在OpenGameArt下[CC-BY 4.0]（https://creativecommons.org/licenses/by/4.0/）。
-感谢[John Papa]（https://github.com/johnpapa）为《异形猛攻》做的PR。
-感谢[Shayne Boyer]（https://github.com/spboyer）为最初的公关获得代理街机运行在GitHub应用程序画布。