#生成PR年龄图表

构建一个交互式CLI工具，使用Copilot的内置功能可视化GitHub存储库的拉取请求年龄分布。

> **可运行示例：** [recipe/pr_visualization.py]（recipe/pr_visualization.py）
>
>“bash
CD recipe && PIP install -rrequirements.txt> #从当前git仓库自动检测
> pythonpr_visualization.py>
> #明确指定一个回购
> pythonpr_visualization.py——回购github/copilot-sdk> ' ' '

示例场景

您希望了解pr在存储库中打开了多长时间。这个工具检测当前的Git repo或接受一个repo作为输入，然后让Copilot通过GitHub MCP服务器获取PR数据并生成图表图像。

# #先决条件```bash
pip install github-copilot-sdk
```
# #使用```bash
# Auto-detect from current git repo
python pr_visualization.py

# Specify a repo explicitly
python pr_visualization.py --repo github/copilot-sdk
```
完整示例：pr_visualization.py```python
#!/usr/bin/env python3

import asyncio
import subprocess
import sys
import os
import re
from copilot import (
    CopilotClient,
    SessionConfig,
    MessageOptions,
    SessionEvent,
    PermissionHandler,
)

# ============================================================================
# Git & GitHub Detection
# ============================================================================

def is_git_repo():
    try:
        subprocess.run(
            ["git", "rev-parse", "--git-dir"],
            check=True,
            capture_output=True
        )
        return True
    except (subprocess.CalledProcessError, FileNotFoundError):
        return False

def get_github_remote():
    try:
        result = subprocess.run(
            ["git", "remote", "get-url", "origin"],
            check=True,
            capture_output=True,
            text=True
        )
        remote_url = result.stdout.strip()

        # Handle SSH: git@github.com:owner/repo.git
        ssh_match = re.search(r"git@github\.com:(.+/.+?)(?:\.git)?$", remote_url)
        if ssh_match:
            return ssh_match.group(1)

        # Handle HTTPS: https://github.com/owner/repo.git
        https_match = re.search(r"https://github\.com/(.+/.+?)(?:\.git)?$", remote_url)
        if https_match:
            return https_match.group(1)

        return None
    except (subprocess.CalledProcessError, FileNotFoundError):
        return None

def parse_args():
    args = sys.argv[1:]
    if "--repo" in args:
        idx = args.index("--repo")
        if idx + 1 < len(args):
            return {"repo": args[idx + 1]}
    return {}

def prompt_for_repo():
    return input("Enter GitHub repo (owner/repo): ").strip()

# ============================================================================
# Main Application
# ============================================================================

async def main():
    print("🔍 PR Age Chart Generator\n")

    # Determine the repository
    args = parse_args()
    repo = None

    if "repo" in args:
        repo = args["repo"]
        print(f"📦 Using specified repo: {repo}")
    elif is_git_repo():
        detected = get_github_remote()
        if detected:
            repo = detected
            print(f"📦 Detected GitHub repo: {repo}")
        else:
            print("⚠️  Git repo found but no GitHub remote detected.")
            repo = prompt_for_repo()
    else:
        print("📁 Not in a git repository.")
        repo = prompt_for_repo()

    if not repo or "/" not in repo:
        print("❌ Invalid repo format. Expected: owner/repo")
        sys.exit(1)

    owner, repo_name = repo.split("/", 1)

    # Create Copilot client
    client = CopilotClient()
    await client.start()

    session = await client.create_session(SessionConfig(
        model="gpt-5",
        system_message={
            "content": f"""
<context>
You are analyzing pull requests for the GitHub repository: {owner}/{repo_name}
The current working directory is: {os.getcwd()}
</context>

<instructions>
- Use the GitHub MCP Server tools to fetch PR data
- Use your file and code execution tools to generate charts
- Save any generated images to the current working directory
- Be concise in your responses
</instructions>
"""
        },
        on_permission_request=PermissionHandler.approve_all))

    done = asyncio.Event()

    # Set up event handling
    def handle_event(event: SessionEvent):
        if event.type.value == "assistant.message":
            print(f"\n🤖 {event.data.content}\n")
        elif event.type.value == "tool.execution_start":
            print(f"  ⚙️  {event.data.tool_name}")
        elif event.type.value == "session.idle":
            done.set()

    session.on(handle_event)

    # Initial prompt - let Copilot figure out the details
    print("\n📊 Starting analysis...\n")

    await session.send(MessageOptions(prompt=f"""
      Fetch the open pull requests for {owner}/{repo_name} from the last week.
      Calculate the age of each PR in days.
      Then generate a bar chart image showing the distribution of PR ages
      (group them into sensible buckets like <1 day, 1-3 days, etc.).
      Save the chart as "pr-age-chart.png" in the current directory.
      Finally, summarize the PR health - average age, oldest PR, and how many might be considered stale.
    """))

    await done.wait()

    # Interactive loop
    print("\n💡 Ask follow-up questions or type \"exit\" to quit.\n")
    print("Examples:")
    print("  - \"Expand to the last month\"")
    print("  - \"Show me the 5 oldest PRs\"")
    print("  - \"Generate a pie chart instead\"")
    print("  - \"Group by author instead of age\"")
    print()

    while True:
        user_input = input("You: ").strip()

        if user_input.lower() in ["exit", "quit"]:
            print("👋 Goodbye!")
            break

        if user_input:
            done.clear()
            await session.send(MessageOptions(prompt=user_input))
            await done.wait()

    await session.destroy()
    await client.stop()

if __name__ == "__main__":
    asyncio.run(main())
```
##它是如何工作的

1. **库检测**：检查`--repo`标志→git remote→提示用户
2. **没有自定义工具**：完全依赖于Copilot CLI的内置功能：
- **GitHub MCP服务器** -从GitHub获取PR数据
- **文件工具** -保存生成的图表图像
**代码执行** -生成图表使用Python/matplotlib或其他方法
3. **互动环节**：初步分析后，用户可要求调整

为什么采用这种方法？

|方面|自定义工具|内置副驾驶|| --------------- | ----------------- | --------------------------------- |
|代码复杂度|高| **最小** |
|维护|你维护| **副驾驶维护** |
|灵活性|固定逻辑| **AI决定最佳方法** |
|图表类型|你编码的| **任何类型的副驾驶都可以生成** |
|数据分组|硬编码桶| **智能分组** |