---
name: publish-to-pages
description: 'Publish presentations and web content to GitHub Pages. Converts PPTX, PDF, HTML, or Google Slides to a live GitHub Pages URL. Handles repo creation, file conversion, Pages enablement, and returns the live URL. Use when the user wants to publish, deploy, or share a presentation or HTML file via GitHub Pages.'
---
# publish-to-pages

发布任何演示文稿或网页内容到GitHub页面在一个镜头。

# # 1。先决条件检查

静静地运行这些。仅表面误差：```bash
command -v gh >/dev/null || echo "MISSING: gh CLI — install from https://cli.github.com"
gh auth status &>/dev/null || echo "MISSING: gh not authenticated — run 'gh auth login'"
command -v python3 >/dev/null || echo "MISSING: python3 (needed for PPTX conversion)"
```
`poppler-utils`是可选的（通过`pdftoppm`转换PDF）。不要挡住它。

# # 2。输入检测

根据用户提供的内容确定输入类型：

|输入|检测||-------|-----------|
|扩展名`.html`或`.htm`|
|扩展`.pptx`|
|扩展名`.pdf`|
|谷歌幻灯片URL | URL包含`docs.google.com/presentation`|

如果没有提供，请用户提供一个repo名称。默认值：filename，不带扩展名。

# # 3。转换

###大文件处理

两个转换脚本都会自动检测大文件并切换到外部资产模式：
- **PPTX:**文件>20MB或>50图像→图像保存为单独的文件在`assets/`- **PDF:**文件>20MB或>50页→页面png保存在`assets/`文件>150MB打印一个警告（PPTX建议PDF路径代替）

这使单个文件在GitHub的100MB限制下保持良好。小文件仍然会生成一个独立的HTML。

您可以使用`--external-assets`或`--no-external-assets`强制执行该行为。

# # # HTML
不需要转换。直接使用该文件作为`index.html`。

# # # PPTX
运行转换脚本：```bash
python3 SKILL_DIR/scripts/convert-pptx.py INPUT_FILE /tmp/output.html
# For large files, force external assets:
python3 SKILL_DIR/scripts/convert-pptx.py INPUT_FILE /tmp/output.html --external-assets
```
如果缺少`python-pptx`，则告诉用户：`pip install python-pptx`# # # PDF
使用包含的脚本进行转换（`pdftoppm`需要`poppler-utils`）：```bash
python3 SKILL_DIR/scripts/convert-pdf.py INPUT_FILE /tmp/output.html
# For large files, force external assets:
python3 SKILL_DIR/scripts/convert-pdf.py INPUT_FILE /tmp/output.html --external-assets
```
每个页面都以PNG格式呈现，并通过幻灯片导航嵌入到HTML中。
如果缺少`pdftoppm`，则告诉用户：`apt install poppler-utils`（或macOS上的`brew install poppler`）。

###谷歌幻灯片
1. 从URL中提取表示ID （`/d/`和`/`之间的长字符串）
2. 下载为PPTX：```bash
curl -L "https://docs.google.com/presentation/d/PRESENTATION_ID/export/pptx" -o /tmp/slides.pptx
```
3. 然后使用上面的转换脚本转换PPTX。

# # 4。出版

# # #能见度
默认情况下，Repos是**public**创建的。如果用户指定`private`（或者想要一个私有的repo），使用`--private`-但请注意，GitHub页面上的私有repo需要一个专业，团队或企业计划。

# # #发布```bash
bash SKILL_DIR/scripts/publish.sh /path/to/index.html REPO_NAME public "Description"
```
如果用户请求，传递`private`而不是`public`。

脚本创建repo，推送`index.html`（如果存在的话，再加上`assets/`），并启用GitHub Pages。

**注意：**当使用外部资源模式时，输出的HTML引用`assets/`格式的文件。发布脚本自动检测并复制HTML文件旁边的`assets/`目录。确保HTML文件及其`assets/`目录位于同一父目录中。

# # 5。输出

告诉用户：
—**存储库：**`https://github.com/USERNAME/REPO_NAME`- **直播网址：**`https://USERNAME.github.io/REPO_NAME/`- **注意：**页面需要1-2分钟才能上线。

##错误处理- **回购已经存在：**建议追加一个数字（`my-slides-2`）或日期（`my-slides-2026`）。
- **页面启用失败：**仍然返回repo URL。用户可以在repo设置中手动启用Pages。
- **PPTX转换失败：**提示用户运行`pip install python-pptx`。
- **PDF转换失败：**建议安装`poppler-utils`（`apt install poppler-utils`或`brew install poppler`）。
- **谷歌幻灯片下载失败：**演示文稿可能无法公开访问。要求用户使其可见或手动下载PPTX。