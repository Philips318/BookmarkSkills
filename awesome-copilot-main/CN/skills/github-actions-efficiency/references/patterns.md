#规范模式

只有在实现过程中需要具体示例时才加载此参考。

##依赖缓存```yaml
- uses: actions/cache@v4
  with:
    path: ~/.npm
    key: ${{ runner.os }}-node-${{ hashFiles('**/package-lock.json') }}
    restore-keys: |
      ${{ runner.os }}-node-
```
调整缓存路径和失效文件以适应repo的生态系统。

##取消陈旧运行```yaml
concurrency:
  group: ${{ github.workflow }}-${{ github.ref }}
  cancel-in-progress: true
```
## Scope触发器```yaml
on:
  push:
    paths:
      - "src/**"
      - "tests/**"
      - "package.json"
```
当排除比包含更容易维护时，请使用`paths-ignore`。

## Job-Level Changed-File Gating

使用一个小的变更检测步骤，发出显式的输出，例如：

——`docs_relevant`——`runtime_relevant`——`compat_relevant`——`run_tests`当事件级过滤器的表达能力不够时，在这些输出上对下游作业进行闸门处理。

矩阵约简

使用与决策匹配的最小矩阵：

-完整的矩阵释放
-减少敏感运行时表面上的兼容性矩阵
-普通代码更改的单一代表腿

##可选回写作业

对改变PR分支的作业使用标签驱动或手动触发器，例如格式化机器人。