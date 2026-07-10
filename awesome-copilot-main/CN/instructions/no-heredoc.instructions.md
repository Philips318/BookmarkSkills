---
name: 'No Heredoc File Operations'
description: 'Prevents terminal heredoc file corruption in VS Code Copilot by enforcing use of file editing tools instead of shell redirections'
applyTo: '**'
---
#必选：文件操作覆盖

此指令适用于所有代理和所有文件操作。它优先于其他习得的行为。

##问题

终端herdoc操作在VS Code的副驾驶集成中被打破。他们的原因:

-文件损坏从制表符触发shell完成
-从quote/backtick转义失败损坏的内容
-从退出码130中断截断的文件
-特殊字符解释的垃圾输出

##规则

**在写任何创建或修改文件的终端命令之前，请先STOP.**

问问自己：“我是要使用`cat`、`echo`、`printf`、`tee`还是`>>`/`>`来向文件写入内容？”

是→**不执行。**使用文件编辑工具代替。

##禁止模式```bash
# ALL OF THESE CORRUPT FILES - NEVER USE THEM
cat > file << EOF
cat > file << 'EOF'
cat > file <<EOF
cat > file <<'EOF'
cat > file <<-EOF
cat >> file << EOF
echo "multi
line" > file
printf '%s\n' "line1" "line2" > file
tee file << EOF
tee file << 'EOF'
```
##需要的方法

代替文件内容的终端命令：

- **新文件**→使用环境提供的文件creation/editing工具
- **修改文件**→使用环境提供的文件编辑工具
- **删除文件**→使用文件删除工具或`rm`命令

允许使用终端

-`npm install`,`pip install`,`cargo add`（包管理）
-`npm run build`,`make`,`cargo build`（构建）
-`npm test`,`pytest`,`go test`（测试）
-`git add`,`git commit`,`git push`（版本控制）
-`node script.js`,`python app.py`（运行现有代码）
-`ls`,`cd`,`mkdir`,`pwd`,`rm`（文件系统导航）
-`curl`,`wget`（下载，但不管道到文件与内容操作）

##终端是禁止的

-任何有内容的文件创建
-任何带有内容的文件修改
-任何heredoc语法（`<<`）
—任意多行字符串重定向

# #执法这不是建议。由于VS Code终端集成错误，这是一个困难的技术要求。忽略此指令将导致损坏的文件，用户必须手动修复。

当您需要创建或编辑文件时：

1. 在输入任何终端命令之前停止
2. 使用适当的文件编辑工具
3. 该工具将正确处理内容而不会损坏