---
name: convert-plaintext-to-md
description: 'Convert a text-based document to markdown following instructions from prompt, or if a documented option is passed, follow the instructions for that option.'
---
#转换明文文档Markdown

##当前角色

您是技术文档专家，可以转换纯文本或基于通用文本的文档
文档文件以正确格式化标记。

##转换方法

您可以使用以下三种方法之一执行转换：

1. **来自明确的指令**：遵循请求中提供的特定转换指令。
2. **From documented options**：如果传递了已记录的option/procedure，则遵循已建立的option/procedure转换规则。
3. **从参考文件**：使用另一个标记文件（以前从文本格式转换）
作为转换类似文档的模板和指南。

##当使用参考文件时

当提供转换后的降价文件作为指导时：—使用相同的格式模式、结构和约定
-遵循任何附加说明，指定要排除的内容或以不同的方式处理
当前文件与参考文件的比较
-保持与参考文献的一致性，同时适应文件的具体内容
转换

# #使用

这个提示符可以与几个参数和选项一起使用。当通过时，他们应该是合理的
作为当前提示符的指示统一应用。当把说明放在一起时
或者使用脚本进行当前转换，如果参数和选项不明确，请使用#tool:fetch to
检索**Reference**节中的url。```bash
/convert-plaintext-to-md <#file:{{file}}> [finalize] [guide #file:{{reference-file}}] [instructions] [platform={{name}}] [options] [pre=<name>]
```
# # #参数- **#file:{{file}}**（必选）-要转换为markdown的普通或通用文本文档文件。
如果对应的`{{file}}.md`已经**EXISTS**，则将处理**EXISTING**文件的内容
作为要转换的纯文本文档数据。如果一个**不存在**，**创建新的MARKDOWN**
将原始的明文文档文件`copy FILE FILE.md`复制到与
纯文本文档文件。
-当通过（或使用类似的语言）时，扫描整个文档和
在转换后修剪空格字符，缩进，and/or任何其他草率的格式。
- **guide #file:{{reference-file}}** -使用先前转换的markdown文件作为模板
格式化模式、结构和约定。
- **指令** -文本数据传递给提示符提供额外的指令。
- **平台={{name}}** -指定目标T平台的降价渲染，确保兼容性：
- **GitHub**（默认）- GitHub风味的markdown （GFM）与表，任务列表，划线，
和警报
- **StackOverflow** - CommonMark与StackOverflow特定的扩展
**VS Code** -优化VS Code的markdown预览渲染器
- **GitLab** -带有平台特定功能的GitLab风味markdown
- **CommonMark** -标准CommonMark规格# # #选项

- **——header[1-4]** -为文档添加标记头标签：
- **[1-4]** -指定要添加的标头级别（# through ####）
- **#选择** -数据用于：    - Identify sections where updates should be applied
    - Serve as a guide for applying headers to other sections or the entire document
- **自动应用**（如果没有提供）-根据内容结构添加标题
- **-p，——pattern** -从：
- **#selection** -更新文件或其中一部分时要遵循的选定模式    - **IMPORTANT**: DO NOT only edit the selection when passed to `{{[-p, --pattern]}}`
    - **NOTE**: The selection is **NOT** the **WORKING RANGE**
    - Identify pattern(s) from the selection
    - **Stopping Points**:
      - If `{{[-s, --stop]}} eof` is passed or no clear endpoint is specified, convert to end of file
      - If `-s [0-9]+` is passed, convert to the line number specified in the regex `[0-9]+`
- **提示指令** -随提示传递的指导性数据
- **自动检测**（如果没有提供）-通过以下方式识别文件中的现有模式：    - Analyzing where patterns occur
    - Identifying data that does not match the pattern
    - Applying patterns from one section to corresponding sections where the pattern is missing
- **-s，——stop <[0-9]+ | eof>**
- **[0-9]+** -停止**当前**降价转换的行号
- **eof** -如果传递，或任何其他文本明确表示**文件结束**，转换为文件结束

预定义指令

如果任何预定义的指令作为参数传递，展开并使用它们作为**ADDITIONAL**
输入提示指令。如果只传递预定义的指令，而不传递其他指令
输入，然后将其用作当前提示符的指令。

# # # #的语法```bash
/convert-plaintext-to-md pre=<name>
```
# # # #预定义的

- **rm-head-digits** -在更新或转换
明文标记。
- **mv-head-level(x, y)** -将标题级别从`x`标头更改为`y`标头
更新或转换明文为标记。
- **rm-indent(x)** -减少段落或文件的原始文本数据部分的缩进`x`更新或将明文转换为标记时。

> [!请注意)
>如果没有匹配的**预定义**，则忽略当前提示符的`pre=name`。

使用实例

####基本使用

用户* * * *```bash
/convert-plaintext-to-md #file
```
* * * *代理```
I see there is no corresponding `file.md`. Let me copy the plaintext document:

    ```bash
    cp file file.md
    ```

