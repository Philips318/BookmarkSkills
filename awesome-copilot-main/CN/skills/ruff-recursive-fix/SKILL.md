---
name: ruff-recursive-fix
description: Run Ruff checks with optional scope and rule overrides, apply safe and unsafe autofixes iteratively, review each change, and resolve remaining findings with targeted edits or user decisions.
---
# Ruff递归修复

# #概述

使用此技能与Ruff一起在受控的、迭代的工作流程中加强代码质量。
它支持:

—可选范围限制为指定文件夹。
—`pyproject.toml`的默认项目设置。
-灵活的Ruff调用（`uv`，直接`ruff`，`python -m ruff`，或同等）。
-可选的运行规则覆盖（`--select`,`--ignore`,`--extend-select`,`--extend-ignore`）。
-自动安全然后不安全的自动修复。
-每次修复通过后进行Diff审查。
-递归重复，直到发现得到解决或需要做出决定。
-只有在需要抑制时才明智地使用内联`# noqa`。

# #输入

在运行前收集这些输入：—`target_path`（可选）：要检查的文件夹或文件。空表示整个存储库。
—`ruff_runner`（可选）：显式Ruff命令前缀（例如`uv run`、`ruff`、`python -m ruff`、`pipx run ruff`）。
—`rules_select`（可选）：要执行的逗号分隔规则代码。
—`rules_ignore`（可选）：忽略以逗号分隔的规则代码。
—`extend_select`（可选）：添加额外的规则，而不替换配置的默认值。
—`extend_ignore`（可选）：额外忽略的规则，不替换配置的默认值。
—`allow_unsafe_fixes`（默认值：true）：是否运行Ruff不安全补丁。
-`ask_on_ambiguity`（默认值：true）：当存在多个有效选项时，总是询问用户。

##命令构造

从输入构建Ruff命令。

# # # 0。解决Ruff Runner

在构建命令之前确定一个可重用的`ruff_cmd`前缀。

分辨率:1. 如果提供了`ruff_runner`，则按原样使用它。
2. 否则，如果`uv`可用并且Ruff是通过`uv`管理的，则使用`uv run ruff`。
3. 否则，如果`PATH`上有`ruff`，则使用`ruff`。
4. 否则，如果Python可用并且Ruff安装在该环境中，则使用`python -m ruff`。
5. 否则使用调用已安装的Ruff的任何特定于项目的等效程序（例如`pipx run ruff`），或者停止并询问用户。

对工作流中的所有`check`和`format`命令使用相同的解析`ruff_cmd`。

基本命令:```bash
<ruff_cmd> check
```
格式化程序命令:```bash
<ruff_cmd> format
```
可选目标：```bash
<ruff_cmd> format <target_path>
```
添加可选目标器：```bash
<ruff_cmd> check <target_path>
```
根据需要添加可选的覆盖：```bash
--select <codes>
--ignore <codes>
--extend-select <codes>
--extend-ignore <codes>
```
例子:```bash
# Full project with defaults from pyproject.toml
ruff check

# One folder with defaults
python -m ruff check src/models

# Override to skip docs and TODO-like rules for this run
uv run ruff check src --extend-ignore D,TD

# Check only selected rules in a folder
ruff check src/data --select F,E9,I
```
# #工作流程

# # # 1。基准分析

1. 使用选定的范围和选项运行`<ruff_cmd> check`。
2. 按类型对发现进行分类：	- Autofixable safe.
	- Autofixable unsafe.
	- Not autofixable.
3. 如果没有发现，停止。

# # # 2。安全自动修复通行证

1. 使用相同的scope/options.运行带有`--fix`的Ruff
2. 仔细检查结果差异以确保语义正确性和风格一致性。
3. 在同一作用域上运行`<ruff_cmd> format`。
4. 重新运行`<ruff_cmd> check`以刷新剩余的发现。

# # # 3。不安全的自动修复通行证

只有在发现结果仍然存在时才运行`allow_unsafe_fixes=true`。

1. 使用相同的scope/options.运行带有`--fix --unsafe-fixes`的Ruff
2. 仔细审查产生的差异，优先考虑对行为敏感的编辑。
3. 在同一作用域上运行`<ruff_cmd> format`。
4. 重新运行`<ruff_cmd> check`。

# # # 4。手动修复通过

其余调查结果：

1. 当有一个清晰、安全的修正时，直接在代码中修复。
2. 保持编辑最小化和本地化。
3. 在同一作用域上运行`<ruff_cmd> format`。
4. 重新运行`<ruff_cmd> check`。

# # # 5。模棱两可的政策

如果在任何步骤中都有多个有效的解决方案，那么在继续之前一定要询问用户。
不要在等价选项之间默默选择。# # # 6。抑制决定（`# noqa`）

只有当所有条件都为真时才使用抑制：

—规则与要求的行为、公共API、框架约定或可读性目标冲突。
-重构与规则的价值不成比例。
-抑制是狭窄和特定的（单行，明确的代码，如果可能的话）。

指南:

-首选`# noqa: <RULE>`，而不是宽泛的`# noqa`。
-为不明显的抑制添加简短的原因注释。
-如果存在两个或两个以上的有效结果，总是询问用户选择哪个选项。

# # # 7。递归循环和停止条件

重复步骤2到6，直到出现以下结果之一：

-`<ruff_cmd> check`返回clean。
-剩余的发现需要architectural/product的决定。
-剩余的调查结果被有意地以书面理由加以隐瞒。
-重复循环没有进展。

每个循环迭代必须在下一个`<ruff_cmd> check`之前包含`<ruff_cmd> format`。当未检测到进度时：

1. 汇总被阻止的规则和受影响的文件。
2. 提供有效的选择和权衡。
3. 请用户选择。

##质量门

在宣布完成之前：

对于所选的scope/options.， Ruff没有返回意外的结果
-检查所有自动修复差异的正确性。
-无明确理由不得增加压制。
-向用户突出显示任何可能影响行为的不安全修复。
- Ruff格式在每次迭代中执行。

##输出合同

在执行结束时，报告：

-使用的范围和Ruff选项。
-执行的迭代次数。
-固定发现的摘要。
-手动修复列表。
-有理由禁止的清单。
-剩余的调查结果（如有）和所需的用户决定。

##建议提示启动器-“使用默认配置对整个repo运行ruff-recursive-fix。”
-“只在src/models上运行ruff-recursive-fix，忽略DOC规则。”
- “对选择F、E9、I且没有不安全修复的测试运行ruff-recursive-fix。”
“在src/data上运行ruff-recursive-fix，在添加任何noqa之前问我。”