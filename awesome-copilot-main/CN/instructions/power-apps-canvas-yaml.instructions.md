---
description: 'Comprehensive guide for working with Power Apps Canvas Apps YAML structure based on Microsoft Power Apps YAML schema v3.0. Covers Power Fx formulas, control structures, data types, and source control best practices.'
applyTo: '**/*.{yaml,yml,md,pa.yaml}'
---
# Power Apps Canvas Apps YAML结构指南

# #概述
本文档提供了基于官方Microsoft Power Apps YAML模式（v3.0）和Power Fx文档的Power Apps画布应用的YAML代码的全面说明。

**官方模式来源**:https://raw.githubusercontent.com/microsoft/PowerApps-Tooling/refs/heads/master/schemas/pa-yaml/v3.0/pa.schema.yaml功率Fx设计原则
Power Fx是Power Apps canvas应用中使用的公式语言。它遵循以下核心原则：

设计原则
- **简单**：使用Excel公式中熟悉的概念
- **Excel一致性**：与Excel公式语法和行为保持一致
-声明式：描述你想要什么，而不是如何实现它
- **功能**：避免副作用；大多数函数都是纯函数
- **组合**：由简单的功能组合而成的复杂逻辑
—**强类型**：类型系统保证数据的完整性
- **集成**：跨电源平台无缝工作语言哲学
Power Fx促进：
-通过熟悉的excel类公式进行低代码开发
-当依赖关系改变时自动重新计算
-具有编译时检查的类型安全
-函数式编程模式

根结构
每个Power Apps YAML文件都遵循这个顶层结构：```yaml
App:
  Properties:
    # App-level properties and formulas
    StartScreen: =Screen1

Screens:
  # Screen definitions

ComponentDefinitions:
  # Custom component definitions

DataSources:
  # Data source configurations

EditorState:
  # Editor metadata (screen order, etc.)
```
# # 1。应用部分`App`节定义应用程序级属性和配置。```yaml
App:
  Properties:
    StartScreen: =Screen1
    BackEnabled: =false
    # Other app properties with Power Fx formulas
```
###重点：
-包含应用程序范围的设置
-属性使用Power Fx公式（前缀为`=`）
-`StartScreen`属性通常指定

# # 2。屏幕部分
将应用程序中的所有屏幕定义为无序映射。```yaml
Screens:
  Screen1:
    Properties:
      # Screen properties
    Children:
      - Label1:
          Control: Label
          Properties:
            Text: ="Hello World"
            X: =10
            Y: =10
      - Button1:
          Control: Button
          Properties:
            Text: ="Click Me"
            X: =10
            Y: =100
```
###屏幕结构：
—**属性**：屏幕级属性和公式
- **Children**：屏幕上的控件数组（按z-index排序）

控制定义格式：```yaml
ControlName:
  Control: ControlType      # Required: Control type identifier
  Properties:
    PropertyName: =PowerFxFormula
  # Optional properties:
  Group: GroupName          # For organizing controls in Studio
  Variant: VariantName      # Control variant (affects default properties)
  MetadataKey: Key          # Metadata identifier for control
  Layout: LayoutName        # Layout configuration
  IsLocked: true/false      # Whether control is locked in editor
  Children: []              # For container controls (ordered by z-index)
```
控制版本：
您可以使用`@`操作符指定控件版本：```yaml
MyButton:
  Control: Button@2.1.0     # Specific version
  Properties:
    Text: ="Click Me"

MyLabel:
  Control: Label            # Uses latest version by default
  Properties:
    Text: ="Hello World"
```
# # 3。控制类型

标准控件
常见的第一方控制包括：
- **基本控件**:`Label`，`Button`,`TextInput`,`HTMLText`- **输入控制：`Slider`，`Toggle`,`Checkbox`,`Radio`,`Dropdown`,`Combobox`,`DatePicker`,`ListBox`- **显示控件**:`Image`，`Icon`,`Video`,`Audio`,`PDF viewer`,`Barcode scanner`- **布局控件**:`Container`，`Rectangle`,`Circle`,`Gallery`,`DataTable`,`Form`- **图表控件**:`Column chart`，`Line chart`,`Pie chart`**高级控件**:`Timer`，`Camera`,`Microphone`,`Add picture`,`Import`,`Export`容器和布局控件
对容器控件及其子控件的特别注意：```yaml
MyContainer:
  Control: Container
  Properties:
    Width: =300
    Height: =200
    Fill: =RGBA(240, 240, 240, 1)
  Children:
    - Label1:
        Control: Label
        Properties:
          Text: ="Inside Container"
          X: =10         # Relative to container
          Y: =10         # Relative to container
    - Button1:
        Control: Button
        Properties:
          Text: ="Container Button"
          X: =10
          Y: =50
