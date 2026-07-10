---
name: content-management-systems
description: 'Workflow for building and modifying content management systems across WordPress, Shopify, Wix, Squarespace, Drupal, WooCommerce, Joomla, HubSpot CMS Hub, Webflow, Adobe Experience Manager, and similar platforms. Use when working on CMS themes, plugins, apps, modules, admin panels, media uploads, content models, editors, markdown pipelines, or static export workflows.'
---
#内容管理系统

当用户在内容管理系统或类似于内容管理系统的软件上工作时，使用此技能。

该技能主要关注CMS工作中重要的接缝：

-主题和模板
-插件，应用程序，模块和扩展
—管理员和编辑器界面
-媒体和上传处理
-内容模型、分类法和元数据
-渲染管道和静态导出流

何时使用此技能-用户提到CMS平台，如WordPress， Shopify, Drupal, Joomla, Webflow, Squarespace, Wix, WooCommerce， HubSpot CMS Hub或Adobe Experience Manager。
-该任务是关于主题开发，模板更改，或设计系统工作在CMS内。
-任务是关于插件，模块，应用程序或扩展点。
—该任务涉及编辑器UX、预览、分类法、slugs、SEO字段或发布行为。
—该任务包括上传、媒体库、创作资产、降价渲染或静态导出。

第一次通过1. 确定平台类别：自托管CMS、SaaS站点构建器、商务平台或hybrid/headless系统。
2. 在编辑之前找到所属的实现接缝：
-主题或模板层
-插件、应用、模块或扩展层
- admin或editor界面
-内容模型或存储层
-媒体管道
-导出、部署或呈现管道
3. 在选择方法之前检查平台约束：
-什么是可编辑的本地
-什么是创作内容与代码
-媒体的归属
-最终站点是服务器渲染、静态导出还是远程托管

CMS规则-遵循平台对主题、模块、模板部分或部分的命名和文件夹约定。
-保持主题资产与用户上传的媒体分开，除非平台明确地将它们合并。
-优先选择结构化内容字段，而不是在表示标记中存储重要的元数据。
-将预览、slugs、分类法、摘要、元字段和发布状态视为一级CMS关注点。
—当配置、主题选择或内容输入无效时，首选安全的默认值和优雅的回退行为。
-当更改编辑器或管理行为时，跟踪存储字段，验证规则，预览路径和最终渲染路径。

##通用工作流

主题和模板-从模板加载器或主题运行时开始，而不是从下游包含开始。
-保留平台的模板层次结构和部分命名约定。
-保持演示更改接近模板和共享主题帮助程序。

插件、应用和模块

-在平台的扩展缝中添加行为，而不是将逻辑分散到模板中。
保持迁移、种子数据和配置更新的明确和版本化。
-当平台需要激活或注册时，记录扩展的设置假设。

管理和编辑UX

保持表单与存储的内容模型一致。
-当内容转换非常重要时，更喜欢面向作者的预览。
-保持验证、CSRF或同等的安全措施，以及与周围管理代码一致的权限。

###媒体和上传—授权媒体使用专用上传路径。
-在活动主题文件夹中保留装饰性或主题所属的图像。
-默认为传统的位置，如`uploads/`为创作媒体和`img/`为主题资产，除非平台要求更强的约定。
—当CMS支持可配置的媒体目录时，使用安全回退暴露设置。

内容模型和迁移

—清晰区分内容实体：页面、帖子、产品、条目、集合、分类和设置。
-首选迁移文件或可导出的模式定义，而不是临时运行时突变。
保持slugs、发布日期、摘要、规范元数据和分类关系的结构化。

### Markdown， HTML和静态导出-在更改渲染器之前，确定markdown是撰写输入，中间内容还是构建输出。
-当可行时，将渲染器更改与预览或验证配对。
-对于静态导出的CMS系统，在构建更改后验证重写的永久链接和资产路径。

识别归属接缝

不管平台是什么，在编辑之前，通过将代码库映射到这些CMS角色来定位拥有的seam：

-运行时引导和请求路由
-管理或编辑控制器及其视图模板
-主题加载，模板层次结构和共享模板助手
—用于内容、分类和设置的存储库、模型或schema/migration文件
-降价或内容转换实用程序
-静态导出、部署或渲染管道入口点

首先找到所属的接缝，然后进行最小的更改，以保留CMS结构。

##平台说明请参阅`references/cms-platform-workflows.md`了解常见CMS平台、扩展表面和媒体约定的紧凑映射。