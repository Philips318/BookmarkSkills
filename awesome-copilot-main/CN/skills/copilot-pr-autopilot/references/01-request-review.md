#步骤1：请求审查

Owner: **父**（无子代理）；预算:n/a.# #输入

—目标PR为`PrNumber`。

##返回合同

-捕获的`baseline`=`LatestCopilotReview.submittedAt`字符串（或空）
传递给步骤2。
-布尔值`single_iteration_mode`-如果触发失败，`true`，因为
副驾驶不是一个有效的审查员；否则`false`。

# #过程

1. 先快照，了解副驾驶是否已经挂起：   ```pwsh
   $snap = pwsh ./scripts/02-check-review-status.ps1 -PrNumber <n>
   $baseline = if ($snap -match '"submittedAt":"([^"]+)"') { $Matches[1] } else { '' }
   $pending  = ($snap -match '"CopilotPending":true')
   ```
在原始JSON上的Regex将`submittedAt`作为字符串
任何PS版本（5.1 / 7）的父代理→子代理边界。x),避免`[datetime]`重新绑定。

2. **如果`$pending`** -跳过触发器；使用`baseline`跳转到步骤2。

3. **否则** -触发扳机：   ```pwsh
   pwsh ./scripts/01-request-review.ps1 -PrNumber <n>
   ```
脚本保留了自己的`InFlight`短路作为安全网，
但标准的“副驾驶是否等待？”信号仍然存在`02-check-review-status.ps1`(见上图)。

4. 如果`01-request-review.ps1`抛出，因为副驾驶无效
审稿人（在回购/账户上未启用副驾驶代码审查），
以[单迭代回退]为例（orchestration.md#single-iteration-fallback）。