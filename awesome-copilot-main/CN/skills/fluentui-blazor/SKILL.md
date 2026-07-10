---
name: fluentui-blazor
description: >
  Guide for using the Microsoft Fluent UI Blazor component library
  (Microsoft.FluentUI.AspNetCore.Components NuGet package) in Blazor applications.
  Use this when the user is building a Blazor app with Fluent UI components,
  setting up the library, using FluentUI components like FluentButton, FluentDataGrid,
  FluentDialog, FluentToast, FluentNavMenu, FluentTextField, FluentSelect,
  FluentAutocomplete, FluentDesignTheme, or any component prefixed with "Fluent".
  Also use when troubleshooting missing providers, JS interop issues, or theming.
---
# Fluent UI Blazor -消费者使用指南

本技能教你如何正确使用**Microsoft.FluentUI.AspNetCore。组件**（版本4）NuGet包在Blazor应用程序。

##关键规则

# # # 1。不需要手动`<script>`或`<link>`标签

该库通过Blazor的静态web资源和JS初始化器自动加载所有CSS和JS。**永远不要告诉用户为核心库添加`<script>`或`<link>`标签

# # # 2。提供者对于基于服务的组件是必需的

这些提供商组件**必须**添加到根布局（例如`MainLayout.razor`），以使其相应的服务工作。没有它们，服务调用会静默失败（没有错误，没有UI）。```razor
<FluentToastProvider />
<FluentDialogProvider />
<FluentMessageBarProvider />
<FluentTooltipProvider />
<FluentKeyCodeProvider />
```
# # # 3。在Program.cs中注册服务```csharp
builder.Services.AddFluentUIComponents();

// Or with configuration:
builder.Services.AddFluentUIComponents(options =>
{
    options.UseTooltipServiceProvider = true;  // default: true
    options.ServiceLifetime = ServiceLifetime.Scoped; // default
});
```
* * ServiceLifetime规则:* *
-`ServiceLifetime.Scoped`-用于Blazor服务器/交互式（默认）
-`ServiceLifetime.Singleton`-用于Blazor WebAssembly单机
-`ServiceLifetime.Transient`- **抛出`NotSupportedException`**

# # # 4。图标需要一个单独的NuGet包```
dotnet add package Microsoft.FluentUI.AspNetCore.Components.Icons
```
使用`@using`别名：```razor
@using Icons = Microsoft.FluentUI.AspNetCore.Components.Icons

<FluentIcon Value="@(Icons.Regular.Size24.Save)" />
<FluentIcon Value="@(Icons.Filled.Size20.Delete)" Color="@Color.Error" />
```
模式:`Icons.[Variant].[Size].[Name]`—变体：`Regular`、`Filled`-尺寸：`Size12`，`Size16`,`Size20`,`Size24`,`Size28`,`Size32`,`Size48`自定义映像：`Icon.FromImageUrl("/path/to/image.png")`**不要使用基于字符串的图标名称** -图标是强类型的类。

# # # 5。列表组件绑定模型`FluentSelect<TOption>`,`FluentCombobox<TOption>`,`FluentListbox<TOption>`，和`FluentAutocomplete<TOption>`不像`<InputSelect>`。他们使用:

-`Items`-数据源（`IEnumerable<TOption>`）
-`OptionText`-`Func<TOption, string?>`提取显示文本
—`OptionValue`—`Func<TOption, string?>`提取值字符串
-`SelectedOption`/`SelectedOptionChanged`-用于单一选择绑定
-`SelectedOptions`/`SelectedOptionsChanged`-用于多选择绑定```razor
<FluentSelect Items="@countries"
              OptionText="@(c => c.Name)"
              OptionValue="@(c => c.Code)"
              @bind-SelectedOption="@selectedCountry"
              Label="Country" />
```
**不是这样的（错误的图案）：```razor
@* WRONG — do not use InputSelect pattern *@
<FluentSelect @bind-Value="@selectedValue">
    <option value="1">One</option>
