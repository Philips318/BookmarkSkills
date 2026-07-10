---
description: 'Guidelines for building TanStack Start applications'
applyTo: '**/*.ts, **/*.tsx, **/*.js, **/*.jsx, **/*.css, **/*.scss, **/*.json'
---
# TanStack从Shadcn/ui开发指南开始

你是一名专业的TypeScript开发人员，擅长使用现代React模式开发TanStack Start应用程序。

##技术栈
- TypeScript（严格模式）
- TanStack启动（路由和SSR）
-Shadcn/ui（UI组件）
-顺风CSS（样式）
- Zod（验证）
- TanStack查询（客户端状态）

代码风格规则

-永远不要使用`any`类型-始终使用正确的TypeScript类型
-更喜欢函数组件而不是类组件
-始终使用Zod模式验证外部数据
-包括所有路由的错误和挂起边界
-遵循ARIA属性的可访问性最佳实践

组件模式

使用带有正确TypeScript接口的函数组件：```typescript
interface ButtonProps {
  children: React.ReactNode;
  onClick: () => void;
  variant?: 'primary' | 'secondary';
}

export default function Button({ children, onClick, variant = 'primary' }: ButtonProps) {
  return (
    <button onClick={onClick} className={cn(buttonVariants({ variant }))}>
      {children}
    </button>
  );
}
```
##数据提取

使用路由加载器：
-渲染所需的初始页面数据
- SSR要求
- seo关键数据

使用React Query：
-频繁更新数据
—Optional/secondarydata
-客户端突变与乐观更新```typescript
// Route Loader
export const Route = createFileRoute('/users')({
  loader: async () => {
    const users = await fetchUsers()
    return { users: userListSchema.parse(users) }
  },
  component: UserList,
})

// React Query
const { data: stats } = useQuery({
  queryKey: ['user-stats', userId],
  queryFn: () => fetchUserStats(userId),
  refetchInterval: 30000,
});
```
##验证

始终验证外部数据。在`src/lib/schemas.ts`中定义模式：```typescript
export const userSchema = z.object({
  id: z.string(),
  name: z.string().min(1).max(100),
  email: z.string().email().optional(),
  role: z.enum(['admin', 'user']).default('user'),
})

export type User = z.infer<typeof userSchema>

// Safe parsing
const result = userSchema.safeParse(data)
if (!result.success) {
  console.error('Validation failed:', result.error.format())
  return null
}
```
# #路线

使用基于文件的路由结构`src/routes/`中的路由。始终包含错误和挂起边界：```typescript
export const Route = createFileRoute('/users/$id')({
  loader: async ({ params }) => {
    const user = await fetchUser(params.id);
    return { user: userSchema.parse(user) };
  },
  component: UserDetail,
  errorBoundary: ({ error }) => (
    <div className="text-red-600 p-4">Error: {error.message}</div>
  ),
  pendingBoundary: () => (
    <div className="flex items-center justify-center p-4">
      <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-primary" />
    </div>
  ),
});
```
## UI组件

总是选择Shadcn/ui组件，而不是自定义组件：```typescript
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';

<Card>
  <CardHeader>
    <CardTitle>User Details</CardTitle>
  </CardHeader>
  <CardContent>
    <Button onClick={handleSave}>Save</Button>
  </CardContent>
</Card>
```
在响应式设计中使用顺风进行样式设计：```typescript
<div className="flex flex-col gap-4 p-6 md:flex-row md:gap-6">
  <Button className="w-full md:w-auto">Action</Button>
</div>
```
# #可访问性

首先使用语义HTML。只有在语义不存在时才添加ARIA：```typescript
// ✅ Good: Semantic HTML with minimal ARIA
<button onClick={toggleMenu}>
  <MenuIcon aria-hidden="true" />
  <span className="sr-only">Toggle Menu</span>
</button>

// ✅ Good: ARIA only when needed (for dynamic states)
<button
  aria-expanded={isOpen}
  aria-controls="menu"
  onClick={toggleMenu}
>
  Menu
</button>

// ✅ Good: Semantic form elements
<label htmlFor="email">Email Address</label>
<input id="email" type="email" />
{errors.email && (
  <p role="alert">{errors.email}</p>
)}
```
##文件组织```
src/
├── components/ui/    # Shadcn/ui components
├── lib/schemas.ts    # Zod schemas
├── routes/          # File-based routes
└── routes/api/      # Server routes (.ts)
```
##进口标准

对所有内部导入使用`@/`别名：```typescript
// ✅ Good
import { Button } from '@/components/ui/button'
import { userSchema } from '@/lib/schemas'

// ❌ Bad
import { Button } from '../components/ui/button'
```
##添加组件

根据需要安装Shadcn组件：```bash
npx shadcn@latest add button card input dialog
```
##常见模式

-始终使用Zod验证外部数据
-对初始数据使用路由加载器，更新使用React Query
—在所有路由上包含error/pending边界
-更喜欢Shadcn组件自定义UI
—始终使用`@/`导入
-遵循可访问性最佳实践