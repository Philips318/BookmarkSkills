# MSYS2引用

MSYS2提供了一组工具和库，用于构建、安装和运行本机Windows软件。它使用Pacman（来自Arch Linux）进行包管理。

##开始

-[入门]（https://www.msys2.org/）
-[什么是MSYS2?]] (https://www.msys2.org/docs/what-is-msys2/)
-[谁在使用MSYS2？] (https://www.msys2.org/docs/who-is-using-msys2/)
- [MSYS2 Installer]（https://www.msys2.org/docs/installer/）
【新闻】(https://www.msys2.org/news/)
- (FAQ) (https://www.msys2.org/docs/faq/)
-[支持的Windows版本和硬件]（https://www.msys2.org/docs/windows_support/）
- [ARM64支持]（https://www.msys2.org/docs/arm64/）

# #环境

MSYS2提供针对不同用例的多种环境：

-[环境概述]（https://www.msys2.org/docs/environments/）
[GCC vsLLVM/Clang]（https://www.msys2.org/docs/environments/#gcc-vs-llvmclang）
- [MSVCRT vs UCRT]（https://www.msys2.org/docs/environments/#msvcrt-vs-ucrt）
-【变更】(https://www.msys2.org/docs/environments/#changelog)

|环境|前缀|工具链| C运行时||-------------|--------|-----------|-----------|
| MSYS |`/usr`| GCC | cygwin |
| MINGW64 |`/mingw64`| GCC | MSVCRT |
| UCRT64 |`/ucrt64`| GCC | UCRT |
| CLANG64 |`/clang64`| LLVM | UCRT |
| CLANGARM64 |`/clangarm64`| LLVM | UCRT |

# #配置

-[更新MSYS2]（https://www.msys2.org/docs/updating/）
-[文件系统路径]（https://www.msys2.org/docs/filesystem-paths/）
-(符号链接)(https://www.msys2.org/docs/symlinks/)
—[配置位置]（https://www.msys2.org/docs/configuration/）
-(终端)(https://www.msys2.org/docs/terminals/)
- [ide和文本编辑器]（https://www.msys2.org/docs/ides-editors/）
-[即时调试]（https://www.msys2.org/docs/jit-debugging/）

##包管理

-[包管理]（https://www.msys2.org/docs/package-management/）
-[包命名]（https://www.msys2.org/docs/package-naming/）
-[包装索引]（https://packages.msys2.org/）
-[存储库和镜像]（https://www.msys2.org/docs/repos-mirrors/）
- [Package Mirrors]（https://www.msys2.org/docs/mirrors/）
-[提示和技巧]（https://www.msys2.org/docs/package-management-tips/）
- (FAQ) (https://www.msys2.org/docs/package-management-faq/)
-(小精灵)(https://www.msys2.org/docs/pacman/)

##开发工具

-[在MSYS2中使用CMake]（https://www.msys2.org/docs/cmake/）
——[Autotools] (https://www.msys2.org/docs/autotools/)
- (Python) (https://www.msys2.org/docs/python/)
- (Git) (https://www.msys2.org/docs/git/)
- (C/C+ +) (https://www.msys2.org/docs/c/)
- (c++) (https://www.msys2.org/docs/cpp/)
——[pkg-config] (https://www.msys2.org/docs/pkgconfig/)
-[在CI中使用MSYS2]（https://www.msys2.org/docs/ci/）##包开发

-[创建新包]（https://www.msys2.org/dev/new-package/）
-[更新现有软件包]（https://www.msys2.org/dev/update-package/）
-[包装指引]（https://www.msys2.org/dev/package-guidelines/）
—[License元数据]（https://www.msys2.org/dev/package-licensing/）
——[PKGBUILD] (https://www.msys2.org/dev/pkgbuild/)
-(镜子)(https://www.msys2.org/dev/mirrors/)
- [MSYS2 Keyring]（https://www.msys2.org/dev/keyring/）
- (Python) (https://www.msys2.org/dev/python/)
-[自动构建过程]（https://www.msys2.org/dev/build-process/）
-[漏洞报告]（https://www.msys2.org/dev/vulnerabilities/）
-[账户和所有权]（https://www.msys2.org/dev/accounts/）

# #维基

-[欢迎来到MSYS2 wiki]（https://www.msys2.org/wiki/Home/）
MSYS2和Cygwin有什么不同？] (https://www.msys2.org/wiki/How-does-MSYS2-differ-from-Cygwin/)
——[MSYS2-Introduction] (https://www.msys2.org/wiki/MSYS2-introduction/)
- [MSYS2历史记录]（https://www.msys2.org/wiki/History/）
-[创建包]（https://www.msys2.org/wiki/Creating-Packages/）
-【分配】(https://www.msys2.org/wiki/Distributing/)
-(发射器)(https://www.msys2.org/wiki/Launchers/)
-(移植)(https://www.msys2.org/wiki/Porting/)
-[重新安装MSYS2]（https://www.msys2.org/wiki/MSYS2-reinstallation/）
-[设置SSHd]（https://www.msys2.org/wiki/Setting-up-SSHd/）
-[签名包]（https://www.msys2.org/wiki/Signing-packages/）
你需要Sudo吗？] (https://www.msys2.org/wiki/Sudo/)
-(终端)(https://www.msys2.org/wiki/Terminals/)
- [Qt Creator]（https://www.msys2.org/wiki/GDB-qtcreator/）
-[待办事项列表]（https://www.msys2.org/wiki/Devtopics/）