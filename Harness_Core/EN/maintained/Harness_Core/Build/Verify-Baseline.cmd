@echo off
setlocal
REM ============================================================================
REM Baseline check: the implementation solution builds and all tests pass.
REM Canonical replacement for the inline "dotnet build / dotnet test --no-build"
REM baseline block (used by @developer Step 2 and the orient checklist).
REM Auto-resolves the *Impl.sln under Src\. A non-zero exit means the CODE or
REM TESTS failed - the commands here are correct and fixed; fix the code/tests.
REM
REM Usage:  Build\Verify-Baseline.cmd
REM ============================================================================

call "%~dp0_GateCommon.cmd" :ResolveImpl || goto :failed

echo [gate] Baseline verification for: "%GATE_SLN%"

echo [gate] dotnet build "%GATE_SLN%"
dotnet build "%GATE_SLN%"
if errorlevel 1 goto :failed

echo [gate] dotnet test "%GATE_SLN%" --no-build
dotnet test "%GATE_SLN%" --no-build
if errorlevel 1 goto :failed

echo [gate] Baseline OK: build clean and all tests passed.
endlocal & exit /b 0

:failed
set "GATE_ERR=%ERRORLEVEL%"
if "%GATE_ERR%"=="0" set "GATE_ERR=1"
echo [gate] Baseline FAILED (exit %GATE_ERR%) - fix the code/tests; the command is correct and centralized.>&2
endlocal & exit /b %GATE_ERR%
