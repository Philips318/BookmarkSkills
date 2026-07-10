#创建和突出显示代码块

与隔离的代码块共享代码示例，并启用语法高亮显示。

##隔离代码块

您可以通过在代码块前后放置三个反引号<code>\`\`\ '</code>来创建隔离代码块。我们建议在代码块前后各放一行空白行，以使原始格式更易于阅读。````text
```
函数test() {
Console.log（“注意到函数前的空行了吗？”）；
}```
````
！[渲染GitHub Markdown的截图，显示使用三个反引号来创建代码块。代码块以“function test() {."]（https://docs.github.com/assets/images/help/writing/fenced-code-block-rendered.png）开头。

> \ [!提示)
要在列表中保留格式，请确保将非隔离代码块缩进8个空格。

若要在隔离代码块中显示三个反引号，请将它们包装在四个反引号中。`````text
````
```
Look! You can see my backticks.
```
````
`````
！[渲染Markdown的截图显示，当你在四个反引号之间写三个反引号时，它们在渲染内容中是可见的。]

如果你经常编辑代码片段和表格，你可能会受益于在GitHub上的所有评论字段中启用固定宽度的字体。有关更多信息，请参阅[关于在GitHub上编写和格式化]（https://docs.github.com/get-started/writing-on-github/getting-started-with-writing-and-formatting-on-github/about-writing-and-formatting-on-github#enabling-fixed-width-fonts-in-the-editor）。

##语法高亮<!-- If you make changes to this feature, check whether any of the changes affect languages listed in /get-started/learning-about-github/github-language-support. If so, please update the language support article accordingly. -->
您可以添加一个可选的语言标识符，以便在隔离的代码块中启用语法高亮显示。

语法高亮会改变源代码的颜色和样式，使其更易于阅读。

例如，要用语法高亮显示Ruby代码：````text
```ruby
需要“redcarpet”
减价=走红毯。新(“Hello World !”)
把markdown.to_html```
````
这将显示带有语法高亮显示的代码块：

！在GitHub上显示的三行Ruby代码的截图。代码的元素以紫色、蓝色和红色显示，以方便扫描。

> \ [!提示)
当你创建一个隔离的代码块，你也想在GitHub页面网站上有语法高亮显示，使用小写的语言标识符。有关更多信息，请参阅[关于GitHub Pages和Jekyll]（https://docs.github.com/pages/setting-up-a-github-pages-site-with-jekyll/about-github-pages-and-jekyll#syntax-highlighting）。

我们使用[Linguist]（https://github.com/github-linguist/linguist）执行语言检测，并选择[第三方语法]（https://github.com/github-linguist/linguist/blob/main/vendor/README.md）进行语法高亮显示。您可以在[该语言的YAML文件]（https://github.com/github-linguist/linguist/blob/main/lib/linguist/languages.yml）中找出哪些关键字是有效的。

##创建图表

您还可以使用代码块在Markdown中创建图表。GitHub支持Mermaid， GeoJSON， TopoJSON和ASCII STL语法。有关更多信息，请参见[创建图表]（https://docs.github.com/get-started/writing-on-github/working-with-advanced-formatting/creating-diagrams）。

##进一步阅读* [GitHub风味降价规范]（https://github.github.com/gfm/）
*[基本书写和格式语法]（https://docs.github.com/get-started/writing-on-github/getting-started-with-writing-and-formatting-on-github/basic-writing-and-formatting-syntax）