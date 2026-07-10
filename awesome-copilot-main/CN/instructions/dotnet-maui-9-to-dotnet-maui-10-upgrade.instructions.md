---
description: 'Instructions for upgrading .NET MAUI applications from version 9 to version 10, including breaking changes, deprecated APIs, and migration strategies for ListView to CollectionView.'
applyTo: '**/*.csproj, **/*.cs, **/*.xaml'
---
#升级从。净毛伊岛9至。净毛伊岛10

本指南可帮助您升级您的。NET MAUI应用程序。NET 9到。通过专注于关键的突破性更改和需要代码更新的过时api。

---

##目录1. (快速启动)(#快速启动)
2. [更新目标框架]（# Update - Target - Framework）
3. [breakingchanges - P0 - Must - Fix]（# breakingchanges - P0 - Must - Fix）
- [MessagingCenter Made Internal]（# MessagingCenter Made - Internal）
- [ListView和TableView Deprecated]（# ListView和TableView - Deprecated）
4. [Deprecated api (P1 - Fix Soon)]（# Deprecated -api - P1——Fix - Soon）
-[动画方法]（#1-animation-methods）
- [DisplayAlert and DisplayActionSheet]（#2- DisplayAlert -and DisplayActionSheet）
——(页面。IsBusy) (# 3-pageisbusy)
- [MediaPicker api]（#4-mediapicker-apis）
5. [建议修改（P2）]（# Recommended - Changes - P2）
6. [大容量迁移工具]（# Bulk - Migration - Tools）
7. [测试升级]（# Testing - Your - Upgrade）
8. (故障排除)(#故障排除)

---

##快速入门

**五步升级流程：**1. **更新TargetFramework**为`net10.0`2. * *更新CommunityToolkit。毛伊岛**到12.3.0+（如果你使用它）-必需
3. ** - MessagingCenter （P0）
4. **迁移ListView/TableView到CollectionView** （P0 - CRITICAL）
5. **修复已弃用的api ** -动画方法，DisplayAlert, IsBusy, MediaPicker （P1）

>⚠️**重大变更**：
> -社区工具包。Maui **必须**是12.3.0或更高版本
> - ListView和TableView现在已经过时了（最重要的迁移工作）

---

更新目标框架

单平台```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
  </PropertyGroup>
</Project>
```
# # #多平台```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFrameworks>net10.0-android;net10.0-ios;net10.0-maccatalyst;net10.0-windows10.0.19041.0</TargetFrameworks>
  </PropertyGroup>
</Project>
```
###可选：Linux兼容性（GitHub Copilot， WSL等）

>💡**对于Linux开发**：如果您正在Linux上构建（例如，GitHub Codespaces， WSL或使用GitHub Copilot），您可以通过有条件地排除iOS/MacCatalyst目标来使您的项目在Linux上编译：```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <!-- Start with Android (always supported) -->
    <TargetFrameworks>net10.0-android</TargetFrameworks>
    
    <!-- Add iOS/Mac Catalyst only when NOT on Linux -->
    <TargetFrameworks Condition="!$([MSBuild]::IsOSPlatform('linux'))">$(TargetFrameworks);net10.0-ios;net10.0-maccatalyst</TargetFrameworks>
    
    <!-- Add Windows only when on Windows -->
    <TargetFrameworks Condition="$([MSBuild]::IsOSPlatform('windows'))">$(TargetFrameworks);net10.0-windows10.0.19041.0</TargetFrameworks>
  </PropertyGroup>
</Project>
```
* *好处:* *
-✅在Linux上编译成功（不需要iOS/Mac工具）
-✅与GitHub Codespaces和Copilot一起工作
-✅根据构建操作系统自动包含正确的目标
-✅操作系统切换时无需更改

参考:* * * * (dotnet/maui# 32186) (https://github.com/dotnet/maui/pull/32186)

更新必需的NuGet包

>⚠️**CRITICAL**：如果您使用了CommunityToolkit。毛伊岛，您**必须**更新到12.3.0或更高版本。早期版本不兼容。并会导致编译错误。```bash
# Update CommunityToolkit.Maui (if you use it)
dotnet add package CommunityToolkit.Maui --version 12.3.0

# Update other common packages to .NET 10 compatible versions
dotnet add package Microsoft.Maui.Controls --version 10.0.0
```
**检查所有NuGet包：**```bash
# List all packages and check for updates
dotnet list package --outdated

# Update all packages to latest compatible versions
dotnet list package --outdated | grep ">" | cut -d '>' -f 1 | xargs -I {} dotnet add package {}
```
---

突破性的变化（P0 -必须修复）

MessagingCenter Made Internal

**状态：**🚨** break ** -`MessagingCenter`现在是`internal`，无法访问。

**错误你会看到：**```
error CS0122: 'MessagingCenter' is inaccessible due to its protection level
```
* *迁移要求:* *

####步骤1：安装CommunityToolkit。Mvvm```bash
dotnet add package CommunityToolkit.Mvvm --version 8.3.0
```
####步骤2：定义消息类```csharp
// OLD: No message class needed
MessagingCenter.Send(this, "UserLoggedIn", userData);

// NEW: Create a message class
public class UserLoggedInMessage
{
    public UserData Data { get; set; }
    
    public UserLoggedInMessage(UserData data)
    {
        Data = data;
    }
}
```
####步骤3：更新发送呼叫```csharp
// ❌ OLD (Broken in .NET 10)
using Microsoft.Maui.Controls;

MessagingCenter.Send(this, "UserLoggedIn", userData);
MessagingCenter.Send<App, string>(this, "StatusChanged", "Active");

// ✅ NEW (Required)
using CommunityToolkit.Mvvm.Messaging;

WeakReferenceMessenger.Default.Send(new UserLoggedInMessage(userData));
WeakReferenceMessenger.Default.Send(new StatusChangedMessage("Active"));
```
####步骤4：更新订阅调用```csharp
// ❌ OLD (Broken in .NET 10)
MessagingCenter.Subscribe<App, UserData>(this, "UserLoggedIn", (sender, data) =>
{
    // Handle message
    CurrentUser = data;
});

// ✅ NEW (Required)
WeakReferenceMessenger.Default.Register<UserLoggedInMessage>(this, (recipient, message) =>
{
    // Handle message
    CurrentUser = message.Data;
});
```
####⚠️重要的行为区别：重复订阅

**WeakReferenceMessenger**抛出`InvalidOperationException`，如果你试图在同一接收者上多次注册相同的消息类型（MessagingCenter允许此）：```csharp
// ❌ This THROWS InvalidOperationException in WeakReferenceMessenger
WeakReferenceMessenger.Default.Register<UserLoggedInMessage>(this, (r, m) => Handler1(m));
WeakReferenceMessenger.Default.Register<UserLoggedInMessage>(this, (r, m) => Handler2(m)); // ❌ THROWS!

// ✅ Solution 1: Unregister before re-registering
WeakReferenceMessenger.Default.Unregister<UserLoggedInMessage>(this);
WeakReferenceMessenger.Default.Register<UserLoggedInMessage>(this, (r, m) => Handler1(m));

// ✅ Solution 2: Handle multiple actions in one registration
WeakReferenceMessenger.Default.Register<UserLoggedInMessage>(this, (r, m) => 
{
    Handler1(m);
    Handler2(m);
});
```
如果你的代码在多个地方订阅了相同的消息（例如，在一个页面构造函数和`OnAppearing`中），你会得到一个运行时崩溃。

####步骤5：完成后取消注册```csharp
// ❌ OLD
MessagingCenter.Unsubscribe<App, UserData>(this, "UserLoggedIn");

// ✅ NEW (CRITICAL - prevents memory leaks)
WeakReferenceMessenger.Default.Unregister<UserLoggedInMessage>(this);

// Or unregister all messages for this recipient
WeakReferenceMessenger.Default.UnregisterAll(this);
```
####完整Before/After示例

(之前* *。网9):* *```csharp
// Sender
public class LoginViewModel
{
    public async Task LoginAsync()
    {
        var user = await AuthService.LoginAsync(username, password);
        MessagingCenter.Send(this, "UserLoggedIn", user);
    }
}

// Receiver
public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
        
        MessagingCenter.Subscribe<LoginViewModel, User>(this, "UserLoggedIn", (sender, user) =>
        {
            WelcomeLabel.Text = $"Welcome, {user.Name}!";
        });
    }
    
    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        MessagingCenter.Unsubscribe<LoginViewModel, User>(this, "UserLoggedIn");
    }
}
```
(后* *。净10):* *```csharp
// 1. Define message
public class UserLoggedInMessage
{
    public User User { get; }
    
