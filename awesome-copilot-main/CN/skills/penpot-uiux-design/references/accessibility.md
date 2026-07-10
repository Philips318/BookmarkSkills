#无障碍指引参考（WCAG）

##快速合规检查表

AA级要求（最低标准）

-[]正常文本的颜色对比度为4.5:1
-[]大字体（18px+或14px粗体）颜色对比度3:1
-[]触摸目标最小44×44px
-[]所有功能可通过键盘
-[]可见焦点指示器
—[]内容闪烁不超过3个times/second-[]页面有描述性标题
-[]链接目的从文本中明确
—[]表单输入有标签
—[]错误信息是描述性的

---

颜色和对比度

对比度

|元件|最小比值|增强（AAA） || ------- | ------------- | -------------- |
|正文| 4.5:1 | 7:1 |
|大字（18px+） | 3:1 | 4.5:1 |
| UI组件| 3:1 | - |
|图形对象| 3:1 | - |

颜色独立性

永远不要把颜色作为传达信息的唯一手段；```text
✗ Error fields shown only in red
✓ Error fields with red border + error icon + text message

✗ Required fields marked only with red asterisk
✓ Required fields labeled "(required)" or with icon + tooltip

✗ Status shown only by color dots
✓ Status with color + icon + label text

```
可访问的颜色组合

**安全的背景文字颜色：**

|背景|文字颜色|对比度|| ---------- | ---------- | -------- |
|白色（#FFFFFF） |深灰色（#1F2937） | 15.5:1✓|
|浅灰色（#F3F4F6） |深灰色（#374151）| 10.9:1✓|
|原色蓝（#2563EB） |白色（#FFFFFF） | 4.6:1✓|
|深色（#111827）|白色（#FFFFFF） | 18.1：✓|

**文本避免使用的颜色：**

-白色上的黄色（对比度不足）
-浅灰色的白色
-白色上的橙色（最多是边缘）

---

##键盘导航

# # #要求

1. **所有互动元素**必须通过Tab键访问
2. **逻辑标签顺序**以下的视觉布局
3. **没有键盘陷阱**（用户可以随时Tab离开）
4. **焦点可见**在键盘导航期间的任何时候
5. **跳过链接**绕过重复的导航

###焦点指标```css
/* Example focus styles */
:focus {
  outline: 2px solid #2563EB;
  outline-offset: 2px;
}

:focus:not(:focus-visible) {
  outline: none; /* Hide for mouse users */
}

:focus-visible {
  outline: 2px solid #2563EB;
  outline-offset: 2px;
}

```
键盘快捷键

|预期行为|| --- | ----------------- |
移动到下一个交互元素|
移动到上一个元素|
|输入| Activatebutton/link|
|空格|激活按钮，切换复选框|
| Escape | Closemodal/dropdown|
|方向键|导航组件|

---

屏幕阅读器支持

语义HTML元素

使用适当的元素来达到目的：

|用途|元素|不是本|| ------- | ------- | -------- |
|导航|`<nav>`|`<div class="nav">`|
|主要内容|`<main>`|`<div id="main">`|
|头|`<header>`|`<div class="header">`|
|页脚|`<footer>`|`<div class="footer">`|
|按钮|`<button>`|`<div onclick>`|
|链接|`<a href>`|`<span onclick>`|

### Heading层次```text
h1 - Page Title (one per page)
  h2 - Major Section
    h3 - Subsection
      h4 - Sub-subsection
    h3 - Another Subsection
  h2 - Another Major Section

```
**不要跳过关卡** （h1→h3，没有h2）

###图像Alt文本```text
Decorative: alt="" (empty, not omitted)
Informative: alt="Description of what image shows"
Functional: alt="Action the image performs"
Complex: alt="Brief description" + detailed description nearby

```
**Alt文本示例：**```text
✓ alt="Bar chart showing sales growth from $10M to $15M in Q4"
✓ alt="Company logo"
✓ alt="" (for decorative background pattern)

✗ alt="image" or alt="photo"
✗ alt="img_12345.jpg"
✗ Missing alt attribute entirely

```
---

触摸和指针

###选择目标尺寸

|平台|最低|建议|| -------- | ------- | ----------- |
| WCAG 2.1 | 44×44px | 48×48px |
| iOS (Apple) | 44×44pt | - |
| Android | 48×48dp | - |

触摸目标间距

-相邻目标之间最小8像素
-更喜欢16px+舒适
-主要行动的更大目标

指针手势

-复杂的手势必须有单指针替代
—拖动操作需要相同的单击操作
-避免触摸设备上的悬停功能

---

##表单可访问性

# # #的标签

每个输入都必须有一个相关的标签：```text
<label for="email">Email Address</label>
<input type="email" id="email" name="email">

```
必填字段```text
<!-- Announce to screen readers -->
<label for="name">
  Name <span aria-label="required">*</span>
</label>
<input type="text" id="name" required aria-required="true">

```
错误处理```text
<label for="email">Email</label>
<input type="email" id="email" aria-invalid="true" aria-describedby="email-error">
<span id="email-error" role="alert">
  Please enter a valid email address
</span>

```
表单说明

—输入前提供格式提示
—在出现错误前显示密码要求
—用fieldset/legend对相关字段进行分组

---

##动态内容

###激活区域

对于动态更新的内容：```text
aria-live="polite" - Announce when convenient
aria-live="assertive" - Announce immediately (interrupts)
role="alert" - Urgent messages (like assertive)
role="status" - Status updates (like polite)

```
###加载状态```text
<button aria-busy="true" aria-live="polite">
  <span class="spinner"></span>
  Loading...
</button>

```
模态对话框

-焦点打开时进入模态
-焦点被困在模态中
-转义键关闭模式
-关闭时焦点返回触发元素

---

测试可访问性

手动测试检查表

1. **仅键盘：**导航整个页面与Tab/Enter2. **屏幕阅读器：**测试VoiceOver （Mac）或NVDA （Windows）
3. **缩放200%:**内容仍然可读和可用
4. **高对比度：**用系统高对比度模式测试
5. **没有鼠标：**完成所有任务没有指向设备

自动化工具

-斧DevTools（浏览器扩展）
- WAVE （WebAIM浏览器扩展）
-灯塔（Chrome DevTools）
-颜色对比检查器（WebAIM，对比度）

###常见问题检查-[]缺少或空的alt文本
-[]空链接或按钮
-[]缺少表单标签
-[]颜色对比度不足
-[]缺少语言属性
-[]标题结构不正确
-[]缺少跳过导航链接
-[]不可访问的自定义小部件

---

ARIA快速参考

# # #的角色

|角色|用途|| ---- | ------- |
|`button`|可点击按钮|
|`link`|导航链接|
|`dialog`|模式对话框|
|`alert`|重要信息|
|`navigation`|导航区|
|`main`|主要内容|
|`search`|搜索功能|
|`tab/tablist/tabpanel`|标签接口|

# # #属性

|属性|用途|| -------- | ------- |
|`aria-label`|可访问名称|
|`aria-labelledby`|对标记元素|的引用
|`aria-describedby`|参考描述|
|`aria-hidden`|躲避辅助技术|
|`aria-expanded`|可扩展状态|
|`aria-selected`|选择状态|
|`aria-disabled`| |禁用状态
|`aria-required`|必选字段|
|`aria-invalid`|输入|无效

黄金法则

** ARIA的第一条规则：**如果原生HTML可以工作，不要使用ARIA。```text
✗ <div role="button" tabindex="0">Click</div>
✓ <button>Click</button>

```
