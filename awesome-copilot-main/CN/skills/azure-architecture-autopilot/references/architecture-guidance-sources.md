#架构指导源（用于设计方向决策）

使用Azure官方架构指南的源注册表**仅用于设计方向决策**。

b> **本文档中的url是“去哪里看”的资源列表
不要将这些url的内容硬编码为固定的事实。
>不要用于SKU、API版本、区域、模型可用性或PE映射决策——这些都是通过`azure-dynamic-sources.md`专门处理的。

---

##目的分离

|目的|文档使用|可决定项||---------|----------------|-----------------|
| **设计方向决策** |本文档（Architecture - guide -sources） |架构模式、最佳实践、服务组合方向、安全边界设计|
| **部署规范验证** |`azure-dynamic-sources.md`| API版本，SKU，区域，型号可用性，PE groupId，实际属性值|

**不能使用本文件决定的事项
- API版本
—SKUnames/pricing—区域可用性
—型号names/versions/deployment类型
- PE groupId / DNS区域映射关系
—资源属性的特定值

---

##主要来源

针对设计方向决策的目标获取目标。

| ID |文档| URL |用途||----|----------|-----|---------|
| A1 | Azure架构中心|https://learn.microsoft.com/en-us/azure/architecture/| Hub -查找特定于域的文档的入口点|
|架构良好的框架|https://learn.microsoft.com/en-us/azure/architecture/framework/|Security/reliability/performance/cost/operations原则|
| A3 |云采用框架/落地区|https://learn.microsoft.com/en-us/azure/cloud-adoption-framework/ready/landing-zone/|企业治理、网络拓扑、订阅结构|
| A4 | AzureAI/ML架构|https://learn.microsoft.com/en-us/azure/architecture/ai-ml/|AI/ML工作负载参考架构hub |
|基本铸造聊天参考架构|https://learn.microsoft.com/en-us/azure/architecture/ai-ml/architecture/basic-azure-ai-foundry-chat|基于铸造的基本聊天机器人结构|
| A6 |基线AI Foundry聊天参考架构|https://learn.microsoft.com/en-us/azure/architecture/ai-ml/architecture/baseline-openai-e2e-chat| Foundry聊天机器人企业基线（包括网络隔离）|
| A7 | RAG方案设计指南|https://learn.microsoft.com/en-us/azure/architecture/ai-ml/guide/rag/rag-solution-design-and-evaluation-guide| RAG模式设计指南|
| A8 |微软Fabric概述|https://learn.microsoft.com/en-us/fabric/get-started/microsoft-fabric-overview| Fabric平台概述和工作负载理解|
|结构治理/采用|https://learn.microsoft.com/en-us/power-bi/guidance/fabric-adoption-roadmap-governance|结构治理、采用路线图|次要来源（仅供了解）

不直接取目标；仅供变更意识参考。

|文档| URL | Notes ||----------|-----|-------|
| Azure更新|https://azure.microsoft.com/en-us/updates/|服务changes/new功能公告。不是一个目标抓取目标|

---

## Fetch触发器-何时查询

架构指导文档不会在每次请求时查询。**仅在以下触发器适用时执行目标提取。

触发条件0. **在阶段1（自动）中确定用户的工作负载类型时**
-预查询相关工作量的参考架构，调整问题深度
-自动触发，即使用户没有提到“最佳实践”等。
-目的：在问题中反映官方基于架构的设计决策点，超越SKU/region规范问题
1. **当用户要求设计方向证明**
-关键词：“最佳实践”、“参考架构”、“推荐结构”、“基线”、“良好架构”、“着陆区”、“企业模式”
2. 当新服务组合的体系结构边界不明确时
—无法从已有的参考files/service-gotchas确定服务间关系
3. **需要企业级security/governance设计时**
-订阅结构，网络拓扑，着陆区模式当触发器不应用时

-简单的资源创建（SKU/APIversion/region问题）→只使用`azure-dynamic-sources.md`-域包中已经包含的服务组合→优先考虑参考文件
-二头肌属性值验证→`service-gotchas.md`或MS Docs二头肌参考

---

##获取预算

|场景|最大读取次数||----------|----------------|
|默认值（当触发器触发时）|架构指导文档**高达2** |
当文档之间的冲突/核心设计的不确定性仍然存在/用户明确地要求更深的理由|时，允许额外的读取|
简单的部署规范问题| **0**（无架构指导查询）|

---

问题类型决定规则

|问题类型|要查询的文档|设计决策点提取|不查询的文档||--------------|-------------------|----------------------------------|----------------------|
| RAG / chatbot / Foundry应用| A5或A6 + A7 |网络隔离级别，认证方法（管理身份vs密钥），索引策略（推vs拉），监控范围|不遍历整个架构中心|
|企业安全/治理/着陆区| A2 + A3 |订阅结构、网络拓扑（hub-spoke等）、identity/governance模型、安全边界|AI/ML域文档不需要|
| Fabric数据平台| A8 + A9 |容量模型（SKU选择标准）、治理级别、数据边界（工作空间分离等）|人工智能相关文档不需要|
|模棱两可的服务组合（不明确的模式）| A1（从hub找到最近的域文档）+该文档|从文档|确定的关键设计决策点|不要遍历所有子文档|
|简单资源创建值（SKU/API/region） |无查询| - |所有架构指导|
|一般AI/ML架构| A4（集线器）+最近参考架构|计算隔离、数据边界、模型服务方法|不完全抓取|---

## URL回退规则

1. 默认使用`en-us`Learn url
2. 如果一个特定的URL返回404 / redirect / deprecated→回落到父中心页面
-示例：如果A5失败→在A4 （AI/MLhub）上搜索“foundry chat”关键字
3. 如果在父中心也没有找到→在A1（架构中心主）上按标题关键字搜索
4. **不要仅仅因为URL存在就将URL的内容作为固定规则使用**

---

禁止全遍历

—不要广泛地遍历（爬行）Architecture Center子文档
-根据问题类型的决策规则，只有针对性地提取1-2个相关文档
-即使在获取的文件中，也只引用相关部分；不阅读整个文档
—禁止无限抓取、递归链接跟随、子页面枚举