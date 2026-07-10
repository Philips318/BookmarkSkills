# Illustrator JavaScript对象模型快速参考

##包容层次```
Application (app)
 └─ Document
     ├─ Layer
     │   ├─ pathItems[]        → PathItem → PathPoint[]
     │   ├─ compoundPathItems[] → CompoundPathItem
     │   ├─ textFrames[]       → TextFrame
     │   │   ├─ characters[]   → TextRange (single char)
     │   │   ├─ words[]        → TextRange (word)
     │   │   ├─ paragraphs[]   → TextRange (paragraph)
     │   │   ├─ lines[]        → TextRange (line)
     │   │   └─ insertionPoints[]
     │   ├─ placedItems[]      → PlacedItem
     │   ├─ rasterItems[]      → RasterItem
     │   ├─ meshItems[]        → MeshItem
     │   ├─ pluginItems[]      → PluginItem
     │   ├─ graphItems[]       → GraphItem
     │   ├─ symbolItems[]      → SymbolItem → Symbol
     │   ├─ groupItems[]       → GroupItem (recursive pageItems)
     │   ├─ nonNativeItems[]   → NonNativeItem
     │   └─ legacyTextItems[]  → LegacyTextItem
     ├─ Artboard[]
     ├─ Swatch[] / Spot[] / Gradient[] / Pattern[]
     ├─ GraphicStyle[] / Brush[] / Symbol[]
     ├─ Story[]
     ├─ CharacterStyle[] / ParagraphStyle[]
     ├─ Variable[] / Dataset[]
     └─ View[]
```
## Artwork Item Types （pageItems members）

|类型| typename | Collection | Notes ||---|---|---|---|
|路径|`PathItem`|`pathItems`|线条，形状，自由路径|
|复合路径|`CompoundPathItem`|`compoundPathItems`|多路径组合|
|组|`GroupItem`|`groupItems`|包含嵌套的页面项|
|文本框|`TextFrame`|`textFrames`|点、区域或路径文本|
|放置图片|`PlacedItem`|`placedItems`|链接外部文件|
|栅格图像|`RasterItem`|`rasterItems`|嵌入式位图|
|网格|`MeshItem`|`meshItems`|梯度网格对象|
图|`GraphItem`|`graphItems`|Chart/graph对象|
|插件项目|`PluginItem`|`pluginItems`|插件生成的艺术|
|符号实例|`SymbolItem`|`symbolItems`|符号实例|
|非本地|`NonNativeItem`|`nonNativeItems`|外来|
|传统文本|`LegacyTextItem`|`legacyTextItems`| Pre-CS文本对象|

颜色对象类型

|对象|颜色空间|值范围|注释||---|---|---|---|
|`RGBColor`| RGB | 0-255每通道|`.red`，`.green`,`.blue`|
|`.cyan`,`.magenta`,`.yellow`，`.black`|每频道0-100
|`GrayColor`|灰度| 0-100 |`.gray`（0=黑，100=白）|
|`LabColor`| Lab | L: 0-100，a/b: -128至127 |`.l`，`.a`,`.b`|
|`SpotColor`|斑点|色调0-100 |`.spot`，`.tint`|
|`PatternColor`|模式| - |`.pattern`，`.matrix`|
|`GradientColor`|梯度| - |`.gradient`，`.origin`,`.angle`|
|`NoColor`|无| - |Transparent/nofill |

##常用脚本常量

文档和颜色

-`DocumentColorSpace.RGB`/`.CMYK`# # #文本

-`Justification.LEFT`/`.CENTER`/`.RIGHT`/`.FULLJUSTIFY`/`.FULLJUSTIFYLASTLINELEFT`/`.FULLJUSTIFYLASTLINECENTER`/`.FULLJUSTIFYLASTLINERIGHT`-`TextType.POINTTEXT`/`.AREATEXT`/`.PATHTEXT`—`FontBaselineOption.NORMALBASELINE`/`.SUPERSCRIPT`/`.SUBSCRIPT`# # #路径

-`PointType.SMOOTH`/`.CORNER`-`StrokeCap.BUTTENDCAP`/`.ROUNDENDCAP`/`.PROJECTINGENDCAP`—`StrokeJoin.MITERENDJOIN`/`.ROUNDENDJOIN`/`.BEVELENDJOIN`# # #的转换-`Transformation.DOCUMENTORIGIN`/`.BOTTOM`/`.BOTTOMLEFT`/`.BOTTOMRIGHT`/`.CENTER`/`.LEFT`/`.RIGHT`/`.TOP`/`.TOPLEFT`/`.TOPRIGHT`混合模式

-`BlendModes.NORMAL`/`.MULTIPLY`/`.SCREEN`/`.OVERLAY`/`.SOFTLIGHT`/`.COLORDODGE`/`.COLORBURN`/`.DARKEN`/`.LIGHTEN`/`.DIFFERENCE`/`.EXCLUSION`/`.HUE`/`.SATURATIONBLEND`/`.COLORBLEND`/`.LUMINOSITY`元素放置

-`ElementPlacement.PLACEATBEGINNING`/`.PLACEATEND`/`.PLACEBEFORE`/`.PLACEAFTER`/`.INSIDE`# # # z顺序

-`ZOrderMethod.BRINGTOFRONT`/`.SENDTOBACK`/`.BRINGFORWARD`/`.SENDBACKWARD`### Save/Export
—`SaveOptions.SAVECHANGES`/`.DONOTSAVECHANGES`/`.PROMPTTOSAVECHANGES`-`ExportType.PNG24`/`.PNG8`/`.JPEG`/`.SVG`/`.TIFF`/`.PHOTOSHOP`/`.AUTOCAD`/`.FLASH`/`.GIF`-`Compatibility.ILLUSTRATOR8`至`.ILLUSTRATOR24`-`PDFCompatibility.ACROBAT4`至`.ACROBAT8`# # #梯度

-`GradientType.LINEAR`/`.RADIAL`# # #变量

-`VariableKind.TEXTUAL`/`.IMAGE`/`.VISIBILITY`/`.GRAPH`###用户交互

-`UserInteractionLevel.DISPLAYALERTS`/`.DONTDISPLAYALERTS`# # #打印

-`PrintArtworkDesignation.ALLLAYERS`/`.VISIBLELAYERS`/`.VISIBLEPRINTABLELAYERS`单位转换

从|到点|公式||---|---|---|
|英寸|点|`inches * 72`|
|厘米|点|`cm * 28.346`|
|毫米|点|`mm * 2.834645`|
| Picas | Points |`picas * 12`|
| Em单位|点|`(emUnits * fontSize) / 1000`|