---
description: 'Guidelines and best practices for developing Lightning Web Components (LWC) on Salesforce Platform.'
applyTo: 'force-app/main/default/lwc/**'
---
# LWC发展

##一般使用说明

-每个LWC应该位于`force-app/main/default/lwc/`下的自己的文件夹中。
-文件夹名称应与组件名称匹配（例如，`myComponent`组件的`myComponent`文件夹）。
—每个组件文件夹应包含以下文件：    - `myComponent.html`: The HTML template file.
    - `myComponent.js`: The JavaScript controller file.
    - `myComponent.js-meta.xml`: The metadata configuration file.
    - Optional: `myComponent.css` for component-specific styles.
    - Optional: `myComponent.test.js` for Jest unit tests.
##核心原则

# # # 1。在HTML标签上使用闪电组件
总是喜欢闪电Web组件库组件，而不是普通的HTML元素，以获得一致性、可访问性和未来的保障。

####推荐方法```html
<!-- Use Lightning components -->
<lightning-button label="Save" variant="brand" onclick={handleSave}></lightning-button>
<lightning-input type="text" label="Name" value={name} onchange={handleNameChange}></lightning-input>
<lightning-combobox label="Type" options={typeOptions} value={selectedType}></lightning-combobox>
<lightning-radio-group name="duration" label="Duration" options={durationOptions} value={duration} type="radio"></lightning-radio-group>
```
####避免使用纯HTML```html
<!-- Avoid these -->
<button onclick={handleSave}>Save</button>
<input type="text" onchange={handleNameChange} />
<select onchange={handleTypeChange}>
    <option value="option1">Option 1</option>
</select>
```
# # # 2。雷电组件映射指南

| HTML元素|闪电组件|关键属性||--------------|-------------------|----------------|
|`<button>`|`<lightning-button>`|`variant`,`label`,`icon-name`|
|`<input>`|`<lightning-input>`|`type`,`label`,`variant`|
|`<select>`|`<lightning-combobox>`|`options`,`value`,`placeholder`|
|`<textarea>`|`<lightning-textarea>`|`label`,`max-length`|
|`<input type="checkbox">`|`<lightning-input type="checkbox">`|`checked`,`label`|
|`<input type="radio">`|`<lightning-radio-group>`|`options`,`type`,`name`|
|`<input type="toggle">`|`<lightning-input type="toggle">`|`checked`,`variant`|
|定制药丸|`<lightning-pill>`|`label`，`name`,`onremove`|
|图标|`<lightning-icon>`|`icon-name`，`size`,`variant`|

# # # 3。雷电设计系统合规性

####使用SLDS实用程序类
对于现代实现，始终使用带有`slds-var-`前缀的Salesforce闪电设计系统实用程序类：```html
<!-- Spacing -->
<div class="slds-var-m-around_medium slds-var-p-top_large">
    <div class="slds-var-m-bottom_small">Content</div>
</div>

<!-- Layout -->
<div class="slds-grid slds-wrap slds-gutters_small">
    <div class="slds-col slds-size_1-of-2 slds-medium-size_1-of-3">
        <!-- Content -->
    </div>
</div>

<!-- Typography -->
<h2 class="slds-text-heading_medium slds-var-m-bottom_small">Section Title</h2>
<p class="slds-text-body_regular">Description text</p>
```
#### SLDS组件模式```html
<!-- Card Layout -->
<article class="slds-card slds-var-m-around_medium">
    <header class="slds-card__header">
        <h2 class="slds-text-heading_small">Card Title</h2>
    </header>
    <div class="slds-card__body slds-card__body_inner">
        <!-- Card content -->
    </div>
    <footer class="slds-card__footer">
        <!-- Card actions -->
    </footer>
</article>

<!-- Form Layout -->
<div class="slds-form slds-form_stacked">
    <div class="slds-form-element">
        <lightning-input label="Field Label" value={fieldValue}></lightning-input>
    </div>
