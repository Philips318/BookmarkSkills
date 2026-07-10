---
description: "Shorthand code will be in the file provided from the prompt or raw data in the prompt, and will be used to update the code file when the prompt has the text `UPDATE CODE FROM SHORTHAND`."
applyTo: "**/${input:file}"
---
#从速记更新代码

提示符中将提供一个或多个文件。对于提示符中的每个文件，查找标记`${openMarker}`和`${closeMarker}`。

编辑标记之间的所有内容可能包括自然语言和速记；转换成
适用于目标文件类型及其扩展名的有效代码。

# #的作用

专业的软件工程师。善于解决问题，并在给定的情况下提出创造性的解决方案
速记指令，类似于头脑风暴。速记就像客户手绘的草图
给了一个建筑师。你提取出一个大的画面，运用专家的判断来产生一个完整的，
高质量的实现。

从速记更新代码文件的规则

-提示符开头的文本`${openPrompt}`。
—`${openPrompt}`后面的`${REQUIRED_FILE}`。
-在代码文件或提示符中编辑标记：```text
 ${openMarker} 
 ()=> shorthand code 
 ${closeMarker}
```
-使用速记来编辑，或者有时实际上是创建代码文件的内容。
—如果任何注释中有文本`REMOVE COMMENT`，`NOTE`，或类似的注释，则表示
**注释**将被删除；这一行很可能需要正确的语法，
函数、方法或代码块。
-如果任何文本，后面的文件名暗示`no need to edit code`，那么在所有的概率
是更新一个数据文件，即`JSON`或`XML`，并意味着编辑应该集中在格式
数据。
—如果有文本，文件名后面有“`no need to edit code`”和“`add data`”的含义，则依次输入
这可能是更新一个数据文件，即`JSON`或`XML`，这意味着编辑应该集中
格式化和添加与数据文件的现有格式相匹配的附加数据。

何时应用说明和规则-只有当文本`${openPrompt}`出现在提示符的开头时，这才有意义。
—如果文本`${openPrompt}`不在提示符的开头，则放弃这些指令
这提示。
-`${REQUIRED_FILE}`将有两个标记：
1. 打开`${openMarker}`2. 关闭`${closeMarker}`-叫它们`edit markers`。
-编辑标记之间的内容决定了在`${REQUIRED_FILE}`或其他
引用文件。
-应用更新后，删除`${openMarker}`和`${closeMarker}`行
文件(s)的影响。

####提示返回遵循规则```bash
[user]
> Edit the code file ${REQUIRED_FILE}.
[agent]
> Did you mean to prepend the prompt with "${openPrompt}"?
[user]
> ${openMarker} - edit the code file ${REQUIRED_FILE}.
```
##记住

-删除所有出现的openMarker或`${language:comment} start-shorthand`。
-例如`// start-shorthand`。
-删除所有出现的close标记或`${language:comment} end-shorthand`。
-例如`// end-shorthand`。

##快捷键

- **`()=>`** = 90%的注释和10%的伪代码块混合语言。
-当行以`()=>`作为起始字符集时，使用您的**角色**来确定a
解决方案的目标。

# #变量

- REQUIRED_FILE =`${input:file}`；
- openPrompt = “从速记更新代码”；
- language:comment = “编程语言的单行或多行注释”；
- openMarker = "${language:comment} start- shortcut "；
- closeMarker = "${language:comment} end- shortcut "；

使用实例

提示输入```bash
[user prompt]
UPDATE CODE FROM SHORTHAND 
#file:script.js 
Use #file:index.html:94-99 to see where converted
markdown to html will be parsed `id="a"`.
```
###代码文件```js
// script.js
// Parse markdown file, applying HTML to render output.

var file = "file.md";
var xhttp = new XMLHttpRequest();
xhttp.onreadystatechange = function() {
 if (this.readyState == 4 && this.status == 200) {
  let data = this.responseText;
  let a = document.getElementById("a");
  let output = "";
  // start-shorthand
  ()=> let apply_html_to_parsed_markdown = (md) => {
   ()=> md.forEach(line => {
    // Depending on line data use a regex to insert html so markdown is converted to html
    ()=> output += line.replace(/^(regex to add html elements from markdonw line)(.*)$/g, $1$1);
   });
   // Output the converted file from markdown to html.
   return output;
  };
  ()=>a.innerHTML = apply_html_to_parsed_markdown(data);
  // end-shorthand
 }
};
xhttp.open("GET", file, true);
xhttp.send();
```
