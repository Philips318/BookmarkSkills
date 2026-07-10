---
name: vscode-ext-localization
description: 'Guidelines for proper localization of VS Code extensions, following VS Code extension development guidelines, libraries and good practices'
---
VS Code扩展本地化

这项技能可以帮助您本地化VS Code扩展的各个方面

何时使用此技能

在需要时使用此技能：
-本地化新的或现有的贡献配置（设置）、命令、菜单、视图或演练
—本地化显示给最终用户的扩展源代码中包含的新的或现有的消息或其他字符串资源

#指令VS Code本地化由三种不同的方法组成，具体取决于正在本地化的资源。当创建或更新新的可本地化资源时，所有当前可用语言的相应本地化必须为created/updated.1. 配置，如设置，命令，菜单，视图，视图欢迎，演练标题和描述，在`package.json`中定义
>一个专属的`package.nls.LANGID.json`文件，类似于巴西葡萄牙语（`pt-br`）本地化的`package.nls.pt-br.json`2. 演练内容（在其自己的`Markdown`文件中定义）
>一个专属的`Markdown`文件，如`walkthrough/someStep.pt-br.md`，用于巴西葡萄牙语本地化
3. 消息和字符串位于扩展源代码（JavaScript或TypeScript文件）
>专属于巴西葡萄牙语本地化的`bundle.l10n.pt-br.json`