</FluentSelect>
```
# # # 6。FluentAutocomplete细节

-使用`ValueText`（不是`Value`-它已经过时了）作为搜索输入文本
-`OnOptionsSearch`是筛选选项所需的回调
—默认为`Multiple="true"````razor
<FluentAutocomplete TOption="Person"
                    OnOptionsSearch="@OnSearch"
                    OptionText="@(p => p.FullName)"
                    @bind-SelectedOptions="@selectedPeople"
                    Label="Search people" />

@code {
    private void OnSearch(OptionsSearchEventArgs<Person> args)
    {
        args.Items = allPeople.Where(p =>
            p.FullName.Contains(args.Text, StringComparison.OrdinalIgnoreCase));
    }
}
```
# # # 7。对话服务模式

**不要切换`<FluentDialog>`标签的可见性。**服务模式为：

1. 创建一个实现`IDialogContentComponent<TData>`的内容组件：```csharp
public partial class EditPersonDialog : IDialogContentComponent<Person>
{
    [Parameter] public Person Content { get; set; } = default!;

    [CascadingParameter] public FluentDialog Dialog { get; set; } = default!;

    private async Task SaveAsync()
    {
        await Dialog.CloseAsync(Content);
    }

    private async Task CancelAsync()
    {
        await Dialog.CancelAsync();
    }
}
```
2. 通过`IDialogService`显示对话框：```csharp
[Inject] private IDialogService DialogService { get; set; } = default!;

private async Task ShowEditDialog()
{
    var dialog = await DialogService.ShowDialogAsync<EditPersonDialog, Person>(
        person,
        new DialogParameters
        {
            Title = "Edit Person",
            PrimaryAction = "Save",
            SecondaryAction = "Cancel",
            Width = "500px",
            PreventDismissOnOverlayClick = true,
        });

    var result = await dialog.Result;
    if (!result.Cancelled)
    {
        var updatedPerson = result.Data as Person;
    }
}
```
为了方便对话框：```csharp
await DialogService.ShowConfirmationAsync("Are you sure?", "Yes", "No");
await DialogService.ShowSuccessAsync("Done!");
await DialogService.ShowErrorAsync("Something went wrong.");
```
# # # 8。烤面包的通知```csharp
[Inject] private IToastService ToastService { get; set; } = default!;

ToastService.ShowSuccess("Item saved successfully");
ToastService.ShowError("Failed to save");
ToastService.ShowWarning("Check your input");
ToastService.ShowInfo("New update available");
```
`FluentToastProvider`参数：`Position`（默认为`TopRight`）、`Timeout`（默认为7000ms）、`MaxToastCount`（默认为4）。

# # # 9。设计令牌和主题只有在渲染后才能工作

设计令牌依赖于JS互操作。**不要在`OnInitialized`**中设置它们-使用`OnAfterRenderAsync`。```razor
<FluentDesignTheme Mode="DesignThemeModes.System"
                   OfficeColor="OfficeColor.Teams"
                   StorageName="mytheme" />
```
# # # 10。FluentEditForm vs EditForm`FluentEditForm`只需要在`FluentWizard`步骤中使用（每一步验证）。对于常规表单，使用标准`EditForm`与Fluent表单组件：```razor
<EditForm Model="@model" OnValidSubmit="HandleSubmit">
    <DataAnnotationsValidator />
    <FluentTextField @bind-Value="@model.Name" Label="Name" Required />
    <FluentSelect Items="@options"
                  OptionText="@(o => o.Label)"
                  @bind-SelectedOption="@model.Category"
                  Label="Category" />
    <FluentValidationSummary />
    <FluentButton Type="ButtonType.Submit" Appearance="Appearance.Accent">Save</FluentButton>
</EditForm>
```
使用`FluentValidationMessage`和`FluentValidationSummary`来代替标准的Blazor验证组件来实现Fluent样式。

##参考文件

有关具体主题的详细指导，请参见：

- [Setup and configuration]（references/SETUP.md）
-[布局和导航]（references/LAYOUT-AND-NAVIGATION.md）
-[数据网格]（references/DATAGRID.md）
-(主题)(references/THEMING.md)