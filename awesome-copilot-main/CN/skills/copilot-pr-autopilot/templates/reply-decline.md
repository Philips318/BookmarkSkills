#回复：有理由拒绝

使用时，分流决定`decline`。回复必须解释为什么
拒绝是正确的选择——不仅仅是因为你考虑过。总是
回复后解决问题；没有回复的开放线程
避免信号。```
Considered this, but declining: <concrete reason rooted in code or
design>. <Optional: the interleaving / scenario you ruled out, or
the alternative cost>. Happy to revisit if <specific trigger>.
```
(独立于领域的例子):

>考虑将锁扩展到初始化路径，但是
> decline：初始化在任何并发之前运行到完成
>调用者可以到达此代码，因此比赛窗口只在之后打开
>初始化回调已经返回。跨模块共享锁
>的耦合成本高于实际暴露所证明的。快乐
如果遥测显示出真正的交错，>将重新访问。