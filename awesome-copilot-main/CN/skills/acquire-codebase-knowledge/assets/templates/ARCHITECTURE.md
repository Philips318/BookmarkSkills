#架构

##核心部分（必选）

1)建筑风格

-主样式：[layered/feature/event-driven/other]
-为什么这样分类：[简短的证据支持的理由]
-主要约束：[塑造设计的2-3个约束]

2)系统流程```text
[entry] -> [processing] -> [domain logic] -> [data/integration] -> [response/output]
```
使用文件支持的证据分4-6个步骤描述流程。Layer/Module责任

|层或模块|拥有|不能拥有|证据||-----------------|------|--------------|----------|
| [name] | [responsibility] | [non-responsibility] | [file] |

重用的模式

|模式|在哪里发现|为什么存在||---------|-------------|---------------|
| [singleton/repository/adapter/etc] | [path] | [reason] |

5)已知的架构风险

-[风险1 +影响]
-[风险2 +影响]

6)证据- [path/to/entrypoint]
- [path/to/main-layer-files]
- [path/to/data-or-integration-layer]
##扩展节（可选）

只在需要时添加：

-启动或初始化订单的详细信息
—Async/event拓扑
-带有重构路径的反模式目录
-失效模式分析和弹性姿态