```
自定义组件```yaml
MyCustomControl:
  Control: Component
  ComponentName: MyComponent
  Properties:
    X: =10
    Y: =10
    # Custom component properties
```
代码组件（PCF）```yaml
MyPCFControl:
  Control: CodeComponent
  ComponentName: publisherprefix_namespace.classname
  Properties:
    X: =10
    Y: =10
```
# # 4。组件定义
定义可重用的自定义组件：```yaml
ComponentDefinitions:
  MyComponent:
    DefinitionType: CanvasComponent
    Description: "A reusable component"
    AllowCustomization: true
    AccessAppScope: false
    CustomProperties:
      InputText:
        PropertyKind: Input
        DataType: Text
        Description: "Input text property"
        Default: ="Default Value"
      OutputValue:
        PropertyKind: Output
        DataType: Number
        Description: "Output number value"
    Properties:
      Fill: =RGBA(255, 255, 255, 1)
      Height: =100
      Width: =200
    Children:
      - Label1:
          Control: Label
          Properties:
            Text: =Parent.InputText
```
自定义属性类型：
—**输入**：接收父节点的值
—**输出**：向父节点发送值
—**InputFunction**：父级调用的函数
—**OutputFunction**：在组件中定义的函数
—**Event**：向父节点触发事件
- **作用**：有副作用的功能

###数据类型：
-`Text`,`Number`,`Boolean`-`DateAndTime`,`Color`,`Currency`-`Record`,`Table`,`Image`-`VideoOrAudio`,`Screen`# # 5。数据源
配置数据连接：```yaml
DataSources:
  MyTable:
    Type: Table
    Parameters:
      TableLogicalName: account

  MyActions:
    Type: Actions
    ConnectorId: shared_office365users
    Parameters:
      # Additional connector parameters
```
数据源类型：
- **表**：数据回避表或其他表格数据
—**Actions**：连接器动作和流

# # 6。编辑状态
维护编辑组织：```yaml
EditorState:
  ScreensOrder:
    - Screen1
    - Screen2
    - Screen3
  ComponentDefinitionsOrder:
    - MyComponent
    - AnotherComponent
```
##功率Fx公式指南

公式语法：
—所有公式必须以`=`开头
-使用Power Fx语法的表达式
-空值可以表示为`null`（不带引号）
例子:  ```yaml
  Text: ="Hello World"
  X: =10
  Visible: =Toggle1.Value
  OnSelect: =Navigate(Screen2, ScreenTransition.Fade)
  OptionalProperty: null    # Represents no value
  ```
常用的公式模式：```yaml
# Static values
Text: ="Static Text"
X: =50
Visible: =true

# Control references
Text: =TextInput1.Text
Visible: =Toggle1.Value

# Parent references (for controls in containers/galleries)
Width: =Parent.Width - 20
Height: =Parent.TemplateHeight    # In gallery templates

# Functions
OnSelect: =Navigate(NextScreen, ScreenTransition.Slide)
Text: =Concatenate("Hello ", User().FullName)

# Conditional logic
Visible: =If(Toggle1.Value, true, false)
Fill: =If(Button1.Pressed, RGBA(255,0,0,1), RGBA(0,255,0,1))

