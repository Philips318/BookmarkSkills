---
name: web-design-reviewer
description: 'This skill enables visual inspection of websites running locally or remotely to identify and fix design issues. Triggers on requests like "review website design", "check the UI", "fix the layout", "find design problems". Detects issues with responsive design, accessibility, visual consistency, and layout breakage, then performs fixes at the source code level.'
---
#网页设计审稿人

这一技能可以使网站设计质量的视觉检查和验证，在源代码级别识别和修复问题。

##适用范围

-静态站点（HTML/CSS/JS）
- SPA框架，如React / Vue / Angular / Svelte
-全栈框架，如Next.js/ next / SvelteKit
- CMS平台，如WordPress / Drupal
-任何其他web应用程序

# #先决条件

# # #要求

1. **目标网站必须运行**
-本地开发服务器（如`http://localhost:3000`）
-暂存环境
-生产环境（只读审查）

2. **浏览器自动化必须可用**
-截图
-页面导航
- DOM信息检索

3. **访问源代码（在进行修复时）**
-项目必须存在于工作区内

##工作流概述```mermaid
flowchart TD
    A[Step 1: Information Gathering] --> B[Step 2: Visual Inspection]
    B --> C[Step 3: Issue Fixing]
    C --> D[Step 4: Re-verification]
    D --> E{Issues Remaining?}
    E -->|Yes| B
    E -->|No| F[Completion Report]
```
---

步骤1：信息收集阶段

1.1 URL确认

如果没有提供URL，请询问用户：

>请提供要审核的网站网址（例如：`http://localhost:3000`）

### 1.2理解项目结构

在进行修复时，收集以下信息：

|项目|示例问题||------|------------------|
你是否使用React / Vue /Next.js等？|
| CSS / SCSS / Tailwind / CSS-in- js等
样式文件和组件位于哪里？|
|仅针对特定页面还是整个网站？|

1.3自动项目检测

尝试从工作区中的文件进行自动检测：```
Detection targets:
├── package.json     → Framework and dependencies
├── tsconfig.json    → TypeScript usage
├── tailwind.config  → Tailwind CSS
├── next.config      → Next.js
├── vite.config      → Vite
├── nuxt.config      → Nuxt
└── src/ or app/     → Source directory
```
1.4识别样式方法

|检测|编辑目标||--------|-----------|-------------|
|纯CSS |`*.css`文件|全局CSS或组件CSS |
|SCSS/Sass|`*.scss`，`*.sass`| SCSS文件|
| CSS模块|`*.module.css`|模块CSS文件|
|顺风CSS |`tailwind.config.*`|组件中的className |
| style -components |`styled.`在代码|JS/TS文件|
|`@emotion/`导入|JS/TS文件|
|内联样式|JS/TS文件|

---

步骤2：目视检查阶段

### 2.1页面遍历

1. 导航到指定的URL
2. 捕捉屏幕截图
3. 检索DOMstructure/snapshot（如果可能）
4. 如果存在其他页面，则遍历导航

2.2检查项目

####布局问题

|事件类型|描述|级别||-------|-------------|----------|
|元素溢出|内容从父元素或视图溢出|高|
|元素重叠|元素意外重叠|高|
|对齐问题|网格或弯曲对齐问题|中等|
|间距不一致|Padding/margin不一致|中等|
|文本剪辑|长文本处理不正确|中等|

####响应性问题

|事件类型|描述|级别||-------|-------------|----------|
|非移动友好|小屏幕布局中断|高|
|断点问题|不自然的过渡时，屏幕大小的变化|中等|
|触摸目标|移动设备上按钮太小|中等|

####无障碍问题

|事件类型|描述|级别||-------|-------------|----------|
|对比度不足|文本和背景对比度低|高|
|无焦点状态|键盘导航时无法判断状态|高|
|缺少所有文本|图片没有替代文本|中等|

####视觉一致性

|事件类型|描述|级别||-------|-------------|----------|
|字体不一致|混合字体系列|中等|
|颜色不一致|品牌颜色不统一|中等|
|间距不一致|相似元素间距不均匀|低|

2.3视口测试（响应式）

在以下视图中测试：

|名称|宽度|代表设备||------|-------|----------------------|
|手机| 375px | iPhoneSE/12迷你|
|平板电脑| 768px | iPad |
|桌面| 1280px |标准PC |
|宽| 1920px |大显示|

---

步骤3：问题修复阶段

问题优先级```mermaid
block-beta
    columns 1
    block:priority["Priority Matrix"]
        P1["P1: Fix Immediately\n(Layout issues affecting functionality)"]
        P2["P2: Fix Next\n(Visual issues degrading UX)"]
        P3["P3: Fix If Possible\n(Minor visual inconsistencies)"]
    end
```
3.2识别源文件

从有问题的元素中识别源文件：

