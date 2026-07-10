#折叠部分到HTML

##`<details>`块（原始HTML在Markdown）

# # #减价````md
<details>

<summary>Tips for collapsed sections</summary>

### You can add a header

You can add text within a collapsed section.

You can add an image or a code block, too.

    ```ruby
    puts "Hello World"
    ```

</details>
````
---

###解析HTML```html
<details>
  <summary>Tips for collapsed sections</summary>

  <h3>You can add a header</h3>

  <p>You can add text within a collapsed section.</p>

  <p>You can add an image or a code block, too.</p>

  <pre><code class="language-ruby">
puts "Hello World"
</code></pre>
</details>
```
# # # #指出:

* Markdown **在`<details>`**内仍然可以正常解析。
*通过`class="language-ruby"`保留语法高亮显示。

---

默认打开（`open`属性）

# # #减价````md
<details open>

<summary>Tips for collapsed sections</summary>

### You can add a header

You can add text within a collapsed section.

You can add an image or a code block, too.

    ```ruby
    puts "Hello World"
    ```

</details>
````
###解析HTML```html
<details open>
  <summary>Tips for collapsed sections</summary>

  <h3>You can add a header</h3>

  <p>You can add text within a collapsed section.</p>

  <p>You can add an image or a code block, too.</p>

  <pre><code class="language-ruby">
puts "Hello World"
</code></pre>
</details>
```
##关键规则

*`<details>`和`<summary>`是**原始HTML**，不是Markdown语法
* Markdown在`<details>`**仍然解析**
*语法高亮在折叠的部分中正常工作
*使用`<summary>`作为**可点击的标签**

段落与内联HTML和SVG

# # #减价```md
You can streamline your Markdown by creating a collapsed section with the `<details>` tag.
```
###解析HTML```html
<p>
  You can streamline your Markdown by creating a collapsed section with the <code>&lt;details&gt;</code> tag.
</p>
```
---

###标记（保留内联SVG）```md
Any Markdown within the `<details>` block will be collapsed until the reader clicks <svg ...></svg> to expand the details.
```
###解析HTML```html
<p>
  Any Markdown within the <code>&lt;details&gt;</code> block will be collapsed until the reader clicks
  <svg version="1.1" width="16" height="16" viewBox="0 0 16 16"
       class="octicon octicon-triangle-right"
       aria-label="The right triangle icon"
       role="img">
    <path d="m6.427 4.427 3.396 3.396a.25.25 0 0 1 0 .354l-3.396 3.396A.25.25 0 0 1 6 11.396V4.604a.25.25 0 0 1 .427-.177Z"></path>
  </svg>
  to expand the details.
</p>
```
