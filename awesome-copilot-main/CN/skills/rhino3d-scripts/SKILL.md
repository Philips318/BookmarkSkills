---
name: rhino3d-scripts
description: 'Authoring and debugging scripts for Rhinoceros 3D (Rhino 8 and later). Use when asked to write RhinoScript (VBScript / .rvb / .vbs), RhinoPython, or RhinoCommon-based scripts; automate Rhino modeling tasks; build command macros; manipulate Rhino geometry, layers, blocks, or document objects; pick objects from the viewport; control redraw and undo; or load and run scripts from the Rhino Script Editor. Covers `rhinoscriptsyntax`, `scriptcontext`, the `Rhino.*` RhinoCommon namespaces (`Rhino.Geometry`, `Rhino.DocObjects`, `Rhino.Input`, `Rhino.UI`, `Rhino.Display`, `Rhino.FileIO`), and the Rhino 8 unified Script Editor.'
---
# Rhino 3D脚本技能

为Rhinoceros 3D编写生产质量脚本。涵盖三个脚本表面（RhinoScript/VBScript、rhinpython、直接RhinoCommon）。. NET)和Rhino 8+脚本编辑器。

何时使用此技能

-用户要求编写、编辑或调试`.rvb`、`.vbs`或`.py`犀牛脚本
-用户想要一个Rhino **命令宏**或想要自动化一系列Rhino命令
-用户希望从代码中操作几何，图层，块，材料，视口或注释
-用户提到`rhinoscriptsyntax`，`scriptcontext`,`RhinoCommon`,`Rhino.Geometry`,`RhinoDoc`，或脚本编辑器
-用户想要选择对象，提示输入，或者在Rhino中构建一个小的UI
-用户询问如何加载，运行或分发脚本（启动脚本，别名，工具栏按钮）

选择一个脚本表面

根据任务选择表面，而不是偏好。对于新工作，默认推荐使用Python。|表面|何时选择|文件ext ||---|---|---|
| ** rhinpython ** (`rhinoscriptsyntax`+ RhinoCommon) |默认用于新脚本。最好的生态系统，可读，完整的RhinoCommon访问。|`.py`|
| **RhinoScript** (VBScript) |维护旧`.rvb`/`.vbs`文件；与VBA/COM.|`.rvb`xqz |积分
** |性能关键循环，复杂的几何形状，利用。网库。|`.cs`|
| **命令宏** |现有Rhino命令的纯序列；没有逻辑。|toolbar/alias|

宏** *不是**脚本——它是命令行输入的字符串（例如`! _-Line 0,0,0 10,0,0 _Enter`）。在需要变量、循环或条件时使用脚本。

# #先决条件- Rhino 7或更高版本（强烈推荐Rhino 8 -统一脚本编辑器在一个窗口中支持Python 3， VB和c#）。
-脚本编辑器：输入`_ScriptEditor`（Rhino 8）或`_EditPythonScript`/`_EditScript`（旧版本）。
—使用“`_-RunPythonScript`”或“`_LoadScript`+`_RunScript`”命令行运行已保存的文件。

##核心模式

### Python：最小化脚手架```python
import rhinoscriptsyntax as rs
import scriptcontext as sc
import Rhino

def main():
    obj_id = rs.GetObject("Select a curve", filter=rs.filter.curve, preselect=True)
    if not obj_id:
        return
    length = rs.CurveLength(obj_id)
    print("Length: {0:.4f}".format(length))

if __name__ == "__main__":
    main()
```
Python：直接使用RhinoCommon```python
import Rhino
import scriptcontext as sc

doc = sc.doc  # Rhino.RhinoDoc.ActiveDoc
tol = doc.ModelAbsoluteTolerance

circle = Rhino.Geometry.Circle(Rhino.Geometry.Point3d(0, 0, 0), 5.0)
curve_id = doc.Objects.AddCircle(circle)
doc.Views.Redraw()
```
VBScript：最小化脚手架```vbscript
Option Explicit

Call Main()

Sub Main()
    Dim strObject
    strObject = Rhino.GetObject("Select a curve", 4)  ' 4 = curve filter
    If IsNull(strObject) Then Exit Sub
    Rhino.Print "Length: " & Rhino.CurveLength(strObject)
End Sub
```
###使用自定义过滤器挑选对象（Python, RhinoCommon）```python
import Rhino
import scriptcontext as sc

