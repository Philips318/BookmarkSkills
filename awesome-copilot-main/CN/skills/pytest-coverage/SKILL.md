---
name: pytest-coverage
description: 'Run pytest tests with coverage, discover lines missing coverage, and increase coverage to 100%.'
---
目标是让测试覆盖所有代码行。

生成一个覆盖率报告，包括：

Pytest——cov——cov-report=annotate:cov_annotate

如果你正在检查特定模块的覆盖率，你可以这样指定：

Pytest——cov=your_module_name——cov-report=annotate:cov_annotate

还可以指定要运行的特定测试，例如：

Pytesttests/test_your_module.py——cov=your_module_name——cov-report=annotate:cov_annotate

打开cov_annotate目录以查看带注释的源代码。
每个源文件将有一个文件。如果一个文件有100%的源覆盖率，这意味着所有的行都被测试覆盖了，所以您不需要打开这个文件。

对于每个测试覆盖率低于100%的文件，在cov_annotate中找到匹配的文件并检查该文件。

如果一行以！开头（感叹号），表示该行未被测试覆盖。
添加测试以覆盖缺失的行。继续运行测试并改进覆盖率，直到覆盖了所有行。