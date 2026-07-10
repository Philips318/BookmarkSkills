#Awesome GitHub Copilot网站

天文+星光网站发布到<https://awesome-copilot.github.com/>。

##本地发展

从**存储库根**运行这些（它们首先生成站点需要的数据）：```bash
npm run website:data    # generate public/data/*.json from repo content
npm run website:dev     # generate data + start the dev server
npm run website:build   # full production build
```
# #可访问性

该网站有一个自动的斧头核心+剧作家审计。在本地使用`npm run website:a11y`从存储库根目录运行它，或者在先构建`dist`之后从`website/`运行`npm run a11y`。

CI阻止严重的违规行为。轻度和中度最佳实践问题报告为非阻塞。

编写约定：资源卡使用`div[role="listitem"]`包装，而不是`<article>`；仅将`role="list"`添加到其直接子元素为列表项的容器中；不要将交互控件嵌套在另一个可聚焦的元素中；`.btn-primary`和ToC链接必须满足WCAG AA（4.5:1）明暗主题的对比度。

社交预览卡（LinkedIn等）

共享链接呈现为由Open Graph / Twitter元标签驱动的大型预览卡。
LinkedIn（和大多数平台）读成**Open Graph**——主要是`og:image`——而Twitter/X也使用`twitter:card=summary_large_image`。大多数标签是自动生成的：- **星光默认**发射`og:title`，`og:description`,`og:url`,`og:type`，`og:site_name`和`twitter:card=summary_large_image`。
**`astro.config.mjs`**（全局`head`）发出共享图像标签：`og:image`，`og:image:width`,`og:image:height`，`og:image:alt`和`twitter:image`。
**`src/components/Head.astro`**添加`twitter:title`/`description`，`og:image:secure_url`，`og:image:type`和`twitter:image:alt`。

每个页面的`title`和`description`（StarlightPage首页）流入卡片文本，
所以要保持目标清晰，以利益为中心。

图像维不变量`og:image:width`/`og:image:height`在`astro.config.mjs`中描述`public/images/social-image.png`（目前**2400×1260**， ~1.91:1）。爬虫使用这些维度来理解图像和
可以在selecting/rendering预览时使用它们。如果您交换图像或添加逐页图像
覆盖，更新**full**图像集，使每个标签保持一致：`og:image`，`og:image:width`,`og:image:height`，`og:image:alt`和`twitter:image`（最后一个很重要）
因为`Head.astro`首先从`twitter:image`衍生出`og:image:secure_url`)。###部署后

LinkedIn积极缓存刮痕。要强制刷新并确认显卡呈现，请运行
通过[LinkedIn Post Inspector]（https://www.linkedin.com/post-inspector/）更改URL。
单独的HTML输出并不能证明活动卡——验证部署的映像返回HTTP 200
HTTPS与`Content-Type: image/png`和无认证。