go = Rhino.Input.Custom.GetObject()
go.SetCommandPrompt("Select breps")
go.GeometryFilter = Rhino.DocObjects.ObjectType.Brep
go.SubObjectSelect = False
go.GetMultiple(1, 0)
if go.CommandResult() != Rhino.Commands.Result.Success:
    pass
else:
    ids = [go.Object(i).ObjectId for i in range(go.ObjectCount)]
```
##分步工作流程

快速批量修改多个对象

1. 禁用重绘：`rs.EnableRedraw(False)`。
2. 在单个撤销记录中包装突变：`undo = doc.BeginUndoRecord("My Op")`…`doc.EndUndoRecord(undo)`。
3. 直接在循环中使用RhinoCommon（跳过`rhinoscriptsyntax`开销）。
4. 重新启用重绘并在`try`/`finally`中调用`doc.Views.Redraw()`，这样崩溃就不会使视口冻结。

将脚本分发给队友

1. 将`.py`/`.rvb`保存在磁盘的某个地方。
2. 将文件夹添加到`Options → Files → Search paths`，以便Rhino可以通过名称找到它。
3. 创建一个工具栏按钮或别名，其宏为：
—Python:`! _-RunPythonScript "MyScript.py"`—RhinoScript:`! _-LoadScript "MyScript.rvb" _-RunScript MySubName`4. 前面的`!`取消任何正在运行的命令；`-`以脚本（无对话框）模式运行命令。

###在Rhino启动时运行代码

1. 在搜索路径中放置`.rvb`/`.py`。
2.`Tools → Options → RhinoScript`（或`Python`）→添加到**启动**列表。该文件在每个会话中执行一次。

# #陷阱- **`rhinoscriptsyntax`返回guid， RhinoCommon返回对象。**混合它们是好的，但`doc.Objects.Find(guid)`是从`rs.*`id到`RhinoObject`的桥梁。
- **坐标随曲面不同而不同。** Python使用`(x, y, z)`元组*或*`Rhino.Geometry.Point3d`；VBScript使用3元素`Array(x, y, z)`。永远不要通过COM将Python列表传递给VBScript助手。
- **`Option Explicit`在VBScript中默认关闭。**输入错误会静默地创建新的变量。总是在`.rvb`文件的顶部添加`Option Explicit`。
- **VBScript没有块作用域。**`Sub`中的所有`Dim`都被提升到程序的顶部。循环计数器泄漏。
—**`Nothing`、`Empty`、`Null`在VBScript中不同**。`IsNull`用于`Rhino.GetObject`失败，`IsEmpty`用于未初始化的`Variant`，`Is Nothing`用于对象引用。
- **括号改变调用语义在VBScript。**`Call Foo(a, b)`和`Foo a, b`有效；`Foo(a, b)`（不是`Call`，带父元素）不是对Sub的调用——它是多参数子节点的语法错误，单参数子节点的强制`ByVal`。
- **公差是每个文档。**总是读取`doc.ModelAbsoluteTolerance`，而不是硬编码`0.001`；用户以毫米、米、英寸等单位工作。
- **长循环应该轮询`Rhino.RhinoApp.EscapeKeyPressed`**所以用户可以取消。否则，犀牛就会被冻住。
- **GUID字符串vs`System.Guid`。**`rhinoscriptsyntax`均可接受；RhinoCommon想要`System.Guid`。如果需要，用`System.Guid(str_id)`转换。
- **不要在紧循环中调用`doc.Views.Redraw()`。**切换重画一次外循环。
**`.rvb`只是`.vbs`改名**与犀牛特定的扩展，所以犀牛的`LoadScript`识别它。相同的VBScript引擎。
**`Rhino.RhinoApp.IsHeadless`可能不存在于老版本的Rhino 8上。**使用`getattr(Rhino.RhinoApp, "IsHeadless", None)`，使用前检查`None`。退回到启发式（例如`sc.doc.Views.Count == 0`）或假设GUI存在。
**`RhinoMath`位于`Rhino.RhinoMath`，而不是`Rhino.DocObjects.RhinoMath`。**访问`Rhino.DocObjects.RhinoMath`会引发`AttributeError`。
- **`doc.Objects.AddBrep()`返回`System.Guid.Empty`ilure。**在Rhino 8 CPython中，`System`命名空间可能不能直接导入；检查返回值是否为字符串：`str(obj_id) == "00000000-0000-0000-0000-000000000000"`。
- **`rhinoscriptsyntax`没有类型存根。**静态分析器（Pylance/Pyright）将`import rhinoscriptsyntax as rs`标记为不可解析。在进口线上用`# type: ignore`压制；该模块在Rhino运行时始终可用。
- **永远不要以Python标准库模块**命名脚本（例如`random.py`，`math.py`,`os.py`）。IronPython 2.7 （`_-RunPythonScript`）在stdlib之前解析脚本目录，因此stdlib中的任何`import random`（例如`tempfile`在内部导入`random`）将找到您的文件，并使用`Cannot import name <X>`失败。CPython 3 （`rhinocode`）不受影响，因为它首先解析stdlib。重命名脚本，或者避免导入使用阴影名称的模块。
- **Em破破号和其他非ascii字符静默中断`_-RunPythonScript`（IronPython 2.7）。**`rhinocode script`使用CPython 3（默认为UTF-8）相同的文件在那里工作，使得失败不明显。IronPython 2.7在第一个违规字节引发`SyntaxError: Non-ASCII character '\xe2'`。最常见的罪魁祸首是**em破折号** （`--`被许多编辑器自动转换为`--`）。将`# -*- coding: utf-8 -*-`添加到必须在这两种运行时下运行的每个脚本的第一行，并用等效的ASCII字符替换印刷字符：em - dash`--`、箭头`->`、乘法`x`。# #故障排除

