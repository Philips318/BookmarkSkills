---
name: sandbox-npm-install
description: 'Install npm packages in a Docker sandbox environment. Use this skill whenever you need to install, reinstall, or update node_modules inside a container where the workspace is mounted via virtiofs. Native binaries (esbuild, lightningcss, rollup) crash on virtiofs, so packages must be installed on the local ext4 filesystem and symlinked back.'
---
# Sandbox npm Install

何时使用此技能

无论何时使用此技能：
-你需要在一个新的沙盒会话中第一次安装npm包
—`package.json`或`package-lock.json`已更改，需要重新安装
-您会遇到带有`SIGILL`、`SIGSEGV`、`mmap`或`unaligned sysNoHugePageOS`等错误的本机二进制崩溃
—“`node_modules`”目录丢失或损坏

# #先决条件

- Docker沙盒环境，带有虚拟机安装的工作空间
—容器中可用的Node.js和NPM
—目标工作区中的`package.json`文件

# #背景

Docker沙盒工作区通常通过**virtiofs**（主机和Linux虚拟机之间的文件同步）挂载。原生Go和Rust二进制文件（esbuild、lightningcss、rollup等）在aarch64上执行时，会因mmap对齐失败而崩溃。修复方法是在容器的本地ext4文件系统上安装，并符号链接回工作空间。##分步安装

从工作空间根目录运行捆绑的安装脚本：```bash
bash scripts/install.sh
```
###常用选项

|选项|描述||---|---|
|`--workspace <path>`|包含`package.json`的目录的路径（如果省略则自动检测）|
|`--playwright`|也安装剧作家铬浏览器的E2E测试|

###脚本的作用

1. 将`package.json`、`package-lock.json`和`.npmrc`（如果存在）复制到本地ext4目录
2. 在本地文件系统上运行`npm ci`（如果没有lockfile，则运行`npm install`）
3. 符号将`node_modules`链接回工作空间
4. 如果存在，验证已知的本机二进制文件（esbuild、rollup、lightningcss、vite）
5. 可选地安装剧作家浏览器和系统依赖项（可用时使用`sudo`）

如果验证失败，请再次运行脚本—在初始设置期间崩溃可能是间歇性的。

##安装后验证

脚本完成后，验证工具链是否正常工作。例如:```bash
npm test             # Run project tests
npm run build        # Build the project
npm run dev          # Start dev server
```
##重要事项

—本地安装目录（如`/home/agent/project-deps`）为“**container-local**”，不会同步回主机
-`node_modules`符号链接在主机上显示为断开的链接-这是无害的，因为`node_modules`通常被忽略
—在主机上运行`npm ci`或`npm install`，自然会将符号链接替换为真实目录
—修改`package.json`或`package-lock.json`后，重新运行安装脚本
-不要直接在挂载的工作区中运行`npm ci`或`npm install`-本机二进制文件会崩溃

# #故障排除

|解决方案||---|---|
|运行dev server时`SIGILL`或`SIGSEGV`重新运行安装脚本；确保您没有在工作空间|中直接运行`npm install`|`node_modules`安装后未找到|检查符号链接是否存在：`ls -la node_modules`|
|安装过程中权限错误|确保当前用户|可写本地deps目录
|验证间歇性失败|再次运行脚本-首次加载时本机二进制崩溃可能不确定|

##虚拟兼容性

如果您的项目使用Vite，您可能需要在`server.fs.allow`中允许符号链接路径。将symlink目标的父目录（例如，`/home/agent/project-deps/`）添加到您的Vite配置中，以便Vite可以通过symlink提供文件。