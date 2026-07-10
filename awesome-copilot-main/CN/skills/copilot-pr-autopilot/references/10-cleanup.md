步骤10：清理过时的东西

Owner: **父**（无子代理）；预算：n/a.运行**一次，之后
收敛**（第9步返回`Converged: true`）。

# #输入

—收敛PR为`PrNumber`。

##返回合同

—无—步骤10为终端。在它运行之后，循环就完成了
父节点调用`task_complete`并给出收敛证明
第9步。

# #过程```pwsh
pwsh ./scripts/10-cleanup-outdated.ps1 -PrNumber <n>
```
只有安全网。大多数循环都没有需要清理的东西——过时了
线程应该已经在步骤8中被回复和解决了
其他开放的线程。未解决的状态是公关中真相的来源
用户界面;`10-cleanup-outdated.ps1`只抓流浪狗。