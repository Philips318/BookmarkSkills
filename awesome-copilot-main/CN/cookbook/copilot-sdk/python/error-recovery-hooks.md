#错误恢复钩子

当工具失败时，LLM要继续调查，而不是放弃部分结果。

# #的问题

当shell命令返回错误或文件操作遇到权限拒绝时，LLM倾向于停止并道歉，而不是尝试不同的方法。这在弹性很重要的代理工作流中产生了不完整的结果。

# #解决方案

使用SDK的钩子系统（`on_post_tool_use`,`on_error_occurred`）按类别对工具结果进行分类，并附加继续指令，以推动LLM继续运行。```python
from enum import Enum


class ToolResultCategory(str, Enum):
    SHELL_ERROR = "shell_error"
    PERMISSION_DENIED = "permission_denied"
    NORMAL = "normal"


class SDKErrorCategory(str, Enum):
    CLIENT_ERROR = "client_error"       # 4xx — not retryable
    TRANSIENT = "transient"             # 5xx / timeout
    NON_RECOVERABLE = "non_recoverable"


# Phrases that signal permission issues in tool output
PERMISSION_DENIAL_PHRASES = [
    "permission denied",
    "access denied",
    "not permitted",
    "operation not allowed",
    "eacces",
    "eperm",
    "403 forbidden",
]

SHELL_ERROR_PHRASES = [
    "command not found",
    "no such file or directory",
    "exit code",
    "errno",
    "traceback",
]

CONTINUATION_MESSAGES = {
    ToolResultCategory.SHELL_ERROR: (
        "\n\n[SYSTEM NOTE: This command encountered an error. "
        "This does NOT mean you should stop. Retry with different "
        "arguments, try a different tool, or move on.]"
    ),
    ToolResultCategory.PERMISSION_DENIED: (
        "\n\n[SYSTEM NOTE: Permission was denied for this specific "
        "action. Continue using alternative approaches.]"
    ),
}


def classify_tool_result(tool_name: str, result_text: str) -> ToolResultCategory:
    result_lower = result_text.lower()
    if any(phrase in result_lower for phrase in PERMISSION_DENIAL_PHRASES):
        return ToolResultCategory.PERMISSION_DENIED
    if any(phrase in result_lower for phrase in SHELL_ERROR_PHRASES):
        return ToolResultCategory.SHELL_ERROR
    return ToolResultCategory.NORMAL


def classify_sdk_error(error_msg: str, recoverable: bool) -> SDKErrorCategory:
    error_lower = error_msg.lower()
    if any(kw in error_lower for kw in ("timeout", "503", "502", "429", "retry")):
        return SDKErrorCategory.TRANSIENT
    if any(kw in error_lower for kw in ("401", "403", "404", "400", "422")):
        return SDKErrorCategory.CLIENT_ERROR
    return SDKErrorCategory.TRANSIENT if recoverable else SDKErrorCategory.NON_RECOVERABLE
```
##钩子注册

将分类器连接到SDK的钩子系统中：```python
def on_post_tool_use(input_data, env):
    """Append continuation hints to failed tool results."""
    tool_name = input_data.get("toolName", "")
    result = str(input_data.get("toolResult", ""))
    category = classify_tool_result(tool_name, result)
    if category in CONTINUATION_MESSAGES:
        return {"toolResult": result + CONTINUATION_MESSAGES[category]}
    return None


def on_error_occurred(input_data, env):
    """Retry transient errors, skip non-recoverable ones gracefully."""
    error_msg = input_data.get("error", "")
    recoverable = input_data.get("recoverable", False)
    category = classify_sdk_error(error_msg, recoverable)
    if category == SDKErrorCategory.TRANSIENT:
        return {"errorHandling": "retry", "retryCount": 2}
    return {
        "errorHandling": "skip",
        "userNotification": "Error occurred — continuing investigation.",
    }
```
# #提示

- **调整短语列表**为您的领域-从您的实际工具输出添加模式。
- **日志分类类别**所以你可以跟踪每一个故障模式火灾的频率，以及LLM是否实际恢复。
- **帽延续深度** -如果同一工具连续失败3次以上，让LLM放弃而不是循环。
-`SYSTEM NOTE`框架工作得很好，因为LLM将其视为权威指令而不是用户评论。

## Runnable示例

参见[`recipe/error_recovery_hooks.py`]（recipe/error_recovery_hooks.py）获得完整的工作示例。