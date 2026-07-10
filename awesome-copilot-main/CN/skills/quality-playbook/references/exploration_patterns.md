# Bug发现的探索模式

该参考定义了阶段1在代码库探索期间应用的探索模式。当探索停留在子系统或体系结构级别时，这些模式针对的是最常被遗漏的bug类。

需求问题是修复成本最高的，因为它们直到实现之后才会被捕获。探索阶段是需求引出——它决定了代码审查和规范审计要寻找什么。一个从来没有派生出来的需求就是一个从来没有被发现过的bug。这些模式的存在是为了系统地揭示广泛勘探所遗漏的需求。

每个模式都包括一个定义、它所针对的错误类、来自不同领域的各种示例以及EXPLORATION.md的预期输出格式。**重要：这些模式是对自由探索的补充，而不是替代。**阶段1开始于由领域知识和代码库理解驱动的开放式探索。在开放的探索之后，应用下面的模式作为结构化的第二步来捕获特定的bug类。如果你发现自己只是在寻找模式所描述的东西，那你就用错了模式。模式是在您对代码库的风险形成自己的理解之后运行的检查表。

---

模式1：回退和退化路径奇偶性

# # #定义

当代码为实现相同的目标提供了多种策略（一条主要路径和一条或多条回退路径）时，每个回退路径必须保持与主要路径相同的行为不变性。回退可以使用不同的机制，但可观察契约必须是等价的。

Bug类与主路径相比，后备路径编写较晚，测试较少，审查也较少。它们通常省略主路径执行的步骤（验证、清理、索引分配、资源释放），因为开发人员复制了主路径，并针对“降级”情况对其进行了简化。结果是一个函数在一般情况下正常工作，但在回退激活时违反了它的契约。

跨域示例- **认证：** web服务尝试OAuth令牌验证，回落到API密钥查找，回落到会话cookie。每个回退必须强制相同的授权范围。Bug: API密钥回退跳过范围验证并授予完全访问权限。
—**连接池：**数据库客户端尝试主连接池，回退到备连接池，回退到创建一次性连接。每个路径必须应用相同的超时和事务隔离设置。Bug：一次性连接回退使用驱动程序默认的隔离级别，而不是配置的隔离级别。
- **资源分配：**内存分配器尝试快速的slab路径，回落到缓慢的页面级路径。两者都必须对敏感字段进行零初始化。Bug：缓慢路径返回未初始化的内存，因为零填充仅在slab快速路径中。
- **HTTP重定向处理：**客户端遵循重定向当重定向越过原点边界时，必须剥离安全敏感的标头（Authorization, Proxy-Authorization, cookies）。Bug：重定向路径剥离Authorization而不是proxy -Authorization，将代理凭证泄漏到重定向的源。
- **序列化回退：**消息代理尝试二进制序列化，回落到JSON，回落到字符串编码。每个路径必须保持相同的字段顺序和空处理语义。Bug: JSON回退会无声地丢弃二进制序列化保留的空字段。如何应用

对于每个核心模块，请查找：尝试一种方法然后切换到另一种方法的条件链，在运行时选择多种实现的strategy/adapter模式，每次尝试使用不同策略的重试逻辑，功能决定运行哪条代码路径的功能协商级联，必须保留或剥离标头的HTTPredirect/retry逻辑。

对于找到的每个级联：
1. 列出主路径和每个回退路径。
2. 对于每个回退，检查它是否执行与主回退相同的关键操作（验证、资源设置、索引分配、清理、错误报告、头剥离、资源释放）。
3. 在主服务器中存在但在回退服务器中缺失的任何操作都是候选需求。EXPLORATION.md输出格式```
## Fallback Path Analysis

### [Name of cascade]
- **Primary path:** [function, file:line] — [what it does]
- **Fallback 1:** [function, file:line] — [what it does, what differs]
- **Fallback 2:** [function, file:line] — [what it does, what differs]
- **Parity gaps:** [specific operations present in primary but missing in fallback]
- **Candidate requirements:** REQ-NNN: [fallback must do X]
```
---

模式2:Dispatcher返回值的正确性

# # #定义

