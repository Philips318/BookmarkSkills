# Cygwin参考

Cygwin提供了大量的GNU和开源工具，这些工具提供了与Windows上的Linux发行版类似的功能，以及一个POSIX API DLL (`cygwin1.dll`)，以实现大量的Linux API兼容性。

# #文档

- [Cygwin用户指南](https://cygwin.com/cygwin-ug-net.html) -全面的官方文档
- [Cygwin常见问题解答]（https://cygwin.com/faq.html）
- [Cygwin主页]（https://cygwin.com/）

##用户指南-目录

第1章：Cygwin概述- **这是什么？** - POSIX兼容层和Windows的GNU工具集
- **快速入门指南（Windows用户）** - Windows用户入门指南
- **快速入门指南（UNIX用户）** -熟悉UNIX/Linux的用户入门
- ** Cygwin工具是免费软件吗？** -授权（GPL/LGPL）
- ** Cygwin项目的简史** -起源和演变
- ** Cygwin功能亮点**
-权限和安全
-文件访问
-文本模式vs.二进制模式
- ANSI C库
-流程创建
——信号
-套接字及选择
- **什么是新的，什么改变了** -发布说明所有版本(1.7。X到3.6)

第2章：设置Cygwin通过`setup-x86_64.exe`安装，镜像选择，包管理
- **环境变量** -配置`PATH`、`HOME`、`CYGWIN`等环境变量
** *通过注册表调整内存限制
- **国际化** -区域设置和字符集配置
- **自定义bash** -`.bashrc`，`.bash_profile`，并提示自定义

第三章：使用Cygwin- **映射路径名称** - Cygwin如何将POSIX路径映射到Windows路径（`/cygdrive/c`=`C:\`）
** -行结束处理（`\n`vs`\r\n`），挂载选项
- **文件权限** - NTFS、acl的POSIX权限模式
-设备文件，`/proc`,`/dev`，套接字文件
- **POSIX帐号、权限、安全性** -User/group映射、`passwd`/`group`文件、`ntsec`- Cygserver** -后台服务共享内存，消息队列，信号量
- **Cygwin Utilities** -内置命令行工具：
—`cygcheck`—系统信息和包诊断
-`cygpath`- POSIX路径和Windows路径之间的转换
-`cygstart`-打开files/URLs与相关的Windows应用程序
-`dumper`—创建Windows小转储
-`getconf`-查询POSIX系统配置
—`getfacl`/`setfacl`—Get/set文件访问控制列表
-`ldd`- L共享库依赖项列表
-`locale`—显示区域信息
-`minidumper`—写入运行中进程的小转储
-`mkgroup`/`mkpasswd`-从Windows帐户生成group/passwd条目
-`mount`/`umount`-管理Cygwin挂载表
—`passwd`—修改密码
-`pldd`-列出进程加载的dll
-`profiler`-配置文件Cygwin程序
—`ps`—列出正在运行的进程
-`regtool`-从shell访问Windows注册表
-`setmetamode`-控制控制台的元键行为
-`ssp`-单步分析器
-`strace`-跟踪系统调用和信号
-`tzset`-打印posix兼容的时区字符串
- **区分大小写的目录** -在Windows 10+上启用每个目录的区分大小写功能
- **有效地使用Cygwin与Windows** -集成提示，从Cygwin运行Windows程序第4章：Cygwin编程

** -使用Cygwin GCC工具链编译xqz48xq++程序
- **调试Cygwin程序** -使用GDB等调试工具
-在Cygwin下创建共享库
- **定义Windows资源** -资源文件和`windres`**性能分析与`gprof`和`ssp`批处理脚本的关键概念

从批处理文件中调用Cygwin```batch
REM Run a Cygwin command from a batch file
C:\cygwin64\bin\bash.exe -l -c "ls -la /home"

REM Convert a Windows path to POSIX for Cygwin
C:\cygwin64\bin\cygpath.exe -u "C:\Users\John Doe\Documents"

REM Convert a POSIX path back to Windows
C:\cygwin64\bin\cygpath.exe -w "/home/jdoe/project"
```
###常见环境变量

|变量|用途||----------|---------|
|`CYGWIN`|运行时选项（如`nodosfilewarning`，`winsymlinks:nativestrict`） |
|`HOME`|用户主目录|
|`PATH`| Cygwin tools |必须包含`/usr/local/bin:/usr/bin`|`SHELL`|默认shell（通常为`/bin/bash`） |
|`TERM`|控制台应用程序的终端类型|