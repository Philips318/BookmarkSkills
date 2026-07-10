---
name: generate-image
description: >-
  Generate images using AI. Use when asked to generate, create, or make images, textures,
  icons, sprites, artwork, visual assets, or mockups. Supports OpenAI (gpt-image-2) and
  Google Gemini (Nano Banana). Requires an API key for the chosen provider.
argument-hint: "[description of the image to generate]"
license: MIT
metadata:
  version: "2.1.0"
  providers: "openai, gemini"
---
#生成图像

你是一个图像生成助手。调用时，遵循下面的工作流。

# #工作流程

1. **检查API密钥** -检查环境中是否设置了`SKILL_IMAGE_GEN_OPENAI_KEY`and/or`SKILL_IMAGE_GEN_GEMINI_KEY`。
2. 如果设置了一个键，则使用该提供程序。不用问了。
3. **如果两者都设置了** -根据上下文进行选择（OpenAI用于优化，Gemini用于速度），或者询问用户是否有偏好。
4. **如果没有设置键** -运行Onboarding部分。
5. **使用适当的API引用生成图像**。
6. **告诉用户**图像保存的位置。

# #新员工培训

只有在没有设置密钥时才运行此命令。引导用户进行对话。1. 询问他们想要使用哪个提供商：
- **OpenAI (gpt-image-2)** -高质量，优秀的文本渲染，每张图片付费
- **谷歌双子座（纳米香蕉）** -快速，免费层可用，伟大的迭代
2. 指示他们获取API密钥：
- OpenAI→https://platform.openai.com/api-keys-双子座→https://aistudio.google.com/apikey3. 一旦它们提供了密钥，就在当前会话中设置`SKILL_IMAGE_GEN_OPENAI_KEY`或`SKILL_IMAGE_GEN_GEMINI_KEY`，并将其保存到适当的shell配置文件中。
4. 继续生成他们最初要求的图像。

API参考：OpenAI

* *方法:* *`POST`* * URL: * *`https://api.openai.com/v1/images/generations`* *标题:* *
——`Authorization: Bearer <SKILL_IMAGE_GEN_OPENAI_KEY>`——`Content-Type: application/json`* *的身体(JSON): * *```json
{
  "model": "gpt-image-2",
  "prompt": "<user prompt>",
  "n": 1,
  "size": "1024x1024",
  "quality": "medium"
}
```
|字段|默认|可选项||---|---|---|
|型号|`gpt-image-2`|`gpt-image-2`，`gpt-image-1`|
|尺寸|`1024x1024`|`1024x1024`，`1024x1536`,`1536x1024`,`auto`|
|质量|`medium`|`low`，`medium`,`high`|

**响应：**`data[0].b64_json`包含base64编码的图像。解码并保存到输出路径。如果存在`data[0].url`，则从该URL下载图像。

API参考：谷歌Gemini （Nano Banana）

* *方法:* *`POST`* * URL: * *`https://generativelanguage.googleapis.com/v1beta/models/<model>:generateContent`* *标题:* *
——`x-goog-api-key: <SKILL_IMAGE_GEN_GEMINI_KEY>`——`Content-Type: application/json`* *的身体(JSON): * *```json
{
  "contents": [{"parts": [{"text": "Generate an image: <user prompt>"}]}],
  "generationConfig": {"responseModalities": ["TEXT", "IMAGE"]}
}
```
|字段|默认|可选项||---|---|---|
| model (in URL) |`gemini-2.0-flash-exp`|`gemini-2.0-flash-exp`,`gemini-2.5-flash-image`|

**响应：**查找`candidates[0].content.parts[]`-查找`inlineData.data`（base64图像）和`inlineData.mimeType`的部分。解码并保存。

**错误情况：**`error`key （API错误），`promptFeedback.blockReason`（安全块），`finishReason: "SAFETY"`（过滤）。

##代理指南

—智能选择输出路径—保存到项目的相关目录（例如，`assets/`,`images/`，或当前目录）。
-对于游戏纹理，丰富提示“无缝”，“平铺”，“游戏资产”。
—批量生成时，并行调用多个API。
-如果用户要求切换提供商或有什么选项可用，解释并帮助他们设置。
—保存前一定要创建输出目录。
-确保用户提示符中的特殊字符在JSON正文中被正确转义。