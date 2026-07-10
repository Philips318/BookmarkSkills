#代码库结构

##核心部分（必选）

顶层地图

只列出有意义的顶级目录和文件。

|路径|目的|证据||------|---------|----------|
|[路径/]|[目的]|[源]|

2)入口点

-主运行时条目：[FILE]
-二级入口点（worker/cli/jobs）： [FILES或NONE]
—如何选择条目（script/config）：[注]

模块边界

|边界|什么属于这里|什么不能在这里||----------|-------------------|------------------------|
| [module/layer] |[责任]|[禁止逻辑]|

命名和组织规则

-文件命名模式：[kebab/camel/Pascal+ examples]
-目录组织模式：[feature/layer/domain]
-导入别名或路径约定：[RULE]

5)证据- [path/to/root-tree-source]
- [path/to/entry-config]
- [path/to/key-module]
##扩展节（可选）

仅当存储库复杂性需要时添加：

—子目录深度映射由feature/layer-Middleware/boot订单详情
-生成vs源布局边界
-单线程工作空间级结构映射