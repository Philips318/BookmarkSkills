@echo off
:: ---------------------------------------------------------------------------
:: Bootstraps the QMS Word-export toolchain from scratch (idempotent):
::   - Python + Node.js/npm (via winget, if missing)
::   - pandoc (winget), mermaid-cli/mmdc (npm), python-docx (pip into repo venv)
:: Thin wrapper so the setup script is callable from cmd without execution-policy
:: friction, mirroring Export-Qms.bat.  Pass -Force to reinstall python-docx.
:: ---------------------------------------------------------------------------
setlocal
set "SCRIPT_DIR=%~dp0"
powershell -NoProfile -ExecutionPolicy Bypass -File "%SCRIPT_DIR%Setup-QmsEnv.ps1" %*
exit /b %ERRORLEVEL%
