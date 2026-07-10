#第二步：等待评审

子代理类型：`general-purpose`；预算：**20分钟硬帽** (1
有界子代理，非扩展驱动)。

**当循环在单次迭代时跳过**
（orchestration.md#single- iterator -fallback）——没有副驾驶
复习要等。

# #输入

从步骤1开始：
——`PrNumber`。
-`baseline`-捕获的`LatestCopilotReview.submittedAt`字符串
在触发之前（如果没有事先副驾驶检查，则为空字符串）。

##返回合同

-`02-check-review-status.ps1`JSON快照。
—`recommendation`∈{`ready`,`give-up-push-commit`}。
-`ready`iff **both**`LatestCopilotReview.submittedAt > baseline`和`ReviewAtHead: true`。

# #过程

大约每**3分钟**轮询`02-check-review-status.ps1`直到达到`ready`或20分钟的上限：```pwsh
pwsh ./scripts/02-check-review-status.ps1 -PrNumber <n>
```
-每次从JSON中提取`submittedAt`和`ReviewAtHead`。
-停止并返回`ready`在第一个滴答满足两者
条件与捕获的`baseline`。
-达到上限时没有`ready`，返回`give-up-push-commit`。

# #陷阱

- **不要投票快于~3分钟。**没有进度信号
从API；更快的轮询只会消耗预算。
- **`give-up-push-commit`回退是父驱动的。**当
子代理返回此建议，父代理推送a
实质性（非空白）提交-在`synchronize`is上自动分配
最可靠的触发器。然后父进程重新进入循环
步骤1用新鲜的`baseline`。
- **单有界运行，而不是扩展驱动。**不要要求
这一步的延伸-如果20分钟不够，正确的做法是`give-up-push-commit`回退，而不是更多的轮询。