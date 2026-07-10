#主题

## FluentDesignTheme（推荐）

主主题组件。把它放在应用程序的根目录。```razor
<FluentDesignTheme Mode="DesignThemeModes.System"
                   OfficeColor="OfficeColor.Teams"
                   StorageName="mytheme" />
```
# # #参数

| |类型|默认值|描述||---|---|---|---|
|`Mode`|`DesignThemeModes`|`System`|`Light`、`Dark`、`System`（以操作系统为准）|
|`CustomColor`|`string?`| null |十六进制强调色（例如`"#0078D4"`） |
|`OfficeColor`|`OfficeColor?`| null |预设重音：`Teams`、`Word`、`Excel`、`PowerPoint`、`Outlook`、`OneNote`|
|`NeutralBaseColor`|`string?`| null |中性调色板基础十六进制颜色|
|`StorageName`|`string?`| null |在此键|下将主题持久化到localStorage
|`Direction`|`LocalizationDirection?`| null |`Ltr`或`Rtl`|
|`OnLuminanceChanged`|`EventCallback<LuminanceChangedEventArgs>`| |当dark/light模式改变|时触发
|`OnLoaded`|`EventCallback<LoadedEventArgs>`| |当主题从存储|加载时触发

双向绑定```razor
<FluentDesignTheme @bind-Mode="@themeMode"
                   @bind-OfficeColor="@officeColor"
                   @bind-CustomColor="@customColor"
                   StorageName="mytheme" />

<FluentSelect Items="@(Enum.GetValues<DesignThemeModes>())"
              @bind-SelectedOption="@themeMode"
              OptionText="@(m => m.ToString())" />

@code {
    private DesignThemeModes themeMode = DesignThemeModes.System;
    private OfficeColor? officeColor = OfficeColor.Teams;
    private string? customColor;
}
```
重要：JS互操作依赖`FluentDesignTheme`在内部使用JavaScript互操作。它不会在服务器端预渲染期间工作。如果你需要对主题变化做出反应：```csharp
// Use OnAfterRenderAsync, NOT OnInitialized
protected override async Task OnAfterRenderAsync(bool firstRender)
{
    if (firstRender)
    {
        // Safe to interact with design tokens here
    }
}
```
## FluentDesignSystemProvider（高级）

用于将设计令牌限定在组件树的子树中。提供50+ CSS自定义属性。```razor
<FluentDesignSystemProvider AccentBaseColor="#0078D4"
                            NeutralBaseColor="#808080"
                            BaseLayerLuminance="0.95">
    <FluentButton Appearance="Appearance.Accent">Themed Button</FluentButton>
</FluentDesignSystemProvider>
```
设计令牌类（基于di，高级）

通过依赖注入进行程序化令牌控制。每个令牌都是一个生成的服务。```csharp
@inject AccentBaseColor AccentBaseColor

protected override async Task OnAfterRenderAsync(bool firstRender)
{
    if (firstRender)
    {
        // Set token for a specific element
        await AccentBaseColor.SetValueFor(myElement, "#FF0000".ToSwatch());

        // Read token value
        var currentColor = await AccentBaseColor.GetValueFor(myElement);

        // Remove override
        await AccentBaseColor.DeleteValueFor(myElement);
    }
}
```
## DesignThemeModes可用

-`DesignThemeModes.Light`- light主题
-`DesignThemeModes.Dark`-暗主题
-`DesignThemeModes.System`-遵循操作系统偏好

可用的OfficeColor预设`Teams`,`Word`,`Excel`,`PowerPoint`,`Outlook`,`OneNote`,`Planner`,`SharePoint`,`Stream`,`Sway`,`VivaEngage`,`VivaInsights`,`VivaLearning`,`VivaTopics`。