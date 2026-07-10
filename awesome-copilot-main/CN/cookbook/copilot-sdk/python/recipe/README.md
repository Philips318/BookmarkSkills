# Runnable Recipe Examples

此文件夹包含每个食谱食谱的独立的、可执行的Python示例。每个文件都可以作为Python脚本直接运行。

# #先决条件

- Python 3.8或更高版本
-安装依赖项（这是从PyPI安装SDK）：```bash
pip install -r requirements.txt
```
##运行示例

每个`.py`文件都是一个完整的可运行程序，具有可执行权限：```bash
python <filename>.py
# or on Unix-like systems:
./<filename>.py
```
可用的食谱

|配方|命令|描述|| -------------------- | ------------------------------------ | -------------------------------------------------- |
|错误处理|`python error_handling.py`|演示错误处理模式|
|错误恢复挂钩|`python error_recovery_hooks.py`|工具故障分类并自动重试|
|多会话|`python multiple_sessions.py`|管理多个独立会话|
|本地文件管理|`python managing_local_files.py`|通过AI分组|对文件进行组织
| PR可视化|`python pr_visualization.py`|生成PR年龄图表|
|持久化会话|`python persisting_sessions.py`|跨重启保存和恢复会话|
| PyInstaller Build |`python pyinstaller_frozen_build.py`|将SDK应用程序打包为冻结的可执行文件|

带参数的例子

**PR可视化与特定的回购：**```bash
python pr_visualization.py --repo github/copilot-sdk
```
**管理本地文件（编辑文件以更改目标文件夹）：**```bash
# Edit the target_folder variable in managing_local_files.py first
python managing_local_files.py
```
本地SDK开发`requirements.txt`从PyPI安装Copilot SDK包。这意味着:

-您将获得最新的稳定版SDK
-无需从源代码构建
-非常适合在您的项目中使用SDK

如果您想使用本地开发版本，请编辑requirements.txt以使用`-e ../..`进行可编辑模式开发。

## Python最佳实践

以下示例遵循Python约定：

PEP 8命名（函数和变量的snake_case）
- Shebang线为直接执行
-正确的异常处理
-在适当的地方输入提示
-标准库使用

虚拟环境（推荐）

对于孤立开发：```bash
# Create virtual environment
python -m venv venv

# Activate it
# Windows:
venv\Scripts\activate
# Unix/macOS:
source venv/bin/activate

# Install dependencies
pip install -r requirements.txt
```
学习资源

- [Python文档]（https://docs.python.org/3/）
- [PEP 8风格指南]（https://pep8.org/）
- [GitHub CopilotPython SDK]（https://github.com/github/copilot-sdk/blob/main/python/README.md）
-[家长食谱]（../README.md）