当函数分派输入类型或条件并且必须返回状态值时，返回值必须对每个输入组合都是正确的-不仅仅是主要情况。处理多种事件类型、请求类型或状态转换的分派器在边缘组合中特别容易出现返回值错误。

Bug类

调度程序通常是针对常见情况编写和测试的。当主事件触发时，返回值是正确的。但是，当出现不寻常的组合（只有次要事件、根本没有事件、多个并发事件）时，返回值逻辑可能是错误的——对于已处理的事件返回“未处理”，对于部分失败返回成功，或者从以前的迭代返回过时的值。

跨域示例- **HTTP中间件：**请求调度程序检查身份验证，速率限制和路由。当已经设置了速率限制触发器但身份验证时，调度程序返回验证状态码，而不是速率限制状态码。Bug：速率限制请求得到401而不是429。
- **CORS处理程序链：** CORS预飞行处理程序设置400（拒绝），然后missing-OPTIONS-handler路径设置404，然后AFTER处理程序标准化404→200（意味着允许的起源）。Bug：拒绝的预航班得到200，因为状态被下游处理程序覆盖了。
- **事件循环：**poll/select循环处理read-ready， write-ready和error条件。当在套接字上仅触发错误条件而没有挂起的读取时，循环返回“no events”，因为read-ready检查为false。错误：连接错误被静默忽略。
- **状态机转换：** A状态机dIspatch函数处理有效转换、无效转换和无操作转换。当发生无操作转换（当前状态==目标状态）时，该函数返回用于无效转换的错误代码。Bug：幂等操作在应该成功的时候失败了。
- **中断处理程序：**硬件中断处理程序检查多个事件类型（数据准备，配置更改，错误）。当只有辅助事件触发时（例如，没有数据的配置更改），处理程序返回“not mine”，因为主事件检查失败，辅助路径没有设置已处理标志。Bug：合法的次要事件被报告为虚假的。如何应用

对于每个核心模块，寻找：带有返回状态的switch/case或if-else链的函数、处理多种事件类型的interrupt/event处理程序、在返回前检查多个条件的请求调度程序、状态机转换函数、多个处理程序写入相同响应状态的中间件链。

对于找到的每个调度程序：
1. 枚举所有的输入组合（不仅仅是那些带有显式大小写标签的组合——还有隐含的“else”和“default”路径）。
2. 对于每个组合，通过整个处理程序链（而不仅仅是直接函数）跟踪返回值。
3. 返回值与预期语义不匹配的任何组合都是候选需求。EXPLORATION.md输出格式```
## Dispatcher Return-Value Analysis

### [Function name] at [file:line]
- **Input types:** [list of conditions/events the function dispatches on]
- **Combinations checked:**
  - [Condition A only]: returns [X] — correct/incorrect because [reason]
  - [Condition B only]: returns [X] — correct/incorrect because [reason]
  - [Both A and B]: returns [X] — correct/incorrect because [reason]
  - [Neither A nor B]: returns [X] — correct/incorrect because [reason]
- **Candidate requirements:** REQ-NNN: [function must return Y when only B fires]
```
---

模式3：跨实现契约一致性

# # #定义

当多个函数为不同的上下文（不同的传输、不同的后端、不同的协议版本）实现相同的逻辑操作时，它们都应该满足相同的规范需求。规范中强制要求的步骤必须出现在每个实现中——一个实现中缺失的步骤出现在另一个实现中是一个强烈的错误信号。

Bug类当在多个地方实现相同的操作时，每个实现通常由不同的开发人员或在不同的时间编写。规范说“重置必须等待完成”，实现A的开发人员编写了等待循环，但实现B的开发人员只编写了重置触发器，而忘记了等待。当单独测试实现B时，这个错误是不可见的，因为它在快速硬件上“工作”——竞争条件只在负载或慢速设备上显示。

