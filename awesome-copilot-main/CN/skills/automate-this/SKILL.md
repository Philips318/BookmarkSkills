---
name: automate-this
description: 'Analyze a screen recording of a manual process and produce targeted, working automation scripts. Extracts frames and audio narration from video files, reconstructs the step-by-step workflow, and proposes automation at multiple complexity levels using tools already installed on the user machine.'
---
#自动化

分析手动过程的屏幕记录，并为其构建工作自动化。

用户记录下自己做的一些重复或乏味的事情，把视频文件交给你，你就知道他们在做什么，为什么，以及如何用脚本把它删掉。

##前提条件检查

在分析任何记录之前，请验证所需的工具是否可用。静静地运行这些检查，只处理表面问题：```bash
command -v ffmpeg >/dev/null 2>&1 && ffmpeg -version 2>/dev/null | head -1 || echo "NO_FFMPEG"
command -v whisper >/dev/null 2>&1 || command -v whisper-cpp >/dev/null 2>&1 || echo "NO_WHISPER"
```
- **ffmpeg是必需的。**如果缺少，告诉用户：`brew install ffmpeg`（macOS）或对应的操作系统。
- **可选。**只有当录音有旁白时才需要。如果缺失并且录音有音轨，建议：`pip install openai-whisper`或`brew install whisper-cpp`。如果用户拒绝，只进行视觉分析。

阶段1：从记录中提取内容

给定一个视频文件路径（通常在`~/Desktop/`），提取视觉帧和音频：

帧提取

每2秒提取一帧。这平衡了上下文窗口限制的覆盖率。```bash
WORK_DIR=$(mktemp -d "${TMPDIR:-/tmp}/automate-this-XXXXXX")
chmod 700 "$WORK_DIR"
mkdir -p "$WORK_DIR/frames"
ffmpeg -y -i "<VIDEO_PATH>" -vf "fps=0.5" -q:v 2 -loglevel warning "$WORK_DIR/frames/frame_%04d.jpg"
ls "$WORK_DIR/frames/" | wc -l
```
对会话中的所有后续临时文件路径使用`$WORK_DIR`。模式为0700的每运行目录确保提取的帧只能由当前用户读取。

如果录制时间超过5分钟（超过150帧），请将间隔增加到每4秒一帧，以保持在上下文限制内。告诉用户你的采样频率降低了。

音频提取和转录

检查视频是否有音轨：```bash
ffprobe -i "<VIDEO_PATH>" -show_streams -select_streams a -loglevel error | head -5
```
如果音频存在：```bash
ffmpeg -y -i "<VIDEO_PATH>" -ac 1 -ar 16000 -loglevel warning "$WORK_DIR/audio.wav"

# Use whichever whisper binary is available
if command -v whisper >/dev/null 2>&1; then
  whisper "$WORK_DIR/audio.wav" --model small --language en --output_format txt --output_dir "$WORK_DIR/"
  cat "$WORK_DIR/audio.txt"
elif command -v whisper-cpp >/dev/null 2>&1; then
  whisper-cpp -m "$(brew --prefix 2>/dev/null)/share/whisper-cpp/models/ggml-small.bin" -l en -f "$WORK_DIR/audio.wav" -otxt -of "$WORK_DIR/audio"
  cat "$WORK_DIR/audio.txt"
else
  echo "NO_WHISPER"
fi
```
如果whisper二进制文件都不可用，并且录音有音频，则通知用户他们缺少叙述上下文，并询问他们是否要安装whisper （`pip install openai-whisper`或`brew install whisper-cpp`）或继续进行仅可视分析。

阶段2：重构流程

