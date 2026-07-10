---
name: pdftk-server
description: 'Skill for using the command-line tool pdftk (PDFtk Server) for working with PDF files. Use when asked to merge PDFs, split PDFs, rotate pages, encrypt or decrypt PDFs, fill PDF forms, apply watermarks, stamp overlays, extract metadata, burst documents into pages, repair corrupted PDFs, attach or extract files, or perform any PDF manipulation from the command line.'
---
# PDFtk服务器

PDFtk Server是一个用于处理PDF文档的命令行工具。它可以合并、拆分、旋转、加密、解密、加水印、盖章、填写表单、提取元数据以及以各种方式操作pdf。

何时使用此技能

-合并或连接多个PDF文件为一个
-拆分或爆发PDF到单独的页面
-旋转PDF页面
—支持对PDF文件进行加密或解密
-从FDF/XFDF数据中填写PDF表单字段
-应用背景水印或前景邮票
-提取PDF元数据、书签或表单字段信息
-修复损坏的PDF文件
-附加或提取嵌入在pdf中的文件
-从PDF中删除特定页面
-单独整理扫描的even/odd页面
—压缩或解压缩PDF页面流

# #先决条件—系统上必须安装PDFtk Server
—**Windows**:`winget install --id PDFLabs.PDFtk.Server`—**macOS**:`brew install pdftk-java`—**Linux (Debian/Ubuntu)**:`sudo apt-get install pdftk`—**Linux (RedHat/Fedora)**:`sudo dnf install pdftk`—访问终端或命令提示符
—执行命令`pdftk --version`验证安装正确性

##分步工作流程

合并多个pdf文件```bash
pdftk file1.pdf file2.pdf cat output merged.pdf
```
使用句柄进行更多控制：```bash
pdftk A=file1.pdf B=file2.pdf cat A B output merged.pdf
```
将PDF拆分为单独的页面```bash
pdftk input.pdf burst
```
###提取特定页面

摘录1-5页和10-15页：```bash
pdftk input.pdf cat 1-5 10-15 output extracted.pdf
```
###删除特定页面

删除第13页：```bash
pdftk input.pdf cat 1-12 14-end output output.pdf
```
###旋转页面

顺时针旋转所有页面90度：```bash
pdftk input.pdf cat 1-endeast output rotated.pdf
```
###加密PDF

设置所有者密码和用户密码，默认128位加密：```bash
pdftk input.pdf output secured.pdf owner_pw mypassword user_pw userpass
```
###解密PDF文件

使用已知密码删除加密：```bash
pdftk secured.pdf input_pw mypassword output unsecured.pdf
```
填写PDF表单

从FDF文件中填充表单字段，并将其扁平化以防止进一步编辑：```bash
pdftk form.pdf fill_form data.fdf output filled.pdf flatten
```
应用背景水印

在输入的每一页后面放置一个单页PDF（输入应该是透明的）：```bash
pdftk input.pdf background watermark.pdf output watermarked.pdf
```
###加盖覆盖

在输入的每一页的顶部放置一个单页PDF：```bash
pdftk input.pdf stamp overlay.pdf output stamped.pdf
```
###提取元数据

导出书签、页面指标和文档信息：```bash
pdftk input.pdf dump_data output metadata.txt
```
修复损坏的PDF

通过pdftk传递损坏的PDF以尝试自动修复：```bash
pdftk broken.pdf output fixed.pdf
```
整理扫描页面

将扫描的偶数和奇数页分开交错；```bash
pdftk A=even.pdf B=odd.pdf shuffle A B output collated.pdf
```
# #故障排除

|问题|解决方案||-------|----------|
|`pdftk`命令未找到|验证安装检查pdftk是否在您的系统PATH |中
|无法解密PDF |确保您通过`input_pw`|提供了正确的所有者或用户密码
|检查输入文件的完整性；尝试运行`pdftk input.pdf output repaired.pdf`first |
|表单字段填充后不可见|使用`flatten`标志将字段合并到页面内容|
|确保输入的PDF具有透明区域；使用`stamp`为不透明覆盖|
|权限拒绝错误|检查输入输出路径的文件权限|

# #引用`references/`文件夹中捆绑的参考文档：—[pdftk-man-page.md]（references/pdftk-man-page.md）—完整的手动参考，包括所有操作、选项和语法
- [pdftk-cli-examples.md](references/pdftk-cli-examples.md) -常见任务的实用命令行示例
- [download.md](references/download.md) -所有平台的安装和下载说明
- [pdftk-server-license.md](references/pdftk-server-license.md) - PDFtk服务器授权信息
—[third-party-materials.md]（references/third-party-materials.md）—第三方图书馆许可