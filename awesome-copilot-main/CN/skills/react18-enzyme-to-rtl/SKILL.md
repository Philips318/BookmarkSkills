---
name: react18-enzyme-to-rtl
description: 'Provides exact Enzyme → React Testing Library migration patterns for React 18 upgrades. Use this skill whenever Enzyme tests need to be rewritten - shallow, mount, wrapper.find(), wrapper.simulate(), wrapper.prop(), wrapper.state(), wrapper.instance(), Enzyme configure/Adapter calls, or any test file that imports from enzyme. This skill covers the full API mapping and the philosophy shift from implementation testing to behavior testing. Always read this skill before rewriting Enzyme tests - do not translate Enzyme APIs 1:1, that produces brittle RTL tests.'
---
# React 18酶→RTL迁移

Enzyme没有React 18适配器，也没有React 18支持路径。必须使用React测试库重写所有酵素测试。

哲学的转变（先读这篇文章）

酶测试实现。RTL测试行为。```jsx
// Enzyme: tests that the component has the right internal state
expect(wrapper.state('count')).toBe(3);
expect(wrapper.instance().handleClick).toBeDefined();
expect(wrapper.find('Button').prop('disabled')).toBe(true);

// RTL: tests what the user actually sees and can do
expect(screen.getByText('Count: 3')).toBeInTheDocument();
expect(screen.getByRole('button', { name: /submit/i })).toBeDisabled();
```
这不是1:1的翻译。验证内部状态或实例方法的酶测试没有RTL的等价物——因为RTL有意不暴露内部。**重写测试以断言可见的结果

## API Map

对于每个酶API的完整before/after代码，请阅读：
- **`references/enzyme-api-map.md`** -全映射：浅映射，mount, find, simulation, prop, state, instance, configure
- **`references/async-patterns.md`** - waitFor, findBy, act(), Apollo MockedProvider，加载状态，错误状态

核心重写模板```jsx
// Every Enzyme test rewrites to this shape:
import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import MyComponent from './MyComponent';

describe('MyComponent', () => {
  it('does the thing', async () => {
    // 1. Render (replaces shallow/mount)
    render(<MyComponent prop="value" />);

    // 2. Query (replaces wrapper.find())
    const button = screen.getByRole('button', { name: /submit/i });

    // 3. Interact (replaces simulate())
    await userEvent.setup().click(button);

    // 4. Assert on visible output (replaces wrapper.state() / wrapper.prop())
    expect(screen.getByText('Submitted!')).toBeInTheDocument();
  });
});
```
RTL查询优先级（按此顺序使用）

1.`getByRole`-匹配可访问的角色（按钮，文本框，标题，复选框等）
2.`getByLabelText`-链接到标签的表单字段
3.`getByPlaceholderText`-输入占位符
4.`getByText`-可见文本内容
5.`getByDisplayValue`-input/select/textarea的电流值
6.`getByAltText`- image Alt文本
7.`getByTitle`-标题属性
8.`getByTestId`-`data-testid`属性（最后一招）`getByRole`优于`getByTestId`。它也测试可访问性。

##使用提供者包装```jsx
// Enzyme with context:
const wrapper = mount(
  <ApolloProvider client={client}>
    <ThemeProvider theme={theme}>
      <MyComponent />
    </ThemeProvider>
  </ApolloProvider>
);

// RTL equivalent (use your project's customRender or wrap inline):
import { render } from '@testing-library/react';
render(
  <MockedProvider mocks={mocks} addTypename={false}>
    <ThemeProvider theme={theme}>
      <MyComponent />
    </ThemeProvider>
  </MockedProvider>
);
// Or use the project's customRender helper if it wraps providers
```
