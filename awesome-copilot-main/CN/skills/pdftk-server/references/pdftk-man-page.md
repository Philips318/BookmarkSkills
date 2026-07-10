# PDFtk服务器手册参考

- **`pdftk`2.02版本
—查看[version history]（https://www.pdflabs.com/docs/pdftk-version-history/）是否有变化
—参考[服务器手册]（https://www.pdflabs.com/docs/pdftk-man-page/）获取最新的文档

# #概述

PDFtk是一个用于操作PDF文档的命令行实用程序。它支持对PDF文件进行合并、拆分、旋转、加密、解密、水印、表单填充和元数据提取等操作。

# #剧情简介```
pdftk [input PDF files | - | PROMPT]
      [input_pw <passwords>]
      [<operation>] [<operation arguments>]
      [output <filename | - | PROMPT>]
      [encrypt_40bit | encrypt_128bit]
      [allow <permissions>]
      [owner_pw <password>] [user_pw <password>]
      [compress | uncompress]
      [flatten] [need_appearances]
      [verbose] [dont_ask | do_ask]
```
##输入选项

**输入PDF文件**：指定一个或多个PDF文件。使用`-`作为stdin或`PROMPT`作为交互式输入。文件可以被分配句柄（单个大写字母）以供操作中参考：```
pdftk A=file1.pdf B=file2.pdf cat A B output merged.pdf
```
**输入密码** (`input_pw`)：对于加密的pdf文件，提供与文件句柄或输入顺序相关的所有者或用户密码：```
pdftk A=secured.pdf input_pw A=foopass cat output unsecured.pdf
```
##核心业务

### cat -连接和组合

合并、拆分或重新排序可选旋转的页面。支持页面范围、反向排序（前缀`r`）、页面限定符（`even`/`odd`）和旋转（罗盘方向`north`、`south`、`east`、`west`、`left`、`right`、`down`）。

页面范围语法：`[handle][begin[-end[qualifier]]][rotation]````
pdftk A=in1.pdf B=in2.pdf cat A1-7 B1-5 A8 output combined.pdf
```
### shuffle -整理页面

依次从每个输入范围取一页，产生交错的结果。用于单独整理扫描的奇数和偶数页。```
pdftk A=even.pdf B=odd.pdf shuffle A B output collated.pdf
```
### burst -分割成单独的页面

将单个PDF拆分为每页一个文件。输出文件使用`printf`风格的格式命名（默认：`pg_%04d.pdf`）。```
pdftk input.pdf burst output page_%02d.pdf
```
### rotate -旋转页面

在保持文档顺序的同时旋转指定的页面。使用与`cat`相同的页面范围语法。```
pdftk in.pdf cat 1-endeast output rotated.pdf
```
### generate_fdf -提取表单数据

从PDF表单创建FDF文件，捕获当前字段值。```
pdftk form.pdf generate_fdf output form_data.fdf
```
填充表单字段

从FDF或XFDF数据文件中填充PDF表单字段。```
pdftk form.pdf fill_form data.fdf output filled.pdf flatten
```
### background -在内容后面应用水印

在输入的每一页后面应用单页PDF作为背景（水印）。输入的PDF应该有一个透明的背景，以获得最佳效果。```
pdftk input.pdf background watermark.pdf output watermarked.pdf
```
multibackground -应用多页水印

与`background`类似，但将背景PDF中的相应页面应用于输入中的匹配页面。```
pdftk input.pdf multibackground watermarks.pdf output watermarked.pdf
```
### stamp -覆盖在内容的顶部

在输入的每一页的顶部贴上一个单页PDF。当覆盖PDF不透明或没有透明度时，使用这个代替`background`。```
pdftk input.pdf stamp overlay.pdf output stamped.pdf
```
multistamp -多页叠加

类似于`stamp`，但是将来自图章PDF的相应页面应用于输入中的匹配页面。```
pdftk input.pdf multistamp overlays.pdf output stamped.pdf
```
### dump_data导出元数据

将PDF元数据、书签和页面指标输出到文本文件。```
pdftk input.pdf dump_data output metadata.txt
```
### dump_data_utf8 -导出元数据（UTF-8）

与`dump_data`相同，但输出UTF-8编码的文本。```
pdftk input.pdf dump_data_utf8 output metadata_utf8.txt
```
### dump_data_fields -提取表单字段信息

报告表单字段信息，包括类型、名称和值。```
pdftk form.pdf dump_data_fields output fields.txt
```
### dump_data_fields_utf8 -提取表单字段信息（UTF-8）

与`dump_data_fields`相同，但输出UTF-8编码的文本。

### dump_data_annots -提取注释

报告PDF注释信息。```
pdftk input.pdf dump_data_annots output annots.txt
```
update_info -修改元数据

从文本文件更新PDF元数据和书签（与`dump_data`输出格式相同）。```
pdftk input.pdf update_info metadata.txt output updated.pdf
```
update_info_utf8 -修改元数据（UTF-8）

与`update_info`相同，但期望使用UTF-8编码输入。

### attach_files -嵌入文件

将文件附加到PDF，可选地附加到特定页面。```
pdftk input.pdf attach_files table.html graph.png to_page 6 output output.pdf
```
### unpack_files -提取附件

从PDF中提取文件附件。```
pdftk input.pdf unpack_files output /path/to/output/
```
##输出选项

|选项|描述||--------|-------------|
|`output <filename>`|指定输出文件。使用`-`进行标准输出，或使用`PROMPT`进行交互。|
|`encrypt_40bit`|采用40位RC4加密|
|`encrypt_128bit`|采用128位RC4加密（设置密码时默认）
|`owner_pw <password>`|设置所有者密码|
|`user_pw <password>`|设置用户密码|
|`allow <permissions>`|授予特定权限（见下文）|
|`compress`|压缩页面流|
|`uncompress`|解压页面流（用于调试）|
|`flatten`|将表单字段平展为页面内容|
|`need_appearances`|重新生成字段外观的信号查看器|
|`keep_first_id`|保留第一次输入|的文档ID
|`keep_final_id`|保留上次输入|的文档ID
|`drop_xfa`|删除XFA表单数据|
|`verbose`|开启详细操作输出|
|`dont_ask`|抑制交互式提示|
|`do_ask`|启用交互式提示|

# #权限

加密时与`allow`关键字一起使用。可用的权限:|权限|描述||------------|-------------|
|`Printing`|允许高质量打印|
|`DegradedPrinting`|允许低质量打印|
|`ModifyContents`|允许内容修改|
|`Assembly`|允许文档组装|
|`CopyContents`|允许内容复制|
|`ScreenReaders`|允许屏幕阅读器访问|
|`ModifyAnnotations`|允许修改注释|
|`FillIn`|允许表单填写|
|`AllFeatures`|授予所有权限|

##重点说明

-页码是基于一个；对最后一个页面使用`end`关键字
-处理单个PDF时，句柄是可选的
—过滤模式（未指定操作）应用输出选项，不进行重组
-反向页面引用使用`r`前缀（例如，`r1`=最后一页，`r2`=倒数第二页）
-`background`操作需要透明输入；使用`stamp`为不透明的覆盖pdf
—输出文件名不能与输入文件名匹配