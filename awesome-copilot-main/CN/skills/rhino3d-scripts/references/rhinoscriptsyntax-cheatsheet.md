#`rhinoscriptsyntax`小抄```python
import rhinoscriptsyntax as rs
```
##用户输入

|函数|返回||---|---|
|`rs.GetObject(message, filter, preselect, select)`| GUID或`None`|
|`rs.GetObjects(message, filter)`| guid列表
|`rs.GetPoint(message, base_point)`|`Point3d`或`None`|
|`rs.GetString(message, default, strings)`| STR |
|`rs.GetInteger`,`rs.GetReal`| int / float |
|`rs.GetBoolean(message, items, defaults)`| bool列表|

常见的`rs.filter.*`标志（或它们一起）：```
point=1, point_cloud=2, curve=4, surface=8, polysurface=16,
mesh=32, light=256, annotation=512, instance_reference=4096,
text_dot=8192, grip=16384, detail=32768, hatch=65536,
morph_control=131072, sub_d=262144
```
##创建几何

|功能| Notes ||---|---|
|`rs.AddPoint(point)`| |
|`rs.AddLine(start, end)`| |
|`rs.AddPolyline(points)`|`points`是一个三元组的列表|
|`rs.AddCircle(plane_or_center, radius)`| |
|`rs.AddArc(plane, radius, angle_deg)`| **度** |
|`rs.AddCurve(points, degree=3)`| NURBS通过控制点|
|`rs.AddInterpCurve(points, degree=3)`| NURBS通过|点
|`rs.AddSphere(center, radius)`| |
|`rs.AddBox(corners)`|`corners`= 8分|
|`rs.AddPlanarSrf(curves)`|返回列表|
|`rs.AddLoftSrf(curves, ...)`|返回guid列表|
|`rs.AddExtrusion(profile, path)`/`rs.ExtrudeCurveStraight`| |

##对象属性

|功能|用途||---|---|
|`rs.ObjectLayer(id [, layer])`|get/set|
|`rs.ObjectColor(id [, color])`| RGB元组|
|`rs.ObjectName(id [, name])`| |
|`rs.ObjectType(id)`| int匹配`rs.filter.*`|
|`rs.IsCurve / IsSurface / IsBrep / IsMesh / IsPoint(id)`| |
|`rs.DeleteObject(id)`/`rs.DeleteObjects(ids)`| |
|`rs.CopyObject(id, translation)`| |
|`rs.MoveObject(id, translation)`| |
|`rs.RotateObject(id, center, angle, axis=None, copy=False)`|角度|
|`rs.ScaleObject(id, origin, scale)`|是一个三元组|

# #曲线

|功能| ||---|---|
|`rs.CurveLength(id)`| |
|`rs.CurveDomain(id)`|`(t0, t1)`|
|`rs.EvaluateCurve(id, t)`|`Point3d`|
|`rs.CurveStartPoint / CurveEndPoint(id)`| |
|`rs.CurveClosestPoint(id, point)`|参数`t`|
|`rs.DivideCurve(id, segments, create_points=False, return_points=True)`| |
|`rs.IsCurveClosed / IsCurvePlanar(id)`| |

# #层

|功能| ||---|---|
|`rs.AddLayer(name, color=None, visible=True, locked=False, parent=None)`| |
|`rs.CurrentLayer([layer])`| |
|`rs.LayerNames()`| list |
|`rs.LayerVisible(name [, visible])`| |
|`rs.DeleteLayer(name)`| |
|`rs.ObjectsByLayer(name)`| guid列表

文档和视图

|功能| ||---|---|
|`rs.UnitAbsoluteTolerance()`| |
|`rs.UnitSystem()`| int (`rs.unit_system_*`) |
|`rs.EnableRedraw(enable)`| **在bulk ops** |周围切换
|`rs.Redraw()`|强制重新绘制|
|`rs.ViewNames()`/`rs.CurrentView([name])`| |
|`rs.ZoomExtents(view=None, all=False)`| |

# #选择

|功能| ||---|---|
|`rs.SelectedObjects()`| list |
|`rs.SelectObject(id)`/`rs.SelectObjects(ids)`| |
|`rs.UnselectAllObjects()`| |
|`rs.InvertSelectedObjects()`| |

##脚本中的宏`rs.Command(command_string, echo=True)`运行宏，就像在命令行输入一样。总是前缀`!`（取消）和`-`（无对话框）：```python
rs.Command("! _-Line 0,0,0 10,0,0", echo=False)
```