分析提取的框架（如果有的话，还有文本），以构建对用户行为的结构化理解。按顺序处理这些框架并确定：1. **使用的应用程序** -哪些应用程序出现在录音中？（浏览器、终端、Finder、邮件客户端、电子表格、IDE等）
2. **操作顺序** -用户按顺序做了什么？点击的,上竞争逐步。
3. **数据流** -哪些信息在步骤之间移动？（复制文本、下载文件、表单输入等）
4. **决策点** -是否存在用户暂停，检查某些内容或做出选择的时刻？
5. **重复模式** -用户是否使用不同的输入多次执行相同的操作？
6. **痛点**——流程中哪些地方看起来缓慢、容易出错或乏味？旁白往往直接揭示了这一点（“我讨厌这部分”，“这总是花很长时间”，“我必须为每一个人做这件事”）。将此重建作为编号的步骤列表呈现给用户，并在建议自动化之前要求他们确认其准确性。这是至关重要的——错误的理解会导致无用的自动化。

格式:```
Here's what I see you doing in this recording:

1. Open Chrome and navigate to [specific URL]
2. Log in with credentials
3. Click through to the reporting dashboard
4. Download a CSV export
5. Open the CSV in Excel
6. Filter rows where column B is "pending"
7. Copy those rows into a new spreadsheet
8. Email the new spreadsheet to [recipient]

You repeated steps 3-8 three times for different report types.

[If narration was present]: You mentioned that the export step is the slowest
part and that you do this every Monday morning.

Does this match what you were doing? Anything I got wrong or missed?
```
在用户确认重建是准确的之前，不要进行第3阶段。

阶段3：环境指纹

在提出自动化之前，要了解用户实际需要使用的是什么。运行这些检查：```bash
echo "=== OS ===" && uname -a
echo "=== Shell ===" && echo $SHELL
echo "=== Python ===" && { command -v python3 && python3 --version 2>&1; } || echo "not installed"
echo "=== Node ===" && { command -v node && node --version 2>&1; } || echo "not installed"
echo "=== Homebrew ===" && { command -v brew && echo "installed"; } || echo "not installed"
echo "=== Common Tools ===" && for cmd in curl jq playwright selenium osascript automator crontab; do command -v $cmd >/dev/null 2>&1 && echo "$cmd: yes" || echo "$cmd: no"; done
```
使用它将建议约束到用户已经拥有的工具。永远不要提出需要安装5个新东西的自动化方案，除非简单的方法确实行不通。

阶段4：提出自动化

基于重构过程和用户环境，提出多达三层的自动化。并不是每个过程都需要三层使用判断。

层结构

**第一层-快速获胜（5分钟内设置）**
最小的有用的自动化。shell别名、一行代码、键盘快捷键和AppleScript代码片段。自动化单个最痛苦的步骤，而不是整个过程。

**Tier 2 -脚本（30分钟内设置）**
一个独立的脚本（bash、Python或Node——无论用户拥有哪个），端到端自动化整个过程。处理常见错误。可以在需要时手动运行。**第三层-完全自动化（2小时内设置）**
来自第2层的脚本，加上：计划执行（cron、launchd或GitHub Actions）、日志记录、错误通知和任何必要的集成脚手架（API密钥、认证令牌等）。

提案格式

对于每一层，提供：```
## Tier [N]: [Name]

**What it automates:** [Which steps from the reconstruction]
**What stays manual:** [Which steps still need a human]
**Time savings:** [Estimated time saved per run, based on the recording length and repetition count]
**Prerequisites:** [Anything needed that isn't already installed — ideally nothing]

**How it works:**
[2-3 sentence plain-English explanation]

**The code:**
[Complete, working, commented code — not pseudocode]

**How to test it:**
[Exact steps to verify it works, starting with a dry run if possible]

**How to undo:**
[How to reverse any changes if something goes wrong]
```
特定于应用的自动化策略

根据记录中出现的应用程序使用以下策略：

* *基于浏览器的工作流:* *
-第一选择：检查网站是否有公共API。API调用比浏览器自动化可靠10倍。搜索API文档。
-第二个选择：`curl`或`wget`对于具有已知端点的简单HTTP请求。
第三个选择：需要点击UI的工作流的剧作家或硒。更喜欢剧作家——它更快，更稳定。
-寻找模式：如果用户从仪表板反复下载相同的报告，它几乎肯定可以通过API或直接URL查询参数。**电子表格和数据工作流：**
-使用pandas进行数据过滤、转换和聚合。
-如果用户在Excel中做简单的列操作，一个5行Python脚本取代整个手动过程。`csvkit`用于快速命令行CSV操作而无需编写代码。
—如果需要以Excel格式输出，请使用“openpyxl”。

