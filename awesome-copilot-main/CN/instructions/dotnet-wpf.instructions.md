---
description: '.NET WPF component and application patterns'
applyTo: '**/*.xaml, **/*.cs'
---
# #总结

这些说明指导GitHub Copilot帮助使用MVVM模式构建高质量、可维护和高性能的WPF应用程序。它包括XAML、数据绑定、UI响应性和。网络的性能。

理想的项目类型

-使用c#和WPF的桌面应用程序
—遵循MVVM （Model-View-ViewModel）设计模式的应用程序
-项目使用。NET 8.0或更高版本
-在XAML中构建的UI组件
-强调性能和响应性的解决方案

# #目标

-生成`INotifyPropertyChanged`和`RelayCommand`样板
-建议清晰地分离ViewModel和View逻辑
-鼓励使用`ObservableCollection<T>`，`ICommand`和适当的绑定
-推荐性能技巧（例如，虚拟化，异步加载）
避免紧耦合的代码隐藏逻辑
-生成可测试的视图模型

提示行为示例###✅好建议
“为登录屏幕生成一个ViewModel，带有用户名和密码的属性，以及一个LoginCommand”
“为ListView编写一个XAML代码片段，使用UI虚拟化并绑定到ObservableCollection”
“在ViewModel中将这个隐藏在代码后面的点击处理程序重构为RelayCommand”
在WPF异步获取数据时增加一个加载旋转器

###❌避免
-在代码隐藏中建议业务逻辑
-使用没有上下文的静态事件处理程序
-生成紧密耦合的XAML没有绑定
-建议使用WinForms或UWP方法

##选择的技术
- c# with。NET 8.0 +
-带有MVVM结构的XAML
-`CommunityToolkit.Mvvm`或自定义`RelayCommand`实现
-Async/await为非阻塞UI
-`ObservableCollection`,`ICommand`,`INotifyPropertyChanged`##要遵循的常见模式
- ViewModel-first绑定
-使用依赖注入。. NET或第三方容器（如Autofac、SimpleInjector）
- XAML命名约定（控件用PascalCase，绑定用camelCase）
避免绑定魔法字符串（使用`nameof`）

示例指令片段副驾驶可以使用```csharp
public class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private string userName;

    [ObservableProperty]
    private string password;

    [RelayCommand]
    private void Login()
    {
        // Add login logic here
    }
}
```

```xml
<StackPanel>
    <TextBox Text="{Binding UserName, UpdateSourceTrigger=PropertyChanged}" />
    <PasswordBox x:Name="PasswordBox" />
    <Button Content="Login" Command="{Binding LoginCommand}" />
</StackPanel>
```
