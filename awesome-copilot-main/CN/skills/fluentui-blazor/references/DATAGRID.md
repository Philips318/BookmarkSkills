# FluentDataGrid`FluentDataGrid<TGridItem>`是用于显示表格数据的强类型通用组件。

##基本用法```razor
<FluentDataGrid Items="@people" TGridItem="Person">
    <PropertyColumn Property="@(p => p.Name)" Sortable="true" />
    <PropertyColumn Property="@(p => p.Email)" />
    <PropertyColumn Property="@(p => p.BirthDate)" Format="yyyy-MM-dd" />
    <TemplateColumn Title="Actions">
        <FluentButton OnClick="@(() => Edit(context))">Edit</FluentButton>
    </TemplateColumn>
</FluentDataGrid>
```
**关键**：列是子组件，而不是属性。在网格中使用`PropertyColumn`、`TemplateColumn`和`SelectColumn`。

##列类型

# # # PropertyColumn

绑定到属性表达式。自动从属性名称或`[Display]`属性派生标题。```razor
<PropertyColumn Property="@(p => p.Name)" Sortable="true" />
<PropertyColumn Property="@(p => p.Price)" Format="C2" Title="Unit Price" />
<PropertyColumn Property="@(p => p.Category)" Comparer="@StringComparer.OrdinalIgnoreCase" />
```
参数：`Property`（必选）、`Format`、`Title`、`Sortable`、`SortBy`、`Comparer`、`IsDefaultSortColumn`、`InitialSortDirection`、`Class`、`Tooltip`。

# # # TemplateColumn

完全自定义渲染通过渲染片段。`context`是`TGridItem`。```razor
<TemplateColumn Title="Status" SortBy="@statusSort">
    <FluentBadge Appearance="Appearance.Accent"
                 BackgroundColor="@(context.IsActive ? "green" : "red")">
        @(context.IsActive ? "Active" : "Inactive")
    </FluentBadge>
</TemplateColumn>
```
# # # SelectColumn

复选框选择列。```razor
<SelectColumn TGridItem="Person"
              SelectMode="DataGridSelectMode.Multiple"
              @bind-SelectedItems="@selectedPeople" />
```
模式：`DataGridSelectMode.Single`、`DataGridSelectMode.Multiple`。

##数据来源

两种相互排斥的方法：

In-memory （iquerable）```razor
<FluentDataGrid Items="@people.AsQueryable()" TGridItem="Person">
    ...
</FluentDataGrid>
```
服务器端/自定义（ItemsProvider）```razor
<FluentDataGrid ItemsProvider="@peopleProvider" TGridItem="Person">
    ...
</FluentDataGrid>

@code {
    private GridItemsProvider<Person> peopleProvider = async request =>
    {
        var result = await PeopleService.GetPeopleAsync(
            request.StartIndex,
            request.Count ?? 50,
            request.GetSortByProperties().FirstOrDefault());

        return GridItemsProviderResult.From(result.Items, result.TotalCount);
    };
}
```
EF核心适配器```csharp
// Program.cs
builder.Services.AddDataGridEntityFrameworkAdapter();
```

```razor
<FluentDataGrid Items="@dbContext.People" TGridItem="Person">
    ...
</FluentDataGrid>
```
# #分页```razor
<FluentDataGrid Items="@people" Pagination="@pagination" TGridItem="Person">
    ...
</FluentDataGrid>

<FluentPaginator State="@pagination" />

@code {
    private PaginationState pagination = new() { ItemsPerPage = 10 };
}
```
# #虚拟化

对于大型数据集，启用虚拟化：```razor
<FluentDataGrid Items="@people" Virtualize="true" ItemSize="46" TGridItem="Person">
    ...
</FluentDataGrid>
```
`ItemSize`是估计的行高，以像素为单位（默认值不同）。对于滚动位置计算很重要。

##关键参数

| |类型|描述||---|---|---|
|`Items`|`IQueryable<TGridItem>?`|内存数据源|
|`ItemsProvider`|`GridItemsProvider<TGridItem>?`|异步数据提供|
|`Pagination`|`PaginationState?`|分页状态|
|`Virtualize`|`bool`|开启虚拟化|
|`ItemSize`|`float`|估计行高（px） |
|`ItemKey`|`Func<TGridItem, object>?`|`@key`|的稳定键
|`ResizableColumns`|`bool`|启用列大小调整|
|`HeaderCellAsButtonWithMenu`|`bool`|可排序头用户界面|
|`GridTemplateColumns`|`string?`| CSS网格模板列|
|`Loading`|`bool`|显示加载指示灯|
|`ShowHover`|`bool`|突出显示悬停|上的行
|`OnRowClick`|`EventCallback<FluentDataGridRow<TGridItem>>`|行单击处理程序|
|`OnRowDoubleClick`|`EventCallback<FluentDataGridRow<TGridItem>>`|行双击处理程序|
|`OnRowFocus`|`EventCallback<FluentDataGridRow<TGridItem>>`|行焦点处理程序|

# #排序```razor
<PropertyColumn Property="@(p => p.Name)" Sortable="true" IsDefaultSortColumn="true"
                InitialSortDirection="SortDirection.Ascending" />
```
或者使用自定义排序：```razor
<TemplateColumn Title="Full Name" SortBy="@(GridSort<Person>.ByAscending(p => p.LastName).ThenAscending(p => p.FirstName))">
    @context.LastName, @context.FirstName
</TemplateColumn>
```