跨域示例- **设备复位：**规范说“驱动程序必须写零，然后轮询，直到状态寄存器读回零。”PCI实现包括轮询循环。MMIO实现写零，但不轮询。Bug: MMIO复位可以与重新初始化竞争。
- **数据库驱动程序：**连接关闭规范表示“驱动程序必须发送一个终止消息，等待确认，然后释放套接字。”PostgreSQL驱动程序可以完成这三个任务。MySQL驱动程序发送终止消息并释放套接字，而不等待确认。Bug：服务器可能会在socket被重用后处理终止。
- **HTTP报头编码：**报头类构造函数将原始字节解码为RFC 7230中的Latin-1。突变方法（`__setitem__`）将值编码为UTF-8。Bug：通过get-then-set往返Latin-1报头会损坏值，因为编码改变了。
Ca - * *cache规范说“invalidation必须删除条目并通知所有订阅者。”内存缓存可以同时完成这两项工作。分布式缓存删除条目，但不广播通知。Bug：其他节点提供陈旧的数据。
- **文件锁定：**存储规范说“锁获取必须设置超时并在失败时清理。”本地文件系统实现设置超时。NFS实现使用无超时的阻塞锁。Bug: NFS锁争用可能无限期挂起进程。如何应用

对于每个核心模块，请查找：在多个文件或类中实现的相同操作名称、跨不同后端的interface/trait实现、相同消息的特定于协议版本的实现、相同生命周期操作的特定于传输的实现、相同逻辑操作的构造函数与突变实现。

对于每一对（或一组）实现：
1. 确定它们共享的规范需求。
2. 列出规范中的强制性步骤。
3. 检查每个步骤的每个实现。
4. 在一个步骤中出现而在另一个步骤中缺失的任何步骤都是候选要求。**检查每个交叉运输操作，而不仅仅是最明显的操作。**如果一个代码库有多个传输（PCI， MMIO, vDPA）或后端（PostgreSQL, MySQL），列举所有具有跨实现等价的操作-重置，中断处理，特性协商，队列设置，配置访问-并检查每一个。您发现的第一个跨实现差距很少是唯一的。一种常见的故障模式是彻底分析复位，然后跳过中断调度，它们具有相同的交叉传输结构。EXPLORATION.md输出格式```
## Cross-Implementation Consistency

### [Operation name] — [spec reference]
- **Implementation A:** [function, file:line] — performs steps: [1, 2, 3]
- **Implementation B:** [function, file:line] — performs steps: [1, 3] (missing step 2)
- **Gap:** [Implementation B missing step 2: description]
- **Candidate requirements:** REQ-NNN: [all implementations of X must perform step 2]
```
---

模式4：枚举和表示完整性

# # #定义

当代码库维护一个可识别值的封闭集时——一个switch/case白名单、一个有效常量数组、一个enum/tagged-union定义、一个trait/visitor方法族、一组模式关键字、一个可接受条目的注册表——规范、上游定义或库自己的公共API表面说应该接受的每个值都必须出现在该集中。不在集合中的值将被静默丢弃、拒绝或错误处理，并且在调用站点上不可见条目的缺失。

Bug类闭集只写一次，很少重新访问。当一个新功能被添加到规范或上游标头时，定义该功能的代码（常量、特性标志、枚举变量）被更新，使用该功能的代码也被更新，但是限制该功能是否在过滤步骤中存活的闭集被遗忘了。这个功能似乎是被支持的——它被定义了，被协商了，被使用了——但它被一个没有人记得更新的过滤功能悄悄地剥夺了。这个bug在正常测试中是不可见的，因为这个功能根本就没有激活，而没有激活看起来就像“另一端不支持它”。这种模式还涵盖了必须镜像公共API的内部表示。如果库的公共API接受i128/u128整数，但内部缓冲表示只有i64/u64的变体，则通过缓冲区的值将被静默截断或拒绝—即使公共API承诺处理它们。

