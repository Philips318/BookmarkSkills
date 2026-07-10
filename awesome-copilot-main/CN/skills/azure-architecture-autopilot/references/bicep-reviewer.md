#二头肌审查代理人

审查生成的Bicep代码并自动修复发现的任何问题。

##审核订单

###第一步：二头肌编译（先运行）

运行实际的二头肌编译** *之前**的检查表。不要仅凭目测就宣布“合格”。```powershell
az bicep build --file main.bicep 2>&1
```
从编译结果中收集所有警告和错误。这是本综述的基础数据。

###步骤2：修复编译Errors/Warnings修复编译结果中发现的问题：
- **ERROR**→必须修复并重新编译
- **警告**→按以下标准处理

**🚨警告处理标准-不要强制不必要的修复：**

警告不会阻止部署。尝试解决警告通常会引入部署错误，因此请使用以下标准：

|警告类型|动作|原因||---|---|---|
| BCP081（类型未定义）| **保持原样**（如果API版本是从MS Docs确认的最新版本）| Local Bicep CLI类型定义尚未更新。对部署|无影响
| BCP035（丢失的财产）| **仔细判断** -检查MS文档，确认是否确实需要该财产；如果没有，则保持原样|添加属性可能会由于兼容性问题（例如，computeMode）而导致部署失败|
| BCP187 （sku/kind类型未验证）| **保持原样** |从MS文档确认的值将在部署|时正常工作
| **保持原样** | DNS区域名称不可避免地需要硬编码|

永远不要做以下事情
降级API版本以解决警告（保持最新稳定）
-添加未在MS文档中确认的属性以解决警告
修复了针对“零警告”的强制修复**原则：在审查结果中记录警告，但如果它们不妨碍部署，则不修复它们

常见问题及回应：
- BCP081（类型未定义）→API版本可能不正确。获取MS文档并更新到实际的最新稳定版本
- BCP036（类型不匹配）→检查属性值的大小写和类型，然后修复
- BCP037（不允许的属性）→检查MS文档以验证该属性是否在该API版本中被支持
- no-hardcode -env- URLs→硬编码url在DNS区域名称等有时是不可避免的在Bicep。评审结果注释

步骤3：检查清单

在编译通过后检查以下项目。请参阅`references/service-gotchas.md`了解完整的获取信息。####严重（必须修复）
-[]微软铸造厂`customSubDomainName`设置存在- **创建后不能更改；如果资源缺失，必须删除并重新创建**
-[]当使用Microsoft Foundry时，**Foundry Project （`accounts/projects`）必须存在** -没有它门户访问不可用
-[]微软铸造厂`identity: { type: 'SystemAssigned' }`-项目创建失败没有它
- []`publicNetworkAccess: 'Disabled'`-所有使用PE的业务
- [] ADLS Gen2`isHnsEnabled: true`-没有它，变成常规Blob Storage
- [] PE -subnet`privateEndpointNetworkPolicies: 'Disabled'`-没有指定参数，创建PE失败
-[]私有DNS区域组-每个PE必须存在
-[]密钥库`enablePurgeProtection: true`####高（推荐修复）
—[]存储`allowBlobPublicAccess: false`，`minimumTlsVersion: 'TLS1_2'`-[]私有DNS区域VNet Link`registrationEnabled: false`-[]每个服务的资源类型和类型值匹配`references/ai-data.md`或MS Docs
-[]模型部署：订单保证（`dependsOn`）
-[]参数文件中没有敏感值- **发现立即删除**####中等（推荐）
-[]使用`uniqueString()`防止资源名冲突
-[]通过资源引用利用隐式依赖

步骤4：硬编码回归检查（防止动态信息泄漏）

验证以下项目在Bicep代码中没有硬编码为文字值：

####必须参数化（无硬编码）
- []`location`-不直接使用文字区域名称（`'eastus'`，`'koreacentral'`等）；通过`param location`传递
-[]型号name/version-非文字；在阶段1中确认并在步骤0中验证可用性的使用值
- [] SKU -与用户确认的使用值

####验证动态值是否回归为引用
这并不直接在本审查的范围内，但如果特定的API版本，SKU列表或区域列表在代码注释或参数描述中硬编码，请删除它们并替换为“检查MS文档”指导。####决策规则违规检查
-[]如果使用`kind: 'OpenAI'`而不是Foundry→除非用户明确要求，否则更改为`kind: 'AIServices'`-[]如果Hub （`MachineLearningServices`）用于一般AI/RAG→更改为Foundry，除非用户明确要求
-[]如果使用独立的Azure OpenAI资源→建议检查Foundry使用情况，除非用户明确要求或文档指出这是必要的

步骤5：修复后重新编译

如果在步骤2-4中进行了任何更改，请再次运行`az bicep build`以验证没有引入新的错误。`az bicep build`的局限性

编译只验证语法和类型。以下项目无法被编译捕获，最终在阶段4的`az deployment group what-if`中进行验证：
-Retired/unavailableSKU
—各区域服务可用性
-模型名称有效性
-仅预览属性
—服务策略变更（配额、容量等）在审查结果中说明这些限制，以便用户理解假设步骤的重要性。

第六步：报告结果```markdown
## Bicep Code Review Results

**Compilation Result**: [PASS/WARNING N items]
**Checklist**: ✅ Passed X items / ⚠️ Warnings X items
**Hardcoding Check**: [PASS / N violations]
**Auto-fixed**: X items

### Compilation Warnings (Remaining)
- [Warning content — including reason why it cannot be fixed]

### Auto-fix Details
- [File:line number] Before → After (reason)

### Hardcoding Violations (If Any)
- [File:line number] [Violation details] → [Fix method]

**Conclusion**: [Ready for deployment / Manual review required]
```
###步骤7：第四阶段过渡-需要保证信息

当询问是否在通过代码审查后进入第四阶段时，**总是包含一个让用户放心的信息**。
用户可能会对“部署”这个词感到不安，所以要清楚地沟通，假设是一个安全的验证步骤。```
ask_user({
  question: "Code review passed! Ready to proceed to the next step?\n\n⚡ This does NOT deploy immediately:\n  1️⃣ What-if validation — Simulates what will be created (not a deployment, safe)\n  2️⃣ Preview diagram — Review the architecture to be deployed as a diagram\n  3️⃣ Final confirmation — Actual deployment only after you review the diagram and approve\n\nNothing will be deployed without your approval.",
  choices: [
    "Proceed to next step (what-if validation + preview diagram) (Recommended)",
    "Just give me the code, I'll deploy later"
  ]
})
```
* *重点:* *
-总是声明“这不会立即部署”
-解释3步流程：假设→预览图→最终确认
-重申“未经您的批准，我们不会部署任何内容”