    public UserLoggedInMessage(User user)
    {
        User = user;
    }
}

// 2. Sender
public class LoginViewModel
{
    public async Task LoginAsync()
    {
        var user = await AuthService.LoginAsync(username, password);
        WeakReferenceMessenger.Default.Send(new UserLoggedInMessage(user));
    }
}

// 3. Receiver
public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
        
        WeakReferenceMessenger.Default.Register<UserLoggedInMessage>(this, (recipient, message) =>
        {
            WelcomeLabel.Text = $"Welcome, {message.User.Name}!";
        });
    }
    
    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        WeakReferenceMessenger.Default.UnregisterAll(this);
    }
}
```
* *主要差异:* *
-✅类型安全的消息类
-✅没有魔法字符串
-✅更好的智能感知支持
-✅更容易重构
-⚠️**一定要记得注销！**

---

ListView和TableView已弃用

**状态：**🚨**DEPRECATED (P0)** -`ListView`，`TableView`和所有Cell类型现在已经过时。迁移到`CollectionView`。

**警告你会看到：**```
warning CS0618: 'ListView' is obsolete: 'ListView is deprecated. Please use CollectionView instead.'
warning CS0618: 'TableView' is obsolete: 'Please use CollectionView instead.'
warning CS0618: 'TextCell' is obsolete: 'The controls which use TextCell (ListView and TableView) are obsolete. Please use CollectionView instead.'
```
* *过时类型:* *
-`ListView`→`CollectionView`-`TableView`→`CollectionView`（设置页面，考虑垂直StackLayout与bindabllayout）
-`TextCell`→自定义带标签的数据模板
-`ImageCell`→自定义数据模板与图像+标签(s)
-`EntryCell`→带入口的自定义数据模板
-`SwitchCell`→自定义DataTemplate with Switch
-`ViewCell`→数据模板

**影响：**这是一个**MAJOR**突破性的变化。ListView和TableView是MAUI应用中最常用的控件。

####为什么这需要时间

转换ListView/TableView到CollectionView不是一个简单的查找-替换：1. **不同的事件模型** -`ItemSelected`→`SelectionChanged`，参数不同
2. **分组不同** - GroupDisplayBinding不再存在
3. **上下文操作** -必须转换为SwipeView
4. **项目大小** -`HasUnevenRows`处理不同
5. **平台特定代码** -iOS/AndroidListView平台配置需要删除
6. **需要测试** - CollectionView虚拟化不同，可能会影响性能

####迁移策略

**步骤1：盘点您的ListViews**```bash
# Find all ListView/TableView usages
grep -r "ListView\|TableView" --include="*.xaml" --include="*.cs" .
```
**步骤2：基本ListView→CollectionView**

* *前(视图):* *```xaml
<ListView ItemsSource="{Binding Items}"
          ItemSelected="OnItemSelected"
          HasUnevenRows="True">
    <ListView.ItemTemplate>
        <DataTemplate>
            <TextCell Text="{Binding Title}"
                     Detail="{Binding Description}" />
        </DataTemplate>
    </ListView.ItemTemplate>
</ListView>
```
(CollectionView):后* * * *```xaml
<CollectionView ItemsSource="{Binding Items}"
                SelectionMode="Single"
                SelectionChanged="OnSelectionChanged">
    <CollectionView.ItemTemplate>
        <DataTemplate>
            <VerticalStackLayout Padding="10">
                <Label Text="{Binding Title}" 
                       FontAttributes="Bold" />
                <Label Text="{Binding Description}"
                       FontSize="12"
                       TextColor="{StaticResource Gray600}" />
            </VerticalStackLayout>
        </DataTemplate>
    </CollectionView.ItemTemplate>
</CollectionView>
```
>⚠️**注：** CollectionView默认为`SelectionMode="None"`（禁用选择）。必须显式设置`SelectionMode="Single"`或`SelectionMode="Multiple"`以启用选择。

* *后台代码的变化:* *```csharp
// ❌ OLD (ListView)
void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
{
    if (e.SelectedItem == null)
        return;
        
    var item = (MyItem)e.SelectedItem;
    // Handle selection
    
    // Deselect
    ((ListView)sender).SelectedItem = null;
}

// ✅ NEW (CollectionView)
void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
{
    if (e.CurrentSelection.Count == 0)
        return;
        
    var item = (MyItem)e.CurrentSelection.FirstOrDefault();
    // Handle selection
    
    // Deselect (optional)
    ((CollectionView)sender).SelectedItem = null;
}
```
**步骤3：分组ListView→分组CollectionView**

**之前（分组ListView）：**```xaml
<ListView ItemsSource="{Binding GroupedItems}"
          IsGroupingEnabled="True"
          GroupDisplayBinding="{Binding Key}">
    <ListView.ItemTemplate>
        <DataTemplate>
            <TextCell Text="{Binding Name}" />
        </DataTemplate>
    </ListView.ItemTemplate>
</ListView>
```
**后（分组CollectionView）：**```xaml
<CollectionView ItemsSource="{Binding GroupedItems}"
                IsGrouped="true">
    <CollectionView.GroupHeaderTemplate>
        <DataTemplate>
            <Label Text="{Binding Key}"
                   FontAttributes="Bold"
                   BackgroundColor="{StaticResource Gray100}"
                   Padding="10,5" />
        </DataTemplate>
    </CollectionView.GroupHeaderTemplate>
    
    <CollectionView.ItemTemplate>
        <DataTemplate>
            <VerticalStackLayout Padding="20,10">
                <Label Text="{Binding Name}" />
            </VerticalStackLayout>
        </DataTemplate>
    </CollectionView.ItemTemplate>
