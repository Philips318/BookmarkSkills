---
name: transloadit-media-processing
description: 'Process media files (video, audio, images, documents) using Transloadit. Use when asked to encode video to HLS/MP4, generate thumbnails, resize or watermark images, extract audio, concatenate clips, add subtitles, OCR documents, or run any media processing pipeline. Covers 86+ processing robots for file transformation at scale.'
license: MIT
compatibility: Requires a free Transloadit account (https://transloadit.com/signup). Uses the @transloadit/mcp-server MCP server or the @transloadit/node CLI.
---
# Transloadit Media Processing

使用Transloadit的云基础设施处理、转换和编码媒体文件。
支持视频，音频，图像和文档与86+专门的处理机器人。

何时使用此技能

在需要时使用此技能：

—将视频编码为HLS、MP4、WebM或其他格式
-从视频生成缩略图或动画gif
-调整大小，裁剪，水印，或优化图像
-转换图像格式（JPEG， PNG, WebP， AVIF， HEIF）
-提取或转码音频（MP3， AAC， FLAC， WAV）
-连接视频或音频剪辑
-在视频上添加字幕或覆盖文本
- OCR文件（PDF，扫描图像）
—执行语音转文本或文本转语音
-应用基于人工智能的内容审核或对象检测
-建立多步骤的媒体管道，连锁操作在一起

# #设置

选项A: MCP服务器（建议用于副驾驶）将Transloadit MCP服务器添加到IDE配置中。这为代理提供了直接访问
转载工具（`create_template`,`create_assembly`，`list_assembly_notifications`等）。

**VS Code/GitHub Copilot** （`.vscode/mcp.json`或用户设置）：```json
{
  "servers": {
    "transloadit": {
      "command": "npx",
      "args": ["-y", "@transloadit/mcp-server", "stdio"],
      "env": {
        "TRANSLOADIT_KEY": "YOUR_AUTH_KEY",
        "TRANSLOADIT_SECRET": "YOUR_AUTH_SECRET"
      }
    }
  }
}
```
在https://transloadit.com/c/-/api-credentials获取API凭据

选项B: CLI

如果你喜欢直接运行命令：```bash
npx -y @transloadit/node assemblies create \
  --steps '{"encoded": {"robot": "/video/encode", "use": ":original", "preset": "hls-1080p"}}' \
  --wait \
  --input ./my-video.mp4
```
核心工作流

将视频编码为HLS（自适应流）```json
{
  "steps": {
    "encoded": {
      "robot": "/video/encode",
      "use": ":original",
      "preset": "hls-1080p"
    }
  }
}
```
从视频生成缩略图```json
{
  "steps": {
    "thumbnails": {
      "robot": "/video/thumbs",
      "use": ":original",
      "count": 8,
      "width": 320,
      "height": 240
    }
  }
}
```
###调整大小和水印图像```json
{
  "steps": {
    "resized": {
      "robot": "/image/resize",
      "use": ":original",
      "width": 1200,
      "height": 800,
      "resize_strategy": "fit"
    },
    "watermarked": {
      "robot": "/image/resize",
      "use": "resized",
      "watermark_url": "https://example.com/logo.png",
      "watermark_position": "bottom-right",
      "watermark_size": "15%"
    }
  }
}
```
OCR文档```json
{
  "steps": {
    "recognized": {
      "robot": "/document/ocr",
      "use": ":original",
      "provider": "aws",
      "format": "text"
    }
  }
}
```
连接音频剪辑```json
{
  "steps": {
    "imported": {
      "robot": "/http/import",
      "url": ["https://example.com/clip1.mp3", "https://example.com/clip2.mp3"]
    },
    "concatenated": {
      "robot": "/audio/concat",
      "use": "imported",
      "preset": "mp3"
    }
  }
}
```
多步骤管道

可以使用`"use"`字段链接步骤。每一步都引用前一步的输出：```json
{
  "steps": {
    "resized": {
      "robot": "/image/resize",
      "use": ":original",
      "width": 1920
    },
    "optimized": {
      "robot": "/image/optimize",
      "use": "resized"
    },
    "exported": {
      "robot": "/s3/store",
      "use": "optimized",
      "bucket": "my-bucket",
      "path": "processed/${file.name}"
    }
  }
}
```
##关键概念

**组装**：一个单一的处理工作。通过`create_assembly`（MCP）或`assemblies create`（CLI）创建。
- **模板**：存储在Transloadit上的可重用步骤集。通过`create_template`（MCP）或`templates create`（CLI）创建。
- **机器人**：处理单元（如`/video/encode`、`/image/resize`）。请参阅https://transloadit.com/docs/transcoding/的完整列表
- **步骤**：定义管道的JSON对象。每个键是一个步骤名称，每个值配置一个机器人。
—**`:original`**：上传的输入文件。

# #提示—在CLI中使用`--wait`阻塞，直到处理完成。
—使用`preset`值（例如，`"hls-1080p"`,`"mp3"`,`"webp"`）作为通用格式目标，而不是指定每个参数。
-链`"use": "step_name"`构建多步骤管道没有中间下载。
—对于批处理，使用`/http/import`从url、S3、GCS、Azure、FTP或Dropbox中提取文件。
模板可以包括在程序集创建时传递的动态值`${variables}`。