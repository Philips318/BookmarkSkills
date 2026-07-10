# TypeScript和SCSS规则参考

## TypeScript规则

—不要使用“`any`”。  ```ts
  // Bad
  const handler = (e: any) => {};

  // Good
  const handler = (e: React.ChangeEvent<HTMLInputElement>) => {};
  ```
-使用`type`的道具，而不是`interface`。  ```ts
  // Bad
  interface ButtonProps {
    label: string;
  }

  // Good
  type ButtonProps = { label: string };
  ```
-显式注释函数返回类型。  ```ts
  // Bad
  const getLabel = () => "hello";

  // Good
  const getLabel = (): string => "hello";
  ```
SCSS规则

# # #令牌

—使用`src/styles/theme.scss`中的颜色变量。
-使用`src/styles/animation.scss`中的动画变量。
-在`src/styles/z-index.scss`中定义z-index标记，并在组件样式中使用这些标记。
—在组件SCSS中，不要硬编码z-index值（例如，避免使用`z-index: 10;`，而是使用z-index令牌）。

样式约束

-优先选择Mantine或其他UI库；只有在需要补充库样式时才使用SCSS。
-不要使用负边距。
-首选无单位`line-height`。
—在`em`中首选`letter-spacing`。
—当需要保证金时，只允许使用`margin-top`和`margin-left`。
—“`margin`”和“`position`”不能配置在根元素上。
—“`src/styles/z-index.scss`”中的数值必须遵循50步制（100,150，…）。