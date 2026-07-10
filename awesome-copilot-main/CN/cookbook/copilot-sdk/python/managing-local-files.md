#按元数据分组文件

使用Copilot可以根据元数据智能地组织文件夹中的文件。

> **可运行示例：** [recipe/managing_local_files.py]（recipe/managing_local_files.py）
>
>“bash
CD recipe && PIP install -rrequirements.txt> pythonmanaging_local_files.py> ' ' '

示例场景

您有一个包含许多文件的文件夹，并且希望根据元数据（如文件类型、创建日期、大小或其他属性）将它们组织到子文件夹中。Copilot可以分析文件并建议或执行分组策略。

##示例代码```python
import asyncio
import os
from copilot import (
    CopilotClient,
    SessionConfig,
    MessageOptions,
    SessionEvent,
    PermissionHandler,
)

async def main():
    # Create and start client
    client = CopilotClient()
    await client.start()

    # Create session
    session = await client.create_session(SessionConfig(model="gpt-5",
        on_permission_request=PermissionHandler.approve_all))

    done = asyncio.Event()

    # Event handler
    def handle_event(event: SessionEvent):
        if event.type.value == "assistant.message":
            print(f"\nCopilot: {event.data.content}")
        elif event.type.value == "tool.execution_start":
            print(f"  → Running: {event.data.tool_name}")
        elif event.type.value == "tool.execution_complete":
            print(f"  ✓ Completed: {event.data.tool_call_id}")
        elif event.type.value == "session.idle":
            done.set()

    session.on(handle_event)

    # Ask Copilot to organize files
    target_folder = os.path.expanduser("~/Downloads")

    await session.send(MessageOptions(prompt=f"""
Analyze the files in "{target_folder}" and organize them into subfolders.

1. First, list all files and their metadata
2. Preview grouping by file extension
3. Create appropriate subfolders (e.g., "images", "documents", "videos")
4. Move each file to its appropriate subfolder

Please confirm before moving any files.
"""))

    await done.wait()

    await session.destroy()
    await client.stop()

if __name__ == "__main__":
    asyncio.run(main())
```
分组策略

###通过文件扩展名```python
# Groups files like:
# images/   -> .jpg, .png, .gif
# documents/ -> .pdf, .docx, .txt
# videos/   -> .mp4, .avi, .mov
```
###按创建日期```python
# Groups files like:
# 2024-01/ -> files created in January 2024
# 2024-02/ -> files created in February 2024
```
###按文件大小```python
# Groups files like:
# tiny-under-1kb/
# small-under-1mb/
# medium-under-100mb/
# large-over-100mb/
```
##干式运行模式

为了安全起见，您可以要求Copilot只预览更改：```python
await session.send(MessageOptions(prompt=f"""
Analyze files in "{target_folder}" and show me how you would organize them
by file type. DO NOT move any files - just show me the plan.
"""))
```
使用AI分析自定义分组

让Copilot根据文件内容确定最佳分组：```python
await session.send(MessageOptions(prompt=f"""
Look at the files in "{target_folder}" and suggest a logical organization.
Consider:
- File names and what they might contain
- File types and their typical uses
- Date patterns that might indicate projects or events

Propose folder names that are descriptive and useful.
"""))
```
##安全考虑

1. **移动前确认**：要求副驾驶在执行移动前确认
2. **处理重复**：考虑如果存在同名的文件会发生什么
3. **保存原件**：考虑复制而不是移动重要文件