</CollectionView>
```
**步骤4：上下文动作→滑动视图**

>⚠️**平台说明：** SwipeView需要触摸输入。在Windows桌面，它只适用于触摸屏，不适用mouse/trackpad.考虑为桌面场景提供替代UI（例如，按钮，右键菜单）。

**之前（ListView with ContextActions）：**```xaml
<ListView.ItemTemplate>
    <DataTemplate>
        <ViewCell>
            <ViewCell.ContextActions>
                <MenuItem Text="Delete" 
                         IsDestructive="True"
                         Command="{Binding Source={RelativeSource AncestorType={x:Type local:MyPage}}, Path=DeleteCommand}"
                         CommandParameter="{Binding .}" />
            </ViewCell.ContextActions>
            
            <Label Text="{Binding Title}" Padding="10" />
        </ViewCell>
    </DataTemplate>
</ListView.ItemTemplate>
```
**后（CollectionView与SwipeView）：**```xaml
<CollectionView.ItemTemplate>
    <DataTemplate>
        <SwipeView>
            <SwipeView.RightItems>
                <SwipeItems>
                    <SwipeItem Text="Delete"
                              BackgroundColor="Red"
                              Command="{Binding Source={RelativeSource AncestorType={x:Type local:MyPage}}, Path=DeleteCommand}"
                              CommandParameter="{Binding .}" />
                </SwipeItems>
            </SwipeView.RightItems>
            
            <VerticalStackLayout Padding="10">
                <Label Text="{Binding Title}" />
            </VerticalStackLayout>
        </SwipeView>
    </DataTemplate>
</CollectionView.ItemTemplate>
```
**步骤5:TableView for Settings→Alternative Approaches**

TableView通常用于设置页面。以下是现代的替代方案：

**选项1：具有分组数据的CollectionView **```xaml
<CollectionView ItemsSource="{Binding SettingGroups}"
                IsGrouped="true"
                SelectionMode="None">
    <CollectionView.GroupHeaderTemplate>
        <DataTemplate>
            <Label Text="{Binding Title}" 
                   FontAttributes="Bold"
                   Margin="10,15,10,5" />
        </DataTemplate>
    </CollectionView.GroupHeaderTemplate>
    
    <CollectionView.ItemTemplate>
        <DataTemplate>
            <Grid Padding="15,10" ColumnDefinitions="*,Auto">
                <Label Text="{Binding Title}" 
                       VerticalOptions="Center" />
                <Switch Grid.Column="1" 
                        IsToggled="{Binding IsEnabled}"
                        IsVisible="{Binding ShowSwitch}" />
            </Grid>
        </DataTemplate>
    </CollectionView.ItemTemplate>
</CollectionView>
```
**选项2：垂直StackLayout（小设置列表）**```xaml
<ScrollView>
    <VerticalStackLayout BindableLayout.ItemsSource="{Binding Settings}"
                        Spacing="10"
                        Padding="15">
        <BindableLayout.ItemTemplate>
            <DataTemplate>
                <Border StrokeThickness="0"
                       BackgroundColor="{StaticResource Gray100}"
                       Padding="15,10">
                    <Grid ColumnDefinitions="*,Auto">
                        <Label Text="{Binding Title}" 
                              VerticalOptions="Center" />
                        <Switch Grid.Column="1" 
                               IsToggled="{Binding IsEnabled}" />
                    </Grid>
                </Border>
            </DataTemplate>
        </BindableLayout.ItemTemplate>
    </VerticalStackLayout>
</ScrollView>
```
**步骤6：删除平台特定的ListView代码**

如果你使用了特定于平台的ListView特性，请删除它们：```csharp
// ❌ OLD - Remove these using statements (NOW OBSOLETE IN .NET 10)
using Microsoft.Maui.Controls.PlatformConfiguration;
using Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific;
using Microsoft.Maui.Controls.PlatformConfiguration.AndroidSpecific;

// ❌ OLD - Remove ListView platform configurations (NOW OBSOLETE IN .NET 10)
myListView.On<iOS>().SetSeparatorStyle(SeparatorStyle.FullWidth);
myListView.On<Android>().IsFastScrollEnabled();

// ❌ OLD - Remove Cell platform configurations (NOW OBSOLETE IN .NET 10)
viewCell.On<iOS>().SetDefaultBackgroundColor(Colors.White);
viewCell.On<Android>().SetIsContextActionsLegacyModeEnabled(false);
```
**迁移：** CollectionView没有相同的平台特定配置。如果你需要特定于平台的样式：```csharp
// ✅ NEW - Use conditional compilation
#if IOS
var backgroundColor = Colors.White;
#elif ANDROID
var backgroundColor = Colors.Transparent;
#endif

var grid = new Grid
{
    BackgroundColor = backgroundColor,
    // ... rest of cell content
};
```
或者在XAML中：```xaml
<CollectionView.ItemTemplate>
    <DataTemplate>
        <Grid>
            <Grid.BackgroundColor>
                <OnPlatform x:TypeArguments="Color">
                    <On Platform="iOS" Value="White" />
                    <On Platform="Android" Value="Transparent" />
                </OnPlatform>
            </Grid.BackgroundColor>
            <!-- Cell content -->
        </Grid>
    </DataTemplate>
</CollectionView.ItemTemplate>
```
####常见模式和陷阱

* * 1。空视图* *```xaml
<!-- CollectionView has built-in EmptyView support -->
<CollectionView ItemsSource="{Binding Items}">
    <CollectionView.EmptyView>
        <ContentView>
            <VerticalStackLayout Padding="50" VerticalOptions="Center">
                <Label Text="No items found" 
                       HorizontalTextAlignment="Center" />
            </VerticalStackLayout>
        </ContentView>
    </CollectionView.EmptyView>
    <!-- ... -->
</CollectionView>
```
* * 2。拉刷新**```xaml
<RefreshView IsRefreshing="{Binding IsRefreshing}"
             Command="{Binding RefreshCommand}">
    <CollectionView ItemsSource="{Binding Items}">
        <!-- ... -->
    </CollectionView>
</RefreshView>
```
* * 3。项目距* *```xaml
<!-- Use ItemsLayout for spacing -->
<CollectionView ItemsSource="{Binding Items}">
    <CollectionView.ItemsLayout>
        <LinearItemsLayout Orientation="Vertical" 
                          ItemSpacing="10" />
    </CollectionView.ItemsLayout>
    <!-- ... -->
</CollectionView>
```
* * 4。页眉和页脚**```xaml
<CollectionView ItemsSource="{Binding Items}">
    <CollectionView.Header>
        <Label Text="My List" 
               FontSize="24" 
               Padding="10" />
    </CollectionView.Header>
    
    <CollectionView.Footer>
        <Label Text="End of list" 
               Padding="10" 
               HorizontalTextAlignment="Center" />
    </CollectionView.Footer>
    
    <!-- ItemTemplate -->
</CollectionView>
```
* * 5。加载更多/无限滚动**```xaml
<CollectionView ItemsSource="{Binding Items}"
                RemainingItemsThreshold="5"
                RemainingItemsThresholdReachedCommand="{Binding LoadMoreCommand}">
    <!-- ItemTemplate -->
