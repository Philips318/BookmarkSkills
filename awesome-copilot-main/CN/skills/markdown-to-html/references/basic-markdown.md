基本的书写和格式化语法

用简单的语法在GitHub上为您的散文和代码创建复杂的格式。

# #标题

要创建标题，在标题文本之前添加一到六个<kbd>#</kbd>符号。您使用的<kbd>#</kbd>的数量将决定标题的层次结构级别和字体大小。```markdown
# A first-level heading
## A second-level heading
### A third-level heading
```
！[渲染GitHub Markdown显示示例h1， h2和h3标头的截图，其字体大小和视觉重量下降以显示层次结构级别。]

当你使用两个或更多的标题时，GitHub自动生成一个目录,您可以访问通过单击“大纲”菜单图标< svg version = " 1.1 "宽度=“16”高度=“16”viewBox =“0 0 16 16”class =“octicon octicon-list-unordered”aria-label =“目录”角色=“img”> <路径d = " M5.75 2.5 h8.5a.75.75 0 0 1 0 1.5 h - 8.5 - a.75.75 0 0 1 0 - 1.5 - 1.5 zm0 5 h8.5a.75.75 0 0 1 0 h - 8.5 - a.75.75 0 0 1 0 - 1.5 - 1.5 zm0 5 h8.5a.75.75 0 0 1 0 h - 8.5 - a.75.75 0 0 1 0 - 1.5 - 14 a1 zm2 1 0 1 1 0 1 1 0 0 1 0 2 zm1-6a1 1 0 1 1 - 2 0 1 1 0 0 1 2 0 zm2 4 a1 1 0 1></path>xqz</svg>在文件头。每个标题标题都列在目录中，您可以单击标题导航到所选部分。！[一个自述文件的屏幕截图与下拉菜单的内容表暴露。目录图标用深橙色标出。

##样式文本

可以在注释字段和`.md`文件中使用粗体、斜体、划线、下标或上标文本来表示强调。

|样式|语法|键盘快捷键|示例|输出| || ---------------------- | ------------------- | ------------------------------------------------------------------------------------- | ---------------------------------------- | -------------------------------------- | ------------------------------------------------- |
|粗体|`** **`或`__ __`|<kbd>Command</kbd>+<kbd>B</kbd>（Mac）或<kbd>Ctrl</kbd>+<kbd>B</kbd>(Windows/Linux) |`**This is bold text**`| **这是粗体文本** | |
|斜体|`* *`或`_ _`|<kbd>Command</kbd>+<kbd>I</kbd>（Mac）或<kbd>Ctrl</kbd>+<kbd>I</kbd>(Windows/Linux) |`_This text is italicized_`| *文本斜体* | |
|删除|`~~ ~~`或`~ ~`|无|`~~This was mistaken text~~`| ~~这是错误的文本~~ | |
|粗体和嵌套斜体|`** **`和`_ _`|无|`**This text is _extremely_ important**`| **这篇文章*极其*重要** | |
|全部加粗和斜体|`*** ***`|无|`***All this text is important***`| ***这些文字都很重要*** | <！——markdownlint- disabled -line emphasis-style——> |
|下标|`<sub> </sub>`|无|`This is a <sub>subscript</sub> text`|<sub>subscript</sub>文本| |
|上标|`<sup> </sup>`|无|`This is a <sup>superscript</sup> text`|<sup>上标t</sup>文本| |
|下划线|`<ins> </ins>`|无|`This is an <ins>underlined</ins> text`|<ins>下划线</ins>文本| |##引用文本

您可以使用<kbd>></kbd>来引用文本。```markdown
Text that is not a quote

