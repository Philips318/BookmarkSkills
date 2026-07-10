---
name: screen-recording
description: 'Create annotated animated GIF demos and screen recordings for pull requests and documentation. Covers frame capture, timing, imageio-based GIF creation, and per-frame annotation workflows.'
---
#屏幕录制

创建动画GIF演示，显示一个功能或工作流程在行动-与注释，可变的时间，和适当的节奏。用于PR描述、文档和发布说明。

何时使用此技能

在需要时使用此技能：

-将多步骤UI交互记录为动画GIF
-创建一个演示before/after行为
-为文档或发行说明构建带注释的演练
-显示一个错误的复制或修复在行动

# #先决条件```bash
pip install playwright Pillow imageio numpy scipy mss -q
playwright install chromium
```
核心工作流程

# # # 1。捕捉帧

使用剧作家逐步完成交互并捕捉每一帧：```python
from playwright.async_api import async_playwright

async def record_frames(url, steps, width=1400, height=900):
    """
    steps: list of dicts with 'action' (async callable taking page)
           and 'name' (frame filename)
    """
    async with async_playwright() as p:
        browser = await p.chromium.launch()
        page = await browser.new_page(viewport={"width": width, "height": height})
        await page.goto(url, wait_until="networkidle")

        for step in steps:
            if step.get("action"):
                await step["action"](page)
                await page.wait_for_timeout(step.get("wait", 500))
            await page.screenshot(path=step["name"])

        await browser.close()
```
# # # 2。组装GIF与imageio

**使用imageio，而不是PIL，用于GIF编写** - PIL的GIF编码器合并视觉上相似的帧，这杀死了动画。```python
import imageio.v3 as iio
from PIL import Image
import numpy as np

frames = []
durations = []

for frame_path, duration_ms in frame_list:
    img = Image.open(frame_path)
    frames.append(np.array(img))
    durations.append(duration_ms)

iio.imwrite("demo.gif", frames, duration=durations, loop=0)
```
# # # 3。可变帧定时

统一的时间会让你觉得一切不是太快就是太慢。使用可变持续时间：

|阶段|持续时间|原因||-------|----------|-----|
|快速动作（打字，点击）| 100ms |感觉自然，保持活力|
|动作后暂停| 600-800ms |让观看者处理发生的事情|
| 500ms+ |主外卖需要时间降落|

# # # 4。注释帧

使用`image-annotations`技能对特定帧应用注释：```python
from PIL import Image, ImageDraw, ImageFont

def annotate_frame(frame_path, annotations, out_path):
    img = Image.open(frame_path)
    draw = ImageDraw.Draw(img)

    for ann in annotations:
        # Apply annotation (rect, arrow, label, etc.)
        pass

    img.save(out_path)
```
# # # 5。淡入注释

对于光滑的注释外观：```python
def apply_fade(base_frame, annotation_layer, alpha):
    """Blend annotation onto frame at given alpha (0.0 to 1.0)"""
    blended = Image.blend(
        base_frame.convert("RGBA"),
        annotation_layer.convert("RGBA"),
        alpha
    )
    return blended.convert("RGB")

# 2-frame pop-in at 10fps: 50% then 100%
faded_frames = [
    apply_fade(base, annotations, 0.5),  # frame 1: half opacity
    apply_fade(base, annotations, 1.0),  # frame 2: full opacity
]
```
在10fps时，使用2个渐变帧（总共0.2s）。在30fps时，使用3-4帧。舒缓曲线在低FPS下看起来很糟糕-简单的弹出式更快捷，更可读。

作为脚本构建

除了简单的演示之外，注释逻辑变得非常复杂。用函数而不是内联代码编写一个专用的脚本（例如，`annotate_gif.py`）。你将迭代时间和位置。

##测试动画

**总是先单独测试** -不要重新构建完整的演示来测试淡出调整：```python
# Small test GIF: 10 bare frames → fade frames → 15 hold frames
# Add a frame counter overlay for debugging:
draw.text((10, height - 30), f"F{i}/{total} a={alpha:.0%} FADE",
          fill="white", font=small_font)
