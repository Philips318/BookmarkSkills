---
applyTo: 'wp-content/plugins/**,wp-content/themes/**,**/*.php,**/*.inc,**/*.js,**/*.jsx,**/*.ts,**/*.tsx,**/*.css,**/*.scss,**/*.json'
description: 'Coding, security, and testing rules for WordPress plugins and themes'
---
WordPress开发-副驾驶说明

**目标：**生成安全、高性能、可测试且符合官方WordPress实践的WordPress代码。首选钩子、小函数、依赖注入（在合理的情况下）和清晰的关注点分离。

1)核心原则
-永远不要修改WordPress核心。通过**actions**和**filters**进行扩展。
-对于插件，总是包含一个头文件，并在入口PHP文件中保护直接执行。
-使用唯一的前缀或PHP命名空间，以避免全局冲突。
-资产排队；永远不要在PHP模板中内联原始`<script>`/`<style>`。
-使面向用户的字符串可翻译，并加载正确的文本域。

最小的插件头和保护```php
<?php
defined('ABSPATH') || exit;
/**
 * Plugin Name: Awesome Feature
 * Description: Example plugin scaffold.
 * Version: 0.1.0
 * Author: Example
 * License: GPL-2.0-or-later
 * Text Domain: awesome-feature
 * Domain Path: /languages
 */
```
编码标准（PHP， JS， CSS， HTML）
-遵循WordPress编码标准（WPCS），编写docblock用于公共api。
PHP：在适当的情况下使用严格比较（`===`,`!==`）。与WPCS中的数组语法和间距保持一致。
- JS：匹配WordPress JS风格；对于block/editor代码，首选`@wordpress/*`包。
CSS：在有用的时候使用类似BEM的类命名；避免过于特定的选择器。
- PHP 7.4+兼容模式，除非项目指定更高。避免使用目标WP/PHP版本不支持的特性。

检查设置建议```xml
<!-- phpcs.xml -->
<?xml version="1.0"?>
<ruleset name="Project WPCS">
  <description>WordPress Coding Standards for this project.</description>
  <file>./</file>
  <exclude-pattern>vendor/*</exclude-pattern>
  <exclude-pattern>node_modules/*</exclude-pattern>
  <rule ref="WordPress"/>
  <rule ref="WordPress-Docs"/>
  <rule ref="WordPress-Extra"/>
  <rule ref="PHPCompatibility"/>
  <config name="testVersion" value="7.4-"/>
</ruleset>
```

```json
// composer.json (snippet)
{
  "require-dev": {
    "dealerdirect/phpcodesniffer-composer-installer": "^1.0",
    "wp-coding-standards/wpcs": "^3.0",
    "phpcompatibility/php-compatibility": "^9.0"
  },
  "scripts": {
    "lint:php": "phpcs -p",
    "fix:php": "phpcbf -p"
  }
}
```

```json
// package.json (snippet)
{
  "devDependencies": {
    "@wordpress/eslint-plugin": "^x.y.z"
  },
  "scripts": {
    "lint:js": "eslint ."
  }
}
```
## 3)安全性和数据处理
- **输出转义，输入净化
—转义：`esc_html()`，`esc_attr()`,`esc_url()`,`wp_kses_post()`。
-消毒：`sanitize_text_field()`，`sanitize_email()`,`sanitize_key()`,`absint()`,`intval()`。
**功能和功能**表单，AJAX, REST：
-添加nonce与`wp_nonce_field()`和验证通过`check_admin_referer()`/`wp_verify_nonce()`。
—使用`current_user_can( 'manage_options' /* or specific cap */ )`限制突变。
**数据库：**总是使用`$wpdb->prepare()`与占位符；永远不要连接不可信的输入。
- **上传：**验证MIME/type和使用`wp_handle_upload()`/`media_handle_upload()`。

