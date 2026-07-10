---
description: "Write only the resulting content into files. Never echo prompt instructions, rationale, or meta-commentary into documentation, comments, or code being produced from a prompt."
applyTo: '**'
---
#排除提示数据

当提示包含用于指导更改的指导性或上下文数据时，
该数据不能出现在正在更新的文件中。输出必须反映
只有指令的“结果”，而不是指令本身
它背后的推理，或者任何对它的应用的承认。

核心规则

> **永远不要在被修改的文件中回显提示内容
>
只写结果。去掉任何元评论、理由或框架
>起源于提示。

什么是提示数据

提示数据是用户作为指令或上下文提供的任何内容
比预期的文件内容：-添加或更改内容的描述（`"add a --verbose flag that..."`）
-内联原理或动机（`"because the old behavior caused..."`）
-引用提示符本身(`"as requested"`,`"per the prompt"`，`"the new feature has been added as"`)
-关于更新的元评论
(`"This section has been updated to reflect..."`)
-代码注释叙述更改而不是描述代码
(`"// Added email validation as requested"``"// Now validates the input per the new requirement"`)
—用作切片标记或模板槽的结构脚手架标签
（`## this Title`中的`this`是脚手架，而不是标题文本）

##输出内容

输出文件应该只包含：-提示所要求的功能、修复或内容-像往常一样编写
属于那里
-文档或代码，读者会发现有用的独立于如何
要求更改
-通用的，陈词滥调的占位符数据(例如，`Jane Doe`，`jane.doe@example.com`,`Acme Corp`,`example.com`)——从来都不是真名，
从提示符或本地提取的电子邮件、域或组织标识符
配置
-在提示符中应用于术语的语言格式一直延续到
输出-如果提示符用反引号包装了一个术语或使用了特定的语法
约定，在输出中遵循相同的约定

##输出质量

提示符的写入质量不能为输出设置标准。不管
关于提示的措辞，结果必须经过润色和制作准备：-正确的语法、大小写和标点符号
-没有草稿质量的散文或随意写的部分
-提示符中不正式或草率的措辞不得带入输出

##用例

在文档中添加特性标志

* * * *提示```text
Update file.ext with new feature --new-opt <argument>, documenting the new
feature in features.md
```
**可接受结果-`features.md`**```text
### --new-opt

Enables extended output. Requires a value argument. Example:

    ```bash
    file --new-opt foo
    ```
```
**不可接受的结果-`features.md`**```text
### --new-opt

The new feature `--new-opt` requiring an argument has now been added as
requested. The feature is documented as such.

Enables extended output. Requires a value argument. Example:

    ```bash
    file --new-opt foo
    ```
```
不可接受的版本与提示的框架相呼应
(`"has now been added as requested"``"The feature is documented as such"`)。
该语言属于提示符，而不是文件。

---

更新代码文件

* * * *提示```text
Add input validation to the createUser function — email must be a valid format.
```
* * * *可接受的结果```js
function createUser(name, email) {
  // Rejects addresses missing a local part, @ sign, or domain
  if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email)) {
    throw new Error('Invalid email address.');
  }
  // ...
}
```
* * * *不可接受的结果```js
// Added email validation as requested in the prompt
function createUser(name, email) {
  // Per the instruction, we now validate that email must be a valid format
  if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email)) {
    throw new Error('Invalid email address.');
  }
  // ...
}
```
不可接受的版本将提示短语泄漏到代码注释中。代码
评论和文档更新是适当和鼓励的——他们应该这样做
描述代码的作用、约束或意图。他们必须做什么
永远不要这样描述更改、引用提示或报告
响应请求它的用户。

# #例外

控件中出现提示内容是少数情况下的合理要求
文件。将这些视为例外，而不是漏洞：- **要求逐字抄写。**用户明确要求提示
要按原样插入的文本（例如，“将此块粘贴到README下面”）`## Notice`”)。插入所要求的内容，仅此而已。
- **文件*是*一个提示或指令工件。**编辑提示时
文件，技能定义，或指令文件，教学内容是
预期的负载。这个规则仍然适用于上一级：不要添加
关于*this*的元注释编辑到那些文件中。
- **变更日志或发布说明条目。**描述……的简短、事实性的句子
改变是适当的。保持对变化的关注，而不是对请求的关注
（`Added --verbose flag`✓/`Added --verbose flag as requested by user`）。

##保存前自检

在提交由提示生成的编辑之前，请扫描diff中的任何
跟踪并删除您找到的内容：-[]“按要求”、“按提示”、“按您的指示”等短语，
“如你所愿”
-[]宣告变化而不是描述主题的句子
（“这部分现在涵盖…”，“更新到包括…”）
-[]注释解释代码为什么被写，而不是它做了什么
-[]在文件中逐字重述用户的请求
-[]确认提示符的存在

如果出现任何这些，重写受影响的部分，使一个新的读者
不知道提示符的人会发现内容自然而独立。

# #故障排除

|修复||---|---|
|输出包含“as requested”或“per the prompt”|删除它|
|直接重写|
|代码注释描述更改|描述代码行为|
|在输出标题中出现提示支架标签|替换为原来的|

# #总结

写下结果，而不是你如何到达那里的故事。一位读者
输出文件应该看到干净、有用的内容——没有提示符的痕迹
这就产生了它。