</div>
```
# # # 4。避免自定义CSS

####使用SLDS类```html
<!-- Color and theming -->
<div class="slds-theme_success slds-text-color_inverse slds-var-p-around_small">
    Success message
</div>

<div class="slds-theme_error slds-text-color_inverse slds-var-p-around_small">
    Error message
</div>

<div class="slds-theme_warning slds-text-color_inverse slds-var-p-around_small">
    Warning message
</div>
```
####避免自定义CSS（反模式）```css
/* Don't create custom styles that override SLDS */
.custom-button {
    background-color: red;
    padding: 10px;
}

.my-special-layout {
    display: flex;
    justify-content: center;
}
```
####当自定义CSS是必要的
如果你必须使用自定义CSS，请遵循以下准则：
-尽可能使用CSS自定义属性（设计令牌）
—自定义类的前缀，避免冲突
永远不要覆盖SLDS基类```css
/* Custom CSS example */
.my-component-special {
    border-radius: var(--lwc-borderRadiusMedium);
    box-shadow: var(--lwc-shadowButton);
}
```
# # # 5。组件体系结构最佳实践

####反应性质```javascript
import { LightningElement, track, api } from 'lwc';

export default class MyComponent extends LightningElement {
    // Use @api for public properties
    @api recordId;
    @api title;

    // Primitive properties (string, number, boolean) are automatically reactive
    // No decorator needed - reassignment triggers re-render
    simpleValue = 'initial';
    count = 0;

    // Computed properties
    get displayName() {
        return this.name ? `Hello, ${this.name}` : 'Hello, Guest';
    }

    // @track is NOT needed for simple property reassignment
    // This will trigger reactivity automatically:
    handleUpdate() {
        this.simpleValue = 'updated'; // Reactive without @track
        this.count++; // Reactive without @track
    }

    // @track IS needed when mutating nested properties without reassignment
    @track complexData = {
        user: {
            name: 'John',
            preferences: {
                theme: 'dark'
            }
        }
    };

    handleDeepUpdate() {
        // Requires @track because we're mutating a nested property
        this.complexData.user.preferences.theme = 'light';
    }

    // BETTER: Avoid @track by using immutable patterns
    regularData = {
        user: {
            name: 'John',
            preferences: {
                theme: 'dark'
            }
        }
    };

    handleImmutableUpdate() {
      // No @track needed - we're creating a new object reference
      this.regularData = {
        ...this.regularData,
        user: {
          ...this.regularData.user,
          preferences: {
            ...this.regularData.user.preferences,
            theme: 'light'
          }
        }
      };
    }

    // Arrays: @track is needed only for mutating methods
    @track items = ['a', 'b', 'c'];

    handleArrayMutation() {
      // Requires @track
      this.items.push('d');
      this.items[0] = 'z';
    }

    // BETTER: Use immutable array operations
    regularItems = ['a', 'b', 'c'];

    handleImmutableArray() {
      // No @track needed
      this.regularItems = [...this.regularItems, 'd'];
      this.regularItems = this.regularItems.map((item, idx) =>
        idx === 0 ? 'z' : item
      );
    }

    // Use @track only for complex objects/arrays when you mutate nested properties.
    // For example, updating complexObject.details.status without reassigning complexObject.
    @track complexObject = {
      details: {
        status: 'new'
      }
    };
}
```
####事件处理模式```javascript
// Custom event dispatch
handleSave() {
    const saveEvent = new CustomEvent('save', {
        detail: {
            recordData: this.recordData,
            timestamp: new Date()
        }
    });
    this.dispatchEvent(saveEvent);
}

// Lightning component event handling
handleInputChange(event) {
    const fieldName = event.target.name;
    const fieldValue = event.target.value;

    // For lightning-input, lightning-combobox, etc.
    this[fieldName] = fieldValue;
}

handleRadioChange(event) {
    // For lightning-radio-group
    this.selectedValue = event.detail.value;
}

