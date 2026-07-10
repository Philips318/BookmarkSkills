---
name: write-coding-standards-from-file
description: 'Write a coding standards document for a project using the coding styles from the file(s) and/or folder(s) passed as arguments in the prompt.'
---
#从文件编写编码标准

使用文件的现有语法为项目建立标准和风格指南。如果传递了多个文件或文件夹，循环遍历文件夹中的每个文件或文件，将文件的数据附加到临时内存或文件中，然后在完成时使用临时数据作为单个实例；就好像它是基于标准和样式指南的文件名一样。

##规则和配置

下面是一组准配置`boolean`和`string[]`变量。处理`true`或每个变量的其他值的条件位于第二级标题`## Variable and Parameter Configuration Conditions`之下。

提示符的参数有一个文本定义。有一个必选参数**`${fileName}`**，以及几个可选参数**`${folderName}`**、**`${instructions}`**和任何**`[configVariableAsParameter]`**。

配置变量* addStandardsTest = false；
* addToREADME = false；
* addToREADMEInsertions = ["atBegin", "middle", "beforeEnd", "bestFitUsingContext"]；
—默认为**beforeEnd**。
* createNewFile = true；
* fetchStyleURL = true；
* findinconsistences = true；
* fixxinconsistences = true；
* newFileName = ["CONTRIBUTING.md", "STYLE.md", "CODE_OF_CONDUCT.md", "CODING_STANDARDS.md", "DEVELOPING.md", "CONTRIBUTION_GUIDE.md", "PROJECT_STANDARDS.md", "BEST_PRACTICES.md", "HACKING.md"]；
—对于`${newFileName}`中的每个文件，如果文件不存在，则使用该文件名和`break`，否则继续使用下一个`${newFileName}`的文件名。
* outputSpecToPrompt = false；
* useTemplate = "verbose"；//或“v”
—取值为“`[["v", "verbose"], ["m", "minimal"], ["b", "best fit"], ["custom"]]`”。
-选择提示文件底部的两个示例模板中的一个，在二级标题`## Coding Standards Templates`下，或者使用另一个更合适的组合。
-如果**自定义**，则每个请求应用。

配置变量作为提示参数如果任何变量名按原样传递给prompt，或者作为类似但明显相关的文本值传递给prompt，那么用传递给prompt的值覆盖默认变量值。

###提示参数** *fileName** =要分析的文件的名称：缩进、变量命名、注释、条件过程、函数过程以及文件编码语言的其他语法相关数据。
* folderName =将用于从多个文件中提取数据到一个聚合数据集的文件夹名称，该数据集将在以下方面进行分析：缩进，变量命名，注释，条件过程，函数过程以及文件编码语言的其他语法相关数据。
*说明=针对特殊情况提供的附加说明、规则和程序。
* [configVariableAsParameter] =如果传递将覆盖配置变量的默认状态。例子:
- useTemplate =如果通过，将覆盖配置`${useTemplate}`默认值。取值为`[["v", "verbose"], ["m", "minimal"], ["b", "best fit"]]`。

####必选参数及可选参数** *文件名** -必需
* folderName - *可选*
*指令- *可选*
* [configVariableAsParameter] - *可选*

变量和参数配置条件

# # #`${fileName}.length > 1 || ${folderName} != undefined`*如果为true，则将`${fixInconsistencies}`设置为false。

# # #`${addToREADME} == true`*将编码标准插入到`README.md`中，而不是输出到提示符或创建新文件。
*如果为true，将`${createNewFile}`和`${outputSpecToPrompt}`都设为false。

# # #`${addToREADMEInsertions} == "atBegin"`*如果`${addToREADME}`为true，则在`README.md`文件的**开始**处插入编码标准数据。

# # #`${addToREADMEInsertions} == "middle"`*如果`${addToREADME}`为真，则在`README.md`文件的**中间**插入编码标准数据，更改标准标题标题以匹配`README.md`组合。

# # #`${addToREADMEInsertions} == "beforeEnd"`*如果`${addToREADME}`为true，则在`README.md`文件的** ** **端插入编码标准数据，在最后一个字符后插入新行，然后将数据插入新行。

# # #`${addToREADMEInsertions} == "bestFitUsingContext"`*如果`${addToREADME}`为真，则根据`README.md`组成和数据流的上下文，在`README.md`文件的** *最佳拟合行**插入编码标准数据。

# # #`${addStandardsTest} == true`一旦编码标准文件完成，编写一个测试文件，以确保传递给它的文件遵守编码标准。

# # #`${createNewFile} == true`*使用`${newFileName}`的值或其中一个可能的值创建一个新文件。
*如果为true，将`${outputSpecToPrompt}`和`${addToREADME}`都设为false。

# # #`${fetchStyleURL} == true`*另外，使用从嵌套在三级标题`### Fetch Links`下的链接中获取的数据作为上下文，为新文件、提示符或`README.md`创建标准、规范和样式数据。
*对于`### Fetch Links`中的每个相关项，运行`#fetch ${item}`。

