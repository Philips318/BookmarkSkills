#布局和导航

##布局组件

# # # FluentLayout

根布局容器。用作最外层的结构部件。```razor
<FluentLayout Orientation="Orientation.Vertical">
    <FluentHeader>...</FluentHeader>
    <FluentBodyContent>...</FluentBodyContent>
    <FluentFooter>...</FluentFooter>
</FluentLayout>
```
### fluenheader / FluentFooter

在`FluentLayout`内粘贴页眉和页脚部分。```razor
<FluentHeader Height="50">
    <FluentStack Orientation="Orientation.Horizontal" HorizontalAlignment="HorizontalAlignment.SpaceBetween">
        <span>App Title</span>
        <FluentButton>Settings</FluentButton>
    </FluentStack>
</FluentHeader>
```
# # # FluentBodyContent`FluentLayout`内的主要可滚动内容区域。

# # # FluentStack

水平或垂直布局的弹性容器。```razor
<FluentStack Orientation="Orientation.Horizontal"
             HorizontalGap="10"
             VerticalGap="10"
             HorizontalAlignment="HorizontalAlignment.Center"
             VerticalAlignment="VerticalAlignment.Center"
             Wrap="true"
             Width="100%">
    <FluentButton>One</FluentButton>
    <FluentButton>Two</FluentButton>
</FluentStack>
```
参数：`Orientation`、`HorizontalGap`、`VerticalGap`、`HorizontalAlignment`、`VerticalAlignment`、`Wrap`、`Width`。

### FluentGrid / FluentGridItem

12列响应式网格系统。```razor
<FluentGrid Spacing="3" Justify="JustifyContent.Center" AdaptiveRendering="true">
    <FluentGridItem xs="12" sm="6" md="4" lg="3">
        Card 1
    </FluentGridItem>
    <FluentGridItem xs="12" sm="6" md="4" lg="3">
        Card 2
    </FluentGridItem>
</FluentGrid>
```
大小参数（`xs`、`sm`、`md`、`lg`、`xl`、`xxl`）表示12个列的跨度。使用`AdaptiveRendering="true"`隐藏不适合的项。

### FluentMainLayout（方便）

预先组成的布局与标题，导航菜单，和主体区域。```razor
<FluentMainLayout Header="@header"
                  SubHeader="@subheader"
                  NavMenuContent="@navMenu"
                  Body="@body"
                  HeaderHeight="50"
                  NavMenuWidth="250"
                  NavMenuTitle="Navigation" />
```
##导航组件

# # # FluentNavMenu

可折叠导航菜单与键盘支持。```razor
<FluentNavMenu Width="250"
               Collapsible="true"
               @bind-Expanded="@menuExpanded"
               Title="Main navigation"
               CollapsedChildNavigation="true"
               Margin="4px 0">
    <FluentNavLink Href="/" Icon="@(Icons.Regular.Size20.Home)" Match="NavLinkMatch.All">
        Home
    </FluentNavLink>
    <FluentNavLink Href="/counter" Icon="@(Icons.Regular.Size20.NumberSymbol)">
        Counter
    </FluentNavLink>
    <FluentNavGroup Title="Admin" Icon="@(Icons.Regular.Size20.Shield)" @bind-Expanded="@adminExpanded">
        <FluentNavLink Href="/admin/users">Users</FluentNavLink>
        <FluentNavLink Href="/admin/roles">Roles</FluentNavLink>
    </FluentNavGroup>
</FluentNavMenu>
```
关键参数:
-`Width`-宽度以像素为单位（折叠时为40px）
-`Collapsible`-启用expand/collapse切换
-`Expanded`/`ExpandedChanged`-可绑定坍缩状态
-`CollapsedChildNavigation`-显示组折叠时的弹出菜单
-`CustomToggle`-用于移动汉堡按钮模式
-`Title`- aria-label用于可访问性

# # # FluentNavGroup

导航菜单中可展开的组。```razor
<FluentNavGroup Title="Settings"
                Icon="@(Icons.Regular.Size20.Settings)"
                @bind-Expanded="@settingsExpanded"
                Gap="2">
    <FluentNavLink Href="/settings/general">General</FluentNavLink>
    <FluentNavLink Href="/settings/profile">Profile</FluentNavLink>
</FluentNavGroup>
```
参数：`Title`、`Expanded`/`ExpandedChanged`、`Icon`、`IconColor`、`HideExpander`、`Gap`、`MaxHeight`、`TitleTemplate`。

# # # FluentNavLink

具有活动状态跟踪的导航链接。```razor
<FluentNavLink Href="/page"
               Icon="@(Icons.Regular.Size20.Document)"
               Match="NavLinkMatch.Prefix"
               Target="_blank"
               Disabled="false">
    Page Title
</FluentNavLink>
```
参数：`Href`、`Target`、`Match`（默认为`NavLinkMatch.Prefix`，或`All`）、`ActiveClass`、`Icon`、`IconColor`、`Disabled`、`Tooltip`。

所有导航组件都继承自`FluentNavBase`，它提供：`Icon`、`IconColor`、`CustomColor`、`Disabled`、`Tooltip`。

### FluentBreadcrumb / FluentBreadcrumbItem```razor
<FluentBreadcrumb>
    <FluentBreadcrumbItem Href="/">Home</FluentBreadcrumbItem>
    <FluentBreadcrumbItem Href="/products">Products</FluentBreadcrumbItem>
    <FluentBreadcrumbItem>Current Page</FluentBreadcrumbItem>
</FluentBreadcrumb>
```
### FluentTab / FluentTabs```razor
<FluentTabs @bind-ActiveTabId="@activeTab">
    <FluentTab Id="tab1" Label="Details">
        Details content
    </FluentTab>
    <FluentTab Id="tab2" Label="History">
        History content
    </FluentTab>
</FluentTabs>
```