</CollectionView>
```
* * 6。项目尺寸优化**

CollectionView使用`ItemSizingStrategy`来控制项目度量：```xaml
<!-- Default: Each item measured individually (like HasUnevenRows="True") -->
<CollectionView ItemSizingStrategy="MeasureAllItems">
    <!-- ... -->
</CollectionView>

<!-- Performance: Only first item measured, rest use same height -->
<CollectionView ItemSizingStrategy="MeasureFirstItem">
    <!-- Use this when all items have similar heights -->
</CollectionView>
```
**性能提示：**如果您的列表项具有一致的高度，请使用`ItemSizingStrategy="MeasureFirstItem"`以获得较大列表的更好性能。

# # # #。NET 10处理程序更改（iOS/MacCatalyst）

>ℹ️**。NET 10在iOS和Mac Catalyst上默认使用了新的优化的CollectionView和CarouselView处理程序**，提供了改进的性能和稳定性。

**如果您之前选择加入新的处理程序。. NET 9**，你现在应该**删除**这段代码：```csharp
// ❌ REMOVE THIS in .NET 10 (these handlers are now default)
#if IOS || MACCATALYST
builder.ConfigureMauiHandlers(handlers =>
{
    handlers.AddHandler<CollectionView, CollectionViewHandler2>();
    handlers.AddHandler<CarouselView, CarouselViewHandler2>();
});
#endif
```
中自动使用优化的处理程序。NET 10 -不需要配置！

**只有当你遇到问题时，你才可以恢复到遗留处理程序：```csharp
// In MauiProgram.cs - only if needed
#if IOS || MACCATALYST
builder.ConfigureMauiHandlers(handlers =>
{
    handlers.AddHandler<Microsoft.Maui.Controls.CollectionView, 
                        Microsoft.Maui.Controls.Handlers.Items.CollectionViewHandler>();
});
#endif
```
然而，微软建议使用新的默认处理程序以获得最佳效果。

####测试清单

迁移之后，测试这些场景：

-[] **物品选择**工作正常
-[] **分组列表**显示适当的标题
-[] **滑动动作**（如果使用）在iOS和Android上工作
-[] **当列表为空时，显示空视图**
-[] **拉刷新**工作（如果使用）
-[] **滚动性能**是可以接受的（特别是对于大列表）
-[] **项目大小**是正确的（CollectionView默认自动大小）
-[] **正确选择视觉状态**shows/hides-[] **数据绑定**正确更新列表
-[] **导航**从列表项工作

####迁移复杂性因素ListView到CollectionView的迁移是复杂的，因为：
-每个ListView可能有唯一的行为
-特定平台的代码需要更新
-需要广泛的测试
-上下文操作需要SwipeView转换
-分组列表需要模板更新
-可能需要更改ViewModel

####快速参考：ListView vs CollectionView

|特性| ListView | CollectionView ||---------|----------|----------------|
| **选择事件** |`ItemSelected`|`SelectionChanged`|
| **选择参数** |`SelectedItemChangedEventArgs`|`SelectionChangedEventArgs`|
| **选择** |`e.SelectedItem`|`e.CurrentSelection.FirstOrDefault()`|
| **上下文菜单** |`ContextActions`|`SwipeView`|
| **分组** |`IsGroupingEnabled="True"`|`IsGrouped="true"`|
| **组头** |`GroupDisplayBinding`|`GroupHeaderTemplate`|
| **偶数行** |`HasUnevenRows="False"`|自动大小（默认）|
| **空状态** |手动|`EmptyView`属性|
| **Cells** | TextCell， ImageCell等|自定义数据模板|

---

##已弃用的api （P1 -即将修复）

这些api仍然可以使用。NET 10，但显示编译器警告。它们将在未来的版本中被删除。

# # # 1。动画方法

**状态：**⚠️**已弃用** -所有同步动画方法替换为异步版本。

**警告你会看到：**```
warning CS0618: 'ViewExtensions.FadeTo(VisualElement, double, uint, Easing)' is obsolete: 'Please use FadeToAsync instead.'
```
* *迁移表:* *

|旧方法|新方法|示例||-----------|-----------|---------|
|`FadeTo()`|`FadeToAsync()`|`await view.FadeToAsync(0, 500);`|
|`ScaleTo()`|`ScaleToAsync()`|`await view.ScaleToAsync(1.5, 300);`|
|`TranslateTo()`|`TranslateToAsync()`|`await view.TranslateToAsync(100, 100, 250);`|
|`RotateTo()`|`RotateToAsync()`|`await view.RotateToAsync(360, 500);`|
|`RotateXTo()`|`RotateXToAsync()`|`await view.RotateXToAsync(45, 300);`|
|`RotateYTo()`|`RotateYToAsync()`|`await view.RotateYToAsync(45, 300);`|
|`ScaleXTo()`|`ScaleXToAsync()`|`await view.ScaleXToAsync(2.0, 300);`|
|`ScaleYTo()`|`ScaleYToAsync()`|`await view.ScaleYToAsync(2.0, 300);`|
|`RelRotateTo()`|`RelRotateToAsync()`|`await view.RelRotateToAsync(90, 300);`|
|`RelScaleTo()`|`RelScaleToAsync()`|`await view.RelScaleToAsync(0.5, 300);`|
|`LayoutTo()`|`LayoutToAsync()`|参见|下面的特别说明

####迁移示例

* *简单的动画:* *```csharp
// ❌ OLD (Deprecated)
await myButton.FadeTo(0, 500);
await myButton.ScaleTo(1.5, 300);
await myButton.TranslateTo(100, 100, 250);

// ✅ NEW (Required)
await myButton.FadeToAsync(0, 500);
await myButton.ScaleToAsync(1.5, 300);
await myButton.TranslateToAsync(100, 100, 250);
```
* *连续动画:* *```csharp
// ❌ OLD
await image.FadeTo(0, 300);
await image.ScaleTo(0.5, 300);
await image.FadeTo(1, 300);

// ✅ NEW
await image.FadeToAsync(0, 300);
await image.ScaleToAsync(0.5, 300);
await image.FadeToAsync(1, 300);
```
平行动画:* * * *```csharp
// ❌ OLD
await Task.WhenAll(
    image.FadeTo(0, 300),
    image.ScaleTo(0.5, 300),
    image.RotateTo(360, 300)
);

// ✅ NEW
await Task.WhenAll(
    image.FadeToAsync(0, 300),
    image.ScaleToAsync(0.5, 300),
    image.RotateToAsync(360, 300)
);
```
与取消* *:* *```csharp
// NEW: Async methods support cancellation
CancellationTokenSource cts = new();

try
{
    await view.FadeToAsync(0, 2000);
}
catch (TaskCanceledException)
{
    // Animation was cancelled
}

// Cancel from elsewhere
cts.Cancel();
```
####特殊情况：LayoutTo`LayoutToAsync()`被弃用，并带有一个特殊的消息：“使用翻译来动画布局更改。”```csharp
// ❌ OLD (Deprecated)
await view.LayoutToAsync(new Rect(100, 100, 200, 200), 250);

// ✅ NEW (Use TranslateToAsync instead)
await view.TranslateToAsync(100, 100, 250);