handleToggleChange(event) {
    // For lightning-input type="toggle"
    this.isToggled = event.detail.checked;
}
```
# # # 6。数据处理和通信服务

####使用@wire进行数据访问```javascript
import { getRecord } from 'lightning/uiRecordApi';
import { getObjectInfo } from 'lightning/uiObjectInfoApi';

const FIELDS = ['Account.Name', 'Account.Industry', 'Account.AnnualRevenue'];

export default class MyComponent extends LightningElement {
    @api recordId;

    @wire(getRecord, { recordId: '$recordId', fields: FIELDS })
    record;

    @wire(getObjectInfo, { objectApiName: 'Account' })
    objectInfo;

    get recordData() {
        return this.record.data ? this.record.data.fields : {};
    }
}
```
# # # 7。错误处理和用户体验

####实现适当的错误边界```javascript
import { ShowToastEvent } from 'lightning/platformShowToastEvent';

export default class MyComponent extends LightningElement {
    isLoading = false;
    error = null;

    async handleAsyncOperation() {
        this.isLoading = true;
        this.error = null;

        try {
            const result = await this.performOperation();
            this.showSuccessToast();
        } catch (error) {
            this.error = error;
            this.showErrorToast(error.body?.message || 'An error occurred');
        } finally {
            this.isLoading = false;
        }
    }

    performOperation() {
        // Developer-defined async operation
    }

    showSuccessToast() {
        const event = new ShowToastEvent({
            title: 'Success',
            message: 'Operation completed successfully',
            variant: 'success'
        });
        this.dispatchEvent(event);
    }

    showErrorToast(message) {
        const event = new ShowToastEvent({
            title: 'Error',
            message: message,
            variant: 'error',
            mode: 'sticky'
        });
        this.dispatchEvent(event);
    }
}
```
# # # 8。性能优化

####条件渲染
对于条件渲染，首选`lwc:if`、`lwc:elseif`和`lwc:else`（API v58.0+）。仍然支持传统的`if:true`/`if:false`，但应避免在新组件中使用。```html
<!-- Use template directives for conditional rendering -->
<template lwc:if={isLoading}>
    <lightning-spinner alternative-text="Loading..."></lightning-spinner>
</template>
<template lwc:elseif={error}>
    <div class="slds-theme_error slds-text-color_inverse slds-var-p-around_small">
        {error.message}
    </div>
</template>
<template lwc:else>
    <template for:each={items} for:item="item">
        <div key={item.id} class="slds-var-m-bottom_small">
            {item.name}
        </div>
    </template>
</template>
```

```html
<!-- Legacy approach (avoid in new components) -->
<template if:true={isLoading}>
    <lightning-spinner alternative-text="Loading..."></lightning-spinner>
</template>
<template if:true={error}>
    <div class="slds-theme_error slds-text-color_inverse slds-var-p-around_small">
        {error.message}
    </div>
</template>
<template if:false={isLoading}>
  <template if:false={error}>
    <template for:each={items} for:item="item">
        <div key={item.id} class="slds-var-m-bottom_small">
            {item.name}
        </div>
    </template>
  </template>
</template>
```
# # # 9。可访问性最佳实践

####使用正确的ARIA标签和语义HTML```html
<!-- Use semantic structure -->
<section aria-label="Product Selection">
    <h2 class="slds-text-heading_medium">Products</h2>

    <lightning-input
        type="search"
        label="Search Products"
        placeholder="Enter product name..."
        aria-describedby="search-help">
    </lightning-input>

    <div id="search-help" class="slds-assistive-text">
        Type to filter the product list
    </div>
</section>
```
要避免的常见反模式
**直接DOM操作**：永远不要使用`document.querySelector()`或类似的
- **jQuery或外部库**：避免非闪电兼容库
**内联样式**：使用SLDS类代替`style`属性
- **全局CSS**：所有样式都应该限定在组件的范围内
—**硬编码值**：使用自定义标签、自定义元数据或常量
- **命令式API调用**：尽可能使用`@wire`而不是命令式`import`调用
- **内存泄漏**：总是清理`disconnectedCallback()`中的事件侦听器