#类似检测-完整的API参考

每个类似检测端点的详细request/response模式。

# #基地

- **基础URL**:`https://app.resemble.ai/api/v2`- **授权**:`Authorization: Bearer <RESEMBLE_API_KEY>`---

##深度造假检测

# # #`POST /detect`提交音频、图像或视频用于人工智能生成分析。```json
{
  "url": "https://example.com/media.mp4",
  "visualize": true,
  "intelligence": true,
  "audio_source_tracing": true
}
```
|参数|类型|必选|描述||------------------------|---------|----------|----------------------------------------------------------|
|`url`| string |是|音频、图像或视频文件|的HTTPS URL
|`callback_url`| string | No | Webhook URL用于异步完成通知|
|`visualize`|布尔|否|生成heatmap/visualization工件|
|`intelligence`|布尔|无|运行多模式智能检测|
|`audio_source_tracing`| boolean |否|识别哪个AI平台合成了假音频|
|`frame_length`| integer |否|Audio/video窗口大小，单位为秒（1-4，默认为2）|
|`start_region`| number |否|待分析段的开始时间（秒）|
|`end_region`| number |否|待分析段结束（秒）|
|`model_types`| string |无|`"image"`或`"talking_head"`（用于换脸检测）|
|`use_reverse_search`| boolean |否|启用反向图像搜索（仅限图像）|
|`use_ood_detector`|布尔|否|启用分发外检测|
|`zero_retention_mode`| boolean |否| |检测完成后自动删除媒体**支持的格式：**音频（WAV, MP3， OGG, M4A， FLAC）·视频（MP4， MOV， AVI， WMV）·图像（JPG， PNG， GIF， WEBP）

###`GET /detect/{uuid}`-投票的结果

检测是异步的。轮询直到`status`为`"completed"`或`"failed"`。从每隔15秒开始，逐渐减少到5秒，然后是10秒。大多数检测在10 - 60秒内完成。

按媒体类型读取结果

**音频结果** -在`metrics`：```json
{
  "label": "fake",
  "score": ["0.92", "0.88", "0.95"],
  "consistency": "0.91",
  "aggregated_score": "0.92",
  "image": "https://..."
}
```
-`label`:`"fake"`或`"real"`-结论
-`score`：每块预测分数（数组）
-`aggregated_score`：总体置信度（0.0-1.0，更高=更有可能合成）
-`consistency`：预测跨块的一致性
-`image`：可视化热图URL（如果`visualize: true`）

**图像结果** -在`image_metrics`：```json
{
  "type": "ImageAnalysis",
  "label": "fake",
  "score": 0.87,
  "image": "https://...",
  "ifl": { "score": 0.82, "heatmap": "https://..." },
  "reverse_image_search_sources": [
    { "url": "...", "title": "...", "verdict": "known_fake", "similarity": 0.95 }
  ]
}
```
-`ifl`：看不见的频率层分析与热图
-`reverse_image_search_sources`：已知在线资源（如果`use_reverse_search: true`）

**视频结果** -在`video_metrics`：```json
{
  "label": "fake",
  "score": 0.89,
  "certainty": 0.91,
  "children": [
    { "type": "VideoResult", "conclusion": "Fake", "score": 0.89, "timestamp": 2.5, "children": [...] }
  ]
}
```
-帧级和段级结果的分层树
-带有音轨的视频返回`metrics`（音频）和`video_metrics`（视觉）

---

# #情报

# # #`POST /intelligence`分析媒体以获得丰富的结构化见解，独立或与检测一起。```json
{ "url": "https://example.com/audio.mp3", "json": true }
```
|参数|类型|必选|描述||----------------|---------|----------|----------------------------------------------------------|
|`url`| string | |中的一个|媒体文件的HTTPS URL
|`media_token`| string | |安全上传令牌（URL替代）|
|`detect_id`| string |否|关联|的现有检测的UUID
|`media_type`| string |否|`"audio"`，`"video"`，或`"image"`（自动检测）|
|`json`|布尔|无|返回结构化字段（默认：falseaudio/video， true image） |
|`callback_url`| string |无|异步模式的Webhook |**Audio/Video结构化响应** (`json: true`)：
-`speaker_info`-说话人描述（年龄、性别）
-`language`/`dialect`-检测语言
-`emotion`-检测情绪状态
-`speaking_style`-会话，正式等
-`context`-演讲的推断上下文
-`message`-内容摘要
-`abnormalities`-在介质中检测到异常
-`transcription`-全文
-`translation`-非英语时的翻译
-`misinformation`-错误信息分析

**图像结构化响应：**
-`scene_description`-图像显示
-`subjects`-已识别people/objects-`authenticity_analysis`-视觉真实性评估
-`context_and_setting`-环境描述
-`abnormalities`-视觉异常
-`misinformation`-错误信息分析

###`POST /detects/{detect_uuid}/intelligence`-问问题

检测完成后，询问有关它的自然语言问题：```json
{ "query": "How confident is the model that this audio is fake?" }
```
返回一个问题UUID。轮询`GET /detects/{detect_uuid}/intelligence/{question_uuid}`，直到`status`为`"completed"`。

**前提条件：**检测必须为`status: "completed"`。否则返回422。

---

音频源跟踪

通过在`POST /detect`中设置`audio_source_tracing: true`来启用。`audio_source_tracing`项下的检测响应显示结果：```json
{ "label": "elevenlabs", "error_message": null }
```
已知的源代码标签：`resemble_ai`、`elevenlabs`、`real`，以及其他随模型扩展的标签。

**重要：**源跟踪仅在音频标记为`"fake"`时运行。如果音频为`"real"`，则不会出现源跟踪结果。

* *独立查询:* *
-`GET /audio_source_tracings`-列出所有源跟踪报告
-`GET /audio_source_tracings/{uuid}`-获取具体报告

---

# #水印

# # #`POST /watermark/apply````json
{
  "url": "https://example.com/image.png",
  "strength": 0.3,
  "custom_message": "my-organization"
}
```
|参数|类型|必选|描述||------------------|--------|----------|-------------------------------------------------------------|
|`url`| string |是|媒体文件|的HTTPS URL
|`strength`| number |否|水印强度0.0-1.0（仅image/video，默认0.2）|
|`custom_message`| string |否|自定义消息（仅image/video，默认为“resembleai”）|

-添加`Prefer: wait`报头用于同步响应
—没有，轮询`GET /watermark/apply/{uuid}/result`—响应包括`watermarked_media`下载带水印文件的URL

# # #`POST /watermark/detect````json
{ "url": "https://example.com/suspect-image.png" }
```
**音频检测结果：**```json
{ "has_watermark": true, "confidence": 0.95 }
```
**Image/Video检测结果：**```json
{ "has_watermark": true }
```
---

## Identity - Speaker Verification （Beta版）

> **Beta功能** -需要加入预览程序。如果用户遇到访问错误，通知用户。

###`POST /identity`-创建身份配置文件```json
{
  "audio_url": "https://example.com/known-speaker.wav",
  "name": "Jane Doe"
}
```
###`POST /identity/search`-根据已知身份搜索```json
{
  "audio_url": "https://example.com/unknown-speaker.wav",
  "top_k": 5
}
```
* *反应:* *```json
{
  "success": true,
  "item": [
    { "uuid": "...", "name": "Jane Doe", "confidence": 0.92, "distance": 0.08 }
  ]
}
```
较低的`distance`=更接近匹配。更高的`confidence`=更强的匹配。

---

##文本检测

b> **测试版功能** -需要`detect_beta_user`角色或包含`dfd_text`产品的计费计划。

# # #`POST /text_detect`添加`Prefer: wait`进行同步响应。否则轮询或使用回调。

|参数|类型|必选|描述||----------------|---------|----------|----------------------------------------------------------|
|`text`| string |是|要分析的文本（最多10万字符）|
|`thinking`| string |否|始终使用`"low"`（默认）|
|`threshold`| float |否|决策阈值0.0-1.0（默认：0.5）|
|`callback_url`| string | No | Webhook URL用于异步完成通知|
|`privacy_mode`| boolean |否|如果为true，分析后不存储文本内容|

* *反应:* *```json
{
  "success": true,
  "item": {
    "uuid": "abc-123",
    "status": "completed",
    "prediction": "ai",
    "confidence": 0.91,
    "text_content": "This is some text to analyze.",
    "privacy_mode": false,
    "created_at": "...",
    "updated_at": "..."
  }
}
```
-`prediction`:`"ai"`或`"human"`-结论
-`confidence`: 0.0-1.0，越高越自信
—`status`:`"processing"`、`"completed"`、`"failed"`###`GET /text_detect/{uuid}`-投票

轮询直到`status`为`"completed"`或`"failed"`。

###`GET /text_detect`- List

返回团队的分页文本检测。

# # #回调

如果提供了`callback_url`，则在完成时发送`POST`：```json
{ "success": true, "item": { ... } }
```
失败:```json
{ "success": false, "item": { ... }, "error": "Error message here" }
```
