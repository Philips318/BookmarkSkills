#gomarkdown/markdown参考

用于解析Markdown和呈现HTML的Go库。快速、可扩展和线程安全。

# #安装```bash
# Add to your Go project
go get github.com/gomarkdown/markdown

# Install CLI tool
go install github.com/gomarkdown/mdtohtml@latest
```
##基本用法

简单转换```go
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
使用命令行工具```bash
# Convert file to HTML
mdtohtml input.md output.html

# Output to stdout
mdtohtml input.md
```
##解析器配置

###常用扩展```go
import (
    "github.com/gomarkdown/markdown"
    "github.com/gomarkdown/markdown/parser"
)

// Create parser with extensions
extensions := parser.CommonExtensions | parser.AutoHeadingIDs
p := parser.NewWithExtensions(extensions)

// Parse markdown
doc := p.Parse(md)
```
可用的解析器扩展

|扩展名|描述信息||-----------|-------------|
|`parser.CommonExtensions`|表，围栏代码，自动链接，罢工|
|`parser.Tables`|管道表支持|
|`parser.FencedCode`|用语言|隔离代码块
|`parser.Autolink`|自动检测url |
|`parser.Strikethrough`| ~~划线~~文本|
|`parser.SpaceHeadings`|在标题|的#后面需要空格
|`parser.HeadingIDs`|自定义标题id {#id} |
|`parser.AutoHeadingIDs`|自动生成标题id |
|`parser.Footnotes`|脚注支持|
|`parser.NoEmptyLineBeforeBlock`| |块前不需要空行
|`parser.HardLineBreak`|换行变成`<br>`|
|`parser.MathJax`| MathJax支持|
|`parser.SuperSubscript`|上^脚本^和下标~脚本~ |
|`parser.Mmark`| Mmark语法支持|

## HTML渲染器配置

###通用标志```go
import (
    "github.com/gomarkdown/markdown"
    "github.com/gomarkdown/markdown/html"
    "github.com/gomarkdown/markdown/parser"
)

// Parser
p := parser.NewWithExtensions(parser.CommonExtensions)

// Renderer
htmlFlags := html.CommonFlags | html.HrefTargetBlank
opts := html.RendererOptions{
    Flags: htmlFlags,
    Title: "My Document",
    CSS: "style.css",
}
renderer := html.NewRenderer(opts)

// Convert
html := markdown.ToHTML(md, p, renderer)
```
可用的HTML标志

|标志位|描述||------|-------------|
|`html.CommonFlags`|一般默认为|
|`html.HrefTargetBlank`|添加`target="_blank"`到链接|
|`html.CompletePage`|生成完整HTML文档|
|`html.UseXHTML`|使用XHTML输出|
|`html.FootnoteReturnLinks`|在脚注中添加返回链接|
|`html.FootnoteNoHRTag`|脚注前无`<hr>`|
聪明的标点符号|
智能分数（1/2→½）|
|`html.SmartypantsDashes`|智能破折号（——→-）|
|`html.SmartypantsLatexDashes`|乳胶风格的破折号|

Renderer选项```go
opts := html.RendererOptions{
    Flags:          htmlFlags,
    Title:          "Document Title",
    CSS:            "path/to/style.css",
    Icon:           "favicon.ico",
    Head:           []byte("<meta name='author' content='...'>"),
    RenderNodeHook: customRenderHook,
}
```
##完整示例```go
package main

import (
    "os"
    "github.com/gomarkdown/markdown"
    "github.com/gomarkdown/markdown/html"
    "github.com/gomarkdown/markdown/parser"
)

func mdToHTML(md []byte) []byte {
    // Parser with extensions
    extensions := parser.CommonExtensions | 
                  parser.AutoHeadingIDs | 
                  parser.NoEmptyLineBeforeBlock
    p := parser.NewWithExtensions(extensions)
    doc := p.Parse(md)

    // HTML renderer with options
    htmlFlags := html.CommonFlags | html.HrefTargetBlank
    opts := html.RendererOptions{Flags: htmlFlags}
    renderer := html.NewRenderer(opts)

    return markdown.Render(doc, renderer)
}

func main() {
    md, _ := os.ReadFile("input.md")
    html := mdToHTML(md)
    os.WriteFile("output.html", html, 0644)
}
```
安全性：对输出进行消毒

**重要：** gomarkdown不会清理HTML输出。对于不受信任的输入使用blumonday：```go
import (
    "github.com/microcosm-cc/bluemonday"
    "github.com/gomarkdown/markdown"
)

// Convert markdown to potentially unsafe HTML
unsafeHTML := markdown.ToHTML(md, nil, nil)

// Sanitize using Bluemonday
p := bluemonday.UGCPolicy()
safeHTML := p.SanitizeBytes(unsafeHTML)
```
###蓝色星期一政策

|策略|描述||--------|-------------|
|`UGCPolicy()`|用户生成内容（最常见）|
|`StrictPolicy()`|删除所有HTML |
|`StripTagsPolicy()`|去掉标签，保留文本|
|`NewPolicy()`|构建自定义策略|

##使用AST

###访问AST```go
import (
    "github.com/gomarkdown/markdown/ast"
    "github.com/gomarkdown/markdown/parser"
)

p := parser.NewWithExtensions(parser.CommonExtensions)
doc := p.Parse(md)

// Walk the AST
ast.WalkFunc(doc, func(node ast.Node, entering bool) ast.WalkStatus {
    if heading, ok := node.(*ast.Heading); ok && entering {
        fmt.Printf("Found heading level %d\n", heading.Level)
    }
    return ast.GoToNext
})
```
自定义渲染器```go
type MyRenderer struct {
    *html.Renderer
}

func (r *MyRenderer) RenderNode(w io.Writer, node ast.Node, entering bool) ast.WalkStatus {
    // Custom rendering logic
    if heading, ok := node.(*ast.Heading); ok && entering {
        fmt.Fprintf(w, "<h%d class='custom'>", heading.Level)
        return ast.GoToNext
    }
    return r.Renderer.RenderNode(w, node, entering)
}
```
##处理换行

Windows和Mac的换行符需要规范化：```go
// Normalize newlines before parsing
normalized := parser.NormalizeNewlines(input)
html := markdown.ToHTML(normalized, nil, nil)
```
# #资源

- [Package Documentation]（https://pkg.go.dev/github.com/gomarkdown/markdown）
-[高级加工指南]（https://blog.kowalczyk.info/article/cxn3/advanced-markdown-processing-in-go.html）
- [GitHub Repository]（https://github.com/gomarkdown/markdown）
—[CLI Tool]（https://github.com/gomarkdown/mdtohtml）
- [Bluemonday消毒剂]（https://github.com/microcosm-cc/bluemonday）