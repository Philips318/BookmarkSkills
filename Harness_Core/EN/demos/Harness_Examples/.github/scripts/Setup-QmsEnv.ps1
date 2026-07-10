#Requires -Version 5.1
<#
.SYNOPSIS
    Sets up the QMS Word-export toolchain from scratch (idempotent).

.DESCRIPTION
    Installs and verifies everything .github\scripts\Export-Qms.py needs, assuming a
    bare Windows machine with only winget available:

        Base interpreters (installed via winget if missing):
          - Python 3.x   (Python.Python.3.12)
          - Node.js/npm  (OpenJS.NodeJS)

        QMS export prerequisites:
          1. pandoc          (winget)              - markdown -> docx engine
          2. mermaid-cli/mmdc (npm global)         - pre-renders Mermaid diagrams to PNG
          3. python-docx     (pip into repo venv)  - cover-page merge

    Also creates the repo virtual environment (venv\) that Export-Qms.bat prefers, so
    python-docx is installed exactly where the exporter looks for it.

    Re-running is safe: anything already present is left untouched (use -Force to
    reinstall python-docx).

.PARAMETER Force
    Reinstall python-docx into the venv even if it is already importable.

.NOTES
    winget itself cannot be bootstrapped here; if it is missing the script reports
    guidance and exits. Newly installed tools may require reopening the terminal so
    PATH refreshes — the script attempts an in-session PATH refresh after each install.
