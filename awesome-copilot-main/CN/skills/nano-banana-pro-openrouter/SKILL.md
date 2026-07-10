---
name: nano-banana-pro-openrouter
description: 'Generate or edit images via OpenRouter with the Gemini 3 Pro Image model. Use for prompt-only image generation, image edits, and multi-image compositing; supports 1K/2K/4K output.'
metadata:
  emoji: 🍌
  requires:
    bins:
      - uv
    env:
      - OPENROUTER_API_KEY
  primaryEnv: OPENROUTER_API_KEY
---
# Nano Banana Pro OpenRouter

# #概述

使用`google/gemini-3-pro-image-preview`模型生成或编辑OpenRouter图像。支持仅提示生成、单图像编辑和多图像合成。

仅提示生成```
uv run {baseDir}/scripts/generate_image.py \
  --prompt "A cinematic sunset over snow-capped mountains" \
  --filename sunset.png
```
编辑单个图像```
uv run {baseDir}/scripts/generate_image.py \
  --prompt "Replace the sky with a dramatic aurora" \
  --input-image input.jpg \
  --filename aurora.png
```
###合成多个图像```
uv run {baseDir}/scripts/generate_image.py \
  --prompt "Combine the subjects into a single studio portrait" \
  --input-image face1.jpg \
  --input-image face2.jpg \
  --filename composite.png
```
# #决议

—将`--resolution`与`1K`、`2K`或`4K`一起使用。
—如果不指定，默认为`1K`。

系统提示自定义

该技能从`assets/SYSTEM_TEMPLATE`读取一个可选的系统提示符。这允许您自定义图像生成行为，而无需修改代码。

##行为和约束

-接受多达3个输入图像通过重复`--input-image`。
-`--filename`接受相对路径（保存到当前目录）或绝对路径。
—如果返回多张图片，在文件名后面添加“`-1`”、“`-2`”等。
-为每个保存的图像打印`MEDIA: <path>`。不要把图片读回回复中。

# #故障排除

如果脚本退出时为非零，则根据这些常见拦截器检查stderr：

|解决方案||---------|------------|
|`OPENROUTER_API_KEY is not set`|请用户自行设置。PowerShell:`$env:OPENROUTER_API_KEY = "sk-or-..."`/ bash:`export OPENROUTER_API_KEY="sk-or-..."`|
|`uv: command not found`或无法识别|macOS/Linux:<code>curl -LsSfhttps://astral.sh/uv/install.sh&#124；Windows:<code>powershell -ExecutionPolicy ByPass -c "irmhttps://astral.sh/uv/install.ps1&#124; iex"</code>。然后重启终端。|
|`AuthenticationError`/ HTTP 401 |密钥无效或无信用。在<https://openrouter.ai/settings/keys>处验证。|

对于瞬态错误（HTTP 429、网络超时），30秒后重试一次。不要对同一个错误重试两次以上，而是将问题暴露给用户。