// Or animate Translation properties directly
var animation = new Animation(v => view.TranslationX = v, 0, 100);
animation.Commit(view, "MoveX", length: 250);
```
---

# # # 2。DisplayAlert和DisplayActionSheet

**状态：**⚠️**已弃用** -同步方法被异步版本取代。

**警告你会看到：**```
warning CS0618: 'Page.DisplayAlert(string, string, string)' is obsolete: 'Use DisplayAlertAsync instead'
```
####迁移示例

* * DisplayAlert: * *```csharp
// ❌ OLD (Deprecated)
await DisplayAlert("Success", "Data saved successfully", "OK");
await DisplayAlert("Error", "Failed to save", "Cancel");
bool result = await DisplayAlert("Confirm", "Delete this item?", "Yes", "No");

// ✅ NEW (Required)
await DisplayAlertAsync("Success", "Data saved successfully", "OK");
await DisplayAlertAsync("Error", "Failed to save", "Cancel");
bool result = await DisplayAlertAsync("Confirm", "Delete this item?", "Yes", "No");
```
* * DisplayActionSheet: * *```csharp
// ❌ OLD (Deprecated)
string action = await DisplayActionSheet(
    "Choose an action",
    "Cancel",
    "Delete",
    "Edit", "Share", "Duplicate"
);

// ✅ NEW (Required)
string action = await DisplayActionSheetAsync(
    "Choose an action",
    "Cancel",
    "Delete",
    "Edit", "Share", "Duplicate"
);
```
**在ViewModels（与IDispatcher）：**```csharp
// If you're calling from a ViewModel, you'll need access to a Page
public class MyViewModel
{
    private readonly IDispatcher _dispatcher;
    private readonly Page _page;
    
    public MyViewModel(IDispatcher dispatcher, Page page)
    {
        _dispatcher = dispatcher;
        _page = page;
    }
    
    public async Task ShowAlertAsync()
    {
        await _dispatcher.DispatchAsync(async () =>
        {
            await _page.DisplayAlertAsync("Info", "Message from ViewModel", "OK");
        });
    }
}
```
---

# # # 3。页面。IsBusy

**状态：**⚠️**已弃用** -属性将在。净11。

**警告你会看到：**```
warning CS0618: 'Page.IsBusy' is obsolete: 'Page.IsBusy has been deprecated and will be removed in .NET 11'
```
**为何被弃用：**
-跨平台行为不一致
-有限的定制选项
-不适合现代MVVM模式

####迁移示例

* *简单的页面:* *```xaml
<!-- ❌ OLD (Deprecated) -->
<ContentPage IsBusy="{Binding IsLoading}">
    <StackLayout>
        <Label Text="Content here" />
    </StackLayout>
</ContentPage>

<!-- ✅ NEW (Recommended) -->
<ContentPage>
    <Grid>
        <!-- Main content -->
        <StackLayout>
            <Label Text="Content here" />
        </StackLayout>
        
        <!-- Loading indicator overlay -->
        <ActivityIndicator IsRunning="{Binding IsLoading}"
                          IsVisible="{Binding IsLoading}"
                          Color="{StaticResource Primary}"
                          VerticalOptions="Center"
                          HorizontalOptions="Center" />
    </Grid>
</ContentPage>
```
**与加载覆盖：**```xaml
<!-- ✅ Better: Custom loading overlay -->
<ContentPage>
    <Grid>
        <!-- Main content -->
        <ScrollView>
            <VerticalStackLayout Padding="20">
                <Label Text="Your content here" />
            </VerticalStackLayout>
        </ScrollView>
        
        <!-- Loading overlay -->
        <Grid IsVisible="{Binding IsLoading}"
              BackgroundColor="#80000000">
            <VerticalStackLayout VerticalOptions="Center"
                               HorizontalOptions="Center"
                               Spacing="10">
                <ActivityIndicator IsRunning="True"
                                 Color="White" />
                <Label Text="Loading..."
                       TextColor="White" />
            </VerticalStackLayout>
        </Grid>
    </Grid>
</ContentPage>
```
在后台代码:* * * *```csharp
// ❌ OLD (Deprecated)
public partial class MyPage : ContentPage
{
    async Task LoadDataAsync()
    {
        IsBusy = true;
        try
        {
            await LoadDataFromServerAsync();
        }
        finally
        {
            IsBusy = false;
        }
    }
}

// ✅ NEW (Recommended)
public partial class MyPage : ContentPage
{
    async Task LoadDataAsync()
    {
        LoadingIndicator.IsVisible = true;
        LoadingIndicator.IsRunning = true;
        try
        {
            await LoadDataFromServerAsync();
        }
        finally
        {
            LoadingIndicator.IsVisible = false;
            LoadingIndicator.IsRunning = false;
        }
    }
}
```
在ViewModel * *: * *```csharp
public class MyViewModel : INotifyPropertyChanged
{
    private bool _isLoading;
    public bool IsLoading
    {
        get => _isLoading;
        set
        {
            _isLoading = value;
            OnPropertyChanged();
        }
    }
    
    public async Task LoadDataAsync()
    {
        IsLoading = true;
        try
        {
            await LoadDataFromServerAsync();
        }
        finally
        {
            IsLoading = false;
        }
    }
}
```
---

# # # 4。MediaPicker api

**状态：**⚠️**已弃用** -单选择方法被多选择变体取代。

**警告你会看到：**```
warning CS0618: 'MediaPicker.PickPhotoAsync(MediaPickerOptions)' is obsolete: 'Switch to PickPhotosAsync which also allows multiple selections.'
warning CS0618: 'MediaPicker.PickVideoAsync(MediaPickerOptions)' is obsolete: 'Switch to PickVideosAsync which also allows multiple selections.'
```
* *什么改变:* *
-`PickPhotoAsync()`→`PickPhotosAsync()`（返回`List<FileResult>`）
-`PickVideoAsync()`→`PickVideosAsync()`（返回`List<FileResult>`）
-在`MediaPickerOptions`上新增`SelectionLimit`属性（默认：1）
-旧的方法仍然有效，但被标记为过时

* *关键行为:* *
- **保留默认行为：**`SelectionLimit = 1`（单次选择）
—设置`SelectionLimit = 0`为无限制多选
—设置“`SelectionLimit > 1`”

* *注:平台* *
-✅**iOS:**本地选择器UI强制的选择限制
-⚠️**Android:**并非所有自定义选择器都支持`SelectionLimit`-请注意！
-⚠️**Windows:**`SelectionLimit`不支持-实现自己的验证

####迁移示例

**简单的照片选择器（保持单一选择行为）：**```csharp
// ❌ OLD (Deprecated)
var photo = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
{
    Title = "Pick a photo"
});

if (photo != null)
{
    var stream = await photo.OpenReadAsync();
    MyImage.Source = ImageSource.FromStream(() => stream);
}

// ✅ NEW (maintains same behavior - picks only 1 photo)
var photos = await MediaPicker.PickPhotosAsync(new MediaPickerOptions
{
    Title = "Pick a photo",
    SelectionLimit = 1  // Explicit: only 1 photo
});

