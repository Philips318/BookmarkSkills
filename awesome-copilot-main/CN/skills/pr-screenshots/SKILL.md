---
name: pr-screenshots
description: 'Embed before/after screenshots and annotated images in pull request descriptions. Covers PR description patterns, image upload for Azure DevOps and GitHub, and sizing best practices.'
---
# PR截图

在拉取请求描述中嵌入before/after截图，这样审阅者就可以在不签出分支的情况下看到视觉变化。

何时使用此技能

当PR改变某些可见内容时使用此技能：

-布局，样式，CSS
-图表，仪表板，数据可视化
- UI组件、表单、模态
—错误信息、CLI输出、日志格式化

PR描述模式

将截图直接放在PR描述正文中。避免用`<details>`折叠方式包装它们——审阅者更有可能看到他们不点击就能看到的图像。```markdown
**Before** — brief description of the problem:

![before](url-to-before-image)

**After** — brief description of the fix:

![after](url-to-after-image)
```
文章要简短。每张图片用一两句话描述读者应该注意的内容。让图像承载大部分的交流。

###多次更改

对于有几个视觉变化的pr，使用单独的before/after对和标题：```markdown
## Filter bar alignment

**Before** — 1px border clash between adjacent buttons:

![before-filters](url)

**After** — borders overlap cleanly, hover tint added:

![after-filters](url)

## Chart tooltip

**Before** — tooltip clipped at container edge:

![before-tooltip](url)

**After** — tooltip repositions to stay visible:

![after-tooltip](url)
```
##图片大小

- **以原生1x分辨率截图** -不调整PIL大小（创建工件）
- **控制显示尺寸在HTML**当图像太大：  ```html
  <img src="url" width="600" alt="description">
  ```
**Before/after对必须使用相同的视口宽度和裁剪** -否则比较是没有意义的

##上传图片

Azure DevOps

通过REST API将图像作为PR附件上传：```powershell
$token = az account get-access-token `
    --resource "499b84ac-1321-427f-aa17-267ca6975798" `
    --query accessToken -o tsv

$base = "https://{org}.visualstudio.com/{projectId}/_apis/git/repositories/{repoId}"
$url = "$base/pullRequests/{prId}/attachments/screenshot.png?api-version=7.1-preview.1"

# Use HttpClient — Invoke-RestMethod can corrupt binary data
$client = New-Object System.Net.Http.HttpClient
$client.DefaultRequestHeaders.Authorization = `
    New-Object System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", $token)
$content = New-Object System.Net.Http.ByteArrayContent(
    , [System.IO.File]::ReadAllBytes("screenshot.png")
)
$content.Headers.ContentType = `
    [System.Net.Http.Headers.MediaTypeHeaderValue]::new("application/octet-stream")
$resp = $client.PostAsync($url, $content).Result
```
PR描述中的参考：```markdown
![description](https://{org}.visualstudio.com/{projectId}/_apis/git/repositories/{repoId}/pullRequests/{prId}/attachments/screenshot.png)
```
**Azure DevOps陷阱：**

- **使用`{org}.visualstudio.com`而不是`dev.azure.com/{org}`** - AzDO的markdown渲染器使用`.visualstudio.com`。`dev.azure.com`格式的加载速度明显较慢
使用`POST`而不是`PUT`（PUT返回405）
—API版本必须为“`7.1-preview.1`”
-不能用相同的文件名重新上传-使用新名称（例如`screenshot-v2.png`）
-使用`HttpClient`而不是`Invoke-RestMethod`- IRM会破坏二进制数据
- repo -相对路径在PR描述中不起作用-必须使用完整的url
不要仅仅为了PR截图而向分支提交图像

# # # GitHub

> **⚠️工作正在进行中。** GitHub的拖放图像上传使用需要浏览器cookie的内部端点。目前还没有清晰的公共API来上传图片到PR描述中。

**提交图像到`pr-assets`孤儿分支，并通过blob url引用（`github.com/{owner}/{repo}/blob/pr-assets/{file}?raw=true`）。它可以工作，但很笨拙——欢迎为更好的方法贡献意见。

# #指南1. **在进行更改之前捕获状态** -很容易忘记，并且稍后重建原始状态缓慢且容易出错
2. 保持描述简短——每张图片用一两句话指出改变的地方就足够了
3. **更喜欢可见的图像折叠部分** -屏幕截图后面的`<details>`标签很容易跳过
4. **在细微变化时进行注释** -使用`image-annotations`技能在差异不是立即明显时添加标注
5. **匹配before/after对之间的视口和裁剪**，这样比较是有意义的

# #的局限性

- GitHub图像上传需要解决方案（没有公共API用于PR描述图像）
- Azure DevOps附件文件名不能重复使用-提前计划命名
-非常大的图像（bbb10mb）可能无法在某些平台上内联呈现