|修复||---|---|
|`rs.GetObject`立即返回`None`|用户按了Escape，或者您的`filter`排除了所有内容。重新检查`rs.filter.*`标志。|
|文件夹不在`Options → Files → Search paths`中。|
你传递了一个2元素的数组。Rhino需要3元素`Array(x, y, z)`。|
你在Rhino外部运行CPython。RhinoCommon仅在Rhino的嵌入式Python中可用（或通过`rhino3dm`用于只读文件工作）。|
|您忘记了`doc.Views.Redraw()`，或者`rs.EnableRedraw(False)`从未重新启用。|
|将批处理包装在`BeginUndoRecord`/`EndUndoRecord`中。|
| startup在打开任何文档之前运行-在`sc.doc is None`时提前返回或跳过与文档相关的工作。|
|`rs.Command("...")`返回`False`|宏字符串畸形。前缀为`!`和`-`，用`_Enter`或一个值结束每个提示符。|
|`AttributeError: type object 'RhinoApp' has no attribute 'IsHeadless'`|属性在后来的Rhino 8构建中添加。使用`getattr(Rhino.RhinoApp, "IsHeadless", None)`，防范`None`。|
|`rhinocode script`忽略脚本路径之后的参数| rhinocode将额外的令牌连接到文件URI上。通过临时文件或Rhino对话框传递数据。看到`references/macros-and-loading.md`。|
|`Cannot import name <X>`在stdlib（例如`tempfile`，`os`）当使用`_-RunPythonScript`|脚本文件名阴影stdlib模块（例如`random.py`阴影`random`）。IronPython 2.7在stdlib之前搜索脚本目录。重命名脚本，或删除`import`，将其拉入阴影模块并将其替换为直接替代（例如，通过`os.environ`读取`%TEMP%`，而不是`import tempfile`）。|
IronPython 2.7 （`_-RunPythonScript`）击中一个em - dash或类似的字符。添加`# -*- coding: utf-8 -*-`作为第一行，或替换字符：em -`--`，箭头`->`。相同的文件在xqz39xq下运行良好z (CPython 3)，它隐藏了问题。|# #引用

- [references/rhinoscriptsyntax-cheatsheet.md](references/rhinoscriptsyntax-cheatsheet.md) -按类别列出最常用的`rs.*`函数。
—[references/rhinocommon-map.md]（references/rhinocommon-map.md）—为哪个任务导入哪个命名空间。
- [references/macros-and-loading.md](references/macros-and-loading.md) -命令行宏语法，`LoadScript`/`RunScript`，搜索路径。
- [references/vbscript-quirks.md](references/vbscript-quirks.md) - VBScript-only陷阱相关的RhinoScript。

上游文档

—RhinoScript登录：<https://docs.mcneel.com/rhino/8/help/en-us/information/rhinoscripting.htm>-开发者中心：<https://developer.rhino3d.com/>—RhinoCommon API索引：<https://mcneel.github.io/rhinocommon-api-docs/api/RhinoCommon/html/R_Project_RhinoCommon.htm>—脚本示例repo:<https://github.com/mcneel/rhino-developer-samples/tree/8/rhinoscript>