---
description: 'Complete PCF API reference with all interfaces and their availability in model-driven and canvas apps'
applyTo: '**/*.{ts,tsx,js}'
---
# Power Apps组件框架API参考

Power Apps组件框架提供了一组丰富的api，使您能够创建功能强大的代码组件。此参考列出了所有可用的接口及其在不同应用类型中的可用性。

## API可用性

下表显示了Power Apps组件框架中可用的所有API接口，以及它们在模型驱动应用和画布应用中的可用性。

| API |模型驱动应用|画布应用||-----|------------------|-------------|
| AttributeMetadata |是|否|
|客户端|是|是|
|列|是|是|
| ConditionExpression |是|是|
|背景信息|是|是|
|数据集|是|是|
|设备|是|是|
|实体|是|是|
|事件|是|是|
|工厂|是|是|
|过滤|是|是|
|正在格式化|是|是|
| ImageObject |是|是|
|链接|是|是|
|模式|是|是|
|导航|是|是|
| numberformatinginfo |是|是|
|寻呼|是|是|
|弹出|是|是|
| PopupService |是|是|
| PropertyHelper |是|是|
|资源|是|是|
| SortStatus |是|是|
| StandardControl |是|是|
|用户设置|是|是|
|实用程序|是|是|
| WebApi |是|是|

关键API命名空间

上下文api`Context`对象提供对所有框架功能的访问，并传递给组件的生命周期方法。它包含:- **客户端**：客户端信息（外形尺寸、网络状态）
- **设备**：设备功能（摄像头，位置，麦克风）
- **Factory**：用于创建框架对象的工厂方法
—**格式**：数字和日期格式
- **模式**：组件模式和跟踪
—**Navigation**：导航方法
- **资源**：访问资源（图像，字符串）
- **UserSettings**：用户设置（区域设置，数字格式，安全角色）
- **Utils**：实用方法（getEntityMetadata, hasEntityPrivilege, lookuobjects）
- **WebApi**: Dataverse Web API方法

数据api

- **DataSet**：使用表格数据
—**Column**：访问列元数据和数据
—**实体**：访问记录数据
—**过滤**：定义数据过滤
—**链接**：定义关系
—**分页**：处理数据分页
—**SortStatus**：管理排序

### UI api—**Popup**：创建弹出对话框
—**PopupService**：管理弹出窗口的生命周期
—**Mode**：获取组件渲染模式

元数据api

—**AttributeMetadata**：列元数据（仅模型驱动）
—**PropertyHelper**：属性元数据助手

标准控制

- **StandardControl**：具有生命周期方法的所有代码组件的基本接口：
—`init()`：初始化组件
—`updateView()`：更新组件界面
—`destroy()`：清理资源
—`getOutputs()`：返回输出值

##使用指南

模型驱动vs画布应用

由于平台差异，一些api仅在模型驱动的应用程序中可用：

—**AttributeMetadata**：仅模型驱动-提供详细的列元数据
-大多数其他api在两个平台上都可用

API版本兼容性-始终检查目标平台（模型驱动或画布）的API可用性
-某些api可能在不同平台上有不同的行为
—在目标环境中测试组件，确保兼容性

###常见模式

1. **访问上下文api **   ```typescript
   // In init or updateView
   const userLocale = context.userSettings.locale;
   const isOffline = context.client.isOffline();
   ```
2. **使用DataSet**   ```typescript
   // Access dataset records
   const records = context.parameters.dataset.records;
   
   // Get sorted columns
   const sortedColumns = context.parameters.dataset.sorting;
   ```
3. 使用之前* * * *   ```typescript
   // Retrieve records
   context.webAPI.retrieveMultipleRecords("account", "?$select=name");
   
   // Create record
   context.webAPI.createRecord("contact", data);
   ```
4. * * * *设备功能   ```typescript
   // Capture image
   context.device.captureImage();
   
   // Get current position
   context.device.getCurrentPosition();
   ```
5. * * * *格式   ```typescript
   // Format date
   context.formatting.formatDateLong(date);
   
   // Format number
   context.formatting.formatDecimal(value);
   ```
最佳实践

1. **类型安全：使用TypeScript进行类型检查和智能感知
2. **Null检查**：在访问API对象之前总是检查null/undefined3. **错误处理**：在try-catch块中包装API调用
4. **平台检测**：检查`context.client.getFormFactor()`以适应行为
5. **API可用性**：在使用前验证您的目标平台的API可用性
6. **性能**：在适当的时候缓存API结果以避免重复调用

##其他资源

有关每个API的详细文档，请参阅[Power Apps组件框架API参考]（https://learn.microsoft.com/power-apps/developer/component-framework/reference/）
每个API的示例代码都可以在[PowerApps-Samples库]（https://github.com/microsoft/PowerApps-Samples/tree/master/component-framework）中找到。