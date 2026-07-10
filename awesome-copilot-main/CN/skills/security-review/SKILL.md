---
name: security-review
description: 'AI-powered codebase security scanner that reasons about code like a security researcher — tracing data flows, understanding component interactions, and catching vulnerabilities that pattern-matching tools miss. Use this skill when asked to scan code for security vulnerabilities, find bugs, check for SQL injection, XSS, command injection, exposed API keys, hardcoded secrets, insecure dependencies, access control issues, or any request like "is my code secure?", "review for security issues", "audit this codebase", or "check for vulnerabilities". Covers injection flaws, authentication and access control bugs, secrets exposure, weak cryptography, insecure dependencies, and business logic issues across JavaScript, TypeScript, Python, Java, PHP, Go, Ruby, and Rust.'
---
#安全审查

一个由人工智能驱动的安全扫描器，可以像人类安全一样分析你的代码库
研究人员将跟踪数据流，理解组件交互，并捕获
模式匹配工具遗漏的漏洞。

何时使用此技能

当请求涉及：—扫描代码库或文件是否存在安全漏洞
-运行安全审查或漏洞检查
-检查SQL注入、XSS、命令注入或其他注入漏洞
-在代码中查找暴露的API密钥、硬编码秘密或凭据
—审计已知cve的依赖关系
—检查认证、授权或访问控制逻辑
—检测不安全密码或弱随机性
—执行数据流分析，跟踪用户对危险sink的输入
-任何请求短语，如“我的代码安全吗？”，“扫描此文件”，或“检查我的repo的漏洞”
—运行`/security-review`或`/security-review <path>`##这个技能是如何工作的与匹配模式的传统静态分析工具不同，该技能：
1. **像安全研究人员一样阅读代码-理解上下文，意图和数据流
2. **跟踪跨文件** -跟踪用户输入如何通过应用程序移动
3. **自我验证发现** -重新检查每个结果以过滤假阳性
4. **分配严重性等级** -关键/高/中/低/信息
5. **建议有针对性的补丁** -每个发现都包括一个具体的修复
6. **需要人工审批** -没有任何东西是自动应用的；你总是先复习

##执行流程

每次**按顺序**执行以下步骤：###步骤1 -范围分辨率
确定要扫描的内容：
—如果提供了路径（`/security-review src/auth/`），只扫描该作用域
-如果没有给出路径，从根目录开始扫描整个项目
-识别所使用的语言和框架(检查package.json，requirements.txt，
走了。mod、Cargo.toml、pom.xml、Gemfile、composer.json等)
-读取`references/language-patterns.md`以加载特定语言的漏洞模式

###步骤2 -依赖审计
在扫描源代码之前，先审核依赖项（快速获胜）：
- **Node.js**：检查`package.json`+`package-lock.json`已知的漏洞包
- **Python**：检查`requirements.txt`/`pyproject.toml`/`Pipfile`—**Java**：检查`pom.xml`/`build.gradle`—**Ruby**：检查`Gemfile.lock`- **Rust**：检查`Cargo.toml`—**Go**：查看`go.sum`-标记软件包与已知的cve，弃用的加密库，或可疑的旧固定版本
-阅读`references/vulnerable-packages.md`的策划观察列表###步骤3 -秘密和曝光扫描
扫描所有文件（包括config， env,CI/CD, Dockerfiles, IaC）：
-硬编码API密钥，令牌，密码，私钥
-`.env`文件意外提交
—注释或调试日志中的秘密
-云凭证（AWS， GCP, Azure, Stripe， Twilio等）
-嵌入凭据的数据库连接字符串
-阅读`references/secret-patterns.md`的正则表达式模式和熵启发应用

###步骤4 -漏洞深度扫描
这是核心扫描图。关于代码的原因-不要只是模式匹配。
阅读`references/vuln-categories.md`了解每个类别的详细信息。

* * * *注入缺陷
- SQL注入：原始查询与字符串插值，ORM误用，二阶SQLi
XSS：未转义输出，危险的setinnerhtml， innerHTML，模板注入
—命令注入：exec/spawn/system，用户输入
—LDAP、XPath、Header、日志注入**认证和访问控制**
—敏感端认证缺失
-对象级授权失效（BOLA/IDOR）
JWT的弱点（无，弱秘密，无过期验证）
-会话固定，缺少CSRF保护
-特权升级路径
-质量分配/参数污染

