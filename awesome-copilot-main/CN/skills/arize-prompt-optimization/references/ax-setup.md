# ax CLI -故障排除

只有当`ax`命令失败时，才参考此命令。不要主动运行这些检查。

##先检查版本

如果安装了`ax`（而不是`command not found`），在进一步研究之前始终运行`ax --version`。版本必须是`0.14.0`或更高版本-许多错误是由过时的安装引起的。如果版本过老，请参见下面的**版本过老**。

# #`ax: command not found`* *macOS/Linux: * *
1. 检查常见位置：`~/.local/bin/ax`，`~/Library/Python/*/bin/ax`2. 安装：`uv tool install arize-ax-cli`（首选）、`pipx install arize-ax-cli`或`pip install arize-ax-cli`3. 如果需要，添加到PATH:`export PATH="$HOME/.local/bin:$PATH"`Windows (PowerShell): * * * *
1. 检查：`Get-Command ax`或`where.exe ax`2. 常见位置：`%APPDATA%\Python\Scripts\ax.exe`，`%LOCALAPPDATA%\Programs\Python\Python*\Scripts\ax.exe`3. 安装:`pip install arize-ax-cli`4. 添加到PATH:`$env:PATH = "$env:APPDATA\Python\Scripts;$env:PATH"`版本太老（低于0.14.0）

升级版本：`uv tool install --force --reinstall arize-ax-cli`、`pipx upgrade arize-ax-cli`或`pip install --upgrade arize-ax-cli`##SSL/certificate错误

—macOS:`export SSL_CERT_FILE=/etc/ssl/cert.pem`—Linux:`export SSL_CERT_FILE=/etc/ssl/certs/ca-certificates.crt`—回退：`export SSL_CERT_FILE=$(python -c "import certifi; print(certifi.where())")`无法识别子命令升级ax（见上文）或使用最近的可用替代方案。

##仍然失败

停下来向用户寻求帮助。