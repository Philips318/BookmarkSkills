---
name: vscode-ext-commands
description: 'Guidelines for contributing commands in VS Code extensions. Indicates naming convention, visibility, localization and other relevant attributes, following VS Code extension development guidelines, libraries and good practices'
---
VS Code扩展命令贡献

这项技能可以帮助您在VS Code扩展中贡献命令

何时使用此技能

在需要时使用此技能：
-添加或更新命令到您的VS Code扩展

#指令VS Code命令必须始终定义一个`title`，与它的类别、可见性或位置无关。我们为每种“类型”的命令使用一些模式，它们具有一些特征，如下所述：

*常规命令：默认情况下，所有命令都应该可以在命令面板中访问，必须定义`category`，并且不需要`icon`，除非命令将在侧栏中使用。* Side Bar命令：它的名字遵循一个特殊的模式，以下划线（`_`）开头，以`#sideBar`为后缀，例如`_extensionId.someCommand#sideBar`。必须定义一个`icon`，可能有`enablement`的规则，也可能没有。侧栏专用命令不应该在命令面板中可见。将它提交给`view/title`或`view/item/context`，我们必须通知_order/position_它将被显示，并且我们可以使用术语“相对于其他command/button”，以便您确定要使用的正确的`group`。另外，为新命令可见定义条件（`when`）也是一种很好的做法。