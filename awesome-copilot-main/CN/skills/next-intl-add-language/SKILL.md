---
name: next-intl-add-language
description: 'Add new language to a Next.js + next-intl application'
---
这是一个使用next-intl进行国际化向Next.js项目添加新语言的指南。

—对于i18n，应用程序使用next-intl。
—所有翻译文件都在`./messages`目录下。
—UI组件为`src/components/language-toggle.tsx`。
-路由和中间件配置处理在：  - `src/i18n/routing.ts`
  - `src/middleware.ts`
添加新语言时：

—将“`en.json`”文件中的所有内容翻译成新的语言。我们的目标是让所有JSON条目都变成新语言，以便进行完整的翻译。
—在“`routing.ts`”和“`middleware.ts`”中添加路径。
—添加语言到`language-toggle.tsx`。