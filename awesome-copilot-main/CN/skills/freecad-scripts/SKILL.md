---
name: freecad-scripts
description: 'Expert skill for writing FreeCAD Python scripts, macros, and automation. Use when asked to create FreeCAD models, parametric objects, Part/Mesh/Sketcher scripts, workbench tools, GUI dialogs with PySide, Coin3D scenegraph manipulation, or any FreeCAD Python API task. Covers FreeCAD scripting basics, geometry creation, FeaturePython objects, interface tools, and macro development.'
---
#免费脚本

为FreeCAD CAD应用程序生成生产质量Python脚本的专家技能。解释3D建模任务的简写、准代码和自然语言描述，并将它们转换为正确的FreeCAD Python API调用。

何时使用此技能

为FreeCAD的内置控制台或宏系统编写Python脚本
-创建或操作3D几何（零件，网格，Sketcher，路径，FEM）
-用自定义属性构建参数化特征python对象
-在FreeCAD中使用PySide/Qt开发GUI工具
-通过Pivy操纵Coin3D场景
-创建自定义工作台或Gui命令
-使用宏自动化重复CAD操作
-转换之间的网格和实体表示
-脚本FEM分析，光线跟踪，或绘图导出

# #先决条件-安装FreeCAD（推荐0.19+；最新API 0.21+/1.0+）
——Python 3。x（与FreeCAD捆绑）
-用于GUI工作：PySide2（与FreeCAD捆绑）
-场景：Pivy（与FreeCAD捆绑）

##免费Python环境

FreeCAD嵌入了一个Python解释器。脚本运行在以下关键模块可用的环境中：```python
import FreeCAD          # Core module (also aliased as 'App')
import FreeCADGui       # GUI module (also aliased as 'Gui') — only in GUI mode
import Part             # Part workbench — BRep/OpenCASCADE shapes
import Mesh             # Mesh workbench — triangulated meshes
import Sketcher         # Sketcher workbench — 2D constrained sketches
import Draft            # Draft workbench — 2D drawing tools
import Arch             # Arch/BIM workbench
import Path             # Path/CAM workbench
import FEM              # FEM workbench
import TechDraw         # TechDraw workbench (replaces Drawing)
import BOPTools         # Boolean operations
import CompoundTools    # Compound shape utilities
```
FreeCAD文档模型```python
# Create or access a document
doc = FreeCAD.newDocument("MyDoc")
doc = FreeCAD.ActiveDocument

# Add objects
box = doc.addObject("Part::Box", "MyBox")
box.Length = 10.0
box.Width = 10.0
box.Height = 10.0

# Recompute
doc.recompute()

# Access objects
obj = doc.getObject("MyBox")
obj = doc.MyBox  # Attribute access also works

# Remove objects
doc.removeObject("MyBox")
```
##核心概念

矢量和位置```python
import FreeCAD

# Vectors
v1 = FreeCAD.Vector(1, 0, 0)
v2 = FreeCAD.Vector(0, 1, 0)
v3 = v1.cross(v2)          # Cross product
d = v1.dot(v2)              # Dot product
v4 = v1 + v2                # Addition
length = v1.Length           # Magnitude
v_norm = FreeCAD.Vector(v1)
v_norm.normalize()           # In-place normalize

# Rotations
rot = FreeCAD.Rotation(FreeCAD.Vector(0, 0, 1), 45)  # axis, angle(deg)
rot = FreeCAD.Rotation(0, 0, 45)                       # Euler angles (yaw, pitch, roll)

# Placements (position + orientation)
placement = FreeCAD.Placement(
    FreeCAD.Vector(10, 20, 0),    # translation
    FreeCAD.Rotation(0, 0, 45),   # rotation
    FreeCAD.Vector(0, 0, 0)       # center of rotation
)
obj.Placement = placement

# Matrix (4x4 transformation)
import math
mat = FreeCAD.Matrix()
mat.move(FreeCAD.Vector(10, 0, 0))
mat.rotateZ(math.radians(45))
```
创建和操作几何（部分模块）

