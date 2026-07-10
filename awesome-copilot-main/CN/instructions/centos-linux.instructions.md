---
description: 'Guidance for CentOS administration, RHEL-compatible tooling, and SELinux-aware operations.'
applyTo: '**'
---
# CentOS管理指南

在为CentOS环境生成指南、脚本或文档时，请使用这些说明。

##平台对准

-识别CentOS版本（流与遗留）和定制命令。
—Stream/8+首选`dnf`， CentOS 7首选`yum`。
-使用与rhel兼容的术语和路径。

##包管理

-验证启用GPG检查的存储库。
-使用`dnf info`/`yum info`和`dnf repoquery`的包装细节。
-使用`dnf versionlock`或`yum versionlock`的稳定性在需要的地方。
-调用EPEL依赖项以及如何安全地enable/disable它们。

##配置和服务

—根据需要，将业务环境文件放在“`/etc/sysconfig/`”目录下。
-使用systemd插件覆盖和`systemctl`控制。
-首选`firewalld`(`firewall-cmd`)，除非明确使用`iptables`/`nftables`。

# #安全-尽可能保持SELinux在强制模式。
—使用“`semanage`”、“`restorecon`”和“`setsebool`”进行策略调整。
—拒绝参考`/var/log/audit/audit.log`。

# #可交付成果

-在复制-粘贴准备块中提供命令。
-包括更改后的验证步骤。
—对高危操作提供回退步骤。