1. * * * * Selector-based搜索
-按类名或类ID搜索代码库
-使用`grep_search`探索样式定义

2. * * * *基于组件的搜索
-从元素文本或结构中识别组件
-使用`semantic_search`浏览相关文件

3. **文件模式过滤**   ```
   Style files: src/**/*.css, styles/**/*
   Components: src/components/**/*
   Pages: src/pages/**, app/**
   ```
3.3应用修复

####特定于框架的修复指南

参见[references/framework-fixes.md]（references/framework-fixes.md）了解详细信息。

####修复原则

1. **最小更改**：只做必要的最小更改来解决问题
2. **尊重现有模式**：遵循项目中现有的代码风格
3. **避免破坏更改**：注意不要影响其他区域
4. **添加注释：在适当的地方添加注释来解释修复的原因

---

步骤4：重新验证阶段

### 4.1修复后确认

1. 重新加载浏览器（或等待开发服务器HMR）
2. 捕获固定区域的屏幕截图
3. 前后比较

4.2回归测试

-验证修复没有影响到其他区域
-确认响应显示没有损坏

4.3迭代决策```mermaid
flowchart TD
    A{Issues Remaining?}
    A -->|Yes| B[Return to Step 2]
    A -->|No| C[Proceed to Completion Report]
```
**迭代限制**：如果特定问题需要超过3次修复尝试，请咨询用户

---

##输出格式

评审结果报告```markdown
# Web Design Review Results

## Summary

| Item | Value |
|------|-------|
| Target URL | {URL} |
| Framework | {Detected framework} |
| Styling | {CSS / Tailwind / etc.} |
| Tested Viewports | Desktop, Mobile |
| Issues Detected | {N} |
| Issues Fixed | {M} |

## Detected Issues

### [P1] {Issue Title}

- **Page**: {Page path}
- **Element**: {Selector or description}
- **Issue**: {Detailed description of the issue}
- **Fixed File**: `{File path}`
- **Fix Details**: {Description of changes}
- **Screenshot**: Before/After

### [P2] {Issue Title}
...

## Unfixed Issues (if any)

### {Issue Title}
- **Reason**: {Why it was not fixed/could not be fixed}
- **Recommended Action**: {Recommendations for user}

## Recommendations

- {Suggestions for future improvements}
```
---

##所需功能

|功能|描述|必选||------------|-------------|----------|
|网页导航|访问url，页面转换|✅|
|截图抓取|页面图片抓取|✅|
|图像分析|视觉问题检测|✅|
| DOM检索|页面结构检索|推荐|
|文件Read/Write|源代码读取和编辑|修复|所需
|代码搜索|代码搜索项目|需要修复|

---

##参考实现

###剧作家MCP的执行

推荐使用[剧作家MCP]（https://github.com/microsoft/playwright-mcp）作为此技能的参考实现。

|功能|剧作家MCP工具|用途||------------|---------------------|---------|
|导航|`browser_navigate`|访问url |
|快照|`browser_snapshot`|检索DOM结构|
|截图|`browser_take_screenshot`|目视巡检图片|
|点击|`browser_click`|与交互元素|交互
|调整|`browser_resize`|响应测试|
|控制台|`browser_console_messages`|检测JS错误|

####配置举例（MCP服务器）```json
{
  "mcpServers": {
    "playwright": {
      "command": "npx",
      "args": ["-y", "@playwright/mcp@latest", "--caps=vision"]
    }
  }
}
```
其他兼容的浏览器自动化工具

|工具|特性||------|----------|
|硒|广泛的浏览器支持，多语言支持|
|木偶师|Chrome/Chromium专注，Node.js|
|柏树|与E2E测试|轻松集成
| WebDriver BiDi |下一代标准化协议|

使用这些工具可以实现相同的工作流。只要它们提供必要的功能（导航、屏幕截图、DOM检索），工具的选择是灵活的。

---

最佳实践

### DO（推荐）

-✅在进行修复之前总是保存屏幕截图
-✅每次修复一个问题并验证每个问题
-✅遵循项目现有的代码风格
-✅重大变更前请与用户确认
-✅文档修复细节彻底

###不要（不推荐）

-❌没有确认的大规模重构
-❌忽视设计系统或品牌准则
-❌修复忽略性能
-❌一次修复多个问题（难以验证）

---

# #故障排除问题：没有找到样式文件

1. 检查`package.json`中的依赖项
2. 考虑CSS-in-JS的可能性
3. 考虑在构建时生成的CSS
4. 询问用户的造型方法

###问题：修复未反映

1. 检查开发服务器HMR是否正常工作
2. 清除浏览器缓存
3. 如果项目需要构建，则重新构建
4. 检查CSS专用性问题

问题：修复影响其他区域

1. 回滚更改
2. 使用更具体的选择器
3. 考虑使用CSS模块或限定样式
4. 请咨询用户确认影响范围