#测试模式

##核心部分（必选）

1)测试堆栈和命令

-主测试框架：[名称+版本]
-Assertion/mockingtools: [tools]
-命令:```bash
[run all tests]
[run unit tests]
[run integration/e2e tests]
[run coverage]
```
2)测试布局

-测试文件放置模式：[co-located/testsfolder/etc]
-命名约定：[pattern]
-安装文件及其运行位置：[路径]

测试范围矩阵

|范围|覆盖范围？|典型目标| Notes ||-------|----------|----------------|-------|
|单位| [yes/no] | [modules/services] |[注]|
|集成| [yes/no] | [API/data边界]|[注]|
| E2E | [yes/no] |[用户流]|[注释]|

### 4)嘲讽和隔离策略

-主要嘲弄方式：[module/class/network]
-隔离保证：[何时重置和重置]
-测试中常见的故障模式：[简短说明]

### 5)覆盖和质量信号

—覆盖工具+阈值：[value or TODO]
-当前报告的覆盖率：[value或TODO]
-已知gaps/flaky区域：[list]

6)证据- [path/to/test-config]
- [path/to/representative-test-file]
- [path/to/ci-or-coverage-config]
##扩展节（可选）

只在需要时添加：

-特定于框架的套件模式
-每个依赖类型的详细模拟食谱
-历史片状测试目录
-测试性能瓶颈和优化思路