#雨果参考

Hugo是世界上最快的静态站点生成器。它可以在几毫秒内建立网站，并支持高级内容管理功能。

# #安装

# # #窗口```powershell
# Using Chocolatey
choco install hugo-extended

# Using Scoop
scoop install hugo-extended

# Using Winget
winget install Hugo.Hugo.Extended
```
# # # macOS```bash
# Using Homebrew
brew install hugo
```
# # # Linux```bash
# Debian/Ubuntu (snap)
snap install hugo --channel=extended

# Using package manager (may not be latest)
sudo apt-get install hugo

# Or download from https://gohugo.io/installation/
```
##快速入门

创建新站点```bash
# Create site
hugo new site mysite
cd mysite

# Initialize git and add theme
git init
git submodule add https://github.com/theNewDynamic/gohugo-theme-ananke themes/ananke
echo "theme = 'ananke'" >> hugo.toml

# Create first post
hugo new content posts/my-first-post.md

# Start development server
hugo server -D
```
目录结构```
mysite/
├── archetypes/      # Content templates
│   └── default.md
├── assets/          # Assets to process (SCSS, JS)
├── content/         # Markdown content
│   └── posts/
├── data/            # Data files (YAML, JSON, TOML)
├── i18n/            # Internationalization
├── layouts/         # Templates
│   ├── _default/
│   ├── partials/
│   └── shortcodes/
├── static/          # Static files (copied as-is)
├── themes/          # Themes
└── hugo.toml        # Configuration
```
命令行命令

|命令|描述||---------|-------------|
|`hugo new site <name>`|创建新站点|
|`hugo new content <path>`|创建内容文件|
|`hugo`|构建到`public/`|
|`hugo server`|启动开发服务器|
|`hugo mod init`|初始化Hugo Modules |
|`hugo mod tidy`|清理|模块

###构建选项```bash
# Basic build
hugo

# Build with minification
hugo --minify

# Build with drafts
hugo -D

# Build for specific environment
hugo --environment production

# Build to custom directory
hugo -d ./dist

# Verbose output
hugo -v
```
###服务器选项```bash
# Start with drafts
hugo server -D

# Bind to all interfaces
hugo server --bind 0.0.0.0

# Custom port
hugo server --port 8080

# Disable live reload
hugo server --disableLiveReload

# Navigate to changed content
hugo server --navigateToChanged
```
##配置（hugo.toml）```toml
# Basic settings
baseURL = 'https://example.com/'
languageCode = 'en-us'
title = 'My Hugo Site'
theme = 'ananke'

# Build settings
[build]
  writeStats = true

# Markdown configuration
[markup]
  [markup.goldmark]
    [markup.goldmark.extensions]
      definitionList = true
      footnote = true
      linkify = true
      strikethrough = true
      table = true
      taskList = true
    [markup.goldmark.parser]
      autoHeadingID = true
      autoHeadingIDType = 'github'
    [markup.goldmark.renderer]
      unsafe = false
  [markup.highlight]
    style = 'monokai'
    lineNos = true

# Taxonomies
[taxonomies]
  category = 'categories'
  tag = 'tags'
  author = 'authors'

# Menus
[menus]
  [[menus.main]]
    name = 'Home'
    pageRef = '/'
    weight = 10
  [[menus.main]]
    name = 'Posts'
    pageRef = '/posts'
    weight = 20

# Parameters
[params]
  description = 'My awesome site'
  author = 'John Doe'
```
##正面问题

Hugo支持TOML、YAML和JSON前端内容：

### TOML（默认）```markdown
+++
title = 'My First Post'
date = 2025-01-28T12:00:00-05:00
draft = false
tags = ['hugo', 'tutorial']
categories = ['blog']
author = 'John Doe'
+++

Content here...
```
# # # YAML```markdown
---
title: "My First Post"
date: 2025-01-28T12:00:00-05:00
draft: false
tags: ["hugo", "tutorial"]
---

Content here...
```
# #模板

基本模板（_default/baseof.html）```html
<!DOCTYPE html>
<html>
<head>
  <title>{{ .Title }} | {{ .Site.Title }}</title>
  {{ partial "head.html" . }}
</head>
<body>
  {{ partial "header.html" . }}
  <main>
    {{ block "main" . }}{{ end }}
  </main>
  {{ partial "footer.html" . }}
</body>
</html>
```
###单页（_default/single.html）```html
{{ define "main" }}
<article>
  <h1>{{ .Title }}</h1>
  <time>{{ .Date.Format "January 2, 2006" }}</time>
  {{ .Content }}
</article>
{{ end }}
```
###列表页面（_default/list.html）```html
{{ define "main" }}
<h1>{{ .Title }}</h1>
{{ range .Pages }}
  <article>
    <h2><a href="{{ .Permalink }}">{{ .Title }}</a></h2>
    <p>{{ .Summary }}</p>
  </article>
{{ end }}
{{ end }}
```
# #短码

内置短代码```markdown
{{< figure src="/images/photo.jpg" title="My Photo" >}}

{{< youtube dQw4w9WgXcQ >}}

{{< gist user 12345 >}}

{{< highlight go >}}
fmt.Println("Hello")
{{< /highlight >}}
```
自定义短代码（layouts/shortcodes/alert.html）```html
<div class="alert alert-{{ .Get "type" | default "info" }}">
  {{ .Inner | markdownify }}
</div>
```
用法:```markdown
{{< alert type="warning" >}}
**Warning:** This is important!
{{< /alert >}}
```
##内容组织

###页面包```
content/
├── posts/
│   └── my-post/           # Page bundle
│       ├── index.md       # Content
│       └── image.jpg      # Resources
└── _index.md              # Section page
```
###访问资源```html
{{ $image := .Resources.GetMatch "image.jpg" }}
{{ with $image }}
  <img src="{{ .RelPermalink }}" alt="...">
{{ end }}
```
Hugo Pipes（资产处理）

SCSS编译```html
{{ $styles := resources.Get "scss/main.scss" | toCSS | minify }}
<link rel="stylesheet" href="{{ $styles.RelPermalink }}">
```
### JavaScript捆绑```html
{{ $js := resources.Get "js/main.js" | js.Build | minify }}
<script src="{{ $js.RelPermalink }}"></script>
```
# #分类法

# # #配置```toml
[taxonomies]
  tag = 'tags'
  category = 'categories'
```
###在前面使用```markdown
+++
tags = ['go', 'hugo']
categories = ['tutorials']
+++
```
###列出分类术语```html
{{ range .Site.Taxonomies.tags }}
  <a href="{{ .Page.Permalink }}">{{ .Page.Title }} ({{ .Count }})</a>
{{ end }}
```
多语言网站```toml
defaultContentLanguage = 'en'

[languages]
  [languages.en]
    title = 'My Site'
    weight = 1
  [languages.es]
    title = 'Mi Sitio'
    weight = 2
```
# #故障排除

|问题|解决方案||-------|----------|
|未找到|检查`baseURL`配置|
|主题未加载|检查配置|的主题路径
|原始HTML不显示|设置`unsafe = true`在goldmark配置|
|使用`--templateMetrics`调试|
|模块错误|执行`hugo mod tidy`|
| CSS未更新|清除浏览器缓存或使用指纹识别|

# #资源

- [Hugo文档]（https://gohugo.io/documentation/）
-[雨果主题]（https://themes.gohugo.io/）
——[雨果演讲]（https://discourse.gohugo.io/）
- [GitHub Repository]（https://github.com/gohugoio/hugo）
-[快速参考]（https://gohugo.io/quick-reference/）