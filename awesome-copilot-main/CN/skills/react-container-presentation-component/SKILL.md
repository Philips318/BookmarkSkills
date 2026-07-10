---
name: react-container-presentation-component
description: "Create a React component using the Container/Presentation pattern in src/components by asking for the component name and type (ui or features), then scaffold files that follow this repository's TypeScript, Storybook, and SCSS conventions. Use when the user explicitly asks for a Container/Presentation-based component or runs /react-container-presentation-component."
argument-hint: "componentName type(ui|features)"
user-invocable: true
---
#Container/Presentation组件

使用该技能在`src/components`下创建一个遵循Container/Presentation模式的React组件。

有关详细规则，请参阅此技能的捆绑引用。- `references/component-architecture.md`
- `references/typescript-and-scss-rules.md`
如果`/react-container-presentation-component`输入不完整，请在创建文件之前先询问问题。

##何时使用

—当用户执行“`/react-container-presentation-component`”命令时
-当用户显式请求遵循Container/Presentation模式的React组件时
-当用户需要帮助决定或实现`ui`vs`features`分类在Container/Presentation模式

##必问问题

如果缺少以下任何信息，请询问使用`ask_user`的用户。

1. 组件名称
2. 类型（`ui`或`features`）
3. 是否替换已有组件（仅在创建`ui`时使用）

问题要求:

-提供类型作为选项（`ui`,`features`）
-组件名称必须是PascalCase格式
-对于`ui`，询问是否应该用新组件替换现有`features`中直接使用的Mantine或其他UI库

# #过程

1. 检查现有组件—查看`src/components/ui/<ComponentName>`或`src/components/features/<ComponentName>`是否已经存在。
—如果存在，不要覆盖；与用户确认首选的方法。

2. 确定目标目录

—`ui`:`src/components/ui/<ComponentName>`—`features`:`src/components/features/<ComponentName>`3. 重新检查分类（仅当指定`ui`时）

—即使指定了`ui`，在创建文件之前，请查看`references/component-architecture.md`中的`Reclassification Rule`。
-如果实现包括状态管理、副作用、异步处理、context/store更新或业务逻辑，则将其视为`features`。
-如果结果更接近`features`，则不按照`ui`进行；在继续之前，使用`ask_user`并确认以下操作之一。
——`Create as features`——`Keep ui and move state/logic to parent or features`4. 创建所需文件

—`ui`:`index.tsx`、`index.module.scss`、`index.stories.tsx`-`features`:`index.tsx`、`use<ComponentName>.tsx`、`presentation.tsx`、`types.ts`、`presentation.module.scss`、`presentation.stories.tsx`5. 替换现有的用法（仅在创建`ui`时）-只有当用户批准时，用新的`ui`组件替换现有`features`中使用Mantine或其他UI库的等效直接实现。

6. 验证

-运行build和lint命令，并确保两者都通过；如果新添加或更新的文件引入了问题，请修复它们。
-遵循`references/component-architecture.md`中的`Storybook Minimum`进行故事状态决策。
-通过`ask_user`询问用户是否运行Storybook检查（例如：“run”/“Skip for now”）。
—当用户选择“运行”时，执行`npm run storybook`。
-如果用户选择“暂时跳过”，在最终报告中明确提及跳过故事书的执行。

##输出合同—报告已创建的文件列表。
-如果进行了替换，报告更改的文件列表和替换的详细信息。
—提供一个创建组件的使用示例。
—报告是否执行了Storybook验证（run/skip），如果执行，包括使用的命令。
-解释该组件被分类为`ui`或`features`的原因。
-总结状态、副作用和渲染责任的位置。
-确认是否有任何依赖方向违规。
-清楚说明任何未解决的事项。