4)国际化（i18n）
-使用文本域将用户可见的字符串与翻译函数包装起来：
-`__( 'Text', 'awesome-feature' )`,`_x()`,`esc_html__()`。
-加载翻译与`load_plugin_textdomain()`或`load_theme_textdomain()`。
—在“`/languages`”中保留一个“`.pot`”，保证域使用的一致性。5)性能
-将繁重的逻辑延迟到特定的钩子；除非必要，否则避免在`init`/`wp_loaded`上进行昂贵的工作。
-使用瞬态或对象缓存昂贵的查询；计划失效。
-只排队你需要的和有条件的（前vs管理；特定的screens/routes）。
-首选paginated/parameterized查询而不是无界循环。

## 6)管理UI和设置
-使用**设置API**选项页面；为每个设置提供`sanitize_callback`。
—对于表，遵循`WP_List_Table`模式。对于通知，使用管理通知API。
-避免直接的HTML回显复杂的ui；首选带有转义的模板或小视图帮助程序。

rest API
-注册到`register_rest_route()`；总是设置一个`permission_callback`。
-Validate/sanitize通过`args`模式请求参数。
-返回干净映射到JSON的`WP_REST_Response`或arrays/objects。## 8) block & Editor（古腾堡）
-使用`block.json`+`register_block_type()`；依赖`@wordpress/*`包。
-在需要时提供服务器渲染回调（动态块）。
-端到端测试应包括：插入块→编辑→保存→前端渲染。

## 9)资产加载```php
add_action('wp_enqueue_scripts', function () {
  wp_enqueue_style(
    'af-frontend',
    plugins_url('assets/frontend.css', __FILE__),
    [],
    '0.1.0'
  );

  wp_enqueue_script(
    'af-frontend',
    plugins_url('assets/frontend.js', __FILE__),
    [ 'wp-i18n', 'wp-element' ],
    '0.1.0',
    true
  );
});
```
—如果多个组件依赖于相同的资产，请使用`wp_register_style/script`先注册。
-对于管理屏幕，挂钩到`admin_enqueue_scripts`和检查屏幕id。

10)测试
### PHPUnit/Integration-使用WordPress测试套件**`PHPUnit`和`WP_UnitTestCase`。
-测试：清理，功能检查，REST权限，数据库查询，钩子。
-优先选择工厂（`self::factory()->post->create()`等）安装夹具。```xml
<!-- phpunit.xml.dist (minimal) -->
<?xml version="1.0" encoding="UTF-8"?>
<phpunit bootstrap="tests/bootstrap.php" colors="true">
  <testsuites>
    <testsuite name="Plugin Test Suite">
      <directory suffix="Test.php">tests/</directory>
    </testsuite>
  </testsuites>
</phpunit>
```

```php
// tests/bootstrap.php (minimal sketch)
<?php
$_tests_dir = getenv('WP_TESTS_DIR') ?: '/tmp/wordpress-tests-lib';
require_once $_tests_dir . '/includes/functions.php';
tests_add_filter( 'muplugins_loaded', function () {
  require dirname(__DIR__) . '/awesome-feature.php';
} );
require $_tests_dir . '/includes/bootstrap.php';
```
# # # E2E
-使用剧作家（或木偶）为editor/front- end流。
-涵盖基本的用户旅程和回归（块插入，设置保存，前端渲染）。

## 11)文档和提交
保持`README.md`最新：安装、使用、功能、hooks/filters和测试说明。
-使用清晰、命令式的提交消息；参考issues/tickets并总结影响。

## 12)副驾驶必须确保什么（清单）
-✅唯一的prefixes/namespaces；没有意外的全局变量。
-✅Nonce +能力检查任何写操作（AJAX/REST/forms）。
-✅经过消毒的输入；输出逃脱了。
-✅用户可见的字符串包装在i18n正确的文本域。
-✅资产通过api排队（没有内联script/style）。
-✅测试added/updated的新行为。
-✅代码通过PHPCS （WPCS）和ESLint。
-✅避免直接连接DB；总是准备好查询。