#铬控制画布

一个GitHub Copilot画布，驱动一个真正的**头铬**窗口通过剧作家。
主机应用程序的内置`browser`画布是WebKit (WKWebView)；这给了你实际的
铬，可由面板UI和代理控制。

画布面板是一个控制条（URL栏，back/forward/reload，屏幕截图）。一个单独的
Chromium窗口进行真正的渲染，因为你不能将Chromium嵌入到WebKit中
iframe。

# #文件`extension.mjs`-扩展：canvas声明，剧作家启动，环回HTTP
用于面板和代理操作的服务器。
-`index.html`-面板渲染的控制条UI。
-`package.json`-声明`playwright`依赖和`"type": "module"`。
-`copilot-extension.json`-name/version元数据。

# #先决条件- **Node.js20.19或更新** （Copilot SDK需要`node ^20.19.0 || >=22.12.0`）。
扩展作为Node子进程运行。
-应用程序的画布/ ui扩展实验启用**。没有它，扩展
加载，但画布从未出现在面板中。在应用程序中启用它
设置→实验。（这可能不适用于所有帐户。）

# #安装

将这个文件夹放到`~/.copilot/extensions/chromium-control-canvas/`（用户范围）或repo's中`.github/extensions/chromium-control-canvas/`（项目范围），然后安装依赖项和
从您复制的文件夹内的Chromium二进制文件：```sh
# User scope
cd ~/.copilot/extensions/chromium-control-canvas

# Or project scope, from the repository root
cd .github/extensions/chromium-control-canvas

npm install                     # playwright is declared in package.json
npx playwright install chromium # downloads the browser, a few hundred MB
```
在应用程序中重新加载扩展，然后打开`chromium-control-canvas`画布。

注意：复制扩展文件只会放置源代码。它** ** **不运行
上面的命令或启用实验，因此这些步骤仍然是必需的
设置。

附加到您自己的Chrome浏览器

默认情况下，画布会启动带有持久配置文件的绑定的Chromium。开车
你已经在运行的Chrome，用调试端口启动它，并传递`cdpUrl`打开画布时：```sh
google-chrome --remote-debugging-port=9222   # then open the canvas with cdpUrl: http://localhost:9222
```
在这种模式下，扩展通过CDP连接，永远不会启动或杀死您的浏览器；
关闭画布只是断开连接。

代理操作

—`navigate { url }`—跳转URL或搜索查询（blocklist- protected）。
-`back`/`forward`/`reload`-历史导航。
-`current_url`-当前的URL和页面标题。
-`snapshot`-结构化的可见交互元素列表，每个元素都有一个稳定的ref。
-`click { ref | selector }`-通过快照ref或CSS选择器单击一个元素。
-`type { ref | selector, text, submit? }`-填充输入；可选地按“Enter”。
-`screenshot { fullPage? }`-保存PNG到`artifacts/`，并返回其路径和大小。

# #笔记—持久配置文件存储在`$COPILOT_HOME/extensions/chromium-control-canvas/profile`(默认`~/.copilot/extensions/chromium-control-canvas/profile`)，因此重新启动后登录仍然有效。
**不要提交或共享此文件夹** -它包含真正的会话cookie。
-原始`evaluate`（任意页内JS）故意省略。
-`navigate`是根据一个阻塞列表进行检查的，并且请求拦截器也会阻塞
通过页面内重定向导航到被阻止的主机。的运`BLOCKLIST`条目是说明性的例子，而不是真正的报道-在其中编辑列表`extension.mjs`以适应您的环境。
环回控制服务器需要每次启动令牌（模板化到面板中），
因此浏览器中的其他页面无法驱动它。
—输入的文本（如密码）在`audit.log`中进行编辑，密码字段值为
快照中不包含。
-在运行时生成，而不是源代码的一部分：在复制的`node_modules/`扩展文件夹，加上`profile/`，`artifacts/`和`audit.log`und呃`$COPILOT_HOME/extensions/chromium-control-canvas/`。