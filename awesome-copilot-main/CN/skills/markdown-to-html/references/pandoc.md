# Pandoc参考

Pandoc是一个通用的文档转换器，可以在多种标记格式之间进行转换，包括Markdown、HTML、LaTeX、Word等等。

# #安装

# # #窗口```powershell
# Using Chocolatey
choco install pandoc

# Using Scoop
scoop install pandoc

# Or download installer from https://pandoc.org/installing.html
```
# # # macOS```bash
# Using Homebrew
brew install pandoc
```
# # # Linux```bash
# Debian/Ubuntu
sudo apt-get install pandoc

# Fedora
sudo dnf install pandoc

# Or download from https://pandoc.org/installing.html
```
##基本用法

将Markdown转换为HTML```bash
# Basic conversion
pandoc input.md -o output.html

# Standalone document with headers
pandoc input.md -s -o output.html

# With custom CSS
pandoc input.md -s --css=style.css -o output.html
```
转换为其他格式```bash
# To PDF (requires LaTeX)
pandoc input.md -s -o output.pdf

# To Word
pandoc input.md -s -o output.docx

# To LaTeX
pandoc input.md -s -o output.tex

# To EPUB
pandoc input.md -s -o output.epub
```
从其他格式转换```bash
# HTML to Markdown
pandoc -f html -t markdown input.html -o output.md

# Word to Markdown
pandoc input.docx -o output.md

# LaTeX to HTML
pandoc -f latex -t html input.tex -o output.html
```
##常用选项

|选项|描述||--------|-------------|
|`-f, --from <format>`|输入格式|
|`-t, --to <format>`|输出格式|
|`-s, --standalone`|生成独立文档|
|`-o, --output <file>`|输出文件|
|`--toc`|包含目录|
|`--toc-depth <n>`| TOC深度(默认：3
|`-N, --number-sections`|数字节标题|
|`--css <url>`|链接到CSS样式表|
|`--template <file>`|使用自定义模板|
|`--metadata <key>=<value>`|设置元数据|
使用MathML进行数学运算
使用MathJax的数学|
|`-V, --variable <key>=<value>`|设置模板变量|

## Markdown扩展

Pandoc支持许多降价扩展：```bash
# Enable specific extensions
pandoc -f markdown+emoji+footnotes input.md -o output.html

# Disable specific extensions
pandoc -f markdown-pipe_tables input.md -o output.html

# Use strict markdown
pandoc -f markdown_strict input.md -o output.html
```
###常用扩展

|扩展名|描述信息||-----------|-------------|
|`pipe_tables`|管道表（默认开启）|
|`footnotes`|脚注支持|
|`emoji`|表情符号短码|
|`smart`|智能引号和破折号|
|`task_lists`|任务列表复选框
|`strikeout`|划线文本|
|`superscript`|上标文本|
|`subscript`|下标文本|
|`raw_html`|原始HTML通过|

# #模板

使用内置模板```bash
# View default template
pandoc -D html

# Use custom template
pandoc --template=mytemplate.html input.md -o output.html
```
模板变量```html
<!DOCTYPE html>
<html>
<head>
  <title>$title$</title>
  $for(css)$
  <link rel="stylesheet" href="$css$">
  $endfor$
</head>
<body>
$body$
</body>
</html>
```
## YAML元数据

在markdown文件中包含元数据：```markdown
---
title: My Document
author: John Doe
date: 2025-01-28
abstract: |
  This is the abstract.
---

# Introduction

Document content here...
```
# #过滤器

使用Lua过滤器```bash
pandoc --lua-filter=filter.lua input.md -o output.html
```
示例Lua过滤器（`filter.lua`）：```lua
function Header(el)
  if el.level == 1 then
    el.classes:insert("main-title")
  end
  return el
end
```
###使用Pandoc过滤器```bash
pandoc --filter pandoc-citeproc input.md -o output.html
```
##批量转换

### Bash脚本```bash
#!/bin/bash
for file in *.md; do
  pandoc "$file" -s -o "${file%.md}.html"
done
```
### PowerShell脚本```powershell
Get-ChildItem -Filter *.md | ForEach-Object {
  $output = $_.BaseName + ".html"
  pandoc $_.Name -s -o $output
}
```
# #资源

- [Pandoc用户指南]（https://pandoc.org/MANUAL.html）
- [Pandoc演示]（https://pandoc.org/demos.html）
- [Pandoc FAQ]（https://pandoc.org/faqs.html）
- [GitHub Repository]（https://github.com/jgm/pandoc）