#分析原则-安全分析方法

此文件包含有关如何分析安全威胁代码的所有规则。它是独立的——执行正确的、基于证据的安全分析所需的一切都在这里。

---

##⛔关键：在标记之前进行验证

**在未确认安全漏洞存在的情况下，切勿标记安全漏洞。**许多平台都有安全默认值。

三步验证

1. **在声明缺少安全性之前检查安全基础架构组件**：
-证书颁发机构（dpr哨兵、证书管理器、保险库）
-业务网格控制平面（Istio, Linkerd, Dapr）
-策略引擎（OPA, Kyverno, Gatekeeper）
-秘密管理器（保险库，Azure密钥保险库，AWS秘密管理器）
-身份提供者（MISE， OAuth代理，OIDC）2. **了解平台默认设置** -在假设之前进行研究：
—Dapr：部署Sentry时默认启用mTLS
- Kubernetes：从v1.6开始默认启用RBAC
—Istio: mTLS默认为permit模式，可以使用STRICT模式
- Azure：默认情况下，许多服务都是静态加密的

3. **区分配置状态**：
- **显式禁用**:`enabled: false`→标志为查找
- **未配置**：未设置→先检查平台默认值
- **隐式启用**：默认行为是安全的→文档作为控制，而不是缺口

证据质量要求

对于每一个发现：
-显示特定的config/code，证明差距（不只是缺少配置）
-对于“缺失安全性”声明，证明默认值是不安全的
-在不确定的情况下与平台文档进行交叉参考

---

安全基础设施清单在进行STRIDE-A分析之前，确定代码库中存在的所有启用安全的组件：

|类别|组件寻找|安全他们提供||----------|----------------------|----------------------|
| Service Mesh | Dapr、Istio、link、Consul Connect | mTLS、流量策略、可观察性|
|证书管理|哨兵，cert-manager， Vault PKI |自动证书issuance/rotation|
|认证| MISE， OAuth2-proxy, Dex， Keycloak |令牌验证，单点登录|
|授权| OPA、Kyverno、Gatekeeper、RBAC |策略实施|
|秘密|金库，外部秘密，CSI驱动|秘密注入，旋转|
|网络|网络策略，花布，纤毛|微分割|

**如果存在这些组件，除非显式禁用，否则它们的安全功能可能处于活动状态

---

安全分析镜头

在分析期间应用这些框架：

- **零信任**：明确验证，最少特权，假设违约
- **纵深防御**：识别缺失的安全层
- **滥用案例**：业务逻辑滥用，工作流操纵，功能滥用

---全面覆盖要求

**不要截断较大代码库的分析。**所有组件必须得到相同的分析深度。

Sidecar安全分析

