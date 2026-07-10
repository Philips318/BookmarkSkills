@echo off
setlocal
REM ============================================================================
REM Full developer quality gate: build + instrumented test + ReSharper, storing
REM every raw output for the evaluator. Canonical replacement for the inline
REM build / dotcover test / inspectcode block in @developer Step 4. Builds once,
REM runs the suite once (instrumented - emitting tests.trx and coverage.xml),
REM then runs static analysis. Auto-resolves the *Impl.sln under Src\.
REM A non-zero exit means the CODE or TESTS failed - the commands here are fixed.
REM
REM Usage:  Build\Run-QualityGate.cmd -OutputDir .harness\tool_outputs\{slug}_{task-id}
REM ============================================================================

set "GATE_REQ_DIR=%~1"
if /i "%~1"=="-OutputDir" set "GATE_REQ_DIR=%~2"
if "%GATE_REQ_DIR%"=="" (
    echo [gate] Missing -OutputDir argument - usage: Run-QualityGate.cmd -OutputDir DIR>&2
    endlocal & exit /b 1
)

REM Run from the repo root so a relative -OutputDir resolves correctly.
pushd "%~dp0.."

call "%~dp0_GateCommon.cmd" :SetFilters
call "%~dp0_GateCommon.cmd" :ResolveImpl || goto :failed
call "%~dp0_GateCommon.cmd" :EnsureDir "%GATE_REQ_DIR%" || goto :failed

set "GATE_BUILD_LOG=%GATE_OUTDIR%\build.log"
set "GATE_COVERAGE=%GATE_OUTDIR%\coverage.xml"
set "GATE_RESHARPER=%GATE_OUTDIR%\resharper.xml"

echo [gate] Quality gate for: "%GATE_SLN%"

echo [gate] dotnet build "%GATE_SLN%"  (-^> build.log)
dotnet build "%GATE_SLN%" > "%GATE_BUILD_LOG%" 2>&1
set "GATE_BUILD_RC=%ERRORLEVEL%"
type "%GATE_BUILD_LOG%"
if not "%GATE_BUILD_RC%"=="0" goto :failed

echo [gate] dotnet dotcover test (instrumented: tests.trx + coverage.xml)
dotnet dotcover test "%GATE_SLN%" --no-build --logger "trx;LogFileName=tests.trx" --results-directory "%GATE_OUTDIR%" "--dcOutput=%GATE_COVERAGE%" --dcReportType=DetailedXML "--dcFilters=%GATE_FILTERS%"
if errorlevel 1 goto :failed

echo [gate] jb inspectcode "%GATE_SLN%"  (-^> resharper.xml)
jb inspectcode "%GATE_SLN%" "--output=%GATE_RESHARPER%"
if errorlevel 1 goto :failed

echo [gate] Quality gate complete. Outputs in: "%GATE_OUTDIR%"
popd
endlocal & exit /b 0

:failed
set "GATE_ERR=%ERRORLEVEL%"
if "%GATE_ERR%"=="0" set "GATE_ERR=1"
echo [gate] Quality gate FAILED (exit %GATE_ERR%) - fix the code/tests; the command is correct and centralized.>&2
popd
endlocal & exit /b %GATE_ERR%
