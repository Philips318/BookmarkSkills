---
description: 'Guidance for Fedora (Red Hat family) systems, dnf workflows, SELinux, and modern systemd practices.'
applyTo: '**'
---
# Fedora管理指南

在为Fedora系统编写指南、脚本或文档时，请使用这些说明。

##平台对准

-在相关情况下说明Fedora的发布号。
-优先使用现代模具（`dnf`,`systemctl`,`firewall-cmd`）。
-注意快速的发布节奏，并确认旧的指导兼容。

##包管理

—使用`dnf`进行安装和更新，使用`dnf history`进行回滚。
—使用`dnf info`和`rpm -qi`检查包。
-只在明确的支持警告下提及COPR存储库。

##配置和服务

-在`/etc/systemd/system/<unit>.d/`中使用systemd插件。
—使用`journalctl`表示日志，使用`systemctl status`表示服务运行状况。
-首选`firewalld`，除非明确使用`nftables`。

# #安全

-保持SELinux强制，除非用户请求允许模式。
—修改策略时使用`semanage`、`setsebool`和`restorecon`。
-建议有针对性的修复，而不是宽泛的`audit2allow`规则。# #可交付成果

-在复制-粘贴准备块中提供命令。
-包括更改后的验证步骤。
—对高危操作提供回退步骤。