⚠️**Sidecars （Dapr， MISE， Envoy等）不是DFD中单独的组件-它们与主容器一起位于同一个pod中（参见diagram-conventions.md规则2）。然而，侧车通信仍然必须进行安全漏洞分析。**如何分析侧车威胁：**
-具有不同威胁面的侧车（例如，MISE auth bypass, Dapr mTLS）在`2-stride-analysis.md`中有自己的`## Component`部分-但不是单独的DFD节点（参见diagram-conventions.md规则2）
-使用格式：威胁标题包括侧车名称，例如，“Dapr侧车明文通信”
-常见侧斗威胁：
- **信息披露（一）：**Dapr/MISE侧车在pod内通过明文HTTP与主容器通信
- **篡改(T):** Daprpub/sub消息未签名或加密
- **欺骗(S):** MISE令牌验证绕过，如果侧车被破坏
- **提升特权(E):** Sidecar在主容器不需要的情况下使用提升的特权运行
- CWE映射：CWE-319（明文传输），CWE-311（缺少加密），CWE-250（不必要的特权）
-这些威胁出现在侧车自己的STRIDE部分（如果有的话）一个明显的威胁面)或在主组件的表下（如果侧车是一个简单的基础设施代理）
-如果sidecar漏洞值得发现，将其列在sidecar组件下，并注明：“影响[Dapr/MISE] sidecar通信”1. **最小覆盖范围：**`0.1-architecture.md`中的每个组件必须在`2-stride-analysis.md`中有一个对应的部分，其中包含实际的威胁枚举（而不仅仅是“未发现威胁”）。
2. **查找密度检查：**作为指导原则，预计每2-3个重要组件大约有1个查找。如果一个回购有15个以上的组件，而你的发现少于8个，重新检查未被分析的组件。
3. **使用子代理进行扩展：**对于包含10个以上组件的repos，将特定组件的STRIDE分析委托给子代理以保持深度。每个子代理应分析3-5个成分。
4. **OWASP检查表扫描：**在组件级跨步之后，使用下面的OWASP Top 10:2025检查表进行横切。这捕获了组件级分析可能遗漏的系统问题（缺少认证、没有审计日志记录、没有速率限制、未签名映像）。
5. **基础设施层检查：**显式检查：容器安全上下文t、网络策略、资源限制、映像签名、秘密管理、backup/DR控件和monitoring/alerting缺口。
6. **详尽的调查结果整合：** STRIDE分析完成后，扫描STRIDE输出的所有已识别的威胁。每个威胁必须映射到：
-`3-findings.md`的发现（与相关威胁合并）
-威胁覆盖验证表中的`🔄 Mitigated by Platform`条目（仅适用于平台处理的威胁）

**⛔每一个`Open`威胁必须有一个发现。**该工具没有接受风险、延迟威胁或决定威胁是“可接受的”的权限。这是工程团队的决定。该工具的工作是识别所有威胁并为它们创建发现。覆盖表应该显示每个开放威胁的`✅ Covered (FIND-XX)`- NEVER`⚠️ Accepted Risk`。如果你有40多个威胁，但只有10个发现，你的整合不足。检查遗漏的数据存储验证、操作控制、凭证管理和供应链问题。

**⛔禁止“接受风险”（强制性）：**
- **永远不要使用`⚠️ Accepted Risk`作为覆盖表状态。**这个标签意味着工具代表工程团队接受了风险。但事实并非如此。它不能。
- **永远不要使用`Accepted`作为STRIDE Status值。**只能使用`Open`、`Mitigated`或`Platform`。
-如果你想写“可接受的风险”→创建一个发现代替。发现的补救部分告诉团队该做什么。团队决定是接受、修复还是推迟。**⛔需要审查的限制（强制性）：**
- **一级威胁（先决条件=`None`）绝对不能归类为“⚠️需求审查”。**未经身份验证的外部攻击者可利用的威胁不能延迟-它必须成为一个发现。
- **如果威胁在STRIDE分析中列出了缓解措施，则应将其作为发现项。**缓解文本是补救-用它来写发现。只有当缓解措施确实不可行时，才遵从“需求审查”。
- **具有`None`先决条件的DoS威胁是一级发现**，而不是强化机会。未经身份验证的攻击者充斥没有速率限制的API是一个可直接利用的漏洞（CWE-770, CWE-400）。
- **不要将整个STRIDE类别批量分类为需求审查。**每个威胁必须根据其先决条件和可利用性单独评估。
- **"⚠️需要“审查”保留用于：**层2/3威胁，其中不可能实现技术缓解（例如，社会工程），或需要工具不具备的业务上下文的威胁。
- **自动化分析没有接受风险的权限** -它只能识别风险。“需求审查”意味着人类必须做出决定。
- **最大需求审查比例：**如果超过30%的威胁被归类为“需求审查”，请重新检查-您可能未充分报告调查结果。典型的比例：对于一个经过充分分析的代码库来说，是10-20%。
7. **按回购规模查找最小阈值：**
-小型repo（< 20个源文件）：预计8+发现
-中等回购（20-100个源文件）：预计12个以上的发现
-大型回购（100+源文件）：18+发现预期

