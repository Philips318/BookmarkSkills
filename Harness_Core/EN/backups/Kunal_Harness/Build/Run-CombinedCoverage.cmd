@echo off
setlocal
REM ============================================================================
REM Combined developer + verification coverage. Canonical replacement for the
REM inline build / dotcover test block in @dev-evaluator Section 1. Builds both
REM the implementation and verification solutions, runs both suites under
REM dotCover, and writes the combined DetailedXML report (coverage-combined.xml).
REM Auto-resolves the *Impl.sln under Src\.
REM A non-zero exit means the CODE or TESTS failed - the commands here are fixed.
REM
REM Usage:  Build\Run-CombinedCoverage.cmd -OutputDir .harness\tool_outputs\{slug}_{task-id}_eval
REM ============================================================================

set "GATE_REQ_DIR=%~1"
if /i "%~1"=="-OutputDir" set "GATE_REQ_DIR=%~2"
if "%GATE_REQ_DIR%"=="" (
    echo [gate] Missing -OutputDir argument - usage: Run-CombinedCoverage.cmd -OutputDir DIR>&2
    endlocal & exit /b 1
)

REM Run from the repo root so a relative -OutputDir resolves correctly.
pushd "%~dp0.."

call "%~dp0_GateCommon.cmd" :SetFilters
call "%~dp0_GateCommon.cmd" :ResolveImpl || goto :failed
call "%~dp0_GateCommon.cmd" :ResolveVerification || goto :failed
call "%~dp0_GateCommon.cmd" :EnsureDir "%GATE_REQ_DIR%" || goto :failed

set "GATE_COVERAGE=%GATE_OUTDIR%\coverage-combined.xml"

echo [gate] Combined coverage for: "%GATE_SLN%" + "%GATE_VSLN%"

echo [gate] dotnet build (impl + verification)
dotnet build "%GATE_SLN%" "%GATE_VSLN%"
if errorlevel 1 goto :failed

echo [gate] dotnet dotcover test (impl + verification, instrumented)
dotnet dotcover test "%GATE_SLN%" "%GATE_VSLN%" --no-build "--dcFilters=%GATE_FILTERS%" --dcReportType=DetailedXML "--dcOutput=%GATE_COVERAGE%"
if errorlevel 1 goto :failed

echo [gate] Combined coverage complete. Report: "%GATE_COVERAGE%"
popd
endlocal & exit /b 0

:failed
set "GATE_ERR=%ERRORLEVEL%"
if "%GATE_ERR%"=="0" set "GATE_ERR=1"
echo [gate] Combined coverage FAILED (exit %GATE_ERR%) - fix the code/tests; the command is correct and centralized.>&2
popd
endlocal & exit /b %GATE_ERR%