跨域示例- **特征协商过滤器：**传输层维护一个switch/case白名单的特征位应该通过过滤。一个新特性（`RING_RESET`）被添加到UAPI头中，供高级代码使用，但从未添加到白名单中。Bug：该特性在协商过程中被静默清除，禁用了驱动程序声称支持的功能。
序列化库的公共`Deserializer`特性支持`deserialize_i128()`/`deserialize_u128()`。非标记和内部标记的枚举反序列化使用的内部缓冲表示（`Content`enum）只有`I64`/`U64`的变体。Bug：即使公共API声称支持128位整数，通过缓冲区的128位整数也会被拒绝，并出现“i128无变体”错误。
—**Schema关键字导入器：**验证库导入JSON Schema文档。规范定义了`uniqueItems`、`contains`、xq用于数组的Z15xqz，`maxContains`。导入器识别这些关键字（没有解析错误），但不强制执行它们。Bug：导入的模式静默地接受违反原始约束的数组。
- **权限系统：**授权中间件维护一个可识别的权限字符串数组。一个新的权限（`audit:write`）被添加到角色定义中，但没有添加到中间件的白名单中。Bug：具有`audit:write`角色的用户被静默拒绝访问，因为中间件不识别该权限。
- **协议消息类型：**消息路由器为识别的消息类型维护switch/case调度。一种新的消息类型被添加到协议规范和序列化层中，但没有添加到路由器中。Bug：新的消息类型在路由器的默认情况下被静默丢弃，发送者不会收到任何错误。如何应用

对于每个核心模块，请查找：switch/case语句，带有显式的大小写标签和默认值drops/clears/rejects，用于过滤或验证的可接受值的数组或集合，必须手动添加新条目的注册函数，镜像规范或公共API的enum/tagged-union定义，每个方法处理一个变体的trait/visitor方法族，必须处理规范定义的每个关键字的模式导入器，内部表示(缓冲区，IR，AST)，它必须覆盖公共接口的全部范围。对于找到的每个闭集：
1. 确定确定哪些值应该有效的权威源。这可以是：规范、头文件、上游枚举、协议定义、或库自己的公共API表面（trait方法、函数签名、类型定义）。
2. 机械地提取闭集（将大小写标签、枚举变量、访问者方法、数组项或模式关键字保存到文件中）。
3. 将提取的集合与权威源进行比较。权威源中没有出现在封闭集中的每个值都是候选需求。**呼叫者补偿不能作为丢失条目的借口。**如果shared/generic函数中的闭集缺少一个条目，这是一个bug——即使特定的调用者在函数运行后通过恢复值来补偿。补偿是一种变通办法，而不是解决办法。任何不知道以静默方式进行补偿的新调用者都会继承该bug。将每个丢失的条目报告为发现，并注意哪些调用者（如果有的话）进行了补偿，但不要因为补偿而忽略了发现。EXPLORATION.md输出格式```
## Enumeration/Representation Completeness

### [Function/type name] at [file:line]
- **Purpose:** [what this closed set gates — e.g., "feature bits that survive transport filtering" or "integer variants the buffer can hold"]
- **Authoritative source:** [where valid values are defined — e.g., "include/uapi/linux/virtio_config.h" or "public Deserializer trait methods"]
- **Extracted entries:** [list of values in the closed set, or reference to mechanical extraction file]
- **Missing entries:** [values present in the authoritative source but absent from the closed set]
- **Candidate requirements:** REQ-NNN: [closed set must include X]
```
---

模式5:API表面一致性

# # #定义

当相同的逻辑操作可以通过多个API表面（直接方法vs.view/wrapper，构造函数vs. mutator，同步vs.异步变量，主API vs.便利别名）执行时，所有的表面都必须为相同的输入产生相同的可观察行为。相同操作的两条路径之间的分歧是一个bug，因为调用者合理地期望无论使用哪个表面都有一致的行为。

Bug类库通常通过多个接口公开相同的底层数据：一个直接方法和一个集合视图（`add()`vs.`asList().add()`）、一个构造函数和一个setter、一个同步和异步变体。这些表面在不同的时间（通常由不同的开发人员）实现，并且它们的边缘情况处理不同—特别是在null/sentinel值、编码、排序和错误报告方面。这种差异在常规测试中是不可见的，因为每次操作通常只测试一个表面。