#>
[CmdletBinding()]
param(
    [switch]$Force
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

# Repo root = two levels up from this script (.github\scripts\)
$RepoRoot = (Resolve-Path (Join-Path $PSScriptRoot '..\..')).Path
$VenvDir  = Join-Path $RepoRoot 'venv'
$VenvPy   = Join-Path $VenvDir 'Scripts\python.exe'

$script:Errors = New-Object System.Collections.Generic.List[string]

function Test-Tool([string]$Name) {
    return [bool](Get-Command $Name -ErrorAction SilentlyContinue)
}

function Test-PyModule([string]$PythonExe, [string]$Module) {
    # Probe whether a module imports in the given interpreter WITHOUT terminating the
    # script. A missing module makes python write a traceback to stderr and exit 1;
    # under $ErrorActionPreference='Stop' that native stderr would otherwise be raised
    # as a terminating error. Localise EAP to 'Continue' and discard all output.
    if (-not (Test-Path $PythonExe)) { return $false }
    $prevEap = $ErrorActionPreference
    $ErrorActionPreference = 'Continue'
    try {
        & $PythonExe -c "import $Module" 2>&1 | Out-Null
        return ($LASTEXITCODE -eq 0)
    } finally {
        $ErrorActionPreference = $prevEap
    }
}

function Update-SessionPath {
    # Refresh the current session's PATH from the persisted Machine + User values so
    # tools installed during this run become discoverable without reopening the shell.
    $machine = [Environment]::GetEnvironmentVariable('Path', 'Machine')
    $user    = [Environment]::GetEnvironmentVariable('Path', 'User')
    $env:Path = (@($machine, $user) | Where-Object { $_ }) -join ';'
}

function Write-Step([string]$Msg) { Write-Host "==> $Msg" -ForegroundColor Cyan }
function Write-Ok([string]$Msg)   { Write-Host "    [OK] $Msg" -ForegroundColor Green }
function Write-Skip([string]$Msg) { Write-Host "    [--] $Msg" -ForegroundColor DarkGray }
function Write-Info([string]$Msg) { Write-Host "    $Msg" }
function Write-Fail([string]$Msg) { Write-Host "    [!!] $Msg" -ForegroundColor Yellow }

function Install-WingetPackage([string]$Id, [string]$Display) {
    Write-Info "Installing $Display via winget ($Id)..."
    winget install --exact --id $Id --source winget `
        --accept-source-agreements --accept-package-agreements --disable-interactivity
    Update-SessionPath
}

# ---------------------------------------------------------------------------
# Phase 0 — winget (cannot be bootstrapped here)
# ---------------------------------------------------------------------------
Write-Step 'Checking winget'
$haveWinget = Test-Tool 'winget'
if ($haveWinget) {
    Write-Ok 'winget found'
} else {
    Write-Fail 'winget NOT found'
    $script:Errors.Add('winget is required to bootstrap from scratch. Install "App Installer" from the Microsoft Store, then re-run this script.')
}

# ---------------------------------------------------------------------------
# Phase 1 — Python interpreter
# ---------------------------------------------------------------------------
Write-Step 'Base interpreter: Python'
if (Test-Tool 'python') {
    Write-Ok ('python found: ' + (Get-Command python).Source)
} elseif ($haveWinget) {
    Install-WingetPackage 'Python.Python.3.12' 'Python 3.12'
    if (-not (Test-Tool 'python')) {
        Write-Fail 'python installed but not on PATH for this session; reopen the terminal and re-run.'
    }
} else {
    $script:Errors.Add('python missing. Install Python 3.10+ from https://www.python.org/downloads/.')
}

# ---------------------------------------------------------------------------
# Phase 2 — Node.js / npm
# ---------------------------------------------------------------------------
Write-Step 'Base interpreter: Node.js / npm'
if (Test-Tool 'npm') {
    Write-Ok ('npm found: ' + (Get-Command npm).Source)
} elseif ($haveWinget) {
    Install-WingetPackage 'OpenJS.NodeJS' 'Node.js (LTS)'
    if (-not (Test-Tool 'npm')) {
        Write-Fail 'npm installed but not on PATH for this session; reopen the terminal and re-run.'
    }
} else {
    $script:Errors.Add('npm missing. Install Node.js from https://nodejs.org/, then re-run.')
}

# ---------------------------------------------------------------------------
# Phase 3 — pandoc
# ---------------------------------------------------------------------------
Write-Step 'Prerequisite 1/3: pandoc'
if (Test-Tool 'pandoc') {
    Write-Ok ('pandoc already installed: ' + (Get-Command pandoc).Source)
} elseif ($haveWinget) {
    Install-WingetPackage 'JohnMacFarlane.Pandoc' 'pandoc'
    if (Test-Tool 'pandoc') { Write-Ok 'pandoc installed' }
    else { Write-Fail 'pandoc installed but not on PATH for this session; reopen the terminal.' }
} else {
    $script:Errors.Add('pandoc missing and winget unavailable. See https://pandoc.org/installing.html.')
}

# ---------------------------------------------------------------------------
# Phase 4 — mermaid-cli (mmdc)
# ---------------------------------------------------------------------------
Write-Step 'Prerequisite 2/3: mermaid-cli (mmdc)'
if (Test-Tool 'mmdc') {
    Write-Ok ('mmdc already installed: ' + (Get-Command mmdc).Source)
} elseif (Test-Tool 'npm') {
    Write-Info 'Installing @mermaid-js/mermaid-cli globally via npm...'
    npm install -g '@mermaid-js/mermaid-cli'
    Update-SessionPath
    if (Test-Tool 'mmdc') { Write-Ok 'mmdc installed' }
    else { Write-Fail 'mmdc installed but not on PATH for this session; reopen the terminal.' }
} else {
    $script:Errors.Add('mermaid-cli missing and npm unavailable. Install Node.js, then: npm install -g @mermaid-js/mermaid-cli.')
}

# ---------------------------------------------------------------------------
# Phase 5 — repo venv + python-docx
# ---------------------------------------------------------------------------
Write-Step 'Prerequisite 3/3: repo venv + python-docx'
if (-not (Test-Tool 'python')) {
    $script:Errors.Add('python missing; cannot create venv or install python-docx.')
} else {
    if (Test-Path $VenvPy) {
        Write-Skip "venv already exists ($VenvDir)"
    } else {
        Write-Info "Creating virtual environment at $VenvDir ..."
        python -m venv $VenvDir
    }

    if (Test-Path $VenvPy) {
        & $VenvPy -m pip install --upgrade pip --quiet

        $docxInstalled = Test-PyModule $VenvPy 'docx'

        if ($docxInstalled -and -not $Force) {
            Write-Ok 'python-docx already installed in venv'
        } else {
            Write-Info 'Installing python-docx into venv...'
            & $VenvPy -m pip install --upgrade python-docx --quiet
        }
    } else {
        $script:Errors.Add('venv creation failed; python-docx not installed.')
    }
}

# ---------------------------------------------------------------------------
# Verification
# ---------------------------------------------------------------------------
Write-Step 'Verification'
Update-SessionPath

$pandocOk = Test-Tool 'pandoc'
$mmdcOk   = Test-Tool 'mmdc'
$docxOk   = Test-PyModule $VenvPy 'docx'

function Format-Status([string]$Label, [bool]$Ok) {
    '    {0,-14} {1}' -f $Label, ($(if ($Ok) { 'OK' } else { 'MISSING' }))
}
Write-Host (Format-Status 'pandoc:'      $pandocOk)
Write-Host (Format-Status 'mmdc:'        $mmdcOk)
Write-Host (Format-Status 'python-docx:' $docxOk)

$allOk = $pandocOk -and $mmdcOk -and $docxOk -and ($script:Errors.Count -eq 0)
if (-not $allOk) {
    Write-Host ''
    Write-Host 'Setup incomplete:' -ForegroundColor Red
    foreach ($e in $script:Errors) { Write-Host "  - $e" -ForegroundColor Red }
    if (-not $pandocOk) { Write-Host '  - pandoc not detected on PATH (you may need to reopen the terminal).' -ForegroundColor Red }
    if (-not $mmdcOk)   { Write-Host '  - mmdc not detected on PATH (you may need to reopen the terminal).' -ForegroundColor Red }
    if (-not $docxOk)   { Write-Host '  - python-docx not importable in the repo venv.' -ForegroundColor Red }
    exit 1
}

Write-Host ''
Write-Host 'All QMS export prerequisites are installed.' -ForegroundColor Green
Write-Host 'Next:  .github\scripts\Export-Qms.bat docs/qms/SDD.md docs/qms/MVReport.md'
exit 0
