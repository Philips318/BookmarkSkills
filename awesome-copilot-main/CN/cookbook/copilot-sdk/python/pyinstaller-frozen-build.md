#使用PyInstaller部署Copilot SDK应用程序

使用PyInstaller（或Nuitka）将Copilot SDK应用程序打包成独立的可执行文件。

# #的问题

当你用PyInstaller冻结Python SDK应用程序时，有三件事会中断：

1. **CLI二进制分辨率** - SDK通过`__file__`定位它的CLI，它指向冻结构建中的PYZ存档。
2. **SSL证书** -在macOS上，被冻结的应用无法找到系统CA证书，因此CLI子进程TLS握手失败。
3. **执行权限** -当从归档文件中提取时，捆绑的CLI二进制文件可能会丢失其`+x`位。

# #解决方案

通过搜索SDK的正常位置和PyInstaller的`_MEIPASS`临时目录来解析CLI路径。通过将`certifi`的CA bundle注入到环境中来修复SSL。在启动前恢复Unix上的执行权限。```python
"""Frozen-build compatibility for Copilot SDK applications."""
import os, sys
from pathlib import Path
from copilot import CopilotClient, SubprocessConfig


def resolve_cli_path() -> str | None:
    """Find the Copilot CLI binary in a frozen build."""
    candidates = []
    binary = "copilot.exe" if sys.platform == "win32" else "copilot"

    # 1. SDK's normal resolution
    try:
        import copilot as pkg
        candidates.append(Path(pkg.__file__).parent / "bin" / binary)
    except Exception:
        pass

    # 2. PyInstaller _MEIPASS fallback
    if getattr(sys, "frozen", False) and hasattr(sys, "_MEIPASS"):
        meipass = Path(sys._MEIPASS)
        candidates.append(meipass / "copilot" / "bin" / binary)
        candidates.append(meipass.parent / "copilot" / "bin" / binary)

    for c in candidates:
        if c.exists():
            if sys.platform != "win32" and not os.access(str(c), os.X_OK):
                os.chmod(str(c), c.stat().st_mode | 0o755)
            return str(c)
    return None


def ensure_ssl_certs():
    """Set SSL env vars for the CLI subprocess (macOS frozen builds)."""
    if os.environ.get("SSL_CERT_FILE"):
        return
    try:
        import certifi
        ca = certifi.where()
        if Path(ca).is_file():
            os.environ["SSL_CERT_FILE"] = ca
            os.environ["REQUESTS_CA_BUNDLE"] = ca
            os.environ.setdefault("NODE_EXTRA_CA_CERTS", ca)
    except ImportError:
        pass  # CLI will use platform defaults


async def create_frozen_client():
    """Create a CopilotClient that works in both normal and frozen builds."""
    ensure_ssl_certs()
    kwargs = {"log_level": "info", "use_stdio": True}
    if getattr(sys, "frozen", False):
        cli = resolve_cli_path()
        if cli:
            kwargs["cli_path"] = cli
    client = CopilotClient(SubprocessConfig(**kwargs), auto_start=True)
    await client.start()
    return client
```
## PyInstaller规范

将SDK的二进制目录包含在`.spec`文件中，以便PyInstaller捆绑它：```python
from PyInstaller.utils.hooks import collect_data_files

data += collect_data_files('copilot', include_py_files=False)
```
# #提示

- **在干净的机器上测试冻结的构建** -`_MEIPASS`提取的行为与您的开发环境不同。
- **Pin`certifi`**在您的要求，使CA包总是可用的。
- **Nuitka**使用不同的提取模型（`--include-package-data=copilot`），但相同的`resolve_cli_path`逻辑工作。

## Runnable示例

参见[`recipe/pyinstaller_frozen_build.py`]（recipe/pyinstaller_frozen_build.py）获得完整的工作示例。