# # #`${findInconsistencies} == true`*评估与缩进，换行，注释，条件和函数嵌套，引号包装相关的语法，如`'`或`"`用于字符串等，并进行分类。
*对于每个类别，做一个计数，如果一个项目不匹配计数的大部分，然后提交到临时内存。
*根据`${fixInconsistencies}`的状态，编辑和修复低计数类别以匹配大多数，或输出提示不一致存储在临时内存中。

# # #`${fixInconsistencies} == true`*编辑和修复语法数据的低计数类别，以匹配使用不一致存储在临时内存中的大多数相应的语法数据。

# # #`typeof ${newFileName} == "string"`*如果指定为`string`，则使用`${newFileName}`中的值创建新文件。

# # #`typeof ${newFileName} != "string"`如果** *不是** *明确定义为`string`，而是`object`或数组，使用`${newFileName}`的值创建一个新文件，应用以下规则：
—对于`${newFileName}`中的每个文件名，如果文件不存在，则使用该文件名和`break`，否则继续下一个。

# # #`${outputSpecToPrompt} == true`*将编码标准输出到提示符，而不是创建文件或添加到README。
*如果为true，将`${createNewFile}`和`${addToREADME}`都设为false。

# # #`${useTemplate} == "v" || ${useTemplate} == "verbose"`*在编写编码标准的数据时，使用三级标题`### "v", "verbose"`下的数据作为指导模板

# # #`${useTemplate} == "m" || ${useTemplate} == "minimal"`*在编写编码标准的数据时，使用三级标题`### "m", "minimal"`下的数据作为指导模板

# # #`${useTemplate} == "b" || ${useTemplate} == "best"`*使用三级标题`### "v", "verbose"`或`### "m", "minimal"`下的数据，根据从`${fileName}`中提取的数据，在编写编码标准时使用最合适的数据作为指导模板。

# # #`${useTemplate} == "custom" || ${useTemplate} == "<ANY_NAME>"`*在编写编码标准的数据时，使用自定义提示、指令、模板或其他传递的数据作为指导模板。

## **if**`${fetchStyleURL} == true`根据编程语言的不同，对于下面列表中的每个链接，如果编程语言为`${fileName} == [<Language> Style Guide]`，则运行`#fetch (URL)`。

