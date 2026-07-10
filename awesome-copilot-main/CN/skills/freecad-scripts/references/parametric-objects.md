# FreeCAD参数对象

创建FeaturePython对象、脚本对象、属性、视图提供程序和序列化的参考指南。

##官方维基参考

-[创建参数对象]（https://wiki.freecad.org/Manual:Creating_parametric_objects）
-[创建python特性对象第一部分]（https://wiki.freecad.org/Create_a_FeaturePython_object_part_I）
-[创建python特性对象第二部分]（https://wiki.freecad.org/Create_a_FeaturePython_object_part_II）
-[脚本对象]（https://wiki.freecad.org/Scripted_objects）
[脚本对象保存属性]（https://wiki.freecad.org/Scripted_objects_saving_attributes）
-[脚本对象迁移]（https://wiki.freecad.org/Scripted_objects_migration）
[带附件的脚本对象]（https://wiki.freecad.org/Scripted_objects_with_attachment）
——[Viewprovider] (https://wiki.freecad.org/Viewprovider)
-[自定义树形图标]（https://wiki.freecad.org/Custom_icon_in_tree_view）
——[属性](https://wiki.freecad.org/Property)
- [PropertyLink: InList and OutList]（https://wiki.freecad.org/PropertyLink:_InList_and_OutList）
- [FeaturePython方法]（https://wiki.freecad.org/FeaturePython_methods）

## FeaturePython对象-完整模板```python
import FreeCAD
import Part

class MyParametricObject:
    """Proxy class for a custom parametric object."""

    def __init__(self, obj):
        """Initialize and add properties."""
        obj.Proxy = self
        self.Type = "MyParametricObject"

        # Add custom properties
        obj.addProperty("App::PropertyLength", "Length", "Dimensions",
                         "The length of the object").Length = 10.0
        obj.addProperty("App::PropertyLength", "Width", "Dimensions",
                         "The width of the object").Width = 10.0
        obj.addProperty("App::PropertyLength", "Height", "Dimensions",
                         "The height of the object").Height = 5.0
        obj.addProperty("App::PropertyBool", "Chamfered", "Options",
                         "Apply chamfer to edges").Chamfered = False
        obj.addProperty("App::PropertyLength", "ChamferSize", "Options",
                         "Size of chamfer").ChamferSize = 1.0

    def execute(self, obj):
        """Called when the document is recomputed. Build the shape here."""
        shape = Part.makeBox(obj.Length, obj.Width, obj.Height)
        if obj.Chamfered and obj.ChamferSize > 0:
            shape = shape.makeChamfer(obj.ChamferSize, shape.Edges)
        obj.Shape = shape

    def onChanged(self, obj, prop):
        """Called when any property changes."""
        if prop == "Chamfered":
            # Show/hide ChamferSize based on Chamfered toggle
            if obj.Chamfered:
                obj.setPropertyStatus("ChamferSize", "-Hidden")
            else:
                obj.setPropertyStatus("ChamferSize", "Hidden")

    def onDocumentRestored(self, obj):
        """Called when the document is loaded. Re-initialize if needed."""
        self.Type = "MyParametricObject"

    def __getstate__(self):
        """Serialize the proxy (for saving .FCStd)."""
        return {"Type": self.Type}

    def __setstate__(self, state):
        """Deserialize the proxy (for loading .FCStd)."""
        if state:
            self.Type = state.get("Type", "MyParametricObject")
```
ViewProvider -完成模板```python
import FreeCADGui
from pivy import coin

class ViewProviderMyObject:
    """Controls how the object appears in the 3D view and tree."""

    def __init__(self, vobj):
        vobj.Proxy = self
        # Add view properties if needed
        # vobj.addProperty("App::PropertyColor", "Color", "Display", "Object color")

    def attach(self, vobj):
        """Called when the view provider is attached to the view object."""
        self.Object = vobj.Object
        self.standard = coin.SoGroup()
        vobj.addDisplayMode(self.standard, "Standard")

    def getDisplayModes(self, vobj):
        """Return available display modes."""
        return ["Standard"]

    def getDefaultDisplayMode(self):
        """Return the default display mode."""
        return "Standard"

    def setDisplayMode(self, mode):
        return mode

    def getIcon(self):
        """Return the icon path for the tree view."""
        return ":/icons/Part_Box.svg"
        # Or return an XPM string, or path to a .svg/.png file

    def updateData(self, obj, prop):
        """Called when the model object's data changes."""
        pass

    def onChanged(self, vobj, prop):
        """Called when a view property changes."""
        pass

    def doubleClicked(self, vobj):
        """Called on double-click in the tree."""
        # Open a task panel, for example
        return True

    def setupContextMenu(self, vobj, menu):
        """Add items to the right-click context menu."""
        action = menu.addAction("My Action")
        action.triggered.connect(lambda: self._myAction(vobj))

    def _myAction(self, vobj):
        FreeCAD.Console.PrintMessage("Context menu action triggered\n")

    def claimChildren(self):
        """Return list of child objects to show in tree hierarchy."""
        # return [self.Object.BaseFeature] if hasattr(self.Object, "BaseFeature") else []
        return []

    def __getstate__(self):
        return None

    def __setstate__(self, state):
        return None
```
##创建对象```python
def makeMyObject(name="MyObject"):
    """Factory function to create the parametric object."""
    doc = FreeCAD.ActiveDocument
    if doc is None:
        doc = FreeCAD.newDocument()

    obj = doc.addObject("Part::FeaturePython", name)
    MyParametricObject(obj)

    if FreeCAD.GuiUp:
        ViewProviderMyObject(obj.ViewObject)

    doc.recompute()
    return obj

# Usage
obj = makeMyObject("ChamferedBlock")
obj.Length = 20.0
obj.Chamfered = True
FreeCAD.ActiveDocument.recompute()
```
完成属性类型引用

数字属性

|类型| Python | Notes ||---|---|---|
|`App::PropertyInteger`|`int`|标准整数|
|`App::PropertyFloat`|`float`|标准浮动|
|`App::PropertyLength`|`float`|长度单位（mm） |
|`App::PropertyDistance`|`float`|距离（可为负值）|
|`App::PropertyAngle`|`float`|角度|
|`App::PropertyArea`|`float`|以|为单位的面积
|`App::PropertyVolume`|`float`|以|为单位的卷
|`App::PropertySpeed`|`float`|速度单位|
|`App::PropertyAcceleration`|`float`|加速|
|`App::PropertyForce`|`float`|强制|
|`App::PropertyPressure`|`float`|压力|
|`App::PropertyPercent`|`int`| 0-100整数|
|`App::PropertyQuantity`|`Quantity`|通用单位感知值|
|`App::PropertyIntegerConstraint`|`(val,min,max,step)`|有界整数|
|`App::PropertyFloatConstraint`|`(val,min,max,step)`|有界浮动|

###String/Path属性

|类型| Python | Notes ||---|---|---|
|`App::PropertyString`|`str`|文本字符串|
|`App::PropertyFont`|`str`|字体名称|
|`App::PropertyFile`|`str`|文件路径|
|`App::PropertyFileIncluded`|`str`|嵌入文件|
|`App::PropertyPath`|`str`|目录路径|

###布尔和枚举

|类型| Python | Notes ||---|---|---|
|`App::PropertyBool`|`bool`|True/False|
|`App::PropertyEnumeration`|`list`/`str`|下拉；Set list然后value |```python
# Enumeration usage
obj.addProperty("App::PropertyEnumeration", "Style", "Options", "Style choice")
obj.Style = ["Solid", "Wireframe", "Points"]  # set choices FIRST
obj.Style = "Solid"                              # then set value
```
几何属性

|类型| Python | Notes ||---|---|---|
|`App::PropertyVector`|`FreeCAD.Vector`|三维矢量|
|`App::PropertyVectorList`|`[Vector,...]`|向量列表|
|`App::PropertyPlacement`|`Placement`|位置+旋转|
|`App::PropertyMatrix`|`Matrix`| 4x4矩阵|
|`App::PropertyVectorDistance`|`Vector`|单位为|的矢量
|`App::PropertyPosition`|`Vector`|位置，单位|
|`App::PropertyDirection`|`Vector`|方向矢量|

链接属性

|类型| Python | Notes ||---|---|---|
|`App::PropertyLink`| obj ref |链接到一个对象|
|`App::PropertyLinkList`|`[obj,...]`|链接到多个对象|
|`App::PropertyLinkSub`|`(obj, [subs])`|带子元素|的链接
|`App::PropertyLinkSubList`|`[(obj,[subs]),...]`|多链路+子链路|
|`App::PropertyLinkChild`| obj ref |声明的子链接|
|`App::PropertyLinkListChild`|`[obj,...]`|多个声明子|

形状和材料

|类型| Python | Notes ||---|---|---|
|`Part::PropertyPartShape`|`Part.Shape`|全形状|
|`App::PropertyColor`|`(r,g,b)`|颜色（0.0-1.0）|
|`App::PropertyColorList`|`[(r,g,b),...]`|每个元素的颜色|
|`App::PropertyMaterial`|`Material`|材料定义|

容器属性

|类型| Python | Notes ||---|---|---|
|`App::PropertyPythonObject`|任意|可序列化的Python对象|
|`App::PropertyIntegerList`|`[int,...]`|整数列表|
|`App::PropertyFloatList`|`[float,...]`|浮动列表|
|`App::PropertyStringList`|`[str,...]`|字符串列表|
|`App::PropertyBoolList`|`[bool,...]`|布尔值列表|
|`App::PropertyMap`|`{str:str}`|字符串字典|

对象依赖跟踪```python
# InList: objects that reference this object
obj.InList          # [objects referencing obj]
obj.InListRecursive # all ancestors

# OutList: objects this object references
obj.OutList         # [objects obj references]
obj.OutListRecursive # all descendants
```
版本间迁移```python
class MyParametricObject:
    # ... existing code ...

    def onDocumentRestored(self, obj):
        """Handle version migration when document loads."""
        # Add properties that didn't exist in older versions
        if not hasattr(obj, "NewProp"):
            obj.addProperty("App::PropertyFloat", "NewProp", "Group", "Tip")
            obj.NewProp = default_value

        # Rename properties (copy value, remove old)
        if hasattr(obj, "OldPropName"):
            if not hasattr(obj, "NewPropName"):
                obj.addProperty("App::PropertyFloat", "NewPropName", "Group", "Tip")
                obj.NewPropName = obj.OldPropName
            obj.removeProperty("OldPropName")
```
##附件支持```python
import Part

class MyAttachableObject:
    def __init__(self, obj):
        obj.Proxy = self
        obj.addExtension("Part::AttachExtensionPython")

    def execute(self, obj):
        # The attachment sets the Placement automatically
        if not obj.MapPathParameter:
            obj.positionBySupport()
        # Build your shape at the origin; Placement handles positioning
        obj.Shape = Part.makeBox(10, 10, 10)
```
