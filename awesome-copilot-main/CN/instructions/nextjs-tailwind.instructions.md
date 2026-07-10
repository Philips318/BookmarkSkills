---
description: 'Next.js + Tailwind development standards and instructions'
applyTo: '**/*.tsx, **/*.ts, **/*.jsx, **/*.js, **/*.css'
---
#Next.js+顺风开发说明

使用顺风CSS样式和TypeScript的高质量Next.js应用的说明。

##项目背景

最新Next.js（应用路由器）
- TypeScript用于类型安全
-顺风CSS样式

##开发标准

# # #架构
-带有服务器和客户端组件的应用路由器
—按feature/domain分组路由
—合理设置错误边界
—默认使用React Server Components
-尽可能利用静态优化

# # #打印稿
-启用严格模式
-清晰的类型定义
-正确的错误处理与类型保护
-运行时类型验证

# # #样式
-顺风CSS与一致的调色板
-响应式设计模式
-暗模式支持
—遵循容器查询最佳实践
-维护语义HTML结构状态管理
- React Server Components用于服务器状态
-客户端状态的React钩子
-正确的加载和错误状态
-适当的乐观更新

###数据抓取
—用于直接查询数据库的服务器组件
-加载状态的反应悬念
—正确的错误处理和重试逻辑
—缓存失效策略

# # #安全
-输入验证和处理
-适当的身份验证检查
- CSRF保护
-速率限制实施
-安全API路由处理

# # #性能
-图像优化与next/image-字体优化next/font—路由预取
正确的代码分割
-包大小优化

##实施过程
1. 规划组件层次结构
2. 定义类型和接口
3. 实现服务器端逻辑
4. 构建客户端组件
5. 添加适当的错误处理
6. 实现响应式样式
7. 添加加载状态
8. 编写测试