###获取链接- [C语言风格指南]（https://users.ece.cmu.edu/~eno/coding/CCodingStandard.html）
- [c#风格指南]（https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions）
- [c++风格指南]（https://isocpp.github.io/CppCoreGuidelines/CppCoreGuidelines）
- [Go风格指南]（https://github.com/golang-standards/project-layout）
- [Java风格指南]（https://coderanch.com/wiki/718799/Style）
- [AngularJS应用风格指南]（https://github.com/mgechev/angularjs-style-guide）
- [jQuery样式指南]（https://contribute.jquery.org/style-guide/js/）
- [JavaScript风格指南]（https://www.w3schools.com/js/js_conventions.asp）
- [JSON风格指南]（https://google.github.io/styleguide/jsoncstyleguide.xml）
- [Kotlin风格指南]（https://kotlinlang.org/docs/coding-conventions.html）
- [Markdown风格指南]（https://cirosantilli.com/markdown-style-guide/）
- [Perl风格指南]（https://perldoc.perl.org/perlstyle）
- [PHP风格指南]（https://phptherightway.com/）
- [Python风格指南]（https://peps.python.org/pep-0008/）
- [Ruby风格指南]（https://rubystyle.guide/）
- [Rust风格指南]（https://github.com/rust-lang/rust/tree/HEAD/src/doc/style-guide/src）
- [Swift风格指南]（https://www.swift.org/documentation/api-design-guidelines/）
- [TypeScript样式指南]（https://www.typescriptlang.org/docs/handbook/declaration-files/do-s-and-don-ts.html）
- [Visual Basic风格指南]（https://en.wikibooks.org/wiki/Visual_Basic/Coding_Standards）
- [Shell脚本样式指南]（https://google.github.io/styleguide/shellguide.html）
- [Git使用风格指南]（https://github.com/agis/git-style-guide）
- [PowerShell风格指南]（https://github.com/PoshCode/PowerShellPracticeAndStyle）
- (CSS) (https://cssguidelin.es/)
- [Sass风格指南]（https://sass-guidelin.es/）
- [HTML样式指南]（https://github.com/marcobiedermann/html-style-guide）
- [Linux内核风格指南]（https://www.kernel.org/doc/html/latest/process/coding-style.html）
- [Node.js风格指南]（https://github.com/felixge/node-style-guide）
-[SQL风格指南]（https://www.sqlstyle.guide/）
- [Angular风格指南]（https://angular.dev/style-guide）
- [Vue风格指南]（https://vuejs.org/style-guide/rules-strongly-recommended.html）
- [Django风格指南]（https://docs.djangoproject.com/en/dev/internals/contributing/writing-code/coding-style/）
- [SystemVerilog风格指南]（https://github.com/lowRISC/style-guides/blob/master/VerilogCodingStyle.md）编码标准模板

# # #`"m", "minimal"````text
    ```markdown
    ## 1. Introduction
    *   **Purpose:** Briefly explain why the coding standards are being established (e.g., to improve code quality, maintainability, and team collaboration).
    *   **Scope:** Define which languages, projects, or modules this specification applies to.

    ## 2. Naming Conventions
    *   **Variables:** `camelCase`
    *   **Functions/Methods:** `PascalCase` or `camelCase`.
    *   **Classes/Structs:** `PascalCase`.
    *   **Constants:** `UPPER_SNAKE_CASE`.

    ## 3. Formatting and Style
    *   **Indentation:** Use 4 spaces per indent (or tabs).
    *   **Line Length:** Limit lines to a maximum of 80 or 120 characters.
    *   **Braces:** Use the "K&R" style (opening brace on the same line) or the "Allman" style (opening brace on a new line).
    *   **Blank Lines:** Specify how many blank lines to use for separating logical blocks of code.

    ## 4. Commenting
    *   **Docstrings/Function Comments:** Describe the function's purpose, parameters, and return values.
    *   **Inline Comments:** Explain complex or non-obvious logic.
    *   **File Headers:** Specify what information should be included in a file header, such as author, date, and file description.

    ## 5. Error Handling
    *   **General:** How to handle and log errors.
    *   **Specifics:** Which exception types to use, and what information to include in error messages.

    ## 6. Best Practices and Anti-Patterns
    *   **General:** List common anti-patterns to avoid (e.g., global variables, magic numbers).
    *   **Language-specific:** Specific recommendations based on the project's programming language.

    ## 7. Examples
    *   Provide a small code example demonstrating the correct application of the rules.
    *   Provide a small code example of an incorrect implementation and how to fix it.

    ## 8. Contribution and Enforcement
    *   Explain how the standards are to be enforced (e.g., via code reviews).
    *   Provide a guide for contributing to the standards document itself.
    ```
```
# # #`"v", verbose"````text
    ```markdown

    # Style Guide

    This document defines the style and conventions used in this project.
    All contributions should follow these rules unless otherwise noted.

    ## 1. General Code Style

    - Favor clarity over brevity.
    - Keep functions and methods small and focused.
    - Avoid repeating logic; prefer shared helpers/utilities.
    - Remove unused variables, imports, code paths, and files.

    ## 2. Naming Conventions

    Use descriptive names. Avoid abbreviations unless well-known.

    | Item            | Convention           | Example            |
    |-----------------|----------------------|--------------------|
    | Variables       | `lower_snake_case`   | `buffer_size`      |
    | Functions       | `lower_snake_case()` | `read_file()`      |
    | Constants       | `UPPER_SNAKE_CASE`   | `MAX_RETRIES`      |
    | Types/Structs   | `PascalCase`         | `FileHeader`       |
    | File Names      | `lower_snake_case`   | `file_reader.c`    |

    ## 3. Formatting Rules

    - Indentation: **4 spaces**
    - Line length: **max 100 characters**
    - Encoding: **UTF-8**, no BOM
    - End files with a newline

    ### Braces (example in C, adjust for your language)

        ```c
        if (condition) {
            do_something();
        } else {
            do_something_else();
        }
        ```

    ### Spacing

    - One space after keywords: `if (x)`, not `if(x)`
    - One blank line between top-level functions

    ## 4. Comments & Documentation

    - Explain *why*, not *what*, unless intent is unclear.
    - Keep comments up-to-date as code changes.
    - Public functions should include a short description of purpose and parameters.

    Recommended tags:

        ```text
        TODO: follow-up work
        FIXME: known incorrect behavior
        NOTE: non-obvious design decision
        ```

    ## 5. Error Handling

    - Handle error conditions explicitly.
    - Avoid silent failures; either return errors or log them appropriately.
    - Clean up resources (files, memory, handles) before returning on failure.

    ## 6. Commit & Review Practices

    ### Commits
    - One logical change per commit.
    - Write clear commit messages:

        ```text
        Short summary (max ~50 chars)
        Optional longer explanation of context and rationale.
        ```

    ### Reviews
    - Keep pull requests reasonably small.
    - Be respectful and constructive in review discussions.
    - Address requested changes or explain if you disagree.

    ## 7. Tests

    - Write tests for new functionality.
    - Tests should be deterministic (no randomness without seeding).
    - Prefer readable test cases over complex test abstraction.

    ## 8. Changes to This Guide

    Style evolves.
    Propose improvements by opening an issue or sending a patch updating this document.
    ```
```
