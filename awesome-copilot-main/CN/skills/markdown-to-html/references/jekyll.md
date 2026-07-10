# Jekyll参考

Jekyll是一个静态网站生成器，可以将Markdown内容转换为完整的网站。它具有博客意识，并支持GitHub Pages。

# #安装

# # #先决条件

—Ruby 2.7.0及以上版本
——RubyGems
- GCC和Make

###安装杰基尔```bash
# Install Jekyll and Bundler
gem install jekyll bundler
```
特定于平台的安装```bash
# macOS (install Xcode CLI tools first)
xcode-select --install
gem install jekyll bundler

# Ubuntu/Debian
sudo apt-get install ruby-full build-essential zlib1g-dev
gem install jekyll bundler

# Windows (use RubyInstaller)
# Download from https://rubyinstaller.org/
gem install jekyll bundler
```
##快速入门

创建新站点```bash
# Create new Jekyll site
jekyll new myblog

# Navigate to site
cd myblog

# Build and serve
bundle exec jekyll serve

# Open http://localhost:4000
```
目录结构```
myblog/
├── _config.yml      # Site configuration
├── _posts/          # Blog posts
│   └── 2025-01-28-welcome.md
├── _layouts/        # Page templates
├── _includes/       # Reusable components
├── _data/           # Data files (YAML, JSON, CSV)
├── _sass/           # Sass partials
├── assets/          # CSS, JS, images
├── index.md         # Home page
└── Gemfile          # Ruby dependencies
```
命令行命令

|命令|描述||---------|-------------|
|`jekyll new <name>`|创建新站点|
|`jekyll build`|构建到`_site/`|
|`jekyll serve`|构建并本地服务|
|`jekyll clean`|删除生成的文件|
|`jekyll doctor`|检查问题|

###构建选项```bash
# Build site
bundle exec jekyll build

# Build with production environment
JEKYLL_ENV=production bundle exec jekyll build

# Build to custom directory
bundle exec jekyll build --destination ./public

# Build with incremental regeneration
bundle exec jekyll build --incremental
```
###服务选项```bash
# Serve with live reload
bundle exec jekyll serve --livereload

# Include draft posts
bundle exec jekyll serve --drafts

# Specify port
bundle exec jekyll serve --port 8080

# Bind to all interfaces
bundle exec jekyll serve --host 0.0.0.0
```
##配置（_config.yml）```yaml
# Site settings
title: My Blog
description: A great blog
baseurl: ""
url: "https://example.com"

# Build settings
markdown: kramdown
theme: minima
plugins:
  - jekyll-feed
  - jekyll-seo-tag

# Kramdown settings
kramdown:
  input: GFM
  syntax_highlighter: rouge
  hard_wrap: false

# Collections
collections:
  docs:
    output: true
    permalink: /docs/:name/

# Defaults
defaults:
  - scope:
      path: ""
      type: "posts"
    values:
      layout: "post"

# Exclude from processing
exclude:
  - Gemfile
  - Gemfile.lock
  - node_modules
  - vendor
```
##正面问题

每个内容文件都需要YAML前置事项：```markdown
---
layout: post
title: "My First Post"
date: 2025-01-28 12:00:00 -0500
categories: blog tutorial
tags: [jekyll, markdown]
author: John Doe
excerpt: "A brief introduction..."
published: true
---

Your content here...
```
##降低处理器

Kramdown（默认）```yaml
# _config.yml
markdown: kramdown
kramdown:
  input: GFM                    # GitHub Flavored Markdown
  syntax_highlighter: rouge
  syntax_highlighter_opts:
    block:
      line_numbers: true
```
# # # CommonMark```ruby
# Gemfile
gem 'jekyll-commonmark-ghpages'
```

```yaml
# _config.yml
markdown: CommonMarkGhPages
commonmark:
  options: ["SMART", "FOOTNOTES"]
  extensions: ["strikethrough", "autolink", "table"]
```
液体模板

# # #变量```liquid
{{ page.title }}
{{ site.title }}
{{ content }}
{{ page.date | date: "%B %d, %Y" }}
```
# # #循环```liquid
{% for post in site.posts %}
  <article>
    <h2><a href="{{ post.url }}">{{ post.title }}</a></h2>
    <p>{{ post.excerpt }}</p>
  </article>
{% endfor %}
```
# # #条件```liquid
{% if page.title %}
  <h1>{{ page.title }}</h1>
{% endif %}

{% unless page.draft %}
  {{ content }}
{% endunless %}
```
# # #包括```liquid
{% include header.html %}
{% include footer.html param="value" %}
```
# #布局

基本布局（_layouts/default.html）```html
<!DOCTYPE html>
<html>
<head>
  <title>{{ page.title }} | {{ site.title }}</title>
  <link rel="stylesheet" href="{{ '/assets/css/style.css' | relative_url }}">
</head>
<body>
  {% include header.html %}
  <main>
    {{ content }}
  </main>
  {% include footer.html %}
</body>
</html>
```
### Post Layout （_layouts/post.html）```html
---
layout: default
---
<article>
  <h1>{{ page.title }}</h1>
  <time>{{ page.date | date: "%B %d, %Y" }}</time>
  {{ content }}
</article>
```
# #插件

###常用插件```ruby
# Gemfile
group :jekyll_plugins do
  gem 'jekyll-feed'        # RSS feed
  gem 'jekyll-seo-tag'     # SEO meta tags
  gem 'jekyll-sitemap'     # XML sitemap
  gem 'jekyll-paginate'    # Pagination
  gem 'jekyll-archives'    # Archive pages
end
```
###使用插件```yaml
# _config.yml
plugins:
  - jekyll-feed
  - jekyll-seo-tag
  - jekyll-sitemap
```
# #故障排除

|问题|解决方案||-------|----------|
| Ruby 3.0+ webrick错误|`bundle add webrick`|
|使用`--user-install`或rbenv |
|缓慢构建|使用`--incremental`|
|液体错误|检查未转义`{``}`|
|添加`encoding: utf-8`配置|
|插件不加载|添加到Gemfile和_config.yml|

# #资源

- [Jekyll文档]（https://jekyllrb.com/docs/）
-[液态模板语言]（https://shopify.github.io/liquid/）
- [Kramdown文档]（https://kramdown.gettalong.org/）
- [GitHub Repository]（https://github.com/jekyll/jekyll）
- [Jekyll主题]（https://jekyllthemes.io/）