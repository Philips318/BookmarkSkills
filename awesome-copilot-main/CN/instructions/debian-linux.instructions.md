---
description: 'Guidance for Debian-based Linux administration, apt workflows, and Debian policy conventions.'
applyTo: '**'
---
# Debian Linux管理指南

在为基于debian的系统编写指南、脚本或文档时，请使用这些说明。

##平台对准

-支持Debian稳定默认值和长期支持期望。
-调用Debian版本（`bookworm`，`bullseye`等）。
-在推荐第三方资源之前，更喜欢官方的Debian资源库。

##包管理

—将`apt`用于交互式命令，将`apt-get`用于脚本。
—使用`apt-cache policy`、`apt show`和`dpkg -l`检查包。
-使用`apt-mark`跟踪手动与自动安装的软件包。
-在`/etc/apt/preferences.d/`中记录任何apt固定并解释原因。

##配置和服务—将配置保存在`/etc`下，避免直接修改`/usr`文件。
-在`/etc/systemd/system/<unit>.d/`中使用systemd插件进行覆盖。
—业务控制和日志首选`systemctl`和`journalctl`。
-使用`ufw`或`nftables`防火墙指导；预期的状态。

# #安全

-说明AppArmor配置文件，并在需要时提及调整。
-建议使用最低权限的`sudo`和最少的软件包安装。
—包含安全变更后的验证命令。

# #可交付成果

-在复制-粘贴准备块中提供命令。
-包括更改后的验证步骤。
-为破坏性操作提供回滚步骤。