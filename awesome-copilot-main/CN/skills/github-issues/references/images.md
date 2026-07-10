#图片在问题和评论

如何通过CLI在GitHub问题体和注释中嵌入图像。

##方法（按可靠性排序）

# # # 1。GitHub Contents API（推荐用于私有仓库）

将图像文件推送到同一个repo中的分支，然后使用一个适用于经过身份验证的查看者的URL引用它们。

步骤1：创建分支```bash
# Get the SHA of the default branch
SHA=$(gh api repos/{owner}/{repo}/git/ref/heads/main --jq '.object.sha')

# Create a new branch
gh api repos/{owner}/{repo}/git/refs -X POST \
  -f ref="refs/heads/{username}/images" \
  -f sha="$SHA"
```
**第二步：通过Contents API上传图片**```bash
# Base64-encode the image and upload
BASE64=$(base64 -i /path/to/image.png)

gh api repos/{owner}/{repo}/contents/docs/images/my-image.png \
  -X PUT \
  -f message="Add image" \
  -f content="$BASE64" \
  -f branch="{username}/images" \
  --jq '.content.path'
```
对每个图像重复。Contents API为每个文件创建一个提交。

**步骤3：参考降价**```markdown
![Description](https://github.com/{owner}/{repo}/raw/{username}/images/docs/images/my-image.png)
```
b> **重要：**使用`github.com/{owner}/{repo}/raw/{branch}/{path}`格式，而不是`raw.githubusercontent.com`。`raw.githubusercontent.com`url对于私有仓库返回404。`github.com/.../raw/...`格式之所以有效，是因为浏览器会在查看器登录并具有仓库访问权限时发送验证cookie。

**优点：**适用于查看器可以访问的任何repo，图像生活在版本控制中，没有过期。
**缺点：**创建提交，查看者必须经过身份验证，图像不会在电子邮件通知或没有repo访问的用户中呈现。

# # # 2。Gist托管（仅限公开图片）

以文件的形式上传图片。只适用于你愿意公开的照片。```bash
# Create a gist with a placeholder file
gh gist create --public -f description.md <<< "Image hosting gist"

# Note: gh gist edit does NOT support binary files.
# You must use the API to add binary content to gists.
```
**限制：** gist不支持通过CLI上传二进制文件。您需要base64编码并存储为文本，这将不会呈现为图像。不推荐。

# # # 3。浏览器上传（最可靠的呈现）

获得永久图像url的最可靠方法是通过GitHub web UI：

1. 在浏览器中打开issue/comment2. 将图像拖拽或粘贴到注释编辑器中
3. GitHub生成一个永久的`https://github.com/user-attachments/assets/{UUID}`URL
4. 这些url适用于任何人，甚至没有回购访问权限，并呈现在电子邮件通知中

** GitHub的`upload/policies/assets`端点需要一个浏览器会话（CSRF令牌+ cookie）。当使用API令牌调用时，它返回一个HTML错误页面。没有用于生成`user-attachments`url的公共API。

##以编程方式截图

使用`puppeteer-core`与本地Chrome截图HTML模型：```javascript
const puppeteer = require('puppeteer-core');

const browser = await puppeteer.launch({
  executablePath: '/Applications/Google Chrome.app/Contents/MacOS/Google Chrome',
  defaultViewport: { width: 900, height: 600, deviceScaleFactor: 2 }
});

const page = await browser.newPage();
await page.setContent(htmlString);

// Screenshot specific elements
const elements = await page.$$('.section');
for (let i = 0; i < elements.length; i++) {
  await elements[i].screenshot({ path: `mockup-${i + 1}.png` });
}

await browser.close();
```
b> **注意：**由于网络隔离，MCP剧作家可能无法连接到本地主机。使用puppeteer-core与本地Chrome安装代替。

##快速参考

|方法|私有回购|永久|不需要授权| api -仅||--------|:---:|:---:|:---:|:---:|
|内容API +`github.com/raw/`|✅|✅|❌|✅|
|浏览器拖放（`user-attachments`） |✅|✅|✅|❌|
|`raw.githubusercontent.com`|❌（404）|✅|❌|✅|
| Gist | Public only |✅|✅|❌（无二进制）|

##常见陷阱

**`raw.githubusercontent.com`返回404私有repos**，即使在URL中有一个有效的令牌。GitHub的CDN不通过认证头。
- **API下载url是临时的。**使用`download_url`返回的url包含一个过期令牌。
- **`upload/policies/assets`需要浏览器会话。**不要尝试从CLI调用这个端点。
- **Base64编码的大文件**可以达到API有效载荷限制。Contents API有一个~100MB的文件大小限制，但是对于base64编码的有效负载，实际限制更低。
- **电子邮件通知**不会渲染需要认证的图像。如果邮件的可读性很重要，请使用浏览器上传的方法。