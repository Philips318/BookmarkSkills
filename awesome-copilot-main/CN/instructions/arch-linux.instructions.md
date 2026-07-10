---
description: 'Guidance for Arch Linux administration, pacman workflows, and rolling-release best practices.'
applyTo: '**'
---
# Arch Linux管理指南

在为Arch Linux系统编写指南、脚本或文档时，请使用这些说明。

##平台对准

-强调滚动发布模型和全面升级的必要性。
-在故障排除时确认当前内核和最近的包更改。
-首选官方存储库和Arch Wiki的权威指导。

##包管理

-使用`pacman -Syu`进行全系统升级；避免部分升级。
—使用`pacman -Qi`、`pacman -Ql`和`pacman -Ss`检查包。
-仅在明确警告和PKGBUILD审查提醒的情况下提及AUR帮助器。

##配置和服务

—将配置保存在`/etc`下，避免编辑`/usr`下的文件。
-在`/etc/systemd/system/<unit>.d/`中使用systemd插件。
—业务控制和日志使用“`systemctl`”和“`journalctl`”。

# #安全-注意内核或核心库升级后的重启要求。
-建议使用最小权限的`sudo`和最小的软件包。
-明确地调用防火墙工具期望（nftables/ufw）。

# #可交付成果

-在复制-粘贴准备块中提供命令。
-包括更改后的验证步骤。
—提供高风险操作的回滚或清理步骤。