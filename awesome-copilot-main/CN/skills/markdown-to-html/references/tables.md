#用表格组织信息

您可以构建表来组织评论、问题、拉取请求和wiki中的信息。

##创建表

您可以使用管道`|`和连字符`-`创建表。连字符用于创建每个列的标题，而管道将每个列分开。为了使表正确呈现，必须在表前包含空行。```markdown

| First Header  | Second Header |
| ------------- | ------------- |
| Content Cell  | Content Cell  |
| Content Cell  | Content Cell  |
```
！[GitHub Markdown表的截图，呈现为两个相等的列。标题显示为黑体字，替代内容行显示为灰色阴影。

表两端的管道都是可选的。

单元格的宽度可以变化，并且不需要在列内完全对齐。标题行的每列中必须至少有三个连字符。```markdown
| Command | Description |
| --- | --- |
| git status | List all new or modified files |
| git diff | Show file differences that haven't been staged |
```
！[截图的GitHub Markdown表与不同宽度的两列。行列出命令“git status”和“git diff”及其描述。

如果你经常编辑代码片段和表格，你可能会受益于在GitHub上的所有评论字段中启用固定宽度的字体。有关更多信息，请参阅[关于在GitHub上编写和格式化]（https://docs.github.com/get-started/writing-on-github/getting-started-with-writing-and-formatting-on-github/about-writing-and-formatting-on-github#enabling-fixed-width-fonts-in-the-editor）。

##格式化表内的内容

你可以在表格中使用[formatting](https://docs.github.com/get-started/writing-on-github/getting-started-with-writing-and-formatting-on-github/basic-writing-and-formatting-syntax)，比如链接、内联代码块和文本样式：```markdown
| Command | Description |
| --- | --- |
| `git status` | List all *new or modified* files |
| `git diff` | Show file differences that **haven't been** staged |
```
！[GitHub Markdown表的截图，命令格式为代码块。][https://docs.github.com/assets/images/help/writing/table-inline-formatting-rendered.png]说明中使用粗体和斜体格式。

通过在标题行中连字符的左、右或两侧包括冒号`:`，可以将文本对齐到列的左、右或中心。```markdown
| Left-aligned | Center-aligned | Right-aligned |
| :---         |     :---:      |          ---: |
| git status   | git status     | git status    |
| git diff     | git diff       | git diff      |
```
！[在GitHub上呈现的带有三列的Markdown表的截图，显示如何将单元格中的文本设置为左对齐，中对齐或右对齐。]

要在单元格中包含管道`|`作为内容，请在管道之前使用`\`：```markdown
| Name     | Character |
| ---      | ---       |
| Backtick | `         |
| Pipe     | \|        |
```
！[在GitHub上渲染的Markdown表的截图，显示了通常关闭单元格的管道在以反斜杠开头时是如何显示的。]

##进一步阅读

* [GitHub风味降价规范]（https://github.github.com/gfm/）
*[基本书写和格式语法]（https://docs.github.com/get-started/writing-on-github/getting-started-with-writing-and-formatting-on-github/basic-writing-and-formatting-syntax）