# Data operations
Items: =Filter(DataSource, Status = "Active")
Text: =LookUp(Users, ID = 123).Name
```
Z-Index和Control order：
—`Children`数组中的控件按z-index排序
-数组中的第一个控件=底层（z-index 1）
-数组中的最后一个控件=顶层（最高z-index）
—所有控件使用从1开始升序排列

命名约定

###实体名称：
-屏幕名称：描述性和唯一的
-控件名称：TypeName + Number（例如，`Button1`,`Label2`）
—组件名称：PascalCase

属性名称：
-标准属性：使用模式的精确大小写
—自定义属性：推荐使用PascalCase

最佳实践

# # # 1。组织结构:
-保持屏幕逻辑有序
-使用`Group`属性分组相关控件
—为所有实体使用有意义的名称

# # # 2。公式书写:
-保持公式可读性和格式良好
-尽可能在复杂公式中使用注释
避免过于复杂的嵌套表达式# # # 3。组件设计:
设计可重用的组件
—为自定义属性提供清晰的描述
-使用合适的属性类型（Input/Output）

# # # 4。数据源管理：
-对数据源使用描述性名称
-文件连接要求
—保持数据源配置最小化

验证规则

所需属性：
-所有控件必须有一个`Control`属性
组件定义必须是`DefinitionType`—数据源必须为“`Type`”

命名模式：
—实体名称：最少1个字符，由字母数字组成
—控制类型id：遵循模式`^([A-Z][a-zA-Z0-9]*/)?[A-Z][a-zA-Z0-9]*(@\d+\.\d+\.\d+)?$`—代码组件名称：遵循模式`^([a-z][a-z0-9]{1,7})_([a-zA-Z0-9]\.)+[a-zA-Z0-9]+$`##常见问题和解决方案

# # # 1。无效控制类型：
—确保控件类型拼写正确
-检查套管是否合适
—验证模式中是否支持控制类型# # # 2。公式错误:
—所有公式必须以`=`开头
-使用正确的Power Fx语法
-检查正确的属性引用

# # # 3。结构验证:
-保持适当的YAML缩进
-确保所需的属性存在
—完全遵循模式结构

# # # 4。自定义组件问题：
-验证`ComponentName`是否匹配定义
-确保正确定义自定义属性
—检查属性类型是否合适
-如果使用外部组件，验证组件库引用

# # # 5。性能注意事项:
避免在YAML中使用深度嵌套的公式
-使用高效的数据源查询
-考虑大型数据集的可委托公式
-在频繁更新的属性中最小化复杂的计算

##高级主题

# # # 1。组件库集成：```yaml
ComponentDefinitions:
  MyLibraryComponent:
    DefinitionType: CanvasComponent
    AllowCustomization: true
    ComponentLibraryUniqueName: "pub_MyComponentLibrary"
    # Component definition details
```
# # # 2。响应式设计注意事项：
-使用`Parent.Width`和`Parent.Height`响应大小
-考虑基于容器的复杂ui布局
-使用动态定位和尺寸的公式

# # # 3。画廊模板:```yaml
MyGallery:
  Control: Gallery
  Properties:
    Items: =DataSource
    TemplateSize: =100
  Children:
    - GalleryTemplate:  # Template for each gallery item
        Children:
          - TitleLabel:
              Control: Label
              Properties:
                Text: =ThisItem.Title
                Width: =Parent.TemplateWidth - 20
```
# # # 4。表单控件和数据卡：```yaml
MyForm:
  Control: Form
  Properties:
    DataSource: =DataSource
    DefaultMode: =FormMode.New
  Children:
    - DataCard1:
        Control: DataCard
        Properties:
          DataField: ="Title"
        Children:
          - DataCardValue1:
              Control: TextInput
              Properties:
                Default: =Parent.Default
```
# # # 5。公式中的错误处理：```yaml
Properties:
  Text: =IfError(LookUp(DataSource, ID = 123).Name, "Not Found")
  Visible: =!IsError(DataSource)
  OnSelect: =IfError(
    Navigate(DetailScreen, ScreenTransition.Cover),
    Notify("Navigation failed", NotificationType.Error)
  )
```
电源应用程序源代码管理

访问源代码文件：
Power Apps的YAML文件可以通过以下几种方法获得：

1. **电源平台命令行**：   ```powershell
   # List canvas apps in environment
   pac canvas list

   # Download and extract YAML files
   pac canvas download --name "MyApp" --extract-to-directory "C:\path\to\destination"
   ```
2. **手动从。msapp中提取**：   ```powershell
   # Extract .msapp file using PowerShell
   Expand-Archive -Path "C:\path\to\yourFile.msapp" -DestinationPath "C:\path\to\destination"
   ```
3. **Dataverse Git Integration**：直接访问源文件而不需要。msapp文件

.msapp的文件结构：
—`\src\App.pa.yaml`—表示App主配置
-`\src\[ScreenName].pa.yaml`-每个屏幕一个文件
-`\src\Component\[ComponentName].pa.yaml`-组件定义

* * * *重要提示:
—只有`\src`文件夹下的文件才用于源代码控制
-。pa.yaml文件是**只读的**，仅用于审查目的
-不支持外部编辑、合并和冲突解决
- JSON文件。对于源代码控制来说，Msapp并不稳定

模式版本演变：
1. **实验格式** （* *）fx.yaml)：不再开发
2. **早期预览**：临时格式，不再使用
3. **源代码**(*。pa.yaml)：具有版本控制支持的当前活动格式

功率Fx公式参考

###公式类别：

#### **函数**：接受参数、执行操作、返回值```yaml
Properties:
  Text: =Concatenate("Hello ", User().FullName)
  X: =Sum(10, 20, 30)
  Items: =Filter(DataSource, Status = "Active")
