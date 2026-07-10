---
description: 'Style components with modern theming using Fluent UI'
applyTo: '**/*.{ts,tsx,js,json,xml,pcfproj,csproj}'
---
样式组件与现代主题（预览）

[本主题是预发布文档，可能会有变化。]

开发人员需要能够样式化他们的组件，使它们看起来像应用程序中包含的其他部分。当现代主题对画布应用程序（通过[现代控件和主题]（https://learn.microsoft.com/en-us/power-apps/maker/canvas-apps/controls/modern-controls/overview-modern-controls）功能）或模型驱动应用程序（通过[新刷新外观](https://learn.microsoft.com/en-us/power-apps/user/modern-fluent-design)）生效时，他们可以做到这一点。

使用基于[Fluent UI React v9]（https://react.fluentui.dev/）的现代主题来设计组件的样式。建议使用这种方法为组件获得最佳性能和主题化体验。

运用现代主题的四种方法

1. **流畅的UI v9控件**
2. **流畅UI v8控件**
3. **非流畅的UI控件**
4. **自定义主题提供者**

##流畅UI v9控件将Fluent UI v9控件包装为组件是利用现代主题的最简单方法，因为现代主题会自动应用于这些控件。唯一的先决条件是确保你的组件添加了一个依赖于[React控件和平台库]（https://learn.microsoft.com/en-us/power-apps/developer/component-framework/react-controls-platform-libraries）。

这种方法允许组件使用与平台相同的React和Fluent库，因此共享将主题令牌传递给组件的相同React上下文。```xml
<resources>
  <code path="index.ts" order="1"/>
  <!-- Dependency on React controls & platform libraries -->
  <platform-library name="React" version="16.14.0" />
  <platform-library name="Fluent" version="9.46.2" />
</resources>
```
##流畅UI v8控件

当你在组件中使用Fluent UI v8控件时，Fluent提供了一个迁移路径来应用v9主题结构。使用[Fluent的v8到v9迁移包]（https://www.npmjs.com/package/@fluentui/react-migration-v8-v9）中包含的`createV8Theme`函数创建基于v9主题令牌的v8主题：```typescript
const theme = createV8Theme(
  context.fluentDesignLanguage.brand,
  context.fluentDesignLanguage.theme
);
return <ThemeProvider theme={theme}></ThemeProvider>;
```
非流畅UI控件

如果组件不使用Fluent UI，则可以通过`fluentDesignLanguage`上下文参数直接依赖于v9主题令牌。使用这个参数可以访问所有[theme]（https://learn.microsoft.com/en-us/power-apps/developer/component-framework/reference/theming）令牌，这样它就可以引用主题的任何方面来样式化自己。```typescript
<span style={{ fontSize: context.fluentDesignLanguage.theme.fontSizeBase300 }}>
  {"Stylizing HTML with platform provided theme."}
</span>
```
自定义主题提供程序

当你的组件需要不同于应用当前主题的样式时，创建你自己的`FluentProvider`，并传递你自己的一组主题令牌，供你的组件使用。```typescript
<FluentProvider theme={context.fluentDesignLanguage.tokenTheme}>
  {/* your control */}
</FluentProvider>
```
##样本控制

每个用例的示例都可以在[现代主题API控制]（https://learn.microsoft.com/en-us/power-apps/developer/component-framework/sample-controls/modern-theming-api-control）中获得。

# #常见问题解答

Q：我的控件使用Fluent UI v9，并且依赖于平台库，但我不想使用现代主题。如何为我的组件禁用它？

A：有两种方法：

选项1：创建自己的组件级`FluentProvider````typescript
<FluentProvider theme={customFluentV9Theme}>
  {/* your control */}
</FluentProvider>
```
**选项2**：将控件包装在`IdPrefixContext.Provider`中，并设置自己的`idPrefix`值。这可以防止组件从平台获取主题令牌。```typescript
<IdPrefixProvider value="custom-control-prefix">
  <Label weight="semibold">This label is not getting Modern Theming</Label>
</IdPrefixProvider>
```
问：我的一些Fluent UI v9控件没有获得样式

答：依赖于React Portal的Fluent v9控件需要在主题提供程序中重新包装，以确保样式被正确应用。您可以使用`FluentProvider`。

问：如何检查是否启用了现代主题？

答：您可以检查令牌是否可用：`context.fluentDesignLanguage?.tokenTheme`。或者在模型驱动的应用程序中，您可以检查应用程序设置：`context.appSettings.getIsFluentThemingEnabled()`。

##相关文章

-[主题（Power Apps组件框架API参考）]（https://learn.microsoft.com/en-us/power-apps/developer/component-framework/reference/theming）
-[现代主题API控制]（https://learn.microsoft.com/en-us/power-apps/developer/component-framework/sample-controls/modern-theming-api-control）
-[在画布应用中使用现代主题（预览）]（https://learn.microsoft.com/en-us/power-apps/maker/canvas-apps/controls/modern-controls/modern-theming）