Part模块封装OpenCASCADE并提供BRep实体建模：```python
import FreeCAD
import Part

# --- Primitive Shapes ---
box = Part.makeBox(10, 10, 10)               # length, width, height
cyl = Part.makeCylinder(5, 20)               # radius, height
sphere = Part.makeSphere(10)                  # radius
cone = Part.makeCone(5, 2, 10)               # r1, r2, height
torus = Part.makeTorus(10, 2)                 # major_r, minor_r

# --- Wires and Edges ---
edge1 = Part.makeLine((0, 0, 0), (10, 0, 0))
edge2 = Part.makeLine((10, 0, 0), (10, 10, 0))
edge3 = Part.makeLine((10, 10, 0), (0, 0, 0))
wire = Part.Wire([edge1, edge2, edge3])

# Circles and arcs
circle = Part.makeCircle(5)                   # radius
arc = Part.makeCircle(5, FreeCAD.Vector(0, 0, 0),
                       FreeCAD.Vector(0, 0, 1), 0, 180)  # start/end angle

# --- Faces ---
face = Part.Face(wire)                        # From a closed wire

# --- Solids from Faces/Wires ---
extrusion = face.extrude(FreeCAD.Vector(0, 0, 10))       # Extrude
revolved = face.revolve(FreeCAD.Vector(0, 0, 0),
                         FreeCAD.Vector(0, 0, 1), 360)    # Revolve

# --- Boolean Operations ---
fused = box.fuse(cyl)           # Union
cut = box.cut(cyl)              # Subtraction
common = box.common(cyl)        # Intersection
fused_clean = fused.removeSplitter()  # Clean up seams

# --- Fillets and Chamfers ---
filleted = box.makeFillet(1.0, box.Edges)          # radius, edges
chamfered = box.makeChamfer(1.0, box.Edges)        # dist, edges

# --- Loft and Sweep ---
loft = Part.makeLoft([wire1, wire2], True)          # wires, solid
swept = Part.Wire([path_edge]).makePipeShell([profile_wire],
                                              True, False)  # solid, frenet

# --- BSpline Curves ---
from FreeCAD import Vector
points = [Vector(0,0,0), Vector(1,2,0), Vector(3,1,0), Vector(4,3,0)]
bspline = Part.BSplineCurve()
bspline.interpolate(points)
edge = bspline.toShape()

# --- Show in document ---
Part.show(box, "MyBox")    # Quick display (adds to active doc)
# Or explicitly:
doc = FreeCAD.ActiveDocument or FreeCAD.newDocument()
obj = doc.addObject("Part::Feature", "MyShape")
obj.Shape = box
doc.recompute()
```
拓扑探索```python
shape = obj.Shape

# Access sub-elements
shape.Vertexes    # List of Vertex objects
shape.Edges       # List of Edge objects
shape.Wires       # List of Wire objects
shape.Faces       # List of Face objects
shape.Shells      # List of Shell objects
shape.Solids      # List of Solid objects

# Bounding box
bb = shape.BoundBox
print(bb.XMin, bb.XMax, bb.YMin, bb.YMax, bb.ZMin, bb.ZMax)
print(bb.Center)

# Properties
shape.Volume
shape.Area
shape.Length       # For edges/wires
face.Surface       # Underlying geometric surface
edge.Curve         # Underlying geometric curve

# Shape type
shape.ShapeType    # "Solid", "Shell", "Face", "Wire", "Edge", "Vertex", "Compound"
```
### Mesh模块```python
import Mesh

# Create mesh from vertices and facets
mesh = Mesh.Mesh()
mesh.addFacet(
    0.0, 0.0, 0.0,   # vertex 1
    1.0, 0.0, 0.0,   # vertex 2
    0.0, 1.0, 0.0    # vertex 3
)

# Import/Export
mesh = Mesh.Mesh("/path/to/file.stl")
mesh.write("/path/to/output.stl")

# Convert Part shape to Mesh
import Part
import MeshPart
shape = Part.makeBox(1, 1, 1)
mesh = MeshPart.meshFromShape(Shape=shape, LinearDeflection=0.1,
                                AngularDeflection=0.5)

