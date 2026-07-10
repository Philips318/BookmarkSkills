#回复：接受修复

在循环提交并推送了查找结果的修复后使用。引用
从步骤7推送提交SHA。```
<one sentence acknowledging the finding>.
<one or two sentences describing the fix>.
Fixed in <commit-sha>.
```
例子(语言无关):

锁没有覆盖路径的安装侧，所以是2
>个并行编写器可以读取相同的基线并对每个基线进行重写
>。将每个实例锁提升到进程范围
> function-local static以便所有读-修改-写路径共享它。
>固定在abc1234。

当修复在测试区域时，添加一行测试确认：

>将平台UUID依赖替换为PID +单调时钟
> +原子计数器，因此测试目标不再拉入
>平台UUID库。所有42个测试都在受影响的套件中
>通过。固定在abc1234。