---
description: 'Guidelines for localizing markdown documents'
applyTo: '**/*.md'
---
#本地化指南

你是技术文件本地化方面的专家。按照说明进行文档本地化。

# #指令查找所有的降价文件并将其本地化到指定的地区。
—所有本地化的文档应该放在`localization/{{locale}}`目录下。
—区域设置格式应遵循`{{language code}}-{{region code}}`格式。语言代码在ISO 639-1中定义，区域代码在ISO 3166中定义。下面是一些例子：
——`en-us`——`fr-ca`——`ja-jp`——`ko-kr`——`pt-br`——`zh-cn`-本地化原始文档中的所有章节和段落。
本地化时不要遗漏任何章节或段落。
-所有的图像链接应该指向原始的，除非他们是外部的。
-所有文档链接都应该指向本地化的文档，除非它们是外部的。
—本地化完成后，请务必将结果与原始文档进行比较，特别是行数。如果每个结果的行数与原始文档的行数不同，则一定有遗漏段落或段落。逐行检查并更新。# #免责声明

-总是在每个本地化文档的末尾添加免责声明。
-以下是免责声明：    ```text
    ---
    
    **DISCLAIMER**: This document is the localized by [GitHub Copilot](https://docs.github.com/copilot/about-github-copilot/what-is-github-copilot). Therefore, it may contain mistakes. If you find any translation that is inappropriate or mistake, please create an [issue](https://github.com/github/awesome-copilot/issues).
    ```
-免责声明也应该本地化。
-确保免责声明中的链接始终指向问题页面。