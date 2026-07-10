#技能图像Gen

使用AI直接从你的编码工作流程生成图像。支持**OpenAI (gpt-image-2)**和**谷歌Gemini (Nano Banana)**。

它的作用

使用自然语言生成图像-图标，精灵，纹理，模型，艺术品-无需离开编辑器。该技能处理API调用、文件保存，并在第一次使用时指导您完成设置。

# #提供者

|提供商|型号|优势||---|---|---|
|高质量，优秀的文本渲染|
|谷歌双子座|双子座-2.0-flash-exp |快速，免费层可用|

# #设置

BYO API密钥。第一次使用时，该技能将引导你完成：

1. 选择供应商
2. 获取API密钥（[OpenAI]（https://platform.openai.com/api-keys）·[Gemini](https://aistudio.google.com/apikey)）
3. 设置环境变量（`SKILL_IMAGE_GEN_OPENAI_KEY`或`SKILL_IMAGE_GEN_GEMINI_KEY`）

# #使用

要求副驾驶生成图像：

-“生成一个像素艺术宝箱”
“为我的游戏创建一个无缝的草纹理”
-“为我的应用做一个极简主义的logo”

# #链接

- **源代码**:[adamd9/skill-image-gen]（https://github.com/adamd9/skill-image-gen）
- **许可证**:MIT