跨域示例—**JSON空值处理：**`JsonArray.add(null)`将空值转换为`JsonNull.INSTANCE`并成功。`JsonArray.asList().add(null)`抛出`NullPointerException`，因为视图的包装器无条件拒绝null。Bug：同一个操作的两个方法有矛盾的null语义。
- **HTTP报头编码：**`Headers([(b"X-Custom", b"\xe9")])`从拉丁1字节构造报头。`headers["X-Custom"] = b"\xe9"`将值存储为UTF-8。Bug：通过get-then-set往返报头会静默地改变编码。
- **WebSocket协议协商：**`WebSocketUpgrade::protocols()`返回一个`BTreeSet<HeaderValue>`，它对客户端的优先顺序协议列表进行排序和重复数据删除。Bug：应用程序看到的顺序与客户端发送的顺序不同，破坏了基于首选项的协商。
- **配置选项传播：**`res.sendFile(path, { etag: false })`应该为这个响应禁用ETag。但是代码在传递给底层`send`模块之前将选项转换为布尔值，失去了“强”与“弱”ETag模式。错误:体育r-call ETag配置被静默忽略或有损转换。
—**映射重复检测：**`map.put(key, value)`返回前一个值来表示重复。当前一个值合法地为`null`时，`put()`返回`null`—与“没有前一个条目”返回的值相同。Bug：当第一个值为null时，不会检测到重复的键。如何应用

对于每个核心模块，查找：`asList()`、`asMap()`、`unmodifiableView()`、`stream()`、`iterator()`等方法返回的view/wrapper对象；构造函数与变异方法对；同一操作的同步与异步变体；委托给主实现的便利别名；接受options/configuration对象的方法。

对于每一对曲面：
1. 确定它们共享的逻辑操作。
2. 在两个表面上测试相同的边缘情况输入（空、空、边界值、特殊字符、顺序敏感数据）。
3. 行为上的任何差异（不同的异常、不同的编码、不同的顺序、一个成功而另一个失败）都是候选需求。EXPLORATION.md输出格式```
## API Surface Consistency

### [Operation name] — [two surfaces compared]
- **Surface A:** [method, file:line] — [behavior on edge input]
- **Surface B:** [method, file:line] — [behavior on same edge input]
- **Divergence:** [what differs — exception type, encoding, ordering, null handling]
- **Candidate requirements:** REQ-NNN: [both surfaces must behave equivalently for input X]
```
---

模式6：规范结构化解析保真度

# # #定义

当代码解析由正式语法或规范定义的值（HTTP头、url、MIME类型、CLI标志、JSON Schema关键字、文件路径）时，解析必须匹配语法的实际规则。快捷方式（子字符串匹配、精确相等、错误的分隔符、没有边界检查的前缀匹配）产生的解析器可用于常见输入，但在有效的边缘情况下失败或接受无效输入。

Bug类开发人员经常实现“足够好”的解析器来处理常见情况：`header.contains("gzip")`而不是用逗号进行标记和修剪空白，`url.startsWith("/api")`而不是检查路径段边界，`connection == "Upgrade"`而不是不区分大小写的标记列表成员。这些快捷方式通过了所有单元测试，因为测试使用了格式良好的输入，但是它们在现实世界的边缘情况下会失效，比如`gzip;q=0`（显式拒绝）、`Connection: keep-alive, Upgrade`（令牌列表）或`/api-docs`（没有边界的前缀匹配）。

跨域示例—**HTTP Accept-Encoding:**中间件检查`accept.contains("gzip")`来决定是否压缩。这将匹配`gzip;q=0`（客户端明确拒绝gzip）和`xgzip`（不是有效的编码）。Bug：响应被压缩时，客户端说不。
- **WebSocket连接头：**代码检查`connection == "Upgrade"`（精确匹配）。根据RFC 7230，`Connection`是一个逗号分隔的令牌列表；`Connection: keep-alive, Upgrade`有效，但精确匹配失败。错误：有效的WebSocket升级被拒绝。
- **SPA回退路由：**单页应用程序处理程序匹配路径与`path.startsWith("/app")`。它匹配`/app/users`（正确）和`/api-docs`（不正确的兄弟路由）。Bug: API文档请求被SPA处理程序吞噬。
- **MIME类型参数处理：**内容协商比较`text/html;level=1`与处理程序键，但在匹配之前剥离参数。Bug：在响应Content-Type中丢失了协商时选择的`level=1`参数。—**URL主机规范化：**代码通过检查`host.startsWith("xn--")`来检测国际化域名。根据IDNA，只有单独的标签以`xn--`开头；`foo.xn--example.com`在中间有punycode标签。Bug：国际化子域不解码。如何应用