如果低于阈值，系统审查：每个组件的授权，代码中的秘密，容器安全性，网络分段，logging/monitoring，输入验证。8. **上下文感知平台比例限制（必选）：**

在完成安全基础设施清单（步骤1）之后，检测部署模式：

|模式|检测信号|平台限制|   |---------|-----------------|----------------|
| **K8s运算符** |`controller-runtime`，`kubebuilder`，或`operator-sdk`中的go.mod/go.sum；`Reconcile()`作用于源| **≤35%** |
| **独立应用程序** |所有其他回购（web应用程序，CLI工具，服务）| **≤20%** |

**为什么k8运营商有更高的平台比率：**运营商将安全性委托给k8平台（RBAC用于CR访问，etcd加密，API服务器TLS， webhook证书验证，Azure AD令牌验证）。操作符代码不能实现这些控制——它们是平台的责任。将它们归类为平台是正确的。

**平台超过限制时的动作：**
-审查每个平台分类威胁
-如果操作员可以采取措施（例如，添加输入验证，在启动时添加RBAC检查）→重新分类为`Open`-如果操作员确实无法操作（例如，etcd加密是一个集群管理问题）→平台是正确的
——文档`0-assessment.md`中检测到的模式和比例→分析背景和假设---

特定于技术的安全检查表

**完成STRIDE分析**后，扫描以下每种技术的代码库。对于发现的每一项技术，验证相应的安全检查是否包含在发现内容中，或者是否记录为已缓解。这捕获了组件级STRIDE经常忽略的特定漏洞。

|技术发现|必须检查|常见发现||-----------------|---------------|----------------|
| **Redis** |`requirepass`禁用，没有TLS，没有ACL |默认禁用认证→查找|
| **Milvus** |`authorizationEnabled: false`，没有TLS，公共gRPC端口|默认禁用认证→查找|
| **PostgreSQL/SQLDB** |超级用户使用，`ssl=false`， SQL注入，连接字符串凭证|输入验证+授权|
| **MongoDB** |关闭认证，无TLS，`--noauth`标志|默认关闭认证|
| **NGINX/Ingress** |缺少TLS， server_info报头，片段注入，速率限制|配置强化|
| **Docker/Containers** |以root运行，无`USER`指令，主机挂载，无seccomp/AppArmor，无符号映像|容器加固|
| **ML/AIModels** |未经认证的推理端点，模型中毒，提示注入，无输入验证| endpoint auth +输入验证|
| **LLM/CloudAI** |PII/secrets发送到外部LLM，无内容过滤，提示注入，数据泄露|数据暴露到云端|
| * * Kubernetes * *|无NetworkPolicy，无PodSecurityPolicy/Standards，无资源限制，RBAC间隙|网络分段+资源限制|
| **Helm Charts** |在values.yaml中硬编码的秘密，没有图像标签固定，没有安全上下文|配置+供应链|
| **密钥管理** |硬编码RSA/HMAC密钥，弱密钥生成，不旋转，密钥在源|加密失败|
| **CI/CD管道** |日志中的秘密，无工件签名，可变依赖，脚本注入|供应链|
| **REST api ** |缺少认证，无速率限制，详细错误，无输入验证|认证+注入|
| **gRPC服务** |无TLS，无身份验证拦截器，生产中启用反射|身份验证+加密|
| **消息队列** |对pub/sub不授权，不加密，不消息签名|授权+完整性|
| **NFS/File共享** |路径遍历，无访问控制，全局可读挂载|访问控制|
| **Audit/Logging** |无安全事件日志记录，日志注入，无篡改保护|监控缺口|**进程：**写完3-findings.md后，扫描这个表来查找repo中存在的技术。对于每种技术，基于该技术的实际使用方式评估其常见的特定于技术的威胁模式，并确保在评估中考虑到任何相关风险。只有在确定了实际威胁或有意义的缓解差距时，才添加调查结果。

