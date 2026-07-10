---
applyTo: '**/*.php, **/*.js, **/*.mustache, **/*.xml, **/*.css, **/*.scss'
description: 'Instructions for GitHub Copilot to generate code in a Moodle project context.'
---
#项目背景

这个存储库包含一个Moodle项目。确保任何生成的代码都与本项目中使用的特定Moodle版本兼容（例如，Moodle 3.11、4.1 LTS或更高版本）。

它包括:
-插件开发（本地，块，mod，授权，注册，工具等）
-主题定制
—CLI脚本
-使用Moodle API与外部服务集成

#代码标准—遵循官方Moodle编码指南：https://moodledev.io/general/development/policies/codingstyle- PHP必须与核心版本（如PHP 7.4 / 8.0 / 8.1）兼容。
-不要使用内核不支持的现代语法，如果它破坏兼容性。
—类命名必须使用Moodle命名空间。
-遵循Moodle的标准插件目录布局（例如：classes/output，classes/form, db/, lang/, templates/…）
-强制使用Moodle安全功能：
-`$DB`与SQL占位符
-`require_login()`,`require_capability()`-使用`required_param()`/`optional_param()`处理的参数

#代码生成规则

-当在插件中创建新的PHP类时，使用与插件组件名称匹配的Moodle组件（Frankenstyle）命名空间，例如`local_myplugin`，`mod_forum`,`block_mycatalog`,`tool_mytool`。
-在插件中，始终尊重结构：
——/ db
朗- /
——/类
——/模板
——/version.php——/settings.php- /lib.php（仅当需要时）-为HTML使用渲染器和Mustache模板。不要在PHP中混用HTML。
-在JavaScript代码中，使用AMD模块，而不是内联脚本。
-尽可能使用Moodle API函数而不是手动代码。
-不要发明不存在的Moodle功能。

#副驾驶应该能够回答的例子

“生成一个基本的本地插件version.php，settings.php，和lib.php。”
“在db/install.xml创建一个新表，并在db/upgrade.php.创建一个升级脚本”
-“使用moodleform生成Moodle表单。”
-“创建一个带有Mustache的渲染器来显示表格。”

#预期风格

-在Moodle上下文中明确和具体的答案。
-始终包含具有完整路径的文件。
—如果有多种方法可以完成某件事，请使用Moodle推荐的方法。