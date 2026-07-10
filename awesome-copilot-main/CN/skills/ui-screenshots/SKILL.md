---
name: ui-screenshots
description: 'Capture screenshots of web apps during development using Playwright and PIL. Supports full-page captures, interactive states, and an iterate-on-crop workflow that avoids slow re-screenshots.'
---
# UI截图

在开发过程中捕获web应用程序和图形ui的屏幕截图，以记录视觉变化。

何时使用此技能

在需要时使用此技能：

-捕获正在运行的web应用程序的当前状态
-在代码更改之前和之后记录UI
-截图交互状态（工具提示，悬停，选定元素）
-捕获页面的特定部分，而无需重新截图

# #先决条件```bash
pip install playwright Pillow -q
playwright install chromium
```
核心工作流程

# # # 1。以原始的整页截图为例```python
from playwright.async_api import async_playwright

async def capture(url="http://localhost:3000", out="screenshot-raw.png", width=1400, height=5000):
    async with async_playwright() as p:
        browser = await p.chromium.launch()
        page = await browser.new_page(viewport={"width": width, "height": height})
        await page.goto(url, wait_until="networkidle")
        await page.wait_for_timeout(4000)  # let charts/animations render
        await page.screenshot(path=out, full_page=True)
        await browser.close()
```
-使用一个**高的视口**（高度=5000），这样页面渲染一切没有滚动
-`wait_until="networkidle"`+`wait_for_timeout(4000)`确保异步图表加载
-`full_page=True`捕获整个可滚动的内容

# # # 2。查看原始图像，然后使用PIL进行裁剪

**不要试图通过剧作家的`clip`参数获得完美的作物。**这是不可靠的整页捕获。```python
from PIL import Image

img = Image.open("screenshot-raw.png")
cropped = img.crop((left, top, right, bottom))  # adjust based on what you see
cropped.save("screenshot-final.png")
```
1. 获取原始截图
2. 查看它以查看实际像素位置
3. 根据您所看到的情况使用PIL进行裁剪
4. 查看结果-如果不正确，重新裁剪（即时，无需重新截图）

# # # 3。迭代裁剪，而不是捕获

-重新截图很慢（浏览器启动+页面加载+渲染等待）
-重新裁剪是即时的（只是PIL）
-获得一个好的原始捕获，然后根据需要以多种方式切片

# # # 4。互动状态```python
element = page.locator("selector").first
await element.hover()
await page.wait_for_timeout(1000)  # let tooltip appear
await page.screenshot(path="screenshot-hover.png", full_page=True)
```
对于没有悬停效果的“选定”状态，点击后将鼠标移开：```python
await element.click()
await page.mouse.move(300, 300)  # move away so hover doesn't show
await page.wait_for_timeout(500)
await page.screenshot(path="screenshot-selected.png", full_page=True)
```
# # # 5。其捕捉

从单个整页截图中裁剪不同的部分：```python
img.crop((0, 200, 920, 900)).save("screenshot-header.png")
img.crop((0, 900, 920, 1600)).save("screenshot-main.png")
```
# #指南

1. **总是在做任何改变之前捕获状态** -如果你忘记了，你必须恢复代码来获得一个之前的镜头
2. **Before/after对必须使用相同的视口宽度和裁剪** -否则比较是无用的
3. **得到一个“之前”后，你已经改变了代码**：使用`git checkout HEAD~1 -- <files>`恢复，截图，然后`git checkout HEAD -- <files>`恢复
4. **对于交互状态**：捕获每个状态之前和之后-不要假设“正常”之前涵盖所有情况
5. **使用`device_scale_factor=1`**在剧作家强制1x像素，所以屏幕截图匹配用户看到的100%缩放
6. **图表需要额外的等待时间** - Plotly， D3等异步渲染；在networkidle之后，最小为4s
7. **窄视口显示渲染错误** -一些border/alignment问题只出现在特定宽度

非web应用程序截图对于桌面应用程序（VS， WPF, WinForms，控制台应用程序，终端），剧作家不能到达。

MSS + ctypes（推荐用于桌面窗口）

通过Win32 API通过标题找到窗口，用`mss`捕获其区域。每次捕获的测试时间为~33ms。```python
import ctypes
from ctypes import c_int, Structure, byref, windll
import mss
from PIL import Image

user32 = windll.user32

def find_window(title_contains):
    """Find visible windows matching a title substring."""
    results = []
    WNDENUMPROC = ctypes.WINFUNCTYPE(ctypes.c_bool, ctypes.c_void_p, ctypes.c_void_p)
    def cb(hwnd, _):
        if user32.IsWindowVisible(hwnd):
            buf = ctypes.create_unicode_buffer(256)
            user32.GetWindowTextW(hwnd, buf, 256)
            if title_contains.lower() in buf.value.lower():
                results.append((hwnd, buf.value))
        return True
    user32.EnumWindows(WNDENUMPROC(cb), 0)
    return results

def capture_window(title_contains, output_path):
    """Capture a window by title substring."""
    windows = find_window(title_contains)
    if not windows:
        raise ValueError(f"No window matching '{title_contains}'")
    hwnd = windows[0][0]

    class RECT(Structure):
        _fields_ = [('left', c_int), ('top', c_int), ('right', c_int), ('bottom', c_int)]
    rect = RECT()
    user32.GetWindowRect(hwnd, byref(rect))
    w, h = rect.right - rect.left, rect.bottom - rect.top

    with mss.mss() as sct:
        shot = sct.grab({'left': rect.left, 'top': rect.top, 'width': w, 'height': h})
        img = Image.frombytes('RGB', shot.size, shot.rgb)
        img.save(output_path)
        return img

# Usage:
capture_window('Visual Studio Code', 'vscode-capture.png')
```
* *先决条件:* *`pip install mss pillow`**限制：**窗口必须是可见的（不能在其他窗口后面或最小化）。

电子应用程序（VS Code等）

**Node.js仅限剧作家** - Python剧作家没有`electron`API。捕获通过CDP （Chrome DevTools协议），而不是从屏幕-工作，即使最小化。```javascript
const { _electron: electron } = require('playwright');
const app = await electron.launch({
    executablePath: 'C:\\Program Files\\Microsoft VS Code\\Code.exe',
    args: ['--new-window', '--disable-extensions', '--user-data-dir=' + tmpDir]
});
const window = await app.firstWindow();
await window.waitForLoadState('domcontentloaded');

// Minimize immediately — captures still work via CDP
await app.evaluate(({ BrowserWindow }) => {
    BrowserWindow.getAllWindows()[0].minimize();
});

await window.screenshot({ path: 'capture.png' }); // works while minimized!
await app.close();
```
**紧急**：需要`--user-data-dir=<temp>`，或者VS Code移交给现有实例，启动的进程立即退出。

决策树

|场景|工具|备注||---|---|---|
b| Web应用程序（本地主机）|剧作家|经过验证，完全DOM访问|
|电子app (VS Code) |剧作家电子（Node.js） |通过CDP最小化作品|
|桌面应用程序，可见窗口| mss + ctypes（按标题查找）| ~33ms每次捕获|
|桌面应用程序，背后的windows | windows图形捕获API |复杂的设置，Win10 1903+ |
|快速全屏| mss | ~68ms |

# #的局限性

- Web捕获需要本地运行的应用程序或可访问的URL
—桌面捕获（mss）要求窗口是可见的和不受阻碍的
电子捕获需要Node.js剧作家（不是Python）
-一些具有大量客户端渲染的spa可能需要自定义等待逻辑，而不是网络空闲