对于每个核心模块，查找：rfc或规范定义的值（头，url， MIME类型，编码名称）上的字符串比较，结构化值上的`contains()`/`indexOf()`/`startsWith()`/`endsWith()`，规范要求不区分大小写的区分，分割错误的分隔符或根本不分割，没有路径段或标记边界的prefix/suffix匹配。

对于找到的每个解析器：
1. 确定定义语法的规范（RFC、ABNF、JSON模式规范、POSIX等）。
2. 检查实现是否处理：标记列表（逗号分隔）、引号字符串、参数（分号分隔）、大小写折叠、空白修整、边界条件。
3. 构造一个根据规范有效但会使实现的快捷解析器失败的输入。该输入是候选测试用例，解析缺口是候选需求。EXPLORATION.md输出格式```
## Spec-Structured Parsing

### [Parser location] at [file:line]
- **Spec:** [which grammar/RFC/standard defines the format]
- **Implementation technique:** [contains/equals/startsWith/split-on-X]
- **Spec-valid input that breaks the parser:** [concrete example]
- **Why it breaks:** [substring match includes invalid case / missing case folding / etc.]
- **Candidate requirements:** REQ-NNN: [parser must tokenize per RFC NNNN §N.N]
```
---

模式7：组合和挂载上下文感知

# # #定义

当代码在复合上下文中运行时——安装在子路由上，嵌套在父模块中，作用域为子容器，由框架适配器包装——框架通常维护活动状态的“规范”表示（活动上下文现在说的是真的）以及来自外部调用站点的“原始”表示（外部调用者最初传递的）。当需要规范化时，读取或写入原始表示的代码（反之亦然）在外部级别正确工作，因为它们恰好相同，但在组合级别静默失败，因为它们偏离。

Bug类代码是在外部级别编写和测试的——顶级路由、根模块、单租户部署、默认作用域——其中规范状态和原始状态是相同的。当相同的代码在组合的上下文中运行时，框架会更新规范状态（挂载的子路径、作用域记录器、事务作用域连接、语言环境感知比较器），但原始状态仍然反映外部调用。缺陷表现在两个对称的方向：一个函数*读取*需要规范化的原始状态，看到陈旧的数据并产生无声漂移（从不匹配，泄漏父上下文，返回错误的输出）；一个从规范状态*写入*一个面向外部的值的函数，当raw需要时，会产生消费者无法使用的输出（删除mount前缀，返回父客户端无法遵循的子相对路径）。无论哪种方式，测试套件典型地执行外部级别只有而且从来没有看到分歧。跨域示例- **HTTP路由中间件（mount-context）：**中间件将请求路径与配置的端点进行比较，读取`r.URL.Path`。当挂载在子路由上时，框架的规范“活动路由路径”（例如，chi中的`RoutePath`， Express子应用中的`req.url`）是子相对路径，而`r.URL.Path`仍然是完整的URL路径。Bug：中间件永远不会在挂载的子进程中匹配，因为它读取了错误的路径表示。
- **数据库事务上下文：**存储库方法通过连接池打开自己的连接。当在显式事务中调用时，框架的规范“当前事务”上下文是显式的上下文，但该方法直接从连接池中读取。Bug：方法的写操作不参与周围的事务；回滚会留下孤立行。
- **日志上下文传播：**库通过`logging.getLogger(__name__)`记录日志。在内部调用时如果异步任务或工作池的作用域是基于上下文变量的关联ID，则日志记录器不会读取上下文变量。Bug：库的日志行缺少框架传播的相关ID，破坏了可追溯性。
- **地区敏感比较：**排序函数使用`str.lower()`进行不区分大小写的比较。当在语言环境感知的上下文中（土耳其语“i”/“İ”/“yi”语义）调用时，将设置框架的规范语言环境，但`str.lower()`读取默认语言环境。Bug：相等比较会根据通常活动的语言环境而静默地有所不同。
- **授权范围继承：** ACL检查读取`request.user`（原始认证主体）。当在模拟上下文中调用时，框架的规范`request.effective_user`是模拟的主体，但检查仍然读取原始主体。Bug：特权升级-检查授权了错误的主体。如何应用

