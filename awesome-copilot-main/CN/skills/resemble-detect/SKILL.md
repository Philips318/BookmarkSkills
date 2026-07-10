---
name: resemble-detect
description: Deepfake detection and media safety — detect AI-generated audio, images, video, and text, trace synthesis sources, apply watermarks, verify speaker identity, and analyze media intelligence using Resemble AI
license: Apache-2.0
compatibility: 'Requires a Resemble AI API key (https://app.resemble.ai) set as RESEMBLE_API_KEY. All media must be accessible via public HTTPS URLs — local file paths are not supported except for text detection.'
---
#相似检测-深度假检测和媒体安全

使用similai平台分析音频、图像、视频和文本，以进行合成操作、人工智能生成的内容、水印、说话者身份和媒体智能。

核心原则-铁律

**“在没有完整的检测结果的情况下，永远不要宣称媒体是真的还是假的。

不要猜测、推断或推测媒体的真实性。每个真实性声明都必须有一个完整的类似检测作业作为支持，该作业返回`label`、`score`和`status: "completed"`。如果检测到的仍然是`processing`，则等待。如果是`failed`，就说出来——不要代替你自己的判断。

##何时使用

当用户的请求涉及以下任何一项时，使用此技能：-检查音频、视频、图像或文本是否由人工智能生成或操纵
-检测深度伪造在任何媒体格式
-核实媒体的真实性或来源
识别哪个AI平台合成音频（源跟踪）
-在媒体上应用或检测水印
-分析媒体的演讲者信息，情绪，转录，或错误信息
-询问有关检测结果的自然语言问题
-根据已知的语音配置文件匹配或验证说话者身份
-检测人工智能生成或机器编写的文本
-任何提及：“深度造假”、“假检测”、“合成媒体”、“语音验证”、“水印”、“媒体取证”、“真实性检查”、“来源追踪”、“这是真的吗”、“人工智能文本”、“文本检测”

**不要将**用于文本到语音的生成，语音克隆或语音到文本的转录-这些是单独的类似功能。能力决策树

|用户想要…使用这个| API端点||-------------------------------------------------------|---------------------------|---------------------------------------|
|检查介质是否为AI-generated / deepfake | ** deepfake Detection** |`POST /detect`|
|知道*哪个AI平台*制作了假音频| **音频源追踪** |`POST /detect`带|标志
|获取扬声器信息，情感，转录从媒体| **智力** |`POST /intelligence`|
|询问完成检测| **检测智能** |`POST /detects/{uuid}/intelligence`|
|应用不可见水印到媒体| **水印应用** |`POST /watermark/apply`|
|检查介质中是否存在水印| **水印检测** |`POST /watermark/detect`|
|根据已知配置文件验证说话人的身份| **身份搜索** |`POST /identity/search`|
|检查文本是否为ai生成| **文本检测** |`POST /text_detect`|
|创建**身份创建** |`POST /identity`|当应用多种功能时（例如，用户想要深度检测和智能），使用`intelligence: true`标志将它们组合在单个`POST /detect`调用中，而不是单独发出请求。

##必需的设置

- **API密钥**：来自类似AI仪表板的承载令牌（设置为`RESEMBLE_API_KEY`）
—**基础URL**:`https://app.resemble.ai/api/v2`- **认证头**:`Authorization: Bearer <RESEMBLE_API_KEY>`- **媒体要求**：所有媒体必须位于可公开访问的HTTPS URL

如果用户提供的是本地文件路径而不是URL，请告知他们文件必须首先托管在公共HTTPS URL上。不要尝试将本地文件上传到API。（例外：`POST /text_detect`接受内联文本内容。）

## MCP Tools Available

当similar MCP服务器连接时，使用这些工具代替原始API调用：

|工具|用途||---------------------------|---------------------------------------------------|
|`resemble_docs_lookup`|获取任何检测子主题|的综合文档
|`resemble_search`|搜索所有文档|
|`resemble_api_endpoint`|获取任何端点|的确切OpenAPI规范
|`resemble_api_search`|通过关键字|查找端点
|`resemble_get_page`|阅读特定的文档页面|
|`resemble_list_topics`|列出所有可用主题|

**工具使用模式**：在主题为`"detect"`的情况下使用`resemble_docs_lookup`来获得全图，然后在进行API调用之前使用`resemble_api_endpoint`来获得确切的request/response模式。

完整的API参考

每个端点的详细request/response模式见**[references/api-reference.md](references/api-reference.md)**。在进行任何API调用之前，请查阅它，以验证确切的参数名称和响应形状。下面几节讨论决策；该参考涵盖了精确的字段格式。

---

阶段1：深度伪造检测核心能力。通过`POST /detect`提交音频，图像或视频用于ai生成的内容分析。

