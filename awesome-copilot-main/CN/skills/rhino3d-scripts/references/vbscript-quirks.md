# VBScript的RhinoScript怪癖

在编写`.rvb`/`.vbs`文件时，这些东西对心智模型是c族或Python的人来说并不明显。

##总是以`Option Explicit`开头

如果没有它，输入错误的变量名将静默地创建一个新的`Variant`集为`Empty`。每个`.rvb`文件的开头应该是：```vbscript
Option Explicit
```
##没有块范围`Sub`/`Function`中的所有`Dim`声明都被提升到顶部。在`If`结束后，可以看到`Dim`块中的`If`。循环计数器在循环中存活。`Nothing`vs`Empty`vs`Null`|哨兵|用|测试|---|---|---|
|`Empty`|`IsEmpty(x)`|`Dim`'d但从未分配|
|`Null`|`IsNull(x)`|显式“无值”-取消|时`Rhino.GetObject`返回什么
|`Nothing`|`x Is Nothing`|一个**对象**引用，只指向`Set`变量|

错误的哨兵→无声的错误。

括号改变语义```vbscript
Foo a, b              ' Call a Sub or Function (return value discarded)
Call Foo(a, b)        ' Call a Sub or Function (return value discarded)
x = Foo(a, b)         ' Call a Function and capture the return
Foo(a, b)             ' SYNTAX ERROR for multi-arg subs

Foo(x)                ' Calls Foo passing x BY VALUE, even if Foo declares ByRef
Foo x                 ' Honors Foo's ByRef declaration
```
如果Sub修改了它的参数，而您的更改没有生效-您将参数包装在括号中。

##`ByRef`为默认值

不像大多数语言，VBScript在默认情况下通过引用传递参数。函数可以改变调用者的变量。是明确的:```vbscript
Sub Increment(ByRef n)
    n = n + 1
End Sub
```
数组是基于0的，但有`UBound`，而不是`Length````vbscript
Dim arr(2)            ' Three elements: arr(0), arr(1), arr(2)
For i = 0 To UBound(arr)
    arr(i) = i * 10
Next
```
`Dim arr(n)`创建`n+1`元素。`ReDim Preserve arr(newSize)`调整大小（只调整multidim数组的最后一个维度）。

##`Set`是对象分配所必需的```vbscript
Set fso = CreateObject("Scripting.FileSystemObject")    ' correct
fso = CreateObject("Scripting.FileSystemObject")        ' RUNTIME ERROR
```
任何时候，右边是一个对象（COM对象、RegExp、Dictionary），都必须使用`Set`。

错误处理是手动的```vbscript
On Error Resume Next
Rhino.AddCircle Array(0,0,0), -1
If Err.Number <> 0 Then
    Rhino.Print "Failed: " & Err.Description
    Err.Clear
End If
On Error GoTo 0           ' Restore normal error behavior
```
`On Error Resume Next`抑制** **错误，直到`On Error GoTo 0`。忘记恢复是一个常见的错误。

点是三元素数组

Rhino期望`Array(x, y, z)`。2元素数组（`Array(x, y)`）引发类型不匹配错误。```vbscript
Dim pt
pt = Array(1.0, 2.0, 3.0)
Rhino.AddPoint pt
```
字符串连接使用`&`，而不是`+`字符串上的`+`只有在两边都是字符串的情况下才有效。如果一侧是数字，`+`执行数字加法并抛出类型不匹配。总是使用`&`：```vbscript
Rhino.Print "Count: " & n
```
需要一个`Variant````vbscript
Dim item
For Each item In someCollection
    ' ...
Next
```
循环变量必须为`Variant`。你不能`Dim item As Long`（VBScript没有键入`Dim`）。