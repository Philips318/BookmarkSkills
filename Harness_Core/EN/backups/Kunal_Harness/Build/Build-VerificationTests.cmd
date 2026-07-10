@echo off
setlocal
REM ============================================================================
REM Build the verification-tests solution and run ReSharper, storing raw outputs.
REM Canonical replacement for the inline build / inspectcode block in
REM @test-designer Step 4. Verification tests are held to the same bar as
REM production code: build clean and zero ReSharper errors.
REM A non-zero exit means the CODE or TESTS failed - the commands here are fixed.
REM
REM Usage:  Build\Build-VerificationTests.cmd -OutputDir .harness\tool_outputs\{slug}_{task-id}_verification
REM ============================================================================

set "GATE_REQ_DIR=%~1"
if /i "%~1"=="-OutputDir" set "GATE_REQ_DIR=%~2"
if "%GATE_REQ_DIR%"=="" (
    echo [gate] Missing -OutputDir argument - usage: Build-VerificationTests.cmd -OutputDir DIR>&2
    endlocal & exit /b 1
)

REM Run from the repo root so a relative -OutputDir resolves correctly.
pushd "%~dp0.."

call "%~dp0_GateCommon.cmd" :ResolveVerification || goto :failed
call "%~dp0_GateCommon.cmd" :EnsureDir "%GATE_REQ_DIR%" || goto :failed

set "GATE_BUILD_LOG=%GATE_OUTDIR%\build.log"
set "GATE_RESHARPER=%GATE_OUTDIR%\resharper.xml"

echo [gate] Verification-tests gate for: "%GATE_VSLN%"

echo [gate] dotnet build "%GATE_VSLN%"  (-^> build.log)
dotnet build "%GATE_VSLN%" > "%GATE_BUILD_LOG%" 2>&1
set "GATE_BUILD_RC=%ERRORLEVEL%"
type "%GATE_BUILD_LOG%"
if not "%GATE_BUILD_RC%"=="0" goto :failed

echo [gate] jb inspectcode "%GATE_VSLN%"  (-^> resharper.xml)
jb inspectcode "%GATE_VSLN%" "--output=%GATE_RESHARPER%"
if errorlevel 1 goto :failed

echo [gate] Verification-tests gate complete. Outputs in: "%GATE_OUTDIR%"
popd
endlocal & exit /b 0

:failed
set "GATE_ERR=%ERRORLEVEL%"
if "%GATE_ERR%"=="0" set "GATE_ERR=1"
echo [gate] Verification-tests gate FAILED (exit %GATE_ERR%) - fix the code/tests; the command is correct and centralized.>&2
popd
endlocal & exit /b %GATE_ERR%
