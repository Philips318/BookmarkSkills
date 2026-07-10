#标记

##快速转换方法

扩展`SKILL.md`在`### Quick Conversion Methods`的部分。

方法1:CLI（推荐用于单个文件）```bash
# Convert file to HTML
marked -i input.md -o output.html

# Convert string directly
marked -s "# Hello World"

# Output: <h1>Hello World</h1>
```
方法二：Node.js脚本```javascript
import { marked } from 'marked';
import { readFileSync, writeFileSync } from 'fs';

const markdown = readFileSync('input.md', 'utf-8');
const html = marked.parse(markdown);
writeFileSync('output.html', html);
```
方法3：浏览器使用```html
<script src="https://cdn.jsdelivr.net/npm/marked/lib/marked.umd.js"></script>
<script>
  const html = marked.parse('# Markdown Content');
  document.getElementById('output').innerHTML = html;
</script>
```
---

##分步工作流程

扩展`SKILL.md`在`### Step-by-Step Workflows`的部分。

工作流程1：单个文件转换

1. 确保已安装标记：`npm install -g marked`2. 运行转换：`marked -i README.md -o README.html`3. 验证已创建输出文件

工作流程2：批量转换（多个文件）

创建脚本`convert-all.js`：```javascript
import { marked } from 'marked';
import { readFileSync, writeFileSync, readdirSync } from 'fs';
import { join, basename } from 'path';

const inputDir = './docs';
const outputDir = './html';

readdirSync(inputDir)
  .filter(file => file.endsWith('.md'))
  .forEach(file => {
    const markdown = readFileSync(join(inputDir, file), 'utf-8');
    const html = marked.parse(markdown);
    const outputFile = basename(file, '.md') + '.html';
    writeFileSync(join(outputDir, outputFile), html);
    console.log(`Converted: ${file} → ${outputFile}`);
  });
```
运行命令：`node convert-all.js`###工作流程3：转换自定义选项```javascript
import { marked } from 'marked';

// Configure options
marked.setOptions({
  gfm: true,           // GitHub Flavored Markdown
  breaks: true,        // Convert \n to <br>
  pedantic: false,     // Don't conform to original markdown.pl
});

const html = marked.parse(markdownContent);
```
工作流程4：完成HTML文档

在一个完整的HTML模板中换行转换后的内容：```javascript
import { marked } from 'marked';
import { readFileSync, writeFileSync } from 'fs';

const markdown = readFileSync('input.md', 'utf-8');
const content = marked.parse(markdown);

const html = `<!DOCTYPE html>
<html lang="en">
<head>
  <meta charset="UTF-8">
  <meta name="viewport" content="width=device-width, initial-scale=1.0">
  <title>Document</title>
  <style>
    body { font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', sans-serif; max-width: 800px; margin: 0 auto; padding: 2rem; }
    pre { background: #f4f4f4; padding: 1rem; overflow-x: auto; }
    code { background: #f4f4f4; padding: 0.2rem 0.4rem; border-radius: 3px; }
  </style>
</head>
<body>
${content}
</body>
</html>`;

writeFileSync('output.html', html);
```