**考虑的关键标志：**
-`visualize: true`生成heatmap/visualization工件
-`intelligence: true`-运行多模式情报与检测（节省往返）
-`audio_source_tracing: true`-识别哪个AI平台合成了假音频（只在`"fake"`音频上触发）
-`use_reverse_search: true`-启用反向图像搜索（仅图像）
-`zero_retention_mode: true`-分析后自动删除媒体（针对敏感内容）

检测是异步的。以2s→5s→10s的间隔轮询`GET /detect/{uuid}`，直到`status`为`"completed"`或`"failed"`。大多数在10-60秒内完成。

**支持的格式：**音频（WAV, MP3， OGG, M4A， FLAC）·视频（MP4， MOV， AVI， WMV）·图像（JPG， PNG， GIF， WEBP）

阅读结果-`metrics`-使用`label`和`aggregated_score`- **图片** -`image_metrics`-使用`label`和`score`；`ifl`有一个不可见频率层热图
-`video_metrics`的判决-frame/segment结果的层次树；Video-with-audio返回`metrics`和`video_metrics`请参阅[references/api-reference.md]（references/api-reference.md#reading-results-by-media-type）了解完整的响应模式。

解读分数

|评分范围|释义||-------------|-----------------------------------------------------|
| 0.0 - 0.3 |强指示authentic/real媒体|
不确定-建议进一步分析
| 0.5 - 0.7 |可能的合成标志进行审查|
| 0.7 - 1.0 |高置信度synthetic/AI-generated|

**总是根据上下文呈现分数。**说“检测返回的分数为0.87，表明高度确信此音频是人工智能生成的”-而不仅仅是“它是假的”。

---

第二阶段：情报-媒体分析

关于媒体的丰富结构化见解：演讲者信息，情感，转录，翻译，错误信息，异常。

运行Intelligence的两种方式：
1. **结合检测** -添加`intelligence: true`到`POST /detect`（优先，一次呼叫）
2. **独立** -`POST /intelligence`与一个URL（当你只需要分析，而不是一个深度假的判决）**Audio/video结构化字段包括：**`speaker_info`、`language`、`dialect`、`emotion`、`speaking_style`、`context`、`message`、`abnormalities`、`transcription`、`translation`、`misinformation`。

**图像结构化字段包括：**`scene_description`、`subjects`、`authenticity_analysis`、`context_and_setting`、`abnormalities`、`misinformation`。

侦测情报-询问有关结果的问题

检测完成后，使用`{ "query": "..." }`通过`POST /detects/{detect_uuid}/intelligence`询问自然语言问题。返回一个问题UUID -轮询`GET /detects/{detect_uuid}/intelligence/{question_uuid}`直到`completed`。

**建议的好问题：**
-“用通俗易懂的语言总结检测结果”
“有什么具体的指标表明这是人工智能生成的？”
“音频和视频检测结果有什么不同？”
-“什么是置信度，它意味着什么？”
“分析中有什么不一致的地方吗？”

**前提条件：**检测必须为`status: "completed"`。针对处理或检测失败提交问题返回422。参见[references/api-reference.md]（references/api-reference.md#intelligence）了解完整参数。

---

阶段3：音频源跟踪

当音频被标记为`"fake"`时，确定是哪个AI平台生成的。

**通过在`POST /detect`请求中设置`audio_source_tracing: true`使能。结果显示在`audio_source_tracing.label`下的检测响应中。

已知标签：`resemble_ai`、`elevenlabs`、`real`，以及其他随模型扩展的标签。

**重要：**源跟踪只运行在音频标记为`"fake"`。真实音频不产生源跟踪结果。

独立查询：`GET /audio_source_tracings`和`GET /audio_source_tracings/{uuid}`。

---

阶段4：水印

对媒体应用不可见的水印以进行来源跟踪，或检测现有的水印。- **应用**:`POST /watermark/apply`与`url`，可选`strength`(0.0-1.0)，可选`custom_message`。为同步响应添加`Prefer: wait`，或轮询`GET /watermark/apply/{uuid}/result`。响应包括`watermarked_media`URL。
- **检测**:`POST /watermark/detect`与`url`。音频返回`{ has_watermark, confidence }`；image/video返回`{ has_watermark }`。

请参阅[references/api-reference.md]（references/api-reference.md#watermarking）了解确切的参数规则。

---

阶段5：身份-说话人验证（测试版）

创建语音身份配置文件，并将传入音频与之匹配。

> **Beta功能** -需要加入预览程序。如果用户遇到访问错误，通知用户。

- **用`{ audio_url, name }`创建配置文件**:`POST /identity`- **搜索**:`POST /identity/search`与`{ audio_url, top_k }`响应返回与`confidence`（更高=更强）和`distance`（更低=更接近）匹配的排序匹配。