识别每个读取或写入状态的函数或组件，在组合*下*可以是标准的或原始的。检查是：当调用者在更大的上下文中组成时，该代码路径是否不变地运行，如果是，它观察（或产生）的状态是否相应地改变？

**消除模式4中的歧义。模式4（枚举和表示完整性）是关于值的闭集：错误是“从识别器的闭集中丢失值”。模式7是关于状态变量的选择：错误是“函数在组合时读取或写入错误的状态表示”。如果两个框架似乎都适用，则选择REQ更可测试的框架。这两种图案很少在同一缺陷上重叠；当他们这样做时，经典vs原始框架通常更直接地指向修复。* *预算。**每通过3-5个影响最大的组合接缝。如果从这个模式中出现了超过10个候选对象，那么网太宽了，模式被过度应用了——用更严格的“这个框架在组合下实际维护什么”过滤器重新访问第1步。

对于找到的每个候选人：1. **识别读和写的规范和原始表示。**在组合下，框架维持什么作为“此关注点的活动状态”？这个函数实际读的是什么？然后问输出的对称问题：当这个函数构造一个向外流动的值（一个重定向目标、一个派生路径、一个记录的关联ID、一个授权的资源句柄）时，这个值是根据它的消费者的正确表示构建的吗？读端和写端缺陷同样常见；检查两个方向。
2. **跟踪合成缝。**框架在哪里更新规范状态？功能是否在更新站点的下游？它是从规范还是原始的表述中读取的？
3. **构建成分检验。**这段代码在父组件（挂载路由器，嵌套事务，sc）中运行的最小示例是什么动态日志记录器，模拟上下文)？函数的行为是否与外部行为相匹配，或者它是否会无声地漂移？
4. **记录发生的事情。**在组合下漂移的函数是候选要求：“函数`<X>`必须读取`<canonical_state>`（而不是`<raw_state>`）[或者为向外输出写`<raw_state>`]，以便在`<parent_context>`内部组合时行为保持正确。”值得明确检查的常见组合接缝：

**路由框架：**`RoutePath`/`req.url`/`request.path_info`相对于原始URL。每个级别的挂载都会更新规范路径；原始url则不然。
- **事务管理器：**显式事务上下文与连接池的自动提交默认值。组合代码必须读取活动事务。
- **日志/跟踪：**上下文变量作用域的关联id与线程本地或默认的日志记录器。组合代码必须读取上下文变量。
- **授权/模拟：**有效主体对原始用户。组合代码必须读取有效主体。EXPLORATION.md输出格式```
## Composition and Mount-Context Analysis

### [Function/component name] at [file:line]
- **Composes inside:** [parent context — e.g., "any chi router that calls Mount() to attach this middleware"]
- **Canonical representation:** [what the framework maintains under composition — e.g., "rctx.RoutePath, the active mounted path"]
- **Raw representation read by this code:** [what the function actually reads — e.g., "r.URL.Path, the full request URL"]
- **Drift scenario:** [smallest composition example that exposes the divergence — e.g., "router.Mount("/api", child) where child uses this middleware to serve /ping"]
- **Observable failure:** [what wrong behavior results — e.g., "404 instead of the expected response"]
- **Candidate requirements:** REQ-NNN: [function MUST read canonical state under composition]
```
---

##扩展列表

这些模式是通过分析跨越7种语言的11个开源存储库中的56个已确认的bug得出的。每一种模式都代表了一类需求，而这些需求是广泛的体系结构总结总是遗漏的。

添加一个新模式：
1. 确定一个在开发过程中被遗漏的bug，但使用特定的分析技术可以发现它。
2. 概括技术：对于代码，浏览器应该问哪些问题？
3. 提供至少5个来自不同领域的不同示例（并非全部来自同一项目）。
4. 定义EXPLORATION.md的预期输出格式。
5. 将模式添加到该文件中，并将相应的部分添加到SKILL.md中的EXPLORATION.md模板中。

我们的目标是建立一个系统探索技术库，随着时间的推移，这些技术会随着新bug类的发现而积累起来。