* *电子邮件工作流:* *
—macOS:`osascript`可以控制邮件。应用程序发送电子邮件与附件。
跨平台：Python`smtplib`用于发送，`imaplib`用于读取。
—如果邮件遵循模板，则从模板文件中生成正文，并使用变量替换。

**文件管理工作流：**
-move/copy/rename模式的Shell脚本。
-`find`+`xargs`表示批处理操作。
-`fswatch`或`watchman`用于变更触发自动化。
-如果用户按日期或类型将文件组织到文件夹中，这是一个3行shell脚本。* *Terminal/CLI工作流:* *
-频繁输入命令的Shell别名。
-多步骤序列的Shell函数。
-为特定于项目的任务集制作文件。
-如果用户用不同的参数运行相同的命令，这是一个循环。

* * macOS-specific工作流:* *
-AppleScript/JXA用于控制本地应用程序（邮件，日历，Finder，预览等）。
-快捷方式。应用程序用于简单的多应用程序工作流，不需要代码。
-`automator`用于基于文件的工作流。
-`launchd`plist文件用于计划任务（优先于macOS上的cron）。**跨应用程序工作流（数据在应用程序之间移动）：**
—确定数据传输点。每一次转移都是一次自动化的机会。
-录音中基于剪贴板的传输表明应用程序不相互通信-寻找api，基于文件的切换，或直接集成代替。
-如果用户从App A复制并粘贴到App B，则自动化应直接从A的数据源读取并写入B的输入格式。

提出有针对性的建议

将这些原则应用到每一个提案中：

1. **首先自动化瓶颈。**录音中的旁白和计时显示了哪一步是真正痛苦的。把最糟糕的步骤自动化30秒胜过把整个过程自动化2小时。2. **匹配用户的技能水平。**如果录音显示某人在终端上很舒服，建议使用shell脚本。如果显示有人在gui中导航，建议使用一个简单的触发器（双击一个脚本，运行一个快捷方式，或者键入一个命令）。

3. **估计实时节省。**计算记录持续时间并乘以他们这样做的频率。“这段录音是4分钟。你说你每天都这样。也就是每年17个小时。一级将时间缩短到每次30秒——你可以得到16个小时的时间。”

4. **处理80%的情况。**自动化的第一个版本应该完美地覆盖公共路径。边缘情况可以在Tier 3中处理，或者标记为手动干预。

5. **保留人类检查点。**如果记录显示用户在过程中审查或批准某些内容，则将其保留为手动步骤。不要自动做出判断。6. **建议演练。**每个脚本都应该有一个模式来显示它在不执行该操作的情况下会做什么。`--dry-run`标志、预览输出或破坏性操作之前的确认提示。

7. **说明真实和秘密。**如果过程涉及登录或使用凭证，永远不要硬编码它们。使用环境变量、钥匙链访问（macOS`security`命令）或在运行时提示它们。

8. **考虑失效模式。**如果网站宕机怎么办？如果文件不存在？如果格式改变了？好的提案会提到这一点并加以处理。

阶段5：构建和测试

当用户选择一层时：1. 将完整的自动化代码写入一个文件（建议一个合理的位置—如果存在，则为用户的项目目录，否则为`~/Desktop/`）。
2. 在用户的注视下演练或测试。
3. 如果测试工作，展示如何实际运行它。
4. 如果失败了，诊断并修复——不要在一次尝试后就放弃。

# #清理

分析完成后（无论结果如何），清理提取的帧和音频：```bash
rm -rf "$WORK_DIR"
```
告诉用户您正在清理临时文件，这样他们就知道不会留下任何东西。