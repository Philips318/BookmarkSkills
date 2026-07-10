@echo off
REM ============================================================================
REM Shared helpers for the harness quality-gate scripts (pure batch - no ps1,
REM so there is no execution-policy / publisher-trust friction at runtime).
REM
REM Called by Verify-Baseline / Run-QualityGate / Build-VerificationTests /
REM Run-CombinedCoverage as:   call "%~dp0_GateCommon.cmd" :<Routine> [arg]
REM
REM Routines (each sets vars in the CALLER's environment; exit 0 ok / 1 failure):
REM   :SetFilters           -> GATE_FILTERS  (canonical dotCover filter)
REM   :ResolveImpl          -> GATE_SLN      (the single *Impl.sln under Src\)
REM   :ResolveVerification  -> GATE_VSLN     (Src\VerificationTests\VerificationTests.sln)
REM   :EnsureDir "<dir>"    -> GATE_OUTDIR   (created if missing, absolute path)
REM
REM A non-zero exit from a gate means the CODE or TESTS failed - the commands are
REM correct and centralized here; fix the code/tests, not the invocation.
REM ============================================================================
if "%~1"=="" exit /b 0
goto %~1

:SetFilters
REM Canonical dotCover filter - identical for every task; never re-typed by an agent.
set "GATE_FILTERS=+:Philips.CT.*;-:*.Test*"
exit /b 0

:ResolveImpl
REM Auto-locate the single implementation solution (*Impl.sln) under Src\.
REM This removes the {repo} placeholder guesswork that caused retries.
for %%I in ("%~dp0..") do set "GATE_ROOT=%%~fI"
if not exist "%GATE_ROOT%\Src" (
    echo [gate] Src directory not found at "%GATE_ROOT%\Src". Is this a populated repository?>&2
    exit /b 1
)
set "GATE_SLN="
set "GATE_COUNT=0"
for /r "%GATE_ROOT%\Src" %%F in (*Impl.sln) do (
    set "GATE_SLN=%%F"
    set /a GATE_COUNT+=1
)
if "%GATE_COUNT%"=="0" (
    echo [gate] No *Impl.sln found under "%GATE_ROOT%\Src". Cannot run the quality gate.>&2
    exit /b 1
)
if %GATE_COUNT% GTR 1 (
    echo [gate] Expected exactly one *Impl.sln under "%GATE_ROOT%\Src" but found %GATE_COUNT%. Disambiguate before running the gate.>&2
    exit /b 1
)
exit /b 0

:ResolveVerification
REM Return the verification-tests solution path; fail clearly if it is absent.
for %%I in ("%~dp0..") do set "GATE_ROOT=%%~fI"
set "GATE_VSLN=%GATE_ROOT%\Src\VerificationTests\VerificationTests.sln"
if not exist "%GATE_VSLN%" (
    echo [gate] VerificationTests.sln not found at "%GATE_VSLN%". The test-designer must create it first.>&2
    exit /b 1
)
exit /b 0

:EnsureDir
if "%~2"=="" (
    echo [gate] EnsureDir requires a directory argument.>&2
    exit /b 1
)
if not exist "%~2" mkdir "%~2"
for %%I in ("%~2") do set "GATE_OUTDIR=%%~fI"
exit /b 0
