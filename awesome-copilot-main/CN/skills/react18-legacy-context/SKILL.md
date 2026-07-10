---
name: react18-legacy-context
description: 'Provides the complete migration pattern for React legacy context API (contextTypes, childContextTypes, getChildContext) to the modern createContext API. Use this skill whenever migrating legacy context in class components - this is always a cross-file migration requiring the provider AND all consumers to be updated together. Use it before touching any contextTypes or childContextTypes code, because migrating only the provider without the consumers (or vice versa) will cause a runtime failure. Always read this skill before writing any context migration - the cross-file coordination steps here prevent the most common context migration bugs.'
---
# React 18遗留上下文迁移

遗留上下文（`contextTypes`,`childContextTypes`,`getChildContext`）在React 16.3中已弃用，并在React 18.3.1中发出警告。它在React 19中被删除了。

##这总是一个跨文件迁移

与大多数其他一次只涉及一个文件的迁移不同，上下文迁移需要协调：
1. 创建上下文对象（通常是一个新文件）
2. 更新**provider**组件
3. 更新**每个消费者**组件

缺少任何消费者会使应用程序中断-它将从错误的上下文中读取或得到`undefined`。

##迁移步骤（始终遵循此顺序）```
Step 1: Find the provider (childContextTypes + getChildContext)
Step 2: Find ALL consumers (contextTypes)
Step 3: Create the context file
Step 4: Update the provider
Step 5: Update each consumer (class components → contextType, function components → useContext)
Step 6: Verify - run the app, check no legacy context warnings remain
```
##扫描命令```bash
# Find all providers
grep -rn "childContextTypes\|getChildContext" src/ --include="*.js" --include="*.jsx" | grep -v "\.test\."

# Find all consumers
grep -rn "contextTypes\s*=" src/ --include="*.js" --include="*.jsx" | grep -v "\.test\."

# Find this.context usage (may be legacy or modern - check which)
grep -rn "this\.context\." src/ --include="*.js" --include="*.jsx" | grep -v "\.test\."
```
##参考文件

- **`references/single-context.md`** -完全迁移一个上下文（主题，认证等）与提供者+类消费者+功能消费者
**`references/multi-context.md`** -具有多个遗留上下文的应用程序（嵌套的提供者，不同上下文的多个消费者）
- **`references/context-file-template.md`** -新上下文模块的标准文件结构