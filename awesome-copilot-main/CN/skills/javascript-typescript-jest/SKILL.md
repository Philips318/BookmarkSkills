---
name: javascript-typescript-jest
description: 'Best practices for writing JavaScript/TypeScript tests using Jest, including mocking strategies, test structure, and common patterns.'
---
测试结构
—以`.test.ts`或`.test.js`后缀命名测试文件
-将测试文件放在要测试的代码旁边，或者放在专用的`__tests__`目录中
-使用描述性的测试名称来解释预期的行为
-使用嵌套描述块组织相关测试
—按照格式：`describe('Component/Function/Class', () => { it('should do something', () => {}) })`有效的嘲讽
-模拟外部依赖项（api、数据库等）以隔离测试
-使用`jest.mock()`模块级模拟
—使用`jest.spyOn()`进行特定的函数模拟
—使用`mockImplementation()`或`mockReturnValue()`定义模拟行为
-在`afterEach`中使用`jest.resetAllMocks()`重置测试之间的模拟

测试异步代码
-在测试中始终返回承诺或使用async/await语法
-使用`resolves`/`rejects`匹配器的承诺
—使用`jest.setTimeout()`为慢速测试设置适当的超时###快照测试
-对更改不频繁的UI组件或复杂对象使用快照测试
—保持快照小而集中
—提交前仔细检查快照更改

测试React组件
-使用React Testing Library over Enzyme来测试组件
-测试用户行为和组件可访问性
—根据可访问性角色、标签或文本内容查询元素
-使用`userEvent`超过`fireEvent`更现实的用户交互

## Common Jest Matchers
—基本版：`expect(value).toBe(expected)`、`expect(value).toEqual(expected)`-真实度：`expect(value).toBeTruthy()`，`expect(value).toBeFalsy()`—编号：`expect(value).toBeGreaterThan(3)`、`expect(value).toBeLessThanOrEqual(3)`—字符串：`expect(value).toMatch(/pattern/)`、`expect(value).toContain('substring')`—阵列：`expect(array).toContain(item)`、`expect(array).toHaveLength(3)`—对象：`expect(object).toHaveProperty('key', value)`—例外：`expect(fn).toThrow()`、`expect(fn).toThrow(Error)`—模拟函数：`expect(mockFn).toHaveBeenCalled()`、`expect(mockFn).toHaveBeenCalledWith(arg1, arg2)`