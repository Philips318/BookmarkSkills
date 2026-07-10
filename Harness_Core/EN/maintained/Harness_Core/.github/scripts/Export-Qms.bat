@echo off
setlocal

:: Prefer the repo venv Python; fall back to whatever is on PATH.
set "SCRIPT_DIR=%~dp0"
set "VENV_PY=%SCRIPT_DIR%..\..\venv\Scripts\python.exe"
if not exist "%VENV_PY%" set "VENV_PY=%SCRIPT_DIR%..\..\Scripts\python.exe"
if not exist "%VENV_PY%" set "VENV_PY=%SCRIPT_DIR%..\..\.venv\Scripts\python.exe"

if exist "%VENV_PY%" (
    set "PYTHON=%VENV_PY%"
) else (
    set "PYTHON=python"
)

"%PYTHON%" "%SCRIPT_DIR%Export-Qms.py" %*
