# Python IoT Edge模块模板

使用此模板构建实施建议和审查。

官方Python基线

-从<https://www.python.org/>和<https://docs.python.org/3/>审核的官方参考资料。
-针对<https://docs.python.org/3/reference/>和<https://docs.python.org/3/library/>验证语言和标准库使用。
-从`references/python-official-best-practices.md`审查的最佳实践。

模块总结

-模块名称：
-业务能力：
-输入:
输出:
—触发条件：

消息契约

-架构版本：
-必填字段：
—可选字段：
-错误的有效载荷合同：

## 3)运行时配置

- Python版本：
-基础映像：
—环境变量：
-期望属性：
-资源限制：

## 4)弹性

—重试策略：
-退让政策：
-排队策略：
-幂等法：
-超时和断路器行为：

5)安全性-秘密来源（从未内联）：
-身份和权限：
—命令授权模式：
—审计日志要求：

6)可观察性

-健康信号：
-业务指标：
-误差指标：
-Correlation/trace要求：
—告警阈值：

验证矩阵

-快乐路径测试：
-无效有效载荷测试：
-网络中断测试：
-吞吐量和延迟测试：
-回滚验证：