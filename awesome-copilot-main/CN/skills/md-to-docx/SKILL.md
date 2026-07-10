---
name: md-to-docx
description: Convert Markdown files to professionally formatted Word (.docx) documents with embedded PNG images — pure JavaScript, no external tools required
---
# Markdown to Word (.docx) Skill

将Markdown （`.md`）文件转换为具有嵌入式PNG图像的专业格式的Word （`.docx`）文档。使用**纯JavaScript**通过`docx`和`marked`npm包-不需要Pandoc， LibreOffice，或任何本地二进制文件。

##如何转换```bash
# Install dependencies (one-time, from the scripts folder)
cd skills/md-to-docx/scripts && npm install

# Convert (run from workspace root)
node skills/md-to-docx/scripts/md-to-docx.mjs <input.md> [output.docx]
```
如果省略`output.docx`，则默认为当前目录下的`<input-basename>.docx`。

##技能文件夹内容

|文件|用途||------|---------|
|`SKILL.md`|该指令文件|
|`scripts/md-to-docx.mjs`|Node.jsMarkdown-to-Word转换器|
|`scripts/package.json`|依赖关系（`docx`,`marked`） |

# #先决条件

|要求|版本|备注||-------------|---------|-------|
| **Node.js** | 18+ |需要运行时|
| **`docx`** | 9+ |纯JS Word文档生成器|
| **`marked`** | 15+ | Markdown解析器|

没有原生二进制文件。没有系统级安装。适用于Windows， macOS和Linux。

# #特性

转换器:

- **提取YAML首页** -使用`title`，`date`,`version`，`audience`的标题页
- **生成标题页** -项目名称，副标题，日期，版本和观众
- **生成一个目录** -建立从H1-H3标题
** -解析相对于输入的`.md`文件的`![alt](path)`引用，读取PNG，并将其嵌入到Word文档中
- Calibri字体，彩色标题（`#1F3864`），样式表与交替行颜色，在控制台的代码块
- **处理所有Markdown元素** -标题，段落，表格，代码块，列表，图像，链接，水平规则##图像嵌入

转换器自动嵌入在Markdown中引用的PNG图像：```markdown
![High-Level Architecture](diagrams/high-level-architecture.drawio.png)
```
相对于输入Markdown文件**，图像路径被解析**。读取PNG，从PNG标题中提取尺寸，并将图像缩放到适合6英寸宽，同时保留长宽比。

如果没有找到映像文件，则插入占位符`[Image not found: <path>]`。

## Front-Matter格式```yaml
---
title: Project Name — Project Summary
date: 2025-01-15
version: 1.0
audience: Engineering Team, Architects, Stakeholders
---
```
标题在`—`或`–`上分为标题页的主标题和副标题。