> Text that is a quote
```
引用的文本在左侧用竖线缩进，并使用灰色字体显示。

！[渲染GitHub Markdown的截图，显示正常和引用文本之间的差异。]

> \ [!请注意)
b>在查看对话时，可以通过突出显示文本，然后键入<kbd>R</kbd>，自动引用注释中的文本。你可以通过点击<svg version="1.1" width="16" height="16" viewBox="0 0 16 16" class="octicon octicon-kebab-horizontal" aria-label="The horizontal kebab icon" role="img"><path d="M8 9a1.5 1.5 0 1 0 0 0 3 1.5 1.5 0 0 0 0 3Zm13 0a1.5 1.5 0 1 0 0 0 3 1.5 1.5 0 0 0 0 3z13 0a1.5 1.5 0 1 0 0 0 0 3Z"></path></svg>，然后**引用回复**。有关键盘快捷键的更多信息，请参见[键盘快捷键]（https://docs.github.com/en/get-started/accessibility/keyboard-shortcuts）。

##引用代码您可以在一个句子中使用单个反引号调用代码或命令。反引号内的文本将不会被格式化。您还可以按<kbd>Command</kbd>+<kbd>E</kbd>（Mac）或<kbd>Ctrl</kbd>+<kbd>E</kbd>（Windows/Linux）键盘快捷键来插入Markdown行内代码块的反引号。```markdown
Use `git status` to list all new or modified files that haven't yet been committed.
```
！[渲染GitHub Markdown的截图显示，被反引号包围的字符显示在固定宽度的字体中，并以浅灰色突出显示。]

若要将代码或文本格式化为其单独的块，请使用三个反引号。````markdown
Some basic Git commands are:
```
git状态
git添加
git提交```
````
！[渲染GitHub Markdown的截图，显示了一个简单的代码块，没有语法高亮显示。]

有关更多信息，请参见[创建和突出显示代码块]（https://docs.github.com/en/get-started/writing-on-github/working-with-advanced-formatting/creating-and-highlighting-code-blocks）。

如果你经常编辑代码片段和表格，你可能会受益于在GitHub上的所有评论字段中启用固定宽度的字体。有关更多信息，请参阅[关于在GitHub上编写和格式化]（https://docs.github.com/en/get-started/writing-on-github/getting-started-with-writing-and-formatting-on-github/about-writing-and-formatting-on-github#enabling-fixed-width-fonts-in-the-editor）。

支持的颜色模型

在问题、pull请求和讨论中，您可以通过使用反引号来指出句子中的颜色。反刻度内支持的颜色模型将显示颜色的可视化。```markdown
The background color is `#ffffff` for light mode and `#000000` for dark mode.
```
！[渲染GitHub Markdown的截图，显示了反刻度内的HEX值如何创建小圆圈的颜色，这里是白色，然后是黑色。]

下面是目前支持的颜色模型。

|颜色|语法|示例|输出信息|| ----- | --------------------------- | ----------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ |
|<code>\`#RRGGBB\`</code>|<code>\`#0969DA\`xqqz5xqz | ！[渲染GitHub Markdown的截图，显示十六进制值#0969DA如何与蓝色圆圈一起出现。]) (https://docs.github.com/assets/images/help/writing/supported-color-models-hex-rendered.png) |
|<code>\`rgb(R,G,B)\`</code>|<code>\`rgb(9, 105, 218)\`</code>| ！[渲染GitHub Markdown的截图，显示RGB值9,105,218是如何用蓝色圆圈显示的。]) (https://docs.github.com/assets/images/help/writing/supported-color-models-rgb-rendered.png) |
|<code>\`hsl(H,S,L)\`</code>|<code>\`hsl(212, 92%, 45%)\`</code>| ！[渲染GitHub Markdown的截图，显示HSL值212,92%，45%是如何用蓝色圆圈显示的。) (https://docs.github.com/assets/images/help/writing/supported-color-models-hsl-rendered.png) |

> \ [!请注意)
>
> *支持的颜色模型在反引号内不能有任何前导或尾随空格。
b> *颜色的可视化只支持在议题、拉取请求和讨论中。

# #链接通过将链接文本包装在括号`[ ]`中，然后将URL包装在括号`( )`中，可以创建内联链接。您也可以使用键盘快捷键<kbd>Command</kbd>+<kbd>K</kbd>来创建链接。选定文本后，可以从剪贴板粘贴URL，以便从所选内容自动创建链接。

您还可以通过突出显示文本并使用键盘快捷键<kbd>Command</kbd>+<kbd>V</kbd>来创建Markdown超链接。如果您想用链接替换文本，请使用键盘快捷键<kbd>Command</kbd>+<kbd>Shift</kbd>+<kbd>V</kbd>。`This site was built using [GitHub Pages](https://pages.github.com/).`！[渲染GitHub Markdown的截图，显示括号内的文本“GitHub Pages”如何显示为蓝色超链接。]> \ [!请注意)
当在注释中写入有效的url时，GitHub会自动创建链接。有关更多信息，请参见[自动链接的引用和url]（https://docs.github.com/en/get-started/writing-on-github/working-with-advanced-formatting/autolinked-references-and-urls）。

## Section链接你可以直接链接到任何有标题的部分。要在渲染文件中查看自动生成的锚点，悬停在表头暴露< svg version = " 1.1 "宽度=“16”高度=“16”viewBox =“0 0 16 16”class =“octicon octicon-link”aria-label =“链接”角色=“img”> <路径d = " m7.775 3.275 1.25 -1.25 3.5 a3.5 0 1 1 4.95 4.95 l - 2.5 - 2.5 a3.5 3.5 0 0 1 - 4.95 0 .751.751 0 0 1 .018-1.042.751.751 0 0 1 1.042 -.018 1.998 - 1.998 0 0 0 0 l2.5 2.83 - 2.5 - 2.002 a2.002 0 0 0 - 2.83 - 1.25 2.83 - 1.25 l - a.751.751 0 0 1 0 0 1 - 1.042 -.018.751.751 .018 4.69 - 9.64 - 1.042 - zm评选- a1.998 1.998 0 0 0 0 l1.25 - 1.25 - 2.83 a.751.751 0 0 11.042.018.751.751 0 01018 1.042l-1.25 1.25a3.5 3.5 01 1-4.95-4.95l2.5-2.5a3.5 3.5 0 01 4.95 0.751.751 0 01 - 0.018 1.042.751.751 0 01 -1.042.018 1.998 1.998 0 0 0-2.83 0- 2.5 2.50 a1.998 1.998 0 0 0 0 2.83Z"></path>xqz</svg>图标，点击该图标在浏览器中显示锚。！[存储库的自述文件截图]在部分标题的左侧，一个链接图标用深橙色勾画出来。

如果你需要在你正在编辑的文件中确定标题的锚点，你可以使用以下基本规则：

*字母被转换为小写。
*空格由连字符（`-`）代替。删除任何其他空格或标点符号。
*删除前导和尾随空格。
*删除标记格式，只留下内容（例如，`_italics_`变成`italics`）。
*如果自动生成的标题锚与同一文档中先前的锚相同，则通过添加连字符和自动递增的整数来生成唯一标识符。

关于URI片段的详细要求，请参见[RFC 3986: Uniform Resource Identifier (URI): Generic Syntax, Section 3.5]（https://www.rfc-editor.org/rfc/rfc3986#section-3.5）。下面的代码块演示了用于从呈现内容中的标题生成锚的基本规则。```markdown
# Example headings

## Sample Section

## This'll be a _Helpful_ Section About the Greek Letter Θ!
A heading containing characters not allowed in fragments, UTF-8 characters, two consecutive spaces between the first and second words, and formatting.

## This heading is not unique in the file

TEXT 1

## This heading is not unique in the file

TEXT 2

# Links to the example headings above

Link to the sample section: [Link Text](#sample-section).

Link to the helpful section: [Link Text](#thisll-be-a-helpful-section-about-the-greek-letter-Θ).

Link to the first non-unique section: [Link Text](#this-heading-is-not-unique-in-the-file).

Link to the second non-unique section: [Link Text](#this-heading-is-not-unique-in-the-file-1).
```
> \ [!请注意)
>如果你编辑了一个标题，或者如果你用“相同”的锚点改变了标题的顺序，你还需要更新到这些标题的链接，因为锚点会改变。

##相关链接

您可以在呈现的文件中定义相对链接和图像路径，以帮助读者导航到存储库中的其他文件。

相对链接是相对于当前文件的链接。例如，如果你在存储库的根目录下有一个自述文件，并且你在*docs/CONTRIBUTING.md*中有另一个文件，那么在你的自述文件中到*CONTRIBUTING.md*的相对链接可能看起来像这样：```text
[Contribution guidelines for this project](docs/CONTRIBUTING.md)
```
GitHub会根据你当前所在的分支自动转换你的相对链接或图像路径，以便链接或路径始终有效。链接的路径将相对于当前文件。以`/`开头的链接将相对于存储库根。您可以使用所有相对链接操作数，例如`./`和`../`。

你的链接文本应该在单行上。下面的示例不起作用。```markdown
[Contribution
guidelines for this project](docs/CONTRIBUTING.md)
```
相对链接对于克隆存储库的用户来说更容易。绝对链接可能在您的存储库的克隆中不起作用-我们建议使用相对链接来引用存储库中的其他文件。

自定义锚

可以使用标准HTML锚标记（`<a name="unique-anchor-name"></a>`）为文档中的任何位置创建导航锚点。为了避免歧义引用，请为锚标记使用唯一的命名方案，例如向`name`属性值添加前缀。

> \ [!请注意)
>自定义锚不会包含在content的文档outline/Table中。

可以使用提供给锚的`name`属性的值链接到自定义锚。语法与链接到为标题自动生成的锚点时完全相同。

例如:```markdown
# Section Heading

Some body text of this section.

<a name="my-custom-anchor-point"></a>
Some text I want to provide a direct link to, but which doesn't have its own heading.

(… more content…)

[A link to that custom anchor](#my-custom-anchor-point)
```
> \ [!提示)
>自动标题链接的自动命名和编号行为不考虑自定义锚。

##换行

如果你在一个仓库中写问题、拉取请求或讨论，GitHub会自动呈现换行：```markdown
This example
Will span two lines
```
然而，如果你是在写。在Md文件中，上面的示例将在没有换行符的情况下在一行上呈现。中创建一个换行符。Md文件，您需要包含以下内容之一：

*在第一行的末尾包括两个空格。  <pre>
这example&nbsp;,
将跨越两条线  </pre>
*在第一行末尾加上一个反斜杠。  ```markdown
  This example\
  Will span two lines
  ```
*在第一行的末尾包含HTML单行换行标记。  ```markdown
  This example<br/>
  Will span two lines
  ```
如果你在两行之间留了一个空行。md文件和Markdown在issue， pull requests，和discussion中会显示用空行分隔的两行：```markdown
This example

Will have a blank line separating both lines
```
# #图片

您可以通过添加<kbd>！</kbd>并在`[ ]`中包装Alt文本。Alt文本是相当于图像中信息的短文本。然后，将图像的链接包装在括号`()`中。`![Screenshot of a comment on a GitHub issue showing an image, added in the Markdown, of an Octocat smiling and raising a tentacle.](https://myoctocat.com/assets/images/base-octocat.svg)`！[GitHub问题的评论截图，显示在Markdown中添加的图像，一只八爪猫微笑着举起触手。]

GitHub支持嵌入图像到您的问题，拉请求，讨论，评论和`.md`文件。您可以显示存储库中的图像、添加指向在线图像的链接或上传图像。有关更多信息，请参见[上传资产]（#upload -assets）。

> \ [!请注意)
>当您想要显示存储库中的图像时，请使用相对链接而不是绝对链接。

下面是一些使用相对链接显示图像的示例。|背景信息|相对链接|| ----------------------------------------------------------- | ---------------------------------------------------------------------- |
|在同一个分支|`/assets/images/electrocat.png`|的`.md`文件中
|在另一个分支|`/../main/assets/images/electrocat.png`|的`.md`文件中
|在问题中，提取存储库|`../blob/main/assets/images/electrocat.png?raw=true`|的请求和注释
|在另一个存储库中的`.md`文件|`/../../../../github/docs/blob/main/assets/images/electrocat.png`|
|在问题中，提取另一个存储库的请求和注释|`../../../github/docs/blob/main/assets/images/electrocat.png?raw=true`|

> \ [!请注意)
b>上表中的最后两个相对链接将适用于私有存储库中的图像，前提是查看者至少具有对包含这些图像的私有存储库的读访问权限。

有关更多信息，请参阅[相对链接]（# Relative - Links）。

图片元素

支持`<picture>`HTML元素。

# #列表通过在一行或多行文本前加上<kbd>-</kbd>、<kbd>\*</kbd>或<kbd>+</kbd>，可以创建一个无序列表。```markdown
- George Washington
* John Adams
+ Thomas Jefferson
```
！[GitHub Markdown的渲染截图，显示前三位美国总统的名字。]（https://docs.github.com/assets/images/help/writing/unordered-list-rendered.png）

为了给列表排序，请在每行之前加上一个数字。```markdown
1. James Madison
2. James Monroe
3. John Quincy Adams
```
！[GitHub Markdown的渲染截图，显示了第四、第五和第六任美国总统的编号列表]（https://docs.github.com/assets/images/help/writing/ordered-list-rendered.png）

嵌套列表

可以通过在另一项下方缩进一个或多个列表项来创建嵌套列表。

要使用GitHub上的web编辑器或使用等宽字体的文本编辑器（如[Visual Studio Code](https://code.visualstudio.com/)）创建嵌套列表，您可以直观地对齐列表。在嵌套列表项前面键入空格字符，直到列表标记字符（<kbd>-</kbd>或<kbd>\*</kbd>）位于它上面的条目中文本的第一个字符的正下方。```markdown
1. First list item
   - First nested list item
     - Second nested list item
```
> \ [!请注意)
b>在基于web的编辑器中，可以缩进或独立一行或多行文本，方法是首先突出显示所需的行，然后分别使用<kbd>Tab</kbd>或<kbd>Shift</kbd>+<kbd>Tab</kbd>。

！[Visual Studio Code中显示嵌套编号行和项目符号缩进的Markdown截图]（https://docs.github.com/assets/images/help/writing/nested-list-alignment.png）

！[渲染GitHub Markdown的截图，显示一个编号的项目后面有两个不同层次的嵌套符号。]

要在GitHub上的评论编辑器中创建一个嵌套列表，它不使用等宽字体，您可以查看嵌套列表上方的列表项，并计算出现在项目内容之前的字符数量。然后在嵌套列表项前面键入该数目的空格字符。在本例中，可以在列表项`100. First list item`下添加一个嵌套列表项，方法是将嵌套列表项缩进至少五个空格，因为在`First list item`之前有五个字符（`100. `）。```markdown
100. First list item
     - First nested list item
```
！[渲染GitHub Markdown的截图，显示编号项目以数字100开头，后面是嵌套一层的项目符号项目。]

您可以使用相同的方法创建多层嵌套列表。例如，由于第一个嵌套列表项在嵌套列表内容`First nested list item`之前有7个字符（`␣␣␣␣␣-␣`），因此需要将第二个嵌套列表项至少再缩进两个字符（最少9个空格）。```markdown
100. First list item
     - First nested list item
       - Second nested list item
```
！[渲染GitHub Markdown的截图，显示编号项目以数字100开头，后面是两个不同嵌套级别的子弹。]

有关更多示例，请参阅[GitHub风味降价规范]（https://github.github.com/gfm/#example-265）。

##任务列表

要创建任务列表，请使用连字符和空格开头的列表项，后跟`[ ]`。要将任务标记为已完成，请使用`[x]`。```markdown
- [x] #739
- [ ] https://github.com/octo-org/octo-repo/issues/740
- [ ] Add delight to the experience when all tasks are complete :tada:
```
！[截图显示降价的渲染版本。对问题的引用呈现为问题标题。]

如果任务列表项描述以圆括号开头，则需要用<kbd>\\</kbd>转义：`- [ ] \(Optional) Open a followup issue`有关更多信息，请参见[关于任务列表]（https://docs.github.com/en/get-started/writing-on-github/working-with-advanced-formatting/about-task-lists）。

##提到人和团队

你可以在GitHub上输入<kbd>@</kbd>加上他们的用户名或团队名来提到一个人或[团队]（https://docs.github.com/en/organizations/organizing-members-into-teams）。这将触发通知并将他们的注意力吸引到对话中。如果你编辑评论提到了他们的用户名或团队名，人们也会收到通知。有关通知的更多信息，请参见[关于通知]（https://docs.github.com/en/account-and-profile/managing-subscriptions-and-notifications-on-github/setting-up-notifications/about-notifications）。> \ [!请注意)
b>只有当一个人对存储库具有读访问权限，并且如果存储库由一个组织拥有，那么这个人是该组织的成员时，他才会收到关于提及的通知。`@github/support What do you think about these updates?`！[渲染GitHub Markdown的截图，显示团队如何提到“@github/support”渲染为粗体，可点击的文本。]（https://docs.github.com/assets/images/help/writing/mention-rendered.png）

当您提到父团队时，其子团队的成员也会收到通知，从而简化了与多组人员的通信。有关更多信息，请参见[关于组织团队]（https://docs.github.com/en/organizations/organizing-members-into-teams/about-teams）。输入<kbd>@</kbd>符号将显示项目中的人员或团队列表。当您键入时，该列表会进行过滤，因此一旦您找到要查找的人员或团队的名称，就可以使用箭头键选择它，然后按tab键或enter键来完成名称。对于团队，输入@organization/team-name，该团队的所有成员都将订阅该对话。

自动完成结果仅限于存储库协作者和线程上的任何其他参与者。

引用问题和拉取请求

通过输入<kbd>#</kbd>，可以在存储库中显示建议问题和拉取请求的列表。输入问题或拉请求号或标题来过滤列表，然后按tab键或enter键来完成突出显示的结果。

有关更多信息，请参见[自动链接的引用和url]（https://docs.github.com/en/get-started/writing-on-github/working-with-advanced-formatting/autolinked-references-and-urls）。

引用外部资源如果为存储库配置了自定义自动链接引用，那么对外部资源（如JIRA issue或Zendesk票证）的引用将转换为缩短的链接。要了解存储库中有哪些自动链接可用，请联系具有存储库管理权限的人员。有关更多信息，请参见[配置引用外部资源的自动链接]（https://docs.github.com/en/repositories/managing-your-repositorys-settings-and-features/managing-repository-settings/configuring-autolinks-to-reference-external-resources）。

##上传资产

您可以通过拖放、从文件浏览器中选择或粘贴等方式上传图像等资产。您可以将资产上传到存储库中的issue、pull requests、comments和`.md`文件。

##使用表情符号

你可以通过输入`:EMOJICODE:`来添加表情符号，一个冒号后面跟着表情符号的名称。`@octocat :+1: This PR looks great - it's ready to merge! :shipit:`！[渲染GitHub Markdown的截图，显示了+1和shipit的表情符号代码如何在视觉上呈现为表情符号。]输入<kbd>:</kbd>会弹出一个建议的表情符号列表。当你输入时，列表会过滤，所以一旦你找到你要找的表情符号，按**Tab**或**Enter**来完成突出显示的结果。

有关可用表情符号和代码的完整列表，请参阅[emoji - cheat sheet]（https://github.com/ikatyang/emoji-cheat-sheet/blob/github-actions-auto-update/README.md）。

# #段落

您可以通过在文本行之间留下空白行来创建新段落。

# #脚注

你可以使用括号语法给你的内容添加脚注：```text
Here is a simple footnote[^1].

A footnote can also have multiple lines[^2].

[^1]: My reference.
[^2]: To add line breaks within a footnote, add 2 spaces to the end of a line.  
This is a second line.
```
脚注将呈现如下：

！[显示用于指示脚注的上标数字的渲染Markdown的截图，以及注释内可选的换行符。]

> \ [!请注意)
>标记中脚注的位置不会影响脚注的呈现位置。您可以在引用脚注之后编写脚注，并且脚注仍将在Markdown的底部呈现。wiki中不支持脚注。

# #警报

**警报**，有时也被称为**callouts**或** warnings **，是一个基于blockquote语法的Markdown扩展，您可以使用它来强调关键信息。在GitHub上，它们以独特的颜色和图标显示，以表明内容的重要性。只有当它们对用户成功至关重要时才使用提醒，每篇文章限制在一到两个，以防止读者过载。此外，您应该避免连续放置警报。警报不能嵌套在其他元素中。

要添加警报，请使用特殊的blockquote行指定警报类型，后跟标准blockquote中的警报信息。警报有五种类型：```markdown
> [!NOTE]
> Useful information that users should know, even when skimming content.

> [!TIP]
> Helpful advice for doing things better or more easily.

> [!IMPORTANT]
> Key information users need to know to achieve their goal.

> [!WARNING]
> Urgent info that needs immediate user attention to avoid problems.

> [!CAUTION]
> Advises about risks or negative outcomes of certain actions.
```
下面是渲染后的警报：

！[已呈现的降价警报截图，显示注意、提示、重要、警告和警告如何以不同颜色的文本和图标呈现]（https://docs.github.com/assets/images/help/writing/alerts-rendered.png）

##隐藏带有注释的内容

你可以告诉GitHub通过将内容放在HTML注释中来隐藏渲染Markdown的内容。```text
<!-- This content will not appear in the rendered Markdown -->
```
忽略Markdown格式

你可以通过在Markdown字符之前使用<kbd>\\</kbd>来告诉GitHub忽略（或转义）Markdown格式。`Let's rename \*our-new-project\* to \*our-old-project\*.`！[渲染GitHub Markdown的截图，显示反斜杠如何防止星号转换为斜体。]

有关反斜杠的更多信息，请参阅Daring Fireball的[Markdown Syntax]（https://daringfireball.net/projects/markdown/syntax#backslash）。

> \ [!请注意)
在问题或pull请求的标题中，Markdown格式不会被忽略。

禁用Markdown渲染

查看Markdown文件时，您可以单击文件顶部的**Code**来禁用Markdown呈现，而查看文件的源代码。

！存储库中Markdown文件的截图，显示与该文件交互的选项。标记为“代码”的按钮用深橙色勾画出来。禁用Markdown呈现使您能够使用源视图特性，例如行链接，这在查看呈现的Markdown文件时是不可能的。

##进一步阅读

*[GitHub风味降价规范]（https://github.github.com/gfm/）
*[关于在GitHub上写作和格式化]（https://docs.github.com/en/get-started/writing-on-github/getting-started-with-writing-and-formatting-on-github/about-writing-and-formatting-on-github）
*[使用高级格式]（https://docs.github.com/en/get-started/writing-on-github/working-with-advanced-formatting）
*[在GitHub上写作的快速入门]（https://docs.github.com/en/get-started/writing-on-github/getting-started-with-writing-and-formatting-on-github/quickstart-for-writing-on-github）