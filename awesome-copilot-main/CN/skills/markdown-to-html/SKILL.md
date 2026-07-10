---
name: markdown-to-html
description: 'Convert Markdown files to HTML similar to `marked.js`, `pandoc`, `gomarkdown/markdown`, or similar tools; or writing custom script to convert markdown to html and/or working on web template systems like `jekyll/jekyll`, `gohugoio/hugo`, or similar web templating systems that utilize markdown documents, converting them to html. Use when asked to "convert markdown to html", "transform md to html", "render markdown", "generate html from markdown", or when working with .md files and/or web a templating system that converts markdown to HTML output. Supports CLI and Node.js workflows with GFM, CommonMark, and standard Markdown flavors.'
---
# Markdown到HTML转换

熟练使用marked.js库将Markdown文档转换为HTML，或编写数据转换脚本；在这种情况下，脚本类似于[markedJS/marked]（https://github.com/markedjs/marked）存储库。对于自定义脚本，知识并不局限于`marked.js`，数据转换方法是利用像[pandoc]（https://github.com/jgm/pandoc）和[gomarkdown/markdown]（https://github.com/gomarkdown/markdown）这样的工具进行数据转换；[jekyll/jekyll]（https://github.com/jekyll/jekyll）和[gohugoio/hugo]（https://github.com/gohugoio/hugo）用于模板系统。

转换脚本或工具应该处理单个文件、批量转换和高级配置。

何时使用此技能-用户要求“将markdown转换为html”或“转换为md文件”
-用户想要“渲染降价”作为HTML输出
-用户需要从生成HTML文档。md文件
-用户正在从Markdown内容建立静态网站
-用户正在建立模板系统转换markdown到html
-用户正在为现有模板系统制作工具、小部件或自定义模板
-用户想要预览标记为呈现HTML

将Markdown转换为HTML

基本的基本转换