# Convert Mesh to Part shape
shape = Part.Shape()
shape.makeShapeFromMesh(mesh.Topology, 0.05)  # tolerance
solid = Part.makeSolid(shape)
```
Sketcher模块

在XY平面上创建一个草图
草图=文件。addObject(“舞台布景设计者::SketchObject”、“MySketch”)
草图。放置=免费的。位置(    FreeCAD.Vector(0, 0, 0),
    FreeCAD.Rotation(0, 0, 0, 1)
）

#添加几何（返回几何索引）
idx_line = sketch.addGeometry(Part.LineSegment（）    FreeCAD.Vector(0, 0, 0), FreeCAD.Vector(10, 0, 0)))
idx_circle = sketch.addGeometry（Part.Circle）    FreeCAD.Vector(5, 5, 0), FreeCAD.Vector(0, 0, 1), 3))
#添加约束
sketch.addConstraint(舞台布景设计者。约束（“重合”，0,2,1,1）)
sketch.addConstraint(舞台布景设计者。约束(“水平”,0))
sketch.addConstraint(舞台布景设计者。约束（"DistanceX", 0,1,0,2,10.0）)
sketch.addConstraint(舞台布景设计者。约束（“半径”，1,3.0）)
sketch.addConstraint(舞台布景设计者。约束（"Fixed", 0,1）)
约束类型：重合，水平，垂直，平行，垂直，
#正切，相等，对称，距离，距离，半径，角度，
#固定（块），内部对齐

doc.recompute ()```

### Draft Module

```python
进口草案
进口FreeCAD

# 2D形状
line = Draft.makeLine向量(0,0,0),FreeCAD。向量(10,0,0))
circle = Draft.makeCircle(5)
rect =草稿makeRectangle(10、5)
聚=草稿。makePolygon(6，半径=5)# hexagon

#操作
草案。(obj, FreeCAD移动。Vector(10,0,0), copy=True)
旋转=草稿。旋转（obj, 45, FreeCAD）。向量(0,0,0),                        axis=FreeCAD.Vector(0,0,1), copy=True)
缩尺=草稿。规模(obj FreeCAD。向量(2 2 2),中心= FreeCAD。向量(0,0,0),                      copy=True)
偏移=汇票。抵消(obj FreeCAD。向量(1,0,0))
数组=草稿。FreeCAD makeArray (obj。向量(15 0 0),                         FreeCAD.Vector(0,15,0), 3, 3)
```

## Creating Parametric Objects (FeaturePython)

FeaturePython objects are custom parametric objects with properties that trigger recomputation:

```python
进口FreeCAD
导入部分

类MyBox:    """A custom parametric box."""

    def __init__(self, obj):
        obj.Proxy = self
        obj.addProperty("App::PropertyLength", "Length", "Dimensions",
                         "Box length").Length = 10.0
        obj.addProperty("App::PropertyLength", "Width", "Dimensions",
                         "Box width").Width = 10.0
        obj.addProperty("App::PropertyLength", "Height", "Dimensions",
                         "Box height").Height = 10.0

    def execute(self, obj):
        """Called on document recompute."""
        obj.Shape = Part.makeBox(obj.Length, obj.Width, obj.Height)

    def onChanged(self, obj, prop):
        """Called when a property changes."""
        pass

    def __getstate__(self):
        return None

    def __setstate__(self, state):
        return None
类ViewProviderMyBox:    """View provider for custom icon and display settings."""

    def __init__(self, vobj):
        vobj.Proxy = self

    def getIcon(self):
        return ":/icons/Part_Box.svg"

    def attach(self, vobj):
        self.Object = vobj.Object

    def updateData(self, obj, prop):
        pass

    def onChanged(self, vobj, prop):
        pass

    def __getstate__(self):
        return None

    def __setstate__(self, state):
        return None
#——用法——
doc = FreeCAD。ActiveDocument或FreeCAD.newDocument（"Test"）
Obj = doc。addObject(“部分::FeaturePython”、“CustomBox”)
MyBox (obj)
ViewProviderMyBox (obj。ViewObject)
doc.recompute ()```

### Common Property Types