---

OWASP Top 10:2025清单

在分析期间检查这些漏洞类别：

| ID |类别|检查是否为||----|----------|----------|
| A01 |访问控制失效| authZ、权限升级、IDOR、CORS配置错误|
| A02 |安全配置错误|默认信用，详细错误，不必要的功能，缺少加固|
|软件供应链故障|易受攻击的依赖项，恶意软件包，受损CI/CD|
|算法弱，秘密暴露，密钥管理不当，明文数据|
| A05 |注入| SQL、NoSQL、操作系统命令、LDAP、XSS、模板注入|
| A06 |不安全的设计|在体系结构级别缺少安全控制，威胁建模存在缺口|
| A07 |认证失败|认证失败，弱会话，凭证填充，缺少MFA |
| A08 |Software/Data完整性失效|不安全反序列化，无符号更新，CI/CD篡改|
| A09 |安全日志和告警失败|审计日志缺失，无告警，日志注入，mo不足nitoring |
| A10 |异常条件处理不当|错误处理不良，竞争条件，资源耗尽|参考:https://owasp.org/Top10/2025/---

##平台安全默认值参考

在标记缺失的安全性之前，请检查这些常见的默认安全行为：

|平台|特性|默认行为| |验证方法|----------|---------|------------------|---------------|
| **Dapr** | mTLS |哨兵部署时启用|检查`dapr_sentry`或`sentry`组件|
| **Dapr** |访问控制|拒绝定义的策略|在配置|中查找`accessControl`| **Kubernetes** | RBAC |自v1.6 |启用检查`--authorization-mode`包括RBAC |
| **Kubernetes** | Secrets | Base64编码（未加密）|检查加密提供商配置|
| **Istio** | mTLS |默认为PERMISSIVE |检查PeerAuthentication资源|
| **Azure存储** |休眠加密|默认启用|始终加密，检查密钥管理|
| **Azure SQL** | TDE |默认启用| |上的透明数据加密
| **PostgreSQL** | SSL |通常默认关闭|检查`ssl`参数|
| **Redis** | Auth |默认关闭|检查`requirepass`配置|
| **Milvus** | Auth |默认关闭|检查`authorizationEnabled`|
| **NGINX Ingress** | TLS |默认不启用|检查f或入口b|中的TLS秘密
| **Docker** |用户|默认Root |查看Dockerfile |中的`USER`**关键洞察**：服务网格（Dapr, Istio, Linkerd）通常自动启用mTLS。数据库（Redis, Milvus, MongoDB）通常默认禁用身份验证。

---

可利用层

基于先决条件，威胁被分为三个可利用层：

|分级|标签|先决条件|分配规则||------|-------|---------------|----------------|
| **第1层** |直接暴露|`None`|可被未经身份验证的外部攻击者利用。|
| **二级** |条件风险|单一先决条件|只需要一种访问形式：`Authenticated User`、`Privileged User`、`Internal Network`或单一`{Boundary} Access`。|
| **三层** |纵深防御|多个先决条件或基础设施接入|需要`Host/OS Access`、`Admin Credentials`、`{Component} Compromise`、`Physical Access`，或`+`的多个先决条件。|

分级分配规则

**⛔规范先决条件→层映射（确定性的，没有例外）：**

前提条件必须只使用这些值（封闭enum）。这一层是机械的：

|先决条件|分级|基本原理||-------------|------|----------|
|`None`| ** tier1 ** |未经认证的外部攻击者，没有事先访问|
|`Authenticated User`| ** tier2 ** |需要有效凭证|
|`Privileged User`| ** tier2 ** |需要admin/operator角色|
|`Internal Network`| ** tier2 ** |内部网络|
|`Local Process Access`| ** tier2 ** |要求在同一主机（本地主机监听器，IPC） |上执行代码
|`Host/OS Access`| **Tier 3** |需要对主机|进行文件系统、控制台或调试访问
|`Admin Credentials`| ** tier3 ** |需要管理员凭据+主机访问|
|`Physical Access`| ** tier3 ** |需要物理存在（USB、串口）|
|`{Component} Compromise`| ** tier3 ** |需要事先妥协另一个组件|
|任意`A + B`组合| ** tier3 ** |多个先决条件=始终为tier3 |

