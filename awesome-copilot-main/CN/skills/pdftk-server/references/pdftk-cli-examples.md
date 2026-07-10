# PDFtk CLI示例

PDFtk是一个命令行程序。在运行这些示例时，请使用计算机终端或命令提示符。

整理扫描页面

将偶数和奇数扫描页交织成一个文件；```bash
pdftk A=even.pdf B=odd.pdf shuffle A B output collated.pdf
```
如果奇数页的顺序相反：```bash
pdftk A=even.pdf B=odd.pdf shuffle A Bend-1 output collated.pdf
```
##解密PDF文件

使用密码从PDF中删除加密：```bash
pdftk secured.pdf input_pw foopass output unsecured.pdf
```
##使用128位强度加密PDF

应用所有者密码加密：```bash
pdftk 1.pdf output 1.128.pdf owner_pw foopass
```
也需要密码才能打开PDF：```bash
pdftk 1.pdf output 1.128.pdf owner_pw foo user_pw baz
```
加密的同时仍然允许打印：```bash
pdftk 1.pdf output 1.128.pdf owner_pw foo user_pw baz allow printing
```
加入pdf

将多个pdf合并为一个：```bash
pdftk in1.pdf in2.pdf cat output out1.pdf
```
使用句柄进行显式控制：```bash
pdftk A=in1.pdf B=in2.pdf cat A B output out1.pdf
```
使用通配符合并目录中的所有pdf：```bash
pdftk *.pdf cat output combined.pdf
```
##删除特定页面

从文件中排除第13页：```bash
pdftk in.pdf cat 1-12 14-end output out1.pdf
```
使用句柄：```bash
pdftk A=in1.pdf cat A1-12 A14-end output out1.pdf
```
##应用40位加密

合并加密40位强度：```bash
pdftk 1.pdf 2.pdf cat output 3.pdf encrypt_40bit owner_pw foopass
```
##连接文件时，一个是密码保护

提供加密输入的密码：```bash
pdftk A=secured.pdf 2.pdf input_pw A=foopass cat output 3.pdf
```
##解压缩PDF页面流

解压内部流以进行检查或调试：```bash
pdftk doc.pdf output doc.unc.pdf uncompress
```
修复损坏的pdf文件

通过pdftk传递损坏的PDF以尝试修复：```bash
pdftk broken.pdf output fixed.pdf
```
##将PDF分解成单独的页面

将每个页面拆分为自己的文件：```bash
pdftk in.pdf burst
```
带有加密和有限打印的Burst：```bash
pdftk in.pdf burst owner_pw foopass allow DegradedPrinting
```
生成PDF元数据报告

导出书签、元数据和页面指标：```bash
pdftk in.pdf dump_data output report.txt
```
##旋转页面

将第一页顺时针旋转90度：```bash
pdftk in.pdf cat 1east 2-end output out.pdf
```
旋转所有页面180度：```bash
pdftk in.pdf cat 1-endsouth output out.pdf
```
从数据中填写PDF表单

从FDF文件填充表单字段：```bash
pdftk form.pdf fill_form data.fdf output filled_form.pdf
```
填完后将表格压平（防止进一步编辑）：```bash
pdftk form.pdf fill_form data.fdf output filled_form.pdf flatten
```
##添加背景水印

在每页后面印上水印；```bash
pdftk input.pdf background watermark.pdf output watermarked.pdf
```
##在顶部加盖一个覆盖

在每个页面的顶部应用覆盖PDF：```bash
pdftk input.pdf stamp overlay.pdf output stamped.pdf
```
##附加文件到PDF

作为附件嵌入文件：```bash
pdftk input.pdf attach_files table.html graph.png output output.pdf
```
##从PDF中提取附件

解压缩所有嵌入文件：```bash
pdftk input.pdf unpack_files output /path/to/output/
```