更多信息请参见[basic-markdown-to-html.md]（references/basic-markdown-to-html.md）```text
    ```markdown
    # Level 1
    ## Level 2

    One sentence with a [link](https://example.com), and a HTML snippet like `<p>paragraph tag</p>`.

    - `ul` list item 1
    - `ul` list item 2

    1. `ol` list item 1
    2. `ol` list item 1

    | Table Item | Description |
    | One | One is the spelling of the number `1`. |
    | Two | Two is the spelling of the number `2`. |

    ```js
    var one = 1;
    var two = 2;

    function simpleMath(x, y) {
     return x + y;
    }
    console.log(simpleMath(one, two));
    ```
    ```

    ```html
    <h1>Level 1</h1>
    <h2>Level 2</h2>

    <p>One sentence with a <a href="https://example.com">link</a>, and a HTML snippet like <code>&lt;p&gt;paragraph tag&lt;/p&gt;</code>.</p>

    <ul>
     <li>`ul` list item 1</li>
     <li>`ul` list item 2</li>
    </ul>

    <ol>
     <li>`ol` list item 1</li>
     <li>`ol` list item 2</li>
    </ol>

    <table>
     <thead>
      <tr>
       <th>Table Item</th>
       <th>Description</th>
      </tr>
     </thead>
     <tbody>
      <tr>
       <td>One</td>
       <td>One is the spelling of the number `1`.</td>
      </tr>
      <tr>
       <td>Two</td>
       <td>Two is the spelling of the number `2`.</td>
      </tr>
     </tbody>
    </table>

    <pre>
     <code>var one = 1;
     var two = 2;

     function simpleMath(x, y) {
      return x + y;
     }
     console.log(simpleMath(one, two));</code>
    </pre>
    ```
```
代码块转换

更多信息请参见[code-blocks-to-html.md]（references/code-blocks-to-html.md）```text

    ```markdown
    your code here
    ```

    ```html
    <pre><code class="language-md">
    your code here
    </code></pre>
    ```

    ```js
    console.log("Hello world");
    ```

    ```html
    <pre><code class="language-js">
    console.log("Hello world");
    </code></pre>
    ```

    ```markdown
      ```

      ```
      visible backticks
      ```

      ```
    ```

    ```html
      <pre><code>
      ```

      visible backticks

      ```
      </code></pre>
    ```
```
折叠部分转换

更多信息请参见[collapsed-sections-to-html.md]（references/collapsed-sections-to-html.md）```text
    ```markdown
    <details>
    <summary>More info</summary>

    ### Header inside

    - Lists
    - **Formatting**
    - Code blocks

        ```js
        console.log("Hello");
        ```

    </details>
    ```

    ```html
    <details>
    <summary>More info</summary>

    <h3>Header inside</h3>

    <ul>
     <li>Lists</li>
     <li><strong>Formatting</strong></li>
     <li>Code blocks</li>
    </ul>

    <pre>
     <code class="language-js">console.log("Hello");</code>
    </pre>

    </details>
    ```
```
数学表达式转换

更多信息请参见[writing-mathematical-expressions-to-html.md]（references/writing-mathematical-expressions-to-html.md）```text
    ```markdown
    This sentence uses `$` delimiters to show math inline: $\sqrt{3x-1}+(1+x)^2$
    ```

    ```html
    <p>This sentence uses <code>$</code> delimiters to show math inline:
     <math-renderer><math xmlns="http://www.w3.org/1998/Math/MathML">
      <msqrt><mn>3</mn><mi>x</mi><mo>−</mo><mn>1</mn></msqrt>
      <mo>+</mo><mo>(</mo><mn>1</mn><mo>+</mo><mi>x</mi>
      <msup><mo>)</mo><mn>2</mn></msup>
     </math>
    </math-renderer>
    </p>
    ```

    ```markdown
    **The Cauchy-Schwarz Inequality**\
    $$\left( \sum_{k=1}^n a_k b_k \right)^2 \leq \left( \sum_{k=1}^n a_k^2 \right) \left( \sum_{k=1}^n b_k^2 \right)$$
    ```

    ```html
    <p><strong>The Cauchy-Schwarz Inequality</strong><br>
     <math-renderer>
      <math xmlns="http://www.w3.org/1998/Math/MathML">
       <msup>
        <mrow><mo>(</mo>
         <munderover><mo data-mjx-texclass="OP">∑</mo>
          <mrow><mi>k</mi><mo>=</mo><mn>1</mn></mrow><mi>n</mi>
         </munderover>
         <msub><mi>a</mi><mi>k</mi></msub>
         <msub><mi>b</mi><mi>k</mi></msub>
         <mo>)</mo>
        </mrow>
        <mn>2</mn>
       </msup>
       <mo>≤</mo>
       <mrow><mo>(</mo>
        <munderover><mo>∑</mo>
         <mrow><mi>k</mi><mo>=</mo><mn>1</mn></mrow>
         <mi>n</mi>
        </munderover>
        <msubsup><mi>a</mi><mi>k</mi><mn>2</mn></msubsup>
        <mo>)</mo>
       </mrow>
       <mrow><mo>(</mo>
         <munderover><mo>∑</mo>
          <mrow><mi>k</mi><mo>=</mo><mn>1</mn></mrow>
          <mi>n</mi>
         </munderover>
         <msubsup><mi>b</mi><mi>k</mi><mn>2</mn></msubsup>
         <mo>)</mo>
       </mrow>
      </math>
     </math-renderer></p>
    ```
```
表转换

更多信息请参见[tables-to-html.md]（references/tables-to-html.md）```text
    ```markdown
    | First Header  | Second Header |
    | ------------- | ------------- |
    | Content Cell  | Content Cell  |
    | Content Cell  | Content Cell  |
    ```

    ```html
    <table>
     <thead><tr><th>First Header</th><th>Second Header</th></tr></thead>
     <tbody>
      <tr><td>Content Cell</td><td>Content Cell</td></tr>
      <tr><td>Content Cell</td><td>Content Cell</td></tr>
     </tbody>
    </table>
    ```

    ```markdown
    | Left-aligned | Center-aligned | Right-aligned |
    | :---         |     :---:      |          ---: |
    | git status   | git status     | git status    |
    | git diff     | git diff       | git diff      |
    ```

    ```html
    <table>
      <thead>
       <tr>
        <th align="left">Left-aligned</th>
        <th align="center">Center-aligned</th>
        <th align="right">Right-aligned</th>
       </tr>
      </thead>
      <tbody>
       <tr>
        <td align="left">git status</td>
        <td align="center">git status</td>
        <td align="right">git status</td>
       </tr>
       <tr>
        <td align="left">git diff</td>
        <td align="center">git diff</td>
        <td align="right">git diff</td>
       </tr>
      </tbody>
    </table>
    ```
```
使用[`markedJS/marked`]（references/marked.md）

# # #先决条件

-已安装Node.js（用于CLI或编程使用）
—安装全局标记为CLI:`npm install -g marked`—或者本地安装：`npm install marked`快速转换方法

参见[marked.md](references/marked.md) **快速转换方法**

###逐步工作流程

参见[marked.md](references/marked.md) **分步工作流程**

### CLI配置

使用配置文件

为持久选项创建`~/.marked.json`：```json
{
  "gfm": true,
  "breaks": true
}
```
或者使用自定义配置：```bash
marked -i input.md -o output.html -c config.json
```
命令行选项参考

|选项|描述||--------|-------------|
|`-i, --input <file>`|输入Markdown文件|
|`-o, --output <file>`|输出HTML文件|
|`-s, --string <string>`|解析字符串而不是文件|
|`-c, --config <file>`|使用自定义配置文件|
|`--gfm`|启用GitHub风味降价|
|`--breaks`|将换行符转换为`<br>`|
|`--help`|显示所有选项|

安全警告

⚠️**标记不清理输出的HTML。**对于不可信的输入，请使用杀菌剂：```javascript
import { marked } from 'marked';
import DOMPurify from 'dompurify';

const unsafeHtml = marked.parse(untrustedMarkdown);
const safeHtml = DOMPurify.sanitize(unsafeHtml);
```
建议删除工具:

- [DOMPurify](https://github.com/cure53/DOMPurify)（推荐）
——[sanitize-html] (https://github.com/apostrophecms/sanitize-html)
——[js-xss] (https://github.com/leizongmin/js-xss)

支持Markdown口味

|风味|支持||--------|---------|
|原价下调| 100% |
| CommonMark 0.31 | 98% |
GitHub风味降价| 97% |

# # #故障排除

|问题|解决方案||-------|----------|
|文件起始特殊字符|条带零宽度字符：`content.replace(/^[\u200B\u200C\u200D\uFEFF]/,"")`|
|代码块不高亮|添加语法高亮，如highlight.js|
|表不呈现|确保`gfm: true`选项设置|
|在选项|中设置`breaks: true`使用DOMPurify净化输出|

##使用[`pandoc`]（references/pandoc.md）

# # #先决条件

-安装了Pandoc（从<https://pandoc.org/installing.html>下载）
- PDF输出：LaTeX安装（macOS上的MacTeX， Windows上的MiKTeX， Linux上的texlive）
-Terminal/command提示访问

快速转换方法

####方法一：CLI Basic Conversion```bash
# Convert markdown to HTML
pandoc input.md -o output.html

# Convert with standalone document (includes header/footer)
pandoc input.md -s -o output.html

# Explicit format specification
pandoc input.md -f markdown -t html -s -o output.html
```
####方法二：过滤模式（交互）```bash
# Start pandoc as a filter
pandoc

# Type markdown, then Ctrl-D (Linux/macOS) or Ctrl-Z+Enter (Windows)
Hello *pandoc*!
# Output: <p>Hello <em>pandoc</em>!</p>
```
####方法三：格式转换```bash
# HTML to Markdown
pandoc -f html -t markdown input.html -o output.md

# Markdown to LaTeX
pandoc input.md -s -o output.tex

# Markdown to PDF (requires LaTeX)
pandoc input.md -s -o output.pdf

# Markdown to Word
pandoc input.md -s -o output.docx
```
### CLI配置

|选项|描述||--------|-------------|
|`-f, --from <format>`|输入格式(markdown, html， latex等
|`-t, --to <format>`|输出格式(html、latex、pdf、docx等
使用header/footer|生成独立文档
|`-o, --output <file>`|输出文件（从扩展名推断）|
|`--mathml`|转换TeX数学到MathML |
|`--metadata title="Title"`|设置文档元数据|
|`--toc`|包含目录|
|`--template <file>`|使用自定义模板|
|`--help`|显示所有选项|

安全警告

⚠️**Pandoc忠实地处理输入。**转换不可信标记时：

—使用`--sandbox`模式关闭对外文件访问
-在处理之前验证输入
-清理浏览器中显示的HTML输出```bash
# Run in sandbox mode for untrusted input
pandoc --sandbox input.md -o output.html
```
支持Markdown口味

|风味|支持||--------|---------|
| Pandoc Markdown | 100%（原生）|
| CommonMark | Full（使用`-f commonmark`） |
GitHub风味Markdown | Full（使用`-f gfm`） |
| MultiMarkdown | Partial |

# # #故障排除

|问题|解决方案||-------|----------|
| PDF生成失败|安装LaTeX (MacTeX、MiKTeX、texlive) | . PDF生成失败
| Windows上的编码问题|在使用pandoc |之前运行`chcp 65001`|为完整的文档添加`-s`标志|
使用`--mathml`或`--mathjax`选项|
|表不呈现|确保正确的表语法使用管道和破折号|

##使用[`gomarkdown/markdown`]（references/gomarkdown.md）

# # #先决条件

—安装1.18及以上版本
—安装库：`go get github.com/gomarkdown/markdown`—命令行工具：`go install github.com/gomarkdown/mdtohtml@latest`快速转换方法

####方法1：简单转换（Go）```go
package main

import (
    "fmt"
    "github.com/gomarkdown/markdown"
)

func main() {
    md := []byte("# Hello World\n\nThis is **bold** text.")
    html := markdown.ToHTML(md, nil, nil)
    fmt.Println(string(html))
}
```
####方法二：CLI Tool```bash
# Install mdtohtml
go install github.com/gomarkdown/mdtohtml@latest

# Convert file
mdtohtml input.md output.html

# Convert file (output to stdout)
mdtohtml input.md
```
####方法3：自定义解析器和渲染器```go
package main

import (
    "github.com/gomarkdown/markdown"
    "github.com/gomarkdown/markdown/html"
    "github.com/gomarkdown/markdown/parser"
)

func mdToHTML(md []byte) []byte {
    // Create parser with extensions
    extensions := parser.CommonExtensions | parser.AutoHeadingIDs | parser.NoEmptyLineBeforeBlock
    p := parser.NewWithExtensions(extensions)
    doc := p.Parse(md)

    // Create HTML renderer with extensions
    htmlFlags := html.CommonFlags | html.HrefTargetBlank
    opts := html.RendererOptions{Flags: htmlFlags}
    renderer := html.NewRenderer(opts)

    return markdown.Render(doc, renderer)
}
```
### CLI配置`mdtohtml`命令行工具有最小的选项：```bash
mdtohtml input-file [output-file]
```
对于高级配置，使用Go库的解析器和渲染器选项：

|解析器扩展|描述||------------------|-------------|
|`parser.CommonExtensions`|表，围栏代码，自动链接，划线等|
|`parser.AutoHeadingIDs`|生成标题|的id
|`parser.NoEmptyLineBeforeBlock`|块|之前不需要空行`parser.MathJax`| MathJax支持LaTeX数学|

| HTML标志|描述||-----------|-------------|
|`html.CommonFlags`|普通HTML输出标志|
|`html.HrefTargetBlank`|添加`target="_blank"`到链接|
|生成完整的HTML页面|
|`html.UseXHTML`|生成XHTML输出|

安全警告

⚠️** gommarkdown不清理输出的HTML。**对于不可信的输入，使用blumonday：```go
import (
    "github.com/microcosm-cc/bluemonday"
    "github.com/gomarkdown/markdown"
)

maybeUnsafeHTML := markdown.ToHTML(md, nil, nil)
html := bluemonday.UGCPolicy().SanitizeBytes(maybeUnsafeHTML)
```
推荐消毒剂：[Bluemonday]（https://github.com/microcosm-cc/bluemonday）

支持Markdown口味

|风味|支持||--------|---------|
|原价下调| 100% |
| CommonMark |高（带扩展）|
GitHub风味Markdown |高（表，围栏代码，突破）|
|MathJax/LaTeX数学|支持扩展|
| Mmark |支持|

# # #故障排除

|问题|解决方案||-------|----------|
|Windows/Mac未解析换行|使用`parser.NormalizeNewlines(input)`|
|启用`parser.Tables`扩展|
|代码块没有突出显示|集成与语法高亮，如Chroma |
|启用`parser.MathJax`扩展|
XSS漏洞|使用blumonday对输出|进行消毒

##使用[`jekyll`]（references/jekyll.md）

# # #先决条件

—Ruby 2.7.0及以上版本
——RubyGems
- GCC和Make（用于本地扩展）
—安装Jekyll和Bundler:`gem install jekyll bundler`快速转换方法

####方法一：新建站点```bash
# Create a new Jekyll site
jekyll new myblog

# Change to site directory
cd myblog

# Build and serve locally
bundle exec jekyll serve

# Access at http://localhost:4000
```
####方法2：建立静态站点```bash
# Build site to _site directory
bundle exec jekyll build

# Build with production environment
JEKYLL_ENV=production bundle exec jekyll build
```
####方法3：实时重新加载开发```bash
# Serve with live reload
bundle exec jekyll serve --livereload

# Serve with drafts
bundle exec jekyll serve --drafts
```
### CLI配置

|命令|描述||---------|-------------|
|`jekyll new <path>`|创建新的Jekyll站点|
|`jekyll build`|建立站点到`_site`目录|
|`jekyll serve`|构建并本地服务|
|`jekyll clean`|删除生成的文件|
|`jekyll doctor`|检查配置问题|

|服务选项|描述||---------------|-------------|
|`--livereload`|重新加载浏览器更改|
|`--drafts`|包括草稿|
|`--port <port>`|设置服务器端口（默认为4000）|
|`--host <host>`|设置服务器主机（默认为localhost） |
|`--baseurl <url>`|设置基础URL |

安全警告

⚠️**杰基尔安全注意事项：**

—避免在生产中使用`safe: false`—使用“`_config.yml`”中的“`exclude`”，防止敏感文件被发布
-消毒用户生成的内容，如果接受外部输入
-保持Jekyll和插件更新```yaml
# _config.yml security settings
exclude:
  - Gemfile
  - Gemfile.lock
  - node_modules
  - vendor
```
支持Markdown口味

|风味|支持||--------|---------|
| Kramdown（默认）| 100% |
通过插件（jekyll-commonmark） |
GitHub风味Markdown |通过插件（jekyll-commonmark-ghpages） |
RedCarpet | Via插件(已弃用

在`_config.yml`配置markdown处理器：```yaml
markdown: kramdown
kramdown:
  input: GFM
  syntax_highlighter: rouge
```
# # #故障排除

|问题|解决方案||-------|----------|
| Ruby 3.0+服务|失败执行`bundle add webrick`|
|执行`bundle install`|
|使用`--incremental`标志|
|检查内容|中未转义的`{`添加到`_config.yml`插件列表|

使用[`hugo`]（references/hugo.md）

# # #先决条件

-安装Hugo（从<https://gohugo.io/installation/>下载）
- Git（推荐用于主题和模块）
- Go（可选，用于Hugo模块）

快速转换方法

####方法一：新建站点```bash
# Create a new Hugo site
hugo new site mysite

# Change to site directory
cd mysite

# Add a theme
git init
git submodule add https://github.com/theNewDynamic/gohugo-theme-ananke themes/ananke
echo "theme = 'ananke'" >> hugo.toml

# Create content
hugo new content posts/my-first-post.md

# Start development server
hugo server -D
```
####方法2：建立静态站点```bash
# Build site to public directory
hugo

# Build with minification
hugo --minify

# Build for specific environment
hugo --environment production
```
####方法三：开发服务器```bash
# Start server with drafts
hugo server -D

# Start with live reload and bind to all interfaces
hugo server --bind 0.0.0.0 --baseURL http://localhost:1313/

# Start with specific port
hugo server --port 8080
```
### CLI配置

|命令|描述||---------|-------------|
|`hugo new site <name>`|创建新的Hugo站点|
|`hugo new content <path>`|创建新的内容文件|
|`hugo`|建立站点到`public`目录|
|`hugo server`|启动开发服务器|
|`hugo mod init`|初始化Hugo Modules |

|构建选项|描述||---------------|-------------|
|`-D, --buildDrafts`|包含草案内容|
|`-E, --buildExpired`|包含过期内容|
|`-F, --buildFuture`|包含未来日期的内容|
|`--minify`|缩小输出|
|`--gc`|在构建|后运行垃圾收集
|`-d, --destination <path>`|输出目录|

|服务器选项|描述||----------------|-------------|
|`--bind <ip>`|绑定|接口
|`-p, --port <port>`|端口号，默认为1313
|`--liveReloadPort <port>`|实时加载端口|
|`--disableLiveReload`|关闭实时加载|
|`--navigateToChanged`|导航到更改内容|

安全警告

⚠️**雨果安全注意事项：**

—外部命令在“`hugo.toml`”中配置安全策略
—在公共存储库中谨慎使用`--enableGitInfo`-验证用户生成内容的短码参数```toml
# hugo.toml security settings
[security]
  enableInlineShortcodes = false
  [security.exec]
    allow = ['^go$', '^npx$', '^postcss$']
  [security.funcs]
    getenv = ['^HUGO_', '^CI$']
  [security.http]
    methods = ['(?i)GET|POST']
    urls = ['.*']
```
支持Markdown口味

|风味|支持||--------|---------|
| Goldmark（默认）| 100% （CommonMark兼容）|
| GitHub风味降价|满（表，划线，自动链接）|
| CommonMark | 100% |
|黑色星期五（legacy） |已弃用，不推荐|

在`hugo.toml`中配置降价：```toml
[markup]
  [markup.goldmark]
    [markup.goldmark.extensions]
      definitionList = true
      footnote = true
      linkify = true
      strikethrough = true
      table = true
      taskList = true
    [markup.goldmark.renderer]
      unsafe = false  # Set true to allow raw HTML
```
# # #故障排除

|问题|解决方案||-------|----------|
| "Page not found" on paths |检查配置|中的`baseURL`|主题未加载|验证主题在`themes/`或Hugo模块|
|缓慢构建|使用`--templateMetrics`识别瓶颈|
|在goldmark config |中设置`unsafe = true`检查`static/`文件夹结构|
|模块错误|执行`hugo mod tidy`|

# #引用

文字和样式标记

——[basic-markdown.md] (references/basic-markdown.md)
——[code-blocks.md] (references/code-blocks.md)
——[collapsed-sections.md] (references/collapsed-sections.md)
——[tables.md] (references/tables.md)
——[writing-mathematical-expressions.md] (references/writing-mathematical-expressions.md)
-降价指南：<https://www.markdownguide.org/basic-syntax/>-样式降价：<https://github.com/sindresorhus/github-markdown-css># # # (`markedJS/marked`) (references/marked.md)

—官方文档：<https://marked.js.org/>—高级选项：<https://marked.js.org/using_advanced>—可扩展性：<https://marked.js.org/using_pro>- GitHub存储库：<https://github.com/markedjs/marked># # # (`pandoc`) (references/pandoc.md)

—入门：<https://pandoc.org/getting-started.html>—官方文档：<https://pandoc.org/MANUAL.html>—可扩展性：<https://pandoc.org/extras.html>- GitHub存储库：<https://github.com/jgm/pandoc># # # (`gomarkdown/markdown`) (references/gomarkdown.md)—官方文档：<https://pkg.go.dev/github.com/gomarkdown/markdown>—高级配置：<https://pkg.go.dev/github.com/gomarkdown/markdown@v0.0.0-20250810172220-2e2c11897d1a/html>-降价加工：<https://blog.kowalczyk.info/article/cxn3/advanced-markdown-processing-in-go.html>- GitHub存储库：<https://github.com/gomarkdown/markdown># # # (`jekyll`) (references/jekyll.md)

—官方文档：<https://jekyllrb.com/docs/>—配置选项：<https://jekyllrb.com/docs/configuration/options/>—插件：<https://jekyllrb.com/docs/plugins/>-(安装)(https://jekyllrb.com/docs/plugins/installation/)
-(发电机)(https://jekyllrb.com/docs/plugins/generators/)
-(转换器)(https://jekyllrb.com/docs/plugins/converters/)
-【命令】(https://jekyllrb.com/docs/plugins/commands/)
——[标记](https://jekyllrb.com/docs/plugins/tags/)
-(过滤器)(https://jekyllrb.com/docs/plugins/filters/)
-(钩)(https://jekyllrb.com/docs/plugins/hooks/)
- GitHub存储库：<https://github.com/jekyll/jekyll># # # (`hugo`) (references/hugo.md)

-官方文档：<https://gohugo.io/documentation/>—所有设置：“<https://gohugo.io/configuration/all/>”
-编辑插件：<https://gohugo.io/tools/editors/>- GitHub存储库：<https://github.com/gohugoio/hugo>