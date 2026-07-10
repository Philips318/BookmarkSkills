#在Windows上发布长评论和正文

在Windows上，`az`命令解析为`az.cmd`，一个由`cmd.exe`调用的批处理包装器。整个命令行的上限为8191个字符，因此长`--discussion`、`--description`或`--content`值可以被静默截断或失败。在编写长参数和路由之前检测shell。跳过这一点是代理花费3-5个回合返回到原始令牌检索和REST调用的最常见原因。

##首先检测shell

|环境|信号|动作||---|---|---|
|`$IsWindows -eq $true`和`$PSVersionTable.PSVersion`设置|使用`azps.ps1`（见下文）|
| macOS / Linux上的PowerShell |`$IsWindows -eq $false`| Plain`az`是可以的，没有cmd.exe包装|
b| bash / zsh / sh |`$BASH_VERSION`或`$ZSH_VERSION`set，或`uname`works | Plain`az`可以，没有cmd.exe包装|
| Windows`cmd.exe`|`%ComSpec%`以`cmd.exe`结尾，不以`$PSVersionTable`结尾|如果安装了PowerShell，则使用`azps.ps1`，否则参见|下面的`az devops invoke`##选项1:`azps.ps1`（Windows上的PowerShell）`azps.ps1`随Azure CLI安装程序一起提供，并直接调用Python入口点。无`cmd.exe`长度上限。```powershell
# Read the long body into a variable and pass it through. No quoting headaches.
$body = Get-Content -Raw .\comment.md
azps.ps1 boards work-item update --id 1234 --discussion $body
```
选项2:Azure CLI提供的专用`--file-path`标志

有些命令有本机文件标志，你应该更喜欢它而不是任何内联体：

—`az devops wiki page create`和`az devops wiki page update`取`--file-path`（可选配`--encoding`）。
-在任何shell上使用它，包括Windows。```bash
az devops wiki page create --path 'My page' --wiki myproject --file-path ./page.md --encoding utf-8
```
##选项3:`az devops invoke`fallback

当不存在`--file-path`时（工作项`--discussion`， PR`--description`），并且您不在PowerShell中，通过底层REST API发布主体。`az devops invoke`在Python入口点内运行，因此它也不受`cmd.exe`上限的约束，并且它从带有`--in-file`的文件中获取请求体：```bash
# Post a long discussion comment to work item 1234.
# REST: POST /{project}/_apis/wit/workItems/{id}/comments?api-version=7.0-preview.3
az devops invoke \
  --area wit --resource comments \
  --route-parameters project={project} workItemId=1234 \
  --api-version 7.0-preview.3 \
  --http-method POST \
  --in-file ./comment.json
```
式中`comment.json`=`{ "text": "<long markdown body>" }`。这是在`azps.ps1`和`--file-path`都不可用时的通用逃生舱口。`az devops invoke`本身接受`--in-file`。

##对于普通字符串参数，不要依赖`@<file>`Azure CLI`@<file>`约定记录了JSON参数（参见[官方引用指南](https://learn.microsoft.com/en-us/cli/azure/use-azure-cli-successfully-quoting)）。它不能保证展开普通的字符串参数，比如`--discussion`或`--description`，所以不要用它来代替上面的三个选项。