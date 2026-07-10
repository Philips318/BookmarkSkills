#步骤3：列出并分类打开的线程

子代理类型：`explore`；预算：5分钟。

# #输入

——`PrNumber`。

##返回合同

表的行，每个打开的线程一个：```
{ thread_id, file, line, author, author_class, severity, summary }
```
式中`author_class`∈`copilot`|`human-or-bot`，由
raw`author.login`（参见获取）。

# #过程

运行清单脚本：```pwsh
pwsh ./scripts/03-list-open-threads.ps1 -PrNumber <n>
```
这将返回来自**所有审阅者**的所有未解决的审阅线程
（副驾驶，人类，`github-advanced-security`，其他机器人）。这个脚本
当注释锚定到a时，将`Path`发送为`<file>:<line>`具体行（如`src/foo.js:42`）；当评论没有行时
锚（文件级/ pr级注释），`Path`只是`<file>`无`:<line>`后缀。调用者应该在最后一个`:`**上分裂，只有当
后缀解析为整数**，并将`Path`单独视为文件
否则。对于每一行，对`author`进行分类：

-`copilot-pull-request-reviewer`或`copilot-pull-request-reviewer[bot]`→`author_class: copilot`-其他一切→`author_class: human-or-bot`将分类表传递到步骤4——分诊规则取决于它。

# #陷阱- **`[bot]`后缀出现在某些表面。**两者匹配`copilot-pull-request-reviewer`和`copilot-pull-request-reviewer[bot]`，它们是同一个演员。
- **默认人类/高级安全线程为`escalate-to-user`in
步骤4。**这里的分类只是标记它们；分诊分类适用于
策略。[04-triage.md] (04-triage.md)。
未解决的问题是真理的源泉。* * Outdated-but-unresolved
线程仍然显示-这是正确的。不要把它们过滤掉；
它们的处理方式与步骤8中的其他开放线程一样。