* * * *数据处理
—日志、错误消息或API响应中的敏感数据
-在静态或传输中缺少加密
-不安全反序列化
—路径遍历/目录遍历
- XXE （XML外部实体）处理
- SSRF（服务器端请求伪造）

* *加密* *
—出于安全考虑，可以使用MD5、SHA1、DES
-硬编码iv或盐
弱随机数生成（用于令牌的Math.random()）
—缺少TLS证书验证* * * *业务逻辑
-比赛条件（TOCTOU）
—金融计算中出现整数溢出
-缺少对敏感端点的速率限制
-可预测的资源标识符

###步骤5 -跨文件数据流分析
每个文件扫描后，执行**全面审查**：
-从入口点跟踪用户控制的输入（HTTP参数，报头，正文，文件上传）
一直到sink （DB查询，exec调用，HTML输出，文件写入）
-识别只有在一起查看多个文件时才会出现的漏洞
—检查服务或模块之间是否存在不安全的信任边界###步骤6 -自我验证通过
对于每个发现：
1. 用新的眼光重新阅读相关代码
2. 问：“这真的是可利用的吗？还是我遗漏了一些消毒措施？”
3. 检查是否有框架或中间件已经在上游处理此问题
4. 降级或丢弃不是真正漏洞的发现
5. 指定最终严重性：临界/高/中/低/提示

###步骤7 -生成安全报告
以`references/report-format.md`中定义的格式输出完整的报告。

###步骤8 -提出补丁
对于每个CRITICAL和HIGH发现，生成一个混凝土补丁：
-显示易受攻击的程式码（之前）
-显示修复后的代码
-解释什么改变了，为什么改变
—保留原有的代码风格、变量名和结构
-添加注释解释内联修复

明确声明：**“在应用之前检查每个补丁。什么都没有改变。

##严重性指南|级别|含义|示例||----------|---------|---------|
|🔴紧急|即时利用风险，可能发生数据泄露| SQLi， RCE，权限绕过|
|🟠HIGH |严重漏洞，存在攻击路径| XSS， IDOR，硬编码机密|
|🟡MEDIUM |可利用条件或链接| CSRF，打开重定向，弱加密|
|🔵低|违反最佳实践，低直接风险|详细错误，缺少标头|
|⚪INFO |观察值得注意，不是漏洞|过时依赖项（无CVE） |

##输出规则**总是**先生成调查结果汇总表（按严重性计数）
- **永远不要**自动应用任何补丁-现在的补丁只供人类审查
- **始终**包括每个发现的可信度评级（高/中/低）
- **按类别而不是按文件分组发现**
- **具体** -包括文件路径，行号，以及确切的易受攻击的代码片段
- **用简单的英语解释风险** -攻击者可以用它做什么？
-如果代码库是干净的，那么清楚地说：“没有发现漏洞”扫描的内容

##参考文件

要了解详细的检测指导，请根据需要加载以下参考文件：-`references/vuln-categories.md`-每个漏洞类别的深度引用，具有检测信号，安全模式和升级检查器
-搜索模式：`SQL injection`、`XSS`、`command injection`、`SSRF`、`BOLA`、`IDOR`、`JWT`、`CSRF`、`cryptography`、`race condition`、`path traversal`-`references/secret-patterns.md`-正则表达式模式，基于熵的检测，和CI/CD秘密风险
-搜索模式：`API key`、`token`、`private key`、`connection string`、`entropy`、`.env`、`GitHub Actions`、`Docker`、`Terraform`-`references/language-patterns.md`-针对JavaScript， Python, Java， PHP, Go， Ruby和Rust的框架特定漏洞模式
-搜索类型：`Express`、`React`、`Next.js`、`Django`、`Flask`、`FastAPI`、`Spring Boot`、`PHP`、`Go`、`Rails`、`Rust`-`references/vulnerable-packages.md`- npm, pip, Maven, Rubygems， Cargo和Go模块的CVE监视列表
-搜索类型：`lodash`、`axios`、`jsonwebtoken`、`Pillow`、`log4j`、`nokogiri`、`CVE`—`references/report-format.md`—结构化输出模板对于具有查找卡、依赖项审计、秘密扫描和补丁建议格式的安全报告
-搜索模式：`report`、`format`、`template`、`finding`、`patch`、`summary`、`confidence`