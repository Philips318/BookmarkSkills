#验证`ObservableValidator``ObservableValidator`用`INotifyDataErrorInfo`扩展`ObservableObject`支持，集成`System.ComponentModel.DataAnnotations`验证属性。

---

##快速入门```csharp
using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;

public sealed partial class RegistrationViewModel : ObservableValidator
{
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required]
    [MinLength(2), MaxLength(100)]
    private string? name;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required, EmailAddress]
    private string? email;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Range(13, 120)]
    private int age;

    [RelayCommand]
    private void Submit()
    {
        ValidateAllProperties();
        if (HasErrors) return;
        // submit...
    }
}
```
`[NotifyDataErrorInfo]`进行生成的setter调用`ValidateProperty(value)`在每次成功设置后运行，因此验证运行为
用户类型。

---

##手动`SetProperty`验证

如果你不用`[ObservableProperty]`，而是手写这个性质，
选择使用`bool validate`参数进行验证：```csharp
[Required, MinLength(2), MaxLength(100)]
public string? Name
{
    get => name;
    set => SetProperty(ref name, value, validate: true);
}
```
---

# #`TrySetProperty`有时，您希望仅在验证成功时设置属性：```csharp
[Required, EmailAddress]
public string? Email
{
    get => email;
    set
    {
        if (TrySetProperty(ref email, value, out IReadOnlyCollection<ValidationResult> errors))
        {
            // value passed validation; success
        }
        else
        {
            // inspect errors
        }
    }
}
```
---

# #`ValidateAllProperties()`强制对具有at的类型中的每个公共属性进行验证
至少有一个`ValidationAttribute`。投稿前致电：```csharp
[RelayCommand(CanExecute = nameof(CanSubmit))]
private void Submit()
{
    ValidateAllProperties();
    if (HasErrors) return;
    submitter.Submit(this);
}

private bool CanSubmit() => !HasErrors;
```
在输入字段上与`[NotifyCanExecuteChangedFor]`配对，再加上a
监听器上的`ErrorsChanged`（或覆盖`OnErrorsChanged`）
与用户类型同步的按钮状态。

---

# #`ValidateProperty(value, propertyName)`手动触发一个属性的验证-在验证时有用
属性`A`取决于属性`B`：```csharp
[Range(20, 80)]
[ObservableProperty]
private int b;

[Range(10, 100)]
[GreaterThan(nameof(B))]
[ObservableProperty]
private int a;

partial void OnBChanged(int value)
{
    // Re-run A's validation since it depends on B.
    ValidateProperty(A, nameof(A));
}
```
---

# #`ClearAllErrors()`重置错误状态-在成功提交后或重置时常见
一种形式:```csharp
[RelayCommand]
private void Reset()
{
    Name = null;
    Email = null;
    Age = 0;
    ClearAllErrors();
}
```
---

自定义验证方法（`[CustomValidation]`）```csharp
[Required, MinLength(3)]
[CustomValidation(typeof(RegistrationViewModel), nameof(ValidateUsername))]
[ObservableProperty]
private string? username;

public static ValidationResult ValidateUsername(string? value, ValidationContext context)
{
    var vm = (RegistrationViewModel)context.ObjectInstance;
    if (vm.userService.IsTaken(value!))
        return new ValidationResult("Username is already taken.");
    return ValidationResult.Success!;
}
```
该方法必须为`static`，并且接受`(value, ValidationContext)`。使用`context.ObjectInstance`返回ViewModel。

---

自定义`ValidationAttribute`对于可重用规则，子类`ValidationAttribute`：```csharp
public sealed class GreaterThanAttribute(string otherPropertyName)
    : ValidationAttribute
{
    public string OtherPropertyName { get; } = otherPropertyName;

    protected override ValidationResult? IsValid(object? value, ValidationContext ctx)
    {
        var instance = ctx.ObjectInstance;
        var other = instance.GetType().GetProperty(OtherPropertyName)?.GetValue(instance);
        if (((IComparable)value!).CompareTo(other) > 0)
            return ValidationResult.Success;
        return new ValidationResult($"Must be greater than {OtherPropertyName}.");
    }
}
```
适用于物业：```csharp
[Range(10, 100)]
[GreaterThan(nameof(B))]
[ObservableProperty]
private int a;
```
---

视图读取错误`ObservableValidator`实现`INotifyDataErrorInfo`。XAML栈渲染`ErrorsChanged`自动当`ValidatesOnNotifyDataErrors=True`（WPF）
或者通过控件模板（WinUI 3， MAUI）。检查代码中的错误：```csharp
foreach (ValidationResult result in vm.GetErrors(nameof(vm.Name)))
{
    Console.WriteLine(result.ErrorMessage);
}

// Across all properties
foreach (ValidationResult result in vm.GetErrors())
{
    Console.WriteLine(result.ErrorMessage);
}

bool any = vm.HasErrors;
```
订阅更改：```csharp
vm.ErrorsChanged += (s, e) =>
{
    Debug.WriteLine($"Errors changed for {e.PropertyName}");
};
```
---

# #提示

—将`ValidateAllProperties()`与`[NotifyCanExecuteChangedFor]`组合
提交按钮实时反映有效性。
-在ViewModel中保留验证规则（或通过自定义属性），而不是
模型中——模型应该是一个普通的DTO。
-用于网络或异步验证（例如，“用户名是否已使用？”）,使用`[CustomValidation]`调用围绕异步查找的同步包装器
(或单独执行异步检查，并通过`AddError(propertyName, ...)`样式的帮助程序（如果您自己编写的话）。
-`ObservableValidator`也不能继承`ObservableRecipient`-
如果需要消息传递，则注入`IMessenger`并直接调用`Send`。