**⛔禁止的前提值：**`Application Access`，`Host Access`（模棱两可-使用`Local Process Access`或`Host/OS Access`）。**如果部署分类为`LOCALHOST_DESKTOP`或`LOCALHOST_SERVICE`，则禁止所有组件使用先决条件`None`-使用`Local Process Access`或`Host/OS Access`。然后，层从纠正的先决条件开始。

⛔先决条件确定（强制性-基于证据，而不是基于判断）

**先决条件必须根据部署配置证据确定，而不是根据一般知识或假设确定。**运行在相同代码上的两个独立分析必须分配相同的先决条件，因为它们是关于部署的客观事实。

**通用决策程序（适用于所有环境）

1. **网络暴露检查-组件是否可以从外部访问？**
-在代码库中寻找外部暴露的证据；     - API gateway / reverse proxy routes pointing to the component
     - Firewall rules or security group configurations
     - Load balancer configurations
     - DNS records or public endpoint definitions
—ANY外部路由存在→基于网络的威胁条件=`None`—不存在外部路由且组件在内部网络中→先决条件=`Internal Network`2. **身份验证检查-端点是否需要凭据？**
-在组件的代码中寻找认证中间件、装饰器或过滤器：     - `@require_auth`, `[Authorize]`, `@login_required`, auth middleware in Express/FastAPI
     - API key validation in request handlers
     - OAuth/OIDC token validation
     - mTLS certificate requirements
—如果在所有端点上强制执行auth→prerequisite =`Authenticated User`如果auth是可选的或被配置标志禁用→prerequisite =`None`（DISABLED auth = no barrier）
—如果auth存在，但存在旁路路由（如`/health`，`/metrics`没有auth）→这些特定的路由具有先决条件=`None`3. **授权检查-需要什么级别的访问？**
—如果没有RBAC/role检查超出认证→先决条件保持`Authenticated User`—如果需要admin/operator角色→先决条件=`Privileged User`-如果需要特定的权限→先决条件名称的权限（例如，`ClusterAdmin Role`）4. **Physical/Local访问检查：**
-如果组件只监听`localhost`/`127.0.0.1`→先决条件=`Local Process Access`（T2）
-如果访问需要console/SSH/filesystem→先决条件=`Host/OS Access`（T3）
—需要物理存在（USB、串口）→前提条件=`Physical Access`（T3）
-如果组件没有监听器（控制台应用程序，库，仅出站）→先决条件=`Host/OS Access`（T3）

5. **默认规则：**如果不能从配置中确定曝光，请在组件曝光表中查找组件的`Min Prerequisite`。如果该表尚未填充，则假定`Local Process Access`（T2）是未知组件的安全默认值。**在没有外部可达性的积极证据的情况下，永远不要假设`None`。** **如果没有网络限制的证据，永远不要假设`Internal Network`**平台特定证据来源：**

|平台|检查曝光位置|内部指示灯|外部指示灯||----------|------------------------|--------------------|--------------------|
| **Kubernetes** |业务类型，入口规则，values.yaml|`ClusterIP`服务，无入口|`LoadBalancer`/`NodePort`，入口路径存在|
| **Docker Compose** |`ports:`映射，网络配置|没有`ports:`映射，内部网络只有|`ports: "8080:8080"`映射到主机|
| **Azure应用服务** |应用设置，访问限制| VNet集成，私有端点|公共URL，无IP限制|
| **虚拟机/裸机** |防火墙规则，NSG， iptables |firewall/NSG中端口被阻塞|端口开放，公网IP绑定|
| **无服务器（功能）** |功能授权级别，API管理|`authLevel: function/admin`|`authLevel: anonymous`|
| * *。. NET / Java / Node** |启动配置，中间件管道|`app.UseAuthentication()`强制|无认证中间件，或禁用认证|
| **Python (FastAPI/Flask)** |中间件，依赖注入|`Depends(get_current_user)`对|路由没有授权依赖，打开|路由**⛔永远不要根据“似乎合理的”或架构假设来分配先决条件。**以实际部署配置为准。相同的组件必须在不同的运行中获得相同的先决条件，因为配置在运行之间不会改变。