| Property Type | Python Type | Description |
|---|---|---|
| `App::PropertyBool` | `bool` | Boolean |
| `App::PropertyInteger` | `int` | Integer |
| `App::PropertyFloat` | `float` | Float |
| `App::PropertyString` | `str` | String |
| `App::PropertyLength` | `float` (units) | Length with units |
| `App::PropertyAngle` | `float` (deg) | Angle in degrees |
| `App::PropertyVector` | `FreeCAD.Vector` | 3D vector |
| `App::PropertyPlacement` | `FreeCAD.Placement` | Position + rotation |
| `App::PropertyLink` | object ref | Link to another object |
| `App::PropertyLinkList` | list of refs | Links to multiple objects |
| `App::PropertyEnumeration` | `list`/`str` | Dropdown selection |
| `App::PropertyFile` | `str` | File path |
| `App::PropertyColor` | `tuple` | RGB color (0.0-1.0) |
| `App::PropertyPythonObject` | any | Serializable Python object |

## Creating GUI Tools

### Gui Commands

```python
进口FreeCAD
进口FreeCADGui

类MyCommand:    """A custom toolbar/menu command."""

    def GetResources(self):
        return {
            "Pixmap": ":/icons/Part_Box.svg",
            "MenuText": "My Custom Command",
            "ToolTip": "Creates a custom box",
            "Accel": "Ctrl+Shift+B"
        }

    def IsActive(self):
        return FreeCAD.ActiveDocument is not None

    def Activated(self):
        # Command logic here
        FreeCAD.Console.PrintMessage("Command activated\n")
FreeCADGui。addCommand(“My_CustomCommand MyCommand ())```

### PySide Dialogs

```python
从PySide2导入QtWidgets， QtCore, QtGui

类MyDialog (QtWidgets。QDialog):    def __init__(self, parent=None):
        super().__init__(parent or FreeCADGui.getMainWindow())
        self.setWindowTitle("My Tool")
        self.setMinimumWidth(300)

        layout = QtWidgets.QVBoxLayout(self)

        # Input fields
        self.label = QtWidgets.QLabel("Length:")
        self.spinbox = QtWidgets.QDoubleSpinBox()
        self.spinbox.setRange(0.1, 1000.0)
        self.spinbox.setValue(10.0)
        self.spinbox.setSuffix(" mm")

        form = QtWidgets.QFormLayout()
        form.addRow(self.label, self.spinbox)
        layout.addLayout(form)

        # Buttons
        btn_layout = QtWidgets.QHBoxLayout()
        self.btn_ok = QtWidgets.QPushButton("OK")
        self.btn_cancel = QtWidgets.QPushButton("Cancel")
        btn_layout.addWidget(self.btn_ok)
        btn_layout.addWidget(self.btn_cancel)
        layout.addLayout(btn_layout)

        self.btn_ok.clicked.connect(self.accept)
        self.btn_cancel.clicked.connect(self.reject)
#使用
dialog = MyDialog（）
如果对话框。exec_() == qtwidgets . qdialog .已接受：    length = dialog.spinbox.value()
    FreeCAD.Console.PrintMessage(f"Length: {length}\n")
```

### Task Panel (Recommended for FreeCAD integration)

```python
类MyTaskPanel:    """Task panel shown in the left sidebar."""

    def __init__(self):
        self.form = QtWidgets.QWidget()
        layout = QtWidgets.QVBoxLayout(self.form)
        self.spinbox = QtWidgets.QDoubleSpinBox()
        self.spinbox.setValue(10.0)
        layout.addWidget(QtWidgets.QLabel("Length:"))
        layout.addWidget(self.spinbox)

    def accept(self):
        # Called when user clicks OK
        length = self.spinbox.value()
        FreeCAD.Console.PrintMessage(f"Accepted: {length}\n")
        FreeCADGui.Control.closeDialog()
        return True

    def reject(self):
        FreeCADGui.Control.closeDialog()
        return True

    def getStandardButtons(self):
        return int(QtWidgets.QDialogButtonBox.Ok |
                   QtWidgets.QDialogButtonBox.Cancel)
#显示面板
panel = MyTaskPanel（）
FreeCADGui.Control.showDialog(面板)```