请参阅[references/api-reference.md]（references/api-reference.md#identity—speaker-verification-beta）了解完整的模式。

---

阶段6：文本检测通过`POST /text_detect`检测文本内容是人工生成的还是人工编写的。

b> **测试版功能** -需要`detect_beta_user`角色或包含`dfd_text`产品的计费计划。

* *关键参数:* *
-`text`（必选，最多100,000个字符）
-`threshold`（默认为0.5）
-`privacy_mode: true`-文本内容分析后不存储
-`callback_url`-异步通知webhook

为同步响应添加`Prefer: wait`，或轮询`GET /text_detect/{uuid}`。响应包括`prediction`（`"ai"`或`"human"`）和`confidence`（0.0-1.0）。

请参阅[references/api-reference.md]（references/api-reference.md#text-detection）了解完整的模式和回调格式。

---

推荐的工作流程

全媒体取证（最彻底）

为了进行全面的分析，将所有功能结合起来：

1. 启用所有标志提交检测：   ```json
   {
     "url": "https://example.com/suspect.mp4",
     "visualize": true,
     "intelligence": true,
     "audio_source_tracing": true,
     "use_reverse_search": true
   }
   ```
2. 轮询到`status: "completed"`3. 阅读`metrics`/`image_metrics`/`video_metrics`了解判决
4. 阅读`intelligence.description`了解结构化媒体分析
5. 如果音频标记为`"fake"`，则检查源平台的`audio_source_tracing.label`6. 如果有任何需要澄清的地方，通过情报检测部门提出后续问题
7. 如果出处相关，通过`POST /watermark/detect`检查水印

快速真实性检查（最快）

1. 提交最小检测：`{ "url": "..." }`2. 投票直到完成
3. 检查`label`和`aggregated_score`（音频）或`label`和`score`（image/video）
4. 报告带有分数上下文的结果

来源管道（内容创造者）

1. 对原始内容应用水印：`POST /watermark/apply`2. 分发带有水印的媒体
3. 稍后，根据任何副本验证出处：`POST /watermark/detect`---

##红旗-停止并重新评估- **在没有检测结果的情况下声明真实性** -永远不要仅仅根据visual/auditory检测就说媒体是真是假
- **忽略分数并仅报告标签** -分数为0.51的`"fake"`标签与分数0.95的含义非常不同
- **向API提交本地文件路径** - API需要可公开访问的HTTPS url（不适用于文本检测）
- **发送超过100,000个字符的文本到文本检测** -分割成块或通知用户的限制
-轮询过于激进** -以2s间隔开始，以指数方式退后；不要在<1s处循环
- **在检测完成之前询问检测情报问题** -导致422错误
- **期望对“真实”音频进行源跟踪** -源跟踪仅运行在标记为`"fake"`的音频上
- **将beta版功能（身份，文本检测）视为生产就绪** -警告用户助教状态
- **忽略敏感媒体的`zero_retention_mode`** -当用户指定敏感或私有媒体时，总是建议使用此标志
**在检测调用上使用`intelligence: true`和`audio_source_tracing: true`，而不是单独的请求##回应陈述指南

当向用户展示结果时：

1. **以判决开头** -“检测表明该音频可能是人工智能生成的（得分：0.87）”
2. **提供分数上下文** -使用上面的分数解释表
3. **提到限制** -检测是概率性的，而不是绝对的证据
4. **包括可操作的后续步骤** -建议情报查询，来源跟踪或水印检查
5. **对于不确定的结果(0.3-0.5)** -明确说明结果不确定，并建议使用不同参数进行额外分析或人工审查
6. **永远不要将检测结果作为法律证据** -检测结果是分析工具，而不是法医证明

##错误处理

|错误|原因|解析||-----------|--------------------------------------------|-------------------------------------------------|
| 400 |请求体无效或缺少`url`|检查所需参数|
|无效或缺少API密钥|验证`RESEMBLE_API_KEY`|
| 404 |检测UUID未找到|验证创建响应|中的UUID
| 422 |检测未完成（for Intelligence） |等待检测到达`completed`状态|
| 429 |速率限制|后退并以指数延迟|重试
| 500 |服务器错误|重试一次，然后报告给用户|

##隐私和合规说明—**零保留模式**：设置`zero_retention_mode: true`为分析后自动删除介质。编辑URL，并在完成后将`media_deleted`设置为true。
—**文本保密模式**：设置文本检测的`privacy_mode: true`，防止文本内容分析后被存储。
—**数据处理**：默认存储媒体url和文本内容。对于GDPR/compliance-sensitive工作流，启用零保留（媒体）或隐私模式（文本）。
—**回调安全**：如果使用`callback_url`，请确保终端为HTTPS，并在接收端进行认证。