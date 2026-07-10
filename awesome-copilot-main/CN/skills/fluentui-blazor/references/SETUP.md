#设置和配置

NuGet包

|包装|用途||---|---|
|`Microsoft.FluentUI.AspNetCore.Components`|核心组件库（必选）|
|`Microsoft.FluentUI.AspNetCore.Components.Icons`|图标包（可选，推荐）|
|`Microsoft.FluentUI.AspNetCore.Components.Emojis`|表情包（可选）|
|`Microsoft.FluentUI.AspNetCore.Components.DataGrid.EntityFrameworkAdapter`|数据网格EF核心适配器（可选）|
|`Microsoft.FluentUI.AspNetCore.Components.DataGrid.ODataAdapter`| odatagrid的数据适配器（可选）|Program.cs注册```csharp
builder.Services.AddFluentUIComponents();
```
配置选项（LibraryConfiguration）

|属性|类型|默认|备注||---|---|---|---|
|`UseTooltipServiceProvider`|`bool`|`true`|`ITooltipService`寄存器。如果为真，你必须将`<FluentTooltipProvider>`添加到布局|
|`RequiredLabel`|`MarkupString`|红色`*`|所需字段指示器|的自定义标记
|`HideTooltipOnCursorLeave`|`bool`|`false`|当光标离开锚和工具提示|时关闭工具提示
|`ServiceLifetime`|`ServiceLifetime`|`Scoped`|只有`Scoped`或`Singleton`。`Transient`扔!|
|`ValidateClassNames`|`bool`|`true`|根据`^-?[_a-zA-Z]+[_a-zA-Z0-9-]*$`|验证CSS类名
|`CollocatedJavaScriptQueryString`|`Func<string, string>?`|`v={version}`| JS文件缓存破坏|

ServiceLifetime通过托管模型

|托管模式| ServiceLifetime ||---|---|
| Blazor Server |`Scoped`（默认）|
| Blazor WebAssembly Standalone |`Singleton`|
| Blazor Web App（交互式）|`Scoped`（默认）|
| Blazor Hybrid (MAUI) |`Singleton`|

# # MainLayout。剃须刀模板```razor
@inherits LayoutComponentBase

<FluentLayout>
    <FluentHeader Height="50">
        My App
    </FluentHeader>

    <FluentStack Orientation="Orientation.Horizontal" HorizontalGap="0" Style="height: 100%;">
        <FluentNavMenu Width="250" Collapsible="true" Title="Navigation">
            <FluentNavLink Href="/" Icon="@(Icons.Regular.Size20.Home)" Match="NavLinkMatch.All">Home</FluentNavLink>
            <FluentNavLink Href="/counter" Icon="@(Icons.Regular.Size20.NumberSymbol)">Counter</FluentNavLink>
            <FluentNavGroup Title="Settings" Icon="@(Icons.Regular.Size20.Settings)">
                <FluentNavLink Href="/settings/general">General</FluentNavLink>
                <FluentNavLink Href="/settings/profile">Profile</FluentNavLink>
            </FluentNavGroup>
        </FluentNavMenu>

        <FluentBodyContent>
            <FluentStack Orientation="Orientation.Vertical" Style="padding: 1rem;">
                @Body
            </FluentStack>
        </FluentBodyContent>
    </FluentStack>
</FluentLayout>

@* Required providers — place after FluentLayout *@
<FluentToastProvider />
<FluentDialogProvider />
<FluentMessageBarProvider />
<FluentTooltipProvider />
<FluentKeyCodeProvider />

@* Theme — place at root *@
<FluentDesignTheme Mode="DesignThemeModes.System"
                   OfficeColor="OfficeColor.Teams"
                   StorageName="mytheme" />
```
或者使用方便组件：```razor
<FluentMainLayout Header="@header"
                  NavMenuContent="@navMenu"
                  Body="@body"
                  HeaderHeight="50"
                  NavMenuWidth="250"
                  NavMenuTitle="Navigation" />

@code {
    private RenderFragment header = @<span>My App</span>;
    private RenderFragment navMenu = @<div>
        <FluentNavLink Href="/">Home</FluentNavLink>
    </div>;
    private RenderFragment body = @<div>@Body</div>;
}
```
# # _Imports.razor

将此添加到您的`_Imports.razor`：```razor
@using Microsoft.FluentUI.AspNetCore.Components
@using Icons = Microsoft.FluentUI.AspNetCore.Components.Icons
```
静态Web资产

不需要手动使用`<link>`或`<script>`标签。图书馆使用：
**CSS**:`reboot.css`（规范化）+组件作用域CSS -通过静态web资产自动加载
**JS**:`lib.module.js`-通过Blazor的JS初始化系统自动加载
-特定于组件的JS（例如DataGrid， Autocomplete） -按需惰性加载

所有服务从`_content/Microsoft.FluentUI.AspNetCore.Components/`。

##已注册服务`AddFluentUIComponents()`自动注册的服务：

|业务|实现|目的||---|---|---|
|`GlobalState`|`GlobalState`|共享应用状态|
|`IToastService`|`ToastService`|吐司通知（需要`FluentToastProvider`） |
|`IDialogService`|`DialogService`|对话框和面板（需要`FluentDialogProvider`） |
|`IMessageService`|`MessageService`|消息栏（需要`FluentMessageBarProvider`） |
|`IKeyCodeService`|`KeyCodeService`|键盘快捷键（需要`FluentKeyCodeProvider`） |
|`IMenuService`|`MenuService`|上下文菜单|
|`ITooltipService`|`TooltipService`|工具提示（需要`FluentTooltipProvider`，选择通过`UseTooltipServiceProvider`） |