```
#### **Signals**：返回环境信息（无参数）```yaml
Properties:
  Text: =Location.Latitude & ", " & Location.Longitude
  Visible: =Connection.Connected
  Color: =If(Acceleration.X > 5, Color.Red, Color.Blue)
```
#### **枚举**：预定义的常量值```yaml
Properties:
  Fill: =Color.Blue
  Transition: =ScreenTransition.Fade
  Align: =Align.Center
```
#### **Named Operators**：访问容器信息```yaml
Properties:
  Text: =ThisItem.Title        # In galleries
  Width: =Parent.Width - 20    # In containers
  Height: =Self.Height / 2     # Self-reference
```
YAML的基本功率Fx功能：

#### **导航和应用控制**：```yaml
OnSelect: =Navigate(NextScreen, ScreenTransition.Cover)
OnSelect: =Back()
OnSelect: =Exit()
OnSelect: =Launch("https://example.com")
```
#### **数据操作**：```yaml
Items: =Filter(DataSource, Category = "Active")
Text: =LookUp(Users, ID = 123).Name
OnSelect: =Patch(DataSource, ThisItem, {Status: "Complete"})
OnSelect: =Collect(LocalCollection, {Name: TextInput1.Text})
```
#### **条件逻辑**：```yaml
Visible: =If(Toggle1.Value, true, false)
Text: =Switch(Status, "New", "🆕", "Complete", "✅", "❓")
Fill: =If(Value < 0, Color.Red, Color.Green)
```
#### **文本操作**：```yaml
Text: =Concatenate("Hello ", User().FullName)
Text: =Upper(TextInput1.Text)
Text: =Substitute(Label1.Text, "old", "new")
Text: =Left(Title, 10) & "..."
```
#### **数学运算**：```yaml
Text: =Sum(Sales[Amount])
Text: =Average(Ratings[Score])
Text: =Round(Calculation, 2)
Text: =Max(Values[Number])
```
#### **日期和时间功能**：```yaml
Text: =Text(Now(), "mm/dd/yyyy")
Text: =DateDiff(StartDate, EndDate, Days)
Text: =Text(Today(), "dddd, mmmm dd, yyyy")
Visible: =IsToday(DueDate)
```
公式语法指南：

#### **基本语法规则**：
—所有公式以`=`开头
-没有前面的`+`或`=`符号（不像Excel）
—文本字符串的双引号：`="Hello World"`—属性引用：`ControlName.PropertyName`-在YAML上下文中不支持注释

#### **公式元素**：```yaml
# Literal values
Text: ="Static Text"
X: =42
Visible: =true

# Control property references
Text: =TextInput1.Text
Visible: =Checkbox1.Value

# Function calls
Text: =Upper(TextInput1.Text)
Items: =Sort(DataSource, Title)

# Complex expressions
Text: =If(IsBlank(TextInput1.Text), "Enter text", Upper(TextInput1.Text))
```
#### **行为与属性公式**：```yaml
# Property formulas (calculate values)
Properties:
  Text: =Concatenate("Hello ", User().FullName)
  Visible: =Toggle1.Value

# Behavior formulas (perform actions - use semicolon for multiple actions)
Properties:
  OnSelect: =Set(MyVar, true); Navigate(NextScreen); Notify("Done!")
```
高级公式模式：

#### **使用集合**：```yaml
Properties:
  Items: =Filter(MyCollection, Status = "Active")
  OnSelect: =ClearCollect(MyCollection, DataSource)
  OnSelect: =Collect(MyCollection, {Name: "New Item", Status: "Active"})
```
#### **错误处理**：```yaml
Properties:
  Text: =IfError(Value(TextInput1.Text), 0)
  OnSelect: =IfError(
    Patch(DataSource, ThisItem, {Field: Value}),
    Notify("Error updating record", NotificationType.Error)
  )
```
#### **动态属性设置**：```yaml
Properties:
  Fill: =ColorValue("#" & HexInput.Text)
  Height: =Parent.Height * (Slider1.Value / 100)
  X: =If(Alignment = "Center", (Parent.Width - Self.Width) / 2, 0)