* *常见的侵犯:* *
-将`Internal Network`分配给具有入口路由的组件→隐藏真实的外部暴露
-假设数据库是“内部的”，而不检查它们是否有公共端点或入口路由
-假设ML模型服务器是“内部的”，当它们可能暴露于直接推理请求时

CVSS-to-Tier一致性检查（强制性）

**分配CVSS矢量和层后，交叉检查矛盾：**

| CVSS度量值|值|分级含义||-------------|-------|------------------|
|`AV:L`（攻击向量：Local） |需要本地接入| **不能为Tier 1** -必须为T2或T3 |
|`AV:A`（攻击向量：相邻）|需要相邻网络| **不能是tier1 ** -必须是T2或T3 |
|`AV:P`（攻击方式：物理）|需要物理接入| **必须为三层** |
|`PR:H`（特权要求：高）|需要admin/privileged访问| **不能是tier1 ** -必须是T2或T3 |
|`PR:L`(Privileges Required: Low) |需要认证用户| **不能为tier1 ** -必须为T2 |
|`PR:N`+`AV:N`|无特权，网络可访问|一级候选（确认没有部署覆盖）|

⚠️**如果发现有`AV:L`和`Tier 1`，这总是一个错误。**修复：
—将分级修改为“T2/T3”（仅限localhost业务的正确方法），或
如果CVSS的AV是可以通过网络访问的，就把它改成`AV:N`⚠️**如果发现有`PR:H`和`Tier 1`，这总是一个错误。**管理要求的发现是T2最低。

部署上下文影响分级分类

**CRITICAL：当有特定的部署条件时，此部分将覆盖上述默认的分级规则

在分配层之前，从代码、文档和体系结构中确定系统的部署模型。在`0.1-architecture.md`中记录**部署分类**和**组件公开表**（参见`skeleton-architecture.md`）。

**部署分类及其层含义：**

|分类|描述| T1允许？|最低先决条件||----------------|-------------|-------------|------------------|
|`LOCALHOST_DESKTOP`|Console/GUI应用程序，没有网络监听器（或仅限本地主机），单用户工作站|❌** no ** -所有发现T2+ |`Host/OS Access`（T3）或`Local Process Access`(T2) |
|`LOCALHOST_SERVICE`|Daemon/service绑定到127.0.0.1只|❌**NO** -所有结果T2+ |`Local Process Access`(T2) |
|`AIRGAPPED`|无internet连接|❌针对网络攻击|`Internal Network`|
|`K8S_SERVICE`| Kubernetes部署ClusterIP/LoadBalancer|✅是|取决于服务类型|
|`NETWORK_SERVICE`|公共API，云端点，面向internet |✅是|`None`（如果没有授权）|`0.1-architecture.md`中的组件暴露表设置了每个组件的先决条件下限。**任何威胁或发现的先决条件不得低于表所允许的条件。此表在步骤1中填写，并绑定到所有后续分析步骤。

**遗留覆盖表（仍然适用于回退）：**

|部署指标|分级覆盖规则||---------------------|-------------------|
|仅绑定`localhost`/`127.0.0.1`|不能为T1 -需要本地接入（T2最小）|
|气隙/无互联网|将基于网络的攻击降低一级|
|单管理员工作站工具|除非被非管理员本地用户|利用，否则不能为T1
|Docker/container单机| Docker socket访问= T2（需要本地管理员）|
|命名管道/ Unix套接字|不能是T1 -需要本地进程访问|