var photo = photos.FirstOrDefault();
if (photo != null)
{
    var stream = await photo.OpenReadAsync();
    MyImage.Source = ImageSource.FromStream(() => stream);
}
```
**简单的视频选择器（保持单一选择行为）：**```csharp
// ❌ OLD (Deprecated)
var video = await MediaPicker.PickVideoAsync(new MediaPickerOptions
{
    Title = "Pick a video"
});

if (video != null)
{
    VideoPlayer.Source = video.FullPath;
}

// ✅ NEW (maintains same behavior - picks only 1 video)
var videos = await MediaPicker.PickVideosAsync(new MediaPickerOptions
{
    Title = "Pick a video",
    SelectionLimit = 1  // Explicit: only 1 video
});

var video = videos.FirstOrDefault();
if (video != null)
{
    VideoPlayer.Source = video.FullPath;
}
```
**没有选项的照片选择器（使用默认值）：**```csharp
// ❌ OLD (Deprecated)
var photo = await MediaPicker.PickPhotoAsync();

// ✅ NEW (default SelectionLimit = 1, so same behavior)
var photos = await MediaPicker.PickPhotosAsync();
var photo = photos.FirstOrDefault();
```
**多张照片选择（新功能）：**```csharp
// ✅ NEW: Pick up to 5 photos
var photos = await MediaPicker.PickPhotosAsync(new MediaPickerOptions
{
    Title = "Pick up to 5 photos",
    SelectionLimit = 5
});

foreach (var photo in photos)
{
    var stream = await photo.OpenReadAsync();
    // Process each photo
}

// ✅ NEW: Unlimited selection
var allPhotos = await MediaPicker.PickPhotosAsync(new MediaPickerOptions
{
    Title = "Pick photos",
    SelectionLimit = 0  // No limit
});
```
**多视频选择（新功能）：**```csharp
// ✅ NEW: Pick up to 3 videos
var videos = await MediaPicker.PickVideosAsync(new MediaPickerOptions
{
    Title = "Pick up to 3 videos",
    SelectionLimit = 3
});

foreach (var video in videos)
{
    // Process each video
    Console.WriteLine($"Selected: {video.FileName}");
}
```
**处理空结果：**```csharp
// NEW: Returns empty list if user cancels (not null)
var photos = await MediaPicker.PickPhotosAsync(new MediaPickerOptions
{
    SelectionLimit = 1
});

// ✅ Check for empty list
if (photos.Count == 0)
{
    await DisplayAlertAsync("Cancelled", "No photo selected", "OK");
    return;
}

var photo = photos.First();
// Process photo...
```
**与Try-Catch（与以前一样）：**```csharp
try
{
    var photos = await MediaPicker.PickPhotosAsync(new MediaPickerOptions
    {
        Title = "Pick a photo",
        SelectionLimit = 1
    });
    
    if (photos.Count > 0)
    {
        await ProcessPhotoAsync(photos.First());
    }
}
catch (PermissionException)
{
    await DisplayAlertAsync("Permission Denied", "Camera access required", "OK");
}
catch (Exception ex)
{
    await DisplayAlertAsync("Error", $"Failed to pick photo: {ex.Message}", "OK");
}
```
####迁移清单

当迁移到新的MediaPicker api时：

—[]将`PickPhotoAsync()`替换为`PickPhotosAsync()`—[]将`PickVideoAsync()`替换为`PickVideosAsync()`-[]设置`SelectionLimit = 1`保持单一选择行为
-[]将`FileResult?`更改为`List<FileResult>`（或使用`.FirstOrDefault()`）
-[]将空检查更新为空列表检查（`photos.Count == 0`）
[]在Android上测试-确保自定义选择器符合限制（或添加验证）
-[]在Windows上测试-如果需要，添加您自己的限制验证
[]考虑多重选择是否会改善你的用户体验（可选）

####平台特定验证（Windows和Android）```csharp
// ✅ Recommended: Validate selection limit on platforms that don't enforce it
var photos = await MediaPicker.PickPhotosAsync(new MediaPickerOptions
{
    Title = "Pick up to 5 photos",
    SelectionLimit = 5
});

// On Windows and some Android pickers, the limit might not be enforced
if (photos.Count > 5)
{
    await DisplayAlertAsync(
        "Too Many Photos", 
        $"Please select up to 5 photos. You selected {photos.Count}.", 
        "OK"
    );
    return;
}

// Continue processing...
```
####捕获方法（未更改）

**注意：**捕获方法(`CapturePhotoAsync`,`CaptureVideoAsync`) **不**弃用，保持不变：```csharp
// ✅ These still work as-is (no changes needed)
var photo = await MediaPicker.CapturePhotoAsync();
var video = await MediaPicker.CaptureVideoAsync();
```
####快速迁移模式

对于所有现有的单选择代码，使用这个模式：**```csharp
// ❌ OLD
var photo = await MediaPicker.PickPhotoAsync(options);
if (photo != null)
{
    // Process photo
}

// ✅ NEW (drop-in replacement)
var photos = await MediaPicker.PickPhotosAsync(options ?? new MediaPickerOptions { SelectionLimit = 1 });
var photo = photos.FirstOrDefault();
if (photo != null)
{
    // Process photo (same code as before)
}
```
---

建议更改（P2）

建议进行这些更改，但不是立即必需的。在下一个重构周期中考虑迁移。

# # #应用程序。主页

**状态：**⚠️**DEPRECATED** -属性将在未来的版本中被删除。

**警告你会看到：**```
warning CS0618: 'Application.MainPage' is obsolete: 'This property is deprecated. Initialize your application by overriding Application.CreateWindow...'
```
####迁移示例```csharp
// ❌ OLD (Deprecated)
public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        MainPage = new AppShell();
    }
    
    // Changing page later
    public void SwitchToLoginPage()
    {
        MainPage = new LoginPage();
    }
}

