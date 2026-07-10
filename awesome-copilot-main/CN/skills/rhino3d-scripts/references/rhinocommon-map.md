# RhinoCommon命名空间映射

使用它来决定为任务导入哪个`Rhino.*`名称空间。所有这些都可以在Python中使用`import Rhino`进行访问。

文档和对象模型

|需要|命名空间|密钥类型||---|---|---|
|活动文件，单位，公差，撤销|`Rhino`|`RhinoDoc`，`RhinoApp`|
|`Rhino.DocObjects`|`RhinoObject`,`ObjectAttributes`,`Layer`,`ObjectType`|
|表格（图层、材质、方块、模糊样式）|`Rhino.DocObjects.Tables`|`LayerTable`、`InstanceDefinitionTable`、`MaterialTable`|
|自定义每对象用户数据|`Rhino.DocObjects.Custom`|`UserData`|
|文件I/O。3dm,import/export) |`Rhino.FileIO`|`File3dm`,`File3dmObject`|

# #几何

|需要|命名空间|密钥类型||---|---|---|
|点，向量，变换|`Rhino.Geometry`|`Point3d`，`Vector3d`,`Transform`,`Plane`,`BoundingBox`|
曲线|`Rhino.Geometry`|`Curve`，`NurbsCurve`,`PolylineCurve`,`LineCurve`,`ArcCurve`|
|表面和底部|`Rhino.Geometry`|`Surface`，`NurbsSurface`,`Brep`,`BrepFace`,`Extrusion`|
|网格|`Rhino.Geometry`|`Mesh`，`MeshFace`,`MeshNgon`|
| SubD |`Rhino.Geometry`|`SubD`,`SubDFace`,`SubDEdge`|
|几何集合（顶点，边，面列表）|`Rhino.Geometry.Collections`|`MeshVertexList`,`BrepEdgeList`|
|路口|`Rhino.Geometry.Intersect`|`Intersection`，`CurveIntersections`|
|网格细化|`Rhino.Geometry.MeshRefinements`| |
|空间变形（弯曲，扭曲等）|`Rhino.Geometry.Morphs`|`BendSpaceMorph`,`TwistSpaceMorph`|

##用户交互

|需要|命名空间|密钥类型||---|---|---|
|提示，getter，命令结果|`Rhino.Input`/`Rhino.Input.Custom`|`RhinoGet`，`GetObject`,`GetPoint`,`GetOption`|
|命令（当构建插件时）|`Rhino.Commands`|`Command`,`Result`|
|表单，对话框，面板|`Rhino.UI`|`Dialog`，`Panels`,`RhinoEtoExtensions`|
| UI控件/数据源|`Rhino.UI.Controls`| |
|口香糖|`Rhino.UI.Gumball`| |

##显示和渲染

|需要|命名空间|密钥类型||---|---|---|
|视口，显示管道，绘制覆盖|`Rhino.Display`|`DisplayPipeline`，`DisplayConduit`,`RhinoViewport`|
|渲染内容（材质，环境）|`Rhino.Render`|`RenderContent`,`RenderMaterial`,`RenderTexture`|
|渲染网格定制|`Rhino.Render.CustomRenderMeshes`| |
|后效果|`Rhino.Render.PostEffects`| |

##运行时和插件

|需要|命名空间|密钥类型||---|---|---|
|插件生命周期，设置|`Rhino.PlugIns`|`PlugIn`，`FileImportPlugIn`,`FileExportPlugIn`|
|进程内Rhino（从外部运行Rhino）。. NET |`Rhino.Runtime.InProcess`|`RhinoCore`|
|本机互操作（指针，封送）|`Rhino.Runtime.InteropWrappers`| |
|用户通知（祝酒）|`Rhino.Runtime.Notifications`| |

##常见模式```python
import Rhino
import scriptcontext as sc
import System.Drawing

doc = sc.doc                                          # Rhino.RhinoDoc
tol = doc.ModelAbsoluteTolerance                      # float
view = doc.Views.ActiveView                           # Rhino.Display.RhinoView
layer_index = doc.Layers.Add("MyLayer", System.Drawing.Color.Red)

# Find a Rhino object from a rhinoscriptsyntax GUID
rhobj = doc.Objects.Find(guid)                        # Rhino.DocObjects.RhinoObject
geom  = rhobj.Geometry                                # Rhino.Geometry.GeometryBase

# Add geometry with attributes
attrs = Rhino.DocObjects.ObjectAttributes()
attrs.LayerIndex = layer_index
attrs.Name = "example"
new_id = doc.Objects.AddCurve(curve, attrs)
```
# #撤销```python
undo_serial = doc.BeginUndoRecord("Batch Op")
try:
    # ... many doc.Objects.* calls ...
    pass
finally:
    doc.EndUndoRecord(undo_serial)
    doc.Views.Redraw()
```