```
桌面屏幕录制（mss）

用于录制桌面应用程序，终端或浏览器之外的任何内容。使用`mss`进行快速屏幕捕获。```python
import mss
from PIL import Image
import time

def record_gif(output_path, region=None, duration=5, fps=8):
    """Record screen region to GIF. region = {left, top, width, height} or None for full screen."""
    with mss.mss() as sct:
        if region is None:
            region = sct.monitors[1]  # primary monitor

        frames = []
        t_end = time.time() + duration
        while time.time() < t_end:
            t0 = time.time()
            shot = sct.grab(region)
            frames.append(Image.frombytes('RGB', shot.size, shot.rgb))
            time.sleep(max(0, 1 / fps - (time.time() - t0)))

    frames[0].save(output_path, save_all=True, append_images=frames[1:],
                   duration=int(1000 / fps), loop=0, optimize=True)
    return len(frames)

record_gif('demo.gif', region={'left': 0, 'top': 0, 'width': 800, 'height': 500}, duration=3)
```
经过测试：3秒，8fps→24帧，~31KB。对于合理的文件大小，保持fps≤10。

**注意：**`PIL.save(save_all=True)`适用于简单的录音，但合并视觉上相似的帧。对于带有淡出效果的带注释的gif，请使用`imageio.v3.imwrite`。

与窗口捕获相结合```python
# Find window rect, then record it as a GIF
# Reuse find_window() from the ui-screenshots skill
import ctypes
from ctypes import c_int, Structure, byref, windll

class RECT(Structure):
    _fields_ = [('left', c_int), ('top', c_int), ('right', c_int), ('bottom', c_int)]

hwnd = find_window('My App')[0][0]
rect = RECT()
windll.user32.GetWindowRect(hwnd, byref(rect))
region = {'left': rect.left, 'top': rect.top,
          'width': rect.right - rect.left, 'height': rect.bottom - rect.top}
record_gif('app-demo.gif', region=region, duration=5, fps=8)
```
基于diff的集群检测

以编程方式找到帧之间的变化区域，以决定要注释什么：```python
import numpy as np
from scipy import ndimage

def find_changed_clusters(frame_a, frame_b, threshold=30, min_pixels=300, dilate=5):
    """Find bounding boxes of changed regions between two frames."""
    diff = np.abs(frame_b.astype(float) - frame_a.astype(float)).max(axis=2)
    mask = diff > threshold
    dilated = ndimage.binary_dilation(mask, iterations=dilate)
    labeled, n = ndimage.label(dilated)
    clusters = []
    for i in range(1, n + 1):
        ys, xs = np.where(labeled == i)
        if len(ys) < min_pixels:
            continue
        clusters.append((xs.min(), ys.min(), xs.max(), ys.max(), len(ys)))
    return sorted(clusters, key=lambda c: -c[4])  # largest first
```
##格式兼容性

|格式|VS Code预览| GitHub |浏览器||--------|----------------|--------|---------|
| GIF |✅动画|✅|✅|
| WebP |⚠️静态|✅|✅|
| MP4 |❌破碎|⚠️|✅|

**GIF是唯一普遍支持的动画格式**跨VS Code预览，GitHub markdown，和浏览器。

# #指南

1. **输入→暂停→标注** -快速动作时，显示NO标注。先暂停，然后注释
2. **英雄信息使用最大字体** -主要内容为64pt+，细节为38pt
3. **GIF调色板不会杀死渐变** - 20个不同的alpha步骤存活256色调色板
4. **typing/interaction最小10fps ** -低看起来卡顿
5. **迭代构建** -首先获得正确的帧序列，然后添加注释，最后调整时序

# #的局限性- GIF限制为每帧256色-精细的UI截图，可能会显示图片内容的波段
-大型动图（高分辨率50帧以上）可以是几个MB -考虑裁剪到相关区域
-在GIF中不支持音频-使用MP4进行解说演示（但失去VS Code预览支持）