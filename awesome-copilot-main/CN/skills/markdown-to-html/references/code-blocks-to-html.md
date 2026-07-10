#代码块到HTML

隔离代码块（无语言）

# # #减价```
function test() {
  console.log("notice the blank line before this function?");
}
```
###解析HTML```html
<pre><code>
function test() {
  console.log("notice the blank line before this function?");
}
</code></pre>
```
---

## GitHub提示Callout

# # #减价```md
> [!TIP]
> To preserve your formatting within a list, make sure to indent non-fenced code blocks by eight spaces.
```
###解析HTML （github特有）```html
<blockquote class="markdown-alert markdown-alert-tip">
  <p><strong>Tip</strong></p>
  <p>To preserve your formatting within a list, make sure to indent non-fenced code blocks by eight spaces.</p>
</blockquote>
```
---

在代码块中显示反引号

# # #减价`````md
    ````
    ```
    Look! You can see my backticks.
    ```
    ````
`````
###解析HTML```html
    <pre><code>
    ```

    Look! You can see my backticks.

    ```
    </code></pre>
```
语法高亮显示（语言标识符）

# # #减价```ruby
require 'redcarpet'
markdown = Redcarpet.new("Hello World!")
puts markdown.to_html
```
###解析HTML```html
<pre><code class="language-ruby">
require 'redcarpet'
markdown = Redcarpet.new("Hello World!")
puts markdown.to_html
</code></pre>
```
`language-ruby`类由GitHub的语法高亮灯（Linguist + grammar）使用。

语法高亮规则（html级）

| Markdown fence |解析`<code>`tag || -------------- | ------------------------------ |
| ' '`js          | `<code class="language-js"> ' |
| ' '`html        | `<code class="language-html"> ' |
| ' '`md          | `<code class="language-md"> ' |
| ' '` (no lang)  | `<code>' |

---

## HTML注释（渲染器忽略）```md
<!-- Internal documentation comment -->
```

```html
<!-- Internal documentation comment -->
```
---

# #链接```md
[About writing and formatting on GitHub](https://docs.github.com/...)
```

```html
<a href="https://docs.github.com/...">About writing and formatting on GitHub</a>
```
---

# #列表```md
* [GitHub Flavored Markdown Spec](https://github.github.com/gfm/)
```

```html
<ul>
  <li>
    <a href="https://github.github.com/gfm/">GitHub Flavored Markdown Spec</a>
  </li>
</ul>
```
---

图（概念解析）

# # #减价````md
```mermaid
图道明
A——b> b```
````
###解析HTML```html
<pre><code class="language-mermaid">
graph TD
  A --> B
</code></pre>
```
##结束语

这里没有出现`language-*`类，因为**没有提供语言标识符**。`<code>`内部的三个反引号被保存为文本。