```
##使用公式最佳实践

###公式组织：
-将复杂的公式分解成更小、更易读的部分
—使用变量存储中间计算
-使用描述性控件名称注释复杂逻辑
—将相关计算分组

性能优化：
-在处理大型数据集时使用委托友好函数
-避免在频繁更新的属性中嵌套函数调用
-使用集合进行复杂的数据转换
最小化对外部数据源的调用

Power Fx数据类型和操作

数据类型类别：

#### **基本类型**：
- **布尔值**:`=true`，`=false`- **编号**:`=123`，`=45.67`- **文本**:`="Hello World"`- **日期：`=Date(2024, 12, 25)`- **时间：`=Time(14, 30, 0)`—**日期时间**:`=Now()`#### **复杂类型**：
- **颜色：`=Color.Red`，`=RGBA(255, 128, 0, 1)`- **记录：`={Name: "John", Age: 30}`- **表**:`=Table({Name: "John"}, {Name: "Jane"})`- **GUID**:`=GUID()`#### **类型转换**：```yaml
Properties:
  Text: =Text(123.45, "#,##0.00")        # Number to text
  Text: =Value("123.45")                 # Text to number
  Text: =DateValue("12/25/2024")         # Text to date
  Visible: =Boolean("true")              # Text to boolean
```
#### **类型检查**：```yaml
Properties:
  Visible: =Not(IsBlank(OptionalField))
  Visible: =Not(IsError(Value(TextInput1.Text)))
  Visible: =IsNumeric(TextInput1.Text)
```
表操作：

#### **创建表**：```yaml
Properties:
  Items: =Table(
    {Name: "Product A", Price: 10.99},
    {Name: "Product B", Price: 15.99}
  )
  Items: =["Option 1", "Option 2", "Option 3"]  # Single-column table
```
#### **过滤和排序**：```yaml
Properties:
  Items: =Filter(Products, Price > 10)
  Items: =Sort(Products, Name, Ascending)
  Items: =SortByColumns(Products, "Price", Descending, "Name", Ascending)
```
#### **数据转换**：```yaml
Properties:
  Items: =AddColumns(Products, "Total", Price * Quantity)
  Items: =RenameColumns(Products, "Price", "Cost")
  Items: =ShowColumns(Products, "Name", "Price")
  Items: =DropColumns(Products, "InternalID")
```
# # # # * *聚合* *:```yaml
Properties:
  Text: =Sum(Products, Price)
  Text: =Average(Products, Rating)
  Text: =Max(Products, Price)
  Text: =CountRows(Products)
```
变量和状态管理：

#### **全局变量**：```yaml
Properties:
  OnSelect: =Set(MyGlobalVar, "Hello World")
  Text: =MyGlobalVar
```
#### **上下文变量**：```yaml
Properties:
  OnSelect: =UpdateContext({LocalVar: "Screen Specific"})
  OnSelect: =Navigate(NextScreen, None, {PassedValue: 42})
```
# # # # * *收藏* *:```yaml
Properties:
  OnSelect: =ClearCollect(MyCollection, DataSource)
  OnSelect: =Collect(MyCollection, {Name: "New Item"})
  Items: =MyCollection
```
Power Fx增强连接器和外部数据

连接器集成：```yaml
DataSources:
  SharePointList:
    Type: Table
    Parameters:
      TableLogicalName: "Custom List"

  Office365Users:
    Type: Actions
    ConnectorId: shared_office365users
```
处理外部数据：```yaml
Properties:
  Items: =Filter(SharePointList, Status = "Active")
  OnSelect: =Office365Users.SearchUser({searchTerm: SearchInput.Text})
```
委托注意事项：```yaml
Properties:
  # Delegable operations (executed server-side)
  Items: =Filter(LargeTable, Status = "Active")    # Efficient

  # Non-delegable operations (may download all records)
  Items: =Filter(LargeTable, Len(Description) > 100)  # Warning issued
```
##故障排除和常见模式

常见错误模式：```yaml
# Handle blank values
Properties:
  Text: =If(IsBlank(OptionalText), "Default", OptionalText)

# Handle errors gracefully
Properties:
  Text: =IfError(RiskyOperation(), "Fallback Value")

# Validate input
Properties:
  Visible: =And(
    Not(IsBlank(NameInput.Text)),
    IsNumeric(AgeInput.Text),
    IsMatch(EmailInput.Text, Email)
  )
```
性能优化：```yaml
# Efficient data loading
Properties:
  Items: =Filter(LargeDataSource, Status = "Active")    # Server-side filtering

# Use delegation-friendly operations
Properties:
  Items: =Sort(Filter(DataSource, Active), Name)        # Delegable
  # Avoid: Sort(DataSource, If(Active, Name, ""))       # Not delegable
```
内存管理：```yaml
# Clear unused collections
Properties:
  OnSelect: =Clear(TempCollection)

# Limit data retrieval
Properties:
  Items: =FirstN(Filter(DataSource, Status = "Active"), 50)
```
请记住：本指南提供了Power Apps Canvas Apps YAML结构和Power Fx公式的全面覆盖。始终根据官方模式和Power Apps Studio环境中的测试公式验证您的YAML。