## Coin3D Scenegraph (Pivy)

```python
源自pivy进口硬币
进口FreeCADGui

#访问scenegraph根目录
sg = FreeCADGui.ActiveDocument.ActiveView.getSceneGraph（）

添加一个自定义的球体分隔符
sep = coin.SoSeparator（）
mat = coin. sommaterial （）
mat.diffuseColor.setValue(1.0, 0.0, 0.0) #
trans = coin.SoTranslation（）
trans.translation。setValue（10,10,10）
sphere = coin.SoSphere（）
sphere.radius.setValue (2.0)
sep.addChild(垫)
sep.addChild(反式)
sep.addChild(球)
sg.addChild(9月)

#稍后移除
sg.removeChild(9月)```

## Custom Workbench Creation

```python
进口FreeCADGui

类MyWorkbench (FreeCADGui。工作台):    MenuText = "My Workbench"
    ToolTip = "A custom workbench"
    Icon = ":/icons/freecad.svg"

    def Initialize(self):
        """Called at workbench activation."""
        import MyCommands  # Import your command module
        self.appendToolbar("My Tools", ["My_CustomCommand"])
        self.appendMenu("My Menu", ["My_CustomCommand"])

    def Activated(self):
        pass

    def Deactivated(self):
        pass

    def GetClassName(self):
        return "Gui::PythonWorkbench"
FreeCADGui.addWorkbench (MyWorkbench)```

## Macro Best Practices

```python
#标准宏头
# -*-编码：utf-8 -*-
# FreeCAD宏：MyMacro
# Description：对宏功能的简要描述
#作者：你的名字
#版本：1.0
日期：2026-04-07

进口FreeCAD
导入部分
从FreeCAD导入库

#保护GUI可用性
如果FreeCAD。GuiUp:    import FreeCADGui
    from PySide2 import QtWidgets, QtCore
def main ():    doc = FreeCAD.ActiveDocument
    if doc is None:
        FreeCAD.Console.PrintError("No active document\n")
        return

    if FreeCAD.GuiUp:
        sel = FreeCADGui.Selection.getSelection()
        if not sel:
            FreeCAD.Console.PrintWarning("No objects selected\n")

    # ... macro logic ...

    doc.recompute()
    FreeCAD.Console.PrintMessage("Macro completed\n")
如果__name__ == "__main__"：    main()
```

### Selection Handling

```python
#获取选定对象
sel = FreeCADGui.Selection.getSelection() #对象列表
sel_ex = freecadgui . select . getselectionex () # Extended（子元素）

对于sel_ex中的selobj：    obj = selobj.Object
    for sub in selobj.SubElementNames:
        print(f"{obj.Name}.{sub}")
        shape = obj.getSubObject(sub)  # Get sub-shape
#以编程方式选择
FreeCADGui.Selection.addSelection (doc.MyBox)
FreeCADGui.Selection.addSelection (doc。MyBox,“Face1”)
FreeCADGui.Selection.clearSelection ()```

### Console Output

```python
FreeCAD.Console。PrintMessage(“信息消息\ n”)
FreeCAD.Console。PrintWarning(“警告消息\ n”)
FreeCAD.Console。PrintError(“错误消息\ n”)
FreeCAD.Console。PrintLog(“Debug/log消息\ n”)```

## Common Patterns

### Parametric Pad from Sketch

```python
doc = FreeCAD。ActiveDocument

#创建草图
草图=文件。addObject(“舞台布景设计者::SketchObject”、“素描”)
sketch.addGeometry (Part.LineSegment (FreeCAD。向量(0,0,0),FreeCAD。向量(10 0 0)))
sketch.addGeometry (Part.LineSegment (FreeCAD。向量(10,0,0),FreeCAD。向量(10 10 0)))
sketch.addGeometry (Part.LineSegment (FreeCAD。向量(10 10 0),FreeCAD。向量(0 10 0)))
sketch.addGeometry (Part.LineSegment (FreeCAD。向量(0 10 0),FreeCAD。向量(0,0,0)))
#关闭与重合约束
对于(3)范围内的I：    sketch.addConstraint(Sketcher.Constraint("Coincident", i, 2, i+1, 1))
sketch.addConstraint(舞台布景设计者。约束（“重合”，3,2,0,1）)

# Pad （PartDesign）
Pad = doc。addObject(“PartDesign:垫”,“垫”)
垫。轮廓=草图
垫。长度= 5.0
草图。可见性= False
doc.recompute ()```

