# FreeCAD脚本基础

FreeCAD Python脚本基础的参考指南：文档模型、控制台、对象、选择和Python环境。

##官方维基参考

-[温和的介绍]（https://wiki.freecad.org/Manual:A_gentle_introduction）
- [Python入门]（https://wiki.freecad.org/Introduction_to_Python）
- [Python脚本教程]（https://wiki.freecad.org/Python_scripting_tutorial）
- [FreeCAD脚本基础]（https://wiki.freecad.org/FreeCAD_Scripting_Basics）
-[脚本和宏]（https://wiki.freecad.org/Scripting_and_macros）
-[使用宏]（https://wiki.freecad.org/Macros）
-[代码片段]（https://wiki.freecad.org/Code_snippets）
(调试)(https://wiki.freecad.org/Debugging)
[分析](https://wiki.freecad.org/Profiling)
- [Python开发环境]（https://wiki.freecad.org/Python_Development_Environment）
-[额外python模块]（https://wiki.freecad.org/Extra_python_modules）
- [FreeCAD矢量数学库]（https://wiki.freecad.org/FreeCAD_vector_math_library）
-[嵌入FreeCAD]（https://wiki.freecad.org/Embedding_FreeCAD）
-[嵌入FreeCADGui]（https://wiki.freecad.org/Embedding_FreeCADGui）
-[启动宏]（https://wiki.freecad.org/Macro_at_Startup）
-[如何安装宏]（https://wiki.freecad.org/How_to_install_macros）
- [ippython笔记本集成]（https://wiki.freecad.org/IPython_notebook_integration）
(数量)(https://wiki.freecad.org/Quantity)

FreeCAD模块层次结构```
FreeCAD (App)          — Core application, documents, objects, properties
├── FreeCAD.Vector     — 3D vector
├── FreeCAD.Rotation   — Quaternion rotation
├── FreeCAD.Placement  — Position + rotation
├── FreeCAD.Matrix     — 4x4 transformation matrix
├── FreeCAD.Units      — Unit conversion and quantities
├── FreeCAD.Console    — Message output
└── FreeCAD.Base       — Base types

FreeCADGui (Gui)       — GUI module (only when GUI is active)
├── Selection          — Selection management
├── Control            — Task panel management
├── ActiveDocument     — GUI document wrapper
└── getMainWindow()    — Qt main window
```
##文档操作```python
import FreeCAD

# Document lifecycle
doc = FreeCAD.newDocument("DocName")
doc = FreeCAD.openDocument("/path/to/file.FCStd")
doc = FreeCAD.ActiveDocument
FreeCAD.setActiveDocument("DocName")
doc.save()
doc.saveAs("/path/to/newfile.FCStd")
FreeCAD.closeDocument("DocName")

# Object management
obj = doc.addObject("Part::Feature", "ObjectName")
obj = doc.addObject("Part::FeaturePython", "CustomObj")
obj = doc.addObject("App::DocumentObjectGroup", "Group")
doc.removeObject("ObjectName")

# Object access
obj = doc.getObject("ObjectName")
obj = doc.ObjectName                # attribute syntax
all_objs = doc.Objects              # all objects in document
names = doc.findObjects("Part::Feature")  # by type

# Recompute
doc.recompute()                     # recompute all
doc.recompute([obj1, obj2])         # recompute specific objects
obj.touch()                         # mark as needing recompute
```
##选择API```python
import FreeCADGui

# Get selection
sel = FreeCADGui.Selection.getSelection()          # [obj, ...]
sel = FreeCADGui.Selection.getSelection("DocName") # from specific doc
sel_ex = FreeCADGui.Selection.getSelectionEx()      # extended info

# Extended selection details
for s in sel_ex:
    print(s.Object.Name)           # parent object
    print(s.SubElementNames)       # ("Face1", "Edge3", ...)
    print(s.SubObjects)            # actual sub-shapes
    for pt in s.PickedPoints:
        print(pt)                  # 3D pick point

# Set selection
FreeCADGui.Selection.addSelection(obj)
FreeCADGui.Selection.addSelection(obj, "Face1")
FreeCADGui.Selection.removeSelection(obj)
FreeCADGui.Selection.clearSelection()

# Selection observer
class MySelectionObserver:
    def addSelection(self, doc, obj, sub, pos):
        print(f"Selected: {obj}.{sub} at {pos}")
    def removeSelection(self, doc, obj, sub):
        print(f"Deselected: {obj}.{sub}")
    def setSelection(self, doc):
        print(f"Selection set changed in {doc}")
    def clearSelection(self, doc):
        print(f"Selection cleared in {doc}")

obs = MySelectionObserver()
FreeCADGui.Selection.addObserver(obs)
# Later: FreeCADGui.Selection.removeObserver(obs)
```
控制台和日志```python
FreeCAD.Console.PrintMessage("Normal message\n")   # blue/default
FreeCAD.Console.PrintWarning("Warning\n")           # orange
FreeCAD.Console.PrintError("Error\n")               # red
FreeCAD.Console.PrintLog("Debug info\n")            # log only

# Console message observer
class MyLogger:
    def __init__(self):
        FreeCAD.Console.PrintMessage("Logger started\n")
    def receive(self, msg):
        # process msg
        pass
```
单位和数量```python
from FreeCAD import Units

# Create quantities
q = Units.Quantity("10 mm")
q = Units.Quantity("1 in")
q = Units.Quantity(25.4, Units.Unit("mm"))
q = Units.parseQuantity("3.14 rad")

# Convert
value_mm = float(q)                    # internal unit (mm for length)
value_in = q.getValueAs("in")          # convert to other unit
value_m = q.getValueAs("m")

# Available unit schemes: mm/kg/s (FreeCAD default), SI, Imperial, etc.
# Common units: mm, m, in, ft, deg, rad, kg, g, lb, s, min, hr
```
产权制度```python
# Add properties to any DocumentObject
obj.addProperty("App::PropertyFloat", "MyProp", "GroupName", "Tooltip")
obj.MyProp = 42.0

# Check property existence
if hasattr(obj, "MyProp"):
    print(obj.MyProp)

# Property metadata
obj.getPropertyByName("MyProp")
obj.getTypeOfProperty("MyProp")        # returns list: ["App::PropertyFloat"]
obj.getDocumentationOfProperty("MyProp")
obj.getGroupOfProperty("MyProp")

# Set property as read-only, hidden, etc.
obj.setPropertyStatus("MyProp", "ReadOnly")
obj.setPropertyStatus("MyProp", "Hidden")
obj.setPropertyStatus("MyProp", "-ReadOnly")   # remove status
# Statuses: ReadOnly, Hidden, Transient, Output, NoRecompute
```