**申请方式：**
1. 在步骤1（上下文收集）中，确定部署模型并记录在0.1-architecture.md中
2. 在步骤6/7（查找验证）中，根据上面的表检查每个T1候选项
3. 如果应用任何覆盖，则降级到T2（如果有多个，则降级到T3）
4. 在发现的描述中记录覆盖的基本原理**示例：** Kusto容器在气隙工作站上，在端口80上监听，没有授权：
—默认分类：T1（未认证，端口80）
-覆盖：localhost-only + single-admin→**T2**（攻击者需要本地访问管理工作站）

**不要覆盖**：
- Kubernetes服务（任何pod都可以到达它们→横向移动是现实的→保持T1）
-网络暴露的api（任何网络用户都可以访问→保持T1）
-云端点（公共互联网→keep T1）
—**Network-exposed API **：侦听端口上未经认证的API为tier1。

第1层的先决条件是`None`——意味着未经身份验证的外部攻击者**没有事先访问。如果利用漏洞需要本地管理访问权限、操作系统级访问权限或物理存在，则不能是第1层。

---

##查找分类

在记录每个发现之前，请验证：-[] **有确凿证据存在**：能否出示证明漏洞的config/code？
-[] **默认不安全**：您是否检查了平台是否默认启用安全？
-[] **安全基础设施检查**：您是否查找Sentry/cert-manager/Vault/etc.？
-[] **显式vs隐式**：安全是显式禁用，还是只是没有显式启用？
-[] **参考平台文档**：当不确定时，参照官方文档进行验证

* *分类结果:* *
- **确认**：漏洞的确凿证据→文档在`3-findings.md`中发现
- **需求验证**：无法确认但存在潜在风险→添加到`0-assessment.md`中的“需求验证”
- **没有发现**：确认安全默认或明确启用→不记录

---

##严重性标准

### SDL bug严重性
按：https://www.microsoft.com/en-us/msrc/sdlbugbar对每个发现进行分类CVSS 4.0分
使用CVSS v4.0基本分数（0.0-10.0）与向量字符串。
参考:https://www.first.org/cvss/v4.0/specification-document# # # CWE
指定常见弱点枚举ID和名称。
参考:https://cwe.mitre.org/# # # OWASP
如果适用，映射到OWASP Top 10:2025类别（A01-A10）。
**总是使用`:2025`后缀**（例如，`A01:2025`），从不使用`:2021`。
参考:https://owasp.org/Top10/2025/###补救努力
- **低**：配置更改，标志切换或单文件修复
- **中等**：多文件代码更改，新的验证逻辑或依赖项更新
- **高**：架构变更、新组件或跨团队协调###跨范围规则
- **外部服务** (AzureOpenAI, AzureAD, Redis, PostgreSQL) **做得到**跨步节-他们是从你的系统的角度来看攻击面
- **外部参与者**（操作员，终端用户）**不获得**跨步部分-他们是威胁来源，而不是目标
-如果你有20个元素，其中2个是外部角色，你要写18个STRIDE部分

**⚠️不包括时间估计。**永远不要在输出中添加“（小时）”，“（天）”，“（周）”，“~1小时”，“~2小时”或任何duration/effort-to-fix估计值。工作级别（Low/Medium/High）就足够了。缓解类型（与owasp一致）
- **重新设计**：通过改变架构消除威胁（OWASP：避免）
- **标准缓解**：应用众所周知的、经过验证的安全控制（OWASP：缓解）
- **自定义缓解**：实现针对此系统的定制代码修复（OWASP：缓解）
- **现有控制：团队已经建立了一个控制来解决这个威胁-记录它（OWASP：修复）
- **接受风险**：确认并记录剩余风险（需要证明）（OWASP：接受）
- **转移风险**：将责任转移到user/operator/third-party（例如，配置选择，SLA） （OWASP：转移）