### Export Shapes

```python
# STEP导出
Part.export([医生。MyBox) /path/to/output.step”)

# STL export （mesh）
导入网格
Mesh.export([医生。MyBox) /path/to/output.stl”)

# IGES导出
Part.export([医生。MyBox) /path/to/output.iges”)

#多种格式通过importlib
进口importlib
importlib.import_module(“importOBJ”). export (doc。MyBox) /path/to/output.obj”)```

### Units and Quantities

```python
# FreeCAD内部使用mm
q = FreeCAD.Units。数量(10毫米)
q_inch = FreeCAD.Units。数量(1)
打印(q_inch。getvaluea ("mm")) # 25.4

#用单位解析用户输入
q = FreeCAD.Units.parseQuantity（"2.5 in"）
value_mm = float(q) #内部单位mm的值```

## Compensation Rules (Quasi-Coder Integration)

When interpreting shorthand or quasi-code for FreeCAD scripts:

1. **Terminology mapping**: "box" → `Part.makeBox()`, "cylinder" → `Part.makeCylinder()`, "sphere" → `Part.makeSphere()`, "merge/combine/join" → `.fuse()`, "subtract/cut/remove" → `.cut()`, "intersect" → `.common()`, "round edges/fillet" → `.makeFillet()`, "bevel/chamfer" → `.makeChamfer()`
2. **Implicit document**: If no document handling is mentioned, wrap in standard `doc = FreeCAD.ActiveDocument or FreeCAD.newDocument()`
3. **Units assumption**: Default to millimeters unless stated otherwise
4. **Recompute**: Always call `doc.recompute()` after modifications
5. **GUI guard**: Wrap GUI-dependent code in `if FreeCAD.GuiUp:` when the script may run headless
6. **Part.show()**: Use `Part.show(shape, "Name")` for quick display, or `doc.addObject("Part::Feature", "Name")` for named persistent objects

## References

### Primary Links

- [Writing Python code](https://wiki.freecad.org/Manual:A_gentle_introduction#Writing_Python_code)
- [Manipulating FreeCAD objects](https://wiki.freecad.org/Manual:A_gentle_introduction#Manipulating_FreeCAD_objects)
- [Vectors and Placements](https://wiki.freecad.org/Manual:A_gentle_introduction#Vectors_and_Placements)
- [Creating and manipulating geometry](https://wiki.freecad.org/Manual:Creating_and_manipulating_geometry)
- [Creating parametric objects](https://wiki.freecad.org/Manual:Creating_parametric_objects)
- [Creating interface tools](https://wiki.freecad.org/Manual:Creating_interface_tools)
- [Python](https://en.wikipedia.org/wiki/Python_%28programming_language%29)
- [Introduction to Python](https://wiki.freecad.org/Introduction_to_Python)
- [Python scripting tutorial](https://wiki.freecad.org/Python_scripting_tutorial)
- [FreeCAD scripting basics](https://wiki.freecad.org/FreeCAD_Scripting_Basics)
- [Gui Command](https://wiki.freecad.org/Gui_Command)

### Bundled Reference Documents

See the [references/](references/) directory for topic-organized guides:

1. [scripting-fundamentals.md](references/scripting-fundamentals.md) — Core scripting, document model, console
2. [geometry-and-shapes.md](references/geometry-and-shapes.md) — Part, Mesh, Sketcher, topology
3. [parametric-objects.md](references/parametric-objects.md) — FeaturePython, properties, scripted objects
4. [gui-and-interface.md](references/gui-and-interface.md) — PySide, dialogs, task panels, Coin3D
5. [workbenches-and-advanced.md](references/workbenches-and-advanced.md) — Workbenches, macros, FEM, Path, recipes