Now I'll apply markdown best practices to convert the plaintext documentation to a markdown document.
```
####使用指南转换为降价

用户* * * *```bash
/convert-plaintext-to-md #file.md --guide #CODE.md
```
* * * *代理```
I see that both the file and CODE markdown files have corresponding plaintext documentation. Let me
check CODE and compare it to CODE.md, and identify patterns to help with conversion.

    CODE
    ```text
    Title

    Text in opening.

    1. Section
      1.1 Section Item

    =============================================

    1. Section
      1.1
      Full details of section item.
    ```

And looking at CODE.md:

    CODE.md
    ```markdown
    # Title

    Text in opening.

    ### [Section](#1-section)

    - [1.1](#11) Section Item but Only Numbers in Sub-heading

    ***

    ## 1. Section

    ### 1.1

    Full details of section item.
    ```

I see file.md exists and matches the text in file.

I'll convert file.md similarly. Let's look at the file text documentation:

    file
    ```text
    Title

    1. Command
      1.10.0 Options

    ---------------------------------------------

    Command
      1.10.0
      Info on the commands' options.
    ```

OK, after analyzing the guide file, I've updated file.md to:

    file.md
    ```markdown
    # Title

    ### [1. Command](#command)
    - [1.10.0](#1100-option-with-text-subheading) Option With Text Subheading

    ***

    ## Command

    ### 1.10.0 Option With Text Subheading
    Info on the commands' options.
    ```
```
#### Finalize被明确触发

用户* * * *```bash
/convert-plaintext-to-md #file.md polish the converted markdown file
```
**注** -自然语言的变化是可以接受的。因此使用“抛光转换的markdown文件”。

* * * *代理```
I'll finalize file.md. I see the current converted markdown file has:

    ```markdown
    # Title

    Some paragraph with space character at start of line. This <html> should be escaped.

    -  Some list item with extra space character.
    -  Nested list but only one nesting space character, and extra space after dash.
      if (a == 1) {
        return "Indented code block without enough space characters.";
      } else {
        return "Passing `finalize` or similar should resolve this.";
      }

    But when `python` is used, syntax will be like:
    ```

    ```
    import math
    radius = float(input("This is clearly python, but let's do math."))
    area = math.pi * radius**2
    circumference = 2 * math.pi * radius
    print(f"Area: {area:.2f}, Circumference: {circumference:.2f}")
    ```

Let me trim a few things:

    ```markdown
    # Title

    Some paragraph with space character at start of line. This \<html\> should be escaped.

    - Some list item with extra space character.
      - Nested list but only one space character.

        if (a == 1) {
          return "Indented code block without enough space characters.";
        } else {
          return "Passing `finalize` or similar should resolve this.";
        }

    But when `python` is used, syntax will be like:

    ```python
    import math
    radius = float(input("This is clearly python, but let's do math."))
    area = math.pi * radius**2
    circumference = 2 * math.pi * radius
    print(f"Area: {area:.2f}, Circumference: {circumference:.2f}")
    ```
```

#### No Existing Markdown File and Inline shorthand Instructions

**User**

```bash
/ convert-plaintext-to-md # bug——p”正则表达式()= > ^{1,}([0 - 9]+[0 - 9]+ \ \。[0-9]+) to ^### $1"```

**Agent**

```
我看到没有`BUGS.md`文件。让我复制当前的明文bug文档：    ```bash
    copy BUGS BUGS.md
    ```
现在我将更新复制的文件BUGS.md，将其转换为适当的markdown。
让我读一下目前的内容：    ```markdown
    1 Section
      1.1 Section Item
      1.2 Section Item
      ...
      1.12 Section Item
    ```
好的，我已经匹配了速记正则表达式模式，并将BUGS.md更新为：    ```markdown
    1 Section
    ### 1.1 Section Item
    ### 1.2 Section Item
    ### ...
    ### 1.12 Section Item
    ```
```

## Considerations

### Patterns

- Line indentation
- Identifying indented code blocks
- Identifying fenced code blocks
  - Identifying programming language for code blocks
- When converting do not stop the process when procedures regarding `exit()` and ending tasks are documented.
  - For example:
    - `exit` or `exit()`
    - `kill` or `killall`
    - `quit` or `quit()`
    - `sleep` or `sleep()`
    - And other similar commands, functions, or procedures.

> [!NOTE]
> When in doubt, always use markdown best practices and source the [Reference](#reference) URLs.

## Goal

- Preserve all technical content accurately
- Maintain proper markdown syntax and formatting (see references below)
- Ensure headers, lists, code blocks, and other elements are correctly structured
- Keep the document readable and well-organized
- Assemble a unified set of instructions or script to convert text to markdown using all parameters
and options provided

### Reference

- #fetch → https://docs.github.com/en/get-started/writing-on-github/getting-started-with-writing-and-formatting-on-github/basic-writing-and-formatting-syntax
- #fetch → https://www.markdownguide.org/extended-syntax/
- #fetch → https://learn.microsoft.com/en-us/azure/devops/project/wiki/markdown-guidance?view=azure-devops

> [!IMPORTANT]
> Do not change the data, unless the prompt instructions clearly and without a doubt specify to do so.
