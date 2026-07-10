#用折叠部分组织信息

您可以通过使用`<details>`标记创建折叠部分来简化Markdown。

创建一个折叠区域

您可以通过创建读者可以选择展开的折叠部分来暂时模糊Markdown的部分。例如，当您希望在问题评论中包含技术细节时，这些细节可能与每个读者都不相关或不感兴趣，您可以将这些细节放在折叠部分中。

在`<details>`块内的任何Markdown将被折叠，直到读者点击<svg version="1.1" width="16" height="16" viewBox="0 0 16 16" class="octicon octicon-triangle-right" aria-label=" the right triangle icon" role="img"><path d="m6.427 4.427 3.396 3.396a.25.25 0 0 1 0 . 3541 -3.396 3.396a.25.25 0 0 11 16 11.396V4.604a.25.25 0 0 1. 427-.177Z"></path>xqz3xqqz展开细节。在`<details>`块中，使用`<summary>`标记让读者知道里面是什么。标签出现在<svg version="1.1" width="16" height="16" viewBox="0 0 16 16" class="octicon octicon-triangle-right" aria-label="The right triangle icon" role="img"><path d="m6.427 4.427 3.396 3.396a.25.25 0 0 1 0 . 3541 -3.396 3.396a.25.25 0 0 11 16 11.396V4.604a.25.25 0 0 1. 427-.177Z"></path></svg>。````markdown
<details>

<summary>Tips for collapsed sections</summary>

### You can add a header

You can add text within a collapsed section.

You can add an image or a code block, too.

```ruby
显示“Hello World”```

</details>
````
默认情况下，`<summary>`标签内的Markdown将被折叠：

！[在GitHub上呈现的该页面上Markdown的截图，显示了一个向右的箭头和标题“折叠部分的提示”。]

读者点击<svg version="1.1" width="16" height="16" viewBox="0 0 16 16" class="octicon octicon-triangle-right" aria-label="The right triangle icon" role="img"><path d="m6.427 4.427 3.396 3.396a.25.25 0 0 1 0 . 3541 -3.396 3.396a.25.25 0 0 11 6 11.396V4.604a.25.25 0 0 1 .427-.177Z"></path>xqz3xqqz，详细信息展开如下：

！[在GitHub上渲染的本页上面的Markdown的截图。]折叠的部分包含标题、文本、图像和代码块。]

可选地，要使该部分默认显示为打开状态，请向`<details>`标签添加`open`属性：```html
<details open>
```
##进一步阅读

* [GitHub风味降价规范]（https://github.github.com/gfm/）
*[基本书写和格式语法]（https://docs.github.com/get-started/writing-on-github/getting-started-with-writing-and-formatting-on-github/basic-writing-and-formatting-syntax）