// ✅ NEW (Recommended)
public partial class App : Application
{
    public App()
    {
        InitializeComponent();
    }
    
    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(new AppShell());
    }
    
    // Changing page later
    public void SwitchToLoginPage()
    {
        if (Windows.Count > 0)
        {
            Windows[0].Page = new LoginPage();
        }
    }
}
```
** CreateWindow的好处：**
-更好的多窗口支持
-更明确的初始化
-更清晰地分离关注点
-与Shell一起工作更好

---

##批量迁移工具

使用这些find/replace模式来快速更新代码库。

### Visual Studio /VS Code**正则表达式模式-Find/Replace**

####动画制作方法```regex
Find:    \.FadeTo\(
Replace: .FadeToAsync(

Find:    \.ScaleTo\(
Replace: .ScaleToAsync(

Find:    \.TranslateTo\(
Replace: .TranslateToAsync(

Find:    \.RotateTo\(
Replace: .RotateToAsync(

Find:    \.RotateXTo\(
Replace: .RotateXToAsync(

Find:    \.RotateYTo\(
Replace: .RotateYToAsync(

Find:    \.ScaleXTo\(
Replace: .ScaleXToAsync(

Find:    \.ScaleYTo\(
Replace: .ScaleYToAsync(

Find:    \.RelRotateTo\(
Replace: .RelRotateToAsync(

Find:    \.RelScaleTo\(
Replace: .RelScaleToAsync(
```
####显示方法```regex
Find:    DisplayAlert\(
Replace: DisplayAlertAsync(

Find:    DisplayActionSheet\(
Replace: DisplayActionSheetAsync(
```
#### MediaPicker方法

**⚠️注：** MediaPicker迁移需要手动代码更改，由于返回类型的变化（`FileResult?`→`List<FileResult>`）。使用这些搜索来查找实例：```bash
# Find PickPhotoAsync usages
grep -rn "PickPhotoAsync" --include="*.cs" .

# Find PickVideoAsync usages
grep -rn "PickVideoAsync" --include="*.cs" .
```
**手动迁移模式：**```csharp
// Find: await MediaPicker.PickPhotoAsync(
// Replace with:
var photos = await MediaPicker.PickPhotosAsync(new MediaPickerOptions { SelectionLimit = 1 });
var photo = photos.FirstOrDefault();

// Find: await MediaPicker.PickVideoAsync(
// Replace with:
var videos = await MediaPicker.PickVideosAsync(new MediaPickerOptions { SelectionLimit = 1 });
var video = videos.FirstOrDefault();
```
####ListView/TableView检测（需要手动迁移）

**⚠️注意：**ListView/TableView迁移不能自动化。使用这些搜索来查找实例：```bash
# Find all ListView usages in XAML
grep -r "<ListView" --include="*.xaml" .

# Find all TableView usages in XAML
grep -r "<TableView" --include="*.xaml" .

# Find ListView in C# code
grep -r "new ListView\|ListView " --include="*.cs" .

# Find Cell types in XAML
grep -r "TextCell\|ImageCell\|EntryCell\|SwitchCell\|ViewCell" --include="*.xaml" .

# Find ItemSelected handlers (need to change to SelectionChanged)
grep -r "ItemSelected=" --include="*.xaml" .
grep -r "ItemSelected\s*\+=" --include="*.cs" .

# Find ContextActions (need to change to SwipeView)
grep -r "ContextActions" --include="*.xaml" .

# Find platform-specific ListView code (needs removal)
grep -r "PlatformConfiguration.*ListView" --include="*.cs" .
```
**创建迁移清单：**```bash
# Generate a report of all ListView/TableView instances
echo "=== ListView/TableView Migration Inventory ===" > migration-report.txt
echo "" >> migration-report.txt
echo "XAML ListView instances:" >> migration-report.txt
grep -rn "<ListView" --include="*.xaml" . >> migration-report.txt
echo "" >> migration-report.txt
echo "XAML TableView instances:" >> migration-report.txt
grep -rn "<TableView" --include="*.xaml" . >> migration-report.txt
echo "" >> migration-report.txt
echo "ItemSelected handlers:" >> migration-report.txt
grep -rn "ItemSelected" --include="*.xaml" --include="*.cs" . >> migration-report.txt
echo "" >> migration-report.txt
cat migration-report.txt
```
### PowerShell脚本```powershell
# Replace animation methods in all .cs files
Get-ChildItem -Path . -Recurse -Filter *.cs | ForEach-Object {
    $content = Get-Content $_.FullName -Raw
    
    # Animation methods
    $content = $content -replace '\.FadeTo\(', '.FadeToAsync('
    $content = $content -replace '\.ScaleTo\(', '.ScaleToAsync('
    $content = $content -replace '\.TranslateTo\(', '.TranslateToAsync('
    $content = $content -replace '\.RotateTo\(', '.RotateToAsync('
    $content = $content -replace '\.RotateXTo\(', '.RotateXToAsync('
    $content = $content -replace '\.RotateYTo\(', '.RotateYToAsync('
    $content = $content -replace '\.ScaleXTo\(', '.ScaleXToAsync('
    $content = $content -replace '\.ScaleYTo\(', '.ScaleYToAsync('
    $content = $content -replace '\.RelRotateTo\(', '.RelRotateToAsync('
    $content = $content -replace '\.RelScaleTo\(', '.RelScaleToAsync('
    
    # Display methods
    $content = $content -replace 'DisplayAlert\(', 'DisplayAlertAsync('
    $content = $content -replace 'DisplayActionSheet\(', 'DisplayActionSheetAsync('
    
    Set-Content $_.FullName $content
}

Write-Host "✅ Migration complete!"
```
---

测试你的升级

###构建验证```bash
# Clean solution
dotnet clean

# Restore packages
dotnet restore

# Build for each platform
dotnet build -f net10.0-android -c Release
dotnet build -f net10.0-ios -c Release
dotnet build -f net10.0-maccatalyst -c Release
dotnet build -f net10.0-windows -c Release

# Check for warnings
dotnet build --no-incremental 2>&1 | grep -i "warning CS0618"
```
###启用警告作为错误（临时）```xml
<!-- Add to your .csproj to catch all obsolete API usage -->
<PropertyGroup>
  <WarningsAsErrors>CS0618</WarningsAsErrors>
</PropertyGroup>
```
测试检查表

-[]应用程序在所有平台上成功启动
-[]所有动画正常工作
-[]对话框（alerts/actionsheets）显示正常
-[]加载指示器工作（如果使用IsBusy）
-[]组件间通信工作（替换MessagingCenter）
-[]构建输出中没有CS0618警告
-[]没有与过时的api相关的运行时异常

---

# #故障排除

###错误：“MessagingCenter”由于其保护级别而无法访问

**原因：** MessagingCenter现在是内部的。净10。

* *解决方案:* *
1. 安装`CommunityToolkit.Mvvm`包
2. 替换为`WeakReferenceMessenger`（参见[MessagingCenter部分](# MessagingCenter -made-internal)）
3. 为每个消息类型创建消息类
4. 不要忘记取消注册！

---

警告：动画方法已过时

**原因：**使用同步动画方法（`FadeTo`，`ScaleTo`等）

* *快速修复:* *```bash
# Use PowerShell script from Bulk Migration Tools section
# Or use Find/Replace patterns
```
* *手工修复:* *
将`Async`添加到每个动画方法调用的末尾：
-`FadeTo`→`FadeToAsync`-`ScaleTo`→`ScaleToAsync`——等等。

---

# # #页面。IsBusy不再工作了

**原因：** IsBusy仍然有效，但已弃用。

**解决方案：**替换为显式ActivityIndicator（参见[IsBusy部分](#3-pageisbusy)）

---

###编译失败，提示“目标框架‘net10.0’未找到”

* *: * *。未安装。NET 10 SDK或不是最新版本。

* *解决方案:* *```bash
# Check SDK version
dotnet --version  # Should be 10.0.100 or later

# Install .NET 10 SDK from:
# https://dotnet.microsoft.com/download/dotnet/10.0

# Update workloads
dotnet workload update
```
---

MessagingCenter迁移会破坏现有代码

共同问题:* * * *

1. **忘记取消注册：**   ```csharp
   // ⚠️ Memory leak if you don't unregister
   protected override void OnDisappearing()
   {
       base.OnDisappearing();
       WeakReferenceMessenger.Default.UnregisterAll(this);
   }
   ```
2. **错误消息类型：**   ```csharp
   // ❌ Wrong
   WeakReferenceMessenger.Default.Register<UserLoggedIn>(this, handler);
   WeakReferenceMessenger.Default.Send(new UserData());  // Wrong type!
   
   // ✅ Correct
   WeakReferenceMessenger.Default.Register<UserLoggedInMessage>(this, handler);
   WeakReferenceMessenger.Default.Send(new UserLoggedInMessage(userData));
   ```
3. **收件人参数混淆：**   ```csharp
   // The recipient parameter is the object that registered (this)
   WeakReferenceMessenger.Default.Register<MyMessage>(this, (recipient, message) =>
   {
       // recipient == this
       // message == the message that was sent
   });
   ```
---

警告：MediaPicker方法已经过时

**原因：**使用已弃用的`PickPhotoAsync`或`PickVideoAsync`方法。

**解决方案：**迁移到`PickPhotosAsync`或`PickVideosAsync`：```csharp
// ❌ OLD
var photo = await MediaPicker.PickPhotoAsync(options);

// ✅ NEW (maintain single-selection)
var photos = await MediaPicker.PickPhotosAsync(new MediaPickerOptions 
{ 
    Title = options?.Title,
    SelectionLimit = 1 
});
var photo = photos.FirstOrDefault();
```
* *主要变化:* *
—返回类型由`FileResult?`变为`List<FileResult>`-使用`.FirstOrDefault()`获得单个结果
—设置“`SelectionLimit = 1`”为保持旧行为
-检查`photos.Count == 0`，而不是`photo == null`---

MediaPicker返回的项目多于SelectionLimit

**原因：** Windows和一些Android自定义选择器不强制`SelectionLimit`。

**解决方案：**增加手动验证：```csharp
var photos = await MediaPicker.PickPhotosAsync(new MediaPickerOptions
{
    SelectionLimit = 5
});

if (photos.Count > 5)
{
    await DisplayAlertAsync("Error", "Too many photos selected", "OK");
    return;
}
```
---

动画在迁移后没有完成

**原因：**忘记关键字`await`。```csharp
// ❌ Wrong - animation runs but code continues immediately
view.FadeToAsync(0, 500);
DoSomethingElse();

// ✅ Correct - wait for animation to complete
await view.FadeToAsync(0, 500);
DoSomethingElse();
```
---

###警告：ListView/TableView/TextCell已过时

**原因：**使用已弃用的ListView， TableView或Cell类型。

**解决方案：**迁移到CollectionView（参见[ListView和TableView章节](# ListView和TableView -deprecated)）

**快速决策指南：**
- **简单列表**→CollectionView与自定义的DataTemplate
- **设置页面与<20项**→垂直stacklayout与bindabllayout
- **设置页面20+项**→分组收集视图
- **分组数据列表**→CollectionView与`IsGrouped="True"`---

CollectionView没有SelectedItem事件

**原因：** CollectionView使用`SelectionChanged`而不是`ItemSelected`。

* *解决方案:* *```csharp
// ❌ OLD (ListView)
void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
{
    var item = e.SelectedItem as MyItem;
}

// ✅ NEW (CollectionView)
void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
{
    var item = e.CurrentSelection.FirstOrDefault() as MyItem;
}
```
---

特定于平台的ListView配置已经过时

**原因：**使用`Microsoft.Maui.Controls.PlatformConfiguration.*Specific.ListView`扩展名。

* *错误:* *```
warning CS0618: 'ListView' is obsolete: 'With the deprecation of ListView, this class is obsolete. Please use CollectionView instead.'
```
* *解决方案:* *
1. 使用语句删除平台特定的ListView：   ```csharp
   // ❌ Remove these
   using Microsoft.Maui.Controls.PlatformConfiguration;
   using Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific;
   using Microsoft.Maui.Controls.PlatformConfiguration.AndroidSpecific;
   ```
2. 删除特定于平台的ListView调用：   ```csharp
   // ❌ Remove these
   myListView.On<iOS>().SetSeparatorStyle(SeparatorStyle.FullWidth);
   myListView.On<Android>().IsFastScrollEnabled();
   viewCell.On<iOS>().SetDefaultBackgroundColor(Colors.White);
   ```
3. CollectionView有不同的平台自定义选项——参考CollectionView文档了解其他选项。

---

ListView迁移后的CollectionView性能问题

* *常见原因:* *

1. **不使用DataTemplate缓存：**   ```xaml
   <!-- ❌ Bad performance -->
   <CollectionView.ItemTemplate>
       <DataTemplate>
           <ComplexView />
       </DataTemplate>
   </CollectionView.ItemTemplate>
   
   <!-- ✅ Better - use simpler templates -->
   <CollectionView.ItemTemplate>
       <DataTemplate>
           <VerticalStackLayout Padding="10">
               <Label Text="{Binding Title}" />
           </VerticalStackLayout>
       </DataTemplate>
   </CollectionView.ItemTemplate>
   ```
2. 复杂的嵌套布局：**
避免在ItemTemplate中深度嵌套布局
-尽可能使用Grid而不是StackLayout
-考虑FlexLayout的复杂布局

3. **图片没有被缓存：**   ```xaml
   <Image Source="{Binding ImageUrl}"
          Aspect="AspectFill"
          HeightRequest="80"
          WidthRequest="80">
       <Image.Behaviors>
           <!-- Add caching behavior if needed -->
       </Image.Behaviors>
   </Image>
   ```
---

##快速参考卡

优先级检查表

**必须修复（P0 -Breaking/Critical）：**
—[]将`MessagingCenter`替换为`WeakReferenceMessenger`—[]迁移`ListView`到`CollectionView`—[]迁移`TableView`到`CollectionView`或`BindableLayout`-[]将`TextCell`，`ImageCell`等替换为自定义datatemplate
-[]将`ContextActions`转换为`SwipeView`-[]删除特定平台的ListView配置

**应该修复（P1 -已弃用）：**
-[]更新动画方法：添加`Async`后缀
-[]更新`DisplayAlert`→`DisplayAlertAsync`-[]更新`DisplayActionSheet`→`DisplayActionSheetAsync`—[]将`Page.IsBusy`替换为`ActivityIndicator`-[]替换`PickPhotoAsync`→`PickPhotosAsync`（用`SelectionLimit = 1`）
-[]替换`PickVideoAsync`→`PickVideosAsync`（用`SelectionLimit = 1`）

**Nice to Have (P2):**
—[]迁移`Application.MainPage`到`CreateWindow`###常见模式```csharp
// Animation
await view.FadeToAsync(0, 500);

// Alert
await DisplayAlertAsync("Title", "Message", "OK");

// Messaging
WeakReferenceMessenger.Default.Send(new MyMessage());
WeakReferenceMessenger.Default.Register<MyMessage>(this, (r, m) => { });
WeakReferenceMessenger.Default.UnregisterAll(this);

// Loading
IsLoading = true;
try { await LoadAsync(); }
finally { IsLoading = false; }
```
---

##其他资源

- **官方文档：**https://learn.microsoft.com/dotnet/maui/- **迁移指南：**https://learn.microsoft.com/dotnet/maui/migration/- **GitHub问题：**https://github.com/dotnet/maui/issues- * * CommunityToolkit。Mvvm: * *https://learn.microsoft.com/dotnet/communitytoolkit/mvvm/---

**文档版本：** 2.0
**最后更新：** 2025年11月
**适用于：**。NET MAUI 10.0.100及更高版本