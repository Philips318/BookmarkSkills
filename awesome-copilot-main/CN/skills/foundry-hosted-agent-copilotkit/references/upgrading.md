#安全升级依赖

该堆栈的每一层都是1.0之前的版本、候选版本或2026年中期的预览版：`@ag-ui/*`是0.0。`agent-framework-ag-ui`是RC， Foundry托管包是alpha/beta，托管代理本身是预览版，CopilotKit每周发布移动api的小版本。升级是这个堆栈崩溃的地方。不要单独撞击一个包裹。

版本关系规则（所有规则必须同时生效）1. **`@ag-ui/client`↔`@copilotkit/runtime`**：运行时引脚一个精确的`@ag-ui/client`版本。你的应用程序的`@ag-ui/client`必须匹配它，否则TypeScript会因为`HttpAgent`的形状差异而中断（例如缺少`pendingInterrupts`）。在碰撞CopilotKit后，读取安装的运行时的`package.json`并对齐。
2. **`@copilotkit/*`包一起移动**:`react-core`，`react-ui`和`runtime`是同步发布的-永远不要混合版本。检查lockfile是否解析了`package.json`请求的内容。
3. **`agent-framework-*`Python包放在一行**:`agent-framework-core`，`agent-framework-foundry`,`agent-framework-ag-ui`，托管包必须来自兼容的版本。依赖于`agent-framework-core`+特定的附加功能，而不是`agent-framework`元包（它拖拽可选的依赖项，会破坏Foundry远程映像构建）。
4. **托管协议版本↔代理清单**:Foundry托管包实现特定的响应协议版本；`agent.yaml`和`agent.manifest.yaml`必须声明相同的`version:`或代理在启动时快速失败，显示协议不匹配的RuntimeError。Bump包和两个清单在一次提交中。
5. **弃用包检查**:`agent-framework-azure-ai`被`agent-framework-foundry`取代。如果代码库仍然导入旧的代码库，那么在进行任何其他升级之前进行迁移。升级循环1. **首先清点当地的变通办法。**维护一个分类账，将代码库中的每个patch/workaround映射到它存在的上游问题（例如，批准转发代码↔microsoft/agent-framework#6652；`previous_response_id`保护↔#6851/#6828；前端取绑定缺陷↔非法调用bug）。升级是唯一可以删除这些问题的时间，并且只有当问题在已发布的版本中被关闭并且保护解决方案的回归测试通过时才可以。永远不要仅仅因为版本问题而删除变通方法。
2. **阅读实际发货的内容。** CopilotKit的发行说明通常是空的自动发行存根——在不同版本之间区分捆绑的`.d.ts`文件以查找API更改，并在集成路径中扫描问题跟踪器以查找回归（远程`HttpAgent`+前端工具历来是一个脆弱的组合）。
3. **根据上述规则连贯地碰撞**；重新安装;检查lockfile解析。
4. * * Re-veRify完整的矩阵，live** -不只是编译：
-read/query路径通过真实UI；
前端工具对代理可见（显式的）
-批准只执行一次门控工具；Reject执行0次；
-批准后的后续回合不重新执行（hitl.md危害）；
-tool/approval卡在`RUN_FINISHED`之后仍然存在，而不仅仅是在流媒体期间。
5. **对于托管代理**：在依赖项更改后重启`azd ai agent run`（本地运行时在运行之间不缓存任何东西，但内存中的种子数据重置-在断言之前重新设置基线），然后重新部署并点检已部署的端点；本地成功不能证明远程映像构建（远程构建独立解决依赖关系-显式引脚避免漂移）。每次升级都要检查已知的上游问题

截至2026年7月，状态是准确的-在采取行动之前重新检查：

|问题|原因|本地解决方案|| --- | --- | --- |
| AG-UI适配器在本地解析HITL批准；永远不会转发到remote/hosted代理，因此已批准的工具不会在桥|中重新执行|自定义批准路由
|microsoft/agent-framework#6851 |审批控制工具通过`previous_response_id`链|在以后不相关的回合中静默地重新执行（重复的副作用）
审批完成后，审批UI状态恢复为“进行中”；与#6851 |化妆品相关，除非与#6851 |配对
|前端工具未转发到`RunAgentInput.tools`与远程`HttpAgent`（1.62. 0）x era) |升级到修复后；每次碰撞后重新测试工具的可视性
|CopilotKit/CopilotKit#5812 |`TEXT_MESSAGE_END`发出后`RUN_ERROR`，打破错误处理|升级过去修复|

铸造厂平台截止日期部署在2026年4月之前的预览后端（`azure-ai-agentserver-agentframework`/`-langgraph`路径）上的托管代理在2026-05-22结束了对它的支持——任何仍然在该路径上的东西都必须重新部署到当前的托管包上，而不是就地升级。请参阅Microsoft Learn上的托管代理迁移指南（`/azure/foundry/agents/how-to/migrate-hosted-agent-preview`）。