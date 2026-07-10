#宏，加载和运行脚本

命令宏（不需要脚本）

宏是一个命令行输入字符串。在Rhino接受命令（别名、工具栏按钮、`_ReadCommandFile`）的任何地方，都可以放置宏。

语法规则

|令牌|含义||---|---|
|`!`| **在此命令启动之前取消**任何当前正在运行的命令。总是用`!`开始宏。|
|`_`|使用**英文**（不变）命令名，因此宏可以在任何语言环境中工作。|
|`-`|以“**script”模式执行命令** -抑制对话框，接受宏字符串输入。|
|`_Enter`|在当前提示符下按“Enter”。|
|`Pause`|停止并等待用户交互式地提供此输入。|
|`;`|注释到行尾。|
|换行|与空格相同——是令牌之间的分隔符，而不是命令终止符。|

示例宏```text
! _-Line 0,0,0 10,0,0
! _-Circle 0,0,0 5 _Enter
! _SelAll _Delete
! _-Properties _Object _Name "MyObject" _EnterEnd _Enter
! _-RunPythonScript "MyScript.py"
```
##运行已保存的脚本

### Python （`.py`）```text
_-RunPythonScript "C:\Users\example\Scripts\MyScript.py"
```
或者，将脚本文件夹放在搜索路径上：```text
_-RunPythonScript "MyScript.py"
```
`_EditPythonScript`打开遗留编辑器；`_ScriptEditor`（Rhino 8）使用Python 3、VB和c#打开统一编辑器。

### RhinoScript （`.rvb`,`.vbs`）

两个步骤：**加载**文件（注册它的subs/functions），然后**运行**一个命名子。```text
_-LoadScript "MyScript.rvb"
_-RunScript MyMainSub
```
一个`.rvb`可以容纳多个子节点；`_RunScript`选择调用哪个。

###搜索路径`Options → Files → Search paths`-当您通过文件名引用脚本时，这里列出的文件夹将被扫描。如果没有这个，你必须给出一个完整的路径。

###启动脚本`Options → RhinoScript → Startup`（和`Options → Python → Startup`） -这些列表中的文件在Rhino打开时运行一次。用于注册自定义命令或别名。

**防止启动代码中丢失文档**：```python
import scriptcontext as sc

def startup():
    if sc.doc is None:
        return

startup()
```
##工具栏按钮和别名

工具栏按钮的**Command**字段只是一个宏。创建一个运行脚本的按钮：```text
! _-RunPythonScript "MyScript.py"
```
将**工具提示**设置为简短的描述；通过按钮编辑器设置图标。

创建别名`Options → Aliases → New`。别名变成键入的命令；它的值就是宏。

##从脚本调用宏```python
import rhinoscriptsyntax as rs
rs.Command("! _-Line 0,0,0 10,0,0", echo=False)
```
`echo=False`抑制命令历史输出，但** *不**抑制提示—始终使用`-`并完成宏字符串中的每个提示。

## rhinocode CLI （Rhino 8）`rhinocode`是Rhino 8命令行工具，用于从外部终端针对正在运行的Rhino实例运行脚本和命令。

基本命令```text
rhinocode script "C:\path\to\MyScript.py"            # run a Python script
rhinocode command "_Circle 0,0,0 5 _Enter"           # run a Rhino command
rhinocode --rhino <instance-id> script "MyScript.py" # target a specific instance
```
`<instance-id>`看起来像`rhinocode_remotepipe_75029`。在Rhino的标题栏或中找到ID
通过在Rhino中运行`StartScriptServer`，它将管道名称打印到命令行。

###架构-管道服务器

rhinocode **不会**生成一个新的Rhino进程。它连接到一个持久服务器，Rhino
暴露(`StartScriptServer`)。脚本在服务器进程中执行，这意味着：

—**环境变量隔离。**在调用shell中设置的变量（`set FOO=bar`）
在脚本中通过`os.environ`是不可见的。服务器在shell之前启动。
**`os.getcwd()`是服务器的工作目录**，而不是您称为rhinocode的目录
从。不要依赖它作为输出路径；显式地传递路径。
- **`print()`输出是管道回**呼叫终端-使用它自由的状态消息。

###向脚本传递数据Rhinocode不支持脚本路径后的位置参数-任何额外的令牌都支持
连接到文件URI上，导致“文件不存在”错误。解决方法:

| | | |如何频道笔记|---|---|---|
|临时文件|调用者将文件写入已知位置；脚本读取并删除它。使用从`__file__`派生的路径（见下文），而不是`%TEMP%`-服务器可能解析不同的临时目录
|犀牛对话|脚本调用`rhinoscriptsyntax.ListBox`/`GetString`|总是工作；用户在Rhino.|中看到一个提示`__file__`是一个URI

当通过rhinocode运行时，`__file__`被设置为带有url编码字符的`file:///`URI
（空格变成`%20`）。在使用它作为文件系统路径之前解码它：```python
import os, sys, urllib.parse

def _script_dir():
    raw = __file__
    if raw.startswith("file:///"):
        raw = urllib.parse.unquote(raw[len("file:///"):])
        if sys.platform == "win32":
            raw = raw.replace("/", os.sep)
    return os.path.dirname(os.path.abspath(raw))
```
