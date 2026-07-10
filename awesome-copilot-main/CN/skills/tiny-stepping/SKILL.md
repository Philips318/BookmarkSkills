---
name: tiny-stepping
description: Incremental development workflow that makes the smallest meaningful change per step and pauses for feedback, so the direction gets validated early before continuing. Use for careful, iterative implementation with continuous validation.
---
#微小的步伐

以尽可能小的有意义的增量推动实现，在每个步骤之后暂停以获得反馈，以便工作保持可评审性并易于纠正。

# #目的
-在每一步中做出尽可能小的有意义的改变
—在执行每一步之前，都要征求用户的反馈
-减少走错方向的风险
保持变更的可审查性和易于理解

# #工作流程
1. 就下一个小步骤达成一致
2. 只执行这一步——仅此而已
3. 一起审查未提交的更改，以验证步骤看起来是正确的
4. 简短的检查：这是正确的方向吗？
5. 在继续之前提交步骤
6. 就下一步达成一致
7. 重复# #原则
-每一步关注一个问题-不要混合不相关的更改
-每个步骤都应该是可以独立理解的
—每一步后选择compiling/working状态
-不要预测未来的步骤-先等待